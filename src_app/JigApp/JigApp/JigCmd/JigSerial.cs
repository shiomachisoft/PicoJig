// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace JigLib
{
    public class JigSerial : JigCmd
    {
        /// <summary>
        /// ボーレート
        /// </summary>
        private const int PORT_BAUD_RATE = 115200;
        /// <summary>
        /// データビット
        /// </summary>
        private const int PORT_DATA_BITS = 8;
        /// <summary>
        /// ポートの出力バッファサイズ
        /// </summary>
        //private const int PORT_WRITE_BUF_SIZE = 8192;
        /// <summary>
        /// ポートの入力バッファサイズ
        /// </summary>
        //private const int PORT_READ_BUF_SIZE = 8192;
        /// <summary>
        /// ポート書き込みタイムアウト時間(ms)
        /// </summary>
        private const int PORT_WRITE_TIMEOUT = 3000;
        /// <summary>
        /// ポート読み取りタイムアウト時間(ms)
        /// </summary>
        private const int PORT_READ_TIMEOUT = 3000;

        /// <summary>
        /// ポートのインスタンス
        /// </summary>
        private SerialPort _port = new SerialPort();
        /// <summary>
        /// 受信タスク
        /// </summary>
        private Task _tskRecv = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JigSerial()
        {
        }

        /// <summary>
        /// ポートをオープン
        /// </summary>
        public override string Connect(Object objPortName)
        {
            string strErrMsg = null;

            lock (_lockPort) // ポートのアクセスの排他
            {
                try
                {
                    // ポートの通信設定
                    _port.PortName = (string)objPortName;           // 通信ポート名
                    _port.BaudRate = PORT_BAUD_RATE;                // ボーレート
                    _port.DataBits = PORT_DATA_BITS;                // データビット長
                    _port.Parity = Parity.None;                     // パリティ無し
                    _port.StopBits = StopBits.One;                  // ストップビット = 1bit
                    _port.Handshake = Handshake.None;               // フロー制御無し
                    //_port.WriteBufferSize = PORT_WRITE_BUF_SIZE;  // 出力バッファのサイズ
                    //_port.ReadBufferSize = PORT_READ_BUF_SIZE;    // 入力バッファのサイズ
                    _port.WriteTimeout = PORT_WRITE_TIMEOUT;        // 書き込みタイムアウト時間
                    _port.ReadTimeout = PORT_READ_TIMEOUT;          // 読み取りタイムアウト時間

                    // ポートをオープン
                    _port.Open();
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                }

                if (strErrMsg == null)
                {
                    // 受信タスクを開始
                    _isDisconnecting = false;
                    _tskRecv = new Task(Recv);
                    _tskRecv.Start();
                    _port.DtrEnable = true; // DTR = ON 
                    PrpConnectName = _port.PortName;
                    _isConnected = true;
                }
            }
        
            return strErrMsg;
        }

        /// <summary>
        /// ポートをクローズ
        /// </summary>
        public override void Disconnect()
        {
            lock (_lockPort) // ポートのアクセスの排他
            {
                if (_tskRecv != null)
                {
                    // コマンド送信の応答待ちを中止する
                    PrpResEvent.Set(); 
                    _isDisconnecting = true;
                    // 受信タスクの終了を待つ
                    _tskRecv.Wait(RECV_TASK_END_TIMEOUT);
                    _tskRecv = null;
                    if (_port.IsOpen) // ポートがオープン済みの場合
                    {
                        try
                        {
                            // ポートをクローズ
                            _port.Close();
                        }
                        catch { }
                    }
                    _isConnected = false;
                }
            }
        }
        
        /// <summary>
        /// ポートがオープン済みか否かを取得
        /// </summary>
        public override bool IsConnected()
        {
            bool bRet = false;

            lock (_lockPort) // ポートのアクセスの排他
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
        /// シリアル送信
        /// </summary>
        protected override string Send(byte[] buf)
        {
            string strErrMsg = null;

            lock (_lockPort) // ポートのアクセスの排他
            {
                if (false == IsConnected())
                {
                    strErrMsg = "Not connected.";
                }
                else
                {
                    try
                    {
                        // シリアル送信
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
        /// シリアル受信データが存在するかを確認
        /// </summary>
        protected override bool IsExistRecvData()
        {
            bool bRet = false;
            int readSize = 0;

            lock (_lockPort) // ポートのアクセスの排他
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
        /// シリアル受信データを1byte取り出し
        /// </summary>
        protected override bool ReadByte(out byte data)
        {
            bool bRet = false;
            int readData = -1;

            data = 0;

            lock (_lockPort) // ポートのアクセスの排他
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
