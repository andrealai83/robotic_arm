#include <AccelStepper.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

// Imposta l'indirizzo I2C (di solito 0x27 o 0x3F)
LiquidCrystal_I2C lcd(0x27, 16, 2);

// Definizione dei pin per i motori (inizialmente impostati come default)
int ENA1 = 13, DIR1 = 12, PUL1 = 11;
int ENA2 = 10, DIR2 = 9, PUL2 = 8;
int ENA3 = 7, DIR3 = 6, PUL3 = 5;
int ENA4 = 4, DIR4 = 3, PUL4 = 2;
int transistorPin = A0;

int ENDSTOP_ENABLED = 1;

#define ENDSTOP_X_PIN A3
#define ENDSTOP_Y_PIN A2
#define ENDSTOP_Z_PIN A5
#define ENDSTOP_A_PIN A4

#define BT_STATE_PIN A1

#define BTN_HOMING_PIN A6
#define BTN_STOP_PIN   A7

// Parametri motore (inizialmente impostati come default)
int passiPerGiro = 3000;
int microstep = 8;
double passiPerGrado = (double)(passiPerGiro * microstep) / 360.0;
int maxSpeed = 10000;
int maxAccel = 10000;

// Istanziazione dei motori
AccelStepper motore1(AccelStepper::DRIVER, PUL1, DIR1);
AccelStepper motore2(AccelStepper::DRIVER, PUL2, DIR2);
AccelStepper motore3(AccelStepper::DRIVER, PUL3, DIR3);
AccelStepper motore4(AccelStepper::DRIVER, PUL4, DIR4);

// Variabili di stato
bool motore1Completato = true;
bool motore2Completato = true;
bool motore3Completato = true;
bool motore4Completato = true;

bool saveRequest = false;

int target1 = 0, target2 = 0, target3 = 0, target4 = 0;
bool calamitaAttiva = false;

int xTargetTemp, yTargetTemp, zTargetTemp, aTargetTemp;
bool eseguiMovimento = false;

byte bluetoothIcon[8] = {
  B00000,
  B00100,
  B01010,
  B00100,
  B01010,
  B00100,
  B00000,
  B00000
};

void setup() {

  Serial.begin(115200);
  Serial1.begin(9600);

  pinMode(BT_STATE_PIN, INPUT);
  pinMode(transistorPin, OUTPUT);

  // Configura i motori con le impostazioni iniziali
  setupMotor(motore1, ENA1);
  setupMotor(motore2, ENA2);
  setupMotor(motore3, ENA3);
  setupMotor(motore4, ENA4);

  pinMode(ENDSTOP_X_PIN, INPUT_PULLUP);
  pinMode(ENDSTOP_Y_PIN, INPUT_PULLUP);
  pinMode(ENDSTOP_Z_PIN, INPUT_PULLUP);
  pinMode(ENDSTOP_A_PIN, INPUT_PULLUP);

  pinMode(BTN_HOMING_PIN, INPUT_PULLUP);
  pinMode(BTN_STOP_PIN, INPUT_PULLUP);

  // Inizializza il display
  lcd.init();
  lcd.createChar(0, bluetoothIcon);
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Motori Pronti");
  delay(1000);
  lcd.clear();
  aggiornaDisplay();
}

bool bluetoothConnesso() {
  return digitalRead(BT_STATE_PIN) == HIGH;
}

// Aggiorna le informazioni sul display
void aggiornaDisplay() {
  lcd.setCursor(0, 0);
  lcd.print("X:");
  lcd.print(target1);
  lcd.print(" Y:");
  lcd.print(target2);

  lcd.setCursor(0, 1);
  lcd.print("Z:");
  lcd.print(target3);
  lcd.print(" A:");
  lcd.print(target4);

  lcd.setCursor(15, 1);
  if (bluetoothConnesso()) {
    lcd.write((byte)0);  // mostra icona Bluetooth
  } else {
    lcd.print(" "); // spazio vuoto se non connesso
  }

  // Stato calamita in alto a destra
  lcd.setCursor(12, 0);
  lcd.print(calamitaAttiva ? "ON " : "OFF");
}

