using SFA.DAS.EmployerCommitments.Web.ViewModels;
using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public interface IApprenticeshipViewModelValidator
    {
        Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance);
        Dictionary<string, string> ValidateAcademicYear(ApprenticeshipViewModel model);
    }
}