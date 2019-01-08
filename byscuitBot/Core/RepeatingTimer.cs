using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace byscuitBot.Core
{
    internal class RepeatingTimer
    {
        private static Timer loopingTimer;
        public static SocketTextChannel channel;
        private static bool giveAway = false;
        public static float minutes = 0;
        public static DateTime timeToStop = DateTime.Now;
        public static bool startTimer = false;

        internal static Task StartTimer()
        {
            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            loopingTimer.Elapsed += OnTimerTicked;
            loopingTimer.Start();
            return Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            //if (Global.giveaway != giveAway)
            if(DateTime.Compare(timeToStop, DateTime.Now) < 0 && startTimer)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Timer Ended");
                embed.WithDescription("");
                embed.WithColor(100, 150, 255);
                embed.WithFooter("Created by Abyscuit");

                
                await channel.SendMessageAsync("", false, embed.Build());
                startTimer = !startTimer;
                //Global.giveaway = giveAway;
            }
            if (Global.giveaway == true)
            {
                if(DateTime.Compare(DateTime.Now, Global.giveawayTime) <= 0)
                {
                    giveAway = false;
                }
            }

            foreach(Antispam.SpamAccount spamAccount in Antispam.spamAccounts)
            {
                User_Accounts.UserAccount account = User_Accounts.UserAccounts.GetAccount(spamAccount.DiscordID);
                if (account.IsMuted)
                {
                    if (DateTime.Compare(spamAccount.BanTime, DateTime.Now) < 0)
                    {
                        account.IsMuted = false;
                        User_Accounts.UserAccounts.SaveAccounts();
                    }
                }
            }
        }
    }
}
