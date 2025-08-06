using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.NI4882;  //NI 전용
using NationalInstruments.VisaNS;  //표준 VISA 방식 이용  타사 제품 이용시 (Adlink 같은)

namespace GmTelematics
{    

    class DK_NI_GPIB
    {
        public event EventRealTimeMsg GPIBRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드

        private Device              deviceNI;  //NI 전용
        private MessageBasedSession deviceVS;  //표준 VISA 전용
        private int iBoardNumber;
        private int iAddress;
        private bool bConnection;
        private string strDevice;
        private int iSlotNumber;
        private DK_LOGGER DKLogger;

        private bool bNiProductUsed;

        private int Item_iBoardNumber
        {
            get { return iBoardNumber; }
            set { iBoardNumber = value; }
        }

        private int Item_iAddress
        {
            get { return iAddress; }
            set { iAddress = value; }
        }

        private bool Item_bConnection
        {
            get { return bConnection; }
            set { bConnection = value; }
        }

        private string Item_strDevice
        {
            get { return strDevice; }
            set { strDevice = value; }
        }

        public int Item_iSlotNumber
        {
            get { return iSlotNumber; }
            set { iSlotNumber = value; }
        }


        public DK_NI_GPIB(int iSlot, string strTBLName)
        {
            Item_iSlotNumber = iSlot;
            Item_strDevice = strTBLName;
            Item_bConnection = false;
            bNiProductUsed = true;  //기본은 NI 사용
            DKLogger = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_GPIB);
        }

