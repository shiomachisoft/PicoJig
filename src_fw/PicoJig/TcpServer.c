// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifdef MY_BOARD_PICO_W
#include "Common.h"

// [define] / [定義]
#define TCP_SERVER_CONNECT_AP_INTERVAL 1000000ULL // us 1 second If connection to AP fails, wait this time before returning the phase to E_TCP_SERVER_PHASE_INITED / 100ms APとの接続に失敗した場合、この時間を待ってからフェーズをE_TCP_SERVER_PHASE_INITEDに戻す
#define TCP_SERVER_CONNECT_AP_TIMEOUT 10000000ULL // us 10 seconds If connection to AP cannot be established after this time has passed, return the phase to E_TCP_SERVER_PHASE_INITED / 10秒 この時間が経過してもAPと接続できない場合、フェーズをE_TCP_SERVER_PHASE_INITEDに戻す

// [define] / [定義]
#define WIFI_HOSTNAME FW_NAME // Hostname / ホスト名
#define TCP_PORT 7777 // Socket port number / ソケットポート番号

// [File scope variables] / [ファイルスコープ変数]
static E_TCP_SERVER_PHASE f_ePhase = E_TCP_SERVER_PHASE_NOT_INIT; // Phase / フェーズ
static volatile uint64_t f_startUs = 0;
static TCP_SERVER_T f_state = {0}; 

// [Function prototype declarations] / [関数プロトタイプ宣言]
// ===== Wi-Fi/Initialization related ===== / ===== Wi-Fi・初期化関連 =====
static int tcp_server_init();
static bool tcp_server_connect_ap_async();
static bool tcp_server_check_link_up();
static void tcp_server_check_link_down();

// ===== TCP server lifecycle related ===== / ===== TCPサーバー・ライフサイクル関連 =====
static bool tcp_server_open(void *arg);
static err_t tcp_server_result(void *arg, int status);
static err_t tcp_server_close(void *arg);
static err_t tcp_server_client_close(void *arg);

// ===== TCP communication callback related ===== / ===== TCP通信コールバック関連 =====
static err_t tcp_server_accept(void *arg, struct tcp_pcb *client_pcb, err_t err);
static err_t tcp_server_recv(void *arg, struct tcp_pcb *tpcb, struct pbuf *p, err_t err);
static err_t tcp_server_sent(void *arg, struct tcp_pcb *tpcb, u16_t len);
static void tcp_server_err(void *arg, err_t err);

// TCP server main processing / TCPサーバーのメイン処理
void tcp_server_main()
{   
    // Check if connection with AP is lost / APとの接続が切れていないかを確認する
    tcp_server_check_link_down();  

    // Execute processing according to phase / フェーズに応じた処理の実行
    switch (f_ePhase)
    {
        case E_TCP_SERVER_PHASE_NOT_INIT: // Not initialized / 未初期化  
            if (0 == tcp_server_init()) {
                f_ePhase = E_TCP_SERVER_PHASE_INITED; // Initialized / 初期化済み
            }        
            break;
        case E_TCP_SERVER_PHASE_INITED: // Initialized / 初期化済み
            if (true == tcp_server_connect_ap_async()) {
                f_ePhase = E_TCP_SERVER_PHASE_CONNECTING_AP; // Connecting to AP / APへの接続処理を実行中
            }
            break;
        case E_TCP_SERVER_PHASE_CONNECTING_AP: // Connecting to AP / APへの接続処理を実行中
            if (true == tcp_server_check_link_up()) {
                f_ePhase = E_TCP_SERVER_PHASE_CONNECTED_AP; // Connected to AP / APに接続済み
            }         
            break;
        case E_TCP_SERVER_PHASE_CONNECTED_AP: // Connected to AP / APに接続済み 
            if (true == tcp_server_open(&f_state)) {
                f_ePhase = E_TCP_SERVER_PHASE_TCP_OPENED; // TCP opened / TCPオープン済み
            }        
            break;
        case E_TCP_SERVER_PHASE_TCP_OPENED:   // TCP opened / TCPオープン済み
        case E_TCP_SERVER_PHASE_TCP_ACCEPTED: // TCP accepted / TCPアクセプト済み
        default:
            // Maintain state (do nothing) / 状態維持(何もしない)
            break;
    }   

    // Poll cyw43 background processing / cyw43のバックグラウンド処理をポーリングする
    if (f_ePhase >= E_TCP_SERVER_PHASE_INITED) { // If initialized / 初期化済みの場合
        cyw43_arch_poll();
    }
}

