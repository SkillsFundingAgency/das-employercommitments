﻿using System.Threading.Tasks;
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
    [CommitmentsRoutePrefix("accounts/{hashedaccountId}")]
    public class TransferRequestController : BaseEmployerController
    {
        public TransferRequestController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ICookieStorageService<string> lastCohortCookieStorageService)
            : base(employerCommitmentsOrchestrator, owinWrapper, multiVariantTestingService, flashMessage, lastCohortCookieStorageService)
        {
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("sender/transfers/{hashedTransferRequestId}")]
        public async Task<ActionResult> TransferDetails(string hashedAccountId, string hashedTransferRequestId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetTransferRequestDetails(hashedAccountId, hashedTransferRequestId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("sender/transfers/{hashedTransferRequestId}/approve")]
        public async Task<ActionResult> TransferApproval(string hashedAccountId, string hashedTransferRequestId,  TransferApprovalConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await EmployerCommitmentsOrchestrator.GetTransferRequestDetails(hashedAccountId, hashedTransferRequestId, OwinWrapper.GetClaimValue(@"sub"));

                return View("TransferDetails", model);
            }
            await EmployerCommitmentsOrchestrator.SetTransferRequestApprovalStatus(hashedAccountId, viewModel.HashedCohortReference, hashedTransferRequestId, viewModel, OwinWrapper.GetClaimValue(@"sub"),
                OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName),
                OwinWrapper.GetClaimValue(DasClaimTypes.Email));

            var status = (bool)viewModel.ApprovalConfirmed ? "approved" : "rejected";

            return View("TransferConfirmation", new TransferConfirmationViewModel { TransferApprovalStatus = status, TransferReceiverName = viewModel.TransferReceiverName});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("sender/transfers/{hashedTransferRequestId}/confirmation")]
        public ActionResult TransferConfirmation(TransferConfirmationViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View("TransferConfirmation", request);
            }
            return Redirect(request.UrlAddress);
        }
    }
}