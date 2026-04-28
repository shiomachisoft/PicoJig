// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義]
#define SPI_ID spi0    // SPI ID / SPIのID
#define CS_HOLD_TIME 5 // CS High/Low hold time (us). Set to a time longer than 1 bit (preferably 2 bits or more). / CSのHigh/Low保持時間(us)。1bit分(できれば2bit分以上)の時間以上に設定する。

// Default values for SPI config / SPI設定のデフォルト値
#define SPI_DEFAULT_FREQ      1000000UL     // Clock frequency (Hz) / クロック周波数(Hz) 
#define SPI_DEFAULT_DATA_BITS 8             // Data bit length / データビット長
#define SPI_DEFAULT_POLARITY  SPI_CPOL_0    // Polarity / 極性
#define SPI_DEFAULT_PHASE     SPI_CPHA_0    // Phase / 位相
#define SPI_DEFAULT_ORDER     SPI_MSB_FIRST // bit order / ビットオーダー

#define SPI_USE_GPIO_AS_CS // Enable this define when using GPIO for CS / CSはGPIOを使用する場合、このdefineを有効にする

// [File scope variables] / [ファイルスコープ変数]
static uint f_dmaCh_tx = 0; // Transmit DMA channel / 送信のDMAチャンネル
static uint f_dmaCh_rx = 0; // Receive DMA channel / 受信のDMAチャンネル

// SPI master send/receive / SPIマスタ送受信
void SPI_SendRecv(PVOID pSendBuf, PVOID pRecvBuf, ULONG sendRecvSize)
{
    dma_channel_config dmaChCfg; // DMA channel config / DMAチャンネル設定

    // Drain any stale data from the SPI RX FIFO before starting a new transaction / 新しいトランザクションを開始する前に、SPI RX FIFOから古いデータを排出する
    while (spi_is_readable(SPI_ID)) { 
        (void)spi_get_hw(SPI_ID)->dr; 
    }

    // Set transmit DMA channel / 送信のDMAチャンネルの設定
    dmaChCfg = dma_channel_get_default_config(f_dmaCh_tx);
    channel_config_set_transfer_data_size(&dmaChCfg, DMA_SIZE_8);
    channel_config_set_dreq(&dmaChCfg, spi_get_dreq(SPI_ID, true));
    dma_channel_configure(f_dmaCh_tx, &dmaChCfg,
                        &spi_get_hw(SPI_ID)->dr, // Write address / 書き込みアドレス
                        pSendBuf, // Read address / 読み取りアドレス
                        sendRecvSize, // Element count (each element is of size transfer_data_size) / 要素数 (各要素のサイズはtransfer_data_size)
                        false); // Don't start yet / まだ開始しない

    // Set receive DMA channel / 受信のDMAチャンネルの設定
    dmaChCfg = dma_channel_get_default_config(f_dmaCh_rx);
    channel_config_set_transfer_data_size(&dmaChCfg, DMA_SIZE_8);
    channel_config_set_dreq(&dmaChCfg, spi_get_dreq(SPI_ID, false));
    channel_config_set_read_increment(&dmaChCfg, false);
    channel_config_set_write_increment(&dmaChCfg, true);
    dma_channel_configure(f_dmaCh_rx, &dmaChCfg,
                        pRecvBuf, // Write address / 書き込みアドレス
                        &spi_get_hw(SPI_ID)->dr, // Read address / 読み取りアドレス
                        sendRecvSize, // Element count (each element is of size transfer_data_size) / 要素数 (各要素のサイズはtransfer_data_size)
                        false); // Don't start yet / まだ開始しない         

#ifdef SPI_USE_GPIO_AS_CS
    // Hold CS = Low for a certain time / CS = Lowを一定時間保持
    gpio_put(SPI_CSN, false);
    busy_wait_us(CS_HOLD_TIME);
#endif

    // Start DMA / DMAを開始
    dma_start_channel_mask((1u << f_dmaCh_tx) | (1u << f_dmaCh_rx));
    // Wait for DMA completion / DMA完了待ち
    dma_channel_wait_for_finish_blocking(f_dmaCh_tx);
    dma_channel_wait_for_finish_blocking(f_dmaCh_rx);

#ifdef SPI_USE_GPIO_AS_CS
    // Hold CS = Low for a certain time / CS = Lowを一定時間保持
    busy_wait_us(CS_HOLD_TIME);
    gpio_put(SPI_CSN, true);
    // Hold CS = High for a certain time / CS = Highを一定時間保持
    busy_wait_us(CS_HOLD_TIME); 
#endif
}

// Store default values in ST_SPI_CONFIG structure / ST_SPI_CONFIG構造体にデフォルト値を格納
void SPI_SetDefault(ST_SPI_CONFIG *pstConfig)
{
    pstConfig->frequency = SPI_DEFAULT_FREQ;
    pstConfig->dataBits  = SPI_DEFAULT_DATA_BITS;
    pstConfig->polarity  = SPI_DEFAULT_POLARITY;
    pstConfig->phase     = SPI_DEFAULT_PHASE;
    pstConfig->order     = SPI_DEFAULT_ORDER;
}

// Initialize SPI / SPIを初期化
void SPI_Init(ST_SPI_CONFIG *pstConfig)
{
    // [Initialize SPI0 (master)] / [SPI0(マスタ)を初期化] 
    spi_init(SPI_ID, pstConfig->frequency);
    spi_set_format(SPI_ID, pstConfig->dataBits, pstConfig->polarity, pstConfig->phase, pstConfig->order); // Communication config / 通信設定

    // Pin function / ピン機能
    gpio_set_function(SPI_RX, GPIO_FUNC_SPI);
    gpio_set_function(SPI_SCK, GPIO_FUNC_SPI);
    gpio_set_function(SPI_TX, GPIO_FUNC_SPI);
#ifdef SPI_USE_GPIO_AS_CS
    gpio_init(SPI_CSN);
    gpio_put(SPI_CSN, true); // Output High / Highを出力
    gpio_set_dir(SPI_CSN, true);
#else
    gpio_set_function(SPI_CSN, GPIO_FUNC_SPI);
#endif

    //hw_set_bits(&spi_get_hw(SPI_ID)->cr1, SPI_SSPCR1_LBM_BITS); // Enable loopback / ループバックを有効

    // [Set DMA] / [DMAを設定]
    // Get DMA channel / DMAチャンネルを取得
    f_dmaCh_tx = dma_claim_unused_channel(true);
    f_dmaCh_rx = dma_claim_unused_channel(true);
}