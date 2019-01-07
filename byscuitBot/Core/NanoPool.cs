using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class NanoPool
    {
        public class Response
        {
            public bool status { get; set; }
            public Account data { get; set; }
            public string error { get; set; }
        }
        public class Account
        {
            public string account { get; set; }
            public string unconfirmed_balance { get; set; }
            public string balance { get; set; }
            public string hashrate { get; set; }
            public Dictionary<string, string> avgHashrate { get; set; }
            public List<Worker> workers { get; set; }
        }

        public class Worker
        {
            public string id { get; set; }
            public ulong uid { get; set; }
            public string hashrate { get; set; }
            public ulong lastshare { get; set; }
            public ulong rating { get; set; }
            public string h1 { get; set; }
            public string h3 { get; set; }
            public string h6 { get; set; }
            public string h12 { get; set; }
            public string h24 { get; set; }
        }


        public static Account GetAccount(string address, SocketUser user)
        {
            if (address == null)
            {
                UserAccount userAccount = GetUser(user);
                if (userAccount != null)
                    address = userAccount.address;
                else
                    address = "";
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.nanopoolGeneralInfo + address);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            Response r = JsonConvert.DeserializeObject<Response>(data);
            Account account = null;
            if (r.status != false)
                account = r.data;
            else
                return null;

            return account;
        }

        public class UserAccount
        {
            public string address;
            public ulong discordID;
            public string discordUsername;
        }

        private static List<UserAccount> accounts = new List<UserAccount>();

        private static string nanopoolFile = "Resources/EthAddress.json";


        public static void LoadAccounts()
        {
            if (DataStorage.SaveExists(nanopoolFile))
            {
                accounts = DataStorage.LoadNanoPool(nanopoolFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }

        }
        public static int GetCount()
        {
            return accounts.Count;
        }
        public static void SaveAccounts()
        {
            DataStorage.SaveNanoPool(accounts, nanopoolFile);
        }

        public static UserAccount GetUser(SocketUser user, string address = null)
        {
            return GetOrCreateAcount(user, address);
        }


        private static UserAccount GetOrCreateAcount(SocketUser socketUser, string address)
        {
            IEnumerable<UserAccount> result = from a in accounts
                                              where a.discordID == socketUser.Id
                                              select a;

            UserAccount user = result.FirstOrDefault();
            if (user == null) user = CreateAccount(socketUser, address);

            //return account
            return user;
        }

        private static UserAccount CreateAccount(SocketUser user, string address)
        {
            if (address != null)
            {
                UserAccount userAccount = new UserAccount()
                {
                    discordUsername = user.Username,
                    discordID = user.Id,
                    address = address
                };

                accounts.Add(userAccount);
                SaveAccounts();
                return userAccount;
            }

            return null;
        }
        public class PriceResponse
        {
            public bool status;
            public Prices data;
        }
        public class Prices
        {
            public double price_usd;
            public double price_btc;
            public double price_eur;
            public double price_rur;
            public double price_CNY;
            public double price_gbp;
        }

        public static Prices GetPrices()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.ethPrice);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            PriceResponse r = JsonConvert.DeserializeObject<PriceResponse>(data);
            Prices prices = null;
            if (r.status != false)
                prices = r.data;
            else
                return null;

            return prices;

        }


        public class Amount
        {
            public double coins;
            public double dollars;
            public double yuan;
            public double euros;
            public double rubles;
            public double bitcoins;
            public double pounds;
        }

        public class CalcResponse
        {
            public bool status;
            public Dictionary<string, Amount> data;
        }

        public static List<Amount> CalculateEth(float hashrate)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.calceth + hashrate);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            CalcResponse r = JsonConvert.DeserializeObject<CalcResponse>(data);
            List<Amount> amounts = new List<Amount>();

            if (r.status != false)
            {
                amounts.Add(r.data["minute"]);
                amounts.Add(r.data["hour"]);
                amounts.Add(r.data["day"]);
                amounts.Add(r.data["week"]);
                amounts.Add(r.data["month"]);
                Amount year = new Amount
                {
                    coins = r.data["month"].coins * 12,
                    dollars = r.data["month"].dollars * 12,
                    yuan = r.data["month"].yuan * 12,
                    euros = r.data["month"].euros * 12,
                    rubles = r.data["month"].rubles * 12,
                    bitcoins = r.data["month"].bitcoins * 12,
                    pounds = r.data["month"].pounds * 12
                };
                amounts.Add(year);
            }
            else
                return null;

            return amounts;

        }
    }
}
