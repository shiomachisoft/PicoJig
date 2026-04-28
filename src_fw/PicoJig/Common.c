// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h" 

// [File scope variables] / [ファイルスコープ変数]
static volatile ULONG f_errorBits = 0; 	// FW error / FWエラー
static critical_section_t f_stSpinLock = {0};	// Spin lock / スピンロック
static ST_QUE f_astQue[CMN_QUE_KIND_NUM] = {0}; // Array of queues / キューの配列
static UCHAR f_aQueData_usbWlSend[CMN_QUE_DATA_MAX_USB_WL_SEND] = {0};	// Data array for USB/wireless send queue / USB/無線送信キューのデータ配列 
static UCHAR f_aQueData_uartSend[CMN_QUE_DATA_MAX_UART_SEND] = {0};		// Data array for UART send queue / UART送信キューのデータ配列
static UCHAR f_aQueData_uartRecv[CMN_QUE_DATA_MAX_UART_RECV] = {0};    	// Data array for UART receive queue / UART受信キューのデータ配列
static ST_I2C_REQ f_astQueData_i2cReq[CMN_QUE_DATA_MAX_I2C_REQ] = {0}; 	// Data array for I2C send/receive queue / I2C送信/受信キューのデータ配列
static UCHAR f_aQueData_wlRecv[CMN_QUE_DATA_MAX_WL_RECV] = {0}; 	 	// Data array for wireless receive queue / 無線受信キューのデータ配列

// Enqueue / エンキュー
bool CMN_Enqueue(ULONG iQue, PVOID pData, bool bSpinLock) 
{
	ULONG errorBits = 0;
	bool bRet = false;
	ST_QUE *pstQue = &f_astQue[iQue];
	UCHAR* puchr;
	ST_I2C_REQ* pstI2cReq;

	if (bSpinLock) {
		CMN_EntrySpinLock(); // Acquire spin lock / スピンロックを獲得
	}

	if ((pstQue->head == (pstQue->tail + 1) % pstQue->max)) { 
		// If queue is full / キューが満杯の場合	

		// Set FW error / FWエラーを設定
		switch (iQue) 
		{
			case CMN_QUE_KIND_USB_WL_SEND: // USB/wireless send / USB/無線送信
				errorBits = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_USB_WL_SEND;
				break;
			case CMN_QUE_KIND_UART_SEND:   // UART send / UART送信
				errorBits = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_UART_SEND;
				break;
			case CMN_QUE_KIND_UART_RECV:   // UART receive / UART受信
				errorBits = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_UART_RECV;
				break; 
			case CMN_QUE_KIND_I2C_REQ:     // I2C master send/receive / I2Cマスタ送信/受信
				errorBits = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_I2C_REQ;
				break;
			case CMN_QUE_KIND_WL_RECV:	   // Wireless receive / 無線受信
				errorBits = CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_WL_RECV;
				break;			
			default:
				// Unreachable / ここに来ない
				break;			
		}
		// Set FW error / FWエラーを設定
		CMN_SetErrorBits(errorBits, false);
	}
	else {
		if (TMR_IsStabilizationWaitTimePassed()) { 
			// If stabilization wait time after boot has passed / 起動してからの安定待ち時間が経過していた場合

			// Queuing / キューイング
			switch (iQue) 
			{
				case CMN_QUE_KIND_USB_WL_SEND:	// USB send / USB送信 
				case CMN_QUE_KIND_UART_SEND: 	// UART send / UART送信
				case CMN_QUE_KIND_UART_RECV: 	// UART receive / UART受信
				case CMN_QUE_KIND_WL_RECV:	 	// Wireless receive / 無線受信  
					puchr = (UCHAR*)pstQue->pBuf;
					memcpy(&puchr[pstQue->tail], pData, sizeof(UCHAR));
					break; 
				case CMN_QUE_KIND_I2C_REQ:   	// I2C master send/receive / I2Cマスタ送信/受信
					pstI2cReq = (ST_I2C_REQ*)pstQue->pBuf;
					memcpy(&pstI2cReq[pstQue->tail], pData, sizeof(ST_I2C_REQ));
					break;
				default:
					// Unreachable / ここに来ない
					break;				
			}	
			pstQue->tail = (pstQue->tail + 1) % pstQue->max;
		}
		else {
			// Discard / 破棄
		}
		bRet = true;
	}

	if (bSpinLock) {	
		CMN_ExitSpinLock(); // Release spin lock / スピンロックを解放
	}

	return bRet;
}

// Dequeue / デキュー
bool CMN_Dequeue(ULONG iQue, PVOID pData, bool bSpinLock)
{
	bool bRet = false;
	ST_QUE *pstQue = &f_astQue[iQue];	
	UCHAR* puchr;
	ST_I2C_REQ* pstI2cReq;	
	
	if (bSpinLock) {
		CMN_EntrySpinLock(); // Acquire spin lock / スピンロックを獲得
	}

	if (pstQue->head == pstQue->tail) {
		// If queue is empty / キューが空の場合

		// No processing / 無処理
	}
	else {
		// Dequeue / デキュー
		switch (iQue) 
		{
			case CMN_QUE_KIND_USB_WL_SEND:	// USB/wireless send / USB/無線送信 
			case CMN_QUE_KIND_UART_SEND: 	// UART send / UART送信
			case CMN_QUE_KIND_UART_RECV: 	// UART receive / UART受信
			case CMN_QUE_KIND_WL_RECV:	 	// Wireless receive / 無線受信 		
				puchr = (UCHAR*)pstQue->pBuf;
				memcpy(pData, &puchr[pstQue->head], sizeof(UCHAR));
				break; 
			case CMN_QUE_KIND_I2C_REQ:   	// I2C master send/receive / I2Cマスタ送信/受信
				pstI2cReq = (ST_I2C_REQ*)pstQue->pBuf;
				memcpy(pData, &pstI2cReq[pstQue->head], sizeof(ST_I2C_REQ));
				break;
			default:
				// Unreachable / ここに来ない
				break;				
		}	
		pstQue->head = (pstQue->head + 1) % pstQue->max;
		bRet = true;
	}

	if (bSpinLock) {
		CMN_ExitSpinLock(); // Release spin lock / スピンロックを解放
	}

    return bRet;
}

