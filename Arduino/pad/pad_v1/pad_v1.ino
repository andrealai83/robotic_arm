#include <SoftwareSerial.h>
#include <U8g2lib.h>
#include <Wire.h>

SoftwareSerial btSerial(2, 3); // RX, TX
U8G2_SH1106_128X64_NONAME_1_HW_I2C u8g2(U8G2_R0, /* reset=*/U8X8_PIN_NONE);

// Pin joystick
int joy1X = A1;  // Base (Y)
int joy1Y = A2;  // Spalla (X)
int joy2X = A0;  // Gomito (Z)
int joy2Y = A3;  // Pinza (A)
int sw1Pin = 4;  // Pulsante joystick 1
int sw2Pin = 5;  // Pulsante joystick 2

int lastX = 0, lastY = 0, lastZ = 0, lastA = 0;
bool magnet = false;

bool attesaReady = false;
const int deadZone = 5;

bool mostraMessaggioTX = false;
unsigned long messaggioTXMillis = 0;

// Icona Bluetooth
const uint8_t btIcon[] U8X8_PROGMEM = {
  0b00011000,
  0b00100100,
  0b01000100,
  0b00011000,
  0b00011000,
  0b01000100,
  0b00100100,
  0b00011000
};

// Icona calamita
const uint8_t magnetIcon[] U8X8_PROGMEM = {
  0b00111100,
  0b01100110,
  0b01100110,
  0b01111110,
  0b01111110,
  0b01100000,
  0b01100000,
  0b01100000
};

void setup() {
  Serial.begin(9600);
  btSerial.begin(9600);
  pinMode(sw1Pin, INPUT_PULLUP);
  pinMode(sw2Pin, INPUT_PULLUP);

  Serial.println("🟢 NANO pronto: Joystick + BT attivi!");

  u8g2.begin();
  u8g2.setFont(u8g2_font_ncenB08_tr);

  // Messaggio iniziale
  u8g2.firstPage();
  do {
    u8g2.drawStr(10, 24, "Joystick Pronto!");
    u8g2.drawStr(10, 45, "By Andre ⚙️");
  } while (u8g2.nextPage());

  delay(2000); // Mostra 2 secondi
}

void loop() {
  // Ricevi dal Mega
  if (btSerial.available()) {
    String risposta = btSerial.readStringUntil('\n');
    risposta.trim();
    Serial.println("🔁 Risposta Mega: " + risposta);
    if (risposta == "ready") {
      attesaReady = false;
    }
  }

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

  // Pulsanti
  bool sw1Pressed = digitalRead(sw1Pin) == LOW;
  bool sw2Pressed = digitalRead(sw2Pin) == LOW;

  // Se cambia qualcosa o premi
  if (abs(valX - lastX) > deadZone || abs(valY - lastY) > deadZone ||
      abs(valZ - lastZ) > deadZone || abs(valA - lastA) > deadZone ||
      sw1Pressed || sw2Pressed) {

    btSerial.println("X:" + String(valX));
    btSerial.println("Y:" + String(valY));
    btSerial.println("Z:" + String(valZ));
    btSerial.println("A:" + String(valA));

    if (sw1Pressed) magnet = !magnet;
    btSerial.println(magnet ? "C:1" : "C:0");

    if (sw2Pressed) btSerial.println("SW2:1");

    btSerial.println("EXEC");
    Serial.println("📤 Inviato");

    mostraMessaggioTX = true;
    messaggioTXMillis = millis();

    attesaReady = true;

    lastX = valX;
    lastY = valY;
    lastZ = valZ;
    lastA = valA;
  }

  // --- OLED DISPLAY DRAW ---
  u8g2.firstPage();
  do {
    u8g2.setFont(u8g2_font_6x10_tf);
    u8g2.setCursor(0, 10); u8g2.print("X: "); u8g2.print(lastX);
    u8g2.setCursor(0, 20); u8g2.print("Y: "); u8g2.print(lastY);
    u8g2.setCursor(0, 30); u8g2.print("Z: "); u8g2.print(lastZ);
    u8g2.setCursor(0, 40); u8g2.print("A: "); u8g2.print(lastA);

    u8g2.setCursor(0, 52);
    u8g2.print("MAG: ");
    u8g2.print(magnet ? "ON" : "OFF");

    if (magnet) {
      u8g2.drawXBMP(60, 44, 8, 8, magnetIcon);
    }

    // Icona BT sempre
    u8g2.drawXBMP(112, 0, 8, 8, btIcon);

    // Messaggio TX per 1 secondo
    if (mostraMessaggioTX && millis() - messaggioTXMillis < 1000) {
      u8g2.setCursor(0, 63);
      u8g2.print("TX: Comando inviato");
    } else {
      mostraMessaggioTX = false;
    }

  } while (u8g2.nextPage());

  delay(10);
}
