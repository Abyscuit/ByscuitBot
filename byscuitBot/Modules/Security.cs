using byscuitBot.Core;
using byscuitBot.Core.Server_Data;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Modules
{
    public class Security : ModuleBase<SocketCommandContext>
    {
        #region Verification

        [Command("verify")]
        [Summary("Verify your account - Usage: {0}verify <~captcha~>")]
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


        #region Server Config


        string[] configCMDs = { "prefix", "color", "footer", "servername", "timestamp", "afkchannel", "afktimeout", "cmdblacklist", "memeblacklist", "miningwhitelist",
        "verifyrole", "verification", "spamthreshold", "spamwarnamt", "spammutetime", "allowads"};
        string[] configDesc = { "", "", "",
        "", "", "",
            "", "Add a channel to the blacklist for the bot commands", "Add a channel to the meme blacklist",
        "Add a channel to the mining whitelist", "", "",
        "", "", "",
        ""};

        [Command("prefix")]
        [Summary("Set the Prefix for the bot - Usage: {0}prefix <prefix>")]
        [RequireUserPermission(GuildPermission.Administrator)]
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
                ".\nYou can now use " + prefix + "help for the list of commands.", iconUrl: config.IconURL);
        }

        [Command("color")]
        [Summary("Set the color of the embed message using 0-255 - Usage: {0}color <Red> <Green> <Blue>")]
        [RequireUserPermission(GuildPermission.Administrator)]
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
        [Summary("Set the footer text of the embed message - Usage: {0}footer <message>")]
        [RequireUserPermission(GuildPermission.Administrator)]
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
        /*
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
        */

        [Command("timestamp")]
        [Summary("Enable or disable timestamp on the embed messages - Usage: {0}timestamp <true/false>")]
        [RequireUserPermission(GuildPermission.Administrator)]
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
        [Summary("Set the afk channel for the server - Usage: {0}afkchannel <Voice_Channel>")]
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
        [Summary("Set the time in minutes (1, 5, 15, 30, 60) of inactivity for users to be moved to the AFK channel - Usage: {0}afktimeout <number>")]
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
        [Summary("Set the default role for verified members - Usage: {0}verifyrole <@role>")]
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
        [Summary("Enable/Disable Verification when a user joins - Usage: {0}verification <true/false>")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Verification(bool b)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.RequiresVerification = b;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Verification Set", string.Format("Verification set to {0}!", b), iconUrl: config.IconURL);
        }
        [Command("spamthreshold")]
        [Summary("Number of warnings before being banned for spamming - Usage: {0}spamthreshold <number>")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SpamThreshold(int amt)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AntiSpamThreshold = amt;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Spam Threshold Set", string.Format("Threshold set to {0}!", amt), iconUrl: config.IconURL);
        }
        [Command("spamwarnamt")]
        [Summary("Number of warnings to be muted for spamming - Usage: {0}spamwarnamt <number>")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SpamWarnAmt(int amt)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AntiSpamWarn = amt;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Spam Warn Set", string.Format("Spam warnings set to {0}!", amt), iconUrl: config.IconURL);
        }
        [Command("spammutetime")]
        [Summary("Spam beginning mute time in mins - Usage: {0}spammutetime <number>")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SpamMuteTime(int mins)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AntiSpamTime = mins;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Spam Mute Time Set", string.Format("Spam mute time set to {0}!", mins), iconUrl: config.IconURL);
        }

        [Command("allowads")]
        [Summary("Allow users to @ everyone with discord link - Usage: {0}allowads <true/false>")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AllowAds(bool b)
        {
            ServerConfig config = ServerConfigs.GetConfig(Context.Guild);
            config.AllowAdvertising = b;
            ServerConfigs.UpdateServerConfig(Context.Guild, config);
            await PrintEmbedMessage("Advertising Set", string.Format("Advertising set to {0}!", b), iconUrl: config.IconURL);
        }

        #endregion


        #region encode/decode

        [Command("urlencode")]
        [Summary("Output a URL Encoded version of the text - Usage: {0}urlencode <input>")]
        public async Task UrlEncode([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + EncodeURL(text) + "```");
            await Context.Message.DeleteAsync();
        }

        [Command("sha1")]
        [Summary("Output a SHA1 hashed version of the text - Usage: {0}sha1 <input>")]
        public async Task Sha1([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Sha1Hash(text) + "```");
            await Context.Message.DeleteAsync();
        }

        [Command("sha256")]
        [Summary("Output a SHA256 hashed version of the text - Usage: {0}sha256 <input>")]
        public async Task Sha256([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + sha256(text) + "```");
            await Context.Message.DeleteAsync();
        }
        [Command("sha512")]
        [Summary("Output a SHA512 hashed version of the text - Usage: {0}sha512 <input>")]
        public async Task Sha512([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Sha512Hash(text) + "```");
            await Context.Message.DeleteAsync();
        }
        [Command("md5")]
        [Summary("Output a MD5 hashed version of the text - Usage: {0}md5 <input>")]
        public async Task MD5([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + MD5Hash(text) + "```");
            await Context.Message.DeleteAsync();
        }

        [Command("base64encode")]
        [Summary("Output a Base64 Encoded version of the text - Usage: {0}base64encode <input>")]
        public async Task EncodeBase64([Remainder]string text)
        {
            await Context.Channel.SendMessageAsync("```" + Base64Encode(text) + "```");
            await Context.Message.DeleteAsync();
        }
        [Command("base64decode")]
        [Summary("Output a Base64 Decoded version of the text - Usage: {0}base64decode <input>")]
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
        public static string DecodeUrl(string url)
        {
            url = WebUtility.UrlDecode(url);
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
