#ifndef ADC_H
#define ADC_H

#include "Common.h"

// [define]
#define ADC_CH_NUM_WITHOUT_TEMP 3 // ADCのチャンネル数(温度センサ含まない)
#define ADC_CH_NUM              4 // ADCのチャンネル数(温度センサ含む)

// [関数プロトタイプ宣言]
void ADC_Init();

#endif