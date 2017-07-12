using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleQueryHandler : IAsyncRequestHandler<GetUserAccountRoleQuery, GetUserAccountRoleResponse>
    {
        private readonly IValidator<GetUserAccountRoleQuery> _validator;
        private readonly IEmployerAccountService _employerAccountService;

        public GetUserAccountRoleQueryHandler(IValidator<GetUserAccountRoleQuery> validator, IEmployerAccountService employerAccountService)
        {
            _validator = validator;
            _employerAccountService = employerAccountService;
        }

        public async Task<GetUserAccountRoleResponse> Handle(GetUserAccountRoleQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var user = await _employerAccountService.GetUserRoleOnAccount(message.HashedAccountId, message.UserId);

            return  new GetUserAccountRoleResponse {User = user};
        }
    }
}