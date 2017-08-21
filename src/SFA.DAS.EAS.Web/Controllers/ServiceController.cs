using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [CommitmentsRoutePrefix("Service")]
    public class ServiceController : BaseController
    {
        public ServiceController(IOwinWrapper owinWrapper, IFeatureToggle featureToggle,IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage) 
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            
        }

        // GET: Service

        [Route("signout")]
        public ActionResult SignOut()
        {
            return OwinWrapper.SignOutUser(Url.ExternalUrlAction("service","signout",true));
        }

        [Authorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            var url = Url.ExternalUrlAction("service", $"password/change?userCancelled={userCancelled}", true);
            return Redirect(url);
        }

        [Authorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            var url = Url.ExternalUrlAction("service", $"password/change?userCancelled={userCancelled}", true);
            return Redirect(url);
        }
    }
}