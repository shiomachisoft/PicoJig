# sys module: Provides system-specific functions such as program termination (sys.exit) / sysモジュール: プログラムの終了(sys.exit)などシステム固有の機能を提供
import sys
# time module: Provides time-related functions such as sleep (time.sleep) / timeモジュール: スリープ処理(time.sleep)など時間に関連する機能を提供
import time
# re module: Provides string search and split functions using regular expressions / reモジュール: 正規表現を用いた文字列の検索・分割機能を提供
import re

# Note: Installation of pythonnet and pyserial is required to run / 注意: 実行には pythonnet と pyserial のインストールが必要です
# pip install pythonnet pyserial
try:
    # pythonnet module: Used to call .NET libraries (DLLs) from Python / pythonnetモジュール: .NETのライブラリ(DLL)をPythonから呼び出すために使用
    import clr
    # Used to get a list of serial ports / シリアルポート一覧取得のために使用
    import serial.tools.list_ports
except ImportError:
    # If modules are not found, output an installation prompt message and terminate the program abnormally (1) / モジュールが見つからない場合は、インストールを促すメッセージを出力してプログラムを異常終了(1)させる
    print("必要なパッケージがインストールされていません。")
    print("以下のコマンドを実行してインストールしてください:")
    print("pip install pythonnet pyserial")
    sys.exit(1)

try:
    # Add reference to PicoJigLib.dll (Assuming it is in the current directory or environment variable path) / PicoJigLib.dll を参照に追加（カレントディレクトリ、または環境変数のパスにある想定）
    clr.AddReference("PicoJigLib")
    # Import classes for TCP client and serial communication from the JigLib namespace in PicoJigLib.dll / PicoJigLib.dll内のJigLib名前空間から、TCPクライアントとシリアル通信用のクラスをインポート
    from JigLib import JigTcpClient, JigSerial
    # Import .NET List class (for use in C# method arguments) / .NETのListクラスをインポート（C#のメソッド引数で使用するため）
    from System.Collections.Generic import List
    # Import .NET Array, Byte, and String classes (to handle byte arrays and string lists) / .NETのArray、Byte、Stringクラスをインポート（バイト配列や文字列リストを扱うため）
    from System import Array, Byte, String
except Exception as e:
    print(f"PicoJigLib.dll の読み込みに失敗しました: {e}")
    print("実行ディレクトリに PicoJigLib.dll が存在するか確認してください。")
    sys.exit(1)

# Wait time for MCU restart (reset) completion (milliseconds) / マイコンの再起動（リセット）完了を待つ時間 (ミリ秒)
RESET_WAIT_TIME_MS = 5000
# List of configurable input GPIO pin numbers / 設定可能な入力GPIOピンの番号リスト
VALID_GP_INPUT_NUMBERS = [3, 4, 5, 8, 9, 10, 11]
# List of configurable output GPIO pin numbers / 設定可能な出力GPIOピンの番号リスト
VALID_GP_OUTPUT_NUMBERS = [12, 13, 14, 15, 20, 21, 22]

# Helper function to reconnect to the MCU / マイコンへ再接続を行うヘルパー関数
# Called because the MCU is reset after sending configuration change commands (Wi-Fi, GPIO, UART, I2C, SPI, etc.) / 設定変更コマンド（Wi-Fi, GPIO, UART, I2C, SPI等）の送信後、マイコンがリセットされるため呼び出す
def reconnect_mcu(jig_cmd, connect_param):
    print(f"Reconnecting to {connect_param}...")
    # Disconnect once / 一度切断する
    jig_cmd.Disconnect()
    # Attempt to reconnect / 再度接続を試みる
    err = jig_cmd.Connect(connect_param)
    if err:
        # If err contains a string, it is a connection error / err に文字列が入っている場合は接続エラー
        print(f"Reconnection error: {err}")
        return False
    else:
        print("Reconnected successfully.")
        return True

