// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [ファイルスコープ変数]
// GPIOのGP番号と方向
static const ST_GPIO_PIN f_castGpioPin[] = {
    // false:入力 true:出力
    { GP_03, false },      // 入力  
    { GP_04, false },      // 入力
    { GP_05, false },      // 入力
    { GP_08, false },      // 入力
    { GP_09, false },      // 入力
    { GP_10, false },      // 入力
    { GP_11, false },      // 入力
    { GP_12, true },       // 出力
    { GP_13, true },       // 出力
    { GP_14, true },       // 出力
    { GP_15, true },       // 出力
    { GP_20, true },       // 出力
    { GP_21, true },       // 出力
    { GP_22, true },       // 出力
};

static ULONG f_gpioInDirBits = 0;  // GPIO入力のビット
static ULONG f_gpioOutDirBits = 0; // GPIO出力のビット

// GPIO入力のビットを取得
ULONG GPIO_GetInDirBits()
{
    return f_gpioInDirBits;
}

// GPIO出力のビットを取得
ULONG GPIO_GetOutDirBits()
{
    return f_gpioOutDirBits;
}

// ST_GPIO_CONFIG構造体にデフォルト値を格納
void GPIO_SetDefault(ST_GPIO_CONFIG *pstConfig)
{
    pstConfig->pullDownBits   = 0; // 全てのGPIO入力はプルアップ
    pstConfig->initialValBits = 0; // 全てのGPIO出力の電源ON時出力値=OFF 
}

// GPIOを初期化
void GPIO_Init(ST_GPIO_CONFIG *pstConfig)
{
    bool dir;        // 入出力方向
    bool initialVal; // 電源ON時出力値
    ULONG gp;        // GP番号
    ULONG i;

    // [オンボードLEDの設定]
#ifndef WL   
    gp = ONBOARD_LED;
    // GPIOの初期化
    gpio_init(gp);   
    // 出力値に電源ON時出力値を設定       　
    gpio_put(gp, true);     
    // 出力方向に設定
    gpio_set_dir(gp, true); 
#endif

    // [GPIOピンの設定]
    for (i = 0; i < sizeof(f_castGpioPin) / sizeof(ST_GPIO_PIN); i++) {
        gp  = f_castGpioPin[i].gp;  // GP番号
        dir = f_castGpioPin[i].dir; // 方向
        if (dir) { 
            // 出力の場合    

            // 電源ON時出力値を決定
            if (pstConfig->initialValBits & (1 << gp)) {
                initialVal = true;
            }
            else {
                initialVal = false;
            }
            
            // GPIOの初期化
            gpio_init(gp);         
            // 電源ON時出力値を出力 
            gpio_put(gp, initialVal);
            // 出力方向に設定
            gpio_set_dir(gp, dir); 
            f_gpioOutDirBits |= (1 << gp); 
        }
        else { 
            // 入力の場合
            
            // GPIOの初期化
            gpio_init(gp);
            // 入方向に設定
            gpio_set_dir(gp, dir);          
            // プルダウン/プルアップの設定
            if (pstConfig->pullDownBits & (1 << gp)) {
                 gpio_pull_down(gp); // プルダウン
            }
            else {
                gpio_pull_up(gp); // プルアップ
            }              
             f_gpioInDirBits |= (1 << gp);
        }
    }
}
