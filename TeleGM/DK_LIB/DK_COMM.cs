using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;



namespace GmTelematics
{

    public struct MsgQueueData
    {
        public int iDevice;           //요청장치번호        
        public int iChannel;          //Channel
        public int iResult;           //수행한 명령의 각 결과        
        public double dTimeout;          //수행할 명령의 각 타임아웃
        public int iSendRecvOption;   //수행할 명령의 각 옵션
        public double dDelaytime;        //수행할 명령의 각 딜레이        
        public string strSendPacket;     //수행할 명령의 패킷
        public string strCommandTBLName;  //TBL 이름
        public string strParam;  
        public AnalyizePack anlPack;
    }
    
    public struct COMMDATA{
        public int      iPortNum;        //COMM NUMBER
        public int      iStatus;         //STATUS CODE
        public string   ResultData;      //RESULT DATA
        public string   ResponseData;    //RESULT DATA
        public string   SendPacket;      //Send DATA
    }
    
    delegate void EventDKCOM(COMMDATA cParam);      //이벤트 날릴 대리자
    delegate void EventSensorDKCOM(bool[] sParam); //보드에서 센서 (버튼, 팔레트 인식) 처리 하는 대리자. 
    delegate void EventRealTimeMsg(int iPort, string cParam);      //이벤트 날릴 대리자
 
    
    class DK_COMM
    {   
#region 변수 선언 및 초기화 부분

        private ThreadStatus TimerStatus;
        private const string strDioVersionCommand     = "24 30 30 56 <CXA> 0D"; // DIO 버젼 확인용
        private const string strDIoInputCommand = "24 30 30 52 <CXA> 0D"; //DIO SENSOR 확인용
        private const string strDIoReadCurrentCommand = "24 30 30 50 32 <CXA> 0D"; //DIO Current 확인용
        private const string strGen10LogStartCommand = "02 FA 00 09 33 49 44 31 66 30 25 58 FA"; //NTF 방지용

        private const int GEN12_SENDBUFFER = 64;

        private System.Diagnostics.Stopwatch swHeartBeatChecker;// = new System.Diagnostics.Stopwatch(); 

        private DK_ANALYZER_GEN9       DKANL_GEN9;     //Gen9 SET 프로토콜 분석 클래스
        private DK_ANALYZER_GEN10      DKANL_GEN10;    //Gen10 SET 프로토콜 분석 클래스
        private DK_ANALYZER_GEN11      DKANL_GEN11;    //Gen11 SET 프로토콜 분석 클래스
        private DK_ANALYZER_GEN11P     DKANL_GEN11P;   //Gen11 SET 출하향 프로토콜 분석 클래스
        private KM_ANALYZER_SET        KMANL_GEN12;    //Gen12 SET 통합 프로토콜 분석 클래스
        private DK_ANALYZER_MCTM       DKANL_MCTM;     //MCTM SET 출하향 프로토콜 분석 클래스
        private DK_ANALYZER_CCM        DKANL_CCM;      //CCM SET 프로토콜 분석 클래스
        private DK_ANALYZER_NAD        DKANL_NAD;      //NAD SET 프로토콜 분석 클래스
        private DK_ANALYZER_TCP        DKANL_TCP;      //TCP SET   프로토콜 분석 클래스
        private DK_ANALYZER_ATT        DKANL_ATT;      //ATT TCP   프로토콜 분석 클래스
        private DK_ANALYZER_DIO_VCP    DKANL_DIO;      //DIO BENCH 프로토콜 분석 클래스
        private DK_ANALYZER_SCANNER    DKANL_SCN;      //SCANNER   프로토콜 분석 클래스
        private DK_ANALYZER_TC3000     DKANL_TC3000;   //TC3000    프로토콜 분석 클래스
        private DK_ANALYZER_DIO_AUDIO  DKANL_AUDIO;    //DIO AUDIO 프로토콜 분석 클래스
        private DK_ANALYZER_DIO_ADC    DKANL_ADC;      //MHT ADC   프로토콜 분석 클래스
        private DK_ANALYZER_ODAPOWER   DKANL_ODA;      //ODA POWER 프로토콜 분석 클래스
        

        public event EventDKCOM       CommSendReport;         //대리자가 날릴 실제 이벤트 메소드
        public event EventSensorDKCOM CommSendReport2;         //대리자가 날릴 실제 이벤트 메소드        
        public event EventRealTimeMsg CommRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
        public  SerialPort ComSerial = null;        //시리얼 포트
        private DK_LOGGER DKLogger;
        private DK_LOGGER DebugLogger;

        //private System.Threading.Timer DelayTimer;   //DELAY 용 타이머 - 기다리고 명령보내고 응답 대기 후 out

        private bool   bDirectMode;
        private Queue<MsgQueueData> ProcessQueue;
        private object lockobjectPQ;
        
        //private bool   bTimeOut;                     //TimeOut Flag
        private double dTimeSet;
        //private bool   bDelayOut;
        private double dDelayTimeSet;

        private Thread ReadThread;                //시리얼 데이터 수신 스레드

        private bool   bResCodeOption;
        private string strSendPack;
        private string strSendPackCompare;
        private string strSendParam;
    
        private StringBuilder sBufferData;
        
        private int    iRecvOption; 

        private string PortName;
        private int    BaudRate;

        private string   TBLname;
        private int      ClassNumber;
        //private DateTime dtOutSet;
        //private DateTime dtCurrTime;
        //private TimeSpan tsNow;
       
              
        private bool bProgramRun;
        private bool bLiveThread;
        private bool bGen10SemiLock;

        private int iSerialType;

        //private byte[] bSample;
        private List<byte> bSample;// = new List<byte>; 
        private List<byte> bHeartBeatSample; // = new List<byte>(); 
        private List<byte> bInputSample;// = new List<byte>; 

        private List<byte> bGen9GPSInfoPackets;// = new List<byte>(); 
        private List<byte> bGen10GPSInfoPackets;// = new List<byte>(); 
        private List<byte> bGen11CCOMGPSInfoPackets;// = new List<byte>(); 
        //private List<byte> bGen12CCOMGPSInfoPackets;// 이게 뭔진 모르지만 일단 만들어보겠음

        private string strLogCommandName;

        private string strLoggingString;
        private byte[] SensorCommandBuffer;
        private byte[] CurrentCommandBuffer;

        private int iCurrentReadCount = 0;
        private int iCurrentNGCount   = 0;
        private COMMDATA cdDataAfter = new COMMDATA();  //DIO 후처리 응답을 담아둘 구조체

        private AnalyizePack Item_AnalPack = new AnalyizePack();

        private bool bBaudrateChagneEnable;

        private byte[] BytesTcpHeartBeatPack;

        private const string strTcpHeartBeatReq  = "02FB010000083252443166FDD8FA";
        private const string strTcpHeartBeatReq2 = "02 FB 01 00 00 08 32 52 44 31 66 FD D8 FA ";

        private System.Diagnostics.Stopwatch swDelayChecker   = new System.Diagnostics.Stopwatch();
        private System.Diagnostics.Stopwatch swTimeOutChecker = new System.Diagnostics.Stopwatch();
        
        private double Item_dTimeSet
        {
            get { return dTimeSet; }
            set { dTimeSet = value; }
        }
        private string Item_strLogCommandName
        {
            get { return strLogCommandName; }
            set { strLogCommandName = value; }
        }
        private string Item_strSendParam
        {
            get { return strSendParam; }
            set { strSendParam = value; }
        }
        private string Item_strSendPack
        {
            get { return strSendPack; }
            set { strSendPack = value; }
        }
        private string Item_strSendPackCompare
        {
            get { return strSendPackCompare; }
            set { strSendPackCompare = value; }
        }
        private int Item_iSerialType
        {
            get { return iSerialType; }
            set { iSerialType = value; }
        }

        private int Item_iRecvOption
        {
            get { return iRecvOption; }
            set { iRecvOption = value; }
        }
        private bool Item_bResultCodeOption
        {
            get { return bResCodeOption; }
            set { bResCodeOption = value; }
        }
        private bool Item_bProgramRun
        {
            get { return bProgramRun; }
            set { bProgramRun = value; }
        }
       
        public int Item_ClassNumber
        {
            get { return ClassNumber; }
            set { ClassNumber = value; }
        }

        /*
        private bool Item_bDelayOut
        {
            get { return bDelayOut; }
            set { bDelayOut = value; }
        }

        private bool Item_bTimeOut
        {
            get { return bTimeOut; }
            set { bTimeOut = value; }
        }
        */

        public string Item_DeviceName
        {
            get { return PortName; }
            set { PortName = value; }
        }

        public string Item_TBLname
        {
            get { return TBLname; }
            set { TBLname = value; }
        }

        public int Item_BaudRate
        {
            get { return BaudRate; }
            set { BaudRate = value; }
        }

        public ThreadStatus GetStatus()
        {
            return TimerStatus;
        }
        private void SetStatus_Recv(int iStatus)
        {
            TimerStatus.iRecving = iStatus;
        }
        private void SetStatus_Clear(int iStatus)
        {
            TimerStatus.iClearing = iStatus;
            try
            {
                if (ComSerial != null)
                    TimerStatus.iBufferCount = ComSerial.BytesToRead;
            }
            catch {}
            
        }
        private void SetStatus_Delay(int iStatus)
        {
            TimerStatus.iDelaying = iStatus;
        }

        private void ManagerDebuging(string strMsg, string strTitle)
        {
            DebugLogger.WriteCommLog(strMsg, strTitle, false);
        }
        public DK_COMM(string strTBLName) {
            //thisLock = new object(); 
            BytesTcpHeartBeatPack = new byte[] { 0x02, 0xFA, 0x00, 0x08, 0x32, 0x52, 0x44, 0x31, 0x66, 0xFD, 0xD8, 0xFA };

            SetStatus_Recv((int)STATUS.READY);
            SetStatus_Clear((int)STATUS.READY);
            SetStatus_Delay((int)STATUS.READY);
            Item_ClassNumber = (int)DEFINES.SET1;
            bSample = new List<byte>();            
            bInputSample = new List<byte>();

            sBufferData = new StringBuilder(4096);
            bSample.Clear();
            
            Item_TBLname = strTBLName;
            
            initialize();
            DKLogger = new DK_LOGGER("SET", false);
            DebugLogger = new DK_LOGGER(Item_TBLname, true);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_COM);

            switch (Item_TBLname) //해당 시리얼 통신의 알맞는 프로토콜 분석 클래스 로딩
            {
                case "ADC":    
                               DKANL_ADC = new DK_ANALYZER_DIO_ADC(); break;
                case "AUDIOSELECTOR": 
                               DKANL_AUDIO = new DK_ANALYZER_DIO_AUDIO(); break;
                case "TC3000": DKANL_TC3000  = new DK_ANALYZER_TC3000(); break;
                case "SET":    DKANL_GEN9    = new DK_ANALYZER_GEN9();
                               DKANL_GEN10   = new DK_ANALYZER_GEN10();
                               DKANL_GEN11   = new DK_ANALYZER_GEN11();
                               DKANL_GEN11P  = new DK_ANALYZER_GEN11P();
                               KMANL_GEN12   = new KM_ANALYZER_SET();
                               DKANL_MCTM    = new DK_ANALYZER_MCTM();    
                               DKANL_TCP     = new DK_ANALYZER_TCP();
                               DKANL_ATT     = new DK_ANALYZER_ATT();
                               bHeartBeatSample = new List<byte>(); 
                               swHeartBeatChecker = new System.Diagnostics.Stopwatch(); 
                               break;
                case "NAD":
                case "CCM":    DKANL_NAD = new DK_ANALYZER_NAD(); 
                               DKANL_CCM = new DK_ANALYZER_CCM(); break;
                case "DIO":    DKANL_DIO     = new DK_ANALYZER_DIO_VCP();
                               strLoggingString = String.Empty;
                               SensorCommandBuffer  = DKANL_DIO.ConvertByteHexString(strDIoInputCommand, false, ref strLoggingString);
                               CurrentCommandBuffer = DKANL_DIO.ConvertByteHexString(strDIoReadCurrentCommand, false, ref strLoggingString);
                               break;
                case "SCANNER": DKANL_SCN = new DK_ANALYZER_SCANNER(); break;
                case "ODAPWR":  DKANL_ODA = new DK_ANALYZER_ODAPOWER(); break;
            }                        
                
        }

