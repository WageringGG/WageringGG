using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public BidController(ApplicationDbContext context, IHubContext<GroupHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("wager/accept/{id}")]
        public async Task<IActionResult> AcceptBid(int id)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var bid = await _context.WagerHostBids.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Hosts).FirstOrDefaultAsync();
            if (bid == null)
            {
                ModelState.AddModelError(string.Empty, "not_found");
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Wager.Status != (byte)Status.Created)
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
            if (bid.Wager != null)
            {
                PersonalNotification notification = new PersonalNotification
                {
                    Date = DateTime.Now,
                    Data = bid.Wager.Id.ToString(),
                    DataModel = (byte)DataModel.Wager
                };
                if (bid.Wager.IsApproved())
                {
                    bid.Wager.Status = (byte)Status.Confirmed;
                    notification.Message = $"{userName} has accepted and confirmed the wager.";
                }
                else
                    notification.Message = $"{userName} has accepted the wager.";
                List<PersonalNotification> notifications = NotificationHandler.AddNotificationToUsers(_context, bid.Wager.HostIds(), notification);
                await SignalRHandler.SendNotificationsAsync(_context, _hubContext, bid.Wager.HostIds(), notifications);
            }
            return Ok();
        }

        [HttpPost("wager/decline/{id}")]
        public async Task<IActionResult> DeclineBid(int id)
        {
            string? userId = User.GetId();
            string? userName = User.GetName();

            var bid = await _context.WagerHostBids.Where(x => x.Id == id).Include(x => x.Wager).ThenInclude(x => x.Hosts).FirstOrDefaultAsync();
            if (bid == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            if (bid.Wager.Status != (byte)Status.Created)
            {
                ModelState.AddModelError(string.Empty, "Wager is not in the created state.");
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
            PersonalNotification notification = new PersonalNotification
            {
                Date = DateTime.Now,
                Message = $"{userName} has declined the wager.",
                Data = bid.Wager.Id.ToString(),
                DataModel = (byte)DataModel.Wager
            };
            List<PersonalNotification> notifications = NotificationHandler.AddNotificationToUsers(_context, bid.Wager.HostIds(), notification);
            await SignalRHandler.SendNotificationsAsync(_context, _hubContext, bid.Wager.HostIds(), notifications);
            return Ok();
        }
    }
}