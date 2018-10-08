using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingApprenticeships
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenUsesFilterCookieManager(
            string hashedAccountId,
            ApprenticeshipFiltersViewModel filtersViewModel,
            string externalUserId,
            GetUserAccountRoleResponse getUserAccountRoleResponse,
            ApprenticeshipSearchQueryResponse apprenticeshipSearchQueryResponse,
            [Frozen] Mock<IFiltersCookieManager> mockFilterCookieManager,
            [Frozen] Mock<IMediator> mockMediator,
            EmployerManageApprenticeshipsOrchestrator sut)
        {
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(getUserAccountRoleResponse);
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()))
                .ReturnsAsync(apprenticeshipSearchQueryResponse);

            await sut.GetApprenticeships(hashedAccountId, filtersViewModel, externalUserId);
            mockFilterCookieManager.Verify(manager => manager.SetCookie(filtersViewModel));
        }
    }
}