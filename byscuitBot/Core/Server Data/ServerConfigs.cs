using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core.Server_Data
{
    class ServerConfigs
    {
        private static List<ServerConfig> configs = new List<ServerConfig>();
        private static string configFile = "Resources/server_configs.json";

        public ServerConfigs()
        {

        }

        public static void LoadServerConfigs()
        {
            if (DataStorage.SaveExists(configFile))
            {
                configs = DataStorage.LoadServerConfigs(configFile).ToList();
            }
            else
            {
                configs = new List<ServerConfig>();
                SaveAccounts();
            }

        }

        public static void SaveAccounts()
        {
            DataStorage.SaveServerConfig(configs, configFile);
        }

        public static ServerConfig GetConfig(SocketGuild guild)
        {
            return GetOrCreateAcount(guild);
        }

        private static ServerConfig GetOrCreateAcount(SocketGuild socketGuild)
        {
            IEnumerable<ServerConfig> result = from a in configs
                                              where a.DiscordServerID == socketGuild.Id
                                              select a;

            ServerConfig guild = result.FirstOrDefault();
            if (guild == null) guild = CreateAccount(socketGuild);

            //return account
            return guild;
        }

        private static ServerConfig CreateAccount(SocketGuild guild)
        {
            ulong dChanID = 0;
            ulong afkChanID = 0;
            string IconUrl = "";
            string AFKChanName = "";
            string DefaultChanName = "";
            if (guild.DefaultChannel != null)
            {
                dChanID = guild.DefaultChannel.Id;
                DefaultChanName = guild.DefaultChannel.Name;
            }
            if (guild.AFKChannel != null)
            {
                afkChanID = guild.AFKChannel.Id;
                AFKChanName = guild.AFKChannel.Name;
            }
            if (guild.IconUrl != null)
                IconUrl = guild.IconUrl;
            ServerConfig newConfig = new ServerConfig()
            {
                DiscordServerID = guild.Id,
                DiscordServerName = guild.Name,
                FooterText = "Created By Abyscuit",
                Prefix = Config.botconf.cmdPrefix,
                EmbedColorRed = 100,
                EmbedColorGreen = 150,
                EmbedColorBlue = 255,
                TimeStamp = false,
                AFKChannelID = afkChanID,
                IconURL = IconUrl,
                AFKTimeout = guild.AFKTimeout,
                DefaultChannelID = dChanID,
                AFKChannelName = AFKChanName,
                DefaultChannelName = DefaultChanName,
                WhiteListMining = null,
                WhiteListNSFW = null,
                BlackListCMDs = null,
                BlackListMeme = null,
                RequiresVerification = false,
                AntiSpamThreshold = 5,
                AntiSpamTime = 10,
                AntiSpamWarn = 3
            };

            configs.Add(newConfig);
            SaveAccounts();
            return newConfig;
        }

        public static void UpdateServerConfig(SocketGuild guild, ServerConfig config)
        {
            ServerConfig serverConfig = GetOrCreateAcount(guild);
            configs.Remove(serverConfig);
            configs.Add(config);
            SaveAccounts();
        }

        //Finish blacklist for CMDs and Whitelist
        //When you wake up
    }
}
