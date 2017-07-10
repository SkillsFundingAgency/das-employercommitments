using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Commands.DeleteApprentice;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenDeletingApprenticeship : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldCallMediatorToDelete()
        {
            //Arrange
            var model = new DeleteApprenticeshipConfirmationViewModel
            {
                HashedAccountId = "ABC123",
                HashedCommitmentId = "ABC321",
                HashedApprenticeshipId = "ABC456"
            };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            MockMediator.Setup(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>())).ReturnsAsync(new Unit());

            var expectedName = "Bob";
            var expectedEmailAddress = "test@email.com";

            //Act
            await EmployerCommitmentOrchestrator.DeleteApprenticeship(model, "externalUserId", expectedName, expectedEmailAddress);

            //Assert
            MockMediator.Verify(
                x =>
                    x.SendAsync(
                        It.Is<DeleteApprenticeshipCommand>(
                            c => c.AccountId == 123L && c.ApprenticeshipId == 456L && c.UserId == "externalUserId" && c.UserDisplayName == expectedName && c.UserEmailAddress == expectedEmailAddress)),
                Times.Once);
        }
    }
}
