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
    public class BidController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HubService _hub;

        public BidController(ApplicationDbContext context, HubService hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPut("wager/{id}")]
        public async Task<IActionResult> SetWagerStatus([FromRoute] int id, [FromBody] bool value)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var member = await _context.WagerMembers.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Members).FirstOrDefaultAsync();
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
            else if (member.Wager.Members.Where(x => x.IsHost).All(x => x.IsApproved == true))
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

            var member = await _context.WagerMembers.Where(x => x.Id == id).Include(x => x.Challenge).ThenInclude(x => x.Members).Include(x => x.Challenge.Wager).FirstOrDefaultAsync();
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
            if (member.Challenge.Members.All(x => x.IsApproved == true))
            {
                /*KeyPair master = KeyPair.FromAccountId(_config["Stellar:SecretSeed"]);
                string startingBalance = ((member.Challenge.Challengers.Count + member.Challenge.Wager.PlayerCount + 1) * Stellar.BASE_RESERVE).ToString();
                CreateAccountOperation createAccount = new CreateAccountOperation.Builder(destination, startingBalance).SetSourceAccount(master).Build();
                transaction = new TransactionBuilder(account).AddMemo(Memo.Text($"Creating wager challenge account (id: {member.ChallengeId})."))
                    .AddOperation(createAccount).Build();
                transaction.Sign(master);
                await _server.SubmitTransaction(transaction);

                member.Challenge.Account = new StellarAccount()
                {
                    Balance = amount,
                    AccountId = destination.AccountId,
                    SecretSeed = destination.SecretSeed
                };
                */
                member.Challenge.Status = Status.Open;
                notification.Message = $"{userName} has confirmed the wager challenge.";

                Wager wager = await _context.Wagers.Where(x => x.Id == member.Challenge.WagerId).Include(x => x.Members).FirstOrDefaultAsync();
                wager.ChallengeCount++;

                //send hosts a notification
                Notification hostNotification = new Notification
                {
                    Date = date,
                    Message = "There is a new wager challenge.",
                    Link = $"/host/wagers/view/{member.Challenge.WagerId}"
                };
                List<Notification> hostNotifications = await _hub.SendNotificationsAsync(wager.HostIds().ToArray(), hostNotification);
                _context.AddRange(hostNotifications);
            }
            else
                notification.Message = $"{userName} has accepted the wager challenge.";
            string[] ids = member.Challenge.Ids().Where(x => x != userId).ToArray();
            List<Notification> notifications = await _hub.SendNotificationsAsync(ids, notification);
            _context.AddRange(notifications);
            _context.SaveChanges();
            return Ok(member.Challenge.Status);
        }

        //decline refund users their $$$
    }
}