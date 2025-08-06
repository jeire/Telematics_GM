using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GmTelematics
{
    public partial class FrmUpdater : Form
    {
        private DK_SOCKET_SERVER dkSckSvr;
        //private DK_SOCKET_CLIENT dkSckClt = new DK_SOCKET_CLIENT();
        private int iMAXNODE = 100;
        private System.Threading.Timer threadInfoTimer;
        private string strTargetName = String.Empty;
        private string strTargetIP = String.Empty;

        public FrmUpdater(int iProgramPort)
        {
            /*
            프로그램별로 독립적인 iProgramPort를 지정해야한다. 
            예를 들어 
            JLR     = 59670번
            OCU     = 59671번
            TCP     = 59672번
            CLUSTER = 59673번
            */
            InitializeComponent();
            dkSckSvr = new DK_SOCKET_SERVER(100, iProgramPort);
            dkSckSvr.SockRealTimeMsg += new EventRealTimeMsg(RecvMessageGateWay);
            dkSckSvr.SockTransffer   += new EventTransfferMsg(RecvTransfferGateWay);
            dkSckSvr.SockRecvFileList += new EventFileList(RecvFileListGateWay);
            SetUI();
            threadInfoTimer = new System.Threading.Timer(CallBack);
            SetFileList(listBoxDLL, "DLL");
            SetFileList(listBoxTBL, "TBL");
            SetFileList(listBoxJOB, "JOB");
            SetFileList(listBoxJOB, "MAP");
            
            
        }

        private void CallBack(object status)
        {
            bool[] bConnection = dkSckSvr.GetConnectionNodeList();
            int iConnCount = 0;
            int iMissMatchCount = 0;
            int iMatchCount = 0;
            for (int i = 0; i < bConnection.Length; i++)
            {
                if (bConnection[i])
                {
                    
                    iConnCount++;
                    if (STEPMANAGER_VALUE.strProgramVersion.Equals(dkSckSvr.GetNodeData(i).strVersion))
                    {
                        CNGridWriter(i, dkSckSvr.GetNodeData(i), false, true);
                        iMatchCount++;
                    }
                    else
                    {
                        CNGridWriter(i, dkSckSvr.GetNodeData(i), false, false);
                        iMissMatchCount++;
                    }
                }
                else
                {
                    CNGridWriter(i, dkSckSvr.GetNodeData(i), true, true);
                }
            }
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    lblConnCount.Text = iConnCount.ToString();
                    lblVerMissMatchCount.Text = iMissMatchCount.ToString();
                    lblVerMatchCount.Text = iMatchCount.ToString();
                })); 
            }
            catch { }
            

        }

        private void button6_Click(object sender, EventArgs e)
        {                    
            //dkSckClt.Connect("192.168.0.7", 10005);            
        }

        private void SetUI()
        {
            dataGridNodeList.DoubleBuffered(true);
            dataGridNodeList.Columns.Add("Col0", "NAME");
            dataGridNodeList.Columns[0].Width = (int)(dataGridNodeList.Width * 0.09);
            dataGridNodeList.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridNodeList.Columns.Add("Col1", "IP ADDRESS");
            dataGridNodeList.Columns[1].Width = (int)(dataGridNodeList.Width * 0.12);
            dataGridNodeList.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridNodeList.Columns.Add("Col2", "NOW JOB FILE");
            dataGridNodeList.Columns[2].Width = (int)(dataGridNodeList.Width * 0.34);
            dataGridNodeList.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridNodeList.Columns.Add("Col3", "VERSION");
            dataGridNodeList.Columns[3].Width = (int)(dataGridNodeList.Width * 0.34);
            dataGridNodeList.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridNodeList.Columns.Add("Col4", "STATUS");
            dataGridNodeList.Columns[4].Width = (int)(dataGridNodeList.Width * 0.09);
            dataGridNodeList.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            
            for (int i = 0; i < dataGridNodeList.Columns.Count; i++)
            {
                dataGridNodeList.Columns[i].ReadOnly = true;
            }

            dataGridNodeList.Rows.Add(iMAXNODE + 1);

            for (int i = 0; i < dataGridNodeList.Rows.Count; i++)
            {
                dataGridNodeList.Rows[i].Tag = (int)i;
            }
            
            lblTargetName.Text = System.Reflection.Assembly.GetEntryAssembly().Location;

           
        }

        private void SetFileList(ListBox targetBox, string strFileType)
        {
             // listBoxDLL
            bool bSuccess = false;
            string[] strDllList = dkSckSvr.GetFileList(strFileType, ref bSuccess); //DLL
            if (!strFileType.Equals("MAP")) targetBox.Items.Clear();
            if (bSuccess)
            {   
                for (int i = 0; i < strDllList.Length; i++)
                {
                    targetBox.Items.Add(strDllList[i]);
                }
            }

        }

        private void RecvMessageGateWay(int iPort, string strMsg)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    if (listboxLog.Items.Count > 150)
                    {
                        listboxLog.Items.Clear();
                    }

                    listboxLog.Items.Add(strMsg);
                    listboxLog.SelectedIndex = listboxLog.Items.Count - 1;

                }));
            }
            catch { }
        }

        private void RecvFileListGateWay(int iPort, int iType, string[] strMsg)
        {
            Button   tmpBtn  = new Button();            
            Label    tmpLbl = new Label();
            TreeView tmpTree = new TreeView();

            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    

                    switch (iType)
                    {
                        case (int)SOCKETCODE.C03: tmpBtn = btnSelectJob;                                                  
                                                  tmpLbl = lblJobListCount; break;
                            
                        case (int)SOCKETCODE.C04: tmpBtn  = btnDownloadLogFile;
                                                  tmpLbl  = lblLogListCount;
                                                  tmpTree = treeViewLog; break;

                        case (int)SOCKETCODE.C05: tmpBtn  = btnDownloadResultFile;
                                                  tmpLbl  = lblResultListCount;
                                                  tmpTree = treeViewResult; break;

                        case (int)SOCKETCODE.C06: tmpBtn  = btnDownloadScreenFile;
                                                  tmpLbl  = lblScreenListCount;
                                                  tmpTree = treeViewScreen; break;
                        default: break;
                    }

                    switch (iType)
                    {
                        case (int)SOCKETCODE.C03:
                                                listBoxClientJOB.Items.Clear();
                                                listBoxClientJOBdown.Items.Clear();
                                                if (strMsg.Length == 0)
                                                {
                                                    tmpBtn.Enabled = false;
                                                    btnDeleteJob.Enabled = false;
                                                    return;
                                                }
                                                tmpLbl.Text = "COUNT : " + strMsg.Length;
                                                for (int i = 0; i < strMsg.Length; i++)
                                                {
                                                    listBoxClientJOB.Items.Add(strMsg[i]);
                                                    listBoxClientJOBdown.Items.Add(strMsg[i]);
                                                }
                                                break;

                        case (int)SOCKETCODE.C04:
                        case (int)SOCKETCODE.C05:
                        case (int)SOCKETCODE.C06:
                                                if (strMsg.Length == 0)
                                                {
                                                    tmpBtn.Enabled = false;
                                                    return;
                                                }
                                                tmpLbl.Text = "COUNT : " + strMsg.Length;
                                                tmpTree.Nodes.Clear();
                                                for (int i = 0; i < strMsg.Length; i++)
                                                {
                                                    tmpTree.Nodes.Add(strMsg[i]);
                                                }                                                
                                                break;

                        default: break;
                    }
                                    

                }));
            }
            catch { }

        }

        private void RecvTransfferGateWay(int iPort, long dVal, string strMessage)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                switch(iPort)
                {
                    case 1:
                            progressBar1.Value = (int)dVal;
                            lblTransfferSatus.Text = strMessage + " (Bytes)";
                            break;
                    case 2:
                            
                            if(strMessage.Contains("LOG"))
                            {
                                if (strMessage.Contains("(NONE)"))
                                {
                                    lblLgTransfferSatus.Text = "Cancel.File Empty in Folder";
                                    progressBarLog.Value = 0;
                                }
                                else
                                {
                                    lblLgTransfferSatus.Text = strMessage + " Bytes )";
                                    progressBarLog.Value = (int)dVal;
                                }
                                
                            }
                            else if (strMessage.Contains("RESULT"))
                            {
                                if (strMessage.Contains("(NONE)"))
                                {
                                    lblResultTransfferSatus.Text = "Cancel.File Empty in Folder";
                                    progressBarResult.Value = 0;
                                }
                                else
                                {
                                    lblResultTransfferSatus.Text = strMessage + " Bytes )";
                                    progressBarResult.Value = (int)dVal;
                                }
                                
                            }
                            else if (strMessage.Contains("SCREEN"))
                            {
                                if (strMessage.Contains("(NONE)"))
                                {
                                    lblScreenTransfferSatus.Text = "Cancel.File Empty in Folder";
                                    progressBarScreen.Value = 0;
                                }
                                else
                                {
                                    lblScreenTransfferSatus.Text = strMessage + " Bytes )";
                                    progressBarScreen.Value = (int)dVal;
                                }
                                
                            }
                            else if (strMessage.Contains("DATA"))
                            {
                                if (strMessage.Contains("(NONE)"))
                                {
                                    lblJobTransfferSatus.Text = "Cancel.File Empty in Folder";
                                    progressBarJob.Value = 0;
                                }
                                else
                                {
                                    lblJobTransfferSatus.Text = strMessage + " Bytes )";
                                    progressBarJob.Value = (int)dVal;
                                }

                            }
                            break;
                             
                }
                

            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CNGridWriter(int iRowNum, MornitorData testdata, bool bClear, bool bMatched)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                try
                {                
                    int iRow = 0;
                   
                    for (int i = 0; i < dataGridNodeList.Rows.Count - 1; i++)
                    {
                        if(iRowNum == (int)dataGridNodeList.Rows[i].Tag)
                        {
                            iRow = i;
                            break;
                        }
                        if(i >= dataGridNodeList.Rows.Count - 1) return;
                    }
                    
                    if (bClear)
                    {
                        
                        for (int i = 0; i < 5; i++)
                        {                            
                            dataGridNodeList.Rows[iRow].Cells[i].Value = String.Empty;
                            dataGridNodeList.Rows[iRow].Cells[3].Style.BackColor = Color.Beige;
                            dataGridNodeList.Rows[iRow].Cells[3].Style.ForeColor = Color.Black;
                        }
                         
                    }
                    else
                    {
                        dataGridNodeList.Rows[iRow].Cells[0].Value = testdata.strName;
                        dataGridNodeList.Rows[iRow].Cells[1].Value = testdata.strIP;
                        dataGridNodeList.Rows[iRow].Cells[2].Value = testdata.strJobName;
                        dataGridNodeList.Rows[iRow].Cells[3].Value = testdata.strVersion;

                        if (bMatched)
                        {
                            dataGridNodeList.Rows[iRow].Cells[3].Style.BackColor = Color.Beige;
                            dataGridNodeList.Rows[iRow].Cells[3].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dataGridNodeList.Rows[iRow].Cells[3].Style.BackColor = Color.Crimson;
                            dataGridNodeList.Rows[iRow].Cells[3].Style.ForeColor = Color.White;
                        }

                        dataGridNodeList.Rows[iRow].Cells[4].Value = testdata.strStatus;
                    }
                    
                    
                }
                catch { }
            }));

        }

        private void button4_Click(object sender, EventArgs e)
        {
               
            /*
            bool[] bConnection = testDkSvr.GetConnectionNodeList();

            for (int i = 0; i < bConnection.Length; i++)
            {                
                if(bConnection[i])
                    CNGridWriter(i, testDkSvr.GetNodeData(i), false);
                else
                    CNGridWriter(i, testDkSvr.GetNodeData(i), true);
            }             
            */
             
            /*
            // 1. 파일날짜 확인하기
            //FileInfo fi = new FileInfo(@"D:\Work\Project\JLR_FA\JLR_Source\bin\Debug\JLR.exe");
            //MessageBox.Show(fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString());

            // 2. 폴더내 파일리스트 가져오기
            DirectoryInfo di = new DirectoryInfo(@"D:\Work\Project\JLR_FA\JLR_Source\bin\Debug\");

            FileInfo[]      fiList = di.GetFiles();
            DirectoryInfo[] diList = di.GetDirectories();

            string strFileList = String.Empty;

            for (int i = 0; i < fiList.Length; i++)
            {
                strFileList = fiList[i].Name + Environment.NewLine;
                RecvMessageGateWay(0, strFileList);
            }

            for (int j = 0; j < diList.Length; j++)
            {
                fiList = diList[j].GetFiles();

                for (int i = 0; i < fiList.Length; i++)
                {                    
                    strFileList = diList[j].Name + "\\" + fiList[i].Name + Environment.NewLine;
                    RecvMessageGateWay(0, strFileList);
                }

            }
            */
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //dkSckClt.Disconnect(true);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            /*
            MornitorData mdData = new MornitorData();
            mdData.strIP = dkSckClt.GetConnectedMyIp();
            mdData.strJobName = "151202_JLR_FA_POWER.JOB";
            mdData.strName = "GPS01";
            mdData.strStatus = "READY";
            mdData.strVersion = "JAGUAR TELEMATIC TEST VER.1.0(20160102)";

            dkSckClt.SendMorniotrData(mdData);
            */
        }


        private void btnServerOpen_Click(object sender, EventArgs e)
        {
            btnServerOpen.Enabled = false;
            btnServerClose.Enabled = true;
            dkSckSvr.ServerOpen();
            threadInfoTimer.Change(0, 1000);
            //threadTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);            
            dataGridNodeList.ClearSelection();
        }
                   
        private void btnServerClose_Click(object sender, EventArgs e)
        {
            threadInfoTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            threadInfoTimer.Dispose();
            dkSckSvr.ServerClose();
            this.Close();
        }

        private void ControlEnable(bool bEnable)
        {
            btnServerOpen.Enabled = bEnable;
        }

        private void dataGridNodeList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            listBoxClientJOB.Items.Clear();
            listBoxClientJOBdown.Items.Clear();
            if (e.RowIndex < 0) return;

            if (dataGridNodeList.Rows[e.RowIndex].Cells[0].Value == null ||
                    dataGridNodeList.Rows[e.RowIndex].Cells[1].Value == null)
            {
                strTargetName = strTargetIP = "";
                lblTargetExe.Text = "NO TARGET : ";
                BtnEnable(false);
                return;
            }

            if (dataGridNodeList.Rows[e.RowIndex].Cells[0].Value.ToString().Length < 1 ||
                    dataGridNodeList.Rows[e.RowIndex].Cells[1].Value.ToString().Length < 1)
            {
                strTargetName = strTargetIP = "";
                lblTargetExe.Text = "NO TARGET : ";
                BtnEnable(false);
                return;
            }

            strTargetName = dataGridNodeList.Rows[e.RowIndex].Cells[0].Value.ToString();
            strTargetIP   = dataGridNodeList.Rows[e.RowIndex].Cells[1].Value.ToString();

            int iNodeNumber = -1;
            if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
            {
                lblTargetExe.Text = "TARGET : " + strTargetName + "(" + strTargetIP + ")";
                BtnEnable(true);
            }
            
        }

        private void BtnEnable(bool bEnable)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: //EXE                    
                case 1: //DLL                    
                case 2: //TBL                   
                case 3: //JOB
                    btnExeUpdate.Enabled = bEnable;
                    btnExeReboot.Enabled = bEnable;
                    break;
                default:
                    btnExeUpdate.Enabled = false;
                    btnExeReboot.Enabled = false;
                    break;

            }
            btnGetJobFileList.Enabled = bEnable;
            btnGetLogFolderList.Enabled = bEnable;
            btnGetResultFolderList.Enabled = bEnable;
            btnGetScreenFolderList.Enabled = bEnable;

            btnSelectJob.Enabled = false;
            btnDeleteJob.Enabled = false;
            btnDownloadLogFile.Enabled = false;
            btnDownloadResultFile.Enabled = false;
            btnDownloadScreenFile.Enabled = false;

            lblJobListCount.Text = " ";
            lblLogListCount.Text = " ";
            lblResultListCount.Text = " ";
            lblScreenListCount.Text = " ";

            listBoxClientJOB.Items.Clear();
            listBoxClientJOBdown.Items.Clear();
            treeViewLog.Nodes.Clear();
            treeViewResult.Nodes.Clear();
            treeViewScreen.Nodes.Clear();

            lblLgTransfferSatus.Text = String.Empty;
            progressBarLog.Value = 0;
            lblScreenTransfferSatus.Text = String.Empty;
            progressBarScreen.Value = 0;
            lblResultTransfferSatus.Text = String.Empty;
            progressBarResult.Value = 0;
            lblJobTransfferSatus.Text = String.Empty;
            progressBarJob.Value = 0;
        }

        private void btnExeUpdate_Click(object sender, EventArgs e)
        {
            // 0 : EXE  1: DLL  2: TBL  3: JOB
            //MessageBox.Show(tabControl.SelectedIndex.ToString());
            progressBar1.Value = 0;
            string strTargetFileName = String.Empty;
            string strTargetFolder = String.Empty;
            switch (tabControl.SelectedIndex)
            {
                case 0: //EXE
                        strTargetFolder = "ROOT";
                        strTargetFileName = lblTargetName.Text; break;
                case 1: //DLL
                        strTargetFolder = "ROOT";
                        if (listBoxDLL.SelectedIndex > -1)
                            strTargetFileName = listBoxDLL.SelectedItem.ToString();                        
                        break;
                case 2: //TBL
                        strTargetFolder = "SYSTEM";
                        if (listBoxTBL.SelectedIndex > -1)
                            strTargetFileName = listBoxTBL.SelectedItem.ToString();                        
                        break;
                case 3: //JOB
                        strTargetFolder = "DATA";
                        if (listBoxJOB.SelectedIndex > -1)
                            strTargetFileName = listBoxJOB.SelectedItem.ToString();                        
                        break;
                default:                        
                        BtnEnable(false);
                        return;
            }
            if (strTargetFileName.Length < 5) return;
            
            int iNodeNumber = 0;

            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.FileSendProcess(iNodeNumber, strTargetFileName, strTargetFolder);
                }
                else
                {
                    lblTargetExe.Text = "NO TARGET";
                    BtnEnable(false);
                }

            }
            
        }

        private void btnExeReboot_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.ClientRebooting(iNodeNumber);
                }
                else
                {
                    lblTargetExe.Text = "NO TARGET";
                }
            }
        }

        private void btnSelectJob_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (listBoxClientJOB.Items.Count > 0 && listBoxClientJOB.SelectedItem != null && listBoxClientJOB.SelectedItem.ToString().Length > 3)
            {
                if (strTargetName.Length > 1 && strTargetIP.Length > 1)
                {
                    if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                    {
                        dkSckSvr.SelectJobCommand(iNodeNumber, listBoxClientJOB.SelectedItem.ToString());
                    }
                    else
                    {
                        lblTargetExe.Text = "NO TARGET";
                    }
                }
            }
            else
            {
                return;
            }

            
        }

        private void btnGetJobFileList_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;
            btnSelectJob.Enabled = false;
            btnDeleteJob.Enabled = false;
            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.GetJobListCommand(iNodeNumber);
                }
                else
                {
                    lblJobListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    lblTargetExe.Text = "NO TARGET";
                    btnGetJobFileList.Enabled = false;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                }
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bTargetOn = false;
            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                bTargetOn = true;
            }

            lblLgTransfferSatus.Text = String.Empty;
            progressBarLog.Value = 0;
            lblScreenTransfferSatus.Text = String.Empty;
            progressBarScreen.Value = 0;
            lblResultTransfferSatus.Text = String.Empty;
            progressBarResult.Value = 0;
            lblJobTransfferSatus.Text = String.Empty;
            progressBarJob.Value = 0;
            
            switch (tabControl.SelectedIndex)
            {
                case 0: //EXE                    
                case 1: //DLL                    
                case 2: //TBL                   
                case 3: //JOB
                    lblJobListCount.Text = lblLogListCount.Text = lblResultListCount.Text = lblScreenListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    treeViewLog.Nodes.Clear();
                    treeViewResult.Nodes.Clear();
                    treeViewScreen.Nodes.Clear();
                    btnGetJobFileList.Enabled = false;
                    btnGetLogFolderList.Enabled = false;
                    btnGetResultFolderList.Enabled = false;
                    btnGetScreenFolderList.Enabled = false;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                    btnDownloadLogFile.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;

                    btnExeUpdate.Enabled = bTargetOn;
                    btnExeReboot.Enabled = bTargetOn;
                    
                    break;
                case 4: //JOB SELECT
                    lblJobListCount.Text = lblLogListCount.Text = lblResultListCount.Text = lblScreenListCount.Text = " ";
                    treeViewLog.Nodes.Clear();
                    treeViewResult.Nodes.Clear();
                    treeViewScreen.Nodes.Clear();
                    btnGetJobFileList.Enabled = bTargetOn;
                    btnGetLogFolderList.Enabled = false;
                    btnGetResultFolderList.Enabled = false;
                    btnGetScreenFolderList.Enabled = false;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                    btnDownloadLogFile.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;

                    btnExeUpdate.Enabled = false;
                    btnExeReboot.Enabled = false;                   
                    break;
                case 5: //LOG    DOWNLAOD
                    lblJobListCount.Text = lblLogListCount.Text = lblResultListCount.Text = lblScreenListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    treeViewLog.Nodes.Clear();
                    treeViewResult.Nodes.Clear();
                    treeViewScreen.Nodes.Clear();
                    btnGetJobFileList.Enabled = false;
                    btnGetLogFolderList.Enabled = bTargetOn;
                    btnGetResultFolderList.Enabled = false;
                    btnGetScreenFolderList.Enabled = false;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                    btnDownloadLogFile.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;

                    btnExeUpdate.Enabled = false;
                    btnExeReboot.Enabled = false;
                    break;
                case 6: //RESULT DOWNLAOD
                    lblJobListCount.Text = lblLogListCount.Text = lblResultListCount.Text = lblScreenListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    treeViewLog.Nodes.Clear();
                    treeViewResult.Nodes.Clear();
                    treeViewScreen.Nodes.Clear();
                    btnGetJobFileList.Enabled = false;
                    btnGetLogFolderList.Enabled = false;
                    btnGetResultFolderList.Enabled = bTargetOn;
                    btnGetScreenFolderList.Enabled = false;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                    btnDownloadLogFile.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;

                    btnExeUpdate.Enabled = false;
                    btnExeReboot.Enabled = false;
                    break;
                case 7: //SCREEN DOWNLAOD
                    lblJobListCount.Text = lblLogListCount.Text = lblResultListCount.Text = lblScreenListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    treeViewLog.Nodes.Clear();
                    treeViewResult.Nodes.Clear();
                    treeViewScreen.Nodes.Clear();
                    btnGetJobFileList.Enabled = false;
                    btnGetLogFolderList.Enabled = false;
                    btnGetResultFolderList.Enabled = false;
                    btnGetScreenFolderList.Enabled = bTargetOn;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                    btnDownloadLogFile.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;

                    btnExeUpdate.Enabled = false;
                    btnExeReboot.Enabled = false;
                    break;
                case 8: //JOB DOWNLAOD
                    lblJobListCount.Text = lblLogListCount.Text = lblResultListCount.Text = lblScreenListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    treeViewLog.Nodes.Clear();
                    treeViewResult.Nodes.Clear();
                    treeViewScreen.Nodes.Clear();
                    btnGetJobFileList.Enabled = false;
                    btnGetLogFolderList.Enabled = false;
                    btnGetResultFolderList.Enabled = false;
                    btnGetScreenFolderList.Enabled = bTargetOn;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                    btnDownloadLogFile.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;

                    btnExeUpdate.Enabled = false;
                    btnExeReboot.Enabled = false;
                    break;
                default:
                    return;
            }
        }

        private void listBoxClientJOB_Click(object sender, EventArgs e)
        {
            if (listBoxClientJOB.Items.Count > 0 && listBoxClientJOB.SelectedItem != null && listBoxClientJOB.SelectedItem.ToString().Length > 3)
            {
                btnSelectJob.Enabled = true;
                btnDeleteJob.Enabled = true;
            }
        }

        private void btnGetLogFolderList_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;
            btnDownloadLogFile.Enabled = false;
            lblLgTransfferSatus.Text = String.Empty;
            progressBarLog.Value = 0;

            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.GetLogFolderListCommand(iNodeNumber);
                 
                }
                else
                {
                    lblLogListCount.Text = " ";
                    treeViewLog.Nodes.Clear();
                    lblTargetExe.Text = "NO TARGET";
                    btnGetLogFolderList.Enabled = false;
                    btnDownloadLogFile.Enabled = false;

                }
            }
        }

        private void btnGetResultFolderList_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;
            btnDownloadResultFile.Enabled = false;

            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.GetResultFolderListCommand(iNodeNumber);

                }
                else
                {
                    lblResultListCount.Text = " ";
                    treeViewResult.Nodes.Clear();
                    lblTargetExe.Text = "NO TARGET";
                    btnGetResultFolderList.Enabled = false;
                    btnDownloadResultFile.Enabled = false;
                }
            }
        }

        private void btnGetScreenFolderList_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;
            btnDownloadScreenFile.Enabled = false;

            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.GetScreenFolderListCommand(iNodeNumber);

                }
                else
                {
                    lblScreenListCount.Text = " ";
                    treeViewScreen.Nodes.Clear();
                    lblTargetExe.Text = "NO TARGET";
                    btnGetScreenFolderList.Enabled = false;
                    btnDownloadScreenFile.Enabled = false;
                }
            }
        }

        private void treeViewLog_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.LastNode == null)
            {
                btnDownloadLogFile.Enabled = true;
            }
            else
            {
                btnDownloadLogFile.Enabled = false;
            }
        }

        private void treeViewResult_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.LastNode == null)
            {
                btnDownloadResultFile.Enabled = true;
            }
            else
            {
                btnDownloadResultFile.Enabled = false;
            }
        }

        private void treeViewScreen_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.LastNode == null)
            {
                btnDownloadScreenFile.Enabled = true;
            }
            else
            {
                btnDownloadScreenFile.Enabled = false;
            }
        }

        private void btnDownloadLogFile_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (treeViewLog.Nodes.Count > 0 && !String.IsNullOrEmpty(treeViewLog.SelectedNode.Text) && treeViewLog.SelectedNode.Text.ToString().Length > 3)
            {
                if (strTargetName.Length > 1 && strTargetIP.Length > 1)
                {
                    if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                    {
                        dkSckSvr.DownloadFilesCommand(iNodeNumber, @"LOG\SET\" + treeViewLog.SelectedNode.Text);
                    }
                    else
                    {
                        lblTargetExe.Text = "NO TARGET";
                    }
                }
            }
            else
            {
                return;
            }
        }
        
        private void btnDownloadResultFile_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (treeViewResult.Nodes.Count > 0 && !String.IsNullOrEmpty(treeViewResult.SelectedNode.Text) && treeViewResult.SelectedNode.Text.ToString().Length > 3)
            {
                if (strTargetName.Length > 1 && strTargetIP.Length > 1)
                {
                    if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                    {
                        dkSckSvr.DownloadFilesCommand(iNodeNumber, @"RESULT\" + treeViewResult.SelectedNode.Text);
                    }
                    else
                    {
                        lblTargetExe.Text = "NO TARGET";
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void btnDownloadScreenFile_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (treeViewScreen.Nodes.Count > 0 && !String.IsNullOrEmpty(treeViewScreen.SelectedNode.Text) && treeViewScreen.SelectedNode.Text.ToString().Length > 3)
            {
                if (strTargetName.Length > 1 && strTargetIP.Length > 1)
                {
                    if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                    {
                        dkSckSvr.DownloadFilesCommand(iNodeNumber, @"SCREEN\" + treeViewScreen.SelectedNode.Text);
                    }
                    else
                    {
                        lblTargetExe.Text = "NO TARGET";
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void dataGridNodeList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridNodeList.Sort(dataGridNodeList.Columns[0], ListSortDirection.Descending);
        }

        private void btnAllReBoot_Click(object sender, EventArgs e)
        {
            
            if (MessageBox.Show("DO YOU WANT TO ALL PC PROGRAM RESTART ?",
                                "CONFIRM MESSAGE", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Threading.Thread[] ExecuThread;
                ExecuThread = new System.Threading.Thread[dataGridNodeList.Rows.Count];
                for (int i = 0; i < dataGridNodeList.Rows.Count; i++)
                {
                    
                    string tmpTargetName;
                    string tmpIpAddress;

                    if(dataGridNodeList.Rows[i].Cells[0].Value != null &&
                        dataGridNodeList.Rows[i].Cells[0].Value.ToString().Length > 3 &&
                         dataGridNodeList.Rows[i].Cells[1].Value != null &&
                          dataGridNodeList.Rows[i].Cells[1].Value.ToString().Length > 3)
                    {
                        tmpTargetName = dataGridNodeList.Rows[i].Cells[0].Value.ToString();
                        tmpIpAddress = dataGridNodeList.Rows[i].Cells[1].Value.ToString();

                        int itmpRefNodeNumber = 0;
                        if (dkSckSvr.GetNodeNumber(ref itmpRefNodeNumber, tmpTargetName, tmpIpAddress))
                        {
                            ExecuThread[i] = new System.Threading.Thread(delegate()
                            {
                                MultiThreadFunc(itmpRefNodeNumber);
                            });
                            ExecuThread[i].Start();
                        }                        
                    }     
                }

            }

            return;
        }

        private void MultiThreadFunc(int iNodeNumber)
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(iNodeNumber * 100);   
            dkSckSvr.ClientRebooting(iNodeNumber);
        }


        private void btnGetJobList_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;
            btnSelectJob.Enabled = false;
            btnDeleteJob.Enabled = false;

            if (strTargetName.Length > 1 && strTargetIP.Length > 1)
            {
                if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                {
                    dkSckSvr.GetJobListCommand(iNodeNumber);
                }
                else
                {
                    lblJobListCount.Text = " ";
                    listBoxClientJOB.Items.Clear();
                    listBoxClientJOBdown.Items.Clear();
                    lblTargetExe.Text = "NO TARGET";
                    btnGetJobFileList.Enabled = false;
                    btnSelectJob.Enabled = false;
                    btnDeleteJob.Enabled = false;
                }
            }
        }

        private void listBoxClientJOBdown_Click(object sender, EventArgs e)
        {
            if (listBoxClientJOBdown.Items.Count > 0 && listBoxClientJOBdown.SelectedItem != null && listBoxClientJOBdown.SelectedItem.ToString().Length > 3)
            {
                btnDownloadJobFile.Enabled = true;
            }
        }

        private void btnDownloadJobFile_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (listBoxClientJOBdown.Items.Count > 0 && listBoxClientJOBdown.SelectedItem != null && listBoxClientJOBdown.SelectedItem.ToString().Length > 3)
            {
                if (strTargetName.Length > 1 && strTargetIP.Length > 1)
                {
                    if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                    {
                        //dkSckSvr.SelectJobCommand(iNodeNumber, listBoxClientJOBdown.SelectedItem.ToString());

                        dkSckSvr.DownloadFilesCommand(iNodeNumber, @"DATA\");

                    }
                    else
                    {
                        lblTargetExe.Text = "NO TARGET";
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void btnDeleteJob_Click(object sender, EventArgs e)
        {
            int iNodeNumber = 0;

            if (listBoxClientJOB.Items.Count > 0 && listBoxClientJOB.SelectedItem != null && listBoxClientJOB.SelectedItem.ToString().Length > 3)
            {
                if (strTargetName.Length > 1 && strTargetIP.Length > 1)
                {
                    if (dkSckSvr.GetNodeNumber(ref iNodeNumber, strTargetName, strTargetIP))
                    {
                        dkSckSvr.DeleteJobCommand(iNodeNumber, listBoxClientJOB.SelectedItem.ToString());                        
                    }
                    else
                    {
                        lblTargetExe.Text = "NO TARGET";
                    }
                }
            }
            else
            {
                return;
            }
        }
   
    }
}