        private void GateWay_COM(string cParam) //로깅할때 데이터가 다시 실시간으로 리턴되면 actor로 보내자.
        {
            CommRealTimeTxRxMsg(Item_ClassNumber, cParam);

        }

        private void initialize()
        {
            lockobjectPQ = new object();
            ProcessQueue = new Queue<MsgQueueData>();
            ComSerial       = new SerialPort();
            TimeOutFlag(false);
            //Item_bDelayOut  = false;
            swDelayChecker.Reset();
            swTimeOutChecker.Reset();

            Item_strSendPack = String.Empty;
            Item_iSerialType = (int)RS232.TEXT;
            Item_bProgramRun = true;            
            bGen10SemiLock = false;
            bBaudrateChagneEnable = false;
             
        }
#endregion

#region COM 제어 부분

        public void ChangeBaudRate(int iBaudrate)
        {
            if (ComSerial != null)
            {
                ComSerial.BaudRate = iBaudrate;
                bBaudrateChagneEnable = true;
            }            
        }

        public bool ChangeComPort(string strPortName)
        {
            if (ComSerial != null)
            {
                try
                {
                    if (ComSerial.IsOpen) ComSerial.Close();
                    ComSerial.PortName = strPortName;
                    ComSerial.Parity   = Parity.None;
                    ComSerial.DataBits = 8;
                    ComSerial.StopBits = StopBits.One;
                    ComSerial.Open();
                    return true;
                }
                catch
                {
                    DefaultComPort();            
                }                
            }

            return false;
            
        }

        public void DefaultComPort()
        {
            if (ComSerial != null)
            {
                try
                {
                    if (ComSerial.IsOpen && ComSerial.PortName.Equals(Item_DeviceName))
                        return;
                    if (ComSerial.IsOpen) 
                        ComSerial.Close();

                    ComSerial.PortName = Item_DeviceName;
                    ComSerial.BaudRate = Item_BaudRate;
                    ComSerial.Parity = Parity.None;
                    ComSerial.DataBits = 8;
                    ComSerial.StopBits = StopBits.One;
                    ComSerial.Open();
                }
                catch { }
            }
        }

        public void DefaultBaudrate()
        {
            if (ComSerial != null)
            {
                ComSerial.BaudRate = Item_BaudRate;
                bBaudrateChagneEnable = false;
            }
        }

        public bool ChangeHandShake(int iDx)
        {
            try
            {
                switch (iDx)
                {
                    case 0: ComSerial.Handshake = Handshake.None; break;
                    case 1: ComSerial.Handshake = Handshake.RequestToSend; break;
                    case 2: ComSerial.Handshake = Handshake.RequestToSendXOnXOff; break;
                    case 3: ComSerial.Handshake = Handshake.XOnXOff; break;
                    default: ComSerial.Handshake = Handshake.None; break;
                }
                return true;
                
            }
            catch 
            {
                return false;
            }
        }

        public bool PortOpen(string strComport, int iBaudrate, bool bDio) //DIO 인 경우에만 true 기타 장치는 false. 버젼확인을 위해서.
        {
            Item_DeviceName = strComport;
            Item_BaudRate = iBaudrate;
            bDirectMode = false;
            bBaudrateChagneEnable = false;
            try
            {
                Close();
                ComSerial.PortName = Item_DeviceName;
                ComSerial.BaudRate = Item_BaudRate;
                ComSerial.Parity   = Parity.None;
                ComSerial.DataBits = 8;
                ComSerial.StopBits = StopBits.One;           
                ComSerial.Open();
                if (bDio)
                {
                    if (!CheckDioVersion())
                    {
                        if (ComSerial.IsOpen) ComSerial.Close();
                        return false;
                    }
                }

                Item_bProgramRun = true;

                TimeOutFlag(false);

                if (ReadThread == null)
                {
                    ReadThread = new Thread(SerialScanning);
                }

                if (ReadThread.IsAlive)
                {   //ReadThread 스레드가 살아있으면 최대 5초간 자연사 대기
                    ReadThread.Join(5000);
                }

                if (ReadThread.IsAlive)
                {   //ReadThread 스레드가 그래도 살아있으면 강제종료
                    ReadThread.Abort();
                }

                bLiveThread = true;

                ReadThread = new Thread(SerialScanning);
                ReadThread.Start();
                return true;
            }

            catch (Exception ex)
            {      
                Item_bProgramRun = false;
                bLiveThread = false;
                string strEx = ex.Message;
                DKLogger.WriteCommLog("[PORTOPEN-ERROR]", Item_TBLname, false);
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname  + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                return false;
            }
        }

        public void PortClose()
        {
            if (swHeartBeatChecker != null)
            {
                if (swHeartBeatChecker.IsRunning)
                {
                    swHeartBeatChecker.Stop();
                }
            }
            bLiveThread = false;            
            Thread.Sleep(5);
            Application.DoEvents();
            if (IsPortOpen())
            {            
                Item_bProgramRun = false; 
                TimeOutFlag(false);
                //if (Item_bDelayOut) DelayStop();
                if(swDelayChecker.IsRunning){ swDelayChecker.Stop(); swDelayChecker.Reset();}
               
                Close();
            }

        }

        public bool IsPortOpen()
        {
            return ComSerial.IsOpen;
        }

        private void Close()
        {
            try
            {
                if (swHeartBeatChecker != null)
                {
                    if (swHeartBeatChecker.IsRunning)
                    {
                        swHeartBeatChecker.Stop();
                    }
                }

                if (ReadThread != null && ReadThread.IsAlive)
                {   //ReadThread 스레드가 그래도 살아있으면 강제종료
                    ReadThread.Abort();
                }
            }
            catch  { }

            try
            {
                if (ComSerial.IsOpen) { ComSerial.Close(); }
            }
            catch{}
            
        }

        public bool IsPortRunning()
        {
            //bool bRun = (Item_bTimeOut || Item_bDelayOut); 
            bool bRun = (swTimeOutChecker.IsRunning || swDelayChecker.IsRunning); 
            
            return bRun;
        }

        public bool IsScanning()
        {
            //return Item_bTimeOut;         
            return swTimeOutChecker.IsRunning;
        }

        public void PortRunningStop()
        {
            TimeOutFlag(false);
            DelayStop();           
        }

#endregion

#region COM 메커니즘

        //DIO 만의 특별한 액션 : INPUT 시그널링에 의한 START, STOP 및 UI 의 버튼 OK , CANCEL 처리.
        private void DIO_COMMAND_SENSOR_CHECK(string strCommand, ref string strData)
        {
            int iPos = 0;
            switch (Item_strLogCommandName)
            {
                case "READ_SENSOR_SPARE1":  iPos = 1; break;
                case "READ_SENSOR_SPARE2":  iPos = 2; break;
                case "READ_SENSOR_MUTE":    iPos = 3; break;
                case "READ_SENSOR_M1":      iPos = 4; break;
                case "READ_SENSOR_M2":      iPos = 5; break;
                case "READ_SENSOR_BUB":     iPos = 6; break;
                case "READ_SENSOR_PALETTE": iPos = 7; break;
                case "READ_SENSOR_SET":     iPos = 8; break;
                default: return;
            }
            switch (strCommand)
            {
                case "R": //센서링 START,SPARE1,SPARE2,MUTE,M1,M2,BUB,PALT,SET,STOP                                        
                    if (strData.Length != (int)DIOPIN.PINUSE && strData.Length != (int)DIOPIN.PINUSE + (int)DOUT.END) //올드펌웨어는 10개, 신규펌웨어는 14개임 (relay status 4개)
                        return;

                    if (strData.Substring(iPos, 1).Equals("1")) 
                    {
                        strData = "1";                        
                        return;
                    }
                    else
                    {
                        strData = "0"; 
                        return;
                    }                    

                default: return;
            }
        }

        private void DIO_COMMAND_CURRENT_CHECK(string strCommand, ref string strData)
        {
            int iPos = 0;
            switch (Item_strLogCommandName)
            {
                case "READ_CURRENT_PRIMARY":
                case "READ_CURRENT_VBAT1": 
                case "*READ_CURRENT_VBAT1":  break;
                default: return;
            }
            switch (strCommand)
            {
                case "P": //Current                            
                    try
                    {
                        iPos = strData.IndexOf('.');
                        double dCurr = double.Parse(strData);
                        STEPMANAGER_VALUE.SetCurrent(dCurr, iPos);
                    }
                    catch
                    {
                        STEPMANAGER_VALUE.SetCurrent(0, 0);
                    }
                    break;

                default: return;
            }
        }


        private void DIO_COMMAND_STRING(string strCommand, string strData)
        {
            switch(strCommand)
            {
                case "R" : //센서링 START,SPARE1,SPARE2,MUTE,M1,M2,BUB,PALT,SET,STOP
                            if (strData.Length != (int)DIOPIN.PINUSE && strData.Length != (int)DIOPIN.PINUSE + (int)DOUT.END) return;  //올드펌웨어는 10개, 신규펌웨어는 14개 (relay status)
                            bool[] tmpSenData = new bool[strData.Length];
                            for (int i = 0; i < tmpSenData.Length; i++)
                            {
                                tmpSenData[i] = false;
                                if(strData.Substring(i,1).Equals("1")) tmpSenData[i] = true;
                                
                            }
                            SendSensorToGateWay(tmpSenData); break;

                case "P": //Current                            
                            try
                            {
                                int iPos = strData.IndexOf('.');
                                double dCurr = double.Parse(strData);
                                STEPMANAGER_VALUE.SetCurrent(dCurr, iPos);
                            }
                            catch
                            {
                                STEPMANAGER_VALUE.SetCurrent(0, 0);
                            }
                            break;
                default: return;
            }
        }

