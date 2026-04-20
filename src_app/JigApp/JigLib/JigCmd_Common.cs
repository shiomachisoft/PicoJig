// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// Message when canceling the wait for a response to the sent command / 送信コマンドの応答待ちをキャンセルした時のメッセージ
        /// </summary>
        public const string STR_MSG_WAIT_RES_CANCEL = "Waiting for a response to the sent command has been canceled.";

        /// <summary>
        /// Maximum size of the data part in the frame / フレーム中のデータ部の最大サイズ
        /// </summary>
        /// <remarks>
        /// Match the value of FRM_DATA_MAX_SIZE on the FW side / FW側のFRM_DATA_MAX_SIZEの値に合わせる
        /// </remarks>
        protected const int FRM_DATA_MAX_SIZE = 1024;
        /// <summary>
        /// Maximum size of the response frame / 応答フレームの最大サイズ
        /// </summary>
        /// <remarks>
        /// The response frame is larger than the notification frame / 応答フレームの方が通知フレームよりサイズが大きい
        /// </remarks>
        protected const int FRM_RES_SIZE = FRM_DATA_MAX_SIZE + 11;
        /// <summary>
        /// Receive task end wait timeout (ms) / 受信タスクの終了待ちタイムアウト時間(ms)
        /// </summary>
        protected const int RECV_TASK_END_TIMEOUT = 20000;
        /// <summary>
        /// Response frame reception timeout (ms) / 応答フレーム受信タイムアウト(ms)
        /// </summary>
        protected const int FRM_RES_TIMEOUT = 10000;

        /// <summary>
        /// Frame end timeout (ms) / フレームエンドタイムアウト(ms) 
        /// </summary>
        /// <remarks>
        /// Discard the receive frame if the end of the frame is not received within the frame end timeout after receiving the header of the receive frame (response/notification frame) / 受信フレーム(応答/通知フレーム)のヘッダを受信後、フレームエンドタイムアウトの時間が経過してもそのフレームの末端を受信しなかった場合、そのフレームは破棄する
        /// </remarks>
        private const int FRM_END_TIMEOUT = 1500;
        /// <summary>
        /// Delay for the while loop in Recv() (ms) / Recv()のwhile文のディレイ(ms)
        /// </summary>
        private const int RECV_DELAY = 50;
        /// <summary>
        /// FW error messages / FWエラーメッセージ
        /// </summary>
        /// <remarks>
        /// Match the define in Common.h of the FW source / FWのソースのCommon.hのdefineに合わせる
        /// </remarks>
        private readonly string[] FW_ERR_MSG_ARY =
        {
            // The microcontroller was reset due to WDT timeout. / WDTタイムアウトでマイコンがリセットした。
            "The microcontroller was reset due to WDT timeout.",
            "UART:Framing error",
            "UART:Parity error",
            "UART:Break error",
            "UART:Overrun error",
            "I2C:address not acknowledged, or, no device present.",
            // Timeout in I2C communication / I2C通信でタイムアウト
            "Timeout in I2C communication",
            // The requested data was discarded because there was no space in the buffer (USB/wireless transmission) / バッファに空きがないので要求データを破棄した(USB/無線送信)
            "The requested data was discarded because there was no space in the buffer (USB/wireless transmission)",
            // The requested data was discarded because there was no space in the buffer (UART transmission) / バッファに空きがないので要求データを破棄した(UART送信)
            "The requested data was discarded because there was no space in the buffer (UART transmission)",
            // The requested data was discarded because there was no space in the buffer (UART reception) / バッファに空きがないので要求データを破棄した(UART受信)
            "The requested data was discarded because there was no space in the buffer (UART reception)",
            // The requested data was discarded because there was no space in the buffer (I2C transmission/reception) / バッファに空きがないので要求データを破棄した(I2C送信/受信)
            "The requested data was discarded because there was no space in the buffer (I2C transmission/reception)",
            // The requested data was discarded because there was no space in the buffer (wireless reception) / バッファに空きがないので要求データを破棄した(無線受信)
            "The requested data was discarded because there was no space in the buffer (wireless reception)",
            // Wireless transmission (BLE/TCP) failed. / 無線送信が失敗した
            "Wireless transmission (BLE/TCP) failed."
        };

        /// <summary>
        /// Definition of the header part in the frame / フレーム中のヘッダ部の定義
        /// </summary>
        protected enum E_FRM_HEADER : byte
        {
            /// <summary>
            /// Request frame / 要求フレーム
            /// </summary>
            REQ = 0xA0,
            /// <summary>
            /// Response frame / 応答フレーム
            /// </summary>
            RES,
            /// <summary>
            /// Notification frame (UART receive) / 通知フレーム(UART受信)
            /// </summary>
            NOTIFY_UART_RECV
        }

        /// <summary>
        /// Definition of the command part in the frame / フレーム中のコマンド部の定義
        /// </summary>
        protected enum E_FRM_CMD : UInt16
        {
            /// <summary>
            /// Get FW info / FW情報取得
            /// </summary>
            GET_FW_INFO = 0x0001,
            /// <summary>
            /// Set GPIO config / GPIO設定変更
            /// </summary>
            SET_GPIO_CONFIG,
            /// <summary>
            /// Get GPIO config / GPIO設定取得
            /// </summary>
            GET_GPIO_CONFIG,
            /// <summary>
            /// Get GPIO input/output value / GPIO入出力値取得
            /// </summary>
            GET_GPIO,
            /// <summary>
            /// Put GPIO / GPIO出力
            /// </summary>
            PUT_GPIO,
            /// <summary>
            /// Get ADC / ADC入力
            /// </summary>
            GET_ADC,
            /// <summary>
            /// Set UART config / UART設定変更
            /// </summary>
            SET_UART_CONFIG,
            /// <summary>
            /// Get UART config / UART設定取得 
            /// </summary>
            GET_UART_CONFIG,
            /// <summary>
            /// Send UART / UART送信
            /// </summary>
            SEND_UART,
            /// <summary>
            /// Set SPI config / SPI設定変更
            /// </summary>
            SET_SPI_CONFIG,
            /// <summary>
            /// Get SPI config / SPI設定取得
            /// </summary>
            GET_SPI_CONFIG,
            /// <summary>
            /// Send/Recv SPI / SPIマスタ送受信
            /// </summary>
            SENDRECV_SPI,
            /// <summary>
            /// Set I2C config / I2C設定変更
            /// </summary>
            SET_I2C_CONFIG,
            /// <summary>
            /// Get I2C config / I2C設定取得
            /// </summary>
            GET_I2C_CONFIG,
            /// <summary>
            /// Send I2C / I2Cマスタ送信
            /// </summary>
            SEND_I2C,
            /// <summary>
            /// Recv I2C / I2Cマスタ受信
            /// </summary>
            RECV_I2C,
            /// <summary>
            /// Start PWM / PWM開始
            /// </summary>
            START_PWM,
            /// <summary>
            /// Stop PWM / PWM停止
            /// </summary>
            STOP_PWM,
            /// <summary>
            /// Get FW err / FWエラー取得
            /// </summary>
            GET_FW_ERR,
            /// <summary>
            /// Clear FW err / FWエラークリア
            /// </summary>
            CLEAR_FW_ERR,
            /// <summary>
            /// Erase FLASH / FLASH消去
            /// </summary>
            ERASE_FLASH,
            /// <summary>
            /// Set Network config / ネットワーク設定変更
            /// </summary>
            SET_NW_CONFIG,
            /// <summary>
            /// Get Network config / ネットワーク設定取得
            /// </summary>
            GET_NW_CONFIG,
            /// <summary>
            /// Set Network config 2 / ネットワーク設定変更2
            /// </summary>
            SET_NW_CONFIG2,
            /// <summary>
            /// Get Network config 2 / ネットワーク設定取得2
            /// </summary>
            GET_NW_CONFIG2,
            /// <summary>
            /// Set Network config 3 / ネットワーク設定変更3
            /// </summary>
            SET_NW_CONFIG3,
            /// <summary>
            /// Get Network config 3 / ネットワーク設定取得3
            /// </summary>
            GET_NW_CONFIG3,
            /// <summary>
            /// Reset MCU / マイコンリセット
            /// </summary>
            RESET_MCU,
        }

        /// <summary>
        /// Definition of the error code part in the response frame / 応答フレーム中のエラーコード部の定義
        /// </summary>
        protected enum E_FRM_ERRCODE : UInt16
        {
            /// <summary>
            /// Success / 成功
            /// </summary>
            SUCCESS = 0x0000,
            /// <summary>
            /// Invalid data size in request / 要求中のデータ部のサイズが不正
            /// </summary>
            DATA_SIZE,
            /// <summary>
            /// Invalid argument in request / 要求中の引数が不正
            /// </summary>
            PARAM,
            /// <summary>
            /// Requested data was discarded because there is no space in the buffer / バッファに空きがないので要求データを破棄した
            /// </summary>
            BUF_NOT_ENOUGH,
            /// <summary>
            /// I2C:address not acknowledged, or, no device present.
            /// </summary>
            FRM_ERR_I2C_NO_DEVICE
        }

        /// <summary>
        /// Request frame structure / 要求フレーム構造体
        /// </summary>
        protected struct ST_FRM_REQ_FRAME
        {
            /// <summary>
            /// Header (1byte) / ヘッダ(1byte)
            /// </summary>
            public E_FRM_HEADER header;
            /// <summary>
            /// Sequence number (2byte) / シーケンス番号(2byte)
            /// </summary>
            public UInt16 seqNo;
            /// <summary>
            /// Command (2byte) / コマンド(2byte)
            /// </summary>
            public E_FRM_CMD cmd;
            /// <summary>
            /// Data size (2byte) / データサイズ(2byte)
            /// </summary>
            public UInt16 dataSize;
            /// <summary>
            /// Data / データ
            /// </summary>
            public byte[] aData;
            /// <summary>
            /// Checksum (2byte) / チェックサム(2byte)
            /// </summary>
            public UInt16 checksum;
        }

        /// <summary>
        /// Response frame structure / 応答フレーム構造体
        /// </summary>
        protected struct ST_FRM_RES_FRAME
        {
            /// <summary>
            /// Header (1byte) / ヘッダ(1byte)
            /// </summary>
            public E_FRM_HEADER header;
            /// <summary>
            /// Sequence number (2byte) / シーケンス番号(2byte)
            /// </summary>
            public UInt16 seqNo;
            /// <summary>
            /// Command (2byte) / コマンド(2byte)
            /// </summary>
            public E_FRM_CMD cmd;
            /// <summary>
            /// Error code (2byte) / エラーコード(2byte) 
            /// </summary>
            public E_FRM_ERRCODE errCode;
            /// <summary>
            /// Data size (2byte) / データサイズ(2byte)
            /// </summary>
            public UInt16 dataSize;
            /// <summary>
            /// Data / データ
            /// </summary>
            public byte[] aData;
            /// <summary>
            /// Checksum (2byte) / チェックサム(2byte)
            /// </summary>
            public UInt16 checksum;
        }

        /// <summary>
        /// Notification frame structure / 通知フレーム構造体
        /// </summary>
        protected struct ST_FRM_NOTIFY_FRAME
        {
            /// <summary>
            /// Header (1byte) / ヘッダ(1byte)
            /// </summary>
            public E_FRM_HEADER header;
            /// <summary>
            /// Data size (2byte) / データサイズ(2byte)
            /// </summary>
            public UInt16 dataSize;
            /// <summary>
            /// Data / データ
            /// </summary>
            public byte[] aData;
            /// <summary>
            /// Checksum (2byte) / チェックサム(2byte)
            /// </summary>
            public UInt16 checksum;
        }

        /// <summary>
        /// Connection destination name / 接続先名
        /// </summary>
        public string PrpConnectName { get; set; } = string.Empty;
        /// <summary>
        /// Queue for UART receive data / UART受信データのキュー
        /// </summary>
        public BlockingCollection<byte> PrpUartRecvDataQue { get; set; } = new BlockingCollection<byte>(4096);
        
        /// <summary>
        /// Whether connected or not / 接続済みか否か
        /// </summary>
        protected bool _isConnected = false;
        /// <summary>
        /// Response frame reception event / 応答フレーム受信イベント
        /// </summary>
        protected ManualResetEvent PrpResEvent { get; set; } = new ManualResetEvent(false);
        /// <summary>
        /// Queue for response frames / 応答フレームのキュー
        /// </summary>
        protected BlockingCollection<ST_FRM_RES_FRAME> PrpResFrmQue { get; set; } = new BlockingCollection<ST_FRM_RES_FRAME>(1);
        /// <summary>
        /// Lock object for exclusive access to COM port/socket / COMポート/ソケットのアクセスを排他するためのロック用オブジェクト
        /// </summary>
        protected Object _lockPort = new Object();
        /// <summary>
        /// Lock object to prevent next transmission while waiting for response to transmission / 送信～応答待ち中は、次の送信をしないようにするためのロック用オブジェクト
        /// </summary>
        protected Object _lockSend = new Object();
        /// <summary>
        /// Lock object for exclusive access to resources related to receive process / 受信処理に関するリソースを排他するロック用オブジェクト
        /// </summary>
        protected Object _lockRecv = new Object();
        /// <summary>
        /// Whether disconnection process is currently running / 切断処理を実行中か否か
        /// </summary>
        protected bool _isDisconnecting = false;

        /// <summary>
        /// Sequence number / シーケンス番号
        /// </summary>
        private UInt16 _seqNo = 0;
        /// <summary>
        /// The last error message of the receive process / 受信処理の最後のエラーメッセージ
        /// </summary>
        private string _strLastRecvErrMsg = null;
        /// <summary>
        /// Receive size of the receive frame (response/notification frame) / 受信フレーム(応答/通知フレーム)の受信サイズ
        /// </summary>
        private int _recvSize = 0;
        /// <summary>
        /// Buffer for the receive frame (response/notification frame) / 受信フレーム(応答/通知フレーム)のバッファ
        /// </summary>
        /// <remarks>
        /// The response frame is larger than the notification frame / 応答フレームの方が通知フレームよりサイズが大きい
        /// </remarks>
        private byte[] _bufRecvFrm = new byte[FRM_RES_SIZE];
        /// <summary>
        /// Timer for frame end timeout / フレームエンドタイムアウト用タイマー
        /// </summary>
        private Timer _timerFrameEnd = null;

        /// <summary>
        /// Constructor / コンストラクタ
        /// </summary>
        public JigCmd()
        {
            // Register callback for frame end timeout / フレームエンドタイムアウトのコールバックを登録
            _timerFrameEnd = new Timer(FrameEndTimeoutCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Calculate checksum / チェックサムを計算
        /// </summary>
        private UInt16 CalcChecksum(byte[] buf, int size)
        {
            int i;
            UInt16 checksum = 0;

            unchecked // Prevent OverflowException / オーバーフロー例外を防止
            {
                for (i = 0; i < size; i++)
                {
                    checksum += buf[i];
                }
            }

            return checksum;
        }
    }
}
