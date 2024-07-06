#include "Common.h"

// [define] 
#define TIMER_CALLBACK_PERIOD   1    // 定期タイマコールバックの周期(ms)
#define TIMER_WDT_TIMEOUT       5000 // WDTタイマクリアのタイムアウト時間(ms) ※FLASHの消去・書き込みが間に合う時間にすること
#define TIMER_RECV_TIMEOUT      500  // 要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
#define TIMER_USB_SEND_TIMEOUT  1000 // USB送信タイムアウト(ms)
#define TIMER_I2C_TIMEOUT       100  // I2C送信/受信タイムアウト(ms)

// [ファイルスコープ変数の宣言]
static repeating_timer_t f_stTimer = {0};                   // 定期タイマコールバック登録時に渡すパラメータ
static ULONG f_timerCnt_wdt = 0;                            // WDTタイマのタイマカウント
static ULONG f_timerCnt_stabilizationWait = 0;              // 起動してからの安定待ち時間のタイマカウント
static ULONG f_aTimerCnt_recvTimeout[E_FRM_LINE_NUM] = {0}; // 右記のタイマカウント:要求フレームのヘッダを受信後、TIMER_RECV_TIMEOUT[ms]経過しても要求フレームの末尾まで受信してない場合はタイムアウトとする
static ULONG f_timerCnt_usbSendTimeout = 0;                 // USB送信タイムアウトのタイマカウント 
static ULONG f_timerCnt_led = 0;                            // LED点滅のタイマカウント
static ULONG f_ledPeriod = TIMER_LED_PERIOD_NORMAL;         // LED点滅周期
static ULONG f_timerCnt_i2cTimeout = 0;                     // I2C送信/受信タイムアウトのタイマカウント

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

    // [USB送信タイムアウトのタイマカウント]
    if (f_timerCnt_usbSendTimeout < TIMER_USB_SEND_TIMEOUT) {
        f_timerCnt_usbSendTimeout++;
    }

    // [I2C送信/受信タイムアウトのタイマカウント]
    if (f_timerCnt_i2cTimeout < TIMER_I2C_TIMEOUT) {
        f_timerCnt_i2cTimeout++;
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

// USB送信タイムアウトのタイマカウントをクリア
void TIMER_ClearUsbSendTimeout()
{
    f_timerCnt_usbSendTimeout = 0;
}

// USB送信タイムアウトか否かを取得
// ※本関数は、pico-sdk\src\rp2_common\pico_stdio_usb\stdio_usb.c のstdio_usb_out_chars()の内部で使用している
bool TIMER_IsUsbSendTimeout()
{
    return (f_timerCnt_usbSendTimeout >= TIMER_USB_SEND_TIMEOUT) ? true : false;
}

// LEDのON/OFFを変更するタイミングか否かを取得
bool TIMER_IsLedChangeTiming()
{
    return (f_timerCnt_led >= f_ledPeriod) ? true : false;
}

// LED点滅のタイマカウントをクリア
void TIMER_ClearLed()
{
    f_timerCnt_led = 0;
}

// LED点滅の周期を取得
ULONG TIMER_GetLedPeriod()
{
    return f_ledPeriod;
}

// I2C送信/受信タイムアウトのタイマカウントをクリア
void TIMER_ClearI2cTimeout()
{
    f_timerCnt_i2cTimeout = 0;
}

// I2C送信/受信タイムアウトか否かを取得
bool TIMER_IsI2cTimeout()
{
    return (f_timerCnt_i2cTimeout >= TIMER_I2C_TIMEOUT) ? true : false;
}

// タイマーを初期化
void TIMER_Init()
{
    // 定期タイマコールバックの登録
    add_repeating_timer_ms(TIMER_CALLBACK_PERIOD, Timer_PeriodicCallback, NULL, &f_stTimer);
}