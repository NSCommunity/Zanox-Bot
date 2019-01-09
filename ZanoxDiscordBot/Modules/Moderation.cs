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
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("z!getInvite")]
        public async Task getinv()
        {
            try
            {
                var invites = await Context.Guild.GetInvitesAsync();

                await Context.Channel.SendMessageAsync(invites.Select(x => x.Url).FirstOrDefault());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!toggleServerLevelingAlert")]
        public async Task toggleServerLevelAlert()
        {
            try
            {
                await Task.Delay(0);
                var account = UserAccounts.GetOrCreateAccount(Context.Guild.Id);
                if (account.levelingAlert == 0)
                {
                    account.levelingAlert = 1;
                    UserAccounts.SaveAccounts();
                    await Context.Channel.SendMessageAsync("Leveling Alerts has been on toggled for this server");
                }
                else
                {
                    account.levelingAlert = 0;
                    UserAccounts.SaveAccounts();
                    await Context.Channel.SendMessageAsync("Leveling Alerts has been toggled off for this server");
                }
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!toggleLevelingAlert")]
        public async Task toggleLevelAlert()
        {
            try
            {
                await Task.Delay(0);
                var account = UserAccounts.GetOrCreateAccount(Context.Channel.Id);
                Task.Delay(100);
                if (account.levelingAlert == 0)
                {
                    account.levelingAlert = 1;
                    UserAccounts.SaveAccounts();
                    await Context.Channel.SendMessageAsync("Leveling Alerts has been on toggled for this channel");
                }
                else
                {
                    account.levelingAlert = 0;
                    UserAccounts.SaveAccounts();
                    await Context.Channel.SendMessageAsync("Leveling Alerts has been toggled off for this channel");
                }
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeChat(uint amount)
        {
            try
            {
                var messages = await this.Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();

                await this.Context.Channel.DeleteMessagesAsync(messages);
                const int delay = 5000;
                var m = await this.ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
                await Task.Delay(delay);
                await m.DeleteAsync();
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!brag")]
        public async Task brag()
        {
            Task.Run(guilds);
        }

        [Command("z!guilds")]
        public async Task guilds()
        {
            string list = "```";
            List<SocketGuild> GList = Context.Client.Guilds.ToList();
            int totalMembers = 0;
            foreach (SocketGuild i in GList)
            {
                list = list + i.Name + " (" + i.Users.Count().ToString() + " members) \n";
                totalMembers += i.Users.Count();
            }
            list = list + "\nThat's a total of " + GList.Count() + " guilds with " + totalMembers + " members```";
            await Context.Channel.SendMessageAsync(list);
        }

        [Command("z!kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, string reason = "No Reason Provided.")
        {
            try
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;

                var embed = new EmbedBuilder();
                embed.WithTitle($"You've been kicked by {Context.User.Username}!");
                embed.WithDescription($"Reason: {reason}");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithCurrentTimestamp();

                await target.SendMessageAsync("", false, embed.Build());
                await Context.User.SendMessageAsync($"{target.Username} has been banned by {Context.User.Username}");
                await user.KickAsync(reason);
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, string reason = "No Reason Provided.")
        {
            try
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;

                var embed = new EmbedBuilder();
                embed.WithTitle($"You've been kicked by {Context.User.Username}!");
                embed.WithDescription($"Reason: {reason}");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithCurrentTimestamp();

                await target.SendMessageAsync("", false, embed.Build());
                await Context.User.SendMessageAsync($"{target.Username} has been banned by {Context.User.Username}");
                await user.Guild.AddBanAsync(user, 5, reason);
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!warn")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGuildUser user, string reason = "No Reason Provided.")
        {
            try
            {
                var userAccount = UserAccounts.GetAccount((SocketUser)user);
                userAccount.NumberOfWarnings++;
                UserAccounts.SaveAccounts();

                if (userAccount.NumberOfWarnings >= 3)
                {
                    await user.Guild.AddBanAsync(user, 20);
                }
                else if (userAccount.NumberOfWarnings == 2) { }
                else if (userAccount.NumberOfWarnings == 1)
                {
                    SocketUser target = null;
                    var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                    target = mentionedUser ?? Context.User;

                    var embed = new EmbedBuilder();
                    embed.WithTitle($"You've been warned by {Context.User.Username}!");
                    embed.WithDescription($"Reason: {reason}");
                    embed.WithColor(new Color(0, 255, 255));
                    embed.WithCurrentTimestamp();

                    await target.SendMessageAsync("", false, embed.Build());
                    await Context.User.SendMessageAsync($"{target.Username} has been banned by {Context.User.Username}");
                }
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!announcement")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Announcement([Remainder]string message)
        {
            try
            {
                var u = Context.User as SocketGuildUser;
                var permission = u.GuildPermissions.Administrator;
                if (!permission)
                {
                    await Context.Channel.SendMessageAsync(":x: You do not have permission to use this command " + Context.User.Mention + "!");
                    return;
                }
                var embed = new EmbedBuilder();
                embed.WithTitle("announcement by " + Context.User.Username);
                embed.WithDescription(message);
                embed.WithColor(new Color(0, 255, 0));
                await Context.Message.DeleteAsync();
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
                await Task.Delay(3000);
                await Context.Channel.SendMessageAsync("@everyone");
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!serverID")]
        public async Task serverID()
        {
            await Context.Channel.SendMessageAsync(Context.Guild.Id.ToString());
        }

        [Command("z!setDefault")]
        public async Task setDef()
        {
            try
            {
                await Task.Delay(0);
                await Context.Channel.SendMessageAsync("Default Channel is now " + Context.Channel.Id + " as server " + Context.Guild.Id);
                var account = UserAccounts.GetOrCreateAccount(Context.Guild.Id);
                account.DefaultChannelID = Context.Channel.Id;
                UserAccounts.SaveAccounts();
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }
    }
}
