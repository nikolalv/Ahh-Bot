using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using AHH_Bot.Database;

namespace AHH_Bot.Commands
{
    public class Voting : ModuleBase<SocketCommandContext>
    {
        [Command("Vote")]
        public async Task NewVote([Remainder] string input)
        {
            int voteDuration = Settings.VoteDuration;
            string[] vote = input.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (vote[0].Length >= 150)
            {
                RestUserMessage errormsg = await Context.Channel.SendMessageAsync(":x: Your vote title cannot exceed 150 characters.");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            /*if (voteDuration < 30)
            {
                RestUserMessage errormsg = await Context.Channel.SendMessageAsync(":x: Vote must last longer than 30 seconds.");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (voteDuration > 18000)
            {
                RestUserMessage errormsg = await Context.Channel.SendMessageAsync(":x: Vote duration cannot excede 5 hours.");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }*/

            var embed = new EmbedBuilder()
            .WithColor(new Color(81, 207, 148))
            .WithAuthor($"Vote created by: {Context.User.Username}", Context.User.GetAvatarUrl())
            .WithTitle(vote[0]);

            if (vote.Length == 1)
            {
                embed.AddField("Answer Below", "\u200b");
            }
            else
            {
                if (vote.Length > 10)
                {
                    RestUserMessage errormsg = await Context.Channel.SendMessageAsync(":x: Your vote cannot have more than 9\u20E3 options.");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                if (vote.Length == 2)
                {
                    RestUserMessage errormsg = await Context.Channel.SendMessageAsync(":x: You are attempting a dictatorship. (Please give more than 1\u20E3 option to your vote)");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                for (int i = 1; i < vote.Length; i++)
                {
                    embed.AddField($"Option {i}", vote[i], true);
                }
            }

            RestUserMessage voteMessage = await Context.Channel.SendMessageAsync("", false, embed.Build());
            Vote currentVote = new Vote(voteMessage.Id, Context.User.Id);
            Data.CurrentVotes.Add(currentVote);

            //Add a vote to user stats
            await Data.Users.Messages.VotesAddUserVoteStat(Context.User.Id, 1);

            if (vote.Length == 1)
            {
                currentVote.Results = new int[2];
                currentVote.isYesNo = true;
                await voteMessage.AddReactionAsync(new Emoji("✅"));
                await voteMessage.AddReactionAsync(new Emoji("❌"));
            }
            else
            {
                currentVote.Results = new int[vote.Length - 1];
                for (int i = 1; i < vote.Length; i++)
                {
                    await voteMessage.AddReactionAsync(new Emoji($"{i}\u20E3"));
                }
            }

            await voteMessage.AddReactionAsync(new Emoji("🗑"));

            await Task.Delay(voteDuration*1000);

            Data.CurrentVotes.Remove(currentVote);

            try
            {
                await voteMessage.DeleteAsync();
            }
            catch
            {
                Console.WriteLine($"{DateTime.Now} | Vote | Attempted to delete message that no longer exists");
            }

            embed = new EmbedBuilder()
                .WithColor(new Color(81, 207, 148))
                .WithAuthor($"Vote created by: {Context.User.Username}", Context.User.GetAvatarUrl())
                .WithTitle(vote[0]);

            if (currentVote.Results.Sum() == currentVote.Results.Length)
            {
                embed.AddField("Result", "No one has voted!");
            }
            else
            {
                if (currentVote.Results.Count(x => x == currentVote.Results.Max()) > 1)
                {
                    if (currentVote.isYesNo)
                    {
                        embed.AddField("Result", "It's a tie!");
                    }
                    else
                    {
                        embed.AddField("Result", "**The population has not decided on an answer and has instead settled with a tie.**");
                        for (int i = 0; i < currentVote.Results.Length; i++)
                        {
                            embed.AddField($"***{vote[i + 1]}***", $"Has received **{currentVote.Results[i] - 1}** vote" + ((currentVote.Results[i] == 2) ? "" : "s"), true);
                        }
                    }
                }
                else
                {
                    if (currentVote.isYesNo)
                    {
                        if (currentVote.Results[0] > currentVote.Results[1])
                            embed.AddField("Result", "The population has decided that the answer to this question is **Yes**");
                        else
                            embed.AddField("Result", "The population has decided that the answer to this question is **No**");
                    }
                    else
                    {
                        embed.AddField("Result", $"The population has decided that the answer to this question is: **{vote[Array.IndexOf(currentVote.Results, currentVote.Results.Max()) + 1]}**");
                    }
                }
            }

            await Context.Channel.SendMessageAsync(null, false, embed.Build());

        }

        public class Vote
        {
            public bool isYesNo = false;
            public ulong ID;
            public ulong UserID;
            public int[] Results;

            public Vote(ulong ID, ulong UserID)
            {
                this.ID = ID;
                this.UserID = UserID;
            }
        }
    }
}
