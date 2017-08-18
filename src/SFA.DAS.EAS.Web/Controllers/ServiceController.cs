using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [RoutePrefix("Service")]
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
    }
}