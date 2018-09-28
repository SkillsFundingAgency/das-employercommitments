using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public interface IFiltersCookieManager
    {
        ApprenticeshipFiltersViewModel CheckForCookie(ApprenticeshipFiltersViewModel filtersViewModel);
    }
}