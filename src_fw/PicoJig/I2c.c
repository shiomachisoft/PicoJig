// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義]
#define I2C_ID i2c1 // I2C ID / I2CのID
#define I2C_TIMEOUT 100000ULL // 100ms I2C send/receive timeout (us) / 100ms I2C送信/受信タイムアウト(us)

// Default values / デフォルト値
#define I2C_DEFAULT_FREQ 100000 // Clock frequency 100kHz / クロック周波数 100kHz

// [File scope variables] / [ファイルスコープ変数]
static USHORT f_dataSize = 0; // Sent/received size / 送信/受信済みサイズ
static ST_I2C_REQ f_stI2cReq = {0}; // I2C send/receive request / I2C送信/受信要求

// [Function prototype declarations] / [関数プロトタイプ宣言]
static int i2c_write_byte_separately(i2c_inst_t *i2c, uint8_t addr, const uint8_t *src, bool nostop, bool first, bool last);
static int i2c_read_byte_separately(i2c_inst_t *i2c, uint8_t addr, uint8_t *dst, bool nostop, bool first, bool last);

// I2C main processing / I2Cメイン処理
void I2C_Main()
{
    int size = 0;
    ULONG errorBits = 0;
    bool bNoStop = true;
    bool bFirst = false;
    bool bLast = false;
   
    if (f_stI2cReq.dataSize > 0) { 
        // If there are remaining bytes for I2C send/receive / I2C送信/受信の残りbyte数がある場合
        
        if (!f_dataSize) { // If sent/received size = 0 / 送信/受信済みサイズ = 0の場合
            bFirst = true;
        }    
        if (1 == f_stI2cReq.dataSize) { // If remaining bytes for I2C send/receive = 1 / I2C送信/受信の残りのbyte数 = 1の場合
            bNoStop = false;
            bLast = true;    
        }

        // 1st byte:  bNoStop=true,  bFirst=true,  bLast=false / 1byte目:  bNoStop=true,  bFirst=true,  bLast=false
        // Middle:    bNoStop=true,  bFirst=false, bLast=false / 中間:      bNoStop=true,  bFirst=false, bLast=false
        // Last byte: bNoStop=false, bFirst=false, bLast=true  / 最終byte: bNoStop=false, bFirst=false, bLast=true

        if (CMD_RECV_I2C == f_stI2cReq.cmd) { // I2C receive / I2C受信
            // Communicate 1 byte at a time to mimic asynchronous processing / 擬似的な非同期処理とするために1byteずつ通信
            // 1 byte I2C receive / 1byteのI2C受信
            size = i2c_read_byte_separately(I2C_ID, f_stI2cReq.slaveAddr, &f_stI2cReq.aData[f_dataSize], bNoStop, bFirst, bLast);
        }
        else { // I2C send / I2C送信
            // Communicate 1 byte at a time to mimic asynchronous processing / 擬似的な非同期処理とするために1byteずつ通信
            // 1 byte I2C send / 1byteのI2C送信
            size = i2c_write_byte_separately(I2C_ID, f_stI2cReq.slaveAddr, &f_stI2cReq.aData[f_dataSize], bNoStop, bFirst, bLast);
        }

        if (1 == size) { // If 1 byte I2C send/receive succeeds / 1byteのI2C送信/受信が成功した場合

            f_dataSize++; // Sent/received size + 1 / 送信/受信済みサイズ+1
            f_stI2cReq.dataSize--; // Remaining bytes for I2C send/receive - 1 / I2C送信/受信の残りbyte数-1
            if (!f_stI2cReq.dataSize) { // If sent/received to the end / 最後まで送信/受信した場合
                // Send success response frame / 成功の応答フレームを送信       
                FRM_SendResFrm(f_stI2cReq.seqNo, f_stI2cReq.cmd, FRM_ERR_SUCCESS, f_dataSize, f_stI2cReq.aData);
                f_dataSize = 0; // Sent/received size = 0 / 送信/受信済みサイズ=0
            } 
        }
        else { // If send/receive fails / 送信/受信に失敗した場合
            // Set FW error / FWエラーを設定
            if (PICO_ERROR_TIMEOUT == size) {
                errorBits = CMN_ERR_BIT_I2C_TIMEOUT;
            }
            else {
                errorBits = CMN_ERR_BIT_I2C_NO_DEVICE;
            }
            // Set FW error / FWエラーを設定
            CMN_SetErrorBits(errorBits, true);
            // Send failure response frame / 失敗の応答フレームを送信        
            FRM_SendResFrm(f_stI2cReq.seqNo, f_stI2cReq.cmd, FRM_ERR_I2C_NO_DEVICE, 0, NULL); 
            f_dataSize = 0;          // Sent/received size = 0 / 送信/受信済みサイズ=0
            f_stI2cReq.dataSize = 0; // Remaining bytes for I2C send/receive = 0 / I2C送信/受信の残りbyte数=0                 
        }
    }
    else { 
        // If there are no remaining bytes for I2C send/receive / I2C送信/受信の残りbyte数が無い場合
        
        // Dequeue I2C send/receive request / I2C送信/受信要求のデキュー
        if (CMN_Dequeue(CMN_QUE_KIND_I2C_REQ, &f_stI2cReq, true)) {
            // No processing / 無処理
        }
    }
}

