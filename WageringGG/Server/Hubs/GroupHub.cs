using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Hubs
{
    public class GroupHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var id = Context.UserIdentifier;
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            await base.OnConnectedAsync();
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendWagerHostBid(string groupName, WagerHostBid bid, IdStatus status)
        {
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveWagerHostBid", bid, status);
        }

        public async Task SendWagerStatus(string groupName, IdStatus status)
        {
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveWagerStatus", status);
        }
    }
}
