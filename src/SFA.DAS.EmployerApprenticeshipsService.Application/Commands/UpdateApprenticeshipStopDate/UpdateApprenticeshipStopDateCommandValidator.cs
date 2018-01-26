using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Application.Validation;

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
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} connot be null or empty.");

            if (IsValidStopDate(command.NewStopDate))
                result.AddError(nameof(command.NewStopDate), $"{nameof(command.NewStopDate)} has an invalid value.");

            return result;
        }

        private static bool IsValidStopDate(DateTime stopDate)
        {
            return !stopDate.Equals(DateTime.MinValue) && !stopDate.Equals(DateTime.MaxValue);
        }

        public Task<ValidationResult> ValidateAsync(UpdateApprenticeshipStopDateCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
