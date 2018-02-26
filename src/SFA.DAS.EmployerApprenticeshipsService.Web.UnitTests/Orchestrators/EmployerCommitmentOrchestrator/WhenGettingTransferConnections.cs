using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingTransferConnections : OrchestratorTestBase
    {
        private Mock<IFeatureToggle> _mockFeatureToggle;
        private GetAccountTransferConnectionsResponse _sendAsyncResponse;
        const string HashedAccountId = "ABC123";
        const string UserId = "User1";

        [SetUp]
        public void Arrange()
        {
            _mockFeatureToggle = new Mock<IFeatureToggle>();
            MockFeatureToggleService.Setup(x => x.Get<Transfers>()).Returns(_mockFeatureToggle.Object);

            _sendAsyncResponse = new GetAccountTransferConnectionsResponse
            {
                TransferConnections = new List<TransferConnection> { new TransferConnection() }
            };
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransferConnectionsRequest>())).ReturnsAsync(_sendAsyncResponse);
        }

        [Test]
        public async Task ShouldNotCallMediatorToCreateButReturnEmptyListIfFeatureToggleIsOff()
        {
            //Arrange
            _mockFeatureToggle.Setup(x => x.FeatureEnabled).Returns(false);

            //Act
            var list = await EmployerCommitmentOrchestrator.GetTransferringEntities(HashedAccountId, UserId);

            //Assert
            Assert.IsEmpty(list.Data.TransferConnections);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetAccountTransferConnectionsRequest>()), Times.Never);
        }

        [Test]
        public async Task ShouldCallMediatorToCreateAndReturnListIfFeatureToggleIsOn()
        {
            //Arrange
            _mockFeatureToggle.Setup(x => x.FeatureEnabled).Returns(true);

            //Act
            var list = await EmployerCommitmentOrchestrator.GetTransferringEntities(HashedAccountId, UserId);

            //Assert
            Assert.IsNotEmpty(list.Data.TransferConnections);
            Assert.AreEqual(1, list.Data.TransferConnections.Count());
            MockMediator.Verify(x => x.SendAsync(It.Is<GetAccountTransferConnectionsRequest>(c => 
                c.HashedAccountId == HashedAccountId && c.UserId == UserId)), Times.Once);
        }
    }
}
