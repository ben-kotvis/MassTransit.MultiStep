using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.MultiStep.Portal.Controllers
{
    [Produces("application/json")]
    [Route("api/Values")]
    public class ValuesController : Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Hello = "World" });
        }
    }
}