// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [ファイルスコープ変数]
uint f_slice_num = 0; // PWMスライス番号

// PWM開始
void PWM_Start(ST_PWM_CONFIG *pstConfig)
{
    // PWM出力を無効に設定
    pwm_set_enabled(f_slice_num, false); 

    // PWM周期を設定
    pwm_set_clkdiv(f_slice_num, pstConfig->clkdiv); // クロック分周器
    pwm_set_wrap(f_slice_num, pstConfig->wrap);     // 分解能
    
    // PWMのHigh期間を設定
    pwm_set_chan_level(f_slice_num, pwm_gpio_to_channel(PWM_OUT), pstConfig->level);
    
    // PWM出力を有効に設定
    pwm_set_enabled(f_slice_num, true);    
}

// PWM停止
void PWM_Stop()
{    
    // PWM出力を無効に設定
    pwm_set_enabled(f_slice_num, false);    
}

// PWMを初期化
void PWM_Init()
{
    // ピンの機能設定
    gpio_set_function(PWM_OUT, GPIO_FUNC_PWM);

    // PWMスライス番号を取得
    f_slice_num = pwm_gpio_to_slice_num(PWM_OUT);
}