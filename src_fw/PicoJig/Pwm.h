// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef PWM_H
#define PWM_H

#include "Common.h"

#pragma pack(1)

// [Structs] / [構造体]
// PWM config / PWM設定
typedef struct _ST_PWM_CONFIG {
    float clkdiv; // Clock divider / クロック分周 
    USHORT wrap;  // Wrap value / ラップ値
    USHORT level; // Compare value / 比較値
} ST_PWM_CONFIG;

#pragma pack()

void PWM_Start(ST_PWM_CONFIG *pstConfig);
void PWM_Stop();
void PWM_Init();

#endif