// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using AMWE_RealTime_Server.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
                    options.LoginPath = new PathString("/cookie/login");
                    options.AccessDeniedPath = new PathString("/cookie/login");
                    options.LogoutPath = new PathString("/cookie/logout");
                });

            //services.AddDbContext<VersionsContext>(options =>
            //    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VersionFiles;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=true"));

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ReportHub>("/report", options => {
                    options.ApplicationMaxBufferSize = 52428800;
                    options.WebSockets.CloseTimeout = TimeSpan.FromDays(1);
                    options.LongPolling.PollTimeout = TimeSpan.FromDays(1);
                });
                routes.MapHub<ClientHandlerHub>("/listen/clients", options => {
                    options.WebSockets.CloseTimeout = TimeSpan.FromDays(1);
                    options.LongPolling.PollTimeout = TimeSpan.FromDays(1);
                });
                //routes.MapHub<ServerHub>("/server", options => {
                //    options.ApplicationMaxBufferSize = 52428800;
                //});
                routes.MapHub<BotNetHub>("/botnet");
                routes.MapHub<UserToAdminChatHub>("/chat");
                routes.MapHub<DiagnoseHub>("/sandbox");
            });
            app.UseMvc();
        }
    }
}
