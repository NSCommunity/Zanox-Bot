using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Webhook;
using Discord.Commands;
using Discord.WebSocket;
using ZanoxDiscordBot.Core.UserAccounts;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using NReco.ImageGenerator;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using PastebinAPI;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ZanoxDiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private bool UserIsZanoxAdmin(SocketGuildUser user)
        {
            return (Program.ZanoxAdmins.Contains(user.Id));
        }

        [Command("z!help")]
        public async Task Help()
        {
            try
            {
                //Let user know we've sent the help information to their DMs
                var embed = new EmbedBuilder();
                embed.WithTitle($":mailbox_with_mail: The help message has been send to you in dms!");
                embed.WithDescription(" ");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithCurrentTimestamp();
                await Context.Channel.SendMessageAsync(" ", false, embed.Build());

                var message = new EmbedBuilder();
                //Title
                embed.WithTitle($"Zanox Bot Help");
                embed.WithDescription("Commands for Zanox");

                //Fun Commands
                embed.AddField($"⠀", "⠀");
                embed.AddField($"Fun Commands!", "fun commands for Zanox");
                embed.AddField($"z!8ball", $"Chooses between an object | to seperate (!8ball {Context.User.Username}| Zanox Bot)");
                embed.AddField($"z!gotcha {Context.User.Username}", $"Send the tagged person a little suprise!");

                //Reputation
                embed.AddField($"⠀", "⠀");
                embed.AddField($"Reputation", "reputation commands for Zanox");
                embed.AddField($"+rep {Context.User.Username} reason", $"Adds a reputation point to the tagged member.");
                embed.AddField($"-rep {Context.User.Username} reason", $"Removes a reputation point to the tagged member.");
                embed.AddField($"z!rep {Context.User.Username} reason", $"Check the amounts of rep points a person has.");

                //Leveling
                embed.AddField($"⠀", "⠀");
                embed.AddField($"Levels", $"Level commands for Zanox");
                embed.AddField($"z!stats {Context.User.Username}", $"Check the Level and XP of a member.");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithCurrentTimestamp();
                embed.WithFooter(Context.User.GetAvatarUrl());

                var react = await Context.User.SendMessageAsync("", false, embed.Build());

                var yes = new Emoji("✅");
                react.AddReactionAsync(yes);

                var no = new Emoji("❌");
                react.AddReactionAsync(no);
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!say")]
        public async Task Say([Remainder]string message)
        {
            try
            {
                if (!message.ToLower().Contains("@everyone"))
                {
                    var u = Context.User as SocketGuildUser;
                    await Context.Message.DeleteAsync();
                    await Context.Channel.SendMessageAsync(message);
                }
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

        private bool UserIsZanox(SocketGuildUser user)
        {
            string targetRoleName = "ZanoxAdmin";
            var resault = from r in user.Guild.Roles
                          where r.Name == targetRoleName
                          select r.Id;
            ulong roleID = resault.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        [Command("z!discord")]
        public async Task Discord()
        {
            try
            {
                await Context.Channel.SendMessageAsync("Discord: https://discord.gg/W6WyVf4");
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!weather")]
        public async Task Weather([Remainder]string city)
        {
            await Task.Delay(0);
            try
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle("Zanox Weather");
                embed.WithImageUrl($"https://www.countryflags.io/{getStringFromUrl($"http://zanoxhosting.ga/api/weather/country.php?q={city}")}/flat/64.png");
                embed.AddInlineField(
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/temperature.php?q={city}&u=C")
                    + "℃",
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/city.php?q={city}")
                    + ", " +
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/country.php?q={city}"));
                embed.AddInlineField("Weather Status",
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/weather.php?q={city}"));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!welcome")]
        public async Task Test()
        {
            try
            {
                var userImg = @"https://cdn.discordapp.com/avatars/" + Context.User.Id + @"/" + Context.User.AvatarId + @".png".Replace(" ", "%20");
                string html = String.Format($"<html><head> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/css/bootstrap.min.css\"></head><body style=\"overflow:hidden;background-image:url(&quot;http://92.244.209.118/DMNHosting/api/discord/welcomeAssets/img/eS4IxK3.png&quot;);\"> <div style=\"display:table;position:absolute;top:0;left:0;height:100%;width:100%;\"> <div style=\"display:table-cell;vertical-align:middle;\"> <div style=\"margin-left:auto;margin-right:auto;text-align:center;\"><img src=\"{userImg}\" style=\"height:125px;width:125px;\"><div></div>\" <div style=\"color:#FFF;\"><h1 style=\"font-size:25px;\"><b>Welcome to {Context.Guild.Name},</b></h1> <h1 style=\"font-size:15px;\"><b>{Context.User.Username}</b></h1> </div></div></div></div><script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/js/bootstrap.bundle.min.js\"></script></body></html>");

                var converter = new HtmlToImageConverter { Width = 500, Height = 250 };
                var jpgBytes = converter.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Png);
                await Context.Guild.DefaultChannel.SendMessageAsync("test");
                await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), $"Welcome {Context.User.Username}.png");
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!tts")]
        [RequireUserPermission(GuildPermission.SendTTSMessages)]
        public async Task TheArrr([Remainder]string input)
        {
            var delete = await Context.Channel.SendMessageAsync(input, true);
            delete.DeleteAsync();
        }

        [Command("z!f")]
        public async Task ffff([Remainder]string input)
        {
            while (true)
                Context.Channel.SendMessageAsync("f");
        }

        public static async Task ExceptionAlert(SocketCommandContext Context, Exception e)
        {
            try
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                await Context.Channel.SendMessageAsync("There was an exception! I've told my creators about all the details!");
                var properties = e.GetType()
                            .GetProperties();
                var fields = properties
                                 .Select(property => new { property.Name, Value = property.GetValue(e, null) })
                                 .Select(x => String.Format("{0} = {1}", x.Name, x.Value != null ? x.Value.ToString() : String.Empty));
                var ZanoxChannel = (Context.Client.GetChannel(532641855083380747) as ISocketMessageChannel);

                string output = "```";

                output += Context.User.Username + "#" + Context.User.Discriminator + " is having some issues!\n";
                output += "Line: " + line + "\n";
                await ZanoxChannel.SendMessageAsync(output + "Command issued: " + Context.Message.Content + "```");
                foreach (string part in SplitInParts(String.Join("\n", fields), 1750))
                    await ZanoxChannel.SendMessageAsync(part);
            }
            catch { }
        }

        [Command("z!pastebin")]
        public async Task pastebinHelp()
        {
            Context.Channel.SendMessageAsync("");
        }

        [Command("z!pastebin")]
        public async Task paste(string cmd, [Remainder]string input)
        {
            var account = UserAccounts.GetAccount(Context.User);
            if (cmd.Contains("paste"))
            {
                if (account.pastebinDev != "undefined")
                {
                    Context.Channel.SendMessageAsync("");
                }
                else
                {

                }
            }
            else
            {
                if (input.Contains(""))
                {

                }
            }
        }

        public string getPrefix(SocketCommandContext cc)
        {
            var account = UserAccounts.GetOrCreateAccount(cc.User.Id);
            return account.prefix;
        }

        public static String getStringFromUrl(string Url)
        {
            using (WebClient client = new WebClient())
            {
                string result = client.DownloadString(Url);
                return result;
            }
        }

        public static IEnumerable<String> SplitInParts(string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static void saveToFile(string filePath, string fileContent)
        {
            File.WriteAllText(filePath, fileContent);
        }

        public static string stringFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public static string stringFromAPI(string url, string key, string value)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(key, value);
                JObject json = JObject.Parse(client.DownloadString(url));
                return json.ToString(Formatting.None);
            }
        }
    }
}