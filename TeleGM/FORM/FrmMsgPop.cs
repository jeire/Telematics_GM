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
    public partial class FrmMsgPop : Form
    {
        private bool bDoubleMode = false;
        private int iBtnValue = (int)STATUS.NONE;
        private string strM1 = String.Empty;
        private string strM2 = String.Empty;
        public FrmMsgPop(string Msg1, String Msg2)
        {
            InitializeComponent();
            strM1 = Msg1;
            strM2 = Msg2;
            
        }
        public void SetButton(string strOKname, string strNGname, bool bVisible)
        {
                bDoubleMode = bVisible;
                btnOK.Text = strOKname;
                btnNG.Text = strNGname;
                btnNG.Visible = bDoubleMode;
            
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            iBtnValue = (int)STATUS.OK;
            this.Close();

        }

        public void CloseBtnOKKey()
        {
            this.Invoke(new MethodInvoker(delegate() { btnOK.PerformClick(); }));
            
        }
        public void CloseBtnNGKey()
        {
            if(bDoubleMode)
            this.Invoke(new MethodInvoker(delegate() { btnNG.PerformClick(); }));

        }
       

        private void FrmMsgPop_Shown(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() {
                btnOK.Focus();
                txtTitle.Text = strM1; txtBox.Text = strM2; 
            }));
            
        }

        private void FrmMsgPop_Leave(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate() { this.Focus(); }));
        }

        private void btnNG_Click(object sender, EventArgs e)
        {
            iBtnValue = (int)STATUS.NG;
            this.Close();
        }

        public int GetBtnValue()
        {
            return iBtnValue;
        }
    }
}
