using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingApprenticeship : OrchestratorTestBase
    {
        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            CommitmentView.TransferSender = new TransferSender { Id = 123 };

            await EmployerCommitmentOrchestrator.GetApprenticeship("HashedAccId", "ExtUserId", "HashedCmtId", "AppId");

            MockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => !r.IncludeFrameworks)), Times.Once);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            CommitmentView.TransferSender = null;

            await EmployerCommitmentOrchestrator.GetApprenticeship("HashedAccId", "ExtUserId", "HashedCmtId", "AppId");

            MockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => r.IncludeFrameworks)), Times.Once);
        }
    }
}
