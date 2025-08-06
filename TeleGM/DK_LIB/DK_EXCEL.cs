using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;


namespace GmTelematics
{
    struct ExcelData
    {
        public string strSubject;
        public string strData;
    }

    class DK_EXCEL
    {
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

        public DK_EXCEL()
        {
            
        }

        public string CheckExcelVersion()
        {
            Excel.Application excelApp = new Excel.Application();

            string strVer = "NONE";
            if (excelApp != null)
            {
                strVer = excelApp.Version.ToString();
            }
            return strVer;
        }

        public bool ReadExcelData(string strFilePath, string strSubjectName, string strWip, ref ExcelData[] rtnLst, ref string strReason)
        {
            GC.Collect();
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            bool bSuccese = false;

            //파일 유무 확인   
            if (!System.IO.File.Exists(strFilePath))
            {   //파일이 없으면 return!
                strReason = "NOT FOUND EXCEL FILE.";
                return bSuccese;
            }

            strReason = "SUCCESS";
                       
            try
            {
                excelApp = new Excel.Application();

                // 엑셀 파일 열기
                wb = excelApp.Workbooks.Open(strFilePath);

                
                // 첫번째 Worksheet
                
                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
        
                int iCols = ws.UsedRange.Columns.Count;
                int iRows = ws.UsedRange.Rows.Count;

         
                var varUsedRange = ws.UsedRange.Value2;
                
                
                rtnLst = new ExcelData[iCols+1];

                for (int x = 1; x <= iCols; x++)
                {                    
                    try
                    {
                        var celldata = varUsedRange[1, x];
                        if (celldata == null) continue;
                        rtnLst[x].strSubject = celldata.ToString();
                        
                        if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                    }
                    catch (System.Exception ex)
                    {
                        string strEx = ex.Message;
                        continue;
                    }
                    
                }

                for (int x = 1; x <= iCols; x++)
                {
                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                    var celldata = varUsedRange[1, x];

                    if (celldata != null && celldata.ToString().Equals(strSubjectName))
                    {
                        for (int y = 2; y <= iRows; y++)
                        {
                            if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;
                            var celldata2 = varUsedRange[y, x];
                            if (celldata2 != null && celldata2.ToString().Equals(strWip))
                            {
                                bool bSearch3 = false;
                                for (int i = 1; i <= iCols; i++)
                                {
                                    if (!STEPMANAGER_VALUE.bProgramRun && !STEPMANAGER_VALUE.bInteractiveMode) break;                                    
                                    
                                    var celldata3 = varUsedRange[y, i];
                                    if (celldata3 != null)
                                    {
                                        rtnLst[i].strData = celldata3.ToString();
                                        bSearch3 = true;
                                    }
                                }
                                if (bSearch3)
                                {
                                    bSuccese = true;
                                }
                                break;
                            }                         
                        }
                       
                        break;
                    }  
                }
               
            }
            catch(Exception e) 
            {
                strReason = e.Message;
                bSuccese = false;
            }
            finally
            {
                try
                {
                    wb.Close(true);
                }
                catch { }

                try
                {
                    excelApp.Quit();
                }
                catch {

                }
                finally
                {
                    // Clean up
                    ReleaseExcelObject(ws);
                    ReleaseExcelObject(wb);
                    ReleaseExcelObject(excelApp);
                }

            }

            if (!bSuccese) strReason = "NOT FOUND : " + strSubjectName;
            return bSuccese;
        }

        private void CopyData(ref string strVar, dynamic dm, int ix, int iy)
        {
            if (dm[ix, iy] != null)
            {
                strVar = dm[ix, iy].ToString();
            }
            else
            {
                strVar = "";
            }
        }
   
