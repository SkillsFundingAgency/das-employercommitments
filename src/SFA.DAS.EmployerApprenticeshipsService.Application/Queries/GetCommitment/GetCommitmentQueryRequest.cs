using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment
{
    public class GetCommitmentQueryRequest : IAsyncRequest<GetCommitmentQueryResponse>
    {
        public GetCommitmentQueryRequest()
        {
            CallType = CallType.Employer;
        }
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
        public CallType CallType { get; set; }
    }
}

