using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockRequest : IAsyncRequest<GetApprenticeshipDataLockResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long AccountId { get; set; }
    }
}