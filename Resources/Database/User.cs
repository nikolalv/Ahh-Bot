using System.ComponentModel.DataAnnotations;

namespace AHH_Bot.Database
{
    public class User
    {
        [Key]
        public ulong UserID { get; set; }
        public int ChocolateCount { get; set; }
        public int CommandsExecuted { get; set; }
        public int MessagesSent { get; set; }
        public int CompletedChallenges { get; set; }
        public int ReactedMessages { get; set; }
        public int VotesCreated { get; set; }
        public string Courses { get; set; }
    }
}
