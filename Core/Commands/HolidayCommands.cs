using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace AHH_Bot.Commands
{
    public class HolidayCommands : ModuleBase<SocketCommandContext>
    {
        [Command("valentine"), Alias("daddy", "papi")]
        public async Task Valentine(IUser user, [Remainder] string message)
        {
            await Context.Message.DeleteAsync();
            if (Context.User.Id == user.Id)
            {
                await Context.User.SendMessageAsync("You cannot send yourself a rose. Someone will come around eventually. :wilted_rose: ");
                return;
            }
            await user.SendMessageAsync($"Someone from {Context.Guild.Name} has sent you a :rose: :heart: With the following message: {message}");
        }

        [Command("valentine"), Alias("daddy", "papi")]
        public async Task Valentine2(IUser user)
        {
            await Context.Message.DeleteAsync();
            if (Context.User.Id == user.Id)
            {
                await Context.User.SendMessageAsync("You cannot send yourself a rose. Someone will come around eventually. :wilted_rose: ");
                return;
            }
            await user.SendMessageAsync($"Someone from {Context.Guild.Name} has sent you a :rose: :heart:");
        }
    }
}