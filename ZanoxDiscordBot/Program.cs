using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord.Net;
using Discord.Commands;
using Discord.Rest;
using Discord.Webhook;
using NReco.ImageGenerator;
using ZanoxDiscordBot.Core.UserAccounts;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace ZanoxDiscordBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task startAPI()
        {
            bool apiStatus = false;
            if (apiStatus)
            {
                Console.WriteLine("Starting API");
                var config = new HttpSelfHostConfiguration("http://localhost:8080");

                config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

                using (HttpSelfHostServer server = new HttpSelfHostServer(config))
                {
                    server.OpenAsync().Wait();
                    Console.WriteLine("API Started");
                }
            }
        }

        public async Task additionalHandler(SocketMessage msg)
        {
            Console.WriteLine(msg);
            //If we want to handle messages async...
        }

        public async Task zConsole()
        {
            await Task.Delay(0);
            List<string> UpdateLog = new List<string>();
            while (true)
            {
                await Task.Delay(0);
                string cmd = Console.ReadLine();

                if (cmd.ToLower() == "botinfo")
                {
                    Console.WriteLine("Guild Count " + _client.Guilds.Count());
                    continue;
                }

                if (cmd.ToLower() == "ping")
                {
                    Console.WriteLine(_client.Latency);
                    continue;
                }

                if (cmd.ToLower() == "guilds")
                {
                    List<SocketGuild> GList = _client.Guilds.ToList();
                    int totalMembers = 0;
                    foreach (SocketGuild i in GList)
                    {
                        Console.WriteLine(i.Name + " (" + i.Users.Count().ToString() + " Memebers)");
                        totalMembers+= i.Users.Count();
                    }
                    Console.WriteLine("That's a total of " + GList.Count() + " guilds with " + totalMembers + " members");
                    continue;
                }

                if (cmd.ToLower() == "invites")
                {
                    Console.WriteLine("This will a while because of discord bot limitations, I will save them to a file, and tell you about it when it's done!");
                    List<SocketGuild> GList = _client.Guilds.ToList();
                    int totalMembers = 0;
                    TextWriter tw = new StreamWriter("invites.txt", true);
                    foreach (SocketGuild i in GList)
                    {
                        var invites = await i.GetInvitesAsync();
                        Console.WriteLine(i.Name + " " + invites.Select(x => x.Url).FirstOrDefault());
                        tw.WriteLine(i.Name + " " + invites.Select(x => x.Url).FirstOrDefault());
                    }
                    continue;
                }

                if (cmd.ToLower() == "changelog push")
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("Updates");
                    for (int i = 0; i < UpdateLog.Count / 2; i++)
                    {
                        embed.AddField(UpdateLog[i * 2], UpdateLog[i * 2 + 1]);
                    }
                    var ender = (_client.GetChannel(525301353816391700) as SocketTextChannel);
                    await ender.SendMessageAsync("", false, embed.Build());
                    continue;
                }

                if (cmd.ToLower() == "changelog clear")
                {
                    UpdateLog.Clear();
                    continue;
                }

                if (cmd.ToLower().Contains("changelog add"))
                {
                    cmd = cmd.Remove(0, 14);
                    var changelogAdd = cmd.Split('¤').ToList();
                    if (changelogAdd.Count % 2 == 0)
                    {
                        foreach (string line in changelogAdd)
                        {
                            Console.WriteLine("Added " + line);
                            UpdateLog.Add(line);
                        }
                    }
                    continue;
                }

                if (cmd.ToLower().Contains("pardon"))
                {
                    var input = cmd.Split(' ');
                    var guild = _client.GetGuild(Convert.ToUInt64(input[1]));
                    var user = _client.GetUser(Convert.ToUInt64(input[2]));
                    Console.Write("\nIn " + guild.Name);
                    Console.WriteLine(" unban " + user.Username);
                    await guild.RemoveBanAsync(user);
                    Console.WriteLine("Done!");
                }

                if (cmd.ToLower().Contains("getroles"))
                {
                    var input = cmd.Split(' ');
                    var guild = _client.GetGuild(Convert.ToUInt64(input[1]));
                    var roles = guild.Roles;
                    foreach (SocketRole role in roles)
                    {
                        Console.WriteLine(role.Name + " " + role.Id);
                    }
                }

                if (cmd.ToLower().Contains("addrole"))
                {
                    var input = cmd.Split(' ');
                    var guild = _client.GetGuild(Convert.ToUInt64(input[1]));
                    var user = guild.GetUser(Convert.ToUInt64(input[2]));
                    var role = guild.GetRole(Convert.ToUInt64(input[3]));

                    user.AddRoleAsync(role);
                }

                if (cmd.ToLower().Contains("removerole"))
                {
                    var input = cmd.Split(' ');
                    var guild = _client.GetGuild(Convert.ToUInt64(input[1]));
                    var user = guild.GetUser(Convert.ToUInt64(input[2]));
                    var role = guild.GetRole(Convert.ToUInt64(input[3]));

                    user.RemoveRoleAsync(role);
                }

                if (cmd.ToLower().Contains("ban"))
                {
                    var input = cmd.Split(' ');
                    var guild = _client.GetGuild(Convert.ToUInt64(input[1]));
                    var user = _client.GetUser(Convert.ToUInt64(input[2]));
                    Console.Write("\nIn " + guild.Name);
                    Console.WriteLine(" ban " + user.Username);
                    await guild.AddBanAsync(user);
                    Console.WriteLine("Done!");
                }

                if (cmd.ToLower().Contains("invite"))
                {
                    var input = cmd.Split(' ');
                    var guild = _client.GetGuild(Convert.ToUInt64(input[1]));
                    Console.WriteLine("Getting invite for " + guild.Name);
                    var invites = await guild.GetInvitesAsync();
                    Console.WriteLine(invites.Select(x => x.Url).FirstOrDefault());
                }
            }
        }
        
        public static List<IUserMessage> reactionWatch;

        public static List<UInt64> ZanoxAdmins = new List<UInt64>();

        public async Task StartAsync()
        {

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            ZanoxAdmins.Add(261418273009041408);
            ZanoxAdmins.Add(249474587530625034);

            _client.UserJoined += AnnounceJoined;
            _client.UserLeft += AnnounceLeave;
            _client.Log += Log;
            Task.Run(zConsole);
            _client.MessageReceived += additionalHandler;
            string hiddenToken = File.ReadLines(@"Resources\token.token").First();
            await _client.LoginAsync(TokenType.Bot, hiddenToken);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            _client.Ready += SetGame;
            _client.JoinedGuild += JoinedGuild;
            _client.LeftGuild += LeftGuild;
            await Task.Delay(-1);
        }

        public async Task AnnounceJoined(SocketGuildUser user) //Welcomes the new user
        {
            var account = UserAccounts.GetOrCreateAccount(user.Guild.Id);
            var channel = _client.GetChannel(account.DefaultChannelID) as SocketTextChannel; // Gets the channel to send the message in
            var userImg = @"https://cdn.discordapp.com/avatars/" + user.Id + @"/" + user.AvatarId + @".png".Replace(" ", "%20");
            string html = String.Format($"<html><head> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/css/bootstrap.min.css\"></head><body style=\"overflow:hidden;background-image:url(&quot;http://zanoxhosting.ga/api/discord/welcomeAssets/img/eS4IxK3.png&quot;);\"> <div style=\"display:table;position:absolute;top:0;left:0;height:100%;width:100%;\"> <div style=\"display:table-cell;vertical-align:middle;\"> <div style=\"margin-left:auto;margin-right:auto;text-align:center;\"><img src=\"{userImg}\" style=\"height:125px;width:125px;\"><div></div>\" <div style=\"color:#FFF;\"><h1 style=\"font-size:25px;\"><b>Welcome to {user.Guild.Name},</b></h1> <h1 style=\"font-size:15px;\"><b>{user.Username}</b></h1> </div></div></div></div><script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/js/bootstrap.bundle.min.js\"></script></body></html>");

            var converter = new HtmlToImageConverter { Width = 500, Height = 250 };
            byte[] pngBytes = converter.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Png);
            File.WriteAllBytes($"Welcome {user.Username}.png", pngBytes);
            await channel.SendFileAsync($"Welcome {user.Username}.png", $"Welcome, {user.Mention}");
            await Task.Delay(100);
            try
            {
                File.Delete($"Welcome {user.Username}.png");
            }
            catch { }
        }

        public async Task AnnounceLeave(SocketGuildUser user) //Welcomes the new user
        {
            var account = UserAccounts.GetOrCreateAccount(user.Guild.Id);
            var channel = _client.GetChannel(account.DefaultChannelID) as SocketTextChannel; // Gets the channel to send the message in
            var userImg = @"https://cdn.discordapp.com/avatars/" + user.Id + @"/" + user.AvatarId + @".png".Replace(" ", "%20");
            string html = String.Format($"<html><head> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/css/bootstrap.min.css\"></head><body style=\"overflow:hidden;background-image:url(&quot;http://zanoxhosting.ga/api/discord/welcomeAssets/img/eS4IxK3.png&quot;);\"> <div style=\"display:table;position:absolute;top:0;left:0;height:100%;width:100%;\"> <div style=\"display:table-cell;vertical-align:middle;\"> <div style=\"margin-left:auto;margin-right:auto;text-align:center;\"><img src=\"{userImg}\" style=\"height:125px;width:125px;\"><div></div>\" <div style=\"color:#FFF;\"><h1 style=\"font-size:25px;\"><b>Farewell,</b></h1> <h1 style=\"font-size:15px;\"><b>{user.Username}</b></h1> </div></div></div></div><script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/js/bootstrap.bundle.min.js\"></script></body></html>");

            var converter = new HtmlToImageConverter { Width = 500, Height = 250 };
            byte[] jpgBytes = converter.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Png);
            File.WriteAllBytes($"Farewell {user.Username}.png", jpgBytes);

            await channel.SendFileAsync($"Farewell {user.Username}.png", $"Farewell, {user.Mention}");
            try
            {
                File.Delete($"Farewell {user.Username}.png");
            }
            catch { }
        }

        public List<string> GuildList;

        public async Task asyncLoop()
        {
            while (true)
            {
                _client.SetGameAsync($"z!help");
                await Task.Delay(10000);
                _client.SetGameAsync($"with {memberCount()} players");
                await Task.Delay(2000);
                _client.SetGameAsync($"with {_client.Guilds.Count} guilds");
                await Task.Delay(2000);
            }
        }

        public int memberCount()
        {
            List<SocketGuild> GList = _client.Guilds.ToList();
            int totalMembers = 0;
            foreach (SocketGuild i in GList)
            {
                totalMembers += i.Users.Count();
            }
            return totalMembers;
        }

        public async Task SetGame()
        {
            await _client.SetGameAsync("z!help");
            Task.Run(asyncLoop);
            Task.Run(startAPI);
        }

        private async Task JoinedGuild(SocketGuild arg)
        {
            await Task.Delay(0);
            Console.WriteLine($"Zanox has joined the discord {arg.Name}!");
        }

        private async Task LeftGuild(SocketGuild arg)
        {
            await Task.Delay(0);
            Console.WriteLine($"Zanox has left the discord {arg.Name}!");
        }

        private async Task Log(LogMessage msg)
        {
            await Task.Delay(0);
            Console.WriteLine(msg.Message);
            if (msg.Message.Contains("Connected"))
            {
                GuildList.Add(msg.Message);
            }
        }
    }
}