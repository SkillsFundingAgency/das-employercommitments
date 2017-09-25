using System;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ApprovedApprenticeships
{
    [TestFixture]
    public class WhenValidatingAcademicYear : ApprovedApprenticeshipValidatorTestBase
    {

        [Test]
        public void ShouldPassWithEmptyModel()
        {
            var result = Sut.ValidateAcademicYear(null);
            result.Count.Should().Be(0);
        }


        [Test(Description = "After CutOff -> StartDate previous AY -> Should Fail")]
        public void ShouldNotValidateChangesToStartDateWhenAfterAcademicYearCutOffDate()
        {
            
            CurrentDateTime.Setup(m => m.Now).Returns(new DateTime(YearNow, 11, 25));
            ValidModel.StartDate = new DateTimeViewModel(1, 6, YearNow);
            var result = Sut.ValidateAcademicYear(new DateTime(YearNow, 6, 1));
            result.Count.Should().Be(1);
            string msg;
            result.TryGetValue("StartDate", out msg).Should().BeTrue();
            msg.Should().Be("The earliest start date you can use is 8 2017");
        }

        [Test(Description = "After CutOff -> StartDate this AY -> Should NOT Fail")]
        public void ShouldValidateChangesToStartDateWhenAfterAcademicYearCutOffDate()
        {
            CurrentDateTime.Setup(m => m.Now).Returns(new DateTime(YearNow, 11, 25));
            var result = Sut.ValidateAcademicYear(new DateTime(YearNow, 8, 1));
            result.Count.Should().Be(0);
        }

        [Test(Description = "Before CutOff -> StartDate previous AY -> Should NOT Fail")]
        public void ShouldNotValidateStartDatePrevAyWhenBeforeAcademicYearCutOffDate()
        {
            CurrentDateTime.Setup(m => m.Now).Returns(new DateTime(YearNow, 9, 25));
            var result = Sut.ValidateAcademicYear(new DateTime(YearNow, 6, 1));
            result.Count.Should().Be(0);
        }

        [Test(Description = "Before CutOff -> StartDate this AY -> Should NOT Fail")]
        public void ShouldValidateStartDateThisAyDateWhenBeforeAcademicYearCutOffDate()
        {
            CurrentDateTime.Setup(m => m.Now).Returns(new DateTime(YearNow, 9, 25));
            var result = Sut.ValidateAcademicYear(new DateTime(YearNow, 8, 1));
            result.Count.Should().Be(0);
        }
    }
}