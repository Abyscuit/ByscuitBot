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
        public static bool shutDown = false;

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public static Form1 form;
        public static Thread t = new Thread(m => 
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            form.FormClosing += Form_FormClosing;
            try
            {
                Application.Run(form);
            }
            catch(Exception ex)
            {
                string text = DateTime.Now + " | EXCEPTION: " + ex.Message;
                Console.WriteLine(text);
                Log.AddTextToLog(text);
            }
        });

        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("GUI cannot be closed while bot is running!");
            e.Cancel = true;
        }
        public async Task StartAsync()
        {
            Console.WriteLine(Utilities.getAlert("abyscuit"));
            await consolePrint(Utilities.getAlert("configTxt"));
            if (Config.botconf.token == "" || Config.botconf.token == null)
            {
                await consolePrint(Utilities.getAlert("configMissing"));
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            if (Config.botconf.CMC_API_KEY == "" || Config.botconf.CMC_API_KEY == null)
            {
                await consolePrint("Missing CoinMarketCap API Key...");
                await consolePrint("CoinMarketCap Commands will not work.");
            }
            if (Config.botconf.ETH_SCAN_API_KEY == "" || Config.botconf.ETH_SCAN_API_KEY == null)
            {
                await consolePrint("Missing EtherScan API Key...");
                await consolePrint("EtherScan Commands will not work.");
            }
            if (Config.botconf.STEAM_API_KEY == "" || Config.botconf.STEAM_API_KEY == null)
            {
                await consolePrint("Missing Steam API Key...");
                await consolePrint("Steam Commands will not work.");
            }
            if (Config.botconf.TWITCH_APi_KEY == "" || Config.botconf.TWITCH_APi_KEY == null)
            {
                await consolePrint("Missing Twitch API Key...");
                await consolePrint("Twitch Commands will not work.");
            }
            await SetupClient();
            await Task.Delay(-1);
        }
        delegate void formCloseCallback();
        public async Task SetupClient()
        {
            //if (t.IsAlive)
            {
                //if(form.InvokeRequired)
                {
                    //formCloseCallback formCallBack = new formCloseCallback(form.Close);
                    //form.Invoke(formCallBack);
                }
                //else form.Close();
            }
            
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Debug //change log level if you want
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
            await consolePrint(msg.Message);
            if (msg.Message == "Failed to resume previous session")
            {
                try
                {
                    await consolePrint("Might need to restart client...");
                }
                catch (Exception ex)
                {
                    await consolePrint(ex.Message);
                }
            }
        }
        public async Task consolePrint(string msg)
        {
            string text = DateTime.Now + " | " + msg;
            Console.WriteLine(text);
            Log.AddTextToLog(text);
        }
    }
}
