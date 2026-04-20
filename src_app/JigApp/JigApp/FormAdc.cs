// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    public partial class FormAdc : Form
    {
        /// <summary>
        /// Form title / フォームのタイトル
        /// </summary>
        private string _strTitle;

        /// <summary>
        /// Task for monitoring / モニタ用タスク
        /// </summary>
        private Task<string> _tskMon = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public FormAdc()
        {
            InitializeComponent();
            _strTitle = this.Text;
            this.Text = _strTitle + " - " + "Monitor Stopped";
        }

        /// <summary>
        /// When the form is loaded / フォームのロード時
        /// </summary>
        private void FormAdc_Load(object sender, EventArgs e)
        {
            // ADC monitor / ADCモニタ
            Monitor();
        }

        /// <summary>
        /// Timer callback / タイマーコールバック
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // ADC monitor / ADCモニタ
            Monitor();
        }

        /// <summary>
        /// ADC monitor / ADCモニタ
        /// </summary>
        private async void Monitor()
        {
            try
            {
                float[] aVolt = null;
                string strErrMsg;

                if (true == Program.PrpJigCmd.IsConnected())
                {
                    if (_tskMon == null || _tskMon.IsCompleted)
                    {
                        _tskMon = Task.Run(() =>
                        {
                            // Send request for "ADC Input" command / 「ADC入力」コマンドの要求を送信
                            return Program.PrpJigCmd.SendCmd_GetAdc(out aVolt);
                        });
                        strErrMsg = await _tskMon;

                        if (this.IsDisposed) return;

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
                            this.Text = _strTitle + " - " + "Monitor Stopped";
                            FormMain.Inst.AppendAppLogText(true, strErrMsg);
                        }
                    }
                }
                else
                {
                    this.Text = _strTitle + " - " + "Monitor Stopped";
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) FormMain.Inst.AppendAppLogText(true, $"Monitor Error: {ex.Message}");
            }
        }
    }
}
