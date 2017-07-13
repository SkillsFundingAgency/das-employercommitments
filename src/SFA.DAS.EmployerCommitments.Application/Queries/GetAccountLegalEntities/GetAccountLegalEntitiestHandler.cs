using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiestHandler : IAsyncRequestHandler<GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        private readonly IValidator<GetAccountLegalEntitiesRequest> _validator;
        private readonly IEmployerAccountService _employerAccountService;

        public GetAccountLegalEntitiestHandler(IValidator<GetAccountLegalEntitiesRequest> validator, IEmployerAccountService employerAccountService)
        {
            _validator = validator;
            _employerAccountService = employerAccountService;
        }

        public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var legalEntities = await _employerAccountService.GetLegalEntitiesForAccount(message.HashedAccountId);

            return new GetAccountLegalEntitiesResponse {LegalEntities = legalEntities};
        }
    }
}