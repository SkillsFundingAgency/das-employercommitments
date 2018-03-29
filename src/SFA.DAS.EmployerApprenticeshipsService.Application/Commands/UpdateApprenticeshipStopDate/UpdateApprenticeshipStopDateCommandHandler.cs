﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
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
        private readonly IMediator _mediator;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly IAcademicYearValidator _academicYearValidator;

        public UpdateApprenticeshipStopDateCommandHandler(IEmployerCommitmentApi commitmentsApi, IMediator mediator, ICurrentDateTime currentDateTime, IValidator<UpdateApprenticeshipStopDateCommand> validator, IAcademicYearDateProvider academicYearDateProvider, IAcademicYearValidator academicYearValidator)
        {
            _commitmentsApi = commitmentsApi;
            _mediator = mediator;
            _currentDateTime = currentDateTime;
            _validator = validator;
            _academicYearDateProvider = academicYearDateProvider;
            _academicYearValidator = academicYearValidator;
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

            await ValidateNewStopDate(command, validationResult);

            await _commitmentsApi.PutApprenticeshipStopDate(command.EmployerAccountId, command.CommitmentId, command.ApprenticeshipId, stopDate);
        }

        private async Task ValidateNewStopDate(UpdateApprenticeshipStopDateCommand command,
            ValidationResult validationResult)
        {
            var response = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                AccountId = command.EmployerAccountId,
                ApprenticeshipId = command.ApprenticeshipId
            });

            if (response.Apprenticeship.IsWaitingToStart(_currentDateTime))
            {
                if (!command.NewStopDate.Equals(response.Apprenticeship.StartDate))
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

                if (response.Apprenticeship.StartDate > command.NewStopDate)
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