using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryRequest : IAsyncRequest<ApprenticeshipSearchQueryResponse>
    {
        public string HashedLegalEntityId { get; set; }
        public ApprenticeshipSearchQuery Query { get; set; }
    }
}
