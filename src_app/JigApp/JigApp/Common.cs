// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JigLib;

namespace JigApp
{
    /// <summary>
    /// 文字列操作のクラス
    /// </summary>
    public static class Str
    {
        public const string STR_FW_NAME_PICOJIG = "PicoJig";
        public const string STR_FW_NAME_PICOJIG_WL = "PicoJig_WL";
        public const string STR_FW_NAME_PICOBRG = "PicoBrg";
        public const string STR_FW_NAME_PICOSEN = "PicoSen";

        /// <summary>
        /// FW名
        /// </summary>
        public static string PrpFwName { get; set; } = string.Empty;

        /// <summary>
        /// 16進数文字列をbyte型の配列に変換
        /// </summary>
        /// <remarks>
        /// セパレータ:スペース/タブ/CRLF/CR
        /// </remarks>
        public static string ConvertHexStringToByteArray(string strText, out byte[] aVal)
        {
            char[] aSeparator = { ' ', ',', '\t', '\r' }; // セパレータ    
            return ConvertStringToValArray(strText, aSeparator, 16, out aVal);
        }

        /// <summary>
        /// 文字列をbyte型の配列に変換
        /// </summary>
        public static string ConvertStringToValArray(string strText, char[] aSeparator, int baseNumber, out byte[] aVal)
        {
            string[] astrSplit; // 分割後の文字列
            string strErrMsg = null;

            // 文字列をセパレータで分割
            strText = strText.Replace("\r\n", "\r");
            astrSplit = strText.Split(aSeparator);

            // [文字列をbyte型の配列に変換]
            // 要素数が分割された文字列の数であるbyte型配列を用意
            aVal = new byte[astrSplit.Count()];
            // 分割された文字列の数だけ、文字列をbyte型に変換
            for (int i = 0; i < astrSplit.Count(); i++)
            {
                try
                {
                    aVal[i] = Convert.ToByte(astrSplit[i], baseNumber);
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                }
            }

            return strErrMsg;
        }
    }

    /// <summary>
    /// UI操作のクラス
    /// </summary>
    public static class UI
    {
        /// <summary>
        /// モニタ用の'赤'を返す
        /// </summary>
        public static Color MonRed { get; set; } = Color.FromArgb(255, 150, 150);

        /// <summary>
        /// モニタ用の'緑'を返す
        /// </summary>
        public static Color MonGreen { get; set; } = Color.FromArgb(150, 255, 150);

        /// <summary>
        /// モニタ用の'青'を返す
        /// </summary>
        public static Color MonBlue { get; set; } = Color.FromArgb(150, 150, 255);

        /// <summary>
        /// 通知のメッセージボックスを表示
        /// </summary>
        public static void ShowInfoMsg(Form frm, string strMsg)
        {
            MessageBox.Show(frm, strMsg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Yes/No確認のメッセージボックスを表示
        /// </summary>
        public static DialogResult ShowYesNoMsg(Form frm, string strMsg)
        {
            return MessageBox.Show(frm, strMsg, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// 警告のメッセージボックスを表示
        /// </summary>
        public static void ShowWarnMsg(Form frm, string strMsg)
        {
            MessageBox.Show(frm, strMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// エラーのメッセージボックスを表示
        /// </summary>
        public static void ShowErrMsg(Form frm, string strMsg)
        {
            FormMain.Inst.AppendAppLogText(true, strMsg);
            MessageBox.Show(frm, strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 引数の文字列と一致するコンボボックスのアイテムを選択する
        /// </summary>
        public static void SelectComboBoxItem(ComboBox combBox, string strText)
        {
            string strItem;

            for (int i = 0; i < combBox.Items.Count; i++)
            {
                strItem = combBox.Items[i].ToString();
                if (strItem == strText)
                {
                    combBox.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 通信ログマネージャのクラス
    /// </summary>
    public class LogViewMng
    {
        private const string STR_S = "[S]";
        private const string STR_R = "[R]";

        /// <summary>
        /// 受信データのモニタ用タスクの終了待ちタイムアウト時間(ms)
        /// </summary>
        private const int MONITOR_TASK_END_TIMEOUT = 3000;
        /// <summary>
        /// Monitor()のwhile文のディレイ(ms)
        /// </summary>
        private const int MON_DELAY = 50;

        /// <summary>
        /// ポートをクローズ中か否か
        /// </summary>
        private bool _isClosing = false;
        /// <summary>
        /// 最後のログの種類は送信か否か
        /// </summary>
        private bool _isLastLogSending = true;
        /// <summary>
        /// 受信データのモニタ用タスク
        /// </summary>
        /// <remarks>
        /// 受信データを検出した場合、通信ログを更新
        /// </remarks>
        private Task _tskMonitor = null;
        /// <summary>
        /// 通信ログを表示するテキストボックス
        /// </summary>
        private TextBox _textBox_Log = null;
        /// <summary>
        /// 受信データのキュー
        /// </summary>
        private BlockingCollection<byte> _recvDataQue = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// 通信ログを表示するテキストボックスと受信データのキューを登録
        /// </remarks>
        public LogViewMng(TextBox textBox_Log, BlockingCollection<byte> recvDataQue = null)
        {
            _textBox_Log = textBox_Log;
            _recvDataQue = recvDataQue;
        }

        /// <summary>
        /// 受信データのモニタ用タスクを開始
        /// </summary>
        public void StartMonitor()
        {
            _tskMonitor = new Task(Monitor);
            _tskMonitor.Start();
        }

        /// <summary>
        /// 受信データのモニタ用タスクを終了
        /// </summary>
        public async void EndMonitor()
        {
            if (_tskMonitor != null)
            {
                await Task.Run(() =>
                {
                    _isClosing = true;
                    _tskMonitor.Wait(MONITOR_TASK_END_TIMEOUT);
                });
            }
        }

        /// <summary>
        /// 通信ログを表示するテキストボックスにログを追加
        /// </summary>
        public void Add(bool isRecv, byte[] aData)
        {
            if (_textBox_Log.Text != string.Empty)
            {
                _textBox_Log.AppendText("\r\n");
            }

            if (isRecv)
            {
                _textBox_Log.AppendText(STR_R);
                _isLastLogSending = false;
            }
            else
            {
                _textBox_Log.AppendText(STR_S);
                _isLastLogSending = true;
            }

            foreach (byte data in aData)
            {
                _textBox_Log.AppendText(data.ToString("X2") + " ");
            }
        }

        /// <summary>
        /// 受信データのモニタ
        /// </summary>
        /// <remarks>
        /// 受信データがある場合、通信ログを表示するテキストボックスに受信データのログを追加
        /// </remarks> 
        private void Monitor()
        {
            byte data;

            while (!_isClosing)
            {
                if (_recvDataQue.TryTake(out data))
                {
                    _textBox_Log.Invoke((MethodInvoker)(() =>
                    {

                        if ((_textBox_Log.Text != string.Empty) && (_isLastLogSending == true))
                        {
                            _textBox_Log.AppendText("\r\n");
                        }

                        if (_isLastLogSending == true)
                        {
                            _textBox_Log.AppendText(STR_R);
                        }
                          
                        _textBox_Log.AppendText(data.ToString("X2") + " ");
                            
                        _isLastLogSending = false;

                    }));
                }
                else
                {
                    Thread.Sleep(MON_DELAY);
                }
            }
        }
    }
}
