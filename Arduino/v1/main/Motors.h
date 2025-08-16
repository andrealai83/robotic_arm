#pragma once
#include <AccelStepper.h>
#include "Config.h"
#include <MultiStepper.h>

// Istanze motori e flag di completamento
extern AccelStepper motore1;
extern AccelStepper motore2;
extern AccelStepper motore3;
extern AccelStepper motore4;

extern bool motore1Completato;
extern bool motore2Completato;
extern bool motore3Completato;
extern bool motore4Completato;

extern MultiStepper squad;

// API
void setupMotors();
void setupMotor(AccelStepper& motore, int pinENA);
void setTarget(AccelStepper& motore, int targetGradi);
void checkMotor(AccelStepper& motore, bool& completato, int endstopPin);
void homingMotor(AccelStepper& motore, int endstopPin, int velocitaNegativa);
void handleMotors();
void moveAllToDegrees(int g1,int g2,int g3,int g4);
void setEnableAll(bool on);