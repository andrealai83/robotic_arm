#pragma once
#include <Arduino.h> 

void handleSerial();
void handleButtons();
int  parseTarget(const String& comando);
void setConfiguration(const String& config);

void parseCommandLine(String s);
void processToken(const String& cmd);
