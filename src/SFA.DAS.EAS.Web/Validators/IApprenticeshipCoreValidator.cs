using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    //public interface IApprenticeshipCoreValidator : IValidator<ApprenticeshipViewModel>
    public interface IApprenticeshipCoreValidator
    {
        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);
    }
}