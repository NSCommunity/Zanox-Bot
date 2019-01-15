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
        [Command("z!UInt64")]
        public async Task UInt64inf()
        {
            string x = "UInt64 (ULong) Max Value: ";
            x += UInt64.MaxValue;
            x += "\nULong Min Value: ";
            x += UInt64.MinValue;
            await Context.Channel.SendMessageAsync(x);
        }

        [Command("z!UInt32")]
        public async Task UInt32inf()
        {
            string x = "UInt32 Max Value: ";
            x += UInt32.MaxValue;
            x += "\nULong Min Value: ";
            x += UInt32.MinValue;
            await Context.Channel.SendMessageAsync(x);
        }

        [Command("z!UInt16")]
        public async Task UInt16inf()
        {
            string x = "UInt16 Max Value: ";
            x += UInt16.MaxValue;
            x += "\nULong Min Value: ";
            x += UInt16.MinValue;
            await Context.Channel.SendMessageAsync(x);
        }

        [Command("z!Int64")]
        public async Task Int64inf()
        {
            string x = "UInt64 (ULong) Max Value: ";
            x += Int64.MaxValue;
            x += "\nULong Min Value: ";
            x += Int64.MinValue;
            await Context.Channel.SendMessageAsync(x);
        }

        [Command("z!Int32")]
        public async Task Int32inf()
        {
            string x = "UInt32 Max Value: ";
            x += Int32.MaxValue;
            x += "\nULong Min Value: ";
            x += Int32.MinValue;
            await Context.Channel.SendMessageAsync(x);
        }

        [Command("z!Int16")]
        public async Task Int16inf()
        {
            string x = "UInt16 Max Value: ";
            x += Int16.MaxValue;
            x += "\nULong Min Value: ";
            x += Int16.MinValue;
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
