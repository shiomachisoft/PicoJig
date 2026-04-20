// Copyright © 2024 Shiomachi Software. All rights reserved.
#ifndef ADC_H
#define ADC_H

#include "Common.h"

// [define] / [定義]
#define ADC_CH_NUM_WITHOUT_TEMP 3 // Number of ADC channels (excluding temperature sensor) / ADCのチャンネル数(温度センサ含まない)
#define ADC_CH_NUM              4 // Number of ADC channels (including temperature sensor) / ADCのチャンネル数(温度センサ含む)

// [Function prototype declarations] / [関数プロトタイプ宣言]
void ADC_Init();

#endif