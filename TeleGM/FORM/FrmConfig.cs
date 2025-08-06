using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.VisaNS;

namespace GmTelematics
{
    public partial class FrmConfig : Form
    {
        private const string devDIO     = "DIO";
        private const string devSET     = "SET";
        private const string dev232Scan = "RS232SCANNER";
        private const string devPcan    = "PCAN";
        private const string devVector  = "VECTOR";
        private const string dev5515C   = "5515C";        
        private const string devMTP200  = "MTP200";
        private const string devTC3000  = "TC3000";
        private const string devTC1400A = "TC1400A";
        private const string devNAD     = "NAD";
        private const string devMoto    = "ZebraScanner";
        private const string devAudio   = "AUDIOSELECTOR";
        private const string devADC     = "ADC-MODULE";      
        private const string devStepCheckMode = "STEPCHECKMODE";
        private const string dev34410a  = "34410A";
        private const string devOdaPower = "ODAPWR";
        private const string devKEITHLEY = "KEITHLEY";
        private const string devDLLGate  = "DLLGATE";
        private const string devMelsec = "MELSEC";
        private const string devMTP120A = "MTP120A";

        private ContextMenuStrip pAddressMenu;        
        private ContextMenuStrip pComportMenu;
        private ContextMenuStrip pAddressMenuUsb;
        private ContextMenuStrip pAddressMenuTcp;

        private ToolStripMenuItem[] mnuAddress;  //gpib address 용
        private ToolStripMenuItem[] mnuComport;  //Comport List 용
        private ToolStripMenuItem[] mnuAddressUsb;  //Usb address 용
        private ToolStripMenuItem[] mnuAddressTcp;  //Tcp address 용

        private DK_LOGGER DKLoggerConfig = new DK_LOGGER("PC", false);

        //For ORACLE
        private const string sOraProductionLine = "PRODUCTION_LINE";
        private const string sOraProcessCode = "PROCESS_CODE";
        private const string sOraPCID = "PC_ID";
        private const string sOraServerIP = "SERVER_IP";
        private const string sOraPort = "PORT";
        private const string sOraServiceName = "SERVICE_NAME";
        private const string sOraOOBCode = "OOB_CODE";
        private const string sOraOOBFlag = "OOB_FLAG";
        private const string sOraCallType = "CALLTYPE";

        private int    iLoginUser = 0;
        private PWUSER pwLoginUser = new PWUSER();
        public FrmConfig(int iCert, PWUSER pwUser)
        {
            iLoginUser = iCert;
            pwLoginUser = pwUser;
            pAddressMenu = new ContextMenuStrip();
            pComportMenu = new ContextMenuStrip();
            pAddressMenuUsb = new ContextMenuStrip();
            pAddressMenuTcp = new ContextMenuStrip();

            InitializeComponent();
            UI_Config();
            Load_Config();
            FindResources();
            FindComports();
            FindResourcesUsb();
            FindResourcesTcp();
            LoadDioCommands();            
        }

#region UI 제어관련

