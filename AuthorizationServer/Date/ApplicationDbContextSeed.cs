using AuthorizationServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Date
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManger;
        private RoleManager<ApplicationRole> _roleManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="server">依赖注入的容器，这里可以获取依赖注入</param>
        /// <returns></returns>
        public async Task AsyncSpeed(ApplicationDbContext context, IServiceProvider server)
        {
            try
            {
                _userManger = server.GetRequiredService<UserManager<ApplicationUser>>();
                _roleManager = server.GetService<RoleManager<ApplicationRole>>();
                var logger = server.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogInformation("speed Init");
                //创建角色
                if (!_roleManager.Roles.Any())
                {
                    var role = new ApplicationRole
                    {
                        Name = "Admin",
                        NormalizedName = "Admin"
                    };
                    var roleResult = await _roleManager.CreateAsync(role);
                }

                //如果没有用户，则创建一个
                if (!_userManger.Users.Any())
                {
                    var defaultUser = new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = "cnblogs@163.com",
                        Avatar = "https://www.cnblogs.com/images/logo_small.gif",
                        //SecurityStamp = "ad", //设置密码的加密key
                    };
                    var userResult = await _userManger.CreateAsync(defaultUser, "123456");

                    //把用户添加到角色权限组
                    await _userManger.AddToRoleAsync(defaultUser, "admin");

                    if (!userResult.Succeeded)
                    {
                        logger.LogError("创建失败");
                        //logger.LogInformation("初始化用户失败");
                        userResult.Errors.ToList().ForEach(e =>
                        {
                            logger.LogError(e.Description);
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("初始化用户失败");
            }
        }
    }
}
