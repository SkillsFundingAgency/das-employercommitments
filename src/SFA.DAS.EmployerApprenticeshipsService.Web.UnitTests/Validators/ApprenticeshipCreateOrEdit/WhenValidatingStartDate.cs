using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.EndsWith("after 04 18"));
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.EndsWith("before 08 18"));
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
    }
}
