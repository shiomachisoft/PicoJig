#include "Common.h"

// [define]
#define FRM_SEND_BUF_SIZE CMN_QUE_DATA_MAX_USB_WL_SEND // USB/無線送信バッファのサイズ

// [ファイルスコープ変数]
static ST_FRM_RECV_DATA_INFO f_astRecvDataInf[E_FRM_LINE_NUM] = {0}; // USB/無線の受信データ情報
static UCHAR f_aSendData[FRM_SEND_BUF_SIZE] = {0}; // USB/無線送信バッファ

// [関数プロトタイプ宣言]
extern int stdio_usb_in_chars(char *buf, int length);
extern void stdio_usb_out_chars(const char *buf, int length); 
static ST_FRM_REQ_FRAME* FRM_RecvReqFrame(ULONG line);
static void FRM_ReqToSend(PVOID pBuf, ULONG size);
static bool FRM_IsConnected(ULONG line);

// USB/無線受信データ取り出し⇒コマンド解析・実行
void FRM_RecvMain()
{
	ULONG iLine;
    ST_FRM_REQ_FRAME *pstReqFrm = NULL; // 要求フレーム

	for (iLine = 0; iLine < E_FRM_LINE_NUM; iLine++)
	{
		// USB/無線受信データから要求フレームを作成する
		pstReqFrm = FRM_RecvReqFrame(iLine);
		if (pstReqFrm != NULL) { // 要求フレームの抽出が完了した場合
			// コマンドを解析・実行
			CMD_ExecReqCmd(pstReqFrm);
		}
	}
}

