// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JigApp
{
    /// <summary>
    /// String operation class / 文字列操作のクラス
    /// </summary>
    public static class Str
    {
        public const string STR_FW_NAME_PICOJIG = "PicoJig";
        public const string STR_FW_NAME_PICOJIG_WL = "PicoJig_WL";
        public const string STR_FW_NAME_PICOBRG = "PicoBrg";
        public const string STR_FW_NAME_PICOIOT = "PicoIot";

        /// <summary>
        /// FW name / FW名
        /// </summary>
        public static string PrpFwName { get; set; } = string.Empty;

        /// <summary>
        /// Convert hex string to byte array / 16進数文字列をbyte型の配列に変換
        /// </summary>
        /// <remarks>
        /// Separator: Space/Tab/CRLF/CR / セパレータ:スペース/タブ/CRLF/CR
        /// </remarks>
        public static string ConvertHexStringToByteArray(string strText, out byte[] aVal)
        {
            char[] aSeparator = { ' ', ',', '\t', '\r', '\n' }; // Separator / セパレータ    
            return ConvertStringToValArray(strText, aSeparator, 16, out aVal);
        }

        /// <summary>
        /// Convert string to byte array / 文字列をbyte型の配列に変換
        /// </summary>
        public static string ConvertStringToValArray(string strText, char[] aSeparator, int baseNumber, out byte[] aVal)
        {
            string[] astrSplit; // Split string / 分割後の文字列
            string strErrMsg = null;

            // Split string by separator / 文字列をセパレータで分割
            astrSplit = strText.Split(aSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (astrSplit.Length == 0)
            {
                aVal = new byte[0];
                return "No valid transmission data has been entered.";
            }

            // [Convert string to byte array] / [文字列をbyte型の配列に変換]
            // Prepare byte array with the number of split strings / 要素数が分割された文字列の数であるbyte型配列を用意
            aVal = new byte[astrSplit.Length];
            // Convert string to byte type for the number of split strings / 分割された文字列の数だけ、文字列をbyte型に変換
            for (int i = 0; i < astrSplit.Length; i++)
            {
                try
                {
                    aVal[i] = Convert.ToByte(astrSplit[i], baseNumber);
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                    break;
                }
            }

            return strErrMsg;
        }
    }

    /// <summary>
    /// UI operation class / UI操作のクラス
    /// </summary>
    public static class UI
    {
        /// <summary>
        /// Return 'Red' for monitor / モニタ用の'赤'を返す
        /// </summary>
        public static Color MonRed { get; set; } = Color.FromArgb(255, 150, 150);

        /// <summary>
        /// Return 'Green' for monitor / モニタ用の'緑'を返す
        /// </summary>
        public static Color MonGreen { get; set; } = Color.FromArgb(150, 255, 150);

        /// <summary>
        /// Return 'Blue' for monitor / モニタ用の'青'を返す
        /// </summary>
        public static Color MonBlue { get; set; } = Color.FromArgb(150, 150, 255);

        /// <summary>
        /// Display info message box / 通知のメッセージボックスを表示
        /// </summary>
        public static void ShowInfoMsg(Form frm, string strMsg)
        {
            MessageBox.Show(frm, strMsg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Display Yes/No confirmation message box / Yes/No確認のメッセージボックスを表示
        /// </summary>
        public static DialogResult ShowYesNoMsg(Form frm, string strMsg)
        {
            return MessageBox.Show(frm, strMsg, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Display warning message box / 警告のメッセージボックスを表示
        /// </summary>
        public static void ShowWarnMsg(Form frm, string strMsg)
        {
            MessageBox.Show(frm, strMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Display error message box / エラーのメッセージボックスを表示
        /// </summary>
        public static void ShowErrMsg(Form frm, string strMsg)
        {
            FormMain.Inst.AppendAppLogText(true, strMsg);
            MessageBox.Show(frm, strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Select combo box item that matches argument string / 引数の文字列と一致するコンボボックスのアイテムを選択する
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

        /// <summary>
        /// Allow only half-width characters when key is pressed in text box / テキストボックスがキープレスされた時に半角のみ許可
        /// </summary>
        public static void TextBox_HalfWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            // Allow backspace (0x08 among control characters) / バックスペースは許可(制御文字のうち 0x08)
            if (c == '\b') return;

            // Allow Ctrl + A(0x01), C(0x03), V(0x16), X(0x18), Z(0x1A) / Ctrl + A(0x01), C(0x03), V(0x16), X(0x18), Z(0x1A) を許可
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (c == 0x01 || c == 0x03 || c == 0x16 || c == 0x18 || c == 0x1A) return;
            }

            // Allow printable ASCII characters (0x20-0x7E) ... Alphanumeric and symbols / 表示可能なASCII文字(0x20〜0x7E)を許可・・・英数字と記号
            if (c >= 0x20 && c <= 0x7E) return;

            // Disable others (full-width or control characters) / それ以外(全角や制御文字)は無効化
            e.Handled = true;
        }
    }

    /// <summary>
    /// Communication log manager class / 通信ログマネージャのクラス
    /// </summary>
    public class LogViewMng
    {
        private const string STR_S = "[S]";
        private const string STR_R = "[R]";

        /// <summary>
        /// Timeout time for receive data monitor task to end (ms) / 受信データのモニタ用タスクの終了待ちタイムアウト時間(ms)
        /// </summary>
        private const int MONITOR_TASK_END_TIMEOUT = 3000;
        /// <summary>
        /// Delay for while loop in Monitor() (ms) / Monitor()のwhile文のディレイ(ms)
        /// </summary>
        private const int MON_DELAY = 50;
        /// <summary>
        /// Maximum number of bytes to reflect in UI at once / 一度にUIへ反映する最大バイト数
        /// </summary>
        private const int MAX_RECV_BUFFER_SIZE = 4096;

        /// <summary>
        /// Whether the port is being closed / ポートをクローズ中か否か
        /// </summary>
        private bool _isClosing = false;
        /// <summary>
        /// Whether the last log type is send / 最後のログの種類は送信か否か
        /// </summary>
        private bool _isLastLogSending = true;
        /// <summary>
        /// Task for receive data monitor / 受信データのモニタ用タスク
        /// </summary>
        /// <remarks>
        /// If receive data is detected, update communication log / 受信データを検出した場合、通信ログを更新
        /// </remarks>
        private Task _tskMonitor = null;
        /// <summary>
        /// Text box for displaying communication log / 通信ログを表示するテキストボックス
        /// </summary>
        private TextBox _textBox_Log = null;
        /// <summary>
        /// Receive data queue / 受信データのキュー
        /// </summary>
        private BlockingCollection<byte> _recvDataQue = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        /// <remarks>
        /// Register text box for displaying communication log and receive data queue / 通信ログを表示するテキストボックスと受信データのキューを登録
        /// </remarks>
        public LogViewMng(TextBox textBox_Log, BlockingCollection<byte> recvDataQue = null)
        {
            _textBox_Log = textBox_Log;
            _recvDataQue = recvDataQue;
        }

        /// <summary>
        /// Start receive data monitor task / 受信データのモニタ用タスクを開始
        /// </summary>
        public void StartMonitor()
        {
            _tskMonitor = new Task(Monitor);
            _tskMonitor.Start();
        }

        /// <summary>
        /// End receive data monitor task / 受信データのモニタ用タスクを終了
        /// </summary>
        public void EndMonitor()
        {
            if (_tskMonitor != null)
            {
                _isClosing = true;
                Task.Run(() =>
                {
                    try
                    {
                        _tskMonitor.Wait(MONITOR_TASK_END_TIMEOUT);
                    }
                    catch (Exception)
                    {
                        // Ignore exceptions during teardown / 終了時の例外は無視する
                    }
                });
            }
        }

        /// <summary>
        /// Add log to text box for displaying communication log / 通信ログを表示するテキストボックスにログを追加
        /// </summary>
        public void Add(bool isRecv, byte[] aData)
        {
            if (_textBox_Log.TextLength > 0)
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

            if (aData != null)
            {
                _textBox_Log.AppendText(string.Join(" ", aData.Select(b => b.ToString("X2"))) + " ");
            }
        }

        /// <summary>
        /// Receive data monitor / 受信データのモニタ
        /// </summary>
        /// <remarks>
        /// If there is receive data, add receive data log to text box for displaying communication log / 受信データがある場合、通信ログを表示するテキストボックスに受信データのログを追加
        /// </remarks> 
        private void Monitor()
        {
            while (!_isClosing)
            {
                if (_recvDataQue.TryTake(out byte data, MON_DELAY))
                {
                    System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
                    buffer.Add(data);
                    while (buffer.Count < MAX_RECV_BUFFER_SIZE && _recvDataQue.TryTake(out byte nextData))
                    {
                        buffer.Add(nextData);
                    }

                    if (_textBox_Log.IsDisposed) break;

                    try
                    {
                        _textBox_Log.Invoke((MethodInvoker)(() =>
                        {
                            if ((_textBox_Log.TextLength > 0) && (_isLastLogSending == true))
                            {
                                _textBox_Log.AppendText("\r\n");
                            }

                            if (_isLastLogSending == true)
                            {
                                _textBox_Log.AppendText(STR_R);
                            }

                            _textBox_Log.AppendText(string.Join(" ", buffer.Select(b => b.ToString("X2"))) + " ");

                            _isLastLogSending = false;
                        }));
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
        }
    }
}
