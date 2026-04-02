#include "Motors.h"
#include "Display.h"
#include "Config.h"
#include "Encoder.h"
#include "PressureSensors.h"

AccelStepper motore1(AccelStepper::DRIVER, PUL1, DIR1);
AccelStepper motore2(AccelStepper::DRIVER, PUL2, DIR2);
AccelStepper motore3(AccelStepper::DRIVER, PUL3, DIR3);
AccelStepper motore4(AccelStepper::DRIVER, PUL4, DIR4);
AccelStepper motore5(AccelStepper::DRIVER, PUL5, DIR5);
AccelStepper motore6(AccelStepper::DRIVER, PUL6, DIR6);
Servo gripper;
static int gripperAngleDeg = 0;
static const uint8_t GRIPPER_STEP_DEG = 1;
static const uint8_t GRIPPER_STEP_DELAY_MS = 20;

bool motore1Completato = true;
bool motore2Completato = true;
bool motore3Completato = true;
bool motore4Completato = true;
bool motore5Completato = true;
bool motore6Completato = true;

// Flag globale per indicare se siamo in movimento coordinato
bool movingCoordinated = false;
static bool motor6Enabled = true;
static unsigned long motor6LastActivityMs = 0;
static const unsigned long MOTOR6_IDLE_TIMEOUT_MS = 3000;
static bool motor6MovedInLastRun = false;

// Configurazione centralizzata degli assi reali configurabili.
// Indice 0 = motore1, 1 = motore2, 2 = motore3, 3 = motore5, 4 = motore6
// invertRotation: true = Inverte i pin DIR (Cambia il senso di rotazione per TUTTI i movimenti)
// homingDirectionSign: 1 = Homing verso Positivo, -1 = Homing verso Negativo. (SCEGLIERE SEPARATAMENTE)
MotorConfig motorConfigs[5] = {
    {false, -1}, // Motore 1: Rotazione Normale, Homing verso Negativo
    {false, 1},  // Motore 2: Rotazione Normale, Homing verso Positivo
    {true, -1},  // Motore 3: Rotazione Invertita, Homing verso Negativo
    {true, -1},  // Motore 5: Rotazione Invertita, homing non usato
    {false, -1}  // Motore 6: Rotazione Normale, homing non usato
};

void setupMotors()
{
  setupMotor(motore1, ENA1, 0);
  setupMotor(motore2, ENA2, 1);
  setupMotor(motore3, ENA3, 2);

  // M4 non ha configurazione autonoma: e' sempre mirror di M2.
  pinMode(ENA4, OUTPUT);
  setEnableAll(true);
  motore4.setMaxSpeed(maxSpeed);
  motore4.setAcceleration(maxAccel);
  motore4.setMinPulseWidth(5);

  setupMotor(motore5, ENA5, 3);
  setupMotor(motore6, ENA6, 4);
  motor6LastActivityMs = millis();
}

void moveAllToDegrees(int g1, int g2, int g3, int g4, int g5, int g6)
{
  // Funzione legacy o di test rapido
  // M4 e' derivato da M2 (specchiato), il quarto parametro viene ignorato.
  (void)g4;
  target1 = g1;
  target2 = g2;
  target3 = g3;
  target4 = -g2;
  target5 = g5;
  target6 = g6;
  startCoordinatedMove();
}

