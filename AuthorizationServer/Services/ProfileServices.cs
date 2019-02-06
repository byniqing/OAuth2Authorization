using AuthorizationServer.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationServer.Services
{
    /// <summary>
    ///  Profile就是用户资料，ids 4里面定义了一个IProfileService的接口用来获取用户的一些信息
    ///  ，主要是为当前的认证上下文绑定claims。我们可以实现IProfileService从外部创建claim扩展到ids4里面。
    ///  然后返回
    /// </summary>
    public class ProfileServices : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public ProfileServices(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<Claim>> GetClaimsFromUserAsync(ApplicationUser user)
        {
            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName)
            };

            var role = await _userManager.GetRolesAsync(user);
            role.ToList().ForEach(f =>
            {
                claims.Add(new Claim(JwtClaimTypes.Role, f));
            });

            if (!string.IsNullOrEmpty(user.Avatar))
            {
                claims.Add(new Claim("avatar", user.Avatar));
            }
            claims.Add(new Claim("姓名", "tom"));
            return claims;
        }

        /// <summary>
        /// 获取用户Claims
        /// 用户请求userinfo endpoint时会触发该方法
        /// http://localhost:5003/connect/userinfo
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //判断用户在同意授权页面是否选择了profile，如果没有选择，则不用返回profile信息
            var isGrantProfile = context.RequestedResources.IdentityResources.Where(s => s.Name == "profile").ToList();

            //如果请求了profile，那么RequestedClaimTypes也是有值的，否则为空 
            var claimTypes = context.RequestedClaimTypes;

            /*
             profile包含了一些基本的用户信息：

             */


            if (isGrantProfile != null && isGrantProfile.Any())
            {
                //直接获取一样
                if (context.IssuedClaims.Count == 0&& context.Subject.Claims.Count() > 0)
                    context.IssuedClaims = context.Subject.Claims.ToList();


                //if (context.IssuedClaims.Count == 0)
                //{
                //    if (context.Subject.Claims.Count() > 0)
                //    {
                //        context.IssuedClaims = context.Subject.Claims.ToList();
                //    }
                //}

                //也可以这样
                var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(subjectId);
                //context.IssuedClaims = await GetClaimsFromUserAsync(user);
            }
           
        }

        /// <summary>
        /// 判断用户是否可用
        /// Identity Server会确定用户是否有效
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = user != null; //该用户是否已经激活，可用，否则不能接受token

            /*
             这样还应该判断用户是否已经锁定，那么应该IsActive=false
             */
        }
    }
}
