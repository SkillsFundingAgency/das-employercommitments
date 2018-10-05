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
    public class GivenAFiltersCookieMananger
    {
        [TestFixture]
        public class WhenCallingGetCookie
        {
            [Test, MoqCustomisedAutoData]
            public void AndNoCookieThenReturnsNewFilters(
                [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
                FiltersCookieManager sut)
            {
                mockCookieStorageService
                    .Setup(service => service.Get(nameof(ApprenticeshipFiltersViewModel)))
                    .Returns((ApprenticeshipFiltersViewModel)null);

                var response = sut.GetCookie();

                response.ShouldBeEquivalentTo(new ApprenticeshipFiltersViewModel());
            }

            [Test, MoqCustomisedAutoData]
            public void AndHasCookieThenReturnsFilterFromCookie(
                ApprenticeshipFiltersViewModel expectedApprenticeshipFiltersViewModel,
                [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
                FiltersCookieManager sut)
            {
                mockCookieStorageService
                    .Setup(service => service.Get(nameof(ApprenticeshipFiltersViewModel)))
                    .Returns(expectedApprenticeshipFiltersViewModel);

                var response = sut.GetCookie();

                response.Should().BeSameAs(expectedApprenticeshipFiltersViewModel);
            }
        }

        [TestFixture]
        public class WhenCallingSetCookie
        {
            private const int DefaultCookieExpiryDays = 1;

            [Test, MoqCustomisedAutoData]
            public void ThenStoresFiltersAsCookie(
                ApprenticeshipFiltersViewModel filtersViewModel,
                [Frozen] Mock<ICookieStorageService<ApprenticeshipFiltersViewModel>> mockCookieStorageService,
                FiltersCookieManager sut)
            {
                sut.SetCookie(filtersViewModel);
                mockCookieStorageService.Verify(service => service.Delete(nameof(ApprenticeshipFiltersViewModel)));
                mockCookieStorageService.Verify(service => service.Create(filtersViewModel, nameof(ApprenticeshipFiltersViewModel), DefaultCookieExpiryDays));
            }
        }
    }
}