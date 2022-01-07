// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using AMWE_RealTime_Server.Controllers;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HubCallerContextDict = System.Collections.Concurrent.ConcurrentDictionary<string, Microsoft.AspNetCore.SignalR.HubCallerContext>;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class ReportHub : Hub
    {
        static bool WorkdayValue = false;

        private readonly ILogger _logger;
        private readonly IHubContext<ClientHandlerHub> _hubContext;

        private readonly Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();

        public ReportHub(ILogger<ReportHub> logger, IHubContext<ClientHandlerHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Подключен клиент {Context.User.Claims.First(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value} {Context.ConnectionId} / Роль: {Context.User.Claims.First(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value}");
            if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            else if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalUserGroup);
            }
            await Clients.Caller.SendAsync("SetWorkday", WorkdayValue);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogError($"От хаба был отключен клиент {Context.User.Claims.First(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value} по причине {exception.Message}");
            _logger.LogDebug($"Подробности: {exception}");
            if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            else if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalUserGroup);
            }
            Client client;
            connectedClients.TryGetValue(Context.ConnectionId, out client);
            await StaticVariables.svControllers.FirstOrDefault()?.Logout(client.Id);
            connectedClients.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize(Roles = Role.GlobalUserRole)]
        public async void SendReport(Report report)
        {
            _logger.LogInformation($"Поступил отчет от {report.Client.Id} / {report.Client.Nameofpc} // Оценка: {report.OverallRating}");
            if (!connectedClients.TryGetValue(Context.ConnectionId, out _))
            {
                connectedClients.Add(Context.ConnectionId, report.Client);
            }
            await Clients.Group(Role.GlobalAdminGroup).SendAsync("CreateReport", report);
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async void SetWorkdayValue(bool value)
        {
            WorkdayValue = value;
            _logger.LogInformation($"Администратор {Context.User.Claims.First(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value} изменил значение Workday на {value}");
            await Clients.All.SendAsync("SetWorkday", value);
        }

        [Authorize(Roles=Role.GlobalAdminRole)]
        public async void ShutdownAllConnections()
        {
            await Clients.Group(Role.GlobalUserGroup).SendAsync("ShutdownHubConnection");
            var a = AuthController.GlobalUsersList.Count;
            var b = AuthController.GlobalUsersList.Select(x => x.Id).ToArray();
            for (uint i = 0; i < a; i++)
            {
                await StaticVariables.svControllers.FirstOrDefault()?.Logout(b[i]);
            }
        }

        public bool GetWorkdayValue()
        {
            return WorkdayValue;
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }
    }
}