using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingApprenticeship
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenSetsFilterFromCookie(
            string hashedAccountId,
            string hashedApprenticeshipId,
            ApprenticeshipFiltersViewModel filtersViewModel,
            string externalUserId,
            GetUserAccountRoleResponse getUserAccountRoleResponse,
            GetApprenticeshipQueryResponse getApprenticeshipQueryResponse,
            ApprenticeshipDetailsViewModel apprenticeshipDetailsViewModel,
            [Frozen] Mock<IFiltersCookieManager> mockFilterCookieManager,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IApprenticeshipMapper> mockMapper,
            EmployerManageApprenticeshipsOrchestrator sut)
        {
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(getUserAccountRoleResponse);
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(getApprenticeshipQueryResponse);

            mockMapper
                .Setup(mapper => mapper.MapToApprenticeshipDetailsViewModel(It.IsAny<Apprenticeship>(), false))
                .ReturnsAsync(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);

            var result = await sut.GetApprenticeship(hashedAccountId, hashedApprenticeshipId, externalUserId);

            result.Data.SearchFiltersForListView.Should().BeSameAs(filtersViewModel);
        }
    }
}