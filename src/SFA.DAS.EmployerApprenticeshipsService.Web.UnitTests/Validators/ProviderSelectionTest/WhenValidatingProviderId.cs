using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using FluentAssertions;
using FluentValidation;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ProviderSelectionTest
{
    [TestFixture]
    public class WhenValidatingProviderId
    {
        private SelectProviderViewModelValidator _validator;
        private SelectProviderViewModel _viewModel;
        private const string _validatorRuleset = "Request";

        [SetUp]
        public void Setup()
        {
            _validator = new SelectProviderViewModelValidator();

            _viewModel = new SelectProviderViewModel
            {
                CohortRef = "10000",
                LegalEntityCode = "123456",
            };

        }

        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase("9999999999")]
        public void UKPNThatIsNotNumericOr8DigitsInLengthIsInvalid(string ukpn)
        {
            _viewModel.ProviderId = ukpn;

            var result = _validator.Validate(_viewModel , ruleSet: _validatorRuleset);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void UKPNThatStartsWithAZeroIsInvalid()
        {
            _viewModel.ProviderId = "012345678";

            var result = _validator.Validate(_viewModel, ruleSet : _validatorRuleset);

            result.IsValid.Should().BeFalse();
        }
        [Test]
        public void ValidUKPNShouldBeValid()
        {
            _viewModel.ProviderId = "12345670";

            var result = _validator.Validate(_viewModel, ruleSet: _validatorRuleset);

            result.IsValid.Should().BeTrue();
        }

    }
}
