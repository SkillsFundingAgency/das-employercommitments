using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeships : ApprenticeshipMapperBase
    {

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
         
            var result = Sut.MapToApprenticeshipDetailsViewModel(new Apprenticeship());

            Assert.AreEqual(TestChangeOfProviderLink, result.ChangeProviderLink);
        }

        [Test]
        public void AndChangeOfProviderFeatureToggleIsDisabled_ThenChangeOfProviderLinkIsSetToEmpty()
        {
            MockChangeOfProviderToggle.Setup(c => c.FeatureEnabled).Returns(false);
            
            var result = Sut.MapToApprenticeshipDetailsViewModel(new Apprenticeship());

            Assert.AreEqual(string.Empty, result.ChangeProviderLink);
        }
    }
}