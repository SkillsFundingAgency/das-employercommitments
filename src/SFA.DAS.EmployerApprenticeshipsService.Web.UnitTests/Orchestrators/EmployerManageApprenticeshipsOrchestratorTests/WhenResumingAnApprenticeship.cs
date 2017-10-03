using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public class WhenResumingAnApprenticeship
    {

     //[Test]
     //   public async Task IfResumingInSameAcademicYearAndStartedTrainingThenChangeSpecifiedShouldSetDateOfChangeToStartDate()
     //   {

     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));
     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2018, 7, 31));
     //       AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2017, 10, 18));


     //       _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started
     //       _testApprenticeship.PauseDate = DateTime.UtcNow.AddMonths(-1).AddDays(5);
     //       OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
     //           "ABC123",
     //           "CDE321",
     //           ChangeStatusType.Resume,
     //           WhenToMakeChangeOptions.Immediately,
     //           null,
     //           "user123");

     //       response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
     //   }

     //   [Test]
     //   public async Task IfResumingInSameAcademicYearAndWaitingToStartTrainingThenChangeSpecifiedShouldSetDateOfChangeToStartDate()
     //   {

     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));
     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2018, 7, 31));
     //       AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2017, 10, 18));

     //       _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate; // Apprenticeship is waiting to start
     //       _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(1);

     //       OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
     //           "ABC123",
     //           "CDE321",
     //           ChangeStatusType.Resume,
     //           WhenToMakeChangeOptions.Immediately,
     //           null,
     //           "user123");

     //       response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
     //   }


     //   [Test]
     //   public async Task IfResumingInNextAcademicYearAndStartedTrainingThenChangeSpecifiedShouldSetDateOfChangeToStartDate()
     //   {

     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));
     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2018, 7, 31));
     //       AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2017, 10, 18));


     //       _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started
     //       _testApprenticeship.PauseDate = DateTime.UtcNow.AddMonths(-1).AddDays(5);
     //       OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
     //           "ABC123",
     //           "CDE321",
     //           ChangeStatusType.Resume,
     //           WhenToMakeChangeOptions.Immediately,
     //           null,
     //           "user123");

     //       response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
     //   }

     //   [Test]
     //   public async Task IfResumingInNextAcademicYearAndWaitingToStartTrainingThenChangeSpecifiedShouldSetDateOfChangeToStartDate()
     //   {

     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));
     //       AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2018, 7, 31));
     //       AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2017, 10, 18));

     //       _testApprenticeship.StartDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate; // Apprenticeship is waiting to start
     //       _testApprenticeship.PauseDate = AcademicYearDateProvider.Object.CurrentAcademicYearStartDate.AddMonths(1);

     //       OrchestratorResponse<ConfirmationStateChangeViewModel> response = await Orchestrator.GetChangeStatusConfirmationViewModel(
     //           "ABC123",
     //           "CDE321",
     //           ChangeStatusType.Resume,
     //           WhenToMakeChangeOptions.Immediately,
     //           null,
     //           "user123");

     //       response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
     //   }



    }
}