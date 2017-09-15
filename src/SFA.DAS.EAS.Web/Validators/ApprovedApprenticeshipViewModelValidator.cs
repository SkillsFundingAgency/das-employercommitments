using System;
using System.Collections.Generic;
using System.Linq;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class ApprovedApprenticeshipViewModelValidator : ApprenticeshipCoreValidator, IValidateApprenticeship
    {
        public ApprovedApprenticeshipViewModelValidator(
            WebApprenticeshipValidationText validationText,
            ICurrentDateTime currentDateTime, 
            IAcademicYearDateProvider academicYear,
            IAcademicYearValidator academicYearValidator) 
            : base(validationText, currentDateTime, academicYear, academicYearValidator)
        {
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance)
        {
            var result = base.Validate(instance);

            return result.Errors.ToDictionary(a => a.PropertyName, b => b.ErrorMessage);
        }
    }

    public interface IValidateApprenticeship
    {
        Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance);

        Dictionary<string, string> ValidateAcademicYear(DateTime? date);
    }
}