        private void DgvHeaderNotSortable(DataGridView target)
        {
            for (int i = 0; i < target.Columns.Count; i++)
            {
                target.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }


        private void UI_Config(){
      
            string strColName = String.Empty;
            string strColText = String.Empty;

            strColName = "Col0"; 
            strColText = "Value";
            dataGridPort1.Columns.Add(strColName, strColText);        
            dataGridPort1.Columns[0].Width = (int)(dataGridPort1.Width * 0.62);
            dataGridPort1.Rows.Add(15);

            dataGridPort1.Rows[0].HeaderCell.Value = devDIO;
            dataGridPort1.Rows[1].HeaderCell.Value = devSET;
            dataGridPort1.Rows[2].HeaderCell.Value = dev5515C;            
            dataGridPort1.Rows[3].HeaderCell.Value = devMTP200;
            dataGridPort1.Rows[4].HeaderCell.Value = dev232Scan;
            dataGridPort1.Rows[5].HeaderCell.Value = devTC3000;
            dataGridPort1.Rows[6].HeaderCell.Value = devTC1400A;
            dataGridPort1.Rows[7].HeaderCell.Value = devNAD + "(CCM)";
            dataGridPort1.Rows[8].HeaderCell.Value = devMoto;
            dataGridPort1.Rows[9].HeaderCell.Value = devAudio;
            dataGridPort1.Rows[10].HeaderCell.Value = devADC;
            dataGridPort1.Rows[11].HeaderCell.Value = dev34410a;
            dataGridPort1.Rows[12].HeaderCell.Value = devOdaPower;
            dataGridPort1.Rows[13].HeaderCell.Value = devKEITHLEY;
            dataGridPort1.Rows[14].HeaderCell.Value = devMTP120A;

            dataGridPort1.RowHeadersWidth = dataGridPort1.Width - dataGridPort1.Columns[0].Width;

            for (int i = 0; i < dataGridPort1.Rows.Count; i++)
            {
                dataGridPort1.Rows[i].Height = 22;
            }

                //int iGridSize = (int)(dataGridViewGMES.Width / 2);

            //ORACLE ----------------------
            dataGridViewOracle.Columns.Add("INDEX00", "VALUE");
            dataGridViewOracle.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewOracle.Rows.Add(8);
            dataGridViewOracle.Rows[0].HeaderCell.Value = "Production Line";
            dataGridViewOracle.Rows[1].HeaderCell.Value = "Process Code";
            dataGridViewOracle.Rows[2].HeaderCell.Value = "PC ID";
            dataGridViewOracle.Rows[3].HeaderCell.Value = "Server IP";
            dataGridViewOracle.Rows[4].HeaderCell.Value = "Server Port";
            dataGridViewOracle.Rows[5].HeaderCell.Value = "Service Name";
            dataGridViewOracle.Rows[6].HeaderCell.Value = "OOB Code";
            dataGridViewOracle.Rows[7].HeaderCell.Value = "OOB Flag";

            dataGridViewOracle.Columns[0].Width = (int)(dataGridViewOracle.Width * 0.5);
            dataGridViewOracle.RowHeadersWidth = dataGridViewOracle.Width - dataGridViewOracle.Columns[0].Width;

            int iGridHeight = (int)((dataGridViewOracle.Height - dataGridViewOracle.ColumnHeadersHeight) / dataGridViewOracle.Rows.Count - 1);
            for (int i = 0; i < dataGridViewOracle.Rows.Count; i++)
            {
                dataGridViewOracle.Rows[i].Height = iGridHeight + 1;
            }

            //GMES ----------------------
            dataGridViewGMES.Columns.Add("INDEX00", "VALUE");
            dataGridViewGMES.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewGMES.Rows.Add(6);
            dataGridViewGMES.Rows[0].HeaderCell.Value = "SEVER IP";
            dataGridViewGMES.Rows[1].HeaderCell.Value = "LOCAL IP";
            dataGridViewGMES.Rows[2].HeaderCell.Value = "PORT NUMBER";
            dataGridViewGMES.Rows[3].HeaderCell.Value = "MSG RETRY";
            dataGridViewGMES.Rows[4].HeaderCell.Value = "T3";
            dataGridViewGMES.Rows[5].HeaderCell.Value = "T6";
            dataGridViewGMES.Columns[0].Width = (int)(dataGridViewGMES.Width * 0.6);
            dataGridViewGMES.RowHeadersWidth = dataGridViewGMES.Width - dataGridViewGMES.Columns[0].Width;
            
            iGridHeight = (int)((dataGridViewGMES.Height - dataGridViewGMES.ColumnHeadersHeight) / dataGridViewGMES.Rows.Count - 1);
            for (int i = 0; i < dataGridViewGMES.Rows.Count; i++)
            {
                dataGridViewGMES.Rows[i].Height = iGridHeight+1;
            }
            
            //MODEL INFO ----------------------  
            dataGridViewModel.AllowUserToResizeRows = false;
            int iTempSize = (int)(dataGridViewModel.Width  - 17);
            dataGridViewModel.Columns.Add("Col0", "MODEL");
            dataGridViewModel.Columns[0].Width = (int)(iTempSize * 0.23);
            dataGridViewModel.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewModel.Columns.Add("Col1", "P/N");
            dataGridViewModel.Columns[1].Width = (int)(iTempSize * 0.20);
            dataGridViewModel.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewModel.Columns.Add("Col2", "WIFI");
            dataGridViewModel.Columns[2].Width = (int)(iTempSize * 0.11);
            dataGridViewModel.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewModel.Columns.Add("Col3", "STID (Min)");
            dataGridViewModel.Columns[3].Width = (int)(iTempSize * 0.23);
            dataGridViewModel.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewModel.Columns.Add("Col3", "STID (Max)");
            dataGridViewModel.Columns[4].Width = (int)(iTempSize * 0.23);
            dataGridViewModel.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewModel.Rows.Add(200);
            for (int i = 0; i < dataGridViewModel.ColumnCount - 1; i++)
            {
                dataGridViewModel.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

            }

            // MTP200 LOSS            
            dataGridMtpLoss.Columns.Add("Col0", "Name");
            dataGridMtpLoss.Columns[0].Width = (int)((dataGridMtpLoss.Width - 5) * 0.5);
            dataGridMtpLoss.Columns[0].ReadOnly = true;
            dataGridMtpLoss.Columns.Add("Col1", "Offset");
            dataGridMtpLoss.Columns[1].Width = (int)(dataGridMtpLoss.Width - (int)((dataGridMtpLoss.Width - 5) * 0.5)) - 1;
            dataGridMtpLoss.AllowUserToResizeColumns = true;
            dataGridMtpLoss.Rows.Add((int)MTPLOSS.MAX);

            //MTP200 LOSS
            for (int i = 0; i < dataGridMtpLoss.Rows.Count; i++)
            {
                dataGridMtpLoss.Rows[i].Cells[0].Value = "LOSS" + i.ToString().PadLeft(2, '0');
            }

            // MELSEC ADDRESS LOSS            
            dataGridMelsecAddress.Columns.Add("Col0", "Name");
            dataGridMelsecAddress.Columns[0].Width = (int)((dataGridMelsecAddress.Width - 5) * 0.5);
            dataGridMelsecAddress.Columns[0].ReadOnly = true;
            dataGridMelsecAddress.Columns.Add("Col1", "Offset");
            dataGridMelsecAddress.Columns[1].Width = (int)(dataGridMelsecAddress.Width - (int)((dataGridMelsecAddress.Width - 5) * 0.5)) - 1;
            dataGridMelsecAddress.AllowUserToResizeColumns = true;
            dataGridMelsecAddress.Rows.Add((int)PLCADDRESS.MAX);

            //dataGridMelsecAddress LOSS
            for (int i = 0; i < dataGridMelsecAddress.Rows.Count; i++)
            {
                dataGridMelsecAddress.Rows[i].Cells[0].Value = "MELSEC" + i.ToString().PadLeft(2, '0');
            }

            // 케이블 커넥터  
            dataGridConnector.Columns.Add("Col0", "CABLE NAME");
            dataGridConnector.Columns[0].Width = (int)((dataGridConnector.Width) * 0.4);
            dataGridConnector.Columns.Add("Col1", "SPEC");
            dataGridConnector.Columns[1].Width = (int)((dataGridConnector.Width) * 0.2);
            dataGridConnector.Columns.Add("Col2", "USAGE");
            dataGridConnector.Columns[2].Width = (int)((dataGridConnector.Width) * 0.2);            
            dataGridConnector.Columns.Add("Col3", "REMAIN");
            dataGridConnector.Columns[3].Width = (int)((dataGridConnector.Width) * 0.2);
            dataGridConnector.AllowUserToResizeColumns = true;
            dataGridConnector.Rows.Add(5);

            dataGridConnector.ColumnHeadersHeight = (int)((dataGridConnector.Height) / (dataGridConnector.Rows.Count +1));
            for (int i = 0; i < dataGridConnector.Rows.Count; i++)
            {
                dataGridConnector.Rows[i].Cells[2].ReadOnly = true;
                dataGridConnector.Rows[i].Cells[2].Style.BackColor = Color.LightGray;
                dataGridConnector.Rows[i].Cells[3].ReadOnly = true;
                dataGridConnector.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                dataGridConnector.Rows[i].Height = (int)((dataGridConnector.Height) / (dataGridConnector.Rows.Count +1));
            }

            DgvHeaderNotSortable(dataGridPort1);
            DgvHeaderNotSortable(dataGridViewOracle);
            DgvHeaderNotSortable(dataGridViewGMES);           
            DgvHeaderNotSortable(dataGridConnector);
            DgvHeaderNotSortable(dataGridMtpLoss);
            DgvHeaderNotSortable(dataGridViewModel);
            DgvHeaderNotSortable(dataGridMelsecAddress);
            //LGEVH 202306 chkJobAuto 강제로 true 처리.
            CrossThreadIssue.ChangeChecked(chkJobAuto, true);
            CrossThreadIssue.AppendEnabled(chkJobAuto, false);
            ControlSave(chkJobAuto, "OPTION", "JOBAUTOMAPPING");
        }

        private void Load_Config()
        {
            string strColText = String.Empty;
            string strGetText = String.Empty;
                        
            ControlLoad(chkBoxSpec, "GMES", "SPECDOWN");
            ControlLoad(chkBoxMesAskPassword, "GMES", "MESASKPASSWORD");

            //ORACLE MES ------------------------------------------------------------           
            for (int i = 0; i < dataGridViewOracle.Rows.Count; i++)
            {
                switch (i)
                {
                    case 0: strColText = sOraProductionLine; break;
                    case 1: strColText = sOraProcessCode; break;
                    case 2: strColText = sOraPCID; break;
                    case 3: strColText = sOraServerIP; break;
                    case 4: strColText = sOraPort; break;
                    case 5: strColText = sOraServiceName; break;
                    case 6: strColText = sOraOOBCode; break;
                    case 7: strColText = sOraOOBFlag; break;

                }
                strGetText = DKLoggerConfig.LoadINI("ORACLE", strColText);
                dataGridViewOracle.Rows[i].Cells[0].Value = strGetText;
            }

            strGetText = DKLoggerConfig.LoadINI("ORACLE", sOraCallType);
            switch (strGetText)
            {
                case "PCB": radioBtnPCB.Checked = true; radioBtnFA.Checked = radioBtnOOB.Checked = false; break;
                case "FA": radioBtnFA.Checked = true; radioBtnPCB.Checked = radioBtnOOB.Checked = false; break;
                case "OOB": radioBtnOOB.Checked = true; radioBtnPCB.Checked = radioBtnFA.Checked = false; break;
                default: radioBtnFA.Checked = true; radioBtnPCB.Checked = radioBtnOOB.Checked = false; break;

            }

            //GMES --------------------------------------------------------------
            for (int i = 0; i < dataGridViewGMES.Rows.Count; i++)
            {
                switch (i)
                {
                    case 0: strColText = "SERVERIP"; break;
                    case 1: strColText = "LOCALIP"; break;
                    case 2: strColText = "PORT"; break;
                    case 3: strColText = "RETRY"; break;
                    case 4: strColText = "T3"; break;
                    case 5: strColText = "T6"; break;
                }
                strGetText = DKLoggerConfig.LoadINI("GMES", strColText);
                dataGridViewGMES.Rows[i].Cells[0].Value = strGetText;
               
            }

            for (int i = 0; i < dataGridPort1.Rows.Count; i++)
            {
                strColText = dataGridPort1.Rows[i].HeaderCell.Value.ToString();
                if (strColText.Contains("(CCM)"))
                    strColText = strColText.Replace("(CCM)", String.Empty);               

                strGetText = DKLoggerConfig.LoadINI("COMPORT", strColText);

                if (strColText.Contains("MTP120") && (string.IsNullOrEmpty(strGetText) || strGetText.Length < 10))
                    strGetText = "TCPIP0::192.168.100.252::inst0::INSTR";

                dataGridPort1.Rows[i].Cells[0].Value = strGetText;               
            }

            for (int i = 0; i < dataGridMtpLoss.Rows.Count; i++)
            {
                strColText = dataGridMtpLoss.Rows[i].Cells[0].Value.ToString();
                strGetText = DKLoggerConfig.LoadINI("MTPLOSS", strColText);
                dataGridMtpLoss.Rows[i].Cells[1].Value = strGetText;
            }

            for (int i = 0; i < dataGridMelsecAddress.Rows.Count; i++)
            {
                strColText = dataGridMelsecAddress.Rows[i].Cells[0].Value.ToString();
                strGetText = DKLoggerConfig.LoadINI("PLC_ADDRESS", strColText);
                dataGridMelsecAddress.Rows[i].Cells[1].Value = strGetText;
            }

            ControlLoad(chkBoxSpec, "GMES", "SPECDOWN");
            ControlLoad(chkBoxMesAskPassword, "GMES", "MESASKPASSWORD");
            
            ControlLoad(chkUsePin, "DIOPIN", DIOPIN.PINUSE.ToString());
            ControlLoad(chkBoxIn1, "DIOPIN", DIOPIN.IN1.ToString());
            ControlLoad(chkBoxIn2, "DIOPIN", DIOPIN.IN2.ToString());
            ControlLoad(chkBoxIn3,   "DIOPIN", DIOPIN.IN3.ToString());
            ControlLoad(chkBoxBub1,     "DIOPIN", DIOPIN.BUB1.ToString());
            ControlLoad(chkBoxBub2,     "DIOPIN", DIOPIN.BUB2.ToString());
            ControlLoad(chkBoxManual1,    "DIOPIN", DIOPIN.EXTERNAL.ToString());
            ControlLoad(chkBoxManual3,    "DIOPIN", DIOPIN.MANUAL3.ToString());
            ControlLoad(chkBoxSetIn,    "DIOPIN", DIOPIN.SETIN.ToString());

            ControlLoad(chkUsePLC, "OPTION", "USEPLC");
            ControlLoad(chkBox5515c, "OPTION", dev5515C);
            ControlLoad(chkBoxMTP200, "OPTION", devMTP200);
            ControlLoad(chkBoxTC3000, "OPTION", devTC3000);
            ControlLoad(chkBoxAudioSelector, "OPTION", devAudio);
            ControlLoad(chkBoxADC, "OPTION", devADC);
            ControlLoad(chkBox34410A, "OPTION", dev34410a);
            ControlLoad(chkBoxOdaPwr, "OPTION", devOdaPower);
            ControlLoad(chkBoxMelsec, "OPTION", devMelsec);

            ControlLoad(chkBoxKEITHLEY, "OPTION", devKEITHLEY);

            ControlLoad(chkBox232Scanner, "OPTION", dev232Scan);
            ControlLoad(chkBoxPCAN, "OPTION", devPcan);
            ControlLoad(chkBoxVector, "OPTION", devVector);
            ControlLoad(chkBoxMTP120A, "OPTION", devMTP120A);

            ControlLoad(chkBoxStepCheckMode, "OPTION", devStepCheckMode);
            ControlLoad(chkBoxMSize, "OPTION", "MSIZE");
            
            ControlLoad(chkBoxBefore, "OPTION", "BEFORESTART");
            ControlLoad(chkBoxOK,  "OPTION", "RESULTOK");
            ControlLoad(chkBoxNG,  "OPTION", "RESULTNG");
            ControlLoad(chkBoxCHK, "OPTION", "RESULTCHK");
            ControlLoad(chkBoxUsrSTOP, "OPTION", "USERSTOP");
            ControlLoad(chkBoxEmpty, "OPTION", "RESULTEMPTY");
            ControlLoad(chkBoxMes, "OPTION", "RESULTMES");
            ControlLoad(chkBoxEject, "OPTION", "RESULTEJECT");
            ControlLoad(chkBoxError, "OPTION", "RESULTERROR");

            ControlLoad(chkBoxDLLGate, "OPTION", devDLLGate);
            


            TextLoad(txtWipLen, "OPTION", "WIPLENTH");
            TextLoad(txtSubIdLen, "OPTION", "SUBIDLENTH");

            TextLoad(txtWarning, "OPTION", "WARNING");
            if (txtWarning.Text.Equals("0"))
            {
                txtWarning.Text = "90";
            }
            ControlLoad(chkStartBarcode, "OPTION", "STARTBARCODE");
            ControlLoad(chkUseBarcode, "OPTION", "USEBARCODE");
            ControlLoad(chkJobAuto, "OPTION", "JOBAUTOMAPPING");
            ControlLoad(chkUseBarcodeOOB, "OPTION", "OOBBARCODETYPE");
            ControlLoad(chkUseSubId, "OPTION", "USESUBID");

            ControlLoad(chkChangeJobPassword, "OPTION", "CHANGEJOBPASSWORD");

            ControlLoad(chkBoxAskExit, "OPTION", "ASKEXIT");
            ControlLoad(chkBoxNormalScreen, "OPTION", "DEFAULTSCREEN");
            ControlLoad(chkStopMesComplete, "OPTION", "MESCOMPLETEONSTOP");
            ControlLoad(chkOnlyOKGmes, "OPTION", "ONLYOKGMES");
            ControlLoad(chkUseZebra, "OPTION", "USEZEBRASCANNER");

            ControlLoad(chkDoncareOOBCode, "OPTION", "DONTCAREOOBCODE");

            ControlLoad(chkForATCO, "OPTION", "FORATCO");
            ControlLoad(chkNoBinLog, "OPTION", "NOBINLOG");
            
            TextLoad(txtStation, "OPTION", "ATCOSTATION");
                        
            ControlLoad(chkBoxCountEnable, "OPTION", "VIEWCOUNT");
            ControlLoad(chkBoxPassRate, "OPTION", "PASSRATE");
            ControlLoad(chkBoxChangeTime, "OPTION", "COUNTRESETTIME");
            try
            {
                nmHour.Value = decimal.Parse(DKLoggerConfig.LoadINI("OPTION", "COUNTRESETHOUR"));
                nmMin.Value = decimal.Parse(DKLoggerConfig.LoadINI("OPTION", "COUNTRESETMIN"));
            }
            catch
            {
                nmHour.Value = nmMin.Value = 0;
            }
            
            ControlLoad(chkUseRetry, "OPTION", "INSPECTIONRETRY");
            TextLoad(txtRetryCount, "OPTION", "RETRYVALUE");


            if (!chkUseBarcode.Checked )
            {
                chkStartBarcode.Checked = chkUseZebra.Checked = chkUseSubId.Checked = false;
                chkStartBarcode.Enabled = chkUseZebra.Enabled = chkUseSubId.Enabled = false;
            }
            else
            {
                chkStartBarcode.Enabled = chkUseZebra.Enabled = chkUseSubId.Enabled = true;
            }

            TextLoad(txtMyName, "UPDATE", "MYNAME");
            TextLoad(txtUpdateIP, "UPDATE", "IP");


            string tmpStr = DKLoggerConfig.LoadINI("OPTION", "PRIMARY");

            if (tmpStr.Equals("ORACLE"))
            {
                tabMesPannel.SelectTab("tabPage2");
            }
            else
            {
                tabMesPannel.SelectTab("tabPage1");
            }

            //AMS         
            string tmpAmsStr1 = DKLoggerConfig.LoadINI("AMS", "TIMEOUT");
            string tmpAmsStr2 = DKLoggerConfig.LoadINI("AMS", "RETRY");            

            if (tmpAmsStr1.Equals("0"))
                txtAMStimeout.Text = "10";
            else
                txtAMStimeout.Text = tmpAmsStr1;

            if (tmpAmsStr2.Equals("0"))
                txtAMSretry.Text = "3";
            else
                txtAMSretry.Text = tmpAmsStr2;

            //CSMES
            TextLoad(txtOSI_ID, "OSI", "ID");
            TextLoad(txtOSI_PassWord, "OSI", "PASSWORD");
            ControlLoad(chkUseOSIMES, "OSI", "UseOSIMES");
            TextLoad(txtOSI_SiteCode, "OSI", "SITECODE");
            TextLoad(txtOSI_LocalIP, "OSI", "CSMESLOCALIP");

            TextLoad(txtWHCode, "OSI", "WHCODE");
            ControlRDLoad("OSI", "WHCODE");

            ConnectorCountLoad();
            CountLoad();

            CheckDefaultValue(); //디폴트 값 체크확인
            LoadModelFile();
        }

        private void LoadModelFile()
        {
            dataGridViewModel.Rows.Clear();
            dataGridViewModel.Rows.Add(300);

            List<TBLMODEL> tmpList = new List<TBLMODEL>();

            bool bLoadModel = DKLoggerConfig.LoadModelFile(ref tmpList);

            if (!bLoadModel) return;

            try
            {
                for (int i = 0; i < tmpList.Count; i++)
                {
                    dataGridViewModel[0, i].Value = tmpList[i].NAME;
                    dataGridViewModel[1, i].Value = tmpList[i].PN;
                    dataGridViewModel[2, i].Value = tmpList[i].BTWIFI;
                    dataGridViewModel[3, i].Value = tmpList[i].StidMin;
                    dataGridViewModel[4, i].Value = tmpList[i].StidMax;
                }
            }
            catch 
            {
 
            }
            

        }

        private void CheckDefaultValue()
        {
            if (txtWipLen.Text.Equals("0")) txtWipLen.Text = "15";
            PinEnableControl();
        }

        private bool CheckDuplicateValue()
        {
            //장치 사용 검사.
            if (chkBox5515c.Checked)
            {
                if (dataGridPort1.Rows[2].Cells[0].Value != null &&
                            dataGridPort1.Rows[2].HeaderCell.Value != null)
                {
                    string comportname2 = dataGridPort1.Rows[2].Cells[0].Value.ToString();
                    if (comportname2.Length < 10)
                    {
                        MessageBox.Show("Please, Write the 5515c GPIB address!"); return false;
                    }
                }
            }

            if (chkBoxKEITHLEY.Checked)
            {
                if (dataGridPort1.Rows[13].Cells[0].Value != null &&
                            dataGridPort1.Rows[13].HeaderCell.Value != null)
                {
                    string comportname12 = dataGridPort1.Rows[13].Cells[0].Value.ToString();
                    if (comportname12.Length < 10)
                    {
                        MessageBox.Show("Please, Write the KEITHLEY GPIB address!"); return false;
                    }
                }
            }

            if (chkBoxMTP200.Checked)
            {
                if (dataGridPort1.Rows[3].Cells[0].Value != null &&
                            dataGridPort1.Rows[3].HeaderCell.Value != null)
                {
                    string comportname2 = dataGridPort1.Rows[3].Cells[0].Value.ToString();
                    if (comportname2.Length < 10)
                    {
                        MessageBox.Show("Please, Write the MTP200 GPIB address!"); return false;
                    }
                }
            }

            if (chkBoxMTP120A.Checked)
            {
                if (dataGridPort1.Rows[14].Cells[0].Value != null &&
                            dataGridPort1.Rows[14].HeaderCell.Value != null)
                {
                    string comportname2 = dataGridPort1.Rows[14].Cells[0].Value.ToString();
                    if (comportname2.Length < 8)
                    {
                        MessageBox.Show("Please, Write the MTP120A TCP address!"); return false;
                    }
                }
            }

            if (chkBox34410A.Checked)
            {
                if (dataGridPort1.Rows[11].Cells[0].Value != null &&
                            dataGridPort1.Rows[11].HeaderCell.Value != null)
                {
                    string comportname2 = dataGridPort1.Rows[11].Cells[0].Value.ToString();
                    if (comportname2.Length < 10)
                    {
                        MessageBox.Show("Please, Write the 34410A USB address!"); return false;
                    }
                }
            }

            //port 중복검사.
            for (int i = 0; i < dataGridPort1.Rows.Count; i++)
            {

                if (dataGridPort1.Rows[i].Cells[0].Value != null &&
                    dataGridPort1.Rows[i].HeaderCell.Value != null)
                {
                    string comportname = dataGridPort1.Rows[i].Cells[0].Value.ToString();

                    for (int j = i + 1; j < dataGridPort1.Rows.Count; j++)
                    {
                        if (dataGridPort1.Rows[j].Cells[0].Value != null &&
                            dataGridPort1.Rows[j].HeaderCell.Value != null)
                        {
                            string comportname2 = dataGridPort1.Rows[j].Cells[0].Value.ToString();

                            if (comportname.Equals(comportname2))
                            {
                                MessageBox.Show("Comport or Address is Duplicated! Please, Check and Try Again.");
                                return false;
                            }
                        }

                    }

                }
            }

            return true;
        }

        private void Save_Config()
        {
            if (!CheckDuplicateValue()) { return; }    
            
            string strColText = String.Empty;
            string strSetText = String.Empty;
            string strSetTitle = String.Empty;
            string tmpCheck = "OFF";

            //ORACLE MES -----------------------------------------------------------------------------

            for (int i = 0; i < dataGridViewOracle.Rows.Count; i++)
            {
                if (dataGridViewOracle.Rows[i].Cells[0].Value != null)
                {
                    strSetText = dataGridViewOracle.Rows[i].Cells[0].Value.ToString();
                    switch (i)
                    {
                        case 0: DKLoggerConfig.SaveINI("ORACLE", sOraProductionLine, strSetText); break;
                        case 1: DKLoggerConfig.SaveINI("ORACLE", sOraProcessCode, strSetText); break;
                        case 2: DKLoggerConfig.SaveINI("ORACLE", sOraPCID, strSetText); break;
                        case 3: DKLoggerConfig.SaveINI("ORACLE", sOraServerIP, strSetText); break;
                        case 4: DKLoggerConfig.SaveINI("ORACLE", sOraPort, strSetText); break;
                        case 5: DKLoggerConfig.SaveINI("ORACLE", sOraServiceName, strSetText); break;
                        case 6: DKLoggerConfig.SaveINI("ORACLE", sOraOOBCode, strSetText); break;
                        case 7: DKLoggerConfig.SaveINI("ORACLE", sOraOOBFlag, strSetText); break;
                    }
                }
            }

            if (radioBtnPCB.Checked) DKLoggerConfig.SaveINI("ORACLE", sOraCallType, "PCB");
            if (radioBtnFA.Checked) DKLoggerConfig.SaveINI("ORACLE", sOraCallType, "FA");
            if (radioBtnOOB.Checked) DKLoggerConfig.SaveINI("ORACLE", sOraCallType, "OOB");   
            

            //GMES --------------------------------------------------------------------------------
            for (int i = 0; i < dataGridViewGMES.Rows.Count; i++ )
            {
                if (dataGridViewGMES.Rows[i].Cells[0].Value != null)
                {
                    strSetText = dataGridViewGMES.Rows[i].Cells[0].Value.ToString();
                    switch (i)
                    {
                        case 0: DKLoggerConfig.SaveINI("GMES", "SERVERIP", strSetText); break;
                        case 1: DKLoggerConfig.SaveINI("GMES", "LOCALIP", strSetText); break;
                        case 2: DKLoggerConfig.SaveINI("GMES", "PORT", strSetText); break;
                        case 3: DKLoggerConfig.SaveINI("GMES", "RETRY", strSetText); break;
                        case 4: DKLoggerConfig.SaveINI("GMES", "T3", strSetText); break;
                        case 5: DKLoggerConfig.SaveINI("GMES", "T6", strSetText); break;
                    }
                }
            }

            tmpCheck = "OFF";
            if (chkBoxSpec.Checked) tmpCheck = "ON";

            DKLoggerConfig.SaveINI("GMES", "SPECDOWN", tmpCheck);

            tmpCheck = "OFF";
            if (chkBoxMesAskPassword.Checked) tmpCheck = "ON";

            DKLoggerConfig.SaveINI("GMES", "MESASKPASSWORD", tmpCheck);
            

            for(int i = 0; i < dataGridPort1.Rows.Count; i++)
            {
                if (dataGridPort1.Rows[i].Cells[0].Value != null &&
                    dataGridPort1.Rows[i].HeaderCell.Value != null)
                {
                    strColText = dataGridPort1.Rows[i].HeaderCell.Value.ToString();
                    strSetText = dataGridPort1.Rows[i].Cells[0].Value.ToString();
                    if (strColText.Contains("(CCM)"))
                    {
                        strColText = strColText.Replace("(CCM)", String.Empty);                        
                    }
                    DKLoggerConfig.SaveINI("COMPORT", strColText, strSetText);                    
                }
                else
                {         
                    if (i == 0) 
                    { MessageBox.Show(devDIO + " Port Check and Try Again.");
                      Load_Config(); return;
                    }

                    if (i == 1) 
                    { MessageBox.Show(devSET + " Port Check and Try Again.");
                      Load_Config(); return;
                    }
                }
            }

            for (int i = 0; i < dataGridMtpLoss.Rows.Count; i++)
            {
                strSetTitle = dataGridMtpLoss.Rows[i].Cells[0].Value.ToString();

                if (dataGridMtpLoss.Rows[i].Cells[1].Value != null)                    
                    strSetText  = dataGridMtpLoss.Rows[i].Cells[1].Value.ToString();   
                else   
                    strSetText = dataGridMtpLoss.Rows[i].Cells[1].Value.ToString();
               
                DKLoggerConfig.SaveINI("MTPLOSS", strSetTitle, strSetText);
            }

            for (int i = 0; i < dataGridMelsecAddress.Rows.Count; i++)
            {
                strSetTitle = dataGridMelsecAddress.Rows[i].Cells[0].Value.ToString();

                if (dataGridMelsecAddress.Rows[i].Cells[1].Value != null)
                    strSetText = dataGridMelsecAddress.Rows[i].Cells[1].Value.ToString();
                else
                    strSetText = dataGridMelsecAddress.Rows[i].Cells[1].Value.ToString();

                DKLoggerConfig.SaveINI("PLC_ADDRESS", strSetTitle, strSetText);
            }
            
            //PINUSED, START, IN1, IN2, IN3, BUB1, BUB2, MANUAL1, MANUAL3, SETIN, STOP, END
            ControlSave(chkUsePin, "DIOPIN", DIOPIN.PINUSE.ToString());
            ControlSave(chkBoxIn1, "DIOPIN", DIOPIN.IN1.ToString());
            ControlSave(chkBoxIn2, "DIOPIN", DIOPIN.IN2.ToString());
            ControlSave(chkBoxIn3, "DIOPIN", DIOPIN.IN3.ToString());
            ControlSave(chkBoxBub1,"DIOPIN", DIOPIN.BUB1.ToString());
            ControlSave(chkBoxBub2,"DIOPIN", DIOPIN.BUB2.ToString());
            ControlSave(chkBoxManual1, "DIOPIN", DIOPIN.EXTERNAL.ToString());
            ControlSave(chkBoxManual3, "DIOPIN", DIOPIN.MANUAL3.ToString());
            ControlSave(chkBoxSetIn,   "DIOPIN", DIOPIN.SETIN.ToString());           
            DKLoggerConfig.SaveINI("DIOPIN", DIOPIN.STOP.ToString(), "OFF"); //STOp 버튼은 강제 설정            
            DKLoggerConfig.SaveINI("DIOPIN", DIOPIN.START.ToString(), "ON"); //START 버튼은 강제 설정            

            ControlSave(chkUsePLC, "OPTION", "USEPLC");
            ControlSave(chkBox5515c, "OPTION", dev5515C);
            ControlSave(chkBoxTC3000, "OPTION", devTC3000);
            ControlSave(chkBoxMTP200, "OPTION", devMTP200);
            ControlSave(chkBox232Scanner, "OPTION", dev232Scan);
            ControlSave(chkBoxPCAN, "OPTION", devPcan);
            ControlSave(chkBoxVector, "OPTION", devVector);
            ControlSave(chkBoxAudioSelector, "OPTION", devAudio);
            ControlSave(chkBoxADC, "OPTION", devADC);
            ControlSave(chkBox34410A, "OPTION", dev34410a);
            ControlSave(chkBoxOdaPwr, "OPTION", devOdaPower);
            ControlSave(chkBoxKEITHLEY, "OPTION", devKEITHLEY);
            ControlSave(chkBoxMelsec, "OPTION", devMelsec);
            ControlSave(chkBoxMTP120A, "OPTION", devMTP120A);

            ControlSave(chkBoxStepCheckMode, "OPTION", devStepCheckMode);
            ControlSave(chkBoxMSize, "OPTION", "MSIZE");

            ControlSave(chkBoxBefore, "OPTION", "BEFORESTART");
            ControlSave(chkBoxOK,    "OPTION", "RESULTOK");
            ControlSave(chkBoxNG,    "OPTION", "RESULTNG");
            ControlSave(chkBoxCHK,   "OPTION", "RESULTCHK");
            ControlSave(chkBoxUsrSTOP, "OPTION",   "USERSTOP");
            ControlSave(chkBoxEmpty, "OPTION", "RESULTEMPTY");
            ControlSave(chkBoxMes, "OPTION", "RESULTMES");
            ControlSave(chkBoxEject, "OPTION", "RESULTEJECT");
            ControlSave(chkBoxError, "OPTION", "RESULTERROR");

            ControlSave(chkBoxDLLGate, "OPTION", devDLLGate);

            TextSave(txtWipLen, "OPTION", "WIPLENTH");
            TextSave(txtSubIdLen, "OPTION", "SUBIDLENTH");
            TextSave(txtWarning, "OPTION", "WARNING");
            
            
            ControlSave(chkStartBarcode, "OPTION", "STARTBARCODE");
            ControlSave(chkUseBarcode, "OPTION", "USEBARCODE");
            ControlSave(chkJobAuto, "OPTION", "JOBAUTOMAPPING");
            ControlSave(chkUseBarcodeOOB, "OPTION", "OOBBARCODETYPE");
            ControlSave(chkUseSubId, "OPTION", "USESUBID");

            ControlSave(chkChangeJobPassword, "OPTION", "CHANGEJOBPASSWORD");

            ControlSave(chkBoxAskExit, "OPTION", "ASKEXIT");
            ControlSave(chkBoxNormalScreen, "OPTION", "DEFAULTSCREEN");
            ControlSave(chkStopMesComplete, "OPTION", "MESCOMPLETEONSTOP");
            ControlSave(chkOnlyOKGmes, "OPTION", "ONLYOKGMES");
            ControlSave(chkUseZebra, "OPTION", "USEZEBRASCANNER");

            ControlSave(chkDoncareOOBCode, "OPTION", "DONTCAREOOBCODE");            

            ControlSave(chkForATCO, "OPTION", "FORATCO");
            ControlSave(chkNoBinLog, "OPTION", "NOBINLOG");

            TextSave(txtStation, "OPTION", "ATCOSTATION");

            ControlSave(chkBoxCountEnable, "OPTION", "VIEWCOUNT");
            ControlSave(chkBoxPassRate, "OPTION", "PASSRATE");
            ControlSave(chkBoxChangeTime, "OPTION", "COUNTRESETTIME");

            try
            {
                string strResetHour = nmHour.Value.ToString();
                DKLoggerConfig.SaveINI("OPTION", "COUNTRESETHOUR", strResetHour);
                string strResetMin = nmMin.Value.ToString();
                DKLoggerConfig.SaveINI("OPTION", "COUNTRESETMIN", strResetMin);
            }
            catch {
                DKLoggerConfig.SaveINI("OPTION", "COUNTRESETMIN", "0");
                DKLoggerConfig.SaveINI("OPTION", "COUNTRESETMIN", "0");
            }

            ControlSave(chkUseRetry, "OPTION", "INSPECTIONRETRY");
            TextSave(txtRetryCount, "OPTION", "RETRYVALUE");
                      
            TextSave(txtMyName, "UPDATE", "MYNAME");
            TextSave(txtUpdateIP, "UPDATE", "IP");


            //AMS            
            TextSave(txtAMStimeout, "AMS", "TIMEOUT");
            TextSave(txtAMSretry, "AMS", "RETRY");

            //CSMES
            TextSave(txtOSI_ID, "OSI", "ID");
            TextSave(txtOSI_PassWord, "OSI", "PASSWORD");
            ControlSave(chkUseOSIMES, "OSI", "UseOSIMES");
            TextSave(txtOSI_SiteCode, "OSI", "SITECODE");
            TextSave(txtOSI_LocalIP, "OSI", "CSMESLOCALIP");

            if (chkUseOSIMES.Checked)
            {
                string strMsg = string.Format("Check MES LOCAL IP {0}", txtOSI_LocalIP.Text.Trim());
                if (!CheckCsMesLocalIp(txtOSI_LocalIP.Text.Trim()))
                {
                    MessageBox.Show(strMsg);
                    txtOSI_LocalIP.Focus();
                }
            }

            TextSave(txtWHCode, "OSI", "WHCODE");

            DioSignalCommandSave();
            ConnectorCountSave();
            CountSave();
            SaveModelList();

        }
        private bool CheckCsMesLocalIp(string LocalIp)
        {
            bool isFind = false;
            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
                if (LocalIp == addr[i].ToString())
                    isFind = true;
            }

            return isFind;
        }
        private void SaveModelList()
        {               
            for (int i = 0; i < dataGridViewModel.Rows.Count; i++)
            {
                string[] strSetText = new string[dataGridViewModel.Columns.Count];
                for (int j = 0; j < dataGridViewModel.Columns.Count; j++)
                {
                    if (dataGridViewModel.Rows[i].Cells[j].Value != null && dataGridViewModel.Rows[i].Cells[j].Value.ToString().Length > 0)
                    {
                        strSetText[j] = dataGridViewModel.Rows[i].Cells[j].Value.ToString();
                        if(j == 2)
                        {
                            switch (strSetText[j])
                            {
                                case "0":
                                case "n":
                                case "N": strSetText[j] = "N"; break;
                                case "1":
                                case "y":
                                case "Y": strSetText[j] = "Y"; break;

                                default: strSetText[j] = "N"; break;
                            }            
                       }

                        if (j == 3 || j == 4)
                        {
                            int iRange = 0;
                            try
                            {
                                iRange = int.Parse(strSetText[j]);
                            }
                            catch 
                            {
                                MessageBox.Show("CAN NOT SAVE. STID ERROR. CHECK VALUE");
                                return;
                            }

                        }
                    }
                    else
                    {
                        LoadModelFile();
                        return;
                    }
                }

                switch (i)
                {
                    case 0:  DKLoggerConfig.SaveModelFile(strSetText, true); break;
                    default: DKLoggerConfig.SaveModelFile(strSetText, false); break;
                }
            }

            LoadModelFile();
        }

