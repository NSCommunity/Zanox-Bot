﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ZanoxDiscordBot.Core.UserAccounts;
using System.Net;
using Newtonsoft.Json;

namespace ZanoxDiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private bool UserIsZanoxAdmin(SocketGuildUser user)
        {
            string targetRoleName = "ZanoxAdmin";
            var resault = from r in user.Guild.Roles
                          where r.Name == targetRoleName
                          select r.Id;
            ulong roleID = resault.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        [Command("!announcement")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Announcement([Remainder]string message)
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
            await Context.Channel.SendMessageAsync("@ everyone");
        }

        [Command("!say")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Say([Remainder]string message)
        {
            var u = Context.User as SocketGuildUser;
            var permission = u.GuildPermissions.Administrator;
            if (!permission)
            {
                await Context.Channel.SendMessageAsync(":x: You do not have permission to use this command " + Context.User.Mention + "!");
                return;
            }
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(message);
        }

        [Command("!8ball")]
        public async Task EightBall([Remainder]string message)
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

        [Command("!key")]
        public async Task SecretKey()
        {
            if (!UserIsZanoxAdmin((SocketGuildUser)Context.User)) return;
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(Utilities.GetAlert("KEY"));
            await Context.Message.DeleteAsync();
        }

        private bool UserIsZanox(SocketGuildUser user)
        {
            string targetRoleName = "ZanoxAdmin";
            var resault = from r in user.Guild.Roles
                          where r.Name == targetRoleName
                          select r.Id;
            ulong roleID = resault.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        [Command("!stats")]
        public async Task Stats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            await Context.Channel.SendMessageAsync($"**{target.Username}** is level {account.Level} and has {account.XP} XP!");
        }

        [Command("+xp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task addXP(uint xp)
        {
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
            await Context.Channel.SendMessageAsync($"You have been given {xp} XP by {Context.User.Username}.");
        }

        [Command("-xp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task removeXP(uint xp)
        {
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
            await Context.Channel.SendMessageAsync($"You have been removed {xp} XP by {Context.User.Username}.");
        }

        [Command("!profile")]
        public async Task ProfileTest([Remainder]string arg = "")
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Username}'s** Profile!");
            embed.WithDescription("");
            embed.WithColor(new Color(0, 255, 255));
            embed.WithFooter(Context.User.GetAvatarUrl());
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("!weather")]
        public async Task Weather()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://samples.openweathermap.org/data/2.5/weather?lat=35&lon=139&appid=b6907d289e10d714a6e88b30761fae22");
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string weather = dataObject.weather[0].weather.ToString();
            await Context.Channel.SendMessageAsync($"The weather is {weather}!");
        }
    }
}