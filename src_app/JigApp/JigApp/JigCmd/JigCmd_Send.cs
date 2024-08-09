// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// 「FW情報取得」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetFwInfo(out string strMakerName, out string strFwName, out string strFwVer, out string strBoardId)
        {
            int offset = 0;
            byte[] aReqData = null;
            byte[] aResData = null;
            UInt32 fwVer;
            string strErrMsg;

            strMakerName = null;
            strFwName = null;
            strFwVer = null;
            strBoardId = null;

            strErrMsg = SendCmd(E_FRM_CMD.GET_FW_INFO, aReqData, out aResData);
            if (strErrMsg == null)
            {
                strMakerName = string.Empty;
                for (int i = 0; i < 16; i++)
                {
                    if (aResData[i] != '\0')
                    {
                        strMakerName += (char)aResData[i];
                    }
                }
                offset += 16;

                strFwName = string.Empty;
                for (int i = 0; i < 16; i++)
                {
                    if (aResData[offset + i] != '\0')
                    {
                        strFwName += (char)aResData[offset + i];
                    }
                }
                offset += 16;

                fwVer = BitConverter.ToUInt32(aResData, offset);
                offset += 4;
                strFwVer = fwVer.ToString("X8");

                strBoardId = string.Empty;
                for (int i = 0; i < 8; i++)
                {
                    strBoardId += aResData[offset + i].ToString("X2");
                }
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「GPIO通信設定変更」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SetGpioConfig(UInt32 pullDownBits, UInt32 initialValBits)
        {
            byte[] aReqData = new byte[8];
            byte[] aResData = null;
            string strErrMsg = null;

            Array.Copy(BitConverter.GetBytes(pullDownBits), 0, aReqData, 0, 4);
            Array.Copy(BitConverter.GetBytes(initialValBits), 0, aReqData, 4, 4);

            strErrMsg = SendCmd(E_FRM_CMD.SET_GPIO_CONFIG, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「GPIO通信設定取得」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetGpioConfig(out UInt32 pullDownBits, out UInt32 initialValBits)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            pullDownBits = 0;
            initialValBits = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_GPIO_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                pullDownBits = BitConverter.ToUInt32(aResData, 0);
                initialValBits = BitConverter.ToUInt32(aResData, 4);
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「GPIO入力」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetGpio(out UInt32 valBits)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            valBits = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_GPIO, aReqData, out aResData);
            if (strErrMsg == null)
            {
                valBits = BitConverter.ToUInt32(aResData, 0);
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「GPIO出力」コマンドの要求を送信
        /// </summary>
        public string SendCmd_OutGpio(UInt32 valBits)
        {
            byte[] aReqData = new byte[4];
            byte[] aResData = null;
            string strErrMsg;

            Array.Copy(BitConverter.GetBytes(valBits), 0, aReqData, 0, 4);

            strErrMsg = SendCmd(E_FRM_CMD.PUT_GPIO, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「ADC入力」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetAdc(out float[] aVolt)
        {
            const int c_chNum = 4;
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            aVolt = null;

            strErrMsg = SendCmd(E_FRM_CMD.GET_ADC, aReqData, out aResData);
            if (strErrMsg == null)
            {
                aVolt = new float[c_chNum];
                for (int i = 0; i < c_chNum; i++)
                {
                    aVolt[i] = BitConverter.ToSingle(aResData, i * 4);
                }
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「UART通信設定変更」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SetUartConfig(UInt32 baudrate, byte dataBits, byte stopBits, byte parity)
        {
            byte[] aReqData = new byte[7];
            byte[] aResData = null;
            string strErrMsg;

            if (dataBits != 8)
            {
                //strErrMsg = "パラメータ不正。(データビット)";
                strErrMsg = "Invalid parameter. (data bit)";
                goto End;
            }

            if (stopBits != 1 && stopBits != 2)
            {
                //strErrMsg = "パラメータ不正。(ストップビット)";
                strErrMsg = "Invalid parameter. (stop bit)";
                goto End;
            }

            if (parity != 0 && parity != 1 && parity != 2)
            {
                //strErrMsg = "パラメータ不正。(パリティ)";
                strErrMsg = "Invalid parameter. (parity)";
                goto End;
            }

            Array.Copy(BitConverter.GetBytes(baudrate), 0, aReqData, 0, 4);
            aReqData[4] = dataBits;
            aReqData[5] = stopBits;
            aReqData[6] = parity;

            strErrMsg = SendCmd(E_FRM_CMD.SET_UART_CONFIG, aReqData, out aResData);
        
        End:
            return strErrMsg;
        }

        /// <summary>
        /// 「UART通信設定取得」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetUartConfig(out UInt32 baudrate, out byte dataBits, out byte stopBits, out byte parity)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            baudrate = 0;
            dataBits = 0;
            stopBits = 0;
            parity = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_UART_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                baudrate = BitConverter.ToUInt32(aResData, 0);
                dataBits = aResData[4];
                stopBits = aResData[5];
                parity = aResData[6];
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「UART送信」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SendUart(byte[] aReqData)
        {
            byte[] aResData = null;
            string strErrMsg;

            if (aReqData.Length < 1 || aReqData.Length > 256)
            {
                //strErrMsg = "パラメータ不正。(送信データサイズ)";
                strErrMsg = "Invalid parameter. (transmission data size)";
                goto End;
            }

            strErrMsg = SendCmd(E_FRM_CMD.SEND_UART, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// 「SPI通信設定変更」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SetSpiConfig(UInt32 freq, byte dataBits, byte polarity, byte phase, byte order)
        {
            byte[] aReqData = new byte[8];
            byte[] aResData = null;
            string strErrMsg;

            if (dataBits != 8)
            {
                //strErrMsg = "パラメータ不正。(データビット)";
                strErrMsg = "Invalid parameter. (data bit)";
                goto End;
            }

            if (polarity != 0 && polarity != 1)
            {
                //strErrMsg = "パラメータ不正。(CPOL)";
                strErrMsg = "Invalid parameter. (CPOL)";
                goto End;
            }

            if (phase != 0 && phase != 1)
            {
                //strErrMsg = "パラメータ不正。(CPHA)";
                strErrMsg = "IInvalid parameter. (CPHA)";
                goto End;
            }

            if (order != 1)
            {
                //strErrMsg = "パラメータ不正。(バイトオーダー)";
                strErrMsg = "Invalid parameter. (Byte order)";
                goto End;
            }

            Array.Copy(BitConverter.GetBytes(freq), 0, aReqData, 0, 4);
            aReqData[4] = dataBits;
            aReqData[5] = polarity;
            aReqData[6] = phase;
            aReqData[7] = order;

            strErrMsg = SendCmd(E_FRM_CMD.SET_SPI_CONFIG, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        ///「SPI通信設定取得」コマンドの要求を送信 
        /// </summary>
        public string SendCmd_GetSpiConfig(out UInt32 freq, out byte dataBits, out byte polarity, out byte phase, out byte order)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            freq = 0;
            dataBits = 0;
            polarity = 0;
            phase = 0;
            order = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_SPI_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                freq = BitConverter.ToUInt32(aResData, 0);
                dataBits = aResData[4];
                polarity = aResData[5];
                phase = aResData[6];
                order = aResData[7];
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「SPIマスタ送受信」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SendSpi(byte[] aReqData, out byte[] aResData)
        {
            string strErrMsg;

            aResData = null;

            if (aReqData.Length < 1 || aReqData.Length > 256)
            {
                //strErrMsg = "パラメータ不正。(送信データサイズ)";
                strErrMsg = "Invalid parameter. (transmission data size)";
                goto End;
            }

            strErrMsg = SendCmd(E_FRM_CMD.SENDRECV_SPI, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// 「I2C通信設定変更」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SetI2cConfig(UInt32 freq)
        {
            byte[] aReqData = new byte[4];
            byte[] aResData = null;
            string strErrMsg;

            Array.Copy(BitConverter.GetBytes(freq), 0, aReqData, 0, 4);

            strErrMsg = SendCmd(E_FRM_CMD.SET_I2C_CONFIG, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「I2C通信設定取得」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetI2cConfig(out UInt32 freq)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            freq = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_I2C_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                freq = BitConverter.ToUInt32(aResData, 0);
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「I2Cマスタ送信」コマンドの要求を送信
        /// </summary>
        /// <param name="slaveAddr">
        /// 7bitスレーブアドレス
        /// </param>
        /// <param name="aReqData">
        /// 送信データ(1～256byte)　
        /// </param>
        public string SendCmd_SendI2c(byte slaveAddr, byte[] aReqData)
        {
            byte[] aReqData2 = new byte[aReqData.Length + 3];
            byte[] aResData = null;
            string strErrMsg;

            if (slaveAddr > 0x7F)
            {
                //strErrMsg = "パラメータ不正。(7bitスレーブアドレス)";
                strErrMsg = "Invalid parameter. (7bit slave address)";
                goto End;
            }

            if (aReqData.Length < 1 || aReqData.Length > 256)
            {
                //strErrMsg = "パラメータ不正。(送信データサイズ)";
                strErrMsg = "Invalid parameter. (transmission data size)";
                goto End;
            }

            aReqData2[0] = slaveAddr; // スレーブアドレス
            aReqData2[1] = (byte)(aReqData.Length & 0xFF); // I2C送信サイズ
            aReqData2[2] = (byte)((aReqData.Length >> 8) & 0xFF);
            Array.Copy(aReqData, 0, aReqData2, 3, aReqData.Length); // I2C送信データ

            strErrMsg = SendCmd(E_FRM_CMD.SEND_I2C, aReqData2, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// 「I2Cマスタ受信」コマンドの要求を送信
        /// </summary>
        /// <param name="slaveAddr">
        /// 7bitスレーブアドレス
        /// </param>
        /// <param name="recvSize">
        /// 受信サイズ
        /// </param>
        /// <param name="aResData">
        /// 受信データ格納先
        /// </param>
        public string SendCmd_RecvI2c(byte slaveAddr, UInt16 recvSize, out byte[] aResData)
        {
            byte[] aReqData = new byte[3];
            string strErrMsg;

            aResData = null;

            if (slaveAddr > 0x7F)
            {
                //strErrMsg = "パラメータ不正。(7bitスレーブアドレス)";
                strErrMsg = "Invalid parameter. (7bit slave address)";
                goto End;
            }

            if (recvSize < 1 || recvSize > 256)
            {
                //strErrMsg = "パラメータ不正。(受信データサイズ)";
                strErrMsg = "Invalid parameter. (Receive data size)";
                goto End;
            }

            aReqData[0] = slaveAddr; // スレーブアドレス
            aReqData[1] = (byte)(recvSize & 0xFF); // I2C受信サイズ
            aReqData[2] = (byte)((recvSize >> 8) & 0xFF);

            strErrMsg = SendCmd(E_FRM_CMD.RECV_I2C, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// 「PWM開始」コマンドの要求を送信
        /// </summary>
        public string SendCmd_StartPwm(float divider, UInt16 wrap, UInt16 level)
        {
            byte[] aReqData = new byte[8];
            byte[] aResData = null;
            string strErrMsg;

            Array.Copy(BitConverter.GetBytes(divider), 0, aReqData, 0, 4);
            Array.Copy(BitConverter.GetBytes(wrap), 0, aReqData, 4, 2);
            Array.Copy(BitConverter.GetBytes(level), 0, aReqData, 6, 2);

            strErrMsg = SendCmd(E_FRM_CMD.START_PWM, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「PWM停止」コマンドの要求を送信
        /// </summary>
        public string SendCmd_StopPwm()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.STOP_PWM, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「FWエラー情報取得」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetFwError(ref List<string> lstErrMsg)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            UInt32 errBits = 0;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.GET_FW_ERR, aReqData, out aResData);
            if (strErrMsg == null)
            {
                errBits = BitConverter.ToUInt32(aResData, 0);
                lstErrMsg.Clear();
                for (int i = 0; i < FW_ERR_MSG_ARY.Length; i++)
                {
                    if ((errBits & (1 << i)) != 0)
                    {
                        lstErrMsg.Add(FW_ERR_MSG_ARY[i]);
                    }
                }

            }

            return strErrMsg;
        }

        /// <summary>
        /// 「FWエラークリア」コマンドの要求を送信
        /// </summary>
        public string SendCmd_ClearFwError()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.CLEAR_FW_ERR, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「FLASH消去」コマンドの要求を送信
        /// </summary>
        public string SendCmd_EraseFlash()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.ERASE_FLASH, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// 「ネットワーク設定変更」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SetNwConfig(string strCountryCode, string strIpAddr, string strSsid, string strPassword)
        {
            byte[] aReqData = new byte[105];
            byte[] aResData = null;
            char[] achCountryCode = new char[3];
            byte[] abyIpAddr = new byte[4];
            char[] achSsid = new char[33];
            char[] achPassword = new char[65];
            string strErrMsg;

            Array.Clear(achCountryCode, 0, achCountryCode.Length);
            Array.Clear(achSsid, 0, achSsid.Length);
            Array.Clear(achPassword, 0, achPassword.Length);

            // カントリーコード
            if (strCountryCode.ToCharArray().Length != achCountryCode.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Country code)";
                goto End;
            }
            Array.Copy(strCountryCode.ToCharArray(), 0, achCountryCode, 0, strCountryCode.ToCharArray().Length);

            // IPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strIpAddr, out abyIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // SSID
            if (strSsid.ToCharArray().Length > achSsid.Length - 1)
            {
                strErrMsg = "Invalid parameter. (SSID)";
                goto End;
            }
            Array.Copy(strSsid.ToCharArray(), 0, achSsid, 0, strSsid.ToCharArray().Length);

            // パスワード
            if (strPassword.ToCharArray().Length > achPassword.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Password)";
                goto End;
            }
            Array.Copy(strPassword.ToCharArray(), 0, achPassword, 0, strPassword.ToCharArray().Length);

            // 要求データ
            for (int i = 0; i < achCountryCode.Length; i++)
            {
                aReqData[i] = (byte)achCountryCode[i];
            }
            Array.Copy(abyIpAddr, 0, aReqData, 3, abyIpAddr.Length);
            for (int i = 0; i < achSsid.Length; i++)
            {
                aReqData[7 + i] = (byte)achSsid[i];
            }
            for (int i = 0; i < achPassword.Length; i++)
            {
                aReqData[40 + i] = (byte)achPassword[i];
            }

            strErrMsg = SendCmd(E_FRM_CMD.SET_NW_CONFIG, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// 「ネットワーク設定変更2」コマンドの要求を送信
        /// </summary>
        public string SendCmd_SetNwConfig2(string strCountryCode, string strIpAddr, string strSsid, string strPassword, string strServerIpAddr, bool isClient)
        {
            byte[] aReqData = new byte[110];
            byte[] aResData = null;
            char[] achCountryCode = new char[3];
            byte[] abyIpAddr = new byte[4];
            char[] achSsid = new char[33];
            char[] achPassword = new char[65];
            byte[] abyServerIpAddr = new byte[4];
            byte byIsClient = 0;
            string strErrMsg;

            Array.Clear(achCountryCode, 0, achCountryCode.Length);
            Array.Clear(achSsid, 0, achSsid.Length);
            Array.Clear(achPassword, 0, achPassword.Length);

            // カントリーコード
            if (strCountryCode.ToCharArray().Length != achCountryCode.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Country code)";
                goto End;
            }
            Array.Copy(strCountryCode.ToCharArray(), 0, achCountryCode, 0, strCountryCode.ToCharArray().Length);

            // IPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strIpAddr, out abyIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // SSID
            if (strSsid.ToCharArray().Length > achSsid.Length - 1)
            {
                strErrMsg = "Invalid parameter. (SSID)";
                goto End;
            }
            Array.Copy(strSsid.ToCharArray(), 0, achSsid, 0, strSsid.ToCharArray().Length);

            // パスワード
            if (strPassword.ToCharArray().Length > achPassword.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Password)";
                goto End;
            }
            Array.Copy(strPassword.ToCharArray(), 0, achPassword, 0, strPassword.ToCharArray().Length);

            // サーバーのIPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strServerIpAddr, out abyServerIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // 要求データ
            for (int i = 0; i < achCountryCode.Length; i++)
            {
                aReqData[i] = (byte)achCountryCode[i];
            }
            Array.Copy(abyIpAddr, 0, aReqData, 3, abyIpAddr.Length);
            for (int i = 0; i < achSsid.Length; i++)
            {
                aReqData[7 + i] = (byte)achSsid[i];
            }
            for (int i = 0; i < achPassword.Length; i++)
            {
                aReqData[40 + i] = (byte)achPassword[i];
            }
            Array.Copy(abyServerIpAddr, 0, aReqData, 105, abyServerIpAddr.Length);
            if (isClient)
            {
                byIsClient = 1;
            }
            aReqData[109] = byIsClient;

            strErrMsg = SendCmd(E_FRM_CMD.SET_NW_CONFIG2, aReqData, out aResData);

            End:
            return strErrMsg;
        }

        /// <summary>
        /// 「ネットワーク設定取得」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetNwConfig(out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            byte[] aIpAddr = new byte[4];
            char[] szCountryCode = new char[3];
            char[] szSsid = new char[33];
            char[] szPassword = new char[65];
            string strErrMsg;
            
            strCountryCode = null;
            strIpAddr = null;
            strSsid = null;
            strPassword = null;

            strErrMsg = SendCmd(E_FRM_CMD.GET_NW_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                Array.Copy(aResData, 0, szCountryCode, 0, szCountryCode.Length);
                Array.Copy(aResData, 3, aIpAddr, 0, aIpAddr.Length);
                Array.Copy(aResData, 7, szSsid, 0, szSsid.Length);
                Array.Copy(aResData, 40, szPassword, 0, szPassword.Length);
                strCountryCode = new string(szCountryCode);
                strIpAddr = aIpAddr[0].ToString() + "." + aIpAddr[1].ToString() + "." + aIpAddr[2].ToString() + "." +  aIpAddr[3].ToString();
                strSsid = new string(szSsid);
                strPassword = new string(szPassword);
            }

            return strErrMsg;
        }

        /// <summary>
        /// 「ネットワーク設定取得2」コマンドの要求を送信
        /// </summary>
        public string SendCmd_GetNwConfig2(out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword, out string strServerIpAddr, out bool isClient)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            byte[] aIpAddr = new byte[4];
            char[] szCountryCode = new char[3];
            char[] szSsid = new char[33];
            char[] szPassword = new char[65];
            byte[] aServerIpAddr = new byte[4];
            string strErrMsg;

            strCountryCode = null;
            strIpAddr = null;
            strSsid = null;
            strPassword = null;
            strServerIpAddr = null;
            isClient = false;

            strErrMsg = SendCmd(E_FRM_CMD.GET_NW_CONFIG2, aReqData, out aResData);
            if (strErrMsg == null)
            {
                Array.Copy(aResData, 0, szCountryCode, 0, szCountryCode.Length);
                Array.Copy(aResData, 3, aIpAddr, 0, aIpAddr.Length);
                Array.Copy(aResData, 7, szSsid, 0, szSsid.Length);
                Array.Copy(aResData, 40, szPassword, 0, szPassword.Length);
                Array.Copy(aResData, 105, aServerIpAddr, 0, aServerIpAddr.Length);

                strCountryCode = new string(szCountryCode);
                strIpAddr = aIpAddr[0].ToString() + "." + aIpAddr[1].ToString() + "." + aIpAddr[2].ToString() + "." + aIpAddr[3].ToString();
                strSsid = new string(szSsid);
                strPassword = new string(szPassword);
                strServerIpAddr = aServerIpAddr[0].ToString() + "." + aServerIpAddr[1].ToString() + "." + aServerIpAddr[2].ToString() + "." + aServerIpAddr[3].ToString();

                if (aResData[109] == 1)
                {
                    isClient = true;
                }
            }

            return strErrMsg;
        }

        /// <summary>
        /// 要求フレームを送信
        /// </summary>
        private string SendCmd(E_FRM_CMD eCmd, byte[] aReqData, out byte[] aResData, int resTimeout = FRM_RES_TIMEOUT)
        {
            byte[] aReqFrm;            // 要求フレーム
            string strErrMsg = null;   // エラーメッセージ
            ST_FRM_REQ_FRAME stReqFrm; // 要求フレーム
            ST_FRM_RES_FRAME stResFrm; // 応答フレーム

            lock (_lockSend) // 送信～応答待ち中は、次の送信をしないようにするためのロック
            {
                aResData = null;

                // 応答フレーム受信キューを空にする
                PrpResEvent.Reset();
                while (true == PrpResFrmQue.TryTake(out stResFrm)) { }

                // [要求フレームを作成]
                stReqFrm.header = E_FRM_HEADER.REQ; // ヘッダ
                stReqFrm.seqNo = _seqNo++;          // シーケンス番号
                stReqFrm.cmd = eCmd;                // コマンド
                if (aReqData == null) // データ部が空の場合
                {
                    stReqFrm.dataSize = 0;  // データサイズ
                    stReqFrm.aData = null;  // データ
                }
                else // データ部が空ではない場合
                {
                    stReqFrm.dataSize = (UInt16)aReqData.Length; // データサイズ
                    stReqFrm.aData = aReqData;                   // データ
                }
                // チェックサム計算前の要求フレームのbyte型配列を取得
                stReqFrm.checksum = 0;
                aReqFrm = ConvertReqFrameStructToByteArray(stReqFrm);
                // チェックサムを計算 
                stReqFrm.checksum = CalcChecksum(aReqFrm, aReqFrm.Length - 2);

                // [要求フレームを送信]
                // チェックサム計算後の要求フレームのbyte型配列を取得
                aReqFrm = ConvertReqFrameStructToByteArray(stReqFrm);
                // 要求フレームを送信    
                strErrMsg = Send(aReqFrm);
                if (strErrMsg != null)
                {
                    // 送信失敗
                    _isConnected = false; // 切断しているとみなす
                    goto End;
                }

                // [応答無しのコマンドの場合]
                switch (eCmd)
                {
                    case E_FRM_CMD.SEND_UART:
                        goto End;
                    default:
                        break;
                }

                // [応答フレーム受信イベント発生待ち]
                if (!PrpResEvent.WaitOne(resTimeout))
                {
                    strErrMsg = "Response frame reception timeout.";
                    _isConnected = false; // 切断しているとみなす
                    goto End;
                }

                // [応答フレーム受信キューから応答フレームを取り出す]
                if (PrpResFrmQue.TryTake(out stResFrm))
                {
                    if (stResFrm.seqNo != stReqFrm.seqNo)
                    {
                        strErrMsg = "The sequence number in the response does not match the request.";
                        goto End;
                    }
                    if (stResFrm.cmd != stReqFrm.cmd)
                    {
                        strErrMsg = "The command being responded to does not match the request.";
                        goto End;
                    }

                    if (stResFrm.errCode == E_FRM_ERRCODE.SUCCESS)
                    {
                        aResData = new byte[stResFrm.aData.Length];
                        Array.Copy(stResFrm.aData, aResData, aResData.Length);
                    }
                    else
                    {
                        strErrMsg = ConvertErrCodeInResFrameToMsg(stResFrm.errCode);
                        goto End;
                    }
                }
                else
                {
                    strErrMsg = STR_MSG_WAIT_RES_CANCEL;
                    goto End;
                }
        End:

                return strErrMsg;
            }
        }

        /// <summary>
        /// 要求フレーム構造体をbyte型配列へ変換する
        /// </summary>
        private byte[] ConvertReqFrameStructToByteArray(ST_FRM_REQ_FRAME stReqFrm)
        {
            List<byte[]> lst = new List<byte[]>(); // byte型配列のリスト

            // 要求フレーム構造体の各フィールドをbyte型配列に変換してリストに追加
            lst.Add(new byte[1] { (byte)stReqFrm.header });
            lst.Add(BitConverter.GetBytes(stReqFrm.seqNo));
            lst.Add(BitConverter.GetBytes((UInt16)stReqFrm.cmd));
            lst.Add(BitConverter.GetBytes(stReqFrm.dataSize));
            lst.Add(stReqFrm.aData);
            lst.Add(BitConverter.GetBytes(stReqFrm.checksum));

            // リストを1つのbyte型配列に結合して返す
            return CombineByteArray(lst);
        }

        /// <summary>
        /// 引数のbyte型配列のリストを1つのbyte型配列に結合して返す
        /// </summary>
        private byte[] CombineByteArray(List<byte[]> lst)
        {
            // 返却するbyte型配列のサイズを求める
            int size = 0;
            foreach (byte[] ary in lst)
            {
                if (ary != null)
                {
                    size += ary.Length;
                }
            }

            // 引数のbyte型配列のリストを1つのbyte型配列に結合する
            int offset = 0;
            byte[] buf = new byte[size];
            foreach (byte[] ary in lst)
            {
                if (ary != null)
                {
                    Buffer.BlockCopy(ary, 0, buf, offset, ary.Length);
                    offset += ary.Length;
                }
            }

            return buf;
        }

        /// <summary>
        /// IPアドレスの文字列をbyte型の配列に変換する
        /// </summary>
        /// <remarks>
        /// セパレータはドット。
        /// </remarks>
        private string ConvertIpAddrStringToByteArray(string strText, out byte[] aVal)
        {
            char[] aSeparator = { '.' }; // セパレータ
            string strErrMsg = ConvertStringToValArray(strText, aSeparator, 10, out aVal);
            if (strErrMsg == null)
            {
                if (aVal.Length != 4)
                {
                    strErrMsg = "Invalid parameter. (IP address)";
                }
            }
            return strErrMsg;
        }

        /// <summary>
        /// 文字列をbyte型の配列に変換する
        /// </summary>
        private string ConvertStringToValArray(string strText, char[] aSeparator, int baseNumber, out byte[] aVal)
        {
            string[] astrSplit; // 分割後の文字列
            string strErrMsg = null;

            // 文字列をセパレータで分割
            strText = strText.Replace("\r\n", "\r");
            astrSplit = strText.Split(aSeparator);

            // [文字列をbyte型の配列に変換]
            // 要素数が分割された文字列の数であるbyte型配列を用意
            aVal = new byte[astrSplit.Count()];
            // 分割された文字列の数だけ、文字列をbyte型に変換
            for (int i = 0; i < astrSplit.Count(); i++)
            {
                try
                {
                    aVal[i] = Convert.ToByte(astrSplit[i], baseNumber);
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                }
            }

            return strErrMsg;
        }
    }
}
