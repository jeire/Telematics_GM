using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GmTelematics
{
    class DK_OOB
    {
        private string[] strLabelData;
        private string[] strMctmLabelData;
      
        public DK_OOB() 
        {
            strLabelData = strMctmLabelData = null;

        }
        
        public bool LoadingLabelData(string strOOBLabel, ref string strReason)
        {
            //TCP 샘플 바코드
            //[)>06Y7520400000000XP8407413012V555343750T4116008000000002S512VIEY192932
            //14Z11760508216Z014618000000425517Z8901170227104422623018Z3101701044226232DD010811PTWG10ANEB

            //GEN10 소장님 패턴. & 
            //[)>06Y~~~~~~~~~~~~~~P~~~~~~~~12V~~~~~~~~~T~~~~~~~~~~~~~~~~S~~~~~~~~~~~~~14Z~~~~~~~~~16Z~~~~~~~~~~~~~~~17Z~~~~~~~~~~~~~~~~~~~~18Z~~~~~~~~~~~~~~~2D~~~~~~1P~~~~~~~~~

            //GEN10 쿤산 샘플 라벨.
            //[)>06Y7520400000000XP8402147112V544948685T1116131000007437S605KSVZ50261514Z10654912916Z35294007033156717Z898600D609163001899018Z4600796600639902D0510161PTC10A3FUC7
            //[        0400000000XP8429832812V555343750T2118317000000569S811VIBB27287714Z16076294716Z35162710544335717Z8901170227115750219518
            //TCP 베트남 샘플라벨
            //[)>06Y7520400000000XP8407413012V555343750T4116009000000307S601VIWP03304114Z11760541916Z01461800000432817Z8901170227105121092918Z3101701051210922D0109161PTWG10ANEB


            //[)>06Y7380000000000XP4257831912V555343750TH118137300006242S805355754

            //GEN11 MY24
            //[)>06Y7520400000000XP8503788712V555343750T5123219000008701S308VIKE03890114Z18529327716Z35866565174192617Z8901170432127398602618Z3101701273986022D0807231PTTA20BNEBN: LOADING OK(LENGTH:12)
            //GEN12
            //[)>06Y7520400000000XP8407413012V555343750T4116009000000307S206VIUQ482926
            //strOOBLabel = "[)>06Y7520400000000XP8407413012V555343750T4116009000000307S206VIUQ482926";

            //2. GM DEFINED VPPS           Y / 14 / 15
            //1. GM PARTNUMBER             P / 8  / 9            
            //3. ASSEMBLY SITE DUNS      12V / 9  / 12
            //4. GM TRACE ID               T / 16 / 17
            //5. GM SN                     S / 13 / 14
            //6. GM STID                 14Z / 9  / 12
            //7. GM IMEI                 16Z / 14 / 17  (문서엔 15Z 라고 되어있음 ㅡㅡ^)
            //8. GM ICCID                 17Z / 6  / 8  (문서엔 2D라고 되어있음
            //9. GM IMSI                  18Z / 10 / 12

            //10. GM DATE               2D 
            //11. GM MODEL              1P
            int iHeader = 5;
            strLabelData  = new string[(int)OOBBARCODE.END];
            int[] iOOBStx = new int[(int)OOBBARCODE.END];
            int[] iOOBLen = new int[(int)OOBBARCODE.END];
            int[] strSub  = new int[(int)OOBBARCODE.END];

            strSub[(int)OOBBARCODE.PARTNUMBER] = 1;
            strSub[(int)OOBBARCODE.VPPS]       = 1;
            strSub[(int)OOBBARCODE.DUNS]       = 3;
            strSub[(int)OOBBARCODE.TRACE]      = 1;
            strSub[(int)OOBBARCODE.SN]         = 1;
            strSub[(int)OOBBARCODE.STID]       = 3;
            strSub[(int)OOBBARCODE.IMEI]       = 3;
            strSub[(int)OOBBARCODE.ICCID]      = 3;
            strSub[(int)OOBBARCODE.IMSI]       = 3;
            strSub[(int)OOBBARCODE.DATE]       = 2;
            strSub[(int)OOBBARCODE.MODEL]      = 2;
 
            iOOBLen[(int)OOBBARCODE.VPPS]       = 15;
            iOOBLen[(int)OOBBARCODE.PARTNUMBER] = 9;
            iOOBLen[(int)OOBBARCODE.DUNS]       = 12;
            iOOBLen[(int)OOBBARCODE.TRACE]      = 17;
            iOOBLen[(int)OOBBARCODE.SN]         = 14;
            iOOBLen[(int)OOBBARCODE.IMEI]       = 18; 
            iOOBLen[(int)OOBBARCODE.ICCID]      = 23;
            iOOBLen[(int)OOBBARCODE.IMSI]       = 18;
            iOOBLen[(int)OOBBARCODE.STID]       = 12;            
            iOOBLen[(int)OOBBARCODE.DATE]       = 8;
            iOOBLen[(int)OOBBARCODE.MODEL]      = 11; // 11자리 이상될수 있음...젠장.

            int iAllSize = 0;

            for (int i = 0; i < (int)OOBBARCODE.END; i++)
            {
                iAllSize += iOOBLen[i];
            }

            if (strOOBLabel.Length < iAllSize)
            {
                strReason = "LOADING ERROR(NO LABEL)";
                return false;
            }

            iOOBStx[(int)OOBBARCODE.VPPS]       = iHeader;
            iOOBStx[(int)OOBBARCODE.PARTNUMBER] = iOOBStx[(int)OOBBARCODE.VPPS]         + iOOBLen[(int)OOBBARCODE.VPPS];
            iOOBStx[(int)OOBBARCODE.DUNS]       = iOOBStx[(int)OOBBARCODE.PARTNUMBER]   + iOOBLen[(int)OOBBARCODE.PARTNUMBER];
            iOOBStx[(int)OOBBARCODE.TRACE]      = iOOBStx[(int)OOBBARCODE.DUNS]         + iOOBLen[(int)OOBBARCODE.DUNS];
            iOOBStx[(int)OOBBARCODE.SN]         = iOOBStx[(int)OOBBARCODE.TRACE]        + iOOBLen[(int)OOBBARCODE.TRACE];
            iOOBStx[(int)OOBBARCODE.STID]       = iOOBStx[(int)OOBBARCODE.SN]           + iOOBLen[(int)OOBBARCODE.SN];
            iOOBStx[(int)OOBBARCODE.IMEI]       = iOOBStx[(int)OOBBARCODE.STID]         + iOOBLen[(int)OOBBARCODE.STID];
            iOOBStx[(int)OOBBARCODE.ICCID]      = iOOBStx[(int)OOBBARCODE.IMEI]         + iOOBLen[(int)OOBBARCODE.IMEI];
            iOOBStx[(int)OOBBARCODE.IMSI]       = iOOBStx[(int)OOBBARCODE.ICCID]        + iOOBLen[(int)OOBBARCODE.ICCID];
            iOOBStx[(int)OOBBARCODE.DATE]       = iOOBStx[(int)OOBBARCODE.IMSI] + iOOBLen[(int)OOBBARCODE.IMSI];
            iOOBStx[(int)OOBBARCODE.MODEL]      = iOOBStx[(int)OOBBARCODE.DATE] + iOOBLen[(int)OOBBARCODE.DATE];


            //순서 NONE, PARTNUMBER, VPPS, DUNS, TRACE, SN, STID, IMEI, ICCID, IMSI, DATE, MODEL, END
            strLabelData[(int)OOBBARCODE.PARTNUMBER] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.PARTNUMBER], iOOBLen[(int)OOBBARCODE.PARTNUMBER]);
            strLabelData[(int)OOBBARCODE.VPPS]      = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.VPPS],      iOOBLen[(int)OOBBARCODE.VPPS]);        
            strLabelData[(int)OOBBARCODE.DUNS]      = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.DUNS],      iOOBLen[(int)OOBBARCODE.DUNS]);
            strLabelData[(int)OOBBARCODE.TRACE]     = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.TRACE],     iOOBLen[(int)OOBBARCODE.TRACE]);
            strLabelData[(int)OOBBARCODE.SN]        = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.SN],        iOOBLen[(int)OOBBARCODE.SN]);
            strLabelData[(int)OOBBARCODE.STID]      = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.STID],      iOOBLen[(int)OOBBARCODE.STID]);
            strLabelData[(int)OOBBARCODE.IMEI]      = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.IMEI],      iOOBLen[(int)OOBBARCODE.IMEI]);
            strLabelData[(int)OOBBARCODE.ICCID]     = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.ICCID],     iOOBLen[(int)OOBBARCODE.ICCID]);
            strLabelData[(int)OOBBARCODE.IMSI]      = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.IMSI],      iOOBLen[(int)OOBBARCODE.IMSI]);
            strLabelData[(int)OOBBARCODE.DATE]      = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.DATE],      iOOBLen[(int)OOBBARCODE.DATE]);
            strLabelData[(int)OOBBARCODE.MODEL]     = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.MODEL]); //11자리 이상될수도 있으므로. 마지막은 다읽는다.

            strLabelData[(int)OOBBARCODE.PARTNUMBER] = strLabelData[(int)OOBBARCODE.PARTNUMBER].Substring(strSub[(int)OOBBARCODE.PARTNUMBER]);
            strLabelData[(int)OOBBARCODE.VPPS]       = strLabelData[(int)OOBBARCODE.VPPS].Substring(strSub[(int)OOBBARCODE.VPPS]);
            strLabelData[(int)OOBBARCODE.DUNS]       = strLabelData[(int)OOBBARCODE.DUNS].Substring(strSub[(int)OOBBARCODE.DUNS]);
            strLabelData[(int)OOBBARCODE.TRACE]     = strLabelData[(int)OOBBARCODE.TRACE].Substring(strSub[(int)OOBBARCODE.TRACE]);
            strLabelData[(int)OOBBARCODE.SN]        = strLabelData[(int)OOBBARCODE.SN].Substring(strSub[(int)OOBBARCODE.SN]);
            strLabelData[(int)OOBBARCODE.STID]      = strLabelData[(int)OOBBARCODE.STID].Substring(strSub[(int)OOBBARCODE.STID]);
            strLabelData[(int)OOBBARCODE.IMEI]      = strLabelData[(int)OOBBARCODE.IMEI].Substring(strSub[(int)OOBBARCODE.IMEI]);
            strLabelData[(int)OOBBARCODE.ICCID]     = strLabelData[(int)OOBBARCODE.ICCID].Substring(strSub[(int)OOBBARCODE.ICCID]);
            strLabelData[(int)OOBBARCODE.IMSI]      = strLabelData[(int)OOBBARCODE.IMSI].Substring(strSub[(int)OOBBARCODE.IMSI]);
            strLabelData[(int)OOBBARCODE.DATE]      = strLabelData[(int)OOBBARCODE.DATE].Substring(strSub[(int)OOBBARCODE.DATE]);
            strLabelData[(int)OOBBARCODE.MODEL]     = strLabelData[(int)OOBBARCODE.MODEL].Substring(strSub[(int)OOBBARCODE.MODEL]);

            if (strLabelData.Length > 10) //최소 10개항목이므로.
            {
                strReason = "LOADING OK(LENGTH:" + strLabelData.Length.ToString() + ")";
                return true;
            }


            else
            {
                strReason = "LOADING NG(LENGTH:" + strLabelData.Length.ToString() + ")";
                return false;
            }
        }

        public bool LoadingLabelDataGEN12(string strOOBLabel, ref string strReason)
        {
            //TCP 샘플 바코드
            //[)>06Y7520400000000XP8407413012V555343750T4116008000000002S512VIEY192932
            //14Z11760508216Z014618000000425517Z8901170227104422623018Z3101701044226232DD010811PTWG10ANEB

            //GEN10 소장님 패턴. & 
            //[)>06Y~~~~~~~~~~~~~~P~~~~~~~~12V~~~~~~~~~T~~~~~~~~~~~~~~~~S~~~~~~~~~~~~~14Z~~~~~~~~~16Z~~~~~~~~~~~~~~~17Z~~~~~~~~~~~~~~~~~~~~18Z~~~~~~~~~~~~~~~2D~~~~~~1P~~~~~~~~~

            //GEN10 쿤산 샘플 라벨.
            //[)>06Y7520400000000XP8402147112V544948685T1116131000007437S605KSVZ50261514Z10654912916Z35294007033156717Z898600D609163001899018Z4600796600639902D0510161PTC10A3FUC7
            //[        0400000000XP8429832812V555343750T2118317000000569S811VIBB27287714Z16076294716Z35162710544335717Z8901170227115750219518
            //TCP 베트남 샘플라벨
            //[)>06Y7520400000000XP8407413012V555343750T4116009000000307S601VIWP03304114Z11760541916Z01461800000432817Z8901170227105121092918Z3101701051210922D0109161PTWG10ANEB


            //[)>06Y7380000000000XP4257831912V555343750TH118137300006242S805355754

            //GEN12
            //[)>06Y7520400000000XP8407413012V555343750T4116009000000307S206VIUQ482926
            //strOOBLabel = "[)>06Y7520400000000XP8407413012V555343750T4116009000000307S206VIUQ482926";

            //2. GM DEFINED VPPS           Y / 14 / 15
            //1. GM PARTNUMBER             P / 8  / 9            
            //3. ASSEMBLY SITE DUNS      12V / 9  / 12
            //4. GM TRACE ID               T / 16 / 17
            //5. GM SN                     S / 13 / 14
            //6. GM STID                 14Z / 9  / 12
            //7. GM IMEI                 16Z / 14 / 17  (문서엔 15Z 라고 되어있음 ㅡㅡ^)
            //8. GM ICCID                 17Z / 6  / 8  (문서엔 2D라고 되어있음
            //9. GM IMSI                  18Z / 10 / 12

            //10. GM DATE               2D 
            //11. GM MODEL              1P
            int iHeader = 5;
            strLabelData = new string[(int)OOBBARCODE.END];
            int[] iOOBStx = new int[(int)OOBBARCODE.END];
            int[] iOOBLen = new int[(int)OOBBARCODE.END];
            int[] strSub = new int[(int)OOBBARCODE.END];

            strSub[(int)OOBBARCODE.PARTNUMBER] = 1;
            strSub[(int)OOBBARCODE.VPPS] = 1;
            strSub[(int)OOBBARCODE.DUNS] = 3;
            strSub[(int)OOBBARCODE.TRACE] = 1;
            strSub[(int)OOBBARCODE.SN] = 1;
        

            iOOBLen[(int)OOBBARCODE.VPPS] = 15;
            iOOBLen[(int)OOBBARCODE.PARTNUMBER] = 9;
            iOOBLen[(int)OOBBARCODE.DUNS] = 12;
            iOOBLen[(int)OOBBARCODE.TRACE] = 17;
            iOOBLen[(int)OOBBARCODE.SN] = 14;
  

            int iAllSize = 0;

            for (int i = 0; i < (int)OOBBARCODE.END; i++)
            {
                iAllSize += iOOBLen[i];
            }

            if (strOOBLabel.Length < iAllSize)
            {
                strReason = "LOADING ERROR(NO LABEL)";
                return false;
            }

            iOOBStx[(int)OOBBARCODE.VPPS] = iHeader;
            iOOBStx[(int)OOBBARCODE.PARTNUMBER] = iOOBStx[(int)OOBBARCODE.VPPS] + iOOBLen[(int)OOBBARCODE.VPPS];
            iOOBStx[(int)OOBBARCODE.DUNS] = iOOBStx[(int)OOBBARCODE.PARTNUMBER] + iOOBLen[(int)OOBBARCODE.PARTNUMBER];
            iOOBStx[(int)OOBBARCODE.TRACE] = iOOBStx[(int)OOBBARCODE.DUNS] + iOOBLen[(int)OOBBARCODE.DUNS];
            iOOBStx[(int)OOBBARCODE.SN] = iOOBStx[(int)OOBBARCODE.TRACE] + iOOBLen[(int)OOBBARCODE.TRACE];
  

            //순서 NONE, PARTNUMBER, VPPS, DUNS, TRACE, SN, END
            strLabelData[(int)OOBBARCODE.PARTNUMBER] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.PARTNUMBER], iOOBLen[(int)OOBBARCODE.PARTNUMBER]);
            strLabelData[(int)OOBBARCODE.VPPS] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.VPPS], iOOBLen[(int)OOBBARCODE.VPPS]);
            strLabelData[(int)OOBBARCODE.DUNS] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.DUNS], iOOBLen[(int)OOBBARCODE.DUNS]);
            strLabelData[(int)OOBBARCODE.TRACE] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.TRACE], iOOBLen[(int)OOBBARCODE.TRACE]);
            strLabelData[(int)OOBBARCODE.SN] = strOOBLabel.Substring(iOOBStx[(int)OOBBARCODE.SN], iOOBLen[(int)OOBBARCODE.SN]);
         
            strLabelData[(int)OOBBARCODE.PARTNUMBER] = strLabelData[(int)OOBBARCODE.PARTNUMBER].Substring(strSub[(int)OOBBARCODE.PARTNUMBER]);
            strLabelData[(int)OOBBARCODE.VPPS] = strLabelData[(int)OOBBARCODE.VPPS].Substring(strSub[(int)OOBBARCODE.VPPS]);
            strLabelData[(int)OOBBARCODE.DUNS] = strLabelData[(int)OOBBARCODE.DUNS].Substring(strSub[(int)OOBBARCODE.DUNS]);
            strLabelData[(int)OOBBARCODE.TRACE] = strLabelData[(int)OOBBARCODE.TRACE].Substring(strSub[(int)OOBBARCODE.TRACE]);
            strLabelData[(int)OOBBARCODE.SN] = strLabelData[(int)OOBBARCODE.SN].Substring(strSub[(int)OOBBARCODE.SN]);
          
            if (strLabelData.Length > 4) //최소 5개항목이므로.
            {
                strReason = "LOADING OK(LENGTH:" + strLabelData.Length.ToString() + ")";
                return true;
            }
            else
            {
                strReason = "LOADING NG(LENGTH:" + strLabelData.Length.ToString() + ")";
                return false;
            }
        }

        public bool LoadingLabelDataMCTM(string strOOBLabel, ref string strReason)
        {
            //MCTM 샘플 바코드
            //[)>06
            //   Y7520400000000X     (VPPS)
            //   P84492806           (GMPN)
            //   12V555343750        (DUNS)
            //   T7117355000002212   (TRACE)
            //   16Z990009610032172  (VZWIMEI)
            //   18Z311480996621219  (VZWIMSI)
            //   17Z89148000003949250661 (VZWICCID)
            //   19Z014977000032170      (ATNTIMEI)
            //   21Z310170113484583      (ATNTIMSI)
            //   20Z89011702271134845832 (ATNTICCID)
            //   22Z014978000032178      (TMUSIMEI)
            //   24Z310260986705064      (TMUSIMSI)
            //   23Z8901260982767050649  (TMUSICCID) 
            //   2D122117                (ASSMDATE)
            //   1PTCAA19ANANN           (LGPN)
            //   S712VIBB474669          (LGSN)

            //변경된 라벨? 2018.02.01
            //[)>06Y7520400000000XP8451203812V555343750T711802600000122316Z99000961003379018Z31148099662116617Z8914800000394924910119Z01497700003379821Y31017011348442620Y8901170227113484426422Y01497800003379624Z31026098670490623Z89012609827670490622D0126181PTCAA19ANANNS801VIEY545932
            int iHeader = 5;
            strMctmLabelData = new string[(int)MCTMBARCODE.END];
            int[] iOOBStx    = new int[(int)MCTMBARCODE.END];
            int[] iOOBLen    = new int[(int)MCTMBARCODE.END];
            int[] strSub     = new int[(int)MCTMBARCODE.END];
            string[] strStx = new string[(int)MCTMBARCODE.END];

            strStx[(int)MCTMBARCODE.VPPS] = "Y";
            strStx[(int)MCTMBARCODE.GMPN] = "P";
            strStx[(int)MCTMBARCODE.DUNS] = "12V";
            strStx[(int)MCTMBARCODE.TRACE] = "T";
            strStx[(int)MCTMBARCODE.VZWIMEI] = "16Z";
            strStx[(int)MCTMBARCODE.VZWIMSI] = "18Z";
            strStx[(int)MCTMBARCODE.VZWICCID] = "17Z";
            strStx[(int)MCTMBARCODE.ATNTIMEI] = "19Z";
            strStx[(int)MCTMBARCODE.ATNTIMSI] = "21Y";   //21Z 에서 21Y 변경. 2018.2.1
            strStx[(int)MCTMBARCODE.ATNTICCID] = "20Y";  //20Z 에서 20Y 변경. 2018.2.1
            strStx[(int)MCTMBARCODE.TMUSIMEI] = "22Y";   //22Z 에서 20Y 변경. 2018.2.1
            strStx[(int)MCTMBARCODE.TMUSIMSI] = "24Z";
            strStx[(int)MCTMBARCODE.TMUSICCID] = "23Z";
            strStx[(int)MCTMBARCODE.ASSMDATE] = "2D";
            strStx[(int)MCTMBARCODE.LGPN] = "1P";
            strStx[(int)MCTMBARCODE.LGSN] = "S";

            strSub[(int)MCTMBARCODE.VPPS] = 1;
            strSub[(int)MCTMBARCODE.GMPN] = 1;
            strSub[(int)MCTMBARCODE.DUNS] = 3;
            strSub[(int)MCTMBARCODE.TRACE] = 1;
            strSub[(int)MCTMBARCODE.VZWIMEI] = 3;
            strSub[(int)MCTMBARCODE.VZWIMSI] = 3;
            strSub[(int)MCTMBARCODE.VZWICCID] = 3;
            strSub[(int)MCTMBARCODE.ATNTIMEI] = 3;
            strSub[(int)MCTMBARCODE.ATNTIMSI] = 3;
            strSub[(int)MCTMBARCODE.ATNTICCID] = 3;
            strSub[(int)MCTMBARCODE.TMUSIMEI] = 3;
            strSub[(int)MCTMBARCODE.TMUSIMSI] = 3;
            strSub[(int)MCTMBARCODE.TMUSICCID] = 3;
            strSub[(int)MCTMBARCODE.ASSMDATE] = 2;
            strSub[(int)MCTMBARCODE.LGPN] = 2;
            strSub[(int)MCTMBARCODE.LGSN] = 1;

            iOOBLen[(int)MCTMBARCODE.VPPS] = 14         + strSub[(int)MCTMBARCODE.VPPS];
            iOOBLen[(int)MCTMBARCODE.GMPN] = 8 + strSub[(int)MCTMBARCODE.GMPN];
            iOOBLen[(int)MCTMBARCODE.DUNS] = 9 + strSub[(int)MCTMBARCODE.DUNS];
            iOOBLen[(int)MCTMBARCODE.TRACE] = 16 + strSub[(int)MCTMBARCODE.TRACE];
            iOOBLen[(int)MCTMBARCODE.VZWIMEI] = 15 + strSub[(int)MCTMBARCODE.VZWIMEI];
            iOOBLen[(int)MCTMBARCODE.VZWIMSI] = 15 + strSub[(int)MCTMBARCODE.VZWIMSI];
            iOOBLen[(int)MCTMBARCODE.VZWICCID] = 20 + strSub[(int)MCTMBARCODE.VZWICCID];
            iOOBLen[(int)MCTMBARCODE.ATNTIMEI] = 15 + strSub[(int)MCTMBARCODE.ATNTIMEI];
            iOOBLen[(int)MCTMBARCODE.ATNTIMSI] = 15 + strSub[(int)MCTMBARCODE.ATNTIMSI];
            iOOBLen[(int)MCTMBARCODE.ATNTICCID] = 20 + strSub[(int)MCTMBARCODE.ATNTICCID];
            iOOBLen[(int)MCTMBARCODE.TMUSIMEI] = 15 + strSub[(int)MCTMBARCODE.TMUSIMEI];
            iOOBLen[(int)MCTMBARCODE.TMUSIMSI] = 15 + strSub[(int)MCTMBARCODE.TMUSIMSI];
            iOOBLen[(int)MCTMBARCODE.TMUSICCID] = 19 + strSub[(int)MCTMBARCODE.TMUSICCID];
            iOOBLen[(int)MCTMBARCODE.ASSMDATE] = 6 + strSub[(int)MCTMBARCODE.ASSMDATE];
            iOOBLen[(int)MCTMBARCODE.LGPN] = 11 + strSub[(int)MCTMBARCODE.LGPN];
            iOOBLen[(int)MCTMBARCODE.LGSN] = 14 + strSub[(int)MCTMBARCODE.LGSN]; 

            int iAllSize = 0;

            for (int i = 0; i < (int)MCTMBARCODE.END; i++)
            {
                iAllSize += iOOBLen[i];
            }

            if (strOOBLabel.Length < iAllSize)
            {
                strReason = "LOADING ERROR(NO LABEL)";
                return false;
            }


            iOOBStx[(int)MCTMBARCODE.VPPS]      = iHeader;
            iOOBStx[(int)MCTMBARCODE.GMPN]      = iOOBStx[(int)MCTMBARCODE.VPPS]     + iOOBLen[(int)MCTMBARCODE.VPPS];
            iOOBStx[(int)MCTMBARCODE.DUNS]      = iOOBStx[(int)MCTMBARCODE.GMPN]     + iOOBLen[(int)MCTMBARCODE.GMPN];
            iOOBStx[(int)MCTMBARCODE.TRACE]     = iOOBStx[(int)MCTMBARCODE.DUNS]     + iOOBLen[(int)MCTMBARCODE.DUNS];
            iOOBStx[(int)MCTMBARCODE.VZWIMEI]   = iOOBStx[(int)MCTMBARCODE.TRACE]    + iOOBLen[(int)MCTMBARCODE.TRACE];
            iOOBStx[(int)MCTMBARCODE.VZWIMSI]   = iOOBStx[(int)MCTMBARCODE.VZWIMEI]  + iOOBLen[(int)MCTMBARCODE.VZWIMEI];
            iOOBStx[(int)MCTMBARCODE.VZWICCID]  = iOOBStx[(int)MCTMBARCODE.VZWIMSI] + iOOBLen[(int)MCTMBARCODE.VZWIMSI];
            iOOBStx[(int)MCTMBARCODE.ATNTIMEI]  = iOOBStx[(int)MCTMBARCODE.VZWICCID] + iOOBLen[(int)MCTMBARCODE.VZWICCID];
            iOOBStx[(int)MCTMBARCODE.ATNTIMSI]  = iOOBStx[(int)MCTMBARCODE.ATNTIMEI] + iOOBLen[(int)MCTMBARCODE.ATNTIMEI];
            iOOBStx[(int)MCTMBARCODE.ATNTICCID] = iOOBStx[(int)MCTMBARCODE.ATNTIMSI] + iOOBLen[(int)MCTMBARCODE.ATNTIMSI];
            iOOBStx[(int)MCTMBARCODE.TMUSIMEI]  = iOOBStx[(int)MCTMBARCODE.ATNTICCID] + iOOBLen[(int)MCTMBARCODE.ATNTICCID];
            iOOBStx[(int)MCTMBARCODE.TMUSIMSI]  = iOOBStx[(int)MCTMBARCODE.TMUSIMEI] + iOOBLen[(int)MCTMBARCODE.TMUSIMEI];
            iOOBStx[(int)MCTMBARCODE.TMUSICCID] = iOOBStx[(int)MCTMBARCODE.TMUSIMSI] + iOOBLen[(int)MCTMBARCODE.TMUSIMSI];
            iOOBStx[(int)MCTMBARCODE.ASSMDATE]  = iOOBStx[(int)MCTMBARCODE.TMUSICCID] + iOOBLen[(int)MCTMBARCODE.TMUSICCID];
            iOOBStx[(int)MCTMBARCODE.LGPN]      = iOOBStx[(int)MCTMBARCODE.ASSMDATE] + iOOBLen[(int)MCTMBARCODE.ASSMDATE];
            iOOBStx[(int)MCTMBARCODE.LGSN]      = iOOBStx[(int)MCTMBARCODE.LGPN]     + iOOBLen[(int)MCTMBARCODE.LGPN];
                     
            strMctmLabelData[(int)MCTMBARCODE.VPPS]     = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.VPPS],     iOOBLen[(int)MCTMBARCODE.VPPS]);
            strMctmLabelData[(int)MCTMBARCODE.GMPN]     = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.GMPN],     iOOBLen[(int)MCTMBARCODE.GMPN]);
            strMctmLabelData[(int)MCTMBARCODE.DUNS]     = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.DUNS],     iOOBLen[(int)MCTMBARCODE.DUNS]);
            strMctmLabelData[(int)MCTMBARCODE.TRACE]    = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.TRACE],    iOOBLen[(int)MCTMBARCODE.TRACE]);
            strMctmLabelData[(int)MCTMBARCODE.VZWIMEI]  = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.VZWIMEI],  iOOBLen[(int)MCTMBARCODE.VZWIMEI]);
            strMctmLabelData[(int)MCTMBARCODE.VZWIMSI]  = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.VZWIMSI],  iOOBLen[(int)MCTMBARCODE.VZWIMSI]);
            strMctmLabelData[(int)MCTMBARCODE.VZWICCID] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.VZWICCID], iOOBLen[(int)MCTMBARCODE.VZWICCID]);
            strMctmLabelData[(int)MCTMBARCODE.ATNTIMEI] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.ATNTIMEI], iOOBLen[(int)MCTMBARCODE.ATNTIMEI]);
            strMctmLabelData[(int)MCTMBARCODE.ATNTIMSI] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.ATNTIMSI], iOOBLen[(int)MCTMBARCODE.ATNTIMSI]);
            strMctmLabelData[(int)MCTMBARCODE.ATNTICCID] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.ATNTICCID], iOOBLen[(int)MCTMBARCODE.ATNTICCID]);
            strMctmLabelData[(int)MCTMBARCODE.TMUSIMEI] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.TMUSIMEI], iOOBLen[(int)MCTMBARCODE.TMUSIMEI]);
            strMctmLabelData[(int)MCTMBARCODE.TMUSIMSI] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.TMUSIMSI], iOOBLen[(int)MCTMBARCODE.TMUSIMSI]);
            strMctmLabelData[(int)MCTMBARCODE.TMUSICCID] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.TMUSICCID], iOOBLen[(int)MCTMBARCODE.TMUSICCID]);
            strMctmLabelData[(int)MCTMBARCODE.ASSMDATE] = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.ASSMDATE], iOOBLen[(int)MCTMBARCODE.ASSMDATE]);
            strMctmLabelData[(int)MCTMBARCODE.LGPN]     = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.LGPN],     iOOBLen[(int)MCTMBARCODE.LGPN]);
            strMctmLabelData[(int)MCTMBARCODE.LGSN]     = strOOBLabel.Substring(iOOBStx[(int)MCTMBARCODE.LGSN]);

            for (int i = (int)MCTMBARCODE.VPPS; i < (int)MCTMBARCODE.END; i++)
            {
                if (!strMctmLabelData[i].IndexOf(strStx[i], 0).Equals(0))
                {
                    strReason = "LABEL FORMAT ERROR:" + (i + MCTMBARCODE.NONE).ToString() + "-" + strMctmLabelData[i];
                    return false;
                }
            } 

            strMctmLabelData[(int)MCTMBARCODE.VPPS]      = strMctmLabelData[(int)MCTMBARCODE.VPPS].Substring(strSub[(int)MCTMBARCODE.VPPS]);
            strMctmLabelData[(int)MCTMBARCODE.GMPN]      = strMctmLabelData[(int)MCTMBARCODE.GMPN].Substring(strSub[(int)MCTMBARCODE.GMPN]);
            strMctmLabelData[(int)MCTMBARCODE.DUNS]      = strMctmLabelData[(int)MCTMBARCODE.DUNS].Substring(strSub[(int)MCTMBARCODE.DUNS]);
            strMctmLabelData[(int)MCTMBARCODE.TRACE]     = strMctmLabelData[(int)MCTMBARCODE.TRACE].Substring(strSub[(int)MCTMBARCODE.TRACE]);
            strMctmLabelData[(int)MCTMBARCODE.VZWIMEI]   = strMctmLabelData[(int)MCTMBARCODE.VZWIMEI].Substring(strSub[(int)MCTMBARCODE.VZWIMEI]);
            strMctmLabelData[(int)MCTMBARCODE.VZWIMSI]   = strMctmLabelData[(int)MCTMBARCODE.VZWIMSI].Substring(strSub[(int)MCTMBARCODE.VZWIMSI]);
            strMctmLabelData[(int)MCTMBARCODE.VZWICCID]  = strMctmLabelData[(int)MCTMBARCODE.VZWICCID].Substring(strSub[(int)MCTMBARCODE.VZWICCID]);
            strMctmLabelData[(int)MCTMBARCODE.ATNTIMEI]  = strMctmLabelData[(int)MCTMBARCODE.ATNTIMEI].Substring(strSub[(int)MCTMBARCODE.ATNTIMEI]);
            strMctmLabelData[(int)MCTMBARCODE.ATNTIMSI]  = strMctmLabelData[(int)MCTMBARCODE.ATNTIMSI].Substring(strSub[(int)MCTMBARCODE.ATNTIMSI]);
            strMctmLabelData[(int)MCTMBARCODE.ATNTICCID] = strMctmLabelData[(int)MCTMBARCODE.ATNTICCID].Substring(strSub[(int)MCTMBARCODE.ATNTICCID]);
            strMctmLabelData[(int)MCTMBARCODE.TMUSIMEI]  = strMctmLabelData[(int)MCTMBARCODE.TMUSIMEI].Substring(strSub[(int)MCTMBARCODE.TMUSIMEI]);
            strMctmLabelData[(int)MCTMBARCODE.TMUSIMSI]  = strMctmLabelData[(int)MCTMBARCODE.TMUSIMSI].Substring(strSub[(int)MCTMBARCODE.TMUSIMSI]);
            strMctmLabelData[(int)MCTMBARCODE.TMUSICCID] = strMctmLabelData[(int)MCTMBARCODE.TMUSICCID].Substring(strSub[(int)MCTMBARCODE.TMUSICCID]);
            strMctmLabelData[(int)MCTMBARCODE.ASSMDATE]  = strMctmLabelData[(int)MCTMBARCODE.ASSMDATE].Substring(strSub[(int)MCTMBARCODE.ASSMDATE]);
            strMctmLabelData[(int)MCTMBARCODE.LGPN]      = strMctmLabelData[(int)MCTMBARCODE.LGPN].Substring(strSub[(int)MCTMBARCODE.LGPN]);
            strMctmLabelData[(int)MCTMBARCODE.LGSN]      = strMctmLabelData[(int)MCTMBARCODE.LGSN].Substring(strSub[(int)MCTMBARCODE.LGSN]);


            bool bNullChk = true;

            for (int i = (int)MCTMBARCODE.VPPS; i < (int)MCTMBARCODE.END; i++)
            {
                if (String.IsNullOrEmpty(strMctmLabelData[i]))
                {
                    bNullChk = false; break;
                }
            }

            if (bNullChk) 
            {
                strReason = "LOADING OK(LENGTH:" + ((int)MCTMBARCODE.END - 1).ToString() + ")";
                return true;
            }
            else
            {
                strReason = "LOADING NG(LABEL FORMAT ERROR)";
                return false;
            }
        }

        public bool GetLabelFieldString(string strPar1, ref string strRes)
        {
            int iDx = 0;
            if (strPar1 == null || strPar1.Length < 1)
            {
                strRes =  "NO PAR1";
                return false;
            }

            try
            {
                iDx = int.Parse(strPar1);
            }
            catch 
            {
                strRes = "ERROR PAR1";
                return false;
            }

            if (strLabelData == null || strLabelData.Length < 1)
            {
                strRes = "NO DATA";
                return false;
            }

            else
            {
                try
                {
                    if (strLabelData[iDx].Length > 1)
                    {
                        strRes = strLabelData[iDx];
                        return true;
                    }
                    else
                    {
                        strRes = "NO DATA";
                        return false;
                    }
                }
                catch
                {
                    strRes = "NO DATA";
                    return false;
                }
                               
            }

        }

        public bool GetLabelFieldHexa(string strPar1, ref string strRes)
        {
            int iDx = 0;
            if (strPar1 == null || strPar1.Length < 1)
            {
                strRes = "NO PAR1";
                return false;
            }

            try
            {
                iDx = int.Parse(strPar1);
            }
            catch
            {
                strRes = "ERROR PAR1";
                return false;
            }

            if (strLabelData == null || strLabelData.Length < 1)
            {
                strRes = "NO DATA";
                return false;
            }

            else
            {
                if (strLabelData[iDx].Length > 1)
                {
                    strRes = strLabelData[iDx];

                    try
                    {
                        int iDec = int.Parse(strRes);
                        byte[] bHex = BitConverter.GetBytes(iDec);
                        strRes = "";
                        for (int i = bHex.Length - 1; i >= 0; i--)
                        {
                            strRes += bHex[i].ToString("X2");
                        }
                    }
                    catch
                    {
                        strRes = "NO DECIMAL";
                        return false;
                    }


                    return true;
                }
                else
                {
                    strRes = "NO DATA";
                    return false;
                }
            }

        }

        public bool GetMctmFieldString(string strPar1, ref string strRes)
        {
            int iDx = 0;
            if (strPar1 == null || strPar1.Length < 1)
            {
                strRes = "NO PAR1";
                return false;
            }

            try
            {
                iDx = int.Parse(strPar1);
            }
            catch
            {
                strRes = "ERROR PAR1";
                return false;
            }

            if (strMctmLabelData == null || strMctmLabelData.Length < 1)
            {
                strRes = "NO DATA";
                return false;
            }

            else
            {
                try
                {
                    if (strMctmLabelData[iDx].Length > 1)
                    {
                        strRes = strMctmLabelData[iDx];
                        return true;
                    }
                    else
                    {
                        strRes = "NO DATA";
                        return false;
                    }
                }
                catch
                {
                    strRes = "NO DATA";
                    return false;
                }

            }

        }

        public bool GetMctmFieldHexa(string strPar1, ref string strRes)
        {
            int iDx = 0;
            if (strPar1 == null || strPar1.Length < 1)
            {
                strRes = "NO PAR1";
                return false;
            }

            try
            {
                iDx = int.Parse(strPar1);
            }
            catch
            {
                strRes = "ERROR PAR1";
                return false;
            }

            if (strMctmLabelData == null || strMctmLabelData.Length < 1)
            {
                strRes = "NO DATA";
                return false;
            }

            else
            {
                if (strMctmLabelData[iDx].Length > 1)
                {
                    strRes = strMctmLabelData[iDx];

                    try
                    {
                        int iDec = int.Parse(strRes);
                        byte[] bHex = BitConverter.GetBytes(iDec);
                        strRes = "";
                        for (int i = bHex.Length - 1; i >= 0; i--)
                        {
                            strRes += bHex[i].ToString("X2");
                        }
                    }
                    catch
                    {
                        strRes = "NO DECIMAL";
                        return false;
                    }


                    return true;
                }
                else
                {
                    strRes = "NO DATA";
                    return false;
                }
            }

        }
    }
}
