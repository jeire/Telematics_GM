//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.IO;
//using System.IO.Compression;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;


//namespace GmTelematics
//{
//    class DK_ETHERNET
//    {
//        public event EventRealTimeMsg EtherNetRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
//        public event EventDKCOM CommSendReport;                        //대리자가 날릴 실제 이벤트 메소드
//        private Socket sockInterface;
//        private bool bConnected = false;

//        private string strConnectedIP = String.Empty;

//        private List<byte> OutBuffer;
//        private DK_LOGGER DKLogger;

//        private string Item_strSendPack = String.Empty;
//        private string Item_strCommandName = String.Empty;

//        private Thread threadCommandChecker = null;

//        private object lockobject = new object();
//        private byte[] bSendPacks = new byte[10];

//        private string strLogTitle = String.Empty;

//        private string strSn = String.Empty;
//        private string strStation = String.Empty;
//        private string strViewStation = String.Empty;
//        private string strJobName = String.Empty;
//        private string strResultOKNG = String.Empty;
//        private string strItems = String.Empty;

//        public DK_ETHERNET(string strName)
//        {
//            strLogTitle = strName;
//            Initialize();
//        }

//        ~DK_ETHERNET()
//        {
//            Disconnect(false);
//        }

//        private void SaveLog(string strCommandName, string strLog)
//        {
//            strLog = strLog.Replace("\n", "[CR]");
//            if (strCommandName.Length > 0)
//            {
//                DKLogger.WriteCommLog(strLog, strLogTitle + ":" + strCommandName, false);

//            }
//            else
//            {
//                DKLogger.WriteCommLog(strLog, strLogTitle + ":" + strCommandName, false);
//            }
//        }

//        private void Initialize()
//        {
//            DKLogger = new DK_LOGGER("SET", false);
//            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_EtherNet);

//            strConnectedIP = String.Empty;
//            bConnected = false;

//            OutBuffer = new List<byte>();
//        }

//        private void GateWay_EtherNet(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
//        {
//            EtherNetRealTimeTxRxMsg(0, cParam);
//        }

//        private void DataUpdate(int iMode, byte[] bData)
//        {
//            lock (lockobject)
//            {
//                switch (iMode)
//                {
//                    case (int)LISTDBOPTION.CLEAR: OutBuffer.Clear(); break;
//                    case (int)LISTDBOPTION.INSERT: OutBuffer.AddRange(bData); break;
//                    default: return;
//                }
//            }
//        }

//        public void Disconnect(bool bLog)
//        {
//            try
//            {
//                if (sockInterface != null)
//                {
//                    if (IsConnected())
//                    {
//                        sockInterface.Disconnect(false);
//                        if (bLog)
//                            SaveLog("", "[RX]SOCKET DICONNECTED");

//                    }
//                    sockInterface.Close();
//                    sockInterface.Dispose();
//                }

//            }
//            catch { }

//            bConnected = false;

//        }

//        private void Connect(string strConnIP, int iConnPort, double dTimeOut, string strCommandName)
//        {
//            Disconnect(false);
//            byte[] bTmps = new byte[1];
//            DataUpdate((int)LISTDBOPTION.CLEAR, bTmps);
//            ETHERNETSTRUCT tmpCmdPack = new ETHERNETSTRUCT();
//            tmpCmdPack.dTimeOut = dTimeOut;
//            tmpCmdPack.iCommandType = (int)ETHERNETCOMMAND.NONE;
//            tmpCmdPack.strSendPack = String.Empty;
//            sockInterface = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

//            try
//            {
//                strConnectedIP = strConnIP;
//                SaveLog(strCommandName, "[TX]CONNTECT TO " + strConnIP + " (" + iConnPort.ToString() + ")");
//                sockInterface.BeginConnect(strConnIP, iConnPort, new AsyncCallback(ConnBack), sockInterface);
//            }
//            catch
//            {
//                Disconnect(false);
//            }
//        }

//        public void SetSN(string strValue)
//        {
//            strSn = strValue;
//        }

//        public void SetStation(string strValue)
//        {
//            strStation = strValue;
//        }

//        public void SetResult(string strValue)
//        {
//            strResultOKNG = strValue;
//        }

//        public void SetItems(string strValue)
//        {
//            strItems = strValue;
//        }

//        public void SetViewStation(string strValue)
//        {
//            strViewStation = strValue;
//        }

//        public void SetJobFileName(string strValue)
//        {
//            strJobName = strValue;
//        }

//        public void LauncherRecvThread2(ETHERNETSTRUCT param) //for DBMS
//        {

