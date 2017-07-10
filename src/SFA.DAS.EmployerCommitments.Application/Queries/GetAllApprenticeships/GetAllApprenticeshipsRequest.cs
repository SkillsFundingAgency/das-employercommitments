using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAllApprenticeships
{
    public class GetAllApprenticeshipsRequest : IAsyncRequest<GetAllApprenticeshipsResponse>
    {
        public long AccountId { get; set; }
    }
}
