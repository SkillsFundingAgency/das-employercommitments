using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections
{
    public class GetAccountTransferConnectionsHandler : IAsyncRequestHandler<GetAccountTransferConnectionsRequest, GetAccountTransferConnectionsResponse>
    {
        private readonly IValidator<GetAccountTransferConnectionsRequest> _validator;
        private readonly IEmployerAccountService _employerAccountService;

        public GetAccountTransferConnectionsHandler(IValidator<GetAccountTransferConnectionsRequest> validator, IEmployerAccountService employerAccountService)
        {
            _validator = validator;
            _employerAccountService = employerAccountService;
        }

        public async Task<GetAccountTransferConnectionsResponse> Handle(GetAccountTransferConnectionsRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transferringEntities = await _employerAccountService.GetTransferConnectionsForAccount(message.HashedAccountId);

            return new GetAccountTransferConnectionsResponse { TransferringEntities = transferringEntities };
        }
    }
}