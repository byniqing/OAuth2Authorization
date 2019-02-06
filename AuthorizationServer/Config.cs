using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthorizationServer
{
    public class Config
    {
        public static List<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource{
                    Name="offline_access", //这样客户端才能收到refresh_token
                    DisplayName="离线访问",
                    Description="用于返回refresh_token",
                    //Required=true,
                    //Emphasize=true
                }
            };
        }
        /// <summary>
        /// 定义用户可以访问的资源
        /// </summary>
        /// <returns></returns>
        public static List<ApiResource> GetApiResources()
        {
            var oauth = new ApiResource
            {
                Name = "OAuth.ApiName", //这是资源名称
                Description = "2",
                DisplayName = "33",
                Scopes = {
                    new Scope{
                        Name="OAuth1", //这里是指定客户端能使用的范围名称 , 是唯一的
                        Description="描述",
                        DisplayName="获得你的个人信息，好友关系",
                        Emphasize=true,
                        Required=true,
                        //ShowInDiscoveryDocument=true,
                    },
                    new Scope{
                        Name="OAuth2",
                        Description="描述",
                        DisplayName="分享内容到你的博客",
                        Emphasize=true,
                        Required=true,
                    },
                    new Scope{
                        Name="OAuth3",
                        Description="描述",
                        DisplayName="获得你的评论",
                    }
                }
            };
            var oidc = new ApiResource
            {
                Name = "oidc.name", //这是资源名称
                Description = "2",
                DisplayName = "33",
                Scopes = {
                     new Scope{
                        Name="OtherInfo",
                        Description="描述",
                        DisplayName="获取你的其他信息",
                    },
                    //new Scope{
                    //    Name="oidc1", //这里是指定客户端能使用的范围名称 , 是唯一的
                    //    Description="描述",
                    //    DisplayName="获得你的个人信息，好友关系",
                    //    Emphasize=true,
                    //    Required=true,
                    //    //ShowInDiscoveryDocument=true,
                    //},
                    //new Scope{
                    //    Name="oidc2",
                    //    Description="描述",
                    //    DisplayName="分享内容到你的博客",
                    //    Emphasize=true,
                    //    Required=true,
                    //}
                }
            };
            return new List<ApiResource> {
                /*
                 具有单个作用域的简单API，这样定义的话，作用域（scope）和Api名称（ApiName）相同
                 */
                new ApiResource("api","描述"),

                 //如果需要更多控制，则扩展版本
                new ApiResource{
                    Name="userinfo", //资源名称，对应客户端的：ApiName，必须是唯一的
                    Description="描述",
                    DisplayName="", //显示的名称
                  
                    //ApiSecrets =
                    //{
                    //    new Secret("secret11".Sha256())
                    //},

                    //作用域，对应下面的Cliet的 AllowedScopes
                    Scopes={
                        new Scope
                        {
                            Name = "apiInfo.read_full",
                            DisplayName = "完全的访问权限",
                            UserClaims={ "super" }
                        },
                        new Scope
                        {
                            Name = "apiinfo.read_only",
                            DisplayName = "只读权限"
                        }
                    },
                },
                oauth,
                oidc
            };
        }

        /// <summary>
        /// 客户端合法性验证
        /// </summary>
        /// <returns></returns>
        public static List<Client> GetClients()
        {
            #region 客户端模式 ClientCredentials
            var ClientCredentials = new Client
            {

                /******************客户端 请求对应的字段*******************
                 client_id：客户端的ID，必选
                 grant_type：授权类型，必选，此处固定值“code”
                 client_secret：客户端的密码，必选
                 scope：申请的权限范围，可选，如果传了必须是正确的，否则也不通过
                 ************************************/

                //这个Client集合里面，ClientId必须是唯一的
                ClientId = "780987652", // 客户端ID，客户端传过来的必须是这个，验证才能通过,
                AllowedGrantTypes = GrantTypes.ClientCredentials,// 授权类型，指客户端可以使用的模式
                ClientSecrets = { new Secret("secret".Sha256()) }, //客户端密钥
                //ClientSecrets={new Secret("secret".Sha512()) },
                //RequireClientSecret = false, //不验证secret ，一般是信得过的第三方

                ClientName = "客户端名称",
                Description = "描述",
                //Claims = new List<Claim> {
                //    new Claim("super","super")
                //},
                /*
                 权限范围，对应的ApiResouce，这里是客户端模式，对应的是用户资源，所以是ApiResouce
                 如果是oidc 这对应的是identityResouece，身份资源
                 所以是取决于AllowedGrantTypes的类型

                允许客户端访问的API作用域
                 */
                AllowedScopes = { "apiInfo.read_full" } //
            };

            var ClientCredentials1 = new Client
            {
                ClientId = "userinfo",
                AllowedGrantTypes = GrantTypes.ClientCredentials, //客户端输入：client_credentials
                ClientSecrets = { new Secret("secret".Sha256()) },
                ClientName = "客户端名称",
                AllowedScopes = { "apiInfo.read_full" } //
            };
            #endregion
            #region 密码模式 ResourceOwnerPassword
            var pwd = new Client
            {
                ClientId = "userinfo_pwd",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,//客户端输入：password
                ClientSecrets = { new Secret("secret".Sha256()) },
                ClientName = "客户端名称",
                RefreshTokenUsage = TokenUsage.ReUse,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowOfflineAccess = true,
                AllowedScopes = { "apiInfo.read_full" } //
            };
            #endregion


            var oidc = new Client
            {
                ClientId = "oidc",
                ClientName = "博客园",
                ClientSecrets = { new Secret("secret".Sha256()) },
                ClientUri = "http://www.cnblogs.com", //客户端
                LogoUri = "https://www.cnblogs.com/images/logo_small.gif",
                //AllowedGrantTypes={GrantType.AuthorizationCode }

                /*
                 如果客户端使用的认证是
                 */
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowedScopes ={
                    /*
                         Profile就是用户资料，ids 4里面定义了一个IProfileService的接口用来获取用户的一些信息，主要是为当前的认证上下文绑定claims。我们可以实现IProfileService从外部创建claim扩展到ids4里面。
                         */
                        IdentityServerConstants.StandardScopes.Profile,
                          /*
                         openid是必须要的。因为客户端接受的的是oidc
                         客户端会根据oidc和SubjectId获取用户信息，
                         所以：Profile也必须要，Profile 就是用户信息

                        如果没有Profile ，就没有办法确认身份
                         */
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        //IdentityServerConstants.StandardScopes.OfflineAccess,
                        "address","OtherInfo"
                        
                    },
                //客户端默认传过来的是这个地址，如果跟这个不一直就会异常
                RedirectUris = { "http://localhost:5001/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },
                AllowOfflineAccess = true,
                /*
                 这样就会把返回的profile信息包含在idtoken中
                 */
                AlwaysIncludeUserClaimsInIdToken=true
            };
            var oauth = new Client
            {
                ClientId = "OAuth.Client",
                ClientName = "博客园",
                ClientSecrets = { new Secret("secret".Sha256()) },
                //AllowedGrantTypes={GrantType.AuthorizationCode }
                AllowedGrantTypes = GrantTypes.Code,
                //RequireConsent=true,
                ClientUri = "http://www.cnblogs.com", //客户端
                LogoUri = "https://www.cnblogs.com/images/logo_small.gif",
                AllowedScopes ={
                        "OAuth1","OAuth2","OAuth3"
                    },

                /*
                 授权成功后，返回地地址
                 */
                RedirectUris = { "http://localhost:5001/OAuth" },
                //注销后重定向的地址
                PostLogoutRedirectUris = { "http://localhost:5001" },
                //RefreshTokenUsage= TokenUsage.ReUse
                //AllowOfflineAccess = true,
                //AllowAccessTokensViaBrowser = true
            };
            return new List<Client> {
               ClientCredentials,
               ////ClientCredentials1,
               pwd,
               oidc,
               oauth
           };
        }


        /// <summary>
        /// 密码模式，需要用的到用户名和密码，正式操作是在数据库中找
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser
                {
                    SubjectId="100000", //用户ID
                    Username="cnblogs", //用户名
                    Password="123", //密码
                    /*
                     * 这里的信息就是： IdentityServerConstants.StandardScopes.Profile
                     */
                    Claims=new List<Claim>{
                        new Claim("name","nsky"),
                        new Claim("email","cnblgos@sina.com"),
                        new Claim("website","http://www.cnblogs.com")
                    }
                }
            };
        }
    }
}
