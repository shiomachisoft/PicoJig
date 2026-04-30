// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef UART_H
#define UART_H

#include "Common.h"

// [define] / [定義]
#define UART_DATA_MAX_SIZE 256 // Make this sufficiently smaller than FRM_DATA_MAX_SIZE / FRM_DATA_MAX_SIZEより十分小さくすること

#pragma pack(1)

// [Structs] / [構造体]
// UART config / UART通信設定
typedef struct _ST_UART_CONFIG {
    ULONG baudrate; // Baud rate / ボーレート
    UCHAR dataBits; // Data bit length / データビット長
    UCHAR stopBits; // Stop bit length / ストップビット長
    UCHAR parity;   // UART_PARITY_NONE, UART_PARITY_EVEN, UART_PARITY_ODD / UART_PARITY_NONE, UART_PARITY_EVEN, UART_PARITY_ODD
} ST_UART_CONFIG;

#pragma pack()

// [Function prototype declarations] / [関数プロトタイプ宣言]
void UART_Main();
void UART_GetDefaultConfig(ST_UART_CONFIG *pstConfig);
void UART_Init(ST_UART_CONFIG *pstConfig);

#endif