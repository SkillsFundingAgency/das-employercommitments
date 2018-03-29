using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitments;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingYourCohort : OrchestratorTestBase
    {
        [Test]
        public async Task ThenAllCountsShouldBeZeroIfNoCommitments()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQuery>()))
                .ReturnsAsync(new GetCommitmentsResponse
                {
                    Commitments = new List<CommitmentListItem>()
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetYourCohorts("ABC123", "ABC321");

            //Assert
            Assert.AreEqual(0, result.Data.DraftCount);
            Assert.AreEqual(0, result.Data.ReadyForReviewCount);
            Assert.AreEqual(0, result.Data.WithProviderCount);
            Assert.AreEqual(0, result.Data.TransferFundedCohortsCount);
        }
    }
}
