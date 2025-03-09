// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define]
#define I2C_ID i2c1 // I2CのID
#define TIMER_I2C_TIMEOUT 100000ULL // 100ms I2C送信/受信タイムアウト(us)

// デフォルト値
#define I2C_DEFAULT_FREQ 100000 // クロック周波数 100kHz

// [ファイルスコープ変数]
static USHORT f_dataSize = 0; // 送信/受信済みサイズ
static ST_I2C_REQ f_stI2cReq = {0}; // I2C送信/受信要求

// [関数プロトタイプ宣言]
static int i2c_write_byte_separately(i2c_inst_t *i2c, uint8_t addr, const uint8_t *src, bool nostop, bool first, bool last);
static int i2c_read_byte_separately(i2c_inst_t *i2c, uint8_t addr, uint8_t *dst, bool nostop, bool first, bool last);

// I2Cメイン処理
void I2C_Main()
{
    int size = 0;
    ULONG errorBit = 0;
    bool bNoStop = true;
    bool bFirst = false;
    bool bLast = false;
   
    if (f_stI2cReq.dataSize > 0) { 
        // I2C送信/受信の残りbyte数がある場合
        
        if (!f_dataSize) { // 送信/受信済みサイズ = 0の場合
            bFirst = true;
        }    
        if (1 == f_stI2cReq.dataSize) { // I2C送信/受信の残りのbyte数 = 1の場合
            bNoStop = false;
            bLast = true;    
        }

        // 1byte目: bNoStop=true,  bFirst=true,  bLast=false
        // 中間:    bNoStop=true,  bFirst=false, bLast=false
        // 最終byte:bNoStop=false, bFirst=false, bLast=true

        if (CMD_RECV_I2C == f_stI2cReq.cmd) { // I2C受信
            // 非同期っぽくするために1byteずつ通信
            // 1byteのI2C受信
            size = i2c_read_byte_separately(I2C_ID, f_stI2cReq.slaveAddr, &f_stI2cReq.aData[f_dataSize], bNoStop, bFirst, bLast);
        }
        else { // I2C送信
            // 非同期っぽくするために1byteずつ通信
            // 1byteのI2C送信
            size = i2c_write_byte_separately(I2C_ID, f_stI2cReq.slaveAddr, &f_stI2cReq.aData[f_dataSize], bNoStop, bFirst, bLast);
        }

        if (1 == size) { // 1byteのI2C送信/受信が成功した場合

            f_dataSize++; // 送信/受信済みサイズ+1
            f_stI2cReq.dataSize--; // I2C送信/受信の残りbyte数-1
            if (!f_stI2cReq.dataSize) { // 最後まで送信/受信した場合
                // 成功の応答フレームを送信       
                FRM_MakeAndSendResFrm(f_stI2cReq.seqNo, f_stI2cReq.cmd, FRM_ERR_SUCCESS, f_dataSize, f_stI2cReq.aData);
                f_dataSize = 0; // 送信/受信済みサイズ=0
            } 
        }
        else { // 送信/受信に失敗した場合
            // FWエラーを設定
            if (PICO_ERROR_TIMEOUT == size) {
                errorBit = CMN_ERR_I2C_TIMEOUT;
            }
            else {
                errorBit = CMN_ERR_I2C_NO_DEVICE;
            }
            CMN_SetErrorBits(errorBit, true);
            // 失敗の応答フレームを送信        
            FRM_MakeAndSendResFrm(f_stI2cReq.seqNo, f_stI2cReq.cmd, FRM_ERR_I2C_NO_DEVICE, 0, NULL); 
            f_dataSize = 0;          // 送信/受信済みサイズ=0
            f_stI2cReq.dataSize = 0; // I2C送信/受信の残りbyte数=0                 
        }
    }
    else { 
        // I2C送信/受信の残りbyte数が無い場合
        
        // I2C送信/受信要求のデキュー
        if (CMN_Dequeue(CMN_QUE_KIND_I2C_REQ, &f_stI2cReq, sizeof(f_stI2cReq), true)) {
            // 無処理
        }
    }
}

// ST_I2C_CONFIG構造体にデフォルト値を格納
void I2C_SetDefault(ST_I2C_CONFIG *pstConfig)
{
    pstConfig->frequency = I2C_DEFAULT_FREQ;
}

// I2Cを初期化
void I2C_Init(ST_I2C_CONFIG *pstConfig)
{
    // [I2C1(マスタ)を初期化]
    // SDA
    gpio_init(I2C_SDA);
    gpio_set_function(I2C_SDA, GPIO_FUNC_I2C);
    gpio_pull_up(I2C_SDA);
    // SCL
    gpio_init(I2C_SCL);
    gpio_set_function(I2C_SCL, GPIO_FUNC_I2C);
    gpio_pull_up(I2C_SCL);
    // クロック周波数
    i2c_init(I2C_ID, pstConfig->frequency);   
}

