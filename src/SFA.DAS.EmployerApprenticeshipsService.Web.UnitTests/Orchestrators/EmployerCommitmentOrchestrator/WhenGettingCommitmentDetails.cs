﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
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
            CommitmentView.TransferSenderId = 123;

            await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()), Times.Once);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Never);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            CommitmentView.TransferSenderId = default(long?);

            await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()), Times.Never);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Once);
        }

    }
}