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
        private const int DefaultCookieExpiryDays = 1;

        [Test, MoqCustomisedAutoData]
        public void AndNotCheckCookieThenReturnsUnmodifiedFilters(
            ApprenticeshipFiltersViewModel filtersViewModel,
            [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
            FiltersCookieManager sut)
        {
            filtersViewModel.CheckCookie = false;
            var response = sut.CheckForCookie(TestHelper.Clone(filtersViewModel));
            response.ShouldBeEquivalentTo(filtersViewModel);
        }

        [Test, MoqCustomisedAutoData]
        public void AndNotCheckCookieThenStillStoresFiltersInCookie(
            ApprenticeshipFiltersViewModel filtersViewModel,
            [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
            FiltersCookieManager sut)
        {
            filtersViewModel.CheckCookie = false;
            sut.CheckForCookie(filtersViewModel);
            mockCookieStorageService.Verify(service => service.Delete(nameof(ApprenticeshipFiltersViewModel)));
            mockCookieStorageService.Verify(service => service.Create(filtersViewModel, nameof(ApprenticeshipFiltersViewModel), DefaultCookieExpiryDays));
        }

        [Test, MoqCustomisedAutoData]
        public void AndCheckCookieThenStillStoresFiltersInCookie(
            ApprenticeshipFiltersViewModel filtersViewModel,
            [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
            FiltersCookieManager sut)
        {
            filtersViewModel.CheckCookie = true;
            sut.CheckForCookie(filtersViewModel);
            mockCookieStorageService.Verify(service => service.Delete(nameof(ApprenticeshipFiltersViewModel)));
            mockCookieStorageService.Verify(service => service.Create(filtersViewModel, nameof(ApprenticeshipFiltersViewModel), DefaultCookieExpiryDays));
        }

        [Test, MoqCustomisedAutoData]
        public void AndCheckCookieThenReturnsFiltersFromCookie(
            ApprenticeshipFiltersViewModel filtersViewModel,
            ApprenticeshipFiltersViewModel filtersViewModelFromCookie,
            [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
            FiltersCookieManager sut)
        {
            filtersViewModel.CheckCookie = true;
            mockCookieStorageService
                .Setup(service => service.Get(nameof(ApprenticeshipFiltersViewModel)))
                .Returns(filtersViewModelFromCookie);

            var response = sut.CheckForCookie(TestHelper.Clone(filtersViewModel));
            response.ShouldBeEquivalentTo(filtersViewModelFromCookie);
        }
    }
}