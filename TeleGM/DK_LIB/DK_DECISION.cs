using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GmTelematics
{
    class DK_DECISION
    {
        public DK_DECISION()
        {

        }
        //비교하여 결과 알려주기.
        public int DecideData(int iComState, string strResFullData, string strData, string strCompareType, string strMin, string strMax, int iOption, string strNgCase, ref string strReplaceData, bool bByPassAscii)
        {
            if (iComState == (int)STATUS.SKIP) 
                return (int)STATUS.SKIP; // SKIP 이면 그냥 SKIP으로 돌려주자.
            if (iOption   == (int)MODE.SEND)   return (int)STATUS.OK;

            switch (iComState)
            {
                case (int)STATUS.OK: return CompareType(strResFullData, strData, strCompareType, strMin, strMax, strNgCase, ref strReplaceData, bByPassAscii);

                case (int)STATUS.NG: return CaseNgStringConfirm((int)STATUS.NG, strNgCase);

                case (int)STATUS.TIMEOUT: return CaseNgStringConfirm((int)STATUS.TIMEOUT, strNgCase); 

                default:  return CaseNgStringConfirm(iComState, strNgCase);
            }

        }

        public bool CheckNoneAscii(string strOrigin, ref string strRemove)
        {
            strOrigin = strOrigin.Replace("\0", String.Empty); //널값 제거
            int i = strOrigin.Length;
            string pattern = "[^ -~]*";
            Regex reg_exp = new Regex(pattern);

            strRemove = reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거
            //return reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거

            if (i != strRemove.Length) return false;
            return true;
        }

        //비교 방식 구분
        private int CompareType(string strFullData, string strData, string strCompareType, string strMin, string strMax, string strNgCase, ref string strReplaceData, bool bBypassAscii)
        {
            int iRtnCode = (int)STATUS.OK;
 
            //비아스키 검사(non aschii 걸러내기) - GMES 올라가면 안된다.
            string strCheckAscii = String.Empty;
            
            if (!CheckNoneAscii(strData, ref strCheckAscii))
            {
                //임시사용 바이어 오디트중. 삭제해야함//////////////////////////
                if(strCompareType.Equals("NONASCII"))
                {
                    strReplaceData = strCheckAscii;
                    strCompareType = "CONTAIN";
                    iRtnCode = CompareContain(strReplaceData, strMin, strMax);
                    if (iRtnCode != (int)STATUS.OK && iRtnCode != (int)STATUS.SKIP)
                    {
                        iRtnCode = CaseNgStringConfirm(iRtnCode, strNgCase);
                        
                    }
                    return iRtnCode;
                }
                ////////////////////////////////////////////////////////

                if (bBypassAscii)
                {
                    strReplaceData = strCheckAscii;                    
                }
                else
                {
                    strReplaceData = strCheckAscii + "(Detect Non Ascii)";
                    iRtnCode = (int)STATUS.NG;
                }
                
            }
            else
            {
                switch (strCompareType)
                {
                    case "NONE":      iRtnCode = (int)STATUS.OK; break;                    
                    case "NUMBER":    iRtnCode = CompareNumber(strData, strMin, strMax, ref strReplaceData); break;
                    case "RESULTCODE": 
                    case "WORD":      iRtnCode = CompareWord(strData, strMax); break;
                    case "TEXT":      iRtnCode = CompareText(strData, strMax); break;
                    case "CONTAIN":   iRtnCode = CompareContain(strData, strMin, strMax); break;
                    case "CONTAIN2":  iRtnCode = CompareContain2(strData, strMin, strMax); break;
                    case "INDEXOF":   iRtnCode = CompareIndexOf(strData, strMin, strMax); break;
                    //case "PARTIAL":   iRtnCode = ComparePartial(strData, strMin, strMax); break;
                    case "DIFFERENT": iRtnCode = CompareDifferent(strData, strMax); break;
                    case "LENGTH":    iRtnCode = CompareLength(strData, strMin, strMax); break;
                    //case "ETC":       iRtnCode = AnalyzeResponseOCU(strFullData, strData, strMin, strMax); break; //미구현
                    case "PATTERN":   iRtnCode = ComparePattern(strData, strMax); break;
                    case "PATTERNS":  iRtnCode = ComparePatterns(strData, strMax); break;
                    case "NOTPATTERN": iRtnCode = CompareNotPattern(strData, strMax); break;
                    case "TRIM":      iRtnCode = CompareTrim(strData, strMin, strMax); break;
                    case "OR":        iRtnCode = CompareOr(strData, strMin, strMax); break;
                    case "EVENONE":   iRtnCode = CompareEvenOne(strData, strMax); break;
                    case "NONASCII":  iRtnCode = CompareEvenOne(strData, strMax); break;

                    case "TIMESTAMP": iRtnCode = (int)STATUS.OK; break;
                    case "UNDERCOVERAGE":  iRtnCode = CompareUnderCoverage(strData, strMax); break;
                    case "UNDERCOVERAGE2": iRtnCode = CompareUnderCoverage2(strData, strMax); break;
                    case "NONECOLON": iRtnCode = CompareNoneColon(strData, strMax); break;


                    

                    default:
                                      iRtnCode = (int)STATUS.NG; break;
                }

                if (iRtnCode != (int)STATUS.OK && iRtnCode != (int)STATUS.SKIP)
                {
                    iRtnCode = CaseNgStringConfirm(iRtnCode, strNgCase);
                }

            }

            return iRtnCode;

        }

        private int CaseNgStringConfirm(int iRes, string strNgcase)
        {
            int iRtnCode = iRes;
            if (iRes == (int)STATUS.ERROR) 
                return iRes; //검사를 진행할 수 없는 에러 처리. user stop 과 같은 개념.

            switch (strNgcase)
            {//절차서 검사단에서 FAIL 일경우 처리.
                case "MONITOR": iRtnCode = (int)STATUS.OK; break;
                case "CHECK":  iRtnCode = (int)STATUS.CHECK; break;
                case "STOP":   iRtnCode = (int)STATUS.NG; break;
                case "EMPTY":  iRtnCode = (int)STATUS.EMPTY; break;
                case "MES":    iRtnCode = (int)STATUS.MESERR; break;
                case "ERROR":  iRtnCode = (int)STATUS.ERROR; break; //검사를 진행할 수 없는 에러 처리. user stop 과 같은 개념.
                default: break;
            }
            return iRtnCode;
        }

        //결과 값 숫자비교
        private int CompareNumber(string strData, string strMin, string strMax, ref string strReplaceData)
        {
            int iCase = 0;

            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMin.Length < 1 && strMax.Length < 1) return (int)STATUS.CHECK;
            if (strMin.Length > 0 && strMax.Length > 0)
            {
                iCase = 3; //둘다 비교
            }
            else
            {
                if (strMin.Length > 0) iCase = 1; //Min 값만 비교
                else iCase = 2; //Max 값만 비교
            }
                        
            double dVal = 0.0;
            double dMin = 0.0;
            double dMax = 0.0;

            try
            {
                dVal = double.Parse(strData);
                if (dVal < 1) { strReplaceData = dVal.ToString("0.#######"); }
                else { strReplaceData = dVal.ToString("0.#######"); }
                
            }
            catch (System.Exception ex)
            {
                string strEx = ex.Message;
                return (int)STATUS.NG;
            }

            try
            {
                switch (iCase)
                {
                    case 1: //MIN값만 비교
                            dMin = double.Parse(strMin);                            
                            if (dVal >= dMin) return (int)STATUS.OK;
                            else return (int)STATUS.NG;

                    case 2: //MAX값만 비교                            
                            dMax = double.Parse(strMax);
                            if (dVal <= dMax) return (int)STATUS.OK;
                            else return (int)STATUS.NG;

                    case 3: //MIN ~ MAX 값 비교
                    default:
                            dMin = double.Parse(strMin);
                            dMax = double.Parse(strMax);
                            if (dVal >= dMin && dVal <= dMax) return (int)STATUS.OK;
                            else return (int)STATUS.NG;
                }

            }
            catch (Exception e)
            {
                string strEx = e.ToString();
                return (int)STATUS.CHECK;
            } 

        }
        
        //결과 값 문자비교
        private int CompareWord(string strData, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            if (strData.Equals(strMax)) return (int)STATUS.OK;
            else return (int)STATUS.NG;
        }

        //결과 값 대소문자 구분없이 비교
        private int CompareText(string strData, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            if (strData.ToUpper().Equals(strMax.ToUpper())) 
                return (int)STATUS.OK;
            else 
                return (int)STATUS.NG;
        }

        //결과 값 min 또는 max 와 일치하는지 비교
        private int CompareOr(string strData, string strMin, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1)  return (int)STATUS.CHECK;
            if (strMin.Length < 1)  return (int)STATUS.CHECK;
            
            if (strData.Equals(strMax)) return (int)STATUS.OK;
            if (strData.Equals(strMin)) return (int)STATUS.OK;
            return (int)STATUS.NG;
        }

        //결과 값 문자비교
        private int CompareEvenOne(string strData, string strMax)
        {
            //콤마로 구분된 여러개의 값들중에 한개라도 같은 것이 있으면 OK
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            string[] strTempSpl = strMax.Split(',');
            if (strTempSpl.Length < 1)
            {
                return (int)STATUS.CHECK;
            }

            for (int i = 0; i < strTempSpl.Length; i++)
            {
                if (strData.Equals(strTempSpl[i])) return (int)STATUS.OK;
            }

            return (int)STATUS.NG;
        }

        //결과 값이 문자열에 포함되어있으면 NG
        private int CompareUnderCoverage(string strData, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            if (strData.Contains(strMax))
                return (int)STATUS.NG;
            else
                return (int)STATUS.OK;
        }

        //콤마로 구분된 여러개의 값들중에 한개라도 같은 것이 있으면 NG
        private int CompareUnderCoverage2(string strData, string strMax)
        {
            
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            string[] strTempSpl = strMax.Split(',');
            if (strTempSpl.Length < 1)
            {
                return (int)STATUS.CHECK;
            }

            for (int i = 0; i < strTempSpl.Length; i++)
            {
                if (strData.Equals(strTempSpl[i])) return (int)STATUS.NG;
            }

            return (int)STATUS.OK;
        }

        //콜론제거후 비교
        private int CompareNoneColon(string strData, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            string strTmax = strMax.Replace(":", String.Empty);
            string strTData = strData.Replace(":", String.Empty);

            if (strTData.Equals(strTmax))
                return (int)STATUS.OK;
            else
                return (int)STATUS.NG;
        }

        //결과 값이 문자열에 포함되어있는가를 비교
        private int CompareContain(string strData,  string strMin, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1)  return (int)STATUS.CHECK;

            int iPos = 0;
            int iIdx = strData.IndexOf(strMax);

            if (strMin.Length > 0)
            {
                iPos = int.Parse(strMin) - 1;
                if (iIdx == iPos)
                {
                    return (int)STATUS.OK;
                }
                else return (int)STATUS.NG;
            }
            else
            {
                if (iIdx > -1)
                {
                    return (int)STATUS.OK;
                }
                else return (int)STATUS.NG;
            }
        }

        //Measure(결과값)이 max(비교값)보다 작을때 max(비교값)을 min 에 입력된 값만큼 잘라서 비교한다.
        private int CompareContain2(string strData, string strMin, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1)  return (int)STATUS.CHECK;
            if (strMin.Length < 1)  return (int)STATUS.CHECK;
            
            int iLenth = 0;
            try
            {
                iLenth = int.Parse(strMin);
            }
            catch
            {
                return (int)STATUS.NG;
            }
            if (iLenth < strMax.Length)
            {
                string strComparedata = strMax.Substring(0, iLenth);

                if (strData.Contains(strComparedata))                
                    return (int)STATUS.OK;                
                else                
                    return (int)STATUS.NG;                
            }
            else
            {
                return (int)STATUS.NG;
            }
        }

        //결과 값의 특정위치에서부터 비교 문자열이 있는지를 비교
        private int CompareIndexOf(string strData, string strMin, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;
            if (strMin.Length < 1) return (int)STATUS.CHECK;

            int iIndex = 0;
            try
            {
                iIndex = int.Parse(strMin);
            }
            catch
            {
                return (int)STATUS.NG;
            }
            if (iIndex + strMax.Length <= strData.Length)
            {
                if (strData.IndexOf(strMax, iIndex) == iIndex)
                {
                    return (int)STATUS.OK;
                }                
                else
                    return (int)STATUS.NG;
            }
            else
            {
                return (int)STATUS.NG;
            }
        }

        //결과 값에 공백 혹은 널문자열이 있으면 전부 삭제하고 비교
        private int CompareTrim(string strData, string strMin, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            strData = strData.Trim();
            strMax = strMax.Trim();
            strData = strData.Replace(" ", String.Empty);
            strMax = strMax.Replace(" ", String.Empty);
            strData = strData.Replace("\0" , String.Empty);            

            if (strData.Equals(strMax)) return (int)STATUS.OK;
            else return (int)STATUS.NG;

        }

        //결과 값이 문자열과 달라야 할 경우가 PASS인 경우
        private int CompareDifferent(string strData, string strMax)
        {
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            if (strData.Equals(strMax)) return (int)STATUS.NG;
            else return (int)STATUS.OK;
            
        }
        //결과 값을 길이만 체크할 경우
        private int CompareLength(string strData, string strMin, string strMax)
        {
            int iCtype = 0; // 0: 둘다비교,  1: min만 비교  2: max만 비교            
            int iMin   = 0;
            int iMax   = 0;

            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMin.Length < 1 && strMax.Length < 1) return (int)STATUS.CHECK;

            if (strMin.Length > 0)
            {
                iMin = int.Parse(strMin);
                iCtype = 1;
            }
            if (strMax.Length > 0)
            {
                iMax = int.Parse(strMax);
                iCtype = 2;
            }
            if (strMin.Length > 0 && strMax.Length > 0)
            {
                iCtype = 0;
            }

            int iRtnRes = (int)STATUS.NG;

            switch (iCtype)
            {
                case 0: //둘다비교
                    if (strData.Length >= iMin && strData.Length <= iMax)
                    {
                        iRtnRes = (int)STATUS.OK;
                    }
                    break;
                case 1: //MIN 만 비교
                    if (strData.Length >= iMin)
                    {
                        iRtnRes = (int)STATUS.OK;
                    }
                    break;
                case 2: //MAX 만 비교
                    if (strData.Length <= iMax)
                    {
                        iRtnRes = (int)STATUS.OK;
                    }
                    break;
            }

            return iRtnRes;            

        }

        //결과 값을 ~문자로 패턴 체크할 경우
        private int ComparePattern(string strData, string strMax)
        {
            //1. 수신 데이터가 패턴 길이보다 작을 경우 NG
            if (strData.Length < strMax.Length ) return (int)STATUS.NG;

            //2. 패턴에 ~ 문자가 없거나 패턴 자체가 없을 경우 CHECK
            if (!strMax.Contains("~") && strMax.Length < 1) return (int)STATUS.CHECK;

            //3. 패턴 비교

            for (int i = 0; i < strMax.Length; i++)
            {
                if (strMax[i] != '~')
                {
                    if (strMax[i] != strData[i]) return (int)STATUS.NG;
                }
            }

            return (int)STATUS.OK;

        }

        //결과 값을 ~문자로 패턴 체크할 경우이며 패턴이 달라야 pass 인경우.
        private int CompareNotPattern(string strData, string strMax)
        {
            //1. 수신 데이터가 패턴 길이보다 작을 경우 NG
            if (strData.Length < strMax.Length) return (int)STATUS.NG;

            //2. 패턴에 ~ 문자가 없거나 패턴 자체가 없을 경우 CHECK
            if (!strMax.Contains("~") && strMax.Length < 1) return (int)STATUS.CHECK;

            //3. 패턴 비교
            bool bPattern = false;
            for (int i = 0; i < strMax.Length; i++)
            {
                if (strMax[i] != '~')
                {
                    if (strMax[i] != strData[i])
                    {
                        bPattern = true;
                    }
                }
            }

            if(bPattern)
                return (int)STATUS.OK;
            else
                return (int)STATUS.NG;

        }

        //결과 값을 패턴비교하는데 여러가지 케이스인경우
        private int ComparePatterns(string strData, string strMax)
        {
            //콤마로 구분된 여러개의 값들중에 한개라도 같은 것이 있으면 OK
            if (strData.Length < 1) return (int)STATUS.NG;
            if (strMax.Length < 1) return (int)STATUS.CHECK;

            string[] strTempSpl = strMax.Split(',');
            if (strTempSpl.Length < 1)
            {
                return (int)STATUS.CHECK;
            }

            bool bSuccess = true;
            for (int j = 0; j < strTempSpl.Length; j++)
            {

                if (strTempSpl[j].Length < 2)
                {
                    return (int)STATUS.CHECK;
                }

                //3. 패턴 비교
                bSuccess = true;
                for (int i = 0; i < strTempSpl[j].Length; i++)
                {
                    if (strTempSpl[j][i] != '~')
                    {
                        if (strTempSpl[j][i] != strData[i])
                        {
                            bSuccess = false;
                            break;
                        }
                    }
                }
                if (bSuccess) return (int)STATUS.OK;

            }
            return (int)STATUS.NG;
        }

        //결과 값을 ~문자로 패턴 체크할 경우 PCAN 에서만 쓰네??
        public int ComparePattern2(string strData, string strMax, ref string[] strBlocks)
        {
            //1. 수신 데이터가 패턴 길이보다 작을 경우 NG
            if (strData.Length < strMax.Length) return (int)STATUS.NG;

            //2. 패턴에 ~ 문자가 없거나 패턴 자체가 없을 경우 CHECK
            if (!strMax.Contains("~") && strMax.Length < 1) return (int)STATUS.CHECK;

            //3. 패턴 비교

            for (int i = 0; i < strMax.Length; i++)
            {
                if (strMax[i] != '~')
                {
                    if (strMax[i].Equals('*'))
                    {
                        int iDx = 0;
                        try
                        {
                            iDx = int.Parse(strMax[i+1].ToString());
                            strBlocks[iDx * 2]       = strData[i].ToString();
                            strBlocks[(iDx * 2) + 1] = strData[i + 1].ToString();
                            i++;
                            continue;
                        }
                        catch 
                        {
                            return (int)STATUS.NG;
                        }
                    }

                    if (strMax[i] != strData[i]) return (int)STATUS.NG;
                }
            }

            return (int)STATUS.OK;

        }

        //기타 특별한 분석시 ETC
        private int AnalyzeResponseETC(string strResponse, string strdata, string strMin, string strMax)
        {

            return 0;
        }
    }
}

