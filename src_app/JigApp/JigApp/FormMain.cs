﻿// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Reflection;
using JigLib;

namespace JigApp
{
    public partial class FormMain : Form
    {
        private const string STR_NOT_DISPLAYED = "---";
        private const string STR_BTN_CONNECT = "Connect";
        private const string STR_BTN_DISCONNECT = "Disconnect";
        private const string STR_LBL_CONNECT = "Connected";
        private const string STR_LBL_DISCONNECT = "Disconnected";

        /// <summary>
        /// Wait time for microcontroller to restart (ms) / マイコンが再起動するのを待つ時間(ms)
        /// </summary>
        private const int REBOOT_WAIT = 5000;
        /// <summary>
        /// How long to retry reconnection (seconds) / どれくらいの時間の間、再接続のリトライを行うか(秒)
        /// </summary>
        private const int RECONNECT_TIME = 15;

        /// <summary>
        /// Own instance / 自分のインスタンス
        /// </summary>
        public static FormMain Inst { get; set; } = null;

        /// <summary>
        /// Application name / アプリ名
        /// </summary>
        private string _strAppName = null;
        /// <summary>
        /// NwConfig form / NwConfigフォーム
        /// </summary>
        private FormNwConfig _formNwConfig = null;
        /// <summary>
        /// GPIO form / GPIOフォーム
        /// </summary>
        private FormGpio _formGpio = null;
        /// <summary>
        /// ADC form / ADCフォーム
        /// </summary>
        private FormAdc _formAdc = null;
        /// <summary>
        /// UART form / UARTフォーム
        /// </summary>
        private FormUart _formUart = null;
        /// <summary>
        /// SPI form / SPIフォーム
        /// </summary>
        private FormSpi _formSpi = null;
        /// <summary>
        /// I2C form / I2Cフォーム
        /// </summary>
        private FormI2c _formI2c = null;
        /// <summary>
        /// PWM form / PWMフォーム
        /// </summary>
        private FormPwm _formPwm = null;
        /// <summary>
        /// List of child form display buttons / 子フォーム表示ボタンのリスト
        /// </summary>
        private List<Button> _lstButton = new List<Button>();
        /// <summary>
        /// List of FW error messages / FWエラーメッセージのリスト
        /// </summary>
        private List<string> _lstFwErrMsg = new List<string>();
        /// <summary>
        /// Task for monitoring / モニタ用タスク
        /// </summary>
        private Task<string> _tskMon = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            // Save own instance / 自分のインスタンスを保存
            Inst = this;
            // Register list of buttons / ボタンのリストを登録
            _lstButton.Add(button_NwConfig);
            _lstButton.Add(button_Gpio);
            _lstButton.Add(button_Adc);
            _lstButton.Add(button_Uart);
            _lstButton.Add(button_Spi);
            _lstButton.Add(button_I2c);
            _lstButton.Add(button_Pwm);
            _lstButton.Add(button_EraseFlash);
            _lstButton.Add(button_ClearFwErr);
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormMain_Load(object sender, EventArgs e)
        {
            // Display app name / アプリ名を表示
            _strAppName = Process.GetCurrentProcess().ProcessName;
            label_AppName.Text = _strAppName;
            // Display title / タイトルを表示
            this.Text = _strAppName + " - " + "Monitor Stopped";
            // Display app version / アプリのバージョンを表示
            FileVersionInfo verInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            label_AppVer.Text = verInfo.FileVersion;
            // Add list of COM port names to combo box / COMポート名の一覧をコンボボックスに追加
            AddSerialPortToList();
            // Set color of connection status label / 接続状態ラベルの色を設定
            label_ConnectStatus.BackColor = UI.MonRed;
            // Set child form display buttons to disabled / 子フォーム表示ボタンを無効に設定
            EnableFormButton(false);
        }

