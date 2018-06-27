using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
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

            viewModel.IsLockedForUpdate.Should().BeFalse();
            viewModel.IsEndDateLockedForUpdate.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfAtLeastOneDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = true };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfPastCutOffDate()
        {
            AcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(true);
            AcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5), HasHadDataLockSuccess = false };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfApprovedTransferFundedWithSuccessfulIlrSubmissionAndCourseNotYetStarted()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(3), HasHadDataLockSuccess = true };
            var commitment = new CommitmentView {TransferSender = new TransferSender {TransferApprovalStatus = TransferApprovalStatus.Approved}};

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
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
        public void ShouldNotHaveITransferFlagSetIfCommitmentHasNoTransferSender()
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

        [TestCase(TransferApprovalStatus.Rejected, true)]
        [TestCase(TransferApprovalStatus.Pending, false)]
        public void ThenCohortTransferRejectionIsIndicated(TransferApprovalStatus status, bool expectRejectionIndicated)
        {
            var apprenticeship = new Apprenticeship();
            var commitment = new CommitmentView
            {
                TransferSender = new TransferSender
                {
                    TransferApprovalStatus = status
                }
            };

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            Assert.AreEqual(expectRejectionIndicated, viewModel.IsInTransferRejectedCohort);
        }

        /// <remarks>
        /// trainingStarted should have no material affect, so could be excluded, but i think there is some value in testing it
        /// </remarks>
        [TestCase(true, false, true, true, TransferApprovalStatus.Approved)]
        [TestCase(false, true, true, true, TransferApprovalStatus.Approved)]
        [TestCase(false, false, true, true, TransferApprovalStatus.Pending)]
        [TestCase(false, true, true, true, TransferApprovalStatus.Pending)]
        [TestCase(false, false, true, true, TransferApprovalStatus.Rejected)]
        [TestCase(false, true, true, true, TransferApprovalStatus.Rejected)]
        [TestCase(false, false, false, true, null)]
        [TestCase(false, true, false, true, null)]
        [TestCase(true, false, true, false, TransferApprovalStatus.Approved)]
        [TestCase(false, true, true, false, TransferApprovalStatus.Approved)]
        [TestCase(false, false, true, false, TransferApprovalStatus.Pending)]
        [TestCase(false, true, true, false, TransferApprovalStatus.Pending)]
        [TestCase(false, false, true, false, TransferApprovalStatus.Rejected)]
        [TestCase(false, true, true, false, TransferApprovalStatus.Rejected)]
        [TestCase(false, false, false, false, null)]
        [TestCase(false, true, false, false, null)]
        public void ThenIsUpdateLockedForStartDateAndCourseShouldBeSetCorrectly(
            bool expected, bool dataLockSuccess, bool transferSender, bool trainingStarted, TransferApprovalStatus? transferApprovalStatus)
        {
            var apprenticeship = new Apprenticeship {
                HasHadDataLockSuccess = dataLockSuccess,
                StartDate = _now.AddMonths(trainingStarted ? -1 : 1)
            };

            var commitment = new CommitmentView
            {
                TransferSender = transferSender ? new TransferSender { TransferApprovalStatus = transferApprovalStatus } : null
            };

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.IsUpdateLockedForStartDateAndCourse);
        }

        //todo: add view unit tests for display fields flags??
        [TestCase(true, true, true, true, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(false, true, false, true, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(true, false, true, true, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(true, false, false, true, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(true, true, true, false, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(false, true, false, false, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(true, false, true, false, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(false, false, false, false, AcademicYearValidationResult.NotWithinFundingPeriod)]
        [TestCase(true, true, true, true, AcademicYearValidationResult.Success)]
        [TestCase(false, true, false, true, AcademicYearValidationResult.Success)]
        [TestCase(true, false, true, true, AcademicYearValidationResult.Success)]
        [TestCase(false, false, false, true, AcademicYearValidationResult.Success)]
        [TestCase(true, true, true, false, AcademicYearValidationResult.Success)]
        [TestCase(false, true, false, false, AcademicYearValidationResult.Success)]
        [TestCase(true, false, true, false, AcademicYearValidationResult.Success)]
        [TestCase(false, false, false, false, AcademicYearValidationResult.Success)]
        public void OfApprovedApprenticeshipThenIsEndDateLockedForUpdateShouldBeSetCorrectly(bool expected, bool dataLockSuccess, bool isStartDateInFuture, bool isAfterLastAcademicYearFundingPeriod, AcademicYearValidationResult academicYearValidationResult)
        {
            AcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(isAfterLastAcademicYearFundingPeriod);
            AcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(academicYearValidationResult);

            var apprenticeship = new Apprenticeship
            {
                HasHadDataLockSuccess = dataLockSuccess,
                StartDate = _now.AddMonths(isStartDateInFuture ? 1 : -1)
            };

            var commitment = new CommitmentView {AgreementStatus = AgreementStatus.BothAgreed};

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.IsEndDateLockedForUpdate);
        }
    }
}