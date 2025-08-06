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
    public partial class FrmPasswordManage : Form
    {
        private DK_LOGGER DKLoggerConfig = new DK_LOGGER("PC", false);
        PWUSER[] testUser = new PWUSER[(int)USERLIMIT.MAX];
        public FrmPasswordManage()
        {
            InitializeComponent();
            InitializeControls();     
            LoadFile();
        }

        private void LoadFile()
        {
            for (int i = 0; i < testUser.Length; i++)
            {
                testUser[i] = new PWUSER();
            }

            int iListCount = DKLoggerConfig.GetPasswordUserCount();

            if (iListCount > 0)
            {
                if (iListCount > (int)USERLIMIT.MAX)
                {
                    iListCount = (int)USERLIMIT.MAX;
                }

                for (int i = 0; i < iListCount; i++)
                {                    
                    testUser[i] = DKLoggerConfig.GetPasswordUserData(i);   
                }

                for (int i = 0; i < iListCount; i++)
                {
                    AddUser(testUser[i]);
                }

                return;
            }

        }

        private void InitializeControls()
        {
            // 케이블 커넥터  
            //LGEVH
            int iCol = 0;
            dataGridUsers.Columns.Add("Col0", "LOG NAME");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.2);
            dataGridUsers.Columns.Add("Col1", "PASSWORD");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.13);
            dataGridUsers.Columns.Add("Col2", "EDIT");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.13);
            dataGridUsers.Columns.Add("Col3", "JOB CHANGE");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.13);
            dataGridUsers.Columns.Add("Col4", "CONFIG");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.13);
            dataGridUsers.Columns.Add("Col5", "MES ON");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.13);

            dataGridUsers.Columns.Add("Col6", "AUTOJOBCONFIG");
            dataGridUsers.Columns[iCol++].Width = (int)((dataGridUsers.Width) * 0.13);

            for (int i = 0; i < dataGridUsers.Columns.Count; i++)
            {
                dataGridUsers.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void AddUser(PWUSER testUser)
        {
            dataGridUsers.AllowUserToResizeColumns = false;
            dataGridUsers.Rows.Add(1);
            int iIdx = dataGridUsers.Rows.Count - 1;
            dataGridUsers.Rows[iIdx].Cells[0].Value = testUser.strLogName;
            dataGridUsers.Rows[iIdx].Cells[1].Value = testUser.strPassword;

            for (int i = 0; i < dataGridUsers.Rows.Count; i++)
            {
                dataGridUsers.Rows[i].Cells[0].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[1].ReadOnly = false;
                dataGridUsers.Rows[i].Cells[2].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[3].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[4].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[5].ReadOnly = true;
                //LGEVH
                dataGridUsers.Rows[i].Cells[6].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
            }
      
        }
        
        private void AddUser()
        {
            if (dataGridUsers.Rows.Count > 18)
            {
                MessageBox.Show("CAN NOT ADD USER ! MEMORY FULL!");
                return;
            }
            System.Threading.Thread.Sleep(500);

            int iIdx = dataGridUsers.Rows.Count;
            testUser[iIdx] = new PWUSER();
            testUser[iIdx].strLogName = DKLoggerConfig.GetAddUserName();
            testUser[iIdx].strPassword = "";
            testUser[iIdx].bEdit = true;
            testUser[iIdx].bJob = true;
            testUser[iIdx].bConfig = true;
            testUser[iIdx].bMes = true;
            //LGEVH
            testUser[iIdx].bAutoJobConfig = true;
            dataGridUsers.AllowUserToResizeColumns = false;
            dataGridUsers.Rows.Add(1);

            dataGridUsers.Rows[iIdx].Cells[0].Value = testUser[iIdx].strLogName;
            

            for (int i = 0; i < dataGridUsers.Rows.Count; i++)
            {
                dataGridUsers.Rows[i].Cells[0].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[1].ReadOnly = false;
                dataGridUsers.Rows[i].Cells[2].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[3].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[4].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[5].ReadOnly = true;
                //LGEVH
                dataGridUsers.Rows[i].Cells[6].ReadOnly = true;
                dataGridUsers.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
            }
                        
            System.Threading.Thread.Sleep(500);
        }

        private void DeleteUser()
        {
            try
            {
                int iIdx = dataGridUsers.CurrentCell.RowIndex;

                if (iIdx >= 0)
                {
                    if (dataGridUsers.Rows[iIdx].Cells[0].Value != null)
                    {
                        string strMsgName = "DO YOU WANT DELETE [ " + dataGridUsers.Rows[iIdx].Cells[0].Value.ToString() + "] ?";

                        if (MessageBox.Show(strMsgName, "CONFIRM MESSAGE", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {                            
                            while (dataGridUsers.Rows.Count >= 0)
                            {
                                if (dataGridUsers.Rows.Count - 1 < 0) break;

                                Control[] tmpChk;

                                int r = dataGridUsers.Rows.Count - 1;

                                for (int i = 1; i < dataGridUsers.Columns.Count; i++)
                                {

                                    string cbName = "cb_" + r.ToString() + "_" + i.ToString();


                                    tmpChk = this.Controls.Find(cbName, true);
                                    if (tmpChk.Length > 0)
                                    {
                                        tmpChk[0].Dispose();
                                    }
                                }

                                dataGridUsers.Rows.RemoveAt(dataGridUsers.Rows.Count-1);
                               
                            }
                                                        
                            testUser[iIdx] = new PWUSER();

                            for (int i = iIdx; i < testUser.Length; i++)
                            {
                                if (i + 1 > testUser.Length-1) break;

                                testUser[i] = testUser[i + 1];                                
                            }


                            for (int i = 0; i < testUser.Length; i++)
                            {
                                if (!String.IsNullOrEmpty(testUser[i].strLogName))
                                {
                                    AddUser(testUser[i]);                                    
                                }
                            }

                        }
                    }
                }   
            }
            catch { }
            
        }

        private void InitializeValue()
        {
       
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            btnAddUser.Enabled = false;
            this.Refresh();
            AddUser();
            btnAddUser.Enabled = true;
            this.Refresh();
        }

        private void dataGridUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex > 1 && e.RowIndex >= 0 && !String.IsNullOrEmpty(testUser[e.RowIndex].strLogName))
            {
                e.PaintBackground(e.ClipBounds, false);

                Point pt = e.CellBounds.Location;  // where you want the bitmap in the cell

                int nChkBoxWidth  = 17;
                int nChkBoxHeight = 17;
                int offsetx = (e.CellBounds.Width - nChkBoxWidth) / 2;
                int offsety = (e.CellBounds.Height - nChkBoxHeight) / 2;

                pt.X += offsetx;
                pt.Y += offsety;
                
                int j = e.ColumnIndex - 1;

                Control[] tmpChk;
                string cBname = "cb_" + (e.RowIndex).ToString() + "_" + j.ToString();
                tmpChk = this.Controls.Find(cBname, true);

                if (tmpChk.Length > 0)
                {
   
                }
                else
                {
                    CheckBox cb = new CheckBox();
                    cb.Size = new Size(nChkBoxWidth, nChkBoxHeight);
                    cb.Location = pt;
                    cb.Name = cBname;
                    //cb.Text = j.ToString(); 
                    cb.Enabled = true;
                    cb.Checked = false;
                    switch(j)
                    {
                        case 1:
                            if (testUser[e.RowIndex].bEdit) cb.Checked = true; break;
                        case 2:
                            if (testUser[e.RowIndex].bJob) cb.Checked = true; break;
                        case 3:
                            if (testUser[e.RowIndex].bConfig) cb.Checked = true; break;
                        case 4:
                            if (testUser[e.RowIndex].bMes) cb.Checked = true; break;
                        case 5://LGEVH
                            if (testUser[e.RowIndex].bAutoJobConfig) cb.Checked = true; break;
                        default: break;
                    }
                   
                    cb.TextAlign = ContentAlignment.MiddleCenter;
                    cb.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                    cb.CheckedChanged += new EventHandler(CheckBoxClick);
                    ((DataGridView)sender).Controls.Add(cb);
                }
                           
                e.Handled = true;
            }
        }

        private void CheckBoxClick(object sender, EventArgs e)
        {
            string strCbName = ((CheckBox)sender).Name.ToString();
            string strCbArry = strCbName.Replace("cbok", "");

            string[] strArrayName = strCbName.Split('_');

            int i = int.Parse(strArrayName[1]);
            int j = int.Parse(strArrayName[2]);

            bool bChk = ((CheckBox)sender).Checked;
     
            switch(j)
            {
                case 1: testUser[i].bEdit = bChk; break;
                case 2: testUser[i].bJob = bChk; break;
                case 3: testUser[i].bConfig = bChk; break;
                case 4: testUser[i].bMes = bChk; break;
                //LGEVH
                case 5: testUser[i].bAutoJobConfig = bChk; break;
                default: break;
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //PASSWORD 입력이 안되있는 것이 있는지 먼저 검사.
            if (dataGridUsers.Rows.Count == 0)
            {
                DKLoggerConfig.SetPasswordUserData(testUser);
                STEPMANAGER_VALUE.SetUserInformation(testUser);
                return;
            }

            for (int i = 0; i < dataGridUsers.Rows.Count; i++)
            {
                if(dataGridUsers.Rows[i].Cells[1].Value == null ||
                    String.IsNullOrEmpty(dataGridUsers.Rows[i].Cells[1].Value.ToString()))
                {
                    MessageBox.Show("Please, Input [" + dataGridUsers.Rows[i].Cells[0].Value.ToString() + "] Password !");
                    return;
                }

                //LGEVH
                testUser[i].strPassword = dataGridUsers.Rows[i].Cells[1].Value.ToString().Trim();
            }

            DKLoggerConfig.SetPasswordUserData(testUser);
            STEPMANAGER_VALUE.SetUserInformation(testUser);
        }

    }
}
