// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義] 
#define TIMER_CALLBACK_PERIOD   1    // Periodic timer callback period (ms) / 定期タイマコールバックの周期(ms)
#define TIMER_WDT_TIMEOUT       5000 // WDT timer clear timeout (ms) * Must be enough time for FLASH erase/write / WDTタイマクリアのタイムアウト時間(ms) ※FLASHの消去・書き込みが間に合う時間にすること
#define TIMER_RECV_TIMEOUT      500  // If the end of the request frame is not received within TIMER_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
#define TIMER_STABILIZATION_WAIT_TIME 50 // Wait time for stabilization after boot (ms) * Value is arbitrary / 起動してからの安定待ち時間(ms) ※値は適当

// [File scope variable declarations] / [ファイルスコープ変数の宣言]
static repeating_timer_t f_stTimer = {0};                   // Parameter passed when registering periodic timer callback / 定期タイマコールバック登録時に渡すパラメータ
static volatile ULONG f_timerCnt_wdt = 0;                            // Timer count for WDT timer / WDTタイマのタイマカウント
static volatile ULONG f_timerCnt_stabilizationWait = 0;              // Timer count for stabilization wait time after boot / 起動してからの安定待ち時間のタイマカウント
static volatile ULONG f_aTimerCnt_recvTimeout[E_FRM_LINE_NUM] = {0}; // Timer count for the following: If the end of the request frame is not received within TIMER_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 右記のタイマカウント:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
static volatile ULONG f_timerCnt_led = 0;                            // Timer count for LED blink / LED点滅のタイマカウント
static volatile ULONG f_ledPeriod = TIMER_LED_PERIOD_NORMAL;         // LED blink period / LED点滅周期

// [Function prototype declarations] / [関数プロトタイプ宣言]
static bool TIMER_PeriodicCallback(repeating_timer_t *rt);

// Periodic timer callback / 定期タイマコールバック
static bool TIMER_PeriodicCallback(repeating_timer_t *pstTimer) 
{
    ULONG i;

    // [Timer count for WDT timer] / [WDTタイマのタイマカウント]
    if (f_timerCnt_wdt <  TIMER_WDT_TIMEOUT) { // If not timed out / タイムアウトしてない場合
        f_timerCnt_wdt++;
    }
    else { // If timed out / タイムアウトした場合
        // Reboot immediately by WDT timeout using watchdog_enable() / watchdog_enable()を使用して即WDTタイムアウトで再起動する
        CMN_WdtEnableReboot();
    }

    // [Timer count for stabilization wait time after boot] / [起動してからの安定待ち時間のタイマカウント]
    if (f_timerCnt_stabilizationWait < TIMER_STABILIZATION_WAIT_TIME) {
        f_timerCnt_stabilizationWait++;
    }

    // Timer count for the following: If the end of the request frame is not received within TIMER_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 右記のタイマカウント:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
    for (i = 0; i < E_FRM_LINE_NUM; i++) {
        if (f_aTimerCnt_recvTimeout[i] < TIMER_RECV_TIMEOUT) {
            f_aTimerCnt_recvTimeout[i]++;
        }
    }

    // [Timer count for LED blink] / [LED点滅のタイマカウント]
    if (CMN_GetFwErrorBits(true)) { // If FW error occurs / FWエラー発生時の場合
        // Set to LED blink period when FW error occurs / FWエラー発生時のLED点滅の周期に設定
        f_ledPeriod = TIMER_LED_PERIOD_ERR;
    }
    else {
        // Set to LED blink period when no FW error occurs / FWエラー未発生時のLED点滅の周期に設定
        f_ledPeriod = TIMER_LED_PERIOD_NORMAL;        
    }

    // Timer count for LED blink / LED点滅のタイマカウント
    if (f_timerCnt_led < f_ledPeriod) {
        f_timerCnt_led++; 
    }
 
    return true; // Keep repeating / 繰り返しを継続する
}

// Clear timer count for WDT timer / WDTタイマのタイマカウントをクリア
void TIMER_WdtClear()
{
    f_timerCnt_wdt = 0;
}

// Get whether stabilization wait time after boot has passed / 起動してからの安定待ち時間が経過したかどうかを取得
bool TIMER_IsStabilizationWaitTimePassed()
{
    return (f_timerCnt_stabilizationWait >=TIMER_STABILIZATION_WAIT_TIME) ? true : false;
}

// Clear timer count for the following: If the end of the request frame is not received within TIMER_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 右記のタイマカウントをクリア:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
void TIMER_ClearRecvTimeout(ULONG line)
{
    f_aTimerCnt_recvTimeout[line] = 0;
}

// Get whether it's timeout for the following: If the end of the request frame is not received within TIMER_RECV_TIMEOUT [ms] after receiving the header, a timeout occurs / 右記のタイムアウトか否かを取得:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
bool TIMER_IsRecvTimeout(ULONG line)
{
    return (f_aTimerCnt_recvTimeout[line] >= TIMER_RECV_TIMEOUT) ? true : false;
}

// Get whether it's time to change the LED ON/OFF state / LEDのON/OFFを変更するタイミングか否かを取得
bool TIMER_IsLedChangeTiming()
{
    bool bRet = false;

    if (f_timerCnt_led >= f_ledPeriod) {
        bRet = true;
        f_timerCnt_led = 0;
    }

    return bRet;
}

// Get LED blink period / LED点滅の周期を取得
ULONG TIMER_GetLedPeriod()
{
    return f_ledPeriod;
}

// Initialize timer / タイマーを初期化
void TIMER_Init()
{
    // Register periodic timer callback / 定期タイマコールバックの登録
    add_repeating_timer_ms(TIMER_CALLBACK_PERIOD, TIMER_PeriodicCallback, NULL, &f_stTimer);
}