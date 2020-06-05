using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.HashingService;

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
        private Mock<IValidationApi> _validationApi;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _now = new DateTime(2018, 3, 1);
            _currentDateTime = new Mock<ICurrentDateTime>();
            _currentDateTime.Setup(x => x.Now).Returns(_now);

            _academicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));

            _validationApi = new Mock<IValidationApi>();
            _validationApi.Setup(x => x.ValidateOverlapping(It.IsAny<ApprenticeshipOverlapValidationRequest>()))
                .ReturnsAsync(new ApprenticeshipOverlapValidationResult());

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(123);

            _validator = new EditApprenticeshipStopDateViewModelValidator(_currentDateTime.Object, _academicYearDateProvider.Object, _validationApi.Object, _hashingService.Object);

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
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Enter the stop date for this apprenticeship")));
        }

        [Test]
        public void ThenDateMustBeValidDate()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(0,2,2018);
            var result = _validator.Validate(_viewModel);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Enter the stop date for this apprenticeship")));
        }

        [Test]
        public void ThenNewStopDateCannotBeInFuture()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(_now.AddMonths(1));
            var result = _validator.Validate(_viewModel);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("The stop date cannot be in the future")));
        }

        [Test]
        public void ThenDateCannotBeBeforeTrainingStartDate()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(_viewModel.ApprenticeshipStartDate.AddMonths(-1));

            var result = _validator.Validate(_viewModel);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("The stop month cannot be before the apprenticeship started")));
        }

        [Test]
        public void ThenNewStopDateCannotBeTheSameAsTheCurrentStopDate()
        {
            _viewModel.NewStopDate = new DateTimeViewModel(_viewModel.CurrentStopDate);

            var result = _validator.Validate(_viewModel);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("Enter a date that is different to the current stopped date")));
        }


        [Test]
        public void ThenNewStopDateCannotOverlapWithAnotherApprenticeshipWithTheSameUln()
        {
            _viewModel.CurrentStopDate = _viewModel.NewStopDate.DateTime.Value.AddMonths(-1);

            _validationApi.Setup(x => x.ValidateOverlapping(It.IsAny<ApprenticeshipOverlapValidationRequest>()))
                .ReturnsAsync(new ApprenticeshipOverlapValidationResult
                {
                    OverlappingApprenticeships = new List<OverlappingApprenticeship>
                    {
                        new OverlappingApprenticeship()
                    }
                });

            var result = _validator.Validate(_viewModel);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals("The date overlaps with existing dates for the same apprentice.")));
        }
    }
}
