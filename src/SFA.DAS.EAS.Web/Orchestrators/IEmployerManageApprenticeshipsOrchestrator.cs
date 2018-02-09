using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public interface IEmployerManageApprenticeshipsOrchestrator
    {
        Task<OrchestratorResponse<ManageApprenticeshipsViewModel>> GetApprenticeships(
            string hashedAccountId, ApprenticeshipFiltersViewModel filters, string externalUserId);

        Task<OrchestratorResponse<EditApprenticeshipStopDateViewModel>> GetApprenticeshipStopDateDetails(string hashedAccountId, string hashedApprenticeshipId, string externalUserId);
        Task<Dictionary<string, string>> ValidateApprenticeshipStopDate(string hashedAccountId, string hashedApprenticeshipId, EditStopDateViewModel updatedModel);
        Task<OrchestratorResponse<ApprenticeshipDetailsViewModel>> GetApprenticeship(string hashedAccountId, string hashedApprenticeshipId, string externalUserId);

        Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeshipForEdit(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId);

        Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetConfirmChangesModel(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId,
            ApprenticeshipViewModel apprenticeship);

        Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetViewChangesViewModel(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId);

        Task SubmitUndoApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId, string userName, string userEmail);
        Task<Dictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel apprenticeship, UpdateApprenticeshipViewModel updatedModel);
        Task<OrchestratorResponse<ChangeStatusChoiceViewModel>> GetChangeStatusChoiceNavigation(string hashedAccountId, string hashedApprenticeshipId, string externalUserId);

        Task<OrchestratorResponse<WhenToMakeChangeViewModel>> GetChangeStatusDateOfChangeViewModel(
            string hashedAccountId, string hashedApprenticeshipId,
            ChangeStatusType changeType, string externalUserId);

        Task<ValidateWhenToApplyChangeResult> ValidateWhenToApplyChange(string hashedAccountId,
            string hashedApprenticeshipId, ChangeStatusViewModel model);

        Task<OrchestratorResponse<ConfirmationStateChangeViewModel>> GetChangeStatusConfirmationViewModel(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange, string externalUserId);
        Task UpdateStatus(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusViewModel model, string externalUserId, string userName, string userEmail);
        Task UpdateStopDate(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusViewModel model, string externalUserId, string userName, string userEmail);
        Task CreateApprenticeshipUpdate(UpdateApprenticeshipViewModel apprenticeship, string hashedAccountId, string userId, string userName, string userEmail);

        Task<OrchestratorResponse<UpdateApprenticeshipViewModel>>
            GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(string hashedAccountId,
                string hashedApprenticeshipId);

        void CreateApprenticeshipViewModelCookie(UpdateApprenticeshipViewModel model);
        Task SubmitReviewApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId, bool isApproved, string userName, string userEmail);
        Task<OrchestratorResponse<PaymentOrderViewModel>> GetPaymentOrder(string hashedAccountId, string user);
        Task UpdatePaymentOrder(string hashedAccountId, IEnumerable<long> paymentItems, string user, string userName, string userEmail);

        Task<bool> AuthorizeRole(string hashedAccountId, string externalUserId, Role[] roles);
    }
}