using System;
using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
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

        protected int YearNow;

        [SetUp]
        public void BaseSetup()
        {
            MockMediator = new Mock<IMediator>();
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse
                {
                    TrainingProgrammes = new List<TrainingProgramme>
                    {
                        new TrainingProgramme
                        {
                            CourseCode = "TESTCOURSE",
                            EffectiveFrom = new DateTime(2018, 5, 1),
                            EffectiveTo = new DateTime(2018, 7, 1)
                        }
                    }
                });

            YearNow = DateTime.Now.Year;
            CurrentDateTime.Setup(x => x.Now).Returns(DateTime.Now.AddMonths(6));
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
