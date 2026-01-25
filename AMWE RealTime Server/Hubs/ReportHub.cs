// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AMWE_RealTime_Server.Models;
using AMWE_RealTime_Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class ReportHub : Hub
    {
        private readonly ILogger _logger;

        private readonly IHubContext<ClientHandlerHub> _hubContext;
        private readonly IHubContext<AdminSystemHub> _admHubContext;
        private readonly ApplicationContext _context;
        private readonly AuthService _authService;

        public static readonly Dictionary<string, Client> ConnectedClients = new Dictionary<string, Client>();

        public ReportHub(ILogger<ReportHub> logger, IHubContext<ClientHandlerHub> hubContext, IHubContext<AdminSystemHub> admHubContext, ApplicationContext context, AuthService authService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _admHubContext = admHubContext;
            _context = context;
            _authService = authService;
        }

        public override async Task OnConnectedAsync()
        {
            string logMsg = $"Подключен клиент {Context.User.Identity.Name} {Context.ConnectionId} / Роль: {Context.User.Claims.First(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value}";
            _logger.LogInformation(logMsg);
            if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await _admHubContext.Clients.All.SendAsync("Log", $"Администратор {Context.User.Identity.Name} вошел в сеть");
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            else if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalUserGroup);
                uint id = Convert.ToUInt32(Context.User.Identity.Name.GetUntilOrEmpty("/")[3..]);
                string nameofpc = Context.User.Identity.Name[Context.User.Identity.Name.IndexOf('/')..];
                ConnectedClients.Add(Context.ConnectionId, new Client() { Id = id, Nameofpc = nameofpc });
            }
            await Clients.Caller.SendAsync("SetWorkday", (await _context.ReportHubState.FirstAsync()).WorkdayValue);
            await Clients.Caller.SendAsync("SetBaseSendingTime", (await _context.ReportHubState.FirstAsync()).BaseRepInterval);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogError($"От хаба был отключен клиент {Context.User.Identity.Name} по причине {exception?.Message}");
            _logger.LogDebug($"Подробности: {exception}");
            if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
                await _admHubContext.Clients.All.SendAsync("Log", $"Администратор {Context.User.Identity.Name} вышел из сети");
            }
            else if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalUserGroup);
                _ = ConnectedClients.TryGetValue(Context.ConnectionId, out Client client);
                await _authService.LogoutAsync(client.Id);
                _ = ConnectedClients.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize(Roles = Role.GlobalUserRole)]
        public async Task SendReport(Report report)
        {
            report.Server = new Uri($"{Context.GetHttpContext().Request.Scheme}://{Context.GetHttpContext().Request.Host}{Context.GetHttpContext().Request.Path}{Context.GetHttpContext().Request.QueryString}");
            report.Timestamp = DateTime.UtcNow;
            _logger.LogInformation($"Поступил отчет от {report.Client.Id} / {report.Client.Nameofpc} // Оценка: {report.OverallRating}");
            await Clients.Group(Role.GlobalAdminGroup).SendAsync("CreateReport", report);
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task SetWorkdayValue(bool value)
        {
            _context.ReportHubState.First().WorkdayValue = value;
            _ = await _context.SaveChangesAsync();
            string logMsg = $"Администратор {Context.User.Identity.Name} изменил состояние рабочего дня на {value}";
            await _admHubContext.Clients.All.SendAsync("Log", logMsg);
            _logger.LogInformation(logMsg);
            await Clients.All.SendAsync("SetWorkday", value);
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task ShutdownAllConnections()
        {
            await Clients.Group(Role.GlobalUserGroup).SendAsync("ShutdownHubConnection");
            IQueryable<ClientState> list = _context.GlobalClientStatesList.Where(x => x.IsOnline);
            int a = list.Count();
            uint[] b = list.Select(x => x.Client.Id).ToArray();
            for (uint i = 0; i < a; i++)
            {
                _ = await StaticVariables.SvControllers.FirstOrDefault()?.Logout(b[i]);
            }
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task EnhanceControl(uint clientID)
        {
            string address = ConnectedClients.FirstOrDefault(x => x.Value.Id == clientID).Key;
            ClientState x = _context.GlobalClientStatesList.First(x => x.Client.Id == clientID);
            if (!x.IsEnhanced)
            {
                x.IsEnhanced = true;
                await Clients.Client(address).SendAsync("EnhanceControl");
                string logMsg = $"Администратор {Context.User.Identity.Name} усилил контроль за сотрудником {clientID}: {x.Client.Nameofpc}";
                _logger.LogInformation(logMsg);
                await _admHubContext.Clients.All.SendAsync("Log", logMsg);
                await _hubContext.Clients.All.SendAsync("EnhanceControlForUser", clientID);
            }
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task LoosenControl(uint clientID)
        {
            string address = ConnectedClients.FirstOrDefault(x => x.Value.Id == clientID).Key;
            ClientState x = _context.GlobalClientStatesList.First(x => x.Client.Id == clientID);
            if (x.IsEnhanced)
            {
                x.IsEnhanced = false;
                await Clients.Client(address).SendAsync("LoosenControl");
                string logMsg = $"Администратор {Context.User.Identity.Name} ослабил контроль за сотрудником {clientID}: {x.Client.Nameofpc}";
                await _admHubContext.Clients.All.SendAsync("Log", logMsg);
                _logger.LogInformation(logMsg);
                await _hubContext.Clients.All.SendAsync("LoosenControlForUser", clientID);
            }
        }

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task UpdateReportPollingTime(TimeSpan timeSpan)
        {
            await Clients.All.SendAsync("SetBaseSendingTime", timeSpan);
            string logMsg = $"Администратор {Context.User.Identity.Name} изменил базовый интервал опроса отчетов с {(await _context.ReportHubState.FirstAsync()).BaseRepInterval} до {timeSpan}";
            await _admHubContext.Clients.All.SendAsync("Log", logMsg);
            _logger.LogInformation(logMsg);
            (await _context.ReportHubState.FirstAsync()).BaseRepInterval = timeSpan;
            _ = await _context.SaveChangesAsync();
        }

        public TimeSpan GetBaseReportPollingInterval()
        {
            return _context.ReportHubState.First().BaseRepInterval;
        }

        public bool GetWorkdayValue()
        {
            return _context.ReportHubState.First().WorkdayValue;
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }
    }
}