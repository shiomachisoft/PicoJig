// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormPwm : Form
    {
        /// <summary>
        /// Form title / フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public FormPwm()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormPwm_Load(object sender, EventArgs e)
        {
            // Display title / タイトルを表示
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
        }

        /// <summary>
        /// When the "Start" button is pressed / 「開始」ボタンを押した時
        /// </summary>
        private async void button_Start_Click(object sender, EventArgs e)
        {
            try
            {
                float clkdiv;  // Clock divider / クロック分周器
                UInt16 wrap;   // Wrap value / ラップ値
                UInt16 level;  // Compare value / 比較値     
                string strErrMsg;

                // Get clock divider / クロック分周器を取得
                clkdiv = (float)numericUpDown_Divider.Value;
                // Get wrap value / ラップ値を取得
                wrap = (UInt16)numericUpDown_Wrap.Value;
                // Get compare value / 比較値を取得
                level = (UInt16)numericUpDown_Level.Value;
          
                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Start PWM" command / 「PWM開始」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_StartPwm(clkdiv, wrap, level);
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg != null)
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Start PWM Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }

        /// <summary>
        /// When the "Stop" button is pressed / 「停止」ボタンを押した時
        /// </summary>
        private async void button_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                string strErrMsg;

                this.Enabled = false;
                strErrMsg = await Task.Run(() =>
                {
                    // Send request for "Stop PWM" command / 「PWM停止」コマンドの要求を送信
                    return Program.PrpJigCmd.SendCmd_StopPwm();
                });
                if (this.IsDisposed) return;
                
                if (strErrMsg != null)
                {
                    UI.ShowErrMsg(this, strErrMsg);
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) UI.ShowErrMsg(this, $"Stop PWM Error: {ex.Message}");
            }
            finally
            {
                if (!this.IsDisposed) this.Enabled = true;
            }
        }
    }
}
