using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenSettingATransferApproval : OrchestratorTestBase
    {
        private Mock<IFeatureToggle> _mockFeatureToggle;
        private GetAccountTransferConnectionsResponse _sendAsyncResponse;
        const string HashedTransferSenderId = "ABC123";
        const long TransferSenderId = 123;
        const string HashedTransferReceiverId = "ABC878";
        const long TransferReceiverId = 878;
        const string HashedCommitmentId = "ABC1234";
        const long CommitmentId = 1234;
        const string UserId = "User1";

        [SetUp]
        public void Arrange()
        {
            MockHashingService.Setup(x => x.DecodeValue(HashedTransferSenderId)).Returns(TransferSenderId);
            MockHashingService.Setup(x => x.DecodeValue(HashedTransferReceiverId)).Returns(TransferReceiverId);
            MockHashingService.Setup(x => x.DecodeValue(HashedCommitmentId)).Returns(CommitmentId);
        }

        [Test]
        public async Task ShouldCallMediator()
        {
            //Arrange
            var model = new TransferApprovalConfirmationViewModel
            {
                ApprovalConfirmed = true,
                HashedTransferReceiverAccountId = HashedTransferReceiverId
            };

            //Act
            await EmployerCommitmentOrchestrator.SetTransferApprovalStatus(HashedTransferSenderId, HashedCommitmentId, model, "UserId", "UserName", "UserEmail");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<TransferApprovalCommand>(c =>
                c.TransferSenderId == TransferSenderId && c.TransferReceiverId == TransferReceiverId &&
                c.CommitmentId == CommitmentId && c.UserName == "UserName" && c.UserEmail == "UserEmail")), Times.Once);
        }
    }
}
