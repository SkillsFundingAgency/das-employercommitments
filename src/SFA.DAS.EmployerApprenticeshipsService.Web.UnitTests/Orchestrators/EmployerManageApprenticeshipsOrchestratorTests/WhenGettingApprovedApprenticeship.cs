using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprovedApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingApprovedApprenticeship
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenSetsFilterFromCookie(
            string hashedAccountId,
            string hashedApprenticeshipId,
            ApprenticeshipFiltersViewModel filtersViewModel,
            string externalUserId,
            GetUserAccountRoleResponse getUserAccountRoleResponse,
            GetApprovedApprenticeshipQueryResponse getApprenticeshipQueryResponse,
            ApprovedApprenticeshipViewModel apprenticeshipDetailsViewModel,
            [Frozen] Mock<IFiltersCookieManager> mockFilterCookieManager,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IApprovedApprenticeshipMapper> mockMapper,
            EmployerManageApprenticeshipsOrchestrator sut)
        {
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(getUserAccountRoleResponse);
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetApprovedApprenticeshipQueryRequest>()))
                .ReturnsAsync(getApprenticeshipQueryResponse);

            mockMapper.Setup(mapper => mapper.Map(It.IsAny<ApprovedApprenticeship>())).Returns(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);

            var result = await sut.GetApprovedApprenticeshipViewModel(hashedAccountId, hashedApprenticeshipId, externalUserId);

            result.Data.SearchFiltersForListView.Should().BeSameAs(filtersViewModel);
        }

        [Test, MoqCustomisedAutoData]
        public async Task ThenTheMappedApprovedApprenticeshipIsReturned(
            string hashedAccountId,
            string hashedApprenticeshipId,
            ApprenticeshipFiltersViewModel filtersViewModel,
            string externalUserId,
            GetUserAccountRoleResponse getUserAccountRoleResponse,
            GetApprovedApprenticeshipQueryResponse getApprenticeshipQueryResponse,
            ApprovedApprenticeshipViewModel apprenticeshipDetailsViewModel,
            [Frozen] Mock<IFiltersCookieManager> mockFilterCookieManager,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IApprovedApprenticeshipMapper> mockMapper,
            EmployerManageApprenticeshipsOrchestrator sut)
        {
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(getUserAccountRoleResponse);
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetApprovedApprenticeshipQueryRequest>()))
                .ReturnsAsync(getApprenticeshipQueryResponse);

            mockMapper.Setup(mapper => mapper.Map(It.IsAny<ApprovedApprenticeship>())).Returns(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);

            var result = await sut.GetApprovedApprenticeshipViewModel(hashedAccountId, hashedApprenticeshipId, externalUserId);

            Assert.AreEqual(apprenticeshipDetailsViewModel, result.Data);
        }
    }
}
