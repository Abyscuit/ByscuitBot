using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class Antispam
    {
        public static float millisecondThreshold = 300;



        //Spam Account Creation Handling
        public class SpamAccount
        {
            public string DiscordUsername;
            public ulong DiscordID;
            public DateTime BanTime;
            public int BanAmount;
            public List<DateTime> LastMessages;
            public DateTime LastBan;
        }

        public static List<SpamAccount> spamAccounts = new List<SpamAccount>();
        public static string spamFile = "Resources/spam_accounts.json";

        public static void LoadSpamAccount()
        {
            if (DataStorage.SaveExists(spamFile))
            {
                spamAccounts = DataStorage.LoadSpamAccounts(spamFile).ToList();
            }
            else
            {
                spamAccounts = new List<SpamAccount>();
                SaveAccounts();
            }

        }

        public static void SaveAccounts()
        {
            DataStorage.SaveSpamAccounts(spamAccounts, spamFile);
        }

        public static SpamAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAcount(user);
        }

        private static SpamAccount GetOrCreateAcount(SocketUser user)
        {
            IEnumerable<SpamAccount> result = from a in spamAccounts
                                               where a.DiscordID == user.Id
                                               select a;

            SpamAccount usr = result.FirstOrDefault();
            if (usr == null) usr = CreateAccount(user);

            //return account
            return usr;
        }

        private static SpamAccount CreateAccount(SocketUser socketUser)
        {
            SpamAccount newAccount = new SpamAccount()
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

        public static void UpdateAccount(SocketUser user, SpamAccount account)
        {
            SpamAccount spamAccount = GetOrCreateAcount(user);
            spamAccounts.Remove(spamAccount);
            spamAccounts.Add(account);
            SaveAccounts();
        }
    }
}
