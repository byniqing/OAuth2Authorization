using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttributeRouting;
using ResourcesServer.Filter;

namespace ResourcesServer.Controllers
{
    [Route("api"), ApiSecurityFilter]
    //[RoutePrefix("info"),ApiSecurityFilter]
    //[ApiController]
    public class ShowController : ControllerBase
    {
        [HttpPost]
        [Route("userInfo")]
        public ActionResult GetUserInfo()
        {
            //return new JsonResult(User.Claims.Select(
            //    c => new { c.Type, c.Value }));

            return new JsonResult(new
            {
                name = "cnblgos",
                address = "hk"
            });
        }
    }
}