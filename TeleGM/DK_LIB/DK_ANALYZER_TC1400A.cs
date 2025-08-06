using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{
    class DK_ANALYZER_TC1400A
    {

        private const byte _ETX = 0x0A;

        private const string HEX_CHARS = "0123456789ABCDEF";

        public DK_ANALYZER_TC1400A()
        {

        }

        public int AnalyzePacket(byte[] bRecvData, ref string rtnData, string strDataType, string strSendPacks)
        {
            string[] strOriginSendPack = System.Text.RegularExpressions.Regex.Split(strSendPacks, " ");
            bool bFind = false;
            int iTailIndex = 0;

            //길이 검사.
            if (bRecvData.Length < 3) return (int)STATUS.RUNNING;

            //ETX 찾기.
            for (int i = 0; i < bRecvData.Length; i++)
            {
                if (i > 1 && bRecvData[i].Equals(_ETX))
                {
                    iTailIndex = i;
                    bFind = true;
                }
            }

            if (!bFind) return (int)STATUS.RUNNING;

            //데이터 형식으로 변환 

            if (bRecvData.Length > 2)
            {
                switch (strDataType)
                {
                    case "ASCII":
                        rtnData = String.Empty;

                        for (int p = 0; p < iTailIndex; p++)
                        {
                            rtnData += (char)bRecvData[p];
                        }
                        return (int)STATUS.OK;

                    case "PARSE1":
                    case "PARSE2":
                    case "PARSE3":
                    case "PARSE4":
                    case "PARSE5":

                        rtnData = String.Empty;

                        for (int p = 0; p < iTailIndex; p++)
                        {
                            rtnData += (char)bRecvData[p];
                        }

                        string[] strSplit = new string[5];
                        strSplit = System.Text.RegularExpressions.Regex.Split(rtnData, ",");

                        if (strSplit.Length != 5)
                        {
                            rtnData = "RESPONSE FORMAT ERROR";
                            return (int)STATUS.NG;
                        }

                        switch (strDataType)
                        {
                            case "PARSE1": rtnData = strSplit[0]; break;
                            case "PARSE2": rtnData = strSplit[1]; break;
                            case "PARSE3": rtnData = strSplit[2]; break;
                            case "PARSE4": rtnData = strSplit[3]; break;
                            case "PARSE5": rtnData = strSplit[4]; break;

                            default:
                                rtnData = "DataType Error";
                                return (int)STATUS.NG;
                        }

                        return (int)STATUS.OK;

                    case "BYTE":
                    default:

                        rtnData = BitConverter.ToString(bRecvData, 0, iTailIndex).Replace("-", "");
                        return (int)STATUS.OK;
                }

            }
            else
            {
                rtnData = "NO DATA";
                return (int)STATUS.NG;
            }


        }

        public byte[] ConvertByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk)
        {
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();

            tmpList.Clear();
            for (int i = 0; i < tmpString.Length; i++)
            {
                tmpList.Add(tmpString[i]);
            }

            for (int i = 0; i < tmpList.Count; i++)
            {

                if (tmpList[i].Equals("<LENGTH>"))
                {
                    int iLen = tmpList.Count - (i + 1);
                    byte[] bLength = new byte[4];
                    bLength = BitConverter.GetBytes(iLen);

                    tmpList[i] = bLength[0].ToString("X2");


                    tmpList.Insert(i, bLength[1].ToString("X2"));
                    tmpList.Insert(i, bLength[2].ToString("X2"));
                    tmpList.Insert(i, bLength[3].ToString("X2"));

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
                    string tmpEx = ex.ToString();
                    rtnValue[i] = (byte)0xFF;
                    brtnOk = false;
                }


            }
            strSendPack = BitConverter.ToString(rtnValue).Replace("-", " ");
            return rtnValue;

        }

        public byte[] HexStringToBytes(string s, ref bool bOk)
        {
            if (s.Length == 0)
            {
                bOk = false;
                return new byte[0];
            }

            //if ((s.Length + 1) % 3 != 0) // 01-00-00-22-23 처럼 이런 - 가 있는 포멧일경우엔 사용.
            if (s.Length % 2 != 0)
            {
                bOk = false;
                return new byte[0];
            }

            byte[] bytes = new byte[(s.Length) / 2];

            int state = 0; // 0 = expect first digit, 1 = expect second digit, 2 = expect hyphen
            int currentByte = 0;
            int x;
            int value = 0;

            foreach (char c in s)
            {
                switch (state)
                {
                    case 0:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                        {
                            bOk = false;
                            return new byte[0];
                        }
                        value = x << 4;
                        state = 1;
                        break;
                    case 1:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                        {
                            bOk = false;
                            return new byte[0];
                        }
                        bytes[currentByte++] = (byte)(value + x);
                        state = 0;
                        break;
                        /*  01-00-00-22-23 처럼 이런 - 가 있는 포멧일경우엔 사용.
                    case 2:
                        if (c != '-')
                            throw new FormatException();
                        state = 0;
                        break;
                         * */
                }
            }
            bOk = true;
            return bytes;
        }


    }
}
