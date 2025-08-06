using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text; //외부 글꼴 사용
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

#region 프로그램 작성 히스토리
/*
# EXPR , GMES ITEM 의 사용용도 정의
1. max field option - #GMES_ , #EXPR_ - ok
2. par1 field option - #GMES_ , #EXPR_ - ok 
3. expr field option - MEAS 는 현재명령의 응답값
#LOAD:필드이름    : meas 대신 필드이름의 값을 보여줌.
#SAVE:필드이름     : meas 값을 필드이름으로 저장함
#MATH:MEAS+필드이름+0.1   : 수식을 계산하여 최종결과로 보여줌.
--------------------------------------------------------------------------------
*/
#endregion


#region ENUM 정의

enum MTPLOSS
{
    MAX = 15
}

enum PLCADDRESS
{
    MAX = 15
}

enum USERLIMIT
{
    MIN = 0,
    MAX = 18
}
enum MESPRI
{
    ORACLE, GMES, END
}

enum LOGTYPE
{
    TX, RX, PC, END
}

enum EFILETYPE
{
    CHECK, TRANSFER, COMPLETE, END
}

enum NADDLLIDX
{
    NONE, PORT_OPEN, PORT_CLOSE, READ_IMEI, WRITE_IMEI, CHECKSUM_IMEI, NV_RESTORE, READ_MSISDN, READ_ICCID, READ_IMSI, EFS_BACKUP,
    DLL_VERSION, READ_SCNV, NV_GET,
    END
}

enum APNINDEX
{
    szAPNName, iAPNNameSize, iUserNameSize, szUserName, iPasswordSize, szPassword, iRenewalRate,
    szDialString, bHRPD, iAPNIndex, iPDIndex, bIPv4, bIPv6, iRmNetIndex, bReadOK, bAlwaysOn, END
}

enum OOBBARCODE
{
    //1. GM PARTNUMBER             P / 8  / 9
    //2. GM DEFINED VPPS           Y / 14 / 15
    //3. ASSEMBLY SITE DUNS      12V / 9  / 12
    //4. GM TRACE ID               T / 16 / 17
    //5. GM SN                     S / 13 / 14
    //6. GM ONSTAR STATION ID    14Z / 9  / 12
    //7. GM IMEI                 15Z / 14 / 17
    //8. GM ICCID                 2D / 6  / 8
    //9. GM IMSI                  1P / 10 / 12
    //10 Supplier Assembly Data         18자리
    NONE, PARTNUMBER, VPPS, DUNS, TRACE, SN, STID, IMEI, ICCID, IMSI, DATE, MODEL, END
}

enum MELSECRESTYPE
{
    NONE, WORD, BYTE, NUMBER, END
}

enum MCTMBARCODE
{
    //   Y7520400000000X     (VPPS)
    //   P84492806           (GMPN)
    //   12V555343750        (DUNS)
    //   T7117355000002212   (TRACE)
    //   16Z990009610032172  (VZWIMEI)
    //   18Z311480996621219  (VZWIMSI)
    //   17Z89148000003949250661 (VZWICCID)
    //   19Z014977000032170      (ATNTIMEI)
    //   21Z310170113484583      (ATNTIMSI)
    //   20Z89011702271134845832 (ATNTICCID)
    //   22Z014978000032178      (TMUSIMEI)
    //   24Z310260986705064      (TMUSIMSI)
    //   23Z8901260982767050649  (TMUSICCID) 
    //   2D122117                (ASSMDATE)
    //   1PTCAA19ANANN           (LGPN)
    //   S712VIBB474669          (LGSN)
    NONE, VPPS, GMPN, DUNS, TRACE, VZWIMEI, VZWIMSI, VZWICCID,
    ATNTIMEI, ATNTIMSI, ATNTICCID, TMUSIMEI, TMUSIMSI, TMUSICCID,
    ASSMDATE, LGPN, LGSN, END



}

enum EFILECODE
{
    ErrCode, ErrMsg, STID, rCERT, cCert, vCert, vPri, vPre, vAuth, vHash, END
}

enum TCPRESTYPE
{
    NONE, BYTE, BYTE1, CHAR, INT, DTC, DTCALL, ALDL_ASCII, ALDL_BITS, DOUBLE, SINGLE, TTFF, SIMINFO, NADINFO, REVERSE, OOBRESULT, SERVICE, APN_TABLE, END
}

enum MCTMRESTYPE
{
    NONE, BYTE, CHAR, INT, DTC, DTCBITS, DOUBLE, SINGLE, INT16, INT32, GPS0, GPS1, GPS2, GPS3, TTFF, USIM1, USIM2, USIM3, ALDL_ASCII, ALDL_BITS, ALDL_HEXA, END
}

enum CCMRESTYPE
{
    NONE, SWVERSION, SWCOMFILEDATE, HWREVISION, MDMCHIPVER, EXTCHAR, EXTINT32, NV946, EXTDEFAULT, STDCHAR, QFUSE_NORMAL, NORESPONSE, DTC_RF, DTC_GPS, STDDEFAULT, GPS_NORMAL, GPS_INFO, MODECHANGE, END
}

enum NADRESTYPE
{
    NONE, SWVERSION, SWCOMFILEDATE, HWREVISION, MDMCHIPVER, EXTCHAR, EXTINT32, NV946, EXTDEFAULT, STDCHAR, QFUSE_NORMAL, NORESPONSE, DTC_RF, DTC_GPS, STDDEFAULT, GPS_NORMAL, GPS_INFO, MODECHANGE, 
    
    //For GEN9 NAD
    GEN9IMEI,
    END
}

enum GEN11RESTYPE
{
    NONE, BYTE, BYTE1, CHAR, INT, UINT32A, UINT32B, DTC, DTCGB, DTCGEM, DTCMANUAL, DTCBITS, DTCGBBITS, DTCGEMBITS, DTCALL, ALDL_ASCII, ALDL_HEXA, ALDL_BITS, DOUBLE, SINGLE, TTFF, SIMINFO, NADINFO, REVERSE, OOBRESULT, IMEICHKSUM, BUBCAL, NVITEM, WIFIRX1, WIFIRX2, WIFIRX3, WIFIRX4, WIFIRX5, FEATUREID, SERVICE, END
}

enum GEN12RESTYPE
{
    //원본 ENUM 소스
    //NONE, BYTE, CHAR, INT, DTC, DOUBLE, SINGLE, UINT16, INT16, INT32, BINT16,
    //BYTE1, //딱 한바이트
    //ADC, TTFF, SELFTEST,
    //TEXT,
    //END
    
    // 기존 WAVE ENUM 값에 GEN11에서 사용했던 RESTYPE 추가한 소스
    NONE, BYTE, BYTE1, CHAR, INT, DTC, DOUBLE, SINGLE, TTFF, UINT16, INT16, INT32, BINT16, ADC, SELFTEST, TEXT,
    UINT32A, UINT32B, DTCGB, DTCGEM, DTCMANUAL, DTCBITS, DTCGBBITS, DTCGEMBITS, DTCALL, ALDL_ASCII, ALDL_HEXA, ALDL_BITS,
    SIMINFO, NADINFO, REVERSE, OOBRESULT, IMEICHKSUM, BUBCAL, NVITEM, WIFIRX1, WIFIRX2, WIFIRX3, WIFIRX4, WIFIRX5, FEATUREID, SERVICE,
    ALDL12_ASCII, ALDL12_HEXA,
    END
}

enum GRIDMAININDEX
{
    NO, TESTNAME, RESULT, MIN, MAX, MEASURE, LAPSE, END
}

enum KALSNAME
{
    DID_SEED_GEN11, DID_SEED_MCTM, DID_SEED_GEN11_VCP, GM_GB_GEN11,
    DID_SEED_GEN12, GM_GB_GEN12, DID_SEED_GEN12_VCP, END
}

enum FILESAVE
{
    CREATE, SAVEAS, END
}

enum KALSRETURNCODE
{
    ERR_UNKNOWN,
    ERR_SUCCESS,        ERR_NOT_EXIST,      ERR_EXIST,              ERR_UPDATE_FAIL, 
    ERR_ADODB_FAIL,     ERR_INVALID_INPUT,  ERR_NOT_DEFINED,        ERR_DUP_DATA,   
    ERR_SET_NOT_EXIST,  ERR_KEY_NOT_EXIST,  ERR_WIP_NOT_EXIST,      ERR_DOWNLOAD_FAIL,
    ERR_FILE_NOT_EXIST, ERR_DIFFERENT_SET,  ERR_ALREADY_SET_MAP,    ERR_DIFFERENT_WIP,  
    ERR_ALREADY_WIP_MAP, END
}

enum POPBTNTYPE
{
    NORMAL, CONTINUE, OKNG, END
}

enum POPTYPE
{
    NOPOP, POPONEBTN, POPTWOBTN, END
}

enum SENSOR
{
    OFF, ON, ING
}

enum DOUT
{
    OK, NG, CHK, RDY, END
}

enum DIOPIN
{   
    START, IN1, IN2, IN3, BUB1, BUB2, EXTERNAL, MANUAL3, SETIN, STOP, PINUSE
}

enum COMSERIAL
{
    NONE, DIO, SET, SCANNER, TC3000, AUDIOSEL, ADC, CCM, ODAPWR, UART2, END
}

enum DEVSTATUSNUMBER
{
    DIO, SET, E5515C, SCANNER, PCAN, TC3000, MTP200, AUDIOSEL, ADC, DMM34410A, VECTOR, ODAPWR, KEITHLEY, MELSEC, MTP120A, END
}

enum RS232
{
    TEXT, MOOHANTECH, GEN9BYTE, GEN10BYTE, GEN11BYTE, GEN12BYTE, TCPBYTE, HEXBYTE, BYTE, ATTBYTE, CCMBYTE, GEN11PBYTE, MCTMBYTE, NADBYTE, SCANNER
}

enum EXPRTYPE
{
    SAVE, LOAD, MATH, DEF, CONV, HEXA, ERROR
}

enum JOBLABEL
{
    A1, A2, A3, A4, A5, A6, A7, A8, A9, A10,
    A11, A12, A13, A14, A15, A16, A17, A18, A19, A20,

    C1, C2, C3, C4, C5, C6, C7, C8, C9, C10,
    C11, C12, C13, C14, C15, C16, C17, C18, C19, C20,

    P1, P2, P3, P4, P5, P6, P7, P8, P9, P10,
    P11, P12, P13, P14, P15, P16, P17, P18, P19, P20,

    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10,
    F11, F12, F13, F14, F15, F16, F17, F18, F19, F20
}

enum EVENTTYPE
{
    MANAGER, ACTOR, COMM, STEPPOINTER, ETC
}

enum GEN9NAME
{
    UNKNOWN = -1, 
    GMLAN29 = 0, 
    GMLAN11, CLASS2, HYBRID, GLOBALA, FCP, CN29, CNGA, 
    GSM = 10
}

enum ANALYIZEGEN9  //GEN9 의 경우 NORMAL 과 BLUEGO가 사용된다.
{
    NONE, NORMAL, NORESPONSE, TTFF, DTC, DTCOQA, REVERSE, SIMINFO, BYTE, RESCODE, CONFCODE,
    CHECKSUM, OLDGPSINFO, GPSINFO1, GPSINFO2, GPSINFO3, GPSINFO4, OOBRESULT,
    CRLF1, CRLF2, CRLF3, CRLF4, CRLF5,
    DTC00, DTC01, DTC02, DTC03, DTC04, DTC05, DTC06, DTC07, DTC08, DTC09, DTC10, DTC11, DTC12, DTC13,
    DTC14, DTC15, DTC16, DTC17, DTC18, DTC19, DTC20, DTC21, DTC22, DTC23, DTC24, DTC25, DTC26, DTC27, DTC28,

    DTC00_BITS, DTC01_BITS, DTC02_BITS, DTC03_BITS, DTC04_BITS, DTC05_BITS, DTC06_BITS, DTC07_BITS, DTC08_BITS, DTC09_BITS,
    DTC10_BITS, DTC11_BITS, DTC12_BITS, DTC13_BITS, DTC14_BITS, DTC15_BITS, DTC16_BITS, DTC17_BITS, DTC18_BITS, DTC19_BITS,
    DTC20_BITS, DTC21_BITS, DTC22_BITS, DTC23_BITS, DTC24_BITS, DTC25_BITS, DTC26_BITS, DTC27_BITS, DTC28_BITS,

    PLATFORM1, PLATFORM2, PLATFORM3, BTMAC,
    ALDL_ASCII, ALDL_HEX, ALDL_DECIMAL, ALDL_BITS,

    ST_ESN_MEID, ST_IMEI, ST_ICCID,
    MDN_COUNTRY, MDN_MIN, MDN_MDN, MDN_HOMESID, MDN_COUNT,
    
    MDN_SID_00, MDN_SID_01, MDN_SID_02, MDN_SID_03, MDN_SID_04, MDN_SID_05,
    MDN_SID_06, MDN_SID_07, MDN_SID_08, MDN_SID_09, MDN_SID_10,

    MDN_NID_00, MDN_NID_01, MDN_NID_02, MDN_NID_03, MDN_NID_04, MDN_NID_05,
    MDN_NID_06, MDN_NID_07, MDN_NID_08, MDN_NID_09, MDN_NID_10,

    NONEED, GEN9_IMSI, GEN9_ICCID, GEN9_APN, GEN9_APN_PW, GEN9_NAI,
    FINFOSIZE,
    END
}

enum ANALYIZEGEN10  //GEN10 의 경우 NORMAL 과 BLUEGO가 사용된다.
{
    NONE, NORMAL, BLUEGO, NORESPONSE, TTFF, ALDL_ASCII, ALDL_BITS, DTC, DTCOQA, REVERSE, SIMINFO, BYTE, RESCODE, T_SINGLE, T_NADINFO,
    CHECKSUM, FINFOSIZE, FINFOBYTE, OLDGPSINFO, GPSINFO1, GPSINFO2, GPSINFO3, GPSINFO4, OOBRESULT, BINPACK, SERVICE, BYTEPACK, 
    APN_TABLE, END
}

enum PEPUCMD
{
    TCP_CHECK, VCP_CHECK, TCP_MANUAL, VCP_MANUAL, END

}

enum GEN10MFGINDEX
{
    IMEI, IMSI, STID, HASH, TRACE, DUMMY, OOB_TEST, MODEL, ICCID

}

enum CLSMODE
{
    NORMAL, FACTORY, GEN9HIGH, FCP
}

enum STATUS
{
    NONE, RUNNING, OK, NG, CHECK, SKIP, TIMEOUT, NOTUSE, NULL, TESTDONE,
    STOP, NGSTOP, GOTO_ERR, EMPTY,
    BTNSTART, BTNSPARE1, BTNSPARE2, BTNMUTE, BTNM1, BTNM2, BTNBUB, BTNPALETE, BTNSET,
    BTNSTOP, READY, MESERR, IGNORE, GOTORETRYFAIL, POPPING, POPPINGOFF, EJECT, PINSELECTSTART,
    CHANGEJOB, ERROR, DELAYLAPSE, DSUB, TIMESTAMP, JUMP_ERR, JUMP, END
}

enum ENDOPTION
{
    RESULTOK, RESULTNG, RESULTCHK, USERSTOP, RESULTEMPTY, RESULTMES, RESULTEJECT, RESULTERROR, END
}

enum PCANBUS
{
    HEAVY, LIGHT, OFF, OK, RESET
}

enum ACTION
{
    RUN, SKIP
}

enum MODE
{
    SENDRECV, SEND, RECV, BUFFER, UNTIL, AVERAGE, RECVSEND, MULTIPLE, NORESPONSE
}
enum DEFINES
{
    SET0, SET1, END
}

enum sIndex
{
    TYPE, CMD, DISPLAY, MESCODE, ACTION, LABEL, LABELCOUNT, CASENG, DELAY,
    TIMEOUT, RETRY, COMPARE, MIN, MAX, OPTION, PAR1, DOC, EXPR, END
}

enum DICINDEX
{
    SEQUENCE, RESULTDT, RESPONSE, SENDPACK, ALL, END
}

enum SimInfoIndex
{
    efUST, efACC, efECC, efFPLMN, efHPPLMN, efIMSI, efKEYS, efKEYSPS, efLOCI,
    efPSLOCI, efARR, efSTART_HFN, efTHRESHOLD, efAD, eSimVer, QualcommChipRev, eSimVer_PROFILE, eSimVer_NSPIF, eSimVer_VVN, END
}

enum ServiceIndexA
{
    //GEN10, GEN10 TCP 용
    szMEID, szICCID, cRadioIF, nActiveChannel, nCurrentSID, iTxPwr, cCallState, szDialedDigits, nPilotPN, cServiceDomain, cRSSI,  
    nErrorRate, cDTMFEvent, nServiceOption, cServiceStatus, szIMEI, cCSAttachState, cPSAttachState, nMCC, nMNC, nArfcn, cBSIC,
    cECIO, iCellID, nUARFCN, cAttachState, nEARFCN, nSNR, nRSRP, szBanner, nTAC, iVocoder, iVocoderRate, iRSRP, cBand_1900,
    nLAC, szMDN, iServiceDomainPref, iScanList, bIsVoLTE, szIncomingNumber, cSINR, cRSRQ, END
}

enum ServiceIndexB
{
    //GEN11 용
    szMEID, szICCID, cRadioIF, nActiveChannel, nCurrentSID, iTxPwr, cCallState, szDialedDigits, nPilotPN, cServiceDomain, cRSSI,
    nErrorRate, cDTMFEvent, nServiceOption, cServiceStatus, szIMEI, cCSAttachState, cPSAttachState, nMCC, nMNC, iArfcn, iBSIC,
    cECIO, iCellID, iUARFCN, cAttachState, iEARFCN, iLteSNR, iLteRSRP, szBanner, nTAC, iVocoder, iVocoderRate, iRSRP, bBand_1900,
    nLAC, szMDN, iServiceDomainPref, iScanList, bIsVoLTE, szIncomingNumber, cSINR, iRSRQ, END
}

enum AMSRXSTEP
{
    call, error_code, error_message, stid, rCert, ccCert, vCert, vPri, vPre, vAuth, vHash, END
}

enum AMSTYPE
{
    GEN12, ETC, END
}

enum WHCODE
{
    NONE, N9V, N9X, TBD, END
}
#endregion



namespace GmTelematics
{

    public partial class FrmFaMain : Form
    {
#region 변수 선언 및 초기화
        
        private DK_PLAYCHECKER DKPlayChecker;
        private DK_STEPMANAGER DKStepManager;
        private System.Threading.Timer threadJobChangeTimer;
        private string strJobChangeFileName;

        private DK_MOTOROLA_SCANNER DKMOTOSCAN;
        private DK_SOCKET_CLIENT DKSckClt;                   //2016.02.15 일 추가. 
        private System.Threading.Timer threadMonitorTimer;   //2016.02.15 일 추가. 
        private bool bForceExit;                             //원격에 의한 강제종료시.

        //private FrmMsgPop      frmMsgWarn = null;
        private FrmEdit        frmEdit;        //에디트 화면
        private FrmConfig      FrmConfig;      //컨피그 화면
        //private int[][]        iLastResult;    //최종결과 플래그
        private Dictionary<int, int> iLastResult; //= new Dictionary<int, int>[(int)DEFINES.END];

        private int            iLastResultIdx; //최종결과 플래그
        
        private string strStartTime; //검사 시작시간
        private string strEndTime;   //검사 종료시간       

        //FrmLogMsg frmLogWindow;
        private const Int32  WM_GMESDLLDATA = 0x400 + 2000;
        private const string devDIO = "DIO";
        private const string devSET = "SET";
        private const string dev5515C = "5515C";
        private const string devScanner = "SCANNER";
        private const string devPcan    = "PCAN";
        private const string devVector = "VECTOR";
        private const string devTC3000 = "TC3000";
        private const string devMTP200 = "MTP200";
        private const string devAudio  = "AUDIO";
        private const string devADC    = "ADC";
        private const string dev34410A = "34410A";
        private const string devODA    = "ODA";
        private const string devKEITHLEY = "KEITHLEY";
        private const string devMELSEC = "MELSEC";
        private const string devMTP120A = "MTP120A";

        private const string constDate  = "DATE";
        private const string constMonth = "MONTH";
        private const string constDay   = "DAY";
        
        private const string constOption = "OPTION";
        private const string constStartBarcode = "STARTBARCODE";
        private const string constUseBarcode = "USEBARCODE";
        private const string constUseSubId   = "USESUBID";
        private const string constOOBbarcode = "OOBBARCODETYPE";
        private const string constCountPass = "COUNTPASS";
        private const string constCountFail = "COUNTFAIL";
        private const string constCountTotal = "COUNTTOTAL";
        private const string constCountWarn = "WARNING";
        private const string constWipLength = "WIPLENTH";
        private const string constSubIdLength = "SUBIDLENTH";
        private const string constDefaultUI = "DEFAULTSCREEN";

        private const string constPrimary = "PRIMARY";




        private bool bPassRate = false; //TOTAL 을 성공율로..
        private bool bUseResetCountTime = false; //카운트가 리셋되는 타임반영유무

        private int iPass = 0;
        private int iFail = 0;
        private int iTotal = 0;
        private int iWipSize = 15;
        private int iSubIdSize = 0; //지그 바코드 format은 YYYYMMDDNNNN 입니다.
        private double dRunningValue = 0;

        private int iSeqTotalCount = 0;
        private bool[] bSkipList;  

        //CABLE COUNT
        private bool[] bUseCable = new bool[5];
        private int iWarn = 90;
        private int[] iCableSpec = new int[5];
        private int[] iCableUsage = new int[5];

        private System.Threading.Thread InterThread;  //구간인터렉티브용 스레드.
        private System.Threading.Thread InterSignalThread;  //PLC용 스레드.
        private System.Threading.Thread RepeatThread;       //반복용스레드

        System.Threading.Thread thdPing;
        System.Threading.Thread thdOrac;
        
        private string strClientMyname = "0";
        private string strClientMyIP = "0";

        private bool bUIFlag = true;

        private Graphics g1;// = new Graphics();
        private Graphics g2;

        private string strGetUseBarcode     = "";
        private string strGetUseSubId       = "";
        private string strGetStartBarcode   = "";
        private string strGetWipSize        = "";
        private string strGetSubIdSize      = "";
        private string strGetOOBBarcode     = "";

        private bool bRepeatMode = false;

        private void MainFormSleepFuction(int iMillisec)
        {            
            Application.DoEvents();
            System.Threading.Thread.Sleep(iMillisec);
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (STEPMANAGER_VALUE.bProgramRun)
            {
                DKStepManager.SaveINI("RUNTIME", "KILLME", "NOW");
            }
            else
            {
                return;
            }
        }
        
        private void InitValue()
        {
            //예외처리 되지 못한 예외를 처리하는곳...조낸 대박이다. 이거 남용하면 안되는뎅.
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            dataGridView1.DoubleBuffered(true);
            
            DKPlayChecker = new DK_PLAYCHECKER();
            DKPlayChecker.SendTime   += new EventTimer(TimerView);
            bForceExit = false;

            DKStepManager = new DK_STEPMANAGER(this.Handle);
            DKStepManager.ManagerSendReport  += new EventMANAGER(GateWay_MAINfrm);
            DKStepManager.ManagerSendReport2 += new EventRealTimeMsg(GateWay_MAINfrm2);
            DKStepManager.ManagerSendReport3 += new EventDeviceStatus(GateWay_MAINfrm3);
            DKStepManager.ManagerSendThredEvent += new EventThreadStatus(GateWay_Debug);
            DKStepManager.EvSetJumpResult += new EventSetJumpResult(SetJumpResult);
            DKStepManager.EvGotoTestGridRow += new EventGotoTestGridRow(GotoTestGridRow);

            iLastResultIdx = 1;           

            STEPMANAGER_VALUE.bNowMsgPop = false;
            STEPMANAGER_VALUE.InitCcmGpsStructure();
            threadJobChangeTimer = new System.Threading.Timer(CallBack);            

            DKMOTOSCAN = new DK_MOTOROLA_SCANNER();
            DKMOTOSCAN.MotorolaBarcodeEvent += new EventMotorola(DKMOTOSCAN_MotorolaBarcodeEvent);
            //기존에 업데이트하고 남은 OLD 파일이 있으면 삭제한다.ㅋㅋ
            DKStepManager.DeleteOldFiles();

            DKSckClt = new DK_SOCKET_CLIENT(DKStepManager.GetLogPath());
            DKSckClt.SockRealTimeMsg += new EventSocketMsg(RecvSckMsgGateWay);

            g1 = listboxLog.CreateGraphics();
            g2 = listboxBinLog.CreateGraphics();

            DKStepManager.LoadSuffixList();
            DKStepManager.LoadPWUserInfo();      // 패스워드 관리 페이지 로드
            DKStepManager.InitInspectionRetry(); // 전체리트라이 리셋

            //CSMES
            STEPMANAGER_VALUE.bUseOSIMES = DKStepManager.LoadINI("OSI", "UseOSIMES").Equals("ON");
        }
                
        void CallBack(object status)
        {
            threadJobChangeTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            this.Invoke(new MethodInvoker(delegate()
            {
                try
                {
                    cbJobFiles.SelectedIndex = cbJobFiles.FindString(strJobChangeFileName);
                    string tmpStr = DKStepManager.LoadINI(constOption, "LASTFILE");
                    cbJobFiles.Refresh();
                    if (!tmpStr.Equals(cbJobFiles.SelectedItem.ToString()))
                    {
                        DKStepManager.SaveINI(constOption, "LASTFILE", cbJobFiles.SelectedItem.ToString());
                        bool bTemp = ProgramListSetUp();
                    }
                    cbJobFiles.SelectedIndex = cbJobFiles.FindString(strJobChangeFileName);
                    btnStart.PerformClick();
                    //CHANGE JOB 으로 들어올경우....스캐너로 찍은 바코드라면 유지해야하는데;;;

                    
                }
                catch (System.Exception ex)
                {
                    string strErr = "[FAIL CHANGE JOB] FileName(" + strJobChangeFileName + ") : " + ex.Message.ToString();
                    MessageBox.Show(strErr);
                }
            }));

        }

        private void SetControlDesign()
        {            
            // 그리드 디자인 부분 
            ixlblWannMsg.Visible = false;
            ixlblNetworkChkMsg.Visible = false;
            
            dataGridView1.AutoSize = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Columns.Add("Col0", "No");
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[0].Width = (int)((dataGridView1.Width - 16.5) * 0.04);
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = System.Drawing.Color.Ivory;
            dataGridView1.Columns.Add("Col1", "TEST NAME");
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].Width = (int)((dataGridView1.Width - 16.5) * 0.36);
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = System.Drawing.Color.Ivory;
            //--
            dataGridView1.Columns.Add("Col2", "RESULT");
            dataGridView1.Columns[2].Width = (int)((dataGridView1.Width - 16.5) * 0.09);
            dataGridView1.Columns.Add("Col3", "MIN");
            dataGridView1.Columns[3].Width = (int)((dataGridView1.Width - 16.5) * 0.10);
            dataGridView1.Columns.Add("Col4", "MAX");
            dataGridView1.Columns[4].Width = (int)((dataGridView1.Width - 16.5) * 0.15);
            dataGridView1.Columns.Add("Col5", "MEASURE");
            dataGridView1.Columns[5].Width = (int)((dataGridView1.Width - 5) * 0.26);
            dataGridView1.Columns.Add("Col6", "LAPSE");
            dataGridView1.Columns[6].Width = (int)((dataGridView1.Width - 5) * 0.04);
            dataGridView1.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.ColumnHeadersHeight = 30;

            for (int i = 2; i < 6; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.Ivory;

            }
            dataGridView1.Rows.Add();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Height = 33;

            }

            dataGridView1.AllowUserToResizeColumns = true;

            SetStatus((int)STATUS.READY);
            CrossThreadIssue.ChangeTextControl(ixlblMesInfo, "");
            CrossThreadIssue.ChangeTextControl(ixlblOracleInfo, "");
            CrossThreadIssue.ChangeTextControl(ixlblMesLED, "OFF");            
            lblMapOn.Text = lblMapOn2.Text = "AUTOJOB OFF";

            // 케이블 커넥터
            dataGridConnector.Columns.Add("Col0", "CABLE NAME");
            dataGridConnector.Columns[0].Width = (int)((dataGridConnector.Width) * 0.4);
            dataGridConnector.Columns.Add("Col1", "SPEC");
            dataGridConnector.Columns[1].Width = (int)((dataGridConnector.Width) * 0.2);
            dataGridConnector.Columns.Add("Col2", "USAGE");
            dataGridConnector.Columns[2].Width = (int)((dataGridConnector.Width) * 0.2);
            dataGridConnector.Columns.Add("Col3", "REMAIN");
            dataGridConnector.Columns[3].Width = (int)((dataGridConnector.Width) * 0.2);
            dataGridConnector.AllowUserToResizeColumns = true;
            dataGridConnector.Rows.Add(5);

            dataGridConnector.ColumnHeadersHeight = (int)((dataGridConnector.Height) / (dataGridConnector.Rows.Count + 1));
            for (int i = 0; i < dataGridConnector.Rows.Count; i++)
            {
                dataGridConnector.Rows[i].Height = (int)((dataGridConnector.Height) / (dataGridConnector.Rows.Count + 1));
            }
            
            //장치 상태 표시창
            string strColName = String.Empty;
            string strColText = String.Empty;

