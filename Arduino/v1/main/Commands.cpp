#include "Commands.h"
#include "Motors.h"
#include "Display.h"
#include "Config.h"
#include "RobotLink.h"
#include "Encoder.h"
#include <Arduino.h>
  
// ========= PARAMETRI VELOCITÀ =========
// gain: quanti passi/secondo si vogliono a m=100
// ===== Tuning =====
static const int   VEL_DEADBAND = 2;        // velocità assoluta <= 2 => 0
static float GAIN_STEPS_PER_S_100_M1 = 2000; // passi/s a m=100
static float GAIN_STEPS_PER_S_100_M2 = 2000;
static float GAIN_STEPS_PER_S_100_M3 = 2000;
static float GAIN_STEPS_PER_S_100_M4 = 2000;

// limiti meccanici (in passi) 
static long LIM_MIN_M1 = 0, LIM_MAX_M1 = 360;
static long LIM_MIN_M2 = 0, LIM_MAX_M2 = 360;
static long LIM_MIN_M3 = 0, LIM_MAX_M3 = 360;
static long LIM_MIN_M4 = 0, LIM_MAX_M4 = 360;

// fail-safe: se non riceviamo pacchetti per X ms, stop ai delta
static const unsigned long RF_TIMEOUT_MS = 500;

// ========= STATO VELOCITÀ =========
static volatile int16_t v1 = 0, v2 = 0, v3 = 0, v4 = 0; // [-100..100]
static unsigned long lastVelUpdateMs = 0;  // per integrazione DT
static unsigned long lastRfMs = 0;         // watchdog RF

#define ENABLE_MOVE_MSG      0       
#define MOVE_MSG_PERIOD_MS   150

// extern long target1, target2, target3, target4; // se servisse
static inline int16_t clamp16(long v){
  if (v < -32768L) return -32768;
  if (v >  32767L) return  32767;
  return (int16_t)v;
}

static inline long clampL(long v, long lo, long hi){
  if (v < lo) return lo;
  if (v > hi) return hi;
  return v;
}

static inline int16_t withDeadband(int16_t v){
  return (abs(v) <= VEL_DEADBAND) ? 0 : v;
}

// === ARRIVO PACCHETTO RF ===
void applyCommand(const Payload& p)
{
  // Solo aggiornamento velocità + stato calamita/save
  v1 = withDeadband(p.m1);
  v2 = withDeadband(p.m2);
  v3 = withDeadband(p.m3);
  v4 = withDeadband(p.m4);
  lastRfMs = millis();

  // Calamita (toggle lato Mega opzionale)
  if (p.c) {
    calamitaAttiva = !calamitaAttiva;
    digitalWrite(TRANSISTOR_PIN, calamitaAttiva ? HIGH : LOW);
    mostraMessaggio(String("C:") + (calamitaAttiva?1:0));
  }
  if (p.save) {
    mostraMessaggio("SAVE");
  }
}

static float r1=0, r2=0, r3=0, r4=0;  
static bool endstopTelemetryEnabled = false;

