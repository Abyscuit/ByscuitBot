using byscuitBot.Core.Server_Data;
using byscuitBot.Modules;
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
        public static ISocketMessageChannel generalChannel;
        public static float minutes = 0;
        public static DateTime timeToStop = DateTime.Now;
        public static bool startTimer = false, clrMsg = false;
        public static DateTime clrMsgTime = DateTime.Now;
        static int oldGCount = 0;
        public static DateTimeOffset compareTime;
        public static List<DateTime> serverDay = new List<DateTime>();
        public static List<bool> dayCheck = new List<bool>();
        public static bool[] debug;

        internal static async Task StartTimer()
        {
            oldGCount = Program.client.Guilds.Count;
            debug = new bool[Program.client.Guilds.Count];

            for (int i = 0; i < oldGCount; i++)
            {
                serverDay.Add(DateTime.Now.AddDays(-1));
                dayCheck.Add(false);
            }
            if (!Program.t.IsAlive)
                Program.t.Start();
            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            loopingTimer.Elapsed += OnTimerTicked;
            loopingTimer.Start();
            await UpdateAuditLog();
            await Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            //----------GIVE AWAY-----------
            //if (Global.giveaway != giveAway)
            if (DateTime.Compare(timeToStop, DateTime.Now) < 0 && startTimer)
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

            //-------Spam account timer----------
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


            //-------Form Updates-------------
            if (clrMsgTime.CompareTo(DateTime.Now) < 0 && clrMsg)
            {
                await channel.DeleteMessageAsync(Global.MessageIdToTrack);
                clrMsg = false;
            }
            int newCount = Program.client.Guilds.Count;
            if (oldGCount != newCount)
            {
                Program.form.updateServers();
                for (int i = oldGCount; i < newCount; i++)
                {
                    serverDay.Add(DateTime.Now);
                    dayCheck.Add(false);
                }
                oldGCount = newCount;
            }


            //---------Timer for events--------
            SocketGuild[] guilds = Program.client.Guilds.ToArray();

            for (int i = 0; i < guilds.Length; i++)
            {
                bool genFound = false;
                foreach (SocketTextChannel textChannel in guilds[i].TextChannels)
                {
                    if (textChannel.Name.ToLower().Contains("general"))
                    {
                        generalChannel = textChannel;
                        genFound = true;
                        break;
                    }
                }
                if (!genFound)
                    generalChannel = guilds[i].DefaultChannel;
                if (!dayCheck[i])
                {
                    //birthday check
                    DateTimeOffset serverCreated = guilds[i].CreatedAt;
                    DateTime serverBirthday = new DateTime(DateTime.Now.Year, serverCreated.Month, serverCreated.Day);
                    if (compareDates(serverBirthday, DateTime.Now))
                    {
                        await generalChannel.SendMessageAsync(String.Format(Global.Holidays[0], guilds[i].Name,
                            DateTimeOffset.Now.Subtract(guilds[i].CreatedAt).TotalDays / 365));
                    }

                    //4th july check
                    DateTime _4thJuly = new DateTime(DateTime.Now.Year, 7, 4);
                    if (compareDates(_4thJuly, DateTime.Now))
                    {
                        //await generalChannel.SendMessageAsync(Global.Holidays[1]);
                    }
                    //halloween check
                    DateTime HALLOWEEN_CHECK = new DateTime(DateTime.Now.Year, 10, 31);
                    if (compareDates(HALLOWEEN_CHECK, DateTime.Now))
                    {
                        await generalChannel.SendMessageAsync(Global.Holidays[2]);
                    }

                }
                if (!compareDates(DateTime.Now, serverDay[i]))
                {
                    dayCheck[i] = true;
                    serverDay[i] = DateTime.Now;
                    Console.WriteLine(DateTime.Now + " | Events checked for " + guilds[i].Name);
                }
            }

        }
        public static bool compareDates(DateTime first, DateTime second)
        {
            bool result = false;
            if (first.Month == second.Month)
                result = true;
            if (first.Day == second.Day && result)
                result = true;
            else result = false;
            return result;
        }


        public static async Task UpdateAuditLog(SocketGuild guild = null)
        {
            if (guild != null)
            {
                await AuditUpdate(guild);
            }// end if guild not null
            else
            {
                foreach (SocketGuild Guild in Program.client.Guilds)
                {
                    await AuditUpdate(Guild);
                }
            }
        }

        public static async Task AuditUpdate(SocketGuild guild)
        {
            foreach (SocketTextChannel chan in guild.TextChannels)
            {
                if (chan.Name.ToLower().Contains("security"))
                {
                    //Console.WriteLine("Security channel found in " + guild.Name);
                    IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> auditLog = guild.GetAuditLogsAsync(10);
                    List<IReadOnlyCollection<RestAuditLogEntry>> auditList = await auditLog.ToList();
                    IAsyncEnumerable<IReadOnlyCollection<IMessage>> messages = chan.GetMessagesAsync(1);
                    List<IReadOnlyCollection<IMessage>> msgList = await messages.ToList();
                    IMessage mesg = null;
                    if(msgList[1].Count > 0)
                        mesg = msgList[1].ToArray()[0];// Newest Message
                    RestAuditLogEntry[] restAuditLogs = auditList[0].ToArray();
                    if (mesg != null)
                    {
                        if (mesg.Embeds.Count > 0)
                        {
                            if (restAuditLogs[0].Id.ToString() == mesg.Embeds.FirstOrDefault().Footer.Value.Text)
                            {
                                break;
                            }
                        }
                        else continue;
                    }
                    for (int i = restAuditLogs.Length - 1; i >= 0; i--)
                    {
                        if (mesg != null)
                        {
                            if (mesg.Embeds.Count > 0)
                            {
                                int dif = DateTimeOffset.Compare(mesg.Embeds.FirstOrDefault().Timestamp.Value, restAuditLogs[i].CreatedAt);
                                if (dif >= 0)
                                {
                                    continue;
                                }
                            }
                            else continue;
                        }
                        RestAuditLogEntry logEntry = restAuditLogs[i];
                        bool send = false;
                        EmbedBuilder embed = new EmbedBuilder();
                        string adminUsername = "**" + logEntry.User + "**";
                        string title = "Server Report";
                        embed.WithTimestamp(logEntry.CreatedAt);
                        string msg = ""; 
                        if (logEntry.Action == ActionType.MemberRoleUpdated)
                        {
                            MemberRoleAuditLogData data = (MemberRoleAuditLogData)logEntry.Data;
                            string targetUsername = "**" + data.Target + "** _(" + data.Target.Id + ")_";
                            string roles = "";
                            foreach (MemberRoleEditInfo role in data.Roles)
                            {
                                roles += string.Format(((role.Added) ? " Added role {0} to " : " Removed role {0} from "), "**_" + role.Name + "_**");
                            }

                            msg = adminUsername + roles + targetUsername;
                            embed.WithColor(new Color(255, 190, 0));
                            send = true;
                        }// end if log entry role updated
                        else if(logEntry.Action == ActionType.MemberUpdated)
                        {
                            MemberUpdateAuditLogData data = (MemberUpdateAuditLogData)logEntry.Data;
                            string targetUsername = "**" + data.Target + "** _(" + data.Target.Id + ")_";
                            if (data.Before.Deaf != data.After.Deaf)
                                msg += string.Format(adminUsername + " {0} " + targetUsername+ "\n", (data.After.Deaf.Value) ? "deafened" : "undeafened");
                            
                            if (data.Before.Mute != data.After.Mute)
                                msg += string.Format(adminUsername + " {0} " + targetUsername + "\n", (data.After.Mute.Value) ? "muted" : "unmuted");

                            
                            if (data.Before.Nickname != data.After.Nickname)
                                msg += string.Format(adminUsername + " changed {0}'s nickname from *{1}* to *{2}*\n", targetUsername,
                                    (data.Before.Nickname == null) ? data.Target.Username : data.Before.Nickname,
                                    (data.After.Nickname == null) ? data.Target.Username : data.After.Nickname);

                            Console.WriteLine(msg);
                            embed.WithColor(255, 255, 0);
                            send = true;
                        }
                        else if (logEntry.Action == ActionType.Kick)
                        {
                            KickAuditLogData data = (KickAuditLogData)logEntry.Data;
                            string targetUsername = "**" + data.Target + "** _(" + data.Target.Id + ")_";
                            msg = targetUsername + " was kicked by " + adminUsername +
                                "\nReason: " + logEntry.Reason;
                            
                            embed.WithColor(new Color(255, 0, 0));
                            send = true;

                        }
                        else if (logEntry.Action == ActionType.Ban)
                        {
                            BanAuditLogData data = (BanAuditLogData)logEntry.Data;
                            string targetUsername = "**" + data.Target + "** _(" + data.Target.Id + ")_";
                            msg = targetUsername + " was banned by " + adminUsername +
                                "\nReason: " + logEntry.Reason;

                            embed.WithColor(new Color(255, 0, 0));
                            send = true;
                        }
                        else if (logEntry.Action == ActionType.Unban)
                        {

                            UnbanAuditLogData data = (UnbanAuditLogData)logEntry.Data;
                            string targetUsername = "**" + data.Target + "** _(" + data.Target.Id + ")_";
                            msg = targetUsername + " was unbanned by " + adminUsername +
                                "\nReason: " + logEntry.Reason;

                            embed.WithColor(new Color(255, 255, 0));
                            send = true;
                        }
                        else if (logEntry.Action == ActionType.Prune)
                        {
                            PruneAuditLogData data = (PruneAuditLogData)logEntry.Data;
                            msg = adminUsername + " removed " + data.MembersRemoved + " users in a " + data.PruneDays + " day prune"+
                                "\nReason: " + logEntry.Reason;

                            embed.WithColor(new Color(255, 0, 0));
                            send = true;
                        }
                        else if (logEntry.Action == ActionType.MessageDeleted)
                        {
                            MessageDeleteAuditLogData data = (MessageDeleteAuditLogData)logEntry.Data;
                            msg = adminUsername + " deleted _" + data.MessageCount + "_ messages in **#" + guild.GetChannel(data.ChannelId).Name + "** channel";

                            embed.WithColor(new Color(255, 190, 0));
                            send = true;

                        }
                        else if (logEntry.Action == ActionType.InviteCreated)
                        {
                            InviteCreateAuditLogData data = (InviteCreateAuditLogData)logEntry.Data;
                            string creatorUsername = "**" + data.Creator + "** _(" + data.Creator.Id + ")_";
                            msg = creatorUsername + " created invite **" + data.Code + "**\n" +
                                "Channel: " + guild.GetChannel(data.ChannelId).Name + "\n" +
                                "Max Uses: " + data.MaxUses + "\n" +
                                "Max Age: " + (data.MaxAge / 60 / 60 / 24) + "\n" +
                                "Temporary: " + data.Temporary;

                            embed.WithColor(new Color(255, 255, 0));
                            send = true;

                        }
                        else if (logEntry.Action == ActionType.InviteDeleted)
                        {
                            InviteDeleteAuditLogData data = (InviteDeleteAuditLogData)logEntry.Data;
                            string creatorUsername = "**" + data.Creator + "** _(" + data.Creator.Id + ")_";
                            msg = adminUsername + " deleted invite **" + data.Code + "** created by " + creatorUsername + "\n" +
                                "Channel: " + guild.GetChannel(data.ChannelId).Name + "\n" +
                                "Uses: " + data.Uses + "\n" +
                                "Max Uses: " + data.MaxUses + "\n" +
                                "Max Age: " + (data.MaxAge / 60 / 60 / 24) + "\n" +
                                "Temporary: " + data.Temporary;

                            embed.WithColor(new Color(255, 255, 0));
                            send = true;

                        }
                        else
                        {
                            Console.WriteLine("LE_Action: " + logEntry.Action + "; LE_Data: " + logEntry.Data.ToString() + "; ");
                        }
                        embed.WithAuthor(title,logEntry.User.GetAvatarUrl());
                        embed.WithDescription(msg);
                        embed.WithFooter(logEntry.Id.ToString());
                        if (send) await chan.SendMessageAsync("", false, embed.Build());
                        //if (send) Console.WriteLine("Sent " + embed.Description);
                    }//end restAuditLog loop
                    break;
                }// end if security channel
            }// end for each text channel in guild
        }
    }
}
