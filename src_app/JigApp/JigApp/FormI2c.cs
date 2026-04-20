﻿// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormI2c : Form
    {
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
        public FormI2c()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormI2c_Load(object sender, EventArgs e)
        {
            // Title / タイトル
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
            // Get communication settings / 通信設定を取得
            GetConfig();
            // Register "Send/Receive Log" text box to communication log manager / 通信ログマネージャに「送受信ログ」テキストボックスを登録
            _logViwMng = new LogViewMng(textBox_Log);
        }

        /// <summary>
        /// When closing the form / フォームを閉じる時
        /// </summary>
        private void FormI2c_FormClosing(object sender, FormClosingEventArgs e)
        {
            // End receive monitor of communication log manager / 通信ログマネージャの受信モニタの終了
            _logViwMng.EndMonitor();
        }

        /// <summary>
        /// When the "Change Communication Settings" button is pressed / 「通信設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            try
            {
                UInt32 freq;// Frequency / 周波数
                string strErrMsg;

                // Display confirmation message / 確認メッセージを表示
                if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n(The microcontroller will be reset.)"))
                {
                    return;
                }

                // Get frequency / 周波数を取得
                freq = (UInt32)numericUpDown_Freq.Value;

                // Send request for "Set I2C Config" command / 「I2C通信設定変更」コマンドの要求を送信
                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    return Program.PrpJigCmd.SendCmd_SetI2cConfig(freq);
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
                byte slaveAddr;          // Slave address / スレーブアドレス
                byte[] aSendData = null; // Send data / 送信データ
                string strErrMsg;
                string strText = textBox_SendData.Text.Trim(); // String in "Send Data" text box / 「送信データ」テキストボックスの文字列

                // If "Send Data" text box is empty / 「送信データ」テキストボックスが空の場合
                if (string.IsNullOrEmpty(strText))
                {
                    strErrMsg = "No transmission data has been entered.";
                    goto End;
                }

                // Convert hex string in "Slave Address" text box to value / 「スレーブアドレス」テキストボックスの文字列を16進数に変換
                slaveAddr = (byte)numericUpDown_SlaveAddr.Value;

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
                    // Send request for "I2C Send" command / 「I2C送信」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_SendI2c(slaveAddr, aSendData);
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
        /// When the "Receive" button is pressed / 「受信」ボタンを押した時
        /// </summary>
        private async void button_Recv_Click(object sender, EventArgs e)
        {
            try
            {
                byte slaveAddr;          // Slave address / スレーブアドレス
                byte[] aRecvData = null; // Receive data / 受信データ
                UInt16 recvSize;         // Receive size / 受信サイズ
                string strErrMsg;

                // Convert hex string in "Slave Address" text box to value / 「スレーブアドレス」テキストボックスの16進数文字列を値に変換
                slaveAddr = (byte)numericUpDown_SlaveAddr.Value;
                // Get receive size / 受信サイズを取得
                recvSize = (UInt16)numericUpDown_RecvSize.Value;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "I2C Receive" command / 「I2C受信」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_RecvI2c(slaveAddr, recvSize, out aRecvData);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
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
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Receive Error: {ex.Message}");
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
                UInt32 freq = 0; // Frequency / 周波数
                string strErrMsg = null;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Get I2C Config" command / 「I2C通信設定取得」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_GetI2cConfig(out freq);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Update communication settings display / 通信設定の表示を更新
                    numericUpDown_Freq.Value = freq;
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