            strColName = "Col0";
            strColText = "DIO";
            dataGridDevice.Columns.Add("Col0", devDIO);
            dataGridDevice.Columns.Add("Col1", devSET);
            dataGridDevice.Columns.Add("Col2", dev5515C);
            dataGridDevice.Columns.Add("Col3", devScanner);
            dataGridDevice.Columns.Add("Col4", devPcan);
            dataGridDevice.Columns.Add("Col5", devTC3000);
            dataGridDevice.Columns.Add("Col6", devMTP200);
            dataGridDevice.Columns.Add("Col7", devAudio);
            dataGridDevice.Columns.Add("Col8", devADC);
            dataGridDevice.Columns.Add("Col9", dev34410A);
            dataGridDevice.Columns.Add("Col10", devVector);
            dataGridDevice.Columns.Add("Col11", devODA);
            dataGridDevice.Columns.Add("Col12", devKEITHLEY);
            dataGridDevice.Columns.Add("Col13", devMELSEC);
            dataGridDevice.Columns.Add("Col14", devMTP120A);

            dataGridDevice.Rows.Add(1);
           
            dataGridDevice.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                        
        }

        private void SetNewDesign()
        {
            axilblBackboarder.Visible = true;
            // 화면 해상도
            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height - 65;

            int iGirdTop = 126 + ((ScreenHeight - 126) / 2);

            // 그리드 디자인 부분 
            dataGridView1.Top = ixlblWannMsg.Top = iGirdTop - 5;
            int igrdHeight = ScreenHeight - dataGridView1.Top;
            dataGridView1.Width = ixlblWannMsg.Width = (int)(ScreenWidth * 0.7);
            dataGridView1.Height = igrdHeight - axiEngCounter.Height;
            dataGridView1.Left = btnStart.Left;
            ixlblWannMsg.Left = dataGridView1.Left;

            // 버튼 컨트롤 위치
            if (!DKStepManager.Item_bInteractiveMode)
                btnStop.Enabled = DKPlayChecker.Item_RUN;

            int iCtrlPos = 2;
            btnExit.Left = ScreenWidth - btnExit.Width - 6;
            btnExit.Top = btnStart.Top = btnStop.Top = btnConfig.Top = btnDisplay.Top = axiLabelCurrent.Top =
                          btnEdit.Top = btnInteractive.Top = iCtrlPos;
            
            axiCurrent.Top = axiLabelCurrent.Bottom + 1;
            // 카운트 박스, JOB 선택콤보

            int tmpWidSize = (int)(ScreenWidth - ixlblMesInfo.Left);
            panelGMES.Width = panelORACLE.Width = ixlblMesInfo.Width = ixlblOracleInfo.Width = (int)(tmpWidSize * 0.25)-8;
            LabelPass.Width = ixlblPass.Width = (int)(tmpWidSize * 0.055);
            LabelFAIL.Width = ixlblFail.Width = (int)(tmpWidSize * 0.055);
            LabelTOTAL.Width = ixlblTotal.Width = (int)(tmpWidSize * 0.055);
            //isegTimer.Width = (int)(tmpWidSize * 0.1);
            panelGMES.Height = panelORACLE.Height = ixlblOracleInfo.Height = ixlblMesInfo.Height + 1;
            cbJobFiles.Width = dataGridDevice.Width = (int)((tmpWidSize - 5 - 8) * 0.585);

            panelORACLE.Top = panelGMES.Top; 
            panelORACLE.Left = panelGMES.Left;

            ixlblOracleInfo.Left = ixlblMesInfo.Left;
            ixlblOracleInfo.Top = ixlblMesInfo.Top;

            LabelPass.Left = ixlblPass.Left = ixlblMesInfo.Right + 8;
            LabelFAIL.Left = ixlblFail.Left = LabelPass.Right + 1;
            LabelTOTAL.Left = ixlblTotal.Left = LabelFAIL.Right + 1;
            isegTimer.Left = btnInteractive.Right + 1; //LabelTOTAL.Right + 1;
            cbJobFiles.Left = dataGridDevice.Left = LabelTOTAL.Right + 1;//isegTimer.Right + 1;
            lblVersion.Left = LabelPass.Left;
            lblVersion.Width = LabelTOTAL.Right - LabelPass.Left;

            // 케이블 커넥터
            dataGridConnector.Top = 126;
            dataGridConnector.Left = dataGridView1.Right + 3;
            dataGridConnector.Width = ScreenWidth - dataGridConnector.Left - 6;
            dataGridConnector.Height = (int)((dataGridView1.Height + axiEngCounter.Height - 15) * 0.7)
                                       - (axiLabelWip1.Height + axiLabelBarcode.Height + 6);

            int iConnWidth = (int)((dataGridConnector.Width) / 10);

            for (int i = 0; i < dataGridConnector.Columns.Count; i++)
            {
                if (i == 0)
                    dataGridConnector.Columns[i].Width = iConnWidth * 4;
                else
                    dataGridConnector.Columns[i].Width = iConnWidth * 2;
            }

            // 차트 current 추가
            ChartCurrent.Size = dataGridConnector.Size;
            ChartCurrent.Location = dataGridConnector.Location;

            // 상태창

            ixlblStatus.Top = 126;
            ixlblStatus.Left = dataGridView1.Left;
            ixlblStatus.Width = dataGridView1.Width;
            ixlblStatus.Height = (int)((dataGridView1.Height + axiEngCounter.Height - 15) * 0.7);
            //ixlblWannMsg.Height = dataGridView1.Height - ixlblStatus.Bottom;
            ixlblWannMsg.Height = dataGridView1.Height;

            if (ixlblStatus.Width > 560)
            {
                if (ixlblStatus.Width > 800)
                {
                    ixlblStatus.Font = new Font("Verdana", 108, FontStyle.Bold);
                    ixlblStatusCount.Font = new Font("Courier New", 20, FontStyle.Bold);
                }
                else
                {
                    ixlblStatus.Font = new Font("Verdana", 68, FontStyle.Bold);
                    ixlblStatusCount.Font = new Font("Courier New", 16, FontStyle.Bold);
                }

            }
            else
            {
                ixlblStatus.Font = new Font("Verdana", 48, FontStyle.Bold);
                ixlblStatusCount.Font = new Font("Courier New", 12, FontStyle.Bold);
            }
            
            progressInspection.Height = (int)((dataGridView1.Height + axiEngCounter.Height - 15) * 0.3);
            progressInspection.Top = ixlblStatus.Bottom + 1; // -progressInspection.Height - 1;            
            progressInspection.Left = ixlblStatus.Left + 1;
            progressInspection.Width = ixlblStatus.Width - 2;

            axilblBackboarder.Height = progressInspection.Height + 4;
            axilblBackboarder.Top = progressInspection.Top - 2;
            axilblBackboarder.Left = progressInspection.Left - 1;
            axilblBackboarder.Width = progressInspection.Width + 2;

            ixlblStatusCount.Height = 40;
            ixlblStatusCount.Top = progressInspection.Top - ixlblStatusCount.Height - 2;
            ixlblStatusCount.Left = ixlblStatus.Left + 2;
            ixlblStatusCount.Width = ixlblStatus.Width - 4;

            ixlblStatusDate.Top = ixlblStatus.Top + 1;
            ixlblStatusDate.Left = ixlblStatus.Left + 1;

            // Wip 관련 & Current 관련
            axiLabelWip1.Top = txtBoxWip1.Top = dataGridConnector.Bottom + 2;
            axiLabelWip2.Top = txtBoxWip2.Top = txtBoxWip1.Bottom + 2;
            axiLabelBarcode.Top = txtBoxBarcode.Top = txtBoxWip2.Bottom + 2;

            axiLabelWip1.Left = axiLabelWip2.Left = axiLabelBarcode.Left = axiLabelGPSTIME.Left = dataGridConnector.Left;
            txtBoxWip1.Left = txtBoxWip2.Left = txtBoxBarcode.Left = axiLabelWip1.Right - 1;
            txtBoxWip1.Width = txtBoxWip2.Width = txtBoxBarcode.Width = dataGridConnector.Width - axiLabelWip1.Width + 1;

            //GPS 표시영역  ----

            int itmpSize = dataGridConnector.Width;
            axiLabelGPSTIME.Top = isegGpsTimer.Top = axiLabelScount.Top = txtBoxScount.Top = axiLabelBarcode.Bottom + 2;
            axiLabelGPSTIME.Width = (int)(itmpSize * 0.3);
            isegGpsTimer.Left = axiLabelGPSTIME.Right;
            isegGpsTimer.Width = (int)(itmpSize * 0.3);
            axiLabelScount.Left = axiLabelCN0.Left = isegGpsTimer.Right;
            axiLabelScount.Width = axiLabelCN0.Width = (int)(itmpSize * 0.25);
            txtBoxScount.Left = txtBoxCN0.Left = axiLabelScount.Right - 1;
            txtBoxScount.Width = txtBoxCN0.Width = itmpSize - axiLabelGPSTIME.Width - isegGpsTimer.Width - axiLabelScount.Width + 1;
            axiLabelCN0.Top = txtBoxCN0.Top = axiLabelScount.Bottom - 1;

            // Log 창   
            tabControl.Left = dataGridConnector.Left;
            tabControl.Top = axiLabelBarcode.Bottom + 2;
            tabControl.Width = dataGridConnector.Width;
            tabControl.Height = ScreenHeight - tabControl.Top - groupStatusBox.Height - axiCommandStatus.Height - 8;

            axiCommandStatus.Top = tabControl.Bottom - 1;
            //스레드 상태 표시창.
            groupStatusBox.Left = axiCommandStatus.Left = dataGridConnector.Left;
            groupStatusBox.Top = axiCommandStatus.Bottom - 3; //tabControl.Bottom - 3;
            groupStatusBox.Width = axiCommandStatus.Width = tabControl.Width;

            int tmpGSBsize = (int)(groupStatusBox.Width / 6) - 2;
            axiDioRecv.Width  = tmpGSBsize;
            axiDioClear.Width = tmpGSBsize;
            axiDioDelay.Width = tmpGSBsize;
            axiDutRecv.Width  = tmpGSBsize;
            axiDutClear.Width = tmpGSBsize;
            axiDutDelay.Width = tmpGSBsize;

            //axiDioRecv.Left = 2;
            axiDioClear.Left = axiDioRecv.Right + 1;
            axiDioDelay.Left = axiDioClear.Right + 1;
            axiDutRecv.Left  = axiDioDelay.Right + 1;
            axiDutClear.Left = axiDutRecv.Right + 1;
            axiDutDelay.Left = axiDutClear.Right + 1;

            tmpGSBsize = (int)(groupStatusBox.Width / 5) - 2;

            axiSensor0.Width = tmpGSBsize;
            axiSensor1.Width = tmpGSBsize;
            axiSensor2.Width = tmpGSBsize;
            axiSensor3.Width = tmpGSBsize;
            axiSensor4.Width = tmpGSBsize;
            axiSensor5.Width = tmpGSBsize;
            axiSensor6.Width = tmpGSBsize;
            axiSensor7.Width = tmpGSBsize;
            axiSensor8.Width = tmpGSBsize;
            axiSensor9.Width = tmpGSBsize;

            axiSensor1.Left = axiSensor0.Right + 1;
            axiSensor2.Left = axiSensor1.Right + 1;
            axiSensor3.Left = axiSensor2.Right + 1;
            axiSensor4.Left = axiSensor3.Right + 1;

            axiSensor6.Left = axiSensor5.Right + 1;
            axiSensor7.Left = axiSensor6.Right + 1;
            axiSensor8.Left = axiSensor7.Right + 1;
            axiSensor9.Left = axiSensor8.Right + 1;


            // 장치 상태 그리드
            int iDevWidth = (int)((dataGridDevice.Width + 20) / dataGridDevice.Columns.Count);
            for (int i = 0; i < dataGridDevice.Columns.Count; i++)
            {
                dataGridDevice.Columns[i].Width = iDevWidth;
            }

            dataGridDevice.Columns[0].Width = iDevWidth - 10;
            dataGridDevice.Columns[1].Width = iDevWidth - 10;

            // 엔진 카운터 및 빌드버젼
            axiBuildVer.Top = axiEngCounter.Top
                            = axiBuffCounter1.Top = axiBuffCounter2.Top = axiBuffCounter3.Top = axiBuffCounter4.Top
                            = groupStatusBox.Bottom + 1;
            axiEngCounter.Left = groupStatusBox.Left;
            axiBuildVer.Left = axiEngCounter.Right + 1;
            axiBuildVer.Width = groupStatusBox.Width - axiEngCounter.Width - 1;

            axiBuffCounter1.Width = axiBuffCounter2.Width = axiBuffCounter4.Width
                                  = (int)(dataGridView1.Width / 6) - 1;
            axiBuffCounter3.Width = (int)((dataGridView1.Width / 6) * 3);
            axiBuffCounter1.Left = dataGridView1.Left;
            axiBuffCounter2.Left = axiBuffCounter1.Right + 1;
            axiBuffCounter3.Left = axiBuffCounter2.Right + 1;
            axiBuffCounter4.Left = axiBuffCounter3.Right + 1;
                        
        }

        private void SetOldDesign()
        {
            axilblBackboarder.Visible = false;
            // 화면 해상도
            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height - 65;

            // 그리드 디자인 부분 
            dataGridView1.Top = ixlblWannMsg.Top = 126;
            int igrdHeight = ScreenHeight - dataGridView1.Top;
            dataGridView1.Width = ixlblWannMsg.Width = (int)(ScreenWidth * 0.7);
            dataGridView1.Height = igrdHeight - axiEngCounter.Height;
            dataGridView1.Left = btnStart.Left;
            ixlblWannMsg.Left = dataGridView1.Left;            

            // 버튼 컨트롤 위치
            if (!DKStepManager.Item_bInteractiveMode)
                btnStop.Enabled = DKPlayChecker.Item_RUN;

            int iCtrlPos = 2;
            btnExit.Left = ScreenWidth - btnExit.Width - 6;
            btnExit.Top = btnStart.Top = btnStop.Top = btnConfig.Top = btnDisplay.Top = axiLabelCurrent.Top =
                          btnEdit.Top = btnInteractive.Top = iCtrlPos;
            
            axiCurrent.Top = axiLabelCurrent.Bottom + 1;
            // 카운트 박스, JOB 선택콤보

            int tmpWidSize = (int)(ScreenWidth - ixlblMesInfo.Left);
            panelGMES.Width = panelORACLE.Width = ixlblMesInfo.Width = ixlblOracleInfo.Width = (int)(tmpWidSize * 0.25) - 8;
            LabelPass.Width = ixlblPass.Width = (int)(tmpWidSize * 0.055);
            LabelFAIL.Width = ixlblFail.Width = (int)(tmpWidSize * 0.055);
            LabelTOTAL.Width = ixlblTotal.Width = (int)(tmpWidSize * 0.055);
            //isegTimer.Width = (int)(tmpWidSize * 0.1);
            panelGMES.Height = panelORACLE.Height = ixlblOracleInfo.Height = ixlblMesInfo.Height + 1;
            cbJobFiles.Width = dataGridDevice.Width = (int)((tmpWidSize - 5 - 8) * 0.585);

            panelORACLE.Top = panelGMES.Top; 
            panelORACLE.Left = panelGMES.Left;

            ixlblOracleInfo.Left = ixlblMesInfo.Left;
            ixlblOracleInfo.Top = ixlblMesInfo.Top;

            LabelPass.Left = ixlblPass.Left = ixlblMesInfo.Right + 8;
            LabelFAIL.Left = ixlblFail.Left = LabelPass.Right + 1;
            LabelTOTAL.Left = ixlblTotal.Left = LabelFAIL.Right + 1;
            isegTimer.Left = btnInteractive.Right + 1; //LabelTOTAL.Right + 1;
            cbJobFiles.Left = dataGridDevice.Left = LabelTOTAL.Right + 1;//isegTimer.Right + 1;
            lblVersion.Left = LabelPass.Left;
            lblVersion.Width = LabelTOTAL.Right - LabelPass.Left;

            // 케이블 커넥터
            dataGridConnector.Top = dataGridView1.Top;
            dataGridConnector.Left = dataGridView1.Right + 3;
            dataGridConnector.Width = ScreenWidth - dataGridConnector.Left - 6;
            dataGridConnector.Height = 140;

            int iConnWidth = (int)((dataGridConnector.Width) / 10);

            for (int i = 0; i < dataGridConnector.Columns.Count; i++)
            {
                if (i == 0)
                    dataGridConnector.Columns[i].Width = iConnWidth * 4;
                else
                    dataGridConnector.Columns[i].Width = iConnWidth * 2;
            }

            // 차트 current 추가
            ChartCurrent.Size = dataGridConnector.Size;
            ChartCurrent.Location = dataGridConnector.Location;

            

            // 상태창

            ixlblStatus.Top = dataGridConnector.Bottom + 2;
            ixlblStatus.Left = dataGridConnector.Left;
            ixlblStatus.Width = dataGridConnector.Width;
            ixlblStatus.Height = 212;
            //ixlblWannMsg.Height = dataGridView1.Height - ixlblStatus.Bottom;
            ixlblWannMsg.Height = dataGridView1.Height / 2;
            ixlblWannMsg.Height = dataGridView1.Height;

            if (ixlblStatus.Width > 560)
            {
                if (ixlblStatus.Width > 800)
                {
                    ixlblStatus.Font = new Font("Verdana", 108, FontStyle.Bold);
                    ixlblStatusCount.Font = new Font("Courier New", 18, FontStyle.Bold);
                }
                else
                {
                    ixlblStatus.Font = new Font("Verdana", 68, FontStyle.Bold);
                    ixlblStatusCount.Font = new Font("Courier New", 16, FontStyle.Bold);
                }

            }
            else
            {
                ixlblStatus.Font = new Font("Verdana", 48, FontStyle.Bold);
                ixlblStatusCount.Font = new Font("Courier New", 12, FontStyle.Bold);
            }

            progressInspection.Height = 31;
            progressInspection.Top = ixlblStatus.Bottom - progressInspection.Height - 1;
            progressInspection.Left = ixlblStatus.Left + 1;
            progressInspection.Width = ixlblStatus.Width - 2;

            ixlblStatusCount.Height = progressInspection.Height;
            ixlblStatusCount.Top = progressInspection.Top - progressInspection.Height - 2;
            ixlblStatusCount.Left = ixlblStatus.Left + 2;
            ixlblStatusCount.Width = ixlblStatus.Width - 4;

            ixlblStatusDate.Top = ixlblStatus.Top + 1;
            ixlblStatusDate.Left = ixlblStatus.Left + 1;

            // Wip 관련 & Current 관련
            axiLabelWip1.Top = txtBoxWip1.Top = ixlblStatus.Bottom + 2;
            axiLabelWip2.Top = txtBoxWip2.Top = txtBoxWip1.Bottom + 2;
            axiLabelBarcode.Top = txtBoxBarcode.Top = txtBoxWip2.Bottom + 2;

            axiLabelWip1.Left = axiLabelWip2.Left = axiLabelBarcode.Left = axiLabelGPSTIME.Left = ixlblStatus.Left;
            txtBoxWip1.Left = txtBoxWip2.Left = txtBoxBarcode.Left = axiLabelWip1.Right - 1;
            txtBoxWip1.Width = txtBoxWip2.Width = txtBoxBarcode.Width = ixlblStatus.Width - axiLabelWip1.Width + 1;

            int itmpSize = ixlblStatus.Width;
            axiLabelGPSTIME.Top = isegGpsTimer.Top = axiLabelScount.Top = txtBoxScount.Top = axiLabelBarcode.Bottom + 2;
            axiLabelGPSTIME.Width = (int)(itmpSize * 0.3);
            isegGpsTimer.Left = axiLabelGPSTIME.Right;
            isegGpsTimer.Width = (int)(itmpSize * 0.3);
            axiLabelScount.Left = axiLabelCN0.Left = isegGpsTimer.Right;
            axiLabelScount.Width = axiLabelCN0.Width = (int)(itmpSize * 0.25);
            txtBoxScount.Left = txtBoxCN0.Left = axiLabelScount.Right - 1;
            txtBoxScount.Width = txtBoxCN0.Width = itmpSize - axiLabelGPSTIME.Width - isegGpsTimer.Width - axiLabelScount.Width + 1;
            axiLabelCN0.Top = txtBoxCN0.Top = axiLabelScount.Bottom - 1;

            // Log 창     
            tabControl.Left = ixlblStatus.Left;
            tabControl.Top = axiLabelBarcode.Bottom + 2;
            tabControl.Width = ixlblStatus.Width;
            tabControl.Height = ScreenHeight - tabControl.Top - groupStatusBox.Height - axiCommandStatus.Height - 8;

            axiCommandStatus.Top = tabControl.Bottom - 1;

            //스레드 상태 표시창.
            groupStatusBox.Left = axiCommandStatus.Left = ixlblStatus.Left;
            groupStatusBox.Top = axiCommandStatus.Bottom - 3; //tabControl.Bottom - 3;
            groupStatusBox.Width = axiCommandStatus.Width = tabControl.Width;

            int tmpGSBsize = (int)(groupStatusBox.Width / 6) - 2;
            axiDioRecv.Width = tmpGSBsize;
            axiDioClear.Width = tmpGSBsize;
            axiDioDelay.Width = tmpGSBsize;
            axiDutRecv.Width = tmpGSBsize;
            axiDutClear.Width = tmpGSBsize;
            axiDutDelay.Width = tmpGSBsize;

            //axiDioRecv.Left = 2;
            axiDioClear.Left = axiDioRecv.Right + 1;
            axiDioDelay.Left = axiDioClear.Right + 1;
            axiDutRecv.Left = axiDioDelay.Right + 1;
            axiDutClear.Left = axiDutRecv.Right + 1;
            axiDutDelay.Left = axiDutClear.Right + 1;

            tmpGSBsize = (int)(groupStatusBox.Width / 5) - 2;

            axiSensor0.Width = tmpGSBsize;
            axiSensor1.Width = tmpGSBsize;
            axiSensor2.Width = tmpGSBsize;
            axiSensor3.Width = tmpGSBsize;
            axiSensor4.Width = tmpGSBsize;
            axiSensor5.Width = tmpGSBsize;
            axiSensor6.Width = tmpGSBsize;
            axiSensor7.Width = tmpGSBsize;
            axiSensor8.Width = tmpGSBsize;
            axiSensor9.Width = tmpGSBsize;

            axiSensor1.Left = axiSensor0.Right + 1;
            axiSensor2.Left = axiSensor1.Right + 1;
            axiSensor3.Left = axiSensor2.Right + 1;
            axiSensor4.Left = axiSensor3.Right + 1;

            axiSensor6.Left = axiSensor5.Right + 1;
            axiSensor7.Left = axiSensor6.Right + 1;
            axiSensor8.Left = axiSensor7.Right + 1;
            axiSensor9.Left = axiSensor8.Right + 1;


            // 장치 상태 그리드
            int iDevWidth = (int)((dataGridDevice.Width+20) / dataGridDevice.Columns.Count);
            for (int i = 0; i < dataGridDevice.Columns.Count; i++)
            {
                dataGridDevice.Columns[i].Width = iDevWidth;
            }

            dataGridDevice.Columns[0].Width = iDevWidth - 10;
            dataGridDevice.Columns[1].Width = iDevWidth - 10;

            // 엔진 카운터 및 빌드버젼
            axiBuildVer.Top = axiEngCounter.Top
                            = axiBuffCounter1.Top = axiBuffCounter2.Top = axiBuffCounter3.Top = axiBuffCounter4.Top
                            = groupStatusBox.Bottom + 1;
            axiEngCounter.Left = groupStatusBox.Left;
            axiBuildVer.Left = axiEngCounter.Right + 1;
            axiBuildVer.Width = groupStatusBox.Width - axiEngCounter.Width - 1;

            axiBuffCounter1.Width = axiBuffCounter2.Width = axiBuffCounter4.Width
                                  = (int)(dataGridView1.Width / 6) - 1;
            axiBuffCounter3.Width = (int)((dataGridView1.Width / 6) * 3);
            axiBuffCounter1.Left = dataGridView1.Left;
            axiBuffCounter2.Left = axiBuffCounter1.Right + 1;
            axiBuffCounter3.Left = axiBuffCounter2.Right + 1;
            axiBuffCounter4.Left = axiBuffCounter3.Right + 1;

        }

        private void ShowOracleInfo(bool bOn, bool bConn)
        {
            string strProductionLine = "LINE   ";
            string strProcessCode    = "CODE   ";
            string strPCID           = "PCID   ";

            string[] tmpStr = new string[6];

            ORACLEINFO InfoName = new ORACLEINFO();
            InfoName = DKStepManager.GetOracleInfoName();

            tmpStr[0] = DKStepManager.LoadINI("ORACLE", InfoName.strProductionLine);
            tmpStr[1] = DKStepManager.LoadINI("ORACLE", InfoName.strProcessCode);
            tmpStr[2] = DKStepManager.LoadINI("ORACLE", InfoName.strPCID);
            tmpStr[3] = DKStepManager.LoadINI("ORACLE", InfoName.strCallType);
            tmpStr[4] = DKStepManager.LoadINI("ORACLE", InfoName.strOOBCode);
            tmpStr[5] = DKStepManager.LoadINI("ORACLE", InfoName.strOOBFlag);

            if (tmpStr[3].Equals("OOB"))
            {
                tmpStr[3] = tmpStr[3] + "(" + tmpStr[4] + " - " + tmpStr[5] + ")";
            }

            StringBuilder sbInfo = new StringBuilder(1024);
            string strBlankL = "           [";
            string strBlankR = "]";
            sbInfo.Append(strBlankL); sbInfo.Append(strProductionLine); sbInfo.Append(strBlankR); sbInfo.AppendLine(tmpStr[0]);
            sbInfo.Append(strBlankL); sbInfo.Append(strProcessCode);    sbInfo.Append(strBlankR); sbInfo.AppendLine(tmpStr[1]);
            sbInfo.Append(strBlankL); sbInfo.Append(strPCID);           sbInfo.Append(strBlankR); sbInfo.AppendLine(tmpStr[2]);
            sbInfo.Append(strBlankL); sbInfo.Append("STATION");         sbInfo.Append(strBlankR); sbInfo.Append(tmpStr[3]);
                        
            CrossThreadIssue.ChangeTextControl(ixlblOracleInfo, sbInfo.ToString());
            StopNetworkCheck_OracleConnection();

            if (bOn)
            {
                ixlblOracleInfo.FontColor = Color.DarkSlateGray;

                if (bConn)
                {
                    StartNetworkCheck_OracleConnection(true);        
                }                
            }
            else
            {
                CrossThreadIssue.ChangeTextControl(ixlblOracleLED, "OFF");
                CrossThreadIssue.ChangeBackColor(ixlblOracleLED, Color.Crimson);
                CrossThreadIssue.ChangeFontColor(ixlblOracleInfo, Color.DarkSlateGray);
            }
        }

        private void NetworkCheckMsgPannel(bool bShow)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {

                    if (bShow)
                    {
                        ixlblNetworkChkMsg.Text = "Waiting for Network Check...(MES SERVER)";
                        ixlblNetworkChkMsg.Visible = true;
                    }
                    else
                    {
                        ixlblNetworkChkMsg.Visible = false;
                    }
                    this.Refresh();


                }));
            }
            catch { }

        }

        private void StopNetworkCheck_OracleConnection()
        {
            NetworkCheckMsgPannel(false);
            DestroyThread(thdPing);
            DestroyThread(thdOrac);
        }

        private void StartNetworkCheck_OracleConnection(bool bOn)
        {
            thdPing = new System.Threading.Thread(CheckNetworkFunction);
            thdPing.Start();

            if (bOn)
            {
                thdOrac = new System.Threading.Thread(CheckOracleConnection);
                thdOrac.Start();  
            }

            
        }

        private void SetUiFlag()
        {
            // UI 디폴트 설정값 가져오기
            string strUIflag = DKStepManager.LoadINI(constOption, constDefaultUI);
            if (!strUIflag.Equals("ON")) bUIFlag = false;
            else bUIFlag = true;

            if (bUIFlag)
            {   //기존 UI 방식으로 
                SetOldDesign();

            }
            else
            {   //신규 UI 방식으로 
                SetNewDesign();
            }
            SetPopPos();
            
            //버젼표시            
            Assembly currAssembly = Assembly.GetExecutingAssembly();
            Version version = currAssembly.GetName().Version;            
            this.Text = "GM TELEMATICS TESTER Ver " + version.ToString();
            STEPMANAGER_VALUE.strProgramVersion = this.Text;
            lblVersion.Text = "Ver." + version.ToString();
            LoadingVersionHistory();

        }

        private void LoadingVersionHistory()
        {
            STEPMANAGER_VALUE.LstVersionHistory.Clear();
                        
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.5.20.0 : Dynamic LTE DLL Load - DLL_FILE_NAME Command Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.5.25.0 : GM_PART_NO, BASE_MODEL_PART_NO To GMES by Convert Decimal Value");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.5.30.0 : CMW500 Performance Up, FREQUENCY.INI(LTE-BAND3 Add)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.5.31.0 : CMW500 INITIALIZE, PRESET ,RST Command Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.5.31.1 : Errmessage the \"-114,\"Header suffix out of range;SOUR:GPRF:GEN:STAT OFF\" in CMW500 INITILIZE is CMW500 2Channel Not Setting.");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 1000.0.00.1 : Only One Copy Version in KOREA DQA");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.6.09.0 : Zebra scanner Nonascii detection Function Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : JOB File to EXCEL Function in EDIT Mode Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : GMES DLL Ver(1.0.6) Update");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : Bin Logging Function Update");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : Oracle & GMES Common Update");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : Erase Space charater in USB Barcode Reading");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.8.26.0 : CheckExprParam  Command Update (GMES Model/suffx)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : MODEL,SUFFIX AutoListUp in MAPPING EDIT");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : COMPORT AutoListup in CONFIG");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : ADD DEVICE - AUDIO SELECTOR");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.9.9.0  : CAN MSG ID UPDATE");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.9.23.0 : PASSWORD MANAGER Function Update");                        
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : ADD DEVICE - ADC MODULE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : ADD LTE_CHINA_NEW_20.DLL Function Update(version, readSCNV)");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.4.0 : NAD DLL - EFS BACKUP Command Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : WCDMA_BER_OFF.CMW FILE UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.10.0: \"NONASCII\" TYPE UPDATE in Keywrite Station(ESIM_PROFILE to GMES - 0xFF error)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.11.0: CMW500 Excute fail Error - Solve");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.26.0: MES & GMES - KIS SERVER CONNECTION UPDATE.");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.27.0: RESULT LOG(CSV) CHANGE (GMES,MES)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : RELEASE TO ATCO(USA)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.10.27.1: RESULT LOG(CSV) CHANGE - DATA FIELD [ ] UPDATE, DEFECT SPACE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.28.1: MES - SET& GET_OOB_INFO PACKAGE CALL PARAM 14LENGTH(IMEI) UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.29.0: LGEVH, LGEKS - KIS Sever Connection Primary & Secondary UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.10.31.0: LGEKS - MES TXNID PACKAGE UPDATE (NOT USE MASTER INSERT:TXNID)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.11.1.0 : LGEKS - MES ExcuteProcedure() RETRY 3th UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.8.0 : CONFIG - MTP200 LOSS VALUE UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.12.0: CONFIG - MODEL  INFO VALUE UPDATE(GEN10 MFG CHECK PARTNUMBER, STID)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.14.0: GM_GET_TXN_ID DELETED - CNS MR.JUNG JIN SUB");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.14.1: MTP LOSS TABLE UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.14.2: TC3000 - BAUDRATE CHANGE COMMAND UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.11.17.0: DateTime.Now. HH update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.11.20.0: GEN10_OQA_COMMAND (RECOGNITION_TIME, REPORT_CURRENT_SID, OOB_SELF_TEST, OOB_SELF_TEST_CHECK) UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.11.21.4: GET_KEYWRITE_MAIN_PSA PACKAGE UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.27.0: PCAN RECV THREADING");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.29.0: SEMI LOG FUNCTION UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.11.29.0: TCP ATT - BUB COMMAND & HEART BEAT(2RD1f) AUTO RESPONSE UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.11.30.0: TCP, GEN10, ATT(TCP) ALDL COMMAND UPDATE - BITS, TOGGLE ON/OFF");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : READ_NV_ITEM (GEN10) - COMMAND UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.2.0 : KS - OOB PSA INFO PAKAGE UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.12.5.0 : GOTO-LABEL A1~19 (STOP), C1~19(CONTINUE)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.12.0: PCAN WRITPCAN() TX LOGGING, UNTIL(100ms) OPTION UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.13.0: UPDATE LOGGING for ATCO");
            STEPMANAGER_VALUE.LstVersionHistory.Add("                : Config - Show Count Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.14.1: BIN LOGGING TYPE CHANGE(WIP/STID)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.14.2: CONFIG PATH SHOW");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.12.15.0: INTERACTIVE - SKIP");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.15.1: DEBUG MODE(NO DIO), SHOW MEASURE(NOTEPAD) UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("    2016.12.16.0: Zebra Scanner Performence Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.12.20.0: PCAN STATUS (OFF/HEAVY/LIGHT) UI View Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.12.21.0: CHECK PCAN STATUS COMMAND UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.12.22.0: PCAN (INITIALZE_HSCAN, UNINITIALZE_HSCAN) COMMAND UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2016.12.29.0: PAGE - TIMER(START, STOP, VALUE) COMMAND UPDATE (For TimeStamp)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.2.0  : DMM(34410A) UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.6.0  : DeleteScreen() SAVE - 120Days.");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.18.0 : EDIT - Ctrl + F UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.19.0 : 5515C TABLE COMMAND CHANGE - GSM_SET_CEL-ACTIVE-CELL");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.21.0 : UI CURRENT - AUTO RANGE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver             : DO NOT INTERACTIVE - SKIP COMMAND");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.25.3 : JOB FILE LOAD & CHECKSUM UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.30.0 : JOB FILE EDITED TRACE UPDATE (HISTORY FOLDER)");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.1.31.0 : CURRENT GRAPH UPDATE");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.2.1.0  : 34410A TBL - CURRENT DC MAX -> AUTO");
                                                     
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.2.4.0  : JOB FILE LOAD - CRC ERROR UPDATE");            
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.2.21.0 : GEN10 - BUB_DISABLE, ENABLE COMMAND UPDATE");                        
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.2.27.0 : TCP.TBL - NV ITEM READ COMMAND UDDATE");
                                                     
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.3.22.0 : CopyCheck - EDIT SAVE AS Error Fix");
                                                     
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.3.24.0 : Gmesitemcoding Error Fix1");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.3.25.0 : Gmesitemcoding Error Fix2");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.3.27.1 : Gmesitemcoding Error Fix3");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.3.30.0 : Gmesitemcoding Error Fix4");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver             : DeviceControlUSB() Function Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver             : Dsub Retry Function Update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.4.5.0  : PCAN until - heavy/light auto reset function update");
            STEPMANAGER_VALUE.LstVersionHistory.Add("Ver 2017.4.28.0 : STOP to READY Status");


        }

        private void SetUiFlagManual()
        {
            if (bUIFlag)
            {
                SetNewDesign();
                bUIFlag = false;
            }
            else
            {
                SetOldDesign();
                bUIFlag = true;
            }
            SetPopPos();
        }

        private void SetPopPos()
        {
            STEPMANAGER_VALUE.iPopPosLeft = dataGridView1.Width;
            STEPMANAGER_VALUE.iPopPosTop = dataGridView1.Top;

            ixlblNetworkChkMsg.Top = btnExit.Top;
            ixlblNetworkChkMsg.Left = btnStart.Left;
            ixlblNetworkChkMsg.Width = btnExit.Right - btnStart.Left;
            ixlblNetworkChkMsg.Height = btnExit.Height;
        }

        private void InitialUI()
        {
            SetControlDesign();
            SetUiFlag(); 
            if (CheckMinimumSize()) //착하검사기같은 작은화면인경우
            {
                isegTimer.DigitCount = 4;
            }            
            ResultReset(); //최종결과  리셋
            GridWidthAutoSize(); //그리드 사이즈 사용자값 로딩.
        }

        public FrmFaMain()
        { 
            InitializeComponent();
            InitValue();
            InitialUI();
			//CheckLgeMD5();
            ComboListUpdate();
            CheckMananger();
            ConnectorCountLoad();
            CountLoad();
            CheckPrimaryMES(true);
            CheckConfigJobMappingStatus();
            SocketConnect();
            MoveCurrentControls();
            OnFileEnc();
        }

        private void OnFileEnc()
        {
            //GMES 로그파일을 전부 암호화 하는 프로그램 실행
            try
            {
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "FileEnc.exe");
            }
            catch { }
        }

        private void IsRestart()
        {
            string tmpStr = DKStepManager.LoadINI("RUNTIME", "RESTART");

            if (tmpStr.Equals("ON"))
            {
                DKStepManager.SaveINI("RUNTIME", "RESTART", "OFF");

                this.Invoke(new MethodInvoker(delegate()
                {
                    btnStart.PerformClick();
                }));
            }
        }

        private void OnBreaker()
        {
            OffBreaker();
            try
            {
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "BREAKER.exe");
                
            }
            catch { }
        }

        private void OffBreaker()
        {
            try
            {
                System.Diagnostics.Process[] remoteByName = System.Diagnostics.Process.GetProcesses();

                if (remoteByName.Length > 0)
                {
                    for (int i = 0; i < remoteByName.Length; i++)
                    {
                        string strname = remoteByName[i].MainWindowTitle;

                        if (strname.Contains("GMBREAKER"))
                        {
                            remoteByName[i].Kill();
                            break;
                        }
                    }
                }
            }
            catch 
            {
            	
            }
            
        }

        private void MoveCurrentControls()
        {
            string tmpStr = DKStepManager.LoadINI(constOption, "VIEWCOUNT");
            if(tmpStr.Equals("ON"))
                ShowCountControls();
            else
                HideCountControls();

        }    

        private void ShowCountControls()
        {
            this.Invoke(new MethodInvoker(delegate() {
                axiCurrent.SegmentMargin = 4;
                axiCurrent.SegmentSeperation = 1;
                axiLabelCurrent.Caption = "PRIMARY CURRENT";
                axiLabelCurrent.Left = dataGridConnector.Left;
                axiLabelCurrent.Height = axiCurrent.Height = (int)(dataGridConnector.Height * 0.25);
                axiLabelCurrent.Width = (int)(dataGridConnector.Width * 0.6);

                axiLabelCurrent.Top = axiCurrent.Top = dataGridConnector.Bottom - axiLabelCurrent.Height;

                axiCurrent.Left = axiLabelCurrent.Right;
                axiCurrent.Width = dataGridConnector.Width - axiLabelCurrent.Width;
            }));
        }

        private void HideCountControls()
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                axiCurrent.SegmentMargin = 6;
                axiCurrent.SegmentSeperation = 2;
                axiLabelCurrent.Caption = "CURRENT";
                axiLabelCurrent.Top = axiCurrent.Top = LabelPass.Top;
                axiLabelCurrent.Left = LabelPass.Left;
                axiLabelCurrent.Height = axiCurrent.Height = ixlblPass.Bottom - LabelPass.Top;
                axiLabelCurrent.Width = LabelPass.Width;
                axiCurrent.Left = LabelFAIL.Left;
                axiCurrent.Width = LabelTOTAL.Right - LabelFAIL.Left;    
            }));            
        }

        private void CheckPrimaryMES(bool bMode)
        {            
            if (bMode)
            {
                switch (StatusPrimaryMES())
                {
                    case (int)MESPRI.ORACLE:
                        toolStripMenuItem2.PerformClick();
                        ControlLoad(chkBoxOracleOn, "ORACLE", "MESON");
                        break;
                    default:
                        toolStripMenuItem1.PerformClick();
                        ControlLoad(chkBoxMesOn, constOption, "MESON");
                        break;
                }
            }
            else
            {
                switch (StatusPrimaryMES())
                {
                    case (int)MESPRI.ORACLE:
                        if (chkBoxOracleOn.Checked)
                        {
                            toolStripMenuItem2.PerformClick();
                            MainFormSleepFuction(500);
                            chkBoxOracleOn.Checked = true;
                        }
                        break;
                    default:                        
                        break;
                }
            }
            

        }

        private int StatusPrimaryMES()
        {
            string tmpStr = DKStepManager.LoadINI(constOption, constPrimary);
            
            switch (tmpStr)
            {
                case "ORACLE":
                    return (int)MESPRI.ORACLE;
                default:
                    return (int)MESPRI.GMES;
            }
        }
		
        private bool CheckMinimumSize()
        {
            string strGetText = DKStepManager.LoadINI("OPTION", "MSIZE");
            if (strGetText.Equals("ON"))
            {
                STEPMANAGER_VALUE.bMSize = true;
                ixlblStatusCount.Font = new Font("Courier New", 11, FontStyle.Bold);
                listboxLog.Font = new Font("Courier New", 8, FontStyle.Regular);
                return true; 

            }
            else
            {
                STEPMANAGER_VALUE.bMSize = false;
                listboxLog.Font = new Font("Courier New", 12, FontStyle.Regular);
                if (ixlblStatus.Width > 560)
                {
                    if (ixlblStatus.Width > 800)                    
                        ixlblStatusCount.Font = new Font("Courier New", 20, FontStyle.Bold);                    
                    else                    
                        ixlblStatusCount.Font = new Font("Courier New", 16, FontStyle.Bold);
                  
                }
                else               
                    ixlblStatusCount.Font = new Font("Courier New", 12, FontStyle.Bold);
                return false;
            }
        }

		private void SocketConnect()
        {            

            if (strClientMyname.Equals("MASTER") && strClientMyIP.Equals("0"))
            {
                //이쪽에 든다면 서버모드이다.
            }
            else
            {
                if (!strClientMyname.Equals("0") && !strClientMyIP.Equals("0"))
                {
                    DKSckClt.Connect(strClientMyIP, 59672); //TCP PORT 59672
                    threadMonitorTimer = new System.Threading.Timer(MornitorCallBack);
                    threadMonitorTimer.Change(0, 1000);
                }
            }
        }

        private void MornitorCallBack(object status)
        {
            if (DKSckClt == null)
            {
                threadMonitorTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }

            if (DKSckClt.IsConnected())
            {
                MornitorData mdData = new MornitorData();
                mdData.strIP = DKSckClt.GetConnectedMyIp();               
                this.Invoke(new MethodInvoker(delegate()
                {
                    if (cbJobFiles.Items.Count < 1 || cbJobFiles.SelectedItem == null || cbJobFiles.SelectedItem.ToString().Length < 3)
                    {
                        mdData.strJobName = "";
                    }
                    else
                    {
                        try
                        {
                            mdData.strJobName = cbJobFiles.SelectedItem.ToString();
                        }
                        catch
                        {
                            mdData.strJobName = "";
                        }
                    }                   
                    
                    string strFrmState = String.Empty;
                    if (CheckShowForm(ref strFrmState))
                    {
                        mdData.strStatus = strFrmState;
                    }
                    else
                    {                        
                        if (DKStepManager.Item_bInteractiveMode)
                        {
                            mdData.strStatus = "INTERACTIVE";
                        }
                        else
                        {
                            mdData.strStatus = ixlblStatus.Caption;
                        }

                    }                    
                    mdData.strVersion = this.Text;
                }));               
                mdData.strName = strClientMyname;  
                DKSckClt.SendMorniotrData(mdData);
            }
            else
            {
                //DKSckClt.Connect(strClientMyIP, 59670); 
            }
        }  


        private void CheckConfigJobMappingStatus()
        {
            string strGetText = DKStepManager.LoadINI("OPTION", "JOBAUTOMAPPING");
            if (strGetText.Equals("ON"))
            {
                lblMapOn.Text = lblMapOn2.Text = "AUTOJOB ON";
                lblMapOn.BackColor = lblMapOn2.BackColor = Color.YellowGreen;
            }
            else
            {
                lblMapOn.Text = lblMapOn2.Text = "AUTOJOB OFF";
                lblMapOn.BackColor = lblMapOn2.BackColor = Color.Gray;
            }
        }

        private bool CheckAskExitQuestion()
        {
            string strAskExit = DKStepManager.LoadINI(constOption, "ASKEXIT");

            if (strAskExit.Equals("ON"))
            {
                return true;
            }

            return false;
        }

        private void CheckBarcodeMode(bool bState)
        {
            // 바코드 옵션 로딩
            strGetUseBarcode   = DKStepManager.LoadINI(constOption, constUseBarcode);
            strGetUseSubId     = DKStepManager.LoadINI(constOption, constUseSubId); 
            strGetStartBarcode = DKStepManager.LoadINI(constOption, constStartBarcode);
            strGetWipSize      = DKStepManager.LoadINI(constOption, constWipLength);
            strGetSubIdSize    = DKStepManager.LoadINI(constOption, constSubIdLength);
            strGetOOBBarcode   = DKStepManager.LoadINI(constOption, constOOBbarcode);
            try
            {
                iWipSize = int.Parse(strGetWipSize);
                iSubIdSize = int.Parse(strGetSubIdSize);
            }
            catch
            {
                iWipSize = 15;
                iSubIdSize = 12;
            }
            
            if (strGetUseSubId.Equals("ON"))
            {
                CrossThreadIssue.ChangeTextControl(axiLabelWip2, "SUB ID");                
            }
            else
            {
                CrossThreadIssue.ChangeTextControl(axiLabelWip2, "");                
            }

            if (strGetUseBarcode.Equals("ON") && bState)
            {
                CrossThreadIssue.AppendEnabled(txtBoxBarcode, true);
            }
            else
            {
                CrossThreadIssue.AppendEnabled(txtBoxBarcode, false);
            }       
        }
        private void DateSave()
        {
            string strMonth = DateTime.Now.Month.ToString();
            string strDay   = DateTime.Now.Day.ToString();
            DKStepManager.SaveINI(constDate, constMonth, strMonth);
            DKStepManager.SaveINI(constDate, constDay,   strDay);
        }

        private bool IsChangeDate()
        {
            string strMonth = DKStepManager.LoadINI(constDate, constMonth);
            string strDay   = DKStepManager.LoadINI(constDate, constDay);
            string strChangeResetTime = DKStepManager.LoadINI(constOption, "COUNTRESETTIME");
            bUseResetCountTime = strChangeResetTime.Equals("ON");

            if(strMonth.Equals("0") || strDay.Equals("0"))
            {
                //최초 이므로 날짜 저장
                DateSave();
                //테스트 카운트 리셋
                CountReset();
                return false;                
            }
          
            string strBeforeMonth = string.Empty;
            string strBeforeDay   = string.Empty;
            string strNowMonth = string.Empty;
            string strNowDay   = string.Empty;

            try
            {
                strBeforeMonth = DKStepManager.LoadINI(constDate, constMonth);
                strBeforeDay   = DKStepManager.LoadINI(constDate, constDay);
                strNowMonth = DateTime.Now.Month.ToString();
                strNowDay   = DateTime.Now.Day.ToString();
                if(!strBeforeMonth.Equals(strNowMonth) || !strBeforeDay.Equals(strNowDay))
                {
                    if (bUseResetCountTime) // 00시에 바뀌지 않고 설정한 시간대에 바뀌길 원할경우.
                    {
                        try
                        {
                            int iSetHour = int.Parse(DKStepManager.LoadINI(constOption, "COUNTRESETHOUR"));
                            int iSetMin = int.Parse(DKStepManager.LoadINI(constOption, "COUNTRESETMIN"));

                            int iNowHour = DateTime.Now.Hour;
                            int iNowMin = DateTime.Now.Minute;

                            if (iNowHour > iSetHour)
                            {
                                DateSave();
                                return true;
                            }
                            if (iNowHour == iSetHour && iNowMin >= iSetMin)
                            {
                                DateSave();
                                return true;
                            }
                        }
                        catch
                        {
                            DateSave(); 
                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        //날짜가 바뀌었으면 오늘날짜 저장
                        DateSave();
                        return true;       //디폴트는 00시에 바뀐다.
                    }
                }
                else
                {
                    return false;
                }

            }
            catch { return true; }
            
        }

        private void CountReset()
        {
            DKStepManager.SaveINI(constOption, constCountPass, "0");
            DKStepManager.SaveINI(constOption, constCountFail, "0");
            DKStepManager.SaveINI(constOption, constCountTotal, "0");
            iPass  = 0;
            iFail  = 0;
            iTotal = 0;

        }

        private void EndSignalOptionLoad()
        {
            //매번 읽는 주체가 스레드로 읽어들여서 Config.save 에 deadlock 현상예상때문에 static 으로 변경
            string tmpStr = String.Empty;
            for (int i = 0; i < (int)ENDOPTION.END; i++)
            {
                tmpStr = DKStepManager.LoadINI(constOption, (i + ENDOPTION.RESULTOK).ToString());
                DKStepManager.bUsedEndOption[i] = tmpStr.Equals("ON");
            }

            for (int i = 0; i < DKStepManager.bUsedEndOption.Length; i++)
            {
                if(DKStepManager.bUsedEndOption[i])
                {
                    for(int j = 0; j < DKStepManager.strEndOptionCommand[i].Length; j++)
                    {
                        DKStepManager.strEndOptionCommand[i][j] = 
                            DKStepManager.LoadINI("SIGNALOPTION", ((i + ENDOPTION.RESULTOK).ToString() + (j + 1).ToString()));
                    }
                    
                }                
            }
        }

        private void CountLoad()
        {
            string tmpStr = DKStepManager.LoadINI(constOption, "PASSRATE");
            bPassRate = tmpStr.Equals("ON");

            if (bPassRate)                
                CrossThreadIssue.ChangeTextControl(LabelTOTAL, "RATE");
            else
                CrossThreadIssue.ChangeTextControl(LabelTOTAL, "TOTAL");                

            if (IsChangeDate()) //날짜가 바뀌었으면 카운트 오토 리셋.
            {
                DeviceLogging(listboxResultLog, true, "# # # # # # #  TEST LOG START # # # # # #", false, 0);
                CountReset();
            }

            try
            {
                CrossThreadIssue.ChangeTextControl(ixlblPass, DKStepManager.LoadINI(constOption, constCountPass));
                CrossThreadIssue.ChangeTextControl(ixlblFail, DKStepManager.LoadINI(constOption, constCountFail));
                                
                if (bPassRate)
                {
                    double dRate = (double)(int.Parse(ixlblPass.Caption)) / (double)(int.Parse(ixlblPass.Caption) + int.Parse(ixlblFail.Caption)) * 100;
                    if (dRate > 0)
                        CrossThreadIssue.ChangeTextControl(ixlblTotal, dRate.ToString("0.00") + " %");                        
                    else
                        CrossThreadIssue.ChangeTextControl(ixlblTotal, "0 %");                        
                }
                else
                    CrossThreadIssue.ChangeTextControl(ixlblTotal, DKStepManager.LoadINI(constOption, constCountTotal));                    
            }
            catch 
            {
                return;
            }
            
            try
            {
                iPass = int.Parse(ixlblPass.Caption);
                iFail = int.Parse(ixlblFail.Caption);
                iTotal = iPass + iFail;//int.Parse(ixlblTotal.Caption); 
            }
            catch
            {
                iPass = iFail = iTotal = 0;
            }
            
            CheckBarcodeMode(true);

            strClientMyname = DKStepManager.LoadINI("UPDATE", "MYNAME");
            strClientMyIP   = DKStepManager.LoadINI("UPDATE", "IP");
            if (strClientMyname.Equals("MASTER") && strClientMyIP.Equals("0"))
            {
                CrossThreadIssue.ChangeBackColor(axiBuildVer, Color.LimeGreen);
            }
            else
            {
                CrossThreadIssue.ChangeBackColor(axiBuildVer, Color.Gainsboro);
            }

            EndSignalOptionLoad();
        }
        private void CountSave()
        {            
            DKStepManager.SaveINI(constOption, constCountPass, ixlblPass.Caption);
            DKStepManager.SaveINI(constOption, constCountFail, ixlblFail.Caption);
            DKStepManager.SaveINI(constOption, constCountTotal, ixlblTotal.Caption);
            
        }
        private void ConnectorCountSave()
        {
            //CABEL COUNT
            string[] strCableName = new string[5];
            string[] strCableSpec = new string[5];
            string[] strCableUsage = new string[5];
            bool bChkWarning = true;
            for (int i = 0; i < strCableName.Length; i++)
            {
                strCableName[i] = String.Empty;
                strCableSpec[i] = "0";
                strCableUsage[i] = "0";
                try
                {
                    if (bUseCable[i])
                    {
                        iCableUsage[i]++;                                               
                        dataGridConnector.Rows[i].Cells[3].Value = (iCableSpec[i] - iCableUsage[i]).ToString();
                        dataGridConnector.Rows[i].Cells[2].Value = iCableUsage[i].ToString();
                        DKStepManager.SaveINI("CABLECOUNT", "CABLEUSAGE" + i.ToString(), iCableUsage[i].ToString());

                        double dRate = (double)iCableUsage[i] / (double)iCableSpec[i];
                        if ((double)iWarn <= (double)(dRate * 100))
                        {
                            bChkWarning = false;
                        }
                        
                    }

                }
                catch (System.Exception ex)
                {
                    string strEx = ex.Message;
                }
            }
            
            if (!bChkWarning)
            {
                ShowMessage("WARNING", "CONNECTOR CABLE : You have exceeded count! ");
            }
            
        }

        private bool ConnectorCountCheck()
        {
            //CABEL COUNT
            string[] strCableName = new string[5];
            string[] strCableSpec = new string[5];
            string[] strCableUsage = new string[5];
            bool bChkWarning = true;
            for (int i = 0; i < strCableName.Length; i++)
            {
                strCableName[i] = String.Empty;
                strCableSpec[i] = "0";
                strCableUsage[i] = "0";
                try
                {
                    if (bUseCable[i])
                    {                        
                        double dRate = (double)iCableUsage[i] / (double)iCableSpec[i];
                        if ((double)iWarn <= (double)(dRate * 100))
                        {
                            bChkWarning = false;
                        }
                    }

                }
                catch (System.Exception ex)
                {                   
                    ShowMessage("WARNING", "ConnectorCountCheck Error" + Environment.NewLine + ex.Message);
                    return false;
                }
            }

            if (!bChkWarning)
            {
                ShowMessage("WARNING", "CONNECTOR CABLE : You have exceeded count! ");
                return false;
            }
            return true;
        }

        private void ControlLoad(CheckBox cbox, string strTitle, string strName)
        {
            string strGetText = DKStepManager.LoadINI(strTitle, strName);
            if (strGetText.Equals("ON"))
            { cbox.Checked = true; }
            else { cbox.Checked = false; }
        }

        private void ControlSave(CheckBox cbox, string strTitle, string strName)
        {
            string tmpCheck = "OFF";
            if (cbox.Checked) tmpCheck = "ON";
            DKStepManager.SaveINI(strTitle, strName, tmpCheck);
        }
        private void ConnectorCountLoad()
        {
            //CABEL COUNT
            string[] strCableName  = new string[5];
            string[] strCableSpec  = new string[5];
            string[] strCableUsage = new string[5];
            
            try
            {
                iWarn = int.Parse(DKStepManager.LoadINI(constOption, constCountWarn));
            }
            catch (System.Exception ex)
            {
                string strEx = ex.Message;
                DKStepManager.SaveINI(constOption, constCountWarn, "90");
                iWarn = 90;
            }
            
            for (int i = 0; i < strCableName.Length; i++)
            {
                try
                {
                    strCableName[i] = String.Empty;
                    strCableSpec[i] = String.Empty;
                    strCableUsage[i] = String.Empty;

                    strCableName[i] = DKStepManager.LoadINI("CABLECOUNT", "CABLENAME" + i.ToString());
                    strCableSpec[i] = DKStepManager.LoadINI("CABLECOUNT", "CABLESPEC" + i.ToString());
                    strCableUsage[i] = DKStepManager.LoadINI("CABLECOUNT", "CABLEUSAGE" + i.ToString());

                    if (!strCableName[i].Equals("0"))
                    {
                        dataGridConnector.Rows[i].Cells[0].Value = strCableName[i];
                        iCableSpec[i] = 10000;
                        iCableUsage[i] = 0;
                        if (!strCableSpec[i].Equals("0"))
                        {
                            iCableSpec[i] = int.Parse(strCableSpec[i]);

                        }
                        else { DKStepManager.SaveINI("CABLECOUNT", "CABLESPEC" + i.ToString(), "10000"); }

                        if (!strCableUsage[i].Equals("0"))
                        {
                            iCableUsage[i] = int.Parse(strCableUsage[i]);
                        }
                        else
                        {
                            iCableUsage[i] = 0;
                        }
                        dataGridConnector.Rows[i].Cells[1].Value = iCableSpec[i].ToString();
                        dataGridConnector.Rows[i].Cells[2].Value = iCableUsage[i].ToString();
                        bUseCable[i] = true;
                    }
                    else
                    {
                        dataGridConnector.Rows[i].Cells[0].Value = String.Empty;
                        dataGridConnector.Rows[i].Cells[1].Value = String.Empty;
                        dataGridConnector.Rows[i].Cells[2].Value = String.Empty;
                        bUseCable[i] = false;
                    }
                }
                catch (System.Exception ex)
                {
                    string strEx = ex.Message;
                    bUseCable[i] = false;
                }
                

                try
                {
                    if (strCableName[i].Length > 1 && strCableSpec[i].Length > 0
                        && strCableUsage[i].Length > 0)
                    {
                        dataGridConnector.Rows[i].Cells[3].Value = (int.Parse(strCableSpec[i]) - int.Parse(strCableUsage[i])).ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    string strEx = ex.Message;
                }
            }            
        }

        private void Form1_Load(object sender, EventArgs e){ }

		private void ShowServerPannel()
        {
            /*
             * killforupdate 함수
            foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
            {
                //프로그램명으로 시작되는 프로세스를 모두 죽인다. 엉뚱한 프로세스를 죽이지 않게 IF문을 잘 사용한다.

                if (process.MainWindowTitle.ToUpper().Contains("JAGUAR TELEMATICS TESTER"))
                {
                    //string sss = process.ProcessName;
                    process.Kill();
                }
            }
            */

            string strMyname = DKStepManager.LoadINI("UPDATE", "MYNAME");
            string strMyIP   = DKStepManager.LoadINI("UPDATE", "IP");

            if(strMyname.Equals("MASTER") && strMyIP.Equals("0"))
            {
                if (!btnStart.Enabled) return;
                DKMOTOSCAN.DisConnect();
                DKStepManager.ActorStop();
                SensorColorOff();

                //JLR:59670번  OCU:59671번  TCP:59672  CLUSTER:59673 
                FrmUpdater FrmUpdatePannel = new FrmUpdater(59672); 
                FrmUpdatePannel.ShowDialog();
                FrmUpdatePannel = null;

                string strReason = String.Empty;
                bool bMan = DKStepManager.CheckDevice(ref strReason);
                //btnInteractive.Enabled = bMan;
                CheckMananger();
                ConnectorCountLoad();
                CountLoad();
                CheckConfigJobMappingStatus();

                if (!bMan) ShowMessage("CHECK", strReason);
                
                if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.
                ViewOptionMessage();
            }
            else
            {
                DeviceLogging(listboxLog, true,  "##########################################", false, 0);
                DeviceLogging(listboxLog, false, "##########################################", false, 0);
                DeviceLogging(listboxLog, false, "###                                    ###", false, 0);
                DeviceLogging(listboxLog, false, "###     THIS IS NOT SEVER COMPUTER     ###", false, 0);
                DeviceLogging(listboxLog, false, "###                                    ###", false, 0);
                DeviceLogging(listboxLog, false, "### Please, Go To the Configuration    ###", false, 0);
                DeviceLogging(listboxLog, false, "###                                    ###", false, 0);
                DeviceLogging(listboxLog, false, "###             and Server Settings.   ###", false, 0);
                DeviceLogging(listboxLog, false, "###                                    ###", false, 0);
                DeviceLogging(listboxLog, false, "###  >>>   MYNAME:MASTER    IP:0       ###", false, 0);
                DeviceLogging(listboxLog, false, "###                                    ###", false, 0);
                DeviceLogging(listboxLog, false, "##########################################", false, 0);
                DeviceLogging(listboxLog, false, "##########################################", false, 0);
                return;
            }
            
            
        }

        private void DKMOTOSCAN_MotorolaBarcodeEvent(int iStatus, string strData, string strErrMsg)
        {
            switch (iStatus)
            {
                case (int)STATUS.OK:
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        txtBoxBarcode.Text = strData;
                            KeyPressEventArgs e = new KeyPressEventArgs((char)Keys.Return);
                            CheckBarcodeText(e);
                        }));
                    break;

                case (int)STATUS.ERROR:
                    DeviceLogging(listboxLog, false, strErrMsg, false, 0);
                    break;
                default: break;
            }
        }

        private void CheckZebraScanner()
        {
            string strGetText = DKStepManager.LoadINI("OPTION", "USEZEBRASCANNER");
            if (strGetText.Equals("ON"))
            {

                string strMotoScanPort = DKStepManager.LoadINI("COMPORT", "ZebraScanner");
                              
                if (DKMOTOSCAN == null)
                {
                    DKMOTOSCAN = new DK_MOTOROLA_SCANNER();
                    DKMOTOSCAN.MotorolaBarcodeEvent += new EventMotorola(DKMOTOSCAN_MotorolaBarcodeEvent);
                }
                DKMOTOSCAN.Connect("COM" + strMotoScanPort, 9600);
                DKMOTOSCAN.AutoScanEnable();

            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            SensorColorOff();
                        
            if (QuestionExit())
            {
                //KillForUpdate();  
                OffBreaker();
                DKStepManager.KillDLLGate();
                try
                {
                    if (threadMonitorTimer != null)
                    {                        
                        threadMonitorTimer.Dispose();
                    }
                    DKSckClt.Disconnect(true);               
                    //업데이트 파일 변경
                    DKStepManager.CheckUpdateFiles();
                    DKStepManager.GmesDisconnection();
                }
                catch
                {
                   
                }
                
            }
            else
            {
                string strReason = String.Empty;
                bool bMan = DKStepManager.CheckDevice(ref strReason);
                CheckMananger();
                if (!bMan) ShowMessage("CHECK", strReason);                
                e.Cancel = true;
                if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.
            }
           
            
        }
#endregion

#region UI 제어 관련

        private void ComboListUpdate()
        {                 
            cbJobFiles.Items.Clear();
            string[] files = DKStepManager.GetFileList("JOB");

            if (files != null || files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    cbJobFiles.Items.Add(files[i].ToString());
                
                }
                if (cbJobFiles.Items.Count > 0) cbJobFiles.SelectedIndex = 0;
            }


            bool bTmp = ProgramListSetUp();


            //dgvResetGrid();
        }

        private void PcanStatusView(object obj)
        {
            PCanMonitor tmpData = new PCanMonitor();
            tmpData = STEPMANAGER_VALUE.GetPcanStatus();
            
            try
            {                
                while (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun)
                {
                    tmpData = STEPMANAGER_VALUE.GetPcanStatus();
                    string strMsg = "PCAN:" + tmpData.strStatus + "/ Heavy:" + tmpData.lBusHeavy.ToString() +
                                    " /Light:" + tmpData.lBusLight.ToString() +
                                    " /Off:" + tmpData.lBusOff.ToString() +
                                    " /Reset:" + tmpData.lReset.ToString();

                    CrossThreadIssue.ChangeTextControl(axiBuffCounter3, strMsg);
                    System.Threading.Thread.Sleep(5);                    
                }
            }
            catch
            {
                return;
            }

        }

        private void EtcLogView(object obj)
        {            
            try
            {
                STEPMANAGER_VALUE.ClearEtcMsgQueue();

                while (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun)
                {
                    StringBuilder strEtc = new StringBuilder(4096);

                    if (STEPMANAGER_VALUE.ExcuteEtcMsgQueue(ref strEtc))
                    {
                        DeviceLogging(listboxEtcLog, false, strEtc.ToString(), true, 0);
                    }

                    System.Threading.Thread.Sleep(5);
                    
                }
            }
            catch 
            {
                return;
            }          
        }

        private void BinLogView(object obj)
        {
            StringBuilder strBinBuffer = new StringBuilder(4096);
            char[] charGL = new char[3];
            char[] charSL = new char[3];
            charGL = Encoding.UTF8.GetChars(Encoding.ASCII.GetBytes("[GL]"));
            charSL = Encoding.UTF8.GetChars(Encoding.ASCII.GetBytes("^^"));

            
            string strDtemp3 = String.Empty;
            string[] strTemp3 = new string[1];
            string[] strTemp = new string[1];
            string[] strTemp4 = new string[1];
            int iFrom = 0;

            

            try
            {
                while (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun)
                {
                    StringBuilder strBin = new StringBuilder(4096);

                    if (STEPMANAGER_VALUE.ExcuteBinMsgQueue(ref strBin, ref iFrom))
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            listboxBinLog.BeginUpdate(); 
                        }));
                        
                        strBinBuffer.Append(strBin);

                        if (strBinBuffer.Length > 128)
                        {
                            if (strBinBuffer.ToString().Contains('^'))
                            {
                                strTemp4 = strBinBuffer.ToString().Split('^');

                                for (int i = 0; i < strTemp4.Length - 1; i++)
                                {
                                    if (!String.IsNullOrEmpty(strTemp4[i]))
                                    {
                                        try
                                        {
                                            DeviceLogging(listboxBinLog, false, strTemp4[i], true, iFrom);

                                        }
                                        catch (Exception e)
                                        {
                                            DeviceLogging(listboxBinLog, false, e.Message, true, iFrom);
                                        }
                                    }

                                    strBinBuffer.Clear();
                                    strBinBuffer.Append(strTemp4[strTemp4.Length - 1]);
                                }
                            }
                            else
                            {
                                try
                                {
                                    DeviceLogging(listboxBinLog, false, strBinBuffer.ToString(), true, iFrom);
                                    strBinBuffer.Clear();
                                }
                                catch (Exception e)
                                {
                                    DeviceLogging(listboxBinLog, false, e.Message, true, iFrom);
                                }
                            }

                        }
                        else
                        {
                            if (strBinBuffer.Length > 3)
                            {
                                if (!strBinBuffer.ToString().Contains("^^"))
                                {
                                    if (strBinBuffer.ToString().Contains("^["))
                                    {
                                        strDtemp3 = strBinBuffer.ToString().Replace("^[", "^^[");
                                        strTemp3 = strDtemp3.Split(charSL);

                                        for (int t = 0; t < strTemp3.Length - 1; t++)
                                        {
                                            if (!String.IsNullOrEmpty(strTemp3[t]))
                                            {
                                                try
                                                {
                                                    DeviceLogging(listboxBinLog, false, strTemp3[t], true, iFrom);

                                                }
                                                catch (Exception e)
                                                {
                                                    DeviceLogging(listboxBinLog, false, e.Message, true, iFrom);
                                                }
                                            }
                                            //Application.DoEvents();
                                        }

                                        strBinBuffer.Clear();
                                        strBinBuffer.Append(strTemp3[strTemp3.Length - 1]);
                                    }

                                }
                                else
                                {
                                    strTemp = strBinBuffer.ToString().Split(charSL);

                                    if (strTemp.Length < 2) break;

                                    for (int i = 0; i < strTemp.Length - 1; i++)
                                    {
                                        if (!String.IsNullOrEmpty(strTemp[i]))
                                        {
                                            try
                                            {
                                                DeviceLogging(listboxBinLog, false, strTemp[i], true, iFrom);

                                            }
                                            catch (Exception e)
                                            {
                                                DeviceLogging(listboxBinLog, false, e.Message, true, iFrom);
                                            }
                                        }
                                        //Application.DoEvents();
                                    }
                                    strBinBuffer.Clear();
                                    strBinBuffer.Append(strTemp[strTemp.Length - 1]);
                                }
                            }
                        }

                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            listboxBinLog.TopIndex = listboxBinLog.Items.Count - 1;
                            listboxBinLog.EndUpdate();
                        }));
                    }

                    System.Threading.Thread.Sleep(1);
                    //Application.DoEvents();
                }
            }
            catch { }
            
            return;
        }

        private void TimerView(double dData, string strDate)
        {
            
            try
            {
                STEPMANAGER_VALUE.strTactTime = dData.ToString("0.0");

                this.Invoke(new MethodInvoker(delegate()
                {
                    isegTimer.Value = dData;                    
                }));

                CrossThreadIssue.ChangeTextControl(ixlblStatusDate, strDate);

            }
            catch { }

            if (dRunningValue + 1 < dData)
            {
                dRunningValue = dData;

                CrossThreadIssue.ChangeFontColor(ixlblStatus, Color.DarkOrange);
                 try
                {
                    this.Invoke(new MethodInvoker(delegate() { 
                        progressInspection.ForeColor = Color.DarkOrange;
                    }));
                }
                 catch { }
                //여기서 깜빡깜빡
            }
            else
            {
                try
                {
                    if (dRunningValue + 0.5 < dData)
                    {
                        CrossThreadIssue.ChangeFontColor(ixlblStatus, Color.Coral);
                        this.Invoke(new MethodInvoker(delegate() { progressInspection.ForeColor = Color.Coral; }));
                    }
                    else
                    {
                        CrossThreadIssue.ChangeFontColor(ixlblStatus, Color.Orange);
                        this.Invoke(new MethodInvoker(delegate() { progressInspection.ForeColor = Color.Orange; }));
                             
                    }
                }
                catch {  }
                
            }

        }
        
        private void SetStatus(int iResultCode)
        {
            string strText = String.Empty;
            Color tmpColor = new Color();
            switch (iResultCode)
            {
                    
                case (int)STATUS.OK:        strText = "PASS";    tmpColor = Color.Green; break;
                case (int)STATUS.NG:        strText = "FAIL";    tmpColor = Color.Crimson; break;
                case (int)STATUS.STOP:      // 검사공통 요청 STOP 일경우 READY 로 표시되도록 요청.
                case (int)STATUS.READY:     strText = "READY";   tmpColor = Color.Black;                    
                                            this.Invoke(new MethodInvoker(delegate() {
                                                ixlblStatusCount.Caption = ""; progressInspection.Value = 0;
                                            }));break;
                case (int)STATUS.DSUB:      strText = "RETRY";   tmpColor = Color.DarkCyan;
                                            this.Invoke(new MethodInvoker(delegate()
                                            {
                                                ixlblStatusCount.Caption = ""; progressInspection.Value = 0;
                                            })); break;
                case (int)STATUS.RUNNING:   strText = "TESTING"; tmpColor = Color.Orange; break;
                case (int)STATUS.MESERR:    strText = "MES";     tmpColor = Color.Crimson; break;
                case (int)STATUS.CHECK:     strText = "CHECK";   tmpColor = Color.SaddleBrown; break;
                case (int)STATUS.EMPTY:     strText = "EMPTY";   tmpColor = Color.SaddleBrown; break;
                     
              
                default: strText = (STATUS.NONE + iResultCode).ToString(); tmpColor = Color.SaddleBrown; break; 
            }

            CrossThreadIssue.ChangeTextControl(ixlblStatus, strText);
            CrossThreadIssue.ChangeFontColor(ixlblStatus, tmpColor);

            this.Invoke(new MethodInvoker(delegate()
            {
                progressInspection.ForeColor = tmpColor;
            }));
            
            

        }

        private bool BeforeTestOpition()
        {
            string[] strCommands = new string[3];
            string strLoadCommand = String.Empty;
            string strListName = "BEFORESTART";

            string strUsed = DKStepManager.LoadINI(constOption, strListName);

            if (!strUsed.Equals("ON")) return true;
            int iCmd = (int)STATUS.NONE;
            
            double dDelaySec = 1.0;
            for (int i = 0; i < strCommands.Length; i++)
            {                
                strLoadCommand = DKStepManager.LoadINI("SIGNALOPTION", (strListName + (i + 1).ToString()));
                if (strLoadCommand.Length > 4)
                {
                    for (int iRetry = 0; iRetry < 3; iRetry++)
                    {
                        if (i == 0)
                            dDelaySec = 0; //첫 명령은 노딜레이.
                        else
                            dDelaySec =1.0;
                        iCmd = DKStepManager.InteractiveCommand((int)COMSERIAL.DIO, strLoadCommand, "", dDelaySec, (int)MODE.SENDRECV);
                        if (iCmd == (int)STATUS.OK) break;
                        
                    }
                }
            }            
            
            return true;
        }

        private void ScannerOKSignalFunc()
        {

            int iCmd = (int)STATUS.NONE;

            double dDelaySec = 2.0;
            string[] strLoadCommand = new string[1];
            strLoadCommand[0] = "RELAY_CLICK_CHECK";            

            for (int i = 0; i < strLoadCommand.Length; i++)
            {
                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    if (i == 0)
                        dDelaySec = 0; //첫 명령은 노딜레이.
                    else
                        dDelaySec = 1.0;

                    if (iRetry > 0) dDelaySec = 0.5;

                    iCmd = DKStepManager.InteractiveCommand((int)COMSERIAL.DIO, strLoadCommand[i], "", dDelaySec, (int)MODE.SENDRECV);
                    if (iCmd == (int)STATUS.OK) break;

                }
            }

            return;
        }

        private void DsubRetrySignalFunc()
        {           
         
            int iCmd = (int)STATUS.NONE;

            double dDelaySec = 2.0;
            string[] strLoadCommand = new string[4];
            strLoadCommand[0] = "RESET";
            strLoadCommand[1] = "RELAY_OFF_PRIMARY_EXT";
            strLoadCommand[2] = "RELAY_CLICK_CHECK";
            strLoadCommand[3] = "RELAY_ON_READY";

            for (int i = 0; i < strLoadCommand.Length; i++)
            {
                if (i == 1 && !DKStepManager.CheckExtPwr()) //외부파워를 쓰지 않는다면 EXT 릴레이는 건너뛴다.
                {
                    continue;
                }

                for (int iRetry = 0; iRetry < 3; iRetry++)
                {
                    if (i == 0)
                        dDelaySec = 0; //첫 명령은 노딜레이.
                    else
                        dDelaySec = 1.0;

                    if (iRetry > 0) dDelaySec = 0.5;

                    iCmd = DKStepManager.InteractiveCommand((int)COMSERIAL.DIO, strLoadCommand[i], "", dDelaySec, (int)MODE.SENDRECV);
                    if (iCmd == (int)STATUS.OK) break;

                }               
            }
            
            return;
        }

        private void TestResultSignalFunc(object obj)
        {
            //throw new ArgumentNullException("Exception TEST"); //강제예외발생방법.

            int iRes = (int)obj;

            string[] strCommands = new string[3];
            string strLoadCommand = String.Empty;
            string strListName = String.Empty;

            int iDx = (int)ENDOPTION.RESULTOK;
            switch (iRes)
            {
                case (int)STATUS.OK:        iDx = (int)ENDOPTION.RESULTOK;    break;
                case (int)STATUS.NG:        iDx = (int)ENDOPTION.RESULTNG;    break;
                case (int)STATUS.CHECK:     iDx = (int)ENDOPTION.RESULTCHK;   break;
                case (int)STATUS.STOP:      iDx = (int)ENDOPTION.USERSTOP;    break;
                case (int)STATUS.EMPTY:     iDx = (int)ENDOPTION.RESULTEMPTY; break;
                case (int)STATUS.MESERR:    iDx = (int)ENDOPTION.RESULTMES;   break;
                case (int)STATUS.EJECT:     iDx = (int)ENDOPTION.RESULTEJECT; break;
                //로봇팔 스캐너 미인식시 시그널 추가
                case (int)STATUS.ERROR:     iDx = (int)ENDOPTION.RESULTERROR; break;
                default: InitialStartButtonSet(true); return;
            }

            if(!DKStepManager.bUsedEndOption[iDx])            
            {
                InitialStartButtonSet(true);
                return;
            }

            int iCmd = (int)STATUS.NONE;

            double dDelaySec = 2.0;
            System.Diagnostics.Stopwatch swStartSignalChecker = new System.Diagnostics.Stopwatch();

            for (int i = 0; i < DKStepManager.strEndOptionCommand[iDx].Length; i++)
            {
                if (DKStepManager.strEndOptionCommand[iDx][i].Length > 4)
                {
                    for (int iRetry = 0; iRetry < 6; iRetry++)
                    {
                        if (i == 0)
                            dDelaySec = 0; //첫 명령은 노딜레이.
                        else
                            dDelaySec = 1.0;

                        if (iRetry > 0) dDelaySec = 0.5;

                        string str1ndCommand = String.Empty; // ON 신호용
                        str1ndCommand = DKStepManager.strEndOptionCommand[iDx][i];

                        if (DKStepManager.GetPlcMode())
                        {
                            switch (DKStepManager.strEndOptionCommand[iDx][i])
                            {
                                case "RELAY_CLICK_OK":
                                case "RELAY_CLICK_NG":                                                                
                                case "RELAY_CLICK_CHECK":
                                case "RELAY_CLICK_EMPTY":
                                case "RELAY_CLICK_ERROR":
                                    str1ndCommand = str1ndCommand.Replace("_CLICK_", "_ON_");
                                    break;
                                default: break;
                            }

                        }

                        iCmd = DKStepManager.InteractiveCommand((int)COMSERIAL.DIO, str1ndCommand, "", dDelaySec, (int)MODE.SENDRECV);

                        if (iCmd == (int)STATUS.OK)
                        {
                            if(DKStepManager.GetPlcMode())
                            {                                
                                string str2ndCommand = String.Empty; // OFF 신호용
                                switch(DKStepManager.strEndOptionCommand[iDx][i])
                                {
                                    case "RELAY_CLICK_OK":
                                    case "RELAY_CLICK_NG":                                    
                                    case "RELAY_CLICK_CHECK":
                                    case "RELAY_CLICK_EMPTY":
                                    case "RELAY_CLICK_ERROR":
                                            str2ndCommand = DKStepManager.strEndOptionCommand[iDx][i];
                                            str2ndCommand = str2ndCommand.Replace("_CLICK_", "_OFF_");
                                            swStartSignalChecker.Reset();
                                            swStartSignalChecker.Start();
                                            while(!DKStepManager.GetSensorDic((int)DIOPIN.START).Equals((int)SENSOR.OFF))
                                            {
                                                System.Threading.Thread.Sleep(50);
                                                if(swStartSignalChecker.Elapsed.Minutes > 30)
                                                {
                                                    swStartSignalChecker.Stop(); break;
                                                }                                       
                                            }
                                            for (int i2ndRetry = 0; i2ndRetry < 6; i2ndRetry++)
                                            {
                                                double d2ndDelaySec = 0.2;

                                                int iCmd2 = DKStepManager.InteractiveCommand((int)COMSERIAL.DIO, str2ndCommand, "", d2ndDelaySec, (int)MODE.SENDRECV);
                                                if (iCmd2 == (int)STATUS.OK)
                                                {
                                                    break;
                                                }

                                            }
                                             break;

                                    default: break;
                                }                           
                            }                            
                            break;
                        }

                    }
                }
            }
            InitialStartButtonSet(true);

            switch (iRes)
            {
                case (int)STATUS.OK:
                    if (CheckMemory())
                    {
                        TestResultSave(iRes); //결과 저장부분 
                        SetStatus(iRes);
                        DKStepManager.DisconnectPorts((int)COMSERIAL.CCM);
                        DKStepManager.ActorStop();
                        Application.Restart();
                        return;
                    }
                    break;
                default: break;
            }    

            return;
        }

        private void TestResultSignal(int iRes)
        {
            if (iRes.Equals((int)STATUS.OK))    //최종결과가 OK 이면 LAST NG 리스트는 클리어.
                DKStepManager.ClearLastNGList();

             InitialStartButtonSet(false);
             DestroyThread(InterSignalThread);
             InterSignalThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(TestResultSignalFunc));
             InterSignalThread.Start(iRes); 
        }

        private void ResultSet()
        {
            int iRes = ResultLastDes();

            if (IsChangeDate()) //날짜가 바뀌었나?
            {
                CountReset();
            }

            if (iRes != (int)STATUS.OK)
            {
                this.Invoke(new MethodInvoker(delegate () { btnDisplay.PerformClick(); }));
            }

            switch (iRes)
            {
                //case (int)STATUS.NONE: SetStatus((int)STATUS.OK); break;
                case (int)STATUS.OK: iPass++; SetStatus((int)STATUS.OK); break;
                case (int)STATUS.NG:
                    iFail++; SetStatus((int)STATUS.NG);
                    CrossThreadIssue.GridScrollingMove(dataGridView1, iLastResultIdx);
                    break;
                case (int)STATUS.CHECK:
                    SetStatus((int)STATUS.CHECK);
                    CrossThreadIssue.GridScrollingMove(dataGridView1, iLastResultIdx);
                    break;
                case (int)STATUS.TIMEOUT:
                    iFail++; SetStatus((int)STATUS.NG);
                    CrossThreadIssue.GridScrollingMove(dataGridView1, iLastResultIdx);
                    break;
                default: SetStatus(iRes); break;

            }

            TestResultSave(iRes);
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    ixlblPass.Caption = iPass.ToString();
                    ixlblFail.Caption = iFail.ToString();
                    iTotal = iPass + iFail;

                    if (bPassRate)
                    {
                        double dRate = (double)iPass / (double)(iPass + iFail) * 100;
                        if (dRate > 0)
                            ixlblTotal.Caption = dRate.ToString("0.00") + " %";
                        else
                            ixlblTotal.Caption = "0 %";
                    }
                    else
                        ixlblTotal.Caption = iTotal.ToString();

                    CountSave(); 
                }));

            }
            catch
            { }

                       
        }

        private void RepeatTestFunc()
        {
            if (!bRepeatMode) return;

            DestroyThread(RepeatThread);

            RepeatThread = new System.Threading.Thread(ClickStartButton);
            RepeatThread.Start();
        }

        private void ClickStartButton()
        {
            System.Threading.Thread.Sleep(2000);

            this.Invoke(new MethodInvoker(delegate()
            {
                if (btnStart.Enabled)
                {
                    btnStart.PerformClick();
                }
            }));


        }

        private void LockSlot(int uIdx)
        {
            int iCol = (int)uIdx + 1;

            if (uIdx <= (int)DEFINES.END)
            {
                dataGridView1.Columns[iCol].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
           
        }

        private void UnLockSlot(int uIdx)
        {
            int iCol = (int)uIdx + 1;

            if (uIdx <= (int)DEFINES.END)
            {
                dataGridView1.Columns[iCol].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                
            }
           
        }

        private void ChangeGrdColColor(bool bChk, int uIdx)
        {
            if (!DKStepManager.ActorCheckPort(uIdx)) return;

            int iCol = (int)uIdx + 1;
            if (!bChk)
            {
                if (uIdx <= (int)DEFINES.END)
                {
                    dataGridView1.Columns[iCol].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                }               
            }
            else
            {
                
                if (uIdx <= (int)DEFINES.END)
                {
                    dataGridView1.Columns[iCol].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                }
               
            }
        }

        private void ButtonSet(bool bState)
        {            
            CrossThreadIssue.AppendEnabled(panelGMES, !bState);
            CrossThreadIssue.AppendEnabled(panelORACLE, !bState);

            CrossThreadIssue.AppendEnabled(btnExit, !bState);
            CrossThreadIssue.AppendEnabled(btnEdit, !bState);
            CrossThreadIssue.AppendEnabled(btnConfig, !bState);
            CrossThreadIssue.AppendEnabled(btnDisplay, !bState);      
            CrossThreadIssue.AppendEnabled(btnStart, !bState);
            CrossThreadIssue.AppendEnabled(btnStop, bState);
            CrossThreadIssue.AppendEnabled(cbJobFiles, !bState);
            CrossThreadIssue.AppendEnabled(btnInteractive, !bState);
            CrossThreadIssue.AppendEnabled(lblVersion, !bState);
                        
            CheckBarcodeMode(!bState);         
                       
        }

        private void DisplayNaming(DataGridView targetView, int iRow, int iCol, string strData)
        {
            targetView.Rows[iRow].Cells[0].Value = (iRow + 1).ToString();
            targetView.Rows[iRow].Cells[iCol].Value = strData;
            targetView.Rows[iRow].Height = 33;
            if(iCol==1) targetView.Rows.Add();
            targetView.Rows[iRow + 1].Height = 33;     
        }

        public void GridWriter(DataGridView targetView, int iRow, string strResult, string strRecvData, Color colData, string strCMax, string strCMin, string strLapse, int itmpRow)
        {

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                try
                {         
                    for (int j = 0; j < targetView.Columns.Count; j++)
                    {
                        targetView.Rows[iRow].Cells[j].Style.BackColor = Color.LightYellow;
                    }

                    targetView.Rows[iRow].Cells[2].Value = strResult;
                    if (strCMin.Length > 0) targetView.Rows[iRow].Cells[3].Value = strCMin;
                    if (strCMax.Length > 0) targetView.Rows[iRow].Cells[4].Value = strCMax;
                    targetView.Rows[iRow].Cells[5].Value = strRecvData;
                    targetView.Rows[iRow].Cells[6].Value = strLapse;
                    targetView.Rows[iRow].Cells[2].Style.BackColor = colData;

                    //#if DEBUG
                    //#if false

                    //20250522, masked 처리. passwordchar 처리
                    string strComp = DKStepManager.GetJOBString(itmpRow, (int)sIndex.ACTION);	
                    string pwdData = string.Empty;
                           
                    if (strComp.Equals("ENC"))
                    {
                        if (strCMin.Length > 0)     //변경된 값
                        {
                            pwdData = new string('*', strCMin.Length);
                            targetView.Rows[iRow].Cells[3].Value = pwdData;
                        }
                        else                        //col 에 고정된 const 값
                        {
                            string strColData = DKStepManager.GetJOBString(itmpRow, (int)sIndex.MIN);
                            if (strColData.Length > 0)
                            {
                                pwdData = new string('*', strColData.Length);
                                targetView.Rows[iRow].Cells[3].Value = pwdData;
                            }
                        }

                        if (strCMax.Length > 0) //
                        {
                            pwdData = new string('*', strCMax.Length);
                            targetView.Rows[iRow].Cells[4].Value = pwdData;
                        }
                        else
                        {
                            string strColData = DKStepManager.GetJOBString(itmpRow, (int)sIndex.MAX);
                            if (strColData.Length > 0)
                            {
                                pwdData = new string('*', strColData.Length);
                                targetView.Rows[iRow].Cells[4].Value = pwdData;
                            }
                        }

                        if (strRecvData.Length > 0)
                        {
                            pwdData = new string('*', strRecvData.Length);
                            targetView.Rows[iRow].Cells[5].Value = pwdData;
                        }                        
                    }
//#endif

                    if (!STEPMANAGER_VALUE.bInteractiveMode)
                    {
                        
                        for (int i = iRow + 1; i < targetView.Rows.Count - 1; i++)
                        {
                            targetView.Rows[i].Cells[2].Value = String.Empty;
                            targetView.Rows[i].Cells[5].Value = String.Empty;
                            targetView.Rows[i].Cells[6].Value = String.Empty;
                            for (int j = 0; j < targetView.Columns.Count; j++)
                            {
                                targetView.Rows[i].Cells[j].Style.BackColor = Color.Ivory;
                            }
                        }

                        targetView.Rows[iRow].Cells[2].Selected = strResult.Equals("WAIT");
                        
                        if (!strResult.Equals("WAIT") && DKStepManager.Item_bTestStarted)
                        {   
                            //PKS
                            //if (targetView.Rows[iRow + 1].Cells[1].Value != null)
                            //{
                            //    targetView.Rows[iRow + 1].Cells[2].Value = "TEST";
                            //    //targetView.Rows[iRow + 1].Cells[2].Selected = true;
                            //    for (int j = 0; j < targetView.Columns.Count; j++)
                            //    {
                            //        targetView.Rows[iRow + 1].Cells[j].Style.BackColor = Color.Khaki;
                            //    }
                            //}
                            
                            ixlblStatusCount.Refresh();
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                }

            }));

            if (!DKStepManager.Item_bInteractiveMode) CrossThreadIssue.GridScrollingLock(targetView, iRow);
            
        }

        private void CheckSlot()
        {
            DKStepManager.SelectChannel((uint)COMSERIAL.DIO, true);

            /*나중 멀티채널 체크를 위해
            for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
            {
                DKStepManager.SaveSlot(constOption, "SLOT" + i.ToString(), "True");

                if (DKStepManager.ActorCheckPort(i))
                {                    
                    DKStepManager.SelectChannel((uint)i, true);
                }
            }*/
        }

        private void ShowMessage(string strTitle, string strMsg)
        {
            CrossThreadIssue.AppendVisible(ixlblWannMsg, true);
            CrossThreadIssue.AppendTextOn(ixlblWannMsg, Environment.NewLine + strTitle + Environment.NewLine + Environment.NewLine + strMsg, true);

            this.Invoke(new MethodInvoker(delegate() {txtBoxBarcode.Focus(); }));
        }

        private void ViewOptionMessage()
        {
            //하단 옵션 표시를 위해 문자열 만들기
            string strOptMsg = String.Empty;

            if (DKStepManager.LoadINI("DIOPIN", "PINUSE").Equals("ON"))
                strOptMsg = "PIN:ON ";
            else
                strOptMsg = "PIN:OFF ";

            if (DKStepManager.LoadINI(constOption, "USEPLC").Equals("ON"))
                strOptMsg += "PLC:ON ";
            else
                strOptMsg += "PLC:OFF ";

            if (DKStepManager.LoadINI(constOption, "USEBARCODE").Equals("ON"))
                strOptMsg += "USB:ON ";
            else
                strOptMsg += "USB:OFF ";

            if (DKStepManager.LoadINI(constOption, "JOBAUTOMAPPING").Equals("ON"))
                strOptMsg += "JOBMAP:ON";
            else
                strOptMsg += "JOBMAP:OFF";

            CrossThreadIssue.ChangeTextControl(axiEngCounter, strOptMsg);            
            CreateSecCount();
        }

        private void CreateSecCount()
        {
            string tmpstrClientMyname = DKStepManager.LoadINI("UPDATE", "MYNAME");
            string tmpstrClientMyIP = DKStepManager.LoadINI("UPDATE", "IP");
            if (tmpstrClientMyname.Equals("MASTER") && tmpstrClientMyIP.Equals("0"))
            {
                STEPMANAGER_VALUE.CreateSecCount();
                CrossThreadIssue.ChangeTextControl(axiBuildVer, "H" + STEPMANAGER_VALUE.GetSecCount1().ToString() + " L" + STEPMANAGER_VALUE.GetSecCount2().ToString());                
            }
            else
            {
                CrossThreadIssue.ChangeTextControl(axiBuildVer, tmpstrClientMyname);
            }

        }

        private bool ProgramListSetUp()
        {
            CrossThreadIssue.AppendVisible(ixlblWannMsg, false);
            ViewOptionMessage();

            int iCount = 0;
            if (cbJobFiles.Items.Count > 0)
            {
                string tmpItemString = String.Empty;
                
                tmpItemString = cbJobFiles.SelectedItem.ToString();

                if (tmpItemString.Length < 3)
                {
                    ShowMessage("WARNING", "CAN NOT TEST START. SELECT JOB FILE AND TRY AGAIN.");
                    DeviceLogging(listboxLog, false, "CAN NOT TEST START. SELECT JOB FILE AND TRY AGAIN:" + tmpItemString + "," + cbJobFiles.SelectedItem.ToString(), true, 0);
                    return false;
                }               
                
                string tmpStr = DKStepManager.LoadINI(constOption, "LASTFILE");
                                
                if (tmpStr.ToString().Length < 3)
                {
                    tmpStr = tmpItemString;
                }
                 
                if (!cbJobFiles.Items.Contains(tmpStr))
                {
                    //ShowMessage("CAN NOT FIND " + tmpStr + " FILE!");
                    return false;
                }


                this.Invoke(new MethodInvoker(delegate()
                {
                    cbJobFiles.SelectedIndex = cbJobFiles.Items.IndexOf(tmpStr);
                }
                ));

                DKStepManager.JOBListClear();
                string strLoadMessage = String.Empty;
                if (!DKStepManager.LoadStep(tmpStr, ref strLoadMessage))
                {
                    ShowMessage("WARNING", strLoadMessage);
                    SetStatus((int)STATUS.ERROR);
                    DKStepManager.JOBListClear();
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add();
                    }));
                    return false;
                }
                else
                {
                    SetStatus((int)STATUS.READY);
                }

                iCount = dataGridView1.Rows.Count;
                this.Invoke(new MethodInvoker(delegate()
                {
                    for (int i = 1; i < iCount - 1; i++)
                    {
                        dataGridView1.Rows.RemoveAt(0);            
                    }

                    string tmpString = String.Empty;
                    string minString = String.Empty;
                    string maxString = String.Empty;
                    string skipString = String.Empty;

                    int iDx = 0;
                    iSeqTotalCount = 0;
                    bSkipList = new bool[DKStepManager.GetJOBListCount()];

                    for (int i = 0; i < DKStepManager.GetJOBListCount(); i++)
                    {
                        tmpString = DKStepManager.GetJOBString(i, (int)sIndex.DISPLAY);
                        minString = DKStepManager.GetJOBString(i, (int)sIndex.MIN);
                        maxString = DKStepManager.GetJOBString(i, (int)sIndex.MAX);
                        skipString = DKStepManager.GetJOBString(i, (int)sIndex.ACTION);

                        //iSeqTotalCount++;

                        if (skipString.Equals("RUN") || skipString.Equals("ENC"))
                        {
                            bSkipList[i] = false;
                            iSeqTotalCount++;

                            if (tmpString.Length > 0)
                            {

                                dataGridView1.Rows[iDx].Tag = i.ToString();
                                DisplayNaming(dataGridView1, iDx, 1, tmpString);
                                DisplayNaming(dataGridView1, iDx, 3, minString);
                                DisplayNaming(dataGridView1, iDx, 4, maxString);
                                iDx++;
                            }
                        }
                        else
                        {
                            bSkipList[i] = true;
                        }

                    }

                }
                ));

                
            }
            else
            {
                DKStepManager.JOBListClear();

                iCount = dataGridView1.Rows.Count - 1;
                for (int i = 1; i < iCount; i++)
                {
                    dataGridView1.Rows.RemoveAt(0);                   
                }
            }
         
            return true;
        }

