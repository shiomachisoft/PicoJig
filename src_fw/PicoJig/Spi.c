// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define]
#define SPI_ID spi0    // SPIのID
#define CS_HOLD_TIME 5 // CSのHigh/Low保持時間(us)。1bit分(できれば2bit分以上)の時間分以上に設定する。

// SPI設定のデフォルト値
#define SPI_DEFAULT_FREQ      1000000UL     // クロック周波数(Hz) 
#define SPI_DEFAULT_DATA_BITS 8             // データビット長
#define SPI_DEFAULT_POLARITY  SPI_CPOL_0    // 極性
#define SPI_DEFAULT_PHASE     SPI_CPHA_0    // 位相
#define SPI_DEFAULT_ORDER     SPI_MSB_FIRST // バイトオーダー

#define SPI_USE_GPIO_AS_CS // CSはGPIOを使用する場合、このdefineを有効にする

// [ファイルスコープ変数]
static uint f_dmaCh_tx = 0; // 送信のDMAチャンネル
static uint f_dmaCh_rx = 0; // 受信のDMAチャンネル

// SPIマスタ送受信
void SPI_SendRecv(PVOID pSendBuf, PVOID pRecvBuf, ULONG sendRecvSize)
{
    dma_channel_config dmaChCfg; // DMAチャンネル設定

    // 送信のDMAチャンネルの設定
    dmaChCfg = dma_channel_get_default_config(f_dmaCh_tx);
    channel_config_set_transfer_data_size(&dmaChCfg, DMA_SIZE_8);
    channel_config_set_dreq(&dmaChCfg, spi_get_dreq(SPI_ID, true));
    dma_channel_configure(f_dmaCh_tx, &dmaChCfg,
                        &spi_get_hw(SPI_ID)->dr, // write address
                        pSendBuf, // read address
                        sendRecvSize, // element count (each element is of size transfer_data_size)
                        false); // don't start yet

    // 受信のDMAチャンネルの設定
    dmaChCfg = dma_channel_get_default_config(f_dmaCh_rx);
    channel_config_set_transfer_data_size(&dmaChCfg, DMA_SIZE_8);
    channel_config_set_dreq(&dmaChCfg, spi_get_dreq(SPI_ID, false));
    channel_config_set_read_increment(&dmaChCfg, false);
    channel_config_set_write_increment(&dmaChCfg, true);
    dma_channel_configure(f_dmaCh_rx, &dmaChCfg,
                        pRecvBuf, // write address
                        &spi_get_hw(SPI_ID)->dr, // read address
                        sendRecvSize, // element count (each element is of size transfer_data_size)
                        false); // don't start yet         

#ifdef SPI_USE_GPIO_AS_CS
    // CS = Lowを一定時間保持
    gpio_put(SPI_CSN, false);
    busy_wait_us(CS_HOLD_TIME);
#endif

    // DMAを開始
    dma_start_channel_mask((1u << f_dmaCh_tx) | (1u << f_dmaCh_rx));
    // DMA完了待ち
    dma_channel_wait_for_finish_blocking(f_dmaCh_rx);

#ifdef SPI_USE_GPIO_AS_CS
    // CS = Lowを一定時間保持
    busy_wait_us(CS_HOLD_TIME);
    gpio_put(SPI_CSN, true);
    // CS = Highを一定時間保持
    busy_wait_us(CS_HOLD_TIME); 
#endif
}

// ST_SPI_CONFIG構造体にデフォルト値を格納
void SPI_SetDefault(ST_SPI_CONFIG *pstConfig)
{
    pstConfig->frequency = SPI_DEFAULT_FREQ;
    pstConfig->dataBits  = SPI_DEFAULT_DATA_BITS;
    pstConfig->polarity  = SPI_DEFAULT_POLARITY;
    pstConfig->phase     = SPI_DEFAULT_PHASE;
    pstConfig->order     = SPI_DEFAULT_ORDER;
}

// SPIを初期化
void SPI_Init(ST_SPI_CONFIG *pstConfig)
{
    // [SPI0(マスタ)を初期化] 
    spi_init(SPI_ID, pstConfig->frequency);
    hw_clear_bits(&spi_get_hw(SPI_ID)->cr1, SPI_SSPCR1_SSE_BITS); //disable the SPI
    spi_set_format(SPI_ID, pstConfig->dataBits, pstConfig->polarity, pstConfig->phase, pstConfig->order); // 通信設定
    hw_set_bits(&spi_get_hw(SPI_ID)->cr1, SPI_SSPCR1_SSE_BITS); //re-enable the SPI

    // ピン機能
    gpio_set_function(SPI_RX, GPIO_FUNC_SPI);
    gpio_set_function(SPI_SCK, GPIO_FUNC_SPI);
    gpio_set_function(SPI_TX, GPIO_FUNC_SPI);
#ifdef SPI_USE_GPIO_AS_CS
    gpio_init(SPI_CSN);
    gpio_put(SPI_CSN, true); // Highを出力
    gpio_set_dir(SPI_CSN, true);
#else
    gpio_set_function(SPI_CSN, GPIO_FUNC_SPI);
#endif

    //hw_set_bits(&spi_get_hw(SPI_ID)->cr1, SPI_SSPCR1_LBM_BITS); // ループバックを有効

    // [DMAを設定]
    // DMAチャンネルを取得
    f_dmaCh_tx = dma_claim_unused_channel(true);
    f_dmaCh_rx = dma_claim_unused_channel(true);
}