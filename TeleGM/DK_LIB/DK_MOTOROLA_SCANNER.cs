using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GmTelematics
{
    enum SSICOMMAND
    {
        NONE, CONNECT1, CONNECT2, TRIGGERON, TRIGGEROFF, RECIEVEOK, END
    }

    enum CONNECTSTATE
    {
        PORTCLOSE, IDLE, CONNSTEP1, CONNSTEP2, CONNECTED, DISCONNECTED, END
    }
    delegate void EventMotorola(int iStatus, string strData, string strErrMsg);      //이벤트 날릴 대리자        
        

    class DK_MOTOROLA_SCANNER
    {
        private SerialPort ComSerial;
        private Thread ReadThread;
        private Thread ConnThread;   
        private int  bConnected;
        private bool bLiveThread;
        private List<byte> bSample;
        private bool bAutoScan;        
        public event EventMotorola MotorolaBarcodeEvent;
        private System.Diagnostics.Stopwatch swScanTimer;// = new System.Diagnostics.Stopwatch(); 
        private System.Diagnostics.Stopwatch swRecvTimer;// = new System.Diagnostics.Stopwatch(); 
        

        public DK_MOTOROLA_SCANNER() 
        {
            bAutoScan = false;
            swScanTimer = new System.Diagnostics.Stopwatch();
            swRecvTimer = new System.Diagnostics.Stopwatch(); 
            bAutoScan = false;
            bConnected = (int)CONNECTSTATE.DISCONNECTED;
            ComSerial  = new SerialPort();
            bSample    = new List<byte>();
        }

        public bool Connect(string strComport, int iBaudRate)
        {
            swTimerStopReset();

            if (IsConnected() == (int)CONNECTSTATE.CONNECTED)
            {
                DisConnect();
            }
            bSample.Clear();            
            ComSerial.PortName = strComport;
            ComSerial.BaudRate = iBaudRate;
            ComSerial.Parity = Parity.None;
            ComSerial.DataBits = 8;
            ComSerial.StopBits = StopBits.One;
            bLiveThread = false;
            try
            {
                ComSerial.Open();
                bConnected = (int)CONNECTSTATE.IDLE;
            }
            catch(Exception ex)
            {
                string tmpStr = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "][ZebraScanner] COMPORT OPEN ERROR. - " + ex.Message;
                MotorolaBarcodeEvent((int)STATUS.ERROR, "", tmpStr);
                return false;
            }

            ThreadObjectKill(ReadThread);
            ThreadObjectKill(ConnThread);

            bLiveThread = true;
            ReadThread = new Thread(SerialScanning);
            ReadThread.Start();
            
            ConnThread = new Thread(AutoConnecting);            
            ConnThread.Start();
            return true;
        }

        private void ThreadObjectKill(Thread targetThread)
        {
            try
            {
                if (targetThread != null && targetThread.IsAlive)
                {
                    ConnThread.Join(100);
                }

                if (targetThread != null && targetThread.IsAlive)
                {
                    ConnThread.Abort();
                }
            }
            catch 
            {
            	
            }
            
        }

        public void DisConnect()
        {
            ScanRelease();
            AutoScanDisable();   
            bLiveThread = false;
            bConnected = (int)CONNECTSTATE.DISCONNECTED;
            ThreadObjectKill(ReadThread);
            ThreadObjectKill(ConnThread);

            if (ComSerial != null && ComSerial.IsOpen)
            {
                ComSerial.Close();
                bSample.Clear();                
            }

            string tmpStr = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "][ZebraScanner] DISCONNECTED.";
            MotorolaBarcodeEvent((int)STATUS.ERROR, "", tmpStr);
        }

        public int IsConnected()
        {
            if (!ComSerial.IsOpen) return (int)CONNECTSTATE.PORTCLOSE;
            return bConnected;
        }
     
        private byte[] GetCommandPack(int iCommandNumber)
        {
            byte[] bReturn;
            switch (iCommandNumber)
            {
                case (int)SSICOMMAND.CONNECT1:
                            bReturn = new byte[]{0x07, 0xc6, 0x04, 0x00, 0xff, 0x8a, 0x08, 0xfd, 0x9e};
                            break;
                case (int)SSICOMMAND.CONNECT2:
                            bReturn = new byte[]{0x07, 0xc6, 0x04, 0x00, 0xff, 0xee, 0x01, 0xfd, 0x41};
                            break;
                case (int)SSICOMMAND.TRIGGERON:
                            bReturn = new byte[]{0x04, 0xe4, 0x04, 0x00, 0xff, 0x14};
                            break;
                case (int)SSICOMMAND.TRIGGEROFF:                     
                            bReturn = new byte[]{0x04, 0xe5, 0x04, 0x00, 0xff, 0x13};
                            break;
                case (int)SSICOMMAND.RECIEVEOK: 
                            bReturn = new byte[]{0x04, 0xD0, 0x04, 0x00, 0xff, 0x28};
                            break;
                default:
                            bReturn = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
                            break;

            }

            return bReturn;
        }

        public void AutoScanEnable()
        {            
            
            bAutoScan = true;
            if (bConnected == (int)CONNECTSTATE.CONNECTED)
                Fire();
            
        }

        public void AutoScanDisable()
        {
            bAutoScan = false;
            swTimerStopReset();
            
        }

        private void AutoConnecting()
        {
            byte[] bSendPack;

            while (bLiveThread)
            {
                try
                {
                    if (bConnected != (int)CONNECTSTATE.CONNECTED)
                    {
                        if (!ComSerial.IsOpen) return;


                        switch (bConnected)
                        {
                            case (int)CONNECTSTATE.IDLE:
                                bSendPack = GetCommandPack((int)SSICOMMAND.CONNECT1);
                                ComSerial.Write(bSendPack, 0, bSendPack.Length);
                                break;

                            case (int)CONNECTSTATE.CONNSTEP1:
                                bSendPack = GetCommandPack((int)SSICOMMAND.CONNECT2);
                                ComSerial.Write(bSendPack, 0, bSendPack.Length);
                                break;

                        }
                        Thread.Sleep(200);
                    }
                }
                catch 
                {
                	
                }
                Thread.Sleep(1);
            }

        }

        private void swTimerStopReset()
        {
            swScanTimer.Stop();
            swScanTimer.Reset();
        }

        private void SerialScanning()
        {
            while (bLiveThread)
            {
                if (!ComSerial.IsOpen)
                {
                    return;
                }

                if (bAutoScan)
                {
                    if (DataScanProcess() > 0)
                    {
                        swTimerStopReset();
                        DataAnalizeProcess();
                    }
                    else
                    {
                        if (!swScanTimer.IsRunning) swScanTimer.Start();
                        ScannerTrigger();
                    }
                }
                else
                {
                    SerialBufferClearing();
                }
                
                Thread.Sleep(1);

            }
        }

        private void ScannerTrigger()
        {                                    
            
            if (swScanTimer.ElapsedMilliseconds > 5000 && bSample.Count == 0)
            {
                Fire();
                swScanTimer.Restart();
            }
                  
        }

        private void Fire()
        {
            ScanRelease();
            Thread.Sleep(250);
            ScanPull();
        }

        private void ScanRelease()
        {
            try
            {
                byte[] bSendPack;
                bSendPack = GetCommandPack((int)SSICOMMAND.TRIGGEROFF);
                ComSerial.Write(bSendPack, 0, bSendPack.Length);
            }
            catch { }            
        }

        private void ScanPull()
        {
            try
            {
                byte[] bSendPack;
                bSendPack = GetCommandPack((int)SSICOMMAND.TRIGGERON);
                ComSerial.Write(bSendPack, 0, bSendPack.Length);
            }
            catch { }
        }

        private void SerialBufferClearing()
        {
            if (bConnected == (int)CONNECTSTATE.CONNECTED)
            {
                if (ComSerial.BytesToRead > 0)
                {
                    try
                    {
                        ComSerial.DiscardInBuffer();
                        bSample.Clear();
                    }
                    catch { }
                }
            }
        }

        private int DataScanProcess()
        {            
            int iBufCount = 0;
            try
            {
                iBufCount = ComSerial.BytesToRead;    //버퍼에 데이터가 들어있는지 확인한다.
                if (iBufCount > 0)                    //버퍼에 데이터가 들어있으면 
                {
                    byte[] tmpChar = new byte[iBufCount];
                    ComSerial.Read(tmpChar, 0, iBufCount);
                    bSample.AddRange(tmpChar);
                }

            }
            catch { }

            return bSample.Count;
        }

        private string DeleteNoneAscii(string strOrigin)
        {
            strOrigin = strOrigin.Replace("\0", String.Empty); //널값 제거
            int i = strOrigin.Length;
            string pattern = "[^ -~]*";
            System.Text.RegularExpressions.Regex reg_exp = new System.Text.RegularExpressions.Regex(pattern);

            string strRemove = reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거
            strRemove = strRemove.Trim(); //공백 제거
            //return reg_exp.Replace(strOrigin, ""); //이것은 걸러내는거

            return strRemove;
        }

        private void CheckRecvTimeOut()
        {
            if (!swRecvTimer.IsRunning) swRecvTimer.Start();
            else
            {
                if (swRecvTimer.ElapsedMilliseconds > 2000)
                {
                    if (bSample.Count > 0)
                    {
                        string strBarcode = BitConverter.ToString(bSample.ToArray()).Replace("-", " ");
                        string tmpStr = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "][ZebraScanner] TIMEOUT : " + strBarcode;
                        MotorolaBarcodeEvent((int)STATUS.ERROR, strBarcode, tmpStr);
                    }
                    
                    Fire();
                    swScanTimer.Restart();
                    bSample.Clear();
                    swRecvTimer.Reset();
                }
            }
        }

        private void DataAnalizeProcess()
        {
            try
            {
                if (bSample.Count < 6)
                {
                    CheckRecvTimeOut();
                    return;
                } 

                if (bSample.Count > 255)
                {
                    CheckRecvTimeOut();
                    bSample.Clear(); 
                    return;
                }

                int iDataLen = (int)bSample[0] + 2;

                if (bSample.Count < iDataLen)
                {
                    CheckRecvTimeOut();
                    return;
                }

                if (bConnected != (int)CONNECTSTATE.CONNECTED) //아직 접속이 되지 않은 상태라면
                {
                    if (bSample[0] == 0x04 && bSample[1] == 0xD0 &&
                            bSample[2] == 0x00 && bSample[3] == 0x00 &&
                                    bSample[4] == 0xFF && bSample[5] == 0x2C)
                    {

                        switch (bConnected)
                        {
                            case (int)CONNECTSTATE.IDLE:
                                bConnected = (int)CONNECTSTATE.CONNSTEP1;
                                break;
                            case (int)CONNECTSTATE.CONNSTEP1:
                                bConnected = (int)CONNECTSTATE.CONNECTED;
                                string tmpStr = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "][ZebraScanner] CONNECTED.";
                                MotorolaBarcodeEvent((int)STATUS.ERROR, "", tmpStr);
                                byte[] bSendPack = GetCommandPack((int)SSICOMMAND.TRIGGERON);
                                ComSerial.Write(bSendPack, 0, bSendPack.Length);
                                swScanTimer.Start();
                                break;
                        }

                        bSample.Clear();
                    }

                }
                else
                {
                    if (bSample[0] == 0x04 && bSample[1] == 0xD0 &&
                            bSample[2] == 0x00 && bSample[3] == 0x00 &&
                                    bSample[4] == 0xFF && bSample[5] == 0x2C)
                    {
                        //일반 OK
                        bSample.Clear();
                    }

                    if (bSample[1] == 0xF3 && bSample[2] == 0x00 && bSample[3] == 0x00 && bSample.Count == iDataLen)
                    {
                        //이거면 바코드를 읽은거다.
                        string strBarcode = String.Empty;
                        if (DecodeCheckSum(bSample.ToArray()))
                        {
                            byte[] bDecode = new byte[iDataLen - 7];

                            for (int i = 0; i < bDecode.Length; i++)
                            {
                                bDecode[i] = bSample[5 + i];
                            }
                            strBarcode = DeleteNoneAscii(Encoding.UTF8.GetString(bDecode));
                            string tmpStr = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "][ZebraScanner] READ : " + strBarcode;
                            MotorolaBarcodeEvent((int)STATUS.OK, strBarcode, tmpStr);                            
                        }
                        else
                        {
                            strBarcode = BitConverter.ToString(bSample.ToArray()).Replace("-", " ");
                            string tmpStr = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "][ZebraScanner] CHECKSUM ERROR : " + strBarcode;
                            MotorolaBarcodeEvent((int)STATUS.ERROR, strBarcode, tmpStr);
                        }

                        byte[] bSendPack = GetCommandPack((int)SSICOMMAND.RECIEVEOK);
                        ComSerial.Write(bSendPack, 0, bSendPack.Length);
                        bSample.Clear();
                        swRecvTimer.Reset();
                    }
                    else
                    {
                        CheckRecvTimeOut();
                    }
                    
                }
                
            }
            catch
            {
            	
            }
            
        }

        private bool DecodeCheckSum(byte[] bData)
        {   
            int isum = 0x0000;

            for (int i = 0; i < bData.Length - 2; i++)
            {
                isum += bData[i];
            }
            int iNot = ~isum;

            string strChksum = iNot.ToString("x2").PadLeft(16, '0');
            string strChksumHigh = strChksum.Substring(12, 2);
            string strChksumLow = strChksum.Substring(14, 2);

            byte bHigh = Convert.ToByte(strChksumHigh, 16);
            byte bLow = Convert.ToByte(strChksumLow, 16);
            bLow++;

            if (bData[bData.Length - 1] == bLow && bData[bData.Length - 2] == bHigh)
                return true;            
            else 
                return false;
            
        }

    }
}
