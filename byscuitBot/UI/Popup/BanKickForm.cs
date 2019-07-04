using byscuitBot.Core;
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

namespace byscuitBot.UI.Popup
{
    public partial class BanKickForm : MetroFramework.Forms.MetroForm
    {
        string username;
        SocketGuildUser user;
        public BanKickForm(string Username, SocketGuildUser User)
        {
            username = Username;
            user = User;
            InitializeComponent();
        }

        int[] banDays = {0, 1, 3, 7, 14, 30, 99 };
        private void BanKickForm_Load(object sender, EventArgs e)
        {
            kickUsrLbl.Text = username + "(" + user.Id + ")";
            //add ban times
            for(int i =0;i<banDays.Length;i++)
            {
                string msg = (i == 0) ? "Kick only" : "Ban for {0}";
                if (i == banDays.Length - 1) msg = string.Format(msg, "Lifetime");
                else if (i > 0) msg = string.Format(msg, (i == 1) ? "24 Hours" : banDays[i] + " Days");
                banCBox.Items.Add(msg);
            }
            for (int i = 0; i <= 7; i++)
            {
                string msg = (i == 0) ? "Don't Delete Any Messages" :
                    string.Format("Delete All Messages For Past {0} Day(s)", i);
                eraseMsgCBox.Items.Add(msg);
            }
            banCBox.SelectedIndex = 0;
            eraseMsgCBox.SelectedIndex = 0;
        }

        private void BanCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (banCBox.SelectedIndex > 0) eraseMsgCBox.Visible = true;
            else eraseMsgCBox.Visible = false;
        }

        private async void KickBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (banCBox.SelectedIndex == 0) await user.KickAsync(reasonTxt.Text);
                else
                {
                    //Add save time for bans in data somewhere
                    await user.BanAsync(eraseMsgCBox.SelectedIndex, reasonTxt.Text);
                }
            }
            catch (Exception ex)
            {
                string text = DateTime.Now + " | EXCEPTION: " + ex.Message;
                Console.WriteLine(text);
                Log.AddTextToLog(text);
            }
        }
    }
}
