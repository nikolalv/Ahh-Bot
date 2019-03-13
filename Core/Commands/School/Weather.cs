using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;

namespace AHH_Bot.Commands
{
    public class Weather : ModuleBase<SocketCommandContext>
    {
        [Command("SchoolStatus"), Alias("JAC", "school", "yeet?")]
        public async Task WeatherYeet()
        {
            var xmlWeather = await new WebClient().DownloadStringTaskAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=Sainte-Anne-de-Bellevue&units=metric&mode=xml&APPID=" + Settings.WeatherAPI));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlWeather);
            string currentTemp = doc.DocumentElement.SelectSingleNode("temperature").Attributes["value"].Value;
            string minTemp = doc.DocumentElement.SelectSingleNode("temperature").Attributes["min"].Value;
            string icon = doc.DocumentElement.SelectSingleNode("weather").Attributes["icon"].Value;
            string location = doc.DocumentElement.SelectSingleNode("city").Attributes["name"].Value;
            string tmr = DateTime.Now.AddDays(1).ToLongDateString();
            Color tempColor;

            if (double.Parse(currentTemp) < 0)
                tempColor = new Color(66, 215, 244);
            else
                tempColor = new Color(247, 34, 91);

            var builder = new EmbedBuilder()
                .WithTitle("Will John Abbott College be open tomorrow?")
                .WithDescription(tmr)
                .WithColor(tempColor)
                .WithTimestamp(DateTime.Now)
                .WithThumbnailUrl("http://openweathermap.org/img/w/"+icon+".png")
                .AddField("Your Safety Matters!", "Please note that if the College had to close due to adverse weather conditions, the Communications department would post a message on the JAC web site and on My JAC Portal.")
                .WithFooter(footer => { footer.WithText($"Weather taken from {location}"); });

            if (double.Parse(minTemp) < -20)
                builder.AddField("***Current School Status***", $"Due to the severe weather conditions John Abbott College will be closed {tmr}. We believe that {minTemp} Celcius could be harmful for our students");
            else
                builder.AddField("***Current School Status***", $"John Abbott College will remain open on {tmr}. We believe the weather is more than optimal for getting an education");

            builder.AddField("Current Temperature:", currentTemp + " Celcius", true);
            builder.AddField("Daily Minimum:", minTemp + " Celcius", true);

            await Context.Channel.SendMessageAsync(null, embed: builder.Build()).ConfigureAwait(false);
        }
    }


}