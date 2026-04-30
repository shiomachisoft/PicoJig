// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// [define] / [定義]
#define CMD_WAIT_SEND_END 1000 // Wait time for response frame send completion before writing config data to FLASH and resetting (ms) / FLASHに設定データを書き込んでリセットする前の応答フレームの送信完了待ち時間(ms)
#define CMD_I2C_MIN_REQ_SIZE 3 // Minimum required data size for I2C request (slave address: 1 byte + dataSize: 2 bytes) / I2C要求に必要な最低限のデータサイズ(スレーブアドレス:1byte + データサイズ:2byte)

// [File scope variables] / [ファイルスコープ変数]
static UCHAR f_aResData[FRM_DATA_MAX_SIZE] ={0}; // Data part of response frame / 応答フレームのデータ部

// [Function prototype declarations] / [関数プロトタイプ宣言]
static void CMD_ExecReqCmd_SetGpioConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetGpioConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetGpio(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_PutGpio(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetAdc(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_SetUartConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetUartConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_SendUart(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_SetSpiConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetSpiConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_SendRecvSpi(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_SetI2cConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetI2cConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_SendI2c(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_RecvI2c(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_StartPwm(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_StopPwm(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetFwError(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_ClearFwError(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_EraseFlash(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetFwInfo(ST_FRM_REQ_FRAME *pstReqFrm);
#ifdef MY_BOARD_PICO_W
static void CMD_ExecReqCmd_SetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm);
#endif
static void CMD_ExecReqCmd_ResetMcu(ST_FRM_REQ_FRAME *pstReqFrm);

// Execute request command / 要求コマンドの実行
void CMD_ExecReqCmd(ST_FRM_REQ_FRAME *pstReqFrm)
{   
    switch (pstReqFrm->cmd) 
    {
        // Get FW info command / FW情報取得コマンド
        case CMD_GET_FW_INFO:
            CMD_ExecReqCmd_GetFwInfo(pstReqFrm);
            break; 
        // Set GPIO config command / GPIO設定変更コマンド
        case CMD_SET_GPIO_CONFIG:
            CMD_ExecReqCmd_SetGpioConfig(pstReqFrm);
            break;
        // Get GPIO config command / GPIO設定取得コマンド
        case CMD_GET_GPIO_CONFIG:    
            CMD_ExecReqCmd_GetGpioConfig(pstReqFrm);
            break;
        // Get GPIO input/output value command / GPIO入出力値取得コマンド
        case CMD_GET_GPIO: 
            CMD_ExecReqCmd_GetGpio(pstReqFrm);
            break;       
        // Set GPIO output command / GPIO出力コマンド    
        case CMD_OUT_GPIO:
            CMD_ExecReqCmd_PutGpio(pstReqFrm);
            break;           
        // Get ADC/temperature command / ADC・温度入力コマンド
        case CMD_GET_ADC:
            CMD_ExecReqCmd_GetAdc(pstReqFrm);
            break;             
        // Set UART config command / UART通信設定変更コマンド
        case CMD_SET_UART_CONFIG:
            CMD_ExecReqCmd_SetUartConfig(pstReqFrm);
            break;
        // Get UART config command / UART通信設定取得コマンド
        case CMD_GET_UART_CONFIG:
            CMD_ExecReqCmd_GetUartConfig(pstReqFrm);
            break;   
        // Send UART command / UART送信コマンド
        case CMD_SEND_UART:
            CMD_ExecReqCmd_SendUart(pstReqFrm);
            break;  
        // Set SPI config command / SPI通信設定変更コマンド     
        case CMD_SET_SPI_CONFIG:
            CMD_ExecReqCmd_SetSpiConfig(pstReqFrm);
            break;        
        // Get SPI config command / SPI通信設定取得コマンド     
        case CMD_GET_SPI_CONFIG:
            CMD_ExecReqCmd_GetSpiConfig(pstReqFrm);
            break;   
        // SPI master send/receive command / SPIマスタ送受信コマンド     
        case CMD_SENDRECV_SPI:
            CMD_ExecReqCmd_SendRecvSpi(pstReqFrm);
            break;   
        // Set I2C config command / I2C通信設定変更コマンド    
        case CMD_SET_I2C_CONFIG:
            CMD_ExecReqCmd_SetI2cConfig(pstReqFrm);
            break;   
        // Get I2C config command / I2C通信設定取得コマンド    
        case CMD_GET_I2C_CONFIG:
            CMD_ExecReqCmd_GetI2cConfig(pstReqFrm);
            break;              
        // I2C master send command / I2Cマスタ送信コマンド 
        case CMD_SEND_I2C:
            CMD_ExecReqCmd_SendI2c(pstReqFrm);
            break;
        // I2C master receive command / I2Cマスタ受信コマンド     
        case CMD_RECV_I2C:    
            CMD_ExecReqCmd_RecvI2c(pstReqFrm);
            break;   
        // Start PWM command / PWM開始コマンド
        case CMD_START_PWM:
            CMD_ExecReqCmd_StartPwm(pstReqFrm);
            break;        
        // Stop PWM command / PWM停止コマンド
        case CMD_STOP_PWM:
            CMD_ExecReqCmd_StopPwm(pstReqFrm);
            break;              
        // Get FW error command / FWエラー取得コマンド
        case CMD_GET_FW_ERR:
            CMD_ExecReqCmd_GetFwError(pstReqFrm);
            break;    
        // Clear FW error command / FWエラークリアコマンド
        case CMD_CLEAR_FW_ERR:
            CMD_ExecReqCmd_ClearFwError(pstReqFrm);
            break; 
        // Erase FLASH command / FLASH消去コマンド    
        case CMD_ERASE_FLASH: 
            CMD_ExecReqCmd_EraseFlash(pstReqFrm);
            break;             
#ifdef MY_BOARD_PICO_W
        // Set network config command / ネットワーク設定変更コマンド
        case CMD_SET_NW_CONFIG:
            CMD_ExecReqCmd_SetNwConfig(pstReqFrm);
            break;    
        // Get network config command / ネットワーク設定取得コマンド
        case CMD_GET_NW_CONFIG:
            CMD_ExecReqCmd_GetNwConfig(pstReqFrm);
            break;         
#endif
        // Reset MCU command / マイコンリセットコマンド
        case CMD_RESET_MCU:
            CMD_ExecReqCmd_ResetMcu(pstReqFrm);
            break;
        default:
            break;       
    }
}

// Execute get FW info command / FW情報取得コマンドの実行
static void CMD_ExecReqCmd_GetFwInfo(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FW_INFO stFwInfo = {0};          // FW info / FW情報
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ
    
    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        strcpy(stFwInfo.szMakerName, MAKER_NAME);     // Maker name / メーカー名
        strcpy(stFwInfo.szFwName, FW_NAME);           // FW name / FW名 
        stFwInfo.fwVer = FW_VER;                      // FW version / FWバージョン
        pico_get_unique_board_id(&stFwInfo.board_id); // Unique board ID size = PICO_UNIQUE_BOARD_ID_SIZE_BYTES / ユニークボードID サイズ = PICO_UNIQUE_BOARD_ID_SIZE_BYTES       
    
        dataSize = sizeof(stFwInfo); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&stFwInfo;     // Data of response frame / 応答フレームのデータ     
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);
}

// Set GPIO config command / GPIO設定変更コマンド
static void CMD_ExecReqCmd_SetGpioConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FLASH_DATA stFlashData = {0};    // FLASH data / FLASHデータ

    // Check data size / データサイズをチェック
    expectedSize = sizeof(ST_GPIO_CONFIG);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系    
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [Write to FLASH] / [FLASHへ書き込み]
        // Read FLASH data / FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // Write to FLASH / FLASHへ書き込み
        memcpy(&stFlashData.stGpioConfig, &pstReqFrm->aData[0], sizeof(ST_GPIO_CONFIG));
        FLASH_Write(&stFlashData); // MCU will be reset / マイコンはリセットされる        
    }
}

// Get GPIO config command / GPIO設定取得コマンド
static void CMD_ExecReqCmd_GetGpioConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FLASH_DATA *pstFlashData = FLASH_GetDataAtPowerOn(); // FLASH data at power-on / 電源起動時のFLASHデータ
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        dataSize = sizeof(pstFlashData->stGpioConfig); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&pstFlashData->stGpioConfig;     // Data of response frame / 応答フレームのデータ            
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);    
}

// Execute get GPIO input/output value command / GPIO入出力値取得コマンドの実行
static void CMD_ExecReqCmd_GetGpio(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;          // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;              // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS; // Error code / エラーコード
    ULONG inOutValBits = 0;           // All current GPIO input/output values / 現在の全てのGPIO入出力値
    PVOID pBuf = NULL;                // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get all current GPIO input/output values / 現在の全てのGPIO入出力値を取得
        inOutValBits = gpio_get_all();
        inOutValBits &= (GPIO_GetInDirBits() | GPIO_GetOutDirBits());

        dataSize = sizeof(inOutValBits); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&inOutValBits;     // Data of response frame / 応答フレームのデータ          
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);  
}

// Execute set GPIO output command / GPIO出力コマンドの実行
static void CMD_ExecReqCmd_PutGpio(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード
    ULONG maskBits = 0;                 // Bit mask / ビットマスク
    ULONG outValBits = 0;               // GPIO output value / GPIO出力値

    // Check data size / データサイズをチェック
    expectedSize = sizeof(outValBits);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get arguments / 引数を取得
        memcpy(&outValBits, pstReqFrm->aData, sizeof(outValBits));
        // Get bit mask / ビットマスクを取得
        maskBits = GPIO_GetOutDirBits();
        // GPIO output / GPIO出力
        gpio_put_masked(maskBits, outValBits);
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
}

// Execute get ADC input command / ADC入力コマンドの実行
static void CMD_ExecReqCmd_GetAdc(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS; // Error code / エラーコード
    ULONG i;
    float adcVal = 0; // AD conversion value / AD変換値
    float aVolt[ADC_CH_NUM] = {0}; // Voltage/Temperature / 電圧・温度
    const float conversionFactor = 3.3f / (1 << 12); // 12-bit conversion, assume max value == ADC_VREF == 3.3 V / 12ビット変換、最大値 == ADC_VREF == 3.3Vと想定
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get voltage / 電圧を取得
        for (i = 0; i < ADC_CH_NUM_WITHOUT_TEMP; i++) {
            adc_select_input(i);
            adcVal = (float)adc_read();
            aVolt[i] = adcVal * conversionFactor;
        }
        // Get temperature from temperature sensor / 温度センサの温度を取得
        adc_select_input(4);
        adcVal = (float)adc_read() * conversionFactor;
        aVolt[ADC_CH_NUM_WITHOUT_TEMP] = 27.0f - (adcVal - 0.706f) / 0.001721f;

        dataSize = sizeof(aVolt); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)aVolt;      // Data of response frame / 応答フレームのデータ           
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);
}

// Execute set UART config command / UART通信設定変更コマンドの実行
static void CMD_ExecReqCmd_SetUartConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_UART_CONFIG stUartConfig = {0};  // UART config / UART通信設定
    ST_FLASH_DATA stFlashData = {0};    // FLASH data / FLASHデータ

    // Check data size / データサイズをチェック
    expectedSize = sizeof(stUartConfig);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { 
        // Get arguments / 引数を取得
        memcpy(&stUartConfig, &pstReqFrm->aData[0], sizeof(stUartConfig)); // UART config / UART通信設定
        // Check arguments / 引数をチェック
        if (stUartConfig.dataBits != 8) { // Data bit length out of range / データビット長が範囲外
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }
        else if ((stUartConfig.stopBits != 1) && (stUartConfig.stopBits != 2)) { // Stop bit length out of range / ストップビット長が範囲外
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }
        else if ((stUartConfig.parity != UART_PARITY_NONE) // Parity out of range / パリティが範囲外
            && (stUartConfig.parity != UART_PARITY_EVEN) 
            && (stUartConfig.parity != UART_PARITY_ODD)) {
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }        
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系     
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [Write to FLASH] / [FLASHへ書き込み]
        // Read FLASH data / FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // Write to FLASH / FLASHへ書き込み
        memcpy(&stFlashData.stUartConfig, &stUartConfig, sizeof(stUartConfig));
        FLASH_Write(&stFlashData); // MCU will be reset / マイコンはリセットされる       
    }
}

// Execute get UART config command / UART通信設定取得コマンドの実行
static void CMD_ExecReqCmd_GetUartConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FLASH_DATA *pstFlashData = FLASH_GetDataAtPowerOn(); // FLASH data at power-on / 電源起動時のFLASHデータ
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        dataSize = sizeof(pstFlashData->stUartConfig); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&pstFlashData->stUartConfig;     // Data of response frame / 応答フレームのデータ        
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);
}

