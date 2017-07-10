using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDetails
{
    public class GetApprenticeshipDetailsQuery : IAsyncRequest<GetApprenticeshipDetailsResponse>
    {
        public int ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
