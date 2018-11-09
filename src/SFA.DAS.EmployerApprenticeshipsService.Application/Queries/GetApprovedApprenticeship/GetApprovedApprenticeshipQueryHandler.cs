using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprovedApprenticeship
{
    public class GetApprovedApprenticeshipQueryHandler : IAsyncRequestHandler<GetApprovedApprenticeshipQueryRequest, GetApprovedApprenticeshipQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;

        public GetApprovedApprenticeshipQueryHandler(IEmployerCommitmentApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        public async Task<GetApprovedApprenticeshipQueryResponse> Handle(GetApprovedApprenticeshipQueryRequest message)
        {
            var result = await
                _commitmentApi.GetApprovedApprenticeship(message.AccountId, message.ApprovedApprenticeshipId);

            return new GetApprovedApprenticeshipQueryResponse
            {
                ApprovedApprenticeship = result
            };
        }
    }
}