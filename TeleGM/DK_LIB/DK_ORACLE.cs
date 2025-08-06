//#define USE_MOOHAN_SERVER //MOOHAN 서버에서 오라클 테스트할경우 선언

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;

namespace GmTelematics
{
    enum ProcedureIndex
    {
        NONE,
        //FA
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_CHECK,
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_COMPLETE,
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_MASTER_INSERT,
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_DETAIL_INSERT,
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_MODEL_INFO,
        //ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GM_GET_TXN_ID, 다시 삭제.. CNS 정진섭.2016.11.14
   
        //FA - KEYWRITE
        ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN,
        ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_TCP,
        ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_PSA,
        ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_SET_MAIN,

        //OOB
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO,
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO_PSA,
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_SET_GSM_INFO,

        //PCB
        ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_CHECK,
        ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_COMPLETE,
        ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_MASTER_INSERT,
        ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_DETAIL_INSERT,
        ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_MODEL_INFO,
        //ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GM_GET_TXN_ID, 다시 삭제.. CNS 정진섭.2016.11.14

        //TIME
        ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_SERVER_TIME_SYNC,
        ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_SERVER_TIME_SYNC,
        

        //MOOHAN SERVER 용    
        #if USE_MOOHAN_SERVER
        GET_SERVER_TIME_SYNC,
        GEN10_STEP_CHECK,
        GEN10_STEP_COMPLETE,
        GEN10_MASTER_INSERT,
        GEN10_DETAIL_INSERT,
        GEN10_GET_MODEL_INFO,
        GEN10_GM_GET_MAIN,
        GEN10_GM_SET_MAIN,
        GEN10_GET_GSM_INFO,
        GEN10_SET_GSM_INFO,

        GEN10_PCB_STEP_CHECK,
        GEN10_PCB_STEP_COMPLETE,
        GEN10_PCB_MASTER_INSERT,
        GEN10_PCB_DETAIL_INSERT,
        GEN10_PCB_GET_MODEL_INFO,
        #endif
        END    
    }
    
    public struct Oracle_Procedure
    {
        public int      iProcedureIndex;
        public string   strPackageName;        
        public string[] strIn;        
        public string[] strOut;
        public string[] strFieldname;
        public string[] strFieldValue;
        
    }

    public struct ORACLEINFO
    {
        public string strProductionLine;
        public string strProcessCode;
        public string strPCID;
        public string strServerIP;
        public string strPort;
        public string strServiceName;
        public string strOOBCode;
        public string strOOBFlag;
        public string strCallType;

        public bool bDontCareOOBcode;
    }

    class DK_ORACLE
    {
        public event EventRealTimeMsg OracleSendReport;

        private OracleConnection oraConn;
        private ConnectionState  oraConnState;
        private string           strConnString;

        private DK_LOGGER        DKLogger;
        private string strSvrIpAddress;
        public DK_ORACLE()
        {               
            oraConn = new OracleConnection();
            oraConnState = oraConn.State;
            strConnString = strSvrIpAddress = String.Empty;
            DKLogger = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_Message);     
            
        }

        private void GateWay_Message(string cParam) //로깅할때 데이터가 다시 실시간으로 리턴되면 actor로 보내자.
        {            
            OracleSendReport(0, cParam);
        }

        #region Connection_Oracle

        private void WriteLogging(string strlog)
        {
            DKLogger.WriteCommLog(strlog, "ORACLE", false);
        }

        public  bool IsConnected(ref string strReason)
        {
            if (oraConn == null)
            {
                strReason = "NOT CONNECTED";                
                return false;
            }

            oraConnState = oraConn.State;

            switch (oraConnState)
            {
                case ConnectionState.Open:
                    strReason = "CONNECTED - " + oraConn.ServerVersion;                    
                    return true;

                default:
                    string strPingResult = String.Empty;
                    int iTTL = 0;
                    long lTime = 0;
                    DKLogger.NetworkPingTest(strSvrIpAddress, 2, ref strPingResult, ref iTTL, ref lTime);
                    strReason = "DISCONNECTED(" + oraConnState.ToString() + ")  Network Ping(" + strPingResult + ")";  
                    return false;
            }

        }

