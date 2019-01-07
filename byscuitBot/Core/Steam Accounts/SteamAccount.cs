using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core.Steam_Accounts
{
    public class SteamAccount
    {
        public string DiscordUsername { get; set; }
        public ulong DiscordID { get; set; }
        public ulong SteamID { get; set; }
        public string SteamUsername { get; set; }
    }
}
