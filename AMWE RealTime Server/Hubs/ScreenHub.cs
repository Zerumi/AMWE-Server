// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AMWE_RealTime_Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class ScreenHub : Hub
    {
        private readonly ILogger _logger;

        private static readonly List<ScreenState> ScreenTransfer = new List<ScreenState>();

        public ScreenHub(ILogger<Screen> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
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
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            }
            else if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async void RequestScreen(Client client, ScreenType screenType)
        {
            try
            {
                ScreenTransfer.Add(new ScreenState()
                {
                    Cid = client.Id,
                    Adm = Context.ConnectionId,
                    Type = screenType
                });
                await Clients.Group($"ID {client.Id}/" + client.Nameofpc).SendAsync("RequestScreen", screenType);
                _logger.LogInformation($"Отправился запрос на скриншот {screenType} у {client.Id} / {client.Nameofpc}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = Role.GlobalUserRole)]
        public async void TransferScreen(Screen screen, ScreenType type)
        {
            uint cid = Convert.ToUInt32(Context.User.Identity.Name.GetUntilOrEmpty("/")[3..]);
            List<ScreenState> admIds = ScreenTransfer.FindAll(x => x.Cid == cid && x.Type == type);
            _logger.LogInformation($"Клиент {cid} ответил на запрос {type} у одного или нескольких администраторов ({admIds.Count})");
            foreach (ScreenState a in admIds)
            {
                await Clients.Client(a.Adm).SendAsync("NewScreen", screen, ReportHub.ConnectedClients.FirstOrDefault(x => x.Value.Id == cid).Value, type);
                _ = ScreenTransfer.Remove(a);
            }
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }
    }
}