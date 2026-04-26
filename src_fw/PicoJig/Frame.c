// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義]
#define FRM_SEND_BUF_SIZE CMN_QUE_DATA_MAX_USB_WL_SEND // Size of USB/wireless send buffer / USB/無線送信バッファのサイズ

// [File scope variables] / [ファイルスコープ変数]
static ST_FRM_RECV_DATA_INFO f_astRecvDataInf[E_FRM_LINE_NUM] = {0}; // USB/wireless receive data info / USB/無線の受信データ情報
static UCHAR f_aSendData[FRM_SEND_BUF_SIZE] = {0}; // USB/wireless send buffer / USB/無線送信バッファ

// [Function prototype declarations] / [関数プロトタイプ宣言] 
static ST_FRM_REQ_FRAME* FRM_RecvReqFrame(ULONG line);
static void FRM_ReqToSend(PVOID pBuf, ULONG size);
static bool FRM_IsConnected(ULONG line);

// Extract USB/wireless receive data ⇒ Parse and execute command / USB/無線受信データ取り出し⇒コマンド解析・実行
void FRM_RecvMain()
{
	ULONG iLine;
	ST_FRM_REQ_FRAME *pstReqFrm = NULL; // Request frame / 要求フレーム

	for (iLine = 0; iLine < E_FRM_LINE_NUM; iLine++)
	{
		// Create request frame from USB/wireless receive data / USB/無線受信データから要求フレームを作成する
		pstReqFrm = FRM_RecvReqFrame(iLine);
		if (pstReqFrm != NULL) { // If extraction of request frame is complete / 要求フレームの抽出が完了した場合
			// Parse and execute command / コマンドを解析・実行
			CMD_ExecReqCmd(pstReqFrm);
		}
	}
}

