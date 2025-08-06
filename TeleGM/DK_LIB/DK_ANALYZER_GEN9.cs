using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GmTelematics
{
    enum GEN9_GPS_IDX
    {
        dummy,
        utc_year, utc_month, utc_day, utc_hour, utc_min, utc_sec,
        lat, lon, elevation, sog, cog, nav_mode, hdop, pdop,
        sv_used_cnt, nav_valid, gps_week, gps_tow, used_SV, ehpe
    }
    public struct GEN9_SV
    {
        public byte[] dummy;
        public byte[] sv_id;
        public byte[] sv_cno;
        public byte[] sv_pseudorange;
        public byte[] sv_azimuth;
        public byte[] sv_elevation;
        public byte[] sv_state;
    }
    /*
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GEN9_GPS_REPORTS
    {

        public uint	dummy;
	    public ushort	utc_year;
	    public byte	utc_month;
	    public byte utc_day;
	    public byte utc_hour;
        public byte utc_min;
	    public float			utc_sec;
	    public double			lat;
	    public double			lon;
	    public int				elevation;
	    public float			sog;
	    public float			cog;
	    public ushort	nav_mode;
	    public float			hdop;
	    public float			pdop;
	    public byte	sv_used_cnt;
	    public ushort	nav_valid;
	    public ushort	gps_week;
	    public uint	gps_tow;
	    public ulong	used_SV;
	    public uint	ehpe;
    }
    */
    
    public struct GEN9_GPS_REPORTS
    {
        public byte[] dummy;
        
        public byte[] utc_year;
        public byte[] utc_month;
        public byte[] utc_day;
        public byte[] utc_hour;
        public byte[] utc_min;
        public byte[] utc_sec;
        public byte[] lat;
        public byte[] lon;
        public byte[] elevation;
            
        public byte[] sog;
        public byte[] cog;
        public byte[] nav_mode;
        public byte[] hdop;
        public byte[] pdop;
        public byte[] sv_used_cnt;
        public byte[] nav_valid;
        public byte[] gps_week;
        public byte[] gps_tow;
        public byte[] used_SV;
        public byte[] ehpe;

    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _stMEIDpESND
    {
        public uint dummy;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] m_apEsn;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] m_aMeid;

        public bool m_bMEIDSet;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _stMEIDpESND_GSM
    {
        public uint dummy;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] m_apEsn;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] m_aMeid;

        public bool m_bMEIDSet;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] m_aIMEI;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] m_aICCID;
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _SIDNIDPairsD
    {
        public ushort m_sSID;
        public ushort m_sNID;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _NAMMgmnt
    {
        public ushort m_sHomeSID;
        //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
        //public byte[] m_sHomeSID;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] m_cpMIN;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] m_cpMDN;

        //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
        public int m_iPairsCnt;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11)]
        public _SIDNIDPairsD[] m_stSIDNIDPairs;

        public short m_sResCode;
        public byte m_bActionCode;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _stNAID
    {
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy;

        public short m_sResCode;
        public short m_sActionCode;
        public short m_sFormat;
        public short m_sPortReqType;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_cpPassword;

        public byte m_sNAISize1; //dummy
        public byte m_sNAISize2; // 이걸로 사용. 원래 두바이트 인데 그냥 바이트오더가 달라서 이렇게 대응
        // 그러니까 씨에서 찍으면 이렇게 나오고 :00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 6F 6E 73 74 61 72 2E 67 73 6D 2E 67 6C 6F 62 61 6C 2E 63 6F 6D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 9A 00 00 00
        // 씨샵에서 마샬링 하며는 이렇게 나온다 :00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 00 6F 6E 73 74 61 72 2E 67 73 6D 2E 67 6C 6F 62 61 6C 2E 63 6F 6D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 9A 00 00 00
        // 세트에서 리드를 해보면 이렇게 나온다 :48 91 26 18 00 00 00 00 00 00 00 00 6F 6E 73 74 61 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 6F 6E 73 74 61 72 2E 67 73 6D 2E 67 6C 6F 62 61 6C 2E 63 6F 6D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 9A 00 00 
       

	    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] m_cpNAI;
        	
        public ushort m_usPort;
        public ushort m_sMsgID;
        public short m_sPortStatus;
    }
    
    class DK_ANALYZER_GEN9
    {        
        //GEN9.7미만
        private const byte DEFRES_STX1L = 0x02;
        private const byte DEFRES_STX2L = 0xFA;
        private const byte DEFRES_ETXL  = 0xFA;

        //GEN9.7이상
        private const byte DEFRES_STX1H = 0x02;
        private const byte DEFRES_STX2H = 0xFB;
        private const byte DEFRES_ETXH = 0xFA;

        //GEN9 Factory
        private const byte DEFRES_FSTX = 0x7E;        
        private const byte DEFRES_FETX = 0xAA;
        private const byte DEFRES_FRES = 0x10;

        private const string HEX_CHARS = "0123456789ABCDEF";

        private const string strWLresponse1 = "[COMMON]|=> systemx";
        private const string strWLresponse2 = "[COMMON]|MsgSend ";
           
        public int AnalyzeMode = (int)CLSMODE.NORMAL;

        public DK_ANALYZER_GEN9()
        {            
            
        }

        public int CheckProtocol(byte[] tmpByteArray, byte[] strOrginSendData, string strCommandName, ref string rtnData, int iAanlyizeOption, string strParam)
        {            
            //체크섬 검사.

            DK_CHECKSUM DKCHK = new DK_CHECKSUM();
            byte bChkHigh = 0x00;
            byte bChkLow  = 0x00;
            byte chkByte  = 0x00;
            int iPos = 0;

            switch (AnalyzeMode)
            {
                case (int)CLSMODE.FACTORY:

                    byte[] bChkArray = new byte[tmpByteArray.Length - 2];

                    Array.Copy(tmpByteArray, 0, bChkArray, 0, bChkArray.Length);
                    DKCHK.Gen9_CX(bChkArray, ref chkByte);

                    if (tmpByteArray[tmpByteArray.Length - 2] != chkByte)
                    {
                        return (int)STATUS.NG;
                    }
                    break;

                case (int)CLSMODE.NORMAL:

                    DKCHK.Gen9_chksum(tmpByteArray, ref bChkHigh, ref bChkLow, false);

                    if (tmpByteArray[tmpByteArray.Length - 3] != bChkHigh ||
                        tmpByteArray[tmpByteArray.Length - 2] != bChkLow)
                    {
                        return (int)STATUS.NG;
                    }
                    break;

                case (int)CLSMODE.FCP:
                    DKCHK.Gen9_chksum(tmpByteArray, ref bChkHigh, ref bChkLow, false);

                    if (tmpByteArray[tmpByteArray.Length - 3] != bChkHigh ||
                        tmpByteArray[tmpByteArray.Length - 2] != bChkLow)
                    {
                        return (int)STATUS.NG;
                    }
                    break;

                case (int)CLSMODE.GEN9HIGH:

                    DKCHK.Gen10_chksum(tmpByteArray, ref bChkHigh, ref bChkLow, false);

                    if (tmpByteArray[tmpByteArray.Length - 3] != bChkHigh ||
                        tmpByteArray[tmpByteArray.Length - 2] != bChkLow)
                    {
                        return (int)STATUS.NG;
                    }
                    break;

                default: return (int)STATUS.ERROR;

            }        

            //GPS 특별명령.
            bool bSpecialFlag = strCommandName.Equals("GEN9_OLD_GPS_INFO");
            byte[] bData = new byte[1];
            int iDataLen = 0;            

            try
            {
                switch (AnalyzeMode)
                {
                    case (int)CLSMODE.FACTORY:

                        //데이터 수집 구간 시작------------------------------------------                    
                        iDataLen = (int)tmpByteArray[1] - 2;

                        if (iDataLen > 0)
                        {
                            bData = new byte[iDataLen];
                            Array.Copy(tmpByteArray, 4, bData, 0, bData.Length);
                        }
                        else
                        {
                            rtnData = "";
                            return (int)STATUS.OK;
                        }
                        break;

                    case (int)CLSMODE.NORMAL:
                    case (int)CLSMODE.FCP:

                        //보낸명령어에 대한 응답데이터인지 확인1
                        for (int i = 4; i < 9; i++)
                        {
                            if (i == 7 && bSpecialFlag)
                            {
                                continue;//GEN9의 OLD GPS INFO 수집일경우는 7번째가 항상 다르기때문에 continue 한다.
                            }

                            if (tmpByteArray[i] != strOrginSendData[i])
                            {
                                return (int)STATUS.RUNNING;
                            }
                        }
                        //데이터 수집 구간 시작------------------------------------------
                        //int iDataLen = (int)tmpByteArray[5];
                        iDataLen = (int)tmpByteArray[3] + ((int)tmpByteArray[2] * (int)0x100);
                        bData = new byte[iDataLen - 8];

                        for (int i = 0; i < bData.Length; i++)
                        {
                            //if (bSpecialFlag)
                            //    bData[i] = tmpByteArray[i + 10];
                            //else
                            bData[i] = tmpByteArray[i + 9];
                        }
                        break;

                    case (int)CLSMODE.GEN9HIGH:

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
                        iDataLen = (int)tmpByteArray[5] + ((int)tmpByteArray[4] * (int)0x100);
                        bData = new byte[iDataLen - 8];

                        for (int i = 0; i < iDataLen - 8; i++)
                        {
                            /*if (bSpecialFlag)
                                bData[i] = tmpByteArray[i + 12];
                            else*/
                                bData[i] = tmpByteArray[i + 11];
                        }
                        break;

                    default: return (int)STATUS.ERROR;

                }
                
                switch (iAanlyizeOption)
                {
                    case (int)ANALYIZEGEN9.GPSINFO1:
                    case (int)ANALYIZEGEN9.GPSINFO2:
                    case (int)ANALYIZEGEN9.GPSINFO3:
                    case (int)ANALYIZEGEN9.GPSINFO4:

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
                            case (int)ANALYIZEGEN9.GPSINFO1: rtnData = bData[3].ToString(); break; //gps svcount
                            case (int)ANALYIZEGEN9.GPSINFO2: rtnData = bData[4].ToString(); break; //gps cn0 Max
                            case (int)ANALYIZEGEN9.GPSINFO3: rtnData = bData[7].ToString(); break; //gnss svcount
                            case (int)ANALYIZEGEN9.GPSINFO4: rtnData = bData[8].ToString(); break; //gnss cn0 Max
                            default: rtnData = "FF"; break;
                        }

                        break;
                    case (int)ANALYIZEGEN9.OLDGPSINFO:

                        bool bGpsHighModel = AnalyzeMode.Equals((int)CLSMODE.GEN9HIGH);
                        STEPMANAGER_VALUE.SetOldGEN9GPSInfo(bData, bGpsHighModel);
                        //STEPMANAGER_VALUE.SetOldGEN9GPSInfo(tmpByteArray);
                        break;

                    case (int)ANALYIZEGEN9.TTFF:

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

                    case (int)ANALYIZEGEN9.DTC:
                        if (strOrginSendData.Length < 12) return (int)STATUS.NG;   
                        if (!CheckGEN9_DtcIndex(strOrginSendData[10], bData, ref rtnData))
                        {                                                      
                            return (int)STATUS.NG;   
                        }                     
                        break;

                    case (int)ANALYIZEGEN9.DTCOQA:
                        rtnData = Encoding.UTF8.GetString(bData);
                        rtnData = rtnData.Trim(); //공백제거.
                        rtnData = rtnData.Replace("-", String.Empty);
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;

                    case (int)ANALYIZEGEN9.OOBRESULT:
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
                    case (int)ANALYIZEGEN9.REVERSE:
                        if (!CheckGEN9_ReverseType(strCommandName, bData, ref rtnData))
                        {
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.SIMINFO:
                        if (!CheckGEN9_SimInfoType(strOrginSendData, bData, ref rtnData))
                        {
                            return (int)STATUS.NG;
                        }                                                    
                        break;

                    case (int)ANALYIZEGEN9.BYTE:
                        for (int i = 0; i < bData.Length; i++)
                        {
                            rtnData += bData[i].ToString("X2");
                        } 
                        break;

                    case (int)ANALYIZEGEN9.CHECKSUM:
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

                    case (int)ANALYIZEGEN9.RESCODE:                        
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

                    case (int)ANALYIZEGEN9.CONFCODE:
                        try
                        {
                            switch (bData[0])
                            {
                                case 0x31:
                                    rtnData = Encoding.UTF8.GetString(bData, 1, 5);
                                    rtnData = rtnData.Replace("\0", String.Empty);
                                    rtnData = rtnData.Replace("?", String.Empty);
                                    rtnData = rtnData.Trim();
                                    break;

                                case 0x32:
                                    rtnData = Encoding.UTF8.GetString(bData, 1, 4);
                                    rtnData = rtnData.Replace("\0", String.Empty);
                                    rtnData = rtnData.Replace("?", String.Empty);
                                    rtnData = rtnData.Trim();
                                    break;
                                default:
                                    rtnData = "PACKET ERROR.";
                                    return (int)STATUS.NG;
                            }
                        }
                        catch
                        {
                            rtnData = "PACKET ERROR.";
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.BTMAC:
                        
                        try
                        {
                            if (bData.Length != 12)
                            {
                                rtnData = "ADDRESS SIZE ERROR.";
                                return (int)STATUS.NG;
                            }

                            rtnData = bData[10].ToString("X2") + bData[11].ToString("X2") +
                                       bData[8].ToString("X2") +
                                       bData[5].ToString("X2") + bData[6].ToString("X2") + bData[7].ToString("X2");
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

                    case (int)ANALYIZEGEN9.PLATFORM1:
                    case (int)ANALYIZEGEN9.PLATFORM2:
                    case (int)ANALYIZEGEN9.PLATFORM3:

                        iPos = 0;
                        switch (iAanlyizeOption)
                        {
                            case (int)ANALYIZEGEN9.PLATFORM1: 
                            case (int)ANALYIZEGEN9.PLATFORM3: iPos = 0;     //by name
                                                              break;
                            case (int)ANALYIZEGEN9.PLATFORM2: iPos = 2; break;                            
                            default: break;
                        }

                        try
                        {
                            if (bData.Length < iPos + 1)
                            {
                                rtnData = "PLATFORM SIZE ERROR.";
                                return (int)STATUS.NG;
                            }

                            rtnData = Encoding.UTF8.GetString(bData, iPos, 1);
                            rtnData = rtnData.Replace("\0", String.Empty);
                            rtnData = rtnData.Replace("?", String.Empty);
                            rtnData = rtnData.Trim();
                            
                            if (iAanlyizeOption.Equals((int)ANALYIZEGEN9.PLATFORM3))
                            {
                                int iCanType = -1;

                                try
                                {
                                    iCanType = int.Parse(rtnData);
                                    rtnData = (GEN9NAME.GMLAN29 + iCanType).ToString();
                                }
                                catch
                                {
                                    rtnData = "TRANSLATE NAME ERROR.";
                                    return (int)STATUS.NG;
                                }
                                
                            }

                        }
                        catch
                        {
                            rtnData = "PACKET ERROR.";
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.CRLF1:
                    case (int)ANALYIZEGEN9.CRLF2:
                    case (int)ANALYIZEGEN9.CRLF3:
                    case (int)ANALYIZEGEN9.CRLF4:
                    case (int)ANALYIZEGEN9.CRLF5:
                        iPos = 0;
                        switch (iAanlyizeOption)
                        {
                            case (int)ANALYIZEGEN9.CRLF1: iPos = 0; break;
                            case (int)ANALYIZEGEN9.CRLF2: iPos = 1; break;
                            case (int)ANALYIZEGEN9.CRLF3: iPos = 2; break;
                            case (int)ANALYIZEGEN9.CRLF4: iPos = 3; break;
                            case (int)ANALYIZEGEN9.CRLF5: iPos = 4; break;
                            default: break;
                        }
                        for (int p = 0; p < bData.Length; p++)
                        {
                            if (bData[p].Equals(0x0D) || bData[p].Equals(0x0A))
                            {
                                bData[p] = 0x2F;
                            }
                        }

                        string strRes = Encoding.UTF8.GetString(bData);
                        string[] strResList = System.Text.RegularExpressions.Regex.Split(strRes, "//");
                        string strResult = String.Empty;
                        if (strResList.Length.Equals(1))
                        {
                            strResult = strRes;
                        }
                        else
                        {
                            strResult = strResList[iPos];
                        }
                        rtnData = strResult;                        
                        rtnData = rtnData.Trim(); //공백제거.
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;

                    case (int)ANALYIZEGEN9.DTC00: case (int)ANALYIZEGEN9.DTC01: case (int)ANALYIZEGEN9.DTC02: case (int)ANALYIZEGEN9.DTC03: case (int)ANALYIZEGEN9.DTC04:
                    case (int)ANALYIZEGEN9.DTC05: case (int)ANALYIZEGEN9.DTC06: case (int)ANALYIZEGEN9.DTC07: case (int)ANALYIZEGEN9.DTC08: case (int)ANALYIZEGEN9.DTC09:
                    case (int)ANALYIZEGEN9.DTC10: case (int)ANALYIZEGEN9.DTC11: case (int)ANALYIZEGEN9.DTC12: case (int)ANALYIZEGEN9.DTC13: case (int)ANALYIZEGEN9.DTC14:
                    case (int)ANALYIZEGEN9.DTC15: case (int)ANALYIZEGEN9.DTC16: case (int)ANALYIZEGEN9.DTC17: case (int)ANALYIZEGEN9.DTC18: case (int)ANALYIZEGEN9.DTC19:
                    case (int)ANALYIZEGEN9.DTC20: case (int)ANALYIZEGEN9.DTC21: case (int)ANALYIZEGEN9.DTC22: case (int)ANALYIZEGEN9.DTC23: case (int)ANALYIZEGEN9.DTC24:
                    case (int)ANALYIZEGEN9.DTC25: case (int)ANALYIZEGEN9.DTC26: case (int)ANALYIZEGEN9.DTC27: case (int)ANALYIZEGEN9.DTC28:

                    case (int)ANALYIZEGEN9.DTC00_BITS: case (int)ANALYIZEGEN9.DTC01_BITS: case (int)ANALYIZEGEN9.DTC02_BITS: case (int)ANALYIZEGEN9.DTC03_BITS:
                    case (int)ANALYIZEGEN9.DTC04_BITS: case (int)ANALYIZEGEN9.DTC05_BITS: case (int)ANALYIZEGEN9.DTC06_BITS: case (int)ANALYIZEGEN9.DTC07_BITS:
                    case (int)ANALYIZEGEN9.DTC08_BITS: case (int)ANALYIZEGEN9.DTC09_BITS: case (int)ANALYIZEGEN9.DTC10_BITS: case (int)ANALYIZEGEN9.DTC11_BITS:
                    case (int)ANALYIZEGEN9.DTC12_BITS: case (int)ANALYIZEGEN9.DTC13_BITS: case (int)ANALYIZEGEN9.DTC14_BITS: case (int)ANALYIZEGEN9.DTC15_BITS:
                    case (int)ANALYIZEGEN9.DTC16_BITS: case (int)ANALYIZEGEN9.DTC17_BITS: case (int)ANALYIZEGEN9.DTC18_BITS: case (int)ANALYIZEGEN9.DTC19_BITS:
                    case (int)ANALYIZEGEN9.DTC20_BITS: case (int)ANALYIZEGEN9.DTC21_BITS: case (int)ANALYIZEGEN9.DTC22_BITS: case (int)ANALYIZEGEN9.DTC23_BITS:
                    case (int)ANALYIZEGEN9.DTC24_BITS:
                    case (int)ANALYIZEGEN9.DTC25_BITS: case (int)ANALYIZEGEN9.DTC26_BITS: case (int)ANALYIZEGEN9.DTC27_BITS: case (int)ANALYIZEGEN9.DTC28_BITS:

                        iPos = 0;
                        bool bExpBitArray = false;
                        switch (iAanlyizeOption)
                        {
                            case (int)ANALYIZEGEN9.DTC00: iPos = 0; break;
                            case (int)ANALYIZEGEN9.DTC01: iPos = 1; break;
                            case (int)ANALYIZEGEN9.DTC02: iPos = 2; break;
                            case (int)ANALYIZEGEN9.DTC03: iPos = 3; break;
                            case (int)ANALYIZEGEN9.DTC04: iPos = 4; break;
                            case (int)ANALYIZEGEN9.DTC05: iPos = 5; break;
                            case (int)ANALYIZEGEN9.DTC06: iPos = 6; break;
                            case (int)ANALYIZEGEN9.DTC07: iPos = 7; break;
                            case (int)ANALYIZEGEN9.DTC08: iPos = 8; break;
                            case (int)ANALYIZEGEN9.DTC09: iPos = 9; break;
                            case (int)ANALYIZEGEN9.DTC10: iPos = 10; break;
                            case (int)ANALYIZEGEN9.DTC11: iPos = 11; break;
                            case (int)ANALYIZEGEN9.DTC12: iPos = 12; break;
                            case (int)ANALYIZEGEN9.DTC13: iPos = 13; break;
                            case (int)ANALYIZEGEN9.DTC14: iPos = 14; break;
                            case (int)ANALYIZEGEN9.DTC15: iPos = 15; break;
                            case (int)ANALYIZEGEN9.DTC16: iPos = 16; break;
                            case (int)ANALYIZEGEN9.DTC17: iPos = 17; break;
                            case (int)ANALYIZEGEN9.DTC18: iPos = 18; break;
                            case (int)ANALYIZEGEN9.DTC19: iPos = 19; break;
                            case (int)ANALYIZEGEN9.DTC20: iPos = 20; break;
                            case (int)ANALYIZEGEN9.DTC21: iPos = 21; break;
                            case (int)ANALYIZEGEN9.DTC22: iPos = 22; break;
                            case (int)ANALYIZEGEN9.DTC23: iPos = 23; break;
                            case (int)ANALYIZEGEN9.DTC24: iPos = 24; break;
                            case (int)ANALYIZEGEN9.DTC25: iPos = 25; break;
                            case (int)ANALYIZEGEN9.DTC26: iPos = 26; break;
                            case (int)ANALYIZEGEN9.DTC27: iPos = 27; break;
                            case (int)ANALYIZEGEN9.DTC28: iPos = 28; break;

                            case (int)ANALYIZEGEN9.DTC00_BITS: iPos = 0; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC01_BITS: iPos = 1; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC02_BITS: iPos = 2; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC03_BITS: iPos = 3; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC04_BITS: iPos = 4; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC05_BITS: iPos = 5; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC06_BITS: iPos = 6; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC07_BITS: iPos = 7; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC08_BITS: iPos = 8; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC09_BITS: iPos = 9; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC10_BITS: iPos = 10; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC11_BITS: iPos = 11; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC12_BITS: iPos = 12; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC13_BITS: iPos = 13; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC14_BITS: iPos = 14; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC15_BITS: iPos = 15; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC16_BITS: iPos = 16; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC17_BITS: iPos = 17; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC18_BITS: iPos = 18; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC19_BITS: iPos = 19; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC20_BITS: iPos = 20; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC21_BITS: iPos = 21; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC22_BITS: iPos = 22; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC23_BITS: iPos = 23; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC24_BITS: iPos = 24; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC25_BITS: iPos = 25; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC26_BITS: iPos = 26; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC27_BITS: iPos = 27; bExpBitArray = true; break;
                            case (int)ANALYIZEGEN9.DTC28_BITS: iPos = 28; bExpBitArray = true; break;
                            default: break;
                        }

                        
                        string strResDtc = Encoding.UTF8.GetString(bData);
                        string[] strResListDtc = System.Text.RegularExpressions.Regex.Split(strResDtc, "-");
                        string strResultDtc = String.Empty;
                        if (strResListDtc.Length.Equals(1))
                        {
                            strResultDtc = strResDtc;
                        }
                        else
                        {
                            if (iPos >= strResListDtc.Length)
                            {
                                rtnData = "DTC Index RangeOver : ResponseCount(" + strResListDtc.Length.ToString() + ") / Inspection Command Index(" + iPos.ToString() + ")";
                                return (int)STATUS.NG;
                            }
                            strResultDtc = strResListDtc[iPos];
                        }

                        if (!bExpBitArray)
                        {
                            rtnData = strResultDtc;
                            rtnData = rtnData.Trim(); //공백제거.
                            rtnData = rtnData.Replace("\0", String.Empty);
                        }
                        else
                        {
                            //8비트로 나타내는 방법 (DQA 담당자 요청방식)
                            bool bConverOK = false;
                            byte[] bDtcByte = HexStringToBytes(strResultDtc, ref bConverOK);
                            if (bConverOK)
                            {
                                rtnData = Convert.ToString(bDtcByte[0], 2).PadLeft(8, '0');
                            }
                            else
                            {
                                rtnData = "Convert Fail.";
                                return (int)STATUS.NG;
                            }
                        }                        
                        break;

                    case (int)ANALYIZEGEN9.ALDL_ASCII:
                    case (int)ANALYIZEGEN9.ALDL_HEX:                    
                    case (int)ANALYIZEGEN9.ALDL_DECIMAL:
                    case (int)ANALYIZEGEN9.ALDL_BITS:
                        if(strCommandName.Contains("[F]"))
                        {
                            if (!CheckGen9_ALDLType_FCP(strOrginSendData, bData, iAanlyizeOption, strParam, ref rtnData, ref STEPMANAGER_VALUE.bOldBinaryALDL))
                            {
                                return (int)STATUS.NG;
                            }
                        }
                        else
                        {
                            if (!CheckGen9_ALDLType(strOrginSendData, bData, iAanlyizeOption, strParam, ref rtnData, ref STEPMANAGER_VALUE.bOldBinaryALDL))
                            {
                                return (int)STATUS.NG;
                            }
                        }
                        break;   

                    case (int)ANALYIZEGEN9.ST_ESN_MEID:
                    case (int)ANALYIZEGEN9.ST_IMEI:
                    case (int)ANALYIZEGEN9.ST_ICCID:

                        //여기서 구조체 마샬링ㅎ자
                        if (!MarshalingFunc_MEID(bData, iAanlyizeOption, ref rtnData).Equals((int)STATUS.OK))
                        {
                            return (int)STATUS.NG;
                        }                       
                        break;

                    case (int)ANALYIZEGEN9.MDN_COUNTRY:
                    case (int)ANALYIZEGEN9.MDN_MIN:
                    case (int)ANALYIZEGEN9.MDN_MDN:
                    case (int)ANALYIZEGEN9.MDN_HOMESID:
                    case (int)ANALYIZEGEN9.MDN_COUNT:
                    case (int)ANALYIZEGEN9.MDN_SID_00:
                    case (int)ANALYIZEGEN9.MDN_SID_01: case (int)ANALYIZEGEN9.MDN_SID_02: case (int)ANALYIZEGEN9.MDN_SID_03: case (int)ANALYIZEGEN9.MDN_SID_04: case (int)ANALYIZEGEN9.MDN_SID_05:
                    case (int)ANALYIZEGEN9.MDN_SID_06: case (int)ANALYIZEGEN9.MDN_SID_07: case (int)ANALYIZEGEN9.MDN_SID_08: case (int)ANALYIZEGEN9.MDN_SID_09: case (int)ANALYIZEGEN9.MDN_SID_10:

                    case (int)ANALYIZEGEN9.MDN_NID_00: 
                    case (int)ANALYIZEGEN9.MDN_NID_01: case (int)ANALYIZEGEN9.MDN_NID_02: case (int)ANALYIZEGEN9.MDN_NID_03: case (int)ANALYIZEGEN9.MDN_NID_04: case (int)ANALYIZEGEN9.MDN_NID_05:                  case (int)ANALYIZEGEN9.MDN_NID_06:
                    case (int)ANALYIZEGEN9.MDN_NID_07: case (int)ANALYIZEGEN9.MDN_NID_08: case (int)ANALYIZEGEN9.MDN_NID_09: case (int)ANALYIZEGEN9.MDN_NID_10:

                        //여기서 구조체 마샬링ㅎ자
                        if (!MarshalingFunc_NAMMgmnt(bData, iAanlyizeOption, ref rtnData).Equals((int)STATUS.OK))
                        {
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.NONEED:  //measure 에 찍히는거 필요없을떄 (괜한 혼돈을 주는 값이 찍히는거 그냥 빼고싶을때 사용)

                        rtnData = "";                        
                        break;

                    case (int)ANALYIZEGEN9.GEN9_IMSI:  //GEN9 TCU 로 부터 IMSI 값을 읽을때는 바이트 오더기준 지그재그 진법으로 읽어야함. 헐랭
                        //214074200611363 -> NAD DLL 로 읽었을떄 
                        //31 30 32 00 09 08 29 41 70 24 00 16 31 36  -> TCU 명령으로 읽었을때 

                        //그러므로 일단 조건을 정하자. 1. 데이터 총 14바이트 이면 지그재그 파싱간다!
                        if (bData.Length.Equals(14))
                        {
                            if (!bData[0].Equals(0x31))
                            {
                                rtnData = "RESPONSE FAIL";
                                return (int)STATUS.NG;
                            }
                            string strGen9Imsi = String.Empty;
                            string strReverse = String.Empty;
                            for (int ix = 0; ix < 8; ix++)
                            {
                                strReverse = bData[6 + ix].ToString("X2");
                                strGen9Imsi += strReverse.Substring(1, 1) + strReverse.Substring(0, 1);
                            }

                            rtnData = strGen9Imsi.Substring(1, 15);                            
                            rtnData = rtnData.Trim(); //공백제거.
                            rtnData = rtnData.Replace("-", String.Empty);
                            rtnData = rtnData.Replace("\0", String.Empty);
                        }
                        else
                        {
                            rtnData = "DATA SIZE ERROR";
                            return (int)STATUS.NG;
                        }                        
                        break;

                    case (int)ANALYIZEGEN9.GEN9_ICCID:
                        //8934076379000069038F
                        //31 30 30 00 0A 98 43 70 36 97 00 00 96 30 F8
                                                
                        //그러므로 일단 조건을 정하자. 1. 데이터 총 15바이트 이면 지그재그 파싱간다!
                        if (bData.Length.Equals(15))
                        {
                            if (!bData[0].Equals(0x31))
                            {
                                rtnData = "RESPONSE FAIL";
                                return (int)STATUS.NG;
                            }
                            string strGen9Imsi = String.Empty;
                            string strReverse = String.Empty;
                            for (int ix = 0; ix < 10; ix++)
                            {
                                strReverse = bData[5 + ix].ToString("X2");
                                strGen9Imsi += strReverse.Substring(1, 1) + strReverse.Substring(0, 1);
                            }

                            rtnData = strGen9Imsi;
                            rtnData = rtnData.Trim(); //공백제거.
                            rtnData = rtnData.Replace("-", String.Empty);
                            rtnData = rtnData.Replace("\0", String.Empty);
                        }
                        else
                        {
                            rtnData = "DATA SIZE ERROR";
                            return (int)STATUS.NG;
                        }                       
                        break;

                    case (int)ANALYIZEGEN9.GEN9_APN:
                    case (int)ANALYIZEGEN9.GEN9_APN_PW:
                    case (int)ANALYIZEGEN9.GEN9_NAI:                        

                        if (MarshalingFunc_NAID(bData, iAanlyizeOption, ref rtnData))
                        {
                            rtnData = rtnData.Trim(); //공백제거.
                            rtnData = rtnData.Replace("\0", String.Empty);
                        }
                        else 
                        {
                            rtnData = "GEN9_APN ERROR";
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.FINFOSIZE:

                        if (!bData[0].Equals(0x31))
                        {
                            return (int)STATUS.NG;
                        }

                        rtnData = Encoding.UTF8.GetString(bData, 1, bData.Length - 1);
                        rtnData = rtnData.Trim();   //공백제거.
                        rtnData = rtnData.Replace("\0", String.Empty);
                        break;

                    case (int)ANALYIZEGEN9.NORMAL:
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

        private bool MarshalingFunc_NAID(byte[] buff, int iType, ref string strMessage)
        {

            try
            {
                _stNAID tmpNAID = new _stNAID();

                InitialNAID(ref tmpNAID);

                int iSizeSturct = Marshal.SizeOf(tmpNAID);

                IntPtr iBuff = Marshal.AllocHGlobal(iSizeSturct);

                Marshal.Copy(buff, 0, iBuff, iSizeSturct);

                object obj = Marshal.PtrToStructure(iBuff, typeof(_stNAID)); // 복사된 데이터를 구조체 객체로 변환한다.

                tmpNAID = (_stNAID)obj;

                Marshal.FreeHGlobal(iBuff);

                switch (iType)
                {
                    case (int)ANALYIZEGEN9.GEN9_APN:

                        strMessage = Encoding.UTF8.GetString(tmpNAID.m_cpNAI, 1, tmpNAID.m_cpNAI[0]);
                        break;
                    case (int)ANALYIZEGEN9.GEN9_APN_PW:
                        strMessage = Encoding.UTF8.GetString(tmpNAID.m_cpPassword);
                        strMessage = strMessage.Replace("\0", "");
                        break;

                    case (int)ANALYIZEGEN9.GEN9_NAI:
                        strMessage = String.Empty;
                        for (int i = 1; i < tmpNAID.m_cpNAI.Length; i++)
                        {
                            if (tmpNAID.m_cpNAI[i].Equals(0x00)) break;
                            strMessage += (char)tmpNAID.m_cpNAI[i];
                        }
                        strMessage = strMessage.Replace("\0", "");
                        break;

                    default:
                        break;
                }

                return true;
            }
            catch
            {
                strMessage = "Response Format Error.";
                return false;
            }
            
        }

        private int MarshalingFunc_MEID(byte[] buff, int iType, ref string strMessage)
        {
            strMessage = String.Empty;
            int iReturn = (int)STATUS.OK;

            int structSizeA = Marshal.SizeOf(typeof(_stMEIDpESND));
            int structSizeB = Marshal.SizeOf(typeof(_stMEIDpESND_GSM));

            _stMEIDpESND     stDataA = new _stMEIDpESND();
            _stMEIDpESND_GSM stDataB = new _stMEIDpESND_GSM();

            IntPtr informations;
            object obj;

            if (buff.Length > 0 && !buff[0].Equals(0x31))
            {
                strMessage = "Failure Data Structure.";
                return (int)STATUS.NG;
            }

            try
            {                
                if (buff.Length - 1 == structSizeA)
                {
                    informations = Marshal.AllocHGlobal(structSizeA);

                    Marshal.Copy(buff, 1, informations, structSizeA);

                    obj = Marshal.PtrToStructure(informations, typeof(_stMEIDpESND));

                    stDataA = (_stMEIDpESND)obj;

                    Marshal.FreeHGlobal(informations);

                    switch(iType)
                    {
                        case (int)ANALYIZEGEN9.ST_ESN_MEID: 
                            
                            if(stDataA.m_bMEIDSet)
                            {
                                strMessage = BitConverter.ToString(stDataA.m_aMeid).Replace("-", "").ToUpper();
                            }
                            else
                            {
                                strMessage = BitConverter.ToString(stDataA.m_apEsn).Replace("-", "").ToUpper();
                            }
                            break;

                        case (int)ANALYIZEGEN9.ST_IMEI:
                        case (int)ANALYIZEGEN9.ST_ICCID:
                        default:

                            strMessage = "Not Supported"; //다른건 지원하지 않음
                            iReturn = (int)STATUS.NG;

                            break;
                    }

                }
                else if (buff.Length - 1 == structSizeB)
                {
                    informations = Marshal.AllocHGlobal(structSizeB);

                    Marshal.Copy(buff, 1, informations, structSizeB);

                    obj = Marshal.PtrToStructure(informations, typeof(_stMEIDpESND_GSM));

                    stDataB = (_stMEIDpESND_GSM)obj;

                    Marshal.FreeHGlobal(informations);

                    switch (iType)
                    {
                        case (int)ANALYIZEGEN9.ST_ESN_MEID:

                            if (stDataA.m_bMEIDSet)
                            {
                                strMessage = BitConverter.ToString(stDataB.m_aMeid).Replace("-", "").ToUpper();
                            }
                            else
                            {
                                strMessage = BitConverter.ToString(stDataB.m_apEsn).Replace("-", "").ToUpper();
                            }
                            break;

                        case (int)ANALYIZEGEN9.ST_IMEI:
                            string strTempIMEI = BitConverter.ToString(stDataB.m_aIMEI).Replace("-", "").ToUpper();

                            //IMEI 는 15자리 이므로 컷팅하자.
                            if(strTempIMEI.Length > 15)
                                strMessage = strTempIMEI.Substring(0, 15);
                            else
                                strMessage = strTempIMEI;
                            break;
                        case (int)ANALYIZEGEN9.ST_ICCID:
                            strMessage = BitConverter.ToString(stDataB.m_aICCID).Replace("-", "").ToUpper();
                            break;
                        default:

                            strMessage = "Not Supported"; //다른건 지원하지 않음
                            iReturn = (int)STATUS.NG;

                            break;
                    }

                }
                else
                {
                    strMessage = "Struct Size Error.";
                    iReturn = (int)STATUS.NG;
                }

            }
            catch
            {
                strMessage = "MarshalingFunc Error.";
                iReturn = (int)STATUS.NG;
            }
        

            
            return iReturn;

        }

        private int MarshalingFunc_NAMMgmnt(byte[] buff, int iType, ref string strMessage)
        {
            strMessage = String.Empty;
            int iReturn = (int)STATUS.OK;

            try
            {
                int structSize = Marshal.SizeOf(typeof(_NAMMgmnt));

                _NAMMgmnt stData = new _NAMMgmnt();

                IntPtr informations;
                object obj;

                informations = Marshal.AllocHGlobal(structSize);

                Marshal.Copy(buff, 0, informations, structSize);

                obj = Marshal.PtrToStructure(informations, typeof(_NAMMgmnt));

                stData = (_NAMMgmnt)obj;

                Marshal.FreeHGlobal(informations);

                switch (iType)
                {
                    case (int)ANALYIZEGEN9.MDN_COUNTRY:
                        strMessage = Encoding.UTF8.GetString(stData.m_cpMIN, 0, 5);
                        break;

                    case (int)ANALYIZEGEN9.MDN_MIN:
                        strMessage = Encoding.UTF8.GetString(stData.m_cpMIN, 5, 11);
                        break;

                    case (int)ANALYIZEGEN9.MDN_MDN:
                        strMessage = Encoding.UTF8.GetString(stData.m_cpMDN, 0, stData.m_cpMDN.Length);
                        break;

                    case (int)ANALYIZEGEN9.MDN_HOMESID:
                        try
                        {
                            byte[] bArrayHomeSID = new byte[2];
                            bArrayHomeSID = BitConverter.GetBytes(stData.m_sHomeSID);
                            Array.Reverse(bArrayHomeSID);
                            strMessage = BitConverter.ToUInt16(bArrayHomeSID, 0).ToString();
                        }
                        catch
                        {
                            strMessage = "error MDN_HOMESID"; 
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.MDN_COUNT:
                        try
                        {
                            byte[] bArrayPairCount = new byte[4];
                            bArrayPairCount = BitConverter.GetBytes(stData.m_iPairsCnt);
                            Array.Reverse(bArrayPairCount);
                            strMessage = BitConverter.ToUInt16(bArrayPairCount, 0).ToString();
                        }
                        catch
                        {
                            strMessage = "error MDN_COUNT"; //다른건 지원하지 않음
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.MDN_SID_00: case (int)ANALYIZEGEN9.MDN_SID_01: case (int)ANALYIZEGEN9.MDN_SID_02: case (int)ANALYIZEGEN9.MDN_SID_03: case (int)ANALYIZEGEN9.MDN_SID_04:
                    case (int)ANALYIZEGEN9.MDN_SID_05: case (int)ANALYIZEGEN9.MDN_SID_06: case (int)ANALYIZEGEN9.MDN_SID_07: case (int)ANALYIZEGEN9.MDN_SID_08: case (int)ANALYIZEGEN9.MDN_SID_09:
                    case (int)ANALYIZEGEN9.MDN_SID_10:

                        int iSidIndex = 0;
                        iSidIndex = iType - (int)ANALYIZEGEN9.MDN_SID_00;

                        try
                        {
                            byte[] bArrayMDN_SID = new byte[2];
                            bArrayMDN_SID = BitConverter.GetBytes(stData.m_stSIDNIDPairs[iSidIndex].m_sSID);
                            Array.Reverse(bArrayMDN_SID);
                            strMessage = BitConverter.ToUInt16(bArrayMDN_SID, 0).ToString();
                        }
                        catch
                        {
                            strMessage = "error MDN_SID"; 
                            return (int)STATUS.NG;
                        }
                        break;

                    case (int)ANALYIZEGEN9.MDN_NID_00: case (int)ANALYIZEGEN9.MDN_NID_01: case (int)ANALYIZEGEN9.MDN_NID_02: case (int)ANALYIZEGEN9.MDN_NID_03: case (int)ANALYIZEGEN9.MDN_NID_04:
                    case (int)ANALYIZEGEN9.MDN_NID_05: case (int)ANALYIZEGEN9.MDN_NID_06: case (int)ANALYIZEGEN9.MDN_NID_07: case (int)ANALYIZEGEN9.MDN_NID_08: case (int)ANALYIZEGEN9.MDN_NID_09:
                    case (int)ANALYIZEGEN9.MDN_NID_10: 

                        int iNidIndex = 0;
                        iNidIndex = iType - (int)ANALYIZEGEN9.MDN_NID_00;

                        try
                        {
                            byte[] bArrayMDN_NID = new byte[2];
                            bArrayMDN_NID = BitConverter.GetBytes(stData.m_stSIDNIDPairs[iNidIndex].m_sNID);
                            Array.Reverse(bArrayMDN_NID);
                            strMessage = BitConverter.ToUInt16(bArrayMDN_NID, 0).ToString();
                        }
                        catch
                        {
                            strMessage = "error MDN_NID"; 
                            return (int)STATUS.NG;
                        }
                        break;

                    default:
                        strMessage = "Unknown Index"; //다른건 지원하지 않음
                        return (int)STATUS.NG;
                }
            }
            catch
            {
                return (int)STATUS.NG;
            }

            strMessage = strMessage.Replace("\0", String.Empty);
            strMessage = strMessage.Trim(); //공백제거.
            return iReturn;
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

        private bool CheckGEN9_SimInfoType(byte[] bSendPack, byte[] bGetData, ref string strRetunData)
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
                            bool bIsAscii = CheckASCII(0, lBytes.Count, lBytes.ToArray());

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
                        strRetunData = "CheckGEN9_SimInfoType:Exception1";
                        return false;
                    }

                }

                return true;
            }
            catch
            {
                strRetunData = "CheckGEN9_SimInfoType:Exception2";
                return false;
            }


        }

        private bool CheckGEN9_ReverseType(string strSendCommand, byte[] bGetData, ref string strRetunData)
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

        private bool CheckGEN9_DtcIndex(byte bSendParam, byte[] bGetData, ref string strRetunData)
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

                //바이트만 그대로 나타내는 방법 (20160229)
                strRetunData = bGetData[2].ToString("X2");

                return true;
            }
            catch
            {
                strRetunData = "CheckGEN9_DtcIndex:Exception";
                return false;
            }

        }

        private bool CheckASCII(int iStx, int iDataLen, byte[] bData)
        {
            //여기서 아스키로 변환해야하는지 한바이트만 체크하자.
            bool bIsAscii = false;
            int iDx = iDataLen;
            if (iDataLen > bData.Length || iDataLen < 1 )
            {
                iDx = bData.Length;
            }

            for (int i = iStx; i < iDx; i++)
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

        //이동성 책임 요청으로 GEN9모델에서 ALDL 관련 패킷이 다르기때문에 길이 체크하는 부분 빼고 전부 같은 함수를 만듬
        private bool CheckGen9_ALDLType_FCP(byte[] bSendPack, byte[] bGetData, int iReadOption, string strParam, ref string strRetunData, ref byte[] bReadBinary)
        {
            try
            {
                //if (bGetData.Length != 0x49) return false;  //Gen9 는 ALDL 사이즈가 0x49 (73byte) 인 듯하다.

                byte bBlockSize = bGetData[0];


                if (bBlockSize.Equals(0x00))
                {
                    strRetunData = "ALDL None Block Size : 0x00";
                    return false;
                }

                if (bBlockSize > bGetData.Length - 1)
                {
                    strRetunData = "ALDL None Block Size RangeOver : " + bBlockSize.ToString("X2");
                    return false;
                }

                bReadBinary = new byte[bBlockSize];

                Array.Copy(bGetData, 1, bReadBinary, 0, (int)bBlockSize);

                int iStxPoint = 0;
                int iEtxPoint = 0;
                string[] strRangeOption = strParam.Split(',');

                switch (strRangeOption.Length)
                {
                    case 3:
                        bool bStx = int.TryParse(strRangeOption[1], out iStxPoint);
                        bool bEtx = int.TryParse(strRangeOption[2], out iEtxPoint);
                        if (!bStx || !bEtx)
                        {
                            strRetunData = "Error PAR1 Option : " + strParam;
                            return false;
                        }
                        else
                        {
                            if (iStxPoint < 0 || iStxPoint > bReadBinary.Length - 1 || iEtxPoint <= 0 ||
                                 iStxPoint + iEtxPoint > bReadBinary.Length)
                            {
                                strRetunData = "Error PAR1 Option : " + strParam;
                                return false;
                            }
                        }

                        break;
                    default:
                        iStxPoint = 0;
                        iEtxPoint = bReadBinary.Length;
                        break;
                }


                switch (iReadOption)
                {
                    case (int)ANALYIZEGEN9.ALDL_ASCII:

                        bool bIsAscii = CheckASCII(iStxPoint, iEtxPoint, bReadBinary);

                        if (bIsAscii)
                        {
                            strRetunData = Encoding.UTF8.GetString(bReadBinary, iStxPoint, iEtxPoint);
                        }
                        else
                        {
                            strRetunData = "Can Not Read by ASCII : " + BitConverter.ToString(bReadBinary, 0, (int)bReadBinary.Length).Replace("-", "");
                            return false;
                        }
                        break;

                    case (int)ANALYIZEGEN9.ALDL_HEX:

                        strRetunData = BitConverter.ToString(bReadBinary, iStxPoint, iEtxPoint).Replace("-", "");
                        break;

                    case (int)ANALYIZEGEN9.ALDL_DECIMAL:
                        //만들자 여기 4바이트 이내 혹은 정해진 인덱스 부터 읽혀진 숫자

                        int iDec = 0;
                        try
                        {
                            byte[] bDecimalBytes = new byte[4];
                            Array.Copy(bReadBinary, iStxPoint, bDecimalBytes, 0, bDecimalBytes.Length);
                            Array.Reverse(bDecimalBytes);
                            iDec = BitConverter.ToInt32(bDecimalBytes, 0);
                        }
                        catch
                        {
                            strRetunData = "Can Not Read by Decimal : " + BitConverter.ToString(bReadBinary, 0, (int)bReadBinary.Length).Replace("-", "");
                            return false;
                        }

                        strRetunData = iDec.ToString();
                        break;

                    case (int)ANALYIZEGEN9.ALDL_BITS:
                        StringBuilder sb = new StringBuilder();

                        for (int i = iStxPoint; i < iEtxPoint; i++)
                        {
                            byte a = Convert.ToByte(bReadBinary[i]);
                            sb.Append(Convert.ToString(a, 2).PadLeft(8, '0'));
                        }
                        strRetunData = sb.ToString();
                        break;
                    default:

                        strRetunData = "Unknown  ALDL ReadOption.";
                        return false;
                }

                strRetunData = strRetunData.Replace("\0", String.Empty);

                return true;
            }
            catch
            {
                strRetunData = "CheckGEN9_ALDLType:Exception";
                return false;
            }

        }

        private bool CheckGen9_ALDLType(byte[] bSendPack, byte[] bGetData, int iReadOption, string strParam, ref string strRetunData, ref byte[] bReadBinary)
        {
            try
            {   
                if (bGetData.Length != 0x49) return false;  //Gen9 는 ALDL 사이즈가 0x49 (73byte) 인 듯하다.

                byte bBlockSize = bGetData[0];
                

                if (bBlockSize.Equals(0x00))
                {
                    strRetunData = "ALDL None Block Size : 0x00";
                    return false;
                }

                if (bBlockSize > bGetData.Length - 1)
                {
                    strRetunData = "ALDL None Block Size RangeOver : " + bBlockSize.ToString("X2");
                    return false;
                }

                bReadBinary = new byte[bBlockSize];

                Array.Copy(bGetData, 1, bReadBinary, 0, (int)bBlockSize);

                int iStxPoint = 0;
                int iEtxPoint = 0;
                string[] strRangeOption = strParam.Split(',');

                switch (strRangeOption.Length)
                {
                    case 3:
                        bool bStx = int.TryParse(strRangeOption[1], out iStxPoint);
                        bool bEtx = int.TryParse(strRangeOption[2], out iEtxPoint);
                        if (!bStx || !bEtx)
                        {
                            strRetunData = "Error PAR1 Option : " + strParam;
                            return false;
                        }
                        else
                        {
                            if (iStxPoint < 0 || iStxPoint > bReadBinary.Length - 1 || iEtxPoint <= 0 ||
                                 iStxPoint + iEtxPoint > bReadBinary.Length)
                            {
                                strRetunData = "Error PAR1 Option : " + strParam;
                                return false;
                            }
                        }

                        break;
                    default:
                        iStxPoint = 0;
                        iEtxPoint = bReadBinary.Length;
                        break;
                }


                switch (iReadOption)
                {
                    case (int)ANALYIZEGEN9.ALDL_ASCII:

                        bool bIsAscii = CheckASCII(iStxPoint, iEtxPoint, bReadBinary);

                        if (bIsAscii)
                        {
                            strRetunData = Encoding.UTF8.GetString(bReadBinary, iStxPoint, iEtxPoint);
                        }
                        else
                        {
                            strRetunData = "Can Not Read by ASCII : " + BitConverter.ToString(bReadBinary, 0, (int)bReadBinary.Length).Replace("-", "");
                            return false;
                        }
                        break;

                    case (int)ANALYIZEGEN9.ALDL_HEX:

                        strRetunData = BitConverter.ToString(bReadBinary, iStxPoint, iEtxPoint).Replace("-", "");
                        break;

                    case (int)ANALYIZEGEN9.ALDL_DECIMAL:
                        //만들자 여기 4바이트 이내 혹은 정해진 인덱스 부터 읽혀진 숫자

                        int iDec = 0;
                        try
                        {
                            byte[] bDecimalBytes = new byte[4];
                            Array.Copy(bReadBinary, iStxPoint, bDecimalBytes, 0, bDecimalBytes.Length);
                            Array.Reverse(bDecimalBytes);
                            iDec = BitConverter.ToInt32(bDecimalBytes, 0);
                        }
                        catch
                        {
                            strRetunData = "Can Not Read by Decimal : " + BitConverter.ToString(bReadBinary, 0, (int)bReadBinary.Length).Replace("-", "");
                            return false;
                        }

                        strRetunData = iDec.ToString();
                        break;
                        
                    case (int)ANALYIZEGEN9.ALDL_BITS:
                        StringBuilder sb = new StringBuilder();

                        for (int i = iStxPoint; i < iEtxPoint; i++)
                        {
                            byte a = Convert.ToByte(bReadBinary[i]);
                            sb.Append(Convert.ToString(a, 2).PadLeft(8, '0'));
                        }
                        strRetunData = sb.ToString();
                        break;
                    default:

                        strRetunData = "Unknown  ALDL ReadOption.";
                        return false;
                }
                
                strRetunData = strRetunData.Replace("\0", String.Empty);
                
                return true;
            }
            catch
            {
                strRetunData = "CheckGEN9_ALDLType:Exception";
                return false;
            }

        }

        private void CheckExceptionCommands(string strCommandName, ref byte[] strOrginSendData)
        {
            //보낸명령어랑 응답 명령이 달라야하는 경우를 위해...
            byte[] bCommand = new byte[5];
            switch (strCommandName)
            {                    
                case "ALDL2_WRITE":
                case "ALDL2_WRITE_ASCII":
                    bCommand[0] = 0x33; bCommand[1] = 0x43; bCommand[2] = 0x50; bCommand[3] = 0x31; bCommand[4] = 0x66;
                    Array.Copy(bCommand, 0, strOrginSendData, 4, bCommand.Length);
                    break;

                default:
                    break;
            }

        }

        private int Analyze_FactoryProtocol(ref byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, int iAanlyizeOption, string strParam)
        {

            try
            {
                //0. VCP 응답패킷이 6바이트 이하면 계속 수신중으로 처리한다. 움하하하하
                if (tmpBytes.Length < 6) 
                    return (int)STATUS.RUNNING;

                //1. 버퍼링 나누기
                //vbvb 여기서 부터 다시하자
                int bChksum = (int)STATUS.TIMEOUT;

                int iFindStx = 0;
                bool bFind = false;
                int iSize = 0;
                for (int j = 0; j < tmpBytes.Length; j++)
                {
                    //1. STX 찾기 // origine 4,5,6,7,8 비교하자.
                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_FSTX)
                    {
                        if (tmpBytes.Length < (j + 6))
                            return (int)STATUS.RUNNING; //계속 수신중

                        if (strOrginSendData[3].Equals(tmpBytes[j + 3]) &&
                                tmpBytes[j + 2].Equals(DEFRES_FRES)) //보낸명령에 대한 응답일경우
                            bFind = true;
                        else
                            bFind = false;
                         
                        if (!bFind) continue;

                        iSize = (int)tmpBytes[j + 1] + 4;

                        if (tmpBytes.Length >= iSize)
                        {
                            iFindStx = j;                            
                            //1. ETX 찾기
                            
                            if (tmpBytes[j + iSize - 1].Equals(DEFRES_FETX))
                            {
                                byte[] tmpBuffer = new byte[iSize];

                                Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);


                                bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData, iAanlyizeOption, strParam);
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

        private int Analyze_FcpNormalProtocol(ref byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, int iAanlyizeOption, string strParam)
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
                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_STX1L && tmpBytes[j + 1] == DEFRES_STX2L)
                    {

                        if (tmpBytes.Length < (j + 3))
                            return (int)STATUS.RUNNING; //계속 수신중

                        //GPS 특별명령인지 체크
                        bool bSpecialFlag = strCommandName.Equals("GEN9_OLD_GPS_INFO");

                        for (int p = 0; p < 5; p++)
                        {
                            if (strOrginSendData[4 + p].Equals(tmpBytes[j + 4 + p]))
                            {
                                bFind = true;
                            }
                            else
                            {
                                if (p == 3 && bSpecialFlag)
                                {
                                    //GEN9의 OLD GPS INFO 수집일경우는 7번째가 항상 다르기때문에 continue 한다.
                                }
                                else
                                {
                                    bFind = false;
                                    break;
                                }
                            }
                        }

                        if (!bFind) continue;

                        if (tmpBytes.Length > (j + 5) && tmpBytes.Length >= ((int)tmpBytes[j + 3] + j + 4))
                        {
                            iFindStx = j;
                            iSize = (int)tmpBytes[j + 3] + ((int)tmpBytes[j + 2] * (int)0x100) + 4;

                            //if (tmpBytes.Length <= (j + iSize - 1)) //기존 원본소스
                            if (tmpBytes.Length < (j + iSize - 1))
                            {
                                return (int)STATUS.RUNNING; //계속 수신중
                            }

                            //1. ETX 찾기
                            //if (bFind && tmpBytes[j] == DEFRES_ETX) 
                            if (bFind && tmpBytes[j + iSize - 1] == DEFRES_ETXL)
                            {
                                byte[] tmpBuffer = new byte[iSize];

                                Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                                bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData, iAanlyizeOption, strParam);
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

        private int Analyze_NormalProtocol(ref byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, int iAanlyizeOption, string strParam)
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
                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_STX1L && tmpBytes[j + 1] == DEFRES_STX2L)
                    {

                        if (tmpBytes.Length < (j + 15))
                            return (int)STATUS.RUNNING; //계속 수신중

                        //GPS 특별명령인지 체크
                        bool bSpecialFlag = strCommandName.Equals("GEN9_OLD_GPS_INFO");

                        for (int p = 0; p < 5; p++)
                        {
                            if (strOrginSendData[4 + p].Equals(tmpBytes[j + 4 + p]))
                            {
                                bFind = true;
                            }
                            else
                            {
                                if (p == 3 && bSpecialFlag)
                                {
                                    //GEN9의 OLD GPS INFO 수집일경우는 7번째가 항상 다르기때문에 continue 한다.
                                }
                                else
                                {
                                    bFind = false;
                                    break;
                                }
                            }
                        }

                        if (!bFind) continue;

                        if (tmpBytes.Length > (j + 5) && tmpBytes.Length >= ((int)tmpBytes[j + 3] + j + 4))
                        {
                            iFindStx = j;
                            iSize = (int)tmpBytes[j + 3] + ((int)tmpBytes[j + 2] * (int)0x100) + 4;

                            //if (tmpBytes.Length <= (j + iSize - 1)) //기존 원본소스
                            if (tmpBytes.Length < (j + iSize - 1))
                            {
                                return (int)STATUS.RUNNING; //계속 수신중
                            }

                            //1. ETX 찾기
                            //if (bFind && tmpBytes[j] == DEFRES_ETX) 
                            if (bFind && tmpBytes[j + iSize - 1] == DEFRES_ETXL)
                            {
                                byte[] tmpBuffer = new byte[iSize];

                                Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                                bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData, iAanlyizeOption, strParam);
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

        private int Analyze_Gen9HighProtocol(ref byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, int iAanlyizeOption, string strParam)
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
                    if (!bFind && j + 1 < tmpBytes.Length && tmpBytes[j] == DEFRES_STX1H && tmpBytes[j + 1] == DEFRES_STX2H)
                    {

                        if (tmpBytes.Length < (j + 15))
                            return (int)STATUS.RUNNING; //계속 수신중

                        //예외명령인지 한번 체크          
                        CheckExceptionCommands(strCommandName, ref strOrginSendData);

                        //GPS 특별명령인지 체크
                        bool bSpecialFlag = strCommandName.Equals("GEN9_OLD_GPS_INFO");

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
                                    //GEN9의 OLD GPS INFO 수집일경우는 7번째가 항상 다르기때문에 continue 한다.
                                }
                                else
                                {
                                    bFind = false;
                                    break;
                                }
                            }
                        }

                        if (!bFind) continue;

                        if (tmpBytes.Length > (j + 5) && tmpBytes.Length >= ((int)tmpBytes[j + 5] + j + 6))
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
                            if (bFind && tmpBytes[j + iSize - 1] == DEFRES_ETXH)
                            {
                                byte[] tmpBuffer = new byte[iSize];

                                Array.Copy(tmpBytes, iFindStx, tmpBuffer, 0, tmpBuffer.Length);

                                bChksum = CheckProtocol(tmpBuffer, strOrginSendData, strCommandName, ref rtnData, iAanlyizeOption, strParam);
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
        
        public int AnalyzePacket(ref byte[] tmpBytes, ref string strCommand, ref string rtnData, byte[] strOrginSendData, string strCommandName, ref string strLogString, int iAanlyizeOption, string strParam)
        {
            //예외명령인지 한번 체크            
            CheckExceptionCommands(strCommandName, ref strOrginSendData);

            switch (AnalyzeMode)
            {
                case (int)CLSMODE.FACTORY:
                    return Analyze_FactoryProtocol(ref tmpBytes, ref strCommand, ref rtnData, strOrginSendData, strCommandName, ref strLogString, iAanlyizeOption, strParam);

                case (int)CLSMODE.NORMAL:
                    return Analyze_NormalProtocol(ref tmpBytes, ref strCommand, ref rtnData, strOrginSendData, strCommandName, ref strLogString, iAanlyizeOption, strParam);

                case (int)CLSMODE.GEN9HIGH:
                    return Analyze_Gen9HighProtocol(ref tmpBytes, ref strCommand, ref rtnData, strOrginSendData, strCommandName, ref strLogString, iAanlyizeOption, strParam);

                case (int)CLSMODE.FCP:
                    return Analyze_FcpNormalProtocol(ref tmpBytes, ref strCommand, ref rtnData, strOrginSendData, strCommandName, ref strLogString, iAanlyizeOption, strParam);

                default : 
                    return (int)STATUS.ERROR;
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

        public byte[] ConvertVcpByteHexString(string strPacket, ref string strSendPack, string strParam, ref bool brtnOk, ref string strReason, bool bCallClass) 
        {
            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strPacket, " ");
            List<string> tmpList = new List<string>();
            bool bOk = true;
            tmpList.Clear();
            strReason = String.Empty;
            string strValue = String.Empty;
            byte[] rtnNull = new byte[1];  //FAIL 리턴용
            
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
                                case "EFILE1":  //EFILE1,2  은 인증서업로드용이다.(For GEN9)
                                case "EFILE2":
                                case "LFILE1":  //LFILE1,2  은 로컬파일업로드용이다.(For GEN9)
                                case "LFILE2":
                                    bOk = true;
                                    byte[] byteFile = new byte[1024];
                                    switch (tmpString[i])
                                    {
                                        case "EFILE1":
                                        case "EFILE2":
                                            byteFile = MakeEfileStruct(iEfileType, strParam, ref bOk, ref strReason, bCallClass);
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

                                case "ALDL_OPTION":

                                            string[] strAldlParam = new string[3];

                                            strAldlParam = strParam.Split(',');

                                            
                                            switch (strAldlParam.Length)
                                            {
                                                case 1: //두개의 파라미터가 없으면 그냥 데이터 파싱할때 전체구간으로 한다. 
                                                case 3: //두개의 파라미터는 아날라이징시 데이터 파싱할때 구간용으로 쓰기 위해서.
                                                    bOk = true;
                                                    byte[] byteALDLByte = HexStringToBytes(strAldlParam[0], ref bOk);
                                                    if (bOk)
                                                    {
                                                        for (int iDx = 0; iDx < byteALDLByte.Length; iDx++)
                                                        {
                                                            tmpList.Add(byteALDLByte[iDx].ToString("X2"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                        brtnOk = false;
                                                        return rtnNull;
                                                    }
                                                    break;

                                                case 0:
                                                case 2:
                                                default:
                                                    strReason = "PAR1 ERROR(" + tmpString[i] + ")";
                                                    brtnOk = false;
                                                    return rtnNull;

                                            }
                                            if (String.IsNullOrEmpty(strParam))
                                            {
                                                strReason = "PAR1 ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            break;

                                case "ALDL1_ASCII_EX":

                                            //파라미터 5개인지 확인 , ID, FLAG + 00 00, DATA, start index, length
                                            string[] strALDL1ParEx = new string[1];
                                            string strChangeParEx = "";
                                            strALDL1ParEx = strParam.Split(',');

                                            if (strALDL1ParEx.Length != 5)
                                            {
                                                strReason = "PAR1 ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            strChangeParEx = strALDL1ParEx[0] + strALDL1ParEx[1] + "0000";

                                            string strNewParam = String.Empty;

                                            try
                                            {
                                                int iParStx = 0;
                                                int iParLength = 0;

                                                iParStx = int.Parse(strALDL1ParEx[3]);
                                                iParLength = int.Parse(strALDL1ParEx[4]);
                                                strNewParam = strALDL1ParEx[2].Substring(iParStx, iParLength);
                                            }
                                            catch
                                            {
                                                strReason = "PAR1 LENGTH ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            strChangeParEx += BitConverter.ToString(Encoding.UTF8.GetBytes(strNewParam)).Replace("-", "");

                                            bOk = true;
                                            byte[] byteALDLParEx = HexStringToBytes(strChangeParEx, ref bOk);
                                            if (bOk)
                                            {
                                                for (int iDx = 0; iDx < byteALDLParEx.Length; iDx++)
                                                {
                                                    tmpList.Add(byteALDLParEx[iDx].ToString("X2"));
                                                }
                                            }
                                            else
                                            {
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;


                                case "ALDL1_HEXA":
                                case "ALDL1_ASCII":
                                case "ALDL1_DECIMAL":
                                    
                                            //파라미터 3개인지 확인 , ID, FLAG + 00 00, DATA
                                            string[] strALDL1Par = new string[1];
                                            string strChangePar = "";
                                            strALDL1Par = strParam.Split(',');

                                            if (strALDL1Par.Length != 3)
                                            {
                                                strReason = "PAR1 ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            switch (tmpString[i])
                                            {
                                                case "ALDL1_HEXA":
                                                    strChangePar = strALDL1Par[0] + strALDL1Par[1] + "0000" + strALDL1Par[2]; 
                                                    break;
                                                case "ALDL1_ASCII":
                                                    strChangePar = strALDL1Par[0] + strALDL1Par[1] + "0000";                                                                                                        
                                                    strChangePar += BitConverter.ToString(Encoding.UTF8.GetBytes(strALDL1Par[2])).Replace("-", "");
                                                    break;

                                                case "ALDL1_DECIMAL":

                                                    UInt32 iDecimal = 0;
                                                    if (UInt32.TryParse(strALDL1Par[2], out iDecimal))
                                                    {                                                        
                                                        byte[] bDecimal = new byte[4];
                                                        bDecimal = BitConverter.GetBytes(iDecimal);
                                                        Array.Reverse(bDecimal); //바이트오더 역순정렬.                                                        
                                                        strChangePar = strALDL1Par[0] + strALDL1Par[1] + "0000" + BitConverter.ToString(bDecimal).Replace("-", "");
                                                    }
                                                    else
                                                    {
                                                        strReason = "DECIMAL Range Over(" + strALDL1Par[2] + ")";
                                                        brtnOk = false;
                                                        return rtnNull;
                                                    }                                                    
                                                    break;

                                            }


                                            bOk = true;
                                            byte[] byteALDLPar = HexStringToBytes(strChangePar, ref bOk);
                                            if (bOk)
                                            {
                                                for (int iDx = 0; iDx < byteALDLPar.Length; iDx++)
                                                {
                                                    tmpList.Add(byteALDLPar[iDx].ToString("X2"));
                                                }
                                            }
                                            else
                                            {
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;

                                case "ALDL2_HEXA":
                                case "ALDL2_ASCII":

                                            //파라미터 4개인지 확인 , ID, StartIndex, DataSize, DATA
                                            string[] strALDL2Par = new string[1];
                                            string strChangePar2 = "";
                                            strALDL2Par = strParam.Split(',');

                                            if (strALDL2Par.Length != 4)
                                            {
                                                strReason = "PAR1 ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            for (int j = 0; j < strALDL2Par.Length; j++)
                                            {
                                                if (String.IsNullOrEmpty(strALDL2Par[j]))
                                                {
                                                    strReason = "PAR1 ERROR";
                                                    brtnOk = false;
                                                    return rtnNull;
                                                }
                                            }

                                            string strDataSize   = String.Empty;
                                            string strStartIndex = String.Empty;
                                            int iStartIndex = 0;
                                            int iDataSize = 0;
                                                                                
                                            if(!int.TryParse(strALDL2Par[1], out iStartIndex))
                                            {
                                                strReason = "PAR1 START INDEX ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            if(!int.TryParse(strALDL2Par[2], out iDataSize))
                                            {
                                                strReason = "PAR1 DATA SIZE ERROR";
                                                brtnOk = false;
                                                return rtnNull;
                                            }

                                            strStartIndex = iStartIndex.ToString("X2");
                                            strDataSize   = iDataSize.ToString("X2");

                                            switch (tmpString[i])
                                            {
                                                case "ALDL2_HEXA":   //Block ID + StartIndex + DataSize + Data;                                                    
                                                    strChangePar2  = strALDL2Par[0] + strStartIndex + strDataSize + strALDL2Par[3];
                                                    break;
                                                case "ALDL2_ASCII":                                                    
                                                    strChangePar2  = strALDL2Par[0] + strStartIndex + strDataSize;
                                                    strChangePar2 += BitConverter.ToString(Encoding.UTF8.GetBytes(strALDL2Par[3])).Replace("-", "");
                                                    break;
                                            }

                                            bOk = true;
                                            byte[] byteALDL2Par = HexStringToBytes(strChangePar2, ref bOk);
                                            if (bOk)
                                            {
                                                for (int iDx = 0; iDx < byteALDL2Par.Length; iDx++)
                                                {
                                                    tmpList.Add(byteALDL2Par[iDx].ToString("X2"));
                                                }
                                            }
                                            else
                                            {
                                                strReason = "DATA TYPE ERROR(" + tmpString[i] + ")";
                                                brtnOk = false;
                                                return rtnNull;
                                            }
                                            break;
                                case "GEN9NAI":

                                    if (String.IsNullOrEmpty(strParam))
                                    {
                                        strReason = "PAR1 EMPTY()";
                                        brtnOk = false;
                                        return rtnNull;
                                    }

                                    string[] strPars = new string[2];

                                    strPars = strParam.Split(',');

                                    if(strPars.Length != 2)
                                    {
                                        strReason = "PAR1 ERROR.";
                                        brtnOk = false;
                                        return rtnNull;
                                    }

                                    byte[] byteNAI = Encoding.UTF8.GetBytes(strPars[0]);
                                    byte[] bytePW  = Encoding.UTF8.GetBytes(strPars[1]);

                                    _stNAID tmpNAID2 = new _stNAID();
                                    InitialNAID(ref tmpNAID2);

                                    tmpNAID2.m_sResCode = 1;
                                    tmpNAID2.m_sMsgID = 154;
                                    tmpNAID2.m_sNAISize2 = (byte)strPars[0].Length;

                                    Array.Copy(byteNAI, 0, tmpNAID2.m_cpNAI, 0, byteNAI.Length);
                                    Array.Copy(bytePW,  0, tmpNAID2.m_cpPassword, 0, bytePW.Length);

                                    int iSizeSturct2 = System.Runtime.InteropServices.Marshal.SizeOf(tmpNAID2);

                                    IntPtr iBuff2 = System.Runtime.InteropServices.Marshal.AllocHGlobal(iSizeSturct2);

                                    System.Runtime.InteropServices.Marshal.StructureToPtr(tmpNAID2, iBuff2, false);

                                    byte[] byteAPNs2 = new byte[iSizeSturct2];

                                    System.Runtime.InteropServices.Marshal.Copy(iBuff2, byteAPNs2, 0, iSizeSturct2);

                                    string strArray2 = String.Empty;

                                    strArray2 = BitConverter.ToString(byteAPNs2).Replace("-", "");

                                    System.Runtime.InteropServices.Marshal.FreeHGlobal(iBuff2);

                                    for (int iDx = 0; iDx < byteAPNs2.Length; iDx++)
                                    {
                                        tmpList.Add(byteAPNs2[iDx].ToString("X2"));
                                    }
                                    break;

                                case "GEN9APN":                                  
                                    if (String.IsNullOrEmpty(strParam))
                                    {
                                        strReason = "PAR1 EMPTY()";
                                        brtnOk = false;
                                        return rtnNull;
                                    }
                                                                      
                                    _stNAID tmpNAID = new _stNAID();
                                    InitialNAID(ref tmpNAID);

                                    tmpNAID.m_sResCode  = 1;
                                    tmpNAID.m_sMsgID    = 154;
                                    tmpNAID.m_sNAISize2 = (byte)strParam.Length;                         
                
                                    byte[] byteTemp = Encoding.UTF8.GetBytes(strParam);

                                    Array.Copy(byteTemp, 0, tmpNAID.m_cpNAI, 0, byteTemp.Length);

                                    int iSizeSturct = System.Runtime.InteropServices.Marshal.SizeOf(tmpNAID);

                                    IntPtr iBuff = System.Runtime.InteropServices.Marshal.AllocHGlobal(iSizeSturct);

                                    System.Runtime.InteropServices.Marshal.StructureToPtr(tmpNAID, iBuff, false);            

                                    byte[] byteAPNs = new byte[iSizeSturct];

                                    System.Runtime.InteropServices.Marshal.Copy(iBuff, byteAPNs, 0, iSizeSturct);            

                                    string strArray = String.Empty;

                                    strArray = BitConverter.ToString(byteAPNs).Replace("-", "");

                                    System.Runtime.InteropServices.Marshal.FreeHGlobal(iBuff);
                                                                                    
                                  
                                    for (int iDx = 0; iDx < byteAPNs.Length; iDx++)
                                    {
                                        tmpList.Add(byteAPNs[iDx].ToString("X2"));
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
                    DKCHK.Gen9_chksum(tmpArray, ref bHighByte, ref bLowByte, true);                    
                    tmpList.RemoveAt(i);
                    tmpList.Insert(i, bLowByte.ToString("X2"));   //BIG ENDIAN 
                    tmpList.Insert(i, bHighByte.ToString("X2"));   

                }
                else if (tmpList[i].Equals("<CX>"))
                {
                    byte[] tmpArray = new byte[tmpList.Count];

                    for (int j = 0; j < i/*tmpArray.Length*/; j++)
                    {//체크섬위치를 찾으면 체크섬 앞의 위치까지를 재정렬한다.
                        try { tmpArray[j] = Convert.ToByte(tmpList[j], 16); }
                        catch (Exception ex)
                        {
                            string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":2:" + ex.Message;
                            STEPMANAGER_VALUE.DebugView(strExMsg);
                            tmpArray[j] = 0xFF;
                        }
                    }


                    DK_CHECKSUM DKCHK = new DK_CHECKSUM(); //체크섬!
                    byte bCX = 0x00;                    
                    DKCHK.Gen9_CX(tmpArray, ref bCX);
                    tmpList.RemoveAt(i);
                    tmpList.Insert(i, bCX.ToString("X2"));                    
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

        public void InitialNAID(ref _stNAID target)
        {
            target = new _stNAID();
            target.dummy = new byte[4];
            target.m_sResCode = 0;
            target.m_sActionCode = 0;
            target.m_sFormat = 0;
            target.m_sPortReqType = 0;
            target.m_cpPassword = new byte[32];
        
            target.m_sNAISize1 = 0;
            target.m_sNAISize2 = 0;
            target.m_cpNAI = new byte[128];
        	
            target.m_usPort = 0;
            target.m_sMsgID = 0;
            target.m_sPortStatus = 0;

            return;
        }

        public byte[] MakeEfileStruct(int iEfileIdx, string strParam, ref bool bRes, ref string strReason, bool bCallClass)
        {
            byte[] bReturnBytes = new byte[10];
            byte[] bFileCount  = new byte[4];   //현재는 멀티로 전송하는것이 구현이 안되었으므로 한번만 보냄.
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
                case "KIS_error_code":      strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrCode];       break;
                case "KIS_error_message":   strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.ErrMsg];        break;
                case "KIS_stid":            strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.STID];          break;
                case "KIS_rCert":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.rCERT];         break;
                case "KIS_ccCert":          strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.cCert];         break;
                case "KIS_vCert":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vCert];         break;
                case "KIS_vPri":            strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPri];          break;
                case "KIS_vPre":            strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vPre];          break;
                case "KIS_vAuth":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vAuth];         break;
                case "KIS_vHash":           strRealData = STEPMANAGER_VALUE.KIS_DATA[(int)EFILECODE.vHash];         break;
                                    
                default:
                    strRealData = strTempSpl[2]; break;
                /*
                strReason = "KIS Data Error (" + strTempSpl[2] + ") ";
                bReturnBytes = new byte[iFuncBytes];
                bRes = false;
                return bReturnBytes;                
                
                    break;
                */
            }
           
            /*

            //임시데이터 테스트
            switch (strTempSpl[2])
            {
                case "KIS_error_code": strRealData = "0"; break;
                case "KIS_error_message": strRealData = ""; break;
                case "KIS_stid": strRealData = "65330058"; break;
                case "KIS_rCert": strRealData = "-----BEGIN CERTIFICATE-----MIIBtTCCAVugAwIBAgIIBPOrptbChgEwCgYIKoZIzj0EAwIwPDESMBAGA1UECgwJVVZfT05TVEFSMRIwEAYDVQQLDAlVVl9PTlNUQVIxEjAQBgNVBAMMCVVWX09OU1RBUjAgFw0xMjAyMTQxOTQ3NTdaGA8yMTAwMTIzMTIzNTk1OVowPDESMBAGA1UECgwJVVZfT05TVEFSMRIwEAYDVQQLDAlVVl9PTlNUQVIxEjAQBgNVBAMMCVVWX09OU1RBUjBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABL49Uqtem4/w6StL6U3LeRTtKj4J/3XigQnAVAWDxEgI+uAKb1awAGF5sF0I69Ars9B0JKV/AB9PLBwJL1xfYzqjRTBDMA4GA1UdDwEB/wQEAwIBBjASBgNVHRMBAf8ECDAGAQH/AgEAMB0GA1UdDgQWBBTV7Jj5gkS/MpAyVtIcPvc2vg9XZDAKBggqhkjOPQQDAgNIADBFAiASEwCZHX4q4A40Bc95KBsTAPx3wlGgsayS62Kr4nwpSAIhAKr56R0CLUbuBhjc3xfTzOgbkRhCahHUw6mAC10j4a+D-----END CERTIFICATE-----"; break;
                case "KIS_ccCert": strRealData = "-----BEGIN CERTIFICATE-----MIIBoDCCAUigAwIBAgIIBKmE1b6N3gEwCQYHKoZIzj0EATAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMCAXDTA5MDgyODIxMzQxOVoYDzIxMDAxMjMxMjM1OTU5WjAzMQ8wDQYDVQQLDAZPTlNUQVIxDzANBgNVBAoMBk9OU1RBUjEPMA0GA1UEAwwGT05TVEFSMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEGyWwOBIV2SANMgVvdsx2+BhFzx2V6GYiEJzcslnBxdAklt2JIauqJHAVuEDsoeDL8Cp5RMT7Q/bq+hSOtIYc26NFMEMwDgYDVR0PAQH/BAQDAgEGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFLBx/CGT0Vg8XBLeR2+QskMMglxQMAkGByqGSM49BAEDRwAwRAIhANZ0Kb9mR8jvYde8ZaduCJhez180rb10PCVIkcrgy6whAh8Ud5oUeN8EFTkfjX9+HQdZSk5i3G3r8eB/jOp0rNLH-----END CERTIFICATE-----"; break;
                case "KIS_vCert": strRealData = "-----BEGIN CERTIFICATE-----MIIBwDCCAWagAwIBAgIIBP2nrQVaQgEwCQYHKoZIzj0EATA8MRIwEAYDVQQKDAlVVl9PTlNUQVIxEjAQBgNVBAsMCVVWX09OU1RBUjESMBAGA1UEAwwJVVZfT05TVEFSMCAXDTEyMDYxNDIzNTkxMloYDzIxMDAxMjMxMjM1OTU5WjApMQ8wDQYDVQQKDAZPblN0YXIxFjAUBgNVBAMMDVNUSUQgNjUzMzAwNTgwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAAT4bbZe0BKuzGiwgPiWQcfLpfDAgaatxiD2StpsHSwUxK0e6rVEAjeyqf1hNf8L/j83uzTOuLwGwItpEsHLw9Fpo2QwYjAOBgNVHQ8BAf8EBAMCA4gwIAYDVR0lAQH/BBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMIMA8GA1UdEwEB/wQFMAMBAQAwHQYDVR0OBBYEFIGARMVUY5EblVBB/wCPPhktuEwZMAkGByqGSM49BAEDSQAwRgIhAJyo6B1EJ2ffLY0FtfmKeGBIW9Zb+QXCXhfteiQCFpZ8AiEAzjp3V7zci7sZafpaifNpRuWVKGgTHlqoe+SDcZ1hFEQ=-----END CERTIFICATE-----";
                    break;
                case "KIS_vPri": strRealData = "-----BEGIN PRIVATE KEY-----MEECAQAwEwYHKoZIzj0CAQYIKoZIzj0DAQcEJzAlAgEBBCDK1dhrd9bnpcg/GSua65VsxyS44PkPecgaXOwW/ohG0A==-----END PRIVATE KEY-----";
                    break;
                case "KIS_vPre": strRealData = "-----BEGIN PRE-SHARED KEY DATA-----l1kTC9OeZBSiex2GtLTQQHmw/vzcRLJNVaC17gUxQE9OCJPLJb91I6xk7/yx8oyFh2oB3PyLR61akbSAhMmLTw==-----END PRE-SHARED KEY DATA----------BEGIN PSK IDENTIFIER DATA-----117605004-----END PSK IDENTIFIER DATA----------BEGIN PSK HINT-----OnStar PoC-----END PSK HINT----------BEGIN PSK ID-----1-----END PSK ID-----"; break;
                case "KIS_vAuth": strRealData = "C1EF5AED686E6BE5"; break;
                case "KIS_vHash": strRealData = "ab73c80c8e9dfcd123d35ee32a4f197e218d6e9a"; break;
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

                    STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountLength     = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize  = strRealData.Length;
                    STEPMANAGER_VALUE.iUploadBytesSendCount = 0;
                    STEPMANAGER_VALUE.LstGen9CertFile = new List<byte>();

                    byte[] bAscFileSize = Encoding.UTF8.GetBytes(strRealData.Length.ToString() + " "); //파라미터간의 Space 바이트 
                    byte[] bAscFilePath = Encoding.UTF8.GetBytes(strTempSpl[0].ToString() + " ");      //파라미터간의 Space 바이트 
                    byte[] bAscFileName = Encoding.UTF8.GetBytes(strTempSpl[1].ToString());
                    iFuncBytes = bAscFileSize.Length + bAscFilePath.Length + bAscFileName.Length;
                    bReturnBytes = new byte[iFuncBytes];

                    STEPMANAGER_VALUE.LstGen9CertFile.AddRange(bRealData);

                    Array.Copy(bAscFileSize, 0, bReturnBytes, iByteIndex, bAscFileSize.Length); iByteIndex = bAscFileSize.Length;
                    Array.Copy(bAscFilePath, 0, bReturnBytes, iByteIndex, bAscFilePath.Length); iByteIndex += bAscFilePath.Length;
                    Array.Copy(bAscFileName, 0, bReturnBytes, iByteIndex, bAscFileName.Length);
                    bRes = true;
                    break;

                case (int)EFILETYPE.TRANSFER:
                    /*
                    iFuncBytes = bFileCount.Length + bRealData.Length;
                    bReturnBytes = new byte[iFuncBytes];

                    Array.Copy(bFileCount, 0, bReturnBytes, iByteIndex, bFileCount.Length); iByteIndex = bFileCount.Length;
                    Array.Copy(bRealData, 0, bReturnBytes, iByteIndex, bRealData.Length);
                    bRes = true;
                    */
                    //--- Gen9 


                    if (STEPMANAGER_VALUE.iUploadBytesCountStartIndex >= STEPMANAGER_VALUE.iUploadBytesCountTotalSize)
                    {
                        strReason = "Already Transfer Complete";
                        bRes = true;
                        return bReturnBytes;
                    }
                    else
                    {
                        try
                        {
                            if (STEPMANAGER_VALUE.iUploadBytesCountTotalSize >= 512 + STEPMANAGER_VALUE.iUploadBytesCountStartIndex)
                            {
                                STEPMANAGER_VALUE.iUploadBytesCountLength = 512;
                            }
                            else
                            {
                                STEPMANAGER_VALUE.iUploadBytesCountLength =
                                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize - STEPMANAGER_VALUE.iUploadBytesCountStartIndex;
                            }

                            byte[] bTempBinary = new byte[STEPMANAGER_VALUE.LstGen9CertFile.Count];
                            byte[] bBinaryFile = new byte[STEPMANAGER_VALUE.iUploadBytesCountLength];

                            bTempBinary = STEPMANAGER_VALUE.LstGen9CertFile.ToArray();

                            Array.Copy(bTempBinary, STEPMANAGER_VALUE.iUploadBytesCountStartIndex, bBinaryFile, 0, STEPMANAGER_VALUE.iUploadBytesCountLength);


                            bFileCount[3] = (byte)STEPMANAGER_VALUE.iUploadBytesSendCount;
                            

                            iFuncBytes = bFileCount.Length + bBinaryFile.Length;
                            bReturnBytes = new byte[iFuncBytes];

                            Array.Copy(bFileCount, 0, bReturnBytes, iByteIndex, bFileCount.Length); iByteIndex = bFileCount.Length;
                            Array.Copy(bBinaryFile, 0, bReturnBytes, iByteIndex, bBinaryFile.Length);
                            bRes = true;

                            if (bCallClass) //TX로 보낼때만 증가시켜야한다... RX때도 함수를 호출하기 때문에 이놈이 증가되버린다....
                            {
                                STEPMANAGER_VALUE.iUploadBytesSendCount++;
                                STEPMANAGER_VALUE.iUploadBytesCountStartIndex += STEPMANAGER_VALUE.iUploadBytesCountLength;
                            }

                        }
                        catch 
                        {
                            strReason = "Gen9 Make EFILE2 Error";
                            bRes = false;
                            return bReturnBytes;
                        }

                    }

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
            byte[] bFileCount = new byte[4];   //현재는 멀티로 전송하는것이 구현이 안되었으므로 한번만 보냄.            
            byte[] bRealData = new byte[512];  //GEN9 는 512가 MAX 이다 ㅅㅂ
            //------------------

            int iFuncBytes = 10;
            
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

            string strProgramPath = AppDomain.CurrentDomain.BaseDirectory + "GEN9\\" + strTempSpl[2];
            int iLocalFizeSize = 0;
            //[3] DATA FILE CHECK     
            switch (iEfileIdx)
            {
                case (int)EFILETYPE.CHECK:
                    STEPMANAGER_VALUE.iUploadBytesCountStartIndex = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountLength = 0;
                    STEPMANAGER_VALUE.iUploadBytesCountTotalSize = 0;
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

                    Array.Reverse(bFileCount);  //젠장, gen9는 big endian 이다.

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
                        if (STEPMANAGER_VALUE.iUploadBytesCountTotalSize >= 512 + STEPMANAGER_VALUE.iUploadBytesCountStartIndex)
                        {
                            STEPMANAGER_VALUE.iUploadBytesCountLength = 512;
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
