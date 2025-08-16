#include "Encoder.h"
#include "Config.h"

float ema = 0;
float angleDeg = 0;
 
void encoderSetup(){
  ema = analogRead(ENCODER_PIN);
}

void encoderUpdate(){
  int raw = analogRead(ENCODER_PIN);
  ema = ALPHA * raw + (1.0f - ALPHA) * ema;
  // mappa su 0..360 usando MIN/MAX calibrati
  float ang = ((ema - MIN_VAL) / (MAX_VAL - MIN_VAL)) * 360.0f;
  if (ang < 0) ang += 360.0f;
  if (ang >= 360) ang -= 360.0f;
  angleDeg = ang;

  static uint32_t tStream = 0;
  if (encoderStreamOn) {
    uint32_t now = millis();
    if (now - tStream >= encoderStreamPeriodMs) {
      tStream = now;
      Serial.print(F("ENC:"));
      Serial.println(angleDeg, 2);
    }
  }

  Serial.print(F("Encoder (deg): "));
  Serial.println(angleDeg, 2);
}
