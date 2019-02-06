using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string PassWrod { get; set; }
        public string ReturnUrl { get; set; }
        public string Action { get; set; }
    }
}
