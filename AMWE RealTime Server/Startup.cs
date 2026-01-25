// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;

using AMWE_RealTime_Server.Hubs;
using AMWE_RealTime_Server.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AMWE_RealTime_Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddScoped<AuthService>();

            _ = services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(
                Configuration.GetConnectionString("DefaultConnection")
            ));

            _ = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/cookie/login");
                    options.AccessDeniedPath = new PathString("/cookie/login");
                    options.LogoutPath = new PathString("/cookie/logout");
                });

            _ = services.AddMvc();
            _ = services.AddSignalR(options => options.EnableDetailedErrors = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                ApplicationContext db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                db.Database.Migrate(); // <- автоматом применяет все миграции
            }

            CookiePolicyOptions cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };

            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }

            _ = app.UseAuthentication();

            _ = app.UseCookiePolicy(cookiePolicyOptions);

            _ = app.UseWebSockets();
            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapHub<ReportHub>("/report", options =>
                {
                    options.ApplicationMaxBufferSize = 52428800;
                    options.WebSockets.CloseTimeout = TimeSpan.FromDays(1);
                });
                _ = endpoints.MapHub<ScreenHub>("/screen", options =>
                {
                    options.ApplicationMaxBufferSize = 52428800;
                    options.WebSockets.CloseTimeout = TimeSpan.FromDays(1);
                });
                _ = endpoints.MapHub<ClientHandlerHub>("/listen/clients", options => options.WebSockets.CloseTimeout = TimeSpan.FromDays(1));
                _ = endpoints.MapHub<AdminSystemHub>("/admin", options => options.WebSockets.CloseTimeout = TimeSpan.FromDays(1));
                _ = endpoints.MapHub<UserToAdminChatHub>("/chat");
                _ = endpoints.MapHub<DiagnoseHub>("/sandbox");

                _ = endpoints.MapControllers();
            });
            // app.UseMvc();
        }
    }
}