        /// <summary>
        /// Add list of COM port names to combo box / COMポート名の一覧をコンボボックスに追加
        /// </summary>
        private void AddSerialPortToList()
        {
            string[] astrPortName;

            // Get list of COM port names / COMポート名一覧を取得
            astrPortName = SerialPort.GetPortNames();
            Array.Sort(astrPortName); // ポート名の昇順にソート

            // Add list of port names to combo box / ポート名一覧をコンボボックスに追加
            for (int i = 0; i < astrPortName.Length; i++)
            {
                if (!comboBox_Port.Items.Contains(astrPortName[i]))
                {
                    comboBox_Port.Items.Add(astrPortName[i]);
                }
            }

            if (comboBox_Port.Items.Count > 0) // If the number of items in combo box is greater than 0 / コンボボックスのアイテム数が0より大きい場合
            {
                comboBox_Port.SelectedIndex = 0; // Select the first item / 先頭のアイテムを選択
            }
        }

        /// <summary>
        /// When closing the form / フォームを閉じる時
        /// </summary>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop timer to prevent callback during disposal / 破棄中にコールバックが呼ばれるのを防ぐためにタイマーを停止
            timer.Stop();
            // Disconnect / 切断する
            Program.PrpJigCmd.Disconnect();
        }

        /// <summary>
        /// Timer callback / タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Monitor connection status and FW error / 接続状態とFWエラーのモニタ
            Monitor();
        }

        /// <summary>
        /// Monitor connection status and FW error / 接続状態とFWエラーのモニタ
        /// </summary>
        private async void Monitor()
        {
            try
            {
                string strFwErrMsg;
                string strErrMsg;

                // [Monitor connection status] / [接続状態のモニタ]
                if ((!Program.PrpJigCmd.IsConnected()) && (label_ConnectStatus.Text == STR_LBL_CONNECT))
                {
                    // When connection with microcontroller is lost / マイコンとの接続が切断された場合
                    AppendAppLogText(true, "Connection status is abnormal.");
                    // Stop timer to prevent multiple retries / タイマーを止めて多重リトライを防ぐ
                    timer.Stop();
                    // Reconnect / 再接続する
                    await Reconnect();
                    return; // Skip subsequent processing / 以降の処理をスキップ
                }

                // [Monitor FW error] / [FWエラーのモニタ]
                if (true == Program.PrpJigCmd.IsConnected())
                {
                    if (_tskMon == null || _tskMon.IsCompleted)
                    {
                        List<string> lstTemp = null;
                        _tskMon = Task.Run(() =>
                        {
                            lstTemp = new List<string>();
                            // Send request for "Get FW Error" command / 「FWエラー取得」コマンドの要求を送信
                            return Program.PrpJigCmd.SendCmd_GetFwError(ref lstTemp);
                        });
                        strErrMsg = await _tskMon;

                        if (this.IsDisposed) return;

                        _lstFwErrMsg = lstTemp; // 代入はUIスレッドで行う

                        if (strErrMsg == null)
                        {
                            strFwErrMsg = string.Empty;
                            foreach (string strMsg in _lstFwErrMsg)
                            {
                                strFwErrMsg += (strMsg + "\r\n");
                            }
                            if (textBox_FwErr.Text != strFwErrMsg)
                            {
                                textBox_FwErr.Text = strFwErrMsg;
                            }
                            this.Text = _strAppName + " - " + Program.PrpJigCmd.PrpConnectName + " - " + "Monitoring";
                        }
                        else
                        {
                            this.Text = _strAppName + " - " + "Monitor Stopped";
                            AppendAppLogText(true, strErrMsg);
                        }
                    }
                }
                else
                {
                    this.Text = _strAppName + " - " + "Monitor Stopped";
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed)
                {
                    AppendAppLogText(true, $"Monitor Error: {ex.Message}");
                }
            }
        }

        // When the USB mode radio button is turned ON / USBモードのラジオボタンをONにした時
        private void radioButton_UsbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (true == radioButton_UsbMode.Checked)
            {
                comboBox_Port.Enabled = true;
                textBox_ServerIpAddr.Enabled = false;
            }
            else
            {
                comboBox_Port.Enabled = false;
                textBox_ServerIpAddr.Enabled = true;
            }
        }

        /// <summary>
        /// When the "Connect/Disconnect" button is pressed / 「接続/切断」ボタンを押した時
        /// </summary>
        private async void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioButton_UsbMode.Checked)// USB mode / USBモードの場合
                {
                    Program.PrpJigCmd = Program.PrpJigSerial;
                }
                else // Wi-Fi mode / Wi-Fiモードの場合
                {
                    Program.PrpJigCmd = Program.PrpJigTcpClient;
                }

                if (label_ConnectStatus.Text == STR_LBL_DISCONNECT) // If disconnected / 切断済みの場合
                {
                    // Connect / 接続する
                    await Connect();
                }
                else // If connected / 接続済みの場合
                {
                    // Disconnect / 切断する
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Connect Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Connect / 接続する
        /// </summary>
        private async Task Connect()
        {
            string strMakerName = STR_NOT_DISPLAYED; // Manufacturer name / メーカー名
            string strFwName = STR_NOT_DISPLAYED;    // FW name / FW名
            string strFwVer = STR_NOT_DISPLAYED;     // FW version / FWバージョン
            string strBoardId = STR_NOT_DISPLAYED;   // Board ID / ボードID
            string strParam; // COM port name/IP address / COMポート名/IPアドレス
            string strErrMsg = null;

            if (radioButton_UsbMode.Checked) // USB mode / USBモードの場合
            {
                if (comboBox_Port.Items.Count <= 0)
                {
                    strErrMsg = "There are no COM ports recognized by Windows.\r\nPlease connect the microcontroller board to the PC via USB and then restart this application.";
                    UI.ShowErrMsg(this, strErrMsg);
                    return;
                }
                strParam = comboBox_Port.Text.Trim(); // COM port name / COMポート名
            }
            else
            {
                strParam = textBox_ServerIpAddr.Text.Trim(); // IP address / IPアドレス
            }

            timer.Stop(); // Stop timer to prevent communication command conflict during connection / 通信コマンドの競合を防ぐため、接続中はタイマーを停止する
            this.Enabled = false;
            AppendAppLogText(false, "Connecting...");
          
            try
            {
                DateTime dt_start = DateTime.Now;
                DateTime dt_end;
                TimeSpan ts;
                do
                {
                    // Connect / 接続する
                    // Execute in separate task to prevent UI freeze / UIフリーズ防止のため別タスクで実行
                    strErrMsg = await Task.Run(() => Program.PrpJigCmd.Connect((Object)strParam));
                    if (strErrMsg == null)
                    {
                        break;
                    }
                    await Task.Delay(100); // Wait for retry / リトライ待ち
                    dt_end = DateTime.Now;
                    ts = dt_end - dt_start;
                } while (ts.TotalSeconds < RECONNECT_TIME);
                
                if (this.IsDisposed) return;
                    
                if (strErrMsg == null)
                {
                    // Send request for "Get FW Info" command / 「FW情報取得」コマンドの要求を送信
                    strErrMsg = await Task.Run(() =>
                    {
                        return Program.PrpJigCmd.SendCmd_GetFwInfo(out strMakerName, out strFwName, out strFwVer, out strBoardId);
                    });
                    
                    if (this.IsDisposed) return;
                    
                    if (strErrMsg != null)
                    {
                        strErrMsg = "Failed to retrieve firmware information after connecting.\n\n" + strErrMsg;
                    }
                }
                
                if (strErrMsg == null) // If command is successful / コマンドが成功した場合
                {
                    // [Update display] / [表示を更新]
                    // Set radio buttons to disabled / ラジオボタンを無効に設定
                    radioButton_UsbMode.Enabled = false;
                    radioButton_Wifi.Enabled = false;
                    // Set COM port name list combo box to disabled / COMポート名一覧のコンボボックスを無効に設定
                    comboBox_Port.Enabled = false;
                    // Set IP address text box to disabled / IPアドレスのテキストボックスを無効に設定
                    textBox_ServerIpAddr.Enabled = false;
                    // Connection status / 接続状態
                    AppendAppLogText(false, "connected");
                    label_ConnectStatus.Text = STR_LBL_CONNECT;
                    label_ConnectStatus.BackColor = UI.MonGreen;
                    // Change button text to "Disconnect" / ボタンの表示を「切断する」に変更
                    button_Connect.Text = STR_BTN_DISCONNECT;
                    // FW name / FW名
                    Str.PrpFwName = strFwName;
                    label_FwName.Text = strFwName;
                    // FW version / FWバージョン
                    label_FwVer.Text = strFwVer;
                    // Board ID / ボードID
                    label_BoardId.Text = strBoardId;
                    // Set child form display buttons to enabled / 子フォーム表示ボタンを有効に設定
                    EnableFormButton(true);
                }
                else // If connection or command failed / 接続が失敗 または コマンドが失敗した場合
                {
                    UI.ShowErrMsg(this, strErrMsg);
                    // Disconnect / 切断する
                    Program.PrpJigCmd.Disconnect();
                }
            }
            finally
            {
                if (!this.IsDisposed)
                {
                    timer.Start(); // Restart timer after connection processing is complete / 接続処理が完了したらタイマーを再開する
                    this.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Disconnect / 切断する
        /// </summary>
        private void Disconnect()
        {
            string strFwName = STR_NOT_DISPLAYED;  // FW name / FW名
            string strFwVer = STR_NOT_DISPLAYED;   // FW version / FWバージョン
            string strBoardId = STR_NOT_DISPLAYED; // Board ID / ボードID
            
            // Disconnect / 切断する
            Program.PrpJigCmd.Disconnect();
            // [Update display] / [表示を更新]
            // Set radio buttons to enabled / ラジオボタンを有効に設定
            radioButton_UsbMode.Enabled = true;
            radioButton_Wifi.Enabled = true;
            // Switch enabled/disabled according to mode / モードに応じて有効・無効を切り替える
            if (radioButton_UsbMode.Checked)
            {
                comboBox_Port.Enabled = true;
                textBox_ServerIpAddr.Enabled = false;
            }
            else
            {
                comboBox_Port.Enabled = false;
                textBox_ServerIpAddr.Enabled = true;
            }
            // Connection status / 接続状態
            AppendAppLogText(false, "disconnected");
            label_ConnectStatus.Text = STR_LBL_DISCONNECT;
            label_ConnectStatus.BackColor = UI.MonRed;
            // Change button text to "Connect" / ボタンの表示を「接続する」に変更
            button_Connect.Text = STR_BTN_CONNECT;
            // FW name / FW名
            label_FwName.Text = strFwName;
            // FW version / FWバージョン
            label_FwVer.Text = strFwVer;
            // Board ID / ボードID
            label_BoardId.Text = strBoardId;
            // Set child form display buttons to disabled / 子フォーム表示ボタンを無効に設定
            EnableFormButton(false);     
        }

        /// <summary>
        /// Reconnect / 再接続する
        /// </summary>
        /// <remarks>
        /// Use this function after a command that restarts the microcontroller succeeds / マイコンが再起動されるようなコマンドが成功した後に本関数を使用する
        /// </remarks>
        public async Task Reconnect()
        {
            // Do nothing if main form is already disposed when this function is called from another form / 他のフォームから本関数が呼ばれた時に、メインフォームが既に破棄されている場合は何もしない
            if (true == this.IsDisposed)
            {
                return;
            }

            try
            {
                // Stop timer to prevent Monitor from running during retry / リトライ中にMonitorが走るのを防ぐためタイマー停止
                timer.Stop(); 

                // Disconnect / 切断する
                Disconnect();
                this.Enabled = false;
                AppendAppLogText(false, "Reconnecting...");
                
                // Wait for microcontroller to restart / マイコンが再起動するのを待つ
                await Task.Delay(REBOOT_WAIT);
                
                if (this.IsDisposed) return;
                
                // Connect / 接続する
                await Connect();
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed)
                {
                    AppendAppLogText(true, $"Reconnect Error: {ex.Message}");
                }
            }
            finally
            {
                if (!this.IsDisposed)
                {
                    this.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Set enabled/disabled of child form display buttons / 子フォーム表示ボタンの有効/無効を設定
        /// </summary>
        void EnableFormButton(bool bEnable)
        {
            foreach (Button btn in _lstButton)
            {
                if  (btn == button_NwConfig)
                {
                    if (!((label_FwName.Text == Str.STR_FW_NAME_PICOJIG_WL) 
                        || (label_FwName.Text == Str.STR_FW_NAME_PICOBRG)
                        || (label_FwName.Text == Str.STR_FW_NAME_PICOIOT)
                        ))
                    {
                        btn.Enabled = false;
                        continue;
                    }
                }

                if (btn == button_Uart)
                {
                    if (label_FwName.Text == Str.STR_FW_NAME_PICOIOT)
                    {
                        btn.Enabled = false;
                        continue;
                    }
                }

                if ((btn == button_Adc) 
                    || (btn == button_Gpio) 
                    || (btn == button_I2c) 
                    || (btn == button_Pwm)
                    || (btn == button_Spi))
                {
                    if (!((label_FwName.Text == Str.STR_FW_NAME_PICOJIG) 
                        || (label_FwName.Text == Str.STR_FW_NAME_PICOJIG_WL)))
                    {
                        btn.Enabled = false;
                        continue;
                    }
                }

                btn.Enabled = bEnable;
            }
        }

        /// <summary>
        /// When the "NwConfig" button is pressed / 「NwConfig」ボタンを押した時
        /// </summary>
        private void button_NwConfig_Click(object sender, EventArgs e)
        {
            // Display NwConfig form / NwConfigフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// When the "GPIO" button is pressed / 「GPIO」ボタンを押した時
        /// </summary>
        private void button_Gpio_Click(object sender, EventArgs e)
        {
            // Display GPIO form / GPIOフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// When the "ADC" button is pressed / 「ADC」ボタンを押した時
        /// </summary>
        private void button_Adc_Click(object sender, EventArgs e)
        {
            // Display ADC form / ADCフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// When the "UART" button is pressed / 「UART」ボタンを押した時
        /// </summary>
        private void button_Uart_Click(object sender, EventArgs e)
        {
            // Display UART form / UARTフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// When the "SPI" button is pressed / 「SPI」ボタンを押した時
        /// </summary>
        private void button_Spi_Click(object sender, EventArgs e)
        {
            // Display SPI form / SPIフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// When the "I2C" button is pressed / 「I2C」ボタンを押したとき
        /// </summary>
        private void button_I2c_Click(object sender, EventArgs e)
        {
            // Display I2C form / I2Cフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// When the "PWM" button is pressed / 「PWM」ボタンを押した時
        /// </summary>
        private void button_Pwm_Click(object sender, EventArgs e)
        {
            // Display PWM form / PWMフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// Display child form according to child form display button / 子フォーム表示ボタンに応じた子フォームを表示
        /// </summary>
        private void ShowChildForm(Button button)
        {
            Form frm = null;

            if (button == button_NwConfig) // NwConfig
            {
                frm = _formNwConfig;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formNwConfig = new FormNwConfig();
                    frm.Show();
                }
            }
            else if (button == button_Gpio) // GPIO
            {
                frm = _formGpio;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formGpio = new FormGpio();
                    frm.Show();
                }
            }
            else if (button == button_Adc) // ADC
            {
                frm = _formAdc;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formAdc = new FormAdc();
                    frm.Show();
                }
            }
            else if (button == button_Uart) // UART
            {
                frm = _formUart;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formUart = new FormUart();
                    frm.Show();
                }
            }
            else if (button == button_I2c) // I2C
            {
                frm = _formI2c;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formI2c = new FormI2c();
                    frm.Show();
                }
            }
            else if (button == button_Spi) // SPI
            {
                frm = _formSpi;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formSpi = new FormSpi();
                    frm.Show();
                }
            }
            else if (button == button_Pwm) // PWM
            {
                frm = _formPwm;
                if (frm == null || frm.IsDisposed)
                {
                    frm = _formPwm = new FormPwm();
                    frm.Show();
                }
            }
            else
            {
                // No processing / 無処理
            }

            if (frm != null)
            {
                // Bring the form to the front properly / フォームを適切に前面に持ってくる
                frm.Activate();
                
                // Restore original state if child form is minimized / 子フォームが最小化されている時、元の状態に戻す
                if (frm.WindowState == FormWindowState.Minimized)
                {
                    frm.WindowState = FormWindowState.Normal;
                }
            }
        }

        /// <summary>
        /// When the "Erase Settings Data" button is pressed / 「設定データ消去」ボタンを押した時
        /// </summary>
        private async void button_EraseFlash_Click(object sender, EventArgs e)
        {
            try
            {
                string strErrMsg;

                // Display confirmation message / 確認メッセージを表示
                if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to erase the setting data in the flash memory?\n\n(The microcontroller will be reset.)"))
                {
                    return;
                }

                // Send request for "Erase Flash" command / 「FLASH消去」コマンドの要求を送信
                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    return Program.PrpJigCmd.SendCmd_EraseFlash();
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Reconnect / 再接続する
                    await Reconnect();
                }
                else
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Erase Flash Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }

        /// <summary>
        /// Add log to "App Log" text box / 「Appログ」テキストボックスにログを追加する
        /// </summary>
        public void AppendAppLogText(bool bError, string strMsg)
        {
            // Do nothing if main form is already disposed when this function is called from another form / 他のフォームから本関数が呼ばれた時に、メインフォームが既に破棄されている場合は何もしない
            if (true == this.IsDisposed)
            {
                return;
            }

            // Make the call thread-safe (Avoid cross-thread exception) / スレッドセーフな呼び出しにする（クロススレッド例外回避）
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AppendAppLogText(bError, strMsg)));
                return;
            }

            string strLog;

            // Prevent message from displaying when waiting for command response is canceled / 送信コマンドの応答待ちをキャンセルした時のメッセージを表示しないようにする
            if (strMsg == JigCmd.STR_MSG_WAIT_RES_CANCEL)
            {
                // No processing / 無処理
            }
            else
            {
                if (bError)
                {
                    strMsg = "Err!!! " + strMsg;
                }
                strLog = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + strMsg + "\r\n";
                textBox_AppLog.AppendText(strLog);
            }
        }

        /// <summary>
        /// When the "Clear App Log" button is pressed / 「Appエラークリア」ボタンを押した時
        /// </summary>
        private void button_ClearAppLog_Click(object sender, EventArgs e)
        {
            textBox_AppLog.Text = string.Empty;
        }

        /// <summary>
        /// When the "Clear FW Error" button is pressed / 「FWエラークリア」ボタンを押した時
        /// </summary>
        private async void button_ClearFwErr_Click(object sender, EventArgs e)
        {
            try
            {
                string strErrMsg;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Clear FW Error" command / 「FWエラークリア」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_ClearFwError();
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // No processing / 無処理
                }
                else
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Clear FW Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }

        /// <summary>
        /// Allow only half-width characters when key is pressed in text box / テキストボックスがキープレスされた時に半角のみ許可
        /// </summary>
        private void textBox_HalfWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            UI.TextBox_HalfWidth_KeyPress(sender, e);
        }
    }
}