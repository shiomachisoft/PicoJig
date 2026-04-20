# PicoJig / PicoJig-WL

[![English](https://img.shields.io/badge/Language-English-blue.svg)](#english) [![日本語](https://img.shields.io/badge/Language-日本語-red.svg)](#japanese)

This repository provides documentation in both **English** and **Japanese**. Please refer to the respective sections below.  
各ドキュメントは**英語版**と**日本語版**を用意しています。リンクから各セクションをご参照ください。

---

## Documentation / ドキュメント

| Document Name / ドキュメント名 | English | 日本語 | Description / 概要 |
| :--- | :---: | :---: | :--- |
| **PicoJig / PicoJig-WL Manual** | [English](./docs/en/01_GUIApp_FW_Manual/README.md) | [日本語](./docs/ja/01_GUIアプリ_FW_マニュアル/README.md) | FW & GUI App setup, usage, and system overview.<br>(FW・GUIアプリの設定や使い方の総合マニュアル) |
| **PicoJigLib.dll User Guide** | [English](./docs/en/02_Library_User_Guide/README.md) | [日本語](./docs/ja/02_ライブラリ利用ガイド/README.md) | API reference for the C# DLL library.<br>(自作アプリに組み込むためのC# DLLリファレンス) |
| **JigLibSample App Manual** | [English](./docs/en/03_Sample_Program_User_Guide/README.md) | [日本語](./docs/ja/03_サンプルプログラム利用ガイド/README.md) | Tutorial for the console-based sample application.<br>(ライブラリを活用したコンソールアプリのチュートリアル) |

---

<a id="english"></a>
## English

### Overview
**PicoJig** and **PicoJig-WL** are combined Firmware (FW), PC Applications, and C# Libraries that allow you to easily control the peripherals of a **Raspberry Pi Pico** or **Raspberry Pi Pico W** from a Windows PC.

#### Main Features
* **Two Connection Modes:**
    * **USB Mode:** Control via Virtual COM port (Supports both Pico and Pico W).
    * **Wi-Fi Mode:** Control remotely via TCP/IP socket communication (Pico W only).
* **Supported Peripherals:**
    * GPIO
    * ADC
    * UART
    * SPI (Master)
    * I2C (Master)
    * PWM

### Repository Components
This repository consists of the following main components:

1.  **Firmware (`.uf2`)** Firmware files for Raspberry Pi Pico and Pico W. 
2.  **PC GUI Application (`PicoJigApp.exe`)** A Windows GUI application to control microcontroller peripherals from a PC via USB or Wi-Fi. *(Requires .NET Framework 4.6.2+)*
3.  **C# Library (`PicoJigLib.dll`)** A C# DLL to control microcontroller peripherals from a PC via USB or Wi-Fi. You can integrate it into your own custom Windows applications.
4.  **Sample Application (`PicoJigLibSample.exe`)** A console-based sample C# program demonstrating how to use `PicoJigLib.dll` to control the microcontroller peripherals.

### Source Code
The source code for all components - the firmware, PC GUI Application, C# Library, and Sample Application - is completely open and publicly available in this repository.

### Quick Start
1.  Write the appropriate firmware (`.uf2`) to your Pico or Pico W:
    * **Pico:** `PicoJig_XXXXXXXX.uf2`
    * **Pico W:** `PicoJig_WL_XXXXXXXX.uf2`
2.  Launch the PC GUI Application (`PicoJigApp.exe`) or the Sample Application (`PicoJigLibSample.exe`) to control the peripherals.

*For details on pin usage and further instructions, please refer to the Documentation.*

---

<a id="japanese"></a>
## 日本語

### 概要
**PicoJig** および **PicoJig-WL** は、Windows PC から **Raspberry Pi Pico** または **Raspberry Pi Pico W** のペリフェラルを簡単に制御できるようにする、ファームウェア (FW)、PC アプリケーション、および C# ライブラリのセットです。

#### 主な機能
* **2つの接続モード:**
    * **USB モード:** 仮想 COM ポート経由での制御 (Pico と Pico W の両方に対応)。
    * **Wi-Fi モード:** TCP/IP ソケット通信経由でのリモート制御 (Pico W のみ)。
* **対応ペリフェラル:**
    * GPIO
    * ADC
    * UART
    * SPI (マスター)
    * I2C (マスター)
    * PWM

### リポジトリの構成要素
本リポジトリは、主に以下の要素で構成されています。

1.  **ファームウェア (`.uf2`)** Raspberry Pi Pico用 および Pico W用のファームウェアファイルです。
2.  **PC GUI アプリケーション (`PicoJigApp.exe`)** PCからUSBやWi-Fi経由でマイコンのペリフェラルを制御するためのWindows GUIアプリです。*(.NET Framework 4.6.2 以降が必要)*
3.  **C# ライブラリ (`PicoJigLib.dll`)** PCからUSBやWi-Fi経由でマイコンのペリフェラルを制御するためのC#のDLLです。独自の Windows アプリケーションに組み込むことができます。
4.  **サンプルアプリケーション (`PicoJigLibSample.exe`)** `PicoJigLib.dll` を使用してマイコンのペリフェラルを制御する方法を示す、コンソールベースの C# サンプルプログラムです。

### ソースコードについて
ファームウェア、PC GUI アプリケーション、C# ライブラリ、サンプルアプリケーションのソースコードはすべて公開されています。

### クイックスタート
1.  適切なファームウェア（`.uf2`）を Pico または Pico W に書き込みます。
    * **Pico:** `PicoJig_XXXXXXXX.uf2`
    * **Pico W:** `PicoJig_WL_XXXXXXXX.uf2`
2.  PC GUI アプリケーション (`PicoJigApp.exe`) またはサンプルアプリケーション (`PicoJigLibSample.exe`) を起動し、ペリフェラルを制御します。

*使用するピンや詳細な使い方については、ドキュメントをご参照ください。*
