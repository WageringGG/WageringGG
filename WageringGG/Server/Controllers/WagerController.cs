using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Models;
using WageringGG.Server.Services;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WagerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HubService _hub;
        private readonly IConfiguration _config;
        private const int ResultSize = 16;

        public WagerController(ApplicationDbContext context, HubService hub, IConfiguration config)
        {
            _context = context;
            _hub = hub;
            _config = config;
        }

        //POST: api/wagers/search
        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetWagers(int gameId, int? page, int? minimumWager, int? maximumWager, int? playerCount)
        {
            if (page < 1)
                ModelState.AddModelError("Page", $"{page} is not a valid page.");
            if (minimumWager.HasValue && maximumWager.HasValue && minimumWager.Value > maximumWager.Value)
                ModelState.AddModelError("Greater than", "Minimum wager cannot be larger than the maximum wager.");
            if (maximumWager.HasValue && maximumWager.Value < 0)
                ModelState.AddModelError("Max Negative", "Maximum wager cannot be negative.");
            if (minimumWager.HasValue && minimumWager.Value < 0)
                ModelState.AddModelError("Min Negative", "Minimum wager cannot be negative.");
            if (playerCount.HasValue && playerCount.Value < 0)
                ModelState.AddModelError("Player Negative", "Player count cannot be negative.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            IQueryable<Wager> wagerQuery = _context.Wagers.AsNoTracking().Where(x => x.GameId == gameId).Where(x => !x.IsPrivate).Where(x => x.Status == Status.Open);

            if (playerCount.HasValue)
                wagerQuery = wagerQuery.Where(x => x.PlayerCount == playerCount);
            if (minimumWager.HasValue)
                wagerQuery = wagerQuery.Where(x => x.Amount > minimumWager);
            if (maximumWager.HasValue)
                wagerQuery = wagerQuery.Where(x => x.Amount < maximumWager);
            PaginatedList<Wager> results = await Paginator<Wager>.CreateAsync(wagerQuery.OrderByDescending(x => x.Date), page ?? 1, ResultSize);
            return Ok(results);
        }

        [HttpPut("status/{id}")]
        [Authorize]
        public async Task<IActionResult> SetStatus(int id, [FromBody] Status status)
        {
            string? userId = User.GetId();
            var wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Hosts).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { Errors.NotFound });
            if (!wager.Hosts.Any(x => x.ProfileId == userId))
                return BadRequest();
            //state diagram
            switch (wager.Status)
            {
                case Status.Pending:
                    break;
                case Status.Open:
                    if (status == Status.Closed)
                        wager.Status = Status.Closed;
                    break;
                case Status.Closed:
                    if (status == Status.Open)
                        wager.Status = Status.Open;
                    if (status == Status.Canceled)
                        wager.Status = Status.Canceled;
                    break;
            }
            _context.SaveChanges();
            return Ok();
        }

        // GET: api/wagers/{id}
        [HttpGet("view/{id}")]
        public async Task<IActionResult> GetWager(int id)
        {
            var wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == id).Include(x => x.Hosts).ThenInclude(x => x.Profile).Include(x => x.Challenges).FirstOrDefaultAsync();
            if (wager == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            return Ok(wager);
        }

        [HttpPost("cancel/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelWager(int id)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            Wager wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Hosts).Include(x => x.Challenges).FirstOrDefaultAsync();

            if (wager == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (wager.Status != (byte)Status.Pending)
            {
                ModelState.AddModelError(string.Empty, "Wager is not in the created.");
                return BadRequest(ModelState.GetErrors());
            }
            if (!wager.Hosts.Any(x => x.ProfileId == userId))
            {
                ModelState.AddModelError(string.Empty, "You are not a host of this wager.");
                return BadRequest(ModelState.GetErrors());
            }
            wager.Status = Status.Canceled;
            Notification notification = new Notification
            {
                Date = DateTime.Now,
                Message = $"{userName} has canceled the wager.",
                Link = $"/wagers/view/{id}"
            };
            List<Notification> notifications = await _hub.SendNotificationsAsync(wager.HostIds().ToArray(), notification);
            _context.Notifications.AddRange(notifications);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteWager(int id)
        {
            string? userId = User.GetId();

            Wager wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Hosts).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { Errors.NotFound });
            if (!wager.Hosts.Any(x => x.ProfileId == userId))
                return BadRequest(new string[] { "You are not a wager host." });
            if (wager.Status != Status.Canceled)
                return BadRequest(new string[] { "The wager must be in the canceled state to be deleted." });

            _context.Wagers.Remove(wager);
            _context.SaveChanges();
            return Ok();
        }

        // POST: api/Wagers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateWager(Wager wager)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            string? userKey = User.GetKey();
            if (userKey == null)
                ModelState.AddModelError(string.Empty, "You do not have a public key registered.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            if (wager.Hosts.Sum(x => x.Receivable) != wager.Amount * 2)
                ModelState.AddModelError(string.Empty, "Receivable");
            if (wager.Hosts.Sum(x => x.Payable) != wager.Amount)
                ModelState.AddModelError(string.Empty, "Payable");
            if (!wager.Hosts.Any(x => x.ProfileId == userId))
                ModelState.AddModelError(string.Empty, "Caller must be a host.");
            if (!wager.HostIds().IsUnique())
                ModelState.AddModelError(string.Empty, "The id's are not unique.");
            if (wager.Amount <= 0)
                ModelState.AddModelError(string.Empty, "The wager amount has to be greater than 0.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            DateTime date = DateTime.Now;
            wager.Date = date;
            wager.Status = Status.Pending;
            wager.ChallengeCount = 0;
            wager.PlayerCount = wager.Hosts.Count;

            foreach (WagerHost host in wager.Hosts)
            {
                host.IsApproved = null;
                if (host.ProfileId == userId)
                    host.IsApproved = true;
            }

            if (wager.Hosts.Count == 1)
                wager.Status = Status.Open;

            _context.Wagers.Add(wager);
            _context.SaveChanges();
            if (wager.Hosts.Count > 1)
            {
                string[] ids = wager.HostIds().ToArray();
                Notification notification = new Notification
                {
                    Date = date,
                    Message = $"{userName} created a wager with you.",
                    Link = $"/host/wagers/view/{wager.Id}"
                };
                List<Notification> notifications = await _hub.SendNotificationsAsync(ids.Where(x => x != userId).ToArray(), notification);
                _context.Notifications.AddRange(notifications);
                await _hub.SendGroupAsync(ids, Wager.Group(wager.Id));
            }
            return Ok(wager.Id);
        }

        [HttpPost("challenge/{wagerId}")]
        [Authorize]
        public async Task<IActionResult> CreateChallenge([FromRoute] int wagerId, [FromBody] WagerChallenge challenge)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            //check funds
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            if (!challenge.Members.Any(x => x.ProfileId == userId))
                ModelState.AddModelError(string.Empty, "Caller must be a host.");
            if (!challenge.Members.IsUnique())
                ModelState.AddModelError(string.Empty, "The id's are not unique.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            Wager wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == wagerId).Include(x => x.Hosts).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { "The wager could not be found." });
            if (wager.Status != Status.Open)
                ModelState.AddModelError(string.Empty, "The wager is not currently accepting challenges.");
            if (wager.HostIds().Intersect(challenge.ChallengerIds()).Count() > 0)
                ModelState.AddModelError(string.Empty, "A wager host cannot challenge themself.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            //set data
            DateTime date = DateTime.Now;
            challenge.IsAccepted = null;
            challenge.Status = Status.Pending;
            challenge.WagerId = wagerId;
            challenge.Date = date;
            foreach (WagerMember challenger in challenge.Members)
            {
                challenger.IsApproved = null;
                challenger.IsHost = false;
                challenger.Entries = 0;
            }

            _context.WagerChallenges.Add(challenge);
            _context.SaveChanges();
            return Ok(challenge.Id);
        }
    }
}