using System;
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

            try
            {

                ValidateNewStopDate(command, apprenticeship);
            }
            catch (Exception ex)
            {
                var message = "ValidateNewStopDate error";
                throw new Exception(message, ex);
            }

            try
            {
                await _commitmentsApi.PutApprenticeshipStopDate(command.EmployerAccountId, command.CommitmentId, command.ApprenticeshipId, stopDate);
            }
            catch (Exception ex)
            {
                var message = "PutApprenticeshipStopDate error";
                throw new Exception(message, ex);
            }

            try
            {
                await _providerEmailNotificationService.SendProviderApprenticeshipStopEditNotification(apprenticeship,
                    command.NewStopDate);
            }
            catch (Exception ex)
            {
                var builder = new System.Text.StringBuilder();

                builder.AppendLine("SendProviderApprenticeshipStopEditNotification error");
                builder.AppendLine("Stop date = " + command.NewStopDate.ToString());

                if(apprenticeship != null)
                {
                    builder.AppendLine("apprenticeship is not null");
                }

                if(_providerEmailNotificationService == null)
                {
                    builder.AppendLine("_providerEmailNotificationService is null");
                }

                builder.AppendLine("LegalEntityName : " + apprenticeship.LegalEntityName);
                var stopdate = apprenticeship.StopDate.HasValue == true ? apprenticeship.StopDate.Value.ToString("dd/MM/yyyy") : "not set" ;
                builder.AppendLine("OLDDATE : " + stopdate);
                builder.AppendLine("ProviderId : " + apprenticeship.ProviderId);

                throw new Exception(builder.ToString(), ex);
            }
        }

        private void ValidateNewStopDate(UpdateApprenticeshipStopDateCommand command, Apprenticeship apprenticeship)
        {
            var validationResult = new ValidationResult();

            var startdate = new DateTime(apprenticeship.StartDate.Value.Year, apprenticeship.StartDate.Value.Month, 1);

            if (apprenticeship.IsWaitingToStart(_currentDateTime))
            {
                if (!command.NewStopDate.Equals(startdate))
                {
                    validationResult.AddError(nameof(command.NewStopDate),
                        "Date must the same as start date if training hasn't started");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }
            }
            else
            {
                if (command.NewStopDate > new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1))
                {
                    validationResult.AddError(nameof(command.NewStopDate), "The stop date cannot be in the future");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }

                if (startdate > command.NewStopDate)
                {
                    validationResult.AddError(nameof(command.NewStopDate), "The stop month cannot be before the apprenticeship started");
                    throw new InvalidRequestException(validationResult.ValidationDictionary);
                }                
            }
        }
    }
}
