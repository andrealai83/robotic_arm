#pragma once
#include <Arduino.h>
#include <RF24.h>

// Pacchetto che arriva dal Nano (deve combaciare col TX)
struct Payload {
  int16_t x, y, z, a;   // -90..+90
  uint8_t c;            // calamita 0/1
  uint8_t save;         // impulso SAVE
  uint8_t exec;         // 1 = pacchetto valido
  uint8_t b6;           // BTN6 (impulso)
  uint8_t b7;           // BTN7 (impulso)
} __attribute__((packed));

static_assert(sizeof(Payload) == 13, "Payload size mismatch (expected 13 bytes)");

// Firma dei callback
typedef void (*CommandHandler)(const Payload& p);
typedef void (*ButtonHandler)();

class RobotLink {
public:
  // cePin, csnPin = pin collegati al nRF24 sul MEGA (es. CE=9, CSN=10)
  RobotLink(uint8_t cePin, uint8_t csnPin);

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

  void _openPipes();
  void _configRadio();
};
