using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    class DK_ANALYZER_NAD   //GEN10 nad 용. (보성)
    {   
        private const byte DEFRES_ETX  = 0x7E;
        private const string HEX_CHARS = "0123456789ABCDEF";


        private List<TBLDATA0> LstTBLnad = new List<TBLDATA0>();       //NAD 명령 테이블 리스트
        private DK_LOGGER tmpLogger = new DK_LOGGER("PC", false);

        public DK_ANALYZER_NAD()
        {
            tmpLogger.LoadTBL0("NAD.TBL", ref LstTBLnad);
        }

        public int AnalyzePacket(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString)
        {
            try
            {
                //0. VCP 응답패킷이 5바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 5) return (int)STATUS.RUNNING;
                     
                int bChksum = (int)STATUS.TIMEOUT;

                for (int j = 0; j < tmpBytes.Length; j++)
                {        
                    if (tmpBytes[j] == DEFRES_ETX)
                    {  
                        byte[] tmpBuffer = new byte[j+1];

                        Array.Copy(tmpBytes, 0, tmpBuffer, 0, tmpBuffer.Length);

                        bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData);
                        strLogString = BitConverter.ToString(tmpBuffer).Replace("-", " ");
                        if (bChksum == (int)STATUS.OK)
                        {
                            return (int)STATUS.OK;
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
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":NAD:" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                tmpLogger.WriteCommLog("Exception: AnalyzePacket() NAD:", ex.Message, false);
                return (int)STATUS.RUNNING;
            }

        }

        public int AnalyzePacketForReportGnss(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref int iLastIndex)
        {
            byte[] tmpBuffer = new byte[59];

            try
            {
                //0. VCP 응답패킷이 5바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 5) return (int)STATUS.RUNNING;

                int bChksum = (int)STATUS.TIMEOUT;
                int iEndPoint = 0;
                int iStxPoint = 0;

                bool bFind = false;
                

                for (int j = 0; j < tmpBytes.Length; j++)
                {
                    if (!bFind && tmpBytes.Length > j+2 && tmpBytes[j] == 0x4D && tmpBytes[j+1] == 0x03)
                    {
                        bFind = true;
                        iStxPoint = j;
                        if (iStxPoint + j > tmpBytes.Length) 
                            return (int)STATUS.RUNNING;
                        j += 56; 
                        continue;
                    }

                    if (bFind && tmpBytes[j] == DEFRES_ETX)
                    {
                        if (tmpBytes.Length - iStxPoint < 58) 
                            return (int)STATUS.RUNNING;

                        tmpBuffer = new byte[j - iEndPoint];
                        try
                        {
                            Array.Copy(tmpBytes, iStxPoint, tmpBuffer, 0, tmpBuffer.Length);
                        }
                        catch 
                        {
                            return (int)STATUS.RUNNING;
                        }                        

                        bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData);

                        iEndPoint = iLastIndex = j + 1;

                        switch (bChksum)
                        {
                            case (int)STATUS.NG: 
                            case (int)STATUS.CHECK:                                            
                                            return (int)STATUS.RUNNING;
                            default: break; //return (int)STATUS.RUNNING;
                        }
                        bFind = false;
                    }
                    
                }
                
                return (int)STATUS.OK;
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":NAD:" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                tmpLogger.WriteCommLog("Exception: AnalyzePacket() NAD:", ex.Message, false);
                return (int)STATUS.RUNNING;
            }

        }


        public PACKTYPE GetNadResFormat(List<TBLDATA0> lstTBLDB, string strCommandName, ref bool bFind, ref int iDataIndex, ref int iLength, ref byte[] bytesOriginPack, ref int iIndexFormat)
        {
            PACKTYPE iResult = new PACKTYPE();
            iResult.iType = (int)NADRESTYPE.NONE;
            iResult.iPos = -1;
            bFind = false;
            try
            {
                for (int i = 0; i < lstTBLDB.Count; i++)
                {
                    if (lstTBLDB[i].CMDNAME.Equals(strCommandName))
                    {
                        string[] strtmpPacks = System.Text.RegularExpressions.Regex.Split(lstTBLDB[i].SENDPAC, " ");
                        bytesOriginPack = new byte[strtmpPacks.Length];
                        for (int j = 0; j < bytesOriginPack.Length; j++)
                        {
                            if (!strtmpPacks[j].Contains("<") && strtmpPacks[j].Length == 2)
                            {
                                try
                                {
                                    bytesOriginPack[j] = Convert.ToByte(strtmpPacks[j], 16);
                                }
                                catch { }                                
                            }
                            
                        }
                        
                        switch (lstTBLDB[i].PARPAC1)
                        {
                            case "SWVERSION": iResult.iType = (int)NADRESTYPE.SWVERSION;     
                                iDataIndex = 39;
                                iLength = 8;
                                iIndexFormat = 1;
                                break;
                            case "SWCOMFILEDATE": iResult.iType = (int)NADRESTYPE.SWCOMFILEDATE;
                                iDataIndex = 1;
                                iLength = 11;
                                iIndexFormat = 1;
                                break;
                            case "HWREVISION": iResult.iType = (int)NADRESTYPE.HWREVISION;     
                                iDataIndex = 4;
                                iLength = 4;
                                iIndexFormat = 4;
                                break;
                            case "MDMCHIPVER": iResult.iType = (int)NADRESTYPE.MDMCHIPVER;     
                                iDataIndex = 4;
                                iLength = 4;
                                iIndexFormat = 1;                                
                                break;
                            case "EXTCHAR": iResult.iType = (int)NADRESTYPE.EXTCHAR;      
                                iDataIndex = 3;
                                iLength = 128; //EXT CHAR 는 전부 다읽는다.
                                iIndexFormat = 3;                                
                                break;
                            case "EXTINT32": iResult.iType = (int)NADRESTYPE.EXTINT32;
                                iDataIndex = 3;
                                iLength = 4; 
                                iIndexFormat = 3;
                                break;
                            case "NV946": iResult.iType = (int)NADRESTYPE.NV946;
                                iDataIndex = 3;
                                iLength = 3;
                                iIndexFormat = 3;
                                break;
                            case "EXTDEFAULT": iResult.iType = (int)NADRESTYPE.EXTDEFAULT; 
                                iDataIndex = 3;
                                iLength = 128;  //EXTDEFAULT 는 전부 다읽는다.
                                iIndexFormat = 135;
                                break;
                            case "STDCHAR": iResult.iType = (int)NADRESTYPE.STDCHAR;
                                iDataIndex = 4;
                                iLength = 1;  //STDCHAR 도 전부 다읽기는 하지만 전체사이즈를 모르기때문에 1로 설정해두고 전부 다읽는다.
                                iIndexFormat = 4;
                                break;
                            case "QFUSE_NORMAL": iResult.iType = (int)NADRESTYPE.QFUSE_NORMAL;   
                                iDataIndex = 5;
                                iLength = 1;  //STDCHAR 도 전부 다읽기는 하지만 전체사이즈를 모르기때문에 1로 설정해두고 전부 다읽는다.
                                iIndexFormat = 3;                                
                                break;
                            case "DTC_RF": iResult.iType = (int)NADRESTYPE.DTC_RF;     
                                iDataIndex = 1;
                                iLength = 1;  
                                iIndexFormat = 1;                                
                                break;
                            case "DTC_GPS": iResult.iType = (int)NADRESTYPE.DTC_GPS; 
                                iDataIndex = 2;
                                iLength = 1;
                                iIndexFormat = 1;
                                break;
                            case "STDDEFAULT": iResult.iType = (int)NADRESTYPE.STDDEFAULT;
                                iDataIndex = 0;
                                iLength = 1;  //STDCHAR 도 전부 다읽기는 하지만 전체사이즈를 모르기때문에 1로 설정해두고 전부 다읽는다.
                                iIndexFormat = 4;
                                break;

                            case "GPS_NORMAL": iResult.iType = (int)NADRESTYPE.GPS_NORMAL;
                                iDataIndex = 2;
                                iLength = 1;
                                iIndexFormat = 4;
                                break;

                            case "GPS_INFO": iResult.iType = (int)NADRESTYPE.GPS_INFO;
                                iDataIndex = 2;
                                iLength = 1;
                                iIndexFormat = 2;
                                break;

                            case "MODECHANGE": iResult.iType = (int)NADRESTYPE.MODECHANGE;
                                iDataIndex = 0;
                                iLength = 1;  
                                iIndexFormat = 7;
                                break;

                            case "NORESPONSE": iResult.iType = (int)NADRESTYPE.NORESPONSE; break;

                            //GEN9
                            case "GEN9IMEI": iResult.iType = (int)NADRESTYPE.GEN9IMEI;
                                iDataIndex = 3;
                                iLength = 128; 
                                iIndexFormat = 3;
                                break;

                            default: iResult.iType = (int)NADRESTYPE.NONE; break;
                        }

                        bFind = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: GetNadResFormat :", ex.Message, false);
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                bFind = false;
            }  

            return iResult;
        }

        public string GetNadCmdType(List<TBLDATA0> lstTBLDB, string strCommandName)
        {
            try
            {
                for (int i = 0; i < lstTBLDB.Count; i++)
                {
                    if (lstTBLDB[i].CMDNAME.Equals(strCommandName))
                    {                        
                        switch (lstTBLDB[i].RECVPAC)
                        {
                            case "EXT":
                            case "STD": return lstTBLDB[i].RECVPAC;
                            default: return "NONE";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: GetNadCmdType :", ex.Message, false);
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                
            }
            return "NONE";
            
        }
        
        public int CheckProtocol(byte[] tmpByteArray, byte[] strOrginSendDatas, string strCommandName, ref string rtnData)
        {            
            //1. 체크섬 검사.

            DK_CHECKSUM DKCHK = new DK_CHECKSUM();
            byte bChkHigh = 0x00;
            byte bChkLow  = 0x00;
            byte[] tmpDecode = DKCHK.CRC16_DECODE(tmpByteArray);
            DKCHK.CRC16_ENCODE(tmpDecode, tmpDecode.Length-3, ref bChkHigh, ref bChkLow);

            if (tmpDecode[tmpDecode.Length - 3] != bChkLow ||
                tmpDecode[tmpDecode.Length - 2] != bChkHigh)
            {
                rtnData = "CHECKSUM NG";
                return (int)STATUS.NG;
            }


            try
            {                   
                //4. 데이터 수집 구간 시작------------------------------------------
                bool bNadRes = false;
                int iDataIndex = 0;
                int iDataLength = 0;
                byte[] bytesOriginFormat = new byte[255];
                int iIndexFormat = 0;
                PACKTYPE nadp = GetNadResFormat(LstTBLnad, strCommandName, ref bNadRes, ref iDataIndex, ref iDataLength, ref bytesOriginFormat, ref iIndexFormat);
                
                if (bNadRes)
                {
                    byte[] strOrginSendData2 = DKCHK.CRC16_DECODE(tmpByteArray);
                    switch (nadp.iType)
                    {
                        case (int)NADRESTYPE.NORESPONSE: //NORESPONSE 형태는 응답이 없다. 물론 여기까지 탈일은 없다... 그러므로 절차서에서 SEND 옵션으로해야한다.
                                                    rtnData = ""; return (int)STATUS.OK;

                        case (int)NADRESTYPE.STDDEFAULT: //STDDEFAULT 형태는 응답이 보낸것와 완전히 일치한다. 
                                            for (int i = 0; i < strOrginSendData2.Length; i++)
                                            {
                                                if (strOrginSendData2[i] != tmpDecode[i])
                                                {
                                                    return (int)STATUS.RUNNING;
                                                }
                                            } 
                                            return (int)STATUS.OK;

                        case (int)NADRESTYPE.EXTDEFAULT: //EXTDEFAULT 형태는 응답이 보낸것와 완전히 일치한다. 
                                            for (int i = 0; i < iIndexFormat; i++)
                                            {
                                                if (strOrginSendData2[i] != tmpDecode[i])
                                                {
                                                    return (int)STATUS.RUNNING;
                                                }
                                            } break;

                        case (int)NADRESTYPE.GPS_INFO:
                                            //GPS REPORT 형태는 두개만 확인한다.

                                            for (int i = 0; i < iIndexFormat; i++)
                                            {
                                                if (bytesOriginFormat[i] != tmpDecode[i])
                                                {
                                                    return (int)STATUS.RUNNING;
                                                }
                                            } break;
                        default:            //나머지 형태는 보낸 데이터에 대한 원하는 응답인지만 확인한다.
                            
                                            for (int i = 0; i < iIndexFormat; i++)
                                            {
                                                if (bytesOriginFormat[i] != tmpDecode[i])
                                                {
                                                    return (int)STATUS.RUNNING;
                                                }
                                            } break;
                    }
                               

                    if (iDataLength < 1 || tmpDecode.Length < iDataIndex + iDataLength + 2)
                    {
                        rtnData = "NO DATA";
                        return (int)STATUS.NG;
                    }

                    switch (nadp.iType)
                    {
                        case (int)NADRESTYPE.SWVERSION:
                        case (int)NADRESTYPE.SWCOMFILEDATE:
                                                    rtnData = Encoding.UTF8.GetString(tmpDecode, iDataIndex, iDataLength);
                                                    break;
                        case (int)NADRESTYPE.HWREVISION:
                                                    Int16 iHrev = BitConverter.ToInt16(tmpDecode, iDataIndex);
                                                    Int16 iLrev = BitConverter.ToInt16(tmpDecode, iDataIndex+2);
                                                    rtnData = iHrev.ToString() + "." + iLrev.ToString();
                                                    break;
                        case (int)NADRESTYPE.MODECHANGE:
                                                    if (tmpDecode[iIndexFormat].Equals(0x01))// 01 success 00 fail
                                                    {
                                                        rtnData = "01";
                                                        return (int)STATUS.OK;
                                                    }
                                                    else
                                                    {
                                                        rtnData = tmpDecode[iIndexFormat].ToString("X2");
                                                        return (int)STATUS.NG;
                                                    }
                        case (int)NADRESTYPE.MDMCHIPVER:
                        case (int)NADRESTYPE.EXTINT32:                                                    
                                                    Int32 iData = BitConverter.ToInt32(tmpDecode, iDataIndex);
                                                    rtnData = iData.ToString("X2").PadLeft(8, '0');                                                    
                                                    break;
                        case (int)NADRESTYPE.NV946:
                                                    rtnData = BitConverter.ToString(tmpDecode, iDataIndex, iDataLength).Replace("-", "");
                                                    break;

                        case (int)NADRESTYPE.EXTCHAR:
                        case (int)NADRESTYPE.EXTDEFAULT:
                                                    rtnData = Encoding.UTF8.GetString(tmpDecode, iDataIndex, tmpDecode.Length - iDataIndex - 5); //ext의 경우에는 맨뒤의 status 2바이트도 빼자.
                                                    rtnData = rtnData.Replace("\0", String.Empty);
                                                    break;
                        case (int)NADRESTYPE.STDCHAR:
                                                    rtnData = Encoding.UTF8.GetString(tmpDecode, iDataIndex, tmpDecode.Length - iDataIndex - 3); //ext의 경우에는 맨뒤의 status 2바이트가없기때문에 3만 뺌.
                                                    rtnData = rtnData.Replace("\0", String.Empty);
                                                    break;
                        case (int)NADRESTYPE.QFUSE_NORMAL:
                                                    byte byteVal = tmpDecode[iDataIndex];
                                                    rtnData = byteVal.ToString("X2");
                                                    break;
                        case (int)NADRESTYPE.DTC_RF:
                        case (int)NADRESTYPE.DTC_GPS:
                                                    int iAdcValue = (int)tmpDecode[iDataIndex];
                                                    rtnData = iAdcValue.ToString();
                                                    break;
                        case (int)NADRESTYPE.GPS_NORMAL:
                                                    if (tmpDecode.Length.Equals(59))
                                                    {
                                                        if (tmpDecode[tmpDecode.Length - 5].Equals(0x00) && tmpDecode[tmpDecode.Length - 4].Equals(0x00))
                                                        {
                                                            rtnData = "SUCCESS";
                                                            return (int)STATUS.OK;
                                                        }
                                                        else
                                                        {
                                                            rtnData = "STATE:" + tmpDecode[tmpDecode.Length - 5].ToString("X2") + tmpDecode[tmpDecode.Length - 4].ToString("X2");
                                                            return (int)STATUS.NG;
                                                        }
                                                    }
                                                    rtnData = "INVALID RESPONSE";
                                                    return (int)STATUS.NG;

                        case (int)NADRESTYPE.GPS_INFO:
                                                    if (tmpDecode.Length.Equals(59))
                                                    {
                                                        STEPMANAGER_VALUE.SetGEN11CCMGPSInfo(tmpDecode);
                                                        return (int)STATUS.OK;
                                                    }
                                                    return (int)STATUS.RUNNING;

                        //GEN9
                        case (int)NADRESTYPE.GEN9IMEI:

                                                    
                                                    if (tmpDecode[iDataIndex] > 0x10 || tmpDecode[iDataIndex] < 0x08)
                                                    {
                                                        rtnData = "IMEI SIZE ERROR";
                                                        return (int)STATUS.NG; 
                                                    }
                                                    byte[] bImeiBytes = new byte[tmpDecode[iDataIndex]];
                                                    Array.Copy(tmpDecode, iDataIndex + 1, bImeiBytes, 0, bImeiBytes.Length);
                                                    
                                                    rtnData = String.Empty;

                                                    for (int i = 0; i < bImeiBytes.Length; i++)
                                                    {
                                                        rtnData += new string(bImeiBytes[i].ToString("X2").Reverse().ToArray());
                                                    }

                                                    rtnData = rtnData.Substring(1, 14);                                                    
                                                    rtnData = rtnData.Replace("\0", String.Empty);
                                                    break;

                        default: 
                                                    rtnData = "Unknown Response Format";
                                                    return (int)STATUS.NG; 
                    }

                    return (int)STATUS.OK;
                }
                else
                {
                    rtnData = "Unknown Response Format";
                    return (int)STATUS.NG; 
                }
                

            }
            catch (Exception ex)
            {
                rtnData = "CheckProtocol Exception2:" + ex.Message.ToString();
                return (int)STATUS.CHECK;
            }

        }
              
        public byte[] ConvertNadByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason, string strCommandName)
        { //strSendPack은 TX에서 로깅하기 위해사용한다. rx에서는 따로 해주어야한다.
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();
            strReason = "OK";
            string strValue = String.Empty;
            tmpList.Clear();
            bool bOk = true;
            byte[] rtnNull = new byte[1];  //FAIL 리턴용

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
                            default:                                          
                                            strReason = "DATA TYPE NONE.";
                                            brtnOk = false;
                                            return rtnNull;
                                          
                        }
                    }
                }
                else
                {
                    tmpList.Add(tmpString[i]);

                }
                
            }

            string strPackType = GetNadCmdType(LstTBLnad, strCommandName);
            //2.1 strPackType 타입에 따라 패킷을 확장한다.
            if (strPackType.Equals("EXT"))
            {
                int iDx = tmpList.Count - 2;
                int iAddCount = 136 - (tmpList.Count + 1);
                for (int j = 0; j < iAddCount; j++)
                {
                    tmpList.Insert(iDx, "00");
                }
            }

            //3. 체크섬를 찾아 바꾼다.
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (tmpList[i].Equals("<CRC16>"))
                {
                    byte[] tmpArray = new byte[tmpList.Count];

                    for (int j = 0; j < i; j++)
                    {//체크섬위치를 찾으면 체크섬 앞의 위치까지를 재정렬한다.
                        try { tmpArray[j] = Convert.ToByte(tmpList[j], 16); }
                        catch (Exception ex) 
                        {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg);
                            tmpArray[j] = 0xFF; 
                        }
                    }
                    
                    DK_CHECKSUM DKCHK = new DK_CHECKSUM(); //체크섬!
                    byte bHighByte = 0x00;
                    byte bLowByte = 0x00;

                    DKCHK.CRC16_ENCODE(tmpArray, i, ref bHighByte, ref bLowByte);
                    
                    tmpList.RemoveAt(i);
                    tmpList.Insert(i, bHighByte.ToString("X2"));
                    tmpList.Insert(i, bLowByte.ToString("X2"));   //LITTLE ENDIAN 
                    
                    //HDLC 로 다시 한번 데이터 변경

                    byte[] tmpData = new byte[tmpList.Count-1];

                    for (int idx = 0; idx < tmpData.Length; idx++)
                    {
                        tmpData[idx] = Convert.ToByte(tmpList[idx], 16); ;
                    }

                    byte[] tmpDecode = DKCHK.CRC16_HDLC(tmpData);
                    tmpList.Clear();
                    for (int iDx = 0; iDx < tmpDecode.Length; iDx++)
                    {
                        tmpList.Add(tmpDecode[iDx].ToString("X2"));
                    }

                    tmpList.Add(DEFRES_ETX.ToString("X2"));
                    break;
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
