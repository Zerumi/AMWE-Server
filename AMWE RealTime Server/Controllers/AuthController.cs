// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AMWE_RealTime_Server.Controllers
{
    [Route("/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IHubContext<ClientHandlerHub> _hubContext;

        public AuthController(IHubContext<ClientHandlerHub> hubContext)
        {
            StaticVariables.svControllers.Add(this);
            _hubContext = hubContext;
        }

        public static uint GlobalClientId = 0;

        public static List<Client> GlobalClientsList = new List<Client>();
        public static List<ClientState> GlobalClientStatesList = new List<ClientState>();
        public static Dictionary<uint, ClaimsPrincipal> GlobalUsersList = new Dictionary<uint, ClaimsPrincipal>();

        [HttpPost]
        [AllowAnonymous]
        public async Task<dynamic> Auth([FromBody] string[] authdata)
        {
            if (authdata[1] == "user")
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, $"ID {GlobalClientId}/" + authdata[0]),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.GlobalUserRole)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                ClaimsPrincipal user = new ClaimsPrincipal(id);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
                Client client = new Client()
                {
                    Id = GlobalClientId,
                    Nameofpc = authdata[0]
                };
                ClientState clientState = new ClientState()
                {
                    Client = client,
                    LastLoginDateTime = DateTime.Now
                };
                GlobalUsersList.Add(client.Id, user);
                GlobalClientsList.Add(client);
                GlobalClientStatesList.Add(clientState);
                await _hubContext.Clients.All.SendAsync("OnUserAuth", clientState);
                GlobalClientId++;
                return client;
            }
            else if (Encryption.Decrypt(authdata[1]) == new StreamReader(System.IO.File.OpenRead(@"password.txt")).ReadToEnd())
            {
                //VerifyVersion version = Array.Find(adminversions, x => x.version == authdata[2]);
                //if (version.isNotSupported || !version.rephandler.Contains(authdata[3]) || !version.m3md2.Contains(authdata[4]) || !version.m3md2_startup.Contains(authdata[5]))
                //{
                //    return Redirect("~/api/update/latest/admin");
                //}
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, authdata[0]),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.GlobalAdminRole)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                return true;
            }
            else if (Encryption.Decrypt(authdata[1]) == new StreamReader(System.IO.File.OpenRead(@"devpassword.txt")).ReadToEnd())
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, authdata[0]),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.GlobalDeveloperRole)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                return "Developer";
            }
            return false;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Logout(uint id)
        {
            ClientState a = GlobalClientStatesList.Find(x => x.Client.Id == id);
            if (a != null)
            {
                a.LastLogoutDateTime = DateTime.Now;
                await _hubContext.Clients.All.SendAsync("OnUserLeft", a);
                GlobalClientsList.Remove(GlobalClientsList.Find(x => x.Id == a.Client.Id));
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return NoContent();
        }

        public static VerifyVersion[] adminversions = new VerifyVersion[]
            {
                new VerifyVersion()
                {
                    Version = "0.6.0.0",
                    IsNotSupported = false,
                    IsLatest = true,
                    IsUpdateNeeded = false,
                    Rephandler = new List<string> {"1.0.0.0"},
                    M3md2 = new List<string> {"1.4.1.0"},
                    M3md2_startup = new List<string> {"1.3.1.0"}
                },
                new VerifyVersion()
                {
                    Version = "1.0.0.0",
                    IsNotSupported = true,
                    IsLatest = false,
                    IsUpdateNeeded = false,
                    Rephandler = new List<string> {"1.0.0.0"},
                    M3md2 = new List<string> {"1.4.1.0"},
                    M3md2_startup = new List<string> {"1.3.1.0"}
                }
            };


        public static VerifyVersion[] userversions = new VerifyVersion[]
            {
                new VerifyVersion()
                {
                    Version = "1.0.0.0",
                    IsNotSupported = false,
                    IsLatest = false,
                    IsUpdateNeeded = false,
                    Rephandler = new List<string> {"2.1.0.0"},
                    M3md2 = new List<string> {"1.4.1.0"},
                    M3md2_startup = new List<string> {"1.3.1.0"}
                }
            };
    }
}