﻿// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormGpio : Form
    {
        private const string STR_HIGH = "High";
        private const string STR_LOW = "Low";
        private const string STR_PULL_UP = "Up";
        private const string STR_PULL_DOWN = "Down";

        /// <summary>
        /// Form title / フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// Whether the monitor has never been successful yet / モニタはまだ一度も成功していないか否か
        /// </summary>
        private bool _isMonitorStillNotSuccessful = true;

        /// <summary>
        /// Input GPIO info structure / 入力GPIO構造体
        /// </summary>
        private struct ST_IN_GPIO_INFO
        {
            /// <summary>
            /// GP number / GP番号
            /// </summary>
            public byte gp;
            /// <summary>
            /// "Pull-Up/Pull-Down" checkbox / 「プルアップ/プルダウン」チェックボックス
            /// </summary>
            public CheckBox checkBox_pull;
            /// <summary>
            /// Current input value label / 現在入力値のラベル
            /// </summary>
            public Label label_inputVal;
        }

        /// <summary>
        /// Output GPIO info structure / 出力GPIO構造体
        /// </summary>
        private struct ST_OUT_GPIO_INFO
        {
            /// <summary>
            /// GP number / GP番号
            /// </summary>
            public byte gp;
            /// <summary>
            /// Initial output value checkbox / 初期出力値のチェックボックス
            /// </summary>
            public CheckBox checkBox_initialVal;
            /// <summary>
            /// Current output value label / 現在出力値のラベル
            /// </summary>
            public Label label_outputVal;
            /// <summary>
            /// Output change value checkbox / 出力変更値のチェックボックス
            /// </summary>
            public CheckBox checkBox_changeVal;
        }

        /// <summary>
        /// List of input GPIO info structures / 入力GPIO構造体のリスト
        /// </summary>
        private List<ST_IN_GPIO_INFO> _lstInGpio = new List<ST_IN_GPIO_INFO>();
        /// <summary>
        /// List of output GPIO info structures / 出力GPIO構造体のリスト
        /// </summary>
        private List<ST_OUT_GPIO_INFO> _lstOutGpio = new List<ST_OUT_GPIO_INFO>();
        /// <summary>
        /// Task for monitoring / モニタ用タスク
        /// </summary>
        private Task<string> _tskMon = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public FormGpio()
        {
            ST_IN_GPIO_INFO stInGpio;
            ST_OUT_GPIO_INFO stOutGpio;

            InitializeComponent();
            _strTitle = this.Text;
            this.Text = _strTitle + " - " + "Monitor stopped";

            // Register list of input GPIOs / 入力GPIOのリストを登録  
            stInGpio.gp = 3;
            stInGpio.checkBox_pull = checkBox_Gp3_Pull;
            stInGpio.label_inputVal = label_Gp3_Val;
            _lstInGpio.Add(stInGpio);

            stInGpio.gp = 4;
            stInGpio.checkBox_pull = checkBox_Gp4_Pull;
            stInGpio.label_inputVal = label_Gp4_Val;
            _lstInGpio.Add(stInGpio);

            stInGpio.gp = 5;
            stInGpio.checkBox_pull = checkBox_Gp5_Pull;
            stInGpio.label_inputVal = label_Gp5_Val;
            _lstInGpio.Add(stInGpio);

            stInGpio.gp = 8;
            stInGpio.checkBox_pull = checkBox_Gp8_Pull;
            stInGpio.label_inputVal = label_Gp8_Val;
            _lstInGpio.Add(stInGpio);

            stInGpio.gp = 9;
            stInGpio.checkBox_pull = checkBox_Gp9_Pull;
            stInGpio.label_inputVal = label_Gp9_Val;
            _lstInGpio.Add(stInGpio);

            stInGpio.gp = 10;
            stInGpio.checkBox_pull = checkBox_Gp10_Pull;
            stInGpio.label_inputVal = label_Gp10_Val;
            _lstInGpio.Add(stInGpio);

            stInGpio.gp = 11;
            stInGpio.checkBox_pull = checkBox_Gp11_Pull;
            stInGpio.label_inputVal = label_Gp11_Val;
            _lstInGpio.Add(stInGpio);

            // Register list of output GPIOs / 出力GPIOのリストを登録
            stOutGpio.gp = 12;
            stOutGpio.checkBox_initialVal = checkBox_Gp12_InitialVal;
            stOutGpio.label_outputVal = label_Gp12_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp12_Val;
            _lstOutGpio.Add(stOutGpio);

            stOutGpio.gp = 13;
            stOutGpio.checkBox_initialVal = checkBox_Gp13_InitialVal;
            stOutGpio.label_outputVal = label_Gp13_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp13_Val;
            _lstOutGpio.Add(stOutGpio);

            stOutGpio.gp = 14;
            stOutGpio.checkBox_initialVal = checkBox_Gp14_InitialVal;
            stOutGpio.label_outputVal = label_Gp14_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp14_Val;
            _lstOutGpio.Add(stOutGpio);

            stOutGpio.gp = 15;
            stOutGpio.checkBox_initialVal = checkBox_Gp15_InitialVal;
            stOutGpio.label_outputVal = label_Gp15_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp15_Val;
            _lstOutGpio.Add(stOutGpio);

            stOutGpio.gp = 20;
            stOutGpio.checkBox_initialVal = checkBox_Gp20_InitialVal;
            stOutGpio.label_outputVal = label_Gp20_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp20_Val;
            _lstOutGpio.Add(stOutGpio);

            stOutGpio.gp = 21;
            stOutGpio.checkBox_initialVal = checkBox_Gp21_InitialVal;
            stOutGpio.label_outputVal = label_Gp21_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp21_Val;
            _lstOutGpio.Add(stOutGpio);

            stOutGpio.gp = 22;
            stOutGpio.checkBox_initialVal = checkBox_Gp22_InitialVal;
            stOutGpio.label_outputVal = label_Gp22_Val;
            stOutGpio.checkBox_changeVal = checkBox_Gp22_Val;
            _lstOutGpio.Add(stOutGpio);
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormGpio_Load(object sender, EventArgs e)
        {
            // Get GPIO settings / GPIO設定を取得
            GetConfig();
            // GPIO monitor / GPIOモニタ
            Monitor();
        }

        /// <summary>
        /// Timer callback / タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {  
            // GPIO monitor / GPIOモニタ
            Monitor();
        }

        /// <summary>
        /// When the check state of the pull-up/pull-down checkbox changes / プルアップ/プルダウンのチェックボックスのチェック状態変化時
        /// </summary>
        private void checkBox_Pull_CheckedChanged(object sender, EventArgs e)
        {
            // Change the display of Pull-Up/Pull-Down / Pull-Up/Pull-Downの表示を変更
            CheckBox checkBox = (CheckBox)sender;
            checkBox.Text = (checkBox.Checked) ? STR_PULL_DOWN : STR_PULL_UP;
        }

        /// <summary>
        /// When the check state of the initial output value changes / 初期出力値のチェック状態変化時
        /// </summary>
        private void checkBox_InitialVal_CheckedChanged(object sender, EventArgs e)
        {
            // Change the display of High/Low / High/Lowの表示を変更
            CheckBox checkBox = (CheckBox)sender;
            checkBox.Text = (checkBox.Checked) ? STR_HIGH : STR_LOW;
        }

        /// <summary>
        /// When the check state of the output change value checkbox changes / 出力変更値のチェックボックスのチェック状態変化時
        /// </summary>
        private void checkBox_OutVal_CheckedChanged(object sender, EventArgs e)
        {
            // Change the display of High/Low / High/Lowの表示を変更
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked)
            {
                checkBox.Text = STR_HIGH;
                checkBox.BackColor = UI.MonRed;
            }
            else
            {
                checkBox.Text = STR_LOW;
                checkBox.BackColor = UI.MonBlue;
            }
        }

        /// <summary>
        /// When the "Change Settings" button is pressed / 「設定の変更」ボタンを押した時
        /// </summary>
        private async void button_SetConfig_Click(object sender, EventArgs e)
        {
            try
            {
                UInt32 pullDownBits = 0;
                UInt32 initialOutValBits = 0;
                string strErrMsg;

                // Display confirmation message / 確認メッセージを表示
                if (DialogResult.No == UI.ShowYesNoMsg(this, "Do you want to save settings to flash memory?\n\n(The microcontroller will be reset.)"))
                {
                    return;
                }

                foreach (ST_IN_GPIO_INFO stInGpio in _lstInGpio)
                {
                    if (stInGpio.checkBox_pull.Checked)
                    {
                        pullDownBits |= (UInt32)(1 << stInGpio.gp);
                    }
                }

                foreach (ST_OUT_GPIO_INFO stOutGpio in _lstOutGpio)
                {
                    if (stOutGpio.checkBox_initialVal.Checked)
                    {
                        initialOutValBits |= (UInt32)(1 << stOutGpio.gp);
                    }
                }

                // Send request for "Set GPIO Config" command / 「GPIO設定変更」コマンドの要求を送信
                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    return Program.PrpJigCmd.SendCmd_SetGpioConfig(pullDownBits, initialOutValBits);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    _isMonitorStillNotSuccessful = true;
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
        /// When the "Change Output Value" button is pressed / 「出力値の変更」ボタンを押した時
        /// </summary>
        private async void button_PutGpio_Click(object sender, EventArgs e)
        {
            try
            {
                UInt32 outValBits = 0;
                string strErrMsg;

                foreach (ST_OUT_GPIO_INFO stOutGpio in _lstOutGpio)
                {
                    if (stOutGpio.checkBox_changeVal.Checked)
                    {
                        outValBits |= (UInt32)(1 << stOutGpio.gp);
                    }
                }

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "GPIO Output" command / 「GPIO出力」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_OutGpio(outValBits);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg != null)
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Change Output Value Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }

        /// <summary>
        /// Get GPIO settings / GPIO設定を取得
        /// </summary>
        private async void GetConfig()
        {
            try
            {
                UInt32 pullDownBits = 0;
                UInt32 initialOutValBits = 0;
                string strErrMsg;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Get GPIO Config" command / 「GPIO設定取得」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_GetGpioConfig(out pullDownBits, out initialOutValBits);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg == null)
                {
                    // Update display of input GPIO settings / 入力GPIO設定の表示を更新
                    foreach (ST_IN_GPIO_INFO stInGpio in _lstInGpio)
                    {
                        if ((pullDownBits & (1 << stInGpio.gp)) != 0)
                        {
                            stInGpio.checkBox_pull.Text = STR_PULL_DOWN;
                            stInGpio.checkBox_pull.Checked = true;
                        }
                        else
                        {
                            stInGpio.checkBox_pull.Text = STR_PULL_UP;
                            stInGpio.checkBox_pull.Checked = false;
                        }
                    }
                    // Update display of output GPIO settings / 出力GPIO設定の表示を更新
                    foreach (ST_OUT_GPIO_INFO stOutGpio in _lstOutGpio)
                    {
                        if ((initialOutValBits & (1 << stOutGpio.gp)) != 0)
                        {
                            stOutGpio.checkBox_initialVal.Text = STR_HIGH;
                            stOutGpio.checkBox_initialVal.Checked = true;
                        }
                        else
                        {
                            stOutGpio.checkBox_initialVal.Text = STR_LOW;
                            stOutGpio.checkBox_initialVal.Checked = false;
                        }
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

        // GPIO monitor / GPIOモニタ
        private async void Monitor()
        {
            try
            {
                UInt32 valBits = 0;
                string strErrMsg;

                if (true == Program.PrpJigCmd.IsConnected())
                {
                    if (_tskMon == null || _tskMon.IsCompleted)
                    {
                        _tskMon = Task.Run(() =>
                        {
                            // Send request for "Get GPIO Input/Output Value" command / 「GPIO入出力値取得」コマンドの要求を送信
                            return Program.PrpJigCmd.SendCmd_GetGpio(out valBits);
                        });
                        strErrMsg = await _tskMon;

                        if (this.IsDisposed) return;

                        if (strErrMsg == null)
                        {
                            // Update display of GPIO input values / GPIO入力値の表示を更新
                            foreach (ST_IN_GPIO_INFO stInGpio in _lstInGpio)
                            {
                                if ((valBits & (1 << stInGpio.gp)) != 0)
                                {
                                    stInGpio.label_inputVal.Text = STR_HIGH;
                                    stInGpio.label_inputVal.BackColor = UI.MonRed;
                                }
                                else
                                {
                                    stInGpio.label_inputVal.Text = STR_LOW;
                                    stInGpio.label_inputVal.BackColor = UI.MonBlue;
                                }
                            }

                            // Update display of GPIO output values / GPIO出力値の表示を更新
                            foreach (ST_OUT_GPIO_INFO stOutGpio in _lstOutGpio)
                            {
                                if ((valBits & (1 << stOutGpio.gp)) != 0)
                                {
                                    stOutGpio.label_outputVal.Text = STR_HIGH;
                                    stOutGpio.label_outputVal.BackColor = UI.MonRed;
                                    if (_isMonitorStillNotSuccessful)
                                    {
                                        stOutGpio.checkBox_changeVal.Text = STR_HIGH;
                                        stOutGpio.checkBox_changeVal.Checked = true;
                                        stOutGpio.checkBox_changeVal.BackColor = UI.MonRed;
                                    }
                                }
                                else
                                {
                                    stOutGpio.label_outputVal.Text = STR_LOW;
                                    stOutGpio.label_outputVal.BackColor = UI.MonBlue;
                                    if (_isMonitorStillNotSuccessful)
                                    {
                                        stOutGpio.checkBox_changeVal.Text = STR_LOW;
                                        stOutGpio.checkBox_changeVal.Checked = false;
                                        stOutGpio.checkBox_changeVal.BackColor = UI.MonBlue;
                                    }
                                }
                            }

                            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName + " - " + "Monitoring";
                            _isMonitorStillNotSuccessful = false;
                        }
                        else
                        {
                            this.Text = _strTitle + " - " + "Monitor stopped";
                            FormMain.Inst.AppendAppLogText(true, strErrMsg);
                        }
                    }
                }
                else
                {
                    this.Text = _strTitle + " - " + "Monitor stopped";
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) FormMain.Inst.AppendAppLogText(true, $"Monitor Error: {ex.Message}");
            }
        }
    }
}
