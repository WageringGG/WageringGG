using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WageringGG.Server.Models;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpDelete("publickey")]
        public async Task<IActionResult> DeletePublicKey()
        {
            string userId = User.GetId();
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            var claims = await _userManager.GetClaimsAsync(user);
            Claim keyClaim = claims.KeyClaim();
            if (keyClaim == null)
                return Ok();
            await _userManager.RemoveClaimAsync(user, keyClaim);
            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }
    }
}