using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class ServerHub : Hub
    {
        VersionsContext context;

        public ServerHub(VersionsContext _context)
        {
            context = _context;
        }

        [Authorize(Roles = Role.GlobalDeveloperRole)]
        public async Task<dynamic> NewLatestVersion(string type, string vers, List<VersionFile> version)
        {
            try
            {
                if (type == "admin")
                {
                    if (context.Versions.FirstOrDefault(x => x.Type == "admin") != default)
                    {
                        context.Versions.Include(x => x.versionfiles).FirstOrDefault(x => x.Type == "admin").versionfiles.Clear();
                        context.Versions.Remove(context.Versions.FirstOrDefault(x => x.Type == "admin"));
                    }
                }
                else if (type == "user")
                {

                }
                else
                {
                    return new ArgumentException("Wrong type");
                }
                context.Versions.Add(new Models.Version() { 
                    Type = type,
                    version = vers,
                    versionfiles = version
                });
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}