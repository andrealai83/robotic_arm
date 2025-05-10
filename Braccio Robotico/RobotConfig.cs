using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Braccio_Robotico
{
    public static class RobotConfig
    {
     
        // Parametri dei motori
        public static int PassiPerGiro { get; set; } = 3000;
        public static int Microstep { get; set; } = 8;
        public static int MaxSpeed { get; set; } = 50000;
        public static int MaxAccel { get; set; } = 4000;

        // Pin dei motori
        public static int ENA1 { get; set; } = 13;
        public static int DIR1 { get; set; } = 12;
        public static int PUL1 { get; set; } = 11;

        public static int ENA2 { get; set; } = 10;
        public static int DIR2 { get; set; } = 9;
        public static int PUL2 { get; set; } = 8;

        public static int ENA3 { get; set; } = 7;
        public static int DIR3 { get; set; } = 6;
        public static int PUL3 { get; set; } = 5;

        public static int ENA4 { get; set; } = 4;
        public static int DIR4 { get; set; } = 3;
        public static int PUL4 { get; set; } = 2;

        public static int TransistorPin { get; set; } = 0; // A0 su Arduino

        // Invia configurazioni all'Arduino
        public static void SendConfiguration(SerialPort serial)
        {
            SendConfig("passiPerGiro", PassiPerGiro, serial);
            SendConfig("microstep", Microstep, serial);
            SendConfig("maxSpeed", MaxSpeed, serial);
            SendConfig("maxAccel", MaxAccel, serial);
        }

        // Invia un singolo parametro
        private static void SendConfig(string name, int value, SerialPort serial)
        {
            string comando = $"CFG:{name}:{value}\n";
            serial.Write(comando);
            Console.WriteLine($"Configurazione inviata: {comando.Trim()}");
        } 
    }
}
