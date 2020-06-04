using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using WageringGG.Server.Hubs;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    public static class HubHandler
    {
        public static async Task SendGroupAsync(IHubContext<GroupHub> hubContext, List<string> ids, string groupName, Notification notification)
        {
            await hubContext.Clients.Groups(ids).SendAsync("ReceiveGroup", groupName, notification);
        }

        public static async Task SendNotificationsAsync(IHubContext<GroupHub> hubContext, List<string> ids, Notification notification)
        {
            await hubContext.Clients.Groups(ids).SendAsync("ReceiveNotification", notification);
        }
    }
}