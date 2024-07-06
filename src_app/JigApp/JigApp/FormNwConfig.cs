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

namespace JigApp
{
    public partial class FormNwConfig : Form
    {
        /// <summary>
        /// フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormNwConfig()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// フォームのロード時
        /// </summary>
        private void FormNetwork_Load(object sender, EventArgs e)
        {
            // タイトル
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
            // 通信設定を取得
            GetConfig();
        }

        /// <summary>
        /// ネットワーク設定を取得
        /// </summary>
        private async void GetConfig()
        {
            string strCountryCode = null;
            string strIpAddr = null;
            string strSsid = null;
            string strPassword = null;
            string strErrMsg = null;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「ネットワーク設定取得」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_GetNwConfig(out strCountryCode, out strIpAddr, out strSsid, out strPassword);
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // ネットワーク設定の表示を更新
                textBox_CountryCode.Text = strCountryCode;
                textBox_IpAddr.Text = strIpAddr;
                textBox_SSID.Text = strSsid;
                textBox_Password.Text = strPassword;
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }

        /// <summary>
        /// 「設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
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
                //「ネットワーク設定設定変更」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_SetNwConfig(textBox_CountryCode.Text.Trim(), textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim());
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
    }
}