// Execute send UART command / UART送信コマンドの実行
static void CMD_ExecReqCmd_SendUart(ST_FRM_REQ_FRAME *pstReqFrm)
{
    ULONG i;
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 

    // Check data size / データサイズをチェック
    if ((pstReqFrm->dataSize > 0) && (pstReqFrm->dataSize <= UART_DATA_MAX_SIZE)) { 
        for (i = 0; i < pstReqFrm->dataSize; i++) { // Repeat for data size / データサイズ分繰り返す
            // Enqueue 1 byte of UART send data / UART送信データ1byteのエンキュー
            if (!CMN_Enqueue(CMN_QUE_KIND_UART_SEND, &pstReqFrm->aData[i], true)) {
                errCode = FRM_ERR_BUF_NOT_ENOUGH; // Queue is full / キューが満杯
                break; // Queue is full / キューが満杯
            }
        }
    }
    else {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
}

// Set SPI config command / SPI通信設定変更コマンド
static void CMD_ExecReqCmd_SetSpiConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_SPI_CONFIG stSpiConfig = {0};    // SPI config / SPI通信設定
    ST_FLASH_DATA stFlashData = {0};    // FLASH data / FLASHデータ

    // Check data size / データサイズをチェック
    expectedSize = sizeof(stSpiConfig);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { 
        // Get arguments / 引数を取得
        memcpy(&stSpiConfig, &pstReqFrm->aData[0], sizeof(stSpiConfig)); // SPI config / SPI通信設定

        // Check arguments / 引数をチェック
        if (stSpiConfig.dataBits != 8) { // Data bit length out of range / データビット長が範囲外
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }
        else if ((stSpiConfig.polarity != SPI_CPOL_0) && (stSpiConfig.polarity != SPI_CPOL_1)) { // Polarity out of range / 極性が範囲外
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }
        else if ((stSpiConfig.phase != SPI_CPHA_0) && (stSpiConfig.phase != SPI_CPHA_1)) { // Phase out of range / 位相が範囲外
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }     
        else if (stSpiConfig.order != SPI_MSB_FIRST) { // Unsupported bit order / サポートされていないビットオーダー
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }             
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系      
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [Write to FLASH] / [FLASHへ書き込み]
        // Read FLASH data / FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // Write to FLASH / FLASHへ書き込み
        memcpy(&stFlashData.stSpiConfig, &stSpiConfig, sizeof(stSpiConfig));  
        FLASH_Write(&stFlashData); // MCU will be reset / マイコンはリセットされる    
    }
}