        private void ConnectorCountClear(int i)
        {           
            string strCableName = String.Empty;
            string strCableSpec = String.Empty;
            if (dataGridConnector.Rows[i].Cells[0].Value != null)
            {
                strCableName = dataGridConnector.Rows[i].Cells[0].Value.ToString();
            }
            else{ return; }
            if (dataGridConnector.Rows[i].Cells[1].Value != null) strCableSpec = dataGridConnector.Rows[i].Cells[1].Value.ToString();

            dataGridConnector.Rows[i].Cells[2].Value = "0";

            try
            {
                if (strCableName.Length > 1 && strCableSpec.Length > 0)
                {
                    dataGridConnector.Rows[i].Cells[3].Value = strCableSpec;
                }
            }
            catch (System.Exception ex)
            {
                string strEx = ex.Message;
            }
            
        }

        private void CountLoad()
        {
            
            txtPass.Text  = DKLoggerConfig.LoadINI("OPTION", "COUNTPASS");
            txtFail.Text  = DKLoggerConfig.LoadINI("OPTION", "COUNTFAIL");
            txtTotal.Text = DKLoggerConfig.LoadINI("OPTION", "COUNTTOTAL");

        }
        private void ConnectorCountLoad()
        {
            //CABEL COUNT
            string[] strCableName = new string[5];
            string[] strCableSpec = new string[5];
            string[] strCableUsage = new string[5];    

            for (int i = 0; i < strCableName.Length; i++)
            {
                strCableName[i] = String.Empty;
                strCableSpec[i] = String.Empty;
                strCableUsage[i] = String.Empty;
                
                strCableName[i] = DKLoggerConfig.LoadINI("CABLECOUNT", "CABLENAME" + i.ToString());
                strCableSpec[i] = DKLoggerConfig.LoadINI("CABLECOUNT", "CABLESPEC" + i.ToString());
                strCableUsage[i] = DKLoggerConfig.LoadINI("CABLECOUNT", "CABLEUSAGE" + i.ToString());

                if (!strCableName[i].Equals("0"))
                {
                    dataGridConnector.Rows[i].Cells[0].Value = strCableName[i];
                    dataGridConnector.Rows[i].Cells[1].Value = strCableSpec[i];
                    dataGridConnector.Rows[i].Cells[2].Value = strCableUsage[i];
                }
                else
                {
                    dataGridConnector.Rows[i].Cells[0].Value = String.Empty;
                    dataGridConnector.Rows[i].Cells[1].Value = "0";
                    dataGridConnector.Rows[i].Cells[2].Value = "0";
                }

                try
                {
                    if (strCableName[i].Length > 1 && strCableSpec[i].Length > 0
                        && strCableUsage[i].Length > 0)
                    {
                        dataGridConnector.Rows[i].Cells[3].Value = (int.Parse(strCableSpec[i]) - int.Parse(strCableUsage[i])).ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    string strEx = ex.Message;
                }
                

            }
        }

        private void CountSave()
        {
            DKLoggerConfig.SaveINI("OPTION", "COUNTPASS", txtPass.Text);
            DKLoggerConfig.SaveINI("OPTION", "COUNTFAIL", txtFail.Text);
            DKLoggerConfig.SaveINI("OPTION", "COUNTTOTAL", txtTotal.Text);
        }

        private void ConnectorCountSave()
        {
            //CABEL COUNT
            string[] strCableName = new string[5];
            string[] strCableSpec = new string[5];
            string[] strCableUsage = new string[5];

            for (int i = 0; i < strCableName.Length; i++)
            {
                strCableName[i]    = String.Empty;
                strCableSpec[i]    = String.Empty;
                strCableUsage[i]   = String.Empty;                

                if (dataGridConnector.Rows[i].Cells[0].Value != null) { strCableName[i]   = dataGridConnector.Rows[i].Cells[0].Value.ToString(); }
                if (dataGridConnector.Rows[i].Cells[1].Value != null) { strCableSpec[i]   = dataGridConnector.Rows[i].Cells[1].Value.ToString(); }
                if (dataGridConnector.Rows[i].Cells[2].Value != null) { strCableUsage[i]  = dataGridConnector.Rows[i].Cells[2].Value.ToString(); }
                

                DKLoggerConfig.SaveINI("CABLECOUNT", "CABLENAME"  + i.ToString(), strCableName[i]);
                DKLoggerConfig.SaveINI("CABLECOUNT", "CABLESPEC"  + i.ToString(), strCableSpec[i]);
                DKLoggerConfig.SaveINI("CABLECOUNT", "CABLEUSAGE" + i.ToString(), strCableUsage[i]);
                
            }
            ConnectorCountLoad();
        }

        private void TextSave(TextBox cbox, string strTitle, string strName)
        {
            if (cbox == txtWarning)
            {
                try
                {
                    int iWarn = int.Parse(txtWarning.Text);
                    if (iWarn < 0 || iWarn > 100)
                    {
                        txtWarning.Text = "90";
                    }
                }
                catch (System.Exception ex)
                {
                    txtWarning.Text = "90";    
                    string strEx = ex.Message;
                }
            }            

            string tmpString = "0";
            if (cbox.Text.ToString() != null && cbox.Text.Length > 0) tmpString = cbox.Text.ToString();
            DKLoggerConfig.SaveINI(strTitle, strName, tmpString);
        }
        private void TextLoad(TextBox cbox, string strTitle, string strName)
        {
            cbox.Text = DKLoggerConfig.LoadINI(strTitle, strName);            
        }
        private void ControlSave(CheckBox cbox, string strTitle, string strName)
        {
            string tmpCheck = "OFF";
            if (cbox.Checked) tmpCheck = "ON";
            DKLoggerConfig.SaveINI(strTitle, strName, tmpCheck);
        }

        private void ControlLoad(CheckBox cbox, string strTitle, string strName)
        {
            string strGetText = DKLoggerConfig.LoadINI(strTitle, strName);
            if (strGetText.Equals("ON"))
            { cbox.Checked = true;}
            else{cbox.Checked = false;}
        }

#endregion

#region 버튼관련
                
        private void btnExit_Click(object sender, EventArgs e)
        {
            ViewSecContorl(false);
            //CSMES
            if (STEPMANAGER_VALUE.bUseOSIMES != DKLoggerConfig.LoadINI("OSI", "UseOSIMES").Equals("ON"))
            {
                STEPMANAGER_VALUE.bUseOSIMES = DKLoggerConfig.LoadINI("OSI", "UseOSIMES").Equals("ON");
                MessageBox.Show("MES changed." + Environment.NewLine + "Please restart program");
            }
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save_Config();
        }

#endregion

        private void FrmConfig_Load(object sender, EventArgs e)
        {
     
        }

        private void PinEnableControl()
        {
            groupBox1.Enabled = chkUsePLC.Enabled = chkUsePin.Checked;
            if (!chkUsePin.Checked) chkUsePLC.Checked = false;
        }
        private void chkUsePin_CheckedChanged(object sender, EventArgs e)
        {
            PinEnableControl();
        }

        private void ContextMenuViewTool()
        {
            if (pAddressMenu == null) return;
            for (int i = 0; i < mnuAddress.Length; i++)
            {
                pAddressMenu.Items.Add(mnuAddress[i]);                
            }
        }

        private void ContextMenuViewTool2()
        {
            if (pComportMenu == null) return;
            for (int i = 0; i < mnuComport.Length; i++)
            {
                pComportMenu.Items.Add(mnuComport[i]);
            }
        }

        private void ContextMenuViewToolUsb()
        {
            if (pAddressMenuUsb == null) return;
            for (int i = 0; i < mnuAddressUsb.Length; i++)
            {
                pAddressMenuUsb.Items.Add(mnuAddressUsb[i]);
            }
        }

        private void ContextMenuViewToolTcp()
        {
            if (pAddressMenuTcp == null) return;
            for (int i = 0; i < mnuAddressTcp.Length; i++)
            {
                pAddressMenuTcp.Items.Add(mnuAddressTcp[i]);
            }
        }
        private void GridClickEventHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem tempItem = sender as ToolStripMenuItem;
            dataGridPort1.CurrentCell.Value = tempItem.Text.ToString();
        }

