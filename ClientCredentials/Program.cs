using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace ClientCredentials
{
    /// <summary>
    /// 客户端模式，请求授权服务器获取token，请求资源服务器获取资源
    /// 依赖包：IdentityModel
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //var Singnature = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.AWo3hMYNvo5EEzxOwYSR8weafaj4OnrcwsW2PJzmxgI";

            //var b = Convert.FromBase64String(Singnature);


            //string result = Encoding.ASCII.GetString(b);
            //string ab = Encoding.UTF8.GetString(b);
            //string cc = Convert.ToBase64String(Encoding.Default.GetBytes(ab));


            if (Singnature.Contains("."))
            {
                //Console.WriteLine("\nAccess Token (decoded):");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAccess Token (decoded):");
                Console.ResetColor();

                var parts = Singnature.Split('.');
                var header = parts[0];
                var claims = parts[1];
                var claims1= parts[2];
                Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims1))));
            }



            string Authority = "http://localhost:5003";
            string ApiResurce = "http://localhost:5002/";
            var tokenCliet = new HttpClient()
            {
                BaseAddress = new Uri(ApiResurce)
            };

            /*
             这样做的目的是：
             资源服务器会去授权服务器认证，所以在客户端可以先判断下授权服务器是否挂了
             */
            DiscoveryCache _cache = new DiscoveryCache(Authority);
            var disco1 = _cache.GetAsync().Result;
            if (disco1.IsError) throw new Exception(disco1.Error);
            //或者
            var disco = tokenCliet.GetDiscoveryDocumentAsync(Authority).Result;
            if (disco.IsError) throw new Exception(disco.Error);


            var response = tokenCliet.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "780987652",
                ClientSecret = "secret",
                //GrantType= "client_credentials"
            }).Result;

            if (response.IsError) throw new Exception(response.Error);

            var token = response.AccessToken;

            //把token，Decode
            if (response.AccessToken.Contains("."))
            {
                //Console.WriteLine("\nAccess Token (decoded):");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAccess Token (decoded):");
                Console.ResetColor();

                var parts = response.AccessToken.Split('.');
                var header = parts[0];
                var claims = parts[1];

                Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
            }
            //设置请求的Token
            tokenCliet.SetBearerToken(token);
            //请求并返回字符串
            var apiResource1 = tokenCliet.GetStringAsync("identity").Result;
            var userinfo = tokenCliet.GetStringAsync("identity/userinfo").Result;

            var j = JObject.Parse(userinfo);
            //或者
            var getVal = tokenCliet.GetAsync("api/values").Result;
            if (getVal.IsSuccessStatusCode)
            {
                Console.WriteLine(getVal.Content.ReadAsStringAsync().Result);
            }
            Console.ReadLine();
        }
    }
}