// Get whether TCP server is initialized / TCPサーバーが初期化済みか否かを取得
bool tcp_server_is_inited()
{
    return (f_ePhase >= E_TCP_SERVER_PHASE_INITED) ? true : false;
}

// Get whether TCP is connected with client / クライアントとTCP接続済みか否かを取得
bool tcp_server_is_connected()
{
    return (E_TCP_SERVER_PHASE_TCP_ACCEPTED == f_ePhase) ? true : false;
}

// Get whether connected to AP / APと接続済みか否かを取得
bool tcp_server_is_link_up()
{
    return (f_ePhase >= E_TCP_SERVER_PHASE_TCP_OPENED) ? true : false;
}

// Store default values in ST_NW_CONFIG structure / ST_NW_CONFIG構造体にデフォルト値を格納
void tcp_server_set_default(ST_NW_CONFIG *pstConfig)
{
    strcpy(pstConfig->szCountryCode, TCP_SERVER_DEFAULT_COUNTRY_CODE);
    pstConfig->aIpAddr[0] = (TCP_SERVER_DEFAULT_IP_ADDR >> 24) & 0xFF;
    pstConfig->aIpAddr[1] = (TCP_SERVER_DEFAULT_IP_ADDR >> 16) & 0xFF;
    pstConfig->aIpAddr[2] = (TCP_SERVER_DEFAULT_IP_ADDR >> 8) & 0xFF;
    pstConfig->aIpAddr[3] = (TCP_SERVER_DEFAULT_IP_ADDR) & 0xFF;
    memset(pstConfig->szSsid, 0, sizeof(pstConfig->szSsid));
    memset(pstConfig->szPassword, 0, sizeof(pstConfig->szPassword));
}

// Initialization / 初期化
static int tcp_server_init()
{
    int err = -1;
    ST_FLASH_DATA* pstFlashData;

    do {
        // Initialize CYW43 architecture / CYW43アーキテクチャの初期化
        err = cyw43_arch_init();
        if (err != 0) break;

        // Enable station mode (Wi-Fi client) / ステーションモード(Wi-Fiクライアント)を有効化
        cyw43_arch_enable_sta_mode();
        netif_set_hostname(netif_default, WIFI_HOSTNAME);

        // Get FLASH data at power-on and set network config such as IP address / 電源起動時のFLASHデータを取得し、IPアドレス等のネットワーク設定を行う
        pstFlashData = FLASH_GetDataAtPowerOn(); 
        ip4_addr_t ip, mask, gw;
        IP4_ADDR(&ip, pstFlashData->stNwConfig.aIpAddr[0], 
                      pstFlashData->stNwConfig.aIpAddr[1], 
                      pstFlashData->stNwConfig.aIpAddr[2],
                      pstFlashData->stNwConfig.aIpAddr[3]);
        IP4_ADDR(&mask, 255, 255, 255, 0);
        IP4_ADDR(&gw, 0, 0, 0, 0);
        netif_set_addr(netif_default, &ip, &mask, &gw);

        // Disable power management (for stable communication) / パワーマネジメントを無効に設定(安定した通信のため)
        err = cyw43_wifi_pm(&cyw43_state, CYW43_NONE_PM);
        if (err != 0) {
            cyw43_arch_deinit(); // Deinitialize on failure to avoid double-init panic on next try / 次回の再試行時に二重初期化でパニックになるのを防ぐため初期化を解除
            break;
        }
    } while(0);    
    
    return err;
}

