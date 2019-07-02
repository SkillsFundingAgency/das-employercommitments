using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQueryHandler : IAsyncRequestHandler<ValidateStatusChangeDateQuery, ValidateStatusChangeDateQueryResponse>
    {
        private readonly IValidator<ValidateStatusChangeDateQuery> _queryValidator;
        private readonly ICurrentDateTime _currentDate;
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly IEmployerCommitmentApi _commitmentsApi;

        public ValidateStatusChangeDateQueryHandler(IValidator<ValidateStatusChangeDateQuery> queryValidator,
            ICurrentDateTime currentDate,
            IAcademicYearValidator academicYearValidator,
            IEmployerCommitmentApi commitmentsApi)
        {
            _queryValidator = queryValidator;
            _currentDate = currentDate;
            _academicYearValidator = academicYearValidator;
            _commitmentsApi = commitmentsApi;
        }

        public async Task<ValidateStatusChangeDateQueryResponse> Handle(ValidateStatusChangeDateQuery message)
        {
            ValidateQuery(message);

            var validationResult = new ValidationResult();
            
            var apprenticeship =
                await _commitmentsApi.GetEmployerApprenticeship(message.AccountId, message.ApprenticeshipId);

            var changeDateToUse = DetermineActualChangeDate(message, apprenticeship);
            
            if (changeDateToUse > new DateTime(_currentDate.Now.Year, _currentDate.Now.Month, 1))
            {
                validationResult.AddError(nameof(message.DateOfChange), "The stop date cannot be in the future");

                return new ValidateStatusChangeDateQueryResponse { ValidationResult = validationResult };
            }

            if (new DateTime(apprenticeship.StartDate.Value.Year, apprenticeship.StartDate.Value.Month, 1) > changeDateToUse)
            {
                validationResult.AddError(nameof(message.DateOfChange), "The stop month cannot be before the apprenticeship started");
                return new ValidateStatusChangeDateQueryResponse { ValidationResult = validationResult };
            }

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
