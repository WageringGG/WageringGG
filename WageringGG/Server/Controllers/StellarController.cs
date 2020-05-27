using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Models;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StellarController : ControllerBase
    {
        private readonly stellar_dotnet_sdk.Server _server;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public StellarController(stellar_dotnet_sdk.Server server, IConfiguration config, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _server = server;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> RequestChallenge([FromQuery] string account)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.GetId());
            if (user == null)
                return BadRequest(new string[] { "User not found." });
            var claims = await _userManager.GetClaimsAsync(user);
            var keyClaim = claims.KeyClaim();
            if (keyClaim != null && keyClaim.Value == account)
                return BadRequest(new string[] { $"User's public key is already {account}." });
            KeyPair serverKeys = KeyPair.FromSecretSeed(_config["Stellar:SecretSeed"]);
            return Ok(WebAuthentication.BuildChallengeTransaction(serverKeys, account, "Wagering.GG").ToEnvelopeXdrBase64());
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTransaction([FromBody] string transaction)
        {
            Transaction signedTransaction = Transaction.FromEnvelopeXdr(transaction);
            Dictionary<string, int> signerSummary = new Dictionary<string, int>();
            signedTransaction.Signatures.ForEach(x =>
            {
                signerSummary.Add(KeyPair.FromPublicKey(x.Signature.InnerValue).Address, 1);
            });
            try
            {
                string serverId = _config["Stellar:PublicKey"];
                ICollection<string> clients = WebAuthentication.VerifyChallengeTransactionThreshold(signedTransaction, serverId, 2, signerSummary);
                if (clients.Count == 1)
                {
                    string key = clients.First();
                    ApplicationUser user = await _userManager.FindByIdAsync(User.GetId());
                    if (user == null)
                        throw new Exception("User not found.");
                    var claims = await _userManager.GetClaimsAsync(user);
                    Profile profile = await _context.Profiles.FindAsync(user.Id);
                    var keyClaim = claims.KeyClaim();
                    var newClaim = new Claim(Claims.PublicKey, key);
                    if (keyClaim == null)
                        await _userManager.AddClaimAsync(user, newClaim);
                    else
                        await _userManager.ReplaceClaimAsync(user, keyClaim, newClaim);
                    profile.PublicKey = key;
                    _context.SaveChanges();
                    await _signInManager.RefreshSignInAsync(user);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new string[] { e.Message });
            }
        }
    }
}