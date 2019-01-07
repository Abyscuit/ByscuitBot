using byscuitBot.Core.User_Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using byscuitBot.Core.Server_Data;

namespace byscuitBot.Core
{
    public class DataStorage
    {
        public static Dictionary<string, string> pairs = new Dictionary<string, string>();
        static string storageFile = "DatStorage.json";
        static DataStorage()
        {
            if (!ValidateStorageFile(storageFile)) return;
            string json = File.ReadAllText(storageFile);
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText(storageFile, json);
        }

        public static void AddPairToStorage(string key, string value)
        {
            if (pairs.ContainsKey(key))
                pairs.Remove(key);

            pairs.Add(key, value);
            SaveData();
        }

        public static int GetPairCount()
        {
            return pairs.Count;
        }

        private static bool ValidateStorageFile(string file)
        {
            if(!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }

            return true;
        }

        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static void SaveServerConfig(IEnumerable<ServerConfig> serverConfigs, string filePath)
        {
            string json = JsonConvert.SerializeObject(serverConfigs, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<ServerConfig> LoadServerConfigs(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ServerConfig>>(json);
        }

        public static void SaveNanoPool(IEnumerable<NanoPool.UserAccount> userAccounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(userAccounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<NanoPool.UserAccount> LoadNanoPool(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<NanoPool.UserAccount>>(json);
        }

        public static void SaveMemes(IEnumerable<Meme> memes, string filePath)
        {
            string json = JsonConvert.SerializeObject(memes, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<Meme> LoadMemes(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Meme>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
