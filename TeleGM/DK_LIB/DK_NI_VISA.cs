using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.VisaNS;

namespace GmTelematics
{

    class DK_NI_VISA
    {
        public event EventRealTimeMsg VisaRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드

        private MessageBasedSession device; 
        private bool bConnection;
        private string strDevice;
        private int iSlotNumber;
        private DK_LOGGER DKLogger;

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


        public DK_NI_VISA(int iSlot, string strTBLName)
        {
            Item_iSlotNumber = iSlot;
            Item_strDevice = strTBLName;
            Item_bConnection = false;
            DKLogger = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_GPIB);
        }

        private void GateWay_GPIB(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
        {
            VisaRealTimeTxRxMsg(0, cParam);
        }

        public bool Connect(string strAddress)
        {      
            string tmpStr = String.Empty;
            try
            {
                //device = (MessageBasedSession)ResourceManager.GetLocalManager().Open(strAddress);
                Close();
                //device = (MessageBasedSession)ResourceManager.GetLocalManager().Open("USB0::0x0957::0x0607::MY53014952::INSTR");

                SaveLog("[TX] " + "*IDN?", "DEVICE_CHECK");

                device = (MessageBasedSession)ResourceManager.GetLocalManager().Open(strAddress);        
                tmpStr = device.Query("*IDN?\n");
                
                if (tmpStr != null && tmpStr.Length > 5)
                {
                    SaveLog("[RX] " + tmpStr, "DEVICE_CHECK");
                    if (tmpStr.IndexOf(Item_strDevice) > -1 || tmpStr.Contains("34465A"))
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
                device = null;
                tmpStr = ex.Message;
                SaveLog("[RX] ADDRESS ERROR -> " + strAddress, "DEVICE_CHECK");
                Item_bConnection = false;
                return Item_bConnection;
            }
            device = null;
            Item_bConnection = false;
            SaveLog("[RX] " + tmpStr, "DEVICE_CHECK");        
            return Item_bConnection;

        }

        public void Close()
        {
            if (device != null)
            {
                device.Dispose();
            }
        }

        private void SaveLog(string strLog, string strCommandName)
        {
            strLog = strLog.Replace("\n", "");
            DKLogger.WriteCommLog(strLog, Item_strDevice + ":" + strCommandName, false);
           
        }

        public bool DirectSendRecv(string strCommand, ref string strResData)
        {           
            string strMakePacket = strCommand;
            
            if (!Item_bConnection)
            {
                strResData = "0";
                return false;                
            }
            string tmpStr = String.Empty;

            try
            {
                device.Write(strMakePacket + "\n");
                System.Threading.Thread.Sleep(10);
                if ((strMakePacket.Contains("?")))
                {
                    try
                    {
                        tmpStr = device.ReadString();
                        if (tmpStr != null && tmpStr.Length > 0 && tmpStr.Contains("\n"))
                        {
                            tmpStr = tmpStr.Replace("\r", String.Empty);
                            tmpStr = tmpStr.Replace("\n", String.Empty);
                            strResData = tmpStr;
                            return true;
                        }
                    }
                    catch { }                  

                }
                strResData = "0";                
                return true;

            }
            catch
            {
                strResData = "ERROR";
                return false;
            }
        }

        public bool SendRecv(string strCommand, string strCommandName, string strSendparEx, ref COMMDATA cData)
        {
            cData.iPortNum = (int)DEFINES.SET1;
            cData.iStatus = (int)STATUS.OK;
            cData.ResponseData = "";
            cData.ResultData = "";
            cData.SendPacket = strCommand;
            string strMakePacket = strCommand;
            if (strSendparEx.Length > 0) strMakePacket = strCommand + " " + strSendparEx;
            
            if (!Item_bConnection)
            {
                cData.ResponseData = "Disconnection.";
                SaveLog("[TX] " + "[DEVICE_DISCONNECTED]" + strMakePacket, strCommandName);
                cData.iStatus = (int)STATUS.CHECK;                
                return false;
            }
            string tmpStr = String.Empty;
            try                
            {
                int iIdxQuestion = 0;     
                SaveLog("[TX] " + strMakePacket, strCommandName); 
                device.Write(strMakePacket + "\n");
                System.Threading.Thread.Sleep(10);
                if ((iIdxQuestion = strMakePacket.IndexOf("?")) > 0)
                {   
                    try
                    {
                        tmpStr = device.ReadString();
                        if (tmpStr != null && tmpStr.Length > 0 && tmpStr.Contains("\n"))
                        {
                            tmpStr = tmpStr.Replace("\r", String.Empty);
                            tmpStr = tmpStr.Replace("\n", String.Empty);
                            SaveLog("[RX] " + tmpStr, strCommandName);
                            cData.ResponseData = tmpStr;
                            cData.ResultData = tmpStr;
                            cData.iStatus = (int)STATUS.OK;
                            return true;
                        }
                    } //
                        
                    catch { }
                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
                    {
                        SaveLog("[RX] " + "USER STOP.", "");
                        cData.iStatus = (int)STATUS.STOP;
                        return false;
                    }
                        
                    SaveLog("[RX] " + tmpStr, strCommandName);
                    cData.iStatus = (int)STATUS.NG;
                    return false;                    
                    
                }
                cData.ResponseData = "OK";
                SaveLog("[RX] " + cData.ResponseData, strCommandName);
                cData.iStatus = (int)STATUS.OK;
                return true;
                               
            }
            catch (Exception ex)
            {
                cData.ResponseData = cData.ResultData = ex.Message;
                SaveLog("[RX] ERROR:" + cData.ResponseData, strCommandName);
                cData.iStatus = (int)STATUS.NG;
                return false;
             
            }            
        }

    }
}