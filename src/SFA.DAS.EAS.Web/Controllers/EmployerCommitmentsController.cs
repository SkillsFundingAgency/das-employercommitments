using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.EmployerUrlHelper.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedaccountId}/apprentices")]
    public class EmployerCommitmentsController : BaseEmployerController
    {
        private readonly IFeatureToggleService _featureToggleService;
        private readonly ILinkGenerator _linkGenerator;

        public EmployerCommitmentsController(
            IEmployerCommitmentsOrchestrator orchestrator,
            IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            ICookieStorageService<string> lastCohortCookieStorageService,
            IFeatureToggleService featureToggleService,
            ILinkGenerator linkGenerator)
            : base(orchestrator, owinWrapper, multiVariantTestingService, flashMessage, lastCohortCookieStorageService)
        {
            _featureToggleService = featureToggleService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [Route("home", Name = "CommitmentsHome")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}"));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts")]
        public async Task<ActionResult> YourCohorts(string hashedAccountId)
        {
            if (_featureToggleService.Get<EnhancedApprovals>().FeatureEnabled)
                return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/unapproved"));

            // otherwise call existing code
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await Orchestrator.GetYourCohorts(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            SetFlashMessageOnModel(model);
            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts/draft")]
        public async Task<ActionResult> Draft(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (_featureToggleService.Get<EnhancedApprovals>().FeatureEnabled)
                return Redirect(Url.CommitmentsV2Link($"{hashedAccountId}/unapproved/draft"));

            var model = await Orchestrator.GetAllDraft(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            SetFlashMessageOnModel(model);
            SaveRequestStatusInCookie(RequestStatus.NewRequest);
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/review")]
        public async Task<ActionResult> ReadyForReview(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor ))
                return View("AccessDenied");

            if (_featureToggleService.Get<EnhancedApprovals>().FeatureEnabled)
                return Redirect(Url.CommitmentsV2Link($"{hashedAccountId}/unapproved/review"));

            var model = await Orchestrator.GetAllReadyForReview(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            SetFlashMessageOnModel(model);
            SaveRequestStatusInCookie(RequestStatus.ReadyForReview);
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/provider")]
        public async Task<ActionResult> WithProvider(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (_featureToggleService.Get<EnhancedApprovals>().FeatureEnabled)
                return Redirect(Url.CommitmentsV2Link($"{hashedAccountId}/unapproved/with-training-provider"));

            SaveRequestStatusInCookie(RequestStatus.WithProviderForApproval);

            var model = await Orchestrator.GetAllWithProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/transferFunded")]
        public async Task<ActionResult> TransferFunded(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (_featureToggleService.Get<EnhancedApprovals>().FeatureEnabled)
                return Redirect(Url.CommitmentsV2Link($"{hashedAccountId}/unapproved/with-transfer-sender"));

            //todo: the pattern seems to be pick one of the statuses associated with a bingo box and save that in the cookie
            // to represent e.g. which page to go back to after delete. we could refactor this, perhaps introduce a new enum.
            // also, subsequent transfer stories will need to check for this status when they GetRequestStatusFromCookie()
            SaveRequestStatusInCookie(RequestStatus.WithSenderForApproval);

            var model = await Orchestrator.GetAllTransferFunded(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View("TransferFundedCohorts", model);
        }

        [HttpGet]
        [Route("cohorts/rejectedTransfers")]
        public async Task<ActionResult> RejectedTransfers(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            SaveRequestStatusInCookie(RequestStatus.RejectedBySender);

            var model = await Orchestrator.GetAllRejectedTransferFunded(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View("RejectedTransferFundedCohorts", model);
        }

        [HttpGet]
        [Route("Inform")]
        public async Task<ActionResult> Inform(string hashedAccountId)
        {
            SaveRequestStatusInCookie(RequestStatus.None);
            var response = await Orchestrator.GetInform(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("transferConnection/create")]
        public async Task<ActionResult> SelectTransferConnection(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await Orchestrator
                .GetTransferConnections(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Data.TransferConnections.Any())
            {
                return View(response);
            }
            return RedirectToAction("SelectLegalEntity");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("transferConnection/create")]
        public async Task<ActionResult> SetTransferConnection(string hashedAccountId, SelectTransferConnectionViewModel selectedTransferConnection)
        {
            if (!ModelState.IsValid)
            {
                var response = await Orchestrator.GetTransferConnections(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                return View("SelectTransferConnection", response);
            }

            var transferConnectionCode =
                selectedTransferConnection.TransferConnectionCode.Equals("None",
                    StringComparison.InvariantCultureIgnoreCase)
                    ? null
                    : selectedTransferConnection.TransferConnectionCode;

            return RedirectToAction("SelectLegalEntity", new { transferConnectionCode });
        }

        [HttpGet]
        [Route("legalEntity/create")]
        public async Task<ActionResult> SelectLegalEntity(string hashedAccountId, string transferConnectionCode, string cohortRef = "")
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await Orchestrator.GetLegalEntities(hashedAccountId, transferConnectionCode, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Data.LegalEntities == null || !response.Data.LegalEntities.Any())
                throw new InvalidStateException($"No legal entities associated with account {hashedAccountId}");

            if (response.Data.LegalEntities.Count() > 1)
                return View(response);

            var autoSelectLegalEntity = response.Data.LegalEntities.First();

            var hasSigned = EmployerCommitmentsOrchestrator.HasSignedAgreement(
                autoSelectLegalEntity, !string.IsNullOrWhiteSpace(transferConnectionCode));

            if (hasSigned)
            {
                return RedirectToAction("SearchProvider", new SelectLegalEntityViewModel
                {
                    TransferConnectionCode = transferConnectionCode,
                    CohortRef = response.Data.CohortRef,
                    LegalEntityCode = autoSelectLegalEntity.Code,
                    // no need to store LegalEntities, as the property is only read in the SelectLegalEntity view, which we're now skipping
                });
            }

            return RedirectToAction("AgreementNotSigned", new 
            {
                HashedAccountId = hashedAccountId,
                LegalEntityCode = autoSelectLegalEntity.Code,
                TransferConnectionCode = transferConnectionCode,
                CohortRef = response.Data.CohortRef,
                HasSignedAgreement = false,
                LegalEntityName = autoSelectLegalEntity.Name ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("legalEntity/create")]
        public async Task<ActionResult> SetLegalEntity(string hashedAccountId, SelectLegalEntityViewModel selectedLegalEntity)
        {
            if (!ModelState.IsValid)
            {
                var response = await Orchestrator.GetLegalEntities(hashedAccountId,
                    selectedLegalEntity.TransferConnectionCode, selectedLegalEntity.CohortRef,
                    OwinWrapper.GetClaimValue(@"sub"));

                return View("SelectLegalEntity", response);
            }

            var agreement = await Orchestrator.GetLegalEntitySignedAgreementViewModel(hashedAccountId,
                selectedLegalEntity.TransferConnectionCode, selectedLegalEntity.LegalEntityCode, selectedLegalEntity.CohortRef, OwinWrapper.GetClaimValue(@"sub"));

            if (agreement.Data.HasSignedAgreement)
                return RedirectToAction("SearchProvider", selectedLegalEntity);

            return RedirectToAction("AgreementNotSigned", agreement.Data);
        }

        [HttpGet]
        [Route("provider/create")]
        public async Task<ActionResult> SearchProvider(string hashedAccountId, string transferConnectionCode, string legalEntityCode, string cohortRef)
        {
            var legalEntities = await Orchestrator.GetLegalEntities(hashedAccountId, string.Empty, string.Empty, OwinWrapper.GetClaimValue(@"sub"));
            var legalEntity = legalEntities.Data.LegalEntities.Single(x => x.Code == legalEntityCode);
            var hashedAleId = legalEntity.AccountLegalEntityPublicHashedId;

            var url = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/unapproved/add/select-provider?AccountLegalEntityHashedId={hashedAleId}&transferSenderId={transferConnectionCode}");
            return Redirect(url);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (_featureToggleService.Get<EnhancedApprovals>().FeatureEnabled)
                return Redirect(Url.CommitmentsV2Link($"{hashedAccountId}/unapproved/{hashedCommitmentId}"));

            var model = await Orchestrator.GetCommitmentDetails(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            model.Data.BackLinkUrl = GetReturnToListUrl(hashedAccountId);
            SetFlashMessageOnModel(model);

            ViewBag.HashedAccountId = hashedAccountId;

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        public async Task<ActionResult> DeleteCohort(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await Orchestrator.GetDeleteCommitmentModel(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCohort(DeleteCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await Orchestrator
                    .GetDeleteCommitmentModel(viewModel.HashedAccountId, viewModel.HashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

                return View(model);
            }

            if (viewModel.DeleteConfirmed == null || !viewModel.DeleteConfirmed.Value)
            {
                return Redirect(_linkGenerator.CommitmentsV2Link($"{viewModel.HashedAccountId}/unapproved/{viewModel.HashedCommitmentId}"));
            }

            await Orchestrator
                .DeleteCommitment(viewModel.HashedAccountId, viewModel.HashedCommitmentId, OwinWrapper.GetClaimValue("sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName), OwinWrapper.GetClaimValue(DasClaimTypes.Email));

            var flashmessage = new FlashMessageViewModel
            {
                Message = "Records deleted",
                Severity = FlashMessageSeverityLevel.Okay
            };

            AddFlashMessageToCookie(flashmessage);
            
            var anyCohortWithCurrentStatus = 
                await Orchestrator.AnyCohortsForCurrentStatus(viewModel.HashedAccountId, GetRequestStatusFromCookie());

            if(!anyCohortWithCurrentStatus)
                return RedirectToAction("YourCohorts", new { viewModel.HashedAccountId });

            return Redirect(GetReturnToListUrl(viewModel.HashedAccountId));
        }

        [HttpGet]
        [Route("{legalEntityCode}/AgreementNotSigned")]
        public async Task<ActionResult> AgreementNotSigned(LegalEntitySignedAgreementViewModel viewModel)
        {
            async Task<bool> IsLevyEmployer()
            {
                return (await Orchestrator.GetApprenticeshipEmployerType(viewModel.HashedAccountId)) == ApprenticeshipEmployerType.Levy;
            }

            viewModel.CanContinueAnyway = await IsLevyEmployer();
            return View(viewModel);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/view")]
        public async Task<ActionResult> ViewApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await Orchestrator.GetApprenticeshipViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);
            return View("ViewApprenticeshipEntry", model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/finished")]
        public async Task<ActionResult> FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await Orchestrator.GetFinishEditingViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/finished")]
        public async Task<ActionResult> FinishedEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var response = await Orchestrator.GetFinishEditingViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId);
                return View(response);
            }

            if(viewModel.SaveStatus.IsSend())
            {
                return RedirectToAction("SubmitExistingCommitment", 
                    new { viewModel.HashedAccountId, viewModel.HashedCommitmentId, viewModel.SaveStatus });
            }

            if (viewModel.SaveStatus.IsFinalApproval())
            {
                var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
                var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
                var userId = OwinWrapper.GetClaimValue(@"sub");
                await Orchestrator.ApproveCommitment(viewModel.HashedAccountId, userId, userDisplayName, userEmail, viewModel.HashedCommitmentId, viewModel.SaveStatus);

                return RedirectToAction("Approved",
                    new { viewModel.HashedAccountId, viewModel.HashedCommitmentId });
            }

            var flashmessage = new FlashMessageViewModel
            {
                Headline = "Details saved but not sent",
                Severity = FlashMessageSeverityLevel.Info
            };

            AddFlashMessageToCookie(flashmessage);

            return RedirectToAction("YourCohorts", new { hashedAccountId = viewModel.HashedAccountId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/CohortApproved")]
        public async Task<ActionResult> Approved(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await Orchestrator.GetAcknowledgementModelForExistingCommitment(
                hashedAccountId,
                hashedCommitmentId,
                OwinWrapper.GetClaimValue(@"sub"));

            var currentStatusCohortAny = await Orchestrator
                .AnyCohortsForCurrentStatus(hashedAccountId, RequestStatus.ReadyForApproval);
            model.Data.BackLink = currentStatusCohortAny
                ? new LinkViewModel { Text = "Return to view cohorts", Url = Url.Action("ReadyForReview", new { hashedAccountId }) }
                : new LinkViewModel { Text = "Return to Your cohorts", Url = Url.Action("YourCohorts", new { hashedAccountId }) };

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/submit")]
        public async Task<ActionResult> SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await Orchestrator.GetSubmitCommitmentModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, saveStatus);
            return View("SubmitCommitmentEntry", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/submit")]
        public async Task<ActionResult> SubmitExistingCommitmentEntry(SubmitCommitmentViewModel model)
        {
            var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
            var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
            var userId = OwinWrapper.GetClaimValue(@"sub");

            await Orchestrator.SubmitCommitment(model, userId, userDisplayName, userEmail);

            return RedirectToAction("AcknowledgementExisting", new { hashedCommitmentId = model.HashedCommitmentId, model.SaveStatus });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await Orchestrator
                .GetAcknowledgementModelForExistingCommitment(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            var status = GetRequestStatusFromCookie();
            bool anyCohortsLeft;
            if (status == RequestStatus.ReadyForReview)
            {
                anyCohortsLeft = await Orchestrator.AnyCohortsForCurrentStatus(hashedAccountId, RequestStatus.ReadyForApproval, status);
            }
            else
            {
                anyCohortsLeft= await Orchestrator.AnyCohortsForCurrentStatus(hashedAccountId, status);
            }
            var returnToCohortsList = 
                   status != RequestStatus.None 
                && anyCohortsLeft;

            var returnUrl = GetReturnUrl(status, hashedAccountId);
            response.Data.BackLink = string.IsNullOrEmpty(returnUrl) || !returnToCohortsList
                ? new LinkViewModel { Url = Url.Action("YourCohorts", new { hashedAccountId }), Text = "Return to Your cohorts" }
                : new LinkViewModel { Url = returnUrl, Text = "Go back to view cohorts" };

            response.Data.Content = GetAcknowledgementContent(saveStatus, response.Data.IsTransfer);

            return View("Acknowledgement", response);
        }

        private AcknowledgementContent GetAcknowledgementContent(SaveStatus saveStatus, bool isTransfer)
        {
            var acknowledgementContent = new AcknowledgementContent {WhatHappensNext = new List<string>()};

            switch (saveStatus)
            {
                case SaveStatus.AmendAndSend:
                    acknowledgementContent.Title = "Cohort sent for review";
                    acknowledgementContent.WhatHappensNext.Add("Your training provider will review your cohort and contact you as soon as possible.");
                    break;
                case SaveStatus.ApproveAndSend:
                {
                    acknowledgementContent.Title = "Cohort approved and sent to training provider";
                    acknowledgementContent.WhatHappensNext.Add(
                        "Your training provider will review your cohort and either confirm the information is correct or contact you to suggest changes.");
                    if (isTransfer)
                    {
                        acknowledgementContent.WhatHappensNext.Add(
                            "Once the training provider approves the cohort a transfer request will be sent to the funding employer to review.");
                        acknowledgementContent.WhatHappensNext.Add(
                            "You’ll receive a notification once the funding employer approves or rejects the transfer request and can view the progress of a request in the with transfer sending employers section."
                        );
                    }
                    break;
                }
                case SaveStatus.Save:
                    acknowledgementContent.Title = "Cohort sent to your training provider";
                    acknowledgementContent.WhatHappensNext.Add(
                        "Your training provider will review your request and contact you as soon as possible - either with questions or to ask you to review the apprentice details they've added.");
                    break;
            }
            return acknowledgementContent;
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public async Task<ActionResult> DeleteApprenticeshipConfirmation(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
            {
                return View("AccessDenied");
            }

            var response = await Orchestrator.GetDeleteApprenticeshipViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public async Task<ActionResult> DeleteApprenticeshipConfirmation(DeleteApprenticeshipConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = await Orchestrator.GetDeleteApprenticeshipViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId, viewModel.HashedApprenticeshipId);

                return View(errorResponse);
            }

            if (viewModel.DeleteConfirmed.HasValue && viewModel.DeleteConfirmed.Value)
            {
                await Orchestrator.DeleteApprenticeship(viewModel, OwinWrapper.GetClaimValue(@"sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName), OwinWrapper.GetClaimValue(DasClaimTypes.Email));

                var flashMessage = new FlashMessageViewModel { Severity = FlashMessageSeverityLevel.Okay, Message = string.Format($"Apprentice record for {viewModel.ApprenticeshipName} deleted") };
                AddFlashMessageToCookie(flashMessage);
                
                return RedirectToAction("Details", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId });
            }

            return Redirect(Url.CommitmentsV2Link($"{viewModel.HashedAccountId}/unapproved/{viewModel.HashedCommitmentId}/apprentices/{viewModel.HashedApprenticeshipId}/edit"));
        }

        private string GetReturnUrl(RequestStatus status, string hashedAccountId)
        {
            switch (status)
            {
                case RequestStatus.NewRequest:
                    return Url.Action("Draft", new { hashedAccountId });
                case RequestStatus.ReadyForReview:
                case RequestStatus.ReadyForApproval:
                    return Url.Action("ReadyForReview", new { hashedAccountId });
                default:
                    return string.Empty;
            }
        }

        private string GetReturnToListUrl(string hashedAccountId)
        {
            switch (GetRequestStatusFromCookie())
            {
                case RequestStatus.WithProviderForApproval:
                case RequestStatus.SentForReview:
                    return Url.Action("WithProvider", new { hashedAccountId });
                case RequestStatus.NewRequest:
                    return Url.Action("Draft", new { hashedAccountId });
                case RequestStatus.ReadyForReview:
                case RequestStatus.ReadyForApproval:
                    return Url.Action("ReadyForReview", new { hashedAccountId });
                case RequestStatus.WithSenderForApproval:
                    return Url.Action("TransferFunded", new {hashedAccountId});
                case RequestStatus.RejectedBySender:
                    return Url.Action("RejectedTransfers", new {hashedAccountId});
                default:
                    return Url.Action("YourCohorts", new { hashedAccountId });
            }
        }
    }
}