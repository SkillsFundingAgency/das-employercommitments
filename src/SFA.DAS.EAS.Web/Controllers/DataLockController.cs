using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedAccountId}/apprentices/manage/{hashedApprenticeshipId}/datalock")]
    public class DataLockController : BaseController
    {
        private readonly DataLockOrchestrator _orchestrator;
        protected readonly ILog _logger;
        private readonly ILinkGenerator _linkGenerator;

        public DataLockController(
            IOwinWrapper owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            DataLockOrchestrator orchestrator,
            ILog logger,
            ILinkGenerator linkGenerator
            ) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _logger = logger;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [Route("restart", Name = "RequestRestart")]
        public ActionResult RequestRestart(string hashedAccountId, string hashedApprenticeshipId)
        {
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/changes/restart"));
        }

        [HttpGet]
        [Route("changes", Name = "RequestChanges")]
        public ActionResult RequestChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/changes/request"));
        }

       
    }
}