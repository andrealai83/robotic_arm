#include "Encoder.h"
#include "Config.h"

float ema = 0.0f;        // filtro EMA
float angleDeg = 0.0f;   // istantaneo motore 0..360 (per compatibilità)

// --- calibrazione RAW ---
static int  minRawSeen = 1023, maxRawSeen = 0; // limiti osservati
static int  minCal     = 0,    maxCal     = 1023; // limiti "lockati"
static bool calLocked  = false;                  // true => usa minCal/maxCal

// --- unwrap e parametri giunto ---
static bool  firstSample   = true;
static float last0_360     = 0.0f;
static float motorContDeg  = 0.0f;   // motore continuo (può crescere senza limiti)
static int   dirSign       = -1;     // +1 o -1 (inverti verso se serve)
static float jointZero     = 0.0f;   // offset zero giunto (comando ENC_Z)

float jointDeg = 0.0f;

void encoderSetup() {
  ema = analogRead(ENCODER_PIN);
}

// Avvia una nuova calibrazione (sblocca e resetta min/max)
void encoderStartCal() {
  calLocked   = false;
  minRawSeen  = 1023;
  maxRawSeen  = 0;
}

// Blocca la calibrazione: salva i limiti osservati e li fissa
void encoderLockCal() {
  if ((maxRawSeen - minRawSeen) > 8) {
    minCal   = minRawSeen;
    maxCal   = maxRawSeen;
    calLocked = true;
  }
}

// Imposta lo zero del giunto sull’angolo attuale
void encoderZero() {
  jointZero = (motorContDeg * dirSign) / moltiplicaRapportoPlanetario;
}

// Imposta il segno della direzione (+1 / -1)
void encoderSetDirSign(int sign) {
  dirSign = (sign >= 0) ? +1 : -1;
}

// Variabili globali
float emaMotorContDeg = 0.0f; 

void encoderUpdate() {
  
  const int raw = analogRead(ENCODER_PIN);
  
  // Aggiorna limiti se NON lockato
  if (!calLocked) {
    if (raw < minRawSeen) minRawSeen = raw;
    if (raw > maxRawSeen) maxRawSeen = raw;
  }

  // Seleziona limiti
  const int minUse = calLocked ? minCal : minRawSeen;
  const int maxUse = calLocked ? maxCal : maxRawSeen;
  const int span = maxUse - minUse;

  // Angolo motore 0..360 (senza EMA)
  float ang0_360;
  if (span > 8) {
    float norm = (raw - minUse) / (float)span;  // Usa raw, non ema
    if (norm < 0) norm = 0;
    if (norm > 1) norm = 1;
    ang0_360 = norm * 360.0f;
  } else {
    ang0_360 = (raw / 1023.0f) * 360.0f;
  }

  // Unwrap + deadband
  if (firstSample) {
    last0_360 = ang0_360;
    firstSample = false;
  }
  float delta = ang0_360 - last0_360;
  if (delta > 180.0f) delta -= 360.0f;
  if (delta < -180.0f) delta += 360.0f;
  if (fabs(delta) < 0.25f) delta = 0.0f;
  motorContDeg += delta;
  last0_360 = ang0_360;

  // Applica EMA sul continuo (se vuoi filtering)
  emaMotorContDeg = ALPHA * motorContDeg + (1.0f - ALPHA) * emaMotorContDeg;

  // Giunto = (motore filtrato * dir) / rapporto - zero
  float jointDegNow = (emaMotorContDeg * dirSign) / moltiplicaRapportoPlanetario - jointZero;

  // Wrap 0..360
  while (jointDegNow < 0) jointDegNow += 360.0f;
  while (jointDegNow >= 360) jointDegNow -= 360.0f;
    
  // mantieni anche l’istantaneo motore 0..360 se serve altrove
  angleDeg = ang0_360;
 
  static uint32_t tStream = 0;
  const uint32_t now = millis();

  jointDeg = jointDegNow;
  
  if (encoderStreamOn && (now - tStream >= encoderStreamPeriodMs)) {
    tStream = now;
    Serial.print(F("EN_DEG_M1:"));
    Serial.println(jointDegNow, 2); 
  }
}
