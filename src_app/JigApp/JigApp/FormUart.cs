﻿// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormUart : Form
    {
        private const byte UART_DATA_BITS = 8;
        private const string STR_NONE = "None";
        private const string STR_EVEN = "Even";
        private const string STR_ODD = "Odd";

        /// <summary>
        /// Form title / フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// Communication log manager / 通信ログマネージャ
        /// </summary>
        private LogViewMng _logViwMng = null;

        /// <summary>
        /// Constructor / コンストラクタ 
        /// </summary>
        public FormUart()
        {
            InitializeComponent();
            _strTitle = this.Text;
            this.Text = _strTitle + " - " + "Monitor stopped";
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormUart_Load(object sender, EventArgs e)
        {
            if (Str.PrpFwName == Str.STR_FW_NAME_PICOBRG)
            {
                label_sendData.Visible = false;
                textBox_SendData.Visible = false;
                label_sendSize.Visible = false;
                button_Send.Visible = false;
                label_log.Visible = false;
                textBox_Log.Visible = false;
                button_Clear.Visible = false;
            }

            // Add items to "Stop Bits" combo box / 「ストップビット」コンボボックスにアイテムを追加
            comboBox_StopBits.Items.Add(1);
            comboBox_StopBits.Items.Add(2);
            comboBox_StopBits.SelectedIndex = 0;

            // Add items to "Parity Bit" combo box / 「パリティビット」コンボボックスにアイテムを追加
            comboBox_Parity.Items.Add(STR_NONE);
            comboBox_Parity.Items.Add(STR_EVEN);
            comboBox_Parity.Items.Add(STR_ODD);
            comboBox_Parity.SelectedIndex = 0;

            // Get communication settings / 通信設定を取得
            GetConfig();

            // Register "Send/Receive Log" text box and UART receive data queue to communication log manager / 通信ログマネージャに「送受信ログ」テキストボックスとUART受信データのキューを登録
            _logViwMng = new LogViewMng(textBox_Log, Program.PrpJigCmd.PrpUartRecvDataQue);
            // Start receive monitor of communication log manager / 通信ログマネージャの受信モニタの開始
            _logViwMng.StartMonitor();
        }

        /// <summary>
        /// When closing the form / フォームを閉じる時
        /// </summary>
        private void FormUart_FormClosing(object sender, FormClosingEventArgs e)
        {
            // End receive monitor of communication log manager / 通信ログマネージャの受信モニタの終了
            _logViwMng.EndMonitor();
        }

        /// <summary>
        /// Timer callback / タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (true == Program.PrpJigCmd.IsConnected())
            {
                string strErrMsg = Program.PrpJigCmd.GetLastRecvErrMsg(); // Not a function to send commands / コマンド送信の関数ではない
                if (strErrMsg == null)
                {
                    this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName + " - " + "Monitoring";
                }
                else
                {
                    this.Text = _strTitle + " - " + "Monitor stopped";
                    FormMain.Inst.AppendAppLogText(true, strErrMsg);
                }
            }
            else
            {
                this.Text = _strTitle + " - " + "Monitor stopped";
            }
        }

        /// <summary>
        /// When the "Change Communication Settings" button is pressed / 「通信設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            try
            {
                byte dataBits = 0;      // Data bits / データビット
                byte stopBits = 0;      // Stop bits / ストップビット
                byte parity = 0;        // Parity / パリティ
                UInt32 baudrate = 0;    // Baud rate / ボーレート
                string strErrMsg;

                // Display confirmation message / 確認メッセージを表示
                if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n(The microcontroller will be reset.)"))
                {
                    return;
                }

                // Get baud rate / ボーレートを取得
                baudrate = (UInt32)numericUpDown_Baudrate.Value;
                // Get data bits / データビットを取得
                dataBits = UART_DATA_BITS;
                // Get stop bits / ストップビットを取得
                stopBits = Convert.ToByte(comboBox_StopBits.SelectedItem.ToString());
                // Get parity / パリティを取得
                parity = (Byte)comboBox_Parity.SelectedIndex;

                // Send request for "Set UART Config" command / 「UART通信設定変更」コマンドの要求を送信
                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    return Program.PrpJigCmd.SendCmd_SetUartConfig(baudrate, dataBits, stopBits, parity);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Reconnect / 再接続する
                    await FormMain.Inst.Reconnect();
                }
                else
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Set Config Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }

        /// <summary>
        /// When the "Send" button is pressed / 「送信」ボタンを押した時
        /// </summary>
        private async void button_Send_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] aSendData = null; // Send data / 送信データ
                string strErrMsg;
                string strText = textBox_SendData.Text.Trim(); // String in "Send Data" text box / 「送信データ」テキストボックスの文字列

                // If "Send Data" text box is empty / 「送信データ」テキストボックスが空の場合
                if (string.IsNullOrEmpty(strText))
                {
                    strErrMsg = "No transmission data has been entered.";
                    goto End;
                }

                // Convert hex string in "Send Data" text box to byte array / 「送信データ」テキストボックスの16進数文字列をbyte型の配列に変換
                strErrMsg = Str.ConvertHexStringToByteArray(strText, out aSendData);
                if (strErrMsg != null)
                {
                    strErrMsg = "The input transmission data is incorrect.\n\n" + strErrMsg;
                    goto End;
                }

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "UART Send" command / 「UART送信」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_SendUart(aSendData);
                });
                if (this.IsDisposed) return;
                
            End:
                if (strErrMsg == null)
                {
                    // Add send data to communication log / 送信データを通信ログに追加
                    _logViwMng.Add(false, aSendData);
                }
                else
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Send Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }

        /// <summary>
        /// When the "Clear" button is pressed / 「クリア」ボタンを押した時
        /// </summary>
        private void button_Clear_Click(object sender, EventArgs e)
        {
            textBox_Log.Text = string.Empty;
        }

        /// <summary>
        /// Get communication settings / 通信設定を取得
        /// </summary>
        private async void GetConfig()
        {
            try
            {
                byte dataBits = 0;   // Data bits / データビット 
                byte stopBits = 0;   // Stop bits / ストップビット
                byte parity = 0;     // Parity / パリティ   
                UInt32 baudrate = 0; // Baud rate / ボーレート
                string strErrMsg;
                string strParity;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Get UART Config" command / 「UART通信設定取得」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_GetUartConfig(out baudrate, out dataBits, out stopBits, out parity);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Update communication settings display / 通信設定の表示を更新
                    numericUpDown_Baudrate.Value = baudrate;
                    UI.SelectComboBoxItem(comboBox_StopBits, stopBits.ToString());
                    switch (parity)
                    {
                        case 1:
                            strParity = STR_EVEN;
                            break;
                        case 2:
                            strParity = STR_ODD;
                            break;
                        default:
                            strParity = STR_NONE;
                            break;
                    }
                    UI.SelectComboBoxItem(comboBox_Parity, strParity);
                }
                else
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"GetConfig Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }
    }
}
