using System;
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
using System.Net.Http;

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
            await Context.Channel.SendMessageAsync("@everyone");
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

        [Command("!level")]
        public async Task Level(uint xp)
        {
            uint level = 5;
            await Context.Channel.SendMessageAsync($"{Context.User.Username} is level {level}!");
        }

        [Command("!stats")]
        public async Task Stats([Remainder]string arg = "")
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
            embed.WithColor(new Color(0, 255, 255));
            embed.WithThumbnailUrl(target.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("+xp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task addXP(uint xp)
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

        [Command("-xp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task removeXP(uint xp)
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

        [Command("+rep")]
        public async Task addRep(SocketUser target = null, [Remainder]string reason = "")
        {
            target = target ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            account.Rep += 1;
            UserAccounts.SaveAccounts();

            var embed = new EmbedBuilder();
            embed.WithTitle($"**{target.Username}** has been given 1 rep!By {Context.User.Username}!");
            embed.WithDescription($"Reason: {reason}");
            embed.WithColor(new Color(0, 255, 255));
            embed.WithFooter(Context.User.GetAvatarUrl());
            embed.WithCurrentTimestamp();
        }

        [Command("-rep")]
        public async Task removeRep(SocketUser target = null, [Remainder]string reason = "")
        {
            target = target ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            account.Rep -= 1;
            UserAccounts.SaveAccounts();

            var embed = new EmbedBuilder();
            embed.WithTitle($"**{target.Username}** has been removed 1 rep! By {Context.User.Username}!");
            embed.WithDescription($"Reason: {reason}");
            embed.WithColor(new Color(0, 255, 255));
            embed.WithFooter(Context.User.GetAvatarUrl());
            embed.WithCurrentTimestamp();
        }

        [Command("weather")]
        public async Task Weather(string city)
        {
            var apiUrl = $"api.openweathermap.org/data/2.5/weather?q={city}";
        }
    }
}