        private void FindResources()
        {
            try
            {
                string[] resources = ResourceManager.GetLocalManager().FindResources("GPIB?*INSTR");
                //string[] resources = ResourceManager.GetLocalManager().FindResources("GPIB?*");
               
                if (resources.Length == 0)
                {
                    return;
                }
                else
                {
                    mnuAddress = new ToolStripMenuItem[resources.Length];
                }

                for(int i = 0; i < resources.Length; i++)
                {
                    mnuAddress[i] = new ToolStripMenuItem(resources[i]);
                    mnuAddress[i].Click += new EventHandler(GridClickEventHandler);
                }
                ContextMenuViewTool();
                
            }
            catch (VisaException)
            {
                // Don't do anything
            }
            catch (Exception ex)
            {
                string strEx = ex.Message;
                
            }
        }

        private void FindComports()
        {
            try
            {

                System.IO.Ports.SerialPort _serialPort;

                List<string> lstComports = new List<string>();

                _serialPort = new System.IO.Ports.SerialPort();
                foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
                {
                    lstComports.Add(s.Replace("COM", String.Empty));
                }

                if (lstComports.Count == 0)
                {
                    return;
                }
                else
                {
                    mnuComport = new ToolStripMenuItem[lstComports.Count];

                    for (int i = 0; i < lstComports.Count; i++)
                    {
                        mnuComport[i] = new ToolStripMenuItem(lstComports[i]);
                        mnuComport[i].Click += new EventHandler(GridClickEventHandler);
                    }
                    ContextMenuViewTool2();
                }

            }
           
            catch {}
        }