#endregion

#region UI 기본

        

        private void FrmFaMain_Shown(object sender, EventArgs e)
        {
            string strReason = String.Empty;
            Application.DoEvents();
            bool bMan = DKStepManager.CheckDevice(ref strReason); //CONNECTION
            try
            {
                if (!bMan) ShowMessage("CHECK", strReason);                
            }
            catch{}
            
            if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.
            IsRestart();
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void FrmFaMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void FrmFaMain_Click(object sender, EventArgs e)
        {
            if (DKStepManager.IsPopUp())
            {
                DKStepManager.PopupActive();
                return;
            }
        }               

        private void listboxLog_MouseClick(object sender, MouseEventArgs e)
        {
            LogBoxScrollSizeReset(listboxLog);
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private bool BarcodeLogicProcess_OOB()
        {
            if (txtBoxBarcode.Text.Length > 20)
            {
                txtBoxBarcode.Text = txtBoxBarcode.Text.Trim();
                txtBoxWip1.Text = txtBoxBarcode.Text;
                txtBoxBarcode.Text = String.Empty;
                DKStepManager.Item_OOBLABEL = txtBoxWip1.Text;
                return true;
            }
            else
            {              
                return false;
            }
            
        }

        private bool BarcodeLogicProcess_SUBID()
        {
            if (txtBoxBarcode.Text.Length.Equals(iSubIdSize))
            {
                txtBoxBarcode.Text = txtBoxBarcode.Text.Trim();
                txtBoxWip2.Text = txtBoxBarcode.Text;
                txtBoxBarcode.Text = String.Empty;
                DKStepManager.Item_SUBID = txtBoxWip2.Text;
                return true;
            }
            else
            {                
                return false;
            }         
        }

        private bool BarcodeLogicProcess_WIP()
        {
            if (txtBoxBarcode.Text.Length.Equals(iWipSize))
            {
                txtBoxBarcode.Text = txtBoxBarcode.Text.Trim();
                txtBoxWip1.Text = txtBoxBarcode.Text;
                txtBoxBarcode.Text = String.Empty;
                DKStepManager.Item_WIPID = txtBoxWip1.Text;
                return true;
            }
            else
            {               
                return false;
            }         
        }

        private void BarcodeLogicProcess_NgMsg()
        {
            ShowMessage("WARNING", "CHECK BARCODE");
            txtBoxWip1.Text = String.Empty;
            txtBoxWip2.Text = String.Empty;
            DKStepManager.Item_WIPID = String.Empty;
            DKStepManager.Item_SUBID = String.Empty;
            DKStepManager.Item_OOBLABEL = String.Empty;
            InitialStartButtonSet(true);
        }

        private bool CheckBarcodeState()
        {
            bool bBarcodeLogic1 = true;
            bool bBarcodeLogic2 = true;

            if (strGetUseBarcode.Equals("ON"))
            {
                DKStepManager.Item_bUseBarcode = true;
                if (String.IsNullOrEmpty(txtBoxWip1.Text))
                    bBarcodeLogic1 = false;

                if (strGetUseSubId.Equals("ON"))
                {
                    DKStepManager.Item_bUseSubId = true;
                    if (String.IsNullOrEmpty(txtBoxWip2.Text))
                        bBarcodeLogic2 = false;

                    if (bBarcodeLogic1 && bBarcodeLogic2)
                        return true;
                    else
                        return false;
                }
                else
                {
                    DKStepManager.Item_bUseSubId = false;
                    return bBarcodeLogic1;
                }

            }
            else
            {
                DKStepManager.Item_bUseBarcode = false;
                return true;
            }
                            
        }

        private void CheckBarcodeText(KeyPressEventArgs e)
        {
            bool bBarcodeFlag = false;
            bool bBarcodeFlagCheck = false;

            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
                return;
            }
            
            if (e.KeyChar == (char)Keys.Return)
            {
                if (strGetOOBBarcode.Equals("ON"))
                {
                    bBarcodeFlag = BarcodeLogicProcess_OOB();
                    if (bBarcodeFlag) bBarcodeFlagCheck = bBarcodeFlag;
                }
                else
                {
                    bBarcodeFlag = BarcodeLogicProcess_WIP();
                    if (bBarcodeFlag) bBarcodeFlagCheck = bBarcodeFlag;
                }

                if (strGetUseSubId.Equals("ON"))
                {
                    bBarcodeFlag = BarcodeLogicProcess_SUBID();
                    if (bBarcodeFlag) bBarcodeFlagCheck = bBarcodeFlag;
                }

                txtBoxBarcode.Text = String.Empty;

                if (CheckBarcodeState())
                {
                    e.Handled = true;
                    
                    if (strGetStartBarcode.Equals("ON"))
                    {                         
                        this.Invoke(new MethodInvoker(delegate() { btnStart.PerformClick(); }));
                    }
                    else
                    {
                        CrossThreadIssue.AppendVisible(ixlblWannMsg, false);

                        if (DKStepManager.GetPlcMode())
                        {
                            ScannerOKSignalFunc();
                        }

                    }

                    return;
                }
                else
                {
                    if (!bBarcodeFlagCheck)
                        BarcodeLogicProcess_NgMsg();
                    else
                        CrossThreadIssue.AppendVisible(ixlblWannMsg, false);
                    e.Handled = true;
                    return;
                }

            }

            if (!strGetOOBBarcode.Equals("ON")) //oob 공정에서는 특수문자도 들어온다.
            {
                if ((e.KeyChar > 47 && e.KeyChar < 58) ||
                                    (e.KeyChar > 64 && e.KeyChar < 91) ||
                                    (e.KeyChar > 96 && e.KeyChar < 123) ||
                                    e.KeyChar == '-' || e.KeyChar == 8)
                { }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void txtBoxBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckBarcodeText(e);
        }

        private void listboxLog_DrawItem(object sender, DrawItemEventArgs e)
        {           
            
            try
            {
                if (e.Index < 0) return;
                //listboxLog.DrawMode = DrawMode.OwnerDrawFixed;
                Font fFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                //e.DrawBackground();
                e = new DrawItemEventArgs(e.Graphics, fFont, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor,
                                          Color.LightBlue);//

                Brush myBrush = Brushes.Goldenrod;

                int iIdxTx = listboxLog.Items[e.Index].ToString().IndexOf("[TX]");
                int iIdxRx = listboxLog.Items[e.Index].ToString().IndexOf("[RX]");

                if (iIdxRx > 0) myBrush = Brushes.Cyan;


                e.Graphics.DrawString(listboxLog.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                e.DrawFocusRectangle();
            }
            catch(Exception ex)
            {
                string strEx = ex.Message.ToString();
                return;
            }

        }

        private void chkBoxMesOn_CheckedChanged(object sender, EventArgs e)
        {
            ControlSave(chkBoxMesOn, constOption, "MESON");
            STEPMANAGER_VALUE.bUseMesOn = chkBoxMesOn.Checked;
            this.Invoke(new MethodInvoker(delegate()
            {

                //if (chkBoxMesOn.Checked)
                //{
                //    DKStepManager.GMES_INIT();
                //}
                //else
                //{
                //    DKStepManager.GMES_OFF();
                //}                                
                //txtBoxBarcode.Focus();
                //
                //CSMES
                if (STEPMANAGER_VALUE.bUseOSIMES)
                {
                    if (chkBoxMesOn.Checked)
                    {
                        ixlblMesInfo.Caption = "";
                        lblGmesVersion.Text = "";
                        DKStepManager.GMES_INIT();
                    }
                    else
                    {
                        DKStepManager.OSIMesDisConnect();
                    }
                }
                else
                {
                    if (chkBoxMesOn.Checked)
                    {
                        ixlblMesInfo.Caption = "";
                        lblGmesVersion.Text = "";
                        DKStepManager.GMES_INIT();
                    }
                    else
                    {
                        DKStepManager.GMES_OFF();
                    }
                }

                txtBoxBarcode.Focus();
            }));  
        }

        //private void chkBoxMesOn_MouseDown(object sender, MouseEventArgs e)
        //{
        //    string tmpAskMesPassword = DKStepManager.LoadINI("GMES", "MESASKPASSWORD");

        //    if (!tmpAskMesPassword.Equals("ON"))
        //    {
        //        return;
        //    }

        //    //LGEVH 202306
        //    bool bCurrentStatus = chkBoxMesOn.Checked;

        //    FrmPassWord tmpFrm = null;
        //    tmpFrm = new FrmPassWord();
        //    tmpFrm.ShowDialog();
        //    PWUSER testUser = new PWUSER();
        //    int iAuth = tmpFrm.IsOK(ref testUser);
        //    if (iAuth.Equals((int)ACCOUNT.SUPERUSER)
        //            || (iAuth.Equals((int)ACCOUNT.USER) && testUser.bMes))
        //    {
        //        chkBoxMesOn.Checked = !bCurrentStatus;
        //    }
        //    else
        //    {
        //        chkBoxMesOn.Checked = bCurrentStatus;
        //    }
        //    tmpFrm.Dispose();
        //}

        private void ixlblStatus_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() {
                SetUiFlagManual();
                MoveCurrentControls();
                txtBoxBarcode.Focus(); 
            }));
        }

        private void axiLabelWip1_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void isegTimer_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void LabelTOTAL_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void LabelFAIL_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void LabelPass_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void ixlblTotal_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void ixlblFail_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void ixlblPass_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void ixlblMesInfo_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void axiLabelBarcode_OnClick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void listboxLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {            
            WideLogWindow();
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void listboxBinLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WideLogWindow();
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void listboxEtcLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WideLogWindow();
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void listboxResultLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WideLogWindow();
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void listboxLog_Click(object sender, EventArgs e)
        {
            if (STEPMANAGER_VALUE.bProgramRun) return;

            try
            {
                StringBuilder strBinLogging = new StringBuilder(4096);
                for (int i = 0; i < listboxLog.Items.Count; i++)
                {
                    strBinLogging.Append(listboxLog.Items[i].ToString() + Environment.NewLine);
                }
                Clipboard.SetText(strBinLogging.ToString());
            }
            catch { }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        private void SelectionJobCommitted()
        {
            string tmpStr = DKStepManager.LoadINI(constOption, "LASTFILE");

            if (!tmpStr.Equals(cbJobFiles.SelectedItem.ToString()))
            {
                DKStepManager.SaveINI(constOption, "LASTFILE", cbJobFiles.SelectedItem.ToString());
                bool bTemp = ProgramListSetUp();
            }

        }

        private void cbJobFiles_SelectionChangeCommitted(object sender, EventArgs e)
        {

            if (DKStepManager.LoadINI(constOption, "CHANGEJOBPASSWORD").Equals("ON"))
            {
                DKMOTOSCAN.DisConnect();
                DKStepManager.ActorStop();
                SensorColorOff();
                //MainFormSleepFuction(10);
                //Prevent 2015.03.26 DK.SIM  

                FrmPassWord tmpFrm = null;
                tmpFrm = new FrmPassWord();
                tmpFrm.ShowDialog();
                PWUSER testUser = new PWUSER();
                if (tmpFrm.IsOK(ref testUser) == (int)ACCOUNT.SUPERUSER || 
                    (tmpFrm.IsOK(ref testUser) == (int)ACCOUNT.USER && testUser.bJob))
                {
                    SelectionJobCommitted();
                }
                else
                {
                    string tmpStr = DKStepManager.LoadINI(constOption, "LASTFILE");
                    cbJobFiles.SelectedIndex = cbJobFiles.FindString(tmpStr);
                }

                tmpFrm.Dispose();
                string strReason = String.Empty;
                bool bMan = DKStepManager.CheckDevice(ref strReason);
                //btnInteractive.Enabled = bMan;
                CheckMananger();
                ConnectorCountLoad();
                CountLoad();
                CheckConfigJobMappingStatus();
                if (!bMan) ShowMessage("CHECK", strReason);                
                if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.
                ViewOptionMessage();

            }
            else
            {
                SelectionJobCommitted();
    
            }

            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }
         

        private void gvSheetListCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(((CheckBox)sender).Text.ToString());
            uint uIdx = uint.Parse(((CheckBox)sender).Text.ToString());
            bool bChk = ((CheckBox)sender).Checked;
            DKStepManager.SelectChannel(uIdx, bChk);
            ChangeGrdColColor(bChk, (int)uIdx);

        }

        private void ShowCellData(int iCol, int iRow, int tmpRow)
        {
            try
            {
                string[] tmpStr = new string[9];                
                for(int i = 0; i < 5; i++) 
                {   //TEST NAME, RESULT, MIN, MAX, MEASURE,
                    try
                    {
                        tmpStr[i] = dataGridView1.Rows[tmpRow].Cells[i+1].Value.ToString();    //TEST NAME
                    }
                    catch 
                    {
                    	tmpStr[i] = String.Empty;
                    }
                }
                JOBDATA tmpJob = new JOBDATA();
                if (DKStepManager.GetLineCommand(iRow, ref tmpJob))
                {
                    tmpStr[5] = tmpJob.PAR1;
                    tmpStr[6] = tmpJob.COMPARE;
                    tmpStr[7] = tmpJob.OPTION;
                    tmpStr[8] = tmpJob.CASENG;
                }              

                tmpStr[0] = tmpStr[0].Replace("[", String.Empty);
                tmpStr[0] = tmpStr[0].Replace("]", String.Empty);
                
                string strFileName = "celldata.txt";
                StringBuilder sbText = new StringBuilder(4096);
                sbText.AppendFormat("[TEST NAME][{0}]" + Environment.NewLine +
                                    "[RESULT   ][{1}]" + Environment.NewLine +
                                    "[MIN      ][{2}]" + Environment.NewLine +
                                    "[MAX      ][{3}]" + Environment.NewLine +
                                    "[MEASURE  ][{4}]" + Environment.NewLine +
                                    "[PAR1     ][{5}]" + Environment.NewLine +
                                    "[COMPARE  ][{6}]" + Environment.NewLine +
                                    "[OPTION   ][{7}]" + Environment.NewLine +
                                    "[CASENG   ][{8}]" + Environment.NewLine,
                                    tmpStr[0], tmpStr[1], tmpStr[2], tmpStr[3], tmpStr[4], tmpStr[5],
                                    tmpStr[6], tmpStr[7], tmpStr[8]);

                ShowDataToNotePad(strFileName, sbText.ToString());
                return;
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
            return;   
        }
          
        private bool QuestionExit()
        {
            if (bForceExit) return true; //강제종료

            //무인화 라인의 경우에 일일이 눌러서 끌수 없으므로 걍 종료시키자.
            if (DKStepManager.LoadINI(constOption, "USEPLC").Equals("ON"))
            {
                return true;
            }

            if (CheckAskExitQuestion())
            {
                if (DialogResult.Yes != MessageBox.Show("DO YOU WANT TO EXIT THIS PROGRAM? ", "CONFIRM MESSAGE", MessageBoxButtons.YesNo))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DestroyThread(RepeatThread);
            DestroyThread(InterSignalThread);
            DKStepManager.IfPlcModeIsAllOffSignal();
            DKStepManager.StopStatusTimer();
            Application.DoEvents();

            if (DKMOTOSCAN != null && DKMOTOSCAN.IsConnected() == (int)CONNECTSTATE.CONNECTED)
            {
                DKMOTOSCAN.DisConnect();
            }
            
            try
            {
                //SSCHO TEST
                                
                StopNetworkCheck_OracleConnection();

                DKStepManager.DisconnectPorts((int)COMSERIAL.CCM);
                DKStepManager.DioReset();

                DKMOTOSCAN.DisConnect();
                DKStepManager.ActorStop();              
                GridSizeAutoSave();
                DKStepManager.SaveSuffixFile(); 
                Application.Exit();
            }
            catch 
            {
                KillMyApplication();
            }
            
          
        }

        private void GridSizeAutoSave()
        {
            for (int i = (int)GRIDMAININDEX.NO; i < (int)GRIDMAININDEX.END; i++)
            {
                DKStepManager.SaveINI("MAINGRID", "WIDTH_" + (i + GRIDMAININDEX.NO).ToString(), dataGridView1.Columns[i].Width.ToString());
            }

        }

        private void GridWidthAutoSize()
        {
            int[] iWsize = new int[(int)GRIDMAININDEX.END];

            int iColsWidth = 0;

            for (int i = (int)GRIDMAININDEX.NO; i < (int)GRIDMAININDEX.END; i++)
            {
                try
                {
                    iWsize[i] = int.Parse(DKStepManager.LoadINI("MAINGRID", "WIDTH_" + (i + GRIDMAININDEX.NO).ToString()));
                }
                catch
                {
                    iWsize[i] = 0;
                }
                iColsWidth += iWsize[i];
            }

            if (iColsWidth > dataGridView1.Width || iColsWidth < dataGridView1.Width - 20)
            {
                DefaultGridColWidth();
                return;
            }

            for (int i = (int)GRIDMAININDEX.NO; i < (int)GRIDMAININDEX.END; i++)
            {
                if (iWsize[i] > 10)
                {
                    dataGridView1.Columns[i].Width = iWsize[i];
                }
                else
                {
                    DefaultGridColWidth();
                    return;
                }
            }
        }

        private void DefaultGridColWidth()
        {
            // 그리드 디폴트 열너비 기능
            int iColsWidth = dataGridView1.Width - 17;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                switch (i)
                {
                    case 0: //NO
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.04); break;
                    case 1: //TEST NAME
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.39); break;
                    case 2: //RESULT
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.08); break;
                    case 3: //MIN
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.05); break;
                    case 4: //MAX
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.2); break;
                    case 5: //MEASURE
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.2); break;
                    case 6: //LAPSE
                        dataGridView1.Columns[i].Width = (int)(iColsWidth * 0.04); break;
                    default: break;
                }
            }
        }

        private void InitialStartButtonSet(bool bEnable)
        {
            CrossThreadIssue.AppendEnabled(lblVersion, bEnable);
            CrossThreadIssue.AppendEnabled(btnStart, bEnable);
            CrossThreadIssue.AppendEnabled(btnEdit, bEnable);
            CrossThreadIssue.AppendEnabled(btnConfig, bEnable);
            CrossThreadIssue.AppendEnabled(btnExit, bEnable);
            CrossThreadIssue.AppendEnabled(btnInteractive, bEnable);
            CrossThreadIssue.AppendEnabled(btnDisplay, bEnable);            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {   
            InitialStartButtonSet(false);
            ProgramControl(true, false);            
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }
        
        private void btnStop_Click(object sender, EventArgs e)
        {            
            OffBreaker();
            DKStepManager.InitInspectionRetry(); //전체리트라이 리셋
            STEPMANAGER_VALUE.bDebugLogEnable = false;
           
            try
            {
                if (DKStepManager.IsPopUp())
                {
                    DKStepManager.PopupClose();

                }
            }
            catch { }

            if (DKStepManager.Item_bInteractiveMode) 
            {
                DKStepManager.ReleaseStepCheck(); //스텝체크 이력 삭제
                DKStepManager.Item_bInteractiveMode = false;
                DestroyThread(InterThread);
                DKStepManager.DisconnectPorts((int)COMSERIAL.CCM);
                
                dataGridView1.MultiSelect = false;
                dataGridView1.Enabled = true;
                CrossThreadIssue.ChangeTextControl(ixlblStatusCount, String.Empty);
                ButtonSet(false);                                 
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return; 
            }  
            ManagerSignProcess((int)STATUS.STOP, String.Empty);
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
            
        }
      
        private void btnEdit_Click(object sender, EventArgs e)
        {
            DestroyThread(InterSignalThread);
            DKStepManager.IfPlcModeIsAllOffSignal();
            OffBreaker();
            DKStepManager.InitInspectionRetry(); //전체리트라이 리셋
            DKMOTOSCAN.DisConnect();
            DKStepManager.ActorStop();
            SensorColorOff();

            //Prevent 2015.03.26 DK.SIM  
            FrmPassWord tmpFrm = null;
            tmpFrm = new FrmPassWord();
            tmpFrm.ShowDialog();
            PWUSER testUser = new PWUSER();
            int iAuth = tmpFrm.IsOK(ref testUser);
            if (iAuth.Equals((int)ACCOUNT.SUPERUSER) ||
                (iAuth.Equals((int)ACCOUNT.USER) && testUser.bEdit))
            {                
                if (iAuth.Equals((int)ACCOUNT.SUPERUSER))
                    frmEdit = new FrmEdit(cbJobFiles.SelectedIndex, "SUPERUSER");
                else
                    frmEdit = new FrmEdit(cbJobFiles.SelectedIndex, testUser.strLogName);
                
                frmEdit.ShowDialog();
                frmEdit.Dispose();
                ComboListUpdate();
                bool bTmp = ProgramListSetUp();
            }
            
            //Prevent 2015.03.26 DK.SIM  
            tmpFrm.Dispose();

            string strReason = String.Empty;
            bool bMan = DKStepManager.CheckDevice(ref strReason);
            //MainFormSleepFuction(10);
            //btnInteractive.Enabled = bMan;
            CheckMananger();
            ConnectorCountLoad();
            CountLoad();    
            try
            {
                if (!bMan) ShowMessage("CHECK", strReason);
            }
            catch {}
            
            if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void ShowDataToNotePad(string strFileName, string strText)
        {
            try
            {
                System.Diagnostics.Process[] remoteByName = System.Diagnostics.Process.GetProcessesByName("notepad");

                if (remoteByName.Length > 0)
                {
                    for (int i = 0; i < remoteByName.Length; i++)
                    {
                        string strname = remoteByName[i].MainWindowTitle;

                        if (strname.Contains(strFileName))
                        {
                            remoteByName[i].Kill();
                        }
                    }
                }

                System.IO.File.WriteAllText(DKStepManager.GetLogPath() + strFileName, strText);
                System.Diagnostics.Process.Start(DKStepManager.GetLogPath() + strFileName);
            }
            catch
            {
                return;
            }
            
        }

        private void CheckLgeMD5()
        {
            Assembly currAssembly = Assembly.GetExecutingAssembly();
            string strVersion = currAssembly.GetName().Version.ToString();
            string strRequireVersion = String.Empty;
            bool bCheck = DKStepManager.CheckMD5(strVersion, ref strRequireVersion);
            if (!bCheck)
            {
                bForceExit = true;
                if (String.IsNullOrEmpty(strRequireVersion))
                    MessageBox.Show("It was not run as \"Start\" program.", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("It was not run as \"Start\" program.", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                KillMyApplication();
            }

            STEPMANAGER_VALUE.bIamMD5 = true;
        }

        private void KillMyApplication()
        {
            try
            {
                System.Diagnostics.Process[] remoteByName = System.Diagnostics.Process.GetProcesses();
                string strMyProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

                for (int i = 0; i < remoteByName.Length; i++)
                {
                    if (remoteByName[i].ProcessName.Contains(strMyProcessName))
                    {
                        remoteByName[i].Kill();
                        //return;
                    }
                }
                    
                
            }
            catch
            {
                return;
            }
        }

        private void TestComm()
        {
            
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            //TestOtp();
            //TestComm();

            DestroyThread(InterSignalThread);
            DKStepManager.IfPlcModeIsAllOffSignal();
            OffBreaker();
            DKStepManager.InitInspectionRetry(); //전체리트라이 리셋
            StopNetworkCheck_OracleConnection();
            DKMOTOSCAN.DisConnect();
            DKStepManager.ActorStop();
            SensorColorOff();
            //MainFormSleepFuction(10);
            //Prevent 2015.03.26 DK.SIM  

            FrmPassWord tmpFrm = null;
            tmpFrm = new FrmPassWord();
            tmpFrm.ShowDialog();
            PWUSER testUser = new PWUSER();
            int iAuth = tmpFrm.IsOK(ref testUser);
            if (iAuth.Equals((int)ACCOUNT.SUPERUSER)
                    || (iAuth.Equals((int)ACCOUNT.USER) && testUser.bConfig))
            {
                if (iAuth.Equals((int)ACCOUNT.SUPERUSER))
                    FrmConfig = new FrmConfig((int)ACCOUNT.SUPERUSER, testUser);
                else
                    FrmConfig = new FrmConfig((int)ACCOUNT.USER, testUser);

                FrmConfig.ShowDialog();
                FrmConfig.Dispose();
            }
            //Prevent 2015.03.26 DK.SIM  
            tmpFrm.Dispose();
            // tmpFrm = null;

            string strReason = String.Empty;
            bool bMan = DKStepManager.CheckDevice(ref strReason);
            //btnInteractive.Enabled =  bMan;
            SetUiFlag();
            CheckMananger();
            ConnectorCountLoad();
            CountLoad();
            CheckConfigJobMappingStatus();
            CheckPrimaryMES(false);
            if (!bMan)
                ShowMessage("CHECK", strReason);
            else
                CrossThreadIssue.AppendVisible(ixlblWannMsg, false);

            if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.

            ViewOptionMessage();
            MoveCurrentControls();
            this.Invoke(new MethodInvoker(delegate () { txtBoxBarcode.Focus(); }));

        }

        private void SensorColorOff()
        {
            CrossThreadIssue.ChangeOutRelayColor(axiDioRecv, false);
            CrossThreadIssue.ChangeOutRelayColor(axiDioClear, false);
            CrossThreadIssue.ChangeOutRelayColor(axiDioDelay, false);
            CrossThreadIssue.ChangeOutRelayColor(axiDutRecv, false);
            CrossThreadIssue.ChangeOutRelayColor(axiDutClear, false);
            CrossThreadIssue.ChangeOutRelayColor(axiDutDelay, false);

            CrossThreadIssue.ChangeBackColor(axiSensor0, false);
            CrossThreadIssue.ChangeBackColor(axiSensor1, false);
            CrossThreadIssue.ChangeBackColor(axiSensor2, false);
            CrossThreadIssue.ChangeBackColor(axiSensor3, false);
            CrossThreadIssue.ChangeBackColor(axiSensor4, false);
            CrossThreadIssue.ChangeBackColor(axiSensor5, false);
            CrossThreadIssue.ChangeBackColor(axiSensor6, false);
            CrossThreadIssue.ChangeBackColor(axiSensor7, false);
            CrossThreadIssue.ChangeBackColor(axiSensor8, false);
            CrossThreadIssue.ChangeBackColor(axiSensor9, false);
        }

        private void WideLogWindow()
        {

            if (dataGridConnector.Left != tabControl.Left)
            {
                tabControl.Width = dataGridConnector.Width;
                tabControl.Left  = dataGridConnector.Left;
            }
            else
            {
                tabControl.Width = tabControl.Right - dataGridView1.Left;
                tabControl.Left  = dataGridView1.Left;
            }
          
        }
           
        private void btnDisplay_Click(object sender, EventArgs e)
        {
            RESDATA tmpRes = new RESDATA();
            
            try
            {
                string tmpString = String.Empty;
                int iDx = 0;

                bool bNaming = false; //디스플레이 네이밍은 한차례만 ㅡㅡ;
                  
                        if (!bNaming)//그리드 초기화도 한차례만 ㅡㅡ;
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(2);
                            for (int j = 0; j < dataGridView1.Rows.Count; j++)
                            {
                                dataGridView1.Rows[j].Height = 33;
                            }

                        }

                        for (int j = 0; j < DKStepManager.GetJOBListCount(); j++)
                        {

                            tmpRes = DKStepManager.GetTestResultData((int)DEFINES.SET1, j);
                            
                            if (!bNaming)//디스플레이 네이밍은 한차례만 ㅡㅡ;
                            {
                                string strNoTestName = DKStepManager.GetJOBString(j, (int)sIndex.DISPLAY);
                                if (strNoTestName.Length < 1)
                                {
                                    strNoTestName = "[" + DKStepManager.GetJOBString(j, (int)sIndex.TYPE) + "] " +
                                                       DKStepManager.GetJOBString(j, (int)sIndex.CMD);

                                }
                                dataGridView1.Rows[iDx].Tag = j.ToString();
                                string minString = DKStepManager.GetJOBString(j, (int)sIndex.MIN);
                                string maxString = DKStepManager.GetJOBString(j, (int)sIndex.MAX);
                                DisplayNaming(dataGridView1, iDx, 1, strNoTestName);
                                DisplayNaming(dataGridView1, iDx, 3, minString);
                                DisplayNaming(dataGridView1, iDx, 4, maxString);
                                iDx++;
                            }                            

                            if (String.IsNullOrEmpty(tmpRes.strDisplayName))
                            {
                                tmpString = tmpRes.strCMD;
                            }
                            else
                            {
                                if (tmpRes.strDisplayName.Length < 1)
                                {
                                    tmpString = tmpRes.strCMD;
                                }
                                else
                                {
                                    tmpString = tmpRes.strDisplayName;
                                }

                            }

                            if (tmpRes.iStatus == (int)STATUS.SKIP) //스킵이면 min max measure 표시 하지 말자
                            {
                                tmpRes.ResponseData = tmpRes.strChangeMax 
                                                    = tmpRes.strChangeMin 
                                                    = tmpRes.ResultData
                                                    = tmpRes.LapseTime 
                                                    = String.Empty;
                            }
                            
                            ManagerSignReport(tmpRes, true);                           

                        }
                        bNaming = true;

                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
            
        }

#endregion

#region LOGIC 관련

        private void GateWay_Debug(THREDSTATUS rData)
        {
            bool[] bCommLed = new bool[(int)DOUT.END];
            
            for (int i = 0; i < bCommLed.Length; i++)
            {
                bCommLed[i] = false;
            }

            bCommLed[(int)DOUT.OK]  = rData.bLedStatus[(int)DIOPIN.PINUSE + (int)DOUT.OK];
            bCommLed[(int)DOUT.NG]  = rData.bLedStatus[(int)DIOPIN.PINUSE + (int)DOUT.NG];
            bCommLed[(int)DOUT.CHK] = rData.bLedStatus[(int)DIOPIN.PINUSE + (int)DOUT.CHK];
            bCommLed[(int)DOUT.RDY] = rData.bLedStatus[(int)DIOPIN.PINUSE + (int)DOUT.RDY];
            

            try
            {
                CrossThreadIssue.ChangeTextControl(axiBuffCounter1, " DIO [" + rData.tDio.iBufferCount.ToString("000") + "]");
                CrossThreadIssue.ChangeTextControl(axiBuffCounter2, " SET [" + rData.tSet.iBufferCount.ToString("000") + "] Queue[" + STEPMANAGER_VALUE.IsExistBinMsgQueue().ToString("00") + "]");

                //RELAY STATUS LED 로 변경

                CrossThreadIssue.ChangeOutRelayColor(axiDioRecv, bCommLed[(int)DOUT.OK]);
                CrossThreadIssue.ChangeOutRelayColor(axiDioDelay, bCommLed[(int)DOUT.CHK]);
                CrossThreadIssue.ChangeOutRelayColor(axiDioClear, bCommLed[(int)DOUT.NG]);
                CrossThreadIssue.ChangeOutRelayColor(axiDutRecv, bCommLed[(int)DOUT.RDY]);

                //사이클 엔진 스테이터스
                CrossThreadIssue.ChangeOutRelayColor(axiDutDelay, rData.bEngineStatus);


                CrossThreadIssue.ChangeTextControl(axiCommandStatus, rData.strCommandState);

                //PINUSED, START, IN1, IN2, IN3, BUB1, BUB2, MANUAL1, MANUAL3, SETIN, STOP, END
                CrossThreadIssue.ChangeBackColor(axiSensor0, rData.bLedStatus[(int)DIOPIN.START]);
                CrossThreadIssue.ChangeBackColor(axiSensor1, rData.bLedStatus[(int)DIOPIN.IN1]);
                CrossThreadIssue.ChangeBackColor(axiSensor2, rData.bLedStatus[(int)DIOPIN.IN2]);
                CrossThreadIssue.ChangeBackColor(axiSensor3, rData.bLedStatus[(int)DIOPIN.IN3]);
                CrossThreadIssue.ChangeBackColor(axiSensor4, rData.bLedStatus[(int)DIOPIN.BUB1]);
                CrossThreadIssue.ChangeBackColor(axiSensor5, rData.bLedStatus[(int)DIOPIN.BUB2]);
                CrossThreadIssue.ChangeBackColor(axiSensor6, rData.bLedStatus[(int)DIOPIN.EXTERNAL]);
                CrossThreadIssue.ChangeBackColor(axiSensor7, rData.bLedStatus[(int)DIOPIN.MANUAL3]);
                CrossThreadIssue.ChangeBackColor(axiSensor8, rData.bLedStatus[(int)DIOPIN.SETIN]);
                CrossThreadIssue.ChangeBackColor(axiSensor9, rData.bLedStatus[(int)DIOPIN.STOP]);
                
                CrossThreadIssue.ChangeBackColor(axiSensorDebug, STEPMANAGER_VALUE.bDebugLogEnable);

                this.Invoke(new MethodInvoker(delegate()
                {
                    int iPos = 0;
                    double dCurr = STEPMANAGER_VALUE.GetCurrent(ref iPos);
                    if (iPos > 0 && iPos < 4)
                        axiCurrent.Precision = 4-iPos;
                    else
                        axiCurrent.Precision = 1;
                    axiCurrent.Value = dCurr;

                    if (STEPMANAGER_VALUE.bProgramRun || STEPMANAGER_VALUE.bInteractiveMode)
                    {
                        if (ChartCurrent.get_YAxis(0).Span < dCurr + (dCurr / 10))
                        {
                            ChartCurrent.get_YAxis(0).Span = dCurr + (dCurr / 10);
                        }
                        ChartCurrent.get_Channel(0).AddYNow(dCurr);
                    }

                    try
                    {
                        long size = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
                        double dShowSize = (double)size / (double)1024000;
                        axiDutClear.Caption = dShowSize.ToString("0.0") + " M";        
                        STEPMANAGER_VALUE.dMemoryUsed = dShowSize;
                    }
                    catch 
                    {

                    }                   
                }));
                
            }
            catch {}            
        }

        private bool GateWay_MAINfrm(RESDATA rData)
        {
            //DK_LOGGER tmplog = new DK_LOGGER("MANAGER", true);
            
            bool bRes = true;
            switch(rData.iType)
            {
                case (int)EVENTTYPE.MANAGER: ManagerSignProcess(rData.iStatus, rData.ResultData); break;
                    
                case (int)EVENTTYPE.COMM:
                                        if (!DKPlayChecker.Item_RUN)
                                        {
                                            if (!DKStepManager.Item_bInteractiveMode) 
                                            {
                                                bRes = false;
                                            }
                                            else
                                            {
                                                ManagerSignReport(rData, false);
                                            }
                                        }
                                        else
                                        {                                            
                                            ManagerSignReport(rData, false);                                       
                                            
                                        }
                                        break;
                
                default: bRes = false; break;
            }
   
            return bRes;
        }
        
        private void GateWay_MAINfrm2(int iPort, string strMsg)
        {
            DeviceLogging(listboxLog, false, strMsg, false, 0);

        }

        private void GateWay_MAINfrm3(DEVICEDATA devData)
        {            
            try
            {
                switch (devData.iStatus)
                {
                    case (int)STATUS.OK: dataGridDevice.Rows[0].Cells[devData.iDevNumber].Style.BackColor = Color.Green;
                        dataGridDevice.Rows[0].Cells[devData.iDevNumber].Style.ForeColor = Color.White;
                        dataGridDevice.Rows[0].Cells[devData.iDevNumber].Value = "OK";
                        break;
                    case (int)STATUS.NG: dataGridDevice.Rows[0].Cells[devData.iDevNumber].Style.BackColor = Color.Crimson;
                        dataGridDevice.Rows[0].Cells[devData.iDevNumber].Style.ForeColor = Color.White;
                        dataGridDevice.Rows[0].Cells[devData.iDevNumber].Value = "NG";
                        break;
                    default: dataGridDevice.Rows[0].Cells[devData.iDevNumber].Style.BackColor = Color.Gray;
                        dataGridDevice.Rows[0].Cells[devData.iDevNumber].Style.ForeColor = Color.Black;
                        dataGridDevice.Rows[0].Cells[devData.iDevNumber].Value = "NOT USE";
                        break;
                }
            }
            catch 
            {
            	
            }           
            
        }

        //문제가 있다. 검사종료후 Nullreference Exception 이 나는 현상있어서 skip
        private bool CheckPlcModeLastContinuosNg(ref string strContinuousItem, ref string strContinuousItemResult)
        {
            return false;
            /*
            if (DKStepManager.GetPlcMode()) //PLC 모드이고
            {
                RESDATA tmpData = new RESDATA();
                List<string[]> tmpNgList = new List<string[]>();
                string[] strTmpItem = new string[2] { String.Empty, String.Empty };
                
                try
                {
                    for (int i = 0; i < DKStepManager.GetTestResultDataCount((int)DEFINES.SET1); i++)
                    {
                        tmpData = DKStepManager.GetTestResultData((int)DEFINES.SET1, i);
                        if (tmpData.strCMDTYPE == null || tmpData.strCMDTYPE.Length == 0) return false;
                        if (tmpData.strCMD == null     || tmpData.strCMD.Length == 0) return false;
                        switch (tmpData.strCMDTYPE)
                        {
                            case "PCAN":
                                if (tmpData.iStatus.Equals((int)STATUS.NG)
                                    || tmpData.iStatus.Equals((int)STATUS.CHECK)
                                    || tmpData.iStatus.Equals((int)STATUS.TIMEOUT)
                                    || tmpData.iStatus.Equals((int)STATUS.ERROR))
                                {
                                    strTmpItem[0] = tmpData.strCMD;
                                    strTmpItem[1] = tmpData.ResponseData;
                                    tmpNgList.Add(strTmpItem);

                                }
                                break;
                            default:
                                if (tmpData.strCMD.Contains("DLL_"))
                                {
                                    if (tmpData.iStatus.Equals((int)STATUS.NG)
                                        || tmpData.iStatus.Equals((int)STATUS.CHECK)
                                            || tmpData.iStatus.Equals((int)STATUS.TIMEOUT)
                                                || tmpData.iStatus.Equals((int)STATUS.ERROR))
                                    {
                                        strTmpItem[0] = tmpData.strCMD;
                                        strTmpItem[1] = tmpData.ResponseData;
                                        tmpNgList.Add(strTmpItem);
                                    }
                                }
                                break;
                        }
                    }
                }
                catch 
                {
                    return false;
                }
                

                if (tmpNgList == null || tmpNgList.Count == 0) return false;

                try
                {
                    if (DKStepManager.GetLastNGListCount() == 0)
                    {
                        for (int i = 0; i < tmpNgList.Count; i++)
                        {
                            DKStepManager.AddLastNGItem(tmpNgList[i][0]);
                        }
                        return false;
                    }
                    else
                    {
                        for (int i = 0; i < tmpNgList.Count; i++)
                        {
                            if (DKStepManager.ContainItem(tmpNgList[i][0]))
                            {
                                strContinuousItem = tmpNgList[i][0];
                                strContinuousItemResult = tmpNgList[i][1];
                                return true;
                            }
                        }
                        return false;
                    }
                }
                catch 
                {
                    return false;
                }                
                                
            }
           
            return false;
             */
        }

        private void ManagerSignProcess(int iStatus, string strParam)
        { 
            if(iStatus.Equals((int)STATUS.END))
            {
                int iRes = ResultLastDes();
                int iCount = 0;
                switch (iRes)
                {
                    case (int)STATUS.EMPTY: 
                    case (int)STATUS.STOP: 
                    case (int)STATUS.EJECT:    
                    case (int)STATUS.OK:       
                    case (int)STATUS.MESERR:    break;

                    case (int)STATUS.NG:       
                    case (int)STATUS.CHECK:    
                    case (int)STATUS.ERROR   :
                            //전체리트라이 반영
                        iCount = DKStepManager.CheckInspectionRetry();
                        if (iCount > 0)
                        {
                            this.Invoke(new MethodInvoker(delegate()
                            {                                
                                OffBreaker();
                                STEPMANAGER_VALUE.bDebugLogEnable = false;
                                DKPlayChecker.Item_RUN = false;
                                DKPlayChecker.TimerStop();
                                DKStepManager.AutoTestStop();
                                MainFormSleepFuction(50);
                                DeviceLogging(listboxLog,    false, "### [INSPECTION AUTO RETRY - " + iCount.ToString() + " ] ###", false, 0);
                                DeviceLogging(listboxBinLog, false, "### [INSPECTION AUTO RETRY - " + iCount.ToString() + " ] ###", true, 0);
                                DeviceLogging(listboxEtcLog, false, "### [INSPECTION AUTO RETRY - " + iCount.ToString() + " ] ###", true, 0);
                                DKStepManager.DecreaseInspectionRetry();
                                if(DKStepManager.GetPlcMode())
                                {   //요기 전후진                                    
                                    System.Threading.Thread threadDsubFunc = new System.Threading.Thread(DsubRetrySignalFunc);
                                    threadDsubFunc.Start();
                                    SetStatus((int)STATUS.DSUB);
                                    TestResultSave((int)STATUS.DSUB); //결과 저장부분
                                    InitialStartButtonSet(true); 
                                }                                
                                else
                                    ProgramControl(true, true);

                            }));
                            return;
                        }
                        else
                        {                            
                            string strItemName = String.Empty;
                            string strItemResult = String.Empty;
                            if (CheckPlcModeLastContinuosNg(ref strItemName, ref strItemResult)) //이전중복 NG 아이템이 있다면
                            {
                                //문제가 있다. 검사종료후 Nullreference Exception 이 나는 현상있어서 skip
                                /*
                                this.Invoke(new MethodInvoker(delegate()
                                {
                                    OffBreaker();
                                    STEPMANAGER_VALUE.bDebugLogEnable = false;
                                    DKPlayChecker.Item_RUN = false;
                                    DKPlayChecker.TimerStop();
                                    DKStepManager.AutoTestStop();
                                    MainFormSleepFuction(50);
                                    DeviceLogging(listboxLog, false,    "### [CONTINUOUS FAIL ITEM - " + strItemName + " ] ###", false, 0);
                                    DeviceLogging(listboxBinLog, false, "### [CONTINUOUS FAIL ITEM - " + strItemName + " ] ###", true, 0);
                                    DeviceLogging(listboxEtcLog, false, "### [CONTINUOUS FAIL ITEM - " + strItemName + " ] ###", true, 0);

                                }));
                                SetStatus((int)STATUS.CHECK);
                                InitialStartButtonSet(true); 
                                DKStepManager.ClearLastNGList();
                                ShowMessage("CHECK EQUIPMENT", "CONTINUOUS FAIL >> [" + strItemName  + "]" + Environment.NewLine
                                                            + Environment.NewLine + "- " + strItemResult + " -");
                                TestResultSave((int)STATUS.CHECK); //결과 저장부분
                                return;
                                */
                            }
                                                     
                        }
                        break;
                    default: break;
                }
            }
            
            
            switch (iStatus)
            {
                case (int)STATUS.NONE:       return; 
                case (int)STATUS.POPPINGOFF: this.Invoke(new MethodInvoker(delegate() { this.Opacity = 1; }));
                                             return;
                case (int)STATUS.POPPING:    this.Invoke(new MethodInvoker(delegate() { this.Opacity = 0.8; }));
                                             return;

                case (int)STATUS.EJECT:
                case (int)STATUS.BTNSTOP:
                case (int)STATUS.STOP:
                                           
                                           if (DKPlayChecker.Item_RUN) 
                                           { 
                                               DKPlayChecker.Item_RUN = false;
                                               ProgramControl(false, false);                                               
                                               
                                               if (iStatus == (int)STATUS.EJECT)
                                               {
                                                   TestResultSignal((int)STATUS.EJECT);
                                                   SetStatus((int)STATUS.EJECT);   
                                                   TestResultSave((int)STATUS.EJECT); //결과 저장부분                                                    
                                               }
                                               else
                                               {
                                                   TestResultSignal((int)STATUS.STOP);
                                                   SetStatus((int)STATUS.STOP);   
                                                   TestResultSave((int)STATUS.STOP); //결과 저장부분                                                    
                                               }
                                             
                                           }                                            
                                           return; 

                case (int)STATUS.BTNSTART: if (STEPMANAGER_VALUE.bNowMsgPop) return;
                                           if (!DKPlayChecker.Item_RUN)
                                           {
                                               ProgramControl(true, false);
                                           }
                                          
                                           return;

                case (int)STATUS.CHANGEJOB:
                case (int)STATUS.CHECK:
                case (int)STATUS.GOTO_ERR:
                case (int)STATUS.JUMP_ERR:
                                           if (DKPlayChecker.Item_RUN) { DKPlayChecker.Item_RUN = false; }
                                           else { 
                                               return; 
                                           }
                                           ProgramControl(false, false);
                                           SetStatus(iStatus);
                                           TestResultSave(iStatus); //결과 저장부분     

                                           if (iStatus == (int)STATUS.CHANGEJOB)
                                           {
                                               strJobChangeFileName = strParam;
                                               threadJobChangeTimer.Change(2000, 1000);
                                           }
                                           return;
                                    
                case (int)STATUS.NGSTOP:
                case (int)STATUS.GOTORETRYFAIL:
                case (int)STATUS.TIMEOUT:
                case (int)STATUS.EMPTY:
                case (int)STATUS.MESERR:
                case (int)STATUS.END:       
                                            if (DKPlayChecker.Item_RUN) { DKPlayChecker.Item_RUN = false; }
                                            else {                                                                                             
                                                return;
                                            }                                            
                                            int iRes = ResultLastDes();
                                            TestResultSignal(iRes);
                                            ProgramControl(false, false);                                              
                                            //TestResultSave(iStatus); //결과 저장부분 
                                            //SetStatus(iStatus);
                                            
                                            break;
                                           
                                               
                default:  return; 
            }
            
            ResultSet();
            RepeatTestFunc();
        }

        private bool CheckMemory()
        {
            //메모리 누수로 인해 메모리가 800mega 까지 증가된다면 프로그램을 재기동하자.... 보험용
            if (STEPMANAGER_VALUE.dMemoryUsed > 800)
                return true;
            else
                return false;
        }

        private void TestResultSave(int iStatus) //결과 저장 부분
        {
            string strReason = (STATUS.NONE + iStatus).ToString();
            string strEsTime = "0";

            strEsTime = DKPlayChecker.GetCurrentInspectionTime().ToString("0.0");

            string strJobName = String.Empty;

            this.Invoke( new MethodInvoker(delegate()
                        {                            
                            strJobName = cbJobFiles.SelectedItem.ToString();
                            this.Update();
                        }
            ));


            this.Invoke(new MethodInvoker(delegate()
            {
                if (!iStatus.Equals((int)STATUS.OK))
                {
                    if (!DKStepManager.GetLogWipId().Equals("NONE"))
                    {
                        SaveCurrentImage(DKStepManager.GetLogWipId());
                    }

                }
                else
                {
                    ChartCurrent.Visible = false;
                }
            }));

            InspectionLoggingProcess(iStatus); //WIP ID 별 별도 로깅 로직
            BinLoggingProcess(iStatus);        //BIN 로깅 로직
            SemiResultLogging(strReason, strJobName); //세미리절트 먼저 로깅.
            ForAtcoResultLogging(strReason, strJobName); // ATCO 로깅.

            string tmpStrPri = DKStepManager.LoadINI("OPTION", "PRIMARY");

            switch (tmpStrPri)
            {
                case "ORACLE":
                    DKStepManager.WriteTestResult2(strJobName, strStartTime, strEndTime, strEsTime, strReason, this.Text, ixlblPass.Caption, ixlblFail.Caption, ixlblTotal.Caption);
                    break;
                default:
                    DKStepManager.WriteTestResult(strJobName, strStartTime, strEndTime, strEsTime, strReason, this.Text, ixlblPass.Caption, ixlblFail.Caption, ixlblTotal.Caption, ixlblMesLED.Caption);
                    break;
            }


            
               
        }

        private void SaveCurrentImage(string strWipId)
        {
            Size szTemp = new Size();
            Point poTemp = new Point();

            try
            {
                szTemp = ChartCurrent.Size;
                poTemp = ChartCurrent.Location;
                ChartCurrent.Visible = false;
                ChartCurrent.Width = dataGridView1.Width;
                ChartCurrent.Height = (int)((dataGridView1.Height + axiEngCounter.Height - 15) * 0.7);
                ChartCurrent.Left = this.Right + 1;
                string strFileName = DKStepManager.GetScreenPath() + DateTime.Now.ToString("MMdd-HHmmss") + "_" + strWipId + "-Current.WMF";                
                ChartCurrent.SaveImageToMetaFile(strFileName);
            }
            catch 
            {
            	
            }

            try
            {
                ChartCurrent.Size = szTemp;
                ChartCurrent.Location = poTemp;
            }
            catch { }
        }

        private void InspectionLoggingProcess(int iStatus)
        {
            try
            {
                if (listboxLog.Items.Count > 1)
                {
                    string[] strLog = new string[listboxLog.Items.Count];
                    for (int i = 0; i < listboxLog.Items.Count; i++)
                    {
                        strLog[i] = listboxLog.Items[i].ToString();
                    }

                    string strBinFileName = String.Empty;
                    string strWipInfo = String.Empty;

                    if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))
                        strWipInfo = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                    else
                    {
                        if (!String.IsNullOrEmpty(DKStepManager.Item_WIPID))
                        {
                            strWipInfo = DKStepManager.Item_WIPID;
                        }
                    }

                    if (!String.IsNullOrEmpty(strWipInfo))
                    {
                        if (strWipInfo.Length > 22)  //GM 바코드가 가장 긴경우는 PCB 공정 22자리 이므로 이보다 길 경우는 자른다.
                        {
                            strWipInfo = strWipInfo.Substring(0, 22);
                        }
                        strBinFileName = strWipInfo;
                    }
                    else
                    {
                        //20221014 아래도 SN 으로 변경
                        string strLoggingID = String.Empty;
                        /*
                        bool bStid = DKStepManager.GetExprData("#LOAD:LABEL_STID", ref strLoggingID);
                        if (bStid) strBinFileName = "STID_" + strLoggingID;
                        else
                        {
                            bool bWip = DKStepManager.GetExprData("#LOAD:WIPID", ref strLoggingID);
                            if (bWip) strBinFileName = strLoggingID;
                        }
                        */
                        bool bStid = DKStepManager.GetExprData("#LOAD:LABEL_SN", ref strLoggingID);
                        if (bStid) strBinFileName = "SN_" + strLoggingID;
                        else
                        {
                            bool bWip = DKStepManager.GetExprData("#LOAD:WIPID", ref strLoggingID);
                            if (bWip) strBinFileName = strLoggingID;
                        }
                    }

                    if(!String.IsNullOrEmpty(strBinFileName))
                        DKStepManager.WriteInspectionLogging(strLog, strBinFileName);
                }


            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
        }

        private void BinLoggingProcess(int iStatus)
        {
            try
            {
                string tmpUseNoBin = DKStepManager.LoadINI("OPTION", "NOBINLOG");
                bool bBinLoggingEnable = true;

                if (tmpUseNoBin.Equals("ON")) //최종결과가 OK가 아닐경우에만 로깅하겠다는 옵션
                {
                    if (iStatus.Equals((int)STATUS.OK))
                        bBinLoggingEnable = false;
                }

                if (bBinLoggingEnable && listboxBinLog.Items.Count > 1)
                {
                    string[] strLog = new string[listboxBinLog.Items.Count];
                    for (int i = 0; i < listboxBinLog.Items.Count; i++)
                    {
                        strLog[i] = listboxBinLog.Items[i].ToString();
                    }

                    string strBinFileName = "NO_WIP_" + DateTime.Now.ToString("yyyyMMdd-HHmmss");

                    string strWipInfo = String.Empty;

                    if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))
                        strWipInfo = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                    else
                    {
                        if (!String.IsNullOrEmpty(DKStepManager.Item_WIPID))
                        {
                            strWipInfo = DKStepManager.Item_WIPID;
                        }
                    }

                    if (!String.IsNullOrEmpty(strWipInfo))
                    {
                        if (strWipInfo.Length > 22)  //GM 바코드가 가장 긴경우는 PCB 공정 22자리 이므로 이보다 길 경우는 자른다.
                        {
                            strWipInfo = strWipInfo.Substring(0, 22);
                        }
                        strBinFileName = strWipInfo;
                    }
                    else
                    {
                        string strLoggingID = String.Empty;
                        bool bStid = DKStepManager.GetExprData("#LOAD:LABEL_STID", ref strLoggingID);
                        if (bStid) strBinFileName = "STID_" + strLoggingID;
                        else
                        {
                            bool bWip = DKStepManager.GetExprData("#LOAD:WIPID", ref strLoggingID);
                            if (bWip) strBinFileName = strLoggingID;
                        }
                    }

                    DKStepManager.WriteBinLogging(strLog, strBinFileName);
                }


            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
        }

        private void SemiResultLogging(string strReason, string strJobName)
        {
            string strTestID = String.Empty;

            if (String.IsNullOrEmpty(DKStepManager.Item_WIPID))
            {
                bool bExpr = DKStepManager.GetExprData("#LOAD:WIPID", ref strTestID);
                if (!bExpr || strTestID.Length < 1)
                {
                    //20221014 아래와 같이 변경. STID가 아닌 SN 으로
                    /*
                    bExpr = DKStepManager.GetExprData("#LOAD:LABEL_STID", ref strTestID);
                    if (!bExpr || strTestID.Length < 1)
                    {
                        if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))
                        {
                            strTestID = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                        }
                        else
                        {
                            strTestID = "NONE";
                        }
                    }
                    */

                    bExpr = DKStepManager.GetExprData("#LOAD:LABEL_SN", ref strTestID);
                    if (!bExpr || strTestID.Length < 1)
                    {
                        if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))
                        {
                            strTestID = STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID;
                        }
                        else
                        {
                            strTestID = "NONE";
                        }
                    }
                }
            }
            else
            {
                strTestID = DKStepManager.Item_WIPID;
            }
            //(판정결과, S/N or WIP, 검사절차서이름 정도만 저장이 되도록)
            string strResultLogMsg = DateTime.Now.ToString("yyMMdd-HH:mm:ss") + "," +
                                     strReason + "," + strTestID + "," + strJobName;

            DK_LOGGER tmpLogger = new DK_LOGGER("TESTLOG", false);
            tmpLogger.WriteCommLog(strResultLogMsg, "TESTLOG", true);
            DeviceLogging(listboxResultLog, false, strResultLogMsg, false, 0);

            
        }
        
        private void ForAtcoResultLogging(string strReason, string strJobName) //북미 ATCO 전산 로그를 위한 별도 로깅...
        {
            
            if (DKStepManager.LoadINI(constOption, "FORATCO").Equals("ON"))
            {
                if (String.IsNullOrEmpty(STEPMANAGER_VALUE.strAtcoLoggingPath))
                {
                    ShowMessage("WARNING", "NOT FOUND LOGGING PATH for ATCO SYSTEM");
                    return;
                }

                switch (strReason) //기존 GEN10 이 PASS.FAIL 이란 용어를 쓰는바람에 변경해주자...ㅡ.ㅡ
                {
                    case "OK": strReason = "PASS"; break;
                    case "NG": strReason = "FAIL"; break;
                    default:   break;
                }

                string strActoStid = String.Empty;
                string strActoImei = String.Empty;
                string strAtcoTrace = String.Empty;
                string strAtcoStation = DKStepManager.LoadINI(constOption, "ATCOSTATION");

                /*
                bool bStid = DKStepManager.GetExprData("#LOAD:LABEL_STID", ref strActoStid);
                bool bImei = DKStepManager.GetExprData("#LOAD:LABEL_IMEI", ref strActoImei);
                bool bTrace = DKStepManager.GetExprData("#LOAD:LABEL_TRACE", ref strAtcoTrace);
                
                if(!bStid)  strActoStid = "NONE";
                if(!bImei)  strActoImei = "NONE";
                if(!bTrace) strAtcoTrace = "NONE";
                */

                //20220918 수정 STID, IMEI, 삭제, #SAVE:LABEL_SN 추가.
                string strAtcoSN = String.Empty;
                bool bSN = DKStepManager.GetExprData("#LOAD:LABEL_SN", ref strAtcoSN);
                if (!bSN) strAtcoSN = "NONE";


                StringBuilder sbAtco = new StringBuilder(1024);

               
                /*
                sbAtco.AppendFormat("[{0}][IMEI:{1}][STID:{2}][TRACE:{3}][RESULT:{4}][STATION:{5}][GPS://]"
                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), strActoImei, strActoStid, strAtcoTrace, strReason, strAtcoStation);
                */

                sbAtco.AppendFormat("[{0}][SN:{1}][TRACE:{2}][RESULT:{3}][STATION:{4}][GPS://]"
                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), strAtcoSN, strAtcoTrace, strReason, strAtcoStation);

                DK_LOGGER tmpLogger = new DK_LOGGER("TESTLOG", false);
                tmpLogger.WriteTargetLog(sbAtco.ToString(), STEPMANAGER_VALUE.strAtcoLoggingPath, "Result-" + DateTime.Now.ToString("yyyyMMdd") + ".txt");

            }
            else
            {
                if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strAtcoLoggingPath))
                {
                    ShowMessage("WARNING", "DISABLED CHECK OPTION IN COFING PANNEL(For ATCO)");
                    return;
                }
            }
        }
        
        private void ManagerSignReport(RESDATA rData, bool bDisp) // bool bDisp 는 DISPLAY ALL 의 경우와 아니냐 따라.다르기때문에 쓰여야 한다.
        {           
            if (iLastResult == null)
            {
                return;
            }

            Color tmpColor = new Color();
            string strResult = String.Empty;
            string strData = String.Empty;
            string strMax = String.Empty;
            string strMin = String.Empty;
            int iDx = rData.iPortNum;
            //int iDx = (int)DEFINES.SET1;
            int iSx = rData.iSequenceNum;

            strData = rData.ResultData;
            if (!String.IsNullOrEmpty(rData.strChangeMax)) strMax = rData.strChangeMax;
            if (!String.IsNullOrEmpty(rData.strChangeMin)) strMin = rData.strChangeMin;

            switch (rData.iStatus)
            {
                case (int)STATUS.OK: tmpColor = System.Drawing.Color.LightYellow;
                    strResult = "OK";
                    break;
                case (int)STATUS.NG: tmpColor = System.Drawing.Color.Crimson;
                    strResult = "NG";
                    break;
                case (int)STATUS.SKIP: tmpColor = System.Drawing.Color.LightGray;
                    strResult = "SKIP";
                    break;
                case (int)STATUS.IGNORE: tmpColor = System.Drawing.Color.Ivory;
                    strResult = "OK";
                    break;
                case (int)STATUS.NGSTOP: tmpColor = System.Drawing.Color.Crimson;
                    strResult = "NG:STOP";
                    break;
                case (int)STATUS.MESERR: tmpColor = System.Drawing.Color.Crimson;
                    strResult = "MES";
                    break;
                case (int)STATUS.DELAYLAPSE:
                case (int)STATUS.TIMESTAMP:     
                    tmpColor = System.Drawing.Color.LightYellow;
                    strResult = "WAIT";
                    break;     
                default: tmpColor = System.Drawing.Color.LightGray;
                    strResult = (STATUS.NONE + rData.iStatus).ToString();
                    break;
            }

            if (!rData.iStatus.Equals((int)STATUS.DELAYLAPSE) && !rData.iStatus.Equals((int)STATUS.TIMESTAMP))
                iLastResult[iSx] = rData.iStatus;


            int itmpCol = iDx + 1;
            int itmpRow = -1;

            string tmpTag = String.Empty;

            for (int i = 0; i <= dataGridView1.Rows.Count - 2; i++)
            {
                if (dataGridView1.Rows[i].Tag != null)
                {
                    tmpTag = dataGridView1.Rows[i].Tag.ToString();
                    if (int.Parse(tmpTag) == rData.iSequenceNum)
                    {
                        itmpRow = i;
                        break;
                    }
                }
            }

            if (!bDisp) //gateway로 통해서 온 자료이면, (즉 DISPLAY ALL 버튼으로 오지 않았다면)
            {
                WriteProgress(rData.iSequenceNum, itmpRow);

                if (itmpRow < 0) return; //DISPLAY NAME 없으면 그리드에도 결과를 노출시키지 않는다.

                if (rData.strDisplayName.Length > 0) //DISPLAY NAME 이 없는지 검사.
                {       
                    if (iDx < (int)DEFINES.END)
                    {
                        GridWriter(dataGridView1, itmpRow, strResult, strData, tmpColor, strMax, strMin, rData.LapseTime, rData.iSequenceNum);
                    }
                }
                else
                {
                    if(DKStepManager.Item_bInteractiveMode)
                    {
                         if(rData.strCMD.Equals(DKStepManager.GetJOBString(itmpRow, (int)sIndex.CMD)))
                         {
                             if (iDx < (int)DEFINES.END)
                             {
                                 GridWriter(dataGridView1, itmpRow, strResult, strData, tmpColor, strMax, strMin, rData.LapseTime, rData.iSequenceNum);
                             }
                         }
                    }
                }
            }
            else
            {
                if (iDx < (int)DEFINES.END) //DISPLAY ALL 버튼으로 왔을때는 전부 표시
                {
                    GridWriter(dataGridView1, itmpRow, strResult, strData, tmpColor, strMax, strMin, rData.LapseTime, rData.iSequenceNum);
                }
            }


        }

        private void WriteProgress(int iCount, int itmpRow)
        {
            if (!STEPMANAGER_VALUE.bInteractiveMode)
            {
                int iSkipCount = 0;
                for (int i = 0; i < iCount; i++)
                {
                    if (bSkipList[i]) iSkipCount++;
                }
                int iMakeNumber = iCount - iSkipCount;
                if (iMakeNumber < 0) iMakeNumber = 0;

                int iRate = (int)((double)(iMakeNumber + 1) / (double)iSeqTotalCount * 100);

                this.Invoke(new MethodInvoker(delegate()
                {
                    //progressInspection.Refresh();
                    if (iRate >= 0 && iRate <= 100)
                    {                        
                        progressInspection.Value = iRate;
                        //ixlblStatusCount.Caption = "INSPECTION PROGRESS : " + (itmpRow + 1).ToString() + " / " + iSeqTotalCount.ToString();
                        ixlblStatusCount.Caption = "INSPECTION PROGRESS : " + progressInspection.Value.ToString() + " %";

                    }
                    else
                    {

                        if (iRate > 100)
                        {
                            progressInspection.Value = 100;
                            //ixlblStatusCount.Caption = "INSPECTION PROGRESS : " + (itmpRow + 1).ToString() + " / " + iSeqTotalCount.ToString();
                            ixlblStatusCount.Caption = "INSPECTION PROGRESS : " + progressInspection.Value.ToString() + " %";

                        }
                    }

                }));

            }
            else
            {
                this.Invoke(new MethodInvoker(delegate()
                {                   
                    progressInspection.Value = 0;
                    //ixlblStatusCount.Caption = "INSPECTION PROGRESS : " + (itmpRow + 1).ToString() + " / " + iSeqTotalCount.ToString();
                    ixlblStatusCount.Caption = "INTERACTIVE MODE";

                }));

            }
        }
           
        private void DeviceLogging(ListBox targetBox, bool bClear, string strLogstring, bool bAutoTimeStamp, int iFrom)
        {
            StringBuilder tmpMsg = new StringBuilder(4096);
            tmpMsg.Clear();


            if (bAutoTimeStamp)
            {
                switch (iFrom)
                {
                    case 1: tmpMsg.Append("R"); //RECV중. datascanprocess 에서 찍는거
                        break;
                    case 2: tmpMsg.Append("C"); //CLEAR중. dataclearing 중에 찍는것
                        break;
                    case 3: tmpMsg.Append("M"); //RECV 와 동일하지만 MULTI SEND RECV 프로세스에서 찍는거
                        break;
                    default:
                        tmpMsg.Append(iFrom.ToString());
                        break;
                }

                tmpMsg.Append(DateTime.Now.ToString("#HH:mm:ss.fff# "));
                tmpMsg.Append(strLogstring);

                //strLogstring = DateTime.Now.ToString(iFrom.ToString() + "#HH:mm:ss.fff# ") + strLogstring;     
            }
            else
            {
                tmpMsg.Append(strLogstring);
            }

            try
            {                
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    
                    if (bClear) targetBox.Items.Clear();
                    targetBox.Items.Add(tmpMsg.ToString());
                    if(!targetBox.Equals(listboxBinLog)) targetBox.TopIndex = targetBox.Items.Count - 1;
                    //921Kbps 관계로 속도 느려지는것때문에 이렇게 할수밖에 없음.
                        
                }));
            }
            catch
            {
                return;
            }
           
        }

        private void LogBoxScrollSizeReset(ListBox targetBox)
        {
            if (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun) return;
            if (targetBox.Items.Count < 1) return;
            int tmphzSize = 0;

            try
            {
                if (targetBox.Equals(listboxBinLog))
                {
                    for (int i = 0; i < listboxBinLog.Items.Count; i++)
                    {
                        if ((int)g2.MeasureString(targetBox.Items[i].ToString(), targetBox.Font).Width > tmphzSize)
                            tmphzSize = (int)g2.MeasureString(targetBox.Items[i].ToString(), targetBox.Font).Width;
                    }
                }
                else
                {
                    for (int i = 0; i < listboxLog.Items.Count; i++)
                    {
                        if ((int)g1.MeasureString(targetBox.Items[i].ToString(), targetBox.Font).Width > tmphzSize)
                            tmphzSize = (int)g1.MeasureString(targetBox.Items[i].ToString(), targetBox.Font).Width;
                    }
                }

                targetBox.HorizontalExtent = tmphzSize;


            }
            catch { }

        }
        
        private int ResultLastDes()
        {
            int iRes    = (int)STATUS.NONE;
            int iRtnDes = (int)STATUS.OK;
            iLastResultIdx = 1;
            for (int i = 0; i < iLastResult.Count; i++)
            {
                iRes = iLastResult[i];
                switch (iRes)
                {
                    //case (int)STATUS.NONE:
                    case (int)STATUS.SKIP:
                    case (int)STATUS.JUMP:
                    case (int)STATUS.OK: iLastResultIdx = i+1;  break;
                    case (int)STATUS.CHECK:                        
                                            if (iRtnDes != (int)STATUS.NG)
                                            {
                                                iLastResultIdx = i + 1;
                                                iRtnDes = (int)STATUS.CHECK;
                                            } break;
                    case (int)STATUS.NG: 
                    case (int)STATUS.TIMEOUT: 
                                         iLastResultIdx = i + 1; return (int)STATUS.NG;
                    case (int)STATUS.EMPTY:
                    case (int)STATUS.MESERR:
                                         iLastResultIdx = i + 1; return iRes;
                    case (int)STATUS.ERROR:
                                         iLastResultIdx = i + 1; return (int)STATUS.ERROR;
                    default:                      
                        iLastResultIdx = i + 1; return (int)STATUS.CHECK;
                     
                }
                
            }
            return iRtnDes;
        }

        private void ResultReset()
        {            
            int iResCount = DKStepManager.GetJOBListCount();
            iLastResult = new Dictionary<int, int>();
         
            for(int i = 0; i < iResCount; i++)
            {
                try
                {
                    iLastResult.Add(i, (int)STATUS.NONE);
                }
                catch { }
            }
            this.Invoke(new MethodInvoker(delegate()
            { txtBoxScount.Text = ""; txtBoxCN0.Text = ""; }));      
            
        }

        private void ShowFreezingCount()
        {
            string strCount = DKStepManager.LoadINI("RUNTIME", "DETECTED");
            string strDate  = DKStepManager.LoadINI("RUNTIME", "LASTDATE");
            CrossThreadIssue.ChangeTextControl(axiBuffCounter4, "F(" + strCount + "," + strDate + ")");
        }

        private bool CheckTBLFiles(ref string strReason)
        {
            return DKStepManager.CheckTBLFile("MCTM", ref strReason);
        }

        private void ProgramControl(bool bRunning, bool bByRetry)
        {
            if (bRunning)
            {
                ShowFreezingCount();

                switch (StatusPrimaryMES())
                {
                    case (int)MESPRI.ORACLE:
                        StopNetworkCheck_OracleConnection();
                        break;
                }

                if (DKStepManager.Item_bInteractiveMode) return;

                string strReason = String.Empty;
                if (!CheckTBLFiles(ref strReason))
                {

                    ShowMessage("WARNING", strReason);
                    SetStatus((int)STATUS.READY);
                    InitialStartButtonSet(true); //CM.LEE 발견.
                    return;
                }

                if (!ConnectorCountCheck())
                {
                    InitialStartButtonSet(true); //CM.LEE 발견.
                    return;
                }

                if (!CheckBarcodeState())
                {
                    ShowMessage("WARNING", "CHECK BARCORDE !");

                    if(DKStepManager.GetPlcMode())
                    {
                        TestResultSignal((int)STATUS.ERROR);
                        SetStatus((int)STATUS.READY);                        
                    }
                    else
                    {
                        
                        InitialStartButtonSet(true); //CM.LEE 발견.
                    }                    
                    return;
                }
                                
                string strErrMsg = String.Empty;
                iLastResult = null;              
                CheckSlot();

                if (!ProgramListSetUp())
                {                                     
                    SetStatus((int)STATUS.READY);
                    return;
                }                                
                if (!DKStepManager.AreYouReady(ref strErrMsg)) {     
                    SetStatus((int)STATUS.READY);                   
                    ShowMessage("WARNING", strErrMsg);
                    OneShotPing();
                    InitialStartButtonSet(true);
                    return;
                }

            }
            DKPlayChecker.Item_RUN = bRunning;
            if (bRunning)
            {
                if (DKMOTOSCAN != null && DKMOTOSCAN.IsConnected() == (int)CONNECTSTATE.CONNECTED)
                {
                    DKMOTOSCAN.AutoScanDisable();
                }

                CrossThreadIssue.AppendVisible(ixlblWannMsg, false);
                CrossThreadIssue.AppendTextOn(ixlblWannMsg, String.Empty, false);
                dRunningValue = 0;               
                ConnectorCountSave();
                ResultReset();
                DKStepManager.Item_bTestStarted = true;
                string strTestJobName = String.Empty;
                this.Invoke(new MethodInvoker(delegate() { strTestJobName = cbJobFiles.SelectedItem.ToString(); }));
                if (!bByRetry)
                {
                    DeviceLogging(listboxLog, true, DateTime.Now.ToString("[HH:mm:ss.ff]") + "[" + STEPMANAGER_VALUE.strProgramVersion + "][DATE:" + DateTime.Now.ToString("yyyy-MM-dd") + "][JOB:" + strTestJobName + "]", false, 0);
                    DeviceLogging(listboxBinLog, true, "### BIN LOGGING START (DATE : " + DateTime.Now.ToString("yyyy-MM-dd") + ")(" + lblVersion.Text + ")###", true, 0);
                    DeviceLogging(listboxEtcLog, true, "# # # # # # #  ETC LOGGING START  # # # # # # #", true, 0);
                }                
                
                SetStatus((int)STATUS.RUNNING);
                //MainFormSleepFuction(1);
                BeforeTestOpition();
                strStartTime = DateTime.Now.ToString("HH:mm:ss");                
                DKPlayChecker.TimerStart();
                string strTempJobName = String.Empty;
                this.Invoke(new MethodInvoker(delegate() {
                    strTempJobName = cbJobFiles.SelectedItem.ToString();
                    ChartCurrent.EnableAllTracking();
                    ChartCurrent.ClearAllData();
                    ChartCurrent.get_XAxis(0).Min = 0;
                    ChartCurrent.Visible = true;
                }));
                GC.Collect();
                OnBreaker();
                //CSMES
                //PKS 추가(23.05.27)
                if (STEPMANAGER_VALUE.bUseOSIMES)
                {
                    DKStepManager.OSIMes_SetStartTimeNCount(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                            int.Parse(ixlblPass.Caption),
                                                            int.Parse(ixlblFail.Caption));
                }
                try
                {
                    strTempJobName = cbJobFiles.SelectedItem.ToString();
                }
                catch { }
                DKStepManager.AutoTestStart(strTempJobName);

                
                if(DKStepManager.IsPCanDevice().Equals((int)STATUS.OK))
                {
                    System.Threading.Thread threadPcanMon = new System.Threading.Thread(PcanStatusView);
                    threadPcanMon.Start();
                }
                
                System.Threading.Thread threadBinLogger = new System.Threading.Thread(BinLogView);
                threadBinLogger.Start();
                System.Threading.Thread threadEtcLogger = new System.Threading.Thread(EtcLogView);
                threadEtcLogger.Start();
            }
            else
            {
                OffBreaker();
                strEndTime = DateTime.Now.ToString("HH:mm:ss");                                
                DKPlayChecker.TimerStop();
                DKStepManager.AutoTestStop();
                DKStepManager.InitInspectionRetry(); //전체리트라이 리셋
                this.Invoke(new MethodInvoker(delegate() { txtBoxWip1.Text = txtBoxWip2.Text = String.Empty; }));
                //MainFormSleepFuction(10);

                if (DKMOTOSCAN != null && DKMOTOSCAN.IsConnected() == (int)CONNECTSTATE.CONNECTED)
                {
                    DKMOTOSCAN.AutoScanEnable();
                }
            }
            
            ButtonSet(bRunning);
            this.Invoke(new MethodInvoker(delegate(){ txtBoxBarcode.Focus();}));
                        
        }
        
        private void CheckMananger()
        {
            try
            {
                Control[] tmpChk;
                for (int i = (int)DEFINES.SET1; i < (int)DEFINES.END; i++)
                {
                    if (!DKStepManager.ActorCheckPort(i))
                    {
                        LockSlot(i);
                        tmpChk = this.Controls.Find("cb" + i.ToString(), true);
                        if (tmpChk.Length > 0)
                        {
                            tmpChk[0].GetType().GetProperty("Enabled").SetValue(tmpChk[0], false, null);
                            tmpChk[0].GetType().GetProperty("Checked").SetValue(tmpChk[0], false, null);
                        }
                    }
                    else
                    {
                        UnLockSlot(i);
                        tmpChk = this.Controls.Find("cb" + i.ToString(), true);
                        if (tmpChk.Length > 0) tmpChk[0].GetType().GetProperty("Enabled").SetValue(tmpChk[0], true, null);
                    }
                }
                CheckZebraScanner();
            }
            catch 
            {
                return;
            }
            
        }

