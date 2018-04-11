using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitments;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingAllWithProvider : OrchestratorTestBase
    {
        [Test]
        public async Task ThenCorrectCommitmentsAreReturned()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQuery>()))
                .ReturnsAsync(new GetCommitmentsResponse
                {
                    Commitments = GetTestCommitmentsOfStatus(1,
                        RequestStatus.WithProviderForApproval,  // 1: should be returned
                        RequestStatus.SentForReview,            // 2: should be returned
                        RequestStatus.SentToProvider,           // 3: should be returned
                        RequestStatus.ReadyForReview            // 4: should not be returned
                        ).ToList()
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetAllWithProvider("ABC123", "ABC321");

            //Assert
            Assert.AreEqual(3, result.Data.Commitments.Count());

            CollectionAssert.AreEquivalent(new[] {"1", "2", "3"}, result.Data.Commitments.Select(c => c.Name));
        }
    }
}
