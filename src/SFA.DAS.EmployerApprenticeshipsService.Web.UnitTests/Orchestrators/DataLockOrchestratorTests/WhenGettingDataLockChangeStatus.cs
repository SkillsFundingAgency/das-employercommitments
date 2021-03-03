using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.EmployerCommitments.Application.Queries.GetPriceHistoryQueryRequest;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.DataLockOrchestratorTests
{
    [TestFixture]
    public class WhenGettingDataLockChangeStatus : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldRetrieveUln()
        {
            //Arrange

            var accountId = "ABC123";
            var apprenticeId = "ABC321";
            var userId = "ABC456";
            var uln = "IAMAULN";

            var orchestrator = new DataLockOrchestrator(MockMediator.Object, MockHashingService.Object, MockLogger.Object, MockApprenticeshipMapper.Object, MockLinkGenerator.Object);

            var apprenticeshipResponse = new GetApprenticeshipQueryResponse
            {
                Apprenticeship = new Apprenticeship {ULN = uln}
            };

            var summaryResponse = new GetDataLockSummaryQueryResponse
            {
                DataLockSummary = new DataLockSummary
                {
                    DataLockWithOnlyPriceMismatch = new List<DataLockStatus>(),
                    DataLockWithCourseMismatch = new List<DataLockStatus>()
                }
            };

            var priceHistoryResponse = new GetPriceHistoryQueryResponse
            {
                History = new List<PriceHistory>()
            };

            IEnumerable<CourseChange> courseChanges = new List<CourseChange>();
            IList<PriceChange> pricesChanges = new List<PriceChange>();

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>())).Returns(Task.FromResult(apprenticeshipResponse));
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetDataLockSummaryQueryRequest>())).Returns(Task.FromResult(summaryResponse));
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetPriceHistoryQueryRequest>())).Returns(Task.FromResult(priceHistoryResponse));

            MockApprenticeshipMapper.Setup(x => x.MapCourseChanges(It.IsAny<IEnumerable<DataLockStatus>>(), It.IsAny<Apprenticeship>(), It.IsAny<IList<PriceHistory>>())).Returns(Task.FromResult(courseChanges));
            MockApprenticeshipMapper.Setup(x => x.MapPriceChanges(It.IsAny<IEnumerable<DataLockStatus>>(), It.IsAny<List<PriceHistory>>())).Returns(pricesChanges);
            
            //Act
            var result = await orchestrator.GetDataLockChangeStatus(accountId, apprenticeId, userId);

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()));

            Assert.AreEqual(uln, result.Data.ULN);
        }
    }
}
