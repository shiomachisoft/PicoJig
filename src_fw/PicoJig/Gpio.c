// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [File scope variables] / [ファイルスコープ変数]
// GPIO GP number and direction / GPIOのGP番号と方向
static const ST_GPIO_PIN f_castGpioPin[] = {
    // false: input, true: output / false:入力 true:出力
    { GP_03, false },      // Input / 入力  
    { GP_04, false },      // Input / 入力
    { GP_05, false },      // Input / 入力
    { GP_08, false },      // Input / 入力
    { GP_09, false },      // Input / 入力
    { GP_10, false },      // Input / 入力
    { GP_11, false },      // Input / 入力
    { GP_12, true },       // Output / 出力
    { GP_13, true },       // Output / 出力
    { GP_14, true },       // Output / 出力
    { GP_15, true },       // Output / 出力
    { GP_20, true },       // Output / 出力
    { GP_21, true },       // Output / 出力
    { GP_22, true },       // Output / 出力
};

static ULONG f_gpioInDirBits = 0;  // GPIO input bits / GPIO入力のビット
static ULONG f_gpioOutDirBits = 0; // GPIO output bits / GPIO出力のビット

// Get GPIO input bits / GPIO入力のビットを取得
ULONG GPIO_GetInDirBits()
{
    return f_gpioInDirBits;
}

// Get GPIO output bits / GPIO出力のビットを取得
ULONG GPIO_GetOutDirBits()
{
    return f_gpioOutDirBits;
}

// Store default values in ST_GPIO_CONFIG structure / ST_GPIO_CONFIG構造体にデフォルト値を格納
void GPIO_GetDefaultConfig(ST_GPIO_CONFIG *pstConfig)
{
    pstConfig->pullDownBits   = 0; // All GPIO inputs are pull-up / 全てのGPIO入力はプルアップ
    pstConfig->initialOutValBits = 0; // Output value of all GPIO outputs at power-on is OFF / 全てのGPIO出力の電源ON時出力値=OFF 
}

// Initialize GPIO / GPIOを初期化
void GPIO_Init(ST_GPIO_CONFIG *pstConfig)
{
    bool dir;        // Input/output direction / 入出力方向
    bool initialOutVal; // Output value at power-on / 電源ON時出力値
    ULONG gp;        // GP number / GP番号
    ULONG i;

    // [Set onboard LED] / [オンボードLEDの設定]
#ifndef MY_BOARD_PICO_W
    gp = ONBOARD_LED;
    // Initialize GPIO / GPIOの初期化
    gpio_init(gp);    
    // Set to output direction / 出力方向に設定
    gpio_set_dir(gp, true); 
#endif

    // Initialize bitmask variables / ビットマスク変数を初期化
    f_gpioOutDirBits = 0;
    f_gpioInDirBits = 0;

    // [Set GPIO pins] / [GPIOピンの設定]
    for (i = 0; i < sizeof(f_castGpioPin) / sizeof(f_castGpioPin[0]); i++) {
        gp  = f_castGpioPin[i].gp;  // GP number / GP番号
        dir = f_castGpioPin[i].dir; // Direction / 方向

        // Initialize GPIO / GPIOの初期化
        gpio_init(gp); 

        if (dir) { 
            // If output / 出力の場合    

            // Determine output value at power-on / 電源ON時出力値を決定
            if (pstConfig->initialOutValBits & (1UL << gp)) {
                initialOutVal = true;
            }
            else {
                initialOutVal = false;
            }
            
            // Output value at power-on / 電源ON時出力値を出力 
            gpio_put(gp, initialOutVal);
            f_gpioOutDirBits |= (1UL << gp); 
        }
        else { 
            // If input / 入力の場合
            
            // Set pull-down/pull-up / プルダウン/プルアップの設定
            if (pstConfig->pullDownBits & (1UL << gp)) {
                gpio_pull_down(gp); // Pull-down / プルダウン
            }
            else {
                gpio_pull_up(gp); // Pull-up / プルアップ
            }              
            f_gpioInDirBits |= (1UL << gp);
        }

        // Set input/output direction / 入出力方向を設定
        gpio_set_dir(gp, dir);
    }
}
