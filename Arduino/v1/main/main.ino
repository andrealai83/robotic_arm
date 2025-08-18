#include "Config.h"
#include "Motors.h"
#include "Display.h"
#include "Encoder.h"
#include "Commands.h"

void setup() {
  Serial.begin(115200);

  setupPins();
  setupMotors();
  setupDisplay();
  encoderSetup();

  recalcPassiPerGrado();
  aggiornaDisplay();
  Serial.println(F("Sistema pronto."));
}

void loop() {
  // comandi da seriale
  handleSerial(); 
  // gestione pulsanti fisici
  handleButtons();

  handleMotors();

  encoderUpdate();
 
  // completamento movimento coordinato
  // if (eseguiMovimento && motore1Completato && motore2Completato && motore3Completato && motore4Completato) {
  //   // Serial.println(F("ready"));
  //   // String MagnetState = calamitaAttiva ? ";C:1" : ";C:0";
  //   // String SaveRequestReceived = saveRequest ? ";SAVE:1" : ";SAVE:0";
  //   // String posizione = "NEWPOSITION:X:" + String(target1) + ";Y:" + String(target2) + ";Z:" + String(target3) + ";A:" + String(target4) + MagnetState + SaveRequestReceived;
  //   // saveRequest = 0;
  //   // Serial.println(posizione);
  //   eseguiMovimento = false;
  // }

  // encoder (singolo su A12 di default)
  //encoderUpdate();

  // opzionale: LCD aggiornato a eventi, non sempre per evitare flicker
  // aggiornaDisplay();
}
