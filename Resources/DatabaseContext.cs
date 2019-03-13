using Microsoft.EntityFrameworkCore;

namespace AHH_Bot.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Guild> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=Database.sqlite");
        }
    }
}
