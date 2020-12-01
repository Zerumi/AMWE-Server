﻿// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AMWE_RealTime_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
      .UseKestrel()
      .UseContentRoot(Directory.GetCurrentDirectory())
      .ConfigureAppConfiguration((hostingContext, config) =>
      {
          var env = hostingContext.HostingEnvironment;
          config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                    optional: true, reloadOnChange: true);
          config.AddEnvironmentVariables();
      })
      .ConfigureLogging((hostingContext, logging) =>
      {
              // Requires `using Microsoft.Extensions.Logging;`
              logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
          logging.AddConsole();
          logging.AddDebug();
          logging.AddEventSourceLogger();
      })
      .UseStartup<Startup>()
      .Build();

            webHost.Run();
        }
    }
}