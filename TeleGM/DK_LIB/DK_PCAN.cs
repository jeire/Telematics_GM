using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TPCANHandle = System.Byte;

namespace GmTelematics
{
    enum CANUSE
    {
        NONE, PSA, GB, GBMY23, GEM, MCTM, END
    }

    struct SharedRecvData
    {
        public bool bResult;
        public bool bSuccess;
        public TPCANMsg tCANMsg;
        public string strMessage;
        public uint uMsgCode;
        public string strCommandName;

        public bool bExtMode;

        public bool bMulti;
        public int  bMultiCount;

        //sending 용
        public int iSendRecvOption;
        public stWriteOnly sendCANMsg; 

    }

    struct stWriteOnly
    {
        public TPCANMsg tmpCANMsg;
        public string str1;
        public string str2;
		public bool bLogDisable;
    }

    class DK_PCAN
    {
        public event EventRealTimeMsg PCANRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
                
        private DK_LOGGER DKLogger;

        private Thread SendThread;   //전용        
        private int SendThreadID;

        private ManualResetEvent ThreadEvent_Pause = new ManualResetEvent(false);
        private ManualResetEvent ThreadEvent_Off   = new ManualResetEvent(false);

        private ManualResetEvent ThreadSendRecv = new ManualResetEvent(false);
        private ManualResetEvent ThreadEvtLock  = new ManualResetEvent(false);
        
        private SharedRecvData   srData = new SharedRecvData();
        
        private Thread RecvThread;   //일반 recv 전용

        private object lockobjectLog;
        private object lockobjectWrite;
        
        #region PCAN Structures

        private delegate void ReadDelegateHandler();
        private TPCANHandle m_PcanHandle;
        private TPCANBaudrate m_Baudrate;
        private TPCANType m_HwType;

        private System.Diagnostics.Stopwatch swDelayTimeChecker = new System.Diagnostics.Stopwatch();

        //private DateTime dtOutSet;
        //private DateTime dtCurrTime;

        private string strFiterCommand;

        private double dTimeSet;
        private double Item_dTimeSet
        {
            get { return dTimeSet; }
            set { dTimeSet = value; }
        }


        private class MessageStatus
        {
            private TPCANMsg m_Msg;
            private TPCANTimestamp m_TimeStamp;
            private TPCANTimestamp m_oldTimeStamp;
            private int m_iIndex;
            private int m_Count;
            private bool m_bShowPeriod;
            private bool m_bWasChanged;

            public MessageStatus(TPCANMsg canMsg, TPCANTimestamp canTimestamp, int listIndex)
            {
                m_Msg = canMsg;
                m_TimeStamp = canTimestamp;
                m_oldTimeStamp = canTimestamp;
                m_iIndex = listIndex;
                m_Count = 1;
                m_bShowPeriod = true;
                m_bWasChanged = false;
            }

            public void Update(TPCANMsg canMsg, TPCANTimestamp canTimestamp)
            {
                m_Msg = canMsg;
                m_oldTimeStamp = m_TimeStamp;
                m_TimeStamp = canTimestamp;
                m_bWasChanged = true;
                m_Count += 1;
            }

            public TPCANMsg CANMsg
            {
                get { return m_Msg; }
            }

            public TPCANTimestamp Timestamp
            {
                get { return m_TimeStamp; }
            }

            public int Position
            {
                get { return m_iIndex; }
            }

            public string TypeString
            {
                get { return GetMsgTypeString(); }
            }

            public string IdString
            {
                get { return GetIdString(); }
            }

            public string DataString
            {
                get { return GetDataString(); }
            }

            public int Count
            {
                get { return m_Count; }
            }

            public bool ShowingPeriod
            {
                get { return m_bShowPeriod; }
                set
                {
                    if (m_bShowPeriod ^ value)
                    {
                        m_bShowPeriod = value;
                        m_bWasChanged = true;
                    }
                }
            }

            public bool MarkedAsUpdated
            {
                get { return m_bWasChanged; }
                set { m_bWasChanged = value; }
            }

            public string TimeString
            {
                get { return GetTimeString(); }
            }

            private string GetTimeString()
            {
                double fTime;

                fTime = m_TimeStamp.millis + (m_TimeStamp.micros / 1000.0);
                if (m_bShowPeriod)
                    fTime -= (m_oldTimeStamp.millis + (m_oldTimeStamp.micros / 1000.0));
                return fTime.ToString("F1");
            }

            private string GetDataString()
            {
                string strTemp;

                strTemp = "";

                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                    return "Remote Request";
                else
                    for (int i = 0; i < m_Msg.LEN; i++)
                        strTemp += string.Format("{0:X2} ", m_Msg.DATA[i]);

                return strTemp;
            }

            private string GetIdString()
            {
                // We format the ID of the message and show it
                //
                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    return string.Format("{0:X8}h", m_Msg.ID);
                else
                    return string.Format("{0:X3}h", m_Msg.ID);
            }

            private string GetMsgTypeString()
            {
                string strTemp;

                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    strTemp = "EXTENDED";
                else
                    strTemp = "STANDARD";

                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                    strTemp += "/RTR";

                return strTemp;
            }

        }

        #endregion

        private void GateWay_PCAN(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
        {
            PCANRealTimeTxRxMsg(0, cParam);
        }

        private void SaveLog(string strLog, string strCommandName)
        {
            lock (lockobjectLog)
            {
                strLog = strLog.Replace("\n", "");
                DKLogger.WriteCommLog(strLog, "PCAN:" + strCommandName, false);
            }
        }

        private void SetVariable()
        {
            m_PcanHandle = Convert.ToByte("51", 16);//PCANBasic.PCAN_PCIBUS1;   //USBTYPE
            m_HwType = TPCANType.PCAN_TYPE_ISA;     //HW TYPE
            DKLogger = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_PCAN);
            strFiterCommand = String.Empty;
        }

