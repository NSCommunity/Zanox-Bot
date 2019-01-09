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
    public class Hypixel : ModuleBase<SocketCommandContext>
    {
        [Command("z!hypixel gameCounts")]
        public async Task gameCount()
        {
            Context.Channel.SendMessageAsync("f");
            string json = Misc.getStringFromUrl("https://api.hypixel.net/gamecounts?key=eac7da68-53e0-4e13-a5ac-c50b71178f97");
            var data = (JObject)JsonConvert.DeserializeObject(json);
            EmbedBuilder embed = new EmbedBuilder();

            var roles = (Context.User as SocketGuildUser).Roles.ToList();
            embed.WithColor(roles[2].Color);
            Console.WriteLine(roles[0].Name);
            Console.WriteLine(roles[1].Name);
            Console.WriteLine(roles[2].Name);
            Console.WriteLine(roles[3].Name);
            Console.WriteLine(roles[4].Name);
            Console.WriteLine(roles[5].Name);
            Console.WriteLine(roles[6].Name);

            embed.AddField("Main Lobby", data["games"]["MAIN_LOBBY"]["players"].Value<string>() + " players");
            embed.AddInlineField("Bed Wars", data["games"]["BEDWARS"]["players"].Value<string>() + " players");
            embed.AddInlineField("SkyWars", data["games"]["SKYWARS"]["players"].Value<string>() + " players");
            embed.AddInlineField("Housing", data["games"]["HOUSING"]["players"].Value<string>() + " players");
            embed.AddInlineField("Murder Mystery", data["games"]["MURDER_MYSTERY"]["players"].Value<string>() + " players");
            embed.AddInlineField("Arcade", data["games"]["ARCADE"]["players"].Value<string>() + " players");
            embed.AddInlineField("Duels", data["games"]["DUELS"]["players"].Value<string>() + " players");
            embed.AddInlineField("Prototype", data["games"]["PROTOTYPE"]["players"].Value<string>() + " players");
            embed.AddInlineField("Blitz SG", data["games"]["SURVIVAL_GAMES"]["players"].Value<string>() + " players");
            embed.AddInlineField("TNT Games", data["games"]["TNTGAMES"]["players"].Value<string>() + " players");
            embed.AddInlineField("UHC", data["games"]["UHC"]["players"].Value<string>() + " players");
            embed.AddInlineField("Mega Walls", data["games"]["WALLS3"]["players"].Value<string>() + " players");
            embed.AddInlineField("Classic Games", data["games"]["LEGACY"]["players"].Value<string>() + " players");
            embed.AddInlineField("Cops & Crims", data["games"]["MCGO"]["players"].Value<string>() + " players");
            embed.AddInlineField("Warlords", data["games"]["BATTLEGROUND"]["players"].Value<string>() + " players");
            embed.AddInlineField("Speed UHC", data["games"]["SPEED_UHC"]["players"].Value<string>() + " players");
            embed.AddInlineField("Super Smash Heroes", data["games"]["SUPER_SMASH"]["players"].Value<string>() + " players");
            embed.AddInlineField("Crazy Walls", data["games"]["TRUE_COMBAT"]["players"].Value<string>() + " players");
            embed.AddInlineField("Skyclash", data["games"]["SKYCLASH"]["players"].Value<string>() + " players");

            Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("z!hypixel gameCount")]
        public async Task gameCountSpecific([Remainder]string input)
        {
            try
            {
                input = input.ToLower();
                input = Regex.Replace(input, @"\s+", "");
                input = Regex.Replace(input, "_", "");
                var inpjson = Misc.getStringFromUrl("https://api.hypixel.net/gamecounts?key=eac7da68-53e0-4e13-a5ac-c50b71178f97");

                Misc.saveToFile("gamecounts.json", inpjson);

                string json = Misc.stringFromFile("gamecounts.json");

                var data = (JObject)JsonConvert.DeserializeObject(json);

                var embed = new EmbedBuilder();

                bool x = true;
                while (x)
                {
                    x = false;
                    if (input == "mainlobby")
                    {
                        embed.WithTitle("Main Lobby");
                        embed.AddField("Total players", data["games"]["MAIN_LOBBY"]["players"].Value<string>());
                        continue;
                    }
                    if (input == "bedwars")
                    {
                        embed.WithTitle("Bed Wars");
                        embed.AddField("Total players", data["games"]["BEDWARS"]["players"].Value<string>());
                        embed.AddInlineField("4v4v4v4", data["games"]["BEDWARS"]["modes"]["BEDWARS_FOUR_FOUR"].Value<string>());
                        embed.AddInlineField("3v3v3v3", data["games"]["BEDWARS"]["modes"]["BEDWARS_FOUR_THREE"].Value<string>());
                        embed.AddInlineField("Doubles", data["games"]["BEDWARS"]["modes"]["BEDWARS_EIGHT_TWO"].Value<string>());
                        embed.AddInlineField("Solo", data["games"]["BEDWARS"]["modes"]["BEDWARS_EIGHT_ONE"].Value<string>());
                        embed.AddInlineField("Castle", data["games"]["BEDWARS"]["modes"]["BEDWARS_CASTLE"].Value<string>());
                        embed.AddInlineField("Capture", data["games"]["BEDWARS"]["modes"]["BEDWARS_CAPTURE"].Value<string>());
                        continue;
                    }
                    if (input == "skywars")
                    {
                        embed.WithTitle("SkyWars");
                        try { embed.AddField("Total players", data["games"]["SKYWARS"]["players"].Value<string>()); } catch { embed.AddInlineField("Total Players", "0"); }
                        try { embed.AddInlineField("Solo Normal", data["games"]["SKYWARS"]["modes"]["solo_normal"].Value<string>()); } catch { embed.AddInlineField("Solo Normal", "0"); }
                        try { embed.AddInlineField("Solo Insane", data["games"]["SKYWARS"]["modes"]["solo_insane"].Value<string>()); } catch { embed.AddInlineField("Solo Insane", "0"); }
                        try { embed.AddInlineField("Solo Rush", data["games"]["SKYWARS"]["modes"]["solo_insane_rush"].Value<string>()); } catch { embed.AddInlineField("Solo Rush", "0"); }
                        try { embed.AddInlineField("Solo Lucky", data["games"]["SKYWARS"]["modes"]["solo_insane_lucky"].Value<string>()); } catch { embed.AddInlineField("Solo Lucky", "0"); }
                        try { embed.AddInlineField("Solo Slime", data["games"]["SKYWARS"]["modes"]["solo_insane_slime"].Value<string>()); } catch { embed.AddInlineField("Solo Slime", "0"); }
                        try { embed.AddInlineField("Solo TNT Madness", data["games"]["SKYWARS"]["modes"]["solo_insane_tnt_madness"].Value<string>()); } catch { embed.AddInlineField("Solo TNT Madness", "0"); }
                        try { embed.AddInlineField("Solo Hunters vs Beasts", data["games"]["SKYWARS"]["modes"]["solo_insane_hunters_vs_beasts"].Value<string>()); } catch { embed.AddInlineField("Solo Hunters vs Beasts", "0"); }
                        try { embed.AddInlineField("Teams Normal", data["games"]["SKYWARS"]["modes"]["teams_normal"].Value<string>()); } catch { embed.AddInlineField("Teams Normal", "0"); }
                        try { embed.AddInlineField("Teams Insane", data["games"]["SKYWARS"]["modes"]["teams_insane"].Value<string>()); } catch { embed.AddInlineField("Teams Insane", "0"); }
                        try { embed.AddInlineField("Teams Rush", data["games"]["SKYWARS"]["modes"]["teams_insane_rush"].Value<string>()); } catch { embed.AddInlineField("Teams Rush", "0"); }
                        try { embed.AddInlineField("Teams Lucky", data["games"]["SKYWARS"]["modes"]["teams_insane_lucky"].Value<string>()); } catch { embed.AddInlineField("Teams Lucky", "0"); }
                        try { embed.AddInlineField("Ranked", data["games"]["SKYWARS"]["modes"]["ranked_normal"].Value<string>()); } catch { embed.AddInlineField("Ranked", "0"); }
                        try { embed.AddInlineField("Mega", data["games"]["SKYWARS"]["modes"]["mega_doubles"].Value<string>()); } catch { embed.AddInlineField("Mega", "0"); }
                        continue;
                    }
                    embed.WithTitle("Gamemode not found :/");
                }
                Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                var asdasd = new StackTrace(e, true);
                // Get the top stack frame
                var sdg = asdasd.GetFrame(0);
                // Get the line number from the stack frame
                Console.WriteLine(sdg.GetFileLineNumber());
                Misc.ExceptionAlert(Context, e);
            }
        }
    }
}
