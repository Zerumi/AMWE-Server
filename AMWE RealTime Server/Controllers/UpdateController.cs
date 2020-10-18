//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AMWE_RealTime_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        VersionsContext _context;

        public UpdateController(VersionsContext context)
        {
            _context = context;
        }

        [HttpGet("latest/{type}")]
        public ICollection<VersionFile> GetLatestVersion(string type)
        {
            return _context.Versions.Include(x => x.versionfiles).FirstOrDefault(x => x.Type == type).versionfiles;
        }
    }
}