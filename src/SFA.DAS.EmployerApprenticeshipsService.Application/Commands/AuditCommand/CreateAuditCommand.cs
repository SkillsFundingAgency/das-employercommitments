using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Models.Audit;

namespace SFA.DAS.EmployerCommitments.Application.Commands.AuditCommand
{
    public class CreateAuditCommand : IAsyncRequest
    {
        public EasAuditMessage EasAuditMessage { get; set; }
    }
}
