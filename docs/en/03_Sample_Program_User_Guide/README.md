# JigLibSample Application Manual

## Table of Contents
- [1. Overview](#1-overview)
- [2. Pin Usage](#2-pin-usage)
- [3. Startup and Connection](#3-startup-and-connection)
- [4. Main Menu](#4-main-menu)
- [5. Notes on Each Function](#5-notes-on-each-function)

## 1. Overview
This application is a console-based sample program that uses `PicoJigLib.dll` to communicate with a microcontroller (Raspberry Pi Pico / Raspberry Pi Pico W) via USB (COM) or Wi-Fi (TCP/IP) to control and retrieve information from peripherals (GPIO, ADC, UART, I2C, SPI, PWM).

## 2. Pin Usage
The microcontroller pin assignments used for each function are as follows. (* Physical pin numbers are in parentheses)

- **GPIO (Input)**: 
  - GP3 (Pin 5), GP4 (Pin 6), GP5 (Pin 7), GP8 (Pin 11), GP9 (Pin 12), GP10 (Pin 14), GP11 (Pin 15)
- **GPIO (Output)**: 
  - GP12 (Pin 16), GP13 (Pin 17), GP14 (Pin 19), GP15 (Pin 20), GP20 (Pin 26), GP21 (Pin 27), GP22 (Pin 29)
- **ADC**:
  - ADC0: GP26 (Pin 31)
  - ADC1: GP27 (Pin 32)
  - ADC2: GP28 (Pin 34)
  - ADC4: Temperature Sensor
- **UART (UART0)**:
  - TX: GP0 (Pin 1) 
  - RX: GP1 (Pin 2)
- **SPI (SPI0)**:
  - RX: GP16 (Pin 21) 
  - CSn: GP17 (Pin 22) 
  - SCK: GP18 (Pin 24) 
  - TX: GP19 (Pin 25)
- **I2C (I2C1)**:
  - SDA: GP6 (Pin 9) 
  - SCL: GP7 (Pin 10)
- **PWM**: 
  - GP2 (Pin 4)

## 3. Startup and Connection
When the application starts, an English prompt will be displayed on the console screen. Follow the steps below to connect to the microcontroller.

### Step 1. Select Communication Method

First, the following message will be displayed.

```text
@Please select a communication method.
  1: USB   (COM)
  2: Wi-Fi (TCP/IP)
Select Method > 
```
Enter "1" or "2" on the keyboard and press the `Enter` key.

### Step 2. Enter Connection Parameters

- **If you select "1" (USB)**:  
  A list of available COM ports will be displayed. Enter the destination port name (e.g., `COM3`) and press the `Enter` key.
  ```text
  Available COM ports:
    COM1
    COM3
  Please enter the COM port name (e.g., COM1): 
  ```

- **If you select "2" (Wi-Fi)**:  
  Enter the microcontroller's IP address and press the `Enter` key.
  ```text
  Please enter the IP address (e.g., 192.168.10.100): 
  ```

The connection attempt will start, and if successful, the following message will be displayed, and it will transition to the main menu screen.
```text
Connecting to COM3 via USB (COM)...
Connected successfully via USB (COM).
```

## 4. Main Menu

The following prompt will be displayed at the bottom of the screen.
```text
@Select Menu
 Enter: Show menu, Number: Execute > 
```
Enter the number of the function you want to execute and press the `Enter` key. (If you press the `Enter` key without entering anything, the menu list will be displayed again.)
Below is an overview of the user input required when executing each function and the messages displayed.

### Function List
- **1: Get FW Info**  
  Gets the FW information.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- FW Info ---
    Maker: SHIOMACHI_SOFT
    FW Name: PicoJig_WL
    FW Version: 26040100
    Board ID: E66428C51F34B027
    ```

- **2: Get Wi-Fi Config**  
  Gets the current Wi-Fi network configuration.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- Network configuration ---
      Device IP Address:  192.168.10.100
      SSID:               My_SSID
      Password:           (hidden)
    ```

- **3: Set Wi-Fi Config**  
  Sets the IP address, SSID, and password, then resets the microcontroller.
  
  - **Conditions for the Wi-Fi router SSID that can be specified**
    - It must support the Wi-Fi standard "IEEE 802.11b/g/n" using the 2.4GHz band. Please be careful not to accidentally specify a 5GHz frequency band SSID.
    - The encryption method must be WPA2.   
  
  - **Input:**   
    - Enter values for the following prompts. (e.g., IP Address: `192.168.10.100`, SSID: `My_SSID`, SSID Password: `password`)
    ```text
    --- Set Wi-Fi Config ---
    IP Address: 
    SSID: 
    SSID Password: 
    ```
  - **Display:**   
    After completion, automatic reconnection processing is performed.  
    > **Note:**  
    > If you are currently connected to the microcontroller via Wi-Fi (TCP/IP) communication, it is designed to automatically attempt reconnection using the **new IP address** changed here.
    ```text
    Wi-Fi configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **4: Get ADC Value**  
  Gets the ADC voltage values and temperature sensor value.
  - **Input:**   
    None
  - **Display:**   
    The voltage [V] and temperature [deg C] of each channel are displayed.  
    ```text
    --- ADC Values ---
    CH0: 1.234 V
    CH1: 0.000 V
    CH2: 3.210 V
    CH4 (Temp): 25.50 deg C
    ```

#### [GPIO]
- **5: Get GPIO Config**  
  Gets the current GPIO configuration.
  - **Input:**   
    None
  - **Display:** 
    - Built-in pull-up/pull-down settings for input GPIO
    - Initial output values at power-on for output GPIO  
    ```text
    --- GPIO configuration ---
    [Input GPs] Pull-up/Pull-down
    GP3: Pull-down
    GP4: Pull-down
    GP5: Pull-up
    GP8: Pull-up
    GP9: Pull-up
    GP10: Pull-up
    GP11: Pull-up
    [Output GPs] Initial output value
    GP12: High
    GP13: High
    GP14: Low
    GP15: Low
    GP20: Low
    GP21: Low
    GP22: Low
    ```

- **6: Set GPIO Config**  
  Changes the GPIO configuration and resets the microcontroller.
  - **Input:**   
    - Built-in pull-up/pull-down settings for input GPIO
    - Initial output values at power-on for output GPIO    
    - Following the prompts, enter `0` or `1` for each GP pin.
    ```text
    --- Set GPIO Config ---
    [Input GPs] Pull-up/Pull-down
    GP3 (0:Pull-up, 1:Pull-down): 
    ...
    [Output GPs] Initial output value
    GP12 (0:Low, 1:High): 
    ...
    ```
  - **Display:**   
    ```text
    GPIO config completed.
    MCU will be reset, waiting for 5 seconds...
    ```

- **7: Get GPIO I/O Value**  
  Gets the current status of the input GPIO and output GPIO.
  - **Input:**   
    None
  - **Display:**   
    The input/output status is displayed.  
    ```text
    --- Get GPIO I/O Values ---
    [Input GPs] Values
    GP3: High
    GP4: High
    GP5: Low
    GP8: Low
    GP9: Low
    GP10: Low
    GP11: Low
    [Output GPs] Values
    GP12: Low
    GP13: Low
    GP14: Low
    GP15: Low
    GP20: Low
    GP21: Low
    GP22: Low
    ```

- **8: Set GPIO Output**  
  Outputs the specified data to the output GPIO.
  - **Input:**   
    - Following the prompts, enter `0` or `1` for each output GP pin.
    ```text
    --- Set GPIO Output ---
    [Output GPs] Status
      GP12 (0:Low, 1:High): 
    ...
    ```
  - **Display:**   
    ```text
      GP12: High
      GP13: High
      GP14: Low
      GP15: Low
      GP20: Low
      GP21: Low
      GP22: Low
    ```

#### [UART]
- **9: Get UART Config**  
  Gets the current UART communication conditions.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- UART configuration ---
      Baudrate: 115200 bps
      DataBits: 8
      StopBits: 1
      Parity:   0 (0:None, 1:Even, 2:Odd)
    ```

- **10: Set UART Config**  
  Changes the UART configuration and resets the microcontroller.
  - **Input:**   
    - Enter numerical values for each (e.g., Baudrate: `115200`, Stop bits: `1`, Parity: `0`).
    ```text
    --- Set UART Config ---
    Baudrate: 
    Stop bits: 
    Parity (0:None, 1:Even, 2:Odd): 
    ```
  - **Display:**   
    ```text
    UART configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **11: UART Send**  
  Sends data via UART.
  - **Input:**   
    - Enter hexadecimal data separated by spaces or commas (e.g., `01 02 0A`).
    ```text
    --- UART Send ---
    Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 0A): 
    ```
  - **Display:**   
    ```text
    UART sent: 3 bytes
    ```

- **12: UART Receive**  
  Displays the UART received data in the queue.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- UART Receive ---
    UART received (Hex): 01 02 0A
    ```
    If there is no data, it will be displayed as follows:  
    ```text
    --- UART Receive ---
    No UART received data.
    ```
  > **Note:**  
  > UART received data is asynchronously stored in an internal queue (up to 4096 bytes).  
  > The data at the time "12" is selected from the menu is displayed on the screen.

#### [I2C]
- **13: Get I2C Config**  
  Gets the current I2C communication frequency.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- I2C configuration ---
      Frequency: 400000 Hz
    ```

- **14: Set I2C Config**  
  Sets the I2C communication frequency (Hz) and resets the microcontroller.
  - **Input:**   
    - Enter a numerical value (e.g., `400000`).
    ```text
    --- Set I2C Config ---
    Frequency(Hz): 
    ```
  - **Display:**   
    ```text
    I2C configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **15: I2C Send**  
  Sends data via I2C.
  - **Input:**   
    - Enter the slave address and the hexadecimal data to send (e.g., Address: `17`, Data: `01 02 03 04`).
    ```text
    --- I2C Send ---
    7bit Slave Address (Hex): 0x
    Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 03 04): 
    ```
  - **Display:**   
    ```text
    I2C sent.
    ```

- **16: I2C Receive**  
  Receives data via I2C.
  - **Input:**   
    - Enter the slave address and the number of bytes to receive (e.g., Address: `17`, Read Length: `4`).
    ```text
    --- I2C Receive ---
    7bit Slave Address (Hex): 0x
    Read Length: 
    ```
  - **Display:**   
    ```text
    I2C received (Hex): 01 02 03 04
    ```

#### [SPI]
- **17: Get SPI Config**  
  Gets the current SPI communication conditions.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- SPI configuration ---
      Frequency: 1000000 Hz
      DataBits:  8
      CPOL:      0
      CPHA:      0
      Order:     1 (1:MSB First)
    ```

- **18: Set SPI Config**  
  Sets the communication frequency, polarity, and phase, then resets the microcontroller.
  - **Input:**   
    - Enter numerical values for each (e.g., Frequency: `1000000`, Polarity: `0`, Phase: `0`).
    ```text
    --- Set SPI Config ---
    Frequency(Hz): 
    Polarity (0:CPOL=0, 1:CPOL=1): 
    Phase (0:CPHA=0, 1:CPHA=1): 
    ```
  - **Display:**   
    ```text
    SPI configured.
    MCU will be reset, waiting for 5 seconds...
    ```

- **19: SPI Comm (Send/Receive)**  
  Sends and receives data via SPI.
  - **Input:**   
    - Enter hexadecimal data to send separated by spaces or commas (e.g., `01 02 03 04`).
    ```text
    --- SPI Comm ---
    Send Data (Max 256 bytes, Hex, space/comma separated, e.g., 01 02 03 04): 
    ```
  - **Display:**   
    ```text
    SPI received data (Hex): 01 02 03 04
    ```

#### [PWM]
- **20: Start PWM**  
  Starts PWM signal output.
  - **Input:**   
    - Enter numerical values for each prompt.
      - **Formula:**  
        - `PWM Frequency = 125MHz / ((Wrap + 1) * Clock divider)`
        - `Duty Cycle = Compare value(Level) / (Wrap + 1)`
      - (e.g., To set PWM frequency to 100Hz and duty cycle to 50% -> Enter Clock divider: `250`, Wrap: `4999`, Compare value: `2500`)
    ```text
    --- Start PWM ---
    Clock divider: 
    Wrap: 
    Compare value(Level): 
    ```
  - **Display:**   
    ```text
    PWM started.
    ```

- **21: Stop PWM**  
  Stops PWM signal output.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- Stop PWM ---
    PWM stopped.
    ```

#### [Others]
- **22: Get FW Error**  
  Gets the FW error information.
  - **Input:**   
    None
  - **Display:**   
    If there are no errors, it will be displayed as follows:  
    ```text
    --- Get FW Error ---
    No FW errors.
    ```
    If there are errors, a list is displayed:  
    ```text
    --- FW Errors ---
     - (Example Error Message 1)
     - (Example Error Message 2)
    ```

- **23: Clear FW Error**  
  Clears the FW error information.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- Clear FW Error ---
    FW errors cleared.
    ```

- **24: Erase Flash**  
  Erases the flash memory of the microcontroller.
  - **Input:**   
    None
  - **Display:**   
    ```text
    --- Erase Flash ---
    Flash erased successfully.
    MCU will be reset, waiting for 5 seconds...
    ```

- **0: Disconnect**  
  Disconnects the communication and exits the application.
  - **Input:**   
    None
  - **Display:**   
    ```text
    Disconnected.
    ```

## 5. Notes on Each Function
- **Regarding configuration commands and Flash Erase (3, 6, 10, 14, 18, 24)**  
  The microcontroller is automatically reset when settings are saved or flash memory is erased.  
  Automatic reconnection is performed after a waiting time of about 5 seconds.

- **Regarding Hexadecimal (Hex) Input**  
  When entering communication data, "0x" is not required. Enter it following the "0x" on the screen, or enter the data separated by spaces or commas.  
  e.g.: `01 02 0A` (for data transmission)

- **Regarding Fixed Parameters**  
  Some communication settings (such as UART data bits, SPI data bits, and bit order) are defined as fixed values in the source code based on the library specifications.