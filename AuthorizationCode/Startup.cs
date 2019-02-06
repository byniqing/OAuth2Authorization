using AuthorizationCode.Date;
using AuthorizationCode.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthorizationCode
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //配置DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //配置文件前面必须是；ConnectionStrings
                //因为默认是：GetSection("ConnectionStrings")[name].
                options.UseSqlServer(Configuration.GetConnectionString("conn"));
                //options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

           
            //依赖注入初始化
            services.AddAuthentication(options =>
            {
                /*
                 要想使用认证系统，必要先注册Scheme
                 而每一个Scheme必须指定一个Handler
                 AuthenticationHandler 负责对用户凭证的验证
                 这里指定的默认认证是cookie认证
                 Scheme可以翻译为方案，即默认的认证方案

                因为这里用到了多个中间件，（AddAuthentication，AddCookie，AddOpenIdConnect）
                OpenIdConnectDefaults.DisplayName 的默认值是oidc
                指定AddOpenIdConnect是默认中间件，在AddOpenIdConnect配置了很多选项

                如果只用了一个中间件，则可以不写，是否还记得cookie认证
                //     services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //     .AddCookie(option =>
                //     {
                //         ///Account/Login?ReturnUrl=%2Fadmin
                //         option.LoginPath = "/login/index";
                //         //option.ReturnUrlParameter = "params"; //指定参数名称
                //         //option.Cookie.Domain
                //         option.AccessDeniedPath = "/login/noAccess";
                //         option.Cookie.Expiration = TimeSpan.FromSeconds(4);
                //         option.Events = new CookieAuthenticationEvents
                //         {
                //             OnValidatePrincipal = LastChangedValidator.ValidateAsync
                //         };
                //     });

                 */

                //options.DefaultScheme = "Cookies";

                //默认的认证方案：cookie认证，信息是保存在cookie中的
                options.DefaultAuthenticateScheme = "Cookies";
                //oidc 就是openidConnect


                //名字随便取，只要AddOpenIdConnect中的的oidc名字一样即可，
                //这样才能找到，默认使用oidc
                //options.DefaultChallengeScheme = "oidc";

                /*
                但我想，我的的网站没有登陆，跳转到自己的登陆页面，
                但用户可以指定第三方登陆，所以，这样不使用oidc，用的时候指定
                使用myCookies中间件
                 */
                options.DefaultChallengeScheme = "Cookies";



                //options.DefaultChallengeScheme = "oidc";
                //默认使用oidc中间件
                //options.DefaultChallengeScheme = OpenIdConnectDefaults.DisplayName;


            })
            .AddCookie("Cookies", options =>
            {
                options.LoginPath = "/Account/Login";
            })
            .AddOAuth("OAuth", options =>
            {
                options.SignInScheme = "Cookies";
                options.ClientId = "OAuth.Client";
                options.ClientSecret = "secret";
                options.AuthorizationEndpoint = "http://localhost:5003/connect/authorize";
                options.TokenEndpoint = "http://localhost:5003/connect/token";
                options.CallbackPath = new PathString("/OAuth");
                options.SaveTokens = true;
                options.Scope.Add("OAuth1");
                options.Scope.Add("OAuth2");
                options.Scope.Add("OAuth3");
                //options.Scope.Add("offline_access");
                options.Events = new OAuthEvents()
                {
                    //远程异常触发
                    OnRemoteFailure = OAuthFailureHandler =>
                    {
                        //var msg = OAuthFailureHandler.Failure.Message;
                        var authProperties = options.StateDataFormat.Unprotect(OAuthFailureHandler.Request.Query["state"]);
                        var redirectUrl = authProperties.RedirectUri;
                        if (redirectUrl.Contains("/"))
                        {
                            redirectUrl = string.Format($"{redirectUrl.Substring(0, redirectUrl.LastIndexOf("/") + 1)}#");// redirectUrl.Substring(0, redirectUrl.IndexOf("/") + 1);
                        }
                        //"http://localhost:5001/#"
                        OAuthFailureHandler.Response.Redirect(redirectUrl);
                        OAuthFailureHandler.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            })
            //.AddGoogle("googole", options => {
            //    options.ClientId = "";
            //    options.ClientSecret = "";

            //})
            .AddOpenIdConnect("oidc", "OpenID Connect", options =>
            {
                //options.SignOutScheme = OpenIdConnectDefaults.DisplayName;
                options.SignInScheme = "Cookies";

                //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                //options.SignOutScheme = IdentityServerConstants.SignoutScheme;


                //默认值start
                //options.CallbackPath = new PathString("/signin-oidc");
                //options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
                //options.RemoteSignOutPath = new PathString("/signout-oidc");
                //options.Scope.Add("openid");
                //options.Scope.Add("profile");
                //options.ResponseMode = OpenIdConnectResponseMode.FormPost;
                //默认值end


                options.Authority = "http://localhost:5003";
                options.RequireHttpsMetadata = false;
                options.ClientId = "oidc";
                options.ClientSecret = "secret";
                options.SaveTokens = true;

                /*
                 这样会去请求UserInfoEndpoint获取到信息后绑定到User.Claims
                 同时access_token也会有
                 */
                //options.GetClaimsFromUserInfoEndpoint = true;
                //options.ClaimActions.MapJsonKey("sub", "sub");
                //options.ClaimActions.MapJsonKey("preferred_username", "preferred_username");
                //options.ClaimActions.MapJsonKey("avatar", "avatar");
                //options.ClaimActions.MapCustomJson("role", job => job["role"].ToString());
                ////////////////////////////////////
                
                //options.Scope.Add("OAuth1");
                options.Scope.Add("offline_access");
                options.Scope.Add("OtherInfo");
                options.Scope.Add("email");
                /*
                 默认值是:id_token
                 */
                options.ResponseType = "id_token code";// OpenIdConnectResponseType.CodeIdToken;
                options.Events = new OpenIdConnectEvents
                {
                    //OnMessageReceived = received => {
                    //    received.HttpContext.ChallengeAsync("oidc");
                    //    received.HandleResponse();
                    //    return Task.FromResult(0);
                    //},
                    //OnRedirectToIdentityProviderForSignOut = re => {
                    //    var ck =await re.HttpContext.AuthenticateAsync();


                    //},
                    /*
                     远程异常触发
                     在授权服务器取消登陆或者取消授权      
                     */
                    OnRemoteFailure = OAuthFailureHandler =>
                    {
                        //跳转首页
                        OAuthFailureHandler.Response.Redirect("/");
                        OAuthFailureHandler.HandleResponse();
                        return Task.FromResult(0);
                    },
                    //未授权时，重定向到OIDC服务器时触发
                    //OnRedirectToIdentityProvider = identity => {
                    //    //这里不跳转到授权服务器，跳转到登陆页面
                    //    identity.Response.Redirect("/Account");
                    //    identity.HandleResponse();
                    //    return Task.FromResult(0);
                    //}
                };
            });

            #region OAuth认证
            //services.AddAuthentication(options =>
            //{
            //    //options.DefaultAuthenticateScheme=OAuthDefaults.DisplayName
            //    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultAuthenticateScheme = "Cookies";
            //    options.DefaultSignInScheme = "Cookies";
            //    //options.DefaultSignOutScheme = "Cookies";
            //    options.DefaultChallengeScheme = "OAuth";
            //})
            //.AddCookie()
            //.AddOAuth("OAuth", options =>
            //{
            //    options.ClientId = "OAuth.Client";
            //    options.ClientSecret = "secret";
            //    options.AuthorizationEndpoint = "http://localhost:5003/connect/authorize";
            //    options.TokenEndpoint = "http://localhost:5003/connect/token";
            //    options.CallbackPath = new PathString("/OAuth");
            //    options.SaveTokens = true;
            //    options.Scope.Add("OAuth1");
            //    options.Scope.Add("OAuth2");
            //    options.Scope.Add("OAuth3");
            //    //options.Scope.Add("offline_access");
            //    options.Events = new OAuthEvents()
            //    {
            //        //OnRedirectToAuthorizationEndpoint = t =>
            //        //{
            //        //    t.Response.Redirect("http://localhost:5001/Account/userinfo");
            //        //    return Task.FromResult(0);
            //        //},

            //        //远程异常触发
            //        OnRemoteFailure = OAuthFailureHandler =>
            //        {
            //            //var msg = OAuthFailureHandler.Failure.Message;
            //            var authProperties = options.StateDataFormat.Unprotect(OAuthFailureHandler.Request.Query["state"]);
            //            var redirectUrl = authProperties.RedirectUri;
            //            if (redirectUrl.Contains("/"))
            //            {
            //                redirectUrl = string.Format($"{redirectUrl.Substring(0, redirectUrl.LastIndexOf("/") + 1)}#");// redirectUrl.Substring(0, redirectUrl.IndexOf("/") + 1);
            //            }
            //            //"http://localhost:5001/#"
            //            OAuthFailureHandler.Response.Redirect(redirectUrl);
            //            OAuthFailureHandler.HandleResponse();
            //            return Task.FromResult(0);
            //        }
            //    };


            //});
            #endregion
        }
        private async Task HandleOnRemoteFailure(RemoteFailureContext context)
        {
            var url = context.Request.Host.ToString();

            context.HandleResponse();
            await Task.FromResult(0);
            //await Task.Run(() =>
            //{
            //    context.Response.Redirect("/Home", true);
            //});
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.Map("/Account/token", singin =>
            {
                singin.Run(async context =>
                {
                    if (context.User.Identity.IsAuthenticated)
                    {

                        var result = await context.AuthenticateAsync();
                        var id_token = result.Properties.GetTokenValue("id_token");
                        var access_token = result.Properties.GetTokenValue("access_token");

                        var items = result.Properties.Items;

                        ///HttpContext.GetTokenAsync("refresh_token")
                        //DiscoveryCache _cache = new DiscoveryCache("http://localhost:5002");
                        //var tokenClient = new TokenClient(_cache.TokenEndpoint, "ClientId", "ClientSecret");
                        //await context.Authentication.GetTokenAsync("refresh_token")
                        //var tokenResponse = await tokenClient.RequestRefreshTokenAsync();


                        //result.Properties.AllowRefresh = true;
                        string str = string.Empty;
                        foreach (var item in items)
                        {
                            str += string.Format($"key:{item.Key}:value:{item.Value}");
                        }

                        await context.Response.WriteAsync(str + "\r\n" + id_token);
                        result.Properties.UpdateTokenValue("", "");
                    }
                });
            });

            app.Map("/Account/refreshToken", singin =>
            {
                singin.Run(async context =>
                {
                    if (context.User.Identity.IsAuthenticated)
                    {

                        DiscoveryCache _cache = new DiscoveryCache("http://localhost:5002");

                        var result = await context.AuthenticateAsync();
                        var refreshToken = result.Properties.GetTokenValue("access_token");
                        var refs = result.Properties.GetTokenValue("RefreshToken");
                        //result.Properties.AllowRefresh
                        var rt = await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.GetTokenAsync(context, "refresh_token");


                        //await context.Response.WriteAsync(refreshToken);
                        var disco = await _cache.GetAsync();
                        var _tokenClient = new HttpClient();


                        var pwd = await _tokenClient.RequestPasswordTokenAsync(new PasswordTokenRequest
                        {
                            Address = "http://localhost:5003/connect/token",
                            ClientId = "userinfo_pwd",
                            ClientSecret = "secret",
                            GrantType = "password",
                            UserName = "cnblogs",
                            Password = "123",
                            Scope = "apiInfo.read_full",
                            Parameters =
                            {
                                //{ "custom_parameter", "custom value"},
                                //{ "scope", "apiInfo.read_full" }
                            }

                        });

                        var response = await _tokenClient.RequestRefreshTokenAsync(
                            new RefreshTokenRequest
                            {
                                Address = disco.TokenEndpoint,//"http://localhost:5002/connect/token",
                                ClientId = "OAuth.Client",
                                GrantType = "refresh_token",
                                ClientSecret = "secret",
                                RefreshToken = refreshToken
                            });

                    }
                });

            });
            //app.Run(context => {
            //    var response = context.Response;
            //    var path = context.Request.Path;
            //    return Task.CompletedTask;
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
