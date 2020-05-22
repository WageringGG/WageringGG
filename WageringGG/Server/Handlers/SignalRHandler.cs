using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Server.Hubs;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    public static class SignalRHandler
    {
        public static async Task SendNotificationsAsync(ApplicationDbContext _context, IHubContext<GroupHub> _hubContext, IEnumerable<string> ids, List<PersonalNotification> notifications)
        {
            List<Connection> connections = await _context.Connections.AsNoTracking().Where(x => x.Connected).Where(x => ids.Contains(x.ProfileId)).ToListAsync();
            foreach (Connection connection in connections)
            {
                PersonalNotification notification = notifications.FirstOrDefault(x => x.ProfileId == connection.ProfileId);
                if (notification != null)
                    await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("ReceiveNotification", notification);
            }
        }
    }
}