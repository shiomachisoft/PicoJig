#include "Common.h"

// ADCを初期化
void ADC_Init()
{
    adc_init();
    adc_gpio_init(GP_26);
    adc_gpio_init(GP_27);
    adc_gpio_init(GP_28);
    adc_set_temp_sensor_enabled(true);
}