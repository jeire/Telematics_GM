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
    public partial class FrmTest : Form
    {
        Panel panelPan = new Panel();
        CheckBox[] chkBoxOK = new CheckBox[2];
        CheckBox[] chkBoxNG = new CheckBox[2];

        public FrmTest(string strTestName, bool[] bNotUse)
        {
            InitializeComponent();
            CreateTitleName(strTestName);
            //CreateCheckBox(bNotUse);
            Position();           
        }

        private void Position()
        {
            int ScreenWidth  = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            btnExit.Left = ScreenWidth - btnExit.Width - 220;
            btnExit.Top =  ScreenHeight - btnExit.Height - 190;
        }

        private void FrmTest_Load(object sender, EventArgs e)
        {

        }

        private void CreateTitleName(string strName)
        {
            Label lblTitle = new Label();

            lblTitle.Text = strName;
            lblTitle.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.BorderStyle = BorderStyle.FixedSingle;
            lblTitle.AutoSize = false;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(560, 65);
            lblTitle.Location = new Point(25, 20);

            
        
            int ScreenWidth  = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            panelPan.Width = 660;
            panelPan.Height = 740;

            panelPan.Left = (int)((ScreenWidth - panelPan.Width) / 3);
            panelPan.Top = (int)((ScreenHeight - panelPan.Height) / 3);

            panelPan.BorderStyle = BorderStyle.FixedSingle;
            panelPan.Controls.Add(lblTitle);
            //this.Controls.Add(lblTitle);
            this.Controls.Add(panelPan);
        }

        private void CreateCheckBox(bool[] bNotUse)
        {
            Label[] lblName = new Label[2];

            for (int i = 0; i < 2; i++)
            {
                lblName[i] = new Label();
                lblName[i].Text = "SET" + (i + 1).ToString();
                lblName[i].Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                lblName[i].BorderStyle = BorderStyle.FixedSingle;
                lblName[i].AutoSize = false;
                lblName[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                lblName[i].Size = new System.Drawing.Size(79, 30);
                lblName[i].Visible = false;

                //Prevent 2015.03.26 DK.SIM  
                //if (i < 2)
                //{
                    lblName[i].Location = new Point(25, 95 + (i * 29));
                //}                
                //else
                //{
                //    lblName[i].Location = new Point(310, 95 + ((i - 20) * 29));
                //}

                panelPan.Controls.Add(lblName[i]);

            }            
            
            for (int i = 0; i < 2; i++)
            {
                chkBoxOK[i] = new CheckBox();
                chkBoxNG[i] = new CheckBox();

                chkBoxOK[i].Name = "cbok" + i.ToString();
                chkBoxOK[i].Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                chkBoxOK[i].ForeColor = System.Drawing.Color.DarkGreen;
                chkBoxOK[i].Text = "OK";

                chkBoxNG[i].Name = "cbng" + i.ToString();
                chkBoxNG[i].Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                chkBoxNG[i].ForeColor = System.Drawing.Color.Crimson;
                chkBoxNG[i].Text = "NG";

                chkBoxOK[i].Width  = 60;
                chkBoxOK[i].Height = 30;
                chkBoxNG[i].Width = 60;
                chkBoxNG[i].Height = 30;

                chkBoxOK[i].Checked = true;
                if (!bNotUse[i + 1])
                {
                    chkBoxOK[i].Checked = false;
                    chkBoxOK[i].Enabled = false;
                    chkBoxNG[i].Enabled = false;
                }
                if (i < 20)
                {
                    int cx1 = lblName[0].Left + lblName[0].Width + 25;
                    chkBoxOK[i].Location = new System.Drawing.Point(cx1, 95 + i * 29);
                    int cx2 = chkBoxOK[i].Left + chkBoxOK[i].Width + 20;
                    chkBoxNG[i].Location = new System.Drawing.Point(cx2, 95 + i * 29);
                }
                else
                {
                    int cx1 = lblName[20].Left + lblName[20].Width + 25;
                    chkBoxOK[i].Location = new System.Drawing.Point(cx1, 95 + ((i - 20) * 29));
                    int cx2 = chkBoxOK[i].Left + chkBoxOK[i].Width + 20;
                    chkBoxNG[i].Location = new System.Drawing.Point(cx2, 95 + ((i - 20) * 29));
                }


                chkBoxOK[i].CheckedChanged += new EventHandler(CheckBoxOKClick);
                chkBoxNG[i].CheckedChanged += new EventHandler(CheckBoxNGClick);

                panelPan.Controls.Add(chkBoxOK[i]);
                panelPan.Controls.Add(chkBoxNG[i]);


            }
        }

        private void CheckBoxOKClick(object sender, EventArgs e)
        {
            string strCbName = ((CheckBox)sender).Name.ToString();
            string strCbArry = strCbName.Replace("cbok", "");

            bool bChk = ((CheckBox)sender).Checked;
            if (bChk) chkBoxNG[int.Parse(strCbArry)].Checked = false;
            else chkBoxNG[int.Parse(strCbArry)].Checked = true;

        }

        private void CheckBoxNGClick(object sender, EventArgs e)
        {
            string strCbName = ((CheckBox)sender).Name.ToString();
            string strCbArry = strCbName.Replace("cbng", "");

            bool bChk = ((CheckBox)sender).Checked;
            if(bChk) chkBoxOK[int.Parse(strCbArry)].Checked = false;
            else chkBoxOK[int.Parse(strCbArry)].Checked = true;
        }


        public bool GetCheckOKValue(int iPort)
        {
            return chkBoxOK[iPort - 1].Checked;            
        }

        public bool GetCheckNGValue(int iPort)
        {
            return chkBoxNG[iPort - 1].Checked;
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

