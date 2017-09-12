using System;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.AcademicYearTests
{
    [TestFixture]
    public class WhenValidatingAcademicYearTests
    {
        private Mock<ICurrentDateTime> _currentDateTime;

        private AcademicYearValidator _sut;

        [SetUp]
        public void Arrange()
        {
            _currentDateTime = new Mock<ICurrentDateTime>();
            _sut = new AcademicYearValidator(_currentDateTime.Object);
        }

        [TestCase("2017-09-07", "2016-07-01")]
        public void ShouldNotValidateStartDateBeforeCloseOfR14(DateTime currentDate, DateTime startDate)
        {
            _currentDateTime.Setup(x => x.Now).Returns(currentDate);
            var result = _sut.FundingPeriodOpen(startDate);
            result.Should().BeFalse();
        }

        [TestCase("2017-09-07", "2016-08-01")]
        [TestCase("2017-09-07", "2017-09-01")]
        [TestCase("2017-09-07", "2017-08-01")]
        [TestCase("2017-09-07", "2017-07-01")]
        [TestCase("2017-09-07", "2018-08-01")]
        [TestCase("2017-09-07", "2019-08-01")]
        public void ShouldValidateStartDateBeforeCloseOfR14(DateTime currentDate, DateTime startDate)
        {
            _currentDateTime.Setup(x => x.Now).Returns(currentDate);
            var result = _sut.FundingPeriodOpen(startDate);
            result.Should().BeTrue();
        }

        [TestCase("2017-10-22", "2016-07-01")]
        [TestCase("2017-10-22", "2016-08-01")]
        [TestCase("2017-10-22", "2016-10-01")]
        [TestCase("2017-10-22", "2017-02-01")]
        [TestCase("2017-10-22", "2017-07-01")]
        public void ShouldNotValidateStartDateAfterCloseOfR14(DateTime currentDate, DateTime startDate)
        {
            _currentDateTime.Setup(x => x.Now).Returns(currentDate);
            var result = _sut.FundingPeriodOpen(startDate);
            result.Should().BeFalse();
        }

        [TestCase("2017-10-22", "2017-08-01")]
        [TestCase("2017-10-22", "2017-09-01")]
        [TestCase("2017-10-22", "2017-10-01")]
        [TestCase("2017-10-22", "2018-02-01")]
        [TestCase("2017-10-22", "2018-08-01")]
        [TestCase("2017-10-22", "2019-08-01")]
        public void ShouldValidateStartDateAfterCloseOfR14(DateTime currentDate, DateTime startDate)
        {
            _currentDateTime.Setup(x => x.Now).Returns(currentDate);
            var result = _sut.FundingPeriodOpen(startDate);
            result.Should().BeTrue();
        }
    }
}
