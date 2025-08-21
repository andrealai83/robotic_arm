#include "Config.h"
#include "Motors.h"
#include "Display.h"
#include "Encoder.h"
#include "Commands.h"
#include "RobotLink.h" 

// Istanza del link radio (CE=9, CSN=10)
RobotLink radioLink(9, 10);
 
void setup() {
  Serial.begin(115200);

  setupPins();
  setupMotors();
  setupDisplay();
  encoderSetup();

  recalcPassiPerGrado();
  aggiornaDisplay();

  radioLink.setHandlers(applyCommand, onBtn6, onBtn7);
  radioLink.begin();
  
  Serial.println(F("Sistema pronto."));
}

void loop() {
 
  radioLink.poll();
  
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
 
