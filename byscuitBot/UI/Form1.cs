using byscuitBot.Core.Server_Data;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace byscuitBot
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        DiscordSocketClient Client;
        List<SocketGuild> guilds;
        IReadOnlyCollection<SocketRole> roles;
        IReadOnlyCollection<SocketTextChannel> textChannels;
        IReadOnlyCollection<SocketVoiceChannel> voiceChannels;
        IReadOnlyCollection<SocketGuildUser> users;
        ServerConfig config;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateServers();
            serversCBox.SelectedIndex = 0;
            //changeSettings();
        }
        public void updateServers()
        {
            guilds = Program.client.Guilds.ToList();
            foreach (SocketGuild guild in guilds)
                serversCBox.Items.Add(guild.Name);
        }

        private void changeSettings()
        {
            textChannelCBox.Items.Clear();
            afkChanCBox.Items.Clear();
            verRoleCBox.Items.Clear();
            newUsrChanCBox.Items.Clear();
            usersCBox.Items.Clear();
            SocketGuild guild = guilds[serversCBox.SelectedIndex];
            textChannels = guild.TextChannels;
            voiceChannels = guild.VoiceChannels;
            roles = guild.Roles;
            users = guild.Users;
            foreach (SocketTextChannel chan in textChannels)
            {
                textChannelCBox.Items.Add(chan.Name);
                newUsrChanCBox.Items.Add(chan.Name);
            }
            foreach (SocketVoiceChannel chan in voiceChannels) afkChanCBox.Items.Add(chan.Name);
            foreach (SocketRole role in roles)
                if (!role.IsEveryone)
                    verRoleCBox.Items.Add(role.Name);
            foreach (SocketGuildUser user in users) usersCBox.Items.Add(user.Username);

            ServerConfigs.LoadServerConfigs();
            config = ServerConfigs.GetConfig(guild);
            tokenTxt.Text = Config.botconf.token;
            prefixTxt.Text = config.Prefix;
            string[] features = guild.Features.ToArray();
            string featString = Environment.NewLine + "Features:";
            foreach (string f in features)
                featString += Environment.NewLine + f;
            statsTxt.Text = string.Format("Stats:{0}Total Users: {1}{0}Owner: {2}{0}Content Filter: {3}{0}Created On: {4}{0}Verification Level: {5}{0}Default Notifications: {6}",
                Environment.NewLine, guild.MemberCount, guild.Owner, guild.ExplicitContentFilter.ToString(), guild.CreatedAt,
                guild.VerificationLevel.ToString(), guild.DefaultMessageNotifications.ToString());
            afkChanCBox.SelectedItem = config.AFKChannelName;
            veriToggle.Checked = config.RequiresVerification;
            if (veriToggle.Checked)
                verRoleCBox.SelectedItem = guild.GetRole(config.VerificationRoleID).Name;
            textChannelCBox.SelectedIndex = 0;
            afkTimeCBox.SelectedItem = ""+(config.AFKTimeout / 60);
            System.Drawing.Color color = System.Drawing.Color.FromArgb(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
            colorBtn.Text = string.Format("Color: {0},{1},{2}", color.R, color.G, color.B);
            colorPrev.BackColor = color;
            tsToggle.Checked = config.TimeStamp;
            adToggle.Checked = config.AllowAdvertising;
            spamWarnTxt.Text = ""+config.AntiSpamWarn;
            spamThresholdTxt.Text = ""+config.AntiSpamThreshold;
            spamTimeTxt.Text = "" + config.AntiSpamTime;
            footerTxt.Text = config.FooterText;
            newUsrMsgToggle.Checked = config.NewUserMessage;
            if (newUsrMsgToggle.Checked)
                newUsrChanCBox.SelectedItem = guild.GetTextChannel(config.NewUserChannel).Name;
            eMentionTog.Checked = config.BlockMentionEveryone;
            //this.Refresh();
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            //Config.botconf.token = tokenTxt.Text;
        }

        private void serversCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeSettings();
            respLbl.Text = "Ready";
        }

        private void colorBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                colorBtn.Text = string.Format("Color: {0},{1},{2}", colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                colorPrev.BackColor = colorDialog1.Color;
            }
            respLbl.Text = "Ready";
        }

        private void adToggle_CheckedChanged(object sender, EventArgs e)
        {
            respLbl.Text = "Ready";
        }

        private void tsToggle_CheckedChanged(object sender, EventArgs e)
        {
            respLbl.Text = "Ready";
        }

        private void veriToggle_CheckedChanged(object sender, EventArgs e)
        {
            respLbl.Text = "Ready";
        }

        private void verRoleCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            respLbl.Text = "Ready";
        }

        private void applyConfigBtn_Click(object sender, EventArgs e)
        {
            SocketGuild guild = guilds[serversCBox.SelectedIndex];
            config = ServerConfigs.GetConfig(guild);
            config.Prefix = prefixTxt.Text;
            SocketVoiceChannel vc = voiceChannels.ToArray()[afkChanCBox.SelectedIndex];
            config.AFKChannelName = vc.Name;
            config.AFKChannelID = vc.Id;
            config.RequiresVerification = veriToggle.Checked;
            if (veriToggle.Checked)
                config.VerificationRoleID = roles.ToArray()[verRoleCBox.SelectedIndex].Id;
            config.AFKTimeout = int.Parse(afkTimeCBox.SelectedItem.ToString()) * 60;
            config.EmbedColorRed = colorPrev.BackColor.R;
            config.EmbedColorGreen = colorPrev.BackColor.G;
            config.EmbedColorBlue = colorPrev.BackColor.B;
            config.TimeStamp = tsToggle.Checked;
            config.AllowAdvertising = adToggle.Checked;
            config.AntiSpamWarn = int.Parse(spamWarnTxt.Text);
            config.AntiSpamThreshold = int.Parse(spamThresholdTxt.Text);
            config.AntiSpamTime = double.Parse(spamTimeTxt.Text);
            config.FooterText = footerTxt.Text;
            config.NewUserMessage = newUsrMsgToggle.Checked;
            SocketTextChannel tc = textChannels.ToArray()[afkChanCBox.SelectedIndex];
            config.NewUserChannel = textChannels.ToArray()[newUsrChanCBox.SelectedIndex].Id;
            config.BlockMentionEveryone = eMentionTog.Checked;
            ServerConfigs.SaveAccounts();
            respLbl.Text = "Server Settings Updated!";
        }

        private void sendMsgBtn_Click(object sender, EventArgs e)
        {
            SocketTextChannel channel = textChannels.ToArray()[textChannelCBox.SelectedIndex];
            if(!embedChk.Checked)
                channel.SendMessageAsync(messageTxt.Text);
            else
            {
                ServerConfig config = ServerConfigs.GetConfig(channel.Guild);
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithColor(config.EmbedColorRed, config.EmbedColorGreen, config.EmbedColorBlue);
                if(titleTxt.Text != "")
                    embed.WithTitle(titleTxt.Text);
                if (config.FooterText != "")
                    embed.WithFooter(config.FooterText);
                    embed.WithDescription(messageTxt.Text);
                if (config.TimeStamp)
                    embed.WithCurrentTimestamp();
                embed.WithThumbnailUrl(config.IconURL);

                //Console.WriteLine(DateTime.Now + " | " + title + " |\n" + msg);
                Console.WriteLine(DateTime.Now + " | Sent server message using GUI in " + channel.Guild.Name + "/" + channel.Name);
                channel.SendMessageAsync("", false, embed.Build());
            }
            respLbl.Text = "Ready";
        }

        private void applyStatusBtn_Click(object sender, EventArgs e)
        {
            Config.botconf.botStatus = botStatusTxt.Text;
            Config.Save();
            Global.Client.SetGameAsync(botStatusTxt.Text);
        }
        private void mentionBtn_Click(object sender, EventArgs e)
        {
            messageTxt.Text += users.ToArray()[usersCBox.SelectedIndex].Mention;
        }
    }
}
