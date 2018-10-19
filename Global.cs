using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;

namespace DabSquadBot
{
    public class Global
    {
        static string errorLogFilePath = @"..\Data\ErrorLog.txt";
        public static string filePath = @"..\Data\";
        public static JSONHelper JSONHelper = new JSONHelper();
        public static Data Data;
        public static List<Color> Colors = new List<Color>
                {
                    Color.Blue,
                    Color.DarkBlue,
                    Color.DarkerGrey,
                    Color.DarkGreen,
                    Color.DarkGrey,
                    Color.DarkMagenta,
                    Color.DarkOrange,
                    Color.DarkPurple,
                    Color.DarkRed,
                    Color.DarkTeal,
                    Color.Gold,
                    Color.Green,
                    Color.LighterGrey,
                    Color.LightGrey,
                    Color.LightOrange,
                    Color.Magenta,
                    Color.Orange,
                    Color.Purple,
                    Color.Red,
                    Color.Teal
                };

        public static Task InitGlobalVars()
        {
            try
            {
                if (!File.Exists(errorLogFilePath))
                {
                    Directory.CreateDirectory(@"..\Data\");
                    File.CreateText(errorLogFilePath);
                }

                if (!File.Exists(filePath + "Data.json"))
                {
                    JSONHelper.JsonSerialize(new Data());
                }

                Data = JSONHelper.JsonDeserialize();

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        public static Embed LogError(Exception ex, SocketCommandContext context = null)
        {
            Console.WriteLine(ex.Message);
            string User = null;
            string UserMessage = null;
            if (context != null)
            {
                User = context.User.Username;
                UserMessage = context.Message.Content;
            }

            string Message = ex.Message;
            string StackTrace = ex.StackTrace;

            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Color = Color.Red,
                Title = Message,
                Footer = new EmbedFooterBuilder
                {
                    Text = DateTime.UtcNow.ToString() + " UTC"
                },
            };

            File.AppendAllLines(errorLogFilePath, new string[] { $"({User}) {{{UserMessage}}}", $"{DateTime.Now.ToString()}: {ex.Message}", ex.StackTrace });

            Embed embed = embedBuilder.Build();
            return embed;
        }
    }
}
