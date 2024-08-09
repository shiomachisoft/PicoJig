// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;
using System.Reflection;
using JigLib;

namespace JigApp
{
    public partial class FormMain : Form
    {
        private const string STR_NOT_DISPLAYED = "---";
        private const string STR_BTN_CONNECT = "connect";
        private const string STR_BTN_DISCONNECT = "disconnect";
        private const string STR_LBL_CONNECT = "connected";
        private const string STR_LBL_DISCONNECT = "disconnected";

        /// <summary>
        /// マイコンが再起動するのを待つ時間(ms)
        /// </summary>
        private const int REBOOT_WAIT = 5000;
        /// <summary>
        /// どれくらいの時間の間、再接続のリトライを行うか(秒)
        /// </summary>
        private const int RECONNECT_TIME = 15;

        /// <summary>
        /// 自分のインスタンス
        /// </summary>
        public static FormMain Inst { get; set; } = null;

        /// <summary>
        /// アプリ名
        /// </summary>
        private string _strAppName = null;
        /// <summary>
        /// NwConfigフォーム
        /// </summary>
        private FormNwConfig _formNwConfig = null;
        /// <summary>
        /// GPIOフォーム
        /// </summary>
        private FormGpio _formGpio = null;
        /// <summary>
        /// ADCフォーム
        /// </summary>
        private FormAdc _formAdc = null;
        /// <summary>
        /// UARTフォーム
        /// </summary>
        private FormUart _formUart = null;
        /// <summary>
        /// SPIフォーム
        /// </summary>
        private FormSpi _formSpi = null;
        /// <summary>
        /// I2Cフォーム
        /// </summary>
        private FormI2c _formI2c = null;
        /// <summary>
        /// PWMフォーム
        /// </summary>
        private FormPwm _formPwm = null;
        /// <summary>
        /// 子フォーム表示ボタンのリスト
        /// </summary>
        private List<Button> _lstButton = new List<Button>();
        /// <summary>
        /// FWエラーメッセージのリスト
        /// </summary>
        private List<string> _lstFwErrMsg = new List<string>();
        /// <summary>
        /// モニタ用タスク
        /// </summary>
        private Task<string> _tskMon = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            // 自分のインスタンスを保存
            Inst = this;
            // ボタンのリストを登録
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
        /// フォームのロード時
        /// </summary>
        private void FormMain_Load(object sender, EventArgs e)
        {
            // アプリ名を表示
            _strAppName = Process.GetCurrentProcess().ProcessName;
            label_AppName.Text = _strAppName;
            // タイトルを表示
            this.Text = _strAppName + " - " + "Monitor stopped";
            // アプリのバージョンを表示
            FileVersionInfo verInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            label_AppVer.Text = verInfo.FileVersion;
            // COMポート名の一覧をコンボボックスに追加
            AddSerialPortToList();
            // 接続状態ラベルの色を設定
            label_ConnectStatus.BackColor = UI.MonRed;
            // 子フォーム表示ボタンを無効に設定
            EnableFormButton(false);
        }

        /// <summary>
        /// COMポート名の一覧をコンボボックスに追加
        /// </summary>
        private void AddSerialPortToList()
        {
            string[] astrPortName;

            // COMポート名一覧を取得
            astrPortName = SerialPort.GetPortNames();
            Array.Sort(astrPortName); // ポート名の昇順にソート

            // ポート名一覧をコンボボックスに追加
            for (int i = 0; i < astrPortName.Length; i++)
            {
                comboBox_Port.Items.Add(astrPortName[i]);
            }

            if (comboBox_Port.Items.Count > 0) // コンボボックスのアイテム数が0より大きい場合
            {
                comboBox_Port.SelectedIndex = 0; // 先頭のアイテムを選択
            }
        }

        /// <summary>
        /// フォームを閉じる時
        /// </summary>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 切断する
            Program.PrpJigCmd.Disconnect();
        }