//            KillThreadObject(threadCommandChecker);
//            try
//            {
//                byte[] bTmps = new byte[1];
//                DataUpdate((int)LISTDBOPTION.CLEAR, bTmps);
//            }
//            catch { }
//            Thread.Sleep(1);
//            if (param.iCommandType.Equals((int)ETHERNETCOMMAND.DISCONNECT))
//            {
//                SaveLog(param.strCommandName, "[TX]SOCKET DICONNECTED");
//                Disconnect(false);
//                SaveLog(param.strCommandName, "[RX]OK");
//                SendToGateWay((int)STATUS.OK, "OK", "OK"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                return;

//            }
//            threadCommandChecker = new Thread(new ParameterizedThreadStart(runThreadFunction2));
//            threadCommandChecker.Start(param);

//            switch (param.iCommandType)
//            {
//                case (int)ETHERNETCOMMAND.CONNECT:
//                    string[] strConnInfo = param.strParam.Split('/');
//                    if (strConnInfo.Length.Equals(2))
//                    {
//                        Connect(strConnInfo[0], int.Parse(strConnInfo[1]), param.dTimeOut, param.strCommandName);
//                    }
//                    else
//                    {
//                        SendToGateWay((int)STATUS.CHECK, "Error Connection Info", "Error Connection Info");
//                        KillThreadObject(threadCommandChecker); return;
//                    }
//                    break;

//                case (int)ETHERNETCOMMAND.SEND:
//                case (int)ETHERNETCOMMAND.SENDRECV:

//                    if (String.IsNullOrEmpty(strStation) || String.IsNullOrEmpty(strSn))
//                    {
//                        SendToGateWay((int)STATUS.CHECK, "UnRegistered Station/Sn Info", "UnRegistered Station/Sn Info");
//                        KillThreadObject(threadCommandChecker); return;
//                    }

//                    string strLogMsg = String.Empty;
//                    switch (param.strSendPack)
//                    {
//                        case "STEP_CHECK":
//                            SendDBMSData("D01", strStation, ref bSendPacks, ref strLogMsg);
//                            break;
//                        case "STEP_COMPLETE":
//                            SendDBMSData("D02", strStation, ref bSendPacks, ref strLogMsg);
//                            break;
//                        case "STEP_VIEW":
//                            SendDBMSData("D03", strViewStation, ref bSendPacks, ref strLogMsg);
//                            break;
//                    }
//                    sendSocketData2(bSendPacks, param.iCommandType, param.strCommandName, strLogMsg);
//                    break;



//                default: KillThreadObject(threadCommandChecker); return;
//            }
//        }

//        //클라이언트쪽 STEPCHECK/STEPCOMPLETE/VIEW 요청부분
//        public void SendDBMSData(string strCommandCode, string strStationName, ref byte[] bSendPacks, ref string strLogMsg)
//        {
//            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
//            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
//            //  0x0102 ,                                 , 0x0304

//            string strMessage = strLogMsg = strCommandCode + "|" + strStationName + "|" + strSn + "|" + strResultOKNG + "|" + strJobName + "|" + strItems;
//            if (!IsConnected()) return;


//            int istrMsgBytesCount = Encoding.UTF8.GetByteCount(strMessage);
//            byte[] Sendbytes = new byte[2 + 3 + 4 + istrMsgBytesCount + 2];
//            byte[] Commandbytes = Encoding.UTF8.GetBytes(strCommandCode);
//            byte[] Databytes = Encoding.UTF8.GetBytes(strMessage);
//            byte[] Lengthbytes = BitConverter.GetBytes(Databytes.Length);

//            Sendbytes[0] = 0x01;
//            Sendbytes[1] = 0x02;

//            Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
//            Array.Copy(Lengthbytes, 0, Sendbytes, 5, Lengthbytes.Length);
//            Array.Copy(Databytes, 0, Sendbytes, 9, Databytes.Length);

//            Sendbytes[Sendbytes.Length - 2] = 0x03;
//            Sendbytes[Sendbytes.Length - 1] = 0x04;

//            bSendPacks = new byte[Sendbytes.Length];
//            Array.Copy(Sendbytes, bSendPacks, Sendbytes.Length);
//        }

//        public bool NetworkPingTest(string strIpaddress, ref string strResult)
//        {
//            StringBuilder sbResultText = new StringBuilder(4096);
//            try
//            {
//                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
//                System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
//                options.DontFragment = true;
//                string data = "Are You There?";
//                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(data);
//                int timeout = 200;
//                System.Net.NetworkInformation.PingReply reply = ping.Send(System.Net.IPAddress.Parse(strIpaddress), timeout, buffer, options);


//                switch (reply.Status)
//                {
//                    case System.Net.NetworkInformation.IPStatus.Success:

//                        sbResultText.Append("PING ");
//                        sbResultText.Append(reply.Address.ToString());

//                        sbResultText.Append(" TTL ");
//                        sbResultText.Append(reply.Options.Ttl.ToString());
//                        sbResultText.Append("ms");
//                        break;
//                    case System.Net.NetworkInformation.IPStatus.TimedOut:
//                        sbResultText.Append("PING ");
//                        sbResultText.Append(strIpaddress);

