using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipRedundancy
{
    public sealed class UpdateApprenticeshipRedundancyCommandValidator : IValidator<UpdateApprenticeshipRedundancyCommand>
    {
        public ValidationResult Validate(UpdateApprenticeshipRedundancyCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new ValidationResult();

            if (command.EmployerAccountId <= 0)
                result.AddError(nameof(command.EmployerAccountId), $"{nameof(command.EmployerAccountId)} has an invalid value.");

            if (command.ApprenticeshipId <= 0)
                result.AddError(nameof(command.ApprenticeshipId), $"{nameof(command.ApprenticeshipId)} has an invalid value.");

            if (string.IsNullOrEmpty(command.UserId))
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} cannot be null or empty.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(UpdateApprenticeshipRedundancyCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
