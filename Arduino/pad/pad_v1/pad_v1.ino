#include <SPI.h>
#include <RF24.h>
#include <U8g2lib.h>
#include <Wire.h>

RF24 radio(9, 10); // CE, CSN
const byte PIPE_TX[6] = "NODE1";   // Nano -> Mega
const byte PIPE_RX[6] = "NODE2";   // Mega -> Nano (ready)

U8G2_SH1106_128X64_NONAME_1_HW_I2C u8g2(U8G2_R0, U8X8_PIN_NONE);

// Joystick/pulsanti
const int joy1X = A1, joy1Y = A2, joy2X = A0, joy2Y = A3;
const int sw1Pin = 4, sw2Pin = 5;
const int sw3Pin = 6, sw4Pin = 7;

// Tuning velocità
const int JOY_DEADZONE = 30;       // 0..1023 (zona morta raw)
const int VEL_MAX = 100;           // range di uscita per m1..m4
const bool USE_CUBIC = true;       // curva risposta

// Stato
int lastM1=0,lastM2=0,lastM3=0,lastM4=0;
bool magnet=false, mostraMessaggioTX=false;
unsigned long messaggioTXMillis=0;

// fronte pulsanti
bool prevSw1=false, prevSw2=false, prevSw3=false, prevSw4=false;

#pragma pack(push,1)
struct AckPayload {
  int16_t p1, p2, p3, p4;
  uint8_t magnet;
  uint8_t enabled;
};
#pragma pack(pop)

static AckPayload lastAck{}; // per l’OLED

struct Payload {
  int16_t m1, m2, m3, m4;  // velocità -100..100
  uint8_t c;               // calamita ON/OFF (toggle con SW1)
  uint8_t save;            // impulso SAVE (SW2)
  uint8_t exec;            // 1 = pacchetto valido
  uint8_t b6;              // impulso BTN6 (D6)
  uint8_t b7;              // impulso BTN7 (D7)
};

bool waitReady(AckPayload* out, uint16_t timeoutMs = 30) {
  unsigned long t0 = millis();
  while (millis() - t0 < timeoutMs) {
    if (radio.isAckPayloadAvailable()) {
      AckPayload ack{};
      radio.read(&ack, sizeof(ack));   // <-- si legge direttamente l’ACK payload
      if (out) *out = ack;
      return true;
    }
  }
  return false;
}

void radioInit(){
   radio.begin();
  radio.setChannel(76);
  radio.setDataRate(RF24_250KBPS);
  radio.setPALevel(RF24_PA_LOW);
  radio.setAutoAck(true);
  radio.enableAckPayload();       
  radio.enableDynamicPayloads();   
  radio.setCRCLength(RF24_CRC_16); 
  radio.setRetries(5,7); 
  radio.openWritingPipe(PIPE_TX);      // "NODE1"
  radio.openReadingPipe(1, PIPE_RX);   // "NODE2"
  radio.flush_rx();
  radio.flush_tx();
  radio.stopListening();               // TX per default
}

// helper: raw(0..1023) -> velocità [-VEL_MAX..VEL_MAX] con deadzone e curva
int rawToVel(int raw) {
  int centered = raw - 512;
  if (abs(centered) < JOY_DEADZONE) return 0;

  // normalizza a -1..1 togliendo la zona morta
  int sign = (centered > 0) ? 1 : -1;
  int mag = abs(centered) - JOY_DEADZONE;
  int denom = 512 - JOY_DEADZONE;
  float x = (float)mag / (float)denom; // 0..1
  if (x > 1.0f) x = 1.0f;

  // curva risposta
  if (USE_CUBIC) x = x*x*x; // più fine vicino al centro

  float vel = sign * x * VEL_MAX;
  // arrotondo a int
  int v = (int)roundf(vel);
  if (v < -VEL_MAX) v = -VEL_MAX;
  if (v >  VEL_MAX) v =  VEL_MAX;
  return v;
}

void setup(){
  Serial.begin(115200);
  pinMode(sw1Pin, INPUT_PULLUP);
  pinMode(sw2Pin, INPUT_PULLUP);
  pinMode(sw3Pin, INPUT_PULLUP);
  pinMode(sw4Pin, INPUT_PULLUP);

  radioInit();

  u8g2.begin();
  u8g2.setFont(u8g2_font_ncenB08_tr);
  u8g2.firstPage(); do { u8g2.drawStr(8, 28, "Joystick + nRF24"); } while(u8g2.nextPage());
  delay(300);
  Serial.println(F("🟢 NANO pronto (TX)"));
}

