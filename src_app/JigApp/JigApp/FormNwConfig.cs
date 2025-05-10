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
        /// ネットワーク設定変更/取得コマンドのバージョン
        /// </summary>
        enum E_NW_CONFIG : int
        {
            NW_CONFIG,
            NW_CONFIG2,
            NW_CONFIG3
        }

        /// <summary>
        /// ネットワーク設定変更/取得コマンドのバージョン
        /// </summary>
        private E_NW_CONFIG _eNwConfig = E_NW_CONFIG.NW_CONFIG;

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
        private void FormNwConfig_Load(object sender, EventArgs e)
        {
            // タイトル
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;

            if (!((Str.PrpFwName == Str.STR_FW_NAME_PICOBRG) || (Str.PrpFwName == Str.STR_FW_NAME_PICOIOT)))
            {
                radioButton_BLE.Visible = false;
                radioButton_Wifi.Visible = false;
            }

            if (Str.PrpFwName == Str.STR_FW_NAME_PICOBRG)
            {
                _eNwConfig = E_NW_CONFIG.NW_CONFIG2;
                groupBox_EMail.Visible = false;
            }
            else if (Str.PrpFwName == Str.STR_FW_NAME_PICOIOT)
            {
                _eNwConfig = E_NW_CONFIG.NW_CONFIG3;
            }
            else
            {
                groupBox_TcpSocketCom.Visible = false;
                groupBox_EMail.Visible = false;
            }

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
            string strServerIpAddr = null;
            bool isClient = false;
            bool isWifi = false;
            string strGMailAddress = null;
            string strGMailAppPassword = null;
            string strToEMailAddress = null;
            byte mailIntervalHour = 1;
            string strErrMsg = null;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                switch (_eNwConfig)
                {
                    case E_NW_CONFIG.NW_CONFIG2:
                        //「ネットワーク設定取得2」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetNwConfig2(out isWifi, out strCountryCode, out strIpAddr, out strSsid, out strPassword, out strServerIpAddr, out isClient);
                    case E_NW_CONFIG.NW_CONFIG3:
                        //「ネットワーク設定取得3」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetNwConfig3(out isWifi, out strCountryCode, out strIpAddr, out strSsid, out strPassword, out strServerIpAddr, out isClient, out strGMailAddress, out strGMailAppPassword, out strToEMailAddress, out mailIntervalHour);
                    default:
                        //「ネットワーク設定取得」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetNwConfig(out strCountryCode, out strIpAddr, out strSsid, out strPassword);
                }
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // ネットワーク設定の表示を更新
                textBox_IpAddr.Text = strIpAddr;
                textBox_SSID.Text = strSsid;
                textBox_Password.Text = strPassword;
                if ((_eNwConfig == E_NW_CONFIG.NW_CONFIG2) || (_eNwConfig == E_NW_CONFIG.NW_CONFIG3))
                {
                    radioButton_Wifi.Checked = isWifi;
                    radioButton_Server.Checked = !isClient;
                    radioButton_Client.Checked = isClient;
                    textBox_ServerIpAddr.Enabled = isClient;
                    textBox_ServerIpAddr.Text = strServerIpAddr;
                }
                if (_eNwConfig == E_NW_CONFIG.NW_CONFIG3)
                {
                    textBox_GMailAddress.Text = strGMailAddress;
                    textBox_GMailAppPassword.Text = strGMailAppPassword;
                    textBox_ToEMailAddress.Text = strToEMailAddress;
                    numericUpDown_MailIntervalHour.Value = mailIntervalHour;
                }
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }

        /// <summary>
        /// 「設定の変更」ボタンを押した時
        /// </summary>
        private void button_SetConfig_Click(object sender, EventArgs e)
        {
            string strErrMsg;

            // 確認メッセージを表示
            if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n(The microcontroller will be reset.)"))
            {
                return;
            }

            string strCountryCode = "JP"; // カントリーコードはマイコンに送信するが、マイコン側ではカントリーコードは未使用。

            if (_eNwConfig == E_NW_CONFIG.NW_CONFIG2)
            {
                //「ネットワーク設定設定変更2」コマンドの要求を送信
                strErrMsg = Program.PrpJigCmd.SendCmd_SetNwConfig2(radioButton_Wifi.Checked, strCountryCode, textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim(), textBox_ServerIpAddr.Text.Trim(), radioButton_Client.Checked);
            }
            else if (_eNwConfig == E_NW_CONFIG.NW_CONFIG3)
            {
                //「ネットワーク設定設定変更3」コマンドの要求を送信                                                                                                                                                                                                                  
                strErrMsg = Program.PrpJigCmd.SendCmd_SetNwConfig3(radioButton_Wifi.Checked, strCountryCode, textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim(), textBox_ServerIpAddr.Text.Trim(), radioButton_Client.Checked, textBox_GMailAddress.Text.Trim(), textBox_GMailAppPassword.Text.Trim(), textBox_ToEMailAddress.Text.Trim(), (byte)numericUpDown_MailIntervalHour.Value);
            }
            else
            {
                //「ネットワーク設定設定変更」コマンドの要求を送信
                strErrMsg = Program.PrpJigCmd.SendCmd_SetNwConfig(strCountryCode, textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim());
            }
            
            if (strErrMsg == null)
            {
                // 再接続する
                FormMain.Inst.Reconnect();
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }

        /// <summary>
        /// 「サーバー」ラジオボタンのチェック状態が変化した時
        /// </summary>
        private void radioButton_Server_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            textBox_ServerIpAddr.Enabled = !radio.Checked;
        }
    }
}
