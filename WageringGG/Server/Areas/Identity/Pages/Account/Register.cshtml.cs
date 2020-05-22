using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Models;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Required]
            [RegularExpression(Regex.DisplayName)]
            [StringLength(12, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [Display(Name = "Display Name")]
            public string? DisplayName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/authentication/login");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            string? normalizedDisplayName = Input.DisplayName?.ToUpper();
            if (await _context.Profiles.FirstOrDefaultAsync(x => x.NormalizedDisplayName == normalizedDisplayName) != null)
                ModelState.AddModelError(string.Empty, "Display name is already taken.");
            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    if (error.Code != "DuplicateUserName") //same as email
                        ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }
            await _userManager.AddClaimAsync(user, new Claim("display_name", Input.DisplayName));

            _logger.LogInformation("User created a new account with password.");
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


            var profile = new Profile
            {
                DisplayName = Input.DisplayName,
                NormalizedDisplayName = normalizedDisplayName,
                Id = user.Id,
                IsVerified = false,
            };
            await _userManager.AddClaimAsync(user, new Claim("is_verified", "false"));
            _context.Profiles.Add(profile);
            _context.SaveChanges();

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
            }
            else
            {
                await _signInManager.SignInAsync(user, true);
                return LocalRedirect(returnUrl);
            }
        }
    }
}
