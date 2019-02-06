using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
namespace AuthorizationServer.Date
{
    public static class WebHostMigrationExtensions
    {
        public static IWebHost MigrationDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> sedder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                //拿到依赖注入容器
                var servers = scope.ServiceProvider;
                //var logger = servers.GetRequiredService<ILogger<TContext>>();
                var logger = servers.GetService<ILogger<TContext>>();
                var context = servers.GetService<TContext>();

                try
                {
                    context.Database.Migrate();
                    sedder(context, servers);
                    logger.LogInformation($"执行DbContex{typeof(TContext).Name}seed方法成功");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"执行DbContex{typeof(TContext).Name}seed方法失败");
                }
            }
            return host;
        }
    }
}
