using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Hubs
{
    [Authorize]
    public class GroupHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var id = Context.UserIdentifier;
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Used on startup for adding user to groups
        /// </summary>
        /// <param name="groups">User's list of groups</param>
        /// <returns>async Task</returns>
        public async Task AddToGroups(string[] groups)
        {
            foreach (string groupName in groups)
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendNotifications(string groupName, PersonalNotification notification)
        {
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveNotification", notification);
        }

        public async Task SendWagerHostBid(string groupName, byte status, WagerHostBid bid, PersonalNotification notification)
        {
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveWagerHostBid", status, bid, notification);
        }
    }
}
