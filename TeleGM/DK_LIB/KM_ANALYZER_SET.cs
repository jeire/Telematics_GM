using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{
    class KM_ANALYZER_SET
    {

        //GEN12
        private const byte DEFRES_STX1 = 0x4B;
        private const byte DEFRES_STX2 = 0x55;
        private const byte DEFRES_ETX = 0x7E;

        private const byte DEFRESPONSE = 0x02;
        private const byte DEFREPORT = 0x03;

        private const byte RES_SUCCESS = 0x01;
        private const byte RES_FAILURE = 0x00;
        private const byte RES_UNDEFINED = 0x02;

        private static List<TBLDATA0> LstTBLtm = new List<TBLDATA0>();       //TM 명령 테이블 리스트
        DK_LOGGER tmpLogger = new DK_LOGGER("PC", false);

        public KM_ANALYZER_SET()
        {
            tmpLogger.LoadTBL0("GEN12.TBL", ref LstTBLtm);
        }

        public SETPACK GetWaveResFormat(List<TBLDATA0> lstTBLDB, string strCommandName, ref bool bFind)
        {
            SETPACK iResult = new SETPACK();
            iResult.iType = (int)GEN12RESTYPE.NONE;
            iResult.iCount = -1;
            iResult.bSplit = false;
            bFind = false;
            try
            {
                for (int i = 0; i < lstTBLDB.Count; i++)
                {
                    if (lstTBLDB[i].CMDNAME.Equals(strCommandName))
                    {
                        switch (lstTBLDB[i].PARPAC2.Trim())
                        {
                            case "HEAD": iResult.bSplit = true; break;
                            case "BODY": iResult.bSplit = false; break;
                            default: iResult.bSplit = true; break;
                        }
                        switch (lstTBLDB[i].RECVPAC.Trim())
                        {   //NONE, BYTE, CHAR, INT, DTC, DOUBLE, SINGLE, INT16, INT32, END
                            //바이트문자열 2자리씩 고정하여 표시
                            case "BYTE": iResult.iType = (int)GEN12RESTYPE.BYTE; break;
                            //아스키로 문자열로 표시
                            case "CHAR": iResult.iType = (int)GEN12RESTYPE.CHAR; break;
                            //아스키로 문자열로 표시하나 트림하지 않는다.
                            case "TEXT": iResult.iType = (int)GEN12RESTYPE.TEXT; break;
                            //숫자 전체 합산
                            case "INT": iResult.iType = (int)GEN12RESTYPE.INT; break;
                            //DTC 값에 의한 별도 표시
                            case "DTC": iResult.iType = (int)GEN12RESTYPE.DTC; break;
                            //8바이트 표시
                            case "DOUBLE": iResult.iType = (int)GEN12RESTYPE.DOUBLE; break; //8byte
                            //4바이트 표시
                            case "SINGLE": iResult.iType = (int)GEN12RESTYPE.SINGLE; break; //4byte

                            case "INT16": iResult.iType = (int)GEN12RESTYPE.INT16; break;  //2byte

                            case "UINT16": iResult.iType = (int)GEN12RESTYPE.UINT16; break; //2byte

                            case "INT32": iResult.iType = (int)GEN12RESTYPE.INT32; break;  //4byte

                            case "BINT16": iResult.iType = (int)GEN12RESTYPE.BINT16; break; //2byte Big Endian

                            case "ADC": iResult.iType = (int)GEN12RESTYPE.ADC; break;    //

                            case "TTFF": iResult.iType = (int)GEN12RESTYPE.TTFF; break;  //UINT16 * 0.001

                            case "SELFTEST": iResult.iType = (int)GEN12RESTYPE.SELFTEST; break;  //uint32, uint8

                            //gen11 analyzer에서 가져옴
                            case "BYTE1": iResult.iType         = (int)GEN12RESTYPE.BYTE1; break;
                            case "UINT32A": iResult.iType       = (int)GEN12RESTYPE.UINT32A; break;
                            case "UINT32B": iResult.iType       = (int)GEN12RESTYPE.UINT32B; break;
                            case "DTC_TABLE": iResult.iType     = (int)GEN12RESTYPE.DTC; break;
                            case "DTC_TABLE_GB": iResult.iType  = (int)GEN12RESTYPE.DTCGB; break; //GEN11 GB모델 DTC
                            case "DTC_TABLE_GEM": iResult.iType = (int)GEN12RESTYPE.DTCGEM; break; //GEN11 GEM모델 DTC
                            case "DTC_MANUAL": iResult.iType    = (int)GEN12RESTYPE.DTCMANUAL; break; //DTC 코드 비교안하는 임시 커멘드, 이동성선임 요청 // 2017.06.12                                
                            case "DTC_BITS": iResult.iType      = (int)GEN12RESTYPE.DTCBITS; break;
                            case "DTC_BITS_GB": iResult.iType   = (int)GEN12RESTYPE.DTCGBBITS; break;
                            case "DTC_BITS_GEM": iResult.iType  = (int)GEN12RESTYPE.DTCGEMBITS; break;
                            case "DTC_ALL": iResult.iType       = (int)GEN12RESTYPE.DTCALL; break;
                            case "ALDL_ASCII":
                                iResult.iType = (int)GEN12RESTYPE.ALDL_ASCII;
                                STEPMANAGER_VALUE.bOldBinaryALDL = new byte[1]; //초기화해줌.
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2]; //초기화해줌.
                                break;
                            case "ALDL_HEXA":
                                iResult.iType = (int)GEN12RESTYPE.ALDL_HEXA;
                                STEPMANAGER_VALUE.bOldBinaryALDL = new byte[1]; //초기화해줌.
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2]; //초기화해줌.
                                break;
                            case "ALDL_BITS": iResult.iType = (int)GEN12RESTYPE.ALDL_BITS; break;
                            case "ALDL12_ASCII":
                                iResult.iType = (int)GEN12RESTYPE.ALDL12_ASCII;
                                STEPMANAGER_VALUE.bOldBinaryALDL = new byte[1]; //초기화해줌.
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2]; //초기화해줌.
                                break;
                            case "ALDL12_HEXA":
                                iResult.iType = (int)GEN12RESTYPE.ALDL12_HEXA;
                                STEPMANAGER_VALUE.bOldBinaryALDL = new byte[1]; //초기화해줌.
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2]; //초기화해줌.
                                break;
                            case "IMEICHKSUM": iResult.iType = (int)GEN12RESTYPE.IMEICHKSUM; break;
                            case "BUBCAL": iResult.iType = (int)GEN12RESTYPE.BUBCAL; break;

                            case "WIFIRX1": iResult.iType = (int)GEN12RESTYPE.WIFIRX1; break;
                            case "WIFIRX2": iResult.iType = (int)GEN12RESTYPE.WIFIRX2; break;
                            case "WIFIRX3": iResult.iType = (int)GEN12RESTYPE.WIFIRX3; break;
                            case "WIFIRX4": iResult.iType = (int)GEN12RESTYPE.WIFIRX4; break;
                            case "WIFIRX5": iResult.iType = (int)GEN12RESTYPE.WIFIRX5; break;

                            case "FEATUREID": iResult.iType = (int)GEN12RESTYPE.FEATUREID; break;


                            case "NVITEM": iResult.iType = (int)GEN12RESTYPE.NVITEM; break;

                            case "SERVICE": iResult.iType = (int)GEN12RESTYPE.SERVICE; break;

                            default: iResult.iType = (int)GEN12RESTYPE.NONE; break;
                        }

                        if (lstTBLDB[i].RECVPAC.Length > 1 && lstTBLDB[i].RECVPAC.Contains("INDEX:") && lstTBLDB[i].RECVPAC.Contains(","))
                        {
                            try
                            {
                                string tmpString = lstTBLDB[i].RECVPAC.Replace("INDEX:", String.Empty);

                                string[] tmpPACTYPE = tmpString.Split(',');

                                if (tmpPACTYPE.Length.Equals(2))
                                {
                                    iResult.iCount = int.Parse(tmpPACTYPE[0]);

                                    switch (tmpPACTYPE[1])
                                    {
                                        case "BYTE1": iResult.iType = (int)GEN12RESTYPE.BYTE1; break;
                                        case "BYTE": iResult.iType = (int)GEN12RESTYPE.BYTE; break;
                                        case "CHAR": iResult.iType = (int)GEN12RESTYPE.CHAR; break;
                                        case "TEXT": iResult.iType = (int)GEN12RESTYPE.TEXT; break;
                                        case "INT": iResult.iType = (int)GEN12RESTYPE.INT; break;
                                        case "DTC": iResult.iType = (int)GEN12RESTYPE.DTC; break;
                                        case "DOUBLE": iResult.iType = (int)GEN12RESTYPE.DOUBLE; break;
                                        case "SINGLE": iResult.iType = (int)GEN12RESTYPE.SINGLE; break;
                                        case "INT16": iResult.iType = (int)GEN12RESTYPE.INT16; break;
                                        case "UINT16": iResult.iType = (int)GEN12RESTYPE.UINT16; break;
                                        case "INT32": iResult.iType = (int)GEN12RESTYPE.INT32; break;
                                        case "BINT16": iResult.iType = (int)GEN12RESTYPE.BINT16; break;
                                        case "ADC": iResult.iType = (int)GEN12RESTYPE.ADC; break;    //
                                        case "TTFF": iResult.iType = (int)GEN12RESTYPE.TTFF; break;
                                        case "SELFTEST": iResult.iType = (int)GEN12RESTYPE.SELFTEST; break;

                                        default: iResult.iType = (int)GEN12RESTYPE.NONE; break;
                                    }
                                }
                            }
                            catch
                            {
                                iResult.iCount = -1;
                                iResult.iType = (int)GEN12RESTYPE.NONE; break;
                            }
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

        private bool CheckWave_DtcIndex(byte bSendParam, byte[] bGetData, ref string strRetunData)
        {
            try
            {
                //미확정                
                //8비트로 나타내는 방법 (DQA 담당자 요청방식)
                strRetunData = Convert.ToString(bGetData[3], 2).PadLeft(8, '0');

                //2번째 비트만 나타내는 방법 (현우씨 요청방식)
                //strRetunData = strRetunData.Substring(6, 1);

                //바이트만 그대로 나타내는 방법 (GEN10과 동일방식 - 20160229)
                //strRetunData = bGetData[3].ToString("X2");

                return true;
            }
            catch
            {
                strRetunData = "CheckWave_DtcIndex:Exception";
                return false;
            }
        }

        private bool CheckGen11_ALDLType(byte[] bSendPack, byte[] bGetData, ref string strRetunData, ref byte[] bReadBinary, int iResType)
        {
            try
            {
                int ALDLMAX = STEPMANAGER_VALUE.GetGen11AldlBlockSize();
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[200]; 150byte // 200으로 증가 // 321로 증가
                // 4. unsigned char  lflag[200];150byte // 200으로 증가 // 321로 증가
                if (bGetData.Length != ALDLMAX + ALDLMAX + 3) return false;

                string strData1 = String.Empty;
                string strData2 = String.Empty;
                string strData3 = String.Empty;
                string strData4 = String.Empty;

                byte[] bData1 = new byte[2];
                byte[] bData2 = new byte[1];
                byte[] bData3 = new byte[ALDLMAX]; //GEN10 TCP 는 150 , GEN11 은 200 이라고 함.,(강종훈책임 20170607 메일참고).  GEN11 MY23부터 321 로 또 증가 (2020.03.20. 이진성책임 메일참고)
                byte[] bData4 = new byte[ALDLMAX];

                Array.Copy(bGetData, 0, bData1, 0, bData1.Length);
                Array.Copy(bGetData, 2, bData2, 0, bData2.Length);
                Array.Copy(bGetData, 3, bData3, 0, bData3.Length);
                Array.Copy(bGetData, ALDLMAX + 3, bData4, 0, bData4.Length);

                //1. ALDL 블럭체크
                if (bSendPack[12] != bData1[0] || bSendPack[13] != bData1[1])
                {
                    return false;
                }

                int iDataLen = 0;

                try
                {
                    iDataLen = BitConverter.ToInt16(bData1, 0); //(int)bData1[2];
                }
                catch
                {
                    strRetunData = "CheckGen11_ALDLType:Exception - DATA TYPE ERROR";
                    return false;
                }

                bReadBinary = new byte[ALDLMAX];  //ALDL WRITE를 사용할때 위해서.
                Array.Copy(bData3, 0, bReadBinary, 0, bReadBinary.Length);
                Array.Copy(bData1, 0, STEPMANAGER_VALUE.bOldBinaryBLCK, 0, 2);

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

                switch (iResType)
                {
                    case (int)GEN11RESTYPE.ALDL_HEXA:

                        strData3 = BitConverter.ToString(bData3, 0, (int)bData2[0]).Replace("-", "");
                        break;

                    case (int)GEN11RESTYPE.ALDL_ASCII:
                    default:

                        if (bIsAscii)
                        {
                            strData3 = Encoding.UTF8.GetString(bData3);
                        }
                        else
                        {
                            strData3 = BitConverter.ToString(bData3, 0, (int)bData2[0]).Replace("-", "");
                        }
                        break;
                }

                strData4 = BitConverter.ToString(bData4).Replace("-", "");
                strData4 = strData4.Replace("00", "");

                strRetunData = strData3;
                strRetunData = strRetunData.Replace("\0", String.Empty);
                return true;
            }
            catch
            {
                strRetunData = "CheckGen12_ALDLType:Exception";
                return false;
            }

        }

        private bool CheckGen12_ALDLType(byte[] bSendPack, byte[] bGetData, ref string strRetunData, ref byte[] bReadBinary, int iResType)
        {
            try
            {
                //int ALDLMAX = STEPMANAGER_VALUE.GetGen11AldlBlockSize();
                //int ALDLMAX = 9;//STEPMANAGER_VALUE.GetGen11AldlBlockSize();bGetData
                
                //20220623
                int ALDLMAX = bGetData.Length;//STEPMANAGER_VALUE.GetGen11AldlBlockSize();bGetData
                
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[200]; 150byte // 200으로 증가 // 321로 증가
                // 4. unsigned char  lflag[200];150byte // 200으로 증가 // 321로 증가
                //if (bGetData.Length != ALDLMAX + ALDLMAX + 3) return false;

                string strData1 = String.Empty;
                string strData2 = String.Empty;
                string strData3 = String.Empty;
                string strData4 = String.Empty;

                byte[] bData1 = new byte[2];
                byte[] bData2 = new byte[1];
                byte[] bData3 = new byte[ALDLMAX]; //GEN10 TCP 는 150 , GEN11 은 200 이라고 함.,(강종훈책임 20170607 메일참고).  GEN11 MY23부터 321 로 또 증가 (2020.03.20. 이진성책임 메일참고)
                byte[] bData4 = new byte[ALDLMAX];

                //Array.Copy(bGetData, 0, bData1, 0, bData1.Length);
                //Array.Copy(bGetData, 2, bData2, 0, bData2.Length);
                Array.Copy(bGetData, 0, bData3, 0, bData3.Length);
                //Array.Copy(bGetData, ALDLMAX + 3, bData4, 0, bData4.Length);

                ////1. ALDL 블럭체크
                //if (bSendPack[12] != bData1[0] || bSendPack[13] != bData1[1])
                //{
                //    return false;
                //}

                int iDataLen = 0;

                try
                {
                    iDataLen = BitConverter.ToInt16(bData1, 0); //(int)bData1[2];
                }
                catch
                {
                    strRetunData = "CheckGen11_ALDLType:Exception - DATA TYPE ERROR";
                    return false;
                }

                bReadBinary = new byte[bGetData.Length];  //ALDL WRITE를 사용할때 위해서.
                Array.Copy(bData3, 0, bReadBinary, 0, bReadBinary.Length);
                //Array.Copy(bData1, 0, STEPMANAGER_VALUE.bOldBinaryBLCK, 0, 2);

                strData1 = BitConverter.ToString(bData1).Replace("-", "");
                strData2 = ((int)bData2[0]).ToString();

                //여기서 아스키로 변환해야하는지 한바이트만 체크하자.
                bool bIsAscii = false;

                //for (int i = 0; i < (int)bData2[0]; i++)
                for (int i = 0; i < bGetData.Length; i++)
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

                switch (iResType)
                {
                    case (int)GEN12RESTYPE.ALDL12_HEXA:

                        //strData3 = BitConverter.ToString(bData3, 0, (int)bData2[0]).Replace("-", "");
                        strData3 = BitConverter.ToString(bData3).Replace("-", "");
                        break;

                    case (int)GEN12RESTYPE.ALDL12_ASCII:
                    default:

                        if (bIsAscii)
                        {
                            strData3 = Encoding.UTF8.GetString(bData3);
                        }
                        else
                        {
                            //strData3 = BitConverter.ToString(bData3, 0, (int)bData2[0]).Replace("-", "");
                            strData3 = BitConverter.ToString(bData3).Replace("-", "");
                        }
                        break;
                }

                //strData4 = BitConverter.ToString(bData4).Replace("-", "");
                //strData4 = strData4.Replace("00", "");

                strRetunData = strData3;
                strRetunData = strRetunData.Replace("\0", String.Empty);
                return true;
            }
            catch
            {
                strRetunData = "CheckGen12_ALDLType:Exception";
                return false;
            }

        }

        public byte[] MakeEfileStruct(int iEfileIdx, string strParam, ref bool bRes, ref string strReason, bool bCallClass)
        {
            byte[] bReturnBytes;
            byte[] bState = new byte[4];      //eTRANSFERFILE ( check, transfer, complete )
            byte[] bFileName = new byte[64];
            byte[] bFilePath = new byte[256];
            byte[] bFileSize = new byte[4];      //uint C# (unsigned long C++)
            byte[] bDataSize = new byte[2];
            byte[] bRealData = new byte[1024];

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
                case "KIS_error_code": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode]; break;
                case "KIS_error_message": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg]; break;
                case "KIS_stid": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID]; break;
                case "KIS_rCert": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT]; break;
                case "KIS_ccCert": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert]; break;
                case "KIS_vCert": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert]; break;
                case "KIS_vPri": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri]; break;
                case "KIS_vPre": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre]; break;
                case "KIS_vAuth": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth]; break;
                case "KIS_vHash": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash]; break;
                default:
                    //local 경로에서 확인.
                    strRealData = "LOCAL";
                    break;
            }


             //TEST DATA
            switch (strTempSpl[2])
            {
                case "KIS_error_code":    strRealData = "0"; break;
                case "KIS_error_message": strRealData = ""; break;
                case "KIS_stid":   strRealData  = "117605004"; break;
                case "KIS_rCert":  strRealData  = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2 + BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q / bq + hSOtIYc26NFMEMwDgYDVR0PAQH / BAQDAgEGMBIGA1UdEwEB / wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx / CGT0Vg8XBLeR2 + QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9 + HQdZSk5i3G3r8eB / jOp0rNLH---- - END CERTIFICATE---- - "; break;
                case "KIS_ccCert": strRealData  = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; break;
                case "KIS_vCert":  strRealData  = "-----BEGIN CERTIFICATE-----MIIBtzCCAV6gAwIBAgIIBgYdF4XYYAEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTIxMDMyOTEzMDkxMloYDzIxMjEwMzMwMjM1OTU5WjAqMQ8wDQYDVQQKDAZPblN0YXIxFzAVBgNVBAMMDlNUSUQgMjA1MjY1MDA1MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGH5o3cVhu9VVl7fm36FoArLpeEktBi+QcPnNIMtfbpSC5OPNn3ZJO2Wlaf7tZiUx4XVthZhsMN3S5VvxApgBeaNkMGIwDgYDVR0PAQH/BAQDAgOIMCAGA1UdJQEB/wQWMBQGCCsGAQUFBwMCBggrBgEFBQcDCDAPBgNVHRMBAf8EBTADAQEAMB0GA1UdDgQWBBTtXfDjejvhQtb0HauvT8Y9+vY6nzAJBgcqhkjOPQQBA0gAMEUCIQD6TlDiWr6CYM7VKZn2pL6aVsh9pN3i7sB2d+o0Q60i4QIgCX3WwHKn5KT1yWqUs2J1rr9vNliy4zTfYRTh3VDYl7U=-----END CERTIFICATE-----"; 
                                   break;
                case "KIS_vPri": strRealData = "-----BEGIN PRIVATE KEY-----MEECAQAwEwYHKoZIzj0CAQYIKoZIzj0DAQcEJzAlAgEBBCAlLHmi9vLc6r8ICu9eFVuiUsH7NNK20PioqhZyIyeLIw==-----END PRIVATE KEY-----"; 
                                  break;
                case "KIS_vPre": strRealData = ""; break;
                case "KIS_vAuth": strRealData = "765302B6BCC137DF"; break;
                case "KIS_vHash": strRealData = ""; break;
                default:
                    //local 경로에서 확인.
                    strRealData = "LOCAL";                    
                    break;
            }
            

            if (String.IsNullOrEmpty(strRealData))
            {
                strReason = "KIS Data Empty : " + strRealData;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            strFilePath = strTempSpl[0] + "/" + strTempSpl[1];
            strFileName = strTempSpl[1];


            //Step1. FileName 만들기           
            byte[] bTempFileName = Encoding.UTF8.GetBytes(strFileName);
            if (bTempFileName.Length >= bFileName.Length)
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
                    STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountLength = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize = 0;
                    break;

                case (int)EFILETYPE.TRANSFER:

                    bState[0] = (byte)EFILETYPE.TRANSFER;
                    if (strRealData.Equals("LOCAL"))
                    {
                        string strProgramPath = AppDomain.CurrentDomain.BaseDirectory + "GEN12_CERT\\" + strTempSpl[2];
                        if (!CertiFileExist(strProgramPath, ref bRealData, ref strReason, bCallClass)) //로컬파일 데이터를 끌어와 바이너리로 변경한다. 
                        {
                            bReturnBytes = new byte[iFuncBytes];
                            bRes = false;
                            return bReturnBytes;
                        }
                    }
                    else
                    {
                        if (!CertiMemoryExist(strRealData, ref bRealData, ref strReason, bCallClass)) //KIS 데이터를 끌어와 바이너리로 변경한다. 
                        {
                            bReturnBytes = new byte[iFuncBytes];
                            bRes = false;
                            return bReturnBytes;
                        }
                    }
                    bFileSize = BitConverter.GetBytes(STEPMANAGER_VALUE.iUploadBytesCountTotalSize);
                    bDataSize = BitConverter.GetBytes((ushort)bRealData.Length);
                    iFuncBytes += bRealData.Length;
                    break;

                case (int)EFILETYPE.COMPLETE:
                    bState[0] = (byte)EFILETYPE.COMPLETE;

                    bFileSize = BitConverter.GetBytes(STEPMANAGER_VALUE.iUploadBytesCountTotalSize);

                    STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountLength = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize = 0;
                    break;

                default:
                    strReason = "EFILETYPE Error";
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
            }

            bReturnBytes = new byte[iFuncBytes];

            int iPos = 0;

            try
            {
                //## 구조체 채워넣기 
                Array.Copy(bState, 0, bReturnBytes, iPos, bState.Length);
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

        public byte[] MakeEfileStruct_Trans(int iEfileIdx, string strParam, ref bool bRes, ref string strReason, bool bCallClass)
        {
            byte[] bReturnBytes;
            byte[] bState = new byte[4];      //eTRANSFERFILE ( check, transfer, complete )

            byte[] bRealData = new byte[1024];

            //int iFuncBytes = bState.Length + bFileName.Length + bFilePath.Length + bFileSize.Length + bDataSize.Length;

            int iFuncBytes = 1;// bState.Length + bFileName.Length + bFilePath.Length + bFileSize.Length + bDataSize.Length;

            //string strFileName;
            //string strFilePath;
            string strRealData;

            //Step0. 파라미터 체크
            if (string.IsNullOrEmpty(strParam))
            {
                strReason = "Par1 Error : " + strParam;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            strParam = strParam.Trim();

            
            //KIS 데이터를 끌고오기위해 여기서 할것이아니라 현재 함수를 호출하는 부분에서 KIS데이터를 파라미터로 전달처리해야한다.
            switch (strParam)
            {
                case "KIS_error_code": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode]; break;
                case "KIS_error_message": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg]; break;
                case "KIS_stid": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID]; break;
                case "KIS_rCert": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT]; break;
                case "KIS_ccCert": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert]; break;
                case "KIS_vCert": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert]; break;
                case "KIS_vPri": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri]; break;
                case "KIS_vPre": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre]; break;
                case "KIS_vAuth": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth]; break;
                case "KIS_vHash": strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash]; break;
                default:
                    //local 경로에서 확인.
                    strRealData = "LOCAL";
                    break;
            }
            
            //sscho TEST DATA
            /*
            switch (strParam)
            {
                case "KIS_error_code": strRealData = "0"; break;
                case "KIS_error_message": strRealData = ""; break;
                case "KIS_stid": strRealData = "117605004"; break;
                case "KIS_rCert": strRealData = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2 + BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q / bq + hSOtIYc26NFMEMwDgYDVR0PAQH / BAQDAgEGMBIGA1UdEwEB / wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx / CGT0Vg8XBLeR2 + QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9 + HQdZSk5i3G3r8eB / jOp0rNLH---- - END CERTIFICATE---- - "; break;
                case "KIS_ccCert": strRealData = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; break;
                case "KIS_vCert":
                    strRealData = "-----BEGIN CERTIFICATE-----MIIBtzCCAV6gAwIBAgIIBgYdF4XYYAEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTIxMDMyOTEzMDkxMloYDzIxMjEwMzMwMjM1OTU5WjAqMQ8wDQYDVQQKDAZPblN0YXIxFzAVBgNVBAMMDlNUSUQgMjA1MjY1MDA1MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGH5o3cVhu9VVl7fm36FoArLpeEktBi+QcPnNIMtfbpSC5OPNn3ZJO2Wlaf7tZiUx4XVthZhsMN3S5VvxApgBeaNkMGIwDgYDVR0PAQH/BAQDAgOIMCAGA1UdJQEB/wQWMBQGCCsGAQUFBwMCBggrBgEFBQcDCDAPBgNVHRMBAf8EBTADAQEAMB0GA1UdDgQWBBTtXfDjejvhQtb0HauvT8Y9+vY6nzAJBgcqhkjOPQQBA0gAMEUCIQD6TlDiWr6CYM7VKZn2pL6aVsh9pN3i7sB2d+o0Q60i4QIgCX3WwHKn5KT1yWqUs2J1rr9vNliy4zTfYRTh3VDYl7U=-----END CERTIFICATE-----";
                    break;
                case "KIS_vPri":
                    strRealData = "-----BEGIN PRIVATE KEY-----MEECAQAwEwYHKoZIzj0CAQYIKoZIzj0DAQcEJzAlAgEBBCAlLHmi9vLc6r8ICu9eFVuiUsH7NNK20PioqhZyIyeLIw==-----END PRIVATE KEY-----";
                    break;
                case "KIS_vPre": strRealData = ""; break;
                case "KIS_vAuth": strRealData = "765302B6BCC137DF"; break;
                case "KIS_vHash": strRealData = ""; break;
                default:
                    //local 경로에서 확인.
                    strRealData = "LOCAL";
                    break;
            }
            //
            */

            if (String.IsNullOrEmpty(strRealData))
            {
                strReason = "KIS Data Empty : " + strRealData;
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
            STEPMANAGER_VALUE.iUploadBytesCountLength = 0;
            STEPMANAGER_VALUE.iUploadBytesCountTotalSize = 0;
            bState[0] = (byte)EFILETYPE.TRANSFER;
            if (strRealData.Equals("LOCAL"))
            {
                string strProgramPath = AppDomain.CurrentDomain.BaseDirectory + "GEN12_CERT\\" + strParam;
                if (!CertiFileExist(strProgramPath, ref bRealData, ref strReason, bCallClass)) //로컬파일 데이터를 끌어와 바이너리로 변경한다. 
                {
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
                }
            }
            else
            {
                if (!CertiMemoryExist(strRealData, ref bRealData, ref strReason, bCallClass)) //KIS 데이터를 끌어와 바이너리로 변경한다. 
                {
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
                }
            }
        
            //sscho
            iFuncBytes = 0;
            iFuncBytes += bRealData.Length;
            
            bReturnBytes = new byte[iFuncBytes];

            int iPos = 0;

            try
            {
                //## 구조체 채워넣기             
                //if (bState[0] == (byte)EFILETYPE.TRANSFER)
                //{
                    Array.Copy(bRealData, 0, bReturnBytes, iPos, bRealData.Length);
                    iPos += bRealData.Length;
                //}
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

        private bool CertiFileExist(string strFilePath, ref byte[] bBinaryFile, ref string strReason, bool bCallClass)
        {
            bool bSuccess = false;

            if (!System.IO.File.Exists(strFilePath))
            {   //파일이 없으면 
                strReason = "NOT FOUND FILE";
                return bSuccess;
            }
            else
            {
                //파일이 있으면
                System.IO.FileStream FS = null;
                System.IO.BinaryReader BR = null;

                try
                {
                    FS = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    BR = new System.IO.BinaryReader(FS);

                    //STEPMANAGER_VALUE.iUploadBytesCountTotalSize = (int)BR.BaseStream.Length; //총사이즈 저장.

                    int iDataLen = (int)BR.BaseStream.Length;

                    bBinaryFile = new byte[iDataLen];
                    BR.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    int i = BR.Read(bBinaryFile, 0, iDataLen);

                    bSuccess = true;
                    strReason = "SUCCESS";

                }
                catch (Exception e)
                {
                    strReason = "MAKE FILE EXCEPTION.";
                    string messs = String.Empty;
                    messs = e.Message;
                    bSuccess = false;

                }
                finally
                {
                    if (BR != null) BR.Close();
                    if (FS != null) FS.Close();
                }

                return bSuccess;
            }
        }

        private bool CertiMemoryExist(string strRealData, ref byte[] bBinaryFile, ref string strReason, bool bCallClass)
        {
            bool bSuccess = false;

            int iDataLen = (int)strRealData.Length; //총사이즈 저장.

            try
            {      
                byte[] bTempBinary = Encoding.UTF8.GetBytes(strRealData);

                bBinaryFile = new byte[bTempBinary.Length];

                Array.Copy(bTempBinary, 0, bBinaryFile, 0, bTempBinary.Length);

                //if (bCallClass) //TX로 보낼때만 증가시켜야한다... RX때도 함수를 호출하기 때문에 이놈이 증가되버린다....
                //    STEPMANAGER_VALUE.iUploadBytesCountStartIndex += STEPMANAGER_VALUE.iUploadBytesCountLength;

                bSuccess = true;
                strReason = "SUCCESS";
            }
            catch
            {
                strReason = "CertiMemoryExist Error";
                return false;
            }


            /*
            STEPMANAGER_VALUE.iUploadBytesCountTotalSize = (int)strRealData.Length; //총사이즈 저장.

            if (STEPMANAGER_VALUE.iUploadBytesCountStartIndex >= STEPMANAGER_VALUE.iUploadBytesCountTotalSize)
            {
                strReason = "Already Transfer Complete";
                string messs = String.Empty;
                bSuccess = true;
            }
            else
            {
                try
                {
                    if (STEPMANAGER_VALUE.iUploadBytesCountTotalSize >= 1024 + STEPMANAGER_VALUE.iUploadBytesCountStartIndex)
                    {
                        STEPMANAGER_VALUE.iUploadBytesCountLength = 1024;
                    }
                    else
                    {
                        STEPMANAGER_VALUE.iUploadBytesCountLength =
                            STEPMANAGER_VALUE.iUploadBytesCountTotalSize - STEPMANAGER_VALUE.iUploadBytesCountStartIndex;
                    }


                    byte[] bTempBinary = Encoding.UTF8.GetBytes(strRealData);

                    bBinaryFile = new byte[STEPMANAGER_VALUE.iUploadBytesCountLength];

                    Array.Copy(bTempBinary, STEPMANAGER_VALUE.iUploadBytesCountStartIndex, bBinaryFile, 0, STEPMANAGER_VALUE.iUploadBytesCountLength);

                    if (bCallClass) //TX로 보낼때만 증가시켜야한다... RX때도 함수를 호출하기 때문에 이놈이 증가되버린다....
                        STEPMANAGER_VALUE.iUploadBytesCountStartIndex += STEPMANAGER_VALUE.iUploadBytesCountLength;

                    bSuccess = true;
                    strReason = "SUCCESS";
                }
                catch
                {
                    strReason = "CertiMemoryExist Error";
                    return false;
                }
            }
            */

            return bSuccess;
        }

        /*
        private byte[] MakeGen11_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {
            byte[] bReturnBytes = new byte[403]; // GEN10-TCP 303, GEN11-TCP 403  - 100증가
            try
            {
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[150]; 150byte
                // 4. unsigned char  lflag[150];150byte

                byte[] bDataBlock = new byte[2];
                byte[] bDataLen   = new byte[1];
                byte[] bDataData  = new byte[200]; // GEN10-TCP 150, GEN11-TCP 200 - 50증가
                byte[] bDataFlag  = new byte[200]; // GEN10-TCP 150, GEN11-TCP 200 - 50증가

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
                Array.Copy(bDataFlag,   0, bReturnBytes, 203, bDataFlag.Length);

                bRes = true;
                return bReturnBytes;
            }
            catch
            {
                strReason = "MakeGen11_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
            }

        }
        */
        //마스킹 기법을 사용하기 위해 변경
        private byte[] MakeGen11_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {
            int ALDLMAX = STEPMANAGER_VALUE.GetGen11AldlBlockSize();

            byte[] bReturnBytes = new byte[ALDLMAX + ALDLMAX + 3];
            try
            {
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[150]; 150byte
                // 4. unsigned char  lflag[150];150byte

                byte[] bDataBlock = new byte[2];
                byte[] bDataLen = new byte[1];
                byte[] bDataData = new byte[ALDLMAX]; // GEN10-TCP 150, GEN11-TCP 200 - 50증가, GEN11 MY23 321증가
                byte[] bDataFlag = new byte[ALDLMAX]; // GEN10-TCP 150, GEN11-TCP 200 - 50증가, GEN11 MY23 321증가

                byte[] bTempBlock;
                byte[] bTempData;

                bool bConvOK1 = false;
                bool bConvOK2 = false;

                int iPoint = 0;
                bool bEndPoint = false; //특정구간만 WRITE 할 경우.

                string[] strParseData = strParam.Split(',');

                switch (strParseData.Length)
                {
                    case 2:
                        iPoint = 0;
                        break;
                    case 3:
                        try
                        {
                            iPoint = int.Parse(strParseData[2]);
                            bEndPoint = true;
                        }
                        catch
                        {
                            strReason = "PAR1 ERROR : " + strParseData[2];
                            bRes = false;
                            return bReturnBytes;
                        }
                        break;
                    default:
                        strReason = "PAR1 INVALID : " + strParam;
                        bRes = false;
                        return bReturnBytes;
                }
                //int iPoint

                if (STEPMANAGER_VALUE.bOldBinaryALDL.Length != bDataData.Length)
                {
                    strReason = "NOT FOUND ALDL OLD DATA";
                    bRes = false;
                    return bReturnBytes;
                }

                if (iPoint >= bDataData.Length)
                {
                    strReason = "DATA POINTER OVER FLOW";
                    bRes = false;
                    return bReturnBytes;
                }

                bTempBlock = HexStringToBytes(strParseData[0], ref bConvOK1);

                if (bTempBlock.Length.Equals(2))
                {
                    //little Endian 으로 변경
                    Array.Reverse(bTempBlock, 0, 2);

                    if (!bTempBlock.SequenceEqual(STEPMANAGER_VALUE.bOldBinaryBLCK))
                    {
                        strReason = "MISSING BLOCK ADDRESS";
                        bRes = false;
                        return bReturnBytes;
                    }
                }


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
                    if (iPoint > 0 || bEndPoint)
                    {
                        Array.Copy(STEPMANAGER_VALUE.bOldBinaryALDL, 0, bDataData, 0, STEPMANAGER_VALUE.bOldBinaryALDL.Length);
                        Array.Copy(bTempData, 0, bDataData, iPoint, bTempData.Length);
                        bDataLen[0] = (byte)(iPoint + bTempData.Length);
                    }
                    else
                    {
                        Array.Copy(bTempData, 0, bDataData, 0, bTempData.Length);
                    }

                    for (int i = 0; i < bDataFlag.Length; i++)
                    {
                        bDataFlag[i] = (byte)(bDataData[i] ^ STEPMANAGER_VALUE.bOldBinaryALDL[i]);
                    }


                    bDataData = new byte[ALDLMAX]; //데이터 재초기화.
                    //int j = 0;
                    for (int i = 0; i < bDataFlag.Length; i++)
                    {
                        if (bDataFlag[i] != 0x00)
                        {
                            if (i < bTempData.Length + iPoint)
                            {
                                bDataData[i] = bTempData[i - iPoint];
                                //j++;
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

                Array.Copy(bDataBlock, 0, bReturnBytes, 0, bDataBlock.Length);
                Array.Copy(bDataLen, 0, bReturnBytes, 2, bDataLen.Length);
                Array.Copy(bDataData, 0, bReturnBytes, 3, bDataData.Length);
                Array.Copy(bDataFlag, 0, bReturnBytes, ALDLMAX + 3, bDataFlag.Length);

                bRes = true;
                return bReturnBytes;

            }


            catch
            {
                strReason = "MakeGen11_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
            }
        }

        private byte[] MakeGen12_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {
            int ALDLMAX = STEPMANAGER_VALUE.GetGen11AldlBlockSize();

            byte[] bReturnBytes = new byte[STEPMANAGER_VALUE.bOldBinaryALDL.Length + 2];
            try
            {
                // 1. unsigned short block;     2byte
                // 2. unsigned char  len;       1byte
                // 3. unsigned char  data[150]; 150byte
                // 4. unsigned char  lflag[150];150byte

                byte[] bDataBlock = new byte[2];
                byte[] bDataLen = new byte[1];
                byte[] bDataData = new byte[STEPMANAGER_VALUE.bOldBinaryALDL.Length]; // GEN10-TCP 150, GEN11-TCP 200 - 50증가, GEN11 MY23 321증가
                byte[] bDataFlag = new byte[STEPMANAGER_VALUE.bOldBinaryALDL.Length]; // GEN10-TCP 150, GEN11-TCP 200 - 50증가, GEN11 MY23 321증가

                byte[] bTempBlock;
                byte[] bTempData;

                bool bConvOK1 = false;
                bool bConvOK2 = false;

                int iPoint = 0;
                bool bEndPoint = false; //특정구간만 WRITE 할 경우.

                string[] strParseData = strParam.Split(',');

                switch (strParseData.Length)
                {
                    case 2:
                        iPoint = 0;
                        break;
                    case 3:
                        try
                        {
                            iPoint = int.Parse(strParseData[2]);
                            bEndPoint = true;
                        }
                        catch
                        {
                            strReason = "PAR1 ERROR : " + strParseData[2];
                            bRes = false;
                            return bReturnBytes;
                        }
                        break;
                    default:
                        strReason = "PAR1 INVALID : " + strParam;
                        bRes = false;
                        return bReturnBytes;
                }
                //int iPoint

                //if (STEPMANAGER_VALUE.bOldBinaryALDL.Length != bDataData.Length)
                if (STEPMANAGER_VALUE.bOldBinaryALDL.Length < 1)
                {
                    strReason = "NOT FOUND ALDL OLD DATA";
                    bRes = false;
                    return bReturnBytes;
                }

                if (iPoint >= STEPMANAGER_VALUE.bOldBinaryALDL.Length)
                {
                    strReason = "DATA POINTER OVER FLOW";
                    bRes = false;
                    return bReturnBytes;
                }
                
                bTempBlock = HexStringToBytes(strParseData[0], ref bConvOK1);

                if (bTempBlock.Length.Equals(2))
                {
                    //little Endian 으로 변경
                    Array.Reverse(bTempBlock, 0, 2);

                    //GEN12 에서는 없음. BLCK 정보가.
                    //if (!bTempBlock.SequenceEqual(STEPMANAGER_VALUE.bOldBinaryBLCK))
                    //{
                    //    strReason = "MISSING BLOCK ADDRESS";
                    //    bRes = false;
                    //    return bReturnBytes;

                    //}
                }


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

                if (bEndPoint)
                {
                    if (iPoint + bTempData.Length > STEPMANAGER_VALUE.bOldBinaryALDL.Length)
                    {
                        strReason = "DATA Length OVER FLOW";
                        bRes = false;
                        return bReturnBytes;
                    }
                }

                if (bConvOK1 && bConvOK2)
                {
                    //Array.Copy(bTempBlock, 0, bDataBlock, 0, bTempBlock.Length);
                    //if (iPoint > 0 || bEndPoint)
                    //{
                        Array.Copy(STEPMANAGER_VALUE.bOldBinaryALDL, 0, bDataData, 0, STEPMANAGER_VALUE.bOldBinaryALDL.Length);
                        Array.Copy(bTempData, 0, bDataData, iPoint, bTempData.Length);
                        bDataLen[0] = (byte)(iPoint + bTempData.Length);
                    //}
                    //else
                    //{
                    //    Array.Copy(bTempData, 0, bDataData, 0, bTempData.Length);
                    //}

                    //for (int i = 0; i < bDataFlag.Length; i++)
                    //{
                    //    bDataFlag[i] = (byte)(bDataData[i] ^ STEPMANAGER_VALUE.bOldBinaryALDL[i]);
                    //}
                    
                    //for (int i = 0; i < bDataFlag.Length; i++)
                    //{
                    //    bDataFlag[i] = (byte)(bDataData[i] ^ STEPMANAGER_VALUE.bOldBinaryALDL[i]);
                    //}

                    /*
                    bDataData = new byte[ALDLMAX]; //데이터 재초기화.
                    //int j = 0;
                    for (int i = 0; i < bDataFlag.Length; i++)
                    {
                        if (bDataFlag[i] != 0x00)
                        {
                            if (i < bTempData.Length + iPoint)
                            {
                                bDataData[i] = bTempData[i - iPoint];
                                //j++;
                            }
                        }
                    }
                    */

                }
                else
                {
                    strReason = "PAR1 INVALID1 : " + strParam;
                    bRes = false;
                    return bReturnBytes;
                }

                //Array.Copy(bDataBlock, 0, bReturnBytes, 0, bDataBlock.Length);
                //Array.Copy(bDataLen, 0, bReturnBytes, 2, bDataLen.Length);
                //Array.Copy(bDataData, 0, bReturnBytes, 3, bDataData.Length);
                //Array.Copy(bDataFlag, 0, bReturnBytes, ALDLMAX + 3, bDataFlag.Length);

                //bTempBlock
                Array.Copy(bDataData, 0, bReturnBytes, 0, bDataData.Length);
                Array.Copy(bTempBlock, 0, bReturnBytes, bDataData.Length, bTempBlock.Length);

                //write 할 때 변경, 현재값
                Array.Copy(bDataData, 0, STEPMANAGER_VALUE.bOldBinaryALDL, 0, bDataData.Length);


                bRes = true;
                return bReturnBytes;

            }
            catch
            {
                strReason = "MakeGen12_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
            }
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

        public int CheckProtocol(byte[] tmpByteArrays, byte[] byteOrginSendData, string strCommandName, ref string rtnData)
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

                for (int i = 7; i < 11; i++)
                {
                    if (tmpDecode[i] != byteOrginSendData[i])
                    {
                        return (int)STATUS.RUNNING;
                    }
                }

                //데이터 수집 구간 시작------------------------------------------             
                int iDataLen = (int)tmpDecode[12] + ((int)tmpDecode[13] * (int)0x100);
                bool bTMRes = false;
                //PACKTYPE gen11p = GetGen11ResFormat(LstTBLGen11, strCommandName, ref bgen11pRes);
                SETPACK Opack = GetWaveResFormat(LstTBLtm, strCommandName, ref bTMRes);


                byte[] bData = new byte[iDataLen];
                if (iDataLen > 0)
                {
                    try
                    {
                        if (Opack.bSplit) //head 가 있는 데이터면, 헤더는 오리지날과 비교하고 나머지를 데이터로 사용해야함....으.....
                        {
                            if (tmpDecode[14] != byteOrginSendData[14])
                            {
                                rtnData = "Index Mismatch";
                                return (int)STATUS.NG;
                            }

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
                double gi = 0.0;
                Single si = 0;
                if (bTMRes)
                {
                    if (Opack.iType != (int)GEN12RESTYPE.NONE && bData.Length < 1)
                    {
                        rtnData = "NO DATA";
                        return (int)STATUS.CHECK;
                    }

                    if (Opack.iCount > -1 && bData.Length < Opack.iCount + 1)
                    {
                        rtnData = "RESPONSE DATA SIZE ERROR.";
                        return (int)STATUS.CHECK;
                    }

                    switch (Opack.iType)
                    {
                        case (int)GEN12RESTYPE.BYTE1: //바이트문자한개만 고정하여 표시
                            if (Opack.iCount == -1)
                            {
                                rtnData = bData[0].ToString("X2");
                            }
                            else
                            {
                                rtnData = bData[Opack.iCount].ToString("X2");
                            }
                            break;

                        case (int)GEN12RESTYPE.BYTE: //데이터 전체 길이를 바이트문자열 2자리로 고정하여 표시
                            for (int i = 0; i < bData.Length; i++)
                            {
                                rtnData += bData[i].ToString("X2");
                            }
                            break;

                        case (int)GEN12RESTYPE.ADC:
                            int iSendIndex = 0;
                            int iRecvIndex = 0;
                            try
                            {
                                iSendIndex = BitConverter.ToInt16(byteOrginSendData, 14);
                                iRecvIndex = BitConverter.ToInt16(bData, 0);
                                if (!iSendIndex.Equals(iRecvIndex))
                                {
                                    rtnData = "ADC CHANNEL ERROR(INDEX)";
                                    return (int)STATUS.CHECK;
                                }
                            }
                            catch
                            {
                                rtnData = "ADC CHANNEL EXCEPTION(INDEX)";
                                return (int)STATUS.CHECK;
                            }

                            UInt32 iAdc = 0;

                            try
                            {
                                iAdc = BitConverter.ToUInt32(bData, 2);
                                rtnData = iAdc.ToString();
                            }
                            catch
                            {
                                rtnData = "ADC VALUE ERROR(UINT32)";
                                return (int)STATUS.CHECK;
                            }

                            break;

                        case (int)GEN12RESTYPE.CHAR:
                            rtnData = Encoding.UTF8.GetString(bData);
                            rtnData = rtnData.Replace("\0", String.Empty);
                            rtnData = rtnData.Replace("?", String.Empty);
                            rtnData = rtnData.Trim();
                            break;

                        case (int)GEN12RESTYPE.TEXT:
                            rtnData = Encoding.UTF8.GetString(bData);
                            break;

                        case (int)GEN12RESTYPE.INT:
                            if (Opack.iCount == -1)
                            {
                                for (int i = 0; i < bData.Length; i++)
                                {
                                    iSum += (int)bData[i];
                                }
                            }
                            else
                            {
                                if (bData.Length >= Opack.iCount)
                                {
                                    iSum = (int)bData[Opack.iCount];
                                }
                                else
                                {
                                    iSum = -9999;
                                }
                            }
                            rtnData = iSum.ToString();
                            break;

                        case (int)GEN12RESTYPE.DOUBLE: //8바이트 표시
                            try
                            {
                                gi = BitConverter.ToDouble(bData, 0);
                                rtnData = gi.ToString("#.##");
                            }
                            catch
                            {
                                rtnData = "DATA TYPE ERROR(NO DOUBLE)";
                                return (int)STATUS.CHECK;
                            }
                            break;

                        case (int)GEN12RESTYPE.SINGLE: //4바이트 표시
                            Single[] siArray = new Single[iDataLen / 4];
                            byte[] bSection = new byte[4];
                            try
                            {
                                if (Opack.iCount == -1)
                                {
                                    Array.Copy(bData, 0, bSection, 0, 4);
                                    siArray[0] = BitConverter.ToSingle(bSection, 0);
                                    rtnData = siArray[0].ToString();
                                }
                                else
                                {

                                    for (int i = 0; i < siArray.Length; i++)
                                    {
                                        Array.Copy(bData, i * 4, bSection, 0, 4);
                                        siArray[i] = BitConverter.ToSingle(bSection, 0);
                                    }
                                    rtnData = siArray[Opack.iCount].ToString();
                                }
                            }
                            catch
                            {
                                rtnData = "Error. Size(" + iDataLen.ToString() + ") Data(" + bData.Length.ToString() + " POS(" + Opack.iCount + ")";
                                return (int)STATUS.CHECK;
                            }
                            break;

                        case (int)GEN12RESTYPE.UINT16: //2바이트                                                     
                            try
                            {
                                UInt16 u16 = 0;// BitConverter.ToInt16(bData, 0);

                                if (Opack.iCount == -1)
                                {
                                    u16 = BitConverter.ToUInt16(bData, 0);
                                }
                                else
                                {
                                    u16 = BitConverter.ToUInt16(bData, Opack.iCount);
                                }
                                rtnData = u16.ToString();
                            }
                            catch
                            {
                                rtnData = "DATA TYPE ERROR(NO UINT16)";
                                return (int)STATUS.CHECK;
                            }
                            break;

                        case (int)GEN12RESTYPE.INT16: //2바이트                                                     
                            try
                            {
                                Int16 i16 = 0;// BitConverter.ToInt16(bData, 0);

                                if (Opack.iCount == -1)
                                {
                                    i16 = BitConverter.ToInt16(bData, 0);
                                }
                                else
                                {
                                    i16 = BitConverter.ToInt16(bData, Opack.iCount);
                                }
                                rtnData = i16.ToString();
                            }
                            catch
                            {
                                rtnData = "DATA TYPE ERROR(NO INT16)";
                                return (int)STATUS.CHECK;
                            }
                            break;

                        case (int)GEN12RESTYPE.INT32: //4바이트 
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

                        case (int)GEN12RESTYPE.BINT16: //2바이트 Big Endian                                                  
                            try
                            {
                                Int16 i16 = 0;// BitConverter.ToInt16(bData, 0);

                                if (Opack.iCount == -1)
                                {
                                    Array.Reverse(bData);
                                    i16 = BitConverter.ToInt16(bData, 0);
                                }
                                else
                                {
                                    Array.Reverse(bData, Opack.iCount, bData.Length - Opack.iCount);
                                    i16 = BitConverter.ToInt16(bData, Opack.iCount);
                                }
                                rtnData = i16.ToString();
                            }
                            catch
                            {
                                rtnData = "DATA TYPE ERROR(NO INT16)";
                                return (int)STATUS.CHECK;
                            }
                            break;


                        case (int)GEN12RESTYPE.DTC:   //리턴으로 오는코드값 검증해야함.
                            try
                            {
                                rtnData = Convert.ToString(bData[0], 2).PadLeft(8, '0');
                            }
                            catch
                            {
                                rtnData = "DATA TYPE ERROR(DTC)";
                                return (int)STATUS.CHECK;
                            }
                            break;

                        case (int)GEN12RESTYPE.NONE:
                            if (bData.Length > 1)
                            {
                                rtnData = "DETECTED USERDATA. CHECK PROTOCOL.";
                                return (int)STATUS.CHECK;
                            }
                            rtnData = String.Empty;
                            break;

                        case (int)GEN12RESTYPE.TTFF: //                                                   
                            try
                            {
                                UInt16 u16 = 0;

                                if (Opack.iCount == -1)
                                {
                                    u16 = BitConverter.ToUInt16(bData, 0);
                                }
                                else
                                {
                                    u16 = BitConverter.ToUInt16(bData, Opack.iCount);
                                }

                                rtnData = (u16 * 0.001).ToString("0.000");
                            }
                            catch
                            {
                                rtnData = "DATA TYPE ERROR(NO TTFF)";
                                return (int)STATUS.CHECK;
                            }
                            break;


                        case (int)GEN12RESTYPE.SELFTEST: //                                                   

                            byte bResByte = 0x00;

                            if (bData.Length == 5)
                            {
                                bResByte = bData[4];
                            }
                            else
                            {
                                rtnData = "DATA TYPE ERROR(SELF TEST)";
                                return (int)STATUS.CHECK;
                            }

                            switch (bResByte)
                            {
                                case 0x00: rtnData = "TEST_NOT_STARTED"; break;
                                case 0x01: rtnData = "TEST_RUNNING"; break;
                                case 0x02: rtnData = "TEST_FINISHED_WITHOUT_ERROR"; break;
                                case 0x03: rtnData = "TEST_FINISHED_WITH_ERROR"; break;
                                case 0x04: rtnData = "TEST_HALTED"; break;
                                default: rtnData = "UNKNOWN_ERROR_CODE"; break;
                            }
                            break;

                        //gen11에서 가져옴
                        case (int)GEN12RESTYPE.IMEICHKSUM:
                            rtnData = Encoding.UTF8.GetString(bData);
                            rtnData = rtnData.Replace("\0", String.Empty);
                            rtnData = rtnData.Replace("?", String.Empty);
                            rtnData = rtnData.Trim();
                            if (rtnData.Length.Equals(15))
                                rtnData = rtnData.Substring(14, 1);
                            else
                            {
                                rtnData = "SIZE ERROR.";
                                return (int)STATUS.NG;
                            }
                            break;

                        case (int)GEN12RESTYPE.ALDL_HEXA:
                        case (int)GEN12RESTYPE.ALDL_ASCII:                        
                            if (!CheckGen11_ALDLType(byteOrginSendData, bData, ref rtnData, ref STEPMANAGER_VALUE.bOldBinaryALDL, Opack.iType))
                            {
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2];
                                return (int)STATUS.NG;
                            }
                            break;
                        case (int)GEN12RESTYPE.ALDL12_HEXA:
                        case (int)GEN12RESTYPE.ALDL12_ASCII:
                            if (!CheckGen12_ALDLType(byteOrginSendData, bData, ref rtnData, ref STEPMANAGER_VALUE.bOldBinaryALDL, Opack.iType))
                            {
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2];
                                return (int)STATUS.NG;
                            }
                            break;
                        case (int)GEN12RESTYPE.ALDL_BITS:
                            if (!CheckGen11_ALDLType(byteOrginSendData, bData, ref rtnData, ref STEPMANAGER_VALUE.bOldBinaryALDL, Opack.iType))
                            {
                                STEPMANAGER_VALUE.bOldBinaryBLCK = new byte[2];
                                return (int)STATUS.NG;
                            }
                            string strParam = rtnData;
                            rtnData = ChangeBitsArray(strParam);
                            break;


                        case (int)GEN12RESTYPE.NVITEM:

                            if (bData.Length != 58)
                            {
                                rtnData = "Recieve Data Size Error.";
                                return (int)STATUS.NG;
                            }

                            try
                            {
                                Int32 i32Id = BitConverter.ToInt32(bData, 0);
                                Int32 i32Len = BitConverter.ToInt32(bData, 4);
                                byte[] bNvData = new byte[50];
                                Array.Copy(bData, 8, bNvData, 0, 50);
                                string strNvData = String.Empty;
                                for (int i = 0; i < i32Len; i++)
                                {
                                    strNvData += bNvData[i].ToString("X2");
                                }

                                rtnData = i32Id.ToString() + "," + i32Len.ToString() + "," + strNvData;
                            }
                            catch
                            {
                                rtnData = "Recieve Data Size Error.";
                                return (int)STATUS.NG;
                            }

                            break;

                        case (int)GEN12RESTYPE.UINT32A:
                            UInt32 ui32A = 0;
                            try
                            {
                                ui32A = BitConverter.ToUInt32(bData, 0);
                                rtnData = ui32A.ToString();
                            }
                            catch
                            {
                                rtnData = "Data type Err.";
                                return (int)STATUS.NG;
                            }
                            break;
                        case (int)GEN12RESTYPE.UINT32B:
                            UInt32 ui32B = 0;
                            try
                            {
                                ui32B = BitConverter.ToUInt32(bData, 4);
                                rtnData = ui32B.ToString();
                            }
                            catch
                            {
                                rtnData = "Data type Err.";
                                return (int)STATUS.NG;
                            }

                            break;

                        case (int)GEN12RESTYPE.BUBCAL:
                            Int16 iValue = 0;
                            try
                            {
                                if (bData.Length == 3)
                                {
                                    iValue = BitConverter.ToInt16(bData, 1);
                                    double dVal = (double)iValue / (double)1000;
                                    if (bData[0] == 0x01)
                                    {
                                        rtnData = dVal.ToString("0.000");
                                    }
                                    else
                                    {
                                        rtnData = "FAIL(" + dVal.ToString("0.000") + ")";
                                        return (int)STATUS.NG;
                                    }
                                }
                                else
                                {
                                    rtnData = "RESPONSE SIZE ERROR.";
                                    return (int)STATUS.NG;
                                }
                            }
                            catch
                            {
                                rtnData = "PARSE ERROR.";
                                return (int)STATUS.NG;
                            }

                            break;

                        case (int)GEN12RESTYPE.WIFIRX1: //모두 표시
                        case (int)GEN12RESTYPE.WIFIRX2: //STATE 만 표시
                        case (int)GEN12RESTYPE.WIFIRX3: //TOTAL PACKET 만 표시
                        case (int)GEN12RESTYPE.WIFIRX4: //GOOD  PACKET 만 표시
                        case (int)GEN12RESTYPE.WIFIRX5: //RATE 로 표시

                            int iState = 0;  // RX 상태 (-1:Invalid, 0:Starting, 1:Started, 2:Stopping, 3:Stopped)
                            int iTotal = 0;  // Total Rx Packet
                            int iGood = 0;  // Good Rx Packet
                            double dRate = 0;  // Good Rx Packet

                            try
                            {
                                if (bData.Length == 12)
                                {
                                    iState = BitConverter.ToInt16(bData, 0);
                                    iTotal = BitConverter.ToInt16(bData, 4);
                                    iGood = BitConverter.ToInt16(bData, 8);

                                    if (iTotal > 0)
                                        dRate = Math.Abs(100 - ((double)iGood / (double)iTotal * 100));
                                    else
                                        dRate = 100;

                                    switch (Opack.iType)
                                    {
                                        case (int)GEN12RESTYPE.WIFIRX1:
                                            rtnData = iState.ToString() + ","
                                                    + iTotal.ToString() + ","
                                                    + iGood.ToString(); break;//모두 표시
                                        case (int)GEN12RESTYPE.WIFIRX2: rtnData = iState.ToString(); break;//STATE 만 표시
                                        case (int)GEN12RESTYPE.WIFIRX3: rtnData = iTotal.ToString(); break;//TOTAL PACKET 만 표시
                                        case (int)GEN12RESTYPE.WIFIRX4: rtnData = iGood.ToString(); break;//GOOD  PACKET 만 표시
                                        case (int)GEN12RESTYPE.WIFIRX5: rtnData = dRate.ToString("0.00"); break;//RATE 로 표시
                                        default: rtnData = "TYPE ERROR."; break;
                                    }
                                }
                                else
                                {
                                    rtnData = "RESPONSE SIZE ERROR.";
                                    return (int)STATUS.NG;
                                }
                            }
                            catch
                            {
                                rtnData = "PARSE ERROR.";
                                return (int)STATUS.NG;
                            }

                            break;


                        case (int)GEN12RESTYPE.FEATUREID:
                            //구조체 사이즈 체크
                            if (bData.Length != 23)
                            {
                                return (int)STATUS.TIMEOUT;
                            }
                            // 1. FID 비교
                            byte[] bOriginID = new byte[2];
                            bOriginID[0] = byteOrginSendData[12];
                            bOriginID[1] = byteOrginSendData[13];

                            if (bData[0].Equals(bOriginID[0]) && bData[1].Equals(bOriginID[1]))
                            {
                                int iFIDLength = BitConverter.ToInt32(bData, 3);

                                // 2. Length 확인
                                if (iFIDLength.Equals(0))
                                {
                                    rtnData = bData[2].ToString("X2");
                                }
                                else
                                {
                                    if (iFIDLength <= 16)
                                    {
                                        for (int ix = 0; ix < iFIDLength; ix++)
                                        {
                                            rtnData += bData[ix + 7].ToString("X2");
                                        }
                                    }
                                    else
                                    {
                                        rtnData = "FeatureID Data Length Over";
                                        return (int)STATUS.NG;
                                    }
                                }

                            }
                            else
                            {
                                rtnData = "Another FeatureID";
                                return (int)STATUS.NG;
                            }
                            break;
                        case (int)GEN12RESTYPE.SERVICE:
                            if (!CheckNAD_ServiceTypeB(byteOrginSendData, bData, ref rtnData))
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

                //Success or Failur 구분 코드.
                switch (tmpDecode[11])
                {
                    case RES_SUCCESS:
                        return (int)STATUS.OK;

                    case RES_FAILURE:

                        if (IsExceptionCommand(strCommandName)) //예외명령 처리.
                        {
                            return (int)STATUS.OK;
                        }
                        rtnData = "ERROR CODE:FAILURE";
                        return (int)STATUS.NG;

                    case RES_UNDEFINED:
                        rtnData = "ERROR CODE:UNDEFINED";
                        return (int)STATUS.NG;

                    default:
                        rtnData = "ERROR CODE:UNKNOWN";
                        return (int)STATUS.NG;
                }

            }
            catch (Exception ex)
            {
                rtnData = "CheckProtocol Exception2:" + ex.Message.ToString();
                return (int)STATUS.CHECK;
            }


        }

        private bool IsExceptionCommand(string strCommandName)
        {
            switch (strCommandName)
            {
                //해당 명령은 failure 여도 pass 처리하자.
                case "SET_AUDIO_LOOPBACK_START":
                case "SET_AUDIO_LOOPBACK_STOP":

                    return true;

                default: return false;
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

        public int AnalyzePacket(byte[] tmpBytes, ref string rtnData, byte[] strOrginSendData, string strCommandName /* ref string strLogString*/)
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

                        bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData);

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
            catch (Exception ex)
            {
                tmpLogger.WriteCommLog("Exception: AnalyzePacket(WAVE) : ", ex.Message, false);
                return (int)STATUS.RUNNING;
            }

        }

        public byte[] ConvertWaveByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason, bool bCallClass)
        {   //strSendPack은 TX에서 로깅하기 위해사용한다. rx에서는 따로 해주어야한다.
            //bCallClass 는 아날라이져에서 부른건지 COMM 에서 부른건지 알수 없기때문에 생긴거다. 이유는 인증서 카운팅을 막아야하기 때문이다.
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();
            strReason = "OK";
            string strValue = String.Empty;
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
                       
                             
                        int iEfileType = (int)EFILETYPE.CHECK;

                        switch (tmpString[i])
                        {
                            case "EFILE1": iEfileType = (int)EFILETYPE.CHECK; break;
                            case "EFILE2": iEfileType = (int)EFILETYPE.TRANSFER; break;
                            case "EFILE3": iEfileType = (int)EFILETYPE.COMPLETE; break;
                            case "EFILE4": iEfileType = (int)EFILETYPE.TRANSFER; break; //GEN12용
                        }

                        switch (tmpString[i])
                        {
                            case "EFILE1":  //EFILE1,2,3 은 인증서업로드용이다.(For GEN11)
                            case "EFILE2":
                            case "EFILE3":
                            case "EFILE4":
                                bOk = true;
                                if(tmpString[i] == "EFILE4")
                                    bParam = MakeEfileStruct_Trans(iEfileType, strParam, ref bOk, ref strReason, bCallClass);
                                else
                                //bParam = MakeEfileStruct(iEfileType, strParam, ref bOk, ref strReason, bCallClass);
                                {
                                    strReason = "TYPE IS NOT SUPPORT IN GEN12";
                                    brtnOk = false;
                                    byte[] bErrorByte = new byte[10];
                                    return bErrorByte;
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
                                    strReason = "DATA TYPE ERROR(" + tmpString[i] + ") - " + strReason;
                                    brtnOk = false;
                                    byte[] bErrorByte = new byte[10];
                                    return bErrorByte;
                                }
                                break;

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

                            case "BYTE2": //두개짜리
                                /* 기본 
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
                                */
                                //GEN11 형
                                bOk = true;
                                bParam = HexStringToBytes(strParam, ref bOk);
                                iDataSize = 0;
                                if (bOk || bParam.Length > 2)
                                {
                                    string tmpCharHigh = String.Empty;
                                    string tmpCharLow = String.Empty;
                                    if (bParam.Length.Equals(2))
                                    {
                                        tmpCharHigh = String.Format("{0:X2}", bParam[0]);
                                        tmpCharLow = String.Format("{0:X2}", bParam[1]);
                                        tmpList.Add(tmpCharLow); iDataSize++;
                                        tmpList.Add(tmpCharHigh); iDataSize++;
                                    }

                                    if (bParam.Length.Equals(1))
                                    {
                                        tmpCharHigh = "00";
                                        tmpCharLow = String.Format("{0:X2}", bParam[0]);
                                        tmpList.Add(tmpCharLow); iDataSize++;
                                        tmpList.Add(tmpCharHigh); iDataSize++;
                                    }
                                }
                                else
                                {
                                    tmpList.Add("FF");
                                    strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                    brtnOk = false;
                                    //return rtnNull;
                                }
                                break;

                            case "INT16":
                                Int16 i16 = 0;
                                try
                                {
                                    int iii = (int)(double.Parse(strParam));
                                    i16 = (Int16)(iii);
                                    byte[] bInt16Data = new byte[2];
                                    bInt16Data = BitConverter.GetBytes(i16);
                                    for (int p = 0; p < bInt16Data.Length; p++)
                                    {
                                        string tmpChar = String.Format("{0:X2}", bInt16Data[p]);
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

                            case "INT32":
                                Int32 i32 = 0;
                                try
                                {
                                    int iii = (int)(double.Parse(strParam));
                                    i32 = (Int32)(iii);
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

                            case "2INT16":
                                iDataSize = 0;
                                string[] strColor = strParam.Split(',');

                                if (strColor.Length != 2)
                                {
                                    tmpList.Add("FF");
                                    strReason = "PAR1 ERROR";
                                    brtnOk = false;
                                    break;
                                }

                                Int16 iDuration = 0;
                                Int16 iFrequency = 0;
                                try
                                {
                                    iDuration = Int16.Parse(strColor[0]);
                                    iFrequency = Int16.Parse(strColor[1]);
                                }
                                catch
                                {
                                    tmpList.Add("FF");
                                    strReason = "PAR1 TYPE ERROR";
                                    brtnOk = false;
                                    break;
                                }

                                byte[] bTmp16Data = new byte[2];
                                bTmp16Data = BitConverter.GetBytes(iDuration);
                                for (int p = 0; p < bTmp16Data.Length; p++)
                                {
                                    string tmpChar = String.Format("{0:X2}", bTmp16Data[p]);
                                    tmpList.Add(tmpChar);
                                    iDataSize++;
                                }

                                bTmp16Data = BitConverter.GetBytes(iFrequency);
                                for (int p = 0; p < bTmp16Data.Length; p++)
                                {
                                    string tmpChar = String.Format("{0:X2}", bTmp16Data[p]);
                                    tmpList.Add(tmpChar);
                                    iDataSize++;
                                }
                                break;

                            case "ALDL_HEXA":   //ALDL포멧으로 만들되 파라미터는 HEX형태로 만든다.
                            case "ALDL_ASCII":  //ALDL포멧으로 만들되 파라미터는 아스키로 만든다.
                            case "ALDL_HEXB":  //ALDL포멧으로 만들되 파라미터는 HEX형태이며 특정 바이트의 특정 비트만 ON 할경우.
                            case "ALDL_HEXC":  //ALDL포멧으로 만들되 파라미터는 HEX형태이며 특정 바이트의 특정 비트만 ON 할경우.
                            case "ALDL12_HEXA":
                            case "ALDL12_ASCII":
                                bOk = true;
                                strValue = String.Empty;
                                bParam = new byte[1];

                                switch (tmpString[i])
                                {
                                    case "ALDL_HEXA": brtnOk = true; break;
                                    case "ALDL_ASCII": brtnOk = true; break;
                                    case "ALDL_HEXB": brtnOk = MakeToggleAldl(true, strParam, ref strValue, ref strReason); break;
                                    case "ALDL_HEXC": brtnOk = MakeToggleAldl(false, strParam, ref strValue, ref strReason); break;
                                    case "ALDL12_HEXA": brtnOk = true; break;
                                    case "ALDL12_ASCII": brtnOk = true; break;
                                    default: brtnOk = false; break;
                                }

                                if (!brtnOk)
                                {
                                    tmpList.Add("FF");                                    
                                    strReason = "PAR1 TYPE ERROR";
                                    brtnOk = false;
                                    break;
                                    //return rtnNull;
                                }

                                switch (tmpString[i])
                                {
                                    case "ALDL_HEXA": bParam = MakeGen11_ALDLType(strParam, ref bOk, ref strReason, false); break;
                                    case "ALDL_ASCII": bParam = MakeGen11_ALDLType(strParam, ref bOk, ref strReason, true); break;
                                    case "ALDL_HEXB": bParam = MakeGen11_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                    case "ALDL_HEXC": bParam = MakeGen11_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                    case "ALDL12_HEXA": bParam = MakeGen12_ALDLType(strParam, ref bOk, ref strReason, false); break;
                                    case "ALDL12_ASCII": bParam = MakeGen12_ALDLType(strParam, ref bOk, ref strReason, true); break;
                                    default: bOk = false; break;
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
                                    strReason = "ERROR MSG : (" + tmpString[i] + ") " + strReason;
                                    //brtnOk = false;
                                    //return rtnNull;

                                    //strReason = "PAR1 TYPE ERROR";
                                    brtnOk = false;
                                    break;
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
                            if (tmpList[j].Equals("<SIZE:DATA>") || tmpList[j].Equals("<SIZE:DATA+1>"))
                            {
                                bool bPlus1 = tmpList[j].Equals("<SIZE:DATA+1>");

                                if (bPlus1) iDataSize = iDataSize + 1;

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
                            if (iLenSize > (int)0xFF)  //TM little endian 방식을 따른다.
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

        public static byte[] HexStringToBytes(string s, ref bool bOk)
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

        private bool CheckNAD_ServiceTypeB(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
        {
            try
            {
                byte[] bParseData = new byte[bGetData.Length - 1]; //전체데이터 복사본
                byte[] bLenth = new byte[2];                       //길이 재는 용도

                STEPMANAGER_VALUE.OOBServiceClear();               //데이터 초기화.

                Array.Copy(bGetData, 1, bParseData, 0, bParseData.Length); //데이터에서 앞에 3바이트를 빼고 시작한다. 연구소도 인원이 교체되서 문서에도 없다. 문서와 구조체가 다르다. 난감하다.

                if (!bGetData.Length.Equals(305))
                {
                    strRetunData = "Data Size missmatch.";
                    return false;
                }

                int iDx = 0;
                for (int i = (int)0; i < (int)ServiceIndexB.END; i++)
                {
                    try
                    {
                        switch (i)
                        {
                            case (int)ServiceIndexB.szMEID: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexB.szICCID: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 30); iDx += 30; break;
                            case (int)ServiceIndexB.cRadioIF: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.nActiveChannel: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.nCurrentSID: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.iTxPwr: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.cCallState: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;

                            case (int)ServiceIndexB.szDialedDigits: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 33); iDx += 33; break;
                            case (int)ServiceIndexB.nPilotPN: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.cServiceDomain: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexB.cRSSI: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexB.nErrorRate: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.cDTMFEvent: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.nServiceOption: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.cServiceStatus: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexB.szIMEI: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexB.cCSAttachState: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexB.cPSAttachState: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.nMCC: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.nMNC: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.iArfcn: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iBSIC: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.cECIO: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iCellID: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iUARFCN: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.cAttachState: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iEARFCN: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iLteSNR: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iLteRSRP: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.szBanner: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 50); iDx += 50; break;
                            case (int)ServiceIndexB.nTAC: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.iVocoder: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.iVocoderRate: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iRSRP: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.bBand_1900: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.nLAC: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.szMDN: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 20); iDx += 20; break;
                            case (int)ServiceIndexB.iServiceDomainPref: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.iScanList: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexB.bIsVoLTE: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
                            case (int)ServiceIndexB.szIncomingNumber: STEPMANAGER_VALUE.OOBServiceInfoB[i] = Encoding.UTF8.GetString(bParseData, iDx, 33); iDx += 33; break;
                            case (int)ServiceIndexB.cSINR: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 2).Replace("-", ""); iDx += 2; break;
                            case (int)ServiceIndexB.iRSRQ: STEPMANAGER_VALUE.OOBServiceInfoB[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            default:
                                break;
                        }

                    }
                    catch
                    {
                        strRetunData = "CheckNAD_ServiceTypeB:Exception1";
                        return false;
                    }

                }

                strRetunData = "OK";
                return true;
            }
            catch
            {
                strRetunData = "CheckNAD_ServiceTypeB:Exception2";
                return false;
            }


        }
    }
}
