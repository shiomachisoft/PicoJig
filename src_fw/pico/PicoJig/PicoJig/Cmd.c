#include "Common.h"

// [define]
#define CMD_WAIT_SEND_END 1000 // FLASHに設定データを書き込んでリセットする前の応答フレームの送信完了待ち時間(ms)

// [ファイルスコープ変数]
static UCHAR f_aResData[FRM_DATA_MAX_SIZE] ={0}; // 応答フレームのデータ部

// [関数プロトタイプ宣言]
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
static void CMD_ExecReqCmd_SetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm);
static void CMD_ExecReqCmd_GetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm);

// 要求コマンド実行
void CMD_ExecReqCmd(ST_FRM_REQ_FRAME *pstReqFrm)
{   
    // 要求コマンドの実行
    switch (pstReqFrm->cmd) 
    {
    // FW情報取得コマンド
    case CMD_GET_FW_INFO:
        CMD_ExecReqCmd_GetFwInfo(pstReqFrm);
        break; 
    // GPIO設定変更コマンド
    case CMD_SET_GPIO_CONFIG:
        CMD_ExecReqCmd_SetGpioConfig(pstReqFrm);
        break;
    // GPIO設定取得コマンド
    case CMD_GET_GPIO_CONFIG:    
        CMD_ExecReqCmd_GetGpioConfig(pstReqFrm);
        break;
    // GPIO入力コマンド
     case CMD_GET_GPIO: 
        CMD_ExecReqCmd_GetGpio(pstReqFrm);
        break;       
    // GPIO出力コマンド    
    case CMD_OUT_GPIO:
        CMD_ExecReqCmd_PutGpio(pstReqFrm);
        break;           
    // ADC・温度入力コマンド
    case CMD_GET_ADC:
        CMD_ExecReqCmd_GetAdc(pstReqFrm);
        break;             
    // UART通信設定変更コマンド
    case CMD_SET_UART_CONFIG:
        CMD_ExecReqCmd_SetUartConfig(pstReqFrm);
        break;
    // UART通信設定取得コマンド
    case CMD_GET_UART_CONFIG:
        CMD_ExecReqCmd_GetUartConfig(pstReqFrm);
        break;   
    // UART送信コマンド
    case CMD_SEND_UART:
        CMD_ExecReqCmd_SendUart(pstReqFrm);
        break;  
    // SPI通信設定変更コマンド     
    case CMD_SET_SPI_CONFIG:
        CMD_ExecReqCmd_SetSpiConfig(pstReqFrm);
        break;        
    // SPI通信設定取得コマンド     
    case CMD_GET_SPI_CONFIG:
        CMD_ExecReqCmd_GetSpiConfig(pstReqFrm);
        break;   
    // SPIマスタ送信受信コマンド     
    case CMD_SENDRECV_SPI:
        CMD_ExecReqCmd_SendRecvSpi(pstReqFrm);
        break;   
    // I2C通信設定変更コマンド    
    case CMD_SET_I2C_CONFIG:
        CMD_ExecReqCmd_SetI2cConfig(pstReqFrm);
        break;   
    // I2C通信設定取得コマンド    
    case CMD_GET_I2C_CONFIG:
        CMD_ExecReqCmd_GetI2cConfig(pstReqFrm);
        break;              
    // I2Cマスタ送信コマンド 
    case CMD_SEND_I2C:
        CMD_ExecReqCmd_SendI2c(pstReqFrm);
        break;
    // I2Cマスタ受信コマンド     
    case CMD_RECV_I2C:    
        CMD_ExecReqCmd_RecvI2c(pstReqFrm);
        break;   
    // PWM開始コマンド
    case CMD_START_PWM:
        CMD_ExecReqCmd_StartPwm(pstReqFrm);
        break;        
     // PWM停止コマンド
    case CMD_STOP_PWM:
        CMD_ExecReqCmd_StopPwm(pstReqFrm);
        break;              
    // FWエラー取得コマンド
    case CMD_GET_FW_ERR:
        CMD_ExecReqCmd_GetFwError(pstReqFrm);
        break;    
    // FWエラークリアコマンド
    case CMD_CLEAR_FW_ERR:
        CMD_ExecReqCmd_ClearFwError(pstReqFrm);
        break; 
    // FLASH消去コマンド    
    case CMD_ERASE_FLASH: 
        CMD_ExecReqCmd_EraseFlash(pstReqFrm);
        break;             
    // ネットワーク設定変更コマンド
    case CMD_SET_NW_CONFIG:
        CMD_ExecReqCmd_SetNwConfig(pstReqFrm);
        break;    
    // ネットワーク設定取得コマンド
    case CMD_GET_NW_CONFIG:
        CMD_ExecReqCmd_GetNwConfig(pstReqFrm);
        break;         
    default:
        break;       
    }
}

