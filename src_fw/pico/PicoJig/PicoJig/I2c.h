#ifndef I2C_H
#define I2C_H

#include "Common.h"

#define I2C_DATA_MAX_SIZE 256 // FRM_DATA_MAX_SIZEより十分小さくすること

#pragma pack(1)

// [構造体]
// I2C通信設定
typedef struct _ST_I2C_CONFIG {
    ULONG frequency; // クロック周波数(Hz)
} ST_I2C_CONFIG;

// I2C送信/受信要求
typedef struct _ST_I2C_REQ {
    USHORT seqNo;                    // シーケンス番号    
    USHORT cmd;                      // コマンド
    UCHAR  slaveAddr;                // スレーブアドレス
    USHORT dataSize;                 // 送信/受信サイズ
    UCHAR  aData[I2C_DATA_MAX_SIZE]; // 送信データ
} ST_I2C_REQ;

#pragma pack()

void I2C_Main();
void I2C_SetDefault(ST_I2C_CONFIG *pstConfig);
void I2C_Init(ST_I2C_CONFIG *pstConfig);

#endif