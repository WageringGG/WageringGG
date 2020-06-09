using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Hubs;
using WageringGG.Server.Models;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;
using stellar = stellar_dotnet_sdk;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WagerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly stellar.Server _server;
        private readonly IHubContext<GroupHub> _hubContext;
        private const int ResultSize = 16;

        public WagerController(ApplicationDbContext context, IHubContext<GroupHub> hubContext, stellar.Server server)
        {
            _context = context;
            _server = server;
            _hubContext = hubContext;
        }

        //POST: api/wagers/search
        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetWagers(int gameId, int? page, string? displayName, int? minimumWager, int? maximumWager, int? playerCount)
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

            byte confirmed = (byte)Status.Confirmed;
            IQueryable<Wager> wagerQuery = _context.Wagers.AsNoTracking().Where(x => x.GameId == gameId).Where(x => !x.IsPrivate).Where(x => x.Status == confirmed);

            if (playerCount.HasValue)
                wagerQuery = wagerQuery.Where(x => x.PlayerCount == playerCount);
            if (minimumWager.HasValue)
                wagerQuery = wagerQuery.Where(x => x.MinimumWager == null || (x.MinimumWager.HasValue && x.MinimumWager > minimumWager) || (x.MaximumWager.HasValue && x.MaximumWager > minimumWager));
            if (maximumWager.HasValue)
                wagerQuery = wagerQuery.Where(x => x.MaximumWager == null || (x.MinimumWager.HasValue && x.MinimumWager < maximumWager) || (x.MaximumWager.HasValue && x.MaximumWager < maximumWager));
            if (displayName != null)
            {
                displayName = displayName.ToUpper();
                wagerQuery = wagerQuery.Include(x => x.Hosts).ThenInclude(x => x.Profile).Where(x => x.Hosts.Any(x => x.Profile.NormalizedDisplayName.Contains(displayName)));
            }
            PaginatedList<Wager> results = await Paginator<Wager>.CreateAsync(wagerQuery.OrderByDescending(x => x.Date), page ?? 1, ResultSize);
            return Ok(results);
        }

        [HttpPut("status/{id}")]
        [Authorize]
        public async Task<IActionResult> SetStatus(int id, [FromBody] byte status)
        {
            string? userId = User.GetId();
            var wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == id).Include(x => x.Hosts).Where(x => x.Hosts.Any(y => y.ProfileId == userId)).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { Errors.NotFound });
            //state diagram
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
            Wager wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Hosts).Include(x => x.Challenges).ThenInclude(x => x.Challengers).FirstOrDefaultAsync();

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
            wager.Status = (byte)Status.Canceled;
            Notification notification = new Notification
            {
                Date = DateTime.Now,
                Message = $"{userName} has canceled the wager.",
                Link = $"/wagers/view/{id}"
            };
            NotificationHandler.AddNotificationToUsers(_context, wager.HostIds(), notification);
            _context.SaveChanges();
            return Ok();
        }

        // POST: api/Wagers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateWager(Wager wagerData)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            string? userKey = User.GetKey();
            if (userKey == null)
                ModelState.AddModelError(string.Empty, "You do not have a public key registered.");
            //check funds

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            if (wagerData.Hosts.Sum(x => x.ReceivablePt) != 100)
                ModelState.AddModelError(string.Empty, "The receivable percentages do not add up to 100.");
            if (wagerData.Hosts.Sum(x => x.PayablePt) != 100)
                ModelState.AddModelError(string.Empty, "The payable percentages do not add up to 100.");
            if (!wagerData.Hosts.Any(x => x.ProfileId == userId))
                ModelState.AddModelError(string.Empty, "Caller must be a host.");
            if (!wagerData.HostIds().IsUnique())
                ModelState.AddModelError(string.Empty, "The id's are not unique.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            DateTime date = DateTime.Now;
            Wager wager = new Wager //prevents overposting
            {
                GameId = wagerData.GameId,
                Date = date,
                Description = wagerData.Description,
                MinimumWager = wagerData.MinimumWager,
                MaximumWager = wagerData.MaximumWager,
                IsPrivate = wagerData.IsPrivate,
                Status = (byte)Status.Pending,
                ChallengeCount = 0,
                PlayerCount = wagerData.Hosts.Count
            };

            foreach (WagerHostBid host in wagerData.Hosts)
            {
                WagerHostBid bid = new WagerHostBid
                {
                    Approved = null,
                    ReceivablePt = host.ReceivablePt,
                    PayablePt = host.PayablePt,
                    ProfileId = host.ProfileId
                };
                if (bid.ProfileId == userId)
                    bid.Approved = true;
                wager.Hosts.Add(bid);
            }

            if (wager.IsApproved())
                wager.Status = (byte)Status.Confirmed;

            _context.Wagers.Add(wager);
            _context.SaveChanges();
            if (wager.Hosts.Count > 1)
            {
                Notification notification = new Notification
                {
                    Date = date,
                    Message = $"{userName} created a wager with you.",
                    Link = $"/host/wagers/view/{wager.Id}"
                };
                IEnumerable<string> others = wager.HostIds().Where(x => x != userId);
                NotificationHandler.AddNotificationToUsers(_context, others, notification);
                await HubHandler.SendGroupAsync(_hubContext, others.ToList(), wager.GroupName, notification);
            }
            return Ok(wager.Id);
        }

        [HttpPost("challenge/{wagerId}")]
        [Authorize]
        public async Task<IActionResult> CreateChallenge([FromRoute] int wagerId, [FromBody] WagerChallenge challengeData)
        {
            string? userKey = User.GetKey();
            string? userId = User.GetId();
            string? userName = User.GetName();
            if (userKey == null)
                ModelState.AddModelError(string.Empty, "You do not have a public key registered.");
            //check funds
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            if (challengeData.Challengers.Sum(x => x.ReceivablePt) != 100)
                ModelState.AddModelError(string.Empty, "The receivable percentages do not add up to 100.");
            if (challengeData.Challengers.Sum(x => x.PayablePt) != 100)
                ModelState.AddModelError(string.Empty, "The payable percentages do not add up to 100.");
            if (!challengeData.Challengers.Any(x => x.ProfileId == userId))
                ModelState.AddModelError(string.Empty, "Caller must be a host.");
            if (!challengeData.ChallengerIds().IsUnique())
                ModelState.AddModelError(string.Empty, "The id's are not unique.");
            Wager wager = await _context.Wagers.Where(x => x.Id == wagerId).Include(x => x.Hosts).FirstOrDefaultAsync();
            if (wager == null)
                ModelState.AddModelError(string.Empty, "The wager could not be found.");
            else if (wager.Status != (byte)Status.Confirmed)
                ModelState.AddModelError(string.Empty, "The wager is not currently accepting challenges.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            DateTime date = DateTime.Now;
            WagerChallenge challenge = new WagerChallenge
            {
                Amount = challengeData.Amount,
                Date = date,
                IsAccepted = false,
                Status = (byte)Status.Pending,
                WagerId = wagerId
            };
            foreach (WagerChallengeBid bid in challengeData.Challengers)
            {
                WagerChallengeBid challengeBid = new WagerChallengeBid
                {
                    Approved = null,
                    PayablePt = bid.PayablePt,
                    ReceivablePt = bid.ReceivablePt,
                    ProfileId = bid.ProfileId
                };
                if (bid.ProfileId == userId)
                    challengeBid.Approved = true;
                challenge.Challengers.Add(challengeBid);
            }
            if (challenge.IsApproved())
                challenge.Status = (byte)Status.Confirmed;

            _context.WagerChallenges.Add(challenge);
            _context.SaveChanges();
            //send notifications
            if (challenge.Challengers.Count > 1)
            {
                Notification notification = new Notification
                {
                    Date = date,
                    Message = $"{userName} created a wager challenge with you.",
                    Link = $"/host/wagers/view/{wagerId}"
                };
                IEnumerable<string> users = challenge.ChallengerIds().Union(wager.HostIds());
                NotificationHandler.AddNotificationToUsers(_context, users, notification);
                await HubHandler.SendGroupAsync(_hubContext, users.ToList(), wager.GroupName, notification);
            }
            return Ok();
        }
    }
}