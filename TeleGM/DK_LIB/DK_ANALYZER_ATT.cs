using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    class DK_ANALYZER_ATT
    {        
        private const byte DEFRES_STX1 = 0x02;
        private const byte DEFRES_STX2 = 0xFB;
        private const byte DEFRES_ETX  = 0xFA;

        private const string HEX_CHARS = "0123456789ABCDEF";

        private byte[] TCP_HB;

        private List<TBLDATA0> LstTBLtcp = new List<TBLDATA0>();       // 명령 테이블 리스트
        private DK_LOGGER tmpLogger = new DK_LOGGER("PC", false);

        public DK_ANALYZER_ATT()
        {            
            tmpLogger.LoadTBL0("ATT_TCP.TBL", ref LstTBLtcp);
            TCP_HB = new byte[5]{0x32, 0x52, 0x44, 0x31, 0x66};
        }

        public PACKTYPE GetTcpResFormat(List<TBLDATA0> lstTBLDB, string strCommandName, ref bool bFind)
        {
            PACKTYPE iResult = new PACKTYPE();
            iResult.iType = (int)TCPRESTYPE.NONE;
            iResult.iPos = -1;
            bFind = false;
            try
            {
                for (int i = 0; i < lstTBLDB.Count; i++)
                {
                    if (lstTBLDB[i].CMDNAME.Equals(strCommandName))
                    {                        
                        switch (lstTBLDB[i].RECVPAC)
                        {
                            case "BYTE":       iResult.iType = (int)TCPRESTYPE.BYTE;   break;
                            case "BYTE1":      iResult.iType = (int)TCPRESTYPE.BYTE1;  break;
                            case "CHAR":       iResult.iType = (int)TCPRESTYPE.CHAR;   break;
                            case "INT":        iResult.iType = (int)TCPRESTYPE.INT;    break;                            
                            case "DTC_TABLE":  iResult.iType = (int)TCPRESTYPE.DTC;    break;                           
                            case "DTC_ALL":    iResult.iType = (int)TCPRESTYPE.DTCALL; break;
                            case "ALDL_ASCII": iResult.iType = (int)TCPRESTYPE.ALDL_ASCII;
                                               STEPMANAGER_VALUE.bOldBinaryALDL = new byte[1]; //초기화해줌.
                                               STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2];
                                               break;
                            case "ALDL_BITS":  iResult.iType = (int)TCPRESTYPE.ALDL_BITS; break;
                            case "DOUBLE":     iResult.iType = (int)TCPRESTYPE.DOUBLE; break;
                            case "SINGLE":     iResult.iType = (int)TCPRESTYPE.SINGLE; break;
                            case "TTFF":       iResult.iType = (int)TCPRESTYPE.TTFF; break;
                            case "SIMINFO":    iResult.iType = (int)TCPRESTYPE.SIMINFO; break;
                            case "NADINFO":    iResult.iType = (int)TCPRESTYPE.NADINFO; break;
                            case "REVERSE":    iResult.iType = (int)TCPRESTYPE.REVERSE; break;
                            case "OOBRESULT":  iResult.iType = (int)TCPRESTYPE.OOBRESULT; break;
                            case "SERVICE":    iResult.iType = (int)TCPRESTYPE.SERVICE; break;
                            case "APN_TABLE":  iResult.iType = (int)TCPRESTYPE.APN_TABLE; break;
                            default:           iResult.iType = (int)TCPRESTYPE.NONE;   break;
                        }

                        if (lstTBLDB[i].PARPAC1.Length > 1 && lstTBLDB[i].PARPAC1.Contains("POS")) 
                        {
                            string tmpString = lstTBLDB[i].PARPAC1.Replace("POS:", String.Empty);

                            iResult.iPos = int.Parse(tmpString);

                        }
                      
                        bFind = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                tmpLogger.WriteCommLog("Exception: GetTcpResFormat :", ex.Message, false);                
                bFind = false;
            }  

            return iResult;
        }
   
        private bool CheckATT_DtcIndex(byte bSendParam, byte[] bGetData, ref string strRetunData)
        {            
            try
            {
                // 받은 데이터 길이 체크
                if (bGetData.Length != 3) return false;

                //여기서는 요청한 DTC 항목이  응답받은 DTC 항목이 맞는지 체크한다.
                byte bBlockIndex = bSendParam;


                if (bBlockIndex != bGetData[1])
                {
                    strRetunData = "NO DTC BLOCK";
                    return false;
                }

                strRetunData = String.Empty;
                
                //8비트로 나타내는 방법 (DQA 담당자 요청방식)
                //strRetunData = Convert.ToString(bGetData[3], 2).PadLeft(8, '0');

                //2번째 비트만 나타내는 방법 (현우씨 요청방식)
                //strRetunData = strRetunData.Substring(6, 1);

                //바이트만 그대로 나타내는 방법 (GEN10과 동일방식 - 20160229)
                strRetunData = bGetData[2].ToString("X2");

                return true;
            }
            catch
            {
                strRetunData = "CheckTCP_DtcIndex:Exception";
                return false;
            }            
            
        }

        private bool CheckATT_ALDLType(byte[] bSendPack, byte[] bGetData, ref string strRetunData, ref byte[] bReadBinary)
        {
            try
            {   //김훈겸책임과 확인결과 옛날 구조체라고 함... TCP 공장이미지와 다름. 따라서 메모리 블럭 확인 불가...
                // 1. unsigned short len;       1byte
                ////////// 2. unsigned char  lflag;        4byte
                ////////// 3. unsigned char  data [150];  150byte
                
                if (bGetData.Length != 155) return false;

                string strData1 = String.Empty;
                string strData2 = String.Empty;
                string strData3 = String.Empty;
                //string strData4 = String.Empty;                                

                byte[] bData1 = new byte[1];
                byte[] bDummy = new byte[4];
                //byte[] bData2 = new byte[2];
                byte[] bData3 = new byte[150];
                //byte[] bData4 = new byte[150];
                

                Array.Copy(bGetData, 0, bData1, 0, bData1.Length);
                Array.Copy(bGetData, 1, bDummy, 0, bDummy.Length);
                //Array.Copy(bGetData, 5, bData2, 0, bData2.Length);                
                Array.Copy(bGetData, 5, bData3, 0, bData3.Length);
                //Array.Copy(bGetData, 153, bData4, 0, bData4.Length);
                bReadBinary = new byte[150];
                Array.Copy(bData3, 0, bReadBinary, 0, bReadBinary.Length);

                //1. ALDL 블럭체크 해야하나 프로토콜 문서 자료가 이상함... ㅡㅡ 그래서 bypass
                /*
                if (bSendPack[12] != bData1[0] || bSendPack[13] != bData1[1])
                {
                    return false;
                }                
                
                strData1 = BitConverter.ToString(bData1).Replace("-", "");
                strData2 = bData2.ToString();
                */

                int iDataLen = 0;
                try
                {
                    iDataLen = (int)bData1[0];                    
                }
                catch 
                {
                    strRetunData = "CheckTCP_ALDLType:Exception - DATA TYPE ERROR";
                    return false;
                }  

                //여기서 아스키로 변환해야하는지 한바이트만 체크하자.
                bool bIsAscii = CheckASCII(iDataLen, bData3);
                     
                if (bIsAscii)
                {
                    strData3 = Encoding.UTF8.GetString(bData3);
                }
                else
                {
                    strData3 = BitConverter.ToString(bData3, 0, (int)iDataLen).Replace("-", "");
                }                  

                strRetunData = strData3;
                strRetunData = strRetunData.Replace("\0", String.Empty);
                return true;
            }
            catch
            {
                strRetunData = "CheckTCP_ALDLType:Exception";
                return false;
            }

        }
        
        private bool CheckATT_SimInfoType(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
        {   
            try
            {
                byte[] bParseData = new byte[bGetData.Length - 3]; //전체데이터 복사본
                byte[] bLenth = new byte[2];                   //길이 재는 용도
                byte[] bBuffer;                                    //임시 버퍼 용도
                STEPMANAGER_VALUE.OOBSimInfoClear();               //데이터 초기화.


                Array.Copy(bGetData, 3, bParseData, 0, bParseData.Length); //데이터에서 앞에 3바이트를 빼고 시작한다. 연구소도 인원이 교체되서 문서에도 없다. 문서와 구조체가 다르다. 난감하다.
                                                                            //[0]이 OK NG [1] [2] 가 길이인듯.
                int iDx = 0;
                for (int i = (int)SimInfoIndex.efUST; i <= (int)SimInfoIndex.QualcommChipRev; i++)
                {
                    try
                    {
                        Array.Copy(bParseData, iDx, bLenth, 0, bLenth.Length);
                        ushort iLenth = BitConverter.ToUInt16(bLenth, 0);
                        bBuffer = new byte[iLenth];
                        Array.Copy(bParseData, iDx + 4, bBuffer, 0, bBuffer.Length);
                        iDx += (iLenth + 4);

                        //*// 여기서 아스키로 변환해야하는지 한바이트만 체크하자//*// 
                        List<byte> lBytes = new List<byte>();
                        lBytes.Clear();
                        for (int j = 0; j < bBuffer.Length; j++)
                        {

                            if (bBuffer[j] != 0xFF)
                            {
                                lBytes.Add(bBuffer[j]);
                            }
                            else
                            {
                                if (i == (int)SimInfoIndex.eSimVer)
                                {
                                    if (lBytes[lBytes.Count - 1] != 0x7C)
                                        lBytes.Add(0x7C);
                                }
                            }

                        }
                        if (lBytes.Count > 0)
                        {
                            bool bIsAscii = CheckASCII(lBytes.Count, lBytes.ToArray());

                            if (bIsAscii)
                            {
                                STEPMANAGER_VALUE.OOBSimInfo[i] = Encoding.UTF8.GetString(lBytes.ToArray());

                                if (i == (int)SimInfoIndex.eSimVer)
                                {
                                    string[] strSimver = STEPMANAGER_VALUE.OOBSimInfo[i].Split('|');
                                    if (strSimver.Length == 4)
                                    {
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_PROFILE] = strSimver[0];
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_NSPIF]   = strSimver[1];
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_VVN]     = strSimver[2];
                                    }
                                }
                            }
                            else
                            {
                                STEPMANAGER_VALUE.OOBSimInfo[i] = BitConverter.ToString(lBytes.ToArray(), 0, lBytes.Count).Replace("-", "").ToUpper();
                                if (i == (int)SimInfoIndex.eSimVer)
                                {
                                    string[] strSimver = System.Text.RegularExpressions.Regex.Split(STEPMANAGER_VALUE.OOBSimInfo[i], "7C");
                                    if (strSimver.Length == 4)
                                    {
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_PROFILE] = strSimver[0];
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_NSPIF] = strSimver[1];
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_VVN] = strSimver[2];
                                    }
                                }

                            }
                        }
                        else
                        {
                            STEPMANAGER_VALUE.OOBSimInfo[i] = "NO DATA";
                        }

                    }
                    catch
                    {
                        strRetunData = "CheckATT_SimInfoType:Exception1";
                        return false;
                    }

                }

                return true;
            }
            catch 
            {
                strRetunData = "CheckATT_SimInfoType:Exception2";
                return false;
            }
            

        }

        private bool CheckATT_ReverseType(string strSendCommand, byte[] bGetData, ref string strRetunData)
        {
            strRetunData = String.Empty;
            switch (strSendCommand)
            {
                case "READ_IMSI":

                    if (bGetData.Length > 3 && bGetData[0] == 0x01 )
                    {
                        int iDataSize = (int)BitConverter.ToInt16(bGetData, 1);
                        if (bGetData.Length == iDataSize + 3)
                        {
                            for (int i = 3; i < bGetData.Length; i++)
                            {
                                strRetunData += new string(bGetData[i].ToString("X2").Reverse().ToArray());

                            }
                            strRetunData = strRetunData.Substring(3);
                            return true;
                        }
                    }
                    return false;

                default: return false;

            }
        }

        private byte[] MakeATT_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {
            byte[] bReturnBytes = new byte[305];
            try
            {
                // 1. unsigned short block;         2byte
                // 2. unsigned char  len;           1byte
                // 3. unsigned char  data[150];   150byte
                // 4. unsigned char  lflag[150];  150byte

                byte[] bDataBlock = new byte[1];
                byte[] bDummy     = new byte[4];
                byte[] bDataData = new byte[150];
                byte[] bDataFlag = new byte[150];

                byte[] bTempBlock;
                byte[] bTempData;

                bool bConvOK1 = false;
                bool bConvOK2 = false;


                string[] strParseData = strParam.Split(',');

                if (strParseData.Length != 2)
                {
                    strReason = "PAR1 INVALID : " + strParam;
                    bRes = false;
                    return bReturnBytes;
                }

                if (STEPMANAGER_VALUE.bOldBinaryALDL.Length != bDataData.Length)
                {
                    strReason = "NOT FOUND ALDL OLD DATA";
                    bRes = false;
                    return bReturnBytes;
                }

                bTempBlock = HexStringToBytes(strParseData[0], ref bConvOK1);

                if (bAsciiType)// TRACE CODE 같은것들은 아스키그대로 써야한단다...
                {
                    bTempData = Encoding.UTF8.GetBytes(strParseData[1]);
                    bConvOK2 = true;
                }
                else
                {
                    bTempData = HexStringToBytes(strParseData[1], ref bConvOK2);
                }

                //bDataLen[0] = (byte)bTempData.Length;

                if (bConvOK1 && bConvOK2)
                {
                    Array.Copy(bTempBlock, 0, bDataBlock, 0, bTempBlock.Length);
                    Array.Copy(bTempData,  0, bDataData,  0, bTempData.Length);

                    
                    for (int i = 0; i < bDataFlag.Length; i++)
                    {
                        bDataFlag[i] = (byte)(bDataData[i] ^ STEPMANAGER_VALUE.bOldBinaryALDL[i]);
                    }

                    bDataData = new byte[150]; //데이터 재초기화.

                    int j = 0;
                    for (int i = 0; i < bDataFlag.Length; i++)
                    {
                        if (bDataFlag[i] != 0x00)
                        {
                            if (i <= bTempData.Length)
                            {
                                bDataData[j] = bTempData[i];
                                j++;
                            }
                        }
                    }

                }
                else
                {
                    strReason = "PAR1 INVALID1 : " + strParam;
                    bRes = false;
                    return bReturnBytes;
                }

                Array.Copy(bDataBlock, 0, bReturnBytes,   0, bDataBlock.Length);
                Array.Copy(bDummy,     0, bReturnBytes,   1, bDummy.Length);
                Array.Copy(bDataData,  0, bReturnBytes,   5, bDataData.Length);
                Array.Copy(bDataFlag,  0, bReturnBytes, 155, bDataFlag.Length);

                bRes = true;
                return bReturnBytes;
            }
            catch
            {
                strReason = "MakeATT_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
            }

        }

        private void CheckExceptionCommands(string strCommandName, ref byte[] strOrginSendData)
        {
            //TCP ATT 프로토콜 거지같아서 이런걸 만듬.... 보낸명령어랑 응답 명령이 달라야하는 경우를 위해...
            byte[] bCommand = new byte[5];
            switch (strCommandName)
            {
                
                case "ALDL_WRITE":
                case "ALDL_WRITE_ASCII":
                case "ALDL_TOGGLE_ON":
                case "ALDL_TOGGLE_OFF":
                        bCommand[0] = 0x33; bCommand[1] = 0x43; bCommand[2] = 0x50; bCommand[3] = 0x31; bCommand[4] = 0x66;
                        Array.Copy(bCommand, 0, strOrginSendData, 4, bCommand.Length);
                        break;
                case "OOB_SELF_TEST":
                        bCommand[0] = 0x46; bCommand[1] = 0x49; bCommand[2] = 0x7A; bCommand[3] = 0x32; bCommand[4] = 0x66;
                        Array.Copy(bCommand, 0, strOrginSendData, 4, bCommand.Length);
                        break;         
                default:                        
                        break;
            }

        }
        
        public int CheckProtocol(byte[] tmpByteArray, byte[] strOrginSendData, string strCommandName, ref string rtnData)
        {            
            //체크섬 검사.

            DK_CHECKSUM DKCHK = new DK_CHECKSUM();
            byte bChkHigh = 0x00;
            byte bChkLow  = 0x00;
            DKCHK.Tcp_chksum(tmpByteArray, ref bChkHigh, ref bChkLow, false);
            
            if (tmpByteArray[tmpByteArray.Length - 3] != bChkHigh ||
                tmpByteArray[tmpByteArray.Length - 2] != bChkLow)
            {
                rtnData = "CHECKSUM NG";
                return (int)STATUS.NG;
            }
                   
            try
            {   
                
                //보낸명령어에 대한 응답데이터인지 한번 더 확인

                for (int i = 4; i < 9; i++)
                {
                    if (tmpByteArray[i + 2] != strOrginSendData[i])
                    {
                        return (int)STATUS.RUNNING;
                    }
                   
                }
                //데이터 수집 구간 시작------------------------------------------
                //int iDataLen = (int)tmpByteArray[3];
                //Big Endian
                int iDataLen = (int)tmpByteArray[5] + ((int)tmpByteArray[4] * (int)0x100);
                bool bTcppRes = false;
                PACKTYPE tcpp = GetTcpResFormat(LstTBLtcp, strCommandName, ref bTcppRes);

                byte[] bData = new byte[iDataLen - 8];
                try
                {       
                    for (int i = 0; i < iDataLen - 8; i++)
                    {
                        bData[i] = tmpByteArray[i + 11];
                    }
                }
                catch (Exception ex)
                {
                    rtnData = "CheckProtocol Exception1:" + ex.Message.ToString();
                    return (int)STATUS.CHECK;
                }
                
                int iSum = 0;                
                if (bTcppRes)
                {
                    if (tcpp.iType != (int)TCPRESTYPE.NONE && bData.Length < 1)
                    {
                        rtnData = "NO DATA";
                        return (int)STATUS.CHECK;
                    }

                    switch (tcpp.iType)
                    {
                        case (int)TCPRESTYPE.BYTE: 
                                                    for (int i = 0; i < bData.Length; i++)
                                                    {
                                                        rtnData += bData[i].ToString("X2");
                                                    } 
                                                    break;
                        case (int)TCPRESTYPE.BYTE1: 
                                                    for (int i = 0; i < bData.Length; i++)
                                                    {
                                                        rtnData += bData[i].ToString();
                                                    }
                                                    break;
                        case (int)TCPRESTYPE.CHAR:
                                                    if (tcpp.iPos > 0 && (bData.Length > tcpp.iPos))
                                                    {
                                                        rtnData = Encoding.UTF8.GetString(bData, tcpp.iPos, bData.Length - tcpp.iPos);
                                                    }
                                                    else
                                                    {
                                                        rtnData = Encoding.UTF8.GetString(bData);
                                                    }                                                    
                                                    rtnData = rtnData.Replace("\0", String.Empty);
                                                    rtnData = rtnData.Replace("?", String.Empty);                
                                                    rtnData = rtnData.Trim(); 
                                                    break;
                        case (int)TCPRESTYPE.INT:
                                                    if (tcpp.iPos == -1)
                                                    {
                                                        for (int i = 0; i < bData.Length; i++)
                                                        {
                                                            iSum += (int)bData[i];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (bData.Length >= tcpp.iPos)
                                                        {
                                                            iSum = (int)bData[tcpp.iPos];
                                                        }
                                                        else
                                                        {
                                                            iSum = -9999;
                                                        }                                                        
                                                    }
                                                    rtnData = iSum.ToString();
                                                    break;

                        case (int)TCPRESTYPE.DOUBLE:
                                                    double gi = BitConverter.ToDouble(bData, 0);
                                                    rtnData = gi.ToString("0.###");
                                                    break;

                        case (int)TCPRESTYPE.SINGLE:
                                                    Single si = 0;
                                                    if (bData.Length == 7)
                                                    {
                                                        if (bData[0] == 0x01)
                                                        {
                                                            si = BitConverter.ToSingle(bData, 3);
                                                            rtnData = si.ToString();
                                                        }
                                                        else
                                                        {
                                                            rtnData = "RESULT CODE FAIL";
                                                            return (int)STATUS.NG;
                                                        }
                                                    }
                                                    else if (bData.Length == 4)
                                                    {
                                                        si = BitConverter.ToSingle(bData, 0);
                                                        rtnData = si.ToString();
                                                    }
                                                    else
                                                    {
                                                        rtnData = "DATA SIZE ERROR";
                                                        return (int)STATUS.NG;
                                                    }

                                                    break;
                        case (int)TCPRESTYPE.DTC:
                                                    if (!CheckATT_DtcIndex(strOrginSendData[10], bData, ref rtnData))
                                                    {                                                      
                                                        return (int)STATUS.NG;   
                                                    }
                                                    break;                               

                        case (int)TCPRESTYPE.DTCALL:

                                                    rtnData = Encoding.UTF8.GetString(bData).Replace("-", String.Empty);

                                                    break;

                        case (int)TCPRESTYPE.ALDL_ASCII:
                                                    if (!CheckATT_ALDLType(strOrginSendData, bData, ref rtnData,
                                                        ref STEPMANAGER_VALUE.bOldBinaryALDL))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }                                                    
                                                    break;
                        case (int)TCPRESTYPE.ALDL_BITS:
                                                    if (!CheckATT_ALDLType(strOrginSendData, bData, ref rtnData,
                                                        ref STEPMANAGER_VALUE.bOldBinaryALDL))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    string strParam = rtnData;
                                                    rtnData = ChangeBitsArray(strParam);
                                                    break;

                        case (int)TCPRESTYPE.SIMINFO:
                                                    if (!CheckATT_SimInfoType(strOrginSendData, bData, ref rtnData))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }                                                    
                                                    break;

                        case (int)TCPRESTYPE.REVERSE:
                                                    if (!CheckATT_ReverseType(strCommandName, bData, ref rtnData))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    break;

                        case (int)TCPRESTYPE.NADINFO:
                                                    //문서에는 char 1byte 가 OK KO 라고 되어있는데 부정확한 의미이다.
                                                    // short length 2byte 
                                                    // char[] data
                                                       
                                                    try
                                                    {
                                                        rtnData = Encoding.UTF8.GetString(bData, 3, bData.Length - 3);
                                                        rtnData = rtnData.Replace("\0", String.Empty);
                                                        rtnData = rtnData.Replace("?", String.Empty);
                                                        rtnData = rtnData.Trim(); 
                                                    }
                                                    catch
                                                    {
                                                        rtnData = "PACKET ERROR.";
                                                        return (int)STATUS.NG;
                                                    }                                                    
                                                    break;
                        
                        case (int)TCPRESTYPE.TTFF:  //pack 1 로 안했다고 8바이트 온단다.... 어떤건 pack 하고 어떤건 pack 안하고... 진짜 GM팀하고 일하기 힘드네.
                                                    //byte[0] :   0(hot) 1(cold)
                                                    //byte[1] ~ [4] : ttff (c++ : unsigned long) (c# : uint)

                                                    byte[] bTtff = new byte[4];
                                                    if(bData.Length != 8)  //5바이트로 체크해야하나 8바이트란다...잭일슨.
                                                    {
                                                        rtnData = "TTFF SIZE ERROR";
                                                        return (int)STATUS.NG;   
                                                    }
                                                    for (int i = 0; i < bTtff.Length; i++)
                                                    {
                                                        //rtnData += bData[i].ToString("x2");
                                                        bTtff[i] = bData[i+4];
                                                    }
                                                    try
                                                    {
                                                        uint uTime = BitConverter.ToUInt32(bTtff, 0);
                                                        double dttff = (double)uTime / 1000;
                                                        rtnData = dttff.ToString("0.###");
                                                    }
                                                    catch
                                                    {
                                                     	rtnData = "TTFF ERROR.";
                                                        return (int)STATUS.NG;   
                                                    }
                                                    
                                                    break;

                        case (int)TCPRESTYPE.OOBRESULT:
                                                    for (int i = 3; i < bData.Length; i++)
                                                    {
                                                        rtnData += bData[i].ToString("X2");
                                                    }                                                    
                                                    break;

                        case (int)TCPRESTYPE.SERVICE:
                                                    if (!CheckNAD_ServiceTypeA(strOrginSendData, bData, ref rtnData))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    break;

                        case (int)TCPRESTYPE.APN_TABLE:
                                                    if (!CheckGen10_APN_TABLE(bData, ref rtnData))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    break;
                        default: 
                                                    rtnData = Encoding.UTF8.GetString(bData);
                                                    rtnData = rtnData.Trim();
                                                    break;
                    }
                }
                else
                {
                    rtnData = Encoding.UTF8.GetString(bData);
                    rtnData = rtnData.Trim(); //공백제거.
                }
                

                //ATT 는 TCP 와 달리 Success or Failur 구분 코드가 없음.
                
                switch (strCommandName) //특정 명령 예외처리 구간.... 이런거 진짜 하기 싫다.
                {
                    //case "OOB_SELF_TEST": return (int)STATUS.OK;

                    default:                        
                        return (int)STATUS.OK; 
                }
                
            }
            catch (Exception ex)
            {
                rtnData = "CheckProtocol Exception2:" + ex.Message.ToString();
                return (int)STATUS.CHECK;
            }          


        }

        public int AnalyzePacket(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString)
        {
            try
            {
                //0. VCP 응답패킷이 9바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 9) return (int)STATUS.RUNNING;

                //1. 버퍼링 나누기
                //vbvb 여기서 부터 다시하자
                int bChksum = (int)STATUS.TIMEOUT;

                int iFindStx = 0;
                bool bFind = false;
                int iSize = 0;
                for (int j = 0; j < tmpBytes.Length; j++)
                {
                    //1. STX 찾기
                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_STX1 && tmpBytes[j + 1] == DEFRES_STX2)
                    {

                        if (tmpBytes.Length < (j + 16)) return (int)STATUS.RUNNING; //계속 수신중

                        //예외명령인지 한번 체크          
                        CheckExceptionCommands(strCommandName, ref strOrginSendData);
                        
                        for (int p = 0; p < 5; p++)
                        {
                            if (strOrginSendData[4 + p].Equals(tmpBytes[j + 6 + p]))
                            {
                                bFind = true;
                            }
                            else
                            {

                                bFind = false;
                                break;
                            }
                        }

                        if (!bFind) continue;

                        if (tmpBytes.Length > (j + 5) && tmpBytes.Length > ((int)tmpBytes[j + 5] + j + 6))
                        {
                            iFindStx = j;

                            iSize = (int)tmpBytes[j + 5] + ((int)tmpBytes[j + 4] * (int)0x100) + 6;

                            if (tmpBytes.Length < (j + iSize - 1))
                            {
                                return (int)STATUS.RUNNING; //계속 수신중
                            }

                            //1. ETX 찾기                    
                            if (bFind && tmpBytes[j + iSize - 1] == DEFRES_ETX)
                            {
                                byte[] tmpBuffer = new byte[iSize];

                                Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                                bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData);
                                strLogString = BitConverter.ToString(tmpBuffer).Replace("-", " ");

                                switch (bChksum)
                                {
                                    case (int)STATUS.OK: return (int)STATUS.OK;
                                    default: break;
                                }
                                bFind = false;
                            }
                            else
                            {

                                bFind = false;
                            }
                        }
                        else
                        {
                            return (int)STATUS.RUNNING; //계속 수신중
                        }
                    }

                    
                }

                switch (bChksum)
                {
                    case (int)STATUS.NG: return (int)STATUS.NG;
                    case (int)STATUS.CHECK: return (int)STATUS.CHECK;
                    default: return (int)STATUS.RUNNING;
                }
            }
            catch (Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: AnalyzePacket() ATT:", ex.Message, false);
                return (int)STATUS.RUNNING;
            }

        }
        
                       
        public byte[] ConvertAttByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason)
        { //strSendPack은 TX에서 로깅하기 위해사용한다. rx에서는 따로 해주어야한다.
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();
            strReason = "OK";
            tmpList.Clear();
            string strValue = String.Empty;
            bool bOk = true;
            //1. DATA를 찾아 바꾼다.
            byte[] rtnNull = new byte[1];  //FAIL 리턴용

            int iDataSize = 0;
            for (int i = 0; i < tmpString.Length; i++)
            {

                int iIdx = tmpString[i].IndexOf("<DATA");

                if (iIdx > -1)
                {
                    tmpString[i] = tmpString[i].Replace("<DATA", String.Empty);
                    tmpString[i] = tmpString[i].Replace(":", String.Empty);
                    tmpString[i] = tmpString[i].Replace(">", String.Empty);
                    byte[] bParam;

                    if (String.IsNullOrEmpty(strParam))
                    {
                        tmpString[i] = "FF";
                        strReason = "PAR1 EMPTY";
                        brtnOk = false;
                        return rtnNull;
                    }
                    else
                    {
                        

                        switch (tmpString[i])
                        {                               
                            case "CHAR":
                                            iDataSize = 0;
                                            bParam = Encoding.UTF8.GetBytes(strParam);
                                            for (int p = 0; p < bParam.Length; p++)
                                            {
                                                string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                tmpList.Add(tmpChar);
                                                iDataSize++;
                                            }
                                            break;
                                                            
                            case "BYTE":                                
                                            bOk = true;
                                            bParam = HexStringToBytes(strParam, ref bOk);
                                            iDataSize = 0;
                                            if (bOk)
                                            {
                                                for (int p = 0; p < bParam.Length; p++)
                                                {
                                                    string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                    tmpList.Add(tmpChar);                                                   
                                                    iDataSize++;
                                                }
                                            }
                                            else
                                            {   
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;                            

                            case "BYTE2": //두개짜리
                                            bOk = true;
                                            bParam = HexStringToBytes(strParam, ref bOk);
                                            iDataSize = 0;
                                            if (bOk)
                                            {
                                                for (int p = 0; p < bParam.Length; p++)
                                                {
                                                    string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                    tmpList.Add(tmpChar);
                                                    iDataSize++;
                                                    if (iDataSize >= 2)
                                                    {                                                   
                                                        break;
                                                    }
                                                }

                                                for (int p = iDataSize; p < 2; p++)
                                                {
                                                    tmpList.Add("00");
                                                    iDataSize++;
                                                }
                                                
                                            }
                                            else
                                            {                                                                                
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;
                            case "DOUBLE2":
                                            if (!strParam.Contains(","))
                                            {
                                                strReason = "PAR1 NO COMMA.";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            iDataSize = 0;
                                            string[] strDidata;
                                            double di1 = 0.0; //위도
                                            double di2 = 0.0; //경도
                                            
                                            byte[] bDataParam1;
                                            byte[] bDataParam2;
                                            try
                                            {
                                                strDidata = strParam.Split(',');
                                                strDidata[0] = strDidata[0].Trim();
                                                strDidata[1] = strDidata[1].Trim();
                                                di1 = double.Parse(strDidata[0]);
                                                di2 = double.Parse(strDidata[1]);
                                                bDataParam1 = BitConverter.GetBytes(di1);
                                                bDataParam2 = BitConverter.GetBytes(di2);
                                                
                                            }
                                            catch 
                                            {
                                                strReason = "PAR1 ERROR.";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            for (int p = 0; p < bDataParam1.Length; p++)
                                            {
                                                string tmpChar = String.Format("{0:X2}", bDataParam1[p]);
                                                tmpList.Add(tmpChar);
                                                iDataSize++;
                                            }

                                            for (int p = 0; p < bDataParam2.Length; p++)
                                            {
                                                string tmpChar = String.Format("{0:X2}", bDataParam2[p]);
                                                tmpList.Add(tmpChar);
                                                iDataSize++;
                                            }
                                            break;

                            case "ALDL_HEXA":    //ALDL포멧으로 만들되 파라미터는 HEX형태로 만든다.
                            case "ALDL_ASCII":   //ALDL포멧으로 만들되 파라미터는 아스키로 만든다.   
                            case "ALDL_HEXB":    //ALDL포멧으로 만들되 파라미터는 HEX형태이나 특정 바이트에서 특정비트를 1로 변경후 다시 바이트배열로.
                            case "ALDL_HEXC":    //ALDL포멧으로 만들되 파라미터는 HEX형태이나 특정 바이트에서 특정비트를 0로 변경후 다시 바이트배열로.
                                            strValue = String.Empty;
                                            bOk = true;
                                            bParam = new byte[1];

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA":  brtnOk = true; break;
                                                case "ALDL_ASCII": brtnOk = true; break;
                                                case "ALDL_HEXB":  brtnOk = MakeToggleAldl(true, strParam, ref strValue, ref strReason); break;
                                                case "ALDL_HEXC":  brtnOk = MakeToggleAldl(false, strParam, ref strValue, ref strReason); break;
                                                default:           brtnOk = false; break;
                                            }
                                            
                                            if (!brtnOk) return rtnNull;

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA":  bParam = MakeATT_ALDLType(strParam, ref bOk, ref strReason, false); break;
                                                case "ALDL_ASCII": bParam = MakeATT_ALDLType(strParam, ref bOk, ref strReason, true); break;
                                                case "ALDL_HEXB":  bParam = MakeATT_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                case "ALDL_HEXC":  bParam = MakeATT_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                default: bOk = false; strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";  break;
                                            }

                                            if (bOk)
                                            {
                                                for (int p = 0; p < bParam.Length; p++)
                                                {
                                                    string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                    tmpList.Add(tmpChar);
                                                    iDataSize++;
                                                }
                                            }
                                            else
                                            {                                                
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;

                            default:                                            
                                            strReason = "DATA TYPE NONE.";
                                            brtnOk = false;
                                            return rtnNull;
                                            
                        }
                    }

                    if (brtnOk)
                    {                       

                        for (int j = 0; j < tmpList.Count; j++)
                        {
                            if (tmpList[j].Equals("<SIZE:DATA>"))
                            {
                                tmpList.RemoveAt(j);

                                if (iDataSize == 0) break;
                                if (iDataSize > (int)0xFFFF)
                                {
                                    tmpList.Insert(j, "FF");
                                    tmpList.Insert(j, "FF");
                                }
                                else
                                {
                                    try
                                    {
                                        if (iDataSize > (int)0xFF) //data size 는 little endian
                                        {
                                            byte bHByte = (byte)((ushort)iDataSize >> 8);
                                            tmpList.Insert(j, bHByte.ToString("x2")); 

                                            byte bLByte = (byte)((ushort)iDataSize & 0xFF);
                                            tmpList.Insert(j, bLByte.ToString("x2"));
                                        }
                                        else
                                        {
                                            tmpList.Insert(j, "00");
                                            tmpList.Insert(j, iDataSize.ToString("x2"));                                            
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":1:" + ex.Message;
                                        STEPMANAGER_VALUE.DebugView(strExMsg);
                                    }

                                }
                            }     
                        }

                    }
                }
                else
                {
                    tmpList.Add(tmpString[i]);

                }
                
            }

            //2. LENGTH 를 찾아 바꾼다.
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (tmpList[i].Equals("<LENGTH>"))
                {
                    tmpList.RemoveAt(i);

                    int iLenSize = tmpList.Count - 1;

                    if (iLenSize > (int)0xFFFF)
                    {
                        tmpList.Insert(i, "FF");
                        tmpList.Insert(i, "FF");
                    }
                    else
                    {
                        try
                        {
                            if (iLenSize > (int)0xFF)  //프로토콜 전체 length는 Big Endian
                            {   
                                byte bLByte = (byte)((ushort)iLenSize & 0xFF);
                                tmpList.Insert(i, bLByte.ToString("X2")); 

                                byte bHByte = (byte)((ushort)iLenSize >> 8);
                                tmpList.Insert(i, bHByte.ToString("X2"));                                                                                    
                            }
                            else
                            {
                                tmpList.Insert(i, iLenSize.ToString("X2"));
                                tmpList.Insert(i, "00");                                
                            }
                        }
                        catch(Exception ex)
                        {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":2:" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg);
                        }

                    }
                }
                
            }

            //3. 체크섬를 찾아 바꾼다.
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (tmpList[i].Equals("<CRC16>"))
                {
                    byte[] tmpArray = new byte[tmpList.Count];

                    for (int j = 0; j < i/*tmpArray.Length*/; j++)
                    {//체크섬위치를 찾으면 체크섬 앞의 위치까지를 재정렬한다.
                        try { tmpArray[j] = Convert.ToByte(tmpList[j], 16); }
                        catch (Exception ex) {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":3:" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg);
                            tmpArray[j] = 0xFF; }
                    }
                    
                    DK_CHECKSUM DKCHK = new DK_CHECKSUM(); //체크섬!
                    byte bHighByte = 0x00;
                    byte bLowByte = 0x00;
                    DKCHK.Tcp_chksum(tmpArray, ref bHighByte, ref bLowByte, true);
                    tmpList.RemoveAt(i);
                    tmpList.Insert(i, bLowByte.ToString("X2"));   //BIG ENDIAN 
                    tmpList.Insert(i, bHighByte.ToString("X2"));                           

                }                
                
            }

            byte[] rtnValue = new byte[tmpList.Count];

            if (!brtnOk) return rtnValue;
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
                    strReason = "MAKE PACKET ERROR.";
                    brtnOk = false;
                    return rtnValue;
                }


            }
            strSendPack = BitConverter.ToString(rtnValue).Replace("-", " ");
            return rtnValue;
        }

        private bool MakeToggleAldl(bool bToggleOn, string strParam, ref string strResult, ref string strReason)
        {
            strResult = String.Empty;
            strReason = "SUCCESS";
            ///////////////////////////////////////////////////////
            int iDxByte = 0;
            int iDxBit = 0;
            string[] strParseData = strParam.Split(',');

            if (strParseData.Length != 4)
            {
                strReason = "PAR1 ERROR(Address,AldlValue,ByteIndex,BitIndex) - (" + strParam + ")";
                return false;
            }

            if (String.IsNullOrEmpty(strParseData[1]))
            {
                strReason = "PAR1 ERROR(ALDL VALUE NONE)";
                return false;
            }

            try
            {
                iDxByte = int.Parse(strParseData[2]);
                iDxBit = int.Parse(strParseData[3]);
            }
            catch
            {
                strReason = "PAR1 ERROR(ByteIndex,BitIndex) - " + strParam + ")";
                return false;
            }

            if (STEPMANAGER_VALUE.bOldBinaryALDL == null)
            {
                strReason = "NONE ALDL VALUE";
                return false; 
            }

            if (STEPMANAGER_VALUE.bOldBinaryALDL.Length < iDxByte && 1 > iDxByte)
            {
                strReason = "BYTE INDEX OVER(" + strParseData[2] + ")";
                return false;
            }

            if (8 < iDxBit && 1 > iDxBit)
            {
                strReason = "BIT INDEX RANGE OVER(Required 1~8) - Param(" + strParseData[3] + ")";
                return false;
            }

            iDxByte--;
            iDxBit--;

            bool bConvOK = false;
            byte[] bChangeAldl = HexStringToBytes(strParseData[1], ref bConvOK);
            
            // (1) byte를 비트문자열로 변환                        
            StringBuilder sb = new StringBuilder();
            byte a = Convert.ToByte(bChangeAldl[iDxByte]);

            sb.Append(Convert.ToString(a, 2).PadLeft(8, '0'));

            if (bToggleOn)
                sb[iDxBit] = '1';
            else
                sb[iDxBit] = '0';

            // (2) 비트문자열을 byte로 변환
            string bitStr = sb.ToString();
            byte b = Convert.ToByte(bitStr, 2); 

            bChangeAldl[iDxByte] = b;

            strResult = strParseData[0] + "," + BitConverter.ToString(bChangeAldl).Replace("-", "");

            return true;
        }

        private bool CheckNAD_ServiceTypeA(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
        {
            try
            {
                byte[] bParseData = new byte[bGetData.Length - 1]; //전체데이터 복사본
                byte[] bLenth = new byte[2];                       //길이 재는 용도

                STEPMANAGER_VALUE.OOBServiceClear();               //데이터 초기화.

                Array.Copy(bGetData, 1, bParseData, 0, bParseData.Length); //데이터에서 앞에 3바이트를 빼고 시작한다. 연구소도 인원이 교체되서 문서에도 없다. 문서와 구조체가 다르다. 난감하다.


                if (!bGetData.Length.Equals(285)) //TCP 2R1Zf 는 GEN10 TCP랑 다르게 285 로 오네. 
                {
                    strRetunData = "Data Size missmatch.";
                    return false;
                }

                int iDx = 0;
                for (int i = (int)ServiceIndexA.szMEID; i < (int)ServiceIndexA.END; i++)
                {
                    try
                    {
                        switch (i)
                        {   //PACK(1)을 안했다는 이유로 몇가지 변수를 데이터타입과 다르게 처리 하라고함 // 연구소 강종훈 책임 (2018.07.18 오전 11:37 메일)
                            case (int)ServiceIndexA.szMEID: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexA.szICCID: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 30); iDx += 30; break;
                            case (int)ServiceIndexA.cRadioIF: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nActiveChannel: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nCurrentSID: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.iTxPwr: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.cCallState: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;

                            case (int)ServiceIndexA.szDialedDigits: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 33); iDx += 33; break;
                            case (int)ServiceIndexA.nPilotPN: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cServiceDomain: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cRSSI: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.nErrorRate: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cDTMFEvent: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nServiceOption: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cServiceStatus: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.szIMEI: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexA.cCSAttachState: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cPSAttachState: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nMCC: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nMNC: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nArfcn: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cBSIC: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cECIO: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.iCellID: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.nUARFCN: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cAttachState: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nEARFCN: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nSNR: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nRSRP: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.szBanner: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 50); iDx += 50; break;
                            case (int)ServiceIndexA.nTAC: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.iVocoder: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.iVocoderRate: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.iRSRP: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.cBand_1900: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.nLAC: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.szMDN: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexA.iServiceDomainPref: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.iScanList: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.bIsVoLTE: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.szIncomingNumber: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 33); iDx += 33; break;
                            case (int)ServiceIndexA.cSINR: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cRSRQ: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            default:
                                break;
                        }

                    }
                    catch
                    {
                        strRetunData = "CheckTCP_ServiceType:Exception1";
                        return false;
                    }

                }

                strRetunData = "OK";
                return true;
            }
            catch
            {
                strRetunData = "CheckTCP_ServiceType:Exception2";
                return false;
            }


        }

        private string ChangeBitsArray(string strRtndata)
        {
            bool bCheck = false;
            byte[] bChangeAldl = HexStringToBytes(strRtndata, ref bCheck);

            // (1) byte를 비트문자열로 변환                        
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bChangeAldl.Length; i++)
            {
                byte a = Convert.ToByte(bChangeAldl[i]);
                sb.Append(Convert.ToString(a, 2).PadLeft(8, '0'));
            }

            return sb.ToString();
        }

        public byte[]  HexStringToBytes(string s, ref bool bOk)
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
          
        private bool CheckASCII(int iDataLen, byte[] bData)
        {
            //여기서 아스키로 변환해야하는지 한바이트만 체크하자.
            bool bIsAscii = false;
            int iDx = iDataLen;
            if (iDataLen > bData.Length)
            {
                iDx = bData.Length;
            }

            for (int i = 0; i < iDx; i++)
            {
                if (bData[i] >= 0x61 && bData[i] <= 0x7A)
                {
                    bIsAscii = true; //61 ~ 7a  영문 소문자
                }

                if (bData[i] >= 0x41 && bData[i] <= 0x5A)
                {
                    bIsAscii = true; //41 ~ 5A  영문 대문자
                }

                if (bData[i] >= 0x30 && bData[i] <= 0x39)
                {
                    bIsAscii = true;  //30 ~ 39 숫자
                }

                if (bData[i] >= 0x00 && bData[i] < 0x20)
                {
                    bIsAscii = false; break;//비아스키
                }

                if (bData[i] >= 0x80 && bData[i] <= 0xFF)
                {
                    bIsAscii = false; break;//비아스키
                }
            }

            return bIsAscii;

        }

        private bool CheckGen10_APN_TABLE(byte[] bGetData, ref string strRetunData)
        {
            STEPMANAGER_VALUE.GEN10APN_TABLE.Clear();

            //총사이즈 체크 2413 바이트
            if (bGetData.Length != 2413)
            {
                strRetunData = "APN_TABLE SIZE ERROR";
                return false;
            }

            //RESULT CODE 체크
            if (bGetData[0] != 0x01)
            {
                strRetunData = "APN_TABLE RESULT NG";
                return false;
            }

            //구조체 데이터 분리후 저장
            /*
               Thursday, April 11, 2019 7:29 PM  이동성 책임 메일 참조
             */

            string[] strDatas = new string[(int)APNINDEX.END];

            byte[] bytePackets = new byte[241];
            int iPackIndex = 3;
            string strTemp = String.Empty;
            bool bTemp = true;
            int iApnIndex = 0;

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    strDatas = new string[(int)APNINDEX.END];
                    iApnIndex = 0;
                    Array.Copy(bGetData, iPackIndex, bytePackets, 0, bytePackets.Length);

                    strTemp = Encoding.UTF8.GetString(bytePackets, iApnIndex, 100).Replace("\0", String.Empty);
                    strDatas[(int)APNINDEX.szAPNName] = strTemp; iApnIndex += 100;

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iAPNNameSize] = strTemp; iApnIndex += 4;

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iUserNameSize] = strTemp; iApnIndex += 4;

                    strTemp = Encoding.UTF8.GetString(bytePackets, iApnIndex, 72).Replace("\0", String.Empty);
                    strDatas[(int)APNINDEX.szUserName] = strTemp; iApnIndex += 72;

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iPasswordSize] = strTemp; iApnIndex += 4;

                    strTemp = Encoding.UTF8.GetString(bytePackets, iApnIndex, 16).Replace("\0", String.Empty);
                    strDatas[(int)APNINDEX.szPassword] = strTemp; iApnIndex += 16;

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iRenewalRate] = strTemp; iApnIndex += 4;

                    strTemp = Encoding.UTF8.GetString(bytePackets, iApnIndex, 20).Replace("\0", String.Empty);
                    strDatas[(int)APNINDEX.szDialString] = strTemp; iApnIndex += 20;

                    bTemp = BitConverter.ToBoolean(bytePackets, iApnIndex); iApnIndex += 1;
                    if (bTemp)
                        strDatas[(int)APNINDEX.bHRPD] = "ACTIVE";
                    else
                        strDatas[(int)APNINDEX.bHRPD] = "DEACTIVE";

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iAPNIndex] = strTemp; iApnIndex += 4;

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iPDIndex] = strTemp; iApnIndex += 4;

                    bTemp = BitConverter.ToBoolean(bytePackets, iApnIndex); iApnIndex += 1;
                    if (bTemp)
                        strDatas[(int)APNINDEX.bIPv4] = "ACTIVE";
                    else
                        strDatas[(int)APNINDEX.bIPv4] = "DEACTIVE";

                    bTemp = BitConverter.ToBoolean(bytePackets, iApnIndex); iApnIndex += 1;
                    if (bTemp)
                        strDatas[(int)APNINDEX.bIPv6] = "ACTIVE";
                    else
                        strDatas[(int)APNINDEX.bIPv6] = "DEACTIVE";

                    strTemp = BitConverter.ToInt32(bytePackets, iApnIndex).ToString();
                    strDatas[(int)APNINDEX.iRmNetIndex] = strTemp; iApnIndex += 4;

                    bTemp = BitConverter.ToBoolean(bytePackets, iApnIndex); iApnIndex += 1;
                    if (bTemp)
                        strDatas[(int)APNINDEX.bReadOK] = "ACTIVE";
                    else
                        strDatas[(int)APNINDEX.bReadOK] = "DEACTIVE";

                    bTemp = BitConverter.ToBoolean(bytePackets, iApnIndex); iApnIndex += 1;
                    if (bTemp)
                        strDatas[(int)APNINDEX.bAlwaysOn] = "ACTIVE";
                    else
                        strDatas[(int)APNINDEX.bAlwaysOn] = "DEACTIVE";


                    iPackIndex += bytePackets.Length;
                    STEPMANAGER_VALUE.GEN10APN_TABLE.Add(strDatas);
                }
            }
            catch
            {
                STEPMANAGER_VALUE.GEN10APN_TABLE.Clear();
                strRetunData = "APN_TABLE PARSE ERROR";
                return false;
            }

            strRetunData = "OK";
            return true;
        }
    }

}
