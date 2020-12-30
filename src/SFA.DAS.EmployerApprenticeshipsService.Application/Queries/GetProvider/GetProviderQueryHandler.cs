using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetProvider
{
    public class GetProviderQueryHandler : IAsyncRequestHandler<GetProviderQueryRequest, GetProviderQueryResponse>
    {
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetProviderQueryHandler(IApprenticeshipInfoService apprenticeshipInfoService)
        {
            if (apprenticeshipInfoService == null)
                throw new ArgumentNullException(nameof(apprenticeshipInfoService));
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetProviderQueryResponse> Handle(GetProviderQueryRequest message)
        {
            var provider = await _apprenticeshipInfoService.GetProvider(message.ProviderId);

            return new GetProviderQueryResponse
            {
                ProvidersView = provider
            };
        }
    }
}