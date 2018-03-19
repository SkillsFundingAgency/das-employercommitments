using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment
{
    public class GetCommitmentQueryRequest : IAsyncRequest<GetCommitmentQueryResponse>
    {
        public GetCommitmentQueryRequest()
        {
            CallerType = CallerType.Employer;
        }
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
        public CallerType CallerType { get; set; }
    }
}

