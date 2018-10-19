using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Net.Providers.WS4Net;
using System.Collections.Generic;
using System.IO;

namespace DabSquadBot
{
    class Program
    {
        private readonly DiscordSocketClient client;

        // Keep the CommandService and IServiceCollection around for use with commands.
        private IServiceCollection _map = new ServiceCollection();
        private CommandService _commands = new CommandService();

        // Program entry point
        static void Main(string[] args)
        {
            // Call the Program constructor, followed by the 
            // MainAsync method and wait until it finishes (which should be never).
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private Program()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
#if (!DEBUG)
                WebSocketProvider = WS4NetProvider.Instance
#endif
            });
        }

        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.
        private static Task Logger(LogMessage message)
        {
            var cc = Console.ForegroundColor;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
            Console.ForegroundColor = cc;

            // If you get an error saying 'CompletedTask' doesn't exist,
            // your project is targeting .NET 4.5.2 or lower. You'll need
            // to adjust your project's target framework to 4.6 or higher
            // (instructions for this are easily Googled).
            // If you *need* to run on .NET 4.5 for compat/other reasons,
            // the alternative is to 'return Task.Delay(0);' instead.
            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            try
            {
                client.Log += Logger;
                client.Ready += Ready;
                client.MessageReceived += MessageReceived;

                //Bot token goes here. Your list of bots can be found here: https://discordapp.com/developers/applications/
                string token = ""; // Remember to keep this private!
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                // Block this task until the program is closed.
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Task Ready()
        {
            Global.InitGlobalVars();
            return Task.CompletedTask;
        }

        string previousImage = "";
        string currentImage = "";

        private async Task MessageReceived(SocketMessage msg)
        {
            try
            {
                if (msg.Content.ToLower().Contains("dab"))
                {
                    Random rnd = new Random();
                    Global.Data = Global.JSONHelper.JsonDeserialize();
                    Global.Data.DabImages.Remove(previousImage);
                    currentImage = Global.Data.DabImages[rnd.Next(Global.Data.DabImages.Count - 1)];

                    await msg.Channel.SendMessageAsync("", embed: new EmbedBuilder() { ImageUrl = currentImage, Color = Global.Colors[rnd.Next(Global.Colors.Count - 1)] }.Build());

                    Global.Data.DabImages.Add(previousImage);
                    previousImage = currentImage;
                }
            }
            catch(Exception ex)
            {
                Global.LogError(ex);
            }
        }
    }
}
