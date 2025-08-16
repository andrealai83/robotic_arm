#include "Config.h"

// PIN motori
int ENA1 = 13, DIR1 = 12, PUL1 = 11;
int ENA2 = 10, DIR2 = 9,  PUL2 = 8;
int ENA3 = 7,  DIR3 = 6,  PUL3 = 5;
int ENA4 = 4,  DIR4 = 3,  PUL4 = 2;

// Stato globale
bool saveRequest = false;
bool eseguiMovimento = false;
bool calamitaAttiva = false;
int target1 = 0, target2 = 0, target3 = 0, target4 = 0;
int ENDSTOP_ENABLED = 1;

// Parametri motori
int passiPerGiro = 200;
int microstep = 4;
double rapportoPlanetario = 14.2;
double passiPerGrado = 0.0;
int maxSpeed = 7000;
int maxAccel = 7000;

bool motorsEnabled = true;          
const bool EN_ACTIVE_HIGH = true;  
  
bool encoderStreamOn = false;
uint16_t encoderStreamPeriodMs = 50; 


void setupPins() {
  pinMode(TRANSISTOR_PIN, OUTPUT);
  pinMode(ENDSTOP_1_PIN, INPUT_PULLUP);
  pinMode(ENDSTOP_2_PIN, INPUT_PULLUP);
  pinMode(ENDSTOP_3_PIN, INPUT_PULLUP);
  pinMode(ENDSTOP_4_PIN, INPUT_PULLUP);
  pinMode(BTN_HOMING_PIN, INPUT_PULLUP);
  pinMode(BTN_STOP_PIN,   INPUT_PULLUP);
}

void recalcPassiPerGrado() {
  passiPerGrado = (double)(passiPerGiro * microstep) / 360.0 * rapportoPlanetario;
}
