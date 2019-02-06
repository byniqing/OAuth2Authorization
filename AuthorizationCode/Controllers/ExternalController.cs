using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthorizationCode.Controllers
{
    public class ExternalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
            };

            return Challenge(props, provider);
        }
        /// <summary>
        /// 授权服务器授权成功后回调方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("oidc");
            var result1 = await HttpContext.AuthenticateAsync();
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            //获取授权来的信息，直接登陆
            return Redirect(returnUrl);
        }
    }
}