// Mostra un messaggio temporaneo sul display
void mostraMessaggio(String messaggio) {
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("CMD: ");
  lcd.print(messaggio);
  //delay(1000); // Mostra il messaggio per 1 secondo
  lcd.clear();
  aggiornaDisplay();  // Torna a mostrare lo stato dei motori
}

void loop() {

  String comando = "";

  if (Serial1.available()) {

    comando = Serial1.readStringUntil('\n');
    comando.trim();
    Serial.println("Comand Received:" + comando);
    ComandReceived(comando);
  }

  if (Serial.available()) {
    comando = Serial.readStringUntil('\n');
    comando.trim();
    ComandReceived(comando);
  }

  // Loop continuo per eseguire i movimenti simultanei
  checkMotor(motore1, motore1Completato, ENDSTOP_X_PIN);
  checkMotor(motore2, motore2Completato, ENDSTOP_Y_PIN);
  checkMotor(motore3, motore3Completato, ENDSTOP_Z_PIN);
  checkMotor(motore4, motore4Completato, ENDSTOP_A_PIN);

  // Quando tutti i motori hanno finito
  if (eseguiMovimento && motore1Completato && motore2Completato && motore3Completato && motore4Completato) {
    Serial.println("ready");

    String MagnetState = calamitaAttiva ? ";C:1" : ";C:0";
    String SaveRequestReceived = saveRequest ? ";SAVE:1" : ";SAVE:0";
    String posizione = "NEWPOSITION:X:" + String(target1) + ";Y:" + String(target2) + ";Z:" + String(target3) + ";A:" + String(target4) + MagnetState + SaveRequestReceived;
    saveRequest = 0;
    Serial.println(posizione);
    Serial1.println("ready");
    eseguiMovimento = false;
    return;
  }

  static unsigned long lastButtonTime = 0;
  if (millis() - lastButtonTime > 300) {
    if (digitalRead(BTN_HOMING_PIN) == LOW) {
      Serial.println("BTN: HOMING");
      mostraMessaggio("BTN: HOMING");
      ComandReceived("HOMING");
      lastButtonTime = millis();
    }

    if (digitalRead(BTN_STOP_PIN) == LOW) {
      Serial.println("BTN: STOP");
      mostraMessaggio("BTN: STOP");

      // Ferma i motori e annulla i movimenti
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

  //aggiornaDisplay();
}

// Configura un motore
void setupMotor(AccelStepper& motore, int pinENA) {
  pinMode(pinENA, OUTPUT);
  digitalWrite(pinENA, HIGH);  // Abilita il motore

  motore.setMaxSpeed(maxSpeed);
  motore.setAcceleration(maxAccel);
}

// Imposta il target in gradi
void setTarget(AccelStepper& motore, int targetGradi) {
  if (targetGradi < -360 || targetGradi > 360) {
    Serial.println("Errore: target fuori limite (-360 a 360 gradi)");
    return;
  }

  long targetPassi = (long)(targetGradi * passiPerGrado);
  motore.moveTo(targetPassi);
  Serial.print("Impostato target: ");
  Serial.println(targetPassi);
}

// Verifica se il motore ha raggiunto il target
void checkMotor(AccelStepper& motore, bool& completato, int endstopPin) {
  if (!completato) {

    if (ENDSTOP_ENABLED == 1 &&  digitalRead(endstopPin) == LOW) {
      motore.stop();          // ferma la velocità
      motore.setCurrentPosition(0); // imposta la posizione attuale come zero
      completato = true;
      Serial.println("Finecorsa raggiunto!");

      // ⚙️ Backoff per rilasciare l'endstop
      int gradiBackoff = 5;
      long passiBackoff = gradiBackoff * passiPerGrado;

      Serial.println("Rilascio finecorsa...");
      motore.moveTo(passiBackoff);
      while (motore.distanceToGo() != 0) {
        motore.run();
      }

      motore.setCurrentPosition(0); // Riazzera dopo il backoff
      Serial.println("Backoff completato");

      return;
    }

    motore.run();
    if (motore.distanceToGo() == 0) {
      completato = true;
      delay(10);
    }
  }
}

void ComandReceived(String comando) {

  Serial.println(">> COMANDO RICEVUTO: " + comando);

  if (comando.startsWith("X:")) {
    xTargetTemp = parseTarget(comando);
    mostraMessaggio("X:" + String(xTargetTemp));
    setTarget(motore1, xTargetTemp);
    motore1Completato = false;
    target1 = xTargetTemp;
  }
  else if (comando.startsWith("SAVE:")) {
    saveRequest = true;
  }
  else if (comando.startsWith("ENDSTOP_ENABLED_1")) {
    ENDSTOP_ENABLED = 1;
  }
  else if (comando.startsWith("ENDSTOP_ENABLED_0")) {
    ENDSTOP_ENABLED = 0;
  }
  else if (comando == "HOME" || comando == "HOMING") {
    mostraMessaggio("HOMING...");
    homingMotor(motore1, ENDSTOP_X_PIN, -2000);
    homingMotor(motore2, ENDSTOP_Y_PIN, -2000);
    homingMotor(motore3, ENDSTOP_Z_PIN, -2000);
    homingMotor(motore4, ENDSTOP_A_PIN, -2000);
    target1 = target2 = target3 = target4 = 0;
    Serial.println("HOMING COMPLETATO");
    aggiornaDisplay();
  }
  else if (comando == "HOME_X") {
    mostraMessaggio("HOMING X...");
    homingMotor(motore1, ENDSTOP_X_PIN, -2000);
    target1 = 0;
    Serial.println("HOMING COMPLETATO");
    aggiornaDisplay();
  }
  else if (comando == "HOME_Y") {
    mostraMessaggio("HOMING Y...");
    homingMotor(motore2, ENDSTOP_Y_PIN, -2000);
    target2 = 0;
    Serial.println("HOMING COMPLETATO");
    aggiornaDisplay();
  }
  else if (comando == "HOME_Z") {
    mostraMessaggio("HOMING Z...");
    homingMotor(motore3, ENDSTOP_Z_PIN, -2000);
    target3 = 0;
    Serial.println("HOMING COMPLETATO");
    aggiornaDisplay();
  }
  else if (comando == "HOME_A") {
    mostraMessaggio("HOMING A...");
    homingMotor(motore4, ENDSTOP_A_PIN, -2000);
    target3 = 0;
    Serial.println("HOMING COMPLETATO");
    aggiornaDisplay();
  }
  else if (comando.startsWith("EMERGENCY_STOP")) {
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
  }
  else if (comando.startsWith("Y:")) {
    yTargetTemp = parseTarget(comando);
    mostraMessaggio("Y:" + String(yTargetTemp));
    setTarget(motore2, yTargetTemp);
    motore2Completato = false;
    target2 = yTargetTemp;
  } else if (comando.startsWith("Z:")) {
    zTargetTemp = parseTarget(comando);
    mostraMessaggio("Z:" + String(zTargetTemp));
    setTarget(motore3, zTargetTemp);
    motore3Completato = false;
    target3 = zTargetTemp;
  } else if (comando.startsWith("A:")) {
    aTargetTemp = parseTarget(comando);
    mostraMessaggio("A:" + String(aTargetTemp));
    setTarget(motore4, aTargetTemp);
    motore4Completato = false;
    target4 = aTargetTemp;
  } else if (comando == "RUN" || comando == "EXEC") {
    // Esegui i movimenti in parallelo
    eseguiMovimento = true;
    mostraMessaggio(comando);
  } else if (comando.startsWith("C:")) {
    int stato = parseTarget(comando);
    calamitaAttiva = (stato == 1);
    digitalWrite(transistorPin, stato == 1 ? HIGH : LOW);
    Serial.println("ok");
    mostraMessaggio("C:" + String(stato));
  } else if (comando.indexOf("CFG:") >= 0) {
    String cleanCommand = comando.substring(comando.indexOf("CFG:") + 4);
    setConfiguration(cleanCommand);
    Serial.println("ok");
  }
}


// Estrae il target dal comando
int parseTarget(String comando) {
  int target = 0;
  String numero = comando.substring(2);

  if (numero.length() == 0) {
    Serial.println("Errore: comando vuoto");
    return 0;
  }

  target = numero.toInt();
  return target;
}

// Configura i parametri dei motori
void setConfiguration(String config) {
  Serial.println(">>> setConfiguration ricevuto: [" + config + "]");

  int sepIndex = config.indexOf(":");
  if (sepIndex == -1) {
    Serial.println("⚠️ Errore: formato parametro non valido");
    return;
  }

  String param = config.substring(0, sepIndex);
  String value = config.substring(sepIndex + 1);
  int intValue = value.toInt();

  Serial.println("Parametro: [" + param + "] Valore: [" + value + "]");

  if (param == "passiPerGiro") {
    passiPerGiro = intValue;
    Serial.println("✅ passiPerGiro aggiornato: " + String(passiPerGiro));
  } else if (param == "microstep") {
    microstep = intValue;
    Serial.println("✅ microstep aggiornato: " + String(microstep));
  } else if (param == "maxSpeed") {
    maxSpeed = intValue;
    motore1.setMaxSpeed(maxSpeed);
    motore2.setMaxSpeed(maxSpeed);
    motore3.setMaxSpeed(maxSpeed);
    motore4.setMaxSpeed(maxSpeed);
    Serial.println("✅ maxSpeed aggiornato: " + String(maxSpeed));
  } else if (param == "maxAccel") {
    maxAccel = intValue;
    motore1.setAcceleration(maxAccel);
    motore2.setAcceleration(maxAccel);
    motore3.setAcceleration(maxAccel);
    motore4.setAcceleration(maxAccel);
    Serial.println("✅ maxAccel aggiornato: " + String(maxAccel));
  } else {
    Serial.println("⚠️ Parametro sconosciuto: " + param);
  }

  passiPerGrado = (double)(passiPerGiro * microstep) / 360.0;
  Serial.println("🔁 Calcolo passiPerGrado: " + String(passiPerGrado));
}


void homingMotor(AccelStepper& motore, int endstopPin, int velocitaNegativa) {

  if (ENDSTOP_ENABLED == 0) {
    Serial.println("Endstop disabilitato: homing saltato.");
    lcd.setCursor(0, 0);
    lcd.print("Homing disabilitato");
    delay(1000);
    lcd.clear();
    aggiornaDisplay();
    return;
  }

  Serial.print("Inizio homing pin ");
  lcd.setCursor(0, 0);
  lcd.print("Homing in corso...        ");
  Serial.println(endstopPin);

  // 🔧 Salva configurazione corrente
  float prevMaxSpeed = motore.maxSpeed();
  float prevAcceleration = motore.acceleration();

  motore.enableOutputs();
  motore.setMaxSpeed(abs(velocitaNegativa));
  motore.setAcceleration(500); // opzionale: ometti se non vuoi toccarla
  motore.setSpeed(velocitaNegativa);

  unsigned long startTime = millis();

  while (digitalRead(endstopPin) == HIGH && millis() - startTime < 10000) {
    motore.runSpeed();
  }

  if (digitalRead(endstopPin) == HIGH) {
    Serial.println("⚠️ Finecorsa non raggiunto entro timeout!");
  }

  motore.stop();
  motore.setCurrentPosition(0);
  Serial.println("Homing completato");

  // ⚙️ Backoff
  int gradiBackoff = 5;
  long passiBackoff = gradiBackoff * passiPerGrado;

  Serial.print("Ritorno indietro di ");
  Serial.print(gradiBackoff);
  Serial.println(" gradi per rilasciare finecorsa");

  motore.moveTo(passiBackoff);
  while (motore.distanceToGo() != 0) {
    motore.run();
  }

  motore.setCurrentPosition(0);
  Serial.println("Backoff completato");

  // 🔁 Ripristina la configurazione originale
  motore.setMaxSpeed(prevMaxSpeed);
  motore.setAcceleration(prevAcceleration);

  lcd.setCursor(0, 0);
  lcd.print("Pronto          ");
}
