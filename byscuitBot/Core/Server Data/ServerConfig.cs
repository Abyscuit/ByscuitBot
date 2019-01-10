
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core.Server_Data
{
    public class ServerConfig
    {
        public string DiscordServerName { get; set; }
        public ulong DiscordServerID { get; set; }
        public ulong AFKChannelID { get; set; }
        public string AFKChannelName { get; set; }
        public int AFKTimeout { get; set; }
        public string Prefix { get; set; }
        public int EmbedColorRed { get; set; }
        public int EmbedColorGreen { get; set; }
        public int EmbedColorBlue { get; set; }
        public bool TimeStamp { get; set; }
        public string FooterText { get; set; }
        public string IconURL { get; set; }
        public ulong DefaultChannelID { get; set; }
        public string DefaultChannelName { get; set; }
        public ulong[] WhiteListMining { get; set; }
        public ulong[] BlackListMeme { get; set; }
        public ulong[] WhiteListNSFW { get; set; }
        public ulong[] BlackListCMDs { get; set; }
        public bool RequiresVerification { get; set; }
        public ulong VerificationRoleID { get; set; }
        public int AntiSpamThreshold { get; set; }
        public double AntiSpamTime { get; set; }
        public int AntiSpamWarn { get; set; }
        public bool AllowAdvertising { get; set; }
    }
}
