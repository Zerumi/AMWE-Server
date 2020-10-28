﻿// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMWE_RealTime_Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
            OnClientLogin += UserAuth;
        }

        private void UserAuth(Client client)
        {
            GlobalUsersList.Add(client);
        }

        public static uint GlobalClientId = 0;
        public static List<Client> GlobalUsersList = new List<Client>();

        [HttpPost]
        [AllowAnonymous]
        public async Task<dynamic> Auth([FromBody] string[] authdata)
        {
            if (authdata[1] == "user")
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, authdata[0]),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.GlobalUserRole)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                var client = new Client
                {
                    Id = GlobalClientId++,
                    Nameofpc = authdata[0]
                };
                OnClientLogin?.Invoke(client);
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

        [HttpDelete]
        public async Task Logout(Client client)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            OnClientLogout?.Invoke(client);
        }

        public delegate void ClientHandler(Client client);

        public static event ClientHandler OnClientLogin;
        public static event ClientHandler OnClientLogout;

        public static VerifyVersion[] adminversions = new VerifyVersion[]
            {
                new VerifyVersion()
                {
                    version = "0.6.0.0",
                    isNotSupported = false,
                    isLatest = true,
                    isUpdateNeeded = false,
                    rephandler = new List<string> {"1.0.0.0"},
                    m3md2 = new List<string> {"1.4.1.0"},
                    m3md2_startup = new List<string> {"1.3.1.0"}
                },
                new VerifyVersion()
                {
                    version = "1.0.0.0",
                    isNotSupported = true,
                    isLatest = false,
                    isUpdateNeeded = false,
                    rephandler = new List<string> {"1.0.0.0"},
                    m3md2 = new List<string> {"1.4.1.0"},
                    m3md2_startup = new List<string> {"1.3.1.0"}
                }
            };


        public static VerifyVersion[] userversions = new VerifyVersion[]
            {
                new VerifyVersion()
                {
                    version = "1.0.0.0",
                    isNotSupported = false,
                    isLatest = false,
                    isUpdateNeeded = false,
                    rephandler = new List<string> {"2.1.0.0"},
                    m3md2 = new List<string> {"1.4.1.0"},
                    m3md2_startup = new List<string> {"1.3.1.0"}
                }
            };
    }
}