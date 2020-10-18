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
    [Authorize(Roles = Role.GlobalAdminRole)]
    public class ClientHandlerHub : Hub
    {
        public ClientHandlerHub()
        {
            AuthController.OnClientLogin += AuthController_OnUserAuth;
        }

        private void AuthController_OnUserAuth(Client client)
        {
            Clients.All.SendAsync("OnUserAuth", client);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("GetAllClients", AuthController.GlobalUsersList);
            await base.OnConnectedAsync();
        }
    }
}