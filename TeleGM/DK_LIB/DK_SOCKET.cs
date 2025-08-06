using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace GmTelematics
{    
    delegate void EventTransfferMsg(int iPort, long iValue, string strMessage);
    delegate void EventFileList(int iPort, int iType, string[] strFileList);
    delegate void EventSocketMsg(int iPort, int iMsgCode, string cParam);      

    enum SOCKETCODE
    {
        NONE, C01, C02, C03, C04, C05, C06, C07, C08, CCC,
        S01, S02, S03, S04, S05, S06, S07, S08, S09, S10, S11,
        MSG, END
    }

    enum FILETYPE
    {
        NONE, BINARY, ASCII, END
    }

    struct MornitorData
    {
        public string strName;
        public string strIP;
        public string strJobName;
        public string strVersion;
        public string strStatus;
    }    
 
    //뽀나쓰 클래스... 
    class DK_NETINFO
    {
        public DK_NETINFO()
        {

        }

        public List<string> GetMyLocalIpAddress()
        {
            List<string> lstResult = new List<string>();

            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < iphost.AddressList.Length; i++)
            {
                if (iphost.AddressList[i].AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    lstResult.Add(iphost.AddressList[i].ToString());                  
                }
            }

            return lstResult;
        }

    }

    //DK_SOCKET_CLIENT나 DK_SOCKET_SERVER에서 사용하는 클래스.
    class AsyncNode
    {
        public int    iNumber;
        public byte[] Buffer;
        public Socket sockWorker;

        public uint iFileSendNowBytes;
        public uint iFileSendTotalBytes;
        public string strSendFileName;
        public string strFullFileName;


        public AsyncNode(int iCreateNumber)
        {
            iNumber = iCreateNumber;
            Buffer = new byte[8192];

            iFileSendNowBytes = 0;
            iFileSendTotalBytes = 0;
            strSendFileName = String.Empty;
            strFullFileName = String.Empty;

        }

        public void ClearBuffer()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
        }

        public int GetNodeNumber()
        {
            return iNumber;
        }
    }

    //클라이언트용
    class DK_SOCKET_CLIENT
    {
        private byte STX0 = 0x01;
        private byte STX1 = 0x02;

        private byte ETX0 = 0x03;
        private byte ETX1 = 0x04;

        private bool          bExit;
        private bool          bConnected;        
        private string        strConnIP;
        private int           iConnPort;
        private Socket        sockUser;
        //private Thread        connThread;
                
        private string        strRecvFileName;
        private uint          iTotalFileSize;
        private uint          iNowFileSize;

        private List<byte>    ListSecondBuffer = new List<byte>();

        public event EventSocketMsg    SockRealTimeMsg;
        public event EventTransfferMsg SockTransffer;

        private string        strJobListFile;
        private List<string>  lstLogFolderFile;
        private List<string>  lstResultFolderFile;
        private List<string>  lstScreenFolderFile;
        private string        strUploadPath;

        private string strDownloadPath;


        public DK_SOCKET_CLIENT(string strCommLogPath)
        {
            bConnected = false;
            bExit = false;
            strJobListFile      = String.Empty;
            lstLogFolderFile    = new List<string>();
            lstResultFolderFile = new List<string>();
            lstScreenFolderFile = new List<string>();
            strUploadPath       = strCommLogPath;
        }

        public string GetReqDownloadPath()
        {
            return strDownloadPath;
        }

        public bool IsConnected() { return bConnected; }

        public bool IsSocketUsed() { return bConnected; } //소켓이 샌딩중인지...

        public string GetConnectedMyIp() {
            if (IsConnected())
                return IPAddress.Parse(((IPEndPoint)sockUser.LocalEndPoint).Address.ToString()).ToString();
            else
                return "0.0.0.0";
        }

        public void Disconnect(bool bEvents)  
        {
            try
            {
                if (sockUser != null)
                {
                    sockUser.Close();
                    sockUser.Dispose();
                }
                
            }
            catch  {  }

            bConnected = false;

            if (bEvents)
            {
                bExit = true;
            }
            else
            {
                if (bExit) return;
                this.TryConnect();
            }

        }

        /* 미접속상태로 프로그램 종료할경우 소켓 커넥션 타임아웃이 너무 길어서 안씀.... 
        private void TryConnect()
        {
            bConnected  = false;
            sockUser = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            MessageGateWay("Connect Server..." + strConnIP);
            while (true)
            {
                if (sockUser == null || bExit) return;

                sockUser = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                try
                {
                    //MessageGateWay("Connecting (" + strConnIP + ") ...");
                    sockUser.Connect(strConnIP, iConnPort); 
                    bConnected = true;
                    MessageGateWay("Server Connected." + strConnIP);
                    AsyncNode Antempler = new AsyncNode(0);
                    Antempler.sockWorker = sockUser;
                    Antempler.ClearBuffer();
                    sockUser.BeginReceive(Antempler.Buffer, 0, Antempler.Buffer.Length, SocketFlags.None, handleDataRecv, Antempler);
                    return;
                    
                }
                catch
                {
                    //MessageGateWay("Connecting Fail.");
                    bConnected = false;
                    if (sockUser != null)
                    {
                        sockUser.Close();
                        sockUser.Dispose();
                    }
                }
                System.Threading.Thread.Sleep(1);
            }            

        }
        */

        private void TryConnect()
        {
            bConnected = false;
            sockUser = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            MessageGateWay((int)SOCKETCODE.MSG, "Connect Server..." + strConnIP);
            
            if (sockUser == null || bExit) return;

            try
            {                
                sockUser.BeginConnect(strConnIP, iConnPort, new AsyncCallback(ConnBack), sockUser);
            }
            catch
            {                
                bConnected = false;
                if (sockUser != null)
                {
                    sockUser.Close();
                    sockUser.Dispose();
                }
                if (bExit) return;
                this.TryConnect();
            }
        }

        private void ConnBack(IAsyncResult iar)
        {
            try
            {
                Socket tmpSock = (Socket)iar.AsyncState;
                IPEndPoint svrEP = (IPEndPoint)tmpSock.RemoteEndPoint;

                tmpSock.EndConnect(iar);
                sockUser = tmpSock;
                bConnected = true;
                MessageGateWay((int)SOCKETCODE.MSG, "Server Connected (" + strConnIP + ")");
                AsyncNode Antempler = new AsyncNode(0);
                Antempler.sockWorker = sockUser;
                Antempler.ClearBuffer();
                sockUser.BeginReceive(Antempler.Buffer, 0, Antempler.Buffer.Length, SocketFlags.None, handleDataRecv, Antempler);
                return;

            }
            catch
            {
                bConnected = false;
                if (sockUser != null)
                {
                    sockUser.Close();
                    sockUser.Dispose();
                }
                if (bExit) return;
                this.TryConnect();
            }
        }

        public void Connect(string strAddress, int iPort)
        {
            if (bConnected) return;
            strConnIP = strAddress;
            iConnPort = iPort;
            this.TryConnect();            
        }

        private void handleDataRecv(IAsyncResult IAR)
        {            
            AsyncNode AnObserver = (AsyncNode)IAR.AsyncState;

            int iRecvBytes = 0;

            try
            {
                iRecvBytes = AnObserver.sockWorker.EndReceive(IAR);                
            }
            catch 
            {
                Disconnect(false);
                MessageGateWay((int)SOCKETCODE.MSG, "Server Disconnected.");
                return;
            }

            if (iRecvBytes > 0)
            {
                //수신바이트 내용 처리부분이나 클라이언트는 파일만 받으므로 
                //파일다운로드만 처리하자. chat 메시지는 없다...
                byte[] bProccessBytes = new byte[iRecvBytes];
                Array.Copy(AnObserver.Buffer, 0, bProccessBytes, 0, bProccessBytes.Length);
                RecvProccess(bProccessBytes, iRecvBytes);
                
                try
                {
                    //재수신 모드
                    AnObserver.ClearBuffer();
                    AnObserver.sockWorker.BeginReceive(AnObserver.Buffer, 0, AnObserver.Buffer.Length, SocketFlags.None, handleDataRecv, AnObserver);
             
                }
                catch
                {
                    Disconnect(false);
                    //MessageGateWay("Disconnected.");
                    return;
                }
            }
            else
            {
                Disconnect(false);
                //MessageGateWay("Disconnected.");
                return;
            }

        }

        private void handleDataSend(IAsyncResult IAR)
        {
            if (!IsConnected()) return;

            AsyncNode AnObserver = (AsyncNode)IAR.AsyncState;

            int iRecvBytes = 0;

            try
            {
                iRecvBytes = AnObserver.sockWorker.EndSend(IAR);
                
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
        }

        private void handleZipSendLoop(IAsyncResult IAR)
        {
            if (!IsConnected()) return;

            AsyncNode AnObserver = (AsyncNode)IAR.AsyncState;

            int iRecvBytes = 0;

            try
            {
                iRecvBytes = AnObserver.sockWorker.EndSend(IAR);

                if (iNowFileSize < iTotalFileSize)
                {
                    System.Threading.Thread.Sleep(1);
                    AutoZipFileSendLoop(AnObserver.GetNodeNumber(), strRecvFileName);
                }
                else
                {
                    //여기서 모니터링 활성화 메시지를 날리자.
                    iNowFileSize = 0;
                    iTotalFileSize = 0;
                }
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }
        }

        //C01
        public void SendMorniotrData(MornitorData mdData)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304

            if (!IsConnected()) return;
            string strMessage = mdData.strName + ","
                                + mdData.strIP + ","
                                + mdData.strJobName + ","
                                + mdData.strVersion + ","
                                + mdData.strStatus;

            int istrMsgBytesCount = Encoding.UTF8.GetByteCount(strMessage);
            byte[] Sendbytes = new byte[2 + 3 + 4 + istrMsgBytesCount + 2];
            byte[] Commandbytes = Encoding.UTF8.GetBytes("C01");            
            byte[] Databytes    = Encoding.UTF8.GetBytes(strMessage);
            byte[] Lengthbytes  = BitConverter.GetBytes(Databytes.Length);

            Sendbytes[0] = STX0;
            Sendbytes[1] = STX1;
            
            Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
            Array.Copy(Lengthbytes,  0, Sendbytes, 5, Lengthbytes.Length);
            Array.Copy(Databytes,    0, Sendbytes, 9, Databytes.Length);

            Sendbytes[Sendbytes.Length-2] = ETX0;
            Sendbytes[Sendbytes.Length-1] = ETX1;

            //string tmpstr = BitConverter.ToString(Sendbytes).Replace("-", " ");
            try
            {
                AsyncNode Antempler = new AsyncNode(0);
                Antempler.sockWorker = sockUser;
                Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSend, Antempler);
                
         
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }         
        }

        //C02
        private void SendFileCommand(string strMsgCommand)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304

            if (!IsConnected()) return;                      

            string strMessage = strMsgCommand;

            byte[] Sendbytes;
            byte[] Commandbytes;
            byte[] Databytes;
            byte[] Lengthbytes;

            string strListData = String.Empty;
            int iFolderStringBytes = 0;
            switch (strMessage)
            {
                case "READY":   
                                Sendbytes       = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes    = Encoding.UTF8.GetBytes("C02");
                                Databytes       = Encoding.UTF8.GetBytes(strMessage);
                                Lengthbytes     = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "JOBLIST":
                                if (String.IsNullOrEmpty(strJobListFile))
                                {
                                    return;
                                }
                                int iFileStringBytes = Encoding.UTF8.GetByteCount(strJobListFile);
                                Sendbytes = new byte[2 + 3 + 4 + iFileStringBytes + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("C03");
                                Databytes = Encoding.UTF8.GetBytes(strJobListFile);
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "LOGFOLDERLIST":
                                if (lstLogFolderFile.Count < 1)
                                {
                                    return;
                                }
                                strListData = MakeStringListFile(lstLogFolderFile);
                                iFolderStringBytes = Encoding.UTF8.GetByteCount(strListData);
                                Sendbytes = new byte[2 + 3 + 4 + iFolderStringBytes + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("C04");
                                Databytes = Encoding.UTF8.GetBytes(strListData);
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "RESULTFOLDERLIST":
                                if (lstResultFolderFile.Count < 1)
                                {
                                    return;
                                }
                                strListData = MakeStringListFile(lstResultFolderFile);
                                iFolderStringBytes = Encoding.UTF8.GetByteCount(strListData);
                                Sendbytes = new byte[2 + 3 + 4 + iFolderStringBytes + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("C05");
                                Databytes = Encoding.UTF8.GetBytes(strListData);
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "RESULTSCREENLIST":
                                if (lstScreenFolderFile.Count < 1)
                                {
                                    return;
                                }
                                strListData = MakeStringListFile(lstScreenFolderFile);
                                iFolderStringBytes = Encoding.UTF8.GetByteCount(strListData);
                                Sendbytes = new byte[2 + 3 + 4 + iFolderStringBytes + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("C06");
                                Databytes = Encoding.UTF8.GetBytes(strListData);
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "REQUEST_FILEDOWNLOAD":

                                //파일 유무체크 및 압축후 파일 타입 이름 사이즈 함수 호출
                                string strFileSize = String.Empty;
                                if (CompressingFolder(strDownloadPath, ref strFileSize))
                                {
                                    string strType = String.Empty;
                                    if (strDownloadPath.Contains(@"LOG\"))
                                    {
                                        strType = "LOG";
                                    }
                                    else if (strDownloadPath.Contains(@"RESULT\"))
                                    {
                                        strType = "RESULT";
                                    }
                                    else if (strDownloadPath.Contains(@"SCREEN\"))
                                    {
                                        strType = "SCREEN";
                                    }
                                    else if (strDownloadPath.Contains(@"DATA\"))
                                    {
                                        strType = "DATA";
                                    }
                                    else
                                    {
                                        return;
                                    }
                                    strType += "," + strFileSize;                                   
                                    Sendbytes = new byte[2 + 3 + 4 + strType.Length + 2];
                                    Commandbytes = Encoding.UTF8.GetBytes("C07");
                                    Databytes = Encoding.UTF8.GetBytes(strType);
                                    Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                    break;
                                }
                                else
                                {   // 파일이 없으므로 취소
                                    Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                    Commandbytes = Encoding.UTF8.GetBytes("CCC");
                                    Databytes = Encoding.UTF8.GetBytes(strMessage);
                                    Lengthbytes = BitConverter.GetBytes(Databytes.Length);                             
                                    MessageGateWay((int)SOCKETCODE.S10, "");
                                    strMessage += "_CANCEL";
                                    break;
                                }
                                

                default: return;
            }
                        
            Sendbytes[0] = STX0;
            Sendbytes[1] = STX1;

            Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
            Array.Copy(Lengthbytes, 0, Sendbytes, 5, Lengthbytes.Length);
            Array.Copy(Databytes, 0, Sendbytes, 9, Databytes.Length);

            Sendbytes[Sendbytes.Length - 2] = ETX0;
            Sendbytes[Sendbytes.Length - 1] = ETX1;

            try
            {
                AsyncNode Antempler = new AsyncNode(0);
                Antempler.sockWorker = sockUser;                
                if(strMessage.Equals("REQUEST_FILEDOWNLOAD"))
                {
                    Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleZipSendLoop, Antempler);                
                }
                else
                {
                    Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSend, Antempler);                
                }
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
            }
        }

        //C08
        private void SendZipFile(int idx, string strFileName)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304 
            if (!IsConnected()) return;                      

            //파일 읽어오기
            int iByteSize = (2048 - 2 + 3 + 4 + 2);

            if (iNowFileSize + iByteSize >= iTotalFileSize)
            {
                iByteSize = (int)(iTotalFileSize - iNowFileSize);
            }

            byte[] Databytes = GetZipFileBytes(strRecvFileName, (int)iNowFileSize, iByteSize);

            iNowFileSize += (uint)iByteSize;

            byte[] Sendbytes = new byte[2 + 3 + 4 + Databytes.Length + 2];
            byte[] Commandbytes = Encoding.UTF8.GetBytes("C08");
            byte[] Lengthbytes = BitConverter.GetBytes(Databytes.Length);

            Sendbytes[0] = STX0;
            Sendbytes[1] = STX1;

            Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
            Array.Copy(Lengthbytes,  0, Sendbytes, 5, Lengthbytes.Length);
            Array.Copy(Databytes,    0, Sendbytes, 9, Databytes.Length);

            Sendbytes[Sendbytes.Length - 2] = ETX0;
            Sendbytes[Sendbytes.Length - 1] = ETX1;

            try
            {
                MessageGateWay((int)SOCKETCODE.MSG, "SEND : " + iNowFileSize.ToString() + " / " + iTotalFileSize.ToString());   
                AsyncNode Antempler = new AsyncNode(0);
                Antempler.sockWorker = sockUser;
                Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleZipSendLoop, Antempler);

            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
            }
        }

        private void AutoZipFileSendLoop(int idx, string strFileName)
        {
            if (iNowFileSize >= iTotalFileSize) return;
            SendZipFile(idx, strFileName);
        }

        public bool CompressingFolder(string strFolderPath, ref string strFileSize)
        {
            //string strPath = System.Windows.Forms.Application.StartupPath;
            //string strDowloadFolder = strPath + @"\" + strFolderPath;            
            string strPath = strUploadPath;
            string strDowloadFolder = strPath + strFolderPath;

            if (strDownloadPath.Contains(@"DATA\"))
            {
                strDowloadFolder = System.Windows.Forms.Application.StartupPath + @"\" + strFolderPath;
            }

            //1. 디렉토리 유무 체크 및 디렉토리내 파일 유무 체크
            DirectoryInfo di = new System.IO.DirectoryInfo(strDowloadFolder);
            if (di.Exists == false) return false;

            FileInfo[] fi    = di.GetFiles("*.*");
            if (fi.Length == 0) return false;

            //2. 파일 압축
            CompressDirectory(strDowloadFolder, "test.zip");
            
            //3. 압축 파일 사이즈 반환
            //GetFileSize(strPath + @"\UPLOAD\test.zip", ref strFileSize);
            GetFileSize(System.Windows.Forms.Application.StartupPath + @"\UPLOAD\test.zip", ref strFileSize);

            iTotalFileSize = uint.Parse(strFileSize);
            strRecvFileName = System.Windows.Forms.Application.StartupPath + @"\UPLOAD\test.zip";

            return true;
        }

        private bool GetFileSize(string strFileName, ref string iFileSize)
        {
            if (File.Exists(strFileName)) //파일이 있으면 
            {
                FileInfo fInfo = new FileInfo(strFileName);
                iFileSize = fInfo.Length.ToString();
                return true;
            }
            return false;
        }

        private bool CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
        {
            try
            {
                //Compress file name
                char[] chars = sRelativePath.ToCharArray();
                zipStream.Write(BitConverter.GetBytes(chars.Length), 0, sizeof(int));
                foreach (char c in chars)
                    zipStream.Write(BitConverter.GetBytes(c), 0, sizeof(char));

                //Compress file content
                byte[] bytes = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));
                zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                zipStream.Write(bytes, 0, bytes.Length);
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                return false;
            }

            return true;

        }

        private bool CompressDirectory(string sInDir, string sOutFile)
        {
            //UPLOAD 폴더 확인 및 생성
            string strPath = System.Windows.Forms.Application.StartupPath + @"\UPLOAD";
            DirectoryInfo di = new DirectoryInfo(strPath);
            if (di.Exists == false){ di.Create(); }
            
            //파일 압축

            try
            {
                string[] sFiles = Directory.GetFiles(sInDir, "*.*", SearchOption.AllDirectories);
                int iDirLen = sInDir[sInDir.Length - 1] == Path.DirectorySeparatorChar ? sInDir.Length : sInDir.Length + 1;

                using (FileStream outFile = new FileStream(strPath + @"\" + sOutFile, FileMode.Create, FileAccess.Write, FileShare.None))
                using (GZipStream str = new GZipStream(outFile, CompressionMode.Compress))
                    foreach (string sFilePath in sFiles)
                    {
                        string sRelativePath = sFilePath.Substring(iDirLen);
                        CompressFile(sInDir, sRelativePath, str);
                    }
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                return false;
            }

            return true;

        }
        
        private byte[] GetZipFileBytes(string strFileName, int iOffset, int iFileSize)
        {
            byte[] PackArray = new byte[iFileSize];

            if (File.Exists(strFileName)) //파일이 있으면 
            {

                FileStream FS = null;
                BinaryReader BR = null;
                try
                {
                    FS = new System.IO.FileStream(strFileName, FileMode.Open, FileAccess.Read);
                    BR = new System.IO.BinaryReader(FS);
                    FS.Seek(iOffset, 0);
                    int i = BR.Read(PackArray, 0, iFileSize);

                }
                catch (Exception e)
                {
                    string messs = String.Empty;
                    messs = e.Message;

                }
                finally
                {
                    if (BR != null) BR.Close();
                    if (FS != null) FS.Close();
                }



            }

            return PackArray;
        }

        private string MakeStringListFile(List<string> tmpList)
        {
            string strString = String.Empty;

            for (int i = 0; i < tmpList.Count; i++)
            {
                if (i < tmpList.Count - 1)
                {
                    strString += tmpList[i] + ",";
                }
                else
                {
                    strString += tmpList[i];
                }
                
            }
            return strString;
        }

        public void SetJobFileList(string strJobListString)
        {
            strJobListFile = strJobListString;
        }

        public void SetLogFolderFileList(List<string> tmpList)
        {
            lstLogFolderFile = tmpList;
        }

        public void SetResultFolderFileList(List<string> tmpList)
        {
            lstResultFolderFile = tmpList;
        }

        public void SetScreenFolderFileList(List<string> tmpList)
        {
            lstScreenFolderFile = tmpList;
        }

        private void RecvProccess(byte[] byteRecvPacks, int iReceivedCount)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304
            int iMSGTYPE = (int)SOCKETCODE.NONE;

            byte[] bCommand = new byte[3];
            byte[] bData = new byte[8192];

            for (int i = 0; i < iReceivedCount; i++)
            {
                ListSecondBuffer.Add(byteRecvPacks[i]);
            }

            bool bContinue = false;

            int iCheckCount = 0;

            while (true)
            {

                if (bContinue) break;

                if (ListSecondBuffer.Count < 11) break;

                byte[] bTempBytes = ListSecondBuffer.ToArray();
                iCheckCount = bTempBytes.Length;         

                //[#1] 헤더 검사 
                if (bTempBytes[0] == STX0 && bTempBytes[1] == STX1)
                {
                    //[#2] 데이터 저장 및 테일 검사                    
                    int iReqSize = BitConverter.ToInt32(bTempBytes, 5);
                    bData = new byte[iReqSize];
                    if (ListSecondBuffer.Count < 11 + iReqSize) break;
                    Array.Copy(bTempBytes, 2 + 3 + 4, bData, 0, bData.Length);

                    //[#3] ETX 검사
                    if (bTempBytes[2 + 3 + 4 + iReqSize] == ETX0 && bTempBytes[2 + 3 + 4 + iReqSize + 1] == ETX1)
                    {
                        //[#4] 메시지 분류
                        if (ListSecondBuffer.Count < 2 + 3 + 4 + iReqSize + 2) break;

                        Array.Copy(bTempBytes, 2, bCommand, 0, bCommand.Length);
                        string strCommand = Encoding.UTF8.GetString(bCommand);

                        try
                        {
                            ListSecondBuffer.RemoveRange(0, 2 + 3 + 4 + iReqSize + 2);
                            if (ListSecondBuffer.Count == 0) bContinue = true;
                        }
                        catch
                        {
                            ListSecondBuffer.Clear();
                            return;
                        }
                        

                        switch (strCommand)
                        {
                            case "S01": iMSGTYPE = (int)SOCKETCODE.S01;                                
                                break; //파일 정보 수신

                            case "S02": iMSGTYPE = (int)SOCKETCODE.S02; 
                                break; //파일 데이터 수신

                            case "S03": iMSGTYPE = (int)SOCKETCODE.S03;
                                break; //REBOOT 명령 수신

                            case "S04": iMSGTYPE = (int)SOCKETCODE.S04;
                                break; //SELECT 명령 수신

                            case "S05": iMSGTYPE = (int)SOCKETCODE.S05;
                                break; //JOBFILE LIST 명령 수신

                            case "S06": iMSGTYPE = (int)SOCKETCODE.S06;
                                break; //LOG FOLDER LIST 명령 수신

                            case "S07": iMSGTYPE = (int)SOCKETCODE.S07;
                                break; //RESULT FOLDER LIST 명령 수신

                            case "S08": iMSGTYPE = (int)SOCKETCODE.S08;
                                break; //SCREEN FOLDER LIST 명령 수신

                            case "S09": iMSGTYPE = (int)SOCKETCODE.S09;
                                break; //FILE 요청 명령 수신

                            case "S10": iMSGTYPE = (int)SOCKETCODE.S10;
                                break; //모니터링 스타트 요청 명령 수신

                            case "S11": iMSGTYPE = (int)SOCKETCODE.S11;
                                break; //모니터링 스타트 요청 명령 수신

                            default:
                                break;
                        }
                    }
                }

                switch (iMSGTYPE)
                {
                    case (int)SOCKETCODE.S01: // 파일 사이즈와 이름이 오므로 해당 파일을 생성한다.
                                                string strRecvPack = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                                string[] strPack = strRecvPack.Split(',');

                                                if (strPack.Length != 3) return;
                                                
                                                if (!strPack[0].Equals("ROOT"))
                                                {
                                                    strRecvFileName = strPack[0] + @"\" + strPack[1];
                                                }
                                                else
                                                {
                                                    strRecvFileName = strPack[1];
                                                }
                                                
                                                iTotalFileSize = uint.Parse(strPack[2]);
                                                iNowFileSize = 0;
                                                CreateFile(strRecvFileName);
                                                //SendFileRecvReady();
                                                MessageGateWay((int)SOCKETCODE.MSG, strRecvFileName + " (" + iTotalFileSize.ToString() + " Bytes)");
                                                break;

                    case (int)SOCKETCODE.S02:
                                                WriteBinFile(strRecvFileName, bData);// 파일 실제 바이너리가 온다.
                                                iNowFileSize += (uint)bData.Length;
                                                //TransfferGateWay(iNowFileSize, iTotalFileSize);
                                                long lVal = (long)iNowFileSize * 100 / (long)iTotalFileSize;
                                                MessageGateWay((int)SOCKETCODE.MSG, strRecvFileName + " Download... " + lVal.ToString() + " %");
                                                
                                                if (iNowFileSize == iTotalFileSize)
                                                {
                                                    // 다운로드 완료 처리 부분
                                                    RenameFile(strRecvFileName);
                                                    MessageGateWay((int)SOCKETCODE.MSG, strRecvFileName + " (Download Complete)");
                                                }
                                                break;

                    case (int)SOCKETCODE.S03:  //프로그램 재시작 명령
                                                MessageGateWay((int)SOCKETCODE.S03, "");                                                
                                                break;

                    case (int)SOCKETCODE.S04:   //JOB 변경명령
                                                string strJobName = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                                MessageGateWay((int)SOCKETCODE.S04, strJobName);
                                                break;

                    case (int)SOCKETCODE.S05:   //JOB 리스트 전송 명령
                                                MessageGateWay((int)SOCKETCODE.S05, "");
                                                System.Threading.Thread.Sleep(1000);                      
                                                SendFileCommand("JOBLIST");
                                                break;

                    case (int)SOCKETCODE.S06:   //LOG 폴더 리스트 전송 명령
                                                MessageGateWay((int)SOCKETCODE.S06, "");
                                                System.Threading.Thread.Sleep(1000);
                                                SendFileCommand("LOGFOLDERLIST");
                                                break;

                    case (int)SOCKETCODE.S07:   //RESULT 폴더 리스트 전송 명령
                                                MessageGateWay((int)SOCKETCODE.S07, "");
                                                System.Threading.Thread.Sleep(1000);
                                                SendFileCommand("RESULTFOLDERLIST");
                                                break;

                    case (int)SOCKETCODE.S08:   //RESULT 폴더 리스트 전송 명령
                                                MessageGateWay((int)SOCKETCODE.S08, "");
                                                System.Threading.Thread.Sleep(1000);
                                                SendFileCommand("RESULTSCREENLIST");
                                                break;

                    case (int)SOCKETCODE.S09:   // 파일  PATH 정보가 오므로 해당 path를 설정해두고 보낸다.
                                                string strFilePath = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);                                                ;
                                                if (strFilePath.Length < 3) return;
                                                strDownloadPath = strFilePath;                    
                                                MessageGateWay((int)SOCKETCODE.S09, "");
                                                System.Threading.Thread.Sleep(1000);
                                                SendFileCommand("REQUEST_FILEDOWNLOAD");
                                                break;

                    case (int)SOCKETCODE.S10:   //모니터링 데이터 리스타트 전송 명령
                                                MessageGateWay((int)SOCKETCODE.S10, "");
                                                break;

                    case (int)SOCKETCODE.S11:   //JOB 삭제명령
                                                string strDJobName = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                                MessageGateWay((int)SOCKETCODE.S11, strDJobName);
                                                break;
                    default: break;
                }          

            }


               
        }

        public bool CreateFile(string strName)
        {

            string strPath = System.Windows.Forms.Application.StartupPath;
            string strCheckFile = strPath + @"\" + strName + "_DKUPDATE";

            if (File.Exists(strCheckFile))
            {
                try
                {
                    File.Delete(strCheckFile);

                }
                catch { }
            }
            return true;            
        }

        public void RenameFile(string strName)
        {            
            string strPath = System.Windows.Forms.Application.StartupPath;
            string strFullName = strPath + @"\" + strName + "_OLDFILE";
            string strReName = strPath + @"\" + strName + "_DKUPDATE"; 
            if (File.Exists(strFullName))
            {   
                try
                {
                    File.Move(strFullName, strReName);
                  
                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                }
            }
           
        }        

        private bool WriteBinFile(string strFileName, byte[] bWriteData)
        {
            FileStream FS = null;
            BinaryWriter BW = null;     
           
            string strPath = System.Windows.Forms.Application.StartupPath;   
            try
            {
                //Prevent 2015.03.26 DK.SIM                
                FS = new FileStream(strPath + @"\" + strFileName + "_OLDFILE", FileMode.Append);
                BW = new BinaryWriter(FS);
                BW.Write(bWriteData);
                BW.Close();
                FS.Close();
               
            }
            catch (Exception ex) 
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }

            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (BW != null) ((IDisposable)BW).Dispose();               
            }


            return true;

        }

        private void TransfferGateWay(uint A, uint B)
        {
            long lVal = (long)A * 100 / (long)B;
            string strMessage = A.ToString() + " / " + B.ToString();
          
            if (SockTransffer != null)
            {
                SockTransffer(1, lVal, strMessage);
            }
            
        }

        private void MessageGateWay(int iMsgCode, string cParam)
        {
            string strMsg = cParam;
            if (SockRealTimeMsg != null)
            {
                SockRealTimeMsg(1, iMsgCode, strMsg);
            }
            
        }
                 
    }

    //서버용
    class DK_SOCKET_SERVER
    {       
        private int            iConnPort;
        private int            iMaxUser;
        private Socket         sockServer;
        private AsyncNode[]    NodeUsers;
        private MornitorData[] NodeData;        
        private bool[]         bUsingNode;
        private bool           bFileTransffering;
        
        private string strRecvFileName;

        private uint iTotalFileSize;
        private uint iNowFileSize;

      
        private byte STX0 = 0x01;
        private byte STX1 = 0x02;

        private byte ETX0 = 0x03;
        private byte ETX1 = 0x04;
        
        public event EventRealTimeMsg  SockRealTimeMsg;
        public event EventTransfferMsg SockTransffer;
        public event EventFileList     SockRecvFileList;

        private List<byte>[] SvrListSecondBuffer;

        public DK_SOCKET_SERVER(int iMaxUserCount, int iOpenPort)
        {
            NodeUsers  = new AsyncNode[iMaxUserCount];
            NodeData   = new MornitorData[iMaxUserCount];
            bUsingNode = new bool[iMaxUserCount];
            SvrListSecondBuffer = new List<byte>[iMaxUserCount];
            for (int i = 0; i < iMaxUserCount; i++)
            {
                SvrListSecondBuffer[i] = new List<byte>();
            }
            iConnPort = iOpenPort;
            iMaxUser   = iMaxUserCount;
            bFileTransffering = false;
        }

        public void ServerOpen()
        {
            try
            {
                sockServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, iConnPort);

                sockServer.Bind(ipep);
                sockServer.Listen(iMaxUser);

                sockServer.BeginAccept(new AsyncCallback(AcceptConnection), null);
                MessageGateWay("ServerOpen.");
            }

            catch
            {
                MessageGateWay("Can Not Open Server.");
            }   

        }

        public void ServerClose()
        {
            for (int i = 0; i < iMaxUser; i++)
            {
                if (bUsingNode[i])
                {
                    bUsingNode[i] = false;
                    try
                    {
                        if (NodeUsers[i].sockWorker != null)
                        {
                            NodeUsers[i].sockWorker.Close();
                            NodeUsers[i].sockWorker.Dispose();
                        }  
                    }
                    catch {  }                                      
                }
                ClearNodeData(i);
            }

            if (sockServer != null)
            {
                try
                {
                    sockServer.Close();
                    sockServer.Dispose();
                    sockServer = null;
                }
                catch { }  
            }
            
            MessageGateWay("ServerClose.");
        }

        private void AcceptConnection(IAsyncResult IAR)
        {           
            if (sockServer == null) return;

            int iNodeNumber = -1;

            for (int i = 0; i < bUsingNode.Length; i++)
            {
                if (!bUsingNode[i]) //비어있는 노드가 있으면 그곳에 클라이언트를 연결시킨다.
                {
                    try
                    {
                        iNodeNumber = i;
                        bUsingNode[i] = true;
                        NodeUsers[i] = new AsyncNode(i);
                        NodeUsers[i].sockWorker = sockServer.EndAccept(IAR);
                        MessageGateWay("Connected[" + iNodeNumber.ToString() + "] - " + IPAddress.Parse(((IPEndPoint)NodeUsers[i].sockWorker.RemoteEndPoint).Address.ToString()).ToString());
                        
                        AsyncNode Antempler = new AsyncNode(i);
                        Antempler.sockWorker = NodeUsers[i].sockWorker;
                        Antempler.ClearBuffer();
                        Antempler.sockWorker.BeginReceive(Antempler.Buffer, 0, Antempler.Buffer.Length, SocketFlags.None, handleDataRecv, Antempler);
                        SendControlCommand(i, "STATUSRESTART", String.Empty);
                        sockServer.BeginAccept(new AsyncCallback(AcceptConnection), null);                        
                        return;
                    }
                    catch (Exception ex)
                    {
                        string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                        STEPMANAGER_VALUE.DebugView(strExMsg); 

                        bUsingNode[i] = false;                        
                        if (NodeUsers[i].sockWorker != null)
                        {
                            NodeUsers[i].sockWorker.Close();
                            NodeUsers[i].sockWorker.Dispose();
                        }
                        return;
                    }
                    
                }
            }
            //# 모든 노드가 꽉찼으면 LISTEN 중단처리 넣어야함.
            if (iNodeNumber == -1)
            {
                try
                {
                    sockServer.Close();
                    sockServer.Dispose();
                    sockServer = null;
                }
                catch 
                {
                    MessageGateWay("Server Port Full.");
                }                
                return;
            }

        }
        
        private void DisconnectNode(AsyncNode an)
        {
            bFileTransffering = false;
            ClearNodeData(an.GetNodeNumber());
            an.sockWorker.Close();
            an.sockWorker.Dispose();
            bUsingNode[an.GetNodeNumber()] = false;
            MessageGateWay("Disconnect Client[" + an.GetNodeNumber() +"]");
        }

        private void handleDataRecv(IAsyncResult IAR)
        {
            AsyncNode AnObserver = (AsyncNode)IAR.AsyncState;

            int iRecvBytes = 0;

            try
            {
                iRecvBytes = AnObserver.sockWorker.EndReceive(IAR);
                bFileTransffering = false;
            }
            catch 
            {
                DisconnectNode(AnObserver);                
                return;
            }

            if (iRecvBytes > 0)
            {
                //수신바이트 내용 처리부분.
                ParsePacketData(AnObserver.GetNodeNumber(), AnObserver.Buffer, iRecvBytes);
               
                try
                {
                    //재수신 모드
                    AnObserver.ClearBuffer();
                    AnObserver.sockWorker.BeginReceive(AnObserver.Buffer, 0, AnObserver.Buffer.Length, SocketFlags.None, handleDataRecv, AnObserver);
                }
                catch
                {
                    DisconnectNode(AnObserver);
                    return;
                }
            }
            else
            {
                DisconnectNode(AnObserver);                
                return;
            }
        }

        private void MessageGateWay(string cParam)
        {
            string strMsg = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "] " + cParam;
            SockRealTimeMsg(1, strMsg);
        }

        private void ParsePacketData(int idx, byte[] byteRecvPacks, int iRecievedCount)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304
            int iMSGTYPE = (int)SOCKETCODE.NONE;

            byte[] bCommand = new byte[3];
            byte[] bData = new byte[8192];

            int iDebugCount = SvrListSecondBuffer[idx].Count;
            for (int i = 0; i < iRecievedCount; i++)
            {
                SvrListSecondBuffer[idx].Add(byteRecvPacks[i]);
            }

            bool bContinue = false;
            
            while (true)
            {
                if (bContinue) break;

                if (SvrListSecondBuffer[idx].Count < 11) break;

                byte[] bTempBytes = SvrListSecondBuffer[idx].ToArray();
                    
                //[#1] 헤더 검사 
                if (bTempBytes[0] == STX0 && bTempBytes[1] == STX1)
                {

                    //[#2] 데이터 저장 및 테일 검사                    
                    int iReqSize = BitConverter.ToInt32(bTempBytes, 5);
                    bData = new byte[iReqSize];
                    if (SvrListSecondBuffer[idx].Count < 11 + iReqSize) break;

                    Array.Copy(bTempBytes, 2 + 3 + 4, bData, 0, bData.Length);

                    //[#3] ETX 검사
                    if (bTempBytes[2 + 3 + 4 + iReqSize] == ETX0 && bTempBytes[2 + 3 + 4 + iReqSize + 1] == ETX1)
                    {
                        //[#4] 메시지 분류
                        if (SvrListSecondBuffer[idx].Count < 2 + 3 + 4 + iReqSize + 2) break;


                        Array.Copy(bTempBytes, 2, bCommand, 0, bCommand.Length);
                        string strCommand = Encoding.UTF8.GetString(bCommand);

                        try
                        {
                            SvrListSecondBuffer[idx].RemoveRange(0, 2 + 3 + 4 + iReqSize + 2);
                            if (SvrListSecondBuffer[idx].Count == 0) bContinue = true;
                          
                        }
                        catch 
                        {
                            SvrListSecondBuffer[idx].Clear();
                            return;
                        }
                        

                        switch (strCommand)
                        {
                            case "C01": iMSGTYPE = (int)SOCKETCODE.C01; break; //모니터링 데이터
                            case "C02": iMSGTYPE = (int)SOCKETCODE.C02; break; //파일 전송 대기 완료                          
                            case "C03": iMSGTYPE = (int)SOCKETCODE.C03; break; //JOB LIST 데이터
                            case "C04": iMSGTYPE = (int)SOCKETCODE.C04; break; //LOG FOLDER LIST 데이터
                            case "C05": iMSGTYPE = (int)SOCKETCODE.C05; break; //RESULT FOLDER LIST 데이터
                            case "C06": iMSGTYPE = (int)SOCKETCODE.C06; break; //SCREEN FOLDER LIST 데이터
                            case "C07": iMSGTYPE = (int)SOCKETCODE.C07; break; //DOWNLOAD FILE SIZE 데이터
                            case "C08": iMSGTYPE = (int)SOCKETCODE.C08; break; //DOWNLOAD BINARY    데이터
                            case "CCC": iMSGTYPE = (int)SOCKETCODE.CCC; break; //DOWNLOAD BINARY 없음 데이터
                            default:
                                return;
                        }
                        string strFolderPack = String.Empty;
                        string strRecvPack = String.Empty;
                        string[] strFolderList;
                        switch (iMSGTYPE)
                        {
                            case (int)SOCKETCODE.C01:
                                strRecvPack = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                string[] strPack = strRecvPack.Split(',');

                                if (strPack.Length != 5) return;

                                NodeData[idx].strName = strPack[0];
                                NodeData[idx].strIP = strPack[1];
                                NodeData[idx].strJobName = strPack[2];
                                NodeData[idx].strVersion = strPack[3];
                                NodeData[idx].strStatus = strPack[4];
                                break;

                            case (int)SOCKETCODE.C03:
                                string strFilePack = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                string[] strFileList = strFilePack.Split(',');
                                SockRecvFileList(idx, (int)SOCKETCODE.C03, strFileList);
                                SvrListSecondBuffer[idx].Clear();
                                return;


                            case (int)SOCKETCODE.C04: // LOG    폴더 리스트 
                            case (int)SOCKETCODE.C05: // RESULT 폴더 리스트
                            case (int)SOCKETCODE.C06: // SCREEN 폴더 리스트
                                strFolderPack = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                strFolderList = strFolderPack.Split(',');
                                SockRecvFileList(idx, iMSGTYPE, strFolderList);
                                SvrListSecondBuffer[idx].Clear();
                                return;

                            case (int)SOCKETCODE.C07: // LOG, RESULT, SCREEN FILES 사이즈 . 파일 사이즈가 이름이 오므로 해당 파일을 생성한다.
                                strRecvPack = Encoding.UTF8.GetString(bData).Replace("\0", String.Empty);
                                string[] strLogPack = strRecvPack.Split(',');
                                if (strLogPack.Length != 2) return;                              
                                iTotalFileSize = uint.Parse(strLogPack[1]);
                                iNowFileSize = 0;
                                NewZipFile(NodeData[idx].strName, strLogPack[0], ref strRecvFileName);
                                break;

                            case (int)SOCKETCODE.C08: //다운로드파일 바이너리 수신

                                WriteZipFile(strRecvFileName, bData);// 파일 실제 바이너리가 온다.
                                iNowFileSize += (uint)bData.Length;
                                string strType = String.Empty;
                                if (strRecvFileName.Contains(@"\LOG"))         { strType = "LOG"; }
                                else if (strRecvFileName.Contains(@"\RESULT")) { strType = "RESULT"; }
                                else if (strRecvFileName.Contains(@"\SCREEN")) { strType = "SCREEN"; }
                                else if (strRecvFileName.Contains(@"\DATA"))   { strType = "DATA"; }
                                else { return; }
                                RecieverGateWay(iNowFileSize, iTotalFileSize, strType);
                                                
                                if (iNowFileSize == iTotalFileSize)
                                {
                                    // 다운로드 완료 압축해제
                                    string strDestination = strRecvFileName.Replace(".zip", String.Empty);
                                    DecompressToDirectory(strRecvFileName, strDestination);
                                    SendControlCommand(idx, "STATUSRESTART", String.Empty);
                                }
                                else if (iNowFileSize > iTotalFileSize)
                                {
                                    SendControlCommand(idx, "STATUSRESTART", String.Empty);
                                }
                                break;

                            case (int)SOCKETCODE.CCC: //다운로드파일 바이너리 없음 수신
                                string strType1 = String.Empty;
                                if (!String.IsNullOrEmpty(strRecvFileName))
                                {
                                    if (strRecvFileName.Contains(@"\LOG")) { strType1 = "LOG"; }
                                    else if (strRecvFileName.Contains(@"\RESULT")) { strType1 = "RESULT"; }
                                    else if (strRecvFileName.Contains(@"\SCREEN")) { strType1 = "SCREEN"; }
                                    else if (strRecvFileName.Contains(@"\DATA")) { strType1 = "DATA"; }
                                    else { return; }
                                    RecieverGateWay(0, 10000, strType1 + "(NONE)");
                                    SvrListSecondBuffer[idx].Clear();
                                    return;
                                }
                                else
                                {
                                    RecieverGateWay(0, 10000, "LOG(NONE)");
                                    RecieverGateWay(0, 10000, "RESULT(NONE)");
                                    RecieverGateWay(0, 10000, "SCREEN(NONE)");
                                    RecieverGateWay(0, 10000, "DATA(NONE)");
                                    SvrListSecondBuffer[idx].Clear();
                                    return;
                                }

                            default: break;
                        } 

                    }
                    else
                    {
                        try
                        {
                            SvrListSecondBuffer[idx].RemoveRange(0, 2 + 3 + 4 + iReqSize + 1);
                        }
                        catch { }
                        int iRemoveCount = 0;
                        for (int i = 0; i < SvrListSecondBuffer[idx].Count; i++)
                        {
                            if (SvrListSecondBuffer[idx][i] == STX0 && SvrListSecondBuffer[idx][i + 1] == STX1)
                            {
                                break;
                            }
                            iRemoveCount++;
                        }

                        try
                        {
                            SvrListSecondBuffer[idx].RemoveRange(0, iRemoveCount);

                        }
                        catch
                        {
                            SvrListSecondBuffer[idx].Clear();
                            return;
                        }

                    }
                    

                }
                else
                {       
                    int iRemoveCount = 0;
                    for (int i = 0; i < SvrListSecondBuffer[idx].Count; i++)
                    {
                        if (SvrListSecondBuffer[idx][i] == STX0 && SvrListSecondBuffer[idx][i + 1] == STX1)
                        {
                            break;
                        }
                        iRemoveCount++;
                    }

                    try
                    {
                        SvrListSecondBuffer[idx].RemoveRange(0, iRemoveCount);
                      
                    }
                    catch
                    {
                        SvrListSecondBuffer[idx].Clear();
                        return;
                    }

                }

            }

                                    

        }

        private void handleDataSend(IAsyncResult IAR)
        {
            AsyncNode AnObserver = (AsyncNode)IAR.AsyncState;

            int iRecvBytes = 0;

            try
            {
                iRecvBytes = AnObserver.sockWorker.EndSend(IAR);
            }
            catch { }
        }      
                
        public void FileSendProcess(int idx, string strFileName, string strFolder)
        {
            if (GetFileTrasfferingStatus()) return;
            SendFileInfo(idx, strFileName, strFolder);            
        }

        public void ClientRebooting(int idx) 
        {   //원격PC 재부팅 명령
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "REBOOT", String.Empty);
        }

        public void SelectJobCommand(int idx, string strJobName)
        {   //원격PC JOB파일선택변경
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "SELECT", strJobName);
        }

        public void DeleteJobCommand(int idx, string strJobName)
        {   //원격PC JOB파일삭제
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "DELETE", strJobName);
        }

        public void GetJobListCommand(int idx)
        {   //원격PC JOB파일 리스트 요청
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "GETJOB", String.Empty);
        }

        public void GetLogFolderListCommand(int idx)
        {   //원격PC 로그폴더 리스트 요청
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "GETLOGFOLDER", String.Empty);
        }

        public void GetResultFolderListCommand(int idx)
        {   //원격PC RESULT폴더 리스트 요청
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "GETRESULTFOLDER", String.Empty);
        }

        public void GetScreenFolderListCommand(int idx)
        {   //원격PC RESULT폴더 리스트 요청
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "GETSCREENFOLDER", String.Empty);
        }

        public void DownloadFilesCommand(int idx, string strPath)
        {   //원격PC 파일 다운로드 요청
            if (GetFileTrasfferingStatus()) return;
            SendControlCommand(idx, "REQDOWNLOAD", strPath);
        }    

        private void AutoFileSendLoop(int idx, string strFileName)
        {
            if (NodeUsers[idx].iFileSendNowBytes >= NodeUsers[idx].iFileSendTotalBytes) return;
            SendFileData(idx, strFileName);
        }
        //S01
        private void SendFileInfo(int idx, string strFileName, string strFolder)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304                        
            
            //파일 정보 읽어오고, 파일 이름과 사이즈를 저장해두기.
            string strFileSize = String.Empty;
            string strMessage = String.Empty;
            if (GetFileSize(strFileName, ref strFileSize))
            {
                // 파일명만 추려 보낸다.
                NodeUsers[idx].strFullFileName = strFileName;
                NodeUsers[idx].strSendFileName = strFileName.Substring(strFileName.LastIndexOf(@"\") + 1);
                NodeUsers[idx].iFileSendTotalBytes = uint.Parse(strFileSize);
                NodeUsers[idx].iFileSendNowBytes = 0;
                strMessage = strFolder + "," + strFileName.Substring(strFileName.LastIndexOf(@"\") + 1) + "," + strFileSize;
            }
            else
            {
                return;
            }
            

            byte[] Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
            byte[] Commandbytes = Encoding.UTF8.GetBytes("S01");
            byte[] Databytes = Encoding.UTF8.GetBytes(strMessage);
            byte[] Lengthbytes = BitConverter.GetBytes(Databytes.Length);

            Sendbytes[0] = STX0;
            Sendbytes[1] = STX1;

            Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
            Array.Copy(Lengthbytes, 0, Sendbytes, 5, Lengthbytes.Length);
            Array.Copy(Databytes, 0, Sendbytes, 9, Databytes.Length);

            Sendbytes[Sendbytes.Length - 2] = ETX0;
            Sendbytes[Sendbytes.Length - 1] = ETX1;

            try
            {
                bFileTransffering = true;
                AsyncNode Antempler = new AsyncNode(idx);
                Antempler.sockWorker = NodeUsers[idx].sockWorker;
                Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSendLoop, Antempler);

            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                bFileTransffering = false;
            }
        }        
        //S02
        private void SendFileData(int idx, string strFileName)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304 
            
            //파일 읽어오기
            int iByteSize = (2048 - 2 + 3 + 4 + 2);
            if (NodeUsers[idx].iFileSendNowBytes + iByteSize >= NodeUsers[idx].iFileSendTotalBytes)
            {
                iByteSize = (int)(NodeUsers[idx].iFileSendTotalBytes - NodeUsers[idx].iFileSendNowBytes);
            }           

            byte[] Databytes = GetFileBytes(strFileName, (int)NodeUsers[idx].iFileSendNowBytes, iByteSize);
          
            
            NodeUsers[idx].iFileSendNowBytes += (uint)iByteSize;

            byte[] Sendbytes = new byte[2 + 3 + 4 + Databytes.Length + 2];
            byte[] Commandbytes = Encoding.UTF8.GetBytes("S02");
            byte[] Lengthbytes = BitConverter.GetBytes(Databytes.Length);

            Sendbytes[0] = STX0;
            Sendbytes[1] = STX1;

            Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
            Array.Copy(Lengthbytes,  0, Sendbytes, 5, Lengthbytes.Length);
            Array.Copy(Databytes,    0, Sendbytes, 9, Databytes.Length);

            Sendbytes[Sendbytes.Length - 2] = ETX0;
            Sendbytes[Sendbytes.Length - 1] = ETX1;

            try
            {
                TransfferGateWay(NodeUsers[idx].iFileSendNowBytes, NodeUsers[idx].iFileSendTotalBytes);
                //MessageGateWay("SEND : " + NodeUsers[idx].iFileSendNowSequence.ToString() + " / " + NodeUsers[idx].iFileSendTotalSequence.ToString());
                bFileTransffering = true;
                AsyncNode Antempler = new AsyncNode(idx);
                Antempler.sockWorker = NodeUsers[idx].sockWorker;
                Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSendLoop, Antempler);

            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                bFileTransffering = false;
            }
        }
        //S03
        private void SendControlCommand(int idx, string strControlName, string strParam)
        {
            //  HEADER ,   COMMAND , DATALENTH , DATA    , TAILER
            //  2byte  ,   3byte   , 4byte     , xByte  , 2byte
            //  0x0102 ,                                 , 0x0304                        

            byte[] Sendbytes;
            byte[] Commandbytes;
            byte[] Databytes;
            byte[] Lengthbytes;

            string strMessage = strControlName;

            //strControlName
            //1. REBOOT (PC 프로그램 재실행 명령)
            //2. SELECT (JOB 파일 선택명령)

            switch (strMessage)
            {
                case "REBOOT": 
                                Sendbytes    = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S03");
                                Databytes    = Encoding.UTF8.GetBytes(strMessage);
                                Lengthbytes  = BitConverter.GetBytes(Databytes.Length);
                                break;
                case "SELECT":
                                int iFileNameLen = Encoding.UTF8.GetByteCount(strParam);
                                Sendbytes    = new byte[2 + 3 + 4 + iFileNameLen + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S04");
                                Databytes    = Encoding.UTF8.GetBytes(strParam);  //변경 절차서 이름
                                Lengthbytes  = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "GETJOB":              
                                Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S05");
                                Databytes   = Encoding.UTF8.GetBytes(strMessage);  //절차서 리스트
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "GETLOGFOLDER":
                                Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S06");
                                Databytes   = Encoding.UTF8.GetBytes(strMessage);  //LOG 폴더 리스트
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "GETRESULTFOLDER":
                                Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S07");
                                Databytes = Encoding.UTF8.GetBytes(strMessage);  //RESULT 폴더 리스트
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "GETSCREENFOLDER":
                                Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S08");
                                Databytes = Encoding.UTF8.GetBytes(strMessage);  //SCREEN 폴더 리스트
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "REQDOWNLOAD":

                                Sendbytes = new byte[2 + 3 + 4 + strParam.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S09");
                                Databytes = Encoding.UTF8.GetBytes(strParam);  // FILE 다운로드 요청
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "STATUSRESTART":

                                Sendbytes = new byte[2 + 3 + 4 + strMessage.Length + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S10");
                                Databytes = Encoding.UTF8.GetBytes(strMessage);  // 모니터링 재시작 요청
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;

                case "DELETE":
                                int iFileNameLenD = Encoding.UTF8.GetByteCount(strParam);
                                Sendbytes = new byte[2 + 3 + 4 + iFileNameLenD + 2];
                                Commandbytes = Encoding.UTF8.GetBytes("S11");
                                Databytes = Encoding.UTF8.GetBytes(strParam);  //변경 절차서 이름
                                Lengthbytes = BitConverter.GetBytes(Databytes.Length);
                                break;


                default: return;
            }

            Sendbytes[0] = STX0;
            Sendbytes[1] = STX1;

            try
            {
                Array.Copy(Commandbytes, 0, Sendbytes, 2, Commandbytes.Length);
                Array.Copy(Lengthbytes, 0, Sendbytes, 5, Lengthbytes.Length);
                Array.Copy(Databytes, 0, Sendbytes, 9, Databytes.Length);
            }
            catch 
            {
                return;
            }
            

            Sendbytes[Sendbytes.Length - 2] = ETX0;
            Sendbytes[Sendbytes.Length - 1] = ETX1;

            try
            {
                bFileTransffering = true;
                AsyncNode Antempler = new AsyncNode(idx);
                Antempler.sockWorker = NodeUsers[idx].sockWorker;
                Antempler.sockWorker.BeginSend(Sendbytes, 0, Sendbytes.Length, SocketFlags.None, handleDataSendLoop, Antempler);

            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg); 
                bFileTransffering = false;
            }
        }        

        private void handleDataSendLoop(IAsyncResult IAR)
        {            
            AsyncNode AnObserver = (AsyncNode)IAR.AsyncState;

            int iRecvBytes = 0;

            try
            {
                iRecvBytes = AnObserver.sockWorker.EndSend(IAR);

                if (NodeUsers[AnObserver.GetNodeNumber()].iFileSendNowBytes
                        < NodeUsers[AnObserver.GetNodeNumber()].iFileSendTotalBytes)
                {
                    System.Threading.Thread.Sleep(1);
                    AutoFileSendLoop(AnObserver.GetNodeNumber(), NodeUsers[AnObserver.GetNodeNumber()].strFullFileName);
                }
                else
                {
                    NodeUsers[AnObserver.GetNodeNumber()].iFileSendNowBytes = 0;
                    NodeUsers[AnObserver.GetNodeNumber()].iFileSendTotalBytes = 0;
                }
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);            
            }
        }

        private bool GetFileTrasfferingStatus()
        {
            return bFileTransffering;
        }

        private void ClearNodeData(int idx)
        {
            NodeData[idx].strName    = String.Empty;
            NodeData[idx].strIP      = String.Empty;
            NodeData[idx].strJobName = String.Empty;
            NodeData[idx].strVersion = String.Empty;
            NodeData[idx].strStatus  = String.Empty;
        }

        public MornitorData GetNodeData(int idx)
        {
            return NodeData[idx];
        }

        public bool[] GetConnectionNodeList()
        {
            return bUsingNode;
        }

        public bool GetNodeNumber(ref int iNodeNumber, string strName, string strIPaddress)
        {
            iNodeNumber = -1;
            for(int i = 0; i < NodeData.Length; i++)
            {
                try
                {
                    if (NodeData[i].strName.Equals(strName) && NodeData[i].strIP.Equals(strIPaddress))
                    {
                        if (bUsingNode[i])
                        {
                            iNodeNumber = i;
                            return true;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                    return false;
                }
                
            }
            return false;
        }

        private void TransfferGateWay(uint A, uint B)
        {
            long lVal = (long)A * 100 / (long)B;
            string strMessage = String.Empty;

            if (A == B)
            {
                strMessage = "[ COMPLETE ] " + A.ToString() + " / " + B.ToString();
            }

            if (A > B)
            {
                strMessage = "[ FAILUAR ] " + A.ToString() + " / " + B.ToString();
            }

            if (A < B)
            {
                strMessage = "FILE UPLOAD... " + A.ToString() + " / " + B.ToString();
            }

            if (SockTransffer != null)
            {
                SockTransffer(1, lVal, strMessage);
            }            
        }

        private void RecieverGateWay(uint A, uint B, string strType)
        {
            long lVal = (long)A * 100 / (long)B;
            string strMessage = String.Empty;

            if (strType.Contains("(NONE)"))
            {
                strMessage = strType + " - File Empty. Cancel.";           
            }
            else
            {
                if (A == B)
                {
                    strMessage = strType + " Download - OK ( " + A.ToString() + " / " + B.ToString();
                }

                if (A > B)
                {
                    strMessage = strType + " Download - Fail ( " + A.ToString() + " / " + B.ToString();
                }

                if (A < B)
                {
                    strMessage = strType + " Downloading... ( " + A.ToString() + " / " + B.ToString();
                }
            }
            

            if (SockTransffer != null)
            {
                SockTransffer(2, lVal, strMessage);
            }
        }

        private bool GetFileSize(string strFileName, ref string strFileSize)
        {
            if (File.Exists(strFileName)) //파일이 있으면 
            {
                FileInfo fInfo = new FileInfo(strFileName);
                strFileSize = fInfo.Length.ToString();
                return true;
            }
            return false;
        }

        private byte[] GetFileBytes(string strFileName, int iOffset, int iFileSize)
        {
            byte[] PackArray = new byte[iFileSize];

            if (File.Exists(strFileName)) //파일이 있으면 
            {

                FileStream FS = null;
                BinaryReader BR = null;
                try
                {
                    FS = new System.IO.FileStream(strFileName, FileMode.Open, FileAccess.Read);
                    BR = new System.IO.BinaryReader(FS);
                    FS.Seek(iOffset, 0);
                    int i = BR.Read(PackArray, 0, iFileSize);
                    
                }
                catch(Exception ex) 
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);

                }
                finally
                {
                    if(BR != null) BR.Close();
                    if(FS != null) FS.Close();
                }

                
                               
            }

            return PackArray;
        }

        private byte[] GetFileChars(string strFileName, int iOffset, int iFileSize)
        {
            byte[] PackArray = new byte[iFileSize];
            char[] tempArray = new char[iFileSize];

            if (File.Exists(strFileName)) //파일이 있으면 
            {

                FileStream FS = null;
                StreamReader SR = null;
                try
                {
                    FS = new System.IO.FileStream(strFileName, FileMode.Open, FileAccess.Read);
                    SR = new System.IO.StreamReader(FS);
                    FS.Seek(iOffset, 0);
                    int i = SR.Read(tempArray, 0, iFileSize);

                    PackArray = Encoding.UTF8.GetBytes(tempArray);

                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                }
                finally
                {
                    if (SR != null) SR.Close();
                    if (FS != null) FS.Close();
                }
            }

            return PackArray;
        }

        public string[] GetFileList(string strFileType, ref bool bSuccess)
        {
            string[] strResult = null;
            string strProgramPath = AppDomain.CurrentDomain.BaseDirectory;

            switch (strFileType)
            {
                case "DLL": break;
                case "TBL": strProgramPath += @"SYSTEM\"; break;
                case "JOB": strProgramPath += @"DATA\";   break;
                case "MAP": strProgramPath += @"DATA\";   break;
                default: break;
            }

            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strProgramPath);
                
                FileInfo[] Fi = di.GetFiles("*." + strFileType);
                FileInfo[] Fi2 = null;
                int iFsize    = Fi.Length;

                if (strProgramPath.Contains(@"\SYSTEM"))
                {
                    Fi2 = di.GetFiles("*.INI");
                   
                    iFsize += Fi2.Length;

                    for (int i = 0; i < Fi2.Length; i++)
                    {
                        if (Fi2[i].ToString().Contains("CONFIG."))
                        {
                            iFsize--;
                        }
                    }
                }

                strResult = new string[iFsize];                
                
                for (int i = 0; i < Fi.Length; i++)
                {
                    strResult[i] = strProgramPath + Fi[i].ToString();                    
                }

                if (iFsize > Fi.Length && Fi2 != null)
                {
                    for (int i = Fi.Length; i < iFsize; i++)
                    {
                        if (!Fi2[iFsize-i].ToString().Contains("CONFIG."))
                        {
                            strResult[i] = strProgramPath + Fi2[iFsize - i].ToString();
                        }                        
                    }
                }                

                if (strResult.Length > 0) bSuccess = true;
                else bSuccess = false;
                return strResult;
            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
                bSuccess = false;
                return strResult;
            }

        }

        private bool WriteZipFile(string strFileName, byte[] bWriteData)
        {
            FileStream FS = null;
            BinaryWriter BW = null;

            try
            {
                //Prevent 2015.03.26 DK.SIM                
                FS = new FileStream(strFileName, FileMode.Append);
                BW = new BinaryWriter(FS);
                BW.Write(bWriteData);
                BW.Close();
                FS.Close();

            }
            catch (Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
            }

            //Prevent 2015.03.26 DK.SIM
            finally
            {
                if (FS != null) ((IDisposable)FS).Dispose();
                if (BW != null) ((IDisposable)BW).Dispose();
            }


            return true;

        }

        public bool NewZipFile(string strNodeName, string strFileType, ref string strRecvFileName)
        {

            string strPath = System.Windows.Forms.Application.StartupPath;
            string strDowloadFolder = strPath + @"\DOWNLOAD";            
            string strDowloadFolder2 = strDowloadFolder  + @"\" + strNodeName; 
            string strDowloadFolder3 = strDowloadFolder2 + @"\" + strFileType;      
            string strDowloadFolder4 = strDowloadFolder3 + @"\" + DateTime.Now.ToString("yyyyMMdd");      // +strName;
            string strFileFullPath   = strDowloadFolder4 + @"\" + strFileType + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".zip";     


            DirectoryInfo Di = new DirectoryInfo(strDowloadFolder);

            if (Di.Exists == false) { Di.Create(); }

            Di = new DirectoryInfo(strDowloadFolder2);

            if (Di.Exists == false) { Di.Create(); }

            Di = new DirectoryInfo(strDowloadFolder3);

            if (Di.Exists == false) { Di.Create(); }

            Di = new DirectoryInfo(strDowloadFolder4);

            if (Di.Exists == false) { Di.Create(); }



            if (File.Exists(strFileFullPath))
            {
                try
                {
                    File.Delete(strFileFullPath);

                }
                catch(Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);
                }
            }
            strRecvFileName = strFileFullPath;
            return true;
        }

        private bool DecompressToDirectory(string sCompressedFile, string sDir)
        {
            try
            {
                using (FileStream inFile = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
                using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                    while (DecompressFile(sDir, zipStream)) ;
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);    
                return false;
            }

            //압축풀고 나서 파일삭제.
            if (File.Exists(sCompressedFile))
            {
                try
                {
                    File.Delete(sCompressedFile);

                }
                catch (Exception ex)
                {
                    string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                    STEPMANAGER_VALUE.DebugView(strExMsg);                
                }
            }            
            return true;
        }

        private bool DecompressFile(string sDir, GZipStream zipStream)
        {
            try
            {
                //Decompress file name
                byte[] bytes = new byte[sizeof(int)];
                int Readed = zipStream.Read(bytes, 0, sizeof(int));
                if (Readed < sizeof(int))
                    return false;

                int iNameLen = BitConverter.ToInt32(bytes, 0);
                bytes = new byte[sizeof(char)];
                StringBuilder sb = new StringBuilder(4096);
                for (int i = 0; i < iNameLen; i++)
                {
                    zipStream.Read(bytes, 0, sizeof(char));
                    char c = BitConverter.ToChar(bytes, 0);
                    sb.Append(c);
                }
                string sFileName = sb.ToString();

                //Decompress file content
                bytes = new byte[sizeof(int)];
                zipStream.Read(bytes, 0, sizeof(int));
                int iFileLen = BitConverter.ToInt32(bytes, 0);

                bytes = new byte[iFileLen];
                zipStream.Read(bytes, 0, bytes.Length);

                string sFilePath = Path.Combine(sDir, sFileName);
                string sFinalDir = Path.GetDirectoryName(sFilePath);
                if (!Directory.Exists(sFinalDir))
                    Directory.CreateDirectory(sFinalDir);

                using (FileStream outFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    outFile.Write(bytes, 0, iFileLen);
            }
            catch(Exception ex)
            {
                string strExMsg = "Exception:" + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ex.Message; 
                STEPMANAGER_VALUE.DebugView(strExMsg);
                return false;
            }


            return true;
        }
    }

}


