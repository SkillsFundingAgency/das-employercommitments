using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenSettingATransferRequestApproval : OrchestratorTestBase
    {
        const string HashedTransferSenderId = "ABC123";
        const long TransferSenderId = 123;
        const string HashedCommitmentId = "ABC1234";
        const long CommitmentId = 1234;
        const string HashedTransferRequestId = "ABC1238";
        const long TransferRequestId = 1238;

        [SetUp]
        public void Arrange()
        {
            MockHashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns((string param) => Convert.ToInt64(param.Remove(0,3)));
        }

        [Test]
        public async Task ShouldCallMediator()
        {
            //Arrange
            var model = new TransferApprovalConfirmationViewModel
            {
                ApprovalConfirmed = true
            };

            //Act
            await EmployerCommitmentOrchestrator.SetTransferRequestApprovalStatus(HashedTransferSenderId, HashedCommitmentId, HashedTransferRequestId, model, "UserId", "UserName", "UserEmail");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<TransferApprovalCommand>(c =>
                c.TransferSenderId == TransferSenderId && c.CommitmentId == CommitmentId  && c.TransferRequestId == TransferRequestId && c.UserId == "UserId" && c.UserName == "UserName" && c.UserEmail == "UserEmail")), Times.Once);
        }
    }
}
