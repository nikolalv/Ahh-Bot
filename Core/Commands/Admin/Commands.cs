﻿using System.Threading.Tasks;
using Discord.Commands;

namespace AHH_Bot.Core.Commands.Admin
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("announce")]
        [RequireOwner]
        public async Task Announce([Remainder] string message)
        {
            foreach (var guilds in Context.Client.Guilds)
            {
                await guilds.DefaultChannel.SendMessageAsync(message);
            }
        }
    }
}
