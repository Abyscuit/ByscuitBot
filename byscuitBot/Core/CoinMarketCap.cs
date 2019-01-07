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
    public class CoinMarketCap
    {
        public class Response
        {
            public Status status;
            public List<Currency> data;
        }
        public class Status
        {
            public string timeStamp;
            public int error_code;
            public string error_message;
            public int elapsed;
            public int credit_count;
        }
        public class Currency
        {
            public uint id;
            public string name;
            public string symbol;
            public string slug;
            public uint cmc_rank;
            public uint num_market_pairs;
            public double circulating_supply;
            public double total_supply;
            public double? max_supply;
            public string last_updated;
            public string date_added;
            public List<string> tags;
            public Platform platform;
            public Quote quote;
        }
        public class Platform
        {
            public uint id;
            public string name;
            public string symbol;
            public string slug;
        }
        public class Quote
        {
            public Key USD;
        }
        public class Key
        {
            public double price;
            public double volume_24h;
            public double percent_change_1h;
            public double percent_change_24h;
            public double percent_change_7d;
            public double market_cap;
            public string last_updated;
        }
        public static List<Currency> GetTop10()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.top10crypto);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            Response r = JsonConvert.DeserializeObject<Response>(data);
            if (r.status.error_code == 0)
                return r.data;
            else
                return null;
        }
        
        public class TokenResponse
        {
            public Status status;
            public Dictionary<string, Currency> data;
        }
        public static Dictionary<string, Currency> GetTokens(string symbols)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.ethtokenvalue + symbols);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            TokenResponse r = JsonConvert.DeserializeObject<TokenResponse>(data);
            if (r.status.error_code == 0)
                return r.data;
            else
                return null;
        }

        public static Currency GetToken(string symbol)
        {
            return GetTokens(symbol)[symbol];
        }
        
    }
}