// Esegue movimento coordinato scalando le velocità
// Esegue movimento coordinato scalando le velocità
void startCoordinatedMove()
{
  // M4 e' asse virtuale: sempre in verso opposto a M2.
  target4 = -target2;

  // 1. Calcola posizione attuale e target in passi
  long current1 = motore1.currentPosition();
  long current2 = motore2.currentPosition();
  long current3 = motore3.currentPosition();
  long current4 = motore4.currentPosition();
  long current5 = motore5.currentPosition();
  long current6 = motore6.currentPosition();

  long targetPos1 = (long)(target1 * passiPerGrado);
  long targetPos2 = -(long)(target2 * passiPerGrado);
  long targetPos3 = (long)(target3 * passiPerGrado);
  // M4 deve replicare lo stesso delta di M2, ma in verso opposto.
  // In questo modo i due motori restano accoppiati anche se le posizioni
  // correnti non sono perfettamente specchiate.
  long delta2 = targetPos2 - current2;
  long targetPos4 = current4 - delta2;
  long targetPos5 = (long)(target5 * passiPerGrado);
  long targetPos6 = (long)(target6 * passiPerGrado);

  // 2. Calcola distanza assoluta per ogni motore
  long dist1 = abs(targetPos1 - current1);
  long dist2 = abs(targetPos2 - current2);
  long dist3 = abs(targetPos3 - current3);
  long dist4 = abs(delta2);
  long dist5 = abs(targetPos5 - current5);
  long dist6 = abs(targetPos6 - current6);
  motor6MovedInLastRun = (dist6 > 0);

  // 3. Trova la distanza massima
  long maxDist = dist1;
  if (dist2 > maxDist)
    maxDist = dist2;
  if (dist3 > maxDist)
    maxDist = dist3;
  if (dist4 > maxDist)
    maxDist = dist4;
  if (dist5 > maxDist)
    maxDist = dist5;
  if (dist6 > maxDist)
    maxDist = dist6;

  if (maxDist == 0)
  {
    // Nessun movimento necessario
    movingCoordinated = false;
    motor6MovedInLastRun = false;
    Serial.println("ready");
    return;
  }

  // Calcola e imposta velocità/accelerazione scalate per ogni motore
  // Il motore che deve fare più strada andrà a velocità piena (maxSpeed).
  // Gli altri andranno in proporzione, MA con un limite minimo di velocità
  // per evitare risonanze e rumori (scattosità).

  const float MIN_SPEED_THRESHOLD = 200.0f; // Velocità minima (step/sec) sotto la quale non scendiamo
  const float MIN_ACCEL_THRESHOLD = 100.0f;

  auto setupAxis = [&](AccelStepper &m, long dist)
  {
    if (dist == 0)
    {
      m.setMaxSpeed(MIN_SPEED_THRESHOLD);
      m.setAcceleration(MIN_ACCEL_THRESHOLD);
    }
    else
    {
      float ratio = (float)dist / (float)maxDist;
      float speed = maxSpeed * ratio;
      float accel = maxAccel * ratio;

      // Se la velocità calcolata è troppo bassa, la alziamo al minimo decente.
      // Questo significa che il motore finirà PRIMA degli altri (desincronizzazione),
      // ma il movimento sarà fluido e silenzioso.
      if (speed < MIN_SPEED_THRESHOLD)
      {
        speed = MIN_SPEED_THRESHOLD;
        // Scaliamo l'accelerazione in proporzione alla nuova velocità forzata o usiamo un fisso?
        // Usiamo un fisso morbido o ricalcoliamo.
        // Se speed aumenta, accel dovrebbe aumentare per raggiungerla in tempi utili.
        // Semplifichiamo: se speed è bassa force, usiamo accel bassa force.
        accel = MIN_ACCEL_THRESHOLD;
      }

      m.setMaxSpeed(speed);
      m.setAcceleration(accel);
    }
  };

  setupAxis(motore1, dist1);
  setupAxis(motore2, dist2);
  setupAxis(motore3, dist3);
  // M4 usa lo stesso profilo di M2 per evitare ritardi percepibili.
  setupAxis(motore4, dist2);
  setupAxis(motore5, dist5);
  setupAxis(motore6, dist6);

  // 5. Imposta i target
  motore1.moveTo(targetPos1);
  motore2.moveTo(targetPos2);
  motore3.moveTo(targetPos3);
  motore4.moveTo(targetPos4);
  motore5.moveTo(targetPos5);
  motore6.moveTo(targetPos6);

  movingCoordinated = true;

  // Reset flag completamento (semplice stato)
  motore1Completato = false;
  motore2Completato = false;
  motore3Completato = false;
  motore4Completato = false;
  motore5Completato = false;
  motore6Completato = false;

  Serial.println(F("Start coordinated move (optimized)..."));
}

