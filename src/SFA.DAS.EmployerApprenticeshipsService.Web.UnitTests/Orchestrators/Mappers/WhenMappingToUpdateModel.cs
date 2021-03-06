﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingToUpdateModel : ApprenticeshipMapperBase
    {
        [Test]
        public async Task TwoEmptyModelIsEqueal()
        {
            var model = await Sut.CompareAndMapToApprenticeshipViewModel(new Apprenticeship(), new ApprenticeshipViewModel());

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmployerRef.Should().BeNull();
        }

        [Test]
        public async Task UpdateEveryField()
        {
            var a = new Apprenticeship
                {
                    FirstName = "Kalle",
                    LastName = "Abba",
                    EmployerRef = "This is a reference",
                    Cost = 4.0M
                };

            var updated = new ApprenticeshipViewModel
                              {
                                  FirstName = "Fredrik",
                                  LastName = "Stockborg",
                                  EmployerRef = "New ref",
                                  Cost = "5"
                              };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a , updated);

            model.FirstName.Should().Be("Fredrik");
            model.LastName.Should().Be("Stockborg");
            model.EmployerRef.Should().Be("New ref");
            model.Cost.Should().Be("5");
        }

        [TestCase(1.5, "1600")]
        [TestCase(1, "1.5")]
        [TestCase(1600, "1500")]
        [TestCase(1600, "1700")]
        [TestCase(1600, "")]
        public async Task AndUpdatingCostField(double current, string updatedCost)
        {
            var a = new Apprenticeship { Cost = new decimal(current) };

            var updated = new ApprenticeshipViewModel { Cost = updatedCost };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);
            model.Cost.Should().Be(updatedCost);
        }

        [Test]
        public async Task ShouldUpdateDate()
        {
            var a = new Apprenticeship
            {
                DateOfBirth = new DateTime(1990, 11, 11),
                StartDate = new DateTime(2045, 12, 08),
                EndDate = new DateTime(2046, 12, 08)
            };

            var dob = new DateTimeViewModel(8, 12, 1998);
            var sd = new DateTimeViewModel(08, 5, 2044);
            var ed = new DateTimeViewModel(09, 12, 2047);
            var updated = new ApprenticeshipViewModel
            {
                DateOfBirth = dob,
                StartDate = sd,
                EndDate = ed
            };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.DateOfBirth.Should().Be(dob);
            model.StartDate.Should().Be(sd);
            model.EndDate.Should().Be(ed);
        }

        [Test]
        public async Task ShouldNotUpdateDatesIfNotChanged()
        {
            var a = new Apprenticeship
            {
                DateOfBirth = new DateTime(1998, 12, 8),
                StartDate = new DateTime(2045, 12, 08),
                EndDate = new DateTime(2046, 12, 08)
            };

            var dob = new DateTimeViewModel(8, 12, 1998);
            var sd = new DateTimeViewModel(08, 12, 2045);
            var ed = new DateTimeViewModel(08, 12, 2046);
            var updated = new ApprenticeshipViewModel
            {
                DateOfBirth = dob,
                StartDate = sd,
                EndDate = ed
            };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.DateOfBirth.Should().BeNull();
            model.StartDate.Should().BeNull();
            model.EndDate.Should().BeNull();
        }

        [Test]
        public async Task ShouldNotUpdateTrainngCode()
        {
            var a = new Apprenticeship
            {
                TrainingCode = "abba-1234"
            };

            var updated = new ApprenticeshipViewModel
            {
                TrainingCode = "abba-1234"
            };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.TrainingCode.Should().BeNull();
            model.TrainingName.Should().BeNull();
            model.TrainingType.Should().BeNull();
        }

        [Test]
        public async Task ShouldUpdateTrainingProgramForFramework()
        {
            var a = new Apprenticeship
            {
                TrainingCode = "abba-666"
            };

            var updated = new ApprenticeshipViewModel
            {
                TrainingCode = "abba-555"
            };

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(
                    new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<TrainingProgramme>
                                    {
                        new TrainingProgramme { CourseCode = "abba-555",  Name = "Framework Title"},
                        new TrainingProgramme { CourseCode = "666" }
                                    }
                    });

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.TrainingCode.Should().Be("abba-555");
            model.TrainingName.Should().Be("Framework Title");
            model.TrainingType.Should().Be(TrainingType.Framework);
        }

        [Test]
        public async Task ShouldUpdateTrainngProgramForStandard()
        {
            var a = new Apprenticeship
            {
                TrainingCode = "abba-666"
            };

            var updated = new ApprenticeshipViewModel
            {
                TrainingCode = "007"
            };

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(
                    new GetTrainingProgrammesQueryResponse
                    {
                        TrainingProgrammes = new List<TrainingProgramme>
                                    {
                        new TrainingProgramme { CourseCode = "abba-555", Name = "Framework Title"},
                        new TrainingProgramme { CourseCode = "007", Name = "Standard Title", }
                                    }
                    });

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.TrainingCode.Should().Be("007");
            model.TrainingName.Should().Be("Standard Title");
            model.TrainingType.Should().Be(TrainingType.Standard);
        }

        [Test]
        public async Task UpdateReferenceFieldWhenEmpty()
        {
            var a = new Apprenticeship
            {
                FirstName = "Kalle",
                LastName = "Abba",
                EmployerRef = "This is a reference",
                Cost = 4.0M
            };

            var updated = new ApprenticeshipViewModel
            {
                EmployerRef = "",
            };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmployerRef.Should().Be("");
            model.Cost.Should().BeNull();
        }

        [Test]
        public async Task ShouldNotUpdateRefIfNotChanged()
        {
            var a = new Apprenticeship
            {
                FirstName = "Kalle",
                LastName = "Abba",
                EmployerRef = "Hello",
                Cost = 4.0M
            };

            var updated = new ApprenticeshipViewModel
            {
                EmployerRef = null,
            };

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmployerRef.Should().Be("");
            model.Cost.Should().BeNull();
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ShouldMapHasHadDataLockSuccess(bool expectedHasHadDataLockSuccess, bool hasHadDataLockSuccess)
        {
            var apprenticeship = new Apprenticeship
            {
                HasHadDataLockSuccess = hasHadDataLockSuccess
            };

            var updated = new ApprenticeshipViewModel();

            var model = await Sut.CompareAndMapToApprenticeshipViewModel(apprenticeship, updated);

            model.HasHadDataLockSuccess.Should().Be(expectedHasHadDataLockSuccess);
        }
    }
}
