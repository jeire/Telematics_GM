using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    class DK_ANALYZER_GEN9DLL
    {
        private const byte _STX = 0x02;
        private const byte _ETX = 0x03;

        public DK_ANALYZER_GEN9DLL()
        {
            
        }

        public int AnalyzePacket(byte[] strData, ref string rtnData)
        {

            //길이 검사.
            if (strData.Length < 6) return (int)STATUS.RUNNING;

            //STX 검사.
            if (!strData[0].Equals(_STX)) return (int)STATUS.RUNNING;

            //ETX 검사.
            if (!strData[strData.Length-1].Equals(_ETX)) return (int)STATUS.RUNNING;

            //형식 검사.
            string strBuffer = Encoding.UTF8.GetString(strData, 1, strData.Length - 2);
            string[] strParse = System.Text.RegularExpressions.Regex.Split(strBuffer, ",");

            if(strParse.Length != 2)
                return (int)STATUS.NG;

            int iResult = (int)STATUS.OK;

            switch (strParse[0])
            {
                case "OK": iResult = (int)STATUS.OK; break;
                case "NG": iResult = (int)STATUS.NG; break;
                case "CHECK": iResult = (int)STATUS.CHECK; break;

                case "ERROR":
                default: iResult = (int)STATUS.ERROR; break;
            }
            rtnData = strParse[1];
            return iResult;
        }


    }
    
}