# Main entry point of the application
# アプリケーションのメインエントリポイント
def main():
    # Infinite loop repeating the entire flow of disconnect and reconnect until the application exits (input 0) / アプリケーションが終了(0を入力)するまで、切断と再接続の全体の流れを繰り返す無限ループ
    while True:
        # Select communication method and create instance / 通信方式の選択とインスタンスの生成
        print("@Please select a communication method.")
        print("  1: USB   (COM)")
        print("  2: Wi-Fi (TCP/IP)")
        
        # Select communication method / 通信方式の選択
        # Keep prompting for input until a valid input ("1" or "2") is obtained / 正しい入力 ("1" または "2") が得られるまで入力を促し続ける
        while True:
            comm_type = input("Select Method > ")
            if comm_type in ("1", "2"):
                # Break the loop if the input is valid / 正しい入力であればループを抜ける
                break
            print("Invalid selection. Please select 1 or 2.")
        
        if comm_type == "2":
            # Create instance of JigTcpClient for TCP/IP communication / TCP/IP通信の場合、JigTcpClientのインスタンスを作成
            jig_cmd = JigTcpClient()
            connect_param = input("Please enter the IP address (e.g., 192.168.10.100): ")
        else:
            # Create instance of JigSerial for USB (serial) communication / USB(シリアル)通信の場合、JigSerialのインスタンスを作成
            jig_cmd = JigSerial()
            
            # Get available COM ports and remove duplicates / 利用可能なCOMポートを取得し、重複を排除する
            ports = list(set([port.device for port in serial.tools.list_ports.comports()]))
            
            # Extract only numbers from port names and sort as numbers / ポート名から数字のみを抽出して数値としてソート
            def extract_num(p):
                digits = re.sub(r'\D', '', p)
                return int(digits) if digits else 0
            ports.sort(key=extract_num)
            
            if len(ports) > 0:
                print("Available COM ports:")
                for port in ports:
                    print(f"  {port}")
            else:
                print("No COM ports available.")
                
            connect_param = input("Please enter the COM port name (e.g., COM1): ")

        # Retry if connection parameter is empty / 接続パラメータが空の場合はやり直し
        # strip() removes leading and trailing spaces and line breaks. Treat as an error if nothing is entered / strip() は文字列前後の空白や改行を削除する。何も入力されていない場合はエラーとする
        if not connect_param.strip():
            print("Connection parameter cannot be empty.\n")
            # Call resource release (Dispose) method if it exists on the already created instance / もし既に生成されたインスタンスにリソース解放(Dispose)メソッドがあれば呼び出す
            if hasattr(jig_cmd, 'Dispose'):
                jig_cmd.Dispose()
            continue

        comm_method = "USB (COM)" if comm_type == "1" else "Wi-Fi (TCP/IP)"
        print(f"Connecting to {connect_param} via {comm_method}...")

        # Connect / 接続
        # Execute connection process to MCU / マイコンへ接続処理を実行
        # Connect method returns None or empty string on success, and error message (string) on failure / Connectメソッドは成功時に None または空文字を返し、失敗時にエラーメッセージ(文字列)を返す
        err_msg = jig_cmd.Connect(connect_param)
        if err_msg:
            # On connection failure / 接続失敗時
            print(f"Connection error: {err_msg}\n")
            jig_cmd.Disconnect()
            if hasattr(jig_cmd, 'Dispose'):
                jig_cmd.Dispose()
            continue

        print(f"Connected successfully via {comm_method}.")

        # loop: Flag to determine whether to continue menu processing (send/receive) / loop: メニュー処理(送受信)を続けるかどうかのフラグ
        loop = True
        # Menu display flag. Set not to redisplay menu after command execution / メニュー表示フラグ。コマンド実行後はメニューを再表示しない設定
        show_menu = True

        # Command execution loop while connection is maintained / 接続が維持されている間のコマンド実行ループ
        while loop:
            try:
                # Display menu list / メニュー一覧の表示
                if show_menu:
                    print("\n--------------------------------------------------")
                    print("Please select a command to execute.")
                    print("  1: Get FW Info")
                    print("  2: Get Wi-Fi Config")
                    print("  3: Set Wi-Fi Config")
                    print("  4: Get ADC Value")
                    print("  5: Get GPIO Config")
                    print("  6: Set GPIO Config")
                    print("  7: Get GPIO I/O Value")
                    print("  8: Set GPIO Output")
                    print("  9: Get UART Config")
                    print(" 10: Set UART Config")
                    print(" 11: UART Send")
                    print(" 12: UART Receive")
                    print(" 13: Get I2C Config")
                    print(" 14: Set I2C Config")
                    print(" 15: I2C Send")
                    print(" 16: I2C Receive")
                    print(" 17: Get SPI Config")
                    print(" 18: Set SPI Config")
                    print(" 19: SPI Comm (Send/Receive)")
                    print(" 20: Start PWM")
                    print(" 21: Stop PWM")
                    print(" 22: Get FW Error")
                    print(" 23: Clear FW Error")
                    print(" 24: Erase Flash")
                    print("  0: Disconnect")
                    show_menu = False

                print("--------------------------------------------------")
                print(f"[Current Connection: {comm_method} - {connect_param}]")
                print("@Select Menu")
                choice = input("  Enter: Show menu, Number: Execute > ")
                print()

                # Redisplay menu on empty input (Enter only) / 空打ち(Enterのみ)の場合はメニューを再表示
                if not choice:
                    show_menu = True
                    continue

                if choice == "1":
                    # Get FW info / FW情報の取得
                    # In Python.NET, out arguments are received as elements of the return value tuple / Python.NETではout引数は戻り値のタプルの要素として受け取ります
                    # If original C# method returns a value directly, the first element (index 0) of the return tuple will be that value, / 元のC#メソッドが値を直接返す場合、戻り値のタプルの最初の要素 (index 0) がその値になり、
                    # and the following elements (index 1 and after) will store the out argument results. / 続く要素 (index 1以降) に out 引数の結果が格納されます。
                    # C# signature: string SendCmd_GetFwInfo(out string maker, out string fw_name, out string fw_ver, out string board_id) / C#のシグネチャ: string SendCmd_GetFwInfo(out string maker, out string fw_name, out string fw_ver, out string board_id)
                    # Return value in Python: (err_msg, maker, fw_name, fw_ver, board_id) / Pythonでの戻り値: (err_msg, maker, fw_name, fw_ver, board_id)
                    res = jig_cmd.SendCmd_GetFwInfo()
                    err_msg = res[0]
                    if not err_msg:
                        print("--- FW Info ---")
                        print(f"Maker: {res[1]}")
                        print(f"FW Name: {res[2]}")
                        print(f"FW Version: {res[3]}")
                        print(f"Board ID: {res[4]}")
                    else:
                        print(f"FW info get error: {err_msg}")

                elif choice == "2":
                    # Get network config / ネットワーク設定の取得
                    # Get Wi-Fi configuration / Wi-Fi設定の取得
                    res = jig_cmd.SendCmd_GetNwConfig()
                    err_msg = res[0]
                    if not err_msg:
                        print("--- Network configuration ---")
                        # Remove NULL character at the end of the string and display / 文字列終端のNULL文字を削除して表示
                        print(f"Device IP Address:  {res[2].rstrip(chr(0))}")
                        print(f"SSID:               {res[3].rstrip(chr(0))}")
                        print("Password:           (hidden)")
                    else:
                        print(f"[Error] {err_msg}")

                elif choice == "3":
                    # Set Wi-Fi config / Wi-Fi設定
                    # Write Wi-Fi configuration / Wi-Fi設定の書き込み
                    print("--- Set Wi-Fi Config ---")
                    # The country code is sent to the MCU, but it is currently unused. Please specify "XX".
                    # カントリーコードはマイコン側へ送信されますが、現在は未使用です。"XX"を指定してください。
                    country_code = "XX" # Country code is fixed / 国コードは固定
                    ip_addr = input("IP Address: ")
                    ssid = input("SSID: ")
                    ssid_password = input("SSID Password: ")

                    # If any input value is empty (including only spaces), return to menu without setting / 入力値のどれかが空(空白のみ含む)の場合は設定を行わずにメニューに戻る
                    if not ip_addr.strip() or not ssid.strip() or not ssid_password.strip():
                        print("Error: IP Address, SSID, and Password cannot be empty.")
                        continue

                    err_msg = jig_cmd.SendCmd_SetNwConfig(country_code, ip_addr, ssid, ssid_password)
                    if not err_msg:
                        print("Wi-Fi configured.")
                        print(f"MCU will be reset, waiting for {RESET_WAIT_TIME_MS // 1000} seconds...")
                        # Wait as MCU restarts after writing settings / 設定書き込み後、マイコンが再起動するため待機
                        time.sleep(RESET_WAIT_TIME_MS / 1000.0)
                        # In the case of TCP/IP connection, reconnect with the new IP address after change
                        # TCP/IP接続の場合は変更後の新しいIPアドレスで再接続する
                        # Update connection parameter if TCP connection and IP address changed / TCP接続かつIPアドレスが変更された場合は、接続パラメータを更新
                        if comm_type == "2" and ip_addr.strip():
                            connect_param = ip_addr
                        # Reconnect / 再接続
                        if not reconnect_mcu(jig_cmd, connect_param):
                            loop = False
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "4":
                    # Get ADC value / ADC値の取得
                    # Get ADC (Analog-to-Digital Converter) values / ADC（アナログ・デジタル変換）値の取得
                    res = jig_cmd.SendCmd_GetAdc()
                    err_msg = res[0]
                    if not err_msg:
                        print("--- ADC Values ---")
                        a_volt = res[1] # Voltage value of each channel (float array) / 各チャンネルの電圧値(float配列)
                        for i, v in enumerate(a_volt):
                            if i == 3:
                                # CH4 (index 3) is internal temperature sensor temperature (°C) / CH4(インデックス3)は内部温度センサーの温度(℃)
                                print(f"CH4 (Temp): {v:.2f} deg C")
                            else:
                                print(f"CH{i}: {v:.3f} V")
                    else:
                        print(f"ADC get error: {err_msg}")

                elif choice == "5":
                    # Get GPIO config / GPIO設定の取得
                    # Get GPIO configuration (pull-up/down, initial output value) / GPIO設定（プルアップ/ダウン、初期出力値）の取得
                    res = jig_cmd.SendCmd_GetGpioConfig()
                    err_msg = res[0]
                    if not err_msg:
                        pull_down_bits = res[1] # Each bit indicates pull-down setting for each pin (0: Pull-up, 1: Pull-down) / 各ビットが各ピンのプルダウン設定を示す(0: プルアップ, 1: プルダウン)
                        initial_out_val_bits = res[2] # Each bit indicates initial output value for each pin (0: Low, 1: High) / 各ビットが各ピンの初期出力値を示す(0: Low, 1: High)
                        print("--- GPIO configuration ---")
                        print("[Input GPs] Pull-up/Pull-down")
                        for gp in VALID_GP_INPUT_NUMBERS:
                            # Determine if the bit for the target GPIO pin is set / 対象のGPIOピンのビットが立っているか判定
                            state = "Pull-down" if (pull_down_bits & (1 << gp)) != 0 else "Pull-up"
                            print(f"GP{gp}: {state}")
                        print("[Output GPs] Initial output value")
                        for gp in VALID_GP_OUTPUT_NUMBERS:
                            state = "High" if (initial_out_val_bits & (1 << gp)) != 0 else "Low"
                            print(f"GP{gp}: {state}")
                    else:
                        print(f"[Error] {err_msg}")

                elif choice == "6":
                    # Set GPIO config / GPIO設定
                    # Write GPIO configuration / GPIO設定の書き込み
                    print("--- Set GPIO Config ---")
                    pull_down_bits = 0
                    print("[Input GPs] Pull-up/Pull-down")
                    for gp in VALID_GP_INPUT_NUMBERS:
                        val = input(f"GP{gp} (0:Pull-up, 1:Pull-down): ")
                        if val == "1":
                            # Set the target bit if the input value is 1 / 入力値が1なら対象のビットを立てる
                            pull_down_bits |= (1 << gp)

                    initial_out_val_bits = 0
                    print("[Output GPs] Initial output value")
                    for gp in VALID_GP_OUTPUT_NUMBERS:
                        val = input(f"GP{gp} (0:Low, 1:High): ")
                        if val == "1":
                            initial_out_val_bits |= (1 << gp)

                    err_msg = jig_cmd.SendCmd_SetGpioConfig(pull_down_bits, initial_out_val_bits)
                    if not err_msg:
                        print("GPIO config completed.")
                        print(f"MCU will be reset, waiting for {RESET_WAIT_TIME_MS // 1000} seconds...")
                        time.sleep(RESET_WAIT_TIME_MS / 1000.0)
                        if not reconnect_mcu(jig_cmd, connect_param):
                            loop = False
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "7":
                    # Get GPIO I/O value / GPIO入出力値取得
                    # Get current I/O state (High/Low) of GPIO / GPIOの現在の入出力状態（High/Low）を取得
                    res = jig_cmd.SendCmd_GetGpio()
                    err_msg = res[0]
                    if not err_msg:
                        in_out_bits = res[1]
                        print("--- Get GPIO I/O Values ---")
                        print("[Input GPs] Values")
                        for gp in VALID_GP_INPUT_NUMBERS:
                            state = "High" if (in_out_bits & (1 << gp)) != 0 else "Low"
                            print(f"GP{gp}: {state}")
                        print("[Output GPs] Values")
                        for gp in VALID_GP_OUTPUT_NUMBERS:
                            state = "High" if (in_out_bits & (1 << gp)) != 0 else "Low"
                            print(f"GP{gp}: {state}")
                    else:
                        print(f"Get GPIO I/O Values error: {err_msg}")

                elif choice == "8":
                    # GPIO output / GPIO出力
                    # Set GPIO output state (High/Low) / GPIOの出力状態（High/Low）を設定
                    print("--- Set GPIO Output ---")
                    out_bits = 0
                    print("[Output GPs] Status")
                    for gp in VALID_GP_OUTPUT_NUMBERS:
                        val = input(f"  GP{gp} (0:Low, 1:High): ")
                        if val == "1":
                            out_bits |= (1 << gp)

                    err_msg = jig_cmd.SendCmd_OutGpio(out_bits)
                    if not err_msg:
                        for gp in VALID_GP_OUTPUT_NUMBERS:
                            state = "High" if (out_bits & (1 << gp)) != 0 else "Low"
                            print(f"  GP{gp}: {state}")
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "9":
                    # Get UART config / UART設定の取得
                    # Get UART communication configuration / UART通信設定の取得
                    res = jig_cmd.SendCmd_GetUartConfig()
                    err_msg = res[0]
                    if not err_msg:
                        print("--- UART configuration ---")
                        print(f"  Baudrate: {res[1]} bps")
                        print(f"  DataBits: {res[2]}")
                        print(f"  StopBits: {res[3]}")
                        print(f"  Parity:   {res[4]} (0:None, 1:Even, 2:Odd)")
                    else:
                        print(f"[Error] {err_msg}")

                elif choice == "10":
                    # Set UART config / UART設定
                    # Write UART communication configuration / UART通信設定の書き込み
                    print("--- Set UART Config ---")
                    baudrate = int(input("Baudrate: "))
                    # Fixed to 8 according to the library usage guide / ライブラリ利用ガイドにより8固定
                    data_bits = 8 # Data length is fixed at 8 bits / データ長は8ビット固定
                    stop_bits = int(input("Stop bits: "))
                    parity = int(input("Parity (0:None, 1:Even, 2:Odd): "))

                    err_msg = jig_cmd.SendCmd_SetUartConfig(baudrate, data_bits, stop_bits, parity)
                    if not err_msg:
                        print("UART configured.")
                        print(f"MCU will be reset, waiting for {RESET_WAIT_TIME_MS // 1000} seconds...")
                        time.sleep(RESET_WAIT_TIME_MS / 1000.0)
                        if not reconnect_mcu(jig_cmd, connect_param):
                            loop = False
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "11":
                    # UART send / UART送信
                    # UART data transmission / UARTデータ送信
                    print("--- UART Send ---")
                    data_str = input("Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): ")
                    # Split string separated by space or comma using regular expressions / 正規表現でスペースやカンマ区切りの文字列を分割
                    # Example: "01, 02 0A" -> ["01", "02", "0A"] / 例: "01, 02 0A" -> ["01", "02", "0A"]
                    hex_strs = re.split(r'[\s,]+', data_str.strip())
                    try:
                        # Convert split hex string to number, then to .NET Byte array / 分割した16進数文字列を数値に変換し、.NETのByte配列に変換
                        # Convert hex string to integer value with int(s, 16) / int(s, 16) で 16進数の文字列を整数の値へ変換する
                        tx_data = Array[Byte]([int(s, 16) for s in hex_strs if s])
                        # Check if data size fits within limit to send to MCU (1 to 256 bytes) / マイコンへ送信できるデータサイズ(1〜256バイト)に収まっているか確認
                        if len(tx_data) == 0 or len(tx_data) > 256:
                            print("Error: Send data must be between 1 and 256 bytes.")
                            continue

                        err_msg = jig_cmd.SendCmd_SendUart(tx_data)
                        if not err_msg:
                            print(f"UART sent: {len(tx_data)} bytes")
                        else:
                            print(f"Error: {err_msg}")
                    except ValueError:
                        # Exception handling when unconvertible string (e.g., "GG") is included in int(s, 16) / int(s, 16) で変換できない文字列(例: "GG")が含まれていた場合の例外処理
                        print("Error: Invalid Hex data format.")

                elif choice == "12":
                    # UART receive / UART受信
                    # Read UART received data (get data accumulated in the library queue) / UART受信データの読み出し（ライブラリ内のキューに蓄積されたデータを取得）
                    print("--- UART Receive ---")
                    # According to JigCmd specifications, UART received data is stored asynchronously in PrpUartRecvDataQue
                    # JigCmdの仕様では、UART受信データは PrpUartRecvDataQue に非同期で格納されます
                    if jig_cmd.PrpUartRecvDataQue.Count > 0:
                        rx_list = []
                        # Keep dequeuing until the queue is empty / キューの中身が空になるまで取り出し続ける
                        while True:
                            # Equivalent process to C#'s TryTake(out byte b) / C#の TryTake(out byte b) と同等の処理
                            # The queue for UART received data is up to 4096 bytes / UART受信データのキューは最大4096byte
                            # In pythonnet, out arguments return as tuple: (success, result) / pythonnetではout引数はタプルで返る: (success, result)
                            # If success is True, it means data was retrieved / success が True であればデータが取り出せたことを意味する
                            res = jig_cmd.PrpUartRecvDataQue.TryTake()
                            if isinstance(res, tuple) and res[0]:
                                rx_list.append(res[1])
                            else:
                                break
                        # Convert received byte list to hex string and display / 受信したバイトリストを16進数文字列に変換して表示
                        # Format each byte (b) to 2-digit hex uppercase (02X) and join / 各バイト(b)を2桁の16進数大文字(02X)にフォーマットして結合する
                        hex_str = " ".join([f"{b:02X}" for b in rx_list])
                        print(f"UART received (Hex): {hex_str}")
                    else:
                        print("No UART received data.")

                elif choice == "13":
                    # Get I2C config / I2C設定の取得
                    # Get I2C communication configuration / I2C通信設定の取得
                    res = jig_cmd.SendCmd_GetI2cConfig()
                    err_msg = res[0]
                    if not err_msg:
                        print("--- I2C configuration ---")
                        print(f"  Frequency: {res[1]} Hz")
                    else:
                        print(f"[Error] {err_msg}")

                elif choice == "14":
                    # Set I2C config / I2C設定
                    # Write I2C communication configuration / I2C通信設定の書き込み
                    print("--- Set I2C Config ---")
                    freq = int(input("Frequency(Hz): "))

                    err_msg = jig_cmd.SendCmd_SetI2cConfig(freq)
                    if not err_msg:
                        print("I2C configured.")
                        print(f"MCU will be reset, waiting for {RESET_WAIT_TIME_MS // 1000} seconds...")
                        time.sleep(RESET_WAIT_TIME_MS / 1000.0)
                        if not reconnect_mcu(jig_cmd, connect_param):
                            loop = False
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "15":
                    # I2C send / I2C送信
                    # I2C data transmission / I2Cデータ送信
                    print("--- I2C Send ---")
                    dev_addr_str = input("7bit Slave Address (Hex): 0x")
                    # Interpret hex string like "0A" as numeric value / "0A" などの16進数文字列を数値として解釈する
                    dev_addr = int(dev_addr_str, 16)
                    data_str = input("Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): ")
                    hex_strs = re.split(r'[\s,]+', data_str.strip())
                    try:
                        # Convert from string to byte array / 文字列からバイト配列へ変換
                        tx_data = Array[Byte]([int(s, 16) for s in hex_strs if s])
                        if len(tx_data) == 0 or len(tx_data) > 256:
                            print("Error: Send data must be between 1 and 256 bytes.")
                            continue

                        err_msg = jig_cmd.SendCmd_SendI2c(dev_addr, tx_data)
                        if not err_msg:
                            print("I2C sent.")
                        else:
                            print(f"Error: {err_msg}")
                    except ValueError:
                        print("Error: Invalid Hex data format.")

                elif choice == "16":
                    # I2C receive / I2C受信
                    # I2C data reception / I2Cデータ受信
                    print("--- I2C Receive ---")
                    dev_addr_str = input("7bit Slave Address (Hex): 0x")
                    # Interpret I2C 7bit slave address in hex / I2Cの7bitスレーブアドレスを16進数で解釈
                    dev_addr = int(dev_addr_str, 16)
                    read_len = int(input("Read Length: "))

                    res = jig_cmd.SendCmd_RecvI2c(dev_addr, read_len)
                    err_msg = res[0]
                    if not err_msg:
                        rx_data = res[1] # Received byte array / 受信したバイト配列
                        hex_str = " ".join([f"{b:02X}" for b in rx_data])
                        print(f"I2C received (Hex): {hex_str}")
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "17":
                    # Get SPI config / SPI設定の取得
                    # Get SPI communication configuration / SPI通信設定の取得
                    res = jig_cmd.SendCmd_GetSpiConfig()
                    err_msg = res[0]
                    if not err_msg:
                        print("--- SPI configuration ---")
                        print(f"  Frequency: {res[1]} Hz")
                        print(f"  DataBits:  {res[2]}")
                        print(f"  CPOL:      {res[3]}")
                        print(f"  CPHA:      {res[4]}")
                        print(f"  Order:     {res[5]} (1:MSB First)")
                    else:
                        print(f"[Error] {err_msg}")

                elif choice == "18":
                    # Set SPI config / SPI設定
                    # Write SPI communication configuration / SPI通信設定の書き込み
                    print("--- Set SPI Config ---")
                    freq = int(input("Frequency(Hz): "))
                    # Fixed to 8 according to the library usage guide / ライブラリ利用ガイドにより8固定
                    data_bits = 8 # Data length is fixed at 8 bits / データ長は8ビット固定
                    polarity = int(input("Polarity (0:CPOL=0, 1:CPOL=1): "))
                    phase = int(input("Phase (0:CPHA=0, 1:CPHA=1): "))
                    # Fixed to 1 (MSB First) according to the library usage guide / ライブラリ利用ガイドにより1 (MSB First)固定
                    order = 1 # Data order is fixed to MSB First / データ順序はMSB Firstで固定

                    err_msg = jig_cmd.SendCmd_SetSpiConfig(freq, data_bits, polarity, phase, order)
                    if not err_msg:
                        print("SPI configured.")
                        print(f"MCU will be reset, waiting for {RESET_WAIT_TIME_MS // 1000} seconds...")
                        time.sleep(RESET_WAIT_TIME_MS / 1000.0)
                        if not reconnect_mcu(jig_cmd, connect_param):
                            loop = False
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "19":
                    # SPI comm (send/receive) / SPI通信 (送受信)
                    # SPI communication (full-duplex, so reception occurs simultaneously with transmission) / SPI通信（全二重通信のため、送信と同時に受信を行う）
                    print("--- SPI Comm ---")
                    data_str = input("Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): ")
                    hex_strs = re.split(r'[\s,]+', data_str.strip())
                    try:
                        # Bulk convert list of hex strings to .NET byte array using list comprehension / リスト内包表記を使用して、16進数文字列のリストを .NET のバイト配列へ一括変換
                        tx_data = Array[Byte]([int(s, 16) for s in hex_strs if s])
                        if len(tx_data) == 0 or len(tx_data) > 256:
                            print("Error: Send data must be between 1 and 256 bytes.")
                            continue

                        res = jig_cmd.SendCmd_SendSpi(tx_data)
                        err_msg = res[0]
                        if not err_msg:
                            rx_data = res[1] # Returns data received equivalent to the send size / 送信サイズ分だけ受信したデータが返る
                            hex_str = " ".join([f"{b:02X}" for b in rx_data])
                            print(f"SPI received data (Hex): {hex_str}")
                        else:
                            print(f"Error: {err_msg}")
                    except ValueError:
                        print("Error: Invalid Hex data format.")

                elif choice == "20":
                    # Start PWM / PWM開始
                    # Start PWM (Pulse Width Modulation) output / PWM（パルス幅変調）出力の開始
                    print("--- Start PWM ---")
                    div = float(input("Clock divider: "))
                    wrap = int(input("Wrap: "))
                    level = int(input("Compare value(Level): "))

                    err_msg = jig_cmd.SendCmd_StartPwm(div, wrap, level)
                    if not err_msg:
                        print("PWM started.")
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "21":
                    # Stop PWM / PWM停止
                    # Stop PWM output / PWM出力の停止
                    print("--- Stop PWM ---")
                    err_msg = jig_cmd.SendCmd_StopPwm()
                    if not err_msg:
                        print("PWM stopped.")
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "22":
                    # Get FW error / FWエラー取得
                    # Get errors occurred inside the MCU (FW error) / マイコン内部で発生したエラー（FWエラー）の取得
                    # When passing objects by ref or out in Python.NET, / Python.NETで `ref` または `out` でオブジェクトを渡す場合、
                    # they are either included in the return tuple or the original instance is updated. / 戻り値のタプルに含まれるか、元のインスタンスが更新される。
                    err_list = List[String]()
                    res = jig_cmd.SendCmd_GetFwError(err_list)
                    
                    # Depending on C# method return signature, determine if tuple or single value and process / C#側メソッドの戻り値シグネチャに依存するため、タプルか単一値かを判定して処理
                    err_msg = res[0] if isinstance(res, tuple) else res
                    ret_list = res[1] if isinstance(res, tuple) else err_list

                    if not err_msg:
                        if ret_list.Count == 0:
                            print("No FW errors.")
                        else:
                            print("--- FW Errors ---")
                            for err in ret_list:
                                print(f" - {err}")
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "23":
                    # Clear FW error / FWエラークリア
                    # Clear FW error history / FWエラー履歴の消去
                    print("--- Clear FW Error ---")
                    err_msg = jig_cmd.SendCmd_ClearFwError()
                    if not err_msg:
                        print("FW errors cleared.")
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "24":
                    # Erase flash / フラッシュ消去
                    # Initialize MCU Flash memory (settings storage area, etc.) / マイコンのFlashメモリ（設定保存領域など）の初期化
                    print("--- Erase Flash ---")
                    err_msg = jig_cmd.SendCmd_EraseFlash()
                    if not err_msg:
                        print("Flash erased successfully.")
                        print(f"MCU will be reset, waiting for {RESET_WAIT_TIME_MS // 1000} seconds...")
                        time.sleep(RESET_WAIT_TIME_MS / 1000.0)
                        if not reconnect_mcu(jig_cmd, connect_param):
                            loop = False
                    else:
                        print(f"Error: {err_msg}")

                elif choice == "0":
                    # Break loop to exit program / プログラム終了のためのループ離脱
                    loop = False

                else:
                    print("Invalid selection. Please enter a menu number.")

            except Exception as ex:
                # If an unexpected error (disconnection, parse failure, etc.) occurs, print details and continue loop / 予期せぬエラー（通信切断、パース失敗など）が発生した場合は詳細を出力し、ループは継続させる
                print(f"Exception: {ex}")
                show_menu = False

        # Disconnect / 切断
        # After breaking loop, disconnect communication and perform termination processing / ループを抜けた後、通信を切断して終了処理を行う
        jig_cmd.Disconnect()
        print("Disconnected.")
        
        # Release resources if IDisposable interface is implemented / IDisposableインターフェースが実装されている場合はリソースを解放する
        if hasattr(jig_cmd, 'Dispose'):
            jig_cmd.Dispose()

        print()

if __name__ == "__main__":
    main()