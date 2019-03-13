using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHH_Bot.Commands
{
    public class GameStats : ModuleBase<SocketCommandContext>
    {
        [Command("apex")]
        public async Task ApexStats(string input = "")
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                RestUserMessage errormsg = await Context.Channel.SendMessageAsync(":x: You must specify a player");
                await Task.Delay(30000);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            string response;
            using (var client = new HttpClient())
            {
                var url = $"https://public-api.tracker.gg/apex/v1/standard/profile/5/{input}";
                client.DefaultRequestHeaders.Add("TRN-Api-Key", Settings.ApexAPI);
                try
                {
                    response = client.GetStringAsync(url).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DateTime.Now} | Game Stats | {e.Message}");
                    RestUserMessage errormsg;
                    if (e.HResult == -2146233088)
                        errormsg = await Context.Channel.SendMessageAsync(":x: This player was not found.");
                    else
                        errormsg = await Context.Channel.SendMessageAsync(":x: Unable to fetch player stats at the moment.");
                    await Task.Delay(30000);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }
            }

            JObject json = JObject.Parse(response);
            var playerData = json.SelectToken("data.metadata");

            var embed = new EmbedBuilder()
                .WithTitle("Apex Legends Stats for: " + playerData.SelectToken("platformUserHandle"))
                .WithColor(66, 244, 134)
                .WithThumbnailUrl("https://ih0.redbubble.net/image.747668209.3756/flat,550x550,075,f.u2.jpg");

            foreach (var node in json.SelectToken("data.stats"))
            {
                embed.AddField(node.SelectToken("metadata.name").ToString(), $"{node.SelectToken("displayValue")}\n Top {node.SelectToken("percentile")}%", true);
            }

            var msg = await Context.Channel.SendMessageAsync(null, false, embed.Build());

            foreach (var node in json.SelectToken("data.children"))
            {
                embed = new EmbedBuilder()
                    .WithTitle($"{playerData.SelectToken("platformUserHandle")}'s stats for {node.SelectToken("metadata.legend_name")}")
                    .WithColor(66, 244, 134)
                    .WithThumbnailUrl(node.SelectToken("metadata.icon").ToString());

                foreach (var statnode in node.SelectToken("stats"))
                    embed.AddField(statnode.SelectToken("metadata.name").ToString(), $"{statnode.SelectToken("displayValue")}\nTop {statnode.SelectToken("percentile")}%", true);

                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }
    }
}
