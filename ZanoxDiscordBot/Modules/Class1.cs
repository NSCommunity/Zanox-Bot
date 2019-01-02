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

namespace ZanoxDiscordBot.Modules
{
    class Class1 : ModuleBase<SocketCommandContext>
    {
        [Command("z!testie")]
        public async Task testiee()
        {
            Console.WriteLine("fffffffffffffffffff");
            await Context.Channel.SendMessageAsync("Testie Testie");
        }
    }
}
