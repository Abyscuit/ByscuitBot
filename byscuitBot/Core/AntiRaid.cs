using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class AntiRaid
    {
        public static float millisecondThreshold = 5000; //5 seconds
        public static bool slowMode = false;


        //Spam Account Creation Handling
        public class RaidAccount
        {
            public string DiscordUsername;
            public ulong DiscordID;
            public DateTime BanTime;
            public int BanAmount;
            public List<DateTime> LastMessages;
            public DateTime LastBan;
        }

        public static List<RaidAccount> spamAccounts = new List<RaidAccount>();
        public static string spamFile = "Resources/raid_accounts.json";

        public static void LoadSpamAccount()
        {
            if (DataStorage.SaveExists(spamFile))
            {
                spamAccounts = DataStorage.LoadRaidAccounts(spamFile).ToList();
            }
            else
            {
                spamAccounts = new List<RaidAccount>();
                SaveAccounts();
            }

        }

        public static void SaveAccounts()
        {
            DataStorage.SaveRaidAccounts(spamAccounts, spamFile);
        }

        public static RaidAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAcount(user);
        }

        private static RaidAccount GetOrCreateAcount(SocketUser user)
        {
            IEnumerable<RaidAccount> result = from a in spamAccounts
                                              where a.DiscordID == user.Id
                                              select a;

            RaidAccount usr = result.FirstOrDefault();
            if (usr == null) usr = CreateAccount(user);

            //return account
            return usr;
        }

        private static RaidAccount CreateAccount(SocketUser socketUser)
        {
            RaidAccount newAccount = new RaidAccount()
            {
                DiscordUsername = socketUser.Username,
                DiscordID = socketUser.Id,
                BanAmount = 0,
                BanTime = DateTime.Now,
                LastMessages = new List<DateTime>()
            };

            spamAccounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }

        public static void UpdateAccount(SocketUser user, RaidAccount account)
        {
            RaidAccount spamAccount = GetOrCreateAcount(user);
            spamAccounts.Remove(spamAccount);
            spamAccounts.Add(account);
            SaveAccounts();
        }
    }
}
