using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipsByUln
{
    public class GetApprenticeshipsByUlnRequest : IAsyncRequest<GetApprenticeshipsByUlnResponse>
    {
        public long AccountId { get; set; }
        public string Uln { get; set; }
    }
}
