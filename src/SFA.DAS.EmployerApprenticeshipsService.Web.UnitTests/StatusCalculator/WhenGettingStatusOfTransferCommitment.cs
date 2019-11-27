using System;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenGettingStatusOfTransferCommitment
    {
        [TestCase(RequestStatus.NewRequest, EditStatus.EmployerOnly, TransferApprovalStatus.Pending, LastAction.None, TestName = "With receiving employer")]
        [TestCase(RequestStatus.SentForReview, EditStatus.ProviderOnly, TransferApprovalStatus.Pending, LastAction.Amend, TestName = "With provider")]
        [TestCase(RequestStatus.WithSenderForApproval, EditStatus.Both, TransferApprovalStatus.Pending , LastAction.None, TestName = "With sender but not yet actioned by them")]
        [TestCase(RequestStatus.ReadyForReview, EditStatus.EmployerOnly, TransferApprovalStatus.Rejected, LastAction.None, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(RequestStatus.None, EditStatus.Both, TransferApprovalStatus.Approved, LastAction.None, TestName = "Approved by all 3 parties")]
        [TestCase(RequestStatus.ReadyForReview, EditStatus.EmployerOnly, TransferApprovalStatus.Pending, LastAction.AmendAfterRejected, TestName = "Returned to ready for review following edit of rejected cohort")]
        public void CommitmentIsTransferFundedAndInValidState(RequestStatus expectedResult, EditStatus editStatus, TransferApprovalStatus transferApprovalStatus, LastAction lastAction)
        {
            var commitment = new CommitmentListItem
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                ApprenticeshipCount = 1,
                LastAction = lastAction,
                EditStatus = editStatus,
                TransferSenderId = 1,
                TransferApprovalStatus = transferApprovalStatus
            };

            var status = commitment.GetStatus();

            Assert.AreEqual(expectedResult, status);
        }

        [TestCase(TransferApprovalStatus.Approved, EditStatus.EmployerOnly, TestName = "If sender approved, must be approved by receiver and provider (not editable by employer)")]
        [TestCase(TransferApprovalStatus.Approved, EditStatus.ProviderOnly, TestName = "If sender approved, must be approved by receiver and provider (not editable by provider)")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.Both, TestName = "If rejected by sender, must be with receiver, not approved by receiver and provider")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.ProviderOnly, TestName = "If rejected by sender, must be with receiver, not provider")]
        public void CommitmentIsTransferFundedAndInInvalidState(TransferApprovalStatus transferApprovalStatus, EditStatus editStatus)
        {
            var commitment = new CommitmentListItem
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                ApprenticeshipCount = 1,
                LastAction = LastAction.None,
                EditStatus = editStatus,
                TransferSenderId = 1,
                TransferApprovalStatus = transferApprovalStatus
            };

            Assert.Throws<InvalidStateException>(() => commitment.GetStatus());
        }

        [TestCase((TransferApprovalStatus)3, EditStatus.ProviderOnly, TestName = "TransferApprovalStatus bogus")]
        [TestCase(TransferApprovalStatus.Approved, EditStatus.Neither, TestName = "Unused EditStatus Neither 1")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.Neither, TestName = "Unused EditStatus Neither 2")]
        [TestCase(TransferApprovalStatus.Pending, EditStatus.Neither, TestName = "Unused EditStatus Neither 3")]
        [TestCase(TransferApprovalStatus.Approved, (EditStatus)4, TestName = "EditStatus bogus")]
        public void CommitmentIsTransferFundedAndStatusesAreInvalid(TransferApprovalStatus transferApprovalStatus, EditStatus editStatus)
        {
            var commitment = new CommitmentListItem
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                ApprenticeshipCount = 1,
                LastAction = LastAction.None,
                EditStatus = editStatus,
                TransferSenderId = 1,
                TransferApprovalStatus = transferApprovalStatus
            };

            Assert.Throws<Exception>(() => commitment.GetStatus());
        }
    }
}
