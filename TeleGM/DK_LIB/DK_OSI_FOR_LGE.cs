using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GmTelematics
{
    public static class OSI_FOR_LGE_DLL
    {
        //
        //
        //public const string STR_LSFM_OSI_DLL_NAME = @"\OSIForLGE_DLL\LSFM_OSI_UP_X86_TEST.dll";
        public const string STR_LSFM_OSI_DLL_NAME = @"\OSIForLGE_DLL\FOR_LGE_DLL_x86.dll";


        [DllImport(STR_LSFM_OSI_DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int dbConnection(StringBuilder chSite, StringBuilder chID, StringBuilder chPWD, StringBuilder chCHK);

        [DllImport(STR_LSFM_OSI_DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int checkSerialNo(StringBuilder chSerial, StringBuilder chCHK);

        [DllImport(STR_LSFM_OSI_DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int checkPackingSerialNo(StringBuilder chSerial, StringBuilder chCHK);

        [DllImport(STR_LSFM_OSI_DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //internal static extern int insert_Result(StringBuilder chSerial, StringBuilder chResult, StringBuilder chKind, StringBuilder chCHK);
        internal static extern int insert_Result(StringBuilder chSerial, StringBuilder chResult, StringBuilder chKind,
            StringBuilder chOK_NG, StringBuilder chModelSuffix, StringBuilder chCategoryName, StringBuilder chCHK);

        [DllImport(STR_LSFM_OSI_DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int dbDisConnnection(StringBuilder chID, StringBuilder chPWD, StringBuilder chCHK);

        [DllImport(STR_LSFM_OSI_DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int setTestMode(StringBuilder chResult); // 이 function은 나중에 삭제될것입니다.

        #region OSI_for_LGE Simulator
        //public static int dbConnection(StringBuilder chID, StringBuilder chPWD, out StringBuilder chCHK)
        //{
        //    chCHK = new StringBuilder("pass");
        //    return 0;

        //    //chCHK = new StringBuilder("fail");
        //    //return 0;

        //}

        //public static int checkSerialNo(StringBuilder chSerial, out StringBuilder chCHK)
        //{
        //    //chCHK = new StringBuilder("RESULT = OK,{SW_TO=111101,NAME=ABCDEFG}");
        //    chCHK = new StringBuilder("RESULT=OK,{STID=226986117;TRACE=5124153000003501;DUNS=555343750;}");
        //    return 0;

        //    //chCHK = new StringBuilder("RESULT = OK,SW_TO =");
        //    //return 0;

        //    //chCHK = new StringBuilder("RESULT = NG");
        //    //return 0;
        //}

        //public static int updateOSI_Result(StringBuilder chSerial, StringBuilder chResult, StringBuilder chKind, out StringBuilder chCHK)
        //{
        //    chCHK = new StringBuilder("pass");
        //    //chCHK = "fail";
        //    //return 0;


        //    //chCHK = new StringBuilder("fail");
        //    return 0;
        //}
        #endregion
    }

    enum OSIForLGEResultKind
    {
        BASIC,
        DETAIL
    }

    class DK_OSI_FOR_LGE
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        private IntPtr WindowHwnd;
        private uint iMsgID;

        private int nConnStatus = -1;
        private Dictionary<string, string>[] dicInspData = new Dictionary<string, string>[10];
        private string[] ErrMsgs = new string[20];
        private string[] WipId = new string[10];

        public string StartTime = string.Empty;
        public string EndTime = string.Empty;
        public int PassCount = 0;
        public int FailCount = 0;
        public StringBuilder ConnID;
        public StringBuilder ConnPWD;

        public bool MES_ON { get; set; }

        public DK_OSI_FOR_LGE(IntPtr cppHwnd, uint iMsgId)
        {
            for (int i = 1; i < 20; i++)
                ErrMsgs[i] = "Error No : " + i.ToString();

            for (int i = 0; i < 10; i++)
                dicInspData[i] = new Dictionary<string, string>();

            WindowHwnd = cppHwnd;
            iMsgID = iMsgId;

            //SetTestMode 실행 => 테스트모드
#if DEBUG
            SetTestMode();
#endif
        }


        public void SetTestMode()
        {

            //=OK,{STID=226792230;TRACE=5124304000006304;DUNS=555343750;}
            StringBuilder chCHK = new StringBuilder("RESULT=OK,{STID=226986117;TRACE=5124153000003501;DUNS=555343750;}");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK,{STID=226792230;TRACE=5124304000006304;DUNS=555343   ");
            //StringBuilder chCHK = new StringBuilder("R=OK,{STID=226792230;TRACE=5124304000006304;DUNS=555343750;}");
            //StringBuilder chCHK = new StringBuilder("R=OK,{STID=226792230;}");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK,STID=226986361,TRACE=5124153000003501,DUNS=555343750;");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK,{SW_TO=111101;SW_FROM=22222;}");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK,{SW_TO=111101;}");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK,");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK,{}");
            //StringBuilder chCHK = new StringBuilder("RESULT=OK");

            int nReturn = OSI_FOR_LGE_DLL.setTestMode(chCHK);
            if (nReturn == 0)
            {

            }
        }

        public void SetConfig(string strSiteCode, string strID, string strPWD, out string retMsg)
        {
            ConnID = new StringBuilder(strID);
            ConnPWD = new StringBuilder(strPWD);
            StringBuilder chCHK = new StringBuilder();
            StringBuilder chSite = new StringBuilder(strSiteCode);

            nConnStatus = OSI_FOR_LGE_DLL.dbConnection(chSite, ConnID, ConnPWD, chCHK);

            retMsg = chCHK.ToString();
            if (nConnStatus == 0)
            {
                string strChk = chCHK.ToString();
                if (strChk.ToUpper().Contains("FAIL"))
                {
                    nConnStatus = 10;
                    ErrMsgs[nConnStatus] = "dbConnnection fail";
                }
            }

            if (nConnStatus == 0)
                SendMessage(WindowHwnd, iMsgID, 1, 1);
            else
                SendMessage(WindowHwnd, iMsgID, 1, 0);

        }

        public int GetConnStatus()
        {
            return nConnStatus;
        }

        public int StepCheck(int iSlot, string strWipId, out string retMsg)
        {
            StringBuilder chID = new StringBuilder(strWipId);
            StringBuilder chCHK = new StringBuilder(1024);  //20250626  사이즈를 할당해야 함 ㅜㅜ
            WipId[iSlot] = "";
            dicInspData[iSlot].Clear();
                      
            int nReturn = OSI_FOR_LGE_DLL.checkSerialNo(chID, chCHK);

            //string sData = string.Empty;
            //retMsg = sData = chCHK.ToString();
            retMsg = chCHK.ToString();

            if (nReturn == 0)
            {
                //RESULT=OK,{SW_TO=111101;SW_FROM=22222;}
                //sData = sData.Replace("{", "").Replace("}", "");
                //sData = sData.Replace("}", "");
                string[] ChkData = chCHK.ToString().Split(',');
                string[] result = ChkData[0].Split('=');
                if (result[1].ToUpper().Contains("OK"))
                {
                    if (ChkData.Length > 1)
                    {
                        if (ChkData[1].Contains("{"))
                            ChkData[1] = ChkData[1].Replace("{", "").Replace("}", "");

                        string[] Attribute = ChkData[1].Split(';');
                        for (int i = 0; i < Attribute.Length; i++)
                        {
                            if (Attribute[i] == "") break;
                            string[] Value = Attribute[i].Split('=');
                            dicInspData[iSlot].Add(Value[0], Value[1]);
                        }
                    }

                    WipId[iSlot] = strWipId;
                }
                else
                {
                    nReturn = 11;
                    ErrMsgs[nReturn] = "checkSerialNo NG";
                }
            }
            return nReturn;
           
        }

        public int PackStepCheck(int iSlot, string strWipId, out string retMsg)
        {
            StringBuilder chID = new StringBuilder(strWipId);
            StringBuilder chCHK = new StringBuilder();
            WipId[iSlot] = "";
            dicInspData[iSlot].Clear();
            int nReturn = OSI_FOR_LGE_DLL.checkPackingSerialNo(chID, chCHK);

            //string sData = string.Empty;
            //retMsg = sData = chCHK.ToString();

            //TEST
            //nReturn = 1;
            //retMsg = "RESULT:NA;";

            retMsg = chCHK.ToString();

            if (nReturn == 0)
            {
                //RESULT=OK,{SW_TO=111101;SW_FROM=22222;}
                //sData = sData.Replace("{", "").Replace("}", "");
                //sData = sData.Replace("}", "");

                string[] ChkData = chCHK.ToString().Split(',');
                string[] result = ChkData[0].Split('=');
                if (result[1].ToUpper().Contains("OK"))
                {
                    if (ChkData.Length > 1)
                    {
                        if (ChkData[1].Contains("{"))
                            ChkData[1] = ChkData[1].Replace("{", "").Replace("}", "");

                        string[] Attribute = ChkData[1].Split(';');
                        for (int i = 0; i < Attribute.Length; i++)
                        {
                            if (Attribute[i] == "") break;
                            string[] Value = Attribute[i].Split('=');
                            dicInspData[iSlot].Add(Value[0], Value[1]);
                        }
                    }

                    WipId[iSlot] = strWipId;
                }
                else
                {
                    nReturn = 11;
                    ErrMsgs[nReturn] = "checkPackingSerialNo NG";
                }
            }
            return nReturn;
        }

        public int StepComplete(int iSlot, string strWipId, string strResult, string strchOK_NG, string strchModelSuffix, string strchCategoryName, out string retMsg)
        {
            StringBuilder chSerial = new StringBuilder(strWipId);
            StringBuilder chResult = new StringBuilder(strResult);
            StringBuilder chKind = new StringBuilder(OSIForLGEResultKind.BASIC.ToString());
            StringBuilder chCHK = new StringBuilder(1024);

            //StringBuilder chOK_NG, StringBuilder chModelSuffix, StringBuilder chCategoryName, StringBuilder chCHK);
            StringBuilder chOK_NG = new StringBuilder(strchOK_NG);
            StringBuilder chModelSuffix = new StringBuilder(strchModelSuffix);
            StringBuilder chCategoryName = new StringBuilder(strchCategoryName);

            //int nReturn = OSI_FOR_LGE_DLL.insert_Result(chSerial, chResult, chKind, chCHK);
            int nReturn = OSI_FOR_LGE_DLL.insert_Result(chSerial, chResult, chKind, chOK_NG, chModelSuffix, chCategoryName, chCHK);
            retMsg = chCHK.ToString();
            if (nReturn == 0)
            {
                string strChk = chCHK.ToString();
                if (strChk.ToUpper().Contains("FAIL"))
                {
                    nReturn = 12;
                    ErrMsgs[nReturn] = "insert_Result[BASIC] fail";
                }
            }

            return nReturn;
        }

        public int StepComplete_Detail(int iSlot, string strWipId, string strResult, string strchOK_NG, string strchModelSuffix, string strchCategoryName, out string retMsg)
        {
            StringBuilder chSerial = new StringBuilder(strWipId);
            StringBuilder chResult = new StringBuilder(strResult);
            StringBuilder chKind = new StringBuilder(OSIForLGEResultKind.DETAIL.ToString());
            StringBuilder chCHK = new StringBuilder();

            StringBuilder chOK_NG = new StringBuilder(strchOK_NG);
            StringBuilder chModelSuffix = new StringBuilder(strchModelSuffix);
            StringBuilder chCategoryName = new StringBuilder(strchCategoryName);

            //int nReturn = OSI_FOR_LGE_DLL.insert_Result(chSerial, chResult, chKind, chCHK);
            int nReturn = OSI_FOR_LGE_DLL.insert_Result(chSerial, chResult, chKind, chOK_NG, chModelSuffix, chCategoryName, chCHK);

            retMsg = chCHK.ToString();
            if (nReturn == 0)
            {
                string strChk = chCHK.ToString();
                if (strChk.ToUpper().Contains("FAIL"))
                {
                    nReturn = 13;
                    ErrMsgs[nReturn] = "insert_Result[Detail] fail";
                }
            }
            return nReturn;
        }

        public int Disconnect()
        {
            //if (nConnStatus != 0) return 0;
            //int nReturn;
            //try
            //{
            //    StringBuilder chCHK = new StringBuilder();

            //    nReturn = OSI_FOR_LGE_DLL.dbDisConnnection(ConnID, ConnPWD, chCHK);
            //    if (nReturn == 0)
            //    {
            //        string strChk = chCHK.ToString();
            //        if (strChk.ToUpper().Contains("FAIL"))
            //        {
            //            nReturn = 14;
            //            ErrMsgs[nReturn] = "dbDisConnnection fail";
            //        }
            //    }

            //    if (nReturn == 0)
            //        nConnStatus = -1;
            //}
            //catch (Exception ex)
            //{
            //    nReturn = 15;
            //    ErrMsgs[nReturn] = ex.Message;
            //    nConnStatus = -1;
            //}
            
            nConnStatus = 1;
            SendMessage(WindowHwnd, iMsgID, 1, 0);
            //return nReturn;
            return 1;
        }

        public bool GetInsp(int iSlot, string strInspName, ref string strRtnInsp)
        {
            try
            {
                if (strInspName.StartsWith("@"))
                    strInspName = strInspName.Substring(1);

                strRtnInsp = dicInspData[iSlot][strInspName];
                return true;
            }
            catch (Exception e)
            {
                strRtnInsp = e.Message;
                return false;
            }
        }

        public string GetErrString(int iErrCode)
        {
            string strRtnMsg = String.Empty;

            if (iErrCode < ErrMsgs.Length)
            {
                strRtnMsg = ErrMsgs[iErrCode];
            }
            else
            {
                strRtnMsg = "Error No : " + iErrCode.ToString();
            }
            return strRtnMsg;
        }

        public string GetWipInfo(int iSlot)
        {
            return WipId[iSlot];
        }

        public void Clear(int iSlot)
        {
            StartTime = "";
            EndTime = "";
            WipId[iSlot] = "";
            dicInspData[iSlot].Clear();
        }


        public string GetDLLName()
        {
            return Path.GetFileName(OSI_FOR_LGE_DLL.STR_LSFM_OSI_DLL_NAME);
        }

        public string GetVersion()
        {
            string fn = Environment.CurrentDirectory + @"\" + OSI_FOR_LGE_DLL.STR_LSFM_OSI_DLL_NAME;

            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(fn);

            return myFileVersionInfo.FileVersion + "[" + File.GetCreationTime(fn).ToString("yyyy-MM-dd") + "]";
        }

    }
}
