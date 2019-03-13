using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Internal;
using AHH_Bot.Database;
using Discord.WebSocket;

namespace AHH_Bot.Commands
{
    [Group("class")]
    public class School : ModuleBase<SocketCommandContext>
    {
        [Command("")]
        public async Task CancelledClasses()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Cancelled Classes as of "+DateTime.Now.ToLongTimeString())
                .WithAuthor("John Abott College", "http://www.johnabbott.qc.ca/wp-content/uploads/2017/10/Islanders-logo_250x250.jpg", "http://johnabbott-cancelledclasses.omnivox.ca/")
                .WithColor(114, 132, 249);

            using (var client = new CookieAwareWebClient())
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(client.DownloadString("http://johnabbott-cancelledclasses.omnivox.ca/"));
                var pageTables = htmlDoc.DocumentNode.SelectNodes("/div[1]/html[1]/body[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table");

                string[] coursesToTrack = Data.Users.GetCourses(Context.User.Id).Union(Data.Guilds.GetCourses(Context.Guild.Id)).ToArray();

                for (int i = 1; i < pageTables.Count; i++)
                {
                    if (pageTables[i].InnerText.Contains(DateTime.Now.Year.ToString()))
                    {
                        if (pageTables[i].InnerText.Contains("No course has been cancelled for this day"))
                        {
                            embed.AddField(pageTables[i].SelectSingleNode("tr[1]").InnerText.Replace("Today,", ""), "No course has been cancelled for this day");
                            continue;
                        }
                        embed.AddField(pageTables[i].SelectSingleNode("tr[1]").InnerText.Replace("Today,", ""), "\u200b");
                    }
                    else
                    {
                        if (coursesToTrack.Any(x => pageTables[i].SelectSingleNode("tr[1]/td[3]/font[2]/br[1]").InnerText.Contains(x)))
                        {
                            embed.AddField(x =>
                            {
                                x.Name = pageTables[i].SelectSingleNode("tr[1]/td[3]/br[1]").PreviousSibling.InnerText.Trim().Replace("&nbsp;", "");

                                x.Value = "\n" + pageTables[i].SelectSingleNode("tr[1]/td[3]/font[2]/br[2]").PreviousSibling.InnerText.Trim().Replace("&nbsp;", "") +
                                          "\n" + pageTables[i].SelectSingleNode("tr[1]/td[3]/font[2]/br[1]").PreviousSibling.InnerText.Trim().Replace("&nbsp;", "");
                                x.IsInline = true;
                            });
                        }
                    }
                }
            }

                await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("add")]
        public async Task AddCourse([Remainder]string course = "")
        {
            if (string.IsNullOrWhiteSpace(course))
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: You must specify a course.");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (course.Contains("DROP TABLE"))
            {
                var errormsg = await Context.Channel.SendMessageAsync(":angry: Stop! You have violated the law! Pay the court a fine or serve your sentence");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            int CourseCount = course.Count(x => x == ',') + 1;
            course = course.ToUpper().Replace(" ", "");

            if (course.Length < (7 * CourseCount) + (CourseCount - 1) || course.Length > (10 * CourseCount) + (CourseCount - 1))
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: Please enter a valid courses!");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (Data.Users.GetCourses(Context.User.Id).Contains(course))
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: One or more of the specified courses are already in the list.");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            if (Data.Guilds.GetCourses(Context.Guild.Id).Contains(course))
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: One or more of the specified courses are already in the servers default course list.");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            await Data.Users.AddCourse(Context.User.Id, course);
            await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully added the following courses to your course list! ```{course.Split(',', StringSplitOptions.RemoveEmptyEntries).Join("\n")}```");
        }

        [Command("remove")]
        public async Task RemoveCourse(string course = "")
        {
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

            if (!Data.Users.GetCourses(Context.User.Id).Contains(course))
            {
                var errormsg = await Context.Channel.SendMessageAsync(":x: The course you entered was not in your course list to begin with :face_palm:");
                await Task.Delay(Settings.ErrorMessageTime);
                await Context.Message.DeleteAsync();
                await errormsg.DeleteAsync();
                return;
            }

            await Data.Users.RemoveCourse(Context.User.Id, course);
            await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully removed {course} from your course list!");
        }

        [Command("reset")]
        public async Task ResetUserCourses()
        {
            await Data.Users.ResetCourses(Context.User.Id);
            await Context.Channel.SendMessageAsync($":white_check_mark: You've successfully removed all courses from your course list!");
        }

        [Command("list")]
        public async Task ViewCourses()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Displaying Courses for " + (Context.User as SocketGuildUser).Nickname)
                .WithColor(53, 125, 242)
                .AddField(x =>
                {
                    x.Name = "Server Default Courses";
                    x.Value = Data.Guilds.GetCourses(Context.Guild.Id).Join("\n");
                })
                .AddField(x =>
                {
                    x.Name = "User Courses";
                    x.Value = Data.Users.GetCourses(Context.User.Id).Join("\n");
                })
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }

    // ================================================
    // Webpages that requires cookies to be viewed 
    // and thus needs the following class
    // Source: https://stackoverflow.com/questions/14551345/accept-cookies-in-webclient
    // ================================================

    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }

        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
                (request as HttpWebRequest).CookieContainer = CookieContainer;

            HttpWebRequest httpRequest = (HttpWebRequest)request;
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return httpRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            String setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

            if (setCookieHeader != null)
                CookieContainer.Add(new Cookie());

            return response;
        }
    }
}
