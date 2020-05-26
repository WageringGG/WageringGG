using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Constants;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int ResultSize = 5;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetUser(string name)
        {
            name = name.ToUpper();
            var user = await _context.Profiles.Where(x => x.NormalizedDisplayName == name).Include(x => x.Ratings).FirstOrDefaultAsync();
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            return Ok(user);
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchUsers(string query)
        {
            query = query.ToUpper();
            var users = await _context.Profiles.Where(x => x.NormalizedDisplayName.Contains(query)).Take(ResultSize).ToListAsync();
            return Ok(users);
        }
    }
}
