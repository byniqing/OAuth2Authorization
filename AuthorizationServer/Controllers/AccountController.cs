using AuthorizationServer.Models;
using AuthorizationServer.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    //[Route("identity")]
    public class AccountController : Controller
    {
        private readonly TestUserStore _userStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        public AccountController(
            //TestUserStore userStore,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            //_userStore = userStore;
            _interaction = interaction;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("index");
        }
        //[Route("authorize")]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            //ViewBag.returnUrl11 = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (login.Action.ToLower() == "cancel")
            {
                var request = await _interaction.GetAuthorizationContextAsync(login.ReturnUrl);
                if (request != null)
                {
                    ConsentResponse grantedConsent = ConsentResponse.Denied;
                    await _interaction.GrantConsentAsync(request, grantedConsent);
                    return Redirect(login.ReturnUrl);
                }
                else
                {
                    //异常处理
                }
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, login.PassWrod))
                {
                    #region 刚开始这样登录是不行的,这是cookie身份验证，cookie身份登录
                    //var claims = new List<Claim> {
                    //    new Claim("name",login.UserName)
                    // };
                    //var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

                    //这种方式是cookie认证，这样登录无效

                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    //    claimsPrincipal,
                    //    new AuthenticationProperties
                    //    {
                    //        IsPersistent = true, //
                    //        ExpiresUtc = DateTime.Now.AddDays(5)
                    //    });



                    #endregion
                    #region 方法1 OAuth身份验证，identityserver身份验证
                    //idsrv验证sub是必须的
                    //var claims1 = new List<Claim> {
                    //    new Claim(JwtClaimTypes.Subject,user.SubjectId),
                    //    new Claim(JwtClaimTypes.Name,user.Username)
                    // };
                    //var claimIdentity1 = new ClaimsIdentity(claims1, OAuthDefaults.DisplayName);
                    //var claimsPrincipal1 = new ClaimsPrincipal(claimIdentity1);

                    //要用 idsrv
                    //await HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme,
                    //    claimsPrincipal1,
                    //    new AuthenticationProperties
                    //    {
                    //        IsPersistent = true, //
                    //        ExpiresUtc = DateTime.Now.AddDays(5)
                    //    });
                    #endregion
                    #region 方法2 或者直接用identityserver4封装的扩展方法 这样登录才正确
                    var p = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.Now.AddDays(5)
                    };

                    //identityserver 是身份验证 ，identity 所以要用该方法
                    /*
                     * 
                     */
                    //Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(HttpContext, user.SubjectId, user.Username, p);

                    //或者 TestUserStore 登陆
                    //await HttpContext.SignInAsync(user.SubjectId, user.Username, p);

                    //换成identityServer登陆 app.UseIdentityServer();
                    await _signinManager.SignInAsync(user, p);

                    /*
                     这里要注意一点
                     因为这个授权服务器自己可以登陆。也可以是从客户端跳转过来授权登陆的
                     如果是自己的登陆的，登陆成功肯定是返回到自己网站的首页
                     如果是从客户端跳转过来的登陆成功有可能是返回到客户端（如果不需要到授权页面）或者是跳转到同意授权页面
                     */

                    //判断是否是returnUrl页面，是自己网站带的returnUrl也不会通过
                    //指示在登录或同意后，返回URL是否是用于重定向的有效URL。
                    if (_interaction.IsValidReturnUrl(login.ReturnUrl))
                    {
                        //登录成功，跳转到同意授权页面
                        return Redirect(login.ReturnUrl);
                    }
                    //如果想不管是重定向，还是自己的都需要跳转，就直接这样
                    //return Redirect(login.ReturnUrl);

                    return Redirect("~/");
                    #endregion
                }
            }
            //else
            return View("login");
        }

        [HttpPost]
        public IActionResult Show()
        {
            return View();
        }

        public async Task<IActionResult> LognOut()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }
        public IActionResult Registe()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registe(LoginModel model)
        {
            var identityUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var identityResult = await _userManager.CreateAsync(identityUser, model.PassWrod);
            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}