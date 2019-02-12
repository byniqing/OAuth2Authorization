using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ResourcesServer
{
    /// <summary>
    /// 资源服务器认证，给请求资源的第三方认证
    /// 依赖包：IdentityServer4.AccesstokenValidation
    /// 
    /// http://localhost:5003/.well-known/openid-configuration看配置
    /// </summary>
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //默认的认证方式是Bearer认证
            services.AddAuthentication("Bearer")
            //配置要验证的信息
            .AddIdentityServerAuthentication(options =>
            {
                //令牌或者说AccessToken颁发的地址，Token中会包含该地址
                //第一次会去认证服务器获取配置信息
                options.Authority = "http://localhost:5003"; //必填
                //options.ApiName = "userinfo";
                //options.ApiName = "OAuth.ApiName";
                options.ApiSecret = "secret";
                //options.SaveToken = true;
                options.RequireHttpsMetadata = false;//暂时取消Https验证，
            });

            //services.AddAuthorization(options => {
            //    options.AddPolicy("client", policy => policy.RequireClaim("client_id"));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
