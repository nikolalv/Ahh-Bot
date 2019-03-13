using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;

namespace AHH_Bot.Commands
{
    public class ProgrammingHelp : ModuleBase<SocketCommandContext>
    {
        [Group("TryParse")]
        public class TryParse : ModuleBase<SocketCommandContext>
        {
            [Command("")] // Set up overloads for bool and other types
            public async Task TryParseMenu()
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Input validation using TryParse")
                    .WithColor(new Color(28, 221, 163))
                    .WithFooter(footer => { footer
                            .WithIconUrl("https://camo.githubusercontent.com/0617f4657fef12e8d16db45b8d73def73144b09f/68747470733a2f2f646576656c6f7065722e6665646f726170726f6a6563742e6f72672f7374617469632f6c6f676f2f6373686172702e706e67");})
                    .AddField("TryParse", "Converts user input into specified type and returns a value that indicates whether it has successfully converted or not.")
                    .AddField("\u200b", "\u200b")
                    .AddField("For a code example enter the type after the initial command", "Example: !tryparse \"type\"")
                    .AddField("\u200b", "Here is the flowchart for a basic integer TryParse")
                    .WithImageUrl("https://i.imgur.com/nR7yLPV.png");

                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
            }

            [Command("")]
            public async Task TypeTryParse([Remainder] string type)
            {
                string[] types = {"int", "double", "char", "ulong", "long", "int", "float", "byte", "sbyte", "short", "ushort", "decimal", "datetime"};
                bool isValid = false;
                type = type.ToLower();
                for (int i = 0; i < types.Length; i++)
                {
                    if (type == types[i])
                        isValid = true;
                }

                if (!isValid)
                {
                    var m = await Context.Channel.SendMessageAsync($"Type \"{type}\" is currently not supported by Aref");
                    await Task.Delay(15000);
                    await m.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    return;
                }
                //ToTitleCase is going to break this but that's ok
                if (type == "datetime")
                    type = "DateTime";

                string example1 =
                    $"public static {type} {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type)}TryParse(string message, {type} lowerLimit = {type}.MinValue, {type} upperLimit = {type}.MaxValue)\n" +
                    "{\n" +
                    "    Console.Write(message);\n" +
                    $"    bool userInput = {type}.TryParse(Console.ReadLine(), out {type} parsedInput);\n\n" +
                    "    while (!userInput || parsedInput < lowerLimit || parsedInput > upperLimit)\n" +
                    "    {\n" +
                    "        Console.ForegroundColor = ConsoleColor.Red;\n" +
                    $"        Console.WriteLine($\"Must be a valid value located between {{lowerLimit}} and {{upperLimit}} inclusively.\");\n" +
                    "        Console.ResetColor();\n" +
                    "        Console.Write(message);\n" +
                    $"        userInput = {type}.TryParse(Console.ReadLine(), out parsedInput);\n" +
                    "    }\n" +
                    "    return parsedInput;\n" +
                    "}";

                string example2 =
                    $"{type} number;\n" +
                    "bool valid;\n\n" +
                    "Console.WriteLine(\"Please enter a positive number\");\n" +
                    $"valid = {type}.TryParse(Console.ReadLine(), out number);\n\n" +
                    "while (!valid || number < 0)\n\n" +
                    "{" +
                    "	Console.WriteLine(\"Error: number is not valid or\n"+
                    "	negative. Please enter a positive number\");\n"+
                    $"	valid = {type}.TryParse(Console.ReadLine(), out number);\n" +
                    "}\n\n" +
                    "Console.WriteLine(\"The number is {0}\", number);";

                var builder = new EmbedBuilder()
                    .WithTitle($"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type)} validation using TryParse")
                    .WithColor(new Color(28, 221, 163))
                    .WithFooter(footer => {footer.WithIconUrl("https://camo.githubusercontent.com/0617f4657fef12e8d16db45b8d73def73144b09f/68747470733a2f2f646576656c6f7065722e6665646f726170726f6a6563742e6f72672f7374617469632f6c6f676f2f6373686172702e706e67");})
                    .AddField("TryParse", "Tries to convert the input into the specified type and returns a value that indicates whether it has successfully converted or not.")
                    .AddField("Example 1", $"This validation lets the user set a minimum and maximum for the value and if that value is not specified it will default to the min and max of type {type}.");

                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                await Context.Channel.SendMessageAsync("```cs\n" + example1 + "\n```");

                builder = new EmbedBuilder()
                    .AddField("Example 2", "This validation also checks whether the number is positive or not.")
                    .WithColor(new Color(28, 221, 163));
                embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                await Context.Channel.SendMessageAsync("```cs\n" + example2 + "\n```");
            }
        }
    }
}
