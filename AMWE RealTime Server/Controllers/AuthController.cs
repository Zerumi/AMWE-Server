using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMWE_RealTime_Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private void UserAuth(Client client)
        {
            GlobalUsersList.Add(client);
        }

        public static uint GlobalClientId = 0;
        public static List<Client> GlobalUsersList = new List<Client>();

        [HttpPost]
        public async Task<bool> Auth((string, string) authdata)
        {
            if (authdata.Item2 == "user")
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, authdata.Item1),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.GlobalUserRole)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                UserAuth(new Client
                {
                    Id = GlobalClientId++,
                    Nameofpc = authdata.Item1
                });
                return true;
            }
            else if (Encryption.Decrypt(authdata.Item2) == new StreamReader(System.IO.File.OpenRead(@"password.txt")).ReadToEnd())
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, authdata.Item1),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin")
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                return true;
            }
            return false;
        }

        [HttpDelete]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}