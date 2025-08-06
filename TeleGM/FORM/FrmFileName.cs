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
    public partial class FrmFileName : Form
    {

        private int iModeType = 0;
        private string strOriginFileName = String.Empty;
        public FrmFileName(string[] files, string strFileName, int iMode)
        {            
            InitializeComponent();
            iModeType = iMode;
            strOriginFileName = strFileName;
            ListUp(files);
        }

        private void ListUp(string[] files)
        {
            switch (iModeType)
            {
                case (int)FILESAVE.CREATE: this.Text = "NEW FILE NAME";
                                           break;
                case (int)FILESAVE.SAVEAS: this.Text = "FILE SAVE AS"; 
                                           break;
                default: break;
            }


            FilelistBox.Items.Clear();
            if (files != null && files.Length > 0) //Prevent 2015.09.17 DK.SIM
            {
                for (int i = 0; i < files.Length; i++)
                {
                    FilelistBox.Items.Add(files[i].ToString());               
                }      
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            lblWarning.Visible = false;
            if (txtFileName.Text.Length < 3)
            {
                lblWarning.Text = "FILE NAME SIZE : " + txtFileName.Text.Length.ToString();
                lblWarning.Visible = true;
                return;
            }
            
            DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblWarning.Visible = false;
            DialogResult = DialogResult.Cancel;
        }
        
        public string GetFileName
        {
            get { return txtFileName.Text; }
        }
        
        private void FrmFileName_Shown(object sender, EventArgs e)
        {
            txtFileName.Focus();
        }

        private void txtFileName_KeyUp(object sender, KeyEventArgs e)
        {
            lblWarning.Visible = false;
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }

        private void FrmFileName_Load(object sender, EventArgs e)
        {

        }

        private void FilelistBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (FilelistBox.Items.Count < 1) return;
            if (FilelistBox.SelectedIndex < 0) return;
            if (FilelistBox.Items[FilelistBox.SelectedIndex].ToString().Length < 1) return;
            string strTmpFileName = FilelistBox.Items[FilelistBox.SelectedIndex].ToString().Replace(".JOB", String.Empty);
            txtFileName.Text = strTmpFileName;
        }
    }
}
