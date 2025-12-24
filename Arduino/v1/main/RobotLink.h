#pragma once
#include <Arduino.h>
#include <RF24.h>

// Pacchetto che arriva dal Nano (deve combaciare col TX) 
struct Payload {
  int16_t m1, m2, m3, m4; // Target per i motori (es. gradi o velocità)
  uint8_t c;              // calamita toggle
  uint8_t save;           // impulso SAVE
  uint8_t exec;           // 1 = valido (puoi ignorarlo in modalità velocity)
  uint8_t b6;             // impulso BTN6
  uint8_t b7;             // impulso BTN7
};

static_assert(sizeof(Payload) == 13, "Payload size mismatch (expected 13 bytes)");

// ACK compatto: 4 posizioni (steps int16) + magnet + enabled
#pragma pack(push,1)
struct AckPayload {
  int16_t p1, p2, p3, p4; // currentPosition() clamp a int16
  uint8_t magnet;         // 0/1
  uint8_t enabled;        // 0/1
};
#pragma pack(pop)

typedef void (*StatusProvider)(AckPayload& out);
  
// Firma dei callback
typedef void (*CommandHandler)(const Payload& p);
typedef void (*ButtonHandler)();

class RobotLink {
public:
 
  // cePin, csnPin = pin collegati al nRF24 sul MEGA (es. CE=9, CSN=10)
  RobotLink(uint8_t cePin, uint8_t csnPin);

  void setStatusProvider(StatusProvider p) { _statusProvider = p; }
 
  // opzionale: cambia i nomi pipe (5 char + terminatore)
  void setPipes(const uint8_t listenPipe[6], const uint8_t replyPipe[6]);

  // Inizializza radio e SPI; imposta canale, datarate, potenza, ACK
  void begin();

  // Registra i callback (tutti opzionali, ma onCommand è il più utile)
  void setHandlers(CommandHandler onCommand,
                   ButtonHandler onBtn6 = nullptr,
                   ButtonHandler onBtn7 = nullptr);

  // Da chiamare nel loop(): legge pacchetti, invoca callback, manda "ready"
  void poll();

  // Facoltativo: reinvia manualmente il "ready"
  void sendReady();

private:
  RF24 _radio;
  uint8_t _pipeListen[6];
  uint8_t _pipeReply[6];

  CommandHandler _onCommand = nullptr;
  ButtonHandler  _onBtn6    = nullptr;
  ButtonHandler  _onBtn7    = nullptr;

  StatusProvider _statusProvider = nullptr; 
  void _openPipes();
  void _configRadio();
};