// Execute get SPI config command / SPI通信設定取得コマンドの実行
static void CMD_ExecReqCmd_GetSpiConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FLASH_DATA *pstFlashData = FLASH_GetDataAtPowerOn(); // FLASH data at power-on / 電源起動時のFLASHデータ
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        dataSize = sizeof(pstFlashData->stSpiConfig); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&pstFlashData->stSpiConfig;     // Data of response frame / 応答フレームのデータ   
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);
}

// Execute SPI master send/receive command / SPIマスタ送受信コマンドの実行
static void CMD_ExecReqCmd_SendRecvSpi(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if ((pstReqFrm->dataSize < 1) || (pstReqFrm->dataSize > SPI_DATA_MAX_SIZE)) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Initialize receive buffer / 受信バッファを初期化
        memset(f_aResData, 0, sizeof(f_aResData));

        // SPI master send/receive / SPIマスタ送受信
        SPI_SendRecv(pstReqFrm->aData, f_aResData, pstReqFrm->dataSize);

        dataSize = pstReqFrm->dataSize; // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)f_aResData;       // Data of response frame / 応答フレームのデータ          
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);
}

// Execute set I2C config command / I2C通信設定変更コマンドの実行
static void CMD_ExecReqCmd_SetI2cConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_I2C_CONFIG stI2cConfig = {0};    // I2C config / I2C通信設定
    ST_FLASH_DATA stFlashData = {0};    // FLASH data / FLASHデータ

    // Check data size / データサイズをチェック
    expectedSize = sizeof(stI2cConfig);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get arguments / 引数を取得
        memcpy(&stI2cConfig, &pstReqFrm->aData[0], sizeof(stI2cConfig)); // I2C config / I2C通信設定
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [Write to FLASH] / [FLASHへ書き込み]
        // Read FLASH data / FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // Write to FLASH / FLASHへ書き込み
        memcpy(&stFlashData.stI2cConfig, &stI2cConfig, sizeof(stI2cConfig));
        FLASH_Write(&stFlashData); // MCU will be reset / マイコンはリセットされる   
    }
}

