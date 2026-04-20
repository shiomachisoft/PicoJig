﻿// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormNwConfig : Form
    {
        /// <summary>
        /// Form title / フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// Version of network configuration change/get command / ネットワーク設定変更/取得コマンドのバージョン
        /// </summary>
        enum E_NW_CONFIG : int
        {
            NW_CONFIG,
            NW_CONFIG2,
            NW_CONFIG3
        }

        /// <summary>
        /// Version of network configuration change/get command / ネットワーク設定変更/取得コマンドのバージョン
        /// </summary>
        private E_NW_CONFIG _eNwConfig = E_NW_CONFIG.NW_CONFIG;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public FormNwConfig()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormNwConfig_Load(object sender, EventArgs e)
        {
            // Title / タイトル
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
                label_Name.Visible = false;
                textBox_Name.Visible = false;
            }
            else if (Str.PrpFwName == Str.STR_FW_NAME_PICOIOT)
            {
                _eNwConfig = E_NW_CONFIG.NW_CONFIG3;
            }
            else
            {
                groupBox_TcpSocketCom.Visible = false;
                groupBox_EMail.Visible = false;
                label_Name.Visible = false;
                textBox_Name.Visible = false;
            }

            // Get communication settings / 通信設定を取得
            GetConfig();
        }

        /// <summary>
        /// Get network settings / ネットワーク設定を取得
        /// </summary>
        private async void GetConfig()
        {
            try
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
                string strName = null;
                string strErrMsg = null;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    switch (_eNwConfig)
                    {
                        case E_NW_CONFIG.NW_CONFIG2:
                            // Send request for "Get Network Config 2" command / 「ネットワーク設定取得2」コマンドの要求を送信
                            return Program.PrpJigCmd.SendCmd_GetNwConfig2(out isWifi, out strCountryCode, out strIpAddr, out strSsid, out strPassword, out strServerIpAddr, out isClient);
                        case E_NW_CONFIG.NW_CONFIG3:
                            // Send request for "Get Network Config 3" command / 「ネットワーク設定取得3」コマンドの要求を送信
                            return Program.PrpJigCmd.SendCmd_GetNwConfig3(out isWifi, out strCountryCode, out strIpAddr, out strSsid, out strPassword, out strServerIpAddr, out isClient, out strGMailAddress, out strGMailAppPassword, out strToEMailAddress, out mailIntervalHour, out strName);
                        default:
                            // Send request for "Get Network Config" command / 「ネットワーク設定取得」コマンドの要求を送信
                            return Program.PrpJigCmd.SendCmd_GetNwConfig(out strCountryCode, out strIpAddr, out strSsid, out strPassword);
                    }
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Update network settings display / ネットワーク設定の表示を更新
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
                        textBox_Name.Text = strName;
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

        /// <summary>
        /// When the "Change Settings" button is pressed / 「設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            try
            {
                string strErrMsg;
                string strName = textBox_Name.Text.Trim();
                string strCountryCode = "XX"; // Country code is sent to the microcontroller, but it is not used on the microcontroller side. / カントリーコードはマイコンに送信するが、マイコン側ではカントリーコードは未使用。

                // Display confirmation message / 確認メッセージを表示
                if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n(The microcontroller will be reset.)"))
                {
                    return;
                }

                // Get values of UI controls (Must be executed on UI thread) / UIコントロールの値を取得(UIスレッドで実行する必要があるため)
                bool isWifi = radioButton_Wifi.Checked;
                string strIpAddr = textBox_IpAddr.Text.Trim();
                string strSsid = textBox_SSID.Text.Trim();
                string strPassword = textBox_Password.Text.Trim();
                string strServerIpAddr = textBox_ServerIpAddr.Text.Trim();
                bool isClient = radioButton_Client.Checked;
                string strGMailAddress = textBox_GMailAddress.Text.Trim();
                string strGMailAppPassword = textBox_GMailAppPassword.Text.Trim();
                string strToEMailAddress = textBox_ToEMailAddress.Text.Trim();
                byte mailIntervalHour = (byte)numericUpDown_MailIntervalHour.Value;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    if (_eNwConfig == E_NW_CONFIG.NW_CONFIG2)
                    {
                        // Send request for "Set Network Config 2" command / 「ネットワーク設定変更2」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_SetNwConfig2(isWifi, strCountryCode, strIpAddr, strSsid, strPassword, strServerIpAddr, isClient);
                    }
                    else if (_eNwConfig == E_NW_CONFIG.NW_CONFIG3)
                    {
                        // Send request for "Set Network Config 3" command / 「ネットワーク設定変更3」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_SetNwConfig3(isWifi, strCountryCode, strIpAddr, strSsid, strPassword, strServerIpAddr, isClient, strGMailAddress, strGMailAppPassword, strToEMailAddress, mailIntervalHour, strName);
                    }
                    else
                    {
                        // Send request for "Set Network Config" command / 「ネットワーク設定変更」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_SetNwConfig(strCountryCode, strIpAddr, strSsid, strPassword);
                    }
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
        /// When the check state of the "Server" radio button changes / 「サーバー」ラジオボタンのチェック状態が変化した時
        /// </summary>
        private void radioButton_Server_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            textBox_ServerIpAddr.Enabled = !radio.Checked;
        }

        /// <summary>
        /// Allow only half-width characters when key is pressed in text box / テキストボックスがキープレスされた時に半角のみ許可
        /// </summary>
        private void textBox_HalfWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            UI.TextBox_HalfWidth_KeyPress(sender, e);
        }
    }
}
