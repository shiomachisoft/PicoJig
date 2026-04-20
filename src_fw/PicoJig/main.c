// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [File scope variables] / [ファイルスコープ変数]
static volatile bool f_isWillClearWdtByCore1 = false; // Whether it is CPU core 1's turn to clear the WDT timer / CPUコア1によってWDTタイマをクリアする番か否か

// [Function prototype declarations] / [関数プロトタイプ宣言]
static void MN_Init();
static void MN_MainLoop_Core0();
static void MN_MainLoop_Core1();
static void MN_ControlLed();
static void MN_ExceptionHandler();
static void MN_RegisterExceptionHandler();

// Main function / メイン関数
int main() 
{
	// Initialization at power-on / 電源起動時の初期化
	MN_Init();

	// Start main loop of CPU core 1 / CPUコア1のメインループを開始
	multicore_launch_core1(MN_MainLoop_Core1); 

	// Start main loop of CPU core 0 / CPUコア0のメインループを開始
	MN_MainLoop_Core0();

	return 0;
}

// Main loop of CPU core 0 / CPUコア0のメインループ
static void MN_MainLoop_Core0()
{
	while (1) 
	{
		if (!f_isWillClearWdtByCore1) { // If it is CPU core 0's turn to clear the WDT timer / CPUコア0によってWDTタイマをクリアする番の場合
			// Clear WDT timer / WDTタイマをクリア
			TIMER_WdtClear();
			f_isWillClearWdtByCore1 = true;
		}	

		// Extract USB/wireless receive data ⇒ Parse and execute command / USB/無線受信データ取り出し⇒コマンド解析・実行
		FRM_RecvMain();

		// UART main processing / UARTメイン処理
		UART_Main();

		// I2C main processing / I2Cメイン処理
		I2C_Main();
	}
}

// Main loop of CPU core 1 / CPUコア1のメインループ
static void MN_MainLoop_Core1() 
{
	// Initialization to accept lockout (pause) request from CPU core 0 during flash write / CPUコア0からのフラッシュ書き込み時のロックアウト(一時停止)要求を受け入れるための初期化
	multicore_lockout_victim_init();

	while (1) 
	{
		if (f_isWillClearWdtByCore1) { // If it is CPU core 1's turn to clear the WDT timer / CPUコア1によってWDTタイマをクリアする番の場合	
			// Clear WDT timer / WDTタイマをクリア
			TIMER_WdtClear();
			f_isWillClearWdtByCore1 = false;
		}	

		// Control LED / LEDを制御する
		MN_ControlLed();

#ifdef MY_BOARD_PICO_W
		// TCP server main processing / TCPサーバーのメイン処理
		tcp_server_main();
#endif

		// USB/wireless send main processing / USB/無線送信のメイン処理
		FRM_SendMain();
	}
}

// Control LED / LEDを制御する
static void MN_ControlLed()
{
	ULONG period;
	static bool bLedOn = false;

	// Get whether it's time to change the LED ON/OFF state / LEDのON/OFFを変更するタイミングか否かを取得
	if (true == TIMER_IsLedChangeTiming()) {
		// Determine whether to turn ON/OFF / ON/OFFのどちらにするかを決める
		period = TIMER_GetLedPeriod();
		if (TIMER_LED_PERIOD_ERR == period) {
			bLedOn = !bLedOn;	
		}	
		else {
#ifdef MY_BOARD_PICO_W			
			if (true == tcp_server_is_link_up()) {
				bLedOn = true;
			} 
			else {
				bLedOn = !bLedOn;	
			}
#else
			bLedOn = !bLedOn;
#endif
		}
		// Output ON/OFF to LED / LEDにON/OFFを出力	
#ifdef MY_BOARD_PICO_W
		if (tcp_server_is_inited()) { // Prevent crash by calling only after cyw43_arch_init succeeds / cyw43_arch_init成功後のみ呼び出すことでクラッシュを防ぐ
			cyw43_arch_gpio_put(CYW43_WL_GPIO_LED_PIN, bLedOn);	
		}
#else		
		gpio_put(ONBOARD_LED, bLedOn);
#endif
	}
}

// Exception handler / 例外ハンドラ
static void MN_ExceptionHandler()
{
	// Reboot immediately by WDT timeout using watchdog_enable() / watchdog_enable()を使用して即WDTタイムアウトで再起動する
	CMN_WdtEnableReboot();
}

// Initialization at power-on / 電源起動時の初期化
static void MN_Init()
{
	ST_FLASH_DATA *pstFlashData; // FLASH data at power-on / 電源起動時のFLASHデータ

	// Register exception handler / 例外ハンドラを登録
	MN_RegisterExceptionHandler();

	// Initialize CDC / CDCを初期化
	stdio_init_all();

	// Initialize common library / 共通ライブラリを初期化
	CMN_Init();	

	// Initialize FLASH library / FLASHライブラリを初期化
	FLASH_Init();
	pstFlashData = FLASH_GetDataAtPowerOn();

	// Initialize GPIO / GPIOを初期化
	GPIO_Init(&pstFlashData->stGpioConfig);

	// Initialize ADC / ADCを初期化
	ADC_Init();

	// Initialize UART / UARTを初期化
	UART_Init(&pstFlashData->stUartConfig);

	// Initialize SPI / SPIを初期化
	SPI_Init(&pstFlashData->stSpiConfig);

	// Initialize I2C / I2Cを初期化
	I2C_Init(&pstFlashData->stI2cConfig);

	// Initialize PWM / PWMを初期化
	PWM_Init();

	// Initialize USB/wireless common processing / USB/無線共通処理を初期化
	FRM_Init();

	// Initialize timer / タイマーを初期化
	TIMER_Init();

	// Wait for stabilization wait time after boot / 起動してからの安定待ち時間を待つ
	while (!TIMER_IsStabilizationWaitTimePassed()) {}

	if (watchdog_enable_caused_reboot()) { // If rebooted by watchdog_enable() WDT timeout instead of watchdog_reboot() / watchdog_reboot()ではなくwatchdog_enable()のWDTタイムアウトで再起動していた場合
		// Set FW error / FWエラーを設定
		CMN_SetErrorBits(CMN_ERR_BIT_WDT_RESET, true);
	}	
}

// Register exception handler / 例外ハンドラを登録
static void MN_RegisterExceptionHandler()
{
	exception_set_exclusive_handler(NMI_EXCEPTION, MN_ExceptionHandler);
	exception_set_exclusive_handler(HARDFAULT_EXCEPTION, MN_ExceptionHandler);
	exception_set_exclusive_handler(SVCALL_EXCEPTION, MN_ExceptionHandler);
	exception_set_exclusive_handler(PENDSV_EXCEPTION, MN_ExceptionHandler);
	exception_set_exclusive_handler(SYSTICK_EXCEPTION, MN_ExceptionHandler);	
}