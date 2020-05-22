using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WageringGG.Server.Models;

namespace WageringGG.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            var logout = "~/authentication/logout";
            returnUrl ??= logout;
            if (returnUrl == "/")
                returnUrl = logout;
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return LocalRedirect(returnUrl);
        }
    }
}