//                        sbResultText.Append(" Destination Host Unreachable");
//                        break;
//                    default:
//                        sbResultText.Append("PING FAIL - ");
//                        sbResultText.Append(reply.Status.ToString());
//                        break;
//                }

//                strResult = sbResultText.ToString();
//                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
//                    return true;
//                else
//                    return false;
//            }

//            catch (Exception ex)
//            {
//                try
//                {
//                    sbResultText.Clear();
//                    sbResultText.Append("PING FAIL - ");
//                    sbResultText.Append(ex.InnerException.Message.ToString());
//                    strResult = sbResultText.ToString();
//                }
//                catch
//                {
//                    sbResultText.Clear();
//                    sbResultText.Append("PING FAIL - ");
//                    sbResultText.Append(ex.Message.ToString());
//                    strResult = sbResultText.ToString();
//                }

//                return false;
//            }
//        }

//        private string GetHostIPAddress()
//        {
//            return strConnectedIP;
//        }

//        private void runThreadFunction2(object obj) //for DBMS
//        {
//            ETHERNETSTRUCT tmpCmdPack = (ETHERNETSTRUCT)obj;

//            DateTime dtm = DateTime.Now;

//            while (true)
//            {
//                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

//                Thread.Sleep(5);

//                if (!RecvTimeOutCheck(dtm, tmpCmdPack.dTimeOut))
//                {
//                    string strReason = String.Empty;
//                    string strLog = String.Empty;
//                    NetworkPingTest(GetHostIPAddress(), ref strReason);
//                    strLog = "TIME OUT (" + strReason + ")";

//                    string strbytes = String.Empty;
//                    if (OutBuffer.Count > 1)
//                    {
//                        byte[] bBytes = OutBuffer.ToArray();
//                        strbytes = BitConverter.ToString(bBytes).Replace("-", " ");

//                        strLog = strbytes + " - " + strLog;
//                    }

//                    SaveLog(tmpCmdPack.strCommandName, "[RX]" + strLog);
//                    SendToGateWay((int)STATUS.TIMEOUT, "TIME OUT", "TIME OUT"); // ACTOR의 게이트웨이로 결과전송  
//                    return;
//                }

//                switch (tmpCmdPack.iCommandType)
//                {
//                    case (int)ETHERNETCOMMAND.CONNECT:
//                        if (IsConnected())
//                        {
//                            SaveLog(tmpCmdPack.strCommandName, "[RX] HOST CONNECTED");
//                            SendToGateWay((int)STATUS.OK, "CONNECTED", "CONNECTED"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                            return;
//                        }
//                        break;
//                    case (int)ETHERNETCOMMAND.SEND: return;
//                    case (int)ETHERNETCOMMAND.SENDRECV:
//                        if (ScanningDBMSRecvBuffer())
//                        {
//                            return;
//                        }
//                        break;
//                    default:
//                        SendToGateWay((int)STATUS.NG, "Unknown Command", "Unknown Command"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return;

//                }
//            }
//        }

//        private bool ScanningDBMSRecvBuffer()
//        {
//            if (OutBuffer.Count > 0)
//            {
//                //
//                string strGetdata = String.Empty;
//                string strFulldata = String.Empty;
//                byte[] bBytes = OutBuffer.ToArray();

//                int iRtnState = DBMSProcess(bBytes, ref strFulldata, ref strGetdata);

//                switch (iRtnState)
//                {
//                    case (int)STATUS.NG:
//                        SaveLog(Item_strCommandName, "[RX]" + strFulldata);
//                        SendToGateWay((int)STATUS.NG, strFulldata, strGetdata); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return true;
//                    case (int)STATUS.OK:
//                        SaveLog(Item_strCommandName, "[RX]" + strFulldata);
//                        SendToGateWay((int)STATUS.OK, strFulldata, strGetdata); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return true;

//                    case (int)STATUS.CHECK:
//                        SaveLog(Item_strCommandName, "[RX]" + strFulldata);
//                        SendToGateWay((int)STATUS.CHECK, strFulldata, strGetdata); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return true;

//                    case (int)STATUS.RUNNING:
//                    default: break;
//                }


//            }

//            return false;
//        }

//        private int DBMSProcess(byte[] bBytes, ref string strDataBase, ref string strResultValue)
//        {

//            try
//            {
//                //1. HEADER 분석

//                if (bBytes[0].Equals(0x01) && bBytes[1].Equals(0x02))
//                {
//                    //2. COMMAND 분석
//                    string strCommandCode = Encoding.UTF8.GetString(bBytes, 2, 3);

//                    int iSize = BitConverter.ToInt32(bBytes, 5);

//                    strDataBase = Encoding.UTF8.GetString(bBytes, 9, iSize);

