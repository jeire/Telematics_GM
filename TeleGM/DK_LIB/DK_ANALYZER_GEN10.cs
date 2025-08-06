using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{   
    
    class DK_ANALYZER_GEN10
    {        
        private const byte DEFRES_STX1 = 0x02;
        private const byte DEFRES_STX2 = 0xFB;
        private const byte DEFRES_ETX  = 0xFA;

        private const string HEX_CHARS = "0123456789ABCDEF";

        private const string strWLresponse1 = "[COMMON]|=> systemx";
        private const string strWLresponse2 = "[COMMON]|MsgSend ";
           
        private List<TBLDATA0> LstTBLvcp = new List<TBLDATA0>();       //GEN10 명령 테이블 리스트
        private DK_LOGGER tmpLogger = new DK_LOGGER("PC", false);

        public DK_ANALYZER_GEN10()
        {
            
            tmpLogger.LoadTBL0("GEN10.TBL", ref LstTBLvcp);
        }

        public int CheckProtocol(byte[] tmpByteArray, byte[] strOrginSendData, string strCommandName, ref string rtnData, int iAanlyizeOption)
        {            
            //체크섬 검사.

            DK_CHECKSUM DKCHK = new DK_CHECKSUM();
            byte bChkHigh = 0x00;
            byte bChkLow  = 0x00;
            DKCHK.Gen10_chksum(tmpByteArray, ref bChkHigh, ref bChkLow, false);

            if (tmpByteArray[tmpByteArray.Length - 3] != bChkHigh ||
                tmpByteArray[tmpByteArray.Length - 2] != bChkLow)
            {
                return (int)STATUS.NG;
            }

            //예외명령인지 한번 체크            
            CheckExceptionCommands(strCommandName, ref strOrginSendData);
       
            //GPS 특별명령.
            bool bSpecialFlag = strCommandName.Equals("GEN10_OLD_GPS_INFO");

            try
            {                
                //보낸명령어에 대한 응답데이터인지 확인1
                for (int i = 4; i < 9; i++)
                {
                    if (i == 7 && bSpecialFlag) 
                    {
                        continue;//GEN10의 OLD GPS INFO 수집일경우는 7번째가 항상 다르기때문에 continue 한다.
                    }

                    if (tmpByteArray[i + 2] != strOrginSendData[i])
                    {
                        return (int)STATUS.RUNNING;
                    }
                }
                //데이터 수집 구간 시작------------------------------------------
                //int iDataLen = (int)tmpByteArray[5];
                int iDataLen = (int)tmpByteArray[5] + ((int)tmpByteArray[4] * (int)0x100);
          
                byte[] bData = new byte[iDataLen-8];

                for (int i = 0; i < iDataLen-8; i++)
                {
                    if (bSpecialFlag)
                        bData[i] = tmpByteArray[i + 12];
                    else
                        bData[i] = tmpByteArray[i + 11];
                }

                switch (iAanlyizeOption)
                {
                    case (int)ANALYIZEGEN10.GPSINFO1:
                    case (int)ANALYIZEGEN10.GPSINFO2:
                    case (int)ANALYIZEGEN10.GPSINFO3:
                    case (int)ANALYIZEGEN10.GPSINFO4:

                        if (bData.Length != 11) // Error Code, dummy1, dummy2, gps svcount, gps cn0 Max, gps cn0 Aver, gps cn0 min,
                                                // gnss svcount, gnss cn0 Max, gnss cn0 Aver, gnss cn0 min,
                        //그래서 11바이트로 체크
                        {
                            rtnData = "INFORMATION SIZE ERROR";
                            return (int)STATUS.NG;
                        }

                        if (bData[0] == 0x00) // Error Code
                        {
                            rtnData = "ERROR CODE 0";
                            return (int)STATUS.NG;
                        }
                        switch (iAanlyizeOption)
                        {
                            case (int)ANALYIZEGEN10.GPSINFO1: rtnData = bData[3].ToString(); break; //gps svcount
                            case (int)ANALYIZEGEN10.GPSINFO2: rtnData = bData[4].ToString(); break; //gps cn0 Max
                            case (int)ANALYIZEGEN10.GPSINFO3: rtnData = bData[7].ToString(); break; //gnss svcount
                            case (int)ANALYIZEGEN10.GPSINFO4: rtnData = bData[8].ToString(); break; //gnss cn0 Max
                            default: rtnData = "FF"; break;
                        }

                        break;
                    case (int)ANALYIZEGEN10.OLDGPSINFO:

                        STEPMANAGER_VALUE.SetOldGEN10GPSInfo(bData);

                        break;

                    case (int)ANALYIZEGEN10.TTFF:

                        byte[] bTtff = new byte[4];
                        if(bData.Length != 11)  //이거 TCP 랑 완전 똑같이 해서 result, datalength, data 이렇게 옴...헐
                                                //그래서 11바이트로 체크
                        {
                            rtnData = "TTFF SIZE ERROR";
                            return (int)STATUS.NG;   
                        }
                        for (int i = 0; i < bTtff.Length; i++)
                        {
                            //rtnData += bData[i].ToString("X2");
                            bTtff[i] = bData[i+7];
                        }
                        try
                        {
                            uint uTime = BitConverter.ToUInt32(bTtff, 0);
                            double dttff = (double)uTime / 1000;
                            if (dttff <= 0) return (int)STATUS.NG;
                            rtnData = dttff.ToString("0.###");
                        }
                        catch
                        {
                            rtnData = "TTFF ERROR.";
                            return (int)STATUS.NG;   
                        }
                        break;

                    case (int)ANALYIZEGEN10.ALDL_ASCII:
                        if (!CheckGen10_ALDLType(strOrginSendData, bData, ref rtnData,
                            ref STEPMANAGER_VALUE.bOldBinaryALDL))
                        {
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN10.ALDL_BITS:
                        if (!CheckGen10_ALDLType(strOrginSendData, bData, ref rtnData,
                            ref STEPMANAGER_VALUE.bOldBinaryALDL))
                        {
                            return (int)STATUS.NG;
                        }
                        string strParam = rtnData;
                        rtnData = ChangeBitsArray(strParam);
                        break;

                    case (int)ANALYIZEGEN10.DTC:
                        if (strOrginSendData.Length < 12) return (int)STATUS.NG;   
                        if (!CheckGEN10_DtcIndex(strOrginSendData[10], bData, ref rtnData))
                        {                                                      
                            return (int)STATUS.NG;   
                        }                     
                        break;

                    case (int)ANALYIZEGEN10.DTCOQA:
                        rtnData = Encoding.UTF8.GetString(bData);
                        rtnData = rtnData.Trim(); //공백제거.
                        rtnData = rtnData.Replace("-", String.Empty);
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;

                    case (int)ANALYIZEGEN10.OOBRESULT:
                        if (bData.Length < 5)
                        {
                            rtnData = "OOB RESULT SIZE ERROR";
                            return (int)STATUS.NG;
                        }
                        byte[] bOobResult = new byte[bData.Length - 3];
                        Array.Copy(bData, 3, bOobResult, 0, bOobResult.Length);
                        rtnData = Encoding.UTF8.GetString(bOobResult);                        
                        rtnData = rtnData.Trim(); //공백제거.
                        rtnData = rtnData.Replace("-", String.Empty);
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;
                    case (int)ANALYIZEGEN10.REVERSE:
                        if (!CheckGEN10_ReverseType(strCommandName, bData, ref rtnData))
                        {
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN10.SERVICE: //안제호 책임요청(2018.07.06)

                        if (!CheckNAD_ServiceType(strOrginSendData, bData, ref rtnData))
                        {
                            return (int)STATUS.NG;
                        }
                        
                        break;

                    case (int)ANALYIZEGEN10.SIMINFO:
                        if (!CheckGEN10_SimInfoType(strOrginSendData, bData, ref rtnData))
                        {
                            return (int)STATUS.NG;
                        }                                                    
                        break;

                    case (int)ANALYIZEGEN10.BYTE:
                        for (int i = 0; i < bData.Length; i++)
                        {
                            rtnData += bData[i].ToString("X2");
                        } 
                        break;

                    case (int)ANALYIZEGEN10.FINFOSIZE:

                        if (!bData[0].Equals(0x31))
                        {
                            return (int)STATUS.NG;
                        }

                        rtnData = Encoding.UTF8.GetString(bData, 1, bData.Length - 1 );
                        rtnData = rtnData.Trim();   //공백제거.
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;

                    case (int)ANALYIZEGEN10.FINFOBYTE:

                        if (!bData[0].Equals(0x31))
                        {
                            return (int)STATUS.NG;
                        }

                        for (int i = 5; i < bData.Length; i++)
                        {
                            rtnData += bData[i].ToString("X2");
                        }
                        break;

                    case (int)ANALYIZEGEN10.T_NADINFO: //TCP 타입
                    case (int)ANALYIZEGEN10.T_SINGLE: //TCP 타입

                        return CheckTCPTypeProtocol(bData, ref rtnData, iAanlyizeOption);
                        
                    case (int)ANALYIZEGEN10.CHECKSUM:
                        //문서에는 unsigned int 로 온다면서 실제로 해보면 아스키로온다....헐...
                        try
                        {
                            rtnData = Encoding.UTF8.GetString(bData, 1, bData.Length - 1);
                            rtnData = rtnData.Replace("\0", String.Empty);
                            rtnData = rtnData.Replace("?", String.Empty);
                            rtnData = rtnData.Trim();
                                                        
                            if (rtnData.Contains((char)0x20))
                            {
                                string[] strSplits = rtnData.Split((char)0x20);
                                if (strSplits.Length == 2)
                                {
                                    if (strOrginSendData.Length <= 13)
                                    {
                                        rtnData = "PAR1 SIZE ERROR";
                                        return (int)STATUS.NG;
                                    }

                                    string strPar1 = Encoding.UTF8.GetString(strOrginSendData, 10, strOrginSendData.Length - 3 - 10);
                                    if (strPar1.Equals(strSplits[0]))
                                    {
                                        rtnData = strSplits[1];
                                    }
                                    else
                                    {
                                        rtnData = "Different Path";
                                        return (int)STATUS.NG;
                                    }
                                    
                                }
                                else
                                {
                                    rtnData = "RESPONSE ERROR(SPACE1)";
                                    return (int)STATUS.NG;
                                }
                            }
                            else
                            {
                                rtnData = "RESPONSE ERROR(SPACE2)";
                                return (int)STATUS.NG;
                            }
                        }
                        catch
                        {
                            rtnData = "PACKET ERROR.";
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN10.RESCODE:                        
                        // 문서에는 첫번째 1byte 가 success 1 : fail : 0 로 처리해야하나  0x20 이 붙는다...     
                        // 테스트 샘플이라 그런것인지는 진짜 세트로 패킷확인해서 맞으면 success/fail 보는것으로 수정해야한다.
                        try
                        {
                            rtnData = Encoding.UTF8.GetString(bData, 1, bData.Length - 1);
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


                    case (int)ANALYIZEGEN10.APN_TABLE:

                        if (!CheckGen10_APN_TABLE(bData, ref rtnData))
                        {
                            return (int)STATUS.NG;
                        }
                        break;

                    default:
                        rtnData = Encoding.UTF8.GetString(bData);
                        rtnData = rtnData.Trim(); //공백제거.
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;
                }
                
                return (int)STATUS.OK;

            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message;
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                return (int)STATUS.CHECK;
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

        private int CheckTCPTypeProtocol(byte[] bData, ref string strRtndata, int iAnalyizeType)
        {
            //GEN10 강종훈 책임 이후에 업데이트 되는 프로토콜은 김훈겸 책임이 담당하므로 응답TYPE 이 TCP 처럼 되어있음.
            if (bData.Length >= 3)
            { //[0]    Success of Fail byte  01 Succeess
              //[1][2] Data Length 2byte : little Endian
              //[N]    datas
                if (bData[0] == 0x01)
                {
                    int iDataLen = BitConverter.ToInt16(bData, 1);
                    if (iDataLen + 3 == bData.Length)
                    {
                        switch (iAnalyizeType)
                        {
                            case (int)ANALYIZEGEN10.T_SINGLE:
                                Single si = BitConverter.ToSingle(bData, 3);
                                //int si = BitConverter.ToInt32(bData, 3);
                                strRtndata = si.ToString("0.##");
                                break;

                            case (int)ANALYIZEGEN10.T_NADINFO:
                            default:
                                strRtndata = Encoding.UTF8.GetString(bData, 3, bData.Length -3);
                                strRtndata = strRtndata.Trim(); //공백제거.
                                strRtndata = strRtndata.Replace("\0", String.Empty);
                                break;
                        }                       

                        return (int)STATUS.OK;
                    }
                    else
                    {
                        strRtndata = "Data Size Error";
                        return (int)STATUS.NG;
                    }
                }
                else
                {
                    strRtndata = "Result Code Fail";
                    return (int)STATUS.NG;
                }

            }
            else
            {
                strRtndata = "Protocol Type Error";
                return (int)STATUS.NG;
            }
            
        }

        private bool CheckNAD_ServiceType(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
        {
            try
            {
                byte[] bParseData = new byte[bGetData.Length - 3]; //전체데이터 복사본
                byte[] bLenth = new byte[2];                       //길이 재는 용도
                
                STEPMANAGER_VALUE.OOBServiceClear();               //데이터 초기화.
                
                Array.Copy(bGetData, 3, bParseData, 0, bParseData.Length); //데이터에서 앞에 3바이트를 빼고 시작한다. 연구소도 인원이 교체되서 문서에도 없다. 문서와 구조체가 다르다. 난감하다.
                //[0]이 OK NG [1] [2] 가 길이인듯.

                if (!bGetData[0].Equals(0x01))
                {
                    strRetunData = "Result Code Failure.";
                    return false;
                }

                ushort iLenth = BitConverter.ToUInt16(bGetData, 1);

                if (!iLenth.Equals(284)) //구조체 데이터 총합 284바이트임.
                {
                    strRetunData = "Data Size missmatch.";
                    return false;
                }
                               
                int iDx = 0;
                for (int i = (int)0; i < (int)ServiceIndexA.END; i++)
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
                            case (int)ServiceIndexA.iVocoder:       STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.iVocoderRate:   STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.iRSRP:          STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 4).Replace("-", ""); iDx += 4; break;
                            case (int)ServiceIndexA.cBand_1900:     STEPMANAGER_VALUE.OOBServiceInfoA[i] = BitConverter.ToString(bParseData, iDx, 1).Replace("-", ""); iDx += 1; break;
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
                        strRetunData = "CheckNAD_ServiceTypeA:Exception1";
                        return false;
                    }

                }
           
                strRetunData = "OK";
                return true;
            }
            catch
            {
                strRetunData = "CheckNAD_ServiceTypeA:Exception2";
                return false;
            }


        }

        private bool CheckGEN10_SimInfoType(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
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
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_NSPIF] = strSimver[1];
                                        STEPMANAGER_VALUE.OOBSimInfo[(int)SimInfoIndex.eSimVer_VVN] = strSimver[2];
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
                        strRetunData = "CheckGEN10_SimInfoType:Exception1";
                        return false;
                    }

                }

                return true;
            }
            catch
            {
                strRetunData = "CheckGEN10_SimInfoType:Exception2";
                return false;
            }


        }

        private bool CheckGEN10_ReverseType(string strSendCommand, byte[] bGetData, ref string strRetunData)
        {
            strRetunData = String.Empty;
            switch (strSendCommand)
            {
                case "READ_IMSI":

                    if (bGetData.Length > 3 && bGetData[0] == 0x01)
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

        private bool CheckGEN10_DtcIndex(byte bSendParam, byte[] bGetData, ref string strRetunData)
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
                strRetunData = "CheckGEN10_DtcIndex:Exception";
                return false;
            }

        }

        private bool CheckGen10_ALDLType(byte[] bSendPack, byte[] bGetData, ref string strRetunData, ref byte[] bReadBinary)
        {
            try
            {   ////////// 1. unsigned short len;          1byte
                ////////// 2. unsigned char  lflag;        4byte
                ////////// 3. unsigned char  data [150];  150byte

                if (bGetData.Length != 155) return false;

                string strData1 = String.Empty;
                string strData2 = String.Empty;
                string strData3 = String.Empty;                              

                byte[] bData1 = new byte[1];
                byte[] bDummy = new byte[4]; //문서에는 ignore 라고 되어 있음. 무시하자.
                byte[] bData3 = new byte[150];


                Array.Copy(bGetData, 0, bData1, 0, bData1.Length);
                Array.Copy(bGetData, 1, bDummy, 0, bDummy.Length);                           
                Array.Copy(bGetData, 5, bData3, 0, bData3.Length);
                bReadBinary = new byte[150];  //ALDL WRITE를 사용할때 위해서.
                Array.Copy(bData3, 0, bReadBinary, 0, bReadBinary.Length);

                int iDataLen = 0;
                try
                {
                    iDataLen = (int)bData1[0];
                }
                catch
                {
                    strRetunData = "CheckGen10_ALDLType:Exception - DATA TYPE ERROR";
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
                strRetunData = "CheckGEN10_ALDLType:Exception";
                return false;
            }

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

        public int AnalyzePacket(ref byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, int iAanlyizeOption)
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
                    //1. STX 찾기 // origine 4,5,6,7,8 비교하자.
                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_STX1 && tmpBytes[j + 1] == DEFRES_STX2)
                    {

                        if (tmpBytes.Length < (j + 16))
                            return (int)STATUS.RUNNING; //계속 수신중

                        //예외명령인지 한번 체크          
                        CheckExceptionCommands(strCommandName, ref strOrginSendData);

                        //GPS 특별명령인지 체크
                        bool bSpecialFlag = strCommandName.Equals("GEN10_OLD_GPS_INFO");

                        for (int p = 0; p < 5; p++)
                        {
                            if (strOrginSendData[4 + p].Equals(tmpBytes[j + 6 + p]))
                            {
                                bFind = true;
                            }
                            else
                            {
                                if (p == 3 && bSpecialFlag)
                                {
                                    //GEN10의 OLD GPS INFO 수집일경우는 7번째가 항상 다르기때문에 continue 한다.
                                }
                                else
                                {
                                    bFind = false;
                                    break;
                                }                                
                            }
                        }

                        if(!bFind) continue;

                        if (tmpBytes.Length > (j + 5) && tmpBytes.Length > ((int)tmpBytes[j + 5] + j + 6))
                        {
                            iFindStx = j; 
                            //iSize = (int)tmpBytes[j + 5] + 6;
                            iSize = (int)tmpBytes[j + 5] + ((int)tmpBytes[j + 4] * (int)0x100) + 6;

                            if (tmpBytes.Length <= (j + iSize - 1))
                            {
                                return (int)STATUS.RUNNING; //계속 수신중
                            }

                            //1. ETX 찾기
                            //if (bFind && tmpBytes[j] == DEFRES_ETX) 
                            if (bFind && tmpBytes[j + iSize - 1] == DEFRES_ETX)
                            {
                                byte[] tmpBuffer = new byte[iSize];

                                Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                                bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData, iAanlyizeOption);
                                strLogString = BitConverter.ToString(tmpBuffer).Replace("-", " ");
                                if (bChksum == (int)STATUS.OK)
                                {
                                    tmpBytes = new byte[tmpBuffer.Length];
                                    Array.Copy(tmpBuffer, 0, tmpBytes, 0, tmpBytes.Length);
                                    return (int)STATUS.OK;
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
            catch
            {
                return (int)STATUS.RUNNING;
            }
            
                
        }

        public int AnalyzeBytePacket(byte[] tmpBytes, ref string rtnData, AnalyizePack anlPack)
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

                bool bOK = false;
                byte[] bCompare = HexStringToBytes(anlPack.strAanlyizeString, ref bOK);

                bool bFind = false;
                for (int i = 0; i < tmpBytes.Length - bCompare.Length; i++)
                {
                    for (int j = 0; j < bCompare.Length; j++)
                    {
                        if (!tmpBytes[i].Equals(bCompare[j]))
                        {
                            bFind = false;
                            break;
                        }
                        if(j.Equals(bCompare.Length - 1))
                            bFind = true;
                    }                    
                }

                if (bFind)
                {
                    rtnData = anlPack.strAanlyizeString;
                    return (int)STATUS.OK;
                }

                if (tmpBytes.Length > 8096)
                {
                    return (int)STATUS.NG;
                }

            }
            catch
            {
                return (int)STATUS.RUNNING;
            }

            return (int)STATUS.RUNNING;
        }

        public int AnalyzeBinLogPacket(string tmpBytes, ref string rtnData, AnalyizePack anlPack)
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
                    rtnData = anlPack.strAanlyizeString;  
                    return (int)STATUS.OK;
                }

                if (tmpBytes.Length > 1024 * 100) //버퍼 10Kbytes
                {
                    return (int)STATUS.NG;
                }

            }
            catch
            {
                return (int)STATUS.RUNNING;
            }

            return (int)STATUS.RUNNING;
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
            catch
            {
                return (int)STATUS.RUNNING;
            }

            return (int)STATUS.RUNNING;
        }

        private string DeleteNoneAscii(string strOrigin)
        {
            strOrigin = strOrigin.Replace("\0", String.Empty); //널값 제거
            int i = strOrigin.Length;
            string pattern = "[^ -~]*";
            System.Text.RegularExpressions.Regex reg_exp = new System.Text.RegularExpressions.Regex(pattern);

            string strRemove = reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거
            strRemove = strRemove.Trim(); //공백 제거
            //return reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거

            return strRemove;
        }

        public int AnalyzeWLCommandPacket(string tmpBytes, ref string rtnData, ref string rtnFullData, string strOrginSendData, string strCompareA, string strCompareB)
        {
            try
            {                
                //1. STX 찾기 
                //[COMMON]|=> systemx
                //[COMMON]|MsgSend 
                tmpBytes = DeleteNoneAscii(tmpBytes);

                int iResponseIdx1 = tmpBytes.IndexOf(strWLresponse1);
                int iResponseIdx2 = tmpBytes.IndexOf(strWLresponse2);

                if (iResponseIdx1 < 0 && iResponseIdx2 < 0) return (int)STATUS.RUNNING;
                if (tmpBytes.Length < iResponseIdx1 + strOrginSendData.Length + 1 + strWLresponse1.Length) return (int)STATUS.RUNNING;
                if (tmpBytes.Length < iResponseIdx2 + strOrginSendData.Length + 1 + strWLresponse2.Length) return (int)STATUS.RUNNING;

                string strBuffer = String.Empty;

                try
                {
                    if (iResponseIdx1 >= 0) strBuffer = tmpBytes.Substring(iResponseIdx1, strWLresponse1.Length + strOrginSendData.Length + 1);
                    if (iResponseIdx2 >= 0) strBuffer = tmpBytes.Substring(iResponseIdx2, strWLresponse2.Length + strOrginSendData.Length + 1);
                
                }
                catch (System.Exception ex)
                {
                	string xxx = ex.Message;
                }

                if (strBuffer.Contains(strOrginSendData))
                   
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
                                return (int)STATUS.RUNNING;
                            }
                        }
                    }
                    if(iResponseIdx1 >= 0)
                    {
                        rtnFullData = tmpBytes.Substring(iResponseIdx1, strOrginSendData.Length + 1);
                    }

                    if (iResponseIdx2 >= 0)
                    {
                        rtnFullData = tmpBytes.Substring(iResponseIdx2, strOrginSendData.Length + 1);
                    }
                    rtnData = "OK";
                    return (int)STATUS.OK;
                }
                else
                {
                    return (int)STATUS.RUNNING;
                }
                                
            }
            catch {  }

            return (int)STATUS.RUNNING;
        }

        public byte[] ConvertVcpByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason, bool bCallClass) 
        {
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();
            bool bOk = true;
            tmpList.Clear();
            strReason = String.Empty;
            string strValue = String.Empty;
            byte[] rtnNull = new byte[1];  //FAIL 리턴용
            byte[] bParam;

            for (int i = 0; i < tmpString.Length; i++)
            {//데이터를 돌면서 체크섬의 위치를 찾는다
                
                if (tmpString[i].Contains("<DATA:"))
                {
                    tmpString[i] = tmpString[i].Replace("<DATA", String.Empty);
                    tmpString[i] = tmpString[i].Replace(":", String.Empty);
                    tmpString[i] = tmpString[i].Replace(">", String.Empty);
                 
                    if (String.IsNullOrEmpty(strParam))
                    {
                        tmpString[i] = "FF";
                        brtnOk = false;
                        tmpList.Add(tmpString[i]);
                        strReason = "PAR1 EMPTY";
                        return rtnNull;
                        
                    }
                    else
                    {      
                        try
                        {
                            int iEfileType = (int)EFILETYPE.CHECK;

                            switch (tmpString[i])
                            {
                                case "EFILE1": iEfileType = (int)EFILETYPE.CHECK; break;
                                case "EFILE2": iEfileType = (int)EFILETYPE.TRANSFER; break;

                                case "LFILE1": iEfileType = (int)EFILETYPE.CHECK; break;
                                case "LFILE2": iEfileType = (int)EFILETYPE.TRANSFER; break;

                            }

                            
                            switch (tmpString[i])
                            {
                                case "EFILE1":  //EFILE1,2  은 인증서업로드용이다.(For GEN10)
                                case "EFILE2":
                                case "LFILE1":  //LFILE1,2  은 로컬파일업로드용이다.(For GEN10)
                                case "LFILE2": 
                                            bOk = true;
                                            byte[] byteFile = new byte[1024];
                                            switch (tmpString[i])
                                            {
                                                case "EFILE1": 
                                                case "EFILE2":
                                                                byteFile = MakeEfileStruct(iEfileType, strParam, ref bOk, ref strReason);
                                                                break;
                                                case "LFILE1": 
                                                case "LFILE2":
                                                                byteFile = MakeLocalfileStruct(iEfileType, strParam, ref bOk, ref strReason, bCallClass);
                                                                break;

                                            }
                                            
                                            if (bOk)
                                            {
                                                for (int p = 0; p < byteFile.Length; p++)
                                                {
                                                    string tmpChar = String.Format("{0:X2}", byteFile[p]);
                                                    tmpList.Add(tmpChar);                                                  
                                                }
                                            }
                                            else
                                            {                                                                                            
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;

                                case "CHAR":
                                            byte[] byteChar = Encoding.UTF8.GetBytes(strParam);
                                            for (int iDx = 0; iDx < byteChar.Length; iDx++)
                                            {
                                                tmpList.Add(byteChar[iDx].ToString("X2"));
                                            }
                                            break;

                                case "BYTE":
                                            bOk = true;
                                            byte[] byteByte = HexStringToBytes(strParam, ref bOk);
                                            if (bOk)
                                            {
                                                for (int iDx = 0; iDx < byteByte.Length; iDx++)
                                                {
                                                    tmpList.Add(byteByte[iDx].ToString("X2"));
                                                }
                                            }
                                            else
                                            {
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;

                                case "ALDL_HEXA": //ALDL포멧으로 만들되 파라미터는 HEX형태로 만든다.
                                case "ALDL_ASCII"://ALDL포멧으로 만들되 파라미터는 아스키로 만든다.
                                case "ALDL_HEXB"://ALDL포멧으로 만들되 파라미터는 HEX형태로 만들되 특정바이트의 특정비트만 1로 변경할때.
                                case "ALDL_HEXC"://ALDL포멧으로 만들되 파라미터는 HEX형태로 만들되 특정바이트의 특정비트만 0로 변경할때.
                                            bOk = true;
                                            bParam = new byte[1];
                                            strValue = String.Empty;

                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA": brtnOk = true; break;
                                                case "ALDL_ASCII": brtnOk = true; break;
                                                case "ALDL_HEXB": brtnOk = MakeToggleAldl(true, strParam, ref strValue, ref strReason); break;
                                                case "ALDL_HEXC": brtnOk = MakeToggleAldl(false, strParam, ref strValue, ref strReason); break;
                                                default:      brtnOk = false; break;
                                            }
                                            
                                            if (!brtnOk) return rtnNull;                                            
                                            
                                            switch (tmpString[i])
                                            {
                                                case "ALDL_HEXA":  bParam = MakeGEN10_ALDLType(strParam, ref bOk, ref strReason, false); break;
                                                case "ALDL_ASCII": bParam = MakeGEN10_ALDLType(strParam, ref bOk, ref strReason, true); break;
                                                case "ALDL_HEXB":  bParam = MakeGEN10_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                case "ALDL_HEXC":  bParam = MakeGEN10_ALDLType(strValue, ref bOk, ref strReason, false); break;
                                                default:      bOk    = false; break;
                                            }

                                            if (bOk)
                                            {
                                                for (int p = 0; p < bParam.Length; p++)
                                                {
                                                    string tmpChar = String.Format("{0:X2}", bParam[p]);
                                                    tmpList.Add(tmpChar);
                                                }
                                            }
                                            else
                                            {
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;

                                default:
                                            strReason = "DATA TYPE ERROR1(" + tmpString[i] + ")";
                                            brtnOk = false;
                                            return rtnNull;
                                            
                            }
                                                    
                        }
                        catch
                        {
                            strReason = "DATA TYPE ERROR2(" + tmpString[i] + ")";
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
                            if (iLenSize > (int)0xFF)  //Big Endian
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
                        catch 
                        {
                            strReason = "DATA PARSE ERROR!";
                            brtnOk = false;
                            return rtnNull;
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
                        catch (Exception ex) 
                        {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":1:" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg); 
                            tmpArray[j] = 0xFF; 
                        }
                    }


                    DK_CHECKSUM DKCHK = new DK_CHECKSUM(); //체크섬!
                    byte bHighByte = 0x00;
                    byte bLowByte = 0x00;
                    DKCHK.Gen10_chksum(tmpArray, ref bHighByte, ref bLowByte, true);                    
                    tmpList.RemoveAt(i);
                    tmpList.Insert(i, bLowByte.ToString("X2"));   //BIG ENDIAN 
                    tmpList.Insert(i, bHighByte.ToString("X2"));   

                }

            }     

            byte[] rtnValue = new byte[tmpList.Count];

            for (int i = 0; i < tmpList.Count; i++)
            {
                try
                {
                    rtnValue[i] = Convert.ToByte(tmpList[i], 16);

                }
                catch 
                {
                    strReason = "DATA TYPE ERROR3(" + tmpString[i] + ")";
                    brtnOk = false;
                    return rtnNull;
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
        
        public bool IsFileCommand(string strPacket, string strCommandName, ref List<string> CmdList)
        {
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            string strFileName = String.Empty;
            
            strFileName = strCommandName + ".wlc";

            for (int i = 0; i < tmpString.Length; i++)
            {  //데이터를 돌면서 FILE의 위치를 찾으면 파일에 적혀있는 명령의 갯수를 리턴한다.
                if (tmpString[i].Equals("<FILE>"))
                {                    
                    bool bFile = tmpLogger.GetGen10WLCommandList(strFileName, ref CmdList);

                    if (!bFile) return false;
                    else return true;                  
                }
            }
            return false;
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

        private byte[] MakeGEN10_ALDLType(string strParam, ref bool bRes, ref string strReason, bool bAsciiType)
        {   //iPoint 는 써야되는 위치를 지정할때 쓴다.
            byte[] bReturnBytes = new byte[305];
            try
            {
                // 1. unsigned short block;         2 byte
                // 2. unsigned char  len;           1 byte
                // 3. unsigned char  data[150];   150 byte
                // 4. unsigned char  lflag[150];  150 byte

                byte[] bDataBlock = new byte[1];
                byte[] bDummy     = new byte[4];
                byte[] bDataData  = new byte[150];
                byte[] bDataFlag  = new byte[150];

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
             
                if (STEPMANAGER_VALUE.bOldBinaryALDL.Length != bDataData.Length )
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
                    Array.Copy(bTempBlock, 0, bDataBlock,     0, bTempBlock.Length);
                    if (iPoint > 0 || bEndPoint)
                    {
                        Array.Copy(STEPMANAGER_VALUE.bOldBinaryALDL, 0, bDataData, 0, STEPMANAGER_VALUE.bOldBinaryALDL.Length);
                        Array.Copy(bTempData, 0, bDataData, iPoint, bTempData.Length);                        
                    }
                    else
                    {
                        Array.Copy(bTempData, 0, bDataData, 0, bTempData.Length);
                    }
                    
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
                            if (i <= bTempData.Length + iPoint)
                            {
                                bDataData[j] = bTempData[i - iPoint];
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

                Array.Copy(bDataBlock, 0, bReturnBytes, 0, bDataBlock.Length);
                Array.Copy(bDummy, 0, bReturnBytes, 1, bDummy.Length);
                Array.Copy(bDataData, 0, bReturnBytes, 5, bDataData.Length);
                Array.Copy(bDataFlag, 0, bReturnBytes, 155, bDataFlag.Length);

                bRes = true;
                return bReturnBytes;
            }
            catch
            {
                strReason = "MakeGEN10_ALDLType:Exception";
                bRes = false;
                return bReturnBytes;
            }

        }

        private void CheckExceptionCommands(string strCommandName, ref byte[] strOrginSendData)
        {
            //프로토콜 거지같아서 이런걸 만듬.... 보낸명령어랑 응답 명령이 달라야하는 경우를 위해...
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

                default:
                    break;
            }

        }
                
        public byte[] MakeEfileStruct(int iEfileIdx, string strParam, ref bool bRes, ref string strReason)
        {
            byte[] bReturnBytes = new byte[10];
            byte[] bFileCount = new byte[4]; //현재는 멀티로 전송하는것이 구현이 안되었으므로 한번만 보냄.
            //byte[] bDataSize = new byte[2];
            byte[] bRealData = new byte[1024];
            //------------------

            string strRealData;
            int iFuncBytes = 10;
            //GEN10 STEP1 TYPE : (3IO8f%d %s %s) FileSize FilePath1, FileName 이므로 구조체가 아니고 char* 형태임... 헐.

            //[1] 파라미터 체크
            string[] strTempSpl = strParam.Split(',');
            if (strTempSpl.Length != 3)
            {
                strReason = "Par1 Error Comma(,) ";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }
            //[2] strTempSpl[0]: 파일경로   strTempSpl[1]: 파일이름   strTempSpl[2]: KIS로부터 받은 데이터
            if (strTempSpl[0].Length < 1 || strTempSpl[1].Length < 1 || strTempSpl[2].Length < 1)
            {
                strReason = "Par1 Error ";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            //[3] KIS 데이터를 끌고오기위해 여기서 할것이아니라 현재 함수를 호출하는 부분에서 KIS데이터를 파라미터로 전달처리해야한다.
            switch (strTempSpl[2])
            {
                case "KIS_error_code":      strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode]; break;
                case "KIS_error_message":   strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg]; break;
                case "KIS_stid":            strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID]; break;
                case "KIS_rCert":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT]; break;
                case "KIS_ccCert":          strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert]; break;
                case "KIS_vCert":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert]; break;
                case "KIS_vPri":            strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri]; break;
                case "KIS_vPre":            strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre]; break;
                case "KIS_vAuth":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth]; break;
                case "KIS_vHash":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash]; break;

                default:
                    strReason = "KIS Data Error (" + strTempSpl[2] + ") ";
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
            }

            /*
            //임시데이터 테스트
            switch (strTempSpl[2])
            {
                case "KIS_error_code":      strRealData = "0"; 
                                            break;
                case "KIS_error_message":   strRealData = ""; 
                                            break;
                case "KIS_stid":            strRealData  = "117605004"; 
                                            break;
                case "KIS_rCert":           strRealData  = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; 
                                            break;
                case "KIS_ccCert":          strRealData  = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; 
                                            break;
                case "KIS_vCert":           strRealData  = "-----BEGIN CERTIFICATE-----MIIBujCCAV+gAwIBAgIIBWTJeeT/AgEwCgYIKoZIzj0EAwIwMzEPMA0GA1UECwwGT05TVEFSMQ8wDQYDVQQKDAZPTlNUQVIxDzANBgNVBAMMBk9OU1RBUjAgFw0xNTExMTgxNTIyMDZaGA8yMTAwMTIzMTIzNTk1OVowKjEPMA0GA1UECgwGT25TdGFyMRcwFQYDVQQDDA5TVElEIDExNzYwNTAwNDBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABGxAbIISqlpQMn2+K4BBm6rgf2pt7bIMxTSUNXZQLtW3Cli6BaaA0f0kLabEmRdNp/RdE2G0RUzYuG5DkYjm9AWjZDBiMA4GA1UdDwEB/wQEAwIDiDAgBgNVHSUBAf8EFjAUBggrBgEFBQcDAgYIKwYBBQUHAwgwDwYDVR0TAQH/BAUwAwEBADAdBgNVHQ4EFgQULG2JCDeehwQr8SIMdH4TdU7weQYwCgYIKoZIzj0EAwIDSQAwRgIhAJ70eFf7zQveI+SNBh/OP6hL2y999y4okIRPA9TMtAlhAiEAvYuQccqnAQCc7/H2Bur0e3IB8WOati9sqycXjXFiJoo=-----END CERTIFICATE-----"; 
                                            break;
                case "KIS_vPri":            strRealData = "-----BEGIN PRIVATE KEY-----MEECAQAwEwYHKoZIzj0CAQYIKoZIzj0DAQcEJzAlAgEBBCDaiDof6CER8Soa9oV5aCDoNybflPiWLB25vNujvJZwkw==-----END PRIVATE KEY-----"; 
                                            break;
                case "KIS_vPre":            strRealData = "-----BEGIN PRE-SHARED KEY DATA-----l1kTC9OeZBSiex2GtLTQQHmw/vzcRLJNVaC17gUxQE9OCJPLJb91I6xk7/yx8oyFh2oB3PyLR61akbSAhMmLTw==-----END PRE-SHARED KEY DATA----------BEGIN PSK IDENTIFIER DATA-----117605004-----END PSK IDENTIFIER DATA----------BEGIN PSK HINT-----OnStar PoC-----END PSK HINT----------BEGIN PSK ID-----1-----END PSK ID-----"; 
                                            break;
                case "KIS_vAuth":           strRealData = "35823FFDD6EB5C53"; 
                                            break;
                case "KIS_vHash":           strRealData = "2d9d3bff4ef836732a618d85851df39753cb9ff9"; 
                                            break;
                default:
                                            strReason = "KIS Data Type Error : " + strTempSpl[2];
                                            bReturnBytes = new byte[iFuncBytes];
                                            bRes = false;
                                            return bReturnBytes;
            }           
            */

            //[4] DATA FILE CHECK
            if (String.IsNullOrEmpty(strRealData))
            {
                strReason = strTempSpl[2] + "(Empty)";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            //[5] MAKE FILE
            bool bMakeSuccess = true;
            bRealData = Encoding.UTF8.GetBytes(strRealData);
            if (bRealData.Length >= 1024)
            {
                bMakeSuccess = false;
                strReason = "FileSize OverFlow(" + bRealData.Length.ToString() + " Bytes) ";

            }
            else
            {
                if (bRealData.Length < 1)
                {
                    bMakeSuccess = false;
                    strReason = "DataFile Missing(0 Bytes)";
                }
            }

            if (!bMakeSuccess)
            {
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            //[4] MAKE BYTES by COMMAND TYPE
            int iByteIndex = 0;
            switch (iEfileIdx)
            {
                case (int)EFILETYPE.CHECK:
                    byte[] bAscFileSize = Encoding.UTF8.GetBytes(strRealData.Length.ToString() + " "); //파라미터간의 Space 바이트 
                    byte[] bAscFilePath = Encoding.UTF8.GetBytes(strTempSpl[0].ToString() + " ");      //파라미터간의 Space 바이트 
                    byte[] bAscFileName = Encoding.UTF8.GetBytes(strTempSpl[1].ToString());
                    iFuncBytes = bAscFileSize.Length + bAscFilePath.Length + bAscFileName.Length;
                    bReturnBytes = new byte[iFuncBytes];

                    Array.Copy(bAscFileSize, 0, bReturnBytes, iByteIndex, bAscFileSize.Length); iByteIndex = bAscFileSize.Length;
                    Array.Copy(bAscFilePath, 0, bReturnBytes, iByteIndex, bAscFilePath.Length); iByteIndex += bAscFilePath.Length;
                    Array.Copy(bAscFileName, 0, bReturnBytes, iByteIndex, bAscFileName.Length);
                    bRes = true;
                    break;

                case (int)EFILETYPE.TRANSFER:

                    iFuncBytes = bFileCount.Length + bRealData.Length;
                    bReturnBytes = new byte[iFuncBytes];

                    Array.Copy(bFileCount, 0, bReturnBytes, iByteIndex, bFileCount.Length); iByteIndex = bFileCount.Length;
                    Array.Copy(bRealData, 0, bReturnBytes, iByteIndex, bRealData.Length);
                    bRes = true;

                    break;

                default: strReason = "EFILETYPE Error(" + iEfileIdx.ToString() + ")";
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
            }

            return bReturnBytes;
        }

        public byte[] MakeLocalfileStruct(int iEfileIdx, string strParam, ref bool bRes, ref string strReason, bool bCallClass)
        {
            byte[] bReturnBytes = new byte[10];
            byte[] bFileCount = new byte[4]; //현재는 멀티로 전송하는것이 구현이 안되었으므로 한번만 보냄.            
            byte[] bRealData = new byte[1024];
            //------------------

            int iFuncBytes = 10;
            //GEN10 STEP1 TYPE : (3IO8f%d %s %s) FileSize FilePath1, FileName 이므로 구조체가 아니고 char* 형태임... 헐.

            //[1] 파라미터 체크
            string[] strTempSpl = strParam.Split(',');
            if (strTempSpl.Length != 3)
            {
                strReason = "Par1 Error Comma(,) ";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }
            //[2] strTempSpl[0]: 파일경로   strTempSpl[1]: 세트내부 저장될 파일 이름   strTempSpl[2]: PC 에서 보낼 파일이름
            if (strTempSpl[0].Length < 1 || strTempSpl[1].Length < 1 || strTempSpl[2].Length < 1)
            {
                strReason = "Par1 Error ";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;
            }

            string strProgramPath = AppDomain.CurrentDomain.BaseDirectory + "GEN10\\" + strTempSpl[2];
            int iLocalFizeSize = 0;
            //[3] DATA FILE CHECK     
            switch (iEfileIdx)
            {
                case (int)EFILETYPE.CHECK:
                    STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountLength     = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize  = 0;
                    STEPMANAGER_VALUE.iUploadBytesSendCount = -1;
                    iLocalFizeSize = LocalFileExist1(strProgramPath, ref strReason);
                    if (iLocalFizeSize < 1)
                    {
                        strReason = "LocalFile Error(0 Bytes)";
                        bReturnBytes = new byte[iFuncBytes];
                        bRes = false;
                        return bReturnBytes;
                    }
                    break;
                case (int)EFILETYPE.TRANSFER:
                    if (!LocalFileExist2(strProgramPath, ref bRealData, ref strReason, bCallClass)) //로컬파일 데이터를 끌어와 바이너리로 변경한다. 
                    {
                        bReturnBytes = new byte[iFuncBytes];
                        bRes = false;
                        return bReturnBytes;
                    }  
                    break;
                default: 
                    strReason = "EFILETYPE Error(" + iEfileIdx.ToString() + ")";
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;

            }           

            //[4] MAKE BYTES by COMMAND TYPE
            int iByteIndex = 0;
            switch (iEfileIdx)
            {
                case (int)EFILETYPE.CHECK:
                    
                    byte[] bAscFileSize = Encoding.UTF8.GetBytes(iLocalFizeSize.ToString() + " "); //파라미터간의 Space 바이트 
                    byte[] bAscFilePath = Encoding.UTF8.GetBytes(strTempSpl[0].ToString() + " ");      //파라미터간의 Space 바이트 
                    byte[] bAscFileName = Encoding.UTF8.GetBytes(strTempSpl[1].ToString());
                    iFuncBytes = bAscFileSize.Length + bAscFilePath.Length + bAscFileName.Length;
                    bReturnBytes = new byte[iFuncBytes];

                    Array.Copy(bAscFileSize, 0, bReturnBytes, iByteIndex, bAscFileSize.Length); iByteIndex = bAscFileSize.Length;
                    Array.Copy(bAscFilePath, 0, bReturnBytes, iByteIndex, bAscFilePath.Length); iByteIndex += bAscFilePath.Length;
                    Array.Copy(bAscFileName, 0, bReturnBytes, iByteIndex, bAscFileName.Length);
                    bRes = true;
                    break;

                case (int)EFILETYPE.TRANSFER:

                    iFuncBytes = bFileCount.Length + bRealData.Length;
                    bReturnBytes = new byte[iFuncBytes];
                    bFileCount = BitConverter.GetBytes(STEPMANAGER_VALUE.iUploadBytesSendCount);
                    Array.Copy(bFileCount, 0, bReturnBytes, iByteIndex, bFileCount.Length); iByteIndex = bFileCount.Length;
                    Array.Copy(bRealData, 0, bReturnBytes, iByteIndex, bRealData.Length);
                    bRes = true;

                    break;

                default: strReason = "LFILETYPE Error(" + iEfileIdx.ToString() + ")";
                    bReturnBytes = new byte[iFuncBytes];
                    bRes = false;
                    return bReturnBytes;
            }

            return bReturnBytes;
        }

        private int LocalFileExist1(string strFilePath, ref string strReason)
        {            
            int iSize = 0;
            if (!System.IO.File.Exists(strFilePath))
            {   //파일이 없으면 
                strReason = "NOT FOUND FILE";
                return 0;
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

                    iSize = (int)BR.BaseStream.Length; //총사이즈 저장.
                    
                    strReason = "SUCCESS";
                    

                }
                catch (Exception e)
                {
                    strReason = "FILE EXCEPTION.";
                    string messs = String.Empty;
                    messs = e.Message;
                    iSize = 0;
                }
                finally
                {
                    if (BR != null) BR.Close();
                    if (FS != null) FS.Close();
                }

                return iSize;
            }

        }

        private bool LocalFileExist2(string strFilePath, ref byte[] bBinaryFile, ref string strReason, bool bCallClass)
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

                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize = (int)BR.BaseStream.Length; //총사이즈 저장.

                    if (STEPMANAGER_VALUE.iUploadBytesCountStartIndex >= STEPMANAGER_VALUE.iUploadBytesCountTotalSize)
                    {
                        strReason = "Already Transfer Complete";
                        string messs = String.Empty;
                        bSuccess = true;
                    }
                    else
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

                        bBinaryFile = new byte[STEPMANAGER_VALUE.iUploadBytesCountLength];
                        BR.BaseStream.Seek(STEPMANAGER_VALUE.iUploadBytesCountStartIndex, System.IO.SeekOrigin.Begin);
                        int i = BR.Read(bBinaryFile, 0, STEPMANAGER_VALUE.iUploadBytesCountLength);


                        if (bCallClass) //TX로 보낼때만 증가시켜야한다... RX때도 함수를 호출하기 때문에 이놈이 증가되버린다....
                        {
                            STEPMANAGER_VALUE.iUploadBytesCountStartIndex += STEPMANAGER_VALUE.iUploadBytesCountLength;
                            STEPMANAGER_VALUE.iUploadBytesSendCount++;
                        }

                        bSuccess = true;
                        strReason = "SUCCESS";
                    }

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

    }

}
