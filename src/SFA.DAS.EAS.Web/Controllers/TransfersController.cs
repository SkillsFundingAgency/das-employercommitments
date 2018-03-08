using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{

    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedaccountId}/transfers")]
    public class TransfersController : BaseEmployerController
    {
        public TransfersController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ICookieStorageService<string> lastCohortCookieStorageService)
            : base(employerCommitmentsOrchestrator, owinWrapper, multiVariantTestingService, flashMessage, lastCohortCookieStorageService)
        {
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/transfer")]
        public async Task<ActionResult> TransferDetails(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetCommitmentDetailsForTransfer(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/transfer/demo")]
        public async Task<ActionResult> TransferDetailsDemo(string hashedAccountId, string hashedCommitmentId)
        {
            var model = new OrchestratorResponse<TransferCommitmentViewModel>();
            model.Data = new TransferCommitmentViewModel
            {
                HashedAccountId = "ABC123",
                HashedCohortReference = "COH1234",
                LegalEntityName = "Receiving Company Name",
                TotalCost = 15000
            };

            model.Data.TrainingList.Add(new TransferCourseSummaryViewModel { CourseTitle = "Course AAAA", ApprenticeshipCount = 12 });
            model.Data.TrainingList.Add(new TransferCourseSummaryViewModel { CourseTitle = "Course BBBB", ApprenticeshipCount = 36 });

            return View("TransferDetails", model);
        }


    }
}