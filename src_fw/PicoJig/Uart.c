// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義]
#define UART_ID  uart0      // UART ID / UARTのID
#define UART_IRQ UART0_IRQ  // UART IRQ / UARTのIRQ

// Default values / デフォルト値
#define UART_DEFAULT_BAUD_RATE 9600             // Baud rate / ボーレート
#define UART_DEFAULT_DATA_BITS 8                // Data bit length / データビット長
#define UART_DEFAULT_STOP_BITS 1                // Stop bit length / ストップビット長
#define UART_DEFAULT_PARITY    UART_PARITY_NONE // Parity / パリティ

// [File scope variables] / [ファイルスコープ変数]
static volatile bool f_isSentFirstByte = false; // Whether the 1st byte has been sent or not / 1byte目を送信済みか否か
static UCHAR f_aNotifyData[UART_DATA_MAX_SIZE] = {0}; // Data part of UART receive notification frame / UART受信通知フレームのデータ部

// [Function prototype declarations] / [関数プロトタイプ宣言]
static void UART_Interrupt();
static void UART_Recv();
static void UART_SendFirstByte();
static inline bool UART_Send();

// UART interrupt / UART割り込み
static void UART_Interrupt() 
{
    io_rw_32 dr;  // Data Register:UARTDR
    io_rw_32 mis = uart_get_hw(UART_ID)->mis; // Masked interrupt status register / マスク済み割り込みステータスレジスタ
    UCHAR errFlags; // UART error flags / UARTエラーフラグ
	ULONG errorBits = 0;   

    if (mis & (UART_UARTMIS_RXMIS_BITS | UART_UARTMIS_RTMIS_BITS)) {
        // In case of UART receive interrupt or receive timeout interrupt / UART受信割り込み、または受信タイムアウト割り込みの場合

        while (uart_is_readable(UART_ID)) { // If UART receive data still exists / UARTの受信データがまだ存在する場合
            // Extract UART receive data (1 byte) and error flags simultaneously / UARTの受信データ(1byte)とエラーフラグを同時に取り出し
            dr = uart_get_hw(UART_ID)->dr;
            errFlags = (dr >> 8) & 0x0F; // Extract error flags directly from bits 8-11 of dr / drの8～11ビット目からエラーフラグを直接抽出

            if (errFlags) {
                // Clear if an error has occurred (writing any value clears all errors) / エラーが発生している場合はクリア(任意の値を書き込むことで全エラーがクリアされる仕様)
                uart_get_hw(UART_ID)->rsr = 0;
            }
  
            if (!errFlags) { // If no UART error occurred / UARTエラーが発生していない場合
                // Extract lower 8 bits of data / 下位8ビットのデータを抽出
                UCHAR rxData = (UCHAR)(dr & 0xFF);
                // Enqueue 1 byte of UART receive data / UART受信データ1byteのエンキュー
                (void)CMN_Enqueue(CMN_QUE_KIND_UART_RECV, (PVOID)&rxData, true);
            }

            if (TMR_IsStabilizationWaitTimePassed()) { 
                // If stabilization wait time after boot has passed / 起動してからの安定待ち時間が経過していた場合

                // Set FW error / FWエラーを設定
                if (errFlags & (1 << 0)) {
                    errorBits |= CMN_ERR_BIT_UART_FRAMING_ERR;
                }        
                if (errFlags & (1 << 1)) {
                    errorBits |= CMN_ERR_BIT_UART_PARITY_ERR;
                } 
                if (errFlags & (1 << 2)) {
                    errorBits |= CMN_ERR_BIT_UART_BREAK_ERR;
                } 
                if (errFlags & (1 << 3)) {
                    errorBits |= CMN_ERR_BIT_UART_OVERRUN_ERR;
                }                         
            }
        }
        // Set FW error / FWエラーを設定
        CMN_SetErrorBits(errorBits, true);
    }

    if (mis & UART_UARTMIS_TXMIS_BITS) {
        // In case of UART send interrupt / UART送信割り込みの場合
   
        // UART send of the next 1 byte / 次の1byteのUART送信
        if (!UART_Send()) { 
            // If the UART send data queue is empty (no more UART send data) / UART送信データのキューが空の場合(UART送信データがもう無い場合) 
            
            f_isSentFirstByte = false; // 1st byte has not been sent yet / 1byte目は未送信   

            // Clear UART send interrupt / UART送信割り込みをクリア
            uart_get_hw(UART_ID)->icr = UART_UARTICR_TXIC_BITS ;

            // -------------------------------------------------------------
            // [Remarks] * Specifications when FIFO is disabled (depth is 1) / [備考] ※FIFOが無効(深さが1)の場合の仕様
            //
            // - UART send interrupt (UARTTXINTR) / ・UART送信割り込み (UARTTXINTR)
            //   If there is no send data in the send FIFO, the send interrupt is asserted HIGH. / 送信FIFOに送信データが存在しない場合は、送信割り込みが HIGH にアサートされる。
            //   Performing a single write to the send FIFO or clearing the interrupt will clear the send interrupt. / 送信FIFOに1回の書き込みを実行するか、割り込みをクリアすると、送信割り込みがクリアされる。
            //
            // - UART receive interrupt (UARTRXINTR) / ・UART受信割り込み (UARTRXINTR)
            //   Cleared just by reading the receive FIFO. / 受信FIFOの読み取りだけでクリアされる。
            // -------------------------------------------------------------
        } 
    }
}


