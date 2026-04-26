// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef TIMER_H
#define TIMER_H

#include "Common.h"

// [define] / [定義]
#define TMR_LED_PERIOD_NORMAL 500  // LED blink period when no FW error occurs (ms) / FWエラー未発生時のLED点滅の周期(ms)
#define TMR_LED_PERIOD_ERR    100  // LED blink period when a FW error occurs (ms) / FWエラー発生時のLED点滅の周期(ms)

// [Function prototype declarations] / [関数プロトタイプ宣言]
void TMR_WdtClear();
bool TMR_IsStabilizationWaitTimePassed();
void TMR_ClearRecvTimeout(ULONG line);
bool TMR_IsRecvTimeout(ULONG line);
bool TMR_IsLedChangeTiming();
ULONG TMR_GetLedPeriod();
void TMR_Init();

#endif