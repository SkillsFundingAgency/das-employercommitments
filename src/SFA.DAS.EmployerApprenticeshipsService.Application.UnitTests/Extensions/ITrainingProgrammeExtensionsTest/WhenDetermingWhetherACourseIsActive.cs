﻿using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Extensions.ITrainingProgrammeExtensionsTest
{
    [TestFixture]
    public class WhenDetermingWhetherACourseIsActive
    {
        [TestCase("2016-01-01", "2016-12-01", "2016-06-01", true, Description = "Within date range")]
        [TestCase(null, "2016-12-01", "2016-06-01", true, Description="Within date range with no defined course start date")]
        [TestCase("2016-01-01", null, "2016-06-01", true, Description = "Withing date range, with no defined course end date")]
        [TestCase(null, null, "2016-06-01", true, Description = "Within date range, with no defined course effective dates")]
        [TestCase("2016-01-01", "2016-12-01", "2015-06-01", false, Description = "Outside (before) date range")]
        [TestCase("2016-01-01", "2016-12-01", "2017-06-01", false, Description = "Outside (after) date range")]
        public void ThenIfWithinCourseEffectiveRangeThenIsActive(DateTime? courseStart, DateTime? courseEnd, DateTime effectiveDate, bool expectIsActive)
        {
            //Arrange
            var course = new Mock<ITrainingProgramme>();
            course.SetupGet(x => x.EffectiveFrom).Returns(courseStart);
            course.SetupGet(x => x.EffectiveTo).Returns(courseEnd);

            //Act
            var result = course.Object.IsActiveOn(effectiveDate);

            //Assert
            Assert.AreEqual(expectIsActive, result);
        }
    }
}
