using System;
using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();
        protected Mock<IMediator> MockMediator;

        protected ApprenticeshipViewModelValidator Validator;
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            MockMediator = new Mock<IMediator>();
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse
                {
                    TrainingProgrammes = new List<ITrainingProgramme>
                    {
                        new Standard
                        {
                            Id = "TESTCOURSE",
                            EffectiveFrom = new DateTime(2018, 5, 1),
                            EffectiveTo = new DateTime(2018, 7, 1)
                        },
                        new Standard
                        {
                            Id = "OTHERCOURSE",
                            EffectiveFrom = new DateTime(2017, 1, 1)
                        }
                    }
                });

            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2017,11,5));
            var academicYearProvider = new AcademicYearDateProvider(CurrentDateTime.Object);
            Validator = new ApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(academicYearProvider),
                academicYearProvider,
                new AcademicYearValidator(CurrentDateTime.Object, academicYearProvider),
                CurrentDateTime.Object,
                MockMediator.Object);

            ValidModel = new ApprenticeshipViewModel
            {
                ULN = "1001234567",
                FirstName = "TestFirstName",
                LastName = "TestLastName"
            };
        }
    }
}
