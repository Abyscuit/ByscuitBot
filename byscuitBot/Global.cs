using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static SocketGuild Guild { get; set; }
        internal static ulong MessageIdToTrack { get; set; }
        internal static bool giveaway { get; set; }
        internal static DateTime giveawayTime { get; set; }
        internal static bool selectYoutube { get; set; }
        internal static bool selectPS { get; set; }
        internal static List<string> vidIDs = new List<string>();
        internal static string[] emojies = { "👌", "💩", ":pepe:", "💯" };
        internal static int selectedEmoji = 0;

        internal static Random rand = new Random();

        

        //-----Welcome/Leave Messages-----
        internal static string[] Welcome =
        {
            "Welcome {0} to {1}!\n**Read the rules and enjoy your stay!**",
            "**Yo {0} just joined {1}!**\nBe cool and chill!",
            "**Everybody!**\n{0} is now in {1}!",
            "Where's the beer?\n__{0}__ just joined **{1}**!",
            "{0} just slid into {1}\n**Enjoy your stay!**"
        };
        internal static string[] Bye =
        {
            "__Bye {0}__\n**Too good for {1}?**",
            "**Hmm, {0} just left {1}!**\nOh well...",
            "**See ya later dude!**\n{0} decided to leave {1}!",
            "{0} slid out the server {1}\nEhhh..."
        };

        #region Steam API
        //----Steam API------
        internal static string API_KEY = "";
        internal static string csgoURL = "http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=730&key="+API_KEY+"&steamid=";
        internal static string resolveURL = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + API_KEY + "&vanityurl=";
        internal static string bansURL = "http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + API_KEY + "&steamids=";
        internal static string accountSummaryURL = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + API_KEY + "&steamids=";
        internal static string gamesURL = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + API_KEY + "&include_appinfo=1&steamid=";
        internal static string ESEA = "https://play.esea.net/users/";
        #endregion


        //----Crypto----
        internal static string ETH_SCAN_KEY = "";
        internal static string CMC_API_KEY = "";
        internal static string nanopoolGeneralInfo = "https://api.nanopool.org/v1/eth/user/";
        internal static string etherscanBalance = "https://api.etherscan.io/api?module=account&action=balance&tag=latest&apikey=" + ETH_SCAN_KEY + "&address=";
        internal static string ethtokenBalance = "http://api.etherscan.io/api?module=account&action=tokentx&startblock=0&endblock=999999999&sort=asc&apikey=" + ETH_SCAN_KEY + "&address=";
        internal static string ethPrice = "https://api.nanopool.org/v1/eth/prices";
        internal static string top10crypto = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest?sort=market_cap&start=1&limit=10&convert=USD&CMC_PRO_API_KEY=" + CMC_API_KEY;
        internal static string calceth = "https://api.nanopool.org/v1/eth/approximated_earnings/";
        internal static string ethtokenvalue = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?convert=USD&CMC_PRO_API_KEY=" + CMC_API_KEY + "&symbol=";


        //------Meme Track-------
        internal static List<string> MemesUsed = new List<string>();

        public static void PrintMsg(string username, string text)
        {
            Console.WriteLine(DateTime.Now + " | " + username+ " used " + text);
        }
    }
}
