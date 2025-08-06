using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*  EXPR의 사용용도 정의
 * max field option


#EXPR_필드이름
1. max field option -  #GMES_필드이름, #EXPR_필드이름  으로 사용
2. par1 field option - #GMES_필드이름, #EXPR_필드이름  으로 사용
3. expr field option - MEAS 는 현재명령의 응답값
                       #LOAD:필드이름            : meas 대신 필드이름의 값을 보여줌.
                       #SAVE:필드이름            : meas 값을 필드이름으로 저장함
                       #MATH:MEAS+필드이름+0.1   : 수식을 계산하여 최종결과로 보여줌.
                       #DEF:필드이름=data        : data 값을 필드이름으로 저장함.

*/
namespace GmTelematics
{
    class DK_EXPR
    {
        Dictionary<string, string> DIC_EXPR;
        private const string STR_SAVE = "#SAVE:";
        private const string STR_LOAD = "#LOAD:";
        private const string STR_MATH = "#MATH:";
        private const string STR_DEF  = "#DEF:";
        private const string STR_MEAS = "MEAS";
        private const string STR_CONV = "#CONV:";
        private const string STR_HEXA = "#HEXA:";
        //202505027 MEAS 값 암호화 처리.
        private const string STR_ENC = "#ENC:";

        public DK_EXPR()
        {
            DIC_EXPR = new Dictionary<string, string>();
        }

        public  int GetExprCount()
        {
            return DIC_EXPR.Count;
        }

        public  int GetExprType(string strExprDataString)
        {
            return ParseString(strExprDataString);
        }
        
        public bool ExcuteSave(string strExprDataString, string strMeas)
        {
            string strTemp = String.Empty;
            if (String.IsNullOrEmpty(strExprDataString)) return false;
            if (strExprDataString.Length <= STR_SAVE.Length) return false;

            strTemp = strExprDataString.Replace(STR_SAVE, String.Empty);
            Item_Save(strTemp, strMeas);
            return true;     
        }

        public bool ExcuteConv(string strExprDataString, string strMeas)
        {   //데시멀값을 헥사타입으로 변환하여 저장하는 함수..
            string strTemp = String.Empty;
            if (String.IsNullOrEmpty(strExprDataString)) return false;
            if (strExprDataString.Length <= STR_CONV.Length) return false;

            strTemp = strExprDataString.Replace(STR_CONV, String.Empty);
           
            try
            {

                uint iDec = uint.Parse(strMeas);                
                byte[] bHex = BitConverter.GetBytes(iDec);  
                strMeas = "";
                for (int i = bHex.Length - 1; i >= 0; i--)
                {
                    strMeas += bHex[i].ToString("X2");
                }
            }
            catch
            {
                return false;
            }           

            Item_Save(strTemp, strMeas);
            return true;
        }

        public bool ExcuteHexa(string strExprDataString, string strMeas)
        {   //데시멀값을 헥사타입으로 변환하여 저장하는 함수..
            string strTemp = String.Empty;
            if (String.IsNullOrEmpty(strExprDataString)) return false;
            if (strExprDataString.Length <= STR_HEXA.Length) return false;

            strTemp = strExprDataString.Replace(STR_HEXA, String.Empty);

            try
            {
                byte[] bHexa = Encoding.UTF8.GetBytes(strMeas);
                string strHexa = BitConverter.ToString(bHexa).Replace("-", "");
                Item_Save(strTemp, strHexa);
                return true;
               
            }
            catch
            {
                return false;
            }

            
        }

        public bool ExcuteLoad(string strExprDataString, ref string strReturn)
        {
            string strTemp = String.Empty;
            if (String.IsNullOrEmpty(strExprDataString)) return false;
            //if (strExprDataString.Length <= STR_LOAD.Length) return false;

            strTemp = strExprDataString.Replace(STR_LOAD, String.Empty);

            //문자열 합치기 기능 추가.
            //저장된 IMEI 데이터가 1234 이고  #EXPR_IMEI[*K] 로 호출되면 리턴값은 K1234 로.
            //저장된 IMEI 데이터가 1234 이고  #EXPR_IMEI[K*] 로 호출되면 리턴값은 1234K 로.  

            string strAppendString = String.Empty;

            if (strTemp.Contains("[*"))
            {
                strAppendString = strTemp.Substring(strTemp.IndexOf('['));
                strAppendString = strAppendString.Replace("[*", String.Empty);
                strAppendString = strAppendString.Replace("]", String.Empty);

                strTemp = strTemp.Substring(0, strTemp.IndexOf('['));
                if (Item_Load(strTemp, ref strReturn))
                {
                    strReturn = strAppendString + strReturn;
                    return true;
                }
                return false;

            }
            else if (strTemp.Contains("*]"))
            {
                strAppendString = strTemp.Substring(strTemp.IndexOf('['));
                strAppendString = strAppendString.Replace("[", String.Empty);
                strAppendString = strAppendString.Replace("*]", String.Empty);

                strTemp = strTemp.Substring(0, strTemp.IndexOf('['));
                if (Item_Load(strTemp, ref strReturn))
                {
                    strReturn = strReturn + strAppendString;
                    return true;
                }
                return false;
            }
            else
            {
                return Item_Load(strTemp, ref strReturn);
            }

        }

