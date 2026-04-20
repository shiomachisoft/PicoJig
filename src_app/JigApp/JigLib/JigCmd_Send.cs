// Copyright © 2024 Shiomachi Software. All rights reserved.
using System;
using System.Collections.Generic;

namespace JigLib
{
    public abstract partial class JigCmd
    {
        /// <summary>
        /// Send request for "Get FW Info" command / 「FW情報取得」コマンドの要求を送信
        /// </summary>
        /// <param name="strMakerName">
        /// Acquired manufacturer name (16 characters) / 取得したメーカー名(16文字)
        /// </param>
        /// <param name="strFwName">
        /// Acquired FW name (16 characters) / 取得したFW名(16文字)
        /// </param>
        /// <param name="strFwVer">
        /// Acquired FW version (8-character hex string) / 取得したFWバージョン(8文字の16進数文字列)
        /// </param>
        /// <param name="strBoardId">
        /// Acquired board ID (16-character hex string) / 取得したボードID(16文字の16進数文字列)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
                    else
                    {
                        break;
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
                    else
                    {
                        break;
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
        /// Send request for "Change GPIO Settings" command / 「GPIO設定変更」コマンドの要求を送信
        /// </summary>
        /// <param name="pullDownBits">
        /// Built-in pull-up/pull-down setting bitmask for input GPIO (each bit 1: pull-down, 0: pull-up) / 入力GPIOの内蔵プルアップ/プルダウン設定ビットマスク(各ビット 1:プルダウン, 0:プルアップ)
        /// </param>
        /// <param name="initialOutValBits">
        /// Output GPIO power-on output value bitmask (each bit 1: High, 0: Low) / 出力GPIOの電源ON時出力値ビットマスク(各ビット 1:High, 0:Low)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SetGpioConfig(UInt32 pullDownBits, UInt32 initialOutValBits)
        {
            byte[] aReqData = new byte[8];
            byte[] aResData = null;
            string strErrMsg = null;

            Array.Copy(BitConverter.GetBytes(pullDownBits), 0, aReqData, 0, 4);
            Array.Copy(BitConverter.GetBytes(initialOutValBits), 0, aReqData, 4, 4);

            strErrMsg = SendCmd(E_FRM_CMD.SET_GPIO_CONFIG, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Get GPIO Settings" command / 「GPIO設定取得」コマンドの要求を送信
        /// </summary>
        /// <param name="pullDownBits">
        /// Acquired built-in pull-up/pull-down setting bitmask for input GPIO (each bit 1: pull-down, 0: pull-up) / 取得した入力GPIOの内蔵プルアップ/プルダウン設定ビットマスク(各ビット 1:プルダウン, 0:プルアップ)
        /// </param>
        /// <param name="initialOutValBits">
        /// Acquired output GPIO power-on output value bitmask (each bit 1: High, 0: Low) / 取得した出力GPIOの電源ON時出力値ビットマスク(各ビット 1:High, 0:Low)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_GetGpioConfig(out UInt32 pullDownBits, out UInt32 initialOutValBits)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            pullDownBits = 0;
            initialOutValBits = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_GPIO_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                pullDownBits = BitConverter.ToUInt32(aResData, 0);
                initialOutValBits = BitConverter.ToUInt32(aResData, 4);
            }

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Get GPIO Input/Output Value" command / 「GPIO入出力値取得」コマンドの要求を送信
        /// </summary>
        /// <param name="inOutValBits">
        /// Acquired GPIO input/output value bitmask (each bit 1: High, 0: Low) / 取得したGPIO入出力値ビットマスク(各ビット 1:High, 0:Low)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_GetGpio(out UInt32 inOutValBits)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            inOutValBits = 0;

            strErrMsg = SendCmd(E_FRM_CMD.GET_GPIO, aReqData, out aResData);
            if (strErrMsg == null)
            {
                inOutValBits = BitConverter.ToUInt32(aResData, 0);
            }

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "GPIO Output" command / 「GPIO出力」コマンドの要求を送信
        /// </summary>
        /// <param name="outValBits">
        /// Bitmask of output GPIO value (each bit 1: High, 0: Low) / 出力GPIO値のビットマスク(各ビット 1:High, 0:Low)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_OutGpio(UInt32 outValBits)
        {
            byte[] aReqData = new byte[4];
            byte[] aResData = null;
            string strErrMsg;

            Array.Copy(BitConverter.GetBytes(outValBits), 0, aReqData, 0, 4);

            strErrMsg = SendCmd(E_FRM_CMD.PUT_GPIO, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "ADC Input" command / 「ADC入力」コマンドの要求を送信
        /// </summary>
        /// <param name="aVolt">
        /// Array of acquired voltage values [V] for each channel (4 elements) / 取得した各チャンネルの電圧値[V]の配列(要素数4)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
        /// Send request for "Change UART Settings" command / 「UART通信設定変更」コマンドの要求を送信
        /// </summary>
        /// <param name="baudrate">
        /// Baud rate (bps) / ボーレート(bps)
        /// </param>
        /// <param name="dataBits">
        /// Data bit length (fixed to 8) / データビット長(8固定)
        /// </param>
        /// <param name="stopBits">
        /// Stop bit (1 or 2) / ストップビット(1 or 2)
        /// </param>
        /// <param name="parity">
        /// Parity (0: None, 1: Odd, 2: Even) / パリティ(0:None, 1:Odd, 2:Even)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SetUartConfig(UInt32 baudrate, byte dataBits, byte stopBits, byte parity)
        {
            byte[] aReqData = new byte[7];
            byte[] aResData = null;
            string strErrMsg;

            if (dataBits != 8)
            {
                //strErrMsg = "パラメータ不正。(データビット)";
                strErrMsg = "Invalid parameter. (Data bits)";
                goto End;
            }

            if (stopBits != 1 && stopBits != 2)
            {
                //strErrMsg = "パラメータ不正。(ストップビット)";
                strErrMsg = "Invalid parameter. (Stop bits)";
                goto End;
            }

            if (parity != 0 && parity != 1 && parity != 2)
            {
                //strErrMsg = "パラメータ不正。(パリティ)";
                strErrMsg = "Invalid parameter. (Parity)";
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
        /// Send request for "Get UART Settings" command / 「UART通信設定取得」コマンドの要求を送信
        /// </summary>
        /// <param name="baudrate">
        /// Acquired baud rate (bps) / 取得したボーレート(bps)
        /// </param>
        /// <param name="dataBits">
        /// Acquired data bit length / 取得したデータビット長
        /// </param>
        /// <param name="stopBits">
        /// Acquired stop bit / 取得したストップビット
        /// </param>
        /// <param name="parity">
        /// Acquired parity / 取得したパリティ
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
        /// Send request for "UART Transmission" command / 「UART送信」コマンドの要求を送信
        /// </summary>
        /// <param name="aReqData">
        /// Transmission data (1 to 256 bytes) / 送信データ(1～256byte)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SendUart(byte[] aReqData)
        {
            byte[] aResData = null;
            string strErrMsg;

            if (aReqData.Length < 1 || aReqData.Length > 256)
            {
                //strErrMsg = "パラメータ不正。(送信データサイズ)";
                strErrMsg = "Invalid parameter. (Transmission data size)";
                goto End;
            }

            strErrMsg = SendCmd(E_FRM_CMD.SEND_UART, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Change SPI Settings" command / 「SPI通信設定変更」コマンドの要求を送信
        /// </summary>
        /// <param name="freq">
        /// Frequency (Hz) / 周波数(Hz)
        /// </param>
        /// <param name="dataBits">
        /// Data bit length (fixed to 8) / データビット長(8固定)
        /// </param>
        /// <param name="polarity">
        /// CPOL(0 or 1) 
        /// </param>
        /// <param name="phase">
        /// CPHA(0 or 1)
        /// </param>
        /// <param name="order">
        /// Bit order (fixed to 1: MSB First) / ビットオーダー(1:MSB First固定)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SetSpiConfig(UInt32 freq, byte dataBits, byte polarity, byte phase, byte order)
        {
            byte[] aReqData = new byte[8];
            byte[] aResData = null;
            string strErrMsg;

            if (dataBits != 8)
            {
                //strErrMsg = "パラメータ不正。(データビット)";
                strErrMsg = "Invalid parameter. (Data bits)";
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
                strErrMsg = "Invalid parameter. (CPHA)";
                goto End;
            }

            if (order != 1)
            {
                //strErrMsg = "パラメータ不正。(ビットオーダー)";
                strErrMsg = "Invalid parameter. (Bit order)";
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
        /// Send request for "Get SPI Settings" command / 「SPI通信設定取得」コマンドの要求を送信 
        /// </summary>
        /// <param name="freq">
        /// Acquired frequency (Hz) / 取得した周波数(Hz)
        /// </param>
        /// <param name="dataBits">
        /// Acquired data bit length / 取得したデータビット長
        /// </param>
        /// <param name="polarity">
        /// Acquired CPOL(0 or 1) / 取得したCPOL(0 or 1)
        /// </param>
        /// <param name="phase">
        /// Acquired CPHA(0 or 1) / 取得したCPHA(0 or 1)
        /// </param>
        /// <param name="order">
        /// Acquired bit order (fixed to 1: MSB First) / 取得したビットオーダー(1:MSB First固定)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
        /// Send request for "SPI Master Send/Receive" command / 「SPIマスタ送受信」コマンドの要求を送信
        /// </summary>
        /// <param name="aReqData">
        /// Transmission data (1 to 256 bytes) / 送信データ(1～256byte)
        /// </param>
        /// <param name="aResData">
        /// Storage destination for received data / 受信データ格納先
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SendSpi(byte[] aReqData, out byte[] aResData)
        {
            string strErrMsg;

            aResData = null;

            if (aReqData.Length < 1 || aReqData.Length > 256)
            {
                //strErrMsg = "パラメータ不正。(送信データサイズ)";
                strErrMsg = "Invalid parameter. (Transmission data size)";
                goto End;
            }

            strErrMsg = SendCmd(E_FRM_CMD.SENDRECV_SPI, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Change I2C Settings" command / 「I2C通信設定変更」コマンドの要求を送信
        /// </summary>
        /// <param name="freq">
        /// Frequency (Hz) / 周波数(Hz)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
        /// Send request for "Get I2C Settings" command / 「I2C通信設定取得」コマンドの要求を送信
        /// </summary>
        /// <param name="freq">
        /// Acquired frequency (Hz) / 取得した周波数(Hz)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
        /// Send request for "I2C Master Transmission" command / 「I2Cマスタ送信」コマンドの要求を送信
        /// </summary>
        /// <param name="slaveAddr">
        /// 7-bit slave address / 7bitスレーブアドレス
        /// </param>
        /// <param name="aReqData">
        /// Transmission data (1 to 256 bytes) / 送信データ(1～256byte)　
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SendI2c(byte slaveAddr, byte[] aReqData)
        {
            byte[] aReqData2 = new byte[aReqData.Length + 3];
            byte[] aResData = null;
            string strErrMsg;

            if (slaveAddr > 0x7F)
            {
                //strErrMsg = "パラメータ不正。(7bitスレーブアドレス)";
                strErrMsg = "Invalid parameter. (7-bit slave address)";
                goto End;
            }

            if (aReqData.Length < 1 || aReqData.Length > 256)
            {
                //strErrMsg = "パラメータ不正。(送信データサイズ)";
                strErrMsg = "Invalid parameter. (Transmission data size)";
                goto End;
            }

            aReqData2[0] = slaveAddr; // Slave address / スレーブアドレス
            aReqData2[1] = (byte)(aReqData.Length & 0xFF); // I2C transmission size / I2C送信サイズ
            aReqData2[2] = (byte)((aReqData.Length >> 8) & 0xFF);
            Array.Copy(aReqData, 0, aReqData2, 3, aReqData.Length); // I2C transmission data / I2C送信データ

            strErrMsg = SendCmd(E_FRM_CMD.SEND_I2C, aReqData2, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "I2C Master Reception" command / 「I2Cマスタ受信」コマンドの要求を送信
        /// </summary>
        /// <param name="slaveAddr">
        /// 7-bit slave address / 7bitスレーブアドレス
        /// </param>
        /// <param name="recvSize">
        /// Receive size / 受信サイズ
        /// </param>
        /// <param name="aResData">
        /// Storage destination for received data / 受信データ格納先
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_RecvI2c(byte slaveAddr, UInt16 recvSize, out byte[] aResData)
        {
            byte[] aReqData = new byte[3];
            string strErrMsg;

            aResData = null;

            if (slaveAddr > 0x7F)
            {
                //strErrMsg = "パラメータ不正。(7bitスレーブアドレス)";
                strErrMsg = "Invalid parameter. (7-bit slave address)";
                goto End;
            }

            if (recvSize < 1 || recvSize > 256)
            {
                //strErrMsg = "パラメータ不正。(受信データサイズ)";
                strErrMsg = "Invalid parameter. (Receive data size)";
                goto End;
            }

            aReqData[0] = slaveAddr; // Slave address / スレーブアドレス
            aReqData[1] = (byte)(recvSize & 0xFF); // I2C receive size / I2C受信サイズ
            aReqData[2] = (byte)((recvSize >> 8) & 0xFF);

            strErrMsg = SendCmd(E_FRM_CMD.RECV_I2C, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Start PWM" command / 「PWM開始」コマンドの要求を送信
        /// </summary>
        /// <param name="clkdiv">
        /// Clock division ratio / クロック分周比
        /// </param>
        /// <param name="wrap">
        /// Wrap / ラップ値
        /// </param>
        /// <param name="level">
        /// Level (comparison value) / レベル(比較値)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_StartPwm(float clkdiv, UInt16 wrap, UInt16 level)
        {
            byte[] aReqData = new byte[8];
            byte[] aResData = null;
            string strErrMsg;

            Array.Copy(BitConverter.GetBytes(clkdiv), 0, aReqData, 0, 4);
            Array.Copy(BitConverter.GetBytes(wrap), 0, aReqData, 4, 2);
            Array.Copy(BitConverter.GetBytes(level), 0, aReqData, 6, 2);

            strErrMsg = SendCmd(E_FRM_CMD.START_PWM, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Stop PWM" command / 「PWM停止」コマンドの要求を送信
        /// </summary>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_StopPwm()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.STOP_PWM, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Get FW Error Info" command / 「FWエラー情報取得」コマンドの要求を送信
        /// </summary>
        /// <param name="lstErrMsg">
        /// List of acquired error messages / 取得したエラーメッセージのリスト
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
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
                if (lstErrMsg == null)
                {
                    lstErrMsg = new List<string>();
                }
                else
                {
                    lstErrMsg.Clear();
                }
                for (int i = 0; i < 32/*FW_ERR_MSG_ARY.Length*/; i++)
                {
                    if ((errBits & (1U << i)) != 0)
                    {
                        if (i < FW_ERR_MSG_ARY.Length)
                        {
                            lstErrMsg.Add(FW_ERR_MSG_ARY[i]);
                        }
                        else
                        {
                            lstErrMsg.Add("Undefined error");
                        }
                    } 
                }
            }

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Clear FW Error" command / 「FWエラークリア」コマンドの要求を送信
        /// </summary>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_ClearFwError()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.CLEAR_FW_ERR, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Erase FLASH" command / 「FLASH消去」コマンドの要求を送信
        /// </summary>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_EraseFlash()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.ERASE_FLASH, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Change Network Settings" command / 「ネットワーク設定変更」コマンドの要求を送信
        /// </summary>
        /// <param name="strCountryCode">
        /// Country code / カントリーコード 
        /// *Sent to the microcontroller, but currently unused. Please specify "XX". / ※マイコン側へ送信されますが、現在は未使用です。"XX"を指定してください。
        /// </param>
        /// <param name="strIpAddr">
        /// IP address (e.g. "192.168.1.10") / IPアドレス(例: "192.168.1.10")
        /// </param>
        /// <param name="strSsid">
        /// SSID / SSID
        /// </param>
        /// <param name="strPassword">
        /// Password / パスワード
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SetNwConfig(string strCountryCode, string strIpAddr, string strSsid, string strPassword)
        {
            byte[] aReqData = new byte[105];
            byte[] aResData = null;
            char[] szCountryCode = new char[3];
            byte[] abyIpAddr = new byte[4];
            char[] szSsid = new char[33];
            char[] szPassword = new char[65];
            string strErrMsg;

            // Country code / カントリーコード
            if (strCountryCode.Length != szCountryCode.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Country code)";
                goto End;
            }
            strCountryCode.CopyTo(0, szCountryCode, 0, strCountryCode.Length);

            // IP address / IPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strIpAddr, out abyIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // SSID / SSID
            if (strSsid.Length > szSsid.Length - 1)
            {
                strErrMsg = "Invalid parameter. (SSID)";
                goto End;
            }
            strSsid.CopyTo(0, szSsid, 0, strSsid.Length);

            // Password / パスワード
            if (strPassword.Length > szPassword.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Password)";
                goto End;
            }
            strPassword.CopyTo(0, szPassword, 0, strPassword.Length);

            // Request data / 要求データ
            Array.Copy(ConvertCharAryToByteAry(szCountryCode), 0, aReqData, 0, szCountryCode.Length);
            Array.Copy(abyIpAddr, 0, aReqData, 3, abyIpAddr.Length);
            Array.Copy(ConvertCharAryToByteAry(szSsid), 0, aReqData, 7, szSsid.Length);
            Array.Copy(ConvertCharAryToByteAry(szPassword), 0, aReqData, 40, szPassword.Length);

            strErrMsg = SendCmd(E_FRM_CMD.SET_NW_CONFIG, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Get Network Settings" command / 「ネットワーク設定取得」コマンドの要求を送信
        /// </summary>
        /// <param name="strCountryCode">
        /// Acquired country code / 取得したカントリーコード
        /// </param>
        /// <param name="strIpAddr">
        /// Acquired IP address / 取得したIPアドレス
        /// </param>
        /// <param name="strSsid">
        /// Acquired SSID / 取得したSSID
        /// </param>
        /// <param name="strPassword">
        /// Acquired password / 取得したパスワード
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_GetNwConfig(out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            byte[] abyIpAddr = new byte[4];
            string strErrMsg;

            strCountryCode = null;
            strIpAddr = null;
            strSsid = null;
            strPassword = null;

            strErrMsg = SendCmd(E_FRM_CMD.GET_NW_CONFIG, aReqData, out aResData);
            if (strErrMsg == null)
            {
                Array.Copy(aResData, 3, abyIpAddr, 0, abyIpAddr.Length);
                strCountryCode = System.Text.Encoding.ASCII.GetString(aResData, 0, 3).TrimEnd('\0');
                strIpAddr = abyIpAddr[0].ToString() + "." + abyIpAddr[1].ToString() + "." + abyIpAddr[2].ToString() + "." + abyIpAddr[3].ToString();
                strSsid = System.Text.Encoding.ASCII.GetString(aResData, 7, 33).TrimEnd('\0');
                strPassword = System.Text.Encoding.ASCII.GetString(aResData, 40, 65).TrimEnd('\0');
            }

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Change Network Settings 2" command / 「ネットワーク設定変更2」コマンドの要求を送信
        /// </summary>
        /// <param name="isWifi">
        /// Wi-Fi enabled flag (true: enabled, false: disabled) / Wi-Fi有効フラグ(true:有効, false:無効)
        /// </param>
        /// <param name="strCountryCode">
        /// Country code / カントリーコード
        /// </param>
        /// <param name="strIpAddr">
        /// IP address / IPアドレス
        /// </param>
        /// <param name="strSsid">
        /// SSID / SSID
        /// </param>
        /// <param name="strPassword">
        /// Password / パスワード
        /// </param>
        /// <param name="strServerIpAddr">
        /// Server IP address / サーバーのIPアドレス
        /// </param>
        /// <param name="isClient">
        /// Client mode flag (true: client, false: server) / クライアントモードフラグ(true:クライアント, false:サーバー)
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SetNwConfig2(bool isWifi, string strCountryCode, string strIpAddr, string strSsid, string strPassword, string strServerIpAddr, bool isClient)
        {
            byte[] aReqData = new byte[111];
            byte[] aResData = null;
            char[] szCountryCode = new char[3];
            byte[] abyIpAddr = new byte[4];
            char[] szSsid = new char[33];
            char[] szPassword = new char[65];
            byte[] abyServerIpAddr = new byte[4];
            byte byIsClient = 0;
            byte byIsWifi = 0;
            string strErrMsg;

            // Country code / カントリーコード
            if (strCountryCode.Length != szCountryCode.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Country code)";
                goto End;
            }
            strCountryCode.CopyTo(0, szCountryCode, 0, strCountryCode.Length);

            // IP address / IPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strIpAddr, out abyIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // SSID / SSID
            if (strSsid.Length > szSsid.Length - 1)
            {
                strErrMsg = "Invalid parameter. (SSID)";
                goto End;
            }
            strSsid.CopyTo(0, szSsid, 0, strSsid.Length);

            // Password / パスワード
            if (strPassword.Length > szPassword.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Password)";
                goto End;
            }
            strPassword.CopyTo(0, szPassword, 0, strPassword.Length);

            // Server IP address / サーバーのIPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strServerIpAddr, out abyServerIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // Request data / 要求データ
            if (isWifi)
            {
                byIsWifi = 1;
            }
            aReqData[0] = byIsWifi;
            Array.Copy(ConvertCharAryToByteAry(szCountryCode), 0, aReqData, 1, szCountryCode.Length);
            Array.Copy(abyIpAddr, 0, aReqData, 4, abyIpAddr.Length);
            Array.Copy(ConvertCharAryToByteAry(szSsid), 0, aReqData, 8, szSsid.Length);
            Array.Copy(ConvertCharAryToByteAry(szPassword), 0, aReqData, 41, szPassword.Length);
            Array.Copy(abyServerIpAddr, 0, aReqData, 106, abyServerIpAddr.Length);
            if (isClient)
            {
                byIsClient = 1;
            }
            aReqData[110] = byIsClient;

            strErrMsg = SendCmd(E_FRM_CMD.SET_NW_CONFIG2, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Get Network Settings 2" command / 「ネットワーク設定取得2」コマンドの要求を送信
        /// </summary>
        /// <param name="isWifi">
        /// Acquired Wi-Fi enabled flag / 取得したWi-Fi有効フラグ
        /// </param>
        /// <param name="strCountryCode">
        /// Acquired country code / 取得したカントリーコード
        /// </param>
        /// <param name="strIpAddr">
        /// Acquired IP address / 取得したIPアドレス
        /// </param>
        /// <param name="strSsid">
        /// Acquired SSID / 取得したSSID
        /// </param>
        /// <param name="strPassword">
        /// Acquired password / 取得したパスワード
        /// </param>
        /// <param name="strServerIpAddr">
        /// Acquired server IP address / 取得したサーバーのIPアドレス
        /// </param>
        /// <param name="isClient">
        /// Acquired client mode flag / 取得したクライアントモードフラグ
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_GetNwConfig2(out bool isWifi, out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword, out string strServerIpAddr, out bool isClient)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            byte[] abyIpAddr = new byte[4];
            byte[] abyServerIpAddr = new byte[4];
            string strErrMsg;

            strCountryCode = null;
            strIpAddr = null;
            strSsid = null;
            strPassword = null;
            strServerIpAddr = null;
            isClient = false;
            isWifi = false;

            strErrMsg = SendCmd(E_FRM_CMD.GET_NW_CONFIG2, aReqData, out aResData);
            if (strErrMsg == null)
            {
                if (aResData[0] == 1)
                {
                    isWifi = true;
                }

                Array.Copy(aResData, 4, abyIpAddr, 0, abyIpAddr.Length);
                Array.Copy(aResData, 106, abyServerIpAddr, 0, abyServerIpAddr.Length);

                strCountryCode = System.Text.Encoding.ASCII.GetString(aResData, 1, 3).TrimEnd('\0');
                strIpAddr = abyIpAddr[0].ToString() + "." + abyIpAddr[1].ToString() + "." + abyIpAddr[2].ToString() + "." + abyIpAddr[3].ToString();
                strSsid = System.Text.Encoding.ASCII.GetString(aResData, 8, 33).TrimEnd('\0');
                strPassword = System.Text.Encoding.ASCII.GetString(aResData, 41, 65).TrimEnd('\0');
                strServerIpAddr = abyServerIpAddr[0].ToString() + "." + abyServerIpAddr[1].ToString() + "." + abyServerIpAddr[2].ToString() + "." + abyServerIpAddr[3].ToString();

                if (aResData[110] == 1)
                {
                    isClient = true;
                }
            }

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Change Network Settings 3" command / 「ネットワーク設定変更3」コマンドの要求を送信
        /// </summary>
        /// <param name="isWifi">
        /// Wi-Fi enabled flag / Wi-Fi有効フラグ
        /// </param>
        /// <param name="strCountryCode">
        /// Country code / カントリーコード
        /// </param>
        /// <param name="strIpAddr">
        /// IP address / IPアドレス
        /// </param>
        /// <param name="strSsid">
        /// SSID / SSID
        /// </param>
        /// <param name="strPassword">
        /// Password / パスワード
        /// </param>
        /// <param name="strServerIpAddr">
        /// Server IP address / サーバーのIPアドレス
        /// </param>
        /// <param name="isClient">
        /// Client mode flag / クライアントモードフラグ
        /// </param>
        /// <param name="strGMailAddress">
        /// Gmail address / Gmailアドレス
        /// </param>
        /// <param name="strGMailAppPassword">
        /// Gmail app password / Gmailアプリパスワード
        /// </param>
        /// <param name="strToEMailAddress">
        /// Destination email address / 送信先メールアドレス
        /// </param>
        /// <param name="mailIntervalHour">
        /// Mail transmission interval (hours) / メール送信間隔(時間)
        /// </param>
        /// <param name="strName">
        /// Identification name / 識別名
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_SetNwConfig3(bool isWifi, string strCountryCode, string strIpAddr, string strSsid, string strPassword, string strServerIpAddr, bool isClient, string strGMailAddress, string strGMailAppPassword, string strToEMailAddress, byte mailIntervalHour, string strName)
        {
            byte[] aReqData = new byte[279];
            byte[] aResData = null;
            char[] szCountryCode = new char[3];
            byte[] abyIpAddr = new byte[4];
            char[] szSsid = new char[33];
            char[] szPassword = new char[65];
            byte[] abyServerIpAddr = new byte[4];
            byte byIsClient = 0;
            byte byIsWifi = 0;
            char[] szGMailAddress = new char[65];
            char[] szGMailAppPassword = new char[20];
            char[] szToEMailAddress = new char[65];
            char[] szName = new char[17];
            string strErrMsg;

            // Country code / カントリーコード
            if (strCountryCode.Length != szCountryCode.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Country code)";
                goto End;
            }
            strCountryCode.CopyTo(0, szCountryCode, 0, strCountryCode.Length);

            // IP address / IPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strIpAddr, out abyIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // SSID / SSID
            if (strSsid.Length > szSsid.Length - 1)
            {
                strErrMsg = "Invalid parameter. (SSID)";
                goto End;
            }
            strSsid.CopyTo(0, szSsid, 0, strSsid.Length);

            // Password / パスワード
            if (strPassword.Length > szPassword.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Password)";
                goto End;
            }
            strPassword.CopyTo(0, szPassword, 0, strPassword.Length);

            // Server IP address / サーバーのIPアドレス
            strErrMsg = ConvertIpAddrStringToByteArray(strServerIpAddr, out abyServerIpAddr);
            if (strErrMsg != null)
            {
                goto End;
            }

            // Gmail address / Gmailアドレス
            if (strGMailAddress.Length > szGMailAddress.Length - 1)
            {
                strErrMsg = "Invalid parameter. (My Gmail Address)";
                goto End;
            }
            strGMailAddress.CopyTo(0, szGMailAddress, 0, strGMailAddress.Length);

            // Google Account App Password / Googleアカウントのアプリパスワード
            if (strGMailAppPassword.Length > szGMailAppPassword.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Gmail Account App Password)";
                goto End;
            }
            strGMailAppPassword.CopyTo(0, szGMailAppPassword, 0, strGMailAppPassword.Length);

            // Destination E-Mail Address / 宛先E-Mailアドレス
            if (strToEMailAddress.Length > szToEMailAddress.Length - 1)
            {
                strErrMsg = "Invalid parameter. (To E-Mail Address)";
                goto End;
            }
            strToEMailAddress.CopyTo(0, szToEMailAddress, 0, strToEMailAddress.Length);

            // Identification name / 識別名
            if (strName.Length > szName.Length - 1)
            {
                strErrMsg = "Invalid parameter. (Name)";
                goto End;
            }
            strName.CopyTo(0, szName, 0, strName.Length);

            // Request data / 要求データ
            if (isWifi)
            {
                byIsWifi = 1;
            }
            aReqData[0] = byIsWifi;
            Array.Copy(ConvertCharAryToByteAry(szCountryCode), 0, aReqData, 1, szCountryCode.Length);
            Array.Copy(abyIpAddr, 0, aReqData, 4, abyIpAddr.Length);
            Array.Copy(ConvertCharAryToByteAry(szSsid), 0, aReqData, 8, szSsid.Length);
            Array.Copy(ConvertCharAryToByteAry(szPassword), 0, aReqData, 41, szPassword.Length);
            Array.Copy(abyServerIpAddr, 0, aReqData, 106, abyServerIpAddr.Length);
            if (isClient)
            {
                byIsClient = 1;
            }
            aReqData[110] = byIsClient;
            Array.Copy(ConvertCharAryToByteAry(szGMailAddress), 0, aReqData, 111, szGMailAddress.Length);
            Array.Copy(ConvertCharAryToByteAry(szGMailAppPassword), 0, aReqData, 176, szGMailAppPassword.Length);
            Array.Copy(ConvertCharAryToByteAry(szToEMailAddress), 0, aReqData, 196, szToEMailAddress.Length);
            aReqData[261] = mailIntervalHour;
            Array.Copy(ConvertCharAryToByteAry(szName), 0, aReqData, 262, szName.Length);

            strErrMsg = SendCmd(E_FRM_CMD.SET_NW_CONFIG3, aReqData, out aResData);

        End:
            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Get Network Settings 3" command / 「ネットワーク設定取得3」コマンドの要求を送信
        /// </summary>
        /// <param name="isWifi">
        /// Acquired Wi-Fi enabled flag / 取得したWi-Fi有効フラグ
        /// </param>
        /// <param name="strCountryCode">
        /// Acquired country code / 取得したカントリーコード
        /// </param>
        /// <param name="strIpAddr">
        /// Acquired IP address / 取得したIPアドレス
        /// </param>
        /// <param name="strSsid">
        /// Acquired SSID / 取得したSSID
        /// </param>
        /// <param name="strPassword">
        /// Acquired password / 取得したパスワード
        /// </param>
        /// <param name="strServerIpAddr">
        /// Acquired server IP address / 取得したサーバーのIPアドレス
        /// </param>
        /// <param name="isClient">
        /// Acquired client mode flag / 取得したクライアントモードフラグ
        /// </param>
        /// <param name="strGMailAddress">
        /// Acquired Gmail address / 取得したGmailアドレス
        /// </param>
        /// <param name="strGMailAppPassword">
        /// Acquired Gmail app password / 取得したGmailアプリパスワード
        /// </param>
        /// <param name="strToEMailAddress">
        /// Acquired destination email address / 取得した送信先メールアドレス
        /// </param>
        /// <param name="mailIntervalHour">
        /// Acquired mail transmission interval (hours) / 取得したメール送信間隔(時間)
        /// </param>
        /// <param name="strName">
        /// Acquired identification name / 取得した識別名
        /// </param>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_GetNwConfig3(out bool isWifi, out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword, out string strServerIpAddr, out bool isClient, out string strGMailAddress, out string strGMailAppPassword, out string strToEMailAddress, out byte mailIntervalHour, out string strName)
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            byte[] abyIpAddr = new byte[4];
            byte[] abyServerIpAddr = new byte[4];
            string strErrMsg;

            strCountryCode = null;
            strIpAddr = null;
            strSsid = null;
            strPassword = null;
            strServerIpAddr = null;
            isClient = false;
            isWifi = false;
            strGMailAddress = null;
            strGMailAppPassword = null;
            strToEMailAddress = null;
            mailIntervalHour = 0;
            strName = null;

            strErrMsg = SendCmd(E_FRM_CMD.GET_NW_CONFIG3, aReqData, out aResData);
            if (strErrMsg == null)
            {
                if (aResData[0] == 1)
                {
                    isWifi = true;
                }

                Array.Copy(aResData, 4, abyIpAddr, 0, abyIpAddr.Length);
                Array.Copy(aResData, 106, abyServerIpAddr, 0, abyServerIpAddr.Length);

                strCountryCode = System.Text.Encoding.ASCII.GetString(aResData, 1, 3).TrimEnd('\0');
                strIpAddr = abyIpAddr[0].ToString() + "." + abyIpAddr[1].ToString() + "." + abyIpAddr[2].ToString() + "." + abyIpAddr[3].ToString();
                strSsid = System.Text.Encoding.ASCII.GetString(aResData, 8, 33).TrimEnd('\0');
                strPassword = System.Text.Encoding.ASCII.GetString(aResData, 41, 65).TrimEnd('\0');
                strServerIpAddr = abyServerIpAddr[0].ToString() + "." + abyServerIpAddr[1].ToString() + "." + abyServerIpAddr[2].ToString() + "." + abyServerIpAddr[3].ToString();
                if (aResData[110] == 1)
                {
                    isClient = true;
                }
                strGMailAddress = System.Text.Encoding.ASCII.GetString(aResData, 111, 65).TrimEnd('\0');
                strGMailAppPassword = System.Text.Encoding.ASCII.GetString(aResData, 176, 20).TrimEnd('\0');
                strToEMailAddress = System.Text.Encoding.ASCII.GetString(aResData, 196, 65).TrimEnd('\0');
                mailIntervalHour = aResData[261];
                strName = System.Text.Encoding.ASCII.GetString(aResData, 262, 17).TrimEnd('\0');
            }

            return strErrMsg;
        }

        /// <summary>
        /// Send request for "Reset MCU" command / 「マイコンリセット」コマンドの要求を送信
        /// </summary>
        /// <returns>
        /// Error message. Returns null on success. / エラーメッセージ。成功時はnullを返す。
        /// </returns>
        public string SendCmd_ResetMcu()
        {
            byte[] aReqData = null;
            byte[] aResData = null;
            string strErrMsg;

            strErrMsg = SendCmd(E_FRM_CMD.RESET_MCU, aReqData, out aResData);

            return strErrMsg;
        }

        /// <summary>
        /// Send a request frame / 要求フレームを送信
        /// </summary>
        private string SendCmd(E_FRM_CMD eCmd, byte[] aReqData, out byte[] aResData, int resTimeout = FRM_RES_TIMEOUT)
        {
            byte[] aReqFrm;            // Request frame / 要求フレーム
            string strErrMsg = null;   // Error message / エラーメッセージ
            ST_FRM_REQ_FRAME stReqFrm; // Request frame / 要求フレーム
            ST_FRM_RES_FRAME stResFrm; // Response frame / 応答フレーム

            lock (_lockSend) // Lock to prevent the next transmission while waiting for a response to transmission / 送信～応答待ち中は、次の送信をしないようにするためのロック
            {
                aResData = null;

                // Empty the response frame receive queue / 応答フレーム受信キューを空にする
                PrpResEvent.Reset();
                while (true == PrpResFrmQue.TryTake(out stResFrm)) { }

                // [Create a request frame] / [要求フレームを作成]
                stReqFrm.header = E_FRM_HEADER.REQ; // Header / ヘッダ
                unchecked                           // Prevent OverflowException / オーバーフロー例外を防止
                {
                    stReqFrm.seqNo = _seqNo++;      // Sequence number / シーケンス番号
                }
                stReqFrm.cmd = eCmd;                // Command / コマンド
                if (aReqData == null) // If the data part is empty / データ部が空の場合
                {
                    stReqFrm.dataSize = 0;  // Data size / データサイズ
                    stReqFrm.aData = null;  // Data / データ
                }
                else // If the data part is not empty / データ部が空ではない場合
                {
                    stReqFrm.dataSize = (UInt16)aReqData.Length; // Data size / データサイズ
                    stReqFrm.aData = aReqData;                   // Data / データ
                }
                // Get the byte array of the request frame before calculating the checksum / チェックサム計算前の要求フレームのbyte型配列を取得
                stReqFrm.checksum = 0;
                aReqFrm = ConvertReqFrameStructToByteArray(stReqFrm);
                // Calculate the checksum / チェックサムを計算 
                stReqFrm.checksum = CalcChecksum(aReqFrm, aReqFrm.Length - 2);

                // [Send the request frame] / [要求フレームを送信]
                // Write the calculated checksum directly to the byte array / 計算したチェックサムをbyte型配列に直接書き込む
                aReqFrm[aReqFrm.Length - 2] = (byte)(stReqFrm.checksum & 0xFF);
                aReqFrm[aReqFrm.Length - 1] = (byte)((stReqFrm.checksum >> 8) & 0xFF);

                // Send the request frame / 要求フレームを送信    
                strErrMsg = Send(aReqFrm);
                if (strErrMsg != null)
                {

                    goto End;
                }

                // [In case of command without response] / [応答無しのコマンドの場合]
                switch (eCmd)
                {
                    case E_FRM_CMD.SEND_UART:
                        goto End;
                    default:
                        break;
                }

                // [Wait for response frame reception event to occur] / [応答フレーム受信イベント発生待ち]
                while (true)
                {
                    if (!PrpResEvent.WaitOne(resTimeout))
                    {
                        strErrMsg = "Response frame reception timeout.";
                        goto End;
                    }

                    // [Retrieve the response frame from the response frame receive queue] / [応答フレーム受信キューから応答フレームを取り出す]
                    if (PrpResFrmQue.TryTake(out stResFrm))
                    {
                        if (stResFrm.seqNo != stReqFrm.seqNo || stResFrm.cmd != stReqFrm.cmd)
                        {
                            // Mismatched response (e.g., from a previous timeout). Reset event and wait again. / 不一致の応答(以前のタイムアウトによるものなど)。イベントをリセットして再度待機。
                            PrpResEvent.Reset();
                            continue;
                        }

                        if (stResFrm.errCode == E_FRM_ERRCODE.SUCCESS)
                        {
                            aResData = new byte[stResFrm.aData.Length];
                            Array.Copy(stResFrm.aData, aResData, aResData.Length);
                        }
                        else
                        {
                            strErrMsg = ConvertErrCodeInResFrameToMsg(stResFrm.errCode);
                        }
                        goto End;
                    }
                    else
                    {
                        if (_isDisconnecting)
                        {
                            strErrMsg = STR_MSG_WAIT_RES_CANCEL;
                            goto End;
                        }
                        // Event was set but queue is empty. Reset and retry. / イベントがセットされたがキューが空。リセットしてリトライ。
                        PrpResEvent.Reset();
                    }
                }
        End:

                return strErrMsg;
            }
        }

        /// <summary>
        /// Convert the request frame structure to a byte array / 要求フレーム構造体をbyte型配列へ変換する
        /// </summary>
        private byte[] ConvertReqFrameStructToByteArray(ST_FRM_REQ_FRAME stReqFrm)
        {
            List<byte[]> lst = new List<byte[]>(); // List of byte arrays / byte型配列のリスト

            // Convert each field of the request frame structure into a byte array and add it to the list / 要求フレーム構造体の各フィールドをbyte型配列に変換してリストに追加
            lst.Add(new byte[1] { (byte)stReqFrm.header });
            lst.Add(BitConverter.GetBytes(stReqFrm.seqNo));
            lst.Add(BitConverter.GetBytes((UInt16)stReqFrm.cmd));
            lst.Add(BitConverter.GetBytes(stReqFrm.dataSize));
            lst.Add(stReqFrm.aData);
            lst.Add(BitConverter.GetBytes(stReqFrm.checksum));

            // Combine the list into a single byte array and return it / リストを1つのbyte型配列に結合して返す
            return CombineByteArray(lst);
        }

        /// <summary>
        /// Combine the list of byte arrays in the argument into a single byte array and return it / 引数のbyte型配列のリストを1つのbyte型配列に結合して返す
        /// </summary>
        private byte[] CombineByteArray(List<byte[]> lst)
        {
            // Determine the size of the byte array to return / 返却するbyte型配列のサイズを求める
            int size = 0;
            foreach (byte[] ary in lst)
            {
                if (ary != null)
                {
                    size += ary.Length;
                }
            }

            // Combine the list of byte arrays in the argument into a single byte array / 引数のbyte型配列のリストを1つのbyte型配列に結合する
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
        /// Convert an IP address string to a byte array / IPアドレスの文字列をbyte型の配列に変換する
        /// </summary>
        /// <remarks>
        /// The separator is a dot. / セパレータはドット。
        /// </remarks>
        private string ConvertIpAddrStringToByteArray(string strText, out byte[] aVal)
        {
            char[] aSeparator = { '.' }; // Separator / セパレータ
            strText = strText.Replace("\r", "").Replace("\n", "");
            string strErrMsg = ConvertStringToValArray(strText, aSeparator, 10, out aVal);
            bool isErr = false;

            if (strErrMsg == null)
            {
                if (aVal.Length != 4)
                {
                    isErr = true;
                }
            }
            else
            {
                isErr = true;
            }

            if (isErr)
            {
                strErrMsg = "Invalid parameter. (IP address)";
            }

            return strErrMsg;
        }

        /// <summary>
        /// Convert a string to a byte array / 文字列をbyte型の配列に変換する
        /// </summary>
        private string ConvertStringToValArray(string strText, char[] aSeparator, int baseNumber, out byte[] aVal)
        {
            string[] astrSplit; // String after splitting / 分割後の文字列
            string strErrMsg = null;

            // Split the string by the separator / 文字列をセパレータで分割
            astrSplit = strText.Split(aSeparator);

            // [Convert the string to a byte array] / [文字列をbyte型の配列に変換]
            // Prepare a byte array with the number of elements equal to the number of split strings / 要素数が分割された文字列の数であるbyte型配列を用意
            aVal = new byte[astrSplit.Length];
            // Convert the string to byte type for the number of split strings / 分割された文字列の数だけ、文字列をbyte型に変換
            for (int i = 0; i < astrSplit.Length; i++)
            {
                try
                {
                    aVal[i] = Convert.ToByte(astrSplit[i], baseNumber);
                }
                catch (Exception ex)
                {
                    strErrMsg = ex.Message;
                    break;
                }
            }

            return strErrMsg;
        }

        /// <summary>
        /// Convert a char array to a byte array / char型の配列をbyte型の配列に変換する
        /// </summary>
        private byte[] ConvertCharAryToByteAry(char[] achArray)
        {
            byte[] abyArray = new byte[achArray.Length];

            for (int i = 0; i < achArray.Length; i++)
            {
                abyArray[i] = (byte)achArray[i];
            }

            return abyArray;
        }
    }
}
