using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate
{
    public sealed class UpdateApprenticeshipStopDateCommandHandler : AsyncRequestHandler<UpdateApprenticeshipStopDateCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipStopDateCommand> _validator;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;

        public UpdateApprenticeshipStopDateCommandHandler(IEmployerCommitmentApi commitmentsApi,
            ICurrentDateTime currentDateTime,
            IValidator<UpdateApprenticeshipStopDateCommand> validator,
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

        protected override async Task HandleCore(UpdateApprenticeshipStopDateCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var stopDate = new ApprenticeshipStopDate
            {
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName },
                NewStopDate = command.NewStopDate
            };

            var apprenticeship = await
                _commitmentsApi.GetEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId);

            ValidateNewStopDate(command, apprenticeship);

            await _commitmentsApi.PutApprenticeshipStopDate(command.EmployerAccountId, command.CommitmentId, command.ApprenticeshipId, stopDate);
           
            await _providerEmailNotificationService.SendProviderApprenticeshipStopEditNotification(apprenticeship,
                command.NewStopDate);
        }

        private void ValidateNewStopDate(UpdateApprenticeshipStopDateCommand command, Apprenticeship apprenticeship)
        {
            var validationResult = new ValidationResult();
        
            if (apprenticeship.IsWaitingToStart(_currentDateTime))
            {
                if (!command.NewStopDate.Equals(apprenticeship.StartDate))
                {
                    validationResult.AddError(nameof(command.NewStopDate),
                        "Date must the same as start date if training hasn't started");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }
            }
            else
            {
                if (command.NewStopDate > _currentDateTime.Now.Date)
                {
                    validationResult.AddError(nameof(command.NewStopDate), "Date must be a date in the past");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }

                if (apprenticeship.StartDate > command.NewStopDate)
                {
                    validationResult.AddError(nameof(command.NewStopDate),
                        "Date cannot be earlier than training start date");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }

                if (_academicYearValidator.Validate(command.NewStopDate) ==
                    AcademicYearValidationResult.NotWithinFundingPeriod)
                {
                    var earliestDate = _academicYearDateProvider.CurrentAcademicYearStartDate.ToString("dd MM yyyy");

                    validationResult.AddError(nameof(command.NewStopDate),
                        $"The earliest date you can stop this apprentice is {earliestDate}");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }
            }
        }
    }
}
