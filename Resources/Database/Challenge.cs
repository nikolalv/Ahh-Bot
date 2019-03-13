using System.ComponentModel.DataAnnotations;

namespace AHH_Bot.Database
{
    public class Challenge
    {
        [Key]
        public int ChallengeNumber { get; set; }
        public string ChallengeName { get; set; }
        public string Difficulty { get; set; }
        public int SolvedByCounter { get; set; }
        public int RewardAmount { get; set; }
    }
}