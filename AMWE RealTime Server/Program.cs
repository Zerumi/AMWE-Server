// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AMWE_RealTime_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost webHost = new WebHostBuilder()
      .UseKestrel()
      .UseContentRoot(Directory.GetCurrentDirectory())
      .ConfigureAppConfiguration((hostingContext, config) =>
      {
          IWebHostEnvironment env = hostingContext.HostingEnvironment;
          _ = config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                    optional: true, reloadOnChange: true);
          _ = config.AddEnvironmentVariables();
      })
      .ConfigureLogging((hostingContext, logging) =>
      {
          // Requires `using Microsoft.Extensions.Logging;`
          _ = logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
          _ = logging.AddConsole();
          _ = logging.AddDebug();
          _ = logging.AddEventSourceLogger();
      })
      .UseStartup<Startup>()
      .Build();
            if (args.Contains("--ef"))
            {
                // EF требует DbContext, но мы не запускаем сервер
                return;
            }
            webHost.Run();
        }
    }
}