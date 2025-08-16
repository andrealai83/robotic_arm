#include "Motors.h"
#include "Display.h"
#include "Config.h"

AccelStepper motore1(AccelStepper::DRIVER, PUL1, DIR1);
AccelStepper motore2(AccelStepper::DRIVER, PUL2, DIR2);
AccelStepper motore3(AccelStepper::DRIVER, PUL3, DIR3);
AccelStepper motore4(AccelStepper::DRIVER, PUL4, DIR4);

bool motore1Completato = true;
bool motore2Completato = true;
bool motore3Completato = true;
bool motore4Completato = true;
 
MultiStepper squad;
bool multiActive = false;

void setupMotors(){
  setupMotor(motore1, ENA1);
  setupMotor(motore2, ENA2);
  setupMotor(motore3, ENA3);
  setupMotor(motore4, ENA4);

  squad.addStepper(motore1);
  squad.addStepper(motore2);
  squad.addStepper(motore3);
  squad.addStepper(motore4);
}

void moveAllToDegrees(int g1,int g2,int g3,int g4){
  long pos[4];
  pos[0] = (long)(g1 * passiPerGrado);
  pos[1] = (long)(g2 * passiPerGrado);
  pos[2] = (long)(g3 * passiPerGrado);
  pos[3] = (long)(g4 * passiPerGrado);
  squad.moveTo(pos);
  multiActive = true;
}

void handleMotors() {
  if (multiActive) {
    squad.run();

    if (motore1.distanceToGo()==0 &&
        motore2.distanceToGo()==0 &&
        motore3.distanceToGo()==0 &&
        motore4.distanceToGo()==0) {
      multiActive = false;
      Serial.println("ready");
    }
  } else {
    checkMotor(motore1, motore1Completato, ENDSTOP_1_PIN);
    checkMotor(motore2, motore2Completato, ENDSTOP_2_PIN);
    checkMotor(motore3, motore3Completato, ENDSTOP_3_PIN);
    checkMotor(motore4, motore4Completato, ENDSTOP_4_PIN);
  }
}

void setupMotor(AccelStepper& motore, int pinENA) {
  pinMode(pinENA, OUTPUT);
  setEnableAll(true);
  motore.setMaxSpeed(maxSpeed);
  motore.setAcceleration(maxAccel);
  motore.setMinPulseWidth(5); 
}

void setEnableAll(bool on) {
  bool level = EN_ACTIVE_HIGH ? (on ? HIGH : LOW) : (on ? LOW : HIGH);
  digitalWrite(ENA1, level);
  digitalWrite(ENA2, level);
  digitalWrite(ENA3, level);
  digitalWrite(ENA4, level);

  encoderStreamOn = !on;
}


void setTarget(AccelStepper& motore, int targetGradi) {
  if (targetGradi < -360 || targetGradi > 360) {
    Serial.println(F("Errore: target fuori limite (-360..360°)"));
    return;
  }
  long targetPassi = (long)(targetGradi * passiPerGrado);
  motore.moveTo(targetPassi);
  Serial.print(F("Impostato target passi: "));
  Serial.println(targetPassi);
}

void checkMotor(AccelStepper& motore, bool& completato, int endstopPin) {
  if (!completato) {
    if (ENDSTOP_ENABLED == 1 && digitalRead(endstopPin) == LOW) {
      motore.stop();
      motore.setCurrentPosition(0);
      completato = true;
      Serial.println(F("Finecorsa raggiunto!"));

      // backoff
      int gradiBackoff = 5;
      long passiBackoff = (long)(gradiBackoff * passiPerGrado);
      Serial.println(F("Rilascio finecorsa..."));
      motore.moveTo(passiBackoff);
      while (motore.distanceToGo() != 0) motore.run();
      motore.setCurrentPosition(0);
      Serial.println(F("Backoff completato"));
      return;
    }

    motore.run(); 

    if (motore.distanceToGo() == 0) {
      completato = true;
    }
  }
}

void homingMotor(AccelStepper& motore, int endstopPin, int velocitaNegativa) {
  if (ENDSTOP_ENABLED == 0) {
    Serial.println(F("Endstop disabilitato: homing saltato."));
    mostraMessaggio("Homing disabilitato");
    delay(1000);
    return;
  }

  mostraMessaggio("Homing in corso...");
  long prevMaxSpeed = (long)motore.maxSpeed();
  long prevAccel = (long)motore.acceleration();

  motore.enableOutputs();
  motore.setMaxSpeed(abs(velocitaNegativa));
  motore.setAcceleration(500);
  motore.setSpeed(velocitaNegativa);

  unsigned long startTime = millis();
  while (digitalRead(endstopPin) == HIGH && millis() - startTime < 10000) {
    motore.runSpeed();
  }
  if (digitalRead(endstopPin) == HIGH) {
    Serial.println(F("⚠️ Finecorsa non raggiunto entro timeout!"));
  }
  motore.stop();
  motore.setCurrentPosition(0);
  Serial.println(F("Homing completato"));

  int gradiBackoff = 5;
  long passiBackoff = (long)(gradiBackoff * passiPerGrado);
  motore.moveTo(passiBackoff);
  while (motore.distanceToGo() != 0) motore.run();
  motore.setCurrentPosition(0);
  Serial.println(F("Backoff completato"));

  motore.setMaxSpeed(prevMaxSpeed);
  motore.setAcceleration(prevAccel);
  mostraMessaggio("Pronto");
}
