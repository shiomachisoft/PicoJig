// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef PWM_H
#define PWM_H

#include "Common.h"

#pragma pack(1)

// [構造体]
// PWM設定
typedef struct ST_PWM_CONFIG {
    float clkdiv; // クロック分周 
    USHORT wrap;  // 分解能
    USHORT level; // High期間
} ST_PWM_CONFIG;

#pragma pack()

void PWM_Start(ST_PWM_CONFIG *pstConfig);
void PWM_Stop();
void PWM_Init();

#endif