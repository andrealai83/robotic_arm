#pragma once

#include <Arduino.h>

void pressureSensorsSetup();
void pressureSensorsUpdate();
uint8_t getPressurePercent(uint8_t sensorIndex);
bool isPressureDetected(uint8_t sensorIndex);
uint8_t getGripPressurePercent();
void printPressureStatus();
