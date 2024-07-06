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
    public partial class FormSpi : Form
    {
        private const byte SPI_DATA_BITS = 8;
        private const byte SPI_MSB_FIRST = 1;
        private const string STR_MODE_0 = "MODE0(CPOL=0,CPHA=0)";
        private const string STR_MODE_1 = "MODE1(CPOL=0,CPHA=1)";
        private const string STR_MODE_2 = "MODE2(CPOL=1,CPHA=0)";
        private const string STR_MODE_3 = "MODE3(CPOL=1,CPHA=1)";

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
        public FormSpi()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// フォームのロード時
        /// </summary>
        private void FormSpi_Load(object sender, EventArgs e)
        {
            // タイトルを表示
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
            // 「モード」コンボボックスにアイテムを追加
            comboBox_Mode.Items.Add(STR_MODE_0);
            comboBox_Mode.Items.Add(STR_MODE_1);
            comboBox_Mode.Items.Add(STR_MODE_2);
            comboBox_Mode.Items.Add(STR_MODE_3);
            comboBox_Mode.SelectedIndex = 0;
            // 通信設定を取得
            GetConfig();
            // 通信ログマネージャに「送受信ログ」テキストボックスを登録
            _logViwMng = new LogViewMng(textBox_Log);
        }

        /// <summary>
        /// 「通信設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            byte dataBits;      // データビット
            byte polarity = 0;  // 極性　
            byte phase= 0;      // 位相
            byte order;         // バイトオーダー
            UInt32 freq;        // 周波数 
            string strErrMsg;

            // 確認メッセージを表示
            if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n[Note]\nIf you want to erase the setting data saved in the flash memory, press the \"Erase setting data in flash memory\" button on the main screen."))
            {
                return;
            }

            // 周波数を取得
            freq = (UInt32)numericUpDown_Freq.Value;
            // データビットを取得
            dataBits = SPI_DATA_BITS;
            // 極性と位相を取得
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
            // バイトオーダーを取得
            order = SPI_MSB_FIRST;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「SPI通信設定変更」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SetSpiConfig(freq, dataBits, polarity, phase, order);
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
            byte[] aRecvData = null; // 受信データ
            string strErrMsg;
            string strText = textBox_SendData.Text.Trim(); // 「送信データ」テキストボックスの文字列

            // 「送信データ」テキストボックスが空の場合
            if (string.IsNullOrEmpty(strText))
            {
                strErrMsg = "No transmission data has been entered.";
                goto End;
            }

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
                //「SPI送信」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SendSpi(aSendData, out aRecvData);
            });
            this.Enabled = true;

        End:
            if (strErrMsg == null)
            {
                // 送信データを通信ログに追加
                _logViwMng.Add(false, aSendData);
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
            byte dataBits = 0; // データビット
            byte polarity = 0; // 極性
            byte phase = 0;    // 位相
            byte order = 0;    // バイトオーダー
            UInt32 freq = 0;   // 周波数
            string strErrMsg = null;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「SPI通信設定取得」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_GetSpiConfig(out freq, out dataBits, out polarity, out phase, out order);
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 通信設定の表示を更新
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
    }
}
