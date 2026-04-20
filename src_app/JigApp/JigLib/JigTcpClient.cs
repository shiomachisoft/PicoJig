// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace JigLib
{
    public class JigTcpClient : JigCmd
    {
        /// <summary>
        /// Socket port number / ソケットポート番号
        /// </summary>
        private const int SOCKET_PORT = 7777;
        /// <summary>
        /// Connection timeout (ms) / 接続タイムアウト時間(ms)
        /// </summary>
        private const int TCP_CONNECT_TIMEOUT = 5000;
        /// <summary>
        /// Socket write timeout (ms) / ソケット書き込みタイムアウト時間(ms)
        /// </summary>
        private const int SOCKET_WRITE_TIMEOUT = 3000;
        /// <summary>
        /// Socket read timeout (ms) / ソケット読み取りタイムアウト時間(ms)
        /// </summary>
        private const int SOCKET_READ_TIMEOUT = 3000;
        /// <summary>
        /// TCP KeepAlive interval (ms) / TCP KeepAliveのインターバル(ms)
        /// </summary>
        private const int TCP_KEEP_ALIVE_INTERVAL = FRM_RES_TIMEOUT - 1000;

        /// <summary>
        /// Socket instance / ソケットのインスタンス
        /// </summary>
        private TcpClient _client = null;
        /// <summary>
        /// Stream / ストリーム
        /// </summary>
        private NetworkStream _stream = null;
        /// <summary>
        /// Receive task / 受信タスク
        /// </summary>
        private Task _tskRecv = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public JigTcpClient()
        {
        }

        /// <summary>
        /// Create a socket and connect to the server / ソケットを生成し、サーバーへ接続する
        /// </summary>
        /// <param name="objServerIpAddr">
        /// IP address of the server to connect to (e.g. "192.168.1.10") / 接続先サーバーのIPアドレス(例: "192.168.1.10")
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public override string Connect(Object objServerIpAddr)
        {
            bool bRet = false;
            string strErrMsg = null;
            string strServerIpAddr = (string)objServerIpAddr; // Server IP address / サーバーのIPアドレス

            lock (_lockPort) // Exclusive access to socket / ソケットのアクセスの排他
            {
                TcpClient client = null;
                try
                {
                    client = new TcpClient();
                    // Connect to server / サーバーへ接続する
                    Task task = client.ConnectAsync(strServerIpAddr, SOCKET_PORT);
                    bRet = task.Wait(TCP_CONNECT_TIMEOUT);
                    if (bRet && client.Connected) // Successful connection / 接続に成功 
                    {
                        //client.NoDelay = true; // Disable Nagle algorithm / Nagleアルゴリズムを無効化
                        _stream = client.GetStream(); // Get stream / ストリームを取得
                        _stream.WriteTimeout = SOCKET_WRITE_TIMEOUT; // Write timeout / 書き込みタイムアウト時間
                        _stream.ReadTimeout = SOCKET_READ_TIMEOUT;   // Read timeout / 読み取りタイムアウト時間 
                        _client = client;
                        // Enable TCP KeepAlive / TCP KeepAliveを有効にする
                        try
                        {
                            SetKeepAlive(_client.Client, true, TCP_KEEP_ALIVE_INTERVAL, TCP_KEEP_ALIVE_INTERVAL);
                        }
                        catch
                        {
                            // Continue connection even if KeepAlive setting fails / KeepAliveの設定に失敗しても接続自体は継続する
                        }
                    }
                    else // Connection failed / 接続に失敗
                    {
                        if (_stream != null)
                        {
                            _stream.Close();
                            _stream = null;
                        }
                        if (client != null)
                        {
                            client.Close();
                            client = null;
                        }
                        strErrMsg = "Connection timed out.";
                    }
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                    if (client != null)
                    {
                        client.Close();
                    }
                }

                if (strErrMsg == null)
                {
                    // Start receive task / 受信タスクを開始
                    _isDisconnecting = false;
                    _tskRecv = new Task(Recv);
                    _tskRecv.Start();
                    PrpConnectName = "Server IP:" + strServerIpAddr;
                    _isConnected = true;
                }
            }
            
            return strErrMsg;     
        }

        /// <summary>
        /// Close the socket / ソケットをクローズ
        /// </summary>
        public override void Disconnect()
        {
            Task waitTask = null;
            lock (_lockPort) // Exclusive access to socket / ソケットのアクセスの排他
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
                    if (_stream != null)
                    {
                        _stream.Close();
                        _stream = null;
                    }
                    if (_client != null)
                    {
                        _client.Close();
                        _client = null;
                    }
                    _isConnected = false;
                }
            }
        }

        /// <summary>
        /// Get whether connected to the server / サーバーと接続済みか否かを取得
        /// </summary>
        /// <returns>
        /// true: Connected, false: Not connected / true:接続済み, false:未接続
        /// </returns>
        public override bool IsConnected()
        {
            bool bRet = false;

            lock (_lockPort) // Exclusive access to socket / ソケットのアクセスの排他
            {
                if (_isConnected && (_client != null) && (_stream != null))
                {
                    try
                    {
                        bRet = _client.Connected;
                    }
                    catch { }
                }
            }

            return bRet;
        }

        /// <summary>
        /// Socket transmission / ソケット送信
        /// </summary>
        protected override string Send(byte[] buf)
        {
            string strErrMsg = null;

            lock (_lockPort) // Exclusive access to socket / ソケットのアクセスの排他
            {
                if (false == IsConnected())
                {
                    strErrMsg = "Not connected.";
                }
                else
                {
                    try
                    {
                        // Socket transmission / ソケット送信
                        _stream.Write(buf, 0, buf.Length);
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
        /// Check if socket receive data exists / ソケット受信データが存在するかを確認
        /// </summary>
        protected override bool IsExistRecvData()
        {
            bool bRet = false;

            lock (_lockPort) // Exclusive access to socket / ソケットのアクセスの排他
            {
                if (_stream != null)
                {
                    try
                    {
                        bRet = _stream.DataAvailable;
                    }
                    catch { }
                }
            }

            return bRet;
        }

        /// <summary>
        /// Extract 1 byte of socket receive data / ソケット受信データを1byte取り出し
        /// </summary>
        protected override bool ReadByte(out byte data)
        {
            bool bRet = false;
            int readData = -1;

            data = 0;

            lock (_lockPort) // Exclusive access to socket / ソケットのアクセスの排他
            {
                if (_stream != null)
                {
                    try
                    {
                        readData = _stream.ReadByte();
                    }
                    catch { };
                }
                if (readData >= 0)
                {
                    data = (byte)readData;
                    bRet = true;
                }
            }    

            return bRet;
        }

        /// <summary>
        /// Set TCP KeepAlive / TCP KeepAliveを設定
        /// </summary>
        private void SetKeepAlive(Socket socket, bool on, uint time, uint interval)
        {
            byte[] optionInValue = new byte[12];
            BitConverter.GetBytes(on ? 1u : 0u).CopyTo(optionInValue, 0);
            BitConverter.GetBytes(time).CopyTo(optionInValue, 4);
            BitConverter.GetBytes(interval).CopyTo(optionInValue, 8);
            socket.IOControl(IOControlCode.KeepAliveValues, optionInValue, null);
        }
    }
}
