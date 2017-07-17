using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleValidator : IValidator<GetUserAccountRoleQuery>
    {
        public ValidationResult Validate(GetUserAccountRoleQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetUserAccountRoleQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
