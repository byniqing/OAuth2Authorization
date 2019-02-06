using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ResourcesServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InfoController : ControllerBase
    {
        public string GetName()
        {
            return "nsky";
        }
        [HttpGet]
        public string Show11()
        {

            return "sss";
        }
        [HttpGet]
        public ActionResult GetEmail()
        {
            var username = User.Claims.First(x => x.Type == "email").Value;
            //return Ok(username);

            return new OkObjectResult(new
            {
                userName = username,
                code = 200
            });
        }

        /// <summary>
        /// 假如获取其他信息，必须要授权
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetOtherInfo()
        {
            //判断是否授权
            var scope = User.Claims.FirstOrDefault(f => f.Type == "scope" && f.Value == "OtherInfo");
            if (scope != null)
            {
                return new JsonResult(
                    new
                    {
                        phone = "110",
                        address = "china",
                        age = "20",
                        gender = "m",
                        hobby = "coding"
                    }
                    );
            }
            else //禁止访问
            {
                return BadRequest(StatusCodes.Status403Forbidden);
            }
        }

        [HttpGet]
        public ActionResult GetClaims()
        {
            return new JsonResult(User.Claims.Select(
                c => new { c.Type, c.Value }));
        }
    }
}