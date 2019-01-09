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
    public class Networking : ModuleBase<SocketCommandContext>
    {
        [Command("z!scanIP")]
        public async Task scan([Remainder]string input)
        {
            Context.Channel.SendMessageAsync("This will take a long time, but I'll send you the results in DMs when it's done!");
            string output = "Your IPScan is completed! ```";
            for (int i = 0; i > 65535; i++)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    try
                    {
                        tcpClient.Connect(input, i);
                        output += $"Port {i} is open\n";

                    }
                    catch (Exception)
                    {
                        output += $"Port {i} is closed\n";
                    }
                }
            }
            Context.User.SendMessageAsync(output + "```");
        }
    }
}