// USB/無線受信データから要求フレームを作成する
static ST_FRM_REQ_FRAME* FRM_RecvReqFrame(ULONG line)
{
	UCHAR data = 0; 					// 受信データ(1byte)
	ULONG reqFrmSize = 0; 			    // 要求フレームのサイズ(チェックサム除く)
	bool isConnected;
	ST_FRM_REQ_FRAME *pstReqFrm = NULL; // 抽出が完了した要求フレーム(未完了の場合はNULL)
	ST_FRM_RECV_DATA_INFO *pstRecv = &f_astRecvDataInf[line];

	// [要求フレームの受信タイムアウト判定]
	isConnected = FRM_IsConnected(line);
	if (pstRecv->reqFrmSize > 0) { // 要求フレームのヘッダは受信済みの場合
		if (TIMER_IsRecvTimeout(line) // 右記のタイムアウトが発生した場合:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
		  || (!isConnected)) { // 未接続の場合 
			pstRecv->reqFrmSize = 0; // フレーム破棄
		}
	}	

	// [USB/無線の受信データ1byte取り出し]
	switch (line) // 回線の種類
	{
		case E_FRM_LINE_USB: // USB
			// USB受信データ1byte取り出し
			if (0 >= stdio_usb_in_chars((char*)&data, sizeof(UCHAR))) { // USB受信データが無い場合
				return pstReqFrm; // NULLを返す
			}
			break;
		case E_FRM_LINE_TCP_SERVER: // TCPサーバー
			if (!CMN_Dequeue(CMN_QUE_KIND_WL_RECV, &data, sizeof(UCHAR), true)) { // CPUコア1のエンキューとCPUコア0のデキューを排他する
				return pstReqFrm; // NULLを返す
			}
			break;
		default:
			break;
	}	

	// [USB/無線の受信データから要求フレームを作成する]

	// ヘッダ
	if (pstRecv->reqFrmSize == offsetof(ST_FRM_REQ_FRAME, header)) {
		if (FRM_HEADER_REQ == data) { 
			// 要求ヘッダの場合

			pstRecv->recved_dataSize = 0;	 // データイサイズ部の受信済みサイズを初期化
			pstRecv->recved_checksum = 0; 	 // チェックサム部の受信済みサイズを初期化
			pstRecv->p = (UCHAR*)&pstRecv->stReqFrm; // 要求フレームデータ格納先ポインタを初期化	
			*pstRecv->p++ = data;			 // ヘッダを格納
			pstRecv->reqFrmSize++;			 // 要求フレームの受信済みサイズ+1

			// 右記のタイマカウントをクリア:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
			TIMER_ClearRecvTimeout(line);	
		}
		else {
			// 要求ヘッダではない場合	

			pstRecv->reqFrmSize = 0; // フレーム破棄
		}		
	}
	// シーケンス番号
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, seqNo) + sizeof(pstRecv->stReqFrm.seqNo)) { 
		*pstRecv->p++ = data;  // シーケンス番号を格納
		pstRecv->reqFrmSize++; // 要求フレームの受信済みサイズ+1
	}
	// コマンド
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, cmd) + sizeof(pstRecv->stReqFrm.cmd)) { 
		*pstRecv->p++ = data;  // コマンドを格納
		pstRecv->reqFrmSize++; // 要求フレームの受信済みサイズ+1					
	}
	// データサイズ
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize)) { 	
		*pstRecv->p++ = data;  	 // データサイズを格納
		pstRecv->reqFrmSize++; 	 // 要求フレームの受信済みサイズ+1	
		pstRecv->recved_dataSize++; // データサイズ部の受信済みサイズ+1
		if (pstRecv->recved_dataSize == sizeof(pstRecv->stReqFrm.dataSize)) { // データサイズ部の受信が完了した場合
			if (pstRecv->stReqFrm.dataSize > FRM_DATA_MAX_SIZE) { // データサイズが最大値を超えている場合
				pstRecv->reqFrmSize = 0; // フレーム破棄
			}			
		}				
	}
	// データ部
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize) + pstRecv->stReqFrm.dataSize) { 
		*pstRecv->p++ = data;  // データ部を格納
		pstRecv->reqFrmSize++;	// 要求フレームの受信済みサイズ+1	
	}
	// チェックサム
	else if (pstRecv->reqFrmSize < offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize) + pstRecv->stReqFrm.dataSize + sizeof(pstRecv->stReqFrm.checksum)) {
		// データ部:aData[]メンバのサイズがFRM_DATA_MAX_SIZE固定のため、pstRecv->recved_checksumのような変数や下記の処理が必要 				
		if (!pstRecv->recved_checksum) { 
			pstRecv->p = (UCHAR*)&pstRecv->stReqFrm.checksum; // 格納先ポインタはチェックサム部のアドレスを指す
		}
		*pstRecv->p++ = data;  	 // チェックサムを格納
		pstRecv->reqFrmSize++; 	 // 要求フレームの受信済みサイズ+1
		pstRecv->recved_checksum++; // チェックサム部の受信済みサイズ+1
	}		
	else {
		// 無処理
	}

	if (pstRecv->reqFrmSize >= offsetof(ST_FRM_REQ_FRAME, dataSize) 
		+ sizeof(pstRecv->stReqFrm.dataSize) 
		+ pstRecv->stReqFrm.dataSize + sizeof(pstRecv->stReqFrm.checksum)) {
		// 要求フレームの抽出が完成した場合	

		pstRecv->reqFrmSize = 0; // 要求フレームの受信済みサイズを初期化

		// [チェックサム検査]
		// 要求フレームのサイズ(チェックサム除く)を計算
		reqFrmSize = offsetof(ST_FRM_REQ_FRAME, dataSize) + sizeof(pstRecv->stReqFrm.dataSize) + pstRecv->stReqFrm.dataSize; 
		// チェックサム検査を実行
		if (CMN_Checksum(&pstRecv->stReqFrm, pstRecv->stReqFrm.checksum, reqFrmSize)) {
			// チェックサム検査に合格した場合
			pstReqFrm = &pstRecv->stReqFrm; // 戻り値に要求フレームのポインタを設定
		}
	}

	return pstReqFrm;
}

// 送信フレーム取り出し⇒USB/無線送信
void FRM_SendMain()
{
	UCHAR data;
	ULONG i;
	ULONG size; // USB/無線送信サイズ

	for (i = 0; i < sizeof(f_aSendData); i++) { // USB/無線送信バッファのサイズ分繰り返す
		// USB/無線送信データ1byteのデキュー
		if (CMN_Dequeue(CMN_QUE_KIND_USB_WL_SEND, &data, sizeof(UCHAR), true)) { // true:CPUコア0のエンキューとCPUコア1のデキューを排他する
			// USB/無線送信バッファにUSB/無線送信要求データを格納
			f_aSendData[i] = data; 
		}
		else {
			break; // キューが空
		}
	}
	size = i; // USB送信サイズ


	if (size > 0) {
		// USB優先
		if (stdio_usb_connected()) { // USB接続済み
			// USB送信 
			stdio_usb_out_chars((const char*)f_aSendData, size);
		}
		else if (tcp_server_is_connected()) { // TCP接続済み
			// TCP送信
			tcp_server_send_data(f_aSendData, size);
		}
		else {
			// 無処理
		}
	}
}

