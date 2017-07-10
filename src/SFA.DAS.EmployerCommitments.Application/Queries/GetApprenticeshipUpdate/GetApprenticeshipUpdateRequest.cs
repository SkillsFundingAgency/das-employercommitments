using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipUpdate
{
    public class GetApprenticeshipUpdateRequest : IAsyncRequest<GetApprenticeshipUpdateResponse>
    {
        public long AccountId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}
