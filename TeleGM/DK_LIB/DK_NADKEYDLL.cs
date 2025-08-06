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
    class DK_NADKEYDLL
    {   
        //DLL를 동적으로 Load 하기 위한 Kernel dll import.
        //------------------------------------------------------------------------------------------------------
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        static extern bool FreeLibrary(int hModule);


        //NAD DLL 의 명령어 정의
        //------------------------------------------------------------------------------------------------------        
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_PortOpen(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int StopBit, int FlowControl);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_PortClose();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_IMEIGet(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_IMEISet(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate int  DLL_MakeCheckSum(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_NVRestore(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_nReadMSISDN(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_nReadICCID(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_nReadIMSI(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_EFSBackup();

        //------------------------------------------------------------------------------------------------------
        //LTE_CHINA_NEW_20.DLL 추가.
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_DLLVersion(byte[] iStrPointer);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_ReadSCNV(int[] intPointer);

        //------------------------------------------------------------------------------------------------------
        //LTE_NEW_21.DLL 추가.
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate bool DLL_NVGet(ref STR_NV snv, int iNvNum);
        //------------------------------------------------------------------------------------------------------
        

        private string strNadDllName; //NA향
        public event EventRealTimeMsg NadKeyDllRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
        private DK_LOGGER DKQLogger;
        private bool isOpenCommand;
        private int      iLibAddress;
        private IntPtr[] iLibFunction;

        private DLL_PortOpen        dlPortOpen;
        private DLL_PortClose       dlPortClose;
        private DLL_IMEIGet         dlIMEIGet;
        private DLL_IMEISet         dlIMEISet;
        private DLL_MakeCheckSum    dlMakeCheckSum;
        private DLL_NVRestore       dlNVRestore;
        private DLL_nReadMSISDN     dlnReadMSISDN;
        private DLL_nReadICCID      dlnReadICCID;
        private DLL_nReadIMSI       dlnReadIMSI;
        private DLL_EFSBackup       dlEFSBackup;
        //LTE_CHINA_NEW_20.DLL 추가.
        private DLL_DLLVersion      dlnDllVersion;
        private DLL_ReadSCNV        dlnReadSCNV;
        private DLL_NVGet           dlnNVGet;

        private object              lockobjectNadKey;

        public DK_NADKEYDLL()
        {
            strNadDllName = "LTE.dll"; //Default Name
            iLibAddress = 0;
            iLibFunction = new IntPtr[(int)NADDLLIDX.END];
            isOpenCommand = false;
            lockobjectNadKey = new object();
            DKQLogger = new DK_LOGGER("SET", false);
            DKQLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_NadKeyDll);
                       

        }

      
        public bool SetDllFileName(string strFileName)
        {

            try
            {
                if (IsOnLibrary()) dlPortClose.Invoke();               
            }
            catch { }
                        
            isOpenCommand = false;

            if (iLibAddress != 0)
            {
                FreeLibrary(iLibAddress);
                iLibAddress = 0;
            }

            strNadDllName = strFileName;

            iLibAddress = LoadLibrary(strNadDllName);
            if (iLibAddress == 0) return false;              

            MakeCommands();                    
            
            return true;
            
        }

        private void MakeCommands()
        {
            iLibFunction = new IntPtr[(int)NADDLLIDX.END];
            iLibFunction[(int)NADDLLIDX.PORT_OPEN]      = GetProcAddress(iLibAddress, "PortOpen");
            iLibFunction[(int)NADDLLIDX.PORT_CLOSE]     = GetProcAddress(iLibAddress, "PortClose");
            iLibFunction[(int)NADDLLIDX.READ_IMEI]      = GetProcAddress(iLibAddress, "IMEIGet");
            iLibFunction[(int)NADDLLIDX.WRITE_IMEI]     = GetProcAddress(iLibAddress, "IMEISet");
            iLibFunction[(int)NADDLLIDX.CHECKSUM_IMEI]  = GetProcAddress(iLibAddress, "MakeCheckSum");
            iLibFunction[(int)NADDLLIDX.NV_RESTORE]     = GetProcAddress(iLibAddress, "NVRestore");
            iLibFunction[(int)NADDLLIDX.READ_MSISDN]    = GetProcAddress(iLibAddress, "nReadMSISDN");
            iLibFunction[(int)NADDLLIDX.READ_ICCID]     = GetProcAddress(iLibAddress, "nReadICCID");
            iLibFunction[(int)NADDLLIDX.READ_IMSI]      = GetProcAddress(iLibAddress, "nReadIMSI");
            iLibFunction[(int)NADDLLIDX.EFS_BACKUP]     = GetProcAddress(iLibAddress, "EFSBackup");

            //LTE_CHINA_NEW_20.DLL 추가.
            iLibFunction[(int)NADDLLIDX.DLL_VERSION] = GetProcAddress(iLibAddress, "Read_DLLVersion");
            iLibFunction[(int)NADDLLIDX.READ_SCNV]   = GetProcAddress(iLibAddress, "ReadSCNV");

            //LTE_NEW_21.DLL 추가.
            iLibFunction[(int)NADDLLIDX.NV_GET]         = GetProcAddress(iLibAddress, "NVGet");            


            dlPortOpen      = (DLL_PortOpen)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.PORT_OPEN], typeof(DLL_PortOpen));
            dlPortClose     = (DLL_PortClose)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.PORT_CLOSE], typeof(DLL_PortClose));
            dlIMEIGet       = (DLL_IMEIGet)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.READ_IMEI], typeof(DLL_IMEIGet));
            dlIMEISet       = (DLL_IMEISet)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.WRITE_IMEI], typeof(DLL_IMEISet));
            dlMakeCheckSum  = (DLL_MakeCheckSum)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.CHECKSUM_IMEI], typeof(DLL_MakeCheckSum));
            dlNVRestore     = (DLL_NVRestore)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.NV_RESTORE], typeof(DLL_NVRestore));
            dlnReadMSISDN   = (DLL_nReadMSISDN)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.READ_MSISDN], typeof(DLL_nReadMSISDN));
            dlnReadICCID    = (DLL_nReadICCID)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.READ_ICCID], typeof(DLL_nReadICCID));
            dlnReadIMSI     = (DLL_nReadIMSI)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.READ_IMSI], typeof(DLL_nReadIMSI));
            

            //LTE_CHINA_NEW_20.DLL 추가. 기존 DLL 은 이것이 없을것이다. 그래서 TRY CATCH
            try
            {
                dlEFSBackup   = (DLL_EFSBackup)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.EFS_BACKUP], typeof(DLL_EFSBackup));
                dlnDllVersion = (DLL_DLLVersion)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.DLL_VERSION], typeof(DLL_DLLVersion));
                dlnReadSCNV   = (DLL_ReadSCNV)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.READ_SCNV], typeof(DLL_ReadSCNV));
            }
            catch
            {
            	
            }

            //LTE_NEW_21.DLL 추가. 기존 DLL 은 이것이 없을것이다. 그래서 TRY CATCH
            try
            {
                dlnNVGet = (DLL_NVGet)Marshal.GetDelegateForFunctionPointer(iLibFunction[(int)NADDLLIDX.NV_GET], typeof(DLL_NVGet));                
            }
            catch
            {
            	
            }
            

        }

        public bool IsOnLibrary()
        {   //DLL 로드 안했으면 false;
            lock (lockobjectNadKey)
            {
                if (iLibAddress != 0) return true;

                return false;
            }
            
        }

        public void UnloadLibrary()
        {   //DLL 로드 안했으면 false;
            lock (lockobjectNadKey)
            {
                if (iLibAddress != 0)
                {
                    FreeLibrary(iLibAddress);
                    iLibAddress = 0;
                    isOpenCommand = false;
                }
            }
        }

        public bool IsPortOpen()
        {
            return isOpenCommand;
        }

        public bool PortOpen(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int iStopBit, int iFlowControl)

        {
            try
            {
                if (IsOnLibrary()) dlPortClose.Invoke();

            }
            catch { }

            lock (lockobjectNadKey)
            {
                isOpenCommand = false;

                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                try
                {
                    isOpenCommand = dlPortOpen.Invoke(iPortNum, iBaudRate, DataBit, iParityBit, iStopBit, iFlowControl);
                }
                catch
                {
                    isOpenCommand = false;
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Ex Failure");
                    return false;
                }

                SaveLog("", "[RX] PortOpen:" + isOpenCommand.ToString());
                return IsPortOpen();
            }
            
        }

        public bool PortClose()
        {
            lock (lockobjectNadKey)
            {
                isOpenCommand = false;
                try
                {
                    if (IsOnLibrary())
                    {
                        SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                        dlPortClose.Invoke();
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
        public bool Read_IMEI(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[15];
                try
                {
                    bool bRes = dlIMEIGet.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Replace("\0", String.Empty); //널값 제거
                    //14바이트보다 크면 마지막 한바이트가 체크섬 값이므로 제거하고 보내자.
                    if (strResult.Length > 14) strResult = strResult.Substring(0, 14);

                    if (bRes)
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public bool Write_IMEI(ref string strResult, string strIMEI)
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

                    bool bRes = dlIMEISet.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Replace("\0", String.Empty); //널값 제거
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            

        }

        [HandleProcessCorruptedStateExceptions]
        public int  CheckSum_IMEI(ref string strResult, string strIMEI)
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
                        return -999;
                    }

                    int iRes = dlMakeCheckSum.Invoke(ByteArray);
                    strResult = iRes.ToString();
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                    return iRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return -999; 
            }   
        }

        [HandleProcessCorruptedStateExceptions]
        public bool NV_Restore(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);

                try
                {
                    byte[] ByteArray = new byte[1024];

                    bool bRes = dlNVRestore.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Trim();
                    strResult = strResult.Replace("\0", String.Empty);
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString() + "(" + strResult + ")");
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            
        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_MSISDN(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[15];
                try
                {
                    bool bRes = dlnReadMSISDN.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Replace("\0", String.Empty); //널값 제거
                    if (bRes)
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            

        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_ICCID(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[21]; //문서에는 21byte 라고 되어 있으나 실제는 20바이트 ICCID ... 혹시나 해서 21바이트로 설정.
                try
                {
                    bool bRes = dlnReadICCID.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Replace("\0", String.Empty); //널값 제거
                    if (bRes)
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            

        }

        [HandleProcessCorruptedStateExceptions]
        public bool Read_IMSI(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[15];
                try
                {
                    bool bRes = dlnReadIMSI.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Replace("\0", String.Empty); //널값 제거
                    if (bRes)
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            

        }

        [HandleProcessCorruptedStateExceptions]
        public bool EFS_Backup(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);

                try
                {
                    bool bRes = dlEFSBackup.Invoke();

                    if (bRes)
                    {
                        strResult = "TRUE";
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    }
                    else
                    {
                        strResult = "FALSE";
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    }
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            

        }
        //LTE CHINA NEW 20. DLL 에 추가된 명령
        [HandleProcessCorruptedStateExceptions]
        public bool Get_DllVersion(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                byte[] ByteArray = new byte[255];
                try
                {
                    bool bRes = dlnDllVersion.Invoke(ByteArray);
                    strResult = Encoding.UTF8.GetString(ByteArray);
                    strResult = strResult.Replace("\0", String.Empty); //널값 제거

                    if (bRes)
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            
        }

        [HandleProcessCorruptedStateExceptions]
        //LTE CHINA NEW 20. DLL 에 추가된 명령
        public bool Read_SCNV(ref string strResult)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                int[] intPointer = new int[128];
                try
                {
                    bool bRes = dlnReadSCNV.Invoke(intPointer);
                    strResult = intPointer[0].ToString(); //Encoding.UTF8.GetString(ByteArray);

                    if (bRes)
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }
            

        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct STR_NV
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1 + 2 + 128 + 2)]
            public byte[] itemdata;

            /*
            public byte  cmdcode;
            public short item;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] itemdata;
            public short nvstat;
            */
        }
        //--------------------------

        [HandleProcessCorruptedStateExceptions]
        //LTE NEW 21. DLL 에 추가된 명령
        public bool NVGet(ref string strResult, int iParam)
        {
            lock (lockobjectNadKey)
            {
                strResult = String.Empty;
                SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                int intNvNum = iParam;
                int[] intPointer = new int[1024];


                                
                try
                {
                    STR_NV bInfo = new STR_NV();
                    bool bRes = dlnNVGet.Invoke(ref bInfo, intNvNum);

                    if (bRes)
                    {
                        strResult = BitConverter.ToString(bInfo.itemdata, 3, 3).Replace("-", "");
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + BitConverter.ToString(bInfo.itemdata).Replace("-", ""));
                    }
                    else
                        SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                    return bRes;
                }
                catch { }

                strResult = "DLL EXCEPTION ERROR.";
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                return false;
            }


        }

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
