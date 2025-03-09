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
        private void FormNetwork_Load(object sender, EventArgs e)
        {
            // タイトル
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;

            if (Str.PrpFwName == Str.STR_FW_NAME_PICOBRG)
            {
                _eNwConfig = E_NW_CONFIG.NW_CONFIG2;
                groupBox_EMail.Visible = false;
            }
            else if (Str.PrpFwName == Str.STR_FW_NAME_PICOIOT)
            {
                _eNwConfig = E_NW_CONFIG.NW_CONFIG3;
                label_CountryCode.Visible = false;
                textBox_CountryCode.Visible = false;
                label_CountryCode_Eg.Visible = false;
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
                        return Program.PrpJigCmd.SendCmd_GetNwConfig2(out strCountryCode, out strIpAddr, out strSsid, out strPassword, out strServerIpAddr, out isClient);
                    case E_NW_CONFIG.NW_CONFIG3:
                        //「ネットワーク設定取得3」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetNwConfig3(out strCountryCode, out strIpAddr, out strSsid, out strPassword, out strServerIpAddr, out isClient, out strGMailAddress, out strGMailAppPassword, out strToEMailAddress, out mailIntervalHour);
                    default:
                        //「ネットワーク設定取得」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetNwConfig(out strCountryCode, out strIpAddr, out strSsid, out strPassword);
                }
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // ネットワーク設定の表示を更新
                textBox_CountryCode.Text = strCountryCode;
                textBox_IpAddr.Text = strIpAddr;
                textBox_SSID.Text = strSsid;
                textBox_Password.Text = strPassword;
                if ((_eNwConfig == E_NW_CONFIG.NW_CONFIG2) || (_eNwConfig == E_NW_CONFIG.NW_CONFIG3))
                {
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
                if (_eNwConfig == E_NW_CONFIG.NW_CONFIG2)
                {
                    //「ネットワーク設定設定変更2」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_SetNwConfig2(textBox_CountryCode.Text.Trim(), textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim(), textBox_ServerIpAddr.Text.Trim(), radioButton_Client.Checked);
                }
                else if (_eNwConfig == E_NW_CONFIG.NW_CONFIG3)
                {
                    //「ネットワーク設定設定変更3」コマンドの要求を送信                                                                                                                                                                                                                  
                    return Program.PrpJigCmd.SendCmd_SetNwConfig3(textBox_CountryCode.Text.Trim(), textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim(), textBox_ServerIpAddr.Text.Trim(), radioButton_Client.Checked, textBox_GMailAddress.Text.Trim(), textBox_GMailAppPassword.Text.Trim(), textBox_ToEMailAddress.Text.Trim(), (byte)numericUpDown_MailIntervalHour.Value);
                }
                else
                {
                    //「ネットワーク設定設定変更」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_SetNwConfig(textBox_CountryCode.Text.Trim(), textBox_IpAddr.Text.Trim(), textBox_SSID.Text.Trim(), textBox_Password.Text.Trim());
                }
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
        /// 「サーバー」ラジオボタンのチェック状態が変化した時
        /// </summary>
        private void radioButton_Server_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            textBox_ServerIpAddr.Enabled = !radio.Checked;
        }
    }
}
