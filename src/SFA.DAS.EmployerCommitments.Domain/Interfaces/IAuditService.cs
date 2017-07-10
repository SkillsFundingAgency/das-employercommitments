using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.Audit;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAuditService
    {
        Task SendAuditMessage(EasAuditMessage message);
    }
}