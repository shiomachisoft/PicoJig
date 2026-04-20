using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JigLib;

namespace JigLibSample
{
    internal class Program
    {
        private const int ResetWaitTimeMs = 5000;

        /// <summary>
        /// Definition of available input GP numbers
        /// 利用可能な入力GP番号の定義
        /// </summary>
        private static readonly int[] ValidGpInputNumbers = { 3, 4, 5, 8, 9, 10, 11 };

        /// <summary>
        /// Definition of available output GP numbers
        /// 利用可能な出力GP番号の定義
        /// </summary>
        private static readonly int[] ValidGpOutputNumbers = { 12, 13, 14, 15, 20, 21, 22 };

        /// <summary>
        /// Main entry point of the application
        /// アプリケーションのメインエントリポイント
        /// </summary>
        /// <param name="args">Command line arguments / コマンドライン引数</param>
        static void Main(string[] args)
        {
            while (true)
            {
                JigCmd jigCmd;

                // Select communication method and create instance / 通信方式の選択とインスタンスの生成
                Console.WriteLine("@Please select a communication method.");
                Console.WriteLine("  1: USB   (COM)");
                Console.WriteLine("  2: Wi-Fi (TCP/IP)");
                Console.Write("Select Method > ");

                string commType;
                while (true)
                {
                    commType = Console.ReadLine();
                    if (commType == "1" || commType == "2") break;

                    Console.WriteLine("Invalid selection. Please select 1 or 2.");
                    Console.Write("Select Method > ");
                }

                if (commType == "2")
                {
                    jigCmd = new JigTcpClient();
                    Console.Write("Please enter the IP address (e.g., 192.168.10.100): ");
                }
                else // commType == "1"
                {
                    jigCmd = new JigSerial();
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames()
                        .Distinct()
                        .OrderBy(p => int.TryParse(new string(p.Where(char.IsDigit).ToArray()), out int num) ? num : 0)
                        .ToArray();
                    if (ports.Length > 0)
                    {
                        Console.WriteLine("Available COM ports:");
                        foreach (string port in ports)
                        {
                            Console.WriteLine($"  {port}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No COM ports available.");
                    }
                    Console.Write("Please enter the COM port name (e.g., COM1): ");
                }

                string connectParam = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(connectParam))
                {
                    Console.WriteLine("Connection parameter cannot be empty.");
                    Console.WriteLine();
                    if (jigCmd is IDisposable d) d.Dispose();
                    continue;
                }

                string commMethod = commType == "1" ? "USB (COM)" : "Wi-Fi (TCP/IP)";
                Console.WriteLine($"Connecting to {connectParam} via {commMethod}...");

                // Connect / 接続
                string errMsg = jigCmd.Connect(connectParam);
                if (errMsg != null)
                {
                    Console.WriteLine($"Connection error: {errMsg}");
                    Console.WriteLine();
                    jigCmd.Disconnect();
                    if (jigCmd is IDisposable d) d.Dispose();
                    continue;
                }

                Console.WriteLine($"Connected successfully via {commMethod}.");

                bool loop = true;
                bool showMenu = true;
                while (loop)
                {
                    try
                    {
                        if (showMenu)
                        {
                            Console.WriteLine();
                            Console.WriteLine("--------------------------------------------------");

                            Console.WriteLine("Please select a command to execute.");
                            Console.WriteLine("  1: Get FW Info");
                            Console.WriteLine("  2: Get Wi-Fi Config");
                            Console.WriteLine("  3: Set Wi-Fi Config");
                            Console.WriteLine("  4: Get ADC Value");
                            Console.WriteLine("  5: Get GPIO Config");
                            Console.WriteLine("  6: Set GPIO Config");
                            Console.WriteLine("  7: Get GPIO I/O Value");
                            Console.WriteLine("  8: Set GPIO Output");
                            Console.WriteLine("  9: Get UART Config");
                            Console.WriteLine(" 10: Set UART Config");
                            Console.WriteLine(" 11: UART Send");
                            Console.WriteLine(" 12: UART Receive");
                            Console.WriteLine(" 13: Get I2C Config");
                            Console.WriteLine(" 14: Set I2C Config");
                            Console.WriteLine(" 15: I2C Send");
                            Console.WriteLine(" 16: I2C Receive");
                            Console.WriteLine(" 17: Get SPI Config");
                            Console.WriteLine(" 18: Set SPI Config");
                            Console.WriteLine(" 19: SPI Comm (Send/Receive)");
                            Console.WriteLine(" 20: Start PWM");
                            Console.WriteLine(" 21: Stop PWM");
                            Console.WriteLine(" 22: Get FW Error");
                            Console.WriteLine(" 23: Clear FW Error");
                            Console.WriteLine(" 24: Erase Flash");
                            Console.WriteLine("  0: Disconnect");
                            showMenu = false;
                        }

                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine($"[Current Connection: {commMethod} - {connectParam}]");
                        Console.WriteLine("@Select Menu");
                        Console.Write("  Enter: Show menu, Number: Execute > ");
                        string choice = Console.ReadLine();
                        Console.WriteLine();

                        if (string.IsNullOrEmpty(choice))
                        {
                            showMenu = true;
                            continue;
                        }

                        switch (choice)
                        {
                            case "1":
                                {
                                    // Get FW info / FW情報の取得
                                    string makerName, fwName, fwVer, boardId;
                                    errMsg = jigCmd.SendCmd_GetFwInfo(out makerName, out fwName, out fwVer, out boardId);

                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- FW Info ---");
                                        Console.WriteLine($"Maker: {makerName}");
                                        Console.WriteLine($"FW Name: {fwName}");
                                        Console.WriteLine($"FW Version: {fwVer}");
                                        Console.WriteLine($"Board ID: {boardId}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"FW info get error: {errMsg}");
                                    }
                                    break;
                                }
                            case "2":
                                {
                                    // Get network config / ネットワーク設定の取得
                                    string countryCode, ipAddr, ssid, password;
                                    errMsg = jigCmd.SendCmd_GetNwConfig(out countryCode, out ipAddr, out ssid, out password);

                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- Network configuration ---");
                                        Console.WriteLine($"Device IP Address:  {ipAddr.TrimEnd('\0')}");
                                        Console.WriteLine($"SSID:               {ssid.TrimEnd('\0')}");
                                        Console.WriteLine($"Password:           (hidden)");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[Error] {errMsg}");
                                    }
                                    break;
                                }
                            case "3":
                                {
                                    // Set Wi-Fi config / Wi-Fi設定
                                    Console.WriteLine("--- Set Wi-Fi Config ---");

                                    string countryCode = "XX"; // The country code is sent to the MCU, but it is currently unused. Please specify "XX". / カントリーコードはマイコン側へ送信されますが、現在は未使用です。"XX"を指定してください。

                                    Console.Write("IP Address: ");
                                    string ipInput = Console.ReadLine();
                                    string ipAddr = ipInput;

                                    Console.Write("SSID: ");
                                    string sInput = Console.ReadLine();
                                    string ssid = sInput;

                                    Console.Write("SSID Password: ");
                                    string pInput = Console.ReadLine();
                                    string ssid_password = pInput;

                                    if (string.IsNullOrWhiteSpace(ipAddr) || string.IsNullOrWhiteSpace(ssid) || string.IsNullOrWhiteSpace(ssid_password))
                                    {
                                        Console.WriteLine("Error: IP Address, SSID, and Password cannot be empty.");
                                        break;
                                    }

                                    errMsg = jigCmd.SendCmd_SetNwConfig(countryCode, ipAddr, ssid, ssid_password);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine($"Wi-Fi configured.");
                                        Console.WriteLine($"MCU will be reset, waiting for {ResetWaitTimeMs / 1000} seconds...");
                                        System.Threading.Thread.Sleep(ResetWaitTimeMs);
                                        if (commType == "2" && !string.IsNullOrWhiteSpace(ipAddr))
                                        {
                                            connectParam = ipAddr; // In the case of TCP/IP connection, reconnect with the new IP address after change / TCP/IP接続の場合は変更後の新しいIPアドレスで再接続する
                                        }                                    
                                        if (!ReconnectMcu(jigCmd, connectParam)) loop = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "4":
                                {
                                    // Get ADC value / ADC値の取得
                                    float[] aVolt;
                                    errMsg = jigCmd.SendCmd_GetAdc(out aVolt);

                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- ADC Values ---");
                                        for (int i = 0; i < aVolt.Length; i++)
                                        {
                                            if (i == 3)
                                            {
                                                Console.WriteLine($"CH4 (Temp): {aVolt[i]:F2} deg C");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"CH{i}: {aVolt[i]:F3} V");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"ADC get error: {errMsg}");
                                    }
                                    break;
                                }
                            case "5":
                                {
                                    // Get GPIO config / GPIO設定の取得
                                    uint pullDownBits, initialOutValBits;
                                    errMsg = jigCmd.SendCmd_GetGpioConfig(out pullDownBits, out initialOutValBits);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- GPIO configuration ---");
                                        Console.WriteLine("[Input GPs] Pull-up/Pull-down");
                                        foreach (int gp in ValidGpInputNumbers)
                                        {
                                            Console.WriteLine($"GP{gp}: {((pullDownBits & (1u << gp)) != 0 ? "Pull-down" : "Pull-up")}");
                                        }                   
                                        Console.WriteLine("[Output GPs] Initial output value");
                                        foreach (int gp in ValidGpOutputNumbers)
                                        {
                                            Console.WriteLine($"GP{gp}: {((initialOutValBits & (1u << gp)) != 0 ? "High" : "Low")}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[Error] {errMsg}");
                                    }
                                    break;
                                }
                            case "6":
                                {
                                    // Set GPIO config / GPIO設定
                                    Console.WriteLine("--- Set GPIO Config ---");

                                    uint pullDownBits = 0;
                                    Console.WriteLine("[Input GPs] Pull-up/Pull-down");
                                    foreach (int gp in ValidGpInputNumbers)
                                    {
                                        Console.Write($"GP{gp} (0:Pull-up, 1:Pull-down): ");
                                        string input = Console.ReadLine();
                                        if (input == "1") pullDownBits |= (1u << gp);
                                    }

                                    uint initialOutValBits = 0;
                                    Console.WriteLine("[Output GPs] Initial output value");
                                    foreach (int gp in ValidGpOutputNumbers)
                                    {
                                        Console.Write($"GP{gp} (0:Low, 1:High): ");
                                        string input = Console.ReadLine();
                                        if (input == "1") initialOutValBits |= (1u << gp);
                                    }

                                    errMsg = jigCmd.SendCmd_SetGpioConfig(pullDownBits, initialOutValBits);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("GPIO config completed.");
                                        Console.WriteLine($"MCU will be reset, waiting for {ResetWaitTimeMs / 1000} seconds...");
                                        System.Threading.Thread.Sleep(ResetWaitTimeMs);
                                        if (!ReconnectMcu(jigCmd, connectParam)) loop = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "7":
                                {
                                    // Get GPIO I/O value / GPIO入出力値取得
                                    uint inOutBits;
                                    errMsg = jigCmd.SendCmd_GetGpio(out inOutBits);

                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- Get GPIO I/O Values ---");
                                        Console.WriteLine("[Input GPs] Values");
                                        foreach (int gp in ValidGpInputNumbers)
                                        {
                                            Console.WriteLine($"GP{gp}: {((inOutBits & (1u << gp)) != 0 ? "High" : "Low")}");
                                        }
                                        Console.WriteLine("[Output GPs] Values");
                                        foreach (int gp in ValidGpOutputNumbers)
                                        {
                                            Console.WriteLine($"GP{gp}: {((inOutBits & (1u << gp)) != 0 ? "High" : "Low")}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Get GPIO I/O Values error: {errMsg}");
                                    }
                                    break;
                                }
                            case "8":
                                {
                                    // GPIO output / GPIO出力
                                    Console.WriteLine("--- Set GPIO Output ---");
                                    uint outBits = 0;
                                    Console.WriteLine("[Output GPs] Status");
                                    foreach (int gp in ValidGpOutputNumbers)
                                    {
                                        Console.Write($"  GP{gp} (0:Low, 1:High): ");
                                        string input = Console.ReadLine();
                                        if (input == "1") outBits |= (1u << gp);
                                    }

                                    errMsg = jigCmd.SendCmd_OutGpio(outBits);
                                    if (errMsg == null)
                                    {
                                        foreach (int gp in ValidGpOutputNumbers)
                                        {
                                            Console.WriteLine($"  GP{gp}: {((outBits & (1u << gp)) != 0 ? "High" : "Low")}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "9":
                                {
                                    // Get UART config / UART設定の取得
                                    uint baudrate;
                                    byte dataBits, stopBits, parity;
                                    errMsg = jigCmd.SendCmd_GetUartConfig(out baudrate, out dataBits, out stopBits, out parity);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- UART configuration ---");
                                        Console.WriteLine($"  Baudrate: {baudrate} bps");
                                        Console.WriteLine($"  DataBits: {dataBits}");
                                        Console.WriteLine($"  StopBits: {stopBits}");
                                        Console.WriteLine($"  Parity:   {parity} (0:None, 1:Even, 2:Odd)");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[Error] {errMsg}");
                                    }
                                    break;
                                }
                            case "10":
                                {
                                    // Set UART config / UART設定
                                    Console.WriteLine("--- Set UART Config ---");

                                    Console.Write("Baudrate: ");
                                    string bInput = Console.ReadLine();
                                    uint baudrate = uint.Parse(bInput);

                                    byte dataBits = 8; // Fixed to 8 according to the library usage guide / ライブラリ利用ガイドにより8固定

                                    Console.Write("Stop bits: ");
                                    string sInput = Console.ReadLine();
                                    byte stopBits = byte.Parse(sInput);

                                    Console.Write("Parity (0:None, 1:Even, 2:Odd): ");
                                    string pInput = Console.ReadLine();
                                    byte parity = byte.Parse(pInput);

                                    errMsg = jigCmd.SendCmd_SetUartConfig(baudrate, dataBits, stopBits, parity);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine($"UART configured.");
                                        Console.WriteLine($"MCU will be reset, waiting for {ResetWaitTimeMs / 1000} seconds...");
                                        System.Threading.Thread.Sleep(ResetWaitTimeMs);
                                        if (!ReconnectMcu(jigCmd, connectParam)) loop = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "11":
                                {
                                    // UART send / UART送信
                                    Console.WriteLine("--- UART Send ---");
                                    Console.Write("Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): ");
                                    string dataStr = Console.ReadLine();
                                    byte[] txData = dataStr.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(s => Convert.ToByte(s, 16))
                                                           .ToArray();

                                    if (txData.Length == 0 || txData.Length > 256)
                                    {
                                        Console.WriteLine("Error: Send data must be between 1 and 256 bytes.");
                                        break;
                                    }

                                    errMsg = jigCmd.SendCmd_SendUart(txData);
                                    if (errMsg == null) Console.WriteLine($"UART sent: {txData.Length} bytes");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "12":
                                {
                                    // UART receive / UART受信
                                    Console.WriteLine("--- UART Receive ---");
                                    // According to JigCmd specifications, UART received data is stored asynchronously in PrpUartRecvDataQue / JigCmdの仕様では、UART受信データは PrpUartRecvDataQue に非同期で格納されます
                                    if (jigCmd.PrpUartRecvDataQue.Count > 0)
                                    {
                                        List<byte> rxList = new List<byte>();
                                        while (jigCmd.PrpUartRecvDataQue.TryTake(out byte b)) // The queue for UART received data is up to 4096 bytes / UART受信データのキューは最大4096byte
                                        {
                                            rxList.Add(b);
                                        }
                                        Console.WriteLine($"UART received (Hex): {BitConverter.ToString(rxList.ToArray()).Replace("-", " ")}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("No UART received data.");
                                    }
                                    break;
                                }
                            case "13":
                                {
                                    // Get I2C config / I2C設定の取得
                                    uint freq;
                                    errMsg = jigCmd.SendCmd_GetI2cConfig(out freq);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- I2C configuration ---");
                                        Console.WriteLine($"  Frequency: {freq} Hz");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[Error] {errMsg}");
                                    }
                                    break;
                                }
                            case "14":
                                {
                                    // Set I2C config / I2C設定
                                    Console.WriteLine("--- Set I2C Config ---");

                                    Console.Write("Frequency(Hz): ");
                                    string fInput = Console.ReadLine();
                                    uint freq = uint.Parse(fInput);

                                    errMsg = jigCmd.SendCmd_SetI2cConfig(freq);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine($"I2C configured.");
                                        Console.WriteLine($"MCU will be reset, waiting for {ResetWaitTimeMs / 1000} seconds...");
                                        System.Threading.Thread.Sleep(ResetWaitTimeMs);
                                        if (!ReconnectMcu(jigCmd, connectParam)) loop = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "15":
                                {
                                    // I2C send / I2C送信
                                    Console.WriteLine("--- I2C Send ---");
                                    Console.Write("7bit Slave Address (Hex): 0x");
                                    byte devAddr = Convert.ToByte(Console.ReadLine(), 16);

                                    Console.Write("Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): ");
                                    string dataStr = Console.ReadLine();
                                    byte[] txData = dataStr.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(s => Convert.ToByte(s, 16))
                                                           .ToArray();

                                    if (txData.Length == 0 || txData.Length > 256)
                                    {
                                        Console.WriteLine("Error: Send data must be between 1 and 256 bytes.");
                                        break;
                                    }

                                    errMsg = jigCmd.SendCmd_SendI2c(devAddr, txData);
                                    if (errMsg == null) Console.WriteLine("I2C sent.");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "16":
                                {
                                    // I2C receive / I2C受信
                                    Console.WriteLine("--- I2C Receive ---");
                                    Console.Write("7bit Slave Address (Hex): 0x");
                                    byte devAddr = Convert.ToByte(Console.ReadLine(), 16);

                                    Console.Write("Read Length: ");
                                    ushort readLen = ushort.Parse(Console.ReadLine());

                                    byte[] rxData;
                                    errMsg = jigCmd.SendCmd_RecvI2c(devAddr, readLen, out rxData);
                                    if (errMsg == null) Console.WriteLine($"I2C received (Hex): {BitConverter.ToString(rxData).Replace("-", " ")}");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "17":
                                {
                                    // Get SPI config / SPI設定の取得
                                    uint freq;
                                    byte dataBits, polarity, phase, order;
                                    errMsg = jigCmd.SendCmd_GetSpiConfig(out freq, out dataBits, out polarity, out phase, out order);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("--- SPI configuration ---");
                                        Console.WriteLine($"  Frequency: {freq} Hz");
                                        Console.WriteLine($"  DataBits:  {dataBits}");
                                        Console.WriteLine($"  CPOL:      {polarity}");
                                        Console.WriteLine($"  CPHA:      {phase}");
                                        Console.WriteLine($"  Order:     {order} (1:MSB First)");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[Error] {errMsg}");
                                    }
                                    break;
                                }
                            case "18":
                                {
                                    // Set SPI config / SPI設定
                                    Console.WriteLine("--- Set SPI Config ---");

                                    Console.Write("Frequency(Hz): ");
                                    string fInput = Console.ReadLine();
                                    uint freq = uint.Parse(fInput);

                                    byte dataBits = 8; // Fixed to 8 according to the library usage guide / ライブラリ利用ガイドにより8固定

                                    Console.Write("Polarity (0:CPOL=0, 1:CPOL=1): ");
                                    string polInput = Console.ReadLine();
                                    byte polarity = byte.Parse(polInput);

                                    Console.Write("Phase (0:CPHA=0, 1:CPHA=1): ");
                                    string phInput = Console.ReadLine();
                                    byte phase = byte.Parse(phInput);

                                    byte order = 1; // Fixed to 1 (MSB First) according to the library usage guide / ライブラリ利用ガイドにより1 (MSB First)固定

                                    errMsg = jigCmd.SendCmd_SetSpiConfig(freq, dataBits, polarity, phase, order);
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine($"SPI configured.");
                                        Console.WriteLine($"MCU will be reset, waiting for {ResetWaitTimeMs / 1000} seconds...");
                                        System.Threading.Thread.Sleep(ResetWaitTimeMs);
                                        if (!ReconnectMcu(jigCmd, connectParam)) loop = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "19":
                                {
                                    // SPI comm (send/receive) / SPI通信 (送受信)
                                    Console.WriteLine("--- SPI Comm ---");
                                    Console.Write("Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): ");
                                    string dataStr = Console.ReadLine();
                                    byte[] txData = dataStr.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(s => Convert.ToByte(s, 16))
                                                           .ToArray();

                                    if (txData.Length == 0 || txData.Length > 256)
                                    {
                                        Console.WriteLine("Error: Send data must be between 1 and 256 bytes.");
                                        break;
                                    }

                                    byte[] rxData;
                                    errMsg = jigCmd.SendCmd_SendSpi(txData, out rxData);
                                    if (errMsg == null) Console.WriteLine($"SPI received data (Hex): {BitConverter.ToString(rxData).Replace("-", " ")}");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "20":
                                {
                                    // Start PWM / PWM開始
                                    Console.WriteLine("--- Start PWM ---");

                                    Console.Write("Clock divider: ");
                                    string divInput = Console.ReadLine();
                                    float div = float.Parse(divInput);

                                    Console.Write("Wrap: ");
                                    string wInput = Console.ReadLine();
                                    ushort wrap = ushort.Parse(wInput);

                                    Console.Write("Compare value(Level): ");
                                    string lInput = Console.ReadLine();
                                    ushort level = ushort.Parse(lInput);

                                    errMsg = jigCmd.SendCmd_StartPwm(div, wrap, level);
                                    if (errMsg == null) Console.WriteLine("PWM started.");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "21":
                                {
                                    // Stop PWM / PWM停止
                                    Console.WriteLine("--- Stop PWM ---");
                                    errMsg = jigCmd.SendCmd_StopPwm();
                                    if (errMsg == null) Console.WriteLine("PWM stopped.");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "22":
                                {
                                    // Get FW error / FWエラー取得
                                    List<string> errList = new List<string>();
                                    errMsg = jigCmd.SendCmd_GetFwError(ref errList);
                                    if (errMsg == null)
                                    {
                                        if (errList.Count == 0)
                                        {
                                            Console.WriteLine("No FW errors.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("--- FW Errors ---");
                                            foreach (var err in errList)
                                            {
                                                Console.WriteLine($" - {err}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "23":
                                {
                                    // Clear FW error / FWエラークリア
                                    Console.WriteLine("--- Clear FW Error ---");
                                    errMsg = jigCmd.SendCmd_ClearFwError();
                                    if (errMsg == null) Console.WriteLine("FW errors cleared.");
                                    else Console.WriteLine($"Error: {errMsg}");
                                    break;
                                }
                            case "24":
                                {
                                    // Erase flash / フラッシュ消去
                                    Console.WriteLine("--- Erase Flash ---");
                                    errMsg = jigCmd.SendCmd_EraseFlash();
                                    if (errMsg == null)
                                    {
                                        Console.WriteLine("Flash erased successfully.");
                                        Console.WriteLine($"MCU will be reset, waiting for {ResetWaitTimeMs / 1000} seconds...");
                                        System.Threading.Thread.Sleep(ResetWaitTimeMs);
                                        if (!ReconnectMcu(jigCmd, connectParam)) loop = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {errMsg}");
                                    }
                                    break;
                                }
                            case "0":
                                loop = false;
                                break;

                            default:
                                Console.WriteLine("Invalid selection. Please enter a menu number.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                        showMenu = false;
                    }
                }

                // Disconnect / 切断
                jigCmd.Disconnect();
                Console.WriteLine("Disconnected.");
                if (jigCmd is IDisposable disposable) disposable.Dispose();

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Disconnect communication with the MCU and reconnect
        /// マイコンとの通信を切断し、再度接続処理を行う
        /// </summary>
        /// <param name="jigCmd">JigCmd instance / JigCmdインスタンス</param>
        /// <param name="connectParam">Connection parameter (COM port name or IP address) / 接続パラメータ（COMポート名またはIPアドレス）</param>
        private static bool ReconnectMcu(JigCmd jigCmd, string connectParam)
        {
            Console.WriteLine($"Reconnecting to {connectParam}...");
            jigCmd.Disconnect();
            string err = jigCmd.Connect(connectParam);
            if (err != null)
            {
                Console.WriteLine($"Reconnection error: {err}");
                return false;
            }
            else
            {
                Console.WriteLine("Reconnected successfully.");
                return true;
            }
        }

    }
}
