#ifndef SPI_H
#define SPI_H

#include "Common.h"

// [define]
#define SPI_DATA_MAX_SIZE 256 // FRM_DATA_MAX_SIZEより十分小さくすること

#pragma pack(1)

// [構造体]
// SPI通信設定
typedef struct _ST_SPI_CONFIG {
    ULONG frequency; // クロック周波数(Hz)
    UCHAR dataBits;  // データビット長
    UCHAR polarity;  // 極性
    UCHAR phase;     // 位相
    UCHAR order;     // バイトオーダー
} ST_SPI_CONFIG;

#pragma pack()

void SPI_SendRecv(PVOID pSendBuf, PVOID pRecvBuf, ULONG sendRecvSize);
void SPI_SetDefault(ST_SPI_CONFIG *pstConfig);
void SPI_Init(ST_SPI_CONFIG *pstConfig);

#endif