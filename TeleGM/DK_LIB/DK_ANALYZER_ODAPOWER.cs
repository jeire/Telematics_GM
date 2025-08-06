using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    class DK_ANALYZER_ODAPOWER
    {
        public const string DEFINE_ETX = "\n";
        public const byte   BYTE_ETX = 0x0A;

        public DK_ANALYZER_ODAPOWER() { }
        
        public int AnalyzePacket(byte[] strData, ref string rtnData)
        {           

            if(strData.Length < 1) return (int)STATUS.RUNNING;

            string tmpLogStr = Encoding.ASCII.GetString(strData);
            int iEtx = tmpLogStr.IndexOf(DEFINE_ETX);
            bool bEtx = false;
            int bPos = 0;
            if (strData[strData.Length - 1] == BYTE_ETX)
            {
                bEtx = true;
                bPos = strData.Length - 1;
            }

            rtnData = String.Empty;

            //수신 데이터가 128바이트 이상이면 NG 처리.
            if (strData.Length > 128)
            {
                //NG 처리.
                return (int)STATUS.NG;

            }

            if (strData.Length > 1 && !bEtx)
            {
                //아니면 계속 수신중으로 처리
                return (int)STATUS.RUNNING;
            }

            //수신 데이터가 여러개 있을경우 맨 앞에것만 처리하고 버린다.
            if (tmpLogStr.Length > 1 && bEtx) 
            {
                rtnData = tmpLogStr;
                rtnData = rtnData.Replace(DEFINE_ETX, String.Empty); //LINE FEED 제거
                rtnData = rtnData.TrimEnd('\0'); //공백 제거
                return (int)STATUS.OK;               

            }
            else
            {
              
                //아니면 계속 수신중으로 처리
                return (int)STATUS.RUNNING;
            }
            
            
        }

         
    }
}
