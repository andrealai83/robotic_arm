#pragma once
#include <Arduino.h> 
#include "RobotLink.h"

void handleSerial();
void handleButtons();
int  parseTarget(const String& comando);
void setConfiguration(const String& config);

void parseCommandLine(String s);
void processToken(const String& cmd);

void applyCommand(const Payload& p); // chiamata dal radioLink
void onBtn6();                       // BTN6 → HOMING (o preset)
void onBtn7();                       // BTN7 → STOP software

void joystickVelocityUpdate();
void fillAck(AckPayload& out);
