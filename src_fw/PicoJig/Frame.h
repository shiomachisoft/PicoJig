// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef FRAME_H
#define FRAME_H

#include "Common.h"

// [define] / [定義]
#define FRM_DATA_MAX_SIZE 1024 // Buffer size for the data part in the frame. Data parts for all commands must be smaller than this size. / フレーム中のデータ部のバッファのサイズ。どのコマンドのデータ部もこのサイズ以下であること。

// [Enums] / [列挙体]
// Frame header / フレームのヘッダ
typedef enum _E_FRM_HEADER {
    FRM_HEADER_REQ = 0xA0,      // Request / 要求
    FRM_HEADER_RES,             // Response / 応答
    FRM_HEADER_NOTIFY_UART_RECV // Notification (UART receive) / 通知(UART受信)
} E_FRM_HEADER;

// Commands / コマンド
typedef enum _E_FRM_CMD {
    CMD_GET_FW_INFO = 0x0001,       // Get FW info / FW情報取得 
    CMD_SET_GPIO_CONFIG,            // Set GPIO config / GPIO設定変更  
    CMD_GET_GPIO_CONFIG,            // Get GPIO config / GPIO設定取得 
    CMD_GET_GPIO,                   // Get GPIO input/output value / GPIO入出力値取得                      
    CMD_OUT_GPIO,                   // Set GPIO output / GPIO出力
    CMD_GET_ADC,                    // Get ADC/temperature / ADC・温度入力
    CMD_SET_UART_CONFIG,            // Set UART config / UART設定変更
    CMD_GET_UART_CONFIG,            // Get UART config / UART設定取得 
    CMD_SEND_UART,                  // Send UART / UART送信
    CMD_SET_SPI_CONFIG,             // Set SPI config / SPI設定変更
    CMD_GET_SPI_CONFIG,             // Get SPI config / SPI設定取得
    CMD_SENDRECV_SPI,               // SPI master send/receive / SPIマスタ送受信 
    CMD_SET_I2C_CONFIG,             // Set I2C config / I2C設定変更
    CMD_GET_I2C_CONFIG,             // Get I2C config / I2C設定取得 
    CMD_SEND_I2C,                   // I2C master send / I2Cマスタ送信
    CMD_RECV_I2C,                   // I2C master receive / I2Cマスタ受信
    CMD_START_PWM,                  // Start PWM / PWM開始  
    CMD_STOP_PWM,                   // Stop PWM / PWM停止
    CMD_GET_FW_ERR,                 // Get FW error / FWエラー取得  
    CMD_CLEAR_FW_ERR,               // Clear FW error / FWエラークリア 
    CMD_ERASE_FLASH,                // Erase FLASH / FLASH消去
    CMD_SET_NW_CONFIG,              // Set network config / ネットワーク設定変更
    CMD_GET_NW_CONFIG,              // Get network config / ネットワーク設定取得
    CMD_SET_NW_CONFIG2,             // Set network config 2 (PicoBrg only) / ネットワーク設定変更2(PicoBrg専用)
    CMD_GET_NW_CONFIG2,             // Get network config 2 (PicoBrg only) / ネットワーク設定取得2(PicoBrg専用)
    CMD_SET_NW_CONFIG3,             // Set network config 3 (PicoIot only) / ネットワーク設定変更3(PicoIot専用)
    CMD_GET_NW_CONFIG3,             // Get network config 3 (PicoIot only) / ネットワーク設定取得3(PicoIot専用)
    CMD_RESET_MCU                   // Reset MCU / マイコンリセット
} E_FRM_CMD;

// Error codes in frame / フレーム中のエラーコード
typedef enum _E_FRM_ERR {
    FRM_ERR_SUCCESS = 0x0000,    // Success / 成功
    FRM_ERR_DATA_SIZE,           // Invalid data size / データサイズが不正  
    FRM_ERR_PARAM,               // Invalid parameter / 引数が不正
    FRM_ERR_BUF_NOT_ENOUGH,      // Buffer not enough / バッファに空きがない
    FRM_ERR_I2C_NO_DEVICE        // I2C: address not acknowledged, or, no device present. / I2C:address not acknowledged, or, no device present. 
} E_FRM_ERR;

// Line type / 回線の種類
typedef enum _E_FRM_LINE {
    E_FRM_LINE_USB = 0,     // USB / USB 
    E_FRM_LINE_TCP_SERVER,  // TCP server / TCPサーバー
    E_FRM_LINE_NUM
} E_FRM_LINE;

#pragma pack(1)

// [Structs] / [構造体]
// Request frame / 要求フレーム
typedef struct _ST_FRM_REQ_FRAME {
    UCHAR   header;                     // Header (1 byte) / ヘッダ(1byte)
    USHORT  seqNo;                      // Sequence number (2 bytes) / シーケンス番号(2byte)
    USHORT  cmd;                        // Command (2 bytes) / コマンド(2byte)
    USHORT  dataSize;                   // Data size (2 bytes) / データサイズ(2byte)
    UCHAR   aData[FRM_DATA_MAX_SIZE];   // Data / データ
    USHORT  checksum;                   // Checksum (2 bytes) / チェックサム(2byte)
} ST_FRM_REQ_FRAME;

// Response frame / 応答フレーム
typedef struct _ST_FRM_RES_FRAME {
    UCHAR   header;                     // Header (1 byte) / ヘッダ(1byte)
    USHORT  seqNo;                      // Sequence number (2 bytes) / シーケンス番号(2byte)
    USHORT  cmd;                        // Command (2 bytes) / コマンド(2byte)
    USHORT  errCode;                    // Error code (2 bytes) / エラーコード(2byte) 
    USHORT  dataSize;                   // Data size (2 bytes) / データサイズ(2byte)
    UCHAR   aData[FRM_DATA_MAX_SIZE];   // Data / データ
    USHORT  checksum;                   // Checksum (2 bytes) / チェックサム(2byte)
} ST_FRM_RES_FRAME;

// Notification frame / 通知フレーム
typedef struct _ST_FRM_NOTIFY_FRAME {
    UCHAR  header;                      // Header (1 byte) / ヘッダ(1byte)
    USHORT dataSize;                    // Data size (2 bytes) / データサイズ(2byte)
    UCHAR  aData[FRM_DATA_MAX_SIZE];    // Data / データ
    USHORT checksum;                    // Checksum (2 bytes) / チェックサム(2byte)    
} ST_FRM_NOTIFY_FRAME;

#pragma pack()

// USB/wireless receive data info / USB/無線の受信データ情報
typedef struct _ST_FRM_RECV_DATA_INFO {
    ULONG reqFrmSize ; 		    // Received size of request frame / 要求フレームの受信済みサイズ
    ULONG recved_dataSize;	    // Received size of data size part / データサイズ部の受信済みサイズ
    ULONG recved_checksum; 	    // Received size of checksum part / チェックサム部の受信済みサイズ
    ST_FRM_REQ_FRAME stReqFrm; 	// Request frame / 要求フレーム 
    UCHAR *p;		            // Pointer to store request frame data / 要求フレームデータ格納先ポインタ
} ST_FRM_RECV_DATA_INFO;

// [Function prototype declarations] / [関数プロトタイプ宣言]
void FRM_Init();
void FRM_SendMain();
void FRM_RecvMain();
void FRM_MakeAndSendResFrm(USHORT seqNo, USHORT cmd, USHORT errCode, USHORT dataSize, PVOID pBuf);
void FRM_MakeAndSendNotifyFrm(UCHAR header, USHORT dataSize, PVOID pBuf);

#endif