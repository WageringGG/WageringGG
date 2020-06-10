using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Hubs;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    public static class HubHandler
    {
        public static async Task SendGroupAsync(IHubContext<GroupHub> hubContext, IEnumerable<string> ids, string groupName)
        {
            await hubContext.Clients.Groups(ids.ToList()).SendAsync("ReceiveGroup", groupName);
        }

        public static async Task SendNotificationsAsync(IHubContext<GroupHub> hubContext, IEnumerable<string> ids, Notification notification)
        {
            await hubContext.Clients.Groups(ids.ToList()).SendAsync("ReceiveNotification", notification);
        }
    }
}