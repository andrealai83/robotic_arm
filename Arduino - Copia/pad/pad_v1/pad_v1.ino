#include <SoftwareSerial.h>
SoftwareSerial btSerial(2, 3); // RX, TX

// Pin joystick
int joy1X = A0;  // Base (Y)
int joy1Y = A1;  // Spalla (X)
int joy2X = A2;  // Gomito (Z)
int joy2Y = A3;  // Pinza (A)

int lastX = 0, lastY = 0, lastZ = 0, lastA = 0;
bool attesaReady = false;

void setup() {
  Serial.begin(9600);
  btSerial.begin(9600);
  Serial.println("🟢 NANO pronto: Joystick + BT attivi!");
}

void loop() {
  // Leggi risposta dal Mega
  if (btSerial.available()) {
    String risposta = btSerial.readStringUntil('\n');
    risposta.trim();
    Serial.println("🔁 Risposta Mega: " + risposta);

    if (risposta == "ready") {
      attesaReady = false;  // pronto a inviare il prossimo comando
    }
  }

  // Se stiamo aspettando il "ready", non inviare nulla
  if (attesaReady) return;

  // Lettura joystick
  int base    = analogRead(joy1X);
  int spalla  = analogRead(joy1Y);
  int gomito  = analogRead(joy2X);
  int pinza   = analogRead(joy2Y);

  int valY = map(base,    0, 1023, -90, 90);
  int valX = map(spalla,  0, 1023, -90, 90);
  int valZ = map(gomito,  0, 1023, -90, 90);
  int valA = map(pinza,   0, 1023, -90, 90);

  // Invia solo se qualcosa è cambiato
  if (valX != lastX || valY != lastY || valZ != lastZ || valA != lastA) {
    String comando = "X:" + String(valX) +
                     "Y:" + String(valY) +
                     "Z:" + String(valZ) +
                     "A:" + String(valA) +
                     "EXEC";

    btSerial.print(comando);
    Serial.println("📤 Inviato: " + comando);

    // Blocca invio finché non arriva il prossimo "ready"
    attesaReady = true;

    // Salva i valori
    lastX = valX;
    lastY = valY;
    lastZ = valZ;
    lastA = valA;
  }

  delay(10); // leggero debounce
}
