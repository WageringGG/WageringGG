﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
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

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendWagerHostBid(string groupName, WagerHostBid bid, WagerStatus status)
        {
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveWagerHostBid", bid, status);
        }

        public async Task SendWagerStatus(string groupName, WagerStatus status)
        {
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveWagerStatus", status);
        }
    }
}