        private bool DIO_INPUT_SIGNALING(byte[] tmpBuffer)
        {            
            if (!ComSerial.IsOpen) return false;
            MainFormSleepFuction(10);//20240205
            try
            {          
                int iBufCount = 0;   
                iBufCount = ComSerial.BytesToRead;//버퍼에 데이터가 들어있는지 확인한다.

                if (iBufCount > 0)                    //버퍼에 데이터가 들어있으면 
                {
                    byte[] tmpChar = new byte[iBufCount];
                    ComSerial.Read(tmpChar, 0, iBufCount);
                    bInputSample.AddRange(tmpChar);
               
                    string strGetdata = String.Empty;
                    string strCmddata = String.Empty;
                    byte[] sInBuffer = bInputSample.ToArray();
                    int iRes = (int)STATUS.RUNNING;
                    if (iCurrentReadCount == 0)
                    {
                        iCurrentNGCount++;

                        iRes = DKANL_DIO.AnalyzePacket(sInBuffer, ref strCmddata, ref strGetdata, CurrentCommandBuffer, "READ_INPUT");

                        if (iRes.Equals((int)STATUS.NG))
                        {                            
                            if (iCurrentNGCount > 5)
                                iCurrentReadCount = -999;                           
                                
                        }
                        if (iRes.Equals((int)STATUS.OK))
                        {
                            iCurrentNGCount = 0;                            
                        }
                    }
                    else
                    {
                        iRes = DKANL_DIO.AnalyzePacket(sInBuffer, ref strCmddata, ref strGetdata, tmpBuffer, "READ_INPUT");
                    }
                    

                    switch (iRes)
                    {
                        case (int)STATUS.RUNNING: break;
                        case (int)STATUS.NG:      break;
                        case (int)STATUS.OK:      DIO_COMMAND_STRING(strCmddata, strGetdata); bInputSample.Clear(); break;
                        case (int)STATUS.NONE: 
                        default: break;
                    }
                    
                    return true;
                }
                else
                {
                    //if (Item_bTimeOut) return true;// 절차서가 시작되면 빠져나가야한다.
                    if (swTimeOutChecker.IsRunning) return true;// 절차서가 시작되면 빠져나가야한다.

                    if (ComSerial.BytesToRead < 1)
                    {
                        bInputSample.Clear();
                        if (iCurrentReadCount < 6)   //5회에서 6회로 변경
                        {
                            ComSerial.Write(tmpBuffer, 0, tmpBuffer.Length);
                            if (iCurrentReadCount != -999) iCurrentReadCount++;
                            Thread.Sleep(10);
                        }
                        else
                        {
                            if (iCurrentReadCount != -999)
                            {
                                ComSerial.Write(CurrentCommandBuffer, 0, CurrentCommandBuffer.Length);
                                iCurrentReadCount = 0;
                            }
                        }
                
                    }
                    return true;
                }                
            }
            catch (Exception ex)
            {                
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                ManagerDebuging(ex.Message, "DIO_INPUT_SIGNALING");
                return false;
            }
            
        }

        private bool CheckDioVersion()
        {
            int i = 0;
            for (int iDx = 0; iDx < 3; iDx++)
            {
                i = DirectSendRecv(strDioVersionCommand, 1.5, (int)MODE.SENDRECV, 0, (int)RS232.MOOHANTECH, "DEVICE_CHECK", "");
                if (i == (int)STATUS.OK)
                {
                    return true;
                }
            
            }
            return false;
        }

        private void InsertDataQueue(byte[] tmpBytes, int iFrom)
        {
            StringBuilder strBin = new StringBuilder(4096);
            

            if (Item_TBLname.Equals("SET") && STEPMANAGER_VALUE.bGen10GpsOldInfo) //GEN10의 경우 OLD 모델은 GPS 정보를 REPORT 형태로 캐치해야한다            
            {                
                if (bGen10GPSInfoPackets == null) bGen10GPSInfoPackets = new List<byte>();
                else
                {
                    if (bGen10GPSInfoPackets.Count > 8192) bGen10GPSInfoPackets.Clear();
                }                

                bGen10GPSInfoPackets.AddRange(tmpBytes);

                byte[] sBuffer = bGen10GPSInfoPackets.ToArray();
                string strCmddata = "GEN10_OLD_GPS_INFO";
                string strGetData = String.Empty;
                string tmpString  = String.Empty;
                byte[] tmpBuffer = { 0x02, 0xFA, 0x00, 0x08, 0x31, 0x52, 0x36, 0x30, 0x66, 0x11, 0x12, 0xFA };
                
                int iRtnState = DKANL_GEN10.AnalyzePacket(ref sBuffer, ref strCmddata, ref strGetData, tmpBuffer, strCmddata, ref tmpString, (int)ANALYIZEGEN10.OLDGPSINFO);

                switch (iRtnState)
                {    
                    case (int)STATUS.RUNNING: 
                        break;
                    default: bGen10GPSInfoPackets.Clear(); 
                        break;
                }

            }
            else if (Item_TBLname.Equals("SET") && STEPMANAGER_VALUE.bGen9GpsOldInfo) //GEN9의 경우 OLD 모델은 GPS 정보를 REPORT 형태로 캐치해야한다            
            {
                if (bGen9GPSInfoPackets == null) bGen9GPSInfoPackets = new List<byte>();
                else
                {
                    if (bGen9GPSInfoPackets.Count > 8192) bGen9GPSInfoPackets.Clear();
                }

                bGen9GPSInfoPackets.AddRange(tmpBytes);

                byte[] sBuffer = bGen9GPSInfoPackets.ToArray();
                string strCmddata = "GEN9_OLD_GPS_INFO";
                string strGetData = String.Empty;
                string tmpString = String.Empty;
                byte[] tmpBuffer = { 0x02, 0xFA, 0x00, 0x08, 0x31, 0x52, 0x36, 0x30, 0x66, 0x11, 0x12, 0xFA }; //1R6 찾아보자

                //찾아보기
                /*
                for (int x = 0; x < tmpBytes.Length - 5; x++)
                {
                    if (tmpBytes[x].Equals(0x02) && tmpBytes[x + 1].Equals(0xFB) 
                        && tmpBytes[x+6].Equals(0x31) && tmpBytes[x +7].Equals(0x52) && tmpBytes[x + 8].Equals(0x36) && tmpBytes[x + 10].Equals(0x66))
                    {
                        byte[] gpsBuffer = new byte[bGen9GPSInfoPackets.Count];

                        Array.Copy(tmpBytes, x, gpsBuffer, 0, tmpBytes.Length - (x + 1));
                        break;
                    }
                }
                */
                   
                int iRtnState = DKANL_GEN9.AnalyzePacket(ref sBuffer, ref strCmddata, ref strGetData, tmpBuffer, strCmddata, ref tmpString, (int)ANALYIZEGEN9.OLDGPSINFO, "");

                switch (iRtnState)
                {
                    case (int)STATUS.RUNNING:
                        break;
                    default: 
                        bGen9GPSInfoPackets.Clear();
                        break;
                }

            }   
            else
            {
                if (bGen10GPSInfoPackets != null && bGen10GPSInfoPackets.Count > 0) bGen10GPSInfoPackets.Clear();
                if (bGen9GPSInfoPackets  != null && bGen9GPSInfoPackets.Count  > 0) bGen9GPSInfoPackets.Clear();
            }

            for (int i = 0; i < tmpBytes.Length; i++)
            {
                switch (tmpBytes[i])
                {
                    case 0x0A:
                    case 0xFA: tmpBytes[i] = 0x5E; break;                    
                }
                
            }
            CheckNoneAscii(Encoding.UTF8.GetString(tmpBytes), ref strBin);
            if (strBin.Length > 0)
            {
                BinMsg tmpMsg = new BinMsg();
                tmpMsg.sb = strBin;
                tmpMsg.iFrom = iFrom;
                STEPMANAGER_VALUE.InsertBinMsgQueue(tmpMsg);
            }
        }

        private void InsertDataQueueCCM(byte[] tmpBytes, int iFrom)
        {
            
            StringBuilder strBin = new StringBuilder(4096);

            if (STEPMANAGER_VALUE.bGen11CCMGpsInfo) //GEN11의 경우 CCM 모델은 GPS 정보를 REPORT 형태로 캐치해야한다
            //if (STEPMANAGER_VALUE.bGen11CCMGpsInfo || STEPMANAGER_VALUE.bGen12CCMGpsInfo) //윗줄이 원본 소스          
            {
                if (bGen11CCOMGPSInfoPackets == null) bGen11CCOMGPSInfoPackets = new List<byte>();
                else
                {
                    if (bGen11CCOMGPSInfoPackets.Count > 8096) bGen11CCOMGPSInfoPackets.Clear();
                }

                bGen11CCOMGPSInfoPackets.AddRange(tmpBytes);                
                byte[] sBuffer = bGen11CCOMGPSInfoPackets.ToArray();
                string strCmddata = "GPS_RESPONSE_INFO";
                string strGetData = String.Empty;
                string tmpString = String.Empty;
                byte[] tmpBuffer = { 0x4D, 0x03, 0x0a, 0x01, 0x01 };
                int iLastIndex = 0;
                int iRtnState = DKANL_CCM.AnalyzePacketForReportGnss(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, strCmddata, ref iLastIndex);

                if(iLastIndex > 0) bGen11CCOMGPSInfoPackets.RemoveRange(0, iLastIndex-2);

            }
            else
            {
                if (bGen11CCOMGPSInfoPackets != null && bGen11CCOMGPSInfoPackets.Count > 0) bGen11CCOMGPSInfoPackets.Clear();
            }
        }

