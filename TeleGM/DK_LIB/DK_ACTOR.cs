using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GmTelematics
{
    class DK_ACTOR
    {
        public event EventDKCOM ActorSendReport;
        public event EventRealTimeMsg ActorSendReport2;
        public event EventSensorDKCOM ActorSendReport3;

        //private readonly int FIXCOUNT;
        private DK_COMM[] DKComm;

        public DK_ACTOR(int iCOMSERIAL_ENUM, string strTBLName)
        {
            
            DKComm = new DK_COMM[(int)COMSERIAL.END];

            DKComm[(int)COMSERIAL.DIO]      = new DK_COMM("DIO");
            DKComm[(int)COMSERIAL.UART2]    = new DK_COMM("UART2");
            DKComm[(int)COMSERIAL.SET]      = new DK_COMM("SET");
            DKComm[(int)COMSERIAL.SCANNER]  = new DK_COMM("SCANNER");
            DKComm[(int)COMSERIAL.TC3000]   = new DK_COMM("TC3000");
            DKComm[(int)COMSERIAL.AUDIOSEL] = new DK_COMM("AUDIOSELECTOR");
            DKComm[(int)COMSERIAL.ADC]      = new DK_COMM("ADC");
            DKComm[(int)COMSERIAL.CCM]      = new DK_COMM("CCM");
            DKComm[(int)COMSERIAL.ODAPWR]   = new DK_COMM("ODAPWR");
            DKComm[(int)COMSERIAL.DIO].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.DIO].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.DIO].CommSendReport2 += new EventSensorDKCOM(GateWay_ACTOR3);
            DKComm[(int)COMSERIAL.SET].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.SET].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.UART2].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.UART2].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.TC3000].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.TC3000].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);            
            DKComm[(int)COMSERIAL.SCANNER].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.SCANNER].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.AUDIOSEL].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.AUDIOSEL].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.ADC].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.ADC].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.CCM].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.CCM].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
            DKComm[(int)COMSERIAL.ODAPWR].CommSendReport += new EventDKCOM(GateWay_ACTOR);
            DKComm[(int)COMSERIAL.ODAPWR].CommRealTimeTxRxMsg += new EventRealTimeMsg(GateWay_ACTOR2);
        }  

#region COM 제어부분

        public bool CommHandShake(int iCOMSERIAL_ENUM, int iHandShake)
        {
            return DKComm[iCOMSERIAL_ENUM].ChangeHandShake(iHandShake);
        }
        public void CommOff(int iCOMSERIAL_ENUM)
        {
            DKComm[iCOMSERIAL_ENUM].PortClose();            
        }

        public bool CommOpen(int iCOMSERIAL_ENUM, string PortNumber, int iBaudrate)
        {
            bool bFlag = false;

            switch (iCOMSERIAL_ENUM)
            {
                case (int)COMSERIAL.DIO:
                case (int)COMSERIAL.AUDIOSEL:
                case (int)COMSERIAL.ADC:      bFlag = true; break; //DIO 만 true, 버젼확인을 위해서.

                case (int)COMSERIAL.UART2: 
                default: break;

            }
          
            bool rtnRes = DKComm[iCOMSERIAL_ENUM].PortOpen(PortNumber, iBaudrate, bFlag);

            if (rtnRes && iCOMSERIAL_ENUM == (int)COMSERIAL.TC3000)
            {
                for (int i = 0; i < 3; i++)
                {
                    int iTc3000 = DirectSendRecvCmd((int)COMSERIAL.TC3000, "EXE_RESET\n", 3, (int)MODE.SENDRECV, 0, (int)RS232.TEXT, "EXE_RESET", "");
                    if (iTc3000.Equals((int)STATUS.OK))
                    {
                        return true;
                    }
                }
                return false;
            }

            return rtnRes;

        }

        public void DefaultSetBaudrate()
        {
            DKComm[(int)COMSERIAL.SET].DefaultBaudrate();
        }

        public void ChangeBaudrate(int iBaudrate, int iTarget)
        {
            DKComm[iTarget].ChangeBaudRate(iBaudrate);
        }

        public bool ChangeComPort(string strPortName, int iTarget)
        {
            return DKComm[iTarget].ChangeComPort(strPortName);
        }

        public void DefaultComPort(int iTarget)
        {
            DKComm[iTarget].DefaultComPort();
        }

        public ThreadStatus GetStatus(int iCOMSERIAL_ENUM)
        {
            return DKComm[iCOMSERIAL_ENUM].GetStatus();
        }

        public bool isLive(int iCOMSERIAL_ENUM)
        {
            return DKComm[iCOMSERIAL_ENUM].IsPortOpen();
        }

        public void ClearComBuffer(int iCOMSERIAL_ENUM)
        {
            DKComm[iCOMSERIAL_ENUM].ClearBuffer();
        }

        public bool IsWorking(int iCOMSERIAL_ENUM)
        {
            return DKComm[iCOMSERIAL_ENUM].IsWorking();
        }

        public void InsertDioCommand(int iCOMSERIAL_ENUM, string strPack, double dTimeout, int iSendRecvOption, double dDelaytime, int iSerialType, string strCommandTBLname, string strParam, AnalyizePack anlPack)
        {

            DKComm[iCOMSERIAL_ENUM].InsertDioCommand(strPack, dTimeout, iSendRecvOption, dDelaytime, iSerialType, strCommandTBLname, strParam, anlPack);
        }


        public void SendRecvCmd(int iCOMSERIAL_ENUM, string strPack, double dTimeout, int iSendRecvOption, double dDelaytime, int iSerialType, string strCommandTBLname, string strParam, AnalyizePack anlPack)
        {
            //if (!isLive(iCOMSERIAL_ENUM) || DKComm[iCOMSERIAL_ENUM].IsPortRunning()) return;
            /*
            if (STEPMANAGER_VALUE.bInteractiveMode) //인터렉티브모드일땐 딜레이 무시
            {
                dDelaytime = 0.0;
            }
             * */
            DKComm[iCOMSERIAL_ENUM].SendRecv(strPack, dTimeout, iSendRecvOption, dDelaytime, iSerialType, strCommandTBLname, strParam, anlPack);
        }

        public int DirectSendRecvCmd(int iCOMSERIAL_ENUM, string strPack, double dTimeout, int iSendRecvOption, double dDelaytime, int iSerialType, string strCommandTBLname, string strParam)
        {
            //if (!isLive(iCOMSERIAL_ENUM) || DKComm[iCOMSERIAL_ENUM].IsPortRunning()) return;

            return DKComm[iCOMSERIAL_ENUM].DirectSendRecv(strPack, dTimeout, iSendRecvOption, dDelaytime, iSerialType, strCommandTBLname, strParam);
        }
        
        public bool isRunnig(int iCOMSERIAL_ENUM)
        {
            return DKComm[iCOMSERIAL_ENUM].IsPortRunning();
        }

        public bool isScanning(int iCOMSERIAL_ENUM)
        {
            return DKComm[iCOMSERIAL_ENUM].IsScanning();
        }

        public void runningAllStop()
        {
            try
            {
                for (int i = (int)COMSERIAL.DIO; i < (int)COMSERIAL.END; i++)
                {
                    if (DKComm[i] != null && DKComm[i].IsPortOpen())
                        DKComm[i].PortRunningStop();
                }
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
            
        }

        public void runningStop(int iCOMSERIAL_ENUM)
        {
            DKComm[iCOMSERIAL_ENUM].PortRunningStop();
        }

        private void GateWay_ACTOR(COMMDATA cData)
        {
            ActorSendReport(cData);
        }

        private void GateWay_ACTOR2(int iPort, string cParam)
        {
            ActorSendReport2(iPort, cParam);
        }

        private void GateWay_ACTOR3(bool[] rData)
        {
            ActorSendReport3(rData);
        }
#endregion

    }
}
