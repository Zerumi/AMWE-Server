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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AMWE_RealTime_Server.Controllers
{
    [Route("/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IHubContext<ClientHandlerHub> _hubContext;
        private readonly ApplicationContext _context;

        public AuthController(IHubContext<ClientHandlerHub> hubContext, ApplicationContext context)
        {
            StaticVariables.SvControllers.Add(this);
            _hubContext = hubContext;
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<dynamic> Auth([FromBody] string[] authdata)
        {
            if (authdata[1] == "user")
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, $"ID {(_context.GlobalClientsList.Count() > 0 ? _context.GlobalClientsList.Max(c => c.Id) + 1 : 1)}/" + authdata[0]),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.GlobalUserRole)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                ClaimsPrincipal user = new ClaimsPrincipal(id);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
                Client client = new Client()
                {
                    Id = _context.GlobalClientsList.Count() > 0 ? _context.GlobalClientsList.Max(c => c.Id) + 1 : 1,
                    Nameofpc = authdata[0]
                };
                ClientState clientState = new ClientState()
                {
                    Client = client,
                    IsOnline = true,
                    LastLoginDateTime = DateTime.Now
                };
                await _hubContext.Clients.All.SendAsync("OnUserAuth", clientState);
                _ = _context.GlobalClientsList.Add(client);
                _ = _context.GlobalClientStatesList.Add(clientState);
                _ = await _context.SaveChangesAsync();
                return client;
            }
            else
            {
                User user = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Username == authdata[0]);

                if (user == null)
                {
                    return false;
                }

                if (user.PasswordType == PasswordType.LegacyAESEncrypted && Encryption.Decrypt(user.Password) == Encryption.Decrypt(authdata[1]))
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, authdata[0]),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
                    };
                    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                    return user.Role.Name == Role.GlobalDeveloperRole ? "Developer" : (dynamic)true;
                }
            }
            return false;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Logout(uint id)
        {
            ClientState a = _context.GlobalClientStatesList
            .Include(a => a.Client)
            .FirstOrDefault(x => x.Client.Id == id);

            if (a != null)
            {
                a.IsOnline = false;
                a.LastLogoutDateTime = DateTime.Now;
                await _hubContext.Clients.All.SendAsync("OnUserLeft", a);
                _ = await _context.SaveChangesAsync();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return NoContent();
        }
    }
}