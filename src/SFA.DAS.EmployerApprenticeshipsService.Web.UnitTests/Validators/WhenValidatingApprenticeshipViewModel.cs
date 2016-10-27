﻿using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Validators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Validators
{
    [TestFixture]
    public sealed class WhenValidatingApprenticeshipViewModel
    {
        private ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator();

        [Test]
        public void ULNMustBeNumericAnd10DigitsInLength()
        {
            var viewModel = new ApprenticeshipViewModel { ULN = "1001234567" };

            var result = _validator.Validate(viewModel);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        public void ULNThatIsNotNumericOr10DigitsInLengthIsIvalid(string uln)
        {
            var viewModel = new ApprenticeshipViewModel {  ULN = uln };

            var result = _validator.Validate(viewModel);

            Assert.That(result.IsValid, Is.False);
        }

        public void ULNThatStartsWithAZeroIsInvalid()
        {
            var viewModel = new ApprenticeshipViewModel { ULN = "0123456789" };

            var result = _validator.Validate(viewModel);

            Assert.That(result.IsValid, Is.False);
        }

        [TestCase("123")]
        [TestCase("1")]
        public void CostIsWholeNumberGreaterThanZeroIsValid(string cost)
        {
            var viewModel = new ApprenticeshipViewModel { Cost = cost };

            var result = _validator.Validate(viewModel);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("123.12")]
        [TestCase("123.1")]
        [TestCase("123.0")]
        [TestCase("fdsfdfd")]
        [TestCase("123.000")]
        public void CostNotNumericOrIsNotAWholeNumber(string cost)
        {
            var viewModel = new ApprenticeshipViewModel { Cost = cost };

            var result = _validator.Validate(viewModel);

            Assert.That(result.IsValid, Is.False);
        }

        [TestCase("0")]
        [TestCase("-0")]
        [TestCase("-123.12")]
        [TestCase("-123")]
        [TestCase("-123.1232")]
        [TestCase("-0.001")]
        public void CostThatIsZeroOrNegativeNumberIsInvalid(string cost)
        {
            var viewModel = new ApprenticeshipViewModel { Cost = cost };

            var result = _validator.Validate(viewModel);

            Assert.That(result.IsValid, Is.False);
        }
    }
}
