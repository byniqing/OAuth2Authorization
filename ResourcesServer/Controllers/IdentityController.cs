using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ResourcesServer.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// 获取当前的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get()
        {
            return new JsonResult(User.Claims.Select(
                c => new { c.Type, c.Value }));
        }

        [HttpGet]
        [Route("userInfo")]
        public ActionResult GetUserInfo()
        {
            //return new JsonResult(User.Claims.Select(
            //    c => new { c.Type, c.Value }));

            return new JsonResult(new {
                name="cnblgos",
                address="hk"
            });
        }
      
    }
}