using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Services;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionService _transactionService;
        private readonly HubService _hub;

        public EntryController(ApplicationDbContext context, HubService hub, TransactionService transactionService)
        {
            _context = context;
            _hub = hub;
            _transactionService = transactionService;
        }

        [HttpPost("buy/wager/{id}")]
        public async Task<IActionResult> BuyWagerEntry([FromRoute] int id, [FromBody] string secretSeed, [FromQuery] int amount = 1)
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

        [HttpPost("refund/wager/{id}")]
        public async Task<IActionResult> RefundWagerEntry([FromRoute] int id, [FromQuery] int amount = 1)
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
            if (member.Entries < amount)
                return BadRequest(new string[] { "Member does not have sufficient entries to refund." });
            TransactionReceipt receipt = new TransactionReceipt
            {
                Amount = member.EntryAmount(member.Wager.Amount) * amount,
                Date = DateTime.Now,
                Data = $"Funding wager {id}",
                ProfileId = userId
            };
            KeyPair destination = KeyPair.FromAccountId(userKey);
            if (!await _transactionService.RefundFunds(_context, destination, receipt))
                return BadRequest();
            member.Entries -= amount;
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut("wager/{id}")]
        public async Task<IActionResult> SetWagerStatus([FromRoute] int id, [FromBody] bool value)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var member = await _context.WagerHosts.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Hosts).FirstOrDefaultAsync();
            if (member == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (member.Wager.Status != Status.Pending)
            {
                ModelState.AddModelError(string.Empty, "Wager is not in the created state.");
                return BadRequest(ModelState.GetErrors());
            }
            if (member.ProfileId != userId)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (member.IsApproved != null)
            {
                ModelState.AddModelError(string.Empty, Errors.AlreadySent);
                return BadRequest(ModelState.GetErrors());
            }

            member.IsApproved = value;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Link = $"/host/wagers/view/{member.WagerId}"
            };
            if (value == false)
            {
                member.Wager.Status = Status.Canceled;
                notification.Message = $"{userName} has declined the wager.";
            }
            else if (member.Wager.Hosts.All(x => x.IsApproved == true))
            {
                member.Wager.Status = Status.Open;
                notification.Message = $"{userName} has confirmed the wager.";
            }
            else
                notification.Message = $"{userName} has accepted the wager.";
            string[] ids = member.Wager.HostIds().Where(x => x != userId).ToArray();
            List<Notification> notifications = await _hub.SendNotificationsAsync(ids, notification);
            _context.Notifications.AddRange(notifications);
            _context.SaveChanges();
            return Ok(member.Wager.Status);
        }

        [HttpPut("wager/challenge/accept/{id}")]
        public async Task<IActionResult> AcceptWagerChallenge(int id, [FromBody] string secretSeed)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            string? userKey = User.GetKey();

            var member = await _context.WagerChallengers.Where(x => x.Id == id).Include(x => x.Challenge).ThenInclude(x => x.Challengers).FirstOrDefaultAsync();
            if (member == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (member.Challenge.Status != (byte)Status.Pending)
            {
                ModelState.AddModelError(string.Empty, "Challenge is not in the pending state.");
                return BadRequest(ModelState.GetErrors());
            }
            if (member.ProfileId != userId)
            {
                ModelState.AddModelError(string.Empty, Errors.NotCorresponding);
                return BadRequest(ModelState.GetErrors());
            }
            if (member.IsApproved != null)
            {
                ModelState.AddModelError(string.Empty, Errors.AlreadySent);
                return BadRequest(ModelState.GetErrors());
            }

            member.IsApproved = true;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Link = $"/client/wagers/view/{member.ChallengeId}"
            };
            if (member.Challenge.Challengers.All(x => x.IsApproved == true))
            {
                member.Challenge.Status = Status.Open;
                notification.Message = $"{userName} has confirmed the wager challenge.";

                //send challengers a notification
                //send hosts a notifications
            }
            else
                notification.Message = $"{userName} has accepted the wager challenge.";
            string[] ids = member.Challenge.ChallengerIds().Where(x => x != userId).ToArray();
            List<Notification> notifications = await _hub.SendNotificationsAsync(ids, notification);
            _context.AddRange(notifications);
            _context.SaveChanges();
            return Ok(member.Challenge.Status);
        }

        //decline refund users their $$$
    }
}