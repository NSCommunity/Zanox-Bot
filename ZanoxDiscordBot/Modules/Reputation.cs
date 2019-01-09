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
    public class Reputation : ModuleBase<SocketCommandContext>
    {
        [Command("z!rep")]
        public async Task Rep([Remainder]string arg = "")
        {
            try
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;

                var account = UserAccounts.GetAccount(target);
                var embed = new EmbedBuilder();
                embed.WithTitle(target.Username + "'s Reputation");
                embed.WithDescription($"requested by {Context.User.Username}!");
                embed.AddField("Reputation Points", account.Rep);
                embed.WithColor(new Color(0, 255, 255));
                embed.WithThumbnailUrl(target.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("+rep")]
        public async Task addRep(SocketUser target = null, [Remainder]string reason = "No Reason Provided.")
        {
            try
            {
                var account = UserAccounts.GetAccount(Context.User);
                string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
                unixT = unixT.Remove(10, unixT.Length - 10);
                int userCooldown = account.repCooldown;

                string ContextUser = Convert.ToString(Context.User);
                ContextUser = ContextUser.Remove(ContextUser.Length - 5, 5);
                if (target.Username != ContextUser)
                {
                    if (Convert.ToInt32(unixT) - 600 > userCooldown)
                    {
                        target = target ?? Context.User;

                        account.Rep += 1;
                        UserAccounts.SaveAccounts();

                        var embed = new EmbedBuilder();
                        embed.WithTitle($"**{target.Username}** has been given 1 rep! By {Context.User.Username}!");
                        embed.WithDescription($"Reason: {reason}");
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();
                        account.repCooldown = Convert.ToInt32(unixT);

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        var embed = new EmbedBuilder();
                        embed.WithTitle($"You're still on cooldown! " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"You can't give yourself rep!");
                    embed.WithColor(new Color(0, 255, 255));
                    embed.WithCurrentTimestamp();

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("-rep")]
        public async Task removeRep(SocketUser target = null, [Remainder]string reason = "No Reason Provided.")
        {
            try
            {
                var account = UserAccounts.GetAccount(Context.User);
                string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
                unixT = unixT.Remove(10, unixT.Length - 10);
                int userCooldown = account.repCooldown;

                string ContextUser = Convert.ToString(Context.User);
                ContextUser = ContextUser.Remove(ContextUser.Length - 5, 5);
                if (target.Username != ContextUser)
                {
                    if (Convert.ToInt32(unixT) - 600 > userCooldown)
                    {
                        target = target ?? Context.User;

                        account.Rep -= 1;
                        UserAccounts.SaveAccounts();

                        var embed = new EmbedBuilder();
                        embed.WithTitle($"**{target.Username}** has been gave negative 1 rep! By {Context.User.Username}!");
                        embed.WithDescription($"Reason: {reason}");
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();
                        account.repCooldown = Convert.ToInt32(unixT);

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        var embed = new EmbedBuilder();
                        embed.WithTitle($"You're still on cooldown! " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"You can't take away your own rep!");
                    embed.WithColor(new Color(0, 255, 255));
                    embed.WithCurrentTimestamp();
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }

            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }
    }
}
