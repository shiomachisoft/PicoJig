﻿// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormSpi : Form
    {
        private const byte SPI_DATA_BITS = 8;
        private const byte SPI_MSB_FIRST = 1;
        private const string STR_MODE_0 = "MODE0(CPOL=0,CPHA=0)";
        private const string STR_MODE_1 = "MODE1(CPOL=0,CPHA=1)";
        private const string STR_MODE_2 = "MODE2(CPOL=1,CPHA=0)";
        private const string STR_MODE_3 = "MODE3(CPOL=1,CPHA=1)";

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
        public FormSpi()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormSpi_Load(object sender, EventArgs e)
        {
            // Display title / タイトルを表示
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
            // Add items to "Mode" combo box / 「モード」コンボボックスにアイテムを追加
            comboBox_Mode.Items.Add(STR_MODE_0);
            comboBox_Mode.Items.Add(STR_MODE_1);
            comboBox_Mode.Items.Add(STR_MODE_2);
            comboBox_Mode.Items.Add(STR_MODE_3);
            comboBox_Mode.SelectedIndex = 0;
            // Get communication settings / 通信設定を取得
            GetConfig();
            // Register "Send/Receive Log" text box to communication log manager / 通信ログマネージャに「送受信ログ」テキストボックスを登録
            _logViwMng = new LogViewMng(textBox_Log);
        }

        /// <summary>
        /// When the "Change Communication Settings" button is pressed / 「通信設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            try
            {
                byte dataBits;      // Data bits / データビット
                byte polarity = 0;  // Polarity / 極性
                byte phase = 0;     // Phase / 位相
                byte order;         // Bit order / ビットオーダー
                UInt32 freq;        // Frequency / 周波数 
                string strErrMsg;

                // Display confirmation message / 確認メッセージを表示
                if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n(The microcontroller will be reset.)"))
                {
                    return;
                }

                // Get frequency / 周波数を取得
                freq = (UInt32)numericUpDown_Freq.Value;
                // Get data bits / データビットを取得
                dataBits = SPI_DATA_BITS;
                // Get polarity and phase / 極性と位相を取得
                switch (comboBox_Mode.SelectedIndex)
                {
                    case 0:
                        polarity = 0;
                        phase = 0;
                        break;
                    case 1:
                        polarity = 0;
                        phase = 1;
                        break;
                    case 2:
                        polarity = 1;
                        phase = 0;
                        break;
                    case 3:
                        polarity = 1;
                        phase = 1;
                        break;
                    default:
                        break;
                }
                // Get bit order / ビットオーダーを取得
                order = SPI_MSB_FIRST;

                // Send request for "Set SPI Config" command / 「SPI通信設定変更」コマンドの要求を送信
                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    return Program.PrpJigCmd.SendCmd_SetSpiConfig(freq, dataBits, polarity, phase, order);
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
                byte[] aRecvData = null; // Receive data / 受信データ
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
                    // Send request for "SPI Send" command / 「SPI送信」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_SendSpi(aSendData, out aRecvData);
                });
                if (this.IsDisposed) return;
                
            End:
                if (strErrMsg == null)
                {
                    // Add send data to communication log / 送信データを通信ログに追加
                    _logViwMng.Add(false, aSendData);
                    // Add receive data to communication log / 受信データを通信ログに追加
                    _logViwMng.Add(true, aRecvData);
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
                byte dataBits = 0; // Data bits / データビット
                byte polarity = 0; // Polarity / 極性
                byte phase = 0;    // Phase / 位相
                byte order = 0;    // Bit order / ビットオーダー
                UInt32 freq = 0;   // Frequency / 周波数
                string strErrMsg = null;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Get SPI Config" command / 「SPI通信設定取得」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_GetSpiConfig(out freq, out dataBits, out polarity, out phase, out order);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Update communication settings display / 通信設定の表示を更新
                    numericUpDown_Freq.Value = freq;
                    if (polarity == 0 && phase == 1)
                    {
                        UI.SelectComboBoxItem(comboBox_Mode, STR_MODE_1);
                    }
                    else if (polarity == 1 && phase == 0)
                    {
                        UI.SelectComboBoxItem(comboBox_Mode, STR_MODE_2);
                    }
                    else if (polarity == 1 && phase == 1)
                    {
                        UI.SelectComboBoxItem(comboBox_Mode, STR_MODE_3);
                    }
                    else
                    {
                        UI.SelectComboBoxItem(comboBox_Mode, STR_MODE_0);
                    }
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