// 応答フレームのUSB/無線送信
void FRM_MakeAndSendResFrm(USHORT seqNo, USHORT cmd, USHORT errCode, USHORT dataSize, PVOID pBuf)
{
	ULONG frmSize;        			// 応答フレームのサイズ(チェックサム除く)
	UCHAR* pDataAry = (UCHAR*)pBuf;	// 応答フレームのデータ部
	ST_FRM_RES_FRAME stResFrm; 		// 応答フレーム

	// 応答フレームを作成
	stResFrm.header   = FRM_HEADER_RES;	// ヘッダ
	stResFrm.seqNo    = seqNo; 			// シーケンス番号
	stResFrm.cmd      = cmd;   			// コマンド
	stResFrm.errCode  = errCode;       	// エラーコード
	stResFrm.dataSize = dataSize;      	// データサイズ	
	// データ
	if ((FRM_ERR_SUCCESS == errCode) && (pDataAry != NULL) && (dataSize > 0)) { 
		memcpy(stResFrm.aData, pDataAry, dataSize);
	}
	// 応答フレームのサイズ(チェックサム除く)を計算
	frmSize = offsetof(ST_FRM_RES_FRAME, dataSize) + sizeof(stResFrm.dataSize) + stResFrm.dataSize;  
	// チェックサムを計算 
	stResFrm.checksum = CMN_CalcChecksum(&stResFrm, frmSize); 
	
	// USB/無線送信要求を発行
	FRM_ReqToSend(&stResFrm, frmSize); // ヘッダ部～データ部 ※データ部:aData[]メンバは全領域送信するわけではない
	FRM_ReqToSend(&stResFrm.checksum, sizeof(stResFrm.checksum)); // チェックサム部
}

// 通知フレームのUSB/無線送信
void FRM_MakeAndSendNotifyFrm(UCHAR header, USHORT dataSize, PVOID pBuf)
{
	UCHAR* pDataAry = (UCHAR*)pBuf; // 通知フレームのデータ部
	ULONG frmSize; 					// 通知フレームのサイズ(チェックサム部除く)
	ST_FRM_NOTIFY_FRAME stNtyFrm; 	// 通知フレーム
	
	// 通知フレームを作成
	stNtyFrm.header   = header; 			 	// ヘッダ
	stNtyFrm.dataSize = dataSize; 			 	// データサイズ
	memcpy(stNtyFrm.aData, pDataAry, dataSize); // データ
	// 通知フレームのサイズ(チェックサム除く)を計算
	frmSize = sizeof(stNtyFrm.header) + sizeof(stNtyFrm.dataSize) + stNtyFrm.dataSize;
	// チェックサムを計算 
	stNtyFrm.checksum = CMN_CalcChecksum(&stNtyFrm, frmSize);

	// USB/無線送信要求を発行
	FRM_ReqToSend(&stNtyFrm, frmSize); // ヘッダ部～データ部 ※データ部:aData[]メンバは全領域送信するわけではない
	FRM_ReqToSend(&stNtyFrm.checksum, sizeof(stNtyFrm.checksum));// チェックサム部	
}

// USB/無線送信要求を発行
static void FRM_ReqToSend(PVOID pBuf, ULONG size)
{
	UCHAR* pDataAry = (UCHAR*)pBuf;
	ULONG i;

	for (i = 0; i < size; i++) {
		// USB/無線送信データをエンキュー
		if (!CMN_Enqueue(CMN_QUE_KIND_USB_WL_SEND, &pDataAry[i], sizeof(UCHAR), true)) { // CPUコア0のエンキューとCPUコア1のデキューを排他する
			break; // キューが満杯
		}
	} 	
}

// USB/無線の接続状態を取得
static bool FRM_IsConnected(ULONG line)
{
	bool isConnected = false;

	switch (line)
	{
		case E_FRM_LINE_USB: // USB
			isConnected = stdio_usb_connected();
			break;
		case E_FRM_LINE_TCP_SERVER: // TCPサーバー
			isConnected = tcp_server_is_connected();
			break;
		default:
			break;
	}	

	return isConnected;
}

// USB/無線共通処理を初期化
void FRM_Init()
{
	ULONG i;

	// 変数を初期化
	for (i = 0; i < E_FRM_LINE_NUM; i++) {
		f_astRecvDataInf[i].p = (UCHAR*)&(f_astRecvDataInf[i].stReqFrm);
	}
}




