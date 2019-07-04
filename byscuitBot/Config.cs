using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace byscuitBot
{
    public class Config
    {
        private const string CONFIG_FOLDER = "Resources";
        private const string CONFIG_FILE = "config.json";
        private const string CONFIG_PATH = CONFIG_FOLDER + "/" + CONFIG_FILE;
        public static BotConfig botconf;

        static Config()
        {
            if (!Directory.Exists(CONFIG_FOLDER))
                Directory.CreateDirectory(CONFIG_FOLDER);

            if (!File.Exists(CONFIG_PATH))
            {
                botconf = new BotConfig();
                botconf.botStatus = "";
                botconf.cmdPrefix = "-";
                botconf.token = "";
                botconf.CMC_API_KEY = "";
                botconf.ETH_SCAN_API_KEY = "";
                botconf.STEAM_API_KEY = "";
                botconf.TWITCH_APi_KEY = "";
                string json = JsonConvert.SerializeObject(botconf, Formatting.Indented);
                File.WriteAllText(CONFIG_PATH, json);
            }
            else
            {
                string json = File.ReadAllText(CONFIG_PATH);
                var data = JsonConvert.DeserializeObject<BotConfig>(json);
                botconf = data;
            }
        }
        public static void Save()
        {
            string json = JsonConvert.SerializeObject(botconf, Formatting.Indented);
            File.WriteAllText(CONFIG_PATH, json);
        }
    }
    public struct BotConfig
    {
        public string token;
        public string cmdPrefix;
        public string botStatus;
        public string CMC_API_KEY;
        public string ETH_SCAN_API_KEY;
        public string STEAM_API_KEY;
        public string TWITCH_APi_KEY;
        public string GOOGLE_API_KEY;
    }
}
