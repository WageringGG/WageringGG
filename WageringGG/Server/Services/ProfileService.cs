using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WageringGG.Server.Models;

namespace WageringGG.Server.Services
{
    public class ProfileService : IProfileService
    {
        protected UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var claims = await _userManager.GetClaimsAsync(user);
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = user != null;
        }
    }
}
