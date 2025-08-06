using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GmTelematics
{
    class DK_PAGE
    {

        private FrmMsgPop frmMsg = null;
        private int iPopNumber = (int)POPTYPE.NOPOP;
        public DK_PAGE() { }

        public int isPopUp()
        {
            return iPopNumber;
        }
        private bool CommandPage(string strCommand, string strOption, bool[] bUseSlots)
        {
            return true;
        }
        public void MsgPopUpFocus()
        {
            if (frmMsg != null && iPopNumber != (int)POPTYPE.NOPOP)
            {
                frmMsg.Invoke(new MethodInvoker(delegate() { frmMsg.TopMost = true; frmMsg.Focus(); }));
            }
        }
         
        public int MsgPopUp(string strTitle, string strMsg, int iBtnType)
        {
            iPopNumber = (int)POPTYPE.POPTWOBTN;
            frmMsg = new FrmMsgPop(strTitle, strMsg);
            int iPosLeft = (int)(STEPMANAGER_VALUE.iPopPosLeft / 2) - (frmMsg.Width / 2);
            frmMsg.StartPosition = FormStartPosition.Manual;
            frmMsg.Location = new System.Drawing.Point(iPosLeft, STEPMANAGER_VALUE.iPopPosTop + 20);
            frmMsg.TopMost = true;
            switch (iBtnType)
            {
                case (int)POPBTNTYPE.NORMAL: frmMsg.ShowDialog();
                                             frmMsg = null;
                                             iPopNumber = (int)POPTYPE.NOPOP;
                                             return (int)STATUS.OK;
                                            
                case (int)POPBTNTYPE.CONTINUE: frmMsg.SetButton("CONTINUE", "STOP", true);
                                               break;
                case (int)POPBTNTYPE.OKNG: frmMsg.SetButton("OK", "NG", true); 
                                           break;
                    
    
            }            
            frmMsg.ShowDialog(); 
            iPopNumber = (int)POPTYPE.NOPOP;
            return frmMsg.GetBtnValue();
        }

        public void MsgPopDown(bool bOK)
        {
            //Prevent 2015.03.26 DK.SIM  
            
            if (iPopNumber != (int)POPTYPE.NOPOP || !frmMsg.IsDisposed )
            {
                try
                {
                    if (bOK) { frmMsg.CloseBtnOKKey(); }
                    else { frmMsg.CloseBtnNGKey(); }
                    
                }
                catch (System.Exception ex)
                {                    
                    string strEx = ex.Message;
                }
            }
        }

    }
}
