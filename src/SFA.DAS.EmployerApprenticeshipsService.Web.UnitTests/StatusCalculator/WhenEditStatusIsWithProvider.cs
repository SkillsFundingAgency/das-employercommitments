﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithProvider
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.ProviderAgreed, TestName = "Sent for review that was approved by provider")]
        [TestCase(RequestStatus.WithProviderForApproval, LastAction.Approve, AgreementStatus.NotAgreed, TestName = "Sent for approval but changed by provider")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var status = _calculator.GetStatus(EditStatus.ProviderOnly, 2, lastAction, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
