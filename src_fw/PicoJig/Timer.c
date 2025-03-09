// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] 
#define TIMER_CALLBACK_PERIOD   1    // 定期タイマコールバックの周期(ms)
#define TIMER_WDT_TIMEOUT       5000 // WDTタイマクリアのタイムアウト時間(ms) ※FLASHの消去・書き込みが間に合う時間にすること
#define TIMER_RECV_TIMEOUT      500  // 要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
#define TIMER_STABILIZATION_WAIT_TIME 50 // 起動してからの安定待ち時間(ms) ※値は適当

// [ファイルスコープ変数の宣言]
static repeating_timer_t f_stTimer = {0};                   // 定期タイマコールバック登録時に渡すパラメータ
static ULONG f_timerCnt_wdt = 0;                            // WDTタイマのタイマカウント
static ULONG f_timerCnt_stabilizationWait = 0;              // 起動してからの安定待ち時間のタイマカウント
static ULONG f_aTimerCnt_recvTimeout[E_FRM_LINE_NUM] = {0}; // 右記のタイマカウント:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
static ULONG f_timerCnt_led = 0;                            // LED点滅のタイマカウント
static ULONG f_ledPeriod = TIMER_LED_PERIOD_NORMAL;         // LED点滅周期

// [関数プロトタイプ宣言]
static bool Timer_PeriodicCallback(repeating_timer_t *rt);

// 定期タイマコールバック
static bool Timer_PeriodicCallback(repeating_timer_t *pstTimer) 
{
    ULONG i;

    // [WDTタイマのタイマカウント]
    if (f_timerCnt_wdt <  TIMER_WDT_TIMEOUT) { // タイムアウトしてない場合
        f_timerCnt_wdt++;
    }
    else { // タイムアウトした場合
        // watchdog_enable()を使用して即WDTタイムアウトで再起動する
        CMN_WdtEnableReboot();
    }

    // [起動してからの安定待ち時間のタイマカウント]
    if (f_timerCnt_stabilizationWait < TIMER_STABILIZATION_WAIT_TIME) {
        f_timerCnt_stabilizationWait++;
    }

    // 右記のタイマカウント:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
    for (i = 0; i < E_FRM_LINE_NUM; i++) {
        if (f_aTimerCnt_recvTimeout[i] < TIMER_RECV_TIMEOUT) {
            f_aTimerCnt_recvTimeout[i]++;
        }
    }

    // [LED点滅のタイマカウント]
    if (CMN_GetFwErrorBits(true)) { // FWエラー発生時の場合
        // FWエラー発生時のLED点滅の周期に設定
        f_ledPeriod = TIMER_LED_PERIOD_ERR;
    }
    else {
        // FWエラー未発生時のLED点滅の周期に設定
        f_ledPeriod = TIMER_LED_PERIOD_NORMAL;        
    }

    // LED点滅のタイマカウント
    if (f_timerCnt_led < f_ledPeriod) {
        f_timerCnt_led++; 
    }
 
    return true; // keep repeating
}

// WDTタイマのタイマカウントをクリア
void TIMER_WdtClear()
{
    f_timerCnt_wdt = 0;
}

// 起動してからの安定待ち時間が経過したかどうかを取得
bool TIMER_IsStabilizationWaitTimePassed()
{
    return (f_timerCnt_stabilizationWait >=TIMER_STABILIZATION_WAIT_TIME) ? true : false;
}

// 右記のタイマカウントをクリア:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
void TIMER_ClearRecvTimeout(ULONG line)
{
    f_aTimerCnt_recvTimeout[line] = 0;
}

// 右記のタイムアウトか否かを取得:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
bool TIMER_IsRecvTimeout(ULONG line)
{
    return (f_aTimerCnt_recvTimeout[line] >= TIMER_RECV_TIMEOUT) ? true : false;
}

// LEDのON/OFFを変更するタイミングか否かを取得
bool TIMER_IsLedChangeTiming()
{
    bool bRet = false;

    if (f_timerCnt_led >= f_ledPeriod) {
        bRet = true;
        f_timerCnt_led = 0;
    }

    return bRet;
}

// LED点滅の周期を取得
ULONG TIMER_GetLedPeriod()
{
    return f_ledPeriod;
}

// タイマーを初期化
void TIMER_Init()
{
    // 定期タイマコールバックの登録
    add_repeating_timer_ms(TIMER_CALLBACK_PERIOD, Timer_PeriodicCallback, NULL, &f_stTimer);
}