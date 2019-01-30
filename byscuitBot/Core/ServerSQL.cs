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
        public static string Select(string table, string where, string equals)
        {
            string queryString = "SELECT * FROM  " + table + " WHERE " + where + " = @equals";
            string result = "";
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
                        result = string.Format("**ID:** {0}\n**CPUKey:** {1}\n**Salt:** {2}\n**Time:** {3}\n**Enabled:** {4}",
                            reader["id"], reader["cpukey"], reader["salt"], totalTime, reader["enabled"]); 
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
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
                //MySqlDataReader reader = command.ExecuteReader();
                /*
                try
                {
                    while (reader.Read())
                    {
                        foreach(string s in columns)
                        {
                            string str = s + ": " + reader[s];
                            result += str;
                            Console.WriteLine(str);
                        }
                        //result = string.Format("**ID:** {0}\n**uLogin:** {1}\n**uHash:** {2}\n**uType:** {3}",
                          //  reader["id"], reader["uLogin"], reader["uHash"], reader["uType"]);
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
                */
            }
            Console.WriteLine(string.Format("{0}", result));

            return result;
        }
    }
}