#endregion

#region GMES 표시관련

        protected override void WndProc(ref Message DllMessage)
        {
            base.WndProc(ref DllMessage);

            if (DllMessage.Msg == WM_GMESDLLDATA)
            {
                if ((int)DllMessage.WParam == 1)
                {
                    //WParam 1 이고 LPARAM 이 0 이면 서버와 연결이 끊어진것으로 본다.
                    if ((int)DllMessage.LParam == 0) { SetMesLabeText(false); return; }
                    //WParam 1 이고 LPARAM 이 1 이면 서버와 HEY 메시지까지 주고 받은 상태로 연결이 이루어졌다고 본다.
                    if ((int)DllMessage.LParam == 1) { SetMesLabeText(true); return; }

                }
            }

        }

        private void SetMesLabeText(bool bStatus)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                if (bStatus)
                {
                    //ixlblMesInfo.Caption = DKStepManager.GmesEqpInfo() + Environment.NewLine +
                    //                       DKStepManager.GmesProcInfo() + Environment.NewLine;
                    //lblGmesVersion.Text = "Dll Ver." + DKStepManager.GmesDllVersion();

                    //CSMES
                    if (STEPMANAGER_VALUE.bUseOSIMES)
                    {
                        ixlblMesInfo.Caption = "Dll Name. " + DKStepManager.OSIMesDllName();
                        lblGmesVersion.Text = "Dll Ver. " + DKStepManager.OSIMesDllVersion();
                    }
                    else
                    {
                        ixlblMesInfo.Caption = DKStepManager.GmesEqpInfo() + Environment.NewLine +
                                               DKStepManager.GmesProcInfo() + Environment.NewLine;
                        lblGmesVersion.Text = "Dll Ver." + DKStepManager.GmesDllVersion();
                    }

                    ixlblMesLED.BackGroundColor = Color.LimeGreen;
                    ixlblMesLED.Caption = "ON";
                    ixlblMesInfo.FontColor = Color.Yellow;

                }
                else
                {

                    ixlblMesLED.BackGroundColor = Color.Crimson;
                    ixlblMesLED.Caption = "OFF";
                    ixlblMesInfo.FontColor = Color.Crimson;

                }
            }));

        }
