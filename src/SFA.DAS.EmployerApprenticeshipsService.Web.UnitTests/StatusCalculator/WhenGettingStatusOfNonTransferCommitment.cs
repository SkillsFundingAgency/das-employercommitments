using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenGettingStatusOfNonTransferCommitment
    {
        // either should text GetStatus() extension method and have CommitmentStatusCalculator internal,
        // or even better test it indirectly though the new orchestration unit tests
        private static readonly CommitmentStatusCalculator Calculator = new CommitmentStatusCalculator();

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
            var status = Calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction, agreementStatus, null, null);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.NewRequest, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 0, LastAction.None, TestName = "Employer creates a new cohort")]
        [TestCase(RequestStatus.NewRequest, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 2, LastAction.None, TestName = "Employer adds an apprentice")]
        [TestCase(RequestStatus.NewRequest, AgreementStatus.NotAgreed, EditStatus.EmployerOnly, 2, LastAction.None, TestName = "Employer saves for later")]
        [TestCase(RequestStatus.SentForReview, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 2, LastAction.Amend, TestName = "Provider adds apprentice")]
        public void EmployerCreatesANewCohort(RequestStatus expectedResult, AgreementStatus agreementStatus, EditStatus editStatus, int numberOfApprenticeships, LastAction lastAction)
        {
            // Scenario 2
            var status = Calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction, agreementStatus, null, null);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.WithProviderForApproval, AgreementStatus.EmployerAgreed, EditStatus.ProviderOnly, 2, LastAction.Approve, TestName = "Employer approves already approved commitment")]
        [TestCase(RequestStatus.WithProviderForApproval, AgreementStatus.NotAgreed, EditStatus.ProviderOnly, 2, LastAction.Approve, TestName = "Sent for approval but changed by provider")]
        [TestCase(RequestStatus.Approved, AgreementStatus.BothAgreed, EditStatus.Both, 2, LastAction.Approve, TestName = "Provider approves already approved commitment")]
        [TestCase(RequestStatus.SentForReview, AgreementStatus.ProviderAgreed, EditStatus.ProviderOnly, 2, LastAction.Amend, TestName = "Sent for review that was approved by provider")]
        public void Scenario3(RequestStatus expectedResult, AgreementStatus agreementStatus, EditStatus editStatus, int numberOfApprenticeships, LastAction lastAction)
        {
            // Scenario 3
            var status = Calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction, agreementStatus, null, null);

            status.Should().Be(expectedResult);
        }
    }
} 
