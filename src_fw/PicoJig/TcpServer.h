// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef TCP_SERVER_H
#define TCP_SERVER_H

#include "Common.h"

// [define] / [定義]
// Default values / デフォルト値
#define TCP_SERVER_DEFAULT_COUNTRY_CODE  "XX"       // Country code / カントリーコード
#define TCP_SERVER_DEFAULT_IP_ADDR       0xC0A80A64 // IP address / IPアドレス

#ifdef MY_BOARD_PICO_W
// [Enums] / [列挙体]
// Phase / フェーズ
typedef enum _E_TCP_SERVER_PHASE {
    E_TCP_SERVER_PHASE_NOT_INIT,        // Not initialized / 未初期化
    E_TCP_SERVER_PHASE_INITED,          // Initialized / 初期化済み
    E_TCP_SERVER_PHASE_CONNECTING_AP,   // Connecting to AP / APへの接続処理を実行中
    E_TCP_SERVER_PHASE_CONNECTED_AP,    // Connected to AP / APに接続済み
    E_TCP_SERVER_PHASE_TCP_OPENED,      // TCP opened / TCPオープン済み
    E_TCP_SERVER_PHASE_TCP_ACCEPTED     // TCP accepted / TCPアクセプト済み
} E_TCP_SERVER_PHASE;

// [Structs] / [構造体]
typedef struct TCP_SERVER_T_ {
    struct tcp_pcb *server_pcb;
    struct tcp_pcb *client_pcb;
} TCP_SERVER_T;

#endif

#pragma pack(1)

// [Structs] / [構造体]
// Network config / ネットワーク設定
typedef struct _ST_NW_CONFIG {
    char  szCountryCode[3]; // Country code *unused / カントリーコード ※未使用
    UCHAR aIpAddr[4];       // IP address / IPアドレス
    char  szSsid[33];       // AP SSID / APのSSID
    char  szPassword[65];   // AP password / APのパスワード
} ST_NW_CONFIG;

#pragma pack()

// [Function prototype declarations] / [関数プロトタイプ宣言]
#ifdef MY_BOARD_PICO_W
void tcp_server_main();
bool tcp_server_is_inited();
bool tcp_server_is_connected();
bool tcp_server_is_link_up();
err_t tcp_server_send_data(uint8_t* buffer_sent, uint32_t size);
void tcp_server_get_default_config(ST_NW_CONFIG *pstConfig);
#endif

#endif