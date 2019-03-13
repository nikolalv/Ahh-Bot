using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;

namespace AHH_Bot.Commands
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync($"Pong! Latency: {Context.Client.Latency}ms");
        }

        [Command("clear")]
        public async Task Clear(int amount = -1)
        {
            if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
            {
                await Context.Channel.SendMessageAsync(":x: User must be an administrator");
                return;
            }

            if (amount < 1 || amount > 500)
            {
                await Context.Channel.SendMessageAsync(":x: Please input a valid number between 1 and 500");
                return;
            }

            foreach (var msg in Context.Channel.GetCachedMessages(amount))
            {
                await Task.Delay(275);
                await msg.DeleteAsync();
            }

            await Context.Channel.SendMessageAsync($":white_check_mark: Successfully deleted {amount} messages.");
        }

        [Command("getpic"), Alias("dp")]
        public async Task GetProfilePic(IUser user = null)
        {
            if (user == null)
                user = Context.User;
            await Context.Channel.SendMessageAsync(user.GetAvatarUrl());
        }
    }
}