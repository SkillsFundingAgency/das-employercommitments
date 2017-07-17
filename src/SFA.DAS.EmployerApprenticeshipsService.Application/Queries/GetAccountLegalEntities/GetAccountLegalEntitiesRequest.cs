using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
    {
        public string UserId { get; set; }
        public string HashedAccountId { get; set; }
        public bool SignedOnly { get; set; }
    }
}
