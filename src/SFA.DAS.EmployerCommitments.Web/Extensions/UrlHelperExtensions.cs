﻿using System.Web.Mvc;
using Microsoft.Azure;

namespace SFA.DAS.EmployerCommitments.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ExternalUrlAction(this UrlHelper helper, string controllerName, string actionName="")
        {

            var baseUrl = GetBaseUrl();

            var accountId = helper.RequestContext.RouteData.Values["hashedAccountId"];

            return $"{baseUrl}accounts/{accountId}/{controllerName}/{actionName}";
        }

        private static string GetBaseUrl()
        {
            return CloudConfigurationManager.GetSetting("EmployerAccountBaseUrl").EndsWith("/")
                ? CloudConfigurationManager.GetSetting("EmployerAccountBaseUrl")
                : CloudConfigurationManager.GetSetting("EmployerAccountBaseUrl") + "/";
        }
    }
}