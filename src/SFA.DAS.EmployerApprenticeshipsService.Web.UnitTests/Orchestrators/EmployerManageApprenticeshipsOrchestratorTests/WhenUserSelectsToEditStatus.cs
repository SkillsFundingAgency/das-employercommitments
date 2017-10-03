using FluentAssertions;
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
        public async Task ThenShouldSkipSelectingChangeDateIfTrainingHasNotStarted(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfPausingLiveApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Pause, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfResumingApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Paused;
            _testApprenticeship.PauseDate = MockDateTime.Object.Now.AddMonths(-1);
            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Resume, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }
       
        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfPausingWaitingToStartApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Already started

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Pause, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }
       
        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfResumingWaitingToStartApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Paused;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Already started

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Resume, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }
       
        [TestCase(PaymentStatus.Active)]
        [TestCase(PaymentStatus.Paused)]
        public async Task ThenShouldNotSkipSelectingChangeDateIfTrainingStarted(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1).Date;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipStep.Should().BeFalse();
        }


        [Test]
        public async Task ThenEarliestDateShouldBeStartDate()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1).Date;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.EarliestDate.Should().Be(_testApprenticeship.StartDate.Value);
        }
       
        [TestCase("2016-03-01", "2017-10-19 18:00:00", "2017-08-01", Description = "R14 date has passed")]
        [TestCase("2016-03-01", "2017-10-17 18:00:00", "2016-03-01", Description = "R14 date has not passed")]
        public async Task ThenIfR14DateHasPassedThenEarliestDateShouldBeStartOfAcademicYear(DateTime startDate, DateTime now, DateTime expectedEarliestDate)
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;
            _testApprenticeship.StartDate = startDate;
            MockDateTime.Setup(x => x.Now).Returns(now);

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await Orchestrator.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.EarliestDate.Should().Be(expectedEarliestDate);
        }


        [Test]
        public async Task IfStoppingThenStartedTrainingAndImmediateChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, null, "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfStoppingThenStartedTrainingAndSpecicDateSpecifiedShouldSetDateOfChangeToSpecifiedDate()
        {
            var specifiedDate = DateTime.UtcNow.AddMonths(-1).Date;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.SpecificDate, specifiedDate, "user123");

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
                null,
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
                null,
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfResumingAStartedApprenticeshipAfterPauseInLastAcademicYearAndResumeIsBeforeR14CutoffTimeThenDateOfchangeIsPauseDate()
        {

            _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-6); // Apprenticeship was started last academic year
            _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-3); // Apprenticeship was was paused last academic year

            MockDateTime.Setup(x => x.Now).Returns(AcademicYearDateProvider.Object.LastAcademicYearFundingPeriod.AddSeconds(-1)); // resume before r14 cutoff

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Resume,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");



            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should()
                .Be(_testApprenticeship.PauseDate);
        }

        [Test]
        public async Task IfResumingAStartedApprenticeshipAfterPauseInLastAcademicYearAndResumeIsAfterR14CutoffTimeThenDateOfchangeIsStartOfacademicYear()
        {
            _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-6); // Apprenticeship was started last academic year
            _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-3); // Apprenticeship was was paused last academic year

            MockDateTime.Setup(x => x.Now).Returns(AcademicYearDateProvider.Object.LastAcademicYearFundingPeriod.AddSeconds(1)); // resume after r14 cutoff
            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Resume,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");



            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(AcademicYearDateProvider.Object.CurrentAcademicYearStartDate);

        }


        [Test]
        public async Task IfResumingAnAwaitingApprenticeshipAfterPauseInLastAcademicYearAndResumeIsBeforeR14CutoffTimeThenDateOfChangeIsTodaysDate()
        {

            _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(6); // Apprenticeship was started last academic year
            _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-3); // Apprenticeship was was paused last academic year

            MockDateTime.Setup(x => x.Now).Returns(AcademicYearDateProvider.Object.LastAcademicYearFundingPeriod.AddSeconds(-1)); // resume before r14 cutoff

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Resume,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");



            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(MockDateTime.Object.Now.Date);
        }

        [Test]
        public async Task IfResumingAnAwaitingApprenticeshipAfterPauseInLastAcademicYearAndResumeIsAfterR14CutoffTimeThenDateOfChangeIsTodaysDate()
        {
            _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(6); // Apprenticeship was started last academic year
            _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-3); // Apprenticeship was was paused last academic year

            MockDateTime.Setup(x => x.Now).Returns(AcademicYearDateProvider.Object.LastAcademicYearFundingPeriod.AddSeconds(1)); // resume after r14 cutoff

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123", 
                "CDE321", 
                ChangeStatusType.Resume, 
                WhenToMakeChangeOptions.Immediately, 
                null, 
                "user123");



            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(MockDateTime.Object.Now.Date);

        }



    }
}
