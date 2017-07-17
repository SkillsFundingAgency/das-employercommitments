using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetDataLockSummaryQueryRequest :
        IAsyncRequest<GetDataLockSummaryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long AccountId { get; set; }
    }
}