void joystickVelocityUpdate() {

  unsigned long now = millis();

  // timeout radio ⇒ vel 0
  if (now - lastRfMs > RF_TIMEOUT_MS) v1 = v2 = v3 = v4 = 0;

  static bool first = true;
  if (first) { first=false; lastVelUpdateMs=now; return; }

  unsigned long dms = now - lastVelUpdateMs;
  lastVelUpdateMs = now;
  if (dms == 0) return;

  if (dms < 2)  dms = 2;
  if (dms > 50) dms = 50;
  const float dt = dms / 1000.0f;

  if (!v1 && !v2 && !v3 && !v4) {
    eseguiMovimento = false;
    return;
  }

  // delta float
  float d1 = (v1/100.0f) * GAIN_STEPS_PER_S_100_M1 * dt + r1;
  float d2 = (v2/100.0f) * GAIN_STEPS_PER_S_100_M2 * dt + r2;
  float d3 = (v3/100.0f) * GAIN_STEPS_PER_S_100_M3 * dt + r3;
  float d4 = (v4/100.0f) * GAIN_STEPS_PER_S_100_M4 * dt + r4;

  // estrai parte intera, conserva residuo
  long s1 = (long) lroundf(d1); r1 = d1 - s1;
  long s2 = (long) lroundf(d2); r2 = d2 - s2;
  long s3 = (long) lroundf(d3); r3 = d3 - s3;
  long s4 = (long) lroundf(d4); r4 = d4 - s4;

  bool changed = false;

  if (s1) { long nt = clampL(target1 + s1, LIM_MIN_M1, LIM_MAX_M1);
            if (nt != target1) { setTarget(motore1, nt); motore1Completato=false; target1=nt; changed=true; } }
  if (s2) { long nt = clampL(target2 + s2, LIM_MIN_M2, LIM_MAX_M2);
            if (nt != target2) { setTarget(motore2, nt); motore2Completato=false; target2=nt; changed=true; } }
  if (s3) { long nt = clampL(target3 + s3, LIM_MIN_M3, LIM_MAX_M3);
            if (nt != target3) { setTarget(motore3, nt); motore3Completato=false; target3=nt; changed=true; } }
  if (s4) { long nt = clampL(target4 + s4, LIM_MIN_M4, LIM_MAX_M4);
            if (nt != target4) { setTarget(motore4, nt); motore4Completato=false; target4=nt; changed=true; } }

  eseguiMovimento = changed;
}

// --- BTN6: HOMING completo  ---
void onBtn6()
{
  parseCommandLine(F("HOMING"));
  Serial.println(F(">> BTN6 → HOMING"));
}

// --- BTN7: STOP software ---
void onBtn7()
{
  extern bool eseguiMovimento;
  extern bool motore1Completato, motore2Completato, motore3Completato, motore4Completato;
  extern AccelStepper motore1, motore2, motore3, motore4;

  motore1.stop(); motore1.setCurrentPosition(motore1.currentPosition()); motore1Completato = true;
  motore2.stop(); motore2.setCurrentPosition(motore2.currentPosition()); motore2Completato = true;
  motore3.stop(); motore3.setCurrentPosition(motore3.currentPosition()); motore3Completato = true;
  motore4.stop(); motore4.setCurrentPosition(motore4.currentPosition()); motore4Completato = true;

  eseguiMovimento = false;

  mostraMessaggio("STOP");
  Serial.println(F(">> BTN7 → STOP"));
}

void handleSerial()
{
  static String line;
  while (Serial.available())
  {
    char c = (char)Serial.read();
    if (c == '\r') continue; // Ignora carriage return
    if (c == '\n')
    {
      line.trim();
      if (line.length())
        parseCommandLine(line);
      line = "";
    }
    else
    {
      line += c;
    }
  }
}

// --- Spezza per ';' e processa ogni token ---
void parseCommandLine(String s)
{ 
  int start = 0;
  while (true)
  {
    int sep = s.indexOf(';', start);
    String token = (sep == -1) ? s.substring(start) : s.substring(start, sep);
    token.trim();
    if (token.length())
      processToken(token);
    if (sep == -1)
      break;
    start = sep + 1;
  }
}
 
static void showMoveMsg(const char* label, int v) {
  #if ENABLE_MOVE_MSG
    static unsigned long last=0;
    if (millis() - last < MOVE_MSG_PERIOD_MS) return;
    last = millis();
    char msg[24];
    snprintf(msg, sizeof(msg), "%s%d", label, v);
    mostraMessaggio(msg);
  #endif
}

