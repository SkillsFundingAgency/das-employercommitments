using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IProviderEmailNotificationService
    {
        Task SendProviderTransferRejectedCommitmentEditNotification(CommitmentView commitment);

        Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment,
            TransferApprovalStatus newTransferApprovalStatus, Dictionary<string, string> tokens);
    }
}
