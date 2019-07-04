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
            "Welcome **{0}** to __{1}__! Read the rules and enjoy your stay!",
            "Yo **{0}** just joined __{1}__! Be cool and chill!",
            "Everybody! **{0}** is now in __{1}__!",
            "Where's the beer? **{0}** just joined __{1}__!",
            "**{0}** just slid into __{1}__!"
        };
        internal static string[] Bye =
        {
            "Bye **{0}**! Too good for __{1}__?",
            "Hmm, **{0}** just left __{1}__! Oh well...",
            "See ya later dude! **{0}** decided to leave __{1}__!",
            "**{0}** slid out the server __{1}__"
        };

        internal static string[] Holidays =
        {
            "🎂**{0}** just turned **{1:N0}** years old! Happy Birthday!🎂",
            "🧨🎇🎆Happy Fourth of July!🎆🎇🧨\n**Be Safe!**",
            "👻🎃Happy Halloween Everyone!🎃👻",
            "🍗🦃Happy Thanksgiving Everyone!🦃🍗",
            "🎄🎅Merry Christmas Everyone!🎅🎄",
            "🍾🎉Happy New Year!🎉🍾"
        };

        #region Steam API
        //----Steam API------
        internal static string API_KEY = Config.botconf.STEAM_API_KEY;
        internal static string csgoURL = "http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=730&key="+API_KEY+"&steamid=";
        internal static string resolveURL = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + API_KEY + "&vanityurl=";
        internal static string bansURL = "http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + API_KEY + "&steamids=";
        internal static string accountSummaryURL = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + API_KEY + "&steamids=";
        internal static string gamesURL = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + API_KEY + "&include_appinfo=1&steamid=";
        internal static string ESEA = "https://play.esea.net/users/";
        #endregion


        //----Crypto----
        internal static string ETH_SCAN_KEY = Config.botconf.ETH_SCAN_API_KEY;
        internal static string CMC_API_KEY = Config.botconf.CMC_API_KEY;
        internal static string nanopoolGeneralInfo = "https://api.nanopool.org/v1/eth/user/";
        internal static string nanopoolWorkers = "https://api.nanopool.org/v1/eth/workers/";
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

        //------Xbox Stuff------
        public static string ConvertIntToHex(uint val)
        {
            return val.ToString("X");
        }
        internal static string[] GAME_HEX = { "FFFE07D1", "41560855", "4156081C", "415608C3",
            "4156091D", "41560929", "415607E6", "41560817", "415608CB", "41560914", "415608FC", "545408A7", "464F0803",
        "425307E6", "5841149E", "FFFE07DE","41560927","41560928","58411457","415608F8","454108E6","545408B8","45410950","454109BA","584111F7","58410A95","58480880",
        "5848085B", "545408B0", "58411420","315A07D1","423607D3","4B4E085E","4A3707D1","534307DB","464F0800","53510804","5841125A","545407F2","5841128F"};

        internal static string[] GAME_TITLE = { "[Dashboard]", "[Call of Duty®: WaW]", "[Black Ops I]", "[Black Ops II]",
            "[Black Ops III]", "[Black Ops III Bundle]", "[Modern Warfare I]", "[Modern Warfare II]", "[Modern Warfare III]", "[Advanced Warfare]", "[Ghosts]", "[Grand Theft Auto V]", "[Sniper Elite 3]",
        "[Skyrim]", "[Minecraft: Story Mode]", "[Account Creation Tool]","[Destiny: Legendary Edition]","[Destiny: Collectors Edition']","[MONOPOLY PLUS]","[Destiny]","[Skate 3]","[Grand Theft Auto: San Andreas]",
            "[Battlefield 3™]","[Battlefield 4™']","[Minecraft: Xbox 360 Edition]","[Iron Brigade]","[Internet Explorer]", "[Xbox Music and Video]", "[NBA 2K14]", "[Rekoil: Liberator]","['NBA 2K16]",
            "[YouTube]","[METAL GEAR SOLID V: THE PHANTOM PAIN]","[Crunchyroll]","[Hitman: Blood Money]","[PAYDAY 2]", "[Hitman: Absolution]","[Counter-Strike: GO]","[Grand Theft Auto IV]","[Terraria – Xbox 360 Edition]" };
        
        //My method
        public static string getTitle(string title)
        {
            for(int i =0;i<GAME_HEX.Length;i++) if (title == GAME_HEX[i]) return GAME_TITLE[i];
            return "No Game Detected";
        }
        /* Levi/king? (German fotze) arsch method
        public static string getTitle(string title)
        {
            string game = "No Game Detected";
            switch (title)
            {
                case "FFFE07D1":
                    game = "[Dashboard]";
                    break;
                case "41560855":
                    game = "[Call of Duty®: WaW]";
                    break;
                case "4156081C":
                    game = "[Black Ops I]";
                    break;
                case "415608C3":
                    game = "[Black Ops II]";
                    break;
                case "4156091D":
                    game = "[Black Ops III]";
                    break;
                case "41560929":
                    game = "[Black Ops III Bundle]";
                    break;
                case "415607E6":
                    game = "[Modern Warfare I]";
                    break;
                case "41560817":
                    game = "[Modern Warfare II]";
                    break;
                case "415608CB":
                    game = "[Modern Warfare III]";
                    break;
                case "41560914":
                    game = "[Advanced Warfare]";
                    break;
                case "415608FC":
                    game = "[Ghosts]";
                    break;
                case "545408A7":
                    game = "[ Grand Theft Auto V]";
                    break;
                case "464F0803":
                    game = "[Sniper Elite 3]";
                    break;
                case "425307E6":
                    game = "[Skyrim]";
                    break;
                case "5841149E":
                    game = "[Minecraft: Story Mode]";
                    break;
                case "FFFE07DE":
                    game = "[Account Creation Tool]";
                    break;
                case "41560927":
                    game = "[Destiny: Legendary Edition]";
                    break;
                case "41560928":
                    game = "[Destiny: Collectors Edition']";
                    break;
                case "58411457":
                    game = "[MONOPOLY PLUS]";
                    break;
                case "415608F8":
                    game = "[Destiny]";
                    break;
                case "454108E6":
                    game = "[Skate 3]";
                    break;
                case "545408B8":
                    game = "[Grand Theft Auto: San Andreas]";
                    break;
                case "45410950":
                    game = "[Battlefield 3™]";
                    break;
                case "454109BA":
                    game = "[Battlefield 4™']";
                    break;
                case "584111F7":
                    game = "[Minecraft: Xbox 360 Edition]";
                    break;
                case "58410A95":
                    game = "[Iron Brigade]";
                    break;
                case "58480880":
                    game = "[Internet Explorer]";
                    break;
                case "5848085B":
                    game = "[Xbox Music and Video]";
                    break;
                case "545408B0":
                    game = "[NBA 2K14]";
                    break;
                case "58411420":
                    game = "[Rekoil: Liberator]";
                    break;
                case "315A07D1":
                    game = "['NBA 2K16]";
                    break;
                case "423607D3":
                    game = "[YouTube]";
                    break;
                case "4B4E085E":
                    game = "[METAL GEAR SOLID V: THE PHANTOM PAIN]";
                    break;
                case "4A3707D1":
                    game = "[Crunchyroll]";
                    break;
                case "534307db":
                    game = "[Hitman: Blood Money]";
                    break;
                case "464f0800":
                    game = "[PAYDAY 2]";
                    break;
                case "53510804":
                    game = "[Hitman: Absolution]";
                    break;
                case "5841125A":
                    game = "[Counter-Strike: GO]";
                    break;
                case "545407F2":
                    game = "[Grand Theft Auto IV]";
                    break;
                case "5841128F":
                    game = "[Terraria – Xbox 360 Edition]";
                    break;
            }
            return game;
        }
        */
    }
}
