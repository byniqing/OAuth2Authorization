using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using htp=Microsoft.AspNetCore.Http;


namespace AuthorizationServer.Controllers
{
    public class LoginController : Controller
    {
        private readonly TestUserStore _user;
        public LoginController(TestUserStore user)
        {
            _user = user;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string Login()
        {
           var u =  _user.FindByUsername("niq");
            if (u == null)
            {
                return "用户名为空";
            }
            if(_user.ValidateCredentials("niq", "pwd"))
            {
                //Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(HttpContext,"",)

                var pr = new AuthenticationProperties {
                    IsPersistent = true,
                    ExpiresUtc=DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                };
                Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(HttpContext,
                    "900", "niq", pr);
                return "succes11";

            }
            return "succes";
        }
    }
}