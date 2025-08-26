#include "Config.h"
#include "Motors.h"
#include "Display.h"
#include "Encoder.h"
#include "Commands.h"
#include "RobotLink.h" 

// Istanza del link radio (CE=9, CSN=10)
RobotLink radioLink(14, 15);

extern char __bss_end; 
extern char *__brkval;

// globals
unsigned long cPoll=0,cVel=0,cRun=0;
unsigned long tPoll=0,tVel=0,tRun=0;

// wrapper
unsigned long ts;
inline void mark(){ ts = micros(); }
inline void acc(unsigned long &t){ t += (micros() - ts); }
 
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

  radioLink.setStatusProvider(fillAck);
  
  Serial.println(F("Sistema pronto."));
}

void loop() {
  
  radioLink.poll();    
        
  handleSerial();

  handleButtons();

  joystickVelocityUpdate();   

  handleMotors();    

  encoderUpdate();
 
  // completamento movimento coordinato
//  if (eseguiMovimento && motore1Completato && motore2Completato && motore3Completato && motore4Completato) {
//      Serial.println(F("ready"));
//      String MagnetState = calamitaAttiva ? ";C:1" : ";C:0";
//      String SaveRequestReceived = saveRequest ? ";SAVE:1" : ";SAVE:0";
//      String posizione = "NEWPOSITION:M1:" + String(target1) + ";M2:" + String(target2) + ";M3:" + String(target3) + ";M4:" + String(target4) + MagnetState + SaveRequestReceived;
//      saveRequest = 0;
//      Serial.println(posizione);
//     eseguiMovimento = false;
//  }


   
}
 
