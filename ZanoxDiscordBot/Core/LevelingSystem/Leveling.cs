using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZanoxDiscordBot.Core.LevelingSystem
{
    internal static class Leveling
    {
        internal static async void UserSentMessageAsync(SocketGuildUser user, SocketTextChannel channel)
        {
            try
            {
                var userAccount = UserAccounts.UserAccounts.GetAccount(user);
                uint oldLevel = userAccount.LevelNumber;
                userAccount.XP += 50;
                UserAccounts.UserAccounts.SaveAccounts();
                uint newLevel = userAccount.LevelNumber;

                if (oldLevel != newLevel)
                {
                    var GuildSettings = UserAccounts.UserAccounts.GetOrCreateAccount(user.Guild.Id);
                    if (GuildSettings.levelingAlert == 1)
                    {
                        var embed = new EmbedBuilder();
                        embed.WithTitle($"{user.Username} you have leveled up!");
                        embed.WithDescription($"{oldLevel} - {newLevel}");
                        embed.AddInlineField("Level", newLevel);
                        embed.AddInlineField("XP", userAccount.XP);
                        embed.WithColor(new Color(67, 160, 71));

                        await channel.SendMessageAsync("", false, embed);
                    }
                }
            }
            catch { /* No Permission */ }
        }
    }
}
