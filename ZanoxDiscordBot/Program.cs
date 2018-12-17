﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;

namespace ZanoxDiscordBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
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

            public async Task SetGame()
        {
            await _client.SetGameAsync("z!help");
        }

        private async Task JoinedGuild(SocketGuild arg)
        {
            Console.WriteLine($"Zanox has joined the discord {arg.Name}!");
        }

        private async Task LeftGuild(SocketGuild arg)
        {
            Console.WriteLine($"Zanox has left the discord {arg.Name}!");
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}