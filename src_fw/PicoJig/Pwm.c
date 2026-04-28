// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [File scope variables] / [ファイルスコープ変数]
static uint f_sliceNum = 0; // PWM slice number / PWMスライス番号

// Start PWM / PWM開始
void PWM_Start(ST_PWM_CONFIG *pstConfig)
{
    // Disable PWM output / PWM出力を無効に設定
    pwm_set_enabled(f_sliceNum, false); 

    // Set PWM period / PWM周期を設定
    pwm_set_clkdiv(f_sliceNum, pstConfig->clkdiv); // Clock divider / クロック分周
    pwm_set_wrap(f_sliceNum, pstConfig->wrap);     // Wrap value / ラップ値
    
    // Set PWM compare value / PWMの比較値を設定
    pwm_set_chan_level(f_sliceNum, pwm_gpio_to_channel(PWM_OUT), pstConfig->level);
    
    // Enable PWM output / PWM出力を有効に設定
    pwm_set_enabled(f_sliceNum, true);    
}

// Stop PWM / PWM停止
void PWM_Stop()
{    
    // Disable PWM output / PWM出力を無効に設定
    pwm_set_enabled(f_sliceNum, false);    
}

// Initialize PWM / PWMを初期化
void PWM_Init()
{
    // Set pin function / ピンの機能設定
    gpio_set_function(PWM_OUT, GPIO_FUNC_PWM);

    // Get PWM slice number / PWMスライス番号を取得
    f_sliceNum = pwm_gpio_to_slice_num(PWM_OUT);   
}