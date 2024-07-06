// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// 回線に接続する
        /// </summary>
        /// <remarks>
        /// 継承先でオーバーライドすること
        /// </remarks>
        public virtual string Connect(Object objParam)
        {
            return null;
        }

        /// <summary>
        /// 回線を切断する
        /// </summary>
        /// <remarks>
        /// 継承先でオーバーライドすること
        /// </remarks>
        public virtual void Disconnect()
        {
        }

        /// <summary>
        /// 回線に接続済みか否かを取得
        /// </summary>
        /// <remarks>
        /// 継承先でオーバーライドすること
        /// </remarks> 
        public virtual bool IsConnected()
        {
            return true;
        }

        /// <summary>
        /// 送信
        /// </summary>
        /// <remarks>
        /// 継承先でオーバーライドすること
        /// </remarks>
        protected virtual string Send(byte[] buf)
        {
            return null;
        }

        /// <summary>
        /// 受信データが存在するか否かを取得
        /// </summary>
        /// <remarks>
        /// 継承先でオーバーライドすること
        /// </remarks>
        protected virtual bool IsExistRecvData()
        {
            return true;
        }

        /// <summary>
        /// 受信データを1byte取り出し
        /// </summary>
        /// <remarks>
        /// 継承先でオーバーライドすること
        /// </remarks>
        protected virtual bool ReadByte(out byte data)
        {
            data = 0;
            return true;
        }
    }
}
