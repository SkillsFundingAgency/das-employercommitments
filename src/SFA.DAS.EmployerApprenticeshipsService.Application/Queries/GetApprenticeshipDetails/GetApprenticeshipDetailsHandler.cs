using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDetails
{
    public class GetApprenticeshipDetailsHandler : IAsyncRequestHandler<GetApprenticeshipDetailsQuery, GetApprenticeshipDetailsResponse>
    {
        private readonly IValidator<GetApprenticeshipDetailsQuery> _validator;
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;

        public GetApprenticeshipDetailsHandler(IValidator<GetApprenticeshipDetailsQuery> validator, IApprenticeshipInfoServiceWrapper apprenticeshipInfoService)
        {
            _validator = validator;
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public Task<GetApprenticeshipDetailsResponse> Handle(GetApprenticeshipDetailsQuery message)
        {
            var validationresult = _validator.Validate(message);

            if (!validationresult.IsValid())
            {
                throw new InvalidRequestException(validationresult.ValidationDictionary);
            }

            return Task.Run(() =>
            {
                var provider = _apprenticeshipInfoService.GetProvider(message.ProviderId);

                var providerName = provider?.Provider?.Name ?? "Unknown provider";

                return new GetApprenticeshipDetailsResponse
                {
                    ProviderName = providerName
                };
            });
        }
    }
}
