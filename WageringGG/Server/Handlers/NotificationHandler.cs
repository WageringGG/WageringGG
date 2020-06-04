using System.Collections.Generic;
using WageringGG.Server.Data;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    public static class NotificationHandler
    {
        /// <summary>
        /// Returns the set of notifications added to users with their corresponding DB Id.
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="userIds"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public static void AddNotificationToUsers(ApplicationDbContext _context, IEnumerable<string> userIds, Notification notification)
        {
            List<Notification> notifications = new List<Notification>();
            foreach (string id in userIds)
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
            if (notifications.Count > 0)
            {
                _context.Notifications.AddRange(notifications);
            }
        }
    }
}