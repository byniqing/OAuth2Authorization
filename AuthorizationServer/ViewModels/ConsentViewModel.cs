using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.ViewModels
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }
        /// <summary>
        /// 身份资源
        /// </summary>
        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        /// <summary>
        /// Api资源
        /// </summary>
        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }
    }
}