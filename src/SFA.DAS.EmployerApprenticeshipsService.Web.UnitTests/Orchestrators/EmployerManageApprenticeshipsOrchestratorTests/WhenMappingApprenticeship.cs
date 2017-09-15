using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenMappingApprenticeship : ManageApprenticeshipsOrchestratorTestBase
    {
       
        [TestCase(8, 5, arg3: 10)]
        [TestCase(8, 5, arg3: 9)]
        public async Task ShouldSetStatusTextForApprenticeshipNotStarted(int nowMonth, int nowDay, int startMonth)
        {
            MockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, nowMonth, nowDay));
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                                  {
                                      Apprenticeship = 
                                        new Apprenticeship
                                        {
                                            PaymentStatus = PaymentStatus.Active,
                                            StartDate = new DateTime(1998, startMonth, 1)
                                        }
                                  });
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            MockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);

            result.Data.Status.Should().Be("Waiting to start");
        }

        [TestCase(8, 5, arg3: 7)]
        [TestCase(8, 1, arg3: 2)]
        [TestCase(8, 1, arg3: 8, Description = "Start date is the same month as now")]
        [TestCase(8, 5, arg3: 8)]
        public async Task ShouldSetStatusTextForApprenticeshipStarted(int nowMonth, int nowDay, int startMonth)
        {
            MockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, nowMonth, nowDay));
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                                        new Apprenticeship
                                        {
                                            PaymentStatus = PaymentStatus.Active,
                                            StartDate = new DateTime(1998, startMonth, 1)
                                        }
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            MockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);

            result.Data.Status.Should().Be("Live");
        }

        [Test]
        public async Task ShouldSetStatusTextForApprenticeshipWhenPaused()
        {
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship { PaymentStatus = PaymentStatus.Paused }
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");

            result.Data.Status.Should().Be("Paused");
        }

        [Test]
        public async Task ShouldSetStatusTextForApprenticeshipWhenStoped()
        {
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship { PaymentStatus = PaymentStatus.Withdrawn }
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");

            result.Data.Status.Should().Be("Stopped");
        }

        [Test]
        public async Task ShouldSetStatusTextForApprenticeshipWhenCompleted()
        {
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship { PaymentStatus = PaymentStatus.Completed }
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");

            result.Data.Status.Should().Be("Finished");
        }

        [Test]
        public async Task ShouldSetRecordStatusTextForApprenticeshipWithUpdateWaitingForApproval()
        {
            MockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, 12, 8));
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                                        new Apprenticeship
                                        {
                                            PaymentStatus = PaymentStatus.Active,
                                            StartDate = new DateTime(1998, 11, 1),
                                            PendingUpdateOriginator = Originator.Employer
                                        }
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse
                                  {
                                      ApprenticeshipUpdate = 
                                          new ApprenticeshipUpdate
                                          {
                                              ApprenticeshipId = 1L,
                                              Originator = Originator.Employer
                                          }
                                  });

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            MockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);
            result.Data.PendingChanges.Should().Be(PendingChanges.WaitingForApproval);
        }

        [Test]
        public async Task ShouldSetRecordStatusTextForApprenticeshipWithUpdateReadyForReview()
        {
            MockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, 12, 8));
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                        new Apprenticeship {
                            PaymentStatus = PaymentStatus.Active
                          , StartDate = new DateTime(1998, 11, 1)
                          , PendingUpdateOriginator = Originator.Provider}
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse
                {
                    ApprenticeshipUpdate =
                        new ApprenticeshipUpdate {ApprenticeshipId = 1L, Originator = Originator.Provider }
                });

            var result = await Orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            MockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);
            result.Data.PendingChanges.Should().Be(PendingChanges.ReadyForApproval);
        }

    }
}
