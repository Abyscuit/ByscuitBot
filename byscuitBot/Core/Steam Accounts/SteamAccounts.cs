using byscuitBot.Modules;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core.Steam_Accounts
{
    public class SteamAccounts
    {
        private static List<SteamAccount> accounts = new List<SteamAccount>();
        private static string accountFile = "Resources/SteamAccounts.json";

        public SteamAccounts()
        {

        }

        public static void LoadUserAccts()
        {
            if (DataStorage.SaveExists(accountFile))
            {
                accounts = SteamData.Load(accountFile).ToList();
            }
            else
            {
                accounts = new List<SteamAccount>();
                SaveAccounts();
            }

        }

        public static void SaveAccounts()
        {
            SteamData.Save(accounts, accountFile);
        }

        public static SteamAccount LinkAccount(SocketUser user, ulong steamID)
        {
            return GetOrCreateAcount(steamID, user.Id, user.Username);
        }

        public static SteamAccount GetAccount(SocketUser user)
        {
            IEnumerable<SteamAccount> result = from a in accounts
                                               where a.DiscordID == user.Id
                                               select a;

            SteamAccount account = result.FirstOrDefault();

            //return account
            return account;
        }

        public static SteamAccount[] GetAllAccounts()
        {
            return accounts.ToArray();
        }

        private static SteamAccount GetOrCreateAcount(ulong steamID, ulong discordID, string discordName)
        {
            IEnumerable<SteamAccount> result = from a in accounts
                                              where a.SteamID == steamID
                                              select a;

            SteamAccount account = result.FirstOrDefault();
            if (account == null) account = CreateAccount(steamID, discordID, discordName);

            //return account
            return account;
        }

        private static SteamAccount CreateAccount(ulong steamID, ulong discordID, string discordUsername)
        {

            SteamAccount newAcct = new SteamAccount()
            {
                SteamID = steamID,
                DiscordID = discordID,
                DiscordUsername = discordUsername,
                SteamUsername = GetAccountObject(steamID).personaname
            };

            accounts.Add(newAcct);
            SaveAccounts();
            return newAcct;
        }

        #region Steam Account Info

        public static string ResolveAccount(string steamUsername)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.resolveURL + steamUsername);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            //Console.WriteLine("Data return " + data);
            Dictionary<string, Dictionary<string, string>> r = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(data);
            string steamID = "";
            if (r["response"].ContainsKey("steamid"))
            {
                steamID = r["response"]["steamid"];
                Console.WriteLine("SteamID: " + steamID);
            }
            else
            {
                steamID = r["response"]["message"];
            }
            return steamID;
        }


        public static string GetAccountSummary(ulong SteamID)
        {
            PlayerSummary account = GetAccountObject(SteamID);

            string username = account.personaname;
            ulong steamid = account.steamid;
            string realName = account.realname;
            string avatarURL = account.avatarfull;
            string personastate = account.personastate.ToString();
            string communityvisibilitystate = account.communityvisibilitystate.ToString();
            string profileURL = account.profileurl;
            string timecreated = account.timecreated.ToString();
            string lastlogoff = account.lastlogoff.ToString();
            string countrycode = account.loccountrycode;
            string statecode = account.locstatecode;
            string cityid = account.loccityid;
            string clanid = account.primaryclanid.ToString();
            string commentpermission = account.commentpermission.ToString();
            /*
            Console.WriteLine("Username: " + username);
            Console.WriteLine("Real Name: " + realName);
            Console.WriteLine("Avatar URL: " + avatarURL);
            Console.WriteLine("Persona State: " + personastate);
            Console.WriteLine("Community State: " + communityvisibilitystate);
            Console.WriteLine("Profile URL: " + profileURL);
            Console.WriteLine("Time Created: " + timecreated);
            Console.WriteLine("Last Logoff: " + lastlogoff);
            Console.WriteLine("Country: " + countrycode);
            Console.WriteLine("State: " + statecode);
            Console.WriteLine("City ID: " + cityid);
            Console.WriteLine("Clan ID: " + clanid);
            Console.WriteLine("SteamID: " + SteamID);
            Console.WriteLine(CheckInGame(account));*/
            string msg = "**Username:** " + username + "\n**Steam ID:** " + steamid + "\n**Real Name:** " + realName + "\n**Online State:** " + SetPersonaState(int.Parse(personastate)) + "\n" + 
                "**Time Created:** " + UnixTimeStampToDateTime(double.Parse(timecreated)) + "\n**Last Logoff:** " +
                UnixTimeStampToDateTime(double.Parse(lastlogoff)) + "\n**Primary Clan ID:** " + clanid + "\n**Community Visibility State:** " + SetCommunityState(int.Parse(communityvisibilitystate)) +
                "\n**Allows Public Profile Comments:** " + commentpermission + "\n**Country:** " + countrycode + "\n**State:** " + statecode + "\n**City ID:** " + cityid +"\n**Profile URL:** " +
                profileURL + "\n**Avatar URL:** " + avatarURL + CheckInGame(account);
                
            /*
            if (objData[0][""].ContainsKey("steamid"))
            {
                steamID = r["response"]["steamid"];
                Console.WriteLine("SteamID: " + steamID);
            }
            else
            {
                steamID = r["response"]["message"];
            }*/
            return msg;
        }
        public static PlayerSummary GetAccountObject(ulong steamID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.accountSummaryURL + steamID);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            //Console.WriteLine("Data return " + data);
            Dictionary<string, Dictionary<string, List<PlayerSummary>>> r = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<PlayerSummary>>>>(data);
            //string steamID = "";
            List<PlayerSummary> objData = r["response"]["players"];
            return objData[0];
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public class PlayerSummary
        {
            //Public Data
            public ulong steamid;
            public string personaname;
            public string profileurl;
            public string avatar, avatarmedium, avatarfull;
            public int personastate;
            public int communityvisibilitystate;
            public int profilestate;
            public ulong lastlogoff;
            public bool commentpermission;

            //Private Data
            public string realname;
            public ulong primaryclanid;
            public ulong timecreated;
            public uint gameid;
            public string gameserverip;
            public string extragameinfo;
            public string loccountrycode = "Private";
            public string locstatecode = "Private";
            public string loccityid = "Private";
        }

        public static string SetPersonaState(int state)
        {
            string status = "Offline/Private";
            switch (state)
            {
                case 0:
                    {
                        status = "Offline/Private";
                        break;
                    }
                case 1:
                    {
                        status = "Online";
                        break;
                    }
                case 2:
                    {
                        status = "Busy";
                        break;
                    }
                case 3:
                    {
                        status = "Away";
                        break;
                    }
                case 4:
                    {
                        status = "Snooze";
                        break;
                    }
                case 5:
                    {
                        status = "Looking To Trade";
                        break;
                    }
                case 6:
                    {
                        status = "Looking To Play";
                        break;
                    }
            }

            return status;
        }

        public static string SetCommunityState(int state)
        {
            string status = "Offline/Private";
            switch (state)
            {
                case 1:
                    {
                        status = "Private/Friends Only";
                        break;
                    }
                case 3:
                    {
                        status = "Public";
                        break;
                    }
            }

            return status;
        }

        public static BanPlayer GetAccountBans(ulong steamID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.bansURL + steamID);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            //Console.WriteLine("Data return " + data);
            Dictionary<string, List<BanPlayer>> r = JsonConvert.DeserializeObject<Dictionary<string, List<BanPlayer>>>(data);
            //string steamID = "";
            List<BanPlayer> objData = r["players"];
            /*
            Console.WriteLine("Steam ID: " + objData[0].SteamId);
            Console.WriteLine("VAC Banned: " + objData[0].VACBanned);
            Console.WriteLine("Number of VAC Bans: " + objData[0].NumberOfVACBans);
            Console.WriteLine("Number of Game Bans: " + objData[0].NumberOfGameBans);
            Console.WriteLine("Community Banned: " + objData[0].CommunityBanned);
            Console.WriteLine("Economy  Banned: " + objData[0].EconomyBan);
            Console.WriteLine("Days Since Last Ban: " + objData[0].DaysSinceLastBan);
            */
            return objData[0];
        }

        public class BanPlayer
        {
            public ulong SteamId;
            public bool CommunityBanned;
            public bool VACBanned;
            public int NumberOfVACBans;
            public int DaysSinceLastBan;
            public int NumberOfGameBans;
            public string EconomyBan;
        }

        public static GameResponse GetAccountGames(ulong steamID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.gamesURL + steamID);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            //Console.WriteLine("Data return " + data);
            Dictionary<string, GameResponse> r = JsonConvert.DeserializeObject<Dictionary<string, GameResponse>>(data);
            //string steamID = "";
            GameResponse objData = r["response"];
            /*
            Console.WriteLine("Game Count: " + objData.game_count);
            for(int i =0;i<objData.games.Count;i++)
            {
                Console.WriteLine("Game appID: " + objData.games[i].appid);
                Console.WriteLine("Game Name: " + objData.games[i].name);
                Console.WriteLine("Game Image Icon Hash: " + objData.games[i].img_icon_url);
                Console.WriteLine("Game Image Logo Hash: " + objData.games[i].img_logo_url);
                Console.WriteLine("Playtime Total: " + objData.games[i].playtime_forever);
                if(objData.games[i].playtime_2weeks != null)
                    Console.WriteLine("Playtime Last 2 Weeks: " + objData.games[i].appid +"\n");
            }*/
            return objData;
        }

        public static string CheckInGame(PlayerSummary account)
        {
            if (account.personastate != 0 && account.gameid != 0)
            {
                    string msg = "\n\n**Currently Playing:** " + account.extragameinfo +
                        "\n**Game ID:** " + account.gameid;
                GameResponse r = GetAccountGames(account.steamid);
                foreach(Games game in r.games)
                {
                    if (game.appid == account.gameid)
                    {
                        msg = "\n\n**Currently Playing:** " + game.name +
                            "\n**Game ID:** " + account.gameid;
                        break;
                    }
                }
                if (account.gameserverip != null)
                    msg += "\n**Game Server IP:** " + account.gameserverip;
                return msg;
            }
            return "";
        }


        public class GameResponse
        {
            public uint game_count;
            public List<Games> games;
        }
        public class Games
        {
            public uint appid;
            public string name;
            public string img_icon_url;
            public string img_logo_url;
            public int? playtime_2weeks;
            public uint playtime_forever;
        }

        public static string gameImgUrl(int appID, string urlHash)
        {
            string url = "http://media.steampowered.com/steamcommunity/public/images/apps/" + appID + "/" + urlHash + ".jpg";
            return url;
        }
        #endregion

        #region CSGO Info


        public static string GetCSGOStats(ulong SteamID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.csgoURL + SteamID);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            Dictionary<string, PlayerStats> r = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(data);
            string msg = "";
            PlayerStats stats = r["playerstats"];
            string[] wantedStats = { "total_kills", "total_deaths", "total_kills_headshot", "total_shots_hit", "total_shots_fired", "total_time_played", "total_wins", "total_rounds_played",
                "total_mvps", "total_matches_won", "total_matches_played", "total_damage_done", "total_money_earned", "total_rescued_hostages", "total_kills_knife", "total_kills_knife_fight",
                "total_kills_enemy_weapon", "total_wins_pistolround", "total_weapons_donated", "total_kills_enemy_blinded", "total_kills_against_zoomed_sniper", "total_dominations",
                "total_domination_overkills", "total_revenges", "total_planted_bombs", "total_defused_bombs", "total_contribution_score"};

            string[] formattedStats = { "Total Kills", "Total Deaths", "Total Headshots", "Total Shots Hit", "Total Shots Fired", "Total Time Played",  "Total Round Wins", "Total Rounds Played",
                "Total MVPs", "Total Matches Won", "Total Matches Played", "Total Damage Done", "Total Money Earned", "Total Rescued Hostages", "Total Knife Kills", "Total Kills Knife Fight",
                "Total Kills With Enemy Weapon", "Total Wins Pistol Round", "Total Weapons Donated", "Total Kills Enemy Blinded", "Total Kills Against Zoomed Sniper", "Total Dominations",
                "Total Domination Overkills", "Total Revenges", "Total Planted Bombs", "Total Defused Bombs", "Total Contribution Score"};

            int max = stats.stats.Count - 1;
            float kills = 0;
            float deaths = 0;
            float KD = 0;
            float rWins = 0;
            float rLosses = 0;
            float rWinRatio = 0;
            float mWins = 0;
            float mLosses = 0;
            float mWinRatio = 0;
            float shotsHit = 0;
            float shotsFired = 0;
            float accuracy = 0;
            float headshots = 0;
            float headshotPercent = 0;
            for (int x = 0; x < wantedStats.Length; x++)
            {
                for (int i = 0; i < stats.stats.Count; i++)
                {
                    Stats curStat = stats.stats[i];
                    if (wantedStats[x] == curStat.name)
                    {
                        string value = string.Format("{0:N0}", curStat.value);
                        string name = formattedStats[x];

                        if (name == "Total Kills")
                            kills = curStat.value;

                        if (name == "Total Round Wins")
                            rWins = curStat.value;

                        if (name == "Total Matches Won")
                            mWins = curStat.value;

                        if (name == "Total Shots Hit")
                            shotsHit = curStat.value;

                        if (name == "Total Money Earned")
                            value = string.Format("${0:N0}", curStat.value);

                        if (name == "Total Time Played")
                            value = string.Format("{0:N2}hrs", float.Parse(curStat.value.ToString()) / 60 / 60);

                        msg += "**" + name + "**: " + value + "\n";
                        if (name == "Total Deaths")
                        {
                            deaths = curStat.value;
                            KD = kills / deaths;
                            msg += "**Total K/D Ratio**: " + string.Format("{0:N2}", KD) + "\n";
                        }
                        if (name == "Total Headshots")
                        {
                            headshots = curStat.value;
                            headshotPercent = headshots / kills * 100f;
                            msg += "**Total Headshot Percent**: " + string.Format("{0:N2}%", headshotPercent) + "\n";
                        }
                        if (name == "Total Rounds Played")
                        {
                            rLosses = curStat.value - rWins;
                            rWinRatio = rWins / rLosses;
                            msg += "**Total Round Win Ratio**: " + string.Format("{0:N2}", rWinRatio) + "\n";
                        }
                        if (name == "Total Matches Played")
                        {
                            mLosses = curStat.value - mWins;
                            mWinRatio = mWins / mLosses;
                            msg += "**Total Match Win Ratio**: " + string.Format("{0:N2}", mWinRatio) + "\n";
                        }
                        if (name == "Total Shots Fired")
                        {
                            shotsFired = curStat.value;
                            accuracy = shotsHit / shotsFired * 100f;
                            msg += "**Accuracy**: " + string.Format("{0:N2}%", accuracy) + "\n";
                        }
                    }
                }
            }
            return msg;
        }

        public static CsgoLastMatch GetCSGOLastMatch(SocketUser user, ulong SteamID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.csgoURL + SteamID);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            Dictionary<string, PlayerStats> r = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(data);
            string msg = "";
            PlayerStats stats = r["playerstats"];
            string[] wantedStats = { "last_match_rounds", "last_match_contribution_score", "last_match_wins", "last_match_t_wins", "last_match_ct_wins", "last_match_kills",
            "last_match_deaths", "last_match_mvps", "last_match_damage", "last_match_money_spent", "last_match_dominations", "last_match_revenges", "last_match_favweapon_id",
             "last_match_favweapon_shots", "last_match_favweapon_hits", "last_match_favweapon_kills"};

            string[] formattedStats = { "Rounds", "Contribution Score", "Wins", "Terrorist Wins", "Counter-Terrorist Wins", "Kills",
            "Deaths", "MVPs", "Damage", "Money Spent", "Dominations", "Revenges", "Favorite Weapon",
             "Favorite Weapon Shots", "Favorite Weapon Hits", "Favorite Weapon Kills"};

            int max = stats.stats.Count - 1;
            float kills = 0;
            float deaths = 0;
            float KD = 0;
            float shots = 0;
            float hits = 0;
            float accuracy = 0;
            float rounds = 0;
            SteamAccount steamaccount = GetAccount(user);
            CsgoLastMatch lastmatch = new CsgoLastMatch();
            bool createNewMatch = false;
            if(steamaccount != null)
            {
                if (steamaccount.SteamID == SteamID && steamaccount.DiscordID == user.Id)
                {
                    createNewMatch = true;
                }
            }

            for (int x = 0; x < wantedStats.Length; x++)
            {
                for (int i = 0; i < stats.stats.Count; i++)
                {
                    Stats curStat = stats.stats[i];
                    if (wantedStats[x] == curStat.name)
                    {
                        string value = curStat.value.ToString();
                        string name = formattedStats[x];
                        
                        if (name == "Revenges")
                        {
                                lastmatch.revenges = (uint)curStat.value;
                        }
                        if (name == "Dominations")
                        {
                                lastmatch.dominations = (uint)curStat.value;
                        }
                        if (name == "Damage")
                        {
                                lastmatch.damage = (uint)curStat.value;
                        }
                        if (name == "MVPs")
                        {
                                lastmatch.mvps = (uint)curStat.value;
                        }
                        if (name == "Contribution Score")
                        {
                                lastmatch.contribution_score = (uint)curStat.value;
                        }
                        if (name == "Terrorist Wins")
                        {
                                lastmatch.t_wins = (uint)curStat.value;
                        }
                        if (name == "Counter-Terrorist Wins")
                        {
                                lastmatch.ct_wins = (uint)curStat.value;
                        }

                        if (name == "Rounds")
                        {
                            rounds = curStat.value;
                                lastmatch.rounds = (uint)curStat.value;
                        }

                        if (name == "Wins")
                        {
                            lastmatch.losses = uint.Parse((rounds - curStat.value) + "");
                            name = "Game Outcome";
                            if (curStat.value > lastmatch.losses)
                                value = "Won";
                            else if (curStat.value == lastmatch.losses)
                                value = "Draw";
                            else
                                value = "Lost";
                            
                                lastmatch.wins = (uint)curStat.value;
                                lastmatch.outcome = value;
                            //Console.WriteLine("Rounds Won:" + curStat.value);
                        }
                        if (name == "Money Spent")
                        {
                            value = string.Format("${0:N2}", float.Parse(value));
                                lastmatch.money_spent = (uint)curStat.value;
                        }

                        if (name == "Kills")
                        {
                            kills = float.Parse(value);
                                lastmatch.kills = (uint)curStat.value;
                        }

                        if (name == "Favorite Weapon Kills")
                        {
                                lastmatch.favweapon_kills = (uint)curStat.value;
                        }
                        if (name == "Favorite Weapon Shots")
                        {
                            shots = curStat.value;
                                lastmatch.favweapon_shots = (uint)curStat.value;
                        }
                        if (name == "Favorite Weapon")
                        {
                            value = getGunName(int.Parse(value));
                                lastmatch.favweapon_name = value;
                        }
                        msg += "**" + name + "**: " + value + "\n";
                        if (name == "Deaths")
                        {
                            deaths = float.Parse(value);
                            KD = kills / deaths;
                            msg += "**K/D Ratio**: " + string.Format("{0:N2}", KD) + "\n";
                                lastmatch.deaths = (uint)curStat.value;
                                lastmatch.kd = KD;
                        }
                        if(name == "Favorite Weapon Hits")
                        {
                            hits = curStat.value;
                            accuracy = hits / shots * 100f;
                            msg += string.Format("**Favorite Weapon Accuracy:** {0:N2}%\n", accuracy);
                                lastmatch.favweapon_hits = (uint)curStat.value;
                                lastmatch.accuracy = accuracy;
                        }
                    }
                }
            }
            if(createNewMatch)
            {
                CsgoLastMatches matches = new CsgoLastMatches();
                matches.steamid = SteamID;
                matches.discordid = user.Id;
                matches.username = user.Username;
                if (matches.lastmatches == null)
                    matches.lastmatches = new List<CsgoLastMatch>();
                for (int i =0;i< SteamData.matches.Count;i++)
                {
                    while (SteamData.matches[i].lastmatches.Count > 10)
                    {
                        SteamData.matches[i].lastmatches.RemoveAt(0);
                    }
                }
                lastmatch.createID();
                matches.lastmatches.Add(lastmatch);
                SteamData.AddLastMatchesToStorage(matches);
                Console.WriteLine("Created new match and Saved to User: "+user.Username);
            }
            //Console.WriteLine(msg);
            return lastmatch;
        }

        public class CsgoLastMatch
        {
            public uint rounds, contribution_score, wins, losses, t_wins, ct_wins, kills,
            deaths, mvps, damage, money_spent, dominations, revenges,
             favweapon_shots, favweapon_hits, favweapon_kills;
            public float accuracy, kd;
            public string outcome, favweapon_name;
            public string id;
            public string gametype;

            public void createID()
            {
                id = Misc.MD5Hash(Misc.Base64Encode(rounds + contribution_score + wins + t_wins + ct_wins + kills + deaths + mvps + damage + money_spent + dominations + revenges +
                    favweapon_shots + favweapon_hits + favweapon_kills + favweapon_name + accuracy + kd + outcome + id));
            }

            public void getGameType()
            {
            }
        }

        public class CsgoLastMatches
        {
            public ulong steamid;
            public string username;
            public ulong discordid;
            public List<CsgoLastMatch> lastmatches;
        }

        public static List<Stats> GetCSGOWeaponStats(ulong SteamID, string weaponName = null)
        {
            List<Stats> wepStats = new List<Stats>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.csgoURL + SteamID);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            Dictionary<string, PlayerStats> r = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(data);
            PlayerStats stats = r["playerstats"];
            string[] wantedStats = { "deagle", "glock", "elite", "fiveseven", "awp", "ak47", "aug", "famas", "g3sg1", "p90", "mac10", "ump45", "xm1014", "m249", "hkp2000",
                "p250", "sg556", "scar20", "ssg08", "mp7", "mp9", "nova", "negev", "sawedoff", "bizon", "tec9", "mag7", "m4a1", "galilar", "taser", "molotov", "decoy" };

            string[] formattedStats = { "Desert Eagle", "Glock", "Dual Elite", "Five-SeveN", "AWP", "AK-47", "AUG", "FAMAS", "G3SG1", "P90", "MAC10", "UMP45", "XM1014", "M249", "P2000",
                "P250", "SG 556", "SCAR-20", "SSG 08", "MP7", "MP9", "Nova", "Negev", "Sawed-Off", "PP-Bizon", "Tec-9", "MAG-7", "M4A1", "Galil AR", "Zeus x27", "Molotov", "Decoy" };

            int max = stats.stats.Count - 1;
            float shots = 0;
            float hits = 0;
            float accuracy = 0;
            string curGun = formattedStats[0];
            string oldGun = formattedStats[0];
            string pre1 = "total_shots_";
            string pre2 = "total_hits_";
            string pre3 = "total_kills_";
            string suf1Format = " Total Shots";
            string suf2Format = " Total Hits";
            string suf3Format = " Total Kills";
            for (int x = 0; x < wantedStats.Length; x++)
            {
                for (int i = 0; i < stats.stats.Count; i++)
                {
                    Stats curStat = stats.stats[i];
                    if (pre1 + wantedStats[x] == curStat.name || pre2 + wantedStats[x] == curStat.name|| pre3 + wantedStats[x] == curStat.name)
                    {
                        
                        curGun = formattedStats[x];
                        string value = curStat.value.ToString();
                        string name = curStat.name;
                        if (weaponName != null)
                        {
                            if (wantedStats[x] == weaponName)
                            {
                                if (pre2 + wantedStats[x] == curStat.name)
                                {
                                    name = formattedStats[x] + suf2Format;
                                    hits = curStat.value;
                                }
                                if (pre1 + wantedStats[x] == curStat.name)
                                {
                                    name = formattedStats[x] + suf1Format;
                                    shots = curStat.value;
                                }
                                if (pre3 + wantedStats[x] == curStat.name)
                                {
                                    name = formattedStats[x] + suf3Format;
                                }
                                //msg += "**" + name + "**: " + value + "\n";
                                Stats stat1 = new Stats();
                                stat1.name = name;
                                stat1.value = curStat.value;
                                wepStats.Add(stat1);
                                if (shots != 0 && hits != 0)
                                {
                                    if (wantedStats[x] != "decoy" && wantedStats[x] != "molotov")
                                    {
                                        if (accuracy == 0)
                                        {
                                            accuracy = hits / shots * 100;
                                            //msg += string.Format("**{0} Accuracy**: {1:N2}%\n", formattedStats[x], accuracy);
                                            Stats stat = new Stats();
                                            stat.name = formattedStats[x] + " Accuracy";
                                            stat.value = accuracy;
                                                wepStats.Add(stat);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (oldGun != curGun)
                            {
                                accuracy = hits / shots * 100;
                                if (wantedStats[x] != "decoy" && wantedStats[x] != "molotov")
                                {
                                    //msg += string.Format("**{0} Accuracy**: {1:N2}%\n", oldGun, accuracy);
                                    Stats stat = new Stats();
                                    stat.name = oldGun + " Accuracy";
                                    stat.value = accuracy;
                                    wepStats.Add(stat);
                                    //if (pre2 + wantedStats[wantedStats.Length / 2 - 1] == curStat.name)
                                      //  msg += ",";
                                }
                                oldGun = curGun;
                            }
                            
                            if (pre2 + wantedStats[x] == curStat.name)
                            {
                                name = formattedStats[x] + suf2Format;
                                hits = curStat.value;
                            }
                            if (pre1 + wantedStats[x] == curStat.name)
                            {
                                name = formattedStats[x] + suf1Format;
                                shots = curStat.value;
                            }
                            if (pre3 + wantedStats[x] == curStat.name)
                            {
                                name = formattedStats[x] + suf3Format;
                            }
                            //msg += "**" + name + "**: " + value + "\n";
                            Stats stat1 = new Stats();
                            stat1.name = name;
                            stat1.value = curStat.value;
                            wepStats.Add(stat1);

                        }
                    }
                }
            }
            //Console.WriteLine(msg);
            return wepStats;
        }

        public class Stats
        {
            public string name;
            public float value;
        }

        public class Achievements
        {
            public string name;
            public int achieved;
        }

        public class PlayerStats
        {
            public string steamID;
            public string gameName;
            public List<Stats> stats;
            public List<Achievements> achievements;
        }

        public static string getGunName(int id)
        {
            switch(id)
            {
                case 1:
                    {
                        return "Desert Eagle";
                    }
                case 2:
                    {
                        return "Dual Berettas";
                    }
                case 3:
                    {
                        return "Five-SeveN";
                    }
                case 4:
                    {
                        return "Glock-18";
                    }
                case 7:
                    {
                        return "AK-47";
                    }
                case 8:
                    {
                        return "AUG";
                    }
                case 9:
                    {
                        return "AWP";
                    }
                case 10:
                    {
                        return "FAMAS";
                    }
                case 11:
                    {
                        return "G3SG1";
                    }
                case 13:
                    {
                        return "Galil AR";
                    }
                case 14:
                    {
                        return "M249";
                    }
                case 16:
                    {
                        return "M4A4";
                    }
                case 17:
                    {
                        return "MAC-10";
                    }
                case 19:
                    {
                        return "P90";
                    }
                case 23:
                    {
                        return "MP5-SD";
                    }
                case 24:
                    {
                        return "UMP-45";
                    }
                case 25:
                    {
                        return "XM1014";
                    }
                case 26:
                    {
                        return "PP-Bizon";
                    }
                case 27:
                    {
                        return "MAG-7";
                    }
                case 28:
                    {
                        return "Negev";
                    }
                case 29:
                    {
                        return "Sawed-Off";
                    }
                case 30:
                    {
                        return "Tec-9";
                    }
                case 31:
                    {
                        return "Zeus x27";
                    }
                case 32:
                    {
                        return "P2000";
                    }
                case 33:
                    {
                        return "MP7";
                    }
                case 34:
                    {
                        return "MP9";
                    }
                case 35:
                    {
                        return "Nova";
                    }
                case 36:
                    {
                        return "P250";
                    }
                case 38:
                    {
                        return "SCAR-20";
                    }
                case 39:
                    {
                        return "SG 553";
                    }
                case 40:
                    {
                        return "SSG 08";
                    }
                case 41:
                    {
                        return "Knife";
                    }
                case 42:
                    {
                        return "Knife";
                    }
                case 59:
                    {
                        return "Knife";
                    }
                case 60:
                    {
                        return "M4A1-S";
                    }
                case 61:
                    {
                        return "USP-S";
                    }
                case 63:
                    {
                        return "CZ75-Auto";
                    }
                case 64:
                    {
                        return "R8 Revolver";
                    }
            }
            return "None";
        }
        #endregion

    }
}
