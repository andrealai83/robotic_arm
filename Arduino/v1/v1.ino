#include <AccelStepper.h>

// Definizione dei pin per i motori (inizialmente impostati come default)
int ENA1 = 13, DIR1 = 12, PUL1 = 11;
int ENA2 = 10, DIR2 = 9,  PUL2 = 8;
int ENA3 = 7,  DIR3 = 6,  PUL3 = 5;
int ENA4 = 4,  DIR4 = 3,  PUL4 = 2;
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

void setup() {
  Serial.begin(115200);
  pinMode(transistorPin, OUTPUT);
  
  // Configura i motori con le impostazioni iniziali
  setupMotor(motore1, ENA1);
  setupMotor(motore2, ENA2);
  setupMotor(motore3, ENA3);
  setupMotor(motore4, ENA4);
  
  Serial.println("Pronto");
}

void loop() {
  // Gestione comandi seriali
  if (Serial.available()) {
    String comando = Serial.readStringUntil('\n');
    comando.trim();

    if (comando.startsWith("X:") || comando.startsWith("Y:") || comando.startsWith("Z:") || comando.startsWith("A:")) {
      int target = parseTarget(comando);
      if (comando.startsWith("X:")) {
        setTarget(motore1, target);
        motore1Completato = false;
      } else if (comando.startsWith("Y:")) {
        setTarget(motore2, target);
        motore2Completato = false;
      } else if (comando.startsWith("Z:")) {
        setTarget(motore3, target);
        motore3Completato = false;
      } else if (comando.startsWith("A:")) {
        setTarget(motore4, target);
        motore4Completato = false;
      }
    }
    else if (comando.startsWith("C:")) {
      int stato = parseTarget(comando);
      digitalWrite(transistorPin, stato == 1 ? HIGH : LOW);
      Serial.println("ok");
    }
    else if (comando.startsWith("CFG:")) {
      setConfiguration(comando.substring(4));
      Serial.println("ok");
    }
  }

  // Esegui i movimenti e verifica se sono completi
  checkMotor(motore1, motore1Completato);
  checkMotor(motore2, motore2Completato);
  checkMotor(motore3, motore3Completato);
  checkMotor(motore4, motore4Completato);
}

// Configura un motore
void setupMotor(AccelStepper& motore, int pinENA) {
  pinMode(pinENA, OUTPUT);
  digitalWrite(pinENA, HIGH); // Abilita il motore

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
      Serial.println("ok");
      delay(10); // Piccolo ritardo per stabilizzare la comunicazione
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
  }
  else if (param == "microstep") {
    microstep = intValue;
    passiPerGrado = (double)(passiPerGiro * microstep) / 360.0;
    Serial.println("Microstep aggiornati");
  }
  else if (param == "maxSpeed") {
    maxSpeed = intValue;
    motore1.setMaxSpeed(maxSpeed);
    motore2.setMaxSpeed(maxSpeed);
    motore3.setMaxSpeed(maxSpeed);
    motore4.setMaxSpeed(maxSpeed);
    Serial.println("Max speed aggiornata");
  }
  else if (param == "maxAccel") {
    maxAccel = intValue;
    motore1.setAcceleration(maxAccel);
    motore2.setAcceleration(maxAccel);
    motore3.setAcceleration(maxAccel);
    motore4.setAcceleration(maxAccel);
    Serial.println("Max acceleration aggiornata");
  }
}
