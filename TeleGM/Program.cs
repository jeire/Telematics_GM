using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace GmTelematics
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FrmFaMain());
         
                                    
            bool createdNew = false;
            //Prevent 2015.03.26 DK.SIM 
            Mutex dup = null;

            try
            {
                dup = new Mutex(true, "GM TELEMATICS TESTER", out createdNew);   // 프로그램명은 WinForm.Text 명을 일컫는다.

                if (createdNew)
                {
                    //Application.EnableVisualStyles();
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrmFaMain());
                    dup.ReleaseMutex();
                }
                else
                {
                    //중복 실행 중일때 하는 일들, (아무것도 하지말자)                    
                }
                //Prevent 2015.03.26 DK.SIM 
                if (dup != null) ((IDisposable)dup).Dispose();
                
            }
            
            catch(System.Exception ex)
            {
                if (ex.Message.Contains("0x80040154"))
                {
                    RegisterOcx();
                }
                else
                {
                    string strEx = "Source : " + ex.Source + ", InnerException : " + ex.InnerException + ", Message : " + ex.Message;                
                    MessageBox.Show(strEx);
                }       
            }
            finally
            {
                if (dup != null) ((IDisposable)dup).Dispose();
            }   
            
        }
        //수행시 라이브러리문제나 static 선언 이전에 발생하는예외로 프로세스가 그냥 죽을때 처리해놓으면 문제를 그나마 상세히 확인할 수 있다.
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating == true)
            {
                //MessageBox.Show(e.ExceptionObject.ToString());
            }
        }

        static void RegisterOcx()
        {
            string strPath = AppDomain.CurrentDomain.BaseDirectory;

            string fileinfo = "/s" + " " + "\"" + strPath + "iPlotLibrary.ocx\"";
            //"\"" + strPath + "iPlotLibrary.ocx\"";    //결과화면 뿌리기
            //"/s" + " " + "\"" + strPath + "iPlotLibrary.ocx\""; //결과화면 숨기기
            //"/u" + " " + "\"" + strPath + "iPlotLibrary.ocx\""; //dll 등록해제

            
            System.Diagnostics.Process reg = new System.Diagnostics.Process();            
            
            try
            {
                reg.StartInfo.FileName = "regsvr32.exe";
                reg.StartInfo.Arguments = fileinfo;
                reg.StartInfo.UseShellExecute = false;
                reg.StartInfo.CreateNoWindow = true;
                reg.StartInfo.RedirectStandardOutput = true;
                reg.StartInfo.Verb = "runas";
                reg.Start();
                reg.WaitForExit();
                reg.Close();
            }
            catch 
            {
                reg.Kill();
                reg.Dispose();
            }
        }
    }
}
