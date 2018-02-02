using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.FeatureToggleTests
{
    [TestFixture]
    public class WhenIGetASimpleFeatureToggle
    {
        private IFeatureToggleService _featureToggleService;
        private Mock<IBooleanToggleValueProvider> _booleanToggleValueProvider;

        [SetUp]
        public void Arrange()
        {
            _booleanToggleValueProvider = new Mock<IBooleanToggleValueProvider>();
            _featureToggleService = new FeatureToggleService(_booleanToggleValueProvider.Object);
        }

        [Test]
        public void ThenTheRequestFeatureToggleIsReturned()
        {
            //Arrange
            _booleanToggleValueProvider.Setup(x => x.EvaluateBooleanToggleValue(It.IsAny<IFeatureToggle>()))
                .Returns(true);

            //Act
            var flag = _featureToggleService.Get<DummyFeatureToggle>();

            //Assert
            Assert.IsNotNull(flag);
            Assert.IsInstanceOf<DummyFeatureToggle>(flag);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ThenItsValueIsRetrievedFromTheValueProvider(bool flagValue)
        {
            //Arrange
            _booleanToggleValueProvider.Setup(x => x.EvaluateBooleanToggleValue(It.IsAny<IFeatureToggle>()))
                .Returns(flagValue);

            //Act
            var flag = _featureToggleService.Get<DummyFeatureToggle>();

            //Assert
            Assert.AreEqual(flagValue, flag.FeatureEnabled);
        }
    }
}
