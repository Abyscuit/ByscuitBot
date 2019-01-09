using byscuitBot.Core;
using byscuitBot.Core.Server_Data;
using byscuitBot.Core.Steam_Accounts;
using byscuitBot.Core.User_Accounts;
using byscuitBot.Modules;
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
        CommandService service;
        public static ulong BotID = 510066148285349900;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            this.client = client;
            service = new CommandService();
            await service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            this.client.MessageReceived += Client_MessageReceived;
            this.client.UserJoined += Client_UserJoined;
            this.client.UserLeft += Client_UserLeft;
        }
        
        public static ulong tsMemberChannel = 512514458065567755;
        public static ulong memberChannel = 512515705036472320;
        //ulong welcomeChannel = 0;
        ulong general = 246718514214338560; //replace with default welcome
                                            //ulong test = 512047865917603852;

        List<SocketGuild> updated = new List<SocketGuild>();
        private async Task Client_MessageReceived(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(client, msg);
            

            //Load Accounts
            UserAccounts.LoadUserAccts();
            SteamAccounts.LoadUserAccts();
            ServerConfigs.LoadServerConfigs();
            NanoPool.LoadAccounts();
            MemeLoader.LoadMemes();
            Antispam.LoadSpamAccount();
            GiveawayManager.LoadGiveaways();
            ServerConfig config = ServerConfigs.GetConfig(context.Guild);
            RepeatingTimer.channel = (SocketTextChannel)context.Channel;
            if (!updated.Contains(context.Guild))
            {
                ServerConfigs.UpdateServerConfig(context.Guild, config);
                updated.Add(context.Guild);
            }

            if(config.RequiresVerification)
            {
                if (GetTChannel(context.Guild.TextChannels, "verification") == null)
                    await context.Guild.CreateTextChannelAsync("verification");
            }
            //Mute Check
            var userAccount = UserAccounts.GetAccount(context.User);
            var spamAccount = Antispam.GetAccount(context.User);

            if (userAccount.IsMuted)
            {
                await context.Message.DeleteAsync();
                return;
            }

            //Get Guild ID
            var guildID = context.Guild.Id;
            DataStorage.AddPairToStorage(context.Guild.Name + "ID", guildID.ToString());
            var generalchan = GetTChannel(context.Guild.TextChannels, "general");


            //Bot Check
            if (!context.User.IsBot)
            {
                spamAccount.LastMessages.Add(DateTime.Now);
                if(spamAccount.BanAmount > 0)
                {
                    if(Math.Abs(spamAccount.LastBan.Subtract(DateTime.Now).Days) > 10)
                    {
                        spamAccount.BanAmount = 0;
                    }
                }
                if (spamAccount.LastMessages.Count > 3)
                {
                    DateTime d1 = spamAccount.LastMessages[0];
                    DateTime d2 = spamAccount.LastMessages[1];
                    DateTime d3 = spamAccount.LastMessages[2];
                    DateTime d4 = spamAccount.LastMessages[3];
                    TimeSpan t1 = new TimeSpan();
                    TimeSpan t2 = new TimeSpan();
                    TimeSpan t3 = new TimeSpan();
                    t1 = d1.Subtract(d2);
                    t2 = d2.Subtract(d3);
                    t3 = d3.Subtract(d4);
                    double mil1 = Math.Abs(t1.TotalMilliseconds);
                    double mil2 = Math.Abs(t2.TotalMilliseconds);
                    double mil3 = Math.Abs(t3.TotalMilliseconds);
                    Console.WriteLine(mil1 + "\n" + mil2 + "\n" + mil3);
                    if (mil1 <= Antispam.millisecondThreshold && 
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
                            message = "\nYou have been muted for " +time+ " Minutes! " + context.Guild.Owner.Mention;
                            banTime = DateTime.Now.AddMinutes(time);
                        }
                        if(spamAccount.BanAmount > config.AntiSpamThreshold)
                        {
                            SocketGuildUser user = (SocketGuildUser)context.User;
                            await user.BanAsync(1,"Spamming");
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
                //If stat channels dont exist
                await CreateStatChannels(context.Guild);
                    
                await updMemberChan(context.Guild);     //Update Stat channels
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
                                }
                                if (roleID == null)
                                {
                                    await user.AddRoleAsync(addrole);
                                    await context.Channel.SendMessageAsync("__" + user.Username + "__ earned role **" + addrole + "**!", false);
                                }
                            }
                        }
                    }
                    string message = "Congrats " + context.User.Mention + ", You just advanced to **Level " + userAccount.LevelNumber + "!**";

                    await context.Channel.SendMessageAsync(message);
                }
            }
            UserAccounts.SaveAccounts();

            int argPos = 0;


            if (msg.HasStringPrefix("" + config.Prefix, ref argPos)
                || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                /*
                bool canSend = true;
                bool mining = false;
                foreach (string cmd in Misc.miningCmds)
                {
                    if (msg.Content.Contains(cmd))
                    {
                        mining = true;
                        break;
                    }
                }
                if (mining)
                {
                    if (config.WhiteListMining != null)
                    {
                        if (config.WhiteListMining.Count() != 0)
                            if (!config.WhiteListMining.Contains(msg.Channel.Id))
                                canSend = false;
                    }
                }
                bool meme = false;
                if (msg.Content.Contains("meme"))
                {
                    meme = true;
                }
                if (meme)
                {
                    if (config.BlackListMeme != null)
                    {
                        if (config.BlackListMeme.Contains(msg.Channel.Id))
                            canSend = false;
                    }
                }
                if (config.BlackListCMDs != null)
                {
                    if (config.BlackListCMDs.Contains(msg.Channel.Id))
                        canSend = false;
                }
                */
                //if (canSend)
                {
                    var result = await service.ExecuteAsync(context, argPos, null);
                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        var role = from r in context.Guild.Roles
                                   where r.Permissions.Administrator && r.Name != "ByscuitBot"
                                   select r.Mention;

                        string rolename = role.FirstOrDefault();
                        bool errorCheck = true;
                        if(context.Message.Content.Contains("creatememe") && context.Message.Attachments.Count > 0)
                        {
                            if(result.ErrorReason.Contains("User not found"))
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
                        }
                    }
                }
            }
            GetAllVoiceChannels(context.Guild.VoiceChannels);
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
            ulong currchar = general;
            //if (debug == 1) currchar = test;
            //else currchar = general;

            var role = from r in user.Guild.TextChannels
                       where r.Name.ToLower().Contains("general")
                       select r;
            var result = role.FirstOrDefault();
            SocketTextChannel chanResult = null;
            foreach (SocketTextChannel t in user.Guild.TextChannels)
            {
                if (t.Name.ToLower().Contains("bye"))
                {
                    chanResult = t;
                    break;
                }

            }
            if (chanResult == null)
            {
                foreach (SocketTextChannel t in user.Guild.TextChannels)
                {
                    if (t.Name.ToLower().Contains("welcome"))
                    {
                        chanResult = t;
                        break;
                    }

                }
                if (chanResult == null)
                {
                    foreach (SocketTextChannel t in user.Guild.TextChannels)
                    {
                        if (t.Name.ToLower().Contains("general"))
                        {
                            chanResult = t;
                            break;
                        }

                    }
                }
            }
            if (chanResult == null)
                chanResult = result;
            GetTChannel(user.Guild.TextChannels, chanResult.Name);


            var channel = client.GetChannel(chanResult.Id)  as SocketTextChannel; // Gets the channel to send the message in

            await CreateStatChannels(user.Guild);
            await updMemberChan(user.Guild);

            ServerConfig config = ServerConfigs.GetConfig(user.Guild);
            string msg = String.Format(Global.Bye[rand.Next(Global.Bye.Length)], user.Mention, user.Guild) + "\n👋"; //Bye message
            var embed = new EmbedBuilder();
            embed.WithTitle($"{user.Username} Left");
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithDescription(msg);
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if(config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            if (user.Id != BotID )
                await channel.SendMessageAsync("", false, embed.Build());   //Send byebye
            Console.WriteLine(DateTime.Now.ToLocalTime() + " | " + user.Username + " left " + channel.Guild.Name);
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            ulong currchar = general;
            //if (debug == 1) currchar = test;
            //else currchar = general;
            
            var role = from r in user.Guild.TextChannels
                       where r.Name.ToLower().Contains("general")
                       select r;
            var result = role.FirstOrDefault();
            SocketTextChannel chanResult = null;
            foreach (SocketTextChannel t in user.Guild.TextChannels)
            {
                if (t.Name.ToLower().Contains("bye"))
                {
                    chanResult = t;
                    break;
                }

            }
            if (chanResult == null)
            {
                foreach (SocketTextChannel t in user.Guild.TextChannels)
                {
                    if (t.Name.ToLower().Contains("welcome"))
                    {
                        chanResult = t;
                        break;
                    }

                }
                if (chanResult == null)
                {
                    foreach (SocketTextChannel t in user.Guild.TextChannels)
                    {
                        if (t.Name.ToLower().Contains("general"))
                        {
                            chanResult = t;
                            break;
                        }

                    }
                }
            }
            if (chanResult == null)
                chanResult = result;
            GetTChannel(user.Guild.TextChannels, chanResult.Name);
            var channel = client.GetChannel(chanResult.Id) as SocketTextChannel; // Gets the channel to send the message in

            await CreateStatChannels(user.Guild);
            await updMemberChan(user.Guild);


            ServerConfig config = ServerConfigs.GetConfig(user.Guild);
            UserAccount account = UserAccounts.GetAccount(user);
            string msg = String.Format(Global.Welcome[rand.Next(Global.Welcome.Length)], user.Mention, user.Guild);  //Welcome MEssage
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
            Console.WriteLine(DateTime.Now.ToLocalTime() + " | " + user.Username + " joined " + channel.Guild.Name);

            if (config.RequiresVerification)
            {
                if (GetTChannel(user.Guild.TextChannels, "verification") == null)
                    await user.Guild.CreateTextChannelAsync("verification");

                await GetTChannel(user.Guild.TextChannels, "verification").SendMessageAsync("Type " + config.Prefix + "verify to verify your account.");
            }
        }

        public static ulong GetCurrentGuild(SocketTextChannel channel)
        {
            var result = from s in channel.Guild.VoiceChannels
                         where s.Name.Contains("Member Count")
                         select s;


            SocketVoiceChannel memberChan = result.FirstOrDefault();
            foreach (var pair in DataStorage.pairs)
            {
                if (pair.Key.Contains("MemberChannel"))
                {
                    if (pair.Value == memberChan.Id.ToString())
                    {
                        return ulong.Parse(pair.Value);
                    }
                }
            }
            GetTChannel(channel.Guild.TextChannels, "welcome");
            return memberChan.Id;
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


        public static void GetAllVoiceChannels(IEnumerable<SocketVoiceChannel> channels)
        {
            foreach (var channel in channels)
            {
                string channelName = channel.Name;
                if(!channelName.Contains("Member Count") && !channelName.Contains("User Count") && !channelName.Contains("Bot Count"))
                    DataStorage.AddPairToStorage(channel.Guild.Name + " | " + channelName, channel.Id.ToString());
            }
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
            int botcount = 0;
            IReadOnlyCollection<SocketGuildUser> users = guild.Users;
            foreach (SocketGuildUser u in users)
            {
                if (u.IsBot)
                    botcount++;
            }
            bool updateChan = false;
            foreach (SocketVoiceChannel chan in guild.VoiceChannels)
            {
                if (chan.Name.Contains("Member Count"))
                {
                    if (!updateChan)
                    {
                        int members = int.Parse(chan.Name.Split(':')[1]);
                        if (members != chan.Guild.Users.Count)
                        {
                            updateChan = true;
                            break;
                        }
                    }
                }
            }
            foreach (SocketVoiceChannel chan in guild.VoiceChannels)
            {
                if(chan.Name.Contains("Member Count") && updateChan)
                {
                        await chan.ModifyAsync(m => { m.Name = "Member Count: " + chan.Guild.MemberCount; });
                        DataStorage.AddPairToStorage(chan.Guild.Name + " MemberChannel", chan.Id.ToString());
                }
                if (chan.Name.Contains("User Count") && updateChan)
                {
                    await chan.ModifyAsync(m => { m.Name = "User Count: " + (chan.Guild.MemberCount - botcount); });
                    DataStorage.AddPairToStorage(chan.Guild.Name + " UserChannel", chan.Id.ToString());
                }
                if (chan.Name.Contains("Bot Count") && updateChan)
                {
                    await chan.ModifyAsync(m => { m.Name = "Bot Count: " + botcount; });
                    DataStorage.AddPairToStorage(chan.Guild.Name + " BotChannel", chan.Id.ToString());
                }
            }
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
    }
}
