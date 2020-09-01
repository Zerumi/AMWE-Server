using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AMWE_RealTime_Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
                Report report = new Report()
                {
                    Client = new Client()
                    {
                        Id = 0,
                        Nameofpc = "Zerumi"
                    },
                    DangerLevel = new AlarmList()
                    {
                        OverallRating = 0.7m,
                        KeyboardRating = 1,
                        MouseRating = 0,
                        ProcessRating = 1
                    },
                    ReportDetails = new ReportDetails()
                    {
                        KeyboardDetails = new KeyboardDetails()
                        {
                            KeyPressedCount = 23,
                            PressedInfo = new KeyPressedInfo[]
                            {
                                new KeyPressedInfo()
                                {
                                    Key = "F4",
                                    PressTimes = 1
                                },
                                new KeyPressedInfo()
                                {
                                    Key = "Alt",
                                    PressTimes = 1
                                },
                                new KeyPressedInfo()
                                {
                                    Key = "V",
                                    PressTimes = 17
                                },
                                new KeyPressedInfo()
                                {
                                    Key = "I",
                                    PressTimes = 4
                                }
                            }
                        },
                        MouseDetails = new MouseDetails()
                        {
                            isMouseCoordChanged = false
                        },
                        ProcessesDetails = new ProcessesDetails()
                        {
                            ChangesCount = 1,
                            ProcessesNow = new string[] { "System", "notepad.exe" },
                            ProccesChanges = new ProccesChange[]
                            {
                                new ProccesChange()
                                {
                                    Action = ProcessAction.Created,
                                    ProcessName = "notepad.exe"
                                }
                            }
                        }
                    }
                };
            });
        }
    }
}
