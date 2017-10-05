using System;
using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
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

            MockDateTime.Setup(m => m.Now).Returns(_now);
        }

        [Test]
        public void ShouldNotHaveLockedStatusIfNoDataLocksFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1) };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, new List<DataLockStatus>());

            viewModel.IsLockedForUpdated.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldNotHaveLockedStatusIfNoDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1) };
            var dataLocks = new List<DataLockStatus> { new DataLockStatus { ErrorCode = DataLockErrorCode.Dlock03 } };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, dataLocks);

            viewModel.IsLockedForUpdated.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1) };
            var dataLocks = new List<DataLockStatus>
                                {
                                    new DataLockStatus { ErrorCode = DataLockErrorCode.None }
                                };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, dataLocks);

            viewModel.IsLockedForUpdated.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }


        [Test]
        public void ShouldHaveLockedStatusIfAtLeastOneDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1) };
            var dataLocks = new List<DataLockStatus>
                                {
                                    new DataLockStatus { ErrorCode = DataLockErrorCode.Dlock04 },
                                    new DataLockStatus { ErrorCode = DataLockErrorCode.None },
                                    new DataLockStatus { ErrorCode = DataLockErrorCode.Dlock07 }
                                };
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, dataLocks);

            viewModel.IsLockedForUpdated.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfPastCutOffDate()
        {
            AcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(true);
            AcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5) };
            var dataLocks = new List<DataLockStatus>();
            var viewModel = Sut.MapToApprenticeshipViewModel(apprenticeship, dataLocks);

            viewModel.IsLockedForUpdated.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }
    }
}