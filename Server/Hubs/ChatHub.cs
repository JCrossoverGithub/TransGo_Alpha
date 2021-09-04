using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TransGo_Alpha.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task AddClass(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendGroupMessage(string groupName, int isfinal, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", isfinal, message);
        }
        public async Task SendMessage(int isfinal, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", isfinal, message);
        }
    }
}
