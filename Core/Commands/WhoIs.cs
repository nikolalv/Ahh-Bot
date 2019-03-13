using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using AHH_Bot.Database;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;

namespace AHH_Bot.Commands
{
    [Group("whois")]
    public class WhoIs : ModuleBase<SocketCommandContext>
    {
        [Command("")]
        public async Task WhoIsDefault(IUser inputUser = null)
        {
            if (inputUser == null)
                inputUser = Context.User;

            var user = inputUser as SocketGuildUser;

            var embed = new EmbedBuilder()
                .WithTitle("Displaying Stats for "+user.Nickname)
                .WithColor(66, 244, 134)
                .WithThumbnailUrl(user.GetAvatarUrl())
                .AddField("Total Chocolates :chocolate_bar:", Data.Chocolates.GetChocolateAmount(user.Id), true)
                .AddField("Messages Sent :e_mail:", Data.Users.Messages.GetSentMsgAmount(user.Id), true)
                .AddField("Votes Created :white_check_mark:", Data.Users.Messages.GetTotalVoteAmount(user.Id), true)
                .AddField("Reaction Count 🔥", Data.Users.Messages.GetReactionAmount(user.Id), true)
                .AddField("Commands Ran :desktop:", Data.Users.Messages.GetExecutedCmdAmount(user.Id), true)
                .AddField("Challenges Completed 🤔", Data.Users.GetCompletedChallenges(user.Id), true)
                .AddField("Member Since", user.JoinedAt.Value.Date.ToLongDateString(), true)
                .AddField("Roles", user.Roles.Where(x => x.Name != "@everyone").Join("\n"), true)
                .WithFooter("Stats as of: ", "http://www.johnabbott.qc.ca/wp-content/uploads/2017/10/Islanders-logo_250x250.jpg")
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("")]
        public async Task WhoIsRole(IRole inputRole)
        {
            var role = inputRole as SocketRole;

            var embed = new EmbedBuilder()
                .WithTitle($"Displaying Details for the ***{role.Name}*** rank")
                .WithColor(role.Color)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Created on", role.CreatedAt.Date.ToLongDateString())
                .AddField("Members with this role", role.Members.Join("\n"))
                .WithFooter("Stats as of: ", "http://www.johnabbott.qc.ca/wp-content/uploads/2017/10/Islanders-logo_250x250.jpg")
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
