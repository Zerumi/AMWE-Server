using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize(Roles=Role.GlobalAdminRole)]
    public class AdminSystemHub : Hub
    {
        static uint count = 0;

        public override async Task OnConnectedAsync()
        {
            count++;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            count--;
            await base.OnDisconnectedAsync(exception);
        }

        public string GetServerInfo()
        {
            string info = $"AMWE Main Server by Zerumi\nКоличество администраторов в сети: {count}\nЗапущен на базе {Environment.OSVersion} // {Environment.MachineName} (Процессоров: {Environment.ProcessorCount})\nАптайм:{new TimeSpan(Environment.TickCount)}\n64bit: {Environment.Is64BitOperatingSystem}, {Environment.Is64BitProcess}\nРабочий набор:{Environment.WorkingSet / 8 / 1024.0d / 1024.0d} MB";
            return info;
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }
    }
}
