using System;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IProviderEmailNotificationService
    {
        Task SendProviderTransferRejectedCommitmentEditNotification(CommitmentView commitment);
        Task SendCreateCommitmentNotification(CommitmentView commitment);
        Task SendProviderApprenticeshipStopNotification(Apprenticeship apprenticeship, DateTime stopDate);
        Task SendProviderApprenticeshipStopEditNotification(Apprenticeship apprenticeship, DateTime newStopDate);
        Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment,
            TransferApprovalStatus newTransferApprovalStatus);
        Task SendProviderApprenticeshipPauseNotification(Apprenticeship apprenticeship, DateTime pausedDate);
        Task SendProviderApprenticeshipResumeNotification(Apprenticeship apprenticeship, DateTime resumeDate);
    }
}
