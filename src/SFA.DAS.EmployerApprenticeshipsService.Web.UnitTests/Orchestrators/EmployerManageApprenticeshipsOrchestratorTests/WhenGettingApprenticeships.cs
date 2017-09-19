using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingApprenticeships : ManageApprenticeshipsOrchestratorTestBase
    {
        [SetUp]
        public void Setup()
        {
        
        
        
            MockMediator.Setup(x => x.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()))
                .ReturnsAsync(new ApprenticeshipSearchQueryResponse
                {
                    Apprenticeships = new List<Apprenticeship>(),
                    Facets = new Facets()
                });


            ApprenticeshipFiltersMapper.Setup(
                x => x.MapToApprenticeshipSearchQuery(It.IsAny<ApprenticeshipFiltersViewModel>()))
                .Returns(new ApprenticeshipSearchQuery());

            ApprenticeshipFiltersMapper.Setup(
                x => x.Map(It.IsAny<Facets>()))
                .Returns(new ApprenticeshipFiltersViewModel());
            
        }

        [Test]
        public async Task ThenShouldMapFiltersToSearchQuery()
        {
            //Act
            await Orchestrator.GetApprenticeships("hashedAccountId", new ApprenticeshipFiltersViewModel(), "UserId");

            //Assert
            ApprenticeshipFiltersMapper.Verify(
                x => x.MapToApprenticeshipSearchQuery(It.IsAny<ApprenticeshipFiltersViewModel>())
                , Times.Once());
        }

        [Test]
        public async Task ThenShouldMapSearchResultsToViewModel()
        {
            //Act
            await Orchestrator.GetApprenticeships("hashedAccountId", new ApprenticeshipFiltersViewModel(), "UserId");

            //Assert
            ApprenticeshipFiltersMapper.Verify(
                x => x.Map(It.IsAny<Facets>())
                , Times.Once());
        }

        [Test]
        public async Task ThenShouldCallMediatorToQueryApprenticeships()
        {
            //Act
            await Orchestrator.GetApprenticeships("hashedAccountId", new ApprenticeshipFiltersViewModel(), "UserId");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()), Times.Once);
        }
    }
}
