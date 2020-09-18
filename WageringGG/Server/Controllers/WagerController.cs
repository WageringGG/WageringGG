using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
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
        private readonly TransactionService _transactionService;
        private readonly HubService _hub;
        private readonly IConfiguration _config;
        private const int ResultSize = 16;

        public WagerController(ApplicationDbContext context, HubService hub, TransactionService transactionService, IConfiguration config)
        {
            _context = context;
            _transactionService = transactionService;
            _hub = hub;
            _config = config;
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

            IQueryable<Wager> wagerQuery = _context.Wagers.AsNoTracking().Where(x => x.GameId == gameId).Where(x => !x.IsPrivate).Where(x => x.Status == Status.Open);

            if (playerCount.HasValue)
                wagerQuery = wagerQuery.Where(x => x.PlayerCount == playerCount);
            if (minimumWager.HasValue)
                wagerQuery = wagerQuery.Where(x => x.Amount > minimumWager);
            if (maximumWager.HasValue)
                wagerQuery = wagerQuery.Where(x => x.Amount < maximumWager);
            if (displayName != null)
            {
                displayName = displayName.ToUpper();
                wagerQuery = wagerQuery.Include(x => x.Members).ThenInclude(x => x.Profile).Where(x => x.Members.Any(x => x.Profile.NormalizedDisplayName.Contains(displayName)));
            }
            PaginatedList<Wager> results = await Paginator<Wager>.CreateAsync(wagerQuery.OrderByDescending(x => x.Date), page ?? 1, ResultSize);
            return Ok(results);
        }

        [HttpPut("status/{id}")]
        [Authorize]
        public async Task<IActionResult> SetStatus(int id, [FromBody] Status status)
        {
            string? userId = User.GetId();
            var wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Members).Where(x => x.Members.Any(y => y.ProfileId == userId && y.IsHost)).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { Errors.NotFound });
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
            var wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == id).Include(x => x.Members).ThenInclude(x => x.Profile).Include(x => x.Challenges).FirstOrDefaultAsync();
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
            Wager wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Members).Include(x => x.Challenges).FirstOrDefaultAsync();

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
            if (!wager.Members.Any(x => x.ProfileId == userId && x.IsHost == true))
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

            Wager wager = await _context.Wagers.Where(x => x.Id == id).Include(x => x.Members).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { Errors.NotFound });
            if (!wager.Members.Any(x => x.ProfileId == userId && x.IsHost == true))
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
        public async Task<IActionResult> CreateWager(Wager wagerData)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            string? userKey = User.GetKey();
            if (userKey == null)
                ModelState.AddModelError(string.Empty, "You do not have a public key registered.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            if (wagerData.Members.Sum(x => x.ReceivablePercentage) != 100)
                ModelState.AddModelError(string.Empty, "The receivable percentages do not add up to 100.");
            if (wagerData.Members.Sum(x => x.PayablePercentage) != 100)
                ModelState.AddModelError(string.Empty, "The payable percentages do not add up to 100.");
            if (!wagerData.Members.Any(x => x.ProfileId == userId))
                ModelState.AddModelError(string.Empty, "Caller must be a host.");
            if (!wagerData.HostIds().IsUnique())
                ModelState.AddModelError(string.Empty, "The id's are not unique.");
            if (wagerData.Amount <= 0)
                ModelState.AddModelError(string.Empty, "The wager amount has to be greater than 0.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            DateTime date = DateTime.Now;
            Wager wager = new Wager //prevents overposting
            {
                GameId = wagerData.GameId,
                Date = date,
                Title = wagerData.Title,
                Description = wagerData.Description,
                Amount = wagerData.Amount,
                IsPrivate = wagerData.IsPrivate,
                Status = Status.Pending,
                ChallengeCount = 0,
                PlayerCount = wagerData.Members.Count,
                Members = new List<WagerMember>()
            };

            foreach (WagerMember host in wagerData.Members)
            {
                WagerMember member = new WagerMember
                {
                    IsApproved = null,
                    ReceivablePercentage = host.ReceivablePercentage,
                    PayablePercentage = host.PayablePercentage,
                    ProfileId = host.ProfileId,
                    IsHost = true
                };
                if (member.ProfileId == userId)
                    member.IsApproved = true;
                wager.Members.Add(member);
            }
            if (wager.Members.All(x => x.IsApproved == true))
                wager.Status = Status.Open;
            //if confirmed start receiving funds
            _context.Wagers.Add(wager);
            _context.SaveChanges();
            if (wager.Members.Count > 1)
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
        public async Task<IActionResult> CreateChallenge([FromRoute] int wagerId, [FromBody] WagerChallenge challengeData)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            //check funds
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            if (challengeData.Members.Sum(x => x.ReceivablePercentage) != 100)
                ModelState.AddModelError(string.Empty, "The receivable percentages do not add up to 100.");
            if (challengeData.Members.Sum(x => x.PayablePercentage) != 100)
                ModelState.AddModelError(string.Empty, "The payable percentages do not add up to 100.");
            if (!challengeData.Members.Any(x => x.ProfileId == userId))
                ModelState.AddModelError(string.Empty, "Caller must be a host.");
            if (!challengeData.Members.IsUnique())
                ModelState.AddModelError(string.Empty, "The id's are not unique.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            Wager wager = await _context.Wagers.AsNoTracking().Where(x => x.Id == wagerId).Include(x => x.Members).FirstOrDefaultAsync();
            if (wager == null)
                return BadRequest(new string[] { "The wager could not be found." });
            if (wager.Status != Status.Open)
                ModelState.AddModelError(string.Empty, "The wager is not currently accepting challenges.");
            if (wager.HostIds().Intersect(challengeData.Ids()).Count() > 0)
                ModelState.AddModelError(string.Empty, "A wager host cannot challenge themself.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());

            DateTime date = DateTime.Now;
            WagerChallenge challenge = new WagerChallenge
            {
                Date = date,
                IsAccepted = false,
                Status = Status.Pending,
                WagerId = wagerId,
                Members = new List<WagerMember>()
            };
            foreach (WagerMember challenger in challengeData.Members)
            {
                WagerMember member = new WagerMember
                {
                    IsApproved = null,
                    PayablePercentage = challenger.PayablePercentage,
                    ReceivablePercentage = challenger.ReceivablePercentage,
                    ProfileId = challenger.ProfileId,
                    WagerId = wagerId
                };
                challenge.Members.Add(member);
            }

            _context.WagerChallenges.Add(challenge);
            _context.SaveChanges();

            if (challenge.Members.Count > 1)
            {
                string[] ids = challenge.Ids().ToArray();
                Notification notification = new Notification
                {
                    Date = date,
                    Message = $"{userName} created a wager challenge with you.",
                    Link = $"/client/wagers/view/{wagerId}"
                };
                List<Notification> notifications = await _hub.SendNotificationsAsync(ids.Where(x => x != userId).ToArray(), notification);
                _context.Notifications.AddRange(notifications);
                await _hub.SendGroupAsync(ids, Wager.Group(wager.Id));
            }
            return Ok(challenge.Id);
        }

        [HttpPost("entry/{id}")]
        public async Task<IActionResult> BuyEntry([FromRoute] int id, [FromBody] string secretSeed, [FromQuery] int amount = 1)
        {
            string? userKey = User.GetKey();
            string? userId = User.GetId();
            string? userName = User.GetName();
            if (userKey == null)
                ModelState.AddModelError(string.Empty, "You do not have a public key registered.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            WagerMember member = await _context.WagerMembers.Where(x => x.WagerId == id).Where(x => x.ProfileId == userId).Include(x => x.Wager).FirstOrDefaultAsync();
            if (member == null)
                return BadRequest(new string[] { "You are not a member of this wager." });
            TransactionReceipt receipt = new TransactionReceipt
            {
                Amount = member.EntryAmount(member.Wager.Amount) * amount,
                Date = DateTime.Now,
                Data = $"Funding wager {id}",
                ProfileId = userId
            };
            KeyPair source = KeyPair.FromSecretSeed(secretSeed);
            if (source.AccountId != userKey)
                return BadRequest("Your registered stellar key and secret seed do not match.");
            if (!await _transactionService.ReceiveFunds(_context, source, receipt))
                return BadRequest();
            member.Entries += amount;
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("refund/{id}")]
        public async Task<IActionResult> RefundEntry([FromRoute] int id, [FromQuery] int amount = 1)
        {
            string? userKey = User.GetKey();
            string? userId = User.GetId();
            string? userName = User.GetName();
            if (userKey == null)
                ModelState.AddModelError(string.Empty, "You do not have a public key registered.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrors());
            WagerMember member = await _context.WagerMembers.Where(x => x.WagerId == id).Where(x => x.ProfileId == userId).Include(x => x.Wager).FirstOrDefaultAsync();
            if (member == null)
                return BadRequest(new string[] { "You are not a member of this wager." });

            return Ok();
        }
    }
}