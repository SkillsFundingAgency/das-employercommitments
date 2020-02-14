using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public interface IEmployerCommitmentsOrchestrator
    {
        Task<OrchestratorResponse<CommitmentInformViewModel>> GetInform(string hashedAccountId, string externalUserId);

        Task<OrchestratorResponse<SelectLegalEntityViewModel>> GetLegalEntities(string hashedAccountId,
            string transferConnectionCode, string cohortRef, string externalUserId);

        Task<OrchestratorResponse<SelectTransferConnectionViewModel>> GetTransferConnections(string hashedAccountId,
            string externalUserId);

        Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId, string externalUserId,
            SelectProviderViewModel model);

        Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId,
            string externalUserId, ConfirmProviderViewModel model);

        Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(string hashedAccountId,
            string transferConnectionCode, string legalEntityCode, string providerId, string cohortRef,
            string externalUserId);

        Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetSkeletonApprenticeshipDetails(
            string hashedAccountId, string externalUserId, string hashedCommitmentId);

        Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeship(string hashedAccountId,
            string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId);

        Task<OrchestratorResponse<ApprenticeshipViewModel>> GetApprenticeshipViewModel(string hashedAccountId,
            string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId);

        Task<OrchestratorResponse<FinishEditingViewModel>> GetFinishEditingViewModel(string hashedAccountId,
            string externalUserId, string hashedCommitmentId);

        Task ApproveCommitment(string hashedAccountId, string externalUserId, string userDisplayName, string userEmail,
            string hashedCommitmentId, SaveStatus saveStatus);

        Task SetTransferRequestApprovalStatus(string hashedAccountId, string hashedCommitmentId,
            string hashedTransferRequestId, TransferApprovalConfirmationViewModel model, string externalUserId,
            string userDisplayName, string userEmail);

        Task<OrchestratorResponse<SubmitCommitmentViewModel>> GetSubmitCommitmentModel(string hashedAccountId,
            string externalUserId, string hashedCommitmentId, SaveStatus saveStatus);

        Task SubmitCommitment(SubmitCommitmentViewModel model, string externalUserId, string userDisplayName,
            string userEmail);

        Task<OrchestratorResponse<AcknowledgementViewModel>> GetAcknowledgementModelForExistingCommitment(
            string hashedAccountId, string hashedCommitmentId, string externalUserId);

        Task<OrchestratorResponse<YourCohortsViewModel>> GetYourCohorts(string hashedAccountId, string externalUserId);

        Task<OrchestratorResponse<CommitmentListViewModel>> GetAllDraft(string hashedAccountId, string externalUserId);

        Task<OrchestratorResponse<CommitmentListViewModel>> GetAllReadyForReview(string hashedAccountId,
            string externalUserId);

        Task<OrchestratorResponse<CommitmentListViewModel>> GetAllWithProvider(string hashedAccountId,
            string externalUserId);

        Task<OrchestratorResponse<TransferFundedCohortsViewModel>> GetAllTransferFunded(string hashedAccountId,
            string externalUserId);

        Task<OrchestratorResponse<TransferFundedCohortsViewModel>> GetAllRejectedTransferFunded(string hashedAccountId,
            string externalUserId);

        Task<OrchestratorResponse<CommitmentDetailsViewModel>> GetCommitmentDetails(string hashedAccountId,
            string hashedCommitmentId, string externalUserId);

        Task<OrchestratorResponse<DeleteCommitmentViewModel>> GetDeleteCommitmentModel(string hashedAccountId,
            string hashedCommitmentId, string externalUserId);

        Task DeleteCommitment(string hashedAccountId, string hashedCommitmentId, string externalUserId, string userName,
            string userEmail);

        Task<OrchestratorResponse<DeleteApprenticeshipConfirmationViewModel>> GetDeleteApprenticeshipViewModel(
            string hashedAccountId, string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId);

        Task<bool> AnyCohortsForCurrentStatus(string hashedAccountId, params RequestStatus[] requestStatusFromSession);

        Task<OrchestratorResponse<LegalEntitySignedAgreementViewModel>> GetLegalEntitySignedAgreementViewModel(
            string hashedAccountId, string transferConnectionCode, string legalEntityCode, string cohortRef,
            string userId);

        Task<Dictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel apprenticeship);

        Task DeleteApprenticeship(DeleteApprenticeshipConfirmationViewModel model, string externalUser, string userName,
            string userEmail);

        Task<OrchestratorResponse<TransferRequestViewModel>> GetTransferRequestDetails(
            string hashedTransferAccountId, Application.Queries.GetTransferRequest.CallerType callerType,
            string hashedTransferRequestId, string externalUserId);

        //todo: probably belongs in a base inerface
        Task<bool> AuthorizeRole(string hashedAccountId, string externalUserId, Role[] roles);
        Task<CommitmentsV2.Types.ApprenticeshipEmployerType> GetApprenticeshipEmployerType(string hashedAccountId);
    }
}
