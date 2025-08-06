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
    class DK_PEPUDLL 
    {   
        [DllImport("PEPU.dll", CharSet = CharSet.Ansi)]
        extern public static bool get_password(IntPtr pszSN, int nSnLen, byte[] pszPW, int nPwLen);

        [DllImport("PEPU.dll", CharSet = CharSet.Ansi)]
        extern public static IntPtr get_error();
    
        //---------------------------------------------------------

        [DllImport("PEPU.dll", CharSet = CharSet.Ansi)]
        extern public static IntPtr get_dump_file(IntPtr pszFilename);

        [DllImport("PEPU.dll", CharSet = CharSet.Ansi)]
        extern public static bool check_seed_file(IntPtr pszPlatform, IntPtr pszPath);


      
        public DK_PEPUDLL()
        {
    
        }

        ~DK_PEPUDLL()
        {
    
        }
         
        public bool GetPassword(string strSN, ref string strPassWord, ref string strReason)
        {
            try
            {
                byte[] sss = new byte[128];
                string strTempSN = strSN;
                strPassWord = "";

                IntPtr intPointer = new IntPtr();
                intPointer = Marshal.StringToHGlobalAnsi(strTempSN);

                int aaa = 8;

                bool bSuccess = get_password(intPointer, strSN.Length, sss, aaa);

                if (bSuccess)
                {
                    strPassWord = Encoding.UTF8.GetString(sss).Replace("\0", "");
                }
                IntPtr dataerr = new IntPtr();
                dataerr = get_error();
                strReason = Marshal.PtrToStringAnsi(dataerr);

                try
                {
                    Marshal.FreeHGlobal(intPointer);
                }
                catch{}
                
                return bSuccess;
            }
            catch (Exception dllex)
            {
                strReason = "DLL ERROR : " + dllex.Message;

                return false;
            }
           
        }

        public bool CheckSeedFile(string pszPlatform, string pszPath, ref string strReason)
        {
            try
            {
                IntPtr intPointerPlatform = new IntPtr();
                IntPtr intPointerPath     = new IntPtr();
                IntPtr dataerr            = new IntPtr();

                intPointerPlatform = Marshal.StringToHGlobalAnsi(pszPlatform);
                intPointerPath = Marshal.StringToHGlobalAnsi(pszPath);

                bool bSuccess = check_seed_file(intPointerPlatform, intPointerPath);
                
                if (bSuccess)
                {
                    strReason = "Success";
                   
                }
                else
                {
                    dataerr = get_error();
                    strReason = "Failure-" + Marshal.PtrToStringAnsi(dataerr);

                    string strDumpMsg = String.Empty;
                    string strDumpMsgResaon = String.Empty;
                    bool bDump = GetDumpFile(pszPath, ref strDumpMsg, ref strDumpMsgResaon);

                    if (bDump)
                        strReason += ",get_dump_file :" + strDumpMsg;
                    else
                        strReason += ",get_dump_file(fail):" + strDumpMsgResaon;
                }
                
                try
                {
                    Marshal.FreeHGlobal(intPointerPlatform);
                    Marshal.FreeHGlobal(intPointerPath);                    
                }
                catch { }

                return bSuccess;
            }
            catch (Exception dllex)
            {
                strReason = "DLL ERROR : " + dllex.Message;

                return false;
            }

        }

        private bool GetDumpFile(string pszPath, ref string strResult, ref string strReason)
        {
            strResult = "";

            try
            {               
                IntPtr intPointerPath = new IntPtr();
                IntPtr dumpMessage = new IntPtr();
                                
                intPointerPath = Marshal.StringToHGlobalAnsi(pszPath);

                dumpMessage = get_dump_file(intPointerPath);
                
                strResult = Marshal.PtrToStringAnsi(dumpMessage);
                
                try
                {
                    Marshal.FreeHGlobal(intPointerPath);                          
                }
                catch { }

                return true;
            }
            catch (Exception dllex)
            {
                strReason = "DLL ERROR : " + dllex.Message;

                return false;
            }

        }

    }
}
