using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetCommitments
{
    public sealed class GetCommitmentsQuery : IAsyncRequest<GetCommitmentsResponse>
    {
        public long AccountId { get; set; }
    }
}
