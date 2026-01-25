using System;
using System.Linq;
using System.Threading.Tasks;

using AMWE_RealTime_Server.Hubs;
using AMWE_RealTime_Server.Models;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AMWE_RealTime_Server.Services
{
    public class AuthService
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<ClientHandlerHub> _hubContext;

        public AuthService(ApplicationContext context, IHubContext<ClientHandlerHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task LogoutAsync(uint id)
        {
            ClientState a = _context.GlobalClientStatesList
            .Include(a => a.Client)
            .FirstOrDefault(x => x.Client.Id == id);

            if (a != null)
            {
                a.IsOnline = false;
                a.LastLogoutDateTime = DateTime.Now;
                await _hubContext.Clients.All.SendAsync("OnUserLeft", a);
                _ = await _context.SaveChangesAsync();
            }
        }
    }
}