        private void SetBaudrate(int iBaudrate)
        {
            switch (iBaudrate)
            {
                case 0: m_Baudrate = TPCANBaudrate.PCAN_BAUD_1M; break;
                case 1: m_Baudrate = TPCANBaudrate.PCAN_BAUD_800K; break;
                case 2: m_Baudrate = TPCANBaudrate.PCAN_BAUD_500K; break;
                case 3: m_Baudrate = TPCANBaudrate.PCAN_BAUD_250K; break;
                case 4: m_Baudrate = TPCANBaudrate.PCAN_BAUD_125K; break;
                case 5: m_Baudrate = TPCANBaudrate.PCAN_BAUD_100K; break;
                case 6: m_Baudrate = TPCANBaudrate.PCAN_BAUD_95K; break;
                case 7: m_Baudrate = TPCANBaudrate.PCAN_BAUD_83K; break;
                case 8: m_Baudrate = TPCANBaudrate.PCAN_BAUD_50K; break;
                case 9: m_Baudrate = TPCANBaudrate.PCAN_BAUD_47K; break;
                case 10: m_Baudrate = TPCANBaudrate.PCAN_BAUD_33K; break;
                case 11: m_Baudrate = TPCANBaudrate.PCAN_BAUD_20K; break;
                case 12: m_Baudrate = TPCANBaudrate.PCAN_BAUD_10K; break;
                case 13: m_Baudrate = TPCANBaudrate.PCAN_BAUD_5K; break;
                default: m_Baudrate = TPCANBaudrate.PCAN_BAUD_500K; break;
            }
        }

        private void ClearSharedData()
        {
            ThreadSendRecv.Reset();
            srData = new SharedRecvData();
            srData.bResult = false;
            srData.bSuccess = false;
            srData.strCommandName = String.Empty;
            srData.strMessage = String.Empty;
            srData.tCANMsg = new TPCANMsg();

            srData.iSendRecvOption = 0;
            srData.sendCANMsg = new stWriteOnly();

            srData.bMulti = false;
            srData.bMultiCount = 0;
        }

        private void UpdateSharedData(SharedRecvData tmpData)
        {
            //Recv(ref TPCANMsg tCANMsg, ref string strMessage, uint uMsgCode, string strCommandName)
            srData.tCANMsg = tmpData.tCANMsg;
            srData.strMessage = tmpData.strMessage;
            srData.bSuccess = tmpData.bSuccess;
            srData.bResult = true;      
            ThreadSendRecv.Set(); 
        }

        public DK_PCAN()
        {
            SendThreadID = (int)CANUSE.NONE;
            lockobjectLog = new object();
            lockobjectWrite = new object();
            SetVariable();
        }

        public bool Initialize(ref string strErrMsg, int iBaudrate)
        {
            Release();

            SetBaudrate(iBaudrate);

            TPCANStatus stsResult;

            swDelayTimeChecker.Reset();

            stsResult = PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, 256, 3);
                        
            switch (stsResult)
            {
                case TPCANStatus.PCAN_ERROR_OK:
                    if (!ConfigureTraceFile(ref strErrMsg))
                    {   
                        return false;
                    }
                    return true;

                case TPCANStatus.PCAN_ERROR_NETINUSE:
                    //PCAN 뷰어가 사용중임.
                case TPCANStatus.PCAN_ERROR_INITIALIZE:
                    //여기서 이미 이니셜되었음.
                    return true;
                    
                default:

                    strErrMsg = GetFormatedError(stsResult);
                    return false;
            }

        }

        public void Release()
        {
            CloseSendThread();
            PCANBasic.Uninitialize(m_PcanHandle);
            
        }

        private TPCANStatus WritePcanWithLog(TPCANHandle m_PcanH, ref TPCANMsg cMsg, string str1, string str2, bool bLogDisable)
        {
            //strCmdName
            lock (lockobjectWrite)
            {
                if (!bLogDisable && !String.IsNullOrEmpty(str2)) SaveLog("[TX] " + str1, str2);
                return CanWrite(m_PcanH, ref cMsg);
            }
        }

        private TPCANStatus WritePcanWithoutLog(TPCANHandle m_PcanH, ref TPCANMsg cMsg, string str1, string str2)
        {
            return CanWrite(m_PcanH, ref cMsg);            
        }

        private TPCANStatus CanWrite(TPCANHandle m_PcanH, ref TPCANMsg cMsg)
        {
            lock (lockobjectWrite)
            {
                return PCANBasic.Write(m_PcanH, ref cMsg);
            }
        }

        public string CheckPcanStatus()
        {   
            TPCANStatus tmpResult = PCANBasic.GetStatus(m_PcanHandle);
            return GetStatus(tmpResult);
        }

        public string CheckPcanStatus2(TPCANStatus tmpResult)
        {
            return GetStatus(tmpResult);            
        }

        private string GetStatus(TPCANStatus tmpResult)
        {
            string strStatus = "OK";

            switch (tmpResult)
            {
                case TPCANStatus.PCAN_ERROR_OK:
                    break;
                case TPCANStatus.PCAN_ERROR_BUSHEAVY:
                    strStatus = "HEAVY";
                    break;
                case TPCANStatus.PCAN_ERROR_BUSLIGHT:
                    strStatus = "LIGHT";
                    break;
                case TPCANStatus.PCAN_ERROR_BUSOFF:
                case TPCANStatus.PCAN_ERROR_BUSOFF | TPCANStatus.PCAN_ERROR_BUSHEAVY:
                case TPCANStatus.PCAN_ERROR_BUSOFF | TPCANStatus.PCAN_ERROR_BUSLIGHT:
                    strStatus = "BUS OFF";
                    break;
                case TPCANStatus.PCAN_ERROR_INITIALIZE:
                    strStatus = "PCAN_UNINITIALIZE";
                    break;
                default:
                    strStatus = GetFormatedError(tmpResult);
                    break;

            }

            return strStatus;
        }

        public bool Send(string strQueryID, string[] strQueryData, ref string strMessage, string strCmdName, ref TPCANMsg tmpCanMsg, string strRecvMsgCode, int iSendRecvOption, bool bLogDisable, double dTimeOut, bool bStdMode = true, int iMultiCount = 0)
        {
            strFiterCommand = strCmdName;
            TPCANStatus stsResult;
            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];

            CANMsg.ID = Convert.ToUInt32(strQueryID, 16);

