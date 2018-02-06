using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferringEntities
{
    public class GetAccountTransferringEntitiesRequest : IAsyncRequest<GetAccountTransferringEntitiesResponse>
    {
        public string UserId { get; set; }
        public string HashedAccountId { get; set; }
        public bool SignedOnly { get; set; }
    }
}
