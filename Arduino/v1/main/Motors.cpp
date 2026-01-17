#include "Motors.h"
#include "Display.h"
#include "Config.h"
#include "Encoder.h"

AccelStepper motore1(AccelStepper::DRIVER, PUL1, DIR1);
AccelStepper motore2(AccelStepper::DRIVER, PUL2, DIR2);
AccelStepper motore3(AccelStepper::DRIVER, PUL3, DIR3);
AccelStepper motore4(AccelStepper::DRIVER, PUL4, DIR4);
Servo gripper;

bool motore1Completato = true;
bool motore2Completato = true;
bool motore3Completato = true;
bool motore4Completato = true;
 
// Flag globale per indicare se siamo in movimento coordinato
bool movingCoordinated = false;

// Configurazione centralizzata dei motori
// Indice 0 = motore1, 1 = motore2, ecc.
// invertRotation: true = Inverte i pin DIR (Cambia il senso di rotazione per TUTTI i movimenti)
// homingDirectionSign: 1 = Homing verso Positivo, -1 = Homing verso Negativo. (SCEGLIERE SEPARATAMENTE)
MotorConfig motorConfigs[4] = {
    { false, -1 }, // Motore 1: Rotazione Normale, Homing verso Negativo
    { true,  -1 }, // Motore 2: Rotazione Invertita, Homing verso Positivo
    { true, -1 }, // Motore 3: Rotazione Normale, Homing verso Negativo
    { true, -1 }  // Motore 4: Rotazione Normale, Homing verso Negativo
};

void setupMotors(){
  setupMotor(motore1, ENA1, 0);
  setupMotor(motore2, ENA2, 1);
  setupMotor(motore3, ENA3, 2);
  setupMotor(motore4, ENA4, 3);
}

void moveAllToDegrees(int g1,int g2,int g3,int g4){
  // Funzione legacy o di test rapido
  // Impostiamo i target globali e chiamiamo la nuova funzione
  target1 = g1;
  target2 = g2;
  target3 = g3;
  target4 = g4;
  startCoordinatedMove();
}

// Esegue movimento coordinato scalando le velocità
void startCoordinatedMove() {
  // 1. Calcola posizione attuale e target in passi
  long current1 = motore1.currentPosition();
  long current2 = motore2.currentPosition();
  long current3 = motore3.currentPosition();
  long current4 = motore4.currentPosition();

  long targetPos1 = (long)(target1 * passiPerGrado);
  long targetPos2 = (long)(target2 * passiPerGrado);
  long targetPos3 = (long)(target3 * passiPerGrado);
  long targetPos4 = (long)(target4 * passiPerGrado);

  // 2. Calcola distanza assoluta per ogni motore
  long dist1 = abs(targetPos1 - current1);
  long dist2 = abs(targetPos2 - current2);
  long dist3 = abs(targetPos3 - current3);
  long dist4 = abs(targetPos4 - current4);

  // 3. Trova la distanza massima
  long maxDist = dist1;
  if (dist2 > maxDist) maxDist = dist2;
  if (dist3 > maxDist) maxDist = dist3;
  if (dist4 > maxDist) maxDist = dist4;

  if (maxDist == 0) {
    // Nessun movimento necessario
    movingCoordinated = false;
    Serial.println("ready");
    return;
  }

  // 4. Calcola e imposta velocità/accelerazione scalate per ogni motore
  // Il motore che deve fare più strada andrà a velocità piena (maxSpeed).
  // Gli altri andranno in proporzione: ratio = dist / maxDist
  
  auto setupAxis = [&](AccelStepper& m, long dist) {
      if (dist == 0) {
          m.setMaxSpeed(100.0f); // Velocità minima arbitraria per non dividere per 0 internalmente
          m.setAcceleration(100.0f);
      } else {
          float ratio = (float)dist / (float)maxDist;
          m.setMaxSpeed(maxSpeed * ratio);
          m.setAcceleration(maxAccel * ratio);
      }
  };

  setupAxis(motore1, dist1);
  setupAxis(motore2, dist2);
  setupAxis(motore3, dist3);
  setupAxis(motore4, dist4);

  // 5. Imposta i target
  motore1.moveTo(targetPos1);
  motore2.moveTo(targetPos2);
  motore3.moveTo(targetPos3);
  motore4.moveTo(targetPos4);

  movingCoordinated = true;
  
  // Reset flag completamento
  motore1Completato = false;
  motore2Completato = false;
  motore3Completato = false;
  motore4Completato = false;

  Serial.println(F("Start coordinated move..."));
}

