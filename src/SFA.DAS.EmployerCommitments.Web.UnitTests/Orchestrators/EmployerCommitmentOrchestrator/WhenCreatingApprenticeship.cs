using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeship;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenCreatingApprenticeship : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldCallMediatorToCreate()
        {
            //Arrange
            var model = new ApprenticeshipViewModel
            {
                HashedAccountId = "ABC123",
                HashedCommitmentId = "ABC321"
            };

            var userName = "Bob";
            var userEmail = "test@email.com";

            //Act
            await EmployerCommitmentOrchestrator.CreateApprenticeship(model, "externalUserId", userName, userEmail);

            //Assert
            MockMediator.Verify(
                x => x.SendAsync(It.Is<CreateApprenticeshipCommand>(c => c.AccountId == 123L && c.UserId == "externalUserId" && c.UserDisplayName == userName && c.UserEmailAddress == userEmail)),
                Times.Once);
        }
    }
}
