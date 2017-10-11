using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    [Authorize]
    [CommitmentsRoutePrefix("accounts/{hashedAccountId}/apprentices/manage/{hashedApprenticeshipId}/datalock")]
    public class DataLockController : BaseController
    {
        private readonly DataLockOrchestrator _orchestrator;

        public DataLockController(
            IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            DataLockOrchestrator orchestrator
            ) : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("restart", Name = "RequestRestart")]
        public async Task<ActionResult> RequestRestart(string hashedAccountId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetDataLockStatusForRestartRequest(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [Route("changes", Name = "RequestChanges")]
        public async Task<ActionResult> RequestChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetDataLockChangeStatus(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [Route("confirmchanges", Name = "ConfirmRequestChanges")]
        public async Task<ActionResult> ConfirmRequestChanges(DataLockStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AddErrorsFromModelState(ModelState);
                var viewModel = await _orchestrator.GetDataLockChangeStatus(model.HashedAccountId, model.HashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
                viewModel.Data.ErrorDictionary = model.ErrorDictionary;
                return View("RequestChanges", "EmployerManageApprenticesController", viewModel);
            }

            await _orchestrator.ConfirmRequestChanges(model.HashedAccountId, model.HashedApprenticeshipId, OwinWrapper.GetClaimValue("sub"), model.ChangesConfirmed ?? false);

            return RedirectToAction("Details", "EmployerManageApprentices", new { model.HashedAccountId, model.HashedApprenticeshipId });
        }
    }
}