[English](#english) | [日本語](#japanese)

---

<a id="english"></a>
# `PicoJigLibSample.py` - Setup Guide

## 1. Install Python on Windows

Download the `Windows installer (64-bit)` from the official Python [download page](https://www.python.org/downloads/windows/) and install it.
* We have confirmed operation with **Python 3.14.5**.

[Important] On the first installation screen, be sure to check the "Add python.exe to PATH" (or "Add Python to environment variables") checkbox before clicking "Install Now".

## 2. Install Python Packages
Open PowerShell and run the following commands to install the two packages below.
- `pythonnet` : To call and use the C# DLL (`PicoJigLib.dll`) from Python.
- `pyserial` : To retrieve a list of COM ports available on the PC when connected via USB.

```bash
pip install pythonnet
pip install pyserial
```

## 3. Placement of `PicoJigLibSample.py` and PicoJigLib.dll

Place `PicoJigLibSample.py` and `PicoJigLib.dll` in the **same folder** on your PC.[cite: 2]

## 4. Launch `PicoJigLibSample.py`

Open PowerShell and navigate to the folder where `PicoJigLibSample.py` is located (using the `cd` command).
Then, run the following command to launch `PicoJigLibSample.py`.

```bash
python PicoJigLibSample.py
```

---

<a id="japanese"></a>
# `PicoJigLibSample.py` - セットアップガイド

## 1. WindowsにPythonをインストール

Python公式サイトの[ダウンロードページ](https://www.python.org/downloads/windows/)から `Windows installer (64-bit)`をダウンロードし、インストールを行ってください。
※ バージョンは **Python 3.14.5** にて動作確認を行っています。

【重要】 インストール時の最初の画面で、「Add python.exe to PATH」（または「Add Python to environment variables」）のチェックボックスに必ずチェックを入れてから「Install Now」をクリックしてください。

## 2. Pythonパッケージのインストール
PowerShellでコマンドを実行して下記の2つのパッケージをインストールしてください。
- `pythonnet` : PythonからC#のDLL (`PicoJigLib.dll`) を呼び出して使用するため
- `pyserial` : USB接続時にPCで利用可能なCOMポートの一覧を取得するため

```bash
pip install pythonnet
pip install pyserial
```
## 3. `PicoJigLibSample.py`とPicoJigLib.dllの配置

`PicoJigLibSample.py` と `PicoJigLib.dll` をPC内の**同じフォルダ**に配置してください。

## 4.  `PicoJigLibSample.py`の起動

PowerShellを開き、`PicoJigLibSample.py` を配置したフォルダに移動（`cd` コマンドを使用）します。
その後、以下のコマンドを実行して`PicoJigLibSample.py`を起動します。

```bash
python PicoJigLibSample.py
```