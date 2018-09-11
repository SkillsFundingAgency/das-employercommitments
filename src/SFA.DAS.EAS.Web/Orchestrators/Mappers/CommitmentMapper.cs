using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public sealed class CommitmentMapper : ICommitmentMapper
    {
        private readonly IHashingService _hashingService;
        private readonly IFeatureToggleService _featureToggleService;
        private readonly IPublicHashingService _publicHashingService;

        public CommitmentMapper(IHashingService hashingService, IFeatureToggleService featureToggleService, IPublicHashingService publicHashingService)
        {
            _hashingService = hashingService;
            _featureToggleService = featureToggleService;
            _publicHashingService = publicHashingService;
        }

        public CommitmentViewModel MapToCommitmentViewModel(CommitmentView commitment)
        {
            return new CommitmentViewModel
            {
                HashedId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName
            };
        }

        public TransferRequestViewModel MapToTransferRequestViewModel(TransferRequest transferRequest)
        {
            return new TransferRequestViewModel()
            {
                HashedTransferReceiverAccountId = _hashingService.HashValue(transferRequest.ReceivingEmployerAccountId),
                PublicHashedTransferReceiverAccountId = _publicHashingService.HashValue(transferRequest.ReceivingEmployerAccountId),
                HashedTransferSenderAccountId = _hashingService.HashValue(transferRequest.SendingEmployerAccountId),
                TransferSenderName = transferRequest.TransferSenderName,
                PublicHashedTransferSenderAccountId = _publicHashingService.HashValue(transferRequest.SendingEmployerAccountId),
                LegalEntityName = transferRequest.LegalEntityName,
                HashedCohortReference = _hashingService.HashValue(transferRequest.CommitmentId),
                TrainingList = transferRequest.TrainingList?.Select(MapTrainingCourse).ToList() ?? new List<TrainingCourseSummaryViewModel>(),
                TransferApprovalStatusDesc = transferRequest.Status.ToString(),
                TransferApprovalStatus = transferRequest.Status,
                TransferApprovalSetBy = transferRequest.ApprovedOrRejectedByUserName,
                TransferApprovalSetOn = transferRequest.ApprovedOrRejectedOn,
                TotalCost = transferRequest.TransferCost,
                FundingCap = transferRequest.FundingCap,
                EnableRejection = _featureToggleService.Get<TransfersRejectOption>().FeatureEnabled,
                ShowFundingCapWarning = transferRequest.Status == TransferApprovalStatus.Pending
                                         && transferRequest.TransferCost < transferRequest.FundingCap
            };
        }

        public IEnumerable<TransferConnectionViewModel> MapToTransferConnectionsViewModel(List<TransferConnection> transferConnections)
        {
            return transferConnections.Select(x =>
                new TransferConnectionViewModel
                {
                    TransferConnectionCode = _publicHashingService.HashValue(x.AccountId),
                    TransferConnectionName = x.AccountName
                });
        }

        private TrainingCourseSummaryViewModel MapTrainingCourse(TrainingCourseSummary source)
        {
            return new TrainingCourseSummaryViewModel
            {
                CourseTitle = source.CourseTitle,
                ApprenticeshipCount = source.ApprenticeshipCount
            };
        }
    }
}