using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var userId = User.GetId();
            //join tournamenthostbids etc
            IEnumerable<string> hostGroups = await _context.WagerHostBids.AsNoTracking().Where(x => x.ProfileId == userId).Select(x => GetGroupName.Wager(x.WagerId)).ToListAsync();
            //IEnumerable<string> clientGroups = await _context.WagerChallengeBids.AsNoTracking().Where(x => x.ProfileId == userId).Select(x => GetGroupName.Wager(x.WagerId)).ToListAsync();
            return Ok(hostGroups);
        }
    }
}