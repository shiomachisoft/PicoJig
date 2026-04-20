// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef FLASH_H
#define FLASH_H

#include "Common.h"

#pragma pack(1)

// [Structs] / [構造体]
// FLASH data / FLASHデータ
typedef struct _ST_FLASH_DATA {
    char           szFwName[FW_NAME_BUF_SIZE];  // FW name / FW名
    ULONG          fwVer;                       // FW version / FWバージョン
    ST_GPIO_CONFIG stGpioConfig;                // GPIO config / GPIO設定
    ST_UART_CONFIG stUartConfig;                // UART config / UART通信設定
    ST_SPI_CONFIG  stSpiConfig;                 // SPI config / SPI通信設定
    ST_I2C_CONFIG  stI2cConfig;                 // I2C config / I2C通信設定
    ST_NW_CONFIG   stNwConfig;                  // Network config / ネットワーク設定
    USHORT         checksum;                    // Checksum / チェックサム
} ST_FLASH_DATA;

#pragma pack()

// [Function prototype declarations] / [関数プロトタイプ宣言]
ST_FLASH_DATA* FLASH_GetDataAtPowerOn();
void FLASH_Read(ST_FLASH_DATA *pstFlashData);
void FLASH_Write(ST_FLASH_DATA *pstFlashData);
void FLASH_Erase();
void FLASH_Init();

#endif