        private void MainFormSleepFuction(int iMillisec)
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(iMillisec);
        }

        private void DataClearProcess2()
        {
            if (Item_TBLname.Equals("DIO"))
            {
                Thread.Sleep(80);
                //if (Item_bTimeOut) return;
                if (swTimeOutChecker.IsRunning) return;
                if (!DIO_INPUT_SIGNALING(SensorCommandBuffer)) return; //센서링을 한다.
                
                MsgQueueData mqd = new MsgQueueData();
                if (ExcuteMsgQueueData(ref mqd))
                {
                    if (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun)
                    {                        
                        SendRecv(mqd.strSendPacket, mqd.dTimeout, mqd.iSendRecvOption, mqd.dDelaytime, mqd.iDevice, mqd.strCommandTBLName, mqd.strParam, mqd.anlPack);
                    }
                    else
                    {
                        DeleteMsgQueueData();
                    }
                }

            }
            else if (Item_TBLname.Equals("SET") || Item_TBLname.Equals("CCM") || Item_TBLname.Equals("NAD"))
            {
                Thread.Sleep(1);
                //if (Item_bDelayOut || Item_bTimeOut) return;
                if (swDelayChecker.IsRunning || swTimeOutChecker.IsRunning) return;

                if (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun)
                {
                    int iBinCount = ComSerial.BytesToRead;//버퍼에 데이터가 들어있는지 확인한다.
                    if (iBinCount > 0 /*STEPMANAGER_VALUE.IsExistBinMsgQueue() < 2048*/)
                    {
                        byte[] tmpBytes = new byte[iBinCount];
                        ComSerial.Read(tmpBytes, 0, iBinCount);
                       
                        switch (Item_iSerialType)
                        {
                            case (int)RS232.ATTBYTE:    bHeartBeatSample.AddRange(tmpBytes);  CheckHeartBeatTCP2(3000); break; //3초마다
                            case (int)RS232.GEN11PBYTE: bHeartBeatSample.AddRange(tmpBytes);  CheckHeartBeatTCP2(5000); break; //5초마다.
                            default: break;
                        }

                        if (Item_TBLname.Equals("SET"))
                            InsertDataQueue(tmpBytes, 2);
                        else if (Item_TBLname.Equals("CCM") || Item_TBLname.Equals("NAD"))                       
                            InsertDataQueueCCM(tmpBytes, 2);
                        
                    }
                    else
                    {
                        if (iBinCount > 0) ClearBuffer();
                    }
                }
                else
                {
                    if (STEPMANAGER_VALUE.IsExistBinMsgQueue() > 0)
                    {
                        STEPMANAGER_VALUE.ClearBinMsgQueue();
                    }
                    if (ComSerial.BytesToRead > 0) ComSerial.DiscardInBuffer();
                }
            }
            else if (Item_TBLname.Equals("UART2"))
            {
                if (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun)
                {
                    Thread.Sleep(10);
                    int iByteCount = ComSerial.BytesToRead;//버퍼에 데이터가 들어있는지 확인한다.
                    if (iByteCount > 0)
                    {
                        byte[] tmpBytes = new byte[iByteCount];
                        ComSerial.Read(tmpBytes, 0, iByteCount);                        
                        STEPMANAGER_VALUE.processGPRMC(tmpBytes);
                        return;
                    }
                    
                }
                else
                {
                    if (ComSerial.BytesToRead > 0) ClearBuffer();
                }
            }

            else
            {
                if (ComSerial.BytesToRead > 0) ClearBuffer();
            }           
        }

        private void DataScanProcess2()
        {
            int iRtnState = (int)STATUS.RUNNING;
            bool bTmp = AnalyzeRecvData(ref iRtnState, Item_iRecvOption, false, Item_strSendParam);

            if (!bTmp)
            {   
                if(Item_iRecvOption.Equals((int)MODE.NORESPONSE))
                {
                    SendToGateWay((int)STATUS.OK, sBufferData.ToString(), ""); // ACTOR의 게이트웨이로 결과전송  
                    LoggingData("[TIMEOUT] ", sBufferData.ToString(), false, bSample.ToArray());
                }
                else
                {
                    SendToGateWay((int)STATUS.TIMEOUT, sBufferData.ToString(), ""); // ACTOR의 게이트웨이로 결과전송  
                    LoggingData("[TIMEOUT] ", sBufferData.ToString(), false, bSample.ToArray());

                    if (Item_iSerialType.Equals((int)RS232.GEN10BYTE))
                    {   //GEN10 의 경우 잘되다가도 특정명령에서 오류가 나면  그다음부터 안되는현상이 있으므로 TIME OUT 나면 LOG START 를 날려주자...
                        //NTF 방지...                  
                        string tmpSendString = String.Empty;
                        string strReason = String.Empty;
                        bool brtnOk = false;
                        byte[] tmpBufferGen10 = DKANL_GEN10.ConvertVcpByteHexString(strGen10LogStartCommand, ref tmpSendString, "", ref brtnOk, ref strReason, false);
                        string strOginalCommandName = Item_strLogCommandName;
                        Item_strLogCommandName = "LOG_START";
                        LoggingData("", tmpSendString, true, tmpBufferGen10);
                        Item_strLogCommandName = strOginalCommandName;
                        ComSerial.Write(tmpBufferGen10, 0, tmpBufferGen10.Length);
                        Thread.Sleep(10);
                    }  
                }
                           
            }
                   
        }

        private void CheckHeartBeatTCP(int iTime)
        {
            if (swHeartBeatChecker != null && !swHeartBeatChecker.IsRunning) swHeartBeatChecker.Start();
            try
            {
                StringBuilder sbBytes = new StringBuilder(4096);
                if (bHeartBeatSample.Count > 0) bHeartBeatSample.Clear();

                for (int i = 0; i < bSample.Count; i++)
                {
                    sbBytes.Append(bSample[i].ToString("X2"));
                }

                int iPoint = sbBytes.ToString().IndexOf(strTcpHeartBeatReq);
                if (iPoint < 0)
                {
                    if (swHeartBeatChecker.ElapsedMilliseconds > iTime)
                    {
                        try
                        {
                            ComSerial.Write(BytesTcpHeartBeatPack, 0, BytesTcpHeartBeatPack.Length);
                            STEPMANAGER_VALUE.DebugView("[R]SEND:3Sec HEART BEAT");
                            swHeartBeatChecker.Restart();
                        }
                        catch (System.Exception ex)
                        {
                            STEPMANAGER_VALUE.DebugView("[R]swHeartBeatChecker.Restart() Fail." + ex.Message);
                        }
                    }

                    return;
                }
                int iDx = 0;
                if ((iPoint % 2) == 0)
                {
                    if (iPoint > 0) iDx = iPoint / 2;

                    bSample.RemoveRange(iDx, strTcpHeartBeatReq.Length / 2);
                    sBufferData.Replace(strTcpHeartBeatReq2, "");
                    ComSerial.Write(BytesTcpHeartBeatPack, 0, BytesTcpHeartBeatPack.Length);
                    STEPMANAGER_VALUE.DebugView("[R]SEND:TCP HEART BEAT");
                    swHeartBeatChecker.Restart();
                }
                else
                {
                    return;
                }
            }
            catch (System.Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
            
            
        }

        private void CheckHeartBeatTCP2(int iTime)
        {
            if (swHeartBeatChecker != null && !swHeartBeatChecker.IsRunning) swHeartBeatChecker.Start();

            try
            {
                StringBuilder sbBytes = new StringBuilder(4096);
                for (int i = 0; i < bHeartBeatSample.Count; i++)
                {
                    sbBytes.Append(bHeartBeatSample[i].ToString("X2"));
                }

                int iPoint = sbBytes.ToString().IndexOf(strTcpHeartBeatReq);
                if (iPoint > 0)
                {
                    ComSerial.Write(BytesTcpHeartBeatPack, 0, BytesTcpHeartBeatPack.Length);
                    bHeartBeatSample.Clear();
                    STEPMANAGER_VALUE.DebugView("[C]SEND:TCP HEART BEAT");
                    try
                    {
                        swHeartBeatChecker.Restart();
                    }
                    catch (System.Exception ex)
                    {
                        STEPMANAGER_VALUE.DebugView("[C]swHeartBeatChecker.Restart() Fail." + ex.Message);
                    }

                }
                else
                {
                    if (bHeartBeatSample.Count > 8192) bHeartBeatSample.Clear();
                    if (swHeartBeatChecker.ElapsedMilliseconds > iTime)
                    {
                        try
                        {
                            ComSerial.Write(BytesTcpHeartBeatPack, 0, BytesTcpHeartBeatPack.Length);
                            STEPMANAGER_VALUE.DebugView("[C]SEND:" + iTime.ToString() + "ms HEART BEAT");
                            swHeartBeatChecker.Restart();
                        }
                        catch (System.Exception ex)
                        {
                            STEPMANAGER_VALUE.DebugView("[C]swHeartBeatChecker.Restart() Fail." + ex.Message);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
                        
        }

        private int  DataScanProcess1()
        {
            if (!ComSerial.IsOpen) return (int)STATUS.ERROR;         //컴포트가 열린상태가 아니면 빠져나간다.

            int iRtnState = (int)STATUS.RUNNING;
            int iBufCount = 0;
            try
            {
                iBufCount = ComSerial.BytesToRead;//버퍼에 데이터가 들어있는지 확인한다.
                if (iBufCount > 0)                    //버퍼에 데이터가 들어있으면 
                {
                    byte[] tmpBytes = new byte[iBufCount];
                    ComSerial.Read(tmpBytes, 0, iBufCount);
                    bSample.AddRange(tmpBytes);                    
                    sBufferData.Append(BitConverter.ToString(tmpBytes).Replace("-", " "));
                    
                    if (Item_TBLname.Equals("SET"))
                    {
                        InsertDataQueue(tmpBytes, 1); 
                        switch (Item_iSerialType)
                        {
                            case (int)RS232.ATTBYTE:    CheckHeartBeatTCP(3000); break; //3초마다
                            case (int)RS232.GEN11PBYTE: CheckHeartBeatTCP(5000); break; //5초마다.
                            default: break;
                        }
                         

                    }
                    else if (Item_TBLname.Equals("CCM") || Item_TBLname.Equals("NAD"))
                    {
                        InsertDataQueueCCM(tmpBytes, 1);                        
                    }

                    switch (Item_iRecvOption)
                    {
                        case (int)MODE.UNTIL:
                        case (int)MODE.AVERAGE:
                        case (int)MODE.SENDRECV:
                        case (int)MODE.MULTIPLE:
                            if (AnalyzeRecvData(ref iRtnState, Item_iRecvOption, false, Item_strSendParam))
                            {
                                TimeOutFlag(false);                               
                                return iRtnState;
                            } break;
                        case (int)MODE.RECV:    //SENDRECV, RECV 모드에서는 타임아웃동안 응답이 완료되면 종료한다.
                            Item_strSendPackCompare = Item_strSendPack;
                            if (AnalyzeRecvData(ref iRtnState, Item_iRecvOption, false, Item_strSendParam))
                            {
                                TimeOutFlag(false);                               
                                return iRtnState;
                            } break;
                        case (int)MODE.RECVSEND:    //
                            Item_strSendPackCompare = Item_strSendPack;
                            if (AnalyzeRecvData(ref iRtnState, Item_iRecvOption, false, Item_strSendParam))
                            {
                                TimeOutFlag(false);
                                if (iRtnState == (int)STATUS.OK)
                                {
                                    SendData();
                                    //SendToGateWay((int)STATUS.OK, "", ""); // 현재 명령 진행 상태를 ACTOR의 게이트웨이로 전송 ㅋㅋ
                                }                                
                                return iRtnState;
                            } break;

                        case (int)MODE.BUFFER: break; //버퍼는 타임아웃까지 쭈욱!

                        case (int)MODE.NORESPONSE: //응답이 없어야(TIME-OUT) PASS 인 옵션,  별걸 다만든다.

                            if (AnalyzeRecvData(ref iRtnState, Item_iRecvOption, false, Item_strSendParam))
                            {
                                TimeOutFlag(false);
                                return (int)STATUS.NG;
                            } break;

                        default: if (AnalyzeRecvData(ref iRtnState, Item_iRecvOption, false, Item_strSendParam))
                            {
                                TimeOutFlag(false);                                
                                return iRtnState;
                            } break;
                    }
                }
                 
            }
            catch (Exception e)
            {
                ManagerDebuging(e.Message, "DataScanProcess1");
            }
           
            return iRtnState;
        }

        private int  Gen10MultiSendRecvProcess(string originSendPackData, string strLogName, ref string strRtnData, ref string strFullData) //멀티커맨드용 데이터 분석. (GEN10의 경우 파일에서 읽어다가 즉석 판정후 여러번 보내야할경우.
        {
            if (!ComSerial.IsOpen) return (int)STATUS.CHECK; ;         //컴포트가 열린상태가 아니면 빠져나간다.

            int iRtnState = (int)STATUS.RUNNING;
            int iBufCount = 0;
            try
            {
                iBufCount = ComSerial.BytesToRead;//버퍼에 데이터가 들어있는지 확인한다.
                if (iBufCount > 0)                    //버퍼에 데이터가 들어있으면 
                {
                    byte[] tmpBytes = new byte[iBufCount];
                    ComSerial.Read(tmpBytes, 0, iBufCount);
                    bSample.AddRange(tmpBytes);
                    sBufferData.Append(BitConverter.ToString(tmpBytes).Replace("-", " "));
                    InsertDataQueue(tmpBytes, 3);

                    byte[] sBuffer = new byte[bSample.Count];
                    string strCmddata = String.Empty;          
                    
                    bSample.CopyTo(sBuffer);

                    //iRtnState = DKANL_GEN10.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, originSendPackData, strLogName);
                    string strCompareA = String.Empty;
                    string strCompareB = String.Empty;
                    string tmpBuffers = System.Text.Encoding.UTF8.GetString(sBuffer);

                    if (strLogName.Equals("READ_WL_COUNTERS"))
                    {
                        strCompareA = "pktengrxdmcast";
                        strCompareB = "txmpdu_sgi";
                    }

                    iRtnState = DKANL_GEN10.AnalyzeWLCommandPacket(tmpBuffers, ref strRtnData, ref strFullData, originSendPackData, strCompareA, strCompareB);
                    if (iRtnState == (int)STATUS.OK)
                    {
                        string tmpString = BitConverter.ToString(sBuffer).Replace("-", " ");
                        LoggingData("", tmpString, false, sBuffer);
                    }
                }
            }
            catch 
            {
                return iRtnState;
            }            
            return iRtnState;
        }
          
        private void SerialScanning()
        {
            int iStatus = (int)STATUS.NONE;

            while (bLiveThread) //타임 아웃 시간 내에서 시리얼 버퍼 스캔한다.
            {
                if (!bDirectMode)
                {
                    try
                    {
                        //if (Item_bDelayOut)
                        if (swDelayChecker.IsRunning)
                        {                           
                            SetStatus_Delay((int)STATUS.RUNNING);
                            SetStatus_Recv((int)STATUS.READY);
                            SetStatus_Clear((int)STATUS.READY);                   
                            DelayTimeOutCheck();
                            //Thread.Sleep(1);
                        }
                        else
                        {
                            //if (Item_bTimeOut)
                            if (swTimeOutChecker.IsRunning)
                            {
                                SetStatus_Delay((int)STATUS.READY);
                                SetStatus_Recv((int)STATUS.RUNNING);
                                SetStatus_Clear((int)STATUS.READY);
                                iStatus = DataScanProcess1();

                                if (iStatus == (int)STATUS.ERROR)
                                {
                                    TimeOutFlag(false);
                                    DataScanProcess2();// 타임아웃 데이터 처리.
                                    bLiveThread = false;
                                }

                                if (iStatus == (int)STATUS.RUNNING)
                                {
                                    RecvTimeOutCheck();
                                }

                            }
                            else
                            {
                                SetStatus_Delay((int)STATUS.READY);
                                SetStatus_Recv((int)STATUS.READY);
                                SetStatus_Clear((int)STATUS.RUNNING);
                                if (!bGen10SemiLock) DataClearProcess2();
                                //Thread.Sleep(1);
                            }
                        }
                   
                    }
                    catch (System.Exception ex)
                    {
                        
                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname + ":" + ex.Message;
                        STEPMANAGER_VALUE.DebugView(strExMsg);
                        if (!ComSerial.IsOpen) break;
                    }   
                   
                }
               
                Thread.Sleep(2);
            }
            SetStatus_Delay((int)STATUS.READY);
            SetStatus_Recv((int)STATUS.READY);
            SetStatus_Clear((int)STATUS.READY);
        }

        private void SendToGateWay(int iStatus, string strFulldata, string strResultMsg)
        {
            COMMDATA cdData     = new COMMDATA();
            cdData.iPortNum     = Item_ClassNumber;
            cdData.iStatus      = iStatus;
            cdData.ResponseData = strFulldata;  
            cdData.ResultData   = strResultMsg;
            cdData.SendPacket   = Item_strSendPack;

            if (!bDirectMode)
            {
                CommSendReport(cdData);
            }         
            
        }

        private void SendSensorToGateWay(bool[] sData)
        {
            CommSendReport2(sData);
        }

        public void InsertDioCommand(string strPack, double dTimeout, int iSendRecvOption, double dDelaySec, int iSerialType, string strCommandTBLName, string strPram, AnalyizePack anlPack)
        {
            MsgQueueData mqd = new MsgQueueData();
            mqd.dDelaytime = dDelaySec;
            mqd.dTimeout = dTimeout;
            mqd.iChannel = (int)DEFINES.SET1;
            mqd.iDevice = iSerialType;
            mqd.iSendRecvOption = iSendRecvOption;
            mqd.strSendPacket = strPack;
            mqd.strCommandTBLName = strCommandTBLName;
            mqd.anlPack = new AnalyizePack();
            mqd.anlPack = anlPack;
            InsertMsgQueueData(mqd);

        }

        public void SendRecv(string strPack, double dTimeout, int iSendRecvOption, double dDelaySec, int iSerialType, string strCommandTBLName, string strPram, AnalyizePack anlPack)
        {
            Item_strLogCommandName = strCommandTBLName;
            Item_strSendParam = strPram;
            Item_iRecvOption = iSendRecvOption;
            Item_strSendPack = strPack;
            Item_iSerialType = iSerialType;
            Item_bResultCodeOption = anlPack.bResultCodeOption;
            if (dTimeout > 0.5) Item_dTimeSet = dTimeout;
            else Item_dTimeSet = 0.5;
            
            Item_AnalPack = anlPack;

            if (!IsPortOpen())
            {
                TimeOutFlag(false);
                LoggingData("[PORTOPEN]ERROR", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                SendToGateWay((int)STATUS.NG, Item_strSendParam, "[PORTOPEN]ERROR"); // ACTOR의 게이트웨이로 결과전송  
                return;
            }  

            switch (iSendRecvOption)
            {
                //리트라이 수만큼 보내고 받고 값 평균 
                case (int)MODE.AVERAGE:
                case (int)MODE.MULTIPLE: 
                                         DelayStart(dDelaySec, iSendRecvOption); return;                                        

                //보내고 받기
                case (int)MODE.SENDRECV:
                case (int)MODE.NORESPONSE: 
                                        DelayStart(dDelaySec, iSendRecvOption); return;                                                                                                                          

                //보내기만
                case (int)MODE.SEND:    DelayStart(dDelaySec, iSendRecvOption); return;                                         

                //받기만
                case (int)MODE.RECV:     DelayStart(dDelaySec, iSendRecvOption); return;

                //받고 보내기
                case (int)MODE.RECVSEND:  DelayStart(dDelaySec, iSendRecvOption); return;

                //보내고 받되 TIMEOUT 끝날때까지 계속적으로 수신한다.
                case (int)MODE.BUFFER:   DelayStart(dDelaySec, iSendRecvOption); return;
                                                            
                //보내고 받되 TIMEOUT 끝날때까지 조건(spec)이 만족하면 끝난다.
                case (int)MODE.UNTIL:   DelayStart(dDelaySec, iSendRecvOption); return;
                                                         
                
                default: return;
            }

        }
        
        public  int DirectSendRecv(string strPack, double dTimeout, int iSendRecvOption, double dDelaySec, int iSerialType, string strCommandTBLName, string strPram)
        {
            Item_strLogCommandName = strCommandTBLName;
            Item_strSendParam = strPram;
            Item_iRecvOption = iSendRecvOption;
            Item_strSendPack = strPack;
            Item_iSerialType = iSerialType;
            if (dTimeout > 0.5) Item_dTimeSet = dTimeout;
            else Item_dTimeSet = 0.5;

            sBufferData.Clear();
            bSample.Clear();

            //여기서 응답처리 다해야할듯...            
            cdDataAfter.iPortNum = Item_ClassNumber;
            cdDataAfter.iStatus = (int)STATUS.RUNNING;
            cdDataAfter.ResponseData = "";
            cdDataAfter.ResultData = "";
            cdDataAfter.SendPacket = strCommandTBLName;

            bDirectMode = true;

            if (iSendRecvOption == (int)MODE.SEND)
            {
                
                TimeOutFlag(true);
                SendData();
                TimeOutFlag(false);                
                sBufferData.Clear();
                bSample.Clear();
                bDirectMode = false;
                return (int)STATUS.OK;
            }

            if (dDelaySec < 1 || STEPMANAGER_VALUE.bInteractiveMode)
            {
                Thread.Sleep(50);
            }
            else
            {
                if (dDelaySec > 10) dDelaySec = 10;
                swDelayChecker.Reset();
                swDelayChecker.Start();
                while (swDelayChecker.IsRunning)
                {
                    if (swDelayChecker.Elapsed.TotalSeconds >= dDelaySec)
                    {
                        swDelayChecker.Stop();
                        break;
                    }              
                    Thread.Sleep(10);
                }                
            }
            
            SendData();
            DateTime dtm = DateTime.Now;            
            while (true)
            {            
                int iStatus = DataScanProcess1();

                switch(iStatus)
                {

                    case (int)STATUS.OK: bDirectMode = false; return (int)STATUS.OK;
                    case (int)STATUS.NG: bDirectMode = false; return (int)STATUS.NG;
                    default: break;

                }
                

                if (!DirectRecvTimeOutCheck(dtm, dTimeout))
                {
                    bDirectMode = false;

                    if (Item_iRecvOption.Equals((int)MODE.NORESPONSE))
                        return (int)STATUS.OK;
                    else
                        return (int)STATUS.TIMEOUT;
                }

                Thread.Sleep(10);   
                      
            }
          
           
        }       

        public void ClearBuffer()
        {
            if (ComSerial != null && ComSerial.IsOpen)
            {
                if (!Item_TBLname.Equals("SET"))
                {
                    ComSerial.DiscardInBuffer();
                }
                ComSerial.DiscardOutBuffer();
            }          
        }

        private void SendData()
        {
            if (Item_TBLname.Equals("DIO")) Thread.Sleep(20);  //DIO 경우 판넬미터도 실시간으로 읽기 때문에 시간을 더 지연하자.
            
            Thread.Sleep(20);
            ClearBuffer();

            if (!bBaudrateChagneEnable)
            {
                switch (iSerialType)
                {//Item_TBLname
                    case (int)RS232.NADBYTE: Item_TBLname = "NAD"; break; //CCM 과 같은 시리얼 포트를 쓰기때문에.
                    case (int)RS232.CCMBYTE: Item_TBLname = "CCM"; break; //CCM 과 같은 시리얼 포트를 쓰기때문에.
                    
                    case (int)RS232.TCPBYTE:    ComSerial.BaudRate = 38400; break;
                    case (int)RS232.GEN11PBYTE: ComSerial.BaudRate = 921600; break;
                    case (int)RS232.GEN11BYTE:  ComSerial.BaudRate = 38400; break;
                    case (int)RS232.GEN12BYTE:  ComSerial.BaudRate = 115200; break; //20220517 변경
                    case (int)RS232.ATTBYTE:    ComSerial.BaudRate = 115200; break;
                    case (int)RS232.GEN10BYTE:  ComSerial.BaudRate = 115200; break;
                    case (int)RS232.GEN9BYTE:   ComSerial.BaudRate = 115200; break;
                    case (int)RS232.MCTMBYTE:   ComSerial.BaudRate = 19200; break;
                    default: break;
                }   
            }

            sBufferData.Clear();            
            bSample.Clear();
            bInputSample.Clear();
            Item_strSendPackCompare = Item_strSendPack;
            string strReason = String.Empty;
            
            try
            {
                switch (Item_iSerialType)
                {

                    case (int)RS232.TEXT:
                        Item_strSendPack = Item_strSendPack.Replace("<DATA>", Item_strSendParam);
                        LoggingData("", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                        ComSerial.Write(Item_strSendPack);
                        break;
                        
                    case (int)RS232.SCANNER:
                        string tmpLogStringsc = Item_strSendPack;
                        byte[] tmpBuffersc = DKANL_SCN.ConvertByteHexString(Item_strSendPack, true, ref tmpLogStringsc);
                        LoggingData("", tmpLogStringsc, true, tmpBuffersc);
                        Item_strSendPack = tmpLogStringsc;
                        ComSerial.Write(tmpBuffersc, 0, tmpBuffersc.Length);
                        break;

                    case (int)RS232.MOOHANTECH:
                        string tmpLogString = Item_strSendPack;
                        byte[] tmpBuffer = new byte[1];
                            
                        switch (Item_TBLname) //해당 시리얼 통신의 알맞는 프로토콜 분석 클래스 로딩
                        {
                            case "ADC":    
                                tmpBuffer = DKANL_ADC.ConvertByteHexString(Item_strSendPack, true, ref tmpLogString); break;
                            case "AUDIOSELECTOR": 
                                tmpBuffer = DKANL_AUDIO.ConvertByteHexString(Item_strSendPack, true, ref tmpLogString); break;
                            default:
                                tmpBuffer = DKANL_DIO.ConvertByteHexString(Item_strSendPack, true, ref tmpLogString); break;
                        }
                        LoggingData("", tmpLogString, true, tmpBuffer);
                        Item_strSendPack = tmpLogString;
                        ComSerial.Write(tmpBuffer, 0, tmpBuffer.Length);
                        break;
                        
                    case (int)RS232.GEN10BYTE:
                        string tmpSendString = Item_strSendPack;
                        bool brtnOk = true;
                        List<string> FileCommandList = new List<string>();

                        if (DKANL_GEN10.IsFileCommand(Item_strSendPack, Item_strLogCommandName, ref FileCommandList)) //2개이상의 명령처리의 경우
                        {
                            bGen10SemiLock = true;
                            Thread.Sleep(10);
                            Application.DoEvents();
                            List<byte[]> tmpBuffersGen10 = new List<byte[]>();
                            string strtmpSenPack = Item_strSendPack.Replace("<FILE>", "<DATA:CHAR>");
                            
                            for (int i = 0; i < FileCommandList.Count; i++)
                            {
                                byte[] tmpBufferCommand = DKANL_GEN10.ConvertVcpByteHexString(strtmpSenPack, ref tmpSendString, FileCommandList[i], ref brtnOk, ref strReason, false);
                                if (brtnOk)
                                {
                                    tmpBuffersGen10.Add(tmpBufferCommand);
                                }                                
                            }

                            bool bCommandOK = false;
                            string strGetData = String.Empty;
                            string strFullData = String.Empty;
                            for (int i = 0; i < tmpBuffersGen10.Count; i++)
                            {
                                LoggingData("", tmpSendString, true, tmpBuffersGen10[i]);
                                Item_strSendPack = tmpSendString;
                                TimeOutFlag(false);
                                Thread.Sleep(10);
                                
                                ClearBuffer();

                                sBufferData.Clear();                                
                                bSample.Clear();
                                bInputSample.Clear();
                                
                                Thread.Sleep(10);

                                Item_strSendPackCompare = Item_strSendPack;
                                ComSerial.Write(tmpBuffersGen10[i], 0, tmpBuffersGen10[i].Length);
                              
                                
                                DateTime dtNowTime = DateTime.Now;
                                bCommandOK = false;
                                
                                while (SemiRecvTimeOutCheck(dtNowTime, Item_dTimeSet))
                                {
                                    if (Gen10MultiSendRecvProcess(FileCommandList[i], Item_strLogCommandName, ref strGetData, ref strFullData) == (int)STATUS.OK)
                                    {
                                        bCommandOK = true;
                                        break;
                                    }                                 
                                    Thread.Sleep(10);                                    
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                }
                               
                                if (!bCommandOK)
                                {                                    
                                    SendToGateWay((int)STATUS.TIMEOUT, "TIMEOUT", "TIMEOUT"); // ACTOR의 게이트웨이로 결과전송  
                                    LoggingData("[TIMEOUT] ", "", false, bSample.ToArray());
                                    bGen10SemiLock = false;
                                    return;
                                }
                              
                            }
                            
                            if (bCommandOK)
                            {
                                SendToGateWay((int)STATUS.OK, strFullData, strGetData); // 현재 명령 진행 상태를 ACTOR의 게이트웨이로 전송 ㅋㅋ
                            }
                            bGen10SemiLock = false;

                        }
                        else 
                        {
                            //일반적인 one & one 명령의 경우
                            bGen10SemiLock = false;
                            byte[] tmpBufferGen10 = DKANL_GEN10.ConvertVcpByteHexString(Item_strSendPack, ref tmpSendString, Item_strSendParam, ref brtnOk, ref strReason, true);
                            
                            if (brtnOk)
                            {
                                LoggingData("", tmpSendString, true, tmpBufferGen10);
                                Item_strSendPack = tmpSendString;
                                ComSerial.Write(tmpBufferGen10, 0, tmpBufferGen10.Length);
                            }
                            else
                            {
                                TimeOutFlag(false);
                                LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                                SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason + "(" + Item_strSendParam + ")"); // ACTOR의 게이트웨이로 결과전송  
                            }                           
                        }

                        bGen10SemiLock = false;
                        break;

                    case (int)RS232.GEN9BYTE:
                        string tmpSendString9 = Item_strSendPack;
                        bool brtn9Ok = true;

                        byte[] tmpBufferGen9 = DKANL_GEN9.ConvertVcpByteHexString(Item_strSendPack, ref tmpSendString9, Item_strSendParam, ref brtn9Ok, ref strReason, true);
                        if (brtn9Ok)
                        {
                            if (tmpBufferGen9[0].Equals(0x7E))
                                //FACTORY 명령
                                DKANL_GEN9.AnalyzeMode = (int)CLSMODE.FACTORY;
                            else
                            {
                                if (Item_strLogCommandName.Contains("[H]"))
                                {
                                    DKANL_GEN9.AnalyzeMode = (int)CLSMODE.GEN9HIGH; //GEN9HIGH 명령임. 거의 GEN10 초기 프로토콜 임.
                                }
                                else if (Item_strLogCommandName.Contains("[F]"))//이건 경민 추가된 소스문
                                {
                                    DKANL_GEN9.AnalyzeMode = (int)CLSMODE.FCP;
                                }
                                else
                                {
                                    DKANL_GEN9.AnalyzeMode = (int)CLSMODE.NORMAL;   //NORMAL 명령 GEN9 전성기 시절 프로토콜 타입.
                                }
                                
                                //아래 3줄은 기존 원본 소스
                                //bool bChkHighProtocol = Item_strLogCommandName.IndexOf("[H]").Equals(0);

                                //if (bChkHighProtocol)
                                //    DKANL_GEN9.AnalyzeMode = (int)CLSMODE.GEN9HIGH; //GEN9HIGH 명령임. 거의 GEN10 초기 프로토콜 임.
                                //else
                                //    DKANL_GEN9.AnalyzeMode = (int)CLSMODE.NORMAL;   //NORMAL 명령 GEN9 전성기 시절 프로토콜 타입.                                
                            }
                            
                            LoggingData("", tmpSendString9, true, tmpBufferGen9);
                            Item_strSendPack = tmpSendString9;
                            ComSerial.Write(tmpBufferGen9, 0, tmpBufferGen9.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason + "(" + Item_strSendParam + ")"); // ACTOR의 게이트웨이로 결과전송  
                        }
                        
                        break;

                    case (int)RS232.TCPBYTE:
                        string tmpSendTString = Item_strSendPack;
                        bool bTrtnOk = true;
                        byte[] tmpBufferTCP = DKANL_TCP.ConvertTcpByteHexString(Item_strSendPack, ref tmpSendTString, Item_strSendParam, ref bTrtnOk, ref strReason);
                                                
                        if (bTrtnOk)
                        {
                            LoggingData("", tmpSendTString, true, tmpBufferTCP);
                            Item_strSendPack = tmpSendTString;
                            ComSerial.Write(tmpBufferTCP, 0, tmpBufferTCP.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason +"(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  
                           
                        }
                        break;

                    case (int)RS232.GEN11BYTE:
                        string tmpSendG11String = Item_strSendPack;
                        bool bG11rtnOk = true;
                        byte[] tmpBufferGEN11 = DKANL_GEN11.ConvertGen11ByteHexString(Item_strSendPack, ref tmpSendG11String, Item_strSendParam, ref bG11rtnOk, ref strReason, true);

                        if (bG11rtnOk)
                        {
                            LoggingData("", tmpSendG11String, true, tmpBufferGEN11);
                            Item_strSendPack = tmpSendG11String;
                            ComSerial.Write(tmpBufferGEN11, 0, tmpBufferGEN11.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  

                        }
                        break;

                    case (int)RS232.GEN12BYTE:
                        string tmpSendWaveString = Item_strSendPack;
                        bool bG12rtnOk = true;
                        byte[] tmpBufferGen12 = KMANL_GEN12.ConvertWaveByteHexString(Item_strSendPack, ref tmpSendWaveString, Item_strSendParam, ref bG12rtnOk, ref strReason, true);

                        if (bG12rtnOk)
                        {
                            LoggingData("", tmpSendWaveString, true, tmpBufferGen12);
                            Item_strSendPack = tmpSendWaveString;
                            //ComSerial.Write(tmpBufferGen12, 0, tmpBufferGen12.Length);

                            //byte[] bReturnBytes = new byte[GEN12_SENDBUFFER];
                            //20220513 GEN12 는 64바이트씩 전송해야 됨.
                            if (tmpBufferGen12.Length > GEN12_SENDBUFFER)
                            {
                                byte[] bData1 = new byte[GEN12_SENDBUFFER];
                                for (int i = 0; i < tmpBufferGen12.Length; i = i+ GEN12_SENDBUFFER)
                                {
                                    if(tmpBufferGen12.Length - i > 64)
                                        bData1 = new byte[GEN12_SENDBUFFER];
                                    else
                                        bData1 = new byte[tmpBufferGen12.Length - i];
                                    Array.Copy(tmpBufferGen12, i, bData1, 0, bData1.Length);
                                    
                                    ComSerial.Write(bData1, 0, bData1.Length);
                                    System.Threading.Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                ComSerial.Write(tmpBufferGen12, 0, tmpBufferGen12.Length);
                            }
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송

                        }
                        break;

                    case (int)RS232.MCTMBYTE:
                        string tmpSendMctmString = Item_strSendPack;
                        bool bMctmrtnOk = true;
                        byte[] tmpBufferMCTM = DKANL_MCTM.ConvertMCTMByteHexString(Item_strSendPack, ref tmpSendMctmString, Item_strSendParam, ref bMctmrtnOk, ref strReason);

                        if (bMctmrtnOk)
                        {
                            LoggingData("", tmpSendMctmString, true, tmpBufferMCTM);
                            Item_strSendPack = tmpSendMctmString;
                            ComSerial.Write(tmpBufferMCTM, 0, tmpBufferMCTM.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  

                        }
                        break;

                    case (int)RS232.GEN11PBYTE:
                        string tmpSendG11PString = Item_strSendPack;
                        bool bG11PrtnOk = true;
                        byte[] tmpBufferGEN11P = DKANL_GEN11P.ConvertGen11ByteHexString(Item_strSendPack, ref tmpSendG11PString, Item_strSendParam, ref bG11PrtnOk, ref strReason, true);

                        if (bG11PrtnOk)
                        {
                            LoggingData("", tmpSendG11PString, true, tmpBufferGEN11P);
                            Item_strSendPack = tmpSendG11PString;
                            ComSerial.Write(tmpBufferGEN11P, 0, tmpBufferGEN11P.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  

                        }
                        break;

                    case (int)RS232.CCMBYTE:
                        string tmpSendCString = Item_strSendPack;
                        bool bCrtnOk = true;
                        byte[] tmpBufferCCM = DKANL_CCM.ConvertCcmByteHexString(Item_strSendPack, ref tmpSendCString, Item_strSendParam, ref bCrtnOk, ref strReason, Item_strLogCommandName);

                        if (bCrtnOk)
                        {
                            LoggingData("", tmpSendCString, true, tmpBufferCCM);
                            Item_strSendPack = tmpSendCString;
                            ComSerial.Write(tmpBufferCCM, 0, tmpBufferCCM.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  

                        }
                        break;

                    case (int)RS232.NADBYTE:
                        string tmpSendNString = Item_strSendPack;
                        bool bNrtnOk = true;
                        byte[] tmpBufferNAD = DKANL_NAD.ConvertNadByteHexString(Item_strSendPack, ref tmpSendNString, Item_strSendParam, ref bNrtnOk, ref strReason, Item_strLogCommandName);

                        if (bNrtnOk)
                        {
                            LoggingData("", tmpSendNString, true, tmpBufferNAD);
                            Item_strSendPack = tmpSendNString;
                            ComSerial.Write(tmpBufferNAD, 0, tmpBufferNAD.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  

                        }
                        break;

                    case (int)RS232.ATTBYTE:
                        string tmpSendAString = Item_strSendPack;
                        bool bArtnOk = true;

                        byte[] tmpBufferATT = DKANL_ATT.ConvertAttByteHexString(Item_strSendPack, ref tmpSendAString, Item_strSendParam, ref bArtnOk, ref strReason);
                                                
                        if (bArtnOk)
                        {
                            LoggingData("", tmpSendAString, true, tmpBufferATT);
                            Item_strSendPack = tmpSendAString;
                            ComSerial.Write(tmpBufferATT, 0, tmpBufferATT.Length);
                        }
                        else
                        {
                            TimeOutFlag(false);
                            LoggingData("[" + strReason + "(" + Item_strSendParam + ")]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString()));
                            SendToGateWay((int)STATUS.NG, Item_strSendParam, strReason); // ACTOR의 게이트웨이로 결과전송  

                        }
                        break;

                    default:
                        LoggingData("[COMMANDFAIL]", Item_strSendPack, true, Encoding.UTF8.GetBytes(sBufferData.ToString())); 
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LoggingData("[COMMANDFAIL]", Item_strSendPack + "->" + ex.ToString(), true, Encoding.UTF8.GetBytes(sBufferData.ToString()));                
            }
                        
        }
        
        private void SendDataOlny()
        {
            SendData(); 
            SendToGateWay((int)STATUS.OK, "", ""); // ACTOR의 게이트웨이로 명령전송 완료 알려줌.
        }

        private void LogPrint()
        {
            string strLogPrint = String.Empty;

            switch (Item_strLogCommandName)
            {
                case "GET_SERVICE_INFORMATION":

                    switch (Item_iSerialType)
                    {
                        case (int)RS232.GEN9BYTE:
                        case (int)RS232.GEN10BYTE:
                        case (int)RS232.ATTBYTE:
                        case (int)RS232.TCPBYTE:
                            for (int i = 0; i < (int)ServiceIndexA.END; i++)
                            {
                                strLogPrint = "[REPORT]" + (i + ServiceIndexA.szMEID).ToString() + " : " + STEPMANAGER_VALUE.OOBServiceInfoA[i];
                                DKLogger.WriteCommLog(strLogPrint, Item_TBLname + ":" + Item_strLogCommandName, false);
                            }
                            break;

                        case (int)RS232.GEN11BYTE:
                        case (int)RS232.GEN11PBYTE:
                        case (int)RS232.GEN12BYTE:
                            for (int i = 0; i < (int)ServiceIndexB.END; i++)
                            {
                                strLogPrint = "[REPORT]" + (i + ServiceIndexB.szMEID).ToString() + " : " + STEPMANAGER_VALUE.OOBServiceInfoB[i];
                                DKLogger.WriteCommLog(strLogPrint, Item_TBLname + ":" + Item_strLogCommandName, false);
                            }
                            break;
                        default: return;
                    }
                    return;
                default: break;
            }            
        }
 
        public bool AnalyzeRecvData(ref int iRtnState, int iRecvOpt, bool bMoreCommand, string strParam) //bMoreCommand 가 true 이면 2개이상의 명령이므로 판정결과를 보내선 안된다.
        {
            iRtnState = (int)STATUS.RUNNING;
            string strGetData = String.Empty;
            string strCmddata = String.Empty;
            byte[] tmpBuffer;
            byte[] sBuffer = new byte[bSample.Count];
            bSample.CopyTo(sBuffer);
            string tmpString = String.Empty;
            bool brtnOk = true;

            try
            {
                switch (Item_TBLname) //해당 시리얼 통신의 알맞는 프로토콜 분석 클래스 로딩
                {
                    case "ODAPWR":
                        iRtnState = DKANL_ODA.AnalyzePacket(sBuffer, ref strGetData);
                        break;
                    case "SCANNER":
                        tmpBuffer = DKANL_SCN.ConvertByteHexString(Item_strSendPackCompare, false, ref tmpString);
                        iRtnState = DKANL_SCN.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer);
                        break;

                    case "CCM":
                    case "NAD":
                    case "SET":
                        if (Item_strSendPackCompare.Length < 1) return false;

                        tmpString = string.Empty;  //BitConverter.ToString(sBuffer).Replace("-", " ");
                        string strReason = String.Empty;

                        switch (Item_iSerialType)
                        {
                            case (int)RS232.CCMBYTE:
                                tmpBuffer = DKANL_CCM.ConvertCcmByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, Item_strLogCommandName);
                                break;
                            case (int)RS232.NADBYTE:
                                tmpBuffer = DKANL_NAD.ConvertNadByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, Item_strLogCommandName);
                                break;
                            case (int)RS232.GEN9BYTE:
                                tmpBuffer = DKANL_GEN9.ConvertVcpByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, false);
                                break;
                            case (int)RS232.GEN10BYTE:
                                tmpBuffer = DKANL_GEN10.ConvertVcpByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, false);
                                break;
                            case (int)RS232.TCPBYTE:
                                tmpBuffer = DKANL_TCP.ConvertTcpByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason);
                                break;
                            case (int)RS232.GEN11BYTE:
                                tmpBuffer = DKANL_GEN11.ConvertGen11ByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, false);
                                break;
                            case (int)RS232.GEN11PBYTE:
                                tmpBuffer = DKANL_GEN11P.ConvertGen11ByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, false);
                                break;
                            case (int)RS232.GEN12BYTE:
                                tmpBuffer = KMANL_GEN12.ConvertWaveByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason, false);
                                break;
                            case (int)RS232.ATTBYTE:
                                tmpBuffer = DKANL_ATT.ConvertAttByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason);
                                break;
                            case (int)RS232.MCTMBYTE:
                                tmpBuffer = DKANL_MCTM.ConvertMCTMByteHexString(Item_strSendPackCompare, ref tmpString, Item_strSendParam, ref brtnOk, ref strReason);
                                break;
                            default:
                                return false;
                        }

                        if (brtnOk)
                        {
                            switch (Item_iSerialType)
                            {
                                case (int)RS232.GEN9BYTE:
                                    switch (Item_AnalPack.iAanlyizeOption)
                                    {
                                        case (int)ANALYIZEGEN9.NORESPONSE:
                                            strGetData = "OK";
                                            iRtnState = (int)STATUS.OK;
                                            break;
                                        default:

                                            iRtnState = DKANL_GEN9.AnalyzePacket(ref sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString, Item_AnalPack.iAanlyizeOption, strParam);
                                            break;
                                    }
                                    break;
                                case (int)RS232.GEN10BYTE:

                                    switch (Item_AnalPack.iAanlyizeOption)
                                    {
                                        case (int)ANALYIZEGEN10.NORESPONSE:
                                            strGetData = "OK";
                                            iRtnState = (int)STATUS.OK;
                                            break;
                                        case (int)ANALYIZEGEN10.BYTEPACK:                                            
                                            iRtnState = DKANL_GEN10.AnalyzeBytePacket(sBuffer, ref strGetData, Item_AnalPack); break;                                       

                                        case (int)ANALYIZEGEN10.BINPACK:
                                            string tmpBuffers1 = System.Text.Encoding.UTF8.GetString(sBuffer);
                                            iRtnState = DKANL_GEN10.AnalyzeBinLogPacket(tmpBuffers1, ref strGetData, Item_AnalPack); break;
                                        case (int)ANALYIZEGEN10.BLUEGO:
                                            string tmpBuffers = System.Text.Encoding.UTF8.GetString(sBuffer);
                                            iRtnState = DKANL_GEN10.AnalyzeBluegoPacket(tmpBuffers, ref strGetData, Item_AnalPack); break;
                                        default:

                                            iRtnState = DKANL_GEN10.AnalyzePacket(ref sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString, Item_AnalPack.iAanlyizeOption);

                                            if (Item_AnalPack.strReplaceString.Length > 5)
                                            {
                                                string strRepl = Item_AnalPack.strReplaceString.Replace("REP=>", String.Empty);
                                                if (strRepl.Equals("<CRLF>"))
                                                {
                                                    strGetData = strGetData.Replace("\r", String.Empty);
                                                    strGetData = strGetData.Replace("\n", String.Empty);
                                                }
                                                else
                                                {
                                                    strGetData = strGetData.Replace(strRepl, String.Empty);
                                                }
                                            }

                                            if (iRtnState.Equals((int)STATUS.OK)) LogPrint();
                                            break;
                                    }
                                    break;

                                case (int)RS232.CCMBYTE:
                                    iRtnState = DKANL_CCM.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString);
                                    break;
                                case (int)RS232.NADBYTE:
                                    iRtnState = DKANL_NAD.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString);
                                    break;
                                case (int)RS232.TCPBYTE:
                                    iRtnState = DKANL_TCP.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString);
                                    if (iRtnState.Equals((int)STATUS.OK)) LogPrint();
                                    break;
                                case (int)RS232.GEN11BYTE:
                                    iRtnState = DKANL_GEN11.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString, Item_bResultCodeOption);
                                    if (iRtnState.Equals((int)STATUS.OK)) LogPrint();
                                    break;
                                case (int)RS232.GEN11PBYTE:
                                    iRtnState = DKANL_GEN11P.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString, Item_bResultCodeOption);
                                    if (iRtnState.Equals((int)STATUS.OK)) LogPrint();
                                    break;
                                case (int)RS232.GEN12BYTE:
                                    iRtnState = KMANL_GEN12.AnalyzePacket(sBuffer, ref strGetData, tmpBuffer, Item_strLogCommandName);
                                    break;
                                case (int)RS232.ATTBYTE:
                                    iRtnState = DKANL_ATT.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString);
                                    if (iRtnState.Equals((int)STATUS.OK)) LogPrint();
                                    break;
                                case (int)RS232.MCTMBYTE:
                                    iRtnState = DKANL_MCTM.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName, ref tmpString, Item_bResultCodeOption);
                                    break;
                                default:

                                    return false;
                            }
                        }
                        else
                        {
                            strGetData = strReason;
                            iRtnState = (int)STATUS.NG;
                        }

                        break;

                    case "DIO":
                        if (Item_strSendPackCompare.Length < 1) return false;
                        tmpBuffer = DKANL_DIO.ConvertByteHexString(Item_strSendPackCompare, false, ref tmpString);
                        tmpString = BitConverter.ToString(sBuffer).Replace("-", " "); //RX 결과용 UI에서.
                        iRtnState = DKANL_DIO.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer, Item_strLogCommandName);

                        if (iRtnState == (int)STATUS.OK)
                        {
                            DIO_COMMAND_SENSOR_CHECK(strCmddata, ref strGetData); //센서명령인지 체크
                            DIO_COMMAND_CURRENT_CHECK(strCmddata, ref strGetData); //커런트명령인지 체크
                        }
                        break;

                    case "AUDIOSELECTOR":
                        if (Item_strSendPackCompare.Length < 1) return false;
                        tmpBuffer = DKANL_AUDIO.ConvertByteHexString(Item_strSendPackCompare, false, ref tmpString);
                        tmpString = BitConverter.ToString(sBuffer).Replace("-", " "); //RX 결과용 UI에서.
                        iRtnState = DKANL_AUDIO.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer);
                        break;
                    case "ADC":
                        if (Item_strSendPackCompare.Length < 1) return false;
                        tmpBuffer = DKANL_ADC.ConvertByteHexString(Item_strSendPackCompare, false, ref tmpString);
                        tmpString = BitConverter.ToString(sBuffer).Replace("-", " "); //RX 결과용 UI에서.
                        iRtnState = DKANL_ADC.AnalyzePacket(sBuffer, ref strCmddata, ref strGetData, tmpBuffer);
                        break;

                    case "TC3000":
                        iRtnState = DKANL_TC3000.AnalyzePacket(sBuffer, ref strGetData, ref tmpString);
                        break;

                    default: return false;
                }    
            }
            catch (System.Exception ex)
            {
                string strExMsg = "";
                if (Item_TBLname.Equals("SET") || Item_TBLname.Equals("CCM") || Item_TBLname.Equals("NAD"))
                {
                    switch (Item_iSerialType)
                    {
                        case (int)RS232.CCMBYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":CCM:" + ex.Message;
                            break;
                        case (int)RS232.NADBYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":NAD:" + ex.Message;
                            break;
                        case (int)RS232.GEN9BYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":GEN9:" + ex.Message;
                            break;
                        case (int)RS232.GEN10BYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":GEN10:" + ex.Message;
                            break;
                        case (int)RS232.TCPBYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":TCP:" + ex.Message;
                            break;
                        case (int)RS232.GEN11BYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":GEN11:" + ex.Message;
                            break;
                        case (int)RS232.GEN11PBYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":GEN11P:" + ex.Message;
                            break;
                        case (int)RS232.GEN12BYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":GEN12:" + ex.Message;
                            break;
                        case (int)RS232.ATTBYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":ATT:" + ex.Message;
                            break;
                        case (int)RS232.MCTMBYTE:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":MCTM:" + ex.Message;
                            break;
                        default:
                            strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname + ":" + ex.Message;
                            break;
                    }
                }
                else
                {
                    strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + Item_TBLname + ":" + ex.Message;
                }                
                STEPMANAGER_VALUE.DebugView(strExMsg);
                iRtnState = (int)STATUS.RUNNING;
            }

            if (bSample.Count < 1)
            {
                iRtnState = (int)STATUS.RUNNING;
                return false;
            }
            
            switch (iRtnState)
            {
                
                case (int)STATUS.RUNNING:
                    return false; //아직 다 못받음. Running ~

                case (int)STATUS.OK:

                    if (iRecvOpt.Equals((int)MODE.NORESPONSE))
                        SendToGateWay((int)STATUS.NG, tmpString, strGetData);
                    else
                    {
                        if (!bMoreCommand) SendToGateWay((int)STATUS.OK, tmpString, strGetData); // 현재 명령 진행 상태를 ACTOR의 게이트웨이로 전송 ㅋㅋ
                    }                    
                    LoggingData("", tmpString, false, sBuffer);
                    ClearBuffer();
                    return true;
                default:
                
                    if (iRecvOpt == (int)MODE.UNTIL || iRecvOpt == (int)MODE.BUFFER)
                    {
                        iRtnState = (int)STATUS.RUNNING;
                        LoggingData("", tmpString, false, sBuffer);
                        ClearBuffer();
                        return false;
                    }  
                   
                    //if(!bMoreCommand) SendToGateWay((int)STATUS.NG, sBufferData.ToString(), strGetData); // 현재 명령 진행 상태를 ACTOR의 게이트웨이로 전송 ㅋㅋ
                    if(!bMoreCommand) SendToGateWay((int)STATUS.NG, tmpString, strGetData); // 현재 명령 진행 상태를 ACTOR의 게이트웨이로 전송 ㅋㅋ
                    LoggingData("", tmpString, false, sBuffer);
                    ClearBuffer();
                    return true;
            }

        }

        private void LoggingData(string strTitle, string strLog, bool bTxRx, byte[] bOrginArray)
        {
            StringBuilder strLogText = new StringBuilder(4096);
            string tmpString = strLog;

            switch (Item_iSerialType)
            {
                case (int)RS232.TEXT:   
                                        string tmpTExr = System.Text.Encoding.UTF8.GetString(bOrginArray);
                                        if(bTxRx)
                                            strLog = strLog.Replace("\r", "[CR]");
                                        else
                                            strLog = tmpTExr.Replace("\r", "[CR]");
                                        strLogText.Append(strLog.Replace("\n", "[LF]")); break;
                case (int)RS232.SCANNER:
                case (int)RS232.MOOHANTECH://BYTE 단위 와 ASCII 단위로 기록하기
                                        string tmpExr = System.Text.Encoding.UTF8.GetString(bOrginArray);
                                        strLog = tmpExr.Replace("\r", ".");
                                        strLog = strLog.Replace("\n", ".");
                                        string tmpLogStr = BitConverter.ToString(bOrginArray).Replace("-", " ");
                                        strLogText.Append(strLog);
                                        strLogText.Append(" (");
                                        strLogText.Append(tmpLogStr);
                                        strLogText.Append(")");
                                        if (bTxRx && Item_strLogCommandName.Contains("DEVICE_CHECK"))
                                        {
                                            strLogText.Append(" [");
                                            strLogText.Append(STEPMANAGER_VALUE.strProgramVersion);
                                            strLogText.Append("]");
                                        }
                                        break;

                case (int)RS232.CCMBYTE:
                case (int)RS232.NADBYTE:
                case (int)RS232.GEN11BYTE:
                case (int)RS232.GEN11PBYTE:
                case (int)RS232.GEN12BYTE:
                case (int)RS232.TCPBYTE:
                case (int)RS232.ATTBYTE:
                case (int)RS232.GEN9BYTE:
                case (int)RS232.GEN10BYTE:
                case (int)RS232.MCTMBYTE:
                                        /*string tmpGExr = System.Text.Encoding.UTF8.GetString(bOrginArray);
                                        strLog = tmpGExr.Replace("\r", ".");
                                        strLog = strLog.Replace("\n", ".");
                                        StringBuilder strRef = new StringBuilder(4096);
                                        CheckNoneAscii(strLog, ref strRef);*/
                                        string tmpGLogStr = String.Empty;                                       
                                        tmpGLogStr = BitConverter.ToString(bOrginArray).Replace("-", " ");
                                        strLogText.Append(tmpGLogStr);
                                        /*
                                        strLogText.Append(" (");
                                        strLogText.Append(strRef);
                                        strLogText.Append(")");*/
                                        break;

                case (int)RS232.HEXBYTE:
                                        strLogText.Append(BitConverter.ToString(bOrginArray).Replace("-", " "));
                                        break;
                default: break;
            }
            if (bTxRx) 
            { 
                strLogText.Insert(0, strTitle); strLogText.Insert(0, "[TX]");
                DKLogger.WriteCommLog(strLogText.ToString(), Item_TBLname + ":" + Item_strLogCommandName, false);
            }
            else 
            { 
                strLogText.Insert(0, strTitle); strLogText.Insert(0, "[RX]");
                DKLogger.WriteCommLog(strLogText.ToString(), Item_TBLname, false);
            }       

            /*
            if (bTxRx) DKLogger.WriteCommLog("[TX]" + strTitle + strLogText, Item_TBLname + ":" + Item_strLogCommandName);
            else       DKLogger.WriteCommLog("[RX]" + strTitle + strLogText, Item_TBLname);
             */
        }

        private bool CheckNoneAscii(string strOrigin, ref StringBuilder strRemove)
        {
            strOrigin = strOrigin.Replace("\0", String.Empty); //널값 제거
            int i = strOrigin.Length;
            string pattern = "[^ -~]*";
            System.Text.RegularExpressions.Regex reg_exp = new System.Text.RegularExpressions.Regex(pattern);

            strRemove.Append(reg_exp.Replace(strOrigin, "")); //이것은 걸러내는거
            //return reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거

            if (i != strRemove.Length) return false;
            return true;
        }
                
        public bool IsWorking()
        {
            //return Item_bDelayOut && Item_bTimeOut;
            return swDelayChecker.IsRunning && swTimeOutChecker.IsRunning;            
        }

        private void InsertMsgQueueData(MsgQueueData MQD)
        {
            lock (lockobjectPQ)
            {
                ProcessQueue.Enqueue(MQD);
            }
        }

        private bool IsExistMsgQueue()
        {
            if(ProcessQueue.Count > 0) return true;
            return false;
        }

        private bool ExcuteMsgQueueData(ref MsgQueueData MQD)
        {
            if (IsExistMsgQueue())
            {
                lock (lockobjectPQ)
                {
                    MQD = ProcessQueue.Dequeue();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DeleteMsgQueueData()
        {
            if (ProcessQueue.Count() < 1) return;
            try
            {
                ProcessQueue.Clear();
            }
            catch { }
        }

#endregion

#region Delay 관련


        private void DelayTimeOutCheck()
        {   

            if (swDelayChecker.ElapsedMilliseconds > dDelayTimeSet*1000)
            {

                switch (Item_iRecvOption)
                {
                    case (int)MODE.AVERAGE:
                    case (int)MODE.SENDRECV:
                    case (int)MODE.MULTIPLE:
                    case (int)MODE.NORESPONSE:
                                            TimeOutFlag(true);
                                            SendData(); break;
                    case (int)MODE.SEND:    TimeOutFlag(false);
                                            SendData();
                                            SendToGateWay((int)STATUS.OK, "", ""); // 현재 명령 진행 상태를 ACTOR의 게이트웨이로 전송 ㅋㅋ
                                            break;

                    case (int)MODE.BUFFER: 
                    case (int)MODE.UNTIL:   
                                            TimeOutFlag(true);
                                            SendData(); break;
                    case (int)MODE.RECV:    TimeOutFlag(true); break;
                    case (int)MODE.RECVSEND:TimeOutFlag(true); break;

                }
                //Item_bDelayOut = false; 
                swDelayChecker.Stop();
            }     
            
        }        

        public bool DelayStart(double dDelayTime, int iOption)
        {
            
            if (dDelayTime > 0.1)
            {               
                dDelayTimeSet = dDelayTime;
            }
            else
            {
                dDelayTime = 0.05;                
                dDelayTimeSet = dDelayTime;                
            }

            try
            {
                sBufferData.Clear();
                bSample.Clear();
            }
            catch { }

            //Item_bDelayOut = true;                        
            swDelayChecker.Restart();
            return true;
        }

        private void DelayStop()
        {
            //Item_bDelayOut = false;
            swDelayChecker.Stop();
        }
        
        
#endregion  

#region TIME OUT 관련

        private bool RecvTimeOutCheck()
        {
            //dtCurrTime = DateTime.Now;
            //tsNow = dtCurrTime - dtOutSet;

            //if (tsNow.TotalSeconds > Item_dTimeSet)
            if (swTimeOutChecker.ElapsedMilliseconds > Item_dTimeSet * 1000)
            {
                TimeOutFlag(false);
                DataScanProcess2();// 타임아웃 데이터 처리.
                return false;                
            }
            return true;
        }

        private bool DirectRecvTimeOutCheck(DateTime dtOut, double dTime)
        {
            DateTime dtCurr = DateTime.Now;
            TimeSpan tsN = dtCurr - dtOut;

            if (tsN.TotalSeconds > dTime)
            {
                // 타임아웃 데이터 처리.
                return false;
            }
            return true;
        }
      
        private bool SemiRecvTimeOutCheck(DateTime dtStartTime, double dTimeout) //GEN10 멀티용
        {
            DateTime dtNowTime = DateTime.Now;
            TimeSpan tsCheck = dtNowTime - dtStartTime;            

            if (tsCheck.TotalSeconds > dTimeout)
            {   
                return false;
            }
            return true;
        }
       
        private void TimeOutFlag(bool bOn)
        {      
            //if (bOn) dtOutSet = DateTime.Now;      
            //Item_bTimeOut = bOn;                

            if (bOn)
                swTimeOutChecker.Restart();
            else
                swTimeOutChecker.Stop();
        }

#endregion
              
    }
}
