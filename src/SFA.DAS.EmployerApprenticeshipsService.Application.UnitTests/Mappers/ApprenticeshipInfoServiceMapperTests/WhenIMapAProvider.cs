using System;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Mappers;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Mappers.ApprenticeshipInfoServiceMapperTests
{
    [TestFixture]
    public class WhenIMapAProvider
    {
        private ApprenticeshipInfoServiceMapper _mapper;
        private Apprenticeships.Api.Types.Providers.Provider _provider;
        private Func<ProvidersView> _act;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipInfoServiceMapper();

            _provider = new Apprenticeships.Api.Types.Providers.Provider
            {
                Ukprn = 12345678,
                ProviderName = "TestProvider"
            };

            _act = () => _mapper.MapFrom(TestHelper.Clone(_provider));
        }

        [Test]
        public void ThenUkprnIsMappedCorrectly()
        {
            var result = _act();

            Assert.AreEqual(_provider.Ukprn, result.Provider.Ukprn);
        }

        [Test]
        public void ThenProviderNameIsMappedCorrectly()
        {
            var result = _act();

            Assert.AreEqual(_provider.ProviderName, result.Provider.ProviderName);
        }
    }
}
