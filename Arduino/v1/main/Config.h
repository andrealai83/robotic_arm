#pragma once
#include <Arduino.h>
 

// === PIN motori (per-asse) ===
extern int ENA1, DIR1, PUL1;
extern int ENA2, DIR2, PUL2;
extern int ENA3, DIR3, PUL3;
extern int ENA4, DIR4, PUL4;
extern int ENA5, DIR5, PUL5;
extern int ENA6, DIR6, PUL6;

// Altri pin
#define TRANSISTOR_PIN A11
#define ENDSTOP_1_PIN A2
#define ENDSTOP_2_PIN A3
#define ENDSTOP_3_PIN A4
#define ENDSTOP_4_PIN A6
#define ENDSTOP_P_PIN A5
#define ENDSTOP_5_PIN A8
#define ENDSTOP_6_PIN A9
#define GRIPPER_PIN 15
#define BT_STATE_PIN   A7
#define BTN_HOMING_PIN A0
#define BTN_STOP_PIN   A1
#define PRESSURE_SENSOR_1_PIN A14
#define PRESSURE_SENSOR_2_PIN A15

// Stato globale semplice (usato da più moduli)
extern bool saveRequest;
extern bool eseguiMovimento;
extern bool calamitaAttiva;
extern int  target1, target2, target3, target4, target5, target6;
extern int  ENDSTOP_ENABLED;

// Parametri motori
extern int passiPerGiro;
extern int microstep;
extern double rapportoPlanetario;
extern float moltiplicaRapportoPlanetario;
extern double passiPerGrado;
extern int maxSpeed;
extern int maxAccel;

extern bool multiActive;

extern bool motorsEnabled;           
extern const bool EN_ACTIVE_HIGH;  
 
#ifndef CONFIG_H
    #define CONFIG_H

    #include <Arduino.h> 

    extern bool encoderStreamOn;               
    extern uint16_t encoderStreamPeriodMs;

#endif

// Setup periferiche base
void setupPins();
void recalcPassiPerGrado();
