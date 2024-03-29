﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using FluentValidation.Mvc;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {
        private readonly IEmployerManageApprenticeshipsOrchestrator _orchestrator;
        private readonly ILinkGenerator _linkGenerator;
        protected readonly ILog _logger;

        public EmployerManageApprenticesController(
            IEmployerManageApprenticeshipsOrchestrator orchestrator,
            IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            ILinkGenerator linkGenerator,
            ILog logger)            
                : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _linkGenerator = linkGenerator;
            _logger = logger;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult ListAll(string hashedAccountId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices"));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details", Name = "OnProgrammeApprenticeshipDetails")]
        [Deprecated]
        public ActionResult Details(string hashedAccountId, string hashedApprenticeshipId)
        {       
           return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details"));
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "ChangeStatusSelectOption")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public  ActionResult ChangeStatus(string hashedAccountId, string hashedApprenticeshipId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details/changestatus"));
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/editstopdate", Name = "EditStopDateOption")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public  ActionResult EditStopDate(string hashedAccountId, string hashedApprenticeshipId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details/editstopdate"));
        }
        
        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "WhenToApplyChange")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public async Task<ActionResult> WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _orchestrator.GetChangeStatusDateOfChangeViewModel(hashedAccountId, hashedApprenticeshipId, changeType, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Data.SkipToMadeRedundantQuestion)
                return RedirectToRoute("MadeRedundant", new
                {
                    changeType = response.Data.ChangeStatusViewModel.ChangeType.ToString().ToLower(),
                    whenToMakeChange = WhenToMakeChangeOptions.Immediately,
                    dateOfChange = default(DateTime?)
                });

            if (response.Data.SkipToConfirmationPage)
                return RedirectToRoute("StatusChangeConfirmation", new
                {
                    changeType = response.Data.ChangeStatusViewModel.ChangeType.ToString().ToLower(),
                    whenToMakeChange = WhenToMakeChangeOptions.Immediately,
                    dateOfChange = default(DateTime?)
                });

            return View(new OrchestratorResponse<WhenToMakeChangeViewModel>
            {
                Data = response.Data
            });
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "PostWhenToApplyChange")]
        [Deprecated]
        public async Task<ActionResult> WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId, [CustomizeValidator(RuleSet = "default,Date")] ChangeStatusViewModel model)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                var viewResponse = await _orchestrator.GetChangeStatusDateOfChangeViewModel(hashedAccountId, hashedApprenticeshipId, model.ChangeType.Value, OwinWrapper.GetClaimValue(@"sub"));

                return View(new OrchestratorResponse<WhenToMakeChangeViewModel>() { Data = viewResponse.Data });
            }

            var response = await _orchestrator.ValidateWhenToApplyChange(hashedAccountId, hashedApprenticeshipId, model);

            if (!response.ValidationResult.IsValid())
            {
                response.ValidationResult.AddToModelState(ModelState);

                var viewResponse = await _orchestrator.GetChangeStatusDateOfChangeViewModel(hashedAccountId, hashedApprenticeshipId, model.ChangeType.Value, OwinWrapper.GetClaimValue(@"sub"));

                return View(new OrchestratorResponse<WhenToMakeChangeViewModel>() { Data = viewResponse.Data });
            }

            return RedirectToRoute("MadeRedundant", new { whenToMakeChange = model.WhenToMakeChange, dateOfChange = response.DateOfChange });
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/maderedundant", Name = "MadeRedundant")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult HasApprenticeBeenMadeRedundant(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, DateTime? dateOfChange, WhenToMakeChangeOptions whenToMakeChange)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details/madeRedundant"));
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "StatusChangeConfirmation")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public  ActionResult StatusChangeConfirmation(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange, bool? madeRedundant)
        {
            switch (changeType)
            {
                case ChangeStatusType.Pause:
                    return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details/pause"));
                case ChangeStatusType.Resume:
                    return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details/resume"));
                case ChangeStatusType.Stop:
                    return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details/confirmStop"));
            }

            //We should never get here
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details"));
        }
        
        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprenticeship")]
        [Deprecated]
        public ActionResult Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{ hashedAccountId}/apprentices/{hashedApprenticeshipId}/ edit"));
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/edit/confirm"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/SubmitChanges")]
        [Deprecated]
        public async Task<ActionResult> SubmitChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var orginalApp = await _orchestrator.GetApprenticeship(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            apprenticeship.OriginalApprenticeship = orginalApp.Data;

            if (!ModelState.IsValid)
            {
                var viewmodel = await _orchestrator.GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(hashedAccountId, hashedApprenticeshipId);
                viewmodel.Data.AddErrorsFromModelState(ModelState);
                SetErrorMessage(viewmodel, viewmodel.Data.ErrorDictionary);
                return View("ConfirmChanges", viewmodel);
            }

            if (apprenticeship.ChangesConfirmed != null && !apprenticeship.ChangesConfirmed.Value)
            {
                return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
            }

            await _orchestrator.CreateApprenticeshipUpdate(apprenticeship, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName),
                    OwinWrapper.GetClaimValue(DasClaimTypes.Email));

            var message = NeedReapproval(apprenticeship)
                ? "Suggested changes sent to training provider for approval, where needed."
                : "Apprentice updated";

            var flashmessage = new FlashMessageViewModel
            {
                Message = message,
                Severity = FlashMessageSeverityLevel.Okay
            };

            AddFlashMessageToCookie(flashmessage);


            return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
        }


        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewChanges")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public  ActionResult ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/changes/view"));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/view")]
        [Deprecated]
        public async Task<ActionResult> ViewChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship)
        {
            if (!ModelState.IsValid)
            {
                var viewmodel = await _orchestrator
                    .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

                viewmodel.Data.AddErrorsFromModelState(ModelState);
                SetErrorMessage(viewmodel, viewmodel.Data.ErrorDictionary);
                viewmodel.Data.ApprenticeDetailsV2Link = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details");
                return View(viewmodel);
            }

            if (apprenticeship.ChangesConfirmed != null && apprenticeship.ChangesConfirmed.Value)
            {
                await _orchestrator.SubmitUndoApprenticeshipUpdate(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName),
                    OwinWrapper.GetClaimValue(DasClaimTypes.Email));
                SetOkayMessage("Changes undone");
            }

            return RedirectToAction("Details");
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/review", Name = "ReviewChanges")]
        [Deprecated]
        public ActionResult ReviewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return Redirect(_linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/changes/review"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/review")]
        [Deprecated]
        public async Task<ActionResult> ReviewChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewmodel = await _orchestrator
                    .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

                viewmodel.Data.AddErrorsFromModelState(ModelState);
                viewModel.ApprenticeDetailsV2Link = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details");
                SetErrorMessage(viewmodel, viewmodel.Data.ErrorDictionary);
                return View(viewmodel);
            }
            if (viewModel.ChangesConfirmed != null)
            {
                await _orchestrator.SubmitReviewApprenticeshipUpdate(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), viewModel.ChangesConfirmed.Value,
                    OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName), OwinWrapper.GetClaimValue(DasClaimTypes.Email));

                var message = viewModel.ChangesConfirmed.Value ? "Changes approved" : "Changes rejected";
                SetOkayMessage(message);
            }
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
                || data.EmployerRef != null;
        }

        private async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await _orchestrator
                .AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }

        private void SetOkayMessage(string message)
        {
            var flashmessage = new FlashMessageViewModel
            {
                Message = message,
                Severity = FlashMessageSeverityLevel.Okay
            };

            AddFlashMessageToCookie(flashmessage);
        }

        private void SetSuccessMessage(string message)
        {
            var flashmessage = new FlashMessageViewModel
            {
                Message = message,
                Severity = FlashMessageSeverityLevel.Success
            };

            AddFlashMessageToCookie(flashmessage);
        }

        private void SetErrorMessage(OrchestratorResponse orchestratorResponse, Dictionary<string, string> errorDictionary)
        {
            orchestratorResponse.FlashMessage = new FlashMessageViewModel
            {
                Headline = "There are errors on this page that need your attention",
                Message = "Check the following details:",
                ErrorMessages = errorDictionary,
                Severity = FlashMessageSeverityLevel.Error
            };
        }

        private void AddErrorMessageToModelState(Dictionary<string, string> errorDictionary)
        {
            if (errorDictionary == null) return;

            foreach (var keyValuePair in errorDictionary)
            {
                ModelState.AddModelError(keyValuePair.Key, keyValuePair.Value);
            }
        }

        private static string GetStatusMessage(ChangeStatusType? model)
        {
            if (model == null) return "";
            switch (model.Value)
            {
                case ChangeStatusType.Pause:
                    return "Apprenticeship paused";
                case ChangeStatusType.Stop:
                    return "Apprenticeship stopped";
                case ChangeStatusType.Resume:
                    return "Apprenticeship resumed";
                case ChangeStatusType.None:
                    return "Apprenticeship resumed";
            }
            return string.Empty;
        }

        private static string GenerateStopSurveyLink()
        {
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter())
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                {
                    var surveyLink = new HtmlAnchor
                    {
                        HRef = "https://www.smartsurvey.co.uk/s/stoppingapprentices/",
                        Target = "_blank",
                        InnerText = "Complete our survey"
                    };
                    surveyLink.Attributes.Add("id", "stop-survey-link");
                    surveyLink.Attributes.Add("rel", "noopener noreferrer");
                    surveyLink.Attributes.Add("aria-label", "Complete our survey");

                    surveyLink.RenderControl(htmlWriter);
                }
                result.Append(stringWriter);
            }
            return result.ToString();
        }

        private bool NeedReapproval(UpdateApprenticeshipViewModel model)
        {
            return
                   !string.IsNullOrEmpty(model.FirstName)
                || !string.IsNullOrEmpty(model.LastName)
                || model.DateOfBirth?.DateTime != null
                || !string.IsNullOrEmpty(model.TrainingCode)
                || model.StartDate?.DateTime != null
                || model.EndDate?.DateTime != null
                || !string.IsNullOrEmpty(model.Cost);
        }
    }
}