        public  void Connect(ref string strReason)
        {
            if (strConnString == null || strConnString.Length < 1)
            {
                strReason = "Not Found ConnectionString.";                
                return;
            }

            switch (oraConnState)
            {
                case ConnectionState.Broken:
                case ConnectionState.Closed:
                                                 break;
                case ConnectionState.Connecting: 
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                case ConnectionState.Open:
                                                 strReason = "Connection Status : " + oraConnState.ToString();                                                 
                                                 return;
                default: break;

            }
            try
            {  
                                
                oraConn.ConnectionString = strConnString;                
                oraConn.Open();
                oraConnState = oraConn.State;
                strReason = "Trying Connect.";
            }
            catch (System.Exception ex)
            {
                string strPingResult = String.Empty;
                int iTTL = 0;
                long lTime = 0;
                DKLogger.NetworkPingTest(strSvrIpAddress, 2, ref strPingResult, ref iTTL, ref lTime);
                strReason = "Connect Fail(" + ex.Message + ") Network Ping(" + strPingResult + ")";  
            }
            
            
        }

        public bool SetSystemTimeSync(string strCalltype, ref string strReason)
        {
            //커넥션 체크
            if (!IsConnected(ref strReason))
            {                
                return false;
            }

            Oracle_Procedure opData = new Oracle_Procedure();
           
            switch (strCalltype)
            {
                case "PCB" : //PCB
                        opData.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_SERVER_TIME_SYNC;
                        break;
                default: //FA, OOB 등 기타.
                        opData.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_SERVER_TIME_SYNC;
                        break;
            }

            //MOOHAN SERVER 임시 TEST 프로시져
            #if USE_MOOHAN_SERVER
            opData.iProcedureIndex = (int)ProcedureIndex.GET_SERVER_TIME_SYNC;
            #endif

            string strexReason = String.Empty;

            bool bProc = MakeOracleProcedureStructure(ref opData, ref strexReason);
            if (bProc)
            {
                bProc = CallProcedure(ref opData, ref strexReason);

                if (bProc)
                {
                    string strReturnTime = String.Empty;
                    string strReturnMsg  = String.Empty;

                    for (int i = 0; i < opData.strFieldname.Length; i++)
                    {
                       if(opData.strFieldname[i].Equals("SERVER_TIME"))
                       {
                           strReturnTime = opData.strFieldValue[i].Trim(); //
                       }

                       if (opData.strFieldname[i].Equals("ERRMSG"))
                       {
                           strReturnMsg = opData.strFieldValue[i].Trim();
                       } 
                    }
                    //여기서 변경작업.
                    //2016-05-03 19:22:41
                    strReturnMsg = strReturnMsg.ToUpper();
                    
                    if (strReturnMsg.Equals("OK") || strReturnMsg.Equals("TRUE"))
                    {
                        DateTime dt;    
                        try
                        {
                            dt = Convert.ToDateTime(strReturnTime);
                            DK_CHANGETIME dkTime = new DK_CHANGETIME();
                            if (!dkTime.SetSystemDateTime(dt))
                            {
                                strReason = "The security permissions are insufficient to set the system time.";
                                strReason = strReason.ToUpper();                                
                                return true;
                            }
                        }
                        catch 
                        {
                            strReason = "DATE TIME FORMAT ERROR : " + strReturnTime;                            
                            return true;        
                        }

                    }

                    strReason = "SUCCESS";                    
                    return true;
                }
                else
                {                    
                    strReason = "CallProcedure Fail : " + strexReason;                    
                    return false;
                }
            }
            else
            {                
                strReason = "MakeOracleProcedureStructure Fail : " + strexReason;                
                return false;
            }

        }

        /* 다시 삭제.. CNS 정진섭.2016.11.14
        public bool GetTxnid(string strCalltype, ref string strTxnId, ref string strReason)
        {

            Oracle_Procedure opData = new Oracle_Procedure();

            switch (strCalltype)
            {
                case "PCB": //PCB
                    opData.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GM_GET_TXN_ID;
                    break;
                default: //FA, OOB 등 기타.
                    opData.iProcedureIndex = (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GM_GET_TXN_ID;
                    break;
            }

            string strexReason = String.Empty;

            bool bProc = MakeOracleProcedureStructure(ref opData, ref strexReason);
            if (bProc)
            {
                bProc = CallProcedure(ref opData, ref strexReason);

                if (bProc)
                {
                    strTxnId = String.Empty;      

                    if (opData.strFieldname[0].Equals("TXNID"))
                    {
                        try
                        {
                            strTxnId = opData.strFieldValue[0].Trim(); //

                            if (!String.IsNullOrEmpty(strTxnId))
                            {                               
                                strReason = "SUCCESS";
                                return true;
                            }
                            else
                            {
                                strReason = "TXNID : null";
                                return false;
                            }
                        }
                        catch
                        {
                            strReason = "DATA ERROR : " + strTxnId;
                            return false;
                        }
                    }

                    strReason = "DATA FORMAT ERROR : " + strexReason;
                    return false;
                }
                else
                {
                    strReason = "CallProcedure Fail : " + strexReason;
                    return false;
                }
            }
            else
            {
                strReason = "MakeOracleProcedureStructure Fail : " + strexReason;
                return false;
            }

        }
        */

