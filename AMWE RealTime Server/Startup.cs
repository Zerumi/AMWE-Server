// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AMWE_RealTime_Server.Hubs;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AMWE_RealTime_Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("AppContext"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                    options.LogoutPath = new PathString("/Account/Logout");
                });

            //services.AddDbContext<VersionsContext>(options =>
            //    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VersionFiles;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=true"));

            services.AddSignalR(options => {
                options.KeepAliveInterval = TimeSpan.FromDays(1);
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromDays(1);
            }).AddHubOptions<ClientHandlerHub>(options => {
                options.KeepAliveInterval = TimeSpan.FromDays(1);
                options.HandshakeTimeout = TimeSpan.FromDays(1);
            }).AddHubOptions<ReportHub>(options => {
                options.KeepAliveInterval = TimeSpan.FromDays(1);
                options.HandshakeTimeout = TimeSpan.FromDays(1);
            });
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ReportHub>("/report");
                routes.MapHub<ClientHandlerHub>("/listen/clients");
                //routes.MapHub<ServerHub>("/server", options => {
                //    options.ApplicationMaxBufferSize = 52428800;
                //});
                routes.MapHub<BotNetHub>("/botnet");
                routes.MapHub<UserToAdminChatHub>("/chat");
            });
            app.UseMvc();
        }
    }
}
