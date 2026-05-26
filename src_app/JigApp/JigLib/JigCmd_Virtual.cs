// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// Connect to the line / 回線に接続する
        /// </summary>
        /// <remarks>
        /// Override in the inheriting class / 継承先でオーバーライドすること
        /// </remarks>
        /// <param name="objParam">
        /// Parameters required for connection (port name, IP address, etc.) / 接続に必要なパラメータ(ポート名やIPアドレスなど)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public virtual string Connect(Object objParam)
        {
            return null;
        }

        /// <summary>
        /// Disconnect the line / 回線を切断する
        /// </summary>
        /// <remarks>
        /// Override in the inheriting class / 継承先でオーバーライドすること
        /// </remarks>
        public virtual void Disconnect()
        {
        }

        /// <summary>
        /// Get whether it is already connected to the line / 回線に接続済みか否かを取得
        /// </summary>
        /// <remarks>
        /// Override in the inheriting class / 継承先でオーバーライドすること
        /// </remarks> 
        /// <returns>
        /// true: Connected, false: Not connected / true:接続済み, false:未接続
        /// </returns>
        public virtual bool IsConnected()
        {
            return true;
        }

        /// <summary>
        /// Send / 送信
        /// </summary>
        /// <remarks>
        /// Override in the inheriting class / 継承先でオーバーライドすること
        /// </remarks>
        protected virtual string Send(byte[] buf)
        {
            return null;
        }

        /// <summary>
        /// Get whether receive data exists / 受信データが存在するか否かを取得
        /// </summary>
        /// <remarks>
        /// Override in the inheriting class / 継承先でオーバーライドすること
        /// </remarks>
        protected virtual bool IsExistRecvData()
        {
            return true;
        }

        /// <summary>
        /// Extract 1 byte of receive data / 受信データを1byte取り出し
        /// </summary>
        /// <remarks>
        /// Override in the inheriting class / 継承先でオーバーライドすること
        /// </remarks>
        protected virtual bool ReadByte(out byte data)
        {
            data = 0;
            return true;
        }
    }
}