// Create request frame from USB/wireless receive data / USB/無線受信データから要求フレームを作成する
static ST_FRM_REQ_FRAME* FRM_RecvReqFrame(ULONG line)
{
	int ret;
	UCHAR data = 0; 					// Receive data (1 byte) / 受信データ(1byte)
	ULONG reqFrmSize = 0; 			    // Size of request frame (excluding checksum) / 要求フレームのサイズ(チェックサム除く)
	bool isConnected;
	ST_FRM_REQ_FRAME *pstReqFrm = NULL; // Request frame that has been extracted (NULL if incomplete) / 抽出が完了した要求フレーム(未完了の場合はNULL)
	ST_FRM_RECV_DATA_INFO *pstRecv = &f_astRecvDataInf[line];

	isConnected = FRM_IsConnected(line);

	// [Receive timeout check for request frame] / [要求フレームの受信タイムアウト判定]
	if (pstRecv->reqFrmSize > 0) { // If header of request frame has been received / 要求フレームのヘッダは受信済みの場合
		if (TMR_IsRecvTimeout(line) // If the following timeout occurs: If the end of the request frame is not received within TMR_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 右記のタイムアウトが発生した場合:要求フレームのヘッダを受信後、TMR_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
		  || (!isConnected)) { // If not connected / 未接続の場合 
			pstRecv->reqFrmSize = 0; // Discard frame / フレーム破棄
		}
	}	

	// [Extract 1 byte of USB/wireless receive data] / [USB/無線の受信データ1byte取り出し]
	switch (line) // Line type / 回線の種類
	{
		case E_FRM_LINE_USB: // USB / USB
			// Extract 1 byte of USB receive data / USB受信データ1byte取り出し
			ret = getchar_timeout_us(0); 
			if (PICO_ERROR_TIMEOUT == ret) { // If there is no USB receive data / USB受信データが無い場合
				return pstReqFrm; // Return NULL / NULLを返す
			}
			data = (UCHAR)ret;
			break;
		case E_FRM_LINE_TCP_SERVER: // TCP server / TCPサーバー
			// Dequeue 1 byte of wireless receive data / 無線受信データの1byteのデキュー
			if (!CMN_Dequeue(CMN_QUE_KIND_WL_RECV, &data, true)) { 
				return pstReqFrm; // Return NULL / NULLを返す
			}
			break;
		default:
			break;
	}	

	// [Create request frame from USB/wireless receive data] / [USB/無線の受信データから要求フレームを作成する]

	// Header / ヘッダ
	if (pstRecv->reqFrmSize == offsetof(ST_FRM_REQ_FRAME, header)) {
		if (FRM_HEADER_REQ == data) { 
			// If request header / 要求ヘッダの場合

			pstRecv->recved_dataSize = 0;	 // Initialize received size of data size part / データサイズ部の受信済みサイズを初期化
			pstRecv->recved_checksum = 0; 	 // Initialize received size of checksum part / チェックサム部の受信済みサイズを初期化
			pstRecv->p = (UCHAR*)&pstRecv->stReqFrm; // Initialize pointer to store request frame data / 要求フレームデータ格納先ポインタを初期化	
			*pstRecv->p++ = data;			 // Store header / ヘッダを格納
			pstRecv->reqFrmSize++;			 // Received size of request frame + 1 / 要求フレームの受信済みサイズ+1

			// Clear timer count for the following: If the end of the request frame is not received within TMR_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 右記のタイマカウントをクリア:要求フレームのヘッダを受信後、TMR_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
			TMR_ClearRecvTimeout(line);	
		}
		else {
			// If not request header / 要求ヘッダではない場合	

			pstRecv->reqFrmSize = 0; // Discard frame / フレーム破棄
		}		
	}
	// Sequence number / シーケンス番号
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, seqNo) + sizeof(pstRecv->stReqFrm.seqNo)) { 
		*pstRecv->p++ = data;  // Store sequence number / シーケンス番号を格納
		pstRecv->reqFrmSize++; // Received size of request frame + 1 / 要求フレームの受信済みサイズ+1
	}
	// Command / コマンド
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, cmd) + sizeof(pstRecv->stReqFrm.cmd)) { 
		*pstRecv->p++ = data;  // Store command / コマンドを格納
		pstRecv->reqFrmSize++; // Received size of request frame + 1 / 要求フレームの受信済みサイズ+1					
	}
	// Data size / データサイズ
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize)) { 	
		*pstRecv->p++ = data;  	 // Store data size / データサイズを格納
		pstRecv->reqFrmSize++; 	 // Received size of request frame + 1 / 要求フレームの受信済みサイズ+1	
		pstRecv->recved_dataSize++; // Received size of data size part + 1 / データサイズ部の受信済みサイズ+1
		if (pstRecv->recved_dataSize == sizeof(pstRecv->stReqFrm.dataSize)) { // If reception of data size part is complete / データサイズ部の受信が完了した場合
			if (pstRecv->stReqFrm.dataSize > FRM_DATA_MAX_SIZE) { // If data size exceeds maximum value / データサイズが最大値を超えている場合
				pstRecv->reqFrmSize = 0; // Discard frame / フレーム破棄
			}			
		}				
	}
	// Data part / データ部
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize) + pstRecv->stReqFrm.dataSize) { 
		*pstRecv->p++ = data;  // Store data part / データ部を格納
		pstRecv->reqFrmSize++;	// Received size of request frame + 1 / 要求フレームの受信済みサイズ+1	
	}
	// Checksum / チェックサム
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize) + pstRecv->stReqFrm.dataSize + sizeof(pstRecv->stReqFrm.checksum)) {
		// Since the size of the data part aData[] member is fixed to FRM_DATA_MAX_SIZE, variables like pstRecv->recved_checksum and the following processing are required / データ部:aData[]メンバのサイズがFRM_DATA_MAX_SIZE固定のため、pstRecv->recved_checksumのような変数や下記の処理が必要 				
		if (!pstRecv->recved_checksum) { 
			pstRecv->p = (UCHAR*)&pstRecv->stReqFrm.checksum; // Pointer points to address of checksum part / 格納先ポインタはチェックサム部のアドレスを指す
		}
		*pstRecv->p++ = data;  	 // Store checksum / チェックサムを格納
		pstRecv->reqFrmSize++; 	 // Received size of request frame + 1 / 要求フレームの受信済みサイズ+1
		pstRecv->recved_checksum++; // Received size of checksum part + 1 / チェックサム部の受信済みサイズ+1
	}		
	else {
		// No processing / 無処理
	}

	if (pstRecv->reqFrmSize >= offsetof(ST_FRM_REQ_FRAME, dataSize) 
		+ sizeof(pstRecv->stReqFrm.dataSize) 
		+ pstRecv->stReqFrm.dataSize + sizeof(pstRecv->stReqFrm.checksum)) {
		// If extraction of request frame is complete / 要求フレームの抽出が完了した場合	

		pstRecv->reqFrmSize = 0; // Initialize received size of request frame / 要求フレームの受信済みサイズを初期化

		// [Checksum validation] / [チェックサム検査]
		// Size of request frame (excluding checksum) / 要求フレームのサイズ(チェックサム除く)を計算
		reqFrmSize = offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize) + pstRecv->stReqFrm.dataSize; 
		// Execute checksum validation / チェックサム検査を実行
		if (CMN_Checksum(&pstRecv->stReqFrm, pstRecv->stReqFrm.checksum, reqFrmSize)) {
			// If checksum validation passes / チェックサム検査に合格した場合
			pstReqFrm = &pstRecv->stReqFrm; // Set pointer of request frame to return value / 戻り値に要求フレームのポインタを設定
		}
	}

	return pstReqFrm;
}

