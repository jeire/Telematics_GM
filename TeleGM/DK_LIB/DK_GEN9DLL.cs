using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Runtime.ExceptionServices;

namespace GmTelematics
{
    class DK_GEN9DLL
    {   
        //DLL를 동적으로 Load 하기 위한 Kernel dll import.
        //------------------------------------------------------------------------------------------------------
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        static extern bool FreeLibrary(int hModule);


        //GSM_DLL.dll (9.7) 의 명령어 정의 -------------------------------------------------------------------------------------------------
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_PortOpen_97(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int StopBit, int FlowControl);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_PortClose_97();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_IMEIGet_97(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_IMEISet_97(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_MakeCheckSum_97(byte[] iStrPointer);
               
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadMSISDN_97(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadICCID_97(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadIMSI_97(byte[] iStrPointer);
        //-------------------------------------------------------------------------------------------------------------------------- 

        //GEN94.dll (9.4) 의 명령어 정의 -------------------------------------------------------------------------------------------------
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_PortOpen_94(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int StopBit, int FlowControl);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_PortClose_94();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_IMEIGet_94(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_IMEISet_94(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_MakeCheckSum_94(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadMSISDN_94(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadICCID_94(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadIMSI_94(byte[] iStrPointer);
        //-------------------------------------------------------------------------------------------------------------------------- 

        // 일단 ESN은 제외
        //ESN.dll (9.X) 의 명령어 정의 -------------------------------------------------------------------------------------------------
        /*
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_PortOpen_9X(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int StopBit, int FlowControl);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_PortClose_9X();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_IMEIGet_9X(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_IMEISet_9X(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_MakeCheckSum_9X(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadMSISDN_9X(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadICCID_9X(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int DLL_nReadIMSI_9X(byte[] iStrPointer);
        //-------------------------------------------------------------------------------------------------------------------------- 
        */
       
        private string strNadDllName97;
        private string strNadDllName94;
        //private string strNadDllName9X;  얘는 일단 제외

        public event EventRealTimeMsg NadKeyDllRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
        private DK_LOGGER DKQLogger;
        private bool isOpenCommand97;
        private bool isOpenCommand94;
        // private bool isOpenCommand9X;얘는 일단 제외

        private int      iLibAddress97;
        private int      iLibAddress94;
        //private int      iLibAddress9X; 얘는 일단 제외

        private IntPtr[] iLibFunction;

        //GSM_DLL.dll(9.7) 의 명령어 정의
        private DLL_PortOpen_97         _97dllPortOpen;
        private DLL_PortClose_97        _97dllPortClose;
        private DLL_IMEIGet_97          _97dllIMEIGet;
        private DLL_IMEISet_97          _97dllIMEISet;
        private DLL_MakeCheckSum_97     _97dllMakeCheckSum;
        private DLL_nReadMSISDN_97      _97dllnReadMSISDN;
        private DLL_nReadICCID_97       _97dllnReadICCID;
        private DLL_nReadIMSI_97        _97dllnReadIMSI;

        //GEN94.dll(9.4) 의 명령어 정의
        private DLL_PortOpen_94         _94dllPortOpen;
        private DLL_PortClose_94        _94dllPortClose;
        private DLL_IMEIGet_94          _94dllIMEIGet;
        private DLL_IMEISet_94          _94dllIMEISet;
        private DLL_MakeCheckSum_94     _94dllMakeCheckSum;
        private DLL_nReadMSISDN_94      _94dllnReadMSISDN;
        private DLL_nReadICCID_94       _94dllnReadICCID;
        private DLL_nReadIMSI_94        _94dllnReadIMSI;

        //ESN.dll(9.X) 의 명령어 정의
        //일단 얘는 제외
        /*
        private DLL_PortOpen_9X         _9XdllPortOpen;
        private DLL_PortClose_9X        _9XdllPortClose;
        private DLL_IMEIGet_9X          _9XdllIMEIGet;
        private DLL_IMEISet_9X          _9XdllIMEISet;
        private DLL_MakeCheckSum_9X     _9XdllMakeCheckSum;
        private DLL_nReadMSISDN_9X      _9XdllnReadMSISDN;
        private DLL_nReadICCID_9X       _9XdllnReadICCID;
        private DLL_nReadIMSI_9X        _9XdllnReadIMSI;
        */
                    
        private object              lockobjectNadKey;

        public DK_GEN9DLL()
        {
            strNadDllName97 = "GSM_DLL.dll"; //9.7 DLL
            strNadDllName94 = "GEN94.dll";   //9.4 DLL
            //strNadDllName9X = "ESN.dll";     //9.X DLL //얘는 일단 제외

            iLibAddress97 = 0;
            iLibAddress94 = 0;
            //iLibAddress9X = 0; 일단 제외

            iLibFunction = new IntPtr[(int)GEN9DLLIDX.END];
            isOpenCommand97 = false;
            isOpenCommand94 = false;
            //isOpenCommand9X = false; 일단 제외

            lockobjectNadKey = new object();
            DKQLogger = new DK_LOGGER("SET", false);
            DKQLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_NadKeyDll);

        }

        private bool MakeCommandOfDll(ref string strReason)
        {
            strReason = "SUCCESS";
            try
            {
                iLibFunction = new IntPtr[(int)GEN9DLLIDX.END];
                iLibFunction[(int)GEN9DLLIDX.PORT_OPEN_97]      = GetProcAddress(iLibAddress97, "?PortOpen@@YAHHHHHHH@Z");
                iLibFunction[(int)GEN9DLLIDX.PORT_CLOSE_97]     = GetProcAddress(iLibAddress97, "?PortClose@@YAHXZ");
                iLibFunction[(int)GEN9DLLIDX.READ_IMEI_97]      = GetProcAddress(iLibAddress97, "?IMEIGet@@YAHPAE@Z");
                iLibFunction[(int)GEN9DLLIDX.WRITE_IMEI_97]     = GetProcAddress(iLibAddress97, "?IMEISet@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.CHECKSUM_IMEI_97]  = GetProcAddress(iLibAddress97, "?MakeCheckSum@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_MSISDN_97]    = GetProcAddress(iLibAddress97, "?nReadMSISDN@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_ICCID_97]     = GetProcAddress(iLibAddress97, "?nReadICCID@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_IMSI_97]      = GetProcAddress(iLibAddress97, "?nReadIMSI@@YAHPAD@Z");

                iLibFunction[(int)GEN9DLLIDX.PORT_OPEN_94]      = GetProcAddress(iLibAddress94, "?PortOpen@@YAHHHHHHH@Z");
                iLibFunction[(int)GEN9DLLIDX.PORT_CLOSE_94]     = GetProcAddress(iLibAddress94, "?PortClose@@YAHXZ");
                iLibFunction[(int)GEN9DLLIDX.READ_IMEI_94]      = GetProcAddress(iLibAddress94, "?IMEIGet@@YAHPAE@Z");
                iLibFunction[(int)GEN9DLLIDX.WRITE_IMEI_94]     = GetProcAddress(iLibAddress94, "?IMEISet@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.CHECKSUM_IMEI_94]  = GetProcAddress(iLibAddress94, "?MakeCheckSum@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_MSISDN_94]    = GetProcAddress(iLibAddress94, "?nReadMSISDN@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_ICCID_94]     = GetProcAddress(iLibAddress94, "?nReadICCID@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_IMSI_94]      = GetProcAddress(iLibAddress94, "?nReadIMSI@@YAHPAD@Z");

                /* 일단 제외
                iLibFunction[(int)GEN9DLLIDX.PORT_OPEN_9X]      = GetProcAddress(iLibAddress9X, "?PortOpen@@YAHHHHHHH@Z");
                iLibFunction[(int)GEN9DLLIDX.PORT_CLOSE_9X]     = GetProcAddress(iLibAddress9X, "?PortClose@@YAHXZ");
                iLibFunction[(int)GEN9DLLIDX.READ_IMEI_9X]      = GetProcAddress(iLibAddress9X, "?IMEIGet@@YAHPAE@Z");
                iLibFunction[(int)GEN9DLLIDX.WRITE_IMEI_9X]     = GetProcAddress(iLibAddress9X, "?IMEISet@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.CHECKSUM_IMEI_9X]  = GetProcAddress(iLibAddress9X, "?MakeCheckSum@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_MSISDN_9X]    = GetProcAddress(iLibAddress9X, "?nReadMSISDN@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_ICCID_9X]     = GetProcAddress(iLibAddress9X, "?nReadICCID@@YAHPAD@Z");
                iLibFunction[(int)GEN9DLLIDX.READ_IMSI_9X]      = GetProcAddress(iLibAddress9X, "?nReadIMSI@@YAHPAD@Z");
                */
                _97dllPortOpen      = (DLL_PortOpen_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.PORT_OPEN_97],            typeof(DLL_PortOpen_97));
                _97dllPortClose     = (DLL_PortClose_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.PORT_CLOSE_97],          typeof(DLL_PortClose_97));
                _97dllIMEIGet       = (DLL_IMEIGet_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_IMEI_97],             typeof(DLL_IMEIGet_97));
                _97dllIMEISet       = (DLL_IMEISet_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.WRITE_IMEI_97],            typeof(DLL_IMEISet_97));
                _97dllMakeCheckSum  = (DLL_MakeCheckSum_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.CHECKSUM_IMEI_97],    typeof(DLL_MakeCheckSum_97));
                _97dllnReadMSISDN   = (DLL_nReadMSISDN_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_MSISDN_97],       typeof(DLL_nReadMSISDN_97));
                _97dllnReadICCID    = (DLL_nReadICCID_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_ICCID_97],         typeof(DLL_nReadICCID_97));
                _97dllnReadIMSI     = (DLL_nReadIMSI_97)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_IMSI_97],           typeof(DLL_nReadIMSI_97));

                _94dllPortOpen      = (DLL_PortOpen_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.PORT_OPEN_94],            typeof(DLL_PortOpen_94));
                _94dllPortClose     = (DLL_PortClose_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.PORT_CLOSE_94],          typeof(DLL_PortClose_94));
                _94dllIMEIGet       = (DLL_IMEIGet_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_IMEI_94],             typeof(DLL_IMEIGet_94));
                _94dllIMEISet       = (DLL_IMEISet_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.WRITE_IMEI_94],            typeof(DLL_IMEISet_94));
                _94dllMakeCheckSum  = (DLL_MakeCheckSum_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.CHECKSUM_IMEI_94],    typeof(DLL_MakeCheckSum_94));
                _94dllnReadMSISDN   = (DLL_nReadMSISDN_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_MSISDN_94],       typeof(DLL_nReadMSISDN_94));
                _94dllnReadICCID    = (DLL_nReadICCID_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_ICCID_94],         typeof(DLL_nReadICCID_94));
                _94dllnReadIMSI     = (DLL_nReadIMSI_94)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_IMSI_94],           typeof(DLL_nReadIMSI_94));

                /* 일단 제외
                _9XdllPortOpen      = (DLL_PortOpen_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.PORT_OPEN_9X],            typeof(DLL_PortOpen_9X));
                _9XdllPortClose     = (DLL_PortClose_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.PORT_CLOSE_9X],          typeof(DLL_PortClose_9X));
                _9XdllIMEIGet       = (DLL_IMEIGet_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_IMEI_9X],             typeof(DLL_IMEIGet_9X));
                _9XdllIMEISet       = (DLL_IMEISet_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.WRITE_IMEI_9X],            typeof(DLL_IMEISet_9X));
                _9XdllMakeCheckSum  = (DLL_MakeCheckSum_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.CHECKSUM_IMEI_9X],    typeof(DLL_MakeCheckSum_9X));
                _9XdllnReadMSISDN   = (DLL_nReadMSISDN_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_MSISDN_9X],       typeof(DLL_nReadMSISDN_9X));
                _9XdllnReadICCID    = (DLL_nReadICCID_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_ICCID_9X],         typeof(DLL_nReadICCID_9X));
                _9XdllnReadIMSI     = (DLL_nReadIMSI_9X)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)GEN9DLLIDX.READ_IMSI_9X],           typeof(DLL_nReadIMSI_9X));
                */
                return true;
            }
            catch
            {
                strReason = "Make Error DLL";
                return false;
            }            
        }

        private bool SetDllFile()
        {
            string strErrorMsg = String.Empty;

            try
            {
                if (IsOnLibrary((int)GEN9TARGET.DLL97)) _97dllPortClose.Invoke();
                if (IsOnLibrary((int)GEN9TARGET.DLL94)) _94dllPortClose.Invoke();
                //if (IsOnLibrary((int)GEN9TARGET.DLL9X)) _9XdllPortClose.Invoke();

                isOpenCommand97 = false;
                isOpenCommand94 = false;
                //isOpenCommand9X = false;

                if (iLibAddress97 != 0)
                {
                    FreeLibrary(iLibAddress97);
                    iLibAddress97 = 0;
                }

                if (iLibAddress94 != 0)
                {
                    FreeLibrary(iLibAddress94);
                    iLibAddress94 = 0;
                }

                /*
                if (iLibAddress9X != 0)
                {
                    FreeLibrary(iLibAddress9X);
                    iLibAddress9X = 0;
                }
                */

                iLibAddress97 = LoadLibrary(strNadDllName97);
                iLibAddress94 = LoadLibrary(strNadDllName94);
                //iLibAddress9X = LoadLibrary(strNadDllName9X);
                  
                if (iLibAddress97 == 0) return false;
                if (iLibAddress94 == 0) return false;
                //if (iLibAddress9X == 0) return false;

                
                return MakeCommandOfDll(ref strErrorMsg);

            }
            catch 
            {
                strErrorMsg = "SetDllFileName Function Error";
                return false;
            }

        }
        
        public bool IsOnLibrary(int iTarget)
        {   //DLL 로드 안했으면 false;
            lock (lockobjectNadKey)
            {
                switch (iTarget)
                {
                    case (int)GEN9TARGET.DLL97:
                        if (iLibAddress97 != 0)
                        {
                            return true;
                        }
                        break;
                    case (int)GEN9TARGET.DLL94:
                        if (iLibAddress94 != 0)
                        {
                            return true;
                        }
                        break;
                    /*
                    case (int)GEN9TARGET.DLL9X:
                        if (iLibAddress9X != 0)
                        {
                            return true;
                        }
                        break;
                    */
                    default:
                        break;
                }
                return false;
            }
            
        }

        public void UnloadLibrary(int iTarget)
        {   //DLL 로드 안했으면 false;
            lock (lockobjectNadKey)
            {
                switch (iTarget)
                {
                    case (int)GEN9TARGET.DLL97:
                        if (iLibAddress97 != 0)
                        {
                            FreeLibrary(iLibAddress97);
                            iLibAddress97 = 0;
                            isOpenCommand97 = false;
                        } break;

                    case (int)GEN9TARGET.DLL94:
                        if (iLibAddress94 != 0)
                        {
                            FreeLibrary(iLibAddress94);
                            iLibAddress94 = 0;
                            isOpenCommand94 = false;
                        } break;
                    /*
                    case (int)GEN9TARGET.DLL9X:
                        if (iLibAddress9X != 0)
                        {
                            FreeLibrary(iLibAddress9X);
                            iLibAddress9X = 0;
                            isOpenCommand9X = false;
                        } break;
                    */
                    default:

                        break;
                }
                return;               
            }
        }

        public bool IsPortOpen(int iTarget)
        {
            switch (iTarget)
            {
                case (int)GEN9TARGET.DLL97:
                    return isOpenCommand97;

                case (int)GEN9TARGET.DLL94:
                    return isOpenCommand94;

                /*
                case (int)GEN9TARGET.DLL9X:
                    return isOpenCommand9X;
                */
                default:
                    return false;
            }
            
        }
        
        
