using System.Collections.Generic;
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
        private const long ValidTransferSenderId = 1L;

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
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0, TestName = "With receiving employer")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/1, /*expectedTransferFundedCohortsCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.None, 0, TestName = "With provider")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/1,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.Both, LastAction.None, 1, TestName = "With sender but not yet actioned by them")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/1,
            ValidTransferSenderId, TransferApprovalStatus.Rejected, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 1, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Approved, AgreementStatus.NotAgreed,
            EditStatus.Both, LastAction.None, 1, TestName = "Approved by all 3 parties")]
        [TestCase(/*expectedDraftCount=*/1, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0, TestName = "Just created by an employer")]
        [TestCase(/*expectedDraftCount=*/0, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/1, /*expectedTransferFundedCohortsCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.None, 0, TestName = "Been sent to provider by employer to add apprentices")]
        public async Task ThenCountsShouldBeCorrectWhenEmployerHasASingleCommitmentThats(
            int expectedDraftCount, int expectedReadyForReviewCount, int expectedWithProviderCount, int expectedTransferFundedCohortsCount,
            long? transferSenderId, TransferApprovalStatus transferApprovalStatus,
            AgreementStatus agreementStatus, EditStatus editStatus, LastAction lastAction, int apprenticeshipCount)
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
                            LastAction = lastAction,
                            ApprenticeshipCount = apprenticeshipCount
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

        [TestCase(/*expectedDraftCount=*/2, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0, TestName = "With a transfers draft and a non-transfers draft")]
        [TestCase(/*expectedDraftCount=*/1, /*expectedReadyForReviewCount=*/0,
            /*expectedWithProviderCount=*/1, /*expectedTransferFundedCohortsCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.None, 0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0, TestName = "With a transfers draft and a non-transfers with provider")]
        public async Task ThenCountsShouldBeCorrectWhenEmployerHasTwoCommitments(
            int expectedDraftCount, int expectedReadyForReviewCount, int expectedWithProviderCount, int expectedTransferFundedCohortsCount,
            long? transferSenderId1, TransferApprovalStatus transferApprovalStatus1,
            AgreementStatus agreementStatus1, EditStatus editStatus1, LastAction lastAction1, int apprenticeshipCount1,
            long? transferSenderId2, TransferApprovalStatus transferApprovalStatus2,
            AgreementStatus agreementStatus2, EditStatus editStatus2, LastAction lastAction2, int apprenticeshipCount2)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQuery>()))
                .ReturnsAsync(new GetCommitmentsResponse
                {
                    Commitments = new List<CommitmentListItem>
                    {
                        new CommitmentListItem
                        {
                            TransferSenderId = transferSenderId1,
                            TransferApprovalStatus = transferApprovalStatus1,
                            AgreementStatus = agreementStatus1,
                            EditStatus = editStatus1,
                            LastAction = lastAction1,
                            ApprenticeshipCount = apprenticeshipCount1
                        },
                        new CommitmentListItem
                        {
                            TransferSenderId = transferSenderId2,
                            TransferApprovalStatus = transferApprovalStatus2,
                            AgreementStatus = agreementStatus2,
                            EditStatus = editStatus2,
                            LastAction = lastAction2,
                            ApprenticeshipCount = apprenticeshipCount2
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
