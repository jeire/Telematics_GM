//MOOHANTECH by DK.SIM 2015.03.26
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace GmTelematics
{   
    delegate void EventTxRxMsg(string cParam); //대리자 선언

    class DK_LOGGER
    {
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll")]
        private static extern int  GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        public event EventTxRxMsg   SendTxRxEvent;    //대리자가 날릴 실제 이벤트 메소드

        private string DeviceName;
        private string INIfileName;
        private string PWLISTfileName;
        private string SuffixFileName;
        private string ModelFileName;
        private string INIFreqfileName;
        private string INIDtcIndexfileName;
        private string GEN11DtcIndexfileName;
        private string GEN11GBDtcIndexfileName;
        private string GEN11GEMDtcIndexfileName;
        //private string GEN12DtcIndexfileName;
        //private string GEN12GBDtcIndexfileName;
        //private string GEN12GEMDtcIndexfileName;

        private string strProgramPath;
        private string strLogPath;
        private string CommLogName;
        private string InsLogName;
        private string CommLogBinName;
        private string ResultLogName;
        private string ResultNGscreen;
        private string History;        
        private string DataFolder;
        private string SystemFolder;
        private string melsecLogName;

        private bool bDebug;
        public string Item_INIfileName
        {
            get { return INIfileName; }
            set { INIfileName = value; }
        }

        public string Item_PWLISTfileName
        {
            get { return PWLISTfileName; }
            set { PWLISTfileName = value; }
        }

        public string Item_SuffixFileName
        {
            get { return SuffixFileName; }
            set { SuffixFileName = value; }
        }

        public string Item_ModelFileName
        {
            get { return ModelFileName; }
            set { ModelFileName = value; }
        }

        public string Item_INIFreqfileName
        {
            get { return INIFreqfileName; }
            set { INIFreqfileName = value; }
        }

        public string Item_INIDtcIndexfileName
        {
            get { return INIDtcIndexfileName; }
            set { INIDtcIndexfileName = value; }
        }

        public string Item_GEN11DtcIndexfileName
        {
            get { return GEN11DtcIndexfileName; }
            set { GEN11DtcIndexfileName = value; }
        }

        public string Item_GEN11GBDtcIndexfileName
        {
            get { return GEN11GBDtcIndexfileName; }
            set { GEN11GBDtcIndexfileName = value; }
        }

        public string Item_GEN11GEMDtcIndexfileName
        {
            get { return GEN11GEMDtcIndexfileName; }
            set { GEN11GEMDtcIndexfileName = value; }
        }

        //public string Item_GEN12DtcIndexfileName
        //{
        //    get { return GEN12DtcIndexfileName; }
        //    set { GEN12DtcIndexfileName = value; }
        //}

        //public string Item_GEN12GBDtcIndexfileName
        //{
        //    get { return GEN12GBDtcIndexfileName; }
        //    set { GEN12GBDtcIndexfileName = value; }
        //}

        //public string Item_GEN12GEMDtcIndexfileName
        //{
        //    get { return GEN12GEMDtcIndexfileName; }
        //    set { GEN12GEMDtcIndexfileName = value; }
        //}

        public string Item_CommLogName
        {
            get { return CommLogName; }
            set { CommLogName = value; }
        }

        public string Item_InsLogName
        {
            get { return InsLogName; }
            set { InsLogName = value; }
        }

        public string Item_CommLogBinName
        {
            get { return CommLogBinName; }
            set { CommLogBinName = value; }
        } 

        public string Item_ResultLogName
        {
            get { return ResultLogName; }
            set { ResultLogName = value; }
        }

        public string Item_ResultNGscreen
        {
            get { return ResultNGscreen; }
            set { ResultNGscreen = value; }
        }

        public string Item_History
        {
            get { return History; }
            set { History = value; }
        }        

        public string Item_DeviceName
        {
            get { return DeviceName; }
            set { DeviceName = value; }
        }

        public string Item_SystemFolder
        {
            get { return SystemFolder; }
            set { SystemFolder = value; }
        }

        public string Item_DataFolder
        {
            get { return DataFolder; }
            set { DataFolder = value; }
        }
        public string Item_MelsecLogName
        {
            get { return melsecLogName; }
            set { melsecLogName = value; }
        }


        public DK_LOGGER(string DeviceName, bool bDebugMode)
        {
            Item_DeviceName = DeviceName;
            //strProgramPath  = AppDomain.CurrentDomain.BaseDirectory;
            strLogPath  = @"C:\GMTELEMATICS\";
            strProgramPath = AppDomain.CurrentDomain.BaseDirectory;
            bDebug = bDebugMode;
            Initialize(bDebug);
        }

        private void CheckFilePath_Result(int iNameOption) 
        {
            CreateFolder(strLogPath + "RESULT\\" + DateTime.Now.ToString("yyyy-MM"));
            string strOption = "_FULL.CSV";

            switch (iNameOption)
            {
                case 1 : //NG
                        strOption = "_NG.CSV"; break;
                case 2 : //ETC
                        strOption = "_ETC.CSV"; break;
                default: //FULL
                        strOption = "_FULL.CSV"; break;
            }


            Item_ResultLogName = strLogPath + "RESULT\\" +
                                   DateTime.Now.ToString("yyyy-MM") + "\\RESULT_" +
                                   DateTime.Now.ToString("yyyyMMdd") + strOption;
        }

        private void CheckFilePath_Log()
        {
            if (bDebug)
            {
                CreateFolder(strLogPath + "LOG\\Debug\\" + Item_DeviceName + "\\" + DateTime.Now.ToString("yyyy-MM"));
                Item_DeviceName = DeviceName;
                Item_CommLogName = strLogPath + "LOG\\Debug\\" + Item_DeviceName + "\\" +
                               DateTime.Now.ToString("yyyy-MM") + "\\" + Item_DeviceName + "_" +
                               DateTime.Now.ToString("yyyyMMdd") + ".LOG";
            }
            else
            {
                CreateFolder(strLogPath + "LOG\\" + Item_DeviceName + "\\" + DateTime.Now.ToString("yyyy-MM"));
                Item_CommLogName = strLogPath + "LOG\\" + Item_DeviceName + "\\" +
                               DateTime.Now.ToString("yyyy-MM") + "\\" + Item_DeviceName + "_" +
                               DateTime.Now.ToString("yyyyMMdd") + ".LOG";
            }
        }

        private void CheckFilePath_InspectionLog(string strFileName)
        {
            CreateFolder(strLogPath + "LOG\\SET\\" + DateTime.Now.ToString("yyyy-MM"));
            Item_InsLogName = strLogPath + "LOG\\SET\\" +
                           DateTime.Now.ToString("yyyy-MM") + "\\" + strFileName + ".LOG";

        } 

        private void CheckFilePath_BinLog(string strFileName)
        {
            CreateFolder(strLogPath + "LOG\\BIN\\" + DateTime.Now.ToString("yyyy-MM"));
            Item_CommLogBinName = strLogPath + "LOG\\BIN\\" +
                            DateTime.Now.ToString("yyyy-MM") + "\\" + strFileName + ".bin";

        } 

        private void CheckFilePath_Screen()
        {
            CreateFolder(strLogPath + "SCREEN\\" + DateTime.Now.ToString("yyyyMM"));
            Item_ResultNGscreen = strLogPath + "SCREEN\\" +
                                   DateTime.Now.ToString("yyyyMM") + "\\";
        }

        private void CheckFilePath_MelsecLog()
        {
            CreateFolder(strLogPath + "LOG\\MELSEC\\" + DateTime.Now.ToString("yyyy-MM"));
            Item_MelsecLogName = strLogPath + "LOG\\MELSEC\\" +
                           DateTime.Now.ToString("yyyy-MM") + "\\" + "MELSEC _" +
                               DateTime.Now.ToString("yyyyMMdd") + ".LOG";
        }

        private void Initialize(bool bDebug)
        {

            Item_SystemFolder    = String.Empty;
            Item_INIfileName     = String.Empty;
            Item_SuffixFileName  = String.Empty;
            Item_ModelFileName   = String.Empty;
            Item_INIFreqfileName = String.Empty;
            Item_INIDtcIndexfileName = String.Empty;
            Item_GEN11DtcIndexfileName = String.Empty;
            Item_GEN11GBDtcIndexfileName = String.Empty;
            Item_GEN11GEMDtcIndexfileName = String.Empty;
            //Item_GEN12DtcIndexfileName = String.Empty;
            //Item_GEN12GBDtcIndexfileName = String.Empty;
            //Item_GEN12GEMDtcIndexfileName = String.Empty;
            Item_CommLogName     = String.Empty;     
            Item_ResultLogName   = String.Empty;
            Item_DataFolder      = String.Empty;

            //config 파일 path 저장.
            Item_SystemFolder    = strProgramPath + "SYSTEM\\";
            Item_INIfileName     = strProgramPath + "SYSTEM\\CONFIG.INI";
            Item_PWLISTfileName  = strProgramPath + "SYSTEM\\SECURE.INI";
            Item_SuffixFileName  = strProgramPath + "SYSTEM\\SUFFIX.INI";
            Item_ModelFileName   = strProgramPath + "SYSTEM\\MODEL.TBL";
            Item_INIFreqfileName = strProgramPath + "SYSTEM\\FREQUENCY.INI";
            Item_INIDtcIndexfileName = strProgramPath + "SYSTEM\\DTCTCPINDEX.INI";
            Item_GEN11DtcIndexfileName = strProgramPath + "SYSTEM\\DTCGEN11INDEX.INI";
            Item_GEN11GBDtcIndexfileName = strProgramPath + "SYSTEM\\DTCGEN11GBINDEX.INI";
            Item_GEN11GEMDtcIndexfileName = strProgramPath + "SYSTEM\\DTCGEN11GEMINDEX.INI";
            //Item_GEN12DtcIndexfileName = strProgramPath + "SYSTEM\\DTCGEN12INDEX.INI";
            //Item_GEN12GBDtcIndexfileName = strProgramPath + "SYSTEM\\DTCGEN12GBINDEX.INI";
            //Item_GEN12GEMDtcIndexfileName = strProgramPath + "SYSTEM\\DTCGEN12GEMINDEX.INI";
            Item_DataFolder      = strProgramPath + "DATA\\";
            Item_ResultLogName   = strLogPath + "RESULT\\" +
                                   DateTime.Now.ToString("yyyy-MM") + "\\RESULT_" +
                                   DateTime.Now.ToString("yyyyMMdd") + "_FULL.CSV";

            Item_ResultNGscreen  = strLogPath + "SCREEN\\" +
                                   DateTime.Now.ToString("yyyyMM") + "\\";

            //SYSTEM 폴더없으면 새로 생성.
            CreateFolder(strProgramPath + "SYSTEM");
            //CreateFolder(strLogPath + "RESULT");
            CreateFolder(strLogPath + "RESULT\\" + DateTime.Now.ToString("yyyy-MM"));

            //CreateFolder(strLogPath + "SCREEN");
            CreateFolder(strLogPath + "SCREEN\\" + DateTime.Now.ToString("yyyyMM"));

            CreateFolder(strProgramPath + "DATA");
            CreateFolder(strLogPath + "LOG");  
           
            if (bDebug)
            {
                //CreateFolder(strLogPath + "LOG\\Debug\\" + Item_DeviceName);
                CreateFolder(strLogPath + "LOG\\Debug\\" + Item_DeviceName + "\\" + DateTime.Now.ToString("yyyy-MM"));
                Item_DeviceName = DeviceName;
                Item_CommLogName = strLogPath + "LOG\\Debug\\" + Item_DeviceName + "\\" +
                               DateTime.Now.ToString("yyyy-MM") + "\\" + Item_DeviceName + "_" +
                               DateTime.Now.ToString("yyyyMMdd") + ".LOG";
            }
            else
            {
                //CreateFolder(strLogPath + "LOG\\" + Item_DeviceName);
                CreateFolder(strLogPath + "LOG\\" + Item_DeviceName + "\\" + DateTime.Now.ToString("yyyy-MM"));


                Item_CommLogName = strLogPath + "LOG\\" + Item_DeviceName + "\\" +
                               DateTime.Now.ToString("yyyy-MM") + "\\" + Item_DeviceName + "_" +
                               DateTime.Now.ToString("yyyyMMdd") + ".LOG";
            }

            //INI 파일없으면 새로 만들고 초기화.
            if (CreateFile(Item_INIfileName) == true)
            {
                CreateINI(); 
            }
            //INI 파일없으면 새로 만들고 초기화.
            if (CreateFile(Item_SuffixFileName) == true)
            {
                CreateSuffixFile();
            }

            CreateFile(Item_ResultLogName);
            CreateFile(Item_CommLogName);

            // 사용 예제
            // WriteResultLog("PASS");          //to RESULT_YYYYMMDD.CSV            
            // WriteCommLog("COMM!");           //to DEVICENAME_YYYYMMDD.LOG            
            // LoadFile("COMPORT", "DIO");      //from config.ini file            
            // SaveFile("COMPORT", "DIO", "3"); //to config.ini file
            
        }

        private void CreateINI()
        {
            //SaveINI("COMPORT", "DIO", "3");

        }

        private void CreateSuffixFile()
        {
            SaveSuffix("INFORMATION", "MODEL_COUNT", "0");
        }
            
        public void CreateFolder(string strPath){

            try
            {
                DirectoryInfo Di = new DirectoryInfo(strPath);

                if (Di.Exists == false) { Di.Create(); }
            }
            catch
            {
            	
            }
            
            
        }

        public bool CreateFile(string strName)
        {
            FileStream FS = null;

            if (!File.Exists(strName))
            {   //파일이 없으면 create mode
                try { 
                    FS = new FileStream(strName, FileMode.CreateNew);
                    FS.Close();
                    return true;
                }
                catch //(Exception e) 
                {
                    //MessageBox.Show("CreateFile Error : " + e.ToString());
                }
            }

            return false;
            //파일이 있으면 append (Seek to end)!
            //FS = new FileStream(strName, FileMode.Append);  
        }

        public void SaveINI(string strSubject, string strName, string strValue)
        {
            try
            {
                WritePrivateProfileString(strSubject, strName, strValue, Item_INIfileName);
            }
            catch //(Exception e)
            {
                //MessageBox.Show("SaveFile Error : " + e.ToString());
            }
        }
        //LGEVH 20230816
        public void SavePWINI(string strSubject, string strName, string strValue)
        {
            try
            {
                //WritePrivateProfileString(strSubject, strName, strValue, Item_INIfileName);

                WATCrypt m_crypt = new WATCrypt("11111111");
                string strPwdData = m_crypt.Encrypt(strValue).Trim();

                //SavePWLISTINI(strSubject + i.ToString(), "PW", pwuserdata[i].strPassword);
                WritePrivateProfileString(strSubject, strName, strPwdData, Item_INIfileName);
            }
            catch //(Exception e)
            {
                //MessageBox.Show("SaveFile Error : " + e.ToString());
            }
        }

        public void SaveSlot(string strSubject, string strName, string strValue)
        {
            try
            {
                WritePrivateProfileString(strSubject, strName, strValue, Item_INIfileName);
            }
            catch //(Exception e)
            {
                //MessageBox.Show("SaveSlot Error : " + e.ToString());
            }
        }

        public string GetConfigPath()
        {
            return Item_INIfileName;
        }

        public string LoadINI(string strSubject, string strName)
        {
            string rtnStr = "0";
            StringBuilder sbTemp = new StringBuilder(1024);
            
            int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, Item_INIfileName);
            
            if (i > 0)
            {
                rtnStr = sbTemp.ToString();
            }       
               
            return rtnStr;
        }
        //LGEVH 20230816
        public string LoadPWINI(string strSubject, string strName)
        {
            string rtnStr = "0";
            string strPassword = String.Empty;
            StringBuilder sbTemp = new StringBuilder(1024);

            int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, Item_INIfileName);

            if (i > 0)
            {
                rtnStr = sbTemp.ToString();
                //암호화 복호화 키 8글자 (필히 8자리여야 함)
                WATCrypt m_crypt = new WATCrypt("11111111");
                try
                {
                    strPassword = m_crypt.Decrypt(rtnStr).Trim('\0');
                }
                catch
                {
                    strPassword = rtnStr;
                }
            }

            return strPassword;
        }
        public string LoadDtcIndexINI(string strSubject, string strName)
        {
            string rtnStr = "0";
            StringBuilder sbTemp = new StringBuilder(1024);


            if (!IsExistFile(Item_INIDtcIndexfileName)) return rtnStr;
            try
            {
                int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, Item_INIDtcIndexfileName);

                if (i > 0)
                {
                    rtnStr = sbTemp.ToString();
                }
            }
            catch
            {
                rtnStr = "Excetion Error.";
            }


            return rtnStr;
        }

        public string LoadGen11DtcIndexINI(string strSubject, string strName, int iDtcFileType)
        {
            string rtnStr = "0";
            StringBuilder sbTemp = new StringBuilder(1024);

            string strDtcFileName = String.Empty;

            switch (iDtcFileType)
            {
                case (int)GEN11RESTYPE.DTCGEM:
                case (int)GEN11RESTYPE.DTCGEMBITS:
                case (int)GEN12RESTYPE.DTCGEM:
                case (int)GEN12RESTYPE.DTCGEMBITS:
                    strDtcFileName = Item_GEN11GEMDtcIndexfileName; break;
                case (int)GEN11RESTYPE.DTCGB:
                case (int)GEN11RESTYPE.DTCGBBITS:
                case (int)GEN12RESTYPE.DTCGB:
                case (int)GEN12RESTYPE.DTCGBBITS:
                    strDtcFileName = Item_GEN11GBDtcIndexfileName; break;
                case (int)GEN11RESTYPE.DTC:
                case (int)GEN11RESTYPE.DTCMANUAL:
                case (int)GEN11RESTYPE.DTCBITS:
                case (int)GEN12RESTYPE.DTC:
                case (int)GEN12RESTYPE.DTCMANUAL:
                case (int)GEN12RESTYPE.DTCBITS:
                default:
                    strDtcFileName = Item_GEN11DtcIndexfileName;
                    break;
            }

            if (!IsExistFile(strDtcFileName)) return rtnStr;
            try
            {
                int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, strDtcFileName);

                if (i > 0)
                {
                    rtnStr = sbTemp.ToString();
                }
            }
            catch
            {
                rtnStr = "Excetion Error.";
            }


            return rtnStr;
        }

        //public string LoadGen12DtcIndexINI(string strSubject, string strName, int iDtcFileType)
        //{
        //    string rtnStr = "0";
        //    StringBuilder sbTemp = new StringBuilder(1024);

        //    string strDtcFileName = String.Empty;

        //    switch (iDtcFileType)
        //    {
        //        case (int)GEN12RESTYPE.DTCGEM:
        //        case (int)GEN12RESTYPE.DTCGEMBITS:
        //            strDtcFileName = Item_GEN12GEMDtcIndexfileName; break;
        //        case (int)GEN12RESTYPE.DTCGB:
        //        case (int)GEN12RESTYPE.DTCGBBITS:
        //            strDtcFileName = Item_GEN12GBDtcIndexfileName; break;
        //        case (int)GEN12RESTYPE.DTC:
        //        case (int)GEN12RESTYPE.DTCMANUAL:
        //        case (int)GEN12RESTYPE.DTCBITS:
        //        default:
        //            strDtcFileName = Item_GEN12DtcIndexfileName;
        //            break;
        //    }

        //    if (!IsExistFile(strDtcFileName)) return rtnStr;
        //    try
        //    {
        //        int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, strDtcFileName);

        //        if (i > 0)
        //        {
        //            rtnStr = sbTemp.ToString();
        //        }
        //    }
        //    catch
        //    {
        //        rtnStr = "Excetion Error.";
        //    }


        //    return rtnStr;
        //}

        public bool LoadSlot(string strSubject, string strName)
        {
            bool rtnBool = false;
            StringBuilder sbTemp = new StringBuilder(255);
            int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 255, Item_INIfileName);

            if (i > 0)
            {
                if (sbTemp.ToString().Equals("True")) rtnBool = true;
                if (sbTemp.ToString().Equals("true")) rtnBool = true;
                if (sbTemp.ToString().Equals("TRUE")) rtnBool = true;
            }
            return rtnBool;
        }

        public void WriteCommLog(string strLog, string strTBLname, bool bSemiLog) 
        {
            FileStream FS   = null;
            StreamWriter SW = null;
            StringBuilder tmpStr = new StringBuilder(4096);
            try
            {
                CheckFilePath_Log();
                
                FS = new FileStream(Item_CommLogName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                if (!bSemiLog)
                {
                    tmpStr.Append("[");
                    tmpStr.Append(DateTime.Now.ToString("HH:mm:ss.ff"));
                    tmpStr.Append("][");
                    tmpStr.Append(strTBLname);
                    tmpStr.Append("]");
                    tmpStr.Append(strLog);
                    SW.WriteLine(tmpStr);                    
                }
                else
                {
                    SW.WriteLine(strLog);
                }
                SW.Close();
                FS.Close();

                if (!bSemiLog)
                {
                    if (SendTxRxEvent != null)
                        SendTxRxEvent(tmpStr.ToString());
                }
            }
            catch (Exception e) { string strEx = e.ToString(); }

            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }
        }

        public void WriteMelsecLog(string strLog, string strTBLname, bool bSemiLog)
        {
            FileStream FS = null;
            StreamWriter SW = null;
            StringBuilder tmpStr = new StringBuilder(4096);
            try
            {
                CheckFilePath_MelsecLog();

                FS = new FileStream(Item_MelsecLogName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                if (!bSemiLog)
                {
                    tmpStr.Append("[");
                    tmpStr.Append(DateTime.Now.ToString("HH:mm:ss.ff"));
                    tmpStr.Append("][");
                    tmpStr.Append(strTBLname);
                    tmpStr.Append("]");
                    tmpStr.Append(strLog);
                    SW.WriteLine(tmpStr);
                }
                else
                {
                    SW.WriteLine(strLog);
                }
                SW.Close();
                FS.Close();
            }
            catch (Exception e) { string strEx = e.ToString(); }

            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }
        }

        public void WriteTargetLog(string strLog, string strFullPath, string strFileName)
        {
            FileStream FS = null;
            StreamWriter SW = null;
            try
            {
                if (String.IsNullOrEmpty(strFullPath)) return;

                CreateFolder(strFullPath);

                FS = new FileStream(strFullPath + strFileName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);
                SW.WriteLine(strLog);
                SW.Close();
                FS.Close();
            }
            catch (Exception e) { string strEx = e.ToString(); }

            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }

        }
         
        public void WriteBinLogging(string[] strLog, string strFileName)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamWriter SW = null;

            try
            {
                CheckFilePath_BinLog(strFileName);

                FS = new FileStream(Item_CommLogBinName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                if (FS.Length > 0)
                {
                    SW.WriteLine("_______________________________________________________________________________________________________________________________________________");
                    SW.WriteLine("");
                }
                
                for (int i = 0; i < strLog.Length; i++)
                {
                    SW.WriteLine(strLog[i]);
                }

                SW.Close();
                FS.Close();
            }
            catch (Exception e) { string strEx = e.ToString(); }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }

        }

        public void WriteInspectionLogging(string[] strLog, string strFileName)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamWriter SW = null;

            try
            {
                CheckFilePath_InspectionLog(strFileName);

                FS = new FileStream(Item_InsLogName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                if (FS.Length > 0)
                {
                    SW.WriteLine("_______________________________________________________________________________________________________________________________________________");
                    SW.WriteLine("");
                }

                for (int i = 0; i < strLog.Length; i++)
                {
                    SW.WriteLine(strLog[i]);
                }

                SW.Close();
                FS.Close();
            }
            catch (Exception e) { string strEx = e.ToString(); }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }

        }

        public void WriteResultLog(string[] strLog, string strResultOption) 
        {
            WriteResultLogging(strLog, 0);
            switch (strResultOption)
            {
                case "OK": break;
                case "NG": WriteResultLogging(strLog, 1); break;                
                default:   WriteResultLogging(strLog, 2); break;
            }
        }

        private void WriteResultLogging(string[] strLog, int iResultOption)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamWriter SW = null;

            try
            {
                CheckFilePath_Result(iResultOption);

                FS = new FileStream(Item_ResultLogName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);
                for (int i = 0; i < strLog.Length; i++)
                {
                    SW.WriteLine(strLog[i]);
                }

                SW.Close();
                FS.Close();
            }
            catch (Exception e) { string strEx = e.ToString(); }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }

        }
        //CSMES
        public void WriteDetailDataLogging(string strLog)
        {
            string DetailLogFolder = strLogPath + "LOG\\STEP_COMPLETE_DETAIL\\" + DateTime.Now.ToString("yyyy-MM");
            CreateFolder(DetailLogFolder);

            string DetailLogFn = DetailLogFolder + "\\" + "DETAIL_" +
                               DateTime.Now.ToString("yyyyMMdd") + ".LOG";

            strLog += Environment.NewLine + Environment.NewLine;

            File.AppendAllText(DetailLogFn, strLog);
        }
        public string[] GetFileList(string strFormat)
        {            
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Item_DataFolder);
                FileInfo[] Fi = di.GetFiles("*." + strFormat);

                string[] rString = new string[Fi.Length];
                for (int i = 0; i < Fi.Length; i++)
                {
                    rString[i] = Fi[i].Name.ToString().Replace(Fi[i].Extension.ToString(), "") + Fi[i].Extension.ToString().ToUpper();
                }
                return rString;
            }
            catch (System.Exception ex)
            {
                string strEx = ex.ToString();
                string[] strRtn = null;                    
                return strRtn;
            }
            
        }

        public string GetLogPath()
        {
            return strLogPath;
        }

        public bool GetFolderList(string strPath, ref List<string> lstFolder)
        {
            try
            {   
                string strFolderPath = strLogPath + strPath;
                lstFolder.Clear();

                System.IO.DirectoryInfo Info = new System.IO.DirectoryInfo(strFolderPath);

                if (Info.Exists)
                {
                    System.IO.DirectoryInfo[] CInfo = Info.GetDirectories("*", System.IO.SearchOption.AllDirectories);

                    foreach (System.IO.DirectoryInfo info in CInfo)
                    {
                        lstFolder.Add(info.FullName.Replace(strFolderPath + @"\", String.Empty));
                    }
                    return true;
                }

                return false;
            }
            catch
            {
                lstFolder.Clear();
                return false;
            }

        }

        public bool GetGen10WLCommandList(string strFileName, ref List<string> rtnList)        
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamReader SR = null;
            bool bFlag = true;
            if (!File.Exists(Item_SystemFolder + "GEN10_WL_COMMANDS\\" + strFileName))
            {   //파일이 없으면 return!
                //essageBox.Show("File Not Found!");                
                return false;
            }
            try
            {
                FS = new FileStream(Item_SystemFolder + "GEN10_WL_COMMANDS\\" + strFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);

                string tmpString = String.Empty;
                rtnList.Clear();
                while (!SR.EndOfStream)
                {
                    tmpString = SR.ReadLine();
                    rtnList.Add(tmpString);
                }
                SR.Close();
                FS.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetGPIBList Error : " + ex.ToString());
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();
            }
            return bFlag;
        }

        public bool LoadAutoJob(ref List<string[]> rtnList)
        {
            //Prevent 2015.03.26 DK.SIM 
            rtnList.Clear();

            string[] strSplit;
            FileStream FS = null;
            StreamReader SR = null;
            bool bFlag = true;

            if (!File.Exists(Item_DataFolder + "AUTOJOB.MAP"))
            {   //파일이 없으면 return!
                CreateFile(Item_DataFolder + "AUTOJOB.MAP");
            }

            try
            {
                FS = new FileStream(Item_DataFolder + "AUTOJOB.MAP", FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);

                string tmpString = String.Empty;
                string strTag = "<" + (char)0x07 + ">";

                if (SR != null)
                {
                    while (!SR.EndOfStream)
                    {
                        tmpString = SR.ReadLine();
                        strSplit = System.Text.RegularExpressions.Regex.Split(tmpString, strTag);                        
                        tmpString = String.Empty;
                        rtnList.Add(strSplit);
                    }
                }

                SR.Close();
                FS.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadJob Error : " + ex.ToString());
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return bFlag;
        }

        public bool LoadJobNew(string strFileName, ref List<string[]> rtnList)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamReader SR = null;
            rtnList.Clear();
            
            bool bFlag = true;

            if (!File.Exists(Item_DataFolder + strFileName))
            {   //파일이 없으면 return!
                MessageBox.Show("File Not Found!");
                return false;
            }
            try
            {
                FS = new FileStream(Item_DataFolder + strFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);

                string tmpString = String.Empty;
                string strTag = "<" + (char)0x07 + ">";
                string[] strSplit;                
                if (SR != null)
                {
                    while (!SR.EndOfStream)
                    {
                        tmpString = SR.ReadLine();
                        strSplit = System.Text.RegularExpressions.Regex.Split(tmpString, strTag);                        
                        tmpString = String.Empty;
                        rtnList.Add(strSplit);
                    }
                }

                SR.Close();
                FS.Close();

                return bFlag;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadJob Error : " + ex.ToString());
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return bFlag;
        }

        public bool LoadTBL0(string strFileName, ref List<TBLDATA0> lstTBL0)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamReader SR = null;
            bool bFlag = true;
            if (!File.Exists(Item_SystemFolder + strFileName))
            {   //파일이 없으면 return!
                MessageBox.Show(strFileName + " File Not Found!");
                return false;
            }
            try
            {
                TBLDATA0 tmpTbl = new TBLDATA0();
                lstTBL0.Clear();
                FS = new FileStream(Item_SystemFolder + strFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);
                
                string tmpString;
                string tmpCMDNAME;
                string tmpSENDPAC;
                string tmpRECVPAC;
                string tmpPARPAC1;
                string tmpPARPAC2;
                string tmpOPTION1;
                string tmpOPTION2;
                tmpString = tmpCMDNAME = tmpSENDPAC = tmpRECVPAC = tmpPARPAC1 = tmpPARPAC2 = tmpOPTION1 = tmpOPTION2 = String.Empty;
                string strTag = "//";
          
                string[] strSplit;
                
                while (!SR.EndOfStream)
                {
                    tmpString = SR.ReadLine();
                    strSplit = System.Text.RegularExpressions.Regex.Split(tmpString, strTag);

                    if (strSplit[0].Contains("!") || tmpString == "" || tmpString.Contains(";;"))
                    {
                        continue;
                    }

                    for (int i = 0; i < strSplit.Length; i++)
                    {
                        if (strSplit[i].Contains("\t"))
                            strSplit[i] = strSplit[i].Replace("\t", String.Empty);
                    }

                    switch (strSplit.Length)
                    {
                        case 1: tmpCMDNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpSENDPAC = String.Empty;
                            tmpRECVPAC = String.Empty;
                            tmpPARPAC1 = String.Empty;
                            tmpPARPAC2 = String.Empty;
                            tmpOPTION1 = String.Empty;
                            tmpOPTION2 = String.Empty;
                            break;
                        case 2: tmpCMDNAME = strSplit[0];
                            MessageBox.Show("Can not Load the file. Because the file format or extension is invalid. ( NAME:" + strSplit[0] + " )");
                            //Prevent 2015.03.26 DK.SIM 
                            if (SR != null) ((IDisposable)SR).Dispose();
                            if (FS != null) ((IDisposable)FS).Dispose();
                            return false;
                        case 3: tmpCMDNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpSENDPAC = strSplit[1];
                            tmpRECVPAC = strSplit[2];
                            tmpPARPAC1 = String.Empty;
                            tmpPARPAC2 = String.Empty;
                            tmpOPTION1 = String.Empty;
                            tmpOPTION2 = String.Empty;
                            break;
                        case 4: tmpCMDNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpSENDPAC = strSplit[1];
                            tmpRECVPAC = strSplit[2];
                            tmpPARPAC1 = strSplit[3];
                            tmpPARPAC2 = String.Empty;
                            tmpOPTION1 = String.Empty;
                            tmpOPTION2 = String.Empty;
                            break;
                        case 5: tmpCMDNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpSENDPAC = strSplit[1];
                            tmpRECVPAC = strSplit[2];
                            tmpPARPAC1 = strSplit[3];
                            tmpPARPAC2 = strSplit[4];
                            tmpOPTION1 = String.Empty;
                            tmpOPTION2 = String.Empty;
                            break;
                        case 6: tmpCMDNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpSENDPAC = strSplit[1];
                            tmpRECVPAC = strSplit[2];
                            tmpPARPAC1 = strSplit[3];
                            tmpPARPAC2 = strSplit[4];
                            tmpOPTION1 = strSplit[5];
                            tmpOPTION2 = String.Empty;
                            break;
                        case 7: tmpCMDNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpSENDPAC = strSplit[1];
                            tmpRECVPAC = strSplit[2];
                            tmpPARPAC1 = strSplit[3];
                            tmpPARPAC2 = strSplit[4];
                            tmpOPTION1 = strSplit[5];
                            tmpOPTION2 = strSplit[6];
                            break;

                        default: MessageBox.Show(strFileName + " - Can not Load the file. Because the file format or extension is invalid. ( TOKEN COUNT : " + strSplit.Length.ToString() + " )");
                            return false;

                    }
                    tmpTbl.CMDNAME = tmpCMDNAME;
                    tmpTbl.SENDPAC = tmpSENDPAC;
                    tmpTbl.RECVPAC = tmpRECVPAC;
                    tmpTbl.PARPAC1 = tmpPARPAC1;
                    tmpTbl.PARPAC2 = tmpPARPAC2;
                    tmpTbl.OPTION1 = tmpOPTION1;
                    tmpTbl.OPTION2 = tmpOPTION2;
                    lstTBL0.Add(tmpTbl);
                    tmpString = String.Empty;
                }

                SR.Close();
                FS.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadTBL0 Error : " + ex.ToString());
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return bFlag;
        }

        public bool LoadDTCTBL(string strFileName, ref List<DTCDATA0> lstTBL0)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamReader SR = null;
            bool bFlag = true;
            if (!File.Exists(Item_SystemFolder + strFileName))
            {   //파일이 없으면 return!
                MessageBox.Show(strFileName + " File Not Found!");
                return false;
            }
            try
            {
                DTCDATA0 tmpTbl = new DTCDATA0();
                lstTBL0.Clear();
                FS = new FileStream(Item_SystemFolder + strFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);

                string tmpString;
                string tmpDTCNAME;
                string tmpDTCCODE;

                tmpString = tmpDTCNAME = tmpDTCCODE = String.Empty;
                string strTag = "//";

                string[] strSplit;

                while (!SR.EndOfStream)
                {
                    tmpString = SR.ReadLine();
                    strSplit = System.Text.RegularExpressions.Regex.Split(tmpString, strTag);

                    if (strSplit.Length == 1)
                    {
                        tmpDTCNAME = strSplit[0].Replace(" ", String.Empty);
                        tmpDTCNAME = tmpDTCNAME.Replace("\t", String.Empty);
                        tmpDTCCODE = String.Empty;

                    }
                    else if(strSplit.Length == 2)
                    {                     
                            tmpDTCNAME = strSplit[0].Replace(" ", String.Empty);
                            tmpDTCNAME = tmpDTCNAME.Replace("\t", String.Empty);  
                            tmpDTCCODE = strSplit[1].Replace(" ", String.Empty);
                            tmpDTCCODE = tmpDTCCODE.Replace("\t", String.Empty);
                            tmpDTCCODE = tmpDTCCODE.Replace("0x", String.Empty); 
                            
                    }

                    tmpTbl.DTCNAME = tmpDTCNAME;
                    tmpTbl.DTCCODE = tmpDTCCODE;
                    lstTBL0.Add(tmpTbl);
                    tmpString = String.Empty;
                }

                SR.Close();
                FS.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadDTCTBL Error : " + ex.Message);
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return bFlag;
        }

        public bool LoadStepJobs(string strFileName, ref List<JOBFILES> returnList) //StepManager, EDIT 에서만 쓰임.
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamReader SR = null;

            bool bFlag = true;
            returnList.Clear();

            if (!File.Exists(Item_DataFolder + strFileName))
            {   //파일이 없으면 return!
                MessageBox.Show("File Not Found!");
                return false;
            }
            try
            {
                FS = new FileStream(Item_DataFolder + strFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);


                string tmpString;
                string strTag = "<" + (char)0x07 + ">";
                string strChksumTag = String.Empty + (char)0x11 + (char)0x08;

                List<string[]> tmpList = new List<string[]>();
                bool bReWrite = false;
                while (!SR.EndOfStream)
                {
                    JOBFILES tmpJobs = new JOBFILES();
                    tmpJobs.strChkSum = "NONE";

                    tmpString = SR.ReadLine();

                    string[] strTemp = System.Text.RegularExpressions.Regex.Split(tmpString, strTag);


                    if (strTemp[strTemp.Length - 1].Contains(strChksumTag))
                    {
                        string strReplace = strTemp[strTemp.Length - 1].Replace(strChksumTag, String.Empty);
                        if (!String.IsNullOrEmpty(strReplace))
                            tmpJobs.strChkSum = strReplace;
                        else
                            tmpJobs.strChkSum = "FFFF";

                        tmpJobs.strJOB = strTemp;
                    }
                    else
                    {
                        tmpJobs.strChkSum = JOB_CRC(strTemp);
                        string[] strReMake = new string[strTemp.Length + 1];
                        Array.Copy(strTemp, strReMake, strTemp.Length);
                        strReMake[strReMake.Length - 1] = strChksumTag + tmpJobs.strChkSum;
                        tmpJobs.strJOB = strReMake;
                        if (!strTemp[0].Equals("TYPE"))
                            bReWrite = true;
                    }

                    returnList.Add(tmpJobs);

                    tmpString = String.Empty;
                    tmpList.Add(tmpJobs.strJOB);
                }
                SR.Close();
                FS.Close();

                if (tmpList.Count > 1 && bReWrite)
                {
                    ClearJob(strFileName);
                    for (int i = 1; i < tmpList.Count; i++)
                    {
                        SaveJob(strFileName, tmpList[i]);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadStepJob Error : " + ex.ToString());
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();
            }
            return bFlag;

        }
        
        public bool ClearJob(string strFileName)
        {
            //Prevent 2015.03.26 DK.SIM  
            FileStream   FS = null;
            StreamWriter SW = null;
         
            if (!File.Exists(Item_DataFolder + strFileName))
            {   
                try
                {
                    FS = new FileStream(Item_DataFolder + strFileName, FileMode.CreateNew);
                    FS.Close();
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("ClearJob Error1 : " + e.ToString());
                }
                //Prevent 2015.03.26 DK.SIM
                finally
                {                    
                    if (FS != null) ((IDisposable)FS).Dispose();
                }
            }
            else
            {                
                try
                {
                    //TYPE<>CMD<>DISPLAYNAME<>MESCODE<>ACTION<>LABEL<>LABELCOUNT<>CASENG<>DELAY<>TIMEOUT<>RETRY<>COMPARE<>MIN<>MAX<>OPTION<>PAR1<>DOC<>EXPR
                    string strTag = "" + (char)0x3c + (char)0x07 + (char)0x3E;
                    string strTitle = "TYPE" + strTag + "CMD" + strTag + "DISPLAYNAME" + strTag + "MESCODE" + strTag +
                                      "ACTION" + strTag + "LABEL" + strTag + "LABELCOUNT" + strTag + "CASENG" + strTag + 
                                      "DELAY" + strTag + "TIMEOUT" + strTag +
                                      "RETRY" + strTag + "COMPARE" + strTag + "MIN" + strTag + "MAX" + strTag + "OPTION" +
                                      strTag + "PAR1" + strTag + "DOC" + strTag + "EXPR";
                    FS = new FileStream(Item_DataFolder + strFileName, FileMode.Create);
                    SW = new StreamWriter(FS, System.Text.Encoding.UTF8);
                    SW.WriteLine(strTitle);

                    SW.Close();
                    FS.Close();

                    return true;
                }
                catch 
                {
                    MessageBox.Show("ClearJob Error2 : PLEASE, TRY AGAIN! "); // + e.ToString());
                }
                //Prevent 2015.03.26 DK.SIM
                finally
                {
                    if (SW != null) ((IDisposable)SW).Dispose();
                    if (FS != null) ((IDisposable)FS).Dispose();

                }
                
            }
            
            return true;
        }

        //JOB 초기화
        public bool SaveAutoJob(List<string[]> lstData)
        {
            FileStream FS = null;
            StreamWriter SW = null;

            try
            {
                FS = new FileStream(Item_DataFolder + "AUTOJOB.MAP", FileMode.Create);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                string strTag = "<" + (char)0x07 + ">";
                string strTitle = String.Empty;

                for (int j = 0; j < lstData.Count; j++)
                {
                    strTitle = String.Empty;
                    for (int i = 0; i < lstData[j].Length; i++)
                    {
                        //Prevent 2015.03.26 DK.SIM  
                        lstData[j][i] = lstData[j][i].Trim();

                        if (i == lstData[j].Length - 1)
                        {
                            strTitle += lstData[j][i];
                        }
                        else
                        {
                            strTitle += (lstData[j][i] + strTag);
                        }
                    }
                    SW.WriteLine(strTitle);
                }


                SW.Close();
                FS.Close();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("SaveAutoJob Error : " + e.ToString());
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SW != null) ((IDisposable)SW).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return true;
        }

        //JOB 초기화
        public bool SaveJob(string strFileName, string[] strData)
        {
            //Prevent 2015.03.26 DK.SIM  
            FileStream FS = null;
            StreamWriter SW = null;

            try
            {
                //TYPE<>CMD<>DISPLAYNAME<>MESCODE<>ACTION<>LABEL<>LABELCOUNT<>CASENG<>DELAY<>TIMEOUT<>RETRY<>COMPARE<>MIN<>MAX<>OPTION<>PAR1<>DOC<>EXPR
                string strTag = "<" + (char)0x07 + ">";
                string strTitle = String.Empty;
                
                for (int i = 0; i < strData.Length; i++)
                {
                    //Prevent 2015.03.26 DK.SIM  
                    strData[i] = strData[i].Trim();                    

                    if (i == strData.Length - 1)
                    {
                        strTitle += strData[i];
                    }
                    else
                    {
                        strTitle += (strData[i] + strTag);
                    }
                    
                }

                
                FS = new FileStream(Item_DataFolder + strFileName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                SW.WriteLine(strTitle);
                SW.Close();
                FS.Close();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("SaveJob Error : " + e.ToString());
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SW != null) ((IDisposable)SW).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return true;
        }

        //JOB 파일 CSV 저장기능
        public bool SaveCSV(string strFileName, string[] strData)
        {
            //Prevent 2015.03.26 DK.SIM  
            FileStream FS = null;
            StreamWriter SW = null;
            bool bSaveSuccess = true;
            try
            {
                //TYPE<>CMD<>DISPLAYNAME<>MESCODE<>ACTION<>LABEL<>LABELCOUNT<>CASENG<>DELAY<>TIMEOUT<>RETRY<>COMPARE<>MIN<>MAX<>OPTION<>PAR1<>DOC<>EXPR
                string strTag = ",";
                string strTitle = String.Empty;

                for (int i = 0; i < strData.Length; i++)
                {
                    //Prevent 2015.03.26 DK.SIM  
                    strData[i] = strData[i].Replace(',', '/');
                    strData[i] = strData[i].Trim();

                    if (i == strData.Length - 1)
                    {
                        strTitle += strData[i];
                    }
                    else
                    {
                        strTitle += (strData[i] + strTag);
                    }

                }


                FS = new FileStream(Item_DataFolder + strFileName, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                SW.WriteLine(strTitle);
                SW.Close();
                FS.Close();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("SaveJob Error : " + e.ToString());
                bSaveSuccess = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SW != null) ((IDisposable)SW).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();
                

            }
            return bSaveSuccess;
        }

        public bool DeleteJob(string strFileName)
        {
            if (File.Exists(Item_DataFolder + strFileName)) //파일이 있으면 삭제
            {
                try
                {
                    FileInfo sfile = new FileInfo(Item_DataFolder + strFileName);
                    sfile.Delete();
                    sfile = null;
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("DeleteJob Error : " + e.ToString());
                }
            }
            return false;
        }

        public bool IsExistFile(string strFileName)
        {
            if (File.Exists(strFileName)) //파일이 있으면 //Item_DataFolder
            {
                return true;
            }
            return false;
        }

        public bool NewJob(string strFileName)
        {
            //Prevent 2015.03.26 DK.SIM  
            FileStream FS = null;
            StreamWriter SW = null;

            if (File.Exists(Item_DataFolder + strFileName)) //파일이 있으면 불가.
            {
                return false;
            }
            else{
                try
                {
                    //TYPE<>CMD<>DISPLAYNAME<>MESCODE<>ACTION<>LABEL<>LABELCOUNT<>CASENG<>DELAY<>TIMEOUT<>RETRY<>COMPARE<>MIN<>MAX<>OPTION<>PAR1<>DOC<>EXPR
                    string strTag = "" + (char)0x3c + (char)0x07 + (char)0x3E;
                    string strTitle = "TYPE" + strTag + "CMD" + strTag + "DISPLAYNAME" + strTag + "MESCODE" + strTag +
                                      "ACTION" + strTag + "LABEL" + strTag + "LABELCOUNT" + strTag + "CASENG" + strTag + 
                                      "DELAY" + strTag + "TIMEOUT" + strTag +
                                      "RETRY" + strTag + "COMPARE" + strTag + "MIN" + strTag + "MAX" + strTag + "OPTION" +
                                      strTag + "PAR1" + strTag + "DOC" + strTag + "EXPR";
                    FS = new FileStream(Item_DataFolder + strFileName, FileMode.Create);
                    SW = new StreamWriter(FS, System.Text.Encoding.UTF8);
 
                    SW.WriteLine(strTitle);

                    SW.Close();
                    FS.Close();

                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("NewJob Error : " + e.ToString());
                }
                //Prevent 2015.03.26 DK.SIM
                finally
                {
                    if (SW != null) ((IDisposable)SW).Dispose();
                    if (FS != null) ((IDisposable)FS).Dispose();

                }
            }
            return false;
        }

        public bool CopyCheck(string strOrginFileName, string strNewFileName)
        {
            //Prevent 2015.03.26 DK.SIM     

            if (!File.Exists(Item_DataFolder + strOrginFileName)) //기존 파일이 없으면 불가.
            {
                return false;
            }
            else
            {
                if (File.Exists(Item_DataFolder + strNewFileName)) //새로 만들어질 파일이 있으면 불가.
                {
                    return false;
                }
                else
                {
                    File.Copy(Item_DataFolder + strOrginFileName, Item_DataFolder + strNewFileName);
                }
            }
            return true;
        }

        public void PrintScreenResult(string strWipId)
        {
            try
            {
                CheckFilePath_Screen();

                Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                Graphics graphics = Graphics.FromImage(printscreen as Image);

                graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

                string strFileName = Item_ResultNGscreen + DateTime.Now.ToString("MMdd-HHmmss") + "_" + strWipId + ".JPG";
                printscreen.Save(strFileName, ImageFormat.Jpeg);

            }
            catch
            {

            }
        }

        public bool GetFileSize(string strFileName, ref string strFileSize)
        {
            //MD5 버젼에서 걍path 가 바뀌는 문제로 인해 아래와 같이 변경
            try
            {
                if (File.Exists(strProgramPath + strFileName)) //파일이 있으면 
                {
                    FileInfo fInfo = new FileInfo(strProgramPath + strFileName);
                    strFileSize = fInfo.Length.ToString();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

            /*  // 기존코드
            if (File.Exists(strProgramPath + strFileName)) //파일이 있으면 
            {
                FileInfo fInfo = new FileInfo(strFileName);
                strFileSize = fInfo.Length.ToString();
                return true;
            }
            return false;
            */
        }

        public bool DeleteScreen()
        {
            //디렉토리 리스트 가져오기
            string strDataSubString = String.Empty;
            System.IO.DirectoryInfo Info = new System.IO.DirectoryInfo(strLogPath + "SCREEN\\");

            if (Info.Exists)
            {
                System.IO.DirectoryInfo[] CInfo = Info.GetDirectories("*", System.IO.SearchOption.AllDirectories);

                foreach (System.IO.DirectoryInfo info in CInfo)
                {
                    strDataSubString = info.FullName;
                    try
                    {
                        string strOldFolderYear = strDataSubString.Substring(strDataSubString.Length - 6, 4);
                        string strOldFolderMonth = strDataSubString.Substring(strDataSubString.Length - 2, 2);
                        string strOldFolder = strOldFolderYear + "-" + strOldFolderMonth + "-01";

                        DateTime cDt1 = DateTime.Now;
                        DateTime cDt2 = DateTime.Parse(strOldFolder);
                        TimeSpan tsCurrent = cDt1 - cDt2;
                        int iDiffDays = tsCurrent.Days;

                        if (iDiffDays >= 120) //약3개월 (120일) 이전자료까지만 보관하며 그 보다 더 이전 폴더는 삭제
                        {
                            System.IO.Directory.Delete(strDataSubString, true);
                        }

                    }
                    catch
                    {
                        continue;
                    }

                }
            }

            return true;

        }

        //파일변경.

        public void ChangeUpdateFile()
        {
            //기본폴더
            RenameFiles(strProgramPath);
            //SYSTEM 폴더
            RenameFiles(Item_SystemFolder);
            //DATA 폴더.
            RenameFiles(Item_DataFolder);

        }

        private void RenameFiles(string strFilePath)
        {
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strFilePath);
                FileInfo[] Fi = di.GetFiles("*_DKUPDATE");

                string[] rString = new string[Fi.Length];
                for (int i = 0; i < Fi.Length; i++)
                {
                    rString[i] = strFilePath + Fi[i].ToString();
                }

                if (rString.Length > 0)
                {

                    string[] strOriginFiles = new string[rString.Length];

                    for (int i = 0; i < Fi.Length; i++)
                    {
                        strOriginFiles[i] = rString[i].Replace("_DKUPDATE", String.Empty);
                        if (IsExistFile(strOriginFiles[i]))
                        {
                            File.Move(strOriginFiles[i], strOriginFiles[i] + "_OLDFILE");
                        }

                        if (IsExistFile(rString[i]))
                        {
                            File.Move(rString[i], rString[i].Replace("_DKUPDATE", String.Empty));
                        }

                    }

                }
            }
            catch (System.Exception ex)
            {
                string strEx = ex.ToString();
            }
        }

        public void DeleteOldFile()
        {
            //기본폴더
            DeleteFiles(strProgramPath);
            //SYSTEM 폴더
            DeleteFiles(Item_SystemFolder);
            //DATA 폴더.
            DeleteFiles(Item_DataFolder);

        }

        private void DeleteFiles(string strFilePath)
        {
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strFilePath);
                FileInfo[] Fi = di.GetFiles("*_OLDFILE");

                string[] rString = new string[Fi.Length];
                for (int i = 0; i < Fi.Length; i++)
                {
                    rString[i] = strFilePath + Fi[i].ToString();
                    if (IsExistFile(rString[i]))
                    {
                        File.Delete(rString[i]);
                    }
                }

            }
            catch (System.Exception ex)
            {
                string strEx = ex.ToString();
            }
        }

        public byte[] FileToBinary(string strFileName)
        {
            FileStream FS = null;
            BinaryReader BR = null;
            byte[] returnArray;
            try
            {
                FS = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
                BR = new BinaryReader(FS);

                returnArray = BR.ReadBytes((int)FS.Length);

                BR.Close();
                FS.Close();
                return returnArray;

            }
            catch (Exception e) { string strEx = e.ToString(); }

            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (BR != null) ((IDisposable)BR).Dispose();
                returnArray = new byte[1];

            }

            return returnArray;

        }

        public void SetEnvironmentOraRegister()
        {
            try
            {   //환경변수 변경할때
                /*string strOriginPath = LoadINI("OPTION", "SYSTEM_PATH");
                if (String.IsNullOrEmpty(strOriginPath) || strOriginPath.Equals("0") || strOriginPath.Length < 2)
                {
                    strOriginPath = GetEnvironmentOraRegister("Path");
                    if (!String.IsNullOrEmpty(strOriginPath) && !strOriginPath.Equals(Environment.CurrentDirectory))
                    {
                        SaveINI("OPTION", "SYSTEM_PATH", strOriginPath);
                    }
                }*/

                //EnvironmentVariableTarget.Machine 으로 하면 실제 환경변수를 변경하게 된다.
                //EnvironmentVariableTarget.Process 으로 하면 프로그램 내에 블록에서만 환경변수를 변경하게 된다.
                Environment.SetEnvironmentVariable("NLS_LANG", "KOREAN_KOREA.KO16MSWIN949", EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("Path", Environment.CurrentDirectory, EnvironmentVariableTarget.Process);

            }
            catch
            {

            }

        }

        //네트워크 핑테스트용 ㅋㅋ
        public bool NetworkPingTest(string strIpaddress, int iTimeOut, ref string strResult, ref int iTTL, ref long lTime)
        {
            try
            {                
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
                options.DontFragment = true;
                string data = "ABCDE12345ABCDE12345ABCDE1234500";
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(data);
                int timeout = (iTimeOut * 1000);                

                System.Net.NetworkInformation.PingReply reply = ping.Send(strIpaddress/*System.Net.IPAddress.Parse(strIpaddress)*/, timeout, buffer, options);


                switch (reply.Status)
                {
                    case System.Net.NetworkInformation.IPStatus.Success:
                            iTTL = reply.Options.Ttl;
                            lTime = reply.RoundtripTime;
                            strResult = "Reply from " +  strIpaddress + ": bytes=" + reply.Buffer.Length.ToString() + " time=" + lTime.ToString() + "ms TTL=" + iTTL.ToString();
                            break;
                    case System.Net.NetworkInformation.IPStatus.TimedOut:
                            strResult = "Time Out - Destinateion host unreachable.";
                            break;
                    default:
                            strResult = "Failure - Destinateion host unreachable.";
                            break;
                }

                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                else
                    return false;
            }

            catch(Exception ex)
            {
                try
                {
                    strResult = "Error - " + ex.InnerException.Message.ToString();
                }
                catch 
                {
                    strResult = "Error - " + ex.Message.ToString();
                }
                
                return false;
            }
        }
                
        public void SaveSuffix(string strSubject, string strName, string strValue)
        {
            try
            {
                WritePrivateProfileString(strSubject, strName, strValue, Item_SuffixFileName);
            }
            catch //(Exception e)
            {
                //MessageBox.Show("SaveFile Error : " + e.ToString());
            }
        }

        public bool SaveModelFile(string[] strData, bool bClear)
        {
            //Prevent 2015.03.26 DK.SIM  
            FileStream FS = null;
            StreamWriter SW = null;
            bool bSaveSuccess = true;
            try
            {
                string strTag = "<>";
                string strTitle = String.Empty;

                for (int i = 0; i < strData.Length; i++)
                {
                    strData[i] = strData[i].Trim();

                    if (i == strData.Length - 1)
                    {
                        strTitle += strData[i];
                    }
                    else
                    {
                        strTitle += (strData[i] + strTag);
                    }

                }

                if (bClear)
                    FS = new FileStream(Item_ModelFileName, FileMode.Create );
                else
                    FS = new FileStream(Item_ModelFileName, FileMode.Append);

                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                SW.WriteLine(strTitle);
                SW.Close();
                FS.Close();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("SaveModelFile Error : " + e.ToString());
                bSaveSuccess = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SW != null) ((IDisposable)SW).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();


            }
            return bSaveSuccess;
        }

        public bool LoadModelFile(ref List<TBLMODEL> lstTBL0)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamReader SR = null;
            bool bFlag = true;
            if (!File.Exists(Item_ModelFileName))
            {   //파일이 없으면 return!                
                return false;
            }
            try
            {
                TBLMODEL tmpTbl = new TBLMODEL();
                lstTBL0.Clear();
                FS = new FileStream(Item_ModelFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);

                string tmpString;
                string NAME;
                string PN;
                string BTWIFI;
                string StidMin;
                string StidMax;
                tmpString = NAME = PN = BTWIFI = StidMin = StidMax = String.Empty;
                string strTag = "<>";

                string[] strSplit;

                while (!SR.EndOfStream)
                {
                    tmpString = SR.ReadLine();
                    strSplit = System.Text.RegularExpressions.Regex.Split(tmpString, strTag);

                    for (int i = 0; i < strSplit.Length; i++)
                    {
                        if (strSplit[i].Contains("\t"))
                            strSplit[i] = strSplit[i].Replace("\t", String.Empty);
                    }

                    switch (strSplit.Length)
                    {                        
                        case 5:
                            NAME    = strSplit[0].Trim();
                            PN      = strSplit[1].Trim();
                            BTWIFI  = strSplit[2].Trim();
                            StidMin = strSplit[3].Trim();
                            StidMax = strSplit[4].Trim();
                            break;
                 
                        default: 
                            return false;

                    }
                    tmpTbl.NAME     = NAME;
                    tmpTbl.PN       = PN;
                    tmpTbl.BTWIFI   = BTWIFI;
                    tmpTbl.StidMin  = StidMin;
                    tmpTbl.StidMax  = StidMax;
                    lstTBL0.Add(tmpTbl);
                    tmpString = String.Empty;
                }

                SR.Close();
                FS.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadTBL0 Error : " + ex.ToString());
                bFlag = false;
            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return bFlag;
        }
        
        public string LoadSuffix(string strSubject, string strName)
        {
            string rtnStr = "0";
            StringBuilder sbTemp = new StringBuilder(1024);

            int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, Item_SuffixFileName);

            if (i > 0)
            {
                rtnStr = sbTemp.ToString();
            }

            return rtnStr;
        }

        public void ClearSuffix()
        {
            //Prevent 2015.03.26 DK.SIM  
            FileStream FS = null;
            try
            {
                FS = new FileStream(Item_SuffixFileName, FileMode.Create);
                FS.Close();
                return;
            }
            catch
            {

            }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
            }

        }
        /*
        public void RestoreEnvironmentOraRegister()
        {    //환경변수  다시 복구할때
            string strOriginPath = LoadINI("OPTION", "SYSTEM_PATH");
            if (!String.IsNullOrEmpty(strOriginPath) && !strOriginPath.Equals("0"))
            {
                strOriginPath = strOriginPath.Replace(@"\\", @"\");
                Environment.SetEnvironmentVariable("Path", strOriginPath, EnvironmentVariableTarget.Machine );    
            }

        }*/

        private void CheckFilePath_History()
        {
            CreateFolder(strLogPath + "HISTORY\\" + DateTime.Now.ToString("yyyyMM"));
            Item_History = strLogPath + "HISTORY\\" +
                                   DateTime.Now.ToString("yyyyMM") + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_EDIT.LOG";

        }

        public void WriteEditHistory(string strEditFile, string strUserName, string strAction)
        {
            //Prevent 2015.03.26 DK.SIM 
            FileStream FS = null;
            StreamWriter SW = null;
            StringBuilder tmpStr = new StringBuilder(4096);

            try
            {
                CheckFilePath_History();

                FS = new FileStream(Item_History, FileMode.Append);
                SW = new StreamWriter(FS, System.Text.Encoding.UTF8);

                tmpStr.Append("[");
                tmpStr.Append(DateTime.Now.ToString("HH:mm:ss.ff"));
                tmpStr.Append("][");
                tmpStr.Append(strUserName);
                tmpStr.Append("][");
                tmpStr.Append(strEditFile);
                tmpStr.Append("][");
                tmpStr.Append(strAction);
                tmpStr.Append("]");

                SW.WriteLine(tmpStr);


                SW.Close();
                FS.Close();
            }
            catch (Exception e) { string strEx = e.ToString(); }
            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (SW != null) ((IDisposable)SW).Dispose();
            }

        }

        public string GetAddUserName()
        {
            string strReturnName = String.Empty;

            strReturnName = "USER_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return strReturnName;

        }

        private void InitialPWList()
        {
            SavePWLISTINI("PASSWORD_MANAGEMENT", "COUNT", "0");

            string strSubject = "PASSWORD_USERLIST";
            for (int i = 0; i < 20; i++)
            {
                SavePWLISTINI(strSubject + i.ToString(), "NAME",   "0");
                SavePWLISTINI(strSubject + i.ToString(), "PW",     "0");
                SavePWLISTINI(strSubject + i.ToString(), "EDIT",   "0");
                SavePWLISTINI(strSubject + i.ToString(), "JOB",    "0");
                SavePWLISTINI(strSubject + i.ToString(), "CONFIG", "0");
                SavePWLISTINI(strSubject + i.ToString(), "MES",    "0");
                //LGEVH
                SavePWLISTINI(strSubject + i.ToString(), "AUTOJOBCONFIG", "0");
            }
        }

        public int GetPasswordUserCount()
        {
            if(IsExistFile(Item_PWLISTfileName))
            {
                string strPasswordUserListCount = String.Empty;
                strPasswordUserListCount = LoadPWLISTINI("PASSWORD_MANAGEMENT", "COUNT");
                if (strPasswordUserListCount.Equals("0"))
                {
                    InitialPWList();
                    return 0;
                }
                else
                {
                    try
                    {
                        return int.Parse(strPasswordUserListCount);
                    }
                    catch 
                    {
                	    InitialPWList();
                        return 0;
                    }
                }
            }
            else
            {
                InitialPWList();
                return 0;
            }
            
            
        }

        public PWUSER GetPasswordUserData(int iDx)
        {
            PWUSER res = new PWUSER();

            if (GetPasswordUserCount() == 0) return res;

            string strSubject = "PASSWORD_USERLIST";
       
            res.strLogName = LoadPWLISTINI(strSubject + iDx.ToString(), "NAME");
            //res.strPassword = LoadPWLISTINI(strSubject + iDx.ToString(), "PW");

            //LGEVH 202306
            string strPwdData = LoadPWLISTINI(strSubject + iDx.ToString(), "PW");
            //암호화 복호화 키 8글자 (필히 8자리여야 함)
            WATCrypt m_crypt = new WATCrypt("11111111");
            try
            {
                res.strPassword = m_crypt.Decrypt(strPwdData).Trim('\0');
            }
            catch
            {
                res.strPassword = strPwdData;
            }
            if (LoadPWLISTINI(strSubject + iDx.ToString(), "EDIT").Equals("0"))
                res.bEdit = false;
            else
                res.bEdit = true;

            if (LoadPWLISTINI(strSubject + iDx.ToString(), "JOB").Equals("0"))
                res.bJob = false;
            else
                res.bJob = true;

            if (LoadPWLISTINI(strSubject + iDx.ToString(), "CONFIG").Equals("0"))
                res.bConfig = false;
            else
                res.bConfig = true;

            if (LoadPWLISTINI(strSubject + iDx.ToString(), "MES").Equals("0"))
                res.bMes = false;
            else
                res.bMes = true;
            //LGEVH 202306
            if (LoadPWLISTINI(strSubject + iDx.ToString(), "AUTOJOBCONFIG").Equals("0"))
                res.bAutoJobConfig = false;
            else
                res.bAutoJobConfig = true;
            return res;
        }

        public void SetPasswordUserData(PWUSER[] pwuserdata)
        {   
            string strSubject = "PASSWORD_USERLIST";
            int iCount = 0;
            for (int i = 0; i < pwuserdata.Length; i++)
            {
                if (!String.IsNullOrEmpty(pwuserdata[i].strLogName))
                {
                    iCount++;
                    SavePWLISTINI(strSubject + i.ToString(), "NAME", pwuserdata[i].strLogName);
                    //SavePWLISTINI(strSubject + i.ToString(), "PW", pwuserdata[i].strPassword);

                    //LGEVH 202306
                    //암호화 복호화 키 8글자 (필히 8자리여야 함)
                    WATCrypt m_crypt = new WATCrypt("11111111");
                    string strPwdData = m_crypt.Encrypt(pwuserdata[i].strPassword).Trim();

                    //SavePWLISTINI(strSubject + i.ToString(), "PW", pwuserdata[i].strPassword);
                    SavePWLISTINI(strSubject + i.ToString(), "PW", strPwdData);
                    if (pwuserdata[i].bEdit)
                        SavePWLISTINI(strSubject + i.ToString(), "EDIT", "1");
                    else
                        SavePWLISTINI(strSubject + i.ToString(), "EDIT", "0");

                    if (pwuserdata[i].bJob)
                        SavePWLISTINI(strSubject + i.ToString(), "JOB", "1");
                    else
                        SavePWLISTINI(strSubject + i.ToString(), "JOB", "0");

                    if (pwuserdata[i].bConfig)
                        SavePWLISTINI(strSubject + i.ToString(), "CONFIG", "1");
                    else
                        SavePWLISTINI(strSubject + i.ToString(), "CONFIG", "0");

                    if (pwuserdata[i].bMes)
                        SavePWLISTINI(strSubject + i.ToString(), "MES", "1");
                    else
                        SavePWLISTINI(strSubject + i.ToString(), "MES", "0");

                    //LGEVH 202306
                    if (pwuserdata[i].bAutoJobConfig)
                        SavePWLISTINI(strSubject + i.ToString(), "AUTOJOBCONFIG", "1");
                    else
                        SavePWLISTINI(strSubject + i.ToString(), "AUTOJOBCONFIG", "0");
                }
                else
                {
                    SavePWLISTINI(strSubject + i.ToString(), "NAME", "0");
                    SavePWLISTINI(strSubject + i.ToString(), "PW", "0");
                    SavePWLISTINI(strSubject + i.ToString(), "EDIT", "0");
                    SavePWLISTINI(strSubject + i.ToString(), "JOB", "0");
                    SavePWLISTINI(strSubject + i.ToString(), "CONFIG", "0");
                    SavePWLISTINI(strSubject + i.ToString(), "MES", "0");
                    //LGEVH 202306
                    SavePWLISTINI(strSubject + i.ToString(), "AUTOJOBCONFIG", "0");
                }
                
            }

            SavePWLISTINI("PASSWORD_MANAGEMENT", "COUNT", iCount.ToString());
         
        }

        public void SavePWLISTINI(string strSubject, string strName, string strValue)
        {
            string strPassword = strValue;
            //if (strName.Equals("PW") && IsEncrytionPassword())
            //{
            //    strPassword = CheckEncrytionPassword(strValue);
            //}

            try
            {
                WritePrivateProfileString(strSubject, strName, strPassword, Item_PWLISTfileName);
            }
            catch { }
        }

        public string LoadPWLISTINI(string strSubject, string strName)
        {
            string rtnStr = "0";
            StringBuilder sbTemp = new StringBuilder(1024);

            int i = GetPrivateProfileString(strSubject, strName, "", sbTemp, 1024, Item_PWLISTfileName);

            if (i > 0)
            {
                //if(strName.Equals("PW") && IsEncrytionPassword())
                //{                    
                //    rtnStr = CheckDecrytionPassword(sbTemp.ToString());
                //}
                //else
                //{
                    rtnStr = sbTemp.ToString();
                //}

                
            }

            return rtnStr;
        }

        /*
        private string CheckEncrytionPassword(string strpw)
        {            
            byte[] strByte = Encoding.UTF8.GetBytes(strpw);
            string strEnc = String.Empty;
            for (int i = 0; i < strByte.Length; i++)
            {
                strByte[i] += 0x20;
                strEnc += strByte[i].ToString("X2");
            }

            return strEnc;
            
        }

        private string CheckDecrytionPassword(string strpw)
        {
            byte[] strByte = HexStringToBytes(strpw);
            
            for (int i = 0; i < strByte.Length; i++)
            {
                strByte[i] -= 0x20;                
            }

            return Encoding.UTF8.GetString(strByte);

        }

        private bool IsEncrytionPassword()
        {
            StringBuilder sbTemp = new StringBuilder(1024);

            int i = GetPrivateProfileString("ENCRYPTION", "STATUS", "", sbTemp, 1024, Item_PWLISTfileName);

            if (i > 0)
            {
                if (sbTemp.ToString().Equals("ENABLE"))
                {
                    return true;
                }
            }

            return false;

        }
        
        public byte[] HexStringToBytes(string s)
        {
            
            if (s.Length == 0)
            {              
                return new byte[0];
            }
           
            if (s.Length % 2 != 0)
            {                
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
                            return new byte[0];
                        }
                        value = x << 4;
                        state = 1;
                        break;
                    case 1:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                        {
                            return new byte[0];
                        }
                        bytes[currentByte++] = (byte)(value + x);
                        state = 0;
                        break;
                 
                }
            }
           
            return bytes;
        }
        */
        public string JOB_CRC(string[] strData)
        {
            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;

            if (strData.Length < 1) return "FFFF";
            List<byte> lstBytes = new List<byte>();

            for (int i = 0; i < strData.Length; i++)
            {
                byte[] bData = Encoding.UTF8.GetBytes(strData[i]);
                lstBytes.AddRange(bData);
            }

            for (int i = 0; i < lstBytes.Count; i++)
            {
                usCRC0 ^= (ushort)(lstBytes[i] & 0xFF);

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

            return usSum.ToString("X2").PadLeft(4, '0');


        }

        //OTP PASSWORD 구현

        public string GetOtpPassword()
        {
            string strCurrentTime = DateTime.Now.ToString("MMdd");
            int iSeed = DateTime.Now.Month + DateTime.Now.Day;

            return strCurrentTime + iSeed.ToString();
        }

        public void FileDeleteBin(string strFileName)
        {
            FileInfo fileinfo = null;

            try
            {
                fileinfo = new FileInfo(strFileName);
                if (fileinfo.Exists)
                {

                    // 복사
                    // true 미설정시 파일이 존재하면 에러 발생
                    //filinfo.CopyTo(@"C:\TEST\test2.txt", true);
                    // 이동
                    //fileinfo.MoveTo(@"C:\TEST\test2_move.txt");
                    // 삭제
                    fileinfo.Delete();

                }
            }
            finally
            {
                if (fileinfo != null) fileinfo = null;
            }
        }

        public bool GetLocalFileForGBKEY(ref string strFileName)
        {
            //경로 확인

            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\TEMP"))
                return false;

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\TEMP");

            foreach (var item in di.GetFiles())
            {
                if (item.Extension.Equals(".dat") && item.Name.Contains("gb-key"))
                {
                    strFileName = AppDomain.CurrentDomain.BaseDirectory + "\\TEMP\\" + item.Name;
                    return true;
                }
            }

            return false;
        }

        public bool GetLocalSeedFileForMCTM(ref string strFileName)
        {
            //경로 확인

            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\SEED"))
                return false;

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\SEED");

            foreach (var item in di.GetFiles())
            {
                if (item.Extension.Equals(".dat") && item.Name.Contains("seed-key"))
                {
                    strFileName = AppDomain.CurrentDomain.BaseDirectory + "\\SEED\\" + item.Name;
                    return true;
                }
            }

            return false;
        }

        //public bool CertiFileExist(string strFilePath, ref string tmpString, ref string strReason)
        //{
        //    bool bSuccess = false;

        //    if (!System.IO.File.Exists(strFilePath))
        //    {   //파일이 없으면 
        //        strReason = "NOT FOUND FILE";
        //        return bSuccess;
        //    }
        //    else
        //    {
        //        //파일이 있으면
        //        FileStream FS = null;
        //        StreamReader SR = null;

        //        try
        //        {
        //            FS = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //            SR = new StreamReader(FS, System.Text.Encoding.UTF8);

        //            tmpString = String.Empty;
                    
        //            while (!SR.EndOfStream)
        //            {
        //                tmpString += SR.ReadLine() + Environment.NewLine;
        //                //tmpString += SR.ReadLine();
        //                //tmpString += Environment.CommandLine;
        //            }
        //            SR.Close();
        //            FS.Close();

        //            bSuccess = true;
        //            strReason = "SUCCESS";
        //        }
        //        catch (Exception e)
        //        {
        //            strReason = "GET FILE EXCEPTION.";
        //            string messs = String.Empty;
        //            messs = e.Message;
        //            bSuccess = false;
        //        }
        //        finally
        //        {
        //            if (SR != null) SR.Close();
        //            if (FS != null) FS.Close();
        //        }

        //        return bSuccess;
        //    }
        //}

        //public bool CertiFileExist_HEX(string strFilePath, ref string tmpString, ref string strReason)
        //{
        //    bool bSuccess = false;

        //    if (!System.IO.File.Exists(strFilePath))
        //    {   //파일이 없으면 
        //        strReason = "NOT FOUND FILE";
        //        return bSuccess;
        //    }
        //    else
        //    {
        //        //파일이 있으면
        //        FileStream FS = null;
        //        StreamReader SR = null;

        //        try
        //        {
        //            FS = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //            SR = new StreamReader(FS, System.Text.Encoding.UTF8);

        //            tmpString = String.Empty;

        //            while (!SR.EndOfStream)
        //            {
        //                tmpString += SR.ReadLine() + Environment.NewLine;
        //                //tmpString += SR.ReadLine();
        //                //tmpString += Environment.CommandLine;
        //            }
        //            SR.Close();
        //            FS.Close();

        //            bSuccess = true;
        //            strReason = "SUCCESS";
        //        }
        //        catch (Exception e)
        //        {
        //            strReason = "GET FILE EXCEPTION.";
        //            string messs = String.Empty;
        //            messs = e.Message;
        //            bSuccess = false;
        //        }
        //        finally
        //        {
        //            if (SR != null) SR.Close();
        //            if (FS != null) FS.Close();
        //        }

        //        return bSuccess;
        //    }
        //}

        public bool CheckMD5(string strVersion, ref string strRequireVersion)
        {
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string[] args = Environment.GetCommandLineArgs();
            strRequireVersion = String.Empty;
            if (args != null && args.Length > 1)
            {
                string sMD5 = args[1].Replace("-", "");
                if (sMD5.Equals("SuperPower")) return true; //gm breaker를 위한. 예외코드
                strRequireVersion = sMD5;
                using (MD5 md5 = MD5.Create())
                {
                    //byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(strVersion)); //strDate
                    byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(strDate)); //
                    string strConvertVersion = BitConverter.ToString(data).Replace("-", "");

                    if (strConvertVersion.Equals(sMD5))
                        return true;
                }
            }
            return false;
        }

        public bool GetLocalMfgFile(string strFileName, ref List<string[]> LstMfg)
        {            
            FileStream FS = null;
            StreamReader SR = null;
            bool bFlag = true;

            if (!File.Exists(strFileName))
            {   //파일이 없으면 return!                
                return false;
            }
            try
            {                
                LstMfg.Clear();
                FS = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
                SR = new StreamReader(FS, System.Text.Encoding.UTF8);

                string[] strSplit;
                string tmpString = String.Empty;
                bool b1stLine = false;

                while (!SR.EndOfStream)
                {
                    tmpString = SR.ReadLine();
                    if (!b1stLine) //첫번째줄은 건너뛰자;
                    {
                        b1stLine = true;
                        continue; 
                    }

                    strSplit = tmpString.Split('\t');

                    if (strSplit.Length != 9)
                    {
                        return false;
                    }
                    
                    for (int i = 0; i < strSplit.Length; i++)
                    {
                        if (strSplit[i].Contains("\t"))
                            strSplit[i] = strSplit[i].Replace("\t", String.Empty);

                        strSplit[i] = strSplit[i].Trim();
                    }
                    LstMfg.Add(strSplit);
                    tmpString = String.Empty;
                }

                SR.Close();
                FS.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetLocalMfgFile Error : " + ex.ToString());
                bFlag = false;
            }            
            finally
            {
                if (SR != null) ((IDisposable)SR).Dispose();
                if (FS != null) ((IDisposable)FS).Dispose();

            }
            return bFlag;
        }
        
    }

    class WATCrypt
    {
        byte[] Skey = new byte[8];

        public WATCrypt(string strKey)
        {
            Skey = ASCIIEncoding.ASCII.GetBytes(strKey);
        }

        public string Encrypt(string p_data)
        {
            if (Skey.Length != 8)
            {
                throw (new Exception("Invalid key. Key length must be 8 byte."));
            }

            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            rc2.Key = Skey;
            rc2.IV = Skey;

            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] data = Encoding.UTF8.GetBytes(p_data.ToCharArray());

            cryStream.Write(data, 0, data.Length);

            cryStream.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string p_data)
        {
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            rc2.Key = Skey;
            rc2.IV = Skey;

            MemoryStream ms = new MemoryStream();

            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] data = Convert.FromBase64String(p_data);
            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.GetBuffer());
        }
    }
}


