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
            [Command("!game")]
        public async Task Game([Remainder]string game)
        {
            if (!UserIsZanoxAdmin((SocketGuildUser)Context.User)) return;
            await Context.Client.SetGameAsync(game);
        }

        [Command("!help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($":mailbox_with_mail: The help message has been send to you in dms!");
            embed.WithDescription("");
            embed.WithColor(new Color(0, 255, 255));
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());

            var message = new EmbedBuilder();
            embed.WithTitle($"Zanox Bot Help");
            embed.WithDescription("Commands for Zanox");
            embed.AddField($"Fun Commands!", "fun commands for Zanox");
            embed.AddField($"!8ball", $"Chooses between an object | to seperate (!8ball {Context.User.Username}| Zanox Bot)");
            embed.AddField($"!gotcha {Context.User.Username}", $"Send the tagged person a little suprise!");
            embed.AddField($"Reputation", "reputation commands for Zanox");
            embed.AddField($"+rep {Context.User.Username} reason", $"Adds a reputation point to the tagged member.");
            embed.AddField($"-rep {Context.User.Username} reason", $"Removes a reputation point to the tagged member.");
            embed.AddField($"!rep {Context.User.Username} reason", $"Check the amounts of rep points a person has.");
            embed.AddField($"Levels", $"Level commands for Zanox");
            embed.AddField($"!stats {Context.User.Username}", $"Check the Level and XP of a member.");
            embed.WithColor(new Color(0, 255, 255));
            embed.WithCurrentTimestamp();
            embed.WithFooter(Context.User.GetAvatarUrl());

            await Context.User.SendMessageAsync("", false, embed.Build());
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
        public async Task addXP(uint xp, string arg = "")
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
        public async Task removeXP(uint xp, string arg = "")
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

        [Command("!rep")]
        public async Task Rep([Remainder]string arg = "")
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

        [Command("+rep")]
        public async Task addRep(SocketUser target = null, [Remainder]string reason = "")
        {
            target = target ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            account.Rep += 1;
            UserAccounts.SaveAccounts();

            var embed = new EmbedBuilder();
            embed.WithTitle($"**{target.Username}** has been given 1 rep! By {Context.User.Username}!");
            embed.WithDescription($"Reason: {reason}");
            embed.WithColor(new Color(0, 255, 255));
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
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
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("weather")]
        public async Task Weather(string city)
        {
            var apiUrl = $"api.openweathermap.org/data/2.5/weather?q={city}";
        }

        [Command("!gotcha")]
        public async Task Gotcha(SocketUser target = null)
        {
            target = target ?? Context.User;

            var account = UserAccounts.GetAccount(Context.User);
            string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
            unixT = unixT.Remove(10, unixT.Length - 10);
            int userCooldown = account.GCooldown;
            if (Convert.ToInt32(unixT) - 600 > userCooldown)
            {
                await target.SendMessageAsync($"**{Context.User.Username}** Gotcha!");
                target.SendMessageAsync($"⠀⠀⠀⠀⠀⡼⡑⢅⠪⡐⠅⠪⡐⢌⠢⡑⠌⡢⢑⠌⠢⡑⠌⡢⠑⢌⠢⠑⡌⠰⡁⡣⢘⠵⢦⡀\n⠀⠀⠀⠀⢰⢍⠊⠔⠡⡈⠜⡠⠑⠄⢅⠌⡂⠢⡁⢊⠢⠘⢄⠊⢌⠂⢅⢑⢈⠢⡨⠠⡑⠨⠢⡫⡆\n⠀⠀⠀⠀⡗⢅⠑⡁⡑⠠⠊⡀⡑⠌⠐⠄⢊⠐⡨⠀⢅⠊⡠⠊⠠⠊⡠⠂⡁⠢⠐⡐⢈⠂⡑⢄⢙⢇⡀\n⠀⠀⠀⡸⡑⢌⠐⠄⢌⠐⡁⠔⢀⠊⡨⠠⢁⠢⢀⠑⠠⢂⠐⠌⡐⢁⠄⠌⠠⢁⠌⠠⠁⠔⢀⠢⢀⠣⢳⢄\n⠀⠀⢠⠫⡂⠔⢁⠂⠢⠐⡀⢊⠠⠂⡐⢐⠀⡂⢁⠈⠢⠠⡈⠄⠢⡀⢆⢨⢂⠔⡀⢅⠈⠂⠔⢀⠅⡐⢁⠫⣆\n⠀⢀⢏⠪⢀⠊⡠⠈⢄⠡⠐⡀⠢⢈⠠⠂⠨⢀⠂⡁⡊⣐⢄⣳⠕⠯⠚⠓⠛⠚⠶⣄⠅⡡⠈⢄⠐⠠⢁⠊⡜⣥⠀\n⠀⣜⠥⡑⠠⠂⡐⠈⠄⠄⡡⠠⢁⠂⠄⡑⠠⢁⢌⢔⢮⠎⠋⠁⠀⠀⠀⠀⠀⠀⠀⠑⢧⠐⡡⠠⢈⠂⢄⠡⡈⢮⡀\n⠰⣝⢆⠢⠁⠂⠌⠠⠑⡀⢂⠐⠄⡈⡐⢀⠑⢤⡳⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢙⡬⡀⠂⠔⢀⠢⠐⡈⢎⡇\n⢘⢮⡣⡂⠡⢁⠊⠠⠁⠔⢀⠁⠢⠐⡀⢅⠈⡲⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢷⡨⠨⡀⠅⢐⠈⠄⢪⢖\n⠈⡮⡳⡕⡡⢀⠊⠠⠑⠈⢄⠁⡊⢀⠢⠠⢈⠌⠳⡔⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢳⢕⢄⠑⢄⢈⠢⡁⢯⡂\n⠘⡮⡹⣖⠤⡁⢊⠠⠑⡀⠂⠔⢀⠂⠢⠠⢈⠂⠔⠠⡑⠝⢖⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠳⢝⣔⢦⡱⣜⠵⠃\n⠀⠸⡢⡊⡛⢼⢔⡄⡡⠠⢁⠌⠠⡈⢄⠁⠂⠌⠠⡁⠔⢈⠂⡙⢕⠢⠲⠪⡚⠪⠪⡋⢚⠕⡫⡲⡀⠀⠁⠈\n⠀⠀⠳⡨⢂⠡⠊⡱⠳⠶⣄⣊⠠⠂⠄⢊⠈⢄⠡⠐⡀⠅⡈⠄⢂⠁⡑⠄⠌⡐⠁⠌⠐⠄⡊⢨⡛⡄\n⠀⠀⠈⢕⠔⡀⠊⠠⡈⠢⡑⢍⡳⣳⢜⡤⣌⠠⢂⠂⠔⢀⠢⠈⠄⢂⠐⡈⠄⠨⡀⠅⠑⡠⢈⢆⡽⠁\n⠀⠀⠀⠨⢆⠌⡈⠐⠄⠡⠐⡡⢪⢗⢽⠆⠉⠙⠣⠷⣜⡤⡢⡡⡨⡀⡢⢐⢈⠔⡠⣊⢦⣪⠖⠏\n⠀ ⠀⠀⠀⠳⡨⡀⡑⢈⠂⡡⠐⢌⡷⣙⢖⣄⠀⠀⠀⠀⠈⠉⠙⠚⠚⠪⠳⠓⠋⠋⠁⠁\n⠀ ⠀⠀⠀⠈⢖⠄⠢⠐⠠⠂⢌⠠⢛⢮⡪⡜⣕⡀\n⠀⠀⠀⠀⠀⠀⠘⣎⢐⠡⢈⠂⠢⠐⡁⢝⢮⡪⡢⡹⣂\n⠀⠀⠀⠀⠀⠀⠀⠸⣢⠡⢂⢈⠐⡁⠔⠠⢓⢵⡪⢢⠑⡝⢢⣄\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠫⡢⠄⢌⢀⠊⡐⠡⢊⢷⡑⡌⡐⠡⡘⢦⡀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡔⢢⢀⠊⢄⠑⠔⡡⢻⡔⢌⠂⡕⡸⠆\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡢⡡⡨⠠⡑⠌⡢⢑⠽⣪⡪⣢⠏\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢲⢐⠅⠢⡑⠨⡢⣙⢜\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢕⡕⡡⢊⠒⢔⢌⡗\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠕⡵⣩⡲⡕");
                if (target == Context.User)
                    account.GCooldown = Convert.ToInt32(unixT);
                Context.Channel.SendMessageAsync("Sending gotcha to " + target + "!");
            } else {
                await Context.Channel.SendMessageAsync("Still on cooldown. " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
            }
        }

        [Command("!unix")]
        public async Task unixTime()
        {
            await Task.Delay(0);
            string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
            unixT = unixT.Remove(10, unixT.Length - 10);
            await Context.Channel.SendMessageAsync(unixT);
        }

        [Command("!removeCooldown")]
        public async Task rCooldown()
        {
            var account = UserAccounts.GetAccount(Context.User);
            account.GCooldown = 0;
        }
    }
}