            switch (strQueryData.Length)
            {
                default: CANMsg.LEN = 0x08; break;

                case 8: CANMsg.LEN = 0x08; break;
                case 7: CANMsg.LEN = 0x07; break;
                case 6: CANMsg.LEN = 0x06; break;
                case 5: CANMsg.LEN = 0x05; break;
                case 4: CANMsg.LEN = 0x04; break;
                case 3: CANMsg.LEN = 0x03; break;
                case 2: CANMsg.LEN = 0x02; break;
                case 1: CANMsg.LEN = 0x01; break;
            }

            if (bStdMode)
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            else
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;

            SharedRecvData tmpData = new SharedRecvData();

            tmpData.bMulti = iSendRecvOption.Equals((int)MODE.MULTIPLE);
            tmpData.bMultiCount = iMultiCount;

            if (strCmdName.IndexOf("EXTENDED_", 0).Equals(0)) //29bit 일경우 아래사용
            {
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;
                tmpData.bExtMode = true;
            }
            else
            {
                tmpData.bExtMode = false;
            }           

            StringBuilder strTmpSendData = new StringBuilder(1024);
            strTmpSendData.Clear();

            strTmpSendData.Append(CANMsg.ID.ToString("X2"));
            strTmpSendData.Append(" ");

            for (int i = 0; i < CANMsg.LEN; i++)
            {
                CANMsg.DATA[i] = Convert.ToByte(strQueryData[i], 16);
                strTmpSendData.Append(strQueryData[i]);
                strTmpSendData.Append(" ");
            }

            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
            {
                strMessage = "USER STOP.";
                return false;
            }                        
            
            
            
            uint uMsgCode = 0;

            switch (iSendRecvOption)
            {
                case (int)MODE.SEND: 
                        break;

                default:

                        ClearSharedData();
                        KillThreadObject(RecvThread);                                
                        tmpData.tCANMsg = tmpCanMsg;
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
                        if(iSendRecvOption.Equals((int)MODE.UNTIL))                        
                            tmpData.iSendRecvOption = (int)MODE.UNTIL;                        
                        else
                            tmpData.iSendRecvOption = (int)MODE.SENDRECV;

                        tmpData.sendCANMsg = new stWriteOnly();
                        tmpData.sendCANMsg.tmpCANMsg = CANMsg;
                        tmpData.sendCANMsg.str1 = strTmpSendData.ToString();
                        tmpData.sendCANMsg.str2 = strCmdName;
                        tmpData.sendCANMsg.bLogDisable = bLogDisable;

                        Item_dTimeSet = dTimeOut;
                        RecvThread = new Thread(new ParameterizedThreadStart(Recv));
                        RecvThread.Start(tmpData);
                        Thread.Sleep(5);
                        break;
            }

            TPCANStatus tmpResult = PCANBasic.GetStatus(m_PcanHandle);

