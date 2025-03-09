// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Net.Http;
using System.IO;

namespace JigLib
{
    public class JigTcpClient : JigCmd
    {
        /// <summary>
        /// ソケットポート番号
        /// </summary>
        private const int SOCKET_PORT = 7777;
        /// <summary>
        /// 接続タイムアウト時間(ms)
        /// </summary>
        private const int TCP_CONNECT_TIMEOUT = 5000;
        /// <summary>
        /// ソケット書き込みタイムアウト時間(ms)
        /// </summary>
        private const int SOCKET_WRITE_TIMEOUT = 3000;
        /// <summary>
        /// ソケット読み取りタイムアウト時間(ms)
        /// </summary>
        private const int SOCKET_READ_TIMEOUT = 3000;
        /// <summary>
        /// TCP KeepAliveのインターバル(ms)
        /// </summary>
        private const int TCP_KEEP_ALIVE_INTERVAL = FRM_RES_TIMEOUT - 1000;

        /// <summary>
        /// ソケットのインスタンス
        /// </summary>
        private TcpClient _client = null;
        /// <summary>
        /// ストリーム
        /// </summary>
        private NetworkStream _stream = null;
        /// <summary>
        /// 受信タスク
        /// </summary>
        private Task _tskRecv = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JigTcpClient()
        {
        }

        /// <summary>
        /// ソケットを生成し、サーバーへ接続する
        /// </summary>
        public override string Connect(Object objServerIpAddr)
        {
            bool bRet = false;
            string strErrMsg = null;
            string strServerIpAddr = (string)objServerIpAddr; // サーバーのIPアドレス

            lock (_lockPort) // ソケットのアクセスの排他
            {
                try
                {
                    TcpClient client = new TcpClient();
                    // サーバーへ接続する
                    Task task = client.ConnectAsync(strServerIpAddr, SOCKET_PORT);
                    Task[] tasks = { task };
                    bRet = Task.WaitAll(tasks, TCP_CONNECT_TIMEOUT);
                    if (true == bRet) // 接続に成功 
                    {
                        //client.NoDelay = true; // Nagleアルゴリズムを無効化
                        _stream = client.GetStream(); // ストリームを取得
                        _stream.WriteTimeout = SOCKET_WRITE_TIMEOUT; // 書き込みタイムアウト時間
                        _stream.ReadTimeout = SOCKET_READ_TIMEOUT;   // 読み取りタイムアウト時間 
                        _client = client;
                        // TCP KeepAliveを有効にする
                        ServicePointManager.SetTcpKeepAlive(true, TCP_KEEP_ALIVE_INTERVAL, TCP_KEEP_ALIVE_INTERVAL);
                    }
                    else // 接続に失敗
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
                        strErrMsg = "connection timeout.";
                    }
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
                    PrpConnectName = "Server IP:" + strServerIpAddr;
                    _isConnected = true;
                }
            }
            
            return strErrMsg;     
        }

        /// <summary>
        /// ソケットをクローズ
        /// </summary>
        public override void Disconnect()
        {
            lock (_lockPort) // ソケットのアクセスの排他
            {
                if (_tskRecv != null)
                {
                    // コマンド送信の応答待ちを中止する
                    PrpResEvent.Set();
                    _isDisconnecting = true;
                    // 受信タスクの終了を待つ
                    _tskRecv.Wait(RECV_TASK_END_TIMEOUT);
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
        /// サーバーと接続済みか否かを取得
        /// </summary>
        public override bool IsConnected()
        {
            bool bRet = false;

            lock (_lockPort) // ソケットのアクセスの排他
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
        /// ソケット送信
        /// </summary>
        protected override string Send(byte[] buf)
        {
            string strErrMsg = null;

            lock (_lockPort) // ソケットのアクセスの排他
            {
                if (false == IsConnected())
                {
                    strErrMsg = "Not connected.";
                }
                else
                {
                    try
                    {
                        // ソケット送信
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
        /// ソケット受信データが存在するかを確認
        /// </summary>
        protected override bool IsExistRecvData()
        {
            bool bRet = false;

            lock (_lockPort) // ソケットのアクセスの排他
            {
                if (_stream != null)
                {
                    bRet = _stream.DataAvailable;
                }
            }

            return bRet;
        }

        /// <summary>
        /// ソケット受信データを1byte取り出し
        /// </summary>
        protected override bool ReadByte(out byte data)
        {
            bool bRet = false;
            int readData = -1;

            data = 0;

            lock (_lockPort) // ソケットのアクセスの排他
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
    }
}
