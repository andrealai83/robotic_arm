#include "Config.h"

// PIN motori
int ENA4 = 13, DIR4 = 12, PUL4 = 11;
int ENA1 = 10, DIR1 = 9,  PUL1 = 8;
int ENA3 = 7,  DIR3 = 6,  PUL3 = 5;
int ENA2 = 4,  DIR2 = 3,  PUL2 = 2;
int ENAP = 19, DIRP = 21, PULP = 20;

// Stato globale
bool saveRequest = false;
bool eseguiMovimento = false;
bool calamitaAttiva = false;
int target1 = 0, target2 = 0, target3 = 0, target4 = 0, targetP = 0;
int ENDSTOP_ENABLED = 1;

// Parametri motori
int passiPerGiro = 200;
int microstep = 4;
double rapportoPlanetario = 14.2;

float moltiplicaRapportoPlanetario = 28.0f;
double passiPerGrado = 0.0;
int maxSpeed = 5000;
int maxAccel = 2000;

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
