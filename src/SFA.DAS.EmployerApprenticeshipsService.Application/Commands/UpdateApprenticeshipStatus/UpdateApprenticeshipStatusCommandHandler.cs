using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus
{
    public sealed class UpdateApprenticeshipStatusCommandHandler : AsyncRequestHandler<UpdateApprenticeshipStatusCommand>
    {
        private IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipStatusCommand> _validator;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IMediator _mediator;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly IAcademicYearValidator _academicYearValidator;

        public UpdateApprenticeshipStatusCommandHandler(IEmployerCommitmentApi commitmentsApi, IMediator mediator, ICurrentDateTime currentDateTime, IValidator<UpdateApprenticeshipStatusCommand> validator, IAcademicYearDateProvider academicYearDateProvider, IAcademicYearValidator academicYearValidator)
        {
            _commitmentsApi = commitmentsApi;
            _mediator = mediator;
            _currentDateTime = currentDateTime;
            _validator = validator;
            _academicYearDateProvider = academicYearDateProvider;
            _academicYearValidator = academicYearValidator;
        }

        protected override async Task HandleCore(UpdateApprenticeshipStatusCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeshipSubmission = new ApprenticeshipSubmission
            {
                PaymentStatus = DeterminePaymentStatusForChange(command.ChangeType),
                DateOfChange = command.DateOfChange,
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
            };

            await ValidateDateOfChange(command, validationResult);

            await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId, apprenticeshipSubmission);
        }

        private async Task ValidateDateOfChange(UpdateApprenticeshipStatusCommand command, Validation.ValidationResult validationResult)
        {
            if (command.ChangeType == ChangeStatusType.Stop) // Only need to validate date for stop currently
            {
                var response = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { AccountId = command.EmployerAccountId, ApprenticeshipId = command.ApprenticeshipId });

                if (response.Apprenticeship.IsWaitingToStart(_currentDateTime))
                {
                    if (!command.DateOfChange.Equals(response.Apprenticeship.StartDate))
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "Date must the same as start date if training hasn't started");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }
                }
                else
                {
                    if (command.DateOfChange > _currentDateTime.Now.Date)
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "Date must be a date in the past");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }

                    if (response.Apprenticeship.StartDate > command.DateOfChange)
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "Date cannot be earlier than training start date");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }

                    if (_academicYearValidator.Validate(command.DateOfChange) == AcademicYearValidationResult.NotWithinFundingPeriod)
                    {
                        var earliestDate = _academicYearDateProvider.CurrentAcademicYearStartDate.ToString("dd MM yyyy");

                        validationResult.AddError(nameof(command.DateOfChange), $"The earliest date you can stop this apprentice is {earliestDate}");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }
                }
            }
        }

        private static PaymentStatus DeterminePaymentStatusForChange(ChangeStatusType changeType)
        {
            switch (changeType)
            {
                case ChangeStatusType.Pause:
                    return PaymentStatus.Paused;
                case ChangeStatusType.Resume:
                    return PaymentStatus.Active;
                case ChangeStatusType.Stop:
                    return PaymentStatus.Withdrawn;
                default:
                    throw new ArgumentOutOfRangeException(nameof(changeType), "Not a valid change type");
            }
        }
    }
}
