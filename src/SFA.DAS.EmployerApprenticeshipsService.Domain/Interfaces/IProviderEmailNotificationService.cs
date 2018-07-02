using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IProviderEmailNotificationService
    {
        Task SendProviderTransferRejectedCommitmentEditNotification(CommitmentView commitment);
        Task SendCreateCommitmentNotification(CommitmentView commitment);
    }
}
