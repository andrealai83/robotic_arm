#include <SPI.h>
#include <RF24.h>

RF24 radio(14,15); 
const byte pipeTX[6] = "P1TX";
const byte pipeRX[6] = "P1RX";

void setup() {
  Serial.begin(115200);
  radio.begin();
  radio.setChannel(76);
  radio.setDataRate(RF24_250KBPS);
  radio.setPALevel(RF24_PA_LOW);
  radio.openWritingPipe(pipeTX);
  radio.openReadingPipe(1, pipeRX);
  radio.stopListening(); // partiamo in TX
  Serial.println(F("NANO pronto"));
}

void loop() {
  static uint32_t counter=0;
  counter++;

  // Invia
  radio.stopListening();
  bool ok = radio.write(&counter, sizeof(counter));
  Serial.print(F("TX ")); Serial.print(counter); 
  Serial.println(ok ? F(" ok") : F(" fail"));

  // Ascolta per eco
  radio.startListening();
  unsigned long t0 = millis();
  while(millis()-t0 < 50) {
    if (radio.available()) {
      uint32_t rx=0;
      radio.read(&rx, sizeof(rx));
      Serial.print(F("RX echo: ")); Serial.println(rx);
      break;
    }
  }

  delay(500);
}
#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

RF24 radio(7, 8); // CE, CSN

const byte address[6] = "00001";

void setup() {
  Serial.begin(9600);
  radio.begin();
  radio.openReadingPipe(0, address);
  radio.setPALevel(RF24_PA_MIN);
  radio.startListening();
}

void loop() {
  if (radio.available()) {
    char text[32] = "";
    radio.read(&text, sizeof(text));
    Serial.println(text);
  }
}
