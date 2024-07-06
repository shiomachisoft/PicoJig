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
    public partial class FormAdc : Form
    {
        /// <summary>
        /// フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// モニタ用タスク
        /// </summary>
        private Task<string> _tskMon = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormAdc()
        {
            InitializeComponent();
            _strTitle = this.Text;
            this.Text = _strTitle + " - " + "Monitor stopped";
        }

        /// <summary>
        /// フォームのロード時
        /// </summary>
        private void FormAdc_Load(object sender, EventArgs e)
        {
            // ADCモニタ
            Monitor();
        }

        /// <summary>
        /// タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // ADCモニタ
            Monitor();
        }

        /// <summary>
        /// ADCモニタ
        /// </summary>
        private async void Monitor()
        {
            float[] aVolt = null;
            string strErrMsg;

            if (true == Program.PrpJigCmd.IsConnected())
            {
                if (_tskMon == null || (_tskMon != null && _tskMon.IsCompleted))
                {
                    _tskMon = Task.Run(() =>
                    {
                        //「ADC入力」コマンドの要求を送信
                        return Program.PrpJigCmd.SendCmd_GetAdc(out aVolt);
                    });
                    strErrMsg = await _tskMon;
                    if (strErrMsg == null)
                    {
                        label_Adc0.Text = aVolt[0].ToString("F3");
                        label_Adc1.Text = aVolt[1].ToString("F3");
                        label_Adc2.Text = aVolt[2].ToString("F3");
                        label_Temp.Text = aVolt[3].ToString("F3");

                        this.Text = _strTitle + " - " + Program.PrpJigCmd.PrpConnectName + " - " + "Monitoring";
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
    }
}
