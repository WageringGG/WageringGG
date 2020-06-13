using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
        public DbSet<WagerApproval> WagerApprovals { get; set; }
        #endregion
        #region Tournaments
        public DbSet<Tournament> Tournaments { get; set; }
        #endregion
        public DbSet<Game> Games { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Notification>().HasKey(x => new { x.Date, x.ProfileId });
            builder.Entity<WagerApproval>().HasKey(x => new { x.HostId, x.ChallengeId });
            builder.Entity<WagerApproval>().HasOne(x => x.Host).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Profile>().HasIndex(x => x.NormalizedDisplayName).IsUnique();
            builder.Entity<Game>().HasIndex(x => x.NormalizedName).IsUnique();

            /*const int users = 10;
            Profile[] profiles = new Profile[users];
            List<Wager> wagers = new List<Wager>();
            for (int i = 0; i < users; i++)
            {
                profiles[i] = new Profile
                {
                    DisplayName = $"user_{i}",
                    NormalizedDisplayName = $"USER_{i}",
                    Id = $"FAKEID{i}",
                };
            }

            builder.Entity<Profile>().HasData(profiles);*/
            builder.Entity<Game>().HasData(Constants.Games.Values);
            base.OnModelCreating(builder);
        }
    }
}