#include "Commands.h"
#include "Motors.h"
#include "Display.h"
#include "Config.h"
#include "RobotLink.h"

// Converte gradi in passi usando il passiPerGrado globale (double)
static inline int degToStepsClamped(int deg) {
  if (deg < -90) deg = -90;
  if (deg >  90) deg =  90;

  extern double passiPerGrado;           // definito in Config
  double stepsd = deg * passiPerGrado;   // conversione °→passi

  // arrotondamento al passo intero
  long steps = (stepsd >= 0.0) ? (long)(stepsd + 0.5) : (long)(stepsd - 0.5);

  // su AVR l'int è 16 bit: limita a [-32768, 32767]
  steps = constrain(steps, -32768L, 32767L);

  return (int)steps;
}


  
// --- Callback invocata dal radioLink.poll() ---
// Traduce il Payload in una singola riga di comandi separati da ';'
// riusando la tua pipeline parseCommandLine/ processToken.
void applyCommand(const Payload& p)
{
  String cmd; cmd.reserve(96);

  cmd += F("M1:"); cmd += degToStepsClamped(p.x); cmd += ';';  // X → M1
  cmd += F("M2:"); cmd += degToStepsClamped(p.y); cmd += ';';  // Y → M2
  cmd += F("M3:"); cmd += degToStepsClamped(p.z); cmd += ';';  // Z → M3
  cmd += F("M4:"); cmd += degToStepsClamped(p.a); cmd += ';';  // A → M4

  cmd += F("C:");  cmd += (p.c ? 1 : 0);           cmd += ';';
  if (p.save)      cmd += F("SAVE:1;");
  cmd += F("EXEC");

  parseCommandLine(cmd);

  Serial.print(F("[RF] ")); Serial.println(cmd);
}


// --- BTN6: HOMING completo (puoi cambiarlo in un preset) ---
void onBtn6()
{
  // Riutilizziamo il tuo comando alto livello
  parseCommandLine(F("HOMING"));
  // oppure, se preferisci solo preset: parseCommandLine("M1:0;M2:0;M3:0;M4:0;EXEC");
  Serial.println(F(">> BTN6 → HOMING"));
}

// --- BTN7: STOP software (replica il tuo handleButtons STOP) ---
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
    if (c == '\r')
      continue; // ignora CR (per CRLF da Windows)
    if (c == '\n')
    { // riga completa -> parse
      line.trim();
      if (line.length())
        parseCommandLine(line);
      line = ""; // reset buffer
    }
    else
    {
      line += c; // accumula
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

// --- Dispatch di un singolo token ---
void processToken(const String &cmd)
{
  // Debug opzionale:
  // Serial.print(F("TOK> ")); Serial.println(cmd);

  if (cmd == "ENA:1" || cmd == "UNLOCK")
  {
    motorsEnabled = true;
    setEnableAll(true);
    mostraMessaggio("ENA ON");
    return;
  }
  if (cmd == "ENA:0" || cmd == "LOCK")
  {
    motorsEnabled = false;
    setEnableAll(false);
    mostraMessaggio("ENA OFF");
    return;
  }
  if (cmd.startsWith("M1:"))
  {
    int v = cmd.substring(3).toInt();
    mostraMessaggio("M1:" + String(v));
    setTarget(motore1, v);
    motore1Completato = false;
    target1 = v;
    return;
  }
  if (cmd.startsWith("M2:"))
  {
    int v = cmd.substring(3).toInt();
    mostraMessaggio("M2:" + String(v));
    setTarget(motore2, v);
    motore2Completato = false;
    target2 = v;
    return;
  }
  if (cmd.startsWith("M3:"))
  {
    int v = cmd.substring(3).toInt();
    mostraMessaggio("M3:" + String(v));
    setTarget(motore3, v);
    motore3Completato = false;
    target3 = v;
    return;
  }
  if (cmd.startsWith("M4:"))
  {
    int v = cmd.substring(3).toInt();
    mostraMessaggio("M4:" + String(v));
    setTarget(motore4, v);
    motore4Completato = false;
    target4 = v;
    return;
  }
  if (cmd == "EXEC" || cmd == "RUN")
  {
    // Avvio movimento coordinato (se usi MultiStepper)
    //moveAllToDegrees(target1, target2, target3, target4);
    //multiActive = true;
    eseguiMovimento = true;
    mostraMessaggio("EXEC");
    return;
  }
  if (cmd.startsWith("C:"))
  {
    int stato = cmd.substring(2).toInt();
    calamitaAttiva = (stato == 1);
    digitalWrite(TRANSISTOR_PIN, stato ? HIGH : LOW);
    Serial.println(F("ok"));
    mostraMessaggio("C:" + String(stato));
    return;
  }
  if (cmd == "HOMING" || cmd == "HOME")
  {
    mostraMessaggio("HOMING...");
    homingMotor(motore1, ENDSTOP_1_PIN, -2000);
    homingMotor(motore2, ENDSTOP_2_PIN, -2000);
    homingMotor(motore3, ENDSTOP_3_PIN, -2000);
    homingMotor(motore4, ENDSTOP_4_PIN, -2000);
    target1 = target2 = target3 = target4 = 0;
    aggiornaDisplay();
    return;
  }
  if (cmd == "HOME_1")
  {
    mostraMessaggio("HOMING 1..."); 
    homingMotor(motore1, ENDSTOP_1_PIN, -2000); 
    target1 = 0; 
    Serial.println("HOMING COMPLETATO"); 
    aggiornaDisplay();
    return;
  } 
  if (cmd == "HOME_2")
  {
    mostraMessaggio("HOMING 2..."); 
    homingMotor(motore2, ENDSTOP_2_PIN, -2000); 
    target2 = 0; 
    Serial.println("HOMING COMPLETATO"); 
    aggiornaDisplay();
    return;
  }
  if (cmd == "HOME_3")
  {
    mostraMessaggio("HOMING 3..."); 
    homingMotor(motore3, ENDSTOP_3_PIN, -2000); 
    target3 = 0; 
    Serial.println("HOMING COMPLETATO"); 
    aggiornaDisplay();
    return;
  }
  if (cmd == "HOME_4")
  {
    mostraMessaggio("HOMING 4..."); 
    homingMotor(motore4, ENDSTOP_4_PIN, -2000); 
    target4 = 0; 
    Serial.println("HOMING COMPLETATO"); 
    aggiornaDisplay();
    return;
  }
  if (cmd.startsWith("CFG:"))
  {
    String clean = cmd.substring(4);
    setConfiguration(clean);
    Serial.println(F("ok"));
    return;
  }
  if (cmd == "ENDSTOP_ENABLED_1")
  {
    ENDSTOP_ENABLED = 1;
    return;
  }
  if (cmd == "ENDSTOP_ENABLED_0")
  {
    ENDSTOP_ENABLED = 0;
    return;
  }

  // token non riconosciuto (opzionale)
  // Serial.print(F("?? ")); Serial.println(cmd);
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
      homingMotor(motore1, ENDSTOP_1_PIN, -2000);
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
