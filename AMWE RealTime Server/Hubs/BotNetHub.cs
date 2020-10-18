using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class BotNetHub : Hub
    {
        [Authorize(Roles = Role.GlobalAdminRole)]
        public void SendCommand(string command, params object[] args)
        {
            Clients.Group(Role.GlobalUserGroup).SendAsync("ExecuteCommand", command, args);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalUserGroup);
            }
            else if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalUserGroup);
            }
            else if(Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}