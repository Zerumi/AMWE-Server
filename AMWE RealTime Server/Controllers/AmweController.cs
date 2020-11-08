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
    }
}