#include "Common.h" 

// [ファイルスコープ変数]
static ULONG f_errorBits = 0; 	// FWエラー
static critical_section_t f_stSpinLock = {0};	// スピンロック
static ST_QUE f_aQue[CMN_QUE_KIND_NUM] = {0}; 	// キューの配列
static UCHAR f_aQueData_usbWlSend[CMN_QUE_DATA_MAX_USB_WL_SEND] = {0};	// USB/無線送信キューのデータ配列 
static UCHAR f_aQueData_uartSend[CMN_QUE_DATA_MAX_UART_SEND] = {0};		// UART送信キューのデータ配列
static UCHAR f_aQueData_uartRecv[CMN_QUE_DATA_MAX_UART_RECV] = {0};    	// UART受信キューのデータ配列
static ST_I2C_REQ f_astQueData_i2cReq[CMN_QUE_DATA_MAX_I2C_REQ] = {0}; 	// I2C送信/受信キューのデータ配列
static UCHAR f_aQueData_wlRecv[CMN_QUE_DATA_MAX_WL_RECV] = {0}; 	 	// 無線受信キューのデータ配列

// エンキュー
bool CMN_Enqueue(ULONG iQue, PVOID pData, ULONG size, bool bSpinLock) 
{
	ULONG errorBit;
	bool bRet = false;
	ST_QUE *pstQue = &f_aQue[iQue];
	UCHAR* puchr;
	ST_I2C_REQ* pstI2cReq;

	if (bSpinLock) {
		CMN_EntrySpinLock(); // スピンロックを獲得
	}

	if ((pstQue->head == (pstQue->tail + 1) % pstQue->max)) { 
		// キューが満杯の場合	

		// FWエラーを設定
		switch (iQue) {
		case CMN_QUE_KIND_USB_WL_SEND: // USB/無線送信
			 errorBit = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_USB_WL_SEND;
			break;
		case CMN_QUE_KIND_UART_SEND:   // UART送信
			errorBit = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_UART_SEND;
			break;
		case CMN_QUE_KIND_UART_RECV:   // UART受信
			errorBit = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_UART_RECV;
			break; 
		case CMN_QUE_KIND_I2C_REQ:     // I2Cマスタ送信/受信
			errorBit = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_I2C_REQ;
			break;
		case CMN_QUE_KIND_WL_RECV:	   // 無線受信
			 errorBit = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_WL_RECV;
			break;			
		default:
			// ここに来ない
			break;			
		}
		// スピンロック獲得済みであればスピンロックを獲得しない。未だ獲得していない場合、スピンロックを獲得する。
		CMN_SetErrorBits(errorBit, !bSpinLock);
	}
	else {
		if (TIMER_IsStabilizationWaitTimePassed()) { 
			// 起動してからの安定待ち時間が経過していた場合

			// キューイング
			switch (iQue) {
			case CMN_QUE_KIND_USB_WL_SEND:	// USB送信 
			case CMN_QUE_KIND_UART_SEND: 	// UART送信
			case CMN_QUE_KIND_UART_RECV: 	// UART受信
			case CMN_QUE_KIND_WL_RECV:	 	// 無線受信  
				puchr = (UCHAR*)pstQue->pBuf;
				memcpy(&puchr[pstQue->tail], pData, size);
				break; 
			case CMN_QUE_KIND_I2C_REQ:   	// I2Cマスタ送信/受信
				pstI2cReq = (ST_I2C_REQ*)pstQue->pBuf;
				memcpy(&pstI2cReq[pstQue->tail], pData, size);
				break;
			default:
				// ここに来ない
				break;				
			}	
			pstQue->tail = (pstQue->tail + 1) % pstQue->max;
		}
		else {
			// 破棄
		}
		bRet = true;
	}

	if (bSpinLock) {
		CMN_ExitSpinLock(); // スピンロックを解放
	}

	return bRet;
}

// デキュー
bool CMN_Dequeue(ULONG iQue, PVOID pData, ULONG size, bool bSpinLock)
{
	bool bRet = false;
	ST_QUE *pstQue = &f_aQue[iQue];	
	UCHAR* puchr;
	ST_I2C_REQ* pstI2cReq;	
	
	if (bSpinLock) {
		CMN_EntrySpinLock(); // スピンロックを獲得
	}

    if (pstQue->head == pstQue->tail) {
		// キューが空の場合

		// 無処理
	}
	else {
		// デキュー
		switch (iQue) {
		case CMN_QUE_KIND_USB_WL_SEND:	// USB/無線送信 
		case CMN_QUE_KIND_UART_SEND: 	// UART送信
		case CMN_QUE_KIND_UART_RECV: 	// UART受信
		case CMN_QUE_KIND_WL_RECV:	 	// 無線受信 		
			puchr = (UCHAR*)pstQue->pBuf;
			memcpy(pData, &puchr[pstQue->head], size);
			break; 
		case CMN_QUE_KIND_I2C_REQ:   	// I2Cマスタ送信/受信
			pstI2cReq = (ST_I2C_REQ*)pstQue->pBuf;
			memcpy(pData, &pstI2cReq[pstQue->head], size);
			break;
		default:
			// ここに来ない
			break;				
		}	
		pstQue->head = (pstQue->head + 1) % pstQue->max;
		bRet = true;
	}

	if (bSpinLock) {
		CMN_ExitSpinLock(); // スピンロックを解放
	}

    return bRet;
}

