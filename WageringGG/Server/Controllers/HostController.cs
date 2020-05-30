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
        [Authorize]
        public async Task<IActionResult> ControlWagers()
        {
            string? userId = User.GetId();
            IEnumerable<Wager> results = await _context.WagerHostBids.AsNoTracking().Where(x => x.ProfileId == userId).Include(x => x.Wager).Select(x => x.Wager).ToListAsync();
            return Ok(results);
        }

        [HttpGet("wager/{id}")]
        [Authorize]
        public async Task<IActionResult> GetControlWager(int id)
        {
            string? userId = User.GetId();

            Wager wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == id).Include(x => x.Hosts).ThenInclude(x => x.Profile).Include(x => x.Challenges).ThenInclude(x => x.Challengers).ThenInclude(x => x.Profile).FirstOrDefaultAsync();

            if (wager == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (!wager.Hosts.Any(x => x.ProfileId == userId))
            {
                ModelState.AddModelError(string.Empty, "You are not a host of this wager.");
                return BadRequest(ModelState.GetErrors());
            }
            return Ok(wager);
        }
    }
}