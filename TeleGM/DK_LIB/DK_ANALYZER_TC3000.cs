using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    class DK_ANALYZER_TC3000
    {
        //TC3000 에서만 쓰이는 상수선언        
        public const byte ETX1 = 0x0D;
        public const byte ETX2 = 0x0A;                

        public DK_ANALYZER_TC3000() { }


        public int AnalyzePacket(byte[] strData, ref string rtnData, ref string rtnLogString)
        {   
            
            rtnData = String.Empty;

            //수신 데이터가 있으나 ETX 가 아니면 RUNNING 처리.
            if (strData.Length > 2 && strData[strData.Length-1] != ETX2 && strData[strData.Length-2] != ETX1)
            {
                return (int)STATUS.RUNNING;
            }
            else
            {
                for (int i = 0; i < strData.Length - 2; i++)
                {
                    if (strData[i] < 0x20 || strData[i] > 0x7E) //일반적인 문자가 아니면 데이터가 깨진것으로 간주하여 NG 리턴.
                    {
                       
                            rtnData = "Non ASCII Characters";
                            rtnLogString = "Non ASCII characters : " + BitConverter.ToString(strData).Replace("-", " ");
                            return (int)STATUS.NG;
                                             
                    }
                }
                try
                {
                    rtnLogString = System.Text.Encoding.UTF8.GetString(strData) + "( " + BitConverter.ToString(strData).Replace("-", " ") + " )";
                    rtnLogString = System.Text.Encoding.UTF8.GetString(strData) + "( " + BitConverter.ToString(strData).Replace("-", " ") + " )";
                    rtnData = Encoding.UTF8.GetString(strData).ToUpper();
                    rtnData = rtnData.Replace("\r", String.Empty);
                    rtnData = rtnData.Replace("\n", String.Empty);
                    return (int)STATUS.OK;
                }
                catch 
                {
                    rtnData = "Non ASCII Characters!";
                    rtnLogString = "Non ASCII characters! : " + BitConverter.ToString(strData).Replace("-", " ");
                    return (int)STATUS.NG;
                }
                
            }

           
            
        }


         
    }
}
