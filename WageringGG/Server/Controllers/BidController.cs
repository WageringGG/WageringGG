﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Handlers;
using WageringGG.Server.Hubs;
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
        private readonly IHubContext<GroupHub> _hubContext;
        private readonly stellar_dotnet_sdk.Server _server;
        private readonly IConfiguration _config;

        public BidController(ApplicationDbContext context, IHubContext<GroupHub> hubContext, stellar_dotnet_sdk.Server server, IConfiguration config)
        {
            _context = context;
            _hubContext = hubContext;
            _server = server;
            _config = config;
        }

        [HttpPut("wager/accept/{id}")]
        public async Task<IActionResult> AcceptWager(int id)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var bid = await _context.WagerHostBids.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Hosts).FirstOrDefaultAsync();
            if (bid == null)
            {
                ModelState.AddModelError(string.Empty, "not_found");
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Wager.Status != (byte)Status.Pending)
            {
                ModelState.AddModelError(string.Empty, "Wager is not in the created state.");
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.ProfileId != userId)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Approved != null)
            {
                ModelState.AddModelError(string.Empty, Errors.AlreadySent);
                return BadRequest(ModelState.GetErrors());
            }

            bid.Approved = true;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Link = $"/host/wagers/view/{bid.WagerId}"
            };
            if (bid.Wager.IsApproved())
            {
                bid.Wager.Status = (byte)Status.Confirmed;
                notification.Message = $"{userName} has confirmed the wager.";
            }
            else
                notification.Message = $"{userName} has accepted the wager.";
            IEnumerable<string> otherHosts = bid.Wager.HostIds().Where(x => x != userId);
            await NotificationHandler.AddNotificationToUsers(_context, _hubContext, otherHosts, notification);
            _context.SaveChanges();
            return Ok(bid.Wager.Status);
        }

        [HttpPut("wager/decline/{id}")]
        public async Task<IActionResult> DeclineWager(int id)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var bid = await _context.WagerHostBids.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Hosts).FirstOrDefaultAsync();
            if (bid == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Wager.Status != (byte)Status.Pending)
            {
                ModelState.AddModelError(string.Empty, "Wager is not in the pending state.");
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.ProfileId != userId)
            {
                ModelState.AddModelError(string.Empty, Errors.NotCorresponding);
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Approved != null)
            {
                ModelState.AddModelError(string.Empty, Errors.AlreadySent);
                return BadRequest(ModelState.GetErrors());
            }
            bid.Approved = false;
            bid.Wager.Status = (byte)Status.Canceled;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Message = $"{userName} has declined the wager.",
                Link = $"/host/wagers/view/{bid.WagerId}"
            };
            IEnumerable<string> otherHosts = bid.Wager.HostIds().Where(x => x != userId);
            await NotificationHandler.AddNotificationToUsers(_context, _hubContext, otherHosts, notification);
            _context.SaveChanges();
            return Ok(bid.Wager.Status);
        }

        [HttpPut("wager/challenge/accept/{id}")]
        public async Task<IActionResult> AcceptWagerChallenge(int id, [FromBody] string secretSeed)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();
            string? userKey = User.GetKey();

            var bid = await _context.WagerChallengeBids.Where(x => x.Id == id).Include(x => x.Challenge).ThenInclude(x => x.Challengers).Include(x => x.Challenge.Account).Include(x => x.Challenge.Wager).FirstOrDefaultAsync();
            if (bid == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Challenge.Status != (byte)Status.Pending)
            {
                ModelState.AddModelError(string.Empty, "Challenge is not in the pending state.");
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.ProfileId != userId)
            {
                ModelState.AddModelError(string.Empty, Errors.NotCorresponding);
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Approved != null)
            {
                ModelState.AddModelError(string.Empty, Errors.AlreadySent);
                return BadRequest(ModelState.GetErrors());
            }

            //check funds
            Asset asset = new AssetTypeNative();
            KeyPair source = KeyPair.FromSecretSeed(secretSeed);
            AccountResponse account = await _server.Accounts.Account(source.AccountId);
            decimal amount = bid.Challenge.Amount / bid.Challenge.Challengers.Count;
            string amountString = amount.ToString();
            if (account == null)
                return BadRequest(new string[] { "Your account could not be loaded." });
            Balance balance = account.Balances.FirstOrDefault(x => asset.Equals(x.Asset));
            if (!decimal.TryParse(balance?.BalanceString, out decimal balanceAmount))
                return BadRequest(new string[] { "Your account has insufficient funds." });

            KeyPair destination = KeyPair.FromAccountId(_config["Stellar:PublicKey"]);
            PaymentOperation payment = new PaymentOperation.Builder(destination, asset, amountString).Build();
            Transaction transaction = new TransactionBuilder(account)
                .AddOperation(payment).AddMemo(Memo.Text($"wager challenge {bid.ChallengeId}")).Build();
            transaction.Sign(source);
            SubmitTransactionResponse transactionResponse = await _server.SubmitTransaction(transaction);
            if (!transactionResponse.IsSuccess())
                return BadRequest(new string[] { "The transaction was not successful." }); //why was transaction not successful

            bid.Approved = true;
            DateTime date = DateTime.Now;
            Notification notification = new Notification
            {
                Date = date,
                Link = $"/client/wagers/view/{bid.ChallengeId}"
            };
            if (bid.Challenge.IsApproved())
            {
                KeyPair master = KeyPair.FromAccountId(_config["Stellar:SecretSeed"]);
                string startingBalance = ((bid.Challenge.Challengers.Count + bid.Challenge.Wager.PlayerCount + 1) * Stellar.BASE_RESERVE).ToString();
                CreateAccountOperation createAccount = new CreateAccountOperation.Builder(destination, startingBalance).SetSourceAccount(master).Build();
                transaction = new TransactionBuilder(account).AddMemo(Memo.Text($"Creating wager challenge account (id: {bid.ChallengeId})."))
                    .AddOperation(createAccount).Build();
                transaction.Sign(master);
                await _server.SubmitTransaction(transaction);

                bid.Challenge.Account = new StellarAccount()
                {
                    Balance = amount,
                    AccountId = destination.AccountId,
                    SecretSeed = destination.SecretSeed
                };

                bid.Challenge.Status = (byte)Status.Confirmed;
                notification.Message = $"{userName} has confirmed the wager challenge.";

                Wager wager = await _context.Wagers.Where(x => x.Id == bid.Challenge.WagerId).Include(x => x.Hosts).FirstOrDefaultAsync();
                wager.ChallengeCount++;
                Notification hostNotification = new Notification
                {
                    Date = date,
                    Message = "There is a new wager challenge.",
                    Link = $"/host/wagers/view/{bid.Challenge.WagerId}"
                };
                await NotificationHandler.AddNotificationToUsers(_context, _hubContext, wager.HostIds(), hostNotification);
            }
            else
                notification.Message = $"{userName} has accepted the wager challenge.";
            IEnumerable<string> otherHosts = bid.Challenge.ChallengerIds().Where(x => x != userId);
            await NotificationHandler.AddNotificationToUsers(_context, _hubContext, otherHosts, notification);
            _context.SaveChanges();
            return Ok(bid.Challenge.Status);
        }

        //decline refund users their $$$
    }
}