﻿// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AMWE_RealTime_Server.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Net;

//namespace AMWE_RealTime_Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UpdateController : ControllerBase
//    {
//        VersionsContext _context;

//        public UpdateController(VersionsContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("latest/{type}")]
//        public ICollection<VersionFile> GetLatestVersion(string type)
//        {
//            return _context.Versions.Include(x => x.versionfiles).ToList().Find(y => y.Type == type && y.version == AuthController.adminversions.First(d => d.isLatest == true).version).versionfiles;
//        }
//    }
//}