using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenCheckingForFiltersCookie
    {
        [Test, MoqCustomisedAutoData]
        public void AndNotCheckCookieThenReturnsUnmodifiedFilters(
            ApprenticeshipFiltersViewModel filtersViewModel,
            [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
            FiltersCookieManager sut)
        {
            var response = sut.CheckForCookie(TestHelper.Clone(filtersViewModel));
            response.ShouldBeEquivalentTo(filtersViewModel);
        }
    }

    public class FiltersCookieManager : IFiltersCookieManager
    {
        public FiltersCookieManager(ICookieStorageService<ApprenticeshipFiltersViewModel> filterCookieStorageService)
        {
            
        }

        public ApprenticeshipFiltersViewModel CheckForCookie(ApprenticeshipFiltersViewModel filtersViewModel)
        {
            /*            var cookieExists = false;
            if (filtersViewModel.ResetFilter)
            {
                filtersViewModel = new ApprenticeshipFiltersViewModel();
                _filterCookieStorageService.Delete(nameof(ApprenticeshipFiltersViewModel));
            }
            else if (!filtersViewModel.HasValues())
            {
                filtersViewModel = _filterCookieStorageService.Get(nameof(ApprenticeshipFiltersViewModel)) ?? new ApprenticeshipFiltersViewModel();
                cookieExists = filtersViewModel.HasValues();
            }

            if (cookieExists)
                _filterCookieStorageService.Update(nameof(ApprenticeshipFiltersViewModel), filtersViewModel);
            else if (filtersViewModel.HasValues())
                _filterCookieStorageService.Create(filtersViewModel,nameof(ApprenticeshipFiltersViewModel));*/
            return filtersViewModel;
        }
    }
}