// Connect to access point (AP) asynchronously / 非同期でアクセスポイント(AP)へ接続する
static bool tcp_server_connect_ap_async()
{
    ST_FLASH_DATA* pstFlashData;

    // Reset Wi-Fi driver state before reconnecting / 再接続前にWi-Fiドライバの内部状態を確実にリセットする
    cyw43_wifi_leave(&cyw43_state, CYW43_ITF_STA);

    pstFlashData = FLASH_GetDataAtPowerOn(); // Get FLASH data at power-on / 電源起動時のFLASHデータを取得
    (void)cyw43_arch_wifi_connect_bssid_async(pstFlashData->stNwConfig.szSsid, 
                                              NULL, 
                                              pstFlashData->stNwConfig.szPassword, 
                                              CYW43_AUTH_WPA2_MIXED_PSK); 
    
    // Move to CONNECTING_AP phase regardless of success or failure, and manage timeout (retry interval) on the check_link_up side / 成功・失敗に関わらず CONNECTING_AP フェーズに移行させ、check_link_up 側でタイムアウト(再試行間隔)を管理する
    // (To prevent infinite unweighted calls in the main loop and resulting load if false is returned on failure) / (失敗時に false を返すとメインループ内でウェイトなしで無限に呼び出され負荷がかかるのを防ぐため)
    f_startUs = time_us_64();
    return true;
}

// Check connection with AP (Implementation referenced from cyw43_arch_wifi_connect_bssid_until()) / APとの接続を確認 (cyw43_arch_wifi_connect_bssid_until()を参考にした実装)
static bool tcp_server_check_link_up()
{
    int status;
    volatile uint64_t endUs, diffUs, threshold;
    bool bRet = false;

    // Get link status / リンクステータスを取得
    status = cyw43_tcpip_link_status(&cyw43_state, CYW43_ITF_STA);
    if (status >= CYW43_LINK_UP) { // If IP address acquisition is completed (link up) / IPアドレスの取得まで完了した場合(リンクアップ)        
        bRet = true;    
    }  
    else {
        // Return to initial state if connection status is failure or timeout occurs / 接続状態が失敗、もしくはタイムアウトした場合は初期状態に戻す
        if (status < CYW43_LINK_DOWN) { // Connection failure confirmed / 接続失敗が確定
            threshold = TCP_SERVER_CONNECT_AP_INTERVAL;
        }
        else { // Connecting / 接続処理中
            threshold = TCP_SERVER_CONNECT_AP_TIMEOUT;
        }
        
        endUs = time_us_64();
        diffUs = endUs - f_startUs;
        if (diffUs >= threshold) {
            f_ePhase = E_TCP_SERVER_PHASE_INITED; // Return phase to attempt reconnection / 再接続を試みるためにフェーズを戻す
        }   
    }

    return bRet;
}

// Detect link down (disconnection) with access point (AP) / アクセスポイント(AP)とのリンクダウン(切断)を検知する
static void tcp_server_check_link_down()
{
    int status;

    if (f_ePhase >= E_TCP_SERVER_PHASE_CONNECTED_AP)
    {
        // Get link status / リンクステータスを取得
        status = cyw43_tcpip_link_status(&cyw43_state, CYW43_ITF_STA); 
        if (status < CYW43_LINK_UP) { // If IP address is lost or link is disconnected / IPアドレス喪失、またはリンクが切断された場合
            tcp_server_close(&f_state); // Close TCP resources / TCPリソースをクローズしておく
            f_ePhase = E_TCP_SERVER_PHASE_INITED; // Return to initial state and prompt reconnection / 初期状態に戻して再接続を促す
        }
    }
}

