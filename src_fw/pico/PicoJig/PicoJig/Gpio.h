#ifndef GPIO_H
#define GPIO_H

#include "Common.h"

// [define]
// GP番号
#define GP_00 0     // UART0 TX
#define GP_01 1     // UART0 RX
#define GP_02 2     // PWM OUT
#define GP_03 3     // GPIO
#define GP_04 4     // GPIO
#define GP_05 5     // GPIO
#define GP_06 6     // I2C1 SDA
#define GP_07 7     // I2C1 SCL
#define GP_08 8     // GPIO
#define GP_09 9     // GPIO
#define GP_10 10    // GPIO
#define GP_11 11    // GPIO
#define GP_12 12    // GPIO
#define GP_13 13    // GPIO
#define GP_14 14    // GPIO
#define GP_15 15    // GPIO
#define GP_16 16    // SPI RX   
#define GP_17 17    // SPI CSN 
#define GP_18 18    // SPI SCK
#define GP_19 19    // SPI TX
#define GP_20 20    // GPIO
#define GP_21 21    // GPIO
#define GP_22 22    // GPIO
#define GP_25 25    // ONBOARD LED
#define GP_26 26    // ADC0
#define GP_27 27    // ADC1
#define GP_28 28    // ADC2

// ピン(GP番号)の機能選択
#define UART_TX     GP_00
#define UART_RX     GP_01
#define PWM_OUT     GP_02
#define I2C_SDA     GP_06
#define I2C_SCL     GP_07
#define SPI_RX      GP_16
#define SPI_CSN     GP_17
#define SPI_SCK     GP_18
#define SPI_TX      GP_19
#define ONBOARD_LED GP_25
#define ADC0        GP_26    
#define ADC1        GP_27
#define ADC2        GP_28

#pragma pack(1)

// [構造体]
// GPIOのGP番号と方向
typedef struct _ST_GPIO_PIN{
    ULONG gp;  // GP番号
    bool  dir; // true:出力 false:入力
} ST_GPIO_PIN;

// GPIO設定
typedef struct _ST_GPIO_CONFIG {
   ULONG pullDownBits;   // プルダウンかプルアップか 
   ULONG initialValBits; // 電源ON時出力値
} ST_GPIO_CONFIG;

#pragma pack()

// [関数プロトタイプ宣言]
ULONG GPIO_GetInDirBits();
ULONG GPIO_GetOutDirBits();
void GPIO_SetDefault(ST_GPIO_CONFIG *pstConfig);
void GPIO_Init(ST_GPIO_CONFIG *pstConfig);

#endif