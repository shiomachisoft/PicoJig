// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// 送信コマンドの応答待ちをキャンセルした時のメッセージ
        /// </summary>
        public const string STR_MSG_WAIT_RES_CANCEL = "Waiting for a response to send a command has been canceled.";

        /// <summary>
        /// 応答フレームの最大サイズ
        /// </summary>
        protected const int FRM_RES_SIZE = 311;
        /// <summary>
        /// フレーム中のデータ部の最大サイズ
        /// </summary>
        /// <remarks>
        /// FW側のFRM_DATA_MAX_SIZEの値に合わせる
        /// </remarks>
        protected const int FRM_DATA_MAX_SIZE = 300;
        /// <summary>
        /// 受信タスクの終了待ちタイムアウト時間(ms)
        /// </summary>
        protected const int RECV_TASK_END_TIMEOUT = 20000;
        /// <summary>
        /// 応答フレーム受信タイムアウト(ms)
        /// </summary>
        protected const int FRM_RES_TIMEOUT = 10000;

        /// <summary>
        ///フレームエンドタイムアウト(ms) 
        /// </summary>
        /// <remarks>
        /// 受信フレーム(応答/通知フレーム)のヘッダを受信後、フレームエンドタイムアウトの時間が経過してもそのフレームの末端を受信しなかった場合、そのフレームは破棄する
        /// </remarks>
        private const int FRM_END_TIMEOUT = 1500;
        /// <summary>
        /// Recv()のwhile文のディレイ(ms)
        /// </summary>
        private const int RECV_DELAY = 50;
        /// <summary>
        /// FWエラーメッセージ
        /// </summary>
        /// <remarks>
        /// FWのソースのCommon.hのdefineに合わせる
        /// </remarks>
        private readonly string[] FW_ERR_MSG_ARY =
        {
            //"WDTタイムアウトでマイコンがリセットした。" ,
            "The microcontroller was reset due to WDT timeout.",
            "UART:Framing error",
            "UART:Parity error",
            "UART:Break error",
            "UART:Overrun error",
            "I2C:address not acknowledged, or, no device present.",
            //"I2C通信でタイムアウト",
            "Timeout in I2C communication",
            //"バッファに空きがないので要求データを破棄した(USB/無線送信)",
            "The requested data was discarded because there was no space in the buffer (USB/wireless transmission)",
            //"バッファに空きがないので要求データを破棄した(UART送信)",
            "The requested data was discarded because there was no space in the buffer (UART transmission)",
            //"バッファに空きがないので要求データを破棄した(UART受信)",
            "The requested data was discarded because there was no space in the buffer (UART reception)",
            //"バッファに空きがないので要求データを破棄した(I2C送信/受信)",
            "The requested data was discarded because there was no space in the buffer (I2C transmission/reception)",
            //"バッファに空きがないので要求データを破棄した(無線受信)"
            "Requested data was discarded because there was no space in the buffer (wireless reception)",
        };

        /// <summary>
        /// フレーム中のヘッダ部の定義
        /// </summary>
        protected enum E_FRM_HEADER : byte
        {
            /// <summary>
            /// 要求フレーム
            /// </summary>
            REQ = 0xA0,
            /// <summary>
            /// 応答フレーム
            /// </summary>
            RES,
            /// <summary>
            /// 通知フレーム(UART受信)
            /// </summary>
            NOTIFY_UART_RECV
        }

        /// <summary>
        /// フレーム中のコマンド部の定義
        /// </summary>
        protected enum E_FRM_CMD : UInt16
        {
            /// <summary>
            /// FW情報取得
            /// </summary>
            GET_FW_INFO = 0x0001,
            /// <summary>
            /// GPIO設定変更
            /// </summary>
            SET_GPIO_CONFIG,
            /// <summary>
            /// GPIO設定取得
            /// </summary>
            GET_GPIO_CONFIG,
            /// <summary>
            /// GPIO入力
            /// </summary>
            GET_GPIO,
            /// <summary>
            /// GPIO出力
            /// </summary>
            PUT_GPIO,
            /// <summary>
            /// ADC入力
            /// </summary>
            GET_ADC,
            /// <summary>
            /// UART設定変更
            /// </summary>
            SET_UART_CONFIG,
            /// <summary>
            /// UART設定取得 
            /// </summary>
            GET_UART_CONFIG,
            /// <summary>
            /// UART送信
            /// </summary>
            SEND_UART,
            /// <summary>
            /// SPI設定変更
            /// </summary>
            SET_SPI_CONFIG,
            /// <summary>
            /// SPI設定取得
            /// </summary>
            GET_SPI_CONFIG,
            /// <summary>
            /// SPIマスタ送受信
            /// </summary>
            SENDRECV_SPI,
            /// <summary>
            /// I2C設定変更
            /// </summary>
            SET_I2C_CONFIG,
            /// <summary>
            /// I2C設定取得
            /// </summary>
            GET_I2C_CONFIG,
            /// <summary>
            /// I2Cマスタ送信
            /// </summary>
            SEND_I2C,
            /// <summary>
            /// I2Cマスタ受信
            /// </summary>
            RECV_I2C,
            /// <summary>
            /// PWM開始
            /// </summary>
            START_PWM,
            /// <summary>
            /// PWM停止
            /// </summary>
            STOP_PWM,
            /// <summary>
            /// エラー取得
            /// </summary>
            GET_FW_ERR,
            /// <summary>
            /// エラークリア
            /// </summary>
            CLEAR_FW_ERR,
            /// <summary>
            /// FLASH消去
            /// </summary>
            ERASE_FLASH,
            /// <summary>
            /// ネットワーク設定変更
            /// </summary>
            SET_NW_CONFIG,
            /// <summary>
            /// ネットワーク設定取得
            /// </summary>
            GET_NW_CONFIG,
            /// <summary>
            /// ネットワーク設定変更2
            /// </summary>
            SET_NW_CONFIG2,
            /// <summary>
            /// ネットワーク設定取得2
            /// </summary>
            GET_NW_CONFIG2,
        }

        /// <summary>
        /// 応答フレーム中のエラーコード部の定義
        /// </summary>
        protected enum E_FRM_ERRCODE : UInt16
        {
            /// <summary>
            /// 成功
            /// </summary>
            SUCCESS = 0x0000,
            /// <summary>
            /// 要求中のデータ部のサイズが不正
            /// </summary>
            DATA_SIZE,
            /// <summary>
            /// 要求中の引数が不正
            /// </summary>
            PARAM,
            /// <summary>
            /// バッファに空きがないので要求データを破棄した
            /// </summary>
            BUF_NOT_ENOUGH,
            /// <summary>
            /// I2C:address not acknowledged, or, no device present.
            /// </summary>
            FRM_ERR_I2C_NO_DEVICE
        }

        /// <summary>
        /// 要求フレーム構造体
        /// </summary>
        protected struct ST_FRM_REQ_FRAME
        {
            /// <summary>
            /// ヘッダ(1byte)
            /// </summary>
            public E_FRM_HEADER header;
            /// <summary>
            /// シーケンス番号(2byte)
            /// </summary>
            public UInt16 seqNo;
            /// <summary>
            /// コマンド(2byte)
            /// </summary>
            public E_FRM_CMD cmd;
            /// <summary>
            /// データサイズ(2byte)
            /// </summary>
            public UInt16 dataSize;
            /// <summary>
            /// データ
            /// </summary>
            public byte[] aData;
            /// <summary>
            /// チェックサム(2byte)
            /// </summary>
            public UInt16 checksum;
        }

        /// <summary>
        /// 応答フレーム構造体
        /// </summary>
        protected struct ST_FRM_RES_FRAME
        {
            /// <summary>
            /// ヘッダ(1byte)
            /// </summary>
            public E_FRM_HEADER header;
            /// <summary>
            /// シーケンス番号(2byte)
            /// </summary>
            public UInt16 seqNo;
            /// <summary>
            /// コマンド(2byte)
            /// </summary>
            public E_FRM_CMD cmd;
            /// <summary>
            /// エラーコード(2byte) 
            /// </summary>
            public E_FRM_ERRCODE errCode;
            /// <summary>
            /// データサイズ(2byte)
            /// </summary>
            public UInt16 dataSize;
            /// <summary>
            /// データ
            /// </summary>
            public byte[] aData;
            /// <summary>
            /// チェックサム(2byte)
            /// </summary>
            public UInt16 checksum;
        }

        /// <summary>
        /// 通知フレーム構造体
        /// </summary>
        protected struct ST_FRM_NOTIFY_FRAME
        {
            /// <summary>
            /// ヘッダ(1byte)
            /// </summary>
            public E_FRM_HEADER header;
            /// <summary>
            /// データサイズ(2byte)
            /// </summary>
            public UInt16 dataSize;
            /// <summary>
            /// データ
            /// </summary>
            public byte[] aData;
            /// <summary>
            /// チェックサム(2byte)
            /// </summary>
            public UInt16 checksum;
        }

        /// <summary>
        /// 接続先名
        /// </summary>
        public string PrpConnectName { get; set; } = string.Empty;
        /// <summary>
        /// UART受信データのキュー
        /// </summary>
        public BlockingCollection<byte> PrpUartRecvDataQue { get; set; } = new BlockingCollection<byte>(4096);
        
        /// <summary>
        /// 接続済みか否か
        /// </summary>
        protected bool _isConnected = false;
        /// <summary>
        /// 応答フレーム受信イベント
        /// </summary>
        protected ManualResetEvent PrpResEvent { get; set; } = new ManualResetEvent(false);
        /// <summary>
        /// 応答フレームのキュー
        /// </summary>
        protected BlockingCollection<ST_FRM_RES_FRAME> PrpResFrmQue { get; set; } = new BlockingCollection<ST_FRM_RES_FRAME>(1);
        /// <summary>
        /// COMポート/ソケットのアクセスを排他するためのロック用オブジェクト
        /// </summary>
        protected Object _lockPort = new Object();
        /// <summary>
        /// 送信～応答待ち中は、次の送信をしないようにするためのロック用オブジェクト
        /// </summary>
        protected Object _lockSend = new Object();
        /// <summary>
        /// 受信処理に関するリソースを排他するロック用オブジェクト
        /// </summary>
        protected Object _lockRecv = new Object();
        /// <summary>
        /// 切断処理を実行中か否か
        /// </summary>
        protected bool _isDisconnecting = false;

        /// <summary>
        /// シーケンス番号
        /// </summary>
        private UInt16 _seqNo = 0;
        /// <summary>
        /// 受信処理の最後のエラーメッセージ
        /// </summary>
        private string _strLastRecvErrMsg = null;
        /// <summary>
        /// 受信フレーム(応答/通知フレーム)の受信サイズ
        /// </summary>
        private int _recvSize = 0;
        /// <summary>
        /// 受信フレーム(応答/通知フレーム)のバッファ
        /// </summary>
        /// <remarks>
        /// 応答フレームの方が通知フレームよりサイズが大きい
        /// </remarks>
        private byte[] _bufRecvFrm = new byte[FRM_RES_SIZE];
        /// <summary>
        /// フレームエンドタイムアウト用タイマー
        /// </summary>
        private Timer _timerFrameEnd = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JigCmd()
        {
            // フレームエンドタイムアウトのコールバックを登録
            _timerFrameEnd = new Timer(FrameEndTimeoutCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// チェックサムを計算
        /// </summary>
        private UInt16 CalcChecksum(byte[] buf, int size)
        {
            int i;
            UInt16 checksum = 0;

            for (i = 0; i < size; i++)
            {
                checksum += buf[i];
            }

            return checksum;
        }
    }
}
