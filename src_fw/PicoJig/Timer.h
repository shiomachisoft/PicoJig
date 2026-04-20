// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef TIMER_H
#define TIMER_H

#include "Common.h"

// [define] / [定義]
#define TIMER_LED_PERIOD_NORMAL 500  // LED blink period when no FW error occurs (ms) / FWエラー未発生時のLED点滅の周期(ms)
#define TIMER_LED_PERIOD_ERR    100  // LED blink period when a FW error occurs (ms) / FWエラー発生時のLED点滅の周期(ms)

// [Function prototype declarations] / [関数プロトタイプ宣言]
void TIMER_WdtClear();
bool TIMER_IsStabilizationWaitTimePassed();
void TIMER_ClearRecvTimeout(ULONG line);
bool TIMER_IsRecvTimeout(ULONG line);
bool TIMER_IsLedChangeTiming();
ULONG TIMER_GetLedPeriod();
void TIMER_Init();

#endif