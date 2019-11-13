using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication18.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThingieController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))
            {
                hostName += domainName;
            }

            Version version = typeof(ThingieController).Assembly.GetName().Version;

            return Ok(new
            {
                UtcTime = DateTime.UtcNow,
                Machine = hostName,
                Version = version
            });
        }

    }
}