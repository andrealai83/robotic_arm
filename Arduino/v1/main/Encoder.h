#pragma once
#include <Arduino.h>

// Pin e calibrazione (modifica se serve)
#define ENCODER_PIN A12
const float ALPHA = 0.2f;
const float MIN_VAL = 0.0f;    // misura a 0° fisici
const float MAX_VAL = 356.0f;  // misura a 360° fisici

extern float angleDeg; // ultimo angolo calcolato 0..360
extern float jointDeg;

void encoderZero();
void encoderSetup();
void encoderUpdate();