// Open TCP server / TCPサーバーをオープンする
static bool tcp_server_open(void *arg) {
    bool bRet = false;
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;

    // Create a new TCP protocol control block (any IP type) / 新しいTCPプロトコル制御ブロックを作成(IPタイプは任意)
    struct tcp_pcb *pcb = tcp_new_ip_type(IPADDR_TYPE_ANY);
    if (!pcb) {
        goto end;
    }
    
    // Bind to specified port number / 指定のポート番号にバインド
    err_t err = tcp_bind(pcb, NULL, TCP_PORT);
    if (err) {
        tcp_close(pcb);
        goto end;
    }

    // Set to wait for connection from client (backlog size: 1) / クライアントからの接続待ち状態にする(バックログサイズ: 1)
    state->server_pcb = tcp_listen_with_backlog(pcb, 1);
    if (!state->server_pcb) {
        if (pcb) {
            tcp_close(pcb);
        }
        goto end;
    }

    // Set argument for callback function and callback when connection request is accepted / コールバック関数用の引数と、接続要求を受け付けた際のコールバックを設定
    tcp_arg(state->server_pcb, state);
    tcp_accept(state->server_pcb, tcp_server_accept);
    
    bRet = true;

end:
    return bRet;    
}

// Process termination result of TCP communication / TCP通信の終了結果を処理する
static err_t tcp_server_result(void *arg, int status) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;

    // ERR_ABRT is set when tcp_close() returns something other than ERR_OK inside tcp_server_client_close(). / tcp_server_client_close()の中でtcp_close()がERR_OK以外を返した時にERR_ABRTがセットされる。
    // That means if it is ERR_ABRT, tcp_server_client_close() has already been executed, so exclude it here. / つまりERR_ABRTということは既にtcp_server_client_close()を実行済みのため、ここでは除外する。
    if ((status != ERR_OK) && (status != ERR_ABRT)) {
        // Close only the client connection and maintain listening as a server / クライアント接続だけをクローズし、サーバーとしてのリスニングは維持する
        tcp_server_client_close(state); 
        f_ePhase = E_TCP_SERVER_PHASE_TCP_OPENED; // Return to waiting for TCP accept / TCPアクセプト待ちに戻る
    }
    return ERR_OK;
}

// Close only client connection / クライアントの接続のみをクローズする
static err_t tcp_server_client_close(void *arg) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    err_t err = ERR_OK;

    if (state->client_pcb != NULL) {
        tcp_arg(state->client_pcb, NULL);
        tcp_poll(state->client_pcb, NULL, 0);
        tcp_sent(state->client_pcb, NULL);
        tcp_recv(state->client_pcb, NULL);
        tcp_err(state->client_pcb, NULL);
        err = tcp_close(state->client_pcb);
        if (err != ERR_OK) {
            tcp_abort(state->client_pcb); // Forcibly disconnect if close fails / クローズに失敗した場合は強制切断
            err = ERR_ABRT;
        }
        state->client_pcb = NULL;
    }
    return err;
}

// Close connection of TCP server and client / TCPサーバーおよびクライアントの接続をクローズする
static err_t tcp_server_close(void *arg) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    err_t err = ERR_OK;

    // Close TCP connection with client / クライアントとのTCP接続をクローズ
    err = tcp_server_client_close(arg);
    
    // Close listening TCP on the server side / サーバー側の待ち受けTCPをクローズ
    if (state->server_pcb) {
        tcp_arg(state->server_pcb, NULL);
        err = tcp_close(state->server_pcb);
        state->server_pcb = NULL;
    }

    return err;
}

// Accept TCP connection request from client / クライアントからのTCP接続要求を受け付ける
static err_t tcp_server_accept(void *arg, struct tcp_pcb *client_pcb, err_t err) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    if (err != ERR_OK || client_pcb == NULL) {    
        return ERR_VAL;
    }

    // If already connected to another client, reject (disconnect) the new connection request / すでに他のクライアントと接続済みの場合は、新しい接続要求を拒否(切断)する
    if (state->client_pcb != NULL) {
        tcp_abort(client_pcb);
        return ERR_ABRT;
    }

    state->client_pcb = client_pcb;
    tcp_arg(client_pcb, state);
    tcp_sent(client_pcb, tcp_server_sent);
    tcp_recv(client_pcb, tcp_server_recv);
    tcp_err(client_pcb, tcp_server_err);

    tcp_nagle_disable(client_pcb); // Disable Nagle algorithm for low latency / リアルタイム性向上のためNagleアルゴリズムを無効化

    f_ePhase = E_TCP_SERVER_PHASE_TCP_ACCEPTED; // TCP accepted / TCPアクセプト済み
    return ERR_OK;
}

