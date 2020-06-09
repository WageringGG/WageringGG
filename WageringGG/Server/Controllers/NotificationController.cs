using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Constants;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] long? lastDate = null)
        {
            string? userId = User.GetId();
            var query = _context.Notifications.Where(x => x.ProfileId == userId);
            if (lastDate.HasValue)
            {
                DateTime date = new DateTime(lastDate.Value);
                query = query.Where(x => x.Date > date);
            }
            IEnumerable<Notification> notifications = await query.OrderByDescending(x => x.Date).ToListAsync();
            return Ok(notifications);
        }

        [HttpDelete("{date}")]
        public async Task<IActionResult> DeleteNotification(long date)
        {
            string? userId = User.GetId();
            Notification notification = await _context.Notifications.FindAsync(new DateTime(date), userId);
            if (notification == null)
            {
                ModelState.AddModelError(string.Empty, Errors.NotFound);
                return BadRequest(ModelState.GetErrors());
            }
            _context.Remove(notification);
            _context.SaveChanges();
            return Ok();
        }
    }
}