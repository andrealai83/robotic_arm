#include "RobotLink.h"
#include <SPI.h>

static const uint8_t DEFAULT_LISTEN[6] = "NODE1"; // Nano -> Mega
static const uint8_t DEFAULT_REPLY [6] = "NODE2"; // Mega -> Nano

RobotLink::RobotLink(uint8_t cePin, uint8_t csnPin)
: _radio(cePin, csnPin)
{
  memcpy(_pipeListen, DEFAULT_LISTEN, 6);
  memcpy(_pipeReply,  DEFAULT_REPLY,  6);
}

void RobotLink::setPipes(const uint8_t listenPipe[6], const uint8_t replyPipe[6]) {
  memcpy(_pipeListen, listenPipe, 6);
  memcpy(_pipeReply,  replyPipe,  6); 
  if (_radio.isChipConnected()) {
    _openPipes();
  }
}

void RobotLink::_configRadio() {
  _radio.setChannel(76);
  _radio.setDataRate(RF24_250KBPS);
  _radio.setPALevel(RF24_PA_LOW);
  _radio.setAutoAck(true);
  _radio.enableAckPayload();       
  _radio.enableDynamicPayloads();  
  _radio.setCRCLength(RF24_CRC_16);
  _radio.setRetries(5, 7);
}

void RobotLink::_openPipes() {
  _radio.openReadingPipe(1, _pipeListen); // ascolta il Nano
  _radio.openWritingPipe(_pipeReply);     // risponde al Nano
}

void RobotLink::begin() {
  
  pinMode(SS, OUTPUT);
  digitalWrite(SS, HIGH);

  _radio.begin();
  _configRadio();
  _openPipes();
  _radio.startListening();
}

void RobotLink::setHandlers(CommandHandler onCommand,
                            ButtonHandler onBtn6,
                            ButtonHandler onBtn7) {
  _onCommand = onCommand;
  _onBtn6 = onBtn6;
  _onBtn7 = onBtn7;
}

void RobotLink::sendReady() {
  _radio.stopListening();
  AckPayload ack{};                         
  if (_statusProvider) _statusProvider(ack); 
  (void)_radio.write(&ack, sizeof(ack)); 
  _radio.startListening();
}

void RobotLink::poll() {
  uint8_t pipe;
  if (_radio.available(&pipe)) {     
    Payload p{};
    _radio.read(&p, sizeof(p));

    if (_onCommand) _onCommand(p);
    if (p.b6 && _onBtn6) _onBtn6();
    if (p.b7 && _onBtn7) _onBtn7();

    Serial.print(F("RF: "));
    Serial.print(p.m1); Serial.print(' ');
    Serial.print(p.m2); Serial.print(' ');
    Serial.print(p.m3); Serial.print(' ');
    Serial.println(p.m4);

    if (_statusProvider) {
      AckPayload ack{};
      _statusProvider(ack);
      _radio.writeAckPayload(pipe, &ack, sizeof(ack));
    }
  }
}