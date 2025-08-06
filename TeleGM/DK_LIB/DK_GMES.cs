using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GmTelematics
{
    class DK_GMES
    {
        private const string DLLNAME = "gmes_dll_md.dll";

#region GMES DLL IMPORT

        [DllImport(DLLNAME)]
        extern public static void gConfig(double dT3, double dT6, int iReTryCount, int iMesOn, int iChkLink, int iDownSpec);

        [DllImport(DLLNAME)]
        extern public static void gSetMes(int iOn);

        [DllImport(DLLNAME)]
        extern public static void gSetMsg(IntPtr cppHwnd, int iMsg);

        [DllImport(DLLNAME)]
        extern public static void gStart(byte[] serverIP, byte[] serverPort, byte[] localIP);

        [DllImport(DLLNAME)]
        extern public static void gStop();

        [DllImport(DLLNAME)]
        extern public static void gReconnect();

        //스텝 체크 전 GMES 와 연결되었는지 체크! 
        /// 연결 상태(conn_status)
        /// 0: 연결되지 않음
        /// 1: 소켓 연결
        /// 2: HEY 메시지 교환까지 끝난 상태
        [DllImport(DLLNAME)]
        extern public static int gConnStatus();
        
        //스텝 체크시 사용하기
        [DllImport(DLLNAME)]
        extern public static int gReqSetid(int slot, byte[] wipId);
        
        //에러스트링 가져오기
        [DllImport(DLLNAME)]
        extern public static IntPtr gGetErrString_(int errorCode);

        //에러스트링 사유내용 가져오기
        [DllImport(DLLNAME)]
        extern public static IntPtr gGetReason_();

        //DLL 버젼 가져오기
        [DllImport(DLLNAME)]
        extern public static IntPtr gGetVersion();

        //스텝 컴플리트 사용
        [DllImport(DLLNAME)]
        extern public static int gReqResult(int slotNumber, byte[] result);
        
        //장비정보(EQPNAME, PROCID 등) 가져오기
        [DllImport(DLLNAME)]
        extern public static IntPtr gGetEqpInfo_(byte[] InfoName);

        //스텝체크한 세트의 공정 정보 가져오기
        [DllImport(DLLNAME)]
        extern public static IntPtr gGetSetInfo_(int iSlot, byte[] InfoName);

        //스텝체크한 세트의 아이템 가져오기(SPEC DOWN 한 INSP 항목)
        [DllImport(DLLNAME)]
        extern public static IntPtr gGetSetItem_(int iSlot, byte[] InfoName);
        
#endregion
        //0. GMES SETTINGS
        //1. GMES ON.OFF
        //2. GMES STATUS
        //3. GMES STEP CHECK
        //4. GMES GET ITEM
        //5. GMES STEP COMPLETE
        //6. GMES GET ERR STRING
        //7. GMES GET EQP NAME
        //8. GMES GET PROC NAME
        //9. GMES GET SET PROC NAME

        private IntPtr WindowHwnd;
        private int    iMsgID;

        private IntPtr Item_WindowHwnd
        {
            get { return WindowHwnd; }
            set { WindowHwnd = value; }
        }

        private int Item_MsgID
        {
            get { return iMsgID; }
            set { iMsgID = value; }
        }

        public DK_GMES(IntPtr cppHwnd, int iMsgId)
        {
            Item_WindowHwnd = cppHwnd;
            Item_MsgID = iMsgId;
            GMES_SetMsg();
        }

        private void GMES_SetMsg()
        {
            gSetMsg(Item_WindowHwnd, Item_MsgID);
        }

        public void GMES_SetConfig(string strRetry, string strT3, string strT6, string strSpecDown,
                                   string strServerIP, string strLocalIP, string strPort)
        {
            double dT3 = double.Parse(strT3);
            double dT6 = double.Parse(strT6);
            int    iRetry = int.Parse(strRetry); 
            int    iDownSpec = 0;

            byte[] strSIp = Encoding.UTF8.GetBytes(strServerIP);
            byte[] strPNu = Encoding.UTF8.GetBytes(strPort);
            byte[] strLIp = Encoding.UTF8.GetBytes(strLocalIP);

            if(strSpecDown.Equals("ON")) iDownSpec = 1;
            
            gConfig(dT3, dT6, iRetry, 1, 0, iDownSpec);

            gStart(strSIp, strPNu, strLIp);

        }        

        public void GMES_ON()
        {
            gSetMes(1);
        }

        public void GMES_OFF()
        {
            gStop();
            gSetMes(0);
        }

        public void GMES_Reconnect()
        {

        }

        public int GMES_GetStatus()
        {
            return gConnStatus();
        }

        public int GMES_StepCheck(int iSlot, string strWipId)
        {
            byte[] strWip = Encoding.UTF8.GetBytes(strWipId);
            return gReqSetid(iSlot, strWip); 
        }

        public int GMES_StepComplete(int iSlot, string strResult)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(strResult);
            //return gReqResult(iSlot, strRes);
            
            return gReqResult(iSlot, DeleteNoneASCII(strRes));
        }

        private byte[] DeleteNoneASCII(byte[] bData)
        {
            List<byte> strLst = new List<byte>();

            byte[] strRes = new byte[bData.Length];

            for (int i = 0; i < bData.Length; i++)
            {                
                if (bData[i] >= 0x00 && bData[i] <= 0x20)
                {
                    continue;//
                }   

                if (bData[i] >= 0x7F && bData[i] <= 0xFF)
                {
                    continue;//
                }

                strLst.Add(bData[i]); 
            }

            if(strLst.Count > 0) 
                strRes = strLst.ToArray();            
            
            return strRes;

        }
       

        public bool GMES_GetInsp(int iSlot, string strInspName, ref string strRtnInsp)
        {
            //GMES DLL 1.0.6 에서 아이템 가져올때 null 오는 버그가 있다. retry 3회로 임시 대응한다.

            if (!STEPMANAGER_VALUE.bUseMesOn) { strRtnInsp = ""; return false; }

            for (int i = 0; i < 1; i++)
            {
                strRtnInsp = String.Empty;

                bool bCallType = false;
                if (strInspName.Contains("@"))
                {   //@가 붙으면 공정정보에서 가져오고 없으면 일반 아이템이다.ㅋㅋㅋ
                    strInspName = strInspName.Replace("@", "MODEL/");
                    bCallType = true;
                }

                byte[] strInsp = Encoding.UTF8.GetBytes(strInspName);
                IntPtr iStrPointer = new IntPtr();

                if (bCallType)
                {
                    iStrPointer = gGetSetInfo_(iSlot, strInsp);
                }
                else
                {
                    iStrPointer = gGetSetItem_(iSlot, strInsp);
                }

                string strRes = Marshal.PtrToStringAnsi(iStrPointer);
                strRtnInsp += strRes;
                if (String.IsNullOrEmpty(strRtnInsp))
                {
                    //dll 버그때문에 안읽히면 스텝체크 다시 해보자.
                    System.Threading.Thread.Sleep(5);
                    if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID))
                    {
                        int iReConn = GMES_StepCheck(iSlot, STEPMANAGER_VALUE.strKALS_UPLOAD_WIPID);
                    }
                    
                }
                else
                {
                    return true;
                }                

            }
            return false;
            
        }

        public string GMES_GetVersion()
        {
            string strReturn = String.Empty;
            try
            {
                IntPtr iStrPointerErr = gGetVersion();
                strReturn = Marshal.PtrToStringAnsi(iStrPointerErr);
            }
            catch
            {
            	strReturn = "DLL_ERROR";
            }

            return strReturn;
        }

        public string GMES_GetErrString(int iErrCode)
        {

            string strRtnMsg = String.Empty;

            switch (iErrCode)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 6:
                case 7:
                    IntPtr iStrPointerErr = gGetErrString_(iErrCode);
                    strRtnMsg = Marshal.PtrToStringAnsi(iStrPointerErr);     
                    break;

                case 5:
                    IntPtr iStrPointerReason =  gGetReason_();
                    strRtnMsg = Marshal.PtrToStringAnsi(iStrPointerReason);    
                    break;

                default:
                    strRtnMsg = "UNKNOWN-ERR";
                    break;
            }

            return strRtnMsg;            
        }

        public string GMES_GetEqpName()
        {            
            IntPtr iStrPointer = gGetEqpInfo_(Encoding.UTF8.GetBytes("EQPNAME"));
            return Marshal.PtrToStringAnsi(iStrPointer);
        }

        public string GMES_GetProcName()
        {
            IntPtr iStrPointer = gGetEqpInfo_(Encoding.UTF8.GetBytes("PROCID"));
            return Marshal.PtrToStringAnsi(iStrPointer);
        }

        public string GMES_GetMyWipId(int iSlot)
        {
            IntPtr iStrPointer = gGetSetInfo_(iSlot, Encoding.UTF8.GetBytes("SETID"));
            return Marshal.PtrToStringAnsi(iStrPointer);
        }

        public string GMES_GetWipInfo(int iSlot)
        {
            IntPtr iStrPointer = gGetSetInfo_(iSlot, Encoding.UTF8.GetBytes("PROCID"));
            return Marshal.PtrToStringAnsi(iStrPointer);
        }

    }
}
