using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Mvc;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedaccountId}/apprentices")]
    public class EmployerCommitmentsController : BaseEmployerController
    {

        public EmployerCommitmentsController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage, ICookieStorageService<string> lastCohortCookieStorageService)
            : base(employerCommitmentsOrchestrator, owinWrapper, multiVariantTestingService, flashMessage, lastCohortCookieStorageService)
        {
        }

        [HttpGet]
        [Route("home", Name = "CommitmentsHome")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;

            var response = await EmployerCommitmentsOrchestrator.GetIndexViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            SetFlashMessageOnModel(response);

            return View(response);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts")]
        public async Task<ActionResult> YourCohorts(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetYourCohorts(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

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

            var model = await EmployerCommitmentsOrchestrator.GetAllDraft(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
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

            var model = await EmployerCommitmentsOrchestrator.GetAllReadyForReview(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
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

            SaveRequestStatusInCookie(RequestStatus.WithProviderForApproval);

            var model = await EmployerCommitmentsOrchestrator.GetAllWithProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/transferFunded")]
        public async Task<ActionResult> TransferFunded(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            //todo: the pattern seems to be pick one of the statuses associated with a bingo box and save that in the cookie
            // to represent e.g. which page to go back to after delete. we could refactor this, perhaps introduce a new enum.
            // also, subsequent transfer stories will need to check for this status when they GetRequestStatusFromCookie()
            SaveRequestStatusInCookie(RequestStatus.WithSenderForApproval);

            var model = await EmployerCommitmentsOrchestrator.GetAllTransferFunded(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View("TransferFundedCohorts", model);
        }

        [HttpGet]
        [Route("Inform")]
        public async Task<ActionResult> Inform(string hashedAccountId)
        {
            SaveRequestStatusInCookie(RequestStatus.None);
            var response = await EmployerCommitmentsOrchestrator.GetInform(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("transferConnection/create")]
        public async Task<ActionResult> SelectTransferConnection(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await EmployerCommitmentsOrchestrator
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
                var response = await EmployerCommitmentsOrchestrator.GetTransferConnections(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
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

            var response = await EmployerCommitmentsOrchestrator
                .GetLegalEntities(hashedAccountId, transferConnectionCode, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Data.LegalEntities == null || !response.Data.LegalEntities.Any())
                throw new InvalidStateException($"No legal entities associated with account {hashedAccountId}");

            var availableLegalEntities = response.Data.LegalEntities.Where(le => le.Agreements != null
                && le.Agreements.Any(a => a.Status == EmployerAgreementStatus.Pending || a.Status == EmployerAgreementStatus.Signed));

            if (availableLegalEntities.Count() == 1)
            {
                var autoSelectLegalEntity = availableLegalEntities.First();

                var hasSigned = EmployerCommitmentsOrchestrator.HasSignedAgreement(
                    autoSelectLegalEntity, !string.IsNullOrWhiteSpace(transferConnectionCode));

                if (hasSigned)
                {
                    return RedirectToAction("SearchProvider", new SelectLegalEntityViewModel
                    {
                        TransferConnectionCode = response.Data.TransferConnectionCode,
                        CohortRef = response.Data.CohortRef,
                        LegalEntityCode = autoSelectLegalEntity.Code,
                        // no need to store LegalEntities, as the property is only read in the SelectLegalEntity view, which we're now skipping
                    });
                }
                else
                {
                    var agreement = await EmployerCommitmentsOrchestrator.GetLegalEntitySignedAgreementViewModel(hashedAccountId,
                        response.Data.TransferConnectionCode, autoSelectLegalEntity.Code, response.Data.CohortRef, OwinWrapper.GetClaimValue(@"sub"));

                    return RedirectToAction("AgreementNotSigned", agreement.Data);
                }
            }

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("legalEntity/create")]
        public async Task<ActionResult> SetLegalEntity(string hashedAccountId, SelectLegalEntityViewModel selectedLegalEntity)
        {
            if (!ModelState.IsValid)
            {
                var response = await EmployerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId,
                    selectedLegalEntity.TransferConnectionCode, selectedLegalEntity.CohortRef,
                    OwinWrapper.GetClaimValue(@"sub"));

                return View("SelectLegalEntity", response);
            }

            var agreement = await EmployerCommitmentsOrchestrator.GetLegalEntitySignedAgreementViewModel(hashedAccountId,
                selectedLegalEntity.TransferConnectionCode, selectedLegalEntity.LegalEntityCode, selectedLegalEntity.CohortRef, OwinWrapper.GetClaimValue(@"sub"));

            if (agreement.Data.HasSignedAgreement)
            {
                return RedirectToAction("SearchProvider", selectedLegalEntity);
            }
            else
            {
                return RedirectToAction("AgreementNotSigned", agreement.Data);
            }
        }

        [HttpGet]
        [Route("provider/create")]
        public async Task<ActionResult> SearchProvider(string hashedAccountId, string transferConnectionCode, string legalEntityCode, string cohortRef)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (string.IsNullOrWhiteSpace(legalEntityCode) || string.IsNullOrWhiteSpace(cohortRef))
            {
                return RedirectToAction("Inform", new {hashedAccountId});
            }

            var response = await EmployerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), transferConnectionCode, legalEntityCode, cohortRef);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("provider/create")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, [System.Web.Http.FromUri] [CustomizeValidator(RuleSet = "Request")] SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var defaultViewModel = await EmployerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.TransferConnectionCode, viewModel.LegalEntityCode, viewModel.CohortRef);
                
                return View("SearchProvider", defaultViewModel);
            }

            var response = await EmployerCommitmentsOrchestrator.GetProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel);

            if (response.Data.Provider == null)
            {
                var defaultViewModel = await EmployerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.TransferConnectionCode, viewModel.LegalEntityCode, viewModel.CohortRef);
                defaultViewModel.Data.NotFound = true;

                RevalidateModel(defaultViewModel);

                return View("SearchProvider", defaultViewModel);
            }

            return View(response);
        }

        private void RevalidateModel(OrchestratorResponse<SelectProviderViewModel> defaultViewModel)
        {
            var validator = new SelectProviderViewModelValidator();
            var results = validator.Validate(defaultViewModel.Data, ruleSet: "SearchResult");
            
            results.AddToModelState(ModelState, null);
        }

        [HttpGet]
        [Route("confirmProvider/create")]
        public async Task<ActionResult> ConfirmProvider(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            return RedirectToAction("Inform", new { hashedAccountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirmProvider/create")]
        public async Task<ActionResult> ConfirmProvider(string hashedAccountId,
            [System.Web.Http.FromUri] ConfirmProviderViewModel viewModelModel)
        {
            if (!ModelState.IsValid)
            {
                if (viewModelModel.Confirmation == null)
                {
                    var response = await EmployerCommitmentsOrchestrator.GetProvider(hashedAccountId,
                        OwinWrapper.GetClaimValue(@"sub"), viewModelModel);

                    return View("SelectProvider", response);
                }
            }

            if (!viewModelModel.Confirmation.Value)
            {
                return RedirectToAction("SearchProvider",
                    new SelectProviderViewModel
                    {
                        TransferConnectionCode = viewModelModel.TransferConnectionCode,
                        LegalEntityCode = viewModelModel.LegalEntityCode,
                        CohortRef = viewModelModel.CohortRef
                    });
            }

            return RedirectToAction("ChoosePath",
                new
                {
                    hashedAccountId = hashedAccountId,
                    transferConnectionCode = viewModelModel.TransferConnectionCode,
                    legalEntityCode = viewModelModel.LegalEntityCode,
                    providerId = viewModelModel.ProviderId,
                    cohortRef = viewModelModel.CohortRef
                });
        }

        [HttpGet]
        [Route("choosePath/create")]
        public async Task<ActionResult> ChoosePath(string hashedAccountId, string transferConnectionCode, string legalEntityCode, string providerId, string cohortRef)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (string.IsNullOrWhiteSpace(legalEntityCode)
                || string.IsNullOrWhiteSpace(providerId)
                || string.IsNullOrWhiteSpace(cohortRef))
            {
                return RedirectToAction("Inform", new { hashedAccountId });
            }

            var model = await EmployerCommitmentsOrchestrator.CreateSummary(hashedAccountId, transferConnectionCode, legalEntityCode, providerId, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("choosePath/create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await EmployerCommitmentsOrchestrator.CreateSummary(viewModel.HashedAccountId,
                    viewModel.TransferConnectionCode, viewModel.LegalEntityCode, viewModel.ProviderId.ToString(),
                    viewModel.CohortRef, OwinWrapper.GetClaimValue(@"sub"));

                return View("ChoosePath", model);
            }

            if (viewModel.SelectedRoute == "employer")
            {
                var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
                var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
                var userId = OwinWrapper.GetClaimValue(@"sub");

                var response =
                    await EmployerCommitmentsOrchestrator.CreateEmployerAssignedCommitment(viewModel, userId,
                        userDisplayName, userEmail);

                return RedirectToAction("Details", new {hashedCommitmentId = response.Data});
            }

            return RedirectToAction("SubmitNewCommitment",
                new
                {
                    hashedAccountId = viewModel.HashedAccountId,
                    transferConnectionCode = viewModel.TransferConnectionCode,
                    legalEntityCode = viewModel.LegalEntityCode,
                    legalEntityName = viewModel.LegalEntityName,
                    legalEntityAddress = viewModel.LegalEntityAddress,
                    legalEntitySource = viewModel.LegalEntitySource,
                    providerId = viewModel.ProviderId,
                    providerName = viewModel.ProviderName,
                    cohortRef = viewModel.CohortRef,
                    saveStatus = SaveStatus.Save
                });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetCommitmentDetails(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));
            
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

            var model = await EmployerCommitmentsOrchestrator.GetDeleteCommitmentModel(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

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
                var model = await EmployerCommitmentsOrchestrator
                    .GetDeleteCommitmentModel(viewModel.HashedAccountId, viewModel.HashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

                return View(model);
            }

            if (viewModel.DeleteConfirmed == null || !viewModel.DeleteConfirmed.Value)
            {
                return RedirectToAction("Details", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId } );
            }

            await EmployerCommitmentsOrchestrator
                .DeleteCommitment(viewModel.HashedAccountId, viewModel.HashedCommitmentId, OwinWrapper.GetClaimValue("sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName), OwinWrapper.GetClaimValue(DasClaimTypes.Email));

            var flashmessage = new FlashMessageViewModel
            {
                Message = "Cohort deleted",
                Severity = FlashMessageSeverityLevel.Okay
            };

            AddFlashMessageToCookie(flashmessage);
            
            var anyCohortWithCurrentStatus = 
                await EmployerCommitmentsOrchestrator.AnyCohortsForCurrentStatus(viewModel.HashedAccountId, GetRequestStatusFromCookie());

            if(!anyCohortWithCurrentStatus)
                return RedirectToAction("YourCohorts", new { viewModel.HashedAccountId });

            return Redirect(GetReturnToListUrl(viewModel.HashedAccountId));
        }

        [HttpGet]
        [Route("{legalEntityCode}/AgreementNotSigned")]
        public ActionResult AgreementNotSigned(LegalEntitySignedAgreementViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/create")]
        public async Task<ActionResult> CreateApprenticeshipEntry(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await EmployerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/apprenticeships/create")]
        public async Task<ActionResult> CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            if (!ModelState.IsValid)
            {
                apprenticeship.AddErrorsFromModelState(ModelState);
            }

            var validatorResult = await EmployerCommitmentsOrchestrator.ValidateApprenticeship(apprenticeship);
            if (validatorResult.Any())
            {
                apprenticeship.AddErrorsFromDictionary(validatorResult);
            }

            if (apprenticeship.ErrorDictionary.Any())
            {
                return await RedisplayCreateApprenticeshipView(apprenticeship);
            }

            try
            {
                await EmployerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship, OwinWrapper.GetClaimValue(@"sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName),
                    OwinWrapper.GetClaimValue(DasClaimTypes.Email));
            }
            catch (InvalidRequestException ex)
            {
                apprenticeship.AddErrorsFromDictionary(ex.ErrorMessages);
                return await RedisplayCreateApprenticeshipView(apprenticeship);
            }

            if (apprenticeship.IsInTransferRejectedCohort)
            {
                AddFlashMessageToCookie(new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Message = "You have successfully edited your cohort.  This will now be available within your Drafts."
                });
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/edit")]
        public async Task<ActionResult> EditApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetApprenticeship(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);
            AddErrorsToModelState(model.Data.ValidationErrors);
            return View("EditApprenticeshipEntry", model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/view")]
        public async Task<ActionResult> ViewApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await EmployerCommitmentsOrchestrator.GetApprenticeshipViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);
            return View("ViewApprenticeshipEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/apprenticeships/{HashedApprenticeshipId}/edit")]
        public async Task<ActionResult> EditApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            if (!ModelState.IsValid)
            {
                apprenticeship.AddErrorsFromModelState(ModelState);
            }

            var validatorResult = await EmployerCommitmentsOrchestrator.ValidateApprenticeship(apprenticeship);
            if (validatorResult.Any())
            {
                apprenticeship.AddErrorsFromDictionary(validatorResult);
            }

            if (apprenticeship.ErrorDictionary.Any())
            {
                return await RedisplayEditApprenticeshipView(apprenticeship);
            }

            try
            {
                await EmployerCommitmentsOrchestrator.UpdateApprenticeship(apprenticeship, OwinWrapper.GetClaimValue(@"sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName), OwinWrapper.GetClaimValue(DasClaimTypes.Email));
            }
            catch (InvalidRequestException ex)
            {
                apprenticeship.AddErrorsFromDictionary(ex.ErrorMessages);
                return await RedisplayEditApprenticeshipView(apprenticeship);
            }

            if (apprenticeship.IsInTransferRejectedCohort)
            {
                AddFlashMessageToCookie(new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Message = "You have successfully edited your cohort.  This will now be available within your Drafts."
                });
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/finished")]
        public async Task<ActionResult> FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await EmployerCommitmentsOrchestrator.GetFinishEditingViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/finished")]
        public async Task<ActionResult> FinishedEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var response = await EmployerCommitmentsOrchestrator.GetFinishEditingViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId);
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
                await EmployerCommitmentsOrchestrator.ApproveCommitment(viewModel.HashedAccountId, userId, userDisplayName, userEmail, viewModel.HashedCommitmentId, viewModel.SaveStatus);

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

            var model = await EmployerCommitmentsOrchestrator.GetAcknowledgementModelForExistingCommitment(
                hashedAccountId,
                hashedCommitmentId,
                OwinWrapper.GetClaimValue(@"sub"));

            var currentStatusCohortAny = await EmployerCommitmentsOrchestrator
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

            var response = await EmployerCommitmentsOrchestrator.GetSubmitCommitmentModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, saveStatus);
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

            await EmployerCommitmentsOrchestrator.SubmitCommitment(model, userId, userDisplayName, userEmail);

            return RedirectToAction("AcknowledgementExisting", new { hashedCommitmentId = model.HashedCommitmentId, model.SaveStatus });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("Submit")]
        public async Task<ActionResult> SubmitNewCommitment(string hashedAccountId, string transferConnectionCode, string legalEntityCode, string legalEntityName, string legalEntityAddress, short legalEntitySource, string providerId, string providerName, string cohortRef, SaveStatus? saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (string.IsNullOrWhiteSpace(legalEntityCode)
                || string.IsNullOrWhiteSpace(legalEntityName)
                || string.IsNullOrWhiteSpace(providerId)
                || string.IsNullOrWhiteSpace(providerName)
                || string.IsNullOrWhiteSpace(cohortRef)
                || string.IsNullOrWhiteSpace(legalEntityAddress)
                || !saveStatus.HasValue)
            {
                return RedirectToAction("Inform", new { hashedAccountId });
            }

            var response = await EmployerCommitmentsOrchestrator.GetSubmitNewCommitmentModel
                (hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), transferConnectionCode, legalEntityCode, legalEntityName, legalEntityAddress, legalEntitySource, providerId, providerName, cohortRef, saveStatus.Value);

            return View("SubmitCommitmentEntry", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("submit")]
        public async Task<ActionResult> SubmitNewCommitmentEntry(SubmitCommitmentViewModel model)
        {
            var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
            var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
            var userId = OwinWrapper.GetClaimValue(@"sub");

            var response = await EmployerCommitmentsOrchestrator.CreateProviderAssignedCommitment(model, userId, userDisplayName, userEmail);

            return RedirectToAction("AcknowledgementNew", new { hashedAccountId = model.HashedAccountId, hashedCommitmentId = response.Data });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/NewCohortAcknowledgement")]
        public async Task<ActionResult> AcknowledgementNew(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await EmployerCommitmentsOrchestrator
                .GetAcknowledgementModelForExistingCommitment(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            response.Data.Content = GetAcknowledgementContent(SaveStatus.Save, response.Data.IsTransfer);

            return View("Acknowledgement", response);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await EmployerCommitmentsOrchestrator
                .GetAcknowledgementModelForExistingCommitment(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            var status = GetRequestStatusFromCookie();
            bool anyCohortsLeft;
            if (status == RequestStatus.ReadyForReview)
            {
                anyCohortsLeft = await EmployerCommitmentsOrchestrator.AnyCohortsForCurrentStatus(hashedAccountId, RequestStatus.ReadyForApproval, status);
            }
            else
            {
                anyCohortsLeft= await EmployerCommitmentsOrchestrator.AnyCohortsForCurrentStatus(hashedAccountId, status);
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

            var response = await EmployerCommitmentsOrchestrator.GetDeleteApprenticeshipViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public async Task<ActionResult> DeleteApprenticeshipConfirmation(DeleteApprenticeshipConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = await EmployerCommitmentsOrchestrator.GetDeleteApprenticeshipViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId, viewModel.HashedApprenticeshipId);

                return View(errorResponse);
            }

            if (viewModel.DeleteConfirmed.HasValue && viewModel.DeleteConfirmed.Value)
            {
                await EmployerCommitmentsOrchestrator.DeleteApprenticeship(viewModel, OwinWrapper.GetClaimValue(@"sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName), OwinWrapper.GetClaimValue(DasClaimTypes.Email));

                var flashMessage = new FlashMessageViewModel { Severity = FlashMessageSeverityLevel.Okay, Message = string.Format($"Apprentice record for {viewModel.ApprenticeshipName} deleted") };
                AddFlashMessageToCookie(flashMessage);
                
                return RedirectToAction("Details", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId });
            }

            return RedirectToAction("EditApprenticeship", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId, viewModel.HashedApprenticeshipId });
        }

        private async Task<ActionResult> RedisplayCreateApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var response = await EmployerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship.HashedCommitmentId);
            response.Data.Apprenticeship = apprenticeship;

            if (response.Data.Apprenticeship.ErrorDictionary.Any())
            {
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "There are errors on this page that need your attention",
                    Message = "Check the following details:",
                    ErrorMessages = apprenticeship.ErrorDictionary,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }

            return View("CreateApprenticeshipEntry", response);
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var response = await EmployerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship.HashedCommitmentId);
            response.Data.Apprenticeship = apprenticeship;

            if (response.Data.Apprenticeship.ErrorDictionary.Any())
            {
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "There are errors on this page that need your attention",
                    Message = "Check the following details:",
                    ErrorMessages = apprenticeship.ErrorDictionary,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }

            return View("EditApprenticeshipEntry", response);
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
                default:
                    return Url.Action("YourCohorts", new { hashedAccountId });
            }
        }

    }
}