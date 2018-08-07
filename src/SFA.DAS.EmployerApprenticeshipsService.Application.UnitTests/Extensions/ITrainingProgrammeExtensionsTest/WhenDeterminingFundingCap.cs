using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Extensions.ITrainingProgrammeExtensionsTest
{
    [TestFixture]
    public class WhenDeterminingFundingCap
    {
        private Mock<ITrainingProgramme> _course;

        [SetUp]
        public void Arrange()
        {
            _course = new Mock<ITrainingProgramme>();
            _course.Setup(x => x.EffectiveFrom).Returns(new DateTime(2018, 03, 01));
            _course.Setup(x => x.EffectiveTo).Returns(new DateTime(2019, 03, 31));
            _course.Setup(x => x.FundingPeriods).Returns(new List<FundingPeriod>
            {
                new FundingPeriod
                {
                    EffectiveFrom = new DateTime(2018,03,01),
                    EffectiveTo = new DateTime(2018,07,31),
                    FundingCap = 5000
                },
                new FundingPeriod
                {
                    EffectiveFrom = new DateTime(2018,08,01),
                    EffectiveTo = null,
                    FundingCap = 2000
                }
            });
        }

        [TestCase("2018-05-15", 5000, Description = "Within first funding band")]
        [TestCase("2018-09-15", 2000, Description = "Within second funding band")]
        [TestCase("2018-01-01", 0, Description = "Before course start")]
        [TestCase("2019-06-01", 0, Description = "After course end")]
        public void ThenTheApplicableFundingPeriodIsUsed(DateTime effectiveDate, int expectCap)
        {
            //Act
            var result = _course.Object.FundingCapOn(effectiveDate);

            //Assert
            Assert.AreEqual(expectCap, result);
        }

        [Test]
        public void IfThereAreNoFundingPeriodsThenCapShouldBeZero()
        {
            //Arrange
            _course.Setup(x => x.FundingPeriods).Returns(new List<FundingPeriod>());

            //Act
            var result = _course.Object.FundingCapOn(new DateTime(2018, 05, 15));

            //Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FundingPeriodsAreEffectiveUntilTheEndOfTheDay()
        {
            //Act
            var result = _course.Object.FundingCapOn(new DateTime(2018, 7, 31, 23, 59, 59));

            //Assert
            Assert.AreEqual(5000, result);
        }
    }
}
