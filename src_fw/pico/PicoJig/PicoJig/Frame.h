#ifndef FRAME_H
#define FRAME_H

// [define]
#define FRM_DATA_MAX_SIZE 300 // フレーム中のデータ部のバッファのサイズ。余裕を含んでいる。

// [列挙体]
// フレームのヘッダ
typedef enum _E_FRM_HEADER
{
    FRM_HEADER_REQ = 0xA0,      // 要求
    FRM_HEADER_RES,             // 応答
    FRM_HEADER_NOTIFY_UART_RECV // 通知(UART受信)
} E_FRM_HEADER;

// コマンド
typedef enum _E_FRM_CMD {
    CMD_GET_FW_INFO = 0x0001,       // FW情報取得 
    CMD_SET_GPIO_CONFIG,            // GPIO設定変更  
    CMD_GET_GPIO_CONFIG,            // GPIO設定取得 
    CMD_GET_GPIO,                   // GPIO入力                      
    CMD_OUT_GPIO,                   // GPIO出力
    CMD_GET_ADC,                    // ADC・温度入力
    CMD_SET_UART_CONFIG,            // UART設定変更
    CMD_GET_UART_CONFIG,            // UART設定取得 
    CMD_SEND_UART,                  // UART送信
    CMD_SET_SPI_CONFIG,             // SPI設定変更
    CMD_GET_SPI_CONFIG,             // SPI設定取得
    CMD_SENDRECV_SPI,               // SPIマスタ送受信 
    CMD_SET_I2C_CONFIG,             // I2C設定変更
    CMD_GET_I2C_CONFIG,             // I2C設定取得 
    CMD_SEND_I2C,                   // I2Cマスタ送信
    CMD_RECV_I2C,                   // I2Cマスタ受信
    CMD_START_PWM,                  // PWM開始  
    CMD_STOP_PWM,                   // PWM停止
    CMD_GET_FW_ERR,                 // FWエラー取得  
    CMD_CLEAR_FW_ERR,               // FWエラークリア 
    CMD_ERASE_FLASH,                // FLASH消去
    CMD_SET_NW_CONFIG,              // ネットワーク設定変更
    CMD_GET_NW_CONFIG               // ネットワーク設定取得
} E_FRM_CMD;

// フレーム中のエラーコード
typedef enum _E_FRM_ERR
{
    FRM_ERR_SUCCESS = 0x0000,    // 成功
    FRM_ERR_DATA_SIZE,           // データサイズが不正  
    FRM_ERR_PARAM,               // 引数が不正
    FRM_ERR_BUF_NOT_ENOUGH,      // バッファに空きがない
    FRM_ERR_I2C_NO_DEVICE        // I2C:address not acknowledged, or, no device present. 
} E_FRM_ERR;

// 回線の種類
typedef enum _E_FRM_LINE {
    E_FRM_LINE_USB = 0,     // USB 
    E_FRM_LINE_TCP_SERVER,  // TCPサーバー
    E_FRM_LINE_NUM
} E_FRM_LINE;

#pragma pack(1)

// [構造体]

// 要求フレーム
typedef struct _ST_FRM_REQ_FRAME 
{
    UCHAR   header;                     // ヘッダ(1byte)
    USHORT  seqNo;                      // シーケンス番号(2byte)
    USHORT  cmd;                        // コマンド(2byte)
    USHORT  dataSize;                   // データサイズ(2byte)
    UCHAR   aData[FRM_DATA_MAX_SIZE];   // データ
    USHORT  checksum;                   // チェックサム(2byte)
} ST_FRM_REQ_FRAME;

// 応答フレーム
typedef struct _ST_FRM_RES_FRAME 
{
    UCHAR   header;                     // ヘッダ(1byte)
    USHORT  seqNo;                      // シーケンス番号(2byte)
    USHORT  cmd;                        // コマンド(2byte)
    USHORT  errCode;                    // エラーコード(2byte) 
    USHORT  dataSize;                   // データサイズ(2byte)
    UCHAR   aData[FRM_DATA_MAX_SIZE];   // データ
    USHORT  checksum;                   // チェックサム(2byte)
} ST_FRM_RES_FRAME;

// 通知フレーム
typedef struct _ST_FRM_NOTIFY_FRAME
{
    UCHAR  header;                      // ヘッダ(1byte)
    USHORT dataSize;                    // データサイズ(2byte)
    UCHAR  aData[FRM_DATA_MAX_SIZE];    // データ
    USHORT checksum;                    // チェックサム(2byte)    
} ST_FRM_NOTIFY_FRAME;

// USB/無線の受信データ情報
typedef struct _ST_FRM_RECV_DATA_INFO {
    ULONG reqFrmSize ; 		    // 要求フレームの受信済みサイズ
    ULONG recved_dataSize;	    // データサイズ部の受信済みサイズ
    ULONG recved_checksum; 	    // チェックサム部の受信済みサイズ
    ST_FRM_REQ_FRAME stReqFrm; 	// 要求フレーム 
    UCHAR *p;		            // 要求フレームデータ格納先ポインタ
} ST_FRM_RECV_DATA_INFO;

#pragma pack()

// [関数プロトタイプ宣言]
void FRM_Init();
void FRM_SendMain();
void FRM_RecvMain();
void FRM_MakeAndSendResFrm(USHORT seqNo, USHORT cmd, USHORT errCode, USHORT dataSize, PVOID pBuf);
void FRM_MakeAndSendNotifyFrm(UCHAR header, USHORT dataSize, PVOID pBuf);

#endif