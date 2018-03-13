﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetFrameworks;
using SFA.DAS.EmployerCommitments.Application.Queries.GetStandards;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingApprenticeship : OrchestratorTestBase
    {
        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            CommitmentView.TransferSenderInfo = new TransferSenderInfo { TransferSenderId = 123 };

            await EmployerCommitmentOrchestrator.GetApprenticeship("HashedAccId", "ExtUserId", "HashedCmtId", "AppId");

            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()), Times.Once);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Never);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            CommitmentView.TransferSenderInfo = null;

            await EmployerCommitmentOrchestrator.GetApprenticeship("HashedAccId", "ExtUserId", "HashedCmtId", "AppId");

            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Once);
        }
    }
}
