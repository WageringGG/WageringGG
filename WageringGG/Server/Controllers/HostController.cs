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
    public class HostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("wagers")]
        public async Task<IActionResult> GetHostWagers()
        {
            string? userId = User.GetId();
            IEnumerable<Wager> results = await _context.WagerMembers.AsNoTracking().Where(x => x.IsHost).Where(x => x.ProfileId == userId).Include(x => x.Wager).Select(x => x.Wager).ToListAsync();
            return Ok(results);
        }

        [HttpGet("wager/{id}")]
        public async Task<IActionResult> GetHostWager(int id)
        {
            string? userId = User.GetId();

            Wager wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == id).Include(x => x.Members).ThenInclude(x => x.Profile).Include(x => x.Challenges).FirstOrDefaultAsync();

            if (wager == null)
                return BadRequest(new string[] { Errors.NotFound });
            if (!wager.Members.Any(x => x.ProfileId == userId && x.IsHost == true))
                return BadRequest(new string[] { "You are not a host of this wager." });
            return Ok(wager);
        }
    }
}