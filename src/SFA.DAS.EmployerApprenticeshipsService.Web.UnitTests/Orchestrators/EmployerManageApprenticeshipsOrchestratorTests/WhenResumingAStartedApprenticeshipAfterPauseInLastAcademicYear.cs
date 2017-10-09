using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public class WhenResumingStartedApprenticeshipAfterPauseInLastAcademicYear : ManageApprenticeshipsOrchestratorTestBase
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
        [Test]
        public async Task IfResumeIsBeforeR14CutoffTimeThenDateOfchangeIsPauseDate()
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
        public async Task IfIsAfterR14CutoffTimeThenDateOfchangeIsStartOfacademicYear()
        {
            _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-6); // Apprenticeship was started last academic year
            _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(-3); // Apprenticeship was was paused last academic year

            MockDateTime.Setup(x => x.Now).Returns(AcademicYearDateProvider.Object.LastAcademicYearFundingPeriod.AddMinutes(1)); // resume after r14 cutoff
            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Resume,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");



            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(AcademicYearDateProvider.Object.CurrentAcademicYearStartDate);

        }



    }
}