//                    string[] strResult = strDataBase.Split('|');

//                    if (strResult.Length != 4)
//                    {
//                        strResultValue = strDataBase;
//                        return (int)STATUS.CHECK;
//                    }
//                    switch (strResult[0])
//                    {
//                        case "OK":
//                            strResultValue = strResult[1];
//                            return (int)STATUS.OK;

//                        case "NG":
//                            strResultValue = strResult[2];
//                            return (int)STATUS.NG;
//                        default:
//                            strResultValue = strDataBase;
//                            return (int)STATUS.CHECK;

//                    }



//                }

//                return (int)STATUS.RUNNING;
//            }
//            catch
//            {
//                return (int)STATUS.RUNNING;
//            }

//        }

//        private void ConnBack(IAsyncResult iar)
//        {
//            try
//            {
//                Socket tmpSock = (Socket)iar.AsyncState;
//                IPEndPoint svrEP = (IPEndPoint)tmpSock.RemoteEndPoint;

//                tmpSock.EndConnect(iar);
//                bConnected = true;
//                sockInterface = tmpSock;
//                EtherNetInfo eNetInfo = new EtherNetInfo();
//                eNetInfo.sockComm = sockInterface;
//                eNetInfo.ClearBuffer();
//                sockInterface.BeginReceive(eNetInfo.Buffer, 0, eNetInfo.Buffer.Length, SocketFlags.None, handleDataRecv, eNetInfo);
//                return;

//            }
//            catch
//            {
//                bConnected = false;
//                if (sockInterface != null)
//                {
//                    sockInterface.Close();
//                    sockInterface.Dispose();
//                }
//            }
//        }

//        public bool IsConnected()
//        {
//            return bConnected;
//        }

//        public string GetConnectedMyIp()
//        {
//            if (IsConnected())
//                return IPAddress.Parse(((IPEndPoint)sockInterface.LocalEndPoint).Address.ToString()).ToString();
//            else
//                return "0.0.0.0";
//        }

//        private void handleDataRecv(IAsyncResult IAR)
//        {
//            EtherNetInfo tmpNetInfo = (EtherNetInfo)IAR.AsyncState;

//            int iRecvBytes = 0;

//            try
//            {
//                iRecvBytes = tmpNetInfo.sockComm.EndReceive(IAR);
//            }
//            catch
//            {
//                return;
//            }

//            if (iRecvBytes > 0)
//            {
//                if (StateThreadObject(threadCommandChecker)) //RECV 중이면 데이터 업데이트
//                {
//                    //수신바이트 내용 리스트에 업데이트
//                    byte[] tmpBytes = new byte[iRecvBytes];
//                    Array.Copy(tmpNetInfo.Buffer, 0, tmpBytes, 0, tmpBytes.Length);
//                    DataUpdate((int)LISTDBOPTION.INSERT, tmpBytes);
//                }

//                try
//                {
//                    //재수신 모드
//                    tmpNetInfo.ClearBuffer();
//                    tmpNetInfo.sockComm.BeginReceive(tmpNetInfo.Buffer, 0, tmpNetInfo.Buffer.Length, SocketFlags.None, handleDataRecv, tmpNetInfo);

//                }
//                catch
//                {
//                    return;
//                }
//            }
//            else
//            {
//                Disconnect(true);
//            }
//        }

//        private void handleDataSend(IAsyncResult IAR)
//        {
//            if (!IsConnected()) return;

//            EtherNetInfo tmpNetInfo = (EtherNetInfo)IAR.AsyncState;

//            int iRecvBytes = 0;

//            try
//            {
//                iRecvBytes = tmpNetInfo.sockComm.EndSend(IAR);

//            }
//            catch (Exception ex)
//            {
//                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
//                STEPMANAGER_VALUE.DebugView(strExMsg);
//            }
//        }

//        private void sendSocketData2(byte[] Sendbytes, int iSendOption, string strCommandName, string strLogMsg)
//        {
//            if (!IsConnected()) return;

//            Item_strCommandName = strCommandName; //RX 로깅때 쓰임.

//            try
//            {
//                EtherNetInfo tmpNetInfo = new EtherNetInfo();
//                tmpNetInfo.sockComm = sockInterface;
//                tmpNetInfo.sockComm.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSend, tmpNetInfo);
//                string strSendPack = Encoding.UTF8.GetString(Sendbytes);
//                SaveLog(strCommandName, "[TX]" + strLogMsg);

//                if (iSendOption.Equals((int)ETHERNETCOMMAND.SEND))
//                {
//                    KillThreadObject(threadCommandChecker);
//                    SendToGateWay((int)STATUS.OK, "OK", "OK"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                }
//            }
//            catch (Exception ex)
//            {
//                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
//                STEPMANAGER_VALUE.DebugView(strExMsg);
//            }
//        }

