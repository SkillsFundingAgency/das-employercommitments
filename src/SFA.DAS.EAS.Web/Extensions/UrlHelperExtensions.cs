using System.Web.Mvc;
using Microsoft.Azure;

namespace SFA.DAS.EmployerCommitments.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ExternalMyaUrlAction(this UrlHelper helper, string controllerName, string actionName="", bool ignoreAccountId = false)
        {
            var baseUrl = GetMyaBaseUrl();
            return BuildExternalUrl(helper, baseUrl, controllerName, actionName, ignoreAccountId);
        }

        public static string ExternalPsrUrlAction(this UrlHelper helper, string controllerName, string actionName = "", bool ignoreAccountId = false)
        {
            var baseUrl = GetPsrBaseUrl();
            return BuildExternalUrl(helper, baseUrl, controllerName, actionName, ignoreAccountId);
        }

        public static string ExternalRecruitUrlAction(this UrlHelper helper, string controllerName = "", string actionName = "", bool ignoreAccountId = false)
        {
            var baseUrl = GetRecruitBaseUrl();
            return BuildExternalUrl(helper, baseUrl, controllerName, actionName, ignoreAccountId);
        }
        
        private static string GetMyaBaseUrl()
        {
            return GetBaseUrl("MyaBaseUrl");
        }

        private static string GetPsrBaseUrl()
        {
            return GetBaseUrl("PsrBaseUrl");
        }

        private static string GetRecruitBaseUrl()
        {
            return GetBaseUrl("RecruitBaseUrl");
        }

        private static string GetBaseUrl(string configKey)
        {
            return CloudConfigurationManager.GetSetting(configKey).EndsWith("/")
                ? CloudConfigurationManager.GetSetting(configKey)
                : CloudConfigurationManager.GetSetting(configKey) + "/";
        }

        private static string BuildExternalUrl(UrlHelper helper, string baseUrl, string controllerName, string actionName = "", bool ignoreAccountId = false)
        {
            var accountId = helper.RequestContext.RouteData.Values["hashedAccountId"];

            return ignoreAccountId ? $"{baseUrl}{controllerName}{(string.IsNullOrWhiteSpace(controllerName) ? "" : "/")}{actionName}"
                : $"{baseUrl}accounts/{accountId}/{controllerName}{(string.IsNullOrWhiteSpace(controllerName) ? "" : "/")}{actionName}";
        }
    }
}