void processToken(const String &cmd)
{
  if (cmd == F("ENA:1") || cmd == F("UNLOCK")) {
    motorsEnabled = true;
    setEnableAll(true);
    mostraMessaggio("ENA ON");
    return;
  }
  if (cmd == F("ENA:0") || cmd == F("LOCK")) {
    motorsEnabled = false;
    setEnableAll(false);
    mostraMessaggio("ENA OFF");
    return;
  }

  if (cmd.startsWith(F("M1:"))) { 
    int v = atoi(cmd.c_str() + 3);
    showMoveMsg("M1:", v);
    target1 = v;
    setTarget(motore1, v);
    motore1Completato = false;
    return;
  }
  if (cmd.startsWith(F("M2:"))) {
    int v = atoi(cmd.c_str() + 3);
    showMoveMsg("M2:", v);
    target2 = v;
    setTarget(motore2, v);
    motore2Completato = false;
    return;
  }
  if (cmd.startsWith(F("M3:"))) {
    int v = atoi(cmd.c_str() + 3);
    showMoveMsg("M3:", v);
    target3 = v;
    setTarget(motore3, v);
    motore3Completato = false;
    return;
  }
  if (cmd.startsWith(F("M4:"))) {
    int v = atoi(cmd.c_str() + 3);
    showMoveMsg("M4:", v);
    target4 = v;
    setTarget(motore4, v);
    motore4Completato = false;
    return;
  }
  if (cmd.startsWith(F("GRIP:"))) {
    int angle = atoi(cmd.c_str() + 5);
    setGripperAngle(angle);
    mostraMessaggio("GRIP:" + String(angle));
    return;
  }

  if (cmd == F("EXEC") || cmd == F("RUN")) {
    // Prima di eseguire, sincronizza i target con le posizioni correnti
    // Questo previene movimenti indesiderati di assi non comandati esplicitamente
    // (es. se fai solo M1:90, M2/M3/M4 non devono muoversi)
        
    startCoordinatedMove();
    mostraMessaggio("EXEC");
    return;
  }

  if (cmd.startsWith(F("C:"))) {
    int stato = atoi(cmd.c_str() + 2);
    calamitaAttiva = (stato == 1);
    digitalWrite(TRANSISTOR_PIN, calamitaAttiva ? HIGH : LOW);
    Serial.println(F("ok"));
    char msg[12]; snprintf(msg, sizeof(msg), "C:%d", stato);
    mostraMessaggio(msg);
    return;
  }

  if (cmd == F("HOMING") || cmd == F("HOME")) {
    mostraMessaggio("HOMING...");
    homingMotor(motore1, ENDSTOP_1_PIN, 0);
    homingMotor(motore2, ENDSTOP_2_PIN, 1);
    homingMotor(motore3, ENDSTOP_3_PIN, 2);
    homingMotor(motore4, ENDSTOP_4_PIN, 3);
    // Resetta gli encoder alla posizione di home
    encoderReset(1);
    encoderReset(2);
    encoderReset(3);
    encoderReset(4);
    target1 = target2 = target3 = target4 = 0;
    aggiornaDisplay();
    return;
  }

  if (cmd == F("HOME_1")) {
    mostraMessaggio("HOMING 1...");
    homingMotor(motore1, ENDSTOP_1_PIN, 0);
    encoderReset(1);  // resetta encoder dopo homing
    target1 = 0;
    Serial.println(F("HOMING COMPLETATO"));
    aggiornaDisplay();
    return;
  }
  if (cmd == F("HOME_2")) {
    mostraMessaggio("HOMING 2...");
    homingMotor(motore2, ENDSTOP_2_PIN, 1);
    encoderReset(2);  // resetta encoder dopo homing
    target2 = 0;
    Serial.println(F("HOMING COMPLETATO"));
    aggiornaDisplay();
    return;
  }
  if (cmd == F("HOME_3")) {
    mostraMessaggio("HOMING 3...");
    homingMotor(motore3, ENDSTOP_3_PIN, 2);
    encoderReset(3);  // resetta encoder dopo homing
    target3 = 0;
    Serial.println(F("HOMING COMPLETATO"));
    aggiornaDisplay();
    return;
  }
  if (cmd == F("HOME_4")) {
    mostraMessaggio("HOMING 4...");
    homingMotor(motore4, ENDSTOP_4_PIN, 3);
    encoderReset(4);  // resetta encoder dopo homing
    target4 = 0;
    Serial.println(F("HOMING COMPLETATO"));
    aggiornaDisplay();
    return;
  }

  if (cmd.startsWith(F("CFG:"))) {
    String clean = cmd.substring(4);
    setConfiguration(clean);
    Serial.println(F("ok"));
    return;
  }

  if (cmd == F("STREAM:1")) {
    encoderStreamOn = true;
    return;
  }

  if (cmd == F("STREAM:0")) {
    encoderStreamOn = false;
    return;
  }

  if (cmd == F("ENDSTOP_ENABLED_1")) { ENDSTOP_ENABLED = 1; return; }
  if (cmd == F("ENDSTOP_ENABLED_0")) { ENDSTOP_ENABLED = 0; return; }
  if (cmd == F("TE:1")) { endstopTelemetryEnabled = true; return; }
  if (cmd == F("TE:0")) { endstopTelemetryEnabled = false; return; }

}

