using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.ViewModels
{
    public class ConsentOptions
    {
        public static bool EnableOfflineAccess = true;
        public static string OfflineAccessDisplayName = "离线访问";
        public static string OfflineAccessDescription = "即使您处于脱机状态，也可以访问您的应用程序和资源";

        public static readonly string MustChooseOneErrorMessage = "您必须至少选择一个权限";
        public static readonly string InvalidSelectionErrorMessage = "无效的选择";
    }
}
