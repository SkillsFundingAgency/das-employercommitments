using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingAcknowledgementModelForExistingCommitment : OrchestratorTestBase
    {
        [Test]
        public async Task ThenIsTransferShouldBeSetWhenTransferSenderIdHasValue()
        {
            CommitmentView.TransferSender = new TransferSender {Id = 1};

            //Act
            var result = await EmployerCommitmentOrchestrator.GetAcknowledgementModelForExistingCommitment("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.IsTrue(result.Data.IsTransfer);
        }

        [Test]
        public async Task ThenIsTransferShouldntBeSetWhenTransferSenderIdHasNoValue()
        {
            //Act
            var result = await EmployerCommitmentOrchestrator.GetAcknowledgementModelForExistingCommitment("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.IsFalse(result.Data.IsTransfer);
        }

        [TestCase(AgreementStatus.BothAgreed, true)]
        [TestCase(AgreementStatus.EmployerAgreed, false)]
        [TestCase(AgreementStatus.ProviderAgreed, false)]
        [TestCase(AgreementStatus.NotAgreed, false)]
        public async Task ThenIsSecondApprovalShouldBeSetCorrectly(AgreementStatus agreementStatus, bool expectedIsSecondApproval)
        {
            CommitmentView.AgreementStatus = agreementStatus;

            var result = await EmployerCommitmentOrchestrator.GetAcknowledgementModelForExistingCommitment("ABC123", "XYZ123", "ABC321");

            Assert.AreEqual(expectedIsSecondApproval, result.Data.IsSecondApproval);
        }
    }
}