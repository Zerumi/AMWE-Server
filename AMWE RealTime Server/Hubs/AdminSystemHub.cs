using System;
using System.Threading.Tasks;

using AMWE_RealTime_Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize(Roles = Role.GlobalAdminRole)]
    public class AdminSystemHub : Hub
    {
        private static uint s_count = 0;

        public override async Task OnConnectedAsync()
        {
            s_count++;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            s_count--;
            await base.OnDisconnectedAsync(exception);
        }

        public string GetServerInfo()
        {
            string info = $"AMWE Main Server by Zerumi\nКоличество администраторов в сети: {s_count}\nЗапущен на базе {Environment.OSVersion} // {Environment.MachineName}\nЗапуск: {new TimeSpan(Environment.TickCount)} (Процессоров: {Environment.ProcessorCount})\n64bit: {Environment.Is64BitOperatingSystem}, {Environment.Is64BitProcess}\nРабочий набор:{Environment.WorkingSet / 8 / 1024.0d / 1024.0d} MB";
            return info;
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }
    }
}