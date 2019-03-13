using System;
using System.Xml;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace AHH_Bot.Commands
{
    public class Assignments : ModuleBase<SocketCommandContext>
    {
        [Command("assignment"), Alias("todo", "to do")]
        public async Task Assignment()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("assignments.xml");
            XmlNodeList nodes = doc.DocumentElement.SelectNodes(@"/todo/assignment");

            foreach (XmlNode node in nodes)
            {
                var builder = new EmbedBuilder()
                    .WithTitle(node.SelectSingleNode("name").InnerText)
                    .WithDescription($"Assignment instructions can be found [here]({node.SelectSingleNode("url").InnerText})")
                    .WithColor(new Color(0xB4B227))
                    .WithTimestamp(DateTime.Now)
                    .WithFooter(footer => {footer.WithText(node.SelectSingleNode("class").InnerText)
                            .WithIconUrl("https://camo.githubusercontent.com/0617f4657fef12e8d16db45b8d73def73144b09f/68747470733a2f2f646576656c6f7065722e6665646f726170726f6a6563742e6f72672f7374617469632f6c6f676f2f6373686172702e706e67");})
                    .WithImageUrl(node.SelectSingleNode("imageurl").InnerText)
                    .WithAuthor(author => {author.WithName(node.SelectSingleNode("teacher").InnerText).WithUrl(node.SelectSingleNode("teacherurl").InnerText).WithIconUrl(node.SelectSingleNode("teacherpic").InnerText);})
                    .AddField("Due Date :calendar_spiral:", $"This assignment is due on **{node.SelectSingleNode("due").InnerText}**");

                string temp = "";
                for (int i = 0; i < node.SelectSingleNode("questioncount").InnerText.Length; i++)
                    temp = temp + node.SelectSingleNode("questioncount").InnerText[i] + "\u20E3";
                builder.AddField("Question Count", temp, true);

                temp = "";
                for (int i = 0; i < node.SelectSingleNode("flowcharts").InnerText.Length; i++)
                    temp = temp + node.SelectSingleNode("flowcharts").InnerText[i] + "\u20E3";
                builder.AddField("Flowchart Count", temp, true);

                for (int i = 0; i < node.SelectSingleNode("questions").ChildNodes.Count; i++)
                    builder.AddField($"Question {i + 1}:", node.SelectSingleNode("questions").SelectSingleNode($"q{i + 1}").InnerText);

                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
            }
        }
    }
}
