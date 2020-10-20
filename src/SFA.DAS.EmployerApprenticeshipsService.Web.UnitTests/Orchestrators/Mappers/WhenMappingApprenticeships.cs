using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeships : ApprenticeshipMapperBase
    {

        private const string TestChangeOfProviderLink = "https://commitments.apprenticehips.gov.uk/apprentice/change-training-provider";

        [Test]
        public void ThenMappingAnApprenticeshipUpdateShouldReturnAResult()
        {
            var update = new ApprenticeshipUpdate();

            Assert.DoesNotThrow(() => Sut.MapFrom(update));
        }

        [Test]
        public void TheMappingAnEmptyUpdateShouldReturnNull()
        {
            Assert.IsNull(Sut.MapFrom((ApprenticeshipUpdate)null));
        }

        [Test]
        public void AndChangeOfProviderFeatureToggleIsEnabled_ThenChangeOfProviderLinkIsSet()
        {
            MockChangeOfProviderToggle.Setup(c => c.FeatureEnabled).Returns(true);
            MockFeatureToggleService.Setup(f => f.Get<ChangeOfProvider>()).Returns(MockChangeOfProviderToggle.Object);

            MockLinkGenerator.Setup(l => l.CommitmentsV2Link(It.IsAny<string>())).Returns(TestChangeOfProviderLink);

            var result = Sut.MapToApprenticeshipDetailsViewModel(new Apprenticeship());

            Assert.AreEqual(TestChangeOfProviderLink, result.ChangeProviderLink);
        }

        [Test]
        public void AndChangeOfProviderFeatureToggleIsDisabled_ThenChangeOfProviderLinkIsSetToEmpty()
        {
            MockChangeOfProviderToggle.Setup(c => c.FeatureEnabled).Returns(false);
            MockFeatureToggleService.Setup(f => f.Get<ChangeOfProvider>()).Returns(MockChangeOfProviderToggle.Object);

            MockLinkGenerator.Setup(l => l.CommitmentsV2Link(It.IsAny<string>())).Returns(TestChangeOfProviderLink);

            var result = Sut.MapToApprenticeshipDetailsViewModel(new Apprenticeship());

            Assert.AreEqual(string.Empty, result.ChangeProviderLink);
        }
    }
}