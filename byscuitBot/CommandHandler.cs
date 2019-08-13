using byscuitBot.Core;
using byscuitBot.Core.Server_Data;
using byscuitBot.Core.Steam_Accounts;
using byscuitBot.Core.User_Accounts;
using byscuitBot.Modules;
using byscuitBot.Properties;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot
{
    public class CommandHandler
    {
        DiscordSocketClient client;
        static CommandService service;
        public static ulong BotID = 510066148285349900;
        public List<IMessage> oldMessage = new List<IMessage>();
        public int msgCount = 0;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            this.client = client;
            service = new CommandService();
            await service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            this.client.MessageReceived += Client_MessageReceived;
            this.client.UserJoined += Client_UserJoined;
            this.client.UserLeft += Client_UserLeft;
            //this.client.ChannelUpdated += Client_ChannelUpdated;
            this.client.UserUnbanned += Client_UserUnbanned;
            //this.client.GuildMemberUpdated += Client_GuildMemberUpdated;
            //this.client.GuildUpdated += Client_GuildUpdated;
            this.client.UserBanned += Client_UserBanned;
            //this.client.UserUpdated += Client_UserUpdated;
            //this.client.RoleCreated += Client_RoleCreated;
            this.client.MessageDeleted += Client_MessageDeleted;
            //this.client.LatencyUpdated += Client_LatencyUpdated;
            //this.client.MessagesBulkDeleted += Client_MessagesBulkDeleted;
            
        }

        public static List<CommandInfo> GetCommands()
        {
            
            return service.Commands.ToList();
        }

        private Task Client_LatencyUpdated(int arg1, int arg2)
        {
            string mes = DateTime.Now + " | ARG1: " + arg1 + "\tARG2: " + arg2;
            Log.AddTextToLog(mes);
            Console.WriteLine(mes);
            return Task.CompletedTask;
        }

        #region update Audit log
        private Task Client_MessagesBulkDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> arg1, ISocketMessageChannel arg2)
        {
            return Task.CompletedTask;
        }

        private async Task Client_RoleCreated(SocketRole arg)
        {
            await RepeatingTimer.UpdateAuditLog();
        }

        private async Task Client_UserUpdated(SocketUser arg1, SocketUser arg2)
        {
            //RepeatingTimer.UpdateAuditLog().GetAwaiter().GetResult();
            //return Task.CompletedTask;
            await RepeatingTimer.UpdateAuditLog();
        }

        private async Task Client_UserBanned(SocketUser user, SocketGuild guild)
        {
            RestBan ban = await guild.GetBanAsync(user);
            RestAuditLogEntry logEntry = await GetAuditLogEntry(guild);
            if (logEntry.Action != ActionType.Ban) return;
            BanAuditLogData data = (BanAuditLogData)logEntry.Data;
            string targetUsername = "**" + user + "** _(" + user.Id + ")_";
            string adminUsername = "**" + logEntry.User + "**";
            string msg = targetUsername + " was banned by " + adminUsername +
                "\nReason: " + (string.IsNullOrEmpty(ban.Reason) ? "No reason provided." : ban.Reason);

            foreach (SocketTextChannel channel in Global.SECURITY_CHANNELS)
            {
                if (channel.Guild.Id == guild.Id)
                {
                    await SendSecurityLog(msg, new Color(255, 0, 0), channel, logEntry.Id.ToString(), logEntry.User.GetAvatarUrl());
                    break;
                }
            }
        }

        private async Task Client_MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            SocketTextChannel textChannel = channel as SocketTextChannel;
            RestAuditLogEntry logEntry = await GetAuditLogEntry(textChannel.Guild);
            if (logEntry.Action != ActionType.MessageDeleted) return;
            MessageDeleteAuditLogData data = (MessageDeleteAuditLogData)logEntry.Data;
            IUser user = await channel.GetUserAsync(data.AuthorId);
            string targetUsername = "**" + user + "** _(" + data.AuthorId + ")_";
            string adminUsername = "**" + logEntry.User + "**";
            
            string msg = adminUsername + " deleted _" + data.MessageCount + "_ messages by " + targetUsername + 
                " in **#" + channel.Name + "** channel";

            foreach (SocketTextChannel chan in Global.SECURITY_CHANNELS)
            {
                if (chan.Guild.Id == textChannel.Guild.Id)
                {
                    if (!CheckPosted(logEntry, chan))
                        await SendSecurityLog(msg, new Color(255, 190, 0), chan, logEntry.Id.ToString(), logEntry.User.GetAvatarUrl());
                    break;
                }
            }
        }

        private async Task Client_GuildUpdated(SocketGuild arg1, SocketGuild arg2)
        {
            await RepeatingTimer.UpdateAuditLog();
        }

        private async Task Client_GuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            /*
            if (!userCheck(arg1, arg2)) await Task.CompletedTask;

            string targetUsername = "**" + arg1.Username + "** _(" + arg1.Id + ")_";
            string msg = "";
            if (arg1.IsDeafened != arg2.IsDeafened)
                msg += string.Format("{0} is now {1}\n", targetUsername, (arg2.IsDeafened) ? "deafened" : "undeafened");

            if (arg1.IsMuted != arg2.IsMuted)
                msg += string.Format("{0} is now {1}\n", targetUsername, (arg2.IsMuted) ? "muted" : "unmuted");


            if (arg1.Nickname != arg2.Nickname)
                msg += string.Format(adminUsername + " changed {0}'s nickname from *{1}* to *{2}*\n", targetUsername,
                    (string.IsNullOrEmpty(arg1.Nickname)) ? arg1.Username : arg1.Nickname,
                    (data.After.Nickname == null) ? data.Target.Username : data.After.Nickname);

            Console.WriteLine(msg);
            embed.WithColor(255, 255, 0);
            embed.WithAuthor(title, logEntry.User.GetAvatarUrl());
            embed.WithDescription(msg);
            embed.WithFooter(logEntry.Id.ToString());
            await chan.SendMessageAsync("", false, embed.Build());
            */
            await Task.CompletedTask;
        }
        private bool userCheck(SocketGuildUser user1, SocketGuildUser user2)
        {
            bool result = false;
            if (user1.IsDeafened != user2.IsDeafened) result = true;
            if (!result && user1.IsMuted != user2.IsMuted) result = true;
            if (!result && !user1.Roles.Equals(user2.Roles)) result = true;
            return result;
        }

        private async Task Client_UserUnbanned(SocketUser user, SocketGuild guild)
        {
            RestAuditLogEntry logEntry = await GetAuditLogEntry(guild);
            if (logEntry.Action != ActionType.Unban) return;
            UnbanAuditLogData data = (UnbanAuditLogData)logEntry.Data;
            string targetUsername = "**" + user + "** _(" + user.Id + ")_";
            string adminUsername = "**" + logEntry.User + "**";
            string msg = targetUsername + " was unbanned by " + adminUsername +
                "\nReason: " + (string.IsNullOrEmpty(logEntry.Reason) ? "No reason provided." : logEntry.Reason);

            foreach (SocketTextChannel channel in Global.SECURITY_CHANNELS)
            {
                if (channel.Guild.Id == guild.Id)
                {
                    await SendSecurityLog(msg, new Color(255, 0, 0), channel, logEntry.Id.ToString(), logEntry.User.GetAvatarUrl());
                    break;
                }
            }
        }

        private async Task Client_ChannelUpdated(SocketChannel channel, SocketChannel socketChannel)
        {
            await RepeatingTimer.UpdateAuditLog();
        }
        private async Task<RestAuditLogEntry> GetAuditLogEntry(SocketGuild guild)
        {
            IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> auditLog = guild.GetAuditLogsAsync(1);
            List<IReadOnlyCollection<RestAuditLogEntry>> auditList = await auditLog.ToList();
            RestAuditLogEntry[] restAuditLogs = auditList[0].ToArray();
            RestAuditLogEntry logEntry = restAuditLogs[0];
            return logEntry;
        }

        private bool CheckPosted(RestAuditLogEntry entry, SocketTextChannel securityChannel)
        {
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> messages = securityChannel.GetMessagesAsync(1);
            List<IReadOnlyCollection<IMessage>> msgList = messages.ToList().GetAwaiter().GetResult();
            IMessage mesg = null;
            if (msgList[1].Count > 0) mesg = msgList[1].ToArray()[0];// Newest Message
            else return false;

            if (mesg.Embeds.Count == 0) return false;
            else
            {
                if (mesg.Embeds.ToArray()[0].Footer == null) return false;
                if (mesg.Embeds.ToArray()[0].Footer.Value.Text == entry.Id.ToString())  return true;
                else return false;
            }
        }

        private async Task SendSecurityLog(string msg, Color color, SocketTextChannel channel, string footer, string authorURL = "")
        {
            EmbedBuilder embed = new EmbedBuilder();
            string title = "Server Report";
            embed.WithTimestamp(DateTimeOffset.Now);
            embed.WithColor(color);
            embed.WithAuthor(title, string.IsNullOrEmpty(authorURL) ? client.CurrentUser.GetAvatarUrl() : authorURL);
            embed.WithDescription(msg);
            embed.WithFooter(footer);
            await channel.SendMessageAsync("", false, embed.Build());
        }
        #endregion

        public static ulong tsMemberChannel = 512514458065567755;
        public static ulong memberChannel = 512515705036472320;

        List<SocketGuild> updated = new List<SocketGuild>();
        private async Task Client_MessageReceived(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(client, msg);
            ulong guildID = 0;
            SocketTextChannel generalchan = null;
            ServerConfig config = new ServerConfig();
            UserAccount userAccount = null;
            Antispam.SpamAccount spamAccount = null;
            //Load Accounts
            UserAccounts.LoadUserAccts();
            SteamAccounts.LoadUserAccts();
            ServerConfigs.LoadServerConfigs();
            NanoPool.LoadAccounts();
            MemeLoader.LoadMemes();
            Antispam.LoadSpamAccount();
            GiveawayManager.LoadGiveaways();

            //----------Do Tests--------
            //if (!context.IsPrivate) await RepeatingTimer.UpdateAuditLog(context.Guild);]

            //Mute Check
            userAccount = UserAccounts.GetAccount(context.User);
            spamAccount = Antispam.GetAccount(context.User);

            if (oldMessage.Count > 0)
            {
                if (oldMessage[oldMessage.Count - 1].Content == s.Content)
                {
                    for (int i = 0; i < oldMessage.Count; i++)
                    {
                        if (oldMessage[i].Content == s.Content && oldMessage[i].Embeds.Count == 0)
                        {
                            msgCount++;
                            if(msgCount >= 5)
                            {
                                try
                                {
                                    await s.DeleteAsync();
                                }
                                catch(Exception ex)
                                {
                                    string exMsg = DateTime.Now + " | EXCEPTION: " + ex.Message;
                                    Console.WriteLine(exMsg);
                                    Log.LogException(exMsg);
                                }
                                string mes = DateTime.Now + " | " + context.User + " suspected raid account. Ban request....";
                                Console.WriteLine(mes);
                            }
                        }
                    }
                }
            }
            if (oldMessage.Count >= 20)
                oldMessage.RemoveAt(0);
            oldMessage.Add(s);
            if (!context.IsPrivate)
            {
                config = ServerConfigs.GetConfig(context.Guild);
                RepeatingTimer.channel = (SocketTextChannel)context.Channel;
                if (!updated.Contains(context.Guild))
                {
                    ServerConfigs.UpdateServerConfig(context.Guild, config);
                    updated.Add(context.Guild);
                }

                if (config.RequiresVerification)
                {
                    if (GetTChannel(context.Guild.TextChannels, "verification") == null)
                        await context.Guild.CreateTextChannelAsync("verification");
                }

                if (userAccount.IsMuted)
                {
                    await context.Message.DeleteAsync();
                    return;
                }

                //Get Guild ID
                guildID = context.Guild.Id;
                DataStorage.AddPairToStorage(context.Guild.Name + "ID", guildID.ToString());
                generalchan = GetTChannel(context.Guild.TextChannels, "general");
            }
            //Bot Check
            if (!context.User.IsBot)
            {
                if (!context.IsPrivate)
                {
                    if (!config.AllowAdvertising)
                    {
                        if (context.Message.Content.Contains(context.Guild.EveryoneRole.ToString()) &&
                            context.Message.Content.Contains("https://discord.gg/"))
                        {
                            await context.Message.DeleteAsync();
                            await context.Channel.SendMessageAsync(context.User.Mention + " Advertising discord servers is not allowed here.");
                            return;
                        }
                    }
                    if (config.BlockMentionEveryone)
                    {
                        if (context.Message.Content.Contains(context.Guild.EveryoneRole.ToString()))
                        {
                            await context.Message.DeleteAsync();
                            await context.Channel.SendMessageAsync(context.User.Mention + " mentioning everyone is prohibited!");
                            return;
                        }

                    }
                    //Check for raid from multiple account spam
                    //Add when you wake up...
                    spamAccount.LastMessages.Add(DateTime.Now);
                    if (spamAccount.BanAmount > 0)
                    {
                        if (Math.Abs(spamAccount.LastBan.Subtract(DateTime.Now).Days) > 10)          //Reset ban amount after 10 days
                        {
                            spamAccount.BanAmount = 0;
                        }
                    }
                    if (spamAccount.LastMessages.Count > 3)
                    {
                        //Get last 4 messages sent
                        DateTime d1 = spamAccount.LastMessages[0];
                        DateTime d2 = spamAccount.LastMessages[1];
                        DateTime d3 = spamAccount.LastMessages[2];
                        DateTime d4 = spamAccount.LastMessages[3];
                        TimeSpan t1 = new TimeSpan();
                        TimeSpan t2 = new TimeSpan();
                        TimeSpan t3 = new TimeSpan();
                        //Subtract them from each other by Milliseconds
                        t1 = d1.Subtract(d2);
                        t2 = d2.Subtract(d3);
                        t3 = d3.Subtract(d4);
                        double mil1 = Math.Abs(t1.TotalMilliseconds);
                        double mil2 = Math.Abs(t2.TotalMilliseconds);
                        double mil3 = Math.Abs(t3.TotalMilliseconds);
                        //Console.WriteLine(mil1 + "\n" + mil2 + "\n" + mil3);

                        //If all past 4 messages are within spam threshold then its considerd spam
                        if (mil1 <= Antispam.millisecondThreshold &&    //Threshold is 5 seconds
                            mil2 <= Antispam.millisecondThreshold &&
                            mil3 <= Antispam.millisecondThreshold)
                        {
                            spamAccount.BanAmount++;
                            string message = "";
                            DateTime banTime = DateTime.Now;
                            if (spamAccount.BanAmount < config.AntiSpamWarn)
                            {
                                message = "\nPlease stop spamming you have been muted for 30 seconds!";
                                banTime = DateTime.Now.AddSeconds(30);
                            }
                            if (spamAccount.BanAmount >= config.AntiSpamWarn && spamAccount.BanAmount < config.AntiSpamThreshold)
                            {
                                int time = (int)config.AntiSpamTime;
                                message = "\nYou have been muted for " + time + " Minutes! " + context.Guild.Owner.Mention;
                                banTime = DateTime.Now.AddMinutes(time);
                            }
                            if (spamAccount.BanAmount > config.AntiSpamThreshold)
                            {
                                SocketGuildUser user = (SocketGuildUser)context.User;
                                await user.BanAsync(1, "Spamming");
                                await context.Channel.SendMessageAsync(context.User.Username + " was banned for 1 day for spamming!");
                                return;
                            }
                            spamAccount.BanTime = banTime;
                            spamAccount.LastBan = DateTime.Now;
                            await context.Channel.SendMessageAsync(context.User.Mention + message);
                            spamAccount.LastMessages.Clear();
                            Antispam.UpdateAccount(context.User, spamAccount);
                            Antispam.SaveAccounts();
                            userAccount.IsMuted = true;
                            UserAccounts.SaveAccounts();
                            return;
                        }
                        spamAccount.LastMessages.Clear();
                    }
                    Antispam.SaveAccounts();
                    if (config.EnableServerStats)
                    {
                        //If stat channels dont exist
                        await CreateStatChannels(context.Guild);
                        await updMemberChan(context.Guild);     //Update Stat channels
                    }

                    if (config.EnableLevelSystem)
                    {
                        uint oldlvl = userAccount.LevelNumber;
                        userAccount.XP += 10;                   //Xp gain
                        userAccount.Points += 10;               //Pointshop

                        if (oldlvl != userAccount.LevelNumber)
                        {
                            for (int i = 0; i < LevelingSytem.levelUp.Length; i++)
                            {
                                if (userAccount.LevelNumber == LevelingSytem.levelUp[i])
                                {

                                    IGuildUser user = (IGuildUser)context.User;

                                    IRole[] r = user.Guild.Roles.ToArray();
                                    IRole addrole = null;
                                    for (int i2 = 0; i2 < r.Length; i2++)
                                    {
                                        if (r[i2].Name.ToLower() == LevelingSytem.upgradeRoles[i].ToLower())
                                        {
                                            addrole = r[i2];
                                            break;
                                        }
                                    }
                                    if (addrole != null)
                                    {
                                        ulong? roleID = null;
                                        foreach (ulong r2 in user.RoleIds)
                                        {
                                            if (r2 == addrole.Id)
                                            {
                                                roleID = r2;
                                                break;
                                            }
                                        }//end foreach loop
                                        if (roleID == null)
                                        {
                                            await user.AddRoleAsync(addrole);
                                            await context.Channel.SendMessageAsync("__" + user.Username + "__ earned role **" + addrole + "**!", false);
                                        }//end if roleID
                                    }//end if addrole != null
                                }// end if level up
                            }//end levels loop
                            string message = "Congrats " + context.User.Mention + ", You just advanced to **Level " + userAccount.LevelNumber + "!**";

                            await context.Channel.SendMessageAsync(message);
                        }//end level check
                    }//end level system check
                }//end if EnableLevelSystem
                UserAccounts.SaveAccounts();
            }//end isPrivate check
            int argPos = 0;

            if (msg.HasStringPrefix("" + config.Prefix, ref argPos)
                || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                if (context.IsPrivate) config = new ServerConfig();
                else
                    config = ServerConfigs.GetConfig(context.Guild);
                IResult result = null;
                result = await service.ExecuteAsync(context, argPos, null);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    bool errorCheck = true;
                    if (context.Message.Content.Contains("creatememe") && context.Message.Attachments.Count > 0)
                    {
                        if (result.ErrorReason.Contains("User not found"))
                        {
                            errorCheck = false;
                        }
                    }
                    if (errorCheck)
                    {
                        string message = "Error when running this command\n**" + result.ErrorReason + "**\n\nView console for more info";
                        var embed = new EmbedBuilder();
                        embed.WithTitle("Command Error");
                        embed.WithDescription(message);
                        embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                        if (config.FooterText != "")
                            embed.WithFooter(config.FooterText);
                        if (config.TimeStamp)
                            embed.WithCurrentTimestamp();

                        await context.Channel.SendMessageAsync("", false, embed.Build());
                        Console.WriteLine(result.ErrorReason);
                        Console.WriteLine(result.Error);
                        Log.LogException(result.ErrorReason);
                        Log.LogException(result.Error.Value.ToString());
                    }
                }
            }
        }


        public async Task CreateStatChannels(SocketGuild guild)
        {
            if (GetVChannel(guild.VoiceChannels, "Bot Count") == null)
            {
                int botcount = 0;
                IReadOnlyCollection<SocketGuildUser> users = guild.Users;
                foreach (SocketGuildUser user in users)
                {
                    if (user.IsBot)
                        botcount++;
                }
                RestVoiceChannel x = await guild.CreateVoiceChannelAsync("Bot Count: " + botcount);
                await x.ModifyAsync(m => { m.Position = 1; m.UserLimit = 0; });
            }
            if (GetVChannel(guild.VoiceChannels, "User Count") == null)
            {
                int botcount = 0;
                IReadOnlyCollection<SocketGuildUser> users = guild.Users;
                foreach (SocketGuildUser user in users)
                {
                    if (user.IsBot)
                        botcount++;
                }
                RestVoiceChannel z = await guild.CreateVoiceChannelAsync("User Count: " + (users.Count - botcount));
                await z.ModifyAsync(m => { m.Position = 2; m.UserLimit = 0; });
            }
            if (GetVChannel(guild.VoiceChannels, "Member Count") == null)
            {
                IReadOnlyCollection<SocketGuildUser> users = guild.Users;
                RestVoiceChannel z = await guild.CreateVoiceChannelAsync("Member Count: " + users.Count);
                await z.ModifyAsync(m => { m.Position = 0; m.UserLimit = 0; });
            }

        }


        Random rand = new Random();
        private async Task Client_UserLeft(SocketGuildUser user)
        {
            await RepeatingTimer.UpdateAuditLog(user.Guild);
            ServerConfig config = ServerConfigs.GetConfig(user.Guild);
            string username = user.Username + "#" + user.Discriminator;
            if (config.NewUserMessage)
            {
                var channel = client.GetChannel(config.NewUserChannel) as SocketTextChannel; // Gets the channel to send the message in
                if (config.EnableServerStats)
                {
                    await CreateStatChannels(user.Guild);
                    await updMemberChan(user.Guild);
                }
                string msg = String.Format(Global.Bye[rand.Next(Global.Bye.Length)], username, user.Guild) + " 👋"; //Bye message
                /*
                var embed = new EmbedBuilder();
                embed.WithTitle($"{user.Username} Left");   
                embed.WithThumbnailUrl(user.GetAvatarUrl());
                embed.WithDescription(msg);
                embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                if (config.FooterText != "")
                    embed.WithFooter(config.FooterText);
                if (config.TimeStamp)
                    embed.WithCurrentTimestamp();
                if (user.Id != BotID)
                    await channel.SendMessageAsync("", false, embed.Build());   //Send byebye
                */
                if (user.Id != BotID)
                    await channel.SendMessageAsync(msg);   //Send byebye
            }
            Console.WriteLine(DateTime.Now.ToLocalTime() + " | " + username + " left " + user.Guild.Name);
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            await RepeatingTimer.UpdateAuditLog(user.Guild);
            ServerConfig config = ServerConfigs.GetConfig(user.Guild);
            string username = user.Username + "#" + user.Discriminator;
            if (config.NewUserMessage)
            {
                var channel = client.GetChannel(config.NewUserChannel) as SocketTextChannel; // Gets the channel to send the message in
                if (config.EnableServerStats)
                {
                    await CreateStatChannels(user.Guild);
                    await updMemberChan(user.Guild);
                }

                UserAccount account = UserAccounts.GetAccount(user);
                string msg = String.Format(Global.Welcome[rand.Next(Global.Welcome.Length)], username, user.Guild);  //Welcome Message
                /*
                var embed = new EmbedBuilder();
                embed.WithTitle("New User Joined!");
                embed.WithDescription(msg);
                embed.WithThumbnailUrl(user.GetAvatarUrl());
                embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                if (config.FooterText != "")
                    embed.WithFooter(config.FooterText);
                if (config.TimeStamp)
                    embed.WithCurrentTimestamp();
                if (user.Id != BotID)
                    await channel.SendMessageAsync("", false, embed.Build());   //Welcomes the new user
                */
                if (user.Id != BotID)
                    await channel.SendMessageAsync(msg);   //Welcomes the new user
            }
            Console.WriteLine(DateTime.Now.ToLocalTime() + " | " + username + " joined " + user.Guild.Name);

            if (config.RequiresVerification)
            {
                if (GetTChannel(user.Guild.TextChannels, "verification") == null)
                    await user.Guild.CreateTextChannelAsync("verification");

                await GetTChannel(user.Guild.TextChannels, "verification").SendMessageAsync("Type " + config.Prefix + "verify to verify your account.");
            }
        }


        public static SocketVoiceChannel GetVChannel(IEnumerable<SocketVoiceChannel> channels, string channelName)
        {
            foreach (var channel in channels)
            {
                if (channel.Name.ToLower().Contains(channelName.ToLower()))
                {
                    if (!channelName.Contains("Member Count") && !channelName.Contains("User Count") && !channelName.Contains("Bot Count"))
                        DataStorage.AddPairToStorage(channel.Guild.Name + " | " + channel.Name, channel.Id.ToString());

                    return channel;
                }
            }
            return null;
        }

        public static SocketTextChannel GetTChannel(IEnumerable<SocketTextChannel> channels, string channelName)
        {
            foreach (var channel in channels)
            {
                if (channel.Name.ToLower().Contains(channelName.ToLower()))
                {
                    DataStorage.AddPairToStorage(channel.Guild.Name + " | " + channel.Name, channel.Id.ToString());
                    return channel;
                }
            }
            return null;
        }




        public static SocketGuildUser GetUser(IEnumerable<SocketGuildUser> users, string username)
        {
            foreach (var user in users)
            {
                if (user.Username.Contains(username))
                {
                    return user;
                }
            }
            return null;
        }

        public static async Task updMemberChan(SocketGuild guild)
        {
            bool updateChan = false;
            SocketVoiceChannel MemberChan = null;
            SocketVoiceChannel UserChan = null;
            SocketVoiceChannel BotChan = null;
            foreach (SocketVoiceChannel chan in guild.VoiceChannels)
            {
                if (chan.Name.Contains("User Count"))
                {
                    UserChan = chan;
                }
                else if (chan.Name.Contains("Bot Count"))
                {
                    BotChan = chan;
                }
                else if (chan.Name.Contains("Member Count"))
                {
                    MemberChan = chan;
                    int members = int.Parse(chan.Name.Split(':')[1]);
                    if (members != chan.Guild.Users.Count)
                        updateChan = true;
                    if (!updateChan) break;
                }
            }
            if (!updateChan) return;
            int botcount = 0;
            IReadOnlyCollection<SocketGuildUser> users = guild.Users;
            foreach (SocketGuildUser u in users)
            {
                if (u.IsBot)
                    botcount++;
            }
            int memberCount = MemberChan.Guild.MemberCount;
            await MemberChan.ModifyAsync(m => { m.Name = "Member Count: " + memberCount; });
            await UserChan.ModifyAsync(m => { m.Name = "User Count: " + (memberCount - botcount); });
            await BotChan.ModifyAsync(m => { m.Name = "Bot Count: " + botcount; });

        }

        public static async void SendMessage(string msg, string title, Color color,SocketTextChannel channel, SocketGuildUser user)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime() + " | " + channel.Name + " | " + user.Username + " | " + title); //Write to console
            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(msg);
            embed.WithColor(color);
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithFooter("Created by Abyscuit");
            if (user.Id != BotID)
                await channel.SendMessageAsync("", false, embed.Build());   //Send Message
        }
        public struct EmbedField
        {
            public string name;
            public string value;
        }
        public async Task PrintEmbedMessage(SocketTextChannel channel, string title = "", string msg = "", string url = "", string iconUrl = "", EmbedField[] fields = null)
        {
            ServerConfig config = new ServerConfig();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (title != "")
                embed.WithTitle(title);
            if (config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (msg != "")
                embed.WithDescription(msg);
            if (url != "")
                embed.WithUrl(url);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            if (iconUrl != "")
                embed.WithThumbnailUrl(iconUrl);
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    embed.AddField(fields[i].name, fields[i].value, true);
                }
            }

            //Console.WriteLine(DateTime.Now + " | " + title + " |\n" + msg);
            string username = "System";
            string guildName = channel.Guild.Name; 
            Console.WriteLine(DateTime.Now + " | " + username + " used " + title + " in " + guildName);
            await channel.SendMessageAsync("", false, embed.Build());

        }
    }
}
