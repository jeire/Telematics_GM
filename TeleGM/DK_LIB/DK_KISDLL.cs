using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
/*
 * CNS 에서 제공한 인증서다운로드 관련 DLL 구현
 * DLL FILE : KISGEN10_DLL.dll
 * 호출함수 정의
 * void __stdcall KIS_KeyData(VBFunction_in *inTag, VBFunction_out *outTag);
 * 구조체 정의
 * struct VBFunction_in
{
	char			*remoteHostName;
	short			remotePort;	
	short			firstBindPort;	
	short			lastBindPort;	
	short			acceptTimeout;	
	short			connectTimeout;	
	char			*stid;			// 8 bytes
	int			    nKeyTypes;
	char            *gentype;
};

struct VBFunction_out
{
	short			error_code;	
	char			*error_message;
    char			*stid;			// 8 bytes
    char			*rCert;			// 1024 bytes
    char			*ccCert;		// 1024 bytes
    char			*vCert;			// 1024 bytes
    char			*vPri;			// 1024 bytes
    char			*vPre;			// 1024 bytes
    char			*vAuth;			// 1024 bytes
	char			*vHash;			// 1024 bytes
};
 *  
*/

namespace GmTelematics
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct VBFunction_in
    {
        public string remoteHostName;
        public short  remotePort;
        public short  firstBindPort;
        public short  lastBindPort;
        public short  acceptTimeout;
        public short  connectTimeout;
        public string stid;			
        public int    nKeyTypes;
        public string gentype;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct VBFunction_out
    {        
        public short  error_code;
        public string error_message;    // 1024 bytes       
        public string stid;			    // 1024 bytes
        public string rCert;			// 1024 bytes
        public string ccCert;		    // 1024 bytes
        public string vCert;			// 1024 bytes
        public string vPri;			    // 1024 bytes
        public string vPre;			    // 1024 bytes
        public string vAuth;			// 1024 bytes
        public string vHash;			// 1024 bytes    

    }

    class DK_KISDLL
    {                                
        private const string strKisDllName = "KISGEN10_DLL.dll";

        public event EventRealTimeMsg KISDLLlRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
        private DK_LOGGER DKQLogger = new DK_LOGGER("SET", false);

        [DllImport(strKisDllName)]
        extern public static void KIS_KeyData(ref VBFunction_in inTag, ref VBFunction_out outTag);


        public DK_KISDLL()
        {
            DKQLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_KISDll);
        }

        private void strReplacing(ref string strDest)
        {
            strDest = strDest.Replace("\n", "");
            strDest = strDest.Replace("\r", "");

        }

        private VBFunction_out SetKeyOutVariable(VBFunction_in vbfInput, ref string strSendp)
        {
            VBFunction_out vbfOutput = new VBFunction_out();
         
            byte[] tempByte = new byte[1024];

            for (int i = 0; i < tempByte.Length; i++)
            {
                tempByte[i] = 0x00;
            }

            vbfOutput.error_code    = 0;
            vbfOutput.error_message = Encoding.UTF8.GetString(tempByte);
            vbfOutput.stid          = Encoding.UTF8.GetString(tempByte);
            vbfOutput.rCert         = Encoding.UTF8.GetString(tempByte);
            vbfOutput.ccCert        = Encoding.UTF8.GetString(tempByte);
            vbfOutput.vCert         = Encoding.UTF8.GetString(tempByte);
            vbfOutput.vPri          = Encoding.UTF8.GetString(tempByte);
            vbfOutput.vPre          = Encoding.UTF8.GetString(tempByte);
            vbfOutput.vAuth         = Encoding.UTF8.GetString(tempByte);
            vbfOutput.vHash         = Encoding.UTF8.GetString(tempByte);

            strSendp =
                    "remoteHostName:"   + vbfInput.remoteHostName +
                    ",remotePort:"      + vbfInput.remotePort.ToString() +
                    ",firstBindPort:"   + vbfInput.firstBindPort.ToString() +
                    ",lastBindPort:"    + vbfInput.lastBindPort.ToString() +
                    ",acceptTimeout:"   + vbfInput.acceptTimeout.ToString() +
                    ",connectTimeout:"  + vbfInput.connectTimeout.ToString() +
                    ",stid:"            + vbfInput.stid +
                    ",nKeyTypes:"       + vbfInput.nKeyTypes.ToString() +
                    ",gentype:"         + vbfInput.gentype;

            SaveLog("", "[TX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + "-" + strSendp);

            return vbfOutput;
        }

        public VBFunction_out KeyDownLoad(ref bool bRes, VBFunction_in vInData1, VBFunction_in vInData2, ref string strSendp, ref string strRecvP, ref string strResult)
        {
            /*
             * 이놈의 DLL은 구조체 참조형이라 해당 사이즈만큼 데이터를 채워 초기화해야한다
             * (String.empty 로 초기화후 보내면 안된다... 이것때문에 이틀고생함...)
             * 1024바이트로 해야하나 LF(\n) 으로 512 바이트 채워 스트링으로 바꾸면 1024바이트가 되서 그런거다.
            */

            VBFunction_out vbfOutput = new VBFunction_out();
            vbfOutput = SetKeyOutVariable(vInData1, ref strSendp);
            
            try
            {
                KIS_KeyData(ref vInData1, ref vbfOutput);

                strReplacing(ref vbfOutput.error_message);
                strReplacing(ref vbfOutput.stid);
                strReplacing(ref vbfOutput.rCert);
                strReplacing(ref vbfOutput.ccCert);
                strReplacing(ref vbfOutput.vCert);
                strReplacing(ref vbfOutput.vPri);
                strReplacing(ref vbfOutput.vPre);
                strReplacing(ref vbfOutput.vAuth);
                strReplacing(ref vbfOutput.vHash);


                strRecvP =
                    "error_code:"       + vbfOutput.error_code.ToString("X4") +
                    ",error_message:"   + vbfOutput.error_message +
                    ",stid:"            + vbfOutput.stid +
                    ",rCert:"           + vbfOutput.rCert +
                    ",ccCert:"          + vbfOutput.ccCert +
                    ",vCert:"           + vbfOutput.vCert +
                    ",vPri:"            + vbfOutput.vPri +
                    ",vPre:"            + vbfOutput.vPre +
                    ",vAuth:"           + vbfOutput.vAuth +
                    ",vHash:"           + vbfOutput.vHash;

                if (vbfOutput.error_code == 0)
                {
                    strResult = "OK";
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Success - " + strRecvP);
                    bRes = true;
                }
                else
                {
                    strResult = GetErrorString(vbfOutput.error_code);
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Failure(" + GetErrorString(vbfOutput.error_code) + ") - " + strRecvP);
                    bRes = false;
                }                    

            }
            catch (Exception ex)
            {
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Exception" + "(" + ex.Message + ")");
                bRes = false;
            }

            if(bRes)
                return vbfOutput;

            vbfOutput = new VBFunction_out();
            vbfOutput = SetKeyOutVariable(vInData2, ref strSendp); 
            
            try
            {
                KIS_KeyData(ref vInData2, ref vbfOutput);

                strReplacing(ref vbfOutput.error_message);
                strReplacing(ref vbfOutput.stid);
                strReplacing(ref vbfOutput.rCert);
                strReplacing(ref vbfOutput.ccCert);
                strReplacing(ref vbfOutput.vCert);
                strReplacing(ref vbfOutput.vPri);
                strReplacing(ref vbfOutput.vPre);
                strReplacing(ref vbfOutput.vAuth);
                strReplacing(ref vbfOutput.vHash);

                strRecvP =
                    "error_code:"       + vbfOutput.error_code.ToString("X4") +
                    ",error_message:"   + vbfOutput.error_message +
                    ",stid:"            + vbfOutput.stid +
                    ",rCert:"           + vbfOutput.rCert +
                    ",ccCert:"          + vbfOutput.ccCert +
                    ",vCert:"           + vbfOutput.vCert +
                    ",vPri:"            + vbfOutput.vPri +
                    ",vPre:"            + vbfOutput.vPre +
                    ",vAuth:"           + vbfOutput.vAuth +
                    ",vHash:"           + vbfOutput.vHash;

                if (vbfOutput.error_code == 0)
                {
                    strResult = "OK";
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Success - " + strRecvP);
                    bRes = true;
                }
                else
                {
                    strResult = GetErrorString(vbfOutput.error_code);
                    SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Failure(" + GetErrorString(vbfOutput.error_code) + ") - " + strRecvP);
                    bRes = false;
                }                

            }
            catch (Exception ex)
            {
                SaveLog("", "[RX] " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":Exception" + "(" + ex.Message + ")");
                bRes = false;
            }
            return vbfOutput;

        }

        private void SaveLog(string strCommandName, string strLog)
        {
            strLog = strLog.Replace("\n", "[CR]");
            if (strCommandName.Length > 0)
            {
                DKQLogger.WriteCommLog(strLog + "-" + strCommandName, "KISDLL", false);
            }
            else
            {
                DKQLogger.WriteCommLog(strLog, "KISDLL", false);
            }
           
        }

        private void GateWay_KISDll(string cParam) //로깅할때 데이터를 다시 실시간으로 manager 로 보내자.
        {
            KISDLLlRealTimeTxRxMsg(0, cParam);
        }

        private string GetErrorString(short sErrorCode)
        {
            string strReturnMsg = String.Empty;

            switch (sErrorCode)
            {
                case (int)0x0000:   strReturnMsg = "SUCCESS";  break;                
                case (int)0x5000:	strReturnMsg = "CIC_API_ERROR_BASE"; break;
                case (int)0x5001:	strReturnMsg = "CIC_ERROR_NULL_PARAMETER"; break;
                case (int)0x5002:	strReturnMsg = "CIC_ERROR_NO_MORE_KEYS"; break;
                case (int)0x5003:	strReturnMsg = "CIC_ERROR_TOO_MANY_LOGS"; break;
                case (int)0x5004:	strReturnMsg = "CIC_ERROR_EXPECTING_TOO_MANY_KEYS"; break;
                case (int)0x5005:	strReturnMsg = "CIC_ERROR_NO_ROOM_IN_KEYSTORE"; break;
                case (int)0x5006:	strReturnMsg = "CIC_FAILURE"; break;
                case (int)0x5007:	strReturnMsg = "CIC_ERROR_SSL"; break; //서버 뒤진것임.
                case (int)0x5008:	strReturnMsg = "CIC_ERROR_2PP"; break;
                case (int)0x5009:	strReturnMsg = "CIC_ERROR_CRYPTO"; break;
                case (int)0x500A:	strReturnMsg = "CIC_ERROR_FILE_IO"; break;
                case (int)0x500B:	strReturnMsg = "CIC_ERROR_INVALID_PARAM"; break;
                default: strReturnMsg = "DLL Unknown Error"; break;               
            }

            return strReturnMsg;
        }
    }
}