void handleMotors()
{
  // Aggiorna stato homing non-bloccante prima di gestire i motori normali
  homingUpdate();
  // Durante homing non-bloccante non eseguire altra logica motori:
  // checkMotor() sullo stesso asse può interferire con runSpeed()/backoff.
  if (homingInProgress)
  {
    updateMotor6IdleState();
    return;
  }

  if (movingCoordinated)
  {
    // Esegui run() per tutti i motori in un loop BLOCCANTE
    // Questo evita la latenza del loop() principale (Serial, LCD, ecc.)
    // rendendo il movimento fluido come l'homing.

    bool emergencyStop = false;

    // Loop dedicato "tight"
    while (true)
    {
      bool running = false;

      // 1. Step motori (priorità massima)
      if (motore1.distanceToGo() != 0)
      {
        motore1.run();
        running = true;
      }
      if (motore2.distanceToGo() != 0)
      {
        motore2.run();
        running = true;
      }
      if (motore3.distanceToGo() != 0)
      {
        motore3.run();
        running = true;
      }
      if (motore4.distanceToGo() != 0)
      {
        motore4.run();
        running = true;
      }
      if (motore5.distanceToGo() != 0)
      {
        motore5.run();
        running = true;
      }
      if (motore6.distanceToGo() != 0)
      {
        motore6.run();
        running = true;
      }

      // 2. Controllo Stop Button (safety check rapido)
      // Usiamo digitalRead diretto per non perdere tempo
      if (digitalRead(BTN_STOP_PIN) == LOW)
      {
        emergencyStop = true;
        break;
      }

      // 3. Se nessun motore si muove, abbiamo finito
      if (!running)
      {
        break;
      }
    }

    if (emergencyStop)
    {
      // Stop immediato
      motore1.stop();
      motore1.setCurrentPosition(motore1.currentPosition());
      motore2.stop();
      motore2.setCurrentPosition(motore2.currentPosition());
      motore3.stop();
      motore3.setCurrentPosition(motore3.currentPosition());
      motore4.stop();
      motore4.setCurrentPosition(motore4.currentPosition());
      motore5.stop();
      motore5.setCurrentPosition(motore5.currentPosition());
      motore6.stop();
      motore6.setCurrentPosition(motore6.currentPosition());

      // Reset flag
      motore1Completato = true;
      motore2Completato = true;
      motore3Completato = true;
      motore4Completato = true;
      motore5Completato = true;
      motore6Completato = true;
      eseguiMovimento = false;

      Serial.println(F("STOP Button Detected during move!"));
      // mostraMessaggio("STOP EMERGENCY"); // Opzionale, richiede Display.h
    }

    // Il movimento coordinato è terminato: riallinea subito i flag di stato.
    if (motor6MovedInLastRun)
    {
      markMotor6Activity();
    }
    motore1Completato = true;
    motore2Completato = true;
    motore3Completato = true;
    motore4Completato = true;
    motore5Completato = true;
    motore6Completato = true;

    movingCoordinated = false;

    // Ripristina velocità originali
    motore1.setMaxSpeed(maxSpeed);
    motore1.setAcceleration(maxAccel);
    motore2.setMaxSpeed(maxSpeed);
    motore2.setAcceleration(maxAccel);
    motore3.setMaxSpeed(maxSpeed);
    motore3.setAcceleration(maxAccel);
    motore4.setMaxSpeed(maxSpeed);
    motore4.setAcceleration(maxAccel);
    motore5.setMaxSpeed(maxSpeed);
    motore5.setAcceleration(maxAccel);
    motore6.setMaxSpeed(maxSpeed);
    motore6.setAcceleration(maxAccel);
    motor6MovedInLastRun = false;

    Serial.println("ready");
  }
  else
  {
    // Modalità manuale indipendente (non usata per movimenti lunghi coord)
    checkMotor(motore1, motore1Completato, ENDSTOP_1_PIN);
    checkMotor(motore2, motore2Completato, ENDSTOP_2_PIN);
    checkMotor(motore3, motore3Completato, ENDSTOP_3_PIN);
  }
  updateMotor6IdleState();
}

