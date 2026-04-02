#include "Config.h"
#include "Motors.h"
#include "Display.h"
#include "Encoder.h"
#include "Commands.h"
#include "RobotLink.h"
#include "PressureSensors.h"

// Istanza del link radio (CE=9, CSN=10)
RobotLink radioLink(48, 49);

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
  setupGripper();
  setupDisplay();
  encoderSetup();
  pressureSensorsSetup();

  recalcPassiPerGrado();
  aggiornaDisplay();

  //radioLink.setHandlers(applyCommand, onBtn6, onBtn7);
  //radioLink.begin();

  //radioLink.setStatusProvider(fillAck);
  
  Serial.println(F("Sistema pronto."));
}

void loop() {
  
  //radioLink.poll();    
        
  handleSerial();

  handleButtons();

  //joystickVelocityUpdate();   

  handleMotors();    

  //encoderUpdate();

  endstopTelemetryUpdate();
  pressureSensorsUpdate();
  
   
}
 
