using Braccio_Robotico.Models;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Braccio_Robotico.Helper
{
    public static class PosizioneStorage
    {
        private static string connString = RobotConfig.connString;

        public static void SalvaSetPosizioni(string nomeSet, List<Movimento> movimenti)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                using (var tr = conn.BeginTransaction())
                {
                    // Inserisce la posizione
                    var insertPosQuery = "INSERT INTO Posizioni (Nome) VALUES (@nome); SELECT last_insert_rowid();";
                    var insertPosCmd = new SQLiteCommand(insertPosQuery, conn, tr);
                    insertPosCmd.Parameters.AddWithValue("@nome", nomeSet);
                    long posId = (long)insertPosCmd.ExecuteScalar();

                    // Inserisce i movimenti
                    int ordine = 0;
                    foreach (var m in movimenti)
                    {
                        var insertMov = new SQLiteCommand(@"INSERT INTO Movimenti (PosizioneId, Ordine, X, Y, Z, A, C)
                                                        VALUES (@pid, @ordine, @x, @y, @z, @a, @c);", conn, tr);
                        insertMov.Parameters.AddWithValue("@pid", posId);
                        insertMov.Parameters.AddWithValue("@ordine", ordine++);
                        insertMov.Parameters.AddWithValue("@x", m.M1);
                        insertMov.Parameters.AddWithValue("@y", m.M2);
                        insertMov.Parameters.AddWithValue("@z", m.M4);
                        insertMov.Parameters.AddWithValue("@a", m.M3);
                        insertMov.Parameters.AddWithValue("@c", m.GRIP);
                        insertMov.ExecuteNonQuery();
                    }

                    tr.Commit();
                }
            }
        }

        public static List<Movimento> CaricaSetPosizioni(int posizioneId)
        {
            var lista = new List<Movimento>();

            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = @"SELECT X, Y, Z, A, C FROM Movimenti
                             WHERE PosizioneId = @pid ORDER BY Ordine;";
                var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@pid", posizioneId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Movimento
                        {
                            M1 = reader.GetInt32(0),
                            M2 = reader.GetInt32(1),
                            M4 = reader.GetInt32(2),
                            M3 = reader.GetInt32(3),
                            GRIP = reader.GetString(4)
                        });
                    }
                }
            }

            return lista;
        }

        public static List<PosizioneSalvata> GetElencoSetPosizioni()
        {
            var lista = new List<PosizioneSalvata>();

            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = "SELECT Id, Nome FROM Posizioni ORDER BY Nome;";
                var cmd = new SQLiteCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new PosizioneSalvata
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1)
                        });
                    }
                }
            }

            return lista;
        }

        public static void EliminaSetPosizione(int posizioneId)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                var tr = conn.BeginTransaction();

                var delMov = new SQLiteCommand("DELETE FROM Movimenti WHERE PosizioneId = @pid;", conn, tr);
                delMov.Parameters.AddWithValue("@pid", posizioneId);
                delMov.ExecuteNonQuery();

                var delPos = new SQLiteCommand("DELETE FROM Posizioni WHERE Id = @pid;", conn, tr);
                delPos.Parameters.AddWithValue("@pid", posizioneId);
                delPos.ExecuteNonQuery();

                tr.Commit();
            }
        }

        public static void RinominaSet(int posizioneId, string nuovoNome)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                var cmd = new SQLiteCommand("UPDATE Posizioni SET Nome = @nome WHERE Id = @pid;", conn);
                cmd.Parameters.AddWithValue("@nome", nuovoNome);
                cmd.Parameters.AddWithValue("@pid", posizioneId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
