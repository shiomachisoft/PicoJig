// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef VER_H
#define VER_H

#include "Common.h"

// [define] / [定義]
// FW version / FWバージョン
#define FW_VER 0x26042800

// FW name / FW名
#ifdef MY_BOARD_PICO_W
#define FW_NAME "PicoJig_WL" // FW name size must be within FW_NAME_BUF_SIZE including NULL character / FW名のサイズは、NULL文字含めてFW_NAME_BUF_SIZEのサイズ以内
#else
#define FW_NAME "PicoJig" // FW name size must be within FW_NAME_BUF_SIZE including NULL character / FW名のサイズは、NULL文字含めてFW_NAME_BUF_SIZEのサイズ以内
#endif

// Buffer size for FW name / FW名のバッファサイズ
#define FW_NAME_BUF_SIZE 16

// Maker name / メーカー名
#define MAKER_NAME "SHIOMACHI_SOFT"

// Buffer size for maker name / メーカー名のバッファサイズ
#define MAKER_NAME_BUF_SIZE 16

#pragma pack(1)

// [Structs] / [構造体]
// FW info / FW情報
typedef struct _ST_FW_INFO {
    char szMakerName[MAKER_NAME_BUF_SIZE];  // Maker name / メーカー名
    char szFwName[FW_NAME_BUF_SIZE];        // FW name / FW名
    ULONG fwVer;                            // FW version / FWバージョン
    pico_unique_board_id_t board_id;        // Unique board ID / ユニークボードID
} ST_FW_INFO;

#pragma pack()

#endif