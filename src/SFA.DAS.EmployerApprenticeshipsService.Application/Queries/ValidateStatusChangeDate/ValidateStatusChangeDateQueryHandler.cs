using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQueryHandler : IAsyncRequestHandler<ValidateStatusChangeDateQuery, ValidateStatusChangeDateQueryResponse>
    {
        private readonly IValidator<ValidateStatusChangeDateQuery> _queryValidator;
        private readonly IMediator _mediator;
        private readonly ICurrentDateTime _currentDate;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;

        public ValidateStatusChangeDateQueryHandler(IValidator<ValidateStatusChangeDateQuery> queryValidator,
            IMediator mediator,
            ICurrentDateTime currentDate,
            IAcademicYearDateProvider academicYearDateProvider)
        {
            _queryValidator = queryValidator;
            _mediator = mediator;
            _currentDate = currentDate;
            _academicYearDateProvider = academicYearDateProvider;
        }

        public async Task<ValidateStatusChangeDateQueryResponse> Handle(ValidateStatusChangeDateQuery message)
        {
            ValidateQuery(message);

            var validationResult = new ValidationResult();

            var response = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { AccountId = message.AccountId, ApprenticeshipId = message.ApprenticeshipId });

            DateTime changeDateToUse;

            changeDateToUse = DetermineActualChangeDate(message, response.Apprenticeship);

            if (changeDateToUse > _currentDate.Now.Date)
            {
                validationResult.AddError(nameof(message.DateOfChange), "Date must be a date in the past");

                return new ValidateStatusChangeDateQueryResponse { ValidationResult = validationResult };
            }

            if (response.Apprenticeship.StartDate > changeDateToUse)
                validationResult.AddError(nameof(message.DateOfChange), "Date cannot be earlier than training start date");

            //todo: academic year rule



            return new ValidateStatusChangeDateQueryResponse { ValidationResult = validationResult, ValidatedChangeOfDate = changeDateToUse };
        }

        private DateTime DetermineActualChangeDate(ValidateStatusChangeDateQuery message, Apprenticeship apprenticeship)
        {
            bool isWaitingToStartTraining = apprenticeship.IsWaitingToStart(_currentDate);

            if (isWaitingToStartTraining)
                return apprenticeship.StartDate.Value.Date;

            return message.ChangeOption == ChangeOption.Immediately ? _currentDate.Now.Date : message.DateOfChange.Value.Date;
        }

        private void ValidateQuery(ValidateStatusChangeDateQuery message)
        {
            var validationresult = _queryValidator.Validate(message);

            if (!validationresult.IsValid())
            {
                throw new InvalidRequestException(validationresult.ValidationDictionary);
            }
        }
    }
}
