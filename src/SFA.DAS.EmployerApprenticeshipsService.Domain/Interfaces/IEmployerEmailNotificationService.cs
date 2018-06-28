using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IEmployerEmailNotificationService
    {
        Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment,
            Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus, Dictionary<string, string> tokens);
    }
}