            switch (tmpResult)
            {
                case TPCANStatus.PCAN_ERROR_OK: STEPMANAGER_VALUE.AddPcanStatus((int)PCANBUS.OK, CheckPcanStatus()); break;
                case TPCANStatus.PCAN_ERROR_INITIALIZE:
                    //SaveLog("[TX] " + strTmpSendData.ToString(), strCmdName);
                    strMessage = "PCAN UNINITIALIZE";
                    if (!iSendRecvOption.Equals((int)MODE.SEND)) KillThreadObject(RecvThread);       
                    return false;

                default:
                    
                    switch (tmpResult)
                    {
                        case TPCANStatus.PCAN_ERROR_BUSHEAVY:
                            STEPMANAGER_VALUE.AddPcanStatus((int)PCANBUS.HEAVY, CheckPcanStatus());
                            //PCANBasic.Reset(m_PcanHandle);
                            break;
                        case TPCANStatus.PCAN_ERROR_BUSLIGHT:
                            STEPMANAGER_VALUE.AddPcanStatus((int)PCANBUS.LIGHT, CheckPcanStatus());
                            //PCANBasic.Reset(m_PcanHandle);
                            break;
                        case TPCANStatus.PCAN_ERROR_BUSOFF:
                        case TPCANStatus.PCAN_ERROR_BUSOFF | TPCANStatus.PCAN_ERROR_BUSHEAVY:
                        case TPCANStatus.PCAN_ERROR_BUSOFF | TPCANStatus.PCAN_ERROR_BUSLIGHT:
                            STEPMANAGER_VALUE.AddPcanStatus((int)PCANBUS.OFF, CheckPcanStatus());
                            break;
                        default:                         
                            break;
                    }
                    STEPMANAGER_VALUE.AddPcanStatus((int)PCANBUS.RESET, CheckPcanStatus());
                    PCANBasic.Uninitialize(m_PcanHandle);
                    PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, 256, 3);
                    break;
            }

            stsResult = WritePcanWithLog(m_PcanHandle, ref CANMsg, strTmpSendData.ToString(), strCmdName, bLogDisable);             

            
            if (stsResult.Equals(TPCANStatus.PCAN_ERROR_OK))
            {                
                switch (iSendRecvOption)
                {
                    case (int)MODE.SEND:
                                        return true; //보내기만 할경우는 그냥 보내고 끝.

                            default:    //일반적으로는 보내고 받는다.
                                        //ThreadSendRecv.WaitOne(1100); 타임아웃 값으로 변경
                                        ThreadSendRecv.WaitOne((int)((dTimeOut * 1000) + 500)); //타임아웃 값으로 변경
                                        Thread.Sleep(10);
                                        if (srData.bResult)
                                        {
                                            tmpCanMsg = srData.tCANMsg;
                                            strMessage = srData.strMessage;
                                            return srData.bSuccess;
                                        }
                                        else
                                        {
                                            strMessage = "No Message";
                                            return false;
                                        }      
                }    
            }

            strMessage = CheckPcanStatus2(stsResult);

            if (!iSendRecvOption.Equals((int)MODE.SEND)) KillThreadObject(RecvThread);
            return false;
        }

        public void Reset()
        {
            TPCANStatus stsResult = PCANBasic.Reset(m_PcanHandle);
        }

        void Write(object obj)
        {
            stWriteOnly swo = (stWriteOnly)obj;

            System.Diagnostics.Stopwatch swChecker = new System.Diagnostics.Stopwatch();
            swChecker.Start();

            DateTime dtCTime = DateTime.Now;
            DateTime dtOSet  = DateTime.Now;
            
            int iTiming = 100;

            while (true)
            {
                try
                {
                    dtCTime = DateTime.Now;
                    TimeSpan tsNow = dtCTime - dtOSet;

                    if (tsNow.Milliseconds > iTiming)
                    {
                        WritePcanWithoutLog(m_PcanHandle, ref swo.tmpCANMsg, swo.str1, swo.str2);
                        iTiming += 100;
                    }

                    if (iTiming >= 1000)
                    {
                        return;
                    }
                }
                catch { }
                Thread.Sleep(1);        
                
            }
            
        }

        void Recv(object obj)
        {            
            SharedRecvData tmpData = (SharedRecvData)obj;
            
            TPCANTimestamp CANTimeStamp = new TPCANTimestamp();
            TPCANStatus    stsResult    = new TPCANStatus();
            TPCANStatus    sttResult    = new TPCANStatus();
            TPCANMsg       tmpCANMsg    = new TPCANMsg();

            //dtOutSet = DateTime.Now;            
          
            StringBuilder strTmpRecvData = new StringBuilder(1024);
            strTmpRecvData.Clear();
            tmpData.bSuccess = false;

            if(tmpData.bExtMode)
                PCANBasic.FilterMessages(m_PcanHandle, tmpData.uMsgCode, tmpData.uMsgCode, TPCANMode.PCAN_MODE_EXTENDED);
            else
                PCANBasic.FilterMessages(m_PcanHandle, tmpData.uMsgCode, tmpData.uMsgCode, TPCANMode.PCAN_MODE_STANDARD);



            PCANBasic.Reset(m_PcanHandle);

            Thread WriteThread = new Thread(new ParameterizedThreadStart(Write));

            if (tmpData.iSendRecvOption.Equals((int)MODE.UNTIL))
                WriteThread.Start(tmpData.sendCANMsg);

            swDelayTimeChecker.Restart();

            int iCount = 0;
            while (RecvTimeOutCheck())
            {
                if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
                {
                    swDelayTimeChecker.Reset();
                    KillThreadObject(WriteThread);
                    tmpData.strMessage = "USER STOP.";
                    tmpData.bSuccess = false;
                    UpdateSharedData(tmpData);                    
                    return;
                }
                
                try
                {
                    stsResult = PCANBasic.Read(m_PcanHandle, out tmpCANMsg, out CANTimeStamp);

                    switch (stsResult)
                    {
                        case TPCANStatus.PCAN_ERROR_BUSHEAVY: 
                        case TPCANStatus.PCAN_ERROR_BUSLIGHT:                            
                            PCANBasic.Reset(m_PcanHandle);
                            break;
                        case TPCANStatus.PCAN_ERROR_BUSOFF:
                        case TPCANStatus.PCAN_ERROR_BUSOFF | TPCANStatus.PCAN_ERROR_BUSHEAVY:
                        case TPCANStatus.PCAN_ERROR_BUSOFF | TPCANStatus.PCAN_ERROR_BUSLIGHT:
                            PCANBasic.Uninitialize(m_PcanHandle);
                            PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, 256, 3);
                            break;

                        case TPCANStatus.PCAN_ERROR_OK:
                            if (tmpCANMsg.ID.Equals(tmpData.uMsgCode))
                            {
                                //일단 받은것 로깅부터.
                                
                                strTmpRecvData.Append("[RX] ");
                                strTmpRecvData.Append(tmpCANMsg.ID.ToString("X2"));
                                strTmpRecvData.Append(" ");
                                strTmpRecvData.Append(BitConverter.ToString(tmpCANMsg.DATA).Replace("-", " "));
                                SaveLog(strTmpRecvData.ToString(), tmpData.strCommandName);

                                byte[] bPending = new byte[8]  { 0x03, 0x7F, 0x27, 0x78, 0x00, 0x00, 0x00, 0x00 };
                                
                                switch(strFiterCommand)
                                {  
                                    case "VALIDATE_SEED_KEY":
                                        //펜딩메시지같은 경우를 거르기 위하여. buffer 옵션
                                        
                                        try
                                        {
                                            if (tmpCANMsg.DATA.SequenceEqual(bPending))
                                            {
                                                strTmpRecvData.Clear();
                                                Item_dTimeSet += 3; //pendding 시 타임아웃시간 3초 늘리기
                                                continue;
                                            }
                                        }
                                        catch { continue; }
                                        
                                        break;
                                    default: 
                                        
                                        bool bPend = true;
                                        for (int i = 0; i < 4; i++)
                                        {
                                            if(i == 2) continue;
                                            if(!tmpCANMsg.DATA[i].Equals(bPending[i]))
                                            {                                                
                                                bPend = false;
                                            }
                                        }
                                        if (bPend)
                                        {
                                            Item_dTimeSet += 3; //pendding 시 타임아웃시간 3초 늘리기
                                            strTmpRecvData.Clear();
                                            continue;
                                        }
                                        break;

                                }

                                iCount++;

                                if (tmpData.bMulti && iCount < tmpData.bMultiCount)
                                {
                                    strTmpRecvData.Clear();
                                    continue;
                                }
                                

                                KillThreadObject(WriteThread);

                                tmpData.tCANMsg = tmpCANMsg;
                                sttResult = stsResult;
                                //dtOutSet = DateTime.Now;
                                swDelayTimeChecker.Reset();
                                tmpData.strMessage = "Recv Success";
                                tmpData.bSuccess = true;
                                UpdateSharedData(tmpData);
                                return;

                            }  break;
                        default:
                            break;
                    }                                    
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
            swDelayTimeChecker.Reset();
            KillThreadObject(WriteThread); 
            tmpData.strMessage = "TIME OUT(PCAN STATUS:" + CheckPcanStatus() + ")";
            UpdateSharedData(tmpData);            
            return;
            
        }

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
       
        private bool ConfigureTraceFile(ref string strErrMsg)
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = 5;
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_SIZE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                strErrMsg = GetFormatedError(stsResult);
                return false;
            }

            iBuffer = PCANBasic.TRACE_FILE_SINGLE | PCANBasic.TRACE_FILE_OVERWRITE;
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_CONFIGURE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                strErrMsg = GetFormatedError(stsResult);
                return false;
            }
            return true;
        }

        private string GetFormatedError(TPCANStatus error)
        {
            StringBuilder strTemp = new StringBuilder(1024);

            PCANBasic.GetErrorText(error, 0, strTemp);
            return strTemp.ToString();
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

        public bool ContinuesWakeup(ref string strErrMsg, int iBaudrate, string strtype)
        {//PSA wake up 전용            
            Release();
            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            System.Windows.Forms.Application.DoEvents();
            SetBaudrate(iBaudrate);
            TPCANStatus stsResult;
            stsResult = PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, 256, 3);

            switch (stsResult)
            {
                case TPCANStatus.PCAN_ERROR_OK:
                    if (!ConfigureTraceFile(ref strErrMsg))
                    {
                        strErrMsg = "INITIALIZE ERROR : " + strErrMsg;
                        return false;
                    }
                    break;

                case TPCANStatus.PCAN_ERROR_NETINUSE:
                    //PCAN 뷰어가 사용중임.
                    strErrMsg = "INITIALIZE ERROR : CLOSE PCAN VIEW";
                    return false;
                case TPCANStatus.PCAN_ERROR_INITIALIZE:
                    //여기서 이미 이니셜되었음.
                    PCANBasic.Uninitialize(m_PcanHandle);
                    break;

                default:

                    strErrMsg = "INITIALIZE ERROR : " + GetFormatedError(stsResult);
                    return false;
            }

            PauseThreadObject(false);
            CloseThreadObject(false);
            switch (strtype)
            {
                case "PSA_BOOT":
                    SendThreadID = (int)CANUSE.PSA;
                    SendThread = new Thread(AutoSending); break;
                case "GEN11_GB_BOOT":
                case "GEN12_GB_BOOT":
                    SendThreadID = (int)CANUSE.GB;
                    SendThread = new Thread(AutoSendingGB); break;
                case "GEN12_GB_BOOT_500":
                    SendThreadID = (int)CANUSE.GB;
                    SendThread = new Thread(AutoSendingGB_500); break;
                case "GEN11_GB_BOOT_MY23":
                case "GEN12_GB_BOOT_MY23":
                    SendThreadID = (int)CANUSE.GBMY23;
                    SendThread = new Thread(AutoSendingGB_MY23); break;
                case "GEN11_GEM_BOOT":
                case "GEN12_GEM_BOOT":
                    SendThreadID = (int)CANUSE.GEM;
                    SendThread = new Thread(AutoSendingGEM); break;

                //경민이가 추가했던 소스
                //case "GEN12_GB_BOOT":
                //    SendThreadID = (int)CANUSE.GB;
                //    SendThread = new Thread(AutoSendingGB); break;
                //case "GEN12_GB_BOOT_MY23":
                //    SendThreadID = (int)CANUSE.GBMY23;
                //    SendThread = new Thread(AutoSendingGB_MY23); break;
                //case "GEN12_GEM_BOOT":
                //    SendThreadID = (int)CANUSE.GEM;
                //    SendThread = new Thread(AutoSendingGEM); break;

                case "MCTM_LSCAN_VNMF":
                    SendThreadID = (int)CANUSE.MCTM;
                    SendThread = new Thread(AutoSendingMCTM); break;

                default:
                    strErrMsg = "UNKNOWN PCAN COMMAND ERROR";
                    return false;
            }
            
            SendThread.Start();         
            return true;
        }

        public bool IsAliveAutoSending(int iCanUse)
        {
            //1: PSA, 2: GM, 3: GEM

            if (SendThread != null)
                if (SendThreadID.Equals(iCanUse))
                    return SendThread.IsAlive;

            return false;
        }

        private void CheckResetBusStatus()
        {
            TPCANStatus tmpResult = PCANBasic.GetStatus(m_PcanHandle);
            string strStatus = String.Empty;
            int  iStatus = 0;
            switch (tmpResult)
            {                
                case TPCANStatus.PCAN_ERROR_BUSHEAVY: strStatus = "HEAVY";  iStatus = (int)PCANBUS.HEAVY; break;
                case TPCANStatus.PCAN_ERROR_BUSOFF:   strStatus = "BUSOFF"; iStatus = (int)PCANBUS.OFF;   break;
            }
            switch (tmpResult)
            {
                    case TPCANStatus.PCAN_ERROR_BUSHEAVY: 
                    case TPCANStatus.PCAN_ERROR_BUSOFF:
                                        STEPMANAGER_VALUE.AddPcanStatus(iStatus, strStatus);
                                        PCANBasic.Uninitialize(m_PcanHandle);
                                        PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, 256, 3);
                                        break;
                    default: break;
            }
        }

        private void AutoSending()
        {
            //PSA WAKE UP CAN message 만들기
            string[] strPsaMessage1 = new string[8];
            string[] strPsaMessage2 = new string[8];
            string[] strPsaMessage3 = new string[8];
            string[] strPsaMessage4 = new string[8];


            strPsaMessage1 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 21 00 00 00", " ");  //BOOT100 //0x036
            strPsaMessage2 = System.Text.RegularExpressions.Regex.Split("08 00 00 00 00 00 00 00", " ");  //BOOT500 //0x0F6
            strPsaMessage3 = System.Text.RegularExpressions.Regex.Split("02 10 01 00 00 00 00 00", " ");  //Test100 //0x077C
            strPsaMessage4 = System.Text.RegularExpressions.Regex.Split("02 10 01 00 00 00 00 00", " ");  //Test1   //0x077C
            //strPsaMessage = System.Text.RegularExpressions.Regex.Split("00 03 02 27 01 00 00 00", " "); //Test2   //0x077C

            int iSequence = 0;
            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                
                Thread.Sleep(1);

                if (ThreadEvent_Pause.WaitOne())
                {
                    dtm = DateTime.Now;

                    while (!DelayTimeCheck(dtm, 100))
                    {
                        if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                        CheckResetBusStatus();
                        Thread.Sleep(1);
                    }
                    
                    tmpCanMsg = new TPCANMsg();

                    switch (iSequence)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3: //로깅 안되게 하려고 PSA_WAKEUP 주석처리
                            Send("36", strPsaMessage1, ref strRtnMsg,"" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            break;
                        case 4:
                            Send("36", strPsaMessage1, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            Send("F6", strPsaMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP2"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            iSequence = -1;
                            break;
                        default:
                            iSequence = -1;
                            break;
                    }
                    iSequence++;


                }
                else
                {
                    if (!ThreadEvent_Off.WaitOne())
                    {
                        return;
                    }
                }
            }
        }

        public bool UnlockProcessGBGEM(int iType, string sModel)
        {   
            string strBootID = "145";
            string[] strCanBootMsg  = System.Text.RegularExpressions.Regex.Split("45 50 D0 00 00 00 00 00", " ");

            switch(iType)
            {
                case 1: //GEM
                    strCanBootMsg  = System.Text.RegularExpressions.Regex.Split("40 40 FF FF FF FF FF FF", " ");
                    strBootID = "621";
                    break;
                case 0: //GB
                default:
                    strCanBootMsg  = System.Text.RegularExpressions.Regex.Split("45 50 D0 00 00 00 00 00", " ");
                    strBootID = "145";
                    if (sModel.Contains("GEN12"))
                        strBootID = "147";

                    break;

            }
            
            string[] strCanAldlMsg  = System.Text.RegularExpressions.Regex.Split("03 22 45 E9 00 00 00 00", " ");
            string[] strCanAldlMsg_Gen12 = System.Text.RegularExpressions.Regex.Split("03 22 5F F0 00 00 00 00", " ");  //gen12
            string[] strCanSessMsg  = System.Text.RegularExpressions.Regex.Split("02 3E 80 00 00 00 00 00", " ");
            string[] strCanStepMsg1 = System.Text.RegularExpressions.Regex.Split("02 10 03 00 00 00 00 00", " ");
            string[] strCanStepMsg2 = System.Text.RegularExpressions.Regex.Split("02 27 01 00 00 00 00 00", " ");
            string[] strCanFlowControl = System.Text.RegularExpressions.Regex.Split("30 00 00 00 00 00 00 00", " ");


            string[] strCanStepMsg3 = System.Text.RegularExpressions.Regex.Split("10 0E 27 02 00 00 00 00", " ");
            string[] strCanStepMsg4 = System.Text.RegularExpressions.Regex.Split("21 00 00 00 00 00 00 00", " ");
            string[] strCanStepMsg5 = System.Text.RegularExpressions.Regex.Split("22 00 00 00 00 00 00 00", " ");

            string[] strCanStepMsg6 = System.Text.RegularExpressions.Regex.Split("06 2E 45 E9 00 00 00 00", " ");
            string[] strCanStepMsg6_gen12 = System.Text.RegularExpressions.Regex.Split("04 2E 5F F0 00 00 00 00", " "); //gen12


            int iSequence = 1;
            int iSession = 1;

            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();

            string strALDL_45E9 = String.Empty;
            string strSession   = String.Empty;
            string strSeed = String.Empty;
            string strKey = String.Empty;
            string strSecurityChange = String.Empty;

            bool[] bStep = new bool[5];

            //총 3회만 시도해보자
            int iTryCount = 3;
            bool bProcessSuccess = false;

            while (iTryCount > 0)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) break;

                CheckResetBusStatus();

                Thread.Sleep(200);

                tmpCanMsg = new TPCANMsg();

                switch (iSession)  //부팅 및 세션유지
                {
                    case 5:
                    case 10:
                        Send(strBootID, strCanBootMsg, ref strRtnMsg, "", ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                        break;
                    case 15:
                        Send(strBootID, strCanBootMsg, ref strRtnMsg, "", ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                        //Send("10DBFEF1", strCanSessMsg, ref strRtnMsg, "", ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0, false);
                        Send("14DA97F1", strCanSessMsg, ref strRtnMsg, "", ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0, false);                        
                        break;
                    default:                        
                        if (iSession > 15) iSession = 1;
                        break;
                }
                iSession++;
                 
                
                switch (iSequence)
                {

                    case 20: // ALDL 45E9 값 사전취득
                        if (!String.IsNullOrEmpty(strALDL_45E9)) break;
                        if (sModel.Contains("GEN11"))
                        {
                            if (Send("14DA97F1", strCanAldlMsg, ref strRtnMsg, "EXTENDED_READ_ALDL_45E9", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                            {
                                if (tmpCanMsg.DATA != null)
                                {
                                    strALDL_45E9 = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                                    //strCanStepMsg6 = System.Text.RegularExpressions.Regex.Split("06 2E 45 E9", " ");
                                    //Sample "06 62 45 E9 32 12 00 00"

                                    string[] strArray_45E9 = System.Text.RegularExpressions.Regex.Split(strALDL_45E9, " ");
                                    byte bChangeByte = ChangeSecurityByte(tmpCanMsg.DATA[5], 3, false); //다섯번째 바이트만 건드린다. disable 0, enable 1   1111 0000 -> 1110 0000 [bit4]

                                    strCanStepMsg6[4] = strArray_45E9[4];
                                    strCanStepMsg6[5] = bChangeByte.ToString("X2");
                                    strCanStepMsg6[6] = strArray_45E9[6];
                                    strCanStepMsg6[7] = strArray_45E9[7];

                                    if (!strALDL_45E9.IndexOf("06 62 45 E9").Equals(0)) strALDL_45E9 = String.Empty;
                                    else
                                        iTryCount = 3;
                                }
                            }
                            else
                            {
                                iTryCount--;
                            }
                        }
                        else if (sModel.Contains("GEN12"))            //UNLOCK_GEN12_GB, 5FF0
                        {
                            if (Send("14DA97F1", strCanAldlMsg_Gen12, ref strRtnMsg, "EXTENDED_READ_ALDL_5FF0", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                            {
                                if (tmpCanMsg.DATA != null)
                                {
                                    strALDL_45E9 = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                                    //strCanStepMsg6 = System.Text.RegularExpressions.Regex.Split("04 2E 5F F0", " ");
                                    //Sample "04 62 5F F0 32 00 00 00"  //1BYTE만.

                                    string[] strArray_45E9 = System.Text.RegularExpressions.Regex.Split(strALDL_45E9, " ");
                                    byte bChangeByte = ChangeSecurityByte(tmpCanMsg.DATA[4], 0, false); //disable 0, enable 1   1111 0000 -> 0111 0000 [bit7] 거꾸로임. 

                                    strCanStepMsg6_gen12[4] = bChangeByte.ToString("X2");
                                    strCanStepMsg6_gen12[5] = strArray_45E9[5];
                                    strCanStepMsg6_gen12[6] = strArray_45E9[6];
                                    strCanStepMsg6_gen12[7] = strArray_45E9[7];

                                    if (!strALDL_45E9.IndexOf("04 62 5F F0").Equals(0)) strALDL_45E9 = String.Empty;
                                    else
                                        iTryCount = 3;
                                }
                            }
                            else
                            {
                                iTryCount--;
                            }
                        }
                        break;
                   
                    default: break;
                }

                iSequence++;

                if (iSequence > 20) iSequence = 1;

                if (String.IsNullOrEmpty(strALDL_45E9))
                {                    
                    continue;
                }               

                //step1 - session open
                if (!bStep[0])
                {
                    if (Send("14DA97F1", strCanStepMsg1, ref strRtnMsg, "EXTENDED_SESSION", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                    {
                        if (tmpCanMsg.DATA != null)
                        {
                            strSession = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                            if (strSession.IndexOf("06 50 03").Equals(0))
                            {
                                iTryCount = 3;
                                bStep[0] = true;
                            }
                            iTryCount--;
                            continue;
                        }
                    }
                }                

                //step2 - READ SEED                 
                bool bSeedSuccess = false;
                if (!bStep[1])
                {
                    if (Send("14DA97F1", strCanStepMsg2, ref strRtnMsg, "EXTENDED_SEED", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                    {
                        if (tmpCanMsg.DATA != null)
                        {
                            strSeed = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                            if (strSeed.IndexOf("10 21").Equals(0))
                            {
                                bSeedSuccess = Send("14DA97F1", strCanFlowControl, ref strRtnMsg, "EXTENDED_SEED", ref tmpCanMsg, "14DAF197", (int)MODE.MULTIPLE, false, 0.1, false, 4);
                            }

                            if (bSeedSuccess)
                            {
                                iTryCount = 3;
                                bStep[1] = true;                                
                            }
                            iTryCount--;
                            continue;
                        }
                    }
                }
               
                //step3 - Send GB Key for Security Access ??
                bool bSecurityAccess = false;
                if (!bStep[2])
                {
                    if (Send("14DA97F1", strCanStepMsg3, ref strRtnMsg, "EXTENDED_SENDKEY", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                    {
                        if (tmpCanMsg.DATA != null)
                        {
                            strKey = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                            if (strKey.IndexOf("30 00 00").Equals(0))
                            {
                                bSecurityAccess = Send("14DA97F1", strCanStepMsg4, ref strRtnMsg, "EXTENDED_SENDKEY", ref tmpCanMsg, "14DAF197", (int)MODE.SEND, false, 0.1, false);
                                 bSecurityAccess = false;
                                if (Send("14DA97F1", strCanStepMsg5, ref strRtnMsg, "EXTENDED_SENDKEY", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                                {
                                    if (tmpCanMsg.DATA != null)
                                    {
                                        strKey = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                                        if (strKey.IndexOf("02 67 02").Equals(0))
                                        {
                                            bSecurityAccess = true;
                                        }
                                    }
                                }
                            }

                            if (bSecurityAccess)
                            {
                                iTryCount = 3;
                                bStep[2] = true;                                
                            }
                            iTryCount--;
                            continue;
                        }
                    }
                }
               
                //step4 - Write ALDL 45E9
                bool bWriteALDL = false;
                if (!bStep[3])
                {
                    if (sModel.Contains("GEN11"))
                    {
                        if (Send("14DA97F1", strCanStepMsg6, ref strRtnMsg, "EXTENDED_SECURE_LOGGING_DISABLE", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                        {
                            if (tmpCanMsg.DATA != null)
                            {
                                strSecurityChange = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                                bWriteALDL = strSecurityChange.IndexOf("03 6E 45 E9").Equals(0);

                                if (bWriteALDL)
                                {
                                    bProcessSuccess = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (sModel.Contains("GEN12"))
                    {

                        if (Send("14DA97F1", strCanStepMsg6_gen12, ref strRtnMsg, "EXTENDED_SECURE_LOGGING_DISABLE", ref tmpCanMsg, "14DAF197", (int)MODE.SENDRECV, false, 0.2, false))
                        {
                            if (tmpCanMsg.DATA != null)
                            {
                                strSecurityChange = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                                bWriteALDL = strSecurityChange.IndexOf("03 6E 5F F0").Equals(0);

                                if (bWriteALDL)
                                {
                                    bProcessSuccess = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                iTryCount--;

                if (iTryCount < 1) break;
            }

            return bProcessSuccess;
        }

        private byte ChangeSecurityByte(byte bOrigin, int iIdx, bool bEnable)
        {            
            // (1) byte를 비트문자열로 변환                        
            StringBuilder sb = new StringBuilder();

            sb.Append(Convert.ToString(bOrigin, 2).PadLeft(8, '0'));

            //if (bEnable)
            //    sb[3] = '1';
            //else
            //    sb[3] = '0';

            if (bEnable)
                sb[iIdx] = '1';
            else
                sb[iIdx] = '0';

            // (2) 비트문자열을 byte로 변환
            string bitStr = sb.ToString();
            return Convert.ToByte(bitStr, 2);

        }

        private void AutoSendingGB()
        {
            //WAKE UP & Telltale ON 메세지 만들기
            string[] strCanGBMessage1 = new string[8];
            string[] strCanGBMessage2 = new string[6];

            strCanGBMessage1 = System.Text.RegularExpressions.Regex.Split("45 40 80 00 00 00 00 00", " ");
            strCanGBMessage2 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 10 00", " ");
            
            int iSequence = 0;
            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);

                if (ThreadEvent_Pause.WaitOne())
                {
                    dtm = DateTime.Now;

                    while (!DelayTimeCheck(dtm, 247))
                    {
                        if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                        CheckResetBusStatus();
                        Thread.Sleep(1);
                    }

                    tmpCanMsg = new TPCANMsg();

                    switch (iSequence)
                    {
                        
                        case 0:
                            Send("284", strCanGBMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);                                                       
                            break;

                        case 1:
                            Send("284", strCanGBMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            Send("145", strCanGBMessage1, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            iSequence = -1;
                            break;

                        default:
                            iSequence = -1;
                            break;
                    }
                    iSequence++;


                }
                else
                {
                    if (!ThreadEvent_Off.WaitOne())
                    {
                        return;
                    }
                }
            }
        }

        private void AutoSendingGB_500()
        {
            //WAKE UP & Telltale ON 메세지 만들기
            string[] strCanGBMessage1 = new string[8];
            string[] strCanGBMessage2 = new string[6];

            //strCanGBMessage1 = System.Text.RegularExpressions.Regex.Split("45 40 80 00 00 00 00 00", " ");
            //strCanGBMessage2 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 10 00", " ");
            
            //20240619
            strCanGBMessage1 = System.Text.RegularExpressions.Regex.Split("47 40 80 00 00 00 00 00", " ");
            strCanGBMessage2 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 00 00 20 00", " ");

            int iSequence = 0;
            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);

                if (ThreadEvent_Pause.WaitOne())
                {
                    dtm = DateTime.Now;

                    while (!DelayTimeCheck(dtm, 495))
                    {
                        if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                        CheckResetBusStatus();
                        Thread.Sleep(1);
                    }

                    tmpCanMsg = new TPCANMsg();

                    switch (iSequence)
                    {
                        case 0:
                            //Send("145", strCanGBMessage1, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            //20240619
                            Send("147", strCanGBMessage1, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            break;

                        case 1:
                            //Send("284", strCanGBMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);                            
                            //20240619
                            Send("36E", strCanGBMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            iSequence = -1;
                            break;
                        default:
                            iSequence = -1;
                            break;
                    }
                    iSequence++;
                }
                else
                {
                    if (!ThreadEvent_Off.WaitOne())
                    {
                        return;
                    }
                }
            }
        }

        private void AutoSendingGB_MY23()
        {
            //WAKE UP & Telltale ON 메세지 만들기
            string[] strCanGBMessage1 = new string[8];
            string[] strCanGBMessage2 = new string[8];

            strCanGBMessage1 = System.Text.RegularExpressions.Regex.Split("45 40 80 00 00 00 00 00", " ");
            strCanGBMessage2 = System.Text.RegularExpressions.Regex.Split("00 00 00 00 00 00 10 00", " ");

            int iSequence = 0;
            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);

                if (ThreadEvent_Pause.WaitOne())
                {
                    dtm = DateTime.Now;

                    while (!DelayTimeCheck(dtm, 247))
                    {
                        if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                        CheckResetBusStatus();
                        Thread.Sleep(1);
                    }

                    tmpCanMsg = new TPCANMsg();

                    switch (iSequence)
                    {

                        case 0:
                            Send("36E", strCanGBMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            break;

                        case 1:
                            Send("36E", strCanGBMessage2, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            Send("145", strCanGBMessage1, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                            iSequence = -1;
                            break;

                        default:
                            iSequence = -1;
                            break;
                    }
                    iSequence++;


                }
                else
                {
                    if (!ThreadEvent_Off.WaitOne())
                    {
                        return;
                    }
                }
            }
        }

        private void AutoSendingGEM()
        {
            //WAKE UP 메세지 만들기
            string[] strCanGemMessage = new string[8];

            strCanGemMessage = System.Text.RegularExpressions.Regex.Split("40 40 FF FF FF FF FF FF", " ");
            
            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1);

                if (ThreadEvent_Pause.WaitOne())
                {
                    dtm = DateTime.Now;

                    while (!DelayTimeCheck(dtm, 635))
                    {
                        if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;
                        CheckResetBusStatus();
                        Thread.Sleep(1);
                    }

                    tmpCanMsg = new TPCANMsg();

                    Send("621", strCanGemMessage, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                    
                }
                else
                {
                    if (!ThreadEvent_Off.WaitOne())
                    {
                        return;
                    }
                }
            }
        }

        private void AutoSendingMCTM()
        {
            //MCTM LSCAN BOOTING message 만들기
            string[] strPsaMessage1 = new string[8];

            strPsaMessage1 = System.Text.RegularExpressions.Regex.Split("00 52 00 00 00 00 00 00", " ");  //BOOT500 //0x0F6

            string strRtnMsg = String.Empty;
            TPCANMsg tmpCanMsg = new TPCANMsg();
            DateTime dtm = DateTime.Now;

            while (true)
            {
                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

                Thread.Sleep(1000);
                if (ThreadEvent_Pause.WaitOne())
                {
                    CheckResetBusStatus();
                    Send("621", strPsaMessage1, ref strRtnMsg, "" /*"PSA_WAKEUP1"*/, ref tmpCanMsg, "00", (int)MODE.SEND, true, 1.0);
                }
                
            }
        }

        private void KillThreadObject(Thread theThread)
        {
            try
            {
                if (theThread != null)
                {
                    if (theThread.IsAlive)
                    {   //ReadThread 스레드가 살아있으면 최대 1초간 자연사 대기
                        //theThread.Join(100);
                    }

                    if (theThread.IsAlive)
                    {   //ReadThread 스레드가 그래도 살아있으면 강제종료
                        theThread.Abort();
                                               
                    }
                }                               
            }
            catch { }
        }

        public void PauseThreadObject(bool bPause)
        {
            if (bPause)
                ThreadEvent_Pause.Reset();
            else
                ThreadEvent_Pause.Set();
        }

        private void CloseThreadObject(bool bPause)
        {
            if (bPause)
                ThreadEvent_Off.Reset();
            else
                ThreadEvent_Off.Set();
        }

        public void CloseSendThread()
        {
            PauseThreadObject(true);
            CloseThreadObject(true);
            KillThreadObject(SendThread);
            KillThreadObject(RecvThread);            
        }

    }
}
