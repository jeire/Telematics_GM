using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ClosedXML.Excel;
using System.IO;

namespace GmTelematics
{
    /*
    enum CATEGORY   //향후 범용성을 고려해서 생각한 1차 컨셉이다. 지금당장은 멍청이들 때문에 안쓰지만 나중에 변경될것을 고려해 남겨둔다.
    {
        PROPERTIES, VALUE, YN, ETC
    }
    */
    class DK_DOCUMENT
    {
        private string strDataPath = String.Empty;
        private string strFormat1 = "XLS";
        private string strFormat2 = "XLSX";

        //멍청이들이 정한 아이템 갯수, fix 라고 하는데 나중에 100퍼 바뀜.
        private const string constCATEGORY01 = "Basic_Properties";
        private const string constCATEGORY02 = "Test_Properties";
        private const string constCATEGORY03 = "Set_Value_SW VERSION";
        private const string constCATEGORY04 = "Set_Value_COUNTRY ID";
        private const string constCATEGORY05 = "Set_Value_PARAMETER";
        private const string constCATEGORY06 = "Part_Number_Value";
        private const string constCATEGORY07 = "Key_Value";
        private const string constCATEGORY08 = "Default_Setting_Value";
        
        private const int iMAXITEMS = 1000; //최대 아이템

        //private XLWorkbook workbook;
        private DK_EXCEL workbook;// = new DK_EXCEL();

        public DK_DOCUMENT()
        {
            strDataPath = AppDomain.CurrentDomain.BaseDirectory + "DATA\\";
            SetCategoryList(); 
        }

        private void SetCategoryList()
        {   //이딴식으로 정의해줘서 짜증난다.. 나중에 포맷 변경요청이 계속 일어날것이다. 이딴식으로 가면 프로그램 매번 수정된다.
            
        }


        public bool GetInspectionDocuments(string strFileName, ref List<InspDoc> lstInspDoc, ref string strReason)
        {
            strReason = "SUCCESS";
            lstInspDoc.Clear();

            //1. 파일 찾기.
            if (!bCheckExcelFile(strFileName))
            {
                strReason = "CAN NOT FOUND FILE";
                return false;
            }

            //2. 파일 스캔하여 회신.
            return bScanExcelFile(strFileName, ref lstInspDoc, ref strReason);
            
        }

        private bool bScanExcelFile(string strFileName, ref List<InspDoc> lstInspDoc, ref string strReason)
        {
            if (!strFileName.ToUpper().Contains(strFormat1) || !strFileName.ToUpper().Contains(strFormat2))
            {
                strReason = "CHECK FILE. XLS or XLSX";
                return false;
            }

            string[] strSubject = new string[4];
            try
            {
                workbook = new DK_EXCEL();
                return workbook.ReadExcelFile(strDataPath + strFileName, ref lstInspDoc, ref strReason);
                
            }
            catch (Exception ex)
            {
                string exStr = ex.Message;
                strReason = "File Read Error.";
                return false;
            }

        }

        private bool bCheckExcelFile(string strFileName)
        {
            if (File.Exists(strDataPath + strFileName)) //Tele_Spec_GM_MX_6.53_v3.xlsx
                return true;
            else
                return false;
        }



        private bool bCheckSubject(string[] strSubject)
        {
            // 파일 포멧 정의 by 이동성 선임 (2018.2.7 메일)
            //A1 = No        - 문서 용도
            //B1 = Category  - 실제 사용
            //C1 = Spec Item - 실제 사용
            //D1 = Contents  - 실제 사용
            //E1 = Remark    - 문서 용도
            if (!strSubject[0].Equals("No"))        return false;
            if (!strSubject[1].Equals("Category"))  return false;
            if (!strSubject[2].Equals("Spec Item")) return false;
            if (!strSubject[3].Equals("Contents"))  return false;

            return true;

        }

        private bool bCheckDuplicateData(List<InspDoc> lstData)
        {            
            List<string> lstNames = new List<string>();
            InspDoc tmpDoc = new InspDoc();
            tmpDoc.Index = String.Empty;
            tmpDoc.Category = String.Empty;
            tmpDoc.SpecItem = String.Empty;
            tmpDoc.Contents = String.Empty;

            for (int i = 0; i < lstData.Count; i++)
            {
                tmpDoc = lstData[i];
                if (!lstNames.Contains(tmpDoc.SpecItem))
                    lstNames.Add(tmpDoc.SpecItem);
            }
            if (lstNames.Count != lstData.Count) 
                return false;
            else
                return true;
            //bool bCheckDup = lstNames.HasDuplicates(); //중복체크

            //return !bCheckDup;
        }

        private int bCheckCategory(InspDoc tmpDoc) //나중에 사용될 방법
        {
            if(tmpDoc.Category.IndexOf("Properties_").Equals(0))
            {
                return (int)CATEGORY.PROPERTIES;
            }

            if (tmpDoc.Category.IndexOf("Value_").Equals(0))
            {
                return (int)CATEGORY.VALUE;
            }

            if (tmpDoc.Category.IndexOf("YN_").Equals(0))
            {
                return (int)CATEGORY.YN;
            }

            return (int)CATEGORY.ETC;
            
        }

        private int bCheckCategory2(InspDoc tmpDoc) //현재 사용
        {
            switch (tmpDoc.Category)
            {
                case constCATEGORY01:
                case constCATEGORY02: return (int)CATEGORY.PROPERTIES;

                case constCATEGORY03:
                case constCATEGORY04:
                case constCATEGORY05:
                case constCATEGORY06:
                case constCATEGORY07:
                case constCATEGORY08: return (int)CATEGORY.VALUE;

                default:              return (int)CATEGORY.ETC;

            }
        }

    }
}