// Execute get I2C config command / I2C通信設定取得コマンドの実行
static void CMD_ExecReqCmd_GetI2cConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FLASH_DATA *pstFlashData = FLASH_GetDataAtPowerOn(); // FLASH data at power-on / 電源起動時のFLASHデータ
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        dataSize = sizeof(pstFlashData->stI2cConfig); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&pstFlashData->stI2cConfig;     // Data of response frame / 応答フレームのデータ           
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf); 
}

// Execute I2C master send command / I2Cマスタ送信コマンドの実行
static void CMD_ExecReqCmd_SendI2c(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_I2C_REQ stI2cReq = {0};          // I2C send request / I2C送信要求

    // Check minimum required data size (slave address: 1 byte + dataSize: 2 bytes) / 最低限必要なデータサイズ(スレーブアドレス:1byte + データサイズ:2byte)をチェック
    if (pstReqFrm->dataSize < CMD_I2C_MIN_REQ_SIZE) {
        errCode = FRM_ERR_DATA_SIZE;
    }
    else {
        memcpy(&stI2cReq.dataSize, &pstReqFrm->aData[1], sizeof(stI2cReq.dataSize)); // I2C send size / I2C送信サイズ 
        // Check I2C send size / I2C送信サイズをチェック   
        if ((stI2cReq.dataSize < 1) || (stI2cReq.dataSize > I2C_DATA_MAX_SIZE)) { 
            errCode = FRM_ERR_PARAM; // Invalid argument (Invalid I2C send size) / 引数が不正(I2C送信サイズが不正)
        }
        else {  
            // Check data size / データサイズをチェック
            expectedSize = sizeof(stI2cReq.slaveAddr) + sizeof(stI2cReq.dataSize) + stI2cReq.dataSize; // Slave address + Size of send size area + Send size / スレーブアドレス + 送信サイズ領域のサイズ + 送信サイズ
            if (pstReqFrm->dataSize != expectedSize) {
                errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
            }         
        }

        stI2cReq.slaveAddr = pstReqFrm->aData[0]; // Slave address / スレーブアドレス
        if ((errCode == FRM_ERR_SUCCESS) && (stI2cReq.slaveAddr > 0x7F)) {
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }
    }

    if (errCode == FRM_ERR_SUCCESS) { // If no error / エラー無しの場合
        // Create I2C send request to enqueue / エンキューするI2C送信要求を作成
        stI2cReq.seqNo = pstReqFrm->seqNo; // Sequence number *Saved to return response later / シーケンス番号 ※後で応答を返すために保存
        stI2cReq.cmd = pstReqFrm->cmd;     // Command *Saved to return response later / コマンド ※後で応答を返すために保存
        memcpy(stI2cReq.aData, &pstReqFrm->aData[CMD_I2C_MIN_REQ_SIZE], stI2cReq.dataSize); // Send data / 送信データ
        // Enqueue I2C send request / I2C送信要求のエンキュー
        if (!CMN_Enqueue(CMN_QUE_KIND_I2C_REQ, &stI2cReq, true)) {
            errCode = FRM_ERR_BUF_NOT_ENOUGH;
        }    
    }

    if (errCode != FRM_ERR_SUCCESS) { // If error / エラー有りの場合 
        // Send response frame / 応答フレームを送信        
        FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
    }    
}

