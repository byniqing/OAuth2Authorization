using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationCode.Models
{
    public class ApplicationRole:IdentityRole<int>
    {
    }
}