// FW情報取得コマンドの実行
static void CMD_ExecReqCmd_GetFwInfo(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FW_INFO stFwInfo;                // FW情報
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        memset(&stFwInfo, 0, sizeof(stFwInfo));
        strcpy(stFwInfo.szMakerName, MAKER_NAME);     // メーカー名
        strcpy(stFwInfo.szFwName, FW_NAME);           // FW名 
        stFwInfo.fwVer = FW_VER;                      // FWバージョン
        pico_get_unique_board_id(&stFwInfo.board_id); // ユニークボードID サイズ = PICO_UNIQUE_BOARD_ID_SIZE_BYTES       
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(stFwInfo), &stFwInfo);
}

// GPIO設定変更コマンド
static void CMD_ExecReqCmd_SetGpioConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FLASH_DATA stFlashData;          // FLASHデータ

    // データサイズをチェック
    dataSize = sizeof(ST_GPIO_CONFIG);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系    
        // 成功の応答フレームを送信 
        FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
        // CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [FLASHへ書き込み]
        // FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // 書き込みデータを用意
        memcpy(&stFlashData.stGpioConfig, &pstReqFrm->aData[0], sizeof(ST_GPIO_CONFIG));
        // FLASHへ書き込み  
        FLASH_Write(&stFlashData); // この関数の中でリセットされる        
    }

    // 失敗の応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);    
}

// GPIO設定取得コマンド
static void CMD_ExecReqCmd_GetGpioConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FLASH_DATA *pstFlashData = NULL; // 電源起動時のFLASHデータ
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 電源起動時のFLASHデータを取得
        pstFlashData = FLASH_GetDataAtPowerOn();
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(pstFlashData->stGpioConfig), &pstFlashData->stGpioConfig);    
}

// GPIO入力コマンドの実行
static void CMD_ExecReqCmd_GetGpio(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;              // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS; // エラーコード
    ULONG valBits = 0;                // 全ての現在GPIO入出力値
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 全ての現在GPIO入出力値を取得
        valBits = gpio_get_all();
        valBits &= (GPIO_GetInDirBits() | GPIO_GetOutDirBits());
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(valBits), &valBits);  
}

// GPIO出力コマンドの実行
static void CMD_ExecReqCmd_PutGpio(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード
    ULONG maskBits = 0;                 // ビットマスク
    ULONG valBits = 0;                  // GPIO出力値

    // データサイズをチェック
    dataSize = sizeof(valBits);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 引数を取得
        memcpy(&valBits, pstReqFrm->aData, sizeof(valBits));
        // ビットマスクを取得
        maskBits = GPIO_GetOutDirBits();
        // GPIO出力
        gpio_put_masked(maskBits, valBits);
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
}

// ADC入力コマンドの実行
static void CMD_ExecReqCmd_GetAdc(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0; // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS; // エラーコード
    ULONG i;
    float adcVal = 0; // AD変換値
    float aVolt[ADC_CH_NUM]; // 電圧・温度
    const float conversionFactor = 3.3f / (1 << 12); // 12-bit conversion, assume max value == ADC_VREF == 3.3 V

    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 電圧を取得
        for (i = 0; i < ADC_CH_NUM_WITHOUT_TEMP; i++) {
            adc_select_input(i);
            adcVal = (float)adc_read();
            aVolt[i] = adcVal * conversionFactor;
        }
        // 温度センサの温度を取得
        adc_select_input(4);
        adcVal = (float)adc_read() * conversionFactor;
        aVolt[ADC_CH_NUM_WITHOUT_TEMP] = 27.0f - (adcVal - 0.706f) / 0.001721f;
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(aVolt), aVolt);
}

