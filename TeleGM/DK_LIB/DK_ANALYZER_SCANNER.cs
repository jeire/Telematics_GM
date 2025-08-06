using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{

    class DK_ANALYZER_SCANNER
    {
        
        private const byte DEFINE_ETX1 = 0x0d;
        private const byte DEFINE_ETX2 = 0x0a;

        public DK_ANALYZER_SCANNER() { }
                
        public int AnalyzePacket(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData)
        
        {
            //byte[] tmpBytes = Encoding.UTF8.GetBytes(strData);
                        
            //STX, ETX, OK/NG, CHECKSUM 바이트만 4바이트가 넘으므로 이하면 계속 수신중으로 처리한다.
            if (tmpBytes.Length < 5) 
                return (int)STATUS.RUNNING;


            if (tmpBytes[tmpBytes.Length - 2] == DEFINE_ETX1 && tmpBytes[tmpBytes.Length - 1] == DEFINE_ETX2)
            {// ETX 를 만족하면
                
                rtnData = String.Empty;

                for (int i = 0; i < tmpBytes.Length-2; i++)
                {
                    rtnData += (char)tmpBytes[i];
                }                         
                return (int)STATUS.OK;               
            }
            else
            {
                //for honey well
                if (tmpBytes[tmpBytes.Length - 1] == DEFINE_ETX1)
                {// ETX 를 만족하면

                    rtnData = String.Empty;

                    for (int i = 0; i < tmpBytes.Length - 1; i++)
                    {
                        rtnData += (char)tmpBytes[i];
                    }
                    return (int)STATUS.OK;
                }
            }
            

            return (int)STATUS.RUNNING;
        }

        public byte[] ConvertByteHexString(string strPacket, bool bMode, ref string strLoggingstring) //MOOHANTECH 보드 전용 : bMode 가 true 이면 절차서에서 false면 스위치버튼 사용에서(버퍼링)
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
                        catch (Exception ex) 
                        {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":1:" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg); 
                            tmpArray[j] = 0xFF; 
                        }

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