        private void FindResourcesUsb()
        {
            try
            {
                string[] resourcesUsb = ResourceManager.GetLocalManager().FindResources("USB?*INSTR"); //USB 용

                if (resourcesUsb.Length == 0)
                {
                    return;
                }
                else
                {
                    mnuAddressUsb = new ToolStripMenuItem[resourcesUsb.Length];
                }

                for (int i = 0; i < resourcesUsb.Length; i++)
                {
                    mnuAddressUsb[i] = new ToolStripMenuItem(resourcesUsb[i]);
                    mnuAddressUsb[i].Click += new EventHandler(GridClickEventHandler);
                }

                ContextMenuViewToolUsb();

            }
            catch (VisaException)
            {
                // Don't do anything
            }
            catch (Exception ex)
            {
                string strEx = ex.Message;

            }
        }

        private void FindResourcesTcp()
        {
            try
            {
                string[] resourcesTcp = ResourceManager.GetLocalManager().FindResources("TCPIP?*INSTR"); //TCP 용                

                if (resourcesTcp.Length == 0)
                {
                    return;
                }
                else
                {
                    mnuAddressTcp = new ToolStripMenuItem[resourcesTcp.Length];
                }

                for (int i = 0; i < resourcesTcp.Length; i++)
                {
                    mnuAddressTcp[i] = new ToolStripMenuItem(resourcesTcp[i]);
                    mnuAddressTcp[i].Click += new EventHandler(GridClickEventHandler);
                }

                ContextMenuViewToolTcp();

            }
            catch (VisaException)
            {
                // Don't do anything
            }
            catch (Exception ex)
            {
                string strEx = ex.Message;

            }
        }
        private void dataGridPort1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 헤더에서 눌렀을때 리턴.
            int irIdx = dataGridPort1.CurrentCell.RowIndex;
            int icIdx = dataGridPort1.CurrentCell.ColumnIndex;
            if (dataGridPort1.ContextMenuStrip == null) return;
            if (e.Button != MouseButtons.Right) return;
            if (pAddressMenu == null) return;            
            if (icIdx != 0) return;      
          