// UART通信設定変更コマンドの実行
static void CMD_ExecReqCmd_SetUartConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_UART_CONFIG stUartConfig;        // UART通信設定
    ST_FLASH_DATA stFlashData;          // FLASHデータ

    // データサイズをチェック
    dataSize = sizeof(stUartConfig);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { 
        // 引数を取得
        memcpy(&stUartConfig, &pstReqFrm->aData[0], sizeof(stUartConfig)); // UART通信設定
        // 引数をチェック
        if (stUartConfig.dataBits != 8) { // データビット長が範囲外
            errCode = FRM_ERR_PARAM; // 引数が不正
        }
        else if ((stUartConfig.stopBits != 1) && (stUartConfig.stopBits != 2)) { // ストップビット長が範囲外
            errCode = FRM_ERR_PARAM; // 引数が不正
        }
        else if ((stUartConfig.parity != UART_PARITY_NONE) // パリティが範囲外
            && (stUartConfig.parity != UART_PARITY_EVEN) 
            && (stUartConfig.parity != UART_PARITY_ODD)) {
            errCode = FRM_ERR_PARAM; // 引数が不正
        }        
        else { // 正常系     
            // 成功の応答フレームを送信 
            FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
            // CPUコア1が応答フレームを送信するのを待つ
            busy_wait_ms(CMD_WAIT_SEND_END);
            // [FLASHへ書き込み]
            // FLASHデータ読み込み
            FLASH_Read(&stFlashData);
            // 書き込みデータを用意
            memcpy(&stFlashData.stUartConfig, &stUartConfig, sizeof(stUartConfig));
            // FLASHへ書き込み  
            FLASH_Write(&stFlashData); // この関数の中でリセットされる       
        }
    }

    // 失敗の応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
}

// UART通信設定取得コマンドの実行
static void CMD_ExecReqCmd_GetUartConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FLASH_DATA *pstFlashData = NULL; // 電源起動時のFLASHデータ
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 電源起動時のFLASHデータを取得
        pstFlashData = FLASH_GetDataAtPowerOn();
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(pstFlashData->stUartConfig), &pstFlashData->stUartConfig);
}

// UART送信コマンドの実行
static void CMD_ExecReqCmd_SendUart(ST_FRM_REQ_FRAME *pstReqFrm)
{
    ULONG i;

    // データサイズをチェック
    if ((pstReqFrm->dataSize > 0) && (pstReqFrm->dataSize <= UART_DATA_MAX_SIZE)) { 
        for (i = 0; i < pstReqFrm->dataSize; i++) { // データサイズ分繰り返す
            // UART送信データ1byteのエンキュー
            if (!CMN_Enqueue(CMN_QUE_KIND_UART_SEND, &pstReqFrm->aData[i], sizeof(UCHAR), true)) { // true:UART送信割り込みのデキューと同期する
                break; // キューが満杯
            }
        }
    }
}

// SPI通信設定変更コマンド
static void CMD_ExecReqCmd_SetSpiConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_SPI_CONFIG stSpiConfig;          // SPI通信設定
    ST_FLASH_DATA stFlashData;          // FLASHデータ

    // データサイズをチェック
    dataSize = sizeof(stSpiConfig);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { 
        // 引数を取得
        memcpy(&stSpiConfig, &pstReqFrm->aData[0], sizeof(stSpiConfig)); // SPI通信設定

        // 引数をチェック
        if (stSpiConfig.dataBits != 8) { // データビット長が範囲外
            errCode = FRM_ERR_PARAM; // 引数が不正
        }
        else if ((stSpiConfig.polarity != SPI_CPOL_0) && (stSpiConfig.polarity != SPI_CPOL_1)) { // 極性が範囲外
            errCode = FRM_ERR_PARAM; // 引数が不正
        }
        else if ((stSpiConfig.phase != SPI_CPHA_0) && (stSpiConfig.phase != SPI_CPHA_1)) { // 位相が範囲外
            errCode = FRM_ERR_PARAM; // 引数が不正
        }     
        else if (stSpiConfig.order != SPI_MSB_FIRST) { // バイトオーダーが範囲外
            errCode = FRM_ERR_PARAM; // 引数が不正
        }             
        else { // 正常系      
            // 成功の応答フレームを送信 
            FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
            // CPUコア1が応答フレームを送信するのを待つ
            busy_wait_ms(CMD_WAIT_SEND_END);
            // [FLASHへ書き込み]
            // FLASHデータ読み込み
            FLASH_Read(&stFlashData);
            // 書き込みデータを用意
            memcpy(&stFlashData.stSpiConfig, &stSpiConfig, sizeof(stSpiConfig));  
            // FLASHへ書き込み
            FLASH_Write(&stFlashData); // この関数の中でリセットされる 
        }
    }

    // 失敗の応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
}

// SPI通信設定取得コマンドの実行
static void CMD_ExecReqCmd_GetSpiConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FLASH_DATA *pstFlashData = NULL; // 電源起動時のFLASHデータ
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 電源起動時のFLASHデータを取得
        pstFlashData = FLASH_GetDataAtPowerOn();
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(pstFlashData->stSpiConfig), &pstFlashData->stSpiConfig);
}

