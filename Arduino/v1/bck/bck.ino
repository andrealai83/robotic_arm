#include <AccelStepper.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <SoftwareSerial.h>
SoftwareSerial btSerial(2, 3); // RX, TX

// Imposta l'indirizzo I2C (di solito 0x27 o 0x3F)
LiquidCrystal_I2C lcd(0x27, 16, 2);

// Definizione dei pin per i motori (inizialmente impostati come default)
int ENA1 = 13, DIR1 = 12, PUL1 = 11;
int ENA2 = 10, DIR2 = 9, PUL2 = 8;
int ENA3 = 7, DIR3 = 6, PUL3 = 5;
int ENA4 = 4, DIR4 = 3, PUL4 = 2;
int transistorPin = A0;

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

int target1 = 0, target2 = 0, target3 = 0, target4 = 0;
bool calamitaAttiva = false;

int xTargetTemp, yTargetTemp, zTargetTemp, aTargetTemp;
bool eseguiMovimento = false;

int homeX = 0, homeY = 0, homeZ = 0, homeA = 0;
 
void setup() {

  Serial.begin(9600);
  btSerial.begin(9600);
  Serial.println("Mega pronto, BT su Serial1");
  btSerial.println("Ciao dal Bluetooth!");

  pinMode(transistorPin, OUTPUT);

  // Configura i motori con le impostazioni iniziali
  setupMotor(motore1, ENA1);
  setupMotor(motore2, ENA2);
  setupMotor(motore3, ENA3);
  setupMotor(motore4, ENA4);

  // Inizializza il display
  lcd.init();
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Motori Pronti");
  delay(1000);
  lcd.clear();
  aggiornaDisplay();
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
  if (Serial.available()) {
    String comando = Serial.readStringUntil('\n');
    comando.trim();

    if (comando.startsWith("X:")) {
      xTargetTemp = parseTarget(comando);
      mostraMessaggio("X:" + String(xTargetTemp));
      setTarget(motore1, xTargetTemp);
      motore1Completato = false;
      target1 = xTargetTemp;
    } 
    else if (comando.startsWith("Y:")) {
      yTargetTemp = parseTarget(comando);
      mostraMessaggio("Y:" + String(yTargetTemp));
      setTarget(motore2, yTargetTemp);
      motore2Completato = false;
      target2 = yTargetTemp;
    } 
    else if (comando.startsWith("Z:")) {
      zTargetTemp = parseTarget(comando);
      mostraMessaggio("Z:" + String(zTargetTemp));
      setTarget(motore3, zTargetTemp);
      motore3Completato = false;
      target3 = zTargetTemp;
    } 
    else if (comando.startsWith("A:")) {
      aTargetTemp = parseTarget(comando);
      mostraMessaggio("A:" + String(aTargetTemp));
      setTarget(motore4, aTargetTemp);
      motore4Completato = false;
      target4 = aTargetTemp;
    } 
    else if (comando == "RUN" || comando == "EXEC") {
      // Esegui i movimenti in parallelo
      eseguiMovimento = true;
      mostraMessaggio(comando);
    } 
    else if (comando == "SETHOME") {
      homeX = target1;
      homeY = target2;
      homeZ = target3;
      homeA = target4;
      mostraMessaggio("Home salvata");
      Serial.println("ok");
    } 
    else if (comando == "GOHOME") {
      mostraMessaggio("Torno a Home");
      setTarget(motore1, homeX);
      motore1Completato = false;
      target1 = homeX;

      setTarget(motore2, homeY);
      motore2Completato = false;
      target2 = homeY;

      setTarget(motore3, homeZ);
      motore3Completato = false;
      target3 = homeZ;

      setTarget(motore4, homeA);
      motore4Completato = false;
      target4 = homeA;

      eseguiMovimento = true;
    }
    else if (comando.startsWith("C:")) {
      int stato = parseTarget(comando);
      calamitaAttiva = (stato == 1);
      digitalWrite(transistorPin, stato == 1 ? HIGH : LOW);
      Serial.println("ok");
      mostraMessaggio("C:" + String(stato));
    } 
    else if (comando.startsWith("CFG:")) {
      setConfiguration(comando.substring(4));
      Serial.println("ok");
    }
  }

  // Loop continuo per eseguire i movimenti simultanei
  checkMotor(motore1, motore1Completato);
  checkMotor(motore2, motore2Completato);
  checkMotor(motore3, motore3Completato);
  checkMotor(motore4, motore4Completato);

  // Quando tutti i motori hanno finito
  if (eseguiMovimento &&
      motore1Completato && motore2Completato &&
      motore3Completato && motore4Completato) {
    Serial.println("ready");
    eseguiMovimento = false;
  }
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
// Verifica se il motore ha raggiunto il target
void checkMotor(AccelStepper& motore, bool& completato) {
  if (!completato) {
    motore.run();
    if (motore.distanceToGo() == 0) {
      completato = true;
      //Serial.println("ok");
      delay(10);  // Piccolo ritardo per stabilizzare la comunicazione
    }
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
  String param = config.substring(0, config.indexOf(":"));
  String value = config.substring(config.indexOf(":") + 1);
  int intValue = value.toInt();

  if (param == "passiPerGiro") {
    passiPerGiro = intValue;
    passiPerGrado = (double)(passiPerGiro * microstep) / 360.0;
    Serial.println("Passi per giro aggiornati");
  } else if (param == "microstep") {
    microstep = intValue;
    passiPerGrado = (double)(passiPerGiro * microstep) / 360.0;
    Serial.println("Microstep aggiornati");
  } else if (param == "maxSpeed") {
    maxSpeed = intValue;
    motore1.setMaxSpeed(maxSpeed);
    motore2.setMaxSpeed(maxSpeed);
    motore3.setMaxSpeed(maxSpeed);
    motore4.setMaxSpeed(maxSpeed);
    Serial.println("Max speed aggiornata");
  } else if (param == "maxAccel") {
    maxAccel = intValue;
    motore1.setAcceleration(maxAccel);
    motore2.setAcceleration(maxAccel);
    motore3.setAcceleration(maxAccel);
    motore4.setAcceleration(maxAccel);
    Serial.println("Max acceleration aggiornata");
  }
}  su arduino mega e #include <SoftwareSerial.h>
SoftwareSerial btSerial(2, 3); // RX, TX


int joy1X = A0, joy1Y = A1;
int joy2X = A2, joy2Y = A3;

void setup() {
  Serial.begin(9600);        // Monitor Seriale del PC
  btSerial.begin(9600);      // Velocità standard del modulo HC-05/06

  Serial.println("BT Ready - In attesa...");
  btSerial.println("Ciao dal Bluetooth!");
}

void loop() {

    // Se ricevo qualcosa via BT, lo stampo sulla seriale
  if (btSerial.available()) {
    char c = btSerial.read();
    Serial.write(c);
  }

  // Se scrivo da PC, lo mando via Bluetooth
  if (Serial.available()) {
    char c = Serial.read();
    btSerial.write(c);
  }
  
  int base = analogRead(joy1X);  // asse base
  int spalla = analogRead(joy1Y); // asse spalla
  int gomito = analogRead(joy2X); // asse gomito
  int pinza = analogRead(joy2Y);  // asse pinza

  // Mappa in gradi: da 0–1023 → -90° a +90°
  int b = map(base, 0, 1023, -90, 90);
  int s = map(spalla, 0, 1023, -90, 90);
  int g = map(gomito, 0, 1023, -90, 90);
  int p = map(pinza, 0, 1023, -90, 90);

  Serial.print("X:"); Serial.println(s);
  Serial.print("Y:"); Serial.println(b);
  Serial.print("Z:"); Serial.println(g);
  Serial.print("A:"); Serial.println(p);
  Serial.println("RUN");

  delay(200); // più reattivo? riduci
}  