using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Validators.EditApprenticeshipStopDate
{
    [TestFixture]
    public class WhenValidatingNewStopDate
    {
        private EditApprenticeshipStopDateViewModelValidator _validator;
        private Mock<ICurrentDateTime> _currentDateTime;
        private DateTime _now;
        private EditApprenticeshipStopDateViewModel _viewModel;
        private Mock<IAcademicYearDateProvider> _academicYearDateProvider;

        [SetUp]
        public void Arrange()
        {
            _now = new DateTime(2018, 3, 1);
            _currentDateTime = new Mock<ICurrentDateTime>();
            _currentDateTime.Setup(x => x.Now).Returns(_now);

            _academicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));

            _validator = new EditApprenticeshipStopDateViewModelValidator(_currentDateTime.Object, _academicYearDateProvider.Object);

            _viewModel = new EditApprenticeshipStopDateViewModel
            {
                ApprenticeshipName = "Test Apprenticeship",
                ApprenticeshipULN = "1234567890",
                ApprenticeshipStartDate = new DateTime(2017,9,1),
                CurrentStopDate = new DateTime(2018,2,1),
                AcademicYearRestriction = new DateTime(2017,8,1),
                NewStopDate = new DateTimeViewModel(new DateTime(2017,9,1))
            };
        }

        [Test]
        public void ThenValidationSucceedsIfValidRequest()
        {
            var result = _validator.Validate(_viewModel);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ThenDateIsRequired()
        {
            _viewModel.NewStopDate = null;
            var result = _validator.Validate(_viewModel);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Date is not valid")));
        }

        [Test]
        public void ThenDateMustBeValidDate()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(0,2,2018);
            var result = _validator.Validate(_viewModel);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Date is not valid")));
        }

        [Test]
        public void ThenNewStopDateCannotBeInFuture()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(_now.AddMonths(1));
            var result = _validator.Validate(_viewModel);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("New stop date must not be in future")));
        }

        [Test]
        public void ThenDateCannotBeBeforeAcademicYearRestrictionDate()
        {
            //if training started in last a.y., and r14 has passed, stop date can't be before start of current a.y.
            _viewModel.ApprenticeshipStartDate = new DateTime(2017,6,1);
            _viewModel.NewStopDate = new DateTimeViewModel(1, 6, 2017);

            var result = _validator.Validate(_viewModel);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("The earliest date you can stop this apprenticeship is 01 08 2017")));
        }

        [Test]
        public void ThenDateCannotBeBeforeTrainingStartDate()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(_viewModel.ApprenticeshipStartDate.AddMonths(-1));

            var result = _validator.Validate(_viewModel);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Date cannot be earlier than training start date")));
        }

        [Test]
        public void ThenDateMustBeBeforeCurrentStopDate()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(_viewModel.CurrentStopDate.AddMonths(1));

            var result = _validator.Validate(_viewModel);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Date must be before current stop date")));
        }
    }
}