        public  void DisConnect()
        {            
            oraConn.Close();
            oraConnState = oraConn.State;
        }

        public  string[] GetPackageList()
        {
            string[] strPackageList = new string[(int)ProcedureIndex.END-1];
           
            int j = 0;
            string strTemp = String.Empty;
            for(int i = (int)ProcedureIndex.NONE + 1; i < (int)ProcedureIndex.END; i++)
            {
                strTemp = (ProcedureIndex.NONE + i).ToString();

                strPackageList[j] = strTemp.Replace("_Escape_", ".");
                j++;
            }

            return strPackageList;
        }

        public  bool SetConnectionString(string strConnIP, string strConnPort, string strConnServiceName, ref string strConnInfo)
        {
            if (String.IsNullOrEmpty(strConnIP) ||
                 String.IsNullOrEmpty(strConnPort) ||
                   String.IsNullOrEmpty(strConnServiceName))
            {                
                return false;
            }
            strSvrIpAddress = strConnIP;
            strConnString = "Data Source=(DESCRIPTION="
                + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + strConnIP + ")(PORT=" + strConnPort + ")))"
                + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + strConnServiceName + ")));"

                //MOOHAN SERVER
                #if USE_MOOHAN_SERVER
                + "User Id=scott;Password=tiger;"; 
                #else
                + "User Id=oraspc;Password=spcora00;";
                #endif
    

            strConnInfo = strConnString;            
            return true;            
        }

#endregion

        public  bool GetProcedureInputCount(int iProcedureIdx, ref string strProcedureName, ref int iInputCount, ref string strInputInformation, ref string strOutputInformation)
        {
            iInputCount = 0;
            strProcedureName = String.Empty;
            string strReason = String.Empty;

            Oracle_Procedure OPstruct = new Oracle_Procedure();
            OPstruct.iProcedureIndex = iProcedureIdx;

            bool bRes = MakeOracleProcedureStructure(ref OPstruct, ref strReason);
            
            if(bRes)
            {
                iInputCount = OPstruct.strIn.Length;
                strProcedureName = OPstruct.strPackageName;

                OracleCommand Oclcmd = new OracleCommand();
                Oclcmd.Connection = oraConn;
                Oclcmd.CommandType = CommandType.StoredProcedure;
                Oclcmd.CommandText = OPstruct.strPackageName;

                bool[] bInOutFlag = new bool[OPstruct.strIn.Length + OPstruct.strOut.Length]; //false:IN  true:OUT

                if (!MakeOclCommandFile(ref OPstruct, ref Oclcmd, ref strReason, ref bInOutFlag))
                {                    
                    return false;
                }
                else
                {                    
                    strInputInformation = String.Empty;
                    strOutputInformation = String.Empty;
                    for (int i = 0; i < Oclcmd.Parameters.Count; i++)
                    {
                        if (bInOutFlag[i])
                        { 
                            strOutputInformation += "[" + Oclcmd.Parameters[i].ParameterName + "]";
                        }
                        else
                        {
                            strInputInformation += "[" + Oclcmd.Parameters[i].ParameterName + "]";
                        }
                    }
                }

            }            
            return bRes;
        }

