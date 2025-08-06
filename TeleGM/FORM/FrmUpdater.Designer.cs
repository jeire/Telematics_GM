namespace GmTelematics
{
    partial class FrmUpdater
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnServerOpen = new System.Windows.Forms.Button();
            this.btnServerClose = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAllReBoot = new System.Windows.Forms.Button();
            this.lblTargetExe = new System.Windows.Forms.Label();
            this.btnExeReboot = new System.Windows.Forms.Button();
            this.lblTransfferSatus = new System.Windows.Forms.Label();
            this.btnExeUpdate = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblTargetName = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxDLL = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listBoxTBL = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listBoxJOB = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.lblJobListCount = new System.Windows.Forms.Label();
            this.btnSelectJob = new System.Windows.Forms.Button();
            this.btnGetJobFileList = new System.Windows.Forms.Button();
            this.listBoxClientJOB = new System.Windows.Forms.ListBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.progressBarLog = new System.Windows.Forms.ProgressBar();
            this.lblLgTransfferSatus = new System.Windows.Forms.Label();
            this.lblLogListCount = new System.Windows.Forms.Label();
            this.btnDownloadLogFile = new System.Windows.Forms.Button();
            this.btnGetLogFolderList = new System.Windows.Forms.Button();
            this.treeViewLog = new System.Windows.Forms.TreeView();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.progressBarResult = new System.Windows.Forms.ProgressBar();
            this.lblResultTransfferSatus = new System.Windows.Forms.Label();
            this.btnDownloadResultFile = new System.Windows.Forms.Button();
            this.btnGetResultFolderList = new System.Windows.Forms.Button();
            this.lblResultListCount = new System.Windows.Forms.Label();
            this.treeViewResult = new System.Windows.Forms.TreeView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.progressBarScreen = new System.Windows.Forms.ProgressBar();
            this.lblScreenTransfferSatus = new System.Windows.Forms.Label();
            this.lblScreenListCount = new System.Windows.Forms.Label();
            this.btnDownloadScreenFile = new System.Windows.Forms.Button();
            this.btnGetScreenFolderList = new System.Windows.Forms.Button();
            this.treeViewScreen = new System.Windows.Forms.TreeView();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.listBoxClientJOBdown = new System.Windows.Forms.ListBox();
            this.progressBarJob = new System.Windows.Forms.ProgressBar();
            this.lblJobTransfferSatus = new System.Windows.Forms.Label();
            this.lblJobDownListCount = new System.Windows.Forms.Label();
            this.btnDownloadJobFile = new System.Windows.Forms.Button();
            this.btnGetJobList = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listboxLog = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVerMatchCount = new System.Windows.Forms.Label();
            this.lblVerMissMatchCount = new System.Windows.Forms.Label();
            this.lblConnCount = new System.Windows.Forms.Label();
            this.lblTitle1 = new System.Windows.Forms.Label();
            this.lblTitle2 = new System.Windows.Forms.Label();
            this.lblTitle3 = new System.Windows.Forms.Label();
            this.dataGridNodeList = new System.Windows.Forms.DataGridView();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.btnDeleteJob = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNodeList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnServerOpen
            // 
            this.btnServerOpen.ForeColor = System.Drawing.Color.Black;
            this.btnServerOpen.Location = new System.Drawing.Point(918, 874);
            this.btnServerOpen.Name = "btnServerOpen";
            this.btnServerOpen.Size = new System.Drawing.Size(165, 31);
            this.btnServerOpen.TabIndex = 1;
            this.btnServerOpen.Text = "Server Open";
            this.btnServerOpen.UseVisualStyleBackColor = true;
            this.btnServerOpen.Click += new System.EventHandler(this.btnServerOpen_Click);
            // 
            // btnServerClose
            // 
            this.btnServerClose.Enabled = false;
            this.btnServerClose.ForeColor = System.Drawing.Color.Black;
            this.btnServerClose.Location = new System.Drawing.Point(1163, 874);
            this.btnServerClose.Name = "btnServerClose";
            this.btnServerClose.Size = new System.Drawing.Size(165, 31);
            this.btnServerClose.TabIndex = 2;
            this.btnServerClose.Text = "Server Close";
            this.btnServerClose.UseVisualStyleBackColor = true;
            this.btnServerClose.Click += new System.EventHandler(this.btnServerClose_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1093, 873);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(60, 20);
            this.button4.TabIndex = 4;
            this.button4.Text = "File Info";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAllReBoot);
            this.groupBox1.Controls.Add(this.lblTargetExe);
            this.groupBox1.Controls.Add(this.btnExeReboot);
            this.groupBox1.Controls.Add(this.lblTransfferSatus);
            this.groupBox1.Controls.Add(this.btnExeUpdate);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.tabControl);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(5, 631);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 274);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // btnAllReBoot
            // 
            this.btnAllReBoot.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllReBoot.ForeColor = System.Drawing.Color.Crimson;
            this.btnAllReBoot.Location = new System.Drawing.Point(778, 241);
            this.btnAllReBoot.Name = "btnAllReBoot";
            this.btnAllReBoot.Size = new System.Drawing.Size(114, 31);
            this.btnAllReBoot.TabIndex = 15;
            this.btnAllReBoot.Text = "ALL REBOOT";
            this.btnAllReBoot.UseVisualStyleBackColor = true;
            this.btnAllReBoot.Click += new System.EventHandler(this.btnAllReBoot_Click);
            // 
            // lblTargetExe
            // 
            this.lblTargetExe.BackColor = System.Drawing.Color.DarkKhaki;
            this.lblTargetExe.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTargetExe.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetExe.ForeColor = System.Drawing.Color.Black;
            this.lblTargetExe.Location = new System.Drawing.Point(7, 16);
            this.lblTargetExe.Name = "lblTargetExe";
            this.lblTargetExe.Size = new System.Drawing.Size(886, 26);
            this.lblTargetExe.TabIndex = 14;
            this.lblTargetExe.Text = "TARGET";
            this.lblTargetExe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExeReboot
            // 
            this.btnExeReboot.Enabled = false;
            this.btnExeReboot.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExeReboot.ForeColor = System.Drawing.Color.Crimson;
            this.btnExeReboot.Location = new System.Drawing.Point(679, 241);
            this.btnExeReboot.Name = "btnExeReboot";
            this.btnExeReboot.Size = new System.Drawing.Size(97, 31);
            this.btnExeReboot.TabIndex = 13;
            this.btnExeReboot.Text = "REBOOT";
            this.btnExeReboot.UseVisualStyleBackColor = true;
            this.btnExeReboot.Click += new System.EventHandler(this.btnExeReboot_Click);
            // 
            // lblTransfferSatus
            // 
            this.lblTransfferSatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransfferSatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTransfferSatus.Location = new System.Drawing.Point(10, 237);
            this.lblTransfferSatus.Name = "lblTransfferSatus";
            this.lblTransfferSatus.Size = new System.Drawing.Size(490, 16);
            this.lblTransfferSatus.TabIndex = 12;
            this.lblTransfferSatus.Text = ".";
            this.lblTransfferSatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExeUpdate
            // 
            this.btnExeUpdate.Enabled = false;
            this.btnExeUpdate.ForeColor = System.Drawing.Color.Black;
            this.btnExeUpdate.Location = new System.Drawing.Point(509, 241);
            this.btnExeUpdate.Name = "btnExeUpdate";
            this.btnExeUpdate.Size = new System.Drawing.Size(157, 31);
            this.btnExeUpdate.TabIndex = 11;
            this.btnExeUpdate.Text = "UPDATE";
            this.btnExeUpdate.UseVisualStyleBackColor = true;
            this.btnExeUpdate.Click += new System.EventHandler(this.btnExeUpdate_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 256);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(500, 15);
            this.progressBar1.TabIndex = 10;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage5);
            this.tabControl.Controls.Add(this.tabPage6);
            this.tabControl.Controls.Add(this.tabPage7);
            this.tabControl.Controls.Add(this.tabPage8);
            this.tabControl.Controls.Add(this.tabPage9);
            this.tabControl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(6, 44);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(887, 190);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage1.Controls.Add(this.lblTargetName);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(879, 161);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Update EXE";
            // 
            // lblTargetName
            // 
            this.lblTargetName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTargetName.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTargetName.Location = new System.Drawing.Point(10, 32);
            this.lblTargetName.Name = "lblTargetName";
            this.lblTargetName.Size = new System.Drawing.Size(810, 94);
            this.lblTargetName.TabIndex = 7;
            this.lblTargetName.Text = ".";
            this.lblTargetName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.LightGray;
            this.tabPage2.Controls.Add(this.listBoxDLL);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(879, 161);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Update DLL";
            // 
            // listBoxDLL
            // 
            this.listBoxDLL.BackColor = System.Drawing.Color.White;
            this.listBoxDLL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxDLL.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxDLL.ForeColor = System.Drawing.Color.Black;
            this.listBoxDLL.FormattingEnabled = true;
            this.listBoxDLL.Location = new System.Drawing.Point(6, 8);
            this.listBoxDLL.Name = "listBoxDLL";
            this.listBoxDLL.Size = new System.Drawing.Size(805, 145);
            this.listBoxDLL.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.LightGray;
            this.tabPage3.Controls.Add(this.listBoxTBL);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(879, 161);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Update TBL";
            // 
            // listBoxTBL
            // 
            this.listBoxTBL.BackColor = System.Drawing.Color.White;
            this.listBoxTBL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxTBL.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxTBL.ForeColor = System.Drawing.Color.Black;
            this.listBoxTBL.FormattingEnabled = true;
            this.listBoxTBL.Location = new System.Drawing.Point(6, 8);
            this.listBoxTBL.Name = "listBoxTBL";
            this.listBoxTBL.Size = new System.Drawing.Size(805, 145);
            this.listBoxTBL.TabIndex = 7;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.LightGray;
            this.tabPage4.Controls.Add(this.listBoxJOB);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(879, 161);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Update JOB";
            // 
            // listBoxJOB
            // 
            this.listBoxJOB.BackColor = System.Drawing.Color.White;
            this.listBoxJOB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxJOB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxJOB.ForeColor = System.Drawing.Color.Black;
            this.listBoxJOB.FormattingEnabled = true;
            this.listBoxJOB.Location = new System.Drawing.Point(6, 8);
            this.listBoxJOB.Name = "listBoxJOB";
            this.listBoxJOB.Size = new System.Drawing.Size(805, 145);
            this.listBoxJOB.TabIndex = 8;
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.LightGray;
            this.tabPage5.Controls.Add(this.btnDeleteJob);
            this.tabPage5.Controls.Add(this.lblJobListCount);
            this.tabPage5.Controls.Add(this.btnSelectJob);
            this.tabPage5.Controls.Add(this.btnGetJobFileList);
            this.tabPage5.Controls.Add(this.listBoxClientJOB);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(879, 161);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Select JOB";
            // 
            // lblJobListCount
            // 
            this.lblJobListCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblJobListCount.ForeColor = System.Drawing.Color.Black;
            this.lblJobListCount.Location = new System.Drawing.Point(609, 132);
            this.lblJobListCount.Name = "lblJobListCount";
            this.lblJobListCount.Size = new System.Drawing.Size(202, 18);
            this.lblJobListCount.TabIndex = 14;
            this.lblJobListCount.Text = " ";
            this.lblJobListCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSelectJob
            // 
            this.btnSelectJob.ForeColor = System.Drawing.Color.Black;
            this.btnSelectJob.Location = new System.Drawing.Point(609, 45);
            this.btnSelectJob.Name = "btnSelectJob";
            this.btnSelectJob.Size = new System.Drawing.Size(202, 31);
            this.btnSelectJob.TabIndex = 13;
            this.btnSelectJob.Text = "SELECT JOB";
            this.btnSelectJob.UseVisualStyleBackColor = true;
            this.btnSelectJob.Click += new System.EventHandler(this.btnSelectJob_Click);
            // 
            // btnGetJobFileList
            // 
            this.btnGetJobFileList.ForeColor = System.Drawing.Color.Black;
            this.btnGetJobFileList.Location = new System.Drawing.Point(609, 8);
            this.btnGetJobFileList.Name = "btnGetJobFileList";
            this.btnGetJobFileList.Size = new System.Drawing.Size(202, 31);
            this.btnGetJobFileList.TabIndex = 12;
            this.btnGetJobFileList.Text = "GET JOB LIST";
            this.btnGetJobFileList.UseVisualStyleBackColor = true;
            this.btnGetJobFileList.Click += new System.EventHandler(this.btnGetJobFileList_Click);
            // 
            // listBoxClientJOB
            // 
            this.listBoxClientJOB.BackColor = System.Drawing.Color.White;
            this.listBoxClientJOB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxClientJOB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxClientJOB.ForeColor = System.Drawing.Color.Black;
            this.listBoxClientJOB.FormattingEnabled = true;
            this.listBoxClientJOB.Location = new System.Drawing.Point(6, 8);
            this.listBoxClientJOB.Name = "listBoxClientJOB";
            this.listBoxClientJOB.Size = new System.Drawing.Size(597, 145);
            this.listBoxClientJOB.TabIndex = 9;
            this.listBoxClientJOB.Click += new System.EventHandler(this.listBoxClientJOB_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.Color.LightGray;
            this.tabPage6.Controls.Add(this.progressBarLog);
            this.tabPage6.Controls.Add(this.lblLgTransfferSatus);
            this.tabPage6.Controls.Add(this.lblLogListCount);
            this.tabPage6.Controls.Add(this.btnDownloadLogFile);
            this.tabPage6.Controls.Add(this.btnGetLogFolderList);
            this.tabPage6.Controls.Add(this.treeViewLog);
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(879, 161);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Download Log";
            // 
            // progressBarLog
            // 
            this.progressBarLog.Location = new System.Drawing.Point(379, 113);
            this.progressBarLog.Name = "progressBarLog";
            this.progressBarLog.Size = new System.Drawing.Size(429, 15);
            this.progressBarLog.TabIndex = 19;
            // 
            // lblLgTransfferSatus
            // 
            this.lblLgTransfferSatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLgTransfferSatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblLgTransfferSatus.Location = new System.Drawing.Point(379, 93);
            this.lblLgTransfferSatus.Name = "lblLgTransfferSatus";
            this.lblLgTransfferSatus.Size = new System.Drawing.Size(429, 16);
            this.lblLgTransfferSatus.TabIndex = 18;
            this.lblLgTransfferSatus.Text = ".";
            this.lblLgTransfferSatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLogListCount
            // 
            this.lblLogListCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLogListCount.ForeColor = System.Drawing.Color.Black;
            this.lblLogListCount.Location = new System.Drawing.Point(379, 139);
            this.lblLogListCount.Name = "lblLogListCount";
            this.lblLogListCount.Size = new System.Drawing.Size(429, 18);
            this.lblLogListCount.TabIndex = 17;
            this.lblLogListCount.Text = " ";
            this.lblLogListCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDownloadLogFile
            // 
            this.btnDownloadLogFile.ForeColor = System.Drawing.Color.Black;
            this.btnDownloadLogFile.Location = new System.Drawing.Point(379, 52);
            this.btnDownloadLogFile.Name = "btnDownloadLogFile";
            this.btnDownloadLogFile.Size = new System.Drawing.Size(429, 31);
            this.btnDownloadLogFile.TabIndex = 16;
            this.btnDownloadLogFile.Text = "DOWNLOAD FILE";
            this.btnDownloadLogFile.UseVisualStyleBackColor = true;
            this.btnDownloadLogFile.Click += new System.EventHandler(this.btnDownloadLogFile_Click);
            // 
            // btnGetLogFolderList
            // 
            this.btnGetLogFolderList.ForeColor = System.Drawing.Color.Black;
            this.btnGetLogFolderList.Location = new System.Drawing.Point(379, 15);
            this.btnGetLogFolderList.Name = "btnGetLogFolderList";
            this.btnGetLogFolderList.Size = new System.Drawing.Size(429, 31);
            this.btnGetLogFolderList.TabIndex = 15;
            this.btnGetLogFolderList.Text = "GET FOLDER LIST";
            this.btnGetLogFolderList.UseVisualStyleBackColor = true;
            this.btnGetLogFolderList.Click += new System.EventHandler(this.btnGetLogFolderList_Click);
            // 
            // treeViewLog
            // 
            this.treeViewLog.Location = new System.Drawing.Point(3, 3);
            this.treeViewLog.Name = "treeViewLog";
            this.treeViewLog.Size = new System.Drawing.Size(370, 155);
            this.treeViewLog.TabIndex = 0;
            this.treeViewLog.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewLog_NodeMouseClick);
            // 
            // tabPage7
            // 
            this.tabPage7.BackColor = System.Drawing.Color.LightGray;
            this.tabPage7.Controls.Add(this.progressBarResult);
            this.tabPage7.Controls.Add(this.lblResultTransfferSatus);
            this.tabPage7.Controls.Add(this.btnDownloadResultFile);
            this.tabPage7.Controls.Add(this.btnGetResultFolderList);
            this.tabPage7.Controls.Add(this.lblResultListCount);
            this.tabPage7.Controls.Add(this.treeViewResult);
            this.tabPage7.Location = new System.Drawing.Point(4, 25);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(879, 161);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Download Result";
            // 
            // progressBarResult
            // 
            this.progressBarResult.Location = new System.Drawing.Point(379, 113);
            this.progressBarResult.Name = "progressBarResult";
            this.progressBarResult.Size = new System.Drawing.Size(429, 15);
            this.progressBarResult.TabIndex = 21;
            // 
            // lblResultTransfferSatus
            // 
            this.lblResultTransfferSatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultTransfferSatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblResultTransfferSatus.Location = new System.Drawing.Point(379, 93);
            this.lblResultTransfferSatus.Name = "lblResultTransfferSatus";
            this.lblResultTransfferSatus.Size = new System.Drawing.Size(429, 16);
            this.lblResultTransfferSatus.TabIndex = 20;
            this.lblResultTransfferSatus.Text = ".";
            this.lblResultTransfferSatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDownloadResultFile
            // 
            this.btnDownloadResultFile.ForeColor = System.Drawing.Color.Black;
            this.btnDownloadResultFile.Location = new System.Drawing.Point(379, 52);
            this.btnDownloadResultFile.Name = "btnDownloadResultFile";
            this.btnDownloadResultFile.Size = new System.Drawing.Size(429, 31);
            this.btnDownloadResultFile.TabIndex = 19;
            this.btnDownloadResultFile.Text = "DOWNLOAD FILE";
            this.btnDownloadResultFile.UseVisualStyleBackColor = true;
            this.btnDownloadResultFile.Click += new System.EventHandler(this.btnDownloadResultFile_Click);
            // 
            // btnGetResultFolderList
            // 
            this.btnGetResultFolderList.ForeColor = System.Drawing.Color.Black;
            this.btnGetResultFolderList.Location = new System.Drawing.Point(379, 15);
            this.btnGetResultFolderList.Name = "btnGetResultFolderList";
            this.btnGetResultFolderList.Size = new System.Drawing.Size(429, 31);
            this.btnGetResultFolderList.TabIndex = 18;
            this.btnGetResultFolderList.Text = "GET FOLDER LIST";
            this.btnGetResultFolderList.UseVisualStyleBackColor = true;
            this.btnGetResultFolderList.Click += new System.EventHandler(this.btnGetResultFolderList_Click);
            // 
            // lblResultListCount
            // 
            this.lblResultListCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblResultListCount.ForeColor = System.Drawing.Color.Black;
            this.lblResultListCount.Location = new System.Drawing.Point(379, 139);
            this.lblResultListCount.Name = "lblResultListCount";
            this.lblResultListCount.Size = new System.Drawing.Size(429, 18);
            this.lblResultListCount.TabIndex = 17;
            this.lblResultListCount.Text = " ";
            this.lblResultListCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // treeViewResult
            // 
            this.treeViewResult.Location = new System.Drawing.Point(3, 3);
            this.treeViewResult.Name = "treeViewResult";
            this.treeViewResult.Size = new System.Drawing.Size(370, 155);
            this.treeViewResult.TabIndex = 1;
            this.treeViewResult.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewResult_NodeMouseClick);
            // 
            // tabPage8
            // 
            this.tabPage8.BackColor = System.Drawing.Color.LightGray;
            this.tabPage8.Controls.Add(this.progressBarScreen);
            this.tabPage8.Controls.Add(this.lblScreenTransfferSatus);
            this.tabPage8.Controls.Add(this.lblScreenListCount);
            this.tabPage8.Controls.Add(this.btnDownloadScreenFile);
            this.tabPage8.Controls.Add(this.btnGetScreenFolderList);
            this.tabPage8.Controls.Add(this.treeViewScreen);
            this.tabPage8.Location = new System.Drawing.Point(4, 25);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(879, 161);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Text = "Download Screen";
            // 
            // progressBarScreen
            // 
            this.progressBarScreen.Location = new System.Drawing.Point(379, 113);
            this.progressBarScreen.Name = "progressBarScreen";
            this.progressBarScreen.Size = new System.Drawing.Size(429, 15);
            this.progressBarScreen.TabIndex = 21;
            // 
            // lblScreenTransfferSatus
            // 
            this.lblScreenTransfferSatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScreenTransfferSatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblScreenTransfferSatus.Location = new System.Drawing.Point(379, 93);
            this.lblScreenTransfferSatus.Name = "lblScreenTransfferSatus";
            this.lblScreenTransfferSatus.Size = new System.Drawing.Size(429, 16);
            this.lblScreenTransfferSatus.TabIndex = 20;
            this.lblScreenTransfferSatus.Text = ".";
            this.lblScreenTransfferSatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblScreenListCount
            // 
            this.lblScreenListCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblScreenListCount.ForeColor = System.Drawing.Color.Black;
            this.lblScreenListCount.Location = new System.Drawing.Point(379, 139);
            this.lblScreenListCount.Name = "lblScreenListCount";
            this.lblScreenListCount.Size = new System.Drawing.Size(429, 18);
            this.lblScreenListCount.TabIndex = 17;
            this.lblScreenListCount.Text = " ";
            this.lblScreenListCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDownloadScreenFile
            // 
            this.btnDownloadScreenFile.ForeColor = System.Drawing.Color.Black;
            this.btnDownloadScreenFile.Location = new System.Drawing.Point(379, 52);
            this.btnDownloadScreenFile.Name = "btnDownloadScreenFile";
            this.btnDownloadScreenFile.Size = new System.Drawing.Size(429, 31);
            this.btnDownloadScreenFile.TabIndex = 16;
            this.btnDownloadScreenFile.Text = "DOWNLOAD FILE";
            this.btnDownloadScreenFile.UseVisualStyleBackColor = true;
            this.btnDownloadScreenFile.Click += new System.EventHandler(this.btnDownloadScreenFile_Click);
            // 
            // btnGetScreenFolderList
            // 
            this.btnGetScreenFolderList.ForeColor = System.Drawing.Color.Black;
            this.btnGetScreenFolderList.Location = new System.Drawing.Point(379, 15);
            this.btnGetScreenFolderList.Name = "btnGetScreenFolderList";
            this.btnGetScreenFolderList.Size = new System.Drawing.Size(429, 31);
            this.btnGetScreenFolderList.TabIndex = 15;
            this.btnGetScreenFolderList.Text = "GET FOLDER LIST";
            this.btnGetScreenFolderList.UseVisualStyleBackColor = true;
            this.btnGetScreenFolderList.Click += new System.EventHandler(this.btnGetScreenFolderList_Click);
            // 
            // treeViewScreen
            // 
            this.treeViewScreen.Location = new System.Drawing.Point(3, 3);
            this.treeViewScreen.Name = "treeViewScreen";
            this.treeViewScreen.Size = new System.Drawing.Size(370, 155);
            this.treeViewScreen.TabIndex = 1;
            this.treeViewScreen.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewScreen_NodeMouseClick);
            // 
            // tabPage9
            // 
            this.tabPage9.BackColor = System.Drawing.Color.LightGray;
            this.tabPage9.Controls.Add(this.listBoxClientJOBdown);
            this.tabPage9.Controls.Add(this.progressBarJob);
            this.tabPage9.Controls.Add(this.lblJobTransfferSatus);
            this.tabPage9.Controls.Add(this.lblJobDownListCount);
            this.tabPage9.Controls.Add(this.btnDownloadJobFile);
            this.tabPage9.Controls.Add(this.btnGetJobList);
            this.tabPage9.Location = new System.Drawing.Point(4, 25);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(879, 161);
            this.tabPage9.TabIndex = 8;
            this.tabPage9.Text = "Download Job";
            // 
            // listBoxClientJOBdown
            // 
            this.listBoxClientJOBdown.BackColor = System.Drawing.Color.White;
            this.listBoxClientJOBdown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxClientJOBdown.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxClientJOBdown.ForeColor = System.Drawing.Color.Black;
            this.listBoxClientJOBdown.FormattingEnabled = true;
            this.listBoxClientJOBdown.Location = new System.Drawing.Point(3, 12);
            this.listBoxClientJOBdown.Name = "listBoxClientJOBdown";
            this.listBoxClientJOBdown.Size = new System.Drawing.Size(435, 145);
            this.listBoxClientJOBdown.TabIndex = 28;
            this.listBoxClientJOBdown.Click += new System.EventHandler(this.listBoxClientJOBdown_Click);
            // 
            // progressBarJob
            // 
            this.progressBarJob.Location = new System.Drawing.Point(444, 113);
            this.progressBarJob.Name = "progressBarJob";
            this.progressBarJob.Size = new System.Drawing.Size(429, 15);
            this.progressBarJob.TabIndex = 27;
            // 
            // lblJobTransfferSatus
            // 
            this.lblJobTransfferSatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJobTransfferSatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblJobTransfferSatus.Location = new System.Drawing.Point(444, 93);
            this.lblJobTransfferSatus.Name = "lblJobTransfferSatus";
            this.lblJobTransfferSatus.Size = new System.Drawing.Size(429, 16);
            this.lblJobTransfferSatus.TabIndex = 26;
            this.lblJobTransfferSatus.Text = ".";
            this.lblJobTransfferSatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblJobDownListCount
            // 
            this.lblJobDownListCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblJobDownListCount.ForeColor = System.Drawing.Color.Black;
            this.lblJobDownListCount.Location = new System.Drawing.Point(444, 139);
            this.lblJobDownListCount.Name = "lblJobDownListCount";
            this.lblJobDownListCount.Size = new System.Drawing.Size(429, 18);
            this.lblJobDownListCount.TabIndex = 25;
            this.lblJobDownListCount.Text = " ";
            this.lblJobDownListCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDownloadJobFile
            // 
            this.btnDownloadJobFile.ForeColor = System.Drawing.Color.Black;
            this.btnDownloadJobFile.Location = new System.Drawing.Point(444, 52);
            this.btnDownloadJobFile.Name = "btnDownloadJobFile";
            this.btnDownloadJobFile.Size = new System.Drawing.Size(429, 31);
            this.btnDownloadJobFile.TabIndex = 24;
            this.btnDownloadJobFile.Text = "DOWNLOAD";
            this.btnDownloadJobFile.UseVisualStyleBackColor = true;
            this.btnDownloadJobFile.Click += new System.EventHandler(this.btnDownloadJobFile_Click);
            // 
            // btnGetJobList
            // 
            this.btnGetJobList.ForeColor = System.Drawing.Color.Black;
            this.btnGetJobList.Location = new System.Drawing.Point(444, 15);
            this.btnGetJobList.Name = "btnGetJobList";
            this.btnGetJobList.Size = new System.Drawing.Size(429, 31);
            this.btnGetJobList.TabIndex = 23;
            this.btnGetJobList.Text = "GET JOB LIST";
            this.btnGetJobList.UseVisualStyleBackColor = true;
            this.btnGetJobList.Click += new System.EventHandler(this.btnGetJobList_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listboxLog);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(910, 631);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(423, 234);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Networks Status";
            // 
            // listboxLog
            // 
            this.listboxLog.BackColor = System.Drawing.Color.Black;
            this.listboxLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listboxLog.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxLog.ForeColor = System.Drawing.Color.LimeGreen;
            this.listboxLog.FormattingEnabled = true;
            this.listboxLog.Location = new System.Drawing.Point(8, 18);
            this.listboxLog.Name = "listboxLog";
            this.listboxLog.Size = new System.Drawing.Size(410, 210);
            this.listboxLog.TabIndex = 5;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblVerMatchCount);
            this.groupBox3.Controls.Add(this.lblVerMissMatchCount);
            this.groupBox3.Controls.Add(this.lblConnCount);
            this.groupBox3.Controls.Add(this.lblTitle1);
            this.groupBox3.Controls.Add(this.lblTitle2);
            this.groupBox3.Controls.Add(this.lblTitle3);
            this.groupBox3.Controls.Add(this.dataGridNodeList);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(5, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1328, 628);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Informations";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(1150, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 26);
            this.label1.TabIndex = 21;
            this.label1.Text = " ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVerMatchCount
            // 
            this.lblVerMatchCount.BackColor = System.Drawing.Color.LightGreen;
            this.lblVerMatchCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVerMatchCount.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVerMatchCount.ForeColor = System.Drawing.Color.Black;
            this.lblVerMatchCount.Location = new System.Drawing.Point(1040, 20);
            this.lblVerMatchCount.Name = "lblVerMatchCount";
            this.lblVerMatchCount.Size = new System.Drawing.Size(104, 26);
            this.lblVerMatchCount.TabIndex = 20;
            this.lblVerMatchCount.Text = "0";
            this.lblVerMatchCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVerMissMatchCount
            // 
            this.lblVerMissMatchCount.BackColor = System.Drawing.Color.Gold;
            this.lblVerMissMatchCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVerMissMatchCount.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVerMissMatchCount.ForeColor = System.Drawing.Color.Black;
            this.lblVerMissMatchCount.Location = new System.Drawing.Point(661, 20);
            this.lblVerMissMatchCount.Name = "lblVerMissMatchCount";
            this.lblVerMissMatchCount.Size = new System.Drawing.Size(102, 26);
            this.lblVerMissMatchCount.TabIndex = 19;
            this.lblVerMissMatchCount.Text = "0";
            this.lblVerMissMatchCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConnCount
            // 
            this.lblConnCount.BackColor = System.Drawing.Color.LightBlue;
            this.lblConnCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConnCount.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnCount.ForeColor = System.Drawing.Color.Black;
            this.lblConnCount.Location = new System.Drawing.Point(281, 20);
            this.lblConnCount.Name = "lblConnCount";
            this.lblConnCount.Size = new System.Drawing.Size(102, 26);
            this.lblConnCount.TabIndex = 18;
            this.lblConnCount.Text = "0";
            this.lblConnCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle1
            // 
            this.lblTitle1.BackColor = System.Drawing.Color.LightBlue;
            this.lblTitle1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle1.ForeColor = System.Drawing.Color.Black;
            this.lblTitle1.Location = new System.Drawing.Point(6, 20);
            this.lblTitle1.Name = "lblTitle1";
            this.lblTitle1.Size = new System.Drawing.Size(276, 26);
            this.lblTitle1.TabIndex = 17;
            this.lblTitle1.Text = "Client Connection Count";
            this.lblTitle1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle2
            // 
            this.lblTitle2.BackColor = System.Drawing.Color.Gold;
            this.lblTitle2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle2.ForeColor = System.Drawing.Color.Black;
            this.lblTitle2.Location = new System.Drawing.Point(386, 20);
            this.lblTitle2.Name = "lblTitle2";
            this.lblTitle2.Size = new System.Drawing.Size(276, 26);
            this.lblTitle2.TabIndex = 16;
            this.lblTitle2.Text = "Program Version Miss Matched ";
            this.lblTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle3
            // 
            this.lblTitle3.BackColor = System.Drawing.Color.LightGreen;
            this.lblTitle3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle3.ForeColor = System.Drawing.Color.Black;
            this.lblTitle3.Location = new System.Drawing.Point(766, 20);
            this.lblTitle3.Name = "lblTitle3";
            this.lblTitle3.Size = new System.Drawing.Size(276, 26);
            this.lblTitle3.TabIndex = 15;
            this.lblTitle3.Text = "Program Version Matched";
            this.lblTitle3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGridNodeList
            // 
            this.dataGridNodeList.AllowUserToAddRows = false;
            this.dataGridNodeList.AllowUserToDeleteRows = false;
            this.dataGridNodeList.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Beige;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            this.dataGridNodeList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridNodeList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridNodeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.LimeGreen;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.DarkSeaGreen;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridNodeList.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridNodeList.Location = new System.Drawing.Point(6, 48);
            this.dataGridNodeList.MultiSelect = false;
            this.dataGridNodeList.Name = "dataGridNodeList";
            this.dataGridNodeList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridNodeList.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridNodeList.RowHeadersVisible = false;
            this.dataGridNodeList.RowHeadersWidth = 55;
            this.dataGridNodeList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.Beige;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
            this.dataGridNodeList.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridNodeList.RowTemplate.Height = 23;
            this.dataGridNodeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridNodeList.Size = new System.Drawing.Size(1317, 572);
            this.dataGridNodeList.TabIndex = 3;
            this.dataGridNodeList.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridNodeList_CellMouseUp);
            this.dataGridNodeList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridNodeList_ColumnHeaderMouseClick);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1097, 890);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(60, 20);
            this.button6.TabIndex = 7;
            this.button6.Text = "Client Connect";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(1092, 890);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(60, 20);
            this.button7.TabIndex = 8;
            this.button7.Text = "Client Close";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(1097, 879);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(60, 20);
            this.button8.TabIndex = 9;
            this.button8.Text = "Send Mornitor";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Visible = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // btnDeleteJob
            // 
            this.btnDeleteJob.ForeColor = System.Drawing.Color.Black;
            this.btnDeleteJob.Location = new System.Drawing.Point(609, 82);
            this.btnDeleteJob.Name = "btnDeleteJob";
            this.btnDeleteJob.Size = new System.Drawing.Size(202, 31);
            this.btnDeleteJob.TabIndex = 15;
            this.btnDeleteJob.Text = "DELETE JOB";
            this.btnDeleteJob.UseVisualStyleBackColor = true;
            this.btnDeleteJob.Click += new System.EventHandler(this.btnDeleteJob_Click);
            // 
            // FrmUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1340, 908);
            this.ControlBox = false;
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnServerClose);
            this.Controls.Add(this.btnServerOpen);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.LimeGreen;
            this.Name = "FrmUpdater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inspection Program Mornitoring Pannel";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.tabPage9.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNodeList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnServerOpen;
        private System.Windows.Forms.Button btnServerClose;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listboxLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblTargetName;
        private System.Windows.Forms.DataGridView dataGridNodeList;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button btnExeReboot;
        private System.Windows.Forms.Label lblTransfferSatus;
        private System.Windows.Forms.Button btnExeUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblTargetExe;
        private System.Windows.Forms.ListBox listBoxDLL;
        private System.Windows.Forms.ListBox listBoxTBL;
        private System.Windows.Forms.ListBox listBoxJOB;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnSelectJob;
        private System.Windows.Forms.Button btnGetJobFileList;
        private System.Windows.Forms.ListBox listBoxClientJOB;
        private System.Windows.Forms.Label lblJobListCount;
        private System.Windows.Forms.Label lblTitle1;
        private System.Windows.Forms.Label lblTitle2;
        private System.Windows.Forms.Label lblTitle3;
        private System.Windows.Forms.Label lblVerMatchCount;
        private System.Windows.Forms.Label lblVerMissMatchCount;
        private System.Windows.Forms.Label lblConnCount;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TreeView treeViewLog;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Label lblLogListCount;
        private System.Windows.Forms.Button btnDownloadLogFile;
        private System.Windows.Forms.Button btnGetLogFolderList;
        private System.Windows.Forms.Button btnDownloadResultFile;
        private System.Windows.Forms.Button btnGetResultFolderList;
        private System.Windows.Forms.Label lblResultListCount;
        private System.Windows.Forms.TreeView treeViewResult;
        private System.Windows.Forms.Label lblScreenListCount;
        private System.Windows.Forms.Button btnDownloadScreenFile;
        private System.Windows.Forms.Button btnGetScreenFolderList;
        private System.Windows.Forms.TreeView treeViewScreen;
        private System.Windows.Forms.ProgressBar progressBarLog;
        private System.Windows.Forms.Label lblLgTransfferSatus;
        private System.Windows.Forms.ProgressBar progressBarResult;
        private System.Windows.Forms.Label lblResultTransfferSatus;
        private System.Windows.Forms.ProgressBar progressBarScreen;
        private System.Windows.Forms.Label lblScreenTransfferSatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAllReBoot;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.ListBox listBoxClientJOBdown;
        private System.Windows.Forms.ProgressBar progressBarJob;
        private System.Windows.Forms.Label lblJobTransfferSatus;
        private System.Windows.Forms.Label lblJobDownListCount;
        private System.Windows.Forms.Button btnDownloadJobFile;
        private System.Windows.Forms.Button btnGetJobList;
        private System.Windows.Forms.Button btnDeleteJob;
    }
}