// Execute I2C master receive command / I2Cマスタ受信コマンドの実行
static void CMD_ExecReqCmd_RecvI2c(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_I2C_REQ stI2cReq = {0};          // I2C receive request / I2C受信要求

    // Check minimum required data size (slave address: 1 byte + dataSize: 2 bytes) / 最低限必要なデータサイズ(スレーブアドレス:1byte + データサイズ:2byte)をチェック
    if (pstReqFrm->dataSize < CMD_I2C_MIN_REQ_SIZE) {
        errCode = FRM_ERR_DATA_SIZE;
    }
    else {
        memcpy(&stI2cReq.dataSize, &pstReqFrm->aData[1], sizeof(stI2cReq.dataSize)); // I2C receive size / I2C受信サイズ  
        // Check I2C receive size / I2C受信サイズをチェック
        if ((stI2cReq.dataSize < 1) || (stI2cReq.dataSize > I2C_DATA_MAX_SIZE)) {
            errCode = FRM_ERR_PARAM; // Invalid argument (Invalid I2C receive size) / 引数が不正(I2C受信サイズが不正)
        }
        else {
            // Check data size / データサイズをチェック
            expectedSize = sizeof(stI2cReq.slaveAddr) + sizeof(stI2cReq.dataSize);  // Slave address + Size of receive size area / スレーブアドレス + 受信サイズ領域のサイズ
            if (pstReqFrm->dataSize != expectedSize) {
                errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
            }
        }

        stI2cReq.slaveAddr = pstReqFrm->aData[0];  // Slave address / スレーブアドレス
        if ((errCode == FRM_ERR_SUCCESS) && (stI2cReq.slaveAddr > 0x7F)) {
            errCode = FRM_ERR_PARAM; // Invalid argument / 引数が不正
        }
    }

    if (errCode == FRM_ERR_SUCCESS) { // If no error / エラー無しの場合
        // Create I2C receive request to enqueue / エンキューするI2C受信要求を作成
        stI2cReq.seqNo = pstReqFrm->seqNo; // Sequence number *Saved to return response later / シーケンス番号 ※後で応答を返すために保存
        stI2cReq.cmd = pstReqFrm->cmd;     // Command *Saved to return response later / コマンド ※後で応答を返すために保存
        // Enqueue I2C receive request / I2C受信要求のエンキュー
        if (!CMN_Enqueue(CMN_QUE_KIND_I2C_REQ, &stI2cReq, true)) {
            errCode = FRM_ERR_BUF_NOT_ENOUGH;
        }    
    }

    if (errCode != FRM_ERR_SUCCESS) { // If error / エラー有りの場合 
        // Send response frame / 応答フレームを送信        
        FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
    }
}