        public  bool CallProcedure(ref Oracle_Procedure OPstruct, ref string strReason)
        {
            //커넥션 체크 - 연결되어있었어도 끊는다. 그리고 매번 새로이 연결한다.
            if (IsConnected(ref strReason))
            {
                DisConnect();
            }
         
            Connect(ref strReason);
            if (!IsConnected(ref strReason))
            {
                Connect(ref strReason);   //2번은 시도한다.
                if (!IsConnected(ref strReason))
                {                    
                    return false;
                }  
            }                                
          
            
            //쿼리문 작성
            OracleCommand Oclcmd = new OracleCommand();
            
            if (!MakeOracleCommandVar(ref OPstruct, ref Oclcmd, ref strReason))
            {
                DisConnect();                
                return false;
            }
            
            //여기서 쿼리시도
            try
            {
                string strTxFullLog = "[TX](" + Oclcmd.CommandText.ToString() + ")";
                if (OPstruct.strIn.Length > 0)
                {
                    for(int i = 0; i < OPstruct.strIn.Length; i++)
                    {
                        if(OPstruct.strIn[i] != null && OPstruct.strIn[i].Length > 0)
                        {
                            strTxFullLog += "<" + OPstruct.strIn[i] + ">";
                        }
                    }
                }
                WriteLogging(strTxFullLog);
                Oclcmd.ExecuteNonQuery();

                for (int i = 0; i < OPstruct.strFieldname.Length; i++)
                {
                    OPstruct.strFieldValue[i] = Oclcmd.Parameters[i].Value.ToString().Trim();
                }

                int j = 0;
                for (int i = 0; i < Oclcmd.Parameters.Count; i++)
                {
                    if(Oclcmd.Parameters[i].Direction == ParameterDirection.Output
                        && Oclcmd.Parameters[i].Value != null && j < OPstruct.strOut.Length)
                    {
                        OPstruct.strOut[j++] = Oclcmd.Parameters[i].Value.ToString().Trim();
                    }
                }

                string strRxFullLog = "[RX](" + Oclcmd.CommandText.ToString() + ")";
               
                for (int i = 0; i < Oclcmd.Parameters.Count; i++)
                {
                    if (OPstruct.strFieldname[i] != null && OPstruct.strFieldValue[i] != null)
                    {
                        strRxFullLog += "<" + OPstruct.strFieldname[i] + ":" + OPstruct.strFieldValue[i] + ">";
                    }
                }
                
                WriteLogging(strRxFullLog);

            }
            catch (System.Exception ex)
            {                
                string strPingResult = String.Empty;
                int iTTL = 0;
                long lTime = 0;
                DKLogger.NetworkPingTest(strSvrIpAddress, 2, ref strPingResult, ref iTTL, ref lTime);
                strReason = "Exception(" + ex.Message + ")  Network Ping(" + strPingResult + ")";  
                DisConnect();                
                return false;
            }

            strReason = "SUCCESS";
            DisConnect();            
            return true;

        }
                
        public  bool MakeOracleProcedureStructure(ref Oracle_Procedure OPstruct, ref string strReason)
        {
            strReason = "SUCCESS";

            if (OPstruct.iProcedureIndex <= (int)ProcedureIndex.NONE || OPstruct.iProcedureIndex >= (int)ProcedureIndex.END)
            {
                strReason = "UNKNOWN PROCEDURE COMMAND.";                
                return false;
            }

            string strPackageName = (OPstruct.iProcedureIndex + ProcedureIndex.NONE).ToString();
            OPstruct.strPackageName = strPackageName.Replace("_Escape_", ".");

            switch (OPstruct.iProcedureIndex)
            {

                //FA
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_CHECK:
                    OPstruct.strIn = new string[1]; OPstruct.strOut = new string[3];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_COMPLETE:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_MASTER_INSERT:
                    OPstruct.strIn = new string[7]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_DETAIL_INSERT:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[0];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_MODEL_INFO:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[2];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[18];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_TCP:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[18];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_PSA:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[20];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_SET_MAIN:
                    OPstruct.strIn = new string[10]; OPstruct.strOut = new string[2];
                    break;

                //OOB
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[10];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO_PSA:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[12];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_SET_GSM_INFO:
                    OPstruct.strIn = new string[3]; OPstruct.strOut = new string[1];
                    break;

                //PCB
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_CHECK:
                    OPstruct.strIn = new string[1]; OPstruct.strOut = new string[3];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_COMPLETE:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_MASTER_INSERT:
                    OPstruct.strIn = new string[7]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_DETAIL_INSERT:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[0];
                    break;
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_MODEL_INFO:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[2];
                    break;                
                //TIME
                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_SERVER_TIME_SYNC:
                    OPstruct.strIn = new string[0]; OPstruct.strOut = new string[2];
                    break;

                case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_SERVER_TIME_SYNC:
                    OPstruct.strIn = new string[0]; OPstruct.strOut = new string[2];
                    break;

           
                //MOOHAN SERVER 용
                //FA
                #if USE_MOOHAN_SERVER
                case (int)ProcedureIndex.GET_SERVER_TIME_SYNC:
                    OPstruct.strIn = new string[0]; OPstruct.strOut = new string[2];
                    break;
                case (int)ProcedureIndex.GEN10_STEP_CHECK:
                    OPstruct.strIn = new string[1]; OPstruct.strOut = new string[3];
                    break;
                case (int)ProcedureIndex.GEN10_STEP_COMPLETE:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.GEN10_MASTER_INSERT:
                    OPstruct.strIn = new string[7]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.GEN10_DETAIL_INSERT:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[0];
                    break;
                case (int)ProcedureIndex.GEN10_GET_MODEL_INFO:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[2];
                    break;
                case (int)ProcedureIndex.GEN10_GM_GET_MAIN:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[18];
                    break;
                case (int)ProcedureIndex.GEN10_GM_SET_MAIN:
                    OPstruct.strIn = new string[10]; OPstruct.strOut = new string[2];
                    break;
                case (int)ProcedureIndex.GEN10_GET_GSM_INFO:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[10];
                    break;
                case (int)ProcedureIndex.GEN10_SET_GSM_INFO:
                    OPstruct.strIn = new string[3]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.GEN10_PCB_STEP_CHECK:
                    OPstruct.strIn = new string[1]; OPstruct.strOut = new string[3];
                    break;
                case (int)ProcedureIndex.GEN10_PCB_STEP_COMPLETE:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.GEN10_PCB_MASTER_INSERT:
                    OPstruct.strIn = new string[7]; OPstruct.strOut = new string[1];
                    break;
                case (int)ProcedureIndex.GEN10_PCB_DETAIL_INSERT:
                    OPstruct.strIn = new string[4]; OPstruct.strOut = new string[0];
                    break;
                case (int)ProcedureIndex.GEN10_PCB_GET_MODEL_INFO:
                    OPstruct.strIn = new string[2]; OPstruct.strOut = new string[2];
                    break;
                #endif
                default:
                    strReason = "NOT FOUND PROCEDURE";                    
                    return false;
            }

            for (int i = 0; i < OPstruct.strIn.Length; i++)
            {
                OPstruct.strIn[i] = String.Empty;
            }

            for (int i = 0; i < OPstruct.strOut.Length; i++)
            {
                OPstruct.strOut[i] = String.Empty;
            }
            
            return true;
        }

