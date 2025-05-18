void setup() {
  Serial.begin(9600);        // PC
  Serial1.begin(38400);      // HC-05 (in AT mode)
  Serial.println("Ready - Type AT commands");
}

void loop() {
  if (Serial.available()) {
    Serial1.write(Serial.read());
  }
  if (Serial1.available()) {
    Serial.write(Serial1.read());
  }
}


// AT+RESET
// AT+ORGL
// AT+UART=9600,0,0
// AT+ROLE=0
// AT+CMODE=0