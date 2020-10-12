using AMWE_RealTime_Server.Controllers;
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
            AuthController.OnUserAuth += AuthController_OnUserAuth;
            _context = context;
        }

        private void AuthController_OnUserAuth(Client client)
        {

        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Role.GlobalUserRole))
            {

            }
            else if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.IsInRole(Role.GlobalUserRole))
            {

            }
            else if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admin");
            }
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize(Roles ="admin")]
        public int GetIntIfAdmin()
        {
            return 1;
        }
        [Authorize(Roles ="user")]
        public int GetIntIfUser()
        {
            return 2;
        }
    }
}