using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class ApprovedApprenticeshipViewModelValidator : ApprenticeshipCoreValidator
    {
        public ApprovedApprenticeshipViewModelValidator()
            : base(new WebApprenticeshipValidationText(), new CurrentDateTime(), new AcademicYear(new CurrentDateTime().Now))
        {
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance)
        {
            var result = base.Validate(instance);

            return result.Errors.ToDictionary(a => a.PropertyName, b => b.ErrorMessage);
        }
    }
}