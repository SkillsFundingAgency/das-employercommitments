using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public interface IValidateApprovedApprenticeship : IApprenticeshipCoreValidator
    {
        Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance);

        Dictionary<string, string> ValidateAcademicYear(UpdateApprenticeshipViewModel model);

        Dictionary<string, string> ValidateApprovedEndDate(UpdateApprenticeshipViewModel instance);
    }
}