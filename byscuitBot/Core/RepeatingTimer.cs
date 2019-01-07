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
        private static SocketTextChannel channel;
        private static bool giveAway = false;

        internal static Task StartTimer()
        {
            if (Global.Guild != null)
            {
                var result = from a in Global.Client.Guilds
                             where a.Id == Global.Guild.Id
                             select a;

                SocketGuild guild = result.FirstOrDefault();

                var result2 = from a in guild.TextChannels
                              where a.Name == "general"
                              select a;


                SocketTextChannel txtChan = result2.FirstOrDefault();

                channel = txtChan;

            }
                loopingTimer = new Timer()
                {
                    Interval = 5000,
                    AutoReset = true,
                    Enabled = true
                };
                loopingTimer.Elapsed += OnTimerTicked;
            return Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            if (Global.giveaway != giveAway && channel != null)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Giveaway ended!");
                embed.WithDescription("Congrats to ");
                embed.WithColor(100, 150, 255);
                embed.WithFooter("Created by Abyscuit");

                
                await channel.SendMessageAsync("", false, embed.Build());

                Global.giveaway = giveAway;
            }
            if (Global.giveaway == true)
            {
                if(DateTime.Compare(DateTime.Now, Global.giveawayTime) <= 0)
                {
                    giveAway = false;
                }
            }

        }
    }
}