void handleMotors() {
  
  if (movingCoordinated) {
    // Esegui run() per tutti i motori in un loop BLOCCANTE
    // Questo evita la latenza del loop() principale (Serial, LCD, ecc.)
    // rendendo il movimento fluido come l'homing.

    bool emergencyStop = false;

    // Loop dedicato "tight"
    while (true) {
        bool running = false;

        // 1. Step motori (priorità massima)
        if (motore1.distanceToGo() != 0) { motore1.run(); running = true; }
        if (motore2.distanceToGo() != 0) { motore2.run(); running = true; }
        if (motore3.distanceToGo() != 0) { motore3.run(); running = true; }
        if (motore4.distanceToGo() != 0) { motore4.run(); running = true; }

        // 2. Controllo Stop Button (safety check rapido)
        // Usiamo digitalRead diretto per non perdere tempo
        if (digitalRead(BTN_STOP_PIN) == LOW) {
            emergencyStop = true;
            break; 
        }

        // 3. Se nessun motore si muove, abbiamo finito
        if (!running) {
            break; 
        }
    }

    if (emergencyStop) {
        // Stop immediato
        motore1.stop(); motore1.setCurrentPosition(motore1.currentPosition());
        motore2.stop(); motore2.setCurrentPosition(motore2.currentPosition());
        motore3.stop(); motore3.setCurrentPosition(motore3.currentPosition());
        motore4.stop(); motore4.setCurrentPosition(motore4.currentPosition());
        
        // Reset flag
        motore1Completato = true;
        motore2Completato = true;
        motore3Completato = true;
        motore4Completato = true;
        eseguiMovimento = false;
        
        Serial.println(F("STOP Button Detected during move!"));
        // mostraMessaggio("STOP EMERGENCY"); // Opzionale, richiede Display.h
    }

    movingCoordinated = false;
      
    // Ripristina velocità originali
    motore1.setMaxSpeed(maxSpeed); motore1.setAcceleration(maxAccel);
    motore2.setMaxSpeed(maxSpeed); motore2.setAcceleration(maxAccel);
    motore3.setMaxSpeed(maxSpeed); motore3.setAcceleration(maxAccel);
    motore4.setMaxSpeed(maxSpeed); motore4.setAcceleration(maxAccel);

    Serial.println("ready");

  } else {
    // Modalità manuale indipendente (non usata per movimenti lunghi coord)
    checkMotor(motore1, motore1Completato, ENDSTOP_1_PIN);
    checkMotor(motore2, motore2Completato, ENDSTOP_2_PIN);
    checkMotor(motore3, motore3Completato, ENDSTOP_3_PIN);
    checkMotor(motore4, motore4Completato, ENDSTOP_4_PIN);
  }
}

void setupMotor(AccelStepper& motore, int pinENA, int motorIndex) {
  pinMode(pinENA, OUTPUT);
  setEnableAll(true);
  
  // Applica inversione se configurata
  if (motorConfigs[motorIndex].invertRotation) {
      motore.setPinsInverted(true, false, false);
  }

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

  if (on) {
    // i motori si stanno disabilitando → non fare nulla
  } else { 
    // Sincronizza posizioni motori con encoder
    motore1.setCurrentPosition((long)(jointDeg1 * passiPerGrado)); 
    motore2.setCurrentPosition((long)(jointDeg2 * passiPerGrado)); 
    motore3.setCurrentPosition((long)(jointDeg3 * passiPerGrado)); 
    motore4.setCurrentPosition((long)(jointDeg4 * passiPerGrado)); 
  }
}


void setTarget(AccelStepper& motore, int targetGradi) {
  if (targetGradi < -360 || targetGradi > 360) {
    Serial.println(F("Errore: target fuori limite (-360..360°)"));
    return;
  }
  long targetPassi = (long)(targetGradi * passiPerGrado);
  motore.moveTo(targetPassi);
  
  static unsigned long last=0;
  if (millis()-last>120) { 
    last=millis(); 
    Serial.print(F("Impostato target passi: ")); 
    Serial.println(targetPassi); 
  }

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

void homingMotor(AccelStepper& motore, int endstopPin, int motorIndex) {
  if (ENDSTOP_ENABLED == 0) {
    Serial.println(F("Endstop disabilitato: homing saltato."));
    mostraMessaggio("Homing disabilitato");
    delay(1000);
    return;
  }

  int baseHomingSpeed = 1000; //maxSpeed;
  
  // Calcola velocità effettiva basata sulla direzione configurata
  // Se homingDir è -1 (standard), speed sarà -2000
  // Se homingDir è 1 (invertito), speed sarà 2000
  int velocitaHoming = baseHomingSpeed * motorConfigs[motorIndex].homingDirectionSign;

  mostraMessaggio("Homing in corso...");
  long prevMaxSpeed = (long)motore.maxSpeed();
  long prevAccel = (long)motore.acceleration();

  motore.enableOutputs();
  motore.setMaxSpeed(abs(velocitaHoming));
  motore.setAcceleration(500);
  motore.setSpeed(velocitaHoming);

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
  //motore.moveTo(passiBackoff);
  while (motore.distanceToGo() != 0) motore.run();
  motore.setCurrentPosition(0);
  Serial.println(F("Backoff completato"));

  motore.setMaxSpeed(prevMaxSpeed);
  motore.setAcceleration(prevAccel);

  // Imposta zero per tutti gli encoder attivi
  encoderZero(1);  // Motor 1
  encoderZero(2);  // Motor 2
  encoderZero(3);  // Motor 3
  encoderZero(4);  // Motor 4
  encoderZero(5);  // Motor 5

  mostraMessaggio("Pronto");
}

void setupGripper() {
  gripper.attach(GRIPPER_PIN);
  gripper.write(90); // Default position
}

void setGripperAngle(int angle) {
  if (angle < 0) angle = 0;
  if (angle > 180) angle = 180;
  gripper.write(angle);
}