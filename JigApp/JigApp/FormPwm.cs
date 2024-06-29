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
    public partial class FormPwm : Form
    {
        /// <summary>
        /// フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormPwm()
        {
            InitializeComponent();
            _strTitle = this.Text;
        }

        /// <summary>
        /// フォームのロード時
        /// </summary>
        private void FormPwm_Load(object sender, EventArgs e)
        {
            // タイトルを表示
            this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName;
        }

        /// <summary>
        /// 「開始」ボタンを押した時
        /// </summary>
        private async void button_Start_Click(object sender, EventArgs e)
        {
            float divider; // クロック分周器
            UInt16 wrap;   // 分解能
            UInt16 level;  // Highの期間     
            string strErrMsg;

            // クロック分周器を取得
            divider = (float)numericUpDown_Divider.Value;
            // 分解能を取得
            wrap = (UInt16)numericUpDown_Wrap.Value;
            // Highの期間を取得
            level = (UInt16)numericUpDown_Level.Value;
      
            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「PWM開始」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_StartPwm(divider, wrap, level);
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 無処理
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }

        /// <summary>
        /// 「停止」ボタンを押した時
        /// </summary>
        private async void button_Stop_Click(object sender, EventArgs e)
        {
            string strErrMsg;

            this.Enabled = false;
            strErrMsg = await Task.Run(() =>
            {
                //「PWM停止」コマンドの要求を送信
                return Program.PrpJigCmd.SendCmd_StopPwm();
            });
            this.Enabled = true;

            if (strErrMsg == null)
            {
                // 無処理
            }
            else
            {
                UI.ShowErrMsg(this, strErrMsg);
            }
        }
    }
}
