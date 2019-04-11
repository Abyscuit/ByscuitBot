using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class UrbanDictionary
    {
        static string website = "https://www.urbandictionary.com/define.php?term="; //Search term immediately after

        public static string GetDefinition(string word)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(website + word);
            request.ContentType = "text/html; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }
            string[] split = data.Replace( "<div class=\"meaning\">", "▬" ).Split('▬');
            string definition = "";
            //for (int i = 1; i < split.Length; i++)
            {
                string[] split2 = split[1].Replace("</div>", "▬").Split('▬');
                string def = split2[0];
                //Console.WriteLine("Definition " + 1 + ":\n" + definition + "\n");
                definition += "Definition:\n" + StripHTML(def) + "\n\n";
            }

            return definition;
            //Response r = JsonConvert.DeserializeObject<Response>(data);
        }

        //My terrible method
        public static string SanitizeText(string text)
        {
            string sanitized = "";
            string[] split = text.Replace("</a>","▬").Split('▬');
            for (int i = 0; i < split.Length; i++)
            {
                string[] split2 = split[i].Split('>');  //Split2[0].Split('<a') is normal
                string[] split3 = split2[0].Split('<');
                sanitized += split3[0];
                if (split2.Length > 1)
                {
                    string str = split2[1];
                    Console.WriteLine("Sanitized: " + str);
                    sanitized += str;
                } 
            }
            sanitized = sanitized.Replace("&apos;", "'");
            //Console.WriteLine("Everything Sanitized:\n" + sanitized + "\n\n");
            return sanitized;
        }

        //Most efficient method
        public static string StripHTML(string HTMLText, bool decode = true)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var stripped = reg.Replace(HTMLText, "");
            return decode ? WebUtility.HtmlDecode(stripped) : stripped;
        }
    }
}