#endregion

#region UI 컨트롤

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DKPlayChecker.Item_RUN || e.RowIndex < 0 || (e.ColumnIndex != 2 && e.ColumnIndex != 5))
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
            if (dataGridView1.CurrentCell.Value == null || String.IsNullOrEmpty(dataGridView1.CurrentCell.Value.ToString()))
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
            if (dataGridView1.Rows[e.RowIndex].Tag == null) return; //Prevent 2015.09.17 DK.SIM 
            int itmpRow = dataGridView1.CurrentCell.RowIndex;
            int iRow = int.Parse(dataGridView1.Rows[e.RowIndex].Tag.ToString());
            int iCol = e.ColumnIndex - 1;
            if (iCol < 1 || iRow < 0)
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
            if (dataGridView1.Rows[itmpRow].Cells[1].Value == null)
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
         
            ShowCellData(iCol, iRow, itmpRow);
            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void btnInteractive_Click(object sender, EventArgs e)
        {    

            if (DKPlayChecker.Item_RUN) return;
            string tmpString = String.Empty;
            CheckSlot();
            dataGridView1.MultiSelect = true;
            DKStepManager.Item_bInteractiveMode = true;

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1.Rows[i].Cells[2].Value = String.Empty;
                dataGridView1.Rows[i].Cells[5].Value = String.Empty;
                dataGridView1.Rows[i].Cells[6].Value = String.Empty;
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Ivory;
                }
            }
            ButtonSet(DKStepManager.Item_bInteractiveMode);

            DKStepManager.CommonInitializeFunc();

            if (DKStepManager.IsPCanDevice().Equals((int)STATUS.OK))
            {
                System.Threading.Thread threadPcanMon = new System.Threading.Thread(PcanStatusView);
                threadPcanMon.Start();
            }

            System.Threading.Thread threadBinLogger = new System.Threading.Thread(BinLogView);
            threadBinLogger.Start();
            System.Threading.Thread threadEtcLogger = new System.Threading.Thread(EtcLogView);
            threadEtcLogger.Start();
        }

        private void ixlblWannMsg_MouseClick(object sender, MouseEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() {
                ixlblWannMsg.Visible = false;
                txtBoxBarcode.Focus(); }));
        }

        private void axiBuildVer_OnClick(object sender, EventArgs e)
        {
            /*
            //OOB 라벨 테스트
            //string strOOBLabel = "[)>06Y7520400000000XP8407413012V555343750T4116008000000002S512VIEY19293214Z11760508216Z01461800000042517Z8901170227104422623018Z3101701044226232DD010811PTWG10ANEB";             
            string strOOBLabel = "[)>06Y7520400000000XP8407413012V555343750T4116009000000307S601VIWP03304114Z11760541916Z01461800000432817Z8901170227105121092918Z3101701051210922D0109161PTWG10ANEB";
            string strReason = "";
            int iHeader = 5;
            string[] strLabelData = new string[(int)OOBBARCODE.END];
            int[] iOOBStx = new int[(int)OOBBARCODE.END];
            int[] iOOBLen = new int[(int)OOBBARCODE.END];
            int[] strSub = new int[(int)OOBBARCODE.END];

            strSub[(int)OOBBARCODE.PARTNUMBER] = 1;
            strSub[(int)OOBBARCODE.VPPS] = 1;
            strSub[(int)OOBBARCODE.DUNS] = 3;
            strSub[(int)OOBBARCODE.TRACE] = 1;
            strSub[(int)OOBBARCODE.SN] = 1;
            strSub[(int)OOBBARCODE.STID] = 3;
            strSub[(int)OOBBARCODE.IMEI] = 3;
            strSub[(int)OOBBARCODE.ICCID] = 3;
            strSub[(int)OOBBARCODE.IMSI] = 3;
            strSub[(int)OOBBARCODE.DATE] = 0;


            iOOBLen[(int)OOBBARCODE.VPPS] = 15;
            iOOBLen[(int)OOBBARCODE.PARTNUMBER] = 9;
            iOOBLen[(int)OOBBARCODE.DUNS] = 12;
            iOOBLen[(int)OOBBARCODE.TRACE] = 17;
            iOOBLen[(int)OOBBARCODE.SN] = 14;
            iOOBLen[(int)OOBBARCODE.STID] = 12;
            iOOBLen[(int)OOBBARCODE.IMEI] = 18;
            iOOBLen[(int)OOBBARCODE.ICCID] = 23;
            iOOBLen[(int)OOBBARCODE.IMSI] = 18;

            int iAllSize = 0;

            for (int i = 0; i < (int)OOBBARCODE.END; i++)
            {
                iAllSize += iOOBLen[i];
            }

            if (strOOBLabel.Length < iAllSize)
            {
                strReason = "LOADING ERROR(NO LABEL)";
                return;
            }

            iOOBStx[(int)OOBBARCODE.VPPS] = iHeader;
            iOOBStx[(int)OOBBARCODE.PARTNUMBER] = iOOBStx[(int)OOBBARCODE.VPPS] + iOOBLen[(int)OOBBARCODE.VPPS];
            iOOBStx[(int)OOBBARCODE.DUNS] = iOOBStx[(int)OOBBARCODE.PARTNUMBER] + iOOBLen[(int)OOBBARCODE.PARTNUMBER];
            iOOBStx[(int)OOBBARCODE.TRACE] = iOOBStx[(int)OOBBARCODE.DUNS] + iOOBLen[(int)OOBBARCODE.DUNS];
            iOOBStx[(int)OOBBARCODE.SN] = iOOBStx[(int)OOBBARCODE.TRACE] + iOOBLen[(int)OOBBARCODE.TRACE];
            iOOBStx[(int)OOBBARCODE.STID] = iOOBStx[(int)OOBBARCODE.SN] + iOOBLen[(int)OOBBARCODE.SN];
            iOOBStx[(int)OOBBARCODE.IMEI] = iOOBStx[(int)OOBBARCODE.STID] + iOOBLen[(int)OOBBARCODE.STID];
            iOOBStx[(int)OOBBARCODE.ICCID] = iOOBStx[(int)OOBBARCODE.IMEI] + iOOBLen[(int)OOBBARCODE.IMEI];
            iOOBStx[(int)OOBBARCODE.IMSI] = iOOBStx[(int)OOBBARCODE.ICCID] + iOOBLen[(int)OOBBARCODE.ICCID];


            //순서 NONE, PARTNUMBER, VPPS, DUNS, TRACE, SN, STID, IMEI, ICCID, IMSI, DATE, END
            strLabelData[(int)OOBBARCODE.PARTNUMBER] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.PARTNUMBER], iOOBLen[(int)OOBBARCODE.PARTNUMBER]);
            strLabelData[(int)OOBBARCODE.VPPS] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.VPPS], iOOBLen[(int)OOBBARCODE.VPPS]);
            strLabelData[(int)OOBBARCODE.DUNS] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.DUNS], iOOBLen[(int)OOBBARCODE.DUNS]);
            strLabelData[(int)OOBBARCODE.TRACE] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.TRACE], iOOBLen[(int)OOBBARCODE.TRACE]);
            strLabelData[(int)OOBBARCODE.SN] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.SN], iOOBLen[(int)OOBBARCODE.SN]);
            strLabelData[(int)OOBBARCODE.STID] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.STID], iOOBLen[(int)OOBBARCODE.STID]);
            strLabelData[(int)OOBBARCODE.IMEI] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.IMEI], iOOBLen[(int)OOBBARCODE.IMEI]);
            strLabelData[(int)OOBBARCODE.ICCID] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.ICCID], iOOBLen[(int)OOBBARCODE.ICCID]);
            strLabelData[(int)OOBBARCODE.IMSI] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.IMSI], iOOBLen[(int)OOBBARCODE.IMSI]);
            strLabelData[(int)OOBBARCODE.DATE] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.DATE], iOOBLen[(int)OOBBARCODE.DATE]);

            strLabelData[(int)OOBBARCODE.PARTNUMBER] = strLabelData[(int)OOBBARCODE.PARTNUMBER].Substring(strSub[(int)OOBBARCODE.PARTNUMBER]);
            strLabelData[(int)OOBBARCODE.VPPS] = strLabelData[(int)OOBBARCODE.VPPS].Substring(strSub[(int)OOBBARCODE.VPPS]);
            strLabelData[(int)OOBBARCODE.DUNS] = strLabelData[(int)OOBBARCODE.DUNS].Substring(strSub[(int)OOBBARCODE.DUNS]);
            strLabelData[(int)OOBBARCODE.TRACE] = strLabelData[(int)OOBBARCODE.TRACE].Substring(strSub[(int)OOBBARCODE.TRACE]);
            strLabelData[(int)OOBBARCODE.SN] = strLabelData[(int)OOBBARCODE.SN].Substring(strSub[(int)OOBBARCODE.SN]);
            strLabelData[(int)OOBBARCODE.STID] = strLabelData[(int)OOBBARCODE.STID].Substring(strSub[(int)OOBBARCODE.STID]);
            strLabelData[(int)OOBBARCODE.IMEI] = strLabelData[(int)OOBBARCODE.IMEI].Substring(strSub[(int)OOBBARCODE.IMEI]);
            strLabelData[(int)OOBBARCODE.ICCID] = strLabelData[(int)OOBBARCODE.ICCID].Substring(strSub[(int)OOBBARCODE.ICCID]);
            strLabelData[(int)OOBBARCODE.IMSI] = strLabelData[(int)OOBBARCODE.IMSI].Substring(strSub[(int)OOBBARCODE.IMSI]);
            strLabelData[(int)OOBBARCODE.DATE] = strLabelData[(int)OOBBARCODE.DATE].Substring(strSub[(int)OOBBARCODE.DATE]);


            if (strLabelData.Length > 10) //최소 10개항목이므로.
            {


                strReason = "LOADING OK(LENGTH:" + strLabelData.Length.ToString() + ")";
                return;
            }


            else
            {
                strReason = "LOADING NG(LENGTH:" + strLabelData.Length.ToString() + ")";
                return;
            }

           */

        }

        void SectionInterative()
        {
            this.Invoke(new MethodInvoker(delegate() { 
                dataGridView1.Enabled = false;
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    dataGridView1.Rows[i].Cells[2].Value = String.Empty;
                    dataGridView1.Rows[i].Cells[5].Value = String.Empty;
                    dataGridView1.Rows[i].Cells[6].Value = String.Empty;
                    dataGridView1.Rows[i].Cells[2].Style.BackColor = Color.White;
                }
            })); 

            DataGridViewSelectedCellCollection dgvscc = dataGridView1.SelectedCells;
            JOBDATA tmpJobdata;
            int iExcuPoint = 0;
            int iRow = 0;
            
            if (dgvscc.Count > 0)
            {
                List<int> SeqList = new List<int>();
                for (int i = 0; i < dgvscc.Count; i++)
                {
                    SeqList.Add(dgvscc[i].RowIndex);
                }
                SeqList.Sort();

                for (int i = 0; i < SeqList.Count; i++)
                {

                    if (SeqList[i] < 0) break;
                    if (dataGridView1.Rows[SeqList[i]].Tag == null) break;

                    iRow = int.Parse(dataGridView1.Rows[SeqList[i]].Tag.ToString());                    
                    tmpJobdata = new JOBDATA();
                    DKStepManager.GetLineCommand(iRow, ref tmpJobdata);

                    while (DKStepManager.Item_bInteractiveMode)
                    {
                        if (i == 0) break;
                        if (dataGridView1.Rows[iExcuPoint].Cells[2].Value != null &&
                        dataGridView1.Rows[iExcuPoint].Cells[2].Value.ToString().Length > 0)
                            break;
                        MainFormSleepFuction(10);
                    }

                    if (DKStepManager.Item_bInteractiveMode)
                    {
                        DKStepManager.CommandLine((int)DEFINES.SET1, iRow);
                    }
                    try
                    {
                        iExcuPoint = (int.Parse(dataGridView1.Rows[SeqList[i]].Cells[0].Value.ToString())) - 1;
                    }
                    catch
                    {
                        break;
                    }
                }

            }
            

            this.Invoke(new MethodInvoker(delegate()
            {
                dataGridView1.ClearSelection();
                dataGridView1.Enabled = true;
                dataGridView1.Focus();
            })); 
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!STEPMANAGER_VALUE.bInteractiveMode)
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
            if (Control.ModifierKeys == Keys.Control) return;

            if (e.RowIndex < 0) return;
            if (dataGridView1.Rows[e.RowIndex].Tag == null) return; //Prevent 2015.09.17 DK.SIM
            if (!DKPlayChecker.Item_RUN && DKStepManager.Item_bInteractiveMode && e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                if (dataGridView1.CurrentCell.Value != null && dataGridView1.CurrentCell.Value.ToString().Length > 1)
                {
                    LaunchInterThread();
                    return;
                }

            }

            if (DKPlayChecker.Item_RUN || e.RowIndex < 0 || e.ColumnIndex != 1)
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
            if (dataGridView1.CurrentCell.Value == null || String.IsNullOrEmpty(dataGridView1.CurrentCell.Value.ToString()))
            {
                this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
                return;
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey) return;
            if (!STEPMANAGER_VALUE.bInteractiveMode) return;
            if (dataGridView1.CurrentCell.ColumnIndex != 1) return;
            LaunchInterThread();
        }

        private void LaunchInterThread()
        {
            if (InterThread == null)
            {
                InterThread = new System.Threading.Thread(SectionInterative);
            }
            else
            {
                if (InterThread.ThreadState.Equals(System.Threading.ThreadState.Running)) return;
                else
                {
                    InterThread = new System.Threading.Thread(SectionInterative);
                }
            }
            InterThread.Start();                    
        }

        private void DestroyThread(System.Threading.Thread targetThread)
        {
            try
            {
                if (targetThread != null)
                {    
                    if (targetThread.IsAlive)
                    {   //구간 인터렉티브 스레드가 그래도 살아있으면 강제 종료;;;   
                        targetThread.Abort();
                    }
                }
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
           
        }

        private void axiSensor4_OnMouseUp(object sender, AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEvent e)
        {
            AddnChkSecCount(1);
        }

        private void axiSensor9_OnMouseUp(object sender, AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEvent e)
        {
            AddnChkSecCount(2);
        }

        private void AddnChkSecCount(int iDx)
        {
            if (DKStepManager.Item_bTestStarted) return;

            switch (iDx)
            {
                case 1: STEPMANAGER_VALUE.AddSecount1(); break;
                case 2: STEPMANAGER_VALUE.AddSecount2(); break;
            }
            if (STEPMANAGER_VALUE.CheckSecCount())
            {
                ShowServerPannel();
            }
        }

        private void RecvTransfferGateWay(int iPort, long dVal, string strMessage)
        {
            this.Invoke(new MethodInvoker(delegate()
            {                
                axiBuffCounter4.Caption  = strMessage + "Bytes.  (" + dVal.ToString() + " %)";
            }));
        }

        private bool CheckShowForm(ref string strStatus)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name.Equals("FrmPassWord"))
                {
                    strStatus = "PASSWORD";  return true;
                }

                if (frm.Name.Equals("FrmConfig"))
                {
                    strStatus = "CONFIG"; return true;
                }

                if (frm.Name.Equals("FrmEdit"))
                {
                    strStatus = "JOB EDIT"; return true;
                } 
            }
            return false;
        }

        private void RecvSckMsgGateWay(int iPort, int iMsgCode, string strMsg)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    switch(iMsgCode)
                    {
                        case (int)SOCKETCODE.MSG: break; 

                        case (int)SOCKETCODE.S01: break;

                        case (int)SOCKETCODE.S02: break;

                        case (int)SOCKETCODE.S04:   //JOB 파일 변경                          
                                                    if (DKStepManager.Item_bTestStarted) return;
                                                    if (strMsg.Length < 3) return;
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            int i = cbJobFiles.FindString(strMsg);
                                                            if( i < 0 ) return;

                                                            if (!strMsg.Equals(cbJobFiles.SelectedItem.ToString()))
                                                            {
                                                                cbJobFiles.SelectedIndex = i;
                                                                DKStepManager.SaveINI(constOption, "LASTFILE", cbJobFiles.SelectedItem.ToString());
                                                                bool bTemp = ProgramListSetUp();
                                                            }
                                                        }
                                                        catch { }
                                                    }));
                                                    break;

                        case (int)SOCKETCODE.S03:   // 프로그램 재부팅                          
                                                    if (DKStepManager.Item_bTestStarted)
                                                    {
                                                        //동작중이라면 종료하지 말자.
                                                        return;
                                                    }

                                                    string strFrmState = String.Empty;
                                                    if (CheckShowForm(ref strFrmState)) return;  //패스워드나 에디트, 환경설정창 떠있으면 종료하지 말자.

                                                    try
                                                    {
                                						DKMOTOSCAN.DisConnect();
	                                                    DKStepManager.ActorStop();
	                                                    if (threadMonitorTimer != null)
	                                                    {
	                                                        threadMonitorTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
	                                                        threadMonitorTimer.Dispose();
	                                                    }
                                                    }
                                                    catch { }
                        
                                                    bForceExit = true; //강제종료                                                    
                                                    Application.Exit();
                                                    System.Threading.Thread restartThread = new System.Threading.Thread(ProgramRestart);
                                                    restartThread.Start();
                                                    return;                            
                            
                        case (int)SOCKETCODE.S05:       
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                    try
                                                    {
                                                        if (cbJobFiles.Items.Count == 0)
                                                        {
                                                            DKSckClt.SetJobFileList("");
                                                            return;
                                                        }
                                                        string strParm = String.Empty;
                                                        for(int i =0; i < cbJobFiles.Items.Count; i++)
                                                        {
                                                            if (i < cbJobFiles.Items.Count-1)
                                                            {
                                                                strParm += cbJobFiles.Items[i].ToString() + ",";
                                                            }else
                                                            {
                                                                strParm += cbJobFiles.Items[i].ToString();
                                                            }
                                    
                                                        }
                                                        //threadMonitorTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                                                        threadMonitorTimer.Change(5000, 1000);
                                                        DKSckClt.SetJobFileList(strParm);
                                                    }
                                                    catch { }

                                                    }));
                                                    break;

                        case (int)SOCKETCODE.S06:
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            List<string> tmpList = new List<string>();
                                                            if (DKStepManager.GetFolderList(@"LOG\SET", ref tmpList))
                                                            {
                                                                threadMonitorTimer.Change(5000, 1000);
                                                                DKSckClt.SetLogFolderFileList(tmpList);
                                                            }
                                                        }
                                                        catch { }
                                                    }));
                                                    break;

                        case (int)SOCKETCODE.S07:
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            List<string> tmpList = new List<string>();
                                                            if (DKStepManager.GetFolderList("RESULT", ref tmpList))
                                                            {
                                                                threadMonitorTimer.Change(5000, 1000);
                                                                DKSckClt.SetResultFolderFileList(tmpList);
                                                            }
                                                        }
                                                        catch { }
                                                    }));
                                                    break;

                        case (int)SOCKETCODE.S08:
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            List<string> tmpList = new List<string>();
                                                            if (DKStepManager.GetFolderList("SCREEN", ref tmpList))
                                                            {
                                                                threadMonitorTimer.Change(5000, 1000);
                                                                DKSckClt.SetScreenFolderFileList(tmpList);
                                                            }
                                                        }
                                                        catch { }
                                                    }));
                                                    break;

                        case (int)SOCKETCODE.S09:
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            threadMonitorTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                                                        
                                                        }
                                                        catch { }
                                                        
                                                    }));
                                                
                                                    break;

                        case (int)SOCKETCODE.S10:
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            threadMonitorTimer.Change(500, 1000);

                                                        }
                                                        catch { }

                                                    }));

                                                    break;

                        case (int)SOCKETCODE.S11:   //JOB 파일 삭제                          
                                                    if (DKStepManager.Item_bTestStarted) return;
                                                    if (strMsg.Length < 3) return;
                                                    this.Invoke(new MethodInvoker(delegate()
                                                    {
                                                        try
                                                        {
                                                            int i = cbJobFiles.FindString(strMsg);
                                                            if (i < 0) return;

                                                            if (!strMsg.Equals(cbJobFiles.SelectedItem.ToString()))
                                                            {
                                                                //선택된 파일이 아니면 삭제.
                                                                DKStepManager.DeleteJobFile(strMsg);
                                                                cbJobFiles.Items.RemoveAt(i);                                                                
                                                            }
                                                        }
                                                        catch { }

                                                    }));
                                                    break;
                        default: break;
                    }

                }));
            }
            catch { }
            
        }

        private void ProgramRestart()
        {
            //update 파일 변경 작업때문에 3.0정초 지연시킨다.
            MainFormSleepFuction(3000);            
            Application.Restart();
        }

        private void listboxBinLog_DrawItem(object sender, DrawItemEventArgs e)
        {     

        }
        
        private void listboxBinLog_MouseClick(object sender, MouseEventArgs e)
        {
            LogBoxScrollSizeReset(listboxBinLog);

            if (STEPMANAGER_VALUE.bInteractiveMode || STEPMANAGER_VALUE.bProgramRun) return;

            try
            {
                StringBuilder strBinLogging = new StringBuilder(4096);
                for (int i = 0; i < listboxBinLog.Items.Count; i++)
                {
                    strBinLogging.Append(listboxBinLog.Items[i].ToString() + Environment.NewLine);
                }
                Clipboard.SetText(strBinLogging.ToString());
            }
            catch { }

            this.Invoke(new MethodInvoker(delegate() { txtBoxBarcode.Focus(); }));
        }

        private void FrmFaMain_Load(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DKStepManager.SaveINI(constOption, constPrimary, "GMES");

            chkBoxMesOn.Checked = false;
            chkBoxOracleOn.Checked = false;
            panelGMES.Visible = true;
            panelORACLE.Visible = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DKStepManager.SaveINI(constOption, constPrimary, "ORACLE");

            chkBoxMesOn.Checked = false;
            chkBoxOracleOn.Checked = false;
            panelGMES.Visible = false;
            panelORACLE.Visible = true;                      
        }

        private void chkBoxOracleOn_CheckedChanged(object sender, EventArgs e)
        {
            ControlSave(chkBoxOracleOn, "ORACLE", "MESON");
            STEPMANAGER_VALUE.bUseOracleOn = chkBoxOracleOn.Checked;
            this.Invoke(new MethodInvoker(delegate()
            {
                ShowOracleInfo(chkBoxOracleOn.Checked, true);
                txtBoxBarcode.Focus();
            })); 
        }

        private void CheckNetworkFunction()
        {            
            bool bPing = false;
            int i = 0;
            while (!bPing)
            {
                bPing = OneShotPing();
                i++;
                if (i == 2)
                {
                    NetworkCheckMsgPannel(true);
                }
            }
        }

        private bool OneShotPing()
        {
            ORACLEINFO tmpInfo = new ORACLEINFO();
            tmpInfo = DKStepManager.GetOracleInfo();
            string strResult = String.Empty;
            StringBuilder sbTime = new StringBuilder(4096);

            sbTime.Clear();
            System.Threading.Thread.Sleep(200);
            System.Threading.Thread.Sleep(200);
            Application.DoEvents();
            System.Threading.Thread.Sleep(200);
            System.Threading.Thread.Sleep(200);
            System.Threading.Thread.Sleep(200);

            bool bPing = DKStepManager.GetNetworkPingTest(tmpInfo.strServerIP, ref strResult);

            sbTime.Append("[");
            sbTime.Append(DateTime.Now.ToString("HH:mm:ss.ff"));
            sbTime.Append("]");
            sbTime.Append(strResult);
            DeviceLogging(listboxLog, false, sbTime.ToString(), false, 0);
            return bPing;
        }

        private void CheckOracleConnection()
        {
            string strReason = String.Empty;
            bool bConnTest = DKStepManager.OracleConnection(true, ref strReason);
            if (!bConnTest)
            {
                CrossThreadIssue.ChangeTextControl(ixlblOracleLED, "OFF");
                CrossThreadIssue.ChangeBackColor(ixlblOracleLED, Color.Crimson);                
                DeviceLogging(listboxLog, false, "## MES CONNECTION TEST FAIL.(" + strReason + ")", false, 0);
            }
            else
            {
                CrossThreadIssue.ChangeTextControl(ixlblOracleLED, "ON");
                CrossThreadIssue.ChangeBackColor(ixlblOracleLED, Color.LimeGreen);
                CrossThreadIssue.ChangeFontColor(ixlblOracleInfo, Color.Yellow);
                DeviceLogging(listboxLog, false, "## MES CONNECTION TEST SUCCESS.", false, 0);
            }
            NetworkCheckMsgPannel(false);
            DestroyThread(thdPing);
            NetworkCheckMsgPannel(false);
        }

        private void tabControl_Resize(object sender, EventArgs e)
        {
            listboxLog.Left = listboxBinLog.Left = listboxEtcLog.Left = axiSensorDebug.Left = listboxResultLog.Left = 1;
            listboxLog.Top = listboxBinLog.Top = axiSensorDebug.Top = listboxResultLog.Top = axiSensorRepeat.Top = 1;

            axiSensorRepeat.Left = axiSensorDebug.Right + 1;

            listboxEtcLog.Top = axiSensorDebug.Bottom + 1;
            listboxLog.Width = listboxBinLog.Width = listboxEtcLog.Width = listboxResultLog.Width = tabControl.Width - 9;
            listboxLog.Height = listboxBinLog.Height = listboxResultLog.Height = tabControl.Height - 26;
            listboxEtcLog.Height = listboxLog.Height - axiSensorDebug.Height;
        }

        private void lblVersion_DoubleClick(object sender, EventArgs e)
        {
            DKMOTOSCAN.DisConnect();
            DKStepManager.ActorStop();
            SensorColorOff();

            //Prevent 2015.03.26 DK.SIM  
            FrmPassWord tmpFrm = null;
            tmpFrm = new FrmPassWord();
            tmpFrm.ShowDialog();
            PWUSER testUser = new PWUSER();
            int iAuth = tmpFrm.IsOK(ref testUser);
            if (iAuth.Equals((int)ACCOUNT.SUPERUSER) ||iAuth.Equals((int)ACCOUNT.USER))
            {
                //여기 히스토리
                DeviceLogging(listboxLog, true, ">>> Program Version History", false, 0);

                for (int i = 0; i < STEPMANAGER_VALUE.LstVersionHistory.Count; i++)
                {
                    DeviceLogging(listboxLog, false, STEPMANAGER_VALUE.LstVersionHistory[i].ToString(), false, 0);
                }

                DeviceLogging(listboxLog, false, "--------------------------------------------------------------------------------------------------------", false, 0);
                DeviceLogging(listboxLog, false, ">>> CONFIG PATH : " + DKStepManager.GetConfigPath(), false, 0);                
                DeviceLogging(listboxLog, false, "--------------------------------------------------------------------------------------------------------", false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION DIO:"          + DKStepManager.LoadINI("COMPORT", "DIO"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION SET:"          + DKStepManager.LoadINI("COMPORT", "SET"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION 5515C:"        + DKStepManager.LoadINI("COMPORT", "5515C"), false, 0);                
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION RS232SCANNER:" + DKStepManager.LoadINI("COMPORT", "RS232SCANNER"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION TC3000:"       + DKStepManager.LoadINI("COMPORT", "TC3000"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION MTP200:"       + DKStepManager.LoadINI("COMPORT", "MTP200"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION NAD:"          + DKStepManager.LoadINI("COMPORT", "NAD"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION ZebraScanner:" + DKStepManager.LoadINI("COMPORT", "ZebraScanner"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION AUDIOSELECTOR:"+ DKStepManager.LoadINI("COMPORT", "AUDIOSELECTOR"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION ADC-MODULE:"   + DKStepManager.LoadINI("COMPORT", "ADC-MODULE"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION 34410A:"       + DKStepManager.LoadINI("COMPORT", "34410A"), false, 0);
                DeviceLogging(listboxLog, false, ">>> DEVICE INFORMATION KEITHLEY:"     + DKStepManager.LoadINI("COMPORT", "KEITHLEY"), false, 0);                

                DeviceLogging(listboxLog, false, "--------------------------------------------------------------------------------------------------------", false, 0);



                ComboListUpdate();
                bool bTmp = ProgramListSetUp();
            }

            tmpFrm.Dispose();
            string strReason = String.Empty;
            bool bMan = DKStepManager.CheckDevice(ref strReason);
            //MainFormSleepFuction(10);            
            CheckMananger();
            ConnectorCountLoad();
            CountLoad();
            if (!bMan) ShowMessage("CHECK", strReason); 
            if (bMan) DKStepManager.IfPlcModeIsReadySignal(true); //PLC모드인경우 최초 READY 릴레이 켜야함.
        }

        private void axiSensorDebug_OnClick(object sender, EventArgs e)
        {
            if (STEPMANAGER_VALUE.bDebugLogEnable)
            {                
                STEPMANAGER_VALUE.bDebugLogEnable = false;
            }
            else
            {             
                STEPMANAGER_VALUE.bDebugLogEnable = true;
            }
      
        }

        private void axiLabelCurrent_OnDblClick(object sender, EventArgs e)
        {
            if (!axiCurrent.Bottom.Equals(dataGridConnector.Bottom))
                ShowCountControls();
            else
                HideCountControls();
            
        }

        private void axiCurrent_OnDblClick(object sender, EventArgs e)
        {
            ChartCurrent.Visible = !ChartCurrent.Visible;
        }

        private void ChartCurrent_OnDblClickDataView(object sender, AxiPlotLibrary.IiPlotXEvents_OnDblClickDataViewEvent e)
        {
            if (ChartCurrent.Size == ixlblStatus.Size)
            {
                ChartCurrent.get_ToolBar(0).DoButtonClickResume();
                ChartCurrent.get_ToolBar(0).Visible = false;
                ChartCurrent.TitleVisible = false;
                ChartCurrent.Location = dataGridConnector.Location;
                ChartCurrent.Size = dataGridConnector.Size;
            }
            else
            {
                ChartCurrent.get_ToolBar(0).Visible = true;
                ChartCurrent.TitleVisible = true;
                ChartCurrent.TitleText = DateTime.Now.ToString("yyyy-MM-dd");
                ChartCurrent.Location = ixlblStatus.Location;
                ChartCurrent.Size = ixlblStatus.Size;
            } 
        }

        private void chkBoxMesOn_MouseDown(object sender, MouseEventArgs e)
        {
            string tmpAskMesPassword = DKStepManager.LoadINI("GMES", "MESASKPASSWORD");

            if (!tmpAskMesPassword.Equals("ON"))
            {
                return;
            }

            bool bCurrentStatus = chkBoxMesOn.Checked;

            FrmPassWord tmpFrm = null;
            tmpFrm = new FrmPassWord();
            tmpFrm.ShowDialog();
            PWUSER testUser = new PWUSER();
            int iAuth = tmpFrm.IsOK(ref testUser);
            if (iAuth.Equals((int)ACCOUNT.SUPERUSER)
                    || (iAuth.Equals((int)ACCOUNT.USER) && testUser.bMes))
            {
                chkBoxMesOn.Checked = !bCurrentStatus;
            }
            else
            {
                chkBoxMesOn.Checked = bCurrentStatus;
            }
            tmpFrm.Dispose();
        }
        
        private void axiSensorRepeat_OnClick(object sender, EventArgs e)
        {
            bRepeatMode = !bRepeatMode;
            CrossThreadIssue.ChangeFontColor(axiSensorRepeat, bRepeatMode);
        }

#endregion

        //PKS
        private void SetJumpResult(int iJobNum, STATUS StsResult)
        {
            iLastResult[iJobNum] = (int)StsResult;
        }

        //PKS
        private void GotoTestGridRow(int iJobNum)
        {
            if (dataGridView1.Rows[iJobNum].Cells[1].Value != null)
            {
                dataGridView1.Rows[iJobNum].Cells[2].Value = "TEST";
                //targetView.Rows[iRow + 1].Cells[2].Selected = true;
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    dataGridView1.Rows[iJobNum].Cells[j].Style.BackColor = Color.Khaki;
                }
            }
        }
    }

#region 기타 UI 관련 클래스

    // 크로스 스레드 문제 처리
    public static class CrossThreadIssue
    {
        public static void ChangeBackColor(AxisAnalogLibrary.AxiLabelX Ctrl, bool bState)
        {
            Color tmpColor = Color.GreenYellow;
            if (!bState) tmpColor = Color.FromArgb(48, 48, 48);

            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.BackGroundColor = tmpColor; }));
            }
            else
            {
                Ctrl.BackGroundColor = tmpColor;
            }
        }

        public static void ChangeBackColor(AxisAnalogLibrary.AxiLabelX Ctrl, Color tmpColor)
        {            
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.BackGroundColor = tmpColor; }));
            }
            else
            {
                Ctrl.BackGroundColor = tmpColor;
            }
        }

        public static void ChangeFontColor(AxisAnalogLibrary.AxiLabelX Ctrl, Color tmpColor)
        {
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.FontColor = tmpColor; }));
            }
            else
            {
                Ctrl.FontColor = tmpColor;
            }
        }

        public static void ChangeFontColor(AxisAnalogLibrary.AxiLabelX Ctrl, bool bState)
        {
            Color tmpColor = Color.Cyan;
            if (!bState) tmpColor = Color.Gray;

            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.FontColor = tmpColor; }));
            }
            else
            {
                Ctrl.FontColor = tmpColor;
            }
        }

        public static void ChangeTextControl(AxisAnalogLibrary.AxiLabelX Ctrl, string strMsg)
        {
            
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.Caption = strMsg; }));
            }
            else
            {
                Ctrl.Caption = strMsg;
            }
        }

        public static void ChangeOutRelayColor(AxisAnalogLibrary.AxiLabelX Ctrl, bool bState)
        {
            Color tmpColor = Color.Yellow;
            if (!bState) tmpColor = Color.Gray;

            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.BackGroundColor = tmpColor; }));
            }
            else
            {
                Ctrl.BackGroundColor = tmpColor;
            }
        }

        public static void AppendEnabled(Control Ctrl, bool bState)
        {
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.Enabled = bState; }));
            }
            else
            {
                Ctrl.Enabled = bState;
            }
        }
        public static void ChangeChecked(CheckBox Ctrl, bool bState)
        {
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate () { Ctrl.Checked = bState; }));
            }
            else
            {
                Ctrl.Checked = bState;
            }
        }

        public static void AppendVisible(Control Ctrl, bool bState)
        {
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.Visible = bState; }));
            }
            else
            {
                Ctrl.Visible = bState;
            }
        }

        public static void AppendTextOn(Control Ctrl, string msg, bool bFront)
        {
            if (Ctrl.InvokeRequired)
            {
                Ctrl.Invoke(new MethodInvoker(delegate()
                    { 
                        Ctrl.Text = msg;
                        if (bFront) Ctrl.BringToFront();
                        else Ctrl.SendToBack();
                    }));       
            }
            else
            {
                if (bFront) Ctrl.BringToFront();
                else Ctrl.SendToBack();
                Ctrl.Text = msg; 
                
            }
        }

        public static void GridScrollingLock(DataGridView Ctrl, int iRow)
        {
            int iLinePos = 7;
            if (STEPMANAGER_VALUE.bMSize) iLinePos = 5;
            int iIdx = iRow - iLinePos;

            if (iIdx < 0 || Ctrl.Rows.Count < iIdx) return;

            if (Ctrl.InvokeRequired)
            {
                if (Ctrl.FirstDisplayedScrollingRowIndex == iIdx) return;
                Ctrl.BeginInvoke(new MethodInvoker(delegate() { Ctrl.FirstDisplayedScrollingRowIndex = iIdx; }));
            }
            else
            {
                if (Ctrl.FirstDisplayedScrollingRowIndex == iIdx) return;
                Ctrl.FirstDisplayedScrollingRowIndex = iIdx;
            }
        }

        public static void GridScrollingMove(DataGridView Ctrl, int iRow)
        {

            int iIdx = iRow - 3;
         
            if (Ctrl.Rows.Count <= iRow) return;
                     
            if (iIdx >= Ctrl.Rows.Count || iIdx <= 0) iIdx = 0;

            if (Ctrl.InvokeRequired)
            {
                if (Ctrl.FirstDisplayedScrollingRowIndex == iIdx) return;
                Ctrl.Invoke(new MethodInvoker(delegate() { Ctrl.FirstDisplayedScrollingRowIndex = iIdx; }));
            }
            else
            {
                if (Ctrl.FirstDisplayedScrollingRowIndex == iIdx) return;
                Ctrl.FirstDisplayedScrollingRowIndex = iIdx;
            }
        }

    }

    // 데이터 그리드 뷰 더블버퍼( 구 REDRAW ) 사용하기 위한 클래스 - 대박...짱남.
    // using System.Reflection; <--- 선언 해야함.

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

     /*   public static void SetDoubleBuffering(this ListBox control, bool value)
        {
             System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                controlProperty.SetValue(control, value, null);
        }*/
    }

    
#endregion
}
