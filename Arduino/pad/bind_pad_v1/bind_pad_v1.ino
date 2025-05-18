// www.robotlk.com , Robot Lk YouTube Channel
#include <SoftwareSerial.h>
SoftwareSerial mySerial(2, 3); // TX=2, RX=3 BLUETOOTH MODULE
void setup() {
Serial.begin(9600);
Serial.println("Enter AT commands:");
mySerial.begin(38400);
}
void loop()
{
if (mySerial.available())
Serial.write(mySerial.read());
if (Serial.available())
mySerial.write(Serial.read());
}

// AT+RESET
// AT+ORGL
// AT+UART=9600,0,0
// AT+ROLE=1
// AT+CMODE=0
// AT+INIT
// AT+BIND=03,DC,9A452EA0
// AT+BIND=03,DC,9A452EA0
// AT+LINK=03,DC,9A452EA0
//03:DC:9A:45:2E:A0

//+ADDR=03:DC:9A:45:2E:A0