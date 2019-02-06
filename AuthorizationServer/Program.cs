using AuthorizationServer.Date;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                .MigrationDbContext<ApplicationDbContext>((context, service) =>
                {
                    new ApplicationDbContextSeed().AsyncSpeed(context, service).Wait();
                })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5003")
                .UseStartup<Startup>();
    }
}
