using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitments;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    // probably shouldn't test this directly, but need to test it as refactoring
    [TestFixture]
    public class WhenCheckingForAnyCohortsForCurrentStatus : OrchestratorTestBase
    {
        [TestCase(false, new RequestStatus[] {}, new[] {RequestStatus.WithProviderForApproval})]
        [TestCase(false, new[] {RequestStatus.WithProviderForApproval}, new RequestStatus[] {})]
        [TestCase(true,  new[] {RequestStatus.WithProviderForApproval}, new[] {RequestStatus.WithProviderForApproval})]
        [TestCase(true,  new[] {RequestStatus.ReadyForReview},          new[] {RequestStatus.ReadyForReview})]
        [TestCase(false, new[] {RequestStatus.WithProviderForApproval}, new[] {RequestStatus.NewRequest})]
        [TestCase(false, new[] {RequestStatus.ReadyForReview},          new[] {RequestStatus.SentToProvider})]
        [TestCase(true,  new[] {RequestStatus.WithProviderForApproval, RequestStatus.ReadyForReview}, new[] {RequestStatus.WithProviderForApproval})]
        [TestCase(true,  new[] {RequestStatus.WithProviderForApproval, RequestStatus.ReadyForReview}, new[] {RequestStatus.ReadyForReview})]
        [TestCase(true,  new[] {RequestStatus.WithProviderForApproval, RequestStatus.ReadyForReview}, new[] {RequestStatus.WithProviderForApproval, RequestStatus.ReadyForReview})]
        [TestCase(false, new[] {RequestStatus.WithProviderForApproval, RequestStatus.ReadyForReview}, new[] {RequestStatus.NewRequest})]
        [TestCase(false, new[] {RequestStatus.WithProviderForApproval, RequestStatus.ReadyForReview}, new[] {RequestStatus.NewRequest, RequestStatus.SentForReview})]
        public async Task ThenCorrectResultIsReturned(bool expectedResult, RequestStatus[] retrievedRequestStatuses, RequestStatus[] currentRequestStatus)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQuery>()))
                .ReturnsAsync(new GetCommitmentsResponse
                {
                    Commitments = GetTestCommitmentsOfStatus(1, retrievedRequestStatuses).ToList()
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.AnyCohortsForCurrentStatus("ABC123", currentRequestStatus);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
