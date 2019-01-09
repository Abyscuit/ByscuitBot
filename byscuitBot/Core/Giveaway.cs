using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core
{
    public class Giveaway
    {
        public string Item;
        public DateTime EndTime;
        public List<ulong> UsersID;
        public ulong WinnerID;
        public ulong CreatorID;
        public ulong MessageID;
        public ulong ChannelID;
    }

    public class GiveawayManager
    {
        public static List<Giveaway> Giveaways = new List<Giveaway>();
        private static string gaFile = "Resources/giveaways.json";
        

        public static void LoadGiveaways()
        {
            if (DataStorage.SaveExists(gaFile))
            {
                Giveaways = DataStorage.LoadGiveaways(gaFile).ToList();
            }
            else
            {
                Giveaways = new List<Giveaway>();
                Save();
            }

        }

        public static void Save()
        {
            DataStorage.SaveGiveaways(Giveaways, gaFile);
        }

        public static Giveaway MakeGiveaway(SocketUser user, string item, RestUserMessage message, DateTime time)
        {
            return CreateGiveaway(user, item, message, time);
        }

        public static Giveaway GetGiveaway(RestUserMessage message)
        {
            return GetOrCreateGiveaway(null, "", message, DateTime.Now);
        }


        private static Giveaway GetOrCreateGiveaway(SocketUser user, string item, RestUserMessage message, DateTime time)
        {
            IEnumerable<Giveaway> result = from a in Giveaways
                                              where a.MessageID == message.Id
                                              select a;

            Giveaway account = result.FirstOrDefault();
            if (account == null) account = CreateGiveaway(user,item,message,time);

            //return account
            return account;
        }

        private static Giveaway CreateGiveaway(SocketUser user, string item, RestUserMessage message, DateTime time)
        {
            Giveaway giveaway = new Giveaway()
            {
                CreatorID = user.Id,
                UsersID = new List<ulong>(),
                Item = item,
                MessageID = message.Id,
                EndTime = time,
                WinnerID = 0,
                ChannelID = message.Channel.Id
            };
            Giveaways.Add(giveaway);
            Save();
            return giveaway;
        }

        public static void DeleteGiveaway(RestUserMessage Message)
        {
            Giveaways.Remove(GetGiveaway(Message));
            Save();
        }
    }
}