// Extract send frame ⇒ USB/wireless send / 送信フレーム取り出し⇒USB/無線送信
void FRM_SendMain()
{
	UCHAR data;
	ULONG i;
	ULONG size; // USB/wireless send size / USB/無線送信サイズ

	for (i = 0; i < sizeof(f_aSendData); i++) { // Repeat for the size of USB/wireless send buffer / USB/無線送信バッファのサイズ分繰り返す
		// Dequeue 1 byte of USB/wireless send data / USB/無線送信データ1byteのデキュー
		if (CMN_Dequeue(CMN_QUE_KIND_USB_WL_SEND, &data, true)) { 
			// Store USB/wireless send request data in USB/wireless send buffer / USB/無線送信バッファにUSB/無線送信要求データを格納
			f_aSendData[i] = data; 
		}
		else {
			break; // Queue is empty / キューが空
		}
	}
	size = i; 

	if (size > 0) {
		// USB priority / USB優先
		if (stdio_usb_connected()) { // USB connected / USB接続済み
			// USB send / USB送信 
			for (i = 0; i < size; i++) 
			{
				putchar_raw(f_aSendData[i]);
			}				
		}
#ifdef MY_BOARD_PICO_W			
		else if (tcp_server_is_connected()) { // TCP connected / TCP接続済み
			// TCP send / TCP送信
			if (ERR_OK != tcp_server_send_data(f_aSendData, size)) {
				// Set FW error / FWエラーを設定
				CMN_SetErrorBits(CMN_ERR_BIT_WL_SEND_ERR, true);				
			}
		}
#endif
		else {
			// No processing / 無処理
		}
	}
}

// USB/wireless send of response frame / 応答フレームのUSB/無線送信
void FRM_MakeAndSendResFrm(USHORT seqNo, USHORT cmd, USHORT errCode, USHORT dataSize, PVOID pBuf)
{
	ULONG frmSize;        			// Size of response frame (excluding checksum) / 応答フレームのサイズ(チェックサム除く)
	UCHAR* pDataAry = (UCHAR*)pBuf;	// Data part of response frame / 応答フレームのデータ部
	ST_FRM_RES_FRAME stResFrm = {0}; // Response frame / 応答フレーム

	// Create response frame / 応答フレームを作成
	stResFrm.header   = FRM_HEADER_RES;	// Header / ヘッダ
	stResFrm.seqNo    = seqNo; 			// Sequence number / シーケンス番号
	stResFrm.cmd      = cmd;   			// Command / コマンド
	stResFrm.errCode  = errCode;       	// Error code / エラーコード
	if (FRM_ERR_SUCCESS != errCode) {
		dataSize = 0;
		pDataAry = NULL;
	}
	stResFrm.dataSize = dataSize;      	// Data size / データサイズ	
	// Data / データ
	if ((pDataAry != NULL) && (dataSize > 0)) { 
		memcpy(stResFrm.aData, pDataAry, dataSize);
	}
	// Size of response frame (excluding checksum) / 応答フレームのサイズ(チェックサム除く)を計算
	frmSize = offsetof(ST_FRM_RES_FRAME, dataSize) + sizeof(stResFrm.dataSize) + stResFrm.dataSize;  
	// Calculate checksum / チェックサムを計算 
	stResFrm.checksum = CMN_CalcChecksum(&stResFrm, frmSize); 
	
	// Issue USB/wireless send request / USB/無線送信要求を発行
	FRM_ReqToSend(&stResFrm, frmSize); // Header part to data part *For the data part aData[] member, only frmSize bytes are subject to transmission / ヘッダ部～データ部 ※データ部:aData[]メンバについてはfrmSize分だけが送信対象
	FRM_ReqToSend(&stResFrm.checksum, sizeof(stResFrm.checksum)); // Checksum part / チェックサム部
}

