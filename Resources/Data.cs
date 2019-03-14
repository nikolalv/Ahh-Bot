using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHH_Bot.Commands;

namespace AHH_Bot.Database
{
    public static class Data
    {
        public static List<Voting.Vote> CurrentVotes = new List<Voting.Vote>();

        public static class Challenges
        {
            public static bool ChallengeExists(DatabaseContext databaseContext, int ChallengeNumber)
            {
                return databaseContext.Challenges.Any(x => x.ChallengeNumber == ChallengeNumber);
            }

            public static async Task AddChallenge(string challengeName, string difficulty, int rewardAmount)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    databaseContext.Challenges.Add(new Challenge
                    {
                        ChallengeNumber = databaseContext.Challenges.Count(),
                        ChallengeName = challengeName,
                        Difficulty = difficulty,
                        RewardAmount = rewardAmount,
                        SolvedByCounter = 0
                    });
                }
            }
        }

        public class Users
        {
            // Returns if a user exists in the database and creates a new user if it doesn't
            public static bool UserExisted(DatabaseContext databaseContext, ulong UserID, int ChocolateCount = 0, int CommandsExecuted = 0, int CompletedChallenges = 0, int MessagesSent = 0, int ReactedMessages = 0, int VotesCreated = 0, string Courses = "")
            {
                if (!databaseContext.Users.Any(x => x.UserID == UserID))
                {
                    databaseContext.Users.Add(new User
                    {
                        UserID = UserID,
                        CompletedChallenges = CompletedChallenges,
                        ChocolateCount = ChocolateCount,
                        CommandsExecuted = CommandsExecuted,
                        MessagesSent = MessagesSent,
                        ReactedMessages = ReactedMessages,
                        VotesCreated = VotesCreated,
                        Courses = Courses
                    });
                    return false;
                }
                return true;
            }

            public class Messages
            {
                #region Messages
                public static async Task AddMessages(ulong UserID, int messageAmount)
                {
                    using (var databaseContext = new DatabaseContext())
                    {
                        if (UserExisted(databaseContext, UserID, MessagesSent: messageAmount))
                        {
                            User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                            currentUser.MessagesSent += messageAmount;
                            databaseContext.Users.Update(currentUser);
                        }

                        await databaseContext.SaveChangesAsync();
                    }
                }

                public static int GetSentMsgAmount(ulong UserID)
                {
                    using (var databaseContext = new DatabaseContext())
                        return databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.MessagesSent).FirstOrDefault();
                }
                #endregion

                #region Commands
                public static async Task AddExecutedCommand(ulong UserID, int commandAmount)
                {
                    using (var databaseContext = new DatabaseContext())
                    {
                        if (UserExisted(databaseContext, UserID, CommandsExecuted: commandAmount))
                        {
                            User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                            currentUser.CommandsExecuted += commandAmount;
                            databaseContext.Users.Update(currentUser);
                        }

                        await databaseContext.SaveChangesAsync();
                    }
                }

                public static int GetExecutedCmdAmount(ulong UserID)
                {
                    using (var databaseContext = new DatabaseContext())
                        return databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.CommandsExecuted).FirstOrDefault();
                }
                #endregion

                #region Reactions
                public static async Task AddReaction(ulong UserID, int reactionAmount)
                {
                    using (var databaseContext = new DatabaseContext())
                    {
                        if (UserExisted(databaseContext, UserID, ReactedMessages: reactionAmount))
                        {
                            User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                            currentUser.ReactedMessages += reactionAmount;
                            databaseContext.Users.Update(currentUser);
                        }

                        await databaseContext.SaveChangesAsync();
                    }
                }

                public static int GetReactionAmount(ulong UserID)
                {
                    using (var databaseContext = new DatabaseContext())
                        return databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.ReactedMessages).FirstOrDefault();
                }
                #endregion

                #region VotesStarted
                public static async Task VotesAddUserVoteStat(ulong UserID, int voteAmount)
                {
                    using (var databaseContext = new DatabaseContext())
                    {
                        if (UserExisted(databaseContext, UserID, VotesCreated: voteAmount))
                        {
                            User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                            currentUser.VotesCreated += voteAmount;
                            databaseContext.Users.Update(currentUser);
                        }

                        await databaseContext.SaveChangesAsync();
                    }
                }

                public static int GetTotalVoteAmount(ulong UserID)
                {
                    using (var databaseContext = new DatabaseContext())
                        return databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.VotesCreated).FirstOrDefault();
                }
                #endregion
            }

            public class Chocolates
            {
                public static async Task AddChocolates(ulong UserID, int chocolateAmount)
                {
                    using (var databaseContext = new DatabaseContext())
                    {
                        if (UserExisted(databaseContext, UserID, ChocolateCount: chocolateAmount))
                        {
                            User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                            currentUser.ChocolateCount += chocolateAmount;
                            databaseContext.Users.Update(currentUser);
                        }

                        await databaseContext.SaveChangesAsync();
                    }
                }

                public static async Task SetChocolates(ulong UserID, int chocolateAmount)
                {
                    using (var databaseContext = new DatabaseContext())
                    {
                        if (UserExisted(databaseContext, UserID, ChocolateCount: chocolateAmount))
                        {
                            User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                            currentUser.ChocolateCount = chocolateAmount;
                            databaseContext.Users.Update(currentUser);
                        }

                        await databaseContext.SaveChangesAsync();
                    }
                }

                public static int GetChocolateAmount(ulong UserID)
                {
                    using (var databaseContext = new DatabaseContext())
                        return databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.ChocolateCount).FirstOrDefault();
                }

                public static ulong[] GetTopChoco(int count)
                {
                    using (var databaseContext = new DatabaseContext())
                        return databaseContext.Users.OrderByDescending(x => x.ChocolateCount).Select(x => x.UserID).Take(count).ToArray();
                }
            }

            #region UserChallenges
            public static int GetCompletedChallenges(ulong UserID)
            {
                using (var databaseContext = new DatabaseContext())
                    return databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.CompletedChallenges).FirstOrDefault();
            }
            #endregion

            #region Courses
            public static async Task AddCourse(ulong UserID, string course)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    if (UserExisted(databaseContext, UserID, Courses: course))
                    {
                        User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                        currentUser.Courses += "," + course;
                        databaseContext.Users.Update(currentUser);
                    }

                    await databaseContext.SaveChangesAsync();
                }
            }

            public static async Task RemoveCourse(ulong UserID, string course)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    if (UserExisted(databaseContext, UserID))
                    {
                        User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                        currentUser.Courses = currentUser.Courses.Replace("," + course, "");
                        databaseContext.Users.Update(currentUser);
                    }

                    await databaseContext.SaveChangesAsync();
                }
            }

            public static async Task ResetCourses(ulong UserID)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    if (UserExisted(databaseContext, UserID))
                    {
                        User currentUser = databaseContext.Users.First(x => x.UserID == UserID);
                        currentUser.Courses = "";
                        databaseContext.Users.Update(currentUser);
                    }

                    await databaseContext.SaveChangesAsync();
                }
            }

            public static string[] GetCourses(ulong UserID)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    string courses = databaseContext.Users.Where(x => x.UserID == UserID).Select(x => x.Courses).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(courses))
                        courses = "Empty! You can add courses using `!class add <Course Code>`";
                    return courses.Split(',', StringSplitOptions.RemoveEmptyEntries);
                }
            }
            #endregion
        }

        public class Guilds
        {
            // Returns if a guild exists in the database and creates a new guild if it doesn't
            private static bool GuildExisted(DatabaseContext databaseContext, ulong ServerID, string Courses = "")
            {
                if (!databaseContext.Guilds.Any(x => x.ServerID == ServerID))
                {
                    databaseContext.Guilds.Add(new Guild
                    {
                        ServerID = ServerID,
                        Courses = Courses
                    });
                    return false;
                }
                return true;
            }

            #region Courses
            public static async Task AddCourse(ulong ServerID, string course)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    if (GuildExisted(databaseContext, ServerID, Courses: course))
                    {
                        Guild currentGuild = databaseContext.Guilds.First(x => x.ServerID == ServerID);
                        currentGuild.Courses += "," + course;
                        databaseContext.Guilds.Update(currentGuild);
                    }

                    await databaseContext.SaveChangesAsync();
                }
            }

            public static async Task RemoveCourse(ulong ServerID, string course)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    if (GuildExisted(databaseContext, ServerID, Courses: course))
                    {
                        Guild currentGuild = databaseContext.Guilds.First(x => x.ServerID == ServerID);
                        currentGuild.Courses = currentGuild.Courses.Replace("," + course, "");
                        databaseContext.Guilds.Update(currentGuild);
                    }

                    await databaseContext.SaveChangesAsync();
                }
            }

            public static async Task ResetCourses(ulong ServerID)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    if (GuildExisted(databaseContext, ServerID))
                    {
                        Guild currentGuild = databaseContext.Guilds.First(x => x.ServerID == ServerID);
                        currentGuild.Courses = "";
                        databaseContext.Guilds.Update(currentGuild);
                    }

                    await databaseContext.SaveChangesAsync();
                }
            }

            public static string[] GetCourses(ulong ServerID)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    string courses = databaseContext.Guilds.Where(x => x.ServerID == ServerID).Select(x => x.Courses).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(courses))
                        courses = "Empty! Please ask the administrator to add default courses using `!guild class add <Course Code>`";
                    return courses.Split(',', StringSplitOptions.RemoveEmptyEntries);
                }
            }
            #endregion
        }
    }
}
