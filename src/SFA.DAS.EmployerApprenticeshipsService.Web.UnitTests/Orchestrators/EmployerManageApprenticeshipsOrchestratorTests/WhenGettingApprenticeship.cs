using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLockSummary;
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

            mockMediator.Setup(x => x.SendAsync(It.IsAny<GetDataLockSummaryQueryRequest>()))
                .ReturnsAsync(new GetDataLockSummaryQueryResponse
                {
                    DataLockSummary = new DataLockSummary
                    {
                        DataLockWithOnlyPriceMismatch = new List<DataLockStatus>(),
                        DataLockWithCourseMismatch = new List<DataLockStatus>()
                    }
                });

            mockMapper
                .Setup(mapper => mapper.MapToApprenticeshipDetailsViewModel(It.IsAny<Apprenticeship>(), false))
                .ReturnsAsync(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);

            var result = await sut.GetApprenticeship(hashedAccountId, hashedApprenticeshipId, externalUserId);

            result.Data.SearchFiltersForListView.Should().BeSameAs(filtersViewModel);
        }


        [Test, MoqCustomisedAutoData]
        public async Task ThenDataLocksTriagedAsChangeAreMappedCorrectly(
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

            mockMediator.Setup(x => x.SendAsync(It.IsAny<GetDataLockSummaryQueryRequest>()))
                .ReturnsAsync(new GetDataLockSummaryQueryResponse
                {
                    DataLockSummary = new DataLockSummary
                    {
                        DataLockWithOnlyPriceMismatch = new List<DataLockStatus>
                        {
                            new DataLockStatus
                            {
                                TriageStatus = TriageStatus.Change
                            }
                        },
                        DataLockWithCourseMismatch = new List<DataLockStatus>()
                    }
                });

            mockMapper
                .Setup(mapper => mapper.MapToApprenticeshipDetailsViewModel(It.IsAny<Apprenticeship>(), false))
                .ReturnsAsync(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);

            var result = await sut.GetApprenticeship(hashedAccountId, hashedApprenticeshipId, externalUserId);
            
            result.Data.PendingDataLockChange.Should().BeTrue();
            result.Data.PendingDataLockRestart.Should().BeFalse();
        }


        [Test, MoqCustomisedAutoData]
        public async Task ThenDataLocksTriagedAsRestartAreMappedCorrectly(
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

            mockMediator.Setup(x => x.SendAsync(It.IsAny<GetDataLockSummaryQueryRequest>()))
                .ReturnsAsync(new GetDataLockSummaryQueryResponse
                {
                    DataLockSummary = new DataLockSummary
                    {
                        DataLockWithOnlyPriceMismatch = new List<DataLockStatus>(),
                        DataLockWithCourseMismatch = new List<DataLockStatus>
                        {
                            new DataLockStatus
                            {
                                TriageStatus = TriageStatus.Restart
                            }
                        }
                    }
                });

            mockMapper
                .Setup(mapper => mapper.MapToApprenticeshipDetailsViewModel(It.IsAny<Apprenticeship>(), false))
                .ReturnsAsync(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);


            var result = await sut.GetApprenticeship(hashedAccountId, hashedApprenticeshipId, externalUserId);

            result.Data.PendingDataLockRestart.Should().BeTrue();
            result.Data.PendingDataLockChange.Should().BeFalse();
        }

    }
}