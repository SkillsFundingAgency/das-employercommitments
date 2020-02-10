using System.Web.Mvc;
using NLog;

namespace SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc
{
    public class DeprecatedAttribute : ActionFilterAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            var urlReferrer = filterContext.RequestContext.HttpContext.Request.UrlReferrer;
            var referrer = urlReferrer == null ? "unknown" : urlReferrer.ToString();

            Logger.Warn($"Deprecated action invoked: {controllerName}.{actionName} from {referrer}");
            base.OnActionExecuting(filterContext);
        }
    }
}