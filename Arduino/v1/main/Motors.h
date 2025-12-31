#pragma once
#include <AccelStepper.h>
#include "Config.h"


// Istanze motori e flag di completamento
extern AccelStepper motore1;
extern AccelStepper motore2;
extern AccelStepper motore3;
extern AccelStepper motore4;

extern bool motore1Completato;
extern bool motore2Completato;
extern bool motore3Completato;
extern bool motore4Completato;

// Configurazione direzioni
struct MotorConfig {
    bool invertRotation;      // true = inverti il senso di marcia (AGISCE SUI PIN)
    int  homingDirectionSign; // 1 = Direzione POSITIVA, -1 = Direzione NEGATIVA (Indipendente da invertRotation)
};

extern MotorConfig motorConfigs[4];

// API
void setupMotors();
void setupMotor(AccelStepper& motore, int pinENA, int motorIndex); // Aggiunto indice
void setTarget(AccelStepper& motore, int targetGradi);
void checkMotor(AccelStepper& motore, bool& completato, int endstopPin);
void homingMotor(AccelStepper& motore, int endstopPin, int motorIndex); // Aggiunto indice per recuperare config
void handleMotors();
void moveAllToDegrees(int g1,int g2,int g3,int g4);

// Nuova gestione coordinata scalata
void startCoordinatedMove();   
void setEnableAll(bool on);