// Store default values in ST_I2C_CONFIG structure / ST_I2C_CONFIG構造体にデフォルト値を格納
void I2C_SetDefault(ST_I2C_CONFIG *pstConfig)
{
    pstConfig->frequency = I2C_DEFAULT_FREQ;
}

// Initialize I2C / I2Cを初期化
void I2C_Init(ST_I2C_CONFIG *pstConfig)
{
    // [Initialize I2C1 (master)] / [I2C1(マスタ)を初期化]
    // SDA
    gpio_init(I2C_SDA);
    gpio_set_function(I2C_SDA, GPIO_FUNC_I2C);
    gpio_pull_up(I2C_SDA);
    // SCL
    gpio_init(I2C_SCL);
    gpio_set_function(I2C_SCL, GPIO_FUNC_I2C);
    gpio_pull_up(I2C_SCL);
    // Clock frequency / クロック周波数
    i2c_init(I2C_ID, pstConfig->frequency);   
}

// I2C send 1 byte at a time / 1byteずつI2C送信
#if 0
int i2c_write_blocking_internal(i2c_inst_t *i2c, uint8_t addr, const uint8_t *src, bool nostop,
                                       check_timeout_fn timeout_check, struct timeout_state *ts) {
#endif
static int i2c_write_byte_separately(i2c_inst_t *i2c, uint8_t addr, const uint8_t *src, bool nostop, bool first, bool last) {
#if 0
    invalid_params_if(I2C, addr >= 0x80); // 7-bit addresses / 7ビットアドレス
    invalid_params_if(I2C, i2c_reserved_addr(addr));
    // Synopsys hw accepts start/stop flags alongside data items in the same / Synopsysハードウェアは、同じFIFOワード内のデータ項目と一緒に
    // FIFO word, so no 0 byte transfers. / start/stopフラグを受け入れるため、0バイト転送は行いません。
    invalid_params_if(I2C, len == 0);
    invalid_params_if(I2C, ((int)len) < 0);
#endif

    if (first) {
        i2c->hw->enable = 0;
        i2c->hw->tar = addr;
        i2c->hw->enable = 1;
    }

    bool abort = false;
    bool timeout = false;

    uint32_t abort_reason = 0;
    int byte_ctr;

    volatile uint64_t startUs, currentUs, diffUs;
#if 0
    int ilen = (int)len;
#endif
    int ilen = 1;
    for (byte_ctr = 0; byte_ctr < ilen; ++byte_ctr) {
#if 0
        bool first = byte_ctr == 0;
        bool last = byte_ctr == ilen - 1;
#endif

        i2c->hw->data_cmd =
                bool_to_bit(first && i2c->restart_on_next) << I2C_IC_DATA_CMD_RESTART_LSB |
                bool_to_bit(last && !nostop) << I2C_IC_DATA_CMD_STOP_LSB |
                *src++;

        // Wait until the transmission of the address/data from the internal / 内部シフトレジスタからのアドレス/データの送信が完了するまで待機します。
        // shift register has completed. For this to function correctly, the / これが正しく機能するためには、IC_CONのTX_EMPTY_CTRLフラグを
        // TX_EMPTY_CTRL flag in IC_CON must be set. The TX_EMPTY_CTRL flag / 設定する必要があります。TX_EMPTY_CTRLフラグはi2c_initで設定されました。
        // was set in i2c_init.
        startUs = time_us_64();
        do {
#if 0
            if (timeout_check) {
                timeout = timeout_check(ts);
                abort |= timeout;
            }
#endif
            currentUs = time_us_64();    
            diffUs = currentUs - startUs;
            if (diffUs >= I2C_TIMEOUT) {
                timeout = true;
                abort |= timeout;
            }
            tight_loop_contents();
        } while (!timeout && !(i2c->hw->raw_intr_stat & I2C_IC_RAW_INTR_STAT_TX_EMPTY_BITS));

        // If there was a timeout, don't attempt to do anything else. / タイムアウトが発生した場合は、他の処理を試行しません。
        if (!timeout) {
            abort_reason = i2c->hw->tx_abrt_source;
            if (abort_reason) {
                // Note clearing the abort flag also clears the reason, and / 中断フラグをクリアすると理由もクリアされることに注意してください。
                // this instance of flag is clear-on-read! Note also the / また、このフラグは読み取り時にクリアされる仕様であることにも注意してください。
                // IC_CLR_TX_ABRT register always reads as 0. / IC_CLR_TX_ABRTレジスタは常に0として読み取られることにも注意してください。
                i2c->hw->clr_tx_abrt;
                abort = true;
            }

            if (abort || (last && !nostop)) {
                // If the transaction was aborted or if it completed / トランザクションが中断された場合、または正常に完了した場合、
                // successfully wait until the STOP condition has occured. / STOP条件が発生するまで待機します。

                // TODO Could there be an abort while waiting for the STOP / TODO ここでSTOP条件を待っている間に中断が発生する可能性はありますか？
                // condition here? If so, additional code would be needed here / もしそうなら、中断に対処するための追加コードがここに必要になります。
                // to take care of the abort.
                startUs = time_us_64();
                do {
#if 0
                    if (timeout_check) {
                        timeout = timeout_check(ts);
                        abort |= timeout;
                    }
#endif
                    currentUs = time_us_64();    
                    diffUs = currentUs - startUs;
                    if (diffUs >= I2C_TIMEOUT) {
                        timeout = true;
                        abort |= timeout;
                    }
                    tight_loop_contents();
                } while (!timeout && !(i2c->hw->raw_intr_stat & I2C_IC_RAW_INTR_STAT_STOP_DET_BITS));

                // If there was a timeout, don't attempt to do anything else. / タイムアウトが発生した場合は、他の処理を試行しません。
                if (!timeout) {
                    i2c->hw->clr_stop_det;
                }
            }
        }

        // Note the hardware issues a STOP automatically on an abort condition. / ハードウェアは中断条件で自動的にSTOPを発行することに注意してください。
        // Note also the hardware clears RX FIFO as well as TX on abort, / また、hwparam IC_AVOID_RX_FIFO_FLUSH_ON_TX_ABRTを0に設定しているため、
        // because we set hwparam IC_AVOID_RX_FIFO_FLUSH_ON_TX_ABRT to 0. / 中断時にハードウェアがTXだけでなくRX FIFOもクリアすることに注意してください。
        if (abort)
            break;
    }

    int rval;

    // A lot of things could have just happened due to the ingenious and / I2Cの独創的で創造的な設計により、多くのことが起こった可能性があります。
    // creative design of I2C. Try to figure things out. / 状況を把握してみてください。
    if (abort) {
        if (timeout)
            rval = PICO_ERROR_TIMEOUT;
        else if (!abort_reason || abort_reason & I2C_IC_TX_ABRT_SOURCE_ABRT_7B_ADDR_NOACK_BITS) {
            // No reported errors - seems to happen if there is nothing connected to the bus. / 報告されたエラーはありません - バスに何も接続されていない場合に発生するようです。
            // Address byte not acknowledged / アドレスバイトが応答されませんでした
            rval = PICO_ERROR_GENERIC;
        } else if (abort_reason & I2C_IC_TX_ABRT_SOURCE_ABRT_TXDATA_NOACK_BITS) {
            // Address acknowledged, some data not acknowledged / アドレスは応答されましたが、一部のデータが応答されませんでした
            rval = byte_ctr;
        } else {
#if 0           
            panic("Unknown abort from I2C instance @%08x: %08x\n", (uint32_t) i2c->hw, abort_reason);
#endif            
            rval = PICO_ERROR_GENERIC;
        }
    } else {
        rval = byte_ctr;
    }

    // nostop means we are now at the end of a *message* but not the end of a *transfer* / nostopは、現在が「メッセージの終わり」であり、「転送の終わり」ではないことを意味します
    i2c->restart_on_next = nostop;
    return rval;
}

// I2C receive 1 byte at a time / 1byteずつI2C受信
#if 0
int i2c_read_blocking_internal(i2c_inst_t *i2c, uint8_t addr, uint8_t *dst, bool nostop,
                               check_timeout_fn timeout_check, timeout_state_t *ts) {
#endif
static int i2c_read_byte_separately(i2c_inst_t *i2c, uint8_t addr, uint8_t *dst, bool nostop, bool first, bool last) {                                
#if 0
    invalid_params_if(I2C, addr >= 0x80); // 7-bit addresses / 7ビットアドレス
    invalid_params_if(I2C, i2c_reserved_addr(addr));
    invalid_params_if(I2C, len == 0);
    invalid_params_if(I2C, ((int)len) < 0);
#endif

    if (first) {
        i2c->hw->enable = 0;
        i2c->hw->tar = addr;
        i2c->hw->enable = 1;
    }

    bool abort = false;
    bool timeout = false;
    uint32_t abort_reason;
    int byte_ctr;

    volatile uint64_t startUs, currentUs, diffUs;
#if 0  
    int ilen = (int)len;
#endif
    int ilen = 1;
    for (byte_ctr = 0; byte_ctr < ilen; ++byte_ctr) {
#if 0
        bool first = byte_ctr == 0;
        bool last = byte_ctr == ilen - 1;
#endif
        while (!i2c_get_write_available(i2c))
            tight_loop_contents();

        i2c->hw->data_cmd =
                bool_to_bit(first && i2c->restart_on_next) << I2C_IC_DATA_CMD_RESTART_LSB |
                bool_to_bit(last && !nostop) << I2C_IC_DATA_CMD_STOP_LSB |
                I2C_IC_DATA_CMD_CMD_BITS; // -> 1 for read / -> 読み取りの場合は1

        startUs = time_us_64();
        do {
            abort_reason = i2c->hw->tx_abrt_source;
            if (i2c->hw->raw_intr_stat & I2C_IC_RAW_INTR_STAT_TX_ABRT_BITS) {
                abort = true;
                i2c->hw->clr_tx_abrt;
            }
#if 0
            if (timeout_check) {
                timeout = timeout_check(ts);
                abort |= timeout;
            }
#endif
            currentUs = time_us_64();    
            diffUs = currentUs - startUs;
            if (diffUs >= I2C_TIMEOUT) {
                timeout = true;
                abort |= timeout;
            }
        } while (!abort && !i2c_get_read_available(i2c));

        if (abort)
            break;

        *dst++ = (uint8_t) i2c->hw->data_cmd;
    }

    int rval;

    if (abort) {
        if (timeout)
            rval = PICO_ERROR_TIMEOUT;
        else if (!abort_reason || abort_reason & I2C_IC_TX_ABRT_SOURCE_ABRT_7B_ADDR_NOACK_BITS) {
            // No reported errors - seems to happen if there is nothing connected to the bus. / 報告されたエラーはありません - バスに何も接続されていない場合に発生するようです。
            // Address byte not acknowledged / アドレスバイトが応答されませんでした
            rval = PICO_ERROR_GENERIC;
        } else {
#if 0            
            panic("Unknown abort from I2C instance @%08x: %08x\n", (uint32_t) i2c->hw, abort_reason);
#endif            
            rval = PICO_ERROR_GENERIC;
        }
    } else {
        rval = byte_ctr;
    }

    i2c->restart_on_next = nostop;
    return rval;
}