using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class Log
    {
        static string currentFolder = DateTime.Now.ToString("yyyy-M-d");
        static string storageFile = string.Format("Log/{0}/{1}.txt", currentFolder, DateTime.Now.ToString("H-mm-ss"));
        static string errorFile = string.Format("Log/{0}/{1}.txt", currentFolder, DateTime.Now.ToString("H-mm-ss") + ".error");
        static List<string> lines = new List<string>();
        static List<string> errors = new List<string>();
        static Log()
        {
            if (!ValidateStorageFile(storageFile)) return;
            if (!ValidateStorageFile(errorFile)) return;
            //Load Previous Logs?
        }

        public static void SaveData()
        {
            try
            {
                File.WriteAllText(storageFile, DisplayLog());
            }
            catch(Exception ex)
            {
                string str = DateTime.Now + " | EXCEPTION: " + ex.Message;
                Console.WriteLine(str);
                lines.Add(str);
                errors.Add(str);
            }
        }

        public static void AddTextToLog(string text)
        {
            lines.Add(text);
            SaveData();
        }
        public static void LogException(string text)
        {
            lines.Add(text);
            errors.Add(text);
            SaveData();
        }

        public static string DisplayLog()
        {
            string result = "";
            try
            {
                for (int i = 0; i < lines.Count; i++)
                    result += lines[i] + "\n";
            }
            catch(Exception ex)
            {
                string str = DateTime.Now + " | EXCEPTION: " + ex.Message;
                Console.WriteLine(str);
                lines.Add(str);
                errors.Add(str);
            }
            return result;
        }

        private static bool ValidateStorageFile(string file)
        {
            if (!Directory.Exists("Log")) Directory.CreateDirectory("Log");
            if (!Directory.Exists("Log/" + currentFolder)) Directory.CreateDirectory("Log/" + currentFolder);
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }

            return true;
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }

    }
}
