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


                    //var app = Dal.ass(appid);
                    //var app = 0;
                    //var appKey = app == null ? "" : 0;// app.AppKey;
                    var appKey = "0";
                    var timestamp = (string)rss.GetValue("timestamp");
                    var signature = (string)rss.GetValue("signature");

                    var array = new[] { appid, uuid, timestamp, appKey };
                    Array.Sort(array);
                    var newSignature = GetMd5(string.Join("", array));


                    if (string.IsNullOrEmpty(appKey) || newSignature != signature)
                    {
                        responseBody = new
                        {
                            code = 400,
                            errmsg = "没有访问权限"
                        };
                    }

                    //判断请求的Action，body的scope和token中的scope是否一致

                    //var username = request.HttpContext.User.Claims.First(x => x.Type == "userInfo").Value;

                    //request.EnableRewind();
                    //request.Body.Position = 0;
                }
            }

            //context.HttpContext.Request.Body.
        }
        /// <summary>
        /// 进行MD5效验
        /// </summary>
        /// <param name="strmd5"></param>
        /// <returns></returns>
        public static string GetMd5(string strmd5)
        {
            byte[] md5Bytes = ASCIIEncoding.Default.GetBytes(strmd5);
            byte[] encodedBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            //FileStream fs= new FileStream(filepath,FileMode.Open,FileAccess.Read);
            encodedBytes = md5.ComputeHash(md5Bytes);
            string nn = BitConverter.ToString(encodedBytes);
            nn = Regex.Replace(nn, "-", "");//因为转化完的都是34-2d这样的，所以替换掉- 
            nn = nn.ToLower();//根据需要转化成小写
            //fs.Close();
            return nn;
        }
    }
}
