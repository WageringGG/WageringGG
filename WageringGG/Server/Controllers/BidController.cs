using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
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
        private readonly stellar_dotnet_sdk.Server _server;
        private readonly IConfiguration _config;

        public BidController(ApplicationDbContext context, HubService hub, stellar_dotnet_sdk.Server server, IConfiguration config)
        {
            _context = context;
            _hub = hub;
            _server = server;
            _config = config;
        }

        [HttpPut("wager/accept/{id}")]
        public async Task<IActionResult> AcceptWager(int id)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var member = await _context.WagerMembers.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Members).FirstOrDefaultAsync();
            if (member == null)
            {
                ModelState.AddModelError(string.Empty, "not_found");
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

            member.IsApproved = true;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Link = $"/host/wagers/view/{member.WagerId}"
            };
            if (member.Wager.Members.Where(x => x.IsHost).All(x => x.IsApproved == true))
            {
                member.Wager.Status = Status.Confirmed;
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

        [HttpPut("wager/decline/{id}")]
        public async Task<IActionResult> DeclineWager(int id)
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
                ModelState.AddModelError(string.Empty, "Wager is not in the pending state.");
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
            member.IsApproved = false;
            member.Wager.Status = Status.Canceled;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Message = $"{userName} has declined the wager.",
                Link = $"/host/wagers/view/{member.WagerId}"
            };
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

            //check funds
            /*
            Asset asset = new AssetTypeNative();
            KeyPair source = KeyPair.FromSecretSeed(secretSeed);
            AccountResponse account = await _server.Accounts.Account(source.AccountId);
            decimal amount = member.Challenge.Amount / member.Challenge.Members.Count;
            string amountString = amount.ToString();
            if (account == null)
                return BadRequest(new string[] { "Your account could not be loaded." });
            Balance balance = account.Balances.FirstOrDefault(x => asset.Equals(x.Asset));
            if (!decimal.TryParse(balance?.BalanceString, out decimal balanceAmount))
                return BadRequest(new string[] { "Your account has insufficient funds." });

            KeyPair destination = KeyPair.FromAccountId(_config["Stellar:PublicKey"]);
            PaymentOperation payment = new PaymentOperation.Builder(destination, asset, amountString).Build();
            Transaction transaction = new TransactionBuilder(account)
                .AddOperation(payment).AddMemo(Memo.Text($"wager challenge {member.ChallengeId}")).Build();
            transaction.Sign(source);
            SubmitTransactionResponse transactionResponse = await _server.SubmitTransaction(transaction);
            if (!transactionResponse.IsSuccess())
                return BadRequest(new string[] { "The transaction was not successful." }); //why was transaction not successful
            */

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
                member.Challenge.Status = Status.Confirmed;
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