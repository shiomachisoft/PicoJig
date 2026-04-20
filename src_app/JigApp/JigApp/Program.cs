// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Windows.Forms;
using JigLib;

namespace JigApp
{
    public static class Program
    {
        /// <summary>
        /// Instance of jig command (serial communication) / 治具コマンド(シリアル通信)のインスタンス
        /// </summary>
        public static JigSerial PrpJigSerial { get; set; } = new JigSerial();
        /// <summary>
        /// Instance of jig command (TCP client) / 治具コマンド(TCPクライアント)のインスタンス
        /// </summary>
        public static JigTcpClient PrpJigTcpClient { get; set; } = new JigTcpClient();
        /// <summary>
        /// Instance of jig command (base class) / 治具コマンド(基底クラス)のインスタンス
        /// </summary>
        public static JigCmd PrpJigCmd { get; set; } = PrpJigSerial;
  
        /// <summary>
        /// The main entry point for the application. / アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
