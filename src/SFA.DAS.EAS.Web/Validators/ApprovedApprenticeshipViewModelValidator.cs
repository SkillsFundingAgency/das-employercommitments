using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class ApprovedApprenticeshipViewModelValidator : ApprenticeshipCoreValidator
    {
        public ApprovedApprenticeshipViewModelValidator(IApprenticeshipValidationErrorText validationText, 
                                                        ICurrentDateTime currentDateTime, 
                                                        IAcademicYearDateProvider academicYearDateProvider, 
                                                        IAcademicYearValidator academicYearValidator )
            : base(validationText, currentDateTime, academicYearDateProvider, academicYearValidator)
        {
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance)
        {
            var result = base.Validate(instance);

            return result.Errors.ToDictionary(a => a.PropertyName, b => b.ErrorMessage);
        }
    }
}