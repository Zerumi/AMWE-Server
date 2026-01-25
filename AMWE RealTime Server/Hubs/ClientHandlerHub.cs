// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Threading.Tasks;

using AMWE_RealTime_Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize(Roles = Role.GlobalAdminRole)]
    public class ClientHandlerHub : Hub
    {
        private readonly ApplicationContext _context;

        public ClientHandlerHub(ApplicationContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("GetAllClients", await _context.GlobalClientStatesList.ToListAsync());
            await base.OnConnectedAsync();
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }
    }
}