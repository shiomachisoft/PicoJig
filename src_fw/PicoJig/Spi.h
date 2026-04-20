// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef SPI_H
#define SPI_H

#include "Common.h"

// [define] / [定義]
#define SPI_DATA_MAX_SIZE 256 // Make this sufficiently smaller than FRM_DATA_MAX_SIZE / FRM_DATA_MAX_SIZEより十分小さくすること

#pragma pack(1)

// [Structs] / [構造体]
// SPI config / SPI通信設定
typedef struct _ST_SPI_CONFIG {
    ULONG frequency; // Clock frequency (Hz) / クロック周波数(Hz)
    UCHAR dataBits;  // Data bit length / データビット長
    UCHAR polarity;  // Polarity / 極性
    UCHAR phase;     // Phase / 位相
    UCHAR order;     // bit order / ビットオーダー
} ST_SPI_CONFIG;

#pragma pack()

void SPI_SendRecv(PVOID pSendBuf, PVOID pRecvBuf, ULONG sendRecvSize);
void SPI_SetDefault(ST_SPI_CONFIG *pstConfig);
void SPI_Init(ST_SPI_CONFIG *pstConfig);

#endif