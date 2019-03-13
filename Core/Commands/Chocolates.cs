using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using AHH_Bot.Database;

namespace AHH_Bot.Commands
{
    [Group("chocolates"), Alias("choco", "credits", "cocoacoin", "chocolate")]
    public class Chocolates : ModuleBase<SocketCommandContext>
    {
        [Command("")]
        public async Task chococredits(IUser user = null)
        {
            if (user == null)
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} you currently have collected {Data.Chocolates.GetChocolateAmount(Context.User.Id)} chocolates :chocolate_bar:");
            else
                await Context.Channel.SendMessageAsync($"{user.Mention} has collected {Data.Chocolates.GetChocolateAmount(user.Id)} chocolates :chocolate_bar:");
        }

        [Command("add")]
        public async Task spawnChoco(IUser user = null, int chocoAmount = 0)
        {
            if (user == null)
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: You must specify a user");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: You do not have the required permissions!");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (chocoAmount <= 0)
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: You must input a valid number");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            await Data.Chocolates.AddChocolates(user.Id, chocoAmount);
            await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully added {chocoAmount} to {user.Mention}'s account.");
        }

        [Command("gift"), Alias("give")]
        public async Task giftChoco(IUser user = null, int chocoAmount = 0)
        {
            if (user == null)
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: You must specify a user");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (user.IsBot)
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: User cannot be a bot!");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (chocoAmount <= 0)
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: You must input a valid number");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (Data.Chocolates.GetChocolateAmount(Context.User.Id) - chocoAmount < 0)
            {
                var errormsg = await Context.Channel.SendMessageAsync($":x: You must at least have {chocoAmount} chocolates!");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            await Context.Channel.SendMessageAsync($":chocolate_bar: You have successfully sent {user.Mention} {chocoAmount} chocolates.");
            await Data.Chocolates.AddChocolates(Context.User.Id, -chocoAmount);
            await Data.Chocolates.AddChocolates(user.Id, chocoAmount);
        }

        [Command("top"), Alias("baltop", "balancetop")]
        public async Task GetTop(int top = 3)
        {
            if (top < 1)
            {
                await Context.Channel.SendMessageAsync(":x: Input a valid number.");
                return;
            }

            if (top > 50)
            {
                await Context.Channel.SendMessageAsync(":x: Number must be under 50.");
                return;
            }

            string temp = "";
            foreach (var user in Data.Chocolates.GetTopChoco(top))
            {
                var guildUser = Context.Client.GetUser(user);
                if (!guildUser.IsBot)
                    temp += $"{guildUser.Username} - {Data.Chocolates.GetChocolateAmount(user)}\n";
            }
            
            await Context.Channel.SendMessageAsync("```\n" + temp + "```");
        }

        [Group("Reset")]
        public class Reset : Chocolates
        {
            [Command("")]
            public async Task SelfReset()
            {
                await Data.Chocolates.SetChocolates(Context.User.Id, 0);
            }

            [Command("")]
            public async Task UserReset(IUser user = null)
            {
                if (user == null)
                    return;

                if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: You do not have the required permissions!");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                await Data.Chocolates.SetChocolates(user.Id, 0);

            }

            [Command("all")]
            [RequireOwner]
            public async Task GuildReset()
            {
                foreach (var user in Context.Guild.Users)
                {
                    if(!user.IsBot)
                        await Data.Chocolates.SetChocolates(user.Id, 0);
                }
            }
        }
    }
}
