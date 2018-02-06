using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferringEntities
{
    public class GetAccountTransferringEntitiesHandler : IAsyncRequestHandler<GetAccountTransferringEntitiesRequest, GetAccountTransferringEntitiesResponse>
    {
        private readonly IValidator<GetAccountTransferringEntitiesRequest> _validator;
        private readonly IEmployerAccountService _employerAccountService;

        public GetAccountTransferringEntitiesHandler(IValidator<GetAccountTransferringEntitiesRequest> validator, IEmployerAccountService employerAccountService)
        {
            _validator = validator;
            _employerAccountService = employerAccountService;
        }

        public async Task<GetAccountTransferringEntitiesResponse> Handle(GetAccountTransferringEntitiesRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transferringEntities = await _employerAccountService.GetTransferConnectionsForAccount(message.HashedAccountId);

            return new GetAccountTransferringEntitiesResponse { TransferringEntities = transferringEntities };
        }
    }
}