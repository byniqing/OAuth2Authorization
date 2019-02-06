using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PassWord
{
    /// <summary>
    /// 客户端模式，请求授权服务器获取token，请求资源服务器获取资源
    /// 依赖包：IdentityModel
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
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


            //获取token
            var AccessToken = "";
            var RefreshToken = "";
            //通过code获取AccessToken
            var client1 = new HttpClientHepler(disco.TokenEndpoint);
            client1.PostAsync(null,
                "client_id=userinfo_pwd" +
                "&client_secret=secret" +
                "&grant_type=password" +
                "&username=cnblogs" +
                "&password=123",
                hd => hd.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded"),
                rtnVal =>
                {
                    var jsonVal = JsonConvert.DeserializeObject<dynamic>(rtnVal);
                    AccessToken = jsonVal.access_token;
                    RefreshToken = jsonVal.refresh_token;
                }).Wait();

            //用RefreshToken 刷新AccessToken
            var responseToken = tokenCliet.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "userinfo_pwd",
                ClientSecret = "secret",
                RefreshToken = RefreshToken
            }).Result;



            var response = tokenCliet.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "userinfo_pwd",
                ClientSecret = "secret",
                //GrantType = "password",
                UserName = "cnblogs",
                Password = "123",
                Scope = "apiInfo.read_full"
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


    public class HttpClientHepler
    {
        string _url;
        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                _url = value;
            }
        }

        public HttpClientHepler(string url)
        {
            Url = url;
        }

        public async Task GetAsync(string queryString, Action<HttpRequestHeaders> addHeader,
            Action<string> okAction = null,
            Action<HttpResponseMessage> faultAction = null, Action<Exception> exAction = null)
        {
            using (HttpClient client = new HttpClient())
            {
                addHeader(client.DefaultRequestHeaders);
                using (HttpResponseMessage response = await client.GetAsync(Url + "?" + queryString))
                {
                    try
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            okAction(await response.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            faultAction?.Invoke(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        exAction?.Invoke(ex);
                    }
                }
            }
        }

        public async Task PostAsync(string queryString, string content, Action<HttpContentHeaders> addHeader,
            Action<string> okAction = null,
            Action<HttpResponseMessage> faultAction = null, Action<Exception> exAction = null)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(content))
                {
                    addHeader(httpContent.Headers);
                    using (HttpResponseMessage response = await client.PostAsync(Url + "?" + queryString, httpContent))
                    {
                        try
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                okAction(await response.Content.ReadAsStringAsync());
                            }
                            else
                            {
                                faultAction?.Invoke(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            exAction?.Invoke(ex);
                        }
                    }
                }
            }
        }


    }
}