// 1byteずつI2C送信
#if 0
int i2c_write_blocking_internal(i2c_inst_t *i2c, uint8_t addr, const uint8_t *src, bool nostop,
                                       check_timeout_fn timeout_check, struct timeout_state *ts) {
#endif
static int i2c_write_byte_separately(i2c_inst_t *i2c, uint8_t addr, const uint8_t *src, bool nostop, bool first, bool last) {
#if 0
    invalid_params_if(I2C, addr >= 0x80); // 7-bit addresses
    invalid_params_if(I2C, i2c_reserved_addr(addr));
    // Synopsys hw accepts start/stop flags alongside data items in the same
    // FIFO word, so no 0 byte transfers.
    invalid_params_if(I2C, len == 0);
    invalid_params_if(I2C, ((int)len) < 0);
#endif

    i2c->hw->enable = 0;
    i2c->hw->tar = addr;
    i2c->hw->enable = 1;

    bool abort = false;
    bool timeout = false;

    uint32_t abort_reason = 0;
    int byte_ctr;

    uint64_t startUs, currentUs, diffUs;
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

        // Wait until the transmission of the address/data from the internal
        // shift register has completed. For this to function correctly, the
        // TX_EMPTY_CTRL flag in IC_CON must be set. The TX_EMPTY_CTRL flag
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
            if (diffUs >= TIMER_I2C_TIMEOUT) {
                timeout = true;
                abort |= timeout;
            }
            tight_loop_contents();
        } while (!timeout && !(i2c->hw->raw_intr_stat & I2C_IC_RAW_INTR_STAT_TX_EMPTY_BITS));

        // If there was a timeout, don't attempt to do anything else.
        if (!timeout) {
            abort_reason = i2c->hw->tx_abrt_source;
            if (abort_reason) {
                // Note clearing the abort flag also clears the reason, and
                // this instance of flag is clear-on-read! Note also the
                // IC_CLR_TX_ABRT register always reads as 0.
                i2c->hw->clr_tx_abrt;
                abort = true;
            }

            if (abort || (last && !nostop)) {
                // If the transaction was aborted or if it completed
                // successfully wait until the STOP condition has occured.

                // TODO Could there be an abort while waiting for the STOP
                // condition here? If so, additional code would be needed here
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
                if (diffUs >= TIMER_I2C_TIMEOUT) {
                        timeout = true;
                        abort |= timeout;
                    }
                    tight_loop_contents();
                } while (!timeout && !(i2c->hw->raw_intr_stat & I2C_IC_RAW_INTR_STAT_STOP_DET_BITS));

                // If there was a timeout, don't attempt to do anything else.
                if (!timeout) {
                    i2c->hw->clr_stop_det;
                }
            }
        }

        // Note the hardware issues a STOP automatically on an abort condition.
        // Note also the hardware clears RX FIFO as well as TX on abort,
        // because we set hwparam IC_AVOID_RX_FIFO_FLUSH_ON_TX_ABRT to 0.
        if (abort)
            break;
    }

    int rval;

    // A lot of things could have just happened due to the ingenious and
    // creative design of I2C. Try to figure things out.
    if (abort) {
        if (timeout)
            rval = PICO_ERROR_TIMEOUT;
        else if (!abort_reason || abort_reason & I2C_IC_TX_ABRT_SOURCE_ABRT_7B_ADDR_NOACK_BITS) {
            // No reported errors - seems to happen if there is nothing connected to the bus.
            // Address byte not acknowledged
            rval = PICO_ERROR_GENERIC;
        } else if (abort_reason & I2C_IC_TX_ABRT_SOURCE_ABRT_TXDATA_NOACK_BITS) {
            // Address acknowledged, some data not acknowledged
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

    // nostop means we are now at the end of a *message* but not the end of a *transfer*
    i2c->restart_on_next = nostop;
    return rval;
}

// 1byteずつI2C受信
#if 0
int i2c_read_blocking_internal(i2c_inst_t *i2c, uint8_t addr, uint8_t *dst, bool nostop,
                               check_timeout_fn timeout_check, timeout_state_t *ts) {
#endif
static int i2c_read_byte_separately(i2c_inst_t *i2c, uint8_t addr, uint8_t *dst, bool nostop, bool first, bool last) {                                
#if 0
    invalid_params_if(I2C, addr >= 0x80); // 7-bit addresses
    invalid_params_if(I2C, i2c_reserved_addr(addr));
    invalid_params_if(I2C, len == 0);
    invalid_params_if(I2C, ((int)len) < 0);
#endif

    i2c->hw->enable = 0;
    i2c->hw->tar = addr;
    i2c->hw->enable = 1;

    bool abort = false;
    bool timeout = false;
    uint32_t abort_reason;
    int byte_ctr;

    uint64_t startUs, currentUs, diffUs;
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
                I2C_IC_DATA_CMD_CMD_BITS; // -> 1 for read

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
            if (diffUs >= TIMER_I2C_TIMEOUT) {
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
            // No reported errors - seems to happen if there is nothing connected to the bus.
            // Address byte not acknowledged
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