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
    public class Fortnite : ModuleBase<SocketCommandContext>
    {
        [Command("z!fStats")]
        public async Task fortniteStats([Remainder]string input)
        {
            if (Context.Guild.Id != 525056817399726102)
                foreach (string part in Misc.SplitInParts(Misc.stringFromAPI("https://api.fortnitetracker.com/v1/profile/pc/" + input, "TRN-Api-Key", Misc.stringFromFile("Resources/fortnite.token")), 1990))
                {
                    await Context.Channel.SendMessageAsync("```" + part + "```");
                }
        }
    }
}
