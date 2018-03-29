﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public sealed class WhenGettingStatusOfCommitment
    {
        private static readonly ICommitmentStatusCalculator Calculator = new CommitmentStatusCalculator();

        #region GetStatus

        [TestCase(RequestStatus.SentToProvider, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 0, LastAction.None, TestName = "Employer sends to provider to add apprentices")]
        [TestCase(RequestStatus.SentToProvider, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 1, LastAction.None, TestName = "Provider adds apprenticeship")]

        [TestCase(RequestStatus.ReadyForReview, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 2, LastAction.Amend, TestName = "Provider sends to employer for amendment")]
        [TestCase(RequestStatus.ReadyForReview, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 3, LastAction.Amend, TestName = "Employer edits apprenticeship")]
        [TestCase(RequestStatus.SentForReview, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 4, LastAction.Amend, TestName = "Employer sends to provider for amendment")]

        [TestCase(RequestStatus.ReadyForApproval, AgreementStatus.ProviderAgreed, EditStatus.EmployerOnly, 5, LastAction.Approve, TestName = "Provider approves")]
        [TestCase(RequestStatus.ReadyForApproval, AgreementStatus.ProviderAgreed, EditStatus.EmployerOnly, 6, LastAction.Approve, TestName = "Employer edits apprenticeship & changes employer reference")]
        [TestCase(RequestStatus.ReadyForReview, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 7, LastAction.Approve, TestName = "Employer change apprenticeship & changes cost")] // ToDo Confirm status text

        [TestCase(RequestStatus.SentForReview, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 9, LastAction.Amend, TestName = "Provider amends")]
        [TestCase(RequestStatus.Approved, AgreementStatus.BothAgreed, EditStatus.Both, 11, LastAction.Approve, TestName = "Employer approves")]
        public void EmployerSendsToProviderToAddApprentices(RequestStatus expectedResult, AgreementStatus agreementStatus, EditStatus editStatus, int numberOfApprenticeships, LastAction lastAction)
        {
            // Scenario 1
            var status = Calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction, agreementStatus);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.NewRequest, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 0, LastAction.None, TestName = "Employer creates a new cohort")]
        [TestCase(RequestStatus.NewRequest, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 2, LastAction.None, TestName = "Employer adds an apprentice")]
        [TestCase(RequestStatus.NewRequest, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 2, LastAction.None, TestName = "Employer saves for later")]
        [TestCase(RequestStatus.SentForReview, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 2, LastAction.Amend, TestName = "Provider adds apprentice")]
        public void EmployerCreatesANewCohort(RequestStatus expectedResult, AgreementStatus agreementStatus, EditStatus editStatus, int numberOfApprenticeships, LastAction lastAction)
        {
            // Scenario 2
            var status = Calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction, agreementStatus);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.WithProviderForApproval, AgreementStatus.EmployerAgreed, EditStatus.ProviderOnly, 2, LastAction.Approve, TestName = "Employer approves already approved commitment")]
        [TestCase(RequestStatus.Approved, AgreementStatus.BothAgreed, EditStatus.Both, 2, LastAction.Approve, TestName = "Provider approves already approved commitment")]
        public void Scenario3(RequestStatus expectedResult, AgreementStatus agreementStatus, EditStatus editStatus, int numberOfApprenticeships, LastAction lastAction)
        {
            // Scenario 3
            var status = Calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction, agreementStatus);

            status.Should().Be(expectedResult);
        }

        #endregion GetStatus

        #region GetTransferStatus

        [TestCase(RequestStatus.NewRequest, EditStatus.EmployerOnly, TransferApprovalStatus.Pending, TestName = "With receiving employer")]
        [TestCase(RequestStatus.NewRequest, EditStatus.ProviderOnly, TransferApprovalStatus.Pending, TestName = "With provider")]
        [TestCase(RequestStatus.WithSender, EditStatus.Both, TransferApprovalStatus.Pending, TestName = "With sender but not yet actioned by them")]
        [TestCase(RequestStatus.WithSender, EditStatus.EmployerOnly, TransferApprovalStatus.Rejected, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(RequestStatus.None, EditStatus.Both, TransferApprovalStatus.Approved, TestName = "Approved by all 3 parties")]
        public void CommitmentIsTransferFundedAndExpectedState(RequestStatus expectedResult, EditStatus editStatus, TransferApprovalStatus transferApprovalStatus)
        {
            var status = Calculator.GetTransferStatus(editStatus, transferApprovalStatus);

            Assert.AreEqual(expectedResult, status);
        }

        #endregion GetTransferStatus
    }
} 
