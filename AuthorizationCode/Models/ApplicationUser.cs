using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationCode.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        ///// <summary>
        ///// 重写主键ID方式
        ///// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //自增方式
        //[Key] //主键
        //public override int Id { get; set; }

    }
}
