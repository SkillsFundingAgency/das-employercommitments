using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDetails
{
    public class GetApprenticeshipDetailsHandler : IAsyncRequestHandler<GetApprenticeshipDetailsQuery, GetApprenticeshipDetailsResponse>
    {
        private readonly IValidator<GetApprenticeshipDetailsQuery> _validator;
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetApprenticeshipDetailsHandler(IValidator<GetApprenticeshipDetailsQuery> validator, IApprenticeshipInfoService apprenticeshipInfoService)
        {
            _validator = validator;
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetApprenticeshipDetailsResponse> Handle(GetApprenticeshipDetailsQuery message)
        {
            var validationresult = _validator.Validate(message);

            if (!validationresult.IsValid())
            {
                throw new InvalidRequestException(validationresult.ValidationDictionary);
            }

            var provider = await _apprenticeshipInfoService.GetProvider(message.ProviderId);

            var providerName = provider?.Provider?.Name ?? "Unknown provider";

            return new GetApprenticeshipDetailsResponse
            {
                ProviderName = providerName
            };
        
        }
    }
}