        private bool MakeOracleCommandVar(ref Oracle_Procedure OPstruct, ref OracleCommand Oclcmd, ref string strReason)
        {
            strReason = "SUCCESS";

            if (OPstruct.iProcedureIndex <= (int)ProcedureIndex.NONE || OPstruct.iProcedureIndex >= (int)ProcedureIndex.END)
            {
                strReason = "UNKNOWN PROCEDURE COMMAND.";                
                return false;
            }
                        
            Oclcmd.Connection  = oraConn;
            Oclcmd.CommandType = CommandType.StoredProcedure;
            Oclcmd.CommandText = OPstruct.strPackageName;

            bool[] bInOutFlag = new bool[OPstruct.strIn.Length + OPstruct.strOut.Length]; //false:IN  true:OUT

            if(!MakeOclCommandFile(ref OPstruct, ref Oclcmd, ref strReason, ref bInOutFlag))
            {                
                return false;
            }

            if (bInOutFlag.Length != Oclcmd.Parameters.Count)
            {
                strReason = "InOutFlag Count & Parameter Count Error.";                
                return false;
            }

            OPstruct.strFieldname  = new string[Oclcmd.Parameters.Count];
            OPstruct.strFieldValue = new string[Oclcmd.Parameters.Count];

            int iIndex = 0;
            for (int i = 0; i < Oclcmd.Parameters.Count; i++)
            {
                if (bInOutFlag[i])
                    Oclcmd.Parameters[i].Direction = ParameterDirection.Output;
                else
                {
                    Oclcmd.Parameters[i].Direction = ParameterDirection.Input;
                    string strInputData = OPstruct.strIn[iIndex++];
                    try
                    {
                        
                        switch(Oclcmd.Parameters[i].OracleDbType)
                        {
                            case OracleDbType.Varchar2: Oclcmd.Parameters[i].Value = strInputData; break;
                            case OracleDbType.Decimal:  Oclcmd.Parameters[i].Value = int.Parse(strInputData); break;
                            case OracleDbType.Double:   Oclcmd.Parameters[i].Value = Double.Parse(strInputData); break;
                            default: Oclcmd.Parameters[i].Value = strInputData; break;
                        }
                    }
                    catch 
                    {
                        strReason = "Data Type Error :" + Oclcmd.Parameters[i].OracleDbType.ToString() + " - " + strInputData;                        
                        return false;
                    }
                    
                }

                OPstruct.strFieldname[i] = Oclcmd.Parameters[i].ParameterName;
            }
                        
            return true;
        }

