#include "Display.h"
#include "Config.h"
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);

bool bluetoothConnesso(){
  return digitalRead(BT_STATE_PIN) == HIGH;
}

void setupDisplay(){
  lcd.init();
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0,0);
  lcd.print("Motori Pronti");
  delay(600);
  lcd.clear();
}

void aggiornaDisplay(){
  lcd.setCursor(0, 0);
  lcd.print("M1:");
  lcd.print(target1);
  lcd.print(" M2:");
  lcd.print(target2);

  lcd.setCursor(0, 1);
  lcd.print("M3:");
  lcd.print(target3);
  lcd.print(" M4:");
  lcd.print(target4);

  lcd.setCursor(15, 1);
  if (bluetoothConnesso()) lcd.write((byte)0);
  else lcd.print(" ");

  lcd.setCursor(12, 0);
  lcd.print(calamitaAttiva ? "ON " : "OFF");

  // in alto a destra mostra stato ENA
  // lcd.setCursor(11, 0);
  // lcd.print(motorsEnabled ? "EN " : "OFF");
}

void mostraMessaggio(const String& messaggio){
  return;
  lcd.clear();
  lcd.setCursor(0,0);
  lcd.print("CMD: ");
  lcd.print(messaggio);
  delay(300);
  lcd.clear();
  aggiornaDisplay();
}
