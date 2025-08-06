using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 어셈블리의 일반 정보는 다음 특성 집합을 통해 제어됩니다.
// 어셈블리와 관련된 정보를 수정하려면
// 이 특성 값을 변경하십시오.
[assembly: AssemblyTitle("GM_TELEMATICS")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("MOOHANTECH")]
[assembly: AssemblyProduct("GM_TELEMATICS")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible을 false로 설정하면 이 어셈블리의 형식이 COM 구성 요소에 
// 표시되지 않습니다. COM에서 이 어셈블리의 형식에 액세스하려면 
// 해당 형식에 대해 ComVisible 특성을 true로 설정하십시오.
[assembly: ComVisible(false)]

// 이 프로젝트가 COM에 노출되는 경우 다음 GUID는 typelib의 ID를 나타냅니다.
[assembly: Guid("c8b9d7b3-6449-4cdd-9297-b37662ab6fc9")]

[assembly: AssemblyVersion("2025.7.7.1")]     // 버젼포멧 ( 년도.월.일.빌드번호 )
[assembly: AssemblyFileVersion("2025.7.7.1")] // 버젼포멧 ( 년도.월.일.빌드번호 )


// 프로그램 변경 이력 
//  2025.7.7.1      :   1. [LGEVH] 암호화 수정 완료.
//  2025.6.26.1     :   1. [CS] ATCO 임시 조치. DB 사이즈가 20000 인데 20000이 넘어서 18000 자만 보내도록 수정.
//  2025.6.25.1     :   1. [CS] STEP_CHECK 수정 Stringbuilder 의 사이즈를 지정해야 함 (1024), 안 하면 프로그램 크래쉬 남
//  2025.6.20.1     :   1. [CS] GET_GEN12_CERT_MANUAL_ASCII 명령어 로직 수정. #EXPR_G_STID,\root.crt 처리 되도록. 
//  2025.6.19.1     :   1. [CS] GEN12.TBL : CS 의 TBL 로 변경 (CS가 더 추가된 형태라서 문제 없고 그 파일에 READ_DATA_ROOT, READ_DATA_CALLCENT, READ_DATA_ROOT_HEX, READ_DATA_CALLCENT_HEX 추가함
//                      2. [CS] GEN12 CERT 용 [PAGE] 명령어들 추가. GET_GEN12_CERT_MANUAL_ASCII, GET_GEN12_CERT_MANUAL_HEX, DATA_PARSING_ASCII2HEX
//  2025.1.22.1     :   1. [CS VU] 창고 코드 추가
//
//  2024.10.21.1    :   1. GOTO, JUMP 에서 JUMP 가 우선순위 되도록 수정
//  2024.10.8.1     :   1. MELSEC 별도 LOG 추가. C:\GMTELEMATICS\LOG\MELSEC\2024-10
//  2024.7.12.1     :   1. 버그 수정 CheckMinValue, CheckMaxValue, GMES_GetInsp
//  2024.6.27.4     :   1. AMS_KEY_DOWNLOAD_GEN12 에서 root, identity, passowrd 추가 
//  2024.6.27.1     :   1. AMS_KEY_DOWNLOAD_GEN12 에서 strFactory 삭제 처리. 별도의 함수 AmsExeKeyDownLoad_GEN12 를 사용
//  2024.6.19.1     :   1. GEN12_GB_BOOT_500 : CAN MSG 및 ID 변경 (배번근 책임)
//  2024.5.21.1     :   1. MES_OFF 시 STEP_CHECK 에서 MES_OFF 시 주석 처리를 해제. 
//  2024.5.2.1      :   1. AMS_GEN12 추가. 
//  2024.2.28.2     :   1. JUMP 로직 수정
//                       - JUMP_OK, JUMP_NG 에서 반대의 경우도 추가. JUMP_OK 에서 NG 되어 CONTINUE 조건, JUMP_NG 에서 OK 판정되어 CONTINUE 조건
//  2024.2.28.1     :   1. JUMP 로직 수정
//                       - 절차에 SKIP 3개 이상시 최종 판정이 체크로 나오는 현상 수정
//  2024.2.26.1     :   1. JUMP 로직 변경
//                          A. [PAGE]JUMP_CHECK 명령어에서 JUMP 하기 위한 별도의 명령어 생성.
//                            - 선행 명령어에서 PASS or FAIL 체크 - MEAS 값 저장 (c.f. #SAVE:MEAS1)
//                            - JUMP_CHECK 명령어 : 선행 명령어의 MEAS 값(#EXPR_MEAS1) 과 판정하여 JUMP or CONTINUE 분기 처리. 판정은 무조건 PASS 처리.
//                          B. JUMP 구간의 SKIP 된 명령어는 SKIP 으로 처리. 
//  2024.2.22.1     :   1. OK, NG 시 JUMP 로직 구현
//
/////
//  2023.11.21.1    :   1. [CS] 김성규 책임에 의해 컴파일만 다시.
//  2023.11.3.1     :   1. [OOB] 이병권 책임. UNLOCK_GEN12_GB 명령어 추가. GEN11, GEN12 모두 검사 반드시 해야 함. BOOTID는 147 로 변경.
//  2023.10.24.1    :   1. [CSMES] item_WIPID 무조건 대문자 변환
//  2023.10.5.1     :   1. [CSMES] SetTestMode(); 주석 처리하고 재 송부
//  2023.10.4.1     :   1. [CSMES] STEP_CHECK 를 하지 않고 STEP_COMPLETE 만 할 경우 [PAGE]SAVE_WIPID 명령어를 통해서 Item_WIPID 변수에 저장
//  2023.8.29.1     :   1. [CSMES] COMPLETE, COMPLETE_DETAIL 예외처리
//  2023.8.21.2     :   1. [CSMES]STEP_CHECK 에서 NG 시 표기 수정
//  2023.8.17.1     :   1. [LGEVH] PASSWORD POPUP, SU PASSWORD 암호화 처리
//  2023.8.8.1      :   1. CSMES 완료
//  2023.7.19.1     :   1. PASSWORD CLICK 기능 원복
//  2023.7.17.1     :   1. LGEVH 1차 적용.
//  2023.6.19.1     :   1. MTP120A CONFIG 설정값 초기 1회 고정 설정 추가
//  2023.6.5.1      :   1. MTP120A 모듈 추가.
//  2023.5.22.1     : GEN12.TBL 파일 업데이트(이동성 책임)
//  2022.10.14.1    : InspectionLoggingProcess(...) 에서 STID_ 를 SN_ 으로 수정, SemiResultLogging(...) 에서 STID 를 SN 으로 수정
// Ver 2022.10.12.1  :  아래 내용 처리 중 주석 처리 안한 부분 재 수정
// Ver 2022.9.18.1  :   ATCO GEN11 의 ForAtcoResultLogging 에서 STID, IMEI, TRACE는 NONE 으로 SN 추가
// Ver 2022.6.30.1  :   VERSION 들 HEAD --> NAD 로 변경. 
// Ver 2022.6.23.1  :   ALDL12_READ/WRITE 에서 기존 고정 길이(9) 에서 가변 길이로 변경
// Ver 2022.6.22.1  :   OOB용 LABEL LOADING_LABEL_FOR_GEN12 추가 
// Ver 2022.5.24.1  :   인증서 업로드 Test Code 삭제 배포
// Ver 2022.5.19.1  : 인증서(SECURE_STORAGE 관련 부분), ALDL12~, GB KEY(MASTER, UNLOCK 등) 로직 추가
// Ver 2022.5.2.1   : GEN2용 ALDL 수정 (GEN11과 동일 로직)
// Ver 2022.4.25.1  :   PCAN 명령어(GEN12_GB_BOOT_500) 추가 : 145, 284를 500ms 마다 번갈아 전송. 즉. 각 명령어는 1000ms 마다 전송
// Ver 2022.4.15.1  :   1. USIM VER 명령어들 : 제일 뒤 BODY 뒤에 공백이 있으면 이 공백도 체크하여 HEAD (DEFAULT 값) 로 처리되어 NG 발생. (INDEX MISMATCH). -> 공백 제거하여 비교하도록 수정
//                      2. READ_IMEI_CHECKSUM : IMEICHECKSUM 의 ENUM 타입이 GEN11 로 되어 있어서 GEN12 로 변경
////////////////////////////////
// Ver 2016.5.12.0 : 프로그램 버젼 포멧 변경
// Ver 2016.5.20.0 : LTE DLL 동적 로딩 요구로 인하여 DK_NADKEYWRITE.CS 제거. DK_NADKEYDLL.CS로 변경. "DLL_FILE_NAME" 명령 추가.
// Ver 2016.5.25.0 : GM_PART_NO, BASE_MODEL_PART_NO값 GMES 올릴 경우에는 다시 데시멀로 바꾸어 올리는것 추가. 하드코딩.
// Ver 2016.5.30.0 : CMW500 속도 개선, FREQUENCY.INI(LTE-BAND3 변경), *OPC? 명령 TIMEOUT 은 무시처리.
// Ver 2016.5.31.0 : CMW500 INITILIZE, PRESET 파일에 RST 추가. 
// Ver 2016.5.31.1 : CMW500 INITILIZE 파일에서  SYST:ERR? 명령후
//                   -114,"Header suffix out of range;SOUR:GPRF:GEN:STAT OFF" 라고 나오는것은 장비에 2채널로 설정이 안되어있을때 나오는거다!
//                   그러므로 프로그램 문제는 없다.
// Ver 1000.0.00.1 : DQA용으로 임시로 1copy만 빌드. UseDeveloperMode 함수 주석처리 (조현우 주임 요청)
// Ver 2016.6.09.0 : zebra 스캐너 비아스키 제거 코드 추가 (DeleteNonAscii)
//                  비공식 업데이트 WCDMA_OB4_1675 (FREQUENCY.INI 파일) 업데이트 - 16년 6월 7일.
//                  비공식 업데이트 DK_ANALYZER_ATT.cs, ATT_TCP.TBL    업데이트 - 16년 6월 9일.                  
//     2016.6.24.0 : DELAY 방법 변경
//     2016.7. 2.0 : MAPPING EIDT 화면 (Ctrl-C,V,X) 기능 추가. 
//     2016.7. 4.0 : EXPR 값에 문자열 합치는 기능 추가.
//                 : (IMEI가 1234 일때] #EXPR_IMEI[*K] 로 호출되면 리턴값은 K1234 로.#EXPR_IMEI[K*] 로 호출되면 리턴값은 1234K 로.  
//     2016.7.12.0 : DK_NI_GPIB 클래스 -> SendRecv 함수에 for문으로 돌리던 부분을 타임아웃 파라미터 추가하여 로직변경. 
//     2016.7.19.0 : EDIT 에서 EXCEL CSV 로 저장기능 추가.

//     2016.7.24.0 : GMES DLL 버그 보완코드 추가 (OFF 해도 아이템 데이터 안사라지는 것때문에)
//                   ITEM DATA VIEW 로깅시 해당 내용 표시,
//                   인터렉티브모드에서 GMES 동작 완료함. 단, 스텝컴플리트는 인터랙티브에서 불가능하게함.
//     2016.7.25.0 : GEN10 PSA기준(키라이팅 공정 완료.....후아.ㅋㅋ)
//     2016.7.28.0 : GMES DLL 업데이트 1.0.6 STLPORT dll 도 포함. CSV 로깅 수정 (EXPR 데이터 표시)
//     2016.7.29.0 : COMPORT ERROR 여도 인터렉티브는 사용할 수 있도록 변경.
//                   LABEL GOTO 구간 RETRY-END 구간으로 변경.
//                   Config 옵션에 Step Complete On Stop 추가(LABLE GOTO RETRY FAIL 시 STOP 되면 STEP COMPLETE NG 올리는 액션 반영)
//     2016.8.2.0  : DIO COM  Queue 방식으로 변경.
//     2016.8.3.0  : 화면에 년월일 표시 추가
//     2016.8.4.0  : 딜레이 명령시 타임마킹 추가. 인코딩 ASCII 에서 UTF8 로 모두 변경. 
//                   시리얼 SCANNING 방식 한바이트 단위에서 배열단위로 변경
//     2016.8.6.0  : BIN 로깅 추가 완료 (클릭copy 가능, bin 폴더에 저장됨(OK가 아닌경우에만)
//     2016.8.7.0  : log 기록시 stringbuilder 타입으로 변경.
//     2016.8.9.0  : GetPrivateProfileString 호출시 StringBuilder 사이즈 설정 (미설정시 Exception.)
//                 : DKStepManager.ReleaseStepCheck(); 스텝체크 이력 삭제 추가 (인터렉티브 해제시)
//                 : sInputBuffer 삭제
//     2016.8.16.0 : Oracle & GMES 혼용 작업시작 (Config 셋팅 및 메인화면 Swiching 완료)
//     2016.8.17.0 : Oracle & GMES 혼용 작업시작 완료, 
//                 : StepManager 내 로깅 방법 간소화 작업 완료. MessageLogging()
//     2016.8.18.0 : 바코드 입력시 스페이스 입력방지코드 추가 및 최종단계에서도 Trim 처리.
// Ver 2016.8.26.0 : CheckExprParam  함수 변경. (콤마로 멀티 파라미터 가능함 - 예를들면 NAME SUFFIX 동시 사용 가능)
//     2016.9.1.0  : 로그파일 csv 저장시 , min max 안나오는것 오류 수정. SKIP 명령의 경우 모두 공란 처리.
//     2016.9.1.0  : MAPPING EDIT 에서 MODEL, SUFFIX 오토로딩 추가.
//     2016.9.5.0  : CONFIG 에서 COMPORT 자동 리스트업 기능 추가.
//                 : DIO AUDIO SELECTOR 추가.
//     2016.9.8.0  : 리트라이 감소 로직 수정 RETCOUNT_DIC[iCount]--;
// Ver 2016.9.9.0  : CAN MSG 로깅 포멧 변경 (MSG ID 추가)
//     2016.9.20.0 : ORACLE (곤산) 패키지 작업.1.
//     2016.9.21.0 : 스타트시 setstatus - running(Testing...)  mfcsleep(1)표시되도록 수정. 
//     2016.9.21.0 : 무인화의 경우 GMES ON일 경우 CONNECTION이 안되었을경우 최대 2초간 대기에서 5초로 대기로 변경.
//     2016.9.22.0 : ORACLE, STEPMANAGER 클래스에 #USE_MOOHAN_SERVER 추가. 컴파일구간 변경위해 (오라클 테스트시)
//     2016.9.22.0 : ORACLE (곤산) 패키지 작업.2. 평택에서 중간 테스트 완료.
// Ver 2016.9.23.0 : 곤산 셋업 배포, 베트남 배포
//     2016.9.23.0 : stdole.dll (엑셀) 관련 로컬복사후 배포해야함...ㅡ.ㅡ
//     2016.9.23.0 : 삭제판 배포(오디트용) "NONASCII" 검색 삭제. ( 메인화면 버젼을 "ReleasE - "라고 해뒀음. 삭제)
//     2016.9.23.0 : CMW config call/tx/rx LTE13, 17, 누락되서 추가함. ㅡㅡㅜㅜ
//     2016.9.23.0 : param 콤마 오류..CheckExprParam() 함수 전면 재수정.
//     2016.9.28.0 : auto GOTO (RETRY~END) 구간 설정시 EMPTY, STOP, MES 는 제외하고 하도록 변경수정.
//     2016.9.30.0 : PASSWORD MANAGER 기능 추가 (moohan123! SUPER USER)
//     2016.10.1.0 : MHT ADC MODULE 컨트롤러 제어 추가 (보드 나오면 테스트 해야함)
//     2016.10.1.0 : LTE_CHINA_NEW_20.DLL 추가 (version, readSCNV )
//     2016.10.3.0 : GM_GET_MAIN_TCP 패키지 추가
// Ver 2016.10.4.0 : NAD DLL - EFS BACKUP 명령 추가. (베트남배포)
//     2016.10.5.0 : LTE INITIALIZE 시 CMW500 EXCUTE FAIL 때문에 WCDMA_BER_OFF.CMW 파일 추가.
//     2016.10.10.0: 로깅박스 탭으로 변경
// Ver 2016.10.10.0: EFS BACKUP 명령 try catch 추가.
//     2016.10.11.0: "NONASCII" 타입의 경우 CONTAIN 타입으로 강제변경후 아스키문자만 거른다. 0xFF가 끼어들어오는 ESIM_PROFILE 같은 명령땜시.
//     2016.10.11.0: SubExcuteCMW500 에서 Exception(TIME LIMIT 이나 recieve fail 같은 error 나면 5회 반복후 그냥 진행)
//                   CMW file 수정 SOUR:GPRF:GEN:STAT ON;*OPC? -> SOUR:GPRF:GEN:STAT ON 로 변경 (로데 임재봉 차장님 추천가이드)
//                   Config 컴포트 설정 그리드 오른쪽버튼 버그 수정.
//     2016.10.20.0: 패스워드 치트키 반영, 화상 키보드 반영
//     2016.10.21.0: ClearTestResultData() 추가. 절차서 교체후 display all 누르면 이전결과 삭제되도록
//     2016.10.26.0: 곤산법인은 MES (오라클)을 쓰는 바람에 KIS 서버를 접속해야하는 부분이 여기에 분기를 추가해야한다.
//                   곤산 ESN (키라이팅공정)은 스텝체크를 하지 말라고 해서 GET_KEYWRITE_MAIN_TCP, GET_KEYWRITE_MAIN에 한것처럼 넣어둠.
//                   InitialKeyData()함수에 GMES 와 MES 의 경우를 분기함.
// Ver 2016.10.27.0: RESULT 로그 변경 (GMES, MES 경우 나눠어서)
//                 : ATCO 배포함.
//     2016.10.27.1: RESULT 로그 저장시 데이터 필드 [ ] 로 감싸기. 앞뒤 공백붙으면 확인이 안되므로.
//                 : MES모드에서 AUTO JOB LED 표시 안되는 부분 수정 완료.
//     2016.10.28.1: MES - OOB 공정 SET & GET_OOB_INFO 호출시 파라미터 14자리로 자르는 하드코딩 추가.
//     2016.10.29.0: KIS 서버 이중화 접속 루틴 구현완료(베트남, 곤산)
//     2016.10.31.0: MES TXNID 패키지 추가 (기존의 master insert 의 txnid 는 되긴 되나 MES 버그로 null 이 올때가 있어서 사용하지 않는다)
// Ver 2016.11.1.0 : ExcuteProcedure() 함수에서 리트라이 3회 반영
//     2016.11.8.0 : CONFIG - MTP200 LOSS VALUE 기능 추가.
//     2016.11.12.0: CONFIG - MODEL  INFO VALUE (GEN10 MFG CHECK PARTNUMBER, STID) 기능 추가.
//     2016.11.14.0: GM_GET_TXN_ID 패키지 삭제 (CNS 정진섭)
//     2016.11.14.1: MTP LOSS TABLE  추가
//     2016.11.14.2: TC3000 보레이트 변경할수 있는 명령 추가 (곤산, 베트남 통일을 위하여)  추가
//     2016.11.16.0: GEN10 ANALYIZER WLCOMMAND 분석쪽 로직 변경....
//     2016.11.18.0: DateTime.Now. HH 로 수정
//     2016.11.18.2: SET_KEYWRITE_MAIN - 최종결과가 NG면 SKIP 
//     2016.11.20.0: GEN10_OQA_COMMAND (RECOGNITION_TIME, REPORT_CURRENT_SID, OOB_SELF_TEST, OOB_SELF_TEST_CHECK) 명령 추가.
//     2016.11.21.1: GEN10_OQA_COMMAND (OOB_DTC_ALL) 명령 추가. (기존의 DTC ALL 과 완전 동일하나 OOB에서는 개별DTC가 안읽히는게 있어서
//                   ALL로 읽은후 그것을 파싱해서 가져다 쓴다.
//     2016.11.21.3: GEN10 OOB - 바코드 스캔시 맨마지막 MODEL 정보는 자릿수가 11자리 이상일수도 있어서 마지막껀 다 읽는것으로 변경.
//     2016.11.21.4: GET_KEYWRITE_MAIN_PSA 패키지 추가.
//     2016.11.27.0: PCAN RECV 스레드 방식으로 변경
// Ver 2016.11.29.0: SEMI LOG FUNCTION UPDATE (TESTLOG 폴더에 간단한 검사로그 저장됨 1줄짜리)
//                 : TCP_ATT COMMAND (BUB 관련 추가, HEART BEAT 추가.) ATCO 배포
// Ver 2016.11.30.0: TCP, GEN10, ATT(TCP출하향) ALDL 명령어 통일화 및 특정비트 ON,OFF 및 BITS ARRAY 표현 명령 추가. (대박)
//                 : READ_NV_ITEM 명령추가 
//     2016.12.1.0 : UI 사이즈 미세 조정. Counter Cable Message bug 수정. 
//     2016.12.2.0 : BINLOG, ETCLOG 스레드 try catch 추가. AutoGotoQuery() 버그 수정.
//                 : 곤산 ORACLE OOB PSA INFO 패키지 추가.
// Ver 2016.12.5.0 : GOTO(LABEL) 시 A1~19 (증설) C1~19 (추가) C계열은 GOTO 문 끝나고 STOP 이 아닌 CONTINUE 옵션
//     2016.12.7.0 : GEN10, ATT(TCP) 아날라이져 배열index error 버그 수정.
//     2016.12.8.0 : PCAN 오토리셋 방식으로 변경, PCAN 필터메시지 추가,  판넬미터 UI 에 기능 추가.
//     2016.12.9.0 : ixlblStatus.Caption 인보크내부에 추가.
//                 : ATT LOGGING 레벨 명령 추가, HEART BEAT 3초간격으로 무조건 보내는것 추가.
//                 : isegTimer.Value.ToString 인보크내부에 추가.
//                 : PCANBasic.Reset(m_PcanHandle); 추가.
//     2016.12.12.0: PCAN WRITPCAN() TX 로깅 추가. UNTIL 옵션시 응답올때까지 TX 100ms 간격으로 Sending 하는 기능 추가. (Thread)
//     2016.12.13.0: ATCO 를 위한 로깅 추가 (for ATCO)
//                 : Config - Show Count 옵션 추가 (UI에서 PASS/FAIL/TOTAL 감추는 용도) 파트장님 지시
//     2016.12.14.1: BIN 로깅 방법 변경. WIPID/STID 별로 append 함. - nad port 표시
//     2016.12.14.2: CONFIG PATH 표시기능
// Ver 2016.12.15.0: 인터렉티브시 SKIP 항목은 SKIP, DIO 없이도 모든 장치사용가능. 
//     2016.12.15.1: 메인 그리드 더블클릭시 NOTEPAD 로 뷰잉 기능 추가.
//                 : Zebra Scanner 5초간격 트리거링으로 변경.
//     2016.12.16.0: Zebra Scanner 클래스 환골탈태
//                1: NAD PORT CLOSE 시 SET 쪽 로그 안남는부분 수정
// Ver 2016.12.20.0: PCAN STATUS (OFF/HEAVY/LIGHT) UI 하단에 실시간 표시
//     2016.12.21.0: CHECK PCAN STATUS 명령 추가.
// Ver 2016.12.22.0: INITIALZE_HSCAN, UNINITIALZE_HSCAN 명령추가. 이동성선임 요청.
//     2016.12.29.0: 절차서 명령에 TIMER(START, STOP, VALUE) 명령 추가. (시간재는명령용도)
// Ver 2017.1.2.0  : DMM(34410A) UPDATE"
//     2017.1.6.0  : DeleteScreen() 3개월 까지만 자동보관하고 그 이전 자료는 삭제 버그 수정 
//     2017.1.14.0 : SET 로그에 프로그램 버젼 및 JOB 파일 NAME 기록추가.
//     2017.1.18.0 : EDIT 에서 Ctrl + F 기능 추가.
//     2017.1.19.0 : 5515C 테이블 명령 수정 GSM_SET_CEL-ACTIVE-CELL           //CALL:BCH:SCEL//GSM|GPRS|EGPRS//-1
//     2017.1.21.0 : UI CURRENT - AUTO RAGNE 추가. ㅋㅋ
// Ver 2017.1.24.0 : ATCO result log 에 JOB name 삭제하고 GPS:/로 변경
//                 : EDIT GOTO(CONTINUE) 문 및 CASE NG에 MES 설정 버그 수정.
//     2017.1.24.2 : JOB FILE LOAD 방식 변경 및 체크섬 추가중..
//     2017.1.25.3 : JOB FILE LOAD 방식 변경 및 체크섬 추가완료.
//     2017.1.30.0 : JOB FILE 수정 이력 추적 기능 완료. (HISTORY 폴더에 저장)
//     2017.1.31.0 : CURRENT GRAPH 추가. (NTF 시 screen-shot 으로 추적)
//     2017.2.1.0  : 34410A TBL - CURRENT DC 옵션 MAX 에서 AUTO 로 재변경. (MAX는 sleep current 측정시 오차가 심함)
//                 : AUTO RANGE 는 자동으로 최소단위까지 변경되나 AUTO RANGE 특성상 최초 READ 1회는 값을 버려야함.
//     2017.2.4.0  : JOB FILE LOAD 시 CRC 에러(외부에서 수정되었다면) HISTORY 로그에 저장
//     2017.2.8.0  : EDIT - FILE LIST 확장자 소문자일경우  COPY TO EXCEL 시 CSV 파일 및 JOB 저장 오류 수정.
//     2017.2.21.0 : GEN10 - BUB_DISABLE, ENABLE 명령 추가됨.
//     2017.2.23.0 : 프로그램 종료시 DKStepManager.DioReset() 추가.

//     2017.2.27.0 : TCP.TBL - NV ITEM READ 명령 추가. 
//                 : COM 클래스 DIO 센서링 및 판넬미터 실시간 모니터링 코드 수정 

//     2017.2.28.0 : [HandleProcessCorruptedStateExceptions] nad key dll 에 익셉션 추가.
//     2017.3.1.0  : GM BREAKER 연동 추가.
//     2017.3.1.1  : CONFIG 에 테스트코드 삭제.
//     2017.3.2.0  : 판넬미터 통신 COMM 5회 NG시 중단으로 변경 및 시그널링 sleep time 50 -> 80으로 변경.
//                 : CycleCore 쪽 Command retry 시 System.Threading.Thread.Sleep(10); Application.DoEvents(); 
//                 : 추가. (send & sendrecv 실패시 hangup 현상 해결위해
//     2017.3.1.1  : VH라인 반영위해 버젼만 예전것으로 변경
//     2017.3.1.1  : offbreaker 각 버튼마다 추가. - 2
//                 : KillThreadObject(threadNadKeyDLL); 수정했으나 안쓰임
//     2017.3.1.1  : NAD KEY DLL 사용시 Thread방식 해제. UnloadLibrary 전 딜레이 1초 추가.
//     2017.3.1.1  : LoadMtpLoss(); 명령 EXPR 클리어 다음으로 추가. 시작할때마다 MTPLOSS 로딩해야하므로.
//     2017.3.1.1  : MTP200 EXPR 파라미터 사용시 indexof > 0 에서 indexof >= 0 로 변경.
//                 : 8192 stringbuilder 버퍼UP
//     2017.3.20.0 : (int)COMSERIAL.TC3000 open check 시 reset 명령으로 장치확인 코드 추가.
//            21.0 : EXIT 버튼 종료시 죽는문제로 인해 DKStepManager.StopStatusTimer(); 추가.
//                 : GmesDisconnection() 도 종료시 추가. EDIT, CONFIG 전부 dispose 처리.
//                 : exit 버튼 클릭시 DKStepManager.ActorStop(); 부활. closing 에서 삭제.
//                 : cbJobFiles.Refresh(); 콜백에 추가.
//                 : CheckMananger() 에  try catch 추가.
//     2017.3.22.0 : CopyCheck 에 파일복사 추가. EDIT SAVE AS 시 한줄삭제되는 버그 수정 (CRC 때문에 생긴일)
//     2017.3.23.0 : KillMyApplication() 명령 추가. (종료시 에러나면 강제로 process 를 죽이는 코드 추가....
//                   goto문 및 Stop On STEPCOMPLETE 및 inspection 전체 리트라이부분 대대적 개선.
//     2017.3.24.0 : Gmesitemcoding 오류 수정. 
//          3.25.0 : Gmesitemcoding 오류 재수정 - LstTST_RES[iPort][i].ResultData 으로 변경
//          3.26.0 : GEN10 AnalyzePacket() 에 STX 찾을때 패킷비교구문추가. STX 가 2번 들어올경우 무한정 타임아웃걸리기때문
//          3.27.0 : GEN10 AnalyzePacket() 에 GPS 1R6 쪽도 손봐야했는데 놓쳐서 재수정..... bSpecialFlag
//          3.27.1 : RESULTDT_DIC gmesitemcoding 부분 재수정 미치겠다
//                 : ATT AnalyzePacket() 도 GEN10 처럼 수정. - 추가.수정.... out of index.
//                 : GE10 AnalyzePacket() 추가.수정.... out of index.
//          3.30.0 : Gmesitemcoding - LstTST_RES[iPort].Count 로 수정... ㅡ.ㅡ헐.
//                 : DeviceControlUSB() 추가. PCAN 장치관리자에서 죽였다 살리기.
//                 : Application_ThreadException 추가. 예외처리 되지 못한 예외처리하는곳...남용말자.
//                 : Dsub Retry 추가. 구현.
//                 : 전체 리트라이 설정시 STEP COMPLETE 는 SKIP 되도록 설정
//          4.1.0  : PLC 모드인경우 (PCAN, NAD) 항목이 연속불량인경우 PLC 동작 멈추고 메시지 POP 후 대기
//          4.3.0  : Application_UnhandledException 추가
//          4.4.0  : 연속NG 중복검사 skip/  Nullreference Exception 나는 현상있어서..
//          4.5.0  : PCAN until 옵션에서 heavy/light 시 자동으로 reset / 치는 기능 추가.
//          4.6    : 엑셀 리드시 컬럼에 null 값 체크및 try catch 추가.
//          4.10.0 : CAN NOT TEST START - invoke 삭제
//          4.21.0 : USERSTOP 표현에서 STOP 표현으로 변경
//          4.24.0 : PSA BOOT 의 AUTO SENDING 도 상태체크해서 무한 RESET 치는 코드 추가.
//          4.28.0 : 검사공통 요청 STOP 일경우 READY 로 표시되도록 요청.
//          5.10.0 : 34410A TBL - RESET 추가.
//          5.17.0 : Vertor 디바이스 추가. (단일 메시지 TX/RX만 가능) 업데이트
//          5.18.0 : Vertor 디바이스 (연속 TX 가능 _CONTINUOS_) 업데이트
//                 : GEN11 TCP 프로토콜 업데이트 1차 완료.
//          5.21.0 : CCM 용 시리얼 추가. CCM 프로토콜 추가. (CCM.TBL)
//          5.22.0 : CCM 용  DIO 벤치 명령 추가.
//            22.1 : Check_QFUSIONG -> Check_QFUSING 명령 이름 오타 정정
//                 : 바코드 입력시 expr 에 저장시켜놓는 기능 추가. (#EXPR_BARCODE
//                 : CCM sleep mode 명령 추가.                  
//            22.2 : NAD PORT 저장 오류 수정.
//       2017.5.29 : GEN11 TCP 프로토콜 업데이트 2차 완료.
//       2017.6.2.0: "NETWORK_PING_TTL", "NETWORK_PING_TIME" 명령 추가 완료
//                 : GEN11 3차 프로토콜 업데이트 (명령 & DTC.TBL)
//                 : "NETWORK_PING_TTL", "NETWORK_PING_TIME" 명령 리트라이 및 딜레이 안되는것 수정 - PAGE 명령은 원래 리트라이가 없는것으로 설계되었기때문.
//       2017.6.7.0: GEN11 ALDL 구조체 사이즈변경, DTC 구조체 변경 by 강종훈 책임.  에 따른 프로그램 수정.
//                 : GEN11 READ MICOM 명령 추가. 
//       2017.6.10.0: GEN11 DTC, DTC INDEX INI 파일 수정. CheckGen11_DtcIndex 8bit, 1byte 표시 두가지로 될수 있도록 수정.
//       2017.6.12.0: GEN11 DTC MANUAL 명령 추가. 버젼 2017.6.10 - 업 안한이유는 바이어 문서때문
//       2017.6.12.0: GEN11 인증서관련 기능 업데이트, 파일전송 MULTIPLE 지원가능토록. 버젼 2017.6.10 - 업 안한이유는 바이어 문서때문
//       2017.6.12.0: GEN11 DTC LIST 변경 - 연구소, 버젼 2017.6.10 - 업 안한이유는 바이어 문서때문
//       2017.6.12.0: GEN11 ALDL WRITE 변경 - READ는 100바이트 증가시켜놨는데 WRITE는 100바이트로 증가안시켰다......ㅜㅜ , 버젼 2017.6.10 - 업 안한이유는 바이어 문서때문
//       2017.6.12.0: GEN11 DTC 29 INDEX CODE 변경 - 연구소 문서 오타 , 버젼 2017.6.10 - 업 안한이유는 바이어 문서때문
//       2017.6.14.0: GEN11 인증서 관련 1KB 이상 MULTIPLE 로 올리는 기능 추가.
//       2017.6.14.0: GEN11 IMEI_CHECKSUM 명령추가 - 이건 기존 프로토콜에서 마지막 자리수를 구하기위해 내가 별도 추가함.
//       2017.6.15.0: GEN11 CCM GPS/GNSS 측정명령추가 - 총조에선 필요없지만 MULTI 4 in 1 을 위해 작성함
//       2017.6.19.0: RESULT 로그에 LAPSE 타임 기록 추가. 이동성선임 요청
//       2017.6.22.0: WRITE VCP CONFIGURE 명령 추가 - 이동성 선임 요청. (프로토콜 문서상엔 사용되지않는다고되어있음...)
//                  : EDIT 에서 아이템 중복검사 기능 추가!. main sleep 부분 왠만한부분은 삭제함..
//                .1: TCP.TBL - NV ITEM / char 에서 byte 로 변경.
//       2017.6.26.0: CCM - 아날라이져 7D 5E 낄경우 사이즈가 하나줄어서 비교시 문제발생. 수정.
//       2017.6.26.1: CCM - HMC 용 AUDIO LOOK BACK 명령 추가.
//       2017.7. 3.0: DIO_VCP : READ_BCMRADIO_VOLTAGE_CHECK 명령추가 (RADIO BOX 구분용)
//       2017.7.11.0: OLD GPS INFO 구조체 변경. GPS S-COUNT/CN0 추가. GEN10-TCP 출하COMMAND 추가 (ATT.TBL)
//       2017.7.12.0: 시리얼옵션 NORESPONSE 추가 - 응답이 없어야 PASS . 즉 TIMEOUT 이면 성공/ 응답이 있으면 NG
//       2017.7.13.0: CCM - RESET 명령 추가.
//       2017.7.14.0: OLD GPS INFO 구조체 변경2. int 형은 c#에서 short 로 대체.
//       2017.7.27.0: RESULT 로그에 LAPSE 타임 오류 수정, Format 에 대괄호 삭제,

//       2017.7.28.0: SET 쪽 COMPORT change 기능 추가. 시작할때는 기본 디폴트 (config 에 있는 set port 로 변경함). 
//                  : JMB 같은 타입을 위해서 변경함. 921600 bps
//       2017.8.10.0: ODA POWER 장비 제어 기능 추가 - For GEN11 BUB CAL 공정.
//                  : GEN11 출하향 명령 추가 완료 
//                  : GEN11 BUB CHARGE/CALIBRATION 관련 명령 추가 완료.
//                  : STEP_COMPLETE ITEMCODING ; 콜론 삭제.
//     2017.8.22.0 : CHANGE_JOB 딜리게이트 속도문제로 잡파일네임이 안넘어가는현상이 있음. 네임 컨트롤로 강제로 가져오도록 수정.
//     2017.9.11.0 : GEN11_HSCAN_TEST CAN 명령 추가.
//     2017.9.12.0 : GEN11, GEN11p, MCTM에 bResultCodeOption 옵션추가. 프로토콜의 ResultCode 로 판정
//     2017.9.15.0 : ODA TBL 수정 및 COMMAND 형식 변경 (?가 있으면 SENDRECV, 없으면 SEND 로 자동처리)
//     2017.9.21.0 : GEN11 BUB ADC CALIBRATION(READ,CHECK) 명령 수정(DATA TYPE)
//     2017.9.22.0 : GEN11 명령 추가 삭제 (CHECK_BUB_ADC_CALIBRATION 삭제, READ_BUB_ADC_CALIB_RESULT_PACK 추가, READ_BUB_ADC_CALIB_RESULT_CELL 추가.
//     2017.9.25.0 : GEN11 명령 추가 및 네임 변경 (Read BUB OCV NP (Cell) 추가, READ_BUB_ADC_CALIB_RESULT_PACK 추가, READ_BUB_ADC_CALIB_NP RESULT_CELL 추가.
//     2017.9.27.0 : MCTM ALDL R/W 명령추가 (1차 바이트단위만) 아직 비트마스킹은 없음.
//                   gmes dll - 김태완과장님껄로 변경 (1.0.7)
//     2017.9.28.0 : MCTM 명령어 업데이트 및 변경
//     2017.9.28.1 : MCTM ALDL R/W 명령추가 비트마스킹(TOGGLE ON/OFF) 기능 추가. PAR1 -> ALDL ADDRESS, 변경할 BYTE번호(1번부터시작), 변경할 BIT번호(1번부터 시작 8번까지)
//     2017.9.30.  : MCTM ALDL R/W 명령추가 비트마스킹(TOGGLE ON/OFF) 기능 추가. PAR1 -> ALDL ADDRESS, 변경할 BYTE번호(1번부터시작), 변경할 BIT번호(1번부터 시작 8번까지)
//     2017.10.1.0 : MCTM ALDL R/W 명령추가 비트마스킹(TOGGLE ON/OFF) 기능 추가. PAR1 -> ALDL ADDRESS, 변경할 BYTE번호(1번부터시작), 변경할 BIT번호(1번부터 시작 8번까지)

//     2017.10.6.0 : DIO VCP BENCH (EXTERNAL 시 PRIMARY, KEY, BACKUP 릴레이 ON.OFF 명령추가) - BOOT DELAY 추가.
//                 : GEN11 START_BUB_RI 명령 추가 (양희권 책임 요청)
//     2017.10.7.0 : MCTM.TBL FACTORY INIT 명령 수정.

//     2017.10.9.0 : MCTM GPRMC 개발중
//     2017.10.9.1 : GEN11 - DIO GOLDEN_NODE_ENABLE/DISABLE 명령 추가.
//     2017.10.9.2 : GEN11 무인화 라인사용시 D-SUB 리트라이를 위하여 EXTERNAL 파워를 OFF했다가 디서브가 다시 들어오면 ON 하는 로직 추가.
//     2017.10.9.3 : DKGMES.GMES_StepComplete 전에 비아스키 다시 한번더 검출코드 추가.
//10.10
// 아래 명령 연구소 문서 오타로 인해 변경 38 - 42 로 변경
//SET_BUB_CHARGE_ON                 //02 FA <LENGTH> 46 43 35 42 66 00 01 00 01 <CRC16> FA////
//SET_BUB_CHARGE_OFF                //02 FA <LENGTH> 46 43 35 42 66 00 01 00 00 <CRC16> FA////

//     2017.10.13.0 : 양희권 책임 아래 명령 다시 변경요청함...............
// 아래 명령 연구소 문서 오타로 인해 변경 42 - 45 로 변경
//SET_BUB_CHARGE_ON                 //02 FA <LENGTH> 46 43 35 42 66 00 01 00 01 <CRC16> FA////
//SET_BUB_CHARGE_OFF                //02 FA <LENGTH> 46 43 35 42 66 00 01 00 00 <CRC16> FA////
//     2017.10.13.0 : GEN11 ALDL WRITE/방식 변경 make, check 
//                  : "EXPR_DATA_VIEW" 기능 추가.
//     2017.10.14.0 : GEN11 OOB 출하명령 재검토.
//     2017.10.18.0 : GMES 클래스에서 StepComplete 시 DeleteNoneASCII 코드 추가 (GMES dll 에 마지막에 자꾸 쓰레기가 낀다고 해서)
//     2017.10.19.0 : GEN11 RESET AUDIO DSP 명령 추가.
//     2017.10.20.0 : GEN11, GEN11P SECURE LOGGING 관련 명령 추가.
//                  : ENABLE_SECURITY_LOGGING
//                  : DISABLE_SECURITY_LOGGING
//                  : READ_SECURITY_LOGGING
//                  : DsubRetrySignalFunc() 에 EXTERNAL 파워 사용시 EXT 릴레이 OFF 추가. (무인화 디서브 리트라이떄문에 파워를 꺼줘야하기때문
//     2017.10.23.0 : CCM Debug / user Mode Command 추가.
//     2017.10.24.0 : BOOT DELAY 명령 수정. BUB CHARGE OFF 명령 수정.
//     10.27.0 : GEN11P - 시큐어로깅 명령 수정 (연구소오타)
//     11.01.0 : GEN11P - PCAN - Secure Lock 해제 기능 추가.
//     11.08.0 : SHIFT_TIME 기능 추가. (for GPS 시뮬레이터 RESET TIME)
//             : MCTM - READ_GPRMC (RELIABILITY, LATITUDE, LONGITUDE ) 명령 추가 (UART2 로 수신되는 부분)
//             : ETC LOGGING BOX - AUTO SCROLLING 개선
//     11.21   : GEN11/P - READ_MICOM_IO_VERSION 명령 추가.
//             : PAGE-CHECK_FILE_SIZE 명령 추가.
//     11.22   : MCTM - GPS DISTANCE 명령 추가.//
//     11.23   : GEN10 - QMI_REQUEST 명령 추가. (BINPACK 타입 - bin log에서 메시지 찾기)
//     11.23.1 : PCAN - VALIDATE_SEED_KEY 명령 추가. (MAX 에 07 67 03 ~~ ~~ ~~ ~~ 01 로 패턴처리하면 PASS) 2017.11.22. 오전 9시:45분 이병권 과장 메일참조 PPT
//     11.24.0 : GEN11/P - CHECK SEED KEY 명령추가.
//     12.1.0  : MCTM COMMAND 추가 (READ_USIM_PROFILE, READ_USIM_NSPIF, READ_USIM_VVN, READ_USIM_CHIP_VERSION)
//     12.2.0  : gmes dll 업데이트 1.0.9

//     12.5.0   : MCTM - tbl 수정 ( write securiy key char -> byte , read security key char -> byte )
//     12.6.0   : GEN11 PCAN - secure logging status 상태 확인 명령 추가.
//     12.8.0   : KALS ( SEED - KEY DOWNLOAD 기능 추가 )
//     12.8.0   : GEN11 NV_ITEM 명령추가 (미테스트)
//     12.11.0  : DID_SEED_GEN11 대문자로 변경
//     12.14.0  : COMPARETYPE - UNDERCOVERAGE 기능 추가 (포함되어 있으면 NG)
//     12.15.0  : MCTM - LOCAL SEED FILE 읽어서 KEYWRITE 임시구현기능 추가.
//     12.16.0  : make aldl 시 bTempBlock ( MCTM 수정)
//     12.22.0  : MCTM OOB 바코드 리딩 기능 추가.
//    18.1.4.0  : KALS_GET_KEY 곤산용 site name 추가.
//    18.1.10.0 : KALS_returnWritingInfo 곤산용 site name 추가.
//    18.1.18.0 : GEN11 - BT/WIFI COMMAND 추가.
//    18.1.19.0 : GEN11 - WIFI(TX/RX)COMMAND 파라미터 넣을수 있도록 변경
//    18.1.24.0 : GEN11 - START/CLOSE PING TEST 변경 (연구소 요청)
//              : BENCH 명령추가(GEN11벤치 MUX - AUDIO, MIC, SPK)
//    18.1.24.0 : START_PING_TEST_NEW,  CLOSE_PING_TEST_NEW 로   변경
//              : MCTM seed key down  구현, nonecolon 옵션추가 (mac 비교시 콜론 제거)
//    18.1.27   : 크랙패스워드 룰변경 (유출되었음) - 문구도 더블클릭해야함.
//              : TC3000 에서 SET_TESETER_BD_ADDR, SET_DUT_BD_ADDR 어드레스 파라미터 전달시 : 콜론이 있다면제거.      
//              : BUB ENALBE STATUE, ENALBE, DISABLE - FAKE 명령 오타 수정.     
//    18.1.29   : BARCODE SUB ID 추가기능 - DISPLAY_BARCODE_SUB 로 출력가능
//    18.2.1    : MCTM 메인바코드 LABEL : 21Z -> 21Y, 20Z -> 20Y, 22Z -> 20Y 변경.
//    18.2.1.1  : usebarcode 에러 수정
//    2018.2.2  : DIO_COMMAND_CURRENT_CHECK 추가 (DIO CURRENT 명령 중에는 그래프에 안찍히므로 찍힐수 있도록 기능수정)
//    2018.2.13.0 : Skip 명령에 의한 pregress rate 표시 버그 수정.
//    2018.2.20.0 : EXCEL 파일 (DOCUMENT_FILE_LINK, DOCUMENT_ITEM_VIEW) 기능 추가. - 이동성 선임 -
//    2018.2.21.0 : NI_GPIB 클래스 내부 구조 변경/ NI 전용 VISA 표준 공용화함. 
//                : 일단 VISA 표준으로 사용되도록 Flag 로 구분지었으므로 문제발생시 이를 원복해야함. connect 부분만 바꾸면됨.
//                : GEN11 - B2B COMMAND 6개 추가.
//    2018.2.22.0 : EXCEL 파일 (DOCUMENT_FILE_LINK, DOCUMENT_ITEM_VIEW) 변경 하드코딩방식. ㅡㅡ 아오
//    2018.2.23.0 : DOCUMENT_FILE_LINK 명령 수정,
//                : CMW500 및 NAD LTE 관련 명령 프로젝트에서 제외.
//    2018.2.25.0 : CCM,NAD(gen10용) GPS 구현 완료 두개가 동일하나 쓰임새가 향후 달라질수 있으므로 두개로 나눔.
//    2018.3.03.0 : Config - Only OK GMES 옵션 추가 (테스트결과가 OK일때만 STEP-COMPLETE 함) 
//    2018.3.14.0 : MTP200.TBL SG-LOSS 관련 명령 2개 추가. (이동성 책임)
//    2018.3.16.0 : DOCUMENT_FILE_LINK 함수에서 spec item 갯수과 document 갯수가 같아도 내용에 대한 유효성이 검사될수 있도록 로직 추가.
//                : Compare Option "TEXT" 추가 - 대소문자 구분없이 비교
//    2018.3.21.0 : GEN11/GEN11P GB 모델 DTC COMMAND 및 DTC INDEX .INI 파일 추가. 
//                : GEN11 DTC COMMAND INDEX - 응답타입을 BYTE 로 변경. 
//    2018.3.24.0 : GEN11 PCAN - VALIDATE SEED KEY 명령시 절차서 변경없이 하드코딩으로 pending 메시지는 거를수 있도록 변경.
//    2018.3.27.0 : CycleEnging() 함수에서 마지막에 CycleCheck 함수 호출시 STATUS가 Running 이 아닐경우만 호출될수 있도록 조건변경. 타이밍상 status 변경으로 인해 CycleDone으로 안갈수 있는 확률이 존재.
//: CycleEngine 긴급 수정 - case (int)STATUS.NONE: //NONE 이면    CycleCore(i); 추가. (별도로  
//    2018.4.7.0  : DIO - BOOT DELAY 안먹는 현상 수정 
//    2018.4.10.0  : GEN11 READ BUTTON STATE 명령 추가. (이정안 선임 / 이동성 책임 베트남 출장중 요청)

//    2018.5.2.0 : GEN11 GB 모델 DTC 25,26 순서변경(U0155 <-> U0146)
//    2018.5.2.0 : GEN11 ALDL READ 시 파라미터 ADDRESS 입력시 2바이트 주소를 위해 High/low little endian 처리. 
//               : 기존에는 1byte만 입력해도 00 을 자동으로 채워 줬으나. 2byte 입력시에는 big endian으로 해서 안되었었음.
//               : PCAN   - EXTENDED 추가, 명령이름 앞에 EXTENDED_ 를 붙이면 작동.
//               : VECTOR - EXTENDED 추가, 명령이름 앞에 EXTENDED_ 를 붙이면 작동.
//               : GEN11 ALDL WRITE 시에도 ADDRESS block을 2바이트 주소를 위해 High/low little endian 처리. 
//               : PCAN, VECTOR - RECV 시 bEXTENDED 확인하여 응답받는 msg id 값에 0x80000 값 or 처리.
//         5.3 : GEN11 ALDL R/W 시 이전 block 체크 로직 추가.
//             : GEN11 - READ AMP TYPE 명령 추가ㅣ.
//              : MCTM WRITE_HW_FULL_VERSION, READ_HW_FULL_VERSION 추가(오경호책임/설중환 책임요청)
//
//   2018.5.4 : 중간에 포트가 날라가면 에러처리함. DK COMM  if (iStatus == (int)STATUS.ERROR)
//   2018.5.11: #HEXA_ expr 기능 추가.
//   2018.5.25: PCAN- GEN11 GB BOOT 명령추가.
//   2018.6.4 : READ_BUB_TEMPERATURE 명령추가 (이동성책임)
//   2018.6.13 : SET PRODUCTION MODE 명령추가.(이동성책임/이정안선임)
//   2018.6.20 : END SIGNAL OPEN - CONFIG 로딩 방식 및 룰 완전변경함 (PLC모드에서만) - RELAY 스테이터스는 추가해야함. UI만 해놓음.
//   2018.6.20 : SERIAL COM 쪽 delay, timeout 측정방식 PC 시간 계산방식에서 stopwatch 방식으로 변경
//   2018.6.23 : KALS 관련 명령 try catch 처리, retry 될수있도록 변경. 
//   2018.6.24 : Closedxml class 에 MemoryClearFunc() 함수 추가. 라이브러리 자체가 메모리 누수가 있다.

//   2018.6.26 : Closedxml 사용 철회..... MS 엑셀로 다시 사용. (일단 클래스와  DLL은 남겨두자.)



//  2018.6.25 : nuget : //ClosedXML 추가 Install-Package ClosedXML -Version 0.87.1  <-- 여기까지가 닷넷 4.0
//                       closexml 로 자동으로 안되면 요건 별도로 Install-Package DocumentFormat.OpenXml -Version 2.8.1
//                       https://www.nuget.org/packages/ClosedXML/0.87.1 사이트 참조

//   2018.6.27 : Closedxml 다시 사용 return 시  bool 로 받아서 넘기는것으로 바꾸어봄.
//             : CheckMemory() 800메가 넘으면 프로그램 재기동...보험처리

//  2018.6.28 : 후처리 retry count 3 회에서 6회로 증가. 
//  2018.6.28.1 : GMES STEP COMPLETE 시 CHECK, MES, ERROR 등은 GMES 업로드 skip한다. (이동성 책임 요청)
//  2018.7.4.0 : PCAN - GEM 모델 CAN WAKEUP 명령 추가. 
//             : GEN11P WRITE FACTORY/ENGINERR/PRODUCTION 모드 명령 수정.
//             : MCTM - TBL (EFS BACKUP NAD3, CHECK EFS NAD3 오타수정_)
//  2018.7.7.0 : GEN10 - 
//               GET_SERVICE_INFORMATION, READ_SERVICE_INFORMATION, READ_SERVICE_STATUS 명령 추가.

//  2018.7.7.0 : GEN11(GEM 모델 CAN WAKEUP - VECTOR이용) 기능 추가. (GEM BOOT, GB BOOT)
//  2018.7.12.0 : GEN11 ALDL BLOCK SIZE 200바이트 에서 250바이트로 증가된 모델 출현에 의해 ALDL2 명령추가 

//  2018.7.14.0 : GEN10(VCP,TCP), GEN11, NORMAL/FACTORY NAD SERVICE INFORMATION 명령추가
//              : MCTM command tbl 패킷 중복유효성 검사 기능추가. CheckDuplicateTBL 로 검색

//  2018.7.17.0 : READ_DID_HEXA 추가. (DID 데이터가 ascii 여도 무조건 hexa 로 표시하기

//  2018.7.19.0 : READ_NAD_SERVICE_STATUS (GEN11,GEN11P) 명령추가 (기존 NAD SERVICE report 데이터 외에 명령으로 추가)
//              : 강종훈 책임메일 참조 2018.07.19 오후 5시 39분
// 
//  2018.7.20.0 : READ_BUB_MODEL2 명령추가.(이동성책임/이진성책임)
//              : CASENG - MONITOR 옵션추가. (판정이나 통신페일시에도 OK 처리됨)
//              : GMES_GetInsp 시도시 dll 버그때문에 null 이면 step check를 재시도 처리반영해봄...........될런지는모름.

//  2018.7.22.0 : CASENG - MONITOR 버그 수정

//  2018.7.23.0 : "Zero Condition RetryCount" 추가 (절차서에 리트라이를 0으로 해놓으면 무한루프에 빠지는걸 예방)

//  2018.8.6.0 : MES - OOB 공정 OOB 재검을위해 OOB CODE 무시하도록 옵션추가 (DONT CARE OOBCODE)

// 2019.8.10.0  : 긴급수정 CONTAIN2 옵션 버그 수정.
// 2019.8.10.0  : GET_SERVICE_INFORMATION (for GEN10 TCP) 재변경. 연구소 요청

// 2019.8.11.0  : LOAD_WIFI_DRIVER, UNLOAD_WIFI_DRIVER 명령추가 (이정안선임요청)
// 2019.8.13.0  : GEN11 NVITEM 버그 수정

// 2019.8.14.0 : GEN11 .OOB 공정 허니웰 바코드 스캐너 제어모듈 추가.

// 2019.8.17.0 : GEN10 / LTE DLL(연구소DLL 김진한선임) NVGET 명령추가.
//             : GEN11/GEN11P : GEN11 GEM모델 DTC리스트 추가.
// 2019.8.20.0 : CCM / NAD(보성) : NV946 명령 추가 (이동성 책임) - 보성 GP12 공정 신규로 변경함.
//             : GEN11, GEN11P DTC TABLE 파일 누락부분 수정.
//             : DELAY timer 교체 (stopwatch)

// 2019.8.20.0 : DID_SEED_GEN11_VCP 추가. 대문자로 변경
// 2018.8.28.0 : GEN11/GEN11P - READ_IMSI_NAD 명령추가. (이정안선임요청)
// 2018.8.30.0 : MCTM DIO TBL 파일 (KEYPAD FRONT/READ DOT 버튼 명령 수정)

// 2018.9.12.0 : GEN11 GEM PCAN/VECTOR - GEM TT ENABLE/DISABLE 명령추가.
//             : GEN11 TBL - GB WRITE GB MASTERKEY/WRITE GB UNLOCKEY/WRITE GB ECUID/READ ECUID 추가.(이동성 책임)
//             : GEN11P TBL - READ ECUID 추가.

// 2018.9.13.0 : KALS - GM_GB_GEN11 추가. 

// 2018.9.17.0 : if (!STEPMANAGER_VALUE.bUseMesOn) { strRtnInsp = ""; return false; } 추가. 

// 2018.9.26.0 : KALS - 확장자 bin -> dat 수정
// 2018.10.8.0 : KALS - GM_GB_GEN11 수정
//      11.5 : static 삭제

// 2018.12.12 PCAN TBL - MCTM_LSCAN_VNMF, MCTM_LSCAN_TEST 추가

// 2018.12.12 PCAN TBL - MCTM_LSCAN_VNMF, MCTM_LSCAN_TEST 추가

// 2019.01.18 : GEN11 - WRITE KEY IO (MATER, UNLOCK, GENERAL, PROVISION) 추가.  파라미터의 M1M2M3 는 어디서 가져옴?? 물어보자
//            : GEN10 - 1KB 이상의 로컬파일 업로드 명령 추가구현(Transter 시 multiple 로 리트라이 1회로 구성해야함)
// 2019.01.19 : 로그 히든 구현. FileEnc.exe 추가.
// 2019.01.23 : 후처리 2nd 리트라이 3회 에서 6회로 증가.

// 2019.01.29 : KALS DLL - ConvertFileName함수추가.... 파일사이즈가 30바이트이상 될경우;;;; 처리함.

// 2019.01.29.1 : ATCO GEN11 UNLOCK 수정 - ChangeDisableBit 함수 추가. :disable 뒤에 비트처리함.
// 2019.01.31.0 : 허니웰 스캐너 enum 값 수정.
// 2019.02.26.0 : ADC, AUDIO 셀렉터 포트오픈 버그 수정
// 2019.02.27.0 : GEN11/P  명령추가.
//                INITIATE_RESET_AUTH_ACTIVE_COUNTER,
//                CHECK_JTAG_LOCK, WRITE_JTAG_LOCK, RESTORE_BACKUP_FILES 
//                READ_MICOM_SERIAL_NUMBER, READ_AUTH_COUNTER, READ_ACTIVE_COUNTER      

// 2019.04.12 : Marshal.FreeHGlobal(informations); 추가. kals.
// 2019.04.12 : PEPU.DLL 구현 완료 - PAGE : GET_PEPU_PASSWORD 구현으로 아래 2개명령 동반 구현 완료
//              WRITE_JTAG_LOCK_WITH_PEPU_PW, WRITE_JTAG_UNLOCK_WITH_PEPU_PW (이동성책임/김인권선임) 작업시간 2틀
// 2019.04.16 : GEN10 - APN TABLE 명령 구현 추가 (이동성책임/김인권선임) 작업시간 2틀
// 2019.05.04 : Gen11CcmGpsCommand 에서 딜레이 주는 옵션때문에 CCM (NAD) 관련 절차서의 딜레이가 중복으로 먹는현상이 있어서 수정함.
// 2019.05.09 : PAGE - LOAD_MFG_FILE 명령추가 (ATCO 에서 구형프로그램에서 쓰는 MFG 데이터 파일 불러오는 명령)
//            : GEN10, GEN11P, ATT 에서 APN "READ_APN_INFO_ALL" 추가. (옵션을 합쳐서 보여주는 기능 O,X,X,X 이런식으로)

// 2019.05.16 : DIO - PLC ERROR (NG+CHECK) (ON,OFF,CLICK) 신호 추가 for 로보트
// 2019.05.31 : WRITE_JTAG_LOCK_WITH_PEPU_PW 프로토콜 변경 46 43 35 4B 66 -> (46 43 35 4C 66) - 이동성책임/연구소 요청
//            : PEPU 호출시 EXPR 파라미터가 사용될 경우 변환코드 누락된것 수정.
// 2019.06.08 : GEN10 - GPS/GNSS SV COUNT 명령어 "2" 추가. 기존것은 REPORT 데이터로 수신. 이것은 명령으로 가져옴
// 2019.06.10.0 : PLAYCHECKER 타임 변경 - Stopwatch 사용 및 결과값 컨트롤에서 가져오지 않고 PLAYCHECK 함수로 가져오는것으로 변경.
// 2019.06.12.0 : PCAN.TBL - GB SECURE LOGGING STATUS GB 추가 및 TBL 데이터 형식 추가. GBSECURE, DEFALUT, RECVCODE 등등.
// 2019.06.13.0 : PCAN 잘못수정된부분 bug fix
// 2019.06.14.0 : KEITHLEY POWER 2306 장비 GPIB 인터페이스 추가(서성호책임) 및 프로그램 종료시 ODAPOWER , KEITHLEY POWER 자동 OFF 함수 추가.
// 2019.06.26.0 : KEITHLEY 명령 추가. SET_CURRENT_RAGNE_AUTO_ON, SET_CURRENT_RAGNE_AUTO_OFF
//                                   SENS:CURR:RANG:AUTO OFF, GET_CURRENT_RAGNE_AUTO, GET_CURRENT_RAGNE
// 2019.06.28.0 : KEITHLEY 값 소수점 7자리까지 나타낼수 있도록 요청. 기존 4자리. (서성호 책임)
// 2019.07.08.0  : UpdateDictionary 함수 추가 - 딕서녀리 버그 fix
// 2019.07.11.0  : KALS SITE CODE 추가 - 남경용
// 2019.07.30.0  : NUMBER 형태 비교시 Measure 표시 7자리로 증설

// 2019.08.06.0 : GEN11P - TBL 추가. (이병권선임/이진성책임 요청)
//                WRITE_ALL_GENERAL_KEY_IO, CHECK_PROVISIONING_MASTER_KEY, CHECK_PROVISIONING_UNLOCK_KEY, CHECK_PROVISIONING_GENERAL_KEY

// 2019.08.12.0 : PCAN SEND 옵션시 행업 현상 수정 ;;;
// 2019.09.4.0 : PEPU.DLL, OPEN SSL DLL 프로젝트에 포함 (이동성책임_)
// 2019.09.16.0 : PEPU.DLL, CHECK VCP/TCP SEED 명령어 기능 추가(이동성 책임, 김인권선임)- 유지보수 4일 작업 
//                GEN11P - INITIATE_RESET_AUTH_ACTIVE_COUNTER 추가.
//                GEN11/GEN11P - CHECK_PROVISIONING_GENERAL_KEY_RN 각각 추가.
// 2019.09.18.0 : PEPU.DLL, CHECK VCP/TCP SEED 메뉴얼 테스트 기능 추가.
// 2019.10.10.0 : SKIP 명령의 경우에 GMES ITEM 명령을 참조하는 항목이 있다면 검색하는 버그  FIX (안하도록)
//              : 5515E 도 사용할수 있도록 변경
// 2019.10.15.0 : CheckDeviceName()추가. 5515C 관련건
// 2019.10.29.0 : AMS DLL 연동 추가.(작업 1주일), AMS_DATA_VIEW 추가., AMS_ERROR_CODE 추가
// 2019.10.30.0 : AMS 수정
// 2019.10.30.1 : AMS 수정 save
// 2019.10.31.0 : AMS dll 업데이트 (cns) - stid 폴더생성되는 현상
// 2019.10.31.1 : ExcuteConv 함수에서 int 에서 long 으로 변경
// 2019.10.31.2 : ExcuteConv 함수에서 int 에서 uint 으로 변경
// 2019.11.4.0 : CNS - AMS dll 변경

// 2019.11.5.0 : 5515c tbl 추가 - CDMA_AANALYZER_FREQUENCY_VALUE (for Gen9 cdma)
//      11.29 : DebugView 에 ocx 로딩실패 익셉션 메시지 팝업 추가 및 CrossThreadIssue.ChangeTextControl 대량 작업 추가.
// 2020.1.8.0 : pcan - Item_dTimeSet += 0.5; //pendding 시 타임아웃시간 늘리기
// plc scanner error signal add
// 2020.2.4.0 : pcan - pendding 시 타임아웃시간 3초 늘리기
// 2020.2.11.0 : Use Plc - usb 바코드 입력되면 체크신호 클릭주기 기능 추가(for 로봇, 송인도책임)
// 2020.3.13.0 : 셀 더블클릭시 win10 에서는 c root 에 저장이 안되므로 로그폴더에서 저장되도록 변경
// 2020.3.27.0 : GEN11 MY23 모델 ALDL3 사이즈 추가로 인한 내부 테이블 대량 코드 변경 (ALDL 단위 명칭등) - 작업 8시간

// 2020.5.6.0  : PCAN-GEN9 명령추가.
// 2020.5.13.0 : AVERAGE 경우도 옵셋값이 필요한경우도 있어서 기능 추가해놓음 (AVERAGE 경우 로 검색)

// 2020.5.29.0 :KALS GetKey 에 Marshal.FreeHGlobal(informations); 추가. 메모리누수?

// 2020.6.5.0 : 검사 자동 반복기능 추가 - RepeatTestFunc() 로 검색 
// 2020.6.9.0 : "CHANGE_JOB" 명령에서  STEPMANAGER_VALUE.bUseMesOn 조건 추가. 이동성책임/이병권선임 요청
//        10    :KALS GetKey 에 Marshal.FreeHGlobal(informations); try catch 추가. 

// 2020.6.22.0  : MD5 빌드	CheckLgeMD5();	

// threadNadKeyDLL 삭제
// 2020.7.3 : Patterns 비교타입 추가
// 2020.7.7 : gen9 구현 1차 빌드
// 2020.7.9 : gen9 gps 명령 [H] 도 추가.
// 2020.7.10: NAD.TBL -> READ_NV_441 추가 (이동성 책임)
// 2020.7.11: undercoverage2 비교옵션 추가. 콤마로 구분된 max 의 값에 measure 값이 포함되면 ng 처리하는 기능.
// 2020.7.13: GEN9.7 K/W 구현완료........ 힘들다.
// 2020.7.28: GEN9 GPS 구현완료. 
// 2020.7.29: GEN9 GPS 구현완료. 
// 2020.7.29.1: GEN9 GPS 구현완료. 

// 2020.9.11 : MD5 이용시 KALS DLL 파일 경로가 C:\GMES\LGE.SWP\TEMP 로 변경되는 불상사가 생긴단다. 그래서 아래 코드 추가.
////           : if (STEPMANAGER_VALUE.bIamMD5) 로 검색하면 댑니다.


// 2020.9.16.0: MELSEC 적용 ( 2.5 일작업처리)
//            : MD5 경로 이상해지는 현상때문에 로거의 GetFileSize 함수 변경 strProgramPath

// 2020.9.17.0: GEN9.TBL - [H]READ_PLATFORM_96,[H]READ_CAN_TYPE_96,[H]READ_CAN_TYPE_NAME_96, [H]READ_BT_TYPE_96 추가. (이동성책임_)

//        19.0: LoadPlcAddress(); 누락된것 추가.
//       .28.0: 5515C - FAST_SWITCH_FORMAT_WCDMA,  FAST_SWITCH_FORMAT_GSM  ,  FAST_SWITCH_FORMAT_CDMA  명령수정 (이동성책임)
//            : GEN9.TBL -  AUTO_ANSWER_ENABLE, [H]AUTO_ANSWER_ENABLE 추가.

// 2020.10.14 : KIS_KEY_DOWNLOAD_MANUAL 추가 (이동성책임요청 - 수동테스트를 위해)
// 2020.10.15 : MakeEfileStruct (gen9 쪽) EFILE 파라미터 3번째꺼에 키값을 직접 사용할수 있도록 변경, (이동성책임요청 - 수동테스트를 위해)
//            : Gen9 아날라이져 변경 - 사이즈 16 에서 15로 변경;;; 왜이랬지;;; 
// 2020.10.21 :: WRITE_OMA_AUTHKEY_IMEI 추가
//            : Gen9 : ALDL1_ASCII_EX 타입추가. 파라미터 5개 이며, 끝에 2개를 START POINT, LENGTH 를 넣어주면 됌, 0부터 시작임 
//            : DLL GATE 업데이트 (ESN DLL 관련 추가)
// 2020.10.22 : PAGE 명령중 MAKE PARAMETERS 명령 추가 (expr, gmes, doc 등 전부 쓰면 쓰는데로 합칠수 있음.
// 2020.10.27 : ESN DLL 교체 및 구현 완료.
//            : GEN9 로컬파일 업로드 구현 완료. 512kb 이상 안올라가던 문제 해결 .
//            : GEN9 파일사이즈 구하는 명령 완료.

// 2020.10.28 : 5515C.TBL - 수정변경 및 추가. GSM_SET_BER-TIME(수정)   WCDMA_BER_TIMEOUT(추가) - 이동성책임요청.
//        : ESN 최종테스트 완료

// 2020.11.2  : case "NOTPATTERN": iRtnCode = CompareNotPattern(strData, strMax); break; 추가. 이동성책임요청
// 2020.11.10 : GEN9 - WRITE_NAI, [H]WRITE_NAI 추가. "GEN9NAI" 타입 추가. nai,pw 로 par1 입력.

// 2020.11.11 : GEN9 - DLLGATE - DLL_ESN_READ_DIRNUMBER 추가.
//            : GEN9 TBL : CHECK_CHAP, CHECK_DMU 수정 및 변경.
//            : GEN9 TBL :[H]READ_NAI , READ_NAI  추가.

// 2020.11.19 : GEN11 - GEN11RESTYPE.ALDL_HEXA 타입 추가. (aldl read 시 무조건 hexa 로 리드함)
// 2020.12.17 : MCTM TBL - DTC ALL -> DEBUG 에서 NONE 으로 수정;;;

// 2021.03.24 : PCAN.TBL - AutoSendingGB 함수 (145 -> 500ms,, 284 -> 250ms 주기로 변경요청)) 이동성책임, 연구소 배범근,,
// 2021.04.01 : DIO.TBL - ETHERNET POWER ON / OFF 명령 추가. (이동성 책임, 이창민차장)

// 2021.04.19   : AMS_KEY_DOWNLOAD 함수 신규파라미터 추가. 패스워드 파일(pem) 파라미터 추가해달라고해서 신규 추가한 함수 , 이동성책임 요청 strRoot, strIdentity, strpw
// 2021.04.19.1 : 수정. msvcr120.dll 포함요청
// 2021.04.19.2 : AMS_KEY_DOWNLOAD 함수. WorkingDirectory  추가... 하 이거 프로세스 위치 지정해줘야함...

// 2021.04.30   : AMS_KEY_DOWNLOAD 함수 AMS DLL, AMSTest.exe 변경 (김종인 책임, 이동성 책임) 베트남용 (Factory 파라미터 추가)
// 2021.05.29   : GEN11 ALDL3 사이즈 재변경요청 - 321 에서 330 으로  (이진성책임, 이정우책임,)
//Ver 2021.6.17.0 : PCAN, VECTOR TBL 명령 추가.(MY23 뭐시기)
//Ver 2021.6.29.0 : AMS_KEY_DOWNLOAD 함수에서 1번 IP 실패시 2번째 IP로 변경하여 시도하도록 변경.
//Ver 2021.8.9.0  : 이동성 책임 요청으로 KIS에는 STID 값에 0이 껴있으면 0을 제거하고 값을 넣는 기능이 있는데, AMS에는 그런 기능이 없어서 추가함.
//Ver 2021.8.13.0 : 이병권 책임 요청으로 AutoSendingGB_MY23 함수에서 strCanGBMessage2 메세지 길이 6에서 8로 늘리고, 데이터 변경됨.(00 00 00 00 10 00 => 00 00 00 00 00 00 10 00)
//Ver 2021.8.26.0 : AMS KEY DOWNLOAD 할때 남경이냐 베트남이냐에 따라 Param 갯수가 틀림. 기존에 STID 값에 0 이 존재하면 없에주는 작업을 진행하였는데, 그것이 남경꺼에만 적용이 되었던 것. 다시 수정함.
//                  해당 부분 확인하려면 "int iNonZeroStid = 0;" 검색하면 됨.
//Ver 2021.10.22.0 : 이동성 책임 요청으로 우선 SET 통신만 가능하도록 수정하여 보내달라 해서 GEN12 명령은 추가 하였지만 TBL FILE은 그냥 BMW WAVE꺼 그대로 가져다 붙임.
//Ver 2021.10.26.0 : 이동성 책임 요청으로 GEN9 STX의 값이 0xFA, 0xFB 로 오면 전부 적용 되도록 변경해달라하여 GEN9 분석 클래스에서 STX를 두가지다 허용 하도록 수정
//Ver 2021.10.26.1 : 윗줄에 대한 내역은 원복 되었으며, 특정 모델에 대한 ALDL 명령에 대한 응답이 기존과 다르게 오기때문에 그 것만 따로 추가해 주기 위한 소스문 작성 되었음.
//Ver 2021.12.3.0  : TC1400A 제어 및 TBL FILE 추가.
//Ver 2022.2.17.0  : Config에 GPIB 통신 하는 장치 주소 넣을때 칸이 밀려서 잘못 저장되고 있었음.


//-------------------------------------------------------------------------------------------------  //
// 버젼내역이 업데이트 되면 LoadingVersionHistory()함수에 대외용으로 표시될 히스토리 내용을 항상 추가해주세요      //
//-------------------------------------------------------------------------------------------------  //