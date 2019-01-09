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
    public class Programming : ModuleBase<SocketCommandContext>
    {
        [Command("z!ULong")]
        public async Task ulonginf()
        {
            string x = "ULong Max Value: ";
            x += ulong.MaxValue;
            x += "\nULong Min Value: ";
            x += ulong.MinValue;
            await Context.Channel.SendMessageAsync(x);
        }

        [Command("z!unix")]
        public async Task unixTime(SocketUser target = null)
        {
            try
            {
                await Task.Delay(0);
                string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
                unixT = unixT.Remove(10, unixT.Length - 10);
                await Context.Channel.SendMessageAsync(unixT);
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!contentFromUrl")]
        public async Task getcont([Remainder]string input)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    Context.Channel.SendMessageAsync(System.Text.Encoding.Default.GetString(client.DownloadData(input)));
                }
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!throwException")]
        public async Task ThrowException()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }
    }
}
