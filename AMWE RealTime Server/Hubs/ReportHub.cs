using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Hubs
{
    public class ReportHub : Hub
    {
        private ApplicationContext _context;

        public ReportHub(ApplicationContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Group("Admin").SendAsync("UpdateUserInfo", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Group("Admin").SendAsync("onUserDisconnected", Context.ConnectionId, exception);
            await base.OnDisconnectedAsync(exception);
        }
        [Authorize(Roles="admin")]
        public int GetIntIfAdmin()
        {
            return 1;
        }
    }
}