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
    public class Special : ModuleBase<SocketCommandContext>
    {
        private HttpClient tweetClient = new HttpClient();

        [Command("z!tweet")]
        public async Task tweetFromZanox([Remainder]string input)
        {
            if (Program.ZanoxAdmins.Contains(Context.User.Id))
            {
                Context.Channel.SendMessageAsync("Sending tweet... Please was a few seconds");
                var embed = new EmbedBuilder();
                embed.WithTitle("Tweeting from ZanoxTweets");
                embed.WithImageUrl("https://cdn2.iconfinder.com/data/icons/minimalism/512/twitter.png");
                embed.AddField("Tweet Sent!", input);
                var values = new Dictionary<string, string> { { "value1", input } };

                var content = new FormUrlEncodedContent(values);
                var response = await tweetClient.PostAsync("https://maker.ifttt.com/trigger/tweet/with/key/fo1dRsVnxv8poyjrFkxUo2qzvAZbI4gZ6DL4E6E9eN4", content);
                Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                Context.Channel.SendMessageAsync("You don't have permission to use this command");
            }
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
                Misc.ExceptionAlert(Context, e);
            }
        }
    }
}
