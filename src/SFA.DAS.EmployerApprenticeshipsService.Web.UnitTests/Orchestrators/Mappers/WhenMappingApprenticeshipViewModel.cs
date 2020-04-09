using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
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
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = true, PaymentStatus = PaymentStatus.Active };
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

            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5), HasHadDataLockSuccess = false, PaymentStatus = PaymentStatus.Active };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new CommitmentView());

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfApprovedTransferFundedWithSuccessfulIlrSubmissionAndCourseNotYetStarted()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(3), HasHadDataLockSuccess = true, PaymentStatus = PaymentStatus.Active };
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

        [Test]
        public void ThenCompletionDateIsMapped()
        {
            var expectedCompletionDate = new DateTime(2020, 02, 20);

            var apprenticeship = new Apprenticeship { CompletionDate = expectedCompletionDate };

            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship).Result;

            Assert.AreEqual(expectedCompletionDate, viewModel.CompletionDate);
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
                PaymentStatus = PaymentStatus.Active,
                StartDate = _now.AddMonths(trainingStarted ? -1 : 1)
            };

            var commitment = new CommitmentView
            {
                TransferSender = transferSender ? new TransferSender { TransferApprovalStatus = transferApprovalStatus } : null
            };

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.IsUpdateLockedForStartDateAndCourse);
        }

        [Test]
        public async Task ThenEndpointAssessorNameIsMapped()
        {
            const string endpointAssessorName = "Bad Assess";

            var apprenticeship = new Apprenticeship { EndpointAssessorName = endpointAssessorName };

            var viewModel = await Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            Assert.AreEqual(endpointAssessorName, viewModel.EndpointAssessorName);
        }

        [TestCase(TrainingType.Standard)]
        [TestCase(TrainingType.Framework)]
        public async Task ThenTrainingTypeIsMapped(TrainingType trainingType)
        {
            var apprenticeship = new Apprenticeship { TrainingType = trainingType };

            var viewModel = await Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            Assert.AreEqual(trainingType, viewModel.TrainingType);
        }

        [TestCase(true, false, true, true, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, true, false, false, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, false, true, true, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(false, true, false, false, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, false, true, true, true, AcademicYearValidationResult.Success, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, true, AcademicYearValidationResult.Success, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, true, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(false, true, false, false, true, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, false, true, true, false, AcademicYearValidationResult.Success, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, false, AcademicYearValidationResult.Success, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, false, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(false, true, false, false, false, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        public void OfApprovedApprenticeshipThenIsEndDateLockedForUpdateShouldBeSetCorrectly(bool expected, bool unchanged,
            bool dataLockSuccess, bool isStartDateInFuture, bool isAfterLastAcademicYearFundingPeriod, AcademicYearValidationResult academicYearValidationResult)
        {
            AcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(isAfterLastAcademicYearFundingPeriod);
            AcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(academicYearValidationResult);

            var apprenticeship = new Apprenticeship
            {
                PaymentStatus = PaymentStatus.Active,
                HasHadDataLockSuccess = dataLockSuccess,
                StartDate = _now.AddMonths(isStartDateInFuture ? 1 : -1)
            };

            var commitment = new CommitmentView {AgreementStatus = AgreementStatus.BothAgreed};

            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.IsEndDateLockedForUpdate);
            if (unchanged)
                Assert.AreEqual(viewModel.IsLockedForUpdate, viewModel.IsEndDateLockedForUpdate);
        }
    }
}