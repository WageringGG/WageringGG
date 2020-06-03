using Microsoft.EntityFrameworkCore;
using WageringGG.Shared.Models;
using Constants = WageringGG.Shared.Constants;
#nullable disable

namespace WageringGG.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        #region Wagers
        public DbSet<Wager> Wagers { get; set; }
        public DbSet<WagerChallenge> WagerChallenges { get; set; }
        public DbSet<WagerChallengeBid> WagerChallengeBids { get; set; }
        public DbSet<WagerHostBid> WagerHostBids { get; set; }
        public DbSet<WagerRule> WagerRules { get; set; }
        #endregion
        #region Tournaments
        public DbSet<Tournament> Tournaments { get; set; }
        #endregion
        public DbSet<Game> Games { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<PersonalNotification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PersonalNotification>().HasKey(x => new { x.Date, x.ProfileId });
            builder.Entity<Profile>().HasIndex(x => x.NormalizedDisplayName).IsUnique();
            builder.Entity<Profile>().HasAlternateKey(x => x.DisplayName);
            builder.Entity<Game>().HasIndex(x => x.NormalizedName).IsUnique();

            builder.Entity<Game>().HasData(Constants.Games.Values);
            base.OnModelCreating(builder);
        }
    }
}