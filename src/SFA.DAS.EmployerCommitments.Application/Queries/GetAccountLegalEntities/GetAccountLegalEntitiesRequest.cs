using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
    {
        public string UserId { get; set; }
        public string HashedLegalEntityId { get; set; }
        public bool SignedOnly { get; set; }
    }
}
