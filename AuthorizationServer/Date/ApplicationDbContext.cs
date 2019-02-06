using AuthorizationServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Date
{
    /// <summary>
    /// dotnet ef migrations add identity
    ///  dotnet ef migrations remove
    ///  dotnet ef database update
    ///  
    /// dotnet ef migrations script -o d:ck.sql
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
