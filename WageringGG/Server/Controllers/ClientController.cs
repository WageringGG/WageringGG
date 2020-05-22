using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
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

        [HttpGet("WagerChallenge")]
        public async Task<IActionResult> WagerChallenges()
        {
            string? userId = User.GetId();
            List<WagerChallenge> results = await _context.WagerChallengeBids.AsNoTracking().Where(x => x.ProfileId == userId).Include(x => x.Challenge).ThenInclude(x => x.Challengers).ThenInclude(x => x.Profile).Select(x => x.Challenge).ToListAsync();
            return Ok(results);
        }
    }
}