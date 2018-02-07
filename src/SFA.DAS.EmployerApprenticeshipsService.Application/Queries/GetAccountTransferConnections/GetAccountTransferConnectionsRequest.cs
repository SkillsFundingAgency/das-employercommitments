using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections
{
    public class GetAccountTransferConnectionsRequest : IAsyncRequest<GetAccountTransferConnectionsResponse>
    {
        public string UserId { get; set; }
        public string HashedAccountId { get; set; }
        public bool SignedOnly { get; set; }
    }
}