// UART main processing / UARTメイン処理
void UART_Main()
{
    // Extract UART receive data ⇒ USB/wireless send (send UART receive notification frame) / UART受信データ取り出し⇒USB/無線送信(UART受信通知フレームの送信)
    UART_Recv();
    // Send 1st byte of UART / 1byte目のUART送信
    UART_SendFirstByte(); 
}

// Extract UART receive data ⇒ USB/wireless send (send UART receive notification frame) / UART受信データ取り出し⇒USB/無線送信(UART受信通知フレームの送信)
static void UART_Recv()
{
    UCHAR data;
    ULONG i;
    ULONG size;

    for (i = 0; i < UART_DATA_MAX_SIZE; i++) { 
        // Dequeue 1 byte of UART receive data / UART受信データ1byteのデキュー
        if (CMN_Dequeue(CMN_QUE_KIND_UART_RECV, &data, true)) { 
            f_aNotifyData[i] = data;  
        }
        else {
            break;
        }
    }
    size = i;
    if (size > 0) {
        // Send UART receive notification frame / UART受信通知フレームの送信
        FRM_SendNotifyFrm(FRM_HEADER_NOTIFY_UART_RECV, size, f_aNotifyData);
    }
}

// Send 1st byte of UART / 1byte目のUART送信
static void UART_SendFirstByte()
{
    if (!f_isSentFirstByte) {  // If 1st byte has not been sent yet / 1byte目をまだ送信していない場合
        if (uart_is_writable(UART_ID)) { // If UART send is possible / UART送信可能な場合
            // Extract UART send data ⇒ UART send / UART送信データ取り出し⇒UART送信
            (void)UART_Send();
        }
    }
}

// Extract UART send data ⇒ UART send / UART送信データ取り出し⇒UART送信
static inline bool UART_Send()
{
    UCHAR data;
    bool isSent = false; // Whether sent or not / 送信したか否か

    // Dequeue 1 byte of UART send data / UART送信データ1byteのデキュー
    if (CMN_Dequeue(CMN_QUE_KIND_UART_SEND, &data, true)) {   
        f_isSentFirstByte = true; // 1 byte has been sent / 1byteを送信済み
        // UART send (1 byte) / UART送信(1byte)
        uart_get_hw(UART_ID)->dr = (io_rw_32)data;
        isSent = true;
    }

    return isSent;
}

// Store default values in ST_UART_CONFIG structure / ST_UART_CONFIG構造体にデフォルト値を格納
void UART_SetDefault(ST_UART_CONFIG *pstConfig)
{
    pstConfig->baudrate = UART_DEFAULT_BAUD_RATE; // Baud rate / ボーレート
    pstConfig->dataBits = UART_DEFAULT_DATA_BITS; // Data bit length / データビット長
    pstConfig->stopBits = UART_DEFAULT_STOP_BITS; // Stop bit length / ストップビット長
    pstConfig->parity   = UART_DEFAULT_PARITY;    // Parity / パリティ
}

// Initialize UART / UARTを初期化
void UART_Init(ST_UART_CONFIG *pstConfig)
{
    // Initialize UART / UARTを初期化
    uart_init(UART_ID, pstConfig->baudrate);  // Baud rate / ボーレート
    // Set pin function / ピンの機能設定
    gpio_set_function(UART_TX, GPIO_FUNC_UART);
    gpio_set_function(UART_RX, GPIO_FUNC_UART);
    // Disable CTS/RTS / CTS/RTSを無効に設定
    uart_set_hw_flow(UART_ID, false, false);
    // Communication config / 通信設定
    uart_set_format(UART_ID, pstConfig->dataBits, pstConfig->stopBits, pstConfig->parity);
    // Disable UART FIFO according to sample program / サンプルプログラムに合わせてUARTのFIFOを無効に設定
    uart_set_fifo_enabled(UART_ID, false);
    // Interrupt config / 割り込み設定
    irq_set_exclusive_handler(UART_IRQ, UART_Interrupt);
    irq_set_priority(UART_IRQ, CMN_IRQ_PRIORITY_UART); // Interrupt priority / 割り込みの優先度
    irq_set_enabled(UART_IRQ, true); // Enable specified interrupt on the executing CPU core / 実行中のCPUコア上の指定の割り込みを有効   
    uart_set_irq_enables(UART_ID, true, true); // Enable UART RX/TX interrupts / UARTのRX・TX割り込みを有効
}
