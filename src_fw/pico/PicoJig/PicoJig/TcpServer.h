#ifndef TCP_SERVER_H
#define TCP_SERVER_H

// [define]
// デフォルト値
#define TCP_SERVER_DEFAULT_COUNTRY_CODE  "JP"       // カントリーコード
#define TCP_SERVER_DEFAULT_IP_ADDR       0xC0A80A64 // IPアドレス

// [列挙体]
// キューの種類
typedef enum _E_TCP_SERVER_PHASE {
    E_TCP_SERVER_PHASE_NOT_INIT,        // 未初期化
    E_TCP_SERVER_PHASE_INITED,          // 初期化済み
    E_TCP_SERVER_PHASE_CONNECTING_AP,   // APへの接続処理を実行中
    E_TCP_SERVER_PHASE_CONNECTED_AP,    // APに接続済み
    E_TCP_SERVER_PHASE_TCP_OPENED,      // TCPオープン済み
    E_TCP_SERVER_PHASE_TCP_ACCEPTED     // TCPアクセプト済み
} E_TCP_SERVER_PHASE;

#pragma pack(1)

// [構造体]
typedef struct TCP_SERVER_T_ {
    struct tcp_pcb *server_pcb;
    struct tcp_pcb *client_pcb;
    // 削除
    // =====>
    //bool complete;
    //uint8_t buffer_sent[TCP_BUF_SIZE];
    //uint8_t buffer_recv[TCP_BUF_SIZE];
    //int sent_len;
    //int recv_len;
    //int run_count;
    // <======
} TCP_SERVER_T;

// ネットワーク設定
typedef struct _ST_NW_CONFIG {
    char  aCountryCode[3];  // カントリーコード
    UCHAR aIpAddr[4];       // IPアドレス
    UCHAR aSsid[33];        // APのSSID
    UCHAR aPassword[65];    // APのパスワード
} ST_NW_CONFIG;

#pragma pack()

// [関数プロトタイプ宣言]
void tcp_server_main();
bool tcp_server_is_connected();
bool tcp_server_is_link_up();
err_t tcp_server_send_data(uint8_t* buffer_sent, uint32_t size);
void tcp_server_set_default(ST_NW_CONFIG *pstConfig);

#endif