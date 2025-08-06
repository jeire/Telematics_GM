using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    // MCTM 프로토콜 포멧
    // HEADER(4byte),LENGTH(2byte),TYPECODE1(1byte),DUMMY0x00(1byte),MODULECODE(1byte),FUNCTION1(1byte),dummy(0x00),RESULTCODE(1byte),DATALENGTH(2byte),DEBUGCODE(1byte),data,+CRC(2byte),ETX(1byte)

    class DK_ANALYZER_MCTM
    {        
        private const byte DEFRES_STX1 = 0x4B;
        private const byte DEFRES_STX2 = 0x55;
        private const byte DEFRES_ETX  = 0x7E;

        private const byte DEFRESPONSE = 0x02;
        private const byte DEFREPORT   = 0x03;

        private const string HEX_CHARS = "0123456789ABCDEF";


        private List<TBLDATA0> LstTBLMctm = new List<TBLDATA0>();       //MCTM 명령 테이블 리스트
        private DK_LOGGER tmpLogger = new DK_LOGGER("PC", false);

        public DK_ANALYZER_MCTM()
        {
            tmpLogger.LoadTBL0("MCTM.TBL", ref LstTBLMctm);
        }

        public PACKTYPE GetMctmResFormat(List<TBLDATA0> lstTBLDB, string strCommandName, ref bool bFind)
        {
            PACKTYPE iResult = new PACKTYPE();
            iResult.iType = (int)MCTMRESTYPE.NONE;
            iResult.iPos = -1;
            iResult.bSplit = false;
            bFind = false;
            try
            {
                for (int i = 0; i < lstTBLDB.Count; i++)
                {
                    if (lstTBLDB[i].CMDNAME.Equals(strCommandName))
                    {
                        switch (lstTBLDB[i].PARPAC2)
                        {
                            case "DEBUG": iResult.bSplit = true; break;
                            case "NONE": iResult.bSplit = false; break;
                            default: iResult.bSplit = true; break;
                        }
                        switch (lstTBLDB[i].RECVPAC)
                        {   //NONE, BYTE, CHAR, INT, DTC, DOUBLE, SINGLE, INT16, INT32, END
                            //바이트문자열 2자리씩 고정하여 표시
                            case "BYTE": iResult.iType = (int)MCTMRESTYPE.BYTE; break;                            
                            //아스키로 문자열로 표시
                            case "CHAR": iResult.iType = (int)MCTMRESTYPE.CHAR; break;
                            //숫자 전체 합산
                            case "INT": iResult.iType = (int)MCTMRESTYPE.INT; break;
                            //DTC 값에 의한 별도 표시
                            case "DTC": iResult.iType = (int)MCTMRESTYPE.DTC; break;
                            //8바이트 표시
                            case "DOUBLE": iResult.iType = (int)MCTMRESTYPE.DOUBLE; break; //8byte
                            //4바이트 표시
                            case "SINGLE": iResult.iType = (int)MCTMRESTYPE.SINGLE; break; //4byte

                            case "INT16": iResult.iType = (int)MCTMRESTYPE.INT16; break;  //2byte

                            case "INT32": iResult.iType = (int)MCTMRESTYPE.INT32; break;  //4byte

                            case "DTC_TABLE": iResult.iType = (int)MCTMRESTYPE.DTC; break;

                            case "DTC_BITS": iResult.iType = (int)MCTMRESTYPE.DTCBITS; break;

                            case "GPS0": iResult.iType = (int)MCTMRESTYPE.GPS0; break; //GPS SVCOUNT
                            case "GPS1": iResult.iType = (int)MCTMRESTYPE.GPS1; break; //GPS  CN0
                            case "GPS2": iResult.iType = (int)MCTMRESTYPE.GPS2; break; //GNSS SVCOUNT
                            case "GPS3": iResult.iType = (int)MCTMRESTYPE.GPS3; break; //GNSS CN0

                            case "TTFF": iResult.iType = (int)MCTMRESTYPE.TTFF; break; //1byte isFixed, 8byte double

                            case "USIM1": iResult.iType = (int)MCTMRESTYPE.USIM1; break; //NAD1
                            case "USIM2": iResult.iType = (int)MCTMRESTYPE.USIM2; break; //NAD2
                            case "USIM3": iResult.iType = (int)MCTMRESTYPE.USIM3; break; //NAD3

                            case "ALDL_ASCII": iResult.iType = (int)MCTMRESTYPE.ALDL_ASCII; break;
                            case "ALDL_BITS": iResult.iType = (int)MCTMRESTYPE.ALDL_BITS; break;
                            case "ALDL_HEXA": iResult.iType = (int)MCTMRESTYPE.ALDL_HEXA; break;

                            default: iResult.iType = (int)MCTMRESTYPE.NONE; break;
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
                bFind = false;
            }  

            return iResult;
        }

        private bool CheckMctm_DtcIndex(byte bSendParam, byte[] bGetData, ref string strRetunData, int iType)
        {
            try
            {
                // 받은 데이터 길이 체크
                if (bGetData.Length != 4)
                {
                    strRetunData = "ERROR DTC SIZE";
                    return false; 
                }                               
              
                strRetunData = String.Empty;

                switch (iType)
                {
                    case (int)MCTMRESTYPE.DTC:     strRetunData = bGetData[3].ToString("X2").ToUpper(); break;
                    case (int)MCTMRESTYPE.DTCBITS: strRetunData = Convert.ToString(bGetData[3], 2).PadLeft(8, '0'); break;
                    default: strRetunData = bGetData[3].ToString("X2"); break;
                    //8비트로 나타내는 방법 (DQA 담당자 요청방식)
                    //strRetunData = Convert.ToString(bGetData[3], 2).PadLeft(8, '0');

                    //2번째 비트만 나타내는 방법 (현우씨 요청방식)
                    //strRetunData = strRetunData.Substring(6, 1);

                    //바이트만 그대로 나타내는 방법 (GEN10과 동일방식 - 20160229)
                    //strRetunData = bGetData[3].ToString("X2");
                }

                return true;
            }
            catch
            {
                strRetunData = "CheckMctm_DtcIndex:Exception";
                return false;
            }

        }
                
        public int CheckProtocol(byte[] tmpByteArrays, byte[] strOrginSendData, string strCommandName, ref string rtnData, bool bResultCodeOption)
        {             
            //체크섬 검사.
            DK_CHECKSUM DKCHK = new DK_CHECKSUM();
            byte bChkHigh = 0x00;
            byte bChkLow = 0x00;
            byte[] tmpDecode = DKCHK.CRC16_DECODE(tmpByteArrays);
            DKCHK.CRC16_ENCODE(tmpDecode, tmpDecode.Length - 3, ref bChkHigh, ref bChkLow);
            if (tmpDecode[tmpDecode.Length - 3] != bChkLow ||
                tmpDecode[tmpDecode.Length - 2] != bChkHigh)
            {
                rtnData = "CHECKSUM NG";
                return (int)STATUS.NG;
            }
           
            try
            {                
                //보낸명령어에 대한 응답데이터인지 확인
                switch (tmpDecode[6])
                {
                    case DEFRESPONSE:
                    case DEFREPORT: break;
                            
                    default: return (int)STATUS.RUNNING; 
                }
                
                for (int i = 7; i < 10; i++)
                {
                    if (tmpDecode[i] != strOrginSendData[i])
                    {
                        return (int)STATUS.RUNNING;
                    }
                }

                //데이터 수집 구간 시작------------------------------------------             
                int iDataLen = (int)tmpDecode[12] + ((int)tmpDecode[13] * (int)0x100);
                bool bMCTMRes = false;
                PACKTYPE Opack = GetMctmResFormat(LstTBLMctm, strCommandName, ref bMCTMRes);

                byte[] bData = new byte[iDataLen];
                if (iDataLen > 0)
                {
                    try
                    {
                        if (Opack.bSplit) //head 가 있는 데이터면, 헤더를 잘라내고 데이터로 사용해야함....으.....
                        {
                            //
                            if (tmpDecode[14] != strOrginSendData[14])
                            {
                                return (int)STATUS.RUNNING;
                            }
                            //
                            bData = new byte[iDataLen - 1];
                            for (int i = 0; i < bData.Length; i++)
                            {
                                bData[i] = tmpDecode[i + 1 + 14];
                            }
                        }
                        else //body만, 즉 잘라낼 헤더가 없다면 그대로 전부 데이터구간으로 확정
                        {
                            for (int i = 0; i < bData.Length; i++)
                            {
                                bData[i] = tmpDecode[i + 14];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rtnData = "CheckProtocol Exception1:" + ex.Message.ToString();
                        return (int)STATUS.CHECK;
                    }
                
                }
                
                int iSum = 0;
                if (bMCTMRes)
                {
                    if (Opack.iType != (int)MCTMRESTYPE.NONE && bData.Length < 1)
                    {
                        rtnData = "NO DATA";
                        return (int)STATUS.CHECK;
                    }

                    switch (Opack.iType)
                    {
                        case (int)MCTMRESTYPE.BYTE: //바이트문자열 2자리로 고정하여 표시
                                                    for (int i = 0; i < bData.Length; i++)
                                                    {
                                                        rtnData += bData[i].ToString("X2");
                                                    } 
                                                    break;

                        case (int)MCTMRESTYPE.CHAR:                                                    
                                                    rtnData = Encoding.UTF8.GetString(bData);
                                                    rtnData = rtnData.Replace("\0", String.Empty);
                                                    rtnData = rtnData.Replace("?", String.Empty);                
                                                    rtnData = rtnData.Trim();
                                                    break;
                        case (int)MCTMRESTYPE.USIM1:
                        case (int)MCTMRESTYPE.USIM2:
                        case (int)MCTMRESTYPE.USIM3:
                                                    byte byteNadIndex = bData[0];
                                                    bool bCompareIndex = true;
                                                    switch (Opack.iType)
                                                    {
                                                        case (int)MCTMRESTYPE.USIM1: if (byteNadIndex != 0x00) bCompareIndex = false; break;
                                                        case (int)MCTMRESTYPE.USIM2: if (byteNadIndex != 0x01) bCompareIndex = false; break;
                                                        case (int)MCTMRESTYPE.USIM3: if (byteNadIndex != 0x02) bCompareIndex = false; break;
                                                        default: break;
                                                    }
                                                    if (!bCompareIndex)
                                                    {
                                                        rtnData = "NAD ID IS INCORRECT";
                                                        return (int)STATUS.NG;
                                                    }
                                                    rtnData = Encoding.UTF8.GetString(bData, 1, bData.Length-1);
                                                    rtnData = rtnData.Replace("\0", String.Empty);
                                                    rtnData = rtnData.Replace("?", String.Empty);
                                                    rtnData = rtnData.Trim();
                                                    break;

                        case (int)MCTMRESTYPE.INT:
                                                    if (Opack.iPos == -1)
                                                    {
                                                        for (int i = 0; i < bData.Length; i++)
                                                        {
                                                            iSum += (int)bData[i];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (bData.Length >= Opack.iPos)
                                                        {
                                                            iSum = (int)bData[Opack.iPos];
                                                        }
                                                        else
                                                        {
                                                            iSum = -9999;
                                                        }                                                        
                                                    }
                                                    rtnData = iSum.ToString();
                                                    break;

                        case (int)MCTMRESTYPE.DOUBLE: //8바이트 표시
                                                    try
                                                    {
                                                        double gi = BitConverter.ToDouble(bData, 0);
                                                        rtnData = gi.ToString("#.##");
                                                    }
                                                    catch 
                                                    {
                                                        rtnData = "DATA TYPE ERROR(NO DOUBLE)";
                                                        return (int)STATUS.CHECK;
                                                    }                                                    
                                                    break;

                        case (int)MCTMRESTYPE.SINGLE: //4바이트 표시
                                                    Single[] si = new Single[iDataLen / 4];
                                                    byte[] bSection = new byte[4];
                                                    try
                                                    {
                                                        if (Opack.iPos == -1)
                                                        {
                                                            Array.Copy(bData, 0, bSection, 0, 4);
                                                            si[0] = BitConverter.ToSingle(bSection, 0);
                                                            rtnData = si[0].ToString();
                                                        }
                                                        else
                                                        {

                                                            for (int i = 0; i < si.Length; i++)
                                                            {
                                                                Array.Copy(bData, i * 4, bSection, 0, 4);
                                                                si[i] = BitConverter.ToSingle(bSection, 0);
                                                            }
                                                            rtnData = si[Opack.iPos].ToString();
                                                        }
                                                    }
                                                    catch 
                                                    {
                                                        rtnData = "Error. Size(" + iDataLen.ToString() + ") Data(" + bData.Length.ToString() + " POS(" + Opack.iPos + ")";                                                        
                                                        return (int)STATUS.CHECK;
                                                    }   
                                                    break;

                        case (int)MCTMRESTYPE.INT16: //2바이트                                                     
                                                    try
                                                    {
                                                        Int16 i16 = 0;// BitConverter.ToInt16(bData, 0);

                                                        if (Opack.iPos == -1)
                                                        {
                                                            i16 = BitConverter.ToInt16(bData, 0);    
                                                        }
                                                        else
                                                        {
                                                            i16 = BitConverter.ToInt16(bData, Opack.iPos);
                                                        }
                                                        rtnData = i16.ToString();
                                                    }
                                                    catch 
                                                    {
                                                        rtnData = "DATA TYPE ERROR(NO INT16)";
                                                        return (int)STATUS.CHECK;
                                                    }   
                                                    break;

                        case (int)MCTMRESTYPE.INT32: //4바이트 
                                                    try
                                                    {
                                                        Int32 i32 = BitConverter.ToInt32(bData, 0);
                                                        rtnData = i32.ToString();
                                                    }                                                    
                                                    catch 
                                                    {
                                                        rtnData = "DATA TYPE ERROR(NO INT32)";
                                                        return (int)STATUS.CHECK;
                                                    }   
                                                    break;                        

                        case (int)MCTMRESTYPE.DTC:
                                                    if (!CheckMctm_DtcIndex(strOrginSendData[12], bData, ref rtnData, (int)MCTMRESTYPE.DTC))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    break;

                        case (int)MCTMRESTYPE.DTCBITS:
                                                    if (!CheckMctm_DtcIndex(strOrginSendData[12], bData, ref rtnData, (int)MCTMRESTYPE.DTCBITS))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    break;

                        case (int)MCTMRESTYPE.NONE:
                                                    rtnData = String.Empty;
                                                    break;

                        case (int)MCTMRESTYPE.GPS0: //GPS SVCOUNT
                        case (int)MCTMRESTYPE.GPS1: //GPS CN0
                        case (int)MCTMRESTYPE.GPS2: //GNSS SVCOUNT
                        case (int)MCTMRESTYPE.GPS3: //GNSS CN0
                                                    try
                                                    {
                                                        if (bData.Length != 4)
                                                        {
                                                            rtnData = "DATA SIZE ERROR(NO 4BYTE)";
                                                            return (int)STATUS.NG;
                                                        }
                                                        int iData1 = (int)bData[0];
                                                        int iData2 = (int)bData[1];
                                                        int iData3 = (int)bData[2];
                                                        int iData4 = (int)bData[3];

                                                        switch (Opack.iType)
                                                        {
                                                            case (int)MCTMRESTYPE.GPS0: rtnData = iData1.ToString();break;//GPS SVCOUNT
                                                            case (int)MCTMRESTYPE.GPS1: rtnData = iData2.ToString();break;//GPS CN0
                                                            case (int)MCTMRESTYPE.GPS2: rtnData = iData3.ToString();break;//GNSS SVCOUNT
                                                            case (int)MCTMRESTYPE.GPS3: rtnData = iData4.ToString();break;//GNSS CN0
                                                            default: rtnData = "-999"; return (int)STATUS.CHECK;  
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        rtnData = "GPS/GNSS DATA ERROR";
                                                        return (int)STATUS.CHECK;
                                                    }
                                                    break;
                        case (int)MCTMRESTYPE.TTFF: //1byte isFixed, 8byte time(double)
                                                    try
                                                    {
                                                        if (bData.Length != 9)
                                                        {
                                                            rtnData = "TTFF DATA SIZE ERROR";
                                                            return (int)STATUS.CHECK;
                                                        }
                                                        double gi = BitConverter.ToDouble(bData, 1);
                                                        rtnData = gi.ToString("#.##");
                                                        if (bData[0] != 0x01)
                                                        {
                                                            rtnData = "No Fixed";
                                                            return (int)STATUS.NG;
                                                        }
                                                    }
                                                    catch 
                                                    {
                                                        rtnData = "TTFF DATA ERROR";
                                                        return (int)STATUS.CHECK;
                                                    }                                                    
                                                    break;

                        case (int)MCTMRESTYPE.ALDL_ASCII:
                                                    STEPMANAGER_VALUE.bMctmALDLData = null;
                                                    STEPMANAGER_VALUE.bMctmALDLIndex = 0x00;
                                                    if (!CheckMCTM_ALDLType(strOrginSendData, bData, ref rtnData, false))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    break;

                        case (int)MCTMRESTYPE.ALDL_BITS:
                                                    STEPMANAGER_VALUE.bMctmALDLData = null;
                                                    STEPMANAGER_VALUE.bMctmALDLIndex = 0x00;
                                                    if (!CheckMCTM_ALDLType(strOrginSendData, bData, ref rtnData, true))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }
                                                    string strParam = rtnData;
                                                    rtnData = ChangeBitsArray(strParam);
                                                    break;

                        case (int)MCTMRESTYPE.ALDL_HEXA:
                                                    STEPMANAGER_VALUE.bMctmALDLData = null;
                                                    STEPMANAGER_VALUE.bMctmALDLIndex = 0x00;
                                                    if (!CheckMCTM_ALDLType(strOrginSendData, bData, ref rtnData, true))
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
                    rtnData = rtnData.Trim();
                }

                if (bResultCodeOption)
                {
                    //프로토콜의 ResultCode(Success or Fail) 로 판정을 요하는 경우는 이것으로 처리한다.                    
                    rtnData = tmpDecode[11].ToString("X2");
                    return (int)STATUS.OK;
                }
                else
                {
                    //Success or Failur 구분 코드.
                    if (tmpDecode[11] == 0x01)
                    {
                        return (int)STATUS.OK;
                    }
                    else
                    {
                        switch (strCommandName) //특정 명령 예외처리 구간...
                        {
                            case "#### TEST ####": 
                                    return (int)STATUS.OK;

                            default:
                                    switch(tmpDecode[11])                                    
                                    {
                                        case 0x00: rtnData = "ERROR CODE:FAIL"; break;
                                        case 0x02: rtnData = "ERROR CODE:Working"; break;
                                        default: rtnData = "ERROR CODE:" + tmpDecode[11].ToString("X2") ; break;
                                    }
                                    return (int)STATUS.NG;
                        }

                    }
                }
                
                 

            }
            catch (Exception ex)
            {
                rtnData = "CheckProtocol Exception2:" + ex.Message.ToString();
                return (int)STATUS.CHECK;
            }

        }               

        public int AnalyzePacket(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, bool bResultCodeOption)
        {
            try
            {
                //0. 응답패킷이 17바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 17) return (int)STATUS.RUNNING;

                //1. 버퍼링 나누기
                int bChksum = (int)STATUS.TIMEOUT;
                int iFind = 0;
                int iFindStx = 0;
                bool bFind = false;
                for (int j = 0; j < tmpBytes.Length; j++)
                {

                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_STX1 && tmpBytes[j + 1] == DEFRES_STX2)
                    {
                        iFindStx = j; bFind = true;
                    }
                    if (bFind && tmpBytes[j] == DEFRES_ETX)
                    {
                        byte[] tmpBuffer = new byte[iFind + 1];

                        Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                        bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData, bResultCodeOption);

                        if (bChksum == (int)STATUS.OK)
                        {
                            return (int)STATUS.OK;
                        }
                        iFind = 0;
                        bFind = false;
                    }

                    if (bFind) iFind++;
                }

                switch (bChksum)
                {
                    case (int)STATUS.NG: return (int)STATUS.NG;
                    case (int)STATUS.CHECK: return (int)STATUS.CHECK;
                    default: return (int)STATUS.RUNNING;
                }
            }
            catch(Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: AnalyzePacket(MCTM) : ", ex.Message, false);
                return (int)STATUS.RUNNING;
            }            
                
        }   
                
        public byte[] ConvertMCTMByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason)
        {   //strSendPack은 TX에서 로깅하기 위해사용한다. rx에서는 따로 해주어야한다.
            //bCallClass 는 아날라이져에서 부른건지 COMM 에서 부른건지 알수 없기때문에 생긴거다. 이유는 인증서 카운팅을 막아야하기 때문이다.
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();
            strReason = "OK";
            tmpList.Clear();
            bool bOk = true;
            //1. DATA를 찾아 바꾼다.
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
                        byte[] bErrorByte = new byte[10];
                        return bErrorByte;
                    }
                    else
                    {                 
                        switch (tmpString[i])
                        {
                          
                            case "SHORT":   
                                            iDataSize = 1;
                                            byte byteOne = 0x00;
                                            try
                                            {
                                                byteOne = byte.Parse(strParam);
                                                tmpList.Add(byteOne.ToString("X2"));
                                            }
                                            catch
                                            {
                                                tmpString[i] = "FF";
                                                strReason = "PAR1 TYPE ERROR";
                                                brtnOk = false;
                                                byte[] bErrorByte = new byte[10];
                                                return bErrorByte;
                                            }                                                                                 
                                            break;
                                            
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

                            case "MCTMMAC":  //: 콜론없이 해달라해서.
                                            iDataSize = 0;
                                            bParam = Encoding.UTF8.GetBytes(strParam.Replace(":", String.Empty));
                                            for (int p = 0; p < bParam.Length; p++)
                                            {
                                                string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                tmpList.Add(tmpChar);
                                                iDataSize++;
                                            }
                                            break;


                            case "CHAR32":  //32바이트 고정배열
                                            iDataSize = 32;
                                            
                                            bParam = Encoding.UTF8.GetBytes(strParam);

                                            for (int p = 0; p < iDataSize; p++)
                                            {
                                                string tmpChar = "00";
                                                if (p < bParam.Length)
                                                    tmpChar = String.Format("{0:X2}", bParam[p]);
                                                else
                                                    tmpChar = "00";

                                                tmpList.Add(tmpChar);
                                            }
                                            break;

                            case "DCHAR32":  //32바이트 2개짜리 고정배열                                            
                                            if (strParam.Contains(','))
                                            {                                            
                                                string[] strParamEx = strParam.Split(',');
                                                
                                                iDataSize = 64;
                                                byte[] bParam1 = Encoding.UTF8.GetBytes(strParamEx[0]);
                                                byte[] bParam2 = Encoding.UTF8.GetBytes(strParamEx[1]);

                                                for (int p = 0; p < 32; p++)
                                                {
                                                    string tmpChar = "00";
                                                    if (p < bParam1.Length)
                                                        tmpChar = String.Format("{0:X2}", bParam1[p]);
                                                    else
                                                        tmpChar = "00";

                                                    tmpList.Add(tmpChar);
                                                }

                                                for (int p = 0; p < 32; p++)
                                                {
                                                    string tmpChar = "00";
                                                    if (p < bParam2.Length)
                                                        tmpChar = String.Format("{0:X2}", bParam2[p]);
                                                    else
                                                        tmpChar = "00";

                                                    tmpList.Add(tmpChar);
                                                }

                                            }
                                            else
                                            {
                                                tmpList.Add("FF");
                                                strReason = "PAR1 ERROR - USE 2PARAM";
                                                brtnOk = false;
                                            }
                                            
                                            break;

                            case "USIM1":
                            case "USIM2":
                            case "USIM3":
                                            switch (tmpString[i])
                                            {
                                                case "USIM1": tmpList.Add("00"); break;
                                                case "USIM2": tmpList.Add("01"); break;
                                                case "USIM3": tmpList.Add("02"); break;
                                                default: tmpList.Add("00"); break;
                                            }
                                            iDataSize = 1;
                                            bParam = Encoding.UTF8.GetBytes(strParam);
                                            for (int p = 0; p < bParam.Length; p++)
                                            {
                                                string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                tmpList.Add(tmpChar);
                                                iDataSize++;
                                            }
                                            break;

                            case "DTC_TABLE":
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
                                                tmpList.Add("FF");
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                            }
                                            break;                            

                            case "HEX":                                
                                            bOk = true;
                                            bParam = DecToHex(strParam, ref bOk);
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
                                                tmpList.Add("FF");
                                                strReason = "HEX DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
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
                                                tmpList.Add("FF");
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                            }
                                            break;

                            case "INT32":                                           
                                            Int32 i32 = 0;
                                            try
                                            {
                                                i32 = Int32.Parse(strParam);
                                                byte[] bInt32Data = new byte[4];
                                                bInt32Data = BitConverter.GetBytes(i32);
                                                for (int p = 0; p < bInt32Data.Length; p++)
                                                {
                                                    string tmpChar = String.Format("{0:X2}", bInt32Data[p]);
                                                    tmpList.Add(tmpChar);
                                                    iDataSize++;
                                                }
                                            }
                                            catch
                                            {
                                                tmpString[i] = "FF";
                                                strReason = "PAR1 TYPE ERROR";
                                                brtnOk = false;
                                                byte[] bErrorByte = new byte[10];
                                                return bErrorByte;
                                            }

                                            break;

                            case "DOUBLE2": 
                                            if (!strParam.Contains(","))
                                            {
                                                strReason = "PAR1 NO COMMA.";
                                                brtnOk = false;
                                                break;
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
                                                break;
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

                            case "ALDL_HEXA":     //ALDL포멧으로 만들되 파라미터는 HEX형태로 만든다.
                            case "ALDL_ASCII":    //ALDL포멧으로 만들되 파라미터는 아스키로 만든다. 
                            case "ALDL_HEXB":     //ALDL포멧으로 만들되 파라미터는 HEX형태이나 특정 바이트에서 특정비트를 1로 변경후 다시 바이트배열로.
                            case "ALDL_HEXC": //ALDL포멧으로 만들되 파라미터는 HEX형태이나 특정 바이트에서 특정비트를 0로 변경후 다시 바이트배열로.
                                            string strValue = String.Empty;
                                            bOk = true;
                                            bParam = new byte[1];

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA": brtnOk = true; break;
                                                case "ALDL_ASCII": brtnOk = true; break;
                                                case "ALDL_HEXB": brtnOk = MakeToggleAldl(true, strParam, ref strValue, ref strReason); break;
                                                case "ALDL_HEXC": brtnOk = MakeToggleAldl(false, strParam, ref strValue, ref strReason); break;
                                                default: brtnOk = false; break;
                                            }

                                            if (!brtnOk) break;

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA": bParam = MakeMCTM_ALDLType(strParam, ref bOk, ref strReason, false); break;
                                                case "ALDL_ASCII": bParam = MakeMCTM_ALDLType(strParam, ref bOk, ref strReason, true); break;
                                                case "ALDL_HEXB": bParam = MakeMCTM_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                case "ALDL_HEXC": bParam = MakeMCTM_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                default: bOk = false; strReason = "DATA TYPE ERROR(" + tmpString[i] + ")"; break;
                                            }

                                            if (!bOk)
                                            {
                                                tmpList.Add("FF");                                            
                                                brtnOk = false;
                                                break;
                                            }
                                            iDataSize = 0;                                
                                            for (int p = 0; p < bParam.Length; p++)
                                            {
                                                string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                tmpList.Add(tmpChar);
                                                iDataSize++;
                                            }
                                            break;

                            default:
                                        tmpList.Add("FF");
                                        strReason = "DATA TYPE NONE.";
                                        brtnOk = false;
                                        break;
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
                                        if (iDataSize > (int)0xFF) //little endian 방식을 따른다.
                                        {
                                            byte bHByte = (byte)((ushort)iDataSize >> 8);
                                            tmpList.Insert(j, bHByte.ToString("X2"));

                                            byte bLByte = (byte)((ushort)iDataSize & 0xFF);
                                            tmpList.Insert(j, bLByte.ToString("X2"));
                                        }
                                        else
                                        {
                                            tmpList.Insert(j, "00");
                                            tmpList.Insert(j, iDataSize.ToString("X2"));

                                        }

                                    }
                                    catch { }

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

                    int iLenSize = tmpList.Count - 3;

                    if (iLenSize > (int)0xFFFF)
                    {
                        tmpList.Insert(i, "FF");
                        tmpList.Insert(i, "FF");
                    }
                    else
                    {
                        try
                        {
                            if (iLenSize > (int)0xFF)  //MCTM little endian 방식을 따른다.
                            {
                                byte bHByte = (byte)((ushort)iLenSize >> 8);
                                tmpList.Insert(i, bHByte.ToString("X2"));

                                byte bLByte = (byte)((ushort)iLenSize & 0xFF);
                                tmpList.Insert(i, bLByte.ToString("X2"));   
                                                             
                            }
                            else
                            {
                                tmpList.Insert(i, "00");
                                tmpList.Insert(i, iLenSize.ToString("X2"));                                                              
                            }
                        }
                        catch
                        {                            
                            strReason = "MAKE PACKET ERROR1";
                            brtnOk = false;
                            byte[] errValue = new byte[tmpList.Count];
                            return errValue;
                        }

                    }
                }
                
            }

            //3. 체크섬를 찾아 바꾼다.
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (tmpList[i].Equals("<CRC16>"))
                {
                    byte[] tmpArray = new byte[i/*tmpList.Count*/];

                    for (int j = 0; j < i/*tmpArray.Length*/; j++)
                    {//체크섬위치를 찾으면 체크섬 앞의 위치까지를 재정렬한다.
                        try { tmpArray[j] = Convert.ToByte(tmpList[j], 16); }
                        catch (Exception ex) 
                        {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg);
                            tmpArray[j] = 0xFF; 
                        
                        }
                    }

                    tmpList.RemoveAt(i);

                    DK_CHECKSUM DKCHK = new DK_CHECKSUM(); //체크섬!
                    byte bHighByte = 0x00;
                    byte bLowByte = 0x00;

                    DKCHK.CRC16_ENCODE(tmpArray, tmpArray.Length, ref bHighByte, ref bLowByte);

                    //HDLC 로 다시 한번 데이터 변경
                    byte[] tmpData = new byte[tmpArray.Length + 2];

                    for (int idx = 0; idx < tmpArray.Length; idx++)
                    {
                        tmpData[idx] = tmpArray[idx];
                    }

                    tmpData[tmpData.Length - 2] = bLowByte;
                    tmpData[tmpData.Length - 1] = bHighByte;


                    byte[] tmpDecode = DKCHK.CRC16_HDLC(tmpData);
                    tmpList.Clear();
                    for (int iDx = 0; iDx < tmpDecode.Length; iDx++)
                    {
                        tmpList.Add(tmpDecode[iDx].ToString("X2"));
                    }

                    tmpList.Add(DEFRES_ETX.ToString("X2"));
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
                    strReason = "MAKE PACKET ERROR2";
                    brtnOk = false;
                    return rtnValue;
                }


            }
            strSendPack = BitConverter.ToString(rtnValue).Replace("-", " ");
            return rtnValue;
        }

        private bool CertiFileCRC16(byte[] bData, ref byte bHigh, ref byte bLow)
        {
            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;            

            for (int i = 0; i < bData.Length; i++)
            {
                usCRC0 ^= (ushort)(bData[i] & 0xFF);

                for (int j = 0; j < 8; j++)
                {
                    usCRC1 = (ushort)(((int)usCRC0 / 2) & 0x7FFF);

                    if ((int)(usCRC0 & 0x01) == 1)
                    {
                        usCRC0 = (ushort)(usCRC1 ^ 0xc659);
                    }
                    else
                    {
                        usCRC0 = usCRC1;
                    }
                }
            }

            usSum = usCRC0;

            bHigh = (byte)(usSum >> 8);
            bLow = (byte)(usSum & 0xFF);

            return true;

        }

        public byte[] DecToHex(string strDecDataString, ref bool bOk)
        {   //데시멀값을 헥사타입으로 변환하여 저장하는 함수..
            byte[] bHex = new byte[1];
            if (String.IsNullOrEmpty(strDecDataString))
            {
                bOk = false;
                return bHex;
            }
                        
            try
            {
                int iDec = int.Parse(strDecDataString);
                bHex = BitConverter.GetBytes(iDec);
                bOk = true;
            }
            catch
            {
                bOk =  false;
                
            }
            return bHex;            

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

        private bool CheckMCTM_ALDLType(byte[] bSendPack, byte[] bGetData, ref string strRetunData, bool bReadHexa)
        {
            try
            {  
                string strData = String.Empty;         

                STEPMANAGER_VALUE.bMctmALDLData = new byte[bGetData.Length];
                STEPMANAGER_VALUE.bMctmALDLIndex = bSendPack[14]; //13 14

                Array.Copy(bGetData, STEPMANAGER_VALUE.bMctmALDLData, bGetData.Length);

                //여기서 아스키로 변환해야하는지 한바이트만 체크하자.
                bool bIsAscii = false;

                for (int i = 0; i < bGetData.Length; i++)
                {
                    if (bGetData[i] >= 0x61 && bGetData[i] <= 0x7A)
                    {
                        bIsAscii = true; //61 ~ 7a  영문 소문자
                    }

                    if (bGetData[i] >= 0x41 && bGetData[i] <= 0x5A)
                    {
                        bIsAscii = true; //41 ~ 5A  영문 대문자
                    }

                    if (bGetData[i] >= 0x30 && bGetData[i] <= 0x39)
                    {
                        bIsAscii = true;  //30 ~ 39 숫자
                    }

                    if (bGetData[i] >= 0x00 && bGetData[i] <= 0x29)
                    {
                        bIsAscii = false; break;//비아스키
                    }

                    if (bGetData[i] >= 0x7B && bGetData[i] <= 0xFF)
                    {
                        bIsAscii = false; break;//비아스키
                    }
                }

                if (bIsAscii)
                {
                    strData = Encoding.UTF8.GetString(bGetData);
                }
                else
                {
                    strData = BitConverter.ToString(bGetData).Replace("-", "");
                }

                if (bReadHexa) //무조건 hexa 로 읽기
                    strData = BitConverter.ToString(bGetData).Replace("-", "");


                strRetunData = strData;
                strRetunData = strRetunData.Replace("\0", String.Empty);
                return true;
            }
            catch
            {
                STEPMANAGER_VALUE.bMctmALDLData = null;
                STEPMANAGER_VALUE.bMctmALDLIndex = 0x00;
                strRetunData = "CheckMCTM_ALDLType:Exception";
                return false;
            }

        }

        private byte[] MakeMCTM_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {
            byte[] bReturnBytes = new byte[1];
            if (STEPMANAGER_VALUE.bMctmALDLData == null || STEPMANAGER_VALUE.bMctmALDLIndex == 0x00)
            {
                bRes = false;
                return bReturnBytes;
            }

            bReturnBytes = new byte[STEPMANAGER_VALUE.bMctmALDLData.Length + 1]; //+1 는 DID address
                        
            try
            {

                byte bTempBlock = 0x00;                
                byte[] bTempData;

                string[] strParseData = strParam.Split(',');

                if (strParseData.Length != 2)
                {
                    strReason = "PAR1 INVALID : " + strParam;
                    bRes = false;
                    return bReturnBytes;
                }

                try
                {
                    bool bAddresOk = false;
                    byte[] bAddress = HexStringToBytes(strParseData[0], ref bAddresOk);
                    bTempBlock = bAddress[0];
                    //bTempBlock = byte.Parse(strParseData[0]);
                }
                catch 
                {
                    strReason = "ADDRESS OVER";
                    bRes = false;
                    return bReturnBytes;
                }


                if (bTempBlock != STEPMANAGER_VALUE.bMctmALDLIndex)
                {
                    strReason = "RELOAD DID ADDRESS";
                    bRes = false;
                    return bReturnBytes;
                }
                bool bConvOK = false;
                if (bAsciiType)// TRACE CODE 같은것들은 아스키그대로 써야한단다...
                {
                    bTempData = Encoding.UTF8.GetBytes(strParseData[1]);
                    bConvOK = true;
                }
                else
                {
                    bTempData = HexStringToBytes(strParseData[1], ref bConvOK);
                }

                if (bConvOK)
                {
                    if (bTempData.Length > STEPMANAGER_VALUE.bMctmALDLData.Length)
                    {
                        strReason = "PAR1 DATA OVER";
                        bRes = false;
                        return bReturnBytes;
                    }                    

                }
                else
                {
                    strReason = "PAR1 INVALID1 : " + strParam;
                    bRes = false;
                    return bReturnBytes;
                }

                bReturnBytes[0] = bTempBlock;

                Array.Copy(bTempData, 0, bReturnBytes, 1, bTempData.Length);                
                bRes = true;
                return bReturnBytes;
            }
            catch
            {
                strReason = "MakeMCTM_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
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

        private bool MakeToggleAldl(bool bToggleOn, string strParam, ref string strResult, ref string strReason)
        {
            strResult = String.Empty;
            strReason = "SUCCESS";
            ///////////////////////////////////////////////////////
            int iDxByte = 0;
            int iDxBit = 0;
            string[] strParseData = strParam.Split(',');

            if (strParseData.Length != 3)
            {
                strReason = "PAR1 ERROR(Address,ByteIndex,BitIndex) - (" + strParam + ")";
                return false;
            }

            byte bAddress = 0x00;

            try
            {
                bAddress = byte.Parse(strParseData[0]);
            }
            catch 
            {
            	strReason = "ADDRESS ERROR - " + strParseData[0];
                return false;
            }


            if (STEPMANAGER_VALUE.bMctmALDLData == null || STEPMANAGER_VALUE.bMctmALDLIndex != bAddress)
            {
                strReason = "RELOAD ADDRESS";
                return false;
            }

            try
            {
                iDxByte = int.Parse(strParseData[1]);
                iDxBit = int.Parse(strParseData[2]);
            }
            catch
            {
                strReason = "PAR1 ERROR(ByteIndex,BitIndex) - " + strParam + ")";
                return false;
            }     

            if (STEPMANAGER_VALUE.bMctmALDLData.Length < iDxByte && 1 > iDxByte)
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

            byte[] bChangeAldl = new byte[STEPMANAGER_VALUE.bMctmALDLData.Length];
            Array.Copy(STEPMANAGER_VALUE.bMctmALDLData, bChangeAldl, bChangeAldl.Length);

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

       

    }

}
