#include "Encoder.h"
#include "Config.h"

// Struttura per gestire un singolo encoder
struct EncoderData {
  float ema;                  // filtro EMA
  float angleDeg;             // istantaneo motore 0..360
  
  // calibrazione RAW
  int  minRawSeen;
  int  maxRawSeen;
  int  minCal;
  int  maxCal;
  bool calLocked;
  
  // unwrap e parametri giunto
  bool  firstSample;
  float last0_360;
  float motorContDeg;        // motore continuo (può crescere senza limiti)
  int   dirSign;             // +1 o -1 (inverti verso se serve)
  float jointZero;           // offset zero giunto
  float jointDeg;
  float emaMotorContDeg;
  
  int encoderPin;            // pin analogico
};

// Istanze dei cinque encoder
static EncoderData encoder1, encoder2, encoder3, encoder4, encoder5;

// Variabili globali esportate
float angleDeg1 = 0.0f, angleDeg2 = 0.0f, angleDeg3 = 0.0f, angleDeg4 = 0.0f, angleDeg5 = 0.0f;
float jointDeg1 = 0.0f, jointDeg2 = 0.0f, jointDeg3 = 0.0f, jointDeg4 = 0.0f, jointDeg5 = 0.0f;

// Funzione helper per inizializzare un encoder
void initEncoder(EncoderData& enc, int pin) {
  enc.encoderPin = pin;
  enc.ema = analogRead(pin);
  enc.angleDeg = 0.0f;
  enc.minRawSeen = 1023;
  enc.maxRawSeen = 0;
  enc.minCal = 0;
  enc.maxCal = 1023;
  enc.calLocked = false;
  enc.firstSample = true;
  enc.last0_360 = 0.0f;
  enc.motorContDeg = 0.0f;
  enc.dirSign = -1;
  enc.jointZero = 0.0f;
  enc.jointDeg = 0.0f;
  enc.emaMotorContDeg = 0.0f;
}

void encoderSetup() {
  initEncoder(encoder1, ENCODER_PIN_MOTOR1);
  initEncoder(encoder2, ENCODER_PIN_MOTOR2);
  initEncoder(encoder3, ENCODER_PIN_MOTOR3);
  initEncoder(encoder4, ENCODER_PIN_MOTOR4);
  initEncoder(encoder5, ENCODER_PIN_MOTOR5);
}

// Ottieni riferimento all'encoder in base al numero motore
EncoderData* getEncoder(int motorNum) {
  switch(motorNum) {
    case 1: return &encoder1;
    case 2: return &encoder2;
    case 3: return &encoder3;
    case 4: return &encoder4;
    case 5: return &encoder5;
    default: return nullptr;
  }
}

// Avvia una nuova calibrazione per un encoder specifico
void encoderStartCal(int motorNum) {
  EncoderData* enc = getEncoder(motorNum);
  if (!enc) return;
  
  enc->calLocked = false;
  enc->minRawSeen = 1023;
  enc->maxRawSeen = 0;
}

// Blocca la calibrazione per un encoder specifico
void encoderLockCal(int motorNum) {
  EncoderData* enc = getEncoder(motorNum);
  if (!enc) return;
  
  if ((enc->maxRawSeen - enc->minRawSeen) > 8) {
    enc->minCal = enc->minRawSeen;
    enc->maxCal = enc->maxRawSeen;
    enc->calLocked = true;
  }
}

// Imposta lo zero del giunto per un encoder specifico
void encoderZero(int motorNum) {
  EncoderData* enc = getEncoder(motorNum);
  if (!enc) return;
  
  enc->jointZero = (enc->motorContDeg * enc->dirSign) / moltiplicaRapportoPlanetario;
}

// Resetta l'encoder a posizione iniziale (da chiamare dopo homing)
void encoderReset(int motorNum) {
  EncoderData* enc = getEncoder(motorNum);
  if (!enc) return;
  
  // Resetta i valori accumulati
  enc->motorContDeg = 0.0f;
  enc->emaMotorContDeg = 0.0f;
  enc->last0_360 = enc->angleDeg;  // usa l'angolo corrente come riferimento
  enc->jointZero = 0.0f;
  enc->jointDeg = 0.0f;
}

