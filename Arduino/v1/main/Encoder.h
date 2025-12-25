#pragma once
#include <Arduino.h>

// Pin encoder per i cinque motori (A10 - A14)
#define ENCODER_PIN_MOTOR1 A14  // MOTORE 1
#define ENCODER_PIN_MOTOR2 A10  // MOTORE 2
#define ENCODER_PIN_MOTOR3 A13  // MOTORE 3
#define ENCODER_PIN_MOTOR4 A12  // MOTORE 4
#define ENCODER_PIN_MOTOR5 A11  // MOTORE 5

// Parametri di calibrazione
const float ALPHA = 0.2f;
const float MIN_VAL = 0.0f;    // misura a 0° fisici
const float MAX_VAL = 356.0f;  // misura a 360° fisici

// Variabili globali per ogni encoder
extern float angleDeg1, angleDeg2, angleDeg3, angleDeg4, angleDeg5;  // angoli istantanei motore 0..360
extern float jointDeg1, jointDeg2, jointDeg3, jointDeg4, jointDeg5;  // angoli giunto

// Funzioni di gestione encoder
void encoderSetup();
void encoderUpdate();

// Funzioni per encoder singoli
void encoderZero(int motorNum);           // imposta zero del giunto (motorNum = 1-5)
void encoderReset(int motorNum);          // resetta encoder a posizione iniziale (dopo homing)
void encoderStartCal(int motorNum);       // avvia calibrazione
void encoderLockCal(int motorNum);        // blocca calibrazione
void encoderSetDirSign(int motorNum, int sign);  // imposta direzione (+1 o -1)
