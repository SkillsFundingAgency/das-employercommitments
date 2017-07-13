using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesValidator : IValidator<GetAccountLegalEntitiesRequest>
    {
        private readonly IEmployerAccountService _employerAccountService;

        public GetAccountLegalEntitiesValidator(IEmployerAccountService employerAccountService)
        {
            _employerAccountService = employerAccountService;
        }

        public ValidationResult Validate(GetAccountLegalEntitiesRequest item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesRequest item)
        {
            
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId));
            }

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var result = await _employerAccountService.GetUserRoleOnAccount(item.HashedAccountId, item.UserId);

            if (result == null)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}