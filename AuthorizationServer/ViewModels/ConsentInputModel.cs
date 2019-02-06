using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.ViewModels
{
    public class ConsentInputModel
    {
        /// <summary>
        /// 单击的是同意还是取消
        /// </summary>
        public string Button { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
    }
}
