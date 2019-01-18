using byscuitBot.Core.Server_Data;
using Discord;
using Discord.Rest;
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
        public static float minutes = 0;
        public static DateTime timeToStop = DateTime.Now;
        public static bool startTimer = false, clrMsg = false;
        public static DateTime clrMsgTime = DateTime.Now;
        static int oldGCount = 0;

        internal static Task StartTimer()
        {
            Console.WriteLine(Utilities.getAlert("loadForm"));
            oldGCount = Program.client.Guilds.Count;
            Program.t.Start();
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
            foreach (Giveaway giveaway in GiveawayManager.Giveaways)
            {
                if (giveaway.EndTime.CompareTo(DateTime.Now) < 0)
                {
                        SocketTextChannel textChannel = channel.Guild.GetTextChannel(giveaway.ChannelID);
                        RestUserMessage message = (RestUserMessage)await textChannel.GetMessageAsync(giveaway.MessageID);
                        ServerConfig config = ServerConfigs.GetConfig(channel.Guild);
                    if (giveaway.UsersID.Count > 0)
                    {
                        int win = Global.rand.Next(0, giveaway.UsersID.Count - 1);
                        ulong winner = giveaway.UsersID[win];
                        giveaway.WinnerID = winner;
                        SocketUser user = channel.Guild.GetUser(winner);
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithTitle(giveaway.Item);
                        embed.WithDescription("**Giveaway Ended " + giveaway.EndTime.ToShortDateString() + " " + giveaway.EndTime.ToShortTimeString() + "!**\nWinner is " + user.Mention + "");
                        embed.WithFooter(config.FooterText);
                        embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                        if (config.TimeStamp)
                            embed.WithCurrentTimestamp();

                        await message.ModifyAsync(m => { m.Embed = embed.Build(); });
                        embed.WithTitle(giveaway.Item);
                        embed.WithDescription("**Winner is " + user.Mention + "**");
                        embed.WithFooter(config.FooterText);
                        embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                        if (config.TimeStamp)
                            embed.WithCurrentTimestamp();
                        await textChannel.SendMessageAsync("🎉**GIVEAWAY ENDED**🎉", false, embed.Build());
                    }
                    else
                    {
                        EmbedBuilder embed = new EmbedBuilder();
                        SocketUser user = channel.Guild.GetUser(giveaway.CreatorID);
                        embed.WithTitle(giveaway.Item);
                        embed.WithDescription("**Giveaway Ended " + giveaway.EndTime.ToShortDateString() + " " + giveaway.EndTime.ToShortTimeString() + "!**\nNo one entered!\n" + user.Mention + 
                            " may start a new one.");
                        embed.WithFooter(config.FooterText);
                        embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                        if (config.TimeStamp)
                            embed.WithCurrentTimestamp();
                        await message.ModifyAsync(m => { m.Embed = embed.Build(); });

                        embed.WithTitle(giveaway.Item);
                        embed.WithDescription("**No one entered!**\nYou may start another giveaway " + user.Mention);
                        embed.WithFooter(config.FooterText);
                        embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                        if (config.TimeStamp)
                            embed.WithCurrentTimestamp();
                        await textChannel.SendMessageAsync("🎉**GIVEAWAY ENDED**🎉", false, embed.Build());
                    }
                    GiveawayManager.DeleteGiveaway(message);
                    GiveawayManager.Save();
                }
            }

            foreach (Antispam.SpamAccount spamAccount in Antispam.spamAccounts)
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

            if(clrMsgTime.CompareTo(DateTime.Now) < 0 && clrMsg)
            {
                await channel.DeleteMessageAsync(Global.MessageIdToTrack);
                clrMsg = false;
            }
            if(oldGCount != Program.client.Guilds.Count)
            {
                Program.form.updateServers();
                oldGCount = Program.client.Guilds.Count;
            }
        }
    }
}
