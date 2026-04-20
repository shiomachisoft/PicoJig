# PicoJigLib.dll User Guide

## Table of Contents
- [Introduction](#introduction)
- [Basic Specifications](#basic-specifications)
- [Notes](#notes)
- [1. Class Initialization and Communication Method Selection](#1-class-initialization-and-communication-method-selection)
- [2. Connection and Disconnection Methods](#2-connection-and-disconnection-methods)
- [3. Wi-Fi Settings](#3-wi-fi-settings)
- [4. Getting FW Information](#4-getting-fw-information)
- [5. GPIO Settings and I/O](#5-gpio-settings-and-io)
- [6. ADC (Analog-to-Digital Conversion)](#6-adc-analog-to-digital-conversion)
- [7. UART Communication](#7-uart-communication)
- [8. SPI Communication](#8-spi-communication)
- [9. I2C Communication](#9-i2c-communication)
- [10. PWM Output](#10-pwm-output)
- [11. Flash Operations](#11-flash-operations)

## Introduction
This manual summarizes the basic usage and available methods and properties for the JigCmd class and its derived classes in the C# DLL, PicoJigLib.dll.  

This library is used to send commands to a microcontroller (Raspberry Pi Pico/Raspberry Pi Pico W) via USB Virtual COM or Wi-Fi TCP/IP communication to control its peripherals.

## Basic Specifications
The return values of all command transmission methods (SendCmd_***) and the Connect method are of type `string`.
- On success: Returns `null`.
- On failure: Returns a message string indicating the error details.

## Notes
When executing various configuration methods (SendCmd_SetNwConfig, SendCmd_SetGpioConfig, SendCmd_SetUartConfig, SendCmd_SetI2cConfig, SendCmd_SetSpiConfig), the configuration data is written to the Flash memory of the microcontroller, after which the microcontroller is reset. Therefore, you must wait 5 seconds before reconnecting.  
For reconnection, refer to ReconnectMcu() in the sample program.

## 1. Class Initialization and Communication Method Selection
Instantiate a derived class of JigCmd depending on the communication method.

- **JigSerial Class**  
  Use this when using a USB Virtual COM.  
  Example: `JigCmd jigCmd = new JigSerial();`

- **JigTcpClient Class**  
  Use this when using Wi-Fi TCP/IP communication.  
  Example: `JigCmd jigCmd = new JigTcpClient();`

## 2. Connection and Disconnection Methods
- `string Connect(Object objParam)`

  Connects to the line.
  - objParam: Parameters required for connection (e.g., port name, IP address)

- `void Disconnect()`

  Disconnects the line.

- `bool IsConnected()`

  Gets whether it is connected to the line. (Return value: true: connected, false: disconnected)


## 3. Wi-Fi Settings
- `string SendCmd_SetNwConfig(string strCountryCode, string strIpAddr, string strSsid, string strPassword)`

  Sends a "Change Network Configuration" command request.
  - strCountryCode: Country code *Sent to the MCU but currently unused. Please specify "XX".
  - strIpAddr: IP address (e.g., "192.168.1.10")
  - strSsid: SSID
  - strPassword: Password  
  
  *Since the microcontroller will be reset when this method is executed, please refer to [Notes].

- `string SendCmd_GetNwConfig(out string strCountryCode, out string strIpAddr, out string strSsid, out string strPassword)`

  Sends a "Get Network Configuration" command request.
  - strCountryCode: Retrieved country code
  - strIpAddr: Retrieved IP address
  - strSsid: Retrieved SSID
  - strPassword: Retrieved password

## 4. Getting FW Information
- `string SendCmd_GetFwInfo(out string strMakerName, out string strFwName, out string strFwVer, out string strBoardId)`

  Sends a "Get FW Information" command request.
  - strMakerName: Retrieved manufacturer name (16 characters)
  - strFwName: Retrieved FW name (16 characters)
  - strFwVer: Retrieved FW version (8-character hex string)
  - strBoardId: Retrieved Board ID (16-character hex string)

- `string SendCmd_GetFwError(ref List<string> lstErrMsg)`

  Sends a "Get FW Error Information" command request.
  - lstErrMsg: Retrieved list of error messages

- `string SendCmd_ClearFwError()`

  Sends a "Clear FW Error" command request.

## 5. GPIO Settings and I/O

### Used Pins
- Input GP numbers: GP3(Pin 5), GP4(Pin 6), GP5(Pin 7), GP8(Pin 11), GP9(Pin 12), GP10(Pin 14), GP11(Pin 15)
- Output GP numbers: GP12(Pin 16), GP13(Pin 17), GP14(Pin 19), GP15(Pin 20), GP20(Pin 26), GP21(Pin 27), GP22(Pin 29)
*Numbers in parentheses are physical pin numbers.
### Bitmask
The bitmask (bit data) handled by the arguments of each method represents the bit position (1 << GP number) corresponding to the target GP number.


- `string SendCmd_GetGpioConfig(out UInt32 pullDownBits, out UInt32 initialOutValBits)`

  Sends a "Get GPIO Configuration" command request.
  - pullDownBits: Retrieved internal pull-up/pull-down configuration bitmask of input GPIOs (Each bit 1: Pull-down, 0: Pull-up)
  - initialOutValBits: Retrieved output value bitmask of output GPIOs at power-on (Each bit 1: High, 0: Low)

- `string SendCmd_SetGpioConfig(UInt32 pullDownBits, UInt32 initialOutValBits)`

  Sends a "Change GPIO Configuration" command request.
  - pullDownBits: Internal pull-up/pull-down configuration bitmask of input GPIOs (Each bit 1: Pull-down, 0: Pull-up)
  - initialOutValBits: Output value bitmask of output GPIOs at power-on (Each bit 1: High, 0: Low)

  *Since the microcontroller will be reset when this method is executed, please refer to [Notes].

- `string SendCmd_GetGpio(out UInt32 inOutValBits)`

  Sends a "Get GPIO I/O Value" command request.
  - inOutValBits: Retrieved GPIO I/O value bitmask (Each bit 1: High, 0: Low)

- `string SendCmd_OutGpio(UInt32 outValBits)`

  Sends a "GPIO Output" command request.
  - outValBits: Bitmask of output GPIO values (Each bit 1: High, 0: Low)

## 6. ADC (Analog-to-Digital Conversion)

### Used Pins
- ADC0: GP26 (Pin 31)
- ADC1: GP27 (Pin 32)
- ADC2: GP28 (Pin 34)
- ADC4: Temperature Sensor

- `string SendCmd_GetAdc(out float[] aVolt)`

  Sends an "ADC Input" command request.
  - aVolt: Retrieved array of voltage values [V] for each channel (Array length 4)

## 7. UART Communication

### Used Pins
- UART0 TX: GP0 (Pin 1)
- UART0 RX: GP1 (Pin 2)


- `string SendCmd_GetUartConfig(out UInt32 baudrate, out byte dataBits, out byte stopBits, out byte parity)`

  Sends a "Get UART Communication Configuration" command request.
  - baudrate: Retrieved baud rate (bps)
  - dataBits: Retrieved data bit length
  - stopBits: Retrieved stop bit (1 or 2)
  - parity: Retrieved parity (0: None, 1: Even, 2: Odd)

- `string SendCmd_SetUartConfig(UInt32 baudrate, byte dataBits, byte stopBits, byte parity)`

  Sends a "Change UART Communication Configuration" command request.
  - baudrate: Baud rate (bps)
  - dataBits: Data bit length (Fixed to 8)
  - stopBits: Stop bit (1 or 2)
  - parity: Parity (0: None, 1: Even, 2: Odd)
  
  *Since the microcontroller will be reset when this method is executed, please refer to [Notes].

- `string SendCmd_SendUart(byte[] aReqData)`

  Sends a "UART Send" command request.
  - aReqData: Data to send (1 to 256 bytes)

- Property: `BlockingCollection<byte> PrpUartRecvDataQue { get; set; }`

  Queue for UART received data.

## 8. SPI Communication

### Used Pins
- SPI0 RX: GP16 (Pin 21)
- SPI0 CSn: GP17 (Pin 22) *This is software control using GPIO instead of hardware CS.
- SPI0 SCK: GP18 (Pin 24)
- SPI0 TX: GP19 (Pin 25)

- `string SendCmd_GetSpiConfig(out UInt32 freq, out byte dataBits, out byte polarity, out byte phase, out byte order)`

  Sends a "Get SPI Communication Configuration" command request.
  - freq: Retrieved frequency (Hz)
  - dataBits: Retrieved data bit length
  - polarity: Retrieved CPOL (0 or 1)
  - phase: Retrieved CPHA (0 or 1)
  - order: Retrieved bit order (1: MSB First)

- `string SendCmd_SetSpiConfig(UInt32 freq, byte dataBits, byte polarity, byte phase, byte order)`

  Sends a "Change SPI Communication Configuration" command request.
  - freq: Frequency (Hz)
  - dataBits: Data bit length (Fixed to 8)
  - polarity: CPOL (0 or 1)
  - phase: CPHA (0 or 1)
  - order: Bit order (Fixed to 1: MSB First)
  
  *Since the microcontroller will be reset when this method is executed, please refer to [Notes].

- `string SendCmd_SendSpi(byte[] aReqData, out byte[] aResData)`

  Sends a "SPI Master Send/Receive" command request.
  - aReqData: Data to send (1 to 256 bytes)
  - aResData: Destination to store received data

## 9. I2C Communication

### Used Pins
- I2C1 SDA: GP6 (Pin 9)
- I2C1 SCL: GP7 (Pin 10)

- `string SendCmd_GetI2cConfig(out UInt32 freq)`

  Sends a "Get I2C Communication Configuration" command request.
  - freq: Retrieved frequency (Hz)

- `string SendCmd_SetI2cConfig(UInt32 freq)`

  Sends a "Change I2C Communication Configuration" command request.
  - freq: Frequency (Hz)
  
  *Since the microcontroller will be reset when this method is executed, please refer to [Notes].

- `string SendCmd_SendI2c(byte slaveAddr, byte[] aReqData)`

  Sends an "I2C Master Send" command request.
  - slaveAddr: 7-bit slave address
  - aReqData: Data to send (1 to 256 bytes)

- `string SendCmd_RecvI2c(byte slaveAddr, UInt16 recvSize, out byte[] aResData)`

  Sends an "I2C Master Receive" command request.
  - slaveAddr: 7-bit slave address
  - recvSize: Receive size
  - aResData: Destination to store received data

## 10. PWM Output

### Used Pins
- PWM: GP2 (Pin 4)

- `string SendCmd_StartPwm(float clkdiv, UInt16 wrap, UInt16 level)`

  Sends a "Start PWM" command request.
  - clkdiv: Clock division ratio
  - wrap: Wrap value
  - level: Level (comparison value)

  The PWM frequency and duty cycle are calculated by the following formulas.
  - PWM Frequency = 125MHz / ((Wrap value + 1) * Clock division ratio)
  - Duty Cycle = Level / (Wrap value + 1)

- `string SendCmd_StopPwm()`

  Sends a "Stop PWM" command request.

## 11. Flash Operations
- `string SendCmd_EraseFlash()`

  Sends an "Erase FLASH" command request.