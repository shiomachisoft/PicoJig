// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Threading;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// Get the last error message of the receive process / 受信処理の最後のエラーメッセージを取得
        /// </summary>
        /// <returns>
        /// Error message. Returns null if no error has occurred. / エラーメッセージ。エラーが発生していない場合はnullを返す。
        /// </returns>
        public string GetLastRecvErrMsg()
        {
            string strErrMsg;

            lock (_lockRecv) // Exclusive access to resources related to the receive process / 受信処理に関するリソースを排他する
            {
                strErrMsg = _strLastRecvErrMsg;
                _strLastRecvErrMsg = null; // Clear after reading / 読み取ったらクリアする
            }

            return strErrMsg;
        }

        /// <summary>
        /// Extract and analyze receive data to create a receive frame (response/notification frame) / 受信データを取り出し・解析して受信フレーム(応答/通知フレーム)を作成
        /// </summary>
        protected void Recv()
        {
            int dataSize = 0;   // Data size part in receive frame / 受信フレーム中のデータサイズ部
            byte data;          // Serial receive data (1byte) / シリアル受信データ(1byte)
            UInt16 expect;      // Expected checksum value / チェックサムの期待値
            ST_FRM_NOTIFY_FRAME stNotifyFrm; // Notification frame / 通知フレーム

            DiscardRecvFrame(); // Initialization / 初期化

            while (!_isDisconnecting)
            {
                bool isSleep = false;
                lock (_lockRecv) // Exclusive access to resources related to the receive process / 受信処理に関するリソースを排他する
                {
                    try
                    {
                        if (true == IsExistRecvData()) // If receive data exists / 受信データが存在する場合
                        {
                            // Extract 1 byte of receive data / 受信データを1byte取り出す
                            if (false == ReadByte(out data))
                            {
                                isSleep = true;
                                goto SkipParse;
                            }

                            // [Create a receive frame (response/notification frame) from receive data] / [受信データから受信フレーム(応答/通知フレーム)を作成する]

                            if (_recvSize == 0) // If receive data = header (header not stored yet) / 受信データ = ヘッダ の場合(ヘッダはまだ格納していない場合)
                            {
                                switch (data) // Header / ヘッダ
                                {
                                    case (byte)E_FRM_HEADER.RES:              // Response / 応答      
                                    case (byte)E_FRM_HEADER.NOTIFY_UART_RECV: // Notification frame (UART receive) / 通知フレーム(UART受信)
                                        // Zero-fill the receive frame buffer / 受信フレームのバッファをゼロ充填
                                        Array.Clear(_bufRecvFrm, 0, _bufRecvFrm.Length);
                                        // Store the header / ヘッダを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        // Start the frame end timeout timer / フレームエンドタイムアウト用タイマーを開始
                                        _timerFrameEnd.Change(FRM_END_TIMEOUT, Timeout.Infinite);
                                        break;
                                    default: // Invalid frame header / 不正なフレームヘッダ
                                        // Discard receive frame / 受信フレーム破棄
                                        DiscardRecvFrame();
                                        break;
                                }
                            }
                            else // If header is already stored / ヘッダは格納済みの場合
                            {
                                if (_bufRecvFrm[0] == (byte)E_FRM_HEADER.RES) // If header = response / ヘッダ = 応答 の場合
                                {
                                    if (_recvSize <= 8) // If receive data = sequence number/command/error code/data size / 受信データ= シーケンス番号/コマンド/エラーコード/データサイズ の場合
                                    {
                                        // Store sequence number/command/error code/data size / シーケンス番号/コマンド/エラーコード/データサイズを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == 9) // If data size is already stored / データサイズは格納済みの場合
                                        {
                                            // Check if data size does not exceed the maximum value / データサイズが最大値を超えてないかをチェック
                                            dataSize = _bufRecvFrm[8] << 8 | _bufRecvFrm[7];
                                            if (dataSize > FRM_DATA_MAX_SIZE) // If data size exceeds the maximum value / データサイズが最大値を超えている場合
                                            {
                                                // Discard receive frame / 受信フレーム破棄
                                                DiscardRecvFrame();
                                            }
                                        }
                                    }
                                    else if (_recvSize <= (9 + dataSize + 2)) // If receive data = data/checksum / 受信データ = データ/チェックサム の場合
                                    {
                                        // Store data/checksum / データ/チェックサムを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == (9 + dataSize + 2)) // If checksum is already stored / チェックサムは格納済みの場合
                                        {
                                            // Checksum verification / チェックサム検査
                                            expect = (UInt16)(((UInt16)_bufRecvFrm[_recvSize - 1]) << 8 | (UInt16)_bufRecvFrm[_recvSize - 2]);
                                            if (ChecksumTest(_bufRecvFrm, _recvSize - 2, expect)) // If checksum verification is OK / チェックサム検査がOKの場合
                                            {
                                                // Queue response frame / 応答フレームをキューイング
                                                ST_FRM_RES_FRAME stResFrm = ConvertByteArrayToResponseFrame(_bufRecvFrm);
                                                if (PrpResFrmQue.TryAdd(stResFrm))
                                                {
                                                    // Notify response frame reception via event / 応答フレーム受信をイベント通知
                                                    PrpResEvent.Set();
                                                }
                                            }
                                            // Receive frame analysis complete / 受信フレーム解析完了
                                            DiscardRecvFrame();
                                        }
                                    }
                                    else
                                    {
                                        // No processing / 無処理
                                    }
                                }
                                else // If header = notification / ヘッダ = 通知 の場合
                                {
                                    if (_recvSize <= 2) // If receive data = data size / 受信データ = データサイズ の場合
                                    {
                                        // Store data size / データサイズを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == 3) // If data size is already stored / データサイズは格納済みの場合
                                        {
                                            // Check if data size does not exceed the maximum value / データサイズが最大値を超えてないかをチェック
                                            dataSize = _bufRecvFrm[2] << 8 | _bufRecvFrm[1];
                                            if (dataSize > FRM_DATA_MAX_SIZE) // If data size exceeds the maximum value / データサイズが最大値を超えている場合
                                            {
                                                // Discard receive frame / 受信フレーム破棄
                                                DiscardRecvFrame();
                                            }
                                        }
                                    }
                                    else if (_recvSize <= (3 + dataSize + 2)) // If receive data = data/checksum / 受信データ = データ/チェックサムの場合
                                    {
                                        // Store data/checksum / データ/チェックサムを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == (3 + dataSize + 2)) // If checksum is already stored / チェックサムは格納済みの場合
                                        {
                                            // Checksum verification / チェックサム検査
                                            expect = (UInt16)(((UInt16)_bufRecvFrm[_recvSize - 1]) << 8 | (UInt16)_bufRecvFrm[_recvSize - 2]);
                                            if (ChecksumTest(_bufRecvFrm, _recvSize - 2, expect)) // If checksum verification is OK / チェックサム検査がOKの場合
                                            {
                                                switch (_bufRecvFrm[0]) // Header / ヘッダ
                                                {
                                                    case (byte)E_FRM_HEADER.NOTIFY_UART_RECV: // Notification frame (UART receive) / 通知フレーム(UART受信)
                                                        // Get notification frame (UART receive) / 通知フレーム(UART受信)を取得 
                                                        stNotifyFrm = ConvertByteArrayToNotifyFrame(_bufRecvFrm);
                                                        // Queue UART receive data / UART受信データをキューイング
                                                        for (int i = 0; i < stNotifyFrm.dataSize; i++)
                                                        {
                                                            PrpUartRecvDataQue.TryAdd(stNotifyFrm.aData[i]);
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            // Receive frame analysis complete / 受信フレーム解析完了
                                            DiscardRecvFrame();
                                        }
                                    }
                                    else
                                    {
                                        // No processing / 無処理
                                    }
                                }
                            }
                        }
                        else // If receive data does not exist / 受信データが存在しない場合
                        {
                            isSleep = true;
                        }
                    SkipParse:;
                    }
                    catch (Exception ex) // If a port error occurs / ポートのエラーが発生した場合
                    {
                        _strLastRecvErrMsg = ex.Message;
                        isSleep = true;
                    }
                }

                if (isSleep)
                {
                    Thread.Sleep(RECV_DELAY);
                }
            }
        }

        /// <summary>
        /// Convert byte array to response frame and return / byte型配列を応答フレームに変換して返す
        /// </summary>
        private ST_FRM_RES_FRAME ConvertByteArrayToResponseFrame(byte[] buf)
        {
            ST_FRM_RES_FRAME stResFrm;

            stResFrm.header = (E_FRM_HEADER)buf[0];
            stResFrm.seqNo = (UInt16)(((UInt16)buf[2]) << 8 | (UInt16)buf[1]);
            stResFrm.cmd = (E_FRM_CMD)(((UInt16)buf[4]) << 8 | (UInt16)buf[3]);
            stResFrm.errCode = (E_FRM_ERRCODE)(((UInt16)buf[6]) << 8 | (UInt16)buf[5]);
            stResFrm.dataSize = (UInt16)(((UInt16)buf[8]) << 8 | (UInt16)buf[7]);
            if (stResFrm.dataSize > 0)
            {
                stResFrm.aData = new byte[stResFrm.dataSize];
                Buffer.BlockCopy(buf, 9, stResFrm.aData, 0, stResFrm.dataSize);
            }
            else
            {
                stResFrm.aData = Array.Empty<byte>();
            }
            int offset = 9 + stResFrm.dataSize;
            stResFrm.checksum = (UInt16)(((UInt16)buf[offset + 1]) << 8 | (UInt16)buf[offset]);

            return stResFrm;
        }

        /// <summary>
        /// Convert byte array to notification frame and return / byte型配列を通知フレームに変換して返す
        /// </summary>
        private ST_FRM_NOTIFY_FRAME ConvertByteArrayToNotifyFrame(byte[] buf)
        {
            ST_FRM_NOTIFY_FRAME stNotifyFrm;

            stNotifyFrm.header = (E_FRM_HEADER)buf[0];
            stNotifyFrm.dataSize = (UInt16)(((UInt16)buf[2]) << 8 | (UInt16)buf[1]);
            if (stNotifyFrm.dataSize > 0)
            {
                stNotifyFrm.aData = new byte[stNotifyFrm.dataSize];
                Buffer.BlockCopy(buf, 3, stNotifyFrm.aData, 0, stNotifyFrm.dataSize);
            }
            else
            {
                stNotifyFrm.aData = Array.Empty<byte>();
            }
            int offset = 3 + stNotifyFrm.dataSize;
            stNotifyFrm.checksum = (UInt16)(((UInt16)buf[offset + 1]) << 8 | (UInt16)buf[offset]);

            return stNotifyFrm;
        }

        /// <summary>
        /// Checksum verification / チェックサム検査
        /// </summary>
        private bool ChecksumTest(byte[] buf, int size, UInt16 expected)
        {
            UInt16 checksum = 0;
            bool bRet = false;

            checksum = CalcChecksum(buf, size);
            if (checksum == expected)
            {
                bRet = true;
            }

            return bRet;
        }

        /// <summary>
        /// Discard the receive frame if the end of the frame is not received within the frame end timeout after receiving the header of the receive frame (response/notification frame) / 受信フレーム(応答/通知フレーム)のヘッダを受信後、フレームエンドタイムアウトの時間が経過してもそのフレームの末端を受信しなかった場合、そのフレームは破棄する
        /// </summary>
        private void FrameEndTimeoutCallback(Object state)
        {
            lock (_lockRecv) // Exclusive access to resources related to the receive process / 受信処理に関するリソースを排他する
            {
                // Discard receive frame / 受信フレーム破棄
                DiscardRecvFrame();
            }
        }

        /// <summary>
        /// Discard receive frame / Receive frame analysis completed so it is no longer needed / 受信フレーム破棄/受信フレーム解析完了のため用済み
        /// </summary>
        private void DiscardRecvFrame()
        {
            // Receive size = 0 / 受信サイズ = 0
            _recvSize = 0;
            // Stop the timer for frame end timeout / フレームエンドタイムアウト用タイマーを停止
            _timerFrameEnd.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Convert the error code in the response frame into an error message and return it / 応答フレーム中のエラーコードをエラーメッセージに変換して返す
        /// </summary>
        private string ConvertErrCodeInResFrameToMsg(E_FRM_ERRCODE errCode)
        {
            string strErrMsg = null;

            // Match E_FRM_ERR in FW source / FWのソースのE_FRM_ERRに合わせる
            switch (errCode)
            {
                case E_FRM_ERRCODE.SUCCESS:
                    break;
                case E_FRM_ERRCODE.DATA_SIZE:
                    //strErrMsg = "マイコンから失敗の応答を受信した。(要求中のデータ部のサイズが不正)";
                    strErrMsg = "A failure response was received from the microcontroller. (The requested data size is invalid)";
                    break;
                case E_FRM_ERRCODE.PARAM:
                    //strErrMsg = "マイコンから失敗の応答を受信した。(要求中の引数が不正)";
                    strErrMsg = "A failure response was received from the microcontroller. (The argument in the request is invalid)";
                    break;
                case E_FRM_ERRCODE.BUF_NOT_ENOUGH:
                    //strErrMsg = "マイコンから失敗の応答を受信した。(バッファに空きがない)";
                    strErrMsg = "A failure response was received from the microcontroller. (There is no space in the buffer)";
                    break;
                case E_FRM_ERRCODE.FRM_ERR_I2C_NO_DEVICE:
                    //strErrMsg = "マイコンから失敗の応答を受信した。(I2C:address not acknowledged, or, no device present.)";
                    strErrMsg = "A failure response was received from the microcontroller. (I2C:address not acknowledged, or, no device present.)";
                    break;
                default:
                    //strErrMsg = "マイコンから失敗の応答を受信した。(未定義のエラーコード)";
                    strErrMsg = "A failure response was received from the microcontroller. (Undefined error code)";
                    break;
            }

            return strErrMsg;
        }
    }
}
