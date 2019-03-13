using System;
using System.Linq;
using System.Threading.Tasks;
using AHH_Bot.Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;

namespace AHH_Bot.Commands
{
    [Group("guild")]
    public class Guild : ModuleBase<SocketCommandContext>
    {
        [Group("class")]
        public class Courses : ModuleBase<SocketCommandContext>
        {
            [Command("add")]
            public async Task AddCourse([Remainder]string course = "")
            {
                if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: You do not have the required permissions!");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(course))
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: You must specify a course.");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                int CourseCount = course.Count(x => x == ',') + 1;
                course = course.ToUpper().Replace(" ", "");

                if (course.Length < (7 * CourseCount) + (CourseCount - 1) || course.Length > (10 * CourseCount) + (CourseCount - 1))
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: Please enter a valid course!");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                if (Data.Guilds.GetCourses(Context.Guild.Id).Contains(course))
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: One or more of the specified courses are already in the list.");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                await Data.Guilds.AddCourse(Context.Guild.Id, course);
                await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully added the following courses to the server course list! ```\n{course.Split(',', StringSplitOptions.RemoveEmptyEntries).Join("\n")}```");
            }

            [Command("remove")]
            public async Task RemoveCourse(string course = "")
            {
                if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: You do not have the required permissions!");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(course))
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: You must specify a course.");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                if (course.Length < 7 || course.Length > 10)
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: Please enter a valid course!");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                course = course.ToUpper().Replace(" ", "");

                if (!Data.Guilds.GetCourses(Context.Guild.Id).Contains(course))
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: The course you entered was not in the server course list to begin with :face_palm:");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                await Data.Guilds.RemoveCourse(Context.Guild.Id, course);
                await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully removed {course} from the server course list!");
            }

            [Command("reset")]
            public async Task ResetGuildCourses()
            {
                if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
                {
                    var errormsg = await Context.Channel.SendMessageAsync(":x: You do not have the required permissions!");
                    await Task.Delay(Settings.ErrorMessageTime);
                    await Context.Message.DeleteAsync();
                    await errormsg.DeleteAsync();
                    return;
                }

                await Data.Guilds.ResetCourses(Context.Guild.Id);
                await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully removed all courses from the server course list!");
            }

            [Command("list")]
            public async Task ViewCourses()
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Displaying Server Default Courses")
                    .WithColor(53, 125, 242)
                    .AddField(x =>
                    {
                        x.Name = "Course List";
                        x.Value = Data.Guilds.GetCourses(Context.Guild.Id).Join("\n");
                    })
                    .WithCurrentTimestamp();

                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }
    }
}
