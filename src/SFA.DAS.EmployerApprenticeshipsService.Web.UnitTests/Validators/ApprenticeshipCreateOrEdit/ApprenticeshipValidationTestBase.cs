﻿using System;
using Moq;
using NUnit.Framework;
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

        protected ApprenticeshipViewModelValidator Validator;
        protected ApprenticeshipViewModel ValidModel;

        protected int YearNow;

        [SetUp]
        public void BaseSetup()
        {
            YearNow = DateTime.Now.Year;
            CurrentDateTime.Setup(x => x.Now).Returns(DateTime.Now.AddMonths(6));
            var academicYearProvider = new AcademicYearDateProvider(CurrentDateTime.Object);
            Validator = new ApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(academicYearProvider), 
                CurrentDateTime.Object, 
                academicYearProvider, 
                new AcademicYearValidator(CurrentDateTime.Object, academicYearProvider));

            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }
    }
}
