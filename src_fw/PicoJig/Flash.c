// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義]
// For Pico: / Picoの場合:
// PICO_FLASH_SIZE_BYTES = 0x200000
// FLASH_SECTOR_SIZE = 0x1000
// FLASH_PAGE_SIZE = 256
#define FLASH_OFFSET  (PICO_FLASH_SIZE_BYTES - FLASH_SECTOR_SIZE)   // Start offset address of the last sector of the last block of FLASH / FLASHの最終ブロックの最終セクタの先頭オフセットアドレス
#define FLASH_WRITE_BUF_SIZE (FLASH_PAGE_SIZE * 2)                  // FLASH data write size * Must be a multiple of FLASH_PAGE_SIZE / FLASHデータ書き込みサイズ ※ FLASH_PAGE_SIZEの倍数とする

// [File scope variables] / [ファイルスコープ変数]
static ST_FLASH_DATA f_stFlashData = {0};            // FLASH data at power-on / 電源起動時のFLASHデータ
static UCHAR f_writeBuf[FLASH_WRITE_BUF_SIZE] = {0}; // FLASH data write buffer / FLASHデータ書き込みバッファ

// Return FLASH data at power-on / 電源起動時のFLASHデータを返す
ST_FLASH_DATA* FLASH_GetDataAtPowerOn()
{
    return &f_stFlashData;
}

// Read config data from the last sector of the last block of FLASH / FLASHの最終ブロックの最終セクタから設定データを読み込む
void FLASH_Read(ST_FLASH_DATA *pstFlashData)
{
    const PVOID pSrc = (const PVOID) (XIP_BASE + FLASH_OFFSET); // Start address of the last sector of the last block. XIP_BASE is the base address of FLASH memory. / 最終ブロックの最終セクタの先頭アドレス。XIP_BASEはFLASHメモリのベースアドレス。
    char szFwName[FW_NAME_BUF_SIZE];
    USHORT checksum;       // Checksum / チェックサム
    bool bDefault = false; // Whether to adopt default config data / デフォルトの設定データを採用するか否か
    
    // Read data from the start address of the last sector of the last block / 最終ブロックの最終セクタの先頭アドレスからデータを読み込む
    memcpy(pstFlashData, pSrc, sizeof(ST_FLASH_DATA));

    // [If FW name, FW version, and checksum are OK, this read data will be returned to the upper layer] / [FW名, FWバージョン, チェックサムが問題ない場合、この読み込んだデータが上位に返されることになる]
    do {
        // Check FW name / FW名のチェック
        memset(szFwName, 0, sizeof(szFwName));
        strcpy(szFwName, FW_NAME);
        // Compare without using strcmp since pstFlashData->szFwName is not guaranteed to end with a NULL character / pstFlashData->szFwNameがNULL文字で終わっているとは限らないのでstrcmpは使用しないで比較する
        if (memcmp(pstFlashData->szFwName, szFwName, FW_NAME_BUF_SIZE) != 0) {
            bDefault = true;  // Adopt default config data / デフォルトの設定データを採用
            break;
        }

        // Check FW version / FWバージョンのチェック
        if (pstFlashData->fwVer != FW_VER) {
            // If FW version is different / FWバージョンが異なる場合
            bDefault = true;  // Adopt default config data / デフォルトの設定データを採用
            break;
        }
        
        // Checksum validation / チェックサム検査
        checksum = CMN_CalcChecksum(pstFlashData, sizeof(ST_FLASH_DATA) - sizeof(pstFlashData->checksum));        
        if (pstFlashData->checksum != checksum) {
            // If checksum validation fails / チェックサム検査がNGの場合
            bDefault = true; // Adopt default config data / デフォルトの設定データを採用
            break;
        }
    } while(0);

    if (bDefault) {
        // Adopt default config data / デフォルトの設定データを採用
        
        // Initialize structure entirely and set FW name/version / 構造体全体を初期化し、FW名やバージョンを設定
        memset(pstFlashData, 0, sizeof(ST_FLASH_DATA));
        strcpy(pstFlashData->szFwName, FW_NAME);
        pstFlashData->fwVer = FW_VER;

        GPIO_GetDefaultConfig(&pstFlashData->stGpioConfig);
        UART_GetDefaultConfig(&pstFlashData->stUartConfig);
        SPI_GetDefaultConfig(&pstFlashData->stSpiConfig);
        I2C_GetDefaultConfig(&pstFlashData->stI2cConfig);
#ifdef MY_BOARD_PICO_W        
        tcp_server_get_default_config(&pstFlashData->stNwConfig);
#else
        memset(&pstFlashData->stNwConfig, 0, sizeof(pstFlashData->stNwConfig));
#endif
    }
}

