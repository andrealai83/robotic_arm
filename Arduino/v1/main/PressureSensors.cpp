#include "HardwareSerial.h"
#include "PressureSensors.h"
#include "Config.h"

namespace {
constexpr float PRESSURE_FILTER_ALPHA = 0.15f;
constexpr float PRESSURE_BASELINE_TRACK_ALPHA = 0.10f;
constexpr float PRESSURE_SCALE = 1.00f;
constexpr uint8_t PRESSURE_FULL_SCALE_DROP_RAW = 24;
constexpr uint8_t PRESSURE_IDLE_DEADBAND_RAW = 2;
constexpr uint8_t PRESSURE_CONTACT_THRESHOLD_PERCENT = 12;

struct PressureSensorData {
  uint8_t pin;
  int baselineRaw;
  float filteredRaw;
  uint8_t percent;
  bool active;
  uint32_t lastActiveMs;
};

PressureSensorData pressureSensors[2] = {
  { PRESSURE_SENSOR_1_PIN, 1023, 1023.0f, 0, false, 0 },
  { PRESSURE_SENSOR_2_PIN, 1023, 1023.0f, 0, false, 0 }
};

void initPressureSensor(PressureSensorData& sensor) {
  sensor.baselineRaw = analogRead(sensor.pin);
  sensor.filteredRaw = (float)sensor.baselineRaw;
  sensor.percent = 0;
  sensor.active = false;
  sensor.lastActiveMs = 0;
}

void updatePressureSensor(PressureSensorData& sensor) {
  const uint32_t now = millis();
  const int raw = analogRead(sensor.pin);
  sensor.filteredRaw = PRESSURE_FILTER_ALPHA * raw + (1.0f - PRESSURE_FILTER_ALPHA) * sensor.filteredRaw;

  if (sensor.filteredRaw > sensor.baselineRaw) {
    sensor.baselineRaw = (int)(PRESSURE_BASELINE_TRACK_ALPHA * sensor.filteredRaw +
                               (1.0f - PRESSURE_BASELINE_TRACK_ALPHA) * sensor.baselineRaw + 0.5f);
  }

  const float rawDrop = max(0.0f, (float)sensor.baselineRaw - sensor.filteredRaw);
  const bool releaseZone = rawDrop <= (float)PRESSURE_IDLE_DEADBAND_RAW;
 
  if (releaseZone) {
    sensor.baselineRaw = (int)(PRESSURE_BASELINE_TRACK_ALPHA * sensor.filteredRaw +
                               (1.0f - PRESSURE_BASELINE_TRACK_ALPHA) * sensor.baselineRaw + 0.5f);
  }

  const float delta = max(0.0f, rawDrop - PRESSURE_IDLE_DEADBAND_RAW);
  float percent = (delta / PRESSURE_FULL_SCALE_DROP_RAW) * 100.0f;
  percent *= PRESSURE_SCALE;

  if (percent < 0.0f) percent = 0.0f;
  if (percent > 100.0f) percent = 100.0f;

  sensor.percent = (uint8_t)(percent + 0.5f);
  sensor.active = (sensor.percent >= PRESSURE_CONTACT_THRESHOLD_PERCENT);

   Serial.println( (uint8_t)(percent));


  if (sensor.active) {
    sensor.lastActiveMs = now;
  } else if (releaseZone || (now - sensor.lastActiveMs > 250)) {
    sensor.percent = 0;
  }
}
}

void pressureSensorsSetup() {
  pinMode(PRESSURE_SENSOR_1_PIN, INPUT);
  pinMode(PRESSURE_SENSOR_2_PIN, INPUT);
  initPressureSensor(pressureSensors[0]);
  initPressureSensor(pressureSensors[1]);
}

void pressureSensorsUpdate() {
  
  updatePressureSensor(pressureSensors[0]);
  updatePressureSensor(pressureSensors[1]);
}

uint8_t getPressurePercent(uint8_t sensorIndex) {
  if (sensorIndex < 1 || sensorIndex > 2) return 0;
  return pressureSensors[sensorIndex - 1].percent;
}

bool isPressureDetected(uint8_t sensorIndex) {
  if (sensorIndex < 1 || sensorIndex > 2) return false;
  return pressureSensors[sensorIndex - 1].active;
}

uint8_t getGripPressurePercent() {
  const uint8_t p1 = getPressurePercent(1);
  const uint8_t p2 = getPressurePercent(2);
  return (p1 > p2) ? p1 : p2;
}

void printPressureStatus() {
  Serial.print(F("PRESS P1:"));
  Serial.print(getPressurePercent(1));
  Serial.print(F("% P2:"));
  Serial.print(getPressurePercent(2));
  Serial.print(F("% MAX:"));
  Serial.println(getGripPressurePercent());
}