void loop(){
  // Leggi joystick raw
  int rM2 = analogRead(joy1X); // mappa come preferisci gli assi
  int rM1 = analogRead(joy1Y);
  int rM3 = analogRead(joy2X);
  int rM4 = analogRead(joy2Y);

  // Converte in velocità [-VEL_MAX..VEL_MAX]
  int m1 = rawToVel(rM1);
  int m2 = rawToVel(rM2);
  int m3 = rawToVel(rM3);
  int m4 = rawToVel(rM4);

  // Pulsanti (attivi LOW)
  bool sw1 = (digitalRead(sw1Pin)==LOW);
  bool sw2 = (digitalRead(sw2Pin)==LOW);
  bool sw3 = (digitalRead(sw3Pin)==LOW);
  bool sw4 = (digitalRead(sw4Pin)==LOW);

  // Fronti
  bool e1 = sw1 && !prevSw1;
  bool e2 = sw2 && !prevSw2;
  bool e3 = sw3 && !prevSw3;  // BTN6 impulso
  bool e4 = sw4 && !prevSw4;  // BTN7 impulso

  bool moved = (m1!=lastM1) || (m2!=lastM2) || (m3!=lastM3) || (m4!=lastM4);
  bool changed = moved || e1 || e2 || e3 || e4;

  if(changed){
    if(e1) magnet = !magnet;  // toggle calamita

    Payload p{
      (int16_t)m1, (int16_t)m2, (int16_t)m3, (int16_t)m4,
      (uint8_t)(magnet?1:0),
      (uint8_t)(e2?1:0),
      1,
      (uint8_t)(e3?1:0),
      (uint8_t)(e4?1:0)
    };

    bool ok = radio.write(&p, sizeof(p));

    if (ok) {
      while (radio.isAckPayloadAvailable()) {
        AckPayload ack{};
        radio.read(&ack, sizeof(ack));
        lastAck = ack; // ora OLED mostra p1..p4/ENA aggiornati
        // Debug (facoltativo):
        // Serial.print(F("[ACK] p: "));
        // Serial.print(ack.p1); Serial.print(',');
        // Serial.print(ack.p2); Serial.print(',');
        // Serial.print(ack.p3); Serial.print(',');
        // Serial.print(ack.p4);
        // Serial.print(F(" mag=")); Serial.print(ack.magnet);
        // Serial.print(F(" ena=")); Serial.println(ack.enabled);
      }
    }

    mostraMessaggioTX=true; messaggioTXMillis=millis();
      
    // aggiorno stati
    lastM1=m1; lastM2=m2; lastM3=m3; lastM4=m4;
    prevSw1=sw1; prevSw2=sw2; prevSw3=sw3; prevSw4=sw4;
  } else {
    prevSw1=sw1; prevSw2=sw2; prevSw3=sw3; prevSw4=sw4;
  }

  // OLED (mostra velocità)
  u8g2.firstPage(); do{
  u8g2.setFont(u8g2_font_6x10_tf);

  // Colonna sinistra: velocità m1..m4
  u8g2.setCursor(0,10);  u8g2.print("m1: "); u8g2.print(lastM1);
  u8g2.setCursor(0,20);  u8g2.print("m2: "); u8g2.print(lastM2);
  u8g2.setCursor(0,30);  u8g2.print("m3: "); u8g2.print(lastM3);
  u8g2.setCursor(0,40);  u8g2.print("m4: "); u8g2.print(lastM4);
  u8g2.setCursor(0,52);  u8g2.print("MAG: "); u8g2.print(magnet?"ON":"OFF");

  // Colonna destra: posizioni dal Mega (steps int16)
  const int colX = 72; // metà schermo circa (128px)
  u8g2.setCursor(colX,10); u8g2.print("p1: "); u8g2.print(lastAck.p1);
  u8g2.setCursor(colX,20); u8g2.print("p2: "); u8g2.print(lastAck.p2);
  u8g2.setCursor(colX,30); u8g2.print("p3: "); u8g2.print(lastAck.p3);
  u8g2.setCursor(colX,40); u8g2.print("p4: "); u8g2.print(lastAck.p4);
  u8g2.setCursor(colX,52); u8g2.print("ENA: "); u8g2.print(lastAck.enabled ? "ON" : "OFF");

    // Messaggio TX (come prima)
    if(mostraMessaggioTX && millis()-messaggioTXMillis<900){
      u8g2.setCursor(0,63); u8g2.print("TX inviato");
    } else mostraMessaggioTX=false;
  } while(u8g2.nextPage());

  delay(10);
}
