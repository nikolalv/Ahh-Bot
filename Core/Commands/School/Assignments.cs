using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace AHH_Bot.Commands
{
    public class Assignments : ModuleBase<SocketCommandContext>
    {
        [Command("assignment"), Alias("todo")]
        public async Task Assignment()
        {
            var googleDriveXML = await new WebClient().DownloadStringTaskAsync(new Uri("https://drive.google.com/uc?export=download&id=1gPjWdD_5IJFFvfLInaIYNGTbOu3kkILG"));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(googleDriveXML);
            XmlNodeList nodes = doc.DocumentElement.SelectNodes(@"/todo/assignment");

            foreach (XmlNode node in nodes)
            {
                var embed = new EmbedBuilder()
                    .WithTitle(node.SelectSingleNode("name").InnerText)
                    .WithDescription($"Assignment instructions can be found [here]({node.SelectSingleNode("url").InnerText})")
                    .WithColor(66, 244, 134)
                    .WithTimestamp(DateTime.Now)
                    .AddField("Due Date :calendar_spiral:", $"This assignment is due on **{node.SelectSingleNode("due").InnerText}**")
                    .WithAuthor(x =>
                    {
                        x.Name = node.SelectSingleNode("teacher").InnerText;
                        x.Url = node.SelectSingleNode("teacherurl").InnerText;
                        x.IconUrl = node.SelectSingleNode("teacherpic").InnerText;
                    })
                    .WithFooter(x =>
                    {
                        x.Text = node.SelectSingleNode("class").InnerText;
                        x.IconUrl = "https://camo.githubusercontent.com/0617f4657fef12e8d16db45b8d73def73144b09f/68747470733a2f2f646576656c6f7065722e6665646f726170726f6a6563742e6f72672f7374617469632f6c6f676f2f6373686172702e706e67";
                    });

                //If image url node is found add it
                if (node.InnerXml.Contains("imageurl"))
                    embed.ImageUrl = node.SelectSingleNode("imageurl").InnerText;

                foreach (XmlNode fieldNode in node.SelectNodes("field"))
                {
                    embed.AddField(x =>
                    {
                        x.Name = fieldNode.Attributes["title"].InnerText;
                        x.Value = fieldNode.InnerText;
                        x.IsInline = bool.Parse(fieldNode.Attributes["inline"].InnerText);
                    });
                }

                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }
    }
}