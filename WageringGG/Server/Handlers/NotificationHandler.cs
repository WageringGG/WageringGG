using System.Collections.Generic;
using WageringGG.Server.Data;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    public static class NotificationHandler
    {
        public static List<PersonalNotification> AddNotificationToUsers(ApplicationDbContext _context, IEnumerable<string> userIds, PersonalNotification notification)
        {
            List<PersonalNotification> notifications = new List<PersonalNotification>();
            foreach (string id in userIds)
            {
                PersonalNotification personalNotification = new PersonalNotification
                {
                    Date = notification.Date,
                    DataModel = notification.DataModel,
                    Data = notification.Data,
                    Message = notification.Message,
                    ProfileId = id
                };
                notifications.Add(personalNotification);
            }
            if (notifications.Count > 0)
            {
                _context.Notifications.AddRange(notifications);
                _context.SaveChanges();
            }
            return notifications;
        }
    }
}