//        private void SendToGateWay(int iStatus, string strFulldata, string strResultMsg)
//        {
//            COMMDATA cdData = new COMMDATA();
//            cdData.iPortNum = (int)DEFINES.SET1;
//            cdData.iStatus = iStatus;
//            cdData.ResponseData = strFulldata;
//            cdData.ResultData = strResultMsg;
//            cdData.SendPacket = Item_strSendPack;
//            CommSendReport(cdData);

//        }

//        private void KillThreadObject(Thread theThread)
//        {
//            try
//            {
//                if (theThread != null)
//                {
//                    if (theThread.IsAlive)
//                    {   //ReadThread 스레드가 그래도 살아있으면 강제종료
//                        theThread.Abort();

//                    }
//                }
//            }
//            catch { }
//        }

//        private bool StateThreadObject(Thread theThread)
//        {
//            if (theThread != null)
//            {
//                return theThread.IsAlive;
//            }
//            return false;
//        }

//        private bool RecvTimeOutCheck(DateTime dtOut, double dTime)
//        {
//            DateTime dtCurr = DateTime.Now;
//            TimeSpan tsN = dtCurr - dtOut;

//            if (tsN.TotalSeconds > dTime)
//            {
//                return false;
//            }
//            return true;
//        }
//    }

//    //For TESTCOM TC1400A
//    class DK_ETHERNET_TC1400A
//    {
//        public event EventRealTimeMsg EtherNetRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
//        public event EventDKCOM CommSendReport;                        //대리자가 날릴 실제 이벤트 메소드
//        private Socket sockInterface;
//        private bool bConnected = false;

//        private string strConnectedIP = String.Empty;

//        private List<byte> OutBuffer;
//        private DK_LOGGER DKLogger;

//        private string Item_strSendPack = String.Empty;
//        private string Item_strCommandName = String.Empty;

//        private Thread threadCommandChecker = null;

//        private object lockobject = new object();
//        private DK_ANALYZER_TC1400A DKANL_TC1400A = new DK_ANALYZER_TC1400A();     //테스콤 T1400A  프로토콜 분석 클래스

//        private byte[] bSendPacks = new byte[10];

//        private string strLogTitle = String.Empty;

//        private string strSn = String.Empty;
//        private string strStation = String.Empty;
//        private string strViewStation = String.Empty;
//        private string strJobName = String.Empty;
//        private string strResult = String.Empty;

//        public DK_ETHERNET_TC1400A(string strName)
//        {
//            strLogTitle = strName;
//            Initialize();
//        }

//        ~DK_ETHERNET_TC1400A()
//        {
//            Disconnect(false);
//        }

//        private void SaveLog(string strCommandName, string strLog)
//        {
//            strLog = strLog.Replace("\n", "[CR]");
//            if (strCommandName.Length > 0)
//            {
//                DKLogger.WriteCommLog(strLog, strLogTitle + ":" + strCommandName, false);

//            }
//            else
//            {
//                DKLogger.WriteCommLog(strLog, strLogTitle + ":" + strCommandName, false);
//            }
//        }

//        private void Initialize()
//        {
//            DKLogger = new DK_LOGGER("SET", false);
//            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_EtherNet);

//            strConnectedIP = String.Empty;
//            bConnected = false;

//            OutBuffer = new List<byte>();
//        }

//        private void GateWay_EtherNet(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
//        {
//            EtherNetRealTimeTxRxMsg(0, cParam);
//        }

//        public void Disconnect(bool bLog)
//        {
//            try
//            {
//                if (sockInterface != null)
//                {
//                    if (IsConnected())
//                    {
//                        sockInterface.Disconnect(false);
//                        if (bLog)
//                            SaveLog("", "[RX]SOCKET DICONNECTED");

//                    }
//                    sockInterface.Close();
//                    sockInterface.Dispose();
//                }

//            }
//            catch { }

//            bConnected = false;

//        }

//        private void Connect(string strConnIP, int iConnPort, double dTimeOut, string strCommandName)
//        {
//            Disconnect(false);
//            byte[] bTmps = new byte[1];
//            DataUpdate(bTmps, true);
//            ETHERNETSTRUCT tmpCmdPack = new ETHERNETSTRUCT();
//            tmpCmdPack.dTimeOut = dTimeOut;
//            tmpCmdPack.iCommandType = (int)ETHERNETCOMMAND.NONE;
//            tmpCmdPack.strSendPack = String.Empty;
//            sockInterface = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

//            try
//            {
//                strConnectedIP = strConnIP;
//                SaveLog(strCommandName, "[TX]CONNTECT TO " + strConnIP + " (" + iConnPort.ToString() + ")");
//                sockInterface.BeginConnect(strConnIP, iConnPort, new AsyncCallback(ConnBack), sockInterface);
//            }
//            catch
//            {
//                Disconnect(false);
//            }
//        }

