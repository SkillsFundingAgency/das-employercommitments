using MediatR;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetTransferRequest
{
    public class GetTransferRequestQueryRequest : IAsyncRequest<GetTransferRequestQueryResponse>
    {
        public long AccountId { get; set; }
        public long TransferRequestId { get; set; }
        public CallerType CallerType { get; set; }
    }
}

