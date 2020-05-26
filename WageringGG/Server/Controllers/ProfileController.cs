using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using stellar_dotnet_sdk;
using System.Security.Claims;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Models;
using WageringGG.Shared.Constants;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpPut("key/{key}")]
        public async Task<IActionResult> EditPublicKey(string key)
        {
            if (!StrKey.IsValidEd25519PublicKey(key))
            {
                ModelState.AddModelError(string.Empty, $"{key} is not a valid stellar key");
                return BadRequest(ModelState.GetErrors());
            }
            string? userId = User.GetId();
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Could not load user with id {userId}");
                return BadRequest(ModelState.GetErrors());
            }
            var claims = await _userManager.GetClaimsAsync(user);
            var profile = await _context.Profiles.FindAsync(user.Id);

            //ADD SEP-0010 PROTOCOL FOR VERIFYING OWNERSHIP
            if (key != profile.PublicKey)
            {
                profile.PublicKey = key;
                Claim keyClaim = claims.KeyClaim();
                Claim newClaim = new Claim(Claims.PublicKey, key);

                if (keyClaim == null)
                    await _userManager.AddClaimAsync(user, newClaim);
                else
                    await _userManager.ReplaceClaimAsync(user, keyClaim, newClaim);
                _context.Profiles.Update(profile);
                _context.SaveChanges();
                await _signInManager.RefreshSignInAsync(user);
            }
            return Ok();
        }
    }
}