using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using byscuitBot.Core;
using byscuitBot.Core.User_Accounts;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3.Data;
using Discord.Rest;
using System.IO;
using System.Security.Cryptography;
using byscuitBot.Core.Steam_Accounts;
using System.Runtime;
using System.Net;
using byscuitBot.Core.Server_Data;

namespace byscuitBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        string[] cmds = { "-------------Server Stuff-------------" ,"roles", "kick", "ban", "unban", "addrole", "addroles", "removerole", "removeallroles",
            "mute", "unmute", "move", "stats", "level", "addxp","subxp", "warn", "pointshop", "serverstats", "invlink", "nickname", "invite", "giveaway", "clear",
            "\n\n------------Media Stuff--------------", "youtube", "select", "meme", "creatememe", "\n\n--------Steam Commands-----------",
        "resolve", "steaminfo", "steambans", "steamgames", "linksteam", "steamaccts", "csgostats", "csgolastmatch", "csgolastmatches", "csgowepstats"};
        string[] desc = { "", "Displays your roles or others roles with @username", "Kick @username 'reason'", "ban @username days 'reason'", "unban @username",
            "adds a role to a user | addrole @username role_name", "adds roles to a user | addrole @username role_name, role_name, role_name", "remove a role from a user", "removes all roles from a user",
            "Mutes a user", "Unmute a user", "Move a user to a Voice Channel", "Get stats of a user", "Get account level of user", "Add XP to a user",
            "Remove XP to a user", "Warn a user", "Lists all the items in the pointshop", "Displays server the servers currents stats", "Get the invite link",
            "Change nickname of a user", "Get an Invite Link for the server", "Create a giveaway | Usage: giveaway <9d23h59m,Item to give>", "Clear messages in bulk | Usage: clear <number>",
            "", "Search YouTube for a keyword", "Select an option displayed",
            "Post a random or specific meme | Usage: meme <optional> keyword",
            "Create a meme using a members avatar | Usage: creatememe @user <top text,bottom text>", "", "Resolve steam URL or username to SteamID64",
            "Get Account Summary of your linked account or a steam user", "Get steam account bans (VAC, community, economy)", "Get All steam games or time played for a specific game",
            "Link steam account to Discord Account", "Get all steam accounts linked to Discord users", "Get relevant CS:GO stats (totals, last match)",
            "Get CS:GO last match info", "Get CS:GO info for up to 10 games of your last matches saved", "Get CS:GO weapon stats of all weapons or a specific weapon"
            };

        string invLink = "https://discord.gg/VYMph8k";
        public Random rand = new Random();

        string[] shop = { "Colin's Bitches", "Well-Reknown Byscuit", "Seasoned Byscuit", "Professional Byscuit", "BUTTERED UP BYSCUIT", "DaCrew", "Moderator" };
        int[] price = { 200, 400, 800, 1600, 3200, 6000, 6500 };

       
        #region users


        [Command("stats")]
        public async Task Stats(SocketGuildUser user = null)
        {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            string username = Context.User.Username;

            if (user != null)
            {

                account = UserAccounts.GetAccount(user);
                username = user.Username;
            }
            else
                user = CommandHandler.GetUser(Context.Guild.Users, username);
            string permissions = "\n__Permissions__\nAdmin: **" + user.GuildPermissions.Administrator + "**\nManage Roles: **" + user.GuildPermissions.ManageRoles + "**\nMute Members: **" + user.GuildPermissions.MuteMembers +
                "**\nDeafen Members: **" + user.GuildPermissions.DeafenMembers + "**\nKick Members: **" + user.GuildPermissions.KickMembers + "**\nBan Members: **" + user.GuildPermissions.BanMembers +
                "**\nChange Members Nickname: **" + user.GuildPermissions.ManageNicknames + "**\nMove Members: **" + user.GuildPermissions.MoveMembers+"**";
            string msg = "**Level:** " + account.LevelNumber + "\n**XP:** " + account.XP + "\n**Points:** " + account.Points + "\n" +
                "**IsMuted:** " + account.IsMuted + "\n**Number of Warnings:** " + account.NumberOfWarnings + "\n**Joined At:** " + user.JoinedAt +
                "\n\n__Roles__\n**" + getRoles(user) +"**"+ permissions; 
            //string msg = "XP: " + account.XP + "\nPoints: " + account.Points;
            


            
            await PrintEmbedMessage("Stats for " + username, msg, iconUrl: user.GetAvatarUrl());
        }

        //giveaway <time> <winners> <Item>
        [Command("giveaway")]
        public async Task Giveaway([Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            string[] split = text.Split(',');
            string time = split[0];
            string item = split[1];
            DateTime endTime = DateTime.Now;
            bool timeChk = false;
            if(time.ToLower().Contains("d"))
            {
                string[] spl = time.ToLower().Split('d');
                int days = int.Parse(spl[0]);
                endTime = endTime.AddDays(days);
                if (time.ToLower().Contains("h"))
                {
                    string[] spl2 = spl[1].Split('h');
                    string hrs = spl2[0];
                    int hours = int.Parse(hrs);
                    endTime = endTime.AddHours(hours);
                    if (time.ToLower().Contains("m"))
                    {
                        string min = spl2[1].Split('m')[0];
                        int mins = int.Parse(min);
                        endTime = endTime.AddMinutes(mins);
                    }
                }
                timeChk = true;
            }
            if (time.ToLower().Contains("h") && !timeChk)
            {
                string[] spl2 = time.Split('h');
                string hrs = spl2[0];
                int hours = int.Parse(hrs);
                endTime = endTime.AddHours(hours);
                if (time.ToLower().Contains("m"))
                {
                    string min = spl2[1].Split('m')[0];
                    int mins = int.Parse(min);
                    endTime = endTime.AddMinutes(mins);
                }
                timeChk = true;
            }
            if (time.ToLower().Contains("m") && !timeChk)
            {
                string min = time.Split('m')[0];
                int mins = int.Parse(min);
                endTime = endTime.AddMinutes(mins);
            }
            var embed = new EmbedBuilder();
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            embed.WithTitle(item);
            embed.WithDescription("React with 🎉 to enter!\n**Giveaway Ends at: "+endTime.ToShortDateString() + " " + endTime.ToShortTimeString() + "**");
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            embed.WithFooter(config.FooterText);



            RepeatingTimer.channel = (SocketTextChannel)Context.Channel;
            RestUserMessage message = await Context.Channel.SendMessageAsync("", false, embed.Build());
            GiveawayManager.MakeGiveaway(Context.User, item, message, endTime);
            //Global.MessageIdToTrack = message.Id;
        }

        [Command("timer")]
        public async Task Timer(int minutes)
        {
            await Context.Message.DeleteAsync();

            var embed = new EmbedBuilder();
            DateTime time = DateTime.Now;
            time = time.AddMinutes(minutes);
            RepeatingTimer.minutes = minutes;
            RepeatingTimer.channel = (SocketTextChannel)Context.Channel;
            RepeatingTimer.timeToStop = time;
            RepeatingTimer.startTimer = true;
            embed.WithTitle("Timer Ends " + time.ToShortDateString() + " " +time.ToShortTimeString() + "!");
            embed.WithColor(100, 150, 255);
            embed.WithFooter("Created by Abyscuit");



            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("react")]
        public async Task React([Remainder]string msg)
        {
            await Context.Message.DeleteAsync();

            Global.Guild = Context.Guild;
            var embed = new EmbedBuilder();
            string[] emojies = new string[Global.emojies.Length];
            IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;
            
            string emojiesTxt = "";
            foreach(GuildEmote emote in emotes)
            {
                emojiesTxt += emote +" ";
            }

            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            embed.WithTitle("React");
            embed.WithDescription(msg + "\nReact with " + emojiesTxt);
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            embed.WithFooter(config.FooterText);
            


            RestUserMessage message = await Context.Channel.SendMessageAsync("", false, embed.Build());
            Global.MessageIdToTrack = message.Id;
        }

        [Command("level")]
        public async Task Level(SocketGuildUser user = null)
        {
            UserAccount account;
            string username = "";
            account = UserAccounts.GetAccount(Context.User);
            username = Context.User.Username;
            if (user != null) {
                account = UserAccounts.GetAccount(user);
                username = user.Username;
            }
            else
                user = (SocketGuildUser)Context.User;

            string msg = account.LevelNumber.ToString();
            
            await PrintEmbedMessage("Level for " + username , msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("serverstats")]
        public async Task ServerStats()
        {
            int memberCount = Context.Guild.MemberCount;
            DateTimeOffset date = Context.Guild.CreatedAt;
            string name = Context.Guild.Name;
            string owner = Context.Guild.Owner.Username;

            ServerConfig serverConfig = ServerConfigs.GetConfig(Context.Guild);
            bool reqV = serverConfig.RequiresVerification;
            string verification = string.Format("\nRequires Verification: **{0}**", reqV);
            if(reqV)
            {
                SocketRole vRole = Context.Guild.GetRole(serverConfig.VerificationRoleID);
                verification += string.Format("\nVerified Role: **{0}**", vRole.Name);
            }
            string configs = "\nBot Prefix: **"+ serverConfig.Prefix +"**" + verification + "\nAFK Channel: **" +Context.Guild.GetVoiceChannel(serverConfig.AFKChannelID).Name +
                "**\nAFK Timeout: **" + serverConfig.AFKTimeout + "**\nAntispam Mute Time: **"+serverConfig.AntiSpamTime+"**\nAntispam Warn Ban: **"+serverConfig.AntiSpamWarn;
            string msg = "Server Name: **" + name + "**\nOwner: **"+ owner + "**\nMember Count: **" + memberCount + "**\nDate Created: **" + date.ToString("MM/dd/yyyy hh:mm:ss") +"**"+ configs +
                "**\n\n__Roles__\n**" + getAllRoles((SocketGuildUser)Context.User)+"**";
            

            var result = from s in Context.Guild.VoiceChannels
                         where s.Name.Contains("Member Count")
                         select s;

            
            await PrintEmbedMessage("Server Stats", msg, iconUrl: Context.Guild.IconUrl);
            SocketVoiceChannel memberChan = result.FirstOrDefault();
            await memberChan.ModifyAsync(m => { m.Name = "Member Count: " + Context.Guild.MemberCount; });
            DataStorage.AddPairToStorage(memberChan.Guild.Name + " MemberChannel", memberChan.Id.ToString());

        }

        [Command("addxp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task AddXP(uint amount, SocketGuildUser user = null)
        {
            UserAccount account;
            string username = "";

            if (user == null)
                user = (SocketGuildUser)Context.User;

                account = UserAccounts.GetAccount(user);
                username = user.Username;

            account.XP += amount;
            UserAccounts.SaveAccounts();
            string msg = "Added **" + amount + "XP** to **" + username +
                "**\nTotal XP: **" + account.XP + "**";
            
            await PrintEmbedMessage("XP Added " + username, msg, iconUrl: user.GetAvatarUrl());
        }
        [Command("subxp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task SubXP(uint amount, SocketGuildUser user = null)
        {
            UserAccount account;
            string username = "";
            if (user == null)
                user = (SocketGuildUser)Context.User;
            account = UserAccounts.GetAccount(user);
            username = user.Username;
            if (account.XP <= amount)
                account.XP = 0;
            else
                account.XP -= amount;
            UserAccounts.SaveAccounts();
            string msg = "Subtracted **" + amount + "XP** from **" + username +
                "**\nTotal XP:**" + account.XP + "**";

            await PrintEmbedMessage("XP Subtracted " + username, msg, iconUrl: user.GetAvatarUrl());
        }

        /*
        [Command("play")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        [RequireBotPermission(GuildPermission.MoveMembers)]
        public async Task Play([Remainder]string keyword) {
                if (!message.member.voiceChannel) return message.channel.send(':no_entry_sign: Please join a voice channel.');
                if (message.guild.me.voiceChannel) return message.channel.send(':no_entry_sign: Error, the bot is already connected to another music channel or a song is playing.');
                if (!args[0]) return message.channel.send(':no_entry_sign: Error, please enter a **URL** following the command.');

                let validate = await ytdl.validateURL(args[0]);

                if (!validate) return message.channel.send(':no_entry_sign: Error, please input a __valid__ url following the command.');

                let info = await ytdl.getInfo(args[0]);

                let connection = await message.member.voiceChannel.join();
                let dispatcher = await connection.playStream(ytdl(args[0], {

                    filter: 'audioonly'
            }));

                let playembed = new Discord.RichEmbed()
                .setTitle("Now playing")
                .setDescription(`${ info.title}`)
    
    message.channel.send(playembed);

            SocketGuildUser user = (SocketGuildUser)Context.User;
            
            SocketVoiceChannel chan = CommandHandler.GetVChannel(Context.Guild.VoiceChannels, user.VoiceChannel.Name);
            if ( chan != null)
            {

                var embed2 = new EmbedBuilder();
                embed2.WithTitle("Moved User");
                embed2.WithDescription(user.Mention + " Moved to " + chan.Name);
                embed2.WithColor(100, 150, 255);
                embed2.WithFooter("Created by Abyscuit");
                await Context.Channel.SendMessageAsync("", false, embed2);
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Failed to Join");
                embed.WithDescription(user.Mention + " is not in a channel!");
                embed.WithColor(100, 150, 255);
                embed.WithFooter("Created by Abyscuit");
                await Context.Channel.SendMessageAsync("", false, embed);
            }

        }
        */

        List<string> videoIDs = new List<string>();
        bool selectvid = Global.selectYoutube;
        
        [Command("youtube")]
        [RequireUserPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Youtube([Remainder]string keyword)
        {
            YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDnxzvq9A-XhVgO0x9I5IRNk67oboUcZD8",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtube.Search.List("snippet");
            searchListRequest.Q = keyword; // Replace with your search term.
            searchListRequest.MaxResults = 10;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();

            ///result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            int i = 0;

            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format(i + ") {0} by {1}", searchResult.Snippet.Title, searchResult.Snippet.ChannelTitle));
                        Global.vidIDs.Add(searchResult.Id.VideoId);
                        i++;
                        break;
                }
            }

            Console.WriteLine(String.Format("__Videos:__\n{0}\n", string.Join("\n", videos)));
            string msg = string.Join("\n", videos);
            Global.selectYoutube = true;
            await PrintEmbedMessage("YouTube Results", msg);
        }

        [Command("select")]
        [RequireUserPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Select(int num)
        {
            if (selectvid)
            {
                if (num >= Global.vidIDs.Count)
                {
                    await PrintEmbedMessage("YouTube Video", "Index out of bounds");
                    return;
                }
                string url = "https://www.youtube.com/watch?v=" + Global.vidIDs[num];

                Console.WriteLine(url);
                await Context.Channel.SendMessageAsync(url, false);
                Global.vidIDs.Clear();
                Global.selectYoutube = false;
            }
            else if (Global.selectPS)
            {
                UserAccount user = UserAccounts.GetAccount(Context.User);
                if (num >= shop.Length)
                {
                    await PrintEmbedMessage("Point Shop", "Index out of bounds");
                    return;
                }
                if (user.Points >= price[num]) {
                    user.Points -= (uint)price[num];
                    UserAccounts.SaveAccounts();
                    string msg = Context.User.Username + " bought " + shop[num] + " for " + price[num];
                    Console.WriteLine(msg);
                    msg = "You just bought " + shop[num] + " for " + price[num] + 
                        "\n__Points Left:__ " + user.Points;

                    SocketGuildUser usr = (SocketGuildUser)Context.User;
                    IRole[] r = usr.Guild.Roles.ToArray();
                    IRole addrole = null;
                    for (int i = 0; i < r.Length; i++)
                    {
                        if (r[i].Name.ToLower() == shop[num].ToLower())
                        {
                            addrole = r[i];
                            break;
                        }
                    }
                    if (addrole != null)
                        await usr.AddRoleAsync(addrole);
                    
                    await PrintEmbedMessage("Point Shop", msg);

                }
                else
                {
                    await Context.Channel.SendMessageAsync("Sorry, you can't afford that!", false);
                }
                Global.selectPS = false;
            }
        }

        [Command("move")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        [RequireBotPermission(GuildPermission.MoveMembers)]
        public async Task Move(SocketGuildUser user, [Remainder]string voiceChannel)
        {
            if (user.VoiceChannel != null)
            {
                SocketVoiceChannel chan = CommandHandler.GetVChannel(user.Guild.VoiceChannels, voiceChannel);
                await user.ModifyAsync(m => { m.Channel = chan; });
                
                await PrintEmbedMessage("Moved User", user.Mention + " Move to " + chan.Name);
            }
            else
            {
                await PrintEmbedMessage("Failed to Move User", user.Mention + " is not in a voice channel");
            }
        }

        [Command("echo")]
        public async Task Echo([Remainder]string msg)
        {
            msg = "**" + msg + "**";
            await PrintEmbedMessage("Echoed Message", msg);
            await Context.Message.DeleteAsync();
        }

        [Command("help")]
        public async Task Help()
        {
            ServerConfig serverConfig = ServerConfigs.GetConfig(Context.Guild); 
            string msg = "Use the '" + serverConfig.Prefix + "' prefix to send a command.\n";
            for (int i = 0; i < cmds.Length; i++)
            {
                if (Context.Guild.Name != "Da Byscuits" && cmds[i] == "invlink") continue;
                if (Context.Guild.Name != "Da Byscuits" && cmds[i] == "pointshop") continue;

                if (cmds[i].Contains("Server Stuff"))
                    msg += "__Server Stuff__\n";
                else if (cmds[i].Contains("Media Stuff"))
                    msg += "\n__Media Stuff__\n";
                else if (cmds[i].Contains("Steam Commands"))
                    msg += "\n__Steam Commands__\n";
                else
                    msg += "**" + cmds[i] + "**: " + desc[i] + "\n";
            }

            

            await PrintEmbedMessage("Help/Commands", msg);
            msg = "\n\n__Server Configuration__\n";
            for (int i = 0; i < configCMDs.Length; i++)
            {
                msg += "**" + configCMDs[i] + "**: " + configDesc[i] + "\n";
            }
            msg += "\n__Cryptocurrency Commands__\n";
            for (int i = 0; i < miningCmds.Length; i++)
            {
                msg += "**" + miningCmds[i] + "**: " + miningDesc[i] + "\n";
            }
            msg += "\n__Twitch Commands__\n";
            for (int i = 0; i < twitchCMDs.Length; i++)
            {
                msg += "**" + twitchCMDs[i] + "**: " + twitchDesc[i] + "\n";
            }
            await PrintEmbedMessage("Help/Commands", msg);
        }
        

        [Command("roles")]
        public async Task Roles(SocketGuildUser user = null)
        {
            string roles = "";
            if (user != null)
            {
                roles = getRoles(user);
            }
            else
            {
                user = (SocketGuildUser)Context.User;
                roles = getRoles(user);
            }
            
            string msg = "**" + roles + "**";
            
            await PrintEmbedMessage("Roles for " + user.Username, msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            await user.KickAsync(reason);
            string msg = "__" + user.Username + "__\nReason: **" + reason + "**";
            
            await PrintEmbedMessage("Kicked", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, int days, [Remainder]string reason = "No reason provided.")
        {
            await user.Guild.AddBanAsync(user, days, reason);
            
            string msg = "__" + user.Username + "__\nReason: **" + reason + "**";
            Global.PrintMsg(Context.User.Username, msg);
            
            await PrintEmbedMessage("Banned", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("unban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Unban(IGuildUser user)
        {
            await user.Guild.RemoveBanAsync(user);
            
            string msg = "**Congrats " + user.Username + "**\n__You're unbanned!__";
            
            await PrintEmbedMessage("Unbanned " + user.Username, msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("invlink")]
        [RequireUserPermission(GuildPermission.CreateInstantInvite)]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public async Task InviteLink()
        {
            Global.PrintMsg(Context.User.Username, "Posting Invite Link");
            await Context.Channel.SendMessageAsync(Context.User.Mention + " "+ invLink, false);
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Mute(SocketGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            UserAccount account = UserAccounts.GetAccount(user);
            await user.ModifyAsync(m => { m.Mute = true; });
            account.IsMuted = true;
            UserAccounts.SaveAccounts();

            string msg = "__" + user.Username + "__\nReason: **" + reason + "**";
            
            await PrintEmbedMessage("Muted", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("unmute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Unmute(SocketGuildUser user)
        {
            UserAccount account = UserAccounts.GetAccount(user);
            await user.ModifyAsync(m => { m.Mute = false; });
            account.IsMuted = false;
            UserAccounts.SaveAccounts();
            
            string msg = "**" + user.Username + "** is now unmuted!";
            
            await PrintEmbedMessage("Unmuted", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("addrole")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRole(IGuildUser user, [Remainder]string role)
        {
            IRole[] r = user.Guild.Roles.ToArray();
            IRole addrole = null;
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i].Name.ToLower() == role.ToLower())
                {
                    addrole = r[i];
                    break;
                }
            }
            if (addrole != null)
                await user.AddRoleAsync(addrole);
           
            string msg = "__" + user.Username + "__\nAdded role **" + addrole + "**";
            
            await PrintEmbedMessage("Role Added", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("addroles")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRoles(IGuildUser user, [Remainder]string roles)
        {
            roles = roles.Replace(", ", "/");
            string[] split = roles.Split('/');
            IRole[] r = user.Guild.Roles.ToArray();
            List<IRole> addrole = new List<IRole>();
            string roleNames = "";
            for (int s = 0; s < split.Length; s++)
            {
                for (int i = 0; i < r.Length; i++)
                {
                    if (r[i].Name.ToLower() == split[s].ToLower())
                    {
                        addrole.Add(r[i]);
                        roleNames += r[i].Name + "\n";
                    }
                }
            }
            if (addrole != null)
                await user.AddRolesAsync(addrole);
            
            string msg = "__" + user.Username + "__\nAdded roles\n**" + roleNames + "**";
            
            await PrintEmbedMessage("Roles Added", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("removerole")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task RemoveRole(IGuildUser user, [Remainder]string role)
        {
            IRole[] r = user.Guild.Roles.ToArray();
            IRole addrole = null;
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i].Name.ToLower() == role.ToLower())
                {
                    addrole = r[i];
                    break;
                }
            }
            if (addrole != null)
                await user.RemoveRoleAsync(addrole);
            
            string msg = "__" + user.Username + "__\nRemoved role **" + addrole + "**";
            
            await PrintEmbedMessage("Role Removed", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("removeallroles")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task RemoveRoles(IGuildUser user)
        {
            IRole[] r = user.Guild.Roles.ToArray();
            string oldRoles = "";
            for (int i = 0; i < r.Length; i++)
            {
                oldRoles += r[i].Name + "\n";
            }
            await user.RemoveRolesAsync(r);
            
            string msg = "__" + user.Username + "__\nRemoved roles \n**" + oldRoles + "**";
            
            await PrintEmbedMessage("Role Removed", msg, iconUrl: user.GetAvatarUrl());
        }

        
        [Command("nickname")]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        public async Task Nickname(SocketGuildUser user, [Remainder]string nickname)
        {

            await user.ModifyAsync( m => { m.Nickname = nickname; });

            string msg = "**" + user.Mention + " is now " + nickname + "**";
            
            await PrintEmbedMessage("Nickname Set", msg, iconUrl:user.GetAvatarUrl());

        }
        
        [Command("data")]
        public async Task GetData()
        {
            int count = DataStorage.GetPairCount() + SteamData.GetCount() + SteamAccounts.GetAllAccounts().Length + NanoPool.GetCount();
            string msg = "Total Data storage contains " + count + " pairs";
            
            await PrintEmbedMessage("Data Pairs", msg);
            DataStorage.AddPairToStorage("pairCount", DataStorage.GetPairCount().ToString());
        }
        
        

        [Command("warn")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Warn(SocketGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            UserAccount account = UserAccounts.GetAccount(user);
            account.NumberOfWarnings++;
            UserAccounts.SaveAccounts();
            uint maxwarnings = 5;
            float warnpercent = (float)account.NumberOfWarnings / (float)maxwarnings * 100f;
            if (account.NumberOfWarnings >= maxwarnings)
            {
                await user.Guild.AddBanAsync(user, 1, reason);
                
                string msg = "**" + user.Mention + " Banned for 1 day.**\nReason: __" + reason + "__";
                
                await PrintEmbedMessage("Banned ", msg, iconUrl: user.GetAvatarUrl());
            }
            else
            {
                string msg = "**" + user.Mention + "**\n**Reason:** __" + reason + "__\n" +
                    "**Warn Percent:** " + warnpercent +"";
                
                await PrintEmbedMessage("Warning", msg, iconUrl: user.GetAvatarUrl());
            }
        }

        public bool IsAuthorized(SocketGuildUser user)
        {
            string roleName = "Fresh Byscuit";
            var result = from r in user.Guild.Roles
                         where r.Name == roleName
                         select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;

            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        public string getRoles(SocketGuildUser user)
        {
            string roles = "";
            foreach (SocketRole role in user.Roles)
            {
                if (!role.IsEveryone)
                    roles += role.Name + "\n";
            }

            return roles;
        }

        public string getAllRoles(SocketGuildUser user)
        {
            string roles = "";
            foreach (SocketRole role in user.Guild.Roles)
            {
                if(!role.IsEveryone)
                roles += role.Name + "\n";
            }

            return roles;
        }


        [Command("pointshop")]
        public async Task PointShop()
        {
                   
            string msg = "";
            for(int i = 0; i < shop.Length;i++)
            {
                msg += i + ") __" + shop[i] + "__ | **" + price[i] + " points**\n";
            }

            await PrintEmbedMessage("Point Shop", msg);
            Global.selectPS = true;
        }
        #endregion

        #region Server Stuff
        [Command("invite")]
        public async Task Invite()
        {
            IReadOnlyCollection<RestInviteMetadata> x = await Context.Guild.GetInvitesAsync();
            RestInviteMetadata inv = null;
            foreach (RestInviteMetadata invite in x)
            {
                if (!invite.IsTemporary && !invite.IsRevoked)
                {
                    if (invite.MaxUses == 0 && invite.MaxAge == null)
                    {
                        inv = invite;
                        break;
                    }
                }
            }
            if (inv == null)
            {
                foreach (RestInviteMetadata invite in x)
                {
                    inv = invite;
                    break;
                }
            }
            string msg = inv.Url;
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("clear")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int num)
        {
            await Context.Message.DeleteAsync();
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> x =  Context.Channel.GetMessagesAsync((num));
            IAsyncEnumerator<IReadOnlyCollection<IMessage>> index = x.GetEnumerator();
            while(await index.MoveNext())
            {
                foreach(IMessage msg in index.Current)
                {
                    await msg.DeleteAsync();
                }
            }
            RepeatingTimer.channel = (SocketTextChannel)Context.Channel;
            RepeatingTimer.clrMsg = true;
            RepeatingTimer.clrMsgTime = DateTime.Now.AddSeconds(5);
            RestUserMessage m = await Context.Channel.SendMessageAsync("Cleared "+ num +" messages!");
            Global.MessageIdToTrack = m.Id;
        }

        #endregion

        #region media


        [Command("meme")]
        [RequireUserPermission(GuildPermission.AttachFiles)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public async Task Meme([Remainder]string text = null)
        {
            string memeToPost = "";
            DirectoryInfo d = new DirectoryInfo(@"Memes\");//Assuming Test is your Folder
            Console.WriteLine("Scanning Directory: " + d.FullName);
            FileInfo[] Files = d.GetFiles(); //Getting Text files
            Console.WriteLine("Found Files: " + Files.Length);
            int fileMax = Files.Length;
            string[] filePath = new string[fileMax];
            for (int i = 0; i < fileMax; i++)
            {
                filePath[i] = Files[i].Name;
            }
            memeToPost = "Memes/" + filePath[rand.Next(0, fileMax)];
            while (Global.MemesUsed.Contains(memeToPost))
            {
                memeToPost = "Memes/" + filePath[rand.Next(0, fileMax)];
            }
            bool useMeme = true;
            if(text != null && text != "")
            {
                string[] tags = text.Split(' ');
                List<Meme> memes = MemeLoader.SearchMeme(tags);
                if (memes.Count == 1)
                    memeToPost = "Memes/" + memes[0].path;
                else if(memes.Count > 1)
                {
                    Meme meme = memes[rand.Next(0,memes.Count - 1)];
                    memeToPost = "Memes/" + meme.path;
                }
                useMeme = false;
            }
            Console.WriteLine("Posting " + memeToPost);
            await Context.Channel.SendFileAsync(memeToPost);
            if(useMeme)
                Global.MemesUsed.Add(memeToPost);
            if (Global.MemesUsed.Count >= 20)
                Global.MemesUsed.RemoveAt(0);
        }

        [Command("creatememe")]
        [RequireUserPermission(GuildPermission.AttachFiles)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public async Task CreateMeme(SocketUser user = null, [Remainder] string text = null)
        {
            string[] split = text.Split(',');
            string top = split[0];
            string bot = "";
            if (split.Length > 1)
                bot = split[1];

            IReadOnlyCollection<Attachment> msgAttach = Context.Message.Attachments;
            string url = "";
            if (msgAttach.Count == 0)
            {
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync("You must upload an image or Mention someone with the command!\nUsage: creatememe @mention top sentence,bottom sentence");
                    return;
                }
                else
                    url = user.GetAvatarUrl(size: 1024);
            }
            else
                url = msgAttach.First().Url;
            Console.WriteLine(url);
            string meme = MemeGenerator.CreateMeme(url, top, bot);
            await Context.Channel.SendFileAsync(meme);
            await Context.Message.DeleteAsync();
            File.Delete(meme);
        }

        [Command("creatememe")]
        [RequireUserPermission(GuildPermission.AttachFiles)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public async Task CreateMeme([Remainder] string text = null)
        {
            string[] split = text.Split(',');
            string top = split[0];
            string bot = "";
            if (split.Length > 1)
                bot = split[1];

            IReadOnlyCollection<Attachment> msgAttach = Context.Message.Attachments;
            string url = "";
            if (msgAttach.Count == 0)
            {
                await Context.Channel.SendMessageAsync("You must upload an image or Mention someone with the command!\nUsage: creatememe @mention top sentence,bottom sentence");
                return;
            }
            else
                url = msgAttach.First().Url;
            Console.WriteLine(url);
            string meme = MemeGenerator.CreateMeme(url, top, bot);
            await Context.Channel.SendFileAsync(meme);
            await Context.Message.DeleteAsync();
            File.Delete(meme);
        }
        
        [Command("upload")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Upload(string name = "", [Remainder]string tags = "")
        {
            IReadOnlyCollection<Attachment> attachments = Context.Message.Attachments;
            string msg = "__Uploaded__";
            int count = 0;
            foreach (Attachment attachment in attachments)
            {
                if (name == "" || count > 0)
                    name = attachment.Filename.Remove(attachment.Filename.Length - 4, 4);
                string[] tag = null;
                if (tags != "" && attachments.Count < 2)
                    tag = tags.ToLower().Split(',');
                Meme meme = new Meme()
                {
                    path = attachment.Filename,
                    name = name,
                    tags = tag
                };
                MemeLoader.UpdateMeme(attachment.Filename, meme);
                string file = "Memes/" + attachment.Filename;
                MemeGenerator.DownloadImage(file, attachment.Url);
                Console.WriteLine("Uploading " + file);
                msg += "\nImage saved to " + file;
                count++;
            }
            await Context.Channel.SendMessageAsync(msg);
            await Context.Message.DeleteAsync();
        }
        #endregion

        #region encode/decode

        [Command("urlencode")]
        public async Task UrlEncode([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + EncodeURL(text) + "```");
            await Context.Message.DeleteAsync();
        }

        [Command("sha1")]
        public async Task Sha1([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Sha1Hash(text) + "```");
            await Context.Message.DeleteAsync();
        }

        [Command("sha256")]
        public async Task Sha256([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + sha256(text) + "```");
            await Context.Message.DeleteAsync();
        }
        [Command("sha512")]
        public async Task Sha512([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Sha512Hash(text) + "```");
            await Context.Message.DeleteAsync();
        }
        [Command("md5")]
        public async Task MD5([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + MD5Hash(text) + "```");
            await Context.Message.DeleteAsync();
        }

        [Command("base64encode")]
        public async Task EncodeBase64([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Base64Encode(text) + "```");
            await Context.Message.DeleteAsync();
        }
        [Command("base64decode")]
        public async Task DecodeBase64([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Base64Decode(text) + "```");
            await Context.Message.DeleteAsync();
        }
        public static string EncodeURL(string url)
        {
            url = WebUtility.UrlEncode(url);
            return url;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Sha1Hash(string input)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("X2")).ToArray());
        }

        public static string Sha512Hash(string input)
        {
            var hash = (new SHA512Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("X2")).ToArray());
        }

        public static string MD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("X2"));
            }
            return hash.ToString();
        }

        #endregion

        #region SteamAPI
        //------SteamAPI----------

        /*------Not Worth-----
        [Command("esea")]
        public async Task Esea([Remainder]string text)
        {
            //Global.PrintMsg(Context.User.Username, "ESEA Stats");
            ESEA.GetInfo(text);
            await PrintEmbedMessage("ESEA Stats", "Placeholder\nCheck Console");
            
        }
        */

        [Command("resolve")]
        public async Task Resolve([Remainder]string text)
        {
            string username = "";
            if (!text.Contains(".com/"))
                username = text;
            else
            {
                string[] divide = text.Split('/');
                username = divide[divide.Length - 1];
            }
            string steamID = SteamAccounts.ResolveAccount(username);
            string msg = "**Resolved Username:** " + username + "\n**SteamID:** " + steamID;
            string imgUrl = "";
            
            if (steamID != "No match")
            {
                msg += "\n" + SteamAccounts.GetAccountSummary(ulong.Parse(steamID));
                imgUrl = SteamAccounts.GetAccountObject(ulong.Parse(steamID)).avatarfull;
            }
            Global.PrintMsg(Context.User.Username, "Steam Account Resolved");
            
            await PrintEmbedMessage("Steam Account Resolved", msg, iconUrl: imgUrl);

            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("steaminfo")]
        public async Task SteamInfo(ulong id = 0, [Remainder]string text = null)
        {
            string username = "";
            if (text != null)
            {
                if (!text.Contains(".com/"))
                    username = text;
                else
                {
                    string[] divide = text.Split('/');
                    username = divide[divide.Length - 1];
                }
                id = ulong.Parse(SteamAccounts.ResolveAccount(username));
            }
            else
            {
                if (id == 0)
                {
                    SteamAccount user = SteamAccounts.GetAccount(Context.User);
                    if(user == null)
                    {
                        await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                        return;
                    }
                    id = user.SteamID;
                }
            }
            string msg = SteamAccounts.GetAccountSummary(id);
            
            Global.PrintMsg(Context.User.Username,  "Steam Account Info");
            
            await PrintEmbedMessage("Steam Account Info", msg, iconUrl: SteamAccounts.GetAccountObject(id).avatarfull);

            //await Context.Channel.SendMessageAsync(msg);
        }

        /*
        public ulong resolveInfo(ulong id, string text)
        {
            if (text != null)
            {
                if (!text.Contains(".com/"))
                    username = text;
                else
                {
                    string[] divide = text.Split('/');
                    username = divide[divide.Length - 1];
                }
                id = ulong.Parse(SteamAccounts.ResolveAccount(username));
            }
            else
            {
                if (id == 0)
                {
                    SteamAccount user = SteamAccounts.GetAccount(Context.User);
                    id = user.SteamID;
                }
            }
            return id;
        }
        */
        [Command("steambans")]
        public async Task SteamBans(ulong id = 0, [Remainder]string text = null)
        {
            string username = "";
            if (text != null)
            {
                if (!text.Contains(".com/"))
                    username = text;
                else
                {
                    string[] divide = text.Split('/');
                    username = divide[divide.Length - 1];
                }
                id = ulong.Parse(SteamAccounts.ResolveAccount(username));
            }
            else
            {
                if (id == 0)
                {
                    SteamAccount user = SteamAccounts.GetAccount(Context.User);
                    if (user == null)
                    {
                        await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                        return;
                    }
                    id = user.SteamID;
                }
            }
            SteamAccounts.BanPlayer player = SteamAccounts.GetAccountBans(id);
            string msg = "";
            msg += "**Steam ID:** " + player.SteamId;
            msg += "\n**VAC Banned:** " + player.VACBanned;
            msg += "\n**Number of VAC Bans:** " + player.NumberOfVACBans;
            msg += "\n**Number of Game Bans:** " + player.NumberOfGameBans;
            msg += "\n**Community Banned:** " + player.CommunityBanned;
            msg += "\n**Economy  Banned:** " + player.EconomyBan;
            msg += "\n**Days Since Last Ban:** " + player.DaysSinceLastBan;
            Global.PrintMsg(Context.User.Username, "Steam Account Bans");

            await PrintEmbedMessage("Steam Account Bans", msg, iconUrl: SteamAccounts.GetAccountObject(id).avatarfull);
        }

        [Command("steamgames")]
        public async Task SteamGames([Remainder]string text = null)
        {
            int appID = 0;
            
            if (text != null)
            {
                text = text.ToLower();
                if(text == "csgo" || text == "cs:go" || text == "global offensive")
                    appID = 730;
            }
            SteamAccount user = SteamAccounts.GetAccount(Context.User);
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                return;
            }
            ulong id = user.SteamID;

            SteamAccounts.GameResponse player = SteamAccounts.GetAccountGames(id);
            string msg = "";
            string title = "Steam Games";
            string imageUrl = SteamAccounts.GetAccountObject(id).avatarfull;
            msg += "**Game Count:** " + player.game_count + "\n";
            for (int i = 0; i < player.games.Count; i++)
            {
                string name = player.games[i].name;
                if (appID == 0 && text == null)
                {
                    msg += string.Format("**__{0}__\nappID:** {1}\n", name, player.games[i].appid);
                    if (player.games[i].playtime_2weeks != null)
                        msg += string.Format("**Playtime Last 2 Weeks:** {0:N2}hrs\n", player.games[i].playtime_2weeks / 60f);
                    msg += string.Format("**Playtime Total:** {0:N2}hrs\n\n", player.games[i].playtime_forever / 60f);
                    //msg += string.Format("**{0} Image Icon Url Hash:** {1}\n", name, player.games[i].img_icon_url);
                    //msg += string.Format("**{0} Image Logo Url Hash:** {1}\n", name, player.games[i].img_logo_url);
                    if (i % 15 == 0 && i > 0)
                    {
                        msg += "|";
                    }
                }
                else
                {
                    if (player.games[i].appid == appID || name.ToLower() == text.ToLower())
                    {
                        title = name;
                        appID = (int)player.games[i].appid;
                        msg = string.Format("**appID:** {0}\n", appID);
                        if (player.games[i].playtime_2weeks != null)
                            msg += string.Format("**Playtime Last 2 Weeks:** {0:N2}hrs\n", player.games[i].playtime_2weeks / 60f);
                        msg += string.Format("**Playtime Total:** __{0:N2}hrs__\n", player.games[i].playtime_forever / 60f);
                        //msg += string.Format("**Image Icon Url Hash:** {0}\n", player.games[i].img_icon_url);
                        //msg += string.Format("**Image Logo Url Hash:** {0}\n", player.games[i].img_logo_url);
                        imageUrl = SteamAccounts.gameImgUrl(appID, player.games[i].img_logo_url);
                        break;
                    }
                }
            }
            string[] msgs = msg.Split('|');
            for (int i = 0; i < msgs.Length; i++)
            {
                Global.PrintMsg(Context.User.Username, title);
                
                await PrintEmbedMessage(title, msgs[i], iconUrl: imageUrl);
            }
        }

        [Command("linksteam")]
        public async Task LinkSteam(ulong id = 0, [Remainder]string text = null)
        {
            string username = "";
            if (text != null)
            {
                if (!text.Contains(".com/"))
                    username = text;
                else
                {
                    string[] divide = text.Split('/');
                    username = divide[divide.Length - 1];
                }
                id = ulong.Parse(SteamAccounts.ResolveAccount(username));
            }
            else
            {
                if(id == 0)
                {
                    Global.PrintMsg(Context.User.Username, "Must have SteamID or Username/URL as parameter");
                    await Context.Channel.SendMessageAsync("Must have SteamID or Username/URL as parameter!\n" +
                        "**linksteam <steamID> <username/URL>**");
                    return;
                }
            }
            
            //string steamID = SteamAccounts.ResolveAccount(username);
            
            if(id == 0)
            {
                Global.PrintMsg(Context.User.Username, "Did not link account! No match!");
                await Context.Channel.SendMessageAsync("Did not link account! No match!");
                return;
            }
            SteamAccount testUser = SteamAccounts.GetAccount(Context.User);
            if(testUser != null)
            {
                Global.PrintMsg(Context.User.Username, "Account is already linked to " + testUser.SteamUsername + "\nSteamID: " + testUser.SteamID);
                await Context.Channel.SendMessageAsync("Account is already linked to **" + testUser.SteamUsername + "**\nSteamID: "+ testUser.SteamID);
                return;
            }
            SteamAccount user = SteamAccounts.LinkAccount(Context.User, id);
            string msg = "Username: " + user.SteamUsername + "\nSteamID: " + id;
            if (user.DiscordID != Context.User.Id)
                msg = "Account linked to another user!";


            SteamAccounts.SaveAccounts();
            Global.PrintMsg(Context.User.Username, msg);
            
            await PrintEmbedMessage("Steam Account Linked", msg, iconUrl: SteamAccounts.GetAccountObject(id).avatarfull);
        }

        [Command("steamaccts")]
        public async Task GetSteamAccounts()
        {
            SteamAccount[] steamAccts = SteamAccounts.GetAllAccounts();
            string msg = "";
            for(int i =0;i<steamAccts.Length;i++)
            {
                msg += "**"+steamAccts[i].DiscordUsername+"** linked to **"+steamAccts[i].SteamUsername+"**\n" +
                    "SteamID: "+steamAccts[i].SteamID;
                if (i <= steamAccts.Length - 1)
                    msg += "\n\n";
            }

            Global.PrintMsg(Context.User.Username, msg);
            
            await PrintEmbedMessage("Steam Accounts Linked", msg);

            //await Context.Channel.SendMessageAsync(msg);
        }


        [Command("csgostats")]
        public async Task CsgoStats(ulong id = 0, [Remainder]string text = null)
        {
            string username = "";
            if (text != null)
            {
                if (!text.Contains(".com/"))
                    username = text;
                else
                {
                    string[] divide = text.Split('/');
                    username = divide[divide.Length - 1];
                }
                id = ulong.Parse(SteamAccounts.ResolveAccount(username));
            }
            else
            {
                if (id == 0)
                {
                    SteamAccount user = SteamAccounts.GetAccount(Context.User);
                    if (user == null)
                    {
                        await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                        return;
                    }
                    id = user.SteamID;
                }
            }
            string msg = SteamAccounts.GetCSGOStats(id);
            string avatarURL = SteamAccounts.GetAccountObject(id).avatarfull;

            await PrintEmbedMessage("CSGO Stats", msg, iconUrl:avatarURL);


            var lastMatch = SteamAccounts.GetCSGOLastMatch(Context.User, id);
            await PostLastMatch(lastMatch, avatarURL);
            Global.PrintMsg(Context.User.Username, "csgostats");
        }

        public async Task PostLastMatch(SteamAccounts.CsgoLastMatch lastMatch, string avatarURL, int? matchNum = null)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            var embed = new EmbedBuilder();
            embed.WithTitle("CS:GO Last Match");
            if (matchNum != null)
                embed.WithDescription("__Match " + matchNum+"__");
            embed.WithThumbnailUrl(avatarURL);
            embed.AddField("Stats", string.Format("**{0}**\n**{1}**\n**{2}**\n**{3}**\n**{4}**\n**{5}**\n**{6}**\n**{7}**\n**{8}**\n**{9}**\n**{10}**\n**{11}**\n**{12}**\n**{13}**\n**{14}**\n**{15}**\n" +
                "**{16}**\n**{17}**\n**{18}**\n**{19}**", "Rounds", "Contribution Score", "Round Wins",
                "Round Losses", "Game Outcome", "Fav Weapon", "Fav Weapon Shots", "Fav Weapon Hits", "Fav Weapon Accuracy", "Fav Weapon Kills", "Terrorist Wins", "Counter-Terrorist Wins",
                "Kills", "Deaths", "K/D Ratio", "MVPs", "Damage", "Money Spent", "Dominations", "Revenges"),true);
            //0)Rounds 1)Score 2)Wins 3)Losses 4)outcome 5)favwepname 6)favwep shots 7)favwep hits 8)favwep accuracy 9)favwep kills 10)t_wins 11)ct_wins 12)kills 13)deaths 14)kd
            //15)mvps 16)damage 17)moneyspent 18)dominations 19)revenges
            embed.AddField("Values", string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8:N2}%\n{9}\n{10}\n{11}\n{12}\n{13}\n{14:N2}\n{15:N0}\n{16:N0}\n${17:N0}\n{18}\n{19}",
                lastMatch.rounds, lastMatch.contribution_score, lastMatch.wins,
                lastMatch.losses, lastMatch.outcome, lastMatch.favweapon_name, lastMatch.favweapon_shots, lastMatch.favweapon_hits, lastMatch.accuracy, lastMatch.favweapon_kills,
                lastMatch.t_wins, lastMatch.ct_wins, lastMatch.kills, lastMatch.deaths, lastMatch.kd, lastMatch.mvps, lastMatch.damage, lastMatch.money_spent, lastMatch.dominations, lastMatch.revenges), true);
            //embed.AddInlineField("Stats", "**Rounds:**\n**Contribution Score:**\n**Round Wins:**");
            //embed.AddInlineField("Values", string.Format("{0}\n{1}\n{2}", lastMatch.rounds, lastMatch.contribution_score, lastMatch.wins));
            //embed.AddInlineField("Rounds", lastMatch.rounds);
            //embed.AddInlineField("Contribution Score", lastMatch.contribution_score);
            //embed.AddInlineField("Round Wins", lastMatch.wins);
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("csgolastmatch")]
        public async Task CsgoLastMatch(ulong id = 0, [Remainder]string text = null)
        {
            string username = "";
            if (text != null)
            {
                if (!text.Contains(".com/"))
                    username = text;
                else
                {
                    string[] divide = text.Split('/');
                    username = divide[divide.Length - 1];
                }
                id = ulong.Parse(SteamAccounts.ResolveAccount(username));
            }
            else
            {
                if (id == 0)
                {
                    SteamAccount user = SteamAccounts.GetAccount(Context.User);
                    if (user == null)
                    {
                        await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                        return;
                    }
                    id = user.SteamID;
                }
            }
            string avatarURL = SteamAccounts.GetAccountObject(id).avatarfull;

            var lastMatch = SteamAccounts.GetCSGOLastMatch(Context.User, id);
            await PostLastMatch(lastMatch, avatarURL);
            Global.PrintMsg(Context.User.Username, "csgolastmatch");
        }

        [Command("csgolastmatches")]
        public async Task CsgoLastMatches(uint num = 0)
        {
            if (num == 0)
                num = 10;
            SteamAccount user = SteamAccounts.GetAccount(Context.User);
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                return;
            }
            ulong id = user.SteamID;
            string avatarURL = SteamAccounts.GetAccountObject(id).avatarfull;

            string msg = "";
            SteamAccounts.CsgoLastMatches lastMatches = null;
            for (int i = 0; i < SteamData.matches.Count; i++)
            {
                if(SteamData.matches[i].steamid == id)
                {
                    lastMatches = SteamData.matches[i];
                    break;
                }
            }
            if (lastMatches == null)
            {
                msg = "Link your account with linksteam cmd before using this cmd!";
                await Context.Channel.SendMessageAsync(msg);
                return;
            }
            
            for(int i = lastMatches.lastmatches.Count - 1;i>=0;i--)
            {
                SteamAccounts.CsgoLastMatch lastMatch = lastMatches.lastmatches[i];
                /*
                //msg += "__Match " + (i + 1) + "__\n";
                //msg += " **Rounds**" + Indent(1, guild) + "**Contribution Score**\n" + Indent(1, guild) + lastMatch.rounds + Indent(3, guild) + lastMatch.contribution_score;
                //Console.WriteLine(msg);
                //msg += "\n**Contribution Score:** " + lastMatch.contribution_score;
                //msg += "\n** Game Outcome**" + Indent(1, guild) + "**Terrorist Wins**\n " + Indent(1, guild) + " " + lastMatch.outcome + Indent(4, guild) + lastMatch.t_wins;
                //msg += "\n**Terrorist Wins:** " + lastMatch.t_wins;
                msg += "\n**Counter-Terrorist Wins:** " + lastMatch.ct_wins;
                msg += "\n**Kills:** " + lastMatch.kills;
                msg += "\n**Deaths:** " + lastMatch.deaths;
                msg += string.Format("\n**K/D Ratio:** {0:N2}", lastMatch.kd);
                msg += "\n**MVPs:** " + lastMatch.mvps;
                msg += string.Format("\n**Damage:** {0:N}", lastMatch.damage);
                msg += string.Format("**\nMoney Spent:** ${0:N2}", lastMatch.money_spent);
                msg += "\n**Dominations:** " + lastMatch.dominations;
                msg += "\n**Revenges:** " + lastMatch.revenges;
                msg += "\n**Favorite Weapon:** " + lastMatch.favweapon_name;
                msg += "\n**Favorite Weapon Shots:** " + lastMatch.favweapon_shots;
                msg += "\n**Favorite Weapon Hits:** " + lastMatch.favweapon_hits;
                msg += string.Format("\n**Favorite Weapon Accuracy:** {0:N2}%", lastMatch.accuracy);
                msg += "\n**Favorite Weapon Kills:** " + lastMatch.favweapon_kills;
                */

                await PostLastMatch(lastMatch, avatarURL, i + 1);
                if (i <= lastMatches.lastmatches.Count - num)
                    break;

            }

        }


        [Command("csgowepstats")]
        public async Task CsgoWeaponStats([Remainder]string text = null)
        {
            string weaponName = text;
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            SteamAccount user = SteamAccounts.GetAccount(Context.User);
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command or enter a steamID64!");
                return;
            }
            ulong id = user.SteamID;
            string avatarURL = SteamAccounts.GetAccountObject(id).avatarfull;

            if (weaponName == null)
            {
                List<SteamAccounts.Stats> stats = SteamAccounts.GetCSGOWeaponStats(id);

                var embed = new EmbedBuilder();
                int wepNum = 0;
                int count = 0;
                string oldWep = stats[0].name.Split(' ')[0];
                string statTxt = "";
                string valueTxt = "";
                for (int i = 0; i < stats.Count; i++)
                {
                    string name = stats[i].name;
                    string value = stats[i].value.ToString();
                    if (name.Contains("Accuracy"))
                        value = string.Format("{0:N2}%", stats[i].value);

                    string curWep = stats[i].name.Split(' ')[0];
                    if (oldWep != curWep)
                    {
                        wepNum++;
                        if (wepNum >= 2)
                        {
                            count++;
                            statTxt += "\n";
                            valueTxt += "\n";
                            wepNum = 0;
                        }
                        else
                        {
                            statTxt += "\n";
                            valueTxt += "\n";
                        }
                        oldWep = curWep;

                    }
                    if(count >= 5 || i >= stats.Count - 1)
                    {
                        embed.AddField("Stat", statTxt, true);
                        embed.AddField("Value", valueTxt, true);
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                        embed = new EmbedBuilder();
                        statTxt = "\n";
                        valueTxt = "\n";
                        count = 0;
                    }
                    embed.WithTitle("CS:GO Weapon Stats");
                    embed.WithThumbnailUrl(avatarURL);
                    statTxt += "\n**" + name + "**";
                    valueTxt += "\n" + value;

                    embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                    if (config.FooterText != "")
                        embed.WithFooter(config.FooterText);
                    if (config.TimeStamp)
                        embed.WithCurrentTimestamp();
                    
                }

                Global.PrintMsg(Context.User.Username, "csgowepstats");

            }
            else
            {
                List<SteamAccounts.Stats> stats = SteamAccounts.GetCSGOWeaponStats(id, weaponName);


                var embed = new EmbedBuilder();
                embed.WithTitle("CS:GO Weapon Stats for " + weaponName);
                embed.WithThumbnailUrl(avatarURL);
                foreach (SteamAccounts.Stats stat in stats)
                {
                    string name = stat.name;
                    string value = stat.value.ToString();
                    if (name.Contains("Accuracy"))
                        value = string.Format("{0:N2}%", stat.value);

                    embed.AddField(name, value, true);
                }
                embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                if (config.FooterText != "")
                    embed.WithFooter(config.FooterText);
                if (config.TimeStamp)
                    embed.WithCurrentTimestamp();
                Global.PrintMsg(Context.User.Username, embed.Title);

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
        #endregion

        #region Server Config


        string[] configCMDs = { "prefix", "color", "footer", "servername", "timestamp", "afkchannel", "afktimeout", "cmdblacklist", "memeblacklist", "miningwhitelist",
        "verifyrole", "verification", "spamthreshold", "spamwarnamt", "spammutetime", "allowads"};
        string[] configDesc = { "Set the Prefix for the bot", "Set the color of the embed message using 0-255. Usage: color <r> <g> <b>", "Set the footer text of the embed message",
        "Change the server's name", "Enable or disable timestamp on the embed messages", "Set the afk channel for the server",
            "Set the time in minutes (1, 5, 15, 30, 60) of inactivity for users to be moved to the AFK channel", "Add a channel to the blacklist for the bot commands", "Add a channel to the meme blacklist",
        "Add a channel to the mining whitelist", "Set the default role for verified members | Usage: verifyrole <@role>", "Enable/Disable Verification when a user joins | Usage: verification <true|false>",
        "Spam warnings before being banned | Usage: spamthreshold 5", "Amount of spam to be muted | Usage: spamwarnamt 3", "Spam beginning mute time in mins, scales by 2 | Usage: spammutetime 5",
        "Allow users to @ everyone with discord link | Usage: allowads <true|false>"};

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Prefix(string prefix)
        {
            if (prefix == " " || prefix == "")
            {
                await Context.Channel.SendMessageAsync("Usage: prefix <char>\nExample: @ByscuitBot prefix -");
                return;
            }
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.Prefix = prefix;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Prefix Set", "Prefix has been set to " + prefix +
                ".\nYou can now use " + prefix + "help for the list of commands.", iconUrl : config.IconURL);
        }

        [Command("color")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Color(int red, int green, int blue)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.EmbedColorRed = red;
            config.EmbedColorGreen = green;
            config.EmbedColorBlue = blue;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Color Set", string.Format("**Red**: {0}\n**Green**: {1}\n**Blue**: {2}", red, green, blue), iconUrl: config.IconURL);
        }

        [Command("footer")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task FooterText([Remainder] string text)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            if (text == null || text == "")
            {
                await Context.Channel.SendMessageAsync(string.Format("Usage: {0}footer <text>\nExample: {0}footer Created By Abyscuit", config.Prefix));
            }
            config.FooterText = text;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Footer Text Set", string.Format("New footer text set to:\n{0}", text), iconUrl: config.IconURL);
        }

        [Command("servername")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task ServerName([Remainder] string text)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            if (text == null || text == "")
            {
                await Context.Channel.SendMessageAsync(string.Format("Usage: {0}servername <name>\nExample: {0}servername Da Byscuits", config.Prefix));
            }
            await Context.Guild.ModifyAsync(m => { m.Name = text; });
            config.DiscordServerName = text;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Footer Text Set", string.Format("New footer text set to:\n{0}", text), iconUrl: config.IconURL);
        }

        [Command("timestamp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Timestamp(string text)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            if (text == null || text == "")
            {
                await Context.Channel.SendMessageAsync(string.Format("Usage: {0}timestamp <true|false>", config.Prefix));
            }
            else
            {
                if (text.ToLower().Contains("enable") || text.ToLower().Contains("yes") || text.ToLower().Contains("on"))
                {
                    text = "true";
                }
                if (text.ToLower().Contains("disable") || text.ToLower().Contains("no") || text.ToLower().Contains("off"))
                {
                    text = "false";
                }
            }
            bool b = false;
            bool result = false;
            result = bool.TryParse(text, out b);
            if (result)
                config.TimeStamp = b;
            else
                await Context.Channel.SendMessageAsync(string.Format("Usage: {0}timestamp <true|false>", config.Prefix));
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Timestamp Configured", string.Format("Timestamp for embed set to **{0}**", text), iconUrl: config.IconURL);
        }

        [Command("afkchannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task AFKChannel([Remainder] string chanName)
        {
            IReadOnlyCollection<SocketVoiceChannel> vChans = Context.Guild.VoiceChannels;
            SocketVoiceChannel selectedChan = null;
            foreach (SocketVoiceChannel chan in vChans)
            {
                if (chan.Name.ToLower() == chanName.ToLower())
                {
                    selectedChan = chan;
                    break;
                }
            }
            if (selectedChan == null)
            {
                await Context.Channel.SendMessageAsync("Voice channel does not exist! Make sure you type it out correctly!\n(not case sensitive)");
            }
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AFKChannelID = selectedChan.Id;
            config.AFKChannelName = selectedChan.Name;
            await Context.Guild.ModifyAsync(m => { m.AfkChannel = selectedChan; });
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("AFK Channel Set", string.Format("**{0}** is now the AFK Channel!\nUsers will be sent there after **{1}mins**", selectedChan.Name, config.AFKTimeout / 60), iconUrl: config.IconURL);
        }

        [Command("afktimeout")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task AFKTimeout(int time)
        {
            time = time * 60;
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AFKTimeout = time;
            await Context.Guild.ModifyAsync(m => { m.AfkTimeout = time; });
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("AFK Timeout Set", string.Format("Users will be sent to {0} after **{1} minutes** of no activity!", config.AFKChannelName, config.AFKTimeout / 60), iconUrl: config.IconURL);
        }

        [Command("verifyrole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task VerifyRole(SocketRole role)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.VerificationRoleID = role.Id;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Verification Role Set", string.Format("Users will be added to the **{0}** role after verification!", role.Name), iconUrl: config.IconURL);
        }

        [Command("verification")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Verification(bool b)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.RequiresVerification = b;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Verification Set", string.Format("Verification set to {0}!", b), iconUrl: config.IconURL);
        }
        [Command("spamthreshold")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task SpamThreshold(int amt)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AntiSpamThreshold = amt;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Spam Threshold Set", string.Format("Threshold set to {0}!", amt), iconUrl: config.IconURL);
        }
        [Command("spamwarnamt")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task SpamWarnAmt(int amt)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AntiSpamWarn = amt;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Spam Warn Set", string.Format("Spam warnings set to {0}!", amt), iconUrl: config.IconURL);
        }
        [Command("spammutetime")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task SpamMuteTime(int mins)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AntiSpamTime = mins;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Spam Mute Time Set", string.Format("Spam mute time set to {0}!", mins), iconUrl: config.IconURL);
        }

        [Command("allowads")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task AllowAds(bool b)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AllowAdvertising = b;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Advertising Set", string.Format("Advertising set to {0}!", b), iconUrl: config.IconURL);
        }

        #endregion

        #region CryptoCurrency
        public static string[] miningCmds = { "nanopool", "linketh", "ethbal", "topcrypto", "calceth", "ethtokens", "crypto" };
        string[] miningDesc = { "Get NanoPool general account info | Usage: nanopool <optional:address>", "Link an ETH address to your discord account | Usage: linketh <address>",
        "Get the balance of an address or yours | Usage: ethbal <optional:address>", "Get the top 10 cryptocurrencies by market cap", "Calculate how much ETH you can mine using your Mh/s",
        "Get ERC-20 Tokens linked to Ethereum address | Usage: ethtokens <optional:address>", "Get basic price info of one or more coins use commas to separate | Usage: crypto btc,eth,bch"};
        [Command("nanopool")]
        public async Task Nanopool([Remainder]string address = null)
        {
            NanoPool.Account account = NanoPool.GetAccount(address, Context.User);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found!";
                await PrintEmbedMessage("NanoPool Account", msg);
                return;
            }


            msg += "**Account:** " + account.account;
            msg += "\n**Unconfirmed Balance:** " + account.unconfirmed_balance;
            msg += "\n**Balance:** " + account.balance;
            double balance = double.Parse(account.balance);
            NanoPool.Prices prices = NanoPool.GetPrices();
            double usdBal = prices.price_usd * balance;
            double btcBal = prices.price_btc * balance;
            double gbpBal = prices.price_gbp * balance;
            double eurBal = prices.price_eur * balance;
            msg += "\n**USD Value:** " + string.Format("${0:N2}", usdBal);
            msg += "\n**BTC Value:** " + string.Format("{0:N8} BTC", btcBal);
            msg += "\n**Hashrate:** " + account.hashrate + " Mh/s";
            msg += "\n\n__Average Hashrate__";
            msg += "\n**1 Hour:** " + account.avgHashrate["h1"] + " Mh/s";
            msg += "\n**3 Hours:** " + account.avgHashrate["h3"] + " Mh/s";
            msg += "\n**6 Hours:** " + account.avgHashrate["h6"] + " Mh/s";
            msg += "\n**12 Hours:** " + account.avgHashrate["h12"] + " Mh/s";
            msg += "\n**24 Hours:** " + account.avgHashrate["h24"] + " Mh/s";
            msg += "\n\n__Workers__";
            for (int i = 0; i < account.workers.Count; i++)
            {
                if (i > 0)
                    msg += "\n";
                msg += "\n**ID:** " + account.workers[i].id;
                msg += "\n**UID:** " + account.workers[i].uid;
                msg += "\n**Hashrate:** " + account.workers[i].hashrate + " Mh/s";
                msg += "\n**Last Share:** " + SteamAccounts.UnixTimeStampToDateTime(account.workers[i].lastshare);
                msg += "\n**Rating:** " + account.workers[i].rating;
                msg += "\n**1hr Avg Hashrate:** " + account.workers[i].h1 + " Mh/s";
                msg += "\n**3hr Avg Hashrate:** " + account.workers[i].h3 + " Mh/s";
                msg += "\n**6hr Avg Hashrate:** " + account.workers[i].h6 + " Mh/s";
                msg += "\n**12hr Avg Hashrate:** " + account.workers[i].h12 + " Mh/s";
                msg += "\n**24hr Avg Hashrate:** " + account.workers[i].h24 + " Mh/s";
            }

            await PrintEmbedMessage("NanoPool Account", msg);

            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("linketh")]
        public async Task LinkEth([Remainder]string address)
        {
            NanoPool.UserAccount account = NanoPool.GetUser(Context.User, address);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found!";
                await PrintEmbedMessage("ETH Link Failed", msg);
                return;
            }
            if (account.discordID != Context.User.Id)
            {
                msg += "**Address:** " + account.address;
                msg += "\n**Already Linked:** " + account.discordUsername;
            }
            else
            {
                msg += "**Address:** " + account.address;
                msg += "\n**Now Linked:** " + account.discordUsername;
            }

            await PrintEmbedMessage("ETH Address Linked", msg);

            //await Context.Channel.SendMessageAsync(msg);
        }


        [Command("ethbal")]
        public async Task Ethbal([Remainder]string address = null)
        {
            NanoPool.UserAccount account = NanoPool.GetUser(Context.User, address);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found! Please use linketh cmd or enter an address to link!";
                await PrintEmbedMessage("Account Retrieval Failed", msg);
                return;
            }
            if (address == null)
                address = account.address;


            double balance = double.Parse(EtherScan.GetBalance(address)) / 1000000000000000000d;
            NanoPool.Prices prices = NanoPool.GetPrices();
            double usdBal = prices.price_usd * balance;
            double btcBal = prices.price_btc * balance;
            double gbpBal = prices.price_gbp * balance;
            double eurBal = prices.price_eur * balance;
            msg += "**Account:** " + address;
            msg += "\n**Balance:** " + string.Format("{0:N15} ETH", balance);
            msg += "\n**USD Value:** " + string.Format("${0:N2}", usdBal);
            msg += "\n**BTC Value:** " + string.Format("{0:N8} BTC", btcBal);

            await PrintEmbedMessage("Ethereum Balance", msg);

            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("ethtokens")]
        public async Task EthTokens([Remainder]string address = null)
        {
            NanoPool.UserAccount account = NanoPool.GetUser(Context.User, address);
            string msg = "";
            if (account == null)
            {
                msg = "Account **" + address + "**\nNot Found! Please use linketh cmd or enter an address to link!";
                await PrintEmbedMessage("Account Retrieval Failed", msg);
                return;
            }
            if (address == null)
                address = account.address;
            List<EtherScan.Token> tokens = EtherScan.GetTokens(address);
            string symbols = "";
            int count = 0;
            foreach (EtherScan.Token token in tokens)
            {
                if (token.tokenSymbol != "" && !token.tokenName.ToLower().Contains("promo"))
                {
                    if (count == 1)
                    {
                        symbols += ",";
                        count = 0;
                    }
                    symbols += token.tokenSymbol;
                    count++;
                }
            }

            Dictionary<string, CoinMarketCap.Currency> currencies = CoinMarketCap.GetTokens(symbols);
            count = 0;
            int num = 0;
            msg += "**Account:** " + address;
            double totalValue = 0;
            foreach (EtherScan.Token token in tokens)
            {
                int decPlace = 0;
                if (token.tokenDecimal != "0" && token.tokenDecimal != "")
                    decPlace = int.Parse(token.tokenDecimal);
                double div = 1;
                for (int i = 0; i < decPlace; i++)
                {
                    div *= 10d;
                }
                double balance = double.Parse(token.value) / div;
                if (currencies.ContainsKey(token.tokenSymbol))
                    totalValue += currencies[token.tokenSymbol].quote.USD.price * balance;
            }
            msg += string.Format("\n**Total Value:** ${0:N5}", totalValue);
            foreach (EtherScan.Token token in tokens)
            {
                int decPlace = 0;
                if (token.tokenDecimal != "0" && token.tokenDecimal != "")
                    decPlace = int.Parse(token.tokenDecimal);
                double div = 1;
                for (int i = 0; i < decPlace; i++)
                {
                    div *= 10d;
                }
                msg += "\n\n**Name:** " + token.tokenName;
                msg += "\n**Symbol:** " + token.tokenSymbol;
                double balance = double.Parse(token.value) / div;
                if (decPlace == 0)
                    msg += "\n**Balance:** " + string.Format("{0:N0} " + token.tokenSymbol, balance);
                if (decPlace == 8)
                    msg += "\n**Balance:** " + string.Format("{0:N8} " + token.tokenSymbol, balance);
                if (decPlace == 18)
                    msg += "\n**Balance:** " + string.Format("{0:N15} " + token.tokenSymbol, balance);
                if (currencies.ContainsKey(token.tokenSymbol))
                    msg += string.Format("\n**USD Value:** ${0:N15}", currencies[token.tokenSymbol].quote.USD.price * balance);
                msg += "\n**Date:** " + SteamAccounts.UnixTimeStampToDateTime(double.Parse(token.timeStamp));
                msg += "\n**Confirmations:** " + string.Format("{0:N0}", ulong.Parse(token.confirmations));
                if (count > 8)
                {
                    msg += "|";
                    count = 0;
                }
                count++;
                num++;
            }
            string[] split = msg.Split('|');
            for (int i = 0; i < split.Length; i++)
            {
                await PrintEmbedMessage("ETH Tokens", split[i]);
            }


            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("crypto")]
        public async Task Crypto([Remainder]string symbols)
        {
            symbols = symbols.ToUpper();
            string[] Symbols = symbols.ToUpper().Split(',');
            string msg = "";
            Dictionary<string, CoinMarketCap.Currency> currencies = CoinMarketCap.GetTokens(symbols);
            for(int i =0;i<Symbols.Length;i++)
            {
                if (i > 0)
                    msg += "\n";
                msg += "\n**Name:** " + currencies[Symbols[i]].name;
                msg += "\n**Symbol:** " + currencies[Symbols[i]].symbol;
                msg += "\n**USD Price:** " + string.Format("${0:N2}",currencies[Symbols[i]].quote.USD.price);
                msg += "\n**Market Cap:** " + string.Format("${0:N2}", currencies[Symbols[i]].quote.USD.market_cap);
                msg += "\n**Volume 24h:** " + string.Format("${0:N2}", currencies[Symbols[i]].quote.USD.volume_24h);
                msg += "\n**1h Change:** " + string.Format("{0:N2}%", currencies[Symbols[i]].quote.USD.percent_change_1h);
                msg += "\n**24h Change:** " + string.Format("{0:N2}%", currencies[Symbols[i]].quote.USD.percent_change_24h);
                msg += "\n**7d Change:** " + string.Format("{0:N2}%", currencies[Symbols[i]].quote.USD.percent_change_7d);
                msg += "\n**Circulating Supply:** " + string.Format("{0:N0}", currencies[Symbols[i]].circulating_supply);
                msg += "\n**Max Supply:** " + string.Format("{0:N0}", currencies[Symbols[i]].max_supply);
            }
            await PrintEmbedMessage("Cyptocurrency " + symbols, msg);
        }
        [Command("topcrypto")]
        public async Task TopCrypto(int num = 0)
        {
            List<CoinMarketCap.Currency> currencies = CoinMarketCap.GetTop10();
            string msg = "";
            int count = 0;
            foreach (CoinMarketCap.Currency currency in currencies)
            {
                msg += "\n\n__" + currency.name + "__";
                msg += "\n**Symbol:** " + currency.symbol;
                msg += "\n**Rank:** " + currency.cmc_rank;
                msg += "\n**Price:** " + string.Format("${0:N2}", currency.quote.USD.price);
                msg += "\n**Market Cap:** " + string.Format("${0:N2}", currency.quote.USD.market_cap);
                msg += "\n**Volume 24h:** " + string.Format("${0:N2}", currency.quote.USD.volume_24h);
                msg += "\n**Change 1h:** " + string.Format("{0:N2}%", currency.quote.USD.percent_change_1h);
                msg += "\n**Change 24h:** " + string.Format("{0:N2}%", currency.quote.USD.percent_change_24h);
                msg += "\n**Change 7d:** " + string.Format("{0:N2}%", currency.quote.USD.percent_change_7d);
                msg += "\n**Circulating Supply:** " + currency.circulating_supply;
                string max_supply = "None";
                if (currency.max_supply != null)
                    max_supply = "" + currency.max_supply;
                msg += "\n**Max Supply:** " + max_supply;
                if (num != 0)
                {
                    num--;
                    if (num <= 0)
                        break;
                }
                count++;
                if (count >= 6)
                {
                    msg += "|";
                    count = 0;
                }
            }

            string[] split = msg.Split('|');
            for (int i = 0; i < split.Length; i++)
            {
                string title = "Top 10 Crypto";
                if (num != 0)
                    title = "Top " + num + " Crypto";
                await PrintEmbedMessage("Top Crypto", split[i]);
            }

            //await Context.Channel.SendMessageAsync(msg);
        }

        [Command("calceth")]
        public async Task CalcETH(float hashrate)
        {
            List<NanoPool.Amount> amounts = NanoPool.CalculateEth(hashrate);
            string msg = "";
            string[][] fields = new string[6][];
            int count = 0;
            int num = 0;
            foreach (NanoPool.Amount amount in amounts)
            {
                fields[num] = new string[2];
                switch (num)
                {
                    case 0:
                        {
                            fields[0][0] = "__Minute__";
                            //msg += "\n\n__Minute__";
                            break;
                        }
                    case 1:
                        {
                            fields[1][0] = "__Hour__";
                            //msg += "\n\n__Hour__";
                            break;
                        }
                    case 2:
                        {
                            fields[2][0] = "__Day__";
                            //msg += "\n\n__Day__";
                            break;
                        }
                    case 3:
                        {
                            fields[3][0] = "__Week__";
                            //msg += "\n\n__Week__";
                            break;
                        }
                    case 4:
                        {
                            fields[4][0] = "__Month__";
                            //msg += "\n\n__Month__";
                            break;
                        }
                    case 5:
                        {
                            fields[5][0] = "__Year__";
                            //msg += "\n\n__Month__";
                            break;
                        }
                }
                msg = "**Coins:** " + string.Format("{0:N10}", amount.coins);
                msg += "\n**Dollars:** " + string.Format("${0:N8}", amount.dollars);
                msg += "\n**Bitcoins:** " + string.Format("{0:N10}", amount.bitcoins);
                msg += "\n**Euros:** " + string.Format("€{0:N8}", amount.euros);
                msg += "\n**Pounds:** " + string.Format("£{0:N8}", amount.pounds);
                fields[num][1] = msg;
                num++;
                count++;
                if (count >= 6)
                {
                    //msg += "|";
                    count = 0;
                }
            }

            //string[] split = msg.Split('|');
            //for (int i = 0; i < split.Length; i++)
            {
                await PrintEmbedMessage("ETH Calculator "+ hashrate + " Mh/s", fields:fields);
            }

            //await Context.Channel.SendMessageAsync(msg);
        }

        #endregion

        #region Twitch
        string[] twitchCMDs = { "twitchuser", "twitchfollowers" };
        string[] twitchDesc = { "Get Twitch user details by user name | Usage: twitchuser <username>", "Get followers from a Twitch user by ID | Usage: twitchfollowers <id>" };
        [Command("twitchuser")]
        public async Task TwitchUser([Remainder]string user = null)
        {
            Twitch.TwitchUser User = Twitch.GetUser(user);
            string msg = "";
            msg += "**User ID:** " + User.id;
            msg += "\n**User Login:** " + User.login;
            msg += "\n**Display Name:** " + User.display_name;
            msg += "\n**Description:** " + User.description;
            msg += "\n**User Type:** " + User.type;
            if (User.email != null)
                msg += "\n**User Email:** " + User.email;
            msg += "\n**Broadcaster Type:** " + User.broadcaster_type;
            msg += "\n**View Count:** " + User.view_count;
            msg += "\n**Offline Image URL:** " + User.offline_image_url;
            msg += "\n**Profile Image URL:** " + User.profile_image_url;

            await PrintEmbedMessage("Twitch User", msg, iconUrl: User.profile_image_url);
        }

        [Command("twitchfollowers")]
        public async Task TwitchFollowers(string id)
        {
            Twitch.TwitchUser User = Twitch.GetUserByID(id);
            Twitch.FollowerResponse fResp = Twitch.GetFollowers(id);
            List<Twitch.Follower> followers = fResp.data;
            string msg = string.Format("__Followers {0}__", fResp.total);
            int count = 0;
            foreach (Twitch.Follower follower in followers)
            {
                msg += "\n\n**Follower ID:** " + follower.from_id;
                msg += "\n**Follower Name:** " + follower.from_name;
                msg += "\n**Followed ID:** " + follower.to_id;
                msg += "\n**Followed Name:** " + follower.to_name;
                msg += "\n**Followed At:** " + follower.followed_at;
                count++;
                if(count > 10)
                {
                    msg += "|";
                    count = 0;
                }
            }
            string[] split = msg.Split('|');
            for (int i = 0; i < split.Length; i++)
            {
                await PrintEmbedMessage("Twitch Followers for " + User.display_name, split[i], iconUrl: User.profile_image_url);
            }
        }

        #endregion

        #region Xbox calls
        [Command("clientinfo")]
        public async Task clientInfo(string CPUKey)
        {
            await Context.Channel.SendMessageAsync(ServerSQL.Select("consoles", "cpukey", CPUKey));
        }

        [Command("insert")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Insert([Remainder] string s)
        {
            await Context.Channel.SendMessageAsync(ServerSQL.Insert("consoles", new string[] { "uLogin","uHash","uType" }, s.Split(',')));
        }
        struct user
        {
            public uint id;
            public string uLogin;
            public string uHash;
            public string uType;
        }
        #endregion

        #region Verification

        [Command("verify")]
        public async Task Verify([Remainder]string captcha = null)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            SocketGuildUser user = (SocketGuildUser)Context.User;
            SocketRole role = Context.Guild.GetRole(config.VerificationRoleID);
            if (user.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " is already verified!");
                await Context.Message.DeleteAsync();
                return;
            }
            await user.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync(Context.User.Mention + " Verified!");
            await Context.Message.DeleteAsync();
        }

        #endregion

        public async Task PrintEmbedMessage(string title = "", string msg = "", string url = "", string iconUrl = "", string[][] fields = null)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if(title != "")
                embed.WithTitle(title);
            if(config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if(msg != "")
                embed.WithDescription(msg);
            if (url != "")
                embed.WithUrl(url);
            if (config.TimeStamp)
                embed.WithCurrentTimestamp();
            if(iconUrl != "")
                embed.WithThumbnailUrl(iconUrl);
            if(fields != null)
            {
                for(int i =0;i<fields.Length;i++)
                {
                    embed.AddField(fields[i][0], fields[i][1], true);
                }
            }
            
            //Console.WriteLine(DateTime.Now + " | " + title + " |\n" + msg);
            Console.WriteLine(DateTime.Now + " | " + Context.User.Username + " used " + title + " in " + Context.Guild.Name);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            
        }
    }
}
