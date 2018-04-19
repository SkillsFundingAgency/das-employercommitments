using System;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeshipViewModel : ApprenticeshipMapperBase
    {
        private DateTime _now;

        [SetUp]
        public void Arrange()
        {
            _now = new DateTime(DateTime.Now.Year, 11, 01);

            AcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(true);
            AcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(AcademicYearValidationResult.Success);

            MockDateTime.Setup(m => m.Now).Returns(_now);
        }

        [Test]
        public void ShouldNotHaveLockedStatusIfNoDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = false };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());
            var n = MockDateTime.Object.Now;

            n.Should().Be(_now);

            viewModel.CanUpdateStartDate.Should().BeTrue();
            viewModel.CanUpdateEndDate.Should().BeTrue();
            viewModel.CanUpdateTraining.Should().BeTrue();
            viewModel.CanUpdateCost.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfAtLeastOneDataLocksSuccessFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = true };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());

            viewModel.CanUpdateStartDate.Should().BeFalse();
            viewModel.CanUpdateEndDate.Should().BeFalse();
            viewModel.CanUpdateTraining.Should().BeFalse();
            viewModel.CanUpdateCost.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfPastCutOffDate()
        {
            AcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(true);
            AcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5), HasHadDataLockSuccess = false };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());

            viewModel.CanUpdateStartDate.Should().BeFalse();
            viewModel.CanUpdateEndDate.Should().BeFalse();
            viewModel.CanUpdateTraining.Should().BeFalse();
            viewModel.CanUpdateCost.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveTransferFlagSetIfCommitmentHasTransferSender()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(+1), HasHadDataLockSuccess = false };
            var commitment = new CommitmentView { TransferSender = new TransferSender { Id = 123 } };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            viewModel.IsPaidForByTransfer.Should().BeTrue();
        }

        [Test]
        public void ShouldNotHaveTransferFlagSetIfCommitmentHasNoTransferSender()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(+1), HasHadDataLockSuccess = false };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());

            viewModel.IsPaidForByTransfer.Should().BeFalse();
        }

        [Test]
        public void ThenULNIsMapped()
        {
            var uln = "IAMAULN";

            var apprenticeship = new Apprenticeship { ULN = uln };

            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship).Result;

            Assert.AreEqual(uln, viewModel.ULN);
        }

        [Test]
        public void ThenStopDateIsMapped()
        {
            var expectedStopDate = new DateTime(2018, 3, 3);

            var apprenticeship = new Apprenticeship { StopDate = expectedStopDate };

            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship).Result;

            Assert.AreEqual(expectedStopDate, viewModel.StopDate);
        }

        [TestCase(false, false, true, TestName = "Disabled as transfer and no successful ilr submission")]
        [TestCase(false, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ThenCanUpdateStartDateAndTrainingIsSetCorrectly(bool expected, bool dataLockSuccess, bool transferSender)
        {
            var apprenticeship = new Apprenticeship { HasHadDataLockSuccess = dataLockSuccess };
            var commitment = new CommitmentView();

            if (transferSender)
                commitment.TransferSender = new TransferSender();

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.CanUpdateStartDate);
            Assert.AreEqual(expected, viewModel.CanUpdateTraining);
        }
    }
}