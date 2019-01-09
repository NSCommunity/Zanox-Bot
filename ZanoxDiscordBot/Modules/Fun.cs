﻿using System;
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
    class Fun : ModuleBase<SocketCommandContext>
    {
        [Command("z!blockGotcha")]
        public async Task GBlock()
        {
            var account = UserAccounts.GetAccount(Context.User);
            if (account.GBlock.Equals(0))
            {
                account.GBlock = 1;
                Context.Channel.SendMessageAsync("Now blocking the Gotcha Command");
            }
            else
            {
                account.GBlock = 0;
                Context.Channel.SendMessageAsync("No longer blocking the Gotcha Command");
            }
            UserAccounts.SaveAccounts();
        }

        [Command("z!8ball")]
        public async Task EightBall([Remainder]string message)
        {
            try
            {
                string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                Random r = new Random();
                string selection = options[r.Next(0, options.Length)];

                var embed = new EmbedBuilder();
                embed.WithTitle(Context.User.Username + " I choose...");
                embed.WithDescription(selection);
                embed.WithColor(new Color(0, 255, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!gotcha")]
        public async Task Gotcha(SocketUser target = null)
        {
            try
            {
                target = target ?? Context.User;

                var account = UserAccounts.GetAccount(Context.User);
                var targetAccount = UserAccounts.GetAccount(target);
                string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
                unixT = unixT.Remove(10, unixT.Length - 10);
                int userCooldown = account.GCooldown; if (Convert.ToInt32(unixT) - 600 > userCooldown || target == Context.User)
                {
                    if (targetAccount.GBlock.Equals(0))
                    {
                        await target.SendMessageAsync($"**{Context.User.Username}** Gotcha!");
                        await target.SendMessageAsync($"⠀⠀⠀⠀⠀⡼⡑⢅⠪⡐⠅⠪⡐⢌⠢⡑⠌⡢⢑⠌⠢⡑⠌⡢⠑⢌⠢⠑⡌⠰⡁⡣⢘⠵⢦⡀\n⠀⠀⠀⠀⢰⢍⠊⠔⠡⡈⠜⡠⠑⠄⢅⠌⡂⠢⡁⢊⠢⠘⢄⠊⢌⠂⢅⢑⢈⠢⡨⠠⡑⠨⠢⡫⡆\n⠀⠀⠀⠀⡗⢅⠑⡁⡑⠠⠊⡀⡑⠌⠐⠄⢊⠐⡨⠀⢅⠊⡠⠊⠠⠊⡠⠂⡁⠢⠐⡐⢈⠂⡑⢄⢙⢇⡀\n⠀⠀⠀⡸⡑⢌⠐⠄⢌⠐⡁⠔⢀⠊⡨⠠⢁⠢⢀⠑⠠⢂⠐⠌⡐⢁⠄⠌⠠⢁⠌⠠⠁⠔⢀⠢⢀⠣⢳⢄\n⠀⠀⢠⠫⡂⠔⢁⠂⠢⠐⡀⢊⠠⠂⡐⢐⠀⡂⢁⠈⠢⠠⡈⠄⠢⡀⢆⢨⢂⠔⡀⢅⠈⠂⠔⢀⠅⡐⢁⠫⣆\n⠀⢀⢏⠪⢀⠊⡠⠈⢄⠡⠐⡀⠢⢈⠠⠂⠨⢀⠂⡁⡊⣐⢄⣳⠕⠯⠚⠓⠛⠚⠶⣄⠅⡡⠈⢄⠐⠠⢁⠊⡜⣥⠀\n⠀⣜⠥⡑⠠⠂⡐⠈⠄⠄⡡⠠⢁⠂⠄⡑⠠⢁⢌⢔⢮⠎⠋⠁⠀⠀⠀⠀⠀⠀⠀⠑⢧⠐⡡⠠⢈⠂⢄⠡⡈⢮⡀\n⠰⣝⢆⠢⠁⠂⠌⠠⠑⡀⢂⠐⠄⡈⡐⢀⠑⢤⡳⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢙⡬⡀⠂⠔⢀⠢⠐⡈⢎⡇\n⢘⢮⡣⡂⠡⢁⠊⠠⠁⠔⢀⠁⠢⠐⡀⢅⠈⡲⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢷⡨⠨⡀⠅⢐⠈⠄⢪⢖\n⠈⡮⡳⡕⡡⢀⠊⠠⠑⠈⢄⠁⡊⢀⠢⠠⢈⠌⠳⡔⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢳⢕⢄⠑⢄⢈⠢⡁⢯⡂\n⠘⡮⡹⣖⠤⡁⢊⠠⠑⡀⠂⠔⢀⠂⠢⠠⢈⠂⠔⠠⡑⠝⢖⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠳⢝⣔⢦⡱⣜⠵⠃\n⠀⠸⡢⡊⡛⢼⢔⡄⡡⠠⢁⠌⠠⡈⢄⠁⠂⠌⠠⡁⠔⢈⠂⡙⢕⠢⠲⠪⡚⠪⠪⡋⢚⠕⡫⡲⡀⠀⠁⠈\n⠀⠀⠳⡨⢂⠡⠊⡱⠳⠶⣄⣊⠠⠂⠄⢊⠈⢄⠡⠐⡀⠅⡈⠄⢂⠁⡑⠄⠌⡐⠁⠌⠐⠄⡊⢨⡛⡄\n⠀⠀⠈⢕⠔⡀⠊⠠⡈⠢⡑⢍⡳⣳⢜⡤⣌⠠⢂⠂⠔⢀⠢⠈⠄⢂⠐⡈⠄⠨⡀⠅⠑⡠⢈⢆⡽⠁\n⠀⠀⠀⠨⢆⠌⡈⠐⠄⠡⠐⡡⢪⢗⢽⠆⠉⠙⠣⠷⣜⡤⡢⡡⡨⡀⡢⢐⢈⠔⡠⣊⢦⣪⠖⠏\n⠀ ⠀⠀⠀⠳⡨⡀⡑⢈⠂⡡⠐⢌⡷⣙⢖⣄⠀⠀⠀⠀⠈⠉⠙⠚⠚⠪⠳⠓⠋⠋⠁⠁\n⠀ ⠀⠀⠀⠈⢖⠄⠢⠐⠠⠂⢌⠠⢛⢮⡪⡜⣕⡀\n⠀⠀⠀⠀⠀⠀⠘⣎⢐⠡⢈⠂⠢⠐⡁⢝⢮⡪⡢⡹⣂\n⠀⠀⠀⠀⠀⠀⠀⠸⣢⠡⢂⢈⠐⡁⠔⠠⢓⢵⡪⢢⠑⡝⢢⣄\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠫⡢⠄⢌⢀⠊⡐⠡⢊⢷⡑⡌⡐⠡⡘⢦⡀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡔⢢⢀⠊⢄⠑⠔⡡⢻⡔⢌⠂⡕⡸⠆\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡢⡡⡨⠠⡑⠌⡢⢑⠽⣪⡪⣢⠏\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢲⢐⠅⠢⡑⠨⡢⣙⢜\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢕⡕⡡⢊⠒⢔⢌⡗\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠕⡵⣩⡲⡕");
                        if (target != Context.User)
                            account.GCooldown = Convert.ToInt32(unixT);
                        var embed = new EmbedBuilder();
                        embed.WithTitle($":mailbox: **{target.Username}** you have a suprise waiting for you in dms!");
                        string fromUser = Convert.ToString(Context.User);
                        fromUser = fromUser.Remove(fromUser.Length - 5, 5);
                        embed.WithDescription($"from " + fromUser);
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        var embed = new EmbedBuilder();
                        embed.AddField(target.Username + "has blocked this command", "It's not my fault ur annoying!");
                        Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Still on cooldown. " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                }
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!ali-a")]
        public async Task alia()
        {
            try
            {
                await Task.Delay(0);
                await Context.Message.DeleteAsync();
                var aliMsg = await Context.Channel.SendMessageAsync("**3**");
                await Task.Delay(500);
                await aliMsg.ModifyAsync(msg => msg.Content = "**2**");
                await Task.Delay(500);
                await aliMsg.ModifyAsync(msg => msg.Content = "**1**");
                await Task.Delay(500);
                await aliMsg.ModifyAsync(msg => msg.Content = "**DROP IT!**");
                await Task.Delay(500);
                await aliMsg.ModifyAsync(msg => msg.Content = "<a:alia1:525299680553205761><a:alia2:525299708156182538>\n<a:alia3:525299721254993930><a:alia4:525299735796514827>");
                await Task.Delay(6000);
                await aliMsg.DeleteAsync();
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!id")]
        public async Task ID()
        {
            try
            {
                string json = "";
                using (WebClient client = new WebClient())
                {
                    json = client.DownloadString("https://randomuser.me/api/?gender=?&nat=US");
                }

                var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

                string firstname = dataObject.results[0].name.first.ToString();
                string lastname = dataObject.results[0].name.last.ToString();
                string avatarURL = dataObject.results[0].picture.large.ToString();

                var embed = new EmbedBuilder();
                embed.WithThumbnailUrl(avatarURL);
                embed.WithTitle("Generated ID");
                embed.AddInlineField("First Name", firstname.First().ToString().ToUpper() + firstname.Substring(1));
                embed.AddInlineField("Last Name", lastname.First().ToString().ToUpper() + lastname.Substring(1));

                await Context.Channel.SendMessageAsync("", embed: embed);
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!ascii")]
        public async Task asciify([Remainder]string inputs)
        {
            var msg = await Context.Channel.SendMessageAsync("Converting Text... Please wait");
            var inputList = inputs.Split(' ');
            string output = "```";
            foreach (string words in inputList)
                foreach (string part in Misc.SplitInParts(words, 25))
                    output += Misc.getStringFromUrl("http://artii.herokuapp.com/make?text=" + part) + "\n";
            await msg.ModifyAsync(msgg => msgg.Content = output + "```");
        }

        [Command("z!emojify")]
        public async Task emojify([Remainder]string msgC)
        {
            string output = "";
            var inputArray = msgC.ToCharArray();
            foreach (int line in inputArray)
            {
                try
                {
                    if ((inputArray[line] >= 'a' && inputArray[line] <= 'z') || (inputArray[line] >= 'A' && inputArray[line] <= 'Z'))
                        output += ":regional_indicator_" + inputArray[line] + ": ";
                    else
                        if (inputArray[line] == Convert.ToChar(" "))
                            output += "   ";
                }
                catch { }
            }
        }
    }
}