void handleButtons()
{
  static unsigned long lastButtonTime = 0;
  if (millis() - lastButtonTime > 300)
  {
    if (digitalRead(BTN_HOMING_PIN) == LOW)
    {
      Serial.println(F("BTN: HOMING"));
      mostraMessaggio("BTN: HOMING");
      String cmd = "HOMING";
      handleSerial(); // process pending serial first
      homingMotor(motore1, ENDSTOP_1_PIN, 0);
      lastButtonTime = millis();
    }
    if (digitalRead(BTN_STOP_PIN) == LOW)
    {
      Serial.println(F("BTN: STOP"));
      mostraMessaggio("BTN: STOP");
      motore1.stop();
      motore1.setCurrentPosition(motore1.currentPosition());
      motore1Completato = true;
      motore2.stop();
      motore2.setCurrentPosition(motore2.currentPosition());
      motore2Completato = true;
      motore3.stop();
      motore3.setCurrentPosition(motore3.currentPosition());
      motore3Completato = true;
      motore4.stop();
      motore4.setCurrentPosition(motore4.currentPosition());
      motore4Completato = true;
      eseguiMovimento = false;
      lastButtonTime = millis();
    }
  }
}

void checkEndStop() {

  String ENDSTOP_1_STATE = digitalRead(ENDSTOP_1_PIN) == LOW ? "LOW" : "HIGH";
  String ENDSTOP_2_STATE = digitalRead(ENDSTOP_2_PIN) == LOW ? "LOW" : "HIGH";
  String ENDSTOP_3_STATE = digitalRead(ENDSTOP_3_PIN) == LOW ? "LOW" : "HIGH";
  String ENDSTOP_4_STATE = digitalRead(ENDSTOP_4_PIN) == LOW ? "LOW" : "HIGH";
 
 if ( ENDSTOP_1_STATE == "LOW")
  {  
    motore1.stop();
    motore1.setCurrentPosition(0); 
    Serial.println(F("Finecorsa raggiunto!")); 
    // backoff
    int gradiBackoff = 5;
    long passiBackoff = (long)(gradiBackoff * passiPerGrado);
    Serial.println(F("Rilascio finecorsa..."));
    motore1.moveTo(passiBackoff);
    while (motore1.distanceToGo() != 0) motore1.run();
    motore1.setCurrentPosition(0);
    Serial.println(F("Backoff completato"));
    return;
  }

   if ( ENDSTOP_2_STATE == "LOW")
  {
    motore2.stop();
    motore2.setCurrentPosition(0); 
    Serial.println(F("Finecorsa raggiunto!")); 
    // backoff
    int gradiBackoff = 5;
    long passiBackoff = (long)(gradiBackoff * passiPerGrado);
    Serial.println(F("Rilascio finecorsa..."));
    motore2.moveTo(passiBackoff);
    while (motore2.distanceToGo() != 0) motore2.run();
    motore2.setCurrentPosition(0);
    Serial.println(F("Backoff completato"));
    return;
  }

  
   if ( ENDSTOP_3_STATE == "LOW")
  {
    motore3.stop();
    motore3.setCurrentPosition(0); 
    Serial.println(F("Finecorsa raggiunto!")); 
    // backoff
    int gradiBackoff = 5;
    long passiBackoff = (long)(gradiBackoff * passiPerGrado);
    Serial.println(F("Rilascio finecorsa..."));
    motore3.moveTo(passiBackoff);
    while (motore3.distanceToGo() != 0) motore3.run();
    motore3.setCurrentPosition(0);
    Serial.println(F("Backoff completato"));
    return;
  }


   if ( ENDSTOP_4_STATE == "LOW")
  {
    motore4.stop();
    motore4.setCurrentPosition(0); 
    Serial.println(F("Finecorsa raggiunto!")); 
    // backoff
    int gradiBackoff = 5;
    long passiBackoff = (long)(gradiBackoff * passiPerGrado);
    Serial.println(F("Rilascio finecorsa..."));
    motore4.moveTo(passiBackoff);
    while (motore4.distanceToGo() != 0) motore4.run();
    motore4.setCurrentPosition(0);
    Serial.println(F("Backoff completato"));
    return;
  }
  
}

