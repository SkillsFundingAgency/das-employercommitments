using System;
using System.Linq;
using Castle.Core.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture()]
    public class WhenValidatingStartDate : ApprenticeshipValidationTestBase
    {
        [Test]
        public void IfTheTrainingProgrammeIsValidThenShouldPassValidation()
        {
            //Arrange
            ValidModel.TrainingCode = "TESTCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 6, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void IfTheTrainingProgrammeHasNotStartedThenShouldFailValidation()
        {
            //Arrange
            ValidModel.TrainingCode = "TESTCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 4, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors[0].ErrorMessage.EndsWith("after 04 2018"));
        }

        [Test]
        public void IfTheTrainingProgrammeHasExpiredThenShouldFailValidation()
        {
            //Arrange
            ValidModel.TrainingCode = "TESTCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 8, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors[0].ErrorMessage.EndsWith("before 08 2018"));
        }

        [Test]
        public void IfTrainingCodeIsNotSuppliedThenShouldNotCheckCourseValidity()
        {
            //Arrange
            ValidModel.TrainingCode = "";
            ValidModel.StartDate = new DateTimeViewModel(1, 4, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsTrue(result.IsValid);
            MockMediator.Verify(x=> x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Never);
        }

        [Test]
        public void AndStartDateIsBeforeAcademicYearThenInvalid()
        {
            //Arrange
            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 11, 5));
            ValidModel.TrainingCode = "OTHERCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 1, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("The earliest start date you can use is 08 2017"));
            Assert.That(result.Errors[0].ErrorCode, Is.EqualTo("AcademicYear_01"));
        }
    }
}
