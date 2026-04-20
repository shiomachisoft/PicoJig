// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.IO.Ports;

namespace JigLib
{
    public class JigSerial : JigCmd
    {
        /// <summary>
        /// Baud rate / ボーレート
        /// </summary>
        private const int PORT_BAUD_RATE = 115200;
        /// <summary>
        /// Data bits / データビット
        /// </summary>
        private const int PORT_DATA_BITS = 8;
        /// <summary>
        /// Port output buffer size / ポートの出力バッファサイズ
        /// </summary>
        //private const int PORT_WRITE_BUF_SIZE = 8192;
        /// <summary>
        /// Port input buffer size / ポートの入力バッファサイズ
        /// </summary>
        //private const int PORT_READ_BUF_SIZE = 8192;
        /// <summary>
        /// Port write timeout (ms) / ポート書き込みタイムアウト時間(ms)
        /// </summary>
        private const int PORT_WRITE_TIMEOUT = 3000;
        /// <summary>
        /// Port read timeout (ms) / ポート読み取りタイムアウト時間(ms)
        /// </summary>
        private const int PORT_READ_TIMEOUT = 3000;

        /// <summary>
        /// Port instance / ポートのインスタンス
        /// </summary>
        private SerialPort _port = new SerialPort();
        /// <summary>
        /// Receive task / 受信タスク
        /// </summary>
        private Task _tskRecv = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public JigSerial()
        {
        }

        /// <summary>
        /// Open the port / ポートをオープン
        /// </summary>
        /// <param name="objPortName">
        /// COM port name to connect to (e.g. "COM1") / 接続するCOMポート名(例: "COM1")
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public override string Connect(Object objPortName)
        {
            string strErrMsg = null;

            lock (_lockPort) // Exclusive access to port / ポートのアクセスの排他
            {
                try
                {
                    // Port communication settings / ポートの通信設定
                    _port.PortName = (string)objPortName;           // Communication port name / 通信ポート名
                    _port.BaudRate = PORT_BAUD_RATE;                // Baud rate / ボーレート
                    _port.DataBits = PORT_DATA_BITS;                // Data bit length / データビット長
                    _port.Parity = Parity.None;                     // No parity / パリティ無し
                    _port.StopBits = StopBits.One;                  // Stop bit = 1bit / ストップビット = 1bit
                    _port.Handshake = Handshake.None;               // No flow control / フロー制御無し
                    //_port.WriteBufferSize = PORT_WRITE_BUF_SIZE;  // Output buffer size / 出力バッファのサイズ
                    //_port.ReadBufferSize = PORT_READ_BUF_SIZE;    // Input buffer size / 入力バッファのサイズ
                    _port.WriteTimeout = PORT_WRITE_TIMEOUT;        // Write timeout time / 書き込みタイムアウト時間
                    _port.ReadTimeout = PORT_READ_TIMEOUT;          // Read timeout time / 読み取りタイムアウト時間

                    // Open port / ポートをオープン
                    _port.Open();
                    _port.DtrEnable = true; // DTR = ON 
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                }

                if (strErrMsg == null)
                {
                    // Start receive task / 受信タスクを開始
                    _isDisconnecting = false;
                    _tskRecv = new Task(Recv);
                    _tskRecv.Start();
                    PrpConnectName = _port.PortName;
                    _isConnected = true;
                }
            }
        
            return strErrMsg;
        }

        /// <summary>
        /// Close the port / ポートをクローズ
        /// </summary>
        public override void Disconnect()
        {
            Task waitTask = null;
            lock (_lockPort) // Exclusive access to port / ポートのアクセスの排他
            {
                if (_tskRecv != null)
                {
                    // Cancel waiting for command transmission response / コマンド送信の応答待ちを中止する
                    PrpResEvent.Set(); 
                    _isDisconnecting = true;
                    waitTask = _tskRecv;
                }
            }

            if (waitTask != null)
            {
                // Wait for receive task to finish / 受信タスクの終了を待つ
                waitTask.Wait(RECV_TASK_END_TIMEOUT);

                lock (_lockPort) // リソース解放
                {
                    _tskRecv = null;
                    if (_port.IsOpen) // If port is already open / ポートがオープン済みの場合
                    {
                        try
                        {
                            // Close port / ポートをクローズ
                            _port.Close();
                        }
                        catch { }
                    }
                    _isConnected = false;
                }
            }
        }
        
        /// <summary>
        /// Get whether the port is already open / ポートがオープン済みか否かを取得
        /// </summary>
        /// <returns>
        /// true: Already open, false: Not open / true:オープン済み, false:未オープン
        /// </returns>
        public override bool IsConnected()
        {
            bool bRet = false;

            lock (_lockPort) // Exclusive access to port / ポートのアクセスの排他
            {
                if (_isConnected)
                {
                    try
                    {
                        bRet = _port.IsOpen;
                    }
                    catch { }
                }    
            }

            return bRet;
        }

        /// <summary>
        /// Serial transmission / シリアル送信
        /// </summary>
        protected override string Send(byte[] buf)
        {
            string strErrMsg = null;

            lock (_lockPort) // Exclusive access to port / ポートのアクセスの排他
            {
                if (false == IsConnected())
                {
                    strErrMsg = "Not connected.";
                }
                else
                {
                    try
                    {
                        // Serial transmission / シリアル送信
                        _port.Write(buf, 0, buf.Length);
                    }
                    catch (Exception ex)
                    {
                        strErrMsg = ex.Message;
                    }
                }
            }
        
            return strErrMsg;
        }

        /// <summary>
        /// Check if serial receive data exists / シリアル受信データが存在するかを確認
        /// </summary>
        protected override bool IsExistRecvData()
        {
            bool bRet = false;
            int readSize = 0;

            lock (_lockPort) // Exclusive access to port / ポートのアクセスの排他
            {
                try
                {
                    if (_port.IsOpen)
                    {
                        readSize = _port.BytesToRead;
                    }
                }
                catch { }
                if (readSize > 0)
                {
                    bRet = true;
                }
            }

            return bRet;
        }

        /// <summary>
        /// Extract 1 byte of serial receive data / シリアル受信データを1byte取り出し
        /// </summary>
        protected override bool ReadByte(out byte data)
        {
            bool bRet = false;
            int readData = -1;

            data = 0;

            lock (_lockPort) // Exclusive access to port / ポートのアクセスの排他
            {
                try
                {
                    if (_port.IsOpen)
                    {
                        try
                        {
                            readData = _port.ReadByte();
                        }
                        catch { };
                    }
                }
                catch { }
                if (readData >= 0)
                {
                    data = (byte)readData;
                    bRet = true;
                }
            }

            return bRet;
        }
    }
}
