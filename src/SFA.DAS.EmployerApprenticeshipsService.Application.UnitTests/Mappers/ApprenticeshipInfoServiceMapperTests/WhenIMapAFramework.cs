using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.EmployerCommitments.Application.Mappers;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Mappers.ApprenticeshipInfoServiceMapperTests
{
    [TestFixture]
    public class WhenIMapFramework
    {
        private ApprenticeshipInfoServiceMapper _mapper;
        private FrameworkSummary _framework;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipInfoServiceMapper();

            _framework = new FrameworkSummary
            {
                Id = "1",
                Title = "TestTitle",
                FrameworkName = "TestFrameworkName",
                PathwayName = "TestPathwayName",
                Level = 1,
                CurrentFundingCap = 1000, //this is to become redundant
                EffectiveFrom = new DateTime(2017, 05, 01),
                EffectiveTo = new DateTime(2020, 7, 31)
            };
        }

        [Test]
        public void ThenTitleIsMappedCorrectly()
        {
            //Act
            var result = _mapper.MapFrom(new List<FrameworkSummary> { CopyOf(_framework) });

            //Assert
            var expectedTitle = $"{_framework.Title}, Level: {_framework.Level}";
            Assert.AreEqual(expectedTitle, result.Frameworks[0].Title);
        }

        [Test]
        public void ThenEffectiveFromIsMappedCorrectly()
        {
            //Act
            var result = _mapper.MapFrom(new List<FrameworkSummary> { CopyOf(_framework) });

            //Assert
            Assert.AreEqual(_framework.EffectiveFrom, result.Frameworks[0].EffectiveFrom);
        }

        [Test]
        public void ThenEffectiveToIsMappedCorrectly()
        {
            //Act
            var result = _mapper.MapFrom(new List<FrameworkSummary> { CopyOf(_framework) });

            //Assert
            Assert.AreEqual(_framework.EffectiveFrom, result.Frameworks[0].EffectiveFrom);
        }

        private static FrameworkSummary CopyOf(FrameworkSummary source)
        {
            //copy the payload to guard against handler modifications
            return JsonConvert.DeserializeObject<FrameworkSummary>(JsonConvert.SerializeObject(source));
        }
    }
}
