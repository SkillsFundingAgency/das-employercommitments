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
        Task SendProviderApprenticeshipStopNotification(Apprenticeship apprenticeship);
        Task SendProviderApprenticeshipStopEditNotification(Apprenticeship apprenticeship, DateTime newStopDate);
        Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment,
            TransferApprovalStatus newTransferApprovalStatus);
    }
}
