*It was created using pico-sdk and C language without using MicroPython or Arduino IDE.  
  
Contains PicoJig-WL and PicoJig.   
Please refer to the manual for usage.   
The manual is available in English and Japanese.  　　     
    
**PicoJig-WL**    
The microcontroller board uses Raspberry Pi Pico W.   
This is firmware and a PC application that controls the GPIO/UART/SPI/I2C/ADC/PWM of Pico W from a PC via Wi-Fi (TCP socket communication) or USB (virtual COM).   

[System Configuration]   
https://sites.google.com/view/shiomachisoft/english-home/picojig

[Screen Capture]   
https://sites.google.com/view/shiomachisoft/english-home/picojig/picojig-screen 

[Features] 
- By sending commands from the PC app to Pi Pico W via Wi-Fi or USB, you can have Pi Pico W send any UART/SPI/I2C/GPIO/PWM data.       
- The UART/SPI/I2C/GPIO/ADC data received by Pi Pico W is passed to the PC app via W-Fi or USB and displayed on the PC app.     
     
**PicoJig**        
The microcontroller board uses a Raspberry Pi Pico.   
Compared to the PicoJig-WL, it does not have Wi-Fi functionality.  
