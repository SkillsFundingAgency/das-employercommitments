using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetProvider
{
    public class GetProviderQueryRequest : IAsyncRequest<GetProviderQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}