            // 현재 마우스 위치의 정보를 얻어옴
            Point pt = dataGridPort1.PointToClient(new Point(MousePosition.X, MousePosition.Y));

            var cellRectangle = dataGridPort1.GetCellDisplayRectangle(dataGridPort1.CurrentCell.ColumnIndex, dataGridPort1.CurrentCell.RowIndex, true);

            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);

            int titleHeight = screenRectangle.Top - this.Top;
            int titleWidth = screenRectangle.Left - this.Left;

            int tx = this.Left + groupBoxDevcie.Left + dataGridPort1.Left + cellRectangle.Left + titleWidth + 20;
            int ty = this.Top + groupBoxDevcie.Top + dataGridPort1.Top + cellRectangle.Top + titleHeight + 20;

            System.Windows.Forms.Cursor.Position = new Point(tx, ty);
            dataGridPort1.ContextMenuStrip.Left = tx;
            dataGridPort1.ContextMenuStrip.Top = ty;
        }

        private void dataGridPort1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            int irIdx = dataGridPort1.CurrentCell.RowIndex;
            if (irIdx < 0) return;
            try
            {
                if (dataGridPort1.Rows[irIdx].HeaderCell.Value == null || String.IsNullOrEmpty(dataGridPort1.Rows[irIdx].HeaderCell.Value.ToString()))
                    return;

                string strGetText = dataGridPort1.Rows[irIdx].HeaderCell.Value.ToString();

                switch (strGetText)
                {
                    case dev5515C:
                    case devKEITHLEY:
                    case devMTP200: dataGridPort1.ContextMenuStrip = pAddressMenu; break;
                    case dev34410a: dataGridPort1.ContextMenuStrip = pAddressMenuUsb; break;
                    case devTC1400A: dataGridPort1.ContextMenuStrip = null; break; //얘는 IP 어드레스를 입력하므로 보조 메뉴는 필요없다.
                    case devMTP120A: dataGridPort1.ContextMenuStrip = pAddressMenuTcp; break;

                    default: dataGridPort1.ContextMenuStrip = pComportMenu; break;
                }
            }

            catch { }

        }

        private void btnPasswordChange_Click(object sender, EventArgs e)
        {
            if (!iLoginUser.Equals((int)ACCOUNT.SUPERUSER))
            {
                MessageBox.Show("YOU CAN NOT CHANGE TO PASSWORD! THIS IS ONLY SUPER-USER"); return;
            }

            if (txtNewPassword.TextLength == 0 ||
                txtCnfPassword.TextLength == 0 ||
                   !txtNewPassword.Text.Equals(txtCnfPassword.Text))
            {
                txtNewPassword.Text = "";
                txtCnfPassword.Text = "";
                MessageBox.Show("PASSWORD CHANGE FAILURE! TRY AGAIN.");
                txtNewPassword.Focus();
                return;
            }

            if(txtNewPassword.Text.Equals(txtCnfPassword.Text))
            {
                //DKLoggerConfig.SaveINI("OPTION", "PASSWORD", txtNewPassword.Text); 
                //LGEVH 20230816                
                DKLoggerConfig.SavePWINI("OPTION", "PASSWORD", txtNewPassword.Text);
                MessageBox.Show("PASSWORD CHANGE SUCCESS!");
                txtNewPassword.Text = "";
                txtCnfPassword.Text = "";
                txtNewPassword.Focus();
           }


        }

        private void txtCnfPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                btnPasswordChange.PerformClick();
            }
        }
        

