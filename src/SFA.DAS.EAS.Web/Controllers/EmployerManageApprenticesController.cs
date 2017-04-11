﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {
        private readonly EmployerManageApprenticeshipsOrchestrator _orchestrator;

        public EmployerManageApprenticesController(
            EmployerManageApprenticeshipsOrchestrator orchestrator, 
            IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
                : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ListAll(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeships(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeship(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage != null )
            {
                model.FlashMessage = flashMessage;
                RemoveFlashMessageFromCookie();
            }

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprenticeship")]
        public async Task<ActionResult> Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator.GetApprenticeshipForEdit(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage?.ErrorMessages != null && flashMessage.ErrorMessages.Any())
            {
                model.FlashMessage = flashMessage;
                RemoveFlashMessageFromCookie();
            }
            
            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(hashedAccountId, hashedApprenticeshipId);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(ApprenticeshipViewModel apprenticeship)
        {
            if (!await IsUserRoleAuthorized(apprenticeship.HashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var dictionary = await _orchestrator.ValidateApprenticeship(apprenticeship);
            AddErrorsToFlashDictionaryCookie(dictionary);
            if (dictionary.Any())
            {
                return RedirectToAction("Edit");
            }

            var model = await _orchestrator.GetConfirmChangesModel(apprenticeship.HashedAccountId, apprenticeship.HashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship);

            if (!AnyChanges(model.Data))
            {
                AddErrorsToFlashDictionaryCookie(new Dictionary<string, string> { { "NoChangesRequested", "No changes made" } });
                return RedirectToAction("Edit");
            }

            _orchestrator.CreateApprenticeshipViewModelCookie(model.Data);

            return RedirectToAction("ConfirmChanges");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/SubmitChanges")]
        public async Task<ActionResult> SubmitChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship, string originalApprenticeshipDecoded)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var originalApprenticeship = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
            apprenticeship.OriginalApprenticeship = originalApprenticeship;

            if (!ModelState.IsValid)
            {
                var errorModel = new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = apprenticeship };
                return View("ConfirmChanges", errorModel);
            }

            if (apprenticeship.ChangesConfirmed != null && !apprenticeship.ChangesConfirmed.Value)
            {
                return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
            }
            
            await _orchestrator.CreateApprenticeshipUpdate(apprenticeship, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            var flashmessage = new FlashMessageViewModel
            {
                Message = $"You suggested changes to the record for {originalApprenticeship.FirstName} {originalApprenticeship.LastName}. Your training provider needs to approve these changes.",
                Severity = FlashMessageSeverityLevel.Okay
            };

            AddFlashMessageToCookie(flashmessage);


            return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
        }


        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewPendingChanges")]
        public async Task<ActionResult> ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var viewModel = await _orchestrator
                .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(viewModel);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/view")]
        public async Task<ActionResult> ViewChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship, string originalApprenticeshipDecoded, bool? undoChanges)
        {
            if (undoChanges == null)
            {
                var originalApprenticeship = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
                apprenticeship.OriginalApprenticeship = originalApprenticeship;
                return View(new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = apprenticeship });
            }

            if (undoChanges.Value)
            {
                await _orchestrator.SubmitUndoApprenticeshipUpdate(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
                SetOkayMessage("Changes undone");
            }
            
            return RedirectToAction("Details");
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/review", Name = "ReviewChanges")]
        public async Task<ActionResult> ReviewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var viewModel = await _orchestrator
                .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/review")]
        public async Task<ActionResult> ReviewChanges(string hashedAccountId, string hashedApprenticeshipId, bool? approveChanges)
        {
            var viewModel = await _orchestrator
                .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            if (approveChanges == null)
                return View(viewModel);

            await _orchestrator.SubmitReviewApprenticeshipUpdate(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), approveChanges.Value);

            var message = approveChanges.Value ? "Changes approved" : "Changes rejected";
            SetOkayMessage(message);
            return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
        }

        
        private bool AnyChanges(UpdateApprenticeshipViewModel data)
        {
            return
                   !string.IsNullOrEmpty(data.FirstName)
                || !string.IsNullOrEmpty(data.LastName)
                || data.DateOfBirth != null
                || !string.IsNullOrEmpty(data.TrainingName)
                || data.StartDate != null
                || data.EndDate != null
                || data.Cost != null
                || !string.IsNullOrEmpty(data.EmployerRef);
        }

        private async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await _orchestrator
                .AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }

        private void AddErrorsToFlashDictionaryCookie(Dictionary<string, string> dict)
        {
            var flashMessage = new FlashMessageViewModel
            {
                ErrorMessages = dict,
                Headline = "Errors to fix",
                Message = "Check the following details:",
                Severity = FlashMessageSeverityLevel.Error
            };

            AddFlashMessageToCookie(flashMessage);
        }

        private void SetOkayMessage(string message)
        {
            var flashmessage = new FlashMessageViewModel
            {
                Message = message,
                Severity = FlashMessageSeverityLevel.Okay
            };
            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

            AddFlashMessageToCookie(flashmessage);
        }
    }
}