// Write config data to the last sector of the last block of FLASH / FLASHの最終ブロックの最終セクタに設定データを書き込む
void FLASH_Write(ST_FLASH_DATA *pstFlashData)
{
    USHORT checksum;   // Checksum / チェックサム
    //ULONG ints;
    
    // Initialize FLASH data write buffer with 0xFF (erased state) / FLASHデータ書き込みバッファを0xFF(消去状態)で初期化
    memset(f_writeBuf, 0xFF, sizeof(f_writeBuf));
    // Set FW name / FW名を設定
    memset(pstFlashData->szFwName, 0, sizeof(pstFlashData->szFwName));
    strcpy(pstFlashData->szFwName, FW_NAME);
    // Set FW version / FWバージョンを設定
    pstFlashData->fwVer = FW_VER;
    // Calculate and set checksum / チェックサムを計算して設定
    checksum = CMN_CalcChecksum(pstFlashData, sizeof(ST_FLASH_DATA) - sizeof(pstFlashData->checksum));
    pstFlashData->checksum = checksum;
    // Copy argument data to FLASH data write buffer / FLASHデータ書き込みバッファに引数データをコピー
    memcpy(f_writeBuf, pstFlashData, sizeof(ST_FLASH_DATA));
   
    // Block CPU core 1 / CPUコア1をブロック
    multicore_lockout_start_blocking();
    // Disable interrupts / 割り込み禁止
    //ints = save_and_disable_interrupts();
    (void)save_and_disable_interrupts();

    // Erase FLASH / FLASH消去
    // Erase unit must be a multiple of FLASH_SECTOR_SIZE (4096 bytes) defined in flash.h / 消去単位はflash.hで定義されている FLASH_SECTOR_SIZE(4096byte)の倍数とする
    flash_range_erase(FLASH_OFFSET, FLASH_SECTOR_SIZE);
    // Write to FLASH / FLASH書き込み
    // Write unit must be a multiple of FLASH_PAGE_SIZE defined in flash.h / 書き込み単位はflash.hで定義されている FLASH_PAGE_SIZEの倍数とする
    flash_range_program(FLASH_OFFSET, f_writeBuf, sizeof(f_writeBuf)); // The size of f_writeBuf is a multiple of FLASH_PAGE_SIZE / f_writeBufのサイズはFLASH_PAGE_SIZEの倍数になっている
    
    // Reboot immediately by WDT timeout without using watchdog_enable() / watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
    CMN_WdtRebootWithoutEnable();

#if 0   
    // Enable interrupts / 割り込み許可
    restore_interrupts(ints); 
    // Unblock CPU core 1 / CPUコア1のブロックを解除
    multicore_lockout_end_blocking();
#endif
}

// Erase data in the last sector of the last block of FLASH / FLASHの最終ブロックの最終セクタのデータを消去
void FLASH_Erase()
{
    //ULONG ints;

    // Block CPU core 1 / CPUコア1をブロック
    multicore_lockout_start_blocking();
    // Disable interrupts / 割り込み禁止
    //ints = save_and_disable_interrupts();
    (void)save_and_disable_interrupts();

    // Erase FLASH / FLASH消去
    // Erase unit must be a multiple of FLASH_SECTOR_SIZE (4096 bytes) defined in flash.h / 消去単位はflash.hで定義されている FLASH_SECTOR_SIZE(4096byte)の倍数とする
    flash_range_erase(FLASH_OFFSET, FLASH_SECTOR_SIZE);
    
    // Reboot immediately by WDT timeout without using watchdog_enable() / watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
    CMN_WdtRebootWithoutEnable();

#if 0
    // Enable interrupts / 割り込み許可
    restore_interrupts(ints);    
    // Unblock CPU core 1 / CPUコア1のブロックを解除
    multicore_lockout_end_blocking();
#endif
}

// Initialize FLASH library / FLASHライブラリを初期化
void FLASH_Init()
{
    // Read FLASH data at power-on / 電源起動時のFLASHデータを読み込み
    FLASH_Read(&f_stFlashData);
}