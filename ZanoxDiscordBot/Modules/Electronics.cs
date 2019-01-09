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
    public class Electronics : ModuleBase<SocketCommandContext>
    {
        [Command("z!Lumens")]
        public async Task Lumy([Remainder]string input)
        {
            await Task.Delay(0);
            var inputList = input.Split('!').ToList();
            decimal total = Convert.ToInt32(inputList[0]) * Convert.ToInt32(inputList[1]);
            await Context.Channel.SendMessageAsync(Convert.ToString(total));
        }

        [Command("z!ParallelRes")]
        public async Task PRes([Remainder]string input)
        {
            await Task.Delay(0);
            var inputList = input.Split('!').ToList();
            decimal total = 0;
            foreach (string lines in inputList)
            {
                total += (1 / Convert.ToDecimal(lines));
            }
            await Context.Channel.SendMessageAsync(Convert.ToString(1 / total));
        }
    }
}
