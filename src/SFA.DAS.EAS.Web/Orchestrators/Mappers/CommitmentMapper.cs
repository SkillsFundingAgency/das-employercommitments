using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public sealed class CommitmentMapper : ICommitmentMapper
    {
        private readonly IHashingService _hashingService;
        private readonly ICommitmentStatusCalculator _statusCalculator;
        private readonly IFeatureToggleService _featureToggleService;

        public CommitmentMapper(IHashingService hashingService, ICommitmentStatusCalculator statusCalculator,
            IFeatureToggleService featureToggleService)
        {
            _hashingService = hashingService;
            _statusCalculator = statusCalculator;
            _featureToggleService = featureToggleService;
        }

        public async Task<CommitmentListItemViewModel> MapToCommitmentListItemViewModelAsync(CommitmentListItem commitment, Func<CommitmentListItem, Task<string>> latestMessageFunc)
        {
            var messageTask = latestMessageFunc.Invoke(commitment);

            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = _statusCalculator.GetStatus(commitment.EditStatus, commitment.ApprenticeshipCount, commitment.LastAction, commitment.AgreementStatus),
                LatestMessage = await messageTask
            };
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
                HashedTransferSenderAccountId = _hashingService.HashValue(transferRequest.SendingEmployerAccountId),
                LegalEntityName = transferRequest.LegalEntityName,
                HashedCohortReference = _hashingService.HashValue(transferRequest.CommitmentId),
                TrainingList = transferRequest.TrainingList?.Select(MapTrainingCourse).ToList() ?? new List<TrainingCourseSummaryViewModel>(),
                TransferApprovalStatusDesc = ToApprovalStatusDescription(transferRequest.Status),
                TransferApprovalStatus = transferRequest.Status,
                TransferApprovalSetBy = transferRequest.ApprovedOrRejectedByUserName,
                TransferApprovalSetOn = transferRequest.ApprovedOrRejectedOn,
                TotalCost = transferRequest.TransferCost
            };
        }

        [Obsolete]
        public TransferCommitmentViewModel MapToTransferCommitmentViewModel(CommitmentView commitment)
        {

            var apprenticeships = commitment.Apprenticeships ?? new List<Apprenticeship>();

            var grouped = apprenticeships.GroupBy(l => l.TrainingCode).Select(cl =>
                new TrainingCourseSummaryViewModel
                {
                    CourseTitle = cl.First().TrainingName,
                    ApprenticeshipCount = cl.Count()
                });

            return new TransferCommitmentViewModel()
            {
                HashedTransferReceiverAccountId = _hashingService.HashValue(commitment.EmployerAccountId),
                HashedTransferSenderAccountId = _hashingService.HashValue(commitment.TransferSender.Id.Value),
                LegalEntityName = commitment.LegalEntityName,
                HashedCohortReference = _hashingService.HashValue(commitment.Id),
                TrainingList = grouped.ToList(),
                TransferApprovalStatusDesc = ToApprovalStatusDescription(commitment.TransferSender.TransferApprovalStatus),
                TransferApprovalStatus = commitment.TransferSender.TransferApprovalStatus,
                TransferApprovalSetBy = commitment.TransferSender.TransferApprovalSetBy,
                TransferApprovalSetOn = commitment.TransferSender.TransferApprovalSetOn,
                TotalCost = apprenticeships.Sum(x => x.Cost) ?? 0,
                EnableRejection = _featureToggleService.Get<TransfersRejectOption>().FeatureEnabled
            };
        }


        private TrainingCourseSummaryViewModel MapTrainingCourse(TrainingCourseSummary source)
        {
            return new TrainingCourseSummaryViewModel
            {
                CourseTitle = source.CourseTitle,
                ApprenticeshipCount = source.ApprenticeshipCount
            };
        }

        private string ToApprovalStatusDescription(TransferApprovalStatus commitmentTransferApprovalStatus)
        {
            switch (commitmentTransferApprovalStatus)
            {
                case TransferApprovalStatus.Approved:
                    return "Approved";
                case TransferApprovalStatus.Rejected:
                    return "Rejected";
                case TransferApprovalStatus.Pending:
                    return "Pending";
                default:
                    return "Unknown";
            }
        }
    }
}