using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Hubs
{
    [Authorize]
    public class GroupHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public GroupHub(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<Profile> GetUserAsync(string id)
        {
            return await _context.Profiles.AsNoTracking().Where(x => x.Id == id)
                .Include(x => x.Connections)
                .FirstOrDefaultAsync();
        }
        public override async Task OnConnectedAsync()
        {
            var id = Context.UserIdentifier;
            var user = await GetUserAsync(id);

            if (user != null)
            {
                var connection = user.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (connection != null)
                {
                    connection.Connected = true;
                }
                else
                {
                    _context.Connections.Add(new Connection
                    {
                        ProfileId = id,
                        ConnectionId = Context.ConnectionId,
                        UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"],
                        Connected = true
                    });
                }
                _context.SaveChanges();
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Used on startup for adding user to groups
        /// </summary>
        /// <param name="groups"></param>
        /// <returns>async Task</returns>
        public async Task AddToGroups(string[] groups)
        {
            foreach (string groupName in groups)
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = await _context.Connections.FindAsync(Context.ConnectionId);
            connection.Connected = false;
            _context.SaveChanges();
            await base.OnDisconnectedAsync(exception);
        }
    }
}