void setupMotor(AccelStepper &motore, int pinENA, int motorIndex)
{
  pinMode(pinENA, OUTPUT);
  setEnableAll(true);

  // Applica inversione se configurata
  if (motorConfigs[motorIndex].invertRotation)
  {
    motore.setPinsInverted(true, false, false);
  }

  motore.setMaxSpeed(maxSpeed);
  motore.setAcceleration(maxAccel);
  motore.setMinPulseWidth(5);
}

void setEnableAll(bool on)
{
  bool level = EN_ACTIVE_HIGH ? (on ? HIGH : LOW) : (on ? LOW : HIGH);
  digitalWrite(ENA1, level);
  digitalWrite(ENA2, level);
  digitalWrite(ENA3, level);
  digitalWrite(ENA4, level);
  digitalWrite(ENA5, level);
  digitalWrite(ENA6, level);
  motor6Enabled = on;
  if (on)
  {
    motor6LastActivityMs = millis();
  }

  encoderStreamOn = !on;

  if (on)
  {
    // i motori si stanno disabilitando → non fare nulla
  }
  else
  {
    // Sincronizza posizioni motori con encoder
    motore1.setCurrentPosition((long)(jointDeg1 * passiPerGrado));
    motore2.setCurrentPosition((long)(jointDeg2 * passiPerGrado));
    motore3.setCurrentPosition((long)(jointDeg3 * passiPerGrado));
    motore4.setCurrentPosition((long)(jointDeg4 * passiPerGrado));
    motore5.setCurrentPosition((long)(jointDeg5 * passiPerGrado));
  }
}

void setEnableMotor6(bool on)
{
  bool level = EN_ACTIVE_HIGH ? (on ? HIGH : LOW) : (on ? LOW : HIGH);
  digitalWrite(ENA6, level);
  motor6Enabled = on;
  if (on)
  {
    motor6LastActivityMs = millis();
  }
}

void markMotor6Activity()
{
  motor6LastActivityMs = millis();
  if (motorsEnabled && !motor6Enabled)
  {
    setEnableMotor6(true);
  }
}

void updateMotor6IdleState()
{
  if (!motorsEnabled)
  {
    if (motor6Enabled)
    {
      setEnableMotor6(false);
    }
    return;
  }

  if (!motor6Enabled)
  {
    return;
  }

  const bool motor6Busy = homingInProgress || movingCoordinated || motore6.distanceToGo() != 0 || !motore6Completato;
  if (motor6Busy)
  {
    motor6LastActivityMs = millis();
    return;
  }

  if (millis() - motor6LastActivityMs >= MOTOR6_IDLE_TIMEOUT_MS)
  {
    setEnableMotor6(false);
  }
}

void setTarget(AccelStepper &motore, int targetGradi)
{
  if (targetGradi < -360 || targetGradi > 360)
  {
    Serial.println(F("Errore: target fuori limite (-360..360°)"));
    return;
  }
  long targetPassi = (long)(targetGradi * passiPerGrado);
  // Inverti SOLO il movimento normale di M2/M4 (homing escluso).
  if (&motore == &motore2 || &motore == &motore4)
    targetPassi = -targetPassi;
  if (&motore == &motore6)
    markMotor6Activity();
  motore.moveTo(targetPassi);

  static unsigned long last = 0;
  if (millis() - last > 120)
  {
    last = millis();
    Serial.print(F("Impostato target passi: "));
    Serial.println(targetPassi);
  }
}

