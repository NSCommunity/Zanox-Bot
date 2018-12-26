﻿using Discord.WebSocket;
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

namespace ZanoxDiscordBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task antiBrag(SocketMessage msg)
        {
            await Task.Delay(0);
            if (false)
            {
                await msg.DeleteAsync();
                var temp = await msg.Channel.SendMessageAsync("anti Brag has stepped in!");
                await Task.Delay(1000);
                await temp.DeleteAsync();
            }
        }

        public async Task zConsole()
        {
            await Task.Delay(0);
            while (true)
            {
                await Task.Delay(0);
                string cmd = Console.ReadLine();

                if (cmd.ToLower() == "botinfo")
                {
                    Console.WriteLine("Guild Count " + _client.Guilds.Count());
                }

                if (cmd.ToLower() == "ping")
                {
                    Console.WriteLine(_client.Latency);
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
                }

                if (cmd.ToLower() == "changelog push")
                {
                    var embed = new EmbedBuilder();
                    for (int i = 0; i < UpdateLog.Count / 2; i++)
                    {
                        embed.WithTitle("Updates");
                        embed.AddField(UpdateLog[i * 2], UpdateLog[i * 2 + 1]);
                    }
                    var ender = (_client.GetChannel(525301353816391700) as SocketTextChannel);
                    await ender.SendMessageAsync("", false, embed.Build());
                }

                if (cmd.ToLower() == "changelog clear")
                {
                    UpdateLog.Clear();
                }

                if (cmd.ToLower().Contains("changelog add"))
                {
                    cmd = cmd.Remove(0, 14);
                    var changelogAdd = cmd.Split('!').ToList();
                    if (changelogAdd.Count % 2 == 0) { foreach (string line in changelogAdd) { UpdateLog.Add(line); } }
                }
            }
        }

        public static List<string> UpdateLog;
        
        public static List<IUserMessage> reactionWatch;

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.UserJoined += AnnounceJoined;
            _client.UserLeft += AnnounceLeave;
            _client.Log += Log;
            Task.Run(zConsole);
            _client.MessageReceived += antiBrag;
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
            string html = String.Format($"<html><head> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/css/bootstrap.min.css\"></head><body style=\"overflow:hidden;background-image:url(&quot;http://92.244.209.118/DMNHosting/api/discord/welcomeAssets/img/eS4IxK3.png&quot;);\"> <div style=\"display:table;position:absolute;top:0;left:0;height:100%;width:100%;\"> <div style=\"display:table-cell;vertical-align:middle;\"> <div style=\"margin-left:auto;margin-right:auto;text-align:center;\"><img src=\"{userImg}\" style=\"height:125px;width:125px;\"><div></div>\" <div style=\"color:#FFF;\"><h1 style=\"font-size:25px;\"><b>Welcome to {user.Guild.Name},</b></h1> <h1 style=\"font-size:15px;\"><b>{user.Username}</b></h1> </div></div></div></div><script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/js/bootstrap.bundle.min.js\"></script></body></html>");

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
            string html = String.Format($"<html><head> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/css/bootstrap.min.css\"></head><body style=\"overflow:hidden;background-image:url(&quot;http://92.244.209.118/DMNHosting/api/discord/welcomeAssets/img/eS4IxK3.png&quot;);\"> <div style=\"display:table;position:absolute;top:0;left:0;height:100%;width:100%;\"> <div style=\"display:table-cell;vertical-align:middle;\"> <div style=\"margin-left:auto;margin-right:auto;text-align:center;\"><img src=\"{userImg}\" style=\"height:125px;width:125px;\"><div></div>\" <div style=\"color:#FFF;\"><h1 style=\"font-size:25px;\"><b>Farewell,</b></h1> <h1 style=\"font-size:15px;\"><b>{user.Username}</b></h1> </div></div></div></div><script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/js/bootstrap.bundle.min.js\"></script></body></html>");

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

        public async Task SetGame()
        {
            await _client.SetGameAsync("z!help");
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