// Imposta il segno della direzione per un encoder specifico
void encoderSetDirSign(int motorNum, int sign) {
  EncoderData* enc = getEncoder(motorNum);
  if (!enc) return;
  
  enc->dirSign = (sign >= 0) ? +1 : -1;
}

// Aggiorna un singolo encoder
void updateSingleEncoder(EncoderData& enc) {
  const int raw = analogRead(enc.encoderPin);
  
  // Aggiorna limiti se NON lockato
  if (!enc.calLocked) {
    if (raw < enc.minRawSeen) enc.minRawSeen = raw;
    if (raw > enc.maxRawSeen) enc.maxRawSeen = raw;
  }

  // Seleziona limiti
  const int minUse = enc.calLocked ? enc.minCal : enc.minRawSeen;
  const int maxUse = enc.calLocked ? enc.maxCal : enc.maxRawSeen;
  const int span = maxUse - minUse;

  // Angolo motore 0..360 (senza EMA)
  float ang0_360;
  if (span > 8) {
    float norm = (raw - minUse) / (float)span;
    if (norm < 0) norm = 0;
    if (norm > 1) norm = 1;
    ang0_360 = norm * 360.0f;
  } else {
    ang0_360 = (raw / 1023.0f) * 360.0f;
  }

  // Unwrap + deadband
  if (enc.firstSample) {
    enc.last0_360 = ang0_360;
    enc.firstSample = false;
  }
  float delta = ang0_360 - enc.last0_360;
  if (delta > 180.0f) delta -= 360.0f;
  if (delta < -180.0f) delta += 360.0f;
  if (fabs(delta) < 0.25f) delta = 0.0f;
  enc.motorContDeg += delta;
  enc.last0_360 = ang0_360;

  // Applica EMA sul continuo
  enc.emaMotorContDeg = ALPHA * enc.motorContDeg + (1.0f - ALPHA) * enc.emaMotorContDeg;

  // Giunto = (motore filtrato * dir) / rapporto - zero
  float jointDegNow = (enc.emaMotorContDeg * enc.dirSign) / moltiplicaRapportoPlanetario - enc.jointZero;

  // Wrap 0..360
  while (jointDegNow < 0) jointDegNow += 360.0f;
  while (jointDegNow >= 360) jointDegNow -= 360.0f;
    
  // mantieni anche l'istantaneo motore 0..360
  enc.angleDeg = ang0_360;
  enc.jointDeg = jointDegNow;
}

void encoderUpdate() {
  // Aggiorna tutti e cinque gli encoder
  updateSingleEncoder(encoder1);
  updateSingleEncoder(encoder2);
  updateSingleEncoder(encoder3);
  updateSingleEncoder(encoder4);
  updateSingleEncoder(encoder5);
  
  // Copia i valori alle variabili globali esportate
  angleDeg1 = encoder1.angleDeg;
  angleDeg2 = encoder2.angleDeg;
  angleDeg3 = encoder3.angleDeg;
  angleDeg4 = encoder4.angleDeg;
  angleDeg5 = encoder5.angleDeg;
  
  jointDeg1 = encoder1.jointDeg;
  jointDeg2 = encoder2.jointDeg;
  jointDeg3 = encoder3.jointDeg;
  jointDeg4 = encoder4.jointDeg;
  jointDeg5 = encoder5.jointDeg;
  
  // Stream dei dati (opzionale, solo se abilitato)
  static uint32_t tStream = 0;
  const uint32_t now = millis();
  
  if (encoderStreamOn && (now - tStream >= encoderStreamPeriodMs)) {
    tStream = now;
    Serial.print(F("EN_DEG M1:"));
    Serial.print(encoder1.jointDeg, 2);
    Serial.print(F(" M2:"));
    Serial.print(encoder2.jointDeg, 2);
    Serial.print(F(" M3:"));
    Serial.print(encoder3.jointDeg, 2);
    Serial.print(F(" M4:"));
    Serial.print(encoder4.jointDeg, 2);
    Serial.print(F(" M5:"));
    Serial.println(encoder5.jointDeg, 2);
  }
}
