using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AuthorizationCode.Controllers
{
    //[Authorize(AuthenticationSchemes = "myCookies")]
    [Authorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var ck = Request.Query["code"];

                var result =  HttpContext.AuthenticateAsync().Result;
                var ck1 = result.Properties.GetTokens();

                //ck1.ToList().ForEach(c => { if (c.Name == "") {  c.Value; } });

                //access_token
                //access_token
                //var abc = (from c in ck1
                //           where c.Name == "access_token999"
                //           select c.Value) ? 0

                //.Select(c => c.Value == "access_token") ;
            }
            //var client = new WebServerClient(authorizationServer, "123456", "abcdef");

            return View();
        }
    }
}