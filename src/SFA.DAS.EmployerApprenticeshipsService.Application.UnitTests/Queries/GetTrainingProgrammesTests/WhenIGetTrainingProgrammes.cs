using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetTrainingProgrammesTests
{
    [TestFixture]
    public class WhenIGetTrainingProgrammes
    {
        private GetTrainingProgrammesQueryHandler _handler;

        private Mock<IApprenticeshipInfoService> _apprenticeshipInfoServiceWrapper;

        private List<Standard> _standards;
        private List<Framework> _frameworks;

        [SetUp]
        public void Arrange()
        {
            _standards = new List<Standard>
            {
                new Standard()
            };
            _frameworks = new List<Framework>
            {
                new Framework()
            };

            _apprenticeshipInfoServiceWrapper = new Mock<IApprenticeshipInfoService>();

            _apprenticeshipInfoServiceWrapper.Setup(x => x.GetFrameworksAsync(It.IsAny<bool>()))
                .ReturnsAsync(new FrameworksView
                {
                    CreatedDate = DateTime.UtcNow,
                    Frameworks = _frameworks
                });

            _apprenticeshipInfoServiceWrapper.Setup(x => x.GetStandardsAsync(It.IsAny<bool>()))
                .ReturnsAsync(new StandardsView
                {
                    CreationDate = DateTime.UtcNow.Date,
                    Standards = _standards
                });

            _handler = new GetTrainingProgrammesQueryHandler(_apprenticeshipInfoServiceWrapper.Object);
        }

        [Test]
        public async Task ThenOnlyStandardsAreReturnedIfIExcludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = false
            });

            Assert.AreEqual(_standards.Count, result.TrainingProgrammes.Count);
            Assert.IsInstanceOf<Standard>(result.TrainingProgrammes[0]);
        }

        [Test]
        public async Task ThenStandardsAndFrameworksAreReturnedIfIIncludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true
            });

            Assert.AreEqual(_standards.Count + _frameworks.Count, result.TrainingProgrammes.Count);
            Assert.IsTrue(result.TrainingProgrammes.Any(x => x is Standard));
            Assert.IsTrue(result.TrainingProgrammes.Any(x => x is Framework));
        }
    }
}
