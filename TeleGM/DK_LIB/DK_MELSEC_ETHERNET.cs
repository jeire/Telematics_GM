using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GmTelematics
{

    class DK_MELSEC_ETHERNET
    {
        public event EventRealTimeMsg MelsecRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드

        private ActUtlTypeLib.ActUtlType AUTlib;
        private bool bConnected;
        private DK_LOGGER DKLogger;

        public DK_MELSEC_ETHERNET()
        {
            AUTlib = new ActUtlTypeLib.ActUtlType();
            bConnected = false;
            DKLogger = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_MELSEC);
        }

        private void SaveLog(string strLog, string strCommandName)
        {
            strLog = strLog.Replace("\n", "");
            DKLogger.WriteCommLog(strLog, "MELSEC" + ":" + strCommandName, false);
            //20241008 MELSEC LOG 추가
            DKLogger.WriteMelsecLog(strLog, "MELSEC" + ":" + strCommandName, false);

        }

        private void GateWay_MELSEC(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
        {
            MelsecRealTimeTxRxMsg(0, cParam);
        }

        public bool Connect(ref string strResult)
        {
            try
            {
                AUTlib.ActLogicalStationNumber = 0;

                SaveLog("[TX]" + "ETHERNET_CONNECTION", "CONNECT");
                int iOpen = AUTlib.Open();

                bConnected = (iOpen.Equals(0) || iOpen.Equals(-268435453));//Already open;
                if (bConnected)
                {
                    strResult = "SUCCESS";
                    SaveLog("[RX]" + "SUCCESS", "CONNECT");
                }
                else
                {
                    strResult = "FAIL"; //25199627 : 타임아웃
                    SaveLog("[RX]" + "ERROR CODE :" + iOpen.ToString(), "CONNECT");
                }
                return bConnected;
            }
            catch (Exception ex)
            {
                strResult = "ERROR";
                SaveLog("[RX]" + "ERROR - " + ex.Message, "CONNECT");
                bConnected = false;
                return false;
            }

        }

        public void Disconnect()
        {
            SaveLog("[TX]" + "ETHERNET_DISCONNECTION", "DISCONNECT");
            if (!bConnected)
            {
                SaveLog("[RX]" + "ALREADY DISCCONNETED", "DISCONNECT");
                return;
            }
            try
            {
                bConnected = false;
                AUTlib.Disconnect();
                AUTlib.Close();
                SaveLog("[RX]" + "DISCCONNETED", "DISCONNECT");
            }
            catch
            {
                SaveLog("[RX]" + "EX DISCCONNETED", "DISCONNECT");
            }
        }

        public bool ReadMemory(string strCommandName, string strParam, int iType, ref string strResponse)
        {
            SaveLog("[TX]" + strParam, strCommandName);

            // 커넥션 체크
            if (!bConnected)
            {
                strResponse = "MELSEC DISCONNETED";
                SaveLog("[RX]" + strResponse, strCommandName);
                return false;
            }

            //파라미터 검사할것.
            int iSize = 0;   //읽어올 사이즈
            int iLength = 0;  //실제 메모리에서 읽어올 사이즈 (melsec어드레스당 2바이트단위로 들어있기때문에 반으로 나눠야한다)
            int iDevResult = -1;  //리턴 결과
            string strAddress = String.Empty; //메모리 어드레스: 현재는 "D" 어드레스만 강제사용
            int iAddressCounter = 0;  //숫자인지도 검사하자.
            byte[] bytesBuffer = new byte[1];


            string[] strParams = System.Text.RegularExpressions.Regex.Split(strParam, ",");
            if (strParams.Length != 2)
            {
                strResponse = "Parameter Error";
                SaveLog("[RX]" + strResponse + " - " + strParam, strCommandName);
                return false;
            }

            try
            {
                iAddressCounter = int.Parse(strParams[0]);
                iSize = int.Parse(strParams[1]);

                if (iSize < 1)
                {
                    strResponse = "Read Size Range Under";
                    SaveLog("[RX]" + strResponse + " - " + strParam, strCommandName);
                    return false;
                }

                int iEvenOdd = iSize % 2;
                if (iEvenOdd.Equals(0))
                    //0이면 짝수
                    iLength = iSize / 2;
                else
                    //1이면 홀수
                    iLength = (iSize / 2) + 1;
            }
            catch
            {
                strResponse = "Parameter Error";
                SaveLog("[RX]" + strResponse + " - " + strParam, strCommandName);
                return false;
            }

            //수행


            int[] iArrayData = new int[iLength];

            switch (iLength)
            {
                case 1: //1개인 경우는 아래 사용
                    strAddress = "D" + iAddressCounter.ToString();
                    iDevResult = AUTlib.ReadDeviceBlock(strAddress, iLength, out iArrayData[0]);
                    break;
                default:
                    // 여러개 읽는 것은 아래 사용
                    // 

                    for (int i = 0; i < iArrayData.Length; i++)
                    {
                        strAddress += "D" + (iAddressCounter + i).ToString();
                        if (i < iArrayData.Length - 1) strAddress += "\n";
                    }
                    try
                    {
                        iDevResult = AUTlib.ReadDeviceRandom(strAddress, iLength, out iArrayData[0]);
                    }
                    catch (Exception ex)
                    {
                        strResponse = "ReadDeviceRandom Exception";
                        SaveLog("[RX]" + strResponse + ". exception : " + ex.Message, strCommandName);
                        return false;
                    }
                    break;
            }


            // 장비 실패시 리턴
            if (!iDevResult.Equals(0))
            {
                strResponse = "MEMORY READ FAIL";
                SaveLog("[RX]" + strResponse + ". ERROR CODE : " + iDevResult.ToString(), strCommandName);
                return false;

            }
            else
            {
                //성공시 데이터 전부 로그 찍자.대신 로깅은 2바이트단위로만, 왜냐면 한 어드레스당 2바이트이라고 한다.
                string strLogMsg = String.Empty;
                string strBuffers = String.Empty;

                string strHigh = String.Empty;
                string strLow = String.Empty;

                for (int i = 0; i < iArrayData.Length; i++)
                {
                    strLogMsg = strLogMsg + iArrayData[i].ToString("X4");
                    strHigh = iArrayData[i].ToString("X4").Substring(0, 2);
                    strLow = iArrayData[i].ToString("X4").Substring(2, 2);
                    strBuffers = strBuffers + strLow + strHigh;
                    if (i < iArrayData.Length - 1) strLogMsg += " ";
                }

                SaveLog("[RX]" + strLogMsg, strCommandName);

                //리턴에 사용할 데이터 정렬
                bool bConvertBytes = false;
                bytesBuffer = HexStringToBytes(strBuffers, ref bConvertBytes);

                if (!bConvertBytes)
                {
                    strResponse = "PARSING ERROR";
                    SaveLog("[RX]" + strResponse, strCommandName);
                    return false;
                }

                if (iSize > bytesBuffer.Length)
                {
                    strResponse = "Size Range Over";
                    SaveLog("[RX]" + strResponse, strCommandName);
                    return false;
                }

            }

            try
            {
                switch (iType)
                {
                    case (int)MELSECRESTYPE.WORD: //ascii
                        strResponse = System.Text.Encoding.UTF8.GetString(bytesBuffer, 0, iSize);
                        break;
                    case (int)MELSECRESTYPE.BYTE: //bytes
                        strResponse = BitConverter.ToString(bytesBuffer, 0, iSize).Replace("-", "");
                        break;

                    default:
                        strResponse = "RETURN TYPE ERROR";
                        SaveLog("[RX]" + strResponse, strCommandName);
                        return false;
                }
            }
            catch
            {
                strResponse = "Function Error.";
                SaveLog("[RX]" + strResponse, strCommandName);
                return false;
            }


            return true;
        }

        public bool WriteMemory(string strCommandName, ref string strResponse)
        {
            SaveLog("[TX]" + "", strCommandName);
            if (!bConnected)
            {
                strResponse = "MELSEC DISCONNETED";
                SaveLog("[RX]" + strResponse, strCommandName);
                return false;
            }

            //아직 구현할 일이 없다.
            return false;
        }

        private byte[] HexStringToBytes(string s, ref bool bOk)
        {
            const string HEX_CHARS = "0123456789ABCDEF";

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
