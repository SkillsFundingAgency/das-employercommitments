using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesValidator : IValidator<GetAccountLegalEntitiesRequest>
    {
        public ValidationResult Validate(GetAccountLegalEntitiesRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId));
            }

            if (string.IsNullOrEmpty(item.HashedLegalEntityId))
            {
                validationResult.AddError(nameof(item.HashedLegalEntityId));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}