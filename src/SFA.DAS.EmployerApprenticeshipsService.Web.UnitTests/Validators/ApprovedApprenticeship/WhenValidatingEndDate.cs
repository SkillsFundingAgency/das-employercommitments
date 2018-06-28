using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.ApprovedApprenticeship
{
    [TestFixture]
    public class WhenValidatingEndDate
    {
        private IValidateApprovedApprenticeship _validator;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IAcademicYearValidator> _mockAcademicYearValidator;
        private UpdateApprenticeshipViewModel _updateApprenticeshipViewModel;

        private const string FieldName = "EndDate";

        //todo: also in provider
        [SetUp]
        public void BaseSetup()
        {
            _mockAcademicYearValidator = new Mock<IAcademicYearValidator>();

            _currentDateTime = new Mock<ICurrentDateTime>();
            var academicYearProvider = new AcademicYearDateProvider(_currentDateTime.Object);

            _updateApprenticeshipViewModel = new UpdateApprenticeshipViewModel();

            _validator = new ApprovedApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(academicYearProvider),
                academicYearProvider,
                _mockAcademicYearValidator.Object,
                _currentDateTime.Object);
        }

        [TestCase(1, 6, 2019, 1, 7, 2019)]
        [TestCase(1, 6, 2019, 1, 8, 2019)]
        public void ShouldFailValidationWhenEndDateMonthInFuture(
            int nowDay,  int nowMonth,  int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            const string expected = "The end date must not be in the future";

            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _updateApprenticeshipViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_updateApprenticeshipViewModel);

            Assert.IsTrue(result.ContainsKey(FieldName));
            Assert.AreEqual(expected, result[FieldName]);
        }

        [TestCase(1, 6, 2019, 1, 6, 2019)]
        [TestCase(1, 6, 2019, 1, 5, 2019)]
        [TestCase(15, 6, 2019, 1, 6, 2019)]
        [TestCase(1, 6, 2019, 15, 6, 2019)]
        public void ShouldPassValidationWhenEndDateIsCurrentMonthOrInPast(
            int nowDay, int nowMonth, int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _updateApprenticeshipViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_updateApprenticeshipViewModel);

            Assert.IsFalse(result.ContainsKey(FieldName));
        }
    }
}
