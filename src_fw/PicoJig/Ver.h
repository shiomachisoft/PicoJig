// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef VER_H
#define VER_H

// [define]
// ↓PicoJig-WLとしてビルドする場合は有効にする
#define MY_BOARD_PICO_W

// FWバージョン
#define FW_VER 0x25050100

// FW名
#ifdef MY_BOARD_PICO_W
#define FW_NAME "PicoJig_WL" // FW名のサイズは、NULL文字含めてFW_NAME_BUF_SIZEのサイズ以内
#else
#define FW_NAME "PicoJig" // FW名のサイズは、NULL文字含めてFW_NAME_BUF_SIZEのサイズ以内
#endif

// FW名のバッファサイズ
#define FW_NAME_BUF_SIZE 16

// メーカー名
#define MAKER_NAME "SHIOMACHI_SOFT"

// メーカー名のバッファサイズ
#define MAKER_NAME_BUF_SIZE 16

#pragma pack(1)

// [構造体]
// FW情報
typedef struct _ST_FW_INFO {
    char szMakerName[MAKER_NAME_BUF_SIZE];  // メーカー名
    char szFwName[FW_NAME_BUF_SIZE];        // FW名
    ULONG fwVer;                            // FWバージョン
    pico_unique_board_id_t board_id;        // ユニークボードID
} ST_FW_INFO;

#pragma pack()


#endif