using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
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
        private readonly EmployerCommitmentsServiceConfiguration _configuration;

        public ServiceController(IOwinWrapper owinWrapper,IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage, EmployerCommitmentsServiceConfiguration configuration) 
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _configuration = configuration;
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            return OwinWrapper.SignOutUser(string.Format(constants.LogoutEndpoint(), idToken, owinContext.Request.Uri.Scheme, owinContext.Request.Uri.Authority));
        }

        [Route("SignOutCleanup")]
        public void SignOutCleanup()
        {
            OwinWrapper.SignOutUser();
        }

        [Authorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            var url = Url.ExternalMyaUrlAction("service", $"password/change?userCancelled={userCancelled}", true);
            return Redirect(url);
        }

        [Authorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            var url = Url.ExternalMyaUrlAction("service", $"password/change?userCancelled={userCancelled}", true);
            return Redirect(url);
        }
    }
}