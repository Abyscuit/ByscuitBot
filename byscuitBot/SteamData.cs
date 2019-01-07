using byscuitBot.Core.Steam_Accounts;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static byscuitBot.Core.Steam_Accounts.SteamAccounts;

namespace byscuitBot
{
    public class SteamData
    {

        //public static Dictionary<string, string> pairs = new Dictionary<string, string>();
        public static List<CsgoLastMatches> matches = new List<CsgoLastMatches>();
        static string storageFile = "SteamData.json";
        static SteamData()
        {
            if (!ValidateStorageFile(storageFile)) return;
            string json = File.ReadAllText(storageFile);
            matches = JsonConvert.DeserializeObject<List<CsgoLastMatches>>(json);
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(matches, Formatting.Indented);
            File.WriteAllText(storageFile, json);
        }
        

        public static void AddLastMatchesToStorage(CsgoLastMatches lastMatches)
        {
            bool accountActive = false;
            int index = 0;
            
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].discordid == lastMatches.discordid)
                {
                    accountActive = true;
                    index = i;
                    break;
                }
            }
            if (accountActive)
            {
                bool isAvailable = true;
                if (matches[index].steamid == lastMatches.steamid)
                {
                    List<CsgoLastMatch> list = matches[index].lastmatches;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].id == lastMatches.lastmatches[0].id)
                        {
                            isAvailable = false;
                        }
                    }
                    if(isAvailable)
                        matches[index].lastmatches.Add(lastMatches.lastmatches[0]);
                }
            }
            else
                matches.Add(lastMatches);

            SaveData();
        }

        public static int GetCount()
        {
            return matches.Count;
        }
        public static CsgoLastMatches GetMatches(ulong id)
        {
            IEnumerable<CsgoLastMatches> result = from a in matches
                                    where a.steamid == id
                                    select a;
            CsgoLastMatches match = result.FirstOrDefault();

            return match;

        }
        private static bool ValidateStorageFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }

            return true;
        }

        public static void Save(IEnumerable<SteamAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<SteamAccount> Load(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<SteamAccount>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
