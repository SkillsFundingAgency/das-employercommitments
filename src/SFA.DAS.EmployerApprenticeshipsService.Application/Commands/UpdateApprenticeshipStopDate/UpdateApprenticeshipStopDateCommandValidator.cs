using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate
{
    public sealed class UpdateApprenticeshipStopDateCommandValidator : IValidator<UpdateApprenticeshipStopDateCommand>
    {
        public ValidationResult Validate(UpdateApprenticeshipStopDateCommand command)
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

        public Task<ValidationResult> ValidateAsync(UpdateApprenticeshipStopDateCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
