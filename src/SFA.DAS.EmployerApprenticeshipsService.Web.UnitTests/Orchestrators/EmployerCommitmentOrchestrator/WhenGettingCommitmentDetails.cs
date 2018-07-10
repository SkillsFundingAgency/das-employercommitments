using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetFrameworks;
using SFA.DAS.EmployerCommitments.Application.Queries.GetStandards;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingCommitmentDetails : OrchestratorTestBase
    {
        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender { Id = 123, TransferApprovalStatus = TransferApprovalStatus.Pending };

        await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()), Times.Once);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Never);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            CommitmentView.TransferSender = null;

            await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Once);
        }

        [TestCase(EditStatus.EmployerOnly, false)]
        [TestCase(EditStatus.ProviderOnly, true)]
        public async Task ThenPageTitleShouldBeDictatedByEditStatus(EditStatus editStatus, bool expectReadOnly)
        {
            CommitmentView.AgreementStatus = AgreementStatus.NotAgreed;
            CommitmentView.EditStatus = editStatus;

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual(expectReadOnly, result.Data.IsReadOnly);
        }

        [Test]
        public async Task ThenIfTheCohortIsPendingTransferApprovalThenItIsReadOnly()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Pending
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsTrue(result.Data.IsReadOnly);
        }

        [Test]
        public async Task ThenPageTitleForTransferPendingShouldBeReviewYourCohort()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Pending
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual("Review your cohort", result.Data.PageTitle);            
        }

        [Test]
        public async Task ThenPageTitleForTransferRejectedShouldBeReviewYourCohort()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Rejected
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual("Review your cohort", result.Data.PageTitle);
        }

        [Test]
        public async Task ThenDeleteButtonShouldBeVisible()
        {
            CommitmentView.AgreementStatus = AgreementStatus.NotAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsFalse(result.Data.HideDeleteButton);
        }

        [TestCase(TransferApprovalStatus.Pending)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public async Task ThenIfTheCohortIsFundedByATransferThenDeleteButtonShouldBeHidden(TransferApprovalStatus status)
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;
            CommitmentView.TransferSender = new TransferSender
            {
                Id = 123,
                TransferApprovalStatus = status
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsTrue(result.Data.HideDeleteButton);
        }

        [Test]
        public void ThenIfTheCohortWasApprovedByTransferSenderThenShouldThrowAnException()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Approved
            };

            Assert.ThrowsAsync<InvalidStateException>(() =>
                EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId"));
        }

        [Test]
        public void ThenIfCohortIsNotForTransferThenIfApprovedByBothPartiesShouldThrowAnException()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = null;

            Assert.ThrowsAsync<InvalidStateException>(() =>
                EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId"));
        }
    }
}