// Execute start PWM command / PWM開始コマンドの実行
static void CMD_ExecReqCmd_StartPwm(ST_FRM_REQ_FRAME *pstReqFrm)
{  
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_PWM_CONFIG stPwmConfig = {0};    // PWM config / PWM設定

    // Check data size / データサイズをチェック
    expectedSize = sizeof(stPwmConfig);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get arguments / 引数を取得
        memcpy(&stPwmConfig, pstReqFrm->aData, sizeof(stPwmConfig)); // PWM config / PWM設定
        // Start PWM / PWM開始
        PWM_Start(&stPwmConfig);        
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);    
}

// Execute stop PWM command / PWM停止コマンドの実行
static void CMD_ExecReqCmd_StopPwm(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系      
        // Stop PWM / PWM停止
        PWM_Stop(); 
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);           
}

// Execute get FW error command / FWエラー取得コマンドの実行
static void CMD_ExecReqCmd_GetFwError(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ULONG errorBits = 0;                // FW error / FWエラー
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get FW error / FWエラーを取得
        errorBits = CMN_GetFwErrorBits();     
        
        dataSize = sizeof(errorBits); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&errorBits;     // Data of response frame / 応答フレームのデータ         
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf); 
}

// Execute clear FW error command / FWエラークリアコマンドの実行
static void CMD_ExecReqCmd_ClearFwError(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Clear FW error / FWエラークリア
        CMN_ClearFwErrorBits(true);
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL); 
}

