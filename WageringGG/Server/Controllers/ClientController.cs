using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("wagers")]
        public async Task<IActionResult> GetWagerChallenges()
        {
            string? userId = User.GetId();
            IEnumerable<WagerChallenge> results = await _context.WagerMembers.AsNoTracking().Where(x => x.ProfileId == userId).Include(x => x.Challenge).Select(x => x.Challenge).ToListAsync();
            return Ok(results);
        }

        [HttpGet("wager/{id}")]
        public async Task<IActionResult> GetWagerChallenge(int id)
        {
            string? userId = User.GetId();
            WagerChallenge challenge = await _context.WagerChallenges.AsNoTracking().Where(x => x.Id == id).Include(x => x.Members).ThenInclude(x => x.Profile).Include(x => x.Wager).FirstOrDefaultAsync();
            if (challenge == null)
                return BadRequest(new string[] { Errors.NotFound });
            if (!challenge.Members.Any(x => x.ProfileId == userId))
                return BadRequest(new string[] { "You are not an owner of the wager challenge." });
            return Ok(challenge);
        }
    }
}