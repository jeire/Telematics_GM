using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace GmTelematics
{
    delegate void EventTimer(double dTime, string strDate); //이벤트 날릴 대리자

    class DK_PLAYCHECKER
    {      
        //OPTION
        private bool bGmes;
        private bool bRun;
        private bool bManual;

        //TEST TIME
        private double fMS;

        private System.Threading.Timer threadTimer;
        private System.Diagnostics.Stopwatch swInspectionTimer = new System.Diagnostics.Stopwatch();

        public event EventTimer SendTime;      //대리자가 날릴 실제 시간
                
        public bool Item_bManual
        {
            get { return bManual; }
            set { bManual = value; }
        }
        public bool Item_GMES
        {
            get { return bGmes; }
            set { bGmes = value; }
        }

        public bool Item_RUN
        {
            get { return bRun; }
            set { bRun = value; }
        }

        public DK_PLAYCHECKER() 
        {            
            initialize();
        }

        private void initialize()
        {
            Item_GMES    = false;
            Item_RUN     = false;
            Item_bManual = false;
            fMS          = 0.0;
            
            threadTimer    = new System.Threading.Timer(CallBack);
            swInspectionTimer.Reset();

        }

        void CallBack(object status)
        {
            TimerProcess();
        }
        
        public void TimerStart()
        {
            fMS = 0.0;
            swInspectionTimer.Restart();
            threadTimer.Change(0, 100);
        }
        public void TimerStop()
        {
            swInspectionTimer.Stop();
            threadTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
         
        private void TimerProcess()
        {
            DateTime dtn = DateTime.Now;

            string strDate = dtn.ToString("yyyy-MM-dd");

            fMS = (double)((double)swInspectionTimer.ElapsedMilliseconds / (double)1000);

            if (fMS > 9999) { fMS = 9999.9; }                        
            
            SendTime(fMS, strDate);
        }

        public double GetCurrentInspectionTime()
        {
            return (double)((double)swInspectionTimer.ElapsedMilliseconds / (double)1000);
        }
    }


}
