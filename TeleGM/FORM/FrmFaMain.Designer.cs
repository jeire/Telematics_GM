namespace GmTelematics
{
    partial class FrmFaMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFaMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnEdit = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.cbJobFiles = new System.Windows.Forms.ComboBox();
            this.btnDisplay = new System.Windows.Forms.Button();
            this.dataGridConnector = new System.Windows.Forms.DataGridView();
            this.txtBoxBarcode = new System.Windows.Forms.TextBox();
            this.txtBoxWip1 = new System.Windows.Forms.TextBox();
            this.dataGridDevice = new System.Windows.Forms.DataGridView();
            this.txtBoxCN0 = new System.Windows.Forms.TextBox();
            this.txtBoxScount = new System.Windows.Forms.TextBox();
            this.groupStatusBox = new System.Windows.Forms.GroupBox();
            this.axiSensor9 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor8 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor7 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor6 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor5 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor4 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor3 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor2 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor1 = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensor0 = new AxisAnalogLibrary.AxiLabelX();
            this.axiDutDelay = new AxisAnalogLibrary.AxiLabelX();
            this.axiDutClear = new AxisAnalogLibrary.AxiLabelX();
            this.axiDutRecv = new AxisAnalogLibrary.AxiLabelX();
            this.axiDioDelay = new AxisAnalogLibrary.AxiLabelX();
            this.axiDioClear = new AxisAnalogLibrary.AxiLabelX();
            this.axiDioRecv = new AxisAnalogLibrary.AxiLabelX();
            this.btnInteractive = new System.Windows.Forms.Button();
            this.ixlblWannMsg = new System.Windows.Forms.TextBox();
            this.progressInspection = new System.Windows.Forms.ProgressBar();
            this.panelGMES = new System.Windows.Forms.Panel();
            this.lblGmesVersion = new System.Windows.Forms.Label();
            this.chkBoxMesOn = new System.Windows.Forms.CheckBox();
            this.contextMenuMesStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.lblMapOn = new System.Windows.Forms.Label();
            this.ixlblMesLED = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblMesInfo = new AxisAnalogLibrary.AxiLabelX();
            this.panelORACLE = new System.Windows.Forms.Panel();
            this.chkBoxOracleOn = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMapOn2 = new System.Windows.Forms.Label();
            this.ixlblOracleLED = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblOracleInfo = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblNetworkChkMsg = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listboxLog = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listboxBinLog = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listboxResultLog = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.axiSensorRepeat = new AxisAnalogLibrary.AxiLabelX();
            this.axiSensorDebug = new AxisAnalogLibrary.AxiLabelX();
            this.listboxEtcLog = new System.Windows.Forms.ListBox();
            this.txtBoxWip2 = new System.Windows.Forms.TextBox();
            this.axiLabelWip2 = new AxisAnalogLibrary.AxiLabelX();
            this.axiLabelCurrent = new AxisAnalogLibrary.AxiLabelX();
            this.axiCurrent = new AxisAnalogLibrary.AxiSevenSegmentAnalogX();
            this.ChartCurrent = new AxiPlotLibrary.AxiPlotX();
            this.ixlblStatusDate = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblStatusCount = new AxisAnalogLibrary.AxiLabelX();
            this.axilblBackboarder = new AxisAnalogLibrary.AxiLabelX();
            this.axiBuffCounter4 = new AxisAnalogLibrary.AxiLabelX();
            this.axiBuffCounter3 = new AxisAnalogLibrary.AxiLabelX();
            this.axiBuffCounter2 = new AxisAnalogLibrary.AxiLabelX();
            this.axiBuffCounter1 = new AxisAnalogLibrary.AxiLabelX();
            this.axiEngCounter = new AxisAnalogLibrary.AxiLabelX();
            this.axiBuildVer = new AxisAnalogLibrary.AxiLabelX();
            this.isegGpsTimer = new AxisAnalogLibrary.AxiSevenSegmentAnalogX();
            this.axiLabelCN0 = new AxisAnalogLibrary.AxiLabelX();
            this.axiLabelScount = new AxisAnalogLibrary.AxiLabelX();
            this.axiLabelGPSTIME = new AxisAnalogLibrary.AxiLabelX();
            this.axiLabelBarcode = new AxisAnalogLibrary.AxiLabelX();
            this.axiLabelWip1 = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblTotal = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblFail = new AxisAnalogLibrary.AxiLabelX();
            this.LabelTOTAL = new AxisAnalogLibrary.AxiLabelX();
            this.LabelFAIL = new AxisAnalogLibrary.AxiLabelX();
            this.isegTimer = new AxisAnalogLibrary.AxiSevenSegmentAnalogX();
            this.ixlblStatus = new AxisAnalogLibrary.AxiLabelX();
            this.ixlblPass = new AxisAnalogLibrary.AxiLabelX();
            this.LabelPass = new AxisAnalogLibrary.AxiLabelX();
            this.axiCommandStatus = new AxisAnalogLibrary.AxiLabelX();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridConnector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDevice)).BeginInit();
            this.groupStatusBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDutDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDutClear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDutRecv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDioDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDioClear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDioRecv)).BeginInit();
            this.panelGMES.SuspendLayout();
            this.contextMenuMesStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblMesLED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblMesInfo)).BeginInit();
            this.panelORACLE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblOracleLED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblOracleInfo)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensorRepeat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensorDebug)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelWip2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblStatusDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblStatusCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axilblBackboarder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiEngCounter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuildVer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isegGpsTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelCN0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelScount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelGPSTIME)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelWip1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblFail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelTOTAL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelFAIL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isegTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiCommandStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.ForeColor = System.Drawing.Color.Peru;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEdit.Location = new System.Drawing.Point(260, 2);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(128, 50);
            this.btnEdit.TabIndex = 0;
            this.btnEdit.Text = "EDIT   ";
            this.btnEdit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(12, 127);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(812, 106);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.Color.Green;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStart.Location = new System.Drawing.Point(6, 2);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(128, 50);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "START ";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.Crimson;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(133, 2);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(128, 50);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "STOP   ";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfig.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnConfig.Image")));
            this.btnConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfig.Location = new System.Drawing.Point(387, 2);
            this.btnConfig.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(128, 50);
            this.btnConfig.TabIndex = 4;
            this.btnConfig.Text = "CONFIG ";
            this.btnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.BlueViolet;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(1132, 2);
            this.btnExit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(120, 50);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "EXIT   ";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cbJobFiles
            // 
            this.cbJobFiles.BackColor = System.Drawing.Color.PeachPuff;
            this.cbJobFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbJobFiles.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbJobFiles.FormattingEnabled = true;
            this.cbJobFiles.Location = new System.Drawing.Point(836, 98);
            this.cbJobFiles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbJobFiles.Name = "cbJobFiles";
            this.cbJobFiles.Size = new System.Drawing.Size(431, 26);
            this.cbJobFiles.TabIndex = 7;
            this.cbJobFiles.SelectionChangeCommitted += new System.EventHandler(this.cbJobFiles_SelectionChangeCommitted);
            // 
            // btnDisplay
            // 
            this.btnDisplay.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisplay.Location = new System.Drawing.Point(514, 2);
            this.btnDisplay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDisplay.Name = "btnDisplay";
            this.btnDisplay.Size = new System.Drawing.Size(128, 50);
            this.btnDisplay.TabIndex = 23;
            this.btnDisplay.Text = "DISPLAY ALL";
            this.btnDisplay.UseVisualStyleBackColor = true;
            this.btnDisplay.Click += new System.EventHandler(this.btnDisplay_Click);
            // 
            // dataGridConnector
            // 
            this.dataGridConnector.AllowUserToAddRows = false;
            this.dataGridConnector.AllowUserToDeleteRows = false;
            this.dataGridConnector.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridConnector.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridConnector.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridConnector.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridConnector.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridConnector.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridConnector.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridConnector.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridConnector.Enabled = false;
            this.dataGridConnector.Location = new System.Drawing.Point(836, 129);
            this.dataGridConnector.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridConnector.MultiSelect = false;
            this.dataGridConnector.Name = "dataGridConnector";
            this.dataGridConnector.ReadOnly = true;
            this.dataGridConnector.RowHeadersVisible = false;
            this.dataGridConnector.RowTemplate.Height = 23;
            this.dataGridConnector.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridConnector.ShowCellErrors = false;
            this.dataGridConnector.ShowCellToolTips = false;
            this.dataGridConnector.ShowEditingIcon = false;
            this.dataGridConnector.ShowRowErrors = false;
            this.dataGridConnector.Size = new System.Drawing.Size(430, 140);
            this.dataGridConnector.TabIndex = 28;
            // 
            // txtBoxBarcode
            // 
            this.txtBoxBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxBarcode.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBarcode.Location = new System.Drawing.Point(955, 559);
            this.txtBoxBarcode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBoxBarcode.MaxLength = 0;
            this.txtBoxBarcode.Name = "txtBoxBarcode";
            this.txtBoxBarcode.Size = new System.Drawing.Size(312, 26);
            this.txtBoxBarcode.TabIndex = 32;
            this.txtBoxBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBoxBarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxBarcode_KeyPress);
            // 
            // txtBoxWip1
            // 
            this.txtBoxWip1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxWip1.Enabled = false;
            this.txtBoxWip1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxWip1.Location = new System.Drawing.Point(955, 499);
            this.txtBoxWip1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBoxWip1.MaxLength = 25;
            this.txtBoxWip1.Name = "txtBoxWip1";
            this.txtBoxWip1.Size = new System.Drawing.Size(312, 26);
            this.txtBoxWip1.TabIndex = 34;
            this.txtBoxWip1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dataGridDevice
            // 
            this.dataGridDevice.AllowUserToAddRows = false;
            this.dataGridDevice.AllowUserToDeleteRows = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.AntiqueWhite;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.AntiqueWhite;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridDevice.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridDevice.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridDevice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridDevice.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridDevice.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridDevice.Enabled = false;
            this.dataGridDevice.EnableHeadersVisualStyles = false;
            this.dataGridDevice.Location = new System.Drawing.Point(836, 54);
            this.dataGridDevice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridDevice.MultiSelect = false;
            this.dataGridDevice.Name = "dataGridDevice";
            this.dataGridDevice.ReadOnly = true;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridDevice.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridDevice.RowHeadersVisible = false;
            this.dataGridDevice.RowHeadersWidth = 55;
            this.dataGridDevice.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.AntiqueWhite;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.AntiqueWhite;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridDevice.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridDevice.RowTemplate.Height = 23;
            this.dataGridDevice.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridDevice.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridDevice.ShowCellErrors = false;
            this.dataGridDevice.ShowCellToolTips = false;
            this.dataGridDevice.ShowEditingIcon = false;
            this.dataGridDevice.ShowRowErrors = false;
            this.dataGridDevice.Size = new System.Drawing.Size(430, 42);
            this.dataGridDevice.TabIndex = 38;
            // 
            // txtBoxCN0
            // 
            this.txtBoxCN0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxCN0.Enabled = false;
            this.txtBoxCN0.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxCN0.Location = new System.Drawing.Point(1190, 619);
            this.txtBoxCN0.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBoxCN0.MaxLength = 25;
            this.txtBoxCN0.Name = "txtBoxCN0";
            this.txtBoxCN0.Size = new System.Drawing.Size(77, 24);
            this.txtBoxCN0.TabIndex = 48;
            this.txtBoxCN0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBoxCN0.Visible = false;
            // 
            // txtBoxScount
            // 
            this.txtBoxScount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxScount.Enabled = false;
            this.txtBoxScount.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxScount.Location = new System.Drawing.Point(1190, 594);
            this.txtBoxScount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBoxScount.MaxLength = 25;
            this.txtBoxScount.Name = "txtBoxScount";
            this.txtBoxScount.Size = new System.Drawing.Size(77, 26);
            this.txtBoxScount.TabIndex = 49;
            this.txtBoxScount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBoxScount.Visible = false;
            // 
            // groupStatusBox
            // 
            this.groupStatusBox.Controls.Add(this.axiSensor9);
            this.groupStatusBox.Controls.Add(this.axiSensor8);
            this.groupStatusBox.Controls.Add(this.axiSensor7);
            this.groupStatusBox.Controls.Add(this.axiSensor6);
            this.groupStatusBox.Controls.Add(this.axiSensor5);
            this.groupStatusBox.Controls.Add(this.axiSensor4);
            this.groupStatusBox.Controls.Add(this.axiSensor3);
            this.groupStatusBox.Controls.Add(this.axiSensor2);
            this.groupStatusBox.Controls.Add(this.axiSensor1);
            this.groupStatusBox.Controls.Add(this.axiSensor0);
            this.groupStatusBox.Controls.Add(this.axiDutDelay);
            this.groupStatusBox.Controls.Add(this.axiDutClear);
            this.groupStatusBox.Controls.Add(this.axiDutRecv);
            this.groupStatusBox.Controls.Add(this.axiDioDelay);
            this.groupStatusBox.Controls.Add(this.axiDioClear);
            this.groupStatusBox.Controls.Add(this.axiDioRecv);
            this.groupStatusBox.Location = new System.Drawing.Point(836, 909);
            this.groupStatusBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupStatusBox.Name = "groupStatusBox";
            this.groupStatusBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupStatusBox.Size = new System.Drawing.Size(430, 70);
            this.groupStatusBox.TabIndex = 53;
            this.groupStatusBox.TabStop = false;
            // 
            // axiSensor9
            // 
            this.axiSensor9.Enabled = true;
            this.axiSensor9.Location = new System.Drawing.Point(226, 49);
            this.axiSensor9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor9.Name = "axiSensor9";
            this.axiSensor9.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor9.OcxState")));
            this.axiSensor9.Size = new System.Drawing.Size(50, 14);
            this.axiSensor9.TabIndex = 82;
            this.axiSensor9.OnMouseUp += new AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEventHandler(this.axiSensor9_OnMouseUp);
            // 
            // axiSensor8
            // 
            this.axiSensor8.Enabled = true;
            this.axiSensor8.Location = new System.Drawing.Point(174, 49);
            this.axiSensor8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor8.Name = "axiSensor8";
            this.axiSensor8.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor8.OcxState")));
            this.axiSensor8.Size = new System.Drawing.Size(50, 14);
            this.axiSensor8.TabIndex = 81;
            // 
            // axiSensor7
            // 
            this.axiSensor7.Enabled = true;
            this.axiSensor7.Location = new System.Drawing.Point(118, 49);
            this.axiSensor7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor7.Name = "axiSensor7";
            this.axiSensor7.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor7.OcxState")));
            this.axiSensor7.Size = new System.Drawing.Size(50, 14);
            this.axiSensor7.TabIndex = 80;
            // 
            // axiSensor6
            // 
            this.axiSensor6.Enabled = true;
            this.axiSensor6.Location = new System.Drawing.Point(62, 49);
            this.axiSensor6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor6.Name = "axiSensor6";
            this.axiSensor6.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor6.OcxState")));
            this.axiSensor6.Size = new System.Drawing.Size(50, 14);
            this.axiSensor6.TabIndex = 79;
            // 
            // axiSensor5
            // 
            this.axiSensor5.Enabled = true;
            this.axiSensor5.Location = new System.Drawing.Point(3, 49);
            this.axiSensor5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor5.Name = "axiSensor5";
            this.axiSensor5.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor5.OcxState")));
            this.axiSensor5.Size = new System.Drawing.Size(50, 14);
            this.axiSensor5.TabIndex = 78;
            // 
            // axiSensor4
            // 
            this.axiSensor4.Enabled = true;
            this.axiSensor4.Location = new System.Drawing.Point(222, 32);
            this.axiSensor4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor4.Name = "axiSensor4";
            this.axiSensor4.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor4.OcxState")));
            this.axiSensor4.Size = new System.Drawing.Size(50, 14);
            this.axiSensor4.TabIndex = 77;
            this.axiSensor4.OnMouseUp += new AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEventHandler(this.axiSensor4_OnMouseUp);
            // 
            // axiSensor3
            // 
            this.axiSensor3.Enabled = true;
            this.axiSensor3.Location = new System.Drawing.Point(170, 32);
            this.axiSensor3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor3.Name = "axiSensor3";
            this.axiSensor3.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor3.OcxState")));
            this.axiSensor3.Size = new System.Drawing.Size(50, 14);
            this.axiSensor3.TabIndex = 76;
            // 
            // axiSensor2
            // 
            this.axiSensor2.Enabled = true;
            this.axiSensor2.Location = new System.Drawing.Point(118, 32);
            this.axiSensor2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor2.Name = "axiSensor2";
            this.axiSensor2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor2.OcxState")));
            this.axiSensor2.Size = new System.Drawing.Size(50, 14);
            this.axiSensor2.TabIndex = 75;
            // 
            // axiSensor1
            // 
            this.axiSensor1.Enabled = true;
            this.axiSensor1.Location = new System.Drawing.Point(60, 32);
            this.axiSensor1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor1.Name = "axiSensor1";
            this.axiSensor1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor1.OcxState")));
            this.axiSensor1.Size = new System.Drawing.Size(50, 14);
            this.axiSensor1.TabIndex = 74;
            // 
            // axiSensor0
            // 
            this.axiSensor0.Enabled = true;
            this.axiSensor0.Location = new System.Drawing.Point(3, 32);
            this.axiSensor0.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensor0.Name = "axiSensor0";
            this.axiSensor0.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensor0.OcxState")));
            this.axiSensor0.Size = new System.Drawing.Size(50, 14);
            this.axiSensor0.TabIndex = 73;
            // 
            // axiDutDelay
            // 
            this.axiDutDelay.Enabled = true;
            this.axiDutDelay.Location = new System.Drawing.Point(358, 15);
            this.axiDutDelay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiDutDelay.Name = "axiDutDelay";
            this.axiDutDelay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiDutDelay.OcxState")));
            this.axiDutDelay.Size = new System.Drawing.Size(58, 14);
            this.axiDutDelay.TabIndex = 64;
            // 
            // axiDutClear
            // 
            this.axiDutClear.Enabled = true;
            this.axiDutClear.Location = new System.Drawing.Point(283, 15);
            this.axiDutClear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiDutClear.Name = "axiDutClear";
            this.axiDutClear.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiDutClear.OcxState")));
            this.axiDutClear.Size = new System.Drawing.Size(58, 14);
            this.axiDutClear.TabIndex = 63;
            // 
            // axiDutRecv
            // 
            this.axiDutRecv.Enabled = true;
            this.axiDutRecv.Location = new System.Drawing.Point(217, 15);
            this.axiDutRecv.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiDutRecv.Name = "axiDutRecv";
            this.axiDutRecv.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiDutRecv.OcxState")));
            this.axiDutRecv.Size = new System.Drawing.Size(58, 14);
            this.axiDutRecv.TabIndex = 62;
            // 
            // axiDioDelay
            // 
            this.axiDioDelay.Enabled = true;
            this.axiDioDelay.Location = new System.Drawing.Point(144, 15);
            this.axiDioDelay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiDioDelay.Name = "axiDioDelay";
            this.axiDioDelay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiDioDelay.OcxState")));
            this.axiDioDelay.Size = new System.Drawing.Size(58, 14);
            this.axiDioDelay.TabIndex = 61;
            // 
            // axiDioClear
            // 
            this.axiDioClear.Enabled = true;
            this.axiDioClear.Location = new System.Drawing.Point(73, 15);
            this.axiDioClear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiDioClear.Name = "axiDioClear";
            this.axiDioClear.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiDioClear.OcxState")));
            this.axiDioClear.Size = new System.Drawing.Size(58, 14);
            this.axiDioClear.TabIndex = 60;
            // 
            // axiDioRecv
            // 
            this.axiDioRecv.Enabled = true;
            this.axiDioRecv.Location = new System.Drawing.Point(3, 15);
            this.axiDioRecv.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiDioRecv.Name = "axiDioRecv";
            this.axiDioRecv.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiDioRecv.OcxState")));
            this.axiDioRecv.Size = new System.Drawing.Size(58, 14);
            this.axiDioRecv.TabIndex = 53;
            // 
            // btnInteractive
            // 
            this.btnInteractive.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.btnInteractive.Location = new System.Drawing.Point(641, 2);
            this.btnInteractive.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnInteractive.Name = "btnInteractive";
            this.btnInteractive.Size = new System.Drawing.Size(128, 50);
            this.btnInteractive.TabIndex = 54;
            this.btnInteractive.Text = "INTERACTIVE";
            this.btnInteractive.UseVisualStyleBackColor = true;
            this.btnInteractive.Click += new System.EventHandler(this.btnInteractive_Click);
            // 
            // ixlblWannMsg
            // 
            this.ixlblWannMsg.BackColor = System.Drawing.Color.Black;
            this.ixlblWannMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ixlblWannMsg.Font = new System.Drawing.Font("Verdana", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ixlblWannMsg.ForeColor = System.Drawing.Color.Yellow;
            this.ixlblWannMsg.Location = new System.Drawing.Point(367, 275);
            this.ixlblWannMsg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblWannMsg.Multiline = true;
            this.ixlblWannMsg.Name = "ixlblWannMsg";
            this.ixlblWannMsg.ReadOnly = true;
            this.ixlblWannMsg.Size = new System.Drawing.Size(431, 218);
            this.ixlblWannMsg.TabIndex = 68;
            this.ixlblWannMsg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ixlblWannMsg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ixlblWannMsg_MouseClick);
            // 
            // progressInspection
            // 
            this.progressInspection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressInspection.BackColor = System.Drawing.Color.Beige;
            this.progressInspection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.progressInspection.Location = new System.Drawing.Point(22, 451);
            this.progressInspection.Margin = new System.Windows.Forms.Padding(1);
            this.progressInspection.MarqueeAnimationSpeed = 250;
            this.progressInspection.Name = "progressInspection";
            this.progressInspection.Size = new System.Drawing.Size(526, 31);
            this.progressInspection.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressInspection.TabIndex = 75;
            this.progressInspection.Value = 10;
            // 
            // panelGMES
            // 
            this.panelGMES.Controls.Add(this.lblGmesVersion);
            this.panelGMES.Controls.Add(this.chkBoxMesOn);
            this.panelGMES.Controls.Add(this.label3);
            this.panelGMES.Controls.Add(this.lblMapOn);
            this.panelGMES.Controls.Add(this.ixlblMesLED);
            this.panelGMES.Controls.Add(this.ixlblMesInfo);
            this.panelGMES.Location = new System.Drawing.Point(6, 53);
            this.panelGMES.Name = "panelGMES";
            this.panelGMES.Size = new System.Drawing.Size(277, 78);
            this.panelGMES.TabIndex = 80;
            // 
            // lblGmesVersion
            // 
            this.lblGmesVersion.BackColor = System.Drawing.Color.Gray;
            this.lblGmesVersion.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGmesVersion.Location = new System.Drawing.Point(100, 48);
            this.lblGmesVersion.Name = "lblGmesVersion";
            this.lblGmesVersion.Size = new System.Drawing.Size(140, 20);
            this.lblGmesVersion.TabIndex = 73;
            this.lblGmesVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkBoxMesOn
            // 
            this.chkBoxMesOn.BackColor = System.Drawing.Color.Gray;
            this.chkBoxMesOn.ContextMenuStrip = this.contextMenuMesStrip;
            this.chkBoxMesOn.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBoxMesOn.Location = new System.Drawing.Point(2, 47);
            this.chkBoxMesOn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkBoxMesOn.Name = "chkBoxMesOn";
            this.chkBoxMesOn.Size = new System.Drawing.Size(83, 20);
            this.chkBoxMesOn.TabIndex = 72;
            this.chkBoxMesOn.Text = "GMES ON";
            this.chkBoxMesOn.UseVisualStyleBackColor = false;
            this.chkBoxMesOn.CheckedChanged += new System.EventHandler(this.chkBoxMesOn_CheckedChanged);
            this.chkBoxMesOn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chkBoxMesOn_MouseDown);
            // 
            // contextMenuMesStrip
            // 
            this.contextMenuMesStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuMesStrip.Name = "contextMenuStrip1";
            this.contextMenuMesStrip.Size = new System.Drawing.Size(152, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem1.Text = "Change GMES";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem2.Text = "Change MES";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Gray;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 26);
            this.label3.TabIndex = 71;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMapOn
            // 
            this.lblMapOn.BackColor = System.Drawing.Color.Gray;
            this.lblMapOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMapOn.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapOn.Location = new System.Drawing.Point(0, 24);
            this.lblMapOn.Name = "lblMapOn";
            this.lblMapOn.Size = new System.Drawing.Size(90, 20);
            this.lblMapOn.TabIndex = 70;
            this.lblMapOn.Text = "STATUS";
            this.lblMapOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ixlblMesLED
            // 
            this.ixlblMesLED.Enabled = true;
            this.ixlblMesLED.Location = new System.Drawing.Point(2, 3);
            this.ixlblMesLED.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblMesLED.Name = "ixlblMesLED";
            this.ixlblMesLED.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblMesLED.OcxState")));
            this.ixlblMesLED.Size = new System.Drawing.Size(88, 20);
            this.ixlblMesLED.TabIndex = 69;
            // 
            // ixlblMesInfo
            // 
            this.ixlblMesInfo.ContextMenuStrip = this.contextMenuMesStrip;
            this.ixlblMesInfo.Enabled = true;
            this.ixlblMesInfo.Location = new System.Drawing.Point(0, 1);
            this.ixlblMesInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblMesInfo.Name = "ixlblMesInfo";
            this.ixlblMesInfo.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblMesInfo.OcxState")));
            this.ixlblMesInfo.Size = new System.Drawing.Size(276, 70);
            this.ixlblMesInfo.TabIndex = 68;
            // 
            // panelORACLE
            // 
            this.panelORACLE.Controls.Add(this.chkBoxOracleOn);
            this.panelORACLE.Controls.Add(this.label2);
            this.panelORACLE.Controls.Add(this.lblMapOn2);
            this.panelORACLE.Controls.Add(this.ixlblOracleLED);
            this.panelORACLE.Controls.Add(this.ixlblOracleInfo);
            this.panelORACLE.Location = new System.Drawing.Point(12, 252);
            this.panelORACLE.Name = "panelORACLE";
            this.panelORACLE.Size = new System.Drawing.Size(234, 79);
            this.panelORACLE.TabIndex = 81;
            this.panelORACLE.Visible = false;
            // 
            // chkBoxOracleOn
            // 
            this.chkBoxOracleOn.BackColor = System.Drawing.Color.Gray;
            this.chkBoxOracleOn.ContextMenuStrip = this.contextMenuMesStrip;
            this.chkBoxOracleOn.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkBoxOracleOn.Location = new System.Drawing.Point(2, 47);
            this.chkBoxOracleOn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkBoxOracleOn.Name = "chkBoxOracleOn";
            this.chkBoxOracleOn.Size = new System.Drawing.Size(83, 20);
            this.chkBoxOracleOn.TabIndex = 79;
            this.chkBoxOracleOn.Text = "MES ON";
            this.chkBoxOracleOn.UseVisualStyleBackColor = false;
            this.chkBoxOracleOn.CheckedChanged += new System.EventHandler(this.chkBoxOracleOn_CheckedChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Gray;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 26);
            this.label2.TabIndex = 83;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMapOn2
            // 
            this.lblMapOn2.BackColor = System.Drawing.Color.Gray;
            this.lblMapOn2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMapOn2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapOn2.Location = new System.Drawing.Point(0, 24);
            this.lblMapOn2.Name = "lblMapOn2";
            this.lblMapOn2.Size = new System.Drawing.Size(90, 20);
            this.lblMapOn2.TabIndex = 82;
            this.lblMapOn2.Text = "STATUS";
            this.lblMapOn2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ixlblOracleLED
            // 
            this.ixlblOracleLED.Enabled = true;
            this.ixlblOracleLED.Location = new System.Drawing.Point(2, 3);
            this.ixlblOracleLED.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblOracleLED.Name = "ixlblOracleLED";
            this.ixlblOracleLED.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblOracleLED.OcxState")));
            this.ixlblOracleLED.Size = new System.Drawing.Size(88, 20);
            this.ixlblOracleLED.TabIndex = 81;
            // 
            // ixlblOracleInfo
            // 
            this.ixlblOracleInfo.Enabled = true;
            this.ixlblOracleInfo.Location = new System.Drawing.Point(1, 1);
            this.ixlblOracleInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblOracleInfo.Name = "ixlblOracleInfo";
            this.ixlblOracleInfo.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblOracleInfo.OcxState")));
            this.ixlblOracleInfo.Size = new System.Drawing.Size(211, 63);
            this.ixlblOracleInfo.TabIndex = 78;
            // 
            // ixlblNetworkChkMsg
            // 
            this.ixlblNetworkChkMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ixlblNetworkChkMsg.BackColor = System.Drawing.Color.Black;
            this.ixlblNetworkChkMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ixlblNetworkChkMsg.Font = new System.Drawing.Font("Verdana", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ixlblNetworkChkMsg.ForeColor = System.Drawing.Color.Yellow;
            this.ixlblNetworkChkMsg.Location = new System.Drawing.Point(217, 560);
            this.ixlblNetworkChkMsg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblNetworkChkMsg.Name = "ixlblNetworkChkMsg";
            this.ixlblNetworkChkMsg.ReadOnly = true;
            this.ixlblNetworkChkMsg.Size = new System.Drawing.Size(581, 52);
            this.ixlblNetworkChkMsg.TabIndex = 82;
            this.ixlblNetworkChkMsg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblVersion
            // 
            this.lblVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVersion.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(283, 105);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(282, 19);
            this.lblVersion.TabIndex = 83;
            this.lblVersion.Text = "Release date";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVersion.DoubleClick += new System.EventHandler(this.lblVersion_DoubleClick);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Location = new System.Drawing.Point(835, 681);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(433, 221);
            this.tabControl.TabIndex = 84;
            this.tabControl.Resize += new System.EventHandler(this.tabControl_Resize);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listboxLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(425, 193);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Inspection Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listboxLog
            // 
            this.listboxLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.listboxLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listboxLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listboxLog.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxLog.ForeColor = System.Drawing.Color.Black;
            this.listboxLog.FormattingEnabled = true;
            this.listboxLog.HorizontalScrollbar = true;
            this.listboxLog.ItemHeight = 16;
            this.listboxLog.Location = new System.Drawing.Point(6, 5);
            this.listboxLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listboxLog.Name = "listboxLog";
            this.listboxLog.ScrollAlwaysVisible = true;
            this.listboxLog.Size = new System.Drawing.Size(406, 178);
            this.listboxLog.TabIndex = 40;
            this.listboxLog.Click += new System.EventHandler(this.listboxLog_Click);
            this.listboxLog.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listboxLog_MouseClick);
            this.listboxLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listboxLog_DrawItem);
            this.listboxLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listboxLog_MouseDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listboxBinLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(425, 193);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Bin Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listboxBinLog
            // 
            this.listboxBinLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.listboxBinLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listboxBinLog.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxBinLog.ForeColor = System.Drawing.Color.White;
            this.listboxBinLog.FormattingEnabled = true;
            this.listboxBinLog.HorizontalScrollbar = true;
            this.listboxBinLog.ItemHeight = 16;
            this.listboxBinLog.Location = new System.Drawing.Point(6, 5);
            this.listboxBinLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listboxBinLog.Name = "listboxBinLog";
            this.listboxBinLog.ScrollAlwaysVisible = true;
            this.listboxBinLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listboxBinLog.Size = new System.Drawing.Size(407, 178);
            this.listboxBinLog.TabIndex = 81;
            this.listboxBinLog.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listboxBinLog_MouseClick);
            this.listboxBinLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listboxBinLog_DrawItem);
            this.listboxBinLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listboxBinLog_MouseDoubleClick);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listboxResultLog);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(425, 193);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Test Log";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listboxResultLog
            // 
            this.listboxResultLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.listboxResultLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listboxResultLog.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxResultLog.ForeColor = System.Drawing.Color.White;
            this.listboxResultLog.FormattingEnabled = true;
            this.listboxResultLog.HorizontalScrollbar = true;
            this.listboxResultLog.ItemHeight = 16;
            this.listboxResultLog.Location = new System.Drawing.Point(9, 15);
            this.listboxResultLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listboxResultLog.Name = "listboxResultLog";
            this.listboxResultLog.ScrollAlwaysVisible = true;
            this.listboxResultLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listboxResultLog.Size = new System.Drawing.Size(407, 162);
            this.listboxResultLog.TabIndex = 85;
            this.listboxResultLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listboxResultLog_MouseDoubleClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.axiSensorRepeat);
            this.tabPage3.Controls.Add(this.axiSensorDebug);
            this.tabPage3.Controls.Add(this.listboxEtcLog);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(425, 193);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Etc";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // axiSensorRepeat
            // 
            this.axiSensorRepeat.Enabled = true;
            this.axiSensorRepeat.Location = new System.Drawing.Point(81, 3);
            this.axiSensorRepeat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensorRepeat.Name = "axiSensorRepeat";
            this.axiSensorRepeat.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensorRepeat.OcxState")));
            this.axiSensorRepeat.Size = new System.Drawing.Size(72, 14);
            this.axiSensorRepeat.TabIndex = 88;
            this.axiSensorRepeat.OnClick += new System.EventHandler(this.axiSensorRepeat_OnClick);
            // 
            // axiSensorDebug
            // 
            this.axiSensorDebug.Enabled = true;
            this.axiSensorDebug.Location = new System.Drawing.Point(3, 3);
            this.axiSensorDebug.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiSensorDebug.Name = "axiSensorDebug";
            this.axiSensorDebug.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiSensorDebug.OcxState")));
            this.axiSensorDebug.Size = new System.Drawing.Size(72, 14);
            this.axiSensorDebug.TabIndex = 83;
            this.axiSensorDebug.OnClick += new System.EventHandler(this.axiSensorDebug_OnClick);
            // 
            // listboxEtcLog
            // 
            this.listboxEtcLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.listboxEtcLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listboxEtcLog.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxEtcLog.ForeColor = System.Drawing.Color.White;
            this.listboxEtcLog.FormattingEnabled = true;
            this.listboxEtcLog.HorizontalScrollbar = true;
            this.listboxEtcLog.ItemHeight = 16;
            this.listboxEtcLog.Location = new System.Drawing.Point(6, 21);
            this.listboxEtcLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listboxEtcLog.Name = "listboxEtcLog";
            this.listboxEtcLog.ScrollAlwaysVisible = true;
            this.listboxEtcLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listboxEtcLog.Size = new System.Drawing.Size(407, 162);
            this.listboxEtcLog.TabIndex = 82;
            this.listboxEtcLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listboxEtcLog_MouseDoubleClick);
            // 
            // txtBoxWip2
            // 
            this.txtBoxWip2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxWip2.Enabled = false;
            this.txtBoxWip2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxWip2.Location = new System.Drawing.Point(955, 529);
            this.txtBoxWip2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBoxWip2.MaxLength = 25;
            this.txtBoxWip2.Name = "txtBoxWip2";
            this.txtBoxWip2.Size = new System.Drawing.Size(312, 26);
            this.txtBoxWip2.TabIndex = 94;
            this.txtBoxWip2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // axiLabelWip2
            // 
            this.axiLabelWip2.Enabled = true;
            this.axiLabelWip2.Location = new System.Drawing.Point(837, 529);
            this.axiLabelWip2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelWip2.Name = "axiLabelWip2";
            this.axiLabelWip2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelWip2.OcxState")));
            this.axiLabelWip2.Size = new System.Drawing.Size(117, 26);
            this.axiLabelWip2.TabIndex = 93;
            // 
            // axiLabelCurrent
            // 
            this.axiLabelCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axiLabelCurrent.Enabled = true;
            this.axiLabelCurrent.Location = new System.Drawing.Point(769, 2);
            this.axiLabelCurrent.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelCurrent.Name = "axiLabelCurrent";
            this.axiLabelCurrent.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelCurrent.OcxState")));
            this.axiLabelCurrent.Size = new System.Drawing.Size(128, 19);
            this.axiLabelCurrent.TabIndex = 35;
            this.axiLabelCurrent.OnDblClick += new System.EventHandler(this.axiLabelCurrent_OnDblClick);
            // 
            // axiCurrent
            // 
            this.axiCurrent.AccessibleRole = System.Windows.Forms.AccessibleRole.OutlineButton;
            this.axiCurrent.Enabled = true;
            this.axiCurrent.Location = new System.Drawing.Point(769, 21);
            this.axiCurrent.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiCurrent.Name = "axiCurrent";
            this.axiCurrent.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiCurrent.OcxState")));
            this.axiCurrent.Size = new System.Drawing.Size(128, 30);
            this.axiCurrent.TabIndex = 36;
            this.axiCurrent.OnDblClick += new System.EventHandler(this.axiCurrent_OnDblClick);
            // 
            // ChartCurrent
            // 
            this.ChartCurrent.Enabled = true;
            this.ChartCurrent.Location = new System.Drawing.Point(24, 717);
            this.ChartCurrent.Name = "ChartCurrent";
            this.ChartCurrent.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ChartCurrent.OcxState")));
            this.ChartCurrent.Size = new System.Drawing.Size(422, 185);
            this.ChartCurrent.TabIndex = 92;
            this.ChartCurrent.Visible = false;
            this.ChartCurrent.OnDblClickDataView += new AxiPlotLibrary.IiPlotXEvents_OnDblClickDataViewEventHandler(this.ChartCurrent_OnDblClickDataView);
            // 
            // ixlblStatusDate
            // 
            this.ixlblStatusDate.Enabled = true;
            this.ixlblStatusDate.Location = new System.Drawing.Point(801, 294);
            this.ixlblStatusDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblStatusDate.Name = "ixlblStatusDate";
            this.ixlblStatusDate.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblStatusDate.OcxState")));
            this.ixlblStatusDate.Size = new System.Drawing.Size(93, 16);
            this.ixlblStatusDate.TabIndex = 77;
            // 
            // ixlblStatusCount
            // 
            this.ixlblStatusCount.Enabled = true;
            this.ixlblStatusCount.Location = new System.Drawing.Point(823, 447);
            this.ixlblStatusCount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblStatusCount.Name = "ixlblStatusCount";
            this.ixlblStatusCount.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblStatusCount.OcxState")));
            this.ixlblStatusCount.Size = new System.Drawing.Size(431, 35);
            this.ixlblStatusCount.TabIndex = 69;
            // 
            // axilblBackboarder
            // 
            this.axilblBackboarder.Enabled = true;
            this.axilblBackboarder.Location = new System.Drawing.Point(22, 356);
            this.axilblBackboarder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axilblBackboarder.Name = "axilblBackboarder";
            this.axilblBackboarder.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axilblBackboarder.OcxState")));
            this.axilblBackboarder.Size = new System.Drawing.Size(424, 137);
            this.axilblBackboarder.TabIndex = 76;
            // 
            // axiBuffCounter4
            // 
            this.axiBuffCounter4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.axiBuffCounter4.Enabled = true;
            this.axiBuffCounter4.Location = new System.Drawing.Point(609, 983);
            this.axiBuffCounter4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiBuffCounter4.Name = "axiBuffCounter4";
            this.axiBuffCounter4.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiBuffCounter4.OcxState")));
            this.axiBuffCounter4.Size = new System.Drawing.Size(111, 13);
            this.axiBuffCounter4.TabIndex = 73;
            // 
            // axiBuffCounter3
            // 
            this.axiBuffCounter3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.axiBuffCounter3.Enabled = true;
            this.axiBuffCounter3.Location = new System.Drawing.Point(383, 983);
            this.axiBuffCounter3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiBuffCounter3.Name = "axiBuffCounter3";
            this.axiBuffCounter3.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiBuffCounter3.OcxState")));
            this.axiBuffCounter3.Size = new System.Drawing.Size(219, 13);
            this.axiBuffCounter3.TabIndex = 72;
            // 
            // axiBuffCounter2
            // 
            this.axiBuffCounter2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.axiBuffCounter2.Enabled = true;
            this.axiBuffCounter2.Location = new System.Drawing.Point(158, 983);
            this.axiBuffCounter2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiBuffCounter2.Name = "axiBuffCounter2";
            this.axiBuffCounter2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiBuffCounter2.OcxState")));
            this.axiBuffCounter2.Size = new System.Drawing.Size(219, 13);
            this.axiBuffCounter2.TabIndex = 71;
            // 
            // axiBuffCounter1
            // 
            this.axiBuffCounter1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.axiBuffCounter1.Enabled = true;
            this.axiBuffCounter1.Location = new System.Drawing.Point(6, 983);
            this.axiBuffCounter1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiBuffCounter1.Name = "axiBuffCounter1";
            this.axiBuffCounter1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiBuffCounter1.OcxState")));
            this.axiBuffCounter1.Size = new System.Drawing.Size(146, 13);
            this.axiBuffCounter1.TabIndex = 70;
            // 
            // axiEngCounter
            // 
            this.axiEngCounter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.axiEngCounter.Enabled = true;
            this.axiEngCounter.Location = new System.Drawing.Point(837, 983);
            this.axiEngCounter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiEngCounter.Name = "axiEngCounter";
            this.axiEngCounter.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiEngCounter.OcxState")));
            this.axiEngCounter.Size = new System.Drawing.Size(291, 13);
            this.axiEngCounter.TabIndex = 66;
            // 
            // axiBuildVer
            // 
            this.axiBuildVer.Enabled = true;
            this.axiBuildVer.Location = new System.Drawing.Point(1209, 983);
            this.axiBuildVer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiBuildVer.Name = "axiBuildVer";
            this.axiBuildVer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiBuildVer.OcxState")));
            this.axiBuildVer.Size = new System.Drawing.Size(59, 13);
            this.axiBuildVer.TabIndex = 65;
            this.axiBuildVer.OnClick += new System.EventHandler(this.axiBuildVer_OnClick);
            // 
            // isegGpsTimer
            // 
            this.isegGpsTimer.AccessibleRole = System.Windows.Forms.AccessibleRole.OutlineButton;
            this.isegGpsTimer.Enabled = true;
            this.isegGpsTimer.Location = new System.Drawing.Point(955, 594);
            this.isegGpsTimer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.isegGpsTimer.Name = "isegGpsTimer";
            this.isegGpsTimer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("isegGpsTimer.OcxState")));
            this.isegGpsTimer.Size = new System.Drawing.Size(141, 49);
            this.isegGpsTimer.TabIndex = 50;
            this.isegGpsTimer.Visible = false;
            // 
            // axiLabelCN0
            // 
            this.axiLabelCN0.Enabled = true;
            this.axiLabelCN0.Location = new System.Drawing.Point(1102, 619);
            this.axiLabelCN0.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelCN0.Name = "axiLabelCN0";
            this.axiLabelCN0.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelCN0.OcxState")));
            this.axiLabelCN0.Size = new System.Drawing.Size(89, 24);
            this.axiLabelCN0.TabIndex = 47;
            this.axiLabelCN0.Visible = false;
            // 
            // axiLabelScount
            // 
            this.axiLabelScount.Enabled = true;
            this.axiLabelScount.Location = new System.Drawing.Point(1102, 594);
            this.axiLabelScount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelScount.Name = "axiLabelScount";
            this.axiLabelScount.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelScount.OcxState")));
            this.axiLabelScount.Size = new System.Drawing.Size(89, 26);
            this.axiLabelScount.TabIndex = 46;
            this.axiLabelScount.Visible = false;
            // 
            // axiLabelGPSTIME
            // 
            this.axiLabelGPSTIME.Enabled = true;
            this.axiLabelGPSTIME.Location = new System.Drawing.Point(837, 594);
            this.axiLabelGPSTIME.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelGPSTIME.Name = "axiLabelGPSTIME";
            this.axiLabelGPSTIME.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelGPSTIME.OcxState")));
            this.axiLabelGPSTIME.Size = new System.Drawing.Size(117, 49);
            this.axiLabelGPSTIME.TabIndex = 45;
            this.axiLabelGPSTIME.Visible = false;
            // 
            // axiLabelBarcode
            // 
            this.axiLabelBarcode.Enabled = true;
            this.axiLabelBarcode.Location = new System.Drawing.Point(837, 559);
            this.axiLabelBarcode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelBarcode.Name = "axiLabelBarcode";
            this.axiLabelBarcode.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelBarcode.OcxState")));
            this.axiLabelBarcode.Size = new System.Drawing.Size(117, 26);
            this.axiLabelBarcode.TabIndex = 31;
            this.axiLabelBarcode.OnClick += new System.EventHandler(this.axiLabelBarcode_OnClick);
            // 
            // axiLabelWip1
            // 
            this.axiLabelWip1.Enabled = true;
            this.axiLabelWip1.Location = new System.Drawing.Point(837, 499);
            this.axiLabelWip1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiLabelWip1.Name = "axiLabelWip1";
            this.axiLabelWip1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiLabelWip1.OcxState")));
            this.axiLabelWip1.Size = new System.Drawing.Size(117, 26);
            this.axiLabelWip1.TabIndex = 29;
            this.axiLabelWip1.OnClick += new System.EventHandler(this.axiLabelWip1_OnClick);
            // 
            // ixlblTotal
            // 
            this.ixlblTotal.Enabled = true;
            this.ixlblTotal.Location = new System.Drawing.Point(471, 81);
            this.ixlblTotal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblTotal.Name = "ixlblTotal";
            this.ixlblTotal.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblTotal.OcxState")));
            this.ixlblTotal.Size = new System.Drawing.Size(94, 23);
            this.ixlblTotal.TabIndex = 16;
            this.ixlblTotal.OnClick += new System.EventHandler(this.ixlblTotal_OnClick);
            // 
            // ixlblFail
            // 
            this.ixlblFail.Enabled = true;
            this.ixlblFail.Location = new System.Drawing.Point(377, 81);
            this.ixlblFail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblFail.Name = "ixlblFail";
            this.ixlblFail.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblFail.OcxState")));
            this.ixlblFail.Size = new System.Drawing.Size(94, 23);
            this.ixlblFail.TabIndex = 15;
            this.ixlblFail.OnClick += new System.EventHandler(this.ixlblFail_OnClick);
            // 
            // LabelTOTAL
            // 
            this.LabelTOTAL.Enabled = true;
            this.LabelTOTAL.Location = new System.Drawing.Point(471, 54);
            this.LabelTOTAL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LabelTOTAL.Name = "LabelTOTAL";
            this.LabelTOTAL.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("LabelTOTAL.OcxState")));
            this.LabelTOTAL.Size = new System.Drawing.Size(94, 30);
            this.LabelTOTAL.TabIndex = 12;
            this.LabelTOTAL.OnClick += new System.EventHandler(this.LabelTOTAL_OnClick);
            // 
            // LabelFAIL
            // 
            this.LabelFAIL.Enabled = true;
            this.LabelFAIL.Location = new System.Drawing.Point(377, 54);
            this.LabelFAIL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LabelFAIL.Name = "LabelFAIL";
            this.LabelFAIL.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("LabelFAIL.OcxState")));
            this.LabelFAIL.Size = new System.Drawing.Size(94, 30);
            this.LabelFAIL.TabIndex = 11;
            this.LabelFAIL.OnClick += new System.EventHandler(this.LabelFAIL_OnClick);
            // 
            // isegTimer
            // 
            this.isegTimer.AccessibleRole = System.Windows.Forms.AccessibleRole.OutlineButton;
            this.isegTimer.Enabled = true;
            this.isegTimer.Location = new System.Drawing.Point(903, 2);
            this.isegTimer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.isegTimer.Name = "isegTimer";
            this.isegTimer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("isegTimer.OcxState")));
            this.isegTimer.Size = new System.Drawing.Size(128, 50);
            this.isegTimer.TabIndex = 8;
            this.isegTimer.OnClick += new System.EventHandler(this.isegTimer_OnClick);
            // 
            // ixlblStatus
            // 
            this.ixlblStatus.Enabled = true;
            this.ixlblStatus.Location = new System.Drawing.Point(837, 275);
            this.ixlblStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblStatus.Name = "ixlblStatus";
            this.ixlblStatus.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblStatus.OcxState")));
            this.ixlblStatus.Size = new System.Drawing.Size(431, 218);
            this.ixlblStatus.TabIndex = 13;
            this.ixlblStatus.OnClick += new System.EventHandler(this.ixlblStatus_OnClick);
            // 
            // ixlblPass
            // 
            this.ixlblPass.Enabled = true;
            this.ixlblPass.Location = new System.Drawing.Point(283, 81);
            this.ixlblPass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ixlblPass.Name = "ixlblPass";
            this.ixlblPass.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ixlblPass.OcxState")));
            this.ixlblPass.Size = new System.Drawing.Size(94, 23);
            this.ixlblPass.TabIndex = 14;
            this.ixlblPass.OnClick += new System.EventHandler(this.ixlblPass_OnClick);
            // 
            // LabelPass
            // 
            this.LabelPass.Enabled = true;
            this.LabelPass.Location = new System.Drawing.Point(283, 54);
            this.LabelPass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LabelPass.Name = "LabelPass";
            this.LabelPass.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("LabelPass.OcxState")));
            this.LabelPass.Size = new System.Drawing.Size(94, 30);
            this.LabelPass.TabIndex = 10;
            this.LabelPass.OnClick += new System.EventHandler(this.LabelPass_OnClick);
            // 
            // axiCommandStatus
            // 
            this.axiCommandStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axiCommandStatus.Enabled = true;
            this.axiCommandStatus.Location = new System.Drawing.Point(823, 903);
            this.axiCommandStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axiCommandStatus.Name = "axiCommandStatus";
            this.axiCommandStatus.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axiCommandStatus.OcxState")));
            this.axiCommandStatus.Size = new System.Drawing.Size(58, 16);
            this.axiCommandStatus.TabIndex = 95;
            // 
            // FrmFaMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1272, 998);
            this.ControlBox = false;
            this.Controls.Add(this.axiCommandStatus);
            this.Controls.Add(this.txtBoxWip2);
            this.Controls.Add(this.axiLabelWip2);
            this.Controls.Add(this.axiLabelCurrent);
            this.Controls.Add(this.axiCurrent);
            this.Controls.Add(this.ChartCurrent);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnInteractive);
            this.Controls.Add(this.btnDisplay);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.ixlblNetworkChkMsg);
            this.Controls.Add(this.panelORACLE);
            this.Controls.Add(this.panelGMES);
            this.Controls.Add(this.ixlblStatusDate);
            this.Controls.Add(this.ixlblStatusCount);
            this.Controls.Add(this.progressInspection);
            this.Controls.Add(this.axilblBackboarder);
            this.Controls.Add(this.axiBuffCounter4);
            this.Controls.Add(this.axiBuffCounter3);
            this.Controls.Add(this.axiBuffCounter2);
            this.Controls.Add(this.axiBuffCounter1);
            this.Controls.Add(this.axiEngCounter);
            this.Controls.Add(this.axiBuildVer);
            this.Controls.Add(this.groupStatusBox);
            this.Controls.Add(this.isegGpsTimer);
            this.Controls.Add(this.txtBoxScount);
            this.Controls.Add(this.txtBoxCN0);
            this.Controls.Add(this.axiLabelCN0);
            this.Controls.Add(this.axiLabelScount);
            this.Controls.Add(this.axiLabelGPSTIME);
            this.Controls.Add(this.dataGridDevice);
            this.Controls.Add(this.txtBoxWip1);
            this.Controls.Add(this.txtBoxBarcode);
            this.Controls.Add(this.axiLabelBarcode);
            this.Controls.Add(this.axiLabelWip1);
            this.Controls.Add(this.dataGridConnector);
            this.Controls.Add(this.ixlblTotal);
            this.Controls.Add(this.ixlblFail);
            this.Controls.Add(this.LabelTOTAL);
            this.Controls.Add(this.LabelFAIL);
            this.Controls.Add(this.isegTimer);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.ixlblStatus);
            this.Controls.Add(this.cbJobFiles);
            this.Controls.Add(this.ixlblWannMsg);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.ixlblPass);
            this.Controls.Add(this.LabelPass);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimizeBox = false;
            this.Name = "FrmFaMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GM TELEMATICS TESTER";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmFaMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmFaMain_Load);
            this.Shown += new System.EventHandler(this.FrmFaMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridConnector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDevice)).EndInit();
            this.groupStatusBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensor0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDutDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDutClear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDutRecv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDioDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDioClear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiDioRecv)).EndInit();
            this.panelGMES.ResumeLayout(false);
            this.contextMenuMesStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ixlblMesLED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblMesInfo)).EndInit();
            this.panelORACLE.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ixlblOracleLED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblOracleInfo)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axiSensorRepeat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiSensorDebug)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelWip2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblStatusDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblStatusCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axilblBackboarder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuffCounter1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiEngCounter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiBuildVer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isegGpsTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelCN0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelScount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelGPSTIME)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiLabelWip1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblFail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelTOTAL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelFAIL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isegTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ixlblPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axiCommandStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ComboBox cbJobFiles;
        private AxisAnalogLibrary.AxiSevenSegmentAnalogX isegTimer;
        private AxisAnalogLibrary.AxiLabelX LabelPass;
        private AxisAnalogLibrary.AxiLabelX LabelFAIL;
        private AxisAnalogLibrary.AxiLabelX LabelTOTAL;
        private AxisAnalogLibrary.AxiLabelX ixlblStatus;
        private AxisAnalogLibrary.AxiLabelX ixlblTotal;
        private AxisAnalogLibrary.AxiLabelX ixlblFail;
        private AxisAnalogLibrary.AxiLabelX ixlblPass;
        private System.Windows.Forms.Button btnDisplay;
        private System.Windows.Forms.DataGridView dataGridConnector;
        private AxisAnalogLibrary.AxiLabelX axiLabelWip1;
        private AxisAnalogLibrary.AxiLabelX axiLabelBarcode;
        private System.Windows.Forms.TextBox txtBoxBarcode;
        private System.Windows.Forms.TextBox txtBoxWip1;
        private AxisAnalogLibrary.AxiLabelX axiLabelCurrent;
        private AxisAnalogLibrary.AxiSevenSegmentAnalogX axiCurrent;
        private System.Windows.Forms.DataGridView dataGridDevice;
        private AxisAnalogLibrary.AxiLabelX axiLabelGPSTIME;
        private AxisAnalogLibrary.AxiLabelX axiLabelScount;
        private AxisAnalogLibrary.AxiLabelX axiLabelCN0;
        private System.Windows.Forms.TextBox txtBoxCN0;
        private System.Windows.Forms.TextBox txtBoxScount;
        private AxisAnalogLibrary.AxiSevenSegmentAnalogX isegGpsTimer;
        private System.Windows.Forms.GroupBox groupStatusBox;
        private AxisAnalogLibrary.AxiLabelX axiDioRecv;
        private AxisAnalogLibrary.AxiLabelX axiDutDelay;
        private AxisAnalogLibrary.AxiLabelX axiDutClear;
        private AxisAnalogLibrary.AxiLabelX axiDutRecv;
        private AxisAnalogLibrary.AxiLabelX axiDioDelay;
        private AxisAnalogLibrary.AxiLabelX axiDioClear;
        private System.Windows.Forms.Button btnInteractive;
        private AxisAnalogLibrary.AxiLabelX axiBuildVer;
        private AxisAnalogLibrary.AxiLabelX axiEngCounter;
        private AxisAnalogLibrary.AxiLabelX axiSensor9;
        private AxisAnalogLibrary.AxiLabelX axiSensor8;
        private AxisAnalogLibrary.AxiLabelX axiSensor7;
        private AxisAnalogLibrary.AxiLabelX axiSensor6;
        private AxisAnalogLibrary.AxiLabelX axiSensor5;
        private AxisAnalogLibrary.AxiLabelX axiSensor4;
        private AxisAnalogLibrary.AxiLabelX axiSensor3;
        private AxisAnalogLibrary.AxiLabelX axiSensor2;
        private AxisAnalogLibrary.AxiLabelX axiSensor1;
        private AxisAnalogLibrary.AxiLabelX axiSensor0;
        private System.Windows.Forms.TextBox ixlblWannMsg;
        private AxisAnalogLibrary.AxiLabelX ixlblStatusCount;
        private AxisAnalogLibrary.AxiLabelX axiBuffCounter1;
        private AxisAnalogLibrary.AxiLabelX axiBuffCounter2;
        private AxisAnalogLibrary.AxiLabelX axiBuffCounter3;
        private AxisAnalogLibrary.AxiLabelX axiBuffCounter4;
        private System.Windows.Forms.ProgressBar progressInspection;
        private AxisAnalogLibrary.AxiLabelX axilblBackboarder;
        private AxisAnalogLibrary.AxiLabelX ixlblStatusDate;
        private System.Windows.Forms.Panel panelGMES;
        private System.Windows.Forms.Label lblGmesVersion;
        private System.Windows.Forms.CheckBox chkBoxMesOn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblMapOn;
        private AxisAnalogLibrary.AxiLabelX ixlblMesLED;
        private AxisAnalogLibrary.AxiLabelX ixlblMesInfo;
        private System.Windows.Forms.Panel panelORACLE;
        private AxisAnalogLibrary.AxiLabelX ixlblOracleInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuMesStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.CheckBox chkBoxOracleOn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMapOn2;
        private AxisAnalogLibrary.AxiLabelX ixlblOracleLED;
        private System.Windows.Forms.TextBox ixlblNetworkChkMsg;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox listboxBinLog;
        private System.Windows.Forms.ListBox listboxLog;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox listboxEtcLog;
        private AxisAnalogLibrary.AxiLabelX axiSensorDebug;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListBox listboxResultLog;
        private AxiPlotLibrary.AxiPlotX ChartCurrent;
        private System.Windows.Forms.TextBox txtBoxWip2;
        private AxisAnalogLibrary.AxiLabelX axiLabelWip2;
        private AxisAnalogLibrary.AxiLabelX axiCommandStatus;
        private AxisAnalogLibrary.AxiLabelX axiSensorRepeat;
    }
}

