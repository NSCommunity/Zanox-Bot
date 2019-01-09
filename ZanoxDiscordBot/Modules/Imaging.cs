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
    public class Imaging : ModuleBase<SocketCommandContext>
    {
        [Command("z!god")]
        public async Task godBecauseEnderWantMeToMakeThis()
        {
            var godImg = new EmbedBuilder();
            godImg.ImageUrl = "https://assetsnffrgf-a.akamaihd.net/assets/m/502012120/univ/art/502012120_univ_sqr_xl.jpg";
            Context.Channel.SendMessageAsync("", false, godImg.Build());
        }

        [Command("z!Dog")]
        [RequireUserPermission(GuildPermission.AttachFiles)]
        public async Task dog()
        {
            string json = Misc.getStringFromUrl(@"https://dog.ceo/api/breeds/image/random");
            json = json.Remove(0, 31).Remove(json.Length - 33, 2).Replace(@"\", "");
            var dogUrl = new Uri(json);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(json), @"dog." + json.Remove(0, json.Length - 3));
            }
            await Context.Channel.SendFileAsync("dog." + json.Remove(0, json.Length - 3));
        }
    }
}
