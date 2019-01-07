using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core.User_Accounts
{
    public class UserAccounts
    {
        private static List<UserAccount> accounts = new List<UserAccount>();
        private static string accountFile = "Resources/accounts.json";

        public UserAccounts()
        {

        }

        public static void LoadUserAccts()
        {
            if (DataStorage.SaveExists(accountFile))
            {
                accounts = DataStorage.LoadUserAccounts(accountFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }

        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, accountFile); 
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAcount(user.Id);
        }

        private static UserAccount GetOrCreateAcount(ulong id)
        {
            IEnumerable<UserAccount> result = from a in accounts
                         where a.ID == id
                         select a;

            UserAccount account = result.FirstOrDefault();
            if (account == null) account = CreateAccount(id);

            //return account
            return account;
        }

        private static UserAccount CreateAccount(ulong id)
        {
            UserAccount newAcct = new UserAccount()
            {
                ID = id,
                Points = 10,
                XP = 0,
                IsMuted = false,
                NumberOfWarnings = 0
            };

            accounts.Add(newAcct);
            SaveAccounts();
            return newAcct;
        }
    }
}
