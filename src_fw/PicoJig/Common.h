// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef COMMON_H
#define COMMON_H

#include <stdio.h>
#include <string.h>
#include <stddef.h>
#include "pico/stdlib.h"
#include "pico/binary_info.h"
#include "hardware/gpio.h"
#include "hardware/adc.h"
#include "hardware/uart.h"
#include "hardware/spi.h"
#include "hardware/dma.h"
#include "hardware/i2c.h"
#include "pico/i2c_slave.h"
#include "hardware/pwm.h"
#include "pico/multicore.h"
#include "hardware/flash.h"
#include "class/cdc/cdc_device.h"
#include "pico/unique_id.h"
#include "hardware/pll.h"
#include "hardware/clocks.h"
#include "hardware/structs/pll.h"
#include "hardware/structs/clocks.h"
#include "hardware/watchdog.h"
#include "hardware/resets.h"
#include "pico/bootrom.h"
#include "hardware/exception.h"

#ifdef MY_BOARD_PICO_W
#include "pico/cyw43_arch.h"
#include "lwip/pbuf.h"
#include "lwip/tcp.h"
#endif

#include "Type.h"
#include "Ver.h"
#include "Frame.h"
#include "Gpio.h"
#include "Adc.h"
#include "Uart.h"
#include "Spi.h"
#include "I2c.h"
#include "Pwm.h"
#include "TcpServer.h"
#include "Flash.h"
#include "Timer.h"
#include "Cmd.h"

// [define] / [定義]
// Interrupt priority / 割り込みの優先度
#define CMN_IRQ_PRIORITY_UART 0 // UART (highest priority) / UART(最優先)

// Queue size / キューサイズ
#define CMN_QUE_DATA_MAX_UART_SEND      1024 // Max queue data for UART send / UART送信キューの最大データ数
#define CMN_QUE_DATA_MAX_UART_RECV      1024 // Max queue data for UART receive / UART受信キューの最大データ数
#define CMN_QUE_DATA_MAX_I2C_REQ        8       // Number of ST_I2C_REQ structures. Byte count of I2C data per ST_I2C_REQ is I2C_DATA_MAX_SIZE / ST_I2C_REQ構造体の数。ST_I2C_REQ 1個につきI2Cデータのbyte数はI2C_DATA_MAX_SIZE
#define CMN_QUE_DATA_MAX_USB_WL_SEND    2048 // Max queue data for USB/wireless send / USB/無線送信キューの最大データ数
#define CMN_QUE_DATA_MAX_WL_RECV        1024 // Max queue data for wireless receive / 無線受信キューの最大データ数

// FW error bits / FWエラービット
#define CMN_ERR_BIT_WDT_RESET                       (1 << 0)  // MCU reset due to WDT timeout / WDTタイムアウトでマイコンがリセットした
#define CMN_ERR_BIT_UART_FRAMING_ERR                (1 << 1)  // UART:Framing error     
#define CMN_ERR_BIT_UART_PARITY_ERR                 (1 << 2)  // UART:Parity error
#define CMN_ERR_BIT_UART_BREAK_ERR                  (1 << 3)  // UART:Break error
#define CMN_ERR_BIT_UART_OVERRUN_ERR                (1 << 4)  // UART:Overrun error
#define CMN_ERR_BIT_I2C_NO_DEVICE                   (1 << 5)  // I2C: address not acknowledged, or, no device present. (Meaning of PICO_ERROR_GENERIC) / I2C: アドレスが応答されない、またはデバイスが存在しない (PICO_ERROR_GENERICの意味)
#define CMN_ERR_BIT_I2C_TIMEOUT                     (1 << 6)  // I2C communication timeout / I2C通信でタイムアウト
#define CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_USB_WL_SEND (1 << 7)  // Request data discarded due to lack of buffer space (USB/wireless send) / バッファに空きがないので要求データを破棄した(USB/無線送信)
#define CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_UART_SEND   (1 << 8)  // Request data discarded due to lack of buffer space (UART send) / バッファに空きがないので要求データを破棄した(UART送信)
#define CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_UART_RECV   (1 << 9)  // Request data discarded due to lack of buffer space (UART receive) / バッファに空きがないので要求データを破棄した(UART受信)
#define CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_I2C_REQ     (1 << 10) // Request data discarded due to lack of buffer space (I2C send/receive) / バッファに空きがないので要求データを破棄した(I2C送信/受信)
#define CMN_ERR_BIT_BUF_SIZE_NOT_ENOUGH_WL_RECV     (1 << 11) // Request data discarded due to lack of buffer space (wireless receive) / バッファに空きがないので要求データを破棄した(無線受信)
#define CMN_ERR_BIT_WL_SEND_ERR                     (1 << 12) // Wireless send failed / 無線送信が失敗した

// [Enums] / [列挙体]
// Queue type / キューの種類
typedef enum _E_CMN_QUE_KIND { 
    CMN_QUE_KIND_USB_WL_SEND = 0,   // USB/wireless send / USB/無線送信
    CMN_QUE_KIND_UART_SEND,         // UART send / UART送信
    CMN_QUE_KIND_UART_RECV,         // UART receive / UART受信 
    CMN_QUE_KIND_I2C_REQ,           // I2C master send/receive / I2Cマスタ送信/受信
    CMN_QUE_KIND_WL_RECV,           // Wireless receive / 無線受信
    CMN_QUE_KIND_NUM                // Number of queue types / キューの種類の数
} E_CMN_QUE_KIND;

// [Structs] / [構造体]
// Queue / キュー
typedef struct _ST_QUE {
    ULONG head; // Head / 先頭
    ULONG tail; // Tail / 末尾
    ULONG max;  // Number of elements in the queue data array / キューのデータ配列の要素数
    PVOID pBuf; // Pointer to the queue data array / キューのデータ配列へのポインタ
} ST_QUE;

// [Function prototype declarations] / [関数プロトタイプ宣言]
bool CMN_Enqueue(ULONG iQue, PVOID pData, bool bSpinLock);
bool CMN_Dequeue(ULONG iQue, PVOID pData, bool bSpinLock);
void CMN_EntrySpinLock();
void CMN_ExitSpinLock();
void CMN_SetErrorBits(ULONG errorBits, bool bSpinLock);
ULONG CMN_GetFwErrorBits();
void CMN_ClearFwErrorBits(bool bSpinLock);
bool CMN_Checksum(PVOID pBuf, USHORT expect, ULONG size);
USHORT CMN_CalcChecksum(PVOID pBuf, ULONG size);
void CMN_WdtEnableReboot();
void CMN_WdtRebootWithoutEnable();
void CMN_Init();

#endif