void setCoupledTargetM2M4(int targetGradiM2)
{
  if (targetGradiM2 < -360 || targetGradiM2 > 360)
  {
    Serial.println(F("Errore: target fuori limite (-360..360\xB0)"));
    return;
  }

  long targetPassiM2 = -(long)(targetGradiM2 * passiPerGrado);
  long deltaM2 = targetPassiM2 - motore2.currentPosition();
  long targetPassiM4 = motore4.currentPosition() - deltaM2;

  motore2.moveTo(targetPassiM2);
  motore4.moveTo(targetPassiM4);

  static unsigned long last = 0;
  if (millis() - last > 120)
  {
    last = millis();
    Serial.print(F("Coupled M2/M4 target passi: "));
    Serial.print(targetPassiM2);
    Serial.print(F(" / "));
    Serial.println(targetPassiM4);
  }
}

void checkMotor(AccelStepper &motore, bool &completato, int endstopPin)
{
  if (!completato)
  {
    if (ENDSTOP_ENABLED == 1 && digitalRead(endstopPin) == LOW)
    {
      motore.stop();
      motore.setCurrentPosition(0);
      completato = true;
      Serial.println(F("Finecorsa raggiunto!"));

      // backoff
      int gradiBackoff = 5;
      long passiBackoff = (long)(gradiBackoff * passiPerGrado);
      Serial.println(F("Rilascio finecorsa..."));
      motore.moveTo(passiBackoff);
      while (motore.distanceToGo() != 0)
        motore.run();
      motore.setCurrentPosition(0);
      Serial.println(F("Backoff completato"));
      return;
    }

    motore.run();

    if (motore.distanceToGo() == 0)
    {
      completato = true;
    }
  }
}

// Homing non-bloccante: stato per gli assi reali con finecorsa.
enum HomingPhase { H_IDLE = 0, H_SEEK = 1, H_BACKOFF = 2, H_DONE = 3 };
static HomingPhase homingPhase[5] = { H_IDLE, H_IDLE, H_IDLE, H_IDLE, H_IDLE };
static bool homingActiveArr[5] = { false, false, false, false, false };
static long prevMaxSpeedArr[5] = {0,0,0,0,0};
static long prevAccelArr[5] = {0,0,0,0,0};
static long prevMaxSpeedM4 = 0;
static long prevAccelM4 = 0;

bool homingInProgress = false;

void homingMotor(AccelStepper &motore, int endstopPin, int motorIndex)
{
  if (ENDSTOP_ENABLED == 0)
  {
    Serial.println(F("Endstop disabilitato: homing saltato."));
    mostraMessaggio("Homing disabilitato");
    delay(1000);
    return;
  }

  // Homing agganciato ai parametri runtime del movimento, ma con limiti di sicurezza.
  const int baseHomingSpeed = constrain(maxSpeed, 80, 600);
  const int homingAccel = constrain(maxAccel, 50, 500);
  const int speedSign = motorConfigs[motorIndex].homingDirectionSign;
  int velocitaHoming = baseHomingSpeed * speedSign;

  Serial.println("Avvio homing non-bloccante motore " + String(motorIndex + 1) + " endstopPin: " + String(endstopPin));

  // Salva velocità/accel precedenti
  prevMaxSpeedArr[motorIndex] = (long)motore.maxSpeed();
  prevAccelArr[motorIndex] = (long)motore.acceleration();

  const bool coupledM2M4 = (motorIndex == 1);
  if (motorIndex == 4)
  {
    markMotor6Activity();
  }
  if (coupledM2M4)
  {
    prevMaxSpeedM4 = (long)motore4.maxSpeed();
    prevAccelM4 = (long)motore4.acceleration();
  }

  motore.enableOutputs();
  motore.setMaxSpeed(abs(velocitaHoming));
  motore.setAcceleration(homingAccel);
  motore.setSpeed(velocitaHoming);

  if (coupledM2M4)
  {
    motore4.enableOutputs();
    motore4.setMaxSpeed(abs(velocitaHoming));
    motore4.setAcceleration(homingAccel);
    motore4.setSpeed(-velocitaHoming);
  }

  // Attiva lo stato; l'azione procederà in homingUpdate() chiamata da handleMotors()
  movingCoordinated = false;
  homingActiveArr[motorIndex] = true;
  homingPhase[motorIndex] = H_SEEK;
  homingInProgress = true;
}