// SPIマスタ送受信コマンドの実行
static void CMD_ExecReqCmd_SendRecvSpi(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 

    // データサイズをチェック
    if ((pstReqFrm->dataSize < 1) || (pstReqFrm->dataSize > SPI_DATA_MAX_SIZE)) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // SPIマスタ送受信
        SPI_SendRecv(pstReqFrm->aData, f_aResData, pstReqFrm->dataSize);
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, pstReqFrm->dataSize, f_aResData);
}

// I2C通信設定変更コマンドの実行
static void CMD_ExecReqCmd_SetI2cConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_I2C_CONFIG stI2cConfig;          // I2C通信設定
    ST_FLASH_DATA stFlashData;          // FLASHデータ

    // データサイズをチェック
    dataSize = sizeof(stI2cConfig);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 引数を取得
        memcpy(&stI2cConfig, &pstReqFrm->aData[0], sizeof(stI2cConfig)); // I2C通信設定
        // 成功の応答フレームを送信 
        FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
        // CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [FLASHへ書き込み]
        // FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // 書き込みデータを用意
        memcpy(&stFlashData.stI2cConfig, &stI2cConfig, sizeof(stI2cConfig));
        // FLASHへ書き込み  
        FLASH_Write(&stFlashData); // この関数の中でリセットされる 
    }

    // 失敗の応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);       
}

// I2C通信設定取得コマンドの実行
static void CMD_ExecReqCmd_GetI2cConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FLASH_DATA *pstFlashData = NULL; // 電源起動時のFLASHデータ
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 電源起動時のFLASHデータを取得
        pstFlashData = FLASH_GetDataAtPowerOn();
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(pstFlashData->stI2cConfig), &pstFlashData->stI2cConfig); 
}

// I2Cマスタ送信コマンドの実行
static void CMD_ExecReqCmd_SendI2c(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_I2C_REQ stI2cReq;                // I2C送信要求

    memcpy(&stI2cReq.dataSize, &pstReqFrm->aData[1], sizeof(stI2cReq.dataSize)); // I2C送信サイズ 
    // I2C送信サイズをチェック   
    if ((stI2cReq.dataSize < 1) || (stI2cReq.dataSize > I2C_DATA_MAX_SIZE)) { 
        errCode = FRM_ERR_PARAM; // 引数が不正(I2C送信サイズが不正)
    }
    else {  
        // データサイズをチェック
        dataSize = sizeof(stI2cReq.slaveAddr) + sizeof(stI2cReq.dataSize) + stI2cReq.dataSize; // スレーブアドレス + 送信サイズ領域のサイズ + 送信サイズ
        if (pstReqFrm->dataSize != dataSize) {
            errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
        }         
    }

    stI2cReq.slaveAddr = pstReqFrm->aData[0]; // スレーブアドレス
    if (stI2cReq.slaveAddr > 0x7F) {
        errCode = FRM_ERR_PARAM; // 引数が不正
    }

    if (errCode == FRM_ERR_SUCCESS) { // エラー無しの場合
        // エンキューするI2C送信要求を作成
        stI2cReq.seqNo = pstReqFrm->seqNo; // シーケンス番号 ※後で応答を返すために保存
        stI2cReq.cmd = pstReqFrm->cmd;     // コマンド ※後で応答を返すために保存
        memcpy(stI2cReq.aData, &pstReqFrm->aData[3], stI2cReq.dataSize); // 送信データ
        // I2C送信要求のエンキュー
        if (!CMN_Enqueue(CMN_QUE_KIND_I2C_REQ, &stI2cReq, sizeof(ST_I2C_REQ), false)) {
            errCode = FRM_ERR_BUF_NOT_ENOUGH;
        }    
    }

    if (errCode != FRM_ERR_SUCCESS) { // エラー有りの場合 
        // 応答フレームを送信        
        FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
    }    
}

