// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMWE_RealTime_Server.Controllers
{
    [Route("")]
    [ApiController]
    public class AmweController : ControllerBase
    {
        [HttpGet]
        public ActionResult RedirectToAmweMainSite()
        {
            return Redirect("http://amwe.glitch.me/");
        }

        [HttpGet("time")]
        public DateTime GetTime()
        {
            return DateTime.UtcNow;
        }
    }
}