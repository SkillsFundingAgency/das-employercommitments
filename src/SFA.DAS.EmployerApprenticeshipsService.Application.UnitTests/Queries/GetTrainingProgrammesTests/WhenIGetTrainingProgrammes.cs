using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
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

        private List<TrainingProgramme> _standards;
        private List<TrainingProgramme> _all;

        [SetUp]
        public void Arrange()
        {
            _standards = new List<TrainingProgramme>
            {
                new TrainingProgramme
                {
                    CourseCode = "123",
                    EffectiveFrom = new DateTime(2016,01,01),
                    EffectiveTo = new DateTime(2016,12,31)
                }
            };
            _all = new List<TrainingProgramme>
            {
                new TrainingProgramme
                {
                    CourseCode = "123avc",
                    EffectiveFrom = new DateTime(2017,01,01),
                    EffectiveTo = new DateTime(2017,12,31)
                }
            };
            _all.AddRange(_standards);

            _apprenticeshipInfoServiceWrapper = new Mock<IApprenticeshipInfoService>();

            _apprenticeshipInfoServiceWrapper.Setup(x => x.GetAll(It.IsAny<bool>()))
                .ReturnsAsync(new AllTrainingProgrammesView
                {
                    CreatedDate = DateTime.UtcNow,
                    TrainingProgrammes = _all
                });

            _apprenticeshipInfoServiceWrapper.Setup(x => x.GetStandards(It.IsAny<bool>()))
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
                IncludeFrameworks = false,
                EffectiveDate = null
            });

            Assert.AreEqual(_standards.Count, result.TrainingProgrammes.Count);
            result.TrainingProgrammes[0].ShouldBeEquivalentTo(_standards[0]);
        }

        [Test]
        public async Task ThenStandardsAndFrameworksAreReturnedIfIIncludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            });

            Assert.AreEqual(_all.Count, result.TrainingProgrammes.Count);
            result.TrainingProgrammes.ShouldBeEquivalentTo(_all);
        }

        [Test]
        public async Task ThenIfISpecifyAnEffectiveDateIOnlyGetCoursesActiveOnThatDay()
        {
            var result = await _handler.Handle(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = new DateTime(2016,06,01)
            });

            Assert.AreEqual(1, result.TrainingProgrammes.Count);
            result.TrainingProgrammes[0].ShouldBeEquivalentTo(_standards[0]);
        }

        [Test]
        public async Task ThenIfIDoNotSpecifyAnEffectiveDateIGetAllCourses()
        {
            var result = await _handler.Handle(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            });

            Assert.AreEqual(2, result.TrainingProgrammes.Count);
        }
    }
}
