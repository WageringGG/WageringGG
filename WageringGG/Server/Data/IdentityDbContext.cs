using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WageringGG.Server.Models;
#nullable disable
namespace WageringGG.Server.Data
{
    public class IdentityDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        IOptions<OperationalStoreOptions> Options { get; set; }

        public IdentityDbContext(
          DbContextOptions options,
          IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            Options = operationalStoreOptions;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().Ignore(x => x.PhoneNumber);
            builder.Entity<ApplicationUser>().Ignore(x => x.PhoneNumberConfirmed);
            builder.ConfigurePersistedGrantContext(Options.Value);
            base.OnModelCreating(builder);
        }
    }
}