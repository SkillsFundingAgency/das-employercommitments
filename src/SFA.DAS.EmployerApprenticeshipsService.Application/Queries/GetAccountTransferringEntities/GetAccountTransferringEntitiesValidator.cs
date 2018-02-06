using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferringEntities
{
    public class GetAccountTransferringEntitiesValidator : IValidator<GetAccountTransferringEntitiesRequest>
    {
        public ValidationResult Validate(GetAccountTransferringEntitiesRequest item)
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

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountTransferringEntitiesRequest item)
        {
            return Task.Run(()=>Validate(item));
        }
    }
}