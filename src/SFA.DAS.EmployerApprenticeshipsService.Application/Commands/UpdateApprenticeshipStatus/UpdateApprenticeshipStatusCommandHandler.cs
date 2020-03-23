using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus
{
    public sealed class UpdateApprenticeshipStatusCommandHandler : AsyncRequestHandler<UpdateApprenticeshipStatusCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipStatusCommand> _validator;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;

        public UpdateApprenticeshipStatusCommandHandler(IEmployerCommitmentApi commitmentsApi,
            ICurrentDateTime currentDateTime,
            IValidator<UpdateApprenticeshipStatusCommand> validator,
            IAcademicYearDateProvider academicYearDateProvider,
            IAcademicYearValidator academicYearValidator,
            IProviderEmailNotificationService providerEmailNotificationService)
        {
            _commitmentsApi = commitmentsApi;
            _currentDateTime = currentDateTime;
            _validator = validator;
            _academicYearDateProvider = academicYearDateProvider;
            _academicYearValidator = academicYearValidator;
            _providerEmailNotificationService = providerEmailNotificationService;
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

            var apprenticeship = await _commitmentsApi.GetEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId);

            ValidateDateOfChange(apprenticeship, command, validationResult);

            await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId, apprenticeshipSubmission);

            switch (command.ChangeType)
            {
                case ChangeStatusType.Stop: await _providerEmailNotificationService.SendProviderApprenticeshipStopNotification(apprenticeship, command.DateOfChange);
                    break;
                case ChangeStatusType.Pause: await _providerEmailNotificationService.SendProviderApprenticeshipPauseNotification(apprenticeship, command.DateOfChange);
                    break;
                case ChangeStatusType.Resume: await _providerEmailNotificationService.SendProviderApprenticeshipResumeNotification(apprenticeship, command.DateOfChange);
                    break;
            }
        }

        private void ValidateDateOfChange(Apprenticeship apprenticeship,
            UpdateApprenticeshipStatusCommand command,
            ValidationResult validationResult)
        {
            if (command.ChangeType == ChangeStatusType.Stop) // Only need to validate date for stop currently
            {
                if (!apprenticeship.IsWaitingToStart(_currentDateTime))
                {
                    if (command.DateOfChange > new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1))
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "The stop date cannot be in the future");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }

                    if (new DateTime(apprenticeship.StartDate.Value.Year, apprenticeship.StartDate.Value.Month, 1) > command.DateOfChange)
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "The stop month cannot be before the apprenticeship started");
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
