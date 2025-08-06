using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GmTelematics
{
    public partial class FrmEdit : Form
    {
#region 변수 선언
        private const string devNAD = "NAD";
        private const string devDIO = "DIO";
        private const string devGEN9  = "GEN9";
        private const string devGEN10 = "GEN10";
        private const string devGEN11 = "GEN11";
        private const string devGEN11P = "GEN11P";
        private const string devGEN12 = "GEN12";
        private const string devCCM    = "CCM";
        private const string devMCTM   = "MCTM";
        private const string devTC3000 = "TC3000";
        private const string devTC1400A = "TC1400A";
        private const string devODAPWR = "ODAPWR";
        private const string devMTP200 = "MTP200";
        private const string devTCP   = "TCP";
        private const string devATT   = "ATT";
        private const string dev5515C = "5515C";
        private const string dev34410A = "34410A";
        private const string devPCAN = "PCAN";
        private const string devVECTOR = "VECTOR";
        private const string devAudio = "AUDIOSELECTOR";
        private const string devADC   = "ADC";
        private const string devKEITHLEY = "KEITHLEY";
        private const string devDLLGATE = "DLLGATE";
        private const string devMELSEC = "MELSEC";
        private const string devMTP120A = "MTP120A";

        private int iGrdColCount = 0;
        private DK_LOGGER DKLogger_EDIT;
        private ContextMenuStrip pMenu;
        private ToolStripMenuItem[] mnuTool = new ToolStripMenuItem[9];
        private ToolStripMenuItem mnuType;        
        private ToolStripMenuItem[] mnuPage = new ToolStripMenuItem[33];
        private ToolStripMenuItem[] mnuScan = new ToolStripMenuItem[4];
        private ToolStripMenuItem[] mnuGmes = new ToolStripMenuItem[18];
        private ToolStripMenuItem[] mnuOracle = new ToolStripMenuItem[12];
        private ToolStripMenuItem[] mnuExcel = new ToolStripMenuItem[5];
        private ToolStripMenuItem mnuDisplay;
        private ToolStripMenuItem[] mnuLabel      = new ToolStripMenuItem[5];// new ToolStripMenuItem[(int)JOBLABEL.A20];
        private ToolStripMenuItem[] mnuLabelCount = new ToolStripMenuItem[8];
        private ToolStripMenuItem[] mnuTBL_GEN9 = null;
        private ToolStripMenuItem[] mnuTBL_GEN10 = null;
        private ToolStripMenuItem[] mnuTBL_TC3000 = null;
        private ToolStripMenuItem[] mnuTBL_TC1400A = null;
        private ToolStripMenuItem[] mnuTBL_MTP200 = null;
        private ToolStripMenuItem[] mnuTBL_ODAPWR = null;
        private ToolStripMenuItem[] mnuTBL_TCP = null;
        private ToolStripMenuItem[] mnuTBL_GEN11 = null;
        private ToolStripMenuItem[] mnuTBL_GEN11P = null;
        private ToolStripMenuItem[] mnuTBL_GEN12 = null;
        private ToolStripMenuItem[] mnuTBL_CCM = null;
        private ToolStripMenuItem[] mnuTBL_NAD = null;
        private ToolStripMenuItem[] mnuTBL_MCTM = null;
        private ToolStripMenuItem[] mnuTBL_ATT = null;
        private ToolStripMenuItem[] mnuTBL_DIO = null;
        private ToolStripMenuItem[] mnuTBL_5515C = null;
        private ToolStripMenuItem[] mnuTBL_KEITHLEY = null;
        private ToolStripMenuItem[] mnuTBL_34410A = null;
        private ToolStripMenuItem[] mnuTBL_PCAN = null;
        private ToolStripMenuItem[] mnuTBL_VECTOR = null;
        private ToolStripMenuItem[] mnuTBL_DTC_GEN9  = null;
        private ToolStripMenuItem[] mnuTBL_DTC_GEN10 = null;
        private ToolStripMenuItem[] mnuTBL_DTC_GEN11 = null;
        private ToolStripMenuItem[] mnuTBL_DTC_GEN12 = null;
        private ToolStripMenuItem[] mnuTBL_DTC_MCTM  = null;
        private ToolStripMenuItem[] mnuTBL_DTC_TCP = null;
        private ToolStripMenuItem[] mnuTBL_AUDIO = null;
        private ToolStripMenuItem[] mnuTBL_ADC = null;
        private ToolStripMenuItem[] mnuTBL_DLLGATE = null;
        private ToolStripMenuItem[] mnuTBL_MELSEC = null;
        private ToolStripMenuItem[] mnuTBL_MTP120A = null;

        private ToolStripMenuItem[] mnuOption   = new ToolStripMenuItem[10];
        private ToolStripMenuItem[] mnuCaseNG   = new ToolStripMenuItem[12];
        private ToolStripMenuItem[] mnuCompare  = new ToolStripMenuItem[22];
        private ToolStripMenuItem[] mnuAction   = new ToolStripMenuItem[3];
        private ToolStripMenuItem[] mnuOOB      = new ToolStripMenuItem[7];

        private List<TBLDATA0> TBL_GEN9;   //GEN9   명령 테이블 리스트
        private List<TBLDATA0> TBL_GEN10;  //GEN10   명령 테이블 리스트
        private List<TBLDATA0> TBL_GEN11;   //GEN11  명령 테이블 리스트  
        private List<TBLDATA0> TBL_GEN11P;   //GEN11  출하명령 테이블 리스트  
        private List<TBLDATA0> TBL_GEN12;   //GEN12  명령 테이블 리스트  
        private List<TBLDATA0> TBL_CCM;   //CCM  명령 테이블 리스트  
        private List<TBLDATA0> TBL_NAD;   //NAD  명령 테이블 리스트  
        private List<TBLDATA0> TBL_MCTM;    //MCTM   명령 테이블 리스트  
        private List<TBLDATA0> TBL_TC3000;  //TC3000   명령 테이블 리스트  
        private List<TBLDATA0> TBL_TC1400A;    //TC1400A  명령 테이블 리스트  
        private List<TBLDATA0> TBL_MTP200;  //MTP200   명령 테이블 리스트          
        private List<TBLDATA0> TBL_ODAPWR;  //ODA POWER   명령 테이블 리스트          
        private List<TBLDATA0> TBL_TCP;    //TCP   명령 테이블 리스트  
        private List<TBLDATA0> TBL_ATT;    //ATT   명령 테이블 리스트  
        private List<TBLDATA0> TBL_DIO;    //DIO   명령 테이블 리스트     
        private List<TBLDATA0> TBL_AUDIO;      //DIO AUDIO   명령 테이블 리스트     
        private List<TBLDATA0> TBL_ADC;        //ADC MODULE   명령 테이블 리스트  
        private List<TBLDATA0> TBL_5515C;      //5515C 명령 테이블 리스트  
        private List<TBLDATA0> TBL_34410A;     //34410A  명령 테이블 리스트  
        private List<TBLDATA0> TBL_PCAN;       //PCAN 명령 테이블 리스트  
        private List<TBLDATA0> TBL_VECTOR;     //VECOTR 명령 테이블 리스트  
        private List<DTCDATA0> TBL_DTC_GEN9;   //GEN9 DTC 테이블 리스트  
        private List<DTCDATA0> TBL_DTC_GEN10;  //GEN10 DTC 테이블 리스트  
        private List<DTCDATA0> TBL_DTC_GEN11;  //GEN11 DTC 테이블 리스트  
        private List<DTCDATA0> TBL_DTC_GEN12;  //GEN12 DTC 테이블 리스트  
        private List<DTCDATA0> TBL_DTC_MCTM;   //MCTM DTC 테이블 리스트  
        private List<DTCDATA0> TBL_DTC_TCP;    //TCP DTC 테이블 리스트  
        private List<TBLDATA0> TBL_KEITHLEY;   //KEITHLEY 명령 테이블 리스트  
        private List<TBLDATA0> TBL_DLLGATE;    //GEN DLL 명령 테이블 리스트  
        private List<TBLDATA0> TBL_MELSEC;     //MELSEC  명령 테이블 리스트
        private List<TBLDATA0> TBL_MTP120A;   //mnuTBL_MTP120A         명령 테이블 리스트  

        private int iCellDragStartPoint;
        private bool bAutoSizeFlag = false;
        private string strEditorName = String.Empty;

        private List<JOBFILES> JOBlist  = new List<JOBFILES>();
        private List<string>   lstCells = new List<string>();
        private bool bOnJobFiles = false;

#endregion

        #region 폼관련

        private void CellDefaultSize()
        {
            bOnJobFiles = false;
            if (bAutoSizeFlag)
            {
                bAutoSizeFlag = false;
                CellAutoSize();
            }
            SetSizeTextFinder();       
        }

        private void SetSizeTextFinder()
        {
            pannelTextFinder.Location = cbJobFiles.Location;
            pannelTextFinder.Width = cbJobFiles.Width - 1;
            btnFinderClose.Left = pannelTextFinder.Right - btnFinderClose.Width - 10;
            lblFind.Width = pannelTextFinder.Width - 4;
            txtTargetFind.Width = btnFinderClose.Left - txtTargetFind.Left - 5;
        }

        private void CellAutoSize()
        {
            if (bAutoSizeFlag)
            {
                for (int i = 0; i < dataGridEdit.Columns.Count; i++)
                {
                    dataGridEdit.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

            }
            else
            {
                for (int i = 0; i < dataGridEdit.Columns.Count; i++)
                {
                    dataGridEdit.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }
        }

        private void FrmEdit_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bAutoSizeFlag = !bAutoSizeFlag;
            CellAutoSize();
        }

        private void ControlFontReset(ToolStripMenuItem[] targetCtrl)
        {
            for (int i = 0; i < targetCtrl.Length; i++)
            {
                targetCtrl[i].Font = new System.Drawing.Font("Courier New", 9.75F);
            }
        }
        private void SetFont()
        {            
            mnuType.Font = new System.Drawing.Font("Courier New", 9.75F);            
            mnuDisplay.Font = new System.Drawing.Font("Courier New", 9.75F);
            ControlFontReset(mnuTool);
            ControlFontReset(mnuLabel);
            ControlFontReset(mnuLabelCount);
            ControlFontReset(mnuTBL_GEN9);
            ControlFontReset(mnuTBL_GEN10);
            ControlFontReset(mnuTBL_GEN11);
            ControlFontReset(mnuTBL_GEN11P);
            ControlFontReset(mnuTBL_GEN12);
            ControlFontReset(mnuTBL_CCM);
            ControlFontReset(mnuTBL_NAD);
            ControlFontReset(mnuTBL_MCTM);
            ControlFontReset(mnuTBL_TC3000);
            ControlFontReset(mnuTBL_TC1400A);
            ControlFontReset(mnuTBL_MTP200);
            ControlFontReset(mnuTBL_ODAPWR);
            ControlFontReset(mnuTBL_TCP);
            ControlFontReset(mnuTBL_ATT);
            ControlFontReset(mnuTBL_NAD);
            ControlFontReset(mnuTBL_5515C);
            ControlFontReset(mnuTBL_34410A);
            ControlFontReset(mnuTBL_PCAN);
            ControlFontReset(mnuTBL_VECTOR);
            ControlFontReset(mnuTBL_DTC_GEN9);
            ControlFontReset(mnuTBL_DTC_GEN10);
            ControlFontReset(mnuTBL_DTC_GEN11);
            ControlFontReset(mnuTBL_DTC_MCTM);  
            ControlFontReset(mnuTBL_DTC_TCP);            
            ControlFontReset(mnuTBL_DIO);
            ControlFontReset(mnuTBL_AUDIO);
            ControlFontReset(mnuTBL_ADC);
            ControlFontReset(mnuTBL_KEITHLEY);
            ControlFontReset(mnuTBL_DLLGATE);
            ControlFontReset(mnuTBL_MELSEC);
            ControlFontReset(mnuOption);
            ControlFontReset(mnuCaseNG);
            ControlFontReset(mnuCompare);
            ControlFontReset(mnuAction);
            ControlFontReset(mnuTBL_MTP120A);
        }

        public FrmEdit(int iJobListIndex, string strUserName)
        {
            InitializeComponent();
            InitValue();
            UI_Edit();
            DKLogger_EDIT = new DK_LOGGER("PC", false);            
            if (TableFileLoad())
            {
                ComboListUpdate(iJobListIndex);
                SetFont();
            }
            strEditorName = strUserName;
        }
        
        private void InitValue()
        {
            iCellDragStartPoint = 0;
            
        }
        private bool TableFileLoad()
        {
            bool bRet = true;
            
            TBL_NAD    = new List<TBLDATA0>();
            TBL_GEN9   = new List<TBLDATA0>();
            TBL_GEN10  = new List<TBLDATA0>();
            TBL_TC3000 = new List<TBLDATA0>();
            TBL_TC1400A = new List<TBLDATA0>();
            TBL_MTP200 = new List<TBLDATA0>();
            TBL_ODAPWR = new List<TBLDATA0>();
            TBL_GEN11  = new List<TBLDATA0>();
            TBL_GEN11P = new List<TBLDATA0>();
            TBL_GEN12  = new List<TBLDATA0>();
            TBL_CCM    = new List<TBLDATA0>();
            TBL_NAD    = new List<TBLDATA0>();
            TBL_MCTM   = new List<TBLDATA0>();
            TBL_TCP    = new List<TBLDATA0>();
            TBL_ATT    = new List<TBLDATA0>();
            TBL_DIO    = new List<TBLDATA0>();
            TBL_AUDIO  = new List<TBLDATA0>();
            TBL_ADC    = new List<TBLDATA0>();
            TBL_5515C  = new List<TBLDATA0>();
            TBL_34410A = new List<TBLDATA0>();
            TBL_PCAN   = new List<TBLDATA0>();
            TBL_VECTOR = new List<TBLDATA0>();
            TBL_KEITHLEY = new List<TBLDATA0>();
            TBL_DTC_GEN9  = new List<DTCDATA0>();
            TBL_DTC_GEN10 = new List<DTCDATA0>();
            TBL_DTC_GEN11 = new List<DTCDATA0>();
            TBL_DTC_GEN12 = new List<DTCDATA0>();
            TBL_DTC_MCTM = new List<DTCDATA0>();
            TBL_DTC_TCP = new List<DTCDATA0>();
            TBL_DLLGATE = new List<TBLDATA0>();
            TBL_MELSEC = new List<TBLDATA0>();
            TBL_MTP120A = new List<TBLDATA0>();

            bool btmpDio      = DKLogger_EDIT.LoadTBL0("DIO_VCP.TBL",     ref TBL_DIO);
            bool btmpAudio    = DKLogger_EDIT.LoadTBL0("AUDIOSELECOTOR.TBL", ref TBL_AUDIO);
            bool btmpADC      = DKLogger_EDIT.LoadTBL0("ADCMODULE.TBL", ref TBL_ADC);
            bool btmpGen9     = DKLogger_EDIT.LoadTBL0("GEN9.TBL",        ref TBL_GEN9);
            bool btmpGen10    = DKLogger_EDIT.LoadTBL0("GEN10.TBL",       ref TBL_GEN10);
            bool btmpTc3000   = DKLogger_EDIT.LoadTBL0("TC3000.TBL",      ref TBL_TC3000);
            bool btmpTc1400a = DKLogger_EDIT.LoadTBL0("TC1400A.TBL",      ref TBL_TC1400A);
            bool btmpMtp200   = DKLogger_EDIT.LoadTBL0("MTP200.TBL",      ref TBL_MTP200);
            bool btmpOdaPwr   = DKLogger_EDIT.LoadTBL0("ODA.TBL",         ref TBL_ODAPWR);
            bool btmpTcp      = DKLogger_EDIT.LoadTBL0("TCP.TBL",         ref TBL_TCP);
            bool btmpGen11    = DKLogger_EDIT.LoadTBL0("GEN11.TBL",       ref TBL_GEN11);
            bool btmpGen11P   = DKLogger_EDIT.LoadTBL0("GEN11P.TBL",      ref TBL_GEN11P);
            bool btmpGen12    = DKLogger_EDIT.LoadTBL0("GEN12.TBL",       ref TBL_GEN12);
            bool btmpCCM      = DKLogger_EDIT.LoadTBL0("CCM.TBL",         ref TBL_CCM);
            bool btmpNAD      = DKLogger_EDIT.LoadTBL0("NAD.TBL",         ref TBL_NAD);
            bool btmpMCTM     = DKLogger_EDIT.LoadTBL0("MCTM.TBL",        ref TBL_MCTM);
            bool btmpAtt      = DKLogger_EDIT.LoadTBL0("ATT_TCP.TBL",     ref TBL_ATT);            
            bool btmp5515c    = DKLogger_EDIT.LoadTBL0("5515C.TBL",       ref TBL_5515C);
            bool btmpPCAN     = DKLogger_EDIT.LoadTBL0("PCAN.TBL",        ref TBL_PCAN);
            bool btmpVECTOR   = DKLogger_EDIT.LoadTBL0("VECTOR.TBL",      ref TBL_VECTOR);
            bool btmpDTCgen9  = DKLogger_EDIT.LoadDTCTBL("DTC_GEN9.TBL",  ref TBL_DTC_GEN9);
            bool btmpDTCgen10 = DKLogger_EDIT.LoadDTCTBL("DTC_GEN10.TBL", ref TBL_DTC_GEN10);
            bool btmpDTCgen11 = DKLogger_EDIT.LoadDTCTBL("DTC_GEN11.TBL", ref TBL_DTC_GEN11);
            bool btmpDTCgen12 = DKLogger_EDIT.LoadDTCTBL("DTC_GEN12.TBL", ref TBL_DTC_GEN12);
            bool btmpDTCMctm  = DKLogger_EDIT.LoadDTCTBL("DTC_MCTM.TBL",  ref TBL_DTC_MCTM);
            bool btmpDTCtcp   = DKLogger_EDIT.LoadDTCTBL("DTC_TCP.TBL",   ref TBL_DTC_TCP);
            bool btmp34410a   = DKLogger_EDIT.LoadTBL0("34410A.TBL", ref TBL_34410A);
            bool btmpKEITHLEY = DKLogger_EDIT.LoadTBL0("KEITHLEY.TBL", ref TBL_KEITHLEY);
            bool btmpDLLGATE  = DKLogger_EDIT.LoadTBL0("DLLGATE.TBL", ref TBL_DLLGATE);
            bool btmpMELSEC   = DKLogger_EDIT.LoadTBL0("MELSEC.TBL", ref TBL_MELSEC);
            bool btmpMTP120A = DKLogger_EDIT.LoadTBL0("MTP120A.TBL", ref TBL_MTP120A);

            if (btmpDio && btmpGen9 && btmpGen10 && btmpGen11 && btmpGen12 && btmpCCM && btmpTcp && btmp5515c && btmpNAD && btmpPCAN
                    && btmpDTCtcp && btmpDTCgen9 && btmpDTCgen10 && btmpTc3000 && btmpMtp200 && btmpAtt && btmpAudio && btmp34410a
                    && btmpVECTOR && btmpOdaPwr && btmpMCTM && btmpKEITHLEY && btmpDLLGATE && btmpMELSEC && btmpTc1400a && btmpMTP120A)
            {
                MakeMenu();
            }
            else
            {
                MessageBox.Show("Please, Check the TABLE Files(DIO_VCP.TBL, GEN9.TBL, GEN10.TBL, GEN11.TBL, GEN12.TBL, CCM.TBL, TCP.TBL, 5515C.TBL, MTP200.TBL, NAD.TBL, 5515C.TBL, PCAN.TBL , DTC_TCP.TBL, ATT_TCP.TBL, AUDIOSELECOTOR.TBL, 34410A.TBL, ODA.TBL, MCTM.TBL, KEITHLEY.TBL, DLLGATE.TBL, MELSEC.TBL, TC1400A.TBL, MTP120A.TBL");
                bRet = false;
            }
            return bRet;
            
        }
        private void FrmEdit_Load(object sender, EventArgs e)
        {

        }
        private void UI_Edit()
        {
            dataGridEdit.DoubleBuffered(true);
            // 화면 해상도
            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            int iEdge = dataGridEdit.Left;
            int iGridSize = ScreenWidth - (iEdge * 2);
            int iTempSize = (int)(iGridSize - 50);
            
            //그리드 디자인
            //TYPE<>CMD<>DISPLAYNAME<>MESCODE<>ACTION<>LABEL<>LABELCOUNT<>CASENG<>DELAY<>TIMEOUT<>RETRY<>COMPARE<>MIN<>MAX<>OPTION<>PAR1<>DOC<>EXPR

            dataGridEdit.Width = iGridSize;
            dataGridEdit.Height = ScreenHeight - 130;
            dataGridEdit.AllowUserToResizeRows = false;

            dataGridEdit.Columns.Add("Col0", "TYPE");
            dataGridEdit.Columns[0].Width = (int)(iTempSize * 0.05);
            dataGridEdit.Columns[0].ReadOnly = true;            
            dataGridEdit.Columns.Add("Col1", "COMMAND");
            dataGridEdit.Columns[1].Width = (int)(iTempSize * 0.125);
            dataGridEdit.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns[1].ReadOnly = true;
            dataGridEdit.Columns.Add("Col2", "DISPLAY NAME");
            dataGridEdit.Columns[2].Width = (int)(iTempSize * 0.155);
            dataGridEdit.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add("Col3", "MES CODE");
            dataGridEdit.Columns[3].Width = (int)(iTempSize * 0.06);
            dataGridEdit.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add("Col4", "ACTION");
            dataGridEdit.Columns[4].Width = (int)(iTempSize * 0.04);
            dataGridEdit.Columns[4].ReadOnly = true;
            dataGridEdit.Columns.Add("Col5", "LABEL");
            dataGridEdit.Columns[5].Width = (int)(iTempSize * 0.035);
            dataGridEdit.Columns[5].ReadOnly = true;
            dataGridEdit.Columns.Add("Col6", "LABEL COUNT");
            dataGridEdit.Columns[6].Width = (int)(iTempSize * 0.04);
            dataGridEdit.Columns[6].ReadOnly = true;
            dataGridEdit.Columns.Add("Col7", "CASE NG");
            dataGridEdit.Columns[7].Width = (int)(iTempSize * 0.07);
            dataGridEdit.Columns[7].ReadOnly = true;
            dataGridEdit.Columns.Add("Col8", "DELAY");
            dataGridEdit.Columns[8].Width = (int)(iTempSize * 0.035);
            dataGridEdit.Columns.Add("Col9", "TIME OUT");
            dataGridEdit.Columns[9].Width = (int)(iTempSize * 0.035);
            dataGridEdit.Columns.Add("Col10", "RE TRY");
            dataGridEdit.Columns[10].Width = (int)(iTempSize * 0.03);
            dataGridEdit.Columns.Add("Col11", "COMPARE");
            dataGridEdit.Columns[11].Width = (int)(iTempSize * 0.07);
            dataGridEdit.Columns[11].ReadOnly = true;
            dataGridEdit.Columns.Add("Col12", "MIN");
            dataGridEdit.Columns[12].Width = (int)(iTempSize * 0.03);
            dataGridEdit.Columns.Add("Col13", "MAX");
            dataGridEdit.Columns[13].Width = (int)(iTempSize * 0.065);
            dataGridEdit.Columns.Add("Col14", "OPTION");
            dataGridEdit.Columns[14].Width = (int)(iTempSize * 0.07);
            dataGridEdit.Columns[14].ReadOnly = true;
            dataGridEdit.Columns.Add("Col15", "PAR1");
            dataGridEdit.Columns[15].Width = (int)(iTempSize * 0.08);
            dataGridEdit.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add("Col16", "DOC");
            dataGridEdit.Columns[16].Width = (int)(iTempSize * 0.04);
            dataGridEdit.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add("Col17", "EXPR");
            dataGridEdit.Columns[17].Width = (int)(iTempSize * 0.07);
            dataGridEdit.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add("Col18", " ");
            dataGridEdit.Columns[18].Width = (int)(iTempSize * 0.17);

            iGrdColCount = dataGridEdit.ColumnCount - 1;
            for (int i = 0; i < iGrdColCount; i++)
            {
                dataGridEdit.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                
            }
            dataGridEdit.Columns[17].ToolTipText = 
                "USE EXPR:\r\n" +
                "max field :#GMES_fieldname or #EXPR_fieldname  \r\n" + 
                "par1 field :#GMES_fieldname or #EXPR_fieldname \r\n " +
                "expr field :#LOAD:fieldname -> Change Measure Value.\r\n" +
                "expr field :#SAVE:fieldname -> Save   Measure Value.\r\n" +
                "expr field :#MATH:10+20+MEAS-20-fieldname   -> Calculate Expression.\r\n" +
                "expr field : MEAS = This Command response measure value.\r\n";
                
            btnExit.Left = dataGridEdit.Right - btnExit.Width;

        }
        private void cbJobFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            bOnJobFiles = false;
            CellDefaultSize();
            dgvResetGrid();
            
        }
        private void ComboListUpdate(int iJobListIndex)
        {
            cbJobFiles.Items.Clear();

            string[] files = DKLogger_EDIT.GetFileList("JOB");

            if (files != null || files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    cbJobFiles.Items.Add(files[i].ToString());
                    //cbJobFiles.SelectedIndex = 0;
                }
                if (iJobListIndex >= 0)
                {

                    if (cbJobFiles.Items.Count > 0)
                    {
                        cbJobFiles.SelectedIndex = iJobListIndex;
                    }
                }
            }
            dgvResetGrid();
        }

        private void CellIndexNumbering()
        {   
            string strTmpName = String.Empty;
            foreach (DataGridViewRow row in dataGridEdit.Rows)
            {
                row.Height = 30;                
                row.HeaderCell.Value = (row.Index + 1).ToString();
                for (int i = (int)sIndex.TYPE; i <= (int)sIndex.EXPR; i++)
                {
                    if (!CheckColor(i)) dataGridEdit.Rows[row.Index].Cells[i].Style.BackColor = Color.PaleGoldenrod;
                  
                    if (dataGridEdit.Rows[row.Index].Cells[i].Value != null)
                    {
                        strTmpName = dataGridEdit.Rows[row.Index].Cells[i].Value.ToString();
                        GridTextColorChage(row.Index, i, strTmpName);
                    }
                }
            }
        }
        public bool OnJobFile(string[] strData, string strCheckSum, int iCol, ref string strReason)
        {
            
            if (strData.Length < iGrdColCount)
            {
                return false;
            }

            try
            {
                //strData[i]에 체크섬이 있는지 확인한다.

                for (int i = 0; i < iGrdColCount; i++)
                {

                    dataGridEdit[i, iCol].Value = strData[i].ToString();
                    
                }
                
                dataGridEdit.Rows.Add();

            }
            catch
            {   
                return false;
            }

            return true;

        }

        private bool CheckValidation() //파일 저장전 유효성 검사.
        {
            //#1. 셀이 비어있는지 체크.
            int iCellNull = 0;
            foreach (DataGridViewRow fees_row in this.dataGridEdit.Rows)
            {   
                for (int i = 0; i < iGrdColCount; i++)
                {
                    if (fees_row.Cells[i].Value != null && fees_row.Cells[i].Value.ToString().Length > 0)
                    {
                        iCellNull++; //한 열에 비어있는 개수 카운트                        
                    }
                }
                if (iCellNull > 0) //비어있지 않다면
                {
                    for (int i = 0; i < iGrdColCount; i++)
                    {
                        if (fees_row.Cells[i].Value == null || String.IsNullOrEmpty(fees_row.Cells[i].Value.ToString()))
                        {
                            if (!CheckNullData(i))
                            {
                                //dataGridEdit.CurrentCell = fees_row.Cells[i];
                                fees_row.Cells[i].Style.BackColor = Color.MediumVioletRed;
                                return false;
                            }
                        }
                    }
                }
                iCellNull = 0;    
            }

            //#2. LABE 중복 체크 및 아이템 코드가 중복인것이 있는지 검사.!

            List<string> ItemList = new List<string>();


            foreach (DataGridViewRow fees_row in this.dataGridEdit.Rows)
            {
                if (fees_row.Cells[(int)sIndex.MESCODE].Value != null &&
                    fees_row.Cells[(int)sIndex.MESCODE].Value.ToString().Length > 0)
                {
                    if (fees_row.Cells[(int)sIndex.ACTION].Value != null &&
                        (fees_row.Cells[(int)sIndex.ACTION].Value.ToString().Equals("RUN")
                        || fees_row.Cells[(int)sIndex.ACTION].Value.ToString().Equals("ENC")))
                    {

                        string strCode = fees_row.Cells[(int)sIndex.MESCODE].Value.ToString();
                        if (ItemList.Contains(strCode))
                        {
                            MessageBox.Show("Can Not Save! Because MES CODE Duplicate : " + strCode);
                            fees_row.Cells[(int)sIndex.MESCODE].Style.BackColor = Color.MediumVioletRed;
                            return false;
                        }
                        else
                            ItemList.Add(fees_row.Cells[(int)sIndex.MESCODE].Value.ToString());

                    }
                }

            
                if (fees_row.Cells[(int)sIndex.LABEL].Value != null &&
                    fees_row.Cells[(int)sIndex.LABEL].Value.ToString().Length > 0)
                {
                    int iFindCount = 0;
                    foreach (DataGridViewRow grid_row in this.dataGridEdit.Rows)
                    {
                        if (grid_row.Cells[(int)sIndex.LABEL].Value != null &&
                            grid_row.Cells[(int)sIndex.LABEL].Value.ToString().Length > 0)
                        {
                            if (fees_row.Cells[(int)sIndex.LABEL].Value.ToString().Equals(grid_row.Cells[(int)sIndex.LABEL].Value.ToString()))
                            {
                                iFindCount++;
                            }
                        }
                    }

                    if (iFindCount > 1)
                    {
                        MessageBox.Show("Can Not Save! Because Duplicate Label(" + fees_row.Cells[(int)sIndex.LABEL].Value.ToString() + ") Exists On The File. Please Check and Try Again.");
                        fees_row.Cells[(int)sIndex.LABEL].Style.BackColor = Color.MediumVioletRed;
                        return false;
                    }

                }               
            }

            return true;
        }

        private void Fsave_Click(object sender, EventArgs e)
        {
            CellDefaultSize();

            if (!CheckValidation())
            {
                return;
            }
            else
            {
                if (cbJobFiles.Items.Count == 0)
                {
                    cbJobFiles.Text = "";
                    MessageBox.Show("JOB File does not exist.");
                    return;
                }
                if (cbJobFiles.SelectedItem.ToString().Length > 0)
                {
                    DKLogger_EDIT.ClearJob(cbJobFiles.SelectedItem.ToString());
                }
                else
                {
                    MessageBox.Show("JOB File does not exist.");
                    return;
                }
            }
            
            string[] strData = new string[iGrdColCount];

            foreach (DataGridViewRow fees_row in this.dataGridEdit.Rows)
            {   //셀이 비어있는지 체크. 그래야 다음칸에 내용을 채운다.
                var cell = fees_row.Cells[0]; 
                if (cell.Value != null)
                {
                    if (!String.IsNullOrEmpty(cell.Value.ToString()))
                    {
                        var value = cell.Value;
                        if (value != null)
                        {
                            for (int i = 0; i < strData.Length; i++)
                            {
                                if (fees_row.Cells[i].Value != null)
                                {
                                    strData[i] = fees_row.Cells[i].Value.ToString();
                                }
                                else
                                {
                                    strData[i] = "";
                                }
                            }
                            DKLogger_EDIT.SaveJob(cbJobFiles.SelectedItem.ToString(), strData);
                        }
                    }
                }
            }
            DKLogger_EDIT.WriteEditHistory(cbJobFiles.SelectedItem.ToString(), strEditorName, "EDIT-SAVE");
            dgvResetGrid();
        }

        private bool CheckColor(int iVal)
        {
            bool rtnFlag = true;
            switch (iVal)
            {
                case (int)sIndex.TYPE: rtnFlag = false; break;
                case (int)sIndex.CMD: rtnFlag = false; break;
                case (int)sIndex.DISPLAY: break;
                case (int)sIndex.MESCODE: break;
                case (int)sIndex.ACTION: rtnFlag = false; break;
                case (int)sIndex.LABEL: rtnFlag = false; break;
                case (int)sIndex.LABELCOUNT: rtnFlag = false; break;
                case (int)sIndex.CASENG: rtnFlag = false; break;
                case (int)sIndex.DELAY: break;
                case (int)sIndex.TIMEOUT: break;
                case (int)sIndex.RETRY: break;
                case (int)sIndex.COMPARE: rtnFlag = false; break;
                case (int)sIndex.MIN: break;
                case (int)sIndex.MAX: break;
                case (int)sIndex.OPTION: rtnFlag = false; break;
                case (int)sIndex.PAR1: break;
                case (int)sIndex.DOC: break;
                case (int)sIndex.EXPR: break;
                default: rtnFlag = true;
                    break;
            }

            return rtnFlag;
        }

        private bool CheckNullData(int iVal)
        {
            bool rtnFlag = true;
            string tmpMsg = String.Empty;
            switch (iVal)
            {
                case (int)sIndex.TYPE:    rtnFlag = false; tmpMsg = sIndex.TYPE.ToString(); break;
                case (int)sIndex.CMD:     rtnFlag = false; tmpMsg = sIndex.CMD.ToString(); break;
                case (int)sIndex.DISPLAY: break;
                case (int)sIndex.MESCODE: break;
                case (int)sIndex.ACTION:  rtnFlag = false; tmpMsg = sIndex.ACTION.ToString(); break;
                case (int)sIndex.LABEL:    break;
                case (int)sIndex.LABELCOUNT: break;
                case (int)sIndex.CASENG:  rtnFlag = false; tmpMsg = sIndex.CASENG.ToString();  break;
                case (int)sIndex.DELAY:   rtnFlag = false; tmpMsg = sIndex.DELAY.ToString();   break;
                case (int)sIndex.TIMEOUT: rtnFlag = false; tmpMsg = sIndex.TIMEOUT.ToString(); break;
                case (int)sIndex.RETRY:   rtnFlag = false; tmpMsg = sIndex.RETRY.ToString();   break;
                case (int)sIndex.COMPARE: rtnFlag = false; tmpMsg = sIndex.COMPARE.ToString(); break;
                case (int)sIndex.MIN:     break;
                case (int)sIndex.MAX:     break;
                case (int)sIndex.OPTION:  rtnFlag = false; tmpMsg = sIndex.OPTION.ToString(); break;
                case (int)sIndex.PAR1:    break;
                case (int)sIndex.DOC:     break;
                case (int)sIndex.EXPR:    break;
                default: rtnFlag = true; break;
            }
            if (!rtnFlag)
            {
                MessageBox.Show(tmpMsg + " has not been entered. Please, Check and try again.");
            }
            return rtnFlag;
        }


                

#endregion     
   
#region 버튼명령

        private void Fdelete_Click(object sender, EventArgs e)
        {
            CellDefaultSize();

            if (cbJobFiles.Items.Count == 0)
            {
                cbJobFiles.Text = "";
                return;
            }


            if (cbJobFiles.SelectedItem.ToString().Length > 0)
            {
                if (MessageBox.Show("DO YOU WANT TO DELETE THIS FILE[ " +
                            cbJobFiles.SelectedItem.ToString() + " ] ?",
                            "CONFIRM MESSAGE", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DKLogger_EDIT.WriteEditHistory(cbJobFiles.SelectedItem.ToString(), strEditorName, "FILE-DELETE");
                    DKLogger_EDIT.DeleteJob(cbJobFiles.SelectedItem.ToString());
                    ComboListUpdate(0);
                }

                return;
            }
            else
            {
                MessageBox.Show("NOTHING FILE!");
                return;
            }


        }

        private void Fnew_Click(object sender, EventArgs e)
        {
            CellDefaultSize();

            string sFileName = String.Empty;

            //Prevent 2015.03.26 DK.SIM 
            string[] files = DKLogger_EDIT.GetFileList("JOB");
            FrmFileName frmFN = null;
            frmFN = new FrmFileName(files, "", (int)FILESAVE.CREATE);

            if (frmFN != null && frmFN.ShowDialog() == DialogResult.OK)
            {
                sFileName = frmFN.GetFileName + ".JOB";
            }

            else
            {
                frmFN.Dispose();
                return;
            }

            if (!DKLogger_EDIT.NewJob(sFileName))
            {
                MessageBox.Show("Error : Already Job File Name!");
                return;
            }

            ComboListUpdate(0);
            DKLogger_EDIT.WriteEditHistory(sFileName, strEditorName, "FILE-NEW");
            cbJobFiles.SelectedIndex = cbJobFiles.FindString(sFileName);

            //cbJobFiles.Text = sFileName;


        }

        private void FSaveAs_Click(object sender, EventArgs e)
        {
            CellDefaultSize();

            string strOrginName = cbJobFiles.SelectedItem.ToString();
            if (!CheckValidation())
            {
                return;
            }
            else
            {
                if (cbJobFiles.Items.Count == 0)
                {
                    cbJobFiles.Text = "";
                    MessageBox.Show("JOB File does not exist.");
                    return;
                }

            }

            string sFileName = String.Empty;

            //Prevent 2015.03.26 DK.SIM 
            string[] files = DKLogger_EDIT.GetFileList("JOB");
            FrmFileName frmFN = null;
            frmFN = new FrmFileName(files, strOrginName, (int)FILESAVE.SAVEAS);

            if (frmFN != null && frmFN.ShowDialog() == DialogResult.OK)
            {
                sFileName = frmFN.GetFileName + ".JOB";
            }

            else
            {
                frmFN.Dispose();
                return;
            }

            if (!DKLogger_EDIT.CopyCheck(strOrginName, sFileName))
            {
                MessageBox.Show("Error : File Name Exist!");
                return;
            }
            

            ComboListUpdate(0);
            DKLogger_EDIT.WriteEditHistory(sFileName, strEditorName, "FILE-SAVE(AS)");
            cbJobFiles.SelectedIndex = cbJobFiles.FindString(sFileName);

            //cbJobFiles.Text = sFileName;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

#endregion

#region 그리드 메뉴 관련

        private void TableInit(ref List<TBLDATA0> lstTable, ref ToolStripMenuItem[] tsMenu)
        {
            int iTmpCount = 0;            
            for (int i = 0; i < lstTable.Count; i++)
            {
                if (lstTable[i].CMDNAME.Contains("#")) iTmpCount++;
            }
            tsMenu = new ToolStripMenuItem[iTmpCount];
        }

        private void TableInit2(ref List<DTCDATA0> lstTable, ref ToolStripMenuItem[] tsMenu)
        {
            int iTmpCount = 0;
            for (int i = 0; i < lstTable.Count; i++)
            {
                if (lstTable[i].DTCNAME.Contains("#")) iTmpCount++;
            }
            tsMenu = new ToolStripMenuItem[iTmpCount];
        }

        private void TableSort(ref List<TBLDATA0> lstTable, ref ToolStripMenuItem[] tsMenu)
        {
            string strTmpName = String.Empty;
            int iTmpCount = 0;
            int j = -1;
            int ix = 0;
            for (int i = 0; i < lstTable.Count; i++)
            {
                if (lstTable[i].CMDNAME.Contains("#"))
                {
                    strTmpName = lstTable[i].CMDNAME.Replace("#", "");
                    tsMenu[iTmpCount] = new ToolStripMenuItem(strTmpName);
                    iTmpCount++;
                    j++;
                    ix = 0;
                }
                else
                {
                    //if (j < 0) j = 0;
                    tsMenu[j].DropDownItems.Add(lstTable[i].CMDNAME);
                    tsMenu[j].DropDownItems[ix].Click += new EventHandler(GridClickMenuHandler);
                    ix++;
                }
            }

        }

        private void TableDTCSort(ref List<DTCDATA0> lstTable, ref ToolStripMenuItem[] tsMenu)
        {
            string strTmpName = String.Empty;
            int iTmpCount = 0;
            int j = -1;
            int ix = 0;
            for (int i = 0; i < lstTable.Count; i++)
            {
                if (lstTable[i].DTCNAME.Contains("#"))
                {
                    strTmpName = lstTable[i].DTCNAME.Replace("#", "");
                    tsMenu[iTmpCount] = new ToolStripMenuItem(strTmpName);
                    iTmpCount++;
                    j++;
                    ix = 0;
                }
                else
                {
                    //if (j < 0) j = 0;
                    tsMenu[j].DropDownItems.Add(lstTable[i].DTCNAME);
                    tsMenu[j].DropDownItems[ix].Click += new EventHandler(GridClickMenuHandler);
                    ix++;
                }
            }
        }
        
        private void MakeMenu()
        {
            // 테스트를 위한 샘플 메뉴
            pMenu = new ContextMenuStrip();
            mnuType = new ToolStripMenuItem("TYPE");
            mnuTool[0] = new ToolStripMenuItem("COPY");
            mnuTool[1] = new ToolStripMenuItem("CUT");
            mnuTool[2] = new ToolStripMenuItem("PASTE");
            mnuTool[3] = new ToolStripMenuItem("DELETE");
            mnuTool[4] = new ToolStripMenuItem("ADD LINE");
            mnuTool[5] = new ToolStripMenuItem("FILE RELOAD");
            mnuTool[6] = new ToolStripMenuItem("SET SKIP");
            mnuTool[7] = new ToolStripMenuItem("SET RUN");
            mnuTool[8] = new ToolStripMenuItem("COPY DISPLAY NAME");


            //ADD LINE
            for (int i = 0; i < 10; i++)
            {
                mnuTool[4].DropDownItems.Add((i + 1).ToString());
                mnuTool[4].DropDownItems[i].Click += new EventHandler(GridAddClickEventHandler);
            }
            
            //TYPE
            mnuType.DropDownItems.Add(devDIO);
            mnuType.DropDownItems.Add("PAGE");
            mnuType.DropDownItems.Add(devGEN9);
            mnuType.DropDownItems.Add(devGEN10);
            mnuType.DropDownItems.Add(devGEN11);
            mnuType.DropDownItems.Add(devGEN11P);
            mnuType.DropDownItems.Add(devGEN12);
            mnuType.DropDownItems.Add(devTCP);
            mnuType.DropDownItems.Add(devCCM);
            mnuType.DropDownItems.Add(devNAD);
            mnuType.DropDownItems.Add(devATT);
            mnuType.DropDownItems.Add(devMCTM);
            mnuType.DropDownItems.Add("GMES");
            mnuType.DropDownItems.Add("MES");
            mnuType.DropDownItems.Add("OOB");
            mnuType.DropDownItems.Add("EXCEL");            
            mnuType.DropDownItems.Add(devADC);
            mnuType.DropDownItems.Add(devAudio);
            mnuType.DropDownItems.Add(dev5515C);
            mnuType.DropDownItems.Add(devMTP200);
            mnuType.DropDownItems.Add(dev34410A);
            mnuType.DropDownItems.Add(devTC3000);
            mnuType.DropDownItems.Add(devTC1400A);
            mnuType.DropDownItems.Add(devPCAN);
            mnuType.DropDownItems.Add(devVECTOR);
            mnuType.DropDownItems.Add("SCAN");
            mnuType.DropDownItems.Add(devODAPWR);
            mnuType.DropDownItems.Add(devKEITHLEY);
            mnuType.DropDownItems.Add(devDLLGATE);
            mnuType.DropDownItems.Add(devMELSEC);
            mnuType.DropDownItems.Add(devMTP120A);

            for (int i = 0; i < mnuType.DropDownItems.Count; i++)
            {
                mnuType.DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);
            }
            //TYPE - PAGE
            mnuScan[0] = new ToolStripMenuItem("TRIGGER_ON");
            mnuScan[0].Click += new EventHandler(GridClickMenuHandler);
            mnuScan[1] = new ToolStripMenuItem("TRIGGER_OFF");
            mnuScan[1].Click += new EventHandler(GridClickMenuHandler);
            mnuScan[2] = new ToolStripMenuItem("HONEYWELL_ON");
            mnuScan[2].Click += new EventHandler(GridClickMenuHandler);
            mnuScan[3] = new ToolStripMenuItem("HONEYWELL_OFF");
            mnuScan[3].Click += new EventHandler(GridClickMenuHandler);            

            //TYPE - PAGE
            mnuPage[0] = new ToolStripMenuItem("DELAY");
            mnuPage[0].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[1] = new ToolStripMenuItem("MESSAGE_POPUP");
            mnuPage[1].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[2] = new ToolStripMenuItem("CHANGE_JOB");
            mnuPage[2].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[3] = new ToolStripMenuItem("CHANGE_SET_BAUDRATE");
            mnuPage[3].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[4] = new ToolStripMenuItem("CHANGE_SET_COMPORT");
            mnuPage[4].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[5] = new ToolStripMenuItem("PATH_ATCO_SYSTEM");
            mnuPage[5].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[6] = new ToolStripMenuItem("LOAD_MFG_FILE");
            mnuPage[6].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[7] = new ToolStripMenuItem("TIMER_START");
            mnuPage[7].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[8] = new ToolStripMenuItem("TIMER_STOP");
            mnuPage[8].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[9] = new ToolStripMenuItem("TIMER_VALUE");
            mnuPage[9].Click += new EventHandler(GridClickMenuHandler);     
            mnuPage[10] = new ToolStripMenuItem("NETWORK_PING_TTL");
            mnuPage[10].Click += new EventHandler(GridClickMenuHandler);     
            mnuPage[11] = new ToolStripMenuItem("NETWORK_PING_TIME");
            mnuPage[11].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[12] = new ToolStripMenuItem("OPEN_UART2");
            mnuPage[12].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[13] = new ToolStripMenuItem("CLOSE_UART2");
            mnuPage[13].Click += new EventHandler(GridClickMenuHandler);     
            mnuPage[14] = new ToolStripMenuItem("UART2_RTS_XONOFF");
            mnuPage[14].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[15] = new ToolStripMenuItem("BOOT_DELAY");
            mnuPage[15].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[16] = new ToolStripMenuItem("EXPR_DATA_VIEW");
            mnuPage[16].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[17] = new ToolStripMenuItem("SHIFT_TIME");
            mnuPage[17].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[18] = new ToolStripMenuItem("CHECK_FILE_SIZE");
            mnuPage[18].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[19] = new ToolStripMenuItem("DOCUMENT_FILE_LINK");
            mnuPage[19].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[20] = new ToolStripMenuItem("DOCUMENT_ITEM_VIEW");
            mnuPage[20].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[21] = new ToolStripMenuItem("GET_PEPU_PASSWORD");
            mnuPage[21].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[22] = new ToolStripMenuItem("CHECK_TCP_SEED_FILE");
            mnuPage[22].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[23] = new ToolStripMenuItem("CHECK_VCP_SEED_FILE");
            mnuPage[23].Click += new EventHandler(GridClickMenuHandler);

            mnuPage[24] = new ToolStripMenuItem("CHECK_TCP_SEED_FILE_MANUAL");
            mnuPage[24].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[25] = new ToolStripMenuItem("CHECK_VCP_SEED_FILE_MANUAL");
            mnuPage[25].Click += new EventHandler(GridClickMenuHandler);

            mnuPage[26] = new ToolStripMenuItem("MAKE_PARAMETERS");
            mnuPage[26].Click += new EventHandler(GridClickMenuHandler);
            //LGEVH 명령어추가
            mnuPage[27] = new ToolStripMenuItem("JOB_MODEL_CHECK");
            mnuPage[27].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[28] = new ToolStripMenuItem("SAVE_WIPID");
            mnuPage[28].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[29] = new ToolStripMenuItem("JUMP_CHECK");
            mnuPage[29].Click += new EventHandler(GridClickMenuHandler);
           
            mnuPage[30] = new ToolStripMenuItem("GET_GEN12_CERT_MANUAL_ASCII");
            mnuPage[30].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[31] = new ToolStripMenuItem("GET_GEN12_CERT_MANUAL_HEX");
            mnuPage[31].Click += new EventHandler(GridClickMenuHandler);
            mnuPage[32] = new ToolStripMenuItem("DATA_PARSING_ASCII2HEX");
            mnuPage[32].Click += new EventHandler(GridClickMenuHandler);


            //TYPE - OOB
            mnuOOB[0] = new ToolStripMenuItem("LOADING_LABEL");
            mnuOOB[0].Click += new EventHandler(GridClickMenuHandler);            
            mnuOOB[1] = new ToolStripMenuItem("GET_DATA_FIELD_WORD");
            mnuOOB[1].Click += new EventHandler(GridClickMenuHandler);
            mnuOOB[2] = new ToolStripMenuItem("GET_DATA_FIELD_HEX");
            mnuOOB[2].Click += new EventHandler(GridClickMenuHandler);
            mnuOOB[3] = new ToolStripMenuItem("LOADING_LABEL_FOR_MCTM");
            mnuOOB[3].Click += new EventHandler(GridClickMenuHandler);
            mnuOOB[4] = new ToolStripMenuItem("GET_MCTM_LABEL_FIELD_WORD");
            mnuOOB[4].Click += new EventHandler(GridClickMenuHandler);
            mnuOOB[5] = new ToolStripMenuItem("GET_MCTM_LABEL_FIELD_HEX");
            mnuOOB[5].Click += new EventHandler(GridClickMenuHandler);
            mnuOOB[6] = new ToolStripMenuItem("LOADING_LABEL_FOR_GEN12");
            mnuOOB[6].Click += new EventHandler(GridClickMenuHandler);

            //TYPE - GMES
            mnuGmes[0] = new ToolStripMenuItem("STEP_CHECK");
            mnuGmes[0].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[1] = new ToolStripMenuItem("STEP_COMPLETE");
            mnuGmes[1].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[2] = new ToolStripMenuItem("STEP_COMPLETE_DETAIL");
            mnuGmes[2].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[3] = new ToolStripMenuItem("PACK_STEP_CHECK");
            mnuGmes[3].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[4] = new ToolStripMenuItem("ITEM_DATA_VIEW");
            mnuGmes[4].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[5] = new ToolStripMenuItem("KALS_GET_KEY");
            mnuGmes[5].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[6] = new ToolStripMenuItem("KALS_GET_KEY_KS");
            mnuGmes[6].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[7] = new ToolStripMenuItem("KALS_GET_KEY_NV");
            mnuGmes[7].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[8] = new ToolStripMenuItem("KALS_RETURN_WRITE_INFO");
            mnuGmes[8].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[9] = new ToolStripMenuItem("KALS_WRTIE_PRODUC_INFO");
            mnuGmes[9].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[10] = new ToolStripMenuItem("KALS_DOWNLOAD_FILENAME");
            mnuGmes[10].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[11] = new ToolStripMenuItem("KIS_KEY_DOWNLOAD");
            mnuGmes[11].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[12] = new ToolStripMenuItem("KIS_KEY_DOWNLOAD_MANUAL");
            mnuGmes[12].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[13] = new ToolStripMenuItem("KIS_KEY_DOWNLOAD_NONZERO");
            mnuGmes[13].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[14] = new ToolStripMenuItem("KIS_DATA_VIEW");
            mnuGmes[14].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[15] = new ToolStripMenuItem("AMS_KEY_DOWNLOAD");
            mnuGmes[15].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[16] = new ToolStripMenuItem("AMS_KEY_DOWNLOAD_GEN12");
            mnuGmes[16].Click += new EventHandler(GridClickMenuHandler);
            mnuGmes[17] = new ToolStripMenuItem("AMS_DATA_VIEW");
            mnuGmes[17].Click += new EventHandler(GridClickMenuHandler);

    
            //TYPE - ORACLE
            mnuOracle[0] = new ToolStripMenuItem("STEP_CHECK");
            mnuOracle[0].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[1] = new ToolStripMenuItem("STEP_COMPLETE");
            mnuOracle[1].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[2] = new ToolStripMenuItem("DATA_VIEW");
            mnuOracle[2].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[3] = new ToolStripMenuItem("GET_MODEL_INFO");
            mnuOracle[3].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[4] = new ToolStripMenuItem("GET_PCB_INFO");
            mnuOracle[4].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[5] = new ToolStripMenuItem("GET_KEYWRITE_MAIN");
            mnuOracle[5].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[6] = new ToolStripMenuItem("GET_KEYWRITE_MAIN_TCP");
            mnuOracle[6].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[7] = new ToolStripMenuItem("GET_KEYWRITE_MAIN_PSA");
            mnuOracle[7].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[8] = new ToolStripMenuItem("SET_KEYWRITE_MAIN");
            mnuOracle[8].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[9] = new ToolStripMenuItem("GET_OOB_INFO");
            mnuOracle[9].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[10] = new ToolStripMenuItem("GET_OOB_INFO_PSA");
            mnuOracle[10].Click += new EventHandler(GridClickMenuHandler);
            mnuOracle[11] = new ToolStripMenuItem("SET_OOB_INFO");
            mnuOracle[11].Click += new EventHandler(GridClickMenuHandler);            

            //TYPE - EXCEL
            mnuExcel[0] = new ToolStripMenuItem("TARGET_FILENAME");
            mnuExcel[0].Click += new EventHandler(GridClickMenuHandler);
            mnuExcel[1] = new ToolStripMenuItem("DISPLAY_BARCODE");
            mnuExcel[1].Click += new EventHandler(GridClickMenuHandler);
            mnuExcel[2] = new ToolStripMenuItem("DISPLAY_BARCODE_SUB");
            mnuExcel[2].Click += new EventHandler(GridClickMenuHandler);
            mnuExcel[3] = new ToolStripMenuItem("FIND_FIELD");
            mnuExcel[3].Click += new EventHandler(GridClickMenuHandler);
            mnuExcel[4] = new ToolStripMenuItem("LOAD_DATA");
            mnuExcel[4].Click += new EventHandler(GridClickMenuHandler);

            //TYPE - TBL(GEN9)  
            TableInit(ref TBL_GEN9, ref mnuTBL_GEN9);            
            //TYPE - TBL(GEN10)  
            TableInit(ref TBL_GEN10, ref mnuTBL_GEN10);            
            //TYPE - TBL(TC3000)           
            TableInit(ref TBL_TC3000, ref mnuTBL_TC3000);
            //TYPE - TC1400A
            TableInit(ref TBL_TC1400A, ref mnuTBL_TC1400A);
            //TYPE - TBL(MTP200)           
            TableInit(ref TBL_MTP200, ref mnuTBL_MTP200);            
            //TYPE - TBL(ODA POWER)           
            TableInit(ref TBL_ODAPWR, ref mnuTBL_ODAPWR);
            //TYPE - TBL(TCP)           
            TableInit(ref TBL_TCP, ref mnuTBL_TCP);
            //TYPE - TBL(GEN11)           
            TableInit(ref TBL_GEN11, ref mnuTBL_GEN11);
            //TYPE - TBL(GEN11P)           
            TableInit(ref TBL_GEN11P, ref mnuTBL_GEN11P);
            //TYPE - TBL(GEN12)
            TableInit(ref TBL_GEN12, ref mnuTBL_GEN12);
            //TYPE - TBL(CCM)           
            TableInit(ref TBL_CCM, ref mnuTBL_CCM);
            //TYPE - TBL(NAD)           
            TableInit(ref TBL_NAD, ref mnuTBL_NAD);
            //TYPE - TBL(MCTM)           
            TableInit(ref TBL_MCTM, ref mnuTBL_MCTM);
            //TYPE - TBL(ATT)           
            TableInit(ref TBL_ATT, ref mnuTBL_ATT);
            //TYPE - TBL(NAD)           
            TableInit(ref TBL_NAD, ref mnuTBL_NAD);                        
            //TYPE _ DIO BENCH
            TableInit(ref TBL_DIO, ref mnuTBL_DIO);            
            //TYPE _ AUDIO DIO
            TableInit(ref TBL_AUDIO, ref mnuTBL_AUDIO);
            //TYPE _ ADC MODULE
            TableInit(ref TBL_ADC, ref mnuTBL_ADC);            
            //TYPE _ 5515C
            TableInit(ref TBL_5515C, ref mnuTBL_5515C);            
            //TYPE - 34410A
            TableInit(ref TBL_34410A, ref mnuTBL_34410A);
            //TYPE - PCAN
            TableInit(ref TBL_PCAN, ref mnuTBL_PCAN);               
            //TYPE - VECTOR
            TableInit(ref TBL_VECTOR, ref mnuTBL_VECTOR);
            //TYPE _ KEITHLEY
            TableInit(ref TBL_KEITHLEY, ref mnuTBL_KEITHLEY);

            //TYPE - DTC_GEN10
            TableInit2(ref TBL_DTC_GEN9, ref mnuTBL_DTC_GEN9);                                       
            //TYPE - DTC_GEN10
            TableInit2(ref TBL_DTC_GEN10, ref mnuTBL_DTC_GEN10);                                       
            //TYPE - DTC_TCP
            TableInit2(ref TBL_DTC_TCP, ref mnuTBL_DTC_TCP);  
            //TYPE - DTC_GEN11
            TableInit2(ref TBL_DTC_GEN11, ref mnuTBL_DTC_GEN11);
            //TYPE - DTC_GEN12
            TableInit2(ref TBL_DTC_GEN12, ref mnuTBL_DTC_GEN12);
            //TYPE - DTC_MCTM
            TableInit2(ref TBL_DTC_MCTM, ref mnuTBL_DTC_MCTM);

            //TYPE - DLL GATE
            TableInit(ref TBL_DLLGATE, ref mnuTBL_DLLGATE);
            //TYPE - MELSEC
            TableInit(ref TBL_MELSEC, ref mnuTBL_MELSEC);

            //TYPE - MTP120A
            TableInit(ref TBL_MTP120A, ref mnuTBL_MTP120A);

            //DISPLAY NAME
            mnuDisplay = new ToolStripMenuItem("Default DISPLAY NAME");
            mnuDisplay.Click += new EventHandler(GridClickDefaultDisplayname);

            //LABEL MENU
            mnuLabel[0] = new ToolStripMenuItem("CLEAR");
            mnuLabel[0].Click += new EventHandler(GridClickMenuHandler);
            mnuLabel[1] = new ToolStripMenuItem("LABEL_STOP");
            mnuLabel[2] = new ToolStripMenuItem("LABEL_CONTINUE");
            mnuLabel[3] = new ToolStripMenuItem("LABEL_JUMP_OK");
            mnuLabel[4] = new ToolStripMenuItem("LABEL_JUMP_FAIL");

            for (int i = 0; i < (int)JOBLABEL.A20; i++)
            {
                mnuLabel[1].DropDownItems.Add((JOBLABEL.A1 + i).ToString());
                mnuLabel[1].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);
            }

            for (int i = 0; i < (int)JOBLABEL.A20; i++)
            {
                mnuLabel[2].DropDownItems.Add((JOBLABEL.C1 + i).ToString());
                mnuLabel[2].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);
            }

            for (int i = 0; i < (int)JOBLABEL.A20; i++)
            {
                mnuLabel[3].DropDownItems.Add((JOBLABEL.P1 + i).ToString());
                mnuLabel[3].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);
            }

            for (int i = 0; i < (int)JOBLABEL.A20; i++)
            {
                mnuLabel[4].DropDownItems.Add((JOBLABEL.F1 + i).ToString());
                mnuLabel[4].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);
            }

            //LABEL COUNT MENU
            mnuLabelCount[0] = new ToolStripMenuItem("1");
            mnuLabelCount[1] = new ToolStripMenuItem("2");
            mnuLabelCount[2] = new ToolStripMenuItem("3");
            mnuLabelCount[3] = new ToolStripMenuItem("5");
            mnuLabelCount[4] = new ToolStripMenuItem("10");
            mnuLabelCount[5] = new ToolStripMenuItem("15");
            mnuLabelCount[6] = new ToolStripMenuItem("20");
            mnuLabelCount[7] = new ToolStripMenuItem("30");

            for (int i = 0; i < mnuLabelCount.Length; i++)
            {
                mnuLabelCount[i].Click += new EventHandler(GridClickMenuHandler);
            }

            //NAD TABLE COMMAND
            TableSort(ref TBL_NAD, ref mnuTBL_NAD);
            //GEN9 TABLE COMMAND
            TableSort(ref TBL_GEN9, ref mnuTBL_GEN9);
            //GEN10 TABLE COMMAND
            TableSort(ref TBL_GEN10, ref mnuTBL_GEN10);
            //TC3000 TABLE COMMAND
            TableSort(ref TBL_TC3000, ref mnuTBL_TC3000);
            //TC1400A
            TableSort(ref TBL_TC1400A, ref mnuTBL_TC1400A);
            //MTP200 TABLE COMMAND
            TableSort(ref TBL_MTP200, ref mnuTBL_MTP200);
            //ODA POWER TABLE COMMAND
            TableSort(ref TBL_ODAPWR, ref mnuTBL_ODAPWR);
            //TCP TABLE COMMAND
            TableSort(ref TBL_TCP, ref mnuTBL_TCP);
            //GEN11 TABLE COMMAND
            TableSort(ref TBL_GEN11, ref mnuTBL_GEN11);
            //GEN11P TABLE COMMAND
            TableSort(ref TBL_GEN11P, ref mnuTBL_GEN11P);
            //GEN12 TABLE COMMAND
            TableSort(ref TBL_GEN12, ref mnuTBL_GEN12);
            //CCM TABLE COMMAND
            TableSort(ref TBL_CCM, ref mnuTBL_CCM);
            //NAD TABLE COMMAND
            TableSort(ref TBL_NAD, ref mnuTBL_NAD);
            //CCM TABLE COMMAND
            TableSort(ref TBL_MCTM, ref mnuTBL_MCTM);
            //ATT TABLE COMMAND
            TableSort(ref TBL_ATT, ref mnuTBL_ATT);  
            //DIO TABLE COMMAND
            TableSort(ref TBL_DIO, ref mnuTBL_DIO);
            //AUDIO TABLE COMMAND
            TableSort(ref TBL_AUDIO, ref mnuTBL_AUDIO);
            //ADC TABLE COMMAND
            TableSort(ref TBL_ADC, ref mnuTBL_ADC);
            //5515C TABLE COMMAND
            TableSort(ref TBL_5515C, ref mnuTBL_5515C);            
            //PCAN
            TableSort(ref TBL_PCAN, ref mnuTBL_PCAN);
            //VECTOR
            TableSort(ref TBL_VECTOR, ref mnuTBL_VECTOR);
            //KEITHLEY TABLE COMMAND
            TableSort(ref TBL_KEITHLEY, ref mnuTBL_KEITHLEY);
            //DTC_GEN9
            TableDTCSort(ref TBL_DTC_GEN9, ref mnuTBL_DTC_GEN9);
            //DTC_GEN10
            TableDTCSort(ref TBL_DTC_GEN10, ref mnuTBL_DTC_GEN10);
            //DTC_TCP
            TableDTCSort(ref TBL_DTC_TCP, ref mnuTBL_DTC_TCP);
            //DTC_GEN11
            TableDTCSort(ref TBL_DTC_GEN11, ref mnuTBL_DTC_GEN11);
            //DTC_GEN12
            TableDTCSort(ref TBL_DTC_GEN12, ref mnuTBL_DTC_GEN12);
            //DTC_GEN11
            TableDTCSort(ref TBL_DTC_MCTM, ref mnuTBL_DTC_MCTM);

            //34410A
            TableSort(ref TBL_34410A, ref mnuTBL_34410A);

            //DLLGATE
            TableSort(ref TBL_DLLGATE, ref mnuTBL_DLLGATE);

            //MELSEC TABLE COMMAND
            TableSort(ref TBL_MELSEC, ref mnuTBL_MELSEC);

            //TYPE - MTP120A
            TableSort(ref TBL_MTP120A, ref mnuTBL_MTP120A);

            mnuOption[0] = new ToolStripMenuItem("SENDRECV");
            mnuOption[1] = new ToolStripMenuItem("SEND");
            mnuOption[2] = new ToolStripMenuItem("RECV");
            mnuOption[3] = new ToolStripMenuItem("BUFFER");
            mnuOption[4] = new ToolStripMenuItem("UNTIL");
            mnuOption[5] = new ToolStripMenuItem("AVERAGE");
            mnuOption[6] = new ToolStripMenuItem("RECVSEND");
            mnuOption[7] = new ToolStripMenuItem("MULTIPLE");
            mnuOption[8] = new ToolStripMenuItem("NORESPONSE");
            mnuOption[9] = new ToolStripMenuItem("HIDDEN");
   
            mnuCaseNG[0] = new ToolStripMenuItem("CONTINUE");
            mnuCaseNG[1] = new ToolStripMenuItem("STOP");
            mnuCaseNG[2] = new ToolStripMenuItem("PAUSE");
            mnuCaseNG[3] = new ToolStripMenuItem("CHECK");
            //mnuCaseNG[4] = new ToolStripMenuItem("IGNORE");
            mnuCaseNG[4] = new ToolStripMenuItem("GOTO_STOP");
            mnuCaseNG[5] = new ToolStripMenuItem("GOTO_CONTINUE");
            mnuCaseNG[6] = new ToolStripMenuItem("JUMP_OK");
            mnuCaseNG[7] = new ToolStripMenuItem("JUMP_NG");            
            mnuCaseNG[8] = new ToolStripMenuItem("EMPTY");
            mnuCaseNG[9] = new ToolStripMenuItem("MES");
            mnuCaseNG[10] = new ToolStripMenuItem("ERROR");
            mnuCaseNG[11] = new ToolStripMenuItem("MONITOR");

            //LABEL MENU
            ToolStripMenuItem toolTmp;
            for (int i = 0; i < (int)JOBLABEL.A20; i++)
            {
                mnuCaseNG[4].DropDownItems.Add((JOBLABEL.A1 + i).ToString());
                mnuCaseNG[4].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);
                mnuCaseNG[5].DropDownItems.Add((JOBLABEL.C1 + i).ToString());
                mnuCaseNG[5].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);

                toolTmp = new ToolStripMenuItem();
                toolTmp.Text = (JOBLABEL.P1 + i).ToString();
                toolTmp.Tag = mnuCaseNG[6].Text;
                mnuCaseNG[6].DropDownItems.Add(toolTmp);
                mnuCaseNG[6].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);

                toolTmp = new ToolStripMenuItem();
                toolTmp.Text = (JOBLABEL.F1 + i).ToString();
                toolTmp.Tag = mnuCaseNG[7].Text;
                mnuCaseNG[7].DropDownItems.Add(toolTmp);
                mnuCaseNG[7].DropDownItems[i].Click += new EventHandler(GridClickMenuHandler);

            }

            mnuCompare[0] = new ToolStripMenuItem("NONE");
            mnuCompare[1] = new ToolStripMenuItem("NUMBER");
            mnuCompare[2] = new ToolStripMenuItem("WORD");
            mnuCompare[3] = new ToolStripMenuItem("TEXT");
            mnuCompare[4] = new ToolStripMenuItem("CONTAIN");
            mnuCompare[5] = new ToolStripMenuItem("CONTAIN2");            
            mnuCompare[6] = new ToolStripMenuItem("DIFFERENT");            
            mnuCompare[7] = new ToolStripMenuItem("LENGTH");
            mnuCompare[8] = new ToolStripMenuItem("PATTERN");
            mnuCompare[9] = new ToolStripMenuItem("PATTERNS");
            mnuCompare[10] = new ToolStripMenuItem("NOTPATTERN");
            mnuCompare[11] = new ToolStripMenuItem("TRIM");
            mnuCompare[12] = new ToolStripMenuItem("ETC");
            mnuCompare[13] = new ToolStripMenuItem("OR");
            mnuCompare[14] = new ToolStripMenuItem("EVENONE");
            mnuCompare[15] = new ToolStripMenuItem("INDEXOF");
            mnuCompare[16] = new ToolStripMenuItem("NONASCII");
            mnuCompare[17] = new ToolStripMenuItem("RESULTCODE");
            mnuCompare[18] = new ToolStripMenuItem("TIMESTAMP");
            mnuCompare[19] = new ToolStripMenuItem("UNDERCOVERAGE");
            mnuCompare[20] = new ToolStripMenuItem("UNDERCOVERAGE2");
            mnuCompare[21] = new ToolStripMenuItem("NONECOLON");            
          
            mnuAction[0] = new ToolStripMenuItem("RUN");
            mnuAction[1] = new ToolStripMenuItem("SKIP");
            mnuAction[2] = new ToolStripMenuItem("ENC");       //MEAS/MIN/MAX 암호화. 

            for (int i = 0; i < mnuTool.Length; i++)
            {
                if (i != 4) { mnuTool[i].Click += new EventHandler(GridClickEventHandler); }
            }

            for (int i = 0; i < mnuOption.Length; i++)
            {
                mnuOption[i].Click += new EventHandler(GridClickMenuHandler);
            }

            for (int i = 0; i < mnuCaseNG.Length; i++)
            {
                mnuCaseNG[i].Click += new EventHandler(GridClickMenuHandler);
            }

            for (int i = 0; i < mnuCompare.Length; i++)
            {
                mnuCompare[i].Click += new EventHandler(GridClickMenuHandler);
            }

            for (int i = 0; i < mnuAction.Length; i++)
            {
                mnuAction[i].Click += new EventHandler(GridClickMenuHandler);
            }

            dataGridEdit.ContextMenuStrip = pMenu;
        }

        private void GridAddClickEventHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem tempItem = sender as ToolStripMenuItem;

            dgvDataCellAdd(int.Parse(tempItem.Text));            

        }

        private void GridClickDefaultDisplayname(object sender, EventArgs e)
        {
            if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value != null &&
                dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString().Length > 0)
            {
                if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.CMD].Value != null &&
                dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.CMD].Value.ToString().Length > 0)
                {
                    string tmpString = dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.CMD].Value.ToString();
                    ContextMenuViewDisplayName(tmpString);                    
                }                
            }            
        }

        private void GridTextColorChage(int iRow, int iCol, string strName)
        {
            bool bOn = false;
            string strKeyWord = String.Empty;
            switch (iCol)
            {
                case (int)sIndex.ACTION: strKeyWord = "SKIP"; break;
                default: return;
            }
            if (strKeyWord.Equals(strName)) bOn = true;


            if (bOn) dataGridEdit.Rows[iRow].Cells[iCol].Style.ForeColor = Color.Crimson;
            else dataGridEdit.Rows[iRow].Cells[iCol].Style.ForeColor = Color.Black;
                        
        }

        private void GridColumnClear()
        {
            int iRow = dataGridEdit.CurrentCell.RowIndex;
            for (int iCol = (int)sIndex.CMD; iCol <= (int)sIndex.EXPR; iCol++)
            {
                dataGridEdit.Rows[iRow].Cells[iCol].Value = String.Empty;
            }

            for (int iCol = (int)sIndex.DISPLAY; iCol <= (int)sIndex.EXPR; iCol++)
            {
                switch (iCol)
                {
                    case (int)sIndex.ACTION:   dataGridEdit.Rows[iRow].Cells[iCol].Value = mnuAction[0].Text; break;
                    case (int)sIndex.CASENG:   dataGridEdit.Rows[iRow].Cells[iCol].Value = mnuCaseNG[0].Text; break;
                    case (int)sIndex.DELAY:    dataGridEdit.Rows[iRow].Cells[iCol].Value = "0"; break;
                    case (int)sIndex.TIMEOUT:  dataGridEdit.Rows[iRow].Cells[iCol].Value = "3"; break;
                    case (int)sIndex.RETRY:    dataGridEdit.Rows[iRow].Cells[iCol].Value = "3"; break;
                    case (int)sIndex.COMPARE:  dataGridEdit.Rows[iRow].Cells[iCol].Value = mnuCompare[0].Text; break;
                    case (int)sIndex.OPTION:   dataGridEdit.Rows[iRow].Cells[iCol].Value = mnuOption[0].Text; break;

                    default: break;
                }
            }
        }

        private void CheckTestCommDevice() //테스콤회사 계측기류는 디폴트 응답(OK) 과 디폴트 딜레이를 1초 설정한다. 
        {
            if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value != null &&
                (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString().Equals(devTC3000) ||
                 dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString().Equals(devMTP200)))
            {
                int iRow = dataGridEdit.CurrentCell.RowIndex;                
                dataGridEdit.Rows[iRow].Cells[(int)sIndex.COMPARE].Value = mnuCompare[2].Text;
                dataGridEdit.Rows[iRow].Cells[(int)sIndex.DELAY].Value = "1";  
                dataGridEdit.Rows[iRow].Cells[(int)sIndex.MAX].Value     = "OK";          
            }
        }

        private void CheckDefaultStopOption(string strCommandName) //옵션이 기본 스탑해야할 경우
        {

            switch (strCommandName)
            {
                case "DOCUMENT_FILE_LINK":
                    int iRow = dataGridEdit.CurrentCell.RowIndex;
                    dataGridEdit.Rows[iRow].Cells[(int)sIndex.CASENG].Value = "STOP";
                    break;
                default:
                    break;
            }
            
        }

        private void GridClickMenuHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem tempItem = sender as ToolStripMenuItem;

            string tempString = tempItem.Text;

            switch (dataGridEdit.CurrentCell.ColumnIndex)
            {
                case (int)sIndex.CMD: GridColumnClear(); 
                                      ContextMenuViewDisplayName(tempItem.Text.ToString());
                                      CheckTestCommDevice();
                                      CheckDefaultStopOption(tempItem.Text.ToString());
                                      break;
                case (int)sIndex.TYPE: GridColumnClear();  break;

                case (int)sIndex.LABEL:
                    if (tempString == "CLEAR")
                    {
                        dataGridEdit.CurrentCell.Value = string.Empty;
                        dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.LABELCOUNT].Value = string.Empty;
                        return;
                    }

                    if (CheckAlreadyLabel(tempString))
                    {
                        MessageBox.Show("ALREADY LABEL [ " + tempItem.Text + " ] ");
                        return;
                    }

                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.LABELCOUNT].Value = mnuLabelCount[0].Text;
                    UpdateBeforeCellData(dataGridEdit.CurrentCell.RowIndex);
                    break;
                case (int)sIndex.CASENG:
                    if (tempItem.Text.Length == 2 || tempItem.Text.Length == 3)
                    {
                        string ParentText = string.Empty;

                        if (tempItem.Tag != null)
                            ParentText = tempItem.Tag.ToString();

                        if (CheckLabelPos(tempItem.Text, dataGridEdit.CurrentCell.RowIndex))
                        {
                            if (ParentText.StartsWith("JUMP"))
                            {
                                tempString = ParentText + ":" + tempItem.Text;
                            }
                            else
                                tempString = "GOTO:" + tempItem.Text; 
                        }
                        else
                        {
                            if (tempItem.Text.Equals("MES"))
                                tempString = tempItem.Text;
                            else
                            {
                                MessageBox.Show("CAN NOT FIND LABEL [ " + tempItem.Text + " ] "); return;
                            }                 
                        }

                    }
                    else
                    {
                        tempString = tempItem.Text;
                    }          
                    break;

                case (int)sIndex.PAR1:

                    if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value != null &&                            
                        (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN10
                        || dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devTCP
                        || dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN11
                        || dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN12
                        || dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devMCTM) &&
                            dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.CMD].Value != null &&
                            dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.CMD].Value.ToString() == "READ_DTC")
                    {   
                        if(dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN10)
                        {
                            for (int i = 0; i < TBL_DTC_GEN10.Count; i++)
                            {
                                if (tempItem.Text.Equals(TBL_DTC_GEN10[i].DTCNAME))
                                {
                                    tempString = TBL_DTC_GEN10[i].DTCCODE.ToString();
                                    tempString = tempString.ToLower();
                                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.MAX].Value = tempString + ",0,0,0";                                

                                    break;
                                }
                            }
                        }

                        if(dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devTCP)
                        {
                            for (int i = 0; i < TBL_DTC_TCP.Count; i++)
                            {
                                if (tempItem.Text.Equals(TBL_DTC_TCP[i].DTCNAME))
                                {
                                    tempString = TBL_DTC_TCP[i].DTCCODE.ToString();
                                    tempString = tempString.ToLower();
                                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.MAX].Value = tempString + ",0,0,0";                                

                                    break;
                                }
                            }
                        }

                        if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN11)
                        {
                            for (int i = 0; i < TBL_DTC_GEN11.Count; i++)
                            {
                                if (tempItem.Text.Equals(TBL_DTC_GEN11[i].DTCNAME))
                                {
                                    tempString = TBL_DTC_GEN11[i].DTCCODE.ToString();
                                    tempString = tempString.ToLower();
                                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.MAX].Value = tempString + ",0,0,0";

                                    break;
                                }
                            }
                        }

                        if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN12)
                        {
                            for (int i = 0; i < TBL_DTC_GEN12.Count; i++)
                            {
                                if (tempItem.Text.Equals(TBL_DTC_GEN12[i].DTCNAME))
                                {
                                    tempString = TBL_DTC_GEN12[i].DTCCODE.ToString();
                                    tempString = tempString.ToLower();
                                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.MAX].Value = tempString + ",0,0,0";

                                    break;
                                }
                            }
                        }

                        if (dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString() == devMCTM)
                        {
                            for (int i = 0; i < TBL_DTC_MCTM.Count; i++)
                            {
                                if (tempItem.Text.Equals(TBL_DTC_MCTM[i].DTCNAME))
                                {
                                    tempString = TBL_DTC_MCTM[i].DTCCODE.ToString();
                                    tempString = tempString.ToLower();
                                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.MAX].Value = tempString + ",0,0,0";

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        tempString = tempItem.Text;
                    }
                    break;
                default:
                    tempString = tempItem.Text;
                    
                    break;

            }
            dataGridEdit.CurrentCell.Value = tempString;

            UpdateBeforeCellData(dataGridEdit.CurrentCell.RowIndex);
            
        }

        private void GridClickEventHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem tempItem = sender as ToolStripMenuItem;
            //MessageBox.Show(tempItem.Text.ToString());

            switch (tempItem.Text)
            {
                case "COPY":
                    dgvCopyToClipboard();
                    break;
                case "CUT":
                    dgvCopyToClipboard();
                    dgvDataCellRemove();
                    break;
                case "PASTE":
                    dgvPasteClipboardValue();
                    break;
                case "DELETE":
                    dgvDataCellRemove();
                    break;
                case "FILE RELOAD":
                    dgvResetGrid();
                    break;
                case "SET SKIP":
                    dgvDataCellSkip(true);
                    break;
                case "SET RUN":
                    dgvDataCellSkip(false);
                    break;
                case "COPY DISPLAY NAME":
                    dgvDataCellDisplay();
                    break;
                default:
                    MessageBox.Show(tempItem.Text.ToString());

                    break;
            }


        }

        private void ContextMenuViewTool()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuTool.Length; i++)
            {
                pMenu.Items.Add(mnuTool[i]);
            }
        }

        private void ContextMenuViewAction()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuAction.Length; i++)
            {
                pMenu.Items.Add(mnuAction[i]);
            }
        }

        private void ContextMenuViewLabel()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuLabel.Length; i++)
            {
                pMenu.Items.Add(mnuLabel[i]);
            }
        }

        private void ContextMenuViewLabelCount()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuLabelCount.Length; i++)
            {
                pMenu.Items.Add(mnuLabelCount[i]);
            }
        }

        private void ContextMenuViewOption()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuOption.Length; i++)
            {
                pMenu.Items.Add(mnuOption[i]);
            }
        }

        private void ContextMenuViewCaseNG()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuCaseNG.Length; i++)
            {
                pMenu.Items.Add(mnuCaseNG[i]);
            }
        }

        private void ContextMenuViewCompare()
        {
            if (pMenu == null) return;
            for (int i = 0; i < mnuCompare.Length; i++)
            {
                pMenu.Items.Add(mnuCompare[i]);
            }
        }

        private void ContextMenuViewDisplayName(string tempItem)
        {
            
           string tmpString = dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.TYPE].Value.ToString();
           dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[(int)sIndex.DISPLAY].Value = "[" + tmpString + "] " + tempItem;
           
        }

        private void ContextMenuViewTBL(string strEx)
        {
            if (pMenu == null) return;
            switch (strEx)
            {
                case devGEN9: for (int i = 0; i < mnuTBL_GEN9.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_GEN9[i]);
                    } break;
                case devGEN10: for (int i = 0; i < mnuTBL_GEN10.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_GEN10[i]);
                    } break;
                case devTC3000: for (int i = 0; i < mnuTBL_TC3000.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_TC3000[i]);
                    } break;
                case devTC1400A:
                    for (int i = 0; i < mnuTBL_TC1400A.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_TC1400A[i]);
                    }
                    break;
                case devMTP200: for (int i = 0; i < mnuTBL_MTP200.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_MTP200[i]);
                    } break;
                case devODAPWR: for (int i = 0; i < mnuTBL_ODAPWR.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_ODAPWR[i]);
                    } break;
                case devTCP: for (int i = 0; i < mnuTBL_TCP.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_TCP[i]);
                    } break;
                case devGEN11: for (int i = 0; i < mnuTBL_GEN11.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_GEN11[i]);
                    } break;
                case devGEN12:
                    for (int i = 0; i < mnuTBL_GEN12.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_GEN12[i]);
                    }
                    break;
                case devGEN11P: for (int i = 0; i < mnuTBL_GEN11P.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_GEN11P[i]);
                    } break;
                case devCCM: for (int i = 0; i < mnuTBL_CCM.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_CCM[i]);
                    } break;
                case devNAD: for (int i = 0; i < mnuTBL_NAD.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_NAD[i]);
                    } break;
                case devMCTM: for (int i = 0; i < mnuTBL_MCTM.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_MCTM[i]);
                    } break;
                case devATT: for (int i = 0; i < mnuTBL_ATT.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_ATT[i]);
                    } break;
                case "PAGE":
                    for (int i = 0; i < mnuPage.Length; i++)
                    {
                        pMenu.Items.Add(mnuPage[i]);
                        mnuPage[i].Font = new System.Drawing.Font("Courier New", 9.75F);
                    } break;
                case "GMES":
                    for (int i = 0; i < mnuGmes.Length; i++)
                    {
                        pMenu.Items.Add(mnuGmes[i]);
                    } break;
                case "MES":
                    for (int i = 0; i < mnuOracle.Length; i++)
                    {
                        pMenu.Items.Add(mnuOracle[i]);
                    } break;
                case "EXCEL":
                    for (int i = 0; i < mnuExcel.Length; i++)
                    {
                        pMenu.Items.Add(mnuExcel[i]);
                    } break;
                case devDIO: for (int i = 0; i < mnuTBL_DIO.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DIO[i]);
                    } break;
                case devAudio: for (int i = 0; i < mnuTBL_AUDIO.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_AUDIO[i]);
                    } break;
                case devADC: for (int i = 0; i < mnuTBL_ADC.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_ADC[i]);
                    } break;
                case dev5515C: for (int i = 0; i < mnuTBL_5515C.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_5515C[i]);
                    } break;
                case dev34410A: for (int i = 0; i < mnuTBL_34410A.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_34410A[i]);
                    } break;
                case devPCAN: for (int i = 0; i < mnuTBL_PCAN.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_PCAN[i]);
                    } break;

                case devVECTOR: for (int i = 0; i < mnuTBL_VECTOR.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_VECTOR[i]);
                    } break;
                case devKEITHLEY: for (int i = 0; i < mnuTBL_KEITHLEY.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_KEITHLEY[i]);
                    } break;
                case "DTC_GEN9": for (int i = 0; i < mnuTBL_DTC_GEN9.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DTC_GEN9[i]);
                    } break;
                case "DTC_GEN10": for (int i = 0; i < mnuTBL_DTC_GEN10.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DTC_GEN10[i]);
                    } break;

                case "DTC_TCP": for (int i = 0; i < mnuTBL_DTC_TCP.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DTC_TCP[i]);
                    } break;
                case "DTC_GEN11": for (int i = 0; i < mnuTBL_DTC_GEN11.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DTC_GEN11[i]);
                    } break;
                case "DTC_GEN12":
                    for (int i = 0; i < mnuTBL_DTC_GEN12.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DTC_GEN12[i]);
                    }
                    break;
                case "DTC_MCTM": for (int i = 0; i < mnuTBL_DTC_MCTM.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DTC_MCTM[i]);
                    } break;
                case "SCAN":
                    for (int i = 0; i < mnuScan.Length; i++)
                    {
                        pMenu.Items.Add(mnuScan[i]);
                    } break;
                case "OOB":
                    for (int i = 0; i < mnuOOB.Length; i++)
                    {
                        pMenu.Items.Add(mnuOOB[i]);
                    } break;
                case "DLLGATE": for (int i = 0; i < mnuTBL_DLLGATE.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_DLLGATE[i]);
                    } break;
                case devMELSEC: for (int i = 0; i < mnuTBL_MELSEC.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_MELSEC[i]);
                    } break;

                case devMTP120A:
                    for (int i = 0; i < mnuTBL_MTP120A.Length; i++)
                    {
                        pMenu.Items.Add(mnuTBL_MTP120A[i]);
                    }
                    break;

                default: break;
            }
        }  

        private void ContextMenuView(int iEx)
        {
            if (pMenu != null) pMenu.Items.Clear();
            if (pMenu == null) return;

            string tmpStr = String.Empty;
            int iRow = dataGridEdit.CurrentCell.RowIndex;
            int iCol = dataGridEdit.CurrentCell.ColumnIndex;
            switch (iEx)
            {
                //    TYPE, CMD, DISPLAY, MESCODE, ACTION, LABEL, LABELCOUNT, CASENG, DELAY,
                //    TIMEOUT, RETRY, COMPARE, MIN, MAX, OPTION, PAR1, DOC, EXPR

                case -1: ContextMenuViewTool(); break;

                case (int)sIndex.TYPE: pMenu.Items.Insert(0, mnuType); break;
                case (int)sIndex.CMD:
                    if (dataGridEdit.Rows[iRow].Cells[iEx - 1].Value != null)
                    {
                        tmpStr = dataGridEdit.Rows[iRow].Cells[iEx - 1].Value.ToString();
                    }
                    ContextMenuViewTBL(tmpStr);
                    break;
                case (int)sIndex.DISPLAY:   pMenu.Items.Insert(0, mnuDisplay); break;
                case (int)sIndex.ACTION:    ContextMenuViewAction(); break;
                case (int)sIndex.LABEL:     ContextMenuViewLabel(); break;
                case (int)sIndex.LABELCOUNT: ContextMenuViewLabelCount(); break;
                case (int)sIndex.CASENG:    ContextMenuViewCaseNG(); break;
                case (int)sIndex.COMPARE:   ContextMenuViewCompare(); break;
                case (int)sIndex.OPTION:    ContextMenuViewOption(); break;
                case (int)sIndex.PAR1:
                    {
                        if (dataGridEdit.Rows[iRow].Cells[(int)sIndex.TYPE].Value != null &&                            
                            dataGridEdit.Rows[iRow].Cells[(int)sIndex.CMD].Value != null &&
                            dataGridEdit.Rows[iRow].Cells[(int)sIndex.CMD].Value.ToString() == "READ_DTC")
                        {
                            if (dataGridEdit.Rows[iRow].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN10)
                                ContextMenuViewTBL("DTC_GEN10");
                            if (dataGridEdit.Rows[iRow].Cells[(int)sIndex.TYPE].Value.ToString() == devTCP)
                                ContextMenuViewTBL("DTC_TCP");
                            if (dataGridEdit.Rows[iRow].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN11)
                                ContextMenuViewTBL("DTC_GEN11");
                            if (dataGridEdit.Rows[iRow].Cells[(int)sIndex.TYPE].Value.ToString() == devGEN12)
                                ContextMenuViewTBL("DTC_GEN12");
                            if (dataGridEdit.Rows[iRow].Cells[(int)sIndex.TYPE].Value.ToString() == devMCTM)
                                ContextMenuViewTBL("DTC_MCTM"); 
                        }
                        
                    }break;
                default: break;
            }
        }

        private bool CheckLabelPos(string strLabelName, int iStdRow)
        {
            if (strLabelName.StartsWith("J") || strLabelName.StartsWith("P") || strLabelName.StartsWith("F"))
            {
                for (int i = 0; i < dataGridEdit.Rows.Count; i++)
                {
                    if (dataGridEdit.Rows[i].Cells[(int)sIndex.LABEL].Value != null &&
                        String.Compare(dataGridEdit.Rows[i].Cells[(int)sIndex.LABEL].Value.ToString(), strLabelName) == 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < iStdRow; i++)
                {
                    if (dataGridEdit.Rows[i].Cells[(int)sIndex.LABEL].Value != null &&
                        dataGridEdit.Rows[i].Cells[(int)sIndex.LABEL].Value.ToString().Equals(strLabelName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

#endregion

#region 그리드 제어 관련

        private void dataGridEdit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bOnJobFiles = true;

            if (e.ColumnIndex < 0)
            {
                ContextMenuView(e.ColumnIndex);
                return;
            }

            ContextMenuView(e.ColumnIndex);
        }
        private void dgvResetGrid()
        {
            dataGridEdit.Rows.Clear();
            dataGridEdit.Rows.Add(10);
          

            if (cbJobFiles.Items.Count == 0)
            {
                cbJobFiles.Text = "";
                return;
            }
            if (cbJobFiles.SelectedIndex >= 0)
            {
                if (cbJobFiles.SelectedItem.ToString().Length > 0)
                {
                    string strReason = String.Empty;
                    LoadJobFile(cbJobFiles.SelectedItem.ToString(), ref strReason);
                }
            }

            CellIndexNumbering();
            dataGridEdit.Update();
            Fsave.Focus();
        }

        public bool LoadJobFile(string strFileName, ref string strReason)
        {           
            JOBlist.Clear();

            bool bLoad = DKLogger_EDIT.LoadStepJobs(strFileName, ref JOBlist);

            if (bLoad && JOBlist.Count > 0)
            {
                int iCol = 0;
                for (int i = 0; i < JOBlist.Count; i++)
                {
                    // 절차서 상단의 주석내용은 제거
                    if (JOBlist[i].strJOB[0].Equals("TYPE"))
                        continue;

                    if (!OnJobFile(JOBlist[i].strJOB, JOBlist[i].strChkSum, iCol, ref strReason))
                    {
                        return false;
                    }
                    iCol++;
                }
                return true;
            }
            else
            {
                strReason = "ERROR JOB COUNT : 0";                
                return false;
            }

        }


        private void dgvDataCellDisplay()
        {
            if (dataGridEdit.SelectedCells.Count == 0)
            {
                return;
            }

            DataGridViewCell startCell = dgvGetStartCell(dataGridEdit);

            int iRowIndex = startCell.RowIndex;
            int iColIndex = startCell.ColumnIndex;
            int[] iCell = dgvGetLastCell();

            for (int j = iRowIndex; j < (iRowIndex + iCell[1]); j++)
            {
                if (dataGridEdit.Rows[j].Cells[(int)sIndex.TYPE].Value != null &&
                    dataGridEdit.Rows[j].Cells[(int)sIndex.CMD].Value != null)
                {
                    string tmpString = dataGridEdit.Rows[j].Cells[(int)sIndex.TYPE].Value.ToString();
                    string tempItem = dataGridEdit.Rows[j].Cells[(int)sIndex.CMD].Value.ToString();
                    UpdateBeforeCellData(j);
                    dataGridEdit.Rows[j].Cells[(int)sIndex.DISPLAY].Value = "[" + tmpString + "] " + tempItem;
                    CompareBeforeCellData(j);
                }
                    
            }

        }
        private void dgvDataCellSkip(bool bSet)
        {
            if (dataGridEdit.SelectedCells.Count == 0)
            {
                return;
            }

            DataGridViewCell startCell = dgvGetStartCell(dataGridEdit);

            int iRowIndex = startCell.RowIndex;
            int iColIndex = startCell.ColumnIndex;

            int[] iCell = dgvGetLastCell();

            string strAction = "SKIP";
            if (!bSet) strAction = "RUN";
            for (int j = iRowIndex; j < (iRowIndex + iCell[1]); j++)
            {
                if (dataGridEdit.Rows[j].Cells[(int)sIndex.ACTION].Value != null)
                {
                    UpdateBeforeCellData(j);
                    dataGridEdit.Rows[j].Cells[(int)sIndex.ACTION].Value = strAction;
                    CompareBeforeCellData(j);
                }                    
            }

        }
        private void dgvDataCellRemove()
        {
            if (dataGridEdit.SelectedCells.Count == 0)
            {
                return;
            }

            if (dataGridEdit.CurrentCell.RowIndex + 1 >= dataGridEdit.RowCount)
            {
                for (int tj = 0; tj < iGrdColCount; tj++)
                {
                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[tj].Value = "";

                }
            }

            DataGridViewCell startCell = dgvGetStartCell(dataGridEdit);

            int iRowIndex = startCell.RowIndex;
            int iColIndex = startCell.ColumnIndex;

            int[] iCell = dgvGetLastCell();


            for (int j = iRowIndex; j < (iRowIndex + iCell[1]); j++)
            {
                dataGridEdit.CurrentCell = dataGridEdit.Rows[iRowIndex].Cells[iColIndex];
                if (iRowIndex + 1 >= dataGridEdit.Rows.Count)
                {
                    dataGridEdit.Rows.Add();                    

                }
                dataGridEdit.Rows.RemoveAt(iRowIndex);

            }

            

            CellIndexNumbering();
        }
        private void dgvDataCellAdd(int iNum)
        {
            if (dataGridEdit.SelectedCells.Count == 0)
            {
                return;
            }

            try
            {
                dataGridEdit.Rows.Insert(dataGridEdit.CurrentCell.RowIndex, iNum);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                dataGridEdit.Rows.Add(iNum);
                 
            }

            CellIndexNumbering();


        }
        private void dgvDeleteClipboard()
        {
            //Delete to clipboard
            /*
            
            */

            if (dataGridEdit.SelectedCells.Count == 0)
            {
                //MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewCell startCell = dgvGetStartCell(dataGridEdit);

            int iRowIndex = startCell.RowIndex;
            int iColIndex = startCell.ColumnIndex;

            int[] iCell = dgvGetLastCell();

            for (int i = iColIndex; i < iColIndex + iCell[0]; i++)
            {
                for (int j = iRowIndex; j < iRowIndex + iCell[1]; j++)
                {
                    UpdateBeforeCellData(j);
                    dataGridEdit.Rows[j].Cells[i].Value = "";
                    CompareBeforeCellData(j);
                }
            }
                        

            /*
            for (int i = iColIndex; i <= dataGridEdit.CurrentCell.ColumnIndex; i++)
            {

                for (int j = iRowIndex; j <= dataGridEdit.CurrentCell.RowIndex; j++)
                {
                    dataGridEdit.Rows[j].Cells[i].Value = "";
                }
            }
            */

        }

        private bool dgvFindInGrdiview(string strTargetText, ref int iRow, ref int iCol)
        {
            bool bFind = false;
            int i = 0;
            int j = 0;

            int iPreRow = dataGridEdit.CurrentCell.RowIndex;
            int iPreCol = dataGridEdit.CurrentCell.ColumnIndex;

            try
            {
                for (i = iPreRow; i < dataGridEdit.RowCount; i++)
                {
                    for (j = 0; j < dataGridEdit.ColumnCount; j++)
                    {
                        if (i.Equals(iPreRow) && j <= iPreCol) continue;

                        if (dataGridEdit.Rows[i].Cells[j].Value != null)
                        {                            
                            if (!String.IsNullOrEmpty(dataGridEdit.Rows[i].Cells[j].Value.ToString()))
                            {   
                                if ((dataGridEdit.Rows[i].Cells[j].Value.ToString().ToUpper().Contains(strTargetText.ToUpper())))
                                {                                    
                                    bFind = true;
                                    iRow = i; iCol = j;
                                    break;
                                }
                            }
                        }
                    }
                    if (bFind) break;
                }
            }
            catch 
            {
                return false;
            }


            return bFind;
        }

        private void dgvCopyToClipboard()
        {
            //Copy to clipboard       
            DataObject dataObj = dataGridEdit.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
		
        private void dgvPasteClipboardValue()
        {
            //Show Error if no cell is selected
            if (dataGridEdit.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the satring Cell
            DataGridViewCell startCell = dgvGetStartCell(dataGridEdit);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = dgvClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                if (rowKey < cbValue.Values.Count && cbValue[rowKey].Values.Count >= (int)sIndex.END + 1)
                {
                    dataGridEdit.Rows.Insert(iRowIndex);
                    CellIndexNumbering();
                }

                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {

                    if (iColIndex <= dataGridEdit.Columns.Count - 1 && iRowIndex <= dataGridEdit.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridEdit[iColIndex, iRowIndex];
                        //진짜 슈퍼 대박. ㅡㅡ copy paste 로 붙여넣으면 \n 값이 끼어서 파일저장시 리턴값 낌..아 짜증나 3시간 개고생  
                        if (cbValue[rowKey].Values.Count >= (int)sIndex.END + 1)
                        {
                            if (cellKey > 0)
                            {
                                cell.Value = cbValue[rowKey][cellKey].ToString().Trim().Replace("\n", "");
                                iColIndex++;                                
                            }
                        }
                        else
                        {
                            cell.Value = cbValue[rowKey][cellKey].ToString().Trim().Replace("\n", "");
                            iColIndex++;
                        }
                    }
                    
                }
                
                iRowIndex++;                
            }
        }
		
        private int[] dgvGetLastCell()
        {
            //get the smallest row,column index
            ArrayList alCountX = new ArrayList();
            ArrayList alCountY = new ArrayList();

            if (dataGridEdit.SelectedCells.Count == 0)
                return null;

            int[] rtnVal = new int[2];


            foreach (DataGridViewCell dgvCell in dataGridEdit.SelectedCells)
            {
                if (alCountX.Count == 0)
                {
                    alCountX.Add(dgvCell.RowIndex);
                }
                else
                {
                    if (!alCountX.Contains(dgvCell.RowIndex))
                    {
                        alCountX.Add(dgvCell.RowIndex);
                    }
                }

                if (alCountY.Count == 0)
                {
                    alCountY.Add(dgvCell.ColumnIndex);
                }
                else
                {
                    if (!alCountY.Contains(dgvCell.ColumnIndex))
                    {
                        alCountY.Add(dgvCell.ColumnIndex);
                    }
                }
            }

            rtnVal[0] = alCountY.Count;
            rtnVal[1] = alCountX.Count;

            return rtnVal;
        }

        private DataGridViewCell dgvGetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;

            }
            return dgView[colIndex, rowIndex];
        }
        private Dictionary<int, Dictionary<int, string>> dgvClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>> copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionay with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
        private void dataGridEdit_KeyDown(object sender, KeyEventArgs e)
        {         
            if (e.KeyData == (Keys.Control | Keys.V))
            {
                //MessageBox.Show("Ctrl + V");
                dgvPasteClipboardValue();
            }
            else if (e.KeyData == (Keys.Delete))
            {
                //MessageBox.Show("Delete Key");                
                dgvDeleteClipboard();
            }

            else if (e.KeyData == (Keys.Control | Keys.X))
            {
                dgvCopyToClipboard();
                dgvDeleteClipboard();
            }

            else if (e.KeyData == (Keys.Control | Keys.F))
            {
                pannelTextFinder.Visible = true;
                txtTargetFind.Focus();
            }

        }
        private void dataGridEdit_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dataGridEdit.CurrentCell = dataGridEdit[0, e.RowIndex];

        }
        private void dataGridEdit_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 || e.RowIndex == -1) return;

            iCellDragStartPoint = e.RowIndex;
            if (e.Button == MouseButtons.Left)
            {
                dataGridEdit.ClearSelection();
                for (int i = 0; i < iGrdColCount; i++)
                {
                    dataGridEdit.Rows[e.RowIndex].Cells[i].Selected = true;
                }

            }


        }
        private void dataGridEdit_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {          

            // 헤더에서 눌렀을때 리턴.
            if (cbJobFiles.Items.Count < 1) return;
            if (pMenu == null) return;
            if (e.RowIndex == -1) return;
            if (e.Button == MouseButtons.Left && e.ColumnIndex == -1)
            {
                for (int i = 0; i < iGrdColCount; i++)
                {
                    for (int j = iCellDragStartPoint; j <= e.RowIndex; j++)
                    {
                        dataGridEdit.Rows[j].Cells[i].Selected = true;
                    }

                }                
                return;
            }

            // 오른쪽 버튼일때
            if (e.Button == MouseButtons.Right)
            {
                // 현재 마우스 위치의 정보를 얻어옴
                Point pt = dataGridEdit.PointToClient(new Point(MousePosition.X, MousePosition.Y));

                var cellRectangle = dataGridEdit.GetCellDisplayRectangle(dataGridEdit.CurrentCell.ColumnIndex, dataGridEdit.CurrentCell.RowIndex, true);

                if (e.ColumnIndex < 0)
                {
                    cellRectangle = dataGridEdit.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                }

                Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);

                int titleHeight = screenRectangle.Top - this.Top;
                int titleWidth = screenRectangle.Left - this.Left;

                int tx = this.Left + dataGridEdit.Left + cellRectangle.Left + titleWidth + 20;
                int ty = this.Top + dataGridEdit.Top + cellRectangle.Top + titleHeight + 20;

                System.Windows.Forms.Cursor.Position = new Point(tx, ty);

                //Prevent 2015.03.26 DK.SIM  
                if (dataGridEdit.ContextMenuStrip != null)
                {
                    dataGridEdit.ContextMenuStrip.Left = tx;
                    dataGridEdit.ContextMenuStrip.Top = ty;
                }               

                if (e.ColumnIndex < 0)
                {
                    for (int i = 0; i < iGrdColCount; i++)
                    {
                        dataGridEdit.Rows[e.RowIndex].Cells[i].Selected = true;
                    }

                    ContextMenuView(e.ColumnIndex);
                    return;
                }
                else
                {
                    dataGridEdit.ClearSelection();
                    dataGridEdit.Rows[dataGridEdit.CurrentCell.RowIndex].Cells[dataGridEdit.CurrentCell.ColumnIndex].Selected = true;
                    ContextMenuView(dataGridEdit.CurrentCell.ColumnIndex);
                }
            }
           

        }
                
#endregion

        private void FSaveCsv_Click(object sender, EventArgs e)
        {
            CellDefaultSize();

            string strOrginName = cbJobFiles.SelectedItem.ToString();
            if (!CheckValidation())
            {
                return;
            }
            else
            {
                if (cbJobFiles.Items.Count == 0)
                {
                    cbJobFiles.Text = "";
                    MessageBox.Show("JOB File does not exist.");
                    return;
                }

            }

            string sFileName = String.Empty;

            sFileName = strOrginName.Replace(".JOB", ".CSV");

            DKLogger_EDIT.DeleteJob(sFileName);

            string[] strData = new string[iGrdColCount];


            string strTitle = "TYPE<>CMD<>DISPLAYNAME<>MESCODE<>ACTION<>LABEL<>LABELCOUNT<>CASENG<>DELAY<>TIMEOUT<>RETRY<>COMPARE<>MIN<>MAX<>OPTION<>PAR1<>DOC<>EXPR";
            string[] strTitleData = System.Text.RegularExpressions.Regex.Split(strTitle, "<>");

            if (!DKLogger_EDIT.SaveCSV(sFileName, strTitleData)) return;

            foreach (DataGridViewRow fees_row in this.dataGridEdit.Rows)
            {   //셀이 비어있는지 체크. 그래야 다음칸에 내용을 채운다.
                var cell = fees_row.Cells[0];
                if (cell.Value != null)
                {
                    if (!String.IsNullOrEmpty(cell.Value.ToString()))
                    {
                        var value = cell.Value;
                        if (value != null)
                        {
                            for (int i = 0; i < strData.Length; i++)
                            {
                                if (fees_row.Cells[i].Value != null)
                                {
                                    strData[i] = fees_row.Cells[i].Value.ToString();
                                }
                                else
                                {
                                    strData[i] = "";
                                }
                            }
                            DKLogger_EDIT.SaveCSV(sFileName, strData);
                        }
                    }
                }
            }       
        }

        private void txtTargetFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Enter) && !String.IsNullOrEmpty(txtTargetFind.Text))
            {
                int iRow = 0;
                int iCol = 0;
                if (dgvFindInGrdiview(txtTargetFind.Text, ref iRow, ref iCol))
                {
                    dataGridEdit.CurrentCell = dataGridEdit.Rows[iRow].Cells[iCol];
                }
            }
            else if (e.KeyData == (Keys.Escape))
            {
                txtTargetFind.Text = "";
                pannelTextFinder.Visible = false;
                dataGridEdit.Focus();
            }
        }

        private void btnFinderClose_Click(object sender, EventArgs e)
        {
            txtTargetFind.Text = "";
            pannelTextFinder.Visible = false;
            dataGridEdit.Focus();
        }

        //이력남기는 용도용1
        private void UpdateBeforeCellData(int iRowIdx)
        {

            if (iRowIdx < 0) return;

            try
            {
                lstCells.Clear();

                try
                {
                    if (dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.TYPE].Value == null ||
                    String.IsNullOrEmpty(dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.TYPE].Value.ToString()))
                        return;

                    if (dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.CMD].Value == null ||
                        String.IsNullOrEmpty(dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.CMD].Value.ToString()))
                        return;
                }
                catch
                {
                    return;
                }


                string[] strCellLines = new string[(int)sIndex.END + 1];

                strCellLines[(int)sIndex.END] = iRowIdx.ToString();

                for (int i = 0; i < (int)sIndex.END; i++)
                {
                    strCellLines[i] = String.Empty;
                    if (dataGridEdit.Rows[iRowIdx].Cells[i].Value != null)
                    {
                        strCellLines[i] = dataGridEdit.Rows[iRowIdx].Cells[i].Value.ToString();
                    }
                    lstCells.Add(strCellLines[i]);
                }
                lstCells.Add(strCellLines[(int)sIndex.END]);
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }

        }
        //이력남기는 용도용2
        private void CompareBeforeCellData(int iRowIdx)
        {
            try
            {                
                if (lstCells.Count < 1) return;

                if (dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.TYPE].Value == null ||
                    String.IsNullOrEmpty(dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.TYPE].Value.ToString()))
                    return;

                if (dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.CMD].Value == null ||
                    String.IsNullOrEmpty(dataGridEdit.Rows[iRowIdx].Cells[(int)sIndex.CMD].Value.ToString()))
                    return;

                string[] strCellLines = new string[(int)sIndex.END + 1];

                strCellLines[(int)sIndex.END] = iRowIdx.ToString();

                for (int i = 0; i < (int)sIndex.END; i++)
                {
                    if (dataGridEdit.Rows[iRowIdx].Cells[i].Value != null)
                    {
                        strCellLines[i] = dataGridEdit.Rows[iRowIdx].Cells[i].Value.ToString();
                    }
                }

                string strErrorText = String.Empty;

                if (lstCells[(int)sIndex.END].Equals(strCellLines[(int)sIndex.END]))
                {

                    for (int i = 0; i < (int)sIndex.END; i++)
                    {
                        strErrorText = "NO:" + (iRowIdx + 1).ToString() + (char)0x05 +
                                    strCellLines[(int)sIndex.TYPE] + (char)0x05 + strCellLines[(int)sIndex.CMD] + (char)0x05;

                        if (!lstCells[i].Equals(strCellLines[i]) && (!String.IsNullOrEmpty(lstCells[i]) || !String.IsNullOrEmpty(strCellLines[i])))
                        {                            
                            if (String.IsNullOrEmpty(lstCells[i]))
                            {
                                strErrorText += (sIndex.TYPE + i).ToString() + ":" + strCellLines[i];
                                DKLogger_EDIT.WriteEditHistory(cbJobFiles.SelectedItem.ToString(), strEditorName, strErrorText);

                            }
                            else
                            {
                                if (String.IsNullOrEmpty(strCellLines[i]))
                                {
                                    strErrorText += (sIndex.TYPE + i).ToString() + ":" + lstCells[i] + (char)0x1A + "#DELETE#";
                                    DKLogger_EDIT.WriteEditHistory(cbJobFiles.SelectedItem.ToString(), strEditorName, strErrorText);

                                }
                                else
                                {
                                    strErrorText += (sIndex.TYPE + i).ToString() + ":" + lstCells[i] + (char)0x1A + strCellLines[i];
                                    DKLogger_EDIT.WriteEditHistory(cbJobFiles.SelectedItem.ToString(), strEditorName, strErrorText);                                 
                                }                                
                            }                           
                        }
                    }
                }

                lstCells.Clear();

            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }

            
            
        }

        //이력남기는 용도용3
        private void dataGridEdit_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!bOnJobFiles) return;

            CompareBeforeCellData(e.RowIndex);

        }
        //이력남기는 용도용4
        private void dataGridEdit_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            UpdateBeforeCellData(e.RowIndex);
        }
        private bool CheckAlreadyLabel(string strLabelName)
        {
            if (strLabelName == "") return false;

            for (int i = 0; i < dataGridEdit.Rows.Count; i++)
            {
                if (dataGridEdit.Rows[i].Cells[0].Value == null ||
                    dataGridEdit.Rows[i].Cells[0].Value.ToString() == "")
                    break;

                if (dataGridEdit.Rows[i].Cells[(int)sIndex.LABEL].Value.ToString() == strLabelName)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
