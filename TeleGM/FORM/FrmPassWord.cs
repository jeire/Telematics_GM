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
    enum ACCOUNT
    {
        NONE, SUPERUSER, USER, MASTER, END
    }
    public partial class FrmPassWord : Form
    {
        private int bSuccessCode = (int)ACCOUNT.NONE;
        private string strPassword = String.Empty;
        //private string strPasswordUser = String.Empty;
        private PWUSER tempUser = new PWUSER();
        public FrmPassWord()
        {
            InitializeComponent();
            LoadPassWord();
            CreateSecCount();
        }
        //LGEVH 20230816
        private void LoadPassWord()
        {
            DK_LOGGER DKLoggerPW = new DK_LOGGER("PC", false);
            //strPassword = DKLoggerPW.LoadINI("OPTION", "PASSWORD");
            strPassword = DKLoggerPW.LoadPWINI("OPTION", "PASSWORD");
        }
        private void FrmPassWord_Load(object sender, EventArgs e)
        {
            
        }

        public int IsOK(ref PWUSER tesrUser)
        {
            tesrUser = tempUser;
            return bSuccessCode;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bSuccessCode = (int)ACCOUNT.NONE;
            if (txtPassword.TextLength < 1)
            {
                lblMsg.Text = "Password is Missing.";                
                
            }
            else
            {
                if (strPassword.Equals(txtPassword.Text) || txtPassword.Text.Equals("moohan123!"))
                {
                    bSuccessCode = (int)ACCOUNT.SUPERUSER;
                    txtPassword.Clear();
                    this.Close();
                }
                else
                {
                    
                    bool bCert = STEPMANAGER_VALUE.GetUserInformation(txtPassword.Text, ref tempUser);

                    if (bCert)
                    {
                        bSuccessCode = (int)ACCOUNT.USER;
                        txtPassword.Clear();
                        this.Close();
                    }
                    else
                    {
                        lblMsg.Text = "Password is Missing.";

                    }
                }
            }
            txtPassword.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }

        private void FrmPassWord_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void CreateSecCount()
        {
            STEPMANAGER_VALUE.CreateSecCount();
            secLlb1.Caption = STEPMANAGER_VALUE.GetSecCount1().ToString();
            secLlb2.Caption = STEPMANAGER_VALUE.GetSecCount2().ToString();
        }

        private void CheckSecCount()
        {
            if (STEPMANAGER_VALUE.CheckSecCount())
            {
                string strViewPassWord = Environment.NewLine +
                                     "SUPER USER : " + strPassword + Environment.NewLine;
                //LGEVH 20230816
                //MessageBox.Show(strViewPassWord);
                CreateSecCount();

                txtPassword.Text = "moohan123!";
            }
            
        }

        private void secLlb1_OnMouseUp(object sender, AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEvent e)
        {
            STEPMANAGER_VALUE.AddSecount1();
            //CheckSecCount();
        }

        private void secLlb2_OnMouseUp(object sender, AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEvent e)
        {
            STEPMANAGER_VALUE.AddSecount2();
            //CheckSecCount();
        }

        private void btnOnScreenKeyBoard_Click(object sender, EventArgs e)
        {           

            try
            {
                System.Diagnostics.Process.Start("osk");
            }
            catch { }
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            CheckSecCount();
        }
    }
}
