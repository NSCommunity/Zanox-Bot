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
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }
        }


        [Command("z!game")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Game([Remainder]string game)
        {
            if (!UserIsZanoxAdmin((SocketGuildUser)Context.User)) return;
            await Context.Client.SetGameAsync(game);
            await Context.User.SendMessageAsync($"The game how now been set to `{game}`");
        }

        [Command("z!help")]
        public async Task Help()
        {
            try
            {
                //Let user know we've sent the help information to their DMs
                var embed = new EmbedBuilder();
                embed.WithTitle($":mailbox_with_mail: The help message has been send to you in dms!");
                embed.WithDescription(" ");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithCurrentTimestamp();
                await Context.Channel.SendMessageAsync(" ", false, embed.Build());

                var message = new EmbedBuilder();
                //Title
                embed.WithTitle($"Zanox Bot Help");
                embed.WithDescription("Commands for Zanox");

                //Fun Commands
                embed.AddField($"⠀", "⠀");
                embed.AddField($"Fun Commands!", "fun commands for Zanox");
                embed.AddField($"z!8ball", $"Chooses between an object | to seperate (!8ball {Context.User.Username}| Zanox Bot)");
                embed.AddField($"z!gotcha {Context.User.Username}", $"Send the tagged person a little suprise!");

                //Reputation
                embed.AddField($"⠀", "⠀");
                embed.AddField($"Reputation", "reputation commands for Zanox");
                embed.AddField($"+rep {Context.User.Username} reason", $"Adds a reputation point to the tagged member.");
                embed.AddField($"-rep {Context.User.Username} reason", $"Removes a reputation point to the tagged member.");
                embed.AddField($"z!rep {Context.User.Username} reason", $"Check the amounts of rep points a person has.");

                //Leveling
                embed.AddField($"⠀", "⠀");
                embed.AddField($"Levels", $"Level commands for Zanox");
                embed.AddField($"z!stats {Context.User.Username}", $"Check the Level and XP of a member.");
                embed.WithColor(new Color(0, 255, 255));
                embed.WithCurrentTimestamp();
                embed.WithFooter(Context.User.GetAvatarUrl());

                var react = await Context.User.SendMessageAsync("", false, embed.Build());

                var yes = new Emoji("✅");
                react.AddReactionAsync(yes);

                var no = new Emoji("❌");
                react.AddReactionAsync(no);
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!say")]
        public async Task Say([Remainder]string message)
        {
            try
            {
                var u = Context.User as SocketGuildUser;
                await Context.Message.DeleteAsync();
                await Context.Channel.SendMessageAsync(message);
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!key")]
        public async Task SecretKey()
        {
            try
            {
                if (!UserIsZanoxAdmin((SocketGuildUser)Context.User)) return;
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync(Utilities.GetAlert("KEY"));
                await Context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
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

        [Command("z!discord")]
        public async Task Discord()
        {
            try
            {
                await Context.Channel.SendMessageAsync("Discord: https://discord.gg/dWwBmxB.");
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }

        }

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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!weather")]
        public async Task Weather([Remainder]string city)
        {
            await Task.Delay(0);
            try
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle("Zanox Weather");
                embed.WithImageUrl($"https://www.countryflags.io/{getStringFromUrl($"http://zanoxhosting.ga/api/weather/country.php?q={city}")}/flat/64.png");
                embed.AddInlineField(
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/temperature.php?q={city}&u=C")
                    + "℃",
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/city.php?q={city}")
                    + ", " +
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/country.php?q={city}"));
                embed.AddInlineField("Weather Status",
                    getStringFromUrl($"http://zanoxhosting.ga/api/weather/weather.php?q={city}"));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!gotcha")]
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
                    await Context.Channel.SendMessageAsync("Still on cooldown. " + ((userCooldown - Convert.ToInt32(unixT)) + 600) + " seconds left");
                }
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }
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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!welcome")]
        public async Task Test()
        {
            try
            {
                var userImg = @"https://cdn.discordapp.com/avatars/" + Context.User.Id + @"/" + Context.User.AvatarId + @".png".Replace(" ", "%20");
                string html = String.Format($"<html><head> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/css/bootstrap.min.css\"></head><body style=\"overflow:hidden;background-image:url(&quot;http://92.244.209.118/DMNHosting/api/discord/welcomeAssets/img/eS4IxK3.png&quot;);\"> <div style=\"display:table;position:absolute;top:0;left:0;height:100%;width:100%;\"> <div style=\"display:table-cell;vertical-align:middle;\"> <div style=\"margin-left:auto;margin-right:auto;text-align:center;\"><img src=\"{userImg}\" style=\"height:125px;width:125px;\"><div></div>\" <div style=\"color:#FFF;\"><h1 style=\"font-size:25px;\"><b>Welcome to {Context.Guild.Name},</b></h1> <h1 style=\"font-size:15px;\"><b>{Context.User.Username}</b></h1> </div></div></div></div><script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.1.0/js/bootstrap.bundle.min.js\"></script></body></html>");

                var converter = new HtmlToImageConverter { Width = 500, Height = 250 };
                var jpgBytes = converter.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Png);
                await Context.Guild.DefaultChannel.SendMessageAsync("test");
                await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), $"Welcome {Context.User.Username}.png");
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!kick")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task hveTest(IGuildUser user, [Remainder] string reason = null)
        {
            try
            {
                Console.WriteLine("Kicked " + user.Username);
                await user.KickAsync();
                await user.Guild.AddBanAsync(user, reason: reason);
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!testGet")]
        public async Task testGet()
        {
            var account = UserAccounts.GetOrCreateAccount(Context.Channel.Id);
            await Context.Channel.SendMessageAsync("account = " + account.levelingAlert);
        }

        [Command("z!I accept all these rules")]
        public async Task acceptRulesOfficial()
        {
            try
            {
                await Task.Delay(0);
                if (Context.Channel.Id == 525282267715600404)
                {
                    Context.Message.DeleteAsync();
                    var user = Context.User;
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");
                    await (user as IGuildUser).AddRoleAsync(role);
                }
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
            }
        }

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
                ExceptionAlert(Context, e);
            }
        }

        [Command("z!ascii")]
        public async Task asciify([Remainder]string input)
        {
            await Context.Channel.SendMessageAsync("```" + getStringFromUrl("http://artii.herokuapp.com/make?text=" + input) + "```");
        }

        [Command("z!emojify")]
        public async Task emojify([Remainder]string msgC)
        {
            try
            {
                string msg = "";
                var inputArray = msgC.ToCharArray();
                foreach (int line in inputArray)
                {
                    if ((inputArray[line] >= 'a' && inputArray[line] <= 'z') || (inputArray[line] >= 'A' && inputArray[line] <= 'Z'))
                    {
                        msg = msg + ":regional_indicator_" + inputArray[line] + ": ";
                    }
                    else
                    {
                        if (inputArray[line] == Convert.ToChar(" "))
                        {
                            msg = msg + "   ";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
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
                ExceptionAlert(Context, e);
            }
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

        [Command("z!brag")]
        public async Task brag()
        {
            Task.Run(guilds);
        }

        [Command("z!ParallelRes")]
        public async Task PRes([Remainder]string input)
        {
            await Task.Delay(0);
            var inputList = input.Split('!').ToList();
            decimal total = 0;
            foreach (string lines in inputList)
            {
                total += (1 / Convert.ToDecimal(lines));
            }
            await Context.Channel.SendMessageAsync(Convert.ToString(1 / total));
        }

        [Command("z!Lumens")]
        public async Task Lumy([Remainder]string input)
        {
            await Task.Delay(0);
            var inputList = input.Split('!').ToList();
            decimal total = Convert.ToInt32(inputList[0]) * Convert.ToInt32(inputList[1]);
            await Context.Channel.SendMessageAsync(Convert.ToString(total));
        }

        [Command("z!serverID")]
        public async Task serverID()
        {
            await Context.Channel.SendMessageAsync(Context.Guild.Id.ToString());
        }

        [Command("z!tts")]
        [RequireUserPermission(GuildPermission.SendTTSMessages)]
        public async Task TheArrr([Remainder]string input)
        {
            var delete = await Context.Channel.SendMessageAsync(input, true);
            delete.DeleteAsync();
        }

        [Command("z!Dog")]
        [RequireUserPermission(GuildPermission.AttachFiles)]
        public async Task dog()
        {
            string json = getStringFromUrl(@"https://dog.ceo/api/breeds/image/random");
            json = json.Remove(0, 31).Remove(json.Length - 33, 2).Replace(@"\", "");
            var dogUrl = new Uri(json);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(json), @"dog." + json.Remove(0, json.Length - 3));
            }
            await Context.Channel.SendFileAsync("dog." + json.Remove(0, json.Length - 3));
        }

        [Command("z!f")]
        public async Task ffff([Remainder]string input)
        {
            while (true)
                Context.Channel.SendMessageAsync(input);
        }

        public async Task ExceptionAlert(SocketCommandContext Context, Exception e)
        {
            try {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                await Context.Channel.SendMessageAsync("There was an exception! I've told my creators about all the details! \n Do you them to contact you?");
                var properties = e.GetType()
                            .GetProperties();
                var fields = properties
                                 .Select(property => new {Name = property.Name, Value = property.GetValue(e, null)})
                                 .Select(x => String.Format("{0} = {1}", x.Name,  x.Value != null ? x.Value.ToString() : String.Empty));
                await Context.Client.GetUser((ulong)261418273009041408).SendMessageAsync(Context.User.Username + "#" + Context.User.Discriminator + " is having some issues!");
                await Context.Client.GetUser((ulong)261418273009041408).SendMessageAsync("Line: " + line);
                await Context.Client.GetUser((ulong)261418273009041408).SendMessageAsync("Command issued: " + Context.Message.Content);
                await Context.Client.GetUser((ulong)261418273009041408).SendMessageAsync(String.Join("\n", fields));
            }
            catch (Exception ex)
            {
                ExceptionAlert(Context, ex);
            }
        }

        public static String getStringFromUrl(string Url)
        {
            using (WebClient client = new WebClient())
            {
                string result = client.DownloadString(Url);
                return result;
            }
        }
    }
}