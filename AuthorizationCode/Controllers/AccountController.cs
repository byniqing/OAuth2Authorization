using AuthorizationCode.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;


namespace AuthorizationCode.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAuthenticationSchemeProvider schemeProvider,
            UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            _schemeProvider = schemeProvider;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 从这里触发授权的，没有设置RedirectUri，所以当授权成功也会回调该
        /// 
        /// OAuth2 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Authorize()
        {
            var ck1 = HttpContext.Request.Query;
            string scheme = "OAuth";

            // see if windows auth has already been requested and succeeded
            //WindowsPrincipal 
            var userResult = await HttpContext.AuthenticateAsync(scheme);
            // DefaultAuthenticateScheme causes User to be set
            // var user = context.User;

            // This is what [Authorize] calls
            var user = userResult.Principal;
            var props1 = userResult.Properties;

            //已经授权，登录并跳转
            if (User.Identity.IsAuthenticated)
            {
                var token1 = userResult.Properties.GetTokens();
                //或者
                var token = userResult.Properties.GetTokenValue("access_token");

                //根据token获取用户信息
                var httpClient = new HttpClient();
                httpClient.SetBearerToken(token);
                httpClient.BaseAddress = new System.Uri("http://localhost:5002/");
                var userinfo = httpClient.GetStringAsync("identity/userinfo").Result;
                var obj = JObject.Parse(userinfo);
                var username = obj.GetValue("name");
                var address = obj.GetValue("address");
                //可以保存数据库创建自己的账号
                ViewBag.userName = username;
                ViewBag.address = address;
                return View("UserInfo");
            }


            //string scheme = "OAuth";
            ////如果已经认证通过，
            //if (User.Identity.IsAuthenticated)
            //{
            //    return Redirect("");
            //}
            ////触发授权，DefaultChallengeScheme配置的是OAuth
            return Challenge(scheme);
            //return await ProcessWindowsLoginAsync("/Account/Authorize");

            //return View();
        }

        /// <summary>
        /// 回调方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("oidc");
            var result1 = await HttpContext.AuthenticateAsync();
            if (result?.Succeeded != true)
            {
                //throw new Exception("External authentication error");
            }
            var id_token = result.Properties.GetTokenValue("id_token");
            var access_token = result.Properties.GetTokenValue("access_token");

            //  var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            return RedirectToAction("UserInfo", "Account");
        }
        /// <summary>
        /// oidc测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OidcAuthorize()
        {
            string scheme = "oidc"; //ConfigureServices中注册的服务是oidc
            string schemeTe = "oidc";
            schemeTe = OpenIdConnectDefaults.DisplayName;

            //var userResult = await HttpContext.AuthenticateAsync(scheme);
            //var user = userResult.Principal;
            //var props1 = userResult.Properties;
            //userResult.Properties.GetTokenValue("access_token");

            if (User.Identity.IsAuthenticated)
            {
                return View("UserInfo");
            }
            //var userResult = await HttpContext.AuthenticateAsync(scheme);

            //var user = userResult.Principal;
            //var props1 = userResult.Properties;


            var props = new AuthenticationProperties
            {
                //指定授权成功后回调的Action
                RedirectUri = Url.Action(nameof(Callback)),

                /*
                 可以传些额外信息，回调会传回来
                 可以通过
                  var result = await HttpContext.AuthenticateAsync("oidc");
                  result.Properties.Items["returnUrl"]
                 */
                //Items =
                //    {
                //        { "returnUrl", returnUrl },
                //        { "scheme", provider },
                //    }
            };

            return Challenge(props, scheme);


            /*
             如果这样，授权服务器回调就是当前Action
             */
            //return Challenge(scheme);


            //await HttpContext.ChallengeAsync(scheme);
            //return await ProcessWindowsLoginAsync("/Account/Authorize");

        }
        //[HttpPost]
        //public IActionResult Authorize()
        //{
        //    string scheme = "OAuth";
        //    ////如果已经认证通过，
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return Redirect("UserInfo");
        //    }
        //    //触发授权，DefaultChallengeScheme配置的是OAuth
        //    return Challenge(scheme);
        //    //return await ProcessWindowsLoginAsync("/Account/Authorize");

        //    //return View();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            //string scheme = "OAuth";
            string scheme = "oidc";
            scheme = OpenIdConnectDefaults.DisplayName;
            // see if windows auth has already been requested and succeeded
            //WindowsPrincipal 
            var userResult = await HttpContext.AuthenticateAsync(scheme);

            // DefaultAuthenticateScheme causes User to be set
            // var user = context.User;

            // This is what [Authorize] calls
            var user = userResult.Principal;
            var props1 = userResult.Properties;

            //已经授权，登录并跳转
            if (User.Identity.IsAuthenticated)
            {
                var ck = userResult.Properties.GetTokens();
                var token = userResult.Properties.GetTokenValue("access_token");

                return View("UserInfo");

            }


            //if (userResult?.Principal is ClaimsPrincipal wp)
            //{
            //    // we will issue the external cookie and then redirect the
            //    // user back to the external callback, in essence, treating windows
            //    // auth the same as any other external authentication mechanism
            //    var props = new AuthenticationProperties()
            //    {
            //        RedirectUri = Url.Action("Callback"),
            //        Items =
            //        {
            //            { "returnUrl", returnUrl },
            //            { "scheme", scheme },
            //        }
            //    };

            //    var id = new ClaimsIdentity(scheme);

            //    var result11 = HttpContext.AuthenticateAsync("idsrv").Result;

            //    id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
            //    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

            //    await HttpContext.SignInAsync("idsrv",
            //        new ClaimsPrincipal(id),
            //        props);
            //    //HttpContext.SignInAsync(wp.Identity.Name, wp.Identity.Name, props);
            //    return Redirect(props.RedirectUri);
            //}
            else
            {
                //触发授权，DefaultChallengeScheme配置的是OAuth
                return Challenge(scheme);
                //await HttpContext.ChallengeAsync("OAuth");
            }
        }

        [HttpGet]
        public IActionResult UserInfo()
        {
            var a = HttpContext.GetTokenAsync("id_token");
            var a1 = HttpContext.GetTokenAsync("access_token");
            var a2 = HttpContext.GetTokenAsync("refresh_token");

            return View();
        }

        [HttpGet]
        public IActionResult Lognout()
        {
            HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("index", "home");
        }

        /// <summary>
        /// 自己网站登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            ViewBag.error = string.Empty;

            if (model.Button == "cancel")
                //判断是登陆还是取消
                return Redirect("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.Now.AddDays(5)
                    };

                    var claims = new List<Claim> {
                        new Claim("name",model.Username)
                            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var cliamsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    //app.UseAuthentication();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cliamsPrincipal, properties);

                    // await _signInManager.SignInAsync(user, properties);
                    return Redirect(model.ReturnUrl);
                }
                //用户名或者密码错误逻辑
            }
            return Redirect("~/");
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            //获取所有注册的scheme
            var schemes = await _schemeProvider.GetAllSchemesAsync();
            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var lv = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                Username = "",
                ExternalProviders = providers.ToArray()
            };
            return View(lv);
        }
    }
}