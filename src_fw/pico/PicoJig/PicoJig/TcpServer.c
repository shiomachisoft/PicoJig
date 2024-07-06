#include "Common.h"

// [define]
#define TCP_SERVER_POLL_TIME_S 5
#define TCP_SERVER_DRIVER_WORK_TIMEOUT 1000 //ms
#define TCP_SERVER_THRESHOLD_LESS_THAN_CYW43_LINK_DOWN 100000ULL // us
#define TCP_SERVER_THRESHOLD_MORE_THAN_CYW43_LINK_DOWN 10000000ULL // us

// [define]
#define WIFI_HOSTNAME FW_NAME // hostname
#define TCP_PORT 7777 // ソケットポート番号

// [ファイルスコープ変数]
static E_TCP_SERVER_PHASE f_ePhase = E_TCP_SERVER_PHASE_NOT_INIT; // フェーズ
static uint64_t f_startUs = 0;
static UCHAR f_aRecvData[CMN_QUE_DATA_MAX_WL_RECV] = {0}; // 受信バッファ
static TCP_SERVER_T f_state = {0}; 

// [関数プロトタイプ宣言]
static int tcp_server_init();
static bool tcp_server_check_link_up();
static void tcp_server_check_link_down();
static err_t tcp_server_accept(void *arg, struct tcp_pcb *client_pcb, err_t err);
static bool tcp_server_open(void *arg);
static err_t tcp_server_close(void *arg);
static err_t tcp_server_result(void *arg, int status);
static err_t tcp_server_sent(void *arg, struct tcp_pcb *tpcb, u16_t len);
static err_t tcp_server_recv(void *arg, struct tcp_pcb *tpcb, struct pbuf *p, err_t err);
static err_t tcp_server_poll(void *arg, struct tcp_pcb *tpcb);
static void tcp_server_err(void *arg, err_t err);
static bool tcp_server_connect_ap_async();

// TCPサーバーのメイン処理
void tcp_server_main()
{   
    // APとの接続が切れていなかを確認する
    tcp_server_check_link_down();  

    // フェーズ
    switch (f_ePhase)
    {
        case E_TCP_SERVER_PHASE_NOT_INIT: // 未初期化  
            if (0 == tcp_server_init()) {
                f_ePhase = E_TCP_SERVER_PHASE_INITED; // 初期化済み
            }        
            break;
        case E_TCP_SERVER_PHASE_INITED: // 初期化済み
            if (true == tcp_server_connect_ap_async()) {
                f_ePhase = E_TCP_SERVER_PHASE_CONNECTING_AP; // APへの接続処理を実行中
            }
            break;
        case E_TCP_SERVER_PHASE_CONNECTING_AP: // APへの接続処理を実行中
            if (true == tcp_server_check_link_up()) {
                f_ePhase = E_TCP_SERVER_PHASE_CONNECTED_AP; // APに接続済み
            }         
            break;
        case E_TCP_SERVER_PHASE_CONNECTED_AP: // APに接続済み 
            if (true == tcp_server_open(&f_state)) {
                f_ePhase = E_TCP_SERVER_PHASE_TCP_OPENED; // TCPオープン済み
            }        
            break;
        case E_TCP_SERVER_PHASE_TCP_OPENED:   // TCPオープン済み
        case E_TCP_SERVER_PHASE_TCP_ACCEPTED: // TCPアクセプト済み
        default:
            // 何もしない
            break;
    }   

    // ポーリング
    if (f_ePhase >= E_TCP_SERVER_PHASE_INITED) { // 初期化済みの場合
        cyw43_arch_poll();
        //cyw43_arch_wait_for_work_until(make_timeout_time_ms(TCP_SERVER_DRIVER_WORK_TIMEOUT)); 
    }
}

// TCP接続済みか否かを取得
bool tcp_server_is_connected()
{
    return (E_TCP_SERVER_PHASE_TCP_ACCEPTED == f_ePhase) ? true : false;
}

// APと接続済み否かを取得
bool tcp_server_is_link_up()
{
    return (f_ePhase >= E_TCP_SERVER_PHASE_TCP_OPENED) ? true : false;
}

