using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GmTelematics
{
    public partial class FrmJobMapping : Form
    {
        DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
        private DK_LOGGER DKLogger_EDIT = new DK_LOGGER("PC", false);
        private ContextMenuStrip sMenu;
        private ToolStripMenuItem[] mnuModel;

        //LGEVH
        private int iLoginUser = 0;
        private PWUSER pwLoginUser = new PWUSER();
        public FrmJobMapping(int iCert, PWUSER pwUser)
        {
            iLoginUser = iCert;
            pwLoginUser = pwUser;

            InitializeComponent();
            UI_INIT();
            MapLoding();
            MakeModelList();
        }

        private void ControlFontReset(ToolStripMenuItem[] targetCtrl)
        {
            for (int i = 0; i < targetCtrl.Length; i++)
            {
                targetCtrl[i].Font = new System.Drawing.Font("Courier New", 9.75F);
            }
        }


        private void MakeModelList()
        {
            if (STEPMANAGER_VALUE.LstSuffix == null || STEPMANAGER_VALUE.LstModel == null) return;


            sMenu = new ContextMenuStrip();

            mnuModel = new ToolStripMenuItem[STEPMANAGER_VALUE.LstModel.Count];


            SUFFIXLIST tmpSuffix = new SUFFIXLIST();
            int[] iSuffixCount = new int[STEPMANAGER_VALUE.LstModel.Count];

            for (int j = 0; j < STEPMANAGER_VALUE.LstSuffix.Count; j++)
            {
                tmpSuffix = STEPMANAGER_VALUE.LstSuffix[j];
                iSuffixCount[tmpSuffix.iDx]++;
            }

            for (int i = 0; i < mnuModel.Length; i++)
            {
                mnuModel[i] = new ToolStripMenuItem(STEPMANAGER_VALUE.LstModel[i]);
            }


            int iTcount = 0;

            for (int i = 0; i < iSuffixCount.Length; i++)
            {
                for (int j = 0; j < iSuffixCount[i]; j++)
                {
                    if (iTcount < STEPMANAGER_VALUE.LstSuffix.Count)
                    {
                        tmpSuffix = STEPMANAGER_VALUE.LstSuffix[iTcount];
                        mnuModel[tmpSuffix.iDx].DropDownItems.Add(tmpSuffix.strSuffix);
                        iTcount++;
                    }
                }
            }


            for (int i = 0; i < mnuModel.Length; i++)
            {
                sMenu.Items.Add(mnuModel[i]);
                for (int j = 0; j < mnuModel[i].DropDownItems.Count; j++)
                {
                    mnuModel[i].DropDownItems[j].Click += new EventHandler(GridClickMenuHandler);
                }

            }

            sMenu.Font = new System.Drawing.Font("Courier New", 9.75F);
            ControlFontReset(mnuModel);
        }

        private void MapLoding()
        {
            List<string[]> lstMap = new List<string[]>();
            bool bRtn = DKLogger_EDIT.LoadAutoJob(ref lstMap);

            int iDx = lstMap.Count;

            if (iDx > 0)
            {
                dataGridEdit.Rows.Add(iDx);

                try
                {
                    for (int i = 0; i < iDx; i++)
                    {
                        dataGridEdit[0, i].Value = lstMap[i][0].ToString();
                        dataGridEdit[1, i].Value = lstMap[i][1].ToString();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("MapLoding Error : " + ex.ToString());
                }
            }



        }

        private void UI_INIT()
        {
            cmb.HeaderText = "JOB FILE LIST";
            cmb.Name = "cmb";

            string[] files = DKLogger_EDIT.GetFileList("JOB");

            if (files != null || files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    cmb.Items.Add(files[i].ToString());

                }
            }

            dataGridEdit.DoubleBuffered(true);
            dataGridEdit.RowHeadersVisible = false;

            dataGridEdit.Columns.Add("Col0", "MAPPING DATA");
            dataGridEdit.Columns[0].Width = (int)(dataGridEdit.Width * 0.3);
            dataGridEdit.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add("Col1", "MAPPING FILE NAME");
            dataGridEdit.Columns[1].ReadOnly = true;
            dataGridEdit.Columns[1].Width = (int)(dataGridEdit.Width * 0.35);
            dataGridEdit.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridEdit.Columns.Add(cmb);
            dataGridEdit.Columns[2].Width = (int)(dataGridEdit.Width * 0.35);
            //LGEVH 202306
            if (iLoginUser.Equals((int)ACCOUNT.SUPERUSER)
              || (iLoginUser.Equals((int)ACCOUNT.USER) && pwLoginUser.bAutoJobConfig))
            {
                BtnControl(true);
            }
            else
                BtnControl(false);
        }
        private void BtnControl(bool bState)
        {
            CrossThreadIssue.AppendEnabled(Fnew, bState);
            CrossThreadIssue.AppendEnabled(Fdelete, bState);
            CrossThreadIssue.AppendEnabled(Fsave, bState);
            //CrossThreadIssue.AppendEnabled(btnExit, bState);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Fsave_Click(object sender, EventArgs e)
        {
            List<string[]> lstData = new List<string[]>();

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
                            string[] strData = new string[2];
                            for (int i = 0; i < strData.Length; i++)
                            {
                                if (fees_row.Cells[i].Value != null)
                                {
                                    if (!String.IsNullOrEmpty(fees_row.Cells[i].Value.ToString()))
                                    {
                                        strData[i] = fees_row.Cells[i].Value.ToString();
                                    }
                                    else
                                    {
                                        MessageBox.Show("CAN NOT SAVE! Please, Input Data! -[0]");
                                        return;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("CAN NOT SAVE! Please, Input Data! -[1]");
                                    return;
                                }
                            }
                            lstData.Add(strData);
                            //DKLogger_EDIT.SaveAutoJob(strData);
                        }
                    }
                    else
                    {
                        MessageBox.Show("CAN NOT SAVE! Please, Input Data! -[2]");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("CAN NOT SAVE! Please, Input Data! -[3]");
                    return;
                }
            }
            DKLogger_EDIT.SaveAutoJob(lstData);

        }

        private void FrmJobMapping_Load(object sender, EventArgs e)
        {

        }

        private void Fnew_Click(object sender, EventArgs e)
        {
            dataGridEdit.Rows.Add();
        }

        private void Fdelete_Click(object sender, EventArgs e)
        {
            if (dataGridEdit.Rows.Count <= 0) return;
            int iDx = dataGridEdit.CurrentCell.RowIndex;

            if (iDx >= 0)
            {
                dataGridEdit.Rows.RemoveAt(iDx);
            }


        }

        private void dataGridEdit_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridEdit.CurrentCell.ColumnIndex == 2 && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += LastColumnComboSelectionChanged;
            }

        }

        private void LastColumnComboSelectionChanged(object sender, EventArgs e)
        {
            var currentcell = dataGridEdit.CurrentCellAddress;
            var sendingCB = sender as DataGridViewComboBoxEditingControl;
            DataGridViewTextBoxCell cel = (DataGridViewTextBoxCell)dataGridEdit.Rows[currentcell.Y].Cells[1];
            cel.Value = sendingCB.EditingControlFormattedValue.ToString();
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
        }

        private void GridClickMenuHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem tempItem = sender as ToolStripMenuItem;
            string tempString = tempItem.Text;
            dataGridEdit.CurrentCell.Value = tempItem.OwnerItem.Text + tempString;


        }

        #region 그리드 에디팅

        private void dgvDeleteClipboard()
        {
            //Delete to clipboard

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
                    dataGridEdit.Rows[j].Cells[i].Value = "";
                }
            }

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
            System.Collections.ArrayList alCountX = new System.Collections.ArrayList();
            System.Collections.ArrayList alCountY = new System.Collections.ArrayList();

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

        #endregion

        private void dataGridEdit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dataGridEdit.ContextMenuStrip = sMenu;
                return;
            }
            else
            {
                dataGridEdit.ContextMenuStrip = null;
            }
        }

    }
}
