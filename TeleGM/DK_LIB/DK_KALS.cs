using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace GmTelematics
{
    class DK_KLAS
    {
        private const string DLLNAME = "KALS.dll";
                
        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]

        public struct RETURN_HDK_RECORD
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)] public byte[] DIVISION_CODE;
   
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)] public byte[] SITE_CODE;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)] public byte[] KEY_CLASS;
                   
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]  public byte[] KEY_TYPE;
     
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)] public byte[] PART_NO;
         
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]  public byte[] KEY_STYLE;
        
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]  public byte[] KEY_TXN_ID;
         
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)] public byte[] KEY_FILE_NAME;
       
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]  public byte[] ALLOCATE_ID;
           
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]  public byte[] KEY_STATUS;
        
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]  public byte[] MAC_ADDRESS_DEC;
          
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]  public byte[] MAC_ADDRESS_HEX;
      
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]  public byte[] AACS_DEVICE_KEY;
        
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]  public byte[] AACS_ENCRYPT_DATA;
       
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 221)] public byte[] AACS_NONENCRYPT_DATA;
           
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]  public byte[] AACS_SPECIAL_KEY;
      
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]  public byte[] SET_SERIAL_NO;
         
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]  public byte[] WIP_ID;
      
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)] public byte[] ORIGINAL_KEY_FILE_NAME;
     
        }     
               
        [DllImport("KALS.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]    
            extern public static int GetKey(
            byte[] iDivision, 
            byte[] iSite, 
            byte[] iClass, 
            byte[] iType, 
            byte[] iPartNo, 
            byte[] iReworkFlag,
            byte[] iReworkSerial,
            out IntPtr RETURN_HDK_RECORD 
            //ref RETURN_HDK_RECORD hdkRECORD  
        );
      
        [DllImport("KALS.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static int ReturnWritingInfo(
            byte[] iDivision,
            byte[] iSite,
            byte[] iClass,
            byte[] iType,
            byte[] iKeyName,
            byte[] iUser
        );

        [DllImport("KALS.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static int WriteProductionInfo(
            byte[] iDivision,
            byte[] iSite,
            byte[] iClass,
            byte[] iType,
            byte[] iKeyName,
            byte[] iSetSerial,
            byte[] iWipId,
            byte[] iUser,
            byte[] iAtt1,
            byte[] iAtt2,
            byte[] iAtt3,
            byte[] iAtt4,
            byte[] iAtt5

        );

        public int Kals_ReturnWritingInfo(int iSectionName, string strDownloadFileName, string strStationName, ref string strErrMsg)
        {
            string strSectionName = (KALSNAME.DID_SEED_GEN11 + iSectionName).ToString(); //RANDOM_SKC, RANDOM_SLAVE, RANDOM_SPI 를 지칭한다.
            string strSiteName = "LGEVN";
            if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_SiteCode))
                strSiteName = STEPMANAGER_VALUE.strKALS_SiteCode;

            byte[] iDivision     = Encoding.UTF8.GetBytes("PGZ");
            //byte[] iSite         = Encoding.UTF8.GetBytes("LGEVN");
            byte[] iSite = Encoding.UTF8.GetBytes(strSiteName);
            byte[] iClass        = Encoding.UTF8.GetBytes("DID");
            byte[] iType         = Encoding.UTF8.GetBytes(strSectionName.ToString());
            byte[] iKeyName      = Encoding.UTF8.GetBytes(strDownloadFileName.ToString()); 
            //byte[] iUser         = Encoding.UTF8.GetBytes("GM_KEY_STATION"); //GEN11, MCTM 공용으로 사용해서,,, 이름 별도로 설정하자.
            byte[] iUser = Encoding.UTF8.GetBytes(strStationName); //GEN11, MCTM 공용으로 사용해서,,, 이름 별도로 설정하자.


            int iRes = ReturnWritingInfo(iDivision, iSite, iClass, iType, iKeyName, iUser);
            strErrMsg = (KALSRETURNCODE.ERR_UNKNOWN + iRes).ToString();
            return iRes;
        }
                
        public int Kals_WriteProduc_WritingInfo(int iSectionName, string strDownloadFileName, string strWipId, string strStationName, ref string strErrMsg)
        {
            string strSectionName = (KALSNAME.DID_SEED_GEN11 + iSectionName).ToString(); //RANDOM_SKC, RANDOM_SLAVE, RANDOM_SPI 를 지칭한다.
            string strSiteName = "LGEVN";
            if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_SiteCode))
                strSiteName = STEPMANAGER_VALUE.strKALS_SiteCode;

            byte[] iDivision = Encoding.UTF8.GetBytes("PGZ");
            //byte[] iSite = Encoding.UTF8.GetBytes("LGEVN");
            byte[] iSite = Encoding.UTF8.GetBytes(strSiteName);
            byte[] iClass = Encoding.UTF8.GetBytes("DID");
            byte[] iType = Encoding.UTF8.GetBytes(strSectionName.ToString());
            byte[] iKeyName = Encoding.UTF8.GetBytes(strDownloadFileName.ToString());
            byte[] iSetSerial = Encoding.UTF8.GetBytes(strWipId.ToString());
            byte[] iWipId = Encoding.UTF8.GetBytes(strWipId.ToString());
            //byte[] iUser = Encoding.UTF8.GetBytes("GM_KEY_STATION"); //strStationName
            byte[] iUser = Encoding.UTF8.GetBytes(strStationName); //strStationName
            byte[] iAtt1 = Encoding.UTF8.GetBytes("");
            byte[] iAtt2 = Encoding.UTF8.GetBytes("");
            byte[] iAtt3 = Encoding.UTF8.GetBytes("");
            byte[] iAtt4 = Encoding.UTF8.GetBytes("");
            byte[] iAtt5 = Encoding.UTF8.GetBytes("");

            int iRes = WriteProductionInfo(iDivision, iSite, iClass, iType, iKeyName, iSetSerial, iWipId, iUser, 
                                           iAtt1, iAtt2, iAtt3, iAtt4, iAtt5);
            strErrMsg = (KALSRETURNCODE.ERR_UNKNOWN + iRes).ToString();
            return iRes;
        }

        private string ConvertFileName(byte[] refFileName)
        {
            string strBytes = String.Empty;
            List<byte> tmpList = new List<byte>();

            for (int i = 0; i < refFileName.Length; i++)
            {
                if (!refFileName[i].Equals(0x00))
                {
                    tmpList.Add(refFileName[i]);
                }
                else
                    break;
            }

            strBytes = System.Text.Encoding.UTF8.GetString(tmpList.ToArray()).Trim();

            return strBytes;           

        }

        public int Kals_GetKey(int iSectionName, ref string strBinary, ref string strDownFileName, ref string strErrMsg)
        {
            

            string strSectionName = (KALSNAME.DID_SEED_GEN11 + iSectionName).ToString(); //RANDOM_SKC, RANDOM_SLAVE, RANDOM_SPI 를 지칭한다.
            string strSiteName = "LGEVN";
            if (!String.IsNullOrEmpty(STEPMANAGER_VALUE.strKALS_SiteCode))
                strSiteName = STEPMANAGER_VALUE.strKALS_SiteCode;

            byte[] iDivision = Encoding.UTF8.GetBytes("PGZ");
            //byte[] iSite = Encoding.UTF8.GetBytes("LGEVN");
            byte[] iSite = Encoding.UTF8.GetBytes(strSiteName);
            byte[] iClass = Encoding.UTF8.GetBytes("DID");
            byte[] iType = Encoding.UTF8.GetBytes(strSectionName.ToString());  //"RANDOM_SKC"
            byte[] iPartNo = Encoding.UTF8.GetBytes("NA");
            byte[] iReworkFlag = Encoding.UTF8.GetBytes("");
            byte[] iReworkSerial = Encoding.UTF8.GetBytes("");

            RETURN_HDK_RECORD hdkRECORD = new RETURN_HDK_RECORD();
            IntPtr informations = IntPtr.Zero;
            int structSize = Marshal.SizeOf(typeof(RETURN_HDK_RECORD));
            informations = Marshal.AllocHGlobal(structSize);

            int i = GetKey(iDivision, iSite, iClass, iType, iPartNo, iReworkFlag, iReworkSerial, out informations);

            strErrMsg = (KALSRETURNCODE.ERR_UNKNOWN + i).ToString();

            if (i == (int)KALSRETURNCODE.ERR_SUCCESS)
            {

                byte[] refFileName = new byte[structSize];                                  //MEMORY FULL BYTE 로깅작업용(1)전체 보여줄떄...
                Marshal.Copy(informations, refFileName, 0, structSize);                     //MEMORY FULL BYTE 로깅작업용(2)전체 보여줄떄...
                //string strLoggingString = System.Text.Encoding.UTF8.GetString(tmpByte);//MEMORY FULL BYTE 로깅작업용(3)전체 보여줄떄...
                               
                hdkRECORD = (RETURN_HDK_RECORD)Marshal.PtrToStructure(informations, typeof(RETURN_HDK_RECORD));

                //KALS DLL 구조체 포멧(형식)상 DIVISION CODE 필드에 들어와야할것이 아니지만 c# 문제 인지 cns에서 잘못만든건지
                //원인은 모르겟지만 일단 DIVISION_CODE 필드에 값이 들어오기 때문에 임시적으로 이것으로 사용한다.
               
                //string strFileName = System.Text.Encoding.UTF8.GetString(hdkRECORD.DIVISION_CODE).Trim('\0');
                //ConvertFileName 은 사실상 구조체정보규약을 어기는 것이다. KALS DLL 과 호환이 되지 않아서 이방법을 택함.. 2019.1.29

                try
                {
                    Marshal.FreeHGlobal(informations);
                }
                catch { }

                string strFileName = ConvertFileName(refFileName);

                if (String.IsNullOrEmpty(strFileName))   //ex)20150820194900_000012
                {
                    strErrMsg = "DOWN FILE NAME : NULL";
                    return (int)KALSRETURNCODE.ERR_UNKNOWN;
                }

                //int iFileNameLength = 0;
                string strExtName = ".dat";
                string strDownLoadFolder = String.Empty;
                string strDownLoadTempFile = String.Empty;
                string strFoderName = String.Empty;

                switch (iSectionName)
                {
                    
                    case (int)KALSNAME.DID_SEED_GEN11_VCP:
                    case (int)KALSNAME.DID_SEED_GEN11: //iFileNameLength = 26; //SEED-KEY_YYYYMMDDXXXXXXXXX.dat
                        strExtName = ".dat";
                        strFoderName = "TEMP\\";
                        strDownLoadFolder = AppDomain.CurrentDomain.BaseDirectory + "GEN11_CERT";
                        strDownLoadTempFile = AppDomain.CurrentDomain.BaseDirectory + "GEN11_CERT\\download_seed.dat";
                        CreateFolder(strDownLoadFolder);
                        if(ExistFile(strDownLoadTempFile))
                            FileDeleteBin(strDownLoadTempFile); //다운로드한 파일삭제
                        break;

                    case (int)KALSNAME.DID_SEED_GEN12:
                        strExtName = ".dat";
                        strFoderName = "TEMP\\";
                        strDownLoadFolder = AppDomain.CurrentDomain.BaseDirectory + "GEN12_CERT";
                        strDownLoadTempFile = AppDomain.CurrentDomain.BaseDirectory + "GEN12_CERT\\download_seed.dat";
                        CreateFolder(strDownLoadFolder);
                        if (ExistFile(strDownLoadTempFile))
                            FileDeleteBin(strDownLoadTempFile); //다운로드한 파일삭제
                        break;

                    case (int)KALSNAME.DID_SEED_MCTM: //iFileNameLength = 26; //seed-key_20171211163209545.dat
                        strExtName = ".dat";
                        strFoderName = "TEMP\\"; 
                        strDownLoadFolder = AppDomain.CurrentDomain.BaseDirectory + "MCTM_SEED";
                        strDownLoadTempFile = AppDomain.CurrentDomain.BaseDirectory + "MCTM_SEED\\download_seed.dat";
                        CreateFolder(strDownLoadFolder);
                        if(ExistFile(strDownLoadTempFile))
                            FileDeleteBin(strDownLoadTempFile); //다운로드한 파일삭제
                        break;

                    case (int)KALSNAME.GM_GB_GEN11: //iFileNameLength = 24; //gb-key_20180717210803637.dat
                    case (int)KALSNAME.GM_GB_GEN12:
                        strExtName = ".dat";
                        strFoderName = "TEMP\\"; 
                        strDownLoadFolder = AppDomain.CurrentDomain.BaseDirectory + "GB_KEY";
                        strDownLoadTempFile = AppDomain.CurrentDomain.BaseDirectory + "GB_KEY\\download_key.dat";
                        CreateFolder(strDownLoadFolder);
                        if(ExistFile(strDownLoadTempFile))
                            FileDeleteBin(strDownLoadTempFile); //다운로드한 파일삭제
                        break;
                    default:
                        strFoderName = "TEMP\\";
                        strExtName = ".bin";
                        break;
                }


                /*
                if (strFileName.Length >= iFileNameLength)   //ex)20150820194900_000012
                {
                    strFileName = strFileName.Substring(0, iFileNameLength);
                    strFileName = strFileName.Replace(strExtName, String.Empty);
                    strFileName = strFileName.Replace(strExtName.ToUpper(), String.Empty);
                }
                else
                {
                    strErrMsg = "DOWN FILE NAME LENGTH ERROR : " + strFileName + "(" + strFileName.Length.ToString() + ")";
                    return (int)KALSRETURNCODE.ERR_UNKNOWN;
                }*/


                //1. 다운받은 경로 찾기 
                
                string strFullPath = String.Empty;

                strFullPath = AppDomain.CurrentDomain.BaseDirectory + strFoderName + strFileName + strExtName;
                strDownFileName = strFileName;
                if (!ExistFile(strFullPath)) //다운로드 되었으나 파일을 찾을 수 없을때.
                {
                    if (STEPMANAGER_VALUE.bIamMD5) //MD5 이용시 LGW SWP 에 의해 KALS DLL 경로가 그쪽으로 향하는 현상이 생긴다. 따라서 그쪽 폴더를 탐색해야한다....ㅅㅂ.
                    {
                        strFullPath = @"C:\GMES\LGE.SWP\TEMP\" + strFileName + strExtName;

                        if (!ExistFile(strFullPath)) //재탐색
                        {
                            strErrMsg = "CAN NOT FOUND DOWNLOAD FILE(2nd) : " + strFileName + strExtName;
                            return (int)KALSRETURNCODE.ERR_UNKNOWN;
                        }
                        else
                        {
                            byte[] BinByte = FileToBinary(strFullPath);
                            strBinary = BitConverter.ToString(BinByte).Replace("-", "");
                            STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath = strDownLoadTempFile;
                            FileCopyBin(strFullPath, strDownLoadTempFile);
                            FileDeleteBin(strFullPath); //다운로드한 파일삭제
                            return (int)KALSRETURNCODE.ERR_SUCCESS;
                        }

                    }
                    else
                    {
                        strErrMsg = "CAN NOT FOUND DOWNLOAD FILE : " + strFileName + strExtName;
                        return (int)KALSRETURNCODE.ERR_UNKNOWN;
                    }
                    
                }
                else
                {
                    byte[] BinByte = FileToBinary(strFullPath);
                    strBinary = BitConverter.ToString(BinByte).Replace("-", "");
                    STEPMANAGER_VALUE.strKALS_DID_SEED_FilePath = strDownLoadTempFile;
                    FileCopyBin(strFullPath, strDownLoadTempFile);
                    FileDeleteBin(strFullPath); //다운로드한 파일삭제
                    return (int)KALSRETURNCODE.ERR_SUCCESS;
                }
            }

            try
            {
                Marshal.FreeHGlobal(informations);
            }
            catch { }


            return i;

        }

        private bool ExistFile(string strFileName)
        {
            if (File.Exists(strFileName))
            {   //파일이 있으면 create mode
                return true;
            }
            else
            {
                return false;
            }
        }

        private byte[] FileToBinary(string strFileName)
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

        private bool FileDeleteBin(string strFileName)
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
                return true;
            }     
            catch
            {
                return false;
            }
            finally
            {
                if (fileinfo != null) fileinfo = null; 
            }
        }

        private bool FileCopyBin(string strFileName, string strCopyPath)
        {
            FileInfo fileinfo = null;

            try
            {
                fileinfo = new FileInfo(strFileName);
                if (fileinfo.Exists)
                {

                    // 복사
                    // true 미설정시 파일이 존재하면 에러 발생
                    fileinfo.CopyTo(strCopyPath, true);
                    // 이동
                    //fileinfo.MoveTo(@"C:\TEST\test2_move.txt");
                    // 삭제
                    //fileinfo.Delete();

                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fileinfo != null) fileinfo = null;
            }
        }

        private void CreateFolder(string strPath)
        {

            try
            {
                DirectoryInfo Di = new DirectoryInfo(strPath);

                if (Di.Exists == false) { Di.Create(); }
            }
            catch
            {

            }


        }
    }
}
