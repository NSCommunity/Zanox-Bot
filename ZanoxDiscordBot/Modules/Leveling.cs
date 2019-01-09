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
    public class Leveling : ModuleBase<SocketCommandContext>
    {
        [Command("z!level")]
        public async Task Level(uint xp)
        {
            try
            {
                uint level = 5;
                await Context.Channel.SendMessageAsync($"{Context.User.Username} is level {level}!");
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!stats")]
        public async Task Stats([Remainder]string arg = "")
        {
            try
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;

                var account = UserAccounts.GetAccount(target);
                var embed = new EmbedBuilder();
                embed.WithTitle(target.Username + "'s Stats");
                embed.WithDescription($"requested by {Context.User.Username}!");
                embed.AddInlineField("Level", account.LevelNumber);
                embed.AddInlineField("XP", account.XP);
                embed.AddInlineField("Reputation", account.Rep);
                embed.WithColor(new Color(0, 255, 255));
                embed.WithThumbnailUrl(target.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("+xp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task addXP(uint xp, string arg = "")
        {
            try
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;
                var u = Context.User as SocketGuildUser;
                var permission = u.GuildPermissions.Administrator;
                if (!permission)
                {
                    await Context.Channel.SendMessageAsync(":x: You do not have permission to use this command " + Context.User.Mention + "!");
                    return;
                }
                var account = UserAccounts.GetAccount(Context.User);
                account.XP += xp;
                UserAccounts.SaveAccounts();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{target.Username}has been given {xp} XP");
                embed.WithDescription($"by {Context.User.Username}.");
                embed.AddInlineField("Level", account.LevelNumber);
                embed.AddInlineField("XP", account.XP);
                embed.WithColor(new Color(0, 255, 255));
                embed.WithThumbnailUrl(target.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("-xp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task removeXP(uint xp, string arg = "")
        {
            try
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;
                var u = Context.User as SocketGuildUser;
                var permission = u.GuildPermissions.Administrator;
                if (!permission)
                {
                    await Context.Channel.SendMessageAsync(":x: You do not have permission to use this command " + Context.User.Mention + "!");
                    return;
                }
                var account = UserAccounts.GetAccount(Context.User);
                account.XP -= xp;
                UserAccounts.SaveAccounts();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{target.Username}has been removed {xp} XP");
                embed.WithDescription($"by {Context.User.Username}.");
                embed.AddInlineField("Level", account.LevelNumber);
                embed.AddInlineField("XP", account.XP);
                embed.WithColor(new Color(0, 255, 255));
                embed.WithThumbnailUrl(target.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }

        [Command("z!profile")]
        public async Task ProfileTest([Remainder]string arg = "")
        {
            try
            {
                var embed = new EmbedBuilder();
                embed.WithTitle($"**{Context.User.Username}'s** Profile!");
                embed.WithDescription("");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithFooter(Context.User.GetAvatarUrl());
                embed.WithCurrentTimestamp();

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Misc.ExceptionAlert(Context, e);
            }
        }
    }
}
