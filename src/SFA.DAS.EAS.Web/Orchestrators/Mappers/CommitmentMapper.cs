using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Extensions;
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

        public async Task<CommitmentListItemViewModel> MapToCommitmentListItemViewModelAsync(CommitmentListItem commitment, Func<CommitmentListItem, Task<string>> latestMessageFunc)
        {
            var messageTask = latestMessageFunc.Invoke(commitment);

            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = commitment.GetStatus(),
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
                PublicHashedTransferReceiverAccountId = _publicHashingService.HashValue(transferRequest.ReceivingEmployerAccountId),
                HashedTransferSenderAccountId = _hashingService.HashValue(transferRequest.SendingEmployerAccountId),
                PublicHashedTransferSenderAccountId = _publicHashingService.HashValue(transferRequest.SendingEmployerAccountId),
                LegalEntityName = transferRequest.LegalEntityName,
                HashedCohortReference = _hashingService.HashValue(transferRequest.CommitmentId),
                TrainingList = transferRequest.TrainingList?.Select(MapTrainingCourse).ToList() ?? new List<TrainingCourseSummaryViewModel>(),
                TransferApprovalStatusDesc = transferRequest.Status.ToString(),
                TransferApprovalStatus = transferRequest.Status,
                TransferApprovalSetBy = transferRequest.ApprovedOrRejectedByUserName,
                TransferApprovalSetOn = transferRequest.ApprovedOrRejectedOn,
                TotalCost = transferRequest.TransferCost,
                EnableRejection = _featureToggleService.Get<TransfersRejectOption>().FeatureEnabled
            };
        }

        public List<TransferConnectionViewModel> MapToTransferConnectionsViewModel(List<TransferConnection> transferConnections)
        {
            return transferConnections.Select(x =>
                new TransferConnectionViewModel
                {
                    TransferConnectionCode = _publicHashingService.HashValue(x.AccountId),
                    TransferConnectionName = x.AccountName
                }).ToList();
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
                PublicHashedTransferReceiverAccountId = _publicHashingService.HashValue(commitment.EmployerAccountId),
                HashedTransferSenderAccountId = _hashingService.HashValue(commitment.TransferSender.Id.Value),
                PublicHashedTransferSenderAccountId = _publicHashingService.HashValue(commitment.TransferSender.Id.Value),
                LegalEntityName = commitment.LegalEntityName,
                HashedCohortReference = _hashingService.HashValue(commitment.Id),
                TrainingList = grouped.ToList(),
                TransferApprovalStatusDesc = commitment.TransferSender.TransferApprovalStatus.ToString(),
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

    }
}