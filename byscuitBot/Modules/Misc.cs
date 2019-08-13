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
using static byscuitBot.Core.ServerSQL;
using System.Diagnostics;
using System.Threading;

namespace byscuitBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        string invLink = "https://discord.gg/VYMph8k";
        public Random rand = new Random();

        string[] shop = { "Colin's Bitches", "Well-Reknown Byscuit", "Seasoned Byscuit", "Professional Byscuit", "BUTTERED UP BYSCUIT", "DaCrew", "Moderator" };
        int[] price = { 200, 400, 800, 1600, 3200, 6000, 6500 };


        #region users


        //giveaway <time> <winners> <Item>
        [Command("giveaway")]
        [Summary("Create a giveaway - {0}giveaway <dhm>,<prize>")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
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
        /*
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
        */

        [Command("level")]
        [Summary("Get the level of a user - Usage: {0}level <optional:@user>")]
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
        [Summary("Searches youtube for videos - Usage: {0}youtube <search terms>")]
        public async Task Youtube([Remainder]string keyword)
        {
            YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Config.botconf.GOOGLE_API_KEY,
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
        [Summary("Use this to make a selection - Usage: {0}select <int>")]
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

        
        [Command("help")]
        public async Task Help()
        {
            if(Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("`This command is only available in the server!`");
                return;
            }
            ServerConfig serverConfig = ServerConfigs.GetConfig(Context.Guild); 
            string msg = "Use the '" + serverConfig.Prefix + "' prefix to send a command.\n";
            List<CommandInfo> cmds = CommandHandler.GetCommands();
            string userCmds = "";
            string adminCmds = "";
            string ownerCmds = "";
            int x = 0;
            int y = 0;
            SocketGuildUser user = Context.User as SocketGuildUser;
            foreach (CommandInfo cmd in cmds)
            {
                if (cmd.Name.ToLower() == "help") continue;
                if (cmd.Preconditions.Count > 0)
                {
                    foreach (PreconditionAttribute precondition in cmd.Preconditions)
                    {
                        if (precondition is RequireBotPermissionAttribute)
                        {
                            RequireBotPermissionAttribute attribute = precondition as RequireBotPermissionAttribute;
                            //msg += "*Requires Bot Permission: " + attribute.GuildPermission + "*\n";
                        }
                        else if (precondition is RequireUserPermissionAttribute)
                        {
                            RequireUserPermissionAttribute attribute = precondition as RequireUserPermissionAttribute;
                            //msg += "*Requires User Permission: " + attribute.GuildPermission + "*\n";
                            if (!user.GuildPermissions.Has(attribute.GuildPermission.Value) || !attribute.GuildPermission.HasValue) continue;
                            if (Admin.AdminAttribute(attribute))
                            {
                                adminCmds += "**__" + cmd.Name + "__**\n";
                                if (!string.IsNullOrEmpty(cmd.Summary))
                                    adminCmds += "*" + cmd.Summary + "*\n";
                                y++;
                            }
                            else
                            {
                                userCmds += "**__" + cmd.Name + "__**\n";
                                if (!string.IsNullOrEmpty(cmd.Summary))
                                    userCmds += "*" + cmd.Summary + "*\n";
                                x++;
                            }
                        }
                        else if (precondition is RequireOwnerAttribute)
                        {
                            RequireOwnerAttribute attribute = precondition as RequireOwnerAttribute;
                            ownerCmds += "**__" + cmd.Name + "__**\n";
                            if (!string.IsNullOrEmpty(cmd.Summary))
                                ownerCmds += "*" + cmd.Summary + "*\n";
                        }
                    }
                }
                else
                {
                    userCmds += "**__" + cmd.Name + "__**\n";
                    if (!string.IsNullOrEmpty(cmd.Summary))
                        userCmds += "*" + cmd.Summary + "*\n";
                    x++;
                }

                if (y > 20)
                {
                    adminCmds += "|";
                    y = 0;
                }
                if (x > 16)
                {
                    userCmds += "|";
                    x = 0;
                }
            }

            string prefix = "/";
            ServerConfig config = null;
            if (!Context.IsPrivate) config = ServerConfigs.GetConfig(Context.Guild);
            if (config != null) prefix = config.Prefix;
            foreach (string s in SplitMessage(userCmds, '|'))
                if (!string.IsNullOrEmpty(s))
                    await DMEmbedMessage("Commands", string.Format(s, prefix));
            foreach (string s in SplitMessage(adminCmds, '|'))
                if (!string.IsNullOrEmpty(s))
                    await DMEmbedMessage("Admin Commands", string.Format(s, prefix));
            if(user.GuildPermissions.Administrator)
                await DMEmbedMessage("Owner Commands", string.Format(ownerCmds, prefix));
        }

        private string[] SplitMessage(string text, char delimiter)
        {
            return text.Split(delimiter);
        }


        [Command("invlink")]
        [Summary("Gets the invite link for the ByscuitBros")]
        public async Task InviteLink()
        {
            Global.PrintMsg(Context.User.Username, "Posting Invite Link");
            await Context.Channel.SendMessageAsync(Context.User.Mention + " "+ invLink, false);
        }

        [Command("pointshop")]
        [Summary("Opens up the pointshop")]
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
        [Summary("Gets an invite link for the server")]
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
        [Summary("Deletes a specified amount of messages in the channel - Usage: {0}clear <number>")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int num)
        {
            await Context.Message.DeleteAsync();
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> x = Context.Channel.GetMessagesAsync((num));
            IAsyncEnumerator<IReadOnlyCollection<IMessage>> index = x.GetEnumerator();
            while (await index.MoveNext())
            {
                foreach (IMessage msg in index.Current)
                {
                    await msg.DeleteAsync();
                }
            }
            RepeatingTimer.channel = (SocketTextChannel)Context.Channel;
            RepeatingTimer.clrMsg = true;
            RepeatingTimer.clrMsgTime = DateTime.Now.AddSeconds(5);
            RestUserMessage m = await Context.Channel.SendMessageAsync("Cleared " + num + " messages!");
            Global.MessageIdToTrack = m.Id;
        }

        [Command("status")]
        [Summary("Gets the status of the bot")]
        public async Task Status()
        {
            int latency = Context.Client.Latency;
            string activity = Context.Client.Activity.Name;
            string user = "" + Context.Client.CurrentUser;
            List<SocketGuild> guilds = Context.Client.Guilds.ToList();
            string msg = string.Format("**Latency**: {0}ms\n", latency);
            msg += "**Activity**: " + activity + "\n";
            msg += "**User**: " + user + "\n";
            msg += "**Guilds**:\n";
            foreach (SocketGuild guild in guilds)
                msg += string.Format("*{0}*{1}", guild.Name, "\n");
            await PrintEmbedMessage("Bot Status", msg);
        }

        #endregion

        #region media
        [Command("meme")]
        [Summary("Posts a meme")]
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
            if (text != null && text != "")
            {
                string[] tags = text.Split(' ');
                List<Meme> memes = MemeLoader.SearchMeme(tags);
                if (memes.Count == 1)
                    memeToPost = "Memes/" + memes[0].path;
                else if (memes.Count > 1)
                {
                    Meme meme = memes[rand.Next(0, memes.Count - 1)];
                    memeToPost = "Memes/" + meme.path;
                }
                useMeme = false;
            }
            Console.WriteLine("Posting " + memeToPost);
            await Context.Channel.SendFileAsync(memeToPost);
            if (useMeme)
                Global.MemesUsed.Add(memeToPost);
            if (Global.MemesUsed.Count >= 20)
                Global.MemesUsed.RemoveAt(0);
        }

        [Command("creatememe")]
        [Summary("Create a meme using an attachment or a user\'s profile picture - Usage: {0}creatememe <@user> <top text, bottom text>")]
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
        [Summary("Create a meme using an attachment - Usage: {0}creatememe <blank_space> <top text, bottom text>")]
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
        [Summary("Upload a picture to the meme folder - Usage: {0}upload <name> <tags,tags,tags>")]
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

        [Command("urban")]
        [Summary("Searches the Urban Dictionary for the specified term - Usage: {0}urban <search term(s)>")]
        public async Task Urban([Remainder]string word)
        {
            word = char.ToUpper(word[0]) + word.Substring(1);
            Console.WriteLine("Encoded word: " + Security.EncodeURL(word) + "\n");
            string msg = UrbanDictionary.GetDefinition(Security.EncodeURL(word));

            await PrintEmbedMessage(word + " - Urban Dictionary", msg);
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
        [Summary("Resolve Steam URL or Username to SteamID64 - Usage: {0}resolve <URL/Username>")]
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
        [Summary("Get Account Summary of your linked account or a steam user - Usage: {0}steaminfo <optional:ID> <optional:URL/Username>")]
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
        [Summary("Get steam account bans (VAC, community, economy) - Usage: {0}steambans <optional:ID> <optional:URL/Username>")]
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

        [Command("comparegames")]
        [Summary("Get all Steam games in common with other user - Usage: {0}comparegames <@user>")]
        public async Task CompareGames(SocketUser socketUser)
        {
            int appID = 0;

            SteamAccount user = SteamAccounts.GetAccount(Context.User);
            SteamAccount otherUser = SteamAccounts.GetAccount(socketUser);
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " You must link your account using the steamlink command!");
                return;
            }
            if (otherUser == null)
            {
                await Context.Channel.SendMessageAsync(socketUser + " needs to link their account using the steamlink command!");
                return;
            }
            ulong id = user.SteamID;
            ulong otherID = otherUser.SteamID;

            SteamAccounts.GameResponse player = SteamAccounts.GetAccountGames(id);
            SteamAccounts.GameResponse otherPlayer = SteamAccounts.GetAccountGames(otherID);
            string msg = "";
            string title = "Steam Games";
            string imageUrl = SteamAccounts.GetAccountObject(id).avatarfull;
            msg += "**" + Context.User + " Game Count:** " + player.game_count + "\n";
            msg += "**" + socketUser + " Game Count:** " + otherPlayer.game_count + "\n\n";
            int count = 0;
            for (int i = 0; i < player.games.Count; i++)
            {
                string name = player.games[i].name;
                if (appID == 0)
                {
                    for (int x = 0; x < otherPlayer.games.Count; x++)
                    {
                        if (otherPlayer.games[x].appid == player.games[i].appid)
                        {
                            msg += string.Format("**__{0}__\nappID:** {1}\n", name, player.games[i].appid);
                            if (player.games[i].playtime_2weeks != null)
                                msg += string.Format("**{1} Playtime Last 2 Weeks:** {0:N2}hrs\n", player.games[i].playtime_2weeks / 60f, Context.User);
                            msg += string.Format("**{1} Playtime Total:** {0:N2}hrs\n\n", player.games[i].playtime_forever / 60f, Context.User);
                            if (otherPlayer.games[i].playtime_2weeks != null)
                                msg += string.Format("**{1} Playtime Last 2 Weeks:** {0:N2}hrs\n", player.games[i].playtime_2weeks / 60f, socketUser);
                            msg += string.Format("**{1} Playtime Total:** {0:N2}hrs\n\n", player.games[i].playtime_forever / 60f, socketUser);
                            //msg += string.Format("**{0} Image Icon Url Hash:** {1}\n", name, player.games[i].img_icon_url);
                            //msg += string.Format("**{0} Image Logo Url Hash:** {1}\n", name, player.games[i].img_logo_url);
                            count++;
                            if (count % 12 == 0 && count > 0)
                            {
                                msg += "|";
                            }
                        }// end if app ids match
                    }//end otherplayer game loop
                }
                else
                {
                    if (player.games[i].appid == appID)
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
        [Command("steamgames")]
        [Summary("Get all Steam games or time played for a specific game - Usage: {0}steamgames <optional:Game_Name>")]
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
        [Summary("Link a Steam account to your Discord Account - Usage: {0}linksteam <optional:ID> <optional:URL/Username>")]
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
        [Summary("Get all Steam accounts linked to Discord users - Usage: {0}steamaccts")]
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
        [Summary("Get relevant CS:GO stats (totals, last match) - Usage: {0}csgostats <optional:ID> <optional:URL/Username>")]
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
        [Summary("Get the info of the CS:GO last match - Usage: {0}csgolastmatch <optional:ID> <optional:URL/Username>")]
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
        [Summary("Get CS:GO info for up to 10 games of your last matches saved - Usage: {0}csgolastmatches <optional:number>")]
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
        [Summary("Get CS:GO weapon stats of all weapons or a specific weapon - Usage: {0}csgowepstats <optional:weapon_name>")]
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


        #region Twitch
        string[] twitchCMDs = { "twitchuser", "twitchfollowers" };
        string[] twitchDesc = { "Get Twitch user details by user name | Usage: twitchuser <username>", "Get followers from a Twitch user by ID | Usage: twitchfollowers <id>" };
        [Command("twitchuser")]
        [Summary("Get the details of a Twitch user - Usage: {0}twitchuser <username>")]
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
        [Summary("Get followers of a Twitch user by ID - Usage: {0}twitchfollowers <id>")]
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
        [Summary("Get the info of a user by CPU Key - Usage: {0}clientinfo <CPU_Key>")]
        public async Task clientInfo(string CPUKey)
        {
            string result = "";
            Xbox xbox = null; 
            if(ServerSQL.IsFreeMode())xbox = ServerSQL.Select("guest", "cpukey", CPUKey);
            else xbox = ServerSQL.Select("consoles", "cpukey", CPUKey);
            if (xbox.CPUKey == "Failed")
            {
                await PrintEmbedMessage(xbox.CPUKey, "Time: " + xbox.Time);
                return;
            }
            result = "**CPUKey:** " + xbox.CPUKey + "\n**Time Left:** "+xbox.Time + 
                "\n**Gamertag:** "+ xbox.Gamertag + 
                "\n**Game:** "+Global.getTitle(Global.GAME_HEX[rand.Next(Global.GAME_HEX.Length)]);
            await PrintEmbedMessage("ID: " + xbox.ID + " | " + xbox.Gamertag, result);
            //await Context.Channel.SendMessageAsync(result);
        }

        string[] hexDig = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        [Command("insert")]
        [Summary("Creates a fake entry for debugging - Usage: {0}insert <name>")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Insert(string name)
        {
            //CPUKey generation
            string cpukey = "";
            for (int i = 0; i < 32; i++)
            {
                int rint = rand.Next(hexDig.Length);
                for (int r = 0; r < 3; r++) rint = rand.Next(hexDig.Length); 
                cpukey += hexDig[rint];
            }//end generation

            //Time generation
            DateTime time = DateTime.Now;
            double rnd = rand.NextDouble();
            double pwr = Math.Pow(10, 5) * 6;
            rnd *= pwr;
            time = time.AddSeconds(rnd);
            //end time gen

            //Prepare SQL columns and values
            string[] columns = { "name", "cpukey","time","enabled" };   //INSERT INTO ? (name, cpukey, time, enabled)
            string[] vals =                                             //VALUES
            {                                                           //(
                "'"+name+"'",                                           //'@name',
                "'" +cpukey+"'",                                        //'@cpukey'
                "TIMESTAMP('"+time.ToString("yyyy-MM-dd hh:MM:ss")+"')",//TIMESTAMP('yyyy-mm-dd hh:mm:ss')
                "1"                                                     //1
            };                                                          //);
            await Context.Channel.SendMessageAsync(ServerSQL.Insert("consoles", columns , vals));   //INSERT INTO consoles (name, cpukey, time, enabled)
                                                                                                    //VALUES ('@name', '@cpukey', TIMESTAMP('yyyy-mm-dd hh:mm:ss'), 1);

            DateTime now = DateTime.Now;
            TimeSpan timeLeft = time.Subtract(now);
            double totalSeconds = timeLeft.TotalSeconds;
            double totalMins = totalSeconds / 60;
            double totalHours = totalMins / 60;
            double totalDays = totalHours / 24;
            string totalTime = (int)totalDays + "d " + (int)(totalHours % 24) + "h " + (int)(totalMins % 60) + "m";
            if (totalDays >= 300)
                totalTime = "Lifetime";
            if (totalSeconds < 0)
                totalTime = "Expired";

            await PrintEmbedMessage("New \"Console\" Added",string.Format( "**Name:** {3}{0}**CPUKey:** {1}{0}**Time Left:** {2}", "\n", cpukey, totalTime,name));
        }
        /*
        [Command("update")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Update(string name)
        {
            
            
            //CPUKey generation
            string cpukey = "";
            for (int i = 0; i < 32; i++)
            {
                int rint = rand.Next(hexDig.Length);
                for (int r = 0; r < 3; r++) rint = rand.Next(hexDig.Length);
                cpukey += hexDig[rint];
            }//end generation

            //Time generation
            DateTime time = DateTime.Now;
            double rnd = rand.NextDouble();
            double pwr = Math.Pow(10, 5) * 6;
            rnd *= pwr;
            time = time.AddSeconds(rnd);
            //end time gen

            //Prepare SQL columns and values
            string[] columns = { "name", "cpukey", "time", "enabled" };   //INSERT INTO ? (name, cpukey, time, enabled)
            string[] vals =                                             //VALUES
            {                                                           //(
                "'"+name+"'",                                           //'@name',
                "'" +cpukey+"'",                                        //'@cpukey'
                "TIMESTAMP('"+time.ToString("yyyy-MM-dd hh:MM:ss")+"')",//TIMESTAMP('yyyy-mm-dd hh:mm:ss')
                "1"                                                     //1
            };                                                          //);
            await Context.Channel.SendMessageAsync(ServerSQL.Insert("consoles", columns, vals));   //INSERT INTO consoles (name, cpukey, time, enabled)
                                                                                                   //VALUES ('@name', '@cpukey', TIMESTAMP('yyyy-mm-dd hh:mm:ss'), 1);

            DateTime now = DateTime.Now;
            TimeSpan timeLeft = time.Subtract(now);
            double totalSeconds = timeLeft.TotalSeconds;
            double totalMins = totalSeconds / 60;
            double totalHours = totalMins / 60;
            double totalDays = totalHours / 24;
            string totalTime = (int)totalDays + "d " + (int)(totalHours % 24) + "h " + (int)(totalMins % 60) + "m";
            if (totalDays >= 300)
                totalTime = "Lifetime";
            if (totalSeconds < 0)
                totalTime = "Expired";

            await PrintEmbedMessage("New \"Console\" Added", string.Format("**Name:** {3}{0}**CPUKey:** {1}{0}**Time Left:** {2}", "\n", cpukey, totalTime, name));
            
        }*/
        #endregion

        public struct EmbedField
        {
            public string name;
            public string value;
        }
        public async Task PrintEmbedMessage(string title = "", string msg = "", string url = "", string iconUrl = "", EmbedField[] fields = null, IUser author = null)
        {
            ServerConfig config = new ServerConfig();
            if (!Context.IsPrivate)
                config = ServerConfigs.GetConfig(Context.Guild);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (title != "")
                embed.WithTitle(title);
            if (config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (msg != "")
                embed.WithDescription(msg);
            if (author != null)
                embed.WithAuthor(author);
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
            string username = Context.User.Username + "#" + Context.User.Discriminator;
            string guildName = "Private Message";
            if (!Context.IsPrivate) guildName = Context.Guild.Name;
            string text = DateTime.Now + " | " + username + " used " + title + " in " + guildName;
            Console.WriteLine(text);
            Log.AddTextToLog(text);
            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }

        public async Task DMEmbedMessage(string title = "", string msg = "", string url = "", string iconUrl = "", EmbedField[] fields = null, IUser author = null)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            if (title != "")
                embed.WithTitle(title);
            if (config.FooterText != "")
                embed.WithFooter(config.FooterText);
            if (msg != "")
                embed.WithDescription(msg);
            if (author != null)
                embed.WithAuthor(author);
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
            string username = Context.User.Username + "#" + Context.User.Discriminator;
            string guildName = "Private Message";
            if (!Context.IsPrivate) guildName = Context.Guild.Name;
            string text = DateTime.Now + " | " + username + " used " + title + " in " + guildName;
            Console.WriteLine(text);
            Log.AddTextToLog(text);

            var x = await Context.User.GetOrCreateDMChannelAsync();
            await x.SendMessageAsync("", false, embed.Build());
        }
    }
}
