using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTransferRequest;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedaccountId}")]
    public class TransferRequestController : BaseEmployerController
    {
        private readonly ILinkGenerator _linkGenerator;
        protected readonly ILog _logger;

        public TransferRequestController(EmployerCommitmentsOrchestrator orchestrator, IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ICookieStorageService<string> lastCohortCookieStorageService, ILinkGenerator linkGenerator,
            ILog logger)
            : base(orchestrator, owinWrapper, multiVariantTestingService, flashMessage, lastCohortCookieStorageService)
        {
            _linkGenerator = linkGenerator;
            _logger = logger;
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("sender/transfers/{hashedTransferRequestId}")]
        public ActionResult TransferDetails(string hashedAccountId, string hashedTransferRequestId)
        {
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_linkGenerator.CommitmentsV2Link($"accounts/{hashedAccountId}/sender/transfers/{hashedTransferRequestId}"));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("receiver/transfers/{hashedTransferRequestId}")]
        public ActionResult TransferDetailsForReceiver(string hashedAccountId, string hashedTransferRequestId)
        {
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_linkGenerator.CommitmentsV2Link($"accounts/{hashedAccountId}/receiver/transfers/{hashedTransferRequestId}"));
        }
        
    }
}