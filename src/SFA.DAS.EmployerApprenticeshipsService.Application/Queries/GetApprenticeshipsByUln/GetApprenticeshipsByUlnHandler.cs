using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipsByUln
{
    public class GetApprenticeshipsByUlnHandler : IAsyncRequestHandler<GetApprenticeshipsByUlnRequest, GetApprenticeshipsByUlnResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;

        public GetApprenticeshipsByUlnHandler(IEmployerCommitmentApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetApprenticeshipsByUlnResponse> Handle(GetApprenticeshipsByUlnRequest message)
        {
            var apprenticeship = await _commitmentsApi.GetActiveApprenticeshipsForUln(message.AccountId, message.Uln);
            return new GetApprenticeshipsByUlnResponse
            {
                Apprenticeships =
                               apprenticeship.Where(m => m
                               .PaymentStatus != PaymentStatus.PendingApproval
                                                         && m.PaymentStatus != PaymentStatus.Withdrawn
                                                         && m.PaymentStatus != PaymentStatus.Completed
                                                         && m.PaymentStatus != PaymentStatus.Deleted)
                               .ToList()
            };
        }
    }
}