// I2Cマスタ受信コマンドの実行
static void CMD_ExecReqCmd_RecvI2c(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_I2C_REQ stI2cReq;                // I2C受信要求

    memcpy(&stI2cReq.dataSize, &pstReqFrm->aData[1], sizeof(stI2cReq.dataSize)); // I2C送信サイズ  
    // I2C受信サイズをチェック
    if ((stI2cReq.dataSize < 1) || (stI2cReq.dataSize > I2C_DATA_MAX_SIZE)) {
        errCode = FRM_ERR_PARAM; // 引数が不正(I2C受信サイズが不正)
    }
    else {
        // データサイズをチェック
        dataSize = sizeof(stI2cReq.slaveAddr) + sizeof(stI2cReq.dataSize);  // スレーブアドレス + 受信サイズ領域のサイズ
        if (pstReqFrm->dataSize != dataSize) {
            errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
        }
    }

    stI2cReq.slaveAddr = pstReqFrm->aData[0];  // スレーブアドレス
    if (stI2cReq.slaveAddr > 0x7F) {
        errCode = FRM_ERR_PARAM; // 引数が不正
    }

    if (errCode == FRM_ERR_SUCCESS) { // エラー無しの場合
        // エンキューするI2C受信要求を作成
        stI2cReq.seqNo = pstReqFrm->seqNo; // シーケンス番号 ※後で応答を返すために保存
        stI2cReq.cmd = pstReqFrm->cmd;     // コマンド ※後で応答を返すために保存
        // I2C受信要求のエンキュー
        if (!CMN_Enqueue(CMN_QUE_KIND_I2C_REQ, &stI2cReq, sizeof(ST_I2C_REQ), false)) {
            errCode = FRM_ERR_BUF_NOT_ENOUGH;
        }    
    }

    if (errCode != FRM_ERR_SUCCESS) { // エラー有りの場合 
        // 応答フレームを送信        
        FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
    }
}

// PWM開始コマンドの実行
static void CMD_ExecReqCmd_StartPwm(ST_FRM_REQ_FRAME *pstReqFrm)
{  
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_PWM_CONFIG stPwmConfig;          // PWM設定

    // データサイズをチェック
    dataSize = sizeof(stPwmConfig);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 引数を取得
        memcpy(&stPwmConfig, pstReqFrm->aData, sizeof(stPwmConfig)); // PWM設定
        // PWM開始
        PWM_Start(&stPwmConfig);        

    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);    
}

// PWM停止コマンドの実行
static void CMD_ExecReqCmd_StopPwm(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 

    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系      
        // PWM停止
        PWM_Stop(); 
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);           
}

// FWエラー取得コマンドの実行
static void CMD_ExecReqCmd_GetFwError(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ULONG errorBits = 0;                // FWエラー

    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // FWエラーを取得
        errorBits = CMN_GetFwErrorBits(true);        
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(errorBits), &errorBits); 
}

// FWエラークリアコマンドの実行
static void CMD_ExecReqCmd_ClearFwError(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 

    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // FWエラークリア
        CMN_ClearFwErrorBits(true);
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL); 
}

// FLASH消去コマンドの実行
static void CMD_ExecReqCmd_EraseFlash(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 

    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE;    // データサイズが不正
    }
    else { // 正常系
        // 成功の応答フレームを送信 
        FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);   
        // CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);         
        // FLASH消去
        FLASH_Erase();
    }

    // 失敗の応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);        
}

// ネットワーク設定変更コマンドの実行
static void CMD_ExecReqCmd_SetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_NW_CONFIG stNwConfig;            // ネットワーク設定
    ST_FLASH_DATA stFlashData;          // FLASHデータ

    // データサイズをチェック
    dataSize = sizeof(stNwConfig);
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 引数を取得
        memcpy(&stNwConfig, &pstReqFrm->aData[0], sizeof(stNwConfig)); // ネットワーク設定
        // 成功の応答フレームを送信 
        FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);
        // CPUコア1が応答フレームを送信するのを待つ
        busy_wait_ms(CMD_WAIT_SEND_END);
        // [FLASHへ書き込み]
        // FLASHデータ読み込み
        FLASH_Read(&stFlashData);
        // 書き込みデータを用意
        memcpy(&stFlashData.stNwConfig, &stNwConfig, sizeof(stNwConfig));
        // FLASHへ書き込み  
        FLASH_Write(&stFlashData); // この関数の中でリセットされる 
    }

    // 失敗の応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, 0, NULL);       
}

// ネットワーク設定取得コマンドの実行
static void CMD_ExecReqCmd_GetNwConfig(ST_FRM_REQ_FRAME *pstReqFrm)
{
    USHORT dataSize = 0;                // データサイズの期待値
    USHORT errCode = FRM_ERR_SUCCESS;   // エラーコード 
    ST_FLASH_DATA *pstFlashData = NULL; // 電源起動時のFLASHデータ
    
    // データサイズをチェック
    if (pstReqFrm->dataSize != dataSize) {
        errCode = FRM_ERR_DATA_SIZE; // データサイズが不正
    }
    else { // 正常系
        // 電源起動時のFLASHデータを取得
        pstFlashData = FLASH_GetDataAtPowerOn();
    }

    // 応答フレームを送信        
    FRM_MakeAndSendResFrm(pstReqFrm->seqNo, pstReqFrm->cmd, errCode, sizeof(pstFlashData->stNwConfig), &pstFlashData->stNwConfig);    
}