// 初期化
static int tcp_server_init()
{
    int err = -1;
    ST_FLASH_DATA* pstFlashData;;
    static bool s_isFirst = true;

    if (true == s_isFirst) {
        s_isFirst = false;
    }
    else {
        cyw43_arch_deinit();
    }

    do {
        pstFlashData = FLASH_GetDataAtPowerOn(); // 電源起動時のFLASHデータを取得
        err = cyw43_arch_init_with_country(CYW43_COUNTRY(pstFlashData->stNwConfig.aCountryCode[0], 
                                                         pstFlashData->stNwConfig.aCountryCode[1], 
                                                         pstFlashData->stNwConfig.aCountryCode[2]
                                                         ));
        if (err != 0) break;

        cyw43_arch_enable_sta_mode();
        netif_set_hostname(netif_default, WIFI_HOSTNAME);

        //err = cyw43_wifi_pm(&cyw43_state, CYW43_PERFORMANCE_PM);
        err = cyw43_wifi_pm(&cyw43_state, cyw43_pm_value(CYW43_NO_POWERSAVE_MODE, 10, 1, 1, 1));
        if (err != 0) break;
    } while(0);    
    
    return err;
}

// APとの接続を確認
static bool tcp_server_check_link_up() // cyw43_arch_wifi_connect_bssid_until()を参考にした
{
    int status;
    uint64_t endUs, diffUs, threshold;
    ST_FLASH_DATA* pstFlashData;
    bool bRet = false;

    status = cyw43_tcpip_link_status(&cyw43_state, CYW43_ITF_STA);
    if (status >= CYW43_LINK_NOIP) { // Connected to wifi, but no IP address        
        pstFlashData = FLASH_GetDataAtPowerOn(); // 電源起動時のFLASHデータを取得
        ip_addr_t ip = IPADDR4_INIT_BYTES(pstFlashData->stNwConfig.aIpAddr[0], 
                                          pstFlashData->stNwConfig.aIpAddr[1], 
                                          pstFlashData->stNwConfig.aIpAddr[2],
                                          pstFlashData->stNwConfig.aIpAddr[3]
                                          );
        netif_set_ipaddr(netif_default, &ip);
        //ip_addr_t mask = IPADDR4_INIT_BYTES(255,255,255,0);
        //netif_set_netmask(netif_default, &mask);
        //ip_addr_t gw = IPADDR4_INIT_BYTES(192,168,10,1); 
        //netif_set_gw(netif_default, &gw); 
        bRet = true;    
    }  
    else {
        endUs = time_us_64();
        diffUs = endUs - f_startUs;
        if (status < CYW43_LINK_DOWN) { // 接続失敗
            threshold = TCP_SERVER_THRESHOLD_LESS_THAN_CYW43_LINK_DOWN;
        }
        else { // 接続を試み中
            threshold = TCP_SERVER_THRESHOLD_MORE_THAN_CYW43_LINK_DOWN;
        }
        if (diffUs >= threshold) {
            f_ePhase = E_TCP_SERVER_PHASE_INITED;
        }   
    }

    return bRet;
}

static bool tcp_server_open(void *arg) {
    bool bRet = false;
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;

    struct tcp_pcb *pcb = tcp_new_ip_type(IPADDR_TYPE_ANY);
    if (!pcb) {
        goto end;
    }

    err_t err = tcp_bind(pcb, NULL, TCP_PORT);
    if (err) {
        goto end;
    }

    state->server_pcb = tcp_listen_with_backlog(pcb, 1);
    if (!state->server_pcb) {
        if (pcb) {
            tcp_close(pcb);
        }
        goto end;
    }

    tcp_arg(state->server_pcb, state);
    tcp_accept(state->server_pcb, tcp_server_accept);
    
    bRet = true;

end:
    return bRet;    
}

