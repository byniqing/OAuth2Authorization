using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationServer.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        //public string TrueName { get; set; }
        
        //public string Address { get; set; }

        public string Avatar { get; set; }


        ///// <summary>
        ///// 重写主键ID方式
        ///// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //自增方式
        //[Key] //主键
        //public override int Id { get; set; }

    }
}
