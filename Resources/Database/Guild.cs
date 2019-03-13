using System.ComponentModel.DataAnnotations;

namespace AHH_Bot.Database
{
    public class Guild
    {
        [Key]
        public ulong ServerID { get; set; }
        public string Courses { get; set; }
    }
}