static err_t tcp_server_accept(void *arg, struct tcp_pcb *client_pcb, err_t err) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    if (err != ERR_OK || client_pcb == NULL) {    
        tcp_server_result(arg, err);     
        return ERR_VAL;
    }

    state->client_pcb = client_pcb;
    tcp_arg(client_pcb, state);
    tcp_sent(client_pcb, tcp_server_sent);
    tcp_recv(client_pcb, tcp_server_recv);
    // ↓これを有効にすると、なぜか、tcp_server_send_data()を実行してからtcp_server_sent()が呼ばれるまでに何秒もかかる
    //tcp_poll(client_pcb, tcp_server_poll, TCP_SERVER_POLL_TIME_S * 2);
    tcp_err(client_pcb, tcp_server_err);

    // 変更
    // ====>
    //return tcp_server_send_data(arg, state->client_pcb);
    f_ePhase = E_TCP_SERVER_PHASE_TCP_ACCEPTED; // TCPアクセプト済み
    return ERR_OK;
    // <=====
}

static err_t tcp_server_result(void *arg, int status) {
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    // 削除
    // =====>
    //state->complete = true;
    // <=====
    // 変更
    // =====>
    //return tcp_server_close(arg);
    // ↓status = ERR_ABRTの場合、tcp_server_close()を実行しない理由:
    // tcp_server_close()の中でtcp_close()がERR_OK以外を返した時にERR_ABRTがセットされる。
    // つまりERR_ABRTということは既にtcp_server_close()を実行済み。
    if ((status != ERR_OK) && (status != ERR_ABRT)) {
        tcp_server_close(state); 
        f_ePhase = E_TCP_SERVER_PHASE_INITED;  
    }
    return ERR_OK;
    // <=====
}
static err_t tcp_server_close(void *arg) {
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
            tcp_abort(state->client_pcb);
            err = ERR_ABRT;
        }
        state->client_pcb = NULL;
    }
    if (state->server_pcb) {
        tcp_arg(state->server_pcb, NULL);
        tcp_close(state->server_pcb);
        state->server_pcb = NULL;
    }

    return err;
}

static err_t tcp_server_sent(void *arg, struct tcp_pcb *tpcb, u16_t len) {
    // 削除
    // =====>
#if 0
    TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    state->sent_len += len;

    if (state->sent_len >= TCP_BUF_SIZE) {
        // We should get the data back from the client
        state->recv_len = 0;
    }
#endif
    // <=====

    return ERR_OK;
}

// 変更
// =====>
//static err_t tcp_server_send_data(void *arg, struct tcp_pcb *tpcb, uint8_t* buffer_sent, uint32_t size)
err_t tcp_server_send_data(uint8_t* buffer_sent, uint32_t size)
// <=====
{
    // 削除
    // =====>      
    //TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    //memcpy(state->buffer_sent, buffer_sent, size);
    //state->sent_len = 0;
    // <=====
    
    // this method is callback from lwIP, so cyw43_arch_lwip_begin is not required, however you
    // can use this method to cause an assertion in debug mode, if this method is called when
    // cyw43_arch_lwip_begin IS needed
    cyw43_arch_lwip_check();
    // 変更
    // =====>
    //err_t err = tcp_write(tpcb, state->buffer_sent, TCP_BUF_SIZE, TCP_WRITE_FLAG_COPY);
    err_t err = tcp_write(f_state.client_pcb, buffer_sent, size, TCP_WRITE_FLAG_COPY);
    // <=====
    if (err != ERR_OK) {
        // 変更
        // =====>
        //return tcp_server_result(arg, -1);
        return tcp_server_result(&f_state, -1);
        // <=====
    }
    return ERR_OK;
}

