using System;

using Moq;
using NUnit.Framework;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ApprovedApprenticeships
{
    public abstract class ApprovedApprenticeshipValidatorTestBase
    {
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();
        protected ApprovedApprenticeshipViewModelValidator Sut;
        protected ApprenticeshipViewModel ValidModel;

        protected UpdateApprenticeshipViewModel UpdatedModel;

        public int YearNow;

        [SetUp]
        public void BaseSetup()
        {
            YearNow = DateTime.Now.Year;
            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 5, 1));
            var academicYearProvider = new AcademicYearDateProvider(CurrentDateTime.Object);
            Sut = new ApprovedApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(),
                CurrentDateTime.Object,
                academicYearProvider,
                new AcademicYearValidator(CurrentDateTime.Object, academicYearProvider));

            UpdatedModel = new UpdateApprenticeshipViewModel();
            ValidModel = new  ApprenticeshipViewModel
            {
                ULN = "1001234567",
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                DateOfBirth = new DateTimeViewModel(8, 12, 1998),
                TrainingCode = "123-1-1",
                StartDate = new DateTimeViewModel(1, 8, DateTime.Now.Year),
                EndDate = new DateTimeViewModel(1, 8, DateTime.Now.Year ),
                Cost = "3000"
            };
        }
    }
}