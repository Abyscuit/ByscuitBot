using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class ESEA
    {
        public static void GetInfo(string user)
        {

            using (WebClient w = new WebClient())
            {
                w.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                string s = w.DownloadString(Global.ESEA + user + "?tab=stats");

                // 2.
                Console.WriteLine(s);
            }
        }
    }
}
