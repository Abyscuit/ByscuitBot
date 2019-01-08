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
    public class EtherScan
    {
        public static string GetBalance(string address)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.etherscanBalance + address);
            request.ContentType = "application/json; charset=utf-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd ();
            }

            Dictionary<string, string> r = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            if (r["result"] != "")
                return r["result"];
            else
                return null;
        }
        public class Token
        {
            public string blockNumber;
            public string timeStamp;
            public string hash;
            public string nonce;
            public string blockHash;
            public string from;
            public string contractAddress;
            public string to;
            public string value;
            public string tokenName;
            public string tokenSymbol;
            public string tokenDecimal;
            public string transactionIndex;
            public string gas;
            public string gasPrice;
            public string gasUsed;
            public string cumulativeGasUsed;
            public string input;
            public string confirmations;
        }
        public class TokenResponse
        {
            public string status;
            public string message;
            public List<Token> result;
        }
        public static List<Token> GetTokens(string address)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global.ethtokenBalance + address);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }

            TokenResponse r = JsonConvert.DeserializeObject<TokenResponse>(data);
            if (r.status != "0")
                return r.result;
            else
                return null;
        }
        
    }
}
