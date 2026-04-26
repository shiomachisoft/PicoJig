// Copyright © 2024 Shiomachi Software. All rights reserved.
#include "Common.h"

// Initialize ADC / ADCを初期化
void ADC_Init()
{
    adc_init();
    adc_gpio_init(ADC0);
    adc_gpio_init(ADC1);
    adc_gpio_init(ADC2);
    adc_set_temp_sensor_enabled(true);
}