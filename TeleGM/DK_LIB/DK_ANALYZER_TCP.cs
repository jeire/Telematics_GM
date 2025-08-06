using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    class DK_ANALYZER_TCP
    {        
        private const byte DEFRES_STX1 = 0x02;
        private const byte DEFRES_STX2 = 0xFB;
        private const byte DEFRES_ETX  = 0xFA;
        private const string HEX_CHARS = "0123456789ABCDEF";

        private List<TBLDATA0> LstTBLtcp = new List<TBLDATA0>();       //GEN10 명령 테이블 리스트
        private DK_LOGGER tmpLogger = new DK_LOGGER("PC", false);
    
        public DK_ANALYZER_TCP()
        {            
            tmpLogger.LoadTBL0("TCP.TBL", ref LstTBLtcp);
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
                            case "SERVICE":    iResult.iType = (int)TCPRESTYPE.SERVICE; break;

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
                tmpLogger.WriteCommLog("Exception: GetTcpResFormat :", ex.Message, false);
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                bFind = false;
            }  

            return iResult;
        }
   
        private bool CheckTCP_DtcIndex(byte bSendParam, byte[] bGetData, ref string strRetunData)
        {            
            try
            {
                // 받은 데이터 길이 체크
                if (bGetData.Length != 4) return false;
                //여기서는 요청한 DTC 항목이  응답받은 DTC 항목이 맞는지 체크한다.
                int iDx = (int)bSendParam;
                string strDtcName = tmpLogger.LoadDtcIndexINI("DTC" + iDx.ToString(), "NO1");
                string strDtcCode = tmpLogger.LoadDtcIndexINI("DTC" + iDx.ToString(), "NO2");
                string strDtcType = tmpLogger.LoadDtcIndexINI("DTC" + iDx.ToString(), "NO3");
                string strDtcDesc = tmpLogger.LoadDtcIndexINI("DTC" + iDx.ToString(), "NO4");
                // 받은 데이터 Little Endian 바로 정렬                        
                string strGetDataDtcCode = bGetData[1].ToString("X2").ToUpper() + bGetData[0].ToString("X2").ToUpper();
                string strGetDataDtcType = bGetData[2].ToString("X2").ToUpper();

                if (strDtcName.Equals("0") || strDtcCode.Equals("0") || strDtcType.Equals("0"))
                {
                    strRetunData = "NO DTC TABLE.";
                    return false;
                }

                strRetunData = String.Empty;
                //1. DTC 테이블에 정의된 내용과 같은지 비교
                if (!strDtcCode.Equals(strGetDataDtcCode) || !strDtcType.Equals(strGetDataDtcType))
                {
                    strRetunData = "ERROR DTC CODE & TYPE";
                    return false;
                }
                //8비트로 나타내는 방법 (DQA 담당자 요청방식)
                //strRetunData = Convert.ToString(bGetData[3], 2).PadLeft(8, '0');

                //2번째 비트만 나타내는 방법 (현우씨 요청방식)
                //strRetunData = strRetunData.Substring(6, 1);

                //바이트만 그대로 나타내는 방법 (GEN10과 동일방식 - 20160229)
                strRetunData = bGetData[3].ToString("X2");

                return true;
            }
            catch
            {
                strRetunData = "CheckTCP_DtcIndex:Exception";
                return false;
            }            
            
        }

        private bool CheckTCP_ALDLType(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
        {
            try
            {
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[150]; 150byte
                // 4. unsigned char  lflag[150];150byte
                if (bGetData.Length != 303) return false;

                string strData1 = String.Empty;
                string strData2 = String.Empty;
                string strData3 = String.Empty;
                string strData4 = String.Empty;                                

                byte[] bData1 = new byte[2];
                byte[] bData2 = new byte[1];
                byte[] bData3 = new byte[150];
                byte[] bData4 = new byte[150];

                Array.Copy(bGetData, 0, bData1, 0, bData1.Length);
                Array.Copy(bGetData, 2, bData2, 0, bData2.Length);
                Array.Copy(bGetData, 3, bData3, 0, bData3.Length);
                Array.Copy(bGetData, 153, bData4, 0, bData4.Length);


                //1. ALDL 블럭체크
                if (bSendPack[12] != bData1[0] || bSendPack[13] != bData1[1])
                {
                    return false;
                }

                strData1 = BitConverter.ToString(bData1).Replace("-", "");
                strData2 = ((int)bData2[0]).ToString();

                //여기서 아스키로 변환해야하는지 한바이트만 체크하자.
                bool bIsAscii = false;

                for (int i = 0; i < (int)bData2[0]; i++)
                {
                    if (bData3[i] >= 0x61 && bData3[i] <= 0x7A)
                    {
                        bIsAscii = true; //61 ~ 7a  영문 소문자
                    }

                    if (bData3[i] >= 0x41 && bData3[i] <= 0x5A)
                    {
                        bIsAscii = true; //41 ~ 5A  영문 대문자
                    }

                    if (bData3[i] >= 0x30 && bData3[i] <= 0x39)
                    {
                        bIsAscii = true;  //30 ~ 39 숫자
                    }

                    if (bData3[i] >= 0x00 && bData3[i] <= 0x29)
                    {
                        bIsAscii = false; break;//비아스키
                    }

                    if (bData3[i] >= 0x7B && bData3[i] <= 0xFF)
                    {
                        bIsAscii = false; break;//비아스키
                    }
                }

                if (bIsAscii)
                {
                    strData3 = Encoding.UTF8.GetString(bData3);
                }
                else
                {
                    strData3 = BitConverter.ToString(bData3, 0, (int)bData2[0]).Replace("-", "");
                }
                   
                strData4 = BitConverter.ToString(bData4).Replace("-", "");
                strData4 = strData4.Replace("00", "");

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

        public byte[] MakeEfileStruct(int iEfileIdx, string strParam, ref bool bRes, ref string strReason)
        {
            byte[] bReturnBytes;
            byte[] bState       = new byte[4];      //eTRANSFERFILE ( check, transfer, complete )
            byte[] bFileName    = new byte[64];      
            byte[] bFilePath    = new byte[256];
            byte[] bFileSize    = new byte[4];      //uint C# (unsigned long C++)
            byte[] bDataSize    = new byte[2];
            byte[] bRealData    = new byte[1024];

            int iFuncBytes = bState.Length + bFileName.Length + bFilePath.Length + bFileSize.Length + bDataSize.Length;

            string strFileName;
            string strFilePath;
            string strRealData;
            
            //Step0. 파라미터 체크
            string[] strTempSpl = strParam.Split(',');
            if (strTempSpl.Length != 3)
            {
                strReason = "Par1 Error - Nothing Comma(,) : " + strParam;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }
            //strTempSpl[0]: 파일경로   strTempSpl[1]: 파일이름   strTempSpl[2]: KIS로부터 받은 데이터
            if (strTempSpl[0].Length < 1 || strTempSpl[1].Length < 1 || strTempSpl[2].Length < 1)
            {
                strReason = "Par1 Error : " + strParam;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            //KIS 데이터를 끌고오기위해 여기서 할것이아니라 현재 함수를 호출하는 부분에서 KIS데이터를 파라미터로 전달처리해야한다.
            switch (strTempSpl[2])
            {
                case "KIS_error_code":    strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode]; break;
                case "KIS_error_message": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg]; break;
                case "KIS_stid":          strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID];  break;
                case "KIS_rCert":         strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT]; break;
                case "KIS_ccCert":        strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert]; break;
                case "KIS_vCert":         strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert]; break;
                case "KIS_vPri":          strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri];  break;
                case "KIS_vPre":          strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre];  break;
                case "KIS_vAuth":         strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth]; break;
                case "KIS_vHash":         strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash]; break;
                default:
                    strReason = "KIS Data Type Error : " + strTempSpl[2];
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
            }

            
            /*//TEST DATA
            switch (strTempSpl[2])
            {
                case "KIS_error_code":    strRealData = "0"; break;
                case "KIS_error_message": strRealData = ""; break;
                case "KIS_stid":   strRealData  = "117605004"; break;
                case "KIS_rCert":  strRealData  = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; break;
                case "KIS_ccCert": strRealData  = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; break;
                case "KIS_vCert":  strRealData  = "-----BEGIN CERTIFICATE-----MIIBujCCAV+gAwIBAgIIBWTJeeT/AgEwCgYIKoZIzj0EAwIwMzEPMA0GA1UECwwGT05TVEFSMQ8wDQYDVQQKDAZPTlNUQVIxDzANBgNVBAMMBk9OU1RBUjAgFw0xNTExMTgxNTIyMDZaGA8yMTAwMTIzMTIzNTk1OVowKjEPMA0GA1UECgwGT25TdGFyMRcwFQYDVQQDDA5TVElEIDExNzYwNTAwNDBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABGxAbIISqlpQMn2+K4BBm6rgf2pt7bIMxTSUNXZQLtW3Cli6BaaA0f0kLabEmRdNp/RdE2G0RUzYuG5DkYjm9AWjZDBiMA4GA1UdDwEB/wQEAwIDiDAgBgNVHSUBAf8EFjAUBggrBgEFBQcDAgYIKwYBBQUHAwgwDwYDVR0TAQH/BAUwAwEBADAdBgNVHQ4EFgQULG2JCDeehwQr8SIMdH4TdU7weQYwCgYIKoZIzj0EAwIDSQAwRgIhAJ70eFf7zQveI+SNBh/OP6hL2y999y4okIRPA9TMtAlhAiEAvYuQccqnAQCc7/H2Bur0e3IB8WOati9sqycXjXFiJoo=-----END CERTIFICATE-----"; 
                                   break;
                case "KIS_vPri": strRealData = "-----BEGIN PRIVATE KEY-----MEECAQAwEwYHKoZIzj0CAQYIKoZIzj0DAQcEJzAlAgEBBCDaiDof6CER8Soa9oV5aCDoNybflPiWLB25vNujvJZwkw==-----END PRIVATE KEY-----"; 
                                  break;
                case "KIS_vPre": strRealData = "-----BEGIN PRE-SHARED KEY DATA-----l1kTC9OeZBSiex2GtLTQQHmw/vzcRLJNVaC17gUxQE9OCJPLJb91I6xk7/yx8oyFh2oB3PyLR61akbSAhMmLTw==-----END PRE-SHARED KEY DATA----------BEGIN PSK IDENTIFIER DATA-----117605004-----END PSK IDENTIFIER DATA----------BEGIN PSK HINT-----OnStar PoC-----END PSK HINT----------BEGIN PSK ID-----1-----END PSK ID-----"; break;
                case "KIS_vAuth": strRealData = "35823FFDD6EB5C53"; break;
                case "KIS_vHash": strRealData = "2d9d3bff4ef836732a618d85851df39753cb9ff9"; break;
                default:
                    strReason = "KIS Data Type Error : " + strTempSpl[2];
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
            }
            */


            if (String.IsNullOrEmpty(strRealData)) 
            {
                strReason    = "KIS Data Empty : " + strRealData;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }
            strFilePath = strTempSpl[0] + "/" + strTempSpl[1];
            strFileName = strTempSpl[1];
            

            //Step1. FileName 만들기           
            byte[] bTempFileName = Encoding.UTF8.GetBytes(strFileName);
            if(bTempFileName.Length >= bFileName.Length)
            {
                strReason = "FileNameSize Error : " + strFileName;
                bReturnBytes = new byte[iFuncBytes]; 
                bRes = false; 
                return bReturnBytes;
            }
            Array.Copy(bTempFileName, bFileName, bTempFileName.Length);
            
            //Step2. FilePath 만들기
            byte[] bTempFilePath = Encoding.UTF8.GetBytes(strFilePath);
            if (bTempFilePath.Length >= bFilePath.Length)
            {
                strReason = "FileNameSize Error : " + strFilePath;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }
            Array.Copy(bTempFilePath, bFilePath, bTempFilePath.Length);
           

            switch (iEfileIdx)
            {
                case (int)EFILETYPE.CHECK:    
                                              bState[0] = (byte)EFILETYPE.CHECK;
                                              break;

                case (int)EFILETYPE.TRANSFER: //Step3. file & data Size 만들기
                                              bState[0] = (byte)EFILETYPE.TRANSFER;

                                              bRealData = Encoding.UTF8.GetBytes(strRealData);
                                              if (bRealData.Length >= 1024)
                                              {
                                                  strReason = "DataFileSize OverFlow : " + bRealData.Length.ToString() + " Bytes";
                                                  bReturnBytes = new byte[iFuncBytes];
                                                  bRes = false;
                                                  return bReturnBytes;
                                              }
                                              bFileSize = BitConverter.GetBytes(bRealData.Length);
                                              bDataSize = BitConverter.GetBytes((ushort)bRealData.Length);
                                              iFuncBytes += bRealData.Length;                     
                                              break;

                case (int)EFILETYPE.COMPLETE: 
                                              bState[0] = (byte)EFILETYPE.COMPLETE;
                                              bRealData = Encoding.UTF8.GetBytes(strRealData);
                                              if (bRealData.Length >= 1024)
                                              {
                                                  strReason = "DataFileSize OverFlow : " + bRealData.Length.ToString() + " Bytes";
                                                  bReturnBytes = new byte[iFuncBytes];
                                                  bRes = false;
                                                  return bReturnBytes;
                                              }
                                              bFileSize = BitConverter.GetBytes(bRealData.Length);
                                              break;

                default:    strReason = "EFILETYPE Error";
                            bReturnBytes = new byte[iFuncBytes]; 
                            bRes = false; 
                            return bReturnBytes;
            }

            bReturnBytes = new byte[iFuncBytes];

            int iPos = 0;

            try
            {
                //## 구조체 채워넣기 
                Array.Copy(bState, 0,    bReturnBytes, iPos, bState.Length);
                iPos += bState.Length;

                Array.Copy(bFileName, 0, bReturnBytes, iPos, bFileName.Length);
                iPos += bFileName.Length;

                Array.Copy(bFilePath, 0, bReturnBytes, iPos, bFilePath.Length);
                iPos += bFilePath.Length;

                Array.Copy(bFileSize, 0, bReturnBytes, iPos, bFileSize.Length);
                iPos += bFileSize.Length;

                Array.Copy(bDataSize, 0, bReturnBytes, iPos, bDataSize.Length);
                iPos += bDataSize.Length;

                if (bState[0] == (byte)EFILETYPE.TRANSFER)
                {                    
                    Array.Copy(bRealData, 0, bReturnBytes, iPos, bRealData.Length);
                    iPos += bRealData.Length;
                }
        
            }
            catch
            {
                strReason = "Make Struct Error";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }
            
            return bReturnBytes;

        }

        private byte[] MakeTCP_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {
            byte[] bReturnBytes = new byte[303];
            try
            {
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[150]; 150byte
                // 4. unsigned char  lflag[150];150byte

                byte[] bDataBlock = new byte[2];
                byte[] bDataLen   = new byte[1];
                byte[] bDataData  = new byte[150];
                byte[] bDataFlag  = new byte[150];

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

                bDataLen[0] = (byte)bTempData.Length;
                if (bConvOK1 && bConvOK2)
                {
                    Array.Copy(bTempBlock, 0, bDataBlock, 0, bTempBlock.Length);
                    Array.Copy(bTempData,  0, bDataData,  0, bTempData.Length);

                    for (int i = 0; i < (int)bDataLen[0]; i++)
                    {
                        bDataFlag[i] = 0xFF;
                    }

                }
                else
                {
                    strReason = "PAR1 INVALID1 : " + strParam;
                    bRes = false;
                    return bReturnBytes;
                }
                   
                Array.Copy(bDataBlock,  0, bReturnBytes,   0, bDataBlock.Length);
                Array.Copy(bDataLen,    0, bReturnBytes,   2, bDataLen.Length);
                Array.Copy(bDataData,   0, bReturnBytes,   3, bDataData.Length);
                Array.Copy(bDataFlag,   0, bReturnBytes, 153, bDataFlag.Length);

                bRes = true;
                return bReturnBytes;
            }
            catch
            {
                strReason = "MakeTCP_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
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
                //int iDataLen = (int)tmpByteArray[5];
                int iDataLen = (int)tmpByteArray[12] + ((int)tmpByteArray[13] * (int)0x100);
                bool bTcppRes = false;
                PACKTYPE tcpp = GetTcpResFormat(LstTBLtcp, strCommandName, ref bTcppRes);

                byte[] bData = new byte[iDataLen];
                try
                {
                    for (int i = 0; i < bData.Length; i++)
                    {
                        bData[i] = tmpByteArray[i + 14];
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
                                                    rtnData = Encoding.UTF8.GetString(bData);
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
                                                    Single si = BitConverter.ToSingle(bData, 0);
                                                    rtnData = si.ToString();
                                                    break;
                        case (int)TCPRESTYPE.DTC:   
                                                    if(!CheckTCP_DtcIndex(strOrginSendData[12], bData, ref rtnData))
                                                    {                                                      
                                                        return (int)STATUS.NG;   
                                                    }
                                                    break;

                        case (int)TCPRESTYPE.DTCALL:

                                                    rtnData = Encoding.UTF8.GetString(bData).Replace("-", String.Empty);

                                                    break;

                        case (int)TCPRESTYPE.ALDL_ASCII:
                                                    if (!CheckTCP_ALDLType(strOrginSendData, bData, ref rtnData))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }                                                    
                                                    break;
                        case (int)TCPRESTYPE.ALDL_BITS:
                                                    if (!CheckTCP_ALDLType(strOrginSendData, bData, ref rtnData))
                                                    {
                                                        return (int)STATUS.NG;
                                                    }                                                                                                        
                                                    string strParam = rtnData;
                                                    rtnData = ChangeBitsArray(strParam);
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
                                                        //rtnData += bData[i].ToString("X2");
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

                        case (int)TCPRESTYPE.SERVICE:
                                                    if (!CheckNAD_ServiceTypeA(strOrginSendData, bData, ref rtnData))
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
                

                //Success or Failur 구분 코드.
                if (tmpByteArray[11] == 0x01)
                {
                    return (int)STATUS.OK;
                }
                else
                {
                    switch (strCommandName) //특정 명령 예외처리 구간.... 이런거 진짜 하기 싫다.
                    {
                        case "LOG_STOP":
                        case "LOG_START":
                        case "OOB_SELF_TEST": return (int)STATUS.OK;
             
                        default:
                            rtnData = "RESULT CODE:FAILURE.";
                            return (int)STATUS.NG; 
                    }
                                   
                }
                 

            }
            catch (Exception ex)
            {
                rtnData = "CheckProtocol Exception2:" + ex.Message.ToString();
                return (int)STATUS.CHECK;
            }          


        }

        //3R1Zf 용
        private bool CheckNAD_ServiceTypeA(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
        {
            try
            {
                byte[] bParseData = new byte[bGetData.Length]; //전체데이터 복사본
                byte[] bLenth = new byte[2];                       //길이 재는 용도

                STEPMANAGER_VALUE.OOBServiceClear();               //데이터 초기화.

                Array.Copy(bGetData, 0, bParseData, 0, bParseData.Length);


                if (!bGetData.Length.Equals(284)) //연구소 요청에 의해 2R1Zf 에서 다시 3Q1Zf 로 다시 변경. 이랬다가 저랬다가. 어휴 (2018 . 08.09)
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
                            case (int)ServiceIndexA.szMEID:         STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexA.szICCID:        STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 30); iDx += 30; break;
                            case (int)ServiceIndexA.cRadioIF:       STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nActiveChannel: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nCurrentSID:    STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.iTxPwr:         STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.cCallState:     STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;

                            case (int)ServiceIndexA.szDialedDigits: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 33); iDx += 33; break;
                            case (int)ServiceIndexA.nPilotPN:       STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cServiceDomain: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cRSSI:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.nErrorRate:     STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cDTMFEvent:     STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nServiceOption: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cServiceStatus: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.szIMEI:         STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexA.cCSAttachState: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cPSAttachState: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nMCC:           STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nMNC:           STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nArfcn:         STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cBSIC:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cECIO:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.iCellID:        STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.nUARFCN:        STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.cAttachState:   STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nEARFCN:        STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nSNR:           STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.nRSRP:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.szBanner:       STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 50); iDx += 50; break;
                            case (int)ServiceIndexA.nTAC:           STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.iVocoder:       STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexA.iVocoderRate:   STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.iRSRP:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.cBand_1900:     STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.nLAC:           STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.szMDN:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexA.iServiceDomainPref: STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.iScanList:      STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.bIsVoLTE:       STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.szIncomingNumber: STEPMANAGER_VALUE.OOBServiceInfoA[i] = Encoding.UTF8.GetString(bParseData, iDx, 33); iDx += 33; break;
                            case (int)ServiceIndexA.cSINR:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexA.cRSRQ:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
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


        public int AnalyzePacket(byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString)
        {
            try
            {
                //0. VCP 응답패킷이 16바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 16) return (int)STATUS.RUNNING;

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
                        if (tmpBytes.Length > (j + 8 + 2))
                        {
                            int iHighLength = (int)tmpBytes[j + 4] * (int)0x100;
                            int iLowLength = (int)tmpBytes[j + 5];
                            
                            bool bCommandCheck = true;

                            //예외명령인지 한번 체크            
                            CheckExceptionCommands(strCommandName, ref strOrginSendData);

                            for (int i = 4; i < 9; i++)
                            {
                                if (tmpBytes[j + i + 2] != strOrginSendData[i]) //보낸명령어에 대한 응답데이터인지 확인1
                                {
                                    bCommandCheck = false;
                                    break;
                                }
                            }                                                      

                            if (bCommandCheck)
                            {
                                iSize = (int)tmpBytes[j + 5] + ((int)tmpBytes[j + 4] * (int)0x100) + 6;

                                if (tmpBytes.Length >= ((iLowLength + iHighLength) + j + 6))
                                {
                                    if (tmpBytes[j + iSize - 1] == DEFRES_ETX) //1. ETX 찾기        
                                    {
                                        iFindStx = j;
                                        bFind = true;

                                        byte[] tmpBuffer = new byte[iSize];
                                      
                                        Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                                        bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData);
                                        strLogString = BitConverter.ToString(tmpBuffer).Replace("-", " ");
                                        if (bChksum == (int)STATUS.OK)
                                        {
                                            return (int)STATUS.OK;
                                        }
                                    }

                                }
                                else
                                {
                                    return (int)STATUS.RUNNING;
                                }
                            }
              
                        }
                    }                           
                }

                switch (bChksum)
                {
                    case (int)STATUS.NG:    return (int)STATUS.NG;
                    case (int)STATUS.CHECK: return (int)STATUS.CHECK;
                    default: return (int)STATUS.RUNNING;
                }
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":TCP:" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg);
                tmpLogger.WriteCommLog("Exception: AnalyzePacket() TCP:", ex.Message, false);
                return (int)STATUS.RUNNING;
            }            
                
        }

        public int AnalyzeBluegoPacket(string tmpBytes, ref string rtnData, AnalyizePack anlPack)
        {
            try
            {
                //0. VCP 응답패킷이 5바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 30)
                {
                    return (int)STATUS.RUNNING;
                }

                //1. STX 찾기 
                if (anlPack.strAanlyizeString.Length < 1)
                {
                    //rtnFullData = "anlPack.strAanlyizeString.Length < 1";
                    rtnData = "PACK FILE CHECK";
                    return (int)STATUS.NG;
                }

                if (tmpBytes.Contains(anlPack.strAanlyizeString))
                {
                    int iStx = tmpBytes.IndexOf(anlPack.strAanlyizeString);
                    int iEtx = tmpBytes.IndexOf(")\n", iStx);

                    if(iStx > 0 && iEtx > iStx)
                    
                    {
                        //rtnFullData = tmpBytes.Substring(iStx, iEtx - iStx);
                        rtnData = tmpBytes.Substring(iStx + anlPack.strAanlyizeString.Length, iEtx - (iStx + anlPack.strAanlyizeString.Length));
                        rtnData = rtnData.Trim();

                        if (anlPack.strReplaceString.Length > 5) //제거할 캐릭터가 있으면 제거한다.
                        {
                            string strRemoveChar = anlPack.strReplaceString.Replace("REP=>", String.Empty);
                            rtnData = rtnData.Replace(strRemoveChar, String.Empty);
                        }
                        return (int)STATUS.OK;
                    }                   
                    
                }

                if (tmpBytes.Length > 2048)
                {
                    return (int)STATUS.NG;
                }

            }
            catch (Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: AnalyzeBluegoPacket() :", ex.Message, false);
                return (int)STATUS.RUNNING;
            } 

            return (int)STATUS.RUNNING;
        }

        public int AnalyzeWLCommandPacket(string tmpBytes, ref string rtnData, ref string rtnFullData, string strOrginSendData, string strCompareA, string strCompareB)
        {
            try
            {
                //0. VCP 응답패킷이 5바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 30) return (int)STATUS.RUNNING;

                //1. STX 찾기 [COMMON]|=> systemx

                string strOrginSendDatas = "(" + strOrginSendData + ")";

                if (tmpBytes.Contains("[COMMON]|=> systemx") && tmpBytes.Contains(strOrginSendDatas))
                {

                    if (strCompareA.Length > 5 && strCompareB.Length > 5)
                    {   
                        if (tmpBytes.Contains(strCompareA) && tmpBytes.Contains(strCompareB))
                        {
                            try
                            {
                                int iStx = tmpBytes.IndexOf((strCompareA));
                                int iEtx = tmpBytes.IndexOf((strCompareB));

                                rtnFullData = tmpBytes.Substring(iStx, iEtx - iStx);
                                rtnData = tmpBytes.Substring(iStx + strCompareA.Length, iEtx - (iStx + strCompareA.Length));
                                rtnData = rtnData.Trim();
                                return (int)STATUS.OK;
                            }
                            catch 
                            {
                                rtnFullData = tmpBytes;
                                rtnData = "Substring Error";
                                return (int)STATUS.OK;
                            }
                        }
                    }
                
                    rtnFullData = tmpBytes.Substring(tmpBytes.IndexOf(("[COMMON]|=> systemx"),  strOrginSendData.Length + 21));
                    rtnData = "OK";
                    return (int)STATUS.OK;
                }

                if (tmpBytes.Length > 2048)
                {
                    return (int)STATUS.NG;
                }
                
            }
            catch (Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: AnalyzeWLCommandPacket() :", ex.Message, false);
                return (int)STATUS.RUNNING;
            } 

            return (int)STATUS.RUNNING;
        }
                
        public byte[] ConvertTcpByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason)
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
                        int iEfileType = (int)EFILETYPE.CHECK;

                        switch (tmpString[i])
                        {
                            case "EFILE1": iEfileType = (int)EFILETYPE.CHECK;    break;
                            case "EFILE2": iEfileType = (int)EFILETYPE.TRANSFER; break;
                            case "EFILE3": iEfileType = (int)EFILETYPE.COMPLETE; break;
                        }

                        switch (tmpString[i])
                        {   
                            case "EFILE1":  //EFILE1,2,3 은 인증서업로드용이다.(For TCP)
                            case "EFILE2": 
                            case "EFILE3": 
                                            bOk = true;
                                            bParam = MakeEfileStruct(iEfileType, strParam, ref bOk, ref strReason);

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
                                                tmpList.Add("FF");
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

                            case "ALDL_HEXA":   //ALDL포멧으로 만들되 파라미터는 HEX형태로 만든다.
                            case "ALDL_ASCII":  //ALDL포멧으로 만들되 파라미터는 아스키로 만든다.
                            case "ALDL_HEXB":  //ALDL포멧으로 만들되 파라미터는 HEX형태이며 특정 바이트의 특정 비트만 ON 할경우.
                            case "ALDL_HEXC":  //ALDL포멧으로 만들되 파라미터는 HEX형태이며 특정 바이트의 특정 비트만 ON 할경우.
                                            bOk = true;                                            
                                            strValue = String.Empty;
                                            bParam = new byte[1];

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA":  brtnOk = true; break;
                                                case "ALDL_ASCII": brtnOk = true; break;
                                                case "ALDL_HEXB":  brtnOk = MakeToggleAldl(true, strParam, ref strValue, ref strReason); break;
                                                case "ALDL_HEXC":  brtnOk = MakeToggleAldl(false, strParam, ref strValue, ref strReason); break;
                                                default:           brtnOk = false; break;
                                            }
                                            
                                            if (!brtnOk)
                                            {
                                                tmpList.Add("FF");                                                                                                                                             
                                                return rtnNull;
                                            }

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA":  bParam = MakeTCP_ALDLType(strParam, ref bOk, ref strReason, false); break;
                                                case "ALDL_ASCII": bParam = MakeTCP_ALDLType(strParam, ref bOk, ref strReason, true); break;
                                                case "ALDL_HEXB":  bParam = MakeTCP_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                case "ALDL_HEXC":  bParam = MakeTCP_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                default:           bOk = false; break;
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
                                                tmpList.Add("FF");
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
                        catch { }

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
                    DKCHK.Tcp_chksum(tmpArray, ref bHighByte, ref bLowByte, true);
                    tmpList.RemoveAt(i);
                    tmpList.Insert(i, bLowByte.ToString("x2"));   //BIG ENDIAN 
                    tmpList.Insert(i, bHighByte.ToString("x2"));                           

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

        private void CheckExceptionCommands(string strCommandName, ref byte[] strOrginSendData)
        {
            //보낸명령어랑 응답 명령이 달라야하는 경우를 위해...
            byte[] bCommand = new byte[5];
            switch (strCommandName)
            {
                case "GET_SERVICE_INFORMATION":        
                    /* 장민호 책임 다시 변경요청. 메일 2018.08.09 참조    
                    bCommand[0] = 0x32; bCommand[1] = 0x52; bCommand[2] = 0x31; bCommand[3] = 0x5A; bCommand[4] = 0x66;
                    Array.Copy(bCommand, 0, strOrginSendData, 4, bCommand.Length);
                    break;
                    */
                default:
                    break;
            }

        }
    }

}
