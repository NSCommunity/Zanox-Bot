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

        [Command("!warn")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGuildUser user, string reason = "No Reason Provided.")
        {
            var userAccount = UserAccounts.GetAccount((SocketUser)user);
            userAccount.NumberOfWarnings++;
            UserAccounts.SaveAccounts();

            if (userAccount.NumberOfWarnings >= 3)
            {
                await user.Guild.AddBanAsync(user, 20);
            }
            else if (userAccount.NumberOfWarnings == 2)
            {
                
            }
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

        [Command("!kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, string reason = "No Reason Provided.")
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

        [Command("!ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, string reason = "No Reason Provided.")
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

        [Command("!game")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Game([Remainder]string game)
        {
            if (!UserIsZanoxAdmin((SocketGuildUser)Context.User)) return;
            await Context.Client.SetGameAsync(game);
            await Context.User.SendMessageAsync($"The game how now been set to `{game}`");
        }

        [Command("!help")]
        public async Task Help()
        {
            try
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
            catch { }
        }

        [Command("!announcement")]
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
            catch { }
        }

        [Command("!say")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Say([Remainder]string message)
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
                await Context.Message.DeleteAsync();
                await Context.Channel.SendMessageAsync(message);
            }
            catch { }
        }

        [Command("!8ball")]
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
            catch { }
        }

        [Command("!key")]
        public async Task SecretKey()
        {
            try
            {
                if (!UserIsZanoxAdmin((SocketGuildUser)Context.User)) return;
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync(Utilities.GetAlert("KEY"));
                await Context.Message.DeleteAsync();
            }
            catch { }
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

        [Command("!discord")]
        public async Task Discord()
        {
            await Context.Channel.SendMessageAsync("Discord: https://discord.gg/dWwBmxB.");
        }

        [Command("!level")]
        public async Task Level(uint xp)
        {
            try
            {
                uint level = 5;
                await Context.Channel.SendMessageAsync($"{Context.User.Username} is level {level}!");
            }
            catch { }
        }

        [Command("!stats")]
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
            catch { }
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
            catch { }
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
            catch { }
        }

        [Command("!profile")]
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
            catch { }
        }

        [Command("!rep")]
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
            catch { }
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

                        Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        var embed = new EmbedBuilder();
                        embed.WithTitle($"You're still on cooldown! " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();

                        Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"You can't give yourself rep!");
                    embed.WithColor(new Color(0, 255, 255));
                    embed.WithCurrentTimestamp();

                    Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            catch { }
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

                        Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        var embed = new EmbedBuilder();
                        embed.WithTitle($"You're still on cooldown! " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                        embed.WithColor(new Color(0, 255, 255));
                        embed.WithCurrentTimestamp();

                        Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"You can't take away your own rep!");
                    embed.WithColor(new Color(0, 255, 255));
                    embed.WithCurrentTimestamp();

                    Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            catch { }
        }

        [Command("weather")]
        public async Task Weather(string city)
        {
            try
            {
                var apiUrl = $"api.openweathermap.org/data/2.5/weather?q={city}";
            }
            catch { }
        }

        [Command("!gotcha")]
        public async Task Gotcha(SocketUser target = null)
        {
            try
            {
                target = target ?? Context.User;

                var account = UserAccounts.GetAccount(Context.User);
                string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
                unixT = unixT.Remove(10, unixT.Length - 10);
                int userCooldown = account.GCooldown;
                if (Convert.ToInt32(unixT) - 600 > userCooldown || target == Context.User)
                {
                    await target.SendMessageAsync($"**{Context.User.Username}** Gotcha!");
                    target.SendMessageAsync($"⠀⠀⠀⠀⠀⡼⡑⢅⠪⡐⠅⠪⡐⢌⠢⡑⠌⡢⢑⠌⠢⡑⠌⡢⠑⢌⠢⠑⡌⠰⡁⡣⢘⠵⢦⡀\n⠀⠀⠀⠀⢰⢍⠊⠔⠡⡈⠜⡠⠑⠄⢅⠌⡂⠢⡁⢊⠢⠘⢄⠊⢌⠂⢅⢑⢈⠢⡨⠠⡑⠨⠢⡫⡆\n⠀⠀⠀⠀⡗⢅⠑⡁⡑⠠⠊⡀⡑⠌⠐⠄⢊⠐⡨⠀⢅⠊⡠⠊⠠⠊⡠⠂⡁⠢⠐⡐⢈⠂⡑⢄⢙⢇⡀\n⠀⠀⠀⡸⡑⢌⠐⠄⢌⠐⡁⠔⢀⠊⡨⠠⢁⠢⢀⠑⠠⢂⠐⠌⡐⢁⠄⠌⠠⢁⠌⠠⠁⠔⢀⠢⢀⠣⢳⢄\n⠀⠀⢠⠫⡂⠔⢁⠂⠢⠐⡀⢊⠠⠂⡐⢐⠀⡂⢁⠈⠢⠠⡈⠄⠢⡀⢆⢨⢂⠔⡀⢅⠈⠂⠔⢀⠅⡐⢁⠫⣆\n⠀⢀⢏⠪⢀⠊⡠⠈⢄⠡⠐⡀⠢⢈⠠⠂⠨⢀⠂⡁⡊⣐⢄⣳⠕⠯⠚⠓⠛⠚⠶⣄⠅⡡⠈⢄⠐⠠⢁⠊⡜⣥⠀\n⠀⣜⠥⡑⠠⠂⡐⠈⠄⠄⡡⠠⢁⠂⠄⡑⠠⢁⢌⢔⢮⠎⠋⠁⠀⠀⠀⠀⠀⠀⠀⠑⢧⠐⡡⠠⢈⠂⢄⠡⡈⢮⡀\n⠰⣝⢆⠢⠁⠂⠌⠠⠑⡀⢂⠐⠄⡈⡐⢀⠑⢤⡳⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢙⡬⡀⠂⠔⢀⠢⠐⡈⢎⡇\n⢘⢮⡣⡂⠡⢁⠊⠠⠁⠔⢀⠁⠢⠐⡀⢅⠈⡲⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢷⡨⠨⡀⠅⢐⠈⠄⢪⢖\n⠈⡮⡳⡕⡡⢀⠊⠠⠑⠈⢄⠁⡊⢀⠢⠠⢈⠌⠳⡔⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢳⢕⢄⠑⢄⢈⠢⡁⢯⡂\n⠘⡮⡹⣖⠤⡁⢊⠠⠑⡀⠂⠔⢀⠂⠢⠠⢈⠂⠔⠠⡑⠝⢖⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠳⢝⣔⢦⡱⣜⠵⠃\n⠀⠸⡢⡊⡛⢼⢔⡄⡡⠠⢁⠌⠠⡈⢄⠁⠂⠌⠠⡁⠔⢈⠂⡙⢕⠢⠲⠪⡚⠪⠪⡋⢚⠕⡫⡲⡀⠀⠁⠈\n⠀⠀⠳⡨⢂⠡⠊⡱⠳⠶⣄⣊⠠⠂⠄⢊⠈⢄⠡⠐⡀⠅⡈⠄⢂⠁⡑⠄⠌⡐⠁⠌⠐⠄⡊⢨⡛⡄\n⠀⠀⠈⢕⠔⡀⠊⠠⡈⠢⡑⢍⡳⣳⢜⡤⣌⠠⢂⠂⠔⢀⠢⠈⠄⢂⠐⡈⠄⠨⡀⠅⠑⡠⢈⢆⡽⠁\n⠀⠀⠀⠨⢆⠌⡈⠐⠄⠡⠐⡡⢪⢗⢽⠆⠉⠙⠣⠷⣜⡤⡢⡡⡨⡀⡢⢐⢈⠔⡠⣊⢦⣪⠖⠏\n⠀ ⠀⠀⠀⠳⡨⡀⡑⢈⠂⡡⠐⢌⡷⣙⢖⣄⠀⠀⠀⠀⠈⠉⠙⠚⠚⠪⠳⠓⠋⠋⠁⠁\n⠀ ⠀⠀⠀⠈⢖⠄⠢⠐⠠⠂⢌⠠⢛⢮⡪⡜⣕⡀\n⠀⠀⠀⠀⠀⠀⠘⣎⢐⠡⢈⠂⠢⠐⡁⢝⢮⡪⡢⡹⣂\n⠀⠀⠀⠀⠀⠀⠀⠸⣢⠡⢂⢈⠐⡁⠔⠠⢓⢵⡪⢢⠑⡝⢢⣄\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠫⡢⠄⢌⢀⠊⡐⠡⢊⢷⡑⡌⡐⠡⡘⢦⡀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡔⢢⢀⠊⢄⠑⠔⡡⢻⡔⢌⠂⡕⡸⠆\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡢⡡⡨⠠⡑⠌⡢⢑⠽⣪⡪⣢⠏\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢲⢐⠅⠢⡑⠨⡢⣙⢜\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢕⡕⡡⢊⠒⢔⢌⡗\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠕⡵⣩⡲⡕");
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
                    await Context.Channel.SendMessageAsync("Still on cooldown. " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                }
            }
            catch { }
        }

        [Command("!ali-a")]
        public async Task alia()
        {
            await Task.Delay(0);
            Context.Message.DeleteAsync();
            var aliMsg = await Context.Channel.SendMessageAsync("**3**");
            await Task.Delay(1000);
            await aliMsg.ModifyAsync(msg => msg.Content = "**2**");
            await Task.Delay(1000);
            await aliMsg.ModifyAsync(msg => msg.Content = "**1**");
            await Task.Delay(1000);
            await aliMsg.ModifyAsync(msg => msg.Content = "**DROP IT!**");
            await Task.Delay(1000);
            await aliMsg.ModifyAsync(msg => msg.Content = "<a:alia1:522851639472685086><a:alia2:522851690060185648>\n<a:alia3:522851727087763476><a:alia4:522851784587214858>");
            await Task.Delay(6000);
            await aliMsg.DeleteAsync();
        }




        [Command("!unix")]
        public async Task unixTime()
        {
            try
            {
                await Task.Delay(0);
                string unixT = (Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));
                unixT = unixT.Remove(10, unixT.Length - 10);
                await Context.Channel.SendMessageAsync(unixT);
            }
            catch { }
        }
    }
}