// Aggiornamento non-bloccante dello stato di homing; chiamare frequentemente (es. da handleMotors())
void homingUpdate()
{
  AccelStepper* motors[5] = { &motore1, &motore2, &motore3, &motore5, &motore6 };
  int endPins[5] = { ENDSTOP_1_PIN, ENDSTOP_2_PIN, ENDSTOP_3_PIN, ENDSTOP_5_PIN, ENDSTOP_6_PIN };

  bool anyActive = false;
  for (int i = 0; i < 5; ++i)
  {
    if (!homingActiveArr[i]) continue;
    anyActive = true;
    AccelStepper* m = motors[i];
    const bool coupled = (i == 1);
    int ep = endPins[i];

    if (homingPhase[i] == H_SEEK)
    {
      // Avanzamento a velocità costante
      m->runSpeed();
      if (coupled) motore4.runSpeed();
      if (digitalRead(ep) == LOW)
      {
        // Raggiunto il finecorsa
        m->stop();
        if (coupled) motore4.stop();

        m->setCurrentPosition(0);
        if (coupled) motore4.setCurrentPosition(0);

        Serial.println(F("Homing: finecorsa raggiunto"));

        // Avvia backoff
        int gradiBackoff = 5;
        long passiBackoff = (long)(gradiBackoff * passiPerGrado);
        long dirBackoff = (long)(-motorConfigs[i].homingDirectionSign * passiBackoff);
        m->moveTo(m->currentPosition() + dirBackoff);
        if (coupled) motore4.moveTo(motore4.currentPosition() - dirBackoff);

        homingPhase[i] = H_BACKOFF;
      }
    }
    else if (homingPhase[i] == H_BACKOFF)
    {
      bool running = false;
      if (m->distanceToGo() != 0)
      {
        m->run();
        running = true;
      }
      if (coupled && motore4.distanceToGo() != 0)
      {
        motore4.run();
        running = true;
      }

      if (!running)
      {
        // Backoff completato
        m->setCurrentPosition(0);
        if (coupled) motore4.setCurrentPosition(0);
        Serial.println(F("Homing: backoff completato"));

        // Ripristina parametri
        m->setMaxSpeed(prevMaxSpeedArr[i]);
        m->setAcceleration(prevAccelArr[i]);
        if (coupled)
        {
          motore4.setMaxSpeed(prevMaxSpeedM4);
          motore4.setAcceleration(prevAccelM4);
        }

        // Encoder presenti solo su M1..M5. M6 resta senza zero encoder.
        const int encoderMotorNum = (i <= 2) ? (i + 1) : (i == 3 ? 5 : 0);
        if (encoderMotorNum != 0) {
          encoderZero(encoderMotorNum);
        }
        if (coupled) encoderZero(4);

        homingActiveArr[i] = false;
        homingPhase[i] = H_DONE;
        if (i == 4)
        {
          markMotor6Activity();
        }
      }
    }
  }

  homingInProgress = anyActive;
}

void setupGripper()
{
  gripper.attach(GRIPPER_PIN);
  gripperAngleDeg = 0;
  gripper.write(gripperAngleDeg); // Default position
}

void setGripperAngle(int angle)
{
  if (angle < 0)
    angle = 0;
  if (angle > 90)
    angle = 90;

  if (angle == gripperAngleDeg) {
    pressureSensorsUpdate();
    return;
  }

  const int direction = (angle > gripperAngleDeg) ? 1 : -1;
  while (gripperAngleDeg != angle) {
    gripperAngleDeg += direction * GRIPPER_STEP_DEG;
    if ((direction > 0 && gripperAngleDeg > angle) ||
        (direction < 0 && gripperAngleDeg < angle)) {
      gripperAngleDeg = angle;
    }

    gripper.write(gripperAngleDeg);
    delay(GRIPPER_STEP_DELAY_MS);
    pressureSensorsUpdate();
  }
}
