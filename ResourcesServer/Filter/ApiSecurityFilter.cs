using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesServer.Filter
{
    public class ApiSecurityFilter : ActionFilterAttribute
    {
        /// <summary>
        /// OnActionExecuting 在执行操作方法之前由 core 框架调用
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //https://blog.csdn.net/confused_kitten/article/details/80915104

            string rawRequest = string.Empty;
            object responseBody = null;
            HttpRequest request = context.HttpContext.Request;
            using (var stream = new StreamReader(request.Body))
            {
                //Stream stream = request.Body;
                char[] buffer = new char[request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                rawRequest = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(buffer));

                //没有body以及没有登录
                if (string.IsNullOrWhiteSpace(rawRequest)
                    //|| request.HttpContext.User.Identity.IsAuthenticated
                    )
                {
                    responseBody = new
                    {
                        code = 100,
                        errmsg = "参数不合法"
                    };
                }
                else
                {
                    var path = request.Path.Value;
                    var host = request.Host;
                    var port = host.Port;

                    path = path.Substring(path.LastIndexOf("/") + 1);

                    var controllerName = context.RouteData.Values["controller"].ToString();
                    var actionName = context.RouteData.Values["action"].ToString();
                    JObject rss = JObject.Parse(rawRequest);
                    var appid = (string)rss.GetValue("token");
                    var uuid = (string)rss.GetValue("action");
                    //判断请求的Action，body的scope和token中的scope是否一致

                    //var username = request.HttpContext.User.Claims.First(x => x.Type == "userInfo").Value;

                    //request.EnableRewind();
                    //request.Body.Position = 0;
                }
            }

            //context.HttpContext.Request.Body.
        }
    }
}
