using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public interface IFiltersCookieManager
    {
        ApprenticeshipFiltersViewModel GetCookie();
        void SetCookie(ApprenticeshipFiltersViewModel filtersViewModel);
    }
}