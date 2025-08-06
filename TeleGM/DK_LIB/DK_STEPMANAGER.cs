//#define USE_MOOHAN_SERVER //MOOHAN 서버에서 오라클 테스트할경우 선언

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Runtime.InteropServices;
using vxlapi_NET;


namespace GmTelematics
{

#region 구조체 및 델리게이트 선언

    delegate void EventThreadStatus(THREDSTATUS cParam);      //이벤트 날릴 대리자
    delegate bool EventMANAGER(RESDATA cParam);      //이벤트 날릴 대리자
    delegate void EventDeviceStatus(DEVICEDATA cParam);      //이벤트 날릴 대리자
    delegate void EventSetJumpResult(int iJobNum, STATUS StsResult);      //이벤트 날릴 대리자
    delegate void EventGotoTestGridRow(int iJobNum);      //이벤트 날릴 대리자

    //--- GEN9 GPS INFO STRUCTURE

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GEN9_GPS_REPORT
    {
        public byte   dummy1;
        public byte   dummy2;
        public byte   dummy3;
        public byte   dummy4;
        public ushort utc_year;
        public byte   utc_month;
        public byte   utc_day;
        public byte   utc_hour;
        public byte   utc_min;
        public float utc_sec;
        public double lat;
        public double lon;
        public short elevation;

        public float sog;
        public float cog;
        public ushort nav_mode;
        public float hdop;
        public float pdop;
        public byte sv_used_cnt;
        public ushort nav_valid;
        public ushort gps_week;
        public ushort gps_tow;
        public uint used_SV;
        public ushort ehpe;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct GEN9_GPS_SVINFO
    {
        public uint dummy;
        public byte sv_id;
        public byte sv_cno;
        public double sv_pseudorange;
        public float sv_azimuth;
        public float sv_elevation;
        public ushort sv_state;        
    }
    //-------------------------------------

    //--- GEN10 OLD GPS INFO STRUCTURE

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]    
    public struct GPS_REPORT
    {       
       public ushort  utc_year;
       public byte    utc_month;
       public byte    utc_day;
       public byte    utc_hour;
       public byte    utc_min;
       public float   utc_sec;
       public double  lat;
       public double  lon;
       public short     elevation;
       public float   sog;
       public float   cog;
       public ushort  nav_mode;
       public float   hdop;
       public float   pdop;
       public byte    sv_used_cnt;
       public ushort  nav_valid;
       public ushort  gps_week;
       public ushort    gps_tow;
       public uint    used_SV;
       public ushort    ehpe;
       public byte    coldState;
       public byte    DR_Cal_Invalid;
       public byte    Location_Fix_Type;
       public byte    GPS_Sig_Strength;
       public float   fltDHdgFF;
       public float   fltDDstFF;
       public int     lSysTimeMsFF;
       public double  dblENCov;

       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]  //문서상 5인데 4로하면 됌.. 구조체 정보 오류가 있음.
       public double[] dblVarX;//[5];        
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]  //C++ 1736 C# 1744
       public double[] d_VelEnu;//[3];        
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] 
       public float[]  f_VelUncEnu;//[3];

       public float   vdop;
       public short     octantHeading;
       public bool    Location_Valid_Status;       
       public byte    cPRNDL;

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct GPS_SVINFO
    {
        public byte sv_id;
        public byte sv_cno;
       public double      sv_pseudorange;
       public float       sv_azimuth;
       public float       sv_elevation;
       public ushort      sv_state;
       public double      sv_c_phase;
       public byte        sv_goodObs;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct GPS_PERIODIC
    {
       public GPS_REPORT    stGpsRegular;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] 
       public GPS_SVINFO[]  stGpsSV;//[16];       
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] 
       public GPS_SVINFO[]  stGlonassSV;//[16];
        public byte nGpsSVCnt;
        public byte nGlonassSVCnt;
    }
    //--------------------------


    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct GEN9_GPS_PERIODIC
    {
        public GEN9_GPS_REPORT stGpsRegular;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public GEN9_GPS_SVINFO[] stGpsSV;//[16];       
             
    }


    public struct JOBFILES
    {
        public string[] strJOB;
        public string strChkSum;
    }

    public struct PWUSER
    {
        public string strLogName;
        public string strPassword;
        public bool bEdit;
        public bool bJob;
        public bool bConfig;
        public bool bMes;
        public bool bAutoJobConfig;
    }

    public struct ThreadStatus
    {
        public int iRecving;
        public int iClearing;
        public int iDelaying;
        public int iBufferCount;
        public string strMsg;
    }

    public struct SUFFIXLIST
    {
        public int iDx;
        public string strSuffix;
    }

    public struct AnalyizePack
    {
        public int iAanlyizeOption;
        public string strAanlyizeString;
        public string strReplaceString;
        public bool bResultCodeOption;
    }

    public struct SENSEDATA
    { //START, SPARE1, SPARE2, MUTE, M1, M2, BUB, PALT, SET, STOP
        public bool bStart;
        public bool bSpare1;
        public bool bSpare2;
        public bool bMute;
        public bool bM1;
        public bool bM2;
        public bool bBUB;
        public bool bSET;
        public bool bStop;
    }

    public struct THREDSTATUS
    {
        public ThreadStatus tDio;
        public ThreadStatus tSet;
        public bool[] bLedStatus;
        public bool bEngineStatus;

        public string strCommandState;
        //여기에 커맨드 상태 추가하자
    }

    public struct LABELSRUCT
    {
        public string strLableName;
        public int iIndex;
    }

    public struct PACKTYPE
    {
        public int iType;
        public int iPos;
        public bool bSplit;
    }

    public struct SETPACK
    {
        public int iType;
        public int iCount;
        public bool bSplit;
    }

    public struct RESDATA
    {
        public int iType;           //MSG  TYPE
        public int iPortNum;        //COMM NUMBER
        public int iSequenceNum;    //SEQUENCE NUMBER
        public int iStatus;         //STATUS CODE
        public string strCMDTYPE;      //COMMAND TYPE
        public string strCMD;          //RESULT MSG
        public string strDisplayName;  //RESULT MSG
        public string ResultData;      //RESULT MSG
        public string ResponseData;    //RESULT MSG
        public string strChangeMax; //EXPR 에 의한 변경값 표시
        public string strChangeMin; //EXPR 에 의한 변경값 표시
        public string LapseTime; //해당명령 걸린시간

    }

    public struct DEVICEDATA
    {
        public int iDevNumber;      //Device 표시순서
        public int iStatus;         //Device Status
    }

    public struct JOBDATA
    {
        public bool   ENABLE;
        public string TYPE;
        public string CMD;
        public string DISPLAYNAME;
        public string MESCODE;
        public string ACTION;
        public string LABEL;
        public string LABELCOUNT;
        public string CASENG;
        public string DELAY;
        public string TIMEOUT;
        public string RETRY;
        public string COMPARE;
        public string MIN;
        public string MAX;
        public string OPTION;
        public string PAR1;
        public string DOC;
        public string EXPR;
  
    }

    public struct TBLDATA0
    {
        public string CMDNAME;
        public string SENDPAC;
        public string RECVPAC;
        public string PARPAC1;
        public string PARPAC2;

        public string OPTION1;
        public string OPTION2;
    }

    public struct TBLMODEL
    {
        public string NAME;
        public string PN;
        public string BTWIFI;
        public string StidMin;
        public string StidMax;
    }

    public struct DTCDATA0
    {
        public string DTCNAME;
        public string DTCCODE;
    }

    public struct BinMsg
    {
        public StringBuilder sb;
        public int iFrom;
    }

    public struct PCanMonitor
    {
        public long lBusHeavy;
        public long lBusLight;
        public long lBusOff;
        public long lReset;
        public string strStatus;
    }

    public struct AttSimInfo
    {
        public string efUST;
        public string efACC;
        public string efECC;
        public string efFPLMN;
        public string efHPPLMN;
        public string efIMSI;
        public string efKEYS;
        public string efKEYSPS;
        public string efLOCI;
        public string efPSLOCI;
        public string efARR;
        public string efSTART_HFN;
        public string efTHRESHOLD;
        public string efAD;
        public string eSimVer;
        public string QualcommChipRev;
    }

    public struct CCMGPSSTRUCT
    {        
        public byte SvCounts;        // Total Gps SV Count        
        public bool bPositionFixed;        
        public CCMGPSNODE[] SVInfo; // sv index, Cn0 value 
    }

    public struct CCMGPSNODE
    {
        public byte   bIndex;
        public double dCn0;
    }

    public struct GPRMCDATA
    {
        public string strReliability; //신뢰성, A:신뢰 V:비신뢰
        public string strlatitude;    //위도
        public string strlongitude;   //경도
        public string strFullPacket;   //전체패킷
    }

    public struct InspDoc
    {
        public string Index;    //의미없음
        public string Category; //나중에 또 사용해달라고 할까봐 확장성을 위해..
        public string SpecItem; //실제 사용되는 값
        public string Contents; //실제 사용되는 값
        public string Remark;   //나중에 또 사용해달라고 할까봐 확장성을 위해.. 
    }

#endregion

    static class STEPMANAGER_VALUE
    {
        public static double dMemoryUsed;
        public static bool bProgramRun;        
        public static bool bNadKeyDllRun;
        public static bool bInteractiveMode;        //인터랙티브모드                              
        public static bool bUseMesOn;
        public static bool bUseOracleOn;
        public static bool bUseAutoJobOn;
        public static bool bIamMD5 = false;
        //CSMES
        public static bool bUseOSIMES = false;

        public static string strKALS_DID_SEED_FileName;
        public static string strKALS_DID_SEED_FilePath;
        
        public static string strKALS_SiteCode;
        public static string strKALS_UPLOAD_WIPID;

        public static string strExcel_FileName;
        public static string strExcel_FieldName;

        public static bool bNowMsgPop;

        public static string strTactTime;

        public static string strProgramVersion;  //이프로그램의 버젼정보를 담아둔다.

        public static int iPopPosLeft;
        public static int iPopPosTop;

        public static string[] KIS_DATA = new string[(int)EFILECODE.END];
		public static bool bMSize; //작은 모니터인지 체크하는 FLAG
        private static int iClickCount1 = 0;
        private static int iClickCount2 = 0;

        private static int iSecCount1 = 0;
        private static int iSecCount2 = 0;

        public static int iUploadBytesCountStartIndex = 0;   //GEN11 인증서 업로드 바이트카운트용
        public static int iUploadBytesCountLength = 0;       //GEN11 인증서 업로드 바이트카운트용
        public static int iUploadBytesCountTotalSize = 0;    //GEN11 인증서 업로드 바이트카운트용

        //위에 있는 변수 찾아서 사용하는 곳에서 추가할지 말지 정해야함 - 경민
        public static int iGen12UploadBytesCountStartIndex = 0;   //GEN12 인증서 업로드 바이트카운트용
        public static int iGen12UploadBytesCountLength     = 0;   //GEN12 인증서 업로드 바이트카운트용
        public static int iGen12UploadBytesCountTotalSize  = 0;   //GEN12 인증서 업로드 바이트카운트용

        public static int iUploadBytesSendCount = 0;    //GEN9, 10 업로드 바이트카운트용
        
        public static List<byte> LstGen9CertFile;

        public static List<string> LstModel;
        public static List<SUFFIXLIST> LstSuffix;

        //private static Queue<StringBuilder> BinLogQueue = new Queue<StringBuilder>();
        private static Queue<BinMsg>        BinLogQueue = new Queue<BinMsg>();
        private static Queue<StringBuilder> EtcLogQueue = new Queue<StringBuilder>();

        public static List<string[]> GEN10APN_TABLE = new List<string[]>();

        public static string[] OOBSimInfo;
        public static string[] OOBServiceInfoA; //GEN10, GEN10 TCP 용
        public static string[] OOBServiceInfoB; //GEN11 용
        //public static string[] OOBServiceInfoC; //GEN12 용 - 경민

        public static byte[] bOldBinaryALDL = new byte[1]; //ATT용 ALDL WRITE 용 바이너리 배열 ( TCP ATT 는 READ 후 XOR 연산 후 WRITE 를 해야하므로 이전데이터를 보관해야한다. 
        public static byte[] bOldBinaryBLCK = new byte[2];
               
        public static byte[] bMctmALDLData = new byte[1]; //mctm 용 aldl array
        public static byte bMctmALDLIndex = 0x00; //mctm 용 aldl address

        private static object lockBinIn = new object();
        private static object lockBinOut = new object();

        private static object lockEtcIn = new object();
        private static object lockEtcOut = new object();
        private static object lockCurrent = new object();
        private static object lockPcanState = new object();

        public static CCMGPSSTRUCT stGEN11CCMGpsInfo  = new CCMGPSSTRUCT();
        public static CCMGPSSTRUCT stGEN11CCMGnssInfo = new CCMGPSSTRUCT();

        //public static CCMGPSSTRUCT stGEN12CCMGpsInfo = new CCMGPSSTRUCT();
        //public static CCMGPSSTRUCT stGEN12CCMGnssInfo = new CCMGPSSTRUCT(); // 경민

        public static GEN9_GPS_REPORTS stGEN9GPSInfo = new GEN9_GPS_REPORTS();

        public static bool bGen11CCMGpsInfo = false;      
        public static System.Diagnostics.Stopwatch Gen11CCMGpsInfoStopWatch  = new System.Diagnostics.Stopwatch();
        public static System.Diagnostics.Stopwatch Gen11CCMGnssInfoStopWatch = new System.Diagnostics.Stopwatch();

        //public static bool bGen12CCMGpsInfo = false;
        //public static System.Diagnostics.Stopwatch Gen12CCMGpsInfoStopWatch = new System.Diagnostics.Stopwatch();
        //public static System.Diagnostics.Stopwatch Gen12CCMGnssInfoStopWatch = new System.Diagnostics.Stopwatch();

        public static bool bGen10GpsOldInfo = false;
        public static bool bGen10GpsNavOn   = false;
        public static string strGen10GpsNavild = "FF";
        public static string strGen10GpsTTFF   = "-1";
        public static double dGen10GpsLat = 0.0;
        public static double dGen10GpsLon = 0.0;

        public static bool bGen9GpsOldInfo = false;
        public static bool bGen9GpsNavOn = false;
        public static string strGen9GpsNavild = "FF";
        public static string strGen9GpsTTFF = "-1";
        public static double dGen9GpsLat = 0.0;
        public static double dGen9GpsLon = 0.0;

        public static int iGen9GpsCount = 0;
        public static int iGen9GpsCn0Max = 0;
        public static int iGen9GpsCn0Aver = 0;

        public static int iAttGpsCount = 0;
        public static int iAttGpsCn0Max = 0;
        public static int iAttGpsCn0Aver = 0;

        public static int iAttGnssCount = 0;
        public static int iAttGnssCn0Max = 0;        
        public static int iAttGnssCn0Aver = 0;
        
        private static DateTime dtm;
        public static List<string> LstVersionHistory = new List<string>();
        public static bool bDebugLogEnable = false;
        
        private static double dCurrent = 0;
        private static int    iCurrentPos = 0;

        public static string strAtcoLoggingPath = String.Empty;

        private static PCanMonitor pcanStatus = new PCanMonitor();
                
        public static GPRMCDATA GPRMC = new GPRMCDATA();

        private static List<byte> bGPRMCPack = new List<byte>();

        private static int GEN11ALDLMAX = 200;
        //private static int GEN12ALDLMAX = 200;

        public static void InitstGEN9GPSInfo()
        {
       
            stGEN9GPSInfo.dummy         = new byte[4];

            stGEN9GPSInfo.utc_year      = new byte[2];
            stGEN9GPSInfo.utc_month     = new byte[1];
            stGEN9GPSInfo.utc_day       = new byte[1];
            stGEN9GPSInfo.utc_hour      = new byte[1];
            stGEN9GPSInfo.utc_min       = new byte[1];
            stGEN9GPSInfo.utc_sec       = new byte[4+2];
            stGEN9GPSInfo.lat           = new byte[8];
            stGEN9GPSInfo.lon           = new byte[8];
            stGEN9GPSInfo.elevation     = new byte[2];

            stGEN9GPSInfo.sog           = new byte[4];
            stGEN9GPSInfo.cog           = new byte[4];
            stGEN9GPSInfo.nav_mode      = new byte[2+2];

            stGEN9GPSInfo.hdop          = new byte[4];
            stGEN9GPSInfo.pdop          = new byte[4];
            stGEN9GPSInfo.sv_used_cnt   = new byte[1+3];
            stGEN9GPSInfo.nav_valid     = new byte[2];
            stGEN9GPSInfo.gps_week      = new byte[2];
            stGEN9GPSInfo.gps_tow       = new byte[4];
            stGEN9GPSInfo.used_SV       = new byte[8];
            stGEN9GPSInfo.ehpe          = new byte[4];

            //int iSizeSturct = Marshal.SizeOf(stGEN9GPSInfo);
          
        }

        public static void InitGen9SVstructure(ref GEN9_SV target)
        {
            target.dummy = new byte[4];
            target.sv_id = new byte[1];
            target.sv_cno = new byte[1+2];
            target.sv_pseudorange = new byte[8];
            target.sv_azimuth = new byte[4];
            target.sv_elevation = new byte[4];
            target.sv_state = new byte[2+6];
        }

        public static bool UpdateGen9Svstructure(int idx, ref GEN9_SV target, byte[] data, bool bGpsHighModel)
        {
            int iPos = idx * 32;

            if (bGpsHighModel)
                iPos = idx * (32 + 16);

            try
            {            
                Array.Copy(data, iPos, target.dummy,            0, target.dummy.Length);            iPos += target.dummy.Length;
                Array.Copy(data, iPos, target.sv_id,            0, target.sv_id.Length);            iPos += target.sv_id.Length;
                Array.Copy(data, iPos, target.sv_cno,           0, target.sv_cno.Length);           iPos += target.sv_cno.Length;
                Array.Copy(data, iPos, target.sv_pseudorange,   0, target.sv_pseudorange.Length);   iPos += target.sv_pseudorange.Length;
                Array.Copy(data, iPos, target.sv_azimuth,       0, target.sv_azimuth.Length);       iPos += target.sv_azimuth.Length;
                Array.Copy(data, iPos, target.sv_elevation,     0, target.sv_elevation.Length);     iPos += target.sv_elevation.Length;
                Array.Copy(data, iPos, target.sv_state,         0, target.sv_state.Length);         iPos += target.sv_state.Length;
            }
            catch 
            {
                return false;
            }

            return true;
        }

        public static void UpdatestGEN9GPSInfo(byte[] data)
        {         

            int iDx = 0;
            try
            {               
                Array.Copy(data, iDx, stGEN9GPSInfo.dummy, 0, stGEN9GPSInfo.dummy.Length); iDx += stGEN9GPSInfo.dummy.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.utc_year, 0, stGEN9GPSInfo.utc_year.Length); iDx += stGEN9GPSInfo.utc_year.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.utc_month, 0, stGEN9GPSInfo.utc_month.Length); iDx += stGEN9GPSInfo.utc_month.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.utc_day, 0, stGEN9GPSInfo.utc_day.Length); iDx += stGEN9GPSInfo.utc_day.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.utc_hour, 0, stGEN9GPSInfo.utc_hour.Length); iDx += stGEN9GPSInfo.utc_hour.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.utc_min, 0, stGEN9GPSInfo.utc_min.Length); iDx += stGEN9GPSInfo.utc_min.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.utc_sec, 0, stGEN9GPSInfo.utc_sec.Length); iDx += stGEN9GPSInfo.utc_sec.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.lat, 0, stGEN9GPSInfo.lat.Length); iDx += stGEN9GPSInfo.lat.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.lon, 0, stGEN9GPSInfo.lon.Length); iDx += stGEN9GPSInfo.lon.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.elevation, 0, stGEN9GPSInfo.elevation.Length); iDx += stGEN9GPSInfo.elevation.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.sog, 0, stGEN9GPSInfo.sog.Length); iDx += stGEN9GPSInfo.sog.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.cog, 0, stGEN9GPSInfo.cog.Length); iDx += stGEN9GPSInfo.cog.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.nav_mode, 0, stGEN9GPSInfo.nav_mode.Length); iDx += stGEN9GPSInfo.nav_mode.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.hdop, 0, stGEN9GPSInfo.hdop.Length); iDx += stGEN9GPSInfo.hdop.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.pdop, 0, stGEN9GPSInfo.pdop.Length); iDx += stGEN9GPSInfo.pdop.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.sv_used_cnt, 0, stGEN9GPSInfo.sv_used_cnt.Length); iDx += stGEN9GPSInfo.sv_used_cnt.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.nav_valid, 0, stGEN9GPSInfo.nav_valid.Length); iDx += stGEN9GPSInfo.nav_valid.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.gps_week, 0, stGEN9GPSInfo.gps_week.Length); iDx += stGEN9GPSInfo.gps_week.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.gps_tow, 0, stGEN9GPSInfo.gps_tow.Length); iDx += stGEN9GPSInfo.gps_tow.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.used_SV, 0, stGEN9GPSInfo.used_SV.Length); iDx += stGEN9GPSInfo.used_SV.Length;
                Array.Copy(data, iDx, stGEN9GPSInfo.ehpe, 0, stGEN9GPSInfo.ehpe.Length); iDx += stGEN9GPSInfo.ehpe.Length;    
            }
            catch
            {
                InitstGEN9GPSInfo();
            }
           
        }
                
        public static string GetGen9Gps(int idx) 
        {
            string strResult = String.Empty;
            try
            {

                switch (idx)
                {
                    case (int)GEN9_GPS_IDX.dummy:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.dummy); break;
                    case (int)GEN9_GPS_IDX.utc_year:
                        Array.Reverse(stGEN9GPSInfo.utc_year);
                        ushort iYear = BitConverter.ToUInt16(stGEN9GPSInfo.utc_year, 0);
                        strResult = iYear.ToString(); break;
                    case (int)GEN9_GPS_IDX.utc_month:                       
                        strResult = (stGEN9GPSInfo.utc_month[0]).ToString(); break;
                    case (int)GEN9_GPS_IDX.utc_day:
                        strResult = (stGEN9GPSInfo.utc_day[0]).ToString(); break;
                    case (int)GEN9_GPS_IDX.utc_hour:
                        strResult = (stGEN9GPSInfo.utc_hour[0]).ToString(); break;
                    case (int)GEN9_GPS_IDX.utc_min:
                        strResult = (stGEN9GPSInfo.utc_min[0]).ToString(); break;

                    case (int)GEN9_GPS_IDX.utc_sec:
                        Array.Reverse(stGEN9GPSInfo.utc_sec);
                        Single ssec = BitConverter.ToSingle(stGEN9GPSInfo.utc_sec, 0);
                        strResult = ssec.ToString("00.000"); break;

                    case (int)GEN9_GPS_IDX.lat:
                        Array.Reverse(stGEN9GPSInfo.lat);
                        double dlat = BitConverter.ToDouble(stGEN9GPSInfo.lat, 0) / 10000000;
                        strResult = dlat.ToString("0.######");
                        break;                        
                    case (int)GEN9_GPS_IDX.lon:
                        Array.Reverse(stGEN9GPSInfo.lon);
                        double dlon = BitConverter.ToDouble(stGEN9GPSInfo.lon, 0) / 10000000;
                        strResult = dlon.ToString("0.######");
                        break; 
                    case (int)GEN9_GPS_IDX.elevation:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.elevation); break;
                    case (int)GEN9_GPS_IDX.sog:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.sog); break;
                    case (int)GEN9_GPS_IDX.cog:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.cog); break;
                    case (int)GEN9_GPS_IDX.nav_mode:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.nav_mode); break;
                    case (int)GEN9_GPS_IDX.hdop:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.hdop); break;
                    case (int)GEN9_GPS_IDX.pdop:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.pdop); break;
                    case (int)GEN9_GPS_IDX.sv_used_cnt:
                        Array.Reverse(stGEN9GPSInfo.sv_used_cnt);
                        int icnt = (int)(stGEN9GPSInfo.sv_used_cnt[1]);
                        strResult = icnt.ToString();
                        break; 
                    case (int)GEN9_GPS_IDX.nav_valid:
                        Array.Reverse(stGEN9GPSInfo.nav_valid);
                        UInt16 invalid = BitConverter.ToUInt16(stGEN9GPSInfo.nav_valid, 0);
                        strResult = invalid.ToString();
                        break;                         
                    case (int)GEN9_GPS_IDX.gps_week:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.gps_week); break;
                    case (int)GEN9_GPS_IDX.gps_tow:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.gps_tow); break;
                    case (int)GEN9_GPS_IDX.used_SV:
                        Array.Reverse(stGEN9GPSInfo.used_SV);
                        UInt64 lusedsv = BitConverter.ToUInt64(stGEN9GPSInfo.used_SV, 0);
                        strResult = lusedsv.ToString();
                        break;                         
                    case (int)GEN9_GPS_IDX.ehpe:
                        strResult = Encoding.UTF8.GetString(stGEN9GPSInfo.ehpe); break;

                    default: break;

                }
            }
            catch
            {
                strResult = "get error";
            }
            return strResult;
        }
    
        public static void DefaultGen11AldlBlockSize()
        {
            GEN11ALDLMAX = 200;
        }

        public static void SetGen11AldlBlockSize(int iSize)
        {
            GEN11ALDLMAX = iSize;
        }

        public static int GetGen11AldlBlockSize()
        {
            return GEN11ALDLMAX;
        }

        //경민 추가(아래 3개 함수)
        //public static void DefaultGen12AldlBlockSize()
        //{
        //    GEN12ALDLMAX = 200;
        //}

        //public static void SetGen12AldlBlockSize(int iSize)
        //{
        //    GEN12ALDLMAX = iSize;
        //}

        //public static int GetGen12AldlBlockSize()
        //{
        //    return GEN12ALDLMAX;
        //}

        public static void processGPRMC(byte[] bGPRMCsample)
        {
            //GPRMC Recommended Minimmum data는 추천되는 최소한의 데이터들이다.

            //규칙1 STX : $   ETX : 0x0d 0x0a
            //규칙2 콤마갯수 : 12개

            //'$'로 시작한다.
            //첫 두 자리는 제품의 종류를 나타낸다. GPS 제품일 경우 GP, 수심 측정 장비인 Depth Sounder 제품일 경우 SD 를 사용한다.
            //다음 세 자리는 해당 프로토콜이 가지고 있는 데이터의 종류를 나타낸다.
            //데이터의 구분은 ','로 한다.
            //'*'로 끝난다.
            //'$'와 '*'사이의 모든 데이터를 exclusive or 연산을 하여 체크섬을 만들어 추가한다.
            //<CR><LF>를 붙인다.

            //$GPRMC,075201.00,V,00.0000,S,09.0000,E,0.0,0.0,260917,0.0,E,N*03           //연결안했을때 데이터 포멧
            //$GPRMC,114455.532,A,3735.0079,N,12701.6446,E,0.000000,121.61,110706,,*0A   //연결했을때 데이터 포멧

            //114455.532 : 는 시간으로서 Zulu time (그리니치 표준시) 기준으로 11시 44분 55.532초를 뜻한다.
            //A          : 는 GPS 신호의 신뢰성을 뜻한다. (A = 신뢰할 수 있음, V = 신뢰할 수 없음)
            //3735.0079  : 는 위도로서 37도 35.0079분을 뜻한다. 도(degree) 단위로 환산시, 35.0079/60 = 0.5 대략 37.5도가 된다.
            //N          : 은 북위를 뜻한다. 적도 남쪽에 있다면 S가 된다.
            //12701.6446 : 은 경도로서 127도 1.6446분을 뜻한다. 도(degree) 단위로 환산시, 1.6446/60 = 0.027 대략 127.0도가 된다.
            //E          : 는 동경을 뜻한다. W가 되면 서경이 된다.
            //'0.000000' : 은 Speed over ground로서 knots 단위의 속도계이다. 비행기에서는 KIAS라는 속도 단위를 사용하고, 배에서는 Knots를 사용한다. 
            //             KIAS는 Knots indicator air speed의 약자이다. km/h로 변환시 대략 1.8을 곱한다.
            //'121.61'   : 은 Track Angle in degree true로서, 진행 방향을 정북을 0도부터 359도 까지 표현한 것이다. 121.61은 대략 동남쪽이다.
            //'110706'   : 은 Date를 뜻한다. 여기에서는 11th, July, 2006이며 2006년 7월 11일이다.
            //' '는 Magnetic Variation으로서 나침반이다. 예시의 GPS Module은 나침반이 내장되어 있지 않다.
            //'*0A'      : 는 체크섬이다.('$'와 '*'사이의 모든 데이터를 exclusive or 연산을 하여 체크섬을 만들어 추가한다.)

            //GPRMC.strlatitude
            try
            {
                bGPRMCPack.AddRange(bGPRMCsample);
                List<string> lstData = new List<string>();
                int iProcessIndex = 0;
                int iRes = AnalyzeGPRMC(bGPRMCPack.ToArray(), ref lstData, ref iProcessIndex);
                if (iRes == (int)STATUS.OK)
                {
                    StringBuilder strDebugMsg = new StringBuilder(4096);
                    for (int i = 0; i < lstData.Count; i++)
                    {
                        strDebugMsg.Clear();
                        strDebugMsg.Append(lstData[i]);
                        InsertEtcMsgQueue(strDebugMsg);
                        
                        // strReliability; //신뢰성, A:신뢰 V:비신뢰
                        // strlatitude;    //위도
                        // strlongitude;   //경도

                        if(i == lstData.Count -1)
                        {
                            string[] strPacks = lstData[i].Split(',');
                            if (strPacks.Length > 10)
                            {
                                if(strPacks[0].Equals("$GPRMC"))
                                {
                                    GPRMC.strFullPacket = lstData[i];
                                    GPRMC.strReliability = strPacks[2];
                                    GPRMC.strlatitude = strPacks[3];
                                    GPRMC.strlongitude = strPacks[5];
                                }
                            }
                        }
                        
                        
                    }

                    bGPRMCPack.RemoveRange(0, iProcessIndex);
                }

                if (bGPRMCPack.Count > 8096) bGPRMCPack.Clear();
            }
            catch (System.Exception ex)
            {
                string strEx = ex.Message;
                return;
            }
            
        }

        private static int AnalyzeGPRMC(byte[] tmpBytes, ref List<string> rtnData, ref int iProcessCount)
        {
            //$GPRMC, 075201.00,V,  00.0000,S,   09.0000,E,0.0     ,   0.0,260917,0.0,E,N*03           //연결안했을때 데이터 포멧
            //$GPRMC,114455.532,A,3735.0079,N,12701.6446,E,0.000000,121.61,110706,   ,*0A   //연결했을때 데이터 포멧

            //strData3 = BitConverter.ToString(bData3, 0, (int)iDataLen).Replace("-", "");
            try
            {
            byte bSTX1 = 0x24; //$
            byte bSTX2 = 0x47; //G
            byte bETX1 = 0x0D; //CR
            byte bETX2 = 0x0A; //LF
            iProcessCount = 0;
            rtnData.Clear();
           
                //0. 응답패킷이 50바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 20) return (int)STATUS.RUNNING;

                //1. 버퍼링 나누기
                int bChksum = (int)STATUS.TIMEOUT;
                int iFind = 0;
                int iFindStx = 0;
                bool bFind = false;
                for (int j = 0; j < tmpBytes.Length; j++)
                {

                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == bSTX1 && tmpBytes[j + 1] == bSTX2)
                    {
                        iFindStx = j; bFind = true;
                    }
                    if (bFind && tmpBytes[j] == bETX2 && tmpBytes[j - 1] == bETX1)
                    {
                        byte[] tmpBuffer = new byte[iFind + 1];

                        //체크섬 일단 bypass
                        Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);
                        string strTmp = Encoding.UTF8.GetString(tmpBytes, iFindStx, tmpBuffer.Length - 2).Replace("-", String.Empty);
                        rtnData.Add(strTmp);

                        iFind = 0;
                        bFind = false;
                        iProcessCount = j;
                        bChksum = (int)STATUS.OK;
                        
                    }

                    if (bFind) iFind++;
                }

                switch (bChksum)
                {
                    case (int)STATUS.OK: return (int)STATUS.OK;
                    case (int)STATUS.NG: return (int)STATUS.NG;
                    case (int)STATUS.CHECK: return (int)STATUS.CHECK;
                    default: return (int)STATUS.RUNNING;
                }
            }
            catch (Exception ex)
            {
                string strExcel_FieldName = "Exception: AnalyzePacket(GPRMC) : " + ex.Message;
                return (int)STATUS.RUNNING;
            }

        }

        //원본소스
        public static void InitCcmGpsStructure()
        {
            stGEN11CCMGpsInfo = new CCMGPSSTRUCT();
            stGEN11CCMGpsInfo.SVInfo = new CCMGPSNODE[16];
            stGEN11CCMGpsInfo.bPositionFixed = false;

            stGEN11CCMGnssInfo = new CCMGPSSTRUCT();
            stGEN11CCMGnssInfo.SVInfo = new CCMGPSNODE[16];
            stGEN11CCMGnssInfo.bPositionFixed = false;

            Gen11CCMGpsInfoStopWatch.Reset();
            Gen11CCMGnssInfoStopWatch.Reset();

            GPRMC = new GPRMCDATA();
        }

        //경민 추가
        //public static void InitCcmGpsStructure()
        //{
        //    stGEN11CCMGpsInfo = new CCMGPSSTRUCT();
        //    stGEN11CCMGpsInfo.SVInfo = new CCMGPSNODE[16];
        //    stGEN11CCMGpsInfo.bPositionFixed = false;

        //    stGEN11CCMGnssInfo = new CCMGPSSTRUCT();
        //    stGEN11CCMGnssInfo.SVInfo = new CCMGPSNODE[16];
        //    stGEN11CCMGnssInfo.bPositionFixed = false;

        //    Gen11CCMGpsInfoStopWatch.Reset();
        //    Gen11CCMGnssInfoStopWatch.Reset();

        //    stGEN12CCMGpsInfo = new CCMGPSSTRUCT();
        //    stGEN12CCMGpsInfo.SVInfo = new CCMGPSNODE[16];
        //    stGEN12CCMGpsInfo.bPositionFixed = false;

        //    stGEN12CCMGnssInfo = new CCMGPSSTRUCT();
        //    stGEN12CCMGnssInfo.SVInfo = new CCMGPSNODE[16];
        //    stGEN12CCMGnssInfo.bPositionFixed = false;

        //    Gen12CCMGpsInfoStopWatch.Reset();
        //    Gen12CCMGnssInfoStopWatch.Reset();

        //    GPRMC = new GPRMCDATA();
        //}

        public static void ClearPcanStatus()
        {
            try
            {
                pcanStatus = new PCanMonitor();
                pcanStatus.lBusHeavy = 0;
                pcanStatus.lBusLight = 0;
                pcanStatus.lBusOff = 0;
                pcanStatus.lReset = 0;
                pcanStatus.strStatus = "OFF";
            }
            catch { }            
        }

        public static void AddPcanStatus(int iBusStatus, string strStatus)
        {
            lock (lockPcanState)
            {
                try
                {    
                    switch (iBusStatus)
                    {
                        case (int)PCANBUS.HEAVY: pcanStatus.lBusHeavy++; break;
                        case (int)PCANBUS.LIGHT: pcanStatus.lBusLight++; break;
                        case (int)PCANBUS.OFF:   pcanStatus.lBusOff++;   break;
                        case (int)PCANBUS.RESET: pcanStatus.lReset++;    break;
                        default: break;
                    }
                    pcanStatus.strStatus = strStatus;
                }
                catch 
                {
                    ClearPcanStatus();
                }
            }            
        }

        public static PCanMonitor GetPcanStatus()
        {
            return pcanStatus;
        }

        public static void DebugView(string strDebug)
        {

            if (strDebug.Contains("isAnalogLibrary") || strDebug.Contains("isDigitalLibrary") || strDebug.Contains("iPlotLibrary"))
            {
                MessageBox.Show("CHECK LIBRARY PATH or PROGRAM FOLDER NAME");
            }

            if (bDebugLogEnable)
            {
                StringBuilder strDebugMsg = new StringBuilder(4096);
                strDebugMsg.Append(strDebug);
                InsertEtcMsgQueue(strDebugMsg);
            }
        }

        public static double GetCurrent(ref int iPos)
        {
            iPos = iCurrentPos;
            if (dCurrent <= 0)
                return 0;
            else
                return dCurrent;
        }

        public static void SetCurrent(double dCurr, int iPos)
        {
            lock (lockCurrent)
            {
                dCurrent = dCurr;
                if (iPos > 0)
                    iCurrentPos = iPos;
            }
        }        

        public static void SetGEN11CCMGPSInfo(byte[] bInfo)
        {
            if (!bGen11CCMGpsInfo) return;
            
            //STATUS & ETX 체크

            if (!bInfo[55].Equals(0x00) || !bInfo[58].Equals(0x7E)) 
                return;

            switch (bInfo[3]) //DRE 분류
            {
                case 0x01:  // REPORT - MEASUREMENT
                            CCMGPSInfo_Measurement(bInfo); 
                            break;

                case 0x02:  // REPORT - POSITION FIX
                            CCMGPSInfo_PositionFix(true, bInfo); 
                            break;

                case 0x04:  // REPORT - NO FIX [5]부터 ~ 51개가 전부 null(0x00) 이면 NO FIX
                            CCMGPSInfo_PositionFix(false, bInfo);
                            break;


                case 0x03:  // REPORT - SV POLY ??                            
                default: return;
            }              
        }

        //경민 추가
        //public static void SetGEN12CCMGPSInfo(byte[] bInfo)
        //{
        //    if (!bGen12CCMGpsInfo) return;

        //    //STATUS & ETX 체크

        //    if (!bInfo[55].Equals(0x00) || !bInfo[58].Equals(0x7E))
        //        return;

        //    switch (bInfo[3]) //DRE 분류
        //    {
        //        case 0x01:  // REPORT - MEASUREMENT
        //            CCMGPSInfo_Measurement(bInfo, "GEN12");
        //            break;

        //        case 0x02:  // REPORT - POSITION FIX
        //            CCMGPSInfo_PositionFix(true, bInfo, "GEN12");
        //            break;

        //        case 0x04:  // REPORT - NO FIX [5]부터 ~ 51개가 전부 null(0x00) 이면 NO FIX
        //            CCMGPSInfo_PositionFix(false, bInfo, "GEN12");
        //            break;


        //        case 0x03:  // REPORT - SV POLY ??                            
        //        default: return;
        //    }
        //}

        private static void CCMGPSInfo_Measurement(byte[] bInfo)
        {
            int iInfoIndex = 5;
            StringBuilder strEtcMsg = new StringBuilder(4096);
            double[] MaxCN0s;

            MaxCN0s = new double[stGEN11CCMGpsInfo.SVInfo.Length];

            switch (bInfo[4])
            {
                case 0x00: //GPS
                    strEtcMsg.Append("[REPORT]GPS.  SV:");
                    STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SvCounts = bInfo[iInfoIndex]; iInfoIndex++;

                    for (int i = 0; i < STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo.Length; i++)
                    {
                        STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].bIndex = bInfo[iInfoIndex]; iInfoIndex++;
                        try
                        {
                            STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0 = ((double)BitConverter.ToInt16(bInfo, iInfoIndex) / 10);
                        }
                        catch
                        {
                            STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0 = 0;
                        }

                        iInfoIndex += 2;
                        MaxCN0s[i] = STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0;
                    }

                    strEtcMsg.Append(((int)STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SvCounts).ToString("00"));
                    strEtcMsg.Append(", CN0:");
                    strEtcMsg.Append(MaxCN0s.Max().ToString("000.0"));
                    strEtcMsg.Append(", ");
                    //strEtcMsg.Append(GetPositionStatus(true, "GEN11"));
                    strEtcMsg.Append(GetPositionStatus(true));
                    strEtcMsg.Append(", ");
                    strEtcMsg.Append(BitConverter.ToString(bInfo).Replace("-", " "));
                    break;

                case 0x01: //GNSS
                    strEtcMsg.Append("[REPORT]GNSS. SV:");
                    STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SvCounts = bInfo[iInfoIndex]; iInfoIndex++;

                    for (int i = 0; i < STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo.Length; i++)
                    {
                        STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].bIndex = bInfo[iInfoIndex]; iInfoIndex++;
                        try
                        {
                            STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0 = ((double)BitConverter.ToInt16(bInfo, iInfoIndex) / 10);
                        }
                        catch
                        {
                            STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0 = 0;
                        }
                        iInfoIndex += 2;
                        MaxCN0s[i] = STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0;
                    }
                    strEtcMsg.Append(((int)STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SvCounts).ToString("00"));
                    strEtcMsg.Append(", CN0:");
                    strEtcMsg.Append(MaxCN0s.Max().ToString("000.0"));
                    strEtcMsg.Append(", ");
                    //strEtcMsg.Append(GetPositionStatus(false, "GEN11"));
                    strEtcMsg.Append(GetPositionStatus(false));
                    strEtcMsg.Append(", ");
                    strEtcMsg.Append(BitConverter.ToString(bInfo).Replace("-", " "));
                    break;
                default: return;
            }

            //아래 경민 추가
            //switch (strTarget)
            //{
            //    case "GEN11":
            //        MaxCN0s = new double[stGEN11CCMGpsInfo.SVInfo.Length];

            //        switch (bInfo[4])
            //        {
            //            case 0x00: //GPS
            //                strEtcMsg.Append("[REPORT]GPS.  SV:");
            //                STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SvCounts = bInfo[iInfoIndex]; iInfoIndex++;

            //                for (int i = 0; i < STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo.Length; i++)
            //                {
            //                    STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].bIndex = bInfo[iInfoIndex]; iInfoIndex++;
            //                    try
            //                    {
            //                        STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0 = ((double)BitConverter.ToInt16(bInfo, iInfoIndex) / 10);
            //                    }
            //                    catch
            //                    {
            //                        STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0 = 0;
            //                    }

            //                    iInfoIndex += 2;
            //                    MaxCN0s[i] = STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0;
            //                }

            //                strEtcMsg.Append(((int)STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SvCounts).ToString("00"));
            //                strEtcMsg.Append(", CN0:");
            //                strEtcMsg.Append(MaxCN0s.Max().ToString("000.0"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(GetPositionStatus(true, "GEN11"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(BitConverter.ToString(bInfo).Replace("-", " "));
            //                break;

            //            case 0x01: //GNSS
            //                strEtcMsg.Append("[REPORT]GNSS. SV:");
            //                STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SvCounts = bInfo[iInfoIndex]; iInfoIndex++;

            //                for (int i = 0; i < STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo.Length; i++)
            //                {
            //                    STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].bIndex = bInfo[iInfoIndex]; iInfoIndex++;
            //                    try
            //                    {
            //                        STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0 = ((double)BitConverter.ToInt16(bInfo, iInfoIndex) / 10);
            //                    }
            //                    catch
            //                    {
            //                        STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0 = 0;
            //                    }
            //                    iInfoIndex += 2;
            //                    MaxCN0s[i] = STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0;
            //                }
            //                strEtcMsg.Append(((int)STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SvCounts).ToString("00"));
            //                strEtcMsg.Append(", CN0:");
            //                strEtcMsg.Append(MaxCN0s.Max().ToString("000.0"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(GetPositionStatus(false, "GEN11"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(BitConverter.ToString(bInfo).Replace("-", " "));
            //                break;
            //            default: return;
            //        }
            //    break;
            //    case "GEN12":

            //        MaxCN0s = new double[stGEN12CCMGpsInfo.SVInfo.Length];

            //        switch (bInfo[4])
            //        {
            //            case 0x00: //GPS
            //                strEtcMsg.Append("[REPORT]GPS.  SV:");
            //                STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SvCounts = bInfo[iInfoIndex]; iInfoIndex++;

            //                for (int i = 0; i < STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo.Length; i++)
            //                {
            //                    STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo[i].bIndex = bInfo[iInfoIndex]; iInfoIndex++;
            //                    try
            //                    {
            //                        STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo[i].dCn0 = ((double)BitConverter.ToInt16(bInfo, iInfoIndex) / 10);
            //                    }
            //                    catch
            //                    {
            //                        STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo[i].dCn0 = 0;
            //                    }

            //                    iInfoIndex += 2;
            //                    MaxCN0s[i] = STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo[i].dCn0;
            //                }

            //                strEtcMsg.Append(((int)STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SvCounts).ToString("00"));
            //                strEtcMsg.Append(", CN0:");
            //                strEtcMsg.Append(MaxCN0s.Max().ToString("000.0"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(GetPositionStatus(true, "GEN12"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(BitConverter.ToString(bInfo).Replace("-", " "));
            //                break;

            //            case 0x01: //GNSS
            //                strEtcMsg.Append("[REPORT]GNSS. SV:");
            //                STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SvCounts = bInfo[iInfoIndex]; iInfoIndex++;

            //                for (int i = 0; i < STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo.Length; i++)
            //                {
            //                    STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo[i].bIndex = bInfo[iInfoIndex]; iInfoIndex++;
            //                    try
            //                    {
            //                        STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo[i].dCn0 = ((double)BitConverter.ToInt16(bInfo, iInfoIndex) / 10);
            //                    }
            //                    catch
            //                    {
            //                        STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo[i].dCn0 = 0;
            //                    }
            //                    iInfoIndex += 2;
            //                    MaxCN0s[i] = STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo[i].dCn0;
            //                }
            //                strEtcMsg.Append(((int)STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SvCounts).ToString("00"));
            //                strEtcMsg.Append(", CN0:");
            //                strEtcMsg.Append(MaxCN0s.Max().ToString("000.0"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(GetPositionStatus(false, "GEN12"));
            //                strEtcMsg.Append(", ");
            //                strEtcMsg.Append(BitConverter.ToString(bInfo).Replace("-", " "));
            //                break;
            //            default: return;
            //        }
            //    break;
            //    default: break;
            //}
            InsertEtcMsgQueue(strEtcMsg);
        }

        private static void CCMGPSInfo_PositionFix(bool bFixed, byte[] bInfo)//, string strTarget)
        {
            if (bFixed)
            {
                // TTFF 시 정지
                if (bInfo[4] > 0)  //GPS SVs
                {
                    STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed = bFixed;
                    STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Stop();
                }
                else
                    STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed = !bFixed;

                if (bInfo[37] > 0) //GNSS SVs
                {
                    STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed = bFixed;
                    STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Stop();
                }
                else
                    STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed = !bFixed;
                //경민 추가
                //switch (strTarget)
                //{
                //    case "GEN11":
                //        // TTFF 시 정지
                //        if (bInfo[4] > 0)  //GPS SVs
                //        {
                //            STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed = bFixed;
                //            STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Stop();
                //        }
                //        else
                //            STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed = !bFixed;

                //        if (bInfo[37] > 0) //GNSS SVs
                //        {
                //            STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed = bFixed;
                //            STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Stop();
                //        }
                //        else
                //            STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed = !bFixed;
                //    break;
                //    case "GEN12":
                //        // TTFF 시 정지
                //        if (bInfo[4] > 0)  //GPS SVs
                //        {
                //            STEPMANAGER_VALUE.stGEN12CCMGpsInfo.bPositionFixed = bFixed;
                //            STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Stop();
                //        }
                //        else
                //            STEPMANAGER_VALUE.stGEN12CCMGpsInfo.bPositionFixed = !bFixed;

                //        if (bInfo[37] > 0) //GNSS SVs
                //        {
                //            STEPMANAGER_VALUE.stGEN12CCMGnssInfo.bPositionFixed = bFixed;
                //            STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Stop();
                //        }
                //        else
                //            STEPMANAGER_VALUE.stGEN12CCMGnssInfo.bPositionFixed = !bFixed;
                //    break;
                //    default: break;
                //}
                
            }
            else
            {
                STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed = bFixed;
                STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed = bFixed;
                //경민 추가
                //switch (strTarget)
                //{
                //    case "GEN11":
                //        STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed = bFixed;
                //        STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed = bFixed;
                //    break;
                //    case "GEN12":
                //        STEPMANAGER_VALUE.stGEN12CCMGpsInfo.bPositionFixed = bFixed;
                //        STEPMANAGER_VALUE.stGEN12CCMGnssInfo.bPositionFixed = bFixed;
                //    break;
                //}
            }
        }

        private static string GetPositionStatus(bool bGpsGnss)//, string strTarget) //true : gps , false : gnss
        {
            string strResOK = "Fixed ";
            string strResNG = "No-Fix";

            if (bGpsGnss)
            {
                if (STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed)
                    return strResOK;
                else
                    return strResNG;
            }
            else
            {
                if (STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed)
                    return strResOK;
                else
                    return strResNG;
            }

            //경민 추가
            //switch (strTarget)
            //{
            //    case "GEN11":
            //        if (bGpsGnss)
            //        {
            //            if (STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed)
            //                return strResOK;
            //            else
            //                return strResNG;
            //        }
            //        else
            //        {
            //            if (STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed)
            //                return strResOK;
            //            else
            //                return strResNG;
            //        }
            //        break;
            //    case "GEN12":
            //        if (bGpsGnss)
            //        {
            //            if (STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed)
            //                return strResOK;
            //            else
            //                return strResNG;
            //        }
            //        else
            //        {
            //            if (STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed)
            //                return strResOK;
            //            else
            //                return strResNG;
            //        }
            //        break;
            //    default:
            //        return strResNG = "TARGET SET ERROR";
            //        break;
            //}
        }  

        public static void SetOldGEN10GPSInfo(byte[] bInfo)
        {
           
            if (!bGen10GpsOldInfo) return;

        
            IntPtr buff = Marshal.AllocHGlobal(bInfo.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.

            Marshal.Copy(bInfo, 0, buff, bInfo.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
            object obj = Marshal.PtrToStructure(buff, typeof(GPS_PERIODIC)); // 복사된 데이터를 구조체 객체로 변환한다.

            int iStSize = Marshal.SizeOf(typeof(GPS_PERIODIC));
            GPS_PERIODIC tmpGPSdic = (GPS_PERIODIC)obj;

            Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
            
            if (!bGen10GpsNavOn)
            {
                strGen10GpsTTFF = "0";
                bGen10GpsNavOn = true;
                dtm = DateTime.Now;
            }
            else
            {
                if (tmpGPSdic.stGpsRegular.nav_valid != 0)
                {
                    DateTime dtCurr = DateTime.Now;
                    TimeSpan tsN = dtCurr - dtm;
                    try
                    {
                        strGen10GpsTTFF = ((int)(tsN.TotalSeconds)).ToString();
                    }
                    catch { }
                    
                }
                else
                {
                    dtm = DateTime.Now;
                }
            }

            strGen10GpsNavild = tmpGPSdic.stGpsRegular.nav_valid.ToString();
            dGen10GpsLat = tmpGPSdic.stGpsRegular.lat * 0.0000001;
            dGen10GpsLon = tmpGPSdic.stGpsRegular.lon * 0.0000001;
            StringBuilder strEtcMsg = new StringBuilder(4096);
            
            strEtcMsg.Append("UTC:");
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.utc_year.ToString());
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.utc_month.ToString().PadLeft(2,'0'));
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.utc_day.ToString().PadLeft(2,'0'));
            strEtcMsg.Append("-");
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.utc_hour.ToString().PadLeft(2, '0'));
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.utc_min.ToString().PadLeft(2, '0'));
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.utc_sec.ToString("00"));
            strEtcMsg.Append(", NAV_VALID:");
            strEtcMsg.Append(tmpGPSdic.stGpsRegular.nav_valid.ToString());
            strEtcMsg.Append(", TTFF:");
            strEtcMsg.Append(strGen10GpsTTFF.ToString());
            strEtcMsg.Append(", LAT:");
            strEtcMsg.Append((tmpGPSdic.stGpsRegular.lat * 0.0000001).ToString("0.000000"));
            strEtcMsg.Append(", LON:");
            strEtcMsg.Append((tmpGPSdic.stGpsRegular.lon * 0.0000001).ToString("0.000000"));

            int iGpsCount = (int)(tmpGPSdic.nGpsSVCnt);
            int iGnssCount = (int)(tmpGPSdic.nGlonassSVCnt);


            strEtcMsg.Append(", GPS-COUNT:");
            strEtcMsg.Append(iGpsCount.ToString());

            strEtcMsg.Append(", GPS-CN0:");

            //GPS
            if (iGpsCount > 0)
            {
                iAttGpsCount = iGpsCount;
                List<int> tmpCn0 = new List<int>();
                for (int i = 0; i < iGpsCount; i++)
                {
                    tmpCn0.Add((int)tmpGPSdic.stGpsSV[i].sv_cno);
                }
                double dAver = tmpCn0.Average();
                strEtcMsg.Append(dAver.ToString("0"));
                iAttGpsCn0Aver = (int)dAver;
                iAttGpsCn0Max  = (int)tmpCn0.Max();
            }
            else
            {
                iAttGpsCount = iAttGpsCn0Aver = iAttGpsCn0Max = 0;                 
                strEtcMsg.Append("0");
            }

            //GNSS
            if (iGnssCount > 0)
            {
                iAttGnssCount = iGnssCount;
                List<int> tmp2Cn0 = new List<int>();
                for (int i = 0; i < iGnssCount; i++)
                {
                    tmp2Cn0.Add((int)tmpGPSdic.stGlonassSV[i].sv_cno);
                }
                double dAver2 = tmp2Cn0.Average();
                iAttGnssCn0Aver = (int)dAver2;
                iAttGnssCn0Max = (int)tmp2Cn0.Max();
            }
            else
            {
                iAttGnssCount = iAttGnssCn0Aver = iAttGnssCn0Max = 0;
                
            }

            InsertEtcMsgQueue(strEtcMsg);

        }

        public static void SetOldGEN9GPSInfo(byte[] OriginInfo, bool bGpsHighModel)
        {

            if (!bGen9GpsOldInfo) return;

            //InitstGEN9GPSInfo();
              

            //1R6 찾아보자
            /*
            for (int x = 0; x < OriginInfo.Length - 10; x++)
            {
                if (OriginInfo[x].Equals(0x02) && (OriginInfo[x + 1].Equals(0xFB) || OriginInfo[x + 1].Equals(0xFA))
                    && OriginInfo[x + 6].Equals(0x31) && OriginInfo[x + 7].Equals(0x52) && OriginInfo[x + 8].Equals(0x36) && OriginInfo[x + 10].Equals(0x66))
                {
                    byte[] gpsBuffer = new byte[OriginInfo.Length];

                    Array.Copy(OriginInfo, x, gpsBuffer, 0, OriginInfo.Length - (x + 1));
                    break;
                }
            }
            */ 
         
            byte[] bInfo = new byte[OriginInfo.Length-1];

            Array.Copy(OriginInfo, 1, bInfo, 0, bInfo.Length);

            UpdatestGEN9GPSInfo(bInfo);

            GEN9_SV[] svInfo = new GEN9_SV[0];

            /*
            IntPtr buff = Marshal.AllocHGlobal(bInfo.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.

            Marshal.Copy(bInfo, 0, buff, bInfo.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
            object obj = Marshal.PtrToStructure(buff, typeof(GEN9_GPS_PERIODIC)); // 복사된 데이터를 구조체 객체로 변환한다.

            int iStSize = Marshal.SizeOf(typeof(GEN9_GPS_PERIODIC));
            GEN9_GPS_PERIODIC tmpGPSdic = (GEN9_GPS_PERIODIC)obj; 

            Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
           */
            string stryear  = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.utc_year);
            string strmonth = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.utc_month);
            string strday   = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.utc_day);

            string strhour  = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.utc_hour);
            string strmin   = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.utc_min);
            string strsec   = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.utc_sec);

            string strsv_used_cnt = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.sv_used_cnt);
            string strlon = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.lon);
            string strlat = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.lat);
            

            string strnav_valid = STEPMANAGER_VALUE.GetGen9Gps((int)GEN9_GPS_IDX.nav_valid);

            strGen9GpsNavild = strnav_valid;

            bool bCheckSVs = false;
            int remainBytes = 0;
            bool bUpdate = false;
            try
            {
                remainBytes = (bInfo.Length - 80) / 32;
                //if (remainBytes.Equals(OriginInfo[0]))                
                {
                    
                    //svInfo = new GEN9_SV[remainBytes];
                    //for (int i = 0; i < remainBytes; i++)

                    svInfo = new GEN9_SV[OriginInfo[0]];
                    for (int i = 0; i < OriginInfo[0]; i++)
                    {
                        STEPMANAGER_VALUE.InitGen9SVstructure(ref svInfo[i]);
                    }
                    bCheckSVs = true;
                }

            }
            catch
            {
                remainBytes = -1;
                bCheckSVs = false;
            }

            if (bCheckSVs && OriginInfo[0]/*remainBytes*/ > 1)
            {
                byte[] byteSVs = new byte[(bInfo.Length - 80)];
                
                int iOffset = 0;
                if (bGpsHighModel) iOffset = 112;

                try
                {
                    Array.Copy(bInfo, 80 + iOffset, byteSVs, 0, byteSVs.Length - iOffset);
                    string strTempString = BitConverter.ToString(byteSVs, 0, byteSVs.Length).Replace("-", " ");
                    for (int i = 0; i < OriginInfo[0]; i++)
                    {
                        bUpdate = STEPMANAGER_VALUE.UpdateGen9Svstructure(i, ref svInfo[i], byteSVs, bGpsHighModel);
                        if (!bUpdate) break;
                    }
                }
                catch
                {
                    bCheckSVs = false;
                }
                
            }

            if (!bGen9GpsNavOn)
            {
                strGen9GpsTTFF = "0";
                bGen9GpsNavOn = true;
                dtm = DateTime.Now;
            }
            else
            {
                if (!strnav_valid.Equals("0"))
                {                   

                    DateTime dtCurr = DateTime.Now;
                    TimeSpan tsN = dtCurr - dtm;
                    try
                    {
                        strGen9GpsTTFF = ((int)(tsN.TotalSeconds)).ToString();
                    }
                    catch { }

                }
                else
                {
                    dtm = DateTime.Now;
                }
            }
            /*
            strGen9GpsNavild = tmpGPSdic.stGpsRegular.nav_valid.ToString();
            dGen9GpsLat = tmpGPSdic.stGpsRegular.lat * 0.0000001;
            dGen9GpsLon = tmpGPSdic.stGpsRegular.lon * 0.0000001;
             * */

            StringBuilder strEtcMsg = new StringBuilder(4096);

            strEtcMsg.Append("UTC:");
            strEtcMsg.Append(stryear);
            strEtcMsg.Append(strmonth.PadLeft(2, '0'));
            strEtcMsg.Append(strday.ToString().PadLeft(2, '0'));
            strEtcMsg.Append("-");
            strEtcMsg.Append(strhour.ToString().PadLeft(2, '0'));
            strEtcMsg.Append(strmin.ToString().PadLeft(2, '0'));
            strEtcMsg.Append(strsec.ToString().PadLeft(5, '0'));
            strEtcMsg.Append(", NAV_VALID:");
            strEtcMsg.Append(strnav_valid.ToString());
            strEtcMsg.Append(", TTFF:");
            strEtcMsg.Append(strGen9GpsTTFF.ToString());
            strEtcMsg.Append(", LAT:");
            strEtcMsg.Append(strlat);
            strEtcMsg.Append(", LON:");
            strEtcMsg.Append(strlon);

            try
            {
                dGen9GpsLat = double.Parse(strlat);
                dGen9GpsLon = double.Parse(strlon);
            }
            catch 
            {
                dGen9GpsLat = 0;
                dGen9GpsLon = 0;
            }
            

            int iGpsCount = (int)OriginInfo[0]; //(tmpGPSdic.nGpsSVCnt);
            
            strEtcMsg.Append(", SV:");
            strEtcMsg.Append(iGpsCount.ToString());

            iGen9GpsCount = iGpsCount;

            //if(bCheckSVs && bUpdate)  //일단 그냥 취합해보자. gen9 는 다 안올라온다.
            {
                strEtcMsg.Append(", CN0-AVER:");
                //GPS
                if (svInfo.Length > 0)
                {
                    //iGen9GpsCount = iGpsCount;
                    List<int> tmpCn0 = new List<int>();
                    for (int i = 0; i < svInfo.Length; i++)
                    { 
                        tmpCn0.Add((int)svInfo[i].sv_cno[0]);
                    }
                    double dAver = tmpCn0.Average();
                    strEtcMsg.Append(dAver.ToString("0"));
                                        
                    iGen9GpsCn0Aver = (int)dAver;
                    iGen9GpsCn0Max = (int)tmpCn0.Max();

                    strEtcMsg.Append(", CN0-MAX:");
                    strEtcMsg.Append(iGen9GpsCn0Max.ToString());
                }
                else
                {
                    iGen9GpsCount = iGen9GpsCn0Aver = iGen9GpsCn0Max = 0;
                    strEtcMsg.Append("0");
                }

            }
           
           
            InsertEtcMsgQueue(strEtcMsg);
            
        }

        public static int IsExistBinMsgQueue()
        {
            return BinLogQueue.Count;
        }

        public static int IsExistEtcMsgQueue()
        {
            return EtcLogQueue.Count;
        }

        public static void ClearBinMsgQueue()
        {
            try
            {
                BinLogQueue.Clear();
            }
            catch {}
        }

        public static void ClearEtcMsgQueue()
        {
            try
            {
                EtcLogQueue.Clear();
            }
            catch { }
        }

        public static void InsertBinMsgQueue(BinMsg strBm)
        {
            lock (lockBinIn)
            {
                BinLogQueue.Enqueue(strBm);
            }
        }

        public static void InsertEtcMsgQueue(StringBuilder strBinLog)
        {
            lock (lockEtcIn)
            {
                EtcLogQueue.Enqueue(strBinLog);
            }
        }

        public static bool ExcuteBinMsgQueue(ref StringBuilder strBinLog, ref int iFrom)
        {
            if (IsExistBinMsgQueue() > 0)
            {
                BinMsg tmpBm = new BinMsg();                

                lock (lockBinOut)
                {
                    tmpBm = BinLogQueue.Dequeue();
                    strBinLog = tmpBm.sb;
                    iFrom = tmpBm.iFrom;
                }
                return true;
            }
            return false;
            
        }

        public static bool ExcuteEtcMsgQueue(ref StringBuilder strEtcLog)
        {
            if (IsExistEtcMsgQueue() > 0)
            {
                lock (lockEtcOut)
                {
                    strEtcLog = EtcLogQueue.Dequeue();
                }
                return true;
            }
            return false;

        }

        public static void OOBSimInfoClear()
        {
            STEPMANAGER_VALUE.OOBSimInfo     = new string[(int)SimInfoIndex.END];
            
            for (int i = (int)SimInfoIndex.efUST; i < (int)SimInfoIndex.END; i++)
            {
                STEPMANAGER_VALUE.OOBSimInfo[i] = String.Empty;
            }

            bOldBinaryALDL = new byte[1];            
        }

        public static void OOBServiceClear()
        {
            STEPMANAGER_VALUE.OOBServiceInfoA = new string[(int)ServiceIndexA.END];
            STEPMANAGER_VALUE.OOBServiceInfoB = new string[(int)ServiceIndexB.END];

            for (int i = 0; i < OOBServiceInfoA.Length; i++)
            {
                STEPMANAGER_VALUE.OOBServiceInfoA[i] = String.Empty;
            }

            for (int i = 0; i < OOBServiceInfoB.Length; i++)
            {
                STEPMANAGER_VALUE.OOBServiceInfoB[i] = String.Empty;
            }
        }

        public static void CreateSecCount()
        {
            //비밀병기를 위한 난수 생성 ㅋㅋ
            Random rnd = new Random();
            rnd.Next();
            iClickCount1 = 0;
            iClickCount2 = 0;
            iSecCount1 = rnd.Next(1, 9);
            iSecCount2 = rnd.Next(1, 9);
        }
        
        public static bool CheckSecCount()
        {            
            if (iClickCount1 == iSecCount1 && iClickCount2 == iSecCount2) return true;
            return false;
        }

        public static void AddSecount1() { iClickCount1++; }
        public static void AddSecount2() { iClickCount2++; }
        public static int GetSecCount1(){ return iSecCount1;}
        public static int GetSecCount2(){ return iSecCount2;}

        private static PWUSER[] UserCert = new PWUSER[(int)USERLIMIT.MAX];

        public static void SetUserInformation(PWUSER[] testUser)
        {
            for (int i = 0; i < UserCert.Length; i++)
            {
                UserCert[i] = testUser[i];
            }
        }

        public static bool GetUserInformation(string strPW, ref PWUSER tempUser)
        {
            for (int i = 0; i < UserCert.Length; i++)
            {
                if (!String.IsNullOrEmpty(UserCert[i].strLogName))
                {
                    if (UserCert[i].strPassword.Equals(strPW))
                    {
                        tempUser.strLogName = UserCert[i].strLogName;
                        tempUser.bEdit = UserCert[i].bEdit;
                        tempUser.bJob = UserCert[i].bJob;
                        tempUser.bConfig = UserCert[i].bConfig;
                        tempUser.bMes = UserCert[i].bMes;
                        //LGEVH
                        tempUser.bAutoJobConfig = UserCert[i].bAutoJobConfig;
                        return true;
                    }
                }
            }
            return false;
        }
    }

    class DK_STEPMANAGER
    {

#region 변수 선언 및 초기화
        
        public event EventThreadStatus ManagerSendThredEvent;
        public event EventMANAGER      ManagerSendReport;          //MAIN Form 으로 날릴 이벤트 메소드
        public event EventRealTimeMsg  ManagerSendReport2;         //MAIN Form 으로 날릴 이벤트 메소드        
        public event EventDeviceStatus ManagerSendReport3;         //MAIN Form 으로 날릴 이벤트 메소드 
        public event EventSetJumpResult EvSetJumpResult;
        public event EventGotoTestGridRow EvGotoTestGridRow;

        private const int    iStatusTime = 20; //시리얼 동작상태 타임 ms
            
        private IntPtr gMesHandle;
        private const Int32  WM_GMESDATA = 0x400 + 2000;
        private const string dev5515C     = "5515C";
        private const string dev34410a    = "34410A";        
        private const string devMTP200    = "MTP200";
        private const string devPCAN      = "PCAN";
        private const string devVector    = "VECTOR";
        private const string devTC3000    = "TC3000";
        private const string devAudio     = "AUDIOSELECTOR";
        private const string devADC       = "ADC";
        private const string devODAPWR    = "ODAPWR";
        private const string devKEITHLEY  = "KEITHLEY";
        private const string devDLLGate   = "DLLGATE";

        private const string devStepCheckMode = "STEPCHECKMODE";
        private const string devMELSEC = "MELSEC";
        private const string devTC1400A = "TC1400A";
        private const string devMTP120A = "MTP120A";

        private const string strDevCon32 = "devcon";   //32비트용
        private const string strDevCon64 = "devcon64"; //64비트용

        private const string DEFINEGMES = "#GMES_";
        private const string DEFINEEXPR = "#EXPR_";
        private const string DEFINEDOCU = "#DOC_";

        private DK_GMES DKGMES;
        private DK_KLAS DKKALS;
        private DK_LOGGER DKLoggerPC;
        private DK_LOGGER DKLoggerMR = new DK_LOGGER("SET", false);
        private DK_ACTOR DKACTOR;
        private DK_OOB DKOOB;
        private DK_ORACLE DKORACLE;
        private DK_VECTOR_BASIC DKVECTOR = new DK_VECTOR_BASIC();
        //CSMES
        public static DK_OSI_FOR_LGE DKOSIMES;
        //private object lockobject = new object();

        private DK_EXPR[] DKExpr = new DK_EXPR[(int)DEFINES.END]; //EXPR 계산용 (슬롯별)
        private Dictionary<int, bool> PSENSOR_DIC = new Dictionary<int, bool>();                   //센서 세팅값
        private Dictionary<int, int> RSENSOR_DIC = new Dictionary<int, int>();                    //실제 센서값
        private Dictionary<int, int>[] SEQUENCE_DIC = new Dictionary<int, int>[(int)DEFINES.END];    //JOB 수행 시퀀스 배열
        private Dictionary<int, int> RETCOUNT_DIC = new Dictionary<int, int>();                    //시퀀스 리트라이(명령단위)
        private Dictionary<int, string> AVERAGE_DIC = new Dictionary<int, string>();                 //결과 평균용
        private bool[]                           STPCHECK_DIC = new bool[(int)DEFINES.END];                    //STEPCHECK수행 결과 배열
        private Dictionary<int, string>[] RESULTDT_DIC = new Dictionary<int, string>[(int)DEFINES.END]; //수행 결과 배열 
        private Dictionary<int, string>[] RESPONSE_DIC = new Dictionary<int, string>[(int)DEFINES.END]; //수행 결과 배열 
        private Dictionary<int, string>[] SENDPACK_DIC = new Dictionary<int, string>[(int)DEFINES.END]; //보낸 명령 배열 

        private List<RESDATA>[] LstTST_RES = new List<RESDATA>[(int)DEFINES.END];    //슬롯별 모든 결과 값 저장 리스트
        private List<JOBDATA> LstJOB_CMD = new List<JOBDATA>();                      //JOB    리스트
        private List<TBLDATA0> LstTBL_GEN9  = new List<TBLDATA0>();                  //GEN9   명령 테이블 리스트//GEN9        
        private List<TBLDATA0> LstTBL_GEN10 = new List<TBLDATA0>();                  //GEN10  명령 테이블 리스트//GEN10        
        private List<TBLDATA0> LstTBL_GEN11 = new List<TBLDATA0>();                  //GEN11  명령 테이블 리스트
        private List<TBLDATA0> LstTBL_GEN11P = new List<TBLDATA0>();                 //GEN11  출하 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_GEN12 = new List<TBLDATA0>();                  //GEN12  명령 테이블 리스트
        private List<TBLDATA0> LstTBL_CCM = new List<TBLDATA0>();                    //CCM    명령 테이블 리스트
        private List<TBLDATA0> LstTBL_NAD = new List<TBLDATA0>();                    //NAD    명령 테이블 리스트
        private List<TBLDATA0> LstTBL_MCTM = new List<TBLDATA0>();                   //MCTM   명령 테이블 리스트
        private List<TBLDATA0> LstTBL_TCP = new List<TBLDATA0>();                    //TCP    명령 테이블 리스트
        private List<TBLDATA0> LstTBL_ATT = new List<TBLDATA0>();                    //ATT    명령 테이블 리스트
        private List<TBLDATA0> LstTBL_DIO = new List<TBLDATA0>();                    //DIO    명령 테이블 리스트
        private List<TBLDATA0> LstTBL_5515c = new List<TBLDATA0>();                  //5515c  명령 테이블 리스트
        private List<TBLDATA0> LstTBL_MTP200 = new List<TBLDATA0>();                 //MTP200 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_PCAN = new List<TBLDATA0>();                   //PCAN   명령 테이블 리스트
        private List<TBLDATA0> LstTBL_VECTOR = new List<TBLDATA0>();                 //VECTOR 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_TC3000 = new List<TBLDATA0>();                 //TC3000 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_TC1400A = new List<TBLDATA0>();                 //TC1400A 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_AUDIO = new List<TBLDATA0>();                  //DIO AUDIO 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_ADC = new List<TBLDATA0>();                    //DIO ADC  명령 테이블 리스트
        private List<string[]> LstTBL_JOBMAP = new List<string[]>();                 //JOB 파일 오토로딩 맵 리스트
        private List<TBLDATA0> LstTBL_34410A = new List<TBLDATA0>();                 //34410 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_PWR = new List<TBLDATA0>();                    //PWR    명령 테이블 리스트
        private List<TBLDATA0> LstTBL_KEITHLEY = new List<TBLDATA0>();               //KEITHLEY  명령 테이블 리스트

        private List<TBLDATA0> LstTBL_DLLGATE = new List<TBLDATA0>();                //GEN9DLL 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_MELSEC  = new List<TBLDATA0>();                //MELSEC 명령 테이블 리스트
        private List<TBLDATA0> LstTBL_MTP120A = new List<TBLDATA0>();

        private List<InspDoc>  LstDoc        = new List<InspDoc>();

        private List<TBLMODEL> LstTBL_Model = new List<TBLMODEL>(); //GEN10 구형에서만 쓰는 모델파일 리스트
        private List<string> LstLastNG = new List<string>(); //검사 NG 리스트 (연속불량리스트)

        public  bool[] UseSlots = new bool[(int)DEFINES.END];                 //슬롯 사용, 미사용 리스트
        private bool OpenCCM = false;                                  //슬롯 사용, 미사용 리스트
        private bool OpenDIO = false;                                  //슬롯 사용, 미사용 리스트
        private bool OpenSET = false;                                  //슬롯 사용, 미사용 리스트
        private bool OpenTC3000 = false;                               //슬롯 사용, 미사용 리스트
        private bool OpenSCAN = false;                                 //슬롯 사용, 미사용 리스트
        private bool OpenODAPWR = false;                               //슬롯 사용, 미사용 리스트

        private int Open5515C = (int)STATUS.NOTUSE;                    //슬롯 사용, 미사용 리스트        
        private int OpenMTP200 = (int)STATUS.NOTUSE;                   //슬롯 사용, 미사용 리스트
        private int OpenPCAN = (int)STATUS.NOTUSE;                     //슬롯 사용, 미사용 리스트
        private int OpenVector = (int)STATUS.NOTUSE;                   //슬롯 사용, 미사용 리스트
        private int OpenAudio = (int)STATUS.NOTUSE;                    //슬롯 사용, 미사용 리스트
        private int OpenADC = (int)STATUS.NOTUSE;                      //슬롯 사용, 미사용 리스트
        private int Open34410A = (int)STATUS.NOTUSE;                   //슬롯 사용, 미사용 리스트
        private int OpenKEITHLEY = (int)STATUS.NOTUSE;                 //슬롯 사용, 미사용 리스트        
        private int OpenDLLGate = (int)STATUS.NOTUSE;                  //슬롯 사용, 미사용 리스트  
        private int OpenMELSEC = (int)STATUS.NOTUSE;                   //슬롯 사용, 미사용 리스트
        private int OpenMTP120A = (int)STATUS.NOTUSE;                //슬롯 사용, 미사용 리스트

        private string strWIPID = String.Empty;
        private string strSUBID = String.Empty;
        private string strOOBLABEL = String.Empty;

        private bool bOOBLableMode = false;
        private uint FixNadPort = 0;

        private bool bUsePLCMode = false;                            //PLC 모드 사용여부

        private DEVICEDATA[] deviceStatus = new DEVICEDATA[(int)DEVSTATUSNUMBER.END];
                
        private Thread threadExcelComm; //EXCEL COMMAND 용        
        private Thread threadKisKeyDLL; //KIS DLL 용
        private Thread threadAmsKeyDLL; //AMS 호출용
        private Thread threadEngine;    //절차서 엔진용
        private bool bEngineOn = false;
        private System.Threading.Timer StatusTimer;                          //actor 상태 타이머

        private bool bTestStarted;                                         //테스트 시작여부
        //private bool bInteractiveMode;                                     //인터랙티브모드 
        private int iNowJobNumber = 0;                                     //절차서 순번
        private bool bOnlyOKGmes = false;           //테스트결과가 OK 일때만 GMES STEP COMPLETE 하는 옵션
        private string strNowJobName = String.Empty;
        private bool[] bCmdDoneCheck = new bool[(int)DEFINES.END];   //한줄단위 절차 체크(각 슬롯이 각 명령을 완료했는가?)

        public  bool[] bUsedEndOption = new bool[(int)ENDOPTION.END];
        public  string[][] strEndOptionCommand = new string[(int)ENDOPTION.END][];

        private bool bThisPop = false;
        private DK_NI_GPIB      DK_GPIB_5515C;
        private DK_NI_GPIB      DK_GPIB_KEITHLEY;         
        private DK_NI_GPIB      DK_GPIB_MTP200;
        private DK_NI_VISA      DK_GPIB_34410A;        
        private DK_PAGE         tmpDKPage         = new DK_PAGE();
        private DK_PCAN         DK_PCAN_USB       = new DK_PCAN();        
        private DK_NADKEYDLL    DK_NADKEY         = new DK_NADKEYDLL(); //동적로딩
        private DK_ETHERNET_GEN9DLL DK_DLLGATE    = new DK_ETHERNET_GEN9DLL("DLL_GATE");
        private DK_ETHERNET_TC1400A DK_TC1400A    = new DK_ETHERNET_TC1400A("TC1400A");
        private DK_MELSEC_ETHERNET DK_MELSEC;
        private DK_NI_VISA DK_VISA_MTP120A;     //SOCKET

        private List<string> LstHiddenCommand = new List<string>();         //buyer 요구사항의 의한 로깅히든명령어 리스트

        private object lockObject = new object(); //EXPR 에 KIS 데이터를 라이팅할때 쓸목적.
        private object lockObjectDic = new object(); //Dic 에 데이터를 업데이트할때 쓸목적.

        private int iWIPSIZE = 15;
        private int iSUBIDSIZE = 12; //지그 바코드 format용도는 YYYYMMDDNNNN 입니다.

        private bool bUseBarcode = false;
        private bool bUseSubId = false;

        //until 용
        private DateTime udtOutSet;
        private DateTime udtCurrTime;
        
        //gpib, nad 용
        private DateTime dtStepDelaySet;
        private DateTime dtStepCurrTime;        
        
        //해당명령 걸린시간 측정용
        private System.Diagnostics.Stopwatch swLapse;
        //절차서 타이머 명령용
        private System.Diagnostics.Stopwatch swTimer;

        private System.Diagnostics.Stopwatch PageDelayStopWatch = new System.Diagnostics.Stopwatch();

        private double dStepTimeSet;

        //출하검사용
        private ExcelData[] exData = new ExcelData[50];
        
        //ORACLE 용
        private const string sOraProductionLine = "PRODUCTION_LINE";
        private const string sOraProcessCode = "PROCESS_CODE";
        private const string sOraPCID = "PC_ID";
        private const string sOraServerIP = "SERVER_IP";
        private const string sOraPort = "PORT";
        private const string sOraServiceName = "SERVICE_NAME";
        private const string sOraOOBCode = "OOB_CODE";
        private const string sOraOOBFlag = "OOB_FLAG";
        private const string sOraCallType = "CALLTYPE";

        private const string sOraDonCareOOBCODE = "DONTCAREOOBCODE";

        private const string constUseInspectionRetry = "INSPECTIONRETRY";
        private const string constUseInspectionRetryCount = "RETRYVALUE";
        private int iInspectionRetryCount = 0;

        private double dExtPwrBootTime = -1;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 부팅딜레이를 위한 변수

        private bool bExtOnPrimary   = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
        private bool bExtOnKey       = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
        private bool bExtOnBackup    = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
        private bool bExtOffPrimary  = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
        private bool bExtOffKey      = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
        private bool bExtOffBackup   = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수

        private string[] strArray44Blocks   = new string[6]; //GEN11 용 44블럭 값. (Secure Logging 해제시 사용할데이터)

        private _NAMMgmnt tmpMDN = new _NAMMgmnt(); //Gen9 MDN write 용 

        private double Item_dStepTimeSet
        {
            get { return dStepTimeSet; }
            set { dStepTimeSet = value; }
        }
        
        public bool Item_bUseBarcode
        {
            get { return bUseBarcode; }
            set { bUseBarcode = value; }
        }

        public bool Item_bUseSubId
        {
            get { return bUseSubId; }
            set { bUseSubId = value; }
        }

        public int Item_iWIPSIZE
        {
            get { return iWIPSIZE; }
            set { iWIPSIZE = value; }
        }

        public int Item_iSUBIDSIZE
        {
            get { return iSUBIDSIZE; }
            set { iSUBIDSIZE = value; }
        }
        
        public string Item_WIPID
        {
            get { return strWIPID; }
            set { strWIPID = value; }
        }

        public string Item_SUBID
        {
            get { return strSUBID; }
            set { strSUBID = value; }
        }

        public string Item_OOBLABEL
        {
            get { return strOOBLABEL; }
            set { strOOBLABEL = value; }
        }


        private bool Item_bThisPop
        {
            get { return bThisPop; }
            set { bThisPop = value; }
        }

        private IntPtr Item_gMesHandle
        {
            get { return gMesHandle; }
            set { gMesHandle = value; }
        }

        public bool Item_bTestStarted
        {
            get { return bTestStarted; }
            set {
                    STEPMANAGER_VALUE.bProgramRun = value;
                    bTestStarted = value;
                    if(bTestStarted) Item_bInteractiveMode = false;
                }
        }
               
        public bool Item_bInteractiveMode
        {
            get { return STEPMANAGER_VALUE.bInteractiveMode; }
            set
            {
                if (value)
                {
                    if (!Item_bTestStarted)
                    {
                        STEPMANAGER_VALUE.bInteractiveMode = true;
                    }
                    else
                    {
                        STEPMANAGER_VALUE.bInteractiveMode = false;
                    }
                }
                else
                {
                    STEPMANAGER_VALUE.bInteractiveMode = false;
                }
            }
        }

#endregion

        public DK_STEPMANAGER(IntPtr WindowHandle) 
        {
            Item_gMesHandle = WindowHandle;
            Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
            initialize();
        }

        public void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);                
            }
        }
        
        public void SetPlcMode(bool bUsed)
        {
            bUsePLCMode = bUsed;           
        }

        public bool GetPlcMode()
        {
            return bUsePLCMode;
        }

        private int PcanConnection()
        {
            //
            DEVICEDATA devData = new DEVICEDATA();

            devData.iDevNumber = (int)DEVSTATUSNUMBER.PCAN;
            if (!ConfigLoad("OPTION", devPCAN))
            {
                devData.iStatus = (int)STATUS.NOTUSE;

            }
            else
            {
                DeviceControlUSB("enable");

                string strRtnMsg = String.Empty;
                if (DK_PCAN_USB.Initialize(ref strRtnMsg, 2))
                {
                    devData.iStatus = (int)STATUS.OK;
                    //DK_PCAN_USB.Release(); 
                }
                else
                {
                    devData.iStatus = (int)STATUS.NG;
                }
            }
            
            SendManagerSendReport3(devData);
            return devData.iStatus;
        }

        private int CheckDLLGateConnection()
        {
            KillDLLGate();

            if (!ConfigLoad("OPTION", devDLLGate))
            {
                return (int)STATUS.NOTUSE;
            }
            
            try
            {
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lib\GEN9\DLL Gate.exe")) //파일이 있으면 //Item_DataFolder
                {
                    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\lib\GEN9\DLL Gate.exe");
                }
                System.Threading.Thread.Sleep(200);
                Application.DoEvents();
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
                {
                    if (process.ProcessName.Contains("DLL Gate"))
                    {
                        return (int)STATUS.OK;                            
                    }
                }
            }
            catch
            {
                return (int)STATUS.NG;
            }
            return (int)STATUS.NG;     
        }

        public void KillDLLGate()
        {
            try
            {
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
                {
                    if (process.ProcessName.Contains("DLL Gate"))
                    {
                        process.Kill();
                    }
                }
            }
            catch { }
        }

        private int IsVectorUse()
        {
            DEVICEDATA devData = new DEVICEDATA();

            devData.iDevNumber = (int)DEVSTATUSNUMBER.VECTOR;
            if (!ConfigLoad("OPTION", devVector))
            {
                devData.iStatus = (int)STATUS.NOTUSE;
            }
            else
            {
                string strRtnMsg = String.Empty;
                if (DKVECTOR.Initialize(ref strRtnMsg, (uint)VectorBaudrate.BAUD_500K))
                {
                    devData.iStatus = (int)STATUS.OK;                    
                }
                else
                {
                    devData.iStatus = (int)STATUS.NG;
                }
            }

            SendManagerSendReport3(devData);
            return devData.iStatus;

        }
        
        public bool CheckDevice(ref string strReason)
        {            
            strReason = "Device Check : OK";
            // load DIO PIN status
            LoadDioPinStatus();
            // Make PassWord
            //LGEVH 20230816
            //string strPassword = DKLoggerPC.LoadINI("OPTION", "PASSWORD");
            //if (strPassword.Equals("0"))
            //{
            //    DKLoggerPC.SaveINI("OPTION", "PASSWORD", "master");
            //}
            string strPassword = DKLoggerPC.LoadPWINI("OPTION", "PASSWORD");
            if (strPassword.Equals("0"))
            {
                DKLoggerPC.SavePWINI("OPTION", "PASSWORD", "master");
            }

            string strOOBLabelMode = DKLoggerPC.LoadINI("OPTION", "OOBBARCODETYPE");
            bOOBLableMode = strOOBLabelMode.Equals("ON");
            
            // COMPORT LOADING & CONNECTION   

            SetPlcMode(ConfigLoad("OPTION", "USEPLC")); //config 에 저장된 plc 사용여부 값 저장.
            
            SerialAllconnection();            
            
            // GPIB 
            Open5515C  = GpibConnection(ref DK_GPIB_5515C,  (int)DEVSTATUSNUMBER.E5515C, dev5515C);            
            OpenMTP200 = GpibConnection(ref DK_GPIB_MTP200, (int)DEVSTATUSNUMBER.MTP200, devMTP200);
            OpenKEITHLEY = GpibConnection(ref DK_GPIB_KEITHLEY, (int)DEVSTATUSNUMBER.KEITHLEY, devKEITHLEY);            
            Open34410A = VisaConnection(ref DK_GPIB_34410A, (int)DEVSTATUSNUMBER.DMM34410A, dev34410a); //34401a는 USB-VISA 방식.

            //TCP SOCKET
            OpenMTP120A = VisaConnection(ref DK_VISA_MTP120A, (int)DEVSTATUSNUMBER.MTP120A, devMTP120A); //SOCKET-VISA 방식.

            // MELSEC - ETHERNET
            OpenMELSEC = MelsecConnection();

            //PCAN
            OpenPCAN = PcanConnection();

            //Vector
            OpenVector = IsVectorUse();

            //
            OpenDLLGate = CheckDLLGateConnection();
            
            //GMES CONFIGRATION LOADING
            GmesConnection();

            string strGetWipSize = DKLoggerPC.LoadINI("OPTION", "WIPLENTH");
            Item_iWIPSIZE = int.Parse(strGetWipSize);

            string strGetSubIdSize = DKLoggerPC.LoadINI("OPTION", "SUBIDLENTH");
            Item_iSUBIDSIZE = int.Parse(strGetSubIdSize);

            if (ConfigLoad("OPTION", devStepCheckMode))
            {
                OpenDIO = true;
                //OpenSET = true;
                UseSlots[1] = true;
                //OpenDevelopMode = true;
                DEVICEDATA DevData = new DEVICEDATA();
                DevData.iDevNumber = (int)DEVSTATUSNUMBER.DIO;
                DevData.iStatus = (int)STATUS.NOTUSE;
                SendManagerSendReport3(DevData);
            }            

            try
            {
                string strNadPort = DKLoggerPC.LoadINI("COMPORT", "NAD");
                FixNadPort = uint.Parse(strNadPort);
            }
            catch
            {
                strReason = "Device Check : NG (NAD)";
                return false;
            }

            if (!OpenDIO || !OpenSET)
            {
                strReason = "Device Check : NG (DIO, SET)";
                return false;               
            }

            StatusTimer.Change(0, iStatusTime);          
            return true;
        }

        private void SendManagerSendReport3(DEVICEDATA devData)
        {
            if (devData.iDevNumber >= 0 && devData.iDevNumber < deviceStatus.Length)
            {
                deviceStatus[devData.iDevNumber].iStatus = devData.iStatus;
            }
            
            ManagerSendReport3(devData);
        }

        private bool ConfigLoad(string strTitle, string strName)
        {
            string strGetText = DKLoggerPC.LoadINI(strTitle, strName);
            if (strGetText.Equals("ON"))
            { return true; }
            else { return false; }
        }

        public void SerialAllconnection()
        {
            for (int i = (int)COMSERIAL.DIO; i < (int)COMSERIAL.END; i++)
            {
                switch (i)
                {
                    case (int)COMSERIAL.CCM: break;
                    default: ConnectionPorts(i); break;
                }                
            }
        }

        private int MelsecConnection()
        {
            DEVICEDATA devData = new DEVICEDATA();
            devData.iDevNumber = (int)DEVSTATUSNUMBER.MELSEC;
            string strResponseData = String.Empty;
            if (!ConfigLoad("OPTION", "MELSEC"))
            {
                devData.iStatus = (int)STATUS.NOTUSE;
                SendManagerSendReport3(devData);
                return devData.iStatus;
            }

            bool bConn = false;

            if (DK_MELSEC == null)
            {
                DK_MELSEC = new DK_MELSEC_ETHERNET();
                DK_MELSEC.MelsecRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);
                bConn = DK_MELSEC.Connect(ref strResponseData);
            }
            else
            {
                DK_MELSEC.Disconnect();
                bConn = DK_MELSEC.Connect(ref strResponseData);
            }

            if (bConn)
                devData.iStatus = (int)STATUS.OK;
            else
                devData.iStatus = (int)STATUS.NG;

            SendManagerSendReport3(devData);
            return devData.iStatus;

        }

        private int GpibConnection(ref DK_NI_GPIB targetGpib, int itargetNumber, string strtagetName) // targetnumber 2 = 5515c
        {            
            DEVICEDATA devData = new DEVICEDATA();
            targetGpib = null;            
            int iBd = 0;
            int iAddr = 0;
            devData.iDevNumber = itargetNumber;
            if (!ConfigLoad("OPTION", strtagetName))
            {
                devData.iStatus = (int)STATUS.NOTUSE;
                SendManagerSendReport3(devData);
                return devData.iStatus;
            }
            string strAddress = DKLoggerPC.LoadINI("COMPORT", strtagetName);
            
            if (strAddress != null && strAddress.Length > 12)
            {                
                bool bOk = SplitAddress(strAddress, ref iBd, ref iAddr);
                if (bOk)
                {
                    targetGpib = new DK_NI_GPIB((int)DEFINES.SET1, strtagetName);
                    targetGpib.GPIBRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);
                    bool bConn = targetGpib.Connect(iBd, iAddr, strAddress); //VISA 표준방식
                    //bool bConn = targetGpib.Connect(iBd, iAddr); //NI 전용방식
                    if (bConn) devData.iStatus = (int)STATUS.OK;
                    
                    else
                    {
                        devData.iStatus = (int)STATUS.NG;
                        targetGpib = null;
                    }

                }
                else { devData.iStatus = (int)STATUS.NG; }
                SendManagerSendReport3(devData);
            }
            else
            {
                devData.iStatus = (int)STATUS.NOTUSE;
                SendManagerSendReport3(devData);
            }

            return devData.iStatus;
        }
  
        private bool SplitAddress(string strOriginAddress, ref int iBdNum, ref int iAddr)
        {
            if (strOriginAddress.IndexOf("GPIB") == 0)
            {
                strOriginAddress = strOriginAddress.Replace("GPIB", String.Empty);
                string[] tmpString = System.Text.RegularExpressions.Regex.Split(strOriginAddress, "::");
                if (tmpString.Length == 3)
                {
                    try
                    {
                        iBdNum = int.Parse(tmpString[0]);
                        iAddr  = int.Parse(tmpString[1]);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                        STEPMANAGER_VALUE.DebugView(strExMsg);
                        return false;
                    }
                    
                }
                return false;
            }
            return false;
        }
        
        private int VisaConnection(ref DK_NI_VISA targetDev, int itargetNumber, string strtagetName)
        {

            DEVICEDATA devData = new DEVICEDATA();
            targetDev = null;
            devData.iDevNumber = itargetNumber;
            if (!ConfigLoad("OPTION", strtagetName))
            {
                devData.iStatus = (int)STATUS.NOTUSE;
                SendManagerSendReport3(devData);
                return devData.iStatus;
            }
            string strAddress = DKLoggerPC.LoadINI("COMPORT", strtagetName);

            if (strAddress != null && strAddress.Length > 12)
            {
                targetDev = new DK_NI_VISA((int)DEFINES.SET1, strtagetName);
                targetDev.VisaRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);
                bool bConn = targetDev.Connect(strAddress);
                if (bConn) devData.iStatus = (int)STATUS.OK;

                else
                {
                    devData.iStatus = (int)STATUS.NG;
                    targetDev = null;
                }

                SendManagerSendReport3(devData);
            }
            else
            {
                devData.iStatus = (int)STATUS.NOTUSE;
                SendManagerSendReport3(devData);
            }

            return devData.iStatus;
        }

        private void initialize()
        {
            swLapse = new System.Diagnostics.Stopwatch();
            swTimer = new System.Diagnostics.Stopwatch();            
            STEPMANAGER_VALUE.bNadKeyDllRun = false;
            RSENSOR_DIC = new Dictionary<int, int>();
            for (int i = (int)DIOPIN.START; i <= (int)DIOPIN.PINUSE; i++)
            {
                if (i.Equals((int)DIOPIN.PINUSE))
                    RSENSOR_DIC.Add(i, (int)SENSOR.OFF); //처음스타트 방지
                else
                    RSENSOR_DIC.Add(i, (int)SENSOR.ING); //처음스타트 방지
            }

            for (int i = (int)DOUT.OK + 1; i < (int)DOUT.END; i++)
            {
                RSENSOR_DIC.Add((int)DIOPIN.PINUSE + i, (int)SENSOR.OFF); //아웃PUT 쪽은 최초에는 OFF 로 설정하자.
            }

            DKLoggerPC = new DK_LOGGER("PC", false);            
            DKLoggerMR.SendTxRxEvent += new EventTxRxMsg(InterChange_Sub_Manager);

            DK_TC1400A.CommSendReport += new EventDKCOM(GateWay_MANAGER);
            DK_TC1400A.EtherNetRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);

            DKACTOR = new DK_ACTOR((int)DEFINES.END, "DIO");
            DKACTOR.ActorSendReport += new EventDKCOM(GateWay_MANAGER);
            DKACTOR.ActorSendReport2 += new EventRealTimeMsg(InterChange_MANAGER);
            DKACTOR.ActorSendReport3 += new EventSensorDKCOM(InterChange_MANAGER2);

            DK_PCAN_USB.PCANRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);
            DKVECTOR.VectorRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);            
            DK_NADKEY.NadKeyDllRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);
            
            DK_DLLGATE.CommSendReport += new EventDKCOM(GateWay_MANAGER);
            DK_DLLGATE.EtherNetRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);


            //*******************************************************************************************************************
            DKLoggerPC.SetEnvironmentOraRegister(); //ORACLE 을 사용할 경우 해당 클래스를 Create 하기 전에 환경변수를 path 를
                                                     //Process 블록으로 끌어오면 오라클 버젼이 중복으로 깔려있어도 내 디렉토리를 참조할 수 있다.
            DKORACLE = new DK_ORACLE();
            DKORACLE.OracleSendReport += new EventRealTimeMsg(InterChange_MANAGER);

            //*******************************************************************************************************************
                        
            StatusTimer = new System.Threading.Timer(CycleActorStatusEngine);
            
            Item_bTestStarted = false;
            Item_bInteractiveMode = false;
            string Msg = String.Empty;

            for (int i = 0; i < deviceStatus.Length; i++)
            {
                deviceStatus[i].iStatus = (int)STATUS.NOTUSE;
                deviceStatus[i].iDevNumber = i;
            }

            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
            {
                UseSlots[i] = false;
                DKExpr[i]       = new DK_EXPR();
                SEQUENCE_DIC[i] = new Dictionary<int, int>();
                STPCHECK_DIC[i] = false;
                RESULTDT_DIC[i] = new Dictionary<int, string>();
                RESPONSE_DIC[i] = new Dictionary<int, string>();
                SENDPACK_DIC[i] = new Dictionary<int, string>();    
                LstTST_RES[i]   = new List<RESDATA>();
            }

            iNowJobNumber = 0;

            for (int i = 0; i < bCmdDoneCheck.Length; i++)
            {
                bCmdDoneCheck[i] = true;
            }

            for (int i = 0; i < (int)ENDOPTION.END; i++)
            {
                bUsedEndOption[i] = false;
                strEndOptionCommand[i] = new string[3];

                for(int j = 0; j < strEndOptionCommand[i].Length; j++)
                {
                    strEndOptionCommand[i][j] = String.Empty;
                }
                
            }

            TableFileLoad();
            ClearLastNGList();
        }

        public bool GetFileSize(string strFileName, ref string strFileSize)
        {
            return DKLoggerMR.GetFileSize(strFileName, ref strFileSize);
        }

        public void ConnectionPorts(int iSERIAL_ENUM)
        {   
            DEVICEDATA DevData = new DEVICEDATA();
            DKACTOR.CommOff(iSERIAL_ENUM);
            string strPort = String.Empty;
            switch (iSERIAL_ENUM)
            {
                case (int)COMSERIAL.DIO:
                                        strPort = DKLoggerPC.LoadINI("COMPORT", "DIO");
                                        OpenDIO = DKACTOR.CommOpen((int)COMSERIAL.DIO, "COM" + strPort, 9600);
                                        if (OpenDIO) DevData.iStatus = (int)STATUS.OK;
                                        else DevData.iStatus = (int)STATUS.NG;
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.DIO;
                                        SendManagerSendReport3(DevData);
                                        break;
                case (int)COMSERIAL.AUDIOSEL:
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.AUDIOSEL;
                                        if (!ConfigLoad("OPTION", "AUDIOSELECTOR"))
                                        {
                                            OpenAudio = DevData.iStatus = (int)STATUS.NOTUSE;                                            
                                        }
                                        else
                                        {
                                            strPort = DKLoggerPC.LoadINI("COMPORT", "AUDIOSELECTOR");
                                            
                                            if(DKACTOR.CommOpen((int)COMSERIAL.AUDIOSEL, "COM" + strPort, 9600))                                           
                                                OpenAudio = DevData.iStatus = (int)STATUS.OK;                                           
                                            else
                                                OpenAudio = DevData.iStatus = (int)STATUS.NG;                                           
                                                                                 
                                        }
                                        SendManagerSendReport3(DevData);
                                        break;
                case (int)COMSERIAL.ADC:
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.ADC;
                                        if (!ConfigLoad("OPTION", "ADC-MODULE"))
                                        {
                                            OpenADC = DevData.iStatus = (int)STATUS.NOTUSE;
                                        }
                                        else
                                        {
                                            strPort = DKLoggerPC.LoadINI("COMPORT", "ADC-MODULE");

                                            if (DKACTOR.CommOpen((int)COMSERIAL.ADC, "COM" + strPort, 9600))
                                                OpenADC = DevData.iStatus = (int)STATUS.OK;
                                            else
                                                OpenADC = DevData.iStatus = (int)STATUS.NG;

                                        }
                                        SendManagerSendReport3(DevData);
                                        break;
                case (int)COMSERIAL.SET:
                                        strPort = DKLoggerPC.LoadINI("COMPORT", "SET");
                                        OpenSET = DKACTOR.CommOpen((int)COMSERIAL.SET, "COM" + strPort, 115200);
                                        if (OpenSET) DevData.iStatus = (int)STATUS.OK;
                                        else DevData.iStatus = (int)STATUS.NG;
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.SET;
                                        SendManagerSendReport3(DevData);
                                        break;
                case (int)COMSERIAL.TC3000:
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.TC3000;
                                        if (!ConfigLoad("OPTION", "TC3000"))
                                        {
                                            DevData.iStatus = (int)STATUS.NOTUSE;                                            
                                        }
                                        else
                                        {
                                            strPort = DKLoggerPC.LoadINI("COMPORT", "TC3000");
                                            OpenTC3000 = DKACTOR.CommOpen((int)COMSERIAL.TC3000, "COM" + strPort, 9600);
                                            if (OpenTC3000) DevData.iStatus = (int)STATUS.OK;
                                            else DevData.iStatus = (int)STATUS.NG;                                            
                                        }
                                        SendManagerSendReport3(DevData);
                                        break;
                case (int)COMSERIAL.SCANNER:
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.SCANNER;
                                        if (!ConfigLoad("OPTION", "RS232SCANNER"))
                                        {
                                            DevData.iStatus = (int)STATUS.NOTUSE;                                            
                                        }
                                        else
                                        {
                                            strPort = DKLoggerPC.LoadINI("COMPORT", "RS232SCANNER");
                                            OpenSCAN = DKACTOR.CommOpen((int)COMSERIAL.SCANNER, "COM" + strPort, 9600);
                                            if (OpenSCAN) DevData.iStatus = (int)STATUS.OK;
                                            else DevData.iStatus = (int)STATUS.NG;                                                                    
                                        }
                                        SendManagerSendReport3(DevData);
                                        break;
                
                case (int)COMSERIAL.ODAPWR: //UI 에 표시못한다. 자리가 없다....
                                        DevData.iDevNumber = (int)DEVSTATUSNUMBER.ODAPWR;
                                        if (!ConfigLoad("OPTION", "ODAPWR"))
                                        {
                                            DevData.iStatus = (int)STATUS.NOTUSE;
                                        }
                                        else
                                        {
                                            strPort = DKLoggerPC.LoadINI("COMPORT", "ODAPWR");
                                            OpenODAPWR = DKACTOR.CommOpen((int)COMSERIAL.ODAPWR, "COM" + strPort, 9600);
                                            if (OpenODAPWR) DevData.iStatus = (int)STATUS.OK;
                                            else DevData.iStatus = (int)STATUS.NG;
                                        }
                                        SendManagerSendReport3(DevData);
                                        break;

                
                default: break;
            }

            
        }

        public void DisconnectPorts(int iSERIAL_ENUM)
        {           
            DKACTOR.CommOff(iSERIAL_ENUM);           
        }

        public bool DeviceControlUSB(string strOption)
        {   
            string strProgramPath = AppDomain.CurrentDomain.BaseDirectory;
            string strParam = "*USB\\VID_0C72*";
            
            System.Diagnostics.Process reg = new System.Diagnostics.Process();

            try
            {
                reg.StartInfo.FileName = strProgramPath + strDevCon32;
                reg.StartInfo.Arguments = strOption + " " + strParam;
                reg.StartInfo.UseShellExecute = false;
                reg.StartInfo.CreateNoWindow = true;
                reg.StartInfo.RedirectStandardOutput = true;
                reg.StartInfo.Verb = "runas";
                reg.Start();
                reg.WaitForExit(5000);
                reg.Close();
            }
            catch
            {
                 
                 reg.Dispose();
        
            }

            return true;
        }

        private void MessageLogging(int iLogType, string strMessage, int iPort)
        {
            StringBuilder sbLogHead = new StringBuilder(4096);
            StringBuilder sbLogBody = new StringBuilder(4096);
            //CSMES
            string strCommandType = string.Empty;
            if (iLogType != (int)LOGTYPE.PC && LstJOB_CMD.Count != 0)   //CSMES 에서 CONNECTION 시 JOB 파일이 선택되지 않았으면 ERROR 발생.
                strCommandType = LstJOB_CMD[iNowJobNumber].CMD;

            switch (iLogType)
            {
                case (int)LOGTYPE.TX:
                    sbLogHead.Append("[TX]");
                    sbLogHead.Append(strMessage);

                    sbLogBody.Append(LstJOB_CMD[iNowJobNumber].TYPE.ToString());
                    sbLogBody.Append(":");
                    sbLogBody.Append(strCommandType);
                    break;

                case (int)LOGTYPE.RX:
                    try
                    {
                        sbLogHead.Append("[RX]");
                        sbLogHead.Append(strMessage);
                        if (!String.IsNullOrEmpty(strMessage))
                            sbLogHead.Append(" ");
                        sbLogHead.Append(RESULTDT_DIC[iPort][iNowJobNumber]);
                        sbLogHead.Append(" ");
                        if (!RESULTDT_DIC[iPort][iNowJobNumber].Equals(RESPONSE_DIC[iPort][iNowJobNumber]))
                        {
                            sbLogHead.Append("- ");
                            sbLogHead.Append(RESPONSE_DIC[iPort][iNowJobNumber]);
                        }
                        sbLogBody.Append(LstJOB_CMD[iNowJobNumber].TYPE);
                        sbLogBody.Append(":");
                        sbLogBody.Append(strCommandType);
                    }
                    catch 
                    { }
                    break;

                case (int)LOGTYPE.PC:
                    sbLogBody.Append(strMessage);
                    break;

                default: return;
            }
            
            DKLoggerMR.WriteCommLog(sbLogHead.ToString(), sbLogBody.ToString(), false);
        }

        private void MessageLoggingEx(DK_LOGGER target, int iLogType, string strMessage, int iPort)
        {
            StringBuilder sbLogHead = new StringBuilder(4096);
            StringBuilder sbLogBody = new StringBuilder(4096);
            string strCommandType = LstJOB_CMD[iNowJobNumber].CMD;
            switch (iLogType)
            {
                case (int)LOGTYPE.TX:
                    sbLogHead.Append("[TX]");
                    sbLogHead.Append(strMessage);

                    sbLogBody.Append(LstJOB_CMD[iNowJobNumber].TYPE.ToString());
                    sbLogBody.Append(":");
                    sbLogBody.Append(strCommandType);
                    break;

                case (int)LOGTYPE.RX:
                    sbLogHead.Append("[RX]");
                    sbLogHead.Append(strMessage);
                    if (!String.IsNullOrEmpty(strMessage))
                        sbLogHead.Append(" ");                    
                    sbLogBody.Append(LstJOB_CMD[iNowJobNumber].TYPE);
                    sbLogBody.Append(":");
                    sbLogBody.Append(strCommandType);
                    break;

                case (int)LOGTYPE.PC:
                    sbLogBody.Append(strMessage);
                    break;

                default: return;
            }

            target.WriteCommLog(sbLogHead.ToString(), sbLogBody.ToString(), false);
        }

#region ORACLE 명령 관련

        public ORACLEINFO GetOracleInfo()
        {
            ORACLEINFO Info = new ORACLEINFO();

            Info.strProductionLine  = DKLoggerPC.LoadINI("ORACLE", sOraProductionLine);
            Info.strProcessCode     = DKLoggerPC.LoadINI("ORACLE", sOraProcessCode);
            Info.strPCID            = DKLoggerPC.LoadINI("ORACLE", sOraPCID);
            Info.strServerIP        = DKLoggerPC.LoadINI("ORACLE", sOraServerIP);
            Info.strPort            = DKLoggerPC.LoadINI("ORACLE", sOraPort);
            Info.strServiceName     = DKLoggerPC.LoadINI("ORACLE", sOraServiceName);
            Info.strOOBCode         = DKLoggerPC.LoadINI("ORACLE", sOraOOBCode);
            Info.strOOBFlag         = DKLoggerPC.LoadINI("ORACLE", sOraOOBFlag);
            Info.strCallType        = DKLoggerPC.LoadINI("ORACLE", sOraCallType);
                        
            Info.bDontCareOOBcode   = DKLoggerPC.LoadINI("OPTION", sOraDonCareOOBCODE).Equals("ON");

            return Info;
        }

        public ORACLEINFO GetOracleInfoName()
        {
            ORACLEINFO Info = new ORACLEINFO();

            Info.strProductionLine  = sOraProductionLine;
            Info.strProcessCode     = sOraProcessCode;
            Info.strPCID            = sOraPCID;
            Info.strServerIP        = sOraServerIP;
            Info.strPort            = sOraPort;
            Info.strServiceName     = sOraServiceName;
            Info.strOOBCode         = sOraOOBCode;
            Info.strOOBFlag         = sOraOOBFlag;
            Info.strCallType        = sOraCallType;

            return Info;
        }

        public bool OracleConnection(bool bTimeSync, ref string strReason)
        {
            if (DKORACLE == null)
            {
                strReason = "MES CAN NOT INITIALZE.";
                return false;
            }

            ORACLEINFO Info = new ORACLEINFO();
            Info = GetOracleInfo();

            strReason = String.Empty;
            bool bOK = false;
            bOK = DKORACLE.SetConnectionString(Info.strServerIP, Info.strPort, Info.strServiceName, ref strReason);

            if (!bOK)
            {
                return false;
            }

            if (!OracleCheckConnection(ref strReason))
            {
                return false;
            }

            if (bTimeSync) DKORACLE.SetSystemTimeSync(Info.strCallType, ref strReason);
            DKORACLE.DisConnect();
            return true;

        }
        
        private bool OracleCheckConnection(ref string strReason)
        {
            
            if (DKORACLE == null)
            {
                return false;
            }
            
            bool bConn = false;

            if (DKORACLE.IsConnected(ref strReason))
            {
                DKORACLE.DisConnect();
            }

            DKORACLE.Connect(ref strReason);            
            
            string strtmpReason = String.Empty;
            if (DKORACLE.IsConnected(ref strtmpReason))
            {
                strReason = strtmpReason;
                bConn = true;
            }

            return bConn;

        }
        
        //오라클 MES INFO 데이터 임시로 EXPR 에 저장하는 함수.
        private void ExprSaveInfoData(int iPort, string[] strSubject, string[] strData)
        {
            lock (lockObject)
            {
                try
                {
                    for (int i = 0; i < strSubject.Length; i++)
                    {
                        bool bExpr = DKExpr[iPort].ExcuteSave("#SAVE:" + strSubject[i], strData[i]);
                    }
                }
                catch{}
            }
        }

        //외부에서 EXPR 에 저장하는 함수.
        private void ExprSaveData(int iPort, string strSubject, string strData)
        {
            lock (lockObject)
            {
                try
                {
                    bool bExpr = DKExpr[iPort].ExcuteSave("#SAVE:" + strSubject, strData);                    
                }
                catch { }
            }
        }

        private bool GetKeyWriteValues(int iPort, ref string[] strKeyValue, ref string strKeyReason)
        {
            strKeyReason = String.Empty;
            strKeyValue = new string[8]; //CHECKSUM, VCERT, VPRIKEY, AUTHCODE, HASH, ICCID, IMSI, MSISDN
            bool[] bExist = new bool[8];

            bExist[0] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_CHECKSUM",  ref strKeyValue[0]);
            bExist[1] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_VCERT",     ref strKeyValue[1]);
            bExist[2] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_VPRIKEY",   ref strKeyValue[2]);
            bExist[3] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_AUTHCODE",  ref strKeyValue[3]);
            bExist[4] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_HASH",      ref strKeyValue[4]);
            bExist[5] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_ICCID",     ref strKeyValue[5]);
            bExist[6] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_IMSI",      ref strKeyValue[6]);
            bExist[7] = DKExpr[iPort].ExcuteLoad("#LOAD:SET_MSISDN",    ref strKeyValue[7]);

            bool bCheck = true;
            for (int i = 0; i < bExist.Length; i++)
            {
                if (!bExist[i])
                {
                    switch (i)
                    {
                        case 0: strKeyReason += "(SET_CHECKSUM)";   break;
                        case 1: strKeyReason += "(SET_VCERT)";      break;
                        case 2: strKeyReason += "(SET_VPRIKEY)";    break;
                        case 3: strKeyReason += "(SET_AUTHCODE)";   break;
                        case 4: strKeyReason += "(SET_HASH)";       break;
                        case 5: strKeyReason += "(SET_ICCID)";      break;
                        case 6: strKeyReason += "(SET_IMSI)";       break;
                        case 7: strKeyReason += "(SET_MSISDN)";     break;
                        default: break;
                    }
                    bCheck = false;
                }
            }
            return bCheck;
        }

        private bool OracleMesCommand(int iPort, int iJobNum, string strCmdType, string strParam)
        {
            bool bRtnVal = false;

            //Oracle 접속 정보 가져오기.
            ORACLEINFO Info = new ORACLEINFO();
            Info = GetOracleInfo();

            Oracle_Procedure opRes = new Oracle_Procedure();
            string[] strOpParam;
            string strReason = String.Empty;

            string strErrMsg = String.Empty;
            string strDtlErrMsg = String.Empty;
            string strInspDesc = String.Empty;
            string strInspName = String.Empty;
            bool bCheckInspName = false;
            bool bCheckInspDesc = false;
            bool bCheckErrMsg = false;
            bool bCheckDtlErrMsg = false;


            switch (Info.strCallType) //GM TELEMATICS는 GEN9 . 10 .TCP OOB, 라벨발행. KEYWRITE 가 좀 사연이 많다.
            {               
                case "OOB":
                    switch (strCmdType)
                    {                                              
                        case "GET_KEYWRITE_MAIN":
                        case "GET_KEYWRITE_MAIN_TCP":
                        case "GET_KEYWRITE_MAIN_PSA":
                        case "SET_KEYWRITE_MAIN": 
                        case "STEP_CHECK": 
                        case "STEP_COMPLETE":
                        case "GET_MODEL_INFO":
                        case "GET_PCB_INFO": 
                                            bRtnVal = false;
                                            MessageLogging((int)LOGTYPE.TX, "(CAN NOT IN OOB STATION)", iPort);
                                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "CAN NOT IN OOB STATION", String.Empty, true);
                                            return bRtnVal;
                        default: break;
                    }    
                    break;

                case "FA":
                    if (!STPCHECK_DIC[iPort])
                    {
                        switch (strCmdType)
                        {
                            case "STEP_CHECK":
                            case "DATA_VIEW": break;                          
                            case "STEP_COMPLETE":
                                                    bRtnVal = false;
                                                    MessageLogging((int)LOGTYPE.TX, "(NO STEP CHECK)", iPort);
                                                    GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NO STEP CHECK.", String.Empty, true);
                                                    return bRtnVal;
                            default: break;
                        }
                    }

                    switch (strCmdType)
                    {
                        case "GET_OOB_INFO":
                        case "GET_OOB_INFO_PSA":
                        case "SET_OOB_INFO":
                        case "GET_PCB_INFO":
                            bRtnVal = false;
                            MessageLogging((int)LOGTYPE.TX, "(CAN NOT IN FA STATION)", iPort);
                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "CAN NOT IN FA STATION", String.Empty, true);
                            return bRtnVal;
                        default: break;
                    }
                    break;

                case "PCB":
                    if (!STPCHECK_DIC[iPort])
                    {
                        switch (strCmdType)
                        {                           
                            case "STEP_CHECK":
                            case "DATA_VIEW": break;                          
                            case "STEP_COMPLETE":
                                                    bRtnVal = false;
                                                    MessageLogging((int)LOGTYPE.TX, "(NO STEP CHECK)", iPort);
                                                    GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NO STEP CHECK.", String.Empty, true);
                                                    return bRtnVal;
                            default: break;
                        }                                            
                    }                         
                    switch (strCmdType)
                    {
                        case "GET_MODEL_INFO":
                        case "GET_OOB_INFO":
                        case "GET_OOB_INFO_PSA":
                        case "SET_OOB_INFO":
                        case "GET_KEYWRITE_MAIN_TCP":
                        case "GET_KEYWRITE_MAIN_PSA":
                        case "GET_KEYWRITE_MAIN":
                        case "SET_KEYWRITE_MAIN":
                     
                            bRtnVal = false;
                            MessageLogging((int)LOGTYPE.TX, "(CAN NOT IN PCB STATION)", iPort);
                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "CAN NOT IN PCB STATION", String.Empty, true);
                            return bRtnVal;
                        default: break;
                    }
                    break;
                default:
                    break;
                    
            }
            
            switch (strCmdType)
            {
                case "STEP_CHECK": break;
                case "STEP_COMPLETE":
                    if (STEPMANAGER_VALUE.bInteractiveMode)
                    {
                        MessageLogging((int)LOGTYPE.TX, "CAN NOT COMPLETE(INTERACTIVE)", iPort);
                        GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "NOT USE BY INTERACTIVE", String.Empty, true);                    
                        return bRtnVal;
                    }
                    break;
                case "DATA_VIEW":
                    break;
                    
                case "GET_MODEL_INFO":
                case "GET_PCB_INFO":
                case "GET_KEYWRITE_MAIN":
                case "GET_KEYWRITE_MAIN_TCP":
                case "GET_KEYWRITE_MAIN_PSA":
           
                    if (Item_bUseBarcode)
                    {
                        if (Item_WIPID.Length == Item_iWIPSIZE) //바코드 스캐너로 읽었다면
                        {
                            STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = Item_WIPID;
                        }
                        else
                        {
                            if (STPCHECK_DIC[iPort] && !String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))//스텝체크를 했다면 스텝체크 WIP으로 
                            {
                                break;
                            }
                            if (!String.IsNullOrEmpty(strParam)) //아니면 파라미터 값으로
                            {
                                STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strParam;
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "BARCODE SCANNER READ SIZE FAIL.", String.Empty, true);
                                bRtnVal = false;
                                return bRtnVal;
                            }
                            
                        }
                    }
                    else //아니면 
                    {
                        if (STPCHECK_DIC[iPort] && !String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))//스텝체크를 했다면 스텝체크 WIP으로 
                        {
                            break;
                        }
                        if (!String.IsNullOrEmpty(strParam)) //아니면 파라미터 값으로
                        {
                            STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strParam;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO PARAM", String.Empty, true);
                            bRtnVal = false;
                            return bRtnVal;
                        }
                        
                    }
                    break;

                case "SET_KEYWRITE_MAIN"://최종결과가 NG면 실행하지 말자.
                    if (String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))
                    {
                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NO GET DATA", String.Empty, true);
                        bRtnVal = false;
                        return bRtnVal;
                    }

                    
                    //최종결과 검색.
                    bool bLastRes = true;
                    for (int i = 0; i < LstTST_RES[iPort].Count; i++)
                    {
                        switch (LstTST_RES[iPort][i].iStatus)
                        {
                            case (int)STATUS.OK: break;
                            case (int)STATUS.SKIP: break;
                            default: bLastRes = false; break;
                        }
                        if (!bLastRes)
                        {   //최종결과가 NG면 실행하지 말자.
                            GateWayMsgProcess((int)STATUS.SKIP, "TEST RESULT NG", "TEST RESULT NG", String.Empty, true);
                            bRtnVal = false;
                            return bRtnVal;
                        }
                    }
                    
                    break;

                case "GET_OOB_INFO":
                case "GET_OOB_INFO_PSA":
                case "SET_OOB_INFO":
                    if (String.IsNullOrEmpty(strParam))
                    {
                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NO PARAM(IMEI)", String.Empty, true);
                        bRtnVal = false;
                        return bRtnVal;
                    }
                    break;
                default:
                    break;

            }


            switch (strCmdType)
            {
                case "STEP_CHECK":
                    MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                    SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + strParam;

                    switch (Info.strCallType)
                    {
                        case "PCB":
                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_CHECK;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_PCB_STEP_CHECK;
                            #endif
                            break;
                        default:
                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_CHECK;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_STEP_CHECK;
                            #endif
                            break;
                    }

                    strOpParam = new string[1];

                    if (Item_bUseBarcode)
                    {
                        if (Item_WIPID.Length == Item_iWIPSIZE) //바코드 스캐너로 읽었다면
                        {
                            strOpParam[0] = Item_WIPID;
                            bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);                            
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strParam))
                            {
                                strOpParam[0] = strParam;
                                bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "BARCODE SCANNER READ SIZE FAIL.", String.Empty, true);
                                bRtnVal = false;
                                return bRtnVal;
                            }                            
                        }
                    }
                    else //아니면 파라미터 값으로
                    {
                        if (String.IsNullOrEmpty(strParam))
                        {
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO PARAM(WIP ID)", String.Empty, true);
                            return false;
                        }
                        strOpParam[0] = strParam;
                        bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);
                        
                    }

                    STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strOpParam[0];


                    if (bRtnVal) //오라클 프로시져 호출 성공후
                    {
                        //현재 스테이션과 같은 정보인지 비교                    
                        bCheckInspName = DKORACLE.GetFieldValue(opRes, "INSP_STEP", ref strInspName);
                        bCheckInspDesc = DKORACLE.GetFieldValue(opRes, "INSP_STEP_DESC", ref strInspDesc);
                        bCheckErrMsg   = DKORACLE.GetFieldValue(opRes, "ERRMSG", ref strErrMsg);

                        if (bCheckErrMsg && bCheckInspName && bCheckInspDesc)
                        {
                            if (strErrMsg.ToUpper().Equals("TRUE")
                                    || strErrMsg.ToUpper().Equals("OK")
                                       || strErrMsg.ToUpper().Equals("SUCCESS"))
                            {
                                if (Info.strProcessCode.Equals(strInspName))
                                {
                                    STPCHECK_DIC[iPort] = true;
                                    GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), strInspName, String.Empty, true);
                                    bRtnVal = true;
                                }
                                else
                                {
                                    STPCHECK_DIC[iPort] = false;
                                    GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strInspName, String.Empty, true);
                                    bRtnVal = false;
                                }
                            }
                            else
                            {
                                STPCHECK_DIC[iPort] = false;
                                GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMsg, String.Empty, true);
                                bRtnVal = false;
                            }

                        }
                        else
                        {
                            STPCHECK_DIC[iPort] = false;
                            bRtnVal = false;

                            if (bCheckErrMsg)
                            {
                                GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMsg, String.Empty, true);
                            }
                            else
                            {
                                if (bCheckInspDesc)
                                {
                                    GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMsg, String.Empty, true);
                                }
                                else
                                {
                                    GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "MES-ERROR", String.Empty, true);
                                }
                            }

                        }

                    }
                    else
                    {   //오라클 프로시져 호출 실패

                        STPCHECK_DIC[iPort] = false;
                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strReason, String.Empty, true);
                        bRtnVal = false;
                    }

                    return bRtnVal;

                case "DATA_VIEW":

                    bRtnVal = String.IsNullOrEmpty(strParam);
               
                    bRtnVal = CheckExprParam(iPort, iJobNum, ref strParam);
                    if (bRtnVal)
                    {
                        GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), strParam, LstJOB_CMD[iJobNum].PAR1, true);
                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO DATA", String.Empty, true);
                    }
                    return bRtnVal;

                case "STEP_COMPLETE":

                    string strDetailIdx = String.Empty;
                    bool bCheckDetailIdx = false;

                    //최종결과 검색.
                    bool bLastRes = true;
                    for (int i = 0; i < LstTST_RES[iPort].Count; i++)
                    {
                        switch (LstTST_RES[iPort][i].iStatus)
                        {
                            case (int)STATUS.OK: break;
                            case (int)STATUS.SKIP: break;
                            default: bLastRes = false; break;
                        }
                        if (!bLastRes) break;
                    }

                    //아이템 코드 올릴것이 있는지 검사.
                    List<string> lstItemCode = new List<string>();
                    lstItemCode = MesItemCoding(iPort, iJobNum);
                    bool bUploadData = false;
                    if (lstItemCode.Count > 0)
                    {
                        bUploadData = true;
                    }

                    MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                    SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + strParam;

                    if (bUploadData) //아이템 코드 업로드 할것이 있으면 MASTER INSERT 와 DETAIL INSERT 를 한다.
                    {
                        switch (Info.strCallType)
                        {
                            case "PCB":
                                opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_MASTER_INSERT;                                
                                //MOOHAN SERVER 임시 TEST 프로시져
                                #if USE_MOOHAN_SERVER
                                opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_PCB_MASTER_INSERT;
                                #endif
                                break;
                            default:
                                opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_MASTER_INSERT;                           
                                //MOOHAN SERVER 임시 TEST 프로시져
                                #if USE_MOOHAN_SERVER
                                opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_MASTER_INSERT;
                                #endif
                                break;
                        }
                        strOpParam = new string[7];                        

                        //순서1 - MASTER INSERT - IN : V_LINE(Production), V_INSP(Process), V_SNNO(WipID), V_RESULT(RESULT), V_TIME(TIME), V_TESTCOUNT(ITEM COUNT, V_PC_ID(PCNAME)
                        //                       OUT : N_TXNID(DETAIL INDEX)
                        strOpParam[0] = Info.strProductionLine;
                        strOpParam[1] = Info.strProcessCode;
                        strOpParam[2] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                        strOpParam[3] = "OK";
                        if (!bLastRes) strOpParam[3] = "NG";
                        strOpParam[4] = DateTime.Now.ToString("yyyyMMddHHmmss"); //TEM2 는 yyyyMMdd hhmmss 중간에 공백이 있는데 이건 없네....ㅡ.ㅡ
                        strOpParam[5] = lstItemCode.Count.ToString();
                        strOpParam[6] = Info.strPCID; 

                        bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);
                        
                        if (bRtnVal) // 오라클 프로시져 호출 성공후
                        {
                            // DETAIL INDEX 가 있는지 확인. 
                            bCheckDetailIdx = DKORACLE.GetFieldValue(opRes, "TXNID", ref strDetailIdx); 
                           
                            if (bCheckDetailIdx && strDetailIdx.Length > 0)
                            {   //MASTER INSERT SUCCESS                                
                                //DETAIL INSERT 할것이 있다면, IN : V_TXNID, V_ITEMID, V_ITEMRESULT, V_ITEMVALUE
                                for (int i = 0; i < lstItemCode.Count; i++)
                                {
                                    opRes = new Oracle_Procedure(); //구조체 초기화
                                    strOpParam = new string[4];     //파라미터 배열 초기화

                                    switch (Info.strCallType)
                                    {
                                        case "PCB":
                                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_DETAIL_INSERT;
                                            //MOOHAN SERVER 임시 TEST 프로시져
                                            #if USE_MOOHAN_SERVER
                                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_PCB_DETAIL_INSERT;
                                            #endif
                                            break;
                                        default:
                                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_DETAIL_INSERT;
                                            //MOOHAN SERVER 임시 TEST 프로시져
                                            #if USE_MOOHAN_SERVER
                                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_DETAIL_INSERT;
                                            #endif
                                            break;
                                    }
                                    
                                    strOpParam[0] = strDetailIdx;

                                    string[] strItemCodeList = new string[3];
                                    strItemCodeList = System.Text.RegularExpressions.Regex.Split(lstItemCode[i], "=");
                                    if (strItemCodeList.Length != 3)
                                    {
                                        continue;
                                    }
                                    strOpParam[1] = strItemCodeList[0]; //아이템코드명
                                    strOpParam[2] = strItemCodeList[1]; //OK,NG
                                    strOpParam[3] = strItemCodeList[2]; //측정값
                                    //순서2 - DETAIL INSERT
                                    bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);

                                    if (!bRtnVal)
                                    {   //오라클 프로시져 호출 실패
                                        STPCHECK_DIC[iPort] = false;
                                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strReason, String.Empty, true);
                                        return bRtnVal;
                                    }
                                }
                            }
                            else
                            {
                                STPCHECK_DIC[iPort] = false;
                                GateWayMsgProcess((int)STATUS.MESERR, "TXNID Error", "TXNID Error", String.Empty, true);
                                return bRtnVal;                                
                            }
                        }
                        else
                        {   //오라클 프로시져 호출 실패
                            STPCHECK_DIC[iPort] = false;
                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strReason, String.Empty, true);
                            return bRtnVal;
                        }
                    }

                    //순서3 - STEP COMPLETE IN : V_WIP_ID, V_INSP_STEP, V_LINE, V_INSP_RESULT, 
                    //                    OUT : V_ERRMSG
                    opRes = new Oracle_Procedure(); //구조체 초기화
                    strOpParam = new string[4];     //파라미터 배열 초기화
                    strOpParam[0] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                    strOpParam[1] = Info.strProcessCode;
                    strOpParam[2] = Info.strProductionLine;
                    strOpParam[3] = "OK";
                    if (!bLastRes) strOpParam[3] = "NG";

                    switch (Info.strCallType)
                    {
                        case "PCB":
                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_COMPLETE;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_PCB_STEP_COMPLETE;
                            #endif
                            break;
                        default:
                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_COMPLETE;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_STEP_COMPLETE;
                            #endif
                            break;
                    }
                    

                    bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);

                    if (bRtnVal)
                    {
                        strErrMsg = String.Empty;
                        bCheckErrMsg = DKORACLE.GetFieldValue(opRes, "ERRMSG", ref strErrMsg);

                        if (strErrMsg.ToUpper().Equals("TRUE")
                                    || strErrMsg.ToUpper().Equals("OK")
                                       || strErrMsg.ToUpper().Equals("SUCCESS"))
                        {
                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), STATUS.OK.ToString(), String.Empty, true);
                            bRtnVal = true;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMsg, String.Empty, true);
                            bRtnVal = false;
                        }

                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strReason, String.Empty, true);
                        bRtnVal = false;
                    }
                    return bRtnVal;

                case "GET_MODEL_INFO":
                case "GET_PCB_INFO":
                case "SET_OOB_INFO":
                case "GET_KEYWRITE_MAIN":
                case "GET_KEYWRITE_MAIN_TCP":
                case "GET_KEYWRITE_MAIN_PSA":
                    strOpParam = new string[1];

                    switch (strCmdType)
                    {
                        case "GET_KEYWRITE_MAIN_TCP": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_TCP;
                            MessageLogging((int)LOGTYPE.TX, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID, iPort);
                            SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GM_GET_MAIN;
                            #endif
                            strOpParam = new string[2];
                            strOpParam[0] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            strOpParam[1] = Info.strPCID;
                            STPCHECK_DIC[iPort] = true;  //키라이팅(ESN) 공정은 스텝체크 안된다고 해서 여기에 한것처럼 넣어둠.
                            break;

                        case "GET_KEYWRITE_MAIN_PSA": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_PSA;
                            MessageLogging((int)LOGTYPE.TX, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID, iPort);
                            SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                                opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GM_GET_MAIN;
                            #endif
                            strOpParam = new string[2];
                            strOpParam[0] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            strOpParam[1] = Info.strPCID;
                            STPCHECK_DIC[iPort] = true;  //키라이팅(ESN) 공정은 스텝체크 안된다고 해서 여기에 한것처럼 넣어둠.
                            break;

                        case "GET_KEYWRITE_MAIN": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN;
                            MessageLogging((int)LOGTYPE.TX, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID, iPort);
                            SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GM_GET_MAIN;
                            #endif
                            strOpParam = new string[2];                            
                            strOpParam[0] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            strOpParam[1] = Info.strPCID;
                            STPCHECK_DIC[iPort] = true; ////키라이팅(ESN) 공정은 스텝체크 안된다고 해서 여기에 한것처럼 넣어둠.
                            break;
                        case "GET_PCB_INFO":  opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_MODEL_INFO;
                            MessageLogging((int)LOGTYPE.TX, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID, iPort);                            
                            SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_PCB_GET_MODEL_INFO;
                            #endif
                            strOpParam = new string[2];
                            strOpParam[0] = "AV"; //GEN10 패키지는 AV 로 fix (CNS 정진섭 차장 - 패키지 문서에도 정의되어있음)
                            strOpParam[1] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            break;
                        case "GET_MODEL_INFO": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_MODEL_INFO;
                            MessageLogging((int)LOGTYPE.TX, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID, iPort);                            
                            SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GET_MODEL_INFO;
                            #endif
                            strOpParam = new string[2];
                            strOpParam[0] = "AV"; //GEN10 패키지는 AV 로 fix (CNS 정진섭 차장 - 패키지 문서에도 정의되어있음)
                            strOpParam[1] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            break;
                        case "SET_OOB_INFO":

                            //최종결과 검색.
                            bool bOOBLastRes = true;
                            for (int i = 0; i < LstTST_RES[iPort].Count; i++)
                            {
                                try
                                {
                                    switch (LstTST_RES[iPort][i].iStatus)
                                    {
                                        case (int)STATUS.OK: break;
                                        case (int)STATUS.SKIP: break;
                                        default: bOOBLastRes = false; break;
                                    }
                                    if (!bOOBLastRes) break;
                                }
                                catch
                                { }

                            }

                            if (!bOOBLastRes)
                            {
                                GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), STATUS.SKIP.ToString(), String.Empty, true);
                                return true;
                            }

                            opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_SET_GSM_INFO;
                            MessageLogging((int)LOGTYPE.TX, strParam + "," + Info.strOOBFlag, iPort);                            
                            SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID + "," + Info.strOOBFlag;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_SET_GSM_INFO;
                            #endif
                            strOpParam = new string[3];
                            strOpParam[0] = "AV"; //GEN10 패키지는 AV 로 fix (CNS 정진섭차장 - 문서에도 정의됨)
                            if (strParam.Length == 15)
                                strParam = strParam.Substring(0, 14);
                            strOpParam[1] = strParam; //GEN10 패키지는 WIPID(SN) 을 올리는게 아니라 IMEI 를 14자리 잘라서 올린다고 함...(CNS 정진섭차장)
                            strOpParam[2] = Info.strOOBFlag;
                            break;
                    }

                    bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);
                    
                    if (bRtnVal) //오라클 프로시져 호출 성공후
                    {
                        strErrMsg = String.Empty;
                        bCheckErrMsg = DKORACLE.GetFieldValue(opRes, "ERRMSG", ref strErrMsg);

                        if (bCheckErrMsg)
                        {
                            if (strErrMsg.ToUpper().Equals("TRUE")
                                    || strErrMsg.ToUpper().Equals("OK")
                                       || strErrMsg.ToUpper().Equals("SUCCESS"))
                            {

                                ExprSaveInfoData(iPort, opRes.strFieldname, opRes.strFieldValue);
                                GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), STATUS.OK.ToString(), String.Empty, true);
                                bRtnVal = true;
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMsg, String.Empty, true);
                                bRtnVal = false;
                            }

                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NO MESSAGE", String.Empty, true);
                            bRtnVal = false;
                        }

                    }
                    else
                    {   //오라클 프로시져 호출 실패

                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strReason, String.Empty, true);
                        bRtnVal = false;

                    }
                    return bRtnVal;
              
                case "GET_OOB_INFO":
                case "GET_OOB_INFO_PSA":
                case "SET_KEYWRITE_MAIN":
                                        
                    MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                    SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + strParam;
                    strOpParam = new string[1];
                    switch (strCmdType)
                    {      
                        case "GET_OOB_INFO": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GET_GSM_INFO;
                            #endif
                            strOpParam = new string[2];                            
                            strOpParam[0] = "AV"; //GEN10 패키지는 AV 로 fix (CNS 정진섭차장 - 문서에도 정의됨)
                            if (strParam.Length == 15)
                                strParam = strParam.Substring(0, 14);
                            strOpParam[1] = strParam; //GEN10 패키지 OOB 는 WIP이 아니라 IMEI 를 14자로 잘라서 올린다고 함...(CNS 정진섭차장) OMG
                            break;

                        case "GET_OOB_INFO_PSA": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO_PSA;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GET_GSM_INFO;
                            #endif
                            strOpParam = new string[2];
                            strOpParam[0] = "AV"; //GEN10 패키지는 AV 로 fix (CNS 정진섭차장 - 문서에도 정의됨)
                            if (strParam.Length == 15)
                                strParam = strParam.Substring(0, 14);
                            strOpParam[1] = strParam; //GEN10 패키지 OOB 는 WIP이 아니라 IMEI 를 14자로 잘라서 올린다고 함...(CNS 정진섭차장) OMG
                            break;

                        case "SET_KEYWRITE_MAIN": opRes.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_SET_MAIN;
                            //MOOHAN SERVER 임시 TEST 프로시져
                            #if USE_MOOHAN_SERVER
                            opRes.iProcedureIndex = (int)ProcedureIndex.GEN10_GM_SET_MAIN;
                            #endif
                            strOpParam = new string[10];
                            string[] strKeyValue = new string[8]; //CHECKSUM, VCERT, VPRIKEY, AUTHCODE, HASH, ICCID, IMSI, MSISDN
                            string strKeyReason = String.Empty;
                            if (GetKeyWriteValues(iPort, ref strKeyValue, ref strKeyReason))
                            {
                                for (int i = 0; i < strKeyValue.Length; i++)
                                {
                                    strOpParam[i + 1] = strKeyValue[i];
                                }                                
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NOT FOUND - " + strKeyReason, String.Empty, true);
                                return false;
                            }
                            strOpParam[0] = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                            strOpParam[9] = Info.strPCID;
                            break;
                    }
                    
                    bRtnVal = DKORACLE.ExcuteProcedure(ref opRes, ref strReason, strOpParam);
                    
                    if (bRtnVal) //오라클 프로시져 호출 성공후
                    {
                        strErrMsg       = String.Empty;
                        strDtlErrMsg    = String.Empty;
                        bCheckErrMsg    = DKORACLE.GetFieldValue(opRes, "ERRMSG", ref strErrMsg);
                        bCheckDtlErrMsg = DKORACLE.GetFieldValue(opRes, "ERRMSG_DTL", ref strDtlErrMsg);

                        if (bCheckErrMsg /*&& bCheckDtlErrMsg*/) //bCheckDtlErrMsg 가 없는 프로시져도 있다... ㅡㅡ
                        {
                            if (strErrMsg.ToUpper().Equals("TRUE")
                                    || strErrMsg.ToUpper().Equals("OK")
                                       || strErrMsg.ToUpper().Equals("SUCCESS"))
                            {

                                if (strCmdType.Equals("GET_OOB_INFO") || strCmdType.Equals("GET_OOB_INFO_PSA"))
                                {
                                    string strOOBFlag = String.Empty;
                                    bool bCheckOOBFlag = DKORACLE.GetFieldValue(opRes, "OOB_TEST_YN", ref strOOBFlag);

                                    if (bCheckOOBFlag)
                                    {                                      
                                        if (Info.bDontCareOOBcode || strOOBFlag.Equals(Info.strOOBCode))
                                        {
                                            ExprSaveInfoData(iPort, opRes.strFieldname, opRes.strFieldValue);
                                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), STATUS.OK.ToString(), String.Empty, true);
                                            bRtnVal = true;
                                        }
                                        else
                                        {
                                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NG OOB Flag(" + Info.strOOBCode + " - " + strOOBFlag + ")", String.Empty, true);
                                            bRtnVal = false;
                                        }
                                    }
                                    else
                                    {
                                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), "NO DATA (OOB Flag)", String.Empty, true);
                                        bRtnVal = false;
                                    }

                                }
                                else
                                {
                                    ExprSaveInfoData(iPort, opRes.strFieldname, opRes.strFieldValue);
                                    GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), STATUS.OK.ToString(), String.Empty, true);
                                    bRtnVal = true;
                                }
                            }
                            else
                            {
                                string strErrMessage = strDtlErrMsg;
                                if (bCheckDtlErrMsg) strErrMessage += "(" + strDtlErrMsg + ")";
                                GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMessage, String.Empty, true);
                                bRtnVal = false;
                            }

                        }
                        else
                        {
                            string strErrMessage = "NO ERROR MESSAGE";
                            if (bCheckDtlErrMsg) strErrMessage += "(DTL:" + strDtlErrMsg + ")";
                            GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strErrMessage, String.Empty, true);
                            bRtnVal = false;
                        }

                    }
                    else
                    {   //오라클 프로시져 호출 실패

                        GateWayMsgProcess((int)STATUS.MESERR, STATUS.MESERR.ToString(), strReason, String.Empty, true);
                        bRtnVal = false;

                    }
                    return bRtnVal;

                default:
                    GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "UNKNOWN COMMAND", String.Empty, true);
                    return false;
            }
        }

        public bool GetNetworkPingTest(string strIpaddress, ref string strResult)
        {
            int iTTL = 0;
            long lTime = 0;
            return DKLoggerPC.NetworkPingTest(strIpaddress, 2, ref strResult, ref iTTL, ref lTime);        
        }
#endregion

#region GMES 명령 관련

        public void   GmesConnection()
        {
            //if (DKGMES != null) DKGMES.GMES_OFF();       
            //DKGMES = new DK_GMES(Item_gMesHandle, WM_GMESDATA);

            string strGetText   = DKLoggerPC.LoadINI("OPTION", "MESON");
            string strSERVERIP  = DKLoggerPC.LoadINI("GMES", "SERVERIP");
            //CSMES
            if (STEPMANAGER_VALUE.bUseOSIMES)
            {
                DKGMES = null;

                if (DKOSIMES == null)
                    DKOSIMES = new DK_OSI_FOR_LGE(Item_gMesHandle, WM_GMESDATA);

                string strOSI_ID = DKLoggerPC.LoadINI("OSI", "ID");
                string strOSI_PassWord = DKLoggerPC.LoadINI("OSI", "PASSWORD");
                string strOSI_SiteCode = DKLoggerPC.LoadINI("OSI", "SITECODE");

                if (strGetText.Equals("ON"))
                {
                    if (DKOSIMES.GetConnStatus() != 0)
                    {
                        string retMsg = "";
                        //CSMES 에서 CONNECTION 시 JOB 파일이 선택되지 않았으면 ERROR 발생.
                        try
                        {
                            MessageLogging((int)LOGTYPE.PC, "GMES:CONNECT][TX] ID:" + strOSI_ID + " PW:" + strOSI_PassWord + " Site:" + strOSI_SiteCode, 0);
                            DKOSIMES.SetConfig(strOSI_SiteCode, strOSI_ID, strOSI_PassWord, out retMsg);
                            MessageLogging((int)LOGTYPE.PC, "GMES:CONNECT][RX] " + retMsg, 0);
                        }
                        catch
                        {
                            string strMsg = "GMES:CONNECT][FAIL, DLL ERROR : " + OSI_FOR_LGE_DLL.STR_LSFM_OSI_DLL_NAME + "] ";
                            MessageLogging((int)LOGTYPE.PC, strMsg, 0);
                            MessageBox.Show(strMsg);
                        }
                    }
                }
            }
            else
            {
                DKOSIMES = null;

                if (DKGMES != null) DKGMES.GMES_OFF();

                DKGMES = new DK_GMES(Item_gMesHandle, WM_GMESDATA);

                string strLOCALIP = DKLoggerPC.LoadINI("GMES", "LOCALIP");
                string strPORT = DKLoggerPC.LoadINI("GMES", "PORT");
                string strRETRY = DKLoggerPC.LoadINI("GMES", "RETRY");
                string strT3 = DKLoggerPC.LoadINI("GMES", "T3");
                string strT6 = DKLoggerPC.LoadINI("GMES", "T6");
                string strSPECDOWN = DKLoggerPC.LoadINI("GMES", "SPECDOWN");
                string strOnlyGmes = DKLoggerPC.LoadINI("OPTION", "ONLYOKGMES");

                bOnlyOKGmes = strOnlyGmes.Equals("ON");

                if (strGetText.Equals("ON"))
                {
                    if (DKGMES.GMES_GetStatus() != 2)
                    {
                        DKGMES.GMES_SetConfig(strRETRY, strT3, strT6, strSPECDOWN, strSERVERIP, strLOCALIP, strPORT);
                        DKGMES.GMES_ON();
                    }
                }
                else
                {
                    if (DKGMES.GMES_GetStatus() != 0)
                    {
                        DKGMES.GMES_OFF();
                    }
                }
            }
        }

        //CSMES
        public void OSIMes_SetStartTimeNCount(string strStartTime, int PassCount, int FailCount)
        {
            DKOSIMES.StartTime = strStartTime;
            DKOSIMES.PassCount = PassCount;
            DKOSIMES.FailCount = FailCount;
        }

        public int OSIMesDisConnect()
        {
            return DKOSIMES.Disconnect();
        }

        public int OSIMesConnStatus()
        {
            return DKOSIMES.GetConnStatus();
        }

        public string OSIMesGetErrString(int iErrCode)
        {
            return DKOSIMES.GetErrString(iErrCode);
        }

        public string OSIMesDllName()
        {
            return DKOSIMES.GetDLLName();
        }

        public string OSIMesDllVersion()
        {
            return DKOSIMES.GetVersion();
        }
        public string GmesProcInfo()
        {
            return DKGMES.GMES_GetProcName();
        }

        public string GmesEqpInfo()
        {
            return DKGMES.GMES_GetEqpName();
        }

        public string GmesDllVersion()
        {
            return DKGMES.GMES_GetVersion();
        }
        
#endregion      

#region NAD KEY DLL 관련

        private void NadKeyDllFileName(int iJobNum, string strCmdType, string strName)
        {
            string strDLLFileName = AppDomain.CurrentDomain.BaseDirectory + strName;
            if (DKLoggerPC.IsExistFile(strDLLFileName))
            {
                if(DK_NADKEY.SetDllFileName(strName))
                    GateWayMsgProcess((int)STATUS.OK, strName, strName, String.Empty, true);
                else
                    GateWayMsgProcess((int)STATUS.NG, "FAIL DLL LOAD", "FAIL DLL LOAD", String.Empty, true);
            }
            else
            {
                DK_NADKEY.UnloadLibrary();
                GateWayMsgProcess((int)STATUS.NG, "FILE NOT FOUND : " + strName, "FILE NOT FOUND : " + strName, String.Empty, true);
            }            
            
        }

        void NadKeyDllOpen(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            
            bool bOpen = DK_NADKEY.PortOpen((int)FixNadPort, 115200, 8, 0, 1, 0);
            if (bOpen)
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResultData = "PORT OPEN OK:" + FixNadPort.ToString();
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResultData = "PORT OPEN FAILURE:" + FixNadPort.ToString();
                resData.ResponseData = "NG";
            }
            resData.iPortNum = iPort;
            resData.SendPacket = "DLL_PORT_OPEN";  
            GateWay_MANAGER(resData);
        }

        void NadKeyDllClose(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            DK_NADKEY.PortClose();
            System.Threading.Thread.Sleep(1000);
   //         DK_NADKEY.UnloadLibrary();
            resData.iStatus = (int)STATUS.OK;
            resData.ResultData = "PORT CLOSE OK:" + FixNadPort.ToString();
            resData.ResponseData = "OK";            
            resData.iPortNum = iPort;
            resData.SendPacket = "DLL_PORT_CLOSE";
            GateWay_MANAGER(resData);
        }

        void NadKeyDllReadImei(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_READ_IMEI";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Read_IMEI(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }
            
            GateWay_MANAGER(resData);
        }

        void NadKeyDllWriteImei(int iPort, string strPar1)
        {           
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_WRITE_IMEI";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Write_IMEI(ref resData.ResultData, strPar1))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllCheckSumImei(int iPort, string strPar1)
        {   
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_WRITE_IMEI";
            resData.iPortNum = iPort;

            int iResValue = DK_NADKEY.CheckSum_IMEI(ref resData.ResultData, strPar1);
            if (iResValue != -999)
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = iResValue.ToString();
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = iResValue.ToString();
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllNvRestore(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_NV_RESTORE";
            resData.iPortNum = iPort;

            bool bResValue = DK_NADKEY.NV_Restore(ref resData.ResultData);
            if (bResValue)
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "SUCCESS";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "FAILURE";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllReadMsisdn(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_READ_MSIDSN";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Read_MSISDN(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllReadIccid(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_READ_ICCID";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Read_ICCID(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllReadImsi(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_READ_IMSI";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Read_IMSI(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllEFSBackup(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_EFS_BACKUP";
            resData.iPortNum = iPort;

            if (DK_NADKEY.EFS_Backup(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);

        }

        void NadKeyDllVersion(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_VERSION";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Get_DllVersion(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllReadSCNV(object ojb)
        {
            int iPort = (int)ojb;
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_VERSION";
            resData.iPortNum = iPort;

            if (DK_NADKEY.Read_SCNV(ref resData.ResultData))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void NadKeyDllNVGet(int iPort, int iPar1)
        {            
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = "DLL_NVGET";
            resData.iPortNum = iPort;

            if (DK_NADKEY.NVGet(ref resData.ResultData, iPar1))
            {
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "OK";
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NG";
            }

            GateWay_MANAGER(resData);
        }

        void KisDllKeyDownLoad(int iPort, VBFunction_in vbfIndata1, VBFunction_in vbfIndata2)
        {            
                  
            DK_KISDLL DK_KIS = new DK_KISDLL();
            DK_KIS.KISDLLlRealTimeTxRxMsg += new EventRealTimeMsg(InterChange_MANAGER);

            COMMDATA resData = new COMMDATA();
            resData.SendPacket = String.Empty;
            resData.iPortNum = iPort;
            resData.ResponseData = String.Empty;
            resData.ResultData   = String.Empty;

            bool bResValue            = false;
            VBFunction_out vbfGetData = new VBFunction_out();

            vbfGetData = DK_KIS.KeyDownLoad(ref bResValue, vbfIndata1, vbfIndata2, ref resData.SendPacket, ref resData.ResponseData, ref resData.ResultData);

            string[] strSubject = new string[10];
            string[] strData    = new string[10];

            //EXPR 에 KIS 서버에서 내려받은 데이터들을 저장하기 위해 초기화. ㅋㅋㅋ 움하하하하하항하하하핳ㅎㅎㅎ
            strSubject[0] = "KIS_error_code";       strData[0] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] =  vbfGetData.error_code.ToString("X4");    
            strSubject[1] = "KIS_error_message";    strData[1] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg]  =  vbfGetData.error_message;  
            strSubject[2] = "KIS_stid";             strData[2] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID]    =  vbfGetData.stid;           
            strSubject[3] = "KIS_rCert";            strData[3] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT]   =  vbfGetData.rCert;          
            strSubject[4] = "KIS_ccCert";           strData[4] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert]   =  vbfGetData.ccCert;         
            strSubject[5] = "KIS_vCert";            strData[5] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert]   =  vbfGetData.vCert;          
            strSubject[6] = "KIS_vPri";             strData[6] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri]    =  vbfGetData.vPri;           
            strSubject[7] = "KIS_vPre";             strData[7] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre]    =  vbfGetData.vPre;           
            strSubject[8] = "KIS_vAuth";            strData[8] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth]   =  vbfGetData.vAuth;          
            strSubject[9] = "KIS_vHash";            strData[9] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash]   =  vbfGetData.vHash;

            ExprWriteKisDownloadData(iPort, strSubject, strData);    

            if (bResValue)
            {
                resData.iStatus = (int)STATUS.OK;                
            }
            else
            {
                resData.iStatus = (int)STATUS.NG;                
            }

            GateWay_MANAGER(resData);
        }

        void AmsExeKeyDownLoad(int iPort, string strIp1, string strIp2, string strGen, string strProduct, string strStid)
        {           
            DK_LOGGER DKLoggerAMS = new DK_LOGGER("SET", false);
            DKLoggerAMS.SendTxRxEvent += new EventTxRxMsg(InterChange_Sub_Manager);
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = String.Empty;
            resData.iPortNum = iPort;
            resData.ResponseData = String.Empty;
            resData.ResultData = String.Empty;
             
            string[] strSubject = new string[10];
            string[] strData = new string[10];


            //call exe            

            string strWindows = String.Empty;
            string strSpace = " ";
            string[] strMakeParam = new string[10];

            strMakeParam[0] = "--apl=" + strIp1 + ":" + "8030";
            strMakeParam[1] = "--apl=" + strIp2 + ":" + "8030";

            strMakeParam[2] = "--timeout=10";
            strMakeParam[3] = "--retries=3";
            strMakeParam[4] = "--status=success";

            strMakeParam[5] = "--gen=" + strGen;

            strMakeParam[6] = strProduct;
            strMakeParam[7] = strStid;

            strMakeParam[8] = @"C:\GMTELEMATICS\LOG\AMS";


            bool amsFail = false;
            for (int amsLoop = 0; amsLoop < 2; amsLoop++)
            {                
                try
                {
                    string strExePath = AppDomain.CurrentDomain.BaseDirectory + @"\lib\AMSTest.exe";

                    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                    start.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\lib";
                    start.FileName = strExePath;
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    start.CreateNoWindow = true;
                    if (!amsFail)
                    {
                        start.Arguments = strMakeParam[0] + strSpace +
                                            strMakeParam[1] + strSpace +
                                            strMakeParam[2] + strSpace +
                                            strMakeParam[3] + strSpace +
                                            strMakeParam[4] + strSpace +
                                            strMakeParam[5] + strSpace +
                                            strMakeParam[6] + strSpace +
                                            strMakeParam[7] + strSpace +
                                            strMakeParam[8];
                    }
                    else
                    {
                        MessageLogging((int)LOGTYPE.TX, strIp2 + "," + strIp1 + "," + strGen + "," + strProduct + "," + strStid, iPort);
                        //IP1, 2 바꾸기
                        start.Arguments = strMakeParam[1] + strSpace +
                                            strMakeParam[0] + strSpace +
                                            strMakeParam[2] + strSpace +
                                            strMakeParam[3] + strSpace +
                                            strMakeParam[4] + strSpace +
                                            strMakeParam[5] + strSpace +
                                            strMakeParam[6] + strSpace +
                                            strMakeParam[7] + strSpace +
                                            strMakeParam[8];
                    }

                    //textBox1.Text;

                    System.IO.TextReader reader;

                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
                    {
                        using (reader = process.StandardOutput)
                        {
                            strWindows = reader.ReadToEnd();//richTextBox1.Text = reader.ReadToEnd();
                        }
                    }

                }
                catch { }

                if (String.IsNullOrEmpty(strWindows))
                {
                    resData.SendPacket = "AMSTest "; //
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "no response.";
                    resData.ResultData = "no response.";

                    amsFail = true;
                }
                else
                {
                    string[] strResonses = new string[1];
                    //string strResData = String.Empty;

                    strWindows = strWindows.Replace("\r", "");
                    strResonses = System.Text.RegularExpressions.Regex.Split(strWindows, "\n");

                    //EXPR 에 KIS 서버에서 내려받은 데이터들을 저장하기 위해 초기화. ㅋㅋㅋ 움하하하하하항하하하핳ㅎㅎㅎ
                    strSubject[0] = "KIS_error_code"; strData[0] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] = "";
                    strSubject[1] = "KIS_error_message"; strData[1] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg] = "";
                    strSubject[2] = "KIS_stid"; strData[2] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID] = "";
                    strSubject[3] = "KIS_rCert"; strData[3] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT] = "";
                    strSubject[4] = "KIS_ccCert"; strData[4] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert] = "";
                    strSubject[5] = "KIS_vCert"; strData[5] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert] = "";
                    strSubject[6] = "KIS_vPri"; strData[6] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri] = "";
                    strSubject[7] = "KIS_vPre"; strData[7] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre] = "";
                    strSubject[8] = "KIS_vAuth"; strData[8] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth] = "";
                    strSubject[9] = "KIS_vHash"; strData[9] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash] = "";


                    string strTotalFrame = String.Empty;

                    for (int i = 0; i < strResonses.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(strResonses[i]))
                            MessageLoggingEx(DKLoggerAMS, (int)LOGTYPE.RX, strResonses[i], iPort);

                        strTotalFrame += strResonses[i].Trim();

                    }

                    string[] strRxFrame = new string[1];
                    strRxFrame = strTotalFrame.Split('[');

                    int iFrameCount = (int)AMSRXSTEP.call;
                    bool bCheckData = true;
                    for (int i = 0; i < strRxFrame.Length; i++)
                    {
                        if (i <= (int)AMSRXSTEP.stid && String.IsNullOrEmpty(strRxFrame[i]))
                            continue;
                        switch (iFrameCount)
                        {
                            case (int)AMSRXSTEP.call: iFrameCount++; break;

                            case (int)AMSRXSTEP.error_code:
                                if (strRxFrame[i].IndexOf("1]error_code:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("1]error_code:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.error_message:
                                if (strRxFrame[i].IndexOf("2]error_message:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("2]error_message:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.stid:
                                if (strRxFrame[i].IndexOf("3]stid:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("3]stid:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.rCert:
                            case (int)AMSRXSTEP.ccCert:
                            case (int)AMSRXSTEP.vCert:
                            case (int)AMSRXSTEP.vPri:
                            case (int)AMSRXSTEP.vPre:
                            case (int)AMSRXSTEP.vAuth:
                            case (int)AMSRXSTEP.vHash:

                                string strFname = iFrameCount.ToString() + "]" + (AMSRXSTEP.call + iFrameCount).ToString() + ":";
                                string strFnameNext = (iFrameCount + 1).ToString() + "]" + (AMSRXSTEP.call + iFrameCount + 1).ToString() + ":";

                                if (strRxFrame[i].IndexOf(strFname).Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace(strFname, "");
                                    iFrameCount++;
                                }
                                else bCheckData = false;
                                break;

                            default:
                                bCheckData = false; break;
                        }

                    }

                    bool bCheckDownload = bCheckData;

                    if (bCheckDownload && strData[0].Equals("0"))
                    {
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] = strData[0];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg] = strData[1];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID] = strData[2];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT] = strData[3];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert] = strData[4];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert] = strData[5];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri] = strData[6];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre] = strData[7];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth] = strData[8];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash] = strData[9];

                        ExprWriteKisDownloadData(iPort, strSubject, strData);
                        resData.iStatus = (int)STATUS.OK;
                        amsFail = false;
                    }
                    else
                    {
                        string strGetCode = String.Empty;
                        strGetCode = strData[1].Replace("ERROR: rc = ", "");

                        resData.ResponseData = strData[1] + "(" + GetAMSErrorCode(strGetCode) + ")";
                        resData.ResultData = strData[1] + "(" + GetAMSErrorCode(strGetCode) + ")";
                        resData.iStatus = (int)STATUS.NG;
                        amsFail = true;
                    }

                }

                if (!amsFail) break;
            }

            GateWay_MANAGER(resData);
        }

        //패스워드 파일(pem) 파라미터 추가해달라고해서 신규 추가한 함수 , 2021.04.19 이동성책임 요청 strRoot, strIdentity, strpw
        void AmsExeKeyDownLoad(int iPort, string strName, string strIp1, string strIp2, string strId, string strGen, string strFactory, string strProduct, string strStid, int amsDataType)
        {            
            DK_LOGGER DKLoggerAMS = new DK_LOGGER("SET", false);
            DKLoggerAMS.SendTxRxEvent += new EventTxRxMsg(InterChange_Sub_Manager);
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = String.Empty;
            resData.iPortNum = iPort;
            resData.ResponseData = String.Empty;
            resData.ResultData = String.Empty;

            string[] strSubject = new string[10];
            string[] strData = new string[10];


            //call exe            

            string strWindows = String.Empty;
            string strSpace = " ";
            string[] strMakeParam = new string[14];


            strMakeParam[0] = "--name=" + strName;

            strMakeParam[1] = "--apl=" + strIp1 + ":" + "8030";
            strMakeParam[2] = "--apl=" + strIp2 + ":" + "8030";

            // 김종인 책임이 DLL 안에 포함하도록 했나봄. 2021.04.30
            //strMakeParam[3] = "--root=" + strRoot;
            //strMakeParam[4] = "--identity=" + strIdentity;
            //strMakeParam[5] = "--password=" + strpw;

            strMakeParam[3] = "--id=" + strId;

            strMakeParam[4] = "--timeout=10";
            strMakeParam[5] = "--retries=3";
            
            strMakeParam[6] = "--gen=" + strGen;

            strMakeParam[7] = "--factory=" + strFactory;

            strMakeParam[8] = strProduct;
            strMakeParam[9] = strStid;

            strMakeParam[10] = @"C:\GMTELEMATICS\LOG\AMS";
            bool amsFail = false;
            for (int amsLoop = 0; amsLoop < 2; amsLoop++)
            {
                try
                {

                    string strExePath = string.Empty;

                    if(amsDataType == (int)AMSTYPE.GEN12)
                        strExePath = AppDomain.CurrentDomain.BaseDirectory + @"lib\AMS_GEN12\AMSTest.exe";
                    else
                        strExePath = AppDomain.CurrentDomain.BaseDirectory + @"lib\AMSTest.exe";

                    string strWorkingDirectory = string.Empty;
                    if (amsDataType == (int)AMSTYPE.GEN12)
                        strWorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"lib\AMS_GEN12";
                    else
                        strWorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"lib";

                    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                    //start.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\lib";
                    start.WorkingDirectory = strWorkingDirectory;
                    start.FileName = strExePath;
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    start.CreateNoWindow = true;
                    if (!amsFail)
                    {
                        start.Arguments = strMakeParam[0] + strSpace +
                                        strMakeParam[1] + strSpace +
                                        strMakeParam[2] + strSpace +
                                        strMakeParam[3] + strSpace +
                                        strMakeParam[4] + strSpace +
                                        strMakeParam[5] + strSpace +
                                        strMakeParam[6] + strSpace +
                                        strMakeParam[7] + strSpace +
                                        strMakeParam[8] + strSpace +
                                        strMakeParam[9] + strSpace +
                                        strMakeParam[10];
                    }
                    else
                    {
                        MessageLogging((int)LOGTYPE.TX, strName + "," + strIp2 + "," + strIp1 + "," + strId + "," + strGen + "," + strFactory + "," + strProduct + "," + strStid, iPort);
                        start.Arguments = strMakeParam[0] + strSpace +
                                        strMakeParam[2] + strSpace +
                                        strMakeParam[1] + strSpace +
                                        strMakeParam[3] + strSpace +
                                        strMakeParam[4] + strSpace +
                                        strMakeParam[5] + strSpace +
                                        strMakeParam[6] + strSpace +
                                        strMakeParam[7] + strSpace +
                                        strMakeParam[8] + strSpace +
                                        strMakeParam[9] + strSpace +
                                        strMakeParam[10];
                    }                    

                    //textBox1.Text;

                    System.IO.TextReader reader;

                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
                    {
                        using (reader = process.StandardOutput)
                        {
                            strWindows = reader.ReadToEnd();//richTextBox1.Text = reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception E)
                {
                    string strEx = E.Message;
                }

                if (String.IsNullOrEmpty(strWindows))
                {
                    resData.SendPacket = "AMSTest "; //
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "no response.";
                    resData.ResultData = "no response.";
                    amsFail = true;
                }
                else
                {
                    string[] strResonses = new string[1];
                    //string strResData = String.Empty;

                    strWindows = strWindows.Replace("\r", "");
                    strResonses = System.Text.RegularExpressions.Regex.Split(strWindows, "\n");

                    //EXPR 에 KIS 서버에서 내려받은 데이터들을 저장하기 위해 초기화. ㅋㅋㅋ 움하하하하하항하하하핳ㅎㅎㅎ
                    strSubject[0] = "KIS_error_code"; strData[0] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] = "";
                    strSubject[1] = "KIS_error_message"; strData[1] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg] = "";
                    strSubject[2] = "KIS_stid"; strData[2] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID] = "";
                    strSubject[3] = "KIS_rCert"; strData[3] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT] = "";
                    strSubject[4] = "KIS_ccCert"; strData[4] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert] = "";
                    strSubject[5] = "KIS_vCert"; strData[5] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert] = "";
                    strSubject[6] = "KIS_vPri"; strData[6] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri] = "";
                    strSubject[7] = "KIS_vPre"; strData[7] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre] = "";
                    strSubject[8] = "KIS_vAuth"; strData[8] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth] = "";
                    strSubject[9] = "KIS_vHash"; strData[9] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash] = "";

                    string strTotalFrame = String.Empty;

                    for (int i = 0; i < strResonses.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(strResonses[i]))
                            MessageLoggingEx(DKLoggerAMS, (int)LOGTYPE.RX, strResonses[i], iPort);

                        strTotalFrame += strResonses[i].Trim();

                    }

                    string[] strRxFrame = new string[1];
                    strRxFrame = strTotalFrame.Split('[');

                    int iFrameCount = (int)AMSRXSTEP.call;
                    bool bCheckData = true;
                    for (int i = 0; i < strRxFrame.Length; i++)
                    {
                        if (i <= (int)AMSRXSTEP.stid && String.IsNullOrEmpty(strRxFrame[i]))
                            continue;
                        switch (iFrameCount)
                        {
                            case (int)AMSRXSTEP.call: iFrameCount++; break;

                            case (int)AMSRXSTEP.error_code:
                                if (strRxFrame[i].IndexOf("1]error_code:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("1]error_code:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.error_message:
                                if (strRxFrame[i].IndexOf("2]error_message:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("2]error_message:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.stid:
                                if (strRxFrame[i].IndexOf("3]stid:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("3]stid:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.rCert:
                            case (int)AMSRXSTEP.ccCert:
                            case (int)AMSRXSTEP.vCert:
                            case (int)AMSRXSTEP.vPri:
                            case (int)AMSRXSTEP.vPre:
                            case (int)AMSRXSTEP.vAuth:
                            case (int)AMSRXSTEP.vHash:

                                string strFname = iFrameCount.ToString() + "]" + (AMSRXSTEP.call + iFrameCount).ToString() + ":";
                                string strFnameNext = (iFrameCount + 1).ToString() + "]" + (AMSRXSTEP.call + iFrameCount + 1).ToString() + ":";

                                if (strRxFrame[i].IndexOf(strFname).Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace(strFname, "");
                                    iFrameCount++;
                                }
                                else bCheckData = false;
                                break;

                            default:
                                bCheckData = false; break;
                        }
                    }

                    bool bCheckDownload = bCheckData;

                    if (bCheckDownload && strData[0].Equals("0"))
                    {
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] = strData[0];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg] = strData[1];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID] = strData[2];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT] = strData[3];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert] = strData[4];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert] = strData[5];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri] = strData[6];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre] = strData[7];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth] = strData[8];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash] = strData[9];

                        ExprWriteKisDownloadData(iPort, strSubject, strData);
                        resData.iStatus = (int)STATUS.OK;
                        amsFail = false;
                    }
                    else
                    {
                        string strGetCode = String.Empty;
                        strGetCode = strData[1].Replace("ERROR: rc = ", "");

                        resData.ResponseData = strData[1] + "(" + GetAMSErrorCode(strGetCode) + ")";
                        resData.ResultData = strData[1] + "(" + GetAMSErrorCode(strGetCode) + ")";
                        resData.iStatus = (int)STATUS.NG;
                        amsFail = true;
                    }
                }

                if (!amsFail) break;
            }
            GateWay_MANAGER(resData);
        }

        //strFactory 를 안함.
        void AmsExeKeyDownLoad_GEN12(int iPort, string strName, string strIp1, string strIp2, string strId, string strGen, string strFactory, string strProduct, string strStid, int amsDataType)
        {
            DK_LOGGER DKLoggerAMS = new DK_LOGGER("SET", false);
            DKLoggerAMS.SendTxRxEvent += new EventTxRxMsg(InterChange_Sub_Manager);
            COMMDATA resData = new COMMDATA();
            resData.SendPacket = String.Empty;
            resData.iPortNum = iPort;
            resData.ResponseData = String.Empty;
            resData.ResultData = String.Empty;

            string[] strSubject = new string[10];
            string[] strData = new string[10];

            //call exe 
            string strWindows = String.Empty;
            string strSpace = " ";
            string[] strMakeParam = new string[13]; //GEN12 strFactory를 빼서 -1 함


            strMakeParam[0] = "--name=" + strName;

            strMakeParam[1] = "--apl=" + strIp1 + ":" + "8030";
            strMakeParam[2] = "--apl=" + strIp2 + ":" + "8030";

            // 김종인 책임이 DLL 안에 포함하도록 했나봄. 2021.04.30
            //strMakeParam[3] = "--root=" + strRoot;
            //strMakeParam[4] = "--identity=" + strIdentity;
            //strMakeParam[5] = "--password=" + strpw;

            strMakeParam[3] = "--root=" + "root_ca_cert_ams.pem";
            strMakeParam[4] = "--identity=" + "ams_agent_agt_apl_id.pem";
            strMakeParam[5] = "--password=" + "password-shrouded.txt";

            strMakeParam[6] = "--id=" + strId;

            strMakeParam[7] = "--timeout=10";
            strMakeParam[8] = "--retries=3";

            strMakeParam[9] = "--gen=" + strGen;

            //GEN12 제거
            //strMakeParam[7] = "--factory=" + strFactory;

            strMakeParam[10] = strProduct;
            strMakeParam[11] = strStid;

            strMakeParam[12] = @"C:\GMTELEMATICS\LOG\AMS";

            bool amsFail = false;
            for (int amsLoop = 0; amsLoop < 2; amsLoop++)
            {
                try
                {

                    string strExePath = string.Empty;

                    if (amsDataType == (int)AMSTYPE.GEN12)
                        strExePath = AppDomain.CurrentDomain.BaseDirectory + @"lib\AMS_GEN12\AMSTest.exe";
                    else
                        strExePath = AppDomain.CurrentDomain.BaseDirectory + @"lib\AMSTest.exe";

                    string strWorkingDirectory = string.Empty;
                    if (amsDataType == (int)AMSTYPE.GEN12)
                        strWorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"lib\AMS_GEN12";
                    else
                        strWorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"lib";

                    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                    //start.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\lib";
                    start.WorkingDirectory = strWorkingDirectory;
                    start.FileName = strExePath;
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    start.CreateNoWindow = true;
                    if (!amsFail)
                    {
                        start.Arguments = strMakeParam[0] + strSpace +
                                        strMakeParam[1] + strSpace +
                                        strMakeParam[2] + strSpace +
                                        strMakeParam[3] + strSpace +
                                        strMakeParam[4] + strSpace +
                                        strMakeParam[5] + strSpace +
                                        strMakeParam[6] + strSpace +
                                        //strMakeParam[7] + strSpace +    //strFactory 제거
                                        strMakeParam[7] + strSpace +
                                        strMakeParam[8] + strSpace +
                                        strMakeParam[9] + strSpace +
                                        strMakeParam[10] + strSpace +
                                        strMakeParam[11] + strSpace +
                                        strMakeParam[12];

                        MessageLogging((int)LOGTYPE.TX, start.Arguments, iPort);
                    }
                    else
                    {
                        //MessageLogging((int)LOGTYPE.TX, strName + "," + strIp2 + "," + strIp1 + "," + strId + "," + strGen + "," + strFactory + "," + strProduct + "," + strStid, iPort);
                        //MessageLogging((int)LOGTYPE.TX, strName + "," + strIp2 + "," + strIp1 + "," + strId + "," + strGen + "," + "," + strProduct + "," + strStid, iPort);
                                                
                        start.Arguments = strMakeParam[0] + strSpace +
                                        strMakeParam[2] + strSpace +
                                        strMakeParam[1] + strSpace +
                                        strMakeParam[3] + strSpace +
                                        strMakeParam[4] + strSpace +
                                        strMakeParam[5] + strSpace +
                                        strMakeParam[6] + strSpace +
                                        //strMakeParam[7] + strSpace +     //strFactory 제거
                                        strMakeParam[7] + strSpace +
                                        strMakeParam[8] + strSpace +
                                        strMakeParam[9] + strSpace +
                                        strMakeParam[10] + strSpace +
                                        strMakeParam[11] + strSpace +
                                        strMakeParam[12];

                        MessageLogging((int)LOGTYPE.TX, start.Arguments, iPort);
                    }

                    //textBox1.Text;

                    System.IO.TextReader reader;

                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
                    {
                        using (reader = process.StandardOutput)
                        {
                            strWindows = reader.ReadToEnd();//richTextBox1.Text = reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception E)
                {
                    string strEx = E.Message;
                }

                if (String.IsNullOrEmpty(strWindows))
                {
                    resData.SendPacket = "AMSTest "; //
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "no response.";
                    resData.ResultData = "no response.";
                    amsFail = true;
                }
                else
                {
                    string[] strResonses = new string[1];
                    //string strResData = String.Empty;

                    strWindows = strWindows.Replace("\r", "");
                    strResonses = System.Text.RegularExpressions.Regex.Split(strWindows, "\n");

                    //EXPR 에 KIS 서버에서 내려받은 데이터들을 저장하기 위해 초기화. ㅋㅋㅋ 움하하하하하항하하하핳ㅎㅎㅎ
                    strSubject[0] = "KIS_error_code"; strData[0] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] = "";
                    strSubject[1] = "KIS_error_message"; strData[1] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg] = "";
                    strSubject[2] = "KIS_stid"; strData[2] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID] = "";
                    strSubject[3] = "KIS_rCert"; strData[3] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT] = "";
                    strSubject[4] = "KIS_ccCert"; strData[4] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert] = "";
                    strSubject[5] = "KIS_vCert"; strData[5] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert] = "";
                    strSubject[6] = "KIS_vPri"; strData[6] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri] = "";
                    strSubject[7] = "KIS_vPre"; strData[7] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre] = "";
                    strSubject[8] = "KIS_vAuth"; strData[8] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth] = "";
                    strSubject[9] = "KIS_vHash"; strData[9] = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash] = "";

                    string strTotalFrame = String.Empty;

                    for (int i = 0; i < strResonses.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(strResonses[i]))
                            MessageLoggingEx(DKLoggerAMS, (int)LOGTYPE.RX, strResonses[i], iPort);

                        strTotalFrame += strResonses[i].Trim();

                    }

                    string[] strRxFrame = new string[1];
                    strRxFrame = strTotalFrame.Split('[');

                    int iFrameCount = (int)AMSRXSTEP.call;
                    bool bCheckData = true;
                    for (int i = 0; i < strRxFrame.Length; i++)
                    {
                        if (i <= (int)AMSRXSTEP.stid && String.IsNullOrEmpty(strRxFrame[i]))
                            continue;
                        switch (iFrameCount)
                        {
                            case (int)AMSRXSTEP.call: iFrameCount++; break;

                            case (int)AMSRXSTEP.error_code:
                                if (strRxFrame[i].IndexOf("1]error_code:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("1]error_code:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.error_message:
                                if (strRxFrame[i].IndexOf("2]error_message:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("2]error_message:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.stid:
                                if (strRxFrame[i].IndexOf("3]stid:").Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace("3]stid:", "");
                                    iFrameCount++;
                                }
                                else bCheckData = false; break;

                            case (int)AMSRXSTEP.rCert:
                            case (int)AMSRXSTEP.ccCert:
                            case (int)AMSRXSTEP.vCert:
                            case (int)AMSRXSTEP.vPri:
                            case (int)AMSRXSTEP.vPre:
                            case (int)AMSRXSTEP.vAuth:
                            case (int)AMSRXSTEP.vHash:

                                string strFname = iFrameCount.ToString() + "]" + (AMSRXSTEP.call + iFrameCount).ToString() + ":";
                                string strFnameNext = (iFrameCount + 1).ToString() + "]" + (AMSRXSTEP.call + iFrameCount + 1).ToString() + ":";

                                if (strRxFrame[i].IndexOf(strFname).Equals(0))
                                {
                                    strData[iFrameCount - 1] = strRxFrame[i].Replace(strFname, "");
                                    iFrameCount++;
                                }
                                else bCheckData = false;
                                break;

                            default:
                                bCheckData = false; break;
                        }
                    }

                    bool bCheckDownload = bCheckData;

                    if (bCheckDownload && strData[0].Equals("0"))
                    {
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode] = strData[0];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg] = strData[1];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID] = strData[2];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT] = strData[3];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert] = strData[4];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert] = strData[5];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri] = strData[6];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre] = strData[7];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth] = strData[8];
                        STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash] = strData[9];

                        ExprWriteKisDownloadData(iPort, strSubject, strData);
                        resData.iStatus = (int)STATUS.OK;
                        amsFail = false;
                    }
                    else
                    {
                        string strGetCode = String.Empty;
                        strGetCode = strData[1].Replace("ERROR: rc = ", "");

                        resData.ResponseData = strData[1] + "(" + GetAMSErrorCode(strGetCode) + ")";
                        resData.ResultData = strData[1] + "(" + GetAMSErrorCode(strGetCode) + ")";
                        resData.iStatus = (int)STATUS.NG;
                        amsFail = true;
                    }
                }

                if (!amsFail) break;
            }
            GateWay_MANAGER(resData);
        }



        private string GetAMSErrorCode(string strErrorCode)
        {
            switch (strErrorCode)
            {
                    case "0x0000": return "AMS_AGT_ERR_NONE";
                    case "0xAA00": return "AMS_AGT_ERR_BASE";
                    case "0xAA01": return "AMS_AGT_ERR_NULL_PTR";
                    case "0xAA02": return "AMS_AGT_ERR_BAD_PARAM";
                    case "0xAA03": return "AMS_AGT_ERR_MEMORY";
                    case "0xAA04": return "AMS_AGT_ERR_INITIALIZE_SYSTEM_FAIL";
                    case "0xAA05": return "AMS_AGT_ERR_SHUTDOWN_SYSTEM_FAIL";
                    case "0xAA06": return "AMS_AGT_ERR_CONNECT_FAIL";
                    case "0xAA07": return "AMS_AGT_ERR_INVALID_STORE";
                    case "0xAA08": return "AMS_AGT_ERR_STORE_EMPTY";
                    case "0xAA09": return "AMS_AGT_ERR_STORE_FULL";
                    case "0xAA0A": return "AMS_AGT_ERR_UNKNOWN_PRODUCT";
                    case "0xAA0B": return "AMS_AGT_ERR_REQUEST_TOO_LARGE";
                    case "0xAA0C": return "AMS_AGT_ERR_UNKNOWN_OID";
                    case "0xAA0D": return "AMS_AGT_ERR_SYSTEM_NOT_INITIALIZED";
                    case "0xAA0E": return "AMS_AGT_ERR_NO_MORE_APPLIANCE";
                    case "0xAA0F": return "AMS_AGT_ERR_NAME_INVALID_LENGTH";
                    case "0xAA10": return "AMS_AGT_ERR_INVALID_SERVICE";
                    case "0xAA11": return "AMS_AGT_ERR_ID_INVALID_LENGTH";
                    case "0xAA12": return "AMS_AGT_ERR_LOG_INVALID_LENGTH";
                    case "0xAA13": return "AMS_AGT_ERR_STATUS_ALREADY_SET";
                    case "0xAA14": return "AMS_AGT_ERR_APPLIANCE_GET_ASSETS";
                    case "0xAA15": return "AMS_AGT_ERR_APPLIANCE_PROCESS_LOGS";
                    case "0xAA16": return "AMS_AGT_ERR_INVALID_PRODUCT";
                    case "0xAA17": return "AMS_AGT_ERR_PRODUCT_REQUIRES_ACC";
                    case "0xAA18": return "AMS_AGT_ERR_INVALID_ACC_DATA";
                    case "0xAA19": return "AMS_AGT_ERR_MALFORMED_CUSTOM_STRING";
                    case "0xAA1A": return "AMS_AGT_ERR_LOGGING_NOT_INITIALIZED";
                    case "0xAA1B": return "AMS_AGT_ERR_APPLIANCE_CONNECT_FAIL";
                    case "0xAA1C": return "AMS_AGT_ERR_PROFILE_NOT_FOUND";
                    case "0xAA1D": return "AMS_AGT_ERR_PROFILE_NOT_ACTIVE";
                    case "0xAA1E": return "AMS_AGT_ERR_PROFILE_NOT_RUNNING";
                    case "0xAA1F": return "AMS_AGT_ERR_PROFILE_BUSY";
                    case "0xAA20": return "AMS_AGT_ERR_CLIENT_WAITED_SERVER_TOO_LONG";
                    case "0xAA21": return "AMS_AGT_ERR_INVALID_IDENTIFIERS";
                    case "0xAA22": return "AMS_AGT_ERR_APPLIANCE_PROCESS_KEYS_AND_LOGS";
                    case "0xAA23": return "AMS_AGT_ERR_ID_INVALID_FORMAT";
                    case "0xAA24": return "AMS_AGT_ERR_PARTIAL_SUCCESS";
                    case "0xAA25": return "AMS_AGT_ERR_UNKNOWN_DATATYPE";
                    case "0xAA26": return "AMS_AGT_ERR_MALFORMED_IDENTIFIER_STRING";
                    case "0xAA27": return "AMS_AGT_ERR_INVALID_BLOCK";
                    case "0xAA28": return "AMS_AGT_ERR_INVALID_DATATYPE";
                    case "0xAA29": return "AMS_AGT_ERR_PUSHED_KEY_EXISTS";
                    case "0xAA2A": return "AMS_AGT_ERR_MISSING_KEY_IDENTIFIER";
                    case "0xAA2B": return "AMS_AGT_ERR_ARRAY_TOO_SMALL";

                    case "0xAAFF": return "AMS_AGT_ERR_INTERNAL";

                    default: return "UNKNOWN_CODE";
            }
        }

#endregion


#region AUTO TEST 관련

        //마지막 NG 리스트 초기화 (연속불량나면 팝업용)
        public void ClearLastNGList()
        {
            try
            {
                LstLastNG.Clear();
            }
            catch 
            {
                LstLastNG = new List<string>();
            }
            
        }

        public int GetLastNGListCount()
        {
         
            try
            {
                return LstLastNG.Count;
            }
            catch 
            {
                return 0;	
            }
            
        }

        public void AddLastNGItem(string strItem)
        {
            try
            {
                LstLastNG.Add(strItem);
            }
            catch 
            {
            	
            }
            
        }

        public bool ContainItem(string strItem)
        {
            if (strItem.Length < 0 || LstLastNG == null || LstLastNG.Count == 0) return false;

            return LstLastNG.Contains(strItem);
        }

        //테스트 결과 CSV 파일에 저장시 장치 상태도 남기기 위해서 1
        private string GetDeviceName()
        {
            string strRes = "DIO,SET,5515C,SCANNER,PCAN,TC3000,MTP200,AUDIOSELECTOR,34410A,VECTOR,KEITHLEY,MTP120A";
            return strRes;
        }

        //테스트 결과 CSV 파일에 저장시 장치 상태도 남기기 위해서 2
        private string GetDeviceStatus()
        {
            string strRes = String.Empty;

            if (OpenDIO) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[0].iStatus).ToString() + "],";
            if (OpenSET) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[1].iStatus).ToString() + "],";
            if (Open5515C==(int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[2].iStatus).ToString() + "],";            
            if (OpenSCAN) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[3].iStatus).ToString() + "],";
            if (OpenPCAN == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[4].iStatus).ToString() + "],";
            if (OpenTC3000) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[5].iStatus).ToString() + "],";
            if (OpenMTP200 == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[6].iStatus).ToString() + "],";
            if (OpenAudio == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[7].iStatus).ToString() + "],";
            if (Open34410A == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[9].iStatus).ToString() + "],";
            if (OpenVector == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[10].iStatus).ToString() + "],";
            if (OpenKEITHLEY == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[12].iStatus).ToString() + "],";
            if (OpenMTP120A == (int)STATUS.OK) strRes += "[OK],"; else strRes += "[" + (STATUS.NONE + deviceStatus[(int)DEVSTATUSNUMBER.MTP120A].iStatus).ToString() + "],";

            return strRes;
        }

        //업데이트파일있으면 이름변경작업하기
        public void CheckUpdateFiles()
        {
            DKLoggerPC.ChangeUpdateFile();
        }

        public void DeleteOldFiles()
        {
            DKLoggerPC.DeleteOldFile();
        }

        public void GmesDisconnection()
        {
            //CSMES
            if (DKGMES != null)
                DKGMES.GMES_OFF();
        }

        public string GetScreenPath()
        {
            return DKLoggerPC.Item_ResultNGscreen;
        }

        //Bin 로그 파일에 저장하기
        public void WriteBinLogging(string[] strLog, string strFileName)
        {
            DKLoggerPC.WriteBinLogging(strLog, strFileName);
        }

        //SET 로그 파일에 저장하기
        public void WriteInspectionLogging(string[] strLog, string strFileName)
        {
            DKLoggerPC.WriteInspectionLogging(strLog, strFileName);
        }

        private void MakeLoggingHiddenList()
        {
            LstHiddenCommand.Clear();

            for (int i = 0; i < LstTBL_GEN10.Count; i++)
            {
                if (LstTBL_GEN10[i].PARPAC1.Equals("HIDDEN"))
                {
                    LstHiddenCommand.Add(LstTBL_GEN10[i].CMDNAME);
                }
            }

            for (int i = 0; i < LstTBL_GEN11.Count; i++)
            {
                if (LstTBL_GEN11[i].PARPAC2.Equals("HIDDEN"))
                {
                    LstHiddenCommand.Add(LstTBL_GEN11[i].CMDNAME);
                }
            }

            for (int i = 0; i < LstTBL_GEN12.Count; i++)
            {
                if (LstTBL_GEN12[i].PARPAC2.Equals("HIDDEN"))
                {
                    LstHiddenCommand.Add(LstTBL_GEN12[i].CMDNAME);
                }
            }

            for (int i = 0; i < LstTBL_TCP.Count; i++)
            {
                if (LstTBL_TCP[i].PARPAC2.Equals("HIDDEN"))
                {
                    LstHiddenCommand.Add(LstTBL_TCP[i].CMDNAME);
                }
            }
        }

        private bool bCheckLoggingHiddenList(string strCommandName)
        {
            if (LstHiddenCommand.Count < 1) return false;

            foreach (string strCmd in LstHiddenCommand)
            {
                if (strCmd.Equals(strCommandName))
                {
                    return true;
                }
            }

            return false;
        }

        //테스트 결과 CSV 파일에 저장하기
        public void WriteTestResult(string strJobName, string strStTime, string strEdtime, string strElapseTime, string strResultReason, string strProgVer, string strPass, string strFail, string strTotal, string strGmesStatus)
        {
            //string strGmesVer = DKGMES.GMES_GetVersion();
            //CSMES
            string strGmesVer = string.Empty;

            if (STEPMANAGER_VALUE.bUseOSIMES)
                strGmesVer = DKOSIMES.GetVersion();
            else
                strGmesVer = DKGMES.GMES_GetVersion();
            int iLogCount     = GetJOBListCount();
            string[] strTempString = new string[iLogCount + 17];
            bool bHidden = false;

            strTempString[0] = "*PROGRAM,VERSION," + strProgVer + ",";
            strTempString[1] = "*GMES,STATUS," + strGmesStatus + ",";

            //strTempString[2] = "*GMES,EQPINFO," + GmesEqpInfo() + ",";
            //strTempString[3] = "*GMES,PROCINFO," + GmesProcInfo() + ",";  
            if (STEPMANAGER_VALUE.bUseOSIMES)
            {
                strTempString[2] = "*GMES,EQPINFO," + OSIMesDllName() + ",";
                strTempString[3] = "*GMES,PROCINFO," + "" + ",";
            }
            else
            {
                strTempString[2] = "*GMES,EQPINFO," + GmesEqpInfo() + ",";
                strTempString[3] = "*GMES,PROCINFO," + GmesProcInfo() + ",";
            }

            strTempString[4] = "*GMES_DLL,VERSION," + strGmesVer + ",";
            strTempString[5] = "*JOB FILE,NAME," + strJobName + ",";
            strTempString[6] = "*JOB FILE,MAPPING," + STEPMANAGER_VALUE.bUseAutoJobOn.ToString() + ",";
            strTempString[7] = "*TEST COUNT,(P|F|T)," + strPass + "|" + strFail + "|" + strTotal + ",";
            strTempString[8] = "*TEST START," + strStTime + ",";
            strTempString[9] = "*TEST END," + strEdtime + ",";
            strTempString[10] = "*ELAPSED TIME," + strElapseTime + " sec,";
            strTempString[11] = "*TEST RESULT," + strResultReason + ",";
            if(Item_bUseBarcode)
                strTempString[12] = "*BARCODE," + Item_WIPID + ",";
            else 
                strTempString[12] = "*BARCODE,NOT USED,"; 

            strTempString[13] = "*DEVICE NAME," + GetDeviceName() + ",";
            strTempString[14] = "*DEVICE STATUS," + GetDeviceStatus() + ",";

            string strTmp = ",RESULT,MEAS,MIN,MAX,COMPARE,PAR1,EXPR,LAPSE";

            strTempString[15] = "*TEST ITEM" + strTmp;

            for (int i = 0; i < iLogCount; i++)
            {
                string strName = GetJOBString(i, (int)sIndex.DISPLAY);
                if (strName.Length < 1)
                {
                    strName = GetJOBString(i, (int)sIndex.CMD);
                }
                else
                {
                    strName = strName.Replace(",", "'");
                }

                //히든 명령인지 검사
                bHidden = bCheckLoggingHiddenList(GetJOBString(i, (int)sIndex.CMD));

                strTempString[16 + i] = "#" + strName + ",";

                for (int j = (int)DEFINES.SET1; j < (int)DEFINES.END; j++)   //1. 모든 슬롯수만큼 돌면서
                {
                    if (UseSlots[j])                        //2. 사용할 슬롯이며 오픈된 포트이며 
                    {
                        RESDATA tmpRes      = GetTestResultData(j, i);
                        string strMeas      = tmpRes.ResultData;                                        

                        if (strMeas != null)// && strRes.Length < 1)
                        {
                            strMeas = strMeas.Replace(",", "'");
                            strMeas = strMeas.Replace("\r", "(CR)");
                            strMeas = strMeas.Replace("\n", "(LF)");                            
                        }
                        else
                        {
                            strMeas = "NULL";
                        }
                        string strRes = (STATUS.NONE + tmpRes.iStatus).ToString();
                        
                        string strType = GetJOBString(i, (int)sIndex.TYPE);
                                                
                        //string strMin  = GetJOBString(i, (int)sIndex.MIN);
                        //string strMax  = GetJOBString(i, (int)sIndex.MAX);

                        string strMin = tmpRes.strChangeMin;
                        string strMax = tmpRes.strChangeMax;

                        if (String.IsNullOrEmpty(strMin)) strMin = GetJOBString(i, (int)sIndex.MIN);
                        if (String.IsNullOrEmpty(strMax)) strMax = GetJOBString(i, (int)sIndex.MAX);
                                               
                        string strComp = GetJOBString(i, (int)sIndex.COMPARE);
                        string strPar1 = GetJOBString(i, (int)sIndex.PAR1);
                        string strExpr = GetJOBString(i, (int)sIndex.EXPR);
                        string strOption = GetJOBString(i, (int)sIndex.OPTION);
                        string strLapse = tmpRes.LapseTime;

                        if (bHidden || strOption.Equals("HIDDEN"))
                        {
                            if (strMax.Length > 0)
                                strMax = "*****";
                            if (strMeas.Length > 0)
                                strMeas = "*****";
                            if (strMin.Length > 0)
                                strMin = "*****";
                            if (strPar1.Length > 0)
                                strPar1 = "*****";
                        }

                        if (tmpRes.iStatus.Equals((int)STATUS.SKIP))
                        {
                            strMeas = ""; strMin = ""; strMax = "";
                        }

                        if (strMin != null)// && strRes.Length < 1)
                        {
                            strMin = strMin.Replace(",", "'");                            
                        }

                        if (strMax != null)// && strRes.Length < 1)
                        {
                            strMax = strMax.Replace(",", "'");                            
                        }

                        if (strPar1 != null)// && strRes.Length < 1)
                        {
                            strPar1 = strPar1.Replace(",", "'");                            
                        }

                        if (strExpr != null)// && strRes.Length < 1)
                        {
                            strExpr = strExpr.Replace(",", "'");
                        }

                        strTempString[16 + i] += "[" + strRes + "],[" + strMeas + "],[" + strMin + "],[" + strMax + "],[" + strComp + "],[" + strPar1 + "],[" + strExpr + "]," + strLapse + ",";
                        
                    }
                    else
                    {
                        strTempString[16 + i] += "NOT_USED,";
                    }
                }

            }
            strTempString[strTempString.Length - 1] = Environment.NewLine;
            DKLoggerPC.WriteResultLog(strTempString, strResultReason);
            if (!strResultReason.Equals("OK"))
            {
                try
                {
                    string strReturnValue = Item_WIPID;
                    if (strReturnValue.Length < 1)
                    {
                        bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteLoad("#LOAD:WIPID", ref strReturnValue);
                        if (!bExpr || strReturnValue.Length < 1)
                        {
                            strReturnValue = "NONE";
                        }

                    }

                    DKLoggerPC.PrintScreenResult(strReturnValue);
                }
                catch(Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                }
            }
            Item_WIPID = String.Empty;
        }
        public string WriteTestResultUseCompleteDetail(int iLogCount, string strJobName, string strStTime, string strEdtime, string strElapseTime, string strResultReason, string strProgVer, string strPass, string strFail, string strTotal)
        {
            string strGmesVer = string.Empty;

            if (STEPMANAGER_VALUE.bUseOSIMES)
                strGmesVer = DKOSIMES.GetVersion();
            else
                strGmesVer = DKGMES.GMES_GetVersion();
            //int iLogCount = GetJOBListCount();
            string[] strTempString = new string[iLogCount + 17];
            bool bHidden = false;

            strTempString[0] = "*PROGRAM,VERSION," + strProgVer + ",";
            strTempString[1] = "*GMES,STATUS," + "NONE" + ",";

            //strTempString[2] = "*GMES,EQPINFO," + GmesEqpInfo() + ",";
            //strTempString[3] = "*GMES,PROCINFO," + GmesProcInfo() + ",";  
            if (STEPMANAGER_VALUE.bUseOSIMES)
            {
                strTempString[2] = "*GMES,EQPINFO," + OSIMesDllName() + ",";
                strTempString[3] = "*GMES,PROCINFO," + "" + ",";
            }
            else
            {
                strTempString[2] = "*GMES,EQPINFO," + GmesEqpInfo() + ",";
                strTempString[3] = "*GMES,PROCINFO," + GmesProcInfo() + ",";
            }

            strTempString[4] = "*GMES_DLL,VERSION," + strGmesVer + ",";
            strTempString[5] = "*JOB FILE,NAME," + strJobName + ",";
            strTempString[6] = "*JOB FILE,MAPPING," + STEPMANAGER_VALUE.bUseAutoJobOn.ToString() + ",";
            strTempString[7] = "*TEST COUNT,(P|F|T)," + strPass + "|" + strFail + "|" + strTotal + ",";
            strTempString[8] = "*TEST START," + strStTime + ",";
            strTempString[9] = "*TEST END," + strEdtime + ",";
            strTempString[10] = "*ELAPSED TIME," + strElapseTime + " sec,";
            strTempString[11] = "*TEST RESULT," + strResultReason + ",";
            if (Item_bUseBarcode)
                strTempString[12] = "*BARCODE," + Item_WIPID + ",";
            else
                strTempString[12] = "*BARCODE,NOT USED,";

            strTempString[13] = "*DEVICE NAME," + GetDeviceName() + ",";
            strTempString[14] = "*DEVICE STATUS," + GetDeviceStatus() + ",";

            string strTmp = ",RESULT,MEAS,MIN,MAX,COMPARE,PAR1,EXPR,LAPSE";

            strTempString[15] = "*TEST ITEM" + strTmp;

            for (int i = 0; i < iLogCount; i++)
            {
                string strName = GetJOBString(i, (int)sIndex.DISPLAY);
                if (strName.Length < 1)
                {
                    strName = GetJOBString(i, (int)sIndex.CMD);
                }
                else
                {
                    strName = strName.Replace(",", "'");
                }

                //히든 명령인지 검사
                bHidden = bCheckLoggingHiddenList(GetJOBString(i, (int)sIndex.CMD));

                strTempString[16 + i] = "#" + strName + ",";

                for (int j = (int)DEFINES.SET1; j < (int)DEFINES.END; j++)   //1. 모든 슬롯수만큼 돌면서
                {
                    if (UseSlots[j])                        //2. 사용할 슬롯이며 오픈된 포트이며 
                    {
                        RESDATA tmpRes = GetTestResultData(j, i);
                        string strMeas = tmpRes.ResultData;

                        if (strMeas != null)// && strRes.Length < 1)
                        {
                            strMeas = strMeas.Replace(",", "'");
                            strMeas = strMeas.Replace("\r", "(CR)");
                            strMeas = strMeas.Replace("\n", "(LF)");
                        }
                        else
                        {
                            strMeas = "NULL";
                        }
                        string strRes = (STATUS.NONE + tmpRes.iStatus).ToString();

                        string strType = GetJOBString(i, (int)sIndex.TYPE);

                        //string strMin  = GetJOBString(i, (int)sIndex.MIN);
                        //string strMax  = GetJOBString(i, (int)sIndex.MAX);

                        string strMin = tmpRes.strChangeMin;
                        string strMax = tmpRes.strChangeMax;

                        if (String.IsNullOrEmpty(strMin)) strMin = GetJOBString(i, (int)sIndex.MIN);
                        if (String.IsNullOrEmpty(strMax)) strMax = GetJOBString(i, (int)sIndex.MAX);

                        string strComp = GetJOBString(i, (int)sIndex.COMPARE);
                        string strPar1 = GetJOBString(i, (int)sIndex.PAR1);
                        string strExpr = GetJOBString(i, (int)sIndex.EXPR);
                        string strOption = GetJOBString(i, (int)sIndex.OPTION);
                        string strLapse = tmpRes.LapseTime;

                        if (bHidden || strOption.Equals("HIDDEN"))
                        {
                            if (strMax.Length > 0)
                                strMax = "*****";
                            if (strMeas.Length > 0)
                                strMeas = "*****";
                            if (strMin.Length > 0)
                                strMin = "*****";
                            if (strPar1.Length > 0)
                                strPar1 = "*****";
                        }

                        if (tmpRes.iStatus.Equals((int)STATUS.SKIP))
                        {
                            strMeas = ""; strMin = ""; strMax = "";
                        }

                        if (strMin != null)// && strRes.Length < 1)
                        {
                            strMin = strMin.Replace(",", "'");
                        }

                        if (strMax != null)// && strRes.Length < 1)
                        {
                            strMax = strMax.Replace(",", "'");
                        }

                        if (strPar1 != null)// && strRes.Length < 1)
                        {
                            strPar1 = strPar1.Replace(",", "'");
                        }

                        if (strExpr != null)// && strRes.Length < 1)
                        {
                            strExpr = strExpr.Replace(",", "'");
                        }

                        strTempString[16 + i] += "[" + strRes + "],[" + strMeas + "],[" + strMin + "],[" + strMax + "],[" + strComp + "],[" + strPar1 + "],[" + strExpr + "]," + strLapse + ",";

                    }
                    else
                    {
                        strTempString[16 + i] += "NOT_USED,";
                    }
                }

            }

            string strTestLog = string.Join(Environment.NewLine, strTempString);
            //CSMES
            DKLoggerPC.WriteDetailDataLogging(strTestLog);
            return strTestLog;
        }
        public void WriteTestResult2(string strJobName, string strStTime, string strEdtime, string strElapseTime, string strResultReason, string strProgVer, string strPass, string strFail, string strTotal)
        {
            ORACLEINFO info = new ORACLEINFO();
            info = GetOracleInfo();

            int iLogCount = GetJOBListCount();
            string[] strTempString = new string[iLogCount + 17];

            strTempString[0] = "*PROGRAM,VERSION," + strProgVer + ",";
            strTempString[1] = "*MES,ProductionLine," + info.strProductionLine + ",";
            strTempString[2] = "*MES,ProcessCode," + info.strProcessCode + ",";
            strTempString[3] = "*MES,CallType," + info.strCallType + ",";

            if (info.strCallType.Equals("OOB"))
            {
                if(info.bDontCareOOBcode)
                    strTempString[3] = "*MES,CallType," + info.strCallType + "(" + "DONTCARE OOBCODE - " + info.strOOBFlag + "),";
                else
                    strTempString[3] = "*MES,CallType," + info.strCallType + "(" + info.strOOBCode + " - " + info.strOOBFlag + "),";
            }

            strTempString[4] = "*MES,PC_ID," + info.strPCID + ",";
            strTempString[5] = "*JOB FILE,NAME," + strJobName + ",";
            strTempString[6] = "*JOB FILE,MAPPING," + STEPMANAGER_VALUE.bUseAutoJobOn.ToString() + ",";
            strTempString[7] = "*TEST COUNT,(P|F|T)," + strPass + "|" + strFail + "|" + strTotal + ",";
            strTempString[8] = "*TEST START," + strStTime + ",";
            strTempString[9] = "*TEST END," + strEdtime + ",";
            strTempString[10] = "*ELAPSED TIME," + strElapseTime + " sec,";
            strTempString[11] = "*TEST RESULT," + strResultReason + ",";
            if (Item_bUseBarcode)
                strTempString[12] = "*BARCODE," + Item_WIPID + ",";
            else
                strTempString[12] = "*BARCODE,NOT USED,";

            strTempString[13] = "*DEVICE NAME," + GetDeviceName() + ",";
            strTempString[14] = "*DEVICE STATUS," + GetDeviceStatus() + ",";

            string strTmp = ",RESULT,MEAS,MIN,MAX,COMPARE,PAR1,EXPR,LAPSE";

            strTempString[15] = "*TEST ITEM" + strTmp;

            for (int i = 0; i < iLogCount; i++)
            {
                string strName = GetJOBString(i, (int)sIndex.DISPLAY);
                if (strName.Length < 1)
                {
                    strName = GetJOBString(i, (int)sIndex.CMD);
                }
                else
                {
                    strName = strName.Replace(",", "'");
                }

                strTempString[16 + i] = "#" + strName + ",";

                for (int j = (int)DEFINES.SET1; j < (int)DEFINES.END; j++)   //1. 모든 슬롯수만큼 돌면서
                {
                    if (UseSlots[j])                        //2. 사용할 슬롯이며 오픈된 포트이며 
                    {
                        RESDATA tmpRes = GetTestResultData(j, i);
                        string strMeas = tmpRes.ResultData;

                        if (strMeas != null)// && strRes.Length < 1)
                        {
                            strMeas = strMeas.Replace(",", "'");
                            strMeas = strMeas.Replace("\r", "(CR)");
                            strMeas = strMeas.Replace("\n", "(LF)");
                        }

                        else
                        {
                            strMeas = "NULL";
                        }
                        string strRes = (STATUS.NONE + tmpRes.iStatus).ToString();

                        string strType = GetJOBString(i, (int)sIndex.TYPE);

                        //string strMin  = GetJOBString(i, (int)sIndex.MIN);
                        //string strMax  = GetJOBString(i, (int)sIndex.MAX);

                        string strMin = tmpRes.strChangeMin;
                        string strMax = tmpRes.strChangeMax;

                        if (String.IsNullOrEmpty(strMin)) strMin = GetJOBString(i, (int)sIndex.MIN);
                        if (String.IsNullOrEmpty(strMax)) strMax = GetJOBString(i, (int)sIndex.MAX);

                        string strComp = GetJOBString(i, (int)sIndex.COMPARE);
                        string strPar1 = GetJOBString(i, (int)sIndex.PAR1);
                        string strExpr = GetJOBString(i, (int)sIndex.EXPR);
                        string strLapse = tmpRes.LapseTime;

                        if (tmpRes.iStatus.Equals((int)STATUS.SKIP))
                        {
                            strMeas = ""; strMin = ""; strMax = "";
                        }

                        if (strMin != null)// && strRes.Length < 1)
                        {
                            strMin = strMin.Replace(",", "'");
                        }

                        if (strMax != null)// && strRes.Length < 1)
                        {
                            strMax = strMax.Replace(",", "'");
                        }

                        if (strPar1 != null)// && strRes.Length < 1)
                        {
                            strPar1 = strPar1.Replace(",", "'");
                        }

                        if (strExpr != null)// && strRes.Length < 1)
                        {
                            strExpr = strExpr.Replace(",", "'");
                        }

                        strTempString[16 + i] += "[" + strRes + "],[" + strMeas + "],[" + strMin + "],[" + strMax + "],[" + strComp + "],[" + strPar1 + "],[" + strExpr + "]," + strLapse + ",";

                    }
                    else
                    {
                        strTempString[16 + i] += "NOT_USED,";
                    }
                }

            }
            strTempString[strTempString.Length - 1] = Environment.NewLine;
            DKLoggerPC.WriteResultLog(strTempString, strResultReason);
            if (!strResultReason.Equals("OK"))
            {
                try
                {
                    string strReturnValue = GetLogWipId();

                    DKLoggerPC.PrintScreenResult(strReturnValue);
                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                }
            }

            Item_WIPID = String.Empty;
        }

        public string GetLogWipId()
        {
            string strReturnValue = Item_WIPID;

            try
            {
                
                if (strReturnValue.Length < 1)
                {
                    bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteLoad("#LOAD:WIPID", ref strReturnValue);
                    if (!bExpr || strReturnValue.Length < 1)
                    {
                        strReturnValue = "NONE";
                    }

                }

                return strReturnValue;
            }
            catch 
            {
                return "NONE";
            }

        }

        public bool GetExprData(string strKey, ref string strReturnValue)
        {
            return DKExpr[(int)DEFINES.SET1].ExcuteLoad(strKey, ref strReturnValue);
        }

        //테스트 시작전 TABLE 파일 유무 체크하기
        private bool CheckTBLFiles(ref string strMsg)
        {
            if (LstTBL_34410A.Count < 1)
            {
                strMsg = "34410A COMMAND TABLE COUNT : " + LstTBL_34410A.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_GEN9.Count < 1)
            {
                strMsg = "GEN9 COMMAND TABLE COUNT : " + LstTBL_GEN9.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_GEN10.Count < 1)
            {
                strMsg = "GEN10 COMMAND TABLE COUNT : " + LstTBL_GEN10.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_TCP.Count < 1)
            {
                strMsg = "TCP COMMAND TABLE COUNT : " + LstTBL_TCP.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_GEN11.Count < 1)
            {
                strMsg = "GEN11 COMMAND TABLE COUNT : " + LstTBL_GEN11.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_GEN11P.Count < 1)
            {
                strMsg = "GEN11P COMMAND TABLE COUNT : " + LstTBL_GEN11P.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_GEN12.Count < 1)
            {
                strMsg = "GEN12 COMMAND TABLE COUNT : " + LstTBL_GEN12.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_CCM.Count < 1)
            {
                strMsg = "CCM COMMAND TABLE COUNT : " + LstTBL_CCM.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_NAD.Count < 1)
            {
                strMsg = "NAD COMMAND TABLE COUNT : " + LstTBL_NAD.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_MCTM.Count < 1)
            {
                strMsg = "MCTM COMMAND TABLE COUNT : " + LstTBL_MCTM.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_ATT.Count < 1)
            {
                strMsg = "ATT COMMAND TABLE COUNT : " + LstTBL_ATT.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_DIO.Count < 1)
            {
                strMsg = "DIO COMMAND TABLE COUNT : " + LstTBL_DIO.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_5515c.Count < 1)
            {
                strMsg = "5515C COMMAND TABLE COUNT : " + LstTBL_5515c.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_KEITHLEY.Count < 1)
            {
                strMsg = "KEITHLEY COMMAND TABLE COUNT : " + LstTBL_KEITHLEY.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_PCAN.Count < 1)
            {
                strMsg = "PCAN COMMAND TABLE COUNT : " + LstTBL_PCAN.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_TC1400A.Count < 1)
            {
                strMsg = "TC1400A COMMAND TABLE COUNT : " + LstTBL_TC1400A.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_VECTOR.Count < 1)
            {
                strMsg = "VECTOR COMMAND TABLE COUNT : " + LstTBL_VECTOR.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_TC3000.Count < 1)
            {
                strMsg = "TC3000 COMMAND TABLE COUNT : " + LstTBL_TC3000.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_MTP200.Count < 1)
            {
                strMsg = "MTP200 COMMAND TABLE COUNT : " + LstTBL_MTP200.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_AUDIO.Count < 1)
            {
                strMsg = "DIO AUDIO SELECTOR COMMAND TABLE COUNT : " + LstTBL_AUDIO.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_ADC.Count < 1)
            {
                strMsg = "MHT ADC MODULE COMMAND TABLE COUNT : " + LstTBL_ADC.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_PWR.Count < 1)
            {
                strMsg = "ODA POWER COMMAND TABLE COUNT : " + LstTBL_PWR.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_DLLGATE.Count < 1)
            {
                strMsg = "DLLGATE COMMAND TABLE COUNT : " + LstTBL_DLLGATE.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }

            if (LstTBL_MELSEC.Count < 1)
            {
                strMsg = "MELSEC COMMAND TABLE COUNT : " + LstTBL_MELSEC.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }


            if (LstTBL_MTP120A.Count < 1)
            {
                strMsg = "MTP120A COMMAND TABLE COUNT : " + LstTBL_MTP120A.Count.ToString();
                return false;     //TBL 파일갯수가 0개 이상인가 체크
            }
            return true;
        }

        public void GMES_INIT()
        {
            GmesConnection();
        }

        public void GMES_OFF()
        {
            if (DKGMES != null) DKGMES.GMES_OFF();
        }

        private void ResetOOB()
        {
            DKOOB = new DK_OOB();            
            
        }

        private void ResetKalsData()
        {
            STEPMANAGER_VALUE.ClearPcanStatus();
            STEPMANAGER_VALUE.strKALS_SiteCode = String.Empty;            
            STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID   = String.Empty;
            STEPMANAGER_VALUE.strKALS_DID_SEED_FileName   = String.Empty;
            STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath   = String.Empty;

            STEPMANAGER_VALUE.bGen9GpsOldInfo = false;
            STEPMANAGER_VALUE.bGen10GpsOldInfo = false;
            STEPMANAGER_VALUE.bGen11CCMGpsInfo = false;
            //경민 추가
            //STEPMANAGER_VALUE.bGen12CCMGpsInfo = false;

            STEPMANAGER_VALUE.InitCcmGpsStructure();
            STEPMANAGER_VALUE.InitstGEN9GPSInfo();

            STEPMANAGER_VALUE.bGen9GpsNavOn = false;
            STEPMANAGER_VALUE.bGen10GpsNavOn = false;
            
            STEPMANAGER_VALUE.strGen10GpsNavild = "FF";
            STEPMANAGER_VALUE.strGen10GpsTTFF   = "-1";
            STEPMANAGER_VALUE.dGen10GpsLat = 0.0;
            STEPMANAGER_VALUE.dGen10GpsLon = 0.0;

            STEPMANAGER_VALUE.dGen9GpsLat = 0.0;
            STEPMANAGER_VALUE.dGen9GpsLon = 0.0;

            STEPMANAGER_VALUE.iGen9GpsCount = 0;
            STEPMANAGER_VALUE.iGen9GpsCn0Max = 0;
            STEPMANAGER_VALUE.iGen9GpsCn0Aver = 0;

            STEPMANAGER_VALUE.iAttGpsCount = 0;
            STEPMANAGER_VALUE.iAttGpsCn0Max = 0;
            STEPMANAGER_VALUE.iAttGpsCn0Aver = 0;

            STEPMANAGER_VALUE.iAttGnssCount = 0;
            STEPMANAGER_VALUE.iAttGnssCn0Max = 0;
            STEPMANAGER_VALUE.iAttGnssCn0Aver = 0;


            STEPMANAGER_VALUE.strAtcoLoggingPath = String.Empty;
            for(int i = 0; i < (int)EFILECODE.END; i++)
            {
                STEPMANAGER_VALUE.KIS_DATA[i] = String.Empty;
            }
            STEPMANAGER_VALUE.strTactTime = String.Empty;            
            STEPMANAGER_VALUE.OOBSimInfoClear();
            STEPMANAGER_VALUE.OOBServiceClear();
            STEPMANAGER_VALUE.GEN10APN_TABLE.Clear();

            STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
            STEPMANAGER_VALUE.iUploadBytesCountLength     = 0;
            STEPMANAGER_VALUE.iUploadBytesCountTotalSize  = 0;
            STEPMANAGER_VALUE.bMctmALDLData = null;
            STEPMANAGER_VALUE.bMctmALDLIndex = 0x00;

        }

        private void LoadJobMapping()
        {
            bool bRtn = DKLoggerPC.LoadAutoJob(ref LstTBL_JOBMAP);
        }

        public void LoadMtpLoss()
        {
            string strGetText = String.Empty;
            string strTitleText = String.Empty;
            try
            {
                for (int i = 0; i < (int)MTPLOSS.MAX; i++)
                {
                    strTitleText = "LOSS" + i.ToString().PadLeft(2, '0');
                    strGetText = DKLoggerMR.LoadINI("MTPLOSS", strTitleText);
                    bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteSave("#SAVE:" + strTitleText, strGetText);
                }
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);            
            }
        }

        public void LoadPlcAddress()
        {
            string strGetText = String.Empty;
            string strTitleText = String.Empty;
            try
            {
                for (int i = 0; i < (int)MTPLOSS.MAX; i++)
                {
                    strTitleText = "MELSEC" + i.ToString().PadLeft(2, '0');
                    strGetText = DKLoggerMR.LoadINI("PLC_ADDRESS", strTitleText);
                    bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteSave("#SAVE:" + strTitleText, strGetText);
                }
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
        }

        public void LoadModelFile()
        {
            LstTBL_Model.Clear();
            bool bLoadModel = DKLoggerMR.LoadModelFile(ref LstTBL_Model);
            
        }

        private void ClearTestResultData()
        {
            if (bTestStarted) return;

            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
            {
                LstTST_RES[i].Clear();
            }
        }

        //UI 에서 테스트시작시 시작가능한지 체크하기
        public void CommonInitializeFunc()
        {
            tmpMDN = new _NAMMgmnt();
            LoadJobMapping(); //JOB MAPPING 파일 로딩.
            ResetKalsData();
            LoadModelFile();
            LoadMtpLoss();
            LoadPlcAddress();
            ResetOOB();
            iNowJobNumber = 0;
            RETCOUNT_DIC.Clear();
            AVERAGE_DIC.Clear();
            //최종결과 리스트도 클리어한다.
            ClearTestResultData();
            LstDoc.Clear();
            DKACTOR.DefaultComPort((int)COMSERIAL.SET);
            GC.Collect();
        }

        public bool AreYouReady(ref string strErrMsg)
        {
            CommonInitializeFunc();
                        
            bool bReady = false;            
            if (LstJOB_CMD.Count < 1)
            {
                strErrMsg = "JOB LIST COUNT : " + LstJOB_CMD.Count.ToString();
                return false;     //JOB 파일갯수가 0개 이상인가 체크
            }
            else
            {               

                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                {

                    if (LstJOB_CMD[0].OPTION == "AVERAGE") //평균값 구하는 명령이면 리트라이에 +1 추가.
                    {
                        RETCOUNT_DIC.Add(i, (int.Parse(LstJOB_CMD[0].RETRY)) + 1);
                    }
                    else { RETCOUNT_DIC.Add(i, int.Parse(LstJOB_CMD[0].RETRY)); }

                    STPCHECK_DIC[i] = false;
                    RESULTDT_DIC[i].Clear();
                    RESPONSE_DIC[i].Clear();
                    SEQUENCE_DIC[i].Clear();
                    SENDPACK_DIC[i].Clear();
                    DKExpr[i].Clear();

                    for (int j = 0; j < LstJOB_CMD.Count; j++)
                    {
                        RESULTDT_DIC[i].Add(j, String.Empty);
                        RESPONSE_DIC[i].Add(j, String.Empty);
                        SENDPACK_DIC[i].Add(j, String.Empty);                        
                        SEQUENCE_DIC[i].Add(j, (int)STATUS.NONE);  //JOB 시퀀스 (슬롯별) 초기화.
                    }                    
                }
                LoadMtpLoss();
                LoadPlcAddress();
            }

            // TABLE FIEL 검사.
            if (!CheckTBLFiles(ref strErrMsg)) return false;

            //OpenDIO
            for (int i = 0; i < (int)DEFINES.END; i++) //사용슬롯이 1개 이상인가 체크
            {
                if (UseSlots[i])
                {
                    bReady = true;
                    bCmdDoneCheck[i] = false;  //한줄단위 실행 체크를 위해서 false (명령실행안함으로 셋팅)
                }
                else
                {
                    bCmdDoneCheck[i] = true;  //한줄단위 실행 체크를 위해서 true (명령실행한것으로 셋팅)
                }
            }

            if (!ConfigLoad("OPTION", devStepCheckMode))
            {
                if (!bReady)
                {
                    strErrMsg = "DIO CONNECTION CHECK!";                    
                    return false;
                }

                if (!OpenSET)
                {
                    strErrMsg = "SET COMPORT CHECK!";
                    return false;
                }
            }
            else
            {
                OpenDIO = true;
                OpenSET = true;
            }
                       

            if (Open5515C == (int)STATUS.NG)
            {
                strErrMsg = "5515C CONNECTION CHECK!";
                return false;
            }

            if (OpenKEITHLEY == (int)STATUS.NG)
            {
                strErrMsg = "KEITHLEY CONNECTION CHECK!";
                return false;
            }

            if (Open34410A == (int)STATUS.NG)
            {
                strErrMsg = "34410A CONNECTION CHECK!";
                return false;
            }
            
            if (OpenMTP200 == (int)STATUS.NG)
            {
                strErrMsg = "MTP200 CONNECTION CHECK!";
                return false;
            }

            if (OpenAudio == (int)STATUS.NG)
            {
                strErrMsg = "AUDIO SELECTOR CONNECTION CHECK!";
                return false;
            }

            if (OpenMTP120A == (int)STATUS.NG)
            {
                strErrMsg = "MTP120A CONNECTION CHECK!";
                return false;
            }

            bReady = false;

            if (OpenDIO && OpenSET) bReady = true;

            string tmpStrPri = DKLoggerPC.LoadINI("OPTION", "PRIMARY");

            switch (tmpStrPri)
            {
                case "ORACLE":
                    //MES ORACLE CHECK                      
                    if (STEPMANAGER_VALUE.bUseOracleOn)
                    {                        
                        if (!OracleConnection(false, ref strErrMsg))
                        {
                            bReady = false;
                            strErrMsg = "MES CONNECTION CHECK!";
                            return false;
                        }
                    }
                    
                    break;
                default:
                    //GMES CHECK
                    //CSMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        if (STEPMANAGER_VALUE.bUseMesOn)
                        {
                            if (DKOSIMES.GetConnStatus() != 0)
                            {
                                bReady = false;
                                strErrMsg = "GMES CONNECTION CHECK!";

                                if (LoadINI("OPTION", "USEPLC").Equals("ON"))
                                {
                                    int i = 0;
                                    while (true)
                                    {
                                        System.Threading.Thread.Sleep(100);
                                        Application.DoEvents();
                                        i++;
                                        if (i > 50) break;
                                        if (DKOSIMES.GetConnStatus() == 0)
                                        {
                                            bReady = true;
                                            break;
                                        }
                                    }
                                }
                                if (!bReady) return false;
                            }
                        }
                        else
                        {
                            DKOSIMES.MES_ON = false;
                        }
                    }
                    else
                    {
                        if (STEPMANAGER_VALUE.bUseMesOn)
                        {
                            if (DKGMES.GMES_GetStatus() != 2)
                            {
                                bReady = false;
                                strErrMsg = "GMES CONNECTION CHECK!";

                                if (LoadINI("OPTION", "USEPLC").Equals("ON"))
                                {
                                    int i = 0;
                                    while (true)
                                    {
                                        System.Threading.Thread.Sleep(100);
                                        Application.DoEvents();
                                        i++;
                                        if (i > 50) break;
                                        if (DKGMES.GMES_GetStatus() == 2)
                                        {
                                            bReady = true;
                                            break;
                                        }
                                    }
                                }
                                if (!bReady) return false;
                            }
                        }
                        else
                        {
                            DKGMES.GMES_OFF();
                        }
                    }
                    break;
            }
           
            ////AUTOJOB CHECK  
            //STEPMANAGER_VALUE.bUseAutoJobOn = false;
            //string strGetMapping = DKLoggerPC.LoadINI("OPTION", "JOBAUTOMAPPING");
            //if (strGetMapping.Equals("ON"))
            //{
            //    if (LstTBL_JOBMAP.Count < 1)
            //    {
            //        strErrMsg = "CHECK JOB MAPPING FILE!";
            //        return false;

            //    }
            //    else
            //    {
            //        STEPMANAGER_VALUE.bUseAutoJobOn = true;
            //    }
            //}
            //LGEVH 
            //AUTOJOB CHECK  
            STEPMANAGER_VALUE.bUseAutoJobOn = false;
            string strGetMapping = DKLoggerPC.LoadINI("OPTION", "JOBAUTOMAPPING");
            if (strGetMapping.Equals("ON"))
            {
                //if (LstTBL_JOBMAP.Count < 1)
                //{
                //    strErrMsg = "CHECK JOB MAPPING FILE!";
                //    return false;

                //}
                //else
                //{
                STEPMANAGER_VALUE.bUseAutoJobOn = true;
                //}
            }

            dExtPwrBootTime = -1;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 부팅딜레이를 위한 변수
            bExtOnPrimary   = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
            bExtOnKey       = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
            bExtOnBackup    = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
            bExtOffPrimary  = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
            bExtOffKey      = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수
            bExtOffBackup   = false;  //무인화에서 EXT PWR 사용시 GOTO 에 의한 RETRY 시 첫명령은 그냥 PASS 하기 위한 변수

            if (!bReady)
            {
                strErrMsg = "CHECK COMPORT.";
                return false;
            }

            AutoGotoQuery();
            if (bReady)
            {
                if (Item_WIPID != null && Item_WIPID.Length > 0) //바코드 스캐너로 읽었다면 EXPR 에 저장해놓자.
                {
                    ExprSaveData((int)DEFINES.SET1, "BARCODE", Item_WIPID);
                }

                if (Item_SUBID != null && Item_SUBID.Length > 0) //바코드 스캐너로 읽었다면 EXPR 에 저장해놓자.
                {
                    ExprSaveData((int)DEFINES.SET1, "BARCODE_SUB", Item_SUBID);
                }

            }
            return bReady;

        }

        private void AutoGotoQuery()
        {
            //GOTO문 자동 생성
            //LABEL 생성부터 GOTO 까지 찾아서 자동 개입
            //1. 레이블 찾고 레이블 카운트를 확인한다.       

            string strGotoName = String.Empty;

            List<LABELSRUCT> lstGotoLalels = new List<LABELSRUCT>();

            for (int i = 0; i < LstJOB_CMD.Count; i++)
            {
                if (LstJOB_CMD[i].CASENG.Contains("GOTO"))
                {
                    LABELSRUCT tmpStruct = new LABELSRUCT();
                    tmpStruct.strLableName = LstJOB_CMD[i].CASENG.Replace("GOTO:", String.Empty);
                    tmpStruct.iIndex = i;
                    lstGotoLalels.Add(tmpStruct);
                }
            }

            if (lstGotoLalels.Count <= 0)
                return;


            for (int j = 0; j < lstGotoLalels.Count; j++)
            {
                LABELSRUCT tmpStruct = new LABELSRUCT();
                tmpStruct = lstGotoLalels[j];
                for (int i = 0; i < LstJOB_CMD.Count; i++)
                {
                    if (LstJOB_CMD[i].ACTION.Equals("RUN") || LstJOB_CMD[i].ACTION.Equals("ENC"))
                    {                  
                        if (!String.IsNullOrEmpty(LstJOB_CMD[i].LABEL) &&
                                LstJOB_CMD[i].LABEL.Equals(tmpStruct.strLableName)) //A1, A2, A3 ...
                        {

                            strGotoName = "GOTO:" + LstJOB_CMD[i].LABEL;

                            for (int t = i + 1; t < tmpStruct.iIndex; t++)
                            {                                
                                switch (LstJOB_CMD[t].CASENG)
                                {                                    
                                    case "MES":
                                    case "EMPTY":
                                    case "STOP":
                                    case "MONITOR":
                                    break;

                                    default:
                                        //20241021  GOTO 는 LABEL 과 GOTO 사이의 CASENG 를 모두 GOTO 로 변경함, JUMP 는 우선순위가 되도록 변경하지 않음. 
                                        //if (LstJOB_CMD[t].ACTION.Equals("RUN") && !LstJOB_CMD[t].CASENG.Contains("GOTO"))
                                        if ((LstJOB_CMD[t].ACTION.Equals("RUN") || LstJOB_CMD[t].ACTION.Equals("ENC"))&& !LstJOB_CMD[t].CASENG.Contains("GOTO") && !LstJOB_CMD[t].CASENG.Contains("JUMP"))
                                        {
                                            JOBDATA tmpJobdata = LstJOB_CMD[t];
                                            tmpJobdata.CASENG = strGotoName;
                                            LstJOB_CMD[t] = tmpJobdata;
                                        }
                                        break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        public void PopupActive()
        {
            if (Item_bThisPop)
                tmpDKPage.MsgPopUpFocus();
        }

        public void PopupClose()
        {
            if (Item_bThisPop)
                tmpDKPage.MsgPopDown(true);
        }

        public int IsPCanDevice()
        {
            return OpenPCAN;
        }

        public bool IsPopUp()
        {
            return Item_bThisPop;
        }
        //UI 에서 테스트시작시 매니저 스타트
        public void AutoTestStart(string strJobName)
        {            

            if (swLapse.IsRunning) { swLapse.Stop(); }
            if (swTimer.IsRunning) { swTimer.Stop(); }
            swLapse.Reset();
            swTimer.Reset();

            iNowJobNumber = 0;
            bEngineOn = false;            
            strNowJobName = strJobName;
            DKACTOR.DefaultSetBaudrate();       
            //if (threadEngine == null)
            //{
            //    threadEngine = new Thread(CycleEngine);
            //}
            //else
            //{
                KillThreadObject(threadEngine);
                KillThreadObject(threadAmsKeyDLL);
                threadEngine = new Thread(CycleEngine);
            //}

            bEngineOn = true;
            threadEngine.Start();
           
        }

        public void ReleaseStepCheck()
        { //스텝체크 이력 삭제
            if (swLapse.IsRunning) { swLapse.Stop(); }
            if (swTimer.IsRunning) { swTimer.Reset(); }

            for (int i = 0; i < STPCHECK_DIC.Length; i++)
            {
                STPCHECK_DIC[i] = false;
            }

            if (DK_TC1400A.IsConnected()) DK_TC1400A.Disconnect(false);

            KillThreadObject(threadAmsKeyDLL);
            KillThreadObject(threadKisKeyDLL);

            if (DK_DLLGATE.IsConnected()) DK_DLLGATE.Disconnect(false);
        }

        //UI 에서 테스트종료시 매니터 종료
        public  void AutoTestStop()
        {            
            STEPMANAGER_VALUE.bDebugLogEnable = false;
            STEPMANAGER_VALUE.bGen9GpsOldInfo = false;
            STEPMANAGER_VALUE.bGen10GpsOldInfo = false;
            STEPMANAGER_VALUE.bGen11CCMGpsInfo = false;
            //경민 추가
            //STEPMANAGER_VALUE.bGen12CCMGpsInfo = false;
            STEPMANAGER_VALUE.InitCcmGpsStructure();

            STEPMANAGER_VALUE.bGen9GpsNavOn = false;
            STEPMANAGER_VALUE.strGen9GpsNavild = "FF";
            STEPMANAGER_VALUE.strGen9GpsTTFF = "-1";
            STEPMANAGER_VALUE.dGen9GpsLat = 0.0;
            STEPMANAGER_VALUE.dGen9GpsLon = 0.0;

            STEPMANAGER_VALUE.bGen10GpsNavOn = false;
            STEPMANAGER_VALUE.strGen10GpsNavild = "FF";
            STEPMANAGER_VALUE.strGen10GpsTTFF   = "-1";
            STEPMANAGER_VALUE.dGen10GpsLat = 0.0;
            STEPMANAGER_VALUE.dGen10GpsLon = 0.0;

            STEPMANAGER_VALUE.iGen9GpsCount = 0;
            STEPMANAGER_VALUE.iGen9GpsCn0Max = 0;
            STEPMANAGER_VALUE.iGen9GpsCn0Aver = 0;

            STEPMANAGER_VALUE.iAttGpsCount = 0;
            STEPMANAGER_VALUE.iAttGpsCn0Max = 0;
            STEPMANAGER_VALUE.iAttGpsCn0Aver = 0;

            STEPMANAGER_VALUE.iAttGnssCount = 0;
            STEPMANAGER_VALUE.iAttGnssCn0Max = 0;
            STEPMANAGER_VALUE.iAttGnssCn0Aver = 0;

            if (DK_TC1400A.IsConnected()) DK_TC1400A.Disconnect(false);

            ReleaseStepCheck();
            swLapse.Reset();
            swTimer.Reset();
            Item_OOBLABEL = "";
            bEngineOn = false;            
            Item_bTestStarted = false;            
            DKACTOR.runningAllStop();
            KillThreadObject(threadExcelComm);
            KillThreadObject(threadAmsKeyDLL);
            KillThreadObject(threadKisKeyDLL);
            

        }
        //테스트 결과물 가져오기
        public RESDATA GetTestResultData(int iSlotNum, int iSeqNum)
        {
            RESDATA rtnRes = new RESDATA();
            rtnRes.iPortNum = (int)STATUS.NULL;   //없다는 뜻으로 이해하자.            
            rtnRes.iStatus = (int)STATUS.NULL;    //없다는 뜻으로 이해하자.            
            rtnRes.ResultData = STATUS.NULL.ToString();//없다는 뜻으로 이해하자.   
            try
            {      
                for (int i = 0; i < LstTST_RES[iSlotNum].Count; i++) //슬롯별 최종결과 리스트에 중복되는 시퀀스번호가 있는지 검사.
                {
                    if (LstTST_RES[iSlotNum][i].iSequenceNum == iSeqNum)
                    {
                        return LstTST_RES[iSlotNum][i];
                    }
                }
               
                return rtnRes;
            }
            catch 
            {
                return rtnRes;
            }
            
        }

        //테스트 결과 데이터 카운트 갯수 리턴해주기
        public int GetTestResultDataCount(int iSlotNum)
        {
            try
            {
                return LstTST_RES[iSlotNum].Count;
            }
            catch 
            {
                return 0;
            }
            
        }

        //테스트 결과 데이터 리턴해주기
        public string GetResultData(int iPort, int iJobNum)
        {
            string strTmp = "NONE";
            if (iPort < (int)DEFINES.SET1 || iPort >= (int)DEFINES.END) return strTmp;
            if (iJobNum < 0 || iJobNum > RESULTDT_DIC[iPort].Count) return strTmp;
            
            try
            {
                return RESULTDT_DIC[iPort][iJobNum];
            }
            catch (System.Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
                return strTmp;
            }   
        }

        //테스트 결과 통신응답 리턴해주기
        public string GetResponseData(int iPort, int iJobNum)
        {
            string strTmp = "NONE";
            if (iPort < (int)DEFINES.SET1 || iPort >= (int)DEFINES.END) return strTmp;
            if (iJobNum < 0 || iJobNum > RESPONSE_DIC[iPort].Count) return strTmp;

            try
            {
                return RESPONSE_DIC[iPort][iJobNum];
            }
            catch (System.Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                return strTmp;
            }
        }

        //테스트시 송신했던 패킷 리턴해주기
        public string GetSendPackData(int iPort, int iJobNum)
        {
            string strTmp = "NONE";
            if (iPort < (int)DEFINES.SET1 || iPort >= (int)DEFINES.END) return strTmp;
            if (iJobNum < 0 || iJobNum > SENDPACK_DIC[iPort].Count) return strTmp;

            try
            {
                return SENDPACK_DIC[iPort][iJobNum];
            }
            catch (System.Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                return strTmp;
            }
        }

        //Actor에서 들어오는 정보를 취합하는 게이트웨이
        private void GateWay_MANAGER(COMMDATA cData)
        {
      
            if (!Item_bTestStarted && !Item_bInteractiveMode)
            {
                return;
            }

            if (cData.iStatus.Equals((int)STATUS.DELAYLAPSE))
            {
                try
                {
                    if (RESULTDT_DIC[cData.iPortNum][iNowJobNumber].Equals(cData.ResultData))
                    {  //같은시간이면 퍼포먼스를 위해 던지지 않는다.ㅋㅋ
                        return;
                    }
                }
                catch { }
            }

            UpdateDictionary((int)DICINDEX.ALL, cData.iPortNum, iNowJobNumber, cData);

            //인터랙티브모드시에는 전달할필요가있다.
            if (Item_bInteractiveMode || cData.iStatus.Equals((int)STATUS.DELAYLAPSE) || cData.iStatus.Equals((int)STATUS.TIMESTAMP))
            {
                CycleSignDone(cData.iPortNum, cData.iStatus, iNowJobNumber);
            }     
           
        }

        //Actor에서 들어오는 정보를 UI로 전달해주는 인터체인지
        private void InterChange_MANAGER(int iPort, string cParam)
        {
           ManagerSendReport2(iPort, cParam);
        }

        //Actor에서 들어오는 센서정보를 UI로 전달해주는 인터체인지
        private void LoadDioPinStatus()
        {
            PSENSOR_DIC.Clear();
            string strUsed = String.Empty;
            bool bUsed = false;
            for(int i = (int)DIOPIN.START; i <= (int)DIOPIN.PINUSE; i++ )
            {
                string tmpStr = (DIOPIN.START + i).ToString();
                strUsed = DKLoggerPC.LoadINI("DIOPIN", tmpStr);
                if (strUsed.Equals("ON")) { bUsed = true; }
                else { bUsed = false; }
                PSENSOR_DIC.Add(i, bUsed);
            }
        }

        private bool CheckSensorDic(int iPinName)
        {
            return PSENSOR_DIC[iPinName];
        }

        private void SetSensorDic(bool[] cParam)
        {

                if (cParam.Length.Equals((int)DIOPIN.PINUSE))
                {
                }
                else
                {
                    if (cParam.Length.Equals((int)DIOPIN.PINUSE + (int)DOUT.END))
                    {
                    }
                    else
                        return;
                }

            


                for (int i = (int)DIOPIN.START; i < cParam.Length; i++)
                {
                    if (cParam[i]) //ON
                    {
                        switch (RSENSOR_DIC[i])
                        {
                            case (int)SENSOR.OFF: RSENSOR_DIC[i] = (int)SENSOR.ON; break;
                            case (int)SENSOR.ON:  RSENSOR_DIC[i] = (int)SENSOR.ING; break;
                            case (int)SENSOR.ING: RSENSOR_DIC[i] = (int)SENSOR.ING; break;
                            default: RSENSOR_DIC[i] = (int)SENSOR.OFF; break;
                        }
                    }
                    else //OFF
                    {
                        RSENSOR_DIC[i] = (int)SENSOR.OFF;
                    }

                }     
           
                   
        }

        public int GetSensorDic(int iPinName)
        {
            return RSENSOR_DIC[iPinName];
        }
                
        private void InterChange_Sub_Manager(string cParam) //로깅할때 데이터가 다시 실시간으로 리턴되면 actor로 보내자.
        {            
            ManagerSendReport2((int)DEFINES.SET1, cParam);
        }

        private void InterChange_MANAGER2(bool[] cParam)
        {
            SetSensorDic(cParam);
            
            if (!PSENSOR_DIC[(int)DIOPIN.PINUSE]) return; //PIN 사용안하면 던질필요없음.

            if (cParam[(int)DIOPIN.STOP]) //STOP 은 무조건 던짐.
            {
                CycleSignAction((int)STATUS.BTNSTOP); return;
            }

            if (!Item_bTestStarted)
            {
                if (bUsePLCMode && GetSensorDic((int)DIOPIN.START) == (int)SENSOR.ON && !cParam[(int)DIOPIN.STOP])
                {
                    CycleSignAction((int)STATUS.PINSELECTSTART);
                    return;
                }

                if (bUsePLCMode && GetSensorDic((int)DIOPIN.START) == (int)SENSOR.OFF && GetSensorDic((int)DIOPIN.EXTERNAL) == (int)SENSOR.ING && 
                      GetSensorDic((int)DIOPIN.SETIN) == (int)SENSOR.ON && !cParam[(int)DIOPIN.STOP])
                {  //PLC 모드이고 외부파워를 쓰고 SETIN 이 최초로 들어왔다면 EXTERNAM POWER 릴레이를 다시 켜준다.(즉, D-SUB 리트라이로 올 경우에 전원인가를 해줘야하는것 때문에)
                    int iCmd = (int)STATUS.NONE;
                    for (int iRetry = 0; iRetry < 3; iRetry++)
                    {
                        iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RELAY_ON_PRIMARY_EXT", "", 0, (int)MODE.SENDRECV);                       
                        
                        if (iCmd == (int)STATUS.OK) break;
                    }   
                }

                for (int i = 0; i < (int)DIOPIN.PINUSE; i++)
                {
                    if (i != (int)DIOPIN.START && i != (int)DIOPIN.STOP && GetSensorDic(i) == (int)SENSOR.ON)
                    {   //START, IN1, IN2, IN3, BUB1, BUB2(INT/EXT), MANUAL1, MANUAL3, SET-IN, STOP

                        if (!bUsePLCMode)
                        {                           
                            if (CheckSensorDic(i))
                            {
                                CycleSignAction((int)STATUS.PINSELECTSTART);
                                return;
                            }   
                        }
                        
                    }                   
                }

            }
            else
            {
                for (int i = (int)DIOPIN.START; i < (int)DIOPIN.PINUSE; i++)
                {
                    if (bUsePLCMode)
                    {
                        if (i != (int)DIOPIN.STOP && GetSensorDic(i) == (int)SENSOR.OFF)
                        { //0.START, 1.SPARE1, 2.SPARE2, 3.MUTE, 4.M1, 5.M2, 6.BUB, 7.PALT, 8.SET, 9.STOP

                            if (!cParam[(int)DIOPIN.START])
                            {
                                CycleSignAction((int)STATUS.EJECT); return;
                            }

                        }
                    }
                    else
                    {
                        if (i != (int)DIOPIN.START && i != (int)DIOPIN.STOP && GetSensorDic(i) == (int)SENSOR.OFF)
                        { //0.START, 1.SPARE1, 2.SPARE2, 3.MUTE, 4.M1, 5.M2, 6.BUB, 7.PALT, 8.SET, 9.STOP
                            if (CheckSensorDic(i))
                            {
                                CycleSignAction((int)STATUS.EJECT); return;
                            }

                        }
                    }
                    
                }

            }

            if (!cParam[(int)DIOPIN.STOP] && !cParam[(int)DIOPIN.START]) //아무것도 아닌상태도 던져야함
            {
                CycleSignAction((int)STATUS.NONE); return;

            }

            if (cParam[(int)DIOPIN.START]) ////START는 무조건 던짐.
            {
                if (!bUsePLCMode)
                {
                    CycleSignAction((int)STATUS.BTNSTART); 
                }
            }
 
        }

        public string GetOtpPassword()
        {
            return DKLoggerPC.GetOtpPassword();
        }

        public void InitInspectionRetry()
        {
            string strUseRetry = "OFF";
            string strCount = "0";
            strUseRetry = LoadINI("OPTION", constUseInspectionRetry);
            strCount = LoadINI("OPTION", constUseInspectionRetryCount);

            if (strUseRetry.Equals("ON"))
            {
                try
                {
                    iInspectionRetryCount = int.Parse(strCount);
                }
                catch
                {
                    iInspectionRetryCount = 0;
                }
            }
            else
            {
                iInspectionRetryCount = 0;
            }
        }

        public void DecreaseInspectionRetry()
        {
            iInspectionRetryCount--;
        }

        public int CheckInspectionRetry()
        {
            string strUseRetry = "OFF";
            string strCount = "0";
            strUseRetry = LoadINI("OPTION", constUseInspectionRetry);
            strCount = LoadINI("OPTION", constUseInspectionRetryCount);
            if (strUseRetry.Equals("ON"))
            {
                return iInspectionRetryCount;
            }
            else
                return 0;
        }

        public bool CheckExtPwr()
        {
            int iStatus = GetSensorDic((int)DIOPIN.EXTERNAL);

            switch (iStatus)
            {
                case (int)SENSOR.ON:
                case (int)SENSOR.ING: 
                            return true;

                default: return false;
            }
        }

        private bool CheckMesCompleteOnStop()
        {
            string strGetText = DKLoggerPC.LoadINI("OPTION", "MESCOMPLETEONSTOP");
            if (!strGetText.Equals("ON"))
            {
                return false;
            }

            if (CheckInspectionRetry() > 0) return false;  //전체 리트라이가 있을경우 STEP COMPLETE는 무시하자. 170323

            //config 에 stop 나도 mes(oracle) or gmes 컴플리트를 할건지 옵션이 있으면 절차서에 컴플리트 명령으로 jump 하자.
            string strReason = String.Empty;

            if (!STEPMANAGER_VALUE.bUseMesOn && !STEPMANAGER_VALUE.bUseOracleOn) //MES ON 이 아니면 할필요없다.
                return false;

            for (int i = iNowJobNumber + 1; i < LstJOB_CMD.Count; i++)
            {
                if ((LstJOB_CMD[i].TYPE.Equals("GMES") || LstJOB_CMD[i].TYPE.Equals("MES")) 
                        && LstJOB_CMD[i].CMD.Equals("STEP_COMPLETE") &&
                            (LstJOB_CMD[i].ACTION.Equals("RUN") || LstJOB_CMD[i].ACTION.Equals("ENC")))
                {
                    iNowJobNumber = i;
                    return true;
                }
            }
            return false;

        }

        //PKS
        /*
        private void CycleSignJump()
        {
            bool bJumpAction = false;
            int iFindIdx = -1;

            if (swLapse.IsRunning) { swLapse.Stop(); }

            //절차 라인 수행 체크리스트 초기화
            for (int i = 0; i < bCmdDoneCheck.Length; i++)
            {
                bCmdDoneCheck[i] = true;
                if (UseSlots[i])
                {
                    bCmdDoneCheck[i] = false;
                }
            }

            if (LstJOB_CMD[iNowJobNumber].ACTION != "SKIP")
            {
                //1. 레이블 찾고 레이블 카운트를 확인한다.        
                string strJumpMode = LstJOB_CMD[iNowJobNumber].CASENG.Split(':')[0];
                string strJumpLabel = LstJOB_CMD[iNowJobNumber].CASENG.Split(':')[1];
                for (int i = 0; i < LstJOB_CMD.Count; i++)
                {
                    if (LstJOB_CMD[i].LABEL.Equals(strJumpLabel))
                    {
                        iFindIdx = i;
                        break;
                    }
                }

                if (iFindIdx == -1)
                {   //2. 레이블 찾지 못하면 종료한다.                
                    CycleSignAction((int)STATUS.JUMP_ERR);
                    return;
                }

                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)   //1. 모든 슬롯수만큼 돌면서
                {
                    if (UseSlots[i])                                         //2. 사용할 슬롯이며 오픈된 포트이며 
                    {
                        int iRemainLabelCount = Convert.ToInt32(LstJOB_CMD[iFindIdx].LABELCOUNT);

                        if (iRemainLabelCount > 0)
                        {
                            switch (strJumpMode)
                            {
                                case "JUMP_NG":
                                    if (SEQUENCE_DIC[i][iNowJobNumber] == (int)STATUS.NG)
                                    {
                                        RESDATA tmpRes = GetTestResultData(i, iNowJobNumber);
                                        tmpRes.iStatus = (int)STATUS.OK;
                                        //Change status
                                        //RESDATA GetTestResultData(int iSlotNum, int iSeqNum)
                                        for (int j = 0; j < LstTST_RES[i].Count; j++) //슬롯별 최종결과 리스트에 중복되는 시퀀스번호가 있는지 검사.
                                        {
                                            if (LstTST_RES[i][j].iSequenceNum == iNowJobNumber)
                                            {
                                                LstTST_RES[i][j] = tmpRes;
                                            }
                                        }
                                        bool btmp = ManagerSendReport(tmpRes);

                                        PreJumpProc(i, iFindIdx, tmpRes);
                                        bJumpAction = true;
                                    }
                                    break;
                                case "JUMP_OK":
                                    if (SEQUENCE_DIC[i][iNowJobNumber] == (int)STATUS.OK)
                                    {
                                        RESDATA tmpRes = GetTestResultData(i, iNowJobNumber);
                                        PreJumpProc(i, iFindIdx, tmpRes);
                                        bJumpAction = true;
                                    }
                                    break;
                            }

                            iRemainLabelCount--;
                            JOBDATA tmpJob = new JOBDATA();
                            tmpJob = LstJOB_CMD[iFindIdx];
                            tmpJob.LABELCOUNT = iRemainLabelCount.ToString();
                            LstJOB_CMD[iFindIdx] = tmpJob;
                        }
                    }
                }
            }
            else
            {
                CycleSignNext();
            }

            //if (bJumpAction)
            //    iNowJobNumber = iFindIdx;
            //else
            //    iNowJobNumber++;

            //if (iNowJobNumber < LstJOB_CMD.Count)
            //{   //리트라이 카운트 리로드 
            //    RETCOUNT_DIC.Clear();
            //    AVERAGE_DIC.Clear();
            //    for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
            //    {
            //        if (LstJOB_CMD[iNowJobNumber].OPTION == "AVERAGE") //평균값 구하는 명령이면 리트라이에 +1 추가.
            //        {
            //            RETCOUNT_DIC.Add(i, (int.Parse(LstJOB_CMD[iNowJobNumber].RETRY)) + 1);
            //        }
            //        else { RETCOUNT_DIC.Add(i, int.Parse(LstJOB_CMD[iNowJobNumber].RETRY)); }

            //    }
            //}
        }
        */

        private void CycleSignJump()
        {
            bool bJumpAction = false;
            int iFindIdx = -1;

            if (swLapse.IsRunning) { swLapse.Stop(); }

            //절차 라인 수행 체크리스트 초기화
            for (int i = 0; i < bCmdDoneCheck.Length; i++)
            {
                bCmdDoneCheck[i] = true;
                if (UseSlots[i])
                {
                    bCmdDoneCheck[i] = false;
                }
            }

            if (LstJOB_CMD[iNowJobNumber].ACTION != "SKIP")
            {
                //1. 레이블 찾고 레이블 카운트를 확인한다.        
                string strJumpMode = LstJOB_CMD[iNowJobNumber].CASENG.Split(':')[0];
                string strJumpLabel = LstJOB_CMD[iNowJobNumber].CASENG.Split(':')[1];
                for (int i = 0; i < LstJOB_CMD.Count; i++)
                {
                    if (LstJOB_CMD[i].LABEL.Equals(strJumpLabel))
                    {
                        iFindIdx = i;
                        break;
                    }
                }

                if (iFindIdx == -1)
                {   
                    //2. 레이블 찾지 못하면 종료한다.                   
                    CycleSignAction((int)STATUS.JUMP_ERR);
                    return;
                }

                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)   //1. 모든 슬롯수만큼 돌면서
                {
                    if (UseSlots[i])                                         //2. 사용할 슬롯이며 오픈된 포트이며 
                    {
                        int iRemainLabelCount = Convert.ToInt32(LstJOB_CMD[iFindIdx].LABELCOUNT);
                        RESDATA tmpRes = GetTestResultData(i, iNowJobNumber);      

                        if (tmpRes.ResponseData.Contains("ERR"))
                        {   
                            //PAR1 값이 없을 경우, PAR1 ERR 이면서 CHECK 로처리. 
                            CycleSignAction((int)STATUS.JUMP_ERR);
                            return;
                        }

                        if (iRemainLabelCount > 0)
                        {
                            switch (strJumpMode)
                            {
                                case "JUMP_NG":
                                    if (SEQUENCE_DIC[i][iNowJobNumber] == (int)STATUS.NG)
                                    {
                                        //Change status
                                        tmpRes.iStatus = (int)STATUS.OK;

                                        for (int j = 0; j < LstTST_RES[i].Count; j++) //슬롯별 최종결과 리스트에 중복되는 시퀀스번호가 있는지 검사.
                                        {
                                            if (LstTST_RES[i][j].iSequenceNum == iNowJobNumber)
                                            {
                                                LstTST_RES[i][j] = tmpRes;
                                            }
                                        }
                                        bool btmp = ManagerSendReport(tmpRes);

                                        PreJumpProc(i, iFindIdx);
                                        bJumpAction = true;
                                    }
                                    else {
                                        CycleSignNext();
                                    }
                                    break;
                                case "JUMP_OK":
                                    if (SEQUENCE_DIC[i][iNowJobNumber] == (int)STATUS.OK)
                                    {
                                        //RESDATA tmpRes = GetTestResultData(i, iNowJobNumber);
                                        PreJumpProc(i, iFindIdx);
                                        bJumpAction = true;
                                    }
                                    else
                                    {
                                        tmpRes.iStatus = (int)STATUS.OK;

                                        for (int j = 0; j < LstTST_RES[i].Count; j++) //슬롯별 최종결과 리스트에 중복되는 시퀀스번호가 있는지 검사.
                                        {
                                            if (LstTST_RES[i][j].iSequenceNum == iNowJobNumber)
                                            {
                                                LstTST_RES[i][j] = tmpRes;
                                            }
                                        }
                                        bool btmp = ManagerSendReport(tmpRes);
                                        CycleSignNext();
                                    }
                                    break;
                            }

                            //iRemainLabelCount--;
                            //JOBDATA tmpJob = new JOBDATA();
                            //tmpJob = LstJOB_CMD[iFindIdx];
                            //tmpJob.LABELCOUNT = iRemainLabelCount.ToString();
                            //LstJOB_CMD[iFindIdx] = tmpJob;
                        }
                    }
                }
            }
            else
            {
                CycleSignNext();
            }
        }

        //PKS
        private void PreJumpProc(int iSlot, int iFindIdx)
        {
            if (iFindIdx <= iNowJobNumber) //뒤로 점프 할때
            {
                //if (RETCOUNT_DIC[iSlot] > 0)   //Retry 수가 남아 있으면 Jump문을 수행한다.//Page문실행시 0으로 초기화되어서 적용안함
                //{
                for (int iJobNum = iFindIdx; iJobNum <= iNowJobNumber; iJobNum++)
                    SEQUENCE_DIC[iSlot][iJobNum] = (int)STATUS.NONE;

                //}
            }
            else  //앞으로 점프 할때
            {
                //for (int iJobNum = iNowJobNumber + 1; iJobNum < iFindIdx; iJobNum++)
                for (int iJobNum = iNowJobNumber; iJobNum < iFindIdx - 1; iJobNum++)
                {
                    //SEQUENCE_DIC[iSlot][iJobNum] = (int)STATUS.SKIP;
                    
                    CycleSignNext();

                    UpdateDictionary((int)DICINDEX.SEQUENCE, iSlot, iJobNum, (int)STATUS.SKIP);
                    bCmdDoneCheck[iSlot] = true;
                    CycleSignDone(iSlot, (int)STATUS.SKIP, iJobNum);
                    

                    //EvSetJumpResult(iJobNum, STATUS.JUMP);                    
                }

                CycleSignNext();
            }
        }

        //절차진행중 GOTO 에 따른 절차 후진 (BACK)
        private void CycleSignBack() 
        {
            //절차 라인 수행 체크리스트 초기화  
            for (int i = 0; i < bCmdDoneCheck.Length; i++)
            {
                bCmdDoneCheck[i] = true;
                if (UseSlots[i])
                {
                    bCmdDoneCheck[i] = false;
                }
            }
            //1. 레이블 찾고 레이블 카운트를 확인한다.        
            string strPosLabel = LstJOB_CMD[iNowJobNumber].CASENG;
            string strDesLabel = strPosLabel.Replace("GOTO:", String.Empty);
            bool bFind = false;
            int  iFindIdx = 0;
            for (int i = 0; i < iNowJobNumber; i++)
            {
                if (LstJOB_CMD[i].LABEL.Equals(strDesLabel))
                {                    
                    iFindIdx = i;
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {   //2. 레이블 찾지 못하면 종료한다.                
                CycleSignAction((int)STATUS.GOTO_ERR);
                return;
            }

            // 3. 성공 또는 SKIP 의 슬롯은 테스트 하지 못하도록 status를 바꾸고 그외는 테스트를 다시 할 수 있도록 NONE으로 설정.
            bool bAllTESTDONE = true;
            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)   //1. 모든 슬롯수만큼 돌면서
            {
                if (UseSlots[i])                        //2. 사용할 슬롯이며 오픈된 포트이며 
                {
                    if (SEQUENCE_DIC[i][iNowJobNumber] == (int)STATUS.OK ||
                        SEQUENCE_DIC[i][iNowJobNumber] == (int)STATUS.TESTDONE)
                    {
                        for (int j = 0; j <= iNowJobNumber; j++)
                        {
                            UpdateDictionary((int)DICINDEX.SEQUENCE, i, j, (int)STATUS.TESTDONE);
                        }
                    }
                    else
                    {
                        bAllTESTDONE = false;
                        for (int j = 0; j <= iNowJobNumber; j++)
                        {
                            switch (SEQUENCE_DIC[i][j])
                            {
                                case (int)STATUS.SKIP:
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, j, (int)STATUS.SKIP); break;
                                default:
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, j, (int)STATUS.NONE); break;
                            }
                        }
                    }                      
                }
            }

            //GOTO에 의해서 해결되지 못한 슬롯이 있으면 계속 반복을 위하여 넘버를 변경하여 계속 테스트를 진행할 수 있도록 한다.
            if (!bAllTESTDONE)
            {
                string strLabelCount = LstJOB_CMD[iFindIdx].LABELCOUNT;
                if (strLabelCount.Length < 1 || int.Parse(strLabelCount) < 1)
                {
                    if (!CheckMesCompleteOnStop())// true 면 그 함수에서 inowjobnumber 가 변경된다.    
                    {
                        if (LstJOB_CMD[iFindIdx].LABEL.IndexOf('C') != 0)
                            iNowJobNumber = LstJOB_CMD.Count; //GOTO 에서도 해결되지 못하면 멈추기.                            
                        else
                            iNowJobNumber++; //리트라이 횟수가 끝났으므로 다음 스텝으로 계속 진행한다.
                    }                 
                }
                else
                {   //LABEL COUNT 를 감소 시킨다.
                    iNowJobNumber       = iFindIdx;
                    strLabelCount       = (int.Parse(strLabelCount) - 1).ToString();
                    JOBDATA tmpJob      = new JOBDATA();
                    tmpJob = LstJOB_CMD[iFindIdx];
                    tmpJob.LABELCOUNT   = strLabelCount;
                    LstJOB_CMD[iFindIdx] = tmpJob;
                }                
            }
            else
            {
                iNowJobNumber++; //아니면 다음 스텝으로 계속 진행한다.
            }


            if (iNowJobNumber < LstJOB_CMD.Count)       
            {   //리트라이 카운트 리로드 
                RETCOUNT_DIC.Clear();
                AVERAGE_DIC.Clear();
                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                {
                    if (LstJOB_CMD[iNowJobNumber].OPTION == "AVERAGE") //평균값 구하는 명령이면 리트라이에 +1 추가.
                    {
                        RETCOUNT_DIC.Add(i, (int.Parse(LstJOB_CMD[iNowJobNumber].RETRY)) + 1);
                    }
                    else { RETCOUNT_DIC.Add(i, int.Parse(LstJOB_CMD[iNowJobNumber].RETRY)); }
                    
                }
            }
        }

        //절차진행중 일시정지
        private void CycleSignPause(bool bDefaultMsg, string strTitle, string strErrMsg)
        {
            bool btmp = false;
            string strT1 = String.Empty;
            string strM1 = String.Empty;

            if (!bDefaultMsg)
            {
                strT1 = "PAUSE";
                strM1 = LstJOB_CMD[iNowJobNumber].CMD.ToString();
            }
            else
            {
                strT1 = strTitle;
                strM1 = strErrMsg;
            }

            RESDATA tmpRES = new RESDATA();
            tmpRES.iType = (int)EVENTTYPE.MANAGER;
            tmpRES.iPortNum = 0;
            tmpRES.iSequenceNum = 0;
            tmpRES.iStatus = (int)STATUS.POPPING;
            btmp = ManagerSendReport(tmpRES);
            Item_bThisPop = true;
            int iVal = tmpDKPage.MsgPopUp(strT1, strM1, (int)POPBTNTYPE.CONTINUE);
            Item_bThisPop = false;
            tmpRES.iStatus = (int)STATUS.POPPINGOFF;
            btmp = ManagerSendReport(tmpRES);
            if (iVal == (int)STATUS.OK)  CycleSignNext();
            else CycleSignAction((int)STATUS.STOP); 
        }

        //절차진행중 해당 명령이 끝나고 다음으로 진행 (NEXT)
        private void CycleSignNext() 
        {
            if (swLapse.IsRunning) { swLapse.Stop();}

            //절차 라인 수행 체크리스트 초기화  
            for(int i = 0; i < bCmdDoneCheck.Length; i++){
                bCmdDoneCheck[i] = true;
                if (UseSlots[i])
                {
                    bCmdDoneCheck[i] = false;
                }
            }
                        
            iNowJobNumber++;

            if (iNowJobNumber < LstJOB_CMD.Count)           
            {   
                //리트라이 카운트 리로드 
                RETCOUNT_DIC.Clear();
                AVERAGE_DIC.Clear();
                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                {
                    try
                    {
                        if (LstJOB_CMD[iNowJobNumber].OPTION == "AVERAGE") //평균값 구하는 명령이면 리트라이에 +1 추가.
                        {
                            RETCOUNT_DIC.Add(i, (int.Parse(LstJOB_CMD[iNowJobNumber].RETRY)) + 1);
                        }
                        else { RETCOUNT_DIC.Add(i, int.Parse(LstJOB_CMD[iNowJobNumber].RETRY)); }
                    }
                    catch(Exception ex)
                    {
                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                        STEPMANAGER_VALUE.DebugView(strExMsg);
                    }                    
                }
            }
        }

        private void CheckButton(RESDATA tmpData)
        {
            bool btmp = false;
            //하드웨어 버튼 스위치 스타트버튼 후킹. 메시지 팝업창 끌때 사용.
            if (tmpData.iType == (int)EVENTTYPE.MANAGER)
            {
                if (tmpData.iStatus == (int)STATUS.BTNSTART) //스타트 버튼 눌렀을 경우 
                {
                    switch (tmpDKPage.isPopUp()) //팝업창 상태 확인
                    {
                        case (int)POPTYPE.NOPOP: break; // 팝업없으면 시작하고 있으므로 리턴
                        case (int)POPTYPE.POPONEBTN:
                        case (int)POPTYPE.POPTWOBTN:                           
                            if (GetSensorDic((int)DIOPIN.START) == (int)SENSOR.ON)
                            {
                                tmpDKPage.MsgPopDown(true);  //OK버튼클릭
                                STEPMANAGER_VALUE.bNowMsgPop = false;
                            } return;
                        default: break;
                    }

                    if (!Item_bTestStarted) //스타트 상태가 아니고
                    {
                        if (GetSensorDic((int)DIOPIN.START) == (int)SENSOR.ON) //최초 온이거나
                        {
                            btmp = ManagerSendReport(tmpData); //윈도우 메인폼으로 보내자.                        

                        }                    
                    }
                                
                    return;

                }
                else if (tmpData.iStatus == (int)STATUS.BTNSTOP) //스탑 버튼 눌렀을 경우 
                {                    

                    switch (tmpDKPage.isPopUp()) //팝업창 상태 확인
                    {
                        case (int)POPTYPE.NOPOP:
                            if (GetSensorDic((int)DIOPIN.STOP) == (int)SENSOR.ON) btmp = ManagerSendReport(tmpData); return;
                        case (int)POPTYPE.POPONEBTN: break;
                        case (int)POPTYPE.POPTWOBTN:
                            if (GetSensorDic((int)DIOPIN.STOP) == (int)SENSOR.ON)
                            {
                                tmpDKPage.MsgPopDown(false);  //NG버튼클릭
                                STEPMANAGER_VALUE.bNowMsgPop = false;
                            } return;
                        default: break;
                    }
                    return;
                }
                else if (tmpData.iStatus == (int)STATUS.EJECT) //스탑 버튼 눌렀을 경우 
                {
                    switch (tmpDKPage.isPopUp()) //팝업창 상태 확인
                    {
                        case (int)POPTYPE.NOPOP:
                                btmp = ManagerSendReport(tmpData); return;
                        case (int)POPTYPE.POPONEBTN: 
                        case (int)POPTYPE.POPTWOBTN:                            
                                tmpDKPage.MsgPopDown(true);  //OK버튼클릭
                                STEPMANAGER_VALUE.bNowMsgPop = false;
                                btmp = ManagerSendReport(tmpData); return;
                        default: break;
                    }
                    return;
                }

                else if (tmpData.iStatus == (int)STATUS.PINSELECTSTART)
                {
                    if (bUsePLCMode)
                    {
                        if (Item_bTestStarted) return;
                        switch (tmpDKPage.isPopUp()) //팝업창 상태 확인
                        {
                            case (int)POPTYPE.NOPOP: break; // 팝업없으면 시작하고 있으므로 리턴
                            case (int)POPTYPE.POPONEBTN:
                            case (int)POPTYPE.POPTWOBTN:
                                tmpDKPage.MsgPopDown(true);  //OK버튼클릭
                                STEPMANAGER_VALUE.bNowMsgPop = false; break;
                            default: break;
                        }
                        tmpData.iStatus = (int)STATUS.BTNSTART;
                        btmp = ManagerSendReport(tmpData);
                        return;//윈도우 메인폼으로 보내자.

                    }
                    else
                    {
                        if (tmpDKPage.isPopUp() != (int)POPTYPE.NOPOP || Item_bTestStarted)
                        {
                            return;
                        }
                        else
                        {
                            tmpData.iStatus = (int)STATUS.BTNSTART;
                            btmp = ManagerSendReport(tmpData);
                            return;//윈도우 메인폼으로 보내자. 
                        }
                    }
                }                
            }
            btmp = ManagerSendReport(tmpData);

        }

        //절차진행중 모든 명령 수행을 마치고 사이클 신호 처리1
        private void CycleSignAction(int iStatus) 
        {            
            RESDATA tmpData        = new RESDATA();
            tmpData.iType          = (int)EVENTTYPE.MANAGER;
            tmpData.iPortNum       = 0;
            tmpData.iSequenceNum   = 0;
            tmpData.iStatus        = (int)iStatus;
            tmpData.ResultData     = String.Empty;
            tmpData.strCMD         = String.Empty;
            tmpData.strDisplayName = String.Empty;
            CheckButton(tmpData);
        }

        //절차진행중 모든 명령 수행을 마치고 사이클 신호 처리2
        private void CycleSignAction(int iStatus, string strParam) 
        {            
            RESDATA tmpData        = new RESDATA();
            tmpData.iType          = (int)EVENTTYPE.MANAGER;
            tmpData.iPortNum       = 0;
            tmpData.iSequenceNum   = 0;
            tmpData.iStatus        = (int)iStatus;//(int)STATUS.END;
            tmpData.ResultData     = strParam;
            tmpData.strCMD         = String.Empty;
            tmpData.strDisplayName = String.Empty;     


            if (iStatus == (int)STATUS.CHANGEJOB)
            {
                tmpData.iType = (int)EVENTTYPE.MANAGER;
            }

            CheckButton(tmpData);
            
        }

        private bool CheckDelayLapseSign(int iPort, int iState, int iNowJobNum)
        {
            RESDATA tmpRes = new RESDATA();

            if (iState.Equals((int)STATUS.DELAYLAPSE))
            {
                tmpRes.iType = (int)EVENTTYPE.COMM;
                tmpRes.iStatus = iState;
                tmpRes.iPortNum = iPort;
                tmpRes.iSequenceNum = iNowJobNumber;
                tmpRes.ResultData   = RESULTDT_DIC[iPort][iNowJobNum];
                tmpRes.ResponseData = RESPONSE_DIC[iPort][iNowJobNum];
                tmpRes.strDisplayName = LstJOB_CMD[iNowJobNum].DISPLAYNAME;
                tmpRes.strCMDTYPE = LstJOB_CMD[iNowJobNum].TYPE;
                tmpRes.strCMD    = LstJOB_CMD[iNowJobNum].CMD;
                tmpRes.LapseTime = RESULTDT_DIC[iPort][iNowJobNum];
                bool btmp = ManagerSendReport(tmpRes);
                return true;
            }

            if (iState.Equals((int)STATUS.TIMESTAMP))
            {
                tmpRes.iType = (int)EVENTTYPE.COMM;
                tmpRes.iStatus = iState;
                tmpRes.iPortNum = iPort;
                tmpRes.iSequenceNum = iNowJobNumber;
                tmpRes.ResultData = RESULTDT_DIC[iPort][iNowJobNum];
                tmpRes.ResponseData = RESULTDT_DIC[iPort][iNowJobNum];
                tmpRes.strDisplayName = LstJOB_CMD[iNowJobNum].DISPLAYNAME;
                tmpRes.strCMDTYPE = LstJOB_CMD[iNowJobNum].TYPE;
                tmpRes.strCMD = LstJOB_CMD[iNowJobNum].CMD;
                tmpRes.LapseTime = swLapse.Elapsed.TotalSeconds.ToString("0.0");
                tmpRes.strChangeMin = RESPONSE_DIC[iPort][iNowJobNum];
                tmpRes.strChangeMax = SENDPACK_DIC[iPort][iNowJobNum];
                bool btmp = ManagerSendReport(tmpRes);
                return true;
            }

            return false;
        }
        
        //절차진행중 해당 명령(한줄)의 모든 액션이 끝나면 판정하여 UI로 결과를 던지는 루틴
        private void CycleSignDone(int iPort, int iState, int iNowJobNum)
        {
            if (CheckDelayLapseSign(iPort, iState, iNowJobNum)) return;

            RESDATA tmpRes = new RESDATA();
            DK_DECISION tmpDec = new DK_DECISION();

            bool btmp = false;

            if (iNowJobNum >= LstJOB_CMD.Count) return;
                        
            //AVERAGE 타입의 경우 명령의 결과가 OK가 아니더라도 성공한 자료가 한개이상 있으면 판정할수 있도록 한다.
            if (LstJOB_CMD[iNowJobNum].OPTION == "AVERAGE" && AVERAGE_DIC.Count > 0)
            {
                iState = (int)STATUS.OK;
            }

            //STATUS.OK 인경우만 판정전에 EXPR 에 계산식이나 다른 사항이 있는지 검사
            if (iState == (int)STATUS.OK) CheckExpression(iPort, iNowJobNum);

            string strComPare = LstJOB_CMD[iNowJobNum].COMPARE;
            string strMin = String.Empty;
            string strMax = String.Empty;
            string strNgcase = LstJOB_CMD[iNowJobNum].CASENG;
            string strChangeMaxString = String.Empty;
            string strChangeMinString = String.Empty;

            if (!iState.Equals((int)STATUS.SKIP)) // SKIP 이면 그냥 SKIP으로 돌려주자.
            {
                //판정전에 MIN 값에 EXPR 이나 GMES INSP 를 필요로 하는지 검사
                if (!CheckMinValue(iPort, iNowJobNum, ref strMin))
                {
                    UpdateDictionary((int)DICINDEX.RESPONSE, iPort, iNowJobNum, "MIN ERROR : " + LstJOB_CMD[iNowJobNum].MIN);
                    UpdateDictionary((int)DICINDEX.SEQUENCE, iPort, iNowJobNum, (int)STATUS.NG);
                }


                //판정전에 MAX 값에 EXPR 이나 GMES INSP 를 필요로 하는지 검사
                if (!CheckMaxValue(iPort, iNowJobNum, ref strMax))
                {
                    UpdateDictionary((int)DICINDEX.RESPONSE, iPort, iNowJobNum, "MAX ERROR : " + LstJOB_CMD[iNowJobNum].MAX);
                    UpdateDictionary((int)DICINDEX.SEQUENCE, iPort, iNowJobNum, (int)STATUS.NG);
                }
            }            

            strChangeMaxString = strMax;
            strChangeMinString = strMin;

            int itmpOption = ConvertOption(LstJOB_CMD[iNowJobNum].OPTION);
            tmpRes.iType = (int)EVENTTYPE.COMM;
            tmpRes.iPortNum = iPort;
            tmpRes.iSequenceNum = iNowJobNumber;
            tmpRes.strCMDTYPE = LstJOB_CMD[iNowJobNum].TYPE;
                       
            if (LstJOB_CMD[iNowJobNum].OPTION == "AVERAGE" && AVERAGE_DIC.Count > 0)
            {
                try
                {
                    double tmpDouble = 0.0;
                    for (int ai = 0; ai < AVERAGE_DIC.Count; ai++)
                    {
                        tmpDouble += double.Parse(AVERAGE_DIC[ai]);
                    }
                    if (AVERAGE_DIC.Count > 0)
                    {
                        tmpDouble = (double)tmpDouble / (double)AVERAGE_DIC.Count;
                    }
                    if (tmpDouble < 1)
                    {
                        tmpRes.ResultData = tmpDouble.ToString("0.###");
                    }
                    else
                    {
                        tmpRes.ResultData = tmpDouble.ToString("0.###");
                    }

                    try
                    {   // AVERAGE 경우도 옵셋값이 필요한경우가 있다.
                        string strExprMathData = String.Empty;
                        bool bExpr = DKExpr[iPort].ExcuteMath(LstJOB_CMD[iNowJobNum].EXPR, tmpRes.ResultData, ref strExprMathData);
                        if (bExpr) tmpRes.ResultData = strExprMathData;
                    }
                    catch { }

                }
                catch (System.Exception ex)
                {
                    string strEx = ex.Message;
                    tmpRes.ResultData = "VALUE TYPE ERROR.";
                }
            }
            else { tmpRes.ResultData = RESULTDT_DIC[iPort][iNowJobNum]; }


            string strReplaceData = String.Empty;

            if (LstJOB_CMD[iNowJobNum].OPTION == "NORESPONSE" && iState.Equals((int)STATUS.OK))            
            {
                tmpRes.iStatus = (int)STATUS.OK;
                tmpRes.ResponseData = tmpRes.ResultData = "";
            }
            else
            {
                bool bByPassAscii = LstJOB_CMD[iNowJobNum].TYPE.Equals("SCAN");

                tmpRes.iStatus = tmpDec.DecideData(iState, RESPONSE_DIC[iPort][iNowJobNum], tmpRes.ResultData,
                                          strComPare, strMin, strMax, itmpOption, strNgcase, ref strReplaceData, bByPassAscii);

                tmpRes.ResponseData = RESPONSE_DIC[iPort][iNowJobNum];

                if(bByPassAscii && bOOBLableMode && tmpRes.iStatus.Equals((int)STATUS.OK))
                    Item_OOBLABEL = strReplaceData;
                
            }

            if (strReplaceData.Length > 0)
            {
                tmpRes.ResultData = strReplaceData;
            }

            tmpRes.strCMD = LstJOB_CMD[iNowJobNum].CMD;
            tmpRes.strDisplayName = LstJOB_CMD[iNowJobNum].DISPLAYNAME;

            //EXPR 값을 참조한다면 절차서 진행화면에 값을 보여지게 하기 위해서...
            if (strChangeMaxString.Length > 0 && !strChangeMaxString.Equals(LstJOB_CMD[iNowJobNum].MAX))
            {
                tmpRes.strChangeMax = strChangeMaxString;
            }
            else
            {
                tmpRes.strChangeMax = String.Empty;
            }

            //EXPR 값을 참조한다면 절차서 진행화면에 값을 보여지게 하기 위해서...
            if (strChangeMinString.Length > 0 && !strChangeMinString.Equals(LstJOB_CMD[iNowJobNum].MIN))
            {
                tmpRes.strChangeMin = strChangeMinString;
            }
            else
            {
                tmpRes.strChangeMin = String.Empty;
            }

            UpdateDictionary((int)DICINDEX.SEQUENCE, iPort, iNowJobNum, tmpRes.iStatus); //판정결과로 업데이트

            if (!tmpRes.iStatus.Equals((int)STATUS.SKIP))
                tmpRes.LapseTime = swLapse.Elapsed.TotalSeconds.ToString("0.#");
            else
                tmpRes.LapseTime = "0";

            if (!STEPMANAGER_VALUE.bInteractiveMode)
            {   //------------------ 인터렉티브모드가 아니면---------------
                
                bool bFind = false;

                for (int i = 0; i < LstTST_RES[iPort].Count; i++) //슬롯별 최종결과 리스트에 중복되는 시퀀스번호가 있는지 검사.
                {
                    if (LstTST_RES[iPort][i].iSequenceNum == tmpRes.iSequenceNum)
                    {
                        bFind = true;
                        //있으면 그곳에 업데이트 치고                        
                        LstTST_RES[iPort][i] = tmpRes;
                        break;
                    }
                }

                if (!bFind) //없으면 신규 추가한다. 
                {
                    LstTST_RES[iPort].Add(tmpRes);
                }

                if (tmpRes.iStatus == (int)STATUS.OK || tmpRes.iStatus == (int)STATUS.SKIP)
                {  
                    bCmdDoneCheck[iPort] = true; //명령 완료(OK, SKIP)면 해당 명령은 더이상 수행하지 않습니다.
                    btmp = ManagerSendReport(tmpRes); //게이트웨이로 결과 전송합니다.                     
                    return;
                }
                else
                {                    
                    if (RETCOUNT_DIC[iPort] > 0) //아직 리트라이 횟수가 남았다면
                    {
                        if (ConvertOption(LstJOB_CMD[iNowJobNumber].OPTION) == (int)MODE.UNTIL)
                        {
                            if (bCmdDoneCheck[iPort])
                            {                                
                                btmp = ManagerSendReport(tmpRes); //게이트웨이로 결과 전송합니다.  
                            }

                        }
                    }
                    else
                    {
                        btmp = ManagerSendReport(tmpRes); //게이트웨이로 결과 전송합니다.    
                    }                 
                }

            }
            else
            {   //------------------ 인터렉티브모드이면 ---------------
                btmp = ManagerSendReport(tmpRes); //게이트웨이로 결과 전송합니다.
                return;
            }

        }
   
        //절차진행중 한줄의 명령이 모든 슬롯이 완료했는가를 체크
        private void CycleCheck()
        {    
            bool bResult = true;

            for (int i = 0; i < bCmdDoneCheck.Length; i++)
            {
                if (!bCmdDoneCheck[i])
                {
                    bResult = false;
                    break;
                }
            }
           
            if (bResult)
            {
                if (iNowJobNumber < LstJOB_CMD.Count)
                {   //여기서 CASE NG 경우를 체크하여 분기한다.
                    //string strCaseNg = LstJOB_CMD[iNowJobNumber].CASENG;
                    //if (strCaseNg.StartsWith("JUMP"))
                    //{
                    //    CycleSignJump();
                    //}
                    //else
                    //    CheckCaseNgCommand();

                    CheckCaseNgCommand();
                }               
            }           
           
        }

        private void CycleCoreSubCaseUntilOption(int iCount)
        {
            udtCurrTime = DateTime.Now;
            TimeSpan utsNow = udtCurrTime - udtOutSet;
            
            if (DKACTOR.isScanning(iCount)) DKACTOR.runningStop(iCount);

            if (utsNow.TotalSeconds > (double)RETCOUNT_DIC[iCount])
            {                
                //GOTO문 없으면 행동 완료 처리
                bCmdDoneCheck[iCount] = true;
                UpdateDictionary((int)DICINDEX.SEQUENCE, iCount, iNowJobNumber, (int)STATUS.TIMEOUT);
                       
            }
            else
            {
                UpdateDictionary((int)DICINDEX.SEQUENCE, iCount, iNowJobNumber, (int)STATUS.RUNNING);
                CommandLine(iCount, iNowJobNumber); //현재 명령을 수행 후    
            }

        }

        //절차진행중 로직을 체크하는 부분
        private void CycleCore(int iCount)
        {
            if (!Item_bTestStarted)
            {
                bEngineOn = false;
                return;
            }
            
            if (SEQUENCE_DIC[iCount][iNowJobNumber] == (int)STATUS.TESTDONE)
            {   //이미 지나간 시퀀스는 지나가자 ( GOTO LABEL 테스트의 경우)   
                bCmdDoneCheck[iCount] = true; 
                return;
            }
            
            if (ConvertAction(LstJOB_CMD[iNowJobNumber].ACTION) == (int)ACTION.SKIP) 
            {   //명령이 SKIP 이면 건너뛰자.

                UpdateDictionary((int)DICINDEX.SEQUENCE, iCount, iNowJobNumber, (int)STATUS.SKIP);
                bCmdDoneCheck[iCount] = true;                
                CycleSignDone(iCount, (int)STATUS.SKIP, iNowJobNumber); 
                return;
            }
            

            switch (SEQUENCE_DIC[iCount][iNowJobNumber])
            {                
                case (int)STATUS.OK: 
                case (int)STATUS.RUNNING: break;
                case (int)STATUS.CHANGEJOB: CycleSignDone(iCount, (int)STATUS.OK, iNowJobNumber);
                                            CycleSignAction((int)STATUS.CHANGEJOB, RESULTDT_DIC[iCount][iNowJobNumber]); return;
                default:

                    if (ConvertOption(LstJOB_CMD[iNowJobNumber].OPTION) != (int)MODE.UNTIL)
                    {
                        if (DKACTOR.isScanning(iCount)) DKACTOR.runningStop(iCount);                         
                        System.Threading.Thread.Sleep(10);
                        Application.DoEvents();
                        if (RETCOUNT_DIC[iCount] > 0)
                        {
                            //PKS
                            //EvGotoTestGridRow(iNowJobNumber);

                            UpdateDictionary((int)DICINDEX.SEQUENCE, iCount, iNowJobNumber, (int)STATUS.RUNNING);
                            RETCOUNT_DIC[iCount]--; //RETRY 카운트 감소
                            bool bResCommand = CommandLine(iCount, iNowJobNumber); //현재 명령을 수행 후 
                            
                            if (!Item_bTestStarted)
                            {
                                CycleSignAction((int)STATUS.STOP); return;
                            }

                            //if(RETCOUNT_DIC[iCount] > 0) RETCOUNT_DIC[iCount]--; //RETRY 카운트 감소     
                           
                        }
                        else
                        {
                            //GOTO문 없으면 행동 완료 처리
                            bCmdDoneCheck[iCount] = true;
                            if(SEQUENCE_DIC[iCount][iNowJobNumber].Equals((int)STATUS.NONE))
                            {
                                RETCOUNT_DIC[iCount] = 0;
                                UpdateDictionary((int)DICINDEX.RESULTDT, iCount, iNowJobNumber, "Zero Condition RetryCount");
                                UpdateDictionary((int)DICINDEX.RESPONSE, iCount, iNowJobNumber, "Zero Condition RetryCount");
                                UpdateDictionary((int)DICINDEX.SEQUENCE, iCount, iNowJobNumber, (int)STATUS.CHECK);                           
                            }
                            

                        }
                    }
                    else
                    {
                        if (SEQUENCE_DIC[iCount][iNowJobNumber] == (int)STATUS.NONE) udtOutSet = DateTime.Now;
                        CycleCoreSubCaseUntilOption(iCount);
                    }
                    break;
            }
			            
        }

        //ACTOR 상태 가져오는 엔진
        void CycleActorStatusEngine(object status)
        {
            try
            {
                THREDSTATUS tmpEvent = new THREDSTATUS();
                //tmpEvent.tDio = DKACTOR.GetStatus((int)COMSERIAL.DIO);
                //tmpEvent.tSet = DKACTOR.GetStatus((int)COMSERIAL.SET);

                tmpEvent.bLedStatus = new bool[(int)DIOPIN.PINUSE + (int)DOUT.END];
                tmpEvent.bEngineStatus = bEngineOn;

                for (int i = (int)DIOPIN.START; i < tmpEvent.bLedStatus.Length; i++)
                {
                    if (GetSensorDic(i) != (int)SENSOR.OFF)
                        tmpEvent.bLedStatus[i] = true;
                    else
                        tmpEvent.bLedStatus[i] = false;
                }
                if (STEPMANAGER_VALUE.bProgramRun)
                {
                    if (iNowJobNumber < LstJOB_CMD.Count)
                        tmpEvent.strCommandState = "[" + iNowJobNumber.ToString("000") + "][" + LstJOB_CMD[iNowJobNumber].CMD + "][" + (STATUS.NONE + SEQUENCE_DIC[(int)DEFINES.SET1][iNowJobNumber]).ToString() + "][" + RETCOUNT_DIC[(int)DEFINES.SET1].ToString() + "]";
                }
                else
                    tmpEvent.strCommandState = "";

                ManagerSendThredEvent(tmpEvent);
            }
            catch 
            {
            	
            }

        }

        //잡카운트 체크
        private bool CheckJobIndex()
        {
            if (iNowJobNumber >= LstJOB_CMD.Count)            
            {
                CycleSignAction((int)STATUS.END);  //절차가 다 끝났으면 OFF
                return true;
            }
            return false;
        }

        //절차진행시키는 반복 엔진
        void CycleEngine(object status)
        {           
            while (bEngineOn)
            {

                if (CheckJobIndex()) return;


                if (!Item_bTestStarted)
                {
                    CycleSignAction((int)STATUS.STOP); return;// //USER STOP 이면 빠져나가자. )
                }

                int iCheckStatus = 0;
                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)   //1. 모든 슬롯수만큼 돌면서
                {
                    try
                    {
                        iCheckStatus = SEQUENCE_DIC[i][iNowJobNumber];

                        switch (iCheckStatus)
                        {

                            case (int)STATUS.OK:      //명령 완료(OK)이나 옵션이 SEND 일 경우 리트라이를 적용한다.
                                    
                                if (ConvertOption(LstJOB_CMD[iNowJobNumber].OPTION) == (int)MODE.SEND
                                    && RETCOUNT_DIC[i] > 0)
                                {
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iNowJobNumber, (int)STATUS.NONE);
                                    CycleCore(i);
                                    CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                    break;
                                }

                                //명령 완료(OK)이나 옵션이 AVERAGE 일 경우 리트라이를 적용한다.
                                if (ConvertOption(LstJOB_CMD[iNowJobNumber].OPTION) == (int)MODE.AVERAGE
                                    && RETCOUNT_DIC[i] > 0)
                                {
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iNowJobNumber, (int)STATUS.NONE);
                                    AVERAGE_DIC.Add(AVERAGE_DIC.Count, RESULTDT_DIC[i][iNowJobNumber]); //값저장.AVERAGE_DIC
                                    CycleCore(i);
                                    CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                    break;
                                }

                                //명령 완료(OK)이나 옵션이 MULTIPLE 일 경우 리트라이를 적용한다. (인증서 업로드 같은 경우)
                                if (ConvertOption(LstJOB_CMD[iNowJobNumber].OPTION) == (int)MODE.MULTIPLE
                                    && STEPMANAGER_VALUE.iUploadBytesCountStartIndex <
                                        STEPMANAGER_VALUE.iUploadBytesCountTotalSize)
                                {
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iNowJobNumber, (int)STATUS.NONE);
                                    RETCOUNT_DIC[i] = 1;
                                    CycleCore(i);
                                    CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                    break;
                                }

                                CycleSignDone(i, (int)STATUS.OK, iNowJobNumber);
                                CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                break;

                            case (int)STATUS.CHECK:      //CHECK 이면 

                                CycleSignDone(i, (int)STATUS.CHECK, iNowJobNumber);
                                CycleCore(i);   
                                CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                break;

                            case (int)STATUS.NG:      //NG 이면 

                                //명령 완료(OK)이나 옵션이 MULTIPLE 일 경우 
                                if (ConvertOption(LstJOB_CMD[iNowJobNumber].OPTION) == (int)MODE.MULTIPLE)
                                {
                                    if (RETCOUNT_DIC[i] > 0) RETCOUNT_DIC[i]--;
                                }
                                CycleSignDone(i, (int)STATUS.NG, iNowJobNumber);
                                CycleCore(i);
                                CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                break;

                            case (int)STATUS.TIMEOUT: //TIMEOUT 이면 
                                CycleSignDone(i, (int)STATUS.TIMEOUT, iNowJobNumber);
                                CycleCore(i);
                                CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                break;

                            case (int)STATUS.RUNNING: //명령 수행중이면 빠져나간다.                                                                           
                                break;

                            case (int)STATUS.NONE: //NONE 이면                                 
                                CycleCore(i);                                
                                break;

                            default:                  //기타 
                                CycleCore(i);                     
                                CycleCheck(); // 계속 진행할 것인가를 체크한다.
                                break;

                        }
                    }
                        
                    catch (Exception ex) //에러나면 로그 남기고 재시도 적용한다.
                    {
                        DKLoggerPC.WriteCommLog("[CycleEngine] switch (SEQUENCE_DIC[" + i.ToString() + "][ " + iNowJobNumber.ToString() + "]) : "
                        + ex.ToString() + "SEQUENCE_DIC TOTALCOUNT : " + SEQUENCE_DIC[(int)DEFINES.SET1].Count.ToString(), "EXCEPTION", false);
                        CycleSignAction((int)STATUS.CHECK);

                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                        STEPMANAGER_VALUE.DebugView(strExMsg);

                        bEngineOn = false;
                        return;
                    }


                }

                System.Threading.Thread.Sleep(1);
                //CycleCheck(); // 계속 진행할 것인가를 체크한다.
            
            }
            
        }

        /*
        private bool UseDeveloperMode(int iNJobNumber)
        {          
            if (OpenDevelopMode)
            {   
                RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                RESULTDT_DIC[(int)DEFINES.SET1][iNJobNumber] = STATUS.CHECK.ToString();
                RESPONSE_DIC[(int)DEFINES.SET1][iNJobNumber] = "ONLY STEP CHCKE MODE";
                SEQUENCE_DIC[(int)DEFINES.SET1][iNJobNumber] = (int)STATUS.CHECK;
                System.Threading.Thread.Sleep(50);
                return true;
        
            }
            return false;
        }
        */
        private bool IsEnableDevice(int iJobNumber)
        {     
            bool bIsEnable = true;

            switch (LstJOB_CMD[iJobNumber].TYPE)
            {
                case "SCAN":      bIsEnable = OpenSCAN; break;

                case devPCAN:     if (OpenPCAN != (int)STATUS.OK) bIsEnable = false; break;

                case devVector:   if (OpenVector != (int)STATUS.OK) bIsEnable = false; break;

                case devMTP200:   if (OpenMTP200!= (int)STATUS.OK) bIsEnable = false; break;

                case dev5515C:    if (Open5515C != (int)STATUS.OK) bIsEnable = false; break;

                case devKEITHLEY: if (OpenKEITHLEY != (int)STATUS.OK) bIsEnable = false; break;

                case devTC3000:   bIsEnable = OpenTC3000; break;

                case devAudio: if (OpenAudio != (int)STATUS.OK) bIsEnable = false; break;

                case dev34410a: if (Open34410A != (int)STATUS.OK) bIsEnable = false; break;

                // case devMELSEC: if (OpenMELSEC != (int)STATUS.OK) bIsEnable = false; break;
                case devMTP120A: if (OpenMTP120A != (int)STATUS.OK) bIsEnable = false; break;

                default: break;

            }

            if (!bIsEnable)
            {
                RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                GateWayMsgProcess((int)STATUS.NG, "DEVICE ERROR", "DEVICE ERROR", String.Empty, true);
            }
            return bIsEnable;
        }

        //DLL GATE 실행
        private void ExcuteDLLGATE(string strPack, double dTimeout, int iSendRecvOption, double dDelaySec, string strCommandTBLName, string strPram)
        {
            if (dDelaySec > 0)
            {
                DelayChecker(dDelaySec, false);
            }

            ETHERNETSTRUCT tmpParam = new ETHERNETSTRUCT();

            tmpParam.dTimeOut = dTimeout;
            tmpParam.strCommandName = strCommandTBLName;
            tmpParam.strSendPack = strPack;
            tmpParam.strParam = strPram;
            switch (strCommandTBLName)
            {
                case "CONNECT":
                    tmpParam.iCommandType = (int)ETHERNETCOMMAND.CONNECT;
                    tmpParam.strParam = String.Empty;

                    string strParam1 = "127.0.0.1"; //LoadINI("COMPORT", "DSA");
                    string strParam2 = "31851";

                    tmpParam.strParam = strParam1 + "/" + strParam2;
                    if (strParam1.Equals("0") || strParam2.Equals("0"))
                    {   
                        GateWayMsgProcess((int)STATUS.NG, "Connection Error", "Connection Error", String.Empty, true);
                        return;
                    }
                    break;

                case "DISCONNECT":
                    tmpParam.iCommandType = (int)ETHERNETCOMMAND.DISCONNECT;
                    tmpParam.strParam = String.Empty;
                    break;

                case "SEND_MESSAGE":

                    if (!DK_DLLGATE.IsConnected())
                    {
                        GateWayMsgProcess((int)STATUS.NG, "DISCONNECTED", "DISCONNECTED", String.Empty, true);
                        return;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(strPram))
                        {   
                            byte[] bMessage = Encoding.UTF8.GetBytes(strPram);
                            tmpParam.strSendPack = BitConverter.ToString(bMessage).Replace("-", " ");
                            tmpParam.strParam = String.Empty;
                            tmpParam.iCommandType = (int)ETHERNETCOMMAND.SENDRECV;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 NO MESSAGE", "PAR1 NO MESSAGE", String.Empty, true);
                            return;
                        }
                    }

                    break;
                default:
                    if (!DK_DLLGATE.IsConnected())
                    {
                        GateWayMsgProcess((int)STATUS.NG, "DISCONNECTED", "DISCONNECTED", String.Empty, true);                        
                        return;
                    }

                    //파라미터가필요한 명령인지 검사.
                    if (tmpParam.strSendPack.Contains("<DATA>"))
                    {
                        if (String.IsNullOrEmpty(strPram))
                        {
                            GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);                            
                            return;
                        }
                        else
                        {
                            strPram = strPram.Replace(',', '|'); // 콤마를 파이프로 바꾸자.
                            tmpParam.strSendPack = tmpParam.strSendPack.Replace("<DATA>", strPram);
                        }
                    }
                    else
                    {
                        tmpParam.strParam = String.Empty;
                    }

                    switch (iSendRecvOption)
                    {
                        case (int)MODE.SEND: tmpParam.iCommandType = (int)ETHERNETCOMMAND.SEND; break;
                        case (int)MODE.SENDRECV: tmpParam.iCommandType = (int)ETHERNETCOMMAND.SENDRECV; break;
                        default:
                            GateWayMsgProcess((int)STATUS.NG, "Unknown Option", "Unknown Option", String.Empty, true);                            
                            return;
                    }

                    break;
            }

            DK_DLLGATE.LauncherRecvThread(tmpParam);
            return;

        }


        //절차진행하는 사이클엔진에서 명령을 수행하는 루틴
        public bool CommandLine(int iPort, int iJobNumber)
        {//JOB 파일을 한줄단위로 실행

            // throw new ArgumentNullException("Exception TEST"); 강제예외발생방법.

            if (iJobNumber >= LstJOB_CMD.Count) return true;

            //인터렉티브 모드면 진행번호 변경
            if (STEPMANAGER_VALUE.bInteractiveMode)
            {
                if (swLapse.IsRunning) swLapse.Stop();
                swLapse.Reset();
                iNowJobNumber = iJobNumber;
            }

            if (DKACTOR.isRunnig(iPort))
            {
                DKACTOR.runningStop(iPort);
                //return false;
                System.Threading.Thread.Sleep(10);
            }
            
            string strSendpac       = String.Empty;
            //string strSendpar1      = LstJOB_CMD[iJobNumber].PAR1;
            double dTimeout         = Double.Parse(LstJOB_CMD[iJobNumber].TIMEOUT);
            double dDelayTime       = Double.Parse(LstJOB_CMD[iJobNumber].DELAY);
            string strCmdName       = LstJOB_CMD[iJobNumber].CMD;
            int iSendRecvOption     = ConvertOption(LstJOB_CMD[iJobNumber].OPTION);
            string strSendparEx     = LstJOB_CMD[iJobNumber].PAR1;
            AnalyizePack anlPack = new AnalyizePack();

            if (LstJOB_CMD[iJobNumber].COMPARE.Equals("RESULTCODE"))
                anlPack.bResultCodeOption = true;
            else
                anlPack.bResultCodeOption = false;

            if (!swLapse.IsRunning) { swLapse.Reset(); swLapse.Start(); }

            //장치를 한번더 체크
            //if (!IsEnableDevice(iJobNumber)) return false;

            if (LstJOB_CMD[iJobNumber].ACTION.Equals("SKIP"))
            {
                GateWayMsgProcess((int)STATUS.SKIP, "", "", String.Empty, true);
                return true;
            }

            switch (LstJOB_CMD[iJobNumber].TYPE)
            {
                case "SCAN": 
                            bool bScanner = ScannerCommand((int)DEFINES.SET1, iJobNumber, strCmdName, dTimeout, dDelayTime, iSendRecvOption);
                            return bScanner;

                case "GEN9":
                            //if (UseDeveloperMode(iJobNumber)) return false;                            
                            if (!OpenSET)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }
                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.                                                       

                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }
                    
                            if (Gen9ExceptionCommand(strCmdName))
                            {
                                return true;
                            }

                            if (Gen9GpsCommand(strCmdName))
                            {
                                return true;
                            }

                            if (CheckGen9MDNCommand(iPort, iJobNumber, strCmdName, ref strSendparEx))// MDN UPDATE or WRITE Make 명령처리
                            {
                                
                                return true;
                            }

                            if (FindPacGen9(strCmdName, ref strSendpac, ref anlPack))
                            {
                                DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.GEN9BYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            }
                            return true;

                case "GEN10":                            
                            //if (UseDeveloperMode(iJobNumber)) return false;                            
                            if (!OpenSET)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }
                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.                                                       
                    
                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))                            
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }
                           
                            
                            //NAD DLL 를 써야하는 명령인지 체크한다.
                            if (strCmdName.Contains("DLL_"))
                            {
                                NadKeyDllCommand(iPort, iJobNumber, strCmdName, strSendparEx);
                                return true;
                            }

                            //OOB_SELF_TEST_CHECK 명령이면 EXPR 값에 있는곳에서 추출해낸다.
                            if (strCmdName.Equals("OOB_SELF_TEST_CHECK"))
                            {
                                return GetGEN10OOBresult(strSendparEx);
                            }

                            //DTC_ALL_OQA 명령이면 EXPR 값에 있는곳에서 추출해낸다. (개별 DTC로 안오는게 있다. 그래서 전부 읽어서 파싱...젠장)
                            if (strCmdName.Equals("OOB_DTC_CHECK"))
                            {
                                return GetGEN10DTCresult(strSendparEx);
                            }

                            // APN TABLE 정보 읽어오는 커맨드인지 확인.
                            if (CheckApnTableCommand(strCmdName, LstJOB_CMD[iJobNumber].PAR1))
                            {
                                return true;
                            }

                            // SIM 정보 읽어오는 커맨드인지 확인.
                            if (CheckSimInfoCommand(strCmdName))
                            {
                                return true;
                            }

                            // SIM 서비스 읽어오는 커맨드인지 확인.
                            if (CheckServiceInfoCommandA(strCmdName, strSendparEx))
                            {
                                return true;
                            }

                            // GEN10 구형 모델 / STID rangeCheck
                            if (Gen10ModelCheckCommand(strCmdName, strSendparEx))
                            {
                                return true;
                            }

                            if (Gen10OldGpsCommand(strCmdName))
                            {
                                return true;
                            }

                            if (LstJOB_CMD[iJobNumber].COMPARE.Equals("RESULTCODE"))
                                anlPack.bResultCodeOption = true;
                            else
                                anlPack.bResultCodeOption = false;

                            if (FindPacGen10(strCmdName, ref strSendpac, ref anlPack))
                            {                                
                                DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.GEN10BYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);                                
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);                               
                            }
                            return true;

                case "TCP":                                                       
                            //if (UseDeveloperMode(iJobNumber)) return false;

                            if (!OpenSET)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }

                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                            
                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);          
                                return true;
                            }

                            //NAD DLL 를 써야하는 명령인지 체크한다.
                            if (strCmdName.Contains("DLL_"))
                            {
                                NadKeyDllCommand(iPort, iJobNumber, strCmdName, strSendparEx);
                                return true;
                            }

                            //OOB_SELF_TEST_CHECK 명령이면 EXPR 값에 있는곳에서 추출해낸다.
                            if (strCmdName.Equals("OOB_SELF_TEST_CHECK"))
                            {
                                return GetOOBresult(strSendparEx);
                            }

                            // SIM 서비스 읽어오는 커맨드인지 확인.
                            if (CheckServiceInfoCommandA(strCmdName, strSendparEx))
                            {
                                return true;
                            }

                            if (FindPacTcp(strCmdName, ref strSendpac))
                            {
                                /*
                                if (strCmdName.Equals("GET_SERVICE_INFORMATION"))
                                    DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, (int)MODE.RECV, dDelayTime, (int)RS232.TCPBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);                                
                                else
                                 * */
                                DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.TCPBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);                                
                                 
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);                               
                            }
                            return true;

                case "GEN11":
                case "GEN11P":
                case "GEN12":
                            switch (strCmdName)
                            {
                                    //OOB_SELF_TEST_CHECK 명령이면 EXPR 값에 있는곳에서 추출해낸다.
                                case "OOB_SELF_TEST_CHECK":
                                    return GetOOBresult(strSendparEx);

                                default: 
                                    if (strCmdName.Contains("ALDL3_"))
                                        //STEPMANAGER_VALUE.SetGen11AldlBlockSize(321);
                                        STEPMANAGER_VALUE.SetGen11AldlBlockSize(330); //2021.05.29. 이진성책임, 이정우책임 321 에서 330으로 변경요청함..
                                    if (strCmdName.Contains("ALDL2_")) 
                                        STEPMANAGER_VALUE.SetGen11AldlBlockSize(250);
                                    if (strCmdName.Contains("ALDL_"))
                                        STEPMANAGER_VALUE.DefaultGen11AldlBlockSize();
                                    break;
                            }

                            if (!OpenSET)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }

                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.

                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }                                                                        

                            if (iSendRecvOption == (int)MODE.MULTIPLE)
                            {
                                //인증서 transfer의 경우 내부적으로 여러번돈다. 하지만 NG시에는 리트라이를 탈수 있으므로 한번만 동작하도록한다.
                                RETCOUNT_DIC[iPort] = 1;
                            }

                            // SIM 서비스 읽어오는 커맨드인지 확인.
                            if (CheckServiceInfoCommandB(strCmdName, strSendparEx))
                            {
                                return true;
                            }

                            switch (LstJOB_CMD[iJobNumber].TYPE)
                            {
                                case "GEN11":                                    

                                    if (FindPacGen11(strCmdName, ref strSendpac))
                                    {
                                        
                                        DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.GEN11BYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                                    }
                                    else
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                                    }
                                    break;
                                case "GEN11P":

                                    if (FindPacGen11P(strCmdName, ref strSendpac))
                                    {
                                        DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.GEN11PBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                                    }
                                    else
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                                    }
                                    break;
                                case "GEN12":

                                    if (FindPacGen12(strCmdName, ref strSendpac))
                                    {

                                        DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.GEN12BYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                                    }
                                    else
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                                    }
                                    break;
                        default: return true;
                            }
                            
                            return true;

                case "NAD":                 
                case "CCM":

                            if (strCmdName.Equals("PORT_OPEN"))
                            {
                                CloseCCMPort();
                                OpenCCM = DKACTOR.CommOpen((int)COMSERIAL.CCM, "COM" + FixNadPort.ToString(), 115200);
                                RETCOUNT_DIC[iPort] = 0;
                                STEPMANAGER_VALUE.InitCcmGpsStructure();
                                if (OpenCCM)
                                    GateWayMsgProcess((int)STATUS.OK, "SUCCESS-" + "COM" + FixNadPort.ToString(), "SUCCESS-" + "COM" + FixNadPort.ToString(), String.Empty, true);
                                else
                                    GateWayMsgProcess((int)STATUS.NG, "OPEN FAIL", "OPEN FAIL", String.Empty, true);
                                return true;
                            }

                            if (strCmdName.Equals("PORT_CLOSE"))
                            {
                                CloseCCMPort();
                                OpenCCM = false;
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                                return true;
                            }

                            if (!OpenCCM)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }

                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.

                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }

                            if (Gen11CcmGpsCommand(strCmdName))
                            {
                                return true;
                            }

                            switch (LstJOB_CMD[iJobNumber].TYPE)
                            {
                                case "NAD":
                                    if (FindPacNAD(strCmdName, ref strSendpac))
                                        DKACTOR.SendRecvCmd((int)COMSERIAL.CCM, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.NADBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                                    else
                                        GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                                    break;

                                case "CCM":
                                    if (FindPacCCM(strCmdName, ref strSendpac))
                                        DKACTOR.SendRecvCmd((int)COMSERIAL.CCM, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.CCMBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                                    else
                                        GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                                    break;
                            }
                            return true;

                case "ATT":
                            //if (UseDeveloperMode(iJobNumber)) return false;
                            if (!OpenSET)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }

                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }

                            //NAD DLL 를 써야하는 명령인지 체크한다.
                            if (strCmdName.Contains("DLL_"))
                            {
                                NadKeyDllCommand(iPort, iJobNumber, strCmdName, strSendparEx);
                                return true;
                            }

                            // SIM 정보 읽어오는 커맨드인지 확인.
                            if (CheckSimInfoCommand(strCmdName))
                            {                                
                                return true;
                            }

                            // APN TABLE 정보 읽어오는 커맨드인지 확인.
                            if (CheckApnTableCommand(strCmdName, LstJOB_CMD[iJobNumber].PAR1))
                            {
                                return true;
                            }

                            // SIM 서비스 읽어오는 커맨드인지 확인.
                            if (CheckServiceInfoCommandA(strCmdName, strSendparEx))
                            {
                                return true;
                            }

                            // GEN10 처럼 OLD 방식의 GPS 정보 읽어오는 커맨드인지 확인.
                            if (Gen10OldGpsCommand(strCmdName))
                            {
                                return true;
                            }

                            //OOB_SELF_TEST_CHECK 명령이면 EXPR 값에 있는곳에서 추출해낸다.
                            if (strCmdName.Equals("OOB_SELF_TEST_CHECK"))
                            {
                                return GetATTOOBresult(strSendparEx);
                            }

                            if (FindPacAtt(strCmdName, ref strSendpac))
                            {
                                DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.ATTBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            }
                            return true;

                case "MCTM":
                          
                            if (!OpenSET)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }

                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.

                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }

                            if (LstJOB_CMD[iJobNumber].COMPARE.Equals("RESULTCODE"))
                                anlPack.bResultCodeOption = true;
                            else
                                anlPack.bResultCodeOption = false;

                            //GPRMC 명령이면 EXPR 값에 있는곳에서 추출해낸다.
                            if (strCmdName.Contains("READ_GPRMC_"))
                            {
                                MCTM_GPRMC_COMMAND(strCmdName);
                                return true;
                            }

                            if (FindPacMCTM(strCmdName, ref strSendpac))
                            {
                                DKACTOR.SendRecvCmd((int)COMSERIAL.SET, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.MCTMBYTE, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            }
                            return true;

                case "DIO":
                                                        
                            if (!OpenDIO)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "COMPORT CLOSED", "COMPORT CLOSED", String.Empty, true);
                                return true;
                            }
                            
                            if (!CheckExtPwr())
                            {  //Extpwr 모드가 아니면  그냥 BYPASS 딜레이 없이 진행
                                switch (strCmdName)
                                {
                                    case "RELAY_ON_KEY_EXT":
                                    case "RELAY_ON_PRIMARY_EXT":
                                    case "RELAY_ON_BACKUP_EXT" :
                                    case "RELAY_OFF_KEY_EXT":
                                    case "RELAY_OFF_PRIMARY_EXT":
                                    case "RELAY_OFF_BACKUP_EXT":
                                        GateWayMsgProcess((int)STATUS.CHECK, "SWITCH EXTERNMAL", "SWITCH EXTERNMAL", String.Empty, true);
                                        return true;
                                    default: break;
                                }

                            }
                            else
                            {
                                if (!STEPMANAGER_VALUE.bInteractiveMode)
                                {
                                    bool bBypass = false;

                                    switch (strCmdName)
                                    {
                                        case "RELAY_ON_KEY_EXT":       if (!bExtOnKey)      bBypass = bExtOnKey      = true; break;
                                        case "RELAY_ON_PRIMARY_EXT":   if (!bExtOnPrimary)  bBypass = bExtOnPrimary  = true; break;
                                        case "RELAY_ON_BACKUP_EXT":    if (!bExtOnBackup)   bBypass = bExtOnBackup   = true; break;
                                        case "RELAY_OFF_KEY_EXT":      if (!bExtOffKey)     bBypass = bExtOffKey     = true; break;
                                        case "RELAY_OFF_PRIMARY_EXT":  if (!bExtOffPrimary) bBypass = bExtOffPrimary = true; break;
                                        case "RELAY_OFF_BACKUP_EXT":   if (!bExtOffBackup)   bBypass = bExtOffBackup  = true; break;
                                        default: break;
                                    }

                                    if (bBypass)
                                    {
                                        switch (strCmdName)
                                        {
                                            case "RELAY_ON_KEY_EXT":
                                            case "RELAY_ON_PRIMARY_EXT":
                                            case "RELAY_ON_BACKUP_EXT":
                                            case "RELAY_OFF_KEY_EXT":
                                            case "RELAY_OFF_PRIMARY_EXT":
                                            case "RELAY_OFF_BACKUP_EXT":
                                                GateWayMsgProcess((int)STATUS.OK, "BYPASS", "BYPASS", String.Empty, true);
                                                return true;
                                            default: break;
                                        }
                                    }
                                }
                                
                            }

                            if (FindPacDioVcpBench(strCmdName, ref strSendpac))
                            {
                                if (strCmdName.Equals("RESET")) CloseCCMPort();
                                DKACTOR.InsertDioCommand((int)COMSERIAL.DIO, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.MOOHANTECH, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);                             
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);                                
                            }
                            return true;
                           
                case devAudio:
                            //if (UseDeveloperMode(iJobNumber)) return false;
                            if (OpenAudio != (int)STATUS.OK)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.CHECK, "DEVICE ERROR", "DEVICE ERROR", String.Empty, true);
                                return true;
                            }

                            if (FindPacDioAudioSelector(strCmdName, ref strSendpac))
                            {
                                DKACTOR.SendRecvCmd((int)COMSERIAL.AUDIOSEL, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.MOOHANTECH, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);                                
                            }
                            return true;

                case devADC:
                            //if (UseDeveloperMode(iJobNumber)) return false;

                            if (OpenADC != (int)STATUS.OK)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.CHECK, "DEVICE ERROR", "DEVICE ERROR", String.Empty, true);
                                return true;
                            }

                            if (FindPacDioAdcModule(strCmdName, ref strSendpac))
                            {
                                DKACTOR.SendRecvCmd((int)COMSERIAL.ADC, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.MOOHANTECH, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            }
                            return true;

                case "PAGE":
                            switch (strCmdName)
                            {
                                case "NETWORK_PING_TTL":
                                case "NETWORK_PING_TIME": break;
                                default: MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iJobNumber].DISPLAYNAME, iPort); break;
                            }
                            
                            bool bPage = FindPageCommand(iPort, iJobNumber, strCmdName);                                                                              
                            return bPage;

                case "MES":                            
                            if (!STEPMANAGER_VALUE.bUseOracleOn)
                            {
                                RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                                GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "MES UNCHECKED", String.Empty, true); 
                                return true;
                            }

                            try
                            {
                                Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                                if (Item_dStepTimeSet > 0)
                                {
                                    dtStepDelaySet = DateTime.Now;
                                    while (!StepDelayTimeCheck())
                                    {
                                        if (Item_dStepTimeSet <= 0) break;
                                        System.Threading.Thread.Sleep(20);
                                        if (!Item_bTestStarted) break;
                                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                    }
                                }
                            }
                            catch { }

                            string strReason = String.Empty;

                            if (!strCmdName.Equals("DATA_VIEW") && !OracleCheckConnection(ref strReason))
                            {
                                RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                                MessageLogging((int)LOGTYPE.TX, "CHECK CONNECTION?", iPort);
                                GateWayMsgProcess((int)STATUS.ERROR, STATUS.ERROR.ToString(), "MES DISCONNECTED", strReason, true);
                                return true;
                            }

                            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                            {//1. 모든 슬롯수만큼 돌면서                            
                                if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                                {
                                    RETCOUNT_DIC[i] = 0; // 3. 공통명령으로 한번만 처리한다.
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.RUNNING);
                                }
                            }
                            bool[] bMesCmd = new bool[(int)DEFINES.END];
                            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                            {//1. 모든 슬롯수만큼 돌면서     
                                bMesCmd[i] = false;
                                if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                                {
                                    //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                                    strSendparEx = String.Empty;
                                    if (!CheckExprParam(i, iJobNumber, ref strSendparEx))
                                    {
                                        bMesCmd[i] = false;                                        
                                        RETCOUNT_DIC[i] = 0;
                                        GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                    }
                                    else
                                    {
                                        bMesCmd[i] = OracleMesCommand(i, iJobNumber, strCmdName, strSendparEx);
                                    }
                                    if (!Item_bTestStarted) break; //USER STOP 이면 빠져나가자. 
                                }
                            }
                            return true;

                case "OOB":
                            try
                            {
                                Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                                dtStepDelaySet = DateTime.Now;
                                while (!StepDelayTimeCheck())
                                {
                                    if (Item_dStepTimeSet <= 0) break;
                                    System.Threading.Thread.Sleep(10);
                                    if (!Item_bTestStarted) break;
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                }
                            }
                            catch { }

                            MessageLogging((int)LOGTYPE.TX, "(" + LstJOB_CMD[iJobNumber].PAR1 + ")", iPort);
                            FindOOBCommand(iJobNumber, strCmdName);                             
                            return true;

                case "EXCEL":
                            MessageLogging((int)LOGTYPE.TX, strSendparEx, iPort);
                            KillThreadObject(threadExcelComm);

                            threadExcelComm = new Thread(delegate()
                            {
                                RunExcelCommand(iPort, iJobNumber, LstJOB_CMD[iJobNumber].CMD, strSendparEx);
                            });
                            threadExcelComm.Start();
                            return true;

                case "GMES":
                    
                    //곤산법인은 MES (오라클)을 쓰는 바람에 KIS 서버를 접속해야하는 부분이 여기에 분기를 추가해야한다.   
                    if (STEPMANAGER_VALUE.bUseOracleOn)
                    {

                    }
                    else
                    {
                        if (!STEPMANAGER_VALUE.bUseMesOn)
                        {
                            RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                            GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "GMES UNCHECKED", String.Empty, true);
                            return true;
                        }

                        try
                        {
                            Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                            if (Item_dStepTimeSet > 0)
                            {
                                dtStepDelaySet = DateTime.Now;
                                while (!StepDelayTimeCheck())
                                {
                                    if (Item_dStepTimeSet <= 0) break;
                                    System.Threading.Thread.Sleep(20);
                                    if (!Item_bTestStarted) break;
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                }
                            }
                        }
                        catch { }
                        //CSMES
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                        {
                            if (DKOSIMES.GetConnStatus() != 0)
                            {
                                bool bGmesConn = false;
                                try
                                {
                                    Item_dStepTimeSet = 15; //15초간 GMES 가 커넥션될때까지 기다려본다.
                                    dtStepDelaySet = DateTime.Now;
                                    while (!StepDelayTimeCheck())
                                    {
                                        MessageLogging((int)LOGTYPE.TX, "CHECK CONNECTION?", iPort);
                                        if (DKOSIMES.GetConnStatus() == 0)
                                        {
                                            System.Threading.Thread.Sleep(250);
                                            MessageLogging((int)LOGTYPE.RX, "CONNECTED.", iPort);
                                            bGmesConn = true; break;
                                        }
                                        MessageLogging((int)LOGTYPE.RX, "DISCONNECTED.", iPort);
                                        if (Item_dStepTimeSet <= 0) break;
                                        System.Threading.Thread.Sleep(250);
                                        if (!Item_bTestStarted) break;
                                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                    }
                                }
                                catch { }

                                if (!bGmesConn)
                                {
                                    RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                                    GateWayMsgProcess((int)STATUS.ERROR, STATUS.ERROR.ToString(), "OSIMES DISCONNECTED", String.Empty, true);
                                    iNowJobNumber = LstJOB_CMD.Count;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (DKGMES.GMES_GetStatus() != 2)
                            {
                                bool bGmesConn = false;
                                try
                                {
                                    Item_dStepTimeSet = 15; //15초간 GMES 가 커넥션될때까지 기다려본다.
                                    dtStepDelaySet = DateTime.Now;
                                    while (!StepDelayTimeCheck())
                                    {
                                        MessageLogging((int)LOGTYPE.TX, "CHECK CONNECTION?", iPort);
                                        if (DKGMES.GMES_GetStatus() == 2)
                                        {
                                            System.Threading.Thread.Sleep(250);
                                            MessageLogging((int)LOGTYPE.RX, "CONNECTED.", iPort);
                                            bGmesConn = true; break;
                                        }
                                        MessageLogging((int)LOGTYPE.RX, "DISCONNECTED.", iPort);
                                        if (Item_dStepTimeSet <= 0) break;
                                        System.Threading.Thread.Sleep(250);
                                        if (!Item_bTestStarted) break;
                                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                    }
                                }
                                catch { }

                                if (!bGmesConn)
                                {
                                    RETCOUNT_DIC[(int)DEFINES.SET1] = 0;
                                    GateWayMsgProcess((int)STATUS.ERROR, STATUS.ERROR.ToString(), "GMES DISCONNECTED", String.Empty, true);
                                    iNowJobNumber = LstJOB_CMD.Count;
                                    return true;
                                }
                            }
                        }
                    }
                    
                           
                    for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                    {//1. 모든 슬롯수만큼 돌면서                            
                        if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                        {
                            if (!strCmdName.Contains("KALS_")) //kals 는 리트라이하자...
                            {
                                RETCOUNT_DIC[i] = 0; // 3. 공통명령으로 한번만 처리한다.                                            
                            }
                            UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.RUNNING);
                        }
                    }

                    bool[] bGmesCmd = new bool[(int)DEFINES.END];
                    for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                    {//1. 모든 슬롯수만큼 돌면서     
                        bGmesCmd[i] = false;
                        if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                        {
                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                            strSendparEx = String.Empty;
                            if (!CheckExprParam(i, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[i] = 0;
                                bGmesCmd[i] = false;
                                GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            }
                            else
                            {
                                bGmesCmd[i] = GmesDllCommand(i, iJobNumber, strCmdName, strSendparEx);
                            }                                
                            if (!Item_bTestStarted) break; //USER STOP 이면 빠져나가자. 
                        }
                    }                        
                        return true;

                case devPCAN:
                            //if (UseDeveloperMode(iJobNumber)) return false;
                            try
                            {
                                Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                                dtStepDelaySet = DateTime.Now;
                                while (!StepDelayTimeCheck())
                                {
                                    if (Item_dStepTimeSet <= 0) break;
                                    System.Threading.Thread.Sleep(20);
                                    if (!Item_bTestStarted) break;
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                }
                            }
                            catch {}
                            ExcutePCAN(strCmdName, iJobNumber);
                            return true;

                case devVector:
                            //if (UseDeveloperMode(iJobNumber)) return false;
                            try
                            {
                                Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                                dtStepDelaySet = DateTime.Now;
                                while (!StepDelayTimeCheck())
                                {
                                    if (Item_dStepTimeSet <= 0) break;
                                    System.Threading.Thread.Sleep(20);
                                    if (!Item_bTestStarted) break;
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                }
                            }
                            catch { }
                            ExcuteVECTOR(strCmdName, iJobNumber);
                            return true;

                case devMTP200:
                            string strOrignParam = strSendparEx;
                            string strGetParam = String.Empty;
                            string strRemainParam = String.Empty;
                            if(!String.IsNullOrEmpty(strSendparEx)) //LOSS 테이블 때문에..
                            {                                
                                int iExpr = strSendparEx.IndexOf(DEFINEEXPR);
                                if (iExpr >= 0)
                                {
                                    strRemainParam = strSendparEx.Substring(0, iExpr);
                                    strSendparEx = strSendparEx.Substring(iExpr);
                                    strSendparEx = strSendparEx.Replace(DEFINEEXPR, String.Empty);
                                    if (DKExpr[iPort].ExcuteLoad(strSendparEx, ref strGetParam))
                                    {
                                        if (iExpr > 0) strSendparEx = strRemainParam + strGetParam;
                                        else strSendparEx = strGetParam;
                                    }
                                } 
                                else
                                    strSendparEx = strOrignParam;
                            }                           
                            
                            //if (UseDeveloperMode(iJobNumber)) return false;
                            if (OpenMTP200 != (int)STATUS.OK)
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "DEVICE ERROR", "DEVICE ERROR", String.Empty, true);
                                return true;
                            }                           

                            return ExcuteMTP200(strCmdName, ref strSendpac, strSendparEx, iJobNumber);
                                        
                case dev5515C:
                            //if (UseDeveloperMode(iJobNumber)) return false;                     
                            if (Open5515C != (int)STATUS.OK)
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "DEVICE ERROR", "DEVICE ERROR", String.Empty, true);
                                return true;
                            }
                            return Excute5515C(strCmdName, ref strSendpac, strSendparEx, iJobNumber);
                case devTC1400A:

                    if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, true);

                        return true;
                    }
                    string strDIDDataType = String.Empty;
                    string strErrReason = String.Empty;

                    bool bFindPack = false;
                    switch (LstJOB_CMD[iJobNumber].TYPE)
                    {
                        //case devSocket: bFindPack = FindPacSocket(strCmdName, strSendparEx, ref strSendpac, ref strDIDDataType, ref strErrReason); break;

                        case devTC1400A: bFindPack = FindPacTC1400A(strCmdName, strSendparEx, ref strSendpac, ref strDIDDataType, ref strErrReason); break;
                    }

                    if (bFindPack)
                    {
                        switch (LstJOB_CMD[iJobNumber].TYPE)
                        {
                            //case devSocket:
                            //    ExcuteSocket(strSendpac, dTimeout, iSendRecvOption, dDelayTime, LstJOB_CMD[iJobNumber].CMD, strSendparEx, strDIDDataType);
                            //    break;
                            case devTC1400A:
                                strSendparEx = String.Empty;
                                ExcuteTC1400A(strSendpac, dTimeout, iSendRecvOption, dDelayTime, LstJOB_CMD[iJobNumber].CMD, strSendparEx, strDIDDataType);
                                break;
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(strErrReason))
                        {
                            strErrReason = "UNKNOWN COMMAND ERROR";
                        }
                        GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), strErrReason, String.Empty, true);
                    }
                    return true;
                case devKEITHLEY:
                            //if (UseDeveloperMode(iJobNumber)) return false;     
                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR" + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, LstJOB_CMD[iJobNumber].PAR1, true);
                                return true;
                            }

                            if (OpenKEITHLEY != (int)STATUS.OK)
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "DEVICE ERROR", "DEVICE ERROR", String.Empty, true);
                                return true;
                            }
                            return ExcuteKEITHLEY(strCmdName, ref strSendpac, strSendparEx, iJobNumber);
                           

                case devODAPWR:
                            //if (UseDeveloperMode(iJobNumber)) return false;
                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR" + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, LstJOB_CMD[iJobNumber].PAR1, true);
                                return true;
                            }

                            if (FindPacOdaPower(strCmdName, ref strSendpac))
                            {   
                                if (strSendpac.Contains("<DATA>") && strSendparEx.Length < 1)
                                {
                                    RETCOUNT_DIC[iPort] = 0;
                                    GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                    return true;
                                }
                                else
                                {
                                    strSendpac = strSendpac.Replace("<DATA>", strSendparEx);
                                }
                                strSendpac += "\n";
                                if (strSendpac.Contains("?"))
                                {
                                    DKACTOR.SendRecvCmd((int)COMSERIAL.ODAPWR, strSendpac, dTimeout, (int)MODE.SENDRECV, 0.6, (int)RS232.TEXT, LstJOB_CMD[iJobNumber].CMD, "", anlPack);
                                }
                                else
                                {
                                    DKACTOR.SendRecvCmd((int)COMSERIAL.ODAPWR, strSendpac, dTimeout, (int)MODE.SEND, 0.6, (int)RS232.TEXT, LstJOB_CMD[iJobNumber].CMD, "", anlPack);
                                }
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            }
                            return true;

                case devTC3000:
                            if (strCmdName.Equals("CHANGE_BAUDRATE"))
                            {
                                if (String.IsNullOrEmpty(strSendparEx) || !CheckBaudRate(strSendparEx))
                                {
                                    GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + strSendparEx, "PAR1 ERROR : " + strSendparEx, String.Empty, true);
                                }
                                else
                                {
                                    if (ChangeTC3000Baudrate(strSendparEx))
                                    {
                                        GateWayMsgProcess((int)STATUS.OK, "Change Baud Rate:" + strSendparEx, strSendparEx, String.Empty, true);
                                    }
                                    else
                                    {
                                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + strSendparEx, "PAR1 ERROR : " + strSendparEx, String.Empty, true);
                                    }
                                }
                                return true;
                            }                                                

                            //if (UseDeveloperMode(iJobNumber)) return false;
                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR" + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, LstJOB_CMD[iJobNumber].PAR1, true);
                                return true;
                            }

                            if (FindPacTC3000(strCmdName, ref strSendpac))
                            {   //TC3000은 RS232.TEXT 타입이다.    
                                if (strSendpac.Contains("<DATA>") && strSendparEx.Length < 1)
                                {
                                    RETCOUNT_DIC[iPort] = 0;
                                    GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);                         
                                    return true; 
                                }
                                // ADDRESS 관련 : 콜론 필터링
                                if (strSendpac.Contains("<DATA>") && strSendparEx.Length > 0)
                                {
                                    switch (strCmdName)
                                    {
                                        case "SET_TESETER_BD_ADDR":
                                        case "SET_DUT_BD_ADDR":
                                            strSendparEx = strSendparEx.Replace(":", String.Empty); break;
                                        default:
                                            break;
                                    }
                                }

                                strSendpac = strSendpac + "\n";
                                DKACTOR.SendRecvCmd((int)COMSERIAL.TC3000, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.TEXT, LstJOB_CMD[iJobNumber].CMD, strSendparEx, anlPack);                                
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);                      
                            }
                            return true;

                case dev34410a:                            
                            string strSendpacOption = String.Empty;
                            if (FindPac34410A(strCmdName, ref strSendpac, ref strSendpacOption))
                            {
                                try
                                {
                                    Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                                    dtStepDelaySet = DateTime.Now;
                                    while (!StepDelayTimeCheck())
                                    {
                                        if (Item_dStepTimeSet <= 0) break;
                                        System.Threading.Thread.Sleep(20);
                                        if (!Item_bTestStarted) break;
                                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                    }
                                }
                                catch { }                                
                                Excute34410A(strCmdName, iJobNumber, strSendpac, strCmdName, strSendparEx, strSendpacOption);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            }
                            return true;
                case devMTP120A:
                    //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                    if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR" + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, LstJOB_CMD[iJobNumber].PAR1, true);
                        return true;
                    }
                    strSendpacOption = String.Empty;
                    if (FindPacMTP120A(strCmdName, ref strSendpac, ref strSendpacOption))
                    {
                        try
                        {
                            Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                            dtStepDelaySet = DateTime.Now;
                            while (!StepDelayTimeCheck())
                            {
                                if (Item_dStepTimeSet <= 0) break;
                                System.Threading.Thread.Sleep(20);
                                if (!Item_bTestStarted) break;
                                if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                            }
                        }
                        catch { }

                        //if (strSendpac.Contains("<DATA>") && strSendparEx.Length < 1)
                        if (strSendpac.Contains("<PORT>") || strSendpac.Contains("<DATA>")) //PORT: OUT_A1, IN_A1, DATA: 입력값
                        {
                            if (strSendparEx.Length < 1)
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }
                        }

                        if (strSendpac.Contains("<PORT>") || strSendpac.Contains("<DATA>"))
                        {
                            //strSendpac = strSendpac.Replace("<DATA>", strSendparEx);

                            string strReturnValue = string.Empty;
                            MTP120AReplaceParam(strCmdName, strSendpac, strSendparEx, ref strReturnValue);
                            strSendpac = strReturnValue;
                        }
                        //MTP120은 위에서 다 처리하기 때문에 아래에서 추가로 안한다.
                        strSendparEx = string.Empty;
                        ExcuteMTP120A(strCmdName, iJobNumber, strSendpac, strCmdName, strSendparEx, strSendpacOption);
                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                    }
                    return true;

                case devDLLGate:

                            if (OpenDLLGate != (int)STATUS.OK)
                            {                                
                                GateWayMsgProcess((int)STATUS.CHECK, "NOT READY DLLGATE", "NOT READY DLLGATE", String.Empty, true);
                                return true;
                            }

                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR" + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, LstJOB_CMD[iJobNumber].PAR1, true);
                                return true;
                            }

                            if (FindPacDLLGATE(strCmdName, ref strSendpac))
                            {
                                if (strCmdName.Contains("PORT_OPEN") && String.IsNullOrEmpty(strSendparEx))
                                {
                                    strSendparEx = FixNadPort.ToString();
                                }

                                ExcuteDLLGATE(strSendpac, dTimeout, iSendRecvOption, dDelayTime, LstJOB_CMD[iJobNumber].CMD, strSendparEx);
                            }
                            else
                            {
                                GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                            
                            }
                            return true;

                case devMELSEC:

                            if (!CheckExprParam(iPort, iJobNumber, ref strSendparEx))
                            {
                                RETCOUNT_DIC[iPort] = 0;                                
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }

                            try
                            {
                                Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                                dtStepDelaySet = DateTime.Now;
                                while (!StepDelayTimeCheck())
                                {
                                    if (Item_dStepTimeSet <= 0) break;
                                    System.Threading.Thread.Sleep(20);
                                    if (!Item_bTestStarted) break;
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                                }
                            }
                            catch { }
                            ExcuteMELSEC(strCmdName, strSendparEx);
                            return true;

                default:
                            GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);                                
                            return true; 
            }

        }

        private void CloseCCMPort()
        {
            DKACTOR.runningStop((int)COMSERIAL.CCM);
            DKACTOR.CommOff((int)COMSERIAL.CCM);
            STEPMANAGER_VALUE.InitCcmGpsStructure();
        }

        private void MCTM_GPRMC_COMMAND(string strCommand)
        {
            switch (strCommand)
            {                
                case "READ_GPRMC_RELIABILITY":
                    if (String.IsNullOrEmpty(STEPMANAGER_VALUE.GPRMC.strReliability))
                        GateWayMsgProcess((int)STATUS.NG, "NO DATA", "NO DATA", String.Empty, true);
                    else
                    {
                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.GPRMC.strReliability, STEPMANAGER_VALUE.GPRMC.strReliability, String.Empty, true);
                        MessageLogging((int)LOGTYPE.RX, STEPMANAGER_VALUE.GPRMC.strFullPacket + ">>", (int)DEFINES.SET1);
                    }
                    return;

                case "READ_GPRMC_LATITUDE":
                    if (String.IsNullOrEmpty(STEPMANAGER_VALUE.GPRMC.strlatitude))
                        GateWayMsgProcess((int)STATUS.NG, "NO DATA", "NO DATA", String.Empty, true);
                    else
                    {
                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.GPRMC.strlatitude, STEPMANAGER_VALUE.GPRMC.strlatitude, String.Empty, true);
                        MessageLogging((int)LOGTYPE.RX, STEPMANAGER_VALUE.GPRMC.strFullPacket + ">>", (int)DEFINES.SET1);
                    }
                    return;
                case "READ_GPRMC_LONGITUDE":
                    if (String.IsNullOrEmpty(STEPMANAGER_VALUE.GPRMC.strlongitude))
                        GateWayMsgProcess((int)STATUS.NG, "NO DATA", "NO DATA", String.Empty, true);
                    else
                    {
                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.GPRMC.strlongitude, STEPMANAGER_VALUE.GPRMC.strlongitude, String.Empty, true);
                        MessageLogging((int)LOGTYPE.RX, STEPMANAGER_VALUE.GPRMC.strFullPacket + ">>", (int)DEFINES.SET1);
                    }
                    return;
                default:
                    GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                    return;
            }

            
        }

        private void KillThreadObject(Thread theThread)
        {
            try
            {
                if (theThread != null)
                {
                    if (theThread.IsAlive)
                    {   //ReadThread 스레드가 살아있으면 최대 0.5 초간 자연사 대기
                        theThread.Join(500);                        
                    }

                    if (theThread.IsAlive)
                    {   //ReadThread 스레드가 그래도 살아있으면 강제종료
                        theThread.Abort();
                    }
                }

            }
            catch { }
        }

        private bool SplitStringValue(string strOrgin, string strTag, int iDesPos, ref string strRtnValue)
        {
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strOrgin, strTag);

            if (tmpString.Length < 1 || tmpString.Length <= iDesPos) return false;

            strRtnValue = tmpString[iDesPos];
            return true;
        }

        //GMES 스텝체크 일경우 MODEL SUFFIX 리스트를 자동으로 만드는 루틴

        private void ClearSuffixList()
        {
            STEPMANAGER_VALUE.LstModel = new List<string>();
            STEPMANAGER_VALUE.LstSuffix = new List<SUFFIXLIST>();
        }

        public void LoadPWUserInfo()
        {
            PWUSER[] testUser = new PWUSER[(int)USERLIMIT.MAX];

            for (int i = 0; i < testUser.Length; i++)
            {
                testUser[i] = new PWUSER();
            }

            int iListCount = DKLoggerPC.GetPasswordUserCount();

            if (iListCount > 0)
            {
                if (iListCount > (int)USERLIMIT.MAX)
                {
                    iListCount = (int)USERLIMIT.MAX;
                }

                for (int i = 0; i < iListCount; i++)
                {
                    testUser[i] = DKLoggerPC.GetPasswordUserData(i);
                }
            }

            STEPMANAGER_VALUE.SetUserInformation(testUser);
   
        }

        public void LoadSuffixList()
        {
            ClearSuffixList();

            int iCount = 0;
            try
            {
                string strCount = DKLoggerMR.LoadSuffix("INFORMATION", "MODEL_COUNT");
                iCount = int.Parse(strCount);

                if (iCount > 0)
                {
                    string strModelName = String.Empty;
                    string strSection = String.Empty;
                    for (int i = 0; i < iCount; i++)
                    {
                        strSection = "MODEL_" + i.ToString();
                        strModelName = DKLoggerMR.LoadSuffix(strSection, "NAME");
                        STEPMANAGER_VALUE.LstModel.Add(strModelName);
                        int iSuffixCount = 0;

                        try
                        {
                            string strSuffixCount = DKLoggerMR.LoadSuffix(strSection, "SUFFIX_COUNT");
                            iSuffixCount = int.Parse(strSuffixCount);
                            string strSuffx = String.Empty;
                            if (iSuffixCount > 0)
                            {
                                SUFFIXLIST srtSuffixData = new SUFFIXLIST();
                                for (int j = 0; j < iSuffixCount; j++)
                                {
                                    srtSuffixData.iDx = i;
                                    srtSuffixData.strSuffix = DKLoggerMR.LoadSuffix(strSection, "SUFFIX_" + j.ToString());
                                    STEPMANAGER_VALUE.LstSuffix.Add(srtSuffixData);
                                }
                            }
                        }
                        catch { ClearSuffixList(); return; }

                    }
                }


            }
            catch { ClearSuffixList(); return; }

        }

        private void AddSuffixList(string strModel, string strSuffix)
        {
            if (STEPMANAGER_VALUE.LstModel == null || STEPMANAGER_VALUE.LstSuffix == null) return;

            int iDicKey = 0;
            if (!STEPMANAGER_VALUE.LstModel.Contains(strModel)) //model 이 리스트에 존재하지 않으면 추가.
            {
                STEPMANAGER_VALUE.LstModel.Add(strModel);
                iDicKey = STEPMANAGER_VALUE.LstModel.Count - 1;
            }
            else
            {
                iDicKey = STEPMANAGER_VALUE.LstModel.IndexOf(strModel); //model 이 리스트에 존재하면 해당 인덱스만 참조.
            }

            bool bFind = false;
            SUFFIXLIST tmpSuffix = new SUFFIXLIST();
            for (int i = 0; i < STEPMANAGER_VALUE.LstSuffix.Count; i++)
            {
                tmpSuffix = STEPMANAGER_VALUE.LstSuffix[i];
                if (tmpSuffix.iDx.Equals(iDicKey) && tmpSuffix.strSuffix.Equals(strSuffix))
                {
                    bFind = true;
                }
            }

            if (!bFind) //suffix 가 리스트에 존재하지 않으면 추가.
            {
                tmpSuffix.iDx = iDicKey;
                tmpSuffix.strSuffix = strSuffix;
                STEPMANAGER_VALUE.LstSuffix.Add(tmpSuffix);
            }

        }

        public void SaveSuffixFile()
        {
            if (STEPMANAGER_VALUE.LstModel == null || STEPMANAGER_VALUE.LstSuffix == null) return;

            DKLoggerMR.ClearSuffix();

            DKLoggerMR.SaveSuffix("INFORMATION", "MODEL_COUNT", STEPMANAGER_VALUE.LstModel.Count.ToString());

            string strModelName = String.Empty;
            for (int i = 0; i < STEPMANAGER_VALUE.LstModel.Count; i++)
            {
                strModelName = "MODEL_" + i.ToString();
                DKLoggerMR.SaveSuffix(strModelName, "NAME", STEPMANAGER_VALUE.LstModel[i]);

                SUFFIXLIST srtSuffix = new SUFFIXLIST();
                int iScount = 0;
                for (int j = 0; j < STEPMANAGER_VALUE.LstSuffix.Count; j++)
                {
                    srtSuffix = STEPMANAGER_VALUE.LstSuffix[j];
                    if (srtSuffix.iDx.Equals(i))
                    {
                        DKLoggerMR.SaveSuffix(strModelName, "SUFFIX_" + iScount.ToString(), srtSuffix.strSuffix);
                        iScount++;
                    }
                }
                DKLoggerMR.SaveSuffix(strModelName, "SUFFIX_COUNT", iScount.ToString());

            }

        }

        private void ExcuteTC1400A(string strPack, double dTimeout, int iSendRecvOption, double dDelaySec, string strCommandTBLName, string strParam, string strDataType)
        {
            if (dDelaySec > 0)
            {
                DelayChecker(dDelaySec, false);
            }

            ETHERNETSTRUCT tmpParam = new ETHERNETSTRUCT();

            tmpParam.dTimeOut = dTimeout;
            tmpParam.strCommandName = strCommandTBLName;
            tmpParam.strSendPack = strPack;
            tmpParam.strParam = strParam;

            tmpParam.strDataType = strDataType;

            switch (strCommandTBLName)
            {
                case "SOCKET_CONNECT":
                    tmpParam.iCommandType = (int)ETHERNETCOMMAND.CONNECT;
                    tmpParam.strParam = String.Empty;

                    string strParam1 = LoadINI("COMPORT", "TC1400A");
                    string strParam2 = String.Empty;

                    if (String.IsNullOrEmpty(LstJOB_CMD[iNowJobNumber].PAR1))//param 없으면 디폴트 적용 (TC1400A)
                    {
                        strParam2 = "1234"; //테스콤 IP는 192.168.100.251 가 디폴트라고 합니다.
                    }
                    else
                    {
                        strParam2 = LstJOB_CMD[iNowJobNumber].PAR1; //IPC:49154
                    }
                    tmpParam.strParam = strParam1 + "/" + strParam2;
                    if (strParam1.Equals("0") || strParam2.Equals("0"))
                    {
                        GateWayMsgProcess((int)STATUS.NG, "No Connection IP/Port", "No Connection IP/Port", tmpParam.strParam, true);
                        return;
                    }
                    break;

                case "SOCKET_DISCONNECT":
                    tmpParam.iCommandType = (int)ETHERNETCOMMAND.DISCONNECT;
                    tmpParam.strParam = String.Empty;
                    break;

                case "SEND_MESSAGE":

                    if (!DK_TC1400A.IsConnected())
                    {
                        GateWayMsgProcess((int)STATUS.NG, "SOCKET DISCONNECTED", "SOCKET DISCONNECTED", String.Empty, true);
                        return;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(strParam))
                        {
                            byte[] bMessage = Encoding.UTF8.GetBytes(strParam);
                            tmpParam.strSendPack = BitConverter.ToString(bMessage).Replace("-", " ");
                            tmpParam.strParam = String.Empty;
                            tmpParam.iCommandType = (int)ETHERNETCOMMAND.SENDRECV;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 NO MESSAGE", "PAR1 NO MESSAGE", String.Empty, true);
                            return;
                        }
                    }

                    break;
                default:
                    if (!DK_TC1400A.IsConnected())
                    {
                        GateWayMsgProcess((int)STATUS.NG, "SOCKET DISCONNECTED", "SOCKET DISCONNECTED", String.Empty, true);
                        return;
                    }

                    //findpac 에서 이미 파라미터를 실어버린다.
                    tmpParam.strParam = String.Empty;

                    switch (iSendRecvOption)
                    {
                        case (int)MODE.SEND: tmpParam.iCommandType = (int)ETHERNETCOMMAND.SEND; break;
                        case (int)MODE.SENDRECV: tmpParam.iCommandType = (int)ETHERNETCOMMAND.SENDRECV; break;
                        default:
                            GateWayMsgProcess((int)STATUS.NG, "Unknown Option", "Unknown Option", String.Empty, true);
                            return;
                    }

                    break;
            }

            DK_TC1400A.LauncherRecvThread(tmpParam);
            return;

        }

        //절차진행하는 사이클엔진에서 GMES 명령을 수행하는 루틴
        private bool GmesDllCommand(int iPort, int iJobNum, string strCmdType, string strParam)
        {
            bool bRtnVal = false;
            int  iMesExcute = 0;
            int iParam = 0; //KALS 용도.
            int iResVal = 0; //KALS 용도.
            string strTmpFileName = String.Empty; //KALS 용도.
            string strResErrMsg = String.Empty; //KALS ERROR MSG
                                                
            //CSMES
            string strRetMsg = string.Empty;
            /*
            //GMES 의 치명적 버그로 인해 아래 코드 추가. GMES OFF 해도 다운받은 데이터가 살아있기 때문에...
            if (!strCmdType.Equals("STEP_CHECK") && !strCmdType.Equals("KIS_KEY_DOWNLOAD_MANUAL"))
            {
                if (!STPCHECK_DIC[iPort])
                {
                    MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                    GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO STEP CHECK", String.Empty, true);
                    return false;
                }
            }
            */

            switch (strCmdType)
            {
                case "STEP_CHECK":
                    MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                    UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + strParam);
                                        
                    if (Item_bUseBarcode)
                    {
                        if (Item_WIPID.Length == Item_iWIPSIZE) //바코드 스캐너로 읽었다면
                        {
                            //iMesExcute = DKGMES.GMES_StepCheck(iPort, Item_WIPID);
                            //CSMES
                            if (STEPMANAGER_VALUE.bUseOSIMES)
                            {
                                Item_WIPID = Item_WIPID.ToUpper();
                                MessageLogging((int)LOGTYPE.TX, Item_WIPID, iPort);
                                UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + Item_WIPID);
                                iMesExcute = DKOSIMES.StepCheck(iPort, Item_WIPID, out strRetMsg);
                            }
                            else
                                iMesExcute = DKGMES.GMES_StepCheck(iPort, Item_WIPID);
                            STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = Item_WIPID; //KALS UP LOAD 용
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strParam))
                            {
                                //iMesExcute = DKGMES.GMES_StepCheck(iPort, strParam);
                                //CSMES
                                if (STEPMANAGER_VALUE.bUseOSIMES)
                                {
                                    strParam = strParam.ToUpper();
                                    iMesExcute = DKOSIMES.StepCheck(iPort, strParam, out strRetMsg);
                                }
                                else
                                    iMesExcute = DKGMES.GMES_StepCheck(iPort, strParam);
                                STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strParam; //KALS UP LOAD 용
                            }
                            else
                            {
                                bRtnVal = false;
                                GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "BARCODE SCANNER READ SIZE FAIL.", String.Empty, true);
                                return bRtnVal;
                            }
                        }
                    }
                    else //아니면 파라미터 값으로
                    {
                        if (String.IsNullOrEmpty(strParam))
                        {
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO PARAM(WIP ID)", String.Empty, true);
                            return false;
                        }
                        //iMesExcute = DKGMES.GMES_StepCheck(iPort, strParam);
                        //CSMES
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                        {
                            strParam = strParam.ToUpper();
                            iMesExcute = DKOSIMES.StepCheck(iPort, strParam, out strRetMsg);
                        }
                        else
                            iMesExcute = DKGMES.GMES_StepCheck(iPort, strParam);
                        STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strParam; //KALS UP LOAD 용
                    }

                    //CSMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                        MessageLogging((int)LOGTYPE.RX, strRetMsg, iPort);

                    //LGEVH 202306
                    bool bIsCmdChangeJob = false;
                    string strCmdChangeJob = "CHANGE_JOB";

                    int iLogCount = GetJOBListCount();
                    for (int i = 0; i < iLogCount; i++)
                    {
                        string strName = GetJOBString(i, (int)sIndex.CMD);
                        string strRun = GetJOBString(i, (int)sIndex.ACTION);

                        if (!string.IsNullOrWhiteSpace(strName))
                        {
                            if (strName.Equals(strCmdChangeJob) && !strRun.Equals("SKIP"))
                            {
                                bIsCmdChangeJob = true;
                                break;
                            }
                        }
                    }

                    //if (ConfigLoad("OPTION", devStepCheckMode))
                    //{
                    //    bIsCmdChangeJob = true; //DEVSTEPCHECK MODE 에서는 CHANGE JOB 없어도 검사 되도록 설정
                    //}
                    //CSMES
                    if (ConfigLoad("OPTION", devStepCheckMode) || STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        bIsCmdChangeJob = true; //DEVSTEPCHECK MODE 에서는 CHANGE JOB 없어도 검사 되도록 설정
                    }

                    if (iMesExcute == 0)
                    {
                        //LGEVH
                        if (bIsCmdChangeJob)
                        {
                            STPCHECK_DIC[iPort] = true;
                            //GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), DKGMES.GMES_GetWipInfo(iPort), String.Empty, true);
                            if (STEPMANAGER_VALUE.bUseOSIMES)
                                GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), DKOSIMES.GetWipInfo(iPort), String.Empty, true);
                            else
                                GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), DKGMES.GMES_GetWipInfo(iPort), String.Empty, true);
                            bRtnVal = true;

                            //STEP CHECK 성공이면 해당 MODEL & SUFFIX 저장해두자.

                            string strTmpModel = String.Empty;
                            string strTmpSuffix = String.Empty;

                            //bool bName = DKGMES.GMES_GetInsp(iPort, "@NAME", ref strTmpModel);
                            //bool bSuffix = DKGMES.GMES_GetInsp(iPort, "@SUFFIX", ref strTmpSuffix);
                            //if (bName && bSuffix)
                            //{
                            //    if (!String.IsNullOrEmpty(strTmpModel) && !String.IsNullOrEmpty(strTmpSuffix))
                            //    {
                            //        AddSuffixList(strTmpModel, strTmpSuffix);
                            //    }
                            //}

                            //CSMES
                            bool bName = false;
                            bool bSuffix = false;

                            if (STEPMANAGER_VALUE.bUseOSIMES)
                            {
                                bName = DKOSIMES.GetInsp(iPort, "@NAME", ref strTmpModel);
                                bSuffix = DKOSIMES.GetInsp(iPort, "@SUFFIX", ref strTmpSuffix);
                            }
                            else
                            {
                                bName = DKGMES.GMES_GetInsp(iPort, "@NAME", ref strTmpModel);
                                bSuffix = DKGMES.GMES_GetInsp(iPort, "@SUFFIX", ref strTmpSuffix);
                            }

                            if (bName && bSuffix)
                            {
                                if (!String.IsNullOrEmpty(strTmpModel) && !String.IsNullOrEmpty(strTmpSuffix))
                                {
                                    AddSuffixList(strTmpModel, strTmpSuffix);
                                }
                            }
                        }
                        else
                        {
                            STPCHECK_DIC[iPort] = false;
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(),
                                "Passed MES.STEP.CHECK. However, the CHANGE_JOB command is not in the job file or is skipped!", String.Empty, true);
                            bRtnVal = false;
                        }
                    }
                    else
                    {
                        STPCHECK_DIC[iPort] = false;
                        //GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);
                        //CSMES
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKOSIMES.GetErrString(iMesExcute), String.Empty, true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);
                        //string ttt = DKGMES.GMES_GetErrString(iMesExcute);
                        bRtnVal = false;
                    }
                   
                    
                    return bRtnVal;

                case "PACK_STEP_CHECK": //CS MES 에서 PACKING 공정에서만 수행하는 명령어
                                        //CSMES
                    if (!STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        bRtnVal = false;
                        MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                        GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "JUST ONLY USE CS.", String.Empty, true);
                        return bRtnVal;
                    }
                    MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                    //LockOfSend((int)DICTYPE.SENDPACK, iPort, iJobNum, strCmdType + " : " + strParam);
                    UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + strParam);
                    //SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + strParam;
                    if (Item_bUseBarcode)
                    {
                        if (Item_WIPID.Length == Item_iWIPSIZE) //바코드 스캐너로 읽었다면
                        {
                            if (STEPMANAGER_VALUE.bUseOSIMES)
                            {
                                Item_WIPID = Item_WIPID.ToUpper();
                                MessageLogging((int)LOGTYPE.TX, Item_WIPID, iPort);                                
                                UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + Item_WIPID);
                                iMesExcute = DKOSIMES.PackStepCheck(iPort, Item_WIPID, out strRetMsg);
                            }
                            //else
                            //    iMesExcute = DKGMES.GMES_StepCheck(iPort, Item_WIPID);

                            STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = Item_WIPID; //KALS UP LOAD 용
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strParam))
                            {
                                if (STEPMANAGER_VALUE.bUseOSIMES)
                                {
                                    strParam = strParam.ToUpper();
                                    iMesExcute = DKOSIMES.PackStepCheck(iPort, strParam, out strRetMsg);
                                }
                                //else
                                //    iMesExcute = DKGMES.GMES_StepCheck(iPort, strParam);

                                STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strParam; //KALS UP LOAD 용
                            }
                            else
                            {
                                bRtnVal = false;
                                GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "BARCODE SCANNER READ SIZE FAIL.", String.Empty, true);
                                return bRtnVal;
                            }
                        }
                    }
                    else //아니면 파라미터 값으로
                    {
                        if (String.IsNullOrEmpty(strParam))
                        {
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO PARAM(WIP ID)", String.Empty, true);
                            return false;
                        }

                        if (STEPMANAGER_VALUE.bUseOSIMES)
                        {
                            strParam = strParam.ToUpper();
                            iMesExcute = DKOSIMES.PackStepCheck(iPort, strParam, out strRetMsg);
                        }
                        //else
                        //    iMesExcute = DKGMES.GMES_StepCheck(iPort, strParam);

                        STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID = strParam; //KALS UP LOAD 용
                    }

                    if (STEPMANAGER_VALUE.bUseOSIMES)
                        MessageLogging((int)LOGTYPE.RX, strRetMsg, iPort);

                    if (iMesExcute == 0)
                    {
                        STPCHECK_DIC[iPort] = true;

                        if (STEPMANAGER_VALUE.bUseOSIMES)
                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), DKOSIMES.GetWipInfo(iPort), String.Empty, true);
                        //else
                        //    GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), DKGMES.GMES_GetWipInfo(iPort), String.Empty, true);

                        bRtnVal = true;
                    }
                    else
                    {
                        STPCHECK_DIC[iPort] = false;
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKOSIMES.GetErrString(iMesExcute), String.Empty, true);
                        //else
                        //    GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);

                        //string ttt = DKGMES.GMES_GetErrString(iMesExcute);
                        bRtnVal = false;
                    }
                    return bRtnVal;
                case "STEP_COMPLETE":
                    
                    if (STEPMANAGER_VALUE.bInteractiveMode)
                    {
                        MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                        GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "DO NOT INTERATIVE.", String.Empty, true);
                        return false;
                    }

                    //CSMES
                    if (!STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        if (!STPCHECK_DIC[iPort])
                        {
                            bRtnVal = false;
                            MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO STEP CHECK.", String.Empty, true);
                            return bRtnVal;
                        }
                    }
                    

                    bool bLastRes = true;
                    bool bChkErrRes = false;

                    //CSMES
                    //2023-05-31 PKS추가
                    GetFinalResult(iPort, out bLastRes, out bChkErrRes);
                    #region 2023-05-31 PKS삭제
                    //for (int i = 0; i < LstTST_RES[iPort].Count; i++) //최종결과 검색.
                    //{
                    //    switch (LstTST_RES[iPort][i].iStatus)
                    //    {
                    //        case (int)STATUS.OK: break;
                    //        case (int)STATUS.SKIP: break;

                    //        case (int)STATUS.NG: bLastRes = false; break;


                    //        case (int)STATUS.CHECK:  
                    //        case (int)STATUS.ERROR:  
                    //        case (int)STATUS.MESERR:
                    //        case (int)STATUS.DSUB: bChkErrRes = true; break;


                    //        default: break; //원래 OK, SKIP 이 아니면 모조리 GMES 에 ng 보고 했으나 이동성 책임 요청으로 NG 만 올리도록 설정함.
                    //        //default: bLastRes = false; break;
                    //    }

                    //    if (!bLastRes) break;
                    //}
                    #endregion

                    if (bChkErrRes)
                    {//원래 OK, SKIP 이 아니면 모조리 GMES 에 ng 보고 했으나 이동성 책임 요청으로 NG 만 올리도록 설정함. check. error, mes 등의 오류는 올리지 않는다.
                        MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                        GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "SKIPPED by CHECK,ERROR,MES", String.Empty, true);
                        return false;
                    }

                    string strResultString = "RESULT=" + STATUS.OK.ToString();
                    if (!bLastRes)
                    {
                        if (CheckInspectionRetry() > 0)
                        {   //검사 전체RETRY 가 설정되어 있을경우 STEP COMPLETE 는 SKIP 하자.
                            GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "INSPECTION RETRY", String.Empty, true);
                            return true;
                        }

                        if (bOnlyOKGmes)
                        {
                            GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "ONLY OK GMES", String.Empty, true);
                            return true;
                        }

                        strResultString = "RESULT=" + STATUS.NG.ToString();
                    }
                    strResultString += GmesItemCoding(iPort, iJobNum);
                    
                    if (!bLastRes)
                    {
                        strResultString += NgCommandList(iPort, iJobNum);
                    }
                    //비아스키 다시한번 걸러보자.
                    DK_DECISION tmpDec = new DK_DECISION();
                    string strLastMsg = String.Empty;
                    //tmpDec.CheckNoneAscii(strResultString, ref strLastMsg);

                    //CSMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        //{}삽입
                        int FirstCommaPos = strResultString.IndexOf(',');
                        if (FirstCommaPos > 0)
                        {
                            strResultString = strResultString.Insert(FirstCommaPos + 1, "{");
                            strResultString += "}";
                        }

                        string strOSI_LocalIp = DKLoggerPC.LoadINI("OSI", "CSMESLOCALIP");
                        strParam = strParam.Replace("<LOCALIP>", strOSI_LocalIp);
                       
                        if (!String.IsNullOrEmpty(Item_WIPID))
                        {
                            Item_WIPID = Item_WIPID.ToUpper();
                        }
                        
                        string tmpStr = LoadINI("OSI", "WHCODE");

                        DKOSIMES.EndTime = DateTime.Now.ToString("HH:mm:ss");
                        string strResultString2 = "WIPID=" + Item_WIPID + "," + strParam + "," + "WHCODE=" + tmpStr + "," +
                                    "WORKING_DATE=" + DKOSIMES.StartTime.Split(' ')[0] + "," +
                                    "START_TIME=" + DKOSIMES.StartTime.Split(' ')[1] + "," +
                                    "END_TIME=" + DKOSIMES.EndTime + "," +
                                    strResultString;

                        tmpDec.CheckNoneAscii(strResultString2, ref strLastMsg);

                        MessageLogging((int)LOGTYPE.TX, strLastMsg, iPort);
                        UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + strLastMsg);
                        
                        //strParm parsing 해서 값 넣기.
                        //CATEGORY_NAME = OSI,HOST_NAME = LGE001,SUFFIX = 1234
                        //20230726 HOST_NAME 을 변경. CONFIG 의 LOCAL IP 로
                        //CATEGORY_NAME = OSI,HOST_NAME = <LOCALIP>,SUFFIX = 1234
                        string[] tmpParamString = System.Text.RegularExpressions.Regex.Split(strParam, ",");
                        string finalResult = STATUS.OK.ToString();
                        string suffix = "NONE";
                        string categoryName = "NONE";
                        if (!bLastRes)
                            finalResult = STATUS.NG.ToString();
                        for (int i = 0; i < tmpParamString.Length; i++)
                        {
                            if (tmpParamString[i].ToUpper().Contains("CATEGORY_NAME"))
                                categoryName = tmpParamString[i].Replace("CATEGORY_NAME", "").Replace("=", "").Trim();
                            if (tmpParamString[i].ToUpper().Contains("SUFFIX"))
                                suffix = tmpParamString[i].Replace("SUFFIX", "").Replace("=", "").Trim();
                        }

                        iMesExcute = DKOSIMES.StepComplete(iPort, Item_WIPID, strLastMsg, finalResult, suffix, categoryName, out strRetMsg);
                        MessageLogging((int)LOGTYPE.RX, strRetMsg, iPort);
                    }
                    else
                    {
                        tmpDec.CheckNoneAscii(strResultString, ref strLastMsg);

                        MessageLogging((int)LOGTYPE.TX, strLastMsg, iPort);
                        //LockOfSend((int)DICTYPE.SENDPACK, iPort, iJobNum, strCmdType + " : " + strLastMsg);
                        UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + strLastMsg);
                        //SENDPACK_DIC[iPort][iJobNum] = strCmdType + " : " + strLastMsg;
                        iMesExcute = DKGMES.GMES_StepComplete(iPort, strLastMsg);
                    }

                    //MessageLogging((int)LOGTYPE.TX, strLastMsg, iPort);
                    //UpdateDictionary((int)DICINDEX.SENDPACK, iPort, iJobNum, strCmdType + " : " + strLastMsg);
                    //iMesExcute = DKGMES.GMES_StepComplete(iPort, strLastMsg);

                    ResetKalsData();

                    if (iMesExcute == 0)
                    {
                        GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), iMesExcute.ToString(), String.Empty, true);
                        bRtnVal = true;
                    }
                    else
                    {
                        //GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKOSIMES.GetErrString(iMesExcute), String.Empty, true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);
                        bRtnVal = false;
                    }
                    return bRtnVal;

                case "STEP_COMPLETE_DETAIL":

                    if (STEPMANAGER_VALUE.bInteractiveMode)
                    {
                        MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                        GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "DO NOT INTERATIVE.", String.Empty, true);
                        return false;
                    }
                    //CSMES
                    if (!STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        bRtnVal = false;
                        MessageLogging((int)LOGTYPE.TX, String.Empty, iPort);
                        GateWayMsgProcess((int)STATUS.SKIP, STATUS.SKIP.ToString(), "JUST ONLY USE CS.", String.Empty, true);
                        return bRtnVal;
                    }

                    bool bLastRes2 = true;
                    bool bChkErrRes2 = false;
                    GetFinalResult(iPort, out bLastRes2, out bChkErrRes2);

                    if (DKOSIMES.EndTime == "")              //STEP_COMPLETE BASIC하면 종료시간이 있음-안할경우 대비
                        DKOSIMES.EndTime = DateTime.Now.ToString("HH:mm:ss");

                    int PassCnt = DKOSIMES.PassCount;
                    int FailCnt = DKOSIMES.FailCount;

                    if (bLastRes2)
                        PassCnt++;
                    else
                        FailCnt++;

                    int Total = PassCnt + FailCnt;

                    string FinalResult = "OK";
                    if (bLastRes2 == false)
                        FinalResult = "NG";

                    string DetailTestLog = WriteTestResultUseCompleteDetail(iJobNum, strNowJobName, DKOSIMES.StartTime, DKOSIMES.EndTime, STEPMANAGER_VALUE.strTactTime,
                                                    FinalResult, STEPMANAGER_VALUE.strProgramVersion, PassCnt.ToString(), FailCnt.ToString(), Total.ToString());

                    string dataSize = "Before DetailLog Data Size : " + DetailTestLog.Length;
                    MessageLogging((int)LOGTYPE.TX, dataSize, iPort);
                    MessageLogging((int)LOGTYPE.TX, DetailTestLog, iPort);

                    //20250626, ATCO 임시 조치. DB 사이즈가 20000 인데 20000이 넘어서 18000 자만 보내도록 수정.
                    if (DetailTestLog.Length > 18000)
                        DetailTestLog = DetailTestLog.Substring(0, 18000);

                    dataSize = "After DetailLog Data Size (MAX : 18000) : " + DetailTestLog.Length;
                    MessageLogging((int)LOGTYPE.TX, dataSize, iPort);

                    //if (DetailTestLog.Length > 20)
                    //    DetailTestLog = DetailTestLog.Substring(0, 20);

                    //strParm parsing 해서 값 넣기.
                    //CATEGORY_NAME=OSI,HOST_NAME=192.168.0.1,SUFFIX=1234
                    string[] tmpParamString1 = System.Text.RegularExpressions.Regex.Split(strParam, ",");
                    string finalResult1 = STATUS.OK.ToString();
                    string suffix2 = "NONE";
                    string categoryName2 = "NONE";
                    if (!bLastRes2)
                        finalResult1 = STATUS.NG.ToString();
                    for (int i = 0; i < tmpParamString1.Length; i++)
                    {
                        if (tmpParamString1[i].ToUpper().Contains("CATEGORY_NAME"))
                            categoryName2 = tmpParamString1[i].Replace("CATEGORY_NAME", "").Replace("=", "").Trim();
                        if (tmpParamString1[i].ToUpper().Contains("SUFFIX"))
                            suffix2 = tmpParamString1[i].Replace("SUFFIX", "").Replace("=", "").Trim();
                    }
                  
                    //(iPort, strLastMsg, finalResult, suffix, categoryName, out strRetMsg);
                    //iMesExcute = DKOSIMES.StepComplete_Detail(iPort, DetailTestLog, out strRetMsg);
                    iMesExcute = DKOSIMES.StepComplete_Detail(iPort, Item_WIPID, DetailTestLog, FinalResult, suffix2, categoryName2, out strRetMsg);

                    MessageLogging((int)LOGTYPE.RX, strRetMsg, iPort);

                    if (iMesExcute == 0)
                    {
                        GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), iMesExcute.ToString(), String.Empty, true);
                        bRtnVal = true;
                    }
                    else
                    {
                        //GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKOSIMES.GetErrString(iMesExcute), String.Empty, true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), DKGMES.GMES_GetErrString(iMesExcute), String.Empty, true);
                        bRtnVal = false;
                    }
                    return bRtnVal;

                case "ITEM_DATA_VIEW":
                    string strReturnValue = String.Empty;
                    //OSIMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        if (DKOSIMES.GetInsp(iPort, strParam, ref strReturnValue))
                        {
                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), strReturnValue, strParam, true);
                            bRtnVal = true;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO DATA", strParam, true);
                            bRtnVal = false;
                        }
                    }
                    else
                    {
                        if (DKGMES.GMES_GetInsp(iPort, strParam, ref strReturnValue))
                        {
                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), strReturnValue, strParam, true);
                            bRtnVal = true;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO DATA", strParam, true);
                            bRtnVal = false;
                        }
                    }
                    return bRtnVal;

                case "KALS_DOWNLOAD_FILENAME":
                    MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                    //1.파라미터 검사
                    iParam = (int)KALSNAME.DID_SEED_GEN11;
                    string tempName = String.Empty;
                    switch (strParam)
                    {
                        case "GM_GB_GEN11":        tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        case "DID_SEED_GEN11_VCP": tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        case "DID_SEED_GEN11":     tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        case "GM_GB_GEN12":        tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        case "DID_SEED_GEN12_VCP": tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        case "DID_SEED_GEN12":     tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        case "DID_SEED_MCTM":      tempName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName; break;
                        default:
                                 GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), "CHECK PAR1", String.Empty, true);
                                 bRtnVal = false;
                                 return bRtnVal;
                    }
                    
                    if (tempName == String.Empty || tempName.Length < 2)
                    {
                        bRtnVal = false;
                        GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO DATA", String.Empty, true);
                    }
                    else
                    {
                        bRtnVal = true;
                        GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), tempName, String.Empty, true);
                    }
                    return bRtnVal;

                case "KALS_GET_KEY":
                case "KALS_GET_KEY_KS":
                case "KALS_GET_KEY_NV":

                    try
                    {
                        DKKALS = new DK_KLAS();
                        MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                        //1.파라미터 검사
                        iParam = (int)KALSNAME.DID_SEED_GEN11;
                        switch (strParam)
                        {
                            case "GM_GB_GEN11":        iParam = (int)KALSNAME.GM_GB_GEN11; break;
                            case "DID_SEED_GEN11_VCP": iParam = (int)KALSNAME.DID_SEED_GEN11_VCP; break;
                            case "DID_SEED_GEN11":     iParam = (int)KALSNAME.DID_SEED_GEN11; break;
                            case "GM_GB_GEN12":        iParam = (int)KALSNAME.GM_GB_GEN12; break;
                            case "DID_SEED_GEN12_VCP": iParam = (int)KALSNAME.DID_SEED_GEN12_VCP; break;
                            case "DID_SEED_GEN12":     iParam = (int)KALSNAME.DID_SEED_GEN12; break;
                            case "DID_SEED_MCTM":      iParam = (int)KALSNAME.DID_SEED_MCTM; break;
                            case "LOCAL_SEED_MCTM": // iParam = (int)KALSNAME.DID_SEED_MCTM; break;                        
                                //파일 꺼내오기
                                string strDownLoadTempFile = String.Empty;
                                if (DKLoggerPC.GetLocalSeedFileForMCTM(ref strDownLoadTempFile))
                                {
                                    byte[] BinByte = FileToBinary(strDownLoadTempFile);
                                    string strBinary = BitConverter.ToString(BinByte).Replace("-", "");
                                    DKLoggerPC.FileDeleteBin(strDownLoadTempFile); //LOCAL 파일삭제
                                    GateWayMsgProcess((int)STATUS.OK, strBinary, strBinary, String.Empty, true);
                                    return true;
                                }
                                else
                                {
                                    GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), "CAN NOT FOUND LOCAL FILE", String.Empty, true);
                                    return false;
                                }


                            case "LOCAL_FILE_GBKEY": // iParam = (int)KALSNAME.DID_SEED_MCTM; break;                        
                                //파일 꺼내오기
                                string strLocalTempFile = "gb-key_20180717210803637.dat";
                                if (DKLoggerPC.GetLocalFileForGBKEY(ref strLocalTempFile))
                                {
                                    byte[] BinByte = FileToBinary(strLocalTempFile);
                                    string strBinary = BitConverter.ToString(BinByte).Replace("-", "");
                                    string[] strGBkey = new string[3];
                                    if (strBinary.Length != 288)
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "SIZE-ERR GM_GB_GEN11(OR GEN12)", "SIZE-ERR GM_GB_GEN11(OR GEN12)", String.Empty, true);
                                        return false;
                                    }
                                    try
                                    {
                                        strGBkey[0] = strBinary.Substring(0, 16 * 2);                   //ECUID     16BYTE
                                        strGBkey[1] = strBinary.Substring(16 * 2, 64 * 2);              //MASTERKEY 64BYTE
                                        strGBkey[2] = strBinary.Substring((16 * 2) + (64 * 2), 64 * 2); //UNLOCKKEY 64BYTE

                                        ExprSaveData((int)DEFINES.SET1, "GB_ECUID", strGBkey[0]);
                                        ExprSaveData((int)DEFINES.SET1, "GB_MASTER", strGBkey[1]);
                                        ExprSaveData((int)DEFINES.SET1, "GB_UNLOCK", strGBkey[2]);
                                    }
                                    catch
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "SIZE-ERR GM_GB_GEN11(OR GEN12)", "SIZE-ERR GM_GB_GEN11(OR GEN12)", String.Empty, true);
                                        return false;
                                    }
                                   
                                    GateWayMsgProcess((int)STATUS.OK, strBinary, strBinary, String.Empty, true);
                                    return true;
                                }
                                else
                                {
                                    GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), "CAN NOT FOUND LOCAL FILE", String.Empty, true);
                                    return false;
                                }

                            default:
                                GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), "CHECK PAR1", String.Empty, true);
                                bRtnVal = false;
                                return bRtnVal;
                        }

                        string strResKey = String.Empty;
                        string strDownLoadFileName = String.Empty;
                        strResErrMsg = String.Empty;

                        switch (strCmdType)
                        {
                            case "KALS_GET_KEY_KS":
                                STEPMANAGER_VALUE.strKALS_SiteCode = "LGEKS"; break;
                            case "KALS_GET_KEY_NV":
                                STEPMANAGER_VALUE.strKALS_SiteCode = "LGENV"; break;
                            default:
                                STEPMANAGER_VALUE.strKALS_SiteCode = "LGEVN"; break;
                        }

                        int iRes = DKKALS.Kals_GetKey(iParam, ref strResKey, ref strDownLoadFileName, ref strResErrMsg);


                        if (iRes == (int)KALSRETURNCODE.ERR_SUCCESS)
                        {
                            switch (strParam)
                            {
                                case "GM_GB_GEN11":
                                case "GM_GB_GEN12":
                                    STEPMANAGER_VALUE.strKALS_DID_SEED_FileName = strDownLoadFileName;
                                    string[] strGBkey = new string[3];
                                    if (strResKey.Length != 288)
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "SIZE-ERR GM_GB_GEN11(OR GEN12)", "SIZE-ERR GM_GB_GEN11(OR GEN12)", String.Empty, true);
                                        return false;
                                    }
                                    try
                                    {
                                        strGBkey[0] = strResKey.Substring(0, 16 * 2);                   //ECUID     16BYTE
                                        strGBkey[1] = strResKey.Substring(16 * 2, 64 * 2);              //MASTERKEY 64BYTE
                                        strGBkey[2] = strResKey.Substring((16 * 2) + (64 * 2), 64 * 2); //UNLOCKKEY 64BYTE

                                        ExprSaveData((int)DEFINES.SET1, "GB_ECUID",  strGBkey[0]);
                                        ExprSaveData((int)DEFINES.SET1, "GB_MASTER", strGBkey[1]);
                                        ExprSaveData((int)DEFINES.SET1, "GB_UNLOCK", strGBkey[2]);
                                    }
                                    catch
                                    {
                                        GateWayMsgProcess((int)STATUS.CHECK, "SIZE-ERR GM_GB_GEN11(OR GEN12)", "SIZE-ERR GM_GB_GEN11(OR GEN12)", String.Empty, true);
                                        return false;
                                    }                                    
                                    break;

                                case "DID_SEED_GEN11_VCP":
                                case "DID_SEED_GEN11":
                                case "DID_SEED_GEN12_VCP":
                                case "DID_SEED_GEN12":
                                case "DID_SEED_MCTM": STEPMANAGER_VALUE.strKALS_DID_SEED_FileName = strDownLoadFileName; break;
                                default:
                                    GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN PARAM", "UNKNOWN PARAM", String.Empty, true);
                                    return false;

                            }
                            GateWayMsgProcess((int)STATUS.OK, strResKey, strResKey, String.Empty, true);
                            bRtnVal = true;
                        }
                        else
                        {
                            switch (strParam)
                            {
                                case "GM_GB_GEN11":
                                case "DID_SEED_GEN11_VCP":
                                case "DID_SEED_GEN11":
                                case "GM_GB_GEN12":
                                case "DID_SEED_GEN12_VCP":
                                case "DID_SEED_GEN12":
                                case "DID_SEED_MCTM": STEPMANAGER_VALUE.strKALS_DID_SEED_FileName = ""; break;
                                default:
                                    GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN PARAM", "UNKNOWN PARAM", String.Empty, true);
                                    return false;
                            }
                            GateWayMsgProcess((int)STATUS.CHECK, strResErrMsg, strResErrMsg, String.Empty, true);
                            bRtnVal = false;
                        }
                        return bRtnVal;
                    }
                    catch (System.Exception ex)
                    {
                        MessageLogging((int)LOGTYPE.RX, "KALS ERROR:" + ex.Message, (int)DEFINES.SET1);
                        GateWayMsgProcess((int)STATUS.CHECK, "KALS ERROR", "KALS ERROR", String.Empty, true);
                        return false;
                    }


                case "KALS_RETURN_WRITE_INFO":
                    try
                    {
                        DKKALS = new DK_KLAS();

                        //1.파라미터 검사
                        iParam = (int)KALSNAME.DID_SEED_GEN11;
                        int iFileNameLength = 21;
                        switch (strParam)
                        {                                 
                            case "GM_GB_GEN11":        iParam = (int)KALSNAME.GM_GB_GEN11;        iFileNameLength = 21; break;
                            case "DID_SEED_GEN11_VCP": iParam = (int)KALSNAME.DID_SEED_GEN11_VCP; iFileNameLength = 21; break;
                            case "DID_SEED_GEN11":     iParam = (int)KALSNAME.DID_SEED_GEN11;     iFileNameLength = 21; break;
                            case "GM_GB_GEN12":        iParam = (int)KALSNAME.GM_GB_GEN12;        iFileNameLength = 21; break;
                            case "DID_SEED_GEN12_VCP": iParam = (int)KALSNAME.DID_SEED_GEN12_VCP; iFileNameLength = 21; break;
                            case "DID_SEED_GEN12":     iParam = (int)KALSNAME.DID_SEED_GEN12;     iFileNameLength = 21; break;
                            case "DID_SEED_MCTM":      iParam = (int)KALSNAME.DID_SEED_MCTM;      iFileNameLength = 21; break;
                            default:
                                bRtnVal = false;
                                MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                                GateWayMsgProcess((int)STATUS.CHECK, STATUS.CHECK.ToString(), "CHECK PAR1", String.Empty, true);
                                return bRtnVal;
                        }

                        strTmpFileName = String.Empty;

                        string strStationName = String.Empty;
                        switch (strParam)
                        {
                            case "GM_GB_GEN11":
                            case "DID_SEED_GEN11_VCP":
                            case "DID_SEED_GEN11":
                            case "GM_GB_GEN12":
                            case "DID_SEED_GEN12_VCP":
                            case "DID_SEED_GEN12":
                            case "DID_SEED_MCTM": strTmpFileName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName;
                                strStationName = "GM_KEY_STATION";
                                break;
                            default:
                                GateWayMsgProcess((int)STATUS.NG, "UNKNOWN PARAM", "UNKNOWN PARAM", String.Empty, true);
                                return false;
                        }

                        if (String.IsNullOrEmpty(strTmpFileName) || strTmpFileName.Length < iFileNameLength)
                        {
                            bRtnVal = false;
                            MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                            GateWayMsgProcess((int)STATUS.CHECK, "CHECK DOWNLOAD FILE", "CHECK DOWNLOAD FILE", String.Empty, true);
                            return bRtnVal;
                        }

                        strResErrMsg = String.Empty;
                        MessageLogging((int)LOGTYPE.TX, strParam + " : " + strTmpFileName, iPort);
                        iResVal = DKKALS.Kals_ReturnWritingInfo(iParam, strTmpFileName, strStationName, ref strResErrMsg);

                        if (iResVal == (int)KALSRETURNCODE.ERR_SUCCESS)
                        {
                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), STATUS.OK.ToString(), String.Empty, true);
                            bRtnVal = true;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, strResErrMsg, strResErrMsg, String.Empty, true);
                            bRtnVal = false;
                        }
                        return bRtnVal;
                    }
                    catch (System.Exception ex)
                    {
                        MessageLogging((int)LOGTYPE.RX, "KALS ERROR:" + ex.Message, (int)DEFINES.SET1);
                        GateWayMsgProcess((int)STATUS.NG, "KALS ERROR", "KALS ERROR", String.Empty, true);
                        return false;
                    }
                    

                case "KALS_WRTIE_PRODUC_INFO":
                    try
                    {
                        DKKALS = new DK_KLAS();

                        //1.파라미터 검사
                        iParam = (int)KALSNAME.DID_SEED_GEN11;
                        int iFileNameLengthinfo = 21;

                        switch (strParam)
                        {
                            case "GM_GB_GEN11":         iParam = (int)KALSNAME.GM_GB_GEN11;         iFileNameLengthinfo = 21; break;
                            case "DID_SEED_GEN11_VCP":  iParam = (int)KALSNAME.DID_SEED_GEN11_VCP;  iFileNameLengthinfo = 21; break;
                            case "DID_SEED_GEN11":      iParam = (int)KALSNAME.DID_SEED_GEN11;      iFileNameLengthinfo = 21; break;
                            case "GM_GB_GEN12":         iParam = (int)KALSNAME.GM_GB_GEN12;         iFileNameLengthinfo = 21; break;
                            case "DID_SEED_GEN12_VCP":  iParam = (int)KALSNAME.DID_SEED_GEN12_VCP;  iFileNameLengthinfo = 21; break;
                            case "DID_SEED_GEN12":      iParam = (int)KALSNAME.DID_SEED_GEN12;      iFileNameLengthinfo = 21; break;
                            case "DID_SEED_MCTM":       iParam = (int)KALSNAME.DID_SEED_MCTM;       iFileNameLengthinfo = 21; break;

                            default:
                                bRtnVal = false;
                                MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                                GateWayMsgProcess((int)STATUS.CHECK, "CHECK PAR1", "CHECK PAR1", String.Empty, true);
                                return bRtnVal;
                        }

                        strTmpFileName = String.Empty;
                        string strStationName2 = String.Empty;
                        switch (strParam)
                        {
                            case "GM_GB_GEN11":
                            case "DID_SEED_GEN11_VCP":
                            case "DID_SEED_GEN11":
                            case "GM_GB_GEN12":
                            case "DID_SEED_GEN12_VCP":
                            case "DID_SEED_GEN12":
                            case "DID_SEED_MCTM": strTmpFileName = STEPMANAGER_VALUE.strKALS_DID_SEED_FileName;
                                strStationName2 = "GM_KEY_STATION";
                                break;
                            default:
                                GateWayMsgProcess((int)STATUS.NG, "UNKNOWN PARAM", "UNKNOWN PARAM", String.Empty, true);
                                return false;
                        }

                        if (String.IsNullOrEmpty(strTmpFileName) || strTmpFileName.Length < iFileNameLengthinfo)
                        {
                            bRtnVal = false;
                            MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                            GateWayMsgProcess((int)STATUS.CHECK, "CHECK DOWNLOAD FILE", "CHECK DOWNLOAD FILE", String.Empty, true);
                            return bRtnVal;
                        }

                        if (String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID) || STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID.Length < 15)
                        {
                            bRtnVal = false;
                            MessageLogging((int)LOGTYPE.TX, strParam, iPort);
                            GateWayMsgProcess((int)STATUS.CHECK, "CHECK WIPID", "CHECK WIPID", String.Empty, true);
                            return bRtnVal;
                        }

                        strResErrMsg = String.Empty;
                        MessageLogging((int)LOGTYPE.TX, strParam + " : " + strTmpFileName + "(WIPID:" + STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID + ")", iPort);
                        iResVal = DKKALS.Kals_WriteProduc_WritingInfo(iParam, strTmpFileName, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID, strStationName2, ref strResErrMsg);

                        if (iResVal == (int)KALSRETURNCODE.ERR_SUCCESS)
                        {
                            GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), STATUS.OK.ToString(), String.Empty, true);
                            bRtnVal = true;
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, strResErrMsg, strResErrMsg, String.Empty, true);
                            bRtnVal = false;
                        }
                        return bRtnVal;
                    }
                    catch (System.Exception ex)
                    {
                        MessageLogging((int)LOGTYPE.RX, "KALS ERROR:" + ex.Message, (int)DEFINES.SET1);
                        GateWayMsgProcess((int)STATUS.NG, "KALS ERROR", "KALS ERROR", String.Empty, true);
                        return false;
                    }
                //AMS

                case "AMS_KEY_DOWNLOAD":
                case "AMS_KEY_DOWNLOAD_GEN12":

                    //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.

                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNum].PAR1))
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "CHECK PAR1", "CHECK PAR1", String.Empty, true);
                        return true;
                    }
                    
                    string[] strPars   = new string[5];
                    string[] strAMSParams = new string[5];
                    string strChangeParam = String.Empty;
                    strPars = LstJOB_CMD[iJobNum].PAR1.Split(',');

                    if (!CheckExprParam(iPort, iJobNum, ref strChangeParam))
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNum].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNum].PAR1, String.Empty, true);
                        return true;
                    }
                    strPars = strChangeParam.Split(',');

                    //이동성 책임 요청으로 인해 생긴 소스 코드문. KIS에서는 KEY 값을 다운 받을때 STID값에 0이 끼여 있으면 0을 제거하는 기능이 들어가 있는데 AMS에는 그런 기능이 없어서 추가함.
                    //STID 값 맨 앞에 0이 끼여있으면 0을 제거하는 소스문

                    int iNonZeroStid = 0;
                    string strTempStid = String.Empty;
                   
                    switch (strPars.Length)
                    {
                        //남경꺼
                        case 5:
                            strTempStid = strPars[4];
                            if (int.TryParse(strTempStid, out iNonZeroStid))
                            {
                                strPars[4] = iNonZeroStid.ToString();
                            }
                            CallAMS(iPort, strPars[0], strPars[1], strPars[2], strPars[3], strPars[4]);
                            break;

                        //베트남꺼
                        case 8:
                            strTempStid = strPars[7];
                            if (int.TryParse(strTempStid, out iNonZeroStid))
                            {
                                strPars[7] = iNonZeroStid.ToString();
                            }

                            if (strCmdType.Equals("AMS_KEY_DOWNLOAD_GEN12"))
                            {
                                //20240627  strFactory 제거, GEN12 에서는 사용하지 않음. 
                                //      iPort, strName,        strIp1,     strIp2,      strId,     strGen, strFactory, strProduct, strStid
                                CallAMS(iPort, strPars[0], strPars[1], strPars[2], strPars[3], strPars[4], strPars[5], strPars[6], strPars[7], (int)AMSTYPE.GEN12);                                
                            }
                            else
                            {
                                //      iPort, strName,        strIp1,     strIp2,      strId,     strGen, strFactory, strProduct, strStid
                                CallAMS(iPort, strPars[0], strPars[1], strPars[2], strPars[3], strPars[4], strPars[5], strPars[6], strPars[7], (int)AMSTYPE.ETC);
                            }
                            break;

                        default:
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 INVALID : " + LstJOB_CMD[iJobNum].PAR1, "PAR1 INVALID : " + LstJOB_CMD[iJobNum].PAR1, String.Empty, true);
                            return true;
                    }

                    return true;

                case "KIS_KEY_DOWNLOAD":

                    InitialKeyData(iPort, false);                   
                    return true;

                case "KIS_KEY_DOWNLOAD_NONZERO":

                    InitialKeyData(iPort, true);
                    return true;


                case "KIS_KEY_DOWNLOAD_MANUAL":

                    string[] strParams = LstJOB_CMD[iJobNum].PAR1.Split(',');

                    if (strParams.Length != 4)
                    {                        
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 INVALID : " + LstJOB_CMD[iJobNum].PAR1, "PAR1 INVALID : " + LstJOB_CMD[iJobNum].PAR1, String.Empty, true);
                        return true;                  
                    }

                    InitialKeyData_Manual(iPort, strParams);
                    return true;

                case "KIS_DATA_VIEW":
                case "AMS_DATA_VIEW":
                    string ResponseData = String.Empty;

                    MessageLogging((int)LOGTYPE.TX, strCmdType + "(" + LstJOB_CMD[iJobNum].PAR1 + ")", iPort);
                    bool bReadExpr = ExprReadKisDownloadData(iPort, LstJOB_CMD[iJobNum].PAR1, ref ResponseData);

                    if (bReadExpr)
                    {                        
                        GateWayMsgProcess((int)STATUS.OK, ResponseData, ResponseData, String.Empty, true); 
                    }
                    else
                    {
                        ResponseData = "NONE";                        
                        GateWayMsgProcess((int)STATUS.NG, ResponseData, ResponseData, String.Empty, true); 
                    }                    
                    return true;

                default:
                    GateWayMsgProcess((int)STATUS.NG, "UNKNOWN COMMAND", "UNKNOWN COMMAND", String.Empty, true); 
                    return false;
            }           
        }

        //CSMES
        private void GetFinalResult(int iPort, out bool bLastRes, out bool bChkErrRes)
        {
            bLastRes = true;
            bChkErrRes = false;

            for (int i = 0; i < LstTST_RES[iPort].Count; i++) //최종결과 검색.
            {
                switch (LstTST_RES[iPort][i].iStatus)
                {
                    case (int)STATUS.OK: break;
                    case (int)STATUS.SKIP: break;

                    case (int)STATUS.NG: bLastRes = false; break;


                    case (int)STATUS.CHECK:
                    case (int)STATUS.ERROR:
                    case (int)STATUS.MESERR:
                    case (int)STATUS.DSUB: bChkErrRes = true; break;


                    default: break; //원래 OK, SKIP 이 아니면 모조리 GMES 에 ng 보고 했으나 이동성 책임 요청으로 NG 만 올리도록 설정함.
                                    //default: bLastRes = false; break;
                }

                if (!bLastRes) break;
            }
        }

        private void ExprWriteKisDownloadData(int iPort, string[] strSubject, string[] strData)
        {
            lock (lockObject)
            {
                try
                {
                    for (int i = 0; i < strSubject.Length; i++)
                    {
                        bool bExpr = DKExpr[iPort].ExcuteSave(strSubject[i], strData[i]);
                    }
                }

                catch { }

            }

        }

        private bool ExprReadKisDownloadData(int iPort, string strSubject, ref string strData)
        {
            bool bExpr = false;
            lock (lockObject)
            {
                try
                {
                    bExpr = DKExpr[iPort].ExcuteLoad(strSubject, ref strData);

                }

                catch { bExpr = false; }

            }
            return bExpr;
        }

        public void CallAMS(int iPort, string strIp1, string strIp2, string strGen, string strProduct, string strStid)
        {
            MessageLogging((int)LOGTYPE.TX, strIp1 + "," + strIp2 + "," + strGen + "," + strProduct + "," + strStid, iPort);
            try
            {
                if (threadAmsKeyDLL != null && threadAmsKeyDLL.ThreadState == ThreadState.Running)
                {
                    GateWayMsgProcess((int)STATUS.NG, "Currently running. Please wait", "Currently running. Please wait", String.Empty, true);
                    return;
                }
            }
            catch { }

            KillThreadObject(threadAmsKeyDLL);
            threadAmsKeyDLL = new Thread(delegate()
            {
                AmsExeKeyDownLoad(iPort, strIp1, strIp2, strGen, strProduct, strStid);
            });

            threadAmsKeyDLL.Start(); return; 
        }

        public void CallAMS(int iPort, string strName, string strIp1, string strIp2, string strId, string strGen, string strFactory, string strProduct, string strStid, int amsDataType)
        {
            if (amsDataType == (int)AMSTYPE.GEN12)
                MessageLogging((int)LOGTYPE.TX, strName + "," + strIp1 + "," + strIp2 + "," + strId + "," + strGen + "," + strProduct + "," + strStid, iPort);
            else
                MessageLogging((int)LOGTYPE.TX, strName + "," + strIp1 + "," + strIp2 + "," + strId + "," + strGen + "," + strFactory + "," + strProduct + "," + strStid, iPort);
            try
            {
                if (threadAmsKeyDLL != null && threadAmsKeyDLL.ThreadState == ThreadState.Running)
                {
                    GateWayMsgProcess((int)STATUS.NG, "Currently running. Please wait", "Currently running. Please wait", String.Empty, true);
                    return;
                }
            }
            catch { }

            KillThreadObject(threadAmsKeyDLL);
            threadAmsKeyDLL = new Thread(delegate ()
            {
                if (amsDataType == (int)AMSTYPE.GEN12)
                    AmsExeKeyDownLoad_GEN12(iPort, strName, strIp1, strIp2, strId, strGen, strFactory, strProduct, strStid, amsDataType);
                else
                    AmsExeKeyDownLoad(iPort, strName, strIp1, strIp2, strId, strGen, strFactory, strProduct, strStid, amsDataType);
            });

            threadAmsKeyDLL.Start(); return;
        }

        public void InitialKeyData(int iPort, bool bStidOption)
        {
            //이중화 접속 구현
            string[] strKisInputPrimary = new string[4];
            string[] strKisInputSecondary = new string[4];

            bool bTotal = true;
            bool bGetRes = false;
            string strItemNamePrimay = String.Empty;
            string strItemNameSecondary = String.Empty;

            for (int i = 0; i < strKisInputPrimary.Length; i++)
            {
                bGetRes = false;
                if (STEPMANAGER_VALUE.bUseMesOn)
                {
                    switch (i)
                    {
                        case 0: strItemNamePrimay    = "KIS_IP_ADDRESS1";
                                strItemNameSecondary = "KIS_IP_ADDRESS2";
                                break;
                        case 1: strItemNamePrimay    = "KIS_KEY_TYPE1";
                                strItemNameSecondary = "KIS_KEY_TYPE2";
                                break;
                        case 2: strItemNamePrimay    = "KIS_GENTYPE";
                                strItemNameSecondary = "KIS_GENTYPE"; 
                                break;
                        case 3: strItemNamePrimay    = "STID";
                                strItemNameSecondary = "STID"; 
                                break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0: strItemNamePrimay    = "CIS_IP1";
                                strItemNameSecondary = "CIS_IP2";
                                break;
                        case 1: strItemNamePrimay    = "CIS_IP1_KEY_TYPE";
                                strItemNameSecondary = "CIS_IP2_KEY_TYPE";
                                break;
                        case 2: strItemNamePrimay    = "CIS_GEN_TYPE";
                                strItemNameSecondary = "CIS_GEN_TYPE";
                                break;
                        case 3: strItemNamePrimay    = "STID";
                                strItemNameSecondary = "STID";
                                break;
                    }
                }

                if (STEPMANAGER_VALUE.bUseMesOn)
                {
                    bGetRes = DKGMES.GMES_GetInsp(iPort, strItemNamePrimay, ref strKisInputPrimary[i]);
                    MessageLogging((int)LOGTYPE.RX, strItemNamePrimay + "(" + strKisInputPrimary[i] + ")", iPort);
                    bGetRes = DKGMES.GMES_GetInsp(iPort, strItemNameSecondary, ref strKisInputSecondary[i]);
                    MessageLogging((int)LOGTYPE.RX, strItemNamePrimay + "(" + strKisInputSecondary[i] + ")", iPort);
                }
                else
                {
                    bGetRes = ExprReadKisDownloadData(iPort, strItemNamePrimay, ref strKisInputPrimary[i]);
                    MessageLogging((int)LOGTYPE.RX, strItemNamePrimay + "(" + strKisInputPrimary[i] + ")", iPort);
                    bGetRes = ExprReadKisDownloadData(iPort, strItemNameSecondary, ref strKisInputSecondary[i]);
                    MessageLogging((int)LOGTYPE.RX, strItemNameSecondary + "(" + strKisInputSecondary[i] + ")", iPort);
                }         

                if (i == 1)
                {
                    try
                    {
                        if (strKisInputPrimary[i] != null && strKisInputPrimary[i].Length > 0)
                        {
                            int iKey = int.Parse(strKisInputPrimary[i]);
                        }
                        else { bGetRes = false; }

                        if (strKisInputSecondary[i] != null && strKisInputSecondary[i].Length > 0)
                        {
                            int iKey = int.Parse(strKisInputSecondary[i]);
                        }
                        else { bGetRes = false; }
                    }
                    catch (Exception ex)
                    {
                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                        STEPMANAGER_VALUE.DebugView(strExMsg); 
                        bGetRes = false;
                    }
                }

                if (!bGetRes || strKisInputPrimary[i] == null || strKisInputPrimary[i].Length < 1)
                {
                    MessageLogging((int)LOGTYPE.TX, "KIS_KEY_DOWNLOAD(ITEM ERROR) - " + strItemNamePrimay, iPort);
                    bTotal = false;
                    break;
                }

                if (!bGetRes || strKisInputSecondary[i] == null || strKisInputSecondary[i].Length < 1)
                {
                    MessageLogging((int)LOGTYPE.TX, "KIS_KEY_DOWNLOAD(ITEM ERROR) - " + strItemNameSecondary, iPort);
                    bTotal = false;
                    break;
                }
            }

            if (!bTotal)
            {
                GateWayMsgProcess((int)STATUS.NG, "PARAMETERS ERROR", "PARAMETERS ERROR", String.Empty, true); 
                return;
            }


            if (STEPMANAGER_VALUE.bInteractiveMode)
            {
                GateWayMsgProcess((int)STATUS.NG, "INTERACTIVE MODE", "INTERACTIVE MODE", String.Empty, true);
                return;
            }

            if (bStidOption)
            {   //GMES에서 주는 Stid 값 맨 앞에 0 이 있으면 kis 에서 인증서 다운이 안되서 0을 제거 할건지 말껀지 option 에 따라 처리해준다.
                int iNonZeroStid = 0;
                string strTempStid = strKisInputPrimary[3];

                if (int.TryParse(strTempStid, out iNonZeroStid))
                {
                    strKisInputPrimary[3] = iNonZeroStid.ToString();
                    MessageLogging((int)LOGTYPE.RX, "Change NonZeroSTID(" + strKisInputPrimary[3] + ")", iPort);
                }
               
            }

            VBFunction_in vbfInput1 = new VBFunction_in();  //DK_KIS에서 쓰일 구조체 primary
            VBFunction_in vbfInput2 = new VBFunction_in();  //DK_KIS에서 쓰일 구조체 secondary

            vbfInput1.remoteHostName    = strKisInputPrimary[0]; //"10.224.0.133";
            vbfInput1.remotePort        = 8504;
            vbfInput1.firstBindPort     = 8000;
            vbfInput1.lastBindPort      = 8100;
            vbfInput1.acceptTimeout     = 10;
            vbfInput1.connectTimeout    = 10;
            vbfInput1.stid              = strKisInputPrimary[3]; // "121801575";
            vbfInput1.nKeyTypes         = int.Parse(strKisInputPrimary[1]);//11;
            vbfInput1.gentype           = strKisInputPrimary[2];// "10";

            vbfInput2.remoteHostName    = strKisInputSecondary[0]; //"10.224.0.133";
            vbfInput2.remotePort        = 8504;
            vbfInput2.firstBindPort     = 8000;
            vbfInput2.lastBindPort      = 8100;
            vbfInput2.acceptTimeout     = 10;
            vbfInput2.connectTimeout    = 10;
            vbfInput2.stid              = strKisInputSecondary[3]; // "121801575";
            vbfInput2.nKeyTypes         = int.Parse(strKisInputSecondary[1]);//11;
            vbfInput2.gentype           = strKisInputSecondary[2];// "10";

            /*test 정보
            VBFunction_in vbfInput = new VBFunction_in();  //DK_KIS에서 쓰일 구조체

            vbfInput.remoteHostName = "10.224.0.133";
            vbfInput.remotePort     = 8504;
            vbfInput.firstBindPort  = 8000;
            vbfInput.lastBindPort   = 8100;
            vbfInput.acceptTimeout  = 10;
            vbfInput.connectTimeout = 10;
            vbfInput.stid           = "117605004";
            vbfInput.nKeyTypes      = 11;
            vbfInput.gentype        = "10";
            */
            try
            {
                if (threadKisKeyDLL != null && threadKisKeyDLL.ThreadState != ThreadState.Stopped)
                {
                    KillThreadObject(threadKisKeyDLL);
                    threadKisKeyDLL = null;
                }
            }
            catch { }
            
            string[] strSubject = new string[10];
            string[] strData    = new string[10];

            //EXPR 에 KIS 서버에서 내려받은 데이터들을 저장하기 위해 초기화. ㅋㅋㅋ 움하하하하하항하하하핳ㅎㅎㅎ
            strSubject[0] = "KIS_error_code";     strData[0]= "-1";
            strSubject[1] = "KIS_error_message";  strData[1]= ""; 
            strSubject[2] = "KIS_stid";           strData[2]= ""; 
            strSubject[3] = "KIS_rCert";          strData[3]= ""; 
            strSubject[4] = "KIS_ccCert";         strData[4]= ""; 
            strSubject[5] = "KIS_vCert";          strData[5]= ""; 
            strSubject[6] = "KIS_vPri";           strData[6]= ""; 
            strSubject[7] = "KIS_vPre";           strData[7]= ""; 
            strSubject[8] = "KIS_vAuth";          strData[8]= ""; 
            strSubject[9] = "KIS_vHash";          strData[9]= "";
 
            ExprWriteKisDownloadData(iPort, strSubject, strData);    
            
            threadKisKeyDLL = new Thread(delegate()
            {
                KisDllKeyDownLoad(iPort, vbfInput1, vbfInput2);
            });

            threadKisKeyDLL.Start(); return;          

        }

        public void InitialKeyData_Manual(int iPort, string[] strParam)  //이동성 책임 요청
        {      
            for (int i = 0; i < strParam.Length; i++)
            {
                if (String.IsNullOrEmpty(strParam[i]))
                {
                    GateWayMsgProcess((int)STATUS.NG, "PARAMETERS NULL", "PARAMETERS NULL", String.Empty, true);
                    return;
                }
            }

            VBFunction_in vbfInput1 = new VBFunction_in();  //DK_KIS에서 쓰일 구조체 primary
            VBFunction_in vbfInput2 = new VBFunction_in();  //DK_KIS에서 쓰일 구조체 secondary

            try
            {
                vbfInput1.remoteHostName = strParam[0]; //"10.224.0.133";
                vbfInput1.remotePort = 8504;
                vbfInput1.firstBindPort = 8000;
                vbfInput1.lastBindPort = 8100;
                vbfInput1.acceptTimeout = 10;
                vbfInput1.connectTimeout = 10;
                vbfInput1.stid = strParam[1]; // "121801575";
                vbfInput1.nKeyTypes = int.Parse(strParam[2]);//11;
                vbfInput1.gentype = strParam[3];// "10";

                vbfInput2.remoteHostName = strParam[0]; //"10.224.0.133";
                vbfInput2.remotePort = 8504;
                vbfInput2.firstBindPort = 8000;
                vbfInput2.lastBindPort = 8100;
                vbfInput2.acceptTimeout = 10;
                vbfInput2.connectTimeout = 10;
                vbfInput2.stid = strParam[1]; // "121801575";
                vbfInput2.nKeyTypes = int.Parse(strParam[2]);//11;
                vbfInput2.gentype = strParam[3];// "10";
            }
            catch 
            {
                GateWayMsgProcess((int)STATUS.NG, "PARAMETERS ERROR", "PARAMETERS ERROR", String.Empty, true);
                return;
            }


            

            /*test 정보
            VBFunction_in vbfInput = new VBFunction_in();  //DK_KIS에서 쓰일 구조체

            vbfInput.remoteHostName = "10.224.0.133";
            vbfInput.remotePort     = 8504;
            vbfInput.firstBindPort  = 8000;
            vbfInput.lastBindPort   = 8100;
            vbfInput.acceptTimeout  = 10;
            vbfInput.connectTimeout = 10;
            vbfInput.stid           = "117605004";
            vbfInput.nKeyTypes      = 11;
            vbfInput.gentype        = "10";
            */
            try
            {
                if (threadKisKeyDLL != null && threadKisKeyDLL.ThreadState != ThreadState.Stopped)
                {
                    KillThreadObject(threadKisKeyDLL);
                    threadKisKeyDLL = null;
                }
            }
            catch { }

            string[] strSubject     = new string[10];
            string[] strData        = new string[10];

            //EXPR 에 KIS 서버에서 내려받은 데이터들을 저장하기 위해 초기화. ㅋㅋㅋ 움하하하하하항하하하핳ㅎㅎㅎ
            strSubject[0] = "KIS_error_code";       strData[0] = "-1";
            strSubject[1] = "KIS_error_message";    strData[1] = "";
            strSubject[2] = "KIS_stid";             strData[2] = "";
            strSubject[3] = "KIS_rCert";            strData[3] = "";
            strSubject[4] = "KIS_ccCert";           strData[4] = "";
            strSubject[5] = "KIS_vCert";            strData[5] = "";
            strSubject[6] = "KIS_vPri";             strData[6] = "";
            strSubject[7] = "KIS_vPre";             strData[7] = "";
            strSubject[8] = "KIS_vAuth";            strData[8] = "";
            strSubject[9] = "KIS_vHash";            strData[9] = "";

            ExprWriteKisDownloadData(iPort, strSubject, strData);

            threadKisKeyDLL = new Thread(delegate()
            {
                KisDllKeyDownLoad(iPort, vbfInput1, vbfInput2);
            });

            threadKisKeyDLL.Start(); return;

        }

        private bool ScannerCommand(int iPort, int iJobNum, string strCmdType, double dTimeout, double dDelayTime, int iSendRecvOption)
        {
            string strSendpac = String.Empty;

            if (!OpenSCAN)
            {
                GateWayMsgProcess((int)STATUS.NG, "SCANNER COMPORT DISCONNECTED", "SCANNER COMPORT DISCONNECTED", String.Empty, true);
                DKLoggerMR.WriteCommLog("[SCANNER]ERROR - COMPORT DISCONNECTED", strCmdType, false);
                return false;
            }

            AnalyizePack anlPack = new AnalyizePack();

            if (LstJOB_CMD[iJobNum].COMPARE.Equals("RESULTCODE"))
                anlPack.bResultCodeOption = true;
            else
                anlPack.bResultCodeOption = false;

            switch (strCmdType)
            {
               
                case "TRIGGER_ON":                    
                    strSendpac = "02";          
                    DKACTOR.SendRecvCmd((int)COMSERIAL.SCANNER, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.SCANNER, LstJOB_CMD[iJobNum].CMD, "", anlPack);
                    return true;

                case "TRIGGER_OFF": 
                    strSendpac = "03";
                    DKACTOR.SendRecvCmd((int)COMSERIAL.SCANNER, strSendpac, dTimeout, (int)MODE.SEND, dDelayTime, (int)RS232.SCANNER, LstJOB_CMD[iJobNum].CMD, "", anlPack);
                    return true;

                case "HONEYWELL_ON":                    
                    strSendpac = "16 54 0D";
                    DKACTOR.SendRecvCmd((int)COMSERIAL.SCANNER, strSendpac, dTimeout, iSendRecvOption, dDelayTime, (int)RS232.SCANNER, LstJOB_CMD[iJobNum].CMD, "", anlPack);
                    return true;

                case "HONEYWELL_OFF":
                    strSendpac = "16 55 0D";
                    DKACTOR.SendRecvCmd((int)COMSERIAL.SCANNER, strSendpac, dTimeout, (int)MODE.SEND, dDelayTime, (int)RS232.SCANNER, LstJOB_CMD[iJobNum].CMD, "", anlPack);
                    return true;

                default:
                    GateWayMsgProcess((int)STATUS.NG, "UNKNOWN COMMAND", "UNKNOWN COMMAND", String.Empty, true);
                    DKLoggerMR.WriteCommLog("[UNKNOWN COMMAND]ERROR", strCmdType, false);
                    return false;
            }
        }

        //절차진행하는 사이클엔진에서 이미 실행된 TCP OOB TEST RESULT 결과값에서 데이터 뽑아오는 함수 (TCP 용)
        private bool GetOOBresult(string  strIndex)
        {
            MessageLogging((int)LOGTYPE.TX, "OOB_SELF_TEST_CHECK(" + strIndex + ")", (int)DEFINES.SET1);

            if (String.IsNullOrEmpty(strIndex))
            {
                GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true); 
            }
            else
            {
                string strOOBdata = String.Empty;

                bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteLoad("OOB_TEST_RESULT", ref strOOBdata);
                if (!bExpr)
                {   //OOB_TEST_RESULT 명령라인에서 EXPR 에 저장하지 않으면 여길 탄다. 
                    //즉 해당명령을 실행을 안했거나 절차서에 EXPR 에 OOB_TEST_RESULT 라는 이름이 빠진거다.
                    GateWayMsgProcess((int)STATUS.CHECK, "NOT FOUND OOB DATA", "NOT FOUND OOB DATA", strIndex, true); 
                }
                else
                {
                    int i = 1;
                    try
                    {
                        i = int.Parse(strIndex);                        
                    }
                    catch 
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR : " + strIndex, "PAR1 ERROR : " + strIndex, String.Empty, true); 
                        return true;
                    }

                    i--;
                    if (i < 0)
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR : START INDEX 1" + strIndex, "PAR1 ERROR : START INDEX 1" + strIndex, String.Empty, true); 
                        return true;
                    }

                    if (i >= strOOBdata.Length)
                    {
                        string strLogMsg = "OOB DATA INDEX(" + strOOBdata.Length + ") OVER : PAR1(" + (i + 1) + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true); 
                        return true;
                    }

                    string strResultData = String.Empty;
                    try
                    {
                        strResultData = strOOBdata.Substring(i, 1);                        
                    }
                    catch
                    {
                        strResultData = "PARSE ERROR - OOB DATA(" + strOOBdata + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strResultData, strResultData, String.Empty, true); 
                        return true;
                    }

                    GateWayMsgProcess((int)STATUS.OK, strResultData, strResultData, String.Empty, true); 
                    return true;

                }
            }
            return true;
        }

        //절차진행하는 사이클엔진에서 이미 실행된 TCP OOB TEST RESULT 결과값에서 데이터 뽑아오는 함수 (ATT 용)
        private bool GetATTOOBresult(string strIndex)
        {
            MessageLogging((int)LOGTYPE.TX, "OOB_SELF_TEST_CHECK(" + strIndex + ")", (int)DEFINES.SET1);

            if (String.IsNullOrEmpty(strIndex))
            {
                GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true); 
            }
            else
            {
                string strOOBdata = String.Empty;

                bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteLoad("OOB_TEST_RESULT", ref strOOBdata);
                if (!bExpr)
                {   //OOB_TEST_RESULT 명령라인에서 EXPR 에 저장하지 않으면 여길 탄다.
                    //즉 해당명령을 실행을 안했거나 절차서에 EXPR 에 OOB_TEST_RESULT 라는 이름이 빠진거다.
                    GateWayMsgProcess((int)STATUS.CHECK, "NOT FOUND OOB DATA", "NOT FOUND OOB DATA", String.Empty, true); 
                }
                else
                {
                    int i = 1;
                    try
                    {
                        i = int.Parse(strIndex);
                    }
                    catch
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR : " + strIndex, "PAR1 ERROR : " + strIndex, String.Empty, true); 
                        return true;
                    }
                    
                    if (i <= 0)
                    {
                        string strLogMsg = "PAR1 ERROR : START INDEX 1" + strIndex;
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true); 
                        return true;
                    }

                    i--;
                    if (i * 2 >= strOOBdata.Length)
                    {
                        string strLogMsg =  "OOB DATA INDEX(" + (strOOBdata.Length / 2).ToString() + ") OVER : PAR1(" + (i + 1) + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true); 
                        return true;
                    }

                    string strResultData = String.Empty;
                    try
                    {
                        strResultData = strOOBdata.Substring(i * 2, 2);
                    }
                    catch
                    {
                        strResultData = "PARSE ERROR - OOB DATA(" + strOOBdata + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strResultData, strResultData, String.Empty, true); 
                        return true;
                    }

                    GateWayMsgProcess((int)STATUS.OK, strResultData, strResultData, String.Empty, true); 
                    return true;
                }
            }
            return true;
        }

        //절차진행하는 사이클엔진에서 이미 실행된 OOB DTC ALL 결과값에서 데이터 뽑아오는 함수 (GEN10 OOB 용)
        private bool GetGEN10DTCresult(string strIndex)
        {
            MessageLogging((int)LOGTYPE.TX, "OOB_DTC_CHECK(" + strIndex + ")", (int)DEFINES.SET1);

            if (String.IsNullOrEmpty(strIndex))
            {
                GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
            }
            else
            {
                string strDTCAlldata = String.Empty;

                bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteLoad("OOB_DTC_RESULT", ref strDTCAlldata);
                if (!bExpr)
                {   //OOB_TEST_RESULT 명령라인에서 EXPR 에 저장하지 않으면 여길 탄다.
                    //즉 해당명령을 실행을 안했거나 절차서에 EXPR 에 OOB_TEST_RESULT 라는 이름이 빠진거다.
                    GateWayMsgProcess((int)STATUS.CHECK, "NOT FOUND OOB_DTC DATA", "NOT FOUND OOB_DTC DATA", String.Empty, true);
                }
                else
                {
                    int i = 1;
                    try
                    {
                        i = int.Parse(strIndex);
                    }
                    catch
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR : " + strIndex, "PAR1 ERROR : " + strIndex, String.Empty, true);
                        return true;
                    }

                    if (i <= 0)
                    {
                        string strLogMsg = "PAR1 ERROR : START INDEX 1" + strIndex;
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true);
                        return true;
                    }

                    i--;
                    if (i * 2 >= strDTCAlldata.Length)
                    {
                        string strLogMsg = "OOB_DTC DATA INDEX(" + (strDTCAlldata.Length / 2).ToString() + ") OVER : PAR1(" + (i + 1) + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true);
                        return true;
                    }

                    string strResultData = String.Empty;
                    try
                    {
                        strResultData = strDTCAlldata.Substring(i * 2, 2);
                    }
                    catch
                    {
                        strResultData = "PARSE ERROR - OOB_DTC DATA(" + strDTCAlldata + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strResultData, strResultData, String.Empty, true);
                        return true;
                    }

                    GateWayMsgProcess((int)STATUS.OK, strResultData, strResultData, String.Empty, true);
                    return true;
                }
            }
            return true;
        }

        //절차진행하는 사이클엔진에서 이미 실행된 TCP OOB TEST RESULT 결과값에서 데이터 뽑아오는 함수 (ATT 용)
        private bool GetGEN10OOBresult(string strIndex)
        {
            MessageLogging((int)LOGTYPE.TX, "OOB_SELF_TEST_CHECK(" + strIndex + ")", (int)DEFINES.SET1);

            if (String.IsNullOrEmpty(strIndex))
            {
                GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
            }
            else
            {
                string strOOBdata = String.Empty;

                bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteLoad("OOB_TEST_RESULT", ref strOOBdata);
                if (!bExpr)
                {   //OOB_TEST_RESULT 명령라인에서 EXPR 에 저장하지 않으면 여길 탄다.
                    //즉 해당명령을 실행을 안했거나 절차서에 EXPR 에 OOB_TEST_RESULT 라는 이름이 빠진거다.
                    GateWayMsgProcess((int)STATUS.CHECK, "NOT FOUND OOB DATA", "NOT FOUND OOB DATA", String.Empty, true);
                }
                else
                {
                    int i = 1;
                    try
                    {
                        i = int.Parse(strIndex);
                    }
                    catch
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR : " + strIndex, "PAR1 ERROR : " + strIndex, String.Empty, true);
                        return true;
                    }

                    if (i <= 0)
                    {
                        string strLogMsg = "PAR1 ERROR : START INDEX 1" + strIndex;
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true);
                        return true;
                    }

                    i--;
                    if (i * 2 >= strOOBdata.Length)
                    {
                        string strLogMsg = "OOB DATA INDEX(" + (strOOBdata.Length / 2).ToString() + ") OVER : PAR1(" + (i + 1) + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strLogMsg, strLogMsg, String.Empty, true);
                        return true;
                    }

                    string strResultData = String.Empty;
                    try
                    {
                        strResultData = strOOBdata.Substring(i * 2, 2);
                    }
                    catch
                    {
                        strResultData = "PARSE ERROR - OOB DATA(" + strOOBdata + ")";
                        GateWayMsgProcess((int)STATUS.CHECK, strResultData, strResultData, String.Empty, true);
                        return true;
                    }

                    GateWayMsgProcess((int)STATUS.OK, strResultData, strResultData, String.Empty, true);
                    return true;
                }
            }
            return true;
        }

        //절차진행하는 사이클엔진에서 Sim정보 커맨드인지 확인
        private bool CheckCommaParameter(int iJobNumber)
        {   //파라미터에 콤마는 EXPR 이나 GMES 값이 들어있을경우 문자열을 콤마를 삭제하고 합치는 기능이다 (suffix & name 처럼)
            //EXPR, GMES 가 없으면 그대로 콤마가 사용된다.
            //하지만 EXPR 도 있고 일반 값도 유지하고 콤마도 사용되는 파라미터로 그대로 전달해야하는 명령이면 하드코딩해서 사용하는 영역이다. 
            bool bReturn = true;
            string strSendpac = String.Empty;
            string strCmdName = LstJOB_CMD[iJobNumber].CMD;
            string strParam = LstJOB_CMD[iJobNumber].PAR1;
            if (!strParam.Contains(DEFINEEXPR) && !strParam.Contains(DEFINEGMES) && !strParam.Contains(DEFINEDOCU)) return true;   //EXPR, GMES 둘다 없으면 그대로 콤마가 사용된다.
            if (strParam.Contains(",") && (strParam.Contains(DEFINEEXPR) || strParam.Contains(DEFINEGMES) || strParam.Contains(DEFINEDOCU)))
            {                
                switch (LstJOB_CMD[iJobNumber].TYPE)
                {                    
                    case "PAGE":
                                 if (strCmdName.Equals("CHANGE_JOB")) bReturn = false; break; //콤마삭제                   
           
                    default:
                        bReturn = true; //콤마사용
                        break;
                }
            }

            return bReturn;
        }

        //절차진행하는 사이클엔진에서 ApnTable 정보 커맨드인지 확인
        private bool CheckApnTableCommand(string strCmdType, string strIndex)
        {
            int iDx = 0;

            switch (strCmdType)
            {
                case "READ_APN_INFO_NAME":
                case "READ_APN_INFO_IPv4":
                case "READ_APN_INFO_IPv6":
                case "READ_APN_INFO_ALWAYSON":
                case "READ_APN_INFO_HRPD":
                case "READ_APN_INFO_ALL":

                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);

                    if(STEPMANAGER_VALUE.GEN10APN_TABLE.Count < 10)
                    {
                        GateWayMsgProcess((int)STATUS.NG, "Not Found APN Info", "Not Found APN Info", "", true);
                        return true;
                    }

                    if (String.IsNullOrEmpty(strIndex))
                    {
                        GateWayMsgProcess((int)STATUS.NG, "Require Index", "Require Index", strIndex, true);
                        return true;
                    }
                    else
                    {
                        try
                        {
                            iDx = int.Parse(strIndex);

                            if (iDx < 0 || iDx >= STEPMANAGER_VALUE.GEN10APN_TABLE.Count)
                            {
                                GateWayMsgProcess((int)STATUS.NG, "RangeOver Index", "RangeOver Index", strIndex, true);
                                return true;
                            }
                        }
                        catch
                        {
                            GateWayMsgProcess((int)STATUS.NG, "Wrong Index", "Wrong Index", strIndex, true);
                            return true;
                        }
                    }
                    break;

                default: return false;

            }

            string[] strApnInfo = new string[(int)APNINDEX.END];
            strApnInfo = STEPMANAGER_VALUE.GEN10APN_TABLE[iDx];
            int iApnIndex = (int)APNINDEX.END;

            switch (strCmdType)
            {
                case "READ_APN_INFO_NAME":      iApnIndex = (int)APNINDEX.szAPNName; break;
                case "READ_APN_INFO_IPv4":      iApnIndex = (int)APNINDEX.bIPv4; break;
                case "READ_APN_INFO_IPv6":      iApnIndex = (int)APNINDEX.bIPv6; break;
                case "READ_APN_INFO_ALWAYSON":  iApnIndex = (int)APNINDEX.bAlwaysOn; break;
                case "READ_APN_INFO_HRPD":      iApnIndex = (int)APNINDEX.bHRPD; break;

                case "READ_APN_INFO_ALL":
                    string strOptionAll = String.Empty; //O,X,N  (N 은 NO DATA)
                    int iiDx = (int)APNINDEX.bIPv4;

                    for (int i = 0; i < 4; i++)
                    {
                        if (!String.IsNullOrEmpty(strApnInfo[iiDx]))
                        {
                            if (strApnInfo[iiDx].Equals("ACTIVE"))
                                strOptionAll += "O";
                            else
                                strOptionAll += "X";
                        }
                        else
                        {
                            strOptionAll += "N";  //NO DATA
                        }

                        switch (iiDx)
                        {
                            case (int)APNINDEX.bIPv4:       iiDx = (int)APNINDEX.bIPv6;     strOptionAll += ",";  break;
                            case (int)APNINDEX.bIPv6:       iiDx = (int)APNINDEX.bAlwaysOn; strOptionAll += ","; break;
                            case (int)APNINDEX.bAlwaysOn:   iiDx = (int)APNINDEX.bHRPD;     strOptionAll += ","; break;
                            case (int)APNINDEX.bHRPD: break;
                        }
                    }

                    GateWayMsgProcess((int)STATUS.OK, strOptionAll, strOptionAll, strIndex, true);
                    return true;
                default: 

                    GateWayMsgProcess((int)STATUS.NG, "Unknown Command Error", "Unknown Command Error", "", true);
                    return true;
            }

            if (!String.IsNullOrEmpty(strApnInfo[iApnIndex]))
            {
                GateWayMsgProcess((int)STATUS.OK, strApnInfo[iApnIndex], strApnInfo[iApnIndex], strIndex, true);
            }
            else
            {
                GateWayMsgProcess((int)STATUS.OK, "NO DATA", "NO DATA", String.Empty, true);
            }

            return true;
        }

        //절차진행하는 사이클엔진에서 Sim정보 커맨드인지 확인
        private bool CheckGen9MDNCommand(int iPort, int iJobNumber, string strCmdType, ref string strSendparEx)
        {
            switch (strCmdType)
            {
                case "CREATE_MDN_DATA":
                                            MessageLogging((int)LOGTYPE.TX, strSendparEx, (int)DEFINES.SET1);
                                            break;
                case "UPDATE_MDN_DATA_PAIRCOUNT":
                case "UPDATE_MDN_DATA_SID":
                case "UPDATE_MDN_DATA_NID":
                case "UPDATE_MDN_DATA_HOME_SID":
                case "UPDATE_MDN_DATA_COUNTRY":
                case "UPDATE_MDN_DATA_MIN":
                case "UPDATE_MDN_DATA_MDN":
                    
                                            MessageLogging((int)LOGTYPE.TX, strSendparEx, (int)DEFINES.SET1);


                                            if (String.IsNullOrEmpty(strSendparEx))
                                            {
                                                RETCOUNT_DIC[iPort] = 0;
                                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                                return true;
                                            }
                                            break;

                default:                    
                    break;

            }
            switch (strCmdType)
            {
                //BASIC
                case "CREATE_MDN_DATA":                    
                    tmpMDN = new _NAMMgmnt();
                    tmpMDN.m_cpMIN = new byte[16];
                    tmpMDN.m_cpMDN = new byte[16];
                    tmpMDN.m_stSIDNIDPairs = new _SIDNIDPairsD[11];
                    tmpMDN.m_iPairsCnt = 0;
                    for (int i = 0; i < tmpMDN.m_stSIDNIDPairs.Length; i++)
                    {
                        tmpMDN.m_stSIDNIDPairs[i].m_sNID = 0;
                        tmpMDN.m_stSIDNIDPairs[i].m_sSID = 0;
                    }
                        
                    RETCOUNT_DIC[iPort] = 0;
                    GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);                    
                    return true;

                
                
                case "UPDATE_MDN_DATA_MDN":
                    try
                    {
                        if (strSendparEx.Length < 10 && strSendparEx.Length > 11) // ATT 소스에는 10byte 인데 실제로 기존 생산 세트한테 읽어보면 11자리로 나와서 일단은 11자리도 받아들여야할것 같다..ㅠㅠ
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "MDN data required 10 or 11 digits. (Ex : 9999999999)", "MDN data required 10 or 11digits. (Ex : 9999999999)", String.Empty, true);
                            return true;
                        }

                        byte[] bMdnValue = Encoding.UTF8.GetBytes(strSendparEx);

                        Array.Copy(bMdnValue, 0, tmpMDN.m_cpMDN , 0, bMdnValue.Length);

                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                        return true;

                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "NOT FOUND CREATED MDN DATA", "NOT FOUND CREATED MDN DATA", String.Empty, true);
                        return true;
                    }

                case "UPDATE_MDN_DATA_MIN":
                    try
                    {
                        if (strSendparEx.Length != 10)
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "MIN data required 10 digits. (Ex : 9999999999)", "MIN data required 10 digits. (Ex : 9999999999)", String.Empty, true);
                            return true;
                        }

                        byte[] bMinValue = Encoding.UTF8.GetBytes(strSendparEx);

                        Array.Copy(bMinValue, 0, tmpMDN.m_cpMIN, 5, bMinValue.Length);

                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                        return true;

                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "NOT FOUND CREATED MDN DATA", "NOT FOUND CREATED MDN DATA", String.Empty, true);
                        return true;
                    }

                case "UPDATE_MDN_DATA_COUNTRY":
                    try
                    {
                        if (strSendparEx.Length != 5)
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "Country code required 5 digits. (Ex : 03000)", "Country code required 5 digits. (Ex : 03000)", String.Empty, true);
                            return true;
                        }

                        byte[] bCountryCode = Encoding.UTF8.GetBytes(strSendparEx);

                        Array.Copy(bCountryCode, 0, tmpMDN.m_cpMIN, 0, bCountryCode.Length);

                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                        return true;

                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "NOT FOUND CREATED MDN DATA", "NOT FOUND CREATED MDN DATA", String.Empty, true);
                        return true;
                    }

                case "UPDATE_MDN_DATA_HOME_SID":
                    try
                    {
                        ushort usHomeSID = 0;
                        if (!ushort.TryParse(strSendparEx, out usHomeSID))
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "HomeSID Error : " + LstJOB_CMD[iJobNumber].PAR1, "HomeSID Error : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        if (usHomeSID > 32767)
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "HomeSID Range Error (32767)", "HomeSID Range Error (32767)", String.Empty, true);
                            return true;
                        }

                        tmpMDN.m_sHomeSID = usHomeSID;

                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                        return true;

                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "NOT FOUND CREATED MDN DATA", "NOT FOUND CREATED MDN DATA", String.Empty, true);
                        return true;
                    }

                case "UPDATE_MDN_DATA_PAIRCOUNT":
                    try
                    {
                        int iPairCount = 0;
                        if (!int.TryParse(strSendparEx, out iPairCount))
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PairCount Error : " + LstJOB_CMD[iJobNumber].PAR1, "PairCount Error : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        if (iPairCount < 0 || iPairCount >= tmpMDN.m_stSIDNIDPairs.Length)
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PairCount Range Error : " + LstJOB_CMD[iJobNumber].PAR1, "PairCount Range Error : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        tmpMDN.m_iPairsCnt = iPairCount;

                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                        return true;

                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "NOT FOUND CREATED MDN DATA", "NOT FOUND CREATED MDN DATA", String.Empty, true);
                        return true;
                    }

                case "UPDATE_MDN_DATA_SID":
                case "UPDATE_MDN_DATA_NID":

                    try
                    {                        

                        string[] strParameters = strSendparEx.Split(',');

                        if (strParameters.Length != 2)
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        int iDx = 0;
                        if (!int.TryParse(strParameters[0], out iDx))
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "Index Error : " + LstJOB_CMD[iJobNumber].PAR1, "Index Error : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        if (iDx < 0 || iDx >= tmpMDN.m_stSIDNIDPairs.Length)
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "Index Range Error : " + LstJOB_CMD[iJobNumber].PAR1, "Index Range Error : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        ushort usData = 0;

                        if (!ushort.TryParse(strParameters[1], out usData))
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "Data Range Error : " + LstJOB_CMD[iJobNumber].PAR1, "Data Range Error : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        switch (strCmdType)
                        {
                            case "UPDATE_MDN_DATA_SID":
                                if (usData > 32767)
                                {
                                    RETCOUNT_DIC[iPort] = 0;
                                    GateWayMsgProcess((int)STATUS.NG, "SID Range Over(32767)", "SID Range Over(32767)", String.Empty, true);
                                    return true;
                                }

                                tmpMDN.m_stSIDNIDPairs[iDx].m_sSID = usData; break;
                            case "UPDATE_MDN_DATA_NID":

                                if (usData > 65535)
                                {
                                    RETCOUNT_DIC[iPort] = 0;
                                    GateWayMsgProcess((int)STATUS.NG, "NID Range Over(65535)", "NID Range Over(65535)", String.Empty, true);
                                    return true;
                                }

                                tmpMDN.m_stSIDNIDPairs[iDx].m_sNID = usData; break;
                            default:
                                break;
                        }

                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                        return true;
                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "NOT FOUND CREATED MDN DATA", "NOT FOUND CREATED MDN DATA", String.Empty, true);
                        return true;
                    }

                    

                case "WRITE_MDN":                    
                    
                    try
                    {
                        if (!CompileMDNStructure())
                        {
                            RETCOUNT_DIC[iPort] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "Error Reverse MDN Data", "Error Reverse MDN Data", String.Empty, true);
                            return true;
                        }

                        int structSize = Marshal.SizeOf(typeof(_NAMMgmnt));
                        byte[] bReturn = new byte[structSize];
                        
                        IntPtr informations;
                                                
                        informations = Marshal.AllocHGlobal(structSize);


                        Marshal.StructureToPtr(tmpMDN, informations, false); //메모리 복사 
                        Marshal.Copy(informations, bReturn, 0, structSize);
                        Marshal.FreeHGlobal(informations);

                        strSendparEx = BitConverter.ToString(bReturn).Replace("-", "");
                        return false;
                    }
                    catch
                    {
                        RETCOUNT_DIC[iPort] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "Error WRITE_MDN", "Error WRITE_MDN", String.Empty, true);
                        return true;
                    }
                    
                default: 
                    return false;
            }

        }

        private bool CompileMDNStructure()
        {
            tmpMDN.m_bActionCode = 0x01;

            //바이트오더 역정렬

            //1.m_sHomeSID

            try
            {
                byte[] bArrayHomeSID = new byte[2];
                bArrayHomeSID = BitConverter.GetBytes(tmpMDN.m_sHomeSID);
                Array.Reverse(bArrayHomeSID);
                tmpMDN.m_sHomeSID = BitConverter.ToUInt16(bArrayHomeSID, 0);
            }
            catch
            {   
                return false;
            }

            //2.m_iPairsCnt
            try
            {
                byte[] bArrayPairsCnt = new byte[4];
                bArrayPairsCnt = BitConverter.GetBytes(tmpMDN.m_iPairsCnt);
                Array.Reverse(bArrayPairsCnt);
                tmpMDN.m_iPairsCnt = BitConverter.ToInt32(bArrayPairsCnt, 0);
            }
            catch
            {
                return false;
            }

            //3. m_sSIDs, m_sNIDs

            try
            {
                byte[] bArraySID = new byte[2];
                byte[] bArrayNID = new byte[2];

                for (int i = 0; i < tmpMDN.m_stSIDNIDPairs.Length; i++)
                {
                    bArraySID = new byte[2];
                    bArrayNID = new byte[2];

                    bArraySID = BitConverter.GetBytes(tmpMDN.m_stSIDNIDPairs[i].m_sSID);
                    bArrayNID = BitConverter.GetBytes(tmpMDN.m_stSIDNIDPairs[i].m_sNID);

                    Array.Reverse(bArraySID);
                    Array.Reverse(bArrayNID);

                    tmpMDN.m_stSIDNIDPairs[i].m_sSID = BitConverter.ToUInt16(bArraySID, 0);
                    tmpMDN.m_stSIDNIDPairs[i].m_sNID = BitConverter.ToUInt16(bArrayNID, 0);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        //절차진행하는 사이클엔진에서 Sim정보 커맨드인지 확인
        private bool CheckSimInfoCommand(string strCmdType)
        {
            bool bCheck = false;

            int iDx = (int)SimInfoIndex.END;
            
            switch (strCmdType)
            {
                //BASIC
                case "READ_SIM_PROFILE_VERSION":  iDx = (int)SimInfoIndex.eSimVer_PROFILE;      bCheck = true; break;
                case "READ_SIM_NSPIF_VERSION":    iDx = (int)SimInfoIndex.eSimVer_NSPIF;        bCheck = true; break;
                case "READ_SIM_VVN":              iDx = (int)SimInfoIndex.eSimVer_VVN;          bCheck = true; break;
                case "READ_SIM_QUALCOMM_VERSION": iDx = (int)SimInfoIndex.QualcommChipRev;      bCheck = true; break;      
                default: bCheck = false; break;
            }

            if (bCheck)
            {
                MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);

                if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.OOBSimInfo[iDx]))
                {
                    GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.OOBSimInfo[iDx], STEPMANAGER_VALUE.OOBSimInfo[iDx], String.Empty, true);
                }
                else
                {
                    GateWayMsgProcess((int)STATUS.NG, "NO DATA", "NO DATA", String.Empty, true);
                }
                
            }

            return bCheck;
        }

        //절차진행하는 사이클엔진에서 Sim정보 커맨드인지 확인
        private bool CheckServiceInfoCommandA(string strCmdType, string strParam)
        {
            bool bCheck = false;

            int iDx = (int)ServiceIndexA.END;

            switch (strCmdType)
            {
                //BASIC
                case "READ_SERVICE_STATUS":     iDx = (int)ServiceIndexA.cServiceStatus;   bCheck = true;
                                                break;

                case "READ_SERVICE_INFORMATION":

                    if (String.IsNullOrEmpty(strParam))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                        return true;
                    }

                    try
                    {
                        iDx = int.Parse(strParam);

                        if (iDx < (int)ServiceIndexA.szMEID || iDx >= (int)ServiceIndexA.END)
                        {
                            GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                            return true;
                        }

                        bCheck = true;
                    }
                    catch 
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                        return true;
                    }
                    break;

                default: return false;
            }

            if (bCheck)
            {

                MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME + " - PAR(" + iDx.ToString()+ ")", (int)DEFINES.SET1);

                if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.OOBServiceInfoA[iDx]))
                {
                    DK_DECISION tmpDec = new DK_DECISION();
                    string strLastMsg = String.Empty;
                    tmpDec.CheckNoneAscii(STEPMANAGER_VALUE.OOBServiceInfoA[iDx], ref strLastMsg);

                    GateWayMsgProcess((int)STATUS.OK, strLastMsg, strLastMsg, String.Empty, true);
                }
                else
                {
                    GateWayMsgProcess((int)STATUS.NG, "NO DATA", "NO DATA", String.Empty, true);
                }

            }

            return bCheck;
        }

        //절차진행하는 사이클엔진에서 Sim정보 커맨드인지 확인
        private bool CheckServiceInfoCommandB(string strCmdType, string strParam)
        {
            bool bCheck = false;

            int iDx = (int)ServiceIndexB.END;

            switch (strCmdType)
            {
                //BASIC
                case "READ_SERVICE_STATUS": iDx = (int)ServiceIndexB.cServiceStatus; bCheck = true;
                    break;

                case "READ_SERVICE_INFORMATION":

                    if (String.IsNullOrEmpty(strParam))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                        return true;
                    }

                    try
                    {
                        iDx = int.Parse(strParam);

                        if (iDx < (int)ServiceIndexB.szMEID || iDx >= (int)ServiceIndexB.END)
                        {
                            GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                            return true;
                        }

                        bCheck = true;
                    }
                    catch
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                        return true;
                    }
                    break;

                default: return false;
            }

            if (bCheck)
            {

                MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME + " - PAR(" + iDx.ToString() + ")", (int)DEFINES.SET1);

                if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.OOBServiceInfoB[iDx]))
                {
                    DK_DECISION tmpDec = new DK_DECISION();
                    string strLastMsg = String.Empty;
                    tmpDec.CheckNoneAscii(STEPMANAGER_VALUE.OOBServiceInfoB[iDx], ref strLastMsg);

                    GateWayMsgProcess((int)STATUS.OK, strLastMsg, strLastMsg, String.Empty, true);
                }
                else
                {
                    GateWayMsgProcess((int)STATUS.NG, "NO DATA", "NO DATA", String.Empty, true);
                }

            }

            return bCheck;
        }

        private bool Gen10ModelCheckCommand(string strCmdType, string strParameter)
        {
            switch (strCmdType)
            {
                case "MFG_CHECK_PARTNUMBER":
                case "MFG_CHECK_STID_RANGE":

                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME + " - " + strParameter, (int)DEFINES.SET1);
                    if (LstTBL_Model.Count < 1)
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "CHECK MODEL FILE:" + strParameter, "CHECK MODEL FILE:" + strParameter, String.Empty, true);
                        return true;
                    }

                    if (String.IsNullOrEmpty(strParameter))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR:" + strParameter, "PAR1 ERROR:" + strParameter, String.Empty, true);
                        return true;
                    }

                    int iSize = 2;
                    switch (strCmdType)
                    {
                        case "MFG_CHECK_PARTNUMBER": iSize = 2; break;
                        case "MFG_CHECK_STID_RANGE": iSize = 3; break;
                        default: iSize = 2; break;

                    }

                    string[] strParam = strParameter.Split(',');

                    if (strParam.Length != iSize)
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERR:" + strParameter, "PAR1 ERR." + strParameter, String.Empty, true);
                        return true;
                    }
                                        
                    TBLMODEL tbl1 = new TBLMODEL(); //BTWIFI 있는거
                    TBLMODEL tbl2 = new TBLMODEL(); //BTWIFI 없는거
                    bool bFind1 = false;
                    bool bFind2 = false;
                    for (int i = 0; i < LstTBL_Model.Count; i++)
                    {
                        if(LstTBL_Model[i].NAME.Equals(strParam[0]) && LstTBL_Model[i].PN.Equals(strParam[1]))
                        {                            
                            if (LstTBL_Model[i].BTWIFI.Equals("Y"))
                            {   
                                bFind1 = true;
                                tbl1 = LstTBL_Model[i];
                            }
                            else
                            {                                
                                bFind2 = true;
                                tbl2 = LstTBL_Model[i];
                            }
                        }
                    }

                    if (!bFind1 && !bFind2)
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "NO DATA:" + strParameter, "NO DATA:" + strParameter, String.Empty, true);
                        return true;
                    }

                    int iRefMin = 0;
                    int iRefMax = 0;
                    int iVal = 0;

                    switch (strCmdType)
                    {
                        case "MFG_CHECK_PARTNUMBER":

                            if (bFind1)
                            {
                                GateWayMsgProcess((int)STATUS.OK, tbl1.PN, tbl1.PN, String.Empty, true);
                                return true;
                            }

                            if (bFind2)
                            {
                                GateWayMsgProcess((int)STATUS.OK, tbl2.PN, tbl2.PN, String.Empty, true);
                                return true;
                            }

                            GateWayMsgProcess((int)STATUS.NG, strParam[1], strParam[1], String.Empty, true);
                            return true;
                            
                        case "MFG_CHECK_STID_RANGE":
                            
                            try
                            {
                                iVal = int.Parse(strParam[2]);
                            }
                            catch
                            {
                            	GateWayMsgProcess((int)STATUS.NG, "STID-ERR:"+strParam[2], "STID-ERR:"+strParam[2], String.Empty, true);
                                return true;
                            }
                            
                            if (bFind1)
                            {
                                try
                                {
                                    iRefMin = int.Parse(tbl1.StidMin);
                                    iRefMax = int.Parse(tbl1.StidMax);
                                }
                                catch
                                {
                                    GateWayMsgProcess((int)STATUS.CHECK, tbl1.StidMin + "~" + tbl1.StidMax, tbl1.StidMin + "~" + tbl1.StidMax, String.Empty, true);
                                    return true;
                                }

                                if (iVal >= iRefMin && iVal <= iRefMax)
                                {
                                    GateWayMsgProcess((int)STATUS.OK, iVal.ToString(), iVal.ToString(), String.Empty, true);
                                    return true;
                                }
                               
                            }
                            
                            if (bFind2)
                            {                              
                                try
                                {
                                    iRefMin = int.Parse(tbl2.StidMin);
                                    iRefMax = int.Parse(tbl2.StidMax);
                                }
                                catch
                                {
                                    GateWayMsgProcess((int)STATUS.CHECK, tbl2.StidMin + "~" + tbl2.StidMax, tbl2.StidMin + "~" + tbl2.StidMax, String.Empty, true);
                                    return true;
                                }

                                if (iVal >= iRefMin && iVal <= iRefMax)
                                    GateWayMsgProcess((int)STATUS.OK, iVal.ToString(), iVal.ToString(), String.Empty, true);
                                else
                                    GateWayMsgProcess((int)STATUS.NG, "RANGE OVER(" + iVal.ToString() + ")" + tbl1.StidMin + "~" + tbl1.StidMax,
                                                         "RANGE OVER(" + iVal.ToString() + ")" + tbl2.StidMin + "~" + tbl2.StidMax, String.Empty, true);                                
                                return true;
                            }

                            if (bFind1)
                            {
                                GateWayMsgProcess((int)STATUS.NG, "RANGE OVER(" + iVal.ToString() + ")" + tbl1.StidMin + "~" + tbl1.StidMax,
                                                         "RANGE OVER(" + iVal.ToString() + ")" + tbl1.StidMin + "~" + tbl1.StidMax, String.Empty, true);
                                return true;
                            }

                            GateWayMsgProcess((int)STATUS.NG, "RANGE OVER : " + strParam[2], "RANGE OVER : " + strParam[2], String.Empty, true);
                            return true;

                        default:
                            GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND", "UNKNOWN COMMAND", String.Empty, true);
                            return true;

                    }                    
                
                default: return false;

            }

        }

        //절차진행하는 사이클엔진에서 GEN11 GPS 명령 수행하는 루틴
        private bool Gen11CcmGpsCommand(string strCmdType)
        {
            switch (strCmdType)
            {
                case "GPS_WAIT":
                case "GNSS_WAIT":
                case "GPS_TTFF":
                case "GNSS_TTFF":
                case "GPS_SVCOUNT":
                case "GNSS_SVCOUNT":
                case "GPS_CN0":
                case "GNSS_CN0":       
                    break;
                default: return false;
            }

            string strDtime = LstJOB_CMD[iNowJobNumber].DELAY;
            double dTime = 1.0;
            
            if ((strDtime.Length < 1) || (strDtime.Equals("0")))
            {
                dTime = 1.0;
            }
            else
            {
                try
                {
                    dTime = double.Parse(strDtime);
                    if (dTime <= 0.1) dTime = 0.1;
                }
                catch { dTime = 0.1; }

                DelayChecker(dTime, false);
            }

            switch (strCmdType)
            {
                case "GPS_START":
                case "GPS_COLD_START":
                case "GPS_HOT_START":
                case "GPS_WARM_START":
                    STEPMANAGER_VALUE.bGen11CCMGpsInfo = true;
                    STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Restart();
                    STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Restart();
                    break;
                case "GPS_STOP":
                    STEPMANAGER_VALUE.bGen11CCMGpsInfo = false;
                    STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Stop();
                    STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Stop();
                    break;   
            }

            switch (strCmdType)
            {                
                case "GPS_WAIT":
                case "GNSS_WAIT":
                case "GPS_TTFF":
                case "GNSS_TTFF":
                case "GPS_SVCOUNT":
                case "GNSS_SVCOUNT":
                case "GPS_CN0":
                case "GNSS_CN0": 
                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                        if (!STEPMANAGER_VALUE.bGen11CCMGpsInfo)
                        {
                            GateWayMsgProcess((int)STATUS.NG, "GPS NOT STARTED", "GPS NOT STARTED", String.Empty, true);
                            return true;
                        }
                        break;
                default: return false;
            }

            switch(strCmdType)
            {
                case "GPS_WAIT":                    
                    if (STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed)
                        GateWayMsgProcess((int)STATUS.OK, "POSITION FIX(GPS)", "POSITION FIX(GPS)", String.Empty, true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, "POSITION NO-FIX(GPS)", "POSITION NO-FIX(GPS)", String.Empty, true);                    
                    return true;

                case "GNSS_WAIT":                    
                    if (STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed)
                        GateWayMsgProcess((int)STATUS.OK, "POSITION FIX(GNSS)", "POSITION FIX(GNSS)", String.Empty, true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, "POSITION NO-FIX(GNSS)", "POSITION NO-FIX(GNSS)", String.Empty, true);                        
                    return true;

                case "GPS_TTFF":                    
                    if (STEPMANAGER_VALUE.stGEN11CCMGpsInfo.bPositionFixed)
                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);                        
                    else
                        GateWayMsgProcess((int)STATUS.NG, STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen11CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);                        
                    return true;

                case "GNSS_TTFF":                    
                    if (STEPMANAGER_VALUE.stGEN11CCMGnssInfo.bPositionFixed)
                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen11CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);
                    return true;

                case "GPS_SVCOUNT":
                    GateWayMsgProcess((int)STATUS.OK, ((int)STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SvCounts).ToString(), ((int)STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SvCounts).ToString(), String.Empty, true);
                    return true;

                case "GNSS_SVCOUNT":                    
                    GateWayMsgProcess((int)STATUS.OK, ((int)STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SvCounts).ToString(), ((int)STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SvCounts).ToString(), String.Empty, true);
                    return true;

                case "GPS_CN0":
                    double[] dValueGpsCn0 = new double[STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo.Length];
                    for (int i = 0; i < dValueGpsCn0.Length; i++)
                        dValueGpsCn0[i] = STEPMANAGER_VALUE.stGEN11CCMGpsInfo.SVInfo[i].dCn0;

                    string strGpsVal = String.Empty;

                    try { strGpsVal = dValueGpsCn0.Max().ToString(); }
                    catch { strGpsVal = "0"; }

                    GateWayMsgProcess((int)STATUS.OK, strGpsVal, strGpsVal, String.Empty, true);
                    return true;

                case "GNSS_CN0":
                    double[] dValueGnssCn0 = new double[STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo.Length];
                    for (int i = 0; i < dValueGnssCn0.Length; i++)
                        dValueGnssCn0[i] = STEPMANAGER_VALUE.stGEN11CCMGnssInfo.SVInfo[i].dCn0;

                    string strGnssVal = String.Empty;
                    try { strGnssVal = dValueGnssCn0.Max().ToString(); }
                    catch { strGnssVal = "0"; }

                    GateWayMsgProcess((int)STATUS.OK, strGnssVal, strGnssVal, String.Empty, true);
                    return true;

                default: return false;      

            }
            
        }

        //절차진행하는 사이클엔진에서 GEN12 GPS 명령 수행하는 루틴
        //private bool Gen12CcmGpsCommand(string strCmdType)
        //{
        //    switch (strCmdType)
        //    {
        //        case "GPS_WAIT":
        //        case "GNSS_WAIT":
        //        case "GPS_TTFF":
        //        case "GNSS_TTFF":
        //        case "GPS_SVCOUNT":
        //        case "GNSS_SVCOUNT":
        //        case "GPS_CN0":
        //        case "GNSS_CN0":
        //            break;
        //        default: return false;
        //    }

        //    string strDtime = LstJOB_CMD[iNowJobNumber].DELAY;
        //    double dTime = 1.0;

        //    if ((strDtime.Length < 1) || (strDtime.Equals("0")))
        //    {
        //        dTime = 1.0;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            dTime = double.Parse(strDtime);
        //            if (dTime <= 0.1) dTime = 0.1;
        //        }
        //        catch { dTime = 0.1; }

        //        DelayChecker(dTime, false);
        //    }

        //    switch (strCmdType)
        //    {
        //        case "GPS_START":
        //        case "GPS_COLD_START":
        //        case "GPS_HOT_START":
        //        case "GPS_WARM_START":
        //            STEPMANAGER_VALUE.bGen12CCMGpsInfo = true;
        //            STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Restart();
        //            STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Restart();
        //            break;
        //        case "GPS_STOP":
        //            STEPMANAGER_VALUE.bGen12CCMGpsInfo = false;
        //            STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Stop();
        //            STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Stop();
        //            break;
        //    }

        //    switch (strCmdType)
        //    {
        //        case "GPS_WAIT":
        //        case "GNSS_WAIT":
        //        case "GPS_TTFF":
        //        case "GNSS_TTFF":
        //        case "GPS_SVCOUNT":
        //        case "GNSS_SVCOUNT":
        //        case "GPS_CN0":
        //        case "GNSS_CN0":
        //            MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
        //            if (!STEPMANAGER_VALUE.bGen12CCMGpsInfo)
        //            {
        //                GateWayMsgProcess((int)STATUS.NG, "GPS NOT STARTED", "GPS NOT STARTED", String.Empty, true);
        //                return true;
        //            }
        //            break;
        //        default: return false;
        //    }

        //    switch (strCmdType)
        //    {
        //        case "GPS_WAIT":
        //            if (STEPMANAGER_VALUE.stGEN12CCMGpsInfo.bPositionFixed)
        //                GateWayMsgProcess((int)STATUS.OK, "POSITION FIX(GPS)", "POSITION FIX(GPS)", String.Empty, true);
        //            else
        //                GateWayMsgProcess((int)STATUS.NG, "POSITION NO-FIX(GPS)", "POSITION NO-FIX(GPS)", String.Empty, true);
        //            return true;

        //        case "GNSS_WAIT":
        //            if (STEPMANAGER_VALUE.stGEN12CCMGnssInfo.bPositionFixed)
        //                GateWayMsgProcess((int)STATUS.OK, "POSITION FIX(GNSS)", "POSITION FIX(GNSS)", String.Empty, true);
        //            else
        //                GateWayMsgProcess((int)STATUS.NG, "POSITION NO-FIX(GNSS)", "POSITION NO-FIX(GNSS)", String.Empty, true);
        //            return true;

        //        case "GPS_TTFF":
        //            if (STEPMANAGER_VALUE.stGEN12CCMGpsInfo.bPositionFixed)
        //                GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);
        //            else
        //                GateWayMsgProcess((int)STATUS.NG, STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen12CCMGpsInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);
        //            return true;

        //        case "GNSS_TTFF":
        //            if (STEPMANAGER_VALUE.stGEN12CCMGnssInfo.bPositionFixed)
        //                GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);
        //            else
        //                GateWayMsgProcess((int)STATUS.NG, STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), STEPMANAGER_VALUE.Gen12CCMGnssInfoStopWatch.Elapsed.TotalSeconds.ToString("0.0"), String.Empty, true);
        //            return true;

        //        case "GPS_SVCOUNT":
        //            GateWayMsgProcess((int)STATUS.OK, ((int)STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SvCounts).ToString(), ((int)STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SvCounts).ToString(), String.Empty, true);
        //            return true;

        //        case "GNSS_SVCOUNT":
        //            GateWayMsgProcess((int)STATUS.OK, ((int)STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SvCounts).ToString(), ((int)STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SvCounts).ToString(), String.Empty, true);
        //            return true;

        //        case "GPS_CN0":
        //            double[] dValueGpsCn0 = new double[STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo.Length];
        //            for (int i = 0; i < dValueGpsCn0.Length; i++)
        //                dValueGpsCn0[i] = STEPMANAGER_VALUE.stGEN12CCMGpsInfo.SVInfo[i].dCn0;

        //            string strGpsVal = String.Empty;

        //            try { strGpsVal = dValueGpsCn0.Max().ToString(); }
        //            catch { strGpsVal = "0"; }

        //            GateWayMsgProcess((int)STATUS.OK, strGpsVal, strGpsVal, String.Empty, true);
        //            return true;

        //        case "GNSS_CN0":
        //            double[] dValueGnssCn0 = new double[STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo.Length];
        //            for (int i = 0; i < dValueGnssCn0.Length; i++)
        //                dValueGnssCn0[i] = STEPMANAGER_VALUE.stGEN12CCMGnssInfo.SVInfo[i].dCn0;

        //            string strGnssVal = String.Empty;
        //            try { strGnssVal = dValueGnssCn0.Max().ToString(); }
        //            catch { strGnssVal = "0"; }

        //            GateWayMsgProcess((int)STATUS.OK, strGnssVal, strGnssVal, String.Empty, true);
        //            return true;

        //        default: return false;

        //    }

        //}

        private void DelayChecker(double dTime, bool bOptionView)
        {
            PageDelayStopWatch.Restart();
            bool bPass = true;
            while (true)
            {
                if (!Item_bTestStarted && !STEPMANAGER_VALUE.bInteractiveMode)
                {
                    PageDelayStopWatch.Stop();
                    return;
                }

                if (PageDelayStopWatch.ElapsedMilliseconds >= (dTime * 1000))
                {
                    PageDelayStopWatch.Stop();
                    break;
                }
                System.Threading.Thread.Sleep(50);

                bPass = !bPass;
                if (bOptionView && bPass)
                {
                    GateWayMsgProcess((int)STATUS.DELAYLAPSE, "WAITING", ((double)PageDelayStopWatch.ElapsedMilliseconds / (double)(1000)).ToString("0.0"), String.Empty, true);
                }
            }

            return;
        }

        
        //절차진행하는 사이클엔진에서 GEN10 OLD GPS 명령 수행하는 루틴 
        private bool Gen9ExceptionCommand(string strCmdType)
        {
            switch (strCmdType)
            {
                case "[H]MAKE_DECIMAL_TO_HEXA":
                case "MAKE_DECIMAL_TO_HEXA":
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);

                    string strParSTID = LstJOB_CMD[iNowJobNumber].PAR1;
                    if (String.IsNullOrEmpty(strParSTID))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 EMPTY()", "PAR1 EMPTY()", String.Empty, true);                        
                    }
                    else
                    {
                        int iSTID = 0;
                        bool bConvertSTID = int.TryParse(strParSTID, out iSTID);

                        if (bConvertSTID)
                        {
                            byte[] bytesSTID = new byte[4];
                            bytesSTID = BitConverter.GetBytes(iSTID);
                            Array.Reverse(bytesSTID);
                            string strResult = String.Empty;
                            strResult = BitConverter.ToString(bytesSTID).Replace("-", "");
                            GateWayMsgProcess((int)STATUS.OK, strResult, strResult, String.Empty, true);
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);                            
                        }
                    }

                    return true;
                default: return false;

            }

        }
        //절차진행하는 사이클엔진에서 GEN10 OLD GPS 명령 수행하는 루틴 
        private bool Gen9GpsCommand(string strCmdType)
        {
            switch (strCmdType)
            {
                case "GPS_TTFF_START":
                case "GPS_NAV_VALID":
                case "GPS_TTFF_WAIT":
                case "GPS_DISTANCE":
                case "GPS_SV_COUNT":
                case "GPS_CN0_AVERAGE":
                case "GPS_CN0_MAX":

                case "[H]GPS_TTFF_START":
                case "[H]GPS_NAV_VALID":
                case "[H]GPS_TTFF_WAIT":
                case "[H]GPS_DISTANCE":
                case "[H]GPS_SV_COUNT":
                case "[H]GPS_CN0_AVERAGE":
                case "[H]GPS_CN0_MAX":

                        break;
                default: return false;

            }

            string strDtime = LstJOB_CMD[iNowJobNumber].DELAY;
            double dTime = 1.0;

            if ((strDtime.Length < 1) || (strDtime.Equals("0")))
            {
                dTime = 1.0;
            }
            else
            {
                try
                {
                    dTime = double.Parse(strDtime);
                    if (dTime <= 0.1) dTime = 0.1;
                }
                catch { dTime = 0.1; }

                DelayChecker(dTime, false);
            }


            switch (strCmdType)
            {
                case "[H]GPS_TTFF_START": 
                case "GPS_TTFF_START": STEPMANAGER_VALUE.bGen9GpsOldInfo = true;
                    STEPMANAGER_VALUE.bGen9GpsNavOn = false;
                    STEPMANAGER_VALUE.strGen9GpsNavild = "FF";
                    STEPMANAGER_VALUE.strGen9GpsTTFF = "-1";
                    STEPMANAGER_VALUE.dGen9GpsLat = 0.0;
                    STEPMANAGER_VALUE.dGen9GpsLon = 0.0;
                    STEPMANAGER_VALUE.iGen9GpsCount = 0;
                    STEPMANAGER_VALUE.iGen9GpsCn0Max = 0;
                    STEPMANAGER_VALUE.iGen9GpsCn0Aver = 0;
                    
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    GateWayMsgProcess((int)STATUS.OK, "GPS REPORT START", "GPS REPORT START", String.Empty, true);
                    break;

                case "[H]GPS_NAV_VALID": 
                case "GPS_NAV_VALID": MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.strGen9GpsNavild, STEPMANAGER_VALUE.strGen9GpsNavild, String.Empty, true);
                    break;

                case "[H]GPS_TTFF_WAIT":
                case "GPS_TTFF_WAIT":
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.strGen9GpsTTFF, STEPMANAGER_VALUE.strGen9GpsTTFF, String.Empty, true);
                    break;

                case "[H]GPS_DISTANCE":
                case "GPS_DISTANCE":
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    if (String.IsNullOrEmpty(LstJOB_CMD[iNowJobNumber].PAR1))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                        return true;
                    }

                    if (!LstJOB_CMD[iNowJobNumber].PAR1.Contains(','))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR.", "PAR1 ERROR.", String.Empty, true);
                        return true;
                    }

                    string[] strParam = LstJOB_CMD[iNowJobNumber].PAR1.Split(',');

                    if (strParam.Length != 2)
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERR", "PAR1 ERR.", String.Empty, true);
                        return true;
                    }

                    try
                    {
                        double lat1 = STEPMANAGER_VALUE.dGen9GpsLat * Math.PI / 180;
                        double lat2 = double.Parse(strParam[0]) * Math.PI / 180;
                        double lon1 = STEPMANAGER_VALUE.dGen9GpsLon * Math.PI / 180;
                        double lon2 = double.Parse(strParam[1]) * Math.PI / 180;

                        double dR = 6371;
                        double alat = Math.Sin((lat1 - lat2) / 2);
                        double alon = Math.Sin((lon1 - lon2) / 2);
                        double a = alat * alat + Math.Cos(lat1) * Math.Cos(lat2) * alon * alon;
                        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                        double d = dR * c * 1000.0;
                        GateWayMsgProcess((int)STATUS.OK, d.ToString("0.0000"), d.ToString("0.0000"), String.Empty, true);
                    }
                    catch
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1-ERROR", "PAR1-ERROR.", String.Empty, true);
                        return true;
                    }
                    break;

                case "[H]GPS_SV_COUNT":
                case "GPS_SV_COUNT":
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iGen9GpsCount.ToString(), STEPMANAGER_VALUE.iGen9GpsCount.ToString(), String.Empty, true);
                    break;

                case "[H]GPS_CN0_AVERAGE":
                case "GPS_CN0_AVERAGE":
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iGen9GpsCn0Aver.ToString(), STEPMANAGER_VALUE.iGen9GpsCn0Aver.ToString(), String.Empty, true);
                    break;

                case "[H]GPS_CN0_MAX":
                case "GPS_CN0_MAX":
                    MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                    GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iGen9GpsCn0Max.ToString(), STEPMANAGER_VALUE.iGen9GpsCn0Max.ToString(), String.Empty, true);
                    break;               

                default: return false;

            }
            return true;
        }

        //절차진행하는 사이클엔진에서 GEN10 OLD GPS 명령 수행하는 루틴
        private bool Gen10OldGpsCommand(string strCmdType)
        {
            switch (strCmdType)
            {
                case "GPS_TTFF_START": 
                case "GPS_NAV_VALID":
                case "GPS_TTFF_WAIT":
                case "GPS_DISTANCE":
                case "GPS_SV_COUNT":
                case "GPS_CN0_AVERAGE":
                case "GPS_CN0_MAX":
                case "GNSS_SV_COUNT":
                case "GNSS_CN0_AVERAGE":
                case "GNSS_CN0_MAX": break;
                default: return false;

            }

            string strDtime = LstJOB_CMD[iNowJobNumber].DELAY;
            double dTime = 1.0;
            
            if ((strDtime.Length < 1) || (strDtime.Equals("0")))
            {
                dTime = 1.0;
            }
            else
            {
                try
                {
                    dTime = double.Parse(strDtime);
                    if (dTime <= 0.1) dTime = 0.1;
                }
                catch { dTime = 0.1; }

                DelayChecker(dTime, false);
            }


            switch (strCmdType)
            {
                case "GPS_TTFF_START": STEPMANAGER_VALUE.bGen10GpsOldInfo = true;
                                       STEPMANAGER_VALUE.bGen10GpsNavOn = false;
                                       STEPMANAGER_VALUE.strGen10GpsNavild = "FF";
                                       STEPMANAGER_VALUE.strGen10GpsTTFF   = "-1";
                                       STEPMANAGER_VALUE.dGen10GpsLat = 0.0;
                                       STEPMANAGER_VALUE.dGen10GpsLon = 0.0;
                                       STEPMANAGER_VALUE.iAttGpsCount = 0;
                                       STEPMANAGER_VALUE.iAttGpsCn0Max = 0;
                                       STEPMANAGER_VALUE.iAttGpsCn0Aver = 0;
                                       STEPMANAGER_VALUE.iAttGnssCount = 0;
                                       STEPMANAGER_VALUE.iAttGnssCn0Max = 0;
                                       STEPMANAGER_VALUE.iAttGnssCn0Aver = 0;
                                       MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                       GateWayMsgProcess((int)STATUS.OK, "GPS REPORT START", "GPS REPORT START", String.Empty, true);                                  
                                       break;

                case "GPS_NAV_VALID":  MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                       GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.strGen10GpsNavild, STEPMANAGER_VALUE.strGen10GpsNavild, String.Empty, true);
                                       break;

                case "GPS_TTFF_WAIT":  
                                       MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                       GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.strGen10GpsTTFF, STEPMANAGER_VALUE.strGen10GpsTTFF, String.Empty, true);
                                       break;
                case "GPS_DISTANCE":
                                       MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                       if (String.IsNullOrEmpty(LstJOB_CMD[iNowJobNumber].PAR1))
                                       {
                                           GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                                           return false;
                                       }

                                       if (!LstJOB_CMD[iNowJobNumber].PAR1.Contains(','))
                                       {
                                           GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR.", "PAR1 ERROR.", String.Empty, true);
                                           return false;
                                       }

                                       string[] strParam = LstJOB_CMD[iNowJobNumber].PAR1.Split(',');

                                       if (strParam.Length != 2)
                                       {
                                           GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERR", "PAR1 ERR.", String.Empty, true);
                                           return false;
                                       }

                                       try
                                       {
                                           double lat1 = STEPMANAGER_VALUE.dGen10GpsLat * Math.PI / 180;
                                           double lat2 = double.Parse(strParam[0]) * Math.PI / 180;
                                           double lon1 = STEPMANAGER_VALUE.dGen10GpsLon * Math.PI / 180;
                                           double lon2 = double.Parse(strParam[1]) * Math.PI / 180;

                                           double dR = 6371;
                                           double alat = Math.Sin((lat1-lat2)/2);
                                           double alon = Math.Sin((lon1-lon2)/2);
                                           double a = alat*alat + Math.Cos(lat1)*Math.Cos(lat2)*alon*alon;
                                           double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
                                           double d = dR * c * 1000.0;
                                           GateWayMsgProcess((int)STATUS.OK, d.ToString("0.0000"), d.ToString("0.0000"), String.Empty, true);
                                       }
                                       catch
                                       {
                                           GateWayMsgProcess((int)STATUS.CHECK, "PAR1-ERROR", "PAR1-ERROR.", String.Empty, true);
                                           return false;
                                       }                                       
                                       break;

                case "GPS_SV_COUNT":
                                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iAttGpsCount.ToString(), STEPMANAGER_VALUE.iAttGpsCount.ToString(), String.Empty, true);
                                       break;
                case "GPS_CN0_AVERAGE":
                                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iAttGpsCn0Aver.ToString(), STEPMANAGER_VALUE.iAttGpsCn0Aver.ToString(), String.Empty, true);
                                       break;
                case "GPS_CN0_MAX":
                                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iAttGpsCn0Max.ToString(), STEPMANAGER_VALUE.iAttGpsCn0Max.ToString(), String.Empty, true);
                                       break;
                case "GNSS_SV_COUNT":
                                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iAttGnssCount.ToString(), STEPMANAGER_VALUE.iAttGnssCount.ToString(), String.Empty, true);
                                       break;
                case "GNSS_CN0_AVERAGE":
                                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iAttGnssCn0Aver.ToString(), STEPMANAGER_VALUE.iAttGnssCn0Aver.ToString(), String.Empty, true);
                                       break;
                case "GNSS_CN0_MAX":
                                        MessageLogging((int)LOGTYPE.TX, LstJOB_CMD[iNowJobNumber].DISPLAYNAME, (int)DEFINES.SET1);
                                        GateWayMsgProcess((int)STATUS.OK, STEPMANAGER_VALUE.iAttGnssCn0Max.ToString(), STEPMANAGER_VALUE.iAttGnssCn0Max.ToString(), String.Empty, true);
                                       break;

                default: return false;                    
                     
            }
            return true;
        }

        //절차진행하는 사이클엔진에서 NAD 명령을 수행하는 루틴
        private bool NadKeyDllCommand(int iPort, int iJobNum, string strCmdType, string strSendparEx)
        {         
            bool bThere = false; //NAD USB 포트가 붙었는지 먼저 조사하자..움하하하하핧핳하핳
            COMMDATA resData = new COMMDATA();

            foreach (string s in SerialPort.GetPortNames())
            {
                if (s.Equals("COM" + FixNadPort.ToString()))
                {
                    bThere = true;
                    break;
                }
            }

            if(!DK_NADKEY.IsPortOpen() && strCmdType.Contains("CLOSE"))
            {                
                resData.iPortNum = iPort;
                resData.iStatus = (int)STATUS.OK;
                resData.ResponseData = "ALREADY PORT CLOSED:" + FixNadPort.ToString();
                resData.ResultData = "ALREADY PORT CLOSED:" + FixNadPort.ToString();
                resData.SendPacket = strCmdType;
                DKLoggerMR.WriteCommLog("[NAD PORT] ALREADY CLOSED:" + FixNadPort.ToString(), strCmdType, false);
                GateWay_MANAGER(resData);
                return false;
            }

            if (!bThere)
            {
                resData.iPortNum = iPort;
                resData.iStatus = (int)STATUS.NG;
                resData.ResponseData = "NAD PORT CLOSED:" + FixNadPort.ToString();
                resData.ResultData = "NAD PORT CLOSED:" + FixNadPort.ToString();
                resData.SendPacket = strCmdType;
                DKLoggerMR.WriteCommLog("[NAD PORT] CLOSED:" + FixNadPort.ToString(), strCmdType, false);
                GateWay_MANAGER(resData);
                return false;
            }          
            
            if ((!strCmdType.Contains("OPEN") && !strCmdType.Contains("NAME") && !DK_NADKEY.IsPortOpen()) && !strCmdType.Contains("CLOSE"))
            {
                if (!strCmdType.Equals("DLL_VERSION"))
                {
                    resData.iPortNum = iPort;
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "NAD PORT CLOSED(" + FixNadPort.ToString() + ")";
                    resData.ResultData = "NAD PORT CLOSED(" + FixNadPort.ToString() + ")";
                    resData.SendPacket = strCmdType;
                    DKLoggerMR.WriteCommLog("[NAD PORT] CLOSED:" + FixNadPort.ToString(), strCmdType, false);
                    GateWay_MANAGER(resData);
                    return false;
                }
                
            }                       

            if (!strCmdType.Equals("DLL_FILE_NAME"))
            {
                if (!DK_NADKEY.IsOnLibrary())
                {
                    resData.iPortNum = iPort;
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "UNLOADED NAD DLL";
                    resData.ResultData = "UNLOADED NAD DLL";
                    resData.SendPacket = strCmdType;
                    DKLoggerMR.WriteCommLog("[ERROR] UNLOADED NAD DLL", strCmdType, false);
                    GateWay_MANAGER(resData);
                    return false;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(strSendparEx))
                {
                    resData.iPortNum = iPort;
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "NO DLL FILE NAME";
                    resData.ResultData = "NO DLL FILE NAME";
                    resData.SendPacket = strCmdType;
                    DKLoggerMR.WriteCommLog("[ERROR] NO DLL FILE NAME", strCmdType, false);
                    GateWay_MANAGER(resData);
                    return false;
                }
            }

            switch (strCmdType)
            {                
                case "DLL_FILE_NAME": NadKeyDllFileName(iJobNum, strCmdType, strSendparEx);
                    return true;

                case "DLL_PORT_OPEN_NA": NadKeyDllOpen(iPort); return true;

                case "DLL_PORT_CLOSE_NA":NadKeyDllClose(iPort); return true;

                case "DLL_READ_IMEI_NA": NadKeyDllReadImei(iPort); return true;

                case "DLL_WRITE_IMEI_NA": NadKeyDllWriteImei(iPort, strSendparEx); return true;

                case "DLL_CHECKSUM_IMEI": NadKeyDllCheckSumImei(iPort, strSendparEx); return true;

                case "DLL_NV_RESTORE": NadKeyDllNvRestore(iPort); return true;

                case "DLL_READ_MSISDN_NA": NadKeyDllReadMsisdn(iPort); return true;

                case "DLL_READ_ICCID_NA": NadKeyDllReadIccid(iPort); return true;

                case "DLL_READ_IMSI_NA": NadKeyDllReadImsi(iPort); return true;

                case "DLL_EFS_BACKUP": NadKeyDllEFSBackup(iPort); return true;

                case "DLL_VERSION": NadKeyDllVersion(iPort); return true;

                case "DLL_READ_SCNV": NadKeyDllReadSCNV(iPort); return true;

                case "DLL_NV_GET":

                    if (String.IsNullOrEmpty(strSendparEx))
                    {
                        
                        resData.iPortNum = iPort;
                        resData.iStatus = (int)STATUS.CHECK;
                        resData.ResponseData = "EMPTY NV NUMBER(PAR1)";
                        resData.ResultData = "EMPTY NV NUMBER(PAR1)";
                        resData.SendPacket = strCmdType;
                        DKLoggerMR.WriteCommLog("EMPTY NV NUMBER(PAR1)", strCmdType, false);
                        GateWay_MANAGER(resData);
                        return false;
                    }

                    int iNvNumber = 0;
                    try
                    {
                        iNvNumber = int.Parse(strSendparEx);
                    }
                    catch 
                    {
                        
                        resData.iPortNum = iPort;
                        resData.iStatus = (int)STATUS.CHECK;
                        resData.ResponseData = "ERROR NV NUMBER(PAR1)";
                        resData.ResultData = "ERROR NV NUMBER(PAR1)";
                        resData.SendPacket = strCmdType;
                        DKLoggerMR.WriteCommLog("ERROR NV NUMBER(PAR1)", strCmdType, false);
                        GateWay_MANAGER(resData);
                        return false;
                    }

                    NadKeyDllNVGet(iPort, iNvNumber); return true;
                
                default:
                    
                    resData.iPortNum = iPort;
                    resData.iStatus = (int)STATUS.NG;
                    resData.ResponseData = "UNKNOWN COMMAND";
                    resData.ResultData = STATUS.NG.ToString();
                    resData.SendPacket = strCmdType;
                    DKLoggerMR.WriteCommLog("[UNKNOWN COMMAND]ERROR", strCmdType, false);
                    GateWay_MANAGER(resData);
                    return false;

            }

        }
        
        //GMES 명령을 수행하는 과정에서 NG 가 있다면 포함하는 루틴
        private string NgCommandList(int iPort, int iJobNum)
        {
            //절차서에 아이템 코드가 있는지 검사하여 추가한다. 없으면 EMPTY 반환.
            string tmpString = String.Empty;
            string tmpItemString = String.Empty;
            string tmpOKNG = String.Empty;
            string tmpName = String.Empty;
            for (int i = 0; i < iJobNum; i++)
            {
                if (LstJOB_CMD[i].MESCODE == null || LstJOB_CMD[i].MESCODE.Length == 0 && (LstJOB_CMD[i].ACTION.Equals("RUN") || LstJOB_CMD[i].ACTION.Equals("ENC")))
                {
                    switch (LstJOB_CMD[i].COMPARE)
                    {
                        case "NONE": //판정이 없는 명령 제외한다.
                            break;
                        default:     //나머진 판정결과를 포함한다.
                                     //if (SEQUENCE_DIC[iPort][i] == (int)STATUS.NG)
                                     //{
                                     //    if (LstJOB_CMD[i].DISPLAYNAME.Length > 0)
                                     //    {
                                     //        tmpName = "(No." + (i + 1).ToString() + ")" + LstJOB_CMD[i].DISPLAYNAME;
                                     //    }
                                     //    else
                                     //    {
                                     //        tmpName = "(No." + (i + 1).ToString() + ")" + LstJOB_CMD[i].CMD;
                                     //    }

                            //    tmpOKNG = "NG";
                            //    tmpItemString = RESULTDT_DIC[iPort][i].Replace(",", "/");
                            //    tmpString += ("," + tmpName + "=" + tmpItemString + ";" + tmpOKNG);
                            //}
                            //CSMES
                            if (SEQUENCE_DIC[iPort][i] == (int)STATUS.NG)
                            {
                                if (STEPMANAGER_VALUE.bUseOSIMES)   //CS 는 Line Number 를 upload 하지 않는다.
                                {
                                    if (LstJOB_CMD[i].DISPLAYNAME.Length > 0)
                                    {
                                        tmpName = LstJOB_CMD[i].DISPLAYNAME;
                                    }
                                    else
                                    {
                                        tmpName = LstJOB_CMD[i].CMD;
                                    }
                                }
                                else
                                {
                                    if (LstJOB_CMD[i].DISPLAYNAME.Length > 0)
                                    {
                                        tmpName = "(No." + (i + 1).ToString() + ")" + LstJOB_CMD[i].DISPLAYNAME;
                                    }
                                    else
                                    {
                                        tmpName = "(No." + (i + 1).ToString() + ")" + LstJOB_CMD[i].CMD;
                                    }
                                }

                                tmpOKNG = "NG";
                                tmpItemString = RESULTDT_DIC[iPort][i].Replace(",", "/");
                                tmpString += ("," + tmpName + "=" + tmpItemString + ";" + tmpOKNG);
                            }
                            break;
                    }
                }
            }

            if (tmpString.Length > 0) //CRLF 제거.
            {
                tmpString = tmpString.Replace("\r", String.Empty);
                tmpString = tmpString.Replace("\n", String.Empty);
            }

            return tmpString;
        }

        //GMES 명령을 수행하는 과정에서 ITEMCODE 가 있다면 포함하는 루틴
        private string GmesItemCoding(int iPort, int iJobNum)
        {
            //절차서에 아이템 코드가 있는지 검사하여 추가한다. 없으면 EMPTY 반환.
            string tmpString = String.Empty;
            string tmpItemString = String.Empty;
            string tmpOKNG = String.Empty;

            for (int i = 0; i < LstTST_RES[iPort].Count; i++)
            {
                if (LstJOB_CMD[i].MESCODE != null && LstJOB_CMD[i].MESCODE.Length > 0 && (LstJOB_CMD[i].ACTION.Equals("RUN") || LstJOB_CMD[i].ACTION.Equals("ENC")))
                {
                    //******************************************************** 예외처리구간.      
          
                    RESULTDT_DIC[iPort][i] = LstTST_RES[iPort][i].ResultData;

                    switch (LstJOB_CMD[i].MESCODE)
                    {
                            //STID,  GM_PART_NO, BASE_MODEL_PART_NO값 경우에는 다시 데시멀로 바꾸어 올리란다....잭일슨
                        case "STID": 
                        case "GM_PART_NO":
                        case "BASE_MODEL_PART_NO":
                                    
                                    string strOrigin = RESULTDT_DIC[iPort][i];
                                    try
                                    {                           
                                        uint uDecVal = uint.Parse(strOrigin, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        RESULTDT_DIC[iPort][i] = uDecVal.ToString();
                         
                                    }
                                    catch 
                                    {
                                        RESULTDT_DIC[iPort][i] = strOrigin;
                                    }
                                    break;

                        case "IMEI": //IMEI 의 경우 14자리만 올린다.
                                    string strOriginIMEI = RESULTDT_DIC[iPort][i];
                                    if (strOriginIMEI != null && strOriginIMEI.Length > 14)
                                    {
                                        RESULTDT_DIC[iPort][i] = strOriginIMEI.Substring(0, 14);
                                    }

                                    break;
                        default:
                              
                                    break;

                    }

                    
                    //********************************************************
                               

                    switch (LstJOB_CMD[i].COMPARE)
                    {
                        case "NONE": //판정이 없는 명령이면 OK or NG 를 올리지 않는다.
                            tmpString += ("," + LstJOB_CMD[i].MESCODE + "=" +
                                          RESULTDT_DIC[iPort][i]);
                            break;
                        default:     //나머진 판정결과를 포함한다. 아이템코드 고쳐야된다. goto문에 의해 stop 되고 stop on stepcomplete 되면 아템코드 전부ng로 올라간다.

                            bool bRes = false;
                            if (i < LstTST_RES[iPort].Count)
                            {
                                switch (LstTST_RES[iPort][i].iStatus)
                                {
                                    case (int)STATUS.OK: tmpOKNG = "OK"; break;
                                    case (int)STATUS.SKIP: bRes = true; break;  //SKIP은 올리지 않는다.
                                    default: tmpOKNG = "NG"; break;
                                }
                                if (!bRes)
                                {
                                    tmpItemString = RESULTDT_DIC[iPort][i].Replace(",", "/");
                                    tmpString += ("," + LstJOB_CMD[i].MESCODE + "=" + tmpItemString + ";" + tmpOKNG);
                                }
                            }

                            break;
                    }
                }
            }

            //전체검사 시간 올리기. tact time string
            tmpString += (",INSPECTION_TACT=" + STEPMANAGER_VALUE.strTactTime);

            if (tmpString.Length > 0) //CRLF 제거.
            {
                tmpString = tmpString.Replace("\r", String.Empty);
                tmpString = tmpString.Replace("\n", String.Empty);
            }

            return tmpString;
        }

        //MES 명령을 수행하는 과정에서 ITEMCODE 가 있다면 내용을 가져오는 루틴
        private List<string> MesItemCoding(int iPort, int iJobNum)
        {
            //절차서에 아이템 코드가 있는지 검사하여 추가한다. 없으면 EMPTY 반환.
            string tmpString = String.Empty;
            string tmpItemString = String.Empty;
            string tmpOKNG = String.Empty;
            List<string> lstResult = new List<string>();
            lstResult.Clear();
            for (int i = 0; i < iJobNum; i++)
            {
                if (LstJOB_CMD[i].MESCODE != null && LstJOB_CMD[i].MESCODE.Length > 0 && (LstJOB_CMD[i].ACTION.Equals("RUN") || LstJOB_CMD[i].ACTION.Equals("ENC")))
                {
                    if (SEQUENCE_DIC[iPort][i] != (int)STATUS.OK)
                        tmpOKNG = "NG";
                    else
                        tmpOKNG = "OK";

                    try
                    {
                        double dTestVal = double.Parse(RESULTDT_DIC[iPort][i]);
                    }
                    catch
                    {
                        continue;
                    }
                    lstResult.Add(LstJOB_CMD[i].MESCODE + "=" + tmpOKNG + "=" + RESULTDT_DIC[iPort][i]);
                }
            }

            return lstResult;
        }

        //절차진행하는 과정에서 해당명령의 수행옵션(RUN, SKIP)을 확인하는 루틴
        private int ConvertAction(string strAction)
        {
            switch (strAction)
            {
                case "ENC": 
                case "RUN":  return (int)ACTION.RUN;
                
                case "SKIP": return (int)ACTION.SKIP;

                default: return (int)ACTION.RUN;
            }
        }

        //절차진행하는 과정에서 해당명령의 통신옵션(SEND, RECV)을 확인하는 루틴
        private int ConvertOption(string strOption)
        {
            switch (strOption)
            {
                case "NORESPONSE": return (int)MODE.NORESPONSE;
                case "SENDRECV":   return (int)MODE.SENDRECV;
                case "HIDDEN"  :   return (int)MODE.SENDRECV; //로그 히든을 위한. ITEMDATAVIEW 전용
                case "SEND"    :   return (int)MODE.SEND;
                case "RECV"    :   return (int)MODE.RECV;
                case "BUFFER"  :   return (int)MODE.BUFFER;
                case "UNTIL"   :   return (int)MODE.UNTIL;
                case "AVERAGE" :   return (int)MODE.AVERAGE;
                case "RECVSEND":   return (int)MODE.RECVSEND;
                case "MULTIPLE":   return (int)MODE.MULTIPLE;                
                default        :   return (int)MODE.SENDRECV;
            }     
        }

        //절차진행하는 과정에서 해당명령 수행 후 행동을 결정해주는 루틴
        private void CheckCaseNgCommand()
        {
            bool bRes = true;
            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)   //1. 모든 슬롯수만큼 돌면서
            {
                if (UseSlots[i])                        //2. 사용할 슬롯이며 오픈된 포트이며 
                {
                    switch (SEQUENCE_DIC[i][iNowJobNumber])
                    {
                        case (int)STATUS.OK: break;//명령 완료(OK)                      
                        case (int)STATUS.SKIP: CycleSignDone(i, (int)STATUS.SKIP, iNowJobNumber); break; //SKIP 이면 
                        case (int)STATUS.NONE:    //NONE 이면
                        case (int)STATUS.RUNNING: break;   //SKIP 이면
                        default: bRes = false;
                            CycleSignDone(i, (int)STATUS.NG, iNowJobNumber);
                            break;   //기타
                    }
                }
            }

            string strCaseNg = LstJOB_CMD[iNowJobNumber].CASENG;

            //20241021  JUMP_CHECK 명령어가 GOTO 사이에 있을 경우 LABEL 때문에 CASENG 가 모두 GOTO: 로 변경됨
            //때문에 strCaseNg 뿐만 아니라 JUMP_CHECK 명령어도 같이 체크함
            //if (strCaseNg.StartsWith("JUMP") || (LstJOB_CMD[iNowJobNumber].TYPE  == "PAGE" && LstJOB_CMD[iNowJobNumber].CMD.Contains("JUMP_CHECK")))
            if (strCaseNg.StartsWith("JUMP"))
                CycleSignJump();
            else
            {
                if (!bRes)
                {
                    switch (strCaseNg)
                    { //시리얼 통신단에서 FAIL 일경우 처리.

                        case "CONTINUE": CycleSignNext(); break;  // 아니면 다음 절차로 이동 

                        case "CHECK":  //check는 mes로 올리지 말아달라고 이동성 책임 요청. (2018.07.01)
                        case "MES":
                        case "EMPTY":
                        case "ERROR": iNowJobNumber = LstJOB_CMD.Count; break;


                        case "STOP":
                            if (!CheckMesCompleteOnStop())// true 면 그 함수에서 inowjobnumber 가 변경된다.    
                            {
                                iNowJobNumber = LstJOB_CMD.Count;

                            }
                            iNowJobNumber--;
                            CycleSignNext();
                            break;
                        //iNowJobNumber = LstJOB_CMD.Count; break; //  EMPTY 또는 STOP 테스트 종료

                        case "PAUSE":
                            if (LstJOB_CMD[iNowJobNumber].TYPE == "GMES")
                            {
                                if (STEPMANAGER_VALUE.bUseMesOn)
                                {
                                    CycleSignPause(true, "WARNING", "GMES " + RESPONSE_DIC[(int)DEFINES.SET1][iNowJobNumber] + " : " + RESULTDT_DIC[(int)DEFINES.SET1][iNowJobNumber]); // 아직 미정이므로 그냥 다음 절차로 이동
                                }
                                else
                                {
                                    CycleSignNext();
                                }
                                break;
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(RESPONSE_DIC[(int)DEFINES.SET1][iNowJobNumber]))
                                    CycleSignPause(false, "", "");
                                else
                                    CycleSignPause(true, "WARNING", RESPONSE_DIC[(int)DEFINES.SET1][iNowJobNumber]); // 아직 미정이므로 그냥 다음 절차로 이동
                                break;
                            }

                        case "MONITOR": CycleSignNext(); break;  // 모니터링용 명령이므로 그냥 다음 절차로 이동

                        default: // GOTO LABEL 이 있는 위치에서 다시 시작하거나 디폴트는 넥스트
                                 
                            string tmpGoto = strCaseNg.Replace("GOTO:", String.Empty);
                            switch (tmpGoto)
                            {
                                case "A1": case "A2": case "A3": case "A4": case "A5": case "A6": case "A7": case "A8":  case "A9": case "A10":
                                case "A11": case "A12": case "A13": case "A14": case "A15": case "A16": case "A17": case "A18": case "A19":
                                case "C1": case "C2": case "C3": case "C4": case "C5": case "C6": case "C7": case "C8": case "C9": case "C10":
                                case "C11": case "C12": case "C13": case "C14": case "C15": case "C16": case "C17": case "C18": case "C19": 
                                      CycleSignBack(); break;
                               
                                default: CycleSignNext(); break;  // 아직 미정이므로 그냥 다음 절차로 이동 

                            }
                    break;   
                    }
                }
                else
                {
                    CycleSignNext(); // 아니면 다음 절차로 이동           
                }
            }
            return; 
        }

        //절차진행하는 과정에서 EXPR 이 있는지 확인하는 부분
        private bool CheckExpression(int iPort, int iNowJobNum)
        {
            //EXPR 검사
            if (LstJOB_CMD[iNowJobNum].EXPR != null && LstJOB_CMD[iNowJobNum].EXPR.Length > 0)
            {
                //EXPR TYPE 검사
                int iType = DKExpr[iPort].GetExprType(LstJOB_CMD[iNowJobNum].EXPR);
                string strReturnValue = String.Empty;
                bool bExpr = false;
                switch (iType)
                {
                    case (int)EXPRTYPE.SAVE:
                        bExpr = DKExpr[iPort].ExcuteSave(LstJOB_CMD[iNowJobNum].EXPR, RESULTDT_DIC[iPort][iNowJobNum]);
                        break;
                    case (int)EXPRTYPE.LOAD:
                        bExpr = DKExpr[iPort].ExcuteLoad(LstJOB_CMD[iNowJobNum].EXPR, ref strReturnValue);
                        if (bExpr) UpdateDictionary((int)DICINDEX.RESULTDT, iPort, iNowJobNum, strReturnValue);
                        break;
                    case (int)EXPRTYPE.MATH:
                        bExpr = DKExpr[iPort].ExcuteMath(LstJOB_CMD[iNowJobNum].EXPR, RESULTDT_DIC[iPort][iNowJobNum], ref strReturnValue);
                        if (bExpr) UpdateDictionary((int)DICINDEX.RESULTDT, iPort, iNowJobNum, strReturnValue);
                        break;
                    case (int)EXPRTYPE.DEF:
                        bExpr = DKExpr[iPort].ExcuteDefine(LstJOB_CMD[iNowJobNum].EXPR, RESULTDT_DIC[iPort][iNowJobNum]);
                        break;
                    case (int)EXPRTYPE.CONV:
                        bExpr = DKExpr[iPort].ExcuteConv(LstJOB_CMD[iNowJobNum].EXPR, RESULTDT_DIC[iPort][iNowJobNum]);
                        break;
                    case (int)EXPRTYPE.HEXA:
                        bExpr = DKExpr[iPort].ExcuteHexa(LstJOB_CMD[iNowJobNum].EXPR, RESULTDT_DIC[iPort][iNowJobNum]);
                        break;
                    default: return false;
                }

                return bExpr;
            }

            return false;

        }

        //절차진행하는 과정에서 PAR1에  EXPR 또는 GMES 또는 DOC 가 있는지 확인하여 포함하는 부분
        private bool CheckExprParam(int iPort, int iNowJobNum, ref string strReturnValue)
        {   //콤마로 더하기 유지 안하는 특별한 커멘드         
            bool bReturn = true;
            strReturnValue = String.Empty;

            string strCommandParam = LstJOB_CMD[iNowJobNum].PAR1;
            bool bCommaUsed = false;  

            if (!String.IsNullOrEmpty(strCommandParam))
            {
                bCommaUsed = CheckCommaParameter(iNowJobNum);

                string[] strPars = new string[1];

                if (strCommandParam.Contains(','))
                {   // ',' 콤마가 사용되는 파라미터면
                    
                    strPars = strCommandParam.Split(',');
                }
                else
                {
                    strPars[0] = strCommandParam;
                    bCommaUsed = false;  
                }

                for (int i = 0; i < strPars.Length; i++)
                {
                    //EXPR 검사
                    int iExpr = strPars[i].IndexOf(DEFINEEXPR);
                    int iGmes = strPars[i].IndexOf(DEFINEGMES);
                    int iDoc  = strPars[i].IndexOf(DEFINEDOCU);

                    if (iExpr >= 0)
                    {
                        //TEST 20230824
                        //bool bExpr = DKExpr[(int)DEFINES.SET1].ExcuteSave("#SAVE:" + "LABEL_STID", "220899031");
                        string tmpExprStr = strPars[i].Replace(DEFINEEXPR, "#LOAD:");
                        string tmpReturnStr = String.Empty;
                        if (DKExpr[iPort].ExcuteLoad(tmpExprStr, ref tmpReturnStr))
                        {
                            if (bCommaUsed)
                            {
                                if (i < strPars.Length - 1)
                                {
                                    strReturnValue += tmpReturnStr + ",";
                                }
                                else strReturnValue += tmpReturnStr;
                            }
                            else strReturnValue += tmpReturnStr;   
                        }
                        else
                        {
                            bReturn = false;
                        }

                    }
                    else if (iGmes == 0)
                    {
                        string tmpGmesStr = strPars[i].Replace(DEFINEGMES, String.Empty);
                        string tmpReturnStr = String.Empty;
                        //CSMES
                        if (STEPMANAGER_VALUE.bUseOSIMES)
                        {
                            //return DKOSIMES.GetInsp(iPort, tmpGmesStr, ref strRtnMax);
                            if (DKOSIMES.GetInsp(iPort, tmpGmesStr, ref tmpReturnStr))
                            {
                                if (bCommaUsed)
                                {
                                    if (i < strPars.Length - 1)
                                    {
                                        strReturnValue += tmpReturnStr + ",";
                                    }
                                    else strReturnValue += tmpReturnStr;
                                }
                                else strReturnValue += tmpReturnStr;
                            }
                            else
                            {
                                bReturn = false;
                            }
                        }
                        else
                        {
                            if (DKGMES.GMES_GetInsp(iPort, tmpGmesStr, ref tmpReturnStr))
                            {
                                if (bCommaUsed)
                                {
                                    if (i < strPars.Length - 1)
                                    {
                                        strReturnValue += tmpReturnStr + ",";
                                    }
                                    else strReturnValue += tmpReturnStr;
                                }
                                else strReturnValue += tmpReturnStr;
                            }
                            else
                            {
                                bReturn = false;
                            }
                        }
                    }
                    else if (iDoc == 0)
                    {
                        string tmpDocStr = strPars[i].Replace(DEFINEDOCU, String.Empty);
                        string tmpReturnDoc = String.Empty;
                        string tmpReturnMsg = String.Empty;
                        if (GetDocumentItem(tmpDocStr, ref tmpReturnDoc, ref tmpReturnMsg))
                        {
                            if (bCommaUsed)
                            {
                                if (i < strPars.Length - 1)
                                {
                                    strReturnValue += tmpReturnDoc + ",";
                                }
                                else strReturnValue += tmpReturnDoc;
                            }
                            else strReturnValue += tmpReturnDoc;
                        }
                        else
                        {
                            bReturn = false;
                        }
                    }
                    else
                    {                        
                        if (bCommaUsed)
                        {
                            if (i < strPars.Length - 1)
                            {
                                strReturnValue += strPars[i] + ",";
                            }
                            else strReturnValue += strPars[i];
                        }
                        else strReturnValue += strPars[i];
                    }
                }                
            }
  
            return bReturn;
        }

        /// <summary>
        /// MTP120A 에서 2개의 PAR를 처리하는 함수
        /// </summary>
        /// <param name="sSendPacket"></param>
        /// <param name="sPar"></param>
        /// <param name="strReturnValue"></param>
        /// <returns></returns>
        private bool MTP120AReplaceParam(string strCmdName, string sSendPacket, string sPar, ref string strReturnValue)
        {
            bool bReturn = true;

            //콤마로 더하기 유지 안하는 특별한 커멘드         
            strReturnValue = String.Empty;

            string strCommandParam = sPar;
            //bool bCommaUsed = false;

            if (!String.IsNullOrEmpty(strCommandParam))
            {
                string[] strPars = new string[1];

                if (strCommandParam.Contains(','))
                {   // ',' 콤마가 사용되는 파라미터면

                    strPars = strCommandParam.Split(',');
                    //bCommaUsed = true;
                }
                else
                {
                    strPars[0] = strCommandParam;
                    //bCommaUsed = false;
                }

                for (int i = 0; i < strPars.Length; i++)
                {
                    if (i == 0)
                    {
                        sSendPacket = sSendPacket.Replace("<PORT>", strPars[0]);
                    }
                    else if (i == 1)
                    {
                        if (strCmdName.Contains("SET_AUDIO_OUTPUT_VOLTAGE_Vrms")) //rms 값을 dB 로 변환
                        {
                            double tempValue = 0.0;
                            double dBValue = 0.0;
                            double.TryParse(strPars[1], out tempValue);
                            //명령어 중 MEASURE_SNR_V 와 같이 _V 인 명령어는 Vrms 로 변환한다.
                            // Vrms = 10^(dBV/20)
                            // dBV = 20 log (Vrms)
                            dBValue = 20 * Math.Log10(tempValue);
                            strPars[1] = dBValue.ToString();
                        }
                        sSendPacket = sSendPacket.Replace("<DATA>", " " + strPars[1]);
                    }
                }
                strReturnValue = sSendPacket;
            }

            return bReturn;
        }

        //절차진행하는 과정에서 MAX값에 EXPR 또는 GMES 가 있는지 확인하여 포함하는 부분
        private bool CheckMaxValue(int iPort, int iNowJobNum, ref string strRtnMax)
        {

            //if (LstJOB_CMD[iNowJobNum].COMPARE.Equals("NONE"))
            if (LstJOB_CMD[iNowJobNum].COMPARE.Equals("NONE") || LstJOB_CMD[iNowJobNum].ACTION.Equals("SKIP"))
            {   //비교하는 명령이 아니면 돌려보내자.
                strRtnMax = LstJOB_CMD[iNowJobNum].MAX;
                return true;
            }

            if (LstJOB_CMD[iNowJobNum].MAX != null && LstJOB_CMD[iNowJobNum].MAX.Length > 0)
            {
                //EXPR 검사
                int iExpr = LstJOB_CMD[iNowJobNum].MAX.IndexOf(DEFINEEXPR);
                int iGmes = LstJOB_CMD[iNowJobNum].MAX.IndexOf(DEFINEGMES);
                int iDoc  = LstJOB_CMD[iNowJobNum].MAX.IndexOf(DEFINEDOCU);
                
                if (iExpr == 0)
                {
                    string tmpExprStr = LstJOB_CMD[iNowJobNum].MAX.Replace(DEFINEEXPR, "#LOAD:");
                    return DKExpr[iPort].ExcuteLoad(tmpExprStr, ref strRtnMax);
                    
                }

                if (iGmes == 0)
                {
                    string tmpGmesStr = LstJOB_CMD[iNowJobNum].MAX.Replace(DEFINEGMES, String.Empty);
                    //return DKGMES.GMES_GetInsp(iPort, tmpGmesStr, ref strRtnMax);
                    //CSMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                        return DKOSIMES.GetInsp(iPort, tmpGmesStr, ref strRtnMax);
                    else
                        return DKGMES.GMES_GetInsp(iPort, tmpGmesStr, ref strRtnMax);

                }

                if (iDoc == 0)
                {
                    string strDocMsg = String.Empty;
                    string tmpDocStr = LstJOB_CMD[iNowJobNum].MAX.Replace(DEFINEDOCU, String.Empty);
                    return GetDocumentItem(tmpDocStr, ref strRtnMax, ref strDocMsg);

                }

                strRtnMax = LstJOB_CMD[iNowJobNum].MAX;
                return true;
            }

            strRtnMax = String.Empty;
            return true;
        }

        //절차진행하는 과정에서 MIN값에 EXPR 또는 GMES 가 있는지 확인하여 포함하는 부분
        private bool CheckMinValue(int iPort, int iNowJobNum, ref string strRtnMin)
        {

            //if (LstJOB_CMD[iNowJobNum].COMPARE.Equals("NONE"))
            if (LstJOB_CMD[iNowJobNum].COMPARE.Equals("NONE") || LstJOB_CMD[iNowJobNum].ACTION.Equals("SKIP"))
            {   //비교하는 명령이 아니면 돌려보내자.
                strRtnMin = LstJOB_CMD[iNowJobNum].MIN;
                return true;
            }

            if (LstJOB_CMD[iNowJobNum].MIN != null && LstJOB_CMD[iNowJobNum].MIN.Length > 0)
            {
                //EXPR 검사
                int iExpr = LstJOB_CMD[iNowJobNum].MIN.IndexOf(DEFINEEXPR);
                int iGmes = LstJOB_CMD[iNowJobNum].MIN.IndexOf(DEFINEGMES);
                int iDoc  = LstJOB_CMD[iNowJobNum].MIN.IndexOf(DEFINEDOCU);

                if (iExpr == 0)
                {
                    string tmpExprStr = LstJOB_CMD[iNowJobNum].MIN.Replace(DEFINEEXPR, "#LOAD:");
                    return DKExpr[iPort].ExcuteLoad(tmpExprStr, ref strRtnMin);

                }

                if (iGmes == 0)
                {
                    string tmpGmesStr = LstJOB_CMD[iNowJobNum].MIN.Replace(DEFINEGMES, String.Empty);
                    //return DKGMES.GMES_GetInsp(iPort, tmpGmesStr, ref strRtnMin);
                    //CSMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                        return DKOSIMES.GetInsp(iPort, tmpGmesStr, ref strRtnMin);
                    else
                        return DKGMES.GMES_GetInsp(iPort, tmpGmesStr, ref strRtnMin);

                }

                if (iDoc == 0)
                {
                    string strDocMsg = String.Empty;
                    string tmpDocStr = LstJOB_CMD[iNowJobNum].MIN.Replace(DEFINEDOCU, String.Empty);
                    return GetDocumentItem(tmpDocStr, ref strRtnMin, ref strDocMsg);

                }

                strRtnMin = LstJOB_CMD[iNowJobNum].MIN;
                return true;
            }

            strRtnMin = String.Empty;
            return true;
        }

        //dictionary 에 값을 변경하는 함수.
        private void UpdateDictionary(int iDx, int i, int j, object objData)
        {
            lock (lockObjectDic)
            {
                try
                {
                    switch (iDx)
                    {
                        case (int)DICINDEX.SEQUENCE:
                            SEQUENCE_DIC[i][j] = (int)objData;
                            break;

                        case (int)DICINDEX.RESULTDT:
                            RESULTDT_DIC[i][j] = (string)objData;
                            break;

                        case (int)DICINDEX.RESPONSE:
                            RESPONSE_DIC[i][j] = (string)objData;
                            break;

                        case (int)DICINDEX.SENDPACK:
                            SENDPACK_DIC[i][j] = (string)objData;
                            break;

                        case (int)DICINDEX.ALL:
                            COMMDATA cData = (COMMDATA)objData;
                            SEQUENCE_DIC[cData.iPortNum][iNowJobNumber] = cData.iStatus;
                            RESULTDT_DIC[cData.iPortNum][iNowJobNumber] = cData.ResultData;
                            RESPONSE_DIC[cData.iPortNum][iNowJobNumber] = cData.ResponseData;
                            SENDPACK_DIC[cData.iPortNum][iNowJobNumber] = cData.SendPacket;
                            break;

                        default: return;

                    }
                }
                catch
                {

                }
            }
        }

#endregion

#region ACTOR 제어 관련

        public int InteractiveCommand(int iCOMSERIAL_ENUM, string strCmdName, string strParam, double dDelaySec, int iSendOption)
        {            
            string strSendpac = String.Empty;
            int iRes = (int)STATUS.NONE;

            switch (iCOMSERIAL_ENUM)
            {
                case (int)COMSERIAL.DIO:
                    if (FindPacDioVcpBench(strCmdName, ref strSendpac)){
                        iRes = DKACTOR.DirectSendRecvCmd(iCOMSERIAL_ENUM, strSendpac, 3, iSendOption, dDelaySec, (int)RS232.MOOHANTECH, strCmdName, strParam);
                        return iRes;
                    }
                    return (int)STATUS.NG;

                case (int)COMSERIAL.ODAPWR:
                    if (FindPacOdaPower(strCmdName, ref strSendpac)){
                        iRes = DKACTOR.DirectSendRecvCmd(iCOMSERIAL_ENUM, strSendpac, 3, iSendOption, dDelaySec, (int)RS232.TEXT, strCmdName, strParam);             
                        return iRes;
                    }
                    return (int)STATUS.NG;
                default:
                    return (int)STATUS.NG;
            }
        
            
        }

        public void IfPlcModeIsAllOffSignal()
        {
            //PLC 자동화 모드인경우 시작시 READY 신호를 보내야한다.
            if (GetPlcMode() && ActorCheckPort((int)COMSERIAL.DIO))
            {
                int iCmd = (int)STATUS.NONE;

                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RELAY_OFF_OK", "", 0, (int)MODE.SENDRECV);
                    if (iCmd == (int)STATUS.OK) break;
                }

                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RELAY_OFF_NG", "", 0, (int)MODE.SENDRECV);
                    if (iCmd == (int)STATUS.OK) break;
                }

                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RELAY_OFF_CHECK", "", 0, (int)MODE.SENDRECV);
                    if (iCmd == (int)STATUS.OK) break;
                }
            }
        }

        public void IfPlcModeIsReadySignal(bool bOnOff)
        {
            //PLC 자동화 모드인경우 시작시 READY 신호를 보내야한다.
            if (GetPlcMode() && ActorCheckPort((int)COMSERIAL.DIO))
            {                
                int iCmd = (int)STATUS.NONE;

                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    if (bOnOff)
                    {
                        iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RELAY_ON_READY", "", 0, (int)MODE.SENDRECV);
                    }
                    else
                    {
                        iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RELAY_OFF_READY", "", 0, (int)MODE.SENDRECV);
                    }
                    if (iCmd == (int)STATUS.OK) break;
                }              
            }
        }

        public void DioReset()
        {           
            if (ActorCheckPort((int)COMSERIAL.DIO))
            {
                int iCmd = (int)STATUS.NONE;
                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    iCmd = InteractiveCommand((int)COMSERIAL.DIO, "RESET", "", 0, (int)MODE.SENDRECV);                   
                    if (iCmd == (int)STATUS.OK) break;
                }
            }
        }

        public void ActorStop()
        {
            IfPlcModeIsReadySignal(false);
            StopStatusTimer();            
            if (OpenPCAN == (int)STATUS.OK)
            {
                DK_PCAN_USB.Release();
            }

            if (OpenVector == (int)STATUS.OK)
            {
                DKVECTOR.Disconnect();
            }

            if (OpenODAPWR)
            {
                InteractiveCommand((int)COMSERIAL.ODAPWR, "RESET", "", 0, (int)MODE.SEND); 
            }

            if (OpenKEITHLEY == (int)STATUS.OK)
            {
                if (DK_GPIB_KEITHLEY != null)
                {
                    COMMDATA resData = new COMMDATA();
                    DK_GPIB_KEITHLEY.SendRecv("*RST", "RESET", "", ref resData, false, 1.0);
                }
            }

            if (OpenMELSEC == (int)STATUS.OK) DK_MELSEC.Disconnect();

            for (int i = (int)COMSERIAL.DIO; i < (int)COMSERIAL.END; i++)
            {
                DKACTOR.CommOff(i);
            }
           
        }

        public void StopStatusTimer()
        {
            StatusTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            System.Threading.Thread.Sleep(50);
        }


        public bool ActorCheckPort(int iNumber)
        {
            return DKACTOR.isLive(iNumber);
        } 
        
#endregion

#region JOB & Config & TBL 관련

        private bool FindJobMapping(string strParam, ref string strFileName)
        {
            strFileName = String.Empty;

            for (int i = 0; i < LstTBL_JOBMAP.Count; i++)
            {
                if (LstTBL_JOBMAP[i][0].Equals(strParam))
                {
                    strFileName = LstTBL_JOBMAP[i][1];
                    return true;
                }
            }
            return false;
        }

        //OOB
        public bool FindOOBCommand(int iJobNumber, string strName)
        {          
            
            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
            {//1. 모든 슬롯수만큼 돌면서
                if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                {
                    RETCOUNT_DIC[i] = 0; // 3. 공통명령으로 한번만 처리한다.
                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.RUNNING);
                }
            }

            if (DKOOB == null)
            {
                ResetOOB();
            }

            string strResponseData = String.Empty;

            switch (strName)
            {    
                case "GET_DATA_FIELD_WORD":
                            
                            if (DKOOB.GetLabelFieldString(LstJOB_CMD[iNowJobNumber].PAR1, ref strResponseData))
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, String.Empty, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, String.Empty, true);
                            break;

                case "GET_DATA_FIELD_HEX":

                            if (DKOOB.GetLabelFieldHexa(LstJOB_CMD[iNowJobNumber].PAR1, ref strResponseData))
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, String.Empty, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, String.Empty, true);
                            break;

                case "LOADING_LABEL":

                            if (DKOOB.LoadingLabelData(strOOBLABEL, ref strResponseData))
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, strOOBLABEL, true); 
                            else
                                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, strOOBLABEL, true); 
                            break;
                case "LOADING_LABEL_FOR_GEN12":

                    if (DKOOB.LoadingLabelDataGEN12(strOOBLABEL, ref strResponseData))
                        GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, strOOBLABEL, true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, strOOBLABEL, true);
                    break;

                case "LOADING_LABEL_FOR_MCTM":

                            if (DKOOB.LoadingLabelDataMCTM(strOOBLABEL, ref strResponseData))
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, strOOBLABEL, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, strOOBLABEL, true);
                            break;
                case "GET_MCTM_LABEL_FIELD_WORD":

                            if (DKOOB.GetMctmFieldString(LstJOB_CMD[iNowJobNumber].PAR1, ref strResponseData))
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, String.Empty, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, String.Empty, true);
                            break;

                case "GET_MCTM_LABEL_FIELD_HEX":

                            if (DKOOB.GetMctmFieldHexa(LstJOB_CMD[iNowJobNumber].PAR1, ref strResponseData))
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, String.Empty, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, String.Empty, true);
                            break;

                default:
                            GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", strOOBLABEL, true); 
                            break;
            }
    
            return true;
        }

        private void GateWayMsgProcess(int iStatus, string strResponseData, string strResultData, string strParam, bool bLogging)
        {
            COMMDATA resData = new COMMDATA();

            resData.iStatus = iStatus;
            resData.iPortNum = (int)DEFINES.SET1;
            resData.ResponseData = strResponseData;
            resData.ResultData = strResultData;
            resData.SendPacket = LstJOB_CMD[iNowJobNumber].CMD;

            if (iStatus.Equals((int)STATUS.TIMESTAMP))
            {
                resData.SendPacket = strParam;
            }
            GateWay_MANAGER(resData);

            if (bLogging)
            {
                if (!iStatus.Equals((int)STATUS.DELAYLAPSE) && !iStatus.Equals((int)STATUS.TIMESTAMP))
                {
                    if (String.IsNullOrEmpty(strParam))
                        MessageLogging((int)LOGTYPE.RX, String.Empty, (int)DEFINES.SET1);
                    else
                        MessageLogging((int)LOGTYPE.RX, strParam + ":", (int)DEFINES.SET1);
                }
            }            
        }

        //BAUDRATE LIST 체크
        private bool CheckPathFormat(string strPath)
        {
            if (String.IsNullOrEmpty(strPath)) return false;
            if (strPath.Length < 5) return false;
            if (strPath.Contains(".")) return false;
            if (!strPath.Contains(":")) return false;
            if (!strPath.Contains(@"\")) return false;
            if (!strPath.Contains(@"\")) return false;
            if (!strPath.LastIndexOf(@"\").Equals(strPath.Length - 1)) return false;
            return true;
        }

        //BAUDRATE LIST 체크
        private bool CheckBaudRate(string strBaudRate)
        {
            switch (strBaudRate)
            {
                case "4800":
                case "9600":
                case "19200":
                case "38400":
                case "57600":
                case "115200": 
                case "230400":
                case "460800":
                case "921600":
                    return true;
                default: return false;
            }
        }

        //SET COMPORT 변경
        private bool ChangeComPort(string strPortName)
        {            
            try
            {
                return DKACTOR.ChangeComPort(strPortName, (int)COMSERIAL.SET);
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);                
            }
            return false;
        }

        //SET BAUDRATE 변경
        private bool ChangeUartBaudrate(string strBaudRate)
        {
            int iBaudrate = 0;
            try
            {
                iBaudrate = int.Parse(strBaudRate);
                DKACTOR.ChangeBaudrate(iBaudrate, (int)COMSERIAL.SET);
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
                return false;
            }
            return true;
        }

        //TC3000 BAUDRATE 변경
        private bool ChangeTC3000Baudrate(string strBaudRate)
        {
            int iBaudrate = 0;
            try
            {
                iBaudrate = int.Parse(strBaudRate);
                DKACTOR.ChangeBaudrate(iBaudrate, (int)COMSERIAL.TC3000);
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
                return false;
            }
            return true;
        }

        //DELAY
        public bool  FindPageCommand(int iPortNum, int iJobNumber, string strName)
        {
            string strOKreason = String.Empty;

            switch (strName)
            {                
                case "NETWORK_PING_TTL":
                case "NETWORK_PING_TIME":
                        double dTime = 0;
                        
                        try
                        {
                            dTime = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                            if (dTime <= 0.1) dTime = 0.1;
                        }
                        catch { dTime = 0.0; }

                        DelayChecker(dTime, false);
                
                        break;

                default:
                    for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                    {//1. 모든 슬롯수만큼 돌면서
                        if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                        {
                            RETCOUNT_DIC[i] = 0; // 3. 공통명령으로 한번만 처리한다.
                            UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.RUNNING);
                        }
                    }
                    break;
            }

            bool btmp = false;

            string strResponse = String.Empty;
            string strResult = String.Empty;
            string strSendparEx = String.Empty;

            switch (strName)
            {

                case "UART2_RTS_XONOFF":
                    int iOption = 2;
                    //case 0: ComSerial.Handshake = Handshake.None; break;
                    //case 1: ComSerial.Handshake = Handshake.RequestToSend; break;
                    //case 2: ComSerial.Handshake = Handshake.RequestToSendXOnXOff; break;
                    //case 3: ComSerial.Handshake = Handshake.XOnXOff; break;
              
               
                    if (DKACTOR.CommHandShake((int)COMSERIAL.UART2,iOption))
                        GateWayMsgProcess((int)STATUS.OK, "SUCCESS", "SUCCESS", String.Empty, true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, "FAIL", "FAIL", String.Empty, true);
                        
                    return true;
                    

                case "OPEN_UART2":
                    try
                    {
                        if (DKACTOR.CommOpen((int)COMSERIAL.UART2, "COM" + LstJOB_CMD[iJobNumber].PAR1, 9600))
                            GateWayMsgProcess((int)STATUS.OK, "SUCCESS : " + LstJOB_CMD[iJobNumber].PAR1, "SUCCESS : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, "PORT ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PORT ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                    }
                    catch 
                    {
                        GateWayMsgProcess((int)STATUS.NG, "PORT ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PORT ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                    }
                    
                    return true;

                case "CLOSE_UART2":
                    try
                    {
                        DKACTOR.CommOff((int)COMSERIAL.UART2);
                        GateWayMsgProcess((int)STATUS.OK, "CLOSED", "CLOSED", String.Empty, true);                        
                    }
                    catch
                    {
                        GateWayMsgProcess((int)STATUS.NG, "PORT ERROR", "PORT ERROR", String.Empty, true);
                    }

                    return true;

                case "NETWORK_PING_TTL":
                case "NETWORK_PING_TIME":
                    string strAddress = LstJOB_CMD[iJobNumber].PAR1;                    
                    //string strResult = String.Empty;
                    int iTimeOut = 3;

                    try
                    {
                        iTimeOut = int.Parse(LstJOB_CMD[iJobNumber].TIMEOUT);
                        if (iTimeOut < 1) iTimeOut = 1;
                    }
                    catch 
                    {
                        iTimeOut = 2;
                    }

                    if(!String.IsNullOrEmpty(strAddress))
                    {
                        int iTTL = 0;
                        long lTime = 0;
                        MessageLogging((int)LOGTYPE.TX, "ping " + strAddress, iPortNum);
                        bool bReturn = DKLoggerPC.NetworkPingTest(strAddress, iTimeOut, ref strResult, ref iTTL, ref lTime);
                        
                        if (bReturn)
                        {
                            switch (strName)
                            {
                                case "NETWORK_PING_TTL":  GateWayMsgProcess((int)STATUS.OK, iTTL.ToString(), iTTL.ToString(), String.Empty, false); break;
                                case "NETWORK_PING_TIME": GateWayMsgProcess((int)STATUS.OK, lTime.ToString(), lTime.ToString(), String.Empty, false); break;
                            }
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, strResult, strResult, String.Empty, true);
                        }
                        MessageLogging((int)LOGTYPE.RX, strResult, iPortNum);
                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                    }
                    return true;


                case "PATH_ATCO_SYSTEM":
                    string strAtcoPath = LstJOB_CMD[iJobNumber].PAR1;
                    RETCOUNT_DIC[iPortNum] = 0;
                    STEPMANAGER_VALUE.strAtcoLoggingPath = String.Empty;

                    if (!CheckPathFormat(strAtcoPath))
                    {
                        GateWayMsgProcess((int)STATUS.NG, "PATH ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PATH ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.OK, LstJOB_CMD[iJobNumber].PAR1, LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                        STEPMANAGER_VALUE.strAtcoLoggingPath = strAtcoPath;
                    }
                    return true;

                case "LOAD_MFG_FILE":

                    //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                    string strPrimaryKey = LstJOB_CMD[iJobNumber].PAR1; //IMEI 로 찾는다.

                    if (!CheckExprParam(iPortNum, iJobNumber, ref strPrimaryKey))
                    {
                        RETCOUNT_DIC[iPortNum] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                        return true;
                    }


                    string strMfgFilePath = "C:\\gen10db.txt" ;
                    

                    RETCOUNT_DIC[iPortNum] = 0;

                    if (!String.IsNullOrEmpty(strMfgFilePath))
                    {
                        List<string[]> LstGen10MFG = new List<string[]>();
                        bool bGetFile = DKLoggerPC.GetLocalMfgFile(strMfgFilePath, ref LstGen10MFG);
                        if (!bGetFile)
                        {
                            GateWayMsgProcess((int)STATUS.NG, "FILE ERROR : " + strMfgFilePath, "FILE ERROR : " + strMfgFilePath, String.Empty, true);
                        }
                        else
                        {
                            //expr 에 등록하기;
                            bool bFind = false;
                            string[] strtmpMFG = new string[1];

                            for (int i = 0; i < LstGen10MFG.Count; i++)
                            {
                                strtmpMFG = LstGen10MFG[i];

                                if (strtmpMFG[(int)GEN10MFGINDEX.IMEI].Equals(strPrimaryKey))
                                {
                                    bFind = true;
                                    //IMEI, IMSI, STID, HASH, TRACE, DUMMY, OOB_TEST, MODEL, ICCID
                                    ExprSaveData((int)DEFINES.SET1, "MFG_IMEI", strtmpMFG[(int)GEN10MFGINDEX.IMEI]);
                                    ExprSaveData((int)DEFINES.SET1, "MFG_IMSI", strtmpMFG[(int)GEN10MFGINDEX.IMSI]);
                                    ExprSaveData((int)DEFINES.SET1, "MFG_STID", strtmpMFG[(int)GEN10MFGINDEX.STID]);
                                    ExprSaveData((int)DEFINES.SET1, "MFG_HASH", strtmpMFG[(int)GEN10MFGINDEX.HASH]);
                                    ExprSaveData((int)DEFINES.SET1, "MFG_TRACE", strtmpMFG[(int)GEN10MFGINDEX.TRACE]);
                                    ExprSaveData((int)DEFINES.SET1, "MFG_MODEL", strtmpMFG[(int)GEN10MFGINDEX.MODEL]);
                                    ExprSaveData((int)DEFINES.SET1, "MFG_ICCID", strtmpMFG[(int)GEN10MFGINDEX.ICCID]);
                                    break;
                                }
                            }

                            if(bFind)
                                GateWayMsgProcess((int)STATUS.OK, strPrimaryKey, strPrimaryKey, String.Empty, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, "NOT FOUND IMEI ON MFGDATA :" + strPrimaryKey, "NOT FOUND IMEI ON MFGDATA :" + strPrimaryKey, String.Empty, true);
                            
                        }
                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.NG, "FILE NOT FOUND : " + strMfgFilePath, "FILE NOT FOUND : " + strMfgFilePath, String.Empty, true);

                        
                    }
                    return true;

                case "DELAY":
                    string strDtime = LstJOB_CMD[iJobNumber].DELAY;
                    double dTime = 1.0;
                    
                    if ((strDtime.Length < 1 ) || (strDtime.Equals("0"))){
                        dTime = 0.0;                        
                    }
                    else
                    {
                        try
                        {
                            dTime = double.Parse(strDtime);
                            if (dTime <= 0.1) dTime = 0.1;
                        }
                        catch { dTime = 0.1; }

                        DelayChecker(dTime, true);

                        for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                        {
                            if (UseSlots[i])
                            {                                
                                UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.OK);
                            }
                        }

                    }
                    GateWayMsgProcess((int)STATUS.OK, "OK", dTime.ToString("0.0"), String.Empty, true);
                    return true;

                case "BOOT_DELAY": // 첫검사시는 0초 다시탈때만 파라미터 반영. CheckExtPwr() 함수로 파라미터 가져오자.
                    string dTempTime = LstJOB_CMD[iJobNumber].DELAY;
                    double dBootLapse = 0;
                    if (!CheckExtPwr())
                    {  //Extpwr 모드가 아니면  딜레이 없이 진행
                        GateWayMsgProcess((int)STATUS.CHECK, "ONLY EXTERNAL POWER MODE", "ONLY EXTERNAL POWER MODE", String.Empty, true);
                        return true;
                    }
                    else
                    {
                        if (dExtPwrBootTime < 0)
                        {                            
                            GateWayMsgProcess((int)STATUS.OK, "OK", "0.0", String.Empty, true);
                            dExtPwrBootTime = 0;
                            return true;
                        }
                        else
                        {
                            try
                            {
                                dExtPwrBootTime = double.Parse(dTempTime);
                                if (dExtPwrBootTime <= 0) dExtPwrBootTime = 0;
                            }
                            catch { dExtPwrBootTime = 0; }

                            DelayChecker(dExtPwrBootTime, true);

                            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                            {
                                if (UseSlots[i])
                                {                                    
                                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.OK);
                                }
                            }

                            GateWayMsgProcess((int)STATUS.OK, "OK", dBootLapse.ToString("0.0"), String.Empty, true);
                            return true;      
                        }                               
                    }


                case "JUMP_CHECK":
                    strDtime = LstJOB_CMD[iJobNumber].DELAY;
                    dTime = 1.0;

                    if ((strDtime.Length < 1) || (strDtime.Equals("0")))
                    {
                        dTime = 0.0;
                    }
                    else
                    {
                        try
                        {
                            dTime = double.Parse(strDtime);
                            if (dTime <= 0.1) dTime = 0.1;
                        }
                        catch { dTime = 0.1; }

                        DelayChecker(dTime, true);

                        //for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                        //{
                        //    if (UseSlots[i])
                        //    {
                        //        UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.OK);
                        //    }
                        //}
                    }
                    string strtmpParam11 = LstJOB_CMD[iJobNumber].PAR1;
                    if (!CheckExprParam(iPortNum, iJobNumber, ref strtmpParam11))
                    {
                        RETCOUNT_DIC[iPortNum] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                        return true;
                    }

                    GateWayMsgProcess((int)STATUS.OK, "OK", strtmpParam11, String.Empty, true);
                    return true;

                case "CHANGE_SET_COMPORT":
                    //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                    string strPortName = LstJOB_CMD[iJobNumber].PAR1;
                    RETCOUNT_DIC[iPortNum] = 0;
                    if (String.IsNullOrEmpty(strPortName) || !strPortName.Contains("COM"))
                    {
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                    }
                    else
                    {
                        if (ChangeComPort(strPortName))
                        {
                            GateWayMsgProcess((int)STATUS.OK, "Change COM Port : " + LstJOB_CMD[iJobNumber].PAR1, strPortName, String.Empty, true);
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                        }
                    }
                    return true;
                case "CHANGE_SET_BAUDRATE":
                    //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                    string strBaudRate = LstJOB_CMD[iJobNumber].PAR1;
                    RETCOUNT_DIC[iPortNum] = 0;
                    if (String.IsNullOrEmpty(strBaudRate) || !CheckBaudRate(strBaudRate))
                    {
                        GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);                       
                    }
                    else
                    {
                        if (ChangeUartBaudrate(strBaudRate))
                        {
                            GateWayMsgProcess((int)STATUS.OK, "Change Baud Rate:" + LstJOB_CMD[iJobNumber].PAR1, strBaudRate, String.Empty, true);                           
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);                           
                        }
                    }
                    return true;

                case "CHANGE_JOB":
                    //여기서 JOB파일 오토맵핑 로직
                    //1. AUTO MAP ON 이 되어있으면 mapping 시도 아니면 skip 으로 진행
                    string strFileName = String.Empty;

                    if (STEPMANAGER_VALUE.bUseMesOn || STEPMANAGER_VALUE.bUseAutoJobOn) //STEPMANAGER_VALUE.bUseMesOn 추가. 이동성책임/이병권선임 요청
                    {
                        if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                        {
                            strOKreason = "CHECK PAR1";                            
                            GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                            return true;
                        }
                        else
                        {
                            //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                            string strtmpParam = LstJOB_CMD[iJobNumber].PAR1;
                            if (!CheckExprParam(iPortNum, iJobNumber, ref strtmpParam))
                            {
                                RETCOUNT_DIC[iPortNum] = 0;
                                GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                                return true;
                            }

                            if (FindJobMapping(strtmpParam, ref strFileName))
                            {
                                if (strNowJobName.Equals(strFileName))
                                {
                                    strOKreason = "File Already Changed.";
                                    GateWayMsgProcess((int)STATUS.OK, strOKreason, strOKreason, String.Empty, true);
                                }
                                else
                                {
                                    if (DKLoggerPC.IsExistFile(DKLoggerPC.Item_DataFolder + strFileName))
                                    {
                                        strOKreason = strFileName;                                        
                                        GateWayMsgProcess((int)STATUS.CHANGEJOB, strOKreason, strOKreason, String.Empty, true);
                                    }
                                    else
                                    {
                                        strOKreason = "NOT FOUND JOBFILE:" + strFileName;                                        
                                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                                    }
                                }
                            }
                            else
                            {
                                strOKreason = "NOT FOUND MAPDATA:" + strtmpParam;                                
                                GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                            }                            
                        }                        
                    }
                    else
                    {
                        strOKreason = "AUTO MAPPING OFF.";
                        GateWayMsgProcess((int)STATUS.OK, strOKreason, strOKreason, String.Empty, true);
                    }
                   
                    System.Threading.Thread.Sleep(10);
                    Application.DoEvents();
                    return true;

                case "MESSAGE_POPUP":

                    string strMsg = LstJOB_CMD[iJobNumber].PAR1;
                    string strTitle = LstJOB_CMD[iJobNumber].DISPLAYNAME;
                    if (strTitle.Length < 1) strTitle = LstJOB_CMD[iJobNumber].CMD;
                    RESDATA tmpRES = new RESDATA();
                    tmpRES.iType = (int)EVENTTYPE.MANAGER;
                    tmpRES.iPortNum = 0;
                    tmpRES.iSequenceNum = 0;
                    tmpRES.iStatus = (int)STATUS.POPPING;
                    btmp = ManagerSendReport(tmpRES);
                    Item_bThisPop = true;
                    int iStatusValue = tmpDKPage.MsgPopUp(strTitle, strMsg, (int)POPBTNTYPE.OKNG);
                    Item_bThisPop = false;
                    tmpRES.iStatus = (int)STATUS.POPPINGOFF;
                    btmp = ManagerSendReport(tmpRES);
                    GateWayMsgProcess(iStatusValue, "", "", strName, true); 
                    return true;

                case "TIMER_START":
                    if (swTimer.IsRunning) swTimer.Restart();
                    else
                    {
                        swTimer.Reset();
                        swTimer.Start();
                    }
                    string strStartTime = DateTime.Now.ToString("HH:mm:ss");
                    GateWayMsgProcess((int)STATUS.OK, strStartTime, strStartTime, String.Empty, true);
                    return true;
                case "TIMER_STOP":
                    swTimer.Stop();
                    string strStopTime = DateTime.Now.ToString("HH:mm:ss");
                    GateWayMsgProcess((int)STATUS.OK, strStopTime, strStopTime, String.Empty, true);
                    return true;
                case "TIMER_VALUE":
                    string strElapsed = swTimer.Elapsed.TotalSeconds.ToString("0.#"); ;
                    GateWayMsgProcess((int)STATUS.OK, strElapsed, strElapsed, String.Empty, true);
                    return true;
                case "EXPR_DATA_VIEW":
                    string tmpExprStr = "#LOAD:" + LstJOB_CMD[iJobNumber].PAR1;
                    string tmpReturnStr = String.Empty;
                    if (DKExpr[iPortNum].ExcuteLoad(tmpExprStr, ref tmpReturnStr))
                        GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), tmpReturnStr, LstJOB_CMD[iJobNumber].PAR1, true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO DATA", LstJOB_CMD[iJobNumber].PAR1, true);
                    return true;

                case "MAKE_PARAMETERS":

                    string strMakeText = String.Empty;
                    if (CheckExprParam(iPortNum, iJobNumber, ref strMakeText))
                    {
                        //여기선 콤마를 삭제한다.
                        strMakeText = strMakeText.Replace(",", "");
                        GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), strMakeText, LstJOB_CMD[iJobNumber].PAR1, true);
                    }                        
                    else
                        GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "NO DATA", LstJOB_CMD[iJobNumber].PAR1, true);
                    return true;

                    

                case "SHIFT_TIME":
                    //DateTime dtm = DateTime.Now;
                    
                    string strShiftMin = String.Empty;
                    string strShiftMax = String.Empty;

                    if (!CheckMinValue(iPortNum, iJobNumber, ref strShiftMin))
                    {
                        RETCOUNT_DIC[iPortNum] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "MIN SPEC ERROR : " + LstJOB_CMD[iJobNumber].MIN, "MIN SPEC ERROR : " + LstJOB_CMD[iJobNumber].MIN, String.Empty, true);
                        return true;
                    }

                    if (!CheckMaxValue(iPortNum, iJobNumber, ref strShiftMax))
                    {
                        RETCOUNT_DIC[iPortNum] = 0;
                        GateWayMsgProcess((int)STATUS.NG, "MAX SPEC ERROR : " + LstJOB_CMD[iJobNumber].MAX, "MAX SPEC ERROR : " + LstJOB_CMD[iJobNumber].MAX, String.Empty, true);
                        return true;
                    }

                    if (String.IsNullOrEmpty(strShiftMin) || String.IsNullOrEmpty(strShiftMax))
                    {
                        GateWayMsgProcess((int)STATUS.NG, "MIN/MAX ERROR", "MIN/MAX ERROR", "", true);
                        return true;
                    }
                    string strNowTime = String.Empty;

                    while (CheckTimeStamp(strShiftMin, strShiftMax, ref strNowTime))
                    {
                        if (!Item_bTestStarted && !STEPMANAGER_VALUE.bInteractiveMode)
                        {
                            return true;
                        }
                        System.Threading.Thread.Sleep(300);
                        GateWayMsgProcess((int)STATUS.TIMESTAMP, strShiftMin, strNowTime, strShiftMax, true);
                    }
                    GateWayMsgProcess((int)STATUS.OK, strNowTime, strNowTime, "", true);

                    return true;

                case "CHECK_FILE_SIZE":
                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK PAR1";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                        return true;
                    }
                    else
                    {
                        string strFileSize = String.Empty;
                        if (!GetFileSize(LstJOB_CMD[iJobNumber].PAR1, ref strFileSize))
                        {
                            GateWayMsgProcess((int)STATUS.NG, "FILE NOT FOUND", "FILE NOT FOUND", String.Empty, true);
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.OK, strFileSize, strFileSize, "", true);
                        }
                        
                    }
                    return true;

                case "DOCUMENT_FILE_LINK":
                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK FILE NAME";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                        return true;
                    }
                    else
                    {
                        if (!DocumentFileLinkFunc(LstJOB_CMD[iJobNumber].PAR1, ref strOKreason))
                        {
                            GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, LstJOB_CMD[iJobNumber].PAR1, true);
                        }
                        else
                        {
                            if(CheckDoucumentColumn(ref strOKreason))
                                GateWayMsgProcess((int)STATUS.OK, strOKreason, strOKreason, LstJOB_CMD[iJobNumber].PAR1, true);
                            else
                                GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, LstJOB_CMD[iJobNumber].PAR1, true);
                        }
                        
                    }
                    return true;
                case "DOCUMENT_ITEM_VIEW":
                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK ITEM NAME";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                        return true;
                    }
                    else
                    {
                        string strValue = String.Empty;

                        if (!GetDocumentItem(LstJOB_CMD[iJobNumber].PAR1, ref strValue, ref strOKreason))
                        {
                            GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, LstJOB_CMD[iJobNumber].PAR1, true);
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.OK, strValue, strValue, LstJOB_CMD[iJobNumber].PAR1, true);
                        }
                        
                    }
                    return true;

                case "CHECK_TCP_SEED_FILE":
                    //STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath = @"D:\seed\vcp\seed-key_20190816155933561.dat";

                    bool bTcpRes = false;
                    bTcpRes = PepuFunction((int)PEPUCMD.TCP_CHECK, "", ref strOKreason);

                    if (bTcpRes)
                        GateWayMsgProcess((int)STATUS.OK, strOKreason, strOKreason, "", true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, "", true);

                    return true;

                case "CHECK_VCP_SEED_FILE":
                    //STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath = @"D:\seed\vcp\seed-key_20190816155933561.dat";

                    bool bVcpRes = false;
                    bVcpRes = PepuFunction((int)PEPUCMD.VCP_CHECK, "", ref strOKreason);

                    if (bVcpRes)
                        GateWayMsgProcess((int)STATUS.OK, strOKreason, strOKreason, "", true);
                    else
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, "", true);

                    return true;

                case "CHECK_VCP_SEED_FILE_MANUAL":
                case "CHECK_TCP_SEED_FILE_MANUAL":

                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK PAR1";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);                        
                    }
                    else
                    {
                        bool bManRes = false;                        
                        switch (strName)
                        {
                            case "CHECK_VCP_SEED_FILE_MANUAL": bManRes = PepuFunction((int)PEPUCMD.VCP_MANUAL, LstJOB_CMD[iJobNumber].PAR1, ref strOKreason); 
                                break;
                            case "CHECK_TCP_SEED_FILE_MANUAL": bManRes = PepuFunction((int)PEPUCMD.TCP_MANUAL, LstJOB_CMD[iJobNumber].PAR1, ref strOKreason); 
                                break;

                            default:
                                strOKreason = "COMMAND ERROR";
                                GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                                break;
                        }

                        if (bManRes)
                            GateWayMsgProcess((int)STATUS.OK, strOKreason, strOKreason, "", true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, "", true);

                    }
                    return true;

                case "GET_PEPU_PASSWORD":
                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK SERIAL NUMBER";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                        return true;
                    }
                    else
                    {
                        DK_PEPUDLL dkPepuDll = new DK_PEPUDLL();
                        string strReason = String.Empty;
                        string strPassword = String.Empty;
                        string strSNParam = LstJOB_CMD[iJobNumber].PAR1;

                        if (!CheckExprParam(iPortNum, iJobNumber, ref strSNParam))
                        {
                            RETCOUNT_DIC[iPortNum] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        bool bGetPassword = dkPepuDll.GetPassword(strSNParam, ref strPassword, ref strReason);

                        if (!bGetPassword)
                        {
                            GateWayMsgProcess((int)STATUS.NG, strReason, strReason, strSNParam, true);
                        }
                        else
                        {
                            GateWayMsgProcess((int)STATUS.OK, strPassword, strPassword, strSNParam, true);
                        }                        
                    }
                    return true;

                case "GET_GEN12_CERT_MANUAL_ASCII":
                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK PAR1";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                    }
                    else
                    {
                        //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                        string strtmpParam = LstJOB_CMD[iJobNumber].PAR1;
                        if (!CheckExprParam(iPortNum, iJobNumber, ref strtmpParam))
                        {
                            RETCOUNT_DIC[iPortNum] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        if (strtmpParam.Contains(','))
                            strtmpParam = strtmpParam.Replace(",", "");

                        bool bFileRes = false;
                        string sRealData = string.Empty;
                        string strReason = string.Empty;                        
                        bFileRes = GetGen12CertFunction(strtmpParam, ref sRealData, ref strReason);

                        if (bFileRes)
                            GateWayMsgProcess((int)STATUS.OK, sRealData, sRealData, "", true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, strReason, strReason, "", true);
                    }
                    return true;

                case "GET_GEN12_CERT_MANUAL_HEX":
                    if (String.IsNullOrEmpty(LstJOB_CMD[iJobNumber].PAR1))
                    {
                        strOKreason = "CHECK PAR1";
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                    }
                    else
                    {
                        //파라미터 검사에서 잘못된 파라미터가 있으면 해당 명령은 실패로 처리한다.
                        string strtmpParam = LstJOB_CMD[iJobNumber].PAR1;
                        if (!CheckExprParam(iPortNum, iJobNumber, ref strtmpParam))
                        {
                            RETCOUNT_DIC[iPortNum] = 0;
                            GateWayMsgProcess((int)STATUS.NG, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                            return true;
                        }

                        if (strtmpParam.Contains(','))
                            strtmpParam = strtmpParam.Replace(",", "");

                        bool bFileRes = false;
                        string sRealData = string.Empty;
                        string strReason = string.Empty;                        
                        bFileRes = GetGen12CertFunction_HEX(LstJOB_CMD[iJobNumber].PAR1, ref sRealData, ref strReason);

                        if (bFileRes)
                            GateWayMsgProcess((int)STATUS.OK, sRealData, sRealData, "", true);
                        else
                            GateWayMsgProcess((int)STATUS.NG, sRealData, sRealData, "", true);
                    }
                    return true;
                case "DATA_PARSING_ASCII2HEX":
                    strResponse = string.Empty;

                    if (!CheckExprParam(iPortNum, iJobNumber, ref strResponse))
                    {
                        strOKreason = "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1;
                        GateWayMsgProcess((int)STATUS.NG, strOKreason, strOKreason, String.Empty, true);
                        return true;
                    }

                    strResponse = BitConverter.ToString(Encoding.UTF8.GetBytes(strResponse)).Replace("-", "");
                    GateWayMsgProcess((int)STATUS.OK, STATUS.OK.ToString(), strResponse, LstJOB_CMD[iJobNumber].PAR1, true);
                    return true;

                //LGEVH 
                case "JOB_MODEL_CHECK":
                    //JOB FILE 명 가져오기.
                    string strSelectedJobfile = LoadINI("OPTION", "LASTFILE");
                    GateWayMsgProcess((int)STATUS.OK, strSelectedJobfile, strSelectedJobfile, String.Empty, true);
                    return true;

                case "SAVE_WIPID": //여기 확인하자.

                    if (!CheckExprParam(iPortNum, iJobNumber, ref strSendparEx))
                    {
                        GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);
                        return true;
                    }

                    if (Item_bUseBarcode)
                    {
                        if (Item_WIPID.Length == Item_iWIPSIZE) //바코드 스캐너로 읽었다면
                        {
                            strResponse = Item_WIPID = strSendparEx; strResult = STATUS.OK.ToString();

                            GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                            return true;
                        }
                        else
                        {
                            if (strSendparEx != null)// && strSendparEx.Length == Item_iWIPSIZE)
                            {
                                Item_WIPID = strSendparEx;
                                strResponse = Item_WIPID; strResult = STATUS.OK.ToString();
                                GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (strSendparEx != null)
                        {
                            Item_WIPID = strSendparEx;
                            strResponse = strResult = Item_WIPID;
                            GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                            return true;
                        }
                    }

                    strResponse = "NO BARCODE"; strResult = "NO BARCODE";
                    GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                    return true;

                default:
                    strOKreason = "UNKNOWN COMMAND";
                    GateWayMsgProcess((int)STATUS.CHECK, strOKreason, strOKreason, String.Empty, true);                       
                    return true;   
            }
        }
             

        private bool GetGen12CertFunction(string strParam, ref string sRealData, ref string strReason)
        {
            strReason = String.Empty;
            sRealData = string.Empty;
            string strProgramPath = AppDomain.CurrentDomain.BaseDirectory + "GEN12_CERT\\" + strParam;

            if (!DKLoggerPC.IsExistFile(strProgramPath))
            {
                strReason = "FILE NOT FOUND";
                return false;
            }

            byte[] bData = DKLoggerMR.FileToBinary(strProgramPath);
            if(bData.Length < 1)
            {
                strReason = "THERE IS NO DATA";
                return false;
            }
            strReason = "SUCCESS";
            sRealData = Encoding.UTF8.GetString(bData);

            return true;
        }

        private bool GetGen12CertFunction_HEX(string strParam, ref string sRealData, ref string strReason)
        {
            strReason = String.Empty;
            sRealData = string.Empty;
            string strProgramPath = AppDomain.CurrentDomain.BaseDirectory + "GEN12_CERT\\" + strParam;

            if (!DKLoggerPC.IsExistFile(strProgramPath))
            {
                strReason = "FILE NOT FOUND";
                return false;
            }

            byte[] bData = DKLoggerMR.FileToBinary(strProgramPath);
            if (bData.Length < 1)
            {
                strReason = "THERE IS NO DATA";
                return false;
            }
            strReason = "SUCCESS";

            sRealData = BitConverter.ToString(bData).Replace("-", " "); 

            return true;
        }

        private bool PepuFunction(int iCommandType, string strManualPath, ref string strReason)
        {
            DK_PEPUDLL dkPepuDll = new DK_PEPUDLL();            
            strReason = String.Empty;

            switch (iCommandType)
            {

                case (int)PEPUCMD.TCP_CHECK:
                case (int)PEPUCMD.VCP_CHECK:
                    if (String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath))
                    {
                        strReason = "FILE NOT FOUND";
                        return false;
                    }
                    break;

                case (int)PEPUCMD.VCP_MANUAL:                   
                case (int)PEPUCMD.TCP_MANUAL:
                    if (String.IsNullOrEmpty(strManualPath))
                    {
                        strReason = "FILE NOT FOUND";
                        return false;
                    }
                    break;
                default:
                    strReason = "FILE NOT FOUND";
                    return false;
            }

            bool bCommand = false;
            
            switch(iCommandType)
            {

                case (int)PEPUCMD.TCP_CHECK:

                    bCommand = dkPepuDll.CheckSeedFile("TCP", STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath, ref strReason);
                    break;

                case (int)PEPUCMD.VCP_CHECK:
                    bCommand = dkPepuDll.CheckSeedFile("VCP", STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath, ref strReason);
                    break;

                case (int)PEPUCMD.VCP_MANUAL:
                    bCommand = dkPepuDll.CheckSeedFile("VCP", strManualPath, ref strReason);
                    break;

                case (int)PEPUCMD.TCP_MANUAL:
                    bCommand = dkPepuDll.CheckSeedFile("TCP", strManualPath, ref strReason);
                    break;


                default: 
                    strReason = "FILE NOT FOUND";
                    return false;
            }
            return bCommand;
        }

        private bool DocumentFileLinkFunc(string strFileName, ref string strReason)
        {            
            DK_CLOSEDXML dkClsxml = new DK_CLOSEDXML();
                                            
            strReason = String.Empty;
            LstDoc.Clear();
            
            bool bResult = dkClsxml.GetInspectionDocuments(strFileName, ref LstDoc, ref strReason);

            dkClsxml = null;

            return bResult;
          

            /*
            DK_DOCUMENT dkDocument = new DK_DOCUMENT();
            
            strReason = String.Empty;
            LstDoc.Clear();
            return dkDocument.GetInspectionDocuments(strFileName, ref LstDoc, ref strReason);
            */
        }

        private bool GetDocumentItem(string strItemName, ref string strValue, ref string strReason)
        {
            if (String.IsNullOrEmpty(strItemName))
            {
                strReason = "Check Item Name";
                return false;
            }

            InspDoc tmpDoc = new InspDoc();

            for (int i = 0; i < LstDoc.Count; i++)
            {
                tmpDoc = LstDoc[i];
                if (tmpDoc.SpecItem.Equals(strItemName))
                {
                    strValue = tmpDoc.Contents;
                    strReason = "OK";
                    return true;
                }
            }

            strReason = "NOT FOUND DATA";
            return false;
            
        }

        private bool CheckDoucumentColumn(ref string strReason)
        {
            if (LstDoc.Count == 0)
            {
                strReason = "Empty Document File";
                return false;
            }

            List<string> LstDocumentsCols = new List<string>();

            for (int i = 0; i < LstJOB_CMD.Count; i++)
            {
                if ((LstJOB_CMD[i].ACTION.Equals("RUN") || LstJOB_CMD[i].ACTION.Equals("ENC"))&& !String.IsNullOrEmpty(LstJOB_CMD[i].DOC))
                {
                    if(!LstDocumentsCols.Contains(LstJOB_CMD[i].DOC))
                    {
                        LstDocumentsCols.Add(LstJOB_CMD[i].DOC);
                    }
                    else
                    {
                        strReason = "(Job File)Duplicate Item:" + LstJOB_CMD[i].DOC;
                        return false;
                    }
                }
            }
         
            if (LstDocumentsCols.Count != LstDoc.Count)
            {
                List<string> LstTmpItems = new List<string>();
                string strTmpLists = String.Empty;

                for(int i = 0; i < LstDoc.Count; i++)
                {                
                    LstTmpItems.Add(LstDoc[i].SpecItem);
                }

                if (LstDocumentsCols.Count > LstDoc.Count) //절차서에 항목이 더 많은 경우(즉, 엑셀문서의 내용이 누락된경우)
                {
                    LstDocumentsCols = LstDocumentsCols.Except(LstTmpItems).ToList();                    
                    
                    for (int i = 0; i < LstDocumentsCols.Count; i++)
                    {
                        strTmpLists += LstDocumentsCols[i] + ",";
                        if (i > 0)
                        {
                            strTmpLists += "...";
                            break;
                        }
                    }
                    strReason = "(Excel File) Missing Item:" + strTmpLists;
                }
                else
                {   //엑셀문서에 항목이 더 많은 경우(즉, 절차서에 내용이 누락된경우)
                    LstTmpItems = LstTmpItems.Except(LstDocumentsCols).ToList();   
                 
                    for (int i = 0; i < LstTmpItems.Count; i++)
                    {
                        strTmpLists += LstTmpItems[i] + ",";
                        if (i > 0)
                        {
                            strTmpLists += "...";
                            break;
                        }
                    }
                    strReason = "(Job File)Missing Item:" + strTmpLists;
                }

                return false;
            }
            else
            {   //18.03.16 아래 내용 추가.
                for (int i = 0; i < LstDoc.Count; i++)
                {
                    bool bFind = LstDocumentsCols.Contains(LstDoc[i].SpecItem);
                    if (!bFind)
                    {
                        strReason = "(Job File)Missing Item:" + LstDoc[i].SpecItem;
                        return false;
                    }
                }
            }

            strReason = "OK";
            return true;
        }

        private bool CheckTimeStamp(string strMin, string strMax, ref string strNowTime)
        {
            int[] iMin = new int[3];
            int[] iMax = new int[3];
       
            string[] strMinTime;
            string[] strMaxTime;

            try
            {
                strMinTime = strMin.Split(':');
                strMaxTime = strMax.Split(':');
                if(strMinTime.Length != 3 || strMaxTime.Length != 3) return false;

                for (int i = 0; i < iMin.Length; i++)
                {
                    iMin[i] = int.Parse(strMinTime[i].ToString());
                    iMax[i] = int.Parse(strMaxTime[i].ToString());                    
                }

                
                TimeSpan tsMin = TimeSpan.FromHours(iMin[0]) + TimeSpan.FromMinutes(iMin[1]) + TimeSpan.FromSeconds(iMin[2]);
                TimeSpan tsMax = TimeSpan.FromHours(iMax[0]) + TimeSpan.FromMinutes(iMax[1]) + TimeSpan.FromSeconds(iMax[2]);
                TimeSpan tsNow = DateTime.Now.TimeOfDay;
                strNowTime = DateTime.Now.ToString("HH:mm:ss");

                if (tsMin.TotalSeconds < tsNow.TotalSeconds && tsMax.TotalSeconds > tsNow.TotalSeconds)
                {
                    return true;
                }
                return false;

                
            }
            catch
            {
            	
            }
            return false;
        }

        //EXCEL COMMAND     
        public bool RunExcelCommand(int iPortNum, int iJobNumber, string strName, string strSendparEx)
        {    
            string strResponse = String.Empty;
            string strResult = String.Empty;
            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
            {
                if (UseSlots[i])//2. 사용할 슬롯이며 오픈된 포트이며 
                {
                    RETCOUNT_DIC[i] = 0; // 3. 공통명령으로 한번만 처리한다.                    
                    UpdateDictionary((int)DICINDEX.SEQUENCE, i, iJobNumber, (int)STATUS.RUNNING);
                }
            }

            if (!CheckExprParam(iPortNum, iJobNumber, ref strSendparEx))
            {
                GateWayMsgProcess((int)STATUS.NG, STATUS.NG.ToString(), "PAR1 ERROR : " + LstJOB_CMD[iJobNumber].PAR1, String.Empty, true);               
                return true;
            }

            if (!strName.Contains("DISPLAY_BARCODE") && strSendparEx.Length < 1)
            {
                strResponse = strResult = "INVALID PAR1 VALUE";                
                GateWayMsgProcess((int)STATUS.CHECK, strResponse, strResult, String.Empty, true);
                return true;
            }
            else
            {
                switch (strName)
                {
                    case "TARGET_FILENAME":                        
                        STEPMANAGER_VALUE.strExcel_FileName = strSendparEx;
                        strResponse = strSendparEx; strResult = STATUS.OK.ToString();
                        GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                        exData = new ExcelData[2];
                        break;

                    case "DISPLAY_BARCODE": //여기 확인하자.
                        
                        if (Item_bUseBarcode)
                        {
                            if (Item_WIPID.Length == Item_iWIPSIZE) //바코드 스캐너로 읽었다면
                            {                                
                                strResponse = Item_WIPID; strResult = STATUS.OK.ToString();                                                             
                                GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true); 
                                break;
                            }
                            else
                            {
                                if (strSendparEx != null && strSendparEx.Length == Item_iWIPSIZE)
                                {
                                    Item_WIPID = strSendparEx;                                    
                                    strResponse = Item_WIPID; strResult = STATUS.OK.ToString();                                    
                                    GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                    break;
                                }      
                            }
                        }
                        else
                        {
                            if (strSendparEx != null)
                            {
                                Item_WIPID = strSendparEx;
                                strResponse  = strResult = Item_WIPID;                                
                                GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                break;
                            }
                        }
                        
                        strResponse = "NO BARCODE"; strResult = "NO BARCODE";
                        GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                        break;

                    case "DISPLAY_BARCODE_SUB":

                        if (Item_bUseBarcode && Item_bUseSubId)
                        {
                            if (Item_SUBID.Length == Item_iSUBIDSIZE) //바코드 스캐너로 읽었다면
                            {
                                strResponse = Item_SUBID; strResult = STATUS.OK.ToString();
                                GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                break;
                            }
                            else
                            {
                                if (strSendparEx != null && strSendparEx.Length == Item_iSUBIDSIZE)
                                {
                                    Item_SUBID = strSendparEx;
                                    strResponse = Item_SUBID; strResult = STATUS.OK.ToString();
                                    GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (strSendparEx != null)
                            {
                                Item_SUBID = strSendparEx;
                                strResponse = strResult = Item_SUBID;
                                GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                break;
                            }
                        }

                        strResponse = "NO BARCODE_SUB"; strResult = "NO BARCODE_SUB";
                        GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                        break;

                    case "FIND_FIELD":
                        
                        STEPMANAGER_VALUE.strExcel_FieldName = strSendparEx;
                        
                        if (Item_WIPID.Length > 0)
                        {
                            DK_EXCEL dkex = new DK_EXCEL();

                            string strReason = String.Empty;
                            bool bOK = dkex.ReadExcelData(STEPMANAGER_VALUE.strExcel_FileName, STEPMANAGER_VALUE.strExcel_FieldName, Item_WIPID, ref exData, ref strReason);

                            if (bOK)
                            {                                
                                strResponse = STATUS.OK.ToString(); strResult = STATUS.OK.ToString();                             
                                GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                break;
                            }
                            else
                            {                                
                                strResponse = strReason; strResult = strReason;                                
                                GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                                break;
                            }

                        }
                        else
                        {   
                            strResponse = "NO BARCORD"; strResult = "NO BARCORD";
                            GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                        }

                        break;

                    case "LOAD_DATA":
                        if (exData.Length == 2)
                        {                        
                            strResponse = "NO DATA"; strResult = "NO DATA";                            
                            GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                            break;
                        }
                        else
                        {
                            bool bFind = false;
                            for (int i = 0; i < exData.Length; i++)
                            {
                                if (exData[i].strSubject != null &&
                                        exData[i].strData != null &&
                                            exData[i].strSubject.Equals(strSendparEx))
                                {
                                    strResponse = strResult = exData[i].strData;
                                    GateWayMsgProcess((int)STATUS.OK, strResponse, strResponse, String.Empty, true);
                                    bFind = true;
                                    break;
                                }
                            }
                            if (!bFind)
                            {
                                strResponse = "NO DATA"; strResult = "NO DATA";                                
                                GateWayMsgProcess((int)STATUS.NG, strResponse, strResponse, String.Empty, true);
                                break;
                            }


                        }
                        break;
                    default:                        
                        strResponse = STATUS.CHECK.ToString(); strResult = STATUS.CHECK.ToString();
                        GateWayMsgProcess((int)STATUS.CHECK, strResponse, strResponse, String.Empty, true);
                        break;

                }
            }
            return true;
        }

        //GEN9
        public bool FindPacGen9(string strName, ref string strPac, ref AnalyizePack anlPack)
        {
            for (int i = 0; i < LstTBL_GEN9.Count; i++)
            {
                if (LstTBL_GEN9[i].CMDNAME.Equals(strName))
                {

                    strPac = LstTBL_GEN9[i].SENDPAC;
                    anlPack.strAanlyizeString = String.Empty;
                    anlPack.strReplaceString = LstTBL_GEN9[i].PARPAC2;
                    switch (LstTBL_GEN9[i].RECVPAC)
                    {
                        case "NORESPONSE": anlPack.iAanlyizeOption  = (int)ANALYIZEGEN9.NORESPONSE; break;                        
                        case "BYTE": anlPack.iAanlyizeOption        = (int)ANALYIZEGEN9.BYTE; break;
                        case "RESCODE": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.RESCODE; break;
                        case "CONFCODE": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.CONFCODE; break;
                        case "NORMAL": anlPack.iAanlyizeOption      = (int)ANALYIZEGEN9.NORMAL; break;
                        
                        case "TTFF": anlPack.iAanlyizeOption        = (int)ANALYIZEGEN9.TTFF; break;
                        
                        
                        case "DTC_TABLE": anlPack.iAanlyizeOption   = (int)ANALYIZEGEN9.DTC; break;
                        case "DTCOQA": anlPack.iAanlyizeOption      = (int)ANALYIZEGEN9.DTCOQA; break;
                        case "REVERSE": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.REVERSE; break;
                        case "SIMINFO": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.SIMINFO; break;
                        

                        case "CHECKSUM": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.CHECKSUM; break;

                        case "GPSINFO1": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.GPSINFO1; break;
                        case "GPSINFO2": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.GPSINFO2; break;
                        case "GPSINFO3": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.GPSINFO3; break;
                        case "GPSINFO4": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.GPSINFO4; break;
                        case "OOBRESULT": anlPack.iAanlyizeOption   = (int)ANALYIZEGEN9.OOBRESULT; break;

                        case "PLATFORM1": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.PLATFORM1; break;
                        case "PLATFORM2": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.PLATFORM2; break;
                        case "PLATFORM3": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.PLATFORM3; break;

                        case "BTMAC": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.BTMAC; break;
                            
                        case "CRLF1": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.CRLF1; break;
                        case "CRLF2": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.CRLF2; break;
                        case "CRLF3": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.CRLF3; break;
                        case "CRLF4": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.CRLF4; break;
                        case "CRLF5": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.CRLF5; break;

                        case "DTC00": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC00; break;
                        case "DTC01": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC01; break;
                        case "DTC02": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC02; break;
                        case "DTC03": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC03; break;
                        case "DTC04": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC04; break;
                        case "DTC05": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC05; break;
                        case "DTC06": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC06; break;
                        case "DTC07": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC07; break;
                        case "DTC08": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC08; break;
                        case "DTC09": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC09; break;
                        case "DTC10": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC10; break;
                        case "DTC11": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC11; break;
                        case "DTC12": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC12; break;
                        case "DTC13": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC13; break;
                        case "DTC14": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC14; break;
                        case "DTC15": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC15; break;
                        case "DTC16": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC16; break;
                        case "DTC17": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC17; break;
                        case "DTC18": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC18; break;
                        case "DTC19": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC19; break;
                        case "DTC20": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC20; break;
                        case "DTC21": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC21; break;
                        case "DTC22": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC22; break;
                        case "DTC23": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC23; break;
                        case "DTC24": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC24; break;
                        case "DTC25": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC25; break;
                        case "DTC26": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC26; break;
                        case "DTC27": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC27; break;
                        case "DTC28": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC28; break;

                        case "DTC00_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC00_BITS; break;
                        case "DTC01_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC01_BITS; break;
                        case "DTC02_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC02_BITS; break;
                        case "DTC03_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC03_BITS; break;
                        case "DTC04_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC04_BITS; break;
                        case "DTC05_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC05_BITS; break;
                        case "DTC06_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC06_BITS; break;
                        case "DTC07_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC07_BITS; break;
                        case "DTC08_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC08_BITS; break;
                        case "DTC09_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC09_BITS; break;
                        case "DTC10_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC10_BITS; break;
                        case "DTC11_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC11_BITS; break;
                        case "DTC12_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC12_BITS; break;
                        case "DTC13_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC13_BITS; break;
                        case "DTC14_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC14_BITS; break;
                        case "DTC15_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC15_BITS; break;
                        case "DTC16_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC16_BITS; break;
                        case "DTC17_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC17_BITS; break;
                        case "DTC18_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC18_BITS; break;
                        case "DTC19_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC19_BITS; break;
                        case "DTC20_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC20_BITS; break;
                        case "DTC21_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC21_BITS; break;
                        case "DTC22_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC22_BITS; break;
                        case "DTC23_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC23_BITS; break;
                        case "DTC24_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC24_BITS; break;
                        case "DTC25_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC25_BITS; break;
                        case "DTC26_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC26_BITS; break;
                        case "DTC27_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC27_BITS; break;
                        case "DTC28_BITS": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.DTC28_BITS; break;

                        case "ALDL_ASCII": anlPack.iAanlyizeOption   = (int)ANALYIZEGEN9.ALDL_ASCII;    break;
                        case "ALDL_HEX": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.ALDL_HEX;      break;
                        case "ALDL_DECIMAL": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.ALDL_DECIMAL;  break;
                        case "ALDL_BITS":  anlPack.iAanlyizeOption   = (int)ANALYIZEGEN9.ALDL_BITS;     break;

                        case "ST_ESN_MEID": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.ST_ESN_MEID; break;                        
                        case "ST_IMEI":  anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.ST_IMEI;  break;
                        case "ST_ICCID": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.ST_ICCID; break;

                        case "MDN_COUNTRY": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_COUNTRY; break;
                        case "MDN_MIN": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.MDN_MIN;     break;
                        case "MDN_MDN": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.MDN_MDN;     break;
                        case "MDN_HOMESID": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_HOMESID; break;
                        case "MDN_COUNT": anlPack.iAanlyizeOption   = (int)ANALYIZEGEN9.MDN_COUNT;   break;

                        case "MDN_SID_00": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_00; break;
                        case "MDN_SID_01": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_01; break;
                        case "MDN_SID_02": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_02; break;
                        case "MDN_SID_03": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_03; break;
                        case "MDN_SID_04": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_04; break;
                        case "MDN_SID_05": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_05; break;
                        case "MDN_SID_06": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_06; break;
                        case "MDN_SID_07": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_07; break;
                        case "MDN_SID_08": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_08; break;
                        case "MDN_SID_09": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_09; break;
                        case "MDN_SID_10": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_SID_10; break;

                        case "MDN_NID_00": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_00; break;
                        case "MDN_NID_01": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_01; break;
                        case "MDN_NID_02": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_02; break;
                        case "MDN_NID_03": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_03; break;
                        case "MDN_NID_04": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_04; break;
                        case "MDN_NID_05": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_05; break;
                        case "MDN_NID_06": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_06; break;
                        case "MDN_NID_07": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_07; break;
                        case "MDN_NID_08": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_08; break;
                        case "MDN_NID_09": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_09; break;
                        case "MDN_NID_10": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.MDN_NID_10; break;


                        case "NONEED": anlPack.iAanlyizeOption     = (int)ANALYIZEGEN9.NONEED; break; //measure 에 안찍고싶을때( 괜한 혼돈을 줄수 있는 매저값 미표기할때)
                        case "GEN9_IMSI": anlPack.iAanlyizeOption  = (int)ANALYIZEGEN9.GEN9_IMSI; break; //GEN9 TCU 에서 IMSI 읽을때 지그재그바이트오더로 읽어야함.
                        case "GEN9_ICCID": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.GEN9_ICCID; break; //GEN9 TCU 에서 ICCID 읽을때 지그재그바이트오더로 읽어야함.
                        case "GEN9_APN": anlPack.iAanlyizeOption   = (int)ANALYIZEGEN9.GEN9_APN; break; //GEN9 APN 읽을때 전용파싱 (구조체임)
                        case "GEN9_APN_PW": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.GEN9_APN_PW; break; //GEN9_APN_PW 읽을때 전용파싱 (구조체임)    
                        case "GEN9_NAI": anlPack.iAanlyizeOption    = (int)ANALYIZEGEN9.GEN9_NAI; break; //GEN9 NAI 읽을때 전용파싱, APN 이랑 같은건데 하나 더만든거임. (구조체임)

                        case "FINFOSIZE": anlPack.iAanlyizeOption = (int)ANALYIZEGEN9.FINFOSIZE; break;

                        default: anlPack.iAanlyizeOption           = (int)ANALYIZEGEN9.NONE; break;

                    }

                    return true;
                }
            }
            return false;
        }
               
        //GEN10
        public bool FindPacGen10(string strName, ref string strPac, ref AnalyizePack anlPack)
        {
            for (int i = 0; i < LstTBL_GEN10.Count; i++)
            {
                if (LstTBL_GEN10[i].CMDNAME.Equals(strName))
                {      

                    strPac = LstTBL_GEN10[i].SENDPAC;
                    anlPack.strAanlyizeString = String.Empty;
                    anlPack.strReplaceString = LstTBL_GEN10[i].PARPAC2;
                    switch (LstTBL_GEN10[i].RECVPAC)
                    {                        
                        case "NORESPONSE":  anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.NORESPONSE; break;
                        case "BYTE":        anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.BYTE;    break;
                        case "RESCODE":     anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.RESCODE; break;
                        case "NORMAL":      anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.NORMAL;  break;
                        case "BLUEGO":      anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.BLUEGO; 
                                            anlPack.strAanlyizeString = LstTBL_GEN10[i].PARPAC1; break;
                        case "BYTEPACK":    anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.BYTEPACK;
                                            anlPack.strAanlyizeString = LstTBL_GEN10[i].PARPAC1; break;
                        case "BINPACK":     anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.BINPACK;
                                            anlPack.strAanlyizeString = LstTBL_GEN10[i].PARPAC1; break;

                        case "TTFF":        anlPack.iAanlyizeOption   = (int)ANALYIZEGEN10.TTFF; break;
                        case "ALDL_ASCII":  anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.ALDL_ASCII;
                                            STEPMANAGER_VALUE.bOldBinaryALDL = new byte[1]; //초기화해줌.
                                            break;
                        case "ALDL_BITS":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.ALDL_BITS; break;
                        case "DTC_TABLE":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.DTC; break;
                        case "DTCOQA":      anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.DTCOQA; break;  
                        case "REVERSE":     anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.REVERSE; break;
                        case "SIMINFO":     anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.SIMINFO; break;
                        case "SERVICE":     anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.SERVICE; break;

                        case "T_SINGLE":    anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.T_SINGLE; break;
                        case "T_NADINFO":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.T_NADINFO; break;
                        case "CHECKSUM":    anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.CHECKSUM; break;

                        case "FINFOSIZE":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.FINFOSIZE; break;
                        case "FINFOBYTE":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.FINFOBYTE; break;

                        case "GPSINFO1":    anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.GPSINFO1; break;
                        case "GPSINFO2":    anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.GPSINFO2; break;
                        case "GPSINFO3":    anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.GPSINFO3; break;
                        case "GPSINFO4":    anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.GPSINFO4; break;
                        case "OOBRESULT":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.OOBRESULT; break;
                        case "APN_TABLE":   anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.APN_TABLE; break;
                        default:            anlPack.iAanlyizeOption = (int)ANALYIZEGEN10.NONE; break;

                    }

                    return true;
                }
            }
            return false;
        }

        //TCP
        public bool FindPacTcp(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_TCP.Count; i++)
            {
                if (LstTBL_TCP[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_TCP[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //GEN11
        public bool FindPacGen11(string strName, ref string strPac)
        {            
            for (int i = 0; i < LstTBL_GEN11.Count; i++)
            {
                if (LstTBL_GEN11[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_GEN11[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //GEN11
        public bool FindPacGen11P(string strName, ref string strPac)
        {            
            for (int i = 0; i < LstTBL_GEN11P.Count; i++)
            {
                if (LstTBL_GEN11P[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_GEN11P[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //GEN12
        public bool FindPacGen12(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_GEN12.Count; i++)
            {
                if (LstTBL_GEN12[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_GEN12[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //MCTM
        public bool FindPacMCTM(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_MCTM.Count; i++)
            {
                if (LstTBL_MCTM[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_MCTM[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //NAD
        public bool FindPacNAD(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_NAD.Count; i++)
            {
                if (LstTBL_NAD[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_NAD[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //CCM
        public bool FindPacCCM(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_CCM.Count; i++)
            {
                if (LstTBL_CCM[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_CCM[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //ATT
        public bool FindPacAtt(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_ATT.Count; i++)
            {
                if (LstTBL_ATT[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_ATT[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //DIO
        public bool FindPacDioVcpBench(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_DIO.Count; i++)
            {
                if (LstTBL_DIO[i].CMDNAME.Equals(strName))
                {                                       
                    strPac = LstTBL_DIO[i].SENDPAC;                   
                    return true;
                }
            }
            return false;
        }

        //DIO - AUDIO
        public bool FindPacDioAudioSelector(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_AUDIO.Count; i++)
            {
                if (LstTBL_AUDIO[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_AUDIO[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //DIO - ADC MODULE
        public bool FindPacDioAdcModule(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_ADC.Count; i++)
            {
                if (LstTBL_ADC[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_ADC[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //ODA POWER
        public bool FindPacOdaPower(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_PWR.Count; i++)
            {
                if (LstTBL_PWR[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_PWR[i].SENDPAC;                    
                    return true;
                }
            }
            return false;
        }

        //TC3000
        public bool FindPacTC3000(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_TC3000.Count; i++)
            {
                if (LstTBL_TC3000[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_TC3000[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }
        
        //DLL GATE
        public bool FindPacDLLGATE(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_DLLGATE.Count; i++)
            {
                if (LstTBL_DLLGATE[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_DLLGATE[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //MELSEC
        public bool FindPacMELSEC(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_MELSEC.Count; i++)
            {
                if (LstTBL_MELSEC[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_MELSEC[i].SENDPAC;
                    return true;
                }
            }
            return false;
        }

        //TC1400A SOCKET
        public bool FindPacTC1400A(string strName, string strParam, ref string strPac, ref string strDataType, ref string strErrorReason)
        {
            bool bReturn = false;

            //소켓 특성상 여기서 파라미터를 교체해줘야할것 같다.
            for (int i = 0; i < LstTBL_TC1400A.Count; i++)
            {
                if (String.Compare(LstTBL_TC1400A[i].CMDNAME.ToString(), strName.ToString()) == 0)
                {
                    strPac = LstTBL_TC1400A[i].SENDPAC;
                    strDataType = LstTBL_TC1400A[i].RECVPAC;
                    bReturn = true;

                    if (strPac.Contains("<DATA>") && String.IsNullOrEmpty(strParam))
                    {
                        strErrorReason = "PAR1 ERROR";
                        return false;
                    }

                    if (strPac.Contains("<DATA>"))
                    {
                        strPac = strPac.Replace("<DATA>", strParam.Trim());
                        bReturn = true;
                    }

                    break;
                }
            }
            return bReturn;
        }

        public bool FindPacMTP120A(string strName, ref string strPac, ref string strSendpacOption)
        {
            for (int i = 0; i < LstTBL_MTP120A.Count; i++)
            {
                if (String.Compare(LstTBL_MTP120A[i].CMDNAME.ToString(), strName.ToString()) == 0)
                {
                    strPac = LstTBL_MTP120A[i].SENDPAC;
                    strSendpacOption = LstTBL_MTP120A[i].RECVPAC;
                    return true;
                }
            }
            return false;
        }

        private void Excute34410A(string strName, int iJobNumber, string strSendpac, string strCmdName, string strSendparEx, string strSendpacOption)
        {
            COMMDATA resData = new COMMDATA();
            if (DK_GPIB_34410A != null)
            {
                DK_GPIB_34410A.SendRecv(strSendpac, strCmdName, strSendparEx, ref resData);
                string strResult = String.Empty;

                if (resData.iStatus == (int)STATUS.OK && resData.ResultData.Length > 0)
                {

                    try
                    { //tmpDouble.ToString("0.00");
                        double dTmpval = double.Parse(resData.ResultData);
                        if (strSendpacOption.Length > 0)
                        {
                            switch (strSendpacOption)
                            {
                                case "A": break;
                                case "mA": dTmpval = dTmpval * 1000; break;
                                case "uA": dTmpval = dTmpval * 1000000; break;
                                case "V": break;
                                default: break;
                            }
                        }

                        if (dTmpval < 1)
                        {
                            resData.ResultData = dTmpval.ToString("0.###");
                        }
                        else
                        {
                            resData.ResultData = dTmpval.ToString("0.###");
                        }

                    }
                    catch
                    {
                        resData.ResultData = "DATA ERROR : " + resData.ResultData;
                        resData.iStatus = (int)STATUS.CHECK;
                    }
                }
            }
            else
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Device Disconnected", "NO USB", String.Empty, true);
                return;
            }

            if (resData.iStatus == (int)STATUS.CHECK)
            {
                resData.iStatus = (int)STATUS.CHECK;
            }
            GateWay_MANAGER(resData);
            return;
        }

        private void ExcutePCAN_Reconnect(string strTarget)
        {
            string strResponseData = String.Empty;
            string strResultData = String.Empty;
            
            bool bPsaMode = DK_PCAN_USB.IsAliveAutoSending((int)CANUSE.PSA);
            bool bGbMode  = DK_PCAN_USB.IsAliveAutoSending((int)CANUSE.GB);
            bool bGbMY23Mode = DK_PCAN_USB.IsAliveAutoSending((int)CANUSE.GBMY23);
            bool bGemMode = DK_PCAN_USB.IsAliveAutoSending((int)CANUSE.GEM);
            bool bMctmMode = DK_PCAN_USB.IsAliveAutoSending((int)CANUSE.MCTM);

            if (bPsaMode || bGbMode || bMctmMode || bGbMY23Mode) //thread가 사용중이었다면 다시 살려야 하므로
            {
                MessageLogging((int)LOGTYPE.TX, "STOP_CAN_CONTINUOS_MODE", (int)DEFINES.SET1);
                DK_PCAN_USB.CloseSendThread();
                System.Threading.Thread.Sleep(100);
            }

            MessageLogging((int)LOGTYPE.TX, "TRY_USB_RECONNECT", (int)DEFINES.SET1);

            if (DeviceControlUSB("restart"))
            {
                try
                {
                    Item_dStepTimeSet = 5;
                    dtStepDelaySet = DateTime.Now;
                    while (!StepDelayTimeCheck())
                    {
                        if (Item_dStepTimeSet <= 0) break;
                        System.Threading.Thread.Sleep(1500);
                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;

                        if (PcanConnection().Equals((int)STATUS.OK))
                        {
                            MessageLogging((int)LOGTYPE.TX, "SUCCESS_USB_RECONNECT", (int)DEFINES.SET1);
                            strResponseData = strResultData = "OK";
                            break;
                        }
                    }
                }
                catch
                {
                    MessageLogging((int)LOGTYPE.TX, "ERROR_USB_RECONNECT", (int)DEFINES.SET1);
                    strResponseData = strResultData = "NONE.";
                }

            }
            else
            {
                MessageLogging((int)LOGTYPE.TX, "FAIL_USB_RECONNECT", (int)DEFINES.SET1);
                strResponseData = strResultData = "NONE";
            }

            if (bPsaMode || bGbMode || bMctmMode) //autosending 사용중이었다면 다시 살려야 하므로
            {
                System.Threading.Thread.Sleep(1000);
                bool bPsaRestart = false;
                string strTmpMsg = String.Empty;
                for (int i = 0; i < 5; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    MessageLogging((int)LOGTYPE.TX, "RESTART_CONTINUOS_MODE", (int)DEFINES.SET1);

                    if (bPsaMode)
                    {
                        bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 4, "PSA_BOOT");
                    }
                    else
                    {
                        switch (strTarget)
                        {
                            case "GEN11":
                            case "GEN11P":
                            case "GEN12":
                                if (bGbMode)
                                {
                                    bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 2, "GEN11_GB_BOOT");
                                }
                                else if (bGbMY23Mode)
                                {
                                    bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 2, "GEN11_GB_BOOT_MY23");
                                }
                                else if (bGemMode)
                                {
                                    bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 2, "GEN11_GEM_BOOT");
                                }
                                else if (bMctmMode)
                                {
                                    bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 10, "MCTM_LSCAN_VNMF");
                                }
                                break;
                            // 경민 추가
                            //case "GEN12":
                            //    if (bGbMode)
                            //    {
                            //        bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 2, "GEN12_GB_BOOT");
                            //    }
                            //    else if (bGbMY23Mode)
                            //    {
                            //        bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 2, "GEN12_GB_BOOT_MY23");
                            //    }
                            //    else if (bGemMode)
                            //    {
                            //        bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 2, "GEN12_GEM_BOOT");
                            //    }
                            //    else if (bMctmMode)
                            //    {
                            //        bPsaRestart = DK_PCAN_USB.ContinuesWakeup(ref strTmpMsg, 10, "MCTM_LSCAN_VNMF");
                            //    }
                            //    break;
                        }
                    }
                    
                    if (bPsaRestart)
                    {
                        break;
                    }
                }
                if (!bPsaRestart) MessageLogging((int)LOGTYPE.TX, "RESTART_CONTINUOS_MODE_FAIL", (int)DEFINES.SET1);
            }
            GateWayMsgProcess((int)STATUS.OK, strResponseData, strResultData, String.Empty, true);
        }

        private void ExcuteMTP120A(string strName, int iJobNumber, string strSendpac, string strCmdName, string strSendparEx, string strSendpacOption)
        {
            COMMDATA resData = new COMMDATA();
            if (DK_VISA_MTP120A != null)
            {
                DK_VISA_MTP120A.SendRecv(strSendpac, strCmdName, strSendparEx, ref resData);
                string strResult = String.Empty;

                //resData.iStatus = (int)STATUS.OK;
                //resData.ResultData = "-11";

                //명령어 중 MEASURE_SNR_V 와 같이 _V 인 명령어는 Vrms 로 변환한다.
                // Vrms = 10^(dBV/20)
                // dBV = 20 log (Vrms)
                if (resData.iStatus == (int)STATUS.OK && resData.ResultData.Length > 0)
                {
                    if (strCmdName.ToUpper().Contains("_VRMS"))
                    {
                        double dTmpval = double.Parse(resData.ResultData);  //dBV
                        dTmpval = Math.Pow(10, (dTmpval / 20));

                        if (dTmpval < 1)
                        {
                            resData.ResultData = dTmpval.ToString("0.###");
                        }
                        else
                        {
                            resData.ResultData = dTmpval.ToString("0.###");
                        }
                    }
                }
            }
            else
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Device Disconnected", "NO SOCKET", String.Empty, true);
                return;
            }

            if (resData.iStatus == (int)STATUS.CHECK)
            {
                resData.iStatus = (int)STATUS.CHECK;
            }
            GateWay_MANAGER(resData);
            return;
        }


        private string ChangeDisableBit(string strOrigin)
        {    
            try
            {  
                byte a = Convert.ToByte(strOrigin);

                // (1) byte를 비트문자열로 변환                        
                StringBuilder sb = new StringBuilder();
                sb.Append(Convert.ToString(a, 2).PadLeft(8, '0'));
                sb[7] = '0'; //disable 처리

                // (2) 비트문자열을 byte로 변환
                string bitStr = sb.ToString();
                int b = Convert.ToInt16(bitStr, 2);

                string strResult = b.ToString();
                return strResult;

            }
            catch
            {
                return "0";
            }
        }

        private bool GetMsgUnlockSecure(string strType, int iStep, ref string[] strUnlock, ref string strResponse)
        {
            switch (strType)
            {
                case "UNLOCK_TCP" :

                    switch (iStep)
                    {
                        case 0:
                            strUnlock = System.Text.RegularExpressions.Regex.Split("03 3B 02 AA 00 00 00 00", " ");
                            strResponse = "02 7B 02 00 00 00 00 00"; break;
                        case 1: System.Threading.Thread.Sleep(900);

                            strUnlock = System.Text.RegularExpressions.Regex.Split("10 08 3B 44 01 02 00 80", " ");
                            strResponse = "30 00 01 00 00 00 00 00"; break;
                        case 2: System.Threading.Thread.Sleep(100);

                            strUnlock = System.Text.RegularExpressions.Regex.Split("21 06 11 00 00 00 00 00", " ");
                            strResponse = "02 7B 44 00 00 00 00 00"; break;
                        case 3:
                            strUnlock = System.Text.RegularExpressions.Regex.Split("03 3B 02 00 00 00 00 00", " ");
                            strResponse = "02 7B 02 00 00 00 00 00"; break;
                        default: return false;
                    }
                    return true;

                case "UNLOCK_GEN11":
                case "UNLOCK_GEN12":
                    switch (iStep)
                    {
                        case 0: //44번지 먼저 READ 한다. - 나중에 ALDL 값이 툴에서 바꿨니 안바꿨니 책임소재 파악을 위하여.
                            strArray44Blocks = new string[12];
                            strUnlock = System.Text.RegularExpressions.Regex.Split("02 1A 44 00 00 00 00 00", " ");
                            strResponse = "10 08 5A 44 *0 *1 *2 *3"; break;
                        case 1: System.Threading.Thread.Sleep(5);
                             //멀티메시지 수신
                            strUnlock = System.Text.RegularExpressions.Regex.Split("30 00 01 00 00 00 00 00", " ");
                            strResponse = "21 *4 *5 00 00 00 00 00"; break;

                        case 2: System.Threading.Thread.Sleep(5);
                            //Securty Access
                            strUnlock = System.Text.RegularExpressions.Regex.Split("02 27 01 00 00 00 00 00", " ");
                            strResponse = "07 67 01 00 00 00 00 00"; break;

                        case 3: System.Threading.Thread.Sleep(50);
                            //Secure logging disable1
                            string strOriginBlocks1 = strArray44Blocks[0] + strArray44Blocks[1] + " " +
                                                     strArray44Blocks[2] + strArray44Blocks[3] + " " +
                                                     strArray44Blocks[4] + strArray44Blocks[5] + " " +
                                                     strArray44Blocks[6] + ChangeDisableBit(strArray44Blocks[7]); //"0"; //disable 
                            strUnlock = System.Text.RegularExpressions.Regex.Split("10 08 3B 44 " + strOriginBlocks1, " ");
                            strResponse = "30 00 01 00 00 00 00 00"; break;

                        case 4: System.Threading.Thread.Sleep(5);
                            //Secure logging disable2
                            string strOriginBlocks2 = strArray44Blocks[8] + strArray44Blocks[9] + " " +
                                                     strArray44Blocks[10] + strArray44Blocks[11];
                            strUnlock = System.Text.RegularExpressions.Regex.Split("21 " + strOriginBlocks2 + " 00 00 00 00 00", " ");
                            strResponse = "02 7B 44 00 00 00 00"; break;

                        case 5: //44번지 다시 READ 하여 확인 - 나중에 ALDL 값이 툴에서 바꿨니 안바꿨니 책임소재 파악을 위하여. 
                            strUnlock = System.Text.RegularExpressions.Regex.Split("02 1A 44 00 00 00 00 00", " ");
                            strResponse = "10 08 5A 44 ~~ ~~ ~~ ~~"; break;

                        case 6: System.Threading.Thread.Sleep(5);
                            //멀티메시지 수신
                            strUnlock = System.Text.RegularExpressions.Regex.Split("30 00 01 00 00 00 00 00", " ");
                            strResponse = "21 ~~ ~~ 00 00 00 00 00"; break;

                        default: return false;
                    }
                    return true;

                //경민 추가
                //case "UNLOCK_GEN12":
                //    switch (iStep)
                //    {
                //        case 0: //44번지 먼저 READ 한다. - 나중에 ALDL 값이 툴에서 바꿨니 안바꿨니 책임소재 파악을 위하여.
                //            strArray44Blocks = new string[12];
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("02 1A 44 00 00 00 00 00", " ");
                //            strResponse = "10 08 5A 44 *0 *1 *2 *3"; break;
                //        case 1:
                //            System.Threading.Thread.Sleep(5);
                //            //멀티메시지 수신
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("30 00 01 00 00 00 00 00", " ");
                //            strResponse = "21 *4 *5 00 00 00 00 00"; break;

                //        case 2:
                //            System.Threading.Thread.Sleep(5);
                //            //Securty Access
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("02 27 01 00 00 00 00 00", " ");
                //            strResponse = "07 67 01 00 00 00 00 00"; break;

                //        case 3:
                //            System.Threading.Thread.Sleep(50);
                //            //Secure logging disable1
                //            string strOriginBlocks1 = strArray44Blocks[0] + strArray44Blocks[1] + " " +
                //                                     strArray44Blocks[2] + strArray44Blocks[3] + " " +
                //                                     strArray44Blocks[4] + strArray44Blocks[5] + " " +
                //                                     strArray44Blocks[6] + ChangeDisableBit(strArray44Blocks[7]); //"0"; //disable 
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("10 08 3B 44 " + strOriginBlocks1, " ");
                //            strResponse = "30 00 01 00 00 00 00 00"; break;

                //        case 4:
                //            System.Threading.Thread.Sleep(5);
                //            //Secure logging disable2
                //            string strOriginBlocks2 = strArray44Blocks[8] + strArray44Blocks[9] + " " +
                //                                     strArray44Blocks[10] + strArray44Blocks[11];
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("21 " + strOriginBlocks2 + " 00 00 00 00 00", " ");
                //            strResponse = "02 7B 44 00 00 00 00"; break;

                //        case 5: //44번지 다시 READ 하여 확인 - 나중에 ALDL 값이 툴에서 바꿨니 안바꿨니 책임소재 파악을 위하여. 
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("02 1A 44 00 00 00 00 00", " ");
                //            strResponse = "10 08 5A 44 ~~ ~~ ~~ ~~"; break;

                //        case 6:
                //            System.Threading.Thread.Sleep(5);
                //            //멀티메시지 수신
                //            strUnlock = System.Text.RegularExpressions.Regex.Split("30 00 01 00 00 00 00 00", " ");
                //            strResponse = "21 ~~ ~~ 00 00 00 00 00"; break;

                //        default: return false;
                //    }
                //    return true;

                default: return false;

            }
            
        }

        private void ExcuteMELSEC(string strName, string strParam)
        {
            if (DK_MELSEC == null) { GateWayMsgProcess((int)STATUS.CHECK, "Not Using MELSEC Interface", "Not Using MELSEC Interface", String.Empty, true); return; }

            bool bResult = false;
            string strResponseData = String.Empty;
            switch (strName)
            {
                case "CONNECT":
                    bResult = DK_MELSEC.Connect(ref strResponseData);
                    break;

                case "DISCONNECT":
                    DK_MELSEC.Disconnect();
                    bResult = true; strResponseData = "OK";
                    break;

                case "READ_MEMORY_WORD":
                    bResult = DK_MELSEC.ReadMemory(strName, strParam, (int)MELSECRESTYPE.WORD, ref strResponseData);
                    break;
                case "READ_MEMORY_BYTE":
                    bResult = DK_MELSEC.ReadMemory(strName, strParam, (int)MELSECRESTYPE.BYTE, ref strResponseData);
                    break;

                default:
                    GateWayMsgProcess((int)STATUS.CHECK, "Unknown Command Error.", "Unknown Command Error.", String.Empty, true); return;
            }

            if (bResult)
                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResponseData, "NOLOG", true);
            else
                GateWayMsgProcess((int)STATUS.NG, strResponseData, strResponseData, "NOLOG", true);
            return;
        }

        private void ExcutePCAN(string strName, int iJobNumber)
        {
            int iSendRecvOption = ConvertOption(LstJOB_CMD[iJobNumber].OPTION);
            string strPac = String.Empty;
            string strSendCode = String.Empty;
            string strRecvCode = String.Empty;
            string strMeasOption = String.Empty;
            string strMeasPac = String.Empty;
            int iBaudrate = 2;

            double dTimeOut = 1.0;
            try
            {
                dTimeOut = double.Parse(LstJOB_CMD[iJobNumber].TIMEOUT);
            }
            catch
            {
                dTimeOut = 1.0;
            }


            bool bFind = false;

            for (int i = 0; i < LstTBL_PCAN.Count; i++)
            {
                if (LstTBL_PCAN[i].CMDNAME.Equals(strName))
                {
                    strPac      = LstTBL_PCAN[i].SENDPAC;
                    strSendCode = LstTBL_PCAN[i].RECVPAC;
                    strRecvCode = LstTBL_PCAN[i].PARPAC1;

                    strMeasOption = LstTBL_PCAN[i].OPTION1;
                    strMeasPac = LstTBL_PCAN[i].OPTION2;

                    try
                    {
                        iBaudrate = int.Parse(LstTBL_PCAN[i].PARPAC2);
                    }
                    catch { iBaudrate = 2; }

                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Unknown Command.", "NO DATA.", String.Empty, true);
                return;
            }                       

            if (OpenPCAN != (int)STATUS.OK)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Device Disconnected.", "Device Disconnected.", String.Empty, true);            
                return;
            }

            string[] tmpdata = System.Text.RegularExpressions.Regex.Split(strPac, " ");

            if (tmpdata.Length != 8)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Command Error.", "NO DATA.", String.Empty, true);
                return;
            }

            string strSendPacket = strName + "," + strSendCode + "," + strPac;
            string strResponseData = "";
            string strResultData = "";
            int  iStatus = 0;

            string strMsg = "OK";

            switch (strName) //나중을 위해서.
            {
                case "RESTART_DEVICE_MANAGER":
                                ExcutePCAN_Reconnect(LstJOB_CMD[iJobNumber].TYPE);
                                return;
                
                case "CHECK_PCAN_STATUS":                                
                                MessageLogging((int)LOGTYPE.TX, String.Empty, (int)DEFINES.SET1);                    
                                strResponseData = strResultData = DK_PCAN_USB.CheckPcanStatus();
                                GateWayMsgProcess((int)STATUS.OK, strResponseData, strResultData, String.Empty, true);       
                                return;
                case "MCTM_LSCAN_VNMF":
                case "GEN11_GEM_BOOT":
                case "GEN11_GB_BOOT":
                case "GEN11_GB_BOOT_MY23":
                case "GEN12_GEM_BOOT":
                case "GEN12_GB_BOOT":
                case "GEN12_GB_BOOT_MY23":
                case "PSA_BOOT":
                case "GEN12_GB_BOOT_500":
                                if (DK_PCAN_USB.ContinuesWakeup(ref strMsg, iBaudrate, strName))
                                 {
                                     iStatus = (int)STATUS.OK;
                                     strSendPacket = strName;
                                     GateWayMsgProcess(iStatus, "OK", "OK", strMsg, true); 
                                     return;
                                 }
                                 else
                                 {
                                     iStatus = (int)STATUS.NG;
                                     strSendPacket = strName;
                                     DK_PCAN_USB.Release();
                                     GateWayMsgProcess(iStatus, strMsg, strMsg, String.Empty, true);
                                     return;
                                 }
                case "MCTM_LSCAN_VNMF_END":
                case "GEN11_GB_END":
                case "GEN11_GEM_END":
                case "GEN12_GB_END":
                case "GEN12_GEM_END":
                case "PSA_END":  DK_PCAN_USB.CloseSendThread();
                                 GateWayMsgProcess((int)STATUS.OK, "OK", "OK", String.Empty, true);
                                 return;

                default:


                                 if (strName.Contains("PSA_TEST") || strName.Equals("MCTM_LSCAN_TEST"))  //PSA CAN 관련 명령일 경우만...
                                 {
                                     //DK_PCAN_USB.PauseThreadObject(true);
                                     TPCANMsg tmpCanMsg = new TPCANMsg();
                                     string strRtnMsg = String.Empty;

                                     if (DK_PCAN_USB.Send(strSendCode, tmpdata, ref strRtnMsg, strName, ref tmpCanMsg, strRecvCode, iSendRecvOption, false, dTimeOut))
                                     {
                                         if (tmpCanMsg.DATA != null)
                                         {
                                             strResponseData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                             strResultData   = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                         }
                                         iStatus = (int)STATUS.OK;
                                         GateWayMsgProcess(iStatus, strResponseData, strResultData, String.Empty, false);
                                         return;

                                     }
                                     else
                                     {                                         
                                         strResponseData = strResultData = strRtnMsg;
                                         iStatus = (int)STATUS.NG;
                                         if (strResponseData.Contains("TIME OUT"))
                                         {
                                             ExcutePCAN_Reconnect(LstJOB_CMD[iJobNumber].TYPE);
                                         }
                                         GateWayMsgProcess(iStatus, strResponseData, strResultData, String.Empty, true);
                                         return;
                                     }
               

                                 }
                                 else  //기타 PSA모델이 아닌 다른것 들은 Auto Sending 을 종료한다.
                                 {
                                     DK_PCAN_USB.CloseSendThread();
                                 }                                 
                                 break;
                
            }

            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
            {
                GateWayMsgProcess((int)STATUS.NG, "USER STOP.", "NO DATA.", String.Empty, true);       
                return;
            }

            if (DK_PCAN_USB.Initialize(ref strMsg, iBaudrate))
            {                
                string strRtnMsg = String.Empty;
                TPCANMsg tmpCanMsg = new TPCANMsg();
                string strErrMsg = String.Empty;
                DK_PCAN_USB.Reset();

                switch (strName) 
                {
                    case "UNLOCK_GEN11_GB":
                    case "UNLOCK_GEN12_GB":
                        if (DK_PCAN_USB.UnlockProcessGBGEM(0, strName))
                        {
                            strResponseData = "UNLOCK COMPLETE-GB";
                            strResultData = "UNLOCK COMPLETE-GB";
                            iStatus = (int)STATUS.OK;
                        }
                        else
                        {
                            strResponseData = "UNLOCK FAIL-GB";
                            strResultData = "UNLOCK FAIL-GB";
                            iStatus = (int)STATUS.NG;
                        }
                        break;
                    case "UNLOCK_GEN11_GEM":
                    case "UNLOCK_GEN12_GEM":
                        if (DK_PCAN_USB.UnlockProcessGBGEM(1, strName))
                        {
                            strResponseData = "UNLOCK COMPLETE-GEM";
                            strResultData   = "UNLOCK COMPLETE-GEM";
                            iStatus = (int)STATUS.OK;
                        }
                        else
                        {
                            strResponseData = "UNLOCK FAIL-GEM";
                            strResultData = "UNLOCK FAIL-GEM";
                            iStatus = (int)STATUS.NG;
                        }
                        break;
                    case "UNLOCK_GEN11":
                    case "UNLOCK_GEN12":
                    case "UNLOCK_TCP":
                        string[] strUnlock = new string[8];
                        string strResponse = String.Empty;
                        string strRecvData = String.Empty;
                        bool bComplete = true;
                        DK_DECISION tmpDec = new DK_DECISION();

                        int iSeq = 0;
                        switch (strName)
                        {
                            case "UNLOCK_GEN11": iSeq = 7; break;
                            case "UNLOCK_GEN12": iSeq = 7; break;
                            case "UNLOCK_TCP": iSeq = 4; break;
                            default: iSeq = 0; break;
                        }

                        for (int i = 0; i < iSeq; i++)
                        {

                            if (!GetMsgUnlockSecure(strName, i, ref strUnlock, ref strResponse)) break;

                            if (DK_PCAN_USB.Send(strSendCode, strUnlock, ref strRtnMsg, strName, ref tmpCanMsg, strRecvCode, (int)MODE.SENDRECV, false, dTimeOut))
                            {
                                if (tmpCanMsg.DATA != null)
                                {
                                    strRecvData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");

                                    if (!tmpDec.ComparePattern2(strRecvData, strResponse, ref strArray44Blocks).Equals((int)STATUS.OK))
                                    {
                                        bComplete = false;
                                        strResponseData = strRtnMsg;
                                        iStatus = (int)STATUS.NG;
                                        break;
                                    }                                    
                                }
                                else
                                {
                                    bComplete = false;
                                    strResponseData = "NO DATA";
                                    iStatus = (int)STATUS.NG;
                                    break;
                                }
                            }
                            else
                            {
                                bComplete = false;
                                strResponseData = strRtnMsg;
                                iStatus = (int)STATUS.NG;
                                break;
                            }
                        }
                        if (bComplete)
                        {
                            strResponseData = "UNLOCK COMPLETE";
                            strResultData   = "UNLOCK COMPLETE";
                            iStatus = (int)STATUS.OK;
                        }
                        break;

                    case "INITIALZE_HSCAN":
                        MessageLogging((int)LOGTYPE.TX, String.Empty, (int)DEFINES.SET1);
                        strResponseData = strResultData = "OK";
                        GateWayMsgProcess((int)STATUS.OK, strResponseData, strResultData, String.Empty, true);
                        return;

                    case "UNINITIALZE_HSCAN":
                        MessageLogging((int)LOGTYPE.TX, String.Empty, (int)DEFINES.SET1);
                        DK_PCAN_USB.Release();
                        strResponseData = strResultData = "OK";
                        GateWayMsgProcess((int)STATUS.OK, strResponseData, strResultData, String.Empty, true);
                        return;

                    default:

                        if (DK_PCAN_USB.Send(strSendCode, tmpdata, ref strRtnMsg, strName, ref tmpCanMsg, strRecvCode, iSendRecvOption, false, dTimeOut))
                        {
                            if (iSendRecvOption.Equals((int)MODE.SEND))
                            {
                                strResponseData = strResultData = "OK";
                                iStatus = (int)STATUS.OK;
                            }
                            else
                            {
                                if (tmpCanMsg.DATA != null)
                                {
                                    iStatus = (int)STATUS.OK;

                                    switch (strMeasOption)
                                    {
                                        /* 2019 년 6월 12일 김성규선임 요청에 의한 - GEN11 연구소 김정은 선임 메일 답변
                                            SEND: 14DA97F1 - 03 22 45 E9 // Read DID $45E9
                                            RECV: 14DAF197 - 06 62 45 E9 XX YY ZZ // DID $45E9의 BYTE1(YY Bit4 값이 1인 경우 Secure logging enable 상태입니다.
                                        */
                                        case "GBSECURE":
                                            string[] tmpStrarray = System.Text.RegularExpressions.Regex.Split(strMeasPac, " ");
                                            bool bCheckRecvPac = true;
                                            for (int i = 0; i < tmpStrarray.Length; i++)
                                            {
                                                if (!tmpStrarray[i].Equals(tmpCanMsg.DATA[i].ToString("X2")))
                                                {
                                                    iStatus = (int)STATUS.NG;
                                                    strResponseData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                                    strResultData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                                    bCheckRecvPac = false;
                                                    break;
                                                }
                                            }

                                            if (bCheckRecvPac)
                                                strResultData = strResponseData = Convert.ToString(tmpCanMsg.DATA[5], 2).PadLeft(8, '0');
                                            break;
                                        case "GBSECURE_00":
                                            //RECV: 14DAF197 - 04 62 5F F0 XX YY ZZ // DID $45E9의 BYTE1(YY Bit4 값이 1인 경우 Secure logging enable 상태입니다.
                                            string[] tmpStrarray1 = System.Text.RegularExpressions.Regex.Split(strMeasPac, " ");
                                            
                                            bool bCheckRecvPac1 = true;
                                            for (int i = 0; i < tmpStrarray1.Length; i++)
                                            {
                                                if (!tmpStrarray1[i].Equals(tmpCanMsg.DATA[i].ToString("X2")))
                                                {
                                                    iStatus = (int)STATUS.NG;
                                                    strResponseData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                                    strResultData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                                    bCheckRecvPac = false;
                                                    break;
                                                }
                                            }

                                            if (bCheckRecvPac1)
                                                strResultData = strResponseData = Convert.ToString(tmpCanMsg.DATA[4], 2).PadLeft(8, '0');
                                            break;

                                        default:
                                            strResponseData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                            strResultData = BitConverter.ToString(tmpCanMsg.DATA).Replace("-", " ");
                                            break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            strResponseData = strResultData = strRtnMsg;
                            iStatus = (int)STATUS.NG;
                        }

                        if (strName.Equals("PSA_TEST") || strName.Equals("MCTM_LSCAN_TEST")) 
                            DK_PCAN_USB.PauseThreadObject(false);
                        else
                            DK_PCAN_USB.Release();
                        break;
                }

            }
            else
            {
                strResponseData = strResultData = "Initialize Fail : " + strMsg;
                
                iStatus = (int)STATUS.NG;                
                strSendPacket = strName;            
               
            }
            switch (iStatus)
            {
                case (int)STATUS.NG: 
                                    if (strResponseData.Contains("TIME OUT"))
                                    {
                                        ExcutePCAN_Reconnect(LstJOB_CMD[iJobNumber].TYPE);
                                    }
                                    GateWayMsgProcess(iStatus, strResponseData, strResultData, String.Empty, true);
                                    break;

                case (int)STATUS.OK: GateWayMsgProcess(iStatus, strResponseData, strResultData, String.Empty, false); break;
            }
                                   
            return;
        }

        private void ExcuteVECTOR(string strName, int iJobNumber)
        {
            int iSendRecvOption = ConvertOption(LstJOB_CMD[iJobNumber].OPTION);
            string strPac = String.Empty;
            string strSendCode = String.Empty;
            string strRecvCode = String.Empty;
            uint uBaudrate = (uint)VectorBaudrate.BAUD_500K;
            bool bFind = false;
            TBLDATA0 tmpCanTBL = new TBLDATA0();
            for (int i = 0; i < LstTBL_VECTOR.Count; i++)
            {
                if (LstTBL_VECTOR[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_VECTOR[i].SENDPAC;
                    strSendCode = LstTBL_VECTOR[i].RECVPAC;
                    strRecvCode = LstTBL_VECTOR[i].PARPAC1;

                    try
                    {
                        if (LstTBL_VECTOR[i].PARPAC2.Equals("HIGH"))
                            uBaudrate = (uint)VectorBaudrate.BAUD_500K;
                        if (LstTBL_VECTOR[i].PARPAC2.Equals("LOW"))
                            uBaudrate = (uint)VectorBaudrate.BAUD_125K;

                    }
                    catch { uBaudrate = (uint)VectorBaudrate.BAUD_500K; }
                    tmpCanTBL = LstTBL_VECTOR[i];
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Unknown Command.", "NO DATA.", String.Empty, true);
                return;
            }

                
            if (OpenVector != (int)STATUS.OK)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Device Disconnected.", "Device Disconnected.", String.Empty, true);
                return;
            }

            string[] tmpdata = System.Text.RegularExpressions.Regex.Split(strPac, " ");

            if (tmpdata.Length == 0 || tmpdata.Length > 8)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Command Error.", "NO DATA.", String.Empty, true);
                return;
            }

            string strSendPacket = strName + "," + strSendCode + "," + strPac;
            string strResponseData = "";
            string strResultData = "";
            int iStatus = 0;

            string strMsg = "OK";

            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode)
            {
                GateWayMsgProcess((int)STATUS.NG, "USER STOP.", "NO DATA.", String.Empty, true);
                return;
            }

            if (DKVECTOR.Initialize(ref strMsg, uBaudrate))
            {
                string strRtnMsg = String.Empty;
                XLClass.xl_can_msg tmpCanMsg = new XLClass.xl_can_msg();
                string strErrMsg = String.Empty;               

                switch (strName)
                {

                    case "GEN11_GB_BOOT":
                    case "GEN11_GB_BOOT_MY23":
                    case "GEN11_GB_END":
                    case "GEN11_GEM_END":
                    case "GEN11_GEM_BOOT":
                    case "GEN12_GB_BOOT":
                    case "GEN12_GB_BOOT_MY23":
                    case "GEN12_GB_END":
                    case "GEN12_GEM_END":
                    case "GEN12_GEM_BOOT":
                        if (DKVECTOR.ContinuesSending(tmpCanTBL, ref strErrMsg))
                            iStatus = (int)STATUS.OK;
                        else
                            iStatus = (int)STATUS.NG;
                        break;
                    default:
                        
                        if(strName.Contains("_CONTINUOS_")) //연속메시지인경우.
                        {
                            if(DKVECTOR.ContinuesSending(tmpCanTBL, ref strErrMsg))
                                iStatus = (int)STATUS.OK;
                            else
                                iStatus = (int)STATUS.NG;
                        }
                        else
                        {
                            if (DKVECTOR.Send(strSendCode, tmpdata, ref strRtnMsg, strName, ref tmpCanMsg, strRecvCode, iSendRecvOption, false))
                            {
                                if (tmpCanMsg.data != null)
                                {
                                    strResponseData = BitConverter.ToString(tmpCanMsg.data).Replace("-", " ");
                                    strResultData = BitConverter.ToString(tmpCanMsg.data).Replace("-", " ");
                                }
                                iStatus = (int)STATUS.OK;

                            }
                            else
                            {
                                strResponseData = strResultData = strRtnMsg;
                                iStatus = (int)STATUS.NG;
                            }
                        }
                        

                        break;
                }

            }
            else
            {
                strResponseData = strResultData = "Initialize Fail : " + strMsg;

                iStatus = (int)STATUS.NG;
                strSendPacket = strName;

            }
            switch (iStatus)
            {
                case (int)STATUS.NG:                    
                    GateWayMsgProcess(iStatus, strResponseData, strResultData, String.Empty, true);
                    break;

                case (int)STATUS.OK: 
                    GateWayMsgProcess(iStatus, strResponseData, strResultData, String.Empty, false); 
                    break;
            }

            return;
        }

        public bool Excute5515C(string strCmdName, ref string strSendpac, string strSendparEx, int iJobNumber)
        {
            int iRefPos = -1;
            if (FindPac5515C(strCmdName, ref strSendpac, ref iRefPos))
            {
                if (LstJOB_CMD[iJobNumber].DELAY.Length > 0)
                {
                    try
                    {
                        Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                        dtStepDelaySet = DateTime.Now;
                        while (!StepDelayTimeCheck())
                        {
                            if (Item_dStepTimeSet <= 0) break;
                            System.Threading.Thread.Sleep(20);
                            if (!Item_bTestStarted) break;
                            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                        }
                    }
                    catch { }
                }

                COMMDATA resData = new COMMDATA();
                if (DK_GPIB_5515C != null)
                {
                    DK_GPIB_5515C.SendRecv(strSendpac, strCmdName, strSendparEx, ref resData, false, 3.0);
                    string strResult = String.Empty;
                    if (iRefPos > -1)
                    {
                        bool bRes = SplitStringValue(resData.ResponseData, ",", iRefPos, ref resData.ResultData);
                        if (!bRes) resData.iStatus = (int)STATUS.CHECK;
                        try
                        { //tmpDouble.ToString("0.00");
                            double dTmpval = double.Parse(resData.ResultData);
                            if (dTmpval < -9999.0)
                            {
                                resData.ResultData = "-9999.0";
                            }
                            else if (dTmpval > 9999.0)
                            {
                                resData.ResultData = "9999.0";
                            }
                            else
                            {
                                if (dTmpval < 1)
                                {
                                    resData.ResultData = dTmpval.ToString("0.###");
                                }
                                else
                                {
                                    resData.ResultData = dTmpval.ToString("0.###");
                                }

                            }

                        }
                        catch { }

                    }

                }
                else
                {
                    resData.iPortNum = (int)DEFINES.SET1;
                    resData.iStatus = (int)STATUS.CHECK;
                    resData.ResponseData = "Device Disconnected.";
                    resData.ResultData = "NO GPIB";
                    resData.SendPacket = strSendpac;
                }
             
                GateWay_MANAGER(resData);
                return true;
            }
            else return false;

        }

        public bool ExcuteKEITHLEY(string strCmdName, ref string strSendpac, string strSendparEx, int iJobNumber)
        {
            
            string strRecvType = String.Empty;

            switch(strCmdName)
            {
                case "AUTO_VOLTAGE_CH1": return ExcuteKEITHLEY_Sub(strCmdName, ref strSendpac, strSendparEx, iJobNumber,1);
                case "AUTO_VOLTAGE_CH2": return ExcuteKEITHLEY_Sub(strCmdName, ref strSendpac, strSendparEx, iJobNumber,2);
                default: break;

            }            

            if (FindPacKEITHLEY(strCmdName, ref strSendpac, ref strRecvType))
            {
                if (strSendpac.Contains("<DATA>"))
                {
                    if (String.IsNullOrEmpty(strSendparEx))
                    {
                        GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                        return true;
                    }
                    else
                    {
                        strSendpac = strSendpac.Replace("<DATA>", strSendparEx);
                        strSendparEx = String.Empty;
                    }
                }
                
                if (LstJOB_CMD[iJobNumber].DELAY.Length > 0)
                {
                    try
                    {
                        Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                        dtStepDelaySet = DateTime.Now;
                        while (!StepDelayTimeCheck())
                        {
                            if (Item_dStepTimeSet <= 0) break;
                            System.Threading.Thread.Sleep(20);
                            if (!Item_bTestStarted) break;
                            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                        }
                    }
                    catch { }
                }

                COMMDATA resData = new COMMDATA();
                if (DK_GPIB_KEITHLEY != null)
                {
                    DK_GPIB_KEITHLEY.SendRecv(strSendpac, strCmdName, strSendparEx, ref resData, false, 3.0);

                    if (resData.iStatus.Equals((int)STATUS.OK))
                    {
                        switch (strRecvType)
                        {
                            case "DOUBLE":
                                double dresVal = 0;
                                try
                                {
                                    dresVal = double.Parse(resData.ResponseData);
                                }
                                catch
                                {
                                    dresVal = -99999;
                                    resData.iStatus = (int)STATUS.CHECK;

                                }
                                resData.ResponseData = resData.ResultData = dresVal.ToString("0.#######");                                
                                break;

                            case "DEFAULT":
                            default:
                                break;
                        }
                    }
                    
                }
                else
                {
                    resData.iPortNum = (int)DEFINES.SET1;
                    resData.iStatus = (int)STATUS.CHECK;
                    resData.ResponseData = "Device Disconnected.";
                    resData.ResultData = "NO GPIB";
                    resData.SendPacket = strSendpac;
                }

                GateWay_MANAGER(resData);
                return true;
            }
            else return false;

        }

        private bool ExcuteKEITHLEY_Sub(string strCmdName, ref string strSendpac, string strSendparEx, int iJobNumber, int iChannel)
        {

            string strCh = "_CH1";
            if(iChannel.Equals(2)) strCh = "_CH2";

            string[] strCommandList = new string[6]; //1.off, set volt , on, read, set volt, read
            
            strCommandList[0] = "OUTPUT_OFF"   + strCh;
            strCommandList[1] = "SET_VOLTAGE"  + strCh;
            strCommandList[2] = "OUTPUT_ON"    + strCh;
            strCommandList[3] = "MEAS_VOLTAGE" + strCh;
            strCommandList[4] = "SET_VOLTAGE"  + strCh;
            strCommandList[5] = "MEAS_VOLTAGE" + strCh;

            if (LstJOB_CMD[iJobNumber].DELAY.Length > 0)
            {
                try
                {
                    Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                    dtStepDelaySet = DateTime.Now;
                    while (!StepDelayTimeCheck())
                    {
                        if (Item_dStepTimeSet <= 0) break;
                        System.Threading.Thread.Sleep(20);
                        if (!Item_bTestStarted) break;
                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                    }
                }
                catch { }
            }

            double dSettingVoltage = 0;
            double dUserVoltage    = 0;    
            double dCurrentVoltage = 0;
            string strVoltage = strSendparEx;
            strSendparEx = String.Empty;

            if(String.IsNullOrEmpty(strVoltage))
            {
                GateWayMsgProcess((int)STATUS.CHECK, "PAR1 ERROR", "PAR1 ERROR", String.Empty, true);
                return true;
            }
            else
            {
                try
                {
                    dSettingVoltage = dUserVoltage = double.Parse(strVoltage);
                }
                catch
                {
            	    GateWayMsgProcess((int)STATUS.CHECK, "SET VOLTAGE ERROR", "SET VOLTAGE ERROR", String.Empty, true);
                    return true;
                }
            }

            if (DK_GPIB_KEITHLEY == null)
            {
                GateWayMsgProcess((int)STATUS.CHECK, "Device Disconnected.", "NO GPIB", String.Empty, true);
                return true;
            }

            COMMDATA resData = new COMMDATA();
            string strRecvType = String.Empty; 
            for(int i = 0; i < strCommandList.Length; i++)
            {
                resData = new COMMDATA();

                System.Threading.Thread.Sleep(100);

                if (FindPacKEITHLEY(strCommandList[i], ref strSendpac, ref strRecvType))
                {
                    if (strSendpac.Contains("<DATA>"))
                    {     
                        strSendpac = strSendpac.Replace("<DATA>", strVoltage);                        
                    }         
           
                    DK_GPIB_KEITHLEY.SendRecv(strSendpac, strCommandList[i], strSendparEx, ref resData, false, 3.0);

                    if (resData.iStatus.Equals((int)STATUS.OK))
                    {
                        switch (strRecvType)
                        {
                            case "DOUBLE":
                                double dresVal = 0;
                                
                                try
                                {
                                    dresVal = double.Parse(resData.ResponseData);
                                    switch(i)
                                    {
                                        case 3 : dCurrentVoltage = dresVal; 


                                                if(dSettingVoltage > dCurrentVoltage)
                                                {
                                                    dSettingVoltage = dSettingVoltage + (dSettingVoltage - dCurrentVoltage);
                                                }
                                                else if(dSettingVoltage < dCurrentVoltage)
                                                {
                                                    dSettingVoltage = dSettingVoltage - (dCurrentVoltage - dSettingVoltage);
                                                }
                                                else
                                                {
                                                    GateWayMsgProcess((int)STATUS.OK, "ALREADY COMPLETE", "ALREADY COMPLETE", String.Empty, true);
                                                    return true;
                                                }
                                                // 0V 이하 검사1
                                                if (dCurrentVoltage <= 0)
                                                {
                                                    GateWayMsgProcess((int)STATUS.NG, "UNDER VOLTAGE : " + dCurrentVoltage.ToString("0.0000"), "UNDER VOLTAGE : " + dCurrentVoltage.ToString("0.0000"), String.Empty, true);
                                                    return true;
                                                }
                                                //10% 검사
                                                double dUnder = Math.Abs(dUserVoltage - (dUserVoltage * 0.1));
                                                double dOver  = Math.Abs(dUserVoltage + (dUserVoltage * 0.1));
                                                if ( dUnder >= dSettingVoltage ||  dOver <= dSettingVoltage)
                                                {
                                                    GateWayMsgProcess((int)STATUS.NG, "RANGE OVER : " + dCurrentVoltage.ToString("0.0000"), "RANGE OVER : " + dCurrentVoltage.ToString("0.0000"), String.Empty, true);
                                                    return true;
                                                }

                                                // 0V 이하 검사2
                                                if (dSettingVoltage <= 0)
                                                {
                                                    GateWayMsgProcess((int)STATUS.NG, "ERROR SET VOLTAGE : " + dSettingVoltage.ToString("0.0000"), "ERROR SET VOLTAGE : " + dSettingVoltage.ToString("0.0000"), String.Empty, true);
                                                    return true;
                                                }

                                                try
                                                {
                                                    strVoltage = dSettingVoltage.ToString("0.0000");
                                                }
                                                catch
                                                {
                                                	GateWayMsgProcess((int)STATUS.CHECK, "Auto OffSet Failed.", "Auto OffSet Failed.", String.Empty, true);
                                                    return true;
                                                }
                                                break;

                                        default: break;
                                    }
                                        

                                }
                                catch
                                {                                    
                                    GateWayMsgProcess((int)STATUS.CHECK, "Auto OffSet Error.", "Auto OffSet Error.", String.Empty, true);
                                    return true;

                                }
                                
                                break;

                            case "DEFAULT":
                            default:
                                break;
                        }
                    }
                    else
                    {
                        GateWayMsgProcess((int)STATUS.NG, resData.ResponseData, resData.ResponseData, String.Empty, true);
                        return true;
                    }
                }
                else
                {
                    GateWayMsgProcess((int)STATUS.CHECK, "UNKNOWN COMMAND ERROR", "UNKNOWN COMMAND ERROR", String.Empty, true);
                    return true;
                }
            }

            GateWayMsgProcess((int)STATUS.OK, "COMPLETE : " + strVoltage, "COMPLETE : " + strVoltage, String.Empty, true);
            return true;
        }

        public bool ExcuteMTP200(string strCmdName, ref string strSendpac, string strSendparEx, int iJobNumber)
        {
            COMMDATA resData = new COMMDATA();
            double dTimeOut = 1;
            try
            {
                dTimeOut = double.Parse(LstJOB_CMD[iJobNumber].TIMEOUT);
            }
            catch
            {
                resData.iPortNum = (int)DEFINES.SET1;
                resData.iStatus = (int)STATUS.CHECK;
                resData.ResponseData = "Error TimeOut Value";
                resData.ResultData = "Error TimeOut Value";
                resData.SendPacket = strSendpac;
                GateWay_MANAGER(resData);
                return false;
            }

            int iRefPos = -1;
            if (FindPacMTP200(strCmdName, ref strSendpac))
            {
                if (strSendpac != null && strSendpac.Contains("<DATA>"))
                {
                    if (String.IsNullOrEmpty(strSendparEx))
                    {
                        resData.iPortNum = (int)DEFINES.SET1;
                        resData.iStatus = (int)STATUS.CHECK;
                        resData.ResponseData = "INVALID PAR1 VALUE";
                        resData.ResultData = "INVALID PAR1 VALUE";
                        resData.SendPacket = strSendpac;
                        GateWay_MANAGER(resData);
                        return false;
                    }
                    strSendpac = strSendpac.Replace("<DATA>", strSendparEx);
                }
                if (LstJOB_CMD[iJobNumber].DELAY.Length > 0)
                {
                    try
                    {
                        Item_dStepTimeSet = double.Parse(LstJOB_CMD[iJobNumber].DELAY);
                        dtStepDelaySet = DateTime.Now;
                        while (!StepDelayTimeCheck())
                        {
                            if (Item_dStepTimeSet <= 0) break;
                            System.Threading.Thread.Sleep(20);
                            if (!Item_bTestStarted) break;
                            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                        }
                    }
                    catch { }
                }


                if (DK_GPIB_MTP200 != null)
                {
                    DK_GPIB_MTP200.SendRecv(strSendpac, strCmdName, "", ref resData, true, dTimeOut); //MTP200 의 파라미터는 
                    string strResult = String.Empty;
                    if (iRefPos > -1)
                    {
                        bool bRes = SplitStringValue(resData.ResponseData, ",", iRefPos, ref resData.ResultData);
                        if (!bRes) resData.iStatus = (int)STATUS.CHECK;
                        try
                        { //tmpDouble.ToString("0.00");
                            double dTmpval = double.Parse(resData.ResultData);
                            if (dTmpval < -9999.0)
                            {
                                resData.ResultData = "-9999.0";
                            }
                            else if (dTmpval > 9999.0)
                            {
                                resData.ResultData = "9999.0";
                            }
                            else
                            {
                                if (dTmpval < 1)
                                {
                                    resData.ResultData = dTmpval.ToString("0.###");
                                }
                                else
                                {
                                    resData.ResultData = dTmpval.ToString("0.###");
                                }

                            }

                        }
                        catch { }

                    }

                }
                else
                {
                    resData.iPortNum = (int)DEFINES.SET1;
                    resData.iStatus = (int)STATUS.CHECK;
                    resData.ResponseData = "Device Disconnected.";
                    resData.ResultData = "NO GPIB";
                    resData.SendPacket = strSendpac;
                }
                if (resData.iStatus == (int)STATUS.CHECK)
                {
                    resData.iStatus = (int)STATUS.CHECK;
                }
                GateWay_MANAGER(resData);
                return true;
            }

            else
            {
                resData.iPortNum = (int)DEFINES.SET1;
                resData.iStatus = (int)STATUS.CHECK;
                resData.ResponseData = "Can not find command.";
                resData.ResultData = "Invalid Command";
                resData.SendPacket = strSendpac;
                GateWay_MANAGER(resData);
                return false;
            }

        }

        //5515c
        public bool FindPac5515C(string strName, ref string strPac, ref int iReffencePos)
        {
            for (int i = 0; i < LstTBL_5515c.Count; i++)
            {
                if (LstTBL_5515c[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_5515c[i].SENDPAC;
                    try
                    {
                        iReffencePos = int.Parse(LstTBL_5515c[i].PARPAC1);
                    }
                    catch (Exception ex)
                    {
                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                        STEPMANAGER_VALUE.DebugView(strExMsg);
                        iReffencePos = -1;
                    }                    
                    return true;
                }
            }
            return false;
        }

        //KEITHLEY
        public bool FindPacKEITHLEY(string strName, ref string strPac, ref string strRecvType)
        {
            for (int i = 0; i < LstTBL_KEITHLEY.Count; i++)
            {
                if (LstTBL_KEITHLEY[i].CMDNAME.Equals(strName))
                {
                    strPac      = LstTBL_KEITHLEY[i].SENDPAC;
                    strRecvType = LstTBL_KEITHLEY[i].RECVPAC;   
                    return true;
                }
            }
            return false;
        }
        
        //MTP200
        public bool FindPacMTP200(string strName, ref string strPac)
        {
            for (int i = 0; i < LstTBL_MTP200.Count; i++)
            {
                if (LstTBL_MTP200[i].CMDNAME.Equals(strName))
                {
                    strPac = LstTBL_MTP200[i].SENDPAC;
                    return true;                    
                }
            }
            return false;
        }

        //34410A
        public bool FindPac34410A(string strName, ref string strPac, ref string strSendpacOption)
        {
            for (int i = 0; i < LstTBL_34410A.Count; i++)
            {
                if (String.Compare(LstTBL_34410A[i].CMDNAME.ToString(), strName.ToString()) == 0)
                {
                    strPac = LstTBL_34410A[i].SENDPAC;
                    strSendpacOption = LstTBL_34410A[i].RECVPAC;
                    return true;
                }
            }
            return false;
        }

        private void TableFileLoad()
        {
            DKLoggerPC.DeleteScreen(); //3개월 이상의 스크린샷 자료삭제

            if (!DKLoggerPC.LoadTBL0("GEN9.TBL", ref LstTBL_GEN9))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("GEN10.TBL", ref LstTBL_GEN10))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("TCP.TBL", ref LstTBL_TCP))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("GEN11.TBL", ref LstTBL_GEN11))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("GEN11P.TBL", ref LstTBL_GEN11P))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("GEN12.TBL", ref LstTBL_GEN12))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("MCTM.TBL", ref LstTBL_MCTM))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("CCM.TBL", ref LstTBL_CCM))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("NAD.TBL", ref LstTBL_NAD))
            {
                return;
            }
            
            if (!DKLoggerPC.LoadTBL0("ATT_TCP.TBL", ref LstTBL_ATT))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("DIO_VCP.TBL", ref LstTBL_DIO))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("5515C.TBL", ref LstTBL_5515c))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("KEITHLEY.TBL", ref LstTBL_KEITHLEY))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("PCAN.TBL", ref LstTBL_PCAN))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("VECTOR.TBL", ref LstTBL_VECTOR))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("TC3000.TBL", ref LstTBL_TC3000))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("MTP200.TBL", ref LstTBL_MTP200))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("AUDIOSELECOTOR.TBL", ref LstTBL_AUDIO))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("ADCMODULE.TBL", ref LstTBL_ADC))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("34410A.TBL", ref LstTBL_34410A))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("TC1400A.TBL", ref LstTBL_TC1400A))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("ODA.TBL", ref LstTBL_PWR))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("DLLGATE.TBL", ref LstTBL_DLLGATE))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("MELSEC.TBL", ref LstTBL_MELSEC))
            {
                return;
            }

            if (!DKLoggerPC.LoadTBL0("MTP120A.TBL", ref LstTBL_MTP120A))
            {
                return;
            }
            MakeLoggingHiddenList(); //히든명령 리스트 만들기

        }

        public  string[] GetFileList(string strFormat)
        {
            return DKLoggerPC.GetFileList(strFormat);
        }

        public byte[] FileToBinary(string strFileName)
        {
            return DKLoggerPC.FileToBinary(strFileName);
        }

        public  bool LoadSlot(string strSubject, string strName)
        {
            return DKLoggerPC.LoadSlot(strSubject, strName);
        }

        public  void SaveSlot(string strSubject, string strName, string strValue)
        {
            DKLoggerPC.SaveSlot(strSubject, strName, strValue);
        }

        public  string LoadINI(string strSubject, string strName)
        {
            return DKLoggerPC.LoadINI(strSubject, strName);                  
        }

        public bool CheckTBLFile(string strTBLname, ref string strReason)
        {

            switch (strTBLname)
            {
                case "MCTM": return CheckDuplicateTBL(LstTBL_MCTM, ref strReason);

                default: return false;
            }

        }

        private bool CheckDuplicateTBL(List<TBLDATA0> targetTBL, ref string strReason)
        {
            string strPacket = String.Empty;
            string strPacketType = String.Empty;
            string strCmdName = String.Empty;

            for (int i = 0; i < targetTBL.Count; i++)
            {
                if (targetTBL[i].CMDNAME.IndexOf('#', 0, 1).Equals(0) || targetTBL[i].SENDPAC.Equals("INTERNAL PROCESS"))
                {
                    continue;
                }
                strCmdName = targetTBL[i].CMDNAME;
                strPacket = targetTBL[i].SENDPAC;
                strPacketType = targetTBL[i].RECVPAC;
                

                for(int j = i + 1; j < targetTBL.Count; j++)
                {
                    if (targetTBL[j].CMDNAME.IndexOf('#', 0, 1).Equals(0))
                    {
                        continue;
                    }

                    if (strPacket.Equals(targetTBL[j].SENDPAC) && strPacketType.Equals(targetTBL[j].RECVPAC))
                    {
                        strReason = "Dulplicate Command [" + strCmdName + "] [" + targetTBL[j].CMDNAME + "]";

                        return false;
                    }
                }

            }
            strReason = "No Problem";
            return true;
        }      

        public bool CheckMD5(string strVersion, ref string strRequireVersion)
        {
            return DKLoggerPC.CheckMD5(strVersion, ref strRequireVersion);
        }
        
        public string GetConfigPath()
        {
            return DKLoggerPC.GetConfigPath();
        }

        public  void  SaveINI(string strSubject, string strName, string strValue)
        {
            DKLoggerPC.SaveINI(strSubject, strName, strValue); 
        }

        public void DeleteJobFile(string strFileName)
        {
            DKLoggerPC.DeleteJob(strFileName);
        }

        private bool LoadJobProcess(string[] strData, string strCheckSum, ref string strReason)
        {
            // 절차서 상단의 주석내용은 제거
            strReason = String.Empty;
            if (strData[0].Equals("TYPE")) return true;
            if (strData.Length < (int)sIndex.EXPR)
            {
                strReason = "JOB INDEX RANGE ERROR";
                return false;
            }

            // 잡파일 체크섬 검사.            
            string[] strArray = new string[strData.Length - 1];
            Array.Copy(strData, strArray, strArray.Length);
            string strJobCrc = DKLoggerPC.JOB_CRC(strArray);
            if (!strJobCrc.Equals(strCheckSum))
            {
                strReason = "JOB FILE ERROR.(CRC)";
                return false;
            }

            try
            {
                AddJobStruct(strData);
            }
            catch (Exception ex)
            {
                DKLoggerPC.WriteCommLog("[EXCEPTION] LoadJobProcess", ex.Message.ToString(), false);
                strReason = "JOB LOAD ERROR. (LoadJobEventProcess)";
                return false;
            }

            return true;
        }

        public  void SelectChannel(uint iNum, bool bCheck) //멀티 채널을 위해서... 미리 만들어놓음.
        {
            if (iNum > UseSlots.Length) { MessageBox.Show("SLOT RANGE OVER"); return; }
            UseSlots[iNum] = bCheck;
        }

        private void AddJobStruct(string[] strData)
        {
            JOBDATA tmpJob = new JOBDATA();
            
            tmpJob.TYPE         = strData[(int)sIndex.TYPE].ToString();
            tmpJob.CMD          = strData[(int)sIndex.CMD].ToString();
            tmpJob.DISPLAYNAME  = strData[(int)sIndex.DISPLAY].ToString();
            tmpJob.MESCODE      = strData[(int)sIndex.MESCODE].ToString();
            tmpJob.ACTION       = strData[(int)sIndex.ACTION].ToString();
            tmpJob.LABEL        = strData[(int)sIndex.LABEL].ToString();
            tmpJob.LABELCOUNT   = strData[(int)sIndex.LABELCOUNT].ToString();
            tmpJob.CASENG       = strData[(int)sIndex.CASENG].ToString();
            tmpJob.DELAY        = strData[(int)sIndex.DELAY].ToString();
            tmpJob.TIMEOUT      = strData[(int)sIndex.TIMEOUT].ToString();
            tmpJob.RETRY        = strData[(int)sIndex.RETRY].ToString();
            tmpJob.COMPARE      = strData[(int)sIndex.COMPARE].ToString();
            tmpJob.MIN          = strData[(int)sIndex.MIN].ToString();
            tmpJob.MAX          = strData[(int)sIndex.MAX].ToString();
            tmpJob.OPTION       = strData[(int)sIndex.OPTION].ToString();
            tmpJob.PAR1         = strData[(int)sIndex.PAR1].ToString();
            tmpJob.DOC          = strData[(int)sIndex.DOC].ToString();
            tmpJob.EXPR         = strData[(int)sIndex.EXPR].ToString();
            LstJOB_CMD.Add(tmpJob);            
            
        }

        public bool LoadStep(string strFileName, ref string strReason)
        {
            List<JOBFILES> JOBlist = new List<JOBFILES>();

            bool bLoad = DKLoggerPC.LoadStepJobs(strFileName, ref JOBlist);

            if (bLoad && JOBlist.Count > 0)
            {
                for (int i = 0; i < JOBlist.Count; i++)
                {
                    if (!LoadJobProcess(JOBlist[i].strJOB, JOBlist[i].strChkSum, ref strReason))
                    {
                        StringBuilder sbError = new StringBuilder();
                        for (int ix = (int)sIndex.TYPE; ix < (int)sIndex.END; ix++)
                        {
                            sbError.Append((sIndex.TYPE + ix).ToString());
                            sbError.Append(":");
                            sbError.Append(JOBlist[i].strJOB[ix]);
                            sbError.Append("|");
                        }
                        DKLoggerPC.WriteEditHistory(strFileName, strReason, " >> " + sbError.ToString());
                        return false;
                    }
                }

                return true;
            }
            else
            {
                strReason = "ERROR JOB COUNT : 0";
                return false;
            }

        }

        public  void JOBListClear()
        {
            LstJOB_CMD.Clear();
            ClearTestResultData();
            
        }

        public int GetJOBListCount() { return LstJOB_CMD.Count; }

        public bool GetFolderList(string strPath, ref List<string> lstFolder)
        {
            return DKLoggerPC.GetFolderList(strPath, ref lstFolder);
        }

        public string GetLogPath()
        {
            return DKLoggerPC.GetLogPath();
        }

        public  string GetJOBString(int sIdx, int sNum)
        {
            JOBDATA tmpJobdata;
            string rtnStr = String.Empty;

            if (sIdx >= LstJOB_CMD.Count)
            {
                rtnStr = "RangeOver";
                return rtnStr;
            }

            tmpJobdata = LstJOB_CMD[sIdx];

            switch (sNum)
            {
                case (int)sIndex.TYPE:      rtnStr = tmpJobdata.TYPE; break;
                case (int)sIndex.CMD:       rtnStr = tmpJobdata.CMD; break;
                case (int)sIndex.DISPLAY:   rtnStr = tmpJobdata.DISPLAYNAME; break;
                case (int)sIndex.MESCODE:   rtnStr = tmpJobdata.MESCODE; break;
                case (int)sIndex.ACTION:    rtnStr = tmpJobdata.ACTION; break;
                case (int)sIndex.LABEL:     rtnStr = tmpJobdata.LABEL; break;
                case (int)sIndex.LABELCOUNT: rtnStr = tmpJobdata.LABELCOUNT; break;
                case (int)sIndex.CASENG:    rtnStr = tmpJobdata.CASENG; break;
                case (int)sIndex.DELAY:     rtnStr = tmpJobdata.DELAY; break;
                case (int)sIndex.TIMEOUT:   rtnStr = tmpJobdata.TIMEOUT; break;
                case (int)sIndex.RETRY:     rtnStr = tmpJobdata.RETRY; break;
                case (int)sIndex.COMPARE:   rtnStr = tmpJobdata.COMPARE; break;
                case (int)sIndex.MIN:       rtnStr = tmpJobdata.MIN; break;
                case (int)sIndex.MAX:       rtnStr = tmpJobdata.MAX; break;
                case (int)sIndex.OPTION:    rtnStr = tmpJobdata.OPTION; break;
                case (int)sIndex.PAR1:      rtnStr = tmpJobdata.PAR1; break;
                case (int)sIndex.DOC:      rtnStr = tmpJobdata.DOC; break;
                case (int)sIndex.EXPR:      rtnStr = tmpJobdata.EXPR; break;

                default : rtnStr = "UNKNOWN INDEX : " + sNum.ToString(); break;
                    
            }

            return rtnStr;
        }

        public  bool GetLineCommand(int sIdx, ref JOBDATA jData)
        {
            if (sIdx >= LstJOB_CMD.Count) { return false; }

            jData = LstJOB_CMD[sIdx];

            return true;
        }

#endregion

#region TIME OUT 관련

        private bool StepDelayTimeCheck()
        {
            dtStepCurrTime = DateTime.Now;
            TimeSpan tsGpibNow = dtStepCurrTime - dtStepDelaySet;

            if (tsGpibNow.TotalSeconds >= Item_dStepTimeSet)
            {               
                // 타임아웃 데이터 처리.
                return true;
            }
            return false;
        }

#endregion

    }
}