//        public void LauncherRecvThread(ETHERNETSTRUCT param)
//        {
//            KillThreadObject(threadCommandChecker);

//            try
//            {
//                byte[] bTmps = new byte[1];
//                DataUpdate(bTmps, true);
//            }
//            catch { }

//            Thread.Sleep(1);
//            if (param.iCommandType.Equals((int)ETHERNETCOMMAND.DISCONNECT))
//            {
//                SaveLog(param.strCommandName, "[TX]SOCKET DICONNECTED");
//                Disconnect(false);
//                SaveLog(param.strCommandName, "[RX]OK");
//                SendToGateWay((int)STATUS.OK, "OK", "OK"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                return;

//            }
//            threadCommandChecker = new Thread(new ParameterizedThreadStart(runThreadFunction));
//            threadCommandChecker.Start(param);

//            switch (param.iCommandType)
//            {
//                case (int)ETHERNETCOMMAND.CONNECT:
//                    string[] strConnInfo = param.strParam.Split('/');
//                    if (strConnInfo.Length.Equals(2))
//                    {
//                        Connect(strConnInfo[0], int.Parse(strConnInfo[1]), param.dTimeOut, param.strCommandName);
//                    }
//                    else
//                    {
//                        SendToGateWay((int)STATUS.CHECK, "Error Connection Info", "Error Connection Info");
//                        KillThreadObject(threadCommandChecker); return;
//                    }
//                    break;

//                case (int)ETHERNETCOMMAND.SEND:
//                case (int)ETHERNETCOMMAND.SENDRECV:
//                    string strReason = String.Empty;
//                    if (MakePackets(param, ref bSendPacks, ref strReason))
//                    {
//                        sendSocketData(bSendPacks, param.iCommandType, param.strCommandName);
//                    }
//                    else
//                    {
//                        SendToGateWay((int)STATUS.CHECK, strReason, strReason);
//                        KillThreadObject(threadCommandChecker); return;
//                    }
//                    break;



//                default: KillThreadObject(threadCommandChecker); return;
//            }
//        }

//        private bool MakePackets(ETHERNETSTRUCT eParam, ref byte[] MadePacketBytes, ref string strReason)
//        {
//            strReason = String.Empty;

//            if (eParam.strSendPack.Length < 1)
//            {
//                strReason = "SEND PACK ERROR";
//                return false;
//            }

//            if (eParam.strSendPack.Contains("<DATA>") && eParam.strParam.Length < 1)
//            {
//                strReason = "PAR1 ERROR";
//                return false;
//            }

//            try
//            {
//                MadePacketBytes = Encoding.UTF8.GetBytes(eParam.strSendPack);
//                Item_strSendPack = eParam.strSendPack;
//                return true;
//            }
//            catch
//            {
//                strReason = "MAKE PACKET ERROR Ex";
//                return false;
//            }

//        }

//        public bool NetworkPingTest(string strIpaddress, ref string strResult)
//        {
//            StringBuilder sbResultText = new StringBuilder(4096);
//            try
//            {
//                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
//                System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
//                options.DontFragment = true;
//                string data = "Are You There?";
//                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(data);
//                int timeout = 200;
//                System.Net.NetworkInformation.PingReply reply = ping.Send(System.Net.IPAddress.Parse(strIpaddress), timeout, buffer, options);


//                switch (reply.Status)
//                {
//                    case System.Net.NetworkInformation.IPStatus.Success:

//                        sbResultText.Append("PING ");
//                        sbResultText.Append(reply.Address.ToString());

//                        sbResultText.Append(" TTL ");
//                        sbResultText.Append(reply.Options.Ttl.ToString());
//                        sbResultText.Append("ms");
//                        break;
//                    case System.Net.NetworkInformation.IPStatus.TimedOut:
//                        sbResultText.Append("PING ");
//                        sbResultText.Append(strIpaddress);

//                        sbResultText.Append(" Destination Host Unreachable");
//                        break;
//                    default:
//                        sbResultText.Append("PING FAIL - ");
//                        sbResultText.Append(reply.Status.ToString());
//                        break;
//                }

//                strResult = sbResultText.ToString();
//                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
//                    return true;
//                else
//                    return false;
//            }

//            catch (Exception ex)
//            {
//                try
//                {
//                    sbResultText.Clear();
//                    sbResultText.Append("PING FAIL - ");
//                    sbResultText.Append(ex.InnerException.Message.ToString());
//                    strResult = sbResultText.ToString();
//                }
//                catch
//                {
//                    sbResultText.Clear();
//                    sbResultText.Append("PING FAIL - ");
//                    sbResultText.Append(ex.Message.ToString());
//                    strResult = sbResultText.ToString();
//                }

//                return false;
//            }
//        }

//        private string GetHostIPAddress()
//        {
//            return strConnectedIP;
//        }

