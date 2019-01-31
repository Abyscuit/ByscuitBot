using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class ServerSQL
    {
        //Change these settings
        public static string DATABASE = "base";
        public static string USER = "root";
        public static string PASS = "";
        public static string IP = "localhost";
        public static uint PORT = 3306;
        //Add port if needed
        static MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder
        {
            Server = IP,
            Database = DATABASE,
            Port = PORT,
            UserID = USER,
            Password = PASS
        };
        public static string Select(string column, string table)
        {
            string queryString = "SELECT "+ column +" FROM  "+table;
            //string connectionString = "Server=" + IP + "; Port=" + PORT + "; Database=" + DATABASE + "; Uid=" + USER + "; Pwd=" + PASS + ";";
            string result = "";
            using (MySqlConnection connection = new MySqlConnection(csb.ToString()))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                //command.Parameters.AddWithValue("@column", column);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        result += reader[column].ToString() + "\n";
                        Console.WriteLine(string.Format("{0}", reader.ToString()));// etc
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            return result;
        }

        //Gets all info from a table where it equals the condition
        public static Xbox Select(string table, string where, string equals)
        {
            string queryString = "SELECT * FROM  " + table + " WHERE " + where + " = @equals";
            Xbox result = null;
            using (MySqlConnection connection = new MySqlConnection(csb.ToString()))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Prepare();
                command.Parameters.AddWithValue("@equals", equals);
                //command.ExecuteNonQuery();
                MySqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        DateTime time = DateTime.Parse(reader["time"].ToString());
                        DateTime now = DateTime.Now;
                        TimeSpan timeLeft = time.Subtract(now);
                        double totalSeconds = timeLeft.TotalSeconds;
                        double totalMins = totalSeconds / 60;
                        double totalHours = totalMins / 60;
                        double totalDays = totalHours / 24;
                        string totalTime = (int)totalDays + "d " + (int)(totalHours % 24) + "h " + (int)(totalMins % 60) + "m";
                        if (totalDays >= 300)
                            totalTime = "Lifetime";
                        if (totalSeconds < 0)
                            totalTime = "Expired";
                        Xbox xbox = new Xbox();
                        xbox.CPUKey = reader["cpukey"].ToString();
                        xbox.ID = (int)reader["id"];
                        xbox.Salt = reader["salt"].ToString();
                        xbox.Enabled = bool.Parse(reader["enabled"].ToString());
                        //1096157269 WaW; 1480659546 CSGO; 1096157460 AW
                        xbox.Game = 4294838225;    //Dashboard
                        xbox.Time = totalTime;
                        xbox.Gamertag = "Abyscuit"; //Replace with reader ToString()
                        xbox.totalDays = totalDays;
                        xbox.totalHours = totalHours;
                        xbox.totalMinutes = totalMins;
                        xbox.totalSeconds = totalSeconds;
                        result = xbox;
                    }
                }
                catch (Exception e)
                {
                    result = new Xbox();
                    result.CPUKey = "Failed";
                    result.Time = e.Message;
                    return result;
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            Console.WriteLine(string.Format("{0}", result));

            return result;
        }

        public class Xbox
        {
            public int ID;
            public string CPUKey;
            public string Salt;
            public string Time;
            public double totalSeconds;
            public double totalHours;
            public double totalDays;
            public double totalMinutes;
            public bool Enabled;
            public uint Game;
            public string Gamertag;
        }
        public static string Insert(string table, string[] columns, string[] values)
        {
            string cols = "";
            for (int i = 0; i < columns.Length; i++)
            {
                cols += columns[i];
                if (i < columns.Length - 1) cols += ", ";
            }
            string vals = "";
            for (int i = 0; i < values.Length; i++)
            {
                vals += "'"+values[i]+"'";
                if (i < values.Length - 1) vals += ", ";
            }
            string queryString = "INSERT INTO " + table + " (" + cols + ") VALUES (" + vals + ")";
            string result = "";
            using (MySqlConnection connection = new MySqlConnection(csb.ToString()))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Prepare();
                command.ExecuteNonQuery();
                result += "Successfully added query!";
            }
            Console.WriteLine(string.Format("{0}", result));

            return result;
        }

    }
}
