if (TARGET tinyusb_device)

	add_compile_options(-Wall
	        -Wno-format          # int != int32_t as far as the compiler is concerned because gcc has int32_t as long int
	        -Wno-unused-function # we have some for the docs that aren't called
	        -Wno-maybe-uninitialized
	        )

    add_executable(PicoJig
            main.c
            Cmd.c
            Flash.c
            Common.c
            Gpio.c
            Adc.c
            Uart.c
            Spi.c
            I2c.c
            Pwm.c
            Timer.c
            TcpServer.c
            Frame.c
            )

	target_include_directories(PicoJig PRIVATE
        ${CMAKE_CURRENT_LIST_DIR}
        ${CMAKE_CURRENT_LIST_DIR}/.. # for our common lwipopts
        )

    # pull in common dependencies
    target_link_libraries(PicoJig 
    		pico_cyw43_arch_lwip_poll
			pico_stdlib 
			hardware_adc 
			hardware_spi 
			hardware_dma 
			hardware_i2c 
			pico_i2c_slave 
			hardware_pwm 
			pico_multicore
			hardware_sync
           	pico_unique_id
          	hardware_clocks
            hardware_flash
            hardware_exception
			)

    # enable usb output, disable uart output
    pico_enable_stdio_usb(PicoJig 1)
    pico_enable_stdio_uart(PicoJig 0)

    # create map/bin/hex/uf2 file etc.
    pico_add_extra_outputs(PicoJig)

elseif(PICO_ON_DEVICE)
    message(WARNING "not building PicoJig because TinyUSB submodule is not initialized in the SDK")
endif()