// Acquire spin lock / スピンロックを獲得
// Spin lock is used to disable interrupts while ensuring mutual exclusion between CPUs. / スピンロックはCPU間排他を行いつつ割り込みを禁止にする場合に使用する。
// Use mutex if only mutual exclusion between CPUs is needed. / CPU間排他だけならミューテックスを使用すること。
// Definitions of critical_section (spin lock) and mutex in Pico are as follows. / Picoのcritical_section(spin lock)とmutexの定義は下記。
// https://www.raspberrypi.com/documentation/pico-sdk/high_level.html#pico_sync
// critical_section(spin lock): / critical_section(スピンロック):
// Critical Section API for short-lived mutual exclusion safe for IRQ and multi-core. / IRQとマルチコアに安全な短時間の相互排他のためのCritical Section API。
// mutex: / mutex:
// Mutex API for non IRQ mutual exclusion between cores. / コア間の非IRQ相互排他のためのMutex API。
void CMN_EntrySpinLock()
{
	critical_section_enter_blocking(&f_stSpinLock);
}

// Release spin lock / スピンロックを解放
void CMN_ExitSpinLock()
{
	critical_section_exit(&f_stSpinLock);
}

// Execute checksum validation / チェックサム検査を実行
bool CMN_Checksum(PVOID pBuf, USHORT expect, ULONG size)
{
	bool bRet;
	
	bRet = (CMN_CalcChecksum(pBuf, size) == expect) ? true : false;
	return bRet;
}

// Calculate checksum / チェックサムを計算
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

// Set FW error / FWエラーを設定
void CMN_SetErrorBits(ULONG errorBits, bool bSpinLock)
{
	if (errorBits != 0) {
		if (bSpinLock) {
			CMN_EntrySpinLock();
		}

		f_errorBits |= errorBits; // OR operation is not atomic, so mutual exclusion is required / OR演算はアトミックではないので排他制御する

		if (bSpinLock) {
			CMN_ExitSpinLock();
		}
	}
}

// Get FW error / FWエラーを取得
ULONG CMN_GetFwErrorBits()
{
	return f_errorBits;;
}

// Clear FW error / FWエラーをクリア
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

// Reboot immediately by WDT timeout using watchdog_enable() / watchdog_enable()を使用して即WDTタイムアウトで再起動する
// Reason for not using the normal method of executing watchdog_enable() at boot and clearing WDT with watchdog_update() in the main loop: / 普通に、起動時にwatchdog_enable()を実行し、メインループでwatchdog_update()でWDTクリアする方法をしない理由:
// With the normal method above, writing a uf2 file by dragging it on the PC causes watchdog_enable_caused_reboot() to return true for some reason. / 上記の普通の方法の場合、PCでuf2ファイルをドラッグで書き込むと、なぜかwatchdog_enable_caused_reboot()がtrueを返すため
void CMN_WdtEnableReboot()
{
    watchdog_enable(1, true);
    while (1) {
        tight_loop_contents();
    }		
}

// Reboot immediately by WDT timeout without using watchdog_enable() / watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
void CMN_WdtRebootWithoutEnable()
{
    watchdog_reboot(0, 0, 1);
    while (1) {
        tight_loop_contents();
    }   	
}

// Initialize common library / 共通ライブラリを初期化
void CMN_Init()
{
	// [Initialize variables] / [変数を初期化]
	critical_section_init(&f_stSpinLock);

	f_astQue[CMN_QUE_KIND_USB_WL_SEND].pBuf = (PVOID)f_aQueData_usbWlSend;
	f_astQue[CMN_QUE_KIND_UART_SEND].pBuf   = (PVOID)f_aQueData_uartSend;
	f_astQue[CMN_QUE_KIND_UART_RECV].pBuf   = (PVOID)f_aQueData_uartRecv;
	f_astQue[CMN_QUE_KIND_I2C_REQ].pBuf     = (PVOID)f_astQueData_i2cReq;
	f_astQue[CMN_QUE_KIND_WL_RECV].pBuf     = (PVOID)f_aQueData_wlRecv;

	f_astQue[CMN_QUE_KIND_USB_WL_SEND].max = CMN_QUE_DATA_MAX_USB_WL_SEND;
	f_astQue[CMN_QUE_KIND_UART_SEND].max   = CMN_QUE_DATA_MAX_UART_SEND;
	f_astQue[CMN_QUE_KIND_UART_RECV].max   = CMN_QUE_DATA_MAX_UART_RECV;
	f_astQue[CMN_QUE_KIND_I2C_REQ].max     = CMN_QUE_DATA_MAX_I2C_REQ;
	f_astQue[CMN_QUE_KIND_WL_RECV].max     = CMN_QUE_DATA_MAX_WL_RECV;
}