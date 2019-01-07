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
using System.Threading.Tasks;

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

        public async Task StartAsync()
        {
            Console.WriteLine(Utilities.getAlert("abyscuit"));
            Console.WriteLine(Utilities.getAlert("configTxt"));
            if (Config.botconf.token == "" || Config.botconf.token == null) return;
            client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = Discord.LogSeverity.Verbose
            });
            
            client.Log += Client_Log;
            client.Ready += RepeatingTimer.StartTimer;
            client.ReactionAdded += Client_ReactionAdded;
            await client.SetGameAsync("@Me help for CMDs");
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
            if(reaction.MessageId == Global.MessageIdToTrack)
            {

                string[] emojies = new string[Global.emojies.Length];
               // string emoji = Global.emojies[Global.selectedEmoji];
                for (int i = 0; i < emojies.Length; i++)
                {
                    emojies[i] = Global.emojies[i];
                    if (reaction.Emote.Name == emojies[i])
                    {
                        string message = "__" + reaction.User.Value.Username + "__ reacted with " + emojies[i];
                        var embed = new EmbedBuilder();
                        embed.WithTitle("Reacted");
                        embed.WithDescription(message);
                        embed.WithColor(100, 150, 255);
                        embed.WithFooter("Created by Abyscuit");

                        await channel.SendMessageAsync("", false, embed.Build());
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
