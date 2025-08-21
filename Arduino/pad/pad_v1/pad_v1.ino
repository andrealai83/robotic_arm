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
const int sw3Pin = 6, sw4Pin = 7;   // <-- nuovi

// Stato
int lastX=0,lastY=0,lastZ=0,lastA=0;
bool magnet=false, mostraMessaggioTX=false, attesaReady=false;
unsigned long messaggioTXMillis=0;
const int deadZone=5;

// stato precedente pulsanti (per riconoscere il fronte)
bool prevSw1=false, prevSw2=false, prevSw3=false, prevSw4=false;

struct Payload {
  int16_t x,y,z,a;   // -90..90
  uint8_t c;         // calamita ON/OFF (toggle con SW1)
  uint8_t save;      // impulso SAVE (SW2)
  uint8_t exec;      // 1 = pacchetto valido
  uint8_t b6;        // impulso BTN6 (D6)
  uint8_t b7;        // impulso BTN7 (D7)
};

bool waitReady(uint16_t timeoutMs=600){
  radio.startListening();
  unsigned long t0=millis();
  while(millis()-t0 < timeoutMs){
    if(radio.available()){
      char buf[8]={0};
      radio.read(&buf,sizeof(buf));
      if(String(buf).startsWith("ready")){
        radio.stopListening();
        return true;
      }
    }
  }
  radio.stopListening();
  return false;
}

void radioInit(){
  radio.begin();
  radio.setChannel(76);
  radio.setDataRate(RF24_250KBPS);
  radio.setPALevel(RF24_PA_LOW);
  radio.setAutoAck(true);
  radio.setRetries(5, 7);
  radio.openWritingPipe(PIPE_TX);
  radio.openReadingPipe(1, PIPE_RX);
  radio.stopListening();
  Serial.print(F("TX pipe: ")); Serial.write(PIPE_TX,5); Serial.println();
  Serial.print(F("RX pipe: ")); Serial.write(PIPE_RX,5); Serial.println();
}

void setup(){
  Serial.begin(115200);
  pinMode(sw1Pin, INPUT_PULLUP);
  pinMode(sw2Pin, INPUT_PULLUP);
  pinMode(sw3Pin, INPUT_PULLUP);    // <-- nuovi
  pinMode(sw4Pin, INPUT_PULLUP);    // <-- nuovi

  radioInit();

  u8g2.begin();
  u8g2.setFont(u8g2_font_ncenB08_tr);
  u8g2.firstPage(); do { u8g2.drawStr(8, 28, "Joystick + nRF24"); } while(u8g2.nextPage());
  delay(300);
  Serial.println(F("🟢 NANO pronto (TX)"));
}

void loop(){
  // Joystick
  int valY = map(analogRead(joy1X),0,1023,-90,90);
  int valX = map(analogRead(joy1Y),0,1023,-90,90);
  int valZ = map(analogRead(joy2X),0,1023,-90,90);
  int valA = map(analogRead(joy2Y),0,1023,-90,90);

  // Lettura pulsanti (attivi LOW)
  bool sw1 = (digitalRead(sw1Pin)==LOW);
  bool sw2 = (digitalRead(sw2Pin)==LOW);
  bool sw3 = (digitalRead(sw3Pin)==LOW);
  bool sw4 = (digitalRead(sw4Pin)==LOW);

  // Rilevo solo il fronte di pressione
  bool e1 = sw1 && !prevSw1;
  bool e2 = sw2 && !prevSw2;
  bool e3 = sw3 && !prevSw3;  // BTN6
  bool e4 = sw4 && !prevSw4;  // BTN7

  bool moved = (abs(valX-lastX)>deadZone) || (abs(valY-lastY)>deadZone) ||
               (abs(valZ-lastZ)>deadZone) || (abs(valA-lastA)>deadZone);

  bool changed = moved || e1 || e2 || e3 || e4;

  if(changed){
    if(e1) magnet = !magnet;  // toggle calamita solo su fronte

    Payload p{
      (int16_t)valX, (int16_t)valY, (int16_t)valZ, (int16_t)valA,
      (uint8_t)(magnet?1:0),
      (uint8_t)(e2?1:0),  // SAVE solo impulso
      1,
      (uint8_t)(e3?1:0),  // BTN6 impulso
      (uint8_t)(e4?1:0)   // BTN7 impulso
    };

    bool ok = radio.write(&p, sizeof(p));
    Serial.print(F("TX -> "));
    Serial.print(p.x);Serial.print(',');Serial.print(p.y);Serial.print(',');
    Serial.print(p.z);Serial.print(',');Serial.print(p.a);
    Serial.print(F(" | C:"));Serial.print(p.c);
    Serial.print(F(" SAVE:"));Serial.print(p.save);
    Serial.print(F(" B6:"));Serial.print(p.b6);
    Serial.print(F(" B7:"));Serial.println(p.b7);
    Serial.println(ok?F("  [OK]"):F("  [FAIL]"));

    mostraMessaggioTX=true; messaggioTXMillis=millis();
    bool ack = waitReady(600);
    Serial.println(ack?F("ACK: ready"):F("ACK: timeout"));

    // aggiorno stati
    lastX=valX; lastY=valY; lastZ=valZ; lastA=valA;
    prevSw1=sw1; prevSw2=sw2; prevSw3=sw3; prevSw4=sw4;
  } else {
    // aggiorno gli stati se non ho inviato (debounce semplice)
    prevSw1=sw1; prevSw2=sw2; prevSw3=sw3; prevSw4=sw4;
  }

  // OLED
  u8g2.firstPage(); do{
    u8g2.setFont(u8g2_font_6x10_tf);
    u8g2.setCursor(0,10); u8g2.print("X: "); u8g2.print(lastX);
    u8g2.setCursor(0,20); u8g2.print("Y: "); u8g2.print(lastY);
    u8g2.setCursor(0,30); u8g2.print("Z: "); u8g2.print(lastZ);
    u8g2.setCursor(0,40); u8g2.print("A: "); u8g2.print(lastA);
    u8g2.setCursor(0,52); u8g2.print("MAG: "); u8g2.print(magnet?"ON":"OFF");
    if(mostraMessaggioTX && millis()-messaggioTXMillis<900){
      u8g2.setCursor(0,63); u8g2.print("TX inviato");
    } else mostraMessaggioTX=false;
  } while(u8g2.nextPage());

  delay(10);
}