#region DLL Fuctions        

        [HandleProcessCorruptedStateExceptions]
        public bool PortOpen(int iTarget, int iPortNum, int iBaudRate, int DataBit, int iParityBit, int iStopBit, int iFlowControl)
        {
            try
            {
                if (!IsOnLibrary(iTarget))
                    SetDllFile();

                if (IsOnLibrary(iTarget))
                {
                    switch(iTarget)
                    {
                        case (int)GEN9TARGET.DLL97:
                                                    isOpenCommand97 = false;
                                                    _97dllPortClose.Invoke(); break;
                        case (int)GEN9TARGET.DLL94:
                                                    isOpenCommand94 = false;
                                                    _94dllPortClose.Invoke(); break;
                        /*
                        case (int)GEN9TARGET.DLL9X:
                                                    isOpenCommand9X = false;
                                                    _9XdllPortClose.Invoke(); break;
                        */
                        default: return false;
                    }
                }

            }
            catch { }

            lock (lockobjectNadKey)
            {
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                try
                {
                    switch(iTarget)
                    {
                        case (int)GEN9TARGET.DLL97:
                                                    isOpenCommand97 = _97dllPortOpen.Invoke(iPortNum, iBaudRate, DataBit, iParityBit, iStopBit, iFlowControl).Equals(1);
                                                    break;
                        case (int)GEN9TARGET.DLL94:
                                                    isOpenCommand94 = _94dllPortOpen.Invoke(iPortNum, iBaudRate, DataBit, iParityBit, iStopBit, iFlowControl).Equals(1);
                                                    break;
                        /*
                        case (int)GEN9TARGET.DLL9X:
                                                    isOpenCommand9X = _9XdllPortOpen.Invoke(iPortNum, iBaudRate, DataBit, iParityBit, iStopBit, iFlowControl).Equals(1);
                                                    break;
                        */
                        default: return false;
                    }


                    
                }
                catch
                {
                    switch(iTarget)
                    {
                        case (int)GEN9TARGET.DLL97:
                                                    isOpenCommand97 = false;
                                                    break;
                        case (int)GEN9TARGET.DLL94:
                                                    isOpenCommand94 = false;
                                                    break;
                        /*
                        case (int)GEN9TARGET.DLL9X:
                                                    isOpenCommand9X = false;
                                                    break;
                        */
                        default: break;
                    }
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Ex Failure");
                    return false;
                }

                switch(iTarget)
                {
                    case (int)GEN9TARGET.DLL97:
                                                SaveLog("", "[RX] PortOpen:" + isOpenCommand97.ToString());
                                                break;
                    case (int)GEN9TARGET.DLL94:
                                                SaveLog("", "[RX] PortOpen:" + isOpenCommand94.ToString());
                                                break;
                    /*
                    case (int)GEN9TARGET.DLL9X:
                                                SaveLog("", "[RX] PortOpen:" + isOpenCommand9X.ToString());
                                                break;
                    */
                    default: break;
                }
                
                return IsPortOpen(iTarget);
            }

        }

        [HandleProcessCorruptedStateExceptions]
        public bool PortClose(int iTarget)
        {
            lock (lockobjectNadKey)
            {
                switch(iTarget)
                {
                    case (int)GEN9TARGET.DLL97: isOpenCommand97 = false; break;
                    case (int)GEN9TARGET.DLL94: isOpenCommand94 = false; break;
                    //case (int)GEN9TARGET.DLL9X: isOpenCommand9X = false; break;
                    default: return false;
                }

                try
                {
                    if (IsOnLibrary(iTarget))
                    {
                        SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);

                        switch (iTarget)
                        {
                            case (int)GEN9TARGET.DLL97: _97dllPortClose.Invoke(); break;
                            case (int)GEN9TARGET.DLL94: _94dllPortClose.Invoke(); break;
                            //case (int)GEN9TARGET.DLL9X: _9XdllPortClose.Invoke(); break;
                            default: break;
                        }
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":true");
                    }

                }
                catch
                {
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":CloseEx");
                }

                return true;
            }

        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_IMEI(int iTarget, ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[15];
                try
                {
                    int iRes = 0;

                    switch (iTarget)
                    {
                        case (int)GEN9TARGET.DLL97: iRes = _97dllIMEIGet.Invoke(ByteArray); break;
                        case (int)GEN9TARGET.DLL94: iRes = _94dllIMEIGet.Invoke(ByteArray); break;
                        //case (int)GEN9TARGET.DLL9X: iRes = _9XdllIMEIGet.Invoke(ByteArray); break;
                        default: strResult = "Unknown Target.";  return false;
                    }

                    bool bRes = false;
                    bRes = iRes.Equals(1);
                    if (bRes)
                    {
                        strResult = Encoding.UTF8.GetString(ByteArray);
                        strResult = strResult.Replace("\0", String.Empty); //널값 제거
                        //14바이트보다 크면 마지막 한바이트가 체크섬 값이므로 제거하고 보내자.
                        if (strResult.Length > 14) strResult = strResult.Substring(0, 14);

                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    }
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public bool Write_IMEI(int iTarget, ref string strResult, string strIMEI)
        {
            lock (lockobjectNadKey)
            {
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + strIMEI);

                try
                {
                    byte[] ByteArray = Encoding.UTF8.GetBytes(strIMEI);
                    if (ByteArray.Length != 14)
                    {
                        strResult = "IMEI SIZE : " + ByteArray.Length.ToString();
                        return false;
                    }

                    int iRes = 0;

                    switch (iTarget)
                    {
                        case (int)GEN9TARGET.DLL97: iRes = _97dllIMEISet.Invoke(ByteArray); break;
                        case (int)GEN9TARGET.DLL94: iRes = _94dllIMEISet.Invoke(ByteArray); break;
                        //case (int)GEN9TARGET.DLL9X: iRes = _9XdllIMEISet.Invoke(ByteArray); break;
                        default: strResult = "Unknown Target."; return false;
                    }

                    bool bRes = false;
                    bRes = iRes.Equals(1);
                    if (bRes)
                    {
                        strResult = Encoding.UTF8.GetString(ByteArray);
                        strResult = strResult.Replace("\0", String.Empty); //널값 제거
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    }
                    else
                    {
                        strResult = "FAIL";
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    }

                    return bRes;
                }
                catch { }

                strResult = "DLL ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }


        }

        [HandleProcessCorruptedStateExceptions]
        public bool CheckSum_IMEI(int iTarget, ref string strResult, string strIMEI)
        {
            lock (lockobjectNadKey)
            {
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strIMEI);

                try
                {
                    byte[] ByteArray = Encoding.UTF8.GetBytes(strIMEI);
                    if (ByteArray.Length != 14)
                    {
                        strResult = "IMEI SIZE : " + ByteArray.Length.ToString();
                        return false;
                    }

                    int iRes = 0;

                    switch (iTarget)
                    {
                        case (int)GEN9TARGET.DLL97: iRes = _97dllMakeCheckSum.Invoke(ByteArray); break;
                        case (int)GEN9TARGET.DLL94: iRes = _94dllMakeCheckSum.Invoke(ByteArray); break;
                        //case (int)GEN9TARGET.DLL9X: iRes = _9XdllMakeCheckSum.Invoke(ByteArray); break;
                        default: strResult = "Unknown Target."; return false;
                    }
                                        
                    bool bRes = false;
                    bRes = iRes.Equals(1);

                    if (bRes)
                    {
                        strResult = iRes.ToString();
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    }
                    else
                    {
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    }
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_MSISDN(int iTarget, ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[15];
                try
                {
                    int iRes = 0;

                    switch (iTarget)
                    {
                        case (int)GEN9TARGET.DLL97: iRes = _97dllnReadMSISDN.Invoke(ByteArray); break;
                        case (int)GEN9TARGET.DLL94: iRes = _94dllnReadMSISDN.Invoke(ByteArray); break;
                        //case (int)GEN9TARGET.DLL9X: iRes = _9XdllnReadMSISDN.Invoke(ByteArray); break;
                        default: strResult = "Unknown Target."; return false;
                    }

                    bool bRes = false;
                    bRes = iRes.Equals(1);

                    if (bRes)
                    {
                        strResult = Encoding.UTF8.GetString(ByteArray);
                        strResult = strResult.Replace("\0", String.Empty); //널값 제거
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    }
                    else
                    {
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    }
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }


        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_ICCID(int iTarget, ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[21]; //문서에는 21byte 라고 되어 있으나 실제는 20바이트 ICCID ... 혹시나 해서 21바이트로 설정.
                try
                {
                    int iRes = 0;

                    switch (iTarget)
                    {
                        case (int)GEN9TARGET.DLL97: iRes = _97dllnReadICCID.Invoke(ByteArray); break;
                        case (int)GEN9TARGET.DLL94: iRes = _94dllnReadICCID.Invoke(ByteArray); break;
                        //case (int)GEN9TARGET.DLL9X: iRes = _9XdllnReadICCID.Invoke(ByteArray); break;
                        default: strResult = "Unknown Target."; return false;
                    }

                    bool bRes = false;
                    bRes = iRes.Equals(1);

                    if (bRes)
                    {
                        strResult = Encoding.UTF8.GetString(ByteArray);
                        strResult = strResult.Replace("\0", String.Empty); //널값 제거
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    }
                    else
                    {
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    }
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }


        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_IMSI(int iTarget, ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[15];
                try
                {
                    int iRes = 0;

                    switch (iTarget)
                    {
                        case (int)GEN9TARGET.DLL97: iRes = _97dllnReadIMSI.Invoke(ByteArray); break;
                        case (int)GEN9TARGET.DLL94: iRes = _94dllnReadIMSI.Invoke(ByteArray); break;
                        //case (int)GEN9TARGET.DLL9X: iRes = _9XdllnReadIMSI.Invoke(ByteArray); break;
                        default: strResult = "Unknown Target."; return false;
                    }

                    bool bRes = false;
                    bRes = iRes.Equals(1);

                    if (bRes)
                    {
                        strResult = Encoding.UTF8.GetString(ByteArray);
                        strResult = strResult.Replace("\0", String.Empty); //널값 제거
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    }
                    else
                    {
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    }
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }


        }

#endregion

        private void SaveLog(string strCommandName, string strLog)
        {            
            strLog = strLog.Replace("\n", "[CR]");
            if (strCommandName.Length > 0)
            {
                DKQLogger.WriteCommLog(strLog + "-" + strCommandName, "NADKEYDLL", false);
            }
            else
            {
                DKQLogger.WriteCommLog(strLog, "NADKEYDLL", false);
            }

        }

        private void GateWay_NadKeyDll(string cParam) //로깅할때 데이터를 다시 실시간으로 manager 로 보내자.
        {
            NadKeyDllRealTimeTxRxMsg(0, cParam);
        }
        
    }
}