#region 체크박스

        private void PinSelect(CheckBox cbBox)
        {

            if (cbBox.Text != chkBoxIn1.Text) chkBoxIn1.Checked = false;
            if (cbBox.Text != chkBoxIn2.Text) chkBoxIn2.Checked = false;
            if (cbBox.Text != chkBoxIn3.Text) chkBoxIn3.Checked = false;
            if (cbBox.Text != chkBoxBub1.Text) chkBoxBub1.Checked = false;
            if (cbBox.Text != chkBoxBub2.Text) chkBoxBub2.Checked = false;
            if (cbBox.Text != chkBoxManual1.Text) chkBoxManual1.Checked = false;
            if (cbBox.Text != chkBoxManual3.Text) chkBoxManual3.Checked = false;
            if (cbBox.Text != chkBoxSetIn.Text) chkBoxSetIn.Checked = false;

        }
        private void chkBoxSpare1_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxIn1);
        }

        private void chkBoxSpare2_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxIn2);
        }

        private void chkBoxMute_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxIn3);
        }

        private void chkBoxM1_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxBub1);
        }

        private void chkBoxM2_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxBub2);
        }

        private void chkBoxBub_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxManual1);
        }

        private void chkBoxPal_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxManual3);
        }

        private void chkBoxSet_Click(object sender, EventArgs e)
        {
            PinSelect(chkBoxSetIn);
        }

        private void LoadDioCommands()
        {
            List<TBLDATA0> TBL_DIO;    //DIO   명령 테이블 리스트 
            TBL_DIO = new List<TBLDATA0>();      
            ClearCombobox();

            bool btmpDio = DKLoggerConfig.LoadTBL0("DIO_VCP.TBL", ref TBL_DIO);
            if (!btmpDio) return;
            string tmpCommand = String.Empty;
            int iIdx = 0;
            for (int i = 0; i < TBL_DIO.Count; i++)
            {
                tmpCommand = TBL_DIO[i].CMDNAME;
                iIdx = tmpCommand.IndexOf("#");
                if (iIdx < 0)
                {
                    cbBeforeStep1.Items.Add(tmpCommand);
                    cbBeforeStep2.Items.Add(tmpCommand);
                    cbBeforeStep3.Items.Add(tmpCommand);
                    cbResOKStep1.Items.Add(tmpCommand);
                    cbResOKStep2.Items.Add(tmpCommand);
                    cbResOKStep3.Items.Add(tmpCommand);
                    cbResNGStep1.Items.Add(tmpCommand);
                    cbResNGStep2.Items.Add(tmpCommand);
                    cbResNGStep3.Items.Add(tmpCommand);
                    cbResCHKStep1.Items.Add(tmpCommand);
                    cbResCHKStep2.Items.Add(tmpCommand);
                    cbResCHKStep3.Items.Add(tmpCommand);
                    cbResUSERTOPStep1.Items.Add(tmpCommand);
                    cbResUSERTOPStep2.Items.Add(tmpCommand);
                    cbResUSERTOPStep3.Items.Add(tmpCommand);
                    cbResEmptyStep1.Items.Add(tmpCommand);
                    cbResEmptyStep2.Items.Add(tmpCommand);
                    cbResEmptyStep3.Items.Add(tmpCommand);
                    cbResMesStep1.Items.Add(tmpCommand);
                    cbResMesStep2.Items.Add(tmpCommand);
                    cbResMesStep3.Items.Add(tmpCommand);
                    cbResEjectStep1.Items.Add(tmpCommand);
                    cbResEjectStep2.Items.Add(tmpCommand);
                    cbResEjectStep3.Items.Add(tmpCommand);
                    cbResErrorStep1.Items.Add(tmpCommand);
                    cbResErrorStep2.Items.Add(tmpCommand);
                    cbResErrorStep3.Items.Add(tmpCommand);
                }
            }
        }

        private void ClearCombobox()
        {
            cbBeforeStep1.Items.Clear();
            cbBeforeStep2.Items.Clear();
            cbBeforeStep3.Items.Clear();
            cbResOKStep1.Items.Clear();
            cbResOKStep2.Items.Clear();
            cbResOKStep3.Items.Clear();
            cbResNGStep1.Items.Clear();
            cbResNGStep2.Items.Clear();
            cbResNGStep3.Items.Clear();
            cbResCHKStep1.Items.Clear();
            cbResCHKStep2.Items.Clear();
            cbResCHKStep3.Items.Clear();
            cbResEmptyStep1.Items.Clear();
            cbResEmptyStep2.Items.Clear();
            cbResEmptyStep3.Items.Clear();
            cbResMesStep1.Items.Clear();
            cbResMesStep2.Items.Clear();
            cbResMesStep3.Items.Clear();
            cbResEjectStep1.Items.Clear();
            cbResEjectStep2.Items.Clear();
            cbResEjectStep3.Items.Clear();
            cbResErrorStep1.Items.Clear();
            cbResErrorStep2.Items.Clear();
            cbResErrorStep3.Items.Clear();
        }

        private void DioSignalCommandSave()
        {
            string strControl = String.Empty;

            strControl = GetCbText(cbBeforeStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "BEFORESTART1", strControl);
            strControl = GetCbText(cbBeforeStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "BEFORESTART2", strControl);
            strControl = GetCbText(cbBeforeStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "BEFORESTART3", strControl);

            strControl = GetCbText(cbResOKStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTOK1", strControl);
            strControl = GetCbText(cbResOKStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTOK2", strControl);
            strControl = GetCbText(cbResOKStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTOK3", strControl);

            strControl = GetCbText(cbResNGStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTNG1", strControl);
            strControl = GetCbText(cbResNGStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTNG2", strControl);
            strControl = GetCbText(cbResNGStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTNG3", strControl);

            strControl = GetCbText(cbResCHKStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTCHK1", strControl);
            strControl = GetCbText(cbResCHKStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTCHK2", strControl);
            strControl = GetCbText(cbResCHKStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTCHK3", strControl);

            strControl = GetCbText(cbResUSERTOPStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "USERSTOP1", strControl);
            strControl = GetCbText(cbResUSERTOPStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "USERSTOP2", strControl);
            strControl = GetCbText(cbResUSERTOPStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "USERSTOP3", strControl);

            strControl = GetCbText(cbResEmptyStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTEMPTY1", strControl);
            strControl = GetCbText(cbResEmptyStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTEMPTY2", strControl);
            strControl = GetCbText(cbResEmptyStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTEMPTY3", strControl);

            strControl = GetCbText(cbResMesStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTMES1", strControl);
            strControl = GetCbText(cbResMesStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTMES2", strControl);
            strControl = GetCbText(cbResMesStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTMES3", strControl);

            strControl = GetCbText(cbResEjectStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTEJECT1", strControl);
            strControl = GetCbText(cbResEjectStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTEJECT2", strControl);
            strControl = GetCbText(cbResEjectStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTEJECT3", strControl);

            strControl = GetCbText(cbResErrorStep1);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTERROR1", strControl);
            strControl = GetCbText(cbResErrorStep2);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTERROR2", strControl);
            strControl = GetCbText(cbResErrorStep3);
            DKLoggerConfig.SaveINI("SIGNALOPTION", "RESULTERROR3", strControl);
            
        }

        private string GetCbText(ComboBox cbBox)
        {
            string strRtnVal = String.Empty;



            if (String.IsNullOrEmpty(cbBox.Text))
            {
                strRtnVal = " ";
            }
            else
            {
                return cbBox.Text;
            }

            return strRtnVal;
        }

        private void SetCbText(ComboBox cbBox, string strCmdname)
        {
            if (cbBox.Items.Contains(strCmdname))
            {
                int iIdx = cbBox.Items.IndexOf(strCmdname);
                cbBox.SelectedItem = iIdx;
                cbBox.SelectedText = strCmdname;
            }
            else
            {
                cbBox.SelectedText = " ";
            }
            
        }

        private void DioSignalCommandLoad()
        {
            string strControl = String.Empty;

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "BEFORESTART1");
            SetCbText(cbBeforeStep1, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "BEFORESTART2");
            SetCbText(cbBeforeStep2, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "BEFORESTART3");
            SetCbText(cbBeforeStep3, strControl);

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTOK1");
            SetCbText(cbResOKStep1, strControl);            
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTOK2");
            SetCbText(cbResOKStep2, strControl);   
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTOK3");
            SetCbText(cbResOKStep3, strControl);   

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTNG1");
            SetCbText(cbResNGStep1, strControl);   
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTNG2");
            SetCbText(cbResNGStep2, strControl);   
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTNG3");
            SetCbText(cbResNGStep3, strControl);   

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTCHK1");
            SetCbText(cbResCHKStep1, strControl);   
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTCHK2");
            SetCbText(cbResCHKStep2, strControl);   
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTCHK3");
            SetCbText(cbResCHKStep3, strControl);

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "USERSTOP1");
            SetCbText(cbResUSERTOPStep1, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "USERSTOP2");
            SetCbText(cbResUSERTOPStep2, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "USERSTOP3");
            SetCbText(cbResUSERTOPStep3, strControl);

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTEMPTY1");
            SetCbText(cbResEmptyStep1, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTEMPTY2");
            SetCbText(cbResEmptyStep2, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTEMPTY3");
            SetCbText(cbResEmptyStep3, strControl);

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTMES1");
            SetCbText(cbResMesStep1, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTMES2");
            SetCbText(cbResMesStep2, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTMES3");
            SetCbText(cbResMesStep3, strControl);

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTEJECT1");
            SetCbText(cbResEjectStep1, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTEJECT2");
            SetCbText(cbResEjectStep2, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTEJECT3");
            SetCbText(cbResEjectStep3, strControl);

            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTERROR1");
            SetCbText(cbResErrorStep1, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTERROR2");
            SetCbText(cbResErrorStep2, strControl);
            strControl = DKLoggerConfig.LoadINI("SIGNALOPTION", "RESULTERROR3");
            SetCbText(cbResErrorStep3, strControl);
        }

#endregion

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void FrmConfig_Shown(object sender, EventArgs e)
        {
            CreateSecCount();
            DioSignalCommandLoad();
            if (chkBoxMSize.Checked)
            {
                this.WindowState = FormWindowState.Maximized;
                this.AutoScroll = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.AutoScroll = false;
            }
            this.Refresh();
        }

        private void btnCableClear_Click(object sender, EventArgs e)
        {
            ConnectorCountClear(0);
        }

        private void btnCableClear2_Click(object sender, EventArgs e)
        {
            ConnectorCountClear(1);
        }

        private void btnCableClear3_Click(object sender, EventArgs e)
        {
            ConnectorCountClear(2);
        }

        private void btnCableClear4_Click(object sender, EventArgs e)
        {
            ConnectorCountClear(3);
        }

        private void btnCableClear5_Click(object sender, EventArgs e)
        {
            ConnectorCountClear(4);
        }

        private void BtnCntClear_Click(object sender, EventArgs e)
        {
            txtPass.Text = txtFail.Text = txtTotal.Text = "0";
        }

        private void txtWarning_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void chkUseBarcode_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkUseBarcode.Checked )
            {
                chkStartBarcode.Checked = chkUseZebra.Checked = chkUseSubId.Checked = false;
                chkStartBarcode.Enabled = chkUseZebra.Enabled = chkUseSubId.Enabled = false;
            }
            else
            {

                chkStartBarcode.Enabled = chkUseZebra.Enabled = chkUseSubId.Enabled = true;
            }


        }

        private void chkUse232Barcode_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkUseBarcode.Checked )
            {
                chkStartBarcode.Checked = chkUseZebra.Checked = chkUseSubId.Checked = false;
                chkStartBarcode.Enabled = chkUseZebra.Enabled = chkUseSubId.Enabled = false;
            }
            else
            {
                chkStartBarcode.Enabled = chkUseZebra.Enabled = chkUseSubId.Enabled = true;
                
            }


        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
            //FrmJobMapping tmpFrm = null;
            //tmpFrm = new FrmJobMapping();
            //tmpFrm.ShowDialog();

            FrmPassWord tmpFrm = null;
            tmpFrm = new FrmPassWord();
            tmpFrm.ShowDialog();
            PWUSER testUser = new PWUSER();
            int iAuth = tmpFrm.IsOK(ref testUser);

            //LGEVH 202306
            FrmJobMapping tmpFrm2 = null;
            if (iAuth.Equals((int)ACCOUNT.SUPERUSER))
                tmpFrm2 = new FrmJobMapping((int)ACCOUNT.SUPERUSER, testUser);
            else
                tmpFrm2 = new FrmJobMapping((int)ACCOUNT.USER, testUser);

            tmpFrm2.ShowDialog();
        }

        private void chkBoxMSize_CheckedChanged(object sender, EventArgs e)
        {
            
            if (chkBoxMSize.Checked)
            {
                this.WindowState = FormWindowState.Maximized;
                this.AutoScroll = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.AutoScroll = false;
            }
            this.Refresh();
        
        }

        private void ViewSecContorl(bool bShow)
        {
            txtMyName.Visible = txtUpdateIP.Visible = lblMyName.Visible = lblUpdateIP.Visible = bShow;
            if (bShow)
            {
                txtMyName.Focus();
            }
        }

        private void secLlb1_OnMouseUp(object sender, AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEvent e)
        {
            STEPMANAGER_VALUE.AddSecount1();
            CheckSecCount();
        }

        private void secLlb2_OnMouseUp(object sender, AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEvent e)
        {
            STEPMANAGER_VALUE.AddSecount2();
            CheckSecCount();
        }

        private void CreateSecCount()
        {
            STEPMANAGER_VALUE.CreateSecCount();
            secLlb1.Caption = STEPMANAGER_VALUE.GetSecCount1().ToString();
            secLlb2.Caption = STEPMANAGER_VALUE.GetSecCount2().ToString();
        }

        private void CheckSecCount()
        {
            ViewSecContorl(STEPMANAGER_VALUE.CheckSecCount());
        }              

        private void btnShowDeviceManager_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("devmgmt.msc");
            }
            catch { }
        }

        private void btnPasswordChangeUser_Click(object sender, EventArgs e)
        {
            if (!iLoginUser.Equals((int)ACCOUNT.SUPERUSER))
            {
                MessageBox.Show("YOU CAN NOT MANAGEMENT. THIS IS ONLY SUPER-USER"); return;
            }

            FrmPasswordManage frmPWM = new FrmPasswordManage();
            frmPWM.ShowDialog();
        }

        private void chkForATCO_CheckedChanged(object sender, EventArgs e)
        {
            lblStation.Enabled = chkForATCO.Checked;
            txtStation.Enabled = chkForATCO.Checked;
        }

        private void chkUseRetry_CheckedChanged(object sender, EventArgs e)
        {
            lblRetryCount.Enabled = chkUseRetry.Checked;
            txtRetryCount.Enabled = chkUseRetry.Checked;
        }

        private void chkBoxChangeTime_CheckedChanged(object sender, EventArgs e)
        {
            nmHour.Enabled = nmMin.Enabled = chkBoxChangeTime.Checked;
        }
        private void ControlRDLoad(string strTitle, string strName)
        {
            string strGetText = DKLoggerConfig.LoadINI(strTitle, strName).Trim();

            switch (strGetText)
            {
                case "NONE": rdWHCode_NONE.Checked = true; break;
                case "N9V": rdWHCode_N9V.Checked = true; break;
                case "N9X": rdWHCode_N9X.Checked = true; break;
                case "TBD": rdWHCode_TBD.Checked = true; break;
                default: rdWHCode_N9V.Checked = true; break;
            }
        }

        private void rdWHCode_Click(object sender, EventArgs e)
        {
            if (rdWHCode_NONE.Checked)
                txtWHCode.Text = WHCODE.NONE.ToString();
            else if (rdWHCode_N9V.Checked)
                txtWHCode.Text = WHCODE.N9V.ToString();
            else if (rdWHCode_N9X.Checked)
                txtWHCode.Text = WHCODE.N9X.ToString();
            else if (rdWHCode_TBD.Checked)
                txtWHCode.Text = WHCODE.TBD.ToString();
            else
                txtWHCode.Text = WHCODE.NONE.ToString();
        }
    }
}