        public bool ExcuteMath(string strExprDataString, string strMeas, ref string strReturn)
        {
            
            string strTemp = String.Empty;
            if (String.IsNullOrEmpty(strExprDataString)) return false;
            if (strExprDataString.Length <= STR_MATH.Length) return false;

            return Item_Math(strExprDataString, strMeas, ref strReturn);

        }
        
        public bool ExcuteDefine(string strExprDataString, string strMeas)
        {
            string strTemp = String.Empty;
            if (String.IsNullOrEmpty(strExprDataString)) return false;
            if (strExprDataString.Length <= STR_DEF.Length) return false;

            strTemp = strExprDataString.Replace(STR_DEF, String.Empty);
            //strTemp = strExprDataString.Replace("=", String.Empty);

            string[] tmpString = System.Text.RegularExpressions.Regex.Split(strTemp, "=");
            if (tmpString.Length != 2) { return false; }

            Item_Save(tmpString[0], tmpString[1]);
            return true;
        }
        
        public  void Clear()
        {
            DIC_EXPR.Clear();
        }

        private int ParseString(string strString)
        {
            try
            {
                int iStxHead = strString.IndexOf('#');
                if (iStxHead != 0) return (int)EXPRTYPE.ERROR;

                int iStxSave = strString.IndexOf(STR_SAVE);
                if (iStxSave == 0) return (int)EXPRTYPE.SAVE;
                int iStxLoad = strString.IndexOf(STR_LOAD);
                if (iStxLoad == 0) return (int)EXPRTYPE.LOAD;
                int iStxMath = strString.IndexOf(STR_MATH);
                if (iStxMath == 0) return (int)EXPRTYPE.MATH;
                int iStxDef = strString.IndexOf(STR_DEF);
                if (iStxDef == 0) return (int)EXPRTYPE.DEF;
                int iStxConv = strString.IndexOf(STR_CONV);
                if (iStxConv == 0) return (int)EXPRTYPE.CONV;
                int iStxHexa = strString.IndexOf(STR_HEXA);
                if (iStxHexa == 0) return (int)EXPRTYPE.HEXA;
                return (int)EXPRTYPE.ERROR;
            }
            catch
            {
                return (int)EXPRTYPE.ERROR;
            }
        }

        private void Item_Save(string strKey, string strData){

            string strTemp = String.Empty;
            
            try
            {

                if (DIC_EXPR.TryGetValue(strKey, out strTemp))
                {
                    DIC_EXPR[strKey] = strData;
                }
                else
                {
                    DIC_EXPR.Add(strKey, strData);
                }
            }
            catch { }
        }

        private bool Item_Load(string strKey, ref string strData)
        {
            string strTemp = String.Empty;
            try
            {
                
                if (DIC_EXPR.TryGetValue(strKey, out strTemp))
                {
                    strData += DIC_EXPR[strKey];
                    return true;
                }
                else
                {                    
                    return false;
                }
            }
            catch
            {                
                return false;
            }
        }

        private bool Item_Math(string strMathString, string strMeas, ref string strReturnVale)
        {
            bool bAbs = false;
            string strTmpMath = strMathString.Replace(STR_MATH, String.Empty); //명령구분 삭제
            strTmpMath = strTmpMath.Replace(STR_MEAS, strMeas);        //현재명령의 결과값 수식 치환

            if (strTmpMath.Contains("abs"))
            {
                bAbs = true;
                strTmpMath = strTmpMath.Replace("abs", String.Empty);
            }


            foreach (KeyValuePair<string, string> tmpPair in DIC_EXPR) //딕셔너리에 있는 데이터가 있으면 전부 수식치환
            {
                strTmpMath = strTmpMath.Replace(tmpPair.Key, tmpPair.Value);
            }

            System.Data.DataTable table = new System.Data.DataTable();

            double dVal = 0.0;
            try
            {

                strReturnVale = table.Compute(strTmpMath, "").ToString();
                dVal = double.Parse(strReturnVale);
                dVal = Math.Truncate(dVal * 1000) / 1000;

                if (bAbs)
                {
                    strReturnVale = Math.Abs(dVal).ToString("0.000");
                }
                else
                {
                    strReturnVale = dVal.ToString("0.000");
                }
                return true;
            }
            catch (Exception e)
            {
                string tmpStr = e.ToString();
                return false;
            }
        }

         
    }
}