void endstopTelemetryUpdate() {
  if (!endstopTelemetryEnabled) return;

  static unsigned long lastMs = 0;
  const unsigned long intervalMs = 200;
  unsigned long now = millis();
  if (now - lastMs < intervalMs) return;
  lastMs = now;

  bool e1 = (digitalRead(ENDSTOP_1_PIN) == LOW);
  bool e2 = (digitalRead(ENDSTOP_2_PIN) == LOW);
  bool e3 = (digitalRead(ENDSTOP_3_PIN) == LOW);
  bool e4 = (digitalRead(ENDSTOP_4_PIN) == LOW);

  mostraStatoEndstop(e1, e2, e3, e4);
  Serial.print(F("ENDSTOP "));
  Serial.print(F("E1:")); Serial.print(e1 ? 1 : 0);
  Serial.print(F(" E2:")); Serial.print(e2 ? 1 : 0);
  Serial.print(F(" E3:")); Serial.print(e3 ? 1 : 0);
  Serial.print(F(" E4:")); Serial.println(e4 ? 1 : 0);
}

int parseTarget(const String &comando)
{
  String numero = comando.substring(3);
  if (numero.length() == 0)
  {
    Serial.println(F("Errore: comando vuoto"));
    return 0;
  }
  return numero.toInt();
}

void setConfiguration(const String &config)
{
  Serial.println(">>> setConfiguration ricevuto: [" + config + "]");

  int sepIndex = config.indexOf(":");
  if (sepIndex == -1)
  {
    Serial.println(F("⚠️ Errore: formato parametro non valido"));
    return;
  }

  String param = config.substring(0, sepIndex);
  String value = config.substring(sepIndex + 1);
  int intValue = value.toInt();

  Serial.println("Parametro: [" + param + "] Valore: [" + value + "]");

  if (param == "passiPerGiro")
  {
    passiPerGiro = intValue;
    Serial.println("✅ passiPerGiro aggiornato: " + String(passiPerGiro));
  }
  else if (param == "microstep")
  {
    microstep = intValue;
    Serial.println("✅ microstep aggiornato: " + String(microstep));
  }
  else if (param == "maxSpeed")
  {
    maxSpeed = intValue;
    motore1.setMaxSpeed(maxSpeed);
    motore2.setMaxSpeed(maxSpeed);
    motore3.setMaxSpeed(maxSpeed);
    motore4.setMaxSpeed(maxSpeed);
    Serial.println("✅ maxSpeed aggiornato: " + String(maxSpeed));
  }
  else if (param == "maxAccel")
  {
    maxAccel = intValue;
    motore1.setAcceleration(maxAccel);
    motore2.setAcceleration(maxAccel);
    motore3.setAcceleration(maxAccel);
    motore4.setAcceleration(maxAccel);
    Serial.println("✅ maxAccel aggiornato: " + String(maxAccel));
  }
  else if (param == "gear")
  {
    rapportoPlanetario = value.toFloat();
    Serial.println("✅ rapportoPlanetario aggiornato: " + String(rapportoPlanetario, 3));
  }
  else
  {
    Serial.println("⚠️ Parametro sconosciuto: " + param);
  }

  recalcPassiPerGrado();
  Serial.println("🔁 Calcolo passiPerGrado: " + String(passiPerGrado));
}

void fillAck(AckPayload& out) {
  extern AccelStepper motore1, motore2, motore3, motore4;
  extern bool calamitaAttiva;
  extern bool motorsEnabled;

  out.p1 = clamp16(motore1.currentPosition());
  out.p2 = clamp16(motore2.currentPosition());
  out.p3 = clamp16(motore3.currentPosition());
  out.p4 = clamp16(motore4.currentPosition());
  out.magnet  = calamitaAttiva ? 1 : 0;
  out.enabled = motorsEnabled   ? 1 : 0;
}
