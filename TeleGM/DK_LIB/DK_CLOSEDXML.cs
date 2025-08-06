using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using System.IO;

namespace GmTelematics
{
    enum CATEGORY   //향후 범용성을 고려해서 생각한 1차 컨셉이다. 지금당장은 멍청이들 때문에 안쓰지만 나중에 변경될것을 고려해 남겨둔다.
    {
        PROPERTIES, VALUE, YN, ETC
    }
        
    class DK_CLOSEDXML
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

        private XLWorkbook workbook;

        public DK_CLOSEDXML()
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

        private bool bCheckExcelFile(string strFileName)
        {
            if (File.Exists(strDataPath + strFileName)) //Tele_Spec_GM_MX_6.53_v3.xlsx
                return true;
            else
                return false;
        }

        private void InitWorkBook()
        {
            try
            {
                if (workbook != null)
                {
                    workbook.Dispose();
                    workbook = null;
                }
            }
            catch 
            {
            	
            }
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
                //GC.Collect();
                // new XLWorkbook(strDataPath + strFileName);  // 기존 엑셀 열기
                InitWorkBook();
                workbook = new XLWorkbook(strDataPath + strFileName);  // 기존 엑셀 열기
                
                //1. 파일 포멧 검사
                strSubject[0] = workbook.Worksheet(1).Cell("A1").Value.ToString();
                strSubject[1] = workbook.Worksheet(1).Cell("B1").Value.ToString();
                strSubject[2] = workbook.Worksheet(1).Cell("C1").Value.ToString();
                strSubject[3] = workbook.Worksheet(1).Cell("D1").Value.ToString();                
                //E1 은 검사하지 말자. 쓰던지 말던지.
                if (!bCheckSubject(strSubject))
                {
                    strReason = "Subject Format Error";                    
                    //MemoryClearFunc(workbook);
                    InitWorkBook();
                    return false;
                }

                //2. 내용물 검색                
                InspDoc tmpDoc = new InspDoc();
                tmpDoc.Index    = String.Empty;
                tmpDoc.Category = String.Empty;
                tmpDoc.SpecItem = String.Empty;
                tmpDoc.Contents = String.Empty;
                string strTmpCellA = String.Empty;
                string strTmpCellB = String.Empty; 
                string strTmpCellC = String.Empty;
                string strTmpCellD = String.Empty;

                for (int i = 2; i <= iMAXITEMS; i++) //문서는 최대 1000개까지만 지원하자.
                {
                    strTmpCellA = "A" + i.ToString();
                    strTmpCellB = "B" + i.ToString();
                    strTmpCellC = "C" + i.ToString();
                    strTmpCellD = "D" + i.ToString();
                    tmpDoc.Index = workbook.Worksheet(1).Cell(strTmpCellA).Value.ToString();
                    if (String.IsNullOrEmpty(tmpDoc.Index))
                    {
                        break;
                    }
                    else
                    {   
                        tmpDoc.Category = workbook.Worksheet(1).Cell(strTmpCellB).Value.ToString();
                        tmpDoc.SpecItem = workbook.Worksheet(1).Cell(strTmpCellC).Value.ToString();
                        tmpDoc.Contents = workbook.Worksheet(1).Cell(strTmpCellD).Value.ToString();
                        if (String.IsNullOrEmpty(tmpDoc.SpecItem) || String.IsNullOrEmpty(tmpDoc.Contents))
                        {
                            continue;
                        }
                        else
                        {
                            /*
                            switch (bCheckCategory(tmpDoc))
                            {                                
                                case (int)CATEGORY.VALUE:
                                    if(tmpDoc.SpecItem.Contains(" "))
                                    {
                                        strReason = "Can't Use SPACE Character in Spec Item Name (line:" + tmpDoc.Index + ", " + tmpDoc.SpecItem + ")";
                             * workbook.Dispose();
                                        MemoryClearFunc();
                                        return false;
                                    }
                                    tmpDoc.Contents = tmpDoc.Contents.Trim();
                                    lstInspDoc.Add(tmpDoc);
                                    break;
                                case (int)CATEGORY.YN:
                                    
                                    if(tmpDoc.SpecItem.Contains(" "))
                                    {
                                        strReason = "Can't Use SPACE Character in Spec Item Name (line:" + tmpDoc.Index + ", " + tmpDoc.SpecItem + ")";
                             * workbook.Dispose();
                                        MemoryClearFunc();
                                        return false;
                                    }

                                    string strYNitem = tmpDoc.Contents.ToUpper();

                                    switch (strYNitem)
                                    {
                                        case "YES": case "NO": case "NONE":
                                            break;
                                        default:
                                            strReason = "Contents is incorrect (only Yes or No or None), (line:" + tmpDoc.Index + ", " + tmpDoc.Contents + ")";
                             * workbook.Dispose();
                                            MemoryClearFunc();
                                            return false;
                                    }
                                    tmpDoc.Contents = strYNitem;
                                    lstInspDoc.Add(tmpDoc);
                                    break;
                                case (int)CATEGORY.PROPERTIES: //프로퍼티는 저장하지 않은 엑셀 문에서에만 관리되는 항목임.
                                case (int)CATEGORY.ETC:        //기타 정의 되지 않는 항목.
                                default:
                                        break;
                            } */

                            switch (bCheckCategory2(tmpDoc))
                            {
                                case (int)CATEGORY.VALUE:
                                case (int)CATEGORY.YN:

                                    if (String.IsNullOrEmpty(tmpDoc.SpecItem)) continue;  //비어있는경우는 사용안하는것으로 간주하므로 무시

                                    if (tmpDoc.SpecItem.Contains(" "))
                                    {
                                        strReason = "Can't Use SPACE Character in Spec Item Name (line:" + tmpDoc.Index + ", " + tmpDoc.SpecItem + ")";
                                        InitWorkBook();
                                        return false;
                                    }
                                    
                                    string strYNitem = tmpDoc.Contents.ToUpper();

                                    switch (strYNitem)
                                    {
                                        case "NO":
                                        case "NONE": continue;  //NO 이거나 NONE 은 사용안하는것으로 간주하므로 무시

                                        case "YES":                                        
                                        default:    
                                                    lstInspDoc.Add(tmpDoc); break;
                                    }
                                    break;
                                case (int)CATEGORY.PROPERTIES: break; //프로퍼티는 저장하지 않은 엑셀 문에서에만 관리되는 항목임.

                                case (int)CATEGORY.ETC:       
                                default:  //기타 정의 되지 않는 항목은 Error 로 처리
                                        strReason = "Error - Unknown Category in File(" + tmpDoc.Category + ")";
                                        InitWorkBook();
                                        return false;
                            }

                        }
                    }
                }
                InitWorkBook();
                
                if (lstInspDoc.Count > 0)
                {
                    //Spec Iem 중복검사.
                    if (!bCheckDuplicateData(lstInspDoc))
                    {
                        strReason = "(Excel File)Duplicate Name - Spec Item";     
                        return false;
                    }
                    else
                    {
                        strReason = "SUCCESS";       
                        return true;
                    }
                    
                }
                else
                {
                    strReason = "NO DATA";   
                    return false;
                }

            }
            catch(Exception ex)
            {
                string exStr = ex.Message;
                strReason = "File Read Error.";                
                return false;
            }

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


