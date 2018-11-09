using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprovedApprenticeship
{
    public class GetApprovedApprenticeshipQueryRequest: IAsyncRequest<GetApprovedApprenticeshipQueryResponse>
    {
        public long ApprovedApprenticeshipId { get; set; }
        public long AccountId { get; set; }
    }
}