        public bool ReadExcelFile(string strFilePath, ref List<InspDoc> lstInspDoc, ref string strReason)
        {
            GC.Collect();
            string[] strSubject = new string[4];
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            bool bSuccese = false;    

            //파일 유무 확인   
            if (!System.IO.File.Exists(strFilePath))
            {   //파일이 없으면 return!
                strReason = "NOT FOUND EXCEL FILE.";
                return bSuccese;
            }

            strReason = "SUCCESS";

            try
            {
                excelApp = new Excel.Application();

                // 엑셀 파일 열기
                wb = excelApp.Workbooks.Open(strFilePath);


                // 첫번째 Worksheet

                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;

                
                //1. 파일 포멧 검사
                    
                
                int iCols = ws.UsedRange.Columns.Count;
                int iRows = ws.UsedRange.Rows.Count;

                var varUsedRange = ws.UsedRange.Value2;

                CopyData(ref strSubject[0], varUsedRange, 1, 1);
                CopyData(ref strSubject[1], varUsedRange, 1, 2);
                CopyData(ref strSubject[2], varUsedRange, 1, 3);
                CopyData(ref strSubject[3], varUsedRange, 1, 4);

                //strSubject[0] = varUsedRange[1, 1].ToString(); //strSubject[0] = //workbook.Worksheet(1).Cell("A1").Value.ToString();
                //strSubject[1] = varUsedRange[1, 2].ToString(); //strSubject[1] = //workbook.Worksheet(1).Cell("B1").Value.ToString();
                //strSubject[2] = varUsedRange[1, 3].ToString(); //strSubject[2] = //workbook.Worksheet(1).Cell("C1").Value.ToString();
                //strSubject[3] = varUsedRange[1, 4].ToString(); //strSubject[3] = //workbook.Worksheet(1).Cell("D1").Value.ToString();   

                //E1 은 검사하지 말자. 쓰던지 말던지.
                if (!bCheckSubject(strSubject))
                {
                    strReason = "Subject Format Error";
                    return false;
                }

                //-------- 구현 내용 ------------

                //2. 내용물 검색                
                InspDoc tmpDoc = new InspDoc();
                tmpDoc.Index = String.Empty;
                tmpDoc.Category = String.Empty;
                tmpDoc.SpecItem = String.Empty;
                tmpDoc.Contents = String.Empty;

                for (int i = 2; i <= iRows; i++) //문서는 최대 1000개까지만 지원하자.
                {
                    CopyData(ref tmpDoc.Index, varUsedRange, i, 1);
                    //tmpDoc.Index = varUsedRange[i, 1].ToString();

                    if (String.IsNullOrEmpty(tmpDoc.Index))
                    {
                        break;
                    }
                    else
                    {
                        //tmpDoc.Category = workbook.Worksheet(1).Cell(strTmpCellB).Value.ToString();
                        //tmpDoc.SpecItem = workbook.Worksheet(1).Cell(strTmpCellC).Value.ToString();
                        //tmpDoc.Contents = workbook.Worksheet(1).Cell(strTmpCellD).Value.ToString();

                        CopyData(ref tmpDoc.Category, varUsedRange, i, 2);
                        CopyData(ref tmpDoc.SpecItem, varUsedRange, i, 3);
                        CopyData(ref tmpDoc.Contents, varUsedRange, i, 4);

                        //tmpDoc.Category = varUsedRange[i, 2].ToString();
                        //tmpDoc.SpecItem = varUsedRange[i, 3].ToString();
                        //tmpDoc.Contents = varUsedRange[i, 4].ToString();

                        if (String.IsNullOrEmpty(tmpDoc.SpecItem) || String.IsNullOrEmpty(tmpDoc.Contents))
                        {
                            continue;
                        }
                        else
                        {
                            switch (bCheckCategory2(tmpDoc))
                            {
                                case (int)CATEGORY.VALUE:
                                case (int)CATEGORY.YN:

                                    if (String.IsNullOrEmpty(tmpDoc.SpecItem)) continue;  //비어있는경우는 사용안하는것으로 간주하므로 무시

                                    if (tmpDoc.SpecItem.Contains(" "))
                                    {
                                        strReason = "Can't Use SPACE Character in Spec Item Name (line:" + tmpDoc.Index + ", " + tmpDoc.SpecItem + ")";
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
                                    return false;
                            }

                        }
                    }
                }

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
            catch
            {
                strReason = "DOCUMENT FORMAT ERROR";
                bSuccese = false;
            }
            finally
            {
                try
                {
                    wb.Close(true);
                }
                catch { }

                try
                {
                    excelApp.Quit();
                }
                catch
                {

                }
                finally
                {
                    // Clean up
                    ReleaseExcelObject(ws);
                    ReleaseExcelObject(wb);
                    ReleaseExcelObject(excelApp);
                }

            }

            if (!bSuccese) strReason = "FAIL";
            return bSuccese;
        }

        private bool bCheckSubject(string[] strSubject)
        {
            // 파일 포멧 정의 by 이동성 선임 (2018.2.7 메일)
            //A1 = No        - 문서 용도
            //B1 = Category  - 실제 사용
            //C1 = Spec Item - 실제 사용
            //D1 = Contents  - 실제 사용
            //E1 = Remark    - 문서 용도
            if (!strSubject[0].Equals("No")) return false;
            if (!strSubject[1].Equals("Category")) return false;
            if (!strSubject[2].Equals("Spec Item")) return false;
            if (!strSubject[3].Equals("Contents")) return false;

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
            if (tmpDoc.Category.IndexOf("Properties_").Equals(0))
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

                default: return (int)CATEGORY.ETC;

            }
        }

        private void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}

