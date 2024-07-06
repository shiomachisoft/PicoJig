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
using JigLib;

namespace JigApp
{
    public partial class FormUart : Form
    {
        private const byte UART_DATA_BITS = 8;
        private const string STR_NONE = "None";
        private const string STR_EVEN = "Even";
        private const string STR_ODD = "Odd";

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
        public FormUart()
        {
            InitializeComponent();
            _strTitle = this.Text;
            this.Text = _strTitle + " - " + "Monitor stopped";
        }

        /// <summary>
        /// フォームのロード時
        /// </summary>
        private void FormUart_Load(object sender, EventArgs e)
        {
            if ((Str.PrpFwName == Str.STR_FW_NAME_PICOBRG) 
                || (Str.PrpFwName == Str.STR_FW_NAME_PICOSENTCP)
                )
            {
                label_sendData.Visible = false;
                textBox_SendData.Visible = false;
                label_sendSize.Visible = false;
                button_Send.Visible = false;
                label_log.Visible = false;
                textBox_Log.Visible = false;
                button_Clear.Visible = false;
            }

            // 「ストップビット」コンボボックスにアイテムを追加
            comboBox_StopBits.Items.Add(1);
            comboBox_StopBits.Items.Add(2);
            comboBox_StopBits.SelectedIndex = 0;

            // 「パリティビット」コンボボックスにアイテムを追加
            comboBox_Parity.Items.Add(STR_NONE);
            comboBox_Parity.Items.Add(STR_EVEN);
            comboBox_Parity.Items.Add(STR_ODD);
            comboBox_Parity.SelectedIndex = 0;

            // 通信設定を取得
            GetConfig();

            // 通信ログマネージャに「送受信ログ」テキストボックスとUART受信データのキューを登録
            _logViwMng = new LogViewMng(textBox_Log, Program.PrpJigCmd.PrpUartRecvDataQue);
            // 通信ログマネージャの受信モニタの開始
            _logViwMng.StartMonitor();
        }

        /// <summary>
        /// フォームを閉じる時
        /// </summary>
        private void FormUart_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 通信ログマネージャの受信モニタの終了
            _logViwMng.EndMonitor();
        }

        /// <summary>
        /// タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (true == Program.PrpJigCmd.IsConnected())
            {
                string strErrMsg = Program.PrpJigCmd.GetLastRecvErrMsg(); // コマンド送信の関数ではない
                if (strErrMsg == null)
                {
                    this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName + " - " + "Monitoring"; ;
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
        /// 「通信設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            byte dataBits = 0;      // データビット
            byte stopBits = 0;      // ストップビット
            byte parity = 0;        // パリティ
            UInt32 baudrate = 0;    // ボーレート
            string strErrMsg;

            // 確認メッセージを表示
            if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n[Note]\nIf you want to erase the setting data saved in the flash memory, press the \"Erase setting data in flash memory\" button on the main screen."))
            {
                return;
            }

            // ボーレートを取得
            baudrate = (UInt32)numericUpDown_Baudrate.Value;
            // データビットを取得
            dataBits = UART_DATA_BITS;
            // ストップビットを取得
            stopBits = Convert.ToByte(comboBox_StopBits.SelectedItem.ToString());
            // パリティを取得
            parity = (Byte)comboBox_Parity.SelectedIndex;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「UART通信設定変更」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SetUartConfig(baudrate, dataBits, stopBits, parity);
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
            byte[] aSendData = null; // 送信データ
            string strErrMsg;
            string strText = textBox_SendData.Text.Trim(); // 「送信データ」テキストボックスの文字列

            // 「送信データ」テキストボックスが空の場合
            if (string.IsNullOrEmpty(strText))
            {
                strErrMsg = "No transmission data has been entered.";
                goto End;
            }

            // 「送信データ」テキストボックスの16進数文字列をbyte型の配列に変換
            strErrMsg = Str.ConvertHexStringToByteArray(strText,out aSendData);
            if (strErrMsg != null)
            {
                strErrMsg = "The input of the sending data is incorrect.\n\n" + strErrMsg;
                goto End;
            }

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「UART送信」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SendUart(aSendData);
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
            byte dataBits = 0;   // データビット 
            byte stopBits = 0;   // ストップビット
            byte parity = 0;     // パリティ   
            UInt32 baudrate = 0; // ボーレート
            string strErrMsg;
            string strParity;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「UART通信設定取得」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_GetUartConfig(out baudrate, out dataBits, out stopBits, out parity);
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 通信設定の表示を更新
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
    }
}
