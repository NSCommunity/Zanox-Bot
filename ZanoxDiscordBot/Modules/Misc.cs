using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace ZanoxDiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {

        [Command("announcement")]
        public async Task Announcement([Remainder]string message)
        {
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

        [Command("8ball")]
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
    }
}