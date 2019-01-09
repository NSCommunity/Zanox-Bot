using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ZanoxDiscordBot.Core.LevelingSystem;
using System.IO;

namespace ZanoxDiscordBot
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;

            bool getInvites = false;
            if (getInvites)
            {
                var invites = await (msg.Author as SocketGuildUser).Guild.GetInvitesAsync();

                string invite = invites.Select(x => x.Url).FirstOrDefault();

                string contents = File.ReadAllText("msgLog.txt");
                if (!contents.Contains(invite))
                {
                    File.AppendAllText("msgLog.txt", invite + "\n");
                }
            }

            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            Leveling.UserSentMessageAsync((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);

            int argPos = 0;
            
            if(msg.HasStringPrefix("", ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                Task.Run(() => _service.ExecuteAsync(context, argPos));
            }
        }
    }
}