// Execute erase FLASH command / FLASH消去コマンドの実行
static void CMD_ExecReqCmd_EraseFlash(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE;    // Invalid data size / データサイズが不正
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL); 

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);         
        // Erase FLASH / FLASH消去
        FLASH_Erase(); // MCU will be reset / マイコンはリセットされる   
    }
}

#ifdef MY_BOARD_PICO_W
// Execute set network config command / ネットワーク設定変更コマンドの実行
static void CMD_ExecReqCmd_SetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_NW_CONFIG stNwConfig = {0};      // Network config / ネットワーク設定
    ST_FLASH_DATA stFlashData = {0};    // FLASH data / FLASHデータ

    // Check data size / データサイズをチェック
    expectedSize = sizeof(stNwConfig);
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        // Get arguments / 引数を取得
        memcpy(&stNwConfig, &pstReqFrm->aData[0], sizeof(stNwConfig)); // Network config / ネットワーク設定
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [Write to FLASH] / [FLASHへ書き込み]
        // Read FLASH data / FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // Write to FLASH / FLASHへ書き込み
        memcpy(&stFlashData.stNwConfig, &stNwConfig, sizeof(stNwConfig));
        FLASH_Write(&stFlashData); // MCU will be reset / マイコンはリセットされる   
    }
}

// Execute get network config command / ネットワーク設定取得コマンドの実行
static void CMD_ExecReqCmd_GetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT dataSize = 0;                // Data size of response frame / 応答フレームのデータサイズ
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 
    ST_FLASH_DATA *pstFlashData = FLASH_GetDataAtPowerOn(); // FLASH data at power-on / 電源起動時のFLASHデータ
    PVOID pBuf = NULL;                  // Data of response frame / 応答フレームのデータ

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }
    else { // Normal case / 正常系
        dataSize = sizeof(pstFlashData->stNwConfig); // Data size of response frame / 応答フレームのデータサイズ
        pBuf = (PVOID)&pstFlashData->stNwConfig;     // Data of response frame / 応答フレームのデータ  
    }

    // Send response frame / 応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, dataSize, pBuf);    
}
#endif

// Execute reset MCU command / マイコンリセットコマンドの実行
static void CMD_ExecReqCmd_ResetMcu(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT expectedSize = 0;            // Expected data size of request frame / 要求フレームのデータサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // Error code / エラーコード 

    // Check data size / データサイズをチェック
    if (pstReqFrm->dataSize != expectedSize) {
        errCode = FRM_ERR_DATA_SIZE; // Invalid data size / データサイズが不正
    }

    // Send response frame (Success/Failure) / 成功・失敗に関わらず応答フレームを送信        
    FRM_SendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);

    if (errCode == FRM_ERR_SUCCESS) { // Normal case / 正常系
        // Wait for CPU core 1 to send response frame / CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // Reset MCU / マイコンをリセット
        // Reboot immediately by WDT timeout without using watchdog_enable() / watchdog_enable()を使用しないで即WDTタイムアウトで再起動する
        CMN_WdtRebootWithoutEnable();           
    }
}