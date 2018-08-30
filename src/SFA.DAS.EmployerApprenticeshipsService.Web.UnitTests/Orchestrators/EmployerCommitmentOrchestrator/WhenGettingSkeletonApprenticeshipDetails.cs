using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingSkeletonApprenticeshipDetails : OrchestratorTestBase
    {
        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            CommitmentView.TransferSender = new TransferSender { Id = 123 };

            await EmployerCommitmentOrchestrator.GetSkeletonApprenticeshipDetails("HashedAccId", "ExtUserId", "HashedCmtId");

            MockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => !r.IncludeFrameworks)), Times.Once);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            CommitmentView.TransferSender = null;

            await EmployerCommitmentOrchestrator.GetSkeletonApprenticeshipDetails("HashedAccId", "ExtUserId", "HashedCmtId");

            MockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => r.IncludeFrameworks)), Times.Once);
        }

        [TestCase(TransferApprovalStatus.Rejected, true)]
        [TestCase(TransferApprovalStatus.Pending, false)]
        public async Task ThenCohortTransferRejectionIsIndicated(TransferApprovalStatus status, bool expectRejectionIndicated)
        {
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = status
            };

            var viewModel = await EmployerCommitmentOrchestrator.GetSkeletonApprenticeshipDetails("HashedAccId", "ExtUserId", "HashedCmtId");

            Assert.AreEqual(expectRejectionIndicated, viewModel.Data.Apprenticeship.IsInTransferRejectedCohort);
        }
    }
}
