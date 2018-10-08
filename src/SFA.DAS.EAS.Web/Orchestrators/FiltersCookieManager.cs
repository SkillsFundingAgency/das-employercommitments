using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public class FiltersCookieManager : IFiltersCookieManager
    {
        private readonly ICookieStorageService<ApprenticeshipFiltersViewModel> _filterCookieStorageService;

        public FiltersCookieManager(ICookieStorageService<ApprenticeshipFiltersViewModel> filterCookieStorageService)
        {
            _filterCookieStorageService = filterCookieStorageService;
        }

        public ApprenticeshipFiltersViewModel GetCookie()
        {
            return _filterCookieStorageService.Get(nameof(ApprenticeshipFiltersViewModel))
                              ?? new ApprenticeshipFiltersViewModel();
        }

        public void SetCookie(ApprenticeshipFiltersViewModel filtersViewModel)
        {
            _filterCookieStorageService.Delete(nameof(ApprenticeshipFiltersViewModel));
            _filterCookieStorageService.Create(filtersViewModel, nameof(ApprenticeshipFiltersViewModel));
        }
    }
}