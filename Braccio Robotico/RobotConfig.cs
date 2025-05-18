using System;
using System.Collections.Generic;
using System.Data.SQLite; 
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Braccio_Robotico
{
    public static class RobotConfig
    {

        public static string connString = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "robotconfig.db")}";
         
        // Parametri dei motori
        public static int PassiPerGiro { get; set; } = 3000;
        public static int Microstep { get; set; } = 8;
        public static int MaxSpeed { get; set; } = 10000;
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

        public static void InizializzaDatabaseSeNecessario()
        {
        //    if (File.Exists("robotconfig.db"))
        //        File.Delete("robotconfig.db");

            if (!File.Exists("robotconfig.db"))
            {
                Console.WriteLine("SQLite database not found. Creating with default values...");

                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();

                    string creaTabella = @"
                CREATE TABLE Config (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PassiPerGiro INTEGER,
                    Microstep INTEGER,
                    MaxSpeed INTEGER,
                    MaxAccel INTEGER,
                    ENA1 INTEGER, DIR1 INTEGER, PUL1 INTEGER,
                    ENA2 INTEGER, DIR2 INTEGER, PUL2 INTEGER,
                    ENA3 INTEGER, DIR3 INTEGER, PUL3 INTEGER,
                    ENA4 INTEGER, DIR4 INTEGER, PUL4 INTEGER,
                    TransistorPin INTEGER
                );
                CREATE TABLE Posizioni (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nome TEXT NOT NULL
                    );
                CREATE TABLE Movimenti (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PosizioneId INTEGER NOT NULL,
                    Ordine INTEGER NOT NULL,
                    X INTEGER,
                    Y INTEGER,
                    Z INTEGER,
                    A INTEGER,
                    C TEXT,
                    FOREIGN KEY (PosizioneId) REFERENCES Posizioni(Id)
                ); 
            ";

                    using (var cmd = new SQLiteCommand(creaTabella, conn))
                        cmd.ExecuteNonQuery();

                    string inserisciDefault = @"
                INSERT INTO Config (
                    PassiPerGiro, Microstep, MaxSpeed, MaxAccel,
                    ENA1, DIR1, PUL1,
                    ENA2, DIR2, PUL2,
                    ENA3, DIR3, PUL3,
                    ENA4, DIR4, PUL4,
                    TransistorPin
                ) VALUES (
                    3000, 8, 10000, 4000,
                    13, 12, 11,
                    10, 9, 8,
                    7, 6, 5,
                    4, 3, 2,
                    0
                );
            ";

                    using (var cmd = new SQLiteCommand(inserisciDefault, conn))
                        cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Database created and initialized.");
            }
            else
            {
                Console.WriteLine("SQLite database already exists.");
            }
        }
         
        public static void CaricaConfigurazioni()
        {
            string query = "SELECT * FROM Config LIMIT 1";

            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        PassiPerGiro = reader.GetInt32(reader.GetOrdinal("PassiPerGiro"));
                        Microstep = reader.GetInt32(reader.GetOrdinal("Microstep"));
                        MaxSpeed = reader.GetInt32(reader.GetOrdinal("MaxSpeed"));
                        MaxAccel = reader.GetInt32(reader.GetOrdinal("MaxAccel"));
                        ENA1 = reader.GetInt32(reader.GetOrdinal("ENA1"));
                        DIR1 = reader.GetInt32(reader.GetOrdinal("DIR1"));
                        PUL1 = reader.GetInt32(reader.GetOrdinal("PUL1"));
                        ENA2 = reader.GetInt32(reader.GetOrdinal("ENA2"));
                        DIR2 = reader.GetInt32(reader.GetOrdinal("DIR2"));
                        PUL2 = reader.GetInt32(reader.GetOrdinal("PUL2"));
                        ENA3 = reader.GetInt32(reader.GetOrdinal("ENA3"));
                        DIR3 = reader.GetInt32(reader.GetOrdinal("DIR3"));
                        PUL3 = reader.GetInt32(reader.GetOrdinal("PUL3"));
                        ENA4 = reader.GetInt32(reader.GetOrdinal("ENA4"));
                        DIR4 = reader.GetInt32(reader.GetOrdinal("DIR4"));
                        PUL4 = reader.GetInt32(reader.GetOrdinal("PUL4"));
                        TransistorPin = reader.GetInt32(reader.GetOrdinal("TransistorPin"));
                    }
                }
            }
        }

        public static void SalvaConfigurazioni()
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = @"
            UPDATE Config SET
                PassiPerGiro = @passi,
                Microstep = @micro,
                MaxSpeed = @speed,
                MaxAccel = @accel,
                ENA1 = @ena1, DIR1 = @dir1, PUL1 = @pul1,
                ENA2 = @ena2, DIR2 = @dir2, PUL2 = @pul2,
                ENA3 = @ena3, DIR3 = @dir3, PUL3 = @pul3,
                ENA4 = @ena4, DIR4 = @dir4, PUL4 = @pul4,
                TransistorPin = @transistor
            WHERE Id = 1;
        ";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@passi", PassiPerGiro);
                    cmd.Parameters.AddWithValue("@micro", Microstep);
                    cmd.Parameters.AddWithValue("@speed", MaxSpeed);
                    cmd.Parameters.AddWithValue("@accel", MaxAccel);
                    cmd.Parameters.AddWithValue("@ena1", ENA1);
                    cmd.Parameters.AddWithValue("@dir1", DIR1);
                    cmd.Parameters.AddWithValue("@pul1", PUL1);
                    cmd.Parameters.AddWithValue("@ena2", ENA2);
                    cmd.Parameters.AddWithValue("@dir2", DIR2);
                    cmd.Parameters.AddWithValue("@pul2", PUL2);
                    cmd.Parameters.AddWithValue("@ena3", ENA3);
                    cmd.Parameters.AddWithValue("@dir3", DIR3);
                    cmd.Parameters.AddWithValue("@pul3", PUL3);
                    cmd.Parameters.AddWithValue("@ena4", ENA4);
                    cmd.Parameters.AddWithValue("@dir4", DIR4);
                    cmd.Parameters.AddWithValue("@pul4", PUL4);
                    cmd.Parameters.AddWithValue("@transistor", TransistorPin);
                    cmd.ExecuteNonQuery();
                }
            }
        }

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