// Send TCP data to client / クライアントへTCPデータを送信する
err_t tcp_server_send_data(uint8_t* buffer_sent, uint32_t size)
{
    // Error if client is not connected / クライアントが接続されていない場合はエラー
    if (NULL == f_state.client_pcb) {
        return ERR_VAL;
    }    

    // Check lwIP context / lwIPのコンテキストチェック
    cyw43_arch_lwip_check();
    
    // Write data to TCP send buffer / データをTCP送信バッファに書き込む
    err_t err = tcp_write(f_state.client_pcb, buffer_sent, size, TCP_WRITE_FLAG_COPY);
    if (err != ERR_OK) {
        // Processing upon write failure (do not disconnect for non-fatal errors) / 書き込み失敗時の処理（致命的でないエラー時は切断しない）
        return err;
    }
    else {
        // Trigger actual data transmission / 実際のデータ送信をトリガーする
        tcp_output(f_state.client_pcb);
    }

    return ERR_OK;
}

// TCP data send completion callback / TCPデータの送信完了コールバック
static err_t tcp_server_sent(void *arg, struct tcp_pcb *tpcb, u16_t len) {
    // No specific processing for this use case / 今回の用途では特に処理なし
    return ERR_OK;
}

// TCP data receive callback / TCPデータの受信コールバック
static err_t tcp_server_recv(void *arg, struct tcp_pcb *tpcb, struct pbuf *p, err_t err) {
    struct pbuf *q;
    uint16_t i;

    // In case of disconnection request from client (p == NULL) etc. / クライアントからの切断要求(p == NULL)などの場合
    if (!p) {
        // Pass status indicating normal disconnection / 正常な切断を意味するステータスを渡す
        return tcp_server_result(arg, ERR_CLSD);
    }
    
    // Check lwIP context / lwIPのコンテキストチェック
    cyw43_arch_lwip_check();
    
    if (p->tot_len > 0) {
        // Follow pbuf chain and enqueue directly to queue / pbufチェインを辿って直接キューにエンキュー
        for (q = p; q != NULL; q = q->next) {
            uint8_t *payload = (uint8_t *)q->payload;
            for (i = 0; i < q->len; i++) {
                if (!CMN_Enqueue(CMN_QUE_KIND_WL_RECV, &payload[i], true)) { 
                    break; // Queue is full / キューが満杯
                }
            }
            if (i < q->len) {
                break; // Break outer loop as well if queue is full in inner loop / 内側のループでキューが満杯になった場合、外側のループも抜ける
            }
        }
        
        // Notify lwIP that data was received by application side / アプリケーション側でデータを受信したことをlwIPに通知
        tcp_recved(tpcb, p->tot_len);
    }
    pbuf_free(p);

    return ERR_OK;
}

// Callback when an error occurs in TCP communication / TCP通信でエラーが発生した時のコールバック
static void tcp_server_err(void *arg, err_t err) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    
    // [Important] lwIP specification: The target client_pcb is already freed at the time the error callback is called. / 【重要】lwIP仕様: エラーコールバックが呼ばれた時点で対象の client_pcb は既に解放済み。
    // Ensure to set to NULL to prevent tcp_close() (Use-After-Free) on freed memory. / 解放済みのメモリに対する tcp_close() (Use-After-Free) を防ぐため、必ず NULL にする。
    state->client_pcb = NULL;

    if (err != ERR_ABRT) {
        tcp_server_result(arg, err);
    } else {
        // Also return to listening standby phase during intentional disconnection by tcp_abort() / tcp_abort()による意図的な切断時も、リスニング待機フェーズには戻しておく
        f_ePhase = E_TCP_SERVER_PHASE_TCP_OPENED;
    }
}

#endif // MY_BOARD_PICO_W