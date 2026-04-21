# PicoJigLib.dll利用ガイド

## 目次
- [はじめに](#はじめに)
- [基本仕様](#基本仕様)
- [注意事項](#注意事項)
- [1. クラスの初期化と通信方式の選択](#1-クラスの初期化と通信方式の選択)
- [2. 接続・切断メソッド](#2-接続切断メソッド)
- [3. Wi-Fi設定](#3-wi-fi設定)
- [4. FW情報取得](#4-fw情報取得)
- [5. GPIO設定・入出力](#5-gpio設定入出力)
- [6. ADC (アナログ・デジタル変換)](#6-adc-アナログデジタル変換)
- [7. UART通信](#7-uart通信)
- [8. SPI通信](#8-spi通信)
- [9. I2C通信](#9-i2c通信)
- [10. PWM出力](#10-pwm出力)
- [11. フラッシュ操作](#11-フラッシュ操作)

## はじめに
本マニュアルは C#のDLLであるPicoJigLib.dllの JigCmd クラスおよびその派生クラスについて、
基本的な使い方と使用可能なメソッド・プロパティをまとめたものです。  

本ライブラリは、USBの仮想COMまたはWi-FiのTCP/IP通信経由でマイコン(Raspberry Pi Pico/Raspberry Pi Pico W)にコマンドを送信し、そのペリフェラルを制御するためのものです。

## 基本仕様
各コマンド送信メソッド (SendCmd_***) および Connect メソッドの戻り値はすべて `string` 型です。
- 成功時: `null` が返ります。
- 失敗時: エラー内容を示すメッセージ文字列が返ります。

## 注意事項
各種設定メソッド（SendCmd_SetNwConfig、SendCmd_SetGpioConfig、SendCmd_SetUartConfig、SendCmd_SetI2cConfig、SendCmd_SetSpiConfig）を実行するとマイコンのFlashメモリに設定データを書き込み後にマイコンがリセットされるため、5秒待ってから再接続が必要です。  
再接続についてはサンプルプログラムの ReconnectMcu() を参照してください。

## 1. クラスの初期化と通信方式の選択
通信方式に応じて、JigCmd の派生クラスをインスタンス化します。

- **JigSerial クラス**  
  USBの仮想COMを利用する場合に使用します。  
  例: `JigCmd jigCmd = new JigSerial();`

- **JigTcpClient クラス**  
  Wi-FiのTCP/IP通信を利用する場合に使用します。  
  例: `JigCmd jigCmd = new JigTcpClient();`

## 2. 接続・切断メソッド
- `string Connect(Object objParam)`

  回線に接続します。
  - objParam: 接続に必要なパラメータ(ポート名やIPアドレスなど)

- `void Disconnect()`

  回線を切断します。

- `bool IsConnected()`

  回線に接続済みか否かを取得します。（戻り値: true:接続済み, false:未接続）


## 3. Wi-Fi設定
- `string SendCmd_SetNwConfig(string strCountryCode, string strIpAddr, string strSsid, string strPassword)`

  「ネットワーク設定変更」コマンドの要求を送信します。
  - strCountryCode: カントリーコード ※マイコン側へ送信されますが、現在は未使用です。"XX"を指定してください。
  - strIpAddr: IPアドレス(例: "192.168.1.10")
  - strSsid: SSID
    - **指定できるWi-FiルーターのSSIDの条件**
      - 2.4GHz帯を使用するWi-Fi規格「IEEE 802.11b/g/n」に対応していること。間違えて5GHzの周波数帯のSSIDを指定しないようにご注意下さい。
      - 暗号化方式はWPA2であること。  
  - strPassword: パスワード  
  
  ※本メソッドを実行するとマイコンがリセットされるため、【注意事項】を参照してください。

- `string SendCmd_GetNwConfig(out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword)`

  「ネットワーク設定取得」コマンドの要求を送信します。
  - strCountryCode: 取得したカントリーコード
  - strIpAddr: 取得したIPアドレス
  - strSsid: 取得したSSID
  - strPassword: 取得したパスワード

## 4. FW情報取得
- `string SendCmd_GetFwInfo(out string strMakerName, out string strFwName, out string strFwVer, out string strBoardId)`

  「FW情報取得」コマンドの要求を送信します。
  - strMakerName: 取得したメーカー名(16文字)
  - strFwName: 取得したFW名(16文字)
  - strFwVer: 取得したFWバージョン(8文字の16進数文字列)
  - strBoardId: 取得したボードID(16文字の16進数文字列)

- `string SendCmd_GetFwError(ref List<string> lstErrMsg)`

  「FWエラー情報取得」コマンドの要求を送信します。
  - lstErrMsg: 取得したエラーメッセージのリスト

- `string SendCmd_ClearFwError()`

  「FWエラークリア」コマンドの要求を送信します。

## 5. GPIO設定・入出力

### 使用ピン
- 入力用GP番号: GP3(5番), GP4(6番), GP5(7番), GP8(11番), GP9(12番), GP10(14番), GP11(15番)
- 出力用GP番号: GP12(16番), GP13(17番), GP14(19番), GP15(20番), GP20(26番), GP21(27番), GP22(29番)
※括弧内は物理ピン番号です。
### ビットマスク
各メソッドの引数で扱うビットマスク（ビットデータ）は、対象となるGP番号に対応するビット位置（1 << GP番号）を表します。


- `string SendCmd_GetGpioConfig(out UInt32 pullDownBits, out UInt32 initialOutValBits)`

  「GPIO設定取得」コマンドの要求を送信します。
  - pullDownBits: 取得した入力GPIOの内蔵プルアップ/プルダウン設定ビットマスク(各ビット 1:プルダウン, 0:プルアップ)
  - initialOutValBits: 取得した出力GPIOの電源ON時出力値ビットマスク(各ビット 1:High, 0:Low)

- `string SendCmd_SetGpioConfig(UInt32 pullDownBits, UInt32 initialOutValBits)`

  「GPIO設定変更」コマンドの要求を送信します。
  - pullDownBits: 入力GPIOの内蔵プルアップ/プルダウン設定ビットマスク(各ビット 1:プルダウン, 0:プルアップ)
  - initialOutValBits: 出力GPIOの電源ON時出力値ビットマスク(各ビット 1:High, 0:Low)

  ※本メソッドを実行するとマイコンがリセットされるため、【注意事項】を参照してください。

- `string SendCmd_GetGpio(out UInt32 inOutValBits)`

  「GPIO入出力値取得」コマンドの要求を送信します。
  - inOutValBits: 取得したGPIO入出力値ビットマスク(各ビット 1:High, 0:Low)

- `string SendCmd_OutGpio(UInt32 outValBits)`

  「GPIO出力」コマンドの要求を送信します。
  - outValBits: 出力GPIO値のビットマスク(各ビット 1:High, 0:Low)

## 6. ADC (アナログ・デジタル変換)

### 使用ピン
- ADC0: GP26 (31番ピン)
- ADC1: GP27 (32番ピン)
- ADC2: GP28 (34番ピン)
- ADC4: 温度センサ

- `string SendCmd_GetAdc(out float[] aVolt)`

  「ADC入力」コマンドの要求を送信します。
  - aVolt: 取得した各チャンネルの電圧値[V]の配列(要素数4)

## 7. UART通信

### 使用ピン
- UART0 TX: GP0 (1番ピン)
- UART0 RX: GP1 (2番ピン)


- `string SendCmd_GetUartConfig(out UInt32 baudrate, out byte dataBits, out byte stopBits, out byte parity)`

  「UART通信設定取得」コマンドの要求を送信します。
  - baudrate: 取得したボーレート(bps)
  - dataBits: 取得したデータビット長
  - stopBits: 取得したストップビット(1 or 2)
  - parity: 取得したパリティ(0:None, 1:Even, 2:Odd)

- `string SendCmd_SetUartConfig(UInt32 baudrate, byte dataBits, byte stopBits, byte parity)`

  「UART通信設定変更」コマンドの要求を送信します。
  - baudrate: ボーレート(bps)
  - dataBits: データビット長(8固定)
  - stopBits: ストップビット(1 or 2)
  - parity: パリティ(0:None, 1:Even, 2:Odd)
  
  ※本メソッドを実行するとマイコンがリセットされるため、【注意事項】を参照してください。

- `string SendCmd_SendUart(byte[] aReqData)`

  「UART送信」コマンドの要求を送信します。
  - aReqData: 送信データ(1～256byte)

- プロパティ: `BlockingCollection<byte> PrpUartRecvDataQue { get; set; }`

  UART受信データのキューです。

## 8. SPI通信

### 使用ピン
- SPI0 RX: GP16 (21番ピン)
- SPI0 CSn: GP17 (22番ピン) ※ハードウェアCSではなくGPIOを用いたソフトウェア制御です。
- SPI0 SCK: GP18 (24番ピン)
- SPI0 TX: GP19 (25番ピン)

- `string SendCmd_GetSpiConfig(out UInt32 freq, out byte dataBits, out byte polarity, out byte phase, out byte order)`

  「SPI通信設定取得」コマンドの要求を送信します。
  - freq: 取得した周波数(Hz)
  - dataBits: 取得したデータビット長
  - polarity: 取得したCPOL(0 or 1)
  - phase: 取得したCPHA(0 or 1)
  - order: 取得したビットオーダー(1:MSB First)

- `string SendCmd_SetSpiConfig(UInt32 freq, byte dataBits, byte polarity, byte phase, byte order)`

  「SPI通信設定変更」コマンドの要求を送信します。
  - freq: 周波数(Hz)
  - dataBits: データビット長(8固定)
  - polarity: CPOL (0 or 1)
  - phase: CPHA (0 or 1)
  - order: ビットオーダー(1:MSB First固定)
  
  ※本メソッドを実行するとマイコンがリセットされるため、【注意事項】を参照してください。

- `string SendCmd_SendSpi(byte[] aReqData, out byte[] aResData)`

  「SPIマスタ送受信」コマンドの要求を送信します。
  - aReqData: 送信データ(1～256byte)
  - aResData: 受信データ格納先

## 9. I2C通信

### 使用ピン
- I2C1 SDA: GP6 (9番ピン)
- I2C1 SCL: GP7 (10番ピン)

- `string SendCmd_GetI2cConfig(out UInt32 freq)`

  「I2C通信設定取得」コマンドの要求を送信します。
  - freq: 取得した周波数(Hz)

- `string SendCmd_SetI2cConfig(UInt32 freq)`

  「I2C通信設定変更」コマンドの要求を送信します。
  - freq: 周波数(Hz)
  
  ※本メソッドを実行するとマイコンがリセットされるため、【注意事項】を参照してください。

- `string SendCmd_SendI2c(byte slaveAddr, byte[] aReqData)`

  「I2Cマスタ送信」コマンドの要求を送信します。
  - slaveAddr: 7bitスレーブアドレス
  - aReqData: 送信データ(1～256byte)　

- `string SendCmd_RecvI2c(byte slaveAddr, UInt16 recvSize, out byte[] aResData)`

  「I2Cマスタ受信」コマンドの要求を送信します。
  - slaveAddr: 7bitスレーブアドレス
  - recvSize: 受信サイズ
  - aResData: 受信したデータ格納先

## 10. PWM出力

### 使用ピン
- PWM: GP2 (4番ピン)

- `string SendCmd_StartPwm(float clkdiv, UInt16 wrap, UInt16 level)`

  「PWM開始」コマンドの要求を送信します。
  - clkdiv: クロック分周比
  - wrap: ラップ値
  - level: レベル(比較値)

  PWM周波数とデューティー比は、以下の式で計算されます。
  - PWM周波数 = 125MHz ÷ ((ラップ値 + 1) × クロック分周)
  - デューティー比 = Level ÷ (ラップ値 + 1)

- `string SendCmd_StopPwm()`

  「PWM停止」コマンドの要求を送信します。

## 11. フラッシュ操作
- `string SendCmd_EraseFlash()`

  「FLASH消去」コマンドの要求を送信します。


