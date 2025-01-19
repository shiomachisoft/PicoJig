## 1.PicoJigの概要  
マイコン基板はRaspberry Pi Pico Wを使用します。   
PCからWi-Fi(TCPソケット通信)またはUSB(仮想COM)経由でPi Pico WのGPIO/UART/SPI/I2C/ADC/PWMを制御するファームウェアとPCアプリです。 
Raspberry Pi Picoを使用する場合はWi-Fi機能がありません。  

## 2.特徴
- (1) PCアプリからPico WにWi-FiまたはUSB経由でコマンドを送信することで、Pi Pico Wに任意のUART/SPI/I2C/GPIO/PWMデータを送信させることができます。  
- (2) Pi Pico Wが受信したUART/SPI/I2C/GPIO/ADCデータをW-FiまたはUSB経由でPCアプリに渡し、PCアプリで表示します。  
- (3) Pico Wに対するWi-Fi設定等は専用PCアプリを使用し、Pico WのFlashメモリに保存します。

## 3.システム構成    
  
![image](https://github.com/user-attachments/assets/e0c38d6b-b5d4-4417-8f48-5e3670726e59)  
  
## 4.使い方
マニュアルを参照。

## 5.ソースコード  
PCアプリもFWもソースコードを公開しています。  