// USB/wireless send of notification frame / 通知フレームのUSB/無線送信
void FRM_MakeAndSendNotifyFrm(UCHAR header, USHORT dataSize, PVOID pBuf)
{
	UCHAR* pDataAry = (UCHAR*)pBuf; // Data part of notification frame / 通知フレームのデータ部
	ULONG frmSize; 					// Size of notification frame (excluding checksum) / 通知フレームのサイズ(チェックサム部除く)
	ST_FRM_NOTIFY_FRAME stNtyFrm = {0}; // Notification frame / 通知フレーム
	
	// Create notification frame / 通知フレームを作成
	stNtyFrm.header   = header; 			 	// Header / ヘッダ
	stNtyFrm.dataSize = dataSize; 			 	// Data size / データサイズ
	memcpy(stNtyFrm.aData, pDataAry, dataSize); // Data / データ
	// Size of notification frame (excluding checksum) / 通知フレームのサイズ(チェックサム除く)を計算
	frmSize = sizeof(stNtyFrm.header) + sizeof(stNtyFrm.dataSize) + stNtyFrm.dataSize;
	// Calculate checksum / チェックサムを計算 
	stNtyFrm.checksum = CMN_CalcChecksum(&stNtyFrm, frmSize);

	// Issue USB/wireless send request / USB/無線送信要求を発行
	FRM_ReqToSend(&stNtyFrm, frmSize); // Header part to data part *For the data part aData[] member, only frmSize bytes are subject to transmission / ヘッダ部～データ部 ※データ部:aData[]メンバについてはfrmSize分だけが送信対象
	FRM_ReqToSend(&stNtyFrm.checksum, sizeof(stNtyFrm.checksum)); // Checksum part / チェックサム部	
}

// Issue USB/wireless send request / USB/無線送信要求を発行
static void FRM_ReqToSend(PVOID pBuf, ULONG size)
{
	UCHAR* pDataAry = (UCHAR*)pBuf;
	ULONG i;

	for (i = 0; i < size; i++) {
		// Enqueue USB/wireless send data / USB/無線送信データをエンキュー
		if (!CMN_Enqueue(CMN_QUE_KIND_USB_WL_SEND, &pDataAry[i], true)) { 
			break; // Queue is full / キューが満杯
		}
	} 	
}

// Get USB/wireless connection status / USB/無線の接続状態を取得
static bool FRM_IsConnected(ULONG line)
{
	bool isConnected = false;

	switch (line)
	{
		case E_FRM_LINE_USB: // USB / USB
			isConnected = stdio_usb_connected();
			break;
#ifdef MY_BOARD_PICO_W				
		case E_FRM_LINE_TCP_SERVER: // TCP server / TCPサーバー
			isConnected = tcp_server_is_connected();
			break;
#endif
		default:
			break;
	}	

	return isConnected;
}

// Initialize USB/wireless common processing / USB/無線共通処理を初期化
void FRM_Init()
{
	ULONG i;

	// Initialize variables / 変数を初期化
	for (i = 0; i < E_FRM_LINE_NUM; i++) {
		f_astRecvDataInf[i].p = (UCHAR*)&(f_astRecvDataInf[i].stReqFrm);
	}
}
