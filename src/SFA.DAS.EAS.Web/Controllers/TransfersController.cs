using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerUsers.WebClientComponents;

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
        [Route("{hashedCommitmentId}")]
        public async Task<ActionResult> TransferDetails(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetCommitmentDetailsForTransfer(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/approve")]
        public async Task<ActionResult> TransferApproval(string hashedAccountId, string hashedCommitmentId, TransferApprovalConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await EmployerCommitmentsOrchestrator.GetCommitmentDetailsForTransfer(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

                return View("TransferDetails", model);
            }

            await EmployerCommitmentsOrchestrator.SetTransferApprovalStatus(hashedAccountId, hashedCommitmentId, viewModel, OwinWrapper.GetClaimValue(@"sub"),
                OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName),
                OwinWrapper.GetClaimValue(DasClaimTypes.Email));

            if (viewModel.ApprovalConfirmed == true)
            {
                return RedirectToAction("TransferApprovedConfirmation");
            }

            return RedirectToAction("TransferRejectedConfirmation");
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/approved")]
        public async Task<ActionResult> TransferApprovedConfirmation()
        {
            return View("TransferConfirmation", (object)"Approved");
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/rejected")]
        public async Task<ActionResult> TransferRejectedConfirmation()
        {
            return View("TransferConfirmation", (object)"Rejected");
        }

        [HttpGet]
        [Route("Confirmation")]
        public async Task<ActionResult> TransferConfirmation(string approvalAction)
        {
            return View();
        }

    }
}