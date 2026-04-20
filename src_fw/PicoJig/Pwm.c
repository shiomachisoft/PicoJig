// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [File scope variables] / [ファイルスコープ変数]
static uint f_slice_num = 0; // PWM slice number / PWMスライス番号

// Start PWM / PWM開始
void PWM_Start(ST_PWM_CONFIG *pstConfig)
{
    // Disable PWM output / PWM出力を無効に設定
    pwm_set_enabled(f_slice_num, false); 

    // Set PWM period / PWM周期を設定
    pwm_set_clkdiv(f_slice_num, pstConfig->clkdiv); // Clock divider / クロック分周器
    pwm_set_wrap(f_slice_num, pstConfig->wrap);     // Wrap value / ラップ値
    
    // Set PWM compare value / PWMの比較値を設定
    pwm_set_chan_level(f_slice_num, pwm_gpio_to_channel(PWM_OUT), pstConfig->level);
    
    // Enable PWM output / PWM出力を有効に設定
    pwm_set_enabled(f_slice_num, true);    
}

// Stop PWM / PWM停止
void PWM_Stop()
{    
    // Disable PWM output / PWM出力を無効に設定
    pwm_set_enabled(f_slice_num, false);    
}

// Initialize PWM / PWMを初期化
void PWM_Init()
{
    // Set pin function / ピンの機能設定
    gpio_set_function(PWM_OUT, GPIO_FUNC_PWM);

    // Get PWM slice number / PWMスライス番号を取得
    f_slice_num = pwm_gpio_to_slice_num(PWM_OUT);   
}