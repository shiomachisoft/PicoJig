#include "Common.h"

// [define]
#define FLASH_OFFSET  (0x1F0000 + FLASH_SECTOR_SIZE * 15) // FLASHの最終ブロック(Block31)の16個あるセクタの最後のセクタの先頭アドレス
#define FLASH_WRITE_BUF_SIZE (FLASH_PAGE_SIZE * 1)        // FLASHデータ書き込みサイズ ※ FLASH_PAGE_SIZE(256byte)の倍数とする

// [ファイルスコープ変数]
static ST_FLASH_DATA f_stFlashData = {0};            // 電源起動時のFLASHデータ
static UCHAR f_writeBuf[FLASH_WRITE_BUF_SIZE] = {0}; // FLASHデータ書き込みバッファ

// 電源起動時のFLASHデータを返す
ST_FLASH_DATA* FLASH_GetDataAtPowerOn()
{
    return &f_stFlashData;
}

// FLASHの最終ブロック(Block31)のセクタ15から設定データを読み込む
void FLASH_Read(ST_FLASH_DATA *pstFlashData)
{
    const PVOID pSrc = (const PVOID) (XIP_BASE + FLASH_OFFSET); // 最終ブロック(Block31)のセクタ15の先頭アドレス。XIP_BASEはプログラムの先頭。
    char szFwName[FW_NAME_BUF_SIZE];
    USHORT checksum;       // チェックサム
    ULONG i;
    bool bDeafult = false; // デフォルトの設定データを採用するか否か
    
    // 最終ブロック(Block31)のセクタ15の先頭アドレスを読み込む
    memcpy(pstFlashData, pSrc, sizeof(ST_FLASH_DATA));

    // [FW名, FWバージョン, チェックサムが問題ない場合、この読み込んだデータが上位に返されることになる]

    // FW名のチェック
    memset(szFwName, 0, sizeof(szFwName));
    strcpy(szFwName, FW_NAME);
    // pstFlashData->szFwNameがNULL文字で終わっているとは限らないのでstrcmpは使用しないで比較する
    for (i = 0; i < FW_NAME_BUF_SIZE; i++)
    {
        if (pstFlashData->szFwName[i] != szFwName[i]) {
            bDeafult = true;  // デフォルトの設定データを採用
            break;
        } 
    }

    // FWバージョンのチェック
    if (!bDeafult) {
        if (pstFlashData->fwVer != FW_VER) {
            // FWバージョンが不正値の場合
            bDeafult = true;  // デフォルトの設定データを採用
        }      
    }

    // チェックサム検査
    if (!bDeafult) {
        // チェックサムを計算する
        checksum = CMN_CalcChecksum(pstFlashData, sizeof(ST_FLASH_DATA) - sizeof(checksum));        
        if (pstFlashData->checksum != checksum) {
            // チェックサム検査がNGの場合
            bDeafult = true; // デフォルトの設定データを採用
        }
    }

    if (bDeafult) {
        // デフォルトの設定データを採用
        GPIO_SetDefault(&pstFlashData->stGpioConfig);
        UART_SetDefault(&pstFlashData->stUartConfig);
        SPI_SetDefault(&pstFlashData->stSpiConfig);
        I2C_SetDefault(&pstFlashData->stI2cConfig);
        tcp_server_set_default(&pstFlashData->stNwConfig);
    }
}

// FLASHの最終ブロック(Block31)のセクタ15に設定データを書き込む
void FLASH_Write(ST_FLASH_DATA *pstFlashData)
{
    USHORT checksum;   // チェックサム
    __unused ULONG ints;
    
    // FLASHデータ書き込みバッファを初期化
    memset(f_writeBuf, 0, sizeof(f_writeBuf));
    // FW名を設定
    memset(pstFlashData->szFwName, 0, sizeof(pstFlashData->szFwName));
    strcpy(pstFlashData->szFwName, FW_NAME);
    // FWバージョンを設定
    pstFlashData->fwVer = FW_VER;
    // チェックサムを計算して設定
    checksum = CMN_CalcChecksum(pstFlashData, sizeof(ST_FLASH_DATA) - sizeof(checksum));
    pstFlashData->checksum = checksum;
    // FLASHデータ書き込みバッファに引数データをコピー
    memcpy(f_writeBuf, pstFlashData, sizeof(ST_FLASH_DATA));
   
    // CPUコア1を停止
    multicore_reset_core1();
    // 割り込みを無効に設定
    ints = save_and_disable_interrupts();
    // WDTタイマをクリア
    TIMER_WdtClear();

    // FLASH消去
    // 消去単位はflash.hで定義されている FLASH_SECTOR_SIZE(4096byte)の倍数とする
    flash_range_erase(FLASH_OFFSET, FLASH_SECTOR_SIZE);
    // FLASH書き込み
    // 書込単位はflash.hで定義されている FLASH_PAGE_SIZE(256byte)の倍数とする
    flash_range_program(FLASH_OFFSET, f_writeBuf, sizeof(f_writeBuf)); // f_writBufのサイズはFLASH_WRITE_BUF_SIZEの倍数になっている
    
    // watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
    CMN_WdtNoEnableReboot();

    // 割り込みフラグを戻す
    //restore_interrupts(ints);     
}

// FLASHの最終ブロック(Block31)のセクタ15のデータを消去
void FLASH_Erase()
{
    __unused ULONG ints;

    // CPUコア1を停止
    multicore_reset_core1();
    // 割り込みを無効に設定
    ints = save_and_disable_interrupts();
    // WDTタイマをクリア
    TIMER_WdtClear();

    // FLASH消去
    // 消去単位はflash.hで定義されている FLASH_SECTOR_SIZE(4096byte)の倍数とする
    flash_range_erase(FLASH_OFFSET, FLASH_SECTOR_SIZE);
    
    // watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
    CMN_WdtNoEnableReboot();

    // 割り込みフラグを戻す
    //restore_interrupts(ints);    
}

// FLASHライブラリを初期化
void FLASH_Init()
{
    // 電源起動時のFLASHデータを読み込み
    FLASH_Read(&f_stFlashData);
}