//        private void runThreadFunction(object obj)
//        {
//            ETHERNETSTRUCT tmpCmdPack = (ETHERNETSTRUCT)obj;

//            DateTime dtm = DateTime.Now;

//            while (true)
//            {
//                if (!STEPMANAGER_VALUE.bInteractiveMode && !STEPMANAGER_VALUE.bProgramRun) return;

//                Thread.Sleep(5);

//                if (!RecvTimeOutCheck(dtm, tmpCmdPack.dTimeOut))
//                {
//                    string strReason = String.Empty;
//                    string strLog = String.Empty;
//                    NetworkPingTest(GetHostIPAddress(), ref strReason);
//                    strLog = "TIME OUT (" + strReason + ")";

//                    string strbytes = String.Empty;
//                    if (OutBuffer.Count > 1)
//                    {
//                        byte[] bBytes = OutBuffer.ToArray();
//                        strbytes = BitConverter.ToString(bBytes).Replace("-", " ");

//                        SaveLog(tmpCmdPack.strCommandName, "[RX]" + strbytes);
//                    }
//                    else
//                    {
//                        SaveLog(tmpCmdPack.strCommandName, "[RX]" + strLog);
//                    }

//                    SendToGateWay((int)STATUS.TIMEOUT, "TIME OUT", "TIME OUT"); // ACTOR의 게이트웨이로 결과전송  
//                    return;
//                }

//                switch (tmpCmdPack.iCommandType)
//                {
//                    case (int)ETHERNETCOMMAND.CONNECT:
//                        if (IsConnected())
//                        {
//                            SaveLog(tmpCmdPack.strCommandName, "[RX] HOST CONNECTED");
//                            SendToGateWay((int)STATUS.OK, "CONNECTED", "CONNECTED"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                            return;
//                        }
//                        break;
//                    case (int)ETHERNETCOMMAND.SEND: return;
//                    case (int)ETHERNETCOMMAND.SENDRECV:
//                        if (ScanningRecvBuffer(tmpCmdPack.strSendPack, tmpCmdPack.strDataType))
//                        {
//                            return;
//                        }
//                        break;
//                    default:
//                        SendToGateWay((int)STATUS.NG, "Unknown Command", "Unknown Command"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return;

//                }
//            }
//        }

//        private bool ScanningRecvBuffer(string strSendPack, string strDataType)
//        {
//            if (OutBuffer.Count > 0)
//            {
//                string strGetdata = String.Empty;
//                byte[] bBytes = OutBuffer.ToArray();
//                int iRtnState = DKANL_TC1400A.AnalyzePacket(bBytes, ref strGetdata, strDataType, strSendPack);

//                string strbytes = String.Empty;

//                for (int p = 0; p < bBytes.Length; p++)
//                {
//                    strbytes += (char)bBytes[p];
//                }

//                strbytes += " (" + BitConverter.ToString(bBytes).Replace("-", " ") + ")";

//                switch (iRtnState)
//                {
//                    case (int)STATUS.ERROR:
//                    case (int)STATUS.CHECK:
//                        SaveLog(Item_strCommandName, "[RX]" + strbytes);
//                        SendToGateWay((int)STATUS.CHECK, strbytes, strGetdata); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return true;
//                    case (int)STATUS.NG:
//                        SaveLog(Item_strCommandName, "[RX]" + strbytes);
//                        SendToGateWay((int)STATUS.NG, strbytes, strGetdata); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return true;
//                    case (int)STATUS.OK:
//                        SaveLog(Item_strCommandName, "[RX]" + strbytes);
//                        SendToGateWay((int)STATUS.OK, strbytes, strGetdata); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                        return true;

//                    case (int)STATUS.RUNNING:
//                    default: break;
//                }


//            }

//            return false;
//        }

//        private void ConnBack(IAsyncResult iar)
//        {
//            try
//            {
//                Socket tmpSock = (Socket)iar.AsyncState;
//                IPEndPoint svrEP = (IPEndPoint)tmpSock.RemoteEndPoint;

//                tmpSock.EndConnect(iar);
//                bConnected = true;
//                sockInterface = tmpSock;
//                EtherNetInfo eNetInfo = new EtherNetInfo();
//                eNetInfo.sockComm = sockInterface;
//                eNetInfo.ClearBuffer();
//                sockInterface.BeginReceive(eNetInfo.Buffer, 0, eNetInfo.Buffer.Length, SocketFlags.None, handleDataRecv, eNetInfo);
//                return;

//            }
//            catch
//            {
//                bConnected = false;
//                if (sockInterface != null)
//                {
//                    sockInterface.Close();
//                    sockInterface.Dispose();
//                }
//            }
//        }

//        public bool IsConnected()
//        {
//            return bConnected;
//        }

//        public string GetConnectedMyIp()
//        {
//            if (IsConnected())
//                return IPAddress.Parse(((IPEndPoint)sockInterface.LocalEndPoint).Address.ToString()).ToString();
//            else
//                return "0.0.0.0";
//        }

//        public string GetLocalIP()
//        {
//            string localIP = "0.0.0.0";

//            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
//            foreach (IPAddress ip in host.AddressList)
//            {
//                if (ip.AddressFamily == AddressFamily.InterNetwork)
//                {
//                    localIP = ip.ToString();
//                    break;
//                }
//            }
//            return localIP;
//        }

//        private void DataUpdate(byte[] bData, bool bClear)
//        {
//            lock (lockobject)
//            {
//                if (bClear)
//                {
//                    OutBuffer.Clear();
//                }
//                else
//                {
//                    OutBuffer.AddRange(bData);
//                }
//            }
//        }

//        private void handleDataRecv(IAsyncResult IAR)
//        {
//            EtherNetInfo tmpNetInfo = (EtherNetInfo)IAR.AsyncState;

//            int iRecvBytes = 0;

//            try
//            {
//                iRecvBytes = tmpNetInfo.sockComm.EndReceive(IAR);
//            }
//            catch
//            {
//                return;
//            }

//            if (iRecvBytes > 0)
//            {
//                if (StateThreadObject(threadCommandChecker)) //RECV 중이면 데이터 업데이트
//                {
//                    //수신바이트 내용 리스트에 업데이트
//                    byte[] tmpBytes = new byte[iRecvBytes];
//                    Array.Copy(tmpNetInfo.Buffer, 0, tmpBytes, 0, tmpBytes.Length);
//                    DataUpdate(tmpBytes, false);
//                }

//                try
//                {
//                    //재수신 모드
//                    tmpNetInfo.ClearBuffer();
//                    tmpNetInfo.sockComm.BeginReceive(tmpNetInfo.Buffer, 0, tmpNetInfo.Buffer.Length, SocketFlags.None, handleDataRecv, tmpNetInfo);

//                }
//                catch
//                {
//                    return;
//                }
//            }
//            else
//            {
//                Disconnect(true);
//            }
//        }

//        private void handleDataSend(IAsyncResult IAR)
//        {
//            if (!IsConnected()) return;

//            EtherNetInfo tmpNetInfo = (EtherNetInfo)IAR.AsyncState;

//            int iRecvBytes = 0;

//            try
//            {
//                iRecvBytes = tmpNetInfo.sockComm.EndSend(IAR);

//            }
//            catch (Exception ex)
//            {
//                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
//                STEPMANAGER_VALUE.DebugView(strExMsg);
//            }
//        }

//        private void sendSocketData(byte[] Sendbytes, int iSendOption, string strCommandName)
//        {
//            if (!IsConnected()) return;

//            Item_strCommandName = strCommandName; //RX 로깅때 쓰임.

//            try
//            {
//                EtherNetInfo tmpNetInfo = new EtherNetInfo();
//                tmpNetInfo.sockComm = sockInterface;
//                tmpNetInfo.sockComm.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSend, tmpNetInfo);

//                SaveLog(strCommandName, "[TX]" + Item_strSendPack);

//                if (iSendOption.Equals((int)ETHERNETCOMMAND.SEND))
//                {
//                    KillThreadObject(threadCommandChecker);
//                    SendToGateWay((int)STATUS.OK, "OK", "OK"); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
//                }
//            }
//            catch (Exception ex)
//            {
//                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
//                STEPMANAGER_VALUE.DebugView(strExMsg);
//            }
//        }

//        private void SendToGateWay(int iStatus, string strFulldata, string strResultMsg)
//        {
//            COMMDATA cdData = new COMMDATA();
//            cdData.iPortNum = (int)DEFINES.SET1;
//            cdData.iStatus = iStatus;
//            cdData.ResponseData = strFulldata;
//            cdData.ResultData = strResultMsg;
//            cdData.SendPacket = Item_strSendPack;
//            CommSendReport(cdData);

//        }

//        private void KillThreadObject(Thread theThread)
//        {
//            try
//            {
//                if (theThread != null)
//                {
//                    if (theThread.IsAlive)
//                    {   //ReadThread 스레드가 그래도 살아있으면 강제종료
//                        theThread.Abort();

//                    }
//                }
//            }
//            catch { }
//        }

//        private bool StateThreadObject(Thread theThread)
//        {
//            if (theThread != null)
//            {
//                return theThread.IsAlive;
//            }
//            return false;
//        }

//        private bool RecvTimeOutCheck(DateTime dtOut, double dTime)
//        {
//            DateTime dtCurr = DateTime.Now;
//            TimeSpan tsN = dtCurr - dtOut;

//            if (tsN.TotalSeconds > dTime)
//            {
//                return false;
//            }
//            return true;
//        }
//    }
//}