        /// <summary>
        /// タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // 接続状態とFWエラーのモニタ
            Monitor();
        }

        /// <summary>
        /// 接続状態とFWエラーのモニタ
        /// </summary>
        private async void Monitor()
        {
            string strFwErrMsg;
            string strErrMsg;

            // [接続状態のモニタ]
            if ((!Program.PrpJigCmd.IsConnected()) && (label_ConnectStatus.Text == STR_LBL_CONNECT)) 
            {
                // マイコンとの接続がUI操作以外の要因で切断された場合
                AppendAppLogText(true, "Connection status is abnormal.");
                // 再接続する
                Reconnect();
            }

            // [FWエラーのモニタ]
            if (true == Program.PrpJigCmd.IsConnected())
            {
                if (_tskMon == null || (_tskMon != null && _tskMon.IsCompleted))
                {
                    _tskMon = Task.Run(() =>
                    {
                        //「FWエラー取得」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetFwError(ref _lstFwErrMsg);
                    });
                    strErrMsg = await _tskMon;
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
                        this.Text = _strAppName + " - " + "Monitor stopped";
                        AppendAppLogText(true, strErrMsg);
                    }
                }
            }
            else
            {
                this.Text = _strAppName + " - " + "Monitor stopped";
            }
        }

        // USBモードのラジオボタンをONにした時
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
        /// 「接続/切断」ボタンを押した時
        /// </summary>
        private void button_Connect_Click(object sender, EventArgs e)
        {
            if (radioButton_UsbMode.Checked)// USBモードの場合
            {
                Program.PrpJigCmd = Program.PrpJigSerial;
            }
            else // Wi-Fiモードの場合
            {
                Program.PrpJigCmd = Program.PrpJigTcpClient;
            }

            if (label_ConnectStatus.Text == STR_LBL_DISCONNECT) // 切断済みの場合
            {
                // 接続する
                Connect();
            }
            else // 接続済みの場合
            {
                // 切断する
                Disconnect();
            }
        }

        /// <summary>
        /// 接続する
        /// </summary>
        private async void Connect()
        {
            string strMakerName = STR_NOT_DISPLAYED; // メーカー名
            string strFwName = STR_NOT_DISPLAYED;    // FW名
            string strFwVer = STR_NOT_DISPLAYED;     // FWバージョン
            string strBoardId = STR_NOT_DISPLAYED;   // ボードID
            string strParam; // COMポート名/IPアドレス
            string strErrMsg = null;

            if (radioButton_UsbMode.Checked) // USBモードの場合
            {
                if (comboBox_Port.Items.Count <= 0)
                {
                    strErrMsg = "There are no COM ports recognized by Windows.\r\nPlease connect the microcontroller board to the PC via USB and then restart this application.";
                    UI.ShowErrMsg(this, strErrMsg);
                    return;
                }
                strParam = comboBox_Port.Text.Trim(); // COMポート名
            }
            else
            {
                strParam = textBox_ServerIpAddr.Text.Trim(); // IPアドレス
            }

            AppendAppLogText(false, "Try connecting...");

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                DateTime dt_start = DateTime.Now;
                DateTime dt_end;
                TimeSpan ts;
                do
                {
                    // 接続する
                    strErrMsg = Program.PrpJigCmd.Connect((Object)strParam);
                    if (strErrMsg == null)
                    {
                        break;
                    }
                    dt_end = DateTime.Now;
                    ts = dt_end - dt_start;
                } while (ts.Seconds < RECONNECT_TIME);
                
                if (strErrMsg == null)
                {
                    //「FW情報取得」コマンドの要求を送信
                    strErrMsg = Program.PrpJigCmd.SendCmd_GetFwInfo(out strMakerName, out strFwName, out strFwVer, out strBoardId);
                    if (strErrMsg != null)
                    {
                        strErrMsg = "Firmware information could not be obtained from the microcontroller after connection.\n\n" + strErrMsg;
                    }
                }
                return strErrMsg;
            });
            this.Enabled = true;
            if (strErrMsg == null) // コマンドが成功した場合
            {
                // [表示を更新]
                // ラジオボタンを無効に設定
                radioButton_UsbMode.Enabled = false;
                radioButton_Wifi.Enabled = false;
                // COMポート名一覧のコンボボックスを無効に設定
                comboBox_Port.Enabled = false;
                // IPアドレスのテキストボックスを無効に設定
                textBox_ServerIpAddr.Enabled = false;
                // 接続状態
                AppendAppLogText(false, "connected");
                label_ConnectStatus.Text = STR_LBL_CONNECT;
                label_ConnectStatus.BackColor = UI.MonGreen;
                // ボタンの表示を「切断する」に変更
                button_Connect.Text = STR_BTN_DISCONNECT;
                // FW名
                Str.PrpFwName = strFwName;
                label_FwName.Text = strFwName;
                // FWバージョン
                label_FwVer.Text = strFwVer;
                // ボードID
                label_BoardId.Text = strBoardId;
                // 子フォーム表示ボタンを有効に設定
                EnableFormButton(true);
            }
            else // 接続が失敗 または コマンドが失敗した場合
            {
                UI.ShowErrMsg(this, strErrMsg);
                // 切断する
                Program.PrpJigCmd.Disconnect();
            }
        }

        /// <summary>
        /// 切断する
        /// </summary>
        private void Disconnect()
        {
            string strFwName = STR_NOT_DISPLAYED;  // FW名
            string strFwVer = STR_NOT_DISPLAYED;   // FWバージョン
            string strBoardId = STR_NOT_DISPLAYED; // ボードID
            
            // 切断する
            Program.PrpJigCmd.Disconnect();
            // [表示を更新]
            // ラジオボタンを有効に設定
            radioButton_UsbMode.Enabled = true;
            radioButton_Wifi.Enabled = true;
            // COMポート名一覧のコンボボックスを有効に設定
            comboBox_Port.Enabled = true;
            // IPアドレスのテキストボックスを有効に設定
            textBox_ServerIpAddr.Enabled = true;
            // 接続状態
            AppendAppLogText(false, "disconnected");
            label_ConnectStatus.Text = STR_LBL_DISCONNECT;
            label_ConnectStatus.BackColor = UI.MonRed;
            // ボタンの表示を「接続する」に変更
            button_Connect.Text = STR_BTN_CONNECT;
            // FW名
            label_FwName.Text = strFwName;
            // FWバージョン
            label_FwVer.Text = strFwVer;
            // ボードID
            label_BoardId.Text = strBoardId;
            // 子フォーム表示ボタンを無効に設定
            EnableFormButton(false);     
        }

        /// <summary>
        /// 再接続する
        /// </summary>
        /// <remarks>
        /// マイコンが再起動されるようなコマンドが成功した後に本関数を使用する
        /// </remarks>
        public void Reconnect()
        {
            // 他のフォームから本関数が呼ばれた時に、メインフォームが既に破棄されている場合は何もしない
            if (true == this.IsDisposed)
            {
                return;
            }

            // 切断する
            Disconnect();
            AppendAppLogText(false, "Try reconnecting...");
            // マイコンが再起動するのを待つ
            Thread.Sleep(REBOOT_WAIT);
            // 接続する
            Connect();
        }

        /// <summary>
        /// 子フォーム表示ボタンの有効/無効を設定
        /// </summary>
        void EnableFormButton(bool bEnable)
        {
            foreach (Button btn in _lstButton)
            {
                if  (btn == button_NwConfig)
                {
                    if (!((label_FwName.Text == Str.STR_FW_NAME_PICOJIG_WL) 
                        || (label_FwName.Text == Str.STR_FW_NAME_PICOBRG)
                        || (label_FwName.Text == Str.STR_FW_NAME_PICOSEN)
                        ))
                    {
                        btn.Enabled = false;
                        continue;
                    }
                }

                if (btn == button_Uart)
                {
                    if (label_FwName.Text == Str.STR_FW_NAME_PICOSEN)
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
                    if (!((label_FwName.Text == Str.STR_FW_NAME_PICOJIG_WL) || (label_FwName.Text == Str.STR_FW_NAME_PICOJIG)))
                    {
                        btn.Enabled = false;
                        continue;
                    }
                }

                btn.Enabled = bEnable;
            }
        }

        /// <summary>
        /// 「NwConfig」ボタンを押した時
        /// </summary>
        private void button_NwConfig_Click(object sender, EventArgs e)
        {
            // NwConfigフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 「GPIO」ボタンを押した時
        /// </summary>
        private void button_Gpio_Click(object sender, EventArgs e)
        {
            // GPIOフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 「ADC」ボタンを押した時
        /// </summary>
        private void button_Adc_Click(object sender, EventArgs e)
        {
            // ADCフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 「UART」ボタンを押した時
        /// </summary>
        private void button_Uart_Click(object sender, EventArgs e)
        {
            // UARTフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 「SPI」ボタンを押した時
        /// </summary>
        private void button_Spi_Click(object sender, EventArgs e)
        {
            // SPIフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 「I2C」ボタンを押したとき
        /// </summary>
        private void button_I2c_Click(object sender, EventArgs e)
        {
            // I2Cフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 「PWM」ボタンを押した時
        /// </summary>
        private void button_Pwm_Click(object sender, EventArgs e)
        {
            // PWMフォームを表示
            ShowChildForm((Button)sender);
        }

        /// <summary>
        /// 子フォーム表示ボタンに応じた子フォームを表示
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
                // 無処理
            }

            // 一時的に子フォームを最前面に表示
            frm.TopMost = true;
            frm.TopMost = false;
            // 子フォームが最小化されている時、元の状態に戻す
            frm.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 「設定データ消去」ボタンを押した時
        /// </summary>
        private async void button_EraseFlash_Click(object sender, EventArgs e)
        {
            string strErrMsg;

            // 確認メッセージを表示
            if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n[Note]\nIf you want to erase the setting data saved in the flash memory, press the \"Erase setting data in flash memory\" button on the main screen."))
            {
                return;
            }

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「FLASH消去」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_EraseFlash();

            });
            if (strErrMsg == null)
            {
                string strInfoMsg = "Setting changes are complete.\nThe microcontroller will be reset.\n\nPlease wait from a few seconds to several tens of seconds.";
                UI.ShowInfoMsg(this, strInfoMsg);
                // 再接続する
                Reconnect();
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
            this.Enabled = true;
        }

        /// <summary>
        /// 「Appログ」テキストボックスにログを追加する
        /// </summary>
        public void AppendAppLogText(bool bError, string strMsg)
        {
            string strLog;

            // 他のフォームから本関数が呼ばれた時に、メインフォームが既に破棄されている場合は何もしない
            if (true == this.IsDisposed)
            {
                return;
            }

            // 送信コマンドの応答待ちをキャンセルした時のメッセージを表示しないようにする
            if (strMsg == JigCmd.STR_MSG_WAIT_RES_CANCEL)
            {
                // 無処理
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
        /// 「Appエラークリア」ボタンを押した時
        /// </summary>
        private void button_ClearAppLog_Click(object sender, EventArgs e)
        {
            textBox_AppLog.Text = string.Empty;
        }

        /// <summary>
        /// 「FWエラークリア」ボタンを押した時
        /// </summary>
        private async void button_ClearFwErr_Click(object sender, EventArgs e)
        {
            string strErrMsg;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「FWエラークリア」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_ClearFwError();
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 無処理
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }
    }
}
