#include "Display.h"
#include "Config.h"
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);

bool bluetoothConnesso(){
  return digitalRead(BT_STATE_PIN) == HIGH;
}

byte charEndStop[8] = {
  B00000,
  B01110,
  B11111,
  B11111,
  B11111,
  B01110,
  B00000,
  B00000
};

void setupDisplay(){
  lcd.init();
  lcd.backlight();
  lcd.createChar(1, charEndStop); // Custom char for Active Endstop
  lcd.clear();
  lcd.setCursor(0,0);
  lcd.print("Motori Pronti");
  delay(600);
  lcd.clear();
}

void aggiornaDisplay(){
  lcd.setCursor(0, 0);
  lcd.print("M1");
  if(digitalRead(ENDSTOP_1_PIN)==LOW) lcd.write(1); else lcd.print(":");
  lcd.print(target1);
  
  lcd.print(" M2");
  if(digitalRead(ENDSTOP_2_PIN)==LOW) lcd.write(1); else lcd.print(":");
  lcd.print(target2);

  lcd.setCursor(0, 1);
  lcd.print("M3");
  if(digitalRead(ENDSTOP_3_PIN)==LOW) lcd.write(1); else lcd.print(":");
  lcd.print(target3);
  
  lcd.print(" M4");
  if(digitalRead(ENDSTOP_4_PIN)==LOW) lcd.write(1); else lcd.print(":");
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
  // lcd.clear();
  // lcd.setCursor(0,0);
  // lcd.print("CMD: ");
  // lcd.print(messaggio);
  // return;
  // delay(1000);
  // lcd.clear();
  aggiornaDisplay();
}

void mostraStatoEndstop(bool e1, bool e2, bool e3, bool e4){
  lcd.setCursor(0, 0);
  lcd.print("E1:");
  lcd.print(e1 ? "ON " : "OFF");
  lcd.print(" E2:");
  lcd.print(e2 ? "ON " : "OFF");

  lcd.setCursor(0, 1);
  lcd.print("E3:");
  lcd.print(e3 ? "ON " : "OFF");
  lcd.print(" E4:");
  lcd.print(e4 ? "ON " : "OFF");
}