static err_t tcp_server_recv(void *arg, struct tcp_pcb *tpcb, struct pbuf *p, err_t err) {
    ULONG i;
    ULONG copySize;
    // 削除
    // =====>
    //TCP_SERVER_T *state = (TCP_SERVER_T*)arg;
    // <=====
    if (!p) {
        return tcp_server_result(arg, -1);
    }
    // this method is callback from lwIP, so cyw43_arch_lwip_begin is not required, however you
    // can use this method to cause an assertion in debug mode, if this method is called when
    // cyw43_arch_lwip_begin IS needed
    cyw43_arch_lwip_check();
    if (p->tot_len > 0) {
        // Receive the buffer
        // 変更
        // =====>
        /*
        const uint16_t buffer_left = TCP_BUF_SIZE - state->recv_len;
        state->recv_len += pbuf_copy_partial(p, state->buffer_recv + state->recv_len,
                                             p->tot_len > buffer_left ? buffer_left : p->tot_len, 0);
        */
        copySize = p->tot_len;
        if (sizeof(f_aRecvData) < copySize) {
            copySize = sizeof(f_aRecvData);
        }
        pbuf_copy_partial(p, f_aRecvData, copySize, 0);
        for (i = 0; i < copySize; i++) {
            // 無線受信データ1byteのエンキュー
            if (!CMN_Enqueue(CMN_QUE_KIND_WL_RECV, &f_aRecvData[i], sizeof(UCHAR), true)) { // CPUコア1のエンキューとCPUコア0のデキューを排他する
                break; // キューが満杯
            }
        }
        // <=====
        tcp_recved(tpcb, p->tot_len);
    }
    pbuf_free(p);

    // 削除
    // =====>
#if 0    
    // Have we have received the whole buffer
    if (state->recv_len == TCP_BUF_SIZE) {

        // check it matches
        if (memcmp(state->buffer_sent, state->buffer_recv, TCP_BUF_SIZE) != 0) {
            return tcp_server_result(arg, -1);
        }

        // Test complete?
        state->run_count++;
        if (state->run_count >= TEST_ITERATIONS) {
            tcp_server_result(arg, 0);
            return ERR_OK;
        }

        // Send another buffer
        return tcp_server_send_data(arg, state->client_pcb);
    }
#endif
    // <=====
    return ERR_OK;
}

// 一定時間、データの送信も受信もない時に呼ばれる
static err_t tcp_server_poll(void *arg, struct tcp_pcb *tpcb) {
    // 変更
    // =====>
    //return tcp_server_result(arg, -1); // no response is an error?
    return tcp_server_result(arg, ERR_OK);
    // <=====
}

static void tcp_server_err(void *arg, err_t err) {
    if (err != ERR_ABRT) {
        tcp_server_result(arg, err);
    }
}

static bool tcp_server_connect_ap_async()
{
    int err;
    char szSsid[33] = {0};
    char szPassword[65] ={0};
    bool bRet = false;
    ST_FLASH_DATA* pstFlashData;

    pstFlashData = FLASH_GetDataAtPowerOn(); // 電源起動時のFLASHデータを取得
    memcpy(szSsid, pstFlashData->stNwConfig.aSsid, sizeof(szSsid));
    memcpy(szPassword, pstFlashData->stNwConfig.aPassword, sizeof(szPassword));
    err = cyw43_arch_wifi_connect_bssid_async(szSsid, NULL, szPassword, CYW43_AUTH_WPA2_AES_PSK); 
    if (0 == err) {
        f_startUs = time_us_64();
        bRet = true; 
    }

    return bRet;
}

static void tcp_server_check_link_down()
{
    int status;

    if (f_ePhase >= E_TCP_SERVER_PHASE_CONNECTED_AP)
    {
        status = cyw43_tcpip_link_status(&cyw43_state, CYW43_ITF_STA); 
        if (status < CYW43_LINK_NOIP) {
            f_ePhase = E_TCP_SERVER_PHASE_INITED;
        }
    }

    if (f_ePhase < E_TCP_SERVER_PHASE_TCP_OPENED) {
        tcp_server_close(&f_state); 
    }    
}

// ST_NW_CONFIG構造体にデフォルト値を格納
void tcp_server_set_default(ST_NW_CONFIG *pstConfig)
{
    strcpy(pstConfig->aCountryCode, TCP_SERVER_DEFAULT_COUNTRY_CODE);
    pstConfig->aIpAddr[0] = (TCP_SERVER_DEFAULT_IP_ADDR >> 24) & 0xFF;
    pstConfig->aIpAddr[1] = (TCP_SERVER_DEFAULT_IP_ADDR >> 16) & 0xFF;
    pstConfig->aIpAddr[2] = (TCP_SERVER_DEFAULT_IP_ADDR >> 8) & 0xFF;
    pstConfig->aIpAddr[3] = (TCP_SERVER_DEFAULT_IP_ADDR) & 0xFF;
    memset(pstConfig->aSsid, 0, sizeof(pstConfig->aSsid));
    memset(pstConfig->aPassword, 0, sizeof(pstConfig->aPassword));
}