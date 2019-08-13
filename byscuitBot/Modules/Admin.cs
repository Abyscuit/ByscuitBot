using byscuitBot.Core;
using byscuitBot.Core.Server_Data;
using byscuitBot.Core.User_Accounts;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {

        #region admin
        [Command("shutdown")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        [Summary("Shuts down the bot completely")]
        public async Task Shutdown()
        {
            await Context.Channel.SendMessageAsync("Shutting down...");
            System.Environment.Exit(0);
            await Context.Channel.SendMessageAsync("Shut down failed.");
        }


        [Command("move")]
        [Summary("Move a user to a Voice Channel - Usage: {0}move <@user>")]
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

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kick a specified user - Usage: {0}kick <@user> <optional:reason>")]
        public async Task Kick(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            await user.KickAsync(reason);
            string msg = "__" + user.Username + "__\nReason: **" + reason + "**";

            await PrintEmbedMessage("Kicked", msg, iconUrl: user.GetAvatarUrl());
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Ban a specified user - Usage: {0}ban <@user> <days> <optional:reason>")]
        public async Task Ban(IGuildUser user, int days, [Remainder]string reason = "No reason provided.")
        {
            await user.Guild.AddBanAsync(user, days, reason);

            string msg = "__" + user.Username + "__\nReason: **" + reason + "**";
            Global.PrintMsg(Context.User.Username, msg);

            await PrintEmbedMessage("Banned", msg, iconUrl: user.GetAvatarUrl());
        }


        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        [Summary("Mutes a user - Usage: {0}mute <@user> <reason>")]
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
        [Summary("Unmutes a user - Usage: {0}unmute <@user> <reason>")]
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
        [Summary("Adds a role to a user - Usage: {0}addrole <@user> <role>")]
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
        [Summary("Adds roles to a user - Usage: {0}addroles <@user> <role, role, role>")]
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
        [Summary("Removes a role from a user - Usage: {0}removerole <@user> <role>")]
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
        [Summary("Removes all roles from a user - Usage: {0}removeallroles <@user>")]
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

        [Command("roles")]
        [Summary("Displays all the roles for a user - Usage: {0}roles <optional:@user>")]
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

        [Command("nickname")]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        [Summary("Change the nickname of a user - Usage: {0}nickname <@user> <nickname>")]
        public async Task Nickname(SocketGuildUser user, [Remainder]string nickname)
        {

            await user.ModifyAsync(m => { m.Nickname = nickname; });

            string msg = "**" + user.Mention + " is now " + nickname + "**";

            await PrintEmbedMessage("Nickname Set", msg, iconUrl: user.GetAvatarUrl());

        }


        [Command("warn")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Warn a user - Usage: {0}warn <@user> <optional:reason>")]
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
                    "**Warn Percent:** " + warnpercent + "";

                await PrintEmbedMessage("Warning", msg, iconUrl: user.GetAvatarUrl());
            }
        }

        [Command("serverstats")]
        [Summary("Get the stats of the server - Usage: {0}serverstats")]
        public async Task ServerStats()
        {
            int memberCount = Context.Guild.MemberCount;
            DateTimeOffset date = Context.Guild.CreatedAt;
            string name = Context.Guild.Name;
            string owner = Context.Guild.Owner.Username;

            ServerConfig serverConfig = ServerConfigs.GetConfig(Context.Guild);
            bool reqV = serverConfig.RequiresVerification;
            string verification = string.Format("\nRequires Verification: **{0}**", reqV);
            if (reqV)
            {
                SocketRole vRole = Context.Guild.GetRole(serverConfig.VerificationRoleID);
                verification += string.Format("\nVerified Role: **{0}**", vRole.Name);
            }
            string configs = "\nBot Prefix: **" + serverConfig.Prefix + "**" + verification + "\nAFK Channel: **" + Context.Guild.GetVoiceChannel(serverConfig.AFKChannelID).Name +
                "**\nAFK Timeout: **" + serverConfig.AFKTimeout + "**\nAntispam Mute Time: **" + serverConfig.AntiSpamTime + "**\nAntispam Warn Ban: **" + serverConfig.AntiSpamWarn;
            string msg = "Server Name: **" + name + "**\nOwner: **" + owner + "**\nMember Count: **" + memberCount + "**\nDate Created: **" + date.ToString("MM/dd/yyyy hh:mm:ss") + "**" + configs +
                "**\n\n__Roles__\n**" + getAllRoles((SocketGuildUser)Context.User) + "**";


            var result = from s in Context.Guild.VoiceChannels
                         where s.Name.Contains("Member Count")
                         select s;


            await PrintEmbedMessage("Server Stats", msg, iconUrl: Context.Guild.IconUrl);
            SocketVoiceChannel memberChan = result.FirstOrDefault();
            await memberChan.ModifyAsync(m => { m.Name = "Member Count: " + Context.Guild.MemberCount; });
            DataStorage.AddPairToStorage(memberChan.Guild.Name + " MemberChannel", memberChan.Id.ToString());

        }

        [Command("stats")]
        [Summary("Get the stats for a user - Usage: {0}stats <optional:@user>")]
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
            {
                if (!Context.IsPrivate)
                    user = CommandHandler.GetUser(Context.Guild.Users, username);
                else
                    user = CommandHandler.GetUser(Context.User.MutualGuilds.ToArray()[0].Users, username);
            }


            string msg = "**Level:** " + account.LevelNumber + "\n**XP:** " + account.XP + "\n**Points:** " + account.Points + "\n" +
                "**IsMuted:** " + account.IsMuted + "\n**Number of Warnings:** " + account.NumberOfWarnings;
            if (!Context.IsPrivate)
            {
                string permissions = "\n__Permissions__\nAdmin: **" + user.GuildPermissions.Administrator + "**\nManage Roles: **" + user.GuildPermissions.ManageRoles + "**\nMute Members: **" + user.GuildPermissions.MuteMembers +
                    "**\nDeafen Members: **" + user.GuildPermissions.DeafenMembers + "**\nKick Members: **" + user.GuildPermissions.KickMembers + "**\nBan Members: **" + user.GuildPermissions.BanMembers +
                    "**\nChange Members Nickname: **" + user.GuildPermissions.ManageNicknames + "**\nMove Members: **" + user.GuildPermissions.MoveMembers + "**";

                string endMsg = "\n\n__Roles__\n**" + getRoles(user) + "**" + permissions;
                msg += "\n**Joined At:** " + user.JoinedAt + endMsg;
            }
            //string msg = "XP: " + account.XP + "\nPoints: " + account.Points;




            await PrintEmbedMessage("Stats for " + username, msg, iconUrl: user.GetAvatarUrl());
        }


        [Command("test")]
        [Summary("Test Command for debuggin\'")]
        [RequireOwner()]
        public async Task Test()
        {
            List<CommandInfo> cmds = CommandHandler.GetCommands();
            string msg = "User Commands:\n";
            string adminCmds = "Admin Commands:\n";
            string ownerCmds = "Owner Commands:\n";
            int x = 0;
            int y = 0;
            foreach (CommandInfo cmd in cmds)
            {
                if (cmd.Preconditions.Count > 0)
                {
                    foreach (PreconditionAttribute precondition in cmd.Preconditions)
                    {
                        if (precondition is RequireBotPermissionAttribute) {
                            RequireBotPermissionAttribute attribute = precondition as RequireBotPermissionAttribute;
                            //msg += "*Requires Bot Permission: " + attribute.GuildPermission + "*\n";
                        }
                        else if(precondition is RequireUserPermissionAttribute)
                        {
                            RequireUserPermissionAttribute attribute = precondition as RequireUserPermissionAttribute;
                            //msg += "*Requires User Permission: " + attribute.GuildPermission + "*\n";
                            if (AdminAttribute(attribute))
                            {
                                adminCmds += "**__" + cmd.Name + "__**\n";
                                if (!string.IsNullOrEmpty(cmd.Summary))
                                    adminCmds += "*" + cmd.Summary + "*\n";
                                y++;
                            }
                            else
                            {
                                msg += "**__" + cmd.Name + "__**\n";
                                if (!string.IsNullOrEmpty(cmd.Summary))
                                    msg += "*" + cmd.Summary + "*\n";
                                x++;
                            }
                        }
                        else if(precondition is RequireOwnerAttribute)
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
                    msg += "**__" + cmd.Name + "__**\n";
                    if (!string.IsNullOrEmpty(cmd.Summary))
                        msg += "*" + cmd.Summary + "*\n";
                    x++;
                }

                if (y > 10)
                {
                    adminCmds += "|";
                    y = 0;
                }
                if (x > 10)
                {
                    msg += "|";
                    x = 0;
                }
            }

            string prefix = "/";
            ServerConfig config = null;
            if (!Context.IsPrivate) config = ServerConfigs.GetConfig(Context.Guild);
            if (config != null) prefix = config.Prefix;
            foreach (string s in SplitMessage(msg, '|'))
                if (!string.IsNullOrEmpty(s))
                    await Context.Channel.SendMessageAsync(string.Format(s, prefix));
            foreach (string s in SplitMessage(adminCmds, '|'))
                if (!string.IsNullOrEmpty(s))
                    await Context.Channel.SendMessageAsync(string.Format(s, prefix));
            await Context.Channel.SendMessageAsync(ownerCmds);

        }

        #endregion
        public static bool AdminAttribute(RequireUserPermissionAttribute attribute)
        {
            if (!attribute.GuildPermission.HasValue) return false;
            bool result = false;
            if (attribute.GuildPermission.Value == GuildPermission.Administrator) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.BanMembers) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.DeafenMembers) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.KickMembers) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.ManageChannels) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.ManageGuild) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.ManageMessages) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.ManageNicknames) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.ManageRoles) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.ManageWebhooks) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.MoveMembers) result = true;
            if (attribute.GuildPermission.Value == GuildPermission.MuteMembers) result = true;
            return result;
        }
        private string[] SplitMessage(string text, char delimiter)
        {
            return text.Split(delimiter);
        }
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
                if (!role.IsEveryone)
                    roles += role.Name + "\n";
            }

            return roles;
        }
    }
}