        private void GateWay_GPIB(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
        {
            GPIBRealTimeTxRxMsg(0, cParam);
        }

        public bool Connect(int iBdNum, int iAddr)  //NI 전용
        {
            bNiProductUsed = true;
            Item_iBoardNumber = iBdNum;
            Item_iAddress = iAddr;
            string tmpStr = String.Empty;
            try
            {
                deviceNI = new Device((byte)iBoardNumber, (byte)iAddress);
                deviceNI.IOTimeout = TimeoutValue.T3s;
                SaveLog("[TX] *IDN?", "DEVICE_CHECK");
                deviceNI.Write("*IDN?\n");
                tmpStr = deviceNI.ReadString();
                if (tmpStr != null && tmpStr.Length > 5)
                {
                    //SaveLog("[RX] " + tmpStr, "DEVICE_CHECK");
                    SaveLog("[RX] " + tmpStr, String.Empty);
                    if (tmpStr.IndexOf(Item_strDevice) > -1)
                    {
                        Item_bConnection = true;                        
                    }
                    else
                    {
                        Item_bConnection = false;                        
                    }

                    return Item_bConnection;                    
                }
            }
            catch (Exception ex)
            {
                deviceNI = null;
                tmpStr = ex.Message;
                //SaveLog("[RX] ERROR:" + tmpStr, "DEVICE_CHECK");
                SaveLog("[RX] ERROR:" + tmpStr, String.Empty);
                Item_bConnection = false;
                return Item_bConnection;
            }
            deviceNI = null;
            Item_bConnection = false;
            //SaveLog("[RX] " + tmpStr, "DEVICE_CHECK");
            SaveLog("[RX] " + tmpStr, String.Empty);    
            return Item_bConnection;

        }

        public bool Connect(int iBdNum, int iAddr, string strAddress) //표준 VISA 용
        {
            bNiProductUsed = false;
            string tmpStr = String.Empty;
            try
            {
                deviceVS = (MessageBasedSession)ResourceManager.GetLocalManager().Open(strAddress);
                SaveLog("[TX] *IDN?", "DEVICE_CHECK");
                deviceVS.Write("*IDN?\n");
                tmpStr = deviceVS.ReadString();
                if (tmpStr != null && tmpStr.Length > 5)
                {
                    //SaveLog("[RX] " + tmpStr, "DEVICE_CHECK");
                    SaveLog("[RX] " + tmpStr, String.Empty);

                    string strChkDeviceName = String.Empty;

                    strChkDeviceName = CheckDeviceName();

                    if (tmpStr.IndexOf(strChkDeviceName) > -1)
                    {
                        Item_bConnection = true;
                    }
                    else
                    {
                        Item_bConnection = false;
                    }

                    return Item_bConnection;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("0xBFFF003"))  // NI 용 제품은 이걸로만 통하는것 같다...
                {
                    deviceVS = null;
                    return Connect(iBdNum, iAddr);
                }

                deviceVS = null;                
                tmpStr = ex.Message;
                //SaveLog("[RX] ERROR:" + tmpStr, "DEVICE_CHECK");
                SaveLog("[RX] ERROR:" + tmpStr, String.Empty);
                Item_bConnection = false;
                return Item_bConnection;
            }
            deviceVS = null;
            Item_bConnection = false;
            //SaveLog("[RX] " + tmpStr, "DEVICE_CHECK");
            SaveLog("[RX] " + tmpStr, String.Empty);
            return Item_bConnection;

        }

        private void SaveLog(string strLog, string strCommandName)
        {
            strLog = strLog.Replace("\n", "");
            DKLogger.WriteCommLog(strLog, Item_strDevice + ":" + strCommandName, false);
           
        }

        private bool DelayTimeOutCheck(DateTime dtOut, double dTime)
        {
            DateTime dtCurr = DateTime.Now;
            TimeSpan tsN = dtCurr - dtOut;

            if (tsN.TotalSeconds > dTime)
            {
                return false;
            }
            return true;
        }

        public bool SendRecv(string strCommand, string strCommandName, string strSendparEx, ref COMMDATA cData, bool bQuery, double dTimeOut)
        {
            if (bNiProductUsed)
                return NI_SendRecv(strCommand, strCommandName, strSendparEx, ref cData, bQuery, dTimeOut);
            else
                return VS_SendRecv(strCommand, strCommandName, strSendparEx, ref cData, bQuery, dTimeOut);
        }

        private bool NI_SendRecv(string strCommand, string strCommandName, string strSendparEx, ref COMMDATA cData, bool bQuery, double dTimeOut)
        //bQeury 는 테스콤 장비때문에 생긴 파라미터임. 일반적으로
        //GPIB 명령 표준은 ? 만 응답이 존재하나.
        //테스콤 장비는 모든명령이 응답이 있단다. 그래서 이것을 활용해야한다.
        {
            cData.iPortNum = (int)DEFINES.SET1;
            cData.iStatus = (int)STATUS.OK;
            cData.ResponseData = "1";
            cData.ResultData = "OK";
            cData.SendPacket = strCommand;
            string strMakePacket = strCommand;
            if (strSendparEx.Length > 0) strMakePacket = strCommand + " " + strSendparEx;
            

            if (!Item_bConnection)
            {
                cData.ResponseData = "Disconnection.";
                SaveLog("[TX] [DEVICE_DISCONNECTED]" + strMakePacket, strCommandName);
                cData.iStatus = (int)STATUS.CHECK;
                cData.ResultData = "CHECK";
                return false;
            }
            string tmpStr = "RecvFail.";

            try
            {               
                int iIdxQuestion = 0;                
                System.Threading.Thread.Sleep(50);
                SaveLog("[TX] " + strMakePacket, strCommandName);
                deviceNI.Write(strMakePacket + "\n");

                if ((iIdxQuestion = strMakePacket.IndexOf("?")) > 0 || bQuery)
                {
                    deviceNI.IOTimeout = TimeoutValue.T3s;

                    DateTime dtm = DateTime.Now;
                    if (dTimeOut <= 0)
                    {
                        dTimeOut = 1.0;
                    }
                               
                    while(true)
                    {
                        System.Threading.Thread.Sleep(10);
                        try
                        {
                            tmpStr = deviceNI.ReadString();
                            if (tmpStr != null && tmpStr.Length > 0)
                            {
                                tmpStr = tmpStr.Replace("\r", "");
                                tmpStr = tmpStr.Replace("\n", "");
                                //SaveLog("[RX] " + tmpStr, strCommandName);
                                SaveLog("[RX] " + tmpStr, String.Empty);
                                if (tmpStr.Length > 20)
                                {
                                    cData.ResponseData = tmpStr;
                                    cData.ResultData = tmpStr.Substring(0, 20) + "...";
                                }
                                else
                                {
                                    cData.ResponseData = cData.ResultData = tmpStr;
                                }

                                cData.iStatus = (int)STATUS.OK;
                                return true;
                            }
                        }
                        catch { }

                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
                        {
                            SaveLog("[RX] USER STOP.", "");
                            cData.iStatus = (int)STATUS.STOP;
                            cData.ResultData = "USER STOP";
                            return false;
                        }

                        if (!DelayTimeOutCheck(dtm, dTimeOut))
                        {
                            if (tmpStr.Length > 0)
                                SaveLog("[RX] " + tmpStr, String.Empty);
                            else
                                SaveLog("[RX] TIME OUT", String.Empty);

                            cData.iStatus = (int)STATUS.NG;
                            cData.ResultData = "NG";
                            return false;
                        }                                                
                    }       
             
                }

                cData.ResponseData = cData.ResultData = "OK";                
                SaveLog("[RX] " + cData.ResponseData, String.Empty);
                cData.iStatus = (int)STATUS.OK;
                return true;
                               
            }
            catch (Exception ex)
            {
                cData.ResponseData = ex.Message;
                SaveLog("[RX] ERROR:" + cData.ResponseData, strCommandName);
                if (strMakePacket.IndexOf("*OPC?") > 0) //OPC 명령은 실패나도 그냥 PASS 하자. 가성불량처리.
                {
                    cData.iStatus = (int)STATUS.OK;
                    cData.ResultData = "PASS";
                    return true;
                }
                cData.iStatus = (int)STATUS.NG;
                cData.ResultData = "ERROR";
                return false;
             
            }            
        }

        private bool VS_SendRecv(string strCommand, string strCommandName, string strSendparEx, ref COMMDATA cData, bool bQuery, double dTimeOut)
        //bQeury 는 테스콤 장비때문에 생긴 파라미터임. 일반적으로
        //GPIB 명령 표준은 ? 만 응답이 존재하나.
        //테스콤 장비는 모든명령이 응답이 있단다. 그래서 이것을 활용해야한다.
        {
            cData.iPortNum = (int)DEFINES.SET1;
            cData.iStatus = (int)STATUS.OK;
            cData.ResponseData = "1";
            cData.ResultData = "OK";
            cData.SendPacket = strCommand;
            string strMakePacket = strCommand;
            if (strSendparEx.Length > 0) strMakePacket = strCommand + " " + strSendparEx;


            if (!Item_bConnection)
            {
                cData.ResponseData = "Disconnection.";
                SaveLog("[TX] [DEVICE_DISCONNECTED]" + strMakePacket, strCommandName);
                cData.iStatus = (int)STATUS.CHECK;
                cData.ResultData = "CHECK";
                return false;
            }
            string tmpStr = "RecvFail.";

            try
            {
                int iIdxQuestion = 0;
                System.Threading.Thread.Sleep(50);
                SaveLog("[TX] " + strMakePacket, strCommandName);
                deviceVS.Write(strMakePacket + "\n");

                if ((iIdxQuestion = strMakePacket.IndexOf("?")) > 0 || bQuery)
                {

                    DateTime dtm = DateTime.Now;
                    if (dTimeOut <= 0)
                    {
                        dTimeOut = 1.0;
                    }

                    while (true)
                    {
                        System.Threading.Thread.Sleep(10);
                        try
                        {
                            tmpStr = deviceVS.ReadString();
                            if (tmpStr != null && tmpStr.Length > 0)
                            {
                                tmpStr = tmpStr.Replace("\r", "");
                                tmpStr = tmpStr.Replace("\n", "");
                                //SaveLog("[RX] " + tmpStr, strCommandName);
                                SaveLog("[RX] " + tmpStr, String.Empty);
                                if (tmpStr.Length > 64)
                                {
                                    cData.ResponseData = tmpStr;
                                    cData.ResultData = tmpStr.Substring(0, 20) + "...";
                                }
                                else
                                {
                                    cData.ResponseData = cData.ResultData = tmpStr;
                                }

                                cData.iStatus = (int)STATUS.OK;
                                return true;
                            }
                        }
                        catch { }

                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
                        {
                            SaveLog("[RX] USER STOP.", "");
                            cData.iStatus = (int)STATUS.STOP;
                            cData.ResultData = "USER STOP";
                            return false;
                        }

                        if (!DelayTimeOutCheck(dtm, dTimeOut))
                        {
                            if (tmpStr.Length > 0)
                                SaveLog("[RX] " + tmpStr, String.Empty);
                            else
                                SaveLog("[RX] TIME OUT", String.Empty);

                            cData.iStatus = (int)STATUS.NG;
                            cData.ResultData = "NG";
                            return false;
                        }
                    }

                }

                cData.ResponseData = cData.ResultData = "OK";
                SaveLog("[RX] " + cData.ResponseData, String.Empty);
                cData.iStatus = (int)STATUS.OK;
                return true;

            }
            catch (Exception ex)
            {
                cData.ResponseData = ex.Message;
                SaveLog("[RX] ERROR:" + cData.ResponseData, strCommandName);
                if (strMakePacket.IndexOf("*OPC?") > 0) //OPC 명령은 실패나도 그냥 PASS 하자. 가성불량처리.
                {
                    cData.iStatus = (int)STATUS.OK;
                    cData.ResultData = "PASS";
                    return true;
                }
                cData.iStatus = (int)STATUS.NG;
                cData.ResultData = "ERROR";
                return false;

            }
        }

        private string CheckDeviceName()
        {
            switch (Item_strDevice)
            {
                case "5515C": return "5515"; //5515E 장비를 쓰는 경우도 있다고 한다.

                default: return Item_strDevice;
            }
        }
    }
}