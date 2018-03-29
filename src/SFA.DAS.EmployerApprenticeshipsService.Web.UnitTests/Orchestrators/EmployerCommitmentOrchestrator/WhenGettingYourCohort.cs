using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
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

        [TestCase(/*expectedDraftCount=*/1, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            1L, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, TestName = "With receiving employer")]
        [TestCase(/*expectedDraftCount=*/1, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            1L, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.None, TestName = "With provider")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/1,
            1L, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.Both, LastAction.None, TestName = "With sender but not yet actioned by them")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/1,
            1L, TransferApprovalStatus.Rejected, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            1L, TransferApprovalStatus.Approved, AgreementStatus.NotAgreed,
            EditStatus.Both, LastAction.None, TestName = "Approved by all 3 parties")]
        public async Task ThenCountsShouldBeCorrectWhenEmployerHasASingleCommitmentThats(
            int expectedDraftCount, int expectedReadyForReviewCount, int expectedWithProviderCount, int expectedTransferFundedCohortsCount,
            long? transferSenderId, TransferApprovalStatus transferApprovalStatus,
            AgreementStatus agreementStatus, EditStatus editStatus, LastAction lastAction)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQuery>()))
                .ReturnsAsync(new GetCommitmentsResponse
                {
                    Commitments = new List<CommitmentListItem>
                    {
                        new CommitmentListItem
                        {
                            TransferSenderId = transferSenderId,
                            TransferApprovalStatus = transferApprovalStatus,
                            AgreementStatus = agreementStatus,
                            EditStatus = editStatus,
                            LastAction = lastAction
                        }
                    }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetYourCohorts("ABC123", "ABC321");

            //Assert
            Assert.AreEqual(expectedDraftCount, result.Data.DraftCount);
            Assert.AreEqual(expectedReadyForReviewCount, result.Data.ReadyForReviewCount);
            Assert.AreEqual(expectedWithProviderCount, result.Data.WithProviderCount);
            Assert.AreEqual(expectedTransferFundedCohortsCount, result.Data.TransferFundedCohortsCount);
        }

    }
}
