using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace GmTelematics
{
    class DK_NADKEYWRITER
    {
        private const string strNadDllName = "LTE_API.dll"; //김태완 과장님 지원
        public event EventRealTimeMsg NadKeyDllRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
        private DK_LOGGER DKQLogger = new DK_LOGGER("SET", false);
        private bool isOpenCommand = false;
        private bool bLoadDllFile = false;

        //1. PORT OPEN
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool PortOpen(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int StopBit, int FlowControl);

        //2. PORT CLOSE
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool PortClose();

        //3. READ IMEI
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool IMEIGet(byte[] iStrPointer);

        //4. WRITE IMEI
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool IMEISet(byte[] iStrPointer);

        //5. CHECKSUM IMEI
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static int MakeCheckSum(byte[] iStrPointer);

        //6. NV RESTORE
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool NVRestore(byte[] iStrPointer);

        //7. MSISDN
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool nReadMSISDN(byte[] iStrPointer);

        //8. ICCID
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool nReadICCID(byte[] iStrPointer);
        
        //9. IMSI
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool nReadIMSI(byte[] iStrPointer);
        
        //10. EFS_BAKCUP
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool EFSBackup();
        
        //11. DLLVersion
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool DLLVersion(byte[] iStrPointer);
        
        //12. ReadSCNV
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool ReadSCNV(byte[] iStrPointer);
        
        
        //------------ API 전
        //8. LTE_DLL_OPEN
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool LTE_DLL_OPEN(byte[] iStrPointer);

        //9. LTE_DLL_CLOSE
        [DllImport(strNadDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        extern public static bool LTE_DLL_CLOSE();
        //------------------
        
        
        public DK_NADKEYWRITER()
        {
            DKQLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_NadKeyDll);
        }

        public bool IsDllFileLoad()
        {
            return bLoadDllFile;
        }

        public bool SetDllFileName(string strFileName)
        {
            byte[] bName = Encoding.UTF8.GetBytes(strFileName);
            bLoadDllFile = LTE_DLL_OPEN(bName);
            return IsDllFileLoad();
        }

        public bool ReleaseDllFile()
        {
            bLoadDllFile = false;
            return LTE_DLL_CLOSE();
        }

        public bool IsPortOpen()
        {
            return isOpenCommand;
        }

        public bool API_PortOpen(int iPortNum, int iBaudRate, int DataBit, int iParityBit, int iStopBit, int iFlowControl)
        {
            isOpenCommand = false;

            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                PortClose();
                System.Threading.Thread.Sleep(50);
                isOpenCommand = PortOpen(iPortNum, iBaudRate, DataBit, iParityBit, iStopBit, iFlowControl);
                SaveLog("", "[RX] PortOpen:" + isOpenCommand.ToString());
                return IsPortOpen();
            }
            catch{}
            
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Failure");
            return IsPortOpen();
        }

        public bool API_PortClose()
        {
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            isOpenCommand = false;
            bool bClose = false;
            try
            {
                bClose = PortClose();
                System.Threading.Thread.Sleep(1000);
                bClose = LTE_DLL_CLOSE();
            }
            catch { }
            string strbClose = ":true";
            if(!bClose) strbClose = ":false";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + strbClose);
            return true;
        }

        public bool API_Read_IMEI(ref string strResult)
        {
            strResult = String.Empty;
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            byte[] ByteArray = new byte[15];
            try
            {
                bool bRes = IMEIGet(ByteArray);
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

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;

        }

        public bool API_Write_IMEI(ref string strResult, string strIMEI)
        {
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + strIMEI);

            try
            {
                byte[] ByteArray = Encoding.ASCII.GetBytes(strIMEI);
                if (ByteArray.Length != 14)
                {
                    strResult = "IMEI SIZE : " + ByteArray.Length.ToString();
                    return false;
                }

                bool bRes = IMEISet(ByteArray);
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

        public int API_CheckSum_IMEI(ref string strResult, string strIMEI)
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

                int iRes = MakeCheckSum(ByteArray);
                strResult = iRes.ToString();
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + iRes.ToString());
                return iRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return -999;

        }

        public bool API_NV_Restore(ref string strResult)
        {
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            try
            {
                byte[] ByteArray = new byte[1024];

                bool bRes = NVRestore(ByteArray);
                strResult = Encoding.UTF8.GetString(ByteArray);
                strResult = strResult.Trim();
                strResult = strResult.Replace("\0", String.Empty);
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString() + "(" + strResult + ")");
                return bRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;

        }

        public bool API_Read_MSISDN(ref string strResult)
        {
            strResult = String.Empty;
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            byte[] ByteArray = new byte[15];
            try
            {
                bool bRes = nReadMSISDN(ByteArray);
                strResult = Encoding.UTF8.GetString(ByteArray);
                strResult = strResult.Replace("\0", String.Empty); //널값 제거
                if (bRes)
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                else
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                return bRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;

        }

        public bool API_Read_ICCID(ref string strResult)
        {
            strResult = String.Empty;
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            byte[] ByteArray = new byte[15];
            try
            {
                bool bRes = nReadICCID(ByteArray);
                strResult = Encoding.UTF8.GetString(ByteArray);
                strResult = strResult.Replace("\0", String.Empty); //널값 제거
                if (bRes)
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                else
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                return bRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;

        }
        
        public bool API_Read_IMSI(ref string strResult)
        {
            strResult = String.Empty;
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            byte[] ByteArray = new byte[15];
            try
            {
                bool bRes = nReadIMSI(ByteArray);
                strResult = Encoding.UTF8.GetString(ByteArray);
                strResult = strResult.Replace("\0", String.Empty); //널값 제거
                if (bRes)
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                else
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                return bRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;
        }
        
        public bool API_EFS_Backup(ref string strResult)
        {
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            try
            {                
                bool bRes = EFSBackup();
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

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;

        }
        
        public bool Get_DllVersion(ref string strResult)
        {
            strResult = String.Empty;
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            byte[] ByteArray = new byte[255];
            try
            {
                bool bRes = DLLVersion(ByteArray);
                strResult = Encoding.UTF8.GetString(ByteArray);
                strResult = strResult.Replace("\0", String.Empty); //널값 제거
                if (bRes)
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                else
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                return bRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;
        }
        
        public bool Read_SCNV(ref string strResult)
        {
            strResult = String.Empty;
            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            byte[] ByteArray = new byte[255];
            try
            {
                bool bRes = ReadSCNV(ByteArray);
                strResult = Encoding.UTF8.GetString(ByteArray);
                strResult = strResult.Replace("\0", String.Empty); //널값 제거
                if (bRes)
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
                else
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + bRes.ToString());
                return bRes;
            }
            catch { }

            strResult = "DLL ERROR.";
            SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + strResult);
            return false;

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
