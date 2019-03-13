using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using AHH_Bot.Database;

namespace AHH_Bot
{
    public class AhhBot
    {
        private DiscordSocketClient _client;
        private CommandService _commands;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 250
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            });

            // Event Handlers
            _client.MessageReceived += MessageReceived;
            _client.Ready += ClientReady;
            _client.Log += ClientLog;
            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await _client.LoginAsync(TokenType.Bot, Settings.BotToken);
            await _client.StartAsync();

            // Preven task from ending
            await Task.Delay(-1);
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await Data.Users.Messages.AddReaction(reaction.UserId, 1);
            foreach (var vote in Data.CurrentVotes)
            {
                if (reaction.MessageId == vote.ID)
                {
                    if (vote.isYesNo)
                    {
                        if (reaction.Emote.Name == "✅")
                            vote.Results[0] += 1;
                        else if (reaction.Emote.Name == "❌")
                            vote.Results[1] += 1;
                    }
                    else
                    {
                        if (new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9}.Contains(char.GetNumericValue(reaction.Emote.Name[0])))
                            vote.Results[(int)char.GetNumericValue(reaction.Emote.Name[0]) - 1]++;
                    }

                    if (reaction.Emote.Name == "🗑" && reaction.UserId == vote.UserID)
                    {
                        await channel.DeleteMessageAsync(reaction.MessageId);
                    }
                }
            }
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await Data.Users.Messages.AddReaction(reaction.UserId, -1);
            foreach (var vote in Data.CurrentVotes)
            {
                if (reaction.MessageId == vote.ID)
                {
                    if (vote.isYesNo)
                    {
                        if (reaction.Emote.Name == "✅")
                            vote.Results[0] -= 1;
                        else if (reaction.Emote.Name == "❌")
                            vote.Results[1] -= 1;
                    }
                    else
                    {
                        if (new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Contains(char.GetNumericValue(reaction.Emote.Name[0])))
                            vote.Results[(int)char.GetNumericValue(reaction.Emote.Name[0]) - 1]--;
                    }
                }
            }
        }

        private async Task ClientLog(LogMessage message)
        {
            Console.WriteLine($"{DateTime.Now} | {message.Source} | {message.Message}");
        }

        private async Task ClientReady()
        {
        }

        private async Task MessageReceived(SocketMessage socketMessage)
        {
            SocketUserMessage Message = socketMessage as SocketUserMessage;
            SocketCommandContext Context = new SocketCommandContext(_client, Message);

            //Checks if message is empty
            if (Context.Message == null || string.IsNullOrWhiteSpace(Context.Message.Content))
                return;

            //Check if user is a bot
            if (Context.User.IsBot)
                return;

            //Checks if message starts with specified prefix
            int prefixLocation = 0;
            char prefixChar = Settings.CmdPrefix;
            if (!(Message.HasCharPrefix(prefixChar, ref prefixLocation) || Message.HasMentionPrefix(_client.CurrentUser, ref prefixLocation)))
            {
                await Data.Users.Messages.AddMessages(Context.User.Id, 1);
                return;
            }

            var result = await _commands.ExecuteAsync(Context, prefixLocation, null);
            if (!result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} | Command Error. | Input: {Context.Message.Content} | Error: {result.ErrorReason}");
            else
                await Data.Users.Messages.AddExecutedCommand(Context.User.Id, 1);
        }
    }
}
