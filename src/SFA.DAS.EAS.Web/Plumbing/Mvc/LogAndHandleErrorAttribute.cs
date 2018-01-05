﻿using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using NLog;

namespace SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc
{
    public class LogAndHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && !filterContext.ExceptionHandled)
            {
                var error = filterContext.Exception;
                Logger.Error(error, "Unhandled exception - " + error.Message);
                var ai = new TelemetryClient();
                ai.TrackException(filterContext.Exception);

                base.OnException(filterContext);
            }
        }
    }
}