// スピンロックを獲得
// スピンロックはCPU間排他をしつつ割り込みを禁止にする場合に使用する。
// CPU間排他だけならミューテックスを使用すること。
// Picoのcritical_section(spin lock)とmutexの定義は下記。
// https://www.raspberrypi.com/documentation/pico-sdk/high_level.html#pico_sync
// critical_section(spin lock):
// Critical Section API for short-lived mutual exclusion safe for IRQ and multi-core. 
// mutex:
// Mutex API for non IRQ mutual exclusion between cores. 
void CMN_EntrySpinLock()
{
	critical_section_enter_blocking(&f_stSpinLock);
}

// スピンロックを解放
void CMN_ExitSpinLock()
{
	critical_section_exit(&f_stSpinLock);
}

// チェックサム検査を実行
bool CMN_Checksum(PVOID pBuf, USHORT expect, ULONG size)
{
	UCHAR* pDataAry = (UCHAR*)pBuf;
	USHORT checksum = 0;
	ULONG i;
	bool bRet = false;

	// チェックサムの値を計算
	for (i = 0; i < size; i++) {
		checksum += pDataAry[i];
	}

	// チェックサム値のチェック
	if (checksum == expect) {
		bRet = true;				
	}

	return bRet;	
}

// チェックサムを計算
USHORT CMN_CalcChecksum(PVOID pBuf, ULONG size)
{
	UCHAR* pDataAry = (UCHAR*)pBuf;
	USHORT checksum = 0;
	ULONG i;

	for (i = 0; i < size; i++) {
		checksum += pDataAry[i];			
	}

	return checksum;
}

// FWエラーを設定
void CMN_SetErrorBits(ULONG errorBit, bool bSpinLock)
{
	if (bSpinLock) {
		CMN_EntrySpinLock();
	}

	f_errorBits |= errorBit; // OR演算はアトミックではないので排他する

	if (bSpinLock) {
		CMN_ExitSpinLock();
	}
}

// FWエラーを取得
ULONG CMN_GetFwErrorBits(bool bSpinLock)
{
	ULONG errorBits;

	if (bSpinLock) {
		CMN_EntrySpinLock();
	}

	errorBits = f_errorBits;

	if (bSpinLock) {
		CMN_ExitSpinLock();
	}	

	return errorBits;
}

// FWエラーをクリア
void CMN_ClearFwErrorBits(bool bSpinLock)
{
	if (bSpinLock) {
		CMN_EntrySpinLock();
	}

	f_errorBits = 0;

	if (bSpinLock) {
		CMN_ExitSpinLock();
	}	
}

// watchdog_enable()を使用して即WDTタイムアウトで再起動する
// 普通に、起動時にwatchdog_enable()を実行し、メインループでwatchdog_update()でWDTクリアする方法をしない理由:
// 上記の普通の方法の場合、PCでuf2ファイルをドラッグで書き込むと、なぜかwatchdog_enable_caused_reboot()がtrueを返すため
void CMN_WdtEnableReboot()
{
    watchdog_enable(1, true);
    while (1) {}		
}

// watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
void CMN_WdtNoEnableReboot()
{
    watchdog_reboot(0, 0, 1);
    while(1) {};   	
}

// 共通ライブラリを初期化
void CMN_Init()
{
	// [変数を初期化]
	critical_section_init(&f_stSpinLock);

	f_aQue[CMN_QUE_KIND_USB_WL_SEND].pBuf = (PVOID)f_aQueData_usbWlSend;
	f_aQue[CMN_QUE_KIND_UART_SEND].pBuf   = (PVOID)f_aQueData_uartSend;
	f_aQue[CMN_QUE_KIND_UART_RECV].pBuf   = (PVOID)f_aQueData_uartRecv;
	f_aQue[CMN_QUE_KIND_I2C_REQ].pBuf     = (PVOID)f_astQueData_i2cReq;
	f_aQue[CMN_QUE_KIND_WL_RECV].pBuf     = (PVOID)f_aQueData_wlRecv;

	f_aQue[CMN_QUE_KIND_USB_WL_SEND].max = CMN_QUE_DATA_MAX_USB_WL_SEND;
	f_aQue[CMN_QUE_KIND_UART_SEND].max   = CMN_QUE_DATA_MAX_UART_SEND;
	f_aQue[CMN_QUE_KIND_UART_RECV].max   = CMN_QUE_DATA_MAX_UART_RECV;
	f_aQue[CMN_QUE_KIND_I2C_REQ].max     = CMN_QUE_DATA_MAX_I2C_REQ;
	f_aQue[CMN_QUE_KIND_WL_RECV].max     = CMN_QUE_DATA_MAX_WL_RECV;
}