        private bool MakeOclCommandFile(ref Oracle_Procedure OPstruct, ref OracleCommand Oclcmd, ref string strReason, ref bool[] bInOutFlag)
        {
            bInOutFlag = new bool[OPstruct.strIn.Length + OPstruct.strOut.Length]; //false:IN  true:OUT

            int j = 0;

            try
            {
                switch (OPstruct.iProcedureIndex)
                {
                    //FA
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_CHECK:
                        Oclcmd.Parameters.Add("SET_SN",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("INSP_STEP_DESC",    OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_STEP_COMPLETE:
                        Oclcmd.Parameters.Add("SET_SN",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("LINE",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_RESULT",       OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;

                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_MASTER_INSERT:
                        Oclcmd.Parameters.Add("LINE",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("SNNO",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("RESULT",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TIME",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TESTCOUNT",          OracleDbType.Decimal);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PC_ID",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TXNID",              OracleDbType.Decimal);       bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_DETAIL_INSERT:
                        Oclcmd.Parameters.Add("TXNID",              OracleDbType.Decimal);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMID",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMRESULT",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMVALUE",          OracleDbType.Double);        bInOutFlag[j++] = false;
                        break; 
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_MODEL_INFO:
                        Oclcmd.Parameters.Add("DIVISION",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("WIPID",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MODEL",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;                    
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN:
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_TCP:
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_PSA:
                        Oclcmd.Parameters.Add("WIP_ID",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PCID",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MODEL",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("SUFFIX",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true; 
                        Oclcmd.Parameters.Add("CIS_IP1",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP2",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP1_KEY_TYPE",   OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP2_KEY_TYPE",   OracleDbType.Varchar2, 255); bInOutFlag[j++] = true; 
                        Oclcmd.Parameters.Add("CIS_GEN_TYPE",       OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("IMEI",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("STID",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("AUTHCODE",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = true; 
                        Oclcmd.Parameters.Add("SERIAL_NO",          OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("TRACECODE",          OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("IMEI_FLAG",          OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("PART_NUMBER",        OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("VPPS",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("DUNS",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        if (OPstruct.iProcedureIndex.Equals((int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_GET_MAIN_PSA))
                        {
                            Oclcmd.Parameters.Add("PRODUCT_DATE",   OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                            Oclcmd.Parameters.Add("PSA_PARTNUMBER", OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        }
                        Oclcmd.Parameters.Add("ERRMSG",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG_DTL",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_GM_KEYWRITE_PKG_Escape_GM_SET_MAIN:
                        Oclcmd.Parameters.Add("WIPID",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("CHECKSUM",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("VEHICLE_CERT",       OracleDbType.Varchar2, 2048); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("VEHICLE_PRIVATE_KEY",OracleDbType.Varchar2, 2048); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("AUTHCODE",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("HASH",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ICCID",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMSI",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MSISDN",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PCID",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG_DTL",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;

                    //OOB
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO:
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO_PSA:
                        Oclcmd.Parameters.Add("DIVISION",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMEI",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;                  
                        Oclcmd.Parameters.Add("IMSI",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("STID",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("AUTHCODE",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("TRACE",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("MODEL",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("OOB_TEST_YN",        OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("OOB_TEST_DATE",      OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("HASH",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ICCID",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        if (OPstruct.iProcedureIndex.Equals((int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_GSM_INFO_PSA))
                        {
                            Oclcmd.Parameters.Add("PRODUCT_DATE",   OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                            Oclcmd.Parameters.Add("PSA_PARTNUMBER", OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        }
                        Oclcmd.Parameters.Add("ERRMSG",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;

                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_SET_GSM_INFO:
                        Oclcmd.Parameters.Add("DIVISION",           OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMEI",               OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("OOB_TEST_YN",        OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;        
                        break;
                    
                    //PCB
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_CHECK:
                        Oclcmd.Parameters.Add("SET_SN",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("INSP_STEP_DESC",    OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_STEP_COMPLETE:
                        Oclcmd.Parameters.Add("SET_SN",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP",         OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("LINE",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_RESULT",       OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_MASTER_INSERT:
                        Oclcmd.Parameters.Add("LINE",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("SNNO",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("RESULT",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TIME",              OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TESTCOUNT",         OracleDbType.Decimal);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PC_ID",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TXNID",             OracleDbType.Decimal);       bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_DETAIL_INSERT:
                        Oclcmd.Parameters.Add("TXNID",             OracleDbType.Decimal);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMID",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMRESULT",        OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMVALUE",         OracleDbType.Double);        bInOutFlag[j++] = false;
                        break;
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_MODEL_INFO:
                        Oclcmd.Parameters.Add("DIVISION",          OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("WIPID",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MODEL",             OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;                    
                    //TIME
                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_GEN10_Escape_GET_SERVER_TIME_SYNC:
                        Oclcmd.Parameters.Add("SERVER_TIME",       OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;

                    case (int)ProcedureIndex.ORAKSAVMES_Escape_MES_SPC_PKG_PCB_GEN10_Escape_GET_SERVER_TIME_SYNC:
                        Oclcmd.Parameters.Add("SERVER_TIME",       OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG",            OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;


                    //MOOHAN SERVER 용
                    #if USE_MOOHAN_SERVER
                    case (int)ProcedureIndex.GET_SERVER_TIME_SYNC:
                        Oclcmd.Parameters.Add("SERVER_TIME", OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255); bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_STEP_CHECK:
                        Oclcmd.Parameters.Add("SET_SN", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("INSP_STEP_DESC", OracleDbType.Varchar2, 255);    bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_STEP_COMPLETE:
                        Oclcmd.Parameters.Add("SET_SN", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("LINE", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_RESULT", OracleDbType.Varchar2, 255);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_MASTER_INSERT:
                        Oclcmd.Parameters.Add("LINE", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("SNNO", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("RESULT", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TIME", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TESTCOUNT", OracleDbType.Decimal);               bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PC_ID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TXNID", OracleDbType.Decimal);                   bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_DETAIL_INSERT:
                        Oclcmd.Parameters.Add("TXNID", OracleDbType.Decimal);                   bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMID", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMRESULT", OracleDbType.Varchar2, 255);        bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMVALUE", OracleDbType.Double);                bInOutFlag[j++] = false;
                        break;
                    case (int)ProcedureIndex.GEN10_GET_MODEL_INFO:
                        Oclcmd.Parameters.Add("DIVISION", OracleDbType.Varchar2, 255);          bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("WIPID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MODEL", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_GM_GET_MAIN:
                        Oclcmd.Parameters.Add("WIP_ID", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PCID", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MODEL", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("SUFFIX", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP1", OracleDbType.Varchar2, 255);           bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP2", OracleDbType.Varchar2, 255);           bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP1_KEY_TYPE", OracleDbType.Varchar2, 255);  bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_IP2_KEY_TYPE", OracleDbType.Varchar2, 255);  bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("CIS_GEN_TYPE", OracleDbType.Varchar2, 255);      bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("IMEI", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("STID", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("AUTHCODE", OracleDbType.Varchar2, 255);          bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("SERIAL_NO", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("TRACECODE", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("IMEI_FLAG", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("PART_NUMBER", OracleDbType.Varchar2, 255);       bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("VPPS", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("DUNS", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG_DTL", OracleDbType.Varchar2, 255);        bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_GM_SET_MAIN:
                        Oclcmd.Parameters.Add("WIPID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("CHECKSUM", OracleDbType.Varchar2, 255);          bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("VEHICLE_CERT", OracleDbType.Varchar2, 255);      bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("VEHICLE_PRIVATE_KEY", OracleDbType.Varchar2, 255); bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("AUTH_CODE", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("HASH", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ICCID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMSI", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MSISDN", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PCID", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG_DTL", OracleDbType.Varchar2, 255);        bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_GET_GSM_INFO:
                        Oclcmd.Parameters.Add("DIVISION", OracleDbType.Varchar2, 255);          bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMEI", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMSI", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("STID", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("AUTHCODE", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("TRACE", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("MODEL", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("OOB_TEST_YN", OracleDbType.Varchar2, 255);       bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("OOB_TEST_DATE", OracleDbType.Varchar2, 255);     bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("HASH", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ICCID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_SET_GSM_INFO:
                        Oclcmd.Parameters.Add("DIVISION", OracleDbType.Varchar2, 255);          bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("IMEI", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("OOB_TEST_YN", OracleDbType.Varchar2, 255);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_PCB_STEP_CHECK:
                        Oclcmd.Parameters.Add("SET_SN", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("INSP_STEP_DESC", OracleDbType.Varchar2, 255);    bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_PCB_STEP_COMPLETE:
                        Oclcmd.Parameters.Add("SET_SN", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_STEP", OracleDbType.Varchar2, 255);         bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("LINE", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP_RESULT", OracleDbType.Varchar2, 255);       bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_PCB_MASTER_INSERT:
                        Oclcmd.Parameters.Add("LINE", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("INSP", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("SNNO", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("RESULT", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TIME", OracleDbType.Varchar2, 255);              bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TESTCOUNT", OracleDbType.Decimal);               bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("PC_ID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("TXNID", OracleDbType.Decimal);                   bInOutFlag[j++] = true;
                        break;
                    case (int)ProcedureIndex.GEN10_PCB_DETAIL_INSERT:
                        Oclcmd.Parameters.Add("TXNID", OracleDbType.Decimal);                   bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMID", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMRESULT", OracleDbType.Varchar2, 255);        bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("ITEMVALUE", OracleDbType.Double);                bInOutFlag[j++] = false;
                        break;
                    case (int)ProcedureIndex.GEN10_PCB_GET_MODEL_INFO:
                        Oclcmd.Parameters.Add("DIVISION", OracleDbType.Varchar2, 255);          bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("WIPID", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = false;
                        Oclcmd.Parameters.Add("MODEL", OracleDbType.Varchar2, 255);             bInOutFlag[j++] = true;
                        Oclcmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 255);            bInOutFlag[j++] = true;
                        break;
#endif
                    default:
                        strReason = "NOT FOUND PROCEDURE";                        
                        return false;
                }

            }
            catch
            {
                strReason = "InOutFlag Count Error.";                
                return false;
            }
            
            return true;
        }
        
        //ExcuteProcedure 는 절차서에서 사용할수 있도록 좀더 편하게 한번더 랩핑.
        public bool ExcuteProcedure(ref Oracle_Procedure OPstruct, ref string strReason, string[] strOpParam)
        {
                        
            int iInputCount         = 0;
            string strProcedureName = String.Empty;
            string strInputInfo     = String.Empty;
            string strOutputInfo    = String.Empty;
            int iProcedureIdx       = 0;
            bool bCheckInputCount   = false;
                            
            for (int j = 0; j < 3; j++)
            {
                iInputCount = 0;
                strProcedureName = String.Empty;
                strInputInfo = String.Empty;
                strOutputInfo = String.Empty;
                iProcedureIdx = OPstruct.iProcedureIndex;
                bCheckInputCount = GetProcedureInputCount(iProcedureIdx, ref strProcedureName, ref iInputCount, ref strInputInfo, ref strOutputInfo);

                if (bCheckInputCount)
                {
                    if (iInputCount > 0)
                    {
                        if (iInputCount != strOpParam.Length)
                        {
                            strReason = "Parameter Error. Require Count : " + iInputCount.ToString();
                            return false;
                            
                        }

                    }
                }
                else
                {
                    strReason = "Check Parameters.";
                    return false;
                }

                string strexReason = String.Empty;

                bool bProc = MakeOracleProcedureStructure(ref OPstruct, ref strexReason);
                if (bProc)
                {
                    for (int i = 0; i < OPstruct.strIn.Length; i++)
                    {
                        if (String.IsNullOrEmpty(strOpParam[i]))
                        {
                            strReason = "NO PARAMETER.";
                            continue;
                        }
                        OPstruct.strIn[i] = strOpParam[i];
                    }

                    bProc = CallProcedure(ref OPstruct, ref strexReason);

                    if (bProc)
                    {
                        strReason = "SUCCESS";
                        return true;
                    }
                    else
                    {
                        strReason = "CallProcedure Fail : " + strexReason;
                        continue;

                    }
                }
                else
                {
                    strReason = "MakeOracleProcedureStructure Fail : " + strexReason;
                    continue;
                }
                
            }

            return false;
        }

        public bool GetFieldValue(Oracle_Procedure opData, string strFieldName, ref string strValue)        
        {
            if(opData.strFieldname.Length < 1 || opData.strFieldValue.Length < 1)
            {
                return false;
            }

            for(int i = 0; i < opData.strFieldname.Length; i++)
            {
                if(opData.strFieldname[i].Equals(strFieldName))
                {
                    if (opData.strFieldValue[i] != null && opData.strFieldValue[i].Length > 0)
                    {
                        strValue = opData.strFieldValue[i].Trim();
                        return true;
                    }
                }
            }

            return false;
        }

    }

    class DK_CHANGETIME
    {
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);
        
        public DK_CHANGETIME()
        {
            
        }

        public bool SetSystemDateTime(DateTime dtNew)
        {                        
            bool bRtv = false;

            try
            {
                SYSTEMTIME st;
                st.wYear = (ushort)dtNew.Year;
                st.wMonth = (ushort)dtNew.Month;
                st.wDayOfWeek = (ushort)dtNew.DayOfWeek;    // Set명령일 경우 이 값은 무시된다.
                st.wDay = (ushort)dtNew.Day;
                st.wHour = (ushort)dtNew.Hour;
                st.wMinute = (ushort)dtNew.Minute;
                st.wSecond = (ushort)dtNew.Second;
                st.wMilliseconds = (ushort)dtNew.Millisecond;
                bRtv = SetLocalTime(ref st);   // UTC+0 시간을 설정한다.               
            }
            catch
            {
                return false;
            }
            return bRtv;
             
        }
    }
}

