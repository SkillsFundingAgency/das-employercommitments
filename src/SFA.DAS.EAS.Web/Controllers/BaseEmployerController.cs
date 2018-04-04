using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{

    public abstract class BaseEmployerController : BaseController
    {
        protected readonly EmployerCommitmentsOrchestrator EmployerCommitmentsOrchestrator;

        private const string LastCohortPageCookieKey = "sfa-das-employerapprenticeshipsservice-lastCohortPage";
        private readonly ICookieStorageService<string> _lastCohortCookieStorageService;

        protected BaseEmployerController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper,
            IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ICookieStorageService<string> lastCohortCookieStorageService)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            EmployerCommitmentsOrchestrator = employerCommitmentsOrchestrator;
            _lastCohortCookieStorageService = lastCohortCookieStorageService;
        }

        protected RequestStatus GetRequestStatusFromCookie()
        {
            var status = _lastCohortCookieStorageService.Get(LastCohortPageCookieKey);

            if (string.IsNullOrWhiteSpace(status))
            {
                return RequestStatus.None;
            }

            return (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
        }

        protected void SaveRequestStatusInCookie(RequestStatus status)
        {
            _lastCohortCookieStorageService.Delete(LastCohortPageCookieKey);
            _lastCohortCookieStorageService.Create(status.ToString(), LastCohortPageCookieKey);
        }

        protected void AddErrorsToModelState(Dictionary<string, string> dict)
        {
            foreach (var error in dict)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        protected void SetFlashMessageOnModel<T>(OrchestratorResponse<T> model)
        {
            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage!=null)
            {
                model.FlashMessage = flashMessage;
            }
        }

        protected async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await EmployerCommitmentsOrchestrator.AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }

        protected void SetErrorMessage(OrchestratorResponse orchestratorResponse, Dictionary<string, string> errorDictionary)
        {
            orchestratorResponse.FlashMessage = new FlashMessageViewModel
            {
                Headline = "There are errors on this page that need your attention",
                Message = "Check the following details:",
                ErrorMessages = errorDictionary,
                Severity = FlashMessageSeverityLevel.Error
            };
        }
    }
}