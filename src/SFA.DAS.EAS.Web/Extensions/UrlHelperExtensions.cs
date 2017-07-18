using System.Web.Mvc;
using Microsoft.Azure;

namespace SFA.DAS.EmployerCommitments.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ExternalUrlAction(this UrlHelper helper, string controllerName, string actionName="", bool ignoreAccountId = false)
        {

            var baseUrl = GetBaseUrl();

            var accountId = helper.RequestContext.RouteData.Values["hashedAccountId"];

            return ignoreAccountId ? $"{baseUrl}{controllerName}/{actionName}" 
                                    : $"{baseUrl}accounts/{accountId}/{controllerName}/{actionName}";
            
        }

        private static string GetBaseUrl()
        {
            return CloudConfigurationManager.GetSetting("MyaBaseUrl").EndsWith("/")
                ? CloudConfigurationManager.GetSetting("MyaBaseUrl")
                : CloudConfigurationManager.GetSetting("MyaBaseUrl") + "/";
        }
    }
}