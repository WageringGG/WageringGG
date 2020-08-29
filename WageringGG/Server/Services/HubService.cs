using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using WageringGG.Server.Hubs;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Services
{
    public class HubService
    {
        private readonly IHubContext<GroupHub> _hubContext;

        public HubService(IHubContext<GroupHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<List<Notification>> SendNotificationsAsync(string[] ids, Notification notification)
        {
            List<Notification> notifications = new List<Notification>();
            foreach (string id in ids)
            {
                Notification personalNotification = new Notification
                {
                    Date = notification.Date,
                    Link = notification.Link,
                    Message = notification.Message,
                    ProfileId = id
                };
                notifications.Add(personalNotification);
            }
            await _hubContext.Clients.Groups(ids).SendAsync("ReceiveNotification", notification);
            return notifications;
        }

        public Task SendGroupAsync(string[] ids, string groupName)
        {
            return _hubContext.Clients.Groups(ids).SendAsync("ReceiveGroup", groupName);
        }
    }
}
