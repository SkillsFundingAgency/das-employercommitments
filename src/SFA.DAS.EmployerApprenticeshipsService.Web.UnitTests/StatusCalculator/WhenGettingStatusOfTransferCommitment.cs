﻿using System;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenGettingStatusOfTransferCommitment
    {
        private static readonly ICommitmentStatusCalculator Calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.NewRequest, EditStatus.EmployerOnly, TransferApprovalStatus.Pending, TestName = "With receiving employer")]
        [TestCase(RequestStatus.NewRequest, EditStatus.ProviderOnly, TransferApprovalStatus.Pending, TestName = "With provider")]
        [TestCase(RequestStatus.WithSender, EditStatus.Both, TransferApprovalStatus.Pending, TestName = "With sender but not yet actioned by them")]
        [TestCase(RequestStatus.WithSender, EditStatus.EmployerOnly, TransferApprovalStatus.Rejected, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(RequestStatus.None, EditStatus.Both, TransferApprovalStatus.Approved, TestName = "Approved by all 3 parties")]
        public void CommitmentIsTransferFundedAndInValidState(RequestStatus expectedResult, EditStatus editStatus, TransferApprovalStatus transferApprovalStatus)
        {
            var status = Calculator.GetTransferStatus(editStatus, transferApprovalStatus);

            Assert.AreEqual(expectedResult, status);
        }

        [TestCase(TransferApprovalStatus.Approved, EditStatus.EmployerOnly, TestName = "If sender approved, must be approved by receiver and provider (not editable by employer)")]
        [TestCase(TransferApprovalStatus.Approved, EditStatus.ProviderOnly, TestName = "If sender approved, must be approved by receiver and provider (not editable by provider)")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.Both, TestName = "If rejected by sender, must be with receiver, not approved by receiver and provider")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.ProviderOnly, TestName = "If rejected by sender, must be with receiver, not provider")]
        public void CommitmentIsTransferFundedAndInInvalidState(TransferApprovalStatus transferApprovalStatus, EditStatus editStatus)
        {
            Assert.Throws<InvalidStateException>(() => Calculator.GetTransferStatus(editStatus, transferApprovalStatus));
        }

        [TestCase((TransferApprovalStatus)3, EditStatus.ProviderOnly, TestName = "TransferApprovalStatus bogus")]
        [TestCase(TransferApprovalStatus.Approved, EditStatus.Neither, TestName = "Unused EditStatus Neither 1")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.Neither, TestName = "Unused EditStatus Neither 2")]
        [TestCase(TransferApprovalStatus.Pending, EditStatus.Neither, TestName = "Unused EditStatus Neither 3")]
        [TestCase(TransferApprovalStatus.Approved, (EditStatus)4, TestName = "EditStatus bogus")]
        public void CommitmentIsTransferFundedAndStatusesAreInvalid(TransferApprovalStatus transferApprovalStatus, EditStatus editStatus)
        {
            Assert.Throws<Exception>(() => Calculator.GetTransferStatus(editStatus, transferApprovalStatus));
        }
    }
}
