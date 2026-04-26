# JigLibSample アプリケーション マニュアル

## 目次
- [1. 概要](#1-概要)
- [2. 使用ピン](#2-使用ピン)
- [3. 起動と接続](#3-起動と接続)
- [4. メインメニュー](#4-メインメニュー)
- [5. 各機能の注意事項](#5-各機能の注意事項)

## 1. 概要
本アプリケーションは、`PicoJigLib.dll`を使用してマイコン(Raspberry Pi Pico / Raspberry Pi Pico W)とUSB(COM)またはWi-Fi(TCP/IP)で通信し、ペリフェラル（GPIO、ADC、UART、I2C、SPI、PWM）の制御や情報取得を行うためのコンソールベースのサンプルプログラムです。

## 2. 使用ピン
各機能で使用するマイコンのピン割り当ては以下の通りです。（※括弧内は物理ピン番号です）

- **GPIO (入力用)**: 
  - GP3(5番), GP4(6番), GP5(7番), GP8(11番), GP9(12番), GP10(14番), GP11(15番)
- **GPIO (出力用)**: 
  - GP12(16番), GP13(17番), GP14(19番), GP15(20番), GP20(26番), GP21(27番), GP22(29番)
- **ADC**:
  - ADC0: GP26(31番)
  - ADC1: GP27(32番)
  - ADC2: GP28(34番)
  - ADC4: 温度センサ
- **UART (UART0)**:
  - TX: GP0(1番) 
  - RX: GP1(2番)
- **SPI (SPI0)**:
  - RX: GP16(21番) 
  - CSn: GP17(22番) 
  - SCK: GP18(24番) 
  - TX: GP19(25番)
- **I2C (I2C1)**:
  - SDA: GP6(9番) 
  - SCL: GP7(10番)
- **PWM**: 
  - GP2(4番)

## 3. 起動と接続
アプリケーションを起動すると、コンソール画面に英語のプロンプトが表示されます。以下の手順でマイコンと接続してください。

### 手順1. 通信方式の選択

最初に以下のメッセージが表示されます。

```text
@Please select a communication method.
  1: USB   (COM)
  2: Wi-Fi (TCP/IP)
Select Method > 
```
キーボードで「1」または「2」を入力し、`Enter` キーを押します。

### 手順2. 接続パラメータの入力

- **「1」 (USB) を選択した場合**:  
  利用可能なCOMポートの一覧が表示されます。接続先ポート名（例: `COM3`）を入力して `Enter` キーを押します。
  ```text
  Available COM ports:
    COM1
    COM3
  Please enter the COM port name (e.g., COM1): 
  ```

- **「2」 (Wi-Fi) を選択した場合**:  
  マイコンのIPアドレスを入力して `Enter` キーを押します。
  ```text
  Please enter the IP address (e.g., 192.168.10.100): 
  ```

接続試行が開始され、成功すると以下のメッセージが表示され、メインメニュー画面に移行します。
```text
Connecting to COM3 via USB (COM)...
Connected successfully via USB (COM).
```

## 4. メインメニュー

画面下部に以下のようなプロンプトが表示されます。
```text
@Select Menu
 Enter: Show menu, Number: Execute > 
```
実行したい機能の番号を入力して `Enter` キーを押してください。（何も入力せずに `Enter` キーを押すと、再度メニュー一覧が表示されます）
以下に各機能の実行時に必要なユーザー入力と、表示されるメッセージの概要を記載します。

### 機能一覧
- **1: Get FW Info**  
  FW情報を取得します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- FW Info ---
    Maker: SHIOMACHI_SOFT
    FW Name: PicoJig_WL
    FW Version: 26040100
    Board ID: E66428C51F34B027
    ```

- **2: Get Wi-Fi Config**  
  現在のWi-Fiネットワーク設定を取得します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- Network configuration ---
      Device IP Address:  192.168.10.100
      SSID:               My_SSID
      Password:           (hidden)
    ```

- **3: Set Wi-Fi Config**  
  IPアドレス、SSID、パスワードを設定してマイコンをリセットします。
  
  - **指定できるWi-FiルーターのSSIDの条件**
    - 2.4GHz帯を使用するWi-Fi規格「IEEE 802.11b/g/n」に対応していること。間違えて5GHzの周波数帯のSSIDを指定しないようにご注意下さい。
    - 暗号化方式はWPA2であること。  
  
  - **入力:**   
    - 以下のプロンプトに対して値を入力します。（例: IP Address: `192.168.10.100`, SSID: `My_SSID`, SSID Password: `password`）
    ```text
    --- Set Wi-Fi Config ---
    IP Address: 
    SSID: 
    SSID Password: 
    ```
  - **表示:**   
    完了後、自動的に再接続処理が行われます。  
    > **※補足**:  
    > 現在Wi-Fi(TCP/IP)通信でマイコンと接続している場合、ここで変更した**新しいIPアドレス**を使用して自動的に再接続を試みる仕様になっています。
    ```text
    Wi-Fi configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **4: Get ADC Value**  
  ADCの電圧値と温度センサ値を取得します。
  - **入力:**   
    なし
  - **表示:**   
    各チャンネルの電圧[V]と温度[℃]が表示されます。  
    ```text
    --- ADC Values ---
    CH0: 1.234 V
    CH1: 0.000 V
    CH2: 3.210 V
    CH4 (Temp): 25.50 deg C
    ```

#### 【GPIO】
- **5: Get GPIO Config**  
  現在のGPIO設定を取得します。
  - **入力:**   
    なし
  - **表示:** 
    - 入力GPIOの内蔵プルアップ/プルダウン設定
    - 出力GPIOの電源ON時出力値  
    ```text
    --- GPIO configuration ---
    [Input GPs] Pull-up/Pull-down
    GP3: Pull-down
    GP4: Pull-down
    GP5: Pull-up
    GP8: Pull-up
    GP9: Pull-up
    GP10: Pull-up
    GP11: Pull-up
    [Output GPs] Initial output value
    GP12: High
    GP13: High
    GP14: Low
    GP15: Low
    GP20: Low
    GP21: Low
    GP22: Low
    ```

- **6: Set GPIO Config**  
  GPIO設定を変更し、マイコンをリセットします。
  - **入力:**   
    - 入力GPIOの内蔵プルアップ/プルダウン設定
    - 出力GPIOの電源ON時出力値    
    - プロンプトに従い、各GPピンに対して `0` または `1` を入力します。
    ```text
    --- Set GPIO Config ---
    [Input GPs] Pull-up/Pull-down
    GP3 (0:Pull-up, 1:Pull-down): 
    ...
    [Output GPs] Initial output value
    GP12 (0:Low, 1:High): 
    ...
    ```
  - **表示:**   
    ```text
    GPIO config completed.
    MCU will be reset, waiting for 5 seconds...
    ```

- **7: Get GPIO I/O Value**  
  現在の入力用GPIOおよび出力用GPIOの状態を取得します。
  - **入力:**   
    なし
  - **表示:**   
    入出力状態が表示されます。  
    ```text
    --- Get GPIO I/O Values ---
    [Input GPs] Values
    GP3: High
    GP4: High
    GP5: Low
    GP8: Low
    GP9: Low
    GP10: Low
    GP11: Low
    [Output GPs] Values
    GP12: Low
    GP13: Low
    GP14: Low
    GP15: Low
    GP20: Low
    GP21: Low
    GP22: Low
    ```

- **8: Set GPIO Output**  
  出力用GPIOに指定したデータを出力します。
  - **入力:**   
    - プロンプトに従い、各出力用GPピンに対して `0` または `1` を入力します。
    ```text
    --- Set GPIO Output ---
    [Output GPs] Status
      GP12 (0:Low, 1:High): 
    ...
    ```
  - **表示:**   
    ```text
      GP12: High
      GP13: High
      GP14: Low
      GP15: Low
      GP20: Low
      GP21: Low
      GP22: Low
    ```

#### 【UART】
- **9: Get UART Config**  
  現在のUART通信条件を取得します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- UART configuration ---
      Baudrate: 115200 bps
      DataBits: 8
      StopBits: 1
      Parity:   0 (0:None, 1:Even, 2:Odd)
    ```

- **10: Set UART Config**  
  UART設定を変更し、マイコンをリセットします。
  - **入力:**   
    - それぞれ数値を入力します（例: Baudrate: `115200`, Stop bits: `1`, Parity: `0`）。
    ```text
    --- Set UART Config ---
    Baudrate: 
    Stop bits: 
    Parity (0:None, 1:Even, 2:Odd): 
    ```
  - **表示:**   
    ```text
    UART configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **11: UART Send**  
  UARTでデータを送信します。
  - **入力:**   
    - スペースまたはカンマ区切りで16進数データ（例: `01 02 0A`）を入力します。
    ```text
    --- UART Send ---
    Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): 
    ```
  - **表示:**   
    ```text
    UART sent: 3 bytes
    ```

- **12: UART Receive**  
  キュー内のUART受信データを表示します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- UART Receive ---
    UART received (Hex): 01 02 0A
    ```
    データがない場合は以下のように表示されます。  
    ```text
    --- UART Receive ---
    No UART received data.
    ```
  > **※注意**:  
  > UARTの受信データは非同期で内部キュー（最大4096バイト）に格納されます。  
  > メニューから「12」を選択した時点でのデータを画面に表示します。

#### 【I2C】
- **13: Get I2C Config**  
  現在のI2C通信周波数を取得します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- I2C configuration ---
      Frequency: 400000 Hz
    ```

- **14: Set I2C Config**  
  I2Cの通信周波数(Hz)を設定し、マイコンをリセットします。
  - **入力:**   
    - 数値（例: `400000`）を入力します。
    ```text
    --- Set I2C Config ---
    Frequency(Hz): 
    ```
  - **表示:**   
    ```text
    I2C configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **15: I2C Send**  
  I2Cでデータを送信します。
  - **入力:**   
    - スレーブアドレスと送信する16進数データを入力します（例: Address: `17`, Data: `01 02 03 04`）。
    ```text
    --- I2C Send ---
    7bit Slave Address (Hex): 0x
    Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 03 04): 
    ```
  - **表示:**   
    ```text
    I2C sent.
    ```

- **16: I2C Receive**  
  I2Cでデータを受信します。
  - **入力:**   
    - スレーブアドレスと受信するバイト数を入力します（例: Address: `17`, Read Length: `4`）。
    ```text
    --- I2C Receive ---
    7bit Slave Address (Hex): 0x
    Read Length: 
    ```
  - **表示:**   
    ```text
    I2C received (Hex): 01 02 03 04
    ```

#### 【SPI】
- **17: Get SPI Config**  
  現在のSPI通信条件を取得します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- SPI configuration ---
      Frequency: 1000000 Hz
      DataBits:  8
      CPOL:      0
      CPHA:      0
      Order:     1 (1:MSB First)
    ```

- **18: Set SPI Config**  
  通信周波数、極性、位相を設定し、マイコンをリセットします。
  - **入力:**   
    - それぞれ数値を入力します（例: Frequency: `1000000`, Polarity: `0`, Phase: `0`）。
    ```text
    --- Set SPI Config ---
    Frequency(Hz): 
    Polarity (0:CPOL=0, 1:CPOL=1): 
    Phase (0:CPHA=0, 1:CPHA=1): 
    ```
  - **表示:**   
    ```text
    SPI configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **19: SPI Comm (Send/Receive)**  
  SPIでデータを送受信します。
  - **入力:**   
    - スペースまたはカンマ区切りで送信する16進数データを入力します（例: `01 02 03 04`）。
    ```text
    --- SPI Comm ---
    Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 03 04): 
    ```
  - **表示:**   
    ```text
    SPI received data (Hex): 01 02 03 04
    ```

#### 【PWM】
- **20: Start PWM**  
  PWM信号の出力を開始します。
  - **入力:**   
    - 各プロンプトに対して数値を入力します。
      - **計算式:**  
        - `PWM周波数 = 125MHz / ((Wrap + 1) * Clock divider)`
        - `デューティー比 = Compare value(Level) / (Wrap + 1)`
      - （例: PWM周波数100Hz・デューティー比50%にする場合 → Clock divider: `250`, Wrap: `4999`, Compare value: `2500` を入力）
    ```text
    --- Start PWM ---
    Clock divider: 
    Wrap: 
    Compare value(Level): 
    ```
  - **表示:**   
    ```text
    PWM started.
    ```

- **21: Stop PWM**  
  PWM信号の出力を停止します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- Stop PWM ---
    PWM stopped.
    ```

#### 【その他】
- **22: Get FW Error**  
  FWのエラー情報を取得します。
  - **入力:**   
    なし
  - **表示:**   
    エラーがない場合は以下のように表示されます。  
    ```text
    --- Get FW Error ---
    No FW errors.
    ```
    エラーがある場合はリストが表示されます。  
    ```text
    --- FW Errors ---
     - (Example Error Message 1)
     - (Example Error Message 2)
    ```

- **23: Clear FW Error**  
  FWのエラー情報をクリアします。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- Clear FW Error ---
    FW errors cleared.
    ```

- **24: Erase Flash**  
  マイコンのフラッシュメモリを消去します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    --- Erase Flash ---
    Flash erased successfully.
    MCU will be reset, waiting for 5 seconds...
    ```

- **0: Disconnect**  
  通信を切断し、アプリケーションを終了します。
  - **入力:**   
    なし
  - **表示:**   
    ```text
    Disconnected.
    ```

## 5. 各機能の注意事項
- **設定系コマンドおよびFlash消去 (3, 6, 10, 14, 18, 24) について**  
  設定内容の保存やFlashメモリの消去に伴い、マイコンが自動的にリセットされます。  
  約5秒の待機時間の後に自動再接続が行われます。

- **16進数 (Hex) の入力について**  
  通信データの入力時など、「0x」は入力不要です。画面上の「0x」に続けて入力するか、データをスペースやカンマ区切りで入力してください。  
  例: `01 02 0A` （データ送信の場合）

- **固定されているパラメータについて**  
  一部の通信設定（UARTのデータビット、SPIのデータビットおよびビットオーダーなど）は、ライブラリ仕様に基づきソースコード内で固定値として定義されています。
