using SFA.DAS.EmployerCommitments.Web.Controllers;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc
{
    public sealed class GoogleTagManagerFilterAttribute : ActionFilterAttribute
    {
        public GoogleTagManagerFilterAttribute()
        {

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId = null;
            string hashedAccountId = null;

            var thisController = filterContext.Controller as BaseController;

            if (thisController != null)
            { 
                userId = thisController.OwinWrapper.GetClaimValue(@"sub");
            }

            if (filterContext.ActionParameters.ContainsKey("hashedAccountId"))
            {
                hashedAccountId = filterContext.ActionParameters["hashedAccountId"] as string;
            }

            filterContext.Controller.ViewBag.GaData = new GaTagData()
            {
                IsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated,
                UserId = userId,
                Acc = hashedAccountId
            };

            base.OnActionExecuting(filterContext);
        }

        public string DataLoaded { get; set; }

        public class GaTagData
        {
            public string DataLoaded { get; set; } = "dataLoaded";
            public bool IsAuthenticated { get; set; }
            public string UserId { get; set; }
            public string HashedAccountId { get; set; }

            public string Vpv { get; set; }
            public string Acc { get; set; }
            public string Org { get; set; }

            public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        }
    }
}