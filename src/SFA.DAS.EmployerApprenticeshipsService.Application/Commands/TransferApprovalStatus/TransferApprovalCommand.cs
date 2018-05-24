using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus
{
    public sealed class TransferApprovalCommand : IAsyncRequest
    {
        public long CommitmentId { get; set; }
        public long TransferRequestId { get; set; }
        public long TransferSenderId { get; set; }
        public Commitments.Api.Types.TransferApprovalStatus TransferStatus { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
    }
}
