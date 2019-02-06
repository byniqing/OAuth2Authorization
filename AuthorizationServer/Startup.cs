using AuthorizationServer.Date;
using AuthorizationServer.Models;
using AuthorizationServer.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;


namespace AuthorizationServer
{
    /// <summary>
    /// 添加包:IdentityServer4.AspNetIdentity
    /// 添加包:IdentityServer4.EntityFramework
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
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //配置DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //配置文件前面必须是；ConnectionStrings
                //因为默认是：GetSection("ConnectionStrings")[name].
                options.UseSqlServer(Configuration.GetConnectionString("conn"));
                //options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            });

            //配置identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); //添加默认的令牌程序

            ////配置密码注册方式
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
            });

            /*
             https://github.com/IdentityServer/IdentityServer4/blob/63a50d7838af25896fbf836ea4e4f37b5e179cd8/src/Constants.cs
             */
            //注册ids中间件
            services.AddIdentityServer(options =>
            {
                //默认值是这个，可以根据自己修改
                options.UserInteraction.LoginUrl = "/Account/Login";//没有授权，跳转的登陆页面
                //options.UserInteraction.LogoutUrl = ""; //退出页面
                //options.UserInteraction.ConsentUrl = ""; //同意授权页面
            })
            .AddDeveloperSigningCredential()//设置开发者临时签名凭据
            //in-men 方式把信息添加到内存中
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryIdentityResources(Config.GetIdentityResource())
            .AddInMemoryClients(Config.GetClients())

            /*
             * 添加来自数据的配置，包括client,ApiResources,IdentityResource信息
             * ConfigurationStore管理的client,ApiResources,IdentityResource信息
             * MigrationsAssembly：配置为此上下文维护迁移的程序集实例，以便其他地方调用migrations，初始化
             * 如果不配置这个，这样就是空对象
             * 用户登陆的信息会保存在PersistedGrants表，
             */
            //.AddConfigurationStore(options =>
            //{
            //    options.ConfigureDbContext = builder =>
            //    {
            //        builder.UseSqlServer(Configuration.GetConnectionString("conn"),
            //            sql => sql.MigrationsAssembly(migrationAssembly));
            //    };
            //})

            /*
             这里存储的是，给用户授权的token和一些授权信息  
             添加来自数据库的操作数据（codes, tokens, consents）
             */
            //.AddOperationalStore(options =>
            //{
            //    options.ConfigureDbContext = builder =>
            //    {
            //        builder.UseSqlServer(Configuration.GetConnectionString("conn"),
            //            sql => sql.MigrationsAssembly(migrationAssembly));
            //    };
            //})
            .AddAspNetIdentity<ApplicationUser>()
            //.AddProfileService<ProfileServices>()
            //或者
            .Services.AddScoped<IProfileService, ProfileServices>();

            //.AddProfileService<ProfileServices>();
            //.AddTestUsers(Config.GetTestUsers());

            /*
             不使用测试用户了。就不会在用TestUserStore里面的TestUser的测试数据了
             1：添加包：IdentityServer4.AspNetIdentity
             2：添加注入:AddAspNetIdentity<ApplicationUser>()
             */

            //services.AddAuthentication();
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

            //InitIdentityServerDataBase(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //使用ids中间件
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// 因为现在没有通过UI去录入api,client等信息
        /// 所有可以先init一些默认信息写入数据库
        /// </summary>
        /// <param name="app"></param>
        public void InitIdentityServerDataBase(IApplicationBuilder app)
        {
            //ApplicationServices返回的就是IServiceProvider，依赖注入的容器
            using (var scope = app.ApplicationServices.CreateScope())
            {
                //Update-Database
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                //var ckk = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
                //ckk.PersistedGrants.Add(new IdentityServer4.EntityFramework.Entities.PersistedGrant {

                //});

                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                /*
                 如果不走这个，
                 那么应该手动执行 Update-Database -Context PersistedGrantDbContext
                 */
                configurationDbContext.Database.Migrate();

                if (!configurationDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        //client.ToEntity() 会把当前实体映射到EF实体
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        configurationDbContext.ApiResources.Add(api.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResource())
                    {
                        configurationDbContext.IdentityResources.Add(identity.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
            }
        }
    }
}
