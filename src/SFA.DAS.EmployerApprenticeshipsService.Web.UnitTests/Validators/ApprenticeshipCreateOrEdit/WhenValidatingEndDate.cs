using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingEndDate : ApprenticeshipValidationTestBase
    {
        [TestCase(1, 6, 2018, 1, 5, 2018)]
        [TestCase(1, 6, 2018, 1, 6, 2018)]
        public void ShouldFailValidationWhenEndDateBeforeStartDate(
            int? startDay, int? startMonth, int? startYear,
            int? endDay, int? endMonth, int? endYear)
        {
            var expected = "The end date must not be on or before the start date";
            ValidModel.StartDate = new DateTimeViewModel(startDay, startMonth, startYear);
            ValidModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }
    }
}
