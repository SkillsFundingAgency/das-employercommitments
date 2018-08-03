using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Mappers;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Mappers.ApprenticeshipInfoServiceMapperTests
{
    [TestFixture]
    public class WhenIMapAProvider
    {
        private ApprenticeshipInfoServiceMapper _mapper;
        private Apprenticeships.Api.Types.Providers.Provider _provider;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipInfoServiceMapper();

            _provider = new Apprenticeships.Api.Types.Providers.Provider
            {
                Ukprn = 12345678,
                ProviderName = "TestProvider"
            };
        }

        [Test]
        public void ThenUkprnIsMappedCorrectly()
        {
            var result = _mapper.MapFrom(CopyOf(_provider));

            Assert.AreEqual(_provider.Ukprn, result.Provider.Ukprn);
        }

        [Test]
        public void ThenProviderNameIsMappedCorrectly()
        {
            var result = _mapper.MapFrom(CopyOf(_provider));

            Assert.AreEqual(_provider.ProviderName, result.Provider.ProviderName);
        }

        private Apprenticeships.Api.Types.Providers.Provider CopyOf(Apprenticeships.Api.Types.Providers.Provider source)
        {
            return JsonConvert.DeserializeObject<Apprenticeships.Api.Types.Providers.Provider>(
                    JsonConvert.SerializeObject(source)
                );
        }
    }
}
