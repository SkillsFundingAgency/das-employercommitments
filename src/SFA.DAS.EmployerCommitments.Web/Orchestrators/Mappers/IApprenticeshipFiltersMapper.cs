using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipFiltersMapper
    {
        ApprenticeshipSearchQuery MapToApprenticeshipSearchQuery(ApprenticeshipFiltersViewModel filters);
        ApprenticeshipFiltersViewModel Map(Facets facets);
    }
}
