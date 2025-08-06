using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vxlapi_NET;
using System.Threading;
using System.Runtime.InteropServices;


namespace GmTelematics
{
    struct SharedVectorData
    {
        public bool bResult;
        public bool bSuccess;
        public XLClass.xl_can_msg xlMsg;
        public string strMessage;
        public uint   uMsgCode;
        public string strCommandName;

        public bool bExtended;
        public int iSendRecvOption;
    }

    enum VectorBaudrate
    {    
        BAUD_33K  = 33333,
        BAUD_100K = 100000,
        BAUD_125K = 125000,
        BAUD_500K = 500000,
        BAUD_1M   = 1000000
    }

    class DK_VECTOR_BASIC
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(int handle, int timeOut);

        public event EventRealTimeMsg VectorRealTimeTxRxMsg;      //실시간로그
        
        private DK_LOGGER DKLogger;

        private ManualResetEvent ThreadEvent_Pause = new ManualResetEvent(false);
        private ManualResetEvent ThreadEvent_Off = new ManualResetEvent(false);

        private ManualResetEvent ThreadSendRecv = new ManualResetEvent(false);
        private ManualResetEvent ThreadEvtLock = new ManualResetEvent(false);


        private SharedVectorData srData = new SharedVectorData();
        private Thread RecvThread;   //일반 recv 전용
        private Thread SendThread;   //자동 Send 전용
        
        private object lockobjectLog;
        private object lockobjectWrite;
        private object lockObject2;

        private bool bConnection = false;

        //private DateTime dtOutSet;
        //private DateTime dtCurrTime;
        private double dCycleTime = 100;

        private System.Diagnostics.Stopwatch swDelayTimeChecker = new System.Diagnostics.Stopwatch();

        private double dTimeSet;
        private double Item_dTimeSet
        {
            get { return dTimeSet; }
            set { dTimeSet = value; }
        }

#region Vector Structures
        private XLClass.xl_event_collection xlSendDataPack;  //보낼 명령
        // Driver access through XLDriver (wrapper)
        private XLDriver myApp;// = new XLDriver();        // Driver configuration
        private XLClass.xl_driver_config driverConfig = new XLClass.xl_driver_config();
        // application name.
        private string appName = "xlCANdemoNET";
        // Variables required by XLDriver
        private XLDefine.XL_HardwareType hwType = XLDefine.XL_HardwareType.XL_HWTYPE_NONE;
        private uint hwIndex = 0;
        private uint hwChannel = 0;
        private int  portHandle = -1;
        private int eventHandle = -1;
        private UInt64 accessMask = 0;
        private UInt64 permissionMask = 0;
        private UInt64 txMask = 0;
        private uint uBaudrate = (uint)VectorBaudrate.BAUD_125K;
#endregion

        private void GateWay_VECTOR(string cParam) //로깅할때 데이터를 UI 로 manager 로 보내자.
        {
            VectorRealTimeTxRxMsg(0, cParam);            
        }

        private void SaveLog(string strLog, string strCommandName)
        {            
            lock (lockobjectLog)
            {
                strLog = strLog.Replace("\n", "");
                DKLogger.WriteCommLog(strLog, "VECTOR:" + strCommandName, false);
            }

        }

        private void SetVariable() //변수 초기화
        {            
            DKLogger     = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_VECTOR);
            xlSendDataPack = new XLClass.xl_event_collection(1);            
        }
        
        private void ClearSharedData()
        {
            ThreadSendRecv.Reset();
            srData = new SharedVectorData();
            srData.bResult = false;
            srData.bSuccess = false;
            srData.strCommandName = String.Empty;
            srData.strMessage = String.Empty;
            srData.xlMsg = new XLClass.xl_can_msg();
            srData.iSendRecvOption = 0;
            
        }

        private void UpdateSharedData(SharedVectorData tmpData)
        {
            srData.xlMsg = tmpData.xlMsg;
            srData.strMessage = tmpData.strMessage;
            srData.bSuccess = tmpData.bSuccess;
            srData.bResult = true;
            ThreadSendRecv.Set();
        }

        public DK_VECTOR_BASIC()
        {
            lockobjectLog = new object();
            lockobjectWrite = new object();
            lockObject2 = new object();
            SetVariable();           
        }

        public void Disconnect()
        {
            bConnection = false;
            try
            {
                ContinuesStop();   

                if (myApp != null)
                {
                    myApp.XL_ClosePort(portHandle);
                    myApp.XL_CloseDriver();
                    myApp = null;
                }
            }
            catch { }
            
        }

        public bool Initialize(ref string strErrMsg, uint uRate)
        {
            bConnection = false;
            if (myApp != null)
            {
                myApp.XL_ClosePort(portHandle);
                myApp.XL_CloseDriver();                
                myApp = null;
            }

            myApp = new XLDriver();
            XLDefine.XL_Status status;
            uBaudrate = uRate;

            swDelayTimeChecker.Reset();
            Release(); // Release & SetBaudRate & SetChannel

            
            //1. Open XL Driver
            status = myApp.XL_OpenDriver();
         
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                strErrMsg = "[FAIL] Vector Driver Load(Error:" + status.ToString() +")";                
                return false;
            }
            /*
            else
            {
                SaveLog("" + "[SUCCESS]", "Vector Driver Open");
            }
            */

            //2. Get XL Driver configuration
            status = myApp.XL_GetDriverConfig(ref driverConfig);
            
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                strErrMsg = "[" + status + "] Get Driver Config";
                return false;
            }
                
            else
            {
                
                strErrMsg = "[SUCCESS] Driver Version:" + myApp.VersionToString(driverConfig.dllVersion) + " Channels:" + driverConfig.channelCount.ToString();
                
                // If the application name cannot be found in VCANCONF...
                if ((myApp.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS)
                    ||
                    (myApp.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS))
                {
                    //...create the item with two CAN channels
                    myApp.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                    myApp.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);

                    strErrMsg = "[FAIL] XL_GetApplConfig";
                    return false;
                }
                else
                {
                    // Read setting of CAN1
               
                    myApp.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);

                    // Notify user if no channel is assigned to this application 
                    if (hwType == XLDefine.XL_HardwareType.XL_HWTYPE_NONE)
                    {
                        strErrMsg = "[FAIL] CAN1 XL_HardwareType.XL_HWTYPE_NONE";
                        return false;
                    }
                    //SaveLog("" + "[SUCCESS]", "CAN1 XL_HardwareType:" + hwType.ToString());

                    accessMask = myApp.XL_GetChannelMask(hwType, (int)hwIndex, (int)hwChannel);
                    txMask = accessMask; // this channel is used for Tx

                   
                    /*
                    // Read setting of CAN2
                  
                    myApp.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);

                    // Notify user if no channel is assigned to this application 
                    if (hwType == XLDefine.XL_HardwareType.XL_HWTYPE_NONE)
                    {
                        SaveLog("" + "[FAIL]", "CAN2 XL_HardwareType.XL_HWTYPE_NONE");
                        return false;
                    }
                   
                    //SaveLog("" + "[SUCCESS]", "CAN2 XL_HardwareType:" + hwType.ToString());

                    accessMask |= myApp.XL_GetChannelMask(hwType, (int)hwIndex, (int)hwChannel); // OR: access both channels for RX later
                   
                    */
                    permissionMask = accessMask;

                    // Open port
                    status = myApp.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);

                    if (status != XLDefine.XL_Status.XL_SUCCESS)
                    {
                        strErrMsg = "[FAIL] XL_OpenPort";
                        return false;
                    }
                    /*
                    else
                    {
                        SaveLog("" + "[SUCCESS]", "XL_OpenPort");
                    }
                    */
                    // Check port
                    status = myApp.XL_CanRequestChipState(portHandle, accessMask);

                    if (status != XLDefine.XL_Status.XL_SUCCESS)
                    {
                        strErrMsg = "[FAIL] XL_OpenPort:" + status;
                        return false;
                    }

                    //SaveLog("" + "[SUCCESS]", "XL_CanRequestChipState");

                    // Activate channel
                    status = myApp.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);

                    if (status != XLDefine.XL_Status.XL_SUCCESS)
                    {
                        strErrMsg = "[FAIL] XL_ActivateChannel:" + status;
                        return false;
                    }

                    //SaveLog("" + "[SUCCESS]", "XL_ActivateChannel");


                    // Get RX event handle
                    status = myApp.XL_SetNotification(portHandle, ref eventHandle, 1);

                    if (status != XLDefine.XL_Status.XL_SUCCESS)
                    {
                        strErrMsg = "[FAIL] XL_SetNotification:" + status;
                        return false;
                    }

                    //SaveLog("" + "[SUCCESS]", "XL_SetNotification");
                    // Reset time stamp clock
                    status = myApp.XL_ResetClock(portHandle);

                    if (status != XLDefine.XL_Status.XL_SUCCESS)
                    {
                        strErrMsg = "[FAIL] XL_ResetClock:" + status;
                        return false;
                    }

                }

            }

            myApp.XL_FlushReceiveQueue(portHandle); 
            SaveLog("" + strErrMsg, "INITIALIZE");
            bConnection = true;
            return true;

        
        }

        private void AutoSending(object obj)
        {
            if (!bConnection) return;

            TBLDATA0 tmpCanTBL = new TBLDATA0();
            tmpCanTBL = (TBLDATA0)obj;
            string strTxMessage = tmpCanTBL.SENDPAC;
            string strQueryID = tmpCanTBL.RECVPAC;
            string strCommand = tmpCanTBL.CMDNAME;
            string strRecvID = tmpCanTBL.PARPAC1;
            string[] strConMessage = new string[8];
            strConMessage = System.Text.RegularExpressions.Regex.Split(strTxMessage, " ");
            byte byteDLC = (byte)strConMessage.Length;

            string strMessage = String.Empty;

            XLClass.xl_can_msg xlSendDataMsg = new XLClass.xl_can_msg(); //보낼 명령 구조체
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);
                dtm = DateTime.Now;

                while (!DelayTimeCheck(dtm, dCycleTime))
                {
                    if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                    Thread.Sleep(1);                    
                }
                Send(strQueryID, strConMessage, ref strMessage, strCommand, ref xlSendDataMsg, strRecvID, (int)MODE.SEND, false);
                
            }
            
        }

        private void AutoSending2(object obj)
        {
            
            if (!bConnection) return;

            TBLDATA0 tmpCanTBL = new TBLDATA0();
            tmpCanTBL = (TBLDATA0)obj;
            string strTxMessage = tmpCanTBL.SENDPAC;
            string strQueryID = tmpCanTBL.RECVPAC;
            string strCommand = tmpCanTBL.CMDNAME;
            string strRecvID = tmpCanTBL.PARPAC1;
            string[] strConMessage1 = new string[8];
            string[] strConMessage2 = new string[6];
            strConMessage1 = System.Text.RegularExpressions.Regex.Split("45 40 80 00 00 00 00 00", " ");
            strConMessage2 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 10 00", " ");

            //byte byteDLC = 0x08;

            string strMessage = String.Empty;

            XLClass.xl_can_msg xlSendDataMsg = new XLClass.xl_can_msg(); //보낼 명령 구조체
            DateTime dtm = DateTime.Now;

            int iSequence = 0;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);
                dtm = DateTime.Now;

                while (!DelayTimeCheck(dtm, dCycleTime))
                {
                    if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                    Thread.Sleep(1);
                }
               
                switch (iSequence)
                {
                    case 0:
                        Send("145", strConMessage1, ref strMessage, strCommand, ref xlSendDataMsg, "00", (int)MODE.SEND, false);                        
                        break;
                    case 1:
                        Send("284", strConMessage2, ref strMessage, strCommand, ref xlSendDataMsg, "00", (int)MODE.SEND, false);                                                
                        iSequence = -1;
                        break;
                    default:
                        iSequence = -1;
                        break;
                }
                iSequence++;
            }

        }

        private void AutoSending3(object obj)
        {
            if (!bConnection) return;

            TBLDATA0 tmpCanTBL = new TBLDATA0();
            tmpCanTBL = (TBLDATA0)obj;
            string strTxMessage = tmpCanTBL.SENDPAC;
            string strQueryID = tmpCanTBL.RECVPAC;
            string strCommand = tmpCanTBL.CMDNAME;
            string strRecvID = tmpCanTBL.PARPAC1;
            string[] strConMessage1 = new string[8];
            string[] strConMessage2 = new string[6];
            strConMessage1 = System.Text.RegularExpressions.Regex.Split("45 40 80 00 00 00 00 00", " ");
            strConMessage2 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 10 00", " ");

            //byte byteDLC = 0x08;

            string strMessage = String.Empty;

            XLClass.xl_can_msg xlSendDataMsg = new XLClass.xl_can_msg(); //보낼 명령 구조체
            DateTime dtm = DateTime.Now;

            int iSequence = 0;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);
                dtm = DateTime.Now;

                while (!DelayTimeCheck(dtm, dCycleTime))
                {
                    if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                    Thread.Sleep(1);
                }

                switch (iSequence)
                {
                    case 0:
                        Send("145", strConMessage1, ref strMessage, strCommand, ref xlSendDataMsg, "00", (int)MODE.SEND, false);
                        break;
                    case 1:
                        Send("36E", strConMessage2, ref strMessage, strCommand, ref xlSendDataMsg, "00", (int)MODE.SEND, false);
                        iSequence = -1;
                        break;
                    default:
                        iSequence = -1;
                        break;
                }
                iSequence++;
            }

        }

        public bool ContinuesSending(TBLDATA0 tmpCanTBL, ref string strErrMsg)
        {
            dCycleTime = 100;

            switch (tmpCanTBL.CMDNAME)
            {
                case "GEN11_GB_END":
                case "GEN11_GEM_END":
                case "GEN12_GB_END":
                case "GEN12_GEM_END":
                case "HSCAN_CONTINUOS_STOP": ContinuesStop(); return true;
                                            
                case "HSCAN_CONTINUOS_TEST":
                    SendThread = new Thread(new ParameterizedThreadStart(AutoSending));
                    break;
                case "GEN11_GEM_BOOT":
                case "GEN12_GEM_BOOT": dCycleTime = 635; //640
                    SendThread = new Thread(new ParameterizedThreadStart(AutoSending));
                    break;
                case "GEN11_GB_BOOT":
                case "GEN12_GB_BOOT": dCycleTime = 495; //500
                    SendThread = new Thread(new ParameterizedThreadStart(AutoSending2));
                    break;
                case "GEN11_GB_BOOT_MY23":
                case "GEN12_GB_BOOT_MY23": dCycleTime = 495; //500
                    SendThread = new Thread(new ParameterizedThreadStart(AutoSending3));
                    break;
                default: return false;
            }

            ContinuesStop();
            SendThread.Start(tmpCanTBL);
            return true;
        }

        private void ContinuesStop()
        {
            KillThreadObject(SendThread);            
        }

        private bool Release()
        {
            ContinuesStop();

            lock (lockObject2)
            {
                XLDefine.XL_Status status = new XLDefine.XL_Status();

                status = myApp.XL_DeactivateChannel(portHandle, permissionMask);

                if (status != XLDefine.XL_Status.XL_SUCCESS)
                {}
                else
                {}
                                
                status = myApp.XL_CanSetChannelBitrate(portHandle, permissionMask, uBaudrate);
                status = myApp.XL_ActivateChannel(portHandle, permissionMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_RESET_CLOCK);
                
                if (status != XLDefine.XL_Status.XL_SUCCESS)
                {                   
                    return false;
                }
                return true;
            }
        }
        
        private XLDefine.XL_Status WriteVectorWithLog(XLClass.xl_event_collection xlEventCollection, string strCommandName, string strSendPack, bool bLogDisable)
        {
            //strCmdName
            lock (lockobjectWrite)
            {
                if (!bLogDisable) 
                    SaveLog("[TX]" + strCommandName, strSendPack);

                return myApp.XL_CanTransmit(portHandle, txMask, xlEventCollection);
            }
        }

        private bool DelayTimeCheck(DateTime dtOut, double dTime)
        {
            DateTime dtCurr = DateTime.Now;
            TimeSpan tsN = dtCurr - dtOut;

            if (tsN.TotalMilliseconds > dTime)
            {
                return true;
            }
            return false;
        }

#region CAN 데이터 TX 관련

        public bool Send(string strQueryID, string[] strQueryData, ref string strMessage, string strCmdName, ref XLClass.xl_can_msg tmpCanMsg,
                         string strRecvMsgCode, int iSendRecvOption, bool bLogDisable)
        {
            if (!bConnection) return false;

            ushort usDLC = (ushort)strQueryData.Length;
            XLClass.xl_event_collection xlSendDataPack = new XLClass.xl_event_collection(1); //보낼 명령 구조체

            if (usDLC == 0 || usDLC > 8) return false;

            string strSendPack = String.Empty;
            SharedVectorData tmpData = new SharedVectorData();

            try
            {
                xlSendDataPack.xlEvent[0].tagData.can_Msg.id  = Convert.ToUInt32(strQueryID, 16);
                if (strCmdName.IndexOf("EXTENDED_", 0).Equals(0)) //29bit 일경우 아래사용
                {
                    xlSendDataPack.xlEvent[0].tagData.can_Msg.id |= 0x80000000;
                    tmpData.bExtended = true;
                }
                else
                {
                    tmpData.bExtended = false;
                }
                xlSendDataPack.xlEvent[0].tagData.can_Msg.dlc = usDLC;
                xlSendDataPack.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
                strSendPack = strQueryID + " ";
                for (int i = 0; i < (int)usDLC; i++)
                {
                    xlSendDataPack.xlEvent[0].tagData.can_Msg.data[i] = Convert.ToByte(strQueryData[i], 16);
                    strSendPack += strQueryData[i] + " ";
                }
            }
            catch 
            {
                return false;
            }

            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
            {
                strMessage = "USER STOP.";
                return false;
            }
            strSendPack = strSendPack.Trim();

            

            uint uMsgCode = 0;

            switch (iSendRecvOption)
            {
                case (int)MODE.SEND:
                    break;

                default:

                    ClearSharedData();
                    KillThreadObject(RecvThread);
                    tmpData.xlMsg = tmpCanMsg;
                    tmpData.strCommandName = strCmdName;
                    tmpData.strMessage = strMessage;

                    try
                    {
                        uMsgCode = Convert.ToUInt32(strRecvMsgCode, 16);
                    }
                    catch
                    {
                        strMessage = "Recv Code Format Error.";
                        return false;
                    }
                    
                    tmpData.uMsgCode = uMsgCode;

                    if(tmpData.bExtended)
                    {
                        tmpData.uMsgCode |= 0x80000000;
                    }
                    
                    tmpData.iSendRecvOption = (int)MODE.SENDRECV;
      
                    RecvThread = new Thread(new ParameterizedThreadStart(Recv));
                    RecvThread.Start(tmpData);
                    Thread.Sleep(20);
                    break;

            }

            XLDefine.XL_Status xlStatus = myApp.XL_CanRequestChipState(portHandle, accessMask);

            switch (xlStatus)
            {
                case XLDefine.XL_Status.XL_SUCCESS:
                case XLDefine.XL_Status.XL_PENDING:
                case XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY:
                case XLDefine.XL_Status.XL_ERR_QUEUE_IS_FULL:
                case XLDefine.XL_Status.XL_ERR_TX_NOT_POSSIBLE:
                case XLDefine.XL_Status.XL_ERR_NO_LICENSE:
                case XLDefine.XL_Status.XL_ERR_WRONG_PARAMETER:
                case XLDefine.XL_Status.XL_ERR_TWICE_REGISTER:
                case XLDefine.XL_Status.XL_ERR_INVALID_CHAN_INDEX:
                case XLDefine.XL_Status.XL_ERR_INVALID_ACCESS:
                case XLDefine.XL_Status.XL_ERR_PORT_IS_OFFLINE:
                case XLDefine.XL_Status.XL_ERR_CHAN_IS_ONLINE:
                case XLDefine.XL_Status.XL_ERR_NOT_IMPLEMENTED:
                case XLDefine.XL_Status.XL_ERR_INVALID_PORT:
                case XLDefine.XL_Status.XL_ERR_HW_NOT_READY:
                case XLDefine.XL_Status.XL_ERR_CMD_TIMEOUT:
                case XLDefine.XL_Status.XL_ERR_HW_NOT_PRESENT:
                case XLDefine.XL_Status.XL_ERR_NOTIFY_ALREADY_ACTIVE:
                case XLDefine.XL_Status.XL_ERR_NO_RESOURCES:
                case XLDefine.XL_Status.XL_ERR_WRONG_CHIP_TYPE:
                case XLDefine.XL_Status.XL_ERR_WRONG_COMMAND:
                case XLDefine.XL_Status.XL_ERR_INVALID_HANDLE:
                case XLDefine.XL_Status.XL_ERR_RESERVED_NOT_ZERO:
                case XLDefine.XL_Status.XL_ERR_INIT_ACCESS_MISSING:
                case XLDefine.XL_Status.XL_ERR_CANNOT_OPEN_DRIVER:
                case XLDefine.XL_Status.XL_ERR_WRONG_BUS_TYPE:
                case XLDefine.XL_Status.XL_ERR_DLL_NOT_FOUND:
                case XLDefine.XL_Status.XL_ERR_INVALID_CHANNEL_MASK:
                case XLDefine.XL_Status.XL_ERR_NOT_SUPPORTED:
                case XLDefine.XL_Status.XL_ERR_CONNECTION_BROKEN:
                case XLDefine.XL_Status.XL_ERR_CONNECTION_CLOSED:
                case XLDefine.XL_Status.XL_ERR_INVALID_STREAM_NAME:
                case XLDefine.XL_Status.XL_ERR_CONNECTION_FAILED:
                case XLDefine.XL_Status.XL_ERR_STREAM_NOT_FOUND:
                case XLDefine.XL_Status.XL_ERR_STREAM_NOT_CONNECTED:
                case XLDefine.XL_Status.XL_ERR_QUEUE_OVERRUN:
                case XLDefine.XL_Status.XL_ERROR:
                default:
                    string strstatus = xlStatus.ToString();
                    break;
            }

            xlStatus = WriteVectorWithLog(xlSendDataPack, strSendPack, strCmdName, bLogDisable);

            switch (xlStatus)
            {
                case XLDefine.XL_Status.XL_SUCCESS:

                                switch (iSendRecvOption)
                                {
                                    case (int)MODE.SEND:
                                        return true; //보내기만 할경우는 그냥 보내고 끝.

                                    default:    //일반적으로는 보내고 받는다.
                                        ThreadSendRecv.WaitOne(1100);
                                        Thread.Sleep(10);
                                        if (srData.bResult)
                                        {
                                            tmpCanMsg = srData.xlMsg;
                                            strMessage = srData.strMessage;
                                            return srData.bSuccess;
                                        }
                                        else
                                        {
                                            strMessage = "No Message";
                                            return false;
                                        }
                                }

                default: break;
            }

            KillThreadObject(RecvThread);
            return false;
        }

#endregion

        #region CAN 데이터 RX 관련

        private bool RecvTimeOutCheck()
        {
            //dtCurrTime = DateTime.Now;

            //TimeSpan tsNow = dtCurrTime - dtOutSet;

            if (swDelayTimeChecker.ElapsedMilliseconds > Item_dTimeSet * 1000)
            {
                return false;
            }

            //System.Threading.Thread.Sleep(1);
            return true;
        }

        private void Recv(object obj)
        {
            SharedVectorData tmpData = (SharedVectorData)obj;

            XLClass.xl_event     receivedEvent = new XLClass.xl_event();
            XLDefine.XL_Status   xlStatus      = XLDefine.XL_Status.XL_SUCCESS;
            XLDefine.WaitResults waitResult    = new XLDefine.WaitResults();
            StringBuilder        strTmpRecvData = new StringBuilder(1024);
            //dtOutSet = DateTime.Now;
            Item_dTimeSet = 1;
            swDelayTimeChecker.Restart();
            tmpData.bSuccess = false;

            while (RecvTimeOutCheck())
            {
                if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
                {
                    swDelayTimeChecker.Reset();
                    tmpData.strMessage = "USER STOP.";
                    tmpData.bSuccess = false;
                    UpdateSharedData(tmpData);
                    return;
                }

                waitResult = (XLDefine.WaitResults)WaitForSingleObject(eventHandle, 10);

                if (waitResult != XLDefine.WaitResults.WAIT_TIMEOUT)
                {
                    xlStatus = myApp.XL_Receive(portHandle, ref receivedEvent);                                            

                    switch (xlStatus)
                    {
                        case XLDefine.XL_Status.XL_SUCCESS:

                            if (receivedEvent.tag == XLDefine.XL_EventTags.XL_RECEIVE_MSG)
                            {
                                // 1. 응답 데이터 체크
                                if (receivedEvent.tagData.can_Msg.id.Equals(tmpData.uMsgCode))
                                {
                                    strTmpRecvData.Append("[RX] ");
                                    strTmpRecvData.Append(receivedEvent.tagData.can_Msg.id.ToString("X2"));
                                    strTmpRecvData.Append(" ");
                                    strTmpRecvData.Append(BitConverter.ToString(receivedEvent.tagData.can_Msg.data).Replace("-", " "));
                                    SaveLog(strTmpRecvData.ToString(), tmpData.strCommandName);

                                    tmpData.xlMsg = receivedEvent.tagData.can_Msg;
                                    swDelayTimeChecker.Reset();
                                    //dtOutSet = DateTime.Now;
                                    tmpData.strMessage = "Recv Success";
                                    tmpData.bSuccess = true;
                                    UpdateSharedData(tmpData);
                                    return;
                                }
                            }
                            break;
                        case XLDefine.XL_Status.XL_ERR_QUEUE_IS_FULL:   myApp.XL_FlushReceiveQueue(portHandle); break;

                        case XLDefine.XL_Status.XL_ERROR:
                        case XLDefine.XL_Status.XL_ERR_QUEUE_OVERRUN: Release(); break;

                        case XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY:
                        default: break;

                    }

                }
                
                Thread.Sleep(1);
            }

            swDelayTimeChecker.Reset();
            tmpData.strMessage = "TIME OUT"; // +xlStatus.ToString();
            UpdateSharedData(tmpData);
            return;

        }       
  
        #endregion

        private void KillThreadObject(Thread theThread)
        {
            try
            {
                if (theThread != null)
                {
                    if (theThread.IsAlive)
                    {   //ReadThread 스레드가 그래도 살아있으면 강제종료
                        theThread.Abort();

                    }
                }
            }
            catch { }
        }

        
    }
}
