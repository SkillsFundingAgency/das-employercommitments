﻿using System;
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
            CommitmentView.TransferSender = new TransferSender { Id = 123};

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

        [Test]
        public async Task ThenIfTheCohortHasBeenRejectedByTransferSenderThenItIsEditable()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Rejected
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsFalse(result.Data.IsReadOnly);
        }

        [Test]
        public async Task ThenIfTheCohortIsPendingTransferApprovalThenItIsReadOnly()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Pending,
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsTrue(result.Data.IsReadOnly);
        }

        [Test]
        public async Task ThenIfTheCohortWasApprovedByTransferSenderThenShouldThrownAnException()
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
    }
}
