// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// 受信処理の最後のエラーメッセージを取得
        /// </summary>
        public string GetLastRecvErrMsg()
        {
            string strErrMsg;

            lock (_lockRecv) // 受信処理に関するリソースを排他する
            {
                strErrMsg = _strLastRecvErrMsg;
            }

            return strErrMsg;
        }

        /// <summary>
        /// 受信データを取り出し・解析して受信フレーム(応答/通知フレーム)を作成
        /// </summary>
        protected void Recv()
        {
            int dataSize = 0;   // 受信フレーム中のデータサイズ部
            byte data;          // シアル受信データ(1byte)
            UInt16 expect;      // チェックサムの期待値
            ST_FRM_NOTIFY_FRAME stNotifyFrm; // 通知フレーム

            DiscardRecvFrame(); // 初期化

            while (!_isDisconnecting)
            {
                lock (_lockRecv) // 受信処理に関するリソースを排他する
                {
                    try
                    {
                        _strLastRecvErrMsg = null;
                        if (true == IsExistRecvData()) // 受信データが存在する場合
                        {
                            // 受信データを1byte取り出す
                            if (false == ReadByte(out data))
                            {
                                continue;
                            }

                            // [受信データから受信フレーム(応答/通知フレーム)を作成する]

                            if (_recvSize == 0) // 受信データ = ヘッダ の場合(ヘッダはまだ格納していない場合)
                            {
                                switch (data) // ヘッダ
                                {
                                    case (byte)E_FRM_HEADER.RES:              // 応答      
                                    case (byte)E_FRM_HEADER.NOTIFY_UART_RECV: // 通知フレーム(UART受信)
                                        // 受信フレームのバッファをゼロ充填
                                        Array.Clear(_bufRecvFrm, 0, _bufRecvFrm.Length);
                                        // ヘッダを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        // フレームエンドタイムアウト用タイマーを開始
                                        _timerFrameEnd.Change(FRM_END_TIMEOUT, Timeout.Infinite);
                                        break;
                                    default: // 不正なフレームヘッダ
                                        // 受信フレーム破棄
                                        DiscardRecvFrame();
                                        break;
                                }
                            }
                            else // ヘッダは格納済みの場合
                            {
                                if (_bufRecvFrm[0] == (byte)E_FRM_HEADER.RES) // ヘッダ = 応答 の場合
                                {
                                    if (_recvSize <= 8) // 受信データ= シーケンス番号/コマンド/エラーコード/データサイズ の場合
                                    {
                                        // シーケンス番号/コマンド/エラーコード/データサイズを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == 9) // データサイズは格納済みの場合
                                        {
                                            // データサイズが最大値を超えてないかをチェック
                                            dataSize = _bufRecvFrm[8] << 8 | _bufRecvFrm[7];
                                            if (dataSize > FRM_DATA_MAX_SIZE) // データサイズが最大値を超えている場合
                                            {
                                                // 受信フレーム破棄
                                                DiscardRecvFrame();
                                            }
                                        }
                                    }
                                    else if (_recvSize <= (9 + dataSize + 2)) // 受信データ = データ/チェックサム の場合
                                    {
                                        // データ/チェックサムを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == (9 + dataSize + 2)) // チェックサムは格納済みの場合
                                        {
                                            // チェックサム検査
                                            expect = (UInt16)(((UInt16)_bufRecvFrm[_recvSize - 1]) << 8 | (UInt16)_bufRecvFrm[_recvSize - 2]);
                                            if (ChecksumTest(_bufRecvFrm, _recvSize - 2, expect)) // チェックサム検査がOKの場合
                                            {
                                                // 応答フレームをキューイング
                                                ST_FRM_RES_FRAME stResFrm = ConvertByteAarrayToResFrameStruct(_bufRecvFrm);
                                                if (PrpResFrmQue.TryAdd(stResFrm))
                                                {
                                                    // 応答フレーム受信をイベント通知
                                                    PrpResEvent.Set();
                                                }
                                            }
                                            // 受信フレーム解析完了
                                            DiscardRecvFrame();
                                        }
                                    }
                                    else
                                    {
                                        // 無処理
                                    }
                                }
                                else // ヘッダ = 通知 の場合
                                {
                                    if (_recvSize <= 2) // 受信データ = データサイズ の場合
                                    {
                                        // データサイズを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == 3) // データサイズは格納済みの場合
                                        {
                                            // データサイズが最大値を超えてないかをチェック
                                            dataSize = _bufRecvFrm[2] << 8 | _bufRecvFrm[1];
                                            if (dataSize > FRM_DATA_MAX_SIZE) // データサイズが最大値を超えている場合
                                            {
                                                // 受信フレーム破棄
                                                DiscardRecvFrame();
                                            }
                                        }
                                    }
                                    else if (_recvSize <= (3 + dataSize + 2)) // 受信データ = データ/チェックサムの場合
                                    {
                                        // データ/チェックサムを格納
                                        _bufRecvFrm[_recvSize++] = data;
                                        if (_recvSize == (3 + dataSize + 2)) // チェックサムは格納済みの場合
                                        {
                                            // チェックサム検査
                                            expect = (ushort)(((ushort)_bufRecvFrm[_recvSize - 1]) << 8 | (ushort)_bufRecvFrm[_recvSize - 2]);
                                            if (ChecksumTest(_bufRecvFrm, _recvSize - 2, expect)) // チェックサム検査がOKの場合
                                            {
                                                switch (_bufRecvFrm[0]) // ヘッダ
                                                {
                                                    case (byte)E_FRM_HEADER.NOTIFY_UART_RECV: // 通知フレーム(UART受信)
                                                        // 通知フレーム(UART受信)を取得 
                                                        stNotifyFrm = ConvertByteAarrayToNotifyFrameStruct(_bufRecvFrm);
                                                        // UART受信データをキューイング
                                                        for (int i = 0; i < stNotifyFrm.dataSize; i++)
                                                        {
                                                            PrpUartRecvDataQue.TryAdd(stNotifyFrm.aData[i]);
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            // 受信フレーム解析完了
                                            DiscardRecvFrame();
                                        }
                                    }
                                    else
                                    {
                                        // 無処理
                                    }
                                }
                            }
                        }
                        else // 受信データが存在しない場合
                        {
                            Thread.Sleep(RECV_DELAY);
                        }
                    }
                    catch (Exception ex) // ポートのエラーが発生した場合
                    {
                        _strLastRecvErrMsg = ex.Message;
                        Thread.Sleep(RECV_DELAY);
                    }
                }
            }
        }

        /// <summary>
        /// byte型配列を応答フレーム構造体に変換して返す
        /// </summary>
        private ST_FRM_RES_FRAME ConvertByteAarrayToResFrameStruct(byte[] buf)
        {
            ST_FRM_RES_FRAME stResFrm;

            stResFrm.header = (E_FRM_HEADER)buf[0];
            stResFrm.seqNo = (UInt16)(((UInt16)buf[2]) << 8 | (UInt16)buf[1]);
            stResFrm.cmd = (E_FRM_CMD)(((UInt16)buf[4]) << 8 | (UInt16)buf[3]);
            stResFrm.errCode = (E_FRM_ERRCODE)(((UInt16)buf[6]) << 8 | (UInt16)buf[5]);
            stResFrm.dataSize = (UInt16)(((UInt16)buf[8]) << 8 | (UInt16)buf[7]);
            stResFrm.aData = new byte[stResFrm.dataSize];
            Buffer.BlockCopy(buf, 9, stResFrm.aData, 0, stResFrm.dataSize);
            int offset = 8 + stResFrm.dataSize;
            stResFrm.checksum = (UInt16)(((UInt16)buf[offset + 1]) << 8 | (UInt16)buf[offset]);

            return stResFrm;
        }

        /// <summary>
        /// byte型配列を通知フレーム構造体に変換して返す
        /// </summary>
        private ST_FRM_NOTIFY_FRAME ConvertByteAarrayToNotifyFrameStruct(byte[] buf)
        {
            ST_FRM_NOTIFY_FRAME stNotifyFrm;

            stNotifyFrm.header = (E_FRM_HEADER)buf[0];
            stNotifyFrm.dataSize = (UInt16)(((UInt16)buf[2]) << 8 | (UInt16)buf[1]);
            stNotifyFrm.aData = new byte[stNotifyFrm.dataSize];
            Buffer.BlockCopy(buf, 3, stNotifyFrm.aData, 0, stNotifyFrm.dataSize);
            int offset = 2 + stNotifyFrm.dataSize;
            stNotifyFrm.checksum = (UInt16)(((UInt16)buf[offset + 1]) << 8 | (UInt16)buf[offset]);

            return stNotifyFrm;
        }

        /// <summary>
        /// チェックサム検査
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
        /// 受信フレーム(応答/通知フレーム)のヘッダを受信後、フレームエンドタイムアウトの時間が経過してもそのフレームの末端を受信しなかった場合、そのフレームは破棄する
        /// </summary>
        private void FrameEndTimeoutCallback(Object state)
        {
            lock (_lockRecv) // 受信処理に関するリソースを排他する
            {
                // 受信フレーム破棄
                DiscardRecvFrame();
            }
        }

        /// <summary>
        /// 受信フレーム破棄/受信フレーム解析完了のため用済み
        /// </summary>
        private void DiscardRecvFrame()
        {
            // 受信サイズ = 0
            _recvSize = 0;
            // フレームエンドタイムアウト用タイマーを停止
            _timerFrameEnd.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 応答フレーム中のエラーコードをエラーメッセージに変換して返す
        /// </summary>
        private string ConvertErrCodeInResFrameToMsg(E_FRM_ERRCODE errCode)
        {
            string strErrMsg = null;

            // FWのソースのE_FRM_ERRに合わせる
            switch (errCode)
            {
                case E_FRM_ERRCODE.SUCCESS:
                    break;
                case E_FRM_ERRCODE.DATA_SIZE:
                    //strErrMsg = "マイコンから失敗の応答を受信した。(要求中のデータ部のサイズが不正)";
                    strErrMsg = "A failure response was received from the microcontroller. (The size of the data part being requested is invalid)";
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
                    strErrMsg = "A failure response was received from the microcontroller. (undefined error code)";
                    break;
            }

            return strErrMsg;
        }
    }
}
