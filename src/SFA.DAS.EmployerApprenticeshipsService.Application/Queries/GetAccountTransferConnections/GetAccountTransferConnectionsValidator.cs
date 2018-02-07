using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections
{
    public class GetAccountTransferConnectionsValidator : IValidator<GetAccountTransferConnectionsRequest>
    {
        public ValidationResult Validate(GetAccountTransferConnectionsRequest item)
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

        public Task<ValidationResult> ValidateAsync(GetAccountTransferConnectionsRequest item)
        {
            return Task.Run(()=>Validate(item));
        }
    }
}