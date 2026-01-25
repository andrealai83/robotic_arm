#pragma once
#include <Arduino.h>

void setupDisplay();
void aggiornaDisplay();
void mostraMessaggio(const String& messaggio);
bool bluetoothConnesso();
void mostraStatoEndstop(bool e1, bool e2, bool e3, bool e4);
