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
    public class Twitch
    {
        static string GetClipsUrl = "https://api.twitch.tv/helix/clips";
        static string GetStreamsUrl = "https://api.twitch.tv/helix/streams";
        static string GetUsersUrl = "https://api.twitch.tv/helix/users";
        static string GetFollowsUrl = "https://api.twitch.tv/helix/users/follows";
        static string clientID = Config.botconf.TWITCH_APi_KEY;
        public class TwitchUser
        {
            public string broadcaster_type;
            public string description;
            public string display_name;
            public string email;
            public string id;
            public string login;
            public string offline_image_url;
            public string profile_image_url;
            public string type;
            public int view_count;
        }

        public static TwitchUser GetUser(string username)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUsersUrl+"?login="+username);
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Client-ID", clientID);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }
            //Console.WriteLine(data);
            Dictionary<string, List<TwitchUser>> r = JsonConvert.DeserializeObject<Dictionary<string, List<TwitchUser>>>(data);
            List<TwitchUser> user = r["data"];
            if (user[0].type == "")
                user[0].type = "None";
            if (user[0].broadcaster_type == "")
                user[0].broadcaster_type = "None";
            return user[0];
        }

        public static TwitchUser GetUserByID(string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUsersUrl + "?id=" + id);
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Client-ID", clientID);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }
            Dictionary<string, List<TwitchUser>> r = JsonConvert.DeserializeObject<Dictionary<string, List<TwitchUser>>>(data);
            List<TwitchUser> user = r["data"];
            if (user[0].type == "")
                user[0].type = "User";
            if (user[0].broadcaster_type == "")
                user[0].broadcaster_type = "User";
            return user[0];
        }

        public class FollowerResponse
        {
            public int total;
            public List<Follower> data;
            public Pagination pagination;
        }
        public class Follower
        {
            public string from_id;
            public string from_name;
            public string to_id;
            public string to_name;
            public string followed_at;
        }

        public class Pagination
        {
            public string cursor;
        }

        public static FollowerResponse GetFollowers(string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetFollowsUrl + "?to_id=" + id);
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Client-ID", clientID);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string data = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }
            FollowerResponse r = JsonConvert.DeserializeObject<FollowerResponse>(data);
            Console.WriteLine("Total: " + r.total);
            foreach(Follower f in r.data)
            {
                Console.WriteLine("From ID: " + f.from_id);
                Console.WriteLine("From Name: " + f.from_name);
                Console.WriteLine("To ID: " + f.to_id);
                Console.WriteLine("To Name: " + f.to_name);
                Console.WriteLine("Followed At: " + f.followed_at);
            }
            return r;
        }
    }
}
