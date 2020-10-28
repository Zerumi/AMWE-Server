// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
//using AMWE_RealTime_Server.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AMWE_RealTime_Server.Hubs
//{
//    [Authorize]
//    public class ServerHub : Hub
//    {
//        VersionsContext context;

//        public ServerHub(VersionsContext _context)
//        {
//            context = _context;
//        }

//        [Authorize(Roles = Role.GlobalDeveloperRole)]
//        public async Task<dynamic> NewLatestVersion(string type, string vers, List<VersionFile> version, bool overwriteversion = false)
//        {
//            try
//            {
//                if (context.Versions.FirstOrDefault(x => x.version == vers) != default && context.Versions.FirstOrDefault(x => x.version == vers).Type == type)
//                {
//                    if (!overwriteversion)
//                    {
//                        return 0;
//                    }
//                    context.Versions.Include(x => x.versionfiles).FirstOrDefault(x => x.version == vers).versionfiles.Clear();
//                    context.Versions.Remove(context.Versions.FirstOrDefault(x => x.version == vers));
//                }
//                context.Versions.Add(new Models.Version() { 
//                    Type = type,
//                    version = vers,
//                    versionfiles = version
//                });
//                return await context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                return ex;
//            }
//        }

//        public bool CheckConflict(string type, string version)
//        {
//            return Array.FindAll(context.Versions.ToArray(), x => x.Type == type).Select(x => x.version).Contains(version);
//        }
//    }
//}