﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenUserSelectsToEditStatus : ManageApprenticeshipsOrchestratorTestBase
    {
        private Apprenticeship _testApprenticeship;

        [SetUp]
        public void SetUp()
        {
            _testApprenticeship = new Apprenticeship
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = new DateTime(1988, 4, 5),
                PaymentStatus = PaymentStatus.Active,
                StartDate = DateTime.UtcNow.AddMonths(1) // Default start date a month in the future.
            };

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = _testApprenticeship
                });

        }

        [TestCase(PaymentStatus.Active)]
        [TestCase(PaymentStatus.Paused)]
        public async Task OfAnActiveOrPausedApprenticeshipThatHasNotYetStarted_ThenTheyShouldSkipSelectingChangeDateAndMoveToMadeRedundantQuestion(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipToConfirmationPage.Should().BeFalse();
            response.Data.SkipToMadeRedundantQuestion.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfPausingLiveApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Pause, "user123");

            response.Data.SkipToConfirmationPage.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfResumingApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Paused;
            _testApprenticeship.PauseDate = MockDateTime.Object.Now.AddMonths(-1);
            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Resume, "user123");

            response.Data.SkipToConfirmationPage.Should().BeTrue();
        }
       
        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfPausingWaitingToStartApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Already started

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Pause, "user123");

            response.Data.SkipToConfirmationPage.Should().BeTrue();
        }
       
        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfResumingWaitingToStartApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Paused;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Already started

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Resume, "user123");

            response.Data.SkipToConfirmationPage.Should().BeTrue();
        }
       
        [TestCase(PaymentStatus.Active)]
        [TestCase(PaymentStatus.Paused)]
        public async Task ThenShouldNotSkipSelectingChangeDateIfTrainingStarted(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1).Date;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipToConfirmationPage.Should().BeFalse();
        }


        [Test]
        public async Task ThenEarliestDateShouldBeApprenticeshipStartDate()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1).Date;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.ApprenticeStartDate.Should().Be(_testApprenticeship.StartDate.Value);
        }

        [Test]
        public async Task IfStoppingThenStartedTrainingAndImmediateChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, null, null,"user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfStoppingThenStartedTrainingAndSpecicDateSpecifiedShouldSetDateOfChangeToSpecifiedDate()
        {
            var specifiedDate = DateTime.UtcNow.AddMonths(-1).Date;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.SpecificDate, specifiedDate, null,"user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(specifiedDate);
        }

        [Test]
        public async Task IfPausingAndStartedTrainingThenChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Pause,
                WhenToMakeChangeOptions.Immediately,
                null,null,
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfPausingAndWaitingToStartTrainingThenChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(2); // Apprenticeship is waiting to start

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Pause,
                WhenToMakeChangeOptions.Immediately,
                null,null,
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfPausingThenViewTransactionsLinkIsSet()
        {
            var expectedLink = "testLink";

            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(2); // Apprenticeship is waiting to start

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                HashedAccountId,
                "CDE321",
                ChangeStatusType.Pause,
                WhenToMakeChangeOptions.Immediately,
                null,null,
                "user123");

            response.Data.ViewTransactionsLink.Should().Be(expectedLink);
        }

    }
}
