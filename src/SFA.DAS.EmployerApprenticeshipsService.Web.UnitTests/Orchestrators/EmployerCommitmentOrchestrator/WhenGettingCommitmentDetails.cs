using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingCommitmentDetails : OrchestratorTestBase
    {
        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender { Id = 123, TransferApprovalStatus = TransferApprovalStatus.Pending };

            await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            MockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => !r.IncludeFrameworks)), Times.Once);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            CommitmentView.TransferSender = null;

            await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            MockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => r.IncludeFrameworks)), Times.Once);
        }

        [TestCase(EditStatus.EmployerOnly, false)]
        [TestCase(EditStatus.ProviderOnly, true)]
        public async Task ThenPageTitleShouldBeDictatedByEditStatus(EditStatus editStatus, bool expectReadOnly)
        {
            CommitmentView.AgreementStatus = AgreementStatus.NotAgreed;
            CommitmentView.EditStatus = editStatus;

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual(expectReadOnly, result.Data.IsReadOnly);
        }

        [Test]
        public async Task ThenIfTheCohortIsPendingTransferApprovalThenItIsReadOnly()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Pending
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsTrue(result.Data.IsReadOnly);
        }

        [Test]
        public async Task ThenPageTitleForTransferPendingShouldBeReviewYourCohort()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Pending
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual("Review your cohort", result.Data.PageTitle);            
        }

        [Test]
        public async Task ThenPageTitleForTransferRejectedShouldBeReviewYourCohort()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Rejected
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual("Review your cohort", result.Data.PageTitle);
        }

        [Test]
        public async Task ThenDeleteButtonShouldBeVisible()
        {
            CommitmentView.AgreementStatus = AgreementStatus.NotAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsFalse(result.Data.HideDeleteButton);
        }

        [TestCase(TransferApprovalStatus.Pending)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public async Task ThenIfTheCohortIsFundedByATransferThenDeleteButtonShouldBeHidden(TransferApprovalStatus status)
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.EmployerOnly;
            CommitmentView.TransferSender = new TransferSender
            {
                Id = 123,
                TransferApprovalStatus = status
            };

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsTrue(result.Data.HideDeleteButton);
        }

        [Test]
        public void ThenIfTheCohortWasApprovedByTransferSenderThenShouldThrowAnException()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Approved
            };

            Assert.ThrowsAsync<InvalidStateException>(() =>
                EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId"));
        }

        [Test]
        public void ThenIfCohortIsNotForTransferThenIfApprovedByBothPartiesShouldThrowAnException()
        {
            CommitmentView.AgreementStatus = AgreementStatus.BothAgreed;
            CommitmentView.EditStatus = EditStatus.Both;
            CommitmentView.TransferSender = null;

            Assert.ThrowsAsync<InvalidStateException>(() =>
                EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId"));
        }

        [Test]
        public async Task AndApprenticeshipIsOverFundingLimitThenACostWarningShouldBeAddedToViewModel()
        {
            CommitmentView.Apprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship
                {
                    StartDate = new DateTime(2020,2,2),
                    Cost = 500
                }
            };

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse
                {
                    TrainingProgrammes = new List<TrainingProgramme>
                    {
                        new TrainingProgramme
                        {
                            FundingPeriods = new List<TrainingProgrammeFundingPeriod>
                            {
                                new TrainingProgrammeFundingPeriod
                                {
                                    EffectiveFrom = new DateTime(2020, 2, 1),
                                    EffectiveTo = new DateTime(2020, 3, 1),
                                    FundingCap = 100
                                }
                            },
                            EffectiveFrom = new DateTime(2020, 2, 1),
                            EffectiveTo = new DateTime(2020, 3, 1),
                            Name = "Tit"
                        }
                    }
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(new GetOverlappingApprenticeshipsQueryResponse
                {
                    Overlaps = Enumerable.Empty<ApprenticeshipOverlapValidationResult>()
                });

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new[] {new KeyValuePair<string,string>("0", "Cost for Tit") }, result.Data.Warnings.AsEnumerable()));
        }

        [Test]
        public async Task ThenCommitmentDetailsShouldBeReturned()
        {
            MockHashingService.Setup(h => h.HashValue(CommitmentView.Id)).Returns("HashedCmtId");

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");

            Assert.AreEqual("HashedAccId", result.Data.HashedAccountId);
            Assert.AreEqual("HashedCmtId", result.Data.HashedId);
            Assert.AreEqual(CommitmentView.AccountLegalEntityPublicHashedId, result.Data.AccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenIfCohortIsFundedByTransferThenTransferSenderPublicHashedIdShouldBeReturned()
        {
            CommitmentView.TransferSender = new TransferSender { Id = 123, TransferApprovalStatus = TransferApprovalStatus.Pending };
            MockPublicHashingService.Setup(h => h.HashValue(CommitmentView.TransferSender.Id.Value)).Returns("HashedTrnId");

            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetails("HashedAccId", "HashedCmtId", "ExtUserId");
            
            Assert.AreEqual("HashedTrnId", result.Data.TransferSenderPublicHashedId);
        }
    }
}
