using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{

    class DK_ANALYZER_DIO_VCP
    {       

        private const byte DEFINE_STX = 0x24;
        private const byte DEFINE_ETX = 0x0D;
        private const byte DEFINE_O = 0x4F;
        private const byte DEFINE_K = 0x4B;
        private const byte DEFINE_N = 0x4E;
        private const byte DEFINE_G = 0x47;

        public DK_ANALYZER_DIO_VCP() { }
        //bSample.ToArray()
        public int AnalyzePacket(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName)
        //public int AnalyzePacket(List<byte> tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData)
        {
            //byte[] tmpBytes = Encoding.UTF8.GetBytes(strData);
                        
            //STX, ETX, OK/NG, CHECKSUM 바이트만 4바이트가 넘으므로 이하면 계속 수신중으로 처리한다.
            if (tmpBytes.Length < 7) 
                return (int)STATUS.RUNNING;

            //STX , ETX , CHECKSUM 확인
            int iEndNum = tmpBytes.Length - 1; //ETX 자리
            int iChkNum = tmpBytes.Length - 3; //CHECKSUM 자리
            int iResNum = tmpBytes.Length - 5; //OK or NG 자리

            if (iEndNum < 1 || iChkNum < 1 || iResNum < 1) 
                return (int)STATUS.RUNNING;
            

            if (tmpBytes[0] == DEFINE_STX && tmpBytes[iEndNum] == DEFINE_ETX)
            {//STX 와 ETX 를 만족하면

                //체크섬 검사.
                
                DK_CHECKSUM DKCHK = new DK_CHECKSUM();
                byte bChkHigh = 0x00;
                byte bChkLow = 0x00;
                byte[] bChkBytes = new byte[tmpBytes.Length - 3];
                Array.Copy(tmpBytes, bChkBytes, tmpBytes.Length - 3);
                DKCHK.XOR_High_Low(bChkBytes, ref bChkHigh, ref bChkLow);

                if (tmpBytes[iChkNum] != bChkHigh || tmpBytes[iChkNum + 1] != bChkLow)
                {
                    rtnData = "CHECKSUM NG";
                    return (int)STATUS.NG;
                }
               

                //보낸명령의 응답인지 체크   
                //if (tmpBytes[3] != strOrginSendData[3]) return (int)STATUS.NG;
                int iDx = 0;
                int iEx = 0;

                if (strOrginSendData.Length < 8)
                {
                    iDx = 3; iEx = 4;
                }
                else
                {
                    iDx = 3; iEx = 5;
                }

                for (int i = iDx; i < iEx; i++)
                {                    
                    if (tmpBytes[i] != strOrginSendData[i])
                    {
                        return (int)STATUS.NG;
                    }
                }                

                rtnData = String.Empty;

                if (strOrginSendData.Length < 8)
                {
                    iDx = 4; 
                }
                else
                {
                    iDx = 6;
                }

                if (iResNum > 2 && tmpBytes.Length > 9)
                {
                    for (int i = iDx; i < iResNum; i++)
                    {
                        rtnData += (char)tmpBytes[i];
                    }

                    rtnData = AnalyzeResultData(rtnData, strCommandName);
                }
         
                
                strCommand = ((char)tmpBytes[3]).ToString();

                //응답 결과가 NG 면 NG 를 리턴
                if (tmpBytes[iResNum] == DEFINE_N && tmpBytes[iResNum + 1] == DEFINE_G) return (int)STATUS.NG;
                //응답 결과가 OK 면 OK 를 리턴
                if (tmpBytes[iResNum] == DEFINE_O && tmpBytes[iResNum + 1] == DEFINE_K) return (int)STATUS.OK;
               
            }

            if (tmpBytes[0] != DEFINE_STX) return (int)STATUS.NG;

            else return (int)STATUS.RUNNING;
        }

        private string AnalyzeResultData(string rtnData, string strCommandName)
        {
            string tmpStr = rtnData;

            tmpStr = rtnData.Trim();
            tmpStr = tmpStr.Replace("[", "");
            tmpStr = tmpStr.Replace("]", "");

            /*
            switch (strCommandName)
            {
                case "READ_CURRENT_PRIMARY_mA":
                case "READ_CURRENT_BACKUP_mA":
                    if()
                default: break;
            }
            */
            return tmpStr;
        }

        public byte[] ConvertByteHexString(string strPacket, bool bMode, ref string strLoggingstring) 
        
            //MOOHANTECH 보드 전용 : bMode 가 true 이면 절차서에서 false면 스위치버튼 사용에서(버퍼링)
        {

            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();

            tmpList.Clear();
            for (int i = 0; i < tmpString.Length; i++)
            {//데이터를 돌면서 체크섬의 위치를 찾는다
                if (tmpString[i].Equals("<CXA>"))
                {
                    byte[] tmpArray = new byte[i];

                    for (int j = 0; j < i/*tmpArray.Length*/; j++)
                    {//체크섬위치를 찾으면 체크섬 앞의 위치까지를 재정렬한다.
                        try { tmpArray[j] = Convert.ToByte(tmpString[j], 16); }
                        catch (Exception ex) {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":1:" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg); 
                            tmpArray[j] = 0xFF; }

                    }

                    DK_CHECKSUM DKCHK = new DK_CHECKSUM(); //체크섬!
                    byte bHighByte = 0x00;
                    byte bLowByte = 0x00;
                    DKCHK.XOR_High_Low(tmpArray, ref bHighByte, ref bLowByte);
                    tmpList.Add(bHighByte.ToString("X2")); //체크섬 상위바이트 넣기
                    tmpList.Add(bLowByte.ToString("X2"));  //체크섬 하위바이트 넣기

                }
                else
                {
                    tmpList.Add(tmpString[i]);
                }
            }

            byte[] rtnValue = new byte[tmpList.Count];

            for (int i = 0; i < tmpList.Count; i++)
            {
                try
                {
                    rtnValue[i] = Convert.ToByte(tmpList[i], 16);
                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":2:" + ex.Message;
                    STEPMANAGER_VALUE.DebugView(strExMsg); 
                    rtnValue[i] = (byte)0xFF;
                }               
            }
            //BYTE 단위로 기록하기
            //string tmpLogString = BitConverter.ToString(rtnValue).Replace("-", " ");
            //ASCII 단위로 기록하기
            //if (bMode) strLoggingstring = Encoding.UTF8.GetString(rtnValue);
            strLoggingstring = BitConverter.ToString(rtnValue).Replace("-", " ");
       
                
            return rtnValue;
        }

        
    }
}
