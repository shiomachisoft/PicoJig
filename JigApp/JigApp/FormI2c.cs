// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JigLib;

namespace JigApp
{
    public partial class FormI2c : Form
    {
        /// <summary>
        /// フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// 通信ログマネージャ
        /// </summary>
        private LogViewMng _logViwMng = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormI2c()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// フォームのロード時
        /// </summary>
        private void FormI2c_Load(object sender, EventArgs e)
        {
            // タイトル
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
            // 通信設定を取得
            GetConfig();
            // 通信ログマネージャに「送受信ログ」テキストボックスを登録
            _logViwMng = new LogViewMng(textBox_Log);
        }

        /// <summary>
        /// フォームを閉じる時
        /// </summary>
        private void FormI2c_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 通信ログマネージャの受信モニタの終了
            _logViwMng.EndMonitor();
        }

        /// <summary>
        /// 「通信設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            UInt32 freq;// 周波数
            string strErrMsg;

            // 確認メッセージを表示
            if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n[Note]\nIf you want to erase the setting data saved in the flash memory, press the \"Erase setting data in flash memory\" button on the main screen."))
            {
                return;
            }

            // 周波数を取得
            freq = (UInt32)numericUpDown_Freq.Value;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「I2C通信設定変更」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SetI2cConfig(freq);
            });
            if (strErrMsg == null)
            {
                string strInfoMsg = "Setting changes are complete.\nThe microcontroller will be reset.\n\nPlease wait from a few seconds to several tens of seconds.";
                UI.ShowInfoMsg(this, strInfoMsg);
                // 再接続する
                FormMain.Inst.Reconnect();
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
            this.Enabled = true;
        }

        /// <summary>
        /// 「送信」ボタンを押した時
        /// </summary>
        private async void button_Send_Click(object sender, EventArgs e)
        {
            byte slaveAddr;          // スレーブアドレス
            byte[] aSendData = null; // 送信データ
            string strErrMsg;
            string strText = textBox_SendData.Text.Trim(); // 「送信データ」テキストボックスの文字列

            // 「送信データ」テキストボックスが空の場合
            if (string.IsNullOrEmpty(strText))
            {
                strErrMsg = "No transmission data has been entered.";
                goto End;
            }

            // 「スレーブアドレス」テキストボックスの文字列を16進数に変換
            slaveAddr = (byte)numericUpDown_SlaveAddr.Value;

            // 「送信データ」テキストボックスの16進数文字列をbyte型の配列に変換
            strErrMsg = Str.ConvertHexStringToByteArray(strText, out aSendData);
            if (strErrMsg != null)
            {
                strErrMsg = "The input of the sending data is incorrect.\n\n" + strErrMsg;
                goto End;
            }

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「I2C送信」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SendI2c(slaveAddr, aSendData);
            });
            this.Enabled = true;

        End:
            if (strErrMsg == null)
            {
                // 送信データを通信ログに追加
                _logViwMng.Add(false, aSendData);
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }

        /// <summary>
        /// 「受信」ボタンを押した時
        /// </summary>
        private async void button_Recv_Click(object sender, EventArgs e)
        {
            byte slaveAddr;          // スレーブアドレス
            byte[] aRecvData = null; // 受信データ
            UInt16 recvSize;         // 受信サイズ
            string strErrMsg;

            // 「スレーブアドレス」テキストボックスの16進数文字列を値に変換
            slaveAddr = (byte)numericUpDown_SlaveAddr.Value;
            // 受信サイズを取得
            recvSize = (UInt16)numericUpDown_RecvSize.Value;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「I2C受信」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_RecvI2c(slaveAddr, recvSize, out aRecvData);
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 受信データを通信ログに追加
                _logViwMng.Add(true, aRecvData);
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }

        /// <summary>
        /// 「クリア」ボタンを押した時
        /// </summary>
        private void button_Clear_Click(object sender, EventArgs e)
        {
            textBox_Log.Text = string.Empty;
        }

        /// <summary>
        /// 通信設定を取得
        /// </summary>
        private async void GetConfig()
        {
            UInt32 freq = 0; // 周波数
            string strErrMsg = null;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「I2C通信設定取得」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_GetI2cConfig(out freq);
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 通信設定の表示を更新
                numericUpDown_Freq.Value = freq;
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }
    }
}
