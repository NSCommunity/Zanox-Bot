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
using AkiNet;
//using AkiNet.Entities;
//using AkiNet.Exceptions;

namespace ZanoxDiscordBot.Modules
{
    public class Akinator : ModuleBase<SocketCommandContext>
    {
        [Command("z!akinator")]
        public async Task akinator()
        {
            Client akinate = Client.StartGame();
            Context.Channel.SendMessageAsync(